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

namespace Mcdonalds.AM.DataAccess
{
    public partial class MajorLeaseFinancAnalysis : BaseWFEntity<MajorLeaseFinancAnalysis>
    {
        public override string WorkflowCode
        {
            get { return FlowCode.MajorLease_FinanceAnalysis; }
        }

        public List<ProjectComment> ProjectComments { get; set; }
        public string Comments { get; set; }
        public override string TableName
        {
            get
            {
                return "MajorLeaseFinancAnalysis";
            }
        }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public string SerialNumber { get; set; }
        public ApproveUsers AppUsers { get; set; }
        public bool IsShowRecall { get; set; }

        public bool IsShowEdit { get; set; }

        public bool IsShowSave { get; set; }

        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            var entity = GetFinanceInfo(projectId, id);
            if (entity != null)
            {
                entity.EntityId = entity.Id;
            }
            return entity;
        }
        public MajorLeaseFinancAnalysis GetFinanceInfo(string strProjectId, string entityId = "")
        {
            MajorLeaseFinancAnalysis entity = null;
            if (string.IsNullOrEmpty(entityId))
                entity = Search(e => e.ProjectId.Equals(strProjectId) && !e.IsHistory).FirstOrDefault();
            else
                entity = Search(e => e.Id.ToString().Equals(entityId)).FirstOrDefault();

            if (entity != null)
            {
                entity.IsProjectFreezed = CheckIfFreezeProject(strProjectId);

                entity.IsShowEdit = ProjectInfo.IsFlowEditable(strProjectId, FlowCode.MajorLease_FinanceAnalysis);
                entity.IsShowRecall = ProjectInfo.IsFlowRecallable(strProjectId, FlowCode.MajorLease_FinanceAnalysis);
            }
            else
            {
                entity = new MajorLeaseFinancAnalysis();
                entity.IsProjectFreezed = CheckIfFreezeProject(strProjectId);
            }
            entity.IsShowSave = ProjectInfo.IsFlowSavable(strProjectId, FlowCode.MajorLease_FinanceAnalysis);

            PopulateAppUsers(entity);
            return entity;
        }

        private void PopulateAppUsers(MajorLeaseFinancAnalysis entity)
        {
            var approvedUsers = ApproveDialogUser.GetApproveDialogUser(entity.Id.ToString());
            entity.AppUsers = new ApproveUsers();
            if (approvedUsers != null)
            {
                var simp = new SimpleEmployee
                {
                    Code = approvedUsers.FMCode
                };
                entity.AppUsers.FM = simp;
            }
        }

        public void Submit()
        {
            var task = TaskWork.GetTaskWork(ProjectId, LastUpdateUserAccount, TaskWorkStatus.UnFinish,
                FlowCode.MajorLease, WorkflowCode);
            task.Status = TaskWorkStatus.Finished;
            task.FinishTime = DateTime.Now;
            string taskUrl = "/MajorLease/Main#/FinanceAnalysis/Process/View?projectId=" + ProjectId;
            task.Url = taskUrl;
            ProcInstID = StartProcess(task);

            using (var scope = new TransactionScope())
            {
                TaskWork.Update(task);

                Save("Submit");
                ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.MajorLease_FinanceAnalysis_Input);
                scope.Complete();
            }
        }

        private int StartProcess(TaskWork task)
        {
            CreateUserAccount = LastUpdateUserAccount;
            var majorLeaseInfo = MajorLeaseInfo.FirstOrDefault(e => e.ProjectId.Equals(ProjectId));
            if (majorLeaseInfo == null)
            {
                throw new Exception("Could not find the Major Lease Info, please check it!");
            }
            //var processDataFields = new List<ProcessDataField>()
            //{
            //    new ProcessDataField("dest_Creator", CreateUserAccount),
            //    new ProcessDataField("dest_FinancialManager",AppUsers.FM.Code ),
            //    new ProcessDataField("ProcessCode", WFMajorLeaseFinancAnalysis.ProcessCode),
            //    new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task))
            //};
            var processDataFields = SetWorkflowDataFields(task);
            return K2FxContext.Current.StartProcess(WFMajorLeaseFinancAnalysis.ProcessCode, CreateUserAccount,
                processDataFields);
        }

        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_FinancialManager",AppUsers.FM.Code ),
                new ProcessDataField("ProcessCode", WFMajorLeaseFinancAnalysis.ProcessCode)
            };

            if (task != null)
            {
                processDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                processDataFields.Add(new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task)));
            }
            return processDataFields;
        }
        public void ExecuteProcess(string employeeCode, string sn, string actionName, string comment, List<ProcessDataField> dataField = null)
        {

            K2FxContext.Current.ApprovalProcess(sn, employeeCode, actionName, comment, dataField);

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
                    case ProjectAction.ReSubmit:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.MajorLease_FinanceAnalysis_Input, GetProjectStatus(actionName));
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.MajorLease_FinanceAnalysis_Upload, GetProjectStatus(actionName));
                        break;
                    default:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.MajorLease_FinanceAnalysis_Upload, GetProjectStatus(actionName));
                        break;
                }
                scope.Complete();

            }

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
                        approveUser.FlowCode = WorkflowCode;
                    }
                    approveUser.FMCode = AppUsers.FM.Code;
                    approveUser.LastUpdateDate = DateTime.Now;
                    approveUser.LastUpdateUserAccount = ClientCookie.UserCode;
                    approveUser.Save();
                    break;
            }
        }
        private void SaveComments(string action)
        {
            var comment = ProjectComment.GetSavedComment(Id, "MajorLeaseFinancAnalysis", ClientCookie.UserCode);
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
                comment.UserAccount = ClientCookie.UserCode;
                comment.UserNameENUS = ClientCookie.UserNameENUS;
                comment.UserNameZHCN = ClientCookie.UserNameZHCN;
                comment.RefTableId = Id;
                comment.Id = Guid.NewGuid();
                comment.RefTableName = "MajorLeaseFinancAnalysis";
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

        public void ApproveFinanceAnalysis(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Approve", Comments);
        }

        public void RejectFinanceAnalysis(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Decline", Comments);
        }

        public void ReturnFinanceAnalysis(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Return", Comments);
        }

        public void ResubmitFinanceAnalysis(string userCode)
        {
            var dataField = SetWorkflowDataFields(null);
            ExecuteProcess(userCode, SerialNumber, "ReSubmit", Comments, dataField);
        }

        public override void Recall(string comment)
        {
            if (!ProcInstID.HasValue)
            {
                throw new Exception("The operation needs the process instance id!");
            }
            K2FxContext.Current.GoToActivityAndRecord(ProcInstID.Value,
               WFMajorLeaseFinancAnalysis.Act_Originator, ClientCookie.UserCode, ProjectAction.Recall, comment);

            using (var scope = new TransactionScope())
            {
                Save(ProjectAction.Recall);

                ProjectInfo.Reset(ProjectId, WorkflowCode, ProjectStatus.Recalled);
                scope.Complete();
            }
        }

        public override string Edit()
        {

            var taskUrl = string.Format("/MajorLease/Main#/FinanceAnalysis?projectId={0}", ProjectId);
            using (var scope = new TransactionScope())
            {
                var majorLeaseInfo = MajorLeaseInfo.FirstOrDefault(e => e.ProjectId.Equals(ProjectId));
                if (majorLeaseInfo == null)
                {
                    throw new Exception("Could not find the Major Lease Info, please check it!");
                }
                var task = majorLeaseInfo.GenerateTaskWork(WorkflowCode,
                     "FinanceAnalysis",
                     "FinanceAnalysis",
                     taskUrl);
                task.ActivityName = NodeCode.Start;
                task.ActionName = SetTaskActionName(ProjectId);
                TaskWork.Add(task);

                var package = MajorLeaseChangePackage.GetMajorPackageInfo(ProjectId);
                if (package != null)
                {
                    package.ProjectId = ProjectId;
                    package.CompleteActorPackageTask(majorLeaseInfo.AssetActorAccount);
                }

                IsHistory = true;
                Update(this);
                var attachments = Attachment.Search(e => e.RefTableID == Id.ToString()
                                                              && e.RefTableName == WFMajorLeaseFinancAnalysis.TableName).AsNoTracking().ToList();
                ProjectInfo.Reset(ProjectId, WorkflowCode);

                Mapper.CreateMap<MajorLeaseFinancAnalysis, MajorLeaseFinancAnalysis>();
                var form = Mapper.Map<MajorLeaseFinancAnalysis>(this);
                form.Id = Guid.Empty;
                form.ProcInstID = null;
                form.IsHistory = false;
                form.Comments = null;
                form.Save();

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
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.MajorLease_FinanceAnalysis_Confirm, ProjectStatus.Finished);
                        var majorLeasePackage = new MajorLeaseChangePackage();
                        majorLeasePackage.GeneratePackageTask(ProjectId);
                        break;
                }
                scope.Complete();
            }
        }

        public override void GenerateDefaultWorkflowInfo(string projectId)
        {
            var entity = new MajorLeaseFinancAnalysis();
            entity.ProjectId = projectId;
            entity.Id = Guid.NewGuid();
            entity.CreateTime = DateTime.Now;
            entity.CreateUserAccount = ClientCookie.UserCode;
            entity.CreateUserNameENUS = ClientCookie.UserNameENUS;
            entity.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
            entity.IsHistory = false;
            Add(entity);
        }
    }

    public class WFMajorLeaseFinancAnalysis
    {
        #region ---- [ Constant Strings ] ----

        public const string ProcessName = @"MCDAMK2Project.MajorLeaseChange\FinanceAnalysis";
        public const string ProcessCode = @"MCD_AM_MLC_FA";
        public const string TableName = "MajorLeaseFinancAnalysis";


        public const string Act_Originator = "Originator"; // 退回至发起人节点名

        #endregion
    }
}
