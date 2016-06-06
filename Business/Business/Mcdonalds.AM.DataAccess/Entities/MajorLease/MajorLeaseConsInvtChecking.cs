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
using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using System.Data.Entity;

namespace Mcdonalds.AM.DataAccess
{
    public partial class MajorLeaseConsInvtChecking : BaseWFEntity<MajorLeaseConsInvtChecking>
    {
        public override string WorkflowCode
        {
            get { return FlowCode.MajorLease_ConsInvtChecking; }
        }

        public List<ProjectComment> ProjectComments { get; set; }
        public string Comments { get; set; }
        public string SerialNumber { get; set; }
        public string USCode { get; set; }
        public bool IsShowRecall { get; set; }

        public bool IsShowEdit { get; set; }

        public bool IsShowSave { get; set; }
        public override string TableName
        {
            get
            {
                return "MajorLeaseConsInvtChecking";
            }
        }
        public ApproveUsers AppUsers { get; set; }

        public string ApprovalType { get; set; }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            var entity = GetConsInvtChecking(projectId, id);
            if (entity != null)
            {
                entity.EntityId = entity.Id;
            }
            return entity;
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
                if (!action.Equals("Edit"))
                {
                    SaveComments(action);
                }

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
                        approveUser.FlowCode = FlowCode.MajorLease_ConsInvtChecking;
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
            var comment = ProjectComment.GetSavedComment(Id, "MajorLeaseConsInvtChecking", ClientCookie.UserCode);
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
                comment.CreateUserNameENUS = ClientCookie.UserNameZHCN;
                comment.CreateUserNameZHCN = ClientCookie.UserNameENUS;
                comment.UserAccount = ClientCookie.UserCode; ;
                comment.UserNameENUS = ClientCookie.UserNameENUS; ;
                comment.UserNameZHCN = ClientCookie.UserNameZHCN; ;
                comment.RefTableId = Id;
                comment.Id = Guid.NewGuid();
                comment.RefTableName = "MajorLeaseConsInvtChecking";
                comment.SourceCode = FlowCode.MajorLease;
                comment.SourceNameZHCN = FlowCode.MajorLease;
                comment.SourceNameENUS = FlowCode.MajorLease;
                comment.TitleNameENUS = ClientCookie.TitleENUS;
                comment.TitleNameZHCN = ClientCookie.TitleENUS;
                comment.Status = string.IsNullOrEmpty(action) ? ProjectCommentStatus.Save : ProjectCommentStatus.Submit;
                comment.ProcInstID = ProcInstID;
                comment.Add();
            }
        }

        public void Submit()
        {
            var task = TaskWork.GetTaskWork(ProjectId, LastUpdateUserAccount, TaskWorkStatus.UnFinish,
                FlowCode.MajorLease, FlowCode.MajorLease_ConsInvtChecking);
            task.Status = TaskWorkStatus.Finished;
            task.FinishTime = DateTime.Now;
            string taskUrl = "/MajorLease/Main#/ConsInvtChecking/Process/View?projectId=" + ProjectId;
            task.Url = taskUrl;
            ProcInstID = StartProcess(task);
            using (TransactionScope scope = new TransactionScope())
            {

                TaskWork.Update(task);

                Save("Submit");
                ProjectInfo.FinishNode(ProjectId, FlowCode.MajorLease_ConsInvtChecking, NodeCode.MajorLease_ConsInvtChecking_Downlod);
                ProjectInfo.FinishNode(ProjectId, FlowCode.MajorLease_ConsInvtChecking, NodeCode.MajorLease_ConsInvtChecking_Input);
                ProjectInfo.FinishNode(ProjectId, FlowCode.MajorLease_ConsInvtChecking, NodeCode.MajorLease_ConsInvtChecking_Upload);
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
            CreateUserAccount = LastUpdateUserAccount;
            var processDataFields = SetWorkflowDataFields(task);
            return K2FxContext.Current.StartProcess(WFMajorLeaseConsInvtChecking.ProcessCode, CreateUserAccount,
                processDataFields);
        }

        private bool CheckIfNeedLevelTwoApproval()
        {
            var isNeedLevelTwoApproval = ApprovalType.Equals("BetweenFiveAndTenPercent")
                                          || ApprovalType.Equals("MoreThanPercent");
            return isNeedLevelTwoApproval;
        }


        public string GetApprovalType()
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
                totalWriteOffVariance = Math.Abs((writeOffAmount.TotalActual.As<decimal>() - writeOffAmount.TotalWriteOff.As<decimal>()) /
                            writeOffAmount.TotalWriteOff.As<decimal>());
            //var totalVariance = totalWriteOffVariance > totalReinvestmentVariance
            //    ? totalWriteOffVariance
            //    : totalReinvestmentVariance;

            return totalWriteOffVariance;
        }

        public void ExecuteProcess(string employeeCode, string sn, string actionName, string comment, List<ProcessDataField> dataField = null)
        {
            using (var scope = new TransactionScope())
            {

                Save(actionName);
                switch (actionName)
                {
                    case ProjectAction.ReSubmit:
                        ProjectInfo.FinishNode(ProjectId, FlowCode.MajorLease_ConsInvtChecking, NodeCode.MajorLease_ConsInvtChecking_Downlod);
                        ProjectInfo.FinishNode(ProjectId, FlowCode.MajorLease_ConsInvtChecking, NodeCode.MajorLease_ConsInvtChecking_Input);
                        ProjectInfo.FinishNode(ProjectId, FlowCode.MajorLease_ConsInvtChecking, NodeCode.MajorLease_ConsInvtChecking_Upload);
                        break;
                    case ProjectAction.Return:
                    case ProjectAction.Recall:
                        ProjectInfo.Reset(ProjectId, WorkflowCode, GetProjectStatus(actionName));
                        break;
                    case ProjectAction.Decline:
                        ProjectInfo.Reject(ProjectId, WorkflowCode);
                        break;
                    default:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.MajorLease_ConsInvtChecking_Upload, GetProjectStatus(actionName));
                        break;
                }

                if (actionName == ProjectAction.Return)
                {
                    TaskWork.Finish(e => e.RefID == ProjectId
                        && e.TypeCode == WorkflowCode
                        && e.Status == TaskWorkStatus.UnFinish
                        && e.K2SN != sn);
                }

                scope.Complete();
            }
            K2FxContext.Current.ApprovalProcess(sn, employeeCode, actionName, comment, dataField);

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
                    nodeCode = NodeCode.MajorLease_ConsInvtChecking_Upload;
                    break;
            }

            return nodeCode;
        }

        public void ApproveConsInvtChecking(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Approve", Comments);
        }

        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            //Log4netHelper.WriteInfo(JsonConvert.SerializeObject(new { AppUsers }));
            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_SupervisorList",string.Join(";",AppUsers.ConstructionManager.Code, AppUsers.FM.Code)),
                new ProcessDataField("dest_LevelTwoApproverList",string.Join(";", AppUsers.FinanceController.Code)),
                new ProcessDataField("dest_LevelThreeApproverList",string.Join(";", AppUsers.VPGM.Code)),
                new ProcessDataField("ProcessCode", WFMajorLeaseConsInvtChecking.ProcessCode),
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

        public void RejectConsInvtChecking(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Decline", Comments);
        }

        public void ReturnConsInvtChecking(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Return", Comments);
        }

        public void ResubmitConsInvtChecking(string userCode)
        {
            //Log4netHelper.WriteInfo("check consinvtchecking");
            List<ProcessDataField> dataField = SetWorkflowDataFields(null);
            ExecuteProcess(userCode, SerialNumber, "ReSubmit", Comments, dataField);
        }

        public MajorLeaseConsInvtChecking GetConsInvtChecking(string strProjectId, string entityId = "")
        {
            ProjectId = strProjectId;
            MajorLeaseConsInvtChecking entity = null;
            if (string.IsNullOrEmpty(entityId))
                entity = Search(e => e.ProjectId.Equals(strProjectId) && !e.IsHistory).FirstOrDefault();
            else
                entity = Search(e => e.Id.ToString().Equals(entityId)).FirstOrDefault();

            var majorLeaseInfo = MajorLeaseInfo.FirstOrDefault(e => e.ProjectId == strProjectId);
            USCode = majorLeaseInfo.USCode;
            if (entity != null)
            {
                entity.IsProjectFreezed = CheckIfFreezeProject(strProjectId);
                entity.ApprovalType = entity.GetApprovalType();

                entity.IsShowEdit = ProjectInfo.IsFlowEditable(strProjectId, FlowCode.MajorLease_ConsInvtChecking);
                entity.IsShowRecall = ProjectInfo.IsFlowRecallable(strProjectId, FlowCode.MajorLease_ConsInvtChecking);
            }
            else
            {
                entity = new MajorLeaseConsInvtChecking();
                entity.ProjectId = strProjectId;
                entity.Save();
                entity.IsProjectFreezed = CheckIfFreezeProject(strProjectId);
            }

            entity.IsShowSave = ProjectInfo.IsFlowSavable(strProjectId, FlowCode.MajorLease_ConsInvtChecking);
            PopulateAppUsers(entity);
            return entity;
        }

        public override ApproveDialogUser GetApproveDialogUser()
        {
            var approvers = ApproveDialogUser.FirstOrDefault(e => e.RefTableID == Id.ToString());
            if (approvers == null)
            {
                approvers = new ApproveDialogUser();
                var majorLeaesInfo = MajorLeaseInfo.FirstOrDefault(e => e.ProjectId == ProjectId);
                if (majorLeaesInfo != null)
                {
                    approvers.ConstructionManagerCode = majorLeaesInfo.CMAccount;
                }
            }
            return approvers;
        }

        private void PopulateAppUsers(MajorLeaseConsInvtChecking entity)
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
                var majorLeaesInfo = MajorLeaseInfo.FirstOrDefault(e => e.ProjectId == ProjectId);
                if (majorLeaesInfo != null)
                {
                    entity.AppUsers.ConstructionManager = new SimpleEmployee()
                    {
                        Code = majorLeaesInfo.CMAccount
                    };
                }
            }
        }

        public void GenerateConsInvtCheckingTask(bool isScheduled = false)
        {
            if (CheckIfNeedGenerateConsInvtCheckingTask())
            {
                var taskUrl = string.Format("/MajorLease/Main#/ConsInvtChecking?projectId={0}", ProjectId);
                var majorLeaseInfo = MajorLeaseInfo.Search(e => e.ProjectId.Equals(ProjectId)).AsNoTracking().FirstOrDefault();
                if (majorLeaseInfo == null)
                {
                    throw new Exception("Could not find the Major Lease Info, please check it!");
                }
                var task = majorLeaseInfo.GenerateTaskWork(WorkflowCode, WorkflowCode, WorkflowCode, taskUrl);
                task.ActivityName = NodeCode.Start;
                task.ActionName = SetTaskActionName(ProjectId);
                task.CreateTime = DateTime.Now;

                if (!isScheduled)
                    TaskWork.Add(task);
                else
                {
                    var consInfoRepos = new MajorLeaseConsInfo();
                    var consInfo = consInfoRepos.GetConsInfo(ProjectId);
                    //到达Cons Info中的工程完工时间后60天 发出任务
                    if (consInfo.ReinBasicInfo != null
                        && consInfo.ReinBasicInfo.ConsCompletionDate.HasValue)
                        ScheduleLog.GenerateTaskSchedule(consInfo.ReinBasicInfo.ConsCompletionDate.Value.AddDays(60), task, ClientCookie.UserCode, ProjectId, WorkflowCode, majorLeaseInfo.USCode);
                }
            }

        }

        private bool CheckIfNeedGenerateConsInvtCheckingTask()
        {
            var isNeed = false;

            var consInfoRepos = new MajorLeaseConsInfo();
            var consInfo = consInfoRepos.GetConsInfo(ProjectId);
            if (consInfo != null && consInfo.ReinvenstmentType == 3)
            {
                isNeed = true;
            }

            return isNeed;
        }

        public override void Recall(string comment)
        {
            if (!ProcInstID.HasValue)
            {
                throw new Exception("The operation needs the process instance id!");
            }

            K2FxContext.Current.GoToActivityAndRecord(ProcInstID.Value,
              WFMajorLeaseConsInvtChecking.Act_Originator, ClientCookie.UserCode, ProjectAction.Recall, comment);
            using (var scope = new TransactionScope())
            {
                Save(ProjectAction.Recall);


                ProjectInfo.Reset(ProjectId, WorkflowCode, ProjectStatus.Recalled);

                scope.Complete();
            }
        }

        public override string Edit()
        {
            var taskUrl = string.Format("/MajorLease/Main#/ConsInvtChecking?projectId={0}", ProjectId);
            using (var scope = new TransactionScope())
            {
                GenerateConsInvtCheckingTask();
                IsHistory = true;
                Update(this);

                var attachments = Attachment.Search(e => e.RefTableID == Id.ToString()
                                                              && e.RefTableName == WFMajorLeaseConsInvtChecking.TableName).AsNoTracking().ToList();

                ProjectInfo.Reset(ProjectId, WorkflowCode);

                Mapper.CreateMap<MajorLeaseConsInvtChecking, MajorLeaseConsInvtChecking>();
                var form = Mapper.Map<MajorLeaseConsInvtChecking>(this);
                form.Id = Guid.Empty;
                form.ProcInstID = null;
                form.IsHistory = false;
                form.Comments = null;
                form.Save("Edit");

                CopyReinvestmentCost(Id, form.Id);
                CopyWriteOffAmount(Id, form.Id);

                var newAttachmentList = new List<Attachment>();
                Mapper.CreateMap<Attachment, Attachment>();
                foreach (var attachment in attachments)
                {
                    var newAttachment = Mapper.Map<Attachment>(attachment);
                    newAttachment.RefTableID = form.Id.ToString();
                    newAttachment.ID = Guid.NewGuid();
                    newAttachmentList.Add(newAttachment);
                }
                Attachment.Add(newAttachmentList.ToArray());
                scope.Complete();
            }

            return taskUrl;
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
    }

    public class WFMajorLeaseConsInvtChecking
    {
        #region ---- [ Constant Strings ] ----

        public const string ProcessName = @"MCDAMK2Project.MajorLeaseChange\ConsInvtChecking";
        public const string ProcessCode = @"MCD_AM_MLC_CIC";
        public const string TableName = "MajorLeaseConsInvtChecking";


        public const string Act_Originator = "Originator"; // 退回至发起人节点名

        #endregion
    }
}
