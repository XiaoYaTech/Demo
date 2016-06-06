using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Configuration;
using System.Data;
using MoyeBuy.Com.Model;
using MoyeBuy.Com.MoyeBuyUtility;
using MoyeBuy.Com.CacheDependencyFactory;


namespace MoyeBuy.Com.MoyeBuyComSite.Proxys
{
    public class AdProxy
    {
        private static readonly int IntAdDuration = Convert.ToInt32(ConfigurationManager.AppSettings["AdCacheDuration"]);
        private static readonly bool IsEnableCache = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableCache"]);
        public static IList<Ad> GetAdvertisement()
        {
            IList<Ad> listAd = null;
            MoyeBuy.Com.BLL.Ad adv= new BLL.Ad();
            try
            {
                if (IsEnableCache)
                {
                    if (System.Web.HttpRuntime.Cache["ad"] == null)
                    {
                        listAd = adv.GetAds();
                        AggregateCacheDependency dependecy = DependencyFacade.GetAdsDependency();
                        if (listAd != null)
                            System.Web.HttpRuntime.Cache.Add("ad", listAd, dependecy, DateTime.Now.AddMinutes(IntAdDuration), System.Web.Caching.Cache.NoSlidingExpiration, CacheItemPriority.Normal, null);
                    }
                    if (listAd == null)
                        listAd = (IList<Ad>)System.Web.HttpRuntime.Cache["ad"];
                }
                else
                    listAd = adv.GetAds();
            }
            catch (Exception ex)
            { 
                
            }
            return listAd;
        }
    }
}