/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   9/28/2014 4:25:58 PM
 * FileName     :   Renewal_LegalReview
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.K2.Services.Entity;
using Mcdonalds.AM.DataAccess.Constants;
using System.Transactions;
using AutoMapper;
using NTTMNC.BPM.Fx.K2.Services;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.DataTransferObjects.Renewal;

namespace Mcdonalds.AM.DataAccess
{
    public partial class RenewalLegalApproval : BaseWFEntity<RenewalLegalApproval>
    {
        public override string WorkflowProcessName { get { return @"MCDAMK2Project.Renewal\LegalReview"; } }

        public override string WorkflowProcessCode
        {
            get { return "MCD_AM_Renewal_LR"; }
        }
        public override string TableName
        {
            get
            {
                return "RenewalLegalApproval";
            }
        }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public override string WorkflowCode
        {
            get
            {
                return FlowCode.Renewal_LegalApproval;
            }
        }

        public override string WorkflowActOriginator
        {
            get
            {
                return "Originator";
            }
        }
        public ApproveUsers AppUsers { get; set; }
        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_LegalUser",AppUsers.Legal.Code),
                new ProcessDataField("ProcessCode", WorkflowProcessCode)
            };

            if (task != null)
            {
                processDataFields.Add(new ProcessDataField("dest_Creator", ClientCookie.UserCode));
                processDataFields.Add(new ProcessDataField("ProjectTaskInfo", JsonConvert.SerializeObject(task)));
            }
            return processDataFields;
        }

        public List<ProcessDataField> SetWorkflowDataFieldsByLegal()
        {
            return new List<ProcessDataField>()
            {
                new ProcessDataField("dest_GeneralCounsel",AppUsers.GeneralCounsel.Code)
            };
        }
        public static void Create(string projectId, string userAccount)
        {
            RenewalLegalApproval legalApproval = new RenewalLegalApproval();
            legalApproval.Id = Guid.NewGuid();
            legalApproval.ProjectId = projectId;
            legalApproval.CreateTime = DateTime.Now;
            legalApproval.CreateUserAccount = userAccount;
            legalApproval.Add();
        }


        public static RenewalLegalApproval Get(string projectId, string id = "")
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

        public static RenewalLegalApprovalDTO InitPage(string projectId, string id = null)
        {
            RenewalLegalApprovalDTO dto = new RenewalLegalApprovalDTO();
            var entity = Get(projectId, id);
            var info = RenewalInfo.Get(projectId);
            var isActor = ClientCookie.UserCode == info.AssetActorAccount;
            entity.IsProjectFreezed = entity.CheckIfFreezeProject(projectId);
            var nextRefTableId = new Guid(FlowInfo.GetRefTableId(entity.TableName, projectId));
            var nextFlowStarted = ProjectInfo.IsFlowStarted(projectId, FlowCode.Renewal_Package);
            var haveTask = TaskWork.Any(t => t.RefID == projectId && t.TypeCode == FlowCode.Renewal_LegalApproval && t.Status == TaskWorkStatus.UnFinish && t.ReceiverAccount == ClientCookie.UserCode);
            var projectComment = ProjectComment.GetSavedComment(entity.Id, entity.TableName, ClientCookie.UserCode);
            var appUser = ApproveDialogUser.GetApproveDialogUser(entity.Id.ToString());
            dto.Info = info;
            dto.Entity = entity;
            dto.ProjectComment = projectComment != null ? projectComment.Content : "";
            dto.IsGeneralCounsel = appUser != null && appUser.GeneralCounselCode == ClientCookie.UserCode;
            dto.Editable = ProjectInfo.IsFlowEditable(projectId, FlowCode.Renewal_LegalApproval);
            dto.Recallable = ProjectInfo.IsFlowRecallable(projectId, FlowCode.Renewal_LegalApproval);
            dto.Savable = ProjectInfo.IsFlowSavable(projectId, FlowCode.Renewal_LegalApproval) && string.IsNullOrEmpty(id);
            return dto;
        }
        public void Save(string comment)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                this.Update();
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
                tranScope.Complete();
            }
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
                    this.ProcInstId = procInstId;
                    this.CreateTime = DateTime.Now;
                    this.CreateUserAccount = ClientCookie.UserCode;
                    this.Update();
                    var approveDialogUser = new ApproveDialogUser();
                    approveDialogUser.Id = Guid.NewGuid();
                    approveDialogUser.LegalCode = AppUsers.Legal.Code;
                    approveDialogUser.ProjectId = this.ProjectId;
                    approveDialogUser.FlowCode = this.WorkflowCode;
                    approveDialogUser.RefTableID = this.Id.ToString();
                    approveDialogUser.Add();
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
                    ProjectInfo.FinishNode(this.ProjectId, this.WorkflowCode, NodeCode.Renewal_LegalApproval_Input);
                    if (onExecuting != null)
                    {
                        onExecuting();
                    }
                    tranScope.Complete();
                }
            }
        }

        public void Approve(string comment, string SerialNumber,bool isGeneralCouncel)
        {
            if (isGeneralCouncel)
            {
                K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Approve", comment);
            }
            else
            {
                K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Approve", comment, SetWorkflowDataFieldsByLegal());
            }
            using (TransactionScope tranScope = new TransactionScope())
            {
                if (!isGeneralCouncel)
                {
                    var approveDialogUser = ApproveDialogUser.GetApproveDialogUser(this.Id.ToString());
                    approveDialogUser.GeneralCounselCode = AppUsers.GeneralCounsel.Code;
                    approveDialogUser.Update();
                }
                this.LastUpdateTime = DateTime.Now;
                this.LastUpdateUserAccount = ClientCookie.UserCode;
                this.Update();
                ProjectComment.AddComment(
                    ProjectCommentAction.Approve,
                    comment,
                    this.Id,
                    this.TableName,
                    FlowCode.Renewal,
                    this.ProcInstId,
                    ProjectCommentStatus.Submit
                );
                if (!isGeneralCouncel)
                {
                    ProjectInfo.FinishNode(this.ProjectId, this.WorkflowCode, NodeCode.Renewal_LegalApproval_LegalReview);
                }
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

        public void Resubmit(string comment, string SerialNumber)
        {
            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Resubmit", comment);
            using (TransactionScope tranScope = new TransactionScope())
            {
                this.CreateUserAccount = ClientCookie.UserCode;
                this.Update();
                var approveDialogUser = ApproveDialogUser.GetApproveDialogUser(this.Id.ToString());
                approveDialogUser.LegalCode = AppUsers.Legal.Code;
                //approveDialogUser.GeneralCounselCode = AppUsers.GeneralCounsel.Code;
                approveDialogUser.LastUpdateDate = DateTime.Now;
                approveDialogUser.LastUpdateUserAccount = ClientCookie.UserCode;
                approveDialogUser.Update();
                ProjectInfo.FinishNode(this.ProjectId, this.WorkflowCode, NodeCode.Renewal_LegalApproval_Input);
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
            var info = RenewalInfo.Get(this.ProjectId);
            using (var tranScope = new TransactionScope())
            {
                var legalApproval = Duplicator.AutoCopy(this);
                legalApproval.Id = Guid.NewGuid();
                legalApproval.IsHistory = false;
                legalApproval.LegalComments = "";
                legalApproval.CreateUserAccount = ClientCookie.UserCode;
                legalApproval.Add();
                IsHistory = true;
                this.Update();
                ProjectInfo.Reset(ProjectId, this.WorkflowCode);
                var attachments = Attachment.GetList(this.TableName, Id.ToString(), string.Empty);
                attachments.ForEach(att =>
                {
                    att.RefTableID = legalApproval.Id.ToString();
                    att.ID = Guid.NewGuid();
                });
                Attachment.Add(attachments.ToArray());
                var assetActor = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == ProjectId && pu.RoleCode == ProjectUserRoleCode.AssetActor);
                TaskWork.Cancel(t => t.RefID == ProjectId && t.Status == TaskWorkStatus.UnFinish);
                var task = info.GenerateSubmitTask(this.WorkflowCode);
                tranScope.Complete();
                return task.Url;
            }
        }

        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            var info = RenewalInfo.Get(this.ProjectId);
            Log4netHelper.WriteErrorLog(status.ToString());
            switch (status)
            {
                case TaskWorkStatus.K2ProcessApproved:
                {
                        Log4netHelper.WriteErrorLog("111111111");
                        ProjectProgress.SetProgress(ProjectId, "80%");
                        info.GenerateSubmitTask(FlowCode.Renewal_Package);
                        ProjectInfo.FinishNode(this.ProjectId, this.WorkflowCode, NodeCode.Renewal_LegalApproval_Approval,ProjectStatus.Finished);
                        Log4netHelper.WriteErrorLog("22222222");
                    }
                    break;
            }
        }

        public Dictionary<string, string> GetPrintTemplateFields()
        {
            var project = ProjectInfo.Get(ProjectId, FlowCode.Renewal_LegalApproval);
            var storeBasic = StoreBasicInfo.GetStorInfo(project.USCode);
            var storeContract = StoreContractInfo.Search(c => c.StoreCode == project.USCode).OrderByDescending(c => c.CreatedTime).FirstOrDefault();
            var info = RenewalInfo.Get(ProjectId);
            var flowInfo = FlowInfo.Get(FlowCode.Renewal);
            var legal = RenewalLegalApproval.Get(project.ProjectId) ?? new RenewalLegalApproval();
            Dictionary<string, string> templateFileds = new Dictionary<string, string>();
            templateFileds.Add("WorkflowName", flowInfo.NameENUS);
            templateFileds.Add("ProjectID", ProjectId);
            templateFileds.Add("USCode", storeBasic.StoreCode);
            templateFileds.Add("Region", storeBasic.Region);
            templateFileds.Add("StoreNameEN", storeBasic.NameENUS);
            templateFileds.Add("Market", storeBasic.Market);
            templateFileds.Add("City", storeBasic.CityZHCN);
            templateFileds.Add("StoreNameCN", storeBasic.NameZHCN);
            templateFileds.Add("StoreAge", Math.Floor((DateTime.Now - storeBasic.OpenDate).TotalDays / 365D).ToString());
            templateFileds.Add("OpenDate", storeBasic.OpenDate.ToString("yyyy-MM-dd"));
            templateFileds.Add("CloseDate", storeBasic.CloseDate.HasValue ? (storeBasic.CloseDate.Value.Year != 1900 ? storeBasic.CloseDate.Value.ToString("yyyy-MM-dd") : "") : "");

            if (storeContract != null)
                templateFileds.Add("CurrentLeaseENDYear", storeContract.EndYear);
            else
                templateFileds.Add("CurrentLeaseENDYear", "");

            templateFileds.Add("AssetsManager", info.AssetManagerNameENUS);
            templateFileds.Add("AssetsActor", info.AssetActorNameENUS);
            templateFileds.Add("AssetsRep", info.AssetRepNameENUS);
            templateFileds.Add("NewLeaseENDYear", info.NewLeaseEndDate.HasValue ? info.NewLeaseEndDate.Value.ToString("yyyy") : "");
            templateFileds.Add("ClosureDate", storeBasic.CloseDate.HasValue ? (storeBasic.CloseDate.Value.Year != 1900 ? storeBasic.CloseDate.Value.ToString("yyyy-MM-dd") : "") : "");

            var contractData = Employee.GetEmployeeContact(ProjectId);
            StringBuilder sbd = new StringBuilder();
            if (contractData != null && contractData.Count > 0)
            {
                foreach (var item in contractData)
                {
                    sbd.Append("<tr><td>&nbsp;</td>");
                    sbd.Append("<td>");
                    sbd.Append(item.NameENUS);
                    sbd.Append("</td>");
                    sbd.Append("<td>");
                    sbd.Append(item.PositionENUS);
                    sbd.Append("</td>");
                    sbd.Append("<td>");
                    sbd.Append(item.Mail);
                    sbd.Append("</td>");
                    sbd.Append("<td>");
                    sbd.Append(item.Phone);
                    sbd.Append("</td>");
                    sbd.Append("<td>");
                    sbd.Append(item.Mobile);
                    sbd.Append("</td>");
                    sbd.Append("</tr>");
                }
            }
            else
            {
                sbd.Append("<tr><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td><td>&nbsp;</td></tr>");
            }
            templateFileds.Add("ContactInfoData", sbd.ToString());

            //Special Application 
            templateFileds.Add("isurgencyyes", legal.IsUrgency ? "checked" : "");
            templateFileds.Add("isurgencyno", !legal.IsUrgency ? "checked" : "");
            templateFileds.Add("urgencyreason", legal.UrgencyReason);

            //transaction involves 
            templateFileds.Add("isrecenttransfer", legal.IsRecentTransfer ? "checked" : "");
            templateFileds.Add("isintermediaries", legal.IsIntermediaries ? "checked" : "");
            templateFileds.Add("isrelatedparties", legal.IsRelatedParties ? "checked" : "");
            templateFileds.Add("isbroker", legal.IsBroker ? "checked" : "");
            templateFileds.Add("ispttp", legal.IsPTTP ? "checked" : "");
            templateFileds.Add("isasiiwgo", legal.IsASIIWGO ? "checked" : "");
            templateFileds.Add("isnoblclause", legal.IsNoBLClause ? "checked" : "");
            templateFileds.Add("isofac", legal.IsOFAC ? "checked" : "");
            templateFileds.Add("isantic", legal.IsAntiC ? "checked" : "");
            templateFileds.Add("isbenefitconflict", legal.IsBenefitConflict ? "checked" : "");
            templateFileds.Add("noneofabove", legal.NoneOfAbove ? "checked" : "");

            //Any Legal Concerns
            templateFileds.Add("anylegalconcernno", !legal.AnyLegalConcern ? "checked" : "");
            templateFileds.Add("anylegalconcernyes", legal.AnyLegalConcern ? "checked" : "");
            templateFileds.Add("illegalstructure", legal.IllegalStructure ? "checked" : "");
            templateFileds.Add("occupying", legal.Occupying ? "checked" : "");
            templateFileds.Add("noauthoritytorelease", legal.NoAuthorityToRelease ? "checked" : "");
            templateFileds.Add("entrustlease", legal.EntrustLease ? "checked" : "");
            templateFileds.Add("sublease", legal.SubLease ? "checked" : "");
            templateFileds.Add("beingsealedup", legal.BeingSealedUp ? "checked" : "");
            templateFileds.Add("beingsealedupdesc", legal.BeingSealedUpDesc);
            templateFileds.Add("licensecantbeobtained", legal.LicenseCantBeObtained ? "checked" : "");
            templateFileds.Add("pendingordispute", legal.PendingOrDispute ? "checked" : "");
            templateFileds.Add("pendingordisputedesc", legal.PendingOrDisputeDesc);
            templateFileds.Add("otherissure", legal.OtherIssure ? "checked" : "");
            templateFileds.Add("OtherIssureDesc", legal.OtherIssureDesc);

            //SOX Audit 
            templateFileds.Add("optionsforrenewalyes", legal.OptionsForRenewal ? "checked" : "");
            templateFileds.Add("optionsforrenewalno", !legal.OptionsForRenewal ? "checked" : "");
            templateFileds.Add("reinstatementrequirementyes", legal.ReinstatementRequirement ? "checked" : "");
            templateFileds.Add("reinstatementrequirementno", !legal.ReinstatementRequirement ? "checked" : "");

            //Legal Department Review 
            templateFileds.Add("endorsed", legal.ReviewStatus == "Endorsed" ? "checked" : "");
            templateFileds.Add("legalcomments", (legal.ReviewStatus == "LegalComments") ? "checked" : "");
            templateFileds.Add("notendorsed", legal.ReviewStatus == "" ? "NotEndorsed" : "");
            templateFileds.Add("submitbeforesign", legal.SubmitBeforeSign ? "checked" : "");
            templateFileds.Add("SubmitBeforeSignDesc", legal.SubmitBeforeSignDesc);
            templateFileds.Add("landlordformleaseuserd", legal.LandlordFormLeaseUserd ? "checked" : "");
            templateFileds.Add("ownerrefusetohonorlease", legal.OwnerRefuseToHonorLease ? "checked" : "");
            templateFileds.Add("mortgageerefusetoguarantee", legal.MortgageeRefuseToGuarantee ? "checked" : "");
            templateFileds.Add("otherlegalcomment", legal.OtherLegalComment ? "checked" : "");
            templateFileds.Add("LegalComments", legal.LegalComments);
            templateFileds.Add("OtherLegalCommentDesc", legal.OtherLegalCommentDesc);
            templateFileds.Add("NotEndorsedIssureNo", legal.NotEndorsedIssureNo.HasValue?legal.NotEndorsedIssureNo.Value.ToString():"");

            //Endorsement by General Counsel 
            templateFileds.Add("GCComment", legal.GCComment);
            return templateFileds;
        }
    }
}
