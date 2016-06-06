using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Configuration;
using System.Data;
using System.Collections;
using MoyeBuy.Com.Model;
using MoyeBuy.Com.BLL;
using MoyeBuy.Com.MoyeBuyUtility;
using MoyeBuy.Com.CacheDependencyFactory;

namespace MoyeBuy.Com.MoyeBuyComSite.Proxys
{
    public class SupplierProxy
    {
        private static readonly int IntProdCategoryDuration = Convert.ToInt32(ConfigurationManager.AppSettings["ProductCategoryCacheDuration"]);
        private static readonly bool IsEnableCache = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableCache"]);
        public static IList<Model.SupplierInfo> GetProductCategory(string strSupplierID)
        {
            IList<Model.SupplierInfo> listSupplier = null;
            MoyeBuy.Com.BLL.Supplier pSupplier = new BLL.Supplier();
            string strCachCategory = strSupplierID;
            if (strSupplierID == "")
                strCachCategory = "supplier";
            try
            {
                if (IsEnableCache)
                {
                    if (System.Web.HttpRuntime.Cache[strCachCategory] == null)
                    {
                        listSupplier = pSupplier.GetProductSupplier(strSupplierID);
                        AggregateCacheDependency dependecy = DependencyFacade.GetAdsDependency();
                        if (listSupplier != null)
                            System.Web.HttpRuntime.Cache.Add(strCachCategory, listSupplier, dependecy, DateTime.Now.AddMilliseconds(IntProdCategoryDuration), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                }
                else
                    listSupplier = pSupplier.GetProductSupplier(strSupplierID);
            }
            catch (Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "SupplierProxy.GetProductCategory()", UtilityFactory.LogType.LogToFile);
            }
            return listSupplier;
        }
    }
}