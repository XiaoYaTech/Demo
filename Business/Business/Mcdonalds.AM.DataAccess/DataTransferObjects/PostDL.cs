using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.DataTransferObjects
{
    public class PostDL
    {
        public class ClosureDL
        {
            public V_AM_DL_Closure Entity { get; set; }
            public bool PushOrNot { get; set; }
        }

        public class ReimageDL
        {
            public V_AM_DL_Reimage Entity { get; set; }
            public bool PushOrNot { get; set; }
        }

        public class MajorLeaseDL
        {
            public V_AM_DL_MajorLeaseChange Entity { get; set; }
            public bool PushOrNot { get; set; }
        }

        public class RebuildDL
        {
            public V_AM_DL_Rebuild Entity { get; set; }
            public bool PushOrNot { get; set; }
        }

        public class RenewalDL
        {
            public V_AM_DL_Renewal Entity { get; set; }
            public bool PushOrNot { get; set; }
        }
    }
}
