/*===========================================================
 * Author       :   Kevin.Yao
 * CreateTime   :   14/10/2014 2:35:50 PM
 * FileName     :   ClosureLDW_FinanceData
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Kevin.Yao
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess.DataTransferObjects;

namespace Mcdonalds.AM.DataAccess
{
    public partial class LDW_FinanceData : BaseEntity<LDW_FinanceData>
    {
        /// <summary>
        /// Get
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static LDW_FinanceData Get(string projectId)
        {
            var fdList = Search(fd => fd.ProjectId == projectId).OrderByDescending(i => i.FinanceDataID).ToList();
            if (fdList.Count > 0)
                return fdList[0];
            return null;
        }

        /// <summary>
        /// Get
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static LDW_FinanceData GetByRefId(Guid refTableId)
        {
            return FirstOrDefault(fd => fd.RefTableId == refTableId);
        }

        /// <summary>
        /// Check
        /// </summary>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public static bool CheckIfInFinanceData(string projectId)
        {
            return Any(fd => fd.ProjectId == projectId);
        }

        /// <summary>
        /// Original Data Operation
        /// </summary>
        /// <param name="refTableId"></param>
        /// <param name="projectId"></param>
        /// <param name="financeYearMonth"> TTM Financial Data : Data Month</param>
        /// <param name="lastyear"></param>
        /// <param name="lastmonth"></param>
        public static void OriginalDataOperation(Guid refTableId, string projectId, string financeYearMonth, string lastyear, string lastmonth)
        {
            if (string.IsNullOrEmpty(projectId))
                return;

            string financeYear;
            string financeMonth;
            McdAMEntities amdb = new McdAMEntities();

            if (!string.IsNullOrEmpty(financeYearMonth))
            {
                financeYear = financeYearMonth.Split('-')[0];
                financeMonth = financeYearMonth.Split('-')[1];

                var ldw_financeData = GetByRefId(refTableId);
                if (ldw_financeData == null)
                    AddFinancialData(refTableId, projectId, financeYear, financeMonth);
                else if (ldw_financeData.FinanceYear != financeYear || ldw_financeData.FinanceMonth != financeMonth)
                    UpdateFinancialData(refTableId, projectId, financeYear, financeMonth);
            }
            else
            {
                var yearMonthObj = amdb.StoreSTMonthlyFinaceInfoTTM.FirstOrDefault();
                if (yearMonthObj != null && !string.IsNullOrEmpty(yearMonthObj.TTMValue))
                {
                    financeYear = yearMonthObj.TTMValue.Substring(0, yearMonthObj.TTMValue.IndexOf('-'));
                    financeMonth = yearMonthObj.TTMValue.Substring(yearMonthObj.TTMValue.IndexOf('-') + 1);
                }
                else
                {
                    financeYear = lastyear;
                    financeMonth = lastmonth;
                }
                if (!CheckIfInFinanceData(projectId))
                    AddFinancialData(refTableId, projectId, financeYear, financeMonth);
            }
        }

        /// <summary>
        /// Add LDW_FinanceData
        /// </summary>
        /// <param name="refTableId"></param>
        /// <param name="projectId"></param>
        /// <param name="financeYear"></param>
        /// <param name="financeMonth"></param>
        /// <returns></returns>
        private static int AddFinancialData(Guid refTableId, string projectId, string financeYear, string financeMonth)
        {
            McdAMEntities amdb = new McdAMEntities();

            var uscode = ClosureInfo.FirstOrDefault(ci => ci.ProjectId == projectId).USCode;
            var storeId = amdb.StoreBasicInfo.Where(s => s.StoreCode.Equals(uscode)).Select(id => id.StoreID).FirstOrDefault();
            var financeData = amdb.DataSync_LDW_AM_STFinanceData.Where(f => f.UsCode == uscode && f.FinanceYear.Equals(financeYear) && f.FinanceMonth.Equals(financeMonth)).FirstOrDefault();
            var financeData2 = amdb.DataSync_LDW_AM_STFinanceData2.Where(f => f.UsCode == uscode && f.FinanceYear.Equals(financeYear) && f.FinanceMonth.Equals(financeMonth)).FirstOrDefault();

            var ldw_FinanceData = new LDW_FinanceData();

            ldw_FinanceData.ProjectId = projectId;
            ldw_FinanceData.StoreID = storeId;
            ldw_FinanceData.UsCode = uscode;
            ldw_FinanceData.RefTableId = refTableId;
            ldw_FinanceData.FinanceYear = financeYear;
            ldw_FinanceData.FinanceMonth = financeMonth;
            ldw_FinanceData.Total_Sales_TTM = financeData.Total_Sales_TTM;
            ldw_FinanceData.Rent_TTM = financeData2.Rent_TTM;
            ldw_FinanceData.comp_sales_ttm = financeData2.comp_sales_ttm;
            ldw_FinanceData.comp_sales_market_ttm = financeData2.comp_sales_market_ttm;
            ldw_FinanceData.comp_gc_ttm = financeData2.comp_gc_ttm;
            ldw_FinanceData.comp_gc_market_ttm = financeData2.comp_gc_market_ttm;
            ldw_FinanceData.Pac_TTM = financeData2.Pac_TTM;
            ldw_FinanceData.PACPct_TTM = financeData2.PACPct_TTM;
            ldw_FinanceData.PACPct_MARKET_TTM = financeData2.PACPct_MARKET_TTM;
            ldw_FinanceData.Depreciation_LHI_TTM = financeData2.Depreciation_LHI_TTM;
            ldw_FinanceData.Interest_LHI_TTM = financeData2.Interest_LHI_TTM;
            ldw_FinanceData.Service_Fee_TTM = financeData2.Service_Fee_TTM;
            ldw_FinanceData.Insurance_TTM = financeData2.Insurance_TTM;
            ldw_FinanceData.Accounting_TTM = financeData2.Accounting_TTM;
            ldw_FinanceData.Taxes_Licenses_TTM = financeData2.Taxes_Licenses_TTM;
            ldw_FinanceData.Depreciation_Essd_TTM = financeData2.Depreciation_Essd_TTM;
            ldw_FinanceData.Interest_Essd_TTM = financeData2.Interest_Essd_TTM;
            ldw_FinanceData.Other_Exp_TTM = financeData2.Other_Exp_TTM;
            ldw_FinanceData.ProductSales_TTM = financeData.ProductSales_TTM;
            ldw_FinanceData.Non_Product_Sales_TTM = financeData2.Non_Product_Sales_TTM;
            ldw_FinanceData.Non_Product_Costs_TTM = financeData2.Non_Product_Costs_TTM;
            ldw_FinanceData.SOIPct_TTM = financeData.SOIPct_TTM;
            ldw_FinanceData.SOIPct_MARKET_TTM = financeData2.SOIPct_MARKET_TTM;
            ldw_FinanceData.CashFlow_TTM = financeData2.CASH_FLOW_TTM;
            ldw_FinanceData.Total_Sales_TTMPY1 = financeData2.Total_Sales_TTMPY1;
            ldw_FinanceData.Total_Sales_TTMPY2 = financeData2.Total_Sales_TTMPY2;
            ldw_FinanceData.comp_sales_ttm_py1 = financeData2.comp_sales_ttm_py1;
            ldw_FinanceData.comp_sales_ttm_py2 = financeData2.comp_sales_ttm_py2;
            ldw_FinanceData.comp_sales_market_ttm_py1 = financeData2.comp_sales_market_ttm_py1;
            ldw_FinanceData.comp_sales_market_ttm_py2 = financeData2.comp_sales_market_ttm_py2;
            ldw_FinanceData.comp_gc_ttm_py1 = financeData2.comp_gc_ttm_py1;
            ldw_FinanceData.comp_gc_ttm_py2 = financeData2.comp_gc_ttm_py2;
            ldw_FinanceData.comp_gc_market_ttm_py1 = financeData2.comp_gc_market_ttm_py1;
            ldw_FinanceData.comp_gc_market_ttm_py2 = financeData2.comp_gc_market_ttm_py2;
            ldw_FinanceData.PAC_TTMPreviousY1 = financeData2.PAC_TTMPreviousY1;
            ldw_FinanceData.PAC_TTMPreviousY2 = financeData2.PAC_TTMPreviousY2;
            ldw_FinanceData.PACPct_MARKET_TTMPreviousY1 = financeData2.PACPct_MARKET_TTMPreviousY1;
            ldw_FinanceData.PACPct_MARKET_TTMPreviousY2 = financeData2.PACPct_MARKET_TTMPreviousY2;
            ldw_FinanceData.SOI_TTMPreviousY1 = financeData2.SOI_TTMPreviousY1;
            ldw_FinanceData.SOI_TTMPreviousY2 = financeData2.SOI_TTMPreviousY2;
            ldw_FinanceData.SOIPct_MARKET_TTMPreviousY1 = financeData2.SOIPct_MARKET_TTMPreviousY1;
            ldw_FinanceData.SOIPct_MARKET_TTMPreviousY2 = financeData2.SOIPct_MARKET_TTMPreviousY2;
            ldw_FinanceData.Cash_Flow_TTMPreviousY1 = financeData2.Cash_Flow_TTMPreviousY1;
            ldw_FinanceData.Cash_Flow_TTMPreviousY2 = financeData2.Cash_Flow_TTMPreviousY2;

            return ldw_FinanceData.Add();
        }

        /// <summary>
        /// Update LDW_FinanceData
        /// </summary>
        /// <param name="refTableId"></param>
        /// <param name="projectId"></param>
        /// <param name="financeYear"></param>
        /// <param name="financeMonth"></param>
        /// <returns></returns>
        private static int UpdateFinancialData(Guid refTableId, string projectId, string financeYear, string financeMonth)
        {
            McdAMEntities amdb = new McdAMEntities();

            var uscode = ClosureInfo.FirstOrDefault(ci => ci.ProjectId == projectId).USCode;
            var storeId = amdb.StoreBasicInfo.Where(s => s.StoreCode.Equals(uscode)).Select(id => id.StoreID).FirstOrDefault();
            var financeData = amdb.DataSync_LDW_AM_STFinanceData.Where(f => f.UsCode == uscode && f.FinanceYear.Equals(financeYear) && f.FinanceMonth.Equals(financeMonth)).FirstOrDefault();
            var financeData2 = amdb.DataSync_LDW_AM_STFinanceData2.Where(f => f.UsCode == uscode && f.FinanceYear.Equals(financeYear) && f.FinanceMonth.Equals(financeMonth)).FirstOrDefault();

            var ldw_FinanceData = GetByRefId(refTableId);

            ldw_FinanceData.StoreID = storeId;
            ldw_FinanceData.UsCode = uscode;
            ldw_FinanceData.FinanceYear = financeYear;
            ldw_FinanceData.FinanceMonth = financeMonth;
            ldw_FinanceData.Total_Sales_TTM = financeData.Total_Sales_TTM;
            ldw_FinanceData.Rent_TTM = financeData2.Rent_TTM;
            ldw_FinanceData.comp_sales_ttm = financeData2.comp_sales_ttm;
            ldw_FinanceData.comp_sales_market_ttm = financeData2.comp_sales_market_ttm;
            ldw_FinanceData.comp_gc_ttm = financeData2.comp_gc_ttm;
            ldw_FinanceData.comp_gc_market_ttm = financeData2.comp_gc_market_ttm;
            ldw_FinanceData.Pac_TTM = financeData2.Pac_TTM;
            ldw_FinanceData.PACPct_TTM = financeData2.PACPct_TTM;
            ldw_FinanceData.PACPct_MARKET_TTM = financeData2.PACPct_MARKET_TTM;
            ldw_FinanceData.Depreciation_LHI_TTM = financeData2.Depreciation_LHI_TTM;
            ldw_FinanceData.Interest_LHI_TTM = financeData2.Interest_LHI_TTM;
            ldw_FinanceData.Service_Fee_TTM = financeData2.Service_Fee_TTM;
            ldw_FinanceData.Insurance_TTM = financeData2.Insurance_TTM;
            ldw_FinanceData.Accounting_TTM = financeData2.Accounting_TTM;
            ldw_FinanceData.Taxes_Licenses_TTM = financeData2.Taxes_Licenses_TTM;
            ldw_FinanceData.Depreciation_Essd_TTM = financeData2.Depreciation_Essd_TTM;
            ldw_FinanceData.Interest_Essd_TTM = financeData2.Interest_Essd_TTM;
            ldw_FinanceData.Other_Exp_TTM = financeData2.Other_Exp_TTM;
            ldw_FinanceData.ProductSales_TTM = financeData.ProductSales_TTM;
            ldw_FinanceData.Non_Product_Sales_TTM = financeData2.Non_Product_Sales_TTM;
            ldw_FinanceData.Non_Product_Costs_TTM = financeData2.Non_Product_Costs_TTM;
            ldw_FinanceData.SOIPct_TTM = financeData.SOIPct_TTM;
            ldw_FinanceData.SOIPct_MARKET_TTM = financeData2.SOIPct_MARKET_TTM;
            ldw_FinanceData.CashFlow_TTM = financeData2.CASH_FLOW_TTM;
            ldw_FinanceData.Total_Sales_TTMPY1 = financeData2.Total_Sales_TTMPY1;
            ldw_FinanceData.Total_Sales_TTMPY2 = financeData2.Total_Sales_TTMPY2;
            ldw_FinanceData.comp_sales_ttm_py1 = financeData2.comp_sales_ttm_py1;
            ldw_FinanceData.comp_sales_ttm_py2 = financeData2.comp_sales_ttm_py2;
            ldw_FinanceData.comp_sales_market_ttm_py1 = financeData2.comp_sales_market_ttm_py1;
            ldw_FinanceData.comp_sales_market_ttm_py2 = financeData2.comp_sales_market_ttm_py2;
            ldw_FinanceData.comp_gc_ttm_py1 = financeData2.comp_gc_ttm_py1;
            ldw_FinanceData.comp_gc_ttm_py2 = financeData2.comp_gc_ttm_py2;
            ldw_FinanceData.comp_gc_market_ttm_py1 = financeData2.comp_gc_market_ttm_py1;
            ldw_FinanceData.comp_gc_market_ttm_py2 = financeData2.comp_gc_market_ttm_py2;
            ldw_FinanceData.PAC_TTMPreviousY1 = financeData2.PAC_TTMPreviousY1;
            ldw_FinanceData.PAC_TTMPreviousY2 = financeData2.PAC_TTMPreviousY2;
            ldw_FinanceData.PACPct_MARKET_TTMPreviousY1 = financeData2.PACPct_MARKET_TTMPreviousY1;
            ldw_FinanceData.PACPct_MARKET_TTMPreviousY2 = financeData2.PACPct_MARKET_TTMPreviousY2;
            ldw_FinanceData.SOI_TTMPreviousY1 = financeData2.SOI_TTMPreviousY1;
            ldw_FinanceData.SOI_TTMPreviousY2 = financeData2.SOI_TTMPreviousY2;
            ldw_FinanceData.SOIPct_MARKET_TTMPreviousY1 = financeData2.SOIPct_MARKET_TTMPreviousY1;
            ldw_FinanceData.SOIPct_MARKET_TTMPreviousY2 = financeData2.SOIPct_MARKET_TTMPreviousY2;
            ldw_FinanceData.Cash_Flow_TTMPreviousY1 = financeData2.Cash_Flow_TTMPreviousY1;
            ldw_FinanceData.Cash_Flow_TTMPreviousY2 = financeData2.Cash_Flow_TTMPreviousY2;

            return ldw_FinanceData.Update();
        }
    }
}
