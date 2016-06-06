using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Configuration;
using System.Data;
using System.Collections;
using Expose178.Com.Model;

namespace Expose178.Com.Expose178Site.Proxys
{
    public class ProxyArticle
    {
        private static readonly int IntArticleDuration = Convert.ToInt32(ConfigurationManager.AppSettings["ArticleCacheDuration"]);
        private static readonly bool IsEnableCache = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableCache"]);
        public static Expose178.Com.Model.Article GetArticle(string strArticleID)
        {
            Article article = null;
            try
            {
                Expose178.Com.BLL.Article bllArticle = new BLL.Article();
                if (IsEnableCache)
                {
                    if (System.Web.HttpRuntime.Cache["article"] == null)
                    {
                        article = bllArticle.GetArticle(strArticleID);
                        AggregateCacheDependency dependency = Expose178.Com.CacheDependencyFactory.DependencyFacade.GetArticleDependency();
                        if (article != null)
                            System.Web.HttpRuntime.Cache.Add("article", article, dependency, DateTime.Now.AddSeconds(IntArticleDuration), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
                    }
                    else
                        article = (Expose178.Com.Model.Article)System.Web.HttpRuntime.Cache["article"];
                }
                else
                    article = bllArticle.GetArticle(strArticleID);
                Expose178.Com.Model.ArticleAddtional addtinal = bllArticle.GetArticleAddtional(strArticleID);
                bllArticle.UpdateArticleAddtional(addtinal);
            }
            catch (Exception ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("Error",ex.Message);
                hshParam.Add("UID", Expose178.Com.GadgetScripts.Gadget.GetUserID());
                Expose178.Com.UtilityFactory.Log.WriteLog(hshParam, "ProxyArticle.GetArticle", UtilityFactory.LogType.LogToDB);
            }
            
            return article;
        }
        public static bool AddUpdateArticle(Expose178.Com.Model.Article article)
        {
            Expose178.Com.BLL.Article bllArticle = new BLL.Article();
            return bllArticle.AddUpdateArticle(article);
        }
        
    }
}