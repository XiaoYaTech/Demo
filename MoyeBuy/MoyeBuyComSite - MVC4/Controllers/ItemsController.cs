using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoyeBuy.Com.MoyeBuyComSite.Controllers
{
    public class ItemsController : BaseController
    {
        private static IList<Model.ProductInfo> listHotProd = Proxys.ProductProxy.GetProductByKeywords("IsSellHot='true'", "1", "30", "ProductName", true);
        public ActionResult Index(string id)
        {
            if (string.IsNullOrEmpty(id))
                return Redirect("/Error/2.html");
            IList<Model.ProductInfo> listProd = Proxys.ProductProxy.GetProductByIDs(id);
            ViewBag.ListHotProd = listHotProd;
            if (listProd.Count > 0)
                return View(listProd[0]);
            else
                return Redirect("/Error/1.html");
        }
        public ActionResult Search(string keywords)
        {
            return View();
        }
    }
}
