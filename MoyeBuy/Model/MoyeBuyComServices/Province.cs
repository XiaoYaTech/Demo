using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class Province
    {
        public string ProvinceID { get; set; }
        public string ProvinceName { get; set; }
        public IList<City> Citys { get; set; }
    }
}
