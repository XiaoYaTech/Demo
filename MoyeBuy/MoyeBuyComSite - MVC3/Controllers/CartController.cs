using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;
using System.Web.Security;
using System.Configuration;
using System.IO;
using System.Text;

namespace MoyeBuy.Com.MoyeBuyComSite.Controllers
{
    public class CartController :BaseController
    {
        MoyeBuy.Com.BLL.Cart bll = new BLL.Cart();
        public ActionResult Index(string id)
        {
            string strID = "";
            string strNum = "";
            string strPrice = "0";
            Dictionary<string, object> listItem = null;
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    strID = MoyeBuyUtility.Gadget.Split(id, "----")[0].ToString();
                    strNum = MoyeBuyUtility.Gadget.Split(id, "----")[1].ToString();
                    strPrice = MoyeBuyUtility.Gadget.Split(id, "----")[2].ToString();
                    decimal decPrice = Convert.ToDecimal(strPrice);
                    bll.AddToCart(strID, strNum, decPrice);
                    listItem = bll.Item;
                }
                else
                    listItem = bll.GetAllCartItem();
            }
            catch (Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("UID", MoyeBuyUtility.Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "CartController.Index", UtilityFactory.LogType.LogToFile);
                return Redirect("/Error/1.html");
            }
            return View(listItem);
        }

        public ActionResult AddToCart(string id)
        {
            string strID = "";
            string strNum = "";
            string strPrice = "0";
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    strID = MoyeBuyUtility.Gadget.Split(id, "----")[0].ToString();
                    strNum = MoyeBuyUtility.Gadget.Split(id, "----")[1].ToString();
                    strPrice = MoyeBuyUtility.Gadget.Split(id, "----")[2].ToString();
                }
                decimal decPrice = Convert.ToDecimal(strPrice);
                bll.AddToCart(strID, strNum, decPrice);
            }
            catch (Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("UID", MoyeBuyUtility.Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "CartController.AddToCart", UtilityFactory.LogType.LogToFile);
                return Redirect("/Error/2.html");
            }
            return View();
        }

        [HttpPost]
        public JsonResult ClearCart()
        {
            this.HttpContext.Response.Cookies["CART"]["DATA"] = "";
            return Json("SUCCESS");
        }
        [HttpPost]
        public JsonResult DelProd(string id)
        {
            string strReturn = "FAIL";
            if (bll.DelProd(id))
                strReturn = "SUCCESS";
            return Json(strReturn);
        }

        public ActionResult CheckOutGuest()
        {
            BLL.Region reg = new BLL.Region();
            ViewBag.ListProvince = reg.GetProvinceByAll();
            return View();
        }

        [HttpPost]
        public JsonResult GetCity(string provinceId)
        {
            BLL.Region reg = new BLL.Region();
            IList<Model.City> listCity = reg.GetCityByProvinceID(provinceId);
            StringBuilder sbdHTML = new StringBuilder();
            if (listCity != null && listCity.Count > 0)
            {
                sbdHTML.Append("<select id=\"city\" name=\"city\" onchange=\"javascript:chagneCity(this);\">");
                sbdHTML.Append("<option value=\"\">---请选择---</option>");
                foreach (var item in listCity)
                {
                    sbdHTML.Append("<option value=\"" + item.CityID + "\" zipcode=\"" + item.ZipCode + "\">" + item.CityName + "</option>");
                }
                sbdHTML.Append("</select>");
            }
            return Json(sbdHTML.ToString());
        }

        [HttpPost]
        public JsonResult GetDistrict(string cityid)
        {
            BLL.Region reg = new BLL.Region();
            IList<Model.District> listDistrict = reg.GetDistrictByCityID(cityid);
            StringBuilder sbdHTML = new StringBuilder();
            if (listDistrict != null && listDistrict.Count > 0)
            {
                sbdHTML.Append("<select id=\"district\" name=\"district\">");
                sbdHTML.Append("<option value=\"\">---请选择---</option>");
                foreach (var item in listDistrict)
                {
                    sbdHTML.Append("<option value=\"" + item.DistrictID + "\">" + item.DistrictName + "</option>");
                }
                sbdHTML.Append("</select>");
            }
            return Json(sbdHTML.ToString());
        }

        [HttpPost]
        public JsonResult CheckoutResult(string cardno,string cardpwd ,string allprice)
        {
            string strReturn = "";
            bool isHaseError = false;
            try
            {
                if (string.IsNullOrEmpty(cardno))
                {
                    strReturn = "卡号不能为空";
                    isHaseError = true;
                }
                if (string.IsNullOrEmpty(allprice))
                {
                    strReturn = "总价格不能为空";
                    isHaseError = true;
                }
                if (!isHaseError)
                {
                    decimal decAllPrice = Convert.ToDecimal(allprice);
                    BLL.GiftCard cardBll = new BLL.GiftCard();
                    Model.GiftCard card = cardBll.GetGifCard(cardno);
                    if (card != null && card.GifCardPwd == cardpwd)
                    {
                        StringBuilder sbdHTML = new StringBuilder();
                        sbdHTML.Append("SUCCESS:");
                        sbdHTML.Append("<div>");
                        sbdHTML.Append("</div>");
                        strReturn = sbdHTML.ToString();
                    }
                    else
                        strReturn = "账号或者密码错误.";
                }
            }
            catch (Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("UID", MoyeBuyUtility.Gadget.GetUserID());
                hshParam.Add("Error", ex.Message);
                MoyeBuy.Com.UtilityFactory.Log.WriteLog(hshParam, "CartController.CheckoutResult", UtilityFactory.LogType.LogToFile);
                strReturn = "系统出错.";
            }
            return Json(strReturn);
        }
    }
}
