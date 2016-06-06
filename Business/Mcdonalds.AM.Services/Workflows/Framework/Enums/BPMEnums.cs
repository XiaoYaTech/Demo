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
//  Filename: BPMEnums.aspx.cs
//  Description:  
//
//  Create by Victor.Huang at 2014/7/9 16:57:03. 
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

/// <summary>
/// The Enums namespace.
/// </summary>
namespace Mcdonalds.AM.Services.Workflows.Enums
{
    /// <summary>
    /// Enum ProcessActions
    /// </summary>
    public enum ProcessActionResult
    {
        /// <summary>
        /// The unknown. 未知Action
        /// </summary>
        Unknown,

        /// <summary>
        /// The approve.同意
        /// </summary>
        Approve,

        /// <summary>
        /// The return.退回单
        /// </summary>
        Return,

        /// <summary>
        /// The resubmit.重新提交
        /// </summary>
        Resubmit,

        /// <summary>
        /// The decline.拒绝流程
        /// </summary>
        Decline,

        Confirm
    }
}