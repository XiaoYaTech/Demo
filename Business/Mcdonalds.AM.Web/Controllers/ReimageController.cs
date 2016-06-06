using System.Web.Mvc;

namespace Mcdonalds.AM.Web.Controllers
{
    public class ReimageController : Controller
    {
        // GET: Reimage
        public ActionResult Main()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        #region ConsInfo

        public ActionResult ConsInfo()
        {
            return View();
        }

        public ActionResult ReimageNav()
        {
            return View("Module/ReimageNav");
        }

        #endregion

        #region Reimage Summary
        public ActionResult ReimageSummary()
        {
            return View();
        }
        #endregion

        #region Package
        public ActionResult ReimagePackage()
        {
            return View();
        }
        #endregion

        #region Site Info

        public ActionResult SiteInfo()
        {
            return View();
        }
        #endregion

        public ActionResult ApproveDialog()
        {
            return View("Module/ApproveDialog");
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

        public ActionResult SelectPackage()
        {
            return View();
        }
        public ActionResult ProjectDetail()
        {
            return View();
        }
        public ActionResult TempClosureMemo()
        {
            return View();
        }

    }
}