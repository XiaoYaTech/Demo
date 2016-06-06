using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoyeBuy.Com.Model;

namespace MoyeBuy.Com.MoyeBuyComSite.Areas.ManageSite.Controllers
{
    public class SupplierController : BaseController
    {
        //
        // GET: /ManageSite/Supplier/

        public ActionResult Index()
        {
            IList<Model.SupplierInfo> listSupplier = MoyeBuyComSite.Proxys.SupplierProxy.GetProductCategory("");
            return View(listSupplier);
        }

        [HttpPost]
        public JsonResult Del(string SupplierID)
        {
            string strResult = "FAIL";
            if (SupplierID != "")
            {
                MoyeBuy.Com.BLL.Supplier bsupplier = new BLL.Supplier();
                if (bsupplier.DelProductSupplierByID(SupplierID))
                    strResult = "SUCCESS";
            }
            return Json(strResult);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult addupd(Model.SupplierInfo supplier)
        {
            string strResult = "FAIL";
            if (supplier.SupplierName != null)
            {
                MoyeBuy.Com.BLL.Supplier bsupplier = new BLL.Supplier();
                if (bsupplier.AddUpdateProductSupplier(supplier))
                    strResult = "SUCCESS";
            }
            return Json(strResult);
        }
    }
}
