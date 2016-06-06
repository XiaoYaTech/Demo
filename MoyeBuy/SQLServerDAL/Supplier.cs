using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Data;
using MoyeBuy.Com.MoyeBuyUtility;
using MoyeBuy.Com.Model;

namespace MoyeBuy.Com.SQLServerDAL
{
    public class Supplier : DALBase, IDAL.ISupplier
    {
        public IList<Model.SupplierInfo> GetSupplier(string strSupplierID)
        {
            IList<Model.SupplierInfo> listSupplierInfo = null;
            try
            {
                string strSupplierName = "";
                DataSet dsSupplierInfo = GetDataSetSupplier(strSupplierID, strSupplierName);
                if (Gadget.DatatSetIsNotNullOrEmpty(dsSupplierInfo))
                {
                    listSupplierInfo = new List<Model.SupplierInfo>();
                    foreach (DataRow dr in dsSupplierInfo.Tables[0].Rows)
                    {
                        MoyeBuy.Com.Model.SupplierInfo supplier = new Model.SupplierInfo();
                        supplier.SupplierID = Gadget.GetDataRowStringValue(dr, "SupplierID");
                        supplier.SupplierName = Gadget.GetDataRowStringValue(dr, "SupplierName");
                        supplier.SupplierAddress = Gadget.GetDataRowStringValue(dr, "SupplierAddress");
                        supplier.SupplierFax = Gadget.GetDataRowStringValue(dr, "SupplierFax");
                        supplier.SupplierPersonName = Gadget.GetDataRowStringValue(dr, "SupplierPersonName");
                        supplier.SupplierPhoneNo = Gadget.GetDataRowStringValue(dr, "SupplierPhoneNo");
                        supplier.LastUpdatedDate = Gadget.GetDataRowDateTimeValue(dr, "LastUpdatedDate");
                        listSupplierInfo.Add(supplier);
                    }
                }
            }
            catch (Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "SQLServerDAL.Supplier.GetProductCategory()", UtilityFactory.LogType.LogToFile);
            }
            return listSupplierInfo;
        }

        public bool AddUpdateSupplier(Model.SupplierInfo supplier)
        {
            DataSet dsResult = null;
            Hashtable hshParam = new Hashtable();
            try
            {
                Gadget.Addparamater(ref hshParam, "SupplierID", supplier.SupplierID);
                Gadget.Addparamater(ref hshParam, "SupplierName", supplier.SupplierName);
                Gadget.Addparamater(ref hshParam, "SupplierPersonName", supplier.SupplierPersonName);
                Gadget.Addparamater(ref hshParam, "SupplierAddress", supplier.SupplierAddress);
                Gadget.Addparamater(ref hshParam, "SupplierFax", supplier.SupplierFax);
                Gadget.Addparamater(ref hshParam, "SupplierPhoneNo", supplier.SupplierPhoneNo);
                dsResult = dbOperator.ProcessData("usp_AddUpdateSupplier", hshParam, strDSN);
            }
            catch (Exception ex)
            {
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "SQLServerDAL.Supplier.AddUpdateProductSupplier()", UtilityFactory.LogType.LogToFile);
            }
            return Gadget.DatatSetIsNotNullOrEmpty(dsResult);
        }

        public bool DelSupplierByID(string strSupplierID)
        {
            DataSet dsResult = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "SupplierID", strSupplierID);
            dsResult = dbOperator.ProcessData("usp_DelProductSupplier", hshParam, strDSN);
            return Gadget.DatatSetIsNotNullOrEmpty(dsResult);
        }

        private DataSet GetDataSetSupplier(string strSupplierID, string strSupplierName)
        {
            DataSet dsResult = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "SupplierID", strSupplierID);
            Gadget.Addparamater(ref hshParam, "SupplierName", strSupplierName);
            dsResult = dbOperator.ProcessData("usp_GetSupplierBySupplierIDName", hshParam, strDSN);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsResult))
                return dsResult;
            else
                return null;
        }
    }
}
