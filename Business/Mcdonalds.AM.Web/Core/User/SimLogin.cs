//================================================================= 
//  Copyright (C) 2014 NTT DATA Inc All rights reserved. 
//     
//  The information contained herein is confidential, proprietary 
//  to NTT DATA Inc. Use of this information by anyone other than 
//  authorized employees of NTT DATA Inc is granted only under a 
//  written non-disclosure agreement, expressly prescribing the 
//  scope and manner of such use. 
//================================================================= 
//  Filename: SimLogin.aspx.cs
//  Description:  
//
//  Create by Victor.Huang at 2014/7/2 13:07:45. 
//  Version 1.0 
//  Victor.Huang [mailto:Victor.Huang@nttdata.com] 
// 
//  History: 
// 
//=================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.Web.Core.Common;
using System.Text;

namespace Mcdonalds.AM.Web.Core.User
{
    /// <summary>
    /// SimLogin
    /// </summary>
    public partial class SimLogin
    {

        McdAMEntities db = new McdAMEntities();

        /// <summary>
        /// 当前登录用户
        /// </summary>
        public static Employee CurrentUser
        {
            get
            {

                if (HttpContext.Current.Session["_TempUser"] == null)
                {
                    HttpContext.Current.Session["_TempUser"] = GetCurrentUser();
                    HttpContext.Current.Session.Timeout = 24 * 60;
                }
                else
                {
                    string eid = (HttpContext.Current.Session["_TempUser"] as Employee).Code;
                    if (!string.IsNullOrEmpty(HttpContext.Current.Request["user-id"]))
                    {
                        string duid = HttpContext.Current.Request["user-id"];
                        string uid = Cryptography.Decrypt(duid, DateTime.Now.ToString("yyyyMMdd"), "oms");
                        if (uid == null)
                        {
                            uid = Cryptography.Decrypt(duid, DateTime.Now.AddDays(-1).ToString("yyyyMMdd"), "oms");
                        }
                        if (eid != uid)
                        {
                            HttpContext.Current.Session["_TempUser"] = GetCurrentUser();
                            HttpContext.Current.Session.Timeout = 24 * 60;
                        }
                    }
                }
                return HttpContext.Current.Session["_TempUser"] as Employee;
            }
        }

        public static void ClearUser()
        {
            HttpContext.Current.Session["_TempUser"] = null;
        }

        private static Employee GetCurrentUser()
        {
            if (!string.IsNullOrEmpty(HttpContext.Current.Request["user-id"]))
            {
                string duid = HttpContext.Current.Request["user-id"];
                string uid = Cryptography.Decrypt(duid, DateTime.Now.ToString("yyyyMMdd"), "oms");
                if (uid == null)
                {
                    uid = Cryptography.Decrypt(duid, DateTime.Now.AddDays(-1).ToString("yyyyMMdd"), "oms");
                }
                //uid = HttpUtility.UrlDecode(duid, Encoding.UTF8); // for testing
                if (uid == null)
                {
                    HttpContext.Current.Response.Write("登录错误");
                    HttpContext.Current.Response.End();
                    return null;
                }

                var user = Employee.Bll.Search(u => u.Code == uid).ToList();
                if (user.Count > 0)
                {
                    return user[0];
                }
                else
                {
                    HttpContext.Current.Response.Write("找不到该用户");
                    HttpContext.Current.Response.End();
                    return null;
                }
            }
            else
            {
                HttpContext.Current.Response.Write("系统登录超时，请重新登录");
                HttpContext.Current.Response.End();
                return null;
            }
        }
    }
}