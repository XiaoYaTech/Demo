using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class SystemLog
    {
        public string SystemLogID { get; set; }
        public string SystemLogMsg { get; set; }
        public string UpdatedByUserID { get; set; }
        public string LastUpdatedDate { get; set; }
    }
}
