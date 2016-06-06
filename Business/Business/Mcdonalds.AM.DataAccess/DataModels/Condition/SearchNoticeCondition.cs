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
//  Filename: SearchNoticeCondition.aspx.cs
//  Description:  
//
//  Create by Victor.Huang at 2014/8/25 11:18:27. 
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
using System.Runtime.Serialization;

namespace Mcdonalds.AM.DataAccess.DataModels.Condition
{
    /// <summary>
    /// SearchNoticeCondition
    /// </summary>
    public partial class SearchNoticeCondition
    {
        public string ProcessId { get; set; }
        public string Title { get; set; }
        public string SenderName { get; set; }
        public string Receiver { get; set; }
        public string StoreCode { get; set; }

        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
    }
}