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
    public class ProductCagegory : DALBase, IDAL.IProductCagegory
    {
        public IList<Model.ProductCategory> GetProductCategory(string strCategoryID)
        {
            IList<Model.ProductCategory> listCategory = null;
            try
            {
                listCategory = new List<Model.ProductCategory>();
                DataSet dsCategory = GetDataSetCategory(strCategoryID);
                if (Gadget.DatatSetIsNotNullOrEmpty(dsCategory))
                {
                    foreach (DataRow dr in dsCategory.Tables[0].Rows)
                    {
                        MoyeBuy.Com.Model.ProductCategory pcategory = new Model.ProductCategory();
                        pcategory.CategoryId = Gadget.GetDataRowStringValue(dr, "CategoryId");
                        pcategory.CategoryName = Gadget.GetDataRowStringValue(dr, "CategoryName");
                        pcategory.CategoryDesc = Gadget.GetDataRowStringValue(dr, "CategoryDesc");
                        pcategory.LastUpdateDate = Gadget.GetDataRowDateTimeValue(dr, "LastUpdateDate");
                        listCategory.Add(pcategory);
                    }
                }
            }
            catch (Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "SQLServerDAL.ProductCategory.GetProductCategory()", UtilityFactory.LogType.LogToFile);
            }
            return listCategory;
        }

        public bool AddUpdateCatgory(Model.ProductCategory pcatgory)
        {
            DataSet dsResult = null;
            Hashtable hshParam = new Hashtable();
            try
            {
                Gadget.Addparamater(ref hshParam, "CategoryID", pcatgory.CategoryId);
                Gadget.Addparamater(ref hshParam, "CategoryName", pcatgory.CategoryName);
                Gadget.Addparamater(ref hshParam, "CategoryDesc", pcatgory.CategoryDesc);
                dsResult = dbOperator.ProcessData("usp_AddUpdateProductCategory", hshParam, strDSN);
            }
            catch (Exception ex)
            {
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "SQLServerDAL.ProductCategory.AddUpdateProductCatgory()", UtilityFactory.LogType.LogToFile);
            }
            return Gadget.DatatSetIsNotNullOrEmpty(dsResult);
        }

        public bool DelProductCatgory(string strCategoryId)
        {
            DataSet dsResult = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "CategoryID", strCategoryId);
            dsResult = dbOperator.ProcessData("usp_DelProductCategoryByID", hshParam, strDSN);
            return Gadget.DatatSetIsNotNullOrEmpty(dsResult);
        }

        private DataSet GetDataSetCategory(string strCategoryID)
        {
            DataSet dsResult = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "CategoryID", strCategoryID);
            dsResult = dbOperator.ProcessData("usp_GetProductCategory", hshParam, strDSN);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsResult))
                return dsResult;
            else
                return null;
        }
    }
}
