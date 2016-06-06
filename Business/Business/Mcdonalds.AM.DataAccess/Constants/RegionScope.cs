using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public class RegionScope
    {
        public const string MCCL = "MCCL";
        public const string Region = "Region";
        public const string Market = "Market";
        public const string City = "City";
    }

    public enum RegionScopeType : int
    {
        MCCL = 100,
        Region = 102,
        Market = 103,
        City = 104
    }
}
