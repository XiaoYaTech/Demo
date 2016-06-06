/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   12/9/2014 1:51:39 PM
 * FileName     :   RenewalAnalysisDTO
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects.Renewal
{
    public class RenewalAnalysisDTO
    {
        public RenewalAnalysis Entity { get; set; }
        public RenewalInfo Info { get; set; }
        public RenewalAnalysisStoreInfo StoreInfo { get; set; }
        public bool HasReinvenstment { get; set; }
        public bool Editable { get; set; }
        public bool Recallable { get; set; }
        public bool Savable { get; set; }
    }

    public class RenewalAnalysisStoreInfo
    {
        public string MinimarketPriority { get; set; }
        public string TADesrability { get; set; }
        public string SitePortfolioType { get; set; }
        public string SiteRerating { get; set; }
        public string LeasedArea { get; set; }
        public string OperationSize { get; set; }
        public string Floors { get; set; }
        public string Seats { get; set; }
        public string BEType { get; set; }
        public string LeaseTenureAndTerm { get; set; }
        public string FreeRentalPeriod { get; set; }
        public string RentStructure { get; set; }
        public decimal? DRMFLastTY { get; set; }
        public decimal? DRMFLastTYSales { get; set; }
        public decimal? SRMFLastTY { get; set; }
        public decimal? SRMFLastTYSales { get; set; }
        public decimal? SRMF1stTY { get; set; }
        public decimal? SRMF1stTYSales { get; set; }
        public decimal? AnnualSOIAvg { get; set; }
        public decimal? CashROIAvg { get; set; }
        public string OriginalInvestment { get; set; }
        public string NBV { get; set; }
        public string AdditionalInvestmentCost  { get; set; }
        public string ExclusivityClause  { get; set; }
        public string McDsEarlyTerminationRight { get; set; }
        public string LandlordEntity { get; set; }

        public string UsCode { get; set; }

        public string NameZHCN { get; set; }

        public string NameENUS { get; set; }
        public string LastRemodeling { get; set; }
        public decimal? AnnualSOILastTY { get; set; }
        public decimal? CashROILastTY { get; set; }
    }
}
