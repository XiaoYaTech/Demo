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
//  Filename: WFConsInvtChecking.aspx.cs
//  Description:  
//
//  Create by Victor.Huang at 2014/7/30 4:41:08. 
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
    /// WFConsInvtChecking 流程配置关联项
    /// </summary>
    public partial class WFConsInvtChecking
    {
        #region ---- [ Constant Strings ] ----

        public const string ProcessName = @"MCDAMK2Project\ClosureConsInvtChecking";
        public const string ProcessCode = @"MCD_AM_Closure_CIC";
        public const string TableName = "ClosureConsInvtChecking";


        public const string Act_Originator = "Originator"; // 退回至发起人节点名
        #endregion

    }
}