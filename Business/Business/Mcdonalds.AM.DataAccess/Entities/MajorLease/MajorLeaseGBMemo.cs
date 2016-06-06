﻿using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Mcdonalds.AM.DataAccess.Common.Extensions;
using Mcdonalds.AM.DataAccess.Constants;
using System.Transactions;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using System.Data.Entity;


namespace Mcdonalds.AM.DataAccess
{
    public partial class MajorLeaseGBMemo : BaseWFEntity<MajorLeaseGBMemo>
    {
        public List<ProjectComment> ProjectComments { get; set; }
        public string Comments { get; set; }
        public string SerialNumber { get; set; }
        public ApproveUsers AppUsers { get; set; }
        public bool IsShowRecall { get; set; }
        public bool IsShowEdit { get; set; }
        public bool IsShowSave { get; set; }
        public string UsCode { get; set; }
        public MajorLeaseInfo Info { get; set; }
        public StoreInfo Store { get; set; }
        public ReinvestmentBasicInfo ReinvestInfo { get; set; }
        public override string WorkflowProcessName
        {
            get
            {
                return @"MCDAMK2Project.MajorLeaseChange\GBMemo";
            }
        }
        public override string WorkflowProcessCode { get { return @"MCD_AM_MLC_GBM"; } }
        public override string TableName { get { return "MajorLeaseGBMemo"; } }
        public static string WorkflowActOriginator { get { return "Originator"; } } // 退回至发起人节点名
        public override string WorkflowCode
        {
            get { return FlowCode.MajorLease_GBMemo; }
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
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            using (var scope = new TransactionScope())
            {
                switch (status)
                {
                    case TaskWorkStatus.K2ProcessApproved:
                        var mjInfo = MajorLeaseInfo.Search(e => e.ProjectId.Equals(ProjectId)).FirstOrDefault();
                        var pmTaskUrl = string.Format(@"/MajorLease/Main#/GBMemo/Process/Notify?projectId={0}", ProjectId);
                        var pmTask = mjInfo.GenerateTaskWork(WorkflowCode, "MajorLease GBMemo", "MajorLease GBMemo",
                            pmTaskUrl);
                        pmTask.ActivityName = "Notify";
                        pmTask.ActionName = "Notify";
                        TaskWork.Add(pmTask);
                        break;
                }
                scope.Complete();
            }
        }
        public override string Edit()
        {
            var taskUrl = string.Format("/MajorLease/Main#/GBMemo?projectId={0}", ProjectId);
            using (var scope = new TransactionScope())
            {
                var rbdInfo = MajorLeaseInfo.Search(e => e.ProjectId.Equals(ProjectId)).FirstOrDefault();
                if (rbdInfo == null)
                {
                    throw new Exception("Could not find the Rebuild Info, please check it!");
                }
                var task = rbdInfo.GenerateTaskWork(WorkflowCode, "MajorLease GBMemo", "MajorLease GBMemo", taskUrl);
                task.ActivityName = NodeCode.Start;
                task.ActionName = SetTaskActionName(ProjectId);
                TaskWork.Add(task);
                CompleteNotifyTask(ProjectId);
                AttachmentsMemoProcessInfo.UpdateNotifyDate(ProjectId, FlowCode.GBMemo,false);
                IsHistory = true;
                Update(this);
                
                Mapper.CreateMap<MajorLeaseGBMemo, MajorLeaseGBMemo>();
                var entity = Mapper.Map<MajorLeaseGBMemo>(this);
                entity.Id = Guid.Empty;
                entity.ProcInstID = null;
                entity.IsHistory = false;
                entity.Comments = null;
                entity.Save("Edit");

                CopyAppUsers(Id.ToString(), entity.Id.ToString());

                scope.Complete();
            }
            return taskUrl;
        }
        public static MajorLeaseGBMemo GetGBMemo(string projectId, string entityId = "")
        {
            var memo = (string.IsNullOrEmpty(entityId) ?
                FirstOrDefault(e => e.ProjectId.Equals(projectId) && !e.IsHistory)
                : FirstOrDefault(e => e.Id.ToString().Equals(entityId))) ?? new MajorLeaseGBMemo();

            memo.ProjectId = projectId;

            var majInfo = new MajorLeaseInfo();
            majInfo = majInfo.GetMajorLeaseInfo(projectId);
            memo.Info = majInfo;
            memo.UsCode = majInfo.USCode;

            memo.Store = StoreBasicInfo.GetStore(memo.UsCode);

            if (memo.Id == Guid.Empty)
            {
                memo.IsClosed = (memo.Store.StoreBasicInfo.statusName == "Closed");
                memo.IsInOperation = false;
                var consInfo = new MajorLeaseConsInfo();
                memo.ReinvestInfo = consInfo.GetReinvestmentBasicInfo(projectId);
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
                                                                   && e.FlowCode == FlowCode.MajorLease_GBMemo);

                if (projectInfo != null)
                {
                    if (ClientCookie.UserCode.Equals(majInfo.PMAccount))
                    {
                        var isFlowFlinshed =
                            TaskWork.Any(e =>
                                    e.RefID == projectId && e.TypeCode == FlowCode.MajorLease_GBMemo &&
                                    e.Status == TaskWorkStatus.K2ProcessApproved && e.ProcInstID == memo.ProcInstID);
                        var isExistTask = TaskWork.Any(e => e.RefID == projectId
                                                                    && e.TypeCode == FlowCode.MajorLease_GBMemo
                                                                    && e.Status == TaskWorkStatus.UnFinish
                                                                    && e.ReceiverAccount == ClientCookie.UserCode
                                                                    && (e.ActivityName == WFMajorLeaseLegalReview.Act_Originator || e.ActivityName == WFMajorLeaseLegalReview.Act_Start));
                        memo.IsShowEdit = isFlowFlinshed;
                        memo.IsShowRecall = !isFlowFlinshed && !isExistTask;
                    }
                }
            }
            //if (ClientCookie.UserCode.Equals(majInfo.PMAccount))
            //    memo.IsShowSave = ProjectInfo.IsFlowSavable(projectId, FlowCode.MajorLease_GBMemo);
            PopulateAppUsers(memo);
            return memo;
        }
        private static void PopulateAppUsers(MajorLeaseGBMemo entity)
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
                if (entity.Info != null)
                {
                    var simp = new SimpleEmployee
                    {
                        Code = entity.Info.CMAccount
                    };
                    entity.AppUsers.ConstructionManager = simp;
                }
            }
        }
        public void Submit()
        {
            var task = TaskWork.GetTaskWork(ProjectId, ClientCookie.UserCode, TaskWorkStatus.UnFinish,
                FlowCode.MajorLease, FlowCode.MajorLease_GBMemo);
            bool isNoTask = false;
            if (task==null)
            {
                string taskUrl = "/MajorLease/Main#/GBMemo/Process/View?projectId=" + ProjectId;
                task = Info.GenerateTaskWork(FlowCode.MajorLease_GBMemo, "GBMemo", "GBMemo", taskUrl);
                task.Status = TaskWorkStatus.Finished;
                task.FinishTime = DateTime.Now;
                isNoTask = true;
            }
            ProcInstID = StartProcess(task);

            using (var scope = new TransactionScope())
            {
                task.Status = TaskWorkStatus.Finished;
                task.FinishTime = DateTime.Now;
                if (isNoTask)
                    TaskWork.Add(task);
                else
                {
                    TaskWork.Update(task);
                }
                Save("Submit");
                ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.MajorLease_GBMemo_Input);
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
                if (!action.Equals("Edit"))
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
                        approveUser.FlowCode = FlowCode.MajorLease_GBMemo;
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
        private int StartProcess(TaskWork task)
        {
            CreateUserAccount = LastUpdateUserAccount;
            var processDataFields = SetWorkflowDataFields(task);
            return K2FxContext.Current.StartProcess(WorkflowProcessCode, CreateUserAccount, processDataFields);
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
                        ProjectInfo.Reset(ProjectId, WorkflowCode, ProjectStatus.Finished);
                        break;
                    case ProjectAction.Decline:
                        ProjectInfo.Reject(ProjectId, WorkflowCode, ProjectStatus.Finished);
                        break;
                    default:
                        ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.MajorLease_GBMemo_Input, ProjectStatus.Finished);
                        break;
                }
                //GBMemo alaways is finished
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
                    nodeCode = NodeCode.MajorLease_GBMemo_Send;
                    break;
            }
            return nodeCode;
        }
    }
}
