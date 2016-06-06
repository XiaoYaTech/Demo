using System;
using System.Web;
using i18n;

namespace Mcdonalds.AM.Web
{
    public class AMHttpModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.BeginRequest += new EventHandler(Application_SetAppLangulage);
        }

        private void Application_SetAppLangulage(object sender, EventArgs e)
        {
            var langCookie = HttpContext.Current.Request.Cookies["Mcd_AM.LangTag"];
            if (langCookie != null)
            {
                var langtag = langCookie.Value;

                i18n.LanguageTag lt = i18n.LanguageTag.GetCachedInstance(langtag);
                if (lt.IsValid())
                {
                    // Set persistent cookie in the client to remember the language choice.
                    HttpContext.Current.Response.Cookies.Add(new HttpCookie("Mcd_AM.LangTag")
                    {
                        Value = lt.ToString(),
                        HttpOnly = true,
                        Expires = DateTime.UtcNow.AddYears(1)
                    });
                }
                // Owise...delete any 'language' cookie in the client.
                else
                {
                    var cookie = HttpContext.Current.Response.Cookies["Mcd_AM.LangTag"];
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

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}