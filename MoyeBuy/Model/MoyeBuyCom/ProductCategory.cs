using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class ProductCategory
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string CategoryDesc { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
