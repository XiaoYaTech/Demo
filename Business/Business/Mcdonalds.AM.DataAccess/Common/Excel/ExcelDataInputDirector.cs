using System;
using System.Collections.Generic;
using System.IO;
using System.Security.AccessControl;
using OfficeOpenXml;
using OfficeOpenXml.Table;

namespace Mcdonalds.AM.DataAccess.Common.Excel
{
    public class ExcelDataInputDirector
    {
        private readonly ExcelDataBase _excelData;
        private readonly ExcelDataInputType _outputType;

        public delegate void FillEntityEventHandler(BaseAbstractEntity entity);

        public FillEntityEventHandler FillEntityEvent;
        public ExcelDataBase ExcelData
        {
            get { return _excelData; }
        }

        public ExcelDataInputDirector(FileInfo fileInfo, ExcelDataInputType outputType)
        {
            _outputType = outputType;
            switch (_outputType)
            {
                case ExcelDataInputType.WriteOffAmount:
                    _excelData = new WriteOffAmountExcelData(fileInfo);
                    break;
                case ExcelDataInputType.ReinvestmentCost:
                    _excelData = new ReinvestmentCostExcelData(fileInfo);
                    break;
                case ExcelDataInputType.MajorLeaseChangeCover:
                    _excelData = new MajorLeaseChangeCoverExcelData(fileInfo);
                    break;
                case ExcelDataInputType.RebuildCover:
                    _excelData = new RebuildCoverExcelData(fileInfo);
                    break;
                case ExcelDataInputType.ClosureWOCheckList:
                    _excelData = new ClosureWOCheckListExcelData(fileInfo);
                    break;
                case ExcelDataInputType.ClosureTool:
                    _excelData = new ClosureToolExcelData(fileInfo);
                    break;
                case ExcelDataInputType.ClosureExecutiveSummary:
                    _excelData = new ClosureExecutiveSummaryExcelData(fileInfo);
                    break;
                case ExcelDataInputType.ReimageSummary:
                    _excelData = new ReimageSummaryExcelData(fileInfo);
                    break;
                case ExcelDataInputType.RenewalLLNegotiationRecord:
                    _excelData = new RenewalLLNegotiationRecordData(fileInfo);
                    break;
                case ExcelDataInputType.RenewalAnalysis:
                    _excelData = new RenewalAnalysisExcelData(fileInfo);
                    break;
                case ExcelDataInputType.RenewalTool:
                    _excelData = new RenewalToolExcelData(fileInfo);
                    break;
                case ExcelDataInputType.RenewalCover:
                    _excelData = new RenewalCoverExcelData(fileInfo);
                    break;
                case ExcelDataInputType.CommentsList:
                    _excelData = new CommentsListExcelData(fileInfo);
                    break;
                case ExcelDataInputType.ClosureCover:
                    _excelData = new ClosureCoverExcelData(fileInfo);
                    break;
                case ExcelDataInputType.ReimageCover:
                    _excelData = new ReimageCoverExcelData(fileInfo);
                    break;
                case ExcelDataInputType.TempClosureCover:
                    _excelData = new TempClosureCoverExcelData(fileInfo);
                    break;
            }
        }

        public ExcelDataInputDirector(ExcelDataBase excelData)
        {
            _excelData = excelData;
        }


        public void Input(ExcelInputDTO excelInput)
        {
            using (var excelPackage = new ExcelPackage(_excelData.FileInfo))
            {
                var worksheet = excelPackage.Workbook.Worksheets[_excelData.SheetName];

                _excelData.Input(worksheet, excelInput);

                excelPackage.Save();
            }
        }

        public void Edit(ExcelInputDTO excelInput)
        {
            using (var excelPackage = new ExcelPackage(_excelData.FileInfo))
            {
                var worksheet = excelPackage.Workbook.Worksheets[_excelData.SheetName];

                _excelData.Edit(worksheet, excelInput);

                excelPackage.Save();
            }
        }

        public void ListInput(List<ExcelInputDTO> excelInputList)
        {
            using (var excelPackage = new ExcelPackage(_excelData.FileInfo))
            {
                var worksheet = excelPackage.Workbook.Worksheets[_excelData.SheetName];
                foreach (var excelInput in excelInputList)
                {
                    _excelData.Input(worksheet, excelInput);
                }

                excelPackage.Save();
            }
        }


        public string GetCellValue(int row, string col)
        {
            using (var excelPackage = new ExcelPackage(_excelData.FileInfo))
            {
                var worksheet = excelPackage.Workbook.Worksheets[_excelData.SheetName];

                if (worksheet == null)
                    return "";

                return ExcelHelper.GetExcelRange<decimal>(worksheet.Cells[row, ExcelHelper.ToExcelIndex(col)]).ToString();
            }
        }



        public static void SaveToExcel<T>(List<VProject> list, string filePath)
        {
            ExcelPackage ep = new OfficeOpenXml.ExcelPackage();
            ExcelWorkbook wb = ep.Workbook;
            ExcelWorksheet ws = wb.Worksheets.Add(typeof(T).Name);

            //配置文件属性
            //wb.Properties.Category = "类别";
            //wb.Properties.Author = "作者";
            //wb.Properties.Comments = "备注";
            //wb.Properties.Company = "公司";
            //wb.Properties.Keywords = "关键字";
            //wb.Properties.Manager = "管理者";
            //wb.Properties.Status = "内容状态";
            //wb.Properties.Subject = "主题";
            //wb.Properties.Title = "标题";
            //wb.Properties.LastModifiedBy = "最后一次保存者";

            FileInfo file = new FileInfo(filePath);
            //ws.Cells["A1"].LoadFromCollection(data, true, TableStyles.Medium10);
            ws.Cells["A1"].Value = "Project Type";
            ws.Cells["B1"].Value = "USCode";
            ws.Cells["C1"].Value = "Store Name";
            ws.Cells["D1"].Value = "Asset Actor";
            ws.Cells["E1"].Value = "City";
            ws.Cells["F1"].Value = "Create Time";
            ws.Cells["G1"].Value = "Status";
            ws.Cells["H1"].Value = "Reimage 1st Rd Filter";

            for (int i = 0; i < list.Count; i++)
            {
                var item = list[i];
                ws.Cells[i + 2, 1].Value = item.FlowCode;
                ws.Cells[i + 2, 2].Value = item.USCode;
                ws.Cells[i + 2, 3].Value = item.StoreNameZHCN;
                ws.Cells[i + 2, 4].Value = item.AssetActorNameENUS;
                ws.Cells[i + 2, 5].Value = item.CityZHCN;
                ws.Cells[i + 2, 6].Value = item.CreateTime.Value.ToString("yyyy-MM-dd HH:mm:ss");
                ws.Cells[i + 2, 7].Value = item.Status == ProjectStatus.UnFinish ? "Active" : item.Status.ToString();
                ws.Cells[i + 2, 8].Value = item.HoldingProjectStatus.HasValue ? item.HoldingProjectStatus.ToString() : "";
            }

            ep.SaveAs(file);
        }
    }

    public enum ExcelDataInputType
    {
        ReinvestmentCost,
        WriteOffAmount,
        MajorLeaseChangeCover,
        RebuildCover,
        ReimageCover,
        ClosureWOCheckList,
        ClosureTool,
        ClosureExecutiveSummary,
        ClosureCover,
        ReimageSummary,
        RenewalLLNegotiationRecord,
        RenewalAnalysis,
        RenewalTool,
        RenewalCover,
        TempClosureCover,
        CommentsList
    }


    public class ExcelInputDTO
    {
        public string TotalSales_TTM { get; set; }
        public string SalesComp_TTM { get; set; }
        public string Region { get; set; }

        public string Province { get; set; }
        public string Market { get; set; }

        public string StoreName { get; set; }

        public string USCode { get; set; }

        public string StoreType { get; set; }
        public string StoreTypeName { get; set; }

        public DateTime ClosureDate { get; set; }

        public string ProjectManager { get; set; }

        public string City { get; set; }

        public string StoreNameEN { get; set; }

        public string StoreNameCN { get; set; }

        public DateTime OpenDate { get; set; }

        public DateTime? GBDate { get; set; }

        public DateTime? ConsCompletionDate { get; set; }

        public string NewDesignType { get; set; }


        public string NormType { get; set; }

        public string EstimatedSeatNO { get; set; }

        public string NewDTSiteArea { get; set; }

        public string NewOperationArea { get; set; }

        public string NewDiningArea { get; set; }

        public string WallPanelArea { get; set; }

        public string WallGraphicArea { get; set; }

        public string FacadeACMArea { get; set; }

        public bool? NewRemoteKiosk { get; set; }

        public bool? NewAttachedKiosk { get; set; }
        public bool? NewMcCafe { get; set; }
        public bool? NewMDS { get; set; }

        public string ProjectId { get; set; }
        public string Floor { get; set; }
        public string TotalArea { get; set; }
        public string TotalSeatsNo { get; set; }
        public int BE { get; set; }
        public string RentType { get; set; }
        public string RentStructure { get; set; }
        public string LeasingTerm { get; set; }
        public DateTime RentCommencementDate { get; set; }
        public DateTime LeaseExpirationDate { get; set; }
        public string GCComp_TTM { get; set; }
        public string PAC_TTM { get; set; }
        public string SOI_TTM { get; set; }
        public string CASHFLOW_TTM { get; set; }
        public string TotalSales_TTMY1 { get; set; }
        public string CompSales_TTMY1 { get; set; }
        public string CompGC_TTMY1 { get; set; }
        public string PAC_TTMY1 { get; set; }
        public string SOI_TTMY1 { get; set; }
        public string CashFlow_TTMY1 { get; set; }
        public string TotalSales_TTMY2 { get; set; }
        public string CompSales_TTMY2 { get; set; }
        public string CompGC_TTMY2 { get; set; }
        public string PAC_TTMY2 { get; set; }
        public string SOI_TTMY2 { get; set; }
        public string CashFlow_TTMY2 { get; set; }
        public string RemoteKiosk1_Status { get; set; }
        public string RemoteKiosk1_OpenDate { get; set; }
        public string RemoteKiosk2_Status { get; set; }
        public string RemoteKiosk2_OpenDate { get; set; }
        public string RemoteKiosk3_Status { get; set; }
        public string RemoteKiosk3_OpenDate { get; set; }
        public string AttachedKiosk1_Status { get; set; }
        public string AttachedKiosk1_OpenDate { get; set; }
        public string AttachedKiosk2_Status { get; set; }
        public string AttachedKiosk2_OpenDate { get; set; }
        public string AttachedKiosk3_Status { get; set; }
        public string AttachedKiosk3_OpenDate { get; set; }
        public string MDS_Status { get; set; }
        public string MDS_OpenDate { get; set; }
        public string McCafe_Status { get; set; }
        public string McCafe_OpenDate { get; set; }
        public string TwentyFourHour_Status { get; set; }
        public string TwentyFourHour_OpenDate { get; set; }
        public string LHI_NBV { get; set; }
        public string ESSD_NBV { get; set; }
        public string TotalCost_NBV { get; set; }
        public string ReopenDate { get; set; }
        public string TempClosureDate { get; set; }
        public string FlowCode { get; set; }
        public string McdParticipants { get; set; }
        public string Content { get; set; }
        public string LLParticipants { get; set; }
        public string Topic { get; set; }
        public string Location { get; set; }
        public string Date { get; set; }
        public string MiniMarket { get; set; }
        public string StoreLocation { get; set; }
        public string CurrentSituation { get; set; }
        public string NegotiationHistory { get; set; }
        public string ProposedSolution { get; set; }
        public string SalesTransfer { get; set; }

        #region Closure
        public string ActualCloseDate { get; set; }
        public string PMNameENUS { get; set; }
        #endregion

        #region Renewal Analysis Excel Template
        public string MinimarketPriority { get; set; }
        public string TADesrability { get; set; }
        public string SitePortfolioType { get; set; }
        public string SiteRerating { get; set; }
        public string LeasedArea { get; set; }
        public string OperationSize { get; set; }
        public string Floors { get; set; }
        public string Seats { get; set; }
        //public string BE { get; set; }
        public string LeaseTenure { get; set; }
        public string FreeRentalPeriod { get; set; }
        //public string RentStructure { get; set; }
        public string AnnualSOITotalRenewalPeriodAvg { get; set; }
        public string CashROITotalRenewalPeriodAvg { get; set; }
        public string OriginalInvestment { get; set; }
        public string NBV { get; set; }
        public string ExclusivityClause { get; set; }
        public string McdEarlyTerminationRight { get; set; }
        public string LandlordEntity { get; set; }
        #endregion

        #region Renewal Tool Excel Template
        public double RenewalYears { get; set; }
        #endregion

        #region CommentsList Template

        public string Form { get; set; }
        public string Comments { get; set; }
        public string CreateBy { get; set; }
        public string SendTo { get; set; }
        public string CreateDate { get; set; }
        public string SenderPosition { get; set; }
        public string IsRead { get; set; }

        #endregion
        public decimal? SalesCompYr2 { get; set; }

        public decimal? SalesCompYr1 { get; set; }

        public decimal? ContributionMargin { get; set; }

        public decimal? RentalStructure { get; set; }

        public decimal? TotalWriteOff { get; set; }

        public decimal? ESSDCost { get; set; }

        public decimal? LHICost { get; set; }

        public decimal? RECost { get; set; }

        public decimal ESSDNBV { get; set; }

        public decimal LHINBV { get; set; }

        public decimal? RENBV { get; set; }

        public decimal? ESSDII { get; set; }

        public decimal? LHIII { get; set; }

        public decimal? REII { get; set; }

        public decimal? NonProductCosts { get; set; }

        public decimal? NonProductSales { get; set; }

        public decimal? OtherIncExp { get; set; }

        public decimal? InterestEssd { get; set; }

        public decimal? DepreciationEssd { get; set; }

        public decimal? TaxesAndLicenses { get; set; }

        public decimal? Insurance { get; set; }

        public decimal? Accounting { get; set; }

        public decimal? ServiceFee { get; set; }

        public decimal? InterestLhi { get; set; }

        public decimal? DepreciationLhi { get; set; }

        public decimal? Rent { get; set; }

        public decimal? Pac { get; set; }

        public decimal? ProductSales { get; set; }

        public decimal? SalesCompYr3 { get; set; }

        public decimal? SalesCompYr4 { get; set; }

        public decimal? SalesCompYr5 { get; set; }

        public decimal? SalesCompYr6 { get; set; }

        public decimal? SalesCompYr7 { get; set; }

        public decimal? SalesCompYr8 { get; set; }

        public decimal? SalesCompYr9 { get; set; }

        public decimal? SalesCompYr10 { get; set; }

        public decimal? SalesCompYr11 { get; set; }

        public decimal? SalesCompYr12 { get; set; }

        public decimal? SalesCompYr13 { get; set; }

        public decimal? SalesCompYr14 { get; set; }

        public decimal? SalesCompYr15 { get; set; }

        public decimal? SalesCompYr16 { get; set; }

        public decimal? SalesCompYr17 { get; set; }

        public decimal? SalesCompYr18 { get; set; }

        public decimal? SalesCompYr19 { get; set; }

        public decimal? SalesCompYr20 { get; set; }

        public string ComSalesDesc { get; set; }

        public string Priority { get; set; }

        public object MMTADescription { get; set; }

        public object MajorGenerators { get; set; }

        public object SiteDescription { get; set; }

        public object TADesrabilityNew { get; set; }

        public object MMTADescriptionNew { get; set; }

        public object MajorGeneratorsNew { get; set; }

        public object SiteRERatingFuture { get; set; }

        public object LeaseTenureAndTerm { get; set; }

        public object MFStructure { get; set; }

        public object SpecialApprovalRequired { get; set; }

        public object DecisionLogic { get; set; }

        public object OtherIssuesNew { get; set; }

        public object SpecialClausesNew { get; set; }

        public object LandlordEntityNew { get; set; }

        public object McDsEPRightNew { get; set; }

        public object McDsEarlyTerminationRightNew { get; set; }

        public object ExclusivityClauseNew { get; set; }

        public object OtherIssues { get; set; }

        public object SpecialClauses { get; set; }

        public object McDsEPRight { get; set; }

        public object McDsEarlyTerminationRight { get; set; }

        public object AdditionalInvestmentCost { get; set; }

        public object LastRemodeling { get; set; }

        public decimal? CashROIAvg { get; set; }

        public decimal? CashROILastTY { get; set; }

        public decimal? AnnualSOIAvg { get; set; }

        public decimal? AnnualSOILastTY { get; set; }

        public decimal? DR1stTYAmount { get; set; }

        public decimal? FairMarketRentAgent { get; set; }

        public decimal? FairMarketRentAmount { get; set; }

        public decimal? SRMF1stTYSales { get; set; }

        public decimal? SRMF1stTY { get; set; }

        public decimal? SRMFLastTYSales { get; set; }

        public decimal? SRMFLastTY { get; set; }

        public decimal? DRMF1stTYSales { get; set; }

        public decimal? DRMF1stTY { get; set; }

        public decimal? DRMFLastTYSales { get; set; }

        public decimal? DRMFLastTY { get; set; }

        public object MFStructureNew { get; set; }

        public object RentStructureNew { get; set; }

        public object FreeRentalPeriodNew { get; set; }

        public object LeaseTenureAndTermNew { get; set; }

        public object SeatsNew { get; set; }

        public object FloorsNew { get; set; }

        public object OperationSizeNew { get; set; }

        public object LeasedAreaNew { get; set; }
        public string BEType { get; set; }
        public object BETypeNew { get; set; }
        public string FinanceYear { get; set; }
        public string FinanceMonth { get; set; }
        public decimal? CompSales { get; set; }

        public string MeetingDate { get; set; }
    }
}
