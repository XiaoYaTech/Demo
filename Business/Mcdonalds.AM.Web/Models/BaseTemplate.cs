/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   7/30/2014 2:15:22 PM
 * FileName     :   BaseTemplate
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.SessionState;

namespace Mcdonalds.AM.Web.Models
{
    public abstract class BaseTemplate : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable
        {
            get { throw new NotImplementedException(); }
        }

        public List<string> TemplateUrls{get;set;}

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/javascript";

            StringBuilder sb = new StringBuilder(string.Format(@"
                    angular.module('nttmnc.fx.modules.templates',['{0}'])
                ", string.Join("','", TemplateUrls.ToArray())));
            TemplateUrls.ForEach(url =>
            {
                sb.Append(buildTemplateCache(context, url));
            });
            context.Response.Write(sb.ToString());
        }

        public abstract void SetTemplateUrls();

        private string buildTemplateCache(HttpContext context, string url)
        {
            WebClient client = new WebClient();
            client.Encoding = Encoding.UTF8;
            var absoluteUrl = string.Concat(context.Request.Url.GetComponents(UriComponents.SchemeAndServer, UriFormat.SafeUnescaped), VirtualPathUtility.ToAbsolute(url, context.Request.ApplicationPath));
            string template = client.DownloadString(absoluteUrl).Replace("\r\n", "").Replace("\r", "").Replace("\n", "");
            return string.Format(@"
                angular.module('{0}', []).run(['$templateCache', function ($templateCache) {{
                    $templateCache.put('{0}','{1}');
                }}]);
            ", url, template.Replace("'", "\'"));
        }
    }
}