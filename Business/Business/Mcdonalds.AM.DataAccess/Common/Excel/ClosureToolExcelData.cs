using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;

namespace Mcdonalds.AM.DataAccess.Common.Excel
{
    public class ClosureToolExcelData : ExcelDataBase
    {

        public ClosureToolExcelData(FileInfo fileInfo)
            : base(fileInfo)
        {
            SheetName = "PMT";
        }

        public override void Parse(OfficeOpenXml.ExcelWorksheet worksheet, int currRow)
        {
            throw new NotImplementedException();
        }

        public override void Import()
        {
            throw new NotImplementedException();
        }
        //private string GetCellValue(decimal? val)
        //{
        //    string result = string.Empty;
        //    if (val.HasValue)
        //    {
        //        result = val.Value.ToString("f2");
        //    }
        //    return result;
        //}
        public override void Input(OfficeOpenXml.ExcelWorksheet worksheet, ExcelInputDTO inputInfo)
        {
            var closureInfo = ClosureInfo.GetByProjectId(inputInfo.ProjectId);
            var storeInfo = StoreBasicInfo.FirstOrDefault(s => s.StoreCode == closureInfo.USCode);
            var storeContract = StoreContractInfo.Get(storeInfo.StoreCode);
            ClosureTool closureTool = ClosureTool.Get(inputInfo.ProjectId);
            //ClosureWOCheckList closureWoCheckList = ClosureWOCheckList.Get(inputInfo.ProjectId);
            worksheet.Cells["B2"].Value = storeInfo.Market;
            worksheet.Cells["B3"].Value = storeInfo.StoreCode;
            worksheet.Cells["B4"].Value = storeInfo.NameZHCN;

            worksheet.Cells["B5"].Value = storeInfo.OpenDate.ToString("yyyy-MM-dd");
            if (storeContract != null)
            {
                worksheet.Cells["B6"].Value = storeContract.PartyAFullName;
                worksheet.Cells["B7"].Value = storeContract.EndDate.HasValue ? storeContract.EndDate.Value.ToString("yyyy-MM-dd") : "";
            }
            worksheet.Cells["B8"].Value = closureInfo.ClosureTypeNameZHCN;

            if (closureInfo.ActualCloseDate != null)
            {
                worksheet.Cells["B9"].Value = closureInfo.ActualCloseDate.Value.ToString("yyyy-MM-dd");
            }
            worksheet.Cells["B10"].Value = closureTool.Product_Sales_RMB_Adjustment;
            worksheet.Cells["B11"].Value = closureTool.PAC_RMB_Adjustment;


            worksheet.Cells["B12"].Value = closureTool.Rent_RMB_Adjustment;



            worksheet.Cells["B13"].Value = closureTool.DepreciationLHI_RMB_Adjustment;


            worksheet.Cells["B14"].Value = closureTool.InterestLHI_RMB_Adjustment;





            //INTEREST LHI 
            // worksheet.Cells[15, 2].Value = closureTool..ToString();

            worksheet.Cells["B15"].Value = closureTool.ServiceFee_RMB_Adjustment;


            worksheet.Cells["B16"].Value = closureTool.Accounting_RMB_Adjustment;


            worksheet.Cells["B17"].Value = closureTool.Accounting_RMB_Adjustment;


            worksheet.Cells["B18"].Value = closureTool.TaxesLicenses_RMB_Adjustment;


            worksheet.Cells["B19"].Value = closureTool.Depreciation_ESSD_RMB_Adjustment;


            worksheet.Cells["B20"].Value = closureTool.Interest_ESSD_RMB_Adjustment;


            worksheet.Cells["B21"].Value = closureTool.OtherIncExp_RMB_Adjustment;

            worksheet.Cells["B22"].Value = closureTool.NonProduct_Sales_RMB_Adjustment;
            worksheet.Cells["B23"].Value = closureTool.NonProduct_Costs_RMB_Adjustment;
            worksheet.Cells["B24"].Value = closureTool.CompSalesMacket_Adjustment;
            worksheet.Cells["B25"].Value = closureTool.CompCG_Adjustment;
            worksheet.Cells["B26"].Value = closureTool.CompCGMacket_Adjustment;
            worksheet.Cells["B27"].Value = closureTool.PACMarket_Adjustment;
            worksheet.Cells["B28"].Value = closureTool.SOIMarket_Adjustment;
            worksheet.Cells["B29"].Value = closureTool.TotalSales_TTMY1;
            worksheet.Cells["B30"].Value = closureTool.CompSales_TTMY1;
            worksheet.Cells["B31"].Value = closureTool.CompSales_Market_TTMY1;
            worksheet.Cells["B32"].Value = closureTool.CompGC_TTMY1;
            worksheet.Cells["B33"].Value = closureTool.CompGCMarket_TTMY1;

            worksheet.Cells["B34"].Value = closureTool.PAC_TTMY1;
            worksheet.Cells["B35"].Value = closureTool.PACMarket_TTMY1;

            worksheet.Cells["B36"].Value = closureTool.SOI_TTMY1;
            worksheet.Cells["B37"].Value = closureTool.SOIMarket_TTMY1;

            worksheet.Cells["B38"].Value = closureTool.CashFlow_TTMY1;
            worksheet.Cells["B39"].Value = closureTool.TotalSales_TTMY2;
            //sheet.Cells[36, 2].Value = closureTool.cashflow.ToString(); 


            worksheet.Cells["B40"].Value = closureTool.CompSales_TTMY2;
            worksheet.Cells["B41"].Value = closureTool.CompSales_Market_TTMY2;

            worksheet.Cells["B42"].Value = closureTool.CompGC_TTMY2;
            worksheet.Cells["B43"].Value = closureTool.CompGCMarket_TTMY2;

            worksheet.Cells["B44"].Value = closureTool.PAC_TTMY2;
            worksheet.Cells["B45"].Value = closureTool.PACMarket_TTMY2;

            worksheet.Cells["B46"].Value = closureTool.SOI_TTMY2;
            worksheet.Cells["B47"].Value = closureTool.SOIMarket_TTMY2;

            worksheet.Cells["B48"].Value = closureTool.CashFlow_TTMY2;
            worksheet.Cells["B49"].Value = DateTime.Now.Year + " July";
            var woEntity = ClosureWOCheckList.Get(closureTool.ProjectId);

            if (woEntity != null)
            {


                worksheet.Cells["B50"].Value = woEntity.RE_Original;
                worksheet.Cells["B51"].Value = woEntity.LHI_Original;
                worksheet.Cells["B52"].Value = woEntity.ESSD_Original;
                worksheet.Cells["B53"].Value = woEntity.RE_NBV;
                worksheet.Cells["B54"].Value = woEntity.LHI_NBV;
                worksheet.Cells["B55"].Value = woEntity.ESSD_NBV;
                worksheet.Cells["B56"].Value = woEntity.EquipmentTransfer;
                worksheet.Cells["B57"].Value = woEntity.ClosingCost;
            }

            var list = ClosureToolImpactOtherStore.Search(e => e.ClosureId == closureTool.Id).AsNoTracking().ToList();
            if (list.Count > 0)
            {
                worksheet.Cells["B58"].Value = list[0].StoreCode;
                worksheet.Cells["B59"].Value = list[0].NameZHCN;
                worksheet.Cells["B60"].Value = list[0].ImpactSaltes;
            }

            if (list.Count > 1)
            {
                worksheet.Cells["B61"].Value = list[1].StoreCode;
                worksheet.Cells["B62"].Value = list[1].NameZHCN;
                worksheet.Cells["B63"].Value = list[1].ImpactSaltes;
            }



            worksheet.Cells["B64"].Value = closureTool.McppcoMargin;


            worksheet.Cells["B65"].Value = closureTool.MccpcoCashFlow;

            worksheet.Cells["B66"].Value = closureTool.Compensation;


            worksheet.Cells["B67"].Value = closureTool.CompAssumption;


            worksheet.Cells["B68"].Value = closureTool.CashflowGrowth;

            if (closureTool.IsOptionOffered.HasValue)
            {
                worksheet.Cells["B69"].Value = closureTool.IsOptionOffered.Value ? "Yes" : "No";
            }
            worksheet.Cells["B70"].Value = closureTool.LeaseTerm;


            worksheet.Cells["B71"].Value = closureTool.Investment;


            worksheet.Cells["B72"].Value = closureTool.NPVRestaurantCashflows;

            if (closureTool.Yr1SOI != null)
                worksheet.Cells["B73"].Value = closureTool.Yr1SOI;

            if (closureTool.Yr1SOI != null)
                worksheet.Cells["B74"].Value = closureTool.IRR;

            if (closureTool.ConclusionComment != null)
                worksheet.Cells["B76"].Value = closureTool.ConclusionComment;
        }
    }
}
