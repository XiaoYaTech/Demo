using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.Common;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   10/16/2014 2:32:45 PM
 * FileName     :   RenewalToolFinMeasureInput
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace Mcdonalds.AM.DataAccess
{
    public partial class RenewalToolFinMeasureInput : BaseEntity<RenewalToolFinMeasureInput>
    {
        public string FinanceDataYearMonth { get; set; }
        public static RenewalToolFinMeasureInput Get(string projectId, Guid toolId)
        {
            var input = FirstOrDefault(i => i.ToolId == toolId);
            if (input == null)
            {
                input = new RenewalToolFinMeasureInput();
                input.Id = Guid.NewGuid();
                input.ToolId = toolId;
            }
            return input;
        }

        public static TTMFinanceData GetFinanceData(string projectId, string financeYear = "", string financeMonth = "")
        {
            var ldw_FinanceData = LDW_FinanceData.Get(projectId);
            var ttmData = new TTMFinanceData();
            if (string.IsNullOrEmpty(financeYear) && string.IsNullOrEmpty(financeMonth) && ldw_FinanceData != null)
            {
                ttmData.Accounting = DataConverter.ToDecimal(ldw_FinanceData.Accounting_TTM);
                ttmData.DepreciationEssd = DataConverter.ToDecimal(ldw_FinanceData.Depreciation_Essd_TTM);
                ttmData.DepreciationLhi = DataConverter.ToDecimal(ldw_FinanceData.Depreciation_LHI_TTM);
                ttmData.NonProductCosts = DataConverter.ToDecimal(ldw_FinanceData.Non_Product_Costs_TTM);
                ttmData.NonProductSales = DataConverter.ToDecimal(ldw_FinanceData.Non_Product_Sales_TTM);
                ttmData.Insurance = DataConverter.ToDecimal(ldw_FinanceData.Insurance_TTM);

                ttmData.InterestEssd = DataConverter.ToDecimal(ldw_FinanceData.Interest_Essd_TTM);
                ttmData.InterestLhi = DataConverter.ToDecimal(ldw_FinanceData.Interest_LHI_TTM);
                ttmData.OtherIncExp = DataConverter.ToDecimal(ldw_FinanceData.Other_Exp_TTM);
                ttmData.Pac = DataConverter.ToDecimal(ldw_FinanceData.Pac_TTM);
                ttmData.ProductSales = DataConverter.ToDecimal(ldw_FinanceData.ProductSales_TTM);
                ttmData.Rent = DataConverter.ToDecimal(ldw_FinanceData.Rent_TTM);
                ttmData.ServiceFee = DataConverter.ToDecimal(ldw_FinanceData.Service_Fee_TTM);
                ttmData.TaxesAndLicenses = DataConverter.ToDecimal(ldw_FinanceData.Taxes_Licenses_TTM);
                ttmData.CompSales = DataConverter.ToDecimal(ldw_FinanceData.comp_sales_ttm);
            }
            else
            {
                var yearMonthObj = StoreSTMonthlyFinaceInfoTTM.FirstOrDefault(f => true);
                if (string.IsNullOrEmpty(financeYear))
                {
                    if (yearMonthObj != null && !string.IsNullOrEmpty(yearMonthObj.TTMValue))
                    {
                        financeYear = yearMonthObj.TTMValue.Substring(0, yearMonthObj.TTMValue.IndexOf('-'));
                    }
                    else
                    {
                        financeYear = Utils.GetLatestYear();
                    }
                }
                if (string.IsNullOrEmpty(financeMonth))
                {
                    if (yearMonthObj != null && !string.IsNullOrEmpty(yearMonthObj.TTMValue))
                    {
                        financeMonth = yearMonthObj.TTMValue.Substring(yearMonthObj.TTMValue.IndexOf('-') + 1);
                    }
                    else
                    {
                        financeMonth = Utils.GetLatestMonth();
                    }
                }
                var uscode = RenewalInfo.Get(projectId).USCode;
                var storeId = StoreBasicInfo.Search(s => s.StoreCode.Equals(uscode)).Select(id => id.StoreID).FirstOrDefault();
                var financeData = DataSync_LDW_AM_STFinanceData.FirstOrDefault(f => f.UsCode == uscode && f.FinanceYear.Equals(financeYear) && f.FinanceMonth.Equals(financeMonth));
                var financeData2 = DataSync_LDW_AM_STFinanceData2.FirstOrDefault(f => f.UsCode == uscode && f.FinanceYear.Equals(financeYear) && f.FinanceMonth.Equals(financeMonth));
                var re = DataSync_LDW_AM_STMonthlyFinaceInfo.Search(f => f.StoreID == storeId).OrderByDescending(f => f.Year).FirstOrDefault();

                ttmData.Accounting = DataConverter.ToDecimal(financeData2.Accounting_TTM);
                ttmData.DepreciationEssd = DataConverter.ToDecimal(financeData2.Depreciation_Essd_TTM);
                ttmData.DepreciationLhi = DataConverter.ToDecimal(financeData2.Depreciation_LHI_TTM);
                ttmData.NonProductCosts = DataConverter.ToDecimal(financeData2.Non_Product_Costs_TTM);
                ttmData.NonProductSales = DataConverter.ToDecimal(financeData2.Non_Product_Sales_TTM);
                ttmData.Insurance = DataConverter.ToDecimal(financeData2.Insurance_TTM);
                ttmData.InterestEssd = DataConverter.ToDecimal(financeData2.Interest_Essd_TTM);
                ttmData.InterestLhi = DataConverter.ToDecimal(financeData2.Interest_LHI_TTM);
                ttmData.OtherIncExp = DataConverter.ToDecimal(financeData2.Other_Exp_TTM);
                ttmData.Pac = DataConverter.ToDecimal(financeData2.Pac_TTM);
                ttmData.ProductSales = DataConverter.ToDecimal(financeData.ProductSales_TTM);
                ttmData.Rent = DataConverter.ToDecimal(financeData2.Rent_TTM);
                ttmData.ServiceFee = DataConverter.ToDecimal(financeData2.Service_Fee_TTM);
                ttmData.TaxesAndLicenses = DataConverter.ToDecimal(financeData2.Taxes_Licenses_TTM);
                ttmData.CompSales = DataConverter.ToDecimal(financeData2.comp_sales_ttm);
            }
            return ttmData;
        }

        public static List<string> GetYearMonths(string projectId,out string SelectedYearMonth)
        {
            var uscode = RenewalInfo.FirstOrDefault(ci => ci.ProjectId == projectId).USCode;
            var yearMonthList = DataSync_LDW_AM_STFinanceData2.Search(f => f.UsCode == uscode).Select(i => new { YearMonth = i.FinanceYear + "-" + i.FinanceMonth }).Distinct().OrderByDescending(i => i).Take(12).ToList().Select(i => { return i.YearMonth; }).ToList();
            var ldw_financeData = LDW_FinanceData.Get(projectId);
            if (ldw_financeData != null)
            {
                SelectedYearMonth = ldw_financeData.FinanceYear + "-" + ldw_financeData.FinanceMonth;
            }
            else
            {
                SelectedYearMonth = yearMonthList.FirstOrDefault();
            }
            return yearMonthList;
        }

        public void Save()
        {
            DataRecordTime = DateTime.Now;
            if (Any(f => f.Id == this.Id))
            {
                this.Update();
            }
            else
            {
                if (this.Id == Guid.Empty)
                {
                    this.Id = Guid.NewGuid();
                }
                this.DataRecordTime = DateTime.Now;
                this.Add();
            }
        }
    }
}
