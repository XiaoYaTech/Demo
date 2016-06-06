using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mcdonalds.AM.Web.Controllers
{
    public class MajorLeaseController : Controller
    {

        public ActionResult Main()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult LegalReview()
        {
            return View();
        }

        public ActionResult FinanceAnalysis()
        {
            return View();
        }

        public ActionResult ConsInfo()
        {
            return View();
        }

        public ActionResult LeaseChangePackage()
        {
            return View();
        }

        public ActionResult ContractInfo()
        {
            return View();
        }

        public ActionResult SiteInfo()
        {
            return View();
        }
        public ActionResult ConsInvtChecking()
        {
            return View();
        }
        public ActionResult ReopenMemo()
        {
            return View();
        }
        public ActionResult GBMemo()
        {
            return View();
        }
        public ActionResult ClosureMemo()
        {
            return View();
        }
        public ActionResult MajorLeaseChangeTemp()
        {
            return View("Module/MajorLeaseChangeTemp");
        }

        public ActionResult NavTop()
        {
            return View("Module/MajorLeaseNav");
        }

        public ActionResult ReinvenstmentAmountType()
        {
            return View("Module/ReinvenstmentAmountType");
        }

        public ActionResult ApproveDialog()
        {
            return View("Module/ApproveDialog");
        }

        public ActionResult ProjectDetail()
        {
            return View();
        }
    }
}