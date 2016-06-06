using System.Web.Mvc;

namespace Expose178.Com.Expose178Site.Areas.ManageSite
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
                "ManageSite_default",
                "ManageSite/{action}/{id}",
                new { controller = "Home" , action = "Index", id = UrlParameter.Optional, },
                null,
                new[] { "Expose178.Com.Expose178Site.Areas.ManageSite.Controllers" }
            );
        }
    }
}
