using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class ProductStore
    {
        public string ProductStoreID { get; set; }
        public string ProductID { get; set; }
        public string ProductCount { get; set; }
        public SupplierInfo Supplier { get; set; }
        public DateTime LastUpdatedDate { get; set; }
    }
}
