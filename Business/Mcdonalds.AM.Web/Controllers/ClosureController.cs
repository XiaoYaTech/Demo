using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mcdonalds.AM.Web.Controllers
{
    public class ClosureController:BaseController
    {

        public ActionResult Main()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult WOCheckList()
        {
            return View();
        }

        public ActionResult WOCheckListView()
        {
            return View();
        }

        public ActionResult WOCheckListProcess()
        {
            return View("Process/WOCheckListProcess");
        }


        public ActionResult WOCheckListProcessEdit()
        {
            return View("Process/WOCheckListProcessEdit");

        }

        public ActionResult ClosureTool()
        {
            return View();
        }

        public ActionResult ClosureToolView()
        {
            return View("View/ClosureToolView");
        }

        public ActionResult ClosureToolProcess()
        {
            return View("Process/ClosureToolProcess");
        }
        public ActionResult ClosureToolProcessEdit()
        {
            return View("Process/ClosureToolProcessEdit");
        }


        public ActionResult LegalReview()
        {
            return View();
        }

        public ActionResult LegalReviewProcess()
        {
            return View("Process/LegalReviewProcess");
        }
        public ActionResult LegalReviewProcessEdit()
        {
            return View("Process/LegalReviewProcessEdit");
        }
        public ActionResult LegalReviewView()
        {
            return View("View/LegalReviewView");
        }
        public ActionResult ClosurePackageView()
        {
            return View("View/ClosurePackageView");
        }


        public ActionResult ExecutiveSummary()
        {
            return View();
        }

        public ActionResult ExecutiveSummaryView()
        {
            return View("View/ExecutiveSummaryView");
        }

        public ActionResult ExecutiveSummaryProcess()
        {
            return View("Process/ExecutiveSummaryProcess");
        }

        public ActionResult ExecutiveSummaryProcessEdit()
        {
            return View("Process/ExecutiveSummaryProcessEdit");
        }

        public ActionResult ClosurePackage()
        {
            return View();
        }

        public ActionResult ClosurePackageProcess()
        {
            return View("Process/ClosurePackageProcess");
        }

        public ActionResult ClosurePackageProcessEdit()
        {
            return View("Process/ClosurePackageProcessEdit");
        }

        public ActionResult ClosurePackageProcessUpload()
        {
            return View("Process/ClosurePackageProcessUpload");
        }

        public ActionResult ContractInfo()
        {
            return View();
        }


        public ActionResult ContractInfoView()
        {
            return View("View/ContractInfoView");
        }


        public ActionResult ClosureMemo()
        {
            return View();
        }

        public ActionResult ConsInvtChecking()
        {
            return View();
        }

        public ActionResult ClosureMemoView()
        {
            return View("View/ClosureMemoView");
        }

        public ActionResult ConsInvtCheckingView()
        {
            return View("View/ConsInvtCheckingView");
        }
        public ActionResult ConsInvtCheckingProcess()
        {
            return View("Process/ConsInvtCheckingProcess");
        }
        public ActionResult ConsInvtCheckingProcessEdit()
        {
            return View("Process/ConsInvtCheckingProcessEdit");
        }

        public ActionResult ProjectDetail()
        {
            return View();
        }
    }
}