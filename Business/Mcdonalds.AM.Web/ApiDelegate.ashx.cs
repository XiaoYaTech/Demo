using Mcdonalds.AM.ApiCaller;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace NTTMNC.Fx.Portal
{
    /// <summary>
    /// Author: Stephen.Wang
    /// Date: 2014-07-02
    /// </summary>
    public class ApiDelegate : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            if (!string.IsNullOrEmpty(context.Request.ContentType))
            {
                context.Response.ContentType = context.Request.ContentType;
            }
            string url = context.Request.QueryString["url"];
            using (MemoryStream ms = new MemoryStream())
            {
                HttpContext.Current.Request.InputStream.CopyTo(ms);
                HttpContext.Current.Response.Write(ApiProxy.Call(url, HttpContext.Current.Request.HttpMethod, HttpContext.Current.Request.QueryString, ms.ToArray()));
            }
        }
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}