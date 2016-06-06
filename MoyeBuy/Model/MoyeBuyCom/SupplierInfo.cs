using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class SupplierInfo
    {
        public string SupplierID { get; set; }
        public string SupplierName { get; set; }
        public string SupplierPersonName { get; set; }
        public string SupplierPhoneNo { get; set; }
        public string SupplierFax { get; set; }
        public string SupplierAddress { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
