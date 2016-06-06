using System.Web.Mvc;

namespace MoyeBuy.Com.MoyeBuyComSite.Areas.ManageSite
{
    public class ManageSiteAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ManageSite";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ManageSite_ProductCategory",
                "Admin/Category/{action}",
                new { action = "Index", controller = "ProductCategory", id = UrlParameter.Optional },
                null,
                new[] { "MoyeBuy.Com.MoyeBuyComSite.Areas.ManageSite.Controllers" }
            );

            context.MapRoute(
                "ManageSite_Supplier_Detail",
                "Admin/Supplier/{id}.html",
                new { action = "Index", controller = "Supplier", id = UrlParameter.Optional },
                null,
                new[] { "MoyeBuy.Com.MoyeBuyComSite.Areas.ManageSite.Controllers" }
            );

            context.MapRoute(
                "ManageSite_Supplier",
                "Admin/Supplier/{action}",
                new { action = "Index", controller = "Supplier", id = UrlParameter.Optional },
                null,
                new[] { "MoyeBuy.Com.MoyeBuyComSite.Areas.ManageSite.Controllers" }
            );

            context.MapRoute(
                "ManageSite_Product",
                "Admin/Product/{action}/{id}",
                new { action = "Index", controller = "Product", id = UrlParameter.Optional },
                null,
                new[] { "MoyeBuy.Com.MoyeBuyComSite.Areas.ManageSite.Controllers" }
            );

            context.MapRoute(
                "ManageSite_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", controller = "Home", id = UrlParameter.Optional },
                null,
                new[] { "MoyeBuy.Com.MoyeBuyComSite.Areas.ManageSite.Controllers" }
            );
        }
    }
}
