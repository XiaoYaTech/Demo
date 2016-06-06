using Mcdonalds.AM.ApiCaller;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.Web.Core
{
    public class McdAMContext
    {
        private const string SessionToken="$CurrentUser$";
        //private const string SessionRoleToken = "$CurrentUserRoleCode$";
        private const string CookieUserToken = "Mcd_AM_User";
        public static UserInfo CurrentUser
        {
            get
            {
                var context = HttpContext.Current;
                if (context.Session[SessionToken] != null)
                {
                    return (UserInfo)context.Session[SessionToken];
                }
                else if (!string.IsNullOrEmpty(context.Request["user-id"]))
                {
                    return Authenticate(context.Request["user-id"]);
                }
                else if (!string.IsNullOrEmpty(context.Request["userId"]))
                {
                    return Authenticate(context.Request["userId"]);
                }
                else return null;
            }
        }

        //public static string CurrentUserRoleCode
        //{
        //    get
        //    {
        //        string userRoleIds = string.Empty;
        //        var context = HttpContext.Current;
        //        if (context.Session[SessionRoleToken] != null)
        //        {
        //            return (string)context.Session[SessionRoleToken];
        //        }
        //        else
        //        {
        //            if (CurrentUser != null && !string.IsNullOrEmpty(CurrentUser.Code))
        //            {
        //                //string url = string.Format("{0}api/role/get/{1}", ConfigurationManager.AppSettings["Fx.ServiceUri"], CurrentUser.Code);
        //                //var userRoleIds = ApiProxy.Call<string>(url, "GET", null, null);
        //                //context.Session[SessionRoleToken] = userRoleIds;
        //                //context.Session.Timeout = 24 * 60;
        //                return userRoleIds;
        //            }
        //        }
        //        return null;
        //    }
        //}

        public static SystemLanguage Language
        {
            get
            {
                var cookieLang = HttpContext.Current.Request.Cookies["Mcd_AM_Lang"];
                var language = SystemLanguage.ZHCN;
                if(cookieLang != null){
                    Enum.TryParse<SystemLanguage>(cookieLang.Value,out language);
                }
                return language;
            }
        }

        public static void ClearUser()
        {
            HttpContext.Current.Session[SessionToken] = null;
            //HttpContext.Current.Session[SessionRoleToken] = null;
            ClearCookie();
        }

        public static void SetLanguage(string lang)
        {
            var cookieLang = new HttpCookie("Mcd_AM_Lang");
            cookieLang.Value = lang.Replace("-", "").Replace("_", "").ToUpperInvariant();
            cookieLang.Expires = DateTime.Now.AddDays(1);
            HttpContext.Current.Response.Cookies.Add(cookieLang);
        }

        public static UserInfo Authenticate(string userId)
        {
            UserInfo userInfo = null;
            var context = HttpContext.Current;
            if (context.Session[SessionToken] != null)
            {
                userInfo = context.Session[SessionToken] as UserInfo;
            }
            else
            {
                string url = string.Format("{0}api/user/auth?userId={1}", ConfigurationManager.AppSettings["Fx.ServiceUri"], userId);
                userInfo = ApiProxy.Call<UserInfo>(url, "GET", null, null);
                context.Session[SessionToken] = userInfo;
                context.Session.Timeout = 24 * 60;
            }

            HttpCookie cookieUser = new HttpCookie(CookieUserToken);
            cookieUser["Code"] = userInfo.Code;
            cookieUser["NameZHCN"] = HttpUtility.UrlEncode(userInfo.NameZHCN);
            cookieUser["NameENUS"] = HttpUtility.UrlEncode(userInfo.NameENUS);
            cookieUser["TitleENUS"] = userInfo.TitleENUS;
            cookieUser.Expires = DateTime.Now.AddMinutes(24 * 60);

            context.Response.Cookies.Add(cookieUser);
            return userInfo;
        }

        /// <summary>
        /// 清空存储UserCode的Cookie.
        /// </summary>
        public static void ClearCookie()
        {
            var context = HttpContext.Current;
            HttpCookie cookieUser = new HttpCookie(CookieUserToken);

            cookieUser.Expires = DateTime.Now.AddDays(-1);

            context.Response.Cookies.Add(cookieUser);

        }

        public enum SystemLanguage
        {
            UNKNOWN = 0,
            ZHCN = 1,
            ENUS = 2
        }
    }
}