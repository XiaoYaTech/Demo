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
//  Filename: MCDAMReturnObject.aspx.cs
//  Description:  
//
//  Create by Victor.Huang at 2014/8/18 11:29:19. 
//  Version 1.0 
//  Victor.Huang [mailto:Victor.Huang@nttdata.com] 
// 
//  History: 
// 
//=================================================================
#endregion

using Newtonsoft.Json;
using System;
using System.Runtime.Serialization;

namespace Mcdonalds.AM.Services.Entities
{
    /// <summary>
    /// MCDAMReturnObject
    /// </summary>
    public partial class MCDAMReturnObject
    {
        /// <summary>
        /// ReturnData
        /// 返回数据，如果是多条记录用;分割
        /// </summary>
        [DataMember]
        public string ReturnData { get; set; }

        /// <summary>
        /// 错误消息
        /// </summary>
        [DataMember]
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 内部错误消息
        /// </summary>
        [DataMember]
        public string InnerErrorMessage { get; set; }

        /// <summary>
        /// 堆栈消息
        /// </summary>
        [DataMember]
        public string StackTrace { get; set; }


        #region ---- [ Constructors ] ----
        //[JsonIgnore]
        public MCDAMReturnObject()
        {

        }

        public MCDAMReturnObject(string retData)
        {
            ReturnData = retData;
        }

        /// <summary>
        /// Initialize a new instance of the <see cref="ServiceReturnObject" /> class.
        /// </summary>
        /// <param name="retData">The return data.返回数据</param>
        /// <param name="errorMessage">The error message.</param>
        /// <param name="innerErrorMessage">The inner error message.</param>
        /// <param name="stackTrace">The stack trace.</param>
        public MCDAMReturnObject(string retData, string errorMessage, string innerErrorMessage, string stackTrace)
        {
            ReturnData = retData;
            ErrorMessage = errorMessage;
            InnerErrorMessage = innerErrorMessage;
            StackTrace = stackTrace;
        }
        #endregion

    }

}