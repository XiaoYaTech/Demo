using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoyeBuy.Com.MoyeBuyComSite.Filters;

namespace MoyeBuy.Com.MoyeBuyComSite.Controllers
{
    [CompressFilter]
    public class BaseController : Controller
    {
        private static IList<Model.Ad> listAdd = MoyeBuyComSite.Proxys.AdProxy.GetAdvertisement();
        public BaseController()
        {
            ViewBag.ListAdd = listAdd;
        }
    }
}
