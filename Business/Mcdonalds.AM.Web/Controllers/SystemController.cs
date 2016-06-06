using Mcdonalds.AM.ApiCaller;
using Mcdonalds.AM.Web.Core;
using System;
using System.Web;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace Mcdonalds.AM.Web.Controllers
{
    public class SystemController : BaseController
    {
        private const string SessionToken = "$CurrentUser$";
        private const string SessionRoleToken = "$CurrentUserRoleCode$";
        private const string CookieUserToken = "Mcd_AM_User";
        public ActionResult Login(string eid)
        {
            //获取用户信息
            if (string.IsNullOrEmpty(eid) || string.IsNullOrWhiteSpace(eid))
            {
                return Redirect("~/Error/LoginError.html");
            }
            var userId = eid;
            var context = System.Web.HttpContext.Current;
            string url = string.Format("{0}api/user/login?userId={1}", ConfigurationManager.AppSettings["Fx.ServiceUri"], userId);
            var userInfo = ApiProxy.Call<UserInfo>(url, "GET", null, null);
            context.Session[SessionToken] = userInfo;
            context.Session.Timeout = 24 * 60;
            HttpCookie cookieUser = new HttpCookie(CookieUserToken);
            cookieUser["Code"] = userInfo.Code;
            cookieUser["NameZHCN"] = HttpUtility.UrlEncode(userInfo.NameZHCN);
            cookieUser["NameENUS"] = HttpUtility.UrlEncode(userInfo.NameENUS);
            cookieUser["TitleENUS"] = userInfo.TitleENUS;
            cookieUser.Expires = DateTime.Now.AddMinutes(24 * 60);
            context.Response.Cookies.Add(cookieUser);
            return RedirectToAction("Index", "Home");
        }
    }
}
