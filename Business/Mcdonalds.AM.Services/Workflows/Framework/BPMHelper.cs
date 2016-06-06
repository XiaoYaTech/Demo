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
//  Filename: BPMHelper.aspx.cs
//  Description:  
//
//  Create by Victor.Huang at 2014/7/9 16:52:39. 
//  Version 1.0 
//  Victor.Huang [mailto:Victor.Huang@nttdata.com] 
// 
//  History: 
//    2014.09.22 victor.huang:Fixed the bug after changed the button text
//=================================================================
#endregion

using Mcdonalds.AM.Services.Workflows.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NTTMNC.BPM.Fx.Core;

namespace Mcdonalds.AM.Services.Workflows
{
    /// <summary>
    /// BPMHelper
    /// </summary>
    public partial class BPMHelper
    {
        /// <summary>
        /// Convert to process action.
        /// </summary>
        /// <param name="actionLower">The action lower.</param>
        /// <returns>ProcessActionResult.</returns>
        public static ProcessActionResult ConvertToProcAction(string actionLower)
        {
            actionLower = actionLower.ToLower();

            // 2014.09.22 victor.huang:Fixed the bug after changed the button text
            ProcessActionResult _action = ConvertHelper.ToEnumType<ProcessActionResult>(actionLower, ProcessActionResult.Unknown);

            switch (actionLower)
            {
                case "resubmit":
                    _action = ProcessActionResult.Resubmit;
                    break;
                case "submit":
                    _action = ProcessActionResult.Approve;
                    break;
                case "decline":
                    _action = ProcessActionResult.Decline;
                    break;
                case "return":
                    _action = ProcessActionResult.Return;
                    break;
                case "confirm":
                    _action = ProcessActionResult.Confirm;
                    break;
            }

            return _action;
        }
    }
}