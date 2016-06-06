using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoyeBuy.Com.MoyeBuyComSite.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UserAuthorizeAttribute:AuthorizeAttribute
    {
        public UserAuthorizeAttribute() { }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("");
            Model.User user = MoyeBuy.Com.MoyeBuyUtility.Gadget.GetUserFromTicket();
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
                filterContext.HttpContext.Response.Redirect("/account/logon",true);
            else if (user.RoleName.ToUpper() != "ADMIN")
                filterContext.HttpContext.Response.Redirect("/home/index", true);
            base.OnAuthorization(filterContext);
        }
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            return base.AuthorizeCore(httpContext);
        }
    }
}