using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;

namespace Expose178.Com.Expose178Site.Controllers
{
    public class BaseController:Controller
    {
        public Expose178.Com.Model.SiteLayout layout = null;
        public BaseController()
        {
            layout = new Model.SiteLayout();
            layout.SiteName = ConfigurationManager.AppSettings["SiteName"].ToString();
            layout.Logo = ConfigurationManager.AppSettings["Logo"].ToString();
            layout.BackgroundImg = ConfigurationManager.AppSettings["BackgroundImg"].ToString();
            ViewBag.SiteLayout = layout;
        }
    }
}