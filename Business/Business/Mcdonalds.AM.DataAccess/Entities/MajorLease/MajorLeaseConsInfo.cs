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
    public partial class MajorLeaseConsInfo : BaseWFEntity<MajorLeaseConsInfo>
    {
        public override string WorkflowCode
        {
            get { return FlowCode.MajorLease_ConsInfo; }
        }

        public List<ProjectComment> ProjectComments { get; set; }
        public string Comments { get; set; }
        public ReinvestmentBasicInfo ReinBasicInfo { get; set; }
        public ReinvestmentCost ReinCost { get; set; }
        public WriteOffAmount WriteOff { get; set; }

        public ApproveUsers AppUsers { get; set; }
        public string SerialNumber { get; set; }

        public bool IsShowRecall { get; set; }

        public bool IsShowEdit { get; set; }

        public bool IsShowSave { get; set; }
        public override string TableName
        {
            get
            {
                return "MajorLeaseConsInfo";
            }
        }
        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            var entity = GetConsInfo(projectId, id);
            if (entity != null)
            {
                entity.EntityId = entity.Id;
            }
            return entity;
        }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public MajorLeaseConsInfo GetConsInfo(string strProjectId, string entityId = "")
        {
            MajorLeaseConsInfo entity = null;
            var majorLeaseInfo = MajorLeaseInfo.Search(e => e.ProjectId == strProjectId).FirstOrDefault();
            if (string.IsNullOrEmpty(entityId))
                entity = Search(e => e.ProjectId.Equals(strProjectId) && !e.IsHistory).FirstOrDefault();
            else
                entity = Search(e => e.Id.ToString().Equals(entityId)).FirstOrDefault();

            if (entity != null)
            {
                entity.IsProjectFreezed = CheckIfFreezeProject(strProjectId);
                entity.ReinCost = ReinvestmentCost.GetByConsInfoId(entity.Id);
                entity.ReinBasicInfo = ReinvestmentBasicInfo.GetByConsInfoId(entity.Id);
                if (entity.ReinBasicInfo == null)
                {
                    entity.ReinBasicInfo = new ReinvestmentBasicInfo
                    {
                        GBDate = majorLeaseInfo.GBDate,
                        ReopenDate = majorLeaseInfo.ReopenDate
                    };
                }
                entity.WriteOff = WriteOffAmount.GetByConsInfoId(entity.Id);

                entity.IsShowEdit = ProjectInfo.IsFlowEditable(strProjectId, WorkflowCode);
                entity.IsShowRecall = ProjectInfo.IsFlowRecallable(strProjectId, WorkflowCode);

            }
            else
            {
                if (majorLeaseInfo != null)
                {
                    entity = new MajorLeaseConsInfo
                    {
                        ReinBasicInfo =
                            new ReinvestmentBasicInfo
                            {
                                GBDate = majorLeaseInfo.GBDate,
                                ReopenDate = majorLeaseInfo.ReopenDate
                            },
                        IsProjectFreezed = CheckIfFreezeProject(strProjectId),
                        ReinvenstmentType = 1
                    };
                }
            }
            entity.IsShowSave = ProjectInfo.IsFlowSavable(strProjectId, FlowCode.MajorLease_ConsInfo);
            PopulateAppUsers(entity);
            return entity;
        }

        private void PopulateAppUsers(MajorLeaseConsInfo entity)
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

                simp = new SimpleEmployee
                {
                    Code = approvedUsers.MCCLConsManagerCode
                };
                entity.AppUsers.MCCLConsManager = simp;

                //simp = new SimpleEmployee
                //{
                //    Code = approvedUsers.MCCLAssetMgrCode
                //};
                //entity.AppUsers.MCCLAssetMgr = simp;

                //simp = new SimpleEmployee
                //{
                //    Code = approvedUsers.MCCLAssetDtrCode
                //};
                //entity.AppUsers.MCCLAssetDtr = simp;
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

        public void Submit()
        {
            var task = TaskWork.GetTaskWork(ProjectId, ClientCookie.UserCode, TaskWorkStatus.UnFinish,
                FlowCode.MajorLease, FlowCode.MajorLease_ConsInfo);
            ProcInstID = StartProcess(task);
            //task.ProcInstID = ProcInstID;
            using (var scope = new TransactionScope())
            {

                task.Finish();

                Save("Submit");
                ProjectInfo.FinishNode(ProjectId, FlowCode.MajorLease_ConsInfo, NodeCode.MajorLease_ConsInfo_Input);

                //var majorLeasePackage = new MajorLeaseChangePackage();
                //majorLeasePackage.GeneratePackageTask(ProjectId);

                //var majorLeaseConsInvtChecking = new MajorLeaseConsInvtChecking();
                //majorLeaseConsInvtChecking.GenerateConsInvtCheckingTask(ProjectId);

                scope.Complete();
            }
        }

        private int? StartProcess(TaskWork task)
        {

            var majorLeaseInfo = MajorLeaseInfo.FirstOrDefault(e => e.ProjectId.Equals(ProjectId));
            if (majorLeaseInfo == null)
            {
                throw new Exception("Could not find the Major Lease Info, please check it!");
            }
            //var processDataFields = new List<ProcessDataField>()
            //{
            //    new ProcessDataField("dest_Creator", CreateUserAccount),
            //    new ProcessDataField("dest_ConsManager",AppUsers.ConstructionManager.Code ),
            //    new ProcessDataField("dest_MCCLConsManager",AppUsers.MCCLConsManager.Code),
            //    new ProcessDataField("ProcessCode", WFMajorLeaseConsInfo.ProcessCode),
            //    new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task)),
            //    new ProcessDataField("IsNoReinvestment",CheckIfHasNoReinvestment().ToString(),"BOOLEAN")
            //};


            var processDataFields = SetWorkflowDataFields(task);
            return K2FxContext.Current.StartProcess(WFMajorLeaseConsInfo.ProcessCode, ClientCookie.UserCode,
                processDataFields);
        }

        private bool CheckIfHasNoReinvestment()
        {
            var hasNoReinvestment = false;
            if (ReinvenstmentType.HasValue)
            {
                if (ReinvenstmentType.Value.Equals(1))
                {
                    hasNoReinvestment = true;
                }
            }

            return hasNoReinvestment;
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
                if (ReinBasicInfo != null)
                {
                    var reinBasicInfo = Duplicator.AutoCopy(ReinBasicInfo);
                    var oldreinBasicInfo = ReinvestmentBasicInfo.FirstOrDefault(e => e.ConsInfoID == Id);
                    if (oldreinBasicInfo != null)
                    {
                        reinBasicInfo.Id = oldreinBasicInfo.Id;
                    }
                    else
                    {
                        reinBasicInfo.Id = 0;
                    }
                    reinBasicInfo.ConsInfoID = Id;
                    reinBasicInfo.Save();

                    UpdateMajorLeaseInfo();
                }

                if (ReinCost != null)
                {
                    var reinCost = Duplicator.AutoCopy(ReinCost);
                    reinCost.Id = new Guid();
                    reinCost.ConsInfoID = Id;

                    reinCost.Save();
                }

                if (WriteOff != null)
                {
                    var writeOff = Duplicator.AutoCopy(WriteOff);
                    writeOff.Id = new Guid();
                    writeOff.ConsInfoID = Id;
                    writeOff.Save();
                }
                SaveApproveUsers(action);
                if (!action.Equals("Edit"))
                {
                    SaveComments(action);
                }

                UpdateAttachmentStatusByReinvenstmentType();
                scope.Complete();
            }
        }

        private void UpdateAttachmentStatusByReinvenstmentType()
        {
            var isHide = false;
            var typecodeList = new List<string>() { "ReinCost", "WriteOff" };
            var attachmentRequirementIds = AttachmentRequirement.Search(e => e.RefTableName == "MajorLeaseConsInfo"
                                                                && typecodeList.Contains(e.TypeCode)).Select(e => e.Id).ToList();
            var attachments = Attachment.Search(e => e.RequirementId.HasValue
                                                     && e.RefTableID == Id.ToString()
                                                     && attachmentRequirementIds.Contains(e.RequirementId.Value)).ToList();
            if (attachments.Any())
            {
                if (ReinvenstmentType.HasValue
               && ReinvenstmentType.Value == 2)
                {
                    isHide = true;
                }

                foreach (var attachment in attachments)
                {
                    attachment.IsHide = isHide;
                }

                Attachment.Update(attachments.ToArray());
            }

        }

        private void UpdateMajorLeaseInfo()
        {
            //update GBDate and ReopenDate Info
            var majorleaseInfo = MajorLeaseInfo.FirstOrDefault(e => e.ProjectId == ProjectId);
            if (majorleaseInfo != null
                && ReinBasicInfo != null)
            {
                majorleaseInfo.GBDate = ReinBasicInfo.GBDate;
                majorleaseInfo.ReopenDate = ReinBasicInfo.ReopenDate;

                MajorLeaseInfo.Update(majorleaseInfo);
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
                        approveUser.FlowCode = FlowCode.MajorLease_ConsInfo;
                    }
                    if (!CheckIfHasNoReinvestment())
                    {
                        approveUser.ConstructionManagerCode = AppUsers.ConstructionManager.Code;
                        approveUser.MCCLConsManagerCode = AppUsers.MCCLConsManager.Code;
                    }
                    approveUser.LastUpdateDate = DateTime.Now;
                    approveUser.LastUpdateUserAccount = ClientCookie.UserCode;
                    approveUser.Save();
                    break;
            }
        }

        private void SaveComments(string action)
        {
            var comment = ProjectComment.GetSavedComment(Id, "MajorLeaseConsInfo", ClientCookie.UserCode);
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
                comment.RefTableName = "MajorLeaseConsInfo";
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

        public void ExecuteProcess(string employeeCode, string sn, string actionName, string comment, List<ProcessDataField> dataFields = null)
        {
            K2FxContext.Current.ApprovalProcess(sn, employeeCode, actionName, comment, dataFields);

            using (var scope = new TransactionScope())
            {

                Save(actionName);
                switch (actionName)
                {
                    case ProjectAction.ReSubmit:
                        ProjectInfo.FinishNode(ProjectId, FlowCode.MajorLease_ConsInfo, NodeCode.MajorLease_ConsInfo_Input, GetProjectStatus(actionName));
                        ProjectInfo.FinishNode(ProjectId, FlowCode.MajorLease_ConsInfo, NodeCode.MajorLease_ConsInfo_Upload, GetProjectStatus(actionName));
                        break;
                    case ProjectAction.Return:
                        ProjectInfo.Reset(ProjectId, FlowCode.MajorLease_ConsInfo);
                        break;
                    default:
                        //ProjectInfo.FinishNode(ProjectId, FlowCode.MajorLease_ConsInfo, NodeCode.MajorLease_ConsInfo_Upload);
                        break;
                }

                scope.Complete();
            }

        }

        public void ApproveConsInfo(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Approve", Comments);
        }

        public void RejectConsInfo(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Decline", Comments);
        }

        public void ReturnConsInfo(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, "Return", Comments);
        }

        public void ResubmitConsInfo(string userCode)
        {
            List<ProcessDataField> dataField = SetWorkflowDataFields(null);
            ExecuteProcess(userCode, SerialNumber, "ReSubmit", Comments, dataField);
        }

        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_ConsManager",CheckIfHasNoReinvestment()?"": AppUsers.ConstructionManager.Code ),
                new ProcessDataField("dest_MCCLConsManager",CheckIfHasNoReinvestment()?"":AppUsers.MCCLConsManager.Code),
                new ProcessDataField("ProcessCode", WFMajorLeaseConsInfo.ProcessCode),
                new ProcessDataField("IsNoReinvestment",CheckIfHasNoReinvestment().ToString(),"BOOLEAN")
            };

            if (task != null)
            {
                processDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                processDataFields.Add(new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task)));
            }
            return processDataFields;
        }
        public ReinvestmentBasicInfo GetReinvestmentBasicInfo(string projectId)
        {
            ReinvestmentBasicInfo reinvestmentBasicInfo;
            using (var context = new McdAMEntities())
            {
                var result = (from mlc in context.MajorLeaseConsInfo
                              join rbi in context.ReinvestmentBasicInfo on mlc.Id equals rbi.ConsInfoID
                              where mlc.ProjectId == projectId && !mlc.IsHistory
                              select rbi).Distinct().AsEnumerable();
                reinvestmentBasicInfo = result.FirstOrDefault();
            }

            return reinvestmentBasicInfo;
        }

        public override void Recall(string comment)
        {
            if (!ProcInstID.HasValue)
            {
                throw new Exception("The operation needs the process instance id!");
            }
            K2FxContext.Current.GoToActivityAndRecord(ProcInstID.Value,
                WFMajorLeaseConsInfo.Act_Originator, ClientCookie.UserCode, ProjectAction.Recall, comment);

            using (var scope = new TransactionScope())
            {
                Save(ProjectAction.Recall);

                ProjectInfo.Reset(ProjectId, WorkflowCode, ProjectStatus.Recalled);
                scope.Complete();
            }
        }

        public override string Edit()
        {
            var taskUrl = string.Format("/MajorLease/Main#/ConsInfo?projectId={0}", ProjectId);
            using (var scope = new TransactionScope())
            {
                var majorLeaseInfo = MajorLeaseInfo.FirstOrDefault(e => e.ProjectId.Equals(ProjectId));
                if (majorLeaseInfo == null)
                {
                    throw new Exception("Could not find the Major Lease Info, please check it!");
                }
                var task = majorLeaseInfo.GenerateTaskWork(FlowCode.MajorLease_ConsInfo,
                     "ConsInfo",
                     "ConsInfo",
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
                                                              && e.RefTableName == WFMajorLeaseConsInfo.TableName).AsNoTracking().ToList();

                ProjectInfo.Reset(ProjectId, FlowCode.MajorLease_ConsInfo);

                Mapper.CreateMap<MajorLeaseConsInfo, MajorLeaseConsInfo>();
                var form = Mapper.Map<MajorLeaseConsInfo>(this);
                form.Id = Guid.Empty;
                form.ProcInstID = null;
                form.IsHistory = false;
                form.Comments = null;
                form.Save("Edit");

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

            switch (status)
            {
                case TaskWorkStatus.K2ProcessApproved:
                    ProjectInfo.FinishNode(ProjectId, FlowCode.MajorLease_ConsInfo,
                        NodeCode.MajorLease_ConsInfo_Confirm, ProjectStatus.Finished);

                    var majorLeasePackage = new MajorLeaseChangePackage();
                    majorLeasePackage.GeneratePackageTask(ProjectId);

                    UpdateConsInvtCheckingStatus();

                    break;
            }

        }

        private void UpdateConsInvtCheckingStatus()
        {
            if (ReinvenstmentType == 2 ||
                ReinvenstmentType == 1)
            {
                ProjectInfo.UpdateProjectStatus(ProjectId, FlowCode.MajorLease_ConsInvtChecking, ProjectStatus.Finished);
            }
            else
            {
                ProjectInfo.UpdateProjectStatus(ProjectId, FlowCode.MajorLease_ConsInvtChecking, ProjectStatus.UnFinish);
            }
        }

        public override void GenerateDefaultWorkflowInfo(string projectId)
        {
            var entity = new MajorLeaseConsInfo();
            entity.ProjectId = projectId;
            entity.Id = Guid.NewGuid();
            entity.CreateTime = DateTime.Now;
            entity.CreateUserAccount = ClientCookie.UserCode;
            entity.CreateUserNameENUS = ClientCookie.UserNameENUS;
            entity.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
            entity.IsHistory = false;

            var majorLeaseInfo = MajorLeaseInfo.FirstOrDefault(e => e.ProjectId == projectId);
            if (majorLeaseInfo != null)
            {
                entity.ReinBasicInfo = new ReinvestmentBasicInfo
                {
                    GBDate = majorLeaseInfo.GBDate,
                    ReopenDate = majorLeaseInfo.ReopenDate
                };
                ReinvenstmentType = 1;
            }
            Add(entity);
        }
    }

    public class WFMajorLeaseConsInfo
    {
        #region ---- [ Constant Strings ] ----

        public const string ProcessName = @"MCDAMK2Project.MajorLeaseChange\ConsInfo";
        public const string ProcessCode = @"MCD_AM_MLC_CI";
        public const string TableName = "MajorLeaseConsInfo";


        public const string Act_Originator = "Originator"; // 退回至发起人节点名

        #endregion
    }
}
