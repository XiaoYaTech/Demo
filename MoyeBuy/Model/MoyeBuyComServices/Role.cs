using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class Role
    {
        public string RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleDesc { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
