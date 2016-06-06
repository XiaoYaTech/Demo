using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoyeBuy.Com.Model;
using System.Configuration;
using System.Collections;
using System.IO;

namespace MoyeBuy.Com.MoyeBuyComSite.Areas.ManageSite.Controllers
{
    public class ProductController : BaseController
    {
        private static readonly BLL.Product bll = new BLL.Product();
        IList<ProductCategory> listProductCategory = null;
        IList<SupplierInfo> listSupplierInfo = null;

        public ProductController()
        {
            listProductCategory = MoyeBuyComSite.Proxys.ProductCategoryProxy.GetProductCategory("");
            listSupplierInfo = MoyeBuyComSite.Proxys.SupplierProxy.GetProductCategory("");
        }

        public ActionResult Index(string id)
        {
            ViewBag.ListCategory = listProductCategory;
            ViewBag.ListSupplier = listSupplierInfo;
            string strFilterString = "";
            string strPageIndex = "1";
            string strPageSize = "30";
            string strSortField = "";
            bool IsASC = true;
            IList<Model.ProductInfo> listProd = null;
            listProd = bll.GetProduct(strFilterString, strPageIndex, strPageSize, strSortField, IsASC);
            return View(listProd);
        }

        public ActionResult Add()
        {
            ViewBag.ListCategory = listProductCategory;
            ViewBag.ListSupplier = listSupplierInfo;
            return View();
        }
        public ActionResult Updt(string id)
        {
            ViewBag.ListCategory = listProductCategory;
            ViewBag.ListSupplier = listSupplierInfo;

            IList<Model.ProductInfo> listProd = null;
            listProd = bll.GetProduct(id);

            Model.ProductInfo prod = new ProductInfo();

            if (listProd != null && listProd.Count > 0)
                prod = listProd[0];
            return View(prod);
        }


        [HttpPost]
        [ValidateInput(false)]
        public JsonResult AddProd(ProductInfo prod)
        {
            return this.UpdtProd(prod);
        }

        [HttpPost]
        [ValidateInput(false)]
        public JsonResult UpdtProd(ProductInfo prod)
        {
            string strReturn = "FAIL";
            if (prod != null && !string.IsNullOrEmpty(prod.ProductName))
            {
                IList<Model.ProductInfo> listProd = new List<Model.ProductInfo>();
                listProd.Add(prod);
                if (bll.AddUpdtProduct(listProd))
                    strReturn = "SUCCESS";
            }
            return Json(strReturn);
        }

        [HttpPost]
        [ValidateInput(true)]
        public JsonResult Del(string prodID)
        {
            string strReturn="FAIL";
            if (bll.DelProduct(prodID))
                strReturn = "SUCCESS";
            return Json(strReturn);
        }

        [HttpPost]
        public JsonResult UploadFile()
        {
            string strReturnMsg = "";
            String ImgUrl = "";
            string savePath = ConfigurationManager.AppSettings["ProdImg"].ToString();
            //定义允许上传的文件扩展名
            Hashtable extTable = new Hashtable();
            extTable.Add("image", "gif,jpg,jpeg,png,bmp");
            //extTable.Add("flash", "swf,flv");
            //extTable.Add("media", "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb");
            //extTable.Add("file", "doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2");

            //最大文件大小
            int maxSize = 1000000;
            HttpPostedFileBase imgFile = this.Request.Files["imgFile"];
            if (imgFile == null)
            {
                strReturnMsg = "错误：请选择文件。";
            }
            String dirPath = this.Server.MapPath(savePath);
            if (!Directory.Exists(dirPath))
            {
                strReturnMsg = "错误：上传目录不存在。";
            }
            String fileName = imgFile.FileName;
            String fileExt = Path.GetExtension(fileName).ToLower();

            if (imgFile.InputStream == null || imgFile.InputStream.Length > maxSize)
            {
                strReturnMsg = "错误：上传文件大小超过限制。";
            }
            if (String.IsNullOrEmpty(fileExt) || Array.IndexOf(((String)extTable["image"]).Split(','), fileExt.Substring(1).ToLower()) == -1)
            {
                strReturnMsg = "错误：上传文件扩展名是不允许的扩展名。\n只允许" + ((String)extTable["image"]) + "格式。";
            }

            //创建文件夹
            dirPath = dirPath + "\\";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            String ymd = DateTime.Now.ToString("yyyyMMdd", System.Globalization.DateTimeFormatInfo.InvariantInfo);
            dirPath += ymd + "\\";
            ImgUrl += ymd + "\\";
            if (!Directory.Exists(dirPath))
            {
                Directory.CreateDirectory(dirPath);
            }
            String newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", System.Globalization.DateTimeFormatInfo.InvariantInfo) + fileExt;
            String filePath = dirPath + newFileName;
            ImgUrl += newFileName;
            strReturnMsg = filePath;
            imgFile.SaveAs(filePath);
            Hashtable hash = new Hashtable();
            if (strReturnMsg.IndexOf("错误:") != -1)
                hash["error"] = 1;
            else
                hash["error"] = 0;
            hash["url"] = ImgUrl;
            hash["path"] = filePath;
            hash["message"] = strReturnMsg;
            return Json(hash);
        }
    }
}
