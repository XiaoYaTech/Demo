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
    public class Supplier
    {
        private static readonly MoyeBuy.Com.IDAL.ISupplier dal = DataAcess.CreateSupplier();

        public IList<Model.SupplierInfo> GetProductSupplier(string strSupplierID)
        {
            return dal.GetSupplier(strSupplierID);
        }

        public bool AddUpdateProductSupplier(Model.SupplierInfo supplier)
        {
            return dal.AddUpdateSupplier(supplier);
        }

        public bool DelProductSupplierByID(string strSupplierID)
        {
            return dal.DelSupplierByID(strSupplierID);
        }
    }
}
