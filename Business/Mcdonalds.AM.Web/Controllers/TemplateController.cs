using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mcdonalds.AM.Web.Controllers
{
    public class TemplateController : Controller
    {
        public PartialViewResult CoreTemplates()
        {
            return PartialView();
        }
        // GET: /Template/
        public ActionResult SelectStore()
        {
            return View();
        }

        public ActionResult NoticeUsers()
        {
            return View();
        }

        public ActionResult ClosureNavs()
        {
            return View();
        }

        public ActionResult ProjectAdvanceSearch()
        {
            return View();
        }

        public ActionResult ClosurePackageSelApprover()
        {
            return View();
        }

        public ActionResult ClosureWOCheckListSelApprover()
        {
            return View();
        }

        public ActionResult ClosureToolSelApprover()
        {
            return View();
        }

        public ActionResult Recall()
        {
            return View();
        }

        public ActionResult LegalReviewSelApprover()
        {
            return View();
        }

        public ActionResult ConsInvtCheckingSelApprover()
        {
            return View();
        }


    }
}