using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class StoreProfitabilityAndLeaseInfo : BaseEntity<StoreProfitabilityAndLeaseInfo>
    {
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
    }
}
