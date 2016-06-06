using System;
using System.Web;
using System.Web.Http;
using System.Web.SessionState;

namespace Mcdonalds.AM.Services
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //JavaScriptDateTimeConverter timeConverter = new JavaScriptDateTimeConverter();
            //GlobalConfiguration.Configuration.Formatters.JsonFormatter.SerializerSettings.Converters.Add(timeConverter);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            GlobalConfiguration.Configuration.EnableCors();

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            //HttpContext.Current.Response.AddHeader("Access-Control-Allow-Origin", "*");

            if (HttpContext.Current.Request.HttpMethod == "OPTIONS")
            {
                //These headers are handling the "pre-flight" OPTIONS call sent by the browser
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE");
                HttpContext.Current.Response.AddHeader("Access-Control-Allow-Headers", "Content-Type, Accept");
                HttpContext.Current.Response.AddHeader("Access-Control-Max-Age", "1728000");
                HttpContext.Current.Response.End();
            }
        }


        protected void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            System.Web.HttpContext.Current.SetSessionStateBehavior(
                SessionStateBehavior.Required);
        }


    }
}
