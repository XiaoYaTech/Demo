using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
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
    public partial class MajorLeaseLegalReview : BaseWFEntity<MajorLeaseLegalReview>
    {
        public override string WorkflowCode
        {
            get { return FlowCode.MajorLease_LegalReview; }
        }

        public List<ProjectComment> ProjectComments { get; set; }
        public string Comments { get; set; }
        public string Action { get; set; }
        public string SerialNumber { get; set; }

        public bool IsShowRecall { get; set; }
        public bool IsShowEdit { get; set; }
        public bool IsShowSave { get; set; }


        public ApproveUsers AppUsers { get; set; }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
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
        public override string TableName
        {
            get
            {
                return "MajorLeaseLegalReview";
            }
        }
        public static MajorLeaseLegalReview GetLegalReviewInfo(string strProjectId, string entityId = "")
        {
            var entity = string.IsNullOrEmpty(entityId) ?
                FirstOrDefault(e => e.ProjectId.Equals(strProjectId) && !e.IsHistory)
                : FirstOrDefault(e => e.Id.ToString().Equals(entityId));

            if (entity != null)
            {
                entity.IsShowEdit = ProjectInfo.IsFlowEditable(strProjectId, FlowCode.MajorLease_LegalReview);
                entity.IsShowRecall = ProjectInfo.IsFlowRecallable(strProjectId, FlowCode.MajorLease_LegalReview);

            }
            else
            {
                entity = new MajorLeaseLegalReview();
            }
            entity.IsProjectFreezed = entity.CheckIfFreezeProject(strProjectId);
            entity.IsShowSave = ProjectInfo.IsFlowSavable(strProjectId, FlowCode.MajorLease_LegalReview);
            PopulateAppUsers(entity);
            return entity;
        }

        private static void PopulateAppUsers(MajorLeaseLegalReview entity)
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
            var task = TaskWork.GetTaskWork(ProjectId, ClientCookie.UserCode, TaskWorkStatus.UnFinish,
                FlowCode.MajorLease, FlowCode.MajorLease_LegalReview);
            string taskUrl = "/MajorLease/Main#/LegalReview/Process/View?projectId=" + ProjectId;
            task.Url = taskUrl;
            ProcInstID = StartProcess(task);
            using (var scope = new TransactionScope())
            {
                task.Status = TaskWorkStatus.Finished;
                task.FinishTime = DateTime.Now;

                TaskWork.Update(task);

                Save("Submit");
                ProjectInfo.FinishNode(ProjectId, FlowCode.MajorLease_LegalReview, NodeCode.MajorLease_LegalReview_Input);
                scope.Complete();
            }

        }


        private int StartProcess(TaskWork task)
        {
            //var repository = new MajorLeaseInfo();
            //var majorLeaseInfo = repository.Search(e => e.ProjectId.Equals(ProjectId)).FirstOrDefault();
            //if (majorLeaseInfo == null)
            //{
            //    throw new Exception("Could not find the Major Lease Info, please check it!");
            //}
            CreateUserAccount = ClientCookie.UserCode;
            var processDataFields = SetWorkflowDataFields(task);
            return K2FxContext.Current.StartProcess(WFMajorLeaseLegalReview.ProcessCode, CreateUserAccount,
                processDataFields);
        }

        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_Legal",AppUsers.Legal.Code ),
                new ProcessDataField("ProcessCode", WFMajorLeaseLegalReview.ProcessCode)
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
                    default:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.MajorLease_LegalReview_Input, GetProjectStatus(actionName));
                        break;
                }

                scope.Complete();
            }

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
                    nodeCode = NodeCode.MajorLease_LegalReview_Upload;
                    break;
            }

            return nodeCode;
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
                        approveUser.FlowCode = FlowCode.MajorLease_LegalReview;
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
            var comment = ProjectComment.GetSavedComment(Id, "MajorLeaseLegalReview", ClientCookie.UserCode);
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
                comment.ActionDesc =
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
                comment.RefTableName = "MajorLeaseLegalReview";
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

        public void ApproveLegalReview(string currUserAccount)
        {
            ExecuteProcess(currUserAccount, SerialNumber, "Approve", Comments);
        }

        public void RejectLegalReview(string currUserAccount)
        {
            ExecuteProcess(currUserAccount, SerialNumber, "Reject", Comments);
        }

        public void ReturnLegalReview(string currUserAccount)
        {
            ExecuteProcess(currUserAccount, SerialNumber, "Return", Comments);
        }

        public void ResubmitLegalReview(string userCode)
        {
            List<ProcessDataField> dataField = SetWorkflowDataFields(null);
            ExecuteProcess(userCode, SerialNumber, "ReSubmit", Comments, dataField);
        }

        public override void Recall(string comment)
        {
            if (!ProcInstID.HasValue)
            {
                throw new Exception("The operation needs the process instance id!");
            }
            K2FxContext.Current.GoToActivityAndRecord(ProcInstID.Value,
              WFMajorLeaseLegalReview.Act_Originator, ClientCookie.UserCode, ProjectAction.Recall, comment);
            using (var scope = new TransactionScope())
            {
                Save(ProjectAction.Recall);
                ProjectInfo.Reset(ProjectId, WorkflowCode, ProjectStatus.Recalled);
                scope.Complete();
            }


        }

        public override void GenerateDefaultWorkflowInfo(string projectId)
        {
            var entity = new MajorLeaseLegalReview
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
            var taskUrl = string.Format("/MajorLease/Main#/LegalReview?projectId={0}", ProjectId);
            using (var scope = new TransactionScope())
            {
                var majorLeaseInfo = MajorLeaseInfo.Search(e => e.ProjectId.Equals(ProjectId)).FirstOrDefault();
                if (majorLeaseInfo == null)
                {
                    throw new Exception("Could not find the Major Lease Info, please check it!");
                }
                TaskWork.Cancel(t => t.RefID == ProjectId && t.Status == TaskWorkStatus.UnFinish && t.TypeCode == this.WorkflowCode);//取消老的流程实例的所有未完成任务
                var task = majorLeaseInfo.GenerateTaskWork(WorkflowCode,
                     "LegalReview",
                     "LegalReview",
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
                                                              && e.RefTableName == WFMajorLeaseLegalReview.TableName).ToList();

                ProjectInfo.Reset(ProjectId, WorkflowCode);


                Mapper.CreateMap<MajorLeaseLegalReview, MajorLeaseLegalReview>();
                var newMajorLeaseLR = Mapper.Map<MajorLeaseLegalReview>(this);
                newMajorLeaseLR.Id = Guid.Empty;
                newMajorLeaseLR.ProcInstID = null;
                newMajorLeaseLR.IsHistory = false;
                newMajorLeaseLR.Comments = null;
                newMajorLeaseLR.Save();

                var newAttachmentList = new List<Attachment>();
                Mapper.CreateMap<Attachment, Attachment>();
                foreach (var attachment in attachments)
                {
                    var newAttachment = Mapper.Map<Attachment>(attachment);
                    newAttachment.RefTableID = newMajorLeaseLR.Id.ToString();
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
                        ProjectInfo.FinishNode(ProjectId, FlowCode.MajorLease_LegalReview, NodeCode.MajorLease_LegalReview_Confirm, ProjectStatus.Finished);

                        var majorLeasePackage = new MajorLeaseChangePackage();
                        majorLeasePackage.GeneratePackageTask(ProjectId);
                        break;
                }

                scope.Complete();
            }

        }

    }

    public class WFMajorLeaseLegalReview
    {
        #region ---- [ Constant Strings ] ----

        public const string ProcessName = @"MCDAMK2Project.MajorLeaseChange\LegalReview";
        public const string ProcessCode = @"MCD_AM_MLC_LR";
        public const string TableName = "MajorLeaseLegalReview";


        public const string Act_Originator = "Originator"; // 退回至发起人节点名
        public const string Act_Start = "Start";

        #endregion
    }
}
