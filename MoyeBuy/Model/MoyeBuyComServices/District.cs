using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.Model
{
    [Serializable]
    public class District
    {
        public string CityID { get; set; }
        public string DistrictID { get; set; }
        public string DistrictName { get; set; }
    }
}
