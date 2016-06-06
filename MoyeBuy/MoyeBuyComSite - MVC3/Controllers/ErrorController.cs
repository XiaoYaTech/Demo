using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoyeBuy.Com.MoyeBuyComSite.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/

        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
                ViewBag.Msg = "";
            else if (id == "1")
                ViewBag.Msg = "系统错误!";
            else if (id == "2")
                ViewBag.Msg = "您查找的商品不存在，可能已经删除或者下架!";
            else
                ViewBag.Msg = "";
            return View();
        }

    }
}
