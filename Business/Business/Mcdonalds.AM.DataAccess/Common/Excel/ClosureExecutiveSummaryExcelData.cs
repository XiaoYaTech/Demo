using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess.Common.Extensions;
using OfficeOpenXml;

namespace Mcdonalds.AM.DataAccess.Common.Excel
{
    public class ClosureExecutiveSummaryExcelData : ExcelDataBase
    {
        public ClosureExecutiveSummaryExcelData(FileInfo fileInfo)
            : base(fileInfo)
        {
            SheetName = "PMT";
        }

        public override void Parse(ExcelWorksheet worksheet, int currRow)
        {
            throw new NotImplementedException();
        }

        public override void Import()
        {
            throw new NotImplementedException();
        }

        public override void Input(ExcelWorksheet worksheet, ExcelInputDTO inputInfo)
        {
            worksheet.Cells["B2"].Value = inputInfo.StoreNameCN.AsString();
            worksheet.Cells["B3"].Value = inputInfo.USCode.AsString();
            worksheet.Cells["B4"].Value = inputInfo.City.AsString();
            worksheet.Cells["B5"].Value = inputInfo.Market.AsString();
            worksheet.Cells["B6"].Value = inputInfo.OpenDate;
            worksheet.Cells["B7"].Value = inputInfo.Floor.AsString();
            worksheet.Cells["B8"].Value = inputInfo.TotalArea.AsString();
            worksheet.Cells["B9"].Value = inputInfo.TotalSeatsNo.AsString();
            worksheet.Cells["B10"].Value = inputInfo.BE;
            worksheet.Cells["B11"].Value = inputInfo.LeasingTerm;
            worksheet.Cells["B12"].Value = inputInfo.RentCommencementDate;
            worksheet.Cells["B13"].Value = inputInfo.LeaseExpirationDate;
            worksheet.Cells["B14"].Value = inputInfo.RentType;
            worksheet.Cells["B15"].Value = inputInfo.RentStructure;
            worksheet.Cells["B16"].Value = TryParseDecimal(inputInfo.TotalSales_TTM);
            worksheet.Cells["B17"].Value = TryParseDecimal(inputInfo.SalesComp_TTM);
            worksheet.Cells["B18"].Value = TryParseDecimal(inputInfo.GCComp_TTM);
            worksheet.Cells["B19"].Value = TryParseDecimal(inputInfo.PAC_TTM);
            worksheet.Cells["B20"].Value = TryParseDecimal(inputInfo.SOI_TTM);
            worksheet.Cells["B21"].Value = TryParseDecimal(inputInfo.CASHFLOW_TTM);
            worksheet.Cells["B22"].Value = TryParseDecimal(inputInfo.TotalSales_TTMY1);
            worksheet.Cells["B23"].Value = TryParseDecimal(inputInfo.CompSales_TTMY1);
            worksheet.Cells["B24"].Value = TryParseDecimal(inputInfo.CompGC_TTMY1);
            worksheet.Cells["B25"].Value = TryParseDecimal(inputInfo.PAC_TTMY1);
            worksheet.Cells["B26"].Value = TryParseDecimal(inputInfo.SOI_TTMY1);
            worksheet.Cells["B27"].Value = TryParseDecimal(inputInfo.CashFlow_TTMY1);
            worksheet.Cells["B28"].Value = TryParseDecimal(inputInfo.TotalSales_TTMY2);
            worksheet.Cells["B29"].Value = TryParseDecimal(inputInfo.CompSales_TTMY2);
            worksheet.Cells["B30"].Value = TryParseDecimal(inputInfo.CompGC_TTMY2);
            worksheet.Cells["B31"].Value = TryParseDecimal(inputInfo.PAC_TTMY2);
            worksheet.Cells["B32"].Value = TryParseDecimal(inputInfo.SOI_TTMY2);
            worksheet.Cells["B33"].Value = TryParseDecimal(inputInfo.CashFlow_TTMY2);
            worksheet.Cells["B34"].Value = inputInfo.RemoteKiosk1_Status.AsString();
            worksheet.Cells["B35"].Value = inputInfo.RemoteKiosk2_Status.AsString();
            worksheet.Cells["B36"].Value = inputInfo.RemoteKiosk3_Status.AsString();
            worksheet.Cells["B37"].Value = inputInfo.AttachedKiosk1_Status.AsString();
            worksheet.Cells["B38"].Value = inputInfo.AttachedKiosk2_Status.AsString();
            worksheet.Cells["B39"].Value = inputInfo.AttachedKiosk3_Status.AsString();
            worksheet.Cells["B40"].Value = inputInfo.MDS_Status.AsString();
            worksheet.Cells["B41"].Value = inputInfo.McCafe_Status.AsString();
            worksheet.Cells["B42"].Value = inputInfo.TwentyFourHour_Status.AsString();
            worksheet.Cells["B43"].Value = inputInfo.RemoteKiosk1_OpenDate;
            worksheet.Cells["B44"].Value = inputInfo.RemoteKiosk2_OpenDate;
            worksheet.Cells["B45"].Value = inputInfo.RemoteKiosk3_OpenDate;
            worksheet.Cells["B46"].Value = inputInfo.AttachedKiosk1_OpenDate;
            worksheet.Cells["B47"].Value = inputInfo.AttachedKiosk2_OpenDate;
            worksheet.Cells["B48"].Value = inputInfo.AttachedKiosk3_OpenDate;
            worksheet.Cells["B49"].Value = inputInfo.MDS_OpenDate;
            worksheet.Cells["B50"].Value = inputInfo.McCafe_OpenDate;
            worksheet.Cells["B51"].Value = inputInfo.TwentyFourHour_OpenDate;
            worksheet.Cells["B52"].Value = TryParseDecimal(inputInfo.LHI_NBV);
            worksheet.Cells["B53"].Value = TryParseDecimal(inputInfo.ESSD_NBV);
            worksheet.Cells["B54"].Value = TryParseDecimal(inputInfo.TotalCost_NBV);
            worksheet.Cells["B55"].Value = inputInfo.CurrentSituation.AsString();
            worksheet.Cells["B56"].Value = inputInfo.NegotiationHistory.AsString();
            worksheet.Cells["B57"].Value = inputInfo.ProposedSolution.AsString();
            worksheet.Cells["B58"].Value = inputInfo.SalesTransfer.AsString();
            worksheet.Cells["B59"].Value = inputInfo.MiniMarket.AsString();
            worksheet.Cells["B60"].Value = inputInfo.StoreLocation.AsString();
        }

        private object TryParseDecimal(string val)
        {
            decimal result = 0;
            if (Decimal.TryParse(val, out result))
                return result;
            else
                return "";
        }
    }
}
