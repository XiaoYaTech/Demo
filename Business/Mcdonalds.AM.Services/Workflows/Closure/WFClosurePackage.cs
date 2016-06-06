#region ---- [ Copyright ] ----
//================================================================= 
//  Copyright (C) 2014 NTT DATA Inc All rights reserved. 
//     
//  The information contained herein is confidential, proprietary 
//  to NTT DATA Inc. Use of this information by anyone other than 
//  authorized employees of NTT DATA Inc is granted only under a 
//  written non-disclosure agreement, expressly prescribing the 
//  scope and manner of such use. 
//================================================================= 
//  Filename: ClosurePackage.aspx.cs
//  Description:  
//
//  Create by Victor.Huang at 2014/7/9 15:18:23. 
//  Version 1.0 
//  Victor.Huang [mailto:Victor.Huang@nttdata.com] 
// 
//  History: 
// 
//=================================================================
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.Services.Workflows.Closure
{
    /// <summary>
    /// ClosurePackage
    /// </summary>
    public partial class WFClosurePackage
    {
        #region ---- [ Constant Strings ] ----

        public const string ProcessName = @"MCDAMK2Project\ClosurePackage";
        public const string ProcessCode = @"MCD_AM_Closure_Package";
        public const string TableName = "ClosurePackage";

        public const string Act_Originator = "Originator"; // 退回至发起人节点名
        #endregion

        // 一般审批人节点，不能取消流程
        public static string[] NormalActivities = {
                                           Act_Originator,
                                           "Market Manager",
                                           "DD_GM_FC_RDD",//要支持在途流程
                                           "DD_GM_FC"
                                       };
    }
}