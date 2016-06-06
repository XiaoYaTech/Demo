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
    public class Product
    {
        private static readonly MoyeBuy.Com.IDAL.IProduct dal = DataAcess.CreateProduct();

        public IList<ProductInfo> GetProduct(string strProductIDs)
        {
            return dal.GetProduct(strProductIDs);
        }
        public IList<ProductInfo> GetProduct(string strFilterString, string strPageIndex,string strPageSize, string strSortField, bool IsASC)
        {
            return dal.GetProduct(strFilterString, strPageIndex, strPageSize, strSortField, IsASC);
        }

        public bool AddUpdtProduct(IList<ProductInfo> listProduct)
        {
            return dal.AddProduct(listProduct);
        }

        public IList<Model.ProductInfo> GetProductByCategory(string strCategory)
        {
            return dal.GetProductByCategory(strCategory);
        }

        public bool DelProduct(string strProductID)
        {
            if (!string.IsNullOrEmpty(strProductID))
                return dal.DelProduct(strProductID);
            else
                return false;
        }
    }

}
