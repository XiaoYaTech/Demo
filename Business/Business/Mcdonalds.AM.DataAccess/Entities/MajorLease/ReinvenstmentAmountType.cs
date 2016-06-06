using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Common;
using System.Transactions;
using Mcdonalds.AM.DataAccess.Entities.Condition;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ReinvenstmentAmountType : BaseEntity<ReinvenstmentAmountType>
    {
        public List<ReinvenstmentAmountType> GetAmountTypeInfo()
        {
            return Search(e=>true).ToList();
        }
    }
}
