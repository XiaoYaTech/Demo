using System.Web.Mvc;

namespace Expose178.Com.Expose178Site.Areas.MangeSite
{
    public class MangeSiteAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "MangeSite";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "MangeSite_default",
                "MangeSite/{action}/{id}",
                new { controller = "Manage", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
