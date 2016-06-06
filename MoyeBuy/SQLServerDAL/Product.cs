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
    public class Product : DALBase, IDAL.IProduct
    {
        public IList<Model.ProductInfo> GetProductByCategory(string strCategory)
        {
            throw new NotImplementedException();
        }

        public IList<Model.ProductInfo> GetProduct(string strFilterString, string strPageIndex, string strPageSize, string strSortField, bool IsASC)
        {
            DataSet dsProd = GetProductBySearchKeyWords(strFilterString, strPageIndex, strPageSize, strSortField, IsASC);
            if (dsProd == null)
                return null;
            else
                return PopulateProduct(dsProd);
        }

        public IList<Model.ProductInfo> GetProduct(string strProductIDs)
        {
            DataSet dsProd = GetProductByProductIDs(strProductIDs);
            if (dsProd == null)
                return null;
            else
                return PopulateProduct(dsProd);
        }

        public bool AddProduct(IList<Model.ProductInfo> listProduct)
        {
            DataSet dsResult = null;
            Hashtable hshParam = new Hashtable();
            try
            {
                foreach (Model.ProductInfo product in listProduct)
                {
                    Gadget.Addparamater(ref hshParam, "ProductID", product.ProductId);
                    Gadget.Addparamater(ref hshParam, "CategoryID", product.CategoryId);
                    Gadget.Addparamater(ref hshParam, "SupplierID", product.SupplierID);
                    Gadget.Addparamater(ref hshParam, "ProductName", product.ProductName);
                    Gadget.Addparamater(ref hshParam, "ProductDesc", product.ProductDesc);
                    Gadget.Addparamater(ref hshParam, "ProductSpec", product.ProductSpec);
                    Gadget.Addparamater(ref hshParam, "ProductImgs", product.ProductImgs);
                    Gadget.Addparamater(ref hshParam, "MoyeBuyPrice", product.MoyeBuyPrice.ToString());
                    Gadget.Addparamater(ref hshParam, "MarketPrice", product.MarketPrice.ToString());
                    Gadget.Addparamater(ref hshParam, "ProductCount", product.ProductCount);
                    Gadget.Addparamater(ref hshParam, "IsSellHot", product.IsSellHot == true ? "1" : "0");
                    Gadget.Addparamater(ref hshParam, "IsOnSell", product.IsOnSell == true ? "1" : "0");
                    dsResult = dbOperator.ProcessData("usp_AddUpdateProductByProductID", hshParam, strDSN);
                }
            }
            catch (Exception ex)
            {
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "BLL.Product.AddUpdtProduct()", UtilityFactory.LogType.LogToFile);
                dsResult = null;
            }
            return Gadget.DatatSetIsNotNullOrEmpty(dsResult);
        }

        private DataSet GetProductByProductIDs(string strProductIDs)
        {
            DataSet dsResult = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "ProductIDs", strProductIDs);
            dsResult = dbOperator.ProcessData("usp_GetProductByProductIDKeyWords", hshParam, strDSN);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsResult))
                return dsResult;
            else
                return null;
        }

        private DataSet GetProductBySearchKeyWords(string strFilterString, string strPageIndex, string strPageSize, string strSortField, bool IsASC)
        {
            DataSet dsResult = null;
            Hashtable hshParam = new Hashtable();
            Gadget.Addparamater(ref hshParam, "FilterString", strFilterString);
            Gadget.Addparamater(ref hshParam, "SortField", strSortField);
            Gadget.Addparamater(ref hshParam, "PageIndex", strPageIndex);
            Gadget.Addparamater(ref hshParam, "PageSize", strPageSize);
            Gadget.Addparamater(ref hshParam, "IsAsc", IsASC == true ? "1" : "0");
            dsResult = dbOperator.ProcessData("usp_GetProductByProductIDKeyWords", hshParam, strDSN);
            if (Gadget.DatatSetIsNotNullOrEmpty(dsResult))
                return dsResult;
            else
                return null;
        }

        private IList<ProductInfo> PopulateProduct(DataSet dsProd)
        {
            IList<ProductInfo> listProdt = new List<ProductInfo>();
            IList<CommentInfo> listComment = new List<CommentInfo>();
            IList<SupplierInfo> listSupplier = new List<SupplierInfo>();
            foreach (DataRow dr in dsProd.Tables[0].Rows)
            {
                SupplierInfo suppier = new SupplierInfo();
                suppier.SupplierID = Gadget.GetDataRowStringValue(dr, "SupplierID");
                suppier.SupplierName = Gadget.GetDataRowStringValue(dr, "SupplierName");
                suppier.SupplierPersonName = Gadget.GetDataRowStringValue(dr, "SupplierPersonName");
                suppier.SupplierPhoneNo = Gadget.GetDataRowStringValue(dr, "SupplierPhoneNo");
                suppier.SupplierFax = Gadget.GetDataRowStringValue(dr, "SupplierFax");
                suppier.SupplierAddress = Gadget.GetDataRowStringValue(dr, "SupplierAddress");
                suppier.LastUpdatedDate = Gadget.GetDataRowDateTimeValue(dr, "LastUpdatedDate");

                Model.ProductCategory category = new Model.ProductCategory();
                category.CategoryId = Gadget.GetDataRowStringValue(dr, "CategoryID");
                category.CategoryName = Gadget.GetDataRowStringValue(dr, "CategoryName");
                category.CategoryDesc = Gadget.GetDataRowStringValue(dr, "CategoryDesc");
                category.LastUpdateDate = Gadget.GetDataRowDateTimeValue(dr, "LastUpdatedDate");

                ProductStore pstore = new ProductStore();
                pstore.ProductStoreID = Gadget.GetDataRowStringValue(dr, "ProductID");
                pstore.ProductID = Gadget.GetDataRowStringValue(dr, "ProductStoreID");
                pstore.ProductCount = Gadget.GetDataRowStringValue(dr, "ProductCount");
                pstore.Supplier = suppier;

                ProductInfo prodt = new ProductInfo();
                prodt.ProductId = Gadget.GetDataRowStringValue(dr, "ProductID");
                prodt.ProductName = Gadget.GetDataRowStringValue(dr, "ProductName");
                prodt.ProductDesc = Gadget.GetDataRowStringValue(dr, "ProductDesc");
                prodt.ProductSpec = Gadget.GetDataRowStringValue(dr, "ProductSpec");
                prodt.ProductImgs = Gadget.GetDataRowStringValue(dr, "ProductImgs");
                prodt.MoyeBuyPrice = Gadget.GetNullableDecimalValue(dr, "MoyeBuyPrice");
                prodt.MarketPrice = Gadget.GetNullableDecimalValue(dr, "MarketPrice");
                prodt.IsSellHot = Gadget.GetDataRowBoolValue(dr, "IsSellHot");
                prodt.IsOnSell = Gadget.GetDataRowBoolValue(dr, "IsOnSell");
                prodt.LastUpdatedDate = Gadget.GetDataRowDateTimeValue(dr, "LastUpdatedDate");
                prodt.PStore = pstore;
                prodt.Category = category;
                listProdt.Add(prodt);
            }
            return listProdt;
        }


        public bool DelProduct(string strProductID)
        {
            DataSet dsRsult = null;
            Hashtable hshParam = new Hashtable();
            try
            {
                Gadget.Addparamater(ref hshParam, "ProductID", strProductID);
                dsRsult = dbOperator.ProcessData("usp_DelProductByID", hshParam, strDSN);
            }
            catch (Exception ex)
            {
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "SQLServerDAL.Product.DelProduct()", UtilityFactory.LogType.LogToFile);
                dsRsult = null;
            }
            return Gadget.DatatSetIsNotNullOrEmpty(dsRsult);
        }
    }
}
