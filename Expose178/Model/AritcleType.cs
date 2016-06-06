using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expose178.Com.Model
{
    [Serializable]
    public class AritcleType
    {
        public string AritcleTypeCode { get; set; }
        public string AritcleTypeDesc { get; set; }
        public string UpdatedByUserID { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
