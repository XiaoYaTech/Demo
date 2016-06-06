using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Configuration;
using System.Data;
using MoyeBuy.Com.Model;
using MoyeBuy.Com.BLL;
using MoyeBuy.Com.MoyeBuyUtility;
using MoyeBuy.Com.CacheDependencyFactory;


namespace MoyeBuy.Com.MoyeBuyComSite.Proxys
{
    public class LayoutMenuProxy
    {
        private static readonly int IntMenuDuration = Convert.ToInt32(ConfigurationManager.AppSettings["MenuCacheDuration"]);
        private static readonly bool IsEnableCache = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableCache"]);
        private static IList<Menu> GetMenu(string strMenuType,bool IsAdminMenu)
        {
            IList<Menu> listMenu = null;
            LayoutMenu lymenu = new LayoutMenu();
            if (IsEnableCache)
            {
                if (System.Web.HttpRuntime.Cache["menu"] == null)
                {
                    listMenu = lymenu.GetMenuData(strMenuType, IsAdminMenu);
                    AggregateCacheDependency dependency = DependencyFacade.GetLayoutMenuDependency();
                    if (listMenu != null)
                        System.Web.HttpRuntime.Cache.Add("menu", listMenu, dependency, DateTime.Now.AddMinutes(IntMenuDuration), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
                }
                if (listMenu == null)
                    listMenu = (IList<Menu>)System.Web.HttpRuntime.Cache["menu"];
            }
            else
                listMenu = lymenu.GetMenuData(strMenuType, IsAdminMenu);
            return listMenu;
        }
        public static IList<Menu> GetMenu()
        {
            return GetMenu("ALL",false);
        }
        public static IList<Menu> GetAdminMenu()
        {
            return GetMenu("ALL", true);
        }
    }
}