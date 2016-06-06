using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using i18n;

namespace Mcdonalds.AM.Web.Controllers
{
    public class LoginController : BaseController
    {
        //
        // GET: /Login/
        public ActionResult Index()
        {            
            return View();
        }
    }
}
