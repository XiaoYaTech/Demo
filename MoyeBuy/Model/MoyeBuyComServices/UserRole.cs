using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class UserRole
    {
        public string UserRoleID { get; set; }
        public string MoyeBuyComUserID { get; set; }
        public string RoleID { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
