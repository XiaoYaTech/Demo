using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoyeBuy.Com.MoyeBuyComSite.Areas.ManageSite.Controllers
{
    public class GiftCardController :BaseController
    {
        //
        // GET: /ManageSite/GiftCard/

        public ActionResult Index()
        {
            IList<Model.GiftCard> listGiftCard = null;
            BLL.GiftCard bll = new BLL.GiftCard();
            listGiftCard = bll.GetGifCard("10", "1", true, "GiftCardNo");
            return View(listGiftCard);
        }

        public ActionResult GenerateCard()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GenerateProcess(string cardcount, string preNum, string cardAmount, string startDate, string expireDate)
        {
            string strResult ="";
            if(string.IsNullOrEmpty(cardcount))
                strResult="生成的张数不能为0";
            if(string.IsNullOrEmpty(preNum))
                strResult="卡号前缀不能为空";
            if(string.IsNullOrEmpty(cardAmount))
                strResult="卡内金额不能为0";
            if(string.IsNullOrEmpty(startDate))
                strResult="开始日期不能为空";
            if(string.IsNullOrEmpty(expireDate))
                strResult="结束不能为空";
            try
            {
                DateTime StartDate = Convert.ToDateTime(startDate);
                DateTime ExpireDate = Convert.ToDateTime(expireDate);
                BLL.GiftCard bll = new BLL.GiftCard();
                int CardNum = Convert.ToInt32(cardcount);
                decimal CardAmount = Convert.ToDecimal(cardAmount);
                bool isGen= bll.GenerateGiftCard(CardNum,CardAmount,preNum,StartDate,ExpireDate);
                if (isGen)
                    strResult = "SUCCESS";
                else
                    strResult = "生成礼券卡异常.";
            }
            catch(Exception ex)
            {
                strResult = ex.Message;
            }
            
            return Json(strResult);
        }
    }
}
