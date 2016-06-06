using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace MoyeBuy.Com.MoyeBuyComSite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "ValidateCode",
                url: "{action}/code.jpg",
                defaults: new { controller = "Account", action = "ValidateCode", id = UrlParameter.Optional },
                constraints: null,
                namespaces: new[] { "MoyeBuy.Com.MoyeBuyComSite.Controllers" }
                );

            routes.MapRoute(
                name:"Error",
                url:"Error/{id}.html",
                defaults:new { controller = "Error", action = "Index", id = UrlParameter.Optional },
                constraints:null,
                namespaces:new[] { "MoyeBuy.Com.MoyeBuyComSite.Controllers" }
                );

            routes.MapRoute(
                "Items",
                "Items/{id}.html",
                new { controller = "Items", action = "Index", id = UrlParameter.Optional },
                null,
                new[] { "MoyeBuy.Com.MoyeBuyComSite.Controllers" }
                );

            routes.MapRoute(
                "Categorys",
                "Categorys/{id}.html",
                new { controller = "Categorys", action = "Index", id = UrlParameter.Optional },
                null,
                new[] { "MoyeBuy.Com.MoyeBuyComSite.Controllers" }
                );

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }, // Parameter defaults
                null,
                new[] { "MoyeBuy.Com.MoyeBuyComSite.Controllers" }
            );
        }
    }
}