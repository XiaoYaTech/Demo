using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using MoyeBuy.Com.MoyeBuyUtility;

namespace MoyeBuy.Com.SQLServerDAL
{
    public class Region:DALBase,IDAL.IRegion
    {
        public IList<Model.Province> GetProvince(string strProvinceID, string strProvinceName)
        {
            IList<Model.Province> listProv = new List<Model.Province>();
            DataSet ds = GetProvinceData(strProvinceID, strProvinceName);
            if (Gadget.DatatSetIsNotNullOrEmpty(ds))
            { 
                foreach(DataRow dr in ds.Tables[0].Rows)
                {
                    Model.Province pro = new Model.Province();
                    pro.ProvinceID = Gadget.GetDataRowStringValue(dr, "ProvinceID");
                    pro.ProvinceName = Gadget.GetDataRowStringValue(dr, "ProvinceName");
                    listProv.Add(pro);
                }
            }
            return listProv;
        }

        public IList<Model.City> GetCity(string strProvinceID, string strCityID, string strCityName, string strZipCode)
        {
            IList<Model.City> listItem = new List<Model.City>();
            DataSet ds = GetCityData(strProvinceID, strCityID, strCityName, strZipCode);
            if (Gadget.DatatSetIsNotNullOrEmpty(ds))
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Model.City item = new Model.City();
                    item.CityID = Gadget.GetDataRowStringValue(dr, "CityID");
                    item.CityName = Gadget.GetDataRowStringValue(dr, "CityName");
                    item.ProvinceID = Gadget.GetDataRowStringValue(dr, "ProvinceID");
                    item.ZipCode = Gadget.GetDataRowStringValue(dr, "ZipCode");
                    listItem.Add(item);
                }
            }
            return listItem;
        }

        public IList<Model.District> GetDistrict(string strDistrictID, string strDistrictName, string strCityID)
        {
            IList<Model.District> listItem = new List<Model.District>();
            DataSet ds = GetDistrictData(strDistrictID, strDistrictName, strCityID);
            if (Gadget.DatatSetIsNotNullOrEmpty(ds))
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    Model.District item = new Model.District();
                    item.CityID = Gadget.GetDataRowStringValue(dr, "CityID");
                    item.DistrictID = Gadget.GetDataRowStringValue(dr, "DistrictID");
                    item.DistrictName = Gadget.GetDataRowStringValue(dr, "DistrictName");
                    listItem.Add(item);
                }
            }
            return listItem;
        }

        private DataSet GetProvinceData(string strProvinceID, string strProvinceName)
        {
            DataSet dsResult = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "ProvinceName", strProvinceName);
            Gadget.Addparamater(ref hshParam, "ProvinceID", strProvinceID);
            dsResult = dbOperator.ProcessData("usp_GetProvince", hshParam, strServiceDSN);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsResult))
                return dsResult;
            else
                return null;
        }
        private DataSet GetCityData(string strProvinceID, string strCityID, string strCityName, string strZipCode)
        {
            DataSet dsResult = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "CityName", strCityName);
            Gadget.Addparamater(ref hshParam, "CityID", strCityID);
            Gadget.Addparamater(ref hshParam, "ProvinceID", strProvinceID);
            Gadget.Addparamater(ref hshParam, "ZipCode", strZipCode);
            dsResult = dbOperator.ProcessData("usp_GetCity", hshParam, strServiceDSN);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsResult))
                return dsResult;
            else
                return null;
        }
        private DataSet GetDistrictData(string strDistrictID, string strDistrictName, string strCityID)
        {
            DataSet dsResult = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "DistrictID", strDistrictID);
            Gadget.Addparamater(ref hshParam, "CityID", strCityID);
            Gadget.Addparamater(ref hshParam, "DistrictName", strDistrictName);
            dsResult = dbOperator.ProcessData("usp_District", hshParam, strServiceDSN);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsResult))
                return dsResult;
            else
                return null;
        }
    }
}
