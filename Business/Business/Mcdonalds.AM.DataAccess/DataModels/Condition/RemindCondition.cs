using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.DataModels.Condition
{
    public class RemindCondition : Remind
    {

        public string Title { get; set; }
        public string SenderAccount { get; set; }
        public string ReceiverAccount { get; set; }
        public string SenderZHCN { get; set; }
        public string SenderENUS { get; set; }
        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
