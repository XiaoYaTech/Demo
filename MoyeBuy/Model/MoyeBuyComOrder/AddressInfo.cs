using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class AddressInfo
    {
        public AddressInfo() { }

        public string Name { get; set; }//收货人姓名
        public string AddressLabel { get; set; }//比如公司，家庭
        public string Province { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string AddressDetail { get; set; }//详细地址比如什么什么路
        public string ZipCode { get; set; }
        public string MobilePhone { get; set; }
        public string TelPhone { get; set; }//固定电话
        public string UpdateByUserID { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
