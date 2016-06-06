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
    public class ProductCategory
    {
        private static readonly MoyeBuy.Com.IDAL.IProductCagegory dal = DataAcess.CreateCategory();

        public IList<Model.ProductCategory> GetProductCategory(string strCategoryID)
        {
            return dal.GetProductCategory(strCategoryID);
        }

        public bool AddUpdateProductCatgory(Model.ProductCategory pcatgory)
        {
            return dal.AddUpdateCatgory(pcatgory);
        }

        public bool DelProductCatgory(string strCategoryId)
        {
            return dal.DelProductCatgory(strCategoryId);
        }
    }
}
