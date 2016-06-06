using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mcdonalds.AM.Web.Controllers
{
    public class TaskWorkController : Controller
    {
        
        public ActionResult List()
        {
            //View/
            return View("List");
        }

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult MoreList()
        {
            return View();
        }

    }
}