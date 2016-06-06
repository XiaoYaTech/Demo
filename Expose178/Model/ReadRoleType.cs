using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expose178.Com.Model
{
    [Serializable]
    public class ReadRoleType
    {
        public string ReadRoleTypeCode { get; set; }
        public string ReadRoleTypeDesc { get; set; }
        public string UpdatedByUserID { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
