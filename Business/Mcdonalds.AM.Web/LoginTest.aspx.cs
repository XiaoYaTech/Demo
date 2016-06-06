
//================================================================= 
//  Copyright (C) 2014 NTT DATA Inc All rights reserved. 
//     
//  The information contained herein is confidential, proprietary 
//  to NTT DATA Inc. Use of this information by anyone other than 
//  authorized employees of NTT DATA Inc is granted only under a 
//  written non-disclosure agreement, expressly prescribing the 
//  scope and manner of such use. 
//================================================================= 
//  Filename: LoginTest.aspx.cs
//  Description:  
//
//  Create by victor.huang at 2014/07/02 13:11. 
//  Version 1.0 
//  victor.huang [mailto:victor.huang@nttdata.com] 
// 
//  History: 
// 
//=================================================================
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Beyondbit.XHCultureOA.Utility.WebUtility;
using Mcdonalds.AM.Web.Core;

namespace Mcdonalds.AM.Web
{
    public partial class LoginTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {


        }


        protected void btn_Click(object sender, EventArgs e)
        {
            string uid = tb1.Text.Trim();
            string s = Cryptography.Encrypt(uid, DateTime.Now.ToString("yyyyMMdd"), "oms");
            UserInfo userInfo = McdAMContext.Authenticate(s);//Stephen Modified

            if (userInfo != null)
            {
                Response.Redirect("PortalTest.aspx?user-id=" + s);
            }

        }

    }
}