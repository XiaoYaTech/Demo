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
//  Filename: TestK2CallbackInterface.cs
//  Description:  Class for test the interface of k2 callback
//
//  Create by victor.huang at 2014/08/15 16:13. 
//  Version 1.0 
//  victor.huang [mailto:victor.huang@nttdata.com] 
// 
//  History: 
// 
//=================================================================
#endregion

using System;
using System.Collections.Generic;
using NTTMNC.BPM.Fx.K2.Extention.McdAM.Entity;
using NUnit.Framework;
using System.Configuration;
using System.Collections.Specialized;
using Newtonsoft.Json;
using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.Core.SysEntity;
using Mcdonalds.AM.ApiCaller;


namespace Mcdonalds.AM.UnitTest.Controllers.K2Callback
{
    [TestFixture]
    public class TestK2CallbackInterface
    {
        string apiHost = ConfigurationManager.AppSettings["ApiHost"];

        #region SetUp / TearDown

        [SetUp]
        public void Init()
        { }

        [TearDown]
        public void Dispose()
        { }

        #endregion

        #region Tests

        [Test]
        public void TestFlowCompleted()
        {
            string url = apiHost + @"api/k2callback/flowcompleted";

            //url = @"http://172.24.130.43:10083/api/flow/topnav";

            NameValueCollection _queryStrings = new NameValueCollection();
            _queryStrings.Add("procInstID", "644");
            _queryStrings.Add("flowStatus", "0");



            string _result = ApiProxy.Call(url, "GET", _queryStrings, null);

            
            if (!string.IsNullOrEmpty(_result))
            {
                // Formatting JSON String
                _result = _result.Trim('"').Replace("\\\"", "\"");
            }

            string _cd = "{\"ReturnData\":\"truefdsf\"}";
            Console.WriteLine("Result 1: {0}", _cd);
            Console.WriteLine("Result 2: {0}", _result);
            //MCDAMReturnObject _returnAMObject = JsonConvert.DeserializeObject<MCDAMReturnObject>(_cd);

            MCDAMReturnObject _returnAMObject = JsonConvert.DeserializeObject<MCDAMReturnObject>(_result);
            //Assert.AreEqual(_cd, _result);
            Console.WriteLine("Result: {0}", JsonConvert.SerializeObject(_result));

            /*
            string _result = ApiProxy.Call(url, "GET", _queryStrings, null);
            Console.WriteLine("Result: {0}", _result);

            MCDAMReturnObject _returnAMObject = JsonConvert.DeserializeObject<MCDAMReturnObject>(_result);

            Console.WriteLine("MCDAMReturnObject: {0}", JsonConvert.SerializeObject(_returnAMObject));*/
        }


        [Test]
        public void TestAddProjectComment()
        {
            string url = apiHost + @"api/k2callback/AddProjectComment";

            IList<WebApiQueryParam> _queryParams = new List<WebApiQueryParam>();
            _queryParams.Add(new WebApiQueryParam("procInstID", "884"));
            _queryParams.Add(new WebApiQueryParam("k2SN", "884_35"));
            _queryParams.Add(new WebApiQueryParam("operatorID", "E5016672"));
            _queryParams.Add(new WebApiQueryParam("action", "Approve"));
            _queryParams.Add(new WebApiQueryParam("comment", ConvertHelper.Base64Encode("Test Approve Comment")));

            var _isExecuted = CallAMAPI(url, _queryParams, "POST");

            Console.WriteLine("Call API Result: {0}", _isExecuted);
        }


        [Test]
        public void TestSyncExpiredTask()
        {
            string url = Formatter.CombinePath(apiHost, @"api/k2callback/syncexpiredtask");

            IList<WebApiQueryParam> _queryParams = new List<WebApiQueryParam>();
            _queryParams.Add(new WebApiQueryParam("procInstID", "6444"));
            _queryParams.Add(new WebApiQueryParam("originalTaskID", "CAAFA419-0EDD-4CAA-BD9C-2FDD2E66E016"));

            MCDAMReturnObject _returnAMObject = WebApiProxy.Call<MCDAMReturnObject>(url, _queryParams, "POST");

            Console.WriteLine("Result: {0}", JsonConvert.SerializeObject(_returnAMObject, Formatting.Indented));
        }

        [Test]
        public void TestPrepareTask()
        {
            string url = Formatter.CombinePath(apiHost, @"api/k2callback/preparetask");

            IList<WebApiQueryParam> _queryParams = new List<WebApiQueryParam>();
            _queryParams.Add(new WebApiQueryParam("procInstID", "1104"));
            _queryParams.Add(new WebApiQueryParam("taskID", "CAAFA419-0EDD-4CAA-BD9C-2FDD2E66E016"));
            _queryParams.Add(new WebApiQueryParam("taskURL", ConvertHelper.Base64Encode("/MajorLease/Main#/MajorLease/ConsInvtChecking/Process/View?ProcInstID=1104")));
            _queryParams.Add(new WebApiQueryParam("receiverID", "E5006154"));
            _queryParams.Add(new WebApiQueryParam("creater", "bpm.service"));
            _queryParams.Add(new WebApiQueryParam("k2SN", "1104_24"));
            _queryParams.Add(new WebApiQueryParam("activity", ConvertHelper.Base64Encode("Supervisor Approval")));

            MCDAMReturnObject _returnAMObject = WebApiProxy.Call<MCDAMReturnObject>(url, _queryParams, "POST");

            Console.WriteLine("Result: {0}", JsonConvert.SerializeObject(_returnAMObject, Formatting.Indented));
        }

        /// <summary>
        /// Call the mcdonalds AM api.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="queryParams">The query parameters.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        private bool CallAMAPI( string url, IList<WebApiQueryParam> queryParams = null, string httpMethod = "GET" )
        {
            bool _isExecuted = false;
            MCDAMReturnObject _returnAMObject = WebApiProxy.Call<MCDAMReturnObject>(url, queryParams, httpMethod);

            if (_returnAMObject != null)
            {
                if (!string.IsNullOrEmpty(_returnAMObject.ReturnData))
                {
                    _isExecuted = ConvertHelper.ToBool(_returnAMObject.ReturnData);
                }

                if (_isExecuted == false && !string.IsNullOrEmpty(_returnAMObject.ErrorMessage))
                {
                    LogMCDAMApiException(_returnAMObject, true);
                }
            }

            return _isExecuted;
        }

        /// <summary>
        /// Log the mcdonalds AM API exception.
        /// </summary>
        /// <param name="apiRetAMObject">The API ret am object.</param>
        /// <param name="isNeedThrowException">if set to <c>true</c> [is need throw exception].</param>
        /// <exception cref="System.Exception"></exception>
        private void LogMCDAMApiException( MCDAMReturnObject apiRetAMObject, bool isNeedThrowException = false )
        {
            if (!string.IsNullOrEmpty(apiRetAMObject.ErrorMessage))
            {
                string _msgTemplate = @"ErrorMessage:  {0} \r\n
                                        InnerErrorMessage:  {1} \r\n
                                        StackTrace:  {2}";
                _msgTemplate = string.Format(_msgTemplate, apiRetAMObject.ErrorMessage, apiRetAMObject.InnerErrorMessage, apiRetAMObject.StackTrace);
                Log4netHelper.WriteErrorLog(_msgTemplate);

                if (isNeedThrowException)
                {
                    throw new Exception(_msgTemplate);
                }
            }
        }
        #endregion
    }
}
