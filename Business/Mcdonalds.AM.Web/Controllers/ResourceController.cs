using Mcdonalds.AM.Web.Core;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Mcdonalds.AM.Web.Controllers
{
    public class ResourceController : Controller
    {
        public JsonResult CurrentUser()
        {
            return Json(McdAMContext.CurrentUser, JsonRequestBehavior.DenyGet);
        }
	}
}