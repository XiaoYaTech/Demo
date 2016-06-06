using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Configuration;
using System.Collections;
using System.Data;
using MoyeBuy.Com.Model;
using MoyeBuy.Com.BLL;
using MoyeBuy.Com.MoyeBuyUtility;
using MoyeBuy.Com.CacheDependencyFactory;

namespace MoyeBuy.Com.MoyeBuyComSite.Proxys
{
    public class ProductProxy
    {
        private static readonly int IntProdDuration = Convert.ToInt32(ConfigurationManager.AppSettings["ProductCacheDuration"]);
        private static readonly bool IsEnableCache = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableCache"]);
        public static IList<ProductInfo> GetProductByIDs(string strProdIDs)
        {
            IList<ProductInfo> listProd = null;
            MoyeBuy.Com.BLL.Product prod = new Product();
            try
            {
                if (IsEnableCache)
                {
                    if (System.Web.HttpRuntime.Cache[strProdIDs] == null)
                    {
                        listProd = prod.GetProduct(strProdIDs);
                        AggregateCacheDependency dependecy = DependencyFacade.GetAdsDependency();
                        if (listProd != null)
                            System.Web.HttpRuntime.Cache.Add(strProdIDs, listProd, dependecy, DateTime.Now.AddMilliseconds(IntProdDuration), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                }
                else
                    listProd = prod.GetProduct(strProdIDs);
            }
            catch (Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "MoyeBuyComSite.Proxys.ProductProxy.GetProductByIDs()", UtilityFactory.LogType.LogToFile);
            }
            return listProd;
        }
        public static IList<ProductInfo> GetProductByKeywords(string strFilterString, string strPageIndex, string strPageSize, string strSortField, bool IsASC)
        {
            IList<ProductInfo> listProd = null;
            MoyeBuy.Com.BLL.Product prod = new Product();
            try
            {
                listProd = prod.GetProduct(strFilterString, strPageIndex, strPageSize, strSortField, IsASC);
            }
            catch (Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "MoyeBuyComSite.Proxys.ProductProxy.GetProductByKeywords()", UtilityFactory.LogType.LogToFile);
            }
            return listProd;
        }
    }
}