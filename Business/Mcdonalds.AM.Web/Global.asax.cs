using System.Web.Http;
using i18n;
using i18n.Helpers;
using Mcdonalds.AM.Web.App_Start;
using Mcdonalds.AM.Web.Core;
using Mcdonalds.AM.Web.Filter;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Mcdonalds.AM.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            GlobalFilters.Filters.Add(new McdAMI18NFilter());
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            BundleTable.EnableOptimizations = ConfigurationManager.AppSettings["EnableOptimizations"].ToLower() == "true";

            i18n.LocalizedApplication.Current.DefaultLanguage = "en";
            i18n.UrlLocalizer.UrlLocalizationScheme = i18n.UrlLocalizationScheme.Scheme2;

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //var userId = HttpContext.Current.Request["user-id"];
            //if (!string.IsNullOrEmpty(userId))
            //{
            //    McdAMContext.Authenticate(userId);
            //}
            SetAppLangulage();


        }

        private void SetAppLangulage()
        {
            var langCookie = HttpContext.Current.Request.Cookies["Mcd_AM.LangTag"];
            if (langCookie != null)
            {
                var langtag = langCookie.Value;

                i18n.LanguageTag lt = i18n.LanguageTag.GetCachedInstance(langtag);
                if (lt.IsValid())
                {
                    // Set persistent cookie in the client to remember the language choice.
                    Response.Cookies.Add(new HttpCookie("Mcd_AM.LangTag")
                    {
                        Value = lt.ToString(),
                        HttpOnly = true,
                        Expires = DateTime.UtcNow.AddYears(1)
                    });

                    switch (langtag)
                    {
                        case "zh":
                            McdAMContext.SetLanguage("ZHCN");
                            break;
                        case "en":
                            McdAMContext.SetLanguage("ENUS");
                            break;
                    }

                }
                // Owise...delete any 'language' cookie in the client.
                else
                {
                    var cookie = Response.Cookies["Mcd_AM.LangTag"];
                    if (cookie != null)
                    {
                        cookie.Value = null;
                        cookie.Expires = DateTime.UtcNow.AddMonths(-1);
                    }
                }
                // Update PAL setting so that new language is reflected in any URL patched in the 
                // response (Late URL Localization).
                HttpContext.Current.SetPrincipalAppLanguageForRequest(lt);
                
            }

        }

    }
}
