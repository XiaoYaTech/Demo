using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class ShippingModeInfo
    {
        public string ShippingModeID { get; set; }
        public string ShippingModeDesc { get; set; }
        public string LastUpdatedDate { get; set; }
    }
}
