using Mcdonalds.AM.DataAccess.Common.Excel;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   9/28/2014 4:23:26 PM
 * FileName     :   RenewalLLNegotiation
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
using System.Web;

namespace Mcdonalds.AM.DataAccess
{
    public partial class RenewalLLNegotiation : BaseWFEntity<RenewalLLNegotiation>
    {
        public override string WorkflowCode
        {
            get
            {
                return FlowCode.Renewal_LLNegotiation;
            }
        }
        public override string TableName
        {
            get
            {
                return "RenewalLLNegotiation";
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
            get { return "MCD_AM_Renewal_LN"; }
        }

        public override string WorkflowProcessName
        {
            get
            {
                return @"MCDAMK2Project.Renewal\LLNegotiation";
            }
        }
        public static void Create(string projectId, string createUserAccount)
        {
            RenewalLLNegotiation llNego = new RenewalLLNegotiation();
            llNego.Id = Guid.NewGuid();
            llNego.ProjectId = projectId;
            llNego.CreateTime = DateTime.Now;
            llNego.CreateUserAccount = createUserAccount;
            llNego.IsHistory = false;
            llNego.Add();
        }

        public static RenewalLLNegotiation Get(string projectId, string id = "")
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
                    UpdateStoreLLRecord();
                    ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Renewal_LLNegotiation_Input);
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
                    ProjectInfo.FinishNode(ProjectId, FlowCode.Renewal_LLNegotiation, NodeCode.Finish, ProjectStatus.Finished);
                    if (ProjectInfo.IsFlowFinished(ProjectId, FlowCode.Renewal_Letter))
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
                ProjectInfo.FinishNode(ProjectId, WorkflowCode, NodeCode.Renewal_LLNegotiation_Input);
                tranScope.Complete();
            }
        }

        public override string Edit()
        {
            var info = RenewalInfo.Get(ProjectId);
            var entity = Duplicator.AutoCopy(this);
            entity.Id = Guid.NewGuid();
            entity.IsHistory = false;
            entity.Add();
            this.IsHistory = true;
            this.Update();
            var records = RenewalLLNegotiationRecord.GetRecords(this.Id);
            records.ForEach(rec =>
            {
                rec.Id = Guid.NewGuid();
                rec.RenewalLLNegotiationId = entity.Id;
            });
            RenewalLLNegotiationRecord.Add(records.ToArray());
            TaskWork.Cancel(e => e.TypeCode == this.WorkflowCode && e.RefID == ProjectId && e.Status == TaskWorkStatus.UnFinish);
            ProjectInfo.Reset(ProjectId, this.WorkflowCode);
            var task = info.GenerateSubmitTask(this.WorkflowCode);
            return task.Url;
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

        public string GenerateAttachment()
        {
            var records = RenewalLLNegotiationRecord.GetRecords(Id);
            var templateName = HttpContext.Current.Server.MapPath("~/Template/Renewal_LLNegotiationRecord_Template.xlsx");
            string fileName = string.Concat(HttpContext.Current.Server.MapPath("~/UploadFiles/"), Guid.NewGuid(), ".xlsx");
            File.Copy(templateName, fileName);
            FileInfo file = new FileInfo(fileName);
            ExcelDataInputDirector excelDirector = new ExcelDataInputDirector(file, ExcelDataInputType.RenewalLLNegotiationRecord);
            List<ExcelInputDTO> datas = records.Select(r => new ExcelInputDTO
            {
                McdParticipants = r.McdParticipants,
                Content = r.Content,
                LLParticipants = r.LLParticipants,
                Topic = r.Topic,
                Location = r.Location,
                MeetingDate = r.Date.Value.ToString("yyyy-MM-dd"),
                CreateDate = r.CreateTime.ToString("yyyy-MM-dd")
            }).ToList();
            excelDirector.ListInput(datas);
            Guid reqId = new Guid("f314bf06-f557-4893-ae78-af3b0b561885");
            Attachment att = Attachment.Get(Id.ToString(), reqId);

            if (att == null)
            {
                att = new Attachment();
                att.InternalName = Path.GetFileName(fileName);
                att.RefTableName = this.TableName;
                att.RefTableID = Id.ToString();
                att.RelativePath = "//";
                att.Name = "Negotiation Record List";
                att.Extension = ".xlsx";
                att.Length = (int)file.Length;
                att.CreateTime = DateTime.Now;
                att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                att.CreatorNameENUS = ClientCookie.UserNameENUS;
                att.CreatorID = ClientCookie.UserCode;
                att.ID = Guid.NewGuid();
                att.RequirementId = reqId;
                att.TypeCode = "";
                Attachment.Add(att);
            }
            else
            {
                att.InternalName = Path.GetFileName(fileName);
                att.RefTableName = this.TableName;
                att.RefTableID = Id.ToString();
                att.RelativePath = "//";
                att.Name = "Negotiation Record List";
                att.Extension = ".xlsx";
                att.Length = (int)file.Length;
                att.CreateTime = DateTime.Now;
                att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                att.CreatorNameENUS = ClientCookie.UserNameENUS;
                att.CreatorID = ClientCookie.UserCode;
                Attachment.Update(att);
            }
            return fileName;
        }

        public void UpdateStoreLLRecord()
        {
            var info = RenewalInfo.Get(this.ProjectId);
            var negotiations = RenewalLLNegotiationRecord.GetRecords(this.Id);
            if (negotiations.Count > 0)
            {
                StoreSTLLRecord record = StoreSTLLRecord.Get(info.USCode);
                if (record == null)
                {
                    var storeBasic = StoreBasicInfo.GetStorInfo(info.USCode);
                    record = new StoreSTLLRecord();
                    record.StoreCode = info.USCode;
                    record.StoreID = storeBasic.StoreID;
                    record.Id = Guid.NewGuid();
                }
                record.LLparticipants = negotiations[0].LLParticipants;
                record.Location = negotiations[0].Location;
                record.McdParticipants = negotiations[0].McdParticipants;
                record.Content = negotiations[0].Content;
                record.CreatedTime = negotiations[0].CreateTime;
                record.Topic = negotiations[0].Topic;
                record.Save();
                var storeNegos = negotiations.Select(n => new StoreSTNegotiation
                {
                    Id = Guid.NewGuid(),
                    StoreID = record.StoreID,
                    StoreCode = record.StoreCode,
                    IsBroker = record.IsBroker,
                    BrokerName = record.BrokerName,
                    DateTime = n.Date.Value.ToString("yyyy-MM-dd"),
                    Location = n.Location,
                    Topic = n.Topic,
                    McdParticipants = n.McdParticipants,
                    McdParticipantsAD = record.McdParticipantsAD,
                    LLparticipants = n.LLParticipants,
                    Content = n.Content,
                    CreatedTime = n.CreateTime
                }).ToArray();
                StoreSTNegotiation.Add(storeNegos);
            }
        }
    }
}
