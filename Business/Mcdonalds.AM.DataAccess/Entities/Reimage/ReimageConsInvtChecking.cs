using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Mcdonalds.AM.DataAccess.Common.Extensions;
using Mcdonalds.AM.DataAccess.Constants;
using System.Transactions;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using System.Data.Entity;
using NTTMNC.BPM.Fx.Core;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ReimageConsInvtChecking : BaseWFEntity<ReimageConsInvtChecking>
    {
        public override string WorkflowCode
        {
            get { return FlowCode.Reimage_ConsInvtChecking; }
        }
        public ApproveUsers AppUsers { get; set; }

        public string ApprovalType { get; set; }

        public bool IsShowRecall { get; set; }

        public string SerialNumber { get; set; }

        public bool IsShowEdit { get; set; }

        public bool IsShowSave { get; set; }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == WFReimageConsInvtChecking.TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public string Comments { get; set; }

        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            var entity = GetConsInvtChecking(projectId, id);
            if (entity != null)
            {
                entity.EntityId = entity.Id;
            }
            return entity;
        }

        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            using (var scope = new TransactionScope())
            {
                switch (status)
                {
                    case TaskWorkStatus.K2ProcessApproved:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Finish);
                        ProjectInfo.CompleteMainIfEnable(ProjectId);
                        break;
                }
                scope.Complete();
            }
        }

        public ReimageConsInvtChecking GetConsInvtChecking(string strProjectId, string entityId = "")
        {
            ProjectId = strProjectId;
            ReimageConsInvtChecking entity = null;
            if (string.IsNullOrEmpty(entityId))
                entity = Search(e => e.ProjectId.Equals(strProjectId) && !e.IsHistory).FirstOrDefault();
            else
                entity = Search(e => e.Id.ToString().Equals(entityId)).FirstOrDefault();

            if (entity != null)
            {
                entity.IsProjectFreezed = CheckIfFreezeProject(strProjectId);
                entity.ApprovalType = entity.GetApprovalType();
                var projectInfo = ProjectInfo.FirstOrDefault(e => e.ProjectId == strProjectId
                                                               && e.FlowCode == WorkflowCode);

                if (projectInfo != null)
                {
                    var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId == strProjectId);

                    if (reimageInfo != null
                        && ClientCookie.UserCode.Equals(reimageInfo.PMAccount))
                    {
                        var isExistTask = TaskWork.Search(e => e.RefID == strProjectId
                                                                    &&
                                                                    e.TypeCode == WorkflowCode
                                                                    && e.Status == TaskWorkStatus.UnFinish
                                                                    && e.ReceiverAccount == ClientCookie.UserCode
                                                                    &&
                                                                    e.ActivityName ==
                                                                    WFReimageConsInvtChecking.Act_Originator).Any();

                        entity.IsShowEdit = projectInfo.Status == ProjectStatus.Finished && !isExistTask;
                        entity.IsShowRecall = projectInfo.Status != ProjectStatus.Finished && !isExistTask;
                    }
                }
            }
            else
            {
                entity = new ReimageConsInvtChecking();
                entity.IsProjectFreezed = CheckIfFreezeProject(strProjectId);
            }
            entity.IsShowSave = ProjectInfo.IsFlowSavable(strProjectId, FlowCode.Reimage_ConsInvtChecking);
            PopulateAppUsers(entity);
            return entity;
        }

        private void PopulateAppUsers(ReimageConsInvtChecking entity)
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

                entity.AppUsers = new ApproveUsers();
                simp = new SimpleEmployee
                {
                    Code = approvedUsers.FMCode
                };
                entity.AppUsers.FM = simp;

                entity.AppUsers = new ApproveUsers();
                simp = new SimpleEmployee
                {
                    Code = approvedUsers.FinanceControllerCode
                };
                entity.AppUsers.FinanceController = simp;

                entity.AppUsers = new ApproveUsers();
                simp = new SimpleEmployee
                {
                    Code = approvedUsers.VPGMCode
                };
                entity.AppUsers.VPGM = simp;
            }
            else
            {
                var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId == ProjectId);
                if (reimageInfo != null)
                {
                    entity.AppUsers.ConstructionManager = new SimpleEmployee
                    {
                        Code = reimageInfo.CMAccount
                    };
                }
            }
        }
        public class WFReimageConsInvtChecking
        {
            #region ---- [ Constant Strings ] ----

            public const string ProcessName = @"MCDAMK2Project.Reimage\ConsInvtChecking";
            public const string ProcessCode = @"MCD_AM_Reimage_CIC";
            public const string TableName = "ReimageConsInvtChecking";


            public const string Act_Originator = "Originator"; // 退回至发起人节点名

            #endregion
        }

        private string GetApprovalType()
        {
            var approvalType = string.Empty;

            var reinvestment = ReinvestmentCost.FirstOrDefault(e => e.ConsInfoID == Id);
            var writeOffAmount = WriteOffAmount.FirstOrDefault(e => e.ConsInfoID == Id);
            if (reinvestment != null
                && writeOffAmount != null)
            {

                var totalVariance = GetTotalVariance(reinvestment, writeOffAmount);

                if (totalVariance <= (decimal)0.05)
                {
                    approvalType = "LeqFivePercent";
                }
                else if (totalVariance > (decimal)0.05
                         && totalVariance <= (decimal)0.1)
                {
                    approvalType = "BetweenFiveAndTenPercent";
                }
                else if (totalVariance > (decimal)0.1)
                {
                    approvalType = "MoreThanPercent";
                }
            }


            return approvalType;
        }
        private decimal GetTotalVariance(ReinvestmentCost reinvestment, WriteOffAmount writeOffAmount)
        {
            var totalReinvestmentVariance = Math.Abs(reinvestment.TotalReinvestmentVariance.As<decimal>());
            decimal totalWriteOffVariance = 0;
            if (writeOffAmount.TotalII.As<decimal>() != 0)
                totalWriteOffVariance = Math.Abs((writeOffAmount.TotalActual.As<decimal>() - writeOffAmount.TotalII.As<decimal>()) /
                         writeOffAmount.TotalII.As<decimal>());
            return totalWriteOffVariance;
        }


        public void Save(string action = "")
        {
            using (var scope = new TransactionScope())
            {
                if (Id == Guid.Empty)
                {
                    Id = Guid.NewGuid();
                    CreateTime = DateTime.Now;

                    LastUpdateTime = DateTime.Now;
                    CreateUserAccount = ClientCookie.UserCode;
                    CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                    CreateUserNameENUS = ClientCookie.UserNameENUS;
                    IsHistory = false;
                    Add(this);
                }
                else
                {
                    LastUpdateTime = DateTime.Now;
                    Update(this);
                }
                SaveApproveUsers(action);
                if (string.Compare(action, "edit", true) != 0)
                {
                    SaveComments(action);
                }
                scope.Complete();
            }

        }

        public void Submit()
        {
            var task = TaskWork.GetTaskWork(ProjectId, ClientCookie.UserCode, TaskWorkStatus.UnFinish,
                FlowCode.Reimage, FlowCode.Reimage_ConsInvtChecking);
            task.Status = TaskWorkStatus.Finished;
            task.FinishTime = DateTime.Now;
            string taskUrl = "/Reimage/Main#/ConsInvtChecking/Process/View?projectId=" + ProjectId;
            task.Url = taskUrl;
            ProcInstID = StartProcess(task);
            using (TransactionScope scope = new TransactionScope())
            {

                TaskWork.Update(task);

                Save("Submit");
                ProjectInfo.FinishNode(ProjectId, FlowCode.Reimage_ConsInvtChecking, NodeCode.Reimage_ConsInvtChecking_Downlod);
                ProjectInfo.FinishNode(ProjectId, FlowCode.Reimage_ConsInvtChecking, NodeCode.Reimage_ConsInvtChecking_Input);
                ProjectInfo.FinishNode(ProjectId, FlowCode.Reimage_ConsInvtChecking, NodeCode.Reimage_ConsInvtChecking_Upload);
                scope.Complete();
            }
        }
        private int? StartProcess(TaskWork task)
        {
            //var processDataFields = new List<ProcessDataField>()
            //{
            //    new ProcessDataField("dest_Creator", CreateUserAccount),
            //    new ProcessDataField("dest_SupervisorList",string.Join(";",AppUsers.ConstructionManager.Code, AppUsers.FM.Code)),
            //    new ProcessDataField("dest_LevelTwoApproverList",GetLeveTwoApproverList()),
            //    new ProcessDataField("ProcessCode", WFMajorLeaseConsInvtChecking.ProcessCode),
            //    new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task)),
            //    new ProcessDataField("IsNeedLevelTwoApproval",CheckIfNeedLevelTwoApproval().ToString(),"BOOLEAN")
            //};
            CreateUserAccount = ClientCookie.UserCode;
            var processDataFields = SetWorkflowDataFields(task);
            return K2FxContext.Current.StartProcess(WFReimageConsInvtChecking.ProcessCode, CreateUserAccount,
                processDataFields);
        }
        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_SupervisorList",string.Join(";",AppUsers.ConstructionManager.Code, AppUsers.FM.Code)),
                new ProcessDataField("dest_LevelTwoApproverList",string.Join(";", AppUsers.FinanceController.Code)),
                new ProcessDataField("dest_LevelThreeApproverList",string.Join(";", AppUsers.VPGM.Code)),
                new ProcessDataField("ProcessCode", WFReimageConsInvtChecking.ProcessCode),
                new ProcessDataField("IsNeedLevelTwoApproval",CheckIfNeedLevelTwoApproval().ToString(),"BOOLEAN"),
                new ProcessDataField("IsNeedLevelThreeApproval",CheckIfNeedLevelThreeApproval().ToString(),"BOOLEAN")
            };

            if (task != null)
            {
                processDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                processDataFields.Add(new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task)));
            }
            return processDataFields;
        }

        private bool CheckIfNeedLevelThreeApproval()
        {
            var isNeedLevelThreeApproval = ApprovalType.Equals("MoreThanPercent");
            return isNeedLevelThreeApproval;
        }
        private bool CheckIfNeedLevelTwoApproval()
        {
            var isNeedLevelTwoApproval = ApprovalType.Equals("BetweenFiveAndTenPercent")
                                          || ApprovalType.Equals("MoreThanPercent");
            return isNeedLevelTwoApproval;
        }

        private string GetLeveTwoApproverList()
        {
            string approvers = "";
            switch (ApprovalType)
            {
                case "LeqFivePercent":
                    //approvers = string.Join(";", AppUsers.ConstructionManager.Code, AppUsers.FM.Code);
                    break;
                case "BetweenFiveAndTenPercent":
                    approvers = string.Join(";", AppUsers.FinanceController.Code);
                    break;
                case "MoreThanPercent":
                    approvers = string.Join(";", AppUsers.FinanceController.Code, AppUsers.VPGM.Code);
                    break;
                default:
                    throw new Exception("Approval type is null, please check it!");
                    break;
            }
            return approvers;
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
                        approveUser.FlowCode = FlowCode.Reimage_ConsInvtChecking;
                    }
                    approveUser.ConstructionManagerCode = AppUsers.ConstructionManager.Code;
                    approveUser.FMCode = AppUsers.FM.Code;
                    approveUser.FinanceControllerCode = AppUsers.FinanceController.Code;
                    approveUser.VPGMCode = AppUsers.VPGM.Code;
                    approveUser.LastUpdateDate = DateTime.Now;
                    approveUser.LastUpdateUserAccount = ClientCookie.UserCode;
                    approveUser.Save();
                    break;
            }
        }
        private void SaveComments(string action)
        {
            var comment = ProjectComment.GetSavedComment(Id, "ReimageConsInvtChecking", ClientCookie.UserCode);
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
                comment.CreateUserAccount = ClientCookie.UserCode;
                comment.CreateUserNameENUS = ClientCookie.UserNameENUS;
                comment.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
                comment.UserAccount = ClientCookie.UserCode; ;
                comment.UserNameENUS = ClientCookie.UserNameENUS;
                comment.UserNameZHCN = ClientCookie.UserNameZHCN; ;
                comment.RefTableId = Id;
                comment.Id = Guid.NewGuid();
                comment.RefTableName = "ReimageConsInvtChecking";
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

        public void ResubmitConsInvtChecking(string userCode)
        {
            List<ProcessDataField> dataField = SetWorkflowDataFields(null);
            ExecuteProcess(userCode, SerialNumber, "ReSubmit", Comments, dataField);
        }

        public void ExecuteProcess(string employeeCode, string sn, string actionName, string comment, List<ProcessDataField> dataField = null)
        {
            if (actionName == ProjectAction.Return)
            {
                TaskWork.Finish(e => e.RefID == ProjectId
                    && e.TypeCode == WorkflowCode
                    && e.Status == TaskWorkStatus.UnFinish);
            }

            K2FxContext.Current.ApprovalProcess(sn, employeeCode, actionName, comment);

            using (var scope = new TransactionScope())
            {

                Save(actionName);
                switch (actionName)
                {
                    case ProjectAction.ReSubmit:
                        ProjectInfo.FinishNode(ProjectId, FlowCode.Reimage_ConsInvtChecking, NodeCode.Reimage_ConsInvtChecking_Downlod);
                        ProjectInfo.FinishNode(ProjectId, FlowCode.Reimage_ConsInvtChecking, NodeCode.Reimage_ConsInvtChecking_Input);
                        ProjectInfo.FinishNode(ProjectId, FlowCode.Reimage_ConsInvtChecking, NodeCode.Reimage_ConsInvtChecking_Upload);
                        break;
                    case ProjectAction.Return:
                    case ProjectAction.Recall:
                        ProjectInfo.Reset(ProjectId, WorkflowCode, GetProjectStatus(actionName));
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Reimage_ConsInvtChecking_Input);
                        break;
                    case ProjectAction.Decline:
                        ProjectInfo.Reject(ProjectId, WorkflowCode);
                        break;
                    default:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Reimage_ConsInvtChecking_Upload, GetProjectStatus(actionName));
                        break;
                }
                scope.Complete();
            }

        }

        private string GetNodeCode(string actionName)
        {
            string nodeCode;
            switch (actionName)
            {
                case ProjectAction.Return:
                    nodeCode = NodeCode.Start;
                    break;
                default:
                    nodeCode = NodeCode.Reimage_ConsInvtChecking_Confirm;
                    break;
            }

            return nodeCode;
        }



        public void ApproveConsInvtChecking(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Approve", Comments);
        }
        public void ReturnConsInvtChecking(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Return", Comments);
        }
        public override string Edit()
        {
            var taskUrl = string.Format("/Reimage/Main#/ConsInvtChecking?projectId={0}", ProjectId);
            using (var scope = new TransactionScope())
            {
                GenerateConsInvtCheckingTask();



                ProjectInfo.Reset(ProjectId, WorkflowCode);
                ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Reimage_ConsInvtChecking_Input);

                var form = Duplicator.AutoCopy(this);
                form.Id = Guid.Empty;
                form.ProcInstID = null;
                form.IsHistory = false;
                form.Comments = null;
                form.CreateTime = DateTime.Now;
                form.Save("edit");
                CopyReinvestmentCost(Id, form.Id);
                CopyWriteOffAmount(Id, form.Id);
                CopyAttachment(Id.ToString(), form.Id.ToString());
                CopyAppUsers(Id.ToString(), form.Id.ToString());
                IsHistory = true;
                Update(this);
                scope.Complete();
            }

            return taskUrl;
        }
        public override void Recall(string comment)
        {
            if (!ProcInstID.HasValue)
            {
                throw new Exception("The operation needs the process instance id!");
            }

            K2FxContext.Current.GoToActivityAndRecord(ProcInstID.Value,
              WFReimageConsInvtChecking.Act_Originator, ClientCookie.UserCode, ProjectAction.Recall, comment);
            using (var scope = new TransactionScope())
            {
                Save(ProjectAction.Recall);


                ProjectInfo.Reset(ProjectId, WorkflowCode, ProjectStatus.Recalled);
                ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Reimage_ConsInvtChecking_Input);
                scope.Complete();
            }
        }

        public bool CheckIfUnfreezePackageWorkflow(string projectId)
        {
            var isUnfreeze = false;
            var flowCodeList = new List<string>()
            {
                FlowCode.Reimage_ConsInfo,
                FlowCode.Reimage_Summary,
                FlowCode.Reimage_Package
            };
            var projectInfos = ProjectInfo.Search(e => e.ProjectId == projectId
                                                        && flowCodeList.Contains(e.FlowCode)).AsNoTracking().ToList();
            Log4netHelper.WriteInfo(JsonConvert.SerializeObject(projectInfos));

            if (projectInfos.Count == flowCodeList.Count
                && projectInfos.All(e => e.Status == ProjectStatus.Finished))
            {
                isUnfreeze = true;
            }

            return isUnfreeze;
        }

        public void GenerateConsInvtCheckingTask(bool isScheduled = false)
        {
            var rmgInfo = ReimageInfo.Search(e => e.ProjectId.Equals(ProjectId)).AsNoTracking().FirstOrDefault();
            if (rmgInfo == null)
            {
                throw new Exception("Could not find the Reimage Info, please check it!");
            }
            var taskUrl = string.Format(@"/Reimage/Main#/ConsInvtChecking?projectId={0}", ProjectId);
            var task = rmgInfo.GenerateTaskWork(WorkflowCode, WorkflowCode, WorkflowCode, taskUrl);
            task.ActivityName = NodeCode.Start;
            task.ActionName = SetTaskActionName(ProjectId);

            if (!isScheduled)
                TaskWork.Add(task);
            else
            {
                var gbMemo = ReimageGBMemo.GetGBMemo(ProjectId);
                if (gbMemo != null && gbMemo.ConstCompletionDate.HasValue)
                    ScheduleLog.GenerateTaskSchedule(gbMemo.ConstCompletionDate.Value.AddDays(60), task, ClientCookie.UserCode, ProjectId, FlowCode.Reimage_ConsInvtChecking, rmgInfo.USCode);
            }
        }

        public void GenerateConsInvertTask(string projectId)
        {
            if (CheckIfUnfreezePackageWorkflow(projectId))
            {
                if (!TaskWork.Any(e => e.RefID == projectId
                        && e.TypeCode == WorkflowCode
                        && e.Status == TaskWorkStatus.UnFinish))
                {
                    var taskWork = new TaskWork();
                    taskWork.SourceCode = FlowCode.Reimage;
                    taskWork.SourceNameENUS = FlowCode.Reimage;
                    taskWork.SourceNameZHCN = FlowCode.Reimage;
                    taskWork.Status = TaskWorkStatus.UnFinish;
                    taskWork.StatusNameZHCN = "任务";
                    taskWork.StatusNameENUS = "任务";
                    taskWork.RefID = projectId;
                    taskWork.Id = Guid.NewGuid();
                    taskWork.CreateTime = DateTime.Now;
                    var reimageInfo = ReimageInfo.FirstOrDefault(e => e.ProjectId == projectId);

                    if (reimageInfo == null)
                    {
                        throw new Exception("Cannot find the relative reimage info!");
                    }

                    taskWork.Title = TaskWork.BuildTitle(projectId, reimageInfo.StoreNameZHCN, reimageInfo.StoreNameENUS);
                    taskWork.TypeCode = WorkflowCode;
                    taskWork.TypeNameENUS = WorkflowCode;
                    taskWork.TypeNameZHCN = WorkflowCode;
                    taskWork.ReceiverAccount = reimageInfo.PMAccount;
                    taskWork.ReceiverNameENUS = reimageInfo.PMNameENUS;
                    taskWork.ReceiverNameZHCN = reimageInfo.PMNameZHCN;
                    taskWork.Url = string.Format(@"/Reimage/Main#/ConsInvtChecking?projectId={0}", projectId);
                    taskWork.StoreCode = reimageInfo.USCode;
                    taskWork.ActivityName = NodeCode.Start;
                    taskWork.ActionName = SetTaskActionName(projectId);
                    TaskWork.Add(taskWork);
                }
            }

        }
    }
}
