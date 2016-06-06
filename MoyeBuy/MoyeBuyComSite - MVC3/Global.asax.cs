using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Data.SqlClient;

namespace MoyeBuy.Com.MoyeBuyComSite
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "ValidateCode",
                "{action}/code.jpg",
                new { controller = "Account", action = "ValidateCode" },
                null,
                new[] { "MoyeBuy.Com.MoyeBuyComSite.Controllers" }
                );

            routes.MapRoute(
                "Error",
                "Error/{id}.html",
                new { controller = "Error", action = "Index", id = UrlParameter.Optional },
                null,
                new[] { "MoyeBuy.Com.MoyeBuyComSite.Controllers" }
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

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }
    }
}