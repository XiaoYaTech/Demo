using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Expose178.Com.Model
{
    [Serializable]
    public class User
    {
        public string UserID { get; set; }
        public string UserEmail { get; set; }
        public string UserName { get; set; }
        public string PwdSalt { get; set; }
        public string PwdHash { get; set; }
        public bool IsNeedChangePwd { get; set; }
        public bool IsEffective { get; set; }
        public bool IsTempUser { get;set; }
        public string IP { get; set; }
        public string Browser { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
