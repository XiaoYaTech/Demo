using i18n;
using Mcdonalds.AM.Web.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mcdonalds.AM.Web.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Main()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        public ActionResult Login()
        {
            McdAMContext.ClearUser();
            McdAMContext.ClearCookie();
            return View();
        }

        [HttpPost]
        public JsonResult Login(string userCode)
        {
            string encryptedCode = Cryptography.Encrypt(userCode, DateTime.Now.ToString("yyyyMMdd"), "oms");
            var user = McdAMContext.Authenticate(encryptedCode);
            return Json(user);
        }

        [HttpPost]
        public JsonResult ValidateUser(string userId)
        {
            var user = McdAMContext.Authenticate(userId);
            return Json(user);
        }

        public ActionResult Personal()
        {
            return View("Index");
        }

        public ActionResult Home2()
        {
            return View();
        }

        public ActionResult ProjectList()
        {
            return View();
        }

        public ActionResult ProjectDetail()
        {
            return View();
        }

        public ActionResult CommentsList()
        {
            return View();
        }

        
        #region Demo
        public ActionResult MaterTrackDemo()
        {
            return View();
        }
        public ActionResult ContractInfoDemo()
        {
            return View();
        }
        #endregion


    }
}