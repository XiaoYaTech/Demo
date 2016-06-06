using System;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using MoyeBuy.Com.IDAL;

namespace MoyeBuy.Com.DALFactory
{
    public sealed class DataAcess
    {
        private static readonly string strAssemblyPath = ConfigurationManager.AppSettings["WebDAL"];
        public static IDBBaseOperator CreateDBBaseOperator()
        {
            string strCalssname = strAssemblyPath + ".DBBaseOperator";
            return (IDBBaseOperator)Assembly.Load(strAssemblyPath).CreateInstance(strCalssname);
        }
        public static IProductCagegory CreateCategory()
        {
            string strCalssname = strAssemblyPath + ".ProductCagegory";
            return (IProductCagegory)Assembly.Load(strAssemblyPath).CreateInstance(strCalssname);
        }
        public static IAds CreateAds()
        {
            string strCalssname = strAssemblyPath + ".Ads";
            return (IAds)Assembly.Load(strAssemblyPath).CreateInstance(strCalssname);
        }
        public static IGiftCard CreateGiftCard()
        {
            string strCalssname = strAssemblyPath + ".GiftCard";
            return (IGiftCard)Assembly.Load(strAssemblyPath).CreateInstance(strCalssname);
        }
        public static ILayoutMenu CreateLayoutMenu()
        {
            string strCalssname = strAssemblyPath + ".LayoutMenu";
            return (ILayoutMenu)Assembly.Load(strAssemblyPath).CreateInstance(strCalssname);
        }
        public static IProduct CreateProduct()
        {
            string strCalssname = strAssemblyPath + ".Product";
            return (IProduct)Assembly.Load(strAssemblyPath).CreateInstance(strCalssname);
        }
        public static ISupplier CreateSupplier()
        {
            string strCalssname = strAssemblyPath + ".Supplier";
            return (ISupplier)Assembly.Load(strAssemblyPath).CreateInstance(strCalssname);
        }
        public static IAccount CreateUser()
        {
            string strCalssname = strAssemblyPath + ".Account";
            return (IAccount)Assembly.Load(strAssemblyPath).CreateInstance(strCalssname);
        }
        public static IRole CreateRole()
        {
            string strCalssname = strAssemblyPath + ".Role";
            return (IRole)Assembly.Load(strAssemblyPath).CreateInstance(strCalssname);
        }
        public static IRegion CreateRegion()
        {
            string strCalssname = strAssemblyPath + ".Region";
            return (IRegion)Assembly.Load(strAssemblyPath).CreateInstance(strCalssname);
        }
    }
}
