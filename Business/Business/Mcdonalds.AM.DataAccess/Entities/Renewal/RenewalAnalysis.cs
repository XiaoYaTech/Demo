using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Common.Excel;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects.Renewal;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.Services.Infrastructure;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.K2.Services;
using NTTMNC.BPM.Fx.K2.Services.Entity;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   9/28/2014 4:25:23 PM
 * FileName     :   Renewal_RenewAnalysis
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
    public partial class RenewalAnalysis : BaseWFEntity<RenewalAnalysis>
    {
        public override string WorkflowCode
        {
            get
            {
                return FlowCode.Renewal_Analysis;
            }
        }
        public override string TableName
        {
            get
            {
                return "RenewalAnalysis";
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
            get { return "MCD_AM_Renewal_RA"; }
        }

        public override string WorkflowProcessName
        {
            get { return @"MCDAMK2Project.Renewal\Analysis"; }
        }
        public static RenewalAnalysis Create(RenewalInfo info)
        {
            RenewalAnalysis analysis = new RenewalAnalysis();
            analysis.Id = Guid.NewGuid();
            analysis.ProjectId = info.ProjectId;
            analysis.CreateTime = DateTime.Now;
            analysis.CreateUserAccount = info.AssetActorAccount;
            analysis.LeaseTenureAndTerm = string.Format("{0} years,from {1:yyyy-MM-dd} to {2:yyyy-MM-dd}", info.RenewalYears, info.NewLeaseStartDate, info.NewLeaseEndDate);
            analysis.Add();
            return analysis;
        }

        public static RenewalAnalysis Get(string projectId, string id = "")
        {
            if (!string.IsNullOrEmpty(id))
            {
                return Get(new Guid(id));
            }
            else
            {
                return FirstOrDefault(e => e.ProjectId == projectId && e.IsHistory == false);
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
        public static RenewalAnalysisDTO InitPage(string projectId)
        {
            RenewalAnalysisDTO dto = new RenewalAnalysisDTO();
            var consInfo = RenewalConsInfo.Get(projectId);
            var analysis = Get(projectId);
            if (consInfo.HasReinvenstment)
            {
                var reinBasic = ReinvestmentBasicInfo.GetByConsInfoId(consInfo.Id) ?? new ReinvestmentBasicInfo();
                analysis.OperationSize = reinBasic.NewOperationSize;
            }
            dto.Info = RenewalInfo.Get(projectId);
            analysis.LastRemodeling = GetLastRemodeling(dto.Info.USCode);
            dto.Entity = analysis;
            dto.HasReinvenstment = consInfo.HasReinvenstment;
            dto.StoreInfo = PrepareStoreInfo(projectId, dto.Info.USCode);
            dto.Editable = ProjectInfo.IsFlowEditable(projectId, FlowCode.Renewal_Analysis);
            dto.Recallable = ProjectInfo.IsFlowRecallable(projectId, FlowCode.Renewal_Analysis);
            dto.Savable = ProjectInfo.IsFlowSavable(projectId, FlowCode.Renewal_Analysis);
            return dto;
        }

        private static  string GetLastRemodeling(string USCode)
        {
            var projectGBMemo =
                ProjectInfo.Search(e => e.USCode.Equals(USCode) && e.FlowCode == FlowCode.Reimage_GBMemo)
                    .OrderByDescending(e => e.CreateTime)
                    .FirstOrDefault();
            string returnValue = "";
            if (projectGBMemo != null && projectGBMemo.Status == ProjectStatus.Finished)
            {
                var projectRimg =
                    ProjectInfo.Search(
                        e => e.ProjectId.Equals(projectGBMemo.ProjectId) && e.FlowCode == FlowCode.Reimage)
                        .FirstOrDefault();
                if (projectRimg != null && projectRimg.Status == ProjectStatus.Completed)
                {
                    var gbmemo = ReimageGBMemo.GetGBMemo(projectGBMemo.ProjectId);
                    if (gbmemo != null && gbmemo.ConstCompletionDate.HasValue)
                    {
                        returnValue = gbmemo.ConstCompletionDate.Value.ToString("yyyy-MM-dd");
                        string strCost = GetReimageInvestmentCost(projectGBMemo.ProjectId);
                        if (!string.IsNullOrEmpty(strCost))
                        {
                            returnValue = returnValue + "," + strCost;
                        }
                    }
                }
            }
            return returnValue;
        }

        private static string GetReimageInvestmentCost(string rimgProjectId)
        {
            string strRturn = "";
            var consCk = ReimageConsInvtChecking.GetWorkflowEntity(rimgProjectId,FlowCode.Reimage_ConsInvtChecking);
            if (consCk != null)
            {
                var revensCost = ReinvestmentCost.GetByConsInfoId(consCk.EntityId);
                if (revensCost != null && !string.IsNullOrEmpty(revensCost.TotalReinvestmentFAAct))
                {
                    strRturn=revensCost.TotalReinvestmentFAAct;
                }
            }
            return strRturn;
        }

        private static RenewalAnalysisStoreInfo PrepareStoreInfo(string projectId, string usCode)
        {
            var store = StoreBasicInfo.GetStorInfo(usCode);
            var storeMMInfo = StoreMMInfo.Get(usCode);
            var storeContract = StoreContractInfo.Get(usCode);
            var ta = StoreSTLocation.GetStoreSTLocation(usCode);
            var tool = RenewalTool.Get(projectId);
            var finOutput = RenewalToolFinMeasureOutput.GetByToolId(tool.Id) ?? new RenewalToolFinMeasureOutput();
            var consInfo = RenewalConsInfo.Get(projectId);
            var toolWriteOff = RenewalToolWriteOffAndReinCost.Get(projectId, tool.Id);
            var floors = string.Join("/", new[]{
                string.Join(",", new[] { 
                    ta.Floor1, 
                    ta.Floor2, 
                    ta.Floor3, 
                    ta.Floor4, 
                    ta.Floor5 }.Where(e => !string.IsNullOrEmpty(e)).ToArray()),
                string.Join(",",new[]{
                    ta.FrontCounterFloor1,
                    ta.FrontCounterFloor2
                }.Where(e => !string.IsNullOrEmpty(e)).ToArray())
            }.Where(e => !string.IsNullOrEmpty(e)).ToArray());

            var seats = string.Join("/", new[]{
                new[]{ta.Seats1,ta.Seats2,ta.Seats3,ta.Seats4,ta.Seats5,ta.FrontCounterSeats}.Select(e=>{
                    var val = 0;
                    int.TryParse(e,out val);
                    return val;
                }).Sum(),
                string.IsNullOrEmpty(ta.OutsideSeats)?0:int.Parse(ta.OutsideSeats)
            });

            var beTypes = string.Join(",", StoreBEInfo.Search(e => e.StoreCode == usCode && e.BETypeName != "FC" && e.BETypeName != "DT").Select(e => e.BETypeName).ToArray());

            var leaseTenureAndTerm = string.Concat(storeContract.LeasePurchaseTerm, " years,from ",
                storeContract.StartDate.HasValue ? storeContract.StartDate.Value.ToString("yyyy-MM-dd") : " / ",
                "to",
                storeContract.EndDate.HasValue ? storeContract.EndDate.Value.ToString("yyyy-MM-dd") : " / "
            );
            
            return new RenewalAnalysisStoreInfo
            {
                UsCode = store.StoreCode,
                NameZHCN = store.NameZHCN,
                NameENUS = store.NameENUS,
                MinimarketPriority = storeMMInfo.Priority,
                TADesrability = storeMMInfo.Desirability,
                SitePortfolioType = storeMMInfo.PortfolioTypeName,
                SiteRerating = storeMMInfo.LocationRatingPP,
                LeasedArea = storeContract.TotalLeasedArea,
                OperationSize = ta.TotalArea,
                Floors = floors,
                Seats = seats,
                BEType = beTypes,
                LeaseTenureAndTerm = leaseTenureAndTerm,
                FreeRentalPeriod = storeContract.FreeRentalPeriod,
                RentStructure = storeContract.RentStructure,
                DRMFLastTY = finOutput.AnnualRentExpenseLY,
                DRMFLastTYSales = finOutput.RentAsProdSalesLY,
                SRMFLastTY = finOutput.AnnualRentExpenseYr1,
                SRMFLastTYSales = finOutput.RentAsProdSalesYr1,
                SRMF1stTY = finOutput.AnnualRentExpenseAvg,
                SRMF1stTYSales = finOutput.RentAsProdSalesAvg,
                AnnualSOILastTY = finOutput.SOIProdSalesLY,
                AnnualSOIAvg = finOutput.SOIProdSalesAvg,
                CashROIAvg = finOutput.CashROIAvg,
                CashROILastTY = finOutput.CashROILY,
                OriginalInvestment = (toolWriteOff.REII + toolWriteOff.LHIII + toolWriteOff.ESSDII).ToString(),
                NBV = (toolWriteOff.RENBV + toolWriteOff.LHINBV + toolWriteOff.ESSDNBV).ToString(),
                AdditionalInvestmentCost = (toolWriteOff.RECost + toolWriteOff.LHICost + toolWriteOff.ESSDCost).ToString(),
                ExclusivityClause = storeContract.ExclusivityClause,
                McDsEarlyTerminationRight = storeContract.WithEarlyTerminationClause == null || storeContract.WithEarlyTerminationClause == 0 ? "N" : "Y",
                LandlordEntity = storeContract.PartyAFullName,
                LastRemodeling = store.ReImageDate.HasValue ? store.ReImageDate.Value.ToString("yyyy-MM-dd") : ""
            };
        }

        public void Save()
        {
            this.CreateUserAccount = ClientCookie.UserCode;
            this.LastUpdateUserAccount = ClientCookie.UserCode;
            this.LastUpdateTime = DateTime.Now;
            this.Update();
            GenerateAttachment();
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
                    ProjectInfo.FinishNode(ProjectId, FlowCode.Renewal_Analysis, NodeCode.Finish, ProjectStatus.Finished);
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
                    ProjectProgress.SetProgress(ProjectId, "50%");
                    ProjectInfo.FinishNode(ProjectId, FlowCode.Renewal_Analysis, NodeCode.Finish, ProjectStatus.Finished);
                    if (info.RenewalYears > 2)
                    {
                        if (!ProjectInfo.FlowHaveTask(ProjectId, FlowCode.Renewal_ClearanceReport))
                        {
                            info.GenerateSubmitTask(FlowCode.Renewal_ClearanceReport);
                        }
                    }
                    else
                    {
                        ProjectInfo.FinishProject(this.ProjectId, FlowCode.Renewal_ClearanceReport);
                        if (!ProjectInfo.FlowHaveTask(ProjectId, FlowCode.Renewal_ConfirmLetter))
                        {
                            info.GenerateSubmitTask(FlowCode.Renewal_ConfirmLetter);
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
                tranScope.Complete();
            }
        }

        public override string Edit()
        {
            var info = RenewalInfo.Get(ProjectId);
            var analysis = Duplicator.AutoCopy(this);
            analysis.Id = Guid.NewGuid();
            analysis.IsHistory = false;
            analysis.Add();
            this.IsHistory = true;
            this.Update();
            ProjectInfo.Reset(ProjectId, this.WorkflowCode);
            RenewalPackage package = RenewalPackage.Get(ProjectId);
            package.AnalysisId = analysis.Id;
            package.Update();
            TaskWork.Cancel(e => e.TypeCode == FlowCode.Renewal_Analysis && e.RefID == ProjectId && e.Status == TaskWorkStatus.UnFinish);
            var task = info.GenerateSubmitTask(FlowCode.Renewal_Analysis);
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

        public void GenerateAttachment()
        {
            var info = RenewalInfo.Get(this.ProjectId);
            var storeInfo = PrepareStoreInfo(this.ProjectId, info.USCode);
            string templateFileName = string.Concat(SiteFilePath.Template_DIRECTORY, "/", SiteFilePath.RenewalAnalysis_Template);
            string fileName = HttpContext.Current.Server.MapPath(string.Format("~/UploadFiles/{0}.xlsx", Guid.NewGuid()));
            var extetion = Path.GetExtension(fileName);
            var internalName = Path.GetFileName(fileName);
            File.Copy(templateFileName, fileName);
            FileInfo fileInfo = new FileInfo(fileName);
            ExcelDataInputDirector excelDirector = new ExcelDataInputDirector(fileInfo, ExcelDataInputType.RenewalAnalysis);
            ExcelInputDTO excelDto = new ExcelInputDTO();
            excelDto.USCode = storeInfo.UsCode;
            excelDto.StoreNameEN = storeInfo.NameENUS;
            excelDto.StoreNameCN = storeInfo.NameZHCN;
            excelDto.MinimarketPriority = string.IsNullOrEmpty(storeInfo.MinimarketPriority) ? null : Dictionary.GetDictionary(storeInfo.MinimarketPriority).NameENUS;
            excelDto.TADesrability = string.IsNullOrEmpty(storeInfo.TADesrability) ? null : Dictionary.GetDictionary(storeInfo.TADesrability).NameENUS;
            excelDto.MMTADescription = this.MMTADescription;
            excelDto.MajorGenerators = this.MajorGenerators;
            excelDto.SitePortfolioType = storeInfo.SitePortfolioType;
            excelDto.SiteRerating = string.IsNullOrEmpty(storeInfo.SiteRerating) ? null : Dictionary.GetDictionary(storeInfo.SiteRerating).NameENUS;
            excelDto.SiteDescription = this.SiteDescription;
            excelDto.TADesrabilityNew = string.IsNullOrEmpty(this.TADesirability) ? null : Dictionary.GetDictionary(this.TADesirability).NameENUS;
            excelDto.MMTADescriptionNew = this.MMTADescriptionNew;
            excelDto.MajorGeneratorsNew = this.MajorGeneratorsNew;
            excelDto.SiteRERatingFuture = string.IsNullOrEmpty(this.SiteRERating) ? null : Dictionary.GetDictionary(this.SiteRERating).NameENUS;
            excelDto.LeasedArea = storeInfo.LeasedArea;
            excelDto.OperationSize = storeInfo.OperationSize;
            excelDto.Floors = storeInfo.Floors;
            excelDto.Seats = storeInfo.Seats;
            excelDto.BEType = storeInfo.BEType;
            excelDto.LeaseTenureAndTerm = storeInfo.LeaseTenureAndTerm;
            excelDto.FreeRentalPeriod = storeInfo.FreeRentalPeriod;
            excelDto.RentStructure = storeInfo.RentStructure;
            excelDto.MFStructure = this.MFStructure;
            excelDto.LeasedAreaNew = this.LeasedArea;
            excelDto.OperationSizeNew = this.OperationSize;
            excelDto.FloorsNew = this.Floors;
            excelDto.SeatsNew = this.Seats;
            excelDto.BETypeNew = this.BEType;
            excelDto.LeaseTenureAndTermNew = this.LeaseTenureAndTerm;
            excelDto.FreeRentalPeriodNew = this.FreeRentalPeriod;
            excelDto.RentStructureNew = this.RentStructure;
            excelDto.MFStructureNew = this.MFStructureNew;
            excelDto.DRMFLastTY = this.DRMFLastTY;
            excelDto.DRMFLastTYSales = this.DRMFLastTYSales;
            excelDto.DRMF1stTY = this.DRMF1stTY;
            excelDto.DRMF1stTYSales = this.DRMF1stTYSales;
            excelDto.SRMFLastTY = storeInfo.SRMFLastTY;
            excelDto.SRMFLastTYSales = storeInfo.SRMFLastTYSales;
            excelDto.SRMF1stTY = storeInfo.SRMF1stTY;
            excelDto.SRMF1stTYSales = storeInfo.SRMF1stTYSales;
            excelDto.FairMarketRentAmount = this.FairMarketRentAmount;
            excelDto.FairMarketRentAgent = this.FairMarketRentAgent;
            excelDto.DR1stTYAmount = this.DR1stTYAmount;
            excelDto.AnnualSOILastTY = this.AnnualSOILastTY;
            excelDto.AnnualSOIAvg = storeInfo.AnnualSOIAvg;
            excelDto.CashROILastTY = this.CashROILastTY;
            excelDto.CashROIAvg = storeInfo.CashROIAvg;
            excelDto.LastRemodeling = this.LastRemodeling;
            excelDto.OriginalInvestment = storeInfo.OriginalInvestment;
            excelDto.NBV = storeInfo.NBV;
            excelDto.AdditionalInvestmentCost = storeInfo.AdditionalInvestmentCost;
            excelDto.ExclusivityClause = DataConverter.ToYesNo(storeInfo.ExclusivityClause);
            excelDto.McDsEarlyTerminationRight = storeInfo.McDsEarlyTerminationRight;
            excelDto.McDsEPRight = this.McDsEPRight;
            excelDto.LandlordEntity = storeInfo.LandlordEntity;
            excelDto.SpecialClauses = this.SpecialClauses;
            excelDto.OtherIssues = this.OtherIssues;
            excelDto.ExclusivityClauseNew = this.ExclusivityClauseNew;
            excelDto.McDsEarlyTerminationRightNew = this.McDsEarlyTerminationRightNew;
            excelDto.McDsEPRightNew = this.McDsEPRightNew;
            excelDto.LandlordEntityNew = this.LandlordEntityNew;
            excelDto.SpecialClausesNew = this.SpecialClausesNew;
            excelDto.OtherIssuesNew = this.OtherIssuesNew;
            excelDto.DecisionLogic = this.DecisionLogic;
            excelDto.SpecialApprovalRequired = this.SpecialApprovalRequired;
            excelDirector.Input(excelDto);
            var att = Attachment.FirstOrDefault(e => e.RefTableID == this.Id.ToString() && e.TypeCode == "RenewalAnalysis");
            bool hasAttach = true;
            if (att == null)
            {
                hasAttach = false;
                att = new Attachment();
                att.ID = Guid.NewGuid();
            }

            att.TypeCode = "RenewalAnalysis";
            att.RefTableID = this.Id.ToString();
            att.RefTableName = "RenewalAnalysis";
            att.Name = "Renewal Analysis";
            att.Extension = extetion;
            att.RelativePath = "/";
            att.InternalName = internalName;
            att.ContentType = "application/vnd.ms-excel";
            att.Length = (int)fileInfo.Length;
            att.CreatorID = ClientCookie.UserCode;
            att.CreatorNameENUS = ClientCookie.UserNameENUS;
            att.CreatorNameZHCN = ClientCookie.UserNameZHCN;
            att.CreateTime = DateTime.Now;
            att.RequirementId = new Guid("06F33F98-76B9-440F-8E8C-9167B4C202B9");
            if (hasAttach)
            {
                att.Update();
            }
            else
            {
                att.Add();
            }
        }
    }
}
