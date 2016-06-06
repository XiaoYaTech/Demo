using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Configuration;
using System.Data;
using System.Collections;
using Expose178.Com.Model;

namespace Expose178.Com.Expose178Site.Proxys
{
    public class ProxyUser
    {
        private static readonly int IntArticleDuration = Convert.ToInt32(ConfigurationManager.AppSettings["UserCacheDuration"]);
        private static readonly bool IsEnableCache = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableUserCache"]);
        public static Expose178.Com.Model.User GetUser(string strUID)
        {
            Model.User mUser = null;
            try
            {
                BLL.User bUser = new BLL.User();
                if (IsEnableCache)
                {
                    mUser = bUser.GetUserByUID(strUID);
                    AggregateCacheDependency dependency = Expose178.Com.CacheDependencyFactory.DependencyFacade.GetArticleDependency();
                    if (mUser != null)
                        System.Web.HttpRuntime.Cache.Add("user", mUser, dependency, DateTime.Now.AddSeconds(IntArticleDuration), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
                }
                else
                    mUser = bUser.GetUserByUID(strUID);
            }
            catch (Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("Error", ex.Message);
                hshParam.Add("UID", Expose178.Com.GadgetScripts.Gadget.GetUserID());
                Expose178.Com.UtilityFactory.Log.WriteLog(hshParam, "ProxyUser.GetUser", UtilityFactory.LogType.LogToFile);
            }
            return mUser;
        }
    }
}