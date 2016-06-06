using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoyeBuy.Com.MoyeBuyComSite.Areas.ManageSite.Controllers
{
    public class AccountController :BaseController
    {
        BLL.Account bll = new BLL.Account();
        public ActionResult Index()
        {
            IList<Model.User> listUser = null;
            listUser = bll.GetUser();
            BLL.Role roleBll =new BLL.Role();
            ViewBag.Role = roleBll.GetRole();
            return View(listUser);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult Register(Model.User user)
        {
            string strResult = "FAIL";
            if (user != null && !string.IsNullOrEmpty(user.MoyeBuyComEmail))
            {
                strResult = bll.Register(user);
            }
            return Json(strResult);
        }
        public JsonResult DelUser(string UID)
        {
            string strResult = "FAIL";
            if (UID != null && !string.IsNullOrEmpty(UID))
            {
                if (bll.DelUser(UID))
                {
                    strResult = "SUCCESS";
                }
            }
            return Json(strResult);
        }
    }
}
