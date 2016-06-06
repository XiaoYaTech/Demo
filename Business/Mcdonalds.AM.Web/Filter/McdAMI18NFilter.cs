using Mcdonalds.AM.Web.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mcdonalds.AM.Web.Filter
{
    public class McdAMI18NFilter:IActionFilter
    {
        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var lang = filterContext.RequestContext.HttpContext.Request["lang"];
            if(!string.IsNullOrEmpty(lang)){
                McdAMContext.SetLanguage(lang);
            }
        }
    }
}