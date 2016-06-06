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
    public class ProxyArticleTitle
    {
        private static readonly int IntArticleTitleDuration = Convert.ToInt32(ConfigurationManager.AppSettings["ArticleTitleCacheDuration"]);
        private static readonly bool IsEnableCache = Convert.ToBoolean(ConfigurationManager.AppSettings["EnableCache"]);
        public static IList<ArticleTitle> GetArticleTitle(string strAritcleTypeCode, string strUpdatedByUserID,string strSortField, bool bIsAsc, bool IsReturnAll, int intPageSize, int intPageIndex)
        {
            IList < ArticleTitle> listArtTitle = null;
            Expose178.Com.BLL.ArticleTitle bllArtTitle = null;
            try
            {
                bllArtTitle = new BLL.ArticleTitle();
                bllArtTitle.AritcleTypeCode = strAritcleTypeCode;
                bllArtTitle.IsAsc = bIsAsc;
                bllArtTitle.IsReturnAll = IsReturnAll;
                bllArtTitle.PageIndex = intPageIndex;
                bllArtTitle.PageSize = intPageSize;
                bllArtTitle.SortField = strSortField;
                bllArtTitle.UpdatedByUserID = strUpdatedByUserID;
                if (IsEnableCache)
                {
                    if (System.Web.HttpRuntime.Cache["articletitle"] == null)
                    {
                        listArtTitle = bllArtTitle.GetArticleTitle();
                        AggregateCacheDependency dependency = Expose178.Com.CacheDependencyFactory.DependencyFacade.GetArticleTitleDependency();
                        if (bllArtTitle != null)
                            System.Web.HttpRuntime.Cache.Add("articletitle", bllArtTitle, dependency, DateTime.Now.AddSeconds(IntArticleTitleDuration), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
                    }
                    if (listArtTitle == null)
                        listArtTitle = (List<ArticleTitle>)System.Web.HttpRuntime.Cache["articletitle"];
                }
                else
                    listArtTitle = bllArtTitle.GetArticleTitle();
            }
            catch (EvaluateException ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("Error", ex.Message);
                hshParam.Add("UID", Expose178.Com.GadgetScripts.Gadget.GetUserID());
                Expose178.Com.UtilityFactory.Log.WriteLog(hshParam, "ProxyArticle.GetArticleTitle", UtilityFactory.LogType.LogToDB);
            }
            return listArtTitle;
        }
        public static IList<ArticleTitle> GetArticleTitle()
        {
            IList<ArticleTitle> listArtTitle = null;
            Expose178.Com.BLL.ArticleTitle bllArtTitle = null;
            try
            {
                bllArtTitle = new BLL.ArticleTitle();
                if (IsEnableCache)
                {
                    if (System.Web.HttpRuntime.Cache["articletitle"] == null)
                    {
                        listArtTitle = bllArtTitle.GetArticleTitle();
                        AggregateCacheDependency dependency = Expose178.Com.CacheDependencyFactory.DependencyFacade.GetArticleTitleDependency();
                        if (bllArtTitle != null)
                            System.Web.HttpRuntime.Cache.Add("articletitle", bllArtTitle, dependency, DateTime.Now.AddSeconds(IntArticleTitleDuration), System.Web.Caching.Cache.NoSlidingExpiration, System.Web.Caching.CacheItemPriority.Normal, null);
                    }
                    if (listArtTitle == null)
                        listArtTitle = (List<ArticleTitle>)System.Web.HttpRuntime.Cache["articletitle"];
                }
                else
                    listArtTitle = bllArtTitle.GetArticleTitle();
            }
            catch (EvaluateException ex)
            {
                Hashtable hshParam = new Hashtable();
                hshParam.Add("Error", ex.Message);
                hshParam.Add("UID", Expose178.Com.GadgetScripts.Gadget.GetUserID());
                Expose178.Com.UtilityFactory.Log.WriteLog(hshParam, "ProxyArticle.GetArticleTitle", UtilityFactory.LogType.LogToDB);
            }
            return listArtTitle;
        }
    }
}