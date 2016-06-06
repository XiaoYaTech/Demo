using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using Mcdonalds.AM.DataAccess;

namespace Mcdonalds.AM.Services.Common
{
    public  class SiteInfo
    {
        private static string webUrl = string.Empty;
        public static string WebUrl 
        {
            get
            {
                if (string.IsNullOrEmpty(webUrl))
                {
                    webUrl = ConfigurationManager.AppSettings["webHost"];
                    
                }
                return webUrl;
            }
        }


        private static string _serviceUrl = string.Empty;
        public static string ServiceUrl
        {
            get
            {
                if (string.IsNullOrEmpty(_serviceUrl))
                {
                    _serviceUrl ="http://"+System.Web.HttpContext.Current.Request.Url.Authority+"/";
                }
                return _serviceUrl;

            }
        }

        /// <summary>
        /// 生成项目查看页面URL
        /// </summary>
        /// <param name="sourceCode"></param>
        /// <param name="nodeCode"></param>
        /// <param name="projectId"></param>
        public static String GetProjectViewPageUrl(string flowCode, string projectId)
        {
            string[] flowCodeArray = flowCode.Split('_');

            var url = string.Format("/{0}/Main#/{1}/Process/View?projectId={2}", flowCodeArray[0], flowCodeArray[1], projectId);
            return url;
        }
        /// <summary>
        /// 生成项目操作页面URL
        /// </summary>
        /// <param name="sourceCode"></param>
        /// <param name="nodeCode"></param>
        /// <param name="projectId"></param>
        public static String GetProjectHandlerPageUrl(string flowCode, string projectId)
        {
            string[] flowCodeArray = flowCode.Split('_');

            var url = string.Format("/{0}/Main#/{1}?projectId={2}", flowCodeArray[0], flowCodeArray[1], projectId);
            return url;
        }
    }
}