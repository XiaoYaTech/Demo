using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Expose178.Com.Expose178Site
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "DefaultArticle", // Route name
                "{controller}/{id}.html", // URL with parameters
                new { controller = "Article", action = "Index" } ,// Parameter defaults
                null,
                new[] { "Expose178.Com.Expose178Site.Controllers" }
            );

            routes.MapRoute(
                "DefaultTitle", // Route name
                "{controller}.html", // URL with parameters
                new { controller = "TitleList", action = "Index" }, // Parameter defaults
                null,
                new[] { "Expose178.Com.Expose178Site.Controllers" }
            );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }, // Parameter defaults
                null,
                new[] { "Expose178.Com.Expose178Site.Controllers" }
            );
        }
    }
}