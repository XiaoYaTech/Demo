/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   11/21/2014 5:18:44 PM
 * FileName     :   TTMFinanceData
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class TTMFinanceData
    {
        public decimal Accounting { get; set; }
        public decimal DepreciationEssd { get; set; }
        public decimal DepreciationLhi { get; set; }
        public decimal NonProductCosts { get; set; }
        public decimal NonProductSales { get; set; }
        public decimal Insurance { get; set; }
        public decimal InterestEssd { get; set; }
        public decimal InterestLhi { get; set; }
        public decimal OtherIncExp { get; set; }
        public decimal Pac { get; set; }
        public decimal CompSales { get; set; }
        public decimal ProductSales { get; set; }
        public decimal Rent { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal TaxesAndLicenses { get; set; }

    }
}
