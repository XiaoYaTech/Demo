using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mcdonalds.AM.Web.Controllers
{
    public class RedirectController : Controller
    {
        //
        // GET: /Redirect/
        public ActionResult Index()
        {
            return Redirect("http://" + Request.Url.Authority + "/Home/Main/#/taskwork");
        }

    }
}