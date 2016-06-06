using i18n;
using i18n.Helpers;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Mcdonalds.AM.Web.Core;

namespace Mcdonalds.AM.Web.Controllers
{
    public class BaseController : Controller
    {
        //protected override void OnActionExecuting(ActionExecutingContext filterContext)
        //{
        //    var langCookie = System.Web.HttpContext.Current.Request.Cookies["Mcd_AM.LangTag"];

        //    if (langCookie != null)
        //    {
        //        var langtag = langCookie.Value;


        //        i18n.LanguageTag lt = i18n.LanguageTag.GetCachedInstance(langtag);
        //        if (lt.IsValid())
        //        {
        //            // Set persistent cookie in the client to remember the language choice.
        //            Response.Cookies.Add(new HttpCookie("Mcd_AM.LangTag")
        //            {
        //                Value = lt.ToString(),
        //                HttpOnly = true,
        //                Expires = DateTime.UtcNow.AddYears(1)
        //            });

        //            switch (langtag)
        //            {
        //                case "zh":
        //                    McdAMContext.SetLanguage("ZHCN");
        //                    break;
        //                case "en":
        //                    McdAMContext.SetLanguage("ENUS");
        //                    break;
        //            }

        //        }
        //        // Owise...delete any 'language' cookie in the client.
        //        else
        //        {
        //            var cookie = Response.Cookies["Mcd_AM.LangTag"];
        //            if (cookie != null)
        //            {
        //                cookie.Value = null;
        //                cookie.Expires = DateTime.UtcNow.AddMonths(-1);
        //            }
        //        }
        //        // Update PAL setting so that new language is reflected in any URL patched in the 
        //        // response (Late URL Localization).
        //        System.Web.HttpContext.Current.SetPrincipalAppLanguageForRequest(lt);

                
               
        //    }
        //}

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //捕获所有的异常信息
            if (filterContext.Exception != null)
            {
                ILog log = LogManager.GetLogger(filterContext.Controller.GetType());
                log.Error("Unhandled exception: " + filterContext.Exception.Message +
                    ". Stack trace: " + filterContext.Exception.StackTrace,
                    filterContext.Exception);
            }
        }

	}
}