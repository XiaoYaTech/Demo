using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoyeBuy.Com.MoyeBuyUtility;
using MoyeBuy.Com.MoyeBuyComSite.Filters;

namespace MoyeBuy.Com.MoyeBuyComSite.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            Dictionary<string, IList<Model.ProductInfo>> dicProd = new Dictionary<string, IList<Model.ProductInfo>>();
            IList<Model.ProductInfo> listHotProd = Proxys.ProductProxy.GetProductByKeywords("IsSellHot='true'", "1", "30", "ProductName", true);
            IList<Model.ProductInfo> listNewProd = Proxys.ProductProxy.GetProductByKeywords(null, "1", "30", "LastUpdatedDate", false);
            dicProd.Add("listHotProd", listHotProd);
            dicProd.Add("listNewProd", listNewProd);
            return View(dicProd);
        }
    }
}
