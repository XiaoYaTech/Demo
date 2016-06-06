using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class FinancialPreanalysisDto
    {
        public DataSync_LDW_AM_STFinanceData STFinanceData { get; set; }
        public decimal? TTMSales { get; set; }

        public string ROI { get; set; }
        public string CurrentPriceTier { get; set; }
        public int Id { get; set; }
        public string PaybackYears { get; set; }
    }
}

