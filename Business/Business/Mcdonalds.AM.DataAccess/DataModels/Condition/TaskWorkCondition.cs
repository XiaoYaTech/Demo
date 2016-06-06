using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.DataModels.Condition
{
    public class TaskWorkCondition : TaskWork
    {
        public int PageIndex { get; set; }

        public int PageSize { get; set; }

        public string StoreNameZHCN { get; set; }

    }
}
