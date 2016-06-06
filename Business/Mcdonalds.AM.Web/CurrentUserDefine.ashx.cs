using Mcdonalds.AM.Web.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;

namespace Mcdonalds.AM.Web
{
    /// <summary>
    /// Summary description for CurrentUserDefine
    /// </summary>
    public class CurrentUserDefine : IHttpHandler,IRequiresSessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/javascript";
            context.Response.CacheControl = "no-cache";
            var userId = context.Request["userid"]??"";
            userId = userId.Trim();
            if (!string.IsNullOrEmpty(userId))
            {
                McdAMContext.Authenticate(userId);              
            }
            if (McdAMContext.CurrentUser != null)
            {
                context.Response.Write(string.Format("window.currentUser = {0};", JsonConvert.SerializeObject(McdAMContext.CurrentUser)));
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