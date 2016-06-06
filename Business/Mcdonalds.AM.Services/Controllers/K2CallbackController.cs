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
//  Filename: K2CallbackController.aspx.cs
//  Description:  
//
//  Create by Victor.Huang at 2014/8/15 15:10:40. 
//  Version 1.0 
//  Victor.Huang [mailto:Victor.Huang@nttdata.com] 
// 
//  History: 
//  2014-9-01 victor.haung: 添加K2 Activity Name
//=================================================================
#endregion

using System;
using System.Linq;
using System.Transactions;
using System.Web.Http;

using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Entities;
using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.K2.BLL;
using System.Web;
using Newtonsoft.Json;
using System.Data.Entity;
using NTTMNC.BPM.Fx.K2.Services;
using System.Collections.Generic;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.Services.EmailServiceReference;


namespace Mcdonalds.AM.Services.Controllers
{
    /// <summary>
    /// K2CallbackController
    /// </summary>
    public partial class K2CallbackController : ApiController
    {

        /// <summary>
        /// 流程结束并返回值
        /// </summary>
        /// <param name="procInstID">The process instance identifier.</param>
        /// <param name="flowStatus">The flow status.流程通过状态, 100 - 拒绝  200- 审批通过</param>
        /// <returns>IHttpActionResult.</returns>
        [Route("api/k2callback/flowcompleted")]
        [HttpPost]
        [HttpGet]
        public IHttpActionResult FlowCompleted(string procInstID, string flowStatus)
        {
            // Log
            Log4netHelper.WriteInfo(string.Format("procInstID:{0}, flowStatus:{1}", procInstID, flowStatus));

            MCDAMReturnObject _returnAMObject = new MCDAMReturnObject(false.ToString());

            using (TransactionScope tranScope = new TransactionScope(TransactionScopeOption.Required, new TimeSpan(0, 1, 0)))
            {
                int _intProcInstID = ConvertHelper.ToInt(procInstID);
                int _intFlowStatus = ConvertHelper.ToInt(flowStatus);
                var _enumFlowStatus = ConvertHelper.ToEnumType<TaskWorkStatus>(flowStatus);

                if (!string.IsNullOrEmpty(procInstID) && _intProcInstID > 0)
                {
                    try
                    {
                        var task = TaskWork.Search(c => c.ProcInstID == _intProcInstID).OrderByDescending(o => o.Num).FirstOrDefault();

                        if (task == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            if (_intProcInstID > 0 && _intFlowStatus > 0)
                            {
                                // 更新Task 状态
                                task.Status = ConvertHelper.ToEnumType<TaskWorkStatus>(_intFlowStatus);
                                int _result = TaskWork.Update(task);

                            }
                            Log4netHelper.WriteInfo("Task details: " + JsonConvert.SerializeObject(task));
                            //
                            // 在这里写其它逻辑  
                            //
                            try
                            {
                                var wfEntity = BaseWFEntity.GetWorkflowEntity(task.RefID, task.TypeCode);
                                if (wfEntity != null)
                                {
                                    wfEntity.Finish(_enumFlowStatus, task);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log4netHelper.WriteError(
                                    JsonConvert.SerializeObject(new { Desc = "[Workflow finish method error]", ex }));
                                throw ex;
                            }

                            Mcdonalds.AM.Services.Common.ActionLogType action = Common.ActionLogType.Approve;
                            var notificationModule = new Mcdonalds.AM.Services.Common.MailHelper.NotificationModule();
                            EmailSendingResultType result = null;
                            EmailMessage emailMessage = null;
                            // 在这里写其它逻辑  
                            switch (_enumFlowStatus)
                            {
                                case TaskWorkStatus.K2ProcessDeclined:
                                    Log4netHelper.WriteError("api/k2callback/flowcompleted---K2ProcessDeclined开始回调邮件接口:" +
                                                               DateTime.Now.ToString());
                                    action = Mcdonalds.AM.Services.Common.ActionLogType.Decline;
                                    emailMessage = MailHelper.BuildEmailMessage(task, ref action, ref notificationModule);
                                    if (emailMessage == null)
                                    {
                                        Log4netHelper.WriteError("K2ProcessDeclined:Can't build EmailMessage");
                                    }
                                    else
                                        result = MailHelper.SendCommentsEmail(emailMessage);
                                    if (result != null)
                                    {
                                        if (result.Successful)
                                        {
                                            Log4netHelper.WriteError("api/k2callback/flowcompleted发送邮件成功; " + "邮件接收人:" +
                                                                       emailMessage.To);
                                        }
                                        else
                                        {
                                            Log4netHelper.WriteError("api/k2callback/flowcompleted发送邮件失败; " + "邮件接收人:" +
                                                                       emailMessage.To + "; 错误信息:/r/n" +
                                                                       (string.IsNullOrEmpty(result.ErrorMessage)
                                                                           ? string.Empty
                                                                           : result.ErrorMessage) + "/r/n" +
                                                                       (string.IsNullOrEmpty(result.InnerErrorMessage)
                                                                           ? string.Empty
                                                                           : result.InnerErrorMessage) +
                                                                       (string.IsNullOrEmpty(result.StackTrace)
                                                                           ? string.Empty
                                                                           : result.StackTrace));
                                        }
                                    }
                                    if (task.ProcInstID != null)
                                    {
                                        //通过K2每添加一条任务，都会回调一次，但是提醒邮件只在添加第一条任务时发送
                                        if (
                                            TaskWork.Count(
                                                t =>
                                                    t.ProcInstID == task.ProcInstID && t.RefID == task.RefID &&
                                                    t.Status == TaskWorkStatus.UnFinish) == 1)
                                        {
                                            string emails = string.Empty;
                                            //通知流程相关人以及抄送人
                                            emails = MailHelper.GetWorkflowRelationUserEmails(task.RefID, task.TypeCode,
                                                notificationModule);
                                            //发送备注提醒邮件
                                            //var emailTo = MailHelper.GetNotifyUserEmails(task.K2SN);
                                            //if (!string.IsNullOrEmpty(emailTo))
                                            //    emails += emailTo;

                                            if (!string.IsNullOrEmpty(emails))
                                            {
                                                emailMessage.To = emails;
                                                var result2 = MailHelper.SendCommentsEmail(emailMessage);
                                                if (result2 != null)
                                                {
                                                    if (result.Successful)
                                                    {
                                                        Log4netHelper.WriteError(
                                                            "api/k2callback/flowcompleted发送提醒邮件成功; " + "邮件接收人:" +
                                                            emailMessage.To);
                                                    }
                                                    else
                                                    {
                                                        Log4netHelper.WriteError(
                                                            "api/k2callback/flowcompleted发送提醒邮件失败; " + "邮件接收人:" +
                                                            emailMessage.To + "; 错误信息:/r/n" +
                                                            (string.IsNullOrEmpty(result.ErrorMessage)
                                                                ? string.Empty
                                                                : result.ErrorMessage) + "/r/n" +
                                                            (string.IsNullOrEmpty(result.InnerErrorMessage)
                                                                ? string.Empty
                                                                : result.InnerErrorMessage) +
                                                            (string.IsNullOrEmpty(result.StackTrace)
                                                                ? string.Empty
                                                                : result.StackTrace));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    //switch (task.TypeCode)
                                    //{
                                    //    case FlowCode.TempClosure_LegalReview:
                                    //        {
                                    //            var url = "/TempClosure/Main#/LegalReview/Process/Resubmit?ProjectId=" + task.RefID;
                                    //            var actor = bllProjectUsers.FirstOrDefault(pu => pu.ProjectId == task.RefID && pu.RoleCode == ProjectUserRoleCode.AssetActor);
                                    //            bllTask.SendTask(task.RefID, task.Title, task.StoreCode, url, actor, FlowCode.TempClosure, FlowCode.TempClosure_LegalReview, _intProcInstID);
                                    //        }
                                    //        break;
                                    //    case FlowCode.TempClosure_ClosurePackage:
                                    //        {
                                    //            var url = "/TempClosure/Main#/ClosurePackage/Process/Resubmit?projectId=" + task.RefID;
                                    //            var actor = bllProjectUsers.FirstOrDefault(pu => pu.ProjectId == task.RefID && pu.RoleCode == ProjectUserRoleCode.AssetActor);
                                    //            bllTask.SendTask(task.RefID, task.Title, task.StoreCode, url, actor, FlowCode.TempClosure, FlowCode.TempClosure_ClosureMemo);
                                    //        }
                                    //        break;
                                    //    default:
                                    //        break;
                                    //}
                                    break;
                                case TaskWorkStatus.K2ProcessApproved:
                                    switch (task.TypeCode)
                                    {
                                        case FlowCode.TempClosure_LegalReview:
                                            {
                                                var url = "/TempClosure/Main#/ClosurePackage?projectId=" + task.RefID;
                                                var actor =
                                                    ProjectUsers.FirstOrDefault(
                                                        pu =>
                                                            pu.ProjectId == task.RefID &&
                                                            pu.RoleCode == ProjectUserRoleCode.AssetActor);
                                                TaskWork.SendTask(task.RefID, task.Title, task.StoreCode, url, actor,
                                                    FlowCode.TempClosure, FlowCode.TempClosure_ClosurePackage, "Start");
                                            }
                                            break;
                                        case FlowCode.TempClosure_ClosurePackage:
                                            {
                                                var url = "/TempClosure/Main#/ClosurePackage/Process/Upload?projectId=" +
                                                          task.RefID;
                                                var actor =
                                                    ProjectUsers.FirstOrDefault(
                                                        pu =>
                                                            pu.ProjectId == task.RefID &&
                                                            pu.RoleCode == ProjectUserRoleCode.AssetActor);
                                                TaskWork.SendTask(task.RefID, task.Title, task.StoreCode, url, actor,
                                                    FlowCode.TempClosure, FlowCode.TempClosure_ClosurePackage, "Start");
                                            }
                                            break;
                                        case FlowCode.Closure_WOCheckList:
                                            {
                                                var closureToolCtrller = new ClosureToolController();
                                                closureToolCtrller.RefreshClosureTool(task.RefID);
                                            }
                                            break;
                                        //case FlowCode.Renewal_Letter:
                                        //    {
                                        //        var renewalInfo = RenewalInfo.Get(task.RefID);
                                        //        var negoProject = ProjectInfo.Get(task.RefID, FlowCode.Renewal_LLNegotiation);
                                        //        if (negoProject.NodeCode == NodeCode.Finish)
                                        //        {
                                        //            if (renewalInfo.NeedProjectCostEst)
                                        //            {
                                        //                renewalInfo.GenerateSubmitTask(FlowCode.Renewal_ConsInfo);
                                        //            }
                                        //            else
                                        //            {
                                        //                renewalInfo.GenerateSubmitTask(FlowCode.Renewal_Tool);
                                        //            }
                                        //        }
                                        //    }
                                        //    break;

                                        default:
                                            break;
                                    }

                                    try
                                    {
                                        Log4netHelper.WriteError("api/k2callback/flowcompleted开始回调邮件接口:" +
                                                                                                       DateTime.Now.ToString());
                                        Log4netHelper.WriteError("task.ProcInstID=" + task.ProcInstID + ";task.TypeCode=" +
                                                                   task.TypeCode + "; _enumFlowStatus" +
                                                                   _enumFlowStatus.ToString());
                                        action = Common.ActionLogType.Approve;
                                        notificationModule =
                                            new Mcdonalds.AM.Services.Common.MailHelper.NotificationModule();
                                        emailMessage = MailHelper.BuildEmailMessage(task, ref action, ref notificationModule);
                                        if (emailMessage == null)
                                        {
                                            Log4netHelper.WriteErrorLog("Can't build EmailMessage");
                                        }
                                        if (task.ProcInstID != null && emailMessage != null)
                                        {
                                            string emails = string.Empty;
                                            //通知流程相关人以及抄送人
                                            emails = MailHelper.GetWorkflowRelationUserEmails(task.RefID, task.TypeCode,
                                                notificationModule);
                                            //发送备注提醒邮件
                                            //var emailTo = MailHelper.GetNotifyUserEmails(task.K2SN);
                                            //if (!string.IsNullOrEmpty(emailTo))
                                            //    emails += emailTo;

                                            if (!string.IsNullOrEmpty(emails))
                                            {
                                                emailMessage.To = emails;
                                                var result2 = MailHelper.SendCommentsEmail(emailMessage);
                                                if (result2 != null)
                                                {
                                                    if (result.Successful)
                                                    {
                                                        Log4netHelper.WriteError(
                                                            "api/k2callback/flowcompleted发送提醒邮件成功; " + "邮件接收人:" +
                                                            emailMessage.To);
                                                    }
                                                    else
                                                    {
                                                        Log4netHelper.WriteError(
                                                            "api/k2callback/flowcompleted发送提醒邮件失败; " + "邮件接收人:" +
                                                            emailMessage.To + "; 错误信息:/r/n" +
                                                            (string.IsNullOrEmpty(result.ErrorMessage)
                                                                ? string.Empty
                                                                : result.ErrorMessage) + "/r/n" +
                                                            (string.IsNullOrEmpty(result.InnerErrorMessage)
                                                                ? string.Empty
                                                                : result.InnerErrorMessage) +
                                                            (string.IsNullOrEmpty(result.StackTrace)
                                                                ? string.Empty
                                                                : result.StackTrace));
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Log4netHelper.WriteError(
                                            JsonConvert.SerializeObject(new { Desc = "[Send Mail Error]", ex }));
                                    }

                                    break;
                            }
                        }
                        // 返回 true
                        _returnAMObject.ReturnData = true.ToString();
                    }
                    catch (System.Exception ex)
                    {
                        Log4netHelper.Log4netWriteErrorLog(@"k2回调 FlowCompleted 接口出错!", ex);
                        _returnAMObject = PopulateAMApiException(ex);

                    }
                }

                tranScope.Complete();
            }

            //////
            return Ok(_returnAMObject);
            //return Ok( JsonConvert.SerializeObject(_returnAMObject) );
        }

        #region ---- [ Private Methods ] ----

        /// <summary>
        /// 返回 API 报错信息.
        /// </summary>
        /// <param name="returnAMObject">The return am object.</param>
        /// <param name="ex">The exception.</param>
        private MCDAMReturnObject PopulateAMApiException(System.Exception ex)
        {
            MCDAMReturnObject returnAMObject = new MCDAMReturnObject();
            returnAMObject.ReturnData = false.ToString();
            returnAMObject.ErrorMessage = ex.Message;
            returnAMObject.InnerErrorMessage = ex.InnerException == null ? string.Empty : ex.InnerException.Message;
            returnAMObject.StackTrace = ex.StackTrace;

            return returnAMObject;
        }

        /// <summary>
        /// Decryping the base64.base64解码
        /// </summary>
        /// <param name="inputValue">The input value.</param>
        /// <returns>System.String.</returns>
        private string DecrypBase64(string inputValue)
        {
            if (GlobalCommon.IsBase64String(inputValue))
            {
                inputValue = Base64Encryptor.Decrypting(inputValue);
            }
            return inputValue;
        }

        #endregion


        /// <summary>
        /// Prepare the task.在打开任务前执行操作
        /// </summary>
        /// <param name="taskID">The task identifier.</param>
        /// <param name="taskURL">The task URL.</param>
        /// <param name="receiverID">The receiver identifier.</param>
        /// <param name="creater">The creater.</param>
        /// <param name="procInstID">The process instance identifier.</param>
        /// <param name="k2SN">The k2 sn.</param>
        /// <returns>IHttpActionResult.</returns>
        [Route("api/k2callback/preparetask")]
        [HttpPost]
        public IHttpActionResult PrepareTask()
        {
            HttpRequest _request = HttpContext.Current.Request;

            string taskID, taskURL, receiverID, creater, procInstID, k2SN, activity;

            taskID = _request["taskID"];
            taskURL = DecrypBase64(_request["taskURL"]);
            receiverID = _request["receiverID"];
            creater = _request["creater"];
            procInstID = _request["procInstID"];
            k2SN = _request["k2SN"];
            activity = DecrypBase64(_request["activity"]);

            Log4netHelper.WriteInfo(
                string.Format("InputVals:{0}",
                    JsonConvert.SerializeObject(new { taskID, taskURL, receiverID, creater, procInstID, k2SN, activity })
                    ));

            MCDAMReturnObject _returnAMObject = new MCDAMReturnObject(false.ToString());

            using (TransactionScope tranScope = new TransactionScope())
            {
                int _intProcInstID = ConvertHelper.ToInt(procInstID);


                if (!string.IsNullOrEmpty(procInstID) && _intProcInstID > 0 && !string.IsNullOrEmpty(taskID))
                {
                    // 2014.09.16 victor.huang: Allow convert task id into guid
                    Guid _taskGUID = ConvertHelper.ToGuid(taskID);

                    try
                    {
                        var _taskWork = TaskWork.Search(c => c.Id == _taskGUID).AsNoTracking().FirstOrDefault();

                        if (_taskWork == null)
                        {
                            return NotFound();
                        }
                        else
                        {
                            // 新建流程任务数据
                            var _empReceiver = Employee.Search(o => o.Code.Contains(receiverID) || o.Account.Contains(receiverID)).FirstOrDefault();

                            _taskWork.Id = Guid.NewGuid();
                            _taskWork.Url = taskURL;
                            _taskWork.Status = TaskWorkStatus.UnFinish; //0 - 收到任务的初始状态  2- 执行过的状态
                            _taskWork.CreateTime = DateTime.Now;
                            _taskWork.FinishTime = null;
                            _taskWork.CreateUserAccount = creater;// K2流程后台管理员帐号
                            _taskWork.ProcInstID = _intProcInstID;
                            _taskWork.K2SN = k2SN;
                            _taskWork.ActivityName = activity; // 2014-9-01 victor.haung
                            _taskWork.ActionName = null;

                            if (_empReceiver != null && !string.IsNullOrEmpty(_empReceiver.Code))
                            {
                                _taskWork.ReceiverAccount = _empReceiver.Code;
                                _taskWork.ReceiverNameZHCN = _empReceiver.NameZHCN;
                                _taskWork.ReceiverNameENUS = _empReceiver.NameENUS;
                            }
                            else if (!string.IsNullOrEmpty(receiverID))
                            {
                                // 如果找不到人员直接传Receiver ID
                                _taskWork.ReceiverAccount = receiverID;
                                _taskWork.ReceiverNameZHCN =
                                _taskWork.ReceiverNameENUS = string.Empty;
                            }

                            try
                            {
                                var wfEntity = BaseWFEntity.GetWorkflowEntity(_taskWork.RefID, _taskWork.TypeCode);
                                if (wfEntity != null)
                                {
                                    wfEntity.PrepareTask(_taskWork);
                                }
                            }
                            catch (Exception ex)
                            {
                                Log4netHelper.WriteError(ex.Message);
                            }
                            //int _result = 1; // for test

                            int _result = TaskWork.Add(_taskWork);// 获取结果ID

                            Log4netHelper.WriteInfo(
                                string.Format("Result:{0}", JsonConvert.SerializeObject(new { _result, _taskWork }))
                            );

                            if (_result > 0)
                            {
                                //
                                // 在这里写其它逻辑  
                                //



                                // 返回 true
                                _returnAMObject.ReturnData = true.ToString();
                            }
                        }// end if if (_taskWork != null)
                    }
                    catch (System.Exception ex)
                    {
                        Log4netHelper.Log4netWriteErrorLog(@"k2回调 PrepareTask 接口出错!", ex);
                        _returnAMObject = PopulateAMApiException(ex);

                    }
                }

                tranScope.Complete();
            }

            //////
            return Ok(_returnAMObject);
        }


        /// <summary>
        /// Adds the project comment.添加项目Comment
        /// </summary>
        /// <returns>IHttpActionResult.</returns>
        [Route("api/k2callback/AddProjectComment")]
        [HttpPost]
        public IHttpActionResult AddProjectComment()
        {
            MCDAMReturnObject _returnAMObject = new MCDAMReturnObject(false.ToString());

            HttpRequest _request = HttpContext.Current.Request;

            string k2SN, operatorID, action, procInstID, activity, comment;

            k2SN = _request["k2SN"];
            operatorID = _request["operatorID"];
            action = _request["action"];
            procInstID = _request["procInstID"];
            comment = DecrypBase64(_request["comment"]);
            activity = string.Empty;

            Log4netHelper.WriteInfo(
                string.Format("InputVals:{0}",
                    JsonConvert.SerializeObject(new { k2SN, operatorID, action, procInstID, comment })
                    )
            );


            int? intProcInstID = ConvertHelper.ToNullableInt(procInstID);
            try { activity = K2FxContext.Current.GetCurrentActivityName(k2SN, operatorID); }
            catch { }

            using (TransactionScope tranScope = new TransactionScope())
            {
                if (!string.IsNullOrEmpty(k2SN) && !string.IsNullOrEmpty(operatorID))
                {
                    try
                    {
                        var __item = TaskWork.Search(c => c.K2SN == k2SN).FirstOrDefault();

                        // 添加 Comment
                        var _empInfo = Employee.Search(o => o.Code == operatorID || o.Account == operatorID).FirstOrDefault();
                        var _projUsers = ProjectUsers.Search(o => o.ProjectId == __item.RefID && o.UserAccount == operatorID).FirstOrDefault();

                        ProjectComment _projComment = ProjectComment.Search(
                                o => o.ProcInstID == intProcInstID && o.RefTableId != null && !string.IsNullOrEmpty(o.RefTableName)
                            ).AsNoTracking().FirstOrDefault();

                        if (_projComment == null)
                        {
                            _projComment = new ProjectComment();
                        }

                        _projComment.Id = Guid.NewGuid();
                        _projComment.UserAccount = _projComment.CreateUserAccount = operatorID;
                        if (_empInfo != null)
                        {
                            _projComment.CreateUserNameENUS = _projComment.CreateUserNameENUS = _empInfo.NameENUS;
                            _projComment.CreateUserNameZHCN = _projComment.CreateUserNameZHCN = _empInfo.NameZHCN;
                        }
                        _projComment.RefTableId = _projComment.RefTableId;
                        _projComment.RefTableName = _projComment.RefTableName;

                        if (_projUsers != null && !string.IsNullOrEmpty(_projUsers.RoleCode))
                        {
                            _projComment.TitleCode = _projUsers.RoleCode;
                            _projComment.TitleNameENUS = _projUsers.RoleNameENUS;
                            _projComment.TitleNameZHCN = _projUsers.RoleNameZHCN;
                        }
                        else
                        {
                            //var __userStorePos = bllVStorePosit.QueryStorePositionByProjectID(__item.RefID, operatorID).FirstOrDefault();
                            //if (__userStorePos != null && !string.IsNullOrEmpty(__userStorePos.PositionCode))
                            //{
                            //    _projComment.TitleCode = __userStorePos.PositionCode;
                            //    _projComment.TitleNameENUS = __userStorePos.PositionNameENUS;
                            //    _projComment.TitleNameZHCN = __userStorePos.PositionNameZHCN;
                            //}
                        }

                        if (__item != null)
                        {
                            _projComment.SourceCode = __item.SourceCode;
                            _projComment.SourceNameENUS = __item.SourceNameENUS;
                            _projComment.SourceNameZHCN = __item.SourceNameZHCN;
                        }

                        _projComment.Action = action;
                        _projComment.CreateTime = DateTime.Now;
                        _projComment.Content = comment;
                        _projComment.Status = ProjectCommentStatus.Submit;

                        _projComment.ProcInstID = intProcInstID;
                        _projComment.ActivityName = activity;
                        //int _resultAddComment = 1;  // for test

                        int _resultAddComment = ProjectComment.Add(_projComment); // Insert Comment

                        if (_resultAddComment > 0)
                        {
                            //
                            // 在这里写其它逻辑  
                            //


                            // 返回 true
                            _returnAMObject.ReturnData = true.ToString();
                        }

                        Log4netHelper.WriteInfo(
                        string.Format("Add Comment:{0}",
                            JsonConvert.SerializeObject(new { _resultAddComment, _projComment.Id, _projComment.Content })
                            )
                        );
                    }
                    catch (System.Exception ex)
                    {
                        _returnAMObject = PopulateAMApiException(ex);
                        Log4netHelper.Log4netWriteErrorLog(@"k2回调 TaskCompleted 接口出错!", ex);
                    }
                }

                tranScope.Complete();
            }

            //////
            return Ok(_returnAMObject);
        }


        /// <summary>
        /// Tasks the completed.
        /// </summary>
        /// <param name="k2SN">The k2 serial number.</param>
        /// <param name="viewURL">The view URL.</param>
        /// <param name="operatorID">The operator identifier.</param>
        /// <param name="action">The action.</param>
        /// <returns>IHttpActionResult.</returns>
        [Route("api/k2callback/taskcompleted")]
        [HttpPost]
        public IHttpActionResult TaskCompleted()
        {
            HttpRequest _request = HttpContext.Current.Request;

            string k2SN, viewURL, operatorID, action, procInstID, activity, comment;

            k2SN = _request["k2SN"];
            viewURL = DecrypBase64(_request["viewURL"]);
            operatorID = _request["operatorID"];
            action = _request["action"];
            procInstID = _request["procInstID"];
            activity = DecrypBase64(_request["activity"]);
            comment = DecrypBase64(_request["comment"]);

            int? intProcInstID = ConvertHelper.ToNullableInt(procInstID);

            Log4netHelper.WriteInfo(
                string.Format("InputVals:{0}",
                    JsonConvert.SerializeObject(new { k2SN, viewURL, operatorID, action, procInstID, activity, comment })
                    )
            );

            MCDAMReturnObject _returnAMObject = new MCDAMReturnObject(false.ToString());

            using (TransactionScope tranScope = new TransactionScope())
            {
                if (!string.IsNullOrEmpty(k2SN))
                {
                    try
                    {
                        var _taskWorkList = TaskWork.Search(c => c.K2SN == k2SN).ToList();

                        if (_taskWorkList == null || _taskWorkList.Count <= 0)
                        {
                            return NotFound();
                        }
                        else
                        {
                            int _updateCount = 0;
                            foreach (TaskWork __item in _taskWorkList)
                            {
                                // 更新流程任务数据
                                __item.Url = viewURL;
                                __item.Status = TaskWorkStatus.Finished; //0 - 收到任务的初始状态  2- 执行过的状态
                                __item.FinishTime = DateTime.Now;
                                __item.DoActionUser = operatorID;// 执行人员EID
                                __item.ActionName = action;

                                //int _result = 1; // for test
                                int _result = TaskWork.Update(__item); // 获取结果ID

                                Log4netHelper.WriteInfo(
                                    string.Format("Updates:{0}",
                                        JsonConvert.SerializeObject(new { _result, __item })
                                        )
                                );

                                if (_result > 0)
                                {/*
                                    _updateCount++;

                                    // 添加 Comment
                                    var _empInfo = bllEmp.Search(o => o.Code == operatorID || o.Account == operatorID).FirstOrDefault();
                                    var _projUsers = bllProjectUsers.Search(o => o.ProjectId == __item.RefID && o.UserAccount == operatorID).FirstOrDefault();

                                    ProjectComment _projComment = bllProjComment.Search(
                                            o => o.ProcInstID == intProcInstID && o.RefTableId != null && string.IsNullOrEmpty( o.RefTableName ) 
                                        ).AsNoTracking().FirstOrDefault();

                                    _projComment.Id = Guid.NewGuid();
                                    _projComment.UserAccount = _projComment.CreateUserAccount = operatorID;
                                    if (_empInfo != null)
                                    {
                                        _projComment.CreateUserNameENUS = _projComment.CreateUserNameENUS = _empInfo.NameENUS;
                                        _projComment.CreateUserNameZHCN = _projComment.CreateUserNameZHCN = _empInfo.NameZHCN;
                                    }
                                    _projComment.RefTableId = _projComment.RefTableId;
                                    _projComment.RefTableName = _projComment.RefTableName;

                                    if ( _projUsers != null && !string.IsNullOrEmpty(_projUsers.RoleCode) )
                                    {
                                        _projComment.TitleCode = _projUsers.RoleCode;
                                        _projComment.TitleNameENUS = _projUsers.RoleNameENUS;
                                        _projComment.TitleNameZHCN = _projUsers.RoleNameZHCN;
                                    }
                                    else
                                    {
                                        var __userStorePos = bllVStorePosit.QueryStorePositionByProjectID(__item.RefID, operatorID).FirstOrDefault();
                                        if (__userStorePos != null && !string.IsNullOrEmpty(__userStorePos.PositionCode))
                                        {
                                            _projComment.TitleCode = __userStorePos.PositionCode;
                                            _projComment.TitleNameENUS = __userStorePos.PositionNameENUS;
                                            _projComment.TitleNameZHCN = __userStorePos.PositionNameZHCN;
                                        }
                                    }

                                    _projComment.Action = action;
                                    _projComment.CreateTime = DateTime.Now;
                                    _projComment.Content = comment;
                                    _projComment.Status = ProjectCommentStatus.Submit;
                                    _projComment.SourceCode = __item.SourceCode;
                                    _projComment.SourceNameENUS = __item.SourceNameENUS;
                                    _projComment.SourceNameZHCN = __item.SourceNameZHCN;
                                    _projComment.ProcInstID = intProcInstID;
                                    _projComment.ActivityName = activity;
                                    //int _resultAddComment = 1;  // for test

                                    int _resultAddComment = bllProjComment.Add(_projComment); // Insert Comment

                                    Log4netHelper.WriteInfo(
                                    string.Format("Add Comment:{0}",
                                        JsonConvert.SerializeObject(new { _resultAddComment, _projComment.Id, _projComment.Content })
                                        )
                                    );
                                    */
                                }
                            }



                            if (_updateCount > 0)
                            {
                                //
                                // 在这里写其它逻辑  
                                //


                                // 返回 true
                                _returnAMObject.ReturnData = true.ToString();
                            }
                        }// end if if (_taskWork != null)
                    }
                    catch (System.Exception ex)
                    {
                        _returnAMObject = PopulateAMApiException(ex);
                        Log4netHelper.Log4netWriteErrorLog(@"k2回调 TaskCompleted 接口出错!", ex);
                    }
                }

                tranScope.Complete();
            }

            //////
            return Ok(_returnAMObject);
        }



        /// <summary>
        /// Synchronizes the expired task.同步过期任务
        /// </summary>
        /// <param name="procInstID">The proc inst identifier.</param>
        /// <returns>IHttpActionResult.</returns>
        [Route("api/k2callback/syncexpiredtask")]
        [HttpPost]
        public IHttpActionResult SyncExpiredTask()
        {
            HttpRequest _request = HttpContext.Current.Request;
            MCDAMReturnObject _returnAMObject = new MCDAMReturnObject(false.ToString());

            string procInstID = string.Empty;
            string originalTaskID = string.Empty;
            string viewURL = string.Empty;

            try
            {
                procInstID = _request["procInstID"];
                originalTaskID = _request["originalTaskID"];
                viewURL = DecrypBase64(_request["viewURL"]);
            }
            catch { }

            int intProcInstId = ConvertHelper.ToInt(procInstID);

            if (!string.IsNullOrEmpty(procInstID) && intProcInstId > 0)
            {
                try
                {
                    var _listK2Task = K2_WaitForApprovalBLL.GetWaitForApprovalTaskByProcID(intProcInstId);

                    using (var tranScope = new TransactionScope())
                    {
                        var taskWorkList = TaskWork.Search(c => c.ProcInstID == intProcInstId).ToList();

                        int updateCount = 0;

                        // Find AM task works
                        foreach (TaskWork item in taskWorkList)
                        {
                            if (item.Status.HasValue && item.Status == TaskWorkStatus.UnFinish && !string.IsNullOrEmpty(item.K2SN))
                            {
                                string[] dimK2SN = item.K2SN.Split('_');

                                int actDestId = ConvertHelper.ToInt(dimK2SN[dimK2SN.Length - 1]);

                                if (actDestId > 0 && !_listK2Task.Exists(o => o.ActInstDestID == actDestId))
                                {
                                    item.Status = TaskWorkStatus.K2TaskExpired; // 过期状态
                                    TaskWork.Update(item);
                                    updateCount++;
                                }
                            }
                        }

                        if (updateCount > 0)
                        {
                            //
                            // 在这里写其它逻辑  
                            //
                        }

                        // 2014.09.03 victor.huang:更新发起流程的项目任务信息
                        Log4netHelper.WriteInfo(string.Format(@"TaskID:{0}", originalTaskID));
                        if (!string.IsNullOrEmpty(originalTaskID))
                        {

                            var _taskGUID = ConvertHelper.ToGuid(originalTaskID);
                            var _taskWorkOld = TaskWork.Search(c => c.Id == _taskGUID && (c.ProcInstID == null || c.ProcInstID < 1)).FirstOrDefault();
                            Log4netHelper.WriteInfo(string.Format(@"Task work 更新前数据:{0}", JsonConvert.SerializeObject(_taskWorkOld)));

                            if (_taskWorkOld != null && _taskWorkOld.Id == _taskGUID)
                            {
                                if (!string.IsNullOrEmpty(viewURL))
                                {
                                    _taskWorkOld.Url = viewURL;
                                }
                                _taskWorkOld.Status = TaskWorkStatus.Finished;
                                _taskWorkOld.ProcInstID = intProcInstId;
                                _taskWorkOld.FinishTime = DateTime.Now;

                                int _intUpdateOldTaskResult = TaskWork.Update(_taskWorkOld);
                                Log4netHelper.WriteInfo(string.Format(@"Update Result: {0}, Task更新后数据: {1}", _intUpdateOldTaskResult, JsonConvert.SerializeObject(_taskWorkOld)));
                            }
                        }

                        tranScope.Complete();
                    }
                    // 返回 true
                    _returnAMObject.ReturnData = true.ToString();
                }
                catch (System.Exception ex)
                {
                    _returnAMObject = PopulateAMApiException(ex);
                    Log4netHelper.Log4netWriteErrorLog(@"k2回调 SyncExpiredTask 接口出错! Proc Inst Id:" + intProcInstId, ex);
                }
            }

            //////
            return Ok(_returnAMObject);
        }

        #region Email Sending
        /// <summary>
        /// 在流程任务执行完毕之后，发送邮件给下一步的执行人，在K2里面进行调用。
        /// </summary>
        /// <returns></returns>
        [Route("api/k2callback/SendEmail")]
        [HttpPost]
        public IHttpActionResult SendEmail(EmailMessage emailMessage)
        {
            Log4netHelper.WriteInfoLog("api/k2callback/SendEmail开始回调邮件接口:" + DateTime.Now.ToString());
            MCDAMReturnObject returnAMObject = new MCDAMReturnObject(false.ToString());
            if (emailMessage == null)
            {
                returnAMObject.ErrorMessage = "EmailMessage对象传入为空!";
                return Ok(returnAMObject);
            }

            try
            {
                //1,获取传入参数
                string sn = string.Empty;
                var _dicEmailValues = emailMessage.EmailBodyValues;
                if (_dicEmailValues == null)
                {
                    returnAMObject.ErrorMessage = "EmailMessage中的EmailBodyValues对象传入为空!";
                    return Ok();
                }
                if (_dicEmailValues.Keys.Contains("SN"))
                {
                    sn = emailMessage.EmailBodyValues["SN"];
                }
                if (string.IsNullOrEmpty(sn) && _dicEmailValues.Keys.Contains("SerialNumber"))
                {
                    sn = emailMessage.EmailBodyValues["SerialNumber"];
                }

                if (string.IsNullOrEmpty(sn))
                {
                    returnAMObject.ErrorMessage = "SN不能为空!";
                    return Ok(returnAMObject);
                }

                //获取操作人
                //string operatorID = string.Empty;
                //if (_dicEmailValues.Keys.Contains("OperatorID"))
                //{
                //    operatorID = emailMessage.EmailBodyValues["OperatorID"];
                //}
                //if (string.IsNullOrEmpty(operatorID))
                //{
                //    returnAMObject.ErrorMessage = "operatorID不能为空!";
                //    return Ok(returnAMObject);
                //}
                //var operatorEmp = Employee.GetSimpleEmployeeByCode(operatorID);

                //2, 获取任务对象
                var task = TaskWork.GetTaskBySN(sn);
                if (task == null)
                {
                    returnAMObject.ErrorMessage = "根据SN不能获取到任务对象!";
                    return Ok(returnAMObject);
                }

                if (task.Status.HasValue && ((int)task.Status.Value) > 0 && ((int)task.Status.Value) < 100)
                {
                    returnAMObject.ReturnData = true.ToString();
                    return Ok(returnAMObject);
                }

                //2，准备Email里面的基本数据信息
                var action = Mcdonalds.AM.Services.Common.ActionLogType.None;
                var notificationModule = new Mcdonalds.AM.Services.Common.MailHelper.NotificationModule();
                emailMessage = MailHelper.BuildEmailMessage(task, ref action, ref notificationModule);
                if (emailMessage == null)
                {
                    //returnAMObject.ErrorMessage = string.Format("Can't build mail message. SN:{0}", sn);
                    //Log4netHelper.WriteErrorLog(returnAMObject.ErrorMessage);
                    return Ok(returnAMObject);
                }
                //if (emailMessage.EmailBodyValues.ContainsKey("Operator"))
                //{
                //    emailMessage.EmailBodyValues["Operator"] = operatorEmp.NameENUS;
                //}
                EmailSendingResultType result = null;
                if (action == Mcdonalds.AM.Services.Common.ActionLogType.Approve)
                    result = MailHelper.SendApprovalEmail(emailMessage);
                else if (action == Mcdonalds.AM.Services.Common.ActionLogType.Recall)
                    result = MailHelper.SendRecallEmail(emailMessage);
                else
                    result = MailHelper.SendCommentsEmail(emailMessage);
                if (result != null)
                {
                    returnAMObject.ReturnData = result.Successful.ToString();
                    returnAMObject.ErrorMessage = result.ErrorMessage;
                    returnAMObject.InnerErrorMessage = result.InnerErrorMessage;
                    if (result.Successful)
                    {
                        Log4netHelper.WriteInfoLog("api/k2callback/SendEmail发送邮件成功1; " + "邮件接收人:" + emailMessage.To);
                    }
                    else
                    {
                        Log4netHelper.WriteInfoLog("api/k2callback/SendEmail发送邮件失败1; " + "邮件接收人:" + emailMessage.To + "; 错误信息:/r/n" + (string.IsNullOrEmpty(result.ErrorMessage) ? string.Empty : result.ErrorMessage) + "/r/n" + (string.IsNullOrEmpty(result.InnerErrorMessage) ? string.Empty : result.InnerErrorMessage) + (string.IsNullOrEmpty(result.StackTrace) ? string.Empty : result.StackTrace));
                    }
                }
                if (task.ProcInstID != null && action != Mcdonalds.AM.Services.Common.ActionLogType.Recall)
                {
                    //通过K2每添加一条任务，都会回调一次，但是提醒邮件只在添加第一条任务时发送
                    if (TaskWork.Count(t => t.ProcInstID == task.ProcInstID && t.RefID == task.RefID && t.Status == TaskWorkStatus.UnFinish) == 1)
                    {
                        string emails = string.Empty;
                        //通知流程相关人
                        emails = MailHelper.GetWorkflowRelationUserEmails(task.RefID, task.TypeCode, notificationModule);
                        //发送备注提醒邮件
                        //var emailTo = MailHelper.GetNotifyUserEmails(sn);
                        //if (!string.IsNullOrEmpty(emailTo))
                        //    emails += emailTo;
                        if (!string.IsNullOrEmpty(emails))
                        {
                            emailMessage.To = emails;
                            var result2 = MailHelper.SendCommentsEmail(emailMessage);
                            if (result2 != null)
                            {
                                returnAMObject.ReturnData = result.Successful.ToString();
                                returnAMObject.ErrorMessage = result.ErrorMessage;
                                returnAMObject.InnerErrorMessage = result.InnerErrorMessage;
                                if (result.Successful)
                                {
                                    Log4netHelper.WriteInfoLog("api/k2callback/SendEmail发送提醒邮件成功2; " + "邮件接收人:" + emailMessage.To);
                                }
                                else
                                {
                                    Log4netHelper.WriteInfoLog("api/k2callback/SendEmail发送提醒邮件失败2; " + "邮件接收人:" + emailMessage.To + "; 错误信息:/r/n" + (string.IsNullOrEmpty(result.ErrorMessage) ? string.Empty : result.ErrorMessage) + "/r/n" + (string.IsNullOrEmpty(result.InnerErrorMessage) ? string.Empty : result.InnerErrorMessage) + (string.IsNullOrEmpty(result.StackTrace) ? string.Empty : result.StackTrace));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                returnAMObject.ErrorMessage = ex.Message;
                returnAMObject.InnerErrorMessage = ex.InnerException == null ? string.Empty : ex.InnerException.Message;
                returnAMObject.StackTrace = ex.StackTrace;
                returnAMObject.ReturnData = "发送邮件失败!";
                Log4netHelper.Log4netWriteErrorLog("发送邮件失败!", ex);
            }

            return Ok(returnAMObject);
        }
        #endregion
    }//end class K2CallbackController
}