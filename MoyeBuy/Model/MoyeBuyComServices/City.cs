using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class City
    {
        public string ProvinceID { get; set; }
        public string CityID { get; set; }
        public string CityName { get; set; }
        public string ZipCode { get; set; }
        public IList<District> Districts { get; set; }
    }
}
