using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class OrderStateInfo
    {
        public string OrderStateID { get; set; }
        public string OrderStateDesc { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
