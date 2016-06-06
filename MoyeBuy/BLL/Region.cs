using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections;
using MoyeBuy.Com.IDAL;
using MoyeBuy.Com.Model;
using MoyeBuy.Com.DALFactory;
using MoyeBuy.Com.MoyeBuyUtility;

namespace MoyeBuy.Com.BLL
{
    public class Region
    {
        private static readonly IDAL.IRegion dal = DALFactory.DataAcess.CreateRegion();
        public IList<Model.Province> GetProvinceByAll()
        {
            return dal.GetProvince("", "");
        }
        public IList<Model.City> GetCityByProvinceID(string strProvinceID)
        {
            return dal.GetCity(strProvinceID, "", "", "");
        }
        public IList<Model.District> GetDistrictByCityID(string strCityID)
        {
            return dal.GetDistrict("", "", strCityID);
        }
    }
}
