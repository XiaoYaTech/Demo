using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoyeBuy.Com.MoyeBuyComSite.Filters
{
    /// <summary>
    /// 页面压缩过滤器(所有页面控制器都加)
    /// </summary>
    public class CompressFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpRequestBase request = filterContext.HttpContext.Request;
            string acceptEncoding = request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(acceptEncoding)) return;
            acceptEncoding = acceptEncoding.ToUpperInvariant();

            HttpResponseBase response = filterContext.HttpContext.Response;
            if (acceptEncoding.Contains("GZIP"))
            {
                response.AppendHeader("Content-encoding", "gzip");
                response.Filter = new System.IO.Compression.GZipStream(response.Filter, System.IO.Compression.CompressionMode.Compress);
            }
        }
    }
}