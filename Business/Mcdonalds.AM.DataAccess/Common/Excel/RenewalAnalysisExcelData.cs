/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   10/21/2014 11:33:18 AM
 * FileName     :   RenewalAnalysisExcelData
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.Common.Excel
{
    public class RenewalAnalysisExcelData : ExcelDataBase
    {
        private readonly string _outputCol;
        public RenewalAnalysisExcelData(FileInfo fileInfo)
            : base(fileInfo)
        {
            this.SheetName = "PMT";
            this.EndRow = 17;
            _outputCol = "E";
        }
        public override void Parse(OfficeOpenXml.ExcelWorksheet worksheet, int currRow)
        {
        }

        public override void Import()
        {
            var analysis = (RenewalAnalysis)Entity;
            if (RenewalAnalysis.Any(e => e.Id == analysis.Id))
            {
                analysis.Update();
            }
            else
            {
                analysis.Add();
            }
        }

        public override void Input(OfficeOpenXml.ExcelWorksheet worksheet, ExcelInputDTO inputInfo)
        {
            worksheet.Cells["B2"].Value = inputInfo.USCode;
            worksheet.Cells["B3"].Value = inputInfo.StoreNameEN;
            worksheet.Cells["B4"].Value = inputInfo.StoreNameCN;
            worksheet.Cells["B5"].Value = worksheet.Cells["B12"].Value = inputInfo.MinimarketPriority;
            worksheet.Cells["B6"].Value = inputInfo.TADesrability;
            worksheet.Cells["B7"].Value = inputInfo.MMTADescription;
            worksheet.Cells["B8"].Value = inputInfo.MajorGenerators;
            worksheet.Cells["B9"].Value = worksheet.Cells["B16"].Value = inputInfo.SitePortfolioType;
            worksheet.Cells["B10"].Value = inputInfo.SiteRerating;
            worksheet.Cells["B11"].Value = worksheet.Cells["B18"].Value = inputInfo.SiteDescription;
            worksheet.Cells["B13"].Value = inputInfo.TADesrabilityNew;
            worksheet.Cells["B14"].Value = inputInfo.MMTADescriptionNew;
            worksheet.Cells["B15"].Value = inputInfo.MajorGeneratorsNew;
            worksheet.Cells["B17"].Value = inputInfo.SiteRERatingFuture;
            worksheet.Cells["B19"].Value = inputInfo.LeasedArea;
            worksheet.Cells["B20"].Value = inputInfo.OperationSize;
            worksheet.Cells["B21"].Value = inputInfo.Floors;
            worksheet.Cells["B22"].Value = inputInfo.Seats;
            worksheet.Cells["B23"].Value = inputInfo.BEType;
            worksheet.Cells["B24"].Value = inputInfo.LeaseTenureAndTerm;
            worksheet.Cells["B25"].Value = inputInfo.FreeRentalPeriod;
            worksheet.Cells["B26"].Value = inputInfo.RentStructure;
            worksheet.Cells["B27"].Value = inputInfo.MFStructure;
            worksheet.Cells["B28"].Value = inputInfo.LeasedAreaNew;
            worksheet.Cells["B29"].Value = inputInfo.OperationSizeNew;
            worksheet.Cells["B30"].Value = inputInfo.FloorsNew;
            worksheet.Cells["B31"].Value = inputInfo.SeatsNew;
            worksheet.Cells["B32"].Value = inputInfo.BETypeNew;
            worksheet.Cells["B33"].Value = inputInfo.LeaseTenureAndTermNew;
            worksheet.Cells["B34"].Value = inputInfo.FreeRentalPeriodNew;
            worksheet.Cells["B35"].Value = inputInfo.RentStructureNew;
            worksheet.Cells["B36"].Value = inputInfo.MFStructureNew;
            worksheet.Cells["B37"].Value = inputInfo.DRMFLastTY;
            worksheet.Cells["B38"].Value = inputInfo.DRMFLastTYSales;
            worksheet.Cells["B39"].Value = inputInfo.DRMF1stTY;
            worksheet.Cells["B40"].Value = inputInfo.DRMF1stTYSales;
            worksheet.Cells["B41"].Value = inputInfo.SRMFLastTY;
            worksheet.Cells["B42"].Value = inputInfo.SRMFLastTYSales;
            worksheet.Cells["B43"].Value = inputInfo.SRMF1stTY;
            worksheet.Cells["B44"].Value = inputInfo.SRMF1stTYSales;
            worksheet.Cells["B45"].Value = inputInfo.FairMarketRentAmount;
            worksheet.Cells["B46"].Value = inputInfo.FairMarketRentAgent;
            worksheet.Cells["B47"].Value = inputInfo.DR1stTYAmount;
            worksheet.Cells["B48"].Value = inputInfo.AnnualSOILastTY;
            worksheet.Cells["B49"].Value = inputInfo.AnnualSOIAvg;
            worksheet.Cells["B50"].Value = inputInfo.CashROILastTY;
            worksheet.Cells["B51"].Value = inputInfo.CashROIAvg;
            worksheet.Cells["B52"].Value = inputInfo.LastRemodeling;
            worksheet.Cells["B53"].Value = inputInfo.OriginalInvestment;
            worksheet.Cells["B54"].Value = inputInfo.NBV;
            worksheet.Cells["B55"].Value = inputInfo.AdditionalInvestmentCost;
            worksheet.Cells["B56"].Value = inputInfo.ExclusivityClause;
            worksheet.Cells["B57"].Value = inputInfo.McDsEarlyTerminationRight;
            worksheet.Cells["B58"].Value = inputInfo.McDsEPRight;
            worksheet.Cells["B59"].Value = inputInfo.LandlordEntity;
            worksheet.Cells["B60"].Value = inputInfo.SpecialClauses;
            worksheet.Cells["B61"].Value = inputInfo.OtherIssues;
            worksheet.Cells["B62"].Value = inputInfo.ExclusivityClauseNew;
            worksheet.Cells["B63"].Value = inputInfo.McDsEarlyTerminationRightNew;
            worksheet.Cells["B64"].Value = inputInfo.McDsEPRightNew;
            worksheet.Cells["B65"].Value = inputInfo.LandlordEntityNew;
            worksheet.Cells["B66"].Value = inputInfo.SpecialClausesNew;
            worksheet.Cells["B67"].Value = inputInfo.OtherIssuesNew;
            worksheet.Cells["B68"].Value = inputInfo.DecisionLogic;
            worksheet.Cells["B69"].Value = inputInfo.SpecialApprovalRequired;
        }
    }
}
