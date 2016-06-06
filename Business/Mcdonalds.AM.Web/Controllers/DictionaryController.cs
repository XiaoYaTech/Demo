using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mcdonalds.AM.Web.Controllers
{
    public class DictionaryController : Controller
    {
        //
        // GET: /Dictionary/
        public ActionResult List()
        {
            //DictionaryDataAccess provider = new DictionaryDataAccess();
            //return View(provider.QueryList(d => true, 10, 0));
            return View();
        }
   
        //
        // GET: /Dictionary/
        public ActionResult Index()
        {
            //DictionaryDataAccess provider = new DictionaryDataAccess();
            //return View(provider.QueryList(d => true, 10, 0));
            return View();
        }
    }
}
