using AutoMapper;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Common.Excel;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.DataTransferObjects.Renewal;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.DataAccess.Workflow;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   9/28/2014 4:24:54 PM
 * FileName     :   RenewalRenewTool
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
    public partial class RenewalTool : BaseWFEntity<RenewalTool>, IWorkflowEntity
    {
        public override string WorkflowProcessName { get { return @"MCDAMK2Project.Renewal\RenewTool"; } }

        public override string WorkflowProcessCode
        {
            get { return "MCD_AM_Renewal_RT"; }
        }

        public override string TableName
        {
            get
            {
                return "RenewalTool";
            }
        }
        public override string WorkflowCode
        {
            get
            {
                return FlowCode.Renewal_Tool;
            }
        }

        public override string WorkflowActOriginator
        {
            get
            {
                return "Originator";
            }
        }

        public Guid NewEntityId { get; set; }
        public override List<ProjectComment> GetEntityProjectComment()
        {
            var souceCode = WorkflowCode.Split('_')[0];
            return ProjectComment.Search(e => e.RefTableName == TableName
               && e.SourceCode == souceCode && e.RefTableId == Id && e.Status == ProjectCommentStatus.Submit)
               .OrderBy(e => e.CreateTime).ToList();
        }
        public ApproveUsers AppUsers { get; set; }
        public static RenewalTool Get(string projectId, string id = null)
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

        public static RenewalTool Create(string projectId, string createUserAccount)
        {
            RenewalTool tool = new RenewalTool();
            tool.Id = Guid.NewGuid();
            tool.ProjectId = projectId;
            tool.CreateTime = DateTime.Now;
            tool.CreateUserAccount = createUserAccount;
            tool.Add();
            return tool;
        }

        public static RenewalToolDTO InitPage(string projectId, string id = null)
        {
            RenewalToolDTO dto = new RenewalToolDTO();
            var tool = RenewalTool.Get(projectId, id);
            var project = ProjectInfo.Get(projectId, FlowCode.Renewal_Tool);
            var info = RenewalInfo.Get(projectId);
            var isFinance = ClientCookie.UserCode == info.FinanceAccount;
            tool.IsProjectFreezed = tool.CheckIfFreezeProject(projectId);
            var nextRefTableId = new Guid(FlowInfo.GetRefTableId("RenewalAnalysis", projectId));
            var nextFlowStarted = ProjectInfo.IsFlowStarted(projectId, FlowCode.Renewal_Analysis);
            var haveTask = TaskWork.Any(t => t.RefID == projectId && t.TypeCode == FlowCode.Renewal_Tool && t.Status == TaskWorkStatus.UnFinish && t.ReceiverAccount == ClientCookie.UserCode);
            var projectComment = ProjectComment.GetSavedComment(tool.Id, "RenewalTool", ClientCookie.UserCode);
            var projectNode = NodeInfo.GetNodeInfo(project.NodeCode);
            var packageStarted = ProjectInfo.IsFlowStarted(projectId, FlowCode.Renewal_Package);
            string selectedYearMonth = null;
            dto.Info = info;
            dto.Entity = tool;
            dto.TTMDataYearMonths = RenewalToolFinMeasureInput.GetYearMonths(projectId, out selectedYearMonth);
            dto.FinMeasureInput = RenewalToolFinMeasureInput.Get(projectId, dto.Entity.Id);
            if (string.IsNullOrEmpty(dto.FinMeasureInput.FinanceYear) || string.IsNullOrEmpty(dto.FinMeasureInput.FinanceMonth))
            {
                var ym = selectedYearMonth.Split('-');
                dto.FinMeasureInput.FinanceYear = ym[0];
                dto.FinMeasureInput.FinanceMonth = ym[1];
            }
            dto.FinMeasureInput.FinanceDataYearMonth = dto.FinMeasureInput.FinanceYear + "-" + dto.FinMeasureInput.FinanceMonth;
            //dto.FinMeasureInput.ContributionMargin = StoreCM.Get(dto.Info.USCode).ContributionMargin;
            McdAMEntities amdb = new McdAMEntities();
            var finfo = amdb.DataSync_LDW_AM_STFinanceData2.FirstOrDefault(f => f.UsCode == dto.Info.USCode &&
                f.FinanceYear == dto.FinMeasureInput.FinanceYear && f.FinanceMonth == dto.FinMeasureInput.FinanceMonth);
            decimal cm = 0;
            if (finfo != null && !string.IsNullOrEmpty(finfo.contribution_marginPct))
                cm = decimal.Parse(finfo.contribution_marginPct);
            dto.FinMeasureInput.ContributionMargin = cm;
            var coninfo = RenewalConsInfo.FirstOrDefault(e => e.ProjectId == projectId && !e.IsHistory);
            var conProj = ProjectInfo.FirstOrDefault(e => e.ProjectId == projectId && e.FlowCode == "Renewal_ConsInfo");
            dto.WriteOffAndReinCost = RenewalToolWriteOffAndReinCost.Get(projectId, dto.Entity.Id, projectNode);
            if (coninfo != null && !coninfo.HasReinvenstment)
            {
                dto.WriteOffAndReinCost.REWriteOff = null;
                dto.WriteOffAndReinCost.LHIWriteOff = null;
                dto.WriteOffAndReinCost.ESSDWriteOff = null;
                dto.WriteOffAndReinCost.ESSDWriteOff = null;
                dto.WriteOffAndReinCost.RECost = null;
                dto.WriteOffAndReinCost.LHICost = null;
                dto.WriteOffAndReinCost.ESSDCost = null;
            }

            dto.Uploadable = projectNode.Sequence >= 3 && ClientCookie.UserCode == dto.Info.AssetActorAccount && !packageStarted;
            dto.ProjectComment = projectComment != null ? projectComment.Content : "";
            dto.Editable = ProjectInfo.IsFlowEditable(projectId, FlowCode.Renewal_Tool);
            dto.Recallable = ProjectInfo.IsFlowRecallable(projectId, FlowCode.Renewal_Tool);
            dto.Savable = ProjectInfo.IsFlowSavable(projectId, FlowCode.Renewal_Tool) && string.IsNullOrEmpty(id);
            dto.IsFinished =
                ProjectInfo.Any(
                    e =>
                        e.ProjectId == projectId && e.FlowCode == FlowCode.Renewal_Tool &&
                        e.Status == ProjectStatus.Finished);
            return dto;
        }

        public void Save(string comment, Action onExecuting = null)
        {
            using (TransactionScope tranScope = new TransactionScope())
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
                this.Update();
                if (onExecuting != null)
                {
                    onExecuting();
                }
                tranScope.Complete();
            }
        }
        public override List<ProcessDataField> SetWorkflowDataFields(TaskWork task)
        {
            var info = RenewalInfo.Get(this.ProjectId);
            var processDataFields = new List<ProcessDataField>()
            {
                new ProcessDataField("dest_Finance",AppUsers.FM.Code),
                new ProcessDataField("dest_AssetActor",info.AssetActorAccount),
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
            try
            {
                RenewalInfo info = RenewalInfo.Get(this.ProjectId);
                var task = TaskWork.GetTaskWork(ProjectId, ClientCookie.UserCode, TaskWorkStatus.UnFinish,
                FlowCode.Renewal, WorkflowCode);

                var dataFields = SetWorkflowDataFields(task);
                var procInstId = K2FxContext.Current.StartProcess(WorkflowProcessCode, ClientCookie.UserCode, dataFields);
                if (procInstId > 0)
                {
                    using (var tranScope = new TransactionScope())
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
                        ProjectInfo.FinishNode(this.ProjectId, this.WorkflowCode, NodeCode.Renewal_Tool_Input);
                        if (onExecuting != null)
                        {
                            onExecuting();
                        }
                        tranScope.Complete();
                    }
                }
            }
            catch (Exception ex)
            {
                Log4netHelper.WriteInfo("[[Renewal Too]]:" + JsonConvert.SerializeObject(ex));
                throw ex;
            }

        }

        public void Approve(string comment, string SerialNumber)
        {
            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Approve", comment);
            using (var tranScope = new TransactionScope())
            {
                ProjectComment.AddComment(
                    ProjectCommentAction.Approve,
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

        public void Reject(string comment, string SerialNumber)
        {
            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Reject", comment);
            using (TransactionScope tranScope = new TransactionScope())
            {
                ProjectInfo.Reject(this.ProjectId, this.WorkflowCode);
                ProjectComment.AddComment(
                    ProjectCommentAction.Decline,
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
            var task = TaskWork.FirstOrDefault(t => t.RefID == this.ProjectId && t.TypeCode == this.WorkflowCode && t.ReceiverAccount == ClientCookie.UserCode);
            var dataFields = SetWorkflowDataFields(task);
            K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Resubmit", comment, dataFields);
            using (TransactionScope tranScope = new TransactionScope())
            {
                task.Finish();
                this.CreateUserAccount = ClientCookie.UserCode;
                this.Update();
                ProjectInfo.FinishNode(this.ProjectId, this.WorkflowCode, NodeCode.Renewal_Tool_Input);
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
            string url;
            using (var tranScope = new TransactionScope())
            {
                var info = RenewalInfo.Get(this.ProjectId);
                var tool = Duplicator.AutoCopy(this);
                tool.Id = Guid.NewGuid();
                tool.IsHistory = false;
                tool.CreateTime = DateTime.Now;
                tool.CreateUserAccount = ClientCookie.UserCode;
                tool.Add();

                NewEntityId = tool.Id;
                IsHistory = true;
                this.Update();
                ProjectInfo.Reset(ProjectId, this.WorkflowCode);
                var attachments = Attachment.GetList(this.TableName, Id.ToString(), string.Empty);
                attachments.ForEach(att =>
                {
                    att.RefTableID = tool.Id.ToString();
                    att.ID = Guid.NewGuid();
                });
                Attachment.Add(attachments.ToArray());

                var FinMI = RenewalToolFinMeasureInput.Get(this.ProjectId, this.Id);
                var newFinMI = Duplicator.AutoCopy(FinMI);
                newFinMI.Id = Guid.NewGuid();
                newFinMI.ToolId = tool.Id;
                newFinMI.Add();

                var WfRc = RenewalToolWriteOffAndReinCost.Get(this.ProjectId, this.Id);
                var newWfRc = Duplicator.AutoCopy(WfRc);
                newWfRc.Id = Guid.NewGuid();
                newWfRc.ToolId = tool.Id;
                newWfRc.Add();

                var FinMO = RenewalToolFinMeasureOutput.GetByToolId(this.Id);
                var newFinMO = Duplicator.AutoCopy(FinMO);
                newFinMO.Id = Guid.NewGuid();
                newFinMO.ToolId = tool.Id;
                newFinMO.Add();

                var package = RenewalPackage.Get(this.ProjectId);
                package.ToolId = tool.Id;
                package.Update();

                var oldTasks = TaskWork.Search(t => t.RefID == ProjectId && t.Status == TaskWorkStatus.UnFinish && new string[] { this.WorkflowCode, FlowCode.Renewal_Analysis }.Contains(t.TypeCode)).ToList();
                oldTasks.ForEach(t =>
                {
                    t.Status = TaskWorkStatus.Cancel;
                });
                TaskWork.Update(oldTasks.ToArray());

                var anlysisProj = ProjectInfo.FirstOrDefault(e => e.FlowCode == FlowCode.Renewal_Analysis
                    && e.Status == ProjectStatus.UnFinish && e.ProjectId == ProjectId);
                if (anlysisProj != null)
                {
                    var taskAnlysis = TaskWork.FirstOrDefault(e => e.TypeCode == FlowCode.Renewal_Analysis && e.RefID == ProjectId && e.Status == TaskWorkStatus.UnFinish);
                    if (taskAnlysis != null)
                    {
                        taskAnlysis.Status = TaskWorkStatus.Finished;
                        taskAnlysis.FinishTime = DateTime.Now;
                        taskAnlysis.Update();
                    }
                }
                var task = info.GenerateSubmitTask(this.WorkflowCode);
                url = task.Url;
                tranScope.Complete();
            }

            return url;
        }

        public string DownloadToolTemplate()
        {
            var templateFileName = string.Concat(SiteFilePath.Template_DIRECTORY, "/", SiteFilePath.RenewalTool_Template);
            string fileName = string.Format("{0}/{1}.xlsx", SiteFilePath.TEMP_DIRECTORY, Guid.NewGuid());
            File.Copy(templateFileName, fileName);
            FileInfo fileInfo = new FileInfo(fileName);
            ExcelDataInputDirector excelDirector = new ExcelDataInputDirector(fileInfo, ExcelDataInputType.RenewalTool);
            ExcelInputDTO excelDto = new ExcelInputDTO();
            var info = RenewalInfo.Get(this.ProjectId);
            var storeBasic = StoreBasicInfo.GetStorInfo(info.USCode);
            var storeContract = StoreContractInfo.Get(info.USCode);
            var finInput = RenewalToolFinMeasureInput.Get(this.ProjectId, this.Id);
            var wfAndReinCost = RenewalToolWriteOffAndReinCost.Get(this.ProjectId, this.Id);
            excelDto.USCode = info.USCode;
            excelDto.Market = storeBasic.Market;
            excelDto.StoreName = storeBasic.NameZHCN + "/" + storeBasic.NameENUS;
            excelDto.OpenDate = storeBasic.OpenDate;
            if (storeContract.EndDate.HasValue)
            {
                excelDto.LeaseExpirationDate = storeContract.EndDate.Value;
            }
            excelDto.RenewalYears = info.RenewalYears;
            excelDto.ProductSales = finInput.ProductSalesAdjustment;
            excelDto.Pac = finInput.PacAdjustment;
            excelDto.Rent = finInput.RentAdjustment;
            excelDto.DepreciationLhi = finInput.DepreciationLhiAdjustment;
            excelDto.InterestLhi = finInput.InterestLhiAdjustment;
            excelDto.ServiceFee = finInput.ServiceFeeAdjustment;
            excelDto.Accounting = finInput.AccountingAdjustment;
            excelDto.Insurance = finInput.InsuranceAdjustment;
            excelDto.TaxesAndLicenses = finInput.TaxesAndLicensesAdjustment;
            excelDto.DepreciationEssd = finInput.DepreciationEssdAdjustment;
            excelDto.InterestEssd = finInput.InterestEssdAdjustment;
            excelDto.OtherIncExp = finInput.OtherIncExpAdjustment;
            excelDto.NonProductSales = finInput.NonProductSalesAdjustment;
            excelDto.NonProductCosts = finInput.NonProductCostsAdjustment;
            excelDto.REII = wfAndReinCost.REII;
            excelDto.LHIII = wfAndReinCost.LHIII;
            excelDto.ESSDII = wfAndReinCost.ESSDII;
            excelDto.RENBV = wfAndReinCost.RENBV;
            if (wfAndReinCost.LHINBV.HasValue)
            {
                excelDto.LHINBV = wfAndReinCost.LHINBV.Value;
            }
            if (wfAndReinCost.ESSDNBV.HasValue)
            {
                excelDto.ESSDNBV = wfAndReinCost.ESSDNBV.Value;
            }
            excelDto.RECost = wfAndReinCost.RECost;
            excelDto.LHICost = wfAndReinCost.LHICost;
            excelDto.ESSDCost = wfAndReinCost.ESSDCost;
            excelDto.TotalWriteOff = wfAndReinCost.REWriteOff + wfAndReinCost.LHIWriteOff + wfAndReinCost.ESSDWriteOff;
            excelDto.RentalStructure = !string.IsNullOrEmpty(finInput.RentalStructure) ? (Nullable<decimal>)decimal.Parse(finInput.RentalStructure) : null;
            excelDto.ContributionMargin = finInput.ContributionMargin;
            excelDto.SalesCompYr1 = finInput.SalesCompYr1;
            excelDto.SalesCompYr2 = finInput.SalesCompYr2;
            excelDto.SalesCompYr3 = finInput.SalesCompYr3;
            excelDto.SalesCompYr4 = finInput.SalesCompYr4;
            excelDto.SalesCompYr5 = finInput.SalesCompYr5;
            excelDto.SalesCompYr6 = finInput.SalesCompYr6;
            excelDto.SalesCompYr7 = finInput.SalesCompYr7;
            excelDto.SalesCompYr8 = finInput.SalesCompYr8;
            excelDto.SalesCompYr9 = finInput.SalesCompYr9;
            excelDto.SalesCompYr10 = finInput.SalesCompYr10;
            excelDto.SalesCompYr11 = finInput.SalesCompYr11;
            excelDto.SalesCompYr12 = finInput.SalesCompYr12;
            excelDto.SalesCompYr13 = finInput.SalesCompYr13;
            excelDto.SalesCompYr14 = finInput.SalesCompYr14;
            excelDto.SalesCompYr15 = finInput.SalesCompYr15;
            excelDto.SalesCompYr16 = finInput.SalesCompYr16;
            excelDto.SalesCompYr17 = finInput.SalesCompYr17;
            excelDto.SalesCompYr18 = finInput.SalesCompYr18;
            excelDto.SalesCompYr19 = finInput.SalesCompYr19;
            excelDto.SalesCompYr20 = finInput.SalesCompYr20;
            excelDto.ComSalesDesc = this.ComSalesDesc;
            excelDto.CompSales = finInput.CompSalesAdjustment;
            excelDto.FinanceYear = finInput.FinanceYear;
            excelDto.FinanceMonth = finInput.FinanceMonth;
            excelDirector.Input(excelDto);
            return fileName;
        }

        public override void Finish(TaskWorkStatus status, TaskWork task)
        {
            var info = RenewalInfo.Get(this.ProjectId);
            switch (status)
            {
                case TaskWorkStatus.K2ProcessApproved:
                    {
                        using (TransactionScope tranScope = new TransactionScope())
                        {
                            ProjectProgress.SetProgress(ProjectId, "40%");
                            ProjectInfo.FinishNode(ProjectId, this.WorkflowCode, NodeCode.Finish);
                            var anlysisProj = ProjectInfo.FirstOrDefault(e => e.FlowCode == FlowCode.Renewal_Analysis
                                && e.Status == ProjectStatus.UnFinish && e.ProjectId == ProjectId);
                            if (anlysisProj != null)
                            {
                                info.GenerateSubmitTask(FlowCode.Renewal_Analysis);
                            }
                            else
                            {
                                //Renewal Tool重新Edit并完成后，系统自动重新生成一次Renewal Analysis文件
                                var anlysis = RenewalAnalysis.Get(ProjectId);
                                if (anlysis != null)
                                {
                                    anlysis.GenerateAttachment();
                                }
                            }
                            tranScope.Complete();
                        }
                    }
                    break;
            }
        }

        public void UploadTool()
        {
            var context = HttpContext.Current;
            var files = context.Request.Files;
            if (files.Count > 0)
            {
                using (TransactionScope tranScope = new TransactionScope())
                {
                    var file = files[0];
                    var templateFileName = string.Concat(SiteFilePath.Template_DIRECTORY, "/", SiteFilePath.RenewalTool_Template);
                    if (!ExcelHelper.MatchVersionNumber(templateFileName, file.InputStream))
                    {
                        throw new Exception("Version not matched!");
                    }
                    var extetion = Path.GetExtension(file.FileName);
                    var internalName = string.Concat(Guid.NewGuid(), extetion);
                    var fileName = context.Server.MapPath(string.Format("~/UploadFiles/{0}", internalName));
                    file.SaveAs(fileName);
                    FileInfo fileInfo = new FileInfo(fileName);
                    ExcelDataImportDirector excelDirector = new ExcelDataImportDirector(fileInfo, ExcelDataImportType.RenewalTool);
                    var finOutput = RenewalToolFinMeasureOutput.GetByToolId(this.Id);
                    if (finOutput == null)
                    {
                        finOutput = new RenewalToolFinMeasureOutput();
                        finOutput.ToolId = this.Id;
                        finOutput.Id = Guid.NewGuid();
                    }
                    excelDirector.ExcelData.Entity = finOutput;
                    excelDirector.ParseAndImport();
                    var att = Attachment.FirstOrDefault(e => e.RefTableID == this.Id.ToString() && e.TypeCode == "RenewalTool");
                    bool hasTool = true;
                    if (att == null)
                    {
                        hasTool = false;
                        att = new Attachment();
                        att.ID = Guid.NewGuid();
                    }

                    att.TypeCode = "RenewalTool";
                    att.RefTableID = this.Id.ToString();
                    att.RefTableName = this.TableName;
                    att.Name = "Renewal Tool";
                    att.Extension = extetion;
                    att.RelativePath = "/";
                    att.InternalName = internalName;
                    att.ContentType = "microsoft/excel";
                    att.Length = file.ContentLength;
                    att.CreatorID = ClientCookie.UserCode;
                    att.CreatorNameENUS = ClientCookie.UserNameENUS;
                    att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
                    att.CreateTime = DateTime.Now;
                    att.RequirementId = new Guid("CE111514-65E3-45C7-AD22-DF4937B1686E");
                    if (hasTool)
                    {
                        att.Update();
                    }
                    else
                    {
                        att.Add();
                    }
                    tranScope.Complete();
                }
            }
            else
            {
                throw new Exception("Please select Renewal Tool to upload");
            }
        }


        public void ConfirmUploadTool(string SerialNumber)
        {
            Save(null);
            var renewalToolAttachment =
                Attachment.FirstOrDefault(att => att.RefTableID == this.Id.ToString() && att.TypeCode == "RenewalTool");
            if (renewalToolAttachment == null)
            {
                throw new Exception("Please upload Renewal Tool first！");
            }

            var fileName = SiteFilePath.UploadFiles_DIRECTORY + "\\" + renewalToolAttachment.InternalName;
            var fileInfo = new FileInfo(fileName);
            var excelDirector = new ExcelDataInputDirector(fileInfo, ExcelDataInputType.RenewalTool);
            var excelDto = new ExcelInputDTO();
            excelDto.ComSalesDesc = ComSalesDesc;
            excelDirector.Edit(excelDto);

            var project = ProjectInfo.Get(this.ProjectId, this.WorkflowCode);
            if (project.Status != ProjectStatus.Finished)
            {
                string comment = "upload Renewal Tool";
                K2FxContext.Current.ApprovalProcess(SerialNumber, ClientCookie.UserCode, "Approve", comment);
                ProjectInfo.FinishNode(this.ProjectId, this.WorkflowCode, NodeCode.Finish, ProjectStatus.Finished);
            }
        }

        public override void PrepareTask(TaskWork taskWork)
        {
            var info = RenewalInfo.Get(ProjectId);
            if (taskWork.ReceiverAccount == info.AssetActorAccount && taskWork.ActivityName == "AssetActor")
            {
                ProjectInfo.FinishNode(ProjectId, this.WorkflowCode, NodeCode.Renewal_Tool_Approval);
                if (!ProjectInfo.IsFlowFinished(ProjectId, FlowCode.Renewal_ConsInfo))
                {
                    taskWork.Cancel();
                }
            }
        }
    }
}
