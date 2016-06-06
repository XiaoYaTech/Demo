using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class PostStoreContractModel
    {
        public StoreContractInfo Contract { get; set; }
        public List<StoreContractRevision> Revisions { get; set; }
    }
}
