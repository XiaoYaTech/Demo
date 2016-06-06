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
    public partial class RebuildConsInfo : BaseWFEntity<RebuildConsInfo>
    {
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
        public override string WorkflowProcessName { get { return @"MCDAMK2Project.Rebuild\ConsInfo"; } }
        public override string WorkflowProcessCode { get { return @"MCD_AM_Rebuild_CI"; } }
        public override string TableName { get { return "RebuildConsInfo"; } }
        public override string WorkflowActOriginator { get { return "Originator"; } } // 退回至发起人节点名
        public override string WorkflowCode
        {
            get { return FlowCode.Rebuild_ConsInfo; }
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
        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_ConsManager", AppUsers.ConstructionManager.Code ),
                new ProcessDataField("dest_MCCLConsManager", AppUsers.MCCLConsManager.Code),
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
            var taskUrl = string.Format("/Rebuild/Main#/ConsInfo?projectId={0}", ProjectId);
            using (var scope = new TransactionScope())
            {
                var rbdInfo = RebuildInfo.FirstOrDefault(e => e.ProjectId.Equals(ProjectId));
                if (rbdInfo == null)
                {
                    throw new Exception("Could not find the Rebuild Info, please check it!");
                }
                var task = rbdInfo.GenerateTaskWork(FlowCode.Rebuild_ConsInfo, WorkflowCode, WorkflowCode, taskUrl);
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

                ProjectInfo.Reset(ProjectId, FlowCode.Rebuild_ConsInfo);

                Mapper.CreateMap<RebuildConsInfo, RebuildConsInfo>();
                var form = Mapper.Map<RebuildConsInfo>(this);
                form.Id = Guid.Empty;
                form.ProcInstID = null;
                form.IsHistory = false;
                form.Comments = null;
                form.Save("Edit");

                CopyAttachment(Id.ToString(), form.Id.ToString());
                CopyAppUsers(Id.ToString(), form.Id.ToString());
                CopyReinvestmentBasicInfo(ReinBasicInfo, Id);
                CopyReinvestmentCost(ReinCost, Id);
                CopyWriteOffAmount(WriteOff, Id);
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
                        ProjectInfo.FinishNode(ProjectId, FlowCode.Rebuild_ConsInfo, NodeCode.Rebuild_ConsInfo_Confirm,ProjectStatus.Finished);

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
            var entity = new RebuildConsInfo();
            entity.ProjectId = projectId;
            entity.Id = Guid.NewGuid();
            entity.CreateTime = DateTime.Now;
            entity.CreateUserAccount = ClientCookie.UserCode;
            entity.CreateUserNameENUS = ClientCookie.UserNameENUS;
            entity.CreateUserNameZHCN = ClientCookie.UserNameZHCN;
            entity.IsHistory = false;

            var rbdInfo = RebuildInfo.FirstOrDefault(e => e.ProjectId == projectId);
            if (rbdInfo != null)
            {
                entity.ReinBasicInfo = new ReinvestmentBasicInfo
                {
                    GBDate = rbdInfo.GBDate,
                    ReopenDate = rbdInfo.ReopenDate,
                    ConsCompletionDate = rbdInfo.ConstCompletionDate
                };
            }
            Add(entity);
        }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public RebuildConsInfo GetConsInfo(string strProjectId, string entityId = "")
        {
            RebuildConsInfo entity = null;
            if (string.IsNullOrEmpty(entityId))
                entity = Search(e => e.ProjectId.Equals(strProjectId) && !e.IsHistory).FirstOrDefault();
            else
                entity = Search(e => e.Id.ToString().Equals(entityId)).FirstOrDefault();

            if (entity != null)
            {
                entity.IsProjectFreezed = CheckIfFreezeProject(strProjectId);
                entity.ReinCost = ReinvestmentCost.GetByConsInfoId(entity.Id);
                entity.ReinBasicInfo = ReinvestmentBasicInfo.GetByConsInfoId(entity.Id);
                var attachmentReinCost = Attachment.FirstOrDefault(e => e.RefTableID == entity.Id.ToString() && e.TypeCode == "ReinCost");
                if (entity.ReinCost!=null && attachmentReinCost != null)
                {
                    entity.ReinCost.ReinCostUser = attachmentReinCost.CreatorNameENUS;
                    entity.ReinCost.ReinCostTime = attachmentReinCost.CreateTime;
                }
                
                if (entity.ReinBasicInfo == null)
                {
                    var rbdInfo = RebuildInfo.FirstOrDefault(e => e.ProjectId == strProjectId);
                    if (rbdInfo != null)
                    {
                        entity.ReinBasicInfo = new ReinvestmentBasicInfo
                        {
                            GBDate = rbdInfo.GBDate,
                            ReopenDate = rbdInfo.ReopenDate,
                            ConsCompletionDate =rbdInfo.ConstCompletionDate
                        };
                    }
                }
                entity.WriteOff = WriteOffAmount.GetByConsInfoId(entity.Id);
                var attachmentWriteOff = Attachment.FirstOrDefault(e => e.RefTableID == entity.Id.ToString() && e.TypeCode == "WriteOff");
                if (entity.WriteOff!=null && attachmentWriteOff != null)
                {
                    entity.WriteOff.WriteOffUser = attachmentWriteOff.CreatorNameENUS;
                    entity.WriteOff.WriteOffTime = attachmentWriteOff.CreateTime;
                }

                entity.IsShowEdit = ProjectInfo.IsFlowEditable(strProjectId, FlowCode.Rebuild_ConsInfo);
                entity.IsShowRecall = ProjectInfo.IsFlowRecallable(strProjectId, FlowCode.Rebuild_ConsInfo);
                entity.IsShowSave = ProjectInfo.IsFlowSavable(strProjectId, FlowCode.Rebuild_ConsInfo);

            }
            else
            {
                var rbdInfo = RebuildInfo.FirstOrDefault(e => e.ProjectId == strProjectId);
                if (rbdInfo != null)
                {
                    entity = new RebuildConsInfo
                    {
                        ReinBasicInfo =
                            new ReinvestmentBasicInfo
                            {
                                GBDate = rbdInfo.GBDate,
                                ReopenDate = rbdInfo.ReopenDate
                            },
                        IsProjectFreezed = CheckIfFreezeProject(strProjectId),
                        ProjectId = strProjectId
                    };
                }
            }
            PopulateAppUsers(entity);
            return entity;
        }
        public static void PopulateAppUsers(RebuildConsInfo entity)
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
                var rbdInfo = RebuildInfo.FirstOrDefault(e => e.ProjectId == entity.ProjectId);
                if (rbdInfo != null)
                {
                    var simp = new SimpleEmployee
                    {
                        Code = rbdInfo.CMAccount
                    };
                    entity.AppUsers.ConstructionManager = simp;
                }
            }
        }
        public void Submit()
        {
            var task = TaskWork.GetTaskWork(ProjectId, LastUpdateUserAccount, TaskWorkStatus.UnFinish,
                FlowCode.Rebuild, FlowCode.Rebuild_ConsInfo);
            //task.Status = TaskWorkStatus.Finished;
            task.FinishTime = DateTime.Now;
            string taskUrl = "/Rebuild/Main#/ConsInfo/Process/View?projectId=" + ProjectId;
            task.Url = taskUrl;
            ProcInstID = StartProcess(task);
            using (var scope = new TransactionScope())
            {
                TaskWork.Update(task);
                Save("Submit");
                ProjectInfo.FinishNode(ProjectId, FlowCode.Rebuild_ConsInfo, NodeCode.Rebuild_ConsInfo_Input);

                //var rbdPackage = new RebuildChangePackage();
                //rbdPackage.GeneratePackageTask(ProjectId);

                //var rbdConsInvtChecking = new RebuildConsInvtChecking();
                //rbdConsInvtChecking.GenerateConsInvtCheckingTask(ProjectId);

                scope.Complete();
            }
        }
        private int? StartProcess(TaskWork task)
        {
            CreateUserAccount = LastUpdateUserAccount;
            var rbdInfo = RebuildInfo.FirstOrDefault(e => e.ProjectId.Equals(ProjectId));
            if (rbdInfo == null)
            {
                throw new Exception("Could not find the Rebuild Info, please check it!");
            }
            var processDataFields = SetWorkflowDataFields(task);
            return K2FxContext.Current.StartProcess(WorkflowProcessCode, CreateUserAccount, processDataFields);
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
                    ReinBasicInfo.ConsInfoID = Id;
                    ReinBasicInfo.Save();
                    var rbdInfo = RebuildInfo.FirstOrDefault(e => e.ProjectId == ProjectId);
                    if (rbdInfo != null)
                    {
                        rbdInfo.GBDate = ReinBasicInfo.GBDate;
                        rbdInfo.ReopenDate = ReinBasicInfo.ReopenDate;
                        rbdInfo.ConstCompletionDate = ReinBasicInfo.ConsCompletionDate;
                        rbdInfo.Update();
                    }
                }

                if (ReinCost != null)
                {
                    ReinCost.ConsInfoID = Id;
                    ReinCost.SaveByRebuild();
                }

                if (WriteOff != null)
                {
                    WriteOff.ConsInfoID = Id;
                    WriteOff.SaveByRebuild();
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
                    approveUser.ConstructionManagerCode = AppUsers.ConstructionManager.Code;
                    approveUser.MCCLConsManagerCode = AppUsers.MCCLConsManager.Code;
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
            var comment = ProjectComment.GetSavedComment(Id, "RebuildConsInfo", LastUpdateUserAccount);
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
                comment.RefTableName = "RebuildConsInfo";
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
        public void ExecuteProcess(string employeeCode, string sn, string actionName, string comment, List<ProcessDataField> dataFields = null)
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
                    case ProjectAction.ReSubmit:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Rebuild_ConsInfo_Input, GetProjectStatus(actionName));
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Rebuild_ConsInfo_Upload, GetProjectStatus(actionName));
                        break;
                    default:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Rebuild_ConsInfo_Upload, GetProjectStatus(actionName));
                        break;
                }
                scope.Complete();
            }
            K2FxContext.Current.ApprovalProcess(sn, employeeCode, actionName, comment, dataFields);
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
                    nodeCode = NodeCode.Rebuild_ConsInfo_Confirm;
                    break;
            }
            return nodeCode;
        }
        public void ApproveConsInfo(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, ProjectAction.Approve, Comments);
        }
        public void RejectConsInfo(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, ProjectAction.Decline, Comments);
        }
        public void ReturnConsInfo(string userCode)
        {
            ExecuteProcess(userCode, SerialNumber, ProjectAction.Return, Comments);
        }
        public void ResubmitConsInfo(string userCode)
        {
            List<ProcessDataField> dataField = SetWorkflowDataFields(null);
            ExecuteProcess(userCode, SerialNumber, ProjectAction.ReSubmit, Comments, dataField);
        }
        public override void Recall(string comment)
        {
            if (!ProcInstID.HasValue)
            {
                throw new Exception("The operation needs the process instance id!");
            }
            K2FxContext.Current.GoToActivityAndRecord(ProcInstID.Value,WorkflowActOriginator, ClientCookie.UserCode, ProjectAction.Recall, comment);

            using (var scope = new TransactionScope())
            {
                Save(ProjectAction.Recall);
                ProjectInfo.Reset(ProjectId, WorkflowCode, ProjectStatus.Recalled);
                scope.Complete();
            }
        }
        public ReinvestmentBasicInfo GetReinvestmentBasicInfo(string projectId)
        {
            ReinvestmentBasicInfo reinvestmentBasicInfo;
            using (var context = new McdAMEntities())
            {
                var result = (from mlc in context.RebuildConsInfo
                              join rbi in context.ReinvestmentBasicInfo on mlc.Id equals rbi.ConsInfoID
                              where mlc.ProjectId == projectId && !mlc.IsHistory
                              select rbi).Distinct().AsEnumerable();
                reinvestmentBasicInfo = result.FirstOrDefault();
            }
            return reinvestmentBasicInfo;
        }

    }
}
