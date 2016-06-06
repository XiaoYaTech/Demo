using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Filters;

namespace Mcdonalds.AM.Web.Filter
{
    public class BasicAuthAttribute : ActionFilterAttribute, IAuthenticationFilter
    {
        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            var user = filterContext.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }

        public void OnAuthentication(AuthenticationContext filterContext)
        {
            var user = filterContext.HttpContext.User;
            if (user == null || !user.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }
}