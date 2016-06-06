using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoyeBuy.Com.Model;

namespace MoyeBuy.Com.MoyeBuyComSite.Areas.ManageSite.Controllers
{
    public class ProductCategoryController :BaseController
    {
        //
        // GET: /ManageSite/ProductCategory/

        public ActionResult Index(string id)
        {
            IList<Model.ProductCategory> listCategory = MoyeBuyComSite.Proxys.ProductCategoryProxy.GetProductCategory(id);
            return View(listCategory);
        }

        [HttpPost]
        public JsonResult Del(string strCategoryID)
        {
            string strResult = "FAIL";
            if (strCategoryID != "")
            {
                MoyeBuy.Com.BLL.ProductCategory category = new BLL.ProductCategory();
                if(category.DelProductCatgory(strCategoryID))
                    strResult = "SUCCESS";
            }
            return Json(strResult);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult AddUpd(Model.ProductCategory pcategory)
        {
            string strResult = "FAIL";
            if (pcategory.CategoryName != null)
            {
                MoyeBuy.Com.BLL.ProductCategory category = new BLL.ProductCategory();
                if (category.AddUpdateProductCatgory(pcategory))
                    strResult = "SUCCESS";
            }
            return Json(strResult);
        }
    }
}
