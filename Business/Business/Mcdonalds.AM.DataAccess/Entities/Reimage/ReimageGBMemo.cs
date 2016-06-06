using AutoMapper;
using Mcdonalds.AM.DataAccess.Common.Extensions;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Infrastructure;
using NTTMNC.BPM.Fx.Core.Json;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ReimageGBMemo : BaseWFEntity<ReimageGBMemo>
    {
        public List<ProjectComment> ProjectComments { get; set; }
        public string Comments { get; set; }
        public string Action { get; set; }
        public string SerialNumber { get; set; }
        public bool IsShowRecall { get; set; }
        public bool IsShowEdit { get; set; }
        public bool IsShowSave { get; set; }
        public ApproveUsers AppUsers { get; set; }
        public StoreInfo Store { get; set; }
        public ReinvestmentBasicInfo ReinvestInfo { get; set; }
        public ReimageInfo rmgInfo { get; set; }
        public override string WorkflowProcessName { get { return @"MCDAMK2Project.Reimage\ReimageGBMemo"; } }
        public override string WorkflowProcessCode { get { return @"MCD_AM_Reimage_GBM"; } }
        public override string TableName { get { return "ReimageGBMemo"; } }
        public static string WorkflowActOriginator { get { return "Originator"; } } // 退回至发起人节点名
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public override string WorkflowCode
        {
            get { return FlowCode.Reimage_GBMemo; }
        }
        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            var entity = GetGBMemo(projectId, id);
            if (entity != null)
            {
                entity.EntityId = entity.Id;
            }
            return entity;
        }
        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_ConstructionManager",AppUsers.ConstructionManager.Code ),
                new ProcessDataField("ProcessCode", WorkflowProcessCode)
            };

            if (task != null)
            {
                processDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                processDataFields.Add(new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task)));
            }
            return processDataFields;
        }
        public override void GenerateDefaultWorkflowInfo(string projectId)
        {
            var entity = new ReimageGBMemo
            {
                ProjectId = projectId,
                Id = Guid.NewGuid(),
                CreateTime = DateTime.Now,
                LastUpdateTime = CreateTime,
                CreateUserAccount = ClientCookie.UserCode,
                CreateUserNameZHCN = ClientCookie.UserNameENUS,
                CreateUserNameENUS = ClientCookie.UserNameZHCN,
                IsHistory = false
            };
            Add(entity);
        }
        public override string Edit()
        {
            var taskUrl = string.Format("/Reimage/Main#/GBMemo?projectId={0}", ProjectId);
            using (var scope = new TransactionScope())
            {
                var rmgInfo = ReimageInfo.Search(e => e.ProjectId.Equals(ProjectId)).FirstOrDefault();
                if (rmgInfo == null)
                {
                    throw new Exception("Could not find the Reimage Info, please check it!");
                }
                var task = rmgInfo.GenerateTaskWork(WorkflowCode, "Reimage_GBMemo", "Reimage_GBMemo", taskUrl);
                task.ActivityName = NodeCode.Start;
                task.ActionName = SetTaskActionName(ProjectId);
                TaskWork.Add(task);
                ProjectInfo.Reset(ProjectId, WorkflowCode);

                if (IsInOperation)
                {
                    ProjectInfo.Reset(ProjectId, FlowCode.Reimage_ReopenMemo);
                    ProjectInfo.Reset(ProjectId, FlowCode.Reimage_TempClosureMemo);
                }
                else
                    CompleteNotFinishTask();
                CompleteNotifyTask(ProjectId);
                IsHistory = true;
                Update(this);

                Mapper.CreateMap<ReimageGBMemo, ReimageGBMemo>();
                var entity = Mapper.Map<ReimageGBMemo>(this);
                entity.Id = Guid.Empty;
                entity.ProcInstID = null;
                entity.IsHistory = false;
                entity.Comments = null;
                entity.CreateTime = DateTime.Now;
                entity.Save();

                CopyAppUsers(Id.ToString(), entity.Id.ToString());
                scope.Complete();
            }

            return taskUrl;

        }
        public void CompleteNotFinishTask()
        {
            var tasks = TaskWork.Search(e => e.RefID == ProjectId
                && e.Status == TaskWorkStatus.UnFinish
                && (e.TypeCode == FlowCode.Reimage_TempClosureMemo || e.TypeCode == FlowCode.Reimage_ReopenMemo)).ToList();
            if (tasks.Count > 0)
            {
                foreach (var task in tasks)
                {
                    task.Status = TaskWorkStatus.Finished;
                    string strTypeCode = task.TypeCode.Split('_')[1];
                    task.Url = string.Format("/Reimage/Main#/{0}/Process/View?projectId={1}", strTypeCode, ProjectId);
                }
                TaskWork.Update(tasks.ToArray());
            }
        }
        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            using (var scope = new TransactionScope())
            {
                switch (status)
                {
                    case TaskWorkStatus.K2ProcessApproved:
                        var imgInfo = ReimageInfo.Search(e => e.ProjectId.Equals(ProjectId)).FirstOrDefault();
                        ProjectInfo.FinishNode(ProjectId, FlowCode.Reimage_GBMemo, NodeCode.Finish,ProjectStatus.Finished);
                        if (IsInOperation)
                        {
                            ProjectInfo.FinishNode(ProjectId, FlowCode.Reimage_ReopenMemo, NodeCode.Finish, ProjectStatus.Finished);
                            ProjectInfo.FinishNode(ProjectId, FlowCode.Reimage_TempClosureMemo, NodeCode.Finish, ProjectStatus.Finished);
                        }
                        else
                        {
                            var taskUrlReopen = string.Format(@"/Reimage/Main#/ReopenMemo?projectId={0}", ProjectId);
                            var taskReopen = imgInfo.GenerateTaskWork(FlowCode.Reimage_ReopenMemo, "Reopen Memo", "Reopen Memo", taskUrlReopen);
                            taskReopen.ActivityName = NodeCode.Start;
                            taskReopen.ActionName = SetTaskActionName(ProjectId);
                            //TaskWork.Add(taskReopen);
                            if (rmgInfo.ReopenDate.HasValue)
                                ScheduleLog.GenerateTaskSchedule(rmgInfo.ReopenDate.Value.AddDays(-7), taskReopen, ClientCookie.UserCode, ProjectId, FlowCode.Reimage_ReopenMemo, rmgInfo.USCode);
                            ProjectInfo.Reset(ProjectId, FlowCode.Reimage_ReopenMemo);

                            var taskUrlClosure = string.Format(@"/Reimage/Main#/TempClosureMemo?projectId={0}", ProjectId);
                            var taskClosure = imgInfo.GenerateTaskWork(FlowCode.Reimage_TempClosureMemo, "TempClosure Memo", "TempClosure Memo", taskUrlClosure);
                            taskClosure.ActivityName = NodeCode.Start;
                            taskClosure.ActionName = SetTaskActionName(ProjectId);
                            TaskWork.Add(taskClosure);
                            ProjectInfo.Reset(ProjectId, FlowCode.Reimage_TempClosureMemo);
                        }

                        var consCheckingTask = TaskWork.FirstOrDefault(e => e.RefID == ProjectId && e.TypeCode == FlowCode.Reimage_ConsInvtChecking && e.Status == TaskWorkStatus.UnFinish);
                        var checkingProj = ProjectInfo.FirstOrDefault(e => e.ProjectId == ProjectId && e.FlowCode == FlowCode.Reimage_ConsInvtChecking);
                        if (consCheckingTask == null && checkingProj != null &&
                            checkingProj.Status != ProjectStatus.Finished)
                        {
                            var consInvtChecking = new ReimageConsInvtChecking();
                            consInvtChecking.ProjectId = task.RefID;
                            consInvtChecking.GenerateConsInvtCheckingTask(true);
                        }

                        ProjectInfo.CompleteMainIfEnable(ProjectId);
                        var pmTaskUrl = string.Format(@"/Reimage/Main#/GBMemo/Process/Notify?projectId={0}", ProjectId);
                        var pmTask = imgInfo.GenerateTaskWork(WorkflowCode, "Reimage GBMemo", "Reimage GBMemo",pmTaskUrl);
                        pmTask.ActivityName = "Notify";
                        pmTask.ActionName = "Notify";
                        TaskWork.Add(pmTask);

                        GenerateSiteInfoTask();
                        break;
                    case TaskWorkStatus.K2ProcessDeclined:
                        ProjectInfo.UpdateProjectStatus(ProjectId, FlowCode.Reimage, ProjectStatus.Rejected);
                        ProjectInfo.UpdateProjectStatus(ProjectId, FlowCode.Reimage_GBMemo, ProjectStatus.Rejected);
                        break;
                }
                scope.Complete();
            }
        }

        public void GenerateSiteInfoTask()
        {
            var taskWork = new TaskWork();
            taskWork.SourceCode = FlowCode.Reimage;
            taskWork.SourceNameENUS = FlowCode.Reimage;
            taskWork.SourceNameZHCN = FlowCode.Reimage;
            taskWork.Status = TaskWorkStatus.UnFinish;
            taskWork.StatusNameZHCN = "任务";
            taskWork.StatusNameENUS = "任务";
            taskWork.RefID = ProjectId;
            taskWork.Id = Guid.NewGuid();
            taskWork.CreateTime = DateTime.Now;

            var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId == ProjectId);
            if (reimageInfo == null)
            {
                throw new Exception("Cannot find the relative reimage info!");
            }

            taskWork.Title = TaskWork.BuildTitle(ProjectId, reimageInfo.StoreNameZHCN, reimageInfo.StoreNameENUS);
            taskWork.TypeCode = FlowCode.Reimage_SiteInfo;
            taskWork.TypeNameENUS = FlowCode.Reimage_SiteInfo;
            taskWork.TypeNameZHCN = FlowCode.Reimage_SiteInfo;
            taskWork.ReceiverAccount = reimageInfo.PMAccount;
            taskWork.ReceiverNameENUS = reimageInfo.PMNameENUS;
            taskWork.ReceiverNameZHCN = reimageInfo.PMNameZHCN;
            taskWork.Url = string.Format(@"/Reimage/Main#/SiteInfo/Process/Approval?projectId={0}&action={1}", ProjectId, "Confirm");
            taskWork.StoreCode = reimageInfo.USCode;
            taskWork.ActivityName = NodeCode.Start;
            taskWork.ActionName = SetTaskActionName(ProjectId);
            TaskWork.Add(taskWork);
        }

        public static ReimageGBMemo GetGBMemo(string projectId, string entityId = "")
        {
            var memo = (string.IsNullOrEmpty(entityId) ?
                FirstOrDefault(e => e.ProjectId.Equals(projectId) && !e.IsHistory)
                : FirstOrDefault(e => e.Id.ToString().Equals(entityId))) ?? new ReimageGBMemo();

            memo.ProjectId = projectId;

            string usCode = "";
            var rmgInfo = ReimageInfo.GetReimageInfo(projectId);
            memo.rmgInfo = rmgInfo;
            usCode = rmgInfo.USCode;

            memo.Store = StoreBasicInfo.GetStore(usCode);
            var consInfo = new ReimageConsInfo();
            memo.ReinvestInfo = consInfo.GetReinvestmentBasicInfo(projectId);

            if (memo.Id == Guid.Empty)
            {
                memo.IsClosed = (memo.Store.StoreBasicInfo.statusName == "Closed");
                memo.IsInOperation = false;
                
                if (memo.ReinvestInfo != null)
                {
                    if ((memo.ReinvestInfo.NewAttachedKiosk.HasValue && memo.ReinvestInfo.NewAttachedKiosk.Value)
                        || (memo.ReinvestInfo.NewRemoteKiosk.HasValue && memo.ReinvestInfo.NewRemoteKiosk.Value))
                    {
                        memo.IsKiosk = true;
                    }
                    memo.IsMcCafe = memo.ReinvestInfo.NewMcCafe.HasValue && memo.ReinvestInfo.NewMcCafe.Value;
                    memo.IsMDS = memo.ReinvestInfo.NewMDS.HasValue && memo.ReinvestInfo.NewMDS.Value;
                    memo.Is24Hour = memo.ReinvestInfo.NewTwientyFourHour.HasValue &&
                                    memo.ReinvestInfo.NewTwientyFourHour.Value;
                    memo.GBDate = memo.ReinvestInfo.GBDate;
                    memo.ConstCompletionDate = memo.ReinvestInfo.ConsCompletionDate;
                    memo.ReopenDate = memo.ReinvestInfo.ReopenDate;
                }
                memo.Save();
            }
            else
            {
                var projectInfo = ProjectInfo.FirstOrDefault(e => e.ProjectId == projectId
                                                                   && e.FlowCode == FlowCode.Reimage_GBMemo);

                if (projectInfo != null)
                {
                    if (ClientCookie.UserCode.Equals(rmgInfo.PMAccount))
                    //if (reimageInfo != null)
                    {
                        var isExistTask = TaskWork.Any(e => e.RefID == projectId
                                                                    && e.TypeCode == FlowCode.Reimage_GBMemo
                                                                    && e.Status == TaskWorkStatus.UnFinish
                                                                    && e.ReceiverAccount == ClientCookie.UserCode
                                                                    &&
                                                                    e.ActivityName ==
                                                                    WorkflowActOriginator);
                        memo.IsShowEdit = projectInfo.Status == ProjectStatus.Finished && !isExistTask;
                        memo.IsShowRecall = CheckIfShowRecallByPojectStatus(projectInfo.Status) && !isExistTask;
                    }
                }
            }
            if (ClientCookie.UserCode.Equals(rmgInfo.PMAccount))
                memo.IsShowSave = ProjectInfo.IsFlowSavable(projectId, FlowCode.Reimage_GBMemo);
            PopulateAppUsers(memo); 
            return memo;
        }
        private static void PopulateAppUsers(ReimageGBMemo entity)
        {
            var approvedUsers = ApproveDialogUser.GetApproveDialogUser(entity.Id.ToString());
            entity.AppUsers = new ApproveUsers();
            if (approvedUsers != null)
            {
                var simp = new SimpleEmployee
                {
                    Code = approvedUsers.ConstructionManagerCode
                };
                entity.AppUsers.ConstructionManager = simp;
            }
            else
            {
                if (entity.rmgInfo != null)
                {
                    var simp = new SimpleEmployee
                    {
                        Code = entity.rmgInfo.CMAccount
                    };
                    entity.AppUsers.ConstructionManager = simp;
                }
            }
        }
        public void Submit()
        {
            var task = TaskWork.GetTaskWork(ProjectId, LastUpdateUserAccount, TaskWorkStatus.UnFinish, FlowCode.Reimage, FlowCode.Reimage_GBMemo);
            string taskUrl = "/Reimage/Main#/GBMemo/Process/View?projectId=" + ProjectId;
            task.Url = taskUrl;
            ProcInstID = StartProcess(task);
            using (var scope = new TransactionScope())
            {
                task.Status = TaskWorkStatus.Finished;
                task.FinishTime = DateTime.Now;
                TaskWork.Update(task);
                Save("Submit");
                ProjectInfo.FinishNode(ProjectId, FlowCode.Reimage_GBMemo, NodeCode.Reimage_GBMemo_Input);
                scope.Complete();
            }
        }
        public void Save(string action = "")
        {
            using (var scope = new TransactionScope())
            {
                LastUpdateTime = DateTime.Now;
                LastUpdateUserAccount = ClientCookie.UserCode;
                LastUpdateUserNameZHCN = ClientCookie.UserNameZHCN;
                LastUpdateUserNameENUS = ClientCookie.UserNameENUS;
                if (Id == Guid.Empty)
                {
                    Id = Guid.NewGuid();
                    IsHistory = false;
                    CreateTime = DateTime.Now;
                    CreateUserAccount = ClientCookie.UserCode;
                    CreateUserNameENUS = ClientCookie.UserNameENUS;
                    CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                    Add();
                }
                else
                {
                    Update();
                }
                SaveApproveUsers(action);
                SaveComments(action);
                scope.Complete();
            }
        }
        private void SaveApproveUsers(string actionName)
        {
            switch (actionName)
            {
                case ProjectAction.Submit:
                case ProjectAction.ReSubmit:
                    var approveUser = ApproveDialogUser.GetApproveDialogUser(Id.ToString());
                    if (approveUser == null)
                    {
                        approveUser = new ApproveDialogUser();
                        approveUser.Id = Guid.Empty;
                        approveUser.RefTableID = Id.ToString();
                        approveUser.ProjectId = ProjectId;
                        approveUser.FlowCode = FlowCode.Reimage_GBMemo;
                    }
                    approveUser.ConstructionManagerCode = AppUsers.ConstructionManager.Code;
                    approveUser.LastUpdateDate = DateTime.Now;
                    approveUser.LastUpdateUserAccount = ClientCookie.UserCode;
                    approveUser.Save();
                    break;
            }
        }
        private void SaveComments(string action)
        {
            var comment = ProjectComment.GetSavedComment(Id, TableName, LastUpdateUserAccount);
            if (comment != null)
            {
                comment.Content = Comments;
                comment.Action = string.IsNullOrEmpty(action) ? "Save" : action;
                comment.Status = string.IsNullOrEmpty(action) ? ProjectCommentStatus.Save : ProjectCommentStatus.Submit;
                comment.Update();
            }
            else
            {
                comment = new ProjectComment();
                comment.Action = string.IsNullOrEmpty(action) ? "Save" : action;
                comment.ActivityName = "";
                comment.Content = Comments;
                comment.CreateTime = DateTime.Now;
                comment.CreateUserAccount = LastUpdateUserAccount;
                comment.CreateUserNameENUS = LastUpdateUserNameENUS;
                comment.CreateUserNameZHCN = LastUpdateUserNameZHCN;
                comment.UserAccount = LastUpdateUserAccount;
                comment.UserNameENUS = LastUpdateUserNameENUS;
                comment.UserNameZHCN = LastUpdateUserNameZHCN;
                comment.RefTableId = Id;
                comment.Id = Guid.NewGuid();
                comment.RefTableName = TableName;
                comment.SourceCode = FlowCode.Reimage;
                comment.SourceNameZHCN = FlowCode.Reimage;
                comment.SourceNameENUS = FlowCode.Reimage;
                comment.TitleNameENUS = ClientCookie.TitleENUS;
                comment.TitleNameZHCN = ClientCookie.TitleENUS;
                comment.Status = string.IsNullOrEmpty(action) ? ProjectCommentStatus.Save : ProjectCommentStatus.Submit;
                comment.ProcInstID = ProcInstID;
                comment.Add();
            }
        }
        private int StartProcess(TaskWork task)
        {
            CreateUserAccount = LastUpdateUserAccount;
            var processDataFields = SetWorkflowDataFields(task);
            return K2FxContext.Current.StartProcess(WorkflowProcessCode, CreateUserAccount, processDataFields);
        }
        public override void Recall(string comment)
        {
            if (!ProcInstID.HasValue)
            {
                throw new Exception("The operation needs the process instance id!");
            }
            K2FxContext.Current.GoToActivityAndRecord(ProcInstID.Value, WorkflowActOriginator, ClientCookie.UserCode, ProjectAction.Recall, comment);
            using (var scope = new TransactionScope())
            {
                Save(ProjectAction.Recall);
                ProjectInfo.Reset(ProjectId, WorkflowCode, ProjectStatus.Recalled);
                scope.Complete();
            }
        }
        public void Approve(string currUserAccount)
        {
            ExecuteProcess(currUserAccount, SerialNumber, ProjectAction.Approve, Comments);
        }
        public void Reject(string currUserAccount)
        {
            ExecuteProcess(currUserAccount, SerialNumber, "Reject", Comments);
        }
        public void Return(string currUserAccount)
        {
            ExecuteProcess(currUserAccount, SerialNumber, ProjectAction.Return, Comments);
        }
        public void Resubmit(string userCode)
        {
            List<ProcessDataField> dataField = SetWorkflowDataFields(null);
            ExecuteProcess(userCode, SerialNumber, ProjectAction.ReSubmit, Comments, dataField);
        }
        public void ExecuteProcess(string employeeCode, string sn, string actionName, string comment, List<ProcessDataField> dataField = null)
        {
            using (var scope = new TransactionScope())
            {
                Save(actionName);
                switch (actionName)
                {
                    case ProjectAction.Return:
                    case ProjectAction.Recall:
                        ProjectInfo.Reset(ProjectId, WorkflowCode, GetProjectStatus(actionName));
                        break;
                    case ProjectAction.Decline:
                        ProjectInfo.Reject(ProjectId, WorkflowCode);
                        break;
                    default:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Reimage_GBMemo_Input, GetProjectStatus(actionName));
                        break;
                }
                scope.Complete();
            }
            K2FxContext.Current.ApprovalProcess(sn, employeeCode, actionName, comment, dataField);
        }
        private string GetNodeName(string actionName)
        {
            string nodeCode;
            switch (actionName)
            {
                case ProjectAction.Return:
                    nodeCode = NodeCode.Start;
                    break;
                default:
                    nodeCode = NodeCode.Reimage_GBMemo_SendMemo;
                    break;
            }
            return nodeCode;
        }
    }
}
