using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;

namespace Expose178.Com.Expose178Site.Controllers
{
    public class ArticleController : BaseController
    {
        private string strUID = GadgetScripts.Gadget.GetUserID();
        //
        // GET: /Article/

        [HttpGet]
        public ActionResult Index(string id)
        {
            Expose178.Com.Model.Article article = Proxys.ProxyArticle.GetArticle(id);
            ViewBag.Title =  GadgetScripts.Gadget.SalRemoveXSS(article.ArticleTile);
            return View(article);
        }

        [HttpGet]
        public ActionResult AddUpdateArticle()
        {
            BLL.AritcleType arType = new BLL.AritcleType();
            BLL.ReadRole readRole = new BLL.ReadRole();
            IDictionary<string, Object> dic = new Dictionary<string, Object>();
            try
            {
                IList<Expose178.Com.Model.AritcleType> listArType = arType.GetListArticleType();
                IList<Expose178.Com.Model.ReadRoleType> listReadRole = readRole.GetListReadRole();
                dic.Add("ListArticleType", listArType);
                dic.Add("ListReadRole", listReadRole);
            }
            catch (Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("UID", strUID);
                hshParam.Add("Error", ex.Message);
                UtilityFactory.Log.WriteLog(hshParam, "ArticleController.AddUpdateAtricle", UtilityFactory.LogType.LogToDB);
            }
            return View(dic);
        }

        [HttpPost]
        public JsonResult Add(Expose178.Com.Model.Article article)
        {
            bool result = false;
            try
            {
                article.ArticleTile = Server.UrlDecode(article.ArticleTile);
                article.ArticleBody = Server.UrlDecode(article.ArticleBody);
                article.UID = strUID;
                result = Proxys.ProxyArticle.AddUpdateArticle(article);
            }
            catch (Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("UID", strUID);
                hshParam.Add("Error", ex.Message);
                UtilityFactory.Log.WriteLog(hshParam, "ArticleController.Add", UtilityFactory.LogType.LogToDB);
            }

            if (result)
                return Json("SUCC");
            else
                return Json("FAILE");
        }
    }
}
