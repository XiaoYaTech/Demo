using Mcdonalds.AM.DataAccess.Infrastructure;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   7/31/2014 6:27:16 PM
 * FileName     :   ClientCookie
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.Services.Infrastructure
{
    public class ClientCookie
    {
        private const string UserCookieTag = "Mcd_AM_User";
        private const string LanguageTag = "Mcd_AM_Lang";
        /// <summary>
        /// 客户端当前用户EID
        /// </summary>
        public static string UserCode
        {
            get
            {
                //return "E5011244";
                var cookie = HttpContext.Current.Request.Cookies[UserCookieTag];
                return cookie != null ? cookie["Code"] : string.Empty;
            }
        }

        /// <summary>
        /// 客户端当前用户中文名
        /// </summary>
        public static string UserNameZHCN
        {
            get
            {
                var cookie = HttpContext.Current.Request.Cookies[UserCookieTag];
                return cookie != null ? HttpUtility.UrlDecode(cookie["NameZHCN"]) : string.Empty;
            }
        }

        /// <summary>
        /// 客户端当前用户英文名
        /// </summary>
        public static string UserNameENUS
        {
            get
            {
                var cookie = HttpContext.Current.Request.Cookies[UserCookieTag];
                return cookie != null ? HttpUtility.UrlDecode(cookie["NameENUS"]) : string.Empty;
            }
        }

        public static SystemLanguage Language
        {
            get
            {
                var cookie = HttpContext.Current.Request.Cookies[LanguageTag];
                var lang = SystemLanguage.ZHCN;
                if (cookie != null)
                {
                    Enum.TryParse<SystemLanguage>(cookie.Value, out lang);
                }
                return lang;
            }
        }

        public static string TitleENUS
        {
            get
            {
                var cookie = HttpContext.Current.Request.Cookies[UserCookieTag];
                return cookie != null ? HttpUtility.UrlDecode(cookie["TitleENUS"]) : string.Empty;
            }
        }
    }
}