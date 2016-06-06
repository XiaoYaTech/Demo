using AutoMapper;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   9/28/2014 4:24:18 PM
 * FileName     :   RenewalConsInfo
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Mcdonalds.AM.DataAccess
{
    public partial class RenewalConsInfo : BaseWFEntity<RenewalConsInfo>
    {
        public override string WorkflowCode
        {
            get
            {
                return FlowCode.Renewal_ConsInfo;
            }
        }
        public override string TableName
        {
            get
            {
                return "RenewalConsInfo";
            }
        }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public override string WorkflowActOriginator
        {
            get
            {
                return "Originator";
            }
        }

        public override string[] WorkflowNormalActors
        {
            get
            {
                return base.WorkflowNormalActors;
            }
        }

        public override string WorkflowProcessCode
        {
            get { return "MCD_AM_Renewal_CI"; }
        }

        public override string WorkflowProcessName
        {
            get { return @"MCDAMK2Project.Renewal\ConsInfo"; }
        }

        public ApproveUsers AppUsers { get; set; }
        public static void Create(string projectId, string createUserAccount, bool hasReinvenstment)
        {
            RenewalConsInfo consInfo = new RenewalConsInfo();
            consInfo.Id = Guid.NewGuid();
            consInfo.ProjectId = projectId;
            consInfo.CreateTime = DateTime.Now;
            consInfo.CreateUserAccount = createUserAccount;
            consInfo.HasReinvenstment = hasReinvenstment;
            consInfo.Add();
        }

        public static ConsInfoDTO<RenewalInfo, RenewalConsInfo> InitPage(string projectId, string id = "")
        {

            RenewalInfo info = RenewalInfo.Get(projectId);
            var consInfo = RenewalConsInfo.Get(projectId, id);
            if (consInfo == null)
            {
                consInfo = new RenewalConsInfo();
                consInfo.ProjectId = projectId;
            }
            var isOriginator = ClientCookie.UserCode == info.PMAccount;
            consInfo.IsProjectFreezed = consInfo.CheckIfFreezeProject(projectId);
            var nextRefTableId = new Guid(FlowInfo.GetRefTableId("RenewalTool", projectId));
            var nextFlowStarted = ProjectInfo.IsFlowStarted(projectId, FlowCode.Renewal_Tool);
            var haveTask = TaskWork.Any(t => t.RefID == projectId && t.TypeCode == FlowCode.Renewal_Tool && t.Status == TaskWorkStatus.UnFinish && t.ReceiverAccount == ClientCookie.UserCode);
            var projectComment = ProjectComment.GetSavedComment(consInfo.Id, "RenewalConsInfo", ClientCookie.UserCode);
            if (string.IsNullOrEmpty(id))
                consInfo.HasReinvenstment = info.NeedProjectCostEst;
            ConsInfoDTO<RenewalInfo, RenewalConsInfo> dto = new ConsInfoDTO<RenewalInfo, RenewalConsInfo>();
            dto.Entity = consInfo;
            dto.Info = info;
            dto.ReinBasicInfo = ReinvestmentBasicInfo.GetByConsInfoId(consInfo.Id);
            dto.ReinCost = ReinvestmentCost.GetByConsInfoId(consInfo.Id);
            dto.WriteOff = WriteOffAmount.GetByConsInfoId(consInfo.Id);
            dto.ProjectComment = projectComment != null ? projectComment.Content : "";
            dto.Editable = ProjectInfo.IsFlowEditable(projectId, FlowCode.Renewal_ConsInfo);
            dto.Recallable = ProjectInfo.IsFlowRecallable(projectId, FlowCode.Renewal_ConsInfo);
            dto.Savable = ProjectInfo.IsFlowSavable(projectId, FlowCode.Renewal_ConsInfo) && string.IsNullOrEmpty(id);
            return dto;
        }

        public static RenewalConsInfo Get(string projectId, string id = "")
        {
            if (!string.IsNullOrEmpty(id))
            {
                return Get(new Guid(id));
            }
            else
            {
                return FirstOrDefault(l => l.ProjectId == projectId && l.IsHistory == false);
            }
        }

        public override BaseWFEntity GetWorkflowInfo(string projectId, string id = "")
        {
            var entity = Get(projectId, id);
            if (entity != null)
            {
                entity.EntityId = entity.Id;
            }
            return entity;
        }

        public void Save(string comment, Action onExecuting = null)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                var SavedComment = ProjectComment.GetSavedComment(this.Id, this.TableName, ClientCookie.UserCode);
                if (SavedComment != null)
                {
                    SavedComment.Action = ProjectCommentAction.Submit;
                    SavedComment.Status = ProjectCommentStatus.Save;
                    SavedComment.Content = comment;
                    SavedComment.CreateTime = DateTime.Now;
                    SavedComment.Update();
                }
                else
                {
                    ProjectComment.AddComment(
                        ProjectCommentAction.Submit,
                        comment,
                        this.Id,
                        this.TableName,
                        FlowCode.Renewal,
                        null,
                        ProjectCommentStatus.Save
                    );
                }
                if (onExecuting != null)
                {
                    onExecuting();
                }
                tranScope.Complete();
            }
        }
        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            RenewalInfo info = RenewalInfo.Get(this.ProjectId);
            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_ConsMgr",!this.HasReinvenstment?"": AppUsers.ConstructionManager.Code ),
                new ProcessDataField("dest_MCCLConsMgr",!this.HasReinvenstment?"":AppUsers.MCCLConsManager.Code),
                new ProcessDataField("ProcessCode",WorkflowProcessCode),
                new ProcessDataField("IsNoReinvestment",(!this.HasReinvenstment).ToString(),"BOOLEAN")
            };

            if (task != null)
            {
                processDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                processDataFields.Add(new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task)));
            }
            return processDataFields;
        }
        public void Submit(string comment, Action onExecuting = null)
        {
            var task = TaskWork.GetTaskWork(ProjectId, ClientCookie.UserCode, TaskWorkStatus.UnFinish,
                FlowCode.Renewal, WorkflowCode);
            var dataFields = SetWorkflowDataFields(task);
            var procInstId = K2FxContext.Current.StartProcess(WorkflowProcessCode, ClientCookie.UserCode, dataFields);
            if (procInstId > 0)
            {
                using (TransactionScope tranScope = new TransactionScope())
                {
                    task.Finish();
                    this.ProcInstId = procInstId;
                    this.CreateTime = DateTime.Now;
                    this.CreateUserAccount = ClientCookie.UserCode;
                    this.Update();
                    if (this.HasReinvenstment)
                    {
                        SaveApprovers();
                    }
                    var project = ProjectInfo.Get(this.ProjectId, this.WorkflowCode);
                    project.CreateUserAccount = ClientCookie.UserCode;
                    project.Update();
                    var SavedComment = ProjectComment.GetSavedComment(this.Id, this.TableName, ClientCookie.UserCode);
                    if (SavedComment != null)
                    {
                        SavedComment.Status = ProjectCommentStatus.Submit;
                        SavedComment.Content = comment;
                        SavedComment.CreateTime = DateTime.Now;
                        SavedComment.Update();
                    }
                    else
                    {
                        ProjectComment.AddComment(
                            ProjectCommentAction.Submit,
                            comment,
                            this.Id,
                            this.TableName,
                            FlowCode.Renewal,
                            procInstId,
                            ProjectCommentStatus.Submit
                        );
                    }
                    if (HasReinvenstment)
                        ProjectInfo.FinishNode(this.ProjectId, this.WorkflowCode, NodeCode.Renewal_ConsInfo_Input);
                    if (onExecuting != null)
                    {
                        onExecuting();
                    }
                    tranScope.Complete();
                }
            }
        }

        public void Approve(string comment, string SerialNumber)
        {
            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Approve", comment);
            ProjectComment.AddComment(
                ProjectCommentAction.Approve,
                comment,
                this.Id,
                this.TableName,
                FlowCode.Renewal,
                this.ProcInstId,
                ProjectCommentStatus.Submit
            );
        }

        public void Return(string comment, string SerialNumber)
        {
            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Return", comment);
            using (TransactionScope tranScope = new TransactionScope())
            {
                ProjectInfo.Reset(this.ProjectId, this.WorkflowCode);
                ProjectComment.AddComment(
                    ProjectCommentAction.Return,
                    comment,
                    this.Id,
                    this.TableName,
                    FlowCode.Renewal,
                    this.ProcInstId,
                    ProjectCommentStatus.Submit
                );
                tranScope.Complete();
            }
        }
        public void Reject(string comment, string SerialNumber)
        {
            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Reject", comment);
            using (TransactionScope tranScope = new TransactionScope())
            {
                ProjectInfo.Reject(this.ProjectId, this.WorkflowCode);
                ProjectComment.AddComment(
                    ProjectCommentAction.Return,
                    comment,
                    this.Id,
                    this.TableName,
                    FlowCode.Renewal,
                    this.ProcInstId,
                    ProjectCommentStatus.Submit
                );
                tranScope.Complete();
            }
        }

        public void Resubmit(string comment, string SerialNumber, Action onExecuting = null)
        {
            var task = TaskWork.FirstOrDefault(t => t.RefID == this.ProjectId && t.TypeCode == this.WorkflowCode && t.ReceiverAccount == ClientCookie.UserCode && t.Status == TaskWorkStatus.UnFinish);
            var dataFields = SetWorkflowDataFields(task);
            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Resubmit", comment, dataFields);
            using (TransactionScope tranScope = new TransactionScope())
            {
                this.CreateUserAccount = ClientCookie.UserCode;
                this.Update();
                if (this.HasReinvenstment)
                {
                    SaveApprovers();
                }
                ProjectInfo.FinishNode(this.ProjectId, this.WorkflowCode, NodeCode.Renewal_ConsInfo_Input);
                ProjectComment.AddComment(
                    ProjectCommentAction.ReSubmit,
                    comment,
                    this.Id,
                    this.TableName,
                    FlowCode.Renewal,
                    this.ProcInstId,
                    ProjectCommentStatus.Submit
                );
                if (onExecuting != null)
                {
                    onExecuting();
                }
                tranScope.Complete();
            }
        }

        public override void Recall(string comment)
        {
            K2FxContext.Current.GoToActivityAndRecord(
                this.ProcInstId,
                this.WorkflowActOriginator,
                ClientCookie.UserCode,
                ProjectAction.Recall,
                comment
            );
            using (TransactionScope tranScope = new TransactionScope())
            {
                ProjectComment.AddComment(
                    ProjectCommentAction.Recall,
                    comment,
                    this.Id,
                    this.TableName,
                    FlowCode.Renewal,
                    this.ProcInstId,
                    ProjectCommentStatus.Submit
                );
                ProjectInfo.Reset(this.ProjectId, this.WorkflowCode, ProjectStatus.Recalled);
                tranScope.Complete();
            }
        }

        public override string Edit()
        {
            using (var tranScope = new TransactionScope())
            {
                var info = RenewalInfo.Get(this.ProjectId);
                var consInfo = Duplicator.AutoCopy(this);
                consInfo.Id = Guid.NewGuid();
                consInfo.IsHistory = false;
                consInfo.CreateUserAccount = ClientCookie.UserCode;
                consInfo.Add();
                IsHistory = true;
                this.Update();
                ProjectInfo.Reset(ProjectId, this.WorkflowCode);
                var attachments = Attachment.GetList(this.TableName, Id.ToString(), string.Empty);
                attachments.ForEach(att =>
                {
                    att.RefTableID = consInfo.Id.ToString();
                    att.ID = Guid.NewGuid();
                });
                Attachment.Add(attachments.ToArray());
                var reinBasic = ReinvestmentBasicInfo.GetByConsInfoId(this.Id);
                if (reinBasic != null)
                {
                    var newReinBasic = Duplicator.AutoCopy(reinBasic);
                    newReinBasic.Id = 0;
                    newReinBasic.ConsInfoID = consInfo.Id;
                    newReinBasic.Add();
                }
                var wf = WriteOffAmount.GetByConsInfoId(this.Id);
                if (wf != null)
                {
                    var newWf = Duplicator.AutoCopy(wf);
                    newWf.Id = Guid.NewGuid();
                    newWf.ConsInfoID = consInfo.Id;
                    newWf.Add();
                }
                var reinCost = ReinvestmentCost.GetByConsInfoId(this.Id);
                if (reinCost != null)
                {
                    var newReinCost = Duplicator.AutoCopy(reinCost);
                    newReinCost.Id = Guid.NewGuid();
                    newReinCost.ConsInfoID = consInfo.Id;
                    newReinCost.Add();
                }
                var oldTasks = TaskWork.Search(t => t.RefID == ProjectId
                    && t.Status == TaskWorkStatus.UnFinish
                    && new string[] { FlowCode.Renewal_ConsInfo, FlowCode.Renewal_Tool }.Contains(t.TypeCode)).ToList();
                oldTasks.ForEach(t =>
                {
                    t.Status = TaskWorkStatus.Cancel;
                });
                TaskWork.Update(oldTasks.ToArray());
                var toolUploadTask = TaskWork.FirstOrDefault(t => t.RefID == ProjectId
                            && t.TypeCode == FlowCode.Renewal_Tool
                            && t.ReceiverAccount == info.AssetActorAccount
                            && t.ActivityName == "AssetActor"
                            && t.Status == TaskWorkStatus.UnFinish);
                if (toolUploadTask != null)
                {
                    toolUploadTask.Status = TaskWorkStatus.Cancel;
                    toolUploadTask.Update();
                }
                CompleteRenewalToolTask();
                var task = info.GenerateSubmitTask(this.WorkflowCode);
                tranScope.Complete();
                return task.Url;
            }
        }

        private void CompleteRenewalToolTask()
        {
            var toolTask = TaskWork.FirstOrDefault(e => e.Status == TaskWorkStatus.UnFinish && e.RefID == ProjectId && e.TypeCode == FlowCode.Renewal_Tool);
            if (toolTask != null)
            {
                toolTask.Status = TaskWorkStatus.Finished;
                toolTask.FinishTime = DateTime.Now;
                toolTask.Update();
            }
        }

        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            RenewalInfo info = RenewalInfo.Get(this.ProjectId);
            switch (status)
            {
                case TaskWorkStatus.K2ProcessApproved:
                    //var toolUploadTask = TaskWork.FirstOrDefault(t => t.RefID == ProjectId
                    //        && t.TypeCode == FlowCode.Renewal_Tool
                    //        && t.ReceiverAccount == info.AssetActorAccount
                    //        && t.ActivityName == "AssetActor"
                    //        && t.Status == TaskWorkStatus.Cancel);
                    //if (toolUploadTask != null)
                    //{
                    //    toolUploadTask.Status = TaskWorkStatus.UnFinish;
                    //    toolUploadTask.Update();
                    //}
                    //else if (!ProjectInfo.IsFlowStarted(this.ProjectId, FlowCode.Renewal_Tool))
                    //{
                    //    info.GenerateSubmitTask(FlowCode.Renewal_Tool);
                    //}
                    var tool = RenewalTool.Get(ProjectId);
                    Guid entityId;
                    var toolProj = ProjectInfo.FirstOrDefault(e => e.Status == ProjectStatus.Finished && e.ProjectId == ProjectId && e.FlowCode == FlowCode.Renewal_Tool);
                    if (toolProj != null)
                    {
                        NoticeToActor(info.AssetActorAccount, info.USCode);
                        tool.Edit();
                        entityId = tool.NewEntityId;
                    }
                    else
                    {
                        entityId = tool.Id;
                        ProjectInfo.Reset(this.ProjectId, FlowCode.Renewal_Tool);
                        info.GenerateSubmitTask(FlowCode.Renewal_Tool);
                    }


                    var entity = RenewalToolWriteOffAndReinCost.FirstOrDefault(w => w.ToolId == entityId);
                    if (entity == null)
                    {
                        entity = new RenewalToolWriteOffAndReinCost();
                        entity.Id = Guid.NewGuid();
                        entity.ToolId = tool.Id;
                        entity.Add();
                    }
                    var writeOffAmount = WriteOffAmount.GetByConsInfoId(this.Id);
                    var reinCost = ReinvestmentCost.GetByConsInfoId(this.Id);
                    if (writeOffAmount != null)
                    {
                        entity.REWriteOff = DataConverter.ToDecimal(writeOffAmount.REWriteOff);
                        entity.LHIWriteOff = DataConverter.ToDecimal(writeOffAmount.LHIWriteOff);
                        entity.ESSDWriteOff = DataConverter.ToDecimal(writeOffAmount.ESSDWriteOff);
                    }
                    else
                    {
                        entity.REWriteOff = 0;
                        entity.LHIWriteOff = 0;
                        entity.ESSDWriteOff = 0;
                    }
                    if (reinCost != null)
                    {
                        entity.RECost = DataConverter.ToDecimal(reinCost.RECostNorm);
                        entity.LHICost = DataConverter.ToDecimal(reinCost.LHINorm);
                        entity.ESSDCost = DataConverter.ToDecimal(reinCost.ESSDNorm);
                    }
                    else
                    {
                        entity.RECost = 0;
                        entity.LHICost = 0;
                        entity.ESSDCost = 0;
                    }
                    Log4netHelper.WriteInfo(JsonConvert.SerializeObject(new { desc = "renewal tool info:", entityId, tool, writeOffAmount, reinCost, Id }));
                    entity.Update();
                    if (HasReinvenstment)
                        ProjectInfo.FinishNode(this.ProjectId, this.WorkflowCode, NodeCode.Renewal_ConsInfo_Approval);

                    break;
                case TaskWorkStatus.K2ProcessDeclined:
                    break;
            }
            task.Finish();
        }

        private void NoticeToActor(string strAssetActorAccount, string usCode)
        {
            //通知Asset Actor
            var consinfo = FirstOrDefault(i => i.ProjectId == ProjectId && i.IsHistory);
            if (consinfo != null)
            {
                List<string> receiverList = new List<string>();
                receiverList.Add(strAssetActorAccount);
                var notificationMsg = new NotificationMsg()
                {
                    FlowCode = FlowCode.Renewal_ConsInfo,
                    ProjectId = ProjectId,
                    SenderCode = ClientCookie.UserCode,
                    Title = "Renewal ConsInfo已经重新编辑",
                    RefId = consinfo.Id,
                    UsCode = usCode,
                    IsSendEmail = false,
                    ReceiverCodeList = receiverList
                };
                Notification.Send(notificationMsg);
            }
        }

        public ReinvestmentBasicInfo GetReinvestmentBasicInfo(string projectId)
        {
            ReinvestmentBasicInfo reinvestmentBasicInfo;
            using (var context = new McdAMEntities())
            {
                var result = (from mlc in context.RenewalConsInfo
                              join rbi in context.ReinvestmentBasicInfo on mlc.Id equals rbi.ConsInfoID
                              where mlc.ProjectId == projectId && !mlc.IsHistory
                              select rbi).Distinct().AsEnumerable();
                reinvestmentBasicInfo = result.FirstOrDefault();
            }
            return reinvestmentBasicInfo;
        }
        private void SaveApprovers()
        {
            bool isNew = false;
            ApproveDialogUser approvers = ApproveDialogUser.GetApproveDialogUser(this.Id.ToString());
            if (approvers == null)
            {
                approvers = new ApproveDialogUser();
                approvers.Id = Guid.NewGuid();
                approvers.ProjectId = this.ProjectId;
                approvers.FlowCode = this.WorkflowCode;
                approvers.RefTableID = this.Id.ToString();
                isNew = true;
            }
            approvers.LastUpdateDate = DateTime.Now;
            approvers.LastUpdateUserAccount = ClientCookie.UserCode;
            approvers.ConstructionManagerCode = AppUsers.ConstructionManager.Code;
            approvers.MCCLConsManagerCode = AppUsers.MCCLConsManager.Code;
            if (isNew)
            {
                approvers.Add();
            }
            else
            {
                approvers.Update();
            }
        }
    }
}
