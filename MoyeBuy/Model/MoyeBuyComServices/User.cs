using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class User
    {
        public string MoyeBuyComUserID { get; set; }
        public string MoyeBuyComUserName { get; set; }
        public string UserPhoneNo { get; set; }
        public string MoyeBuyComEmail { get; set; }
        public string MoyeBuyComPwdSalt { get; set; }
        public string MoyeBuyComPwdHash { get; set; }
        public string AddressID { get; set; }
        public string RoleID { get; set; }
        public string RoleName { get; set; }
        public string RoleDesc { get; set; }
        public bool IsEffective { get; set; }
        public bool IsNeedChangePwd { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public decimal Amount { get; set; }
        public decimal GatherGrade { get; set; }
    }
}
