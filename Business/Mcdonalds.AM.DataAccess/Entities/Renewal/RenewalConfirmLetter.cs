using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   10/18/2014 3:28:22 PM
 * FileName     :   RenewalConfirmLetter
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
    public partial class RenewalConfirmLetter : BaseWFEntity<RenewalConfirmLetter>
    {
        public override string WorkflowCode
        {
            get
            {
                return FlowCode.Renewal_ConfirmLetter;
            }
        }
        public override string TableName
        {
            get
            {
                return "RenewalConfirmLetter";
            }
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
            get { return "MCD_AM_Renewal_CL"; }
        }

        public override string WorkflowProcessName
        {
            get { return @"MCDAMK2Project.Renewal\ConfirmLetter"; }
        }
        public static void Create(string projectId, string userAccount)
        {
            RenewalConfirmLetter rcl = new RenewalConfirmLetter();
            rcl.Id = Guid.NewGuid();
            rcl.ProjectId = projectId;
            rcl.CreateTime = DateTime.Now;
            rcl.CreateUserAccount = userAccount;
            rcl.Add();
        }

        public static RenewalConfirmLetter Get(string projectId, string id = "")
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

        public void Save()
        {
            this.CreateUserAccount = ClientCookie.UserCode;
            this.LastUpdateUserAccount = ClientCookie.UserCode;
            this.LastUpdateTime = DateTime.Now;
            this.Update();
        }

        public void Submit()
        {
            var task = TaskWork.GetTaskWork(ProjectId, ClientCookie.UserCode, TaskWorkStatus.UnFinish,
                FlowCode.Renewal, WorkflowCode);
            var dataFields = SetWorkflowDataFields(task);
            var procInstId = K2FxContext.Current.StartProcess(WorkflowProcessCode, ClientCookie.UserCode, dataFields);
            if (procInstId > 0)
            {
                using (TransactionScope tranScope = new TransactionScope())
                {
                    this.ProcInstId = procInstId;
                    Save();
                    tranScope.Complete();
                }
            }
        }

        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_Creator", ClientCookie.UserCode),
                new ProcessDataField("ProcessCode", WorkflowProcessCode),
                new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task))
            };

            if (task != null)
            {
                processDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                processDataFields.Add(new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task)));
            }
            return processDataFields;
        }
        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            var info = RenewalInfo.Get(ProjectId);
            switch (status)
            {
                case TaskWorkStatus.K2ProcessApproved:
                    ProjectProgress.SetProgress(ProjectId, "70%");
                    ProjectInfo.FinishNode(ProjectId, FlowCode.Renewal_ConfirmLetter, NodeCode.Finish, ProjectStatus.Finished);
                    if (!ProjectInfo.IsFlowStarted(ProjectId, FlowCode.Renewal_LegalApproval))
                    {
                        info.GenerateSubmitTask(FlowCode.Renewal_LegalApproval);
                    }
                    break;
                case TaskWorkStatus.K2ProcessDeclined:
                    break;
            }
        }

        public void Resubmit(string serialNumber)
        {
            var comment = "";
            var task = TaskWork.FirstOrDefault(t => t.RefID == this.ProjectId && t.TypeCode == this.WorkflowCode && t.ReceiverAccount == ClientCookie.UserCode && t.Status == TaskWorkStatus.UnFinish);
            var dataFields = SetWorkflowDataFields(task);
            K2FxContext.Current.ApprovalProcess(serialNumber, ClientCookie.UserCode, "Resubmit", comment, dataFields);
            using (TransactionScope tranScope = new TransactionScope())
            {
                Save();
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

        public override string Edit()
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                var info = RenewalInfo.Get(ProjectId);

                this.IsHistory = true;
                this.Update();
                var entity = Duplicator.AutoCopy(this);
                entity.Id = Guid.NewGuid();
                entity.IsHistory = false;
                entity.Add();
                var attachments = Attachment.GetList(this.TableName, Id.ToString(), string.Empty);
                attachments.ForEach(att =>
                {
                    att.RefTableID = entity.Id.ToString();
                    att.ID = Guid.NewGuid();
                });
                Attachment.Add(attachments.ToArray());
                TaskWork.Cancel(e => e.TypeCode == this.WorkflowCode && e.RefID == ProjectId && e.Status == TaskWorkStatus.UnFinish);
                ProjectInfo.Reset(ProjectId, this.WorkflowCode);
                var task = info.GenerateSubmitTask(this.WorkflowCode);
                tranScope.Complete();

                return task.Url;
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
    }
}
