using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Mcdonalds.AM.Web.Controllers
{
    public class RenewalModuleController : Controller
    {
        // GET: RenewalModule
        public ActionResult StoreBasicInfo()
        {
            return View();
        }

        public ActionResult LetterStoreBasicInfo()
        {
            return View();
        }

        public ActionResult LetterApprovalDialog()
        {
            return View("ApprovalDialog/Letter");
        }

        public ActionResult NegotiationRecord()
        {
            return View();
        }

        public ActionResult ConsInfoApprovalDialog()
        {
            return View("ApprovalDialog/ConsInfo");
        }

        public ActionResult AnalysisForm()
        {
            return View();
        }

        public ActionResult RenewalToolFinMeasureInput()
        {
            return View();
        }

        public ActionResult RenewalToolWriteOffAndReinCost()
        {
            return View();
        }

        public ActionResult RenewalToolFinMeasureOutput()
        {
            return View();
        }

        public ActionResult ToolApprovalDialog()
        {
            return View("ApprovalDialog/Tool");
        }

        public ActionResult SpecialApplication()
        {
            return View();
        }

        public ActionResult TransactionInvolve()
        {
            return View();
        }

        public ActionResult AnyLegalConcern()
        {
            return View();
        }

        public ActionResult LegalDepartmentReview()
        {
            return View();
        }

        public ActionResult SoxAudit()
        {
            return View();
        }

        public ActionResult EndorsementByGeneralCounsel()
        {
            return View();
        }

        public ActionResult LegalApprovalDialog()
        {
            return View("ApprovalDialog/LegalApproval");
        }

        public ActionResult KeyMeasures()
        {
            return View();
        }

        public ActionResult PackageApprovalDialog()
        {
            return View("ApprovalDialog/Package");
        }
        public ActionResult GBMemoApprovalDialog()
        {
            return View("ApprovalDialog/GBMemo");
        }
    }
}