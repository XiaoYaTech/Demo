using Mcdonalds.AM.Services.Common;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   10/28/2014 6:25:54 PM
 * FileName     :   RenewalToolExcelData
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;

namespace Mcdonalds.AM.DataAccess.Common.Excel
{
    public class RenewalToolExcelData : ExcelDataBase
    {
        private readonly string _outputCol;
        public RenewalToolExcelData(FileInfo fileInfo)
            : base(fileInfo)
        {
            this.SheetName = "PMT";
            EndRow = 16;
            _outputCol = "E";
        }
        public override void Parse(OfficeOpenXml.ExcelWorksheet worksheet, int currRow)
        {
            var finMeasureOutput = (RenewalToolFinMeasureOutput)Entity;
            if (currRow > 1)
            {
                var output = GetExcelRange<string>(worksheet, currRow, _outputCol);
                switch (currRow)
                {
                    case 2:
                        finMeasureOutput.AnnualRentExpenseLY = DataConverter.ToDecimalNullable(output);
                        break;
                    case 3:
                        finMeasureOutput.RentAsProdSalesLY = DataConverter.ToDecimalNullable(output);
                        break;
                    case 4:
                        finMeasureOutput.OccupancyProdSalesLY = DataConverter.ToDecimalNullable(output);
                        break;
                    case 5:
                        finMeasureOutput.SOIProdSalesLY = DataConverter.ToDecimalNullable(output);
                        break;
                    case 6:
                        finMeasureOutput.CashROILY = DataConverter.ToDecimalNullable(output);
                        break;
                    case 7:
                        finMeasureOutput.AnnualRentExpenseYr1 = DataConverter.ToDecimalNullable(output);
                        break;
                    case 8:
                        finMeasureOutput.RentAsProdSalesYr1 = DataConverter.ToDecimalNullable(output);
                        break;
                    case 9:
                        finMeasureOutput.OccupancyProdSalesYr1 = DataConverter.ToDecimalNullable(output);
                        break;
                    case 10:
                        finMeasureOutput.SOIProdSalesYr1 = DataConverter.ToDecimalNullable(output);
                        break;
                    case 11:
                        finMeasureOutput.CashROIYr1 = DataConverter.ToDecimalNullable(output);
                        break;
                    case 12:
                        finMeasureOutput.AnnualRentExpenseAvg = DataConverter.ToDecimalNullable(output);
                        break;
                    case 13:
                        finMeasureOutput.RentAsProdSalesAvg = DataConverter.ToDecimalNullable(output);
                        break;
                    case 14:
                        finMeasureOutput.OccupancyProdSalesAvg = DataConverter.ToDecimalNullable(output);
                        break;
                    case 15:
                        finMeasureOutput.SOIProdSalesAvg = DataConverter.ToDecimalNullable(output);
                        break;
                    case 16:
                        finMeasureOutput.CashROIAvg = DataConverter.ToDecimalNullable(output);
                        break;
                }
            }
        }

        public override void Import()
        {
            var finMeasureOutput = (RenewalToolFinMeasureOutput)Entity;
            if (RenewalToolFinMeasureOutput.Any(e => e.Id == finMeasureOutput.Id))
            {
                finMeasureOutput.Update();
            }
            else
            {
                finMeasureOutput.Add();
            }
        }

        public override void Edit(ExcelWorksheet worksheet, ExcelInputDTO excelInput)
        {
            worksheet.Cells["B54"].Value = excelInput.ComSalesDesc;
        }

        public override void Input(OfficeOpenXml.ExcelWorksheet worksheet, ExcelInputDTO inputInfo)
        {
            worksheet.Cells["B2"].Value = inputInfo.Market;
            worksheet.Cells["B3"].Value = inputInfo.USCode;
            worksheet.Cells["B4"].Value = inputInfo.StoreName;
            worksheet.Cells["B5"].Value = inputInfo.OpenDate;
            worksheet.Cells["B6"].Value = inputInfo.LeaseExpirationDate;
            worksheet.Cells["B7"].Value = inputInfo.RenewalYears;
            worksheet.Cells["B8"].Value = inputInfo.ProductSales;
            worksheet.Cells["B9"].Value = inputInfo.Pac;
            worksheet.Cells["B10"].Value = inputInfo.Rent;
            worksheet.Cells["B11"].Value = inputInfo.DepreciationLhi;
            worksheet.Cells["B12"].Value = inputInfo.InterestLhi;
            worksheet.Cells["B13"].Value = inputInfo.ServiceFee;
            worksheet.Cells["B14"].Value = inputInfo.Accounting;
            worksheet.Cells["B15"].Value = inputInfo.Insurance;
            worksheet.Cells["B16"].Value = inputInfo.TaxesAndLicenses;
            worksheet.Cells["B17"].Value = inputInfo.DepreciationEssd;
            worksheet.Cells["B18"].Value = inputInfo.InterestEssd;
            worksheet.Cells["B19"].Value = inputInfo.OtherIncExp;
            worksheet.Cells["B20"].Value = inputInfo.NonProductSales;
            worksheet.Cells["B21"].Value = inputInfo.NonProductCosts;
            worksheet.Cells["B22"].Value = inputInfo.REII;
            worksheet.Cells["B23"].Value = inputInfo.LHIII;
            worksheet.Cells["B24"].Value = inputInfo.ESSDII;
            worksheet.Cells["B25"].Value = inputInfo.RENBV;
            worksheet.Cells["B26"].Value = inputInfo.LHINBV;
            worksheet.Cells["B27"].Value = inputInfo.ESSDNBV;
            worksheet.Cells["B28"].Value = inputInfo.RECost;
            worksheet.Cells["B29"].Value = inputInfo.LHICost;
            worksheet.Cells["B30"].Value = inputInfo.ESSDCost;
            worksheet.Cells["B31"].Value = inputInfo.TotalWriteOff;
            worksheet.Cells["B32"].Value = inputInfo.RentalStructure;
            worksheet.Cells["B33"].Value = inputInfo.ContributionMargin;
            worksheet.Cells["B34"].Value = inputInfo.SalesCompYr1;
            worksheet.Cells["B35"].Value = inputInfo.SalesCompYr2;
            worksheet.Cells["B36"].Value = inputInfo.SalesCompYr3;
            worksheet.Cells["B37"].Value = inputInfo.SalesCompYr4;
            worksheet.Cells["B38"].Value = inputInfo.SalesCompYr5;
            worksheet.Cells["B39"].Value = inputInfo.SalesCompYr6;
            worksheet.Cells["B40"].Value = inputInfo.SalesCompYr7;
            worksheet.Cells["B41"].Value = inputInfo.SalesCompYr8;
            worksheet.Cells["B42"].Value = inputInfo.SalesCompYr9;
            worksheet.Cells["B43"].Value = inputInfo.SalesCompYr10;
            worksheet.Cells["B44"].Value = inputInfo.SalesCompYr11;
            worksheet.Cells["B45"].Value = inputInfo.SalesCompYr12;
            worksheet.Cells["B46"].Value = inputInfo.SalesCompYr13;
            worksheet.Cells["B47"].Value = inputInfo.SalesCompYr14;
            worksheet.Cells["B48"].Value = inputInfo.SalesCompYr15;
            worksheet.Cells["B49"].Value = inputInfo.SalesCompYr16;
            worksheet.Cells["B50"].Value = inputInfo.SalesCompYr17;
            worksheet.Cells["B51"].Value = inputInfo.SalesCompYr18;
            worksheet.Cells["B52"].Value = inputInfo.SalesCompYr19;
            worksheet.Cells["B53"].Value = inputInfo.SalesCompYr20;
            worksheet.Cells["B54"].Value = inputInfo.ComSalesDesc;
            worksheet.Cells["B55"].Value = inputInfo.CompSales;
            worksheet.Cells["B56"].Value = inputInfo.FinanceYear + "-" + inputInfo.FinanceMonth;
            
        }
    }
}
