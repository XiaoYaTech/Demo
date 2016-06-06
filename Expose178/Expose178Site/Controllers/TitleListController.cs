using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections;

namespace Expose178.Com.Expose178Site.Controllers
{
    public class TitleListController : BaseController
    {
        //
        // GET: /TitleList/
        private Expose178Site.Proxys.ProxyArticleTitle proxyTitle = null;
        private string strUID = GadgetScripts.Gadget.GetUserID();

        public TitleListController()
        {
            proxyTitle = new Proxys.ProxyArticleTitle();
        }

        public ActionResult Index()
        {
            IList<Expose178.Com.Model.ArticleTitle> listArticleTitle = GetListArticleTile("");
            return View(listArticleTitle);
        }

        public ActionResult MyArticleList()
        {
            IList<Expose178.Com.Model.ArticleTitle> listArticleTitle = GetListArticleTile(strUID);
            return View(listArticleTitle);
        }

        private IList<Expose178.Com.Model.ArticleTitle> GetListArticleTile(string strUpdatedUserID)
        {
            IList<Expose178.Com.Model.ArticleTitle> listArticleTitle = null;
            try
            {
                string strAricleTypeCode = "";
                if (Request.Form.AllKeys.Contains("AricleTypeCode"))
                {
                    strAricleTypeCode = Request.Form["AricleTypeCode"].ToString();
                }

                string strSortField = "";
                if (Request.Form.AllKeys.Contains("AricleTypeCode"))
                {
                    strSortField = Request.Form["SortField"].ToString();
                }

                bool IsASC = false;
                if (Request.Form.AllKeys.Contains("IsASC"))
                {
                    if (Request.Form["IsASC"].ToString()=="0")
                        IsASC = true;
                }

                int intPageIndex = 1;
                if (Request.Form.AllKeys.Contains("PageIndex"))
                {
                    intPageIndex = Convert.ToInt32(Request.Form["PageIndex"].ToString());
                }

                int intPageSize = 30;
                if (Request.Form.AllKeys.Contains("PageSize"))
                {
                    intPageSize = Convert.ToInt32(Request.Form["PageSize"].ToString());
                }

                listArticleTitle = Proxys.ProxyArticleTitle.GetArticleTitle(strAricleTypeCode, strUpdatedUserID, strSortField, IsASC, false, intPageSize, intPageIndex);
            }
            catch (Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("Error", ex.Message);
                hshParam.Add("UID", strUID);
                Expose178.Com.UtilityFactory.Log.WriteLog(hshParam, "TitleListController.GetListArticleTile", UtilityFactory.LogType.LogToDB);
            }
            return listArticleTitle;
        }
    }
}
