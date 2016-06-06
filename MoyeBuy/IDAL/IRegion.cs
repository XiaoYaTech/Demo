using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MoyeBuy.Com.IDAL
{
    public interface IRegion
    {
        IList<Model.Province> GetProvince(string strProvinceID,string strProvinceName);
        IList<Model.City> GetCity(string strProvinceID,string strCityID, string strCityName,string strZipCode);
        IList<Model.District> GetDistrict(string strDistrictID, string strDistrictName,string CityID);
    }
}
