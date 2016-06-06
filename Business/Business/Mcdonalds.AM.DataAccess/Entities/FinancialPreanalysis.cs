using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class FinancialPreanalysis : BaseEntity<FinancialPreanalysis>
    {
        public string TotalReinvestmentNorm { get; set; }
        public void Save()
        {
            if (Id == 0)
            {
                Add(this);
            }
            else
            {
                Update(this);
            }
        }

        public void GetNewAddBE()
        {
            var newAddBEStrList = new List<string>();

            if (IsMcCafe.HasValue
                && IsMcCafe.Value)
            {
                newAddBEStrList.Add("McCafe");
            }

            if (IsKiosk.HasValue
    && IsKiosk.Value)
            {
                newAddBEStrList.Add("Kiosk");
            }

            if (IsMDS.HasValue
                && IsMDS.Value)
            {
                newAddBEStrList.Add("MDS");
            }

            if (IsTwientyFourHour.HasValue
                && IsTwientyFourHour.Value)
            {
                newAddBEStrList.Add("24H");
            }

            NewAddBE = string.Join(",", newAddBEStrList.ToArray());

        }
    }
}
