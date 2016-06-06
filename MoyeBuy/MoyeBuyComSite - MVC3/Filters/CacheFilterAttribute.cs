using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoyeBuy.Com.MoyeBuyComSite.Filters
{
    /// <summary>
    /// 缓存过虑器(不经常更新的页面添加)
    /// </summary>
    public class CacheFilterAttribute : ActionFilterAttribute
    {
        public int Duration { get; set; }

        public CacheFilterAttribute()
        {
            Duration = 10;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (Duration <= 0) return;

            HttpCachePolicyBase cache = filterContext.HttpContext.Response.Cache;
            TimeSpan cacheDuration = TimeSpan.FromSeconds(Duration);

            cache.SetCacheability(HttpCacheability.Public);
            cache.SetExpires(DateTime.Now.Add(cacheDuration));
            cache.SetMaxAge(cacheDuration);
            cache.AppendCacheExtension("must revalidate,proxy-revalidate");
        }
    }
}