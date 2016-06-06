using AutoMapper;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.DataAccess.Workflow;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   9/28/2014 4:09:23 PM
 * FileName     :   RenewalRenewLetter
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Mcdonalds.AM.DataAccess
{
    public partial class RenewalLetter : BaseWFEntity<RenewalLetter>, IWorkflowEntity
    {
        public override string WorkflowProcessName { get { return @"MCDAMK2Project.Renewal\RenewLetter"; } }

        public override string WorkflowProcessCode
        {
            get { return "MCD_AM_Renewal_RL"; }
        }

        public override string TableName
        {
            get
            {
                return "RenewalLetter";
            }
        }
        public override string WorkflowCode
        {
            get
            {
                return FlowCode.Renewal_Letter;
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
        public static void Create(string projectId, string userAccount)
        {
            var letter = new RenewalLetter();
            letter.Id = Guid.NewGuid();
            letter.ProjectId = projectId;
            letter.CreateTime = DateTime.Now;
            letter.CreateUserAccount = userAccount;
            letter.Add();
        }

        public static RenewalLetter Get(string projectId, string id = "")
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

        public void Save(string comment)
        {
            var SavedComment = ProjectComment.GetSavedComment(this.Id, this.TableName, ClientCookie.UserCode);
            if (SavedComment != null)
            {
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
        }
        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            RenewalInfo info = RenewalInfo.Get(this.ProjectId);
            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_MCCLAssetManager",info.AssetManagerAccount),
                new ProcessDataField("ProcessCode", WorkflowProcessCode)
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
            var actor = ProjectUsers.GetProjectUser(this.ProjectId, ProjectUserRoleCode.AssetActor);
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
                    if (onExecuting != null)
                    {
                        onExecuting();
                    }
                    ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Renewal_Letter_Upload);
                    ProjectProgress.SetProgress(ProjectId, "20%");
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

        public void Return(string comment, string SerialNumber)
        {
            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Return", comment);
            using (TransactionScope tranScope = new TransactionScope())
            {
                //TaskWork.Finish(t => t.ReceiverAccount == ClientCookie.UserCode && t.Status == TaskWorkStatus.UnFinish && t.TypeCode == this.WorkflowCode && t.RefID == this.ProjectId);
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

        public void Resubmit(string comment, string SerialNumber)
        {
            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Resubmit", comment);
            using (TransactionScope tranScope = new TransactionScope())
            {
                this.CreateUserAccount = ClientCookie.UserCode;
                this.Update();
                ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Renewal_Letter_Upload);
                ProjectComment.AddComment(
                    ProjectCommentAction.ReSubmit,
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

        public override void Recall(string comment)
        {
            K2FxContext.Current.GoToActivityAndRecord(
                this.ProcInstId.Value,
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
                var letter = Duplicator.AutoCopy(this);
                letter.Id = Guid.NewGuid();
                letter.IsHistory = false;
                letter.CreateUserAccount = ClientCookie.UserCode;
                letter.Add();
                IsHistory = true;
                this.Update();
                ProjectInfo.Reset(ProjectId, this.WorkflowCode);
                var attachments = Attachment.GetList(this.TableName, Id.ToString(), string.Empty);
                attachments.ForEach(att =>
                {
                    att.RefTableID = letter.Id.ToString();
                    att.ID = Guid.NewGuid();
                });
                Attachment.Add(attachments.ToArray());
                var assetActor = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == ProjectId && pu.RoleCode == ProjectUserRoleCode.AssetActor);
                var oldTasks = TaskWork.Search(t => t.RefID == ProjectId && t.Status == TaskWorkStatus.UnFinish && (t.TypeCode == this.WorkflowCode || t.TypeCode == FlowCode.Renewal_ConsInfo)).ToList();
                oldTasks.ForEach(t =>
                {
                    t.Status = TaskWorkStatus.Cancel;
                });
                TaskWork.Update(oldTasks.ToArray());
                var latestTask = TaskWork.FirstOrDefault(t => t.RefID == ProjectId && t.TypeCode == this.WorkflowCode);
                string url = "/Renewal/Main#/Letter?projectId=" + ProjectId;
                TaskWork.SendTask(ProjectId, latestTask.Title, latestTask.StoreCode, url, assetActor, FlowCode.Renewal, this.WorkflowCode, "Start");
                tranScope.Complete();
                return url;
            }
        }

        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            switch (status)
            {
                case TaskWorkStatus.K2ProcessApproved:
                    {
                        ProjectInfo.FinishNode(ProjectId, this.WorkflowCode, NodeCode.Finish);
                        ProjectInfo.FinishNode(ProjectId, FlowCode.Renewal_LLNegotiation, NodeCode.Renewal_LLNegotiation_ConfirmLetter);
                        if (!ProjectInfo.IsFlowStarted(ProjectId, FlowCode.Renewal_ConsInfo))
                        {
                            var info = RenewalInfo.Get(task.RefID);
                            if (ProjectInfo.IsFlowFinished(ProjectId, FlowCode.Renewal_LLNegotiation))
                            {
                                ProjectProgress.SetProgress(ProjectId, "30%");
                                if (info.NeedProjectCostEst)
                                {
                                    if (!ProjectInfo.IsFlowStarted(ProjectId, FlowCode.Renewal_ConsInfo))
                                    {
                                        info.GenerateSubmitTask(FlowCode.Renewal_ConsInfo);
                                    }
                                }
                                else
                                {
                                    ProjectInfo.FinishNode(ProjectId, FlowCode.Renewal_ConsInfo, NodeCode.Finish, ProjectStatus.Finished);
                                    if (!ProjectInfo.IsFlowStarted(ProjectId, FlowCode.Renewal_Tool))
                                    {
                                        info.GenerateSubmitTask(FlowCode.Renewal_Tool);
                                    }
                                }
                            }
                        }
                        TaskWork.Finish(t => t.TypeCode == this.WorkflowCode && t.RefID == this.ProjectId && t.Status == TaskWorkStatus.UnFinish);
                    }
                    break;
            }
        }
    }
}
