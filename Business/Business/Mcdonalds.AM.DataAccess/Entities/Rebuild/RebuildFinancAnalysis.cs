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
    public partial class RebuildFinancAnalysis : BaseWFEntity<RebuildFinancAnalysis>
    {
        public List<ProjectComment> ProjectComments { get; set; }
        public string Comments { get; set; }
        public string SerialNumber { get; set; }
        public ApproveUsers AppUsers { get; set; }
        public bool IsShowRecall { get; set; }
        public bool IsShowEdit { get; set; }
        public bool IsShowSave { get; set; }
        public override string WorkflowProcessName { get { return @"MCDAMK2Project.Rebuild\FinanceAnalysis"; } }
        public override string WorkflowProcessCode { get { return @"MCD_AM_Rebuild_FA"; } }
        public override string TableName { get { return "RebuildFinancAnalysis"; } }
        public override string WorkflowActOriginator { get { return "Originator"; } } // 退回至发起人节点名
        public override string WorkflowCode
        {
            get { return FlowCode.Rebuild_FinanceAnalysis; }
        }
        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            var entity = GetFinanceInfo(projectId, id);
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
                new ProcessDataField("dest_FinancialManager",AppUsers.FM.Code ),
                new ProcessDataField("ProcessCode", WorkflowProcessCode)
            };

            if (task != null)
            {
                processDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                processDataFields.Add(new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task)));
            }
            return processDataFields;
        }
        public override string Edit()
        {
            var taskUrl = string.Format("/Rebuild/Main#/FinanceAnalysis?projectId={0}", ProjectId);
            using (var scope = new TransactionScope())
            {
                var rbdInfo = RebuildInfo.FirstOrDefault(e => e.ProjectId.Equals(ProjectId));
                if (rbdInfo == null)
                {
                    throw new Exception("Could not find the Rebuild Info, please check it!");
                }
                var task = rbdInfo.GenerateTaskWork(WorkflowCode,
                     "FinanceAnalysis",
                     "FinanceAnalysis",
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

                Mapper.CreateMap<RebuildFinancAnalysis, RebuildFinancAnalysis>();
                var form = Mapper.Map<RebuildFinancAnalysis>(this);
                form.Id = Guid.Empty;
                form.ProcInstID = null;
                form.IsHistory = false;
                form.Comments = null;
                form.Save("Edit");

                CopyAttachment(Id.ToString(), form.Id.ToString());
                CopyAppUsers(Id.ToString(), form.Id.ToString());
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
                        var rbdPackage = new RebuildPackage();
                        rbdPackage.ProjectId = ProjectId;
                        rbdPackage.GeneratePackageTask();
                        break;
                }
                scope.Complete();
            }
        }
        public override void GenerateDefaultWorkflowInfo(string projectId)
        {
            var entity = new RebuildFinancAnalysis();
            entity.ProjectId = projectId;
            entity.Id = Guid.NewGuid();
            entity.CreateTime = DateTime.Now;
            entity.CreateUserAccount = ClientCookie.UserCode;
            entity.CreateUserNameENUS = ClientCookie.UserNameENUS;
            entity.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
            entity.IsHistory = false;
            Add(entity);
        }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public RebuildFinancAnalysis GetFinanceInfo(string strProjectId, string entityId = "")
        {
            RebuildFinancAnalysis entity = null;
            if (string.IsNullOrEmpty(entityId))
                entity = Search(e => e.ProjectId.Equals(strProjectId) && !e.IsHistory).FirstOrDefault();
            else
                entity = Search(e => e.Id.ToString().Equals(entityId)).FirstOrDefault();

            if (entity != null)
            {
                entity.IsProjectFreezed = CheckIfFreezeProject(strProjectId);

                entity.IsShowEdit = ProjectInfo.IsFlowEditable(strProjectId, FlowCode.Rebuild_FinanceAnalysis);
                entity.IsShowRecall = ProjectInfo.IsFlowRecallable(strProjectId, FlowCode.Rebuild_FinanceAnalysis);
                entity.IsShowSave = ProjectInfo.IsFlowSavable(strProjectId, FlowCode.Rebuild_FinanceAnalysis);

            }
            else
            {
                entity = new RebuildFinancAnalysis();
                entity.IsProjectFreezed = CheckIfFreezeProject(strProjectId);
            }
            PopulateAppUsers(entity);
            return entity;
        }
        public static void PopulateAppUsers(RebuildFinancAnalysis entity)
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
                FlowCode.Rebuild, WorkflowCode);
            task.Status = TaskWorkStatus.Finished;
            task.FinishTime = DateTime.Now;
            string taskUrl = "/Rebuild/Main#/FinanceAnalysis/Process/View?projectId=" + ProjectId;
            task.Url = taskUrl;
            ProcInstID = StartProcess(task);

            using (var scope = new TransactionScope())
            {
                TaskWork.Update(task);

                Save("Submit");
                ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Rebuild_FinanceAnalysis_Input);
                scope.Complete();
            }
        }
        private int StartProcess(TaskWork task)
        {
            CreateUserAccount = LastUpdateUserAccount;
            var rbdInfo = RebuildInfo.FirstOrDefault(e => e.ProjectId.Equals(ProjectId));
            if (rbdInfo == null)
            {
                throw new Exception("Could not find the Rebuild Info, please check it!");
            }
            var processDataFields = SetWorkflowDataFields(task);
            return K2FxContext.Current.StartProcess(WorkflowProcessCode, CreateUserAccount,
                processDataFields);
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
            if (action == "Edit")
                return;
            var comment = ProjectComment.GetSavedComment(Id, "RebuildFinancAnalysis", LastUpdateUserAccount);
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
                comment.RefTableName = "RebuildFinancAnalysis";
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
        public void ApproveFinanceAnalysis(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, ProjectAction.Approve, Comments);
        }
        public void RejectFinanceAnalysis(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, ProjectAction.Decline, Comments);
        }
        public void ReturnFinanceAnalysis(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, ProjectAction.Return, Comments);
        }
        public void ResubmitFinanceAnalysis(string userCode)
        {
            var dataField = SetWorkflowDataFields(null);
            ExecuteProcess(userCode, SerialNumber, ProjectAction.ReSubmit, Comments, dataField);
        }
        public override void Recall(string comment)
        {
            if (!ProcInstID.HasValue)
            {
                throw new Exception("The operation needs the process instance id!");
            }
            K2FxContext.Current.GoToActivityAndRecord(ProcInstID.Value,
               WorkflowActOriginator, ClientCookie.UserCode, ProjectAction.Recall, comment);

            using (var scope = new TransactionScope())
            {
                Save(ProjectAction.Recall);
                ProjectInfo.Reset(ProjectId, WorkflowCode, ProjectStatus.Recalled);
                scope.Complete();
            }
        }
        public void ExecuteProcess(string employeeCode, string sn, string actionName, string comment, List<ProcessDataField> dataField = null)
        {
            using (var scope = new TransactionScope())
            {
                Save(actionName);
                switch (actionName)
                {
                    case ProjectAction.Recall:
                    case ProjectAction.Return:
                        ProjectInfo.Reset(ProjectId, WorkflowCode,GetProjectStatus(actionName));
                        break;
                    case ProjectAction.Decline:
                        ProjectInfo.Reject(ProjectId, WorkflowCode);
                        break;
                    case ProjectAction.Approve:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Rebuild_FinanceAnalysis_Confirm, GetProjectStatus(actionName));
                        break;
                    default:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Rebuild_FinanceAnalysis_Input,GetProjectStatus(actionName));
                        break;
                }
                scope.Complete();
            }
            K2FxContext.Current.ApprovalProcess(sn, employeeCode, actionName, comment, dataField);
        }
    }
}
