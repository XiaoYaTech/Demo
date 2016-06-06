using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoyeBuy.Com.MoyeBuyComSite.Filters;

namespace MoyeBuy.Com.MoyeBuyComSite.Areas.ManageSite.Controllers
{
    [UserAuthorize]
    public class BaseController : Controller
    {
        private static IList<MoyeBuy.Com.Model.Menu> MenuNavs = MoyeBuy.Com.MoyeBuyComSite.Proxys.LayoutMenuProxy.GetAdminMenu();
        public BaseController()
        {
            ViewBag.LeftMenu = MenuNavs;
        }
    }
}