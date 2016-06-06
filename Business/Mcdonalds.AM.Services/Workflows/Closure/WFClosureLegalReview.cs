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
//  Filename: ClosureLegalReview.aspx.cs
//  Description:  
//
//  Create by Victor.Huang at 2014/7/9 15:20:10. 
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
    /// ClosureLegalReview
    /// </summary>
    public partial class WFClosureLegalReview
    {
        #region ---- [ Constant Strings ] ----

        public const string ProcessName = @"MCDAMK2Project\ClosurePackage";
        public const string ProcessCode = @"MCD_AM_Closure_LR";
        public const string TableName = "ClosureLegalReview";

        public const string Act_Originator = "Originator"; // 退回至发起人节点名
        #endregion
    }
}