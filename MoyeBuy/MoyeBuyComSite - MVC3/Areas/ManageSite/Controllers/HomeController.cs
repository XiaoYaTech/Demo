using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MoyeBuy.Com.MoyeBuyUtility;
using MoyeBuy.Com.Model;
using System.Collections;

namespace MoyeBuy.Com.MoyeBuyComSite.Areas.ManageSite.Controllers
{
    public class HomeController : BaseController
    {
        //
        // GET: /ManageSite/Home/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MoyeBuyMenu()
        {
            IList<Menu> listMenu = Proxys.LayoutMenuProxy.GetMenu();
            ViewBag.listMenu = listMenu;
            return View();
        }

        public ActionResult AdminMenu()
        {
            IList<Menu> listMenu = Proxys.LayoutMenuProxy.GetAdminMenu();
            ViewBag.listMenu = listMenu;
            return View(listMenu);
        }

        public ActionResult CategoryMenu()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddMenu(IList<Menu> listMenu, string mainMenuID,string saveType)
        {
            string strReturn = "FAIL";
            IList<Menu> passedMenu = new List<Menu>();
            int i = 0;
            if (listMenu==null)
                return Json(strReturn);
            foreach (Menu menu in listMenu)
            {
                string strMenuID = Request.Form["listMenu[" + i + "][MenuID]"].ToString();
                if (strMenuID == "")
                    menu.MenuID = null;
                else
                    menu.MenuID = Convert.ToInt32(strMenuID); ;
                menu.MenuName = Request.Form["listMenu["+i+"][MenuName]"].ToString();
                menu.MenuClassName = Request.Form["listMenu[" + i + "][MenuClassName]"].ToString();
                menu.MenuUrl = Request.Form["listMenu[" + i + "][MenuUrl]"].ToString();
                menu.Target = Request.Form["listMenu[" + i + "][Target]"].ToString();
                menu.IsAdminMenu = Convert.ToBoolean(Request.Form["listMenu[" + i + "][IsAdminMenu]"].ToString()=="1"?"true":"false");
                menu.MenuType = Request.Form["listMenu[" + i + "][MenuType]"].ToString();
                menu.Disq = Request.Form["listMenu[" + i + "][MenuDisq]"].ToString();
                passedMenu.Add(menu);
                i++;
            }
            if (passedMenu != null && passedMenu.Count > 0)
            {
                MoyeBuy.Com.BLL.LayoutMenu menu = new BLL.LayoutMenu();
                string strNewIDs = menu.AddUpdateMenu(passedMenu);
                if (strNewIDs != "" && saveType == "SaveMainMenu")
                {
                    strReturn = "SUCCESS";
                }
                else if (strNewIDs != "" && saveType == "SaveSubMenu")
                {
                    if (menu.MappingSubMenu("", mainMenuID, strNewIDs))
                        strReturn = "SUCCESS";
                }
            }
            return Json(strReturn);
        }
        [HttpPost]
        public void DelMenu(string strMenuID)
        { 
            
        }
    }
}
