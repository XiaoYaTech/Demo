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
//  Filename: ClosureTool.aspx.cs
//  Description:  
//
//  Create by Victor.Huang at 2014/7/9 15:17:05. 
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
    /// ClosureTool
    /// </summary>
    public partial class WFClosureTool
    {
        #region ---- [ Constant Strings ] ----

        public const string ProcessName = @"MCDAMK2Project\ClosureTool";
        public const string ProcessCode = @"MCD_AM_Closure_Tool";
        public const string TableName = "ClosureTool";

        public const string Act_Originator = "Originator"; // 退回至发起人节点名
        public const string Act_AssetActor = "Asset Actor"; // Actor 节点名
        #endregion
    }
}