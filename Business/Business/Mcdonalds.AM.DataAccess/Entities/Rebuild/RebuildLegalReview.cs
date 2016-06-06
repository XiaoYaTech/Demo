using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Mcdonalds.AM.DataAccess.Common.Extensions;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.K2.Services;
using Mcdonalds.AM.DataAccess.Constants;
using System.Transactions;
using NTTMNC.BPM.Fx.K2.Services.Entity;

namespace Mcdonalds.AM.DataAccess
{
    public partial class RebuildLegalReview : BaseWFEntity<RebuildLegalReview>
    {
        public List<ProjectComment> ProjectComments { get; set; }
        public string Comments { get; set; }
        public string Action { get; set; }
        public string SerialNumber { get; set; }
        public bool IsShowRecall { get; set; }
        public bool IsShowEdit { get; set; }
        public bool IsShowSave { get; set; }
        public ApproveUsers AppUsers { get; set; }
        public override string WorkflowProcessName { get { return @"MCDAMK2Project.Rebuild\LegalReview"; } }
        public override string WorkflowProcessCode { get { return @"MCD_AM_Rebuild_LR"; } }
        public override string TableName { get { return "RebuildLegalReview"; } }
        public static string WorkflowActOriginator { get { return "Originator"; } } // 退回至发起人节点名
        public override string WorkflowCode
        {
            get { return FlowCode.Rebuild_LegalReview; }
        }
        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            var entity = GetLegalReviewInfo(projectId, id);
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
                new ProcessDataField("dest_Legal",AppUsers.Legal.Code ),
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
            var entity = new RebuildLegalReview
            {
                ProjectId = projectId,
                Id = Guid.NewGuid(),
                CreateTime = DateTime.Now,
                CreateUserAccount = ClientCookie.UserCode,
                CreateUserNameENUS = ClientCookie.UserNameENUS,
                CreateUserNameZHCN = ClientCookie.UserNameZHCN,
                IsHistory = false
            };
            Add(entity);
        }
        public override string Edit()
        {
            var taskUrl = string.Format("/Rebuild/Main#/LegalReview?projectId={0}", ProjectId);
            using (var scope = new TransactionScope())
            {
                var rbdInfo = RebuildInfo.Search(e => e.ProjectId.Equals(ProjectId)).FirstOrDefault();
                if (rbdInfo == null)
                {
                    throw new Exception("Could not find the Rebuild Info, please check it!");
                }
                TaskWork.Cancel(t => t.RefID == ProjectId && t.Status == TaskWorkStatus.UnFinish && t.TypeCode == this.WorkflowCode);//取消老的流程实例的所有未完成任务
                var task = rbdInfo.GenerateTaskWork(WorkflowCode,
                     "RebuildLegalReview",
                     "RebuildLegalReview",
                     taskUrl);
                task.ActivityName = NodeCode.Start;
                task.ActionName = SetTaskActionName(ProjectId);
                TaskWork.Add(task);

                var package = RebuildPackage.GetRebuildPackageInfo(ProjectId);
                if (package != null)
                {
                    package.ProjectId = ProjectId;
                    package.CompleteActorPackageTask(rbdInfo.AssetActorAccount);
                }

                IsHistory = true;
                Update(this);

                ProjectInfo.Reset(ProjectId, WorkflowCode);

                Mapper.CreateMap<RebuildLegalReview, RebuildLegalReview>();
                var newLeaseLR = Mapper.Map<RebuildLegalReview>(this);
                newLeaseLR.Id = Guid.Empty;
                newLeaseLR.ProcInstID = null;
                newLeaseLR.IsHistory = false;
                newLeaseLR.Comments = null;
                newLeaseLR.Save("Edit");

                CopyAttachment(Id.ToString(), newLeaseLR.Id.ToString());
                CopyAppUsers(Id.ToString(), newLeaseLR.Id.ToString());
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
                        ProjectInfo.FinishNode(ProjectId, FlowCode.Rebuild_LegalReview, NodeCode.Finish);
                        var rbdPackage = new RebuildPackage();
                        rbdPackage.ProjectId = ProjectId;
                        rbdPackage.GeneratePackageTask();
                        break;
                }
                scope.Complete();
            }
        }
        public static RebuildLegalReview GetLegalReviewInfo(string strProjectId, string entityId = "")
        {
            var entity = string.IsNullOrEmpty(entityId) ?
                FirstOrDefault(e => e.ProjectId.Equals(strProjectId) && !e.IsHistory)
                : FirstOrDefault(e => e.Id.ToString().Equals(entityId));

            if (entity != null)
            {
                entity.IsProjectFreezed = entity.CheckIfFreezeProject(strProjectId);

                entity.IsShowEdit = ProjectInfo.IsFlowEditable(strProjectId, FlowCode.Rebuild_LegalReview);
                entity.IsShowRecall = ProjectInfo.IsFlowRecallable(strProjectId, FlowCode.Rebuild_LegalReview);
                entity.IsShowSave = ProjectInfo.IsFlowSavable(strProjectId, FlowCode.Rebuild_LegalReview);

            }
            else
            {
                entity = new RebuildLegalReview();
                entity.IsProjectFreezed = entity.CheckIfFreezeProject(strProjectId);
            }
            PopulateAppUsers(entity);
            return entity;
        }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public static void PopulateAppUsers(RebuildLegalReview entity)
        {
            var approvedUsers = ApproveDialogUser.GetApproveDialogUser(entity.Id.ToString());
            entity.AppUsers = new ApproveUsers();
            if (approvedUsers != null)
            {
                var simp = new SimpleEmployee
                {
                    Code = approvedUsers.LegalCode
                };
                entity.AppUsers.Legal = simp;
            }
        }
        public void Submit()
        {
            var task = TaskWork.GetTaskWork(ProjectId, LastUpdateUserAccount, TaskWorkStatus.UnFinish,
                FlowCode.Rebuild, FlowCode.Rebuild_LegalReview);

            string taskUrl = "/Rebuild/Main#/LegalReview/Process/View?projectId=" + ProjectId;
            task.Url = taskUrl;
            ProcInstID = StartProcess(task);
            using (var scope = new TransactionScope())
            {
                task.Status = TaskWorkStatus.Finished;
                task.FinishTime = DateTime.Now;
                TaskWork.Update(task);
                Save("Submit");
                ProjectInfo.FinishNode(ProjectId, FlowCode.Rebuild_LegalReview, NodeCode.Rebuild_LegalReview_Input);
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
                    CreateUserNameZHCN = ClientCookie.UserNameENUS;
                    CreateUserNameENUS = ClientCookie.UserNameZHCN;
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
                        approveUser.FlowCode = FlowCode.Rebuild_LegalReview;
                    }
                    approveUser.LegalCode = AppUsers.Legal.Code;
                    approveUser.LastUpdateDate = DateTime.Now;
                    approveUser.LastUpdateUserAccount = ClientCookie.UserCode;
                    approveUser.Save();
                    break;
            }
        }
        private void SaveComments(string action)
        {
            if (action == "Edit")
                return;
            var comment = ProjectComment.GetSavedComment(Id, "RebuildLegalReview", LastUpdateUserAccount);
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
                comment.RefTableName = "RebuildLegalReview";
                comment.SourceCode = FlowCode.Rebuild;
                comment.SourceNameZHCN = FlowCode.Rebuild;
                comment.SourceNameENUS = FlowCode.Rebuild;
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
            return K2FxContext.Current.StartProcess(WorkflowProcessCode, CreateUserAccount,
                processDataFields);
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
        public void ApproveLegalReview(string currUserAccount)
        {
            ExecuteProcess(currUserAccount, SerialNumber, ProjectAction.Approve, Comments);
        }

        public void RejectLegalReview(string currUserAccount)
        {
            ExecuteProcess(currUserAccount, SerialNumber, "Reject", Comments);
        }

        public void ReturnLegalReview(string currUserAccount)
        {
            ExecuteProcess(currUserAccount, SerialNumber, ProjectAction.Return, Comments);
        }

        public void ResubmitLegalReview(string userCode)
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
                    case ProjectAction.Approve:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Rebuild_LegalReview_Confirm, GetProjectStatus(actionName));
                        break;
                    default:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Rebuild_LegalReview_Input, GetProjectStatus(actionName));
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
                case ProjectAction.Approve:
                    nodeCode = NodeCode.Finish;
                    break;
                default:
                    nodeCode = NodeCode.Rebuild_LegalReview_Upload;
                    break;
            }
            return nodeCode;
        }
    }
}
