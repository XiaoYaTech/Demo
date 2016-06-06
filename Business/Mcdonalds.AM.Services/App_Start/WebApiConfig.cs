using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.WebHost;
using System.Web.Routing;
using System.Web.SessionState;

namespace Mcdonalds.AM.Services
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            config.Filters.Add(new ExceptionLogFilterAttribute());

            // Web API routes
            config.MapHttpAttributeRoutes();

            RouteTable.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
           ).RouteHandler = new SessionStateRouteHandler();

            config.Formatters.JsonFormatter.SerializerSettings.DateTimeZoneHandling =
                Newtonsoft.Json.DateTimeZoneHandling.Local;

        }

        public class SessionableControllerHandler : HttpControllerHandler, IRequiresSessionState
        {
            public SessionableControllerHandler(RouteData routeData)
                : base(routeData)
            { }
        }

        public class SessionStateRouteHandler : IRouteHandler
        {
            IHttpHandler IRouteHandler.GetHttpHandler(RequestContext requestContext)
            {
                return new SessionableControllerHandler(requestContext.RouteData);
            }
        }
    }
}
