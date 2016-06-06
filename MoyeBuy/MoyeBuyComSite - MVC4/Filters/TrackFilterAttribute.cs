using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MoyeBuy.Com.MoyeBuyComSite.Filters
{
    /// <summary>
    /// 点击统计过滤器(所有页面控制器都加,点击的ajax事件都加,加载的ajax不加)
    /// </summary>
    public class TrackFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //HttpRequestBase request = filterContext.RequestContext.HttpContext.Request;

            //HttpCookie cookie = request.Cookies["absvisitor"];
            //int wbvId = -1;
            //if (cookie != null && cookie.Values["MEM_WBV_ID"] != null)
            //{
            //    wbvId = Res.Abs2.Library.WebUtility.InputInt(cookie.Values["MEM_WBV_ID"].ToString());
            //}
            //else
            //{
            //    wbvId = AddVisitor(request, new Res.Abs2.BLL.UserBLL().GetAnonymousId().ToString());
            //}
            //Res.Abs2.BLL.WebTrackBLL trackBll = new Res.Abs2.BLL.WebTrackBLL();
            //Res.Abs2.Model.BWebActionObj action = new Res.Abs2.Model.BWebActionObj();
            //action.WBA_END_DT = DateTime.MinValue;
            //action.WBA_OUT_FLAG = false;
            //action.WBA_PATH = request.Url.ToString();
            //action.WBA_START_DT = DateTime.Now;
            //action.WBA_WBV_ID = wbvId;
            //action.WBA_XAXIS = 0;
            //action.WBA_YAXIS = 0;
            //int wbaId = -1;
            //trackBll.AddWebAction(action, out wbaId);
        }

        //private int AddVisitor(HttpRequestBase Request, string cookie)
        //{
        //    Res.Abs2.BLL.WebTrackBLL trackBll = new Res.Abs2.BLL.WebTrackBLL();
        //    //int source = WebUtility.InputInt(Request.QueryString["src"]);//来源
        //    string source = Request.QueryString["src"];
        //    int wsrId = -1;
        //    if (string.IsNullOrEmpty(source) == false)
        //    {
        //        wsrId = trackBll.GetWebSourceIdByCode(source);
        //    }
        //    int active = Res.Abs2.Library.WebUtility.InputInt(Request.QueryString["cam"]);//活动
        //    int wbvId = 0;
        //    string fromPath = Request.UrlReferrer == null ? "" : Request.UrlReferrer.ToString();
        //    string remoteAddress = Request.ServerVariables.Get("Remote_Addr").ToString();
        //    string remoteHost = Request.ServerVariables.Get("Remote_Host").ToString();
        //    string remotePlatform = Request.Browser.Platform;
        //    string remoteBrowser = Request.Browser.Browser + ',' + Request.Browser.MajorVersion;
        //    string remoteIp = Res.Abs2.Library.WebUtility.GetIP();
        //    Res.Abs2.Model.BWebVisitorObj visitor = new Res.Abs2.Model.BWebVisitorObj();
        //    visitor.WBV_BROWSER = remoteBrowser;
        //    visitor.WBV_COOKIE = cookie;
        //    visitor.WBV_FROM_PATH = fromPath;
        //    visitor.WBV_IP = remoteIp;
        //    visitor.WBV_KEYWORD = string.IsNullOrEmpty(fromPath) ? "" : GetSearchEngineKeyword(fromPath);
        //    visitor.WBV_LOGIN_DT = DateTime.Now;
        //    visitor.WBV_LOGOUT_DT = DateTime.MinValue;
        //    visitor.WBV_MONITOR = "";//?
        //    visitor.WBV_OUT_PATH = "";
        //    visitor.WBV_REGION = "";//?
        //    visitor.WBV_START_PATH = Request.Url.ToString();
        //    visitor.WBV_WCA_ID = active;
        //    visitor.WBV_WSR_ID = wsrId;
        //    trackBll.AddWebVisitor(visitor, out wbvId);
        //    return wbvId;
        //}


    }
}