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
    public class ProductCategoryProxy
    {
        private static readonly int IntProdCategoryDuration = Convert.ToInt32(ConfigurationManager.AppSettings["ProductCategoryCacheDuration"]);
        private static readonly bool IsEnableCache = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableCache"]);
        public static IList<Model.ProductCategory> GetProductCategory(string strCategoryID)
        {
            IList<Model.ProductCategory> listCategory = null;
            MoyeBuy.Com.BLL.ProductCategory pCategory = new BLL.ProductCategory();
            string strCachCategory = strCategoryID;
            if (strCategoryID == "")
                strCachCategory = "category";
            try
            {
                if (IsEnableCache)
                {
                    if (System.Web.HttpRuntime.Cache[strCachCategory] == null)
                    {
                        listCategory = pCategory.GetProductCategory(strCategoryID);
                        AggregateCacheDependency dependecy = DependencyFacade.GetAdsDependency();
                        if (listCategory != null)
                            System.Web.HttpRuntime.Cache.Add(strCachCategory, listCategory, dependecy, DateTime.Now.AddMilliseconds(IntProdCategoryDuration), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                }
                else
                    listCategory = pCategory.GetProductCategory(strCategoryID);
            }
            catch (Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("UID", Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "MoyeBuyComSite.Proxys.ProductCategoryProxy.GetProductCategory()", UtilityFactory.LogType.LogToFile);
            }
            return listCategory;
        }
    }
}