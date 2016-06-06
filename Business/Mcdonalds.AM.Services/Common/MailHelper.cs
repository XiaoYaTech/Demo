using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.Services.Controllers.Closure;
using Mcdonalds.AM.Services.EmailServiceReference;
using NTTMNC.BPM.Fx.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.Services.Common
{
    public static class MailHelper
    {
        public static EmailMessage BuildEmailMessage(string sn, ActionLogType action, ref NotificationModule notificationModule)
        {
            EmailMessage emailMessage = null;
            if (string.IsNullOrEmpty(sn))
            {
                Log4netHelper.WriteErrorLog("SN can't be null or empty.");
                return emailMessage;
            }
            var task = TaskWork.GetTaskBySN(sn);
            if (task == null)
            {
                Log4netHelper.WriteErrorLog(string.Format("Can't get task. SN: {0}", sn));
                return emailMessage;
            }
            emailMessage = BuildEmailMessage(task, ref action, ref notificationModule);
            return emailMessage;
        }

        public static EmailMessage BuildEmailMessage(TaskWork task, ref ActionLogType action, ref NotificationModule notificationModule)
        {
            EmailMessage emailMessage = null;
            var firstApproveStepName = string.Empty;
            var rejectStepName = string.Empty;

            switch (task.TypeCode)
            {
                case FlowCode.Closure_ClosurePackage:
                    emailMessage = BuildClosurePackageEmailMessage(task, notificationModule, ref action);
                    emailMessage.IsFirstApproveStep = false;
                    emailMessage.NeedRejectStep = false;
                    firstApproveStepName = Constants.Closure_Package_FirstApproveStepName;
                    rejectStepName = Constants.Closure_Package_RejectStepName;
                    break;
                case FlowCode.TempClosure_ClosurePackage:
                    emailMessage = BuildTempClosurePackageEmailMessage(task, notificationModule, ref action);
                    emailMessage.IsFirstApproveStep = false;
                    emailMessage.NeedRejectStep = false;
                    firstApproveStepName = Constants.TempClosure_Package_FirstApproveStepName;
                    rejectStepName = Constants.TempClosure_Package_RejectStepName;
                    break;
                case FlowCode.MajorLease_Package:
                    emailMessage = BuildMajorLeasePackageEmailMessage(task, notificationModule, ref action);
                    emailMessage.IsFirstApproveStep = false;
                    emailMessage.NeedRejectStep = false;
                    firstApproveStepName = Constants.MajorLease_Package_FirstApproveStepName;
                    rejectStepName = Constants.MajorLease_Package_RejectStepName;
                    break;
                case FlowCode.Reimage_Package:
                    emailMessage = BuildReimagePackageEmailMessage(task, notificationModule, ref action);
                    emailMessage.IsFirstApproveStep = false;
                    emailMessage.NeedRejectStep = false;
                    firstApproveStepName = Constants.Reimage_Package_FirstApproveStepName;
                    rejectStepName = Constants.Reimage_Package_RejectStepName;
                    break;
                case FlowCode.Rebuild_Package:
                    emailMessage = BuildRebuildPackageEmailMessage(task, notificationModule, ref action);
                    emailMessage.IsFirstApproveStep = false;
                    emailMessage.NeedRejectStep = false;
                    firstApproveStepName = Constants.Rebuild_Package_FirstApproveStepName;
                    rejectStepName = Constants.Rebuild_Package_RejectStepName;
                    break;
                case FlowCode.Renewal_Package:
                    emailMessage = BuildRenewalPackageEmailMessage(task, notificationModule, ref action);
                    emailMessage.IsFirstApproveStep = false;
                    emailMessage.NeedRejectStep = false;
                    firstApproveStepName = Constants.Renewal_Package_FirstApproveStepName;
                    rejectStepName = Constants.Renewal_Package_RejectStepName;
                    break;
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(task.ActivityName) && emailMessage != null)
            {
                var firstStep = getActivityNameList(firstApproveStepName);
                var rejectStep = getActivityNameList(rejectStepName);
                if (firstStep.Contains(task.ActivityName))
                    emailMessage.IsFirstApproveStep = true;
                if (rejectStep.Contains(task.ActivityName))
                    emailMessage.NeedRejectStep = true;
            }
            if (emailMessage != null && !string.IsNullOrEmpty(notificationModule.FlowCode))
            {
                notificationModule.RemindInfo = new Remind()
                {
                    Title = emailMessage.IsFirstApproveStep ? string.Format("【{4}】{0}_{1}_{2}_Submitted by {3}", notificationModule.FlowCode, notificationModule.StoreCode, notificationModule.StoreNameENUS, notificationModule.OriginatorNameENUS, notificationModule.ProjectId) : string.Format("【{6}】{0}__{1}_{2}_{3} by {4}_Submitted by {5}", notificationModule.FlowCode, notificationModule.StoreCode, notificationModule.StoreNameENUS, notificationModule.Operation, notificationModule.OperatorNameENUS, notificationModule.OriginatorNameENUS, notificationModule.ProjectId),
                    RegisterCode = task.SourceCode,
                    SenderAccount = notificationModule.OperatorCode,
                    SenderNameENUS = notificationModule.OperatorNameENUS,
                    SenderNameZHCN = notificationModule.OperatorNameZHCN
                };
                Remind.Send(notificationModule.RemindInfo, notificationModule.ReceiverList, notificationModule.ProjectId, notificationModule.FlowCode);
            }
            return emailMessage;
        }

        /// <summary>
        /// 生成activityNameList
        /// </summary>
        /// <param name="rejectStepStr"></param>
        /// <returns></returns>
        private static List<string> getActivityNameList(string str)
        {
            var result = new List<string>();
            foreach (var item in str.Split(';'))
            {
                if (!string.IsNullOrEmpty(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        /// <summary>
        /// 生成Email Message信息
        /// </summary>
        /// <param name="actionLogs"></param>
        /// <returns></returns>
        public static EmailMessage BuildEmailMessage(string refTableName, Guid? refTableId, string sourceCode)
        {
            EmailMessage emailMessage = new EmailMessage();

            ProjectCommentCondition condition = new ProjectCommentCondition();
            //只显示通过意见
            List<VProjectComment> commentList = new List<VProjectComment>();
            //全部意见
            List<VProjectComment> commentDetailList = new List<VProjectComment>();
            if (!string.IsNullOrEmpty(refTableName) && refTableId.HasValue)
            {
                condition.RefTableName = refTableName;
                condition.RefTableId = refTableId.Value;
                condition.SourceCode = sourceCode;
                commentList = VProjectComment.SearchVListForPDF(condition);
                commentDetailList = VProjectComment.SearchVList(condition);
            }

            //邮件日志信息
            if (commentDetailList == null || commentDetailList.Count < 1) return emailMessage;
            List<WorkflowActionLogForEmail> emailLogs = new List<WorkflowActionLogForEmail>();
            foreach (var log in commentDetailList)
            {
                var workflowActionLogForEmail = new WorkflowActionLogForEmail();
                workflowActionLogForEmail.ActionName = log.ActionDesc;
                workflowActionLogForEmail.EID = log.CreateUserAccount;
                workflowActionLogForEmail.OperatorName = log.UserNameENUS;
                workflowActionLogForEmail.PositionNameENUS = log.PositionENUS;
                workflowActionLogForEmail.PositionNameZHCN = log.PositionZHCN;
                workflowActionLogForEmail.OperationDateTime = log.CreateTime.HasValue ? log.CreateTime.Value : DateTime.MinValue;
                workflowActionLogForEmail.Comments = log.Content;
                workflowActionLogForEmail.LogType = EmailServiceReference.ActionLogType.Comments;
                emailLogs.Add(workflowActionLogForEmail);
            }
            foreach (var log in commentList)
            {
                var workflowActionLogForEmail = new WorkflowActionLogForEmail();
                workflowActionLogForEmail.ActionName = log.ActionDesc;
                workflowActionLogForEmail.EID = log.CreateUserAccount;
                workflowActionLogForEmail.OperatorName = log.UserNameENUS;
                workflowActionLogForEmail.PositionNameENUS = log.PositionENUS;
                workflowActionLogForEmail.PositionNameZHCN = log.PositionZHCN;
                workflowActionLogForEmail.OperationDateTime = log.CreateTime.HasValue ? log.CreateTime.Value : DateTime.MinValue;
                workflowActionLogForEmail.Comments = log.Content;
                switch (log.Action)
                {
                    case "Decline":
                    case "Approve":
                    case "Return":
                    case "Recall":
                        workflowActionLogForEmail.LogType = EmailServiceReference.ActionLogType.Approval;
                        break;
                    case "Submit":
                    case "ReSubmit":
                        workflowActionLogForEmail.LogType = EmailServiceReference.ActionLogType.Submit;
                        break;
                    default:
                        break;
                }
                emailLogs.Add(workflowActionLogForEmail);
            }
            emailMessage.ActionLogs = emailLogs.OrderBy(e => e.OperationDateTime).ToArray();
            return emailMessage;
        }

        /// <summary>
        /// 发送审批邮件
        /// </summary>
        /// <param name="emailMessage"></param>
        /// <returns></returns>
        public static EmailSendingResultType SendApprovalEmail(EmailMessage emailMessage)
        {
            EmailSendingResultType result = new EmailSendingResultType();
            try
            {
                using (EmailServiceClient client = new EmailServiceClient())
                {
                    result = client.SendApprovalEmail(emailMessage);
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                result.InnerErrorMessage = ex.InnerException == null ? string.Empty : ex.InnerException.Message;
                result.StackTrace = ex.StackTrace;
                result.Successful = false;
            }
            return result;
        }

        /// <summary>
        /// 发送备注邮件
        /// </summary>
        /// <param name="emailMessage"></param>
        /// <returns></returns>
        public static EmailSendingResultType SendCommentsEmail(EmailMessage emailMessage)
        {
            EmailSendingResultType result = new EmailSendingResultType();
            try
            {
                using (EmailServiceClient client = new EmailServiceClient())
                {
                    result = client.SendCommentsEmail(emailMessage);
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                result.InnerErrorMessage = ex.InnerException == null ? string.Empty : ex.InnerException.Message;
                result.StackTrace = ex.StackTrace;
                result.Successful = false;
            }
            return result;
        }

        /// <summary>
        /// 发送提醒邮件
        /// </summary>
        /// <param name="emailMessage"></param>
        /// <returns></returns>
        public static EmailSendingResultType SendNotifyEmail(EmailMessage emailMessage)
        {
            EmailSendingResultType result = new EmailSendingResultType();
            try
            {
                using (EmailServiceClient client = new EmailServiceClient())
                {
                    result = client.SendNotificationEmail(emailMessage);
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                result.InnerErrorMessage = ex.InnerException == null ? string.Empty : ex.InnerException.Message;
                result.StackTrace = ex.StackTrace;
                result.Successful = false;
            }
            return result;
        }

        /// <summary>
        /// 发送Recall提醒邮件
        /// </summary>
        /// <param name="emailMessage"></param>
        /// <returns></returns>
        public static EmailSendingResultType SendRecallEmail(EmailMessage emailMessage)
        {
            EmailSendingResultType result = new EmailSendingResultType();
            try
            {
                using (EmailServiceClient client = new EmailServiceClient())
                {
                    result = client.SendRecallEmail(emailMessage);
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                result.InnerErrorMessage = ex.InnerException == null ? string.Empty : ex.InnerException.Message;
                result.StackTrace = ex.StackTrace;
                result.Successful = false;
            }
            return result;
        }

        /// <summary>
        /// 发送普通邮件（无邮件模版），发送的时候在调用的时候自行组织邮件的标题和内容。
        /// </summary>
        /// <param name="to">需要发送的电子邮件地址，多个地址用“;”分开</param>
        /// <param name="subject">邮件标题</param>
        /// <param name="content">邮件内容，可以是HTML格式的内容</param>
        /// <returns>返回结果，可以判断邮件是否发送成功以及发送出错的原因</returns>
        public static EmailSendingResultType SendEmail(string to, string subject, string content)
        {
            EmailSendingResultType result = new EmailSendingResultType();
            try
            {
                using (EmailServiceClient client = new EmailServiceClient())
                {
                    result = client.SendEmail(to, subject, content);
                }
            }
            catch (Exception ex)
            {
                result.ErrorMessage = ex.Message;
                result.InnerErrorMessage = ex.InnerException == null ? string.Empty : ex.InnerException.Message;
                result.StackTrace = ex.StackTrace;
                result.Successful = false;
            }
            return result;
        }

        static EmailMessage BuildClosurePackageEmailMessage(TaskWork task, NotificationModule notification, ref ActionLogType action)
        {
            EmailMessage emailMessage = null;
            Dictionary<string, string> bodyValues = new Dictionary<string, string>();
            var closureInfoEntity = ClosureInfo.GetByProjectId(task.RefID);
            if (action == ActionLogType.None)
            {
                if (task.ActivityName != "Start" && task.ActivityName != "Originator")
                    action = ActionLogType.Approve;
                else
                {
                    var projectInfo = ProjectInfo.Search(e => e.ProjectId == task.RefID && e.FlowCode == task.TypeCode).FirstOrDefault();
                    if (projectInfo != null && projectInfo.Status == ProjectStatus.Recalled)
                        action = ActionLogType.Recall;
                    else
                        action = ActionLogType.Return;
                }
            }
            //bodyValues.Add("ApplicantName", currentUser.NameENUS);//--提交人
            bodyValues.Add("StoreCode", closureInfoEntity.USCode);
            bodyValues.Add("StoreName", closureInfoEntity.StoreNameENUS);
            bodyValues.Add("Actor", closureInfoEntity.AssetActorNameENUS);////--呈递人
            bodyValues.Add("ProjectName", FlowCode.Closure);//项目名称
            bodyValues.Add("WorkflowName", FlowCode.Closure_ClosurePackage);////--流程名称
            //邮件内容里面的审批信息
            var actionDesc = string.Empty;
            switch (action)
            {
                case ActionLogType.Approve:
                    actionDesc = "Approved";
                    break;
                case ActionLogType.Decline:
                    actionDesc = "Rejected";
                    break;
                case ActionLogType.Return:
                    actionDesc = "Returned";
                    break;
                case ActionLogType.Recall:
                    actionDesc = "Recalled";
                    break;
                case ActionLogType.Comments:
                    actionDesc = "Commented";
                    break;
            }
            if (!string.IsNullOrEmpty(actionDesc))
                bodyValues.Add("Operation", actionDesc);
            //流程发起人
            bodyValues.Add("Originator", closureInfoEntity.AssetActorNameENUS);
            bodyValues.Add("SN", task.K2SN);//SN
            bodyValues.Add("StepName", string.Empty);//
            var webRootUrl = ConfigurationManager.AppSettings["webHost"];
            var rootUrl = webRootUrl.EndsWith("/") ? webRootUrl.Substring(0, webRootUrl.LastIndexOf("/")) : webRootUrl;
            var formUrl = rootUrl + task.Url;
            if (task.ProcInstID.HasValue && !string.IsNullOrEmpty(task.RefID) && task.Url.ToLower().IndexOf(task.RefID.ToLower()) < 0)
            {
                formUrl = string.Format("{0}&projectId={1}", formUrl, task.RefID);
            }
            bodyValues.Add("FormUrl", HtmlConversionUtility.FormatPortalUrl(task.ReceiverAccount, formUrl, "zh-cn"));
            //邮件的Comments信息
            //获取ClosurePackage的对象
            var closurePackage = ClosurePackage.Get(closureInfoEntity.ProjectId);
            var projectComment = GetLatestOperator(ClosurePackage.TableName, closurePackage.Id);
            bodyValues.Add("OperatorID", task.ReceiverAccount);//--任务接收人
            bodyValues.Add("Operator", projectComment.UserNameENUS);//--提交人
            bodyValues.Add("EntityID", closurePackage.Id.ToString());//

            emailMessage = MailHelper.BuildEmailMessage(task.RefTableName, task.RefTableId, task.SourceCode);
            //Store Basic Info
            emailMessage.StoreBasicInfoDict = closurePackage.GetPrintTemplateFields();
            emailMessage.EmailBodyValues = bodyValues;

            var receiverList = new List<SimpleEmployee>();
            if (action == ActionLogType.Approve)
            {
                //获取邮件接收人
                var receiver = Employee.GetSimpleEmployeeByCode(task.ReceiverAccount);
                if (receiver.Code != projectComment.UserAccount)
                {
                    emailMessage.To = receiver.Mail;
                    receiverList.Add(receiver);
                }
            }
            else
            {
                receiverList = GetReturnANDRejectNotifyUserEmails(task, closureInfoEntity.AssetActorAccount, projectComment.UserAccount);
                emailMessage.To = String.Join(";", receiverList.Select(i => i.Mail).ToArray());
            }
            //获取邮件附件
            if (action != ActionLogType.Recall)
            {
                var attList = Attachment.GetAllAttachmentsIncludeRequire(ClosurePackage.TableName, task.RefID, FlowCode.Closure_ClosurePackage);
                Dictionary<string, string> attachments = new Dictionary<string, string>();
                foreach (var att in attList)
                {
                    if (!string.IsNullOrEmpty(att.InternalName))
                    {
                        if (att.TypeCode == "Cover")
                            continue;
                        var fileExt = att.InternalName.Substring(att.InternalName.LastIndexOf("."));
                        var fileName = string.Empty;
                        if (!string.IsNullOrEmpty(att.RequireName))
                        {
                            fileName = att.RequireName + fileExt;
                        }
                        else
                        {
                            if (att.Name.IndexOf(".") < 0)
                            {
                                fileName = att.Name + fileExt;
                            }
                            else
                            {
                                fileName = att.Name;
                            }
                        }
                        string key = SiteFilePath.UploadFiles_DIRECTORY + "\\" + att.InternalName;
                        if (attachments.Keys.Contains(key)) continue;
                        attachments.Add(key, fileName);
                    }
                }
                emailMessage.AttachmentsDict = attachments;
            }
            //邮件模板Code,如果不指定，则会用默认的邮件模板发送邮件.
            emailMessage.TemplateCode = EmailTemplateCode.Closure_Package;

            #region Build NotifycationModule
            notification.EntityId = closurePackage.Id;
            notification.FlowCode = task.TypeCode;
            notification.Operation = actionDesc;
            notification.OperatorCode = projectComment.UserAccount;
            notification.OperatorNameENUS = projectComment.UserNameENUS;
            notification.OperatorNameZHCN = projectComment.UserNameZHCN;
            notification.OriginatorNameENUS = closureInfoEntity.AssetActorNameENUS;
            notification.OriginatorNameZHCN = closureInfoEntity.AssetActorNameZHCN;
            notification.ProjectId = task.RefID;
            notification.ReceiverList = receiverList;
            notification.StoreCode = closureInfoEntity.USCode;
            notification.StoreNameENUS = closureInfoEntity.StoreNameENUS;
            notification.StoreNameZHCN = closureInfoEntity.StoreNameZHCN;
            #endregion

            return emailMessage;
        }

        static EmailMessage BuildTempClosurePackageEmailMessage(TaskWork task, NotificationModule notification, ref ActionLogType action)
        {
            EmailMessage emailMessage = null;
            Dictionary<string, string> bodyValues = new Dictionary<string, string>();
            var tempClosureInfoEntity = TempClosureInfo.FirstOrDefault(i => i.ProjectId == task.RefID);
            if (action == ActionLogType.None)
            {
                if (task.ActivityName != "Start" && task.ActivityName != "Originator")
                    action = ActionLogType.Approve;
                else
                {
                    var projectInfo = ProjectInfo.Search(e => e.ProjectId == task.RefID && e.FlowCode == task.TypeCode).FirstOrDefault();
                    if (projectInfo != null && projectInfo.Status == ProjectStatus.Recalled)
                        action = ActionLogType.Recall;
                    else
                        action = ActionLogType.Return;
                }
            }
            //bodyValues.Add("ApplicantName", currentUser.NameENUS);//--提交人
            bodyValues.Add("StoreCode", tempClosureInfoEntity.USCode);
            bodyValues.Add("StoreName", tempClosureInfoEntity.StoreNameENUS);
            bodyValues.Add("Actor", tempClosureInfoEntity.AssetActorNameENUS);////--呈递人
            bodyValues.Add("ProjectName", FlowCode.TempClosure);//项目名称
            bodyValues.Add("WorkflowName", FlowCode.TempClosure_ClosurePackage);////--流程名称
            var actionDesc = string.Empty;
            switch (action)
            {
                case ActionLogType.Approve:
                    actionDesc = "Approved";
                    break;
                case ActionLogType.Decline:
                    actionDesc = "Rejected";
                    break;
                case ActionLogType.Return:
                    actionDesc = "Returned";
                    break;
                case ActionLogType.Recall:
                    actionDesc = "Recalled";
                    break;
                case ActionLogType.Comments:
                    actionDesc = "Commented";
                    break;
            }
            if (!string.IsNullOrEmpty(actionDesc))
                bodyValues.Add("Operation", actionDesc);
            //邮件内容里面的审批信息
            //流程发起人
            bodyValues.Add("Originator", tempClosureInfoEntity.AssetActorNameENUS);
            bodyValues.Add("SN", task.K2SN);//SN
            bodyValues.Add("StepName", string.Empty);//
            var webRootUrl = ConfigurationManager.AppSettings["webHost"];
            var rootUrl = webRootUrl.EndsWith("/") ? webRootUrl.Substring(0, webRootUrl.LastIndexOf("/")) : webRootUrl;
            var formUrl = rootUrl + task.Url;
            if (task.ProcInstID.HasValue && !string.IsNullOrEmpty(task.RefID) && task.Url.ToLower().IndexOf(task.RefID.ToLower()) < 0)
            {
                formUrl = string.Format("{0}&projectId={1}", formUrl, task.RefID);
            }
            bodyValues.Add("FormUrl", HtmlConversionUtility.FormatPortalUrl(task.ReceiverAccount, formUrl, "zh-cn"));
            //邮件的Comments信息
            //获取TempClosurePackage的对象
            var tempClosurePackage = TempClosurePackage.Get(tempClosureInfoEntity.ProjectId);
            var projectComment = GetLatestOperator(tempClosurePackage.TableName, tempClosurePackage.Id);
            bodyValues.Add("OperatorID", task.ReceiverAccount);//--任务接收人
            bodyValues.Add("Operator", projectComment.UserNameENUS);//--提交人
            bodyValues.Add("EntityID", tempClosurePackage.Id.ToString());

            //获取邮件正文的基本信息（Store Basic Info等）
            var storeBasicInfoDict = tempClosurePackage.GetPrintTemplateFields();

            //流程日志
            emailMessage = MailHelper.BuildEmailMessage(tempClosurePackage.TableName, tempClosurePackage.Id, FlowCode.TempClosure);
            //Store Basic Info
            emailMessage.StoreBasicInfoDict = storeBasicInfoDict;
            emailMessage.EmailBodyValues = bodyValues;
            var receiverList = new List<SimpleEmployee>();
            if (action == ActionLogType.Approve)
            {
                //获取邮件接收人
                var receiver = Employee.GetSimpleEmployeeByCode(task.ReceiverAccount);
                if (receiver.Code != projectComment.UserAccount)
                {
                    emailMessage.To = receiver.Mail;
                    receiverList.Add(receiver);
                }
            }
            else
            {
                receiverList = GetReturnANDRejectNotifyUserEmails(task, tempClosureInfoEntity.AssetActorAccount, projectComment.UserAccount);
                emailMessage.To = String.Join(";", receiverList.Select(i => i.Mail).ToArray());
            }
            //获取邮件附件
            if (action != ActionLogType.Recall)
            {
                var attList = Attachment.GetAllAttachmentsIncludeRequire(tempClosurePackage.TableName, task.RefID, FlowCode.TempClosure_ClosurePackage);
                Dictionary<string, string> attachments = new Dictionary<string, string>();
                foreach (var att in attList)
                {
                    if (!string.IsNullOrEmpty(att.InternalName))
                    {
                        if (att.TypeCode == "Cover")
                            continue;
                        var fileExt = att.InternalName.Substring(att.InternalName.LastIndexOf("."));
                        var fileName = string.Empty;
                        if (!string.IsNullOrEmpty(att.RequireName))
                        {
                            fileName = att.RequireName + fileExt;
                        }
                        else
                        {
                            if (att.Name.IndexOf(".") < 0)
                            {
                                fileName = att.Name + fileExt;
                            }
                            else
                            {
                                fileName = att.Name;
                            }
                        }
                        string key = SiteFilePath.UploadFiles_DIRECTORY + "\\" + att.InternalName;
                        if (attachments.Keys.Contains(key)) continue;
                        attachments.Add(key, fileName);
                    }
                }
                emailMessage.AttachmentsDict = attachments;
            }
            //邮件模板Code,如果不指定，则会用默认的邮件模板发送邮件.
            emailMessage.TemplateCode = EmailTemplateCode.TempClosure_Package;

            #region Build NotifycationModule
            notification.EntityId = tempClosurePackage.Id;
            notification.FlowCode = task.TypeCode;
            notification.Operation = actionDesc;
            notification.OperatorCode = projectComment.UserAccount;
            notification.OperatorNameENUS = projectComment.UserNameENUS;
            notification.OperatorNameZHCN = projectComment.UserNameZHCN;
            notification.OriginatorNameENUS = tempClosureInfoEntity.AssetActorNameENUS;
            notification.OriginatorNameZHCN = tempClosureInfoEntity.AssetActorNameZHCN;
            notification.ProjectId = task.RefID;
            notification.ReceiverList = receiverList;
            notification.StoreCode = tempClosureInfoEntity.USCode;
            notification.StoreNameENUS = tempClosureInfoEntity.StoreNameENUS;
            notification.StoreNameZHCN = tempClosureInfoEntity.StoreNameZHCN;
            #endregion

            return emailMessage;
        }

        static EmailMessage BuildMajorLeasePackageEmailMessage(TaskWork task, NotificationModule notification, ref ActionLogType action)
        {
            EmailMessage emailMessage = null;
            Dictionary<string, string> bodyValues = new Dictionary<string, string>();
            var majorLeaseInfoEntity = MajorLeaseInfo.FirstOrDefault(i => i.ProjectId == task.RefID);
            if (action == ActionLogType.None)
            {
                if (task.ActivityName != "Start" && task.ActivityName != "Originator")
                    action = ActionLogType.Approve;
                else
                {
                    var projectInfo = ProjectInfo.Search(e => e.ProjectId == task.RefID && e.FlowCode == task.TypeCode).FirstOrDefault();
                    if (projectInfo != null && projectInfo.Status == ProjectStatus.Recalled)
                        action = ActionLogType.Recall;
                    else
                        action = ActionLogType.Return;
                }
            }
            //bodyValues.Add("ApplicantName", currentUser.NameENUS);//--提交人
            bodyValues.Add("StoreCode", majorLeaseInfoEntity.USCode);
            bodyValues.Add("StoreName", majorLeaseInfoEntity.StoreNameENUS);
            bodyValues.Add("Actor", majorLeaseInfoEntity.AssetActorNameENUS);////--呈递人
            bodyValues.Add("ProjectName", FlowCode.MajorLease);//项目名称
            bodyValues.Add("WorkflowName", FlowCode.MajorLease_Package);////--流程名称
            var actionDesc = string.Empty;
            switch (action)
            {
                case ActionLogType.Approve:
                    actionDesc = "Approved";
                    break;
                case ActionLogType.Decline:
                    actionDesc = "Rejected";
                    break;
                case ActionLogType.Return:
                    actionDesc = "Returned";
                    break;
                case ActionLogType.Recall:
                    actionDesc = "Recalled";
                    break;
                case ActionLogType.Comments:
                    actionDesc = "Commented";
                    break;
            }
            if (!string.IsNullOrEmpty(actionDesc))
                bodyValues.Add("Operation", actionDesc);
            //邮件内容里面的审批信息
            //流程发起人
            bodyValues.Add("Originator", majorLeaseInfoEntity.AssetActorNameENUS);
            bodyValues.Add("SN", task.K2SN);//SN
            bodyValues.Add("StepName", string.Empty);//
            var webRootUrl = ConfigurationManager.AppSettings["webHost"];
            var rootUrl = webRootUrl.EndsWith("/") ? webRootUrl.Substring(0, webRootUrl.LastIndexOf("/")) : webRootUrl;
            var formUrl = rootUrl + task.Url;
            if (task.ProcInstID.HasValue && !string.IsNullOrEmpty(task.RefID) && task.Url.ToLower().IndexOf(task.RefID.ToLower()) < 0)
            {
                formUrl = string.Format("{0}&projectId={1}", formUrl, task.RefID);
            }
            bodyValues.Add("FormUrl", HtmlConversionUtility.FormatPortalUrl(task.ReceiverAccount, formUrl, "zh-cn"));
            //邮件的Comments信息
            //获取TempClosurePackage的对象
            var majorLeasePackage = MajorLeaseChangePackage.GetMajorPackageInfo(task.RefID);
            var projectComment = GetLatestOperator(majorLeasePackage.TableName, majorLeasePackage.Id);
            bodyValues.Add("OperatorID", task.ReceiverAccount);//--任务接收人
            bodyValues.Add("Operator", projectComment.UserNameENUS);//--提交人
            bodyValues.Add("EntityID", majorLeasePackage.Id.ToString());

            //获取邮件正文的基本信息（Store Basic Info等）
            var storeBasicInfoDict = majorLeasePackage.GetPrintTemplateFields();

            //流程日志
            emailMessage = MailHelper.BuildEmailMessage(majorLeasePackage.TableName, majorLeasePackage.Id, FlowCode.MajorLease);
            //Store Basic Info
            emailMessage.StoreBasicInfoDict = storeBasicInfoDict;
            emailMessage.EmailBodyValues = bodyValues;
            var receiverList = new List<SimpleEmployee>();
            if (action == ActionLogType.Approve)
            {
                //获取邮件接收人
                var receiver = Employee.GetSimpleEmployeeByCode(task.ReceiverAccount);
                if (receiver.Code != projectComment.UserAccount)
                {
                    emailMessage.To = receiver.Mail;
                    receiverList.Add(receiver);
                }
            }
            else
            {
                receiverList = GetReturnANDRejectNotifyUserEmails(task, majorLeaseInfoEntity.AssetActorAccount, projectComment.UserAccount);
                emailMessage.To = String.Join(";", receiverList.Select(i => i.Mail).ToArray());
            }
            //获取邮件附件
            //var attList = MajorLeaseChangePackage.GetPackageAgreementList(majorLeasePackage.ProjectId, majorLeasePackage.Id.ToString(), SiteFilePath.UploadFiles_URL);
            if (action != ActionLogType.Recall)
            {
                var attList = Attachment.GetAllAttachmentsIncludeRequire(majorLeasePackage.TableName, task.RefID, FlowCode.MajorLease_Package);
                Dictionary<string, string> attachments = new Dictionary<string, string>();
                foreach (var att in attList)
                {
                    if (!string.IsNullOrEmpty(att.InternalName))
                    {
                        if (att.TypeCode == "Cover")
                            continue;
                        var fileExt = att.InternalName.Substring(att.InternalName.LastIndexOf("."));
                        var fileName = string.Empty;
                        if (!string.IsNullOrEmpty(att.RequireName))
                        {
                            fileName = att.RequireName + fileExt;
                        }
                        else
                        {
                            if (att.Name.IndexOf(".") < 0)
                            {
                                fileName = att.Name + fileExt;
                            }
                            else
                            {
                                fileName = att.Name;
                            }
                        }
                        string key = SiteFilePath.UploadFiles_DIRECTORY + "\\" + att.InternalName;
                        if (attachments.Keys.Contains(key)) continue;
                        attachments.Add(key, fileName);
                    }
                }
                emailMessage.AttachmentsDict = attachments;
            }
            //邮件模板Code,如果不指定，则会用默认的邮件模板发送邮件.
            emailMessage.TemplateCode = EmailTemplateCode.MajorLease_Package;

            #region Build NotifycationModule
            notification.EntityId = majorLeasePackage.Id;
            notification.FlowCode = task.TypeCode;
            notification.Operation = actionDesc;
            notification.OperatorCode = projectComment.UserAccount;
            notification.OperatorNameENUS = projectComment.UserNameENUS;
            notification.OperatorNameZHCN = projectComment.UserNameZHCN;
            notification.OriginatorNameENUS = majorLeaseInfoEntity.AssetActorNameENUS;
            notification.OriginatorNameZHCN = majorLeaseInfoEntity.AssetActorNameZHCN;
            notification.ProjectId = task.RefID;
            notification.ReceiverList = receiverList;
            notification.StoreCode = majorLeaseInfoEntity.USCode;
            notification.StoreNameENUS = majorLeaseInfoEntity.StoreNameENUS;
            notification.StoreNameZHCN = majorLeaseInfoEntity.StoreNameZHCN;
            #endregion

            return emailMessage;
        }

        static EmailMessage BuildReimagePackageEmailMessage(TaskWork task, NotificationModule notification, ref ActionLogType action)
        {
            EmailMessage emailMessage = null;
            Dictionary<string, string> bodyValues = new Dictionary<string, string>();
            var reimageInfoEntity = ReimageInfo.FirstOrDefault(i => i.ProjectId == task.RefID);
            if (action == ActionLogType.None)
            {
                if (task.ActivityName != "Start" && task.ActivityName != "Originator")
                    action = ActionLogType.Approve;
                else
                {
                    var projectInfo = ProjectInfo.Search(e => e.ProjectId == task.RefID && e.FlowCode == task.TypeCode).FirstOrDefault();
                    if (projectInfo != null && projectInfo.Status == ProjectStatus.Recalled)
                        action = ActionLogType.Recall;
                    else
                        action = ActionLogType.Return;
                }
            }
            //bodyValues.Add("ApplicantName", currentUser.NameENUS);//--提交人
            bodyValues.Add("StoreCode", reimageInfoEntity.USCode);
            bodyValues.Add("StoreName", reimageInfoEntity.StoreNameENUS);
            bodyValues.Add("Actor", reimageInfoEntity.AssetActorNameENUS);////--呈递人
            bodyValues.Add("ProjectName", FlowCode.Reimage);//项目名称
            bodyValues.Add("WorkflowName", FlowCode.Reimage_Package);////--流程名称
            var actionDesc = string.Empty;
            switch (action)
            {
                case ActionLogType.Approve:
                    actionDesc = "Approved";
                    break;
                case ActionLogType.Decline:
                    actionDesc = "Rejected";
                    break;
                case ActionLogType.Return:
                    actionDesc = "Returned";
                    break;
                case ActionLogType.Recall:
                    actionDesc = "Recalled";
                    break;
                case ActionLogType.Comments:
                    actionDesc = "Commented";
                    break;
            }
            if (!string.IsNullOrEmpty(actionDesc))
                bodyValues.Add("Operation", actionDesc);
            //邮件内容里面的审批信息
            //流程发起人
            bodyValues.Add("Originator", reimageInfoEntity.AssetActorNameENUS);
            bodyValues.Add("SN", task.K2SN);//SN
            bodyValues.Add("StepName", string.Empty);//
            var webRootUrl = ConfigurationManager.AppSettings["webHost"];
            var rootUrl = webRootUrl.EndsWith("/") ? webRootUrl.Substring(0, webRootUrl.LastIndexOf("/")) : webRootUrl;
            var formUrl = rootUrl + task.Url;
            if (task.ProcInstID.HasValue && !string.IsNullOrEmpty(task.RefID) && task.Url.ToLower().IndexOf(task.RefID.ToLower()) < 0)
            {
                formUrl = string.Format("{0}&projectId={1}", formUrl, task.RefID);
            }
            bodyValues.Add("FormUrl", HtmlConversionUtility.FormatPortalUrl(task.ReceiverAccount, formUrl, "zh-cn"));
            //邮件的Comments信息
            //获取TempClosurePackage的对象
            var reimagePackage = ReimagePackage.GetReimagePackageInfo(reimageInfoEntity.ProjectId);
            var projectComment = GetLatestOperator(ReimagePackage.TableName, reimagePackage.Id);
            bodyValues.Add("OperatorID", task.ReceiverAccount);//--任务接收人
            bodyValues.Add("Operator", projectComment.UserNameENUS);//--提交人
            bodyValues.Add("EntityID", reimagePackage.Id.ToString());

            //获取邮件正文的基本信息（Store Basic Info等）
            var storeBasicInfoDict = reimagePackage.GetPrintTemplateFields();

            //流程日志
            emailMessage = MailHelper.BuildEmailMessage(ReimagePackage.TableName, reimagePackage.Id, FlowCode.Reimage);
            //Store Basic Info
            emailMessage.StoreBasicInfoDict = storeBasicInfoDict;
            emailMessage.EmailBodyValues = bodyValues;
            var receiverList = new List<SimpleEmployee>();
            if (action == ActionLogType.Approve)
            {
                //获取邮件接收人
                var receiver = Employee.GetSimpleEmployeeByCode(task.ReceiverAccount);
                if (receiver.Code != projectComment.UserAccount)
                {
                    emailMessage.To = receiver.Mail;
                    receiverList.Add(receiver);
                }
            }
            else
            {
                receiverList = GetReturnANDRejectNotifyUserEmails(task, reimageInfoEntity.AssetActorAccount, projectComment.UserAccount);
                emailMessage.To = String.Join(";", receiverList.Select(i => i.Mail).ToArray());
            }
            //获取邮件附件
            if (action != ActionLogType.Recall)
            {
                var attList = Attachment.GetAllAttachmentsIncludeRequire(ReimagePackage.TableName, task.RefID, FlowCode.Reimage_Package);
                Dictionary<string, string> attachments = new Dictionary<string, string>();
                foreach (var att in attList)
                {
                    if (!string.IsNullOrEmpty(att.InternalName))
                    {
                        if (att.TypeCode == "Cover")
                            continue;
                        var fileExt = att.InternalName.Substring(att.InternalName.LastIndexOf("."));
                        var fileName = string.Empty;
                        if (!string.IsNullOrEmpty(att.RequireName))
                        {
                            fileName = att.RequireName + fileExt;
                        }
                        else
                        {
                            if (att.Name.IndexOf(".") < 0)
                            {
                                fileName = att.Name + fileExt;
                            }
                            else
                            {
                                fileName = att.Name;
                            }
                        }
                        string key = SiteFilePath.UploadFiles_DIRECTORY + "\\" + att.InternalName;
                        if (attachments.Keys.Contains(key)) continue;
                        attachments.Add(key, fileName);
                    }
                }
                emailMessage.AttachmentsDict = attachments;
            }
            //邮件模板Code,如果不指定，则会用默认的邮件模板发送邮件.
            emailMessage.TemplateCode = EmailTemplateCode.Reimage_Package;

            #region Build NotifycationModule
            notification.EntityId = reimagePackage.Id;
            notification.FlowCode = task.TypeCode;
            notification.Operation = actionDesc;
            notification.OperatorCode = projectComment.UserAccount;
            notification.OperatorNameENUS = projectComment.UserNameENUS;
            notification.OperatorNameZHCN = projectComment.UserNameZHCN;
            notification.OriginatorNameENUS = reimageInfoEntity.AssetActorNameENUS;
            notification.OriginatorNameZHCN = reimageInfoEntity.AssetActorNameZHCN;
            notification.ProjectId = task.RefID;
            notification.ReceiverList = receiverList;
            notification.StoreCode = reimageInfoEntity.USCode;
            notification.StoreNameENUS = reimageInfoEntity.StoreNameENUS;
            notification.StoreNameZHCN = reimageInfoEntity.StoreNameZHCN;
            #endregion

            return emailMessage;
        }

        static EmailMessage BuildRebuildPackageEmailMessage(TaskWork task, NotificationModule notification, ref ActionLogType action)
        {
            EmailMessage emailMessage = null;
            Dictionary<string, string> bodyValues = new Dictionary<string, string>();
            var rebuildInfoEntity = RebuildInfo.FirstOrDefault(i => i.ProjectId == task.RefID);
            if (action == ActionLogType.None)
            {
                if (task.ActivityName != "Start" && task.ActivityName != "Originator")
                    action = ActionLogType.Approve;
                else
                {
                    var projectInfo = ProjectInfo.Search(e => e.ProjectId == task.RefID && e.FlowCode == task.TypeCode).FirstOrDefault();
                    if (projectInfo != null && projectInfo.Status == ProjectStatus.Recalled)
                        action = ActionLogType.Recall;
                    else
                        action = ActionLogType.Return;
                }
            }
            //bodyValues.Add("ApplicantName", currentUser.NameENUS);//--提交人
            bodyValues.Add("StoreCode", rebuildInfoEntity.USCode);
            bodyValues.Add("StoreName", rebuildInfoEntity.StoreNameENUS);
            bodyValues.Add("Actor", rebuildInfoEntity.AssetActorNameENUS);////--呈递人
            bodyValues.Add("ProjectName", FlowCode.Rebuild);//项目名称
            bodyValues.Add("WorkflowName", FlowCode.Rebuild_Package);////--流程名称
            var actionDesc = string.Empty;
            switch (action)
            {
                case ActionLogType.Approve:
                    actionDesc = "Approved";
                    break;
                case ActionLogType.Decline:
                    actionDesc = "Rejected";
                    break;
                case ActionLogType.Return:
                    actionDesc = "Returned";
                    break;
                case ActionLogType.Recall:
                    actionDesc = "Recalled";
                    break;
                case ActionLogType.Comments:
                    actionDesc = "Commented";
                    break;
            }
            if (!string.IsNullOrEmpty(actionDesc))
                bodyValues.Add("Operation", actionDesc);
            //邮件内容里面的审批信息
            //流程发起人
            bodyValues.Add("Originator", rebuildInfoEntity.AssetActorNameENUS);
            bodyValues.Add("SN", task.K2SN);//SN
            bodyValues.Add("StepName", string.Empty);//
            var webRootUrl = ConfigurationManager.AppSettings["webHost"];
            var rootUrl = webRootUrl.EndsWith("/") ? webRootUrl.Substring(0, webRootUrl.LastIndexOf("/")) : webRootUrl;
            var formUrl = rootUrl + task.Url;
            if (task.ProcInstID.HasValue && !string.IsNullOrEmpty(task.RefID) && task.Url.ToLower().IndexOf(task.RefID.ToLower()) < 0)
            {
                formUrl = string.Format("{0}&projectId={1}", formUrl, task.RefID);
            }
            bodyValues.Add("FormUrl", HtmlConversionUtility.FormatPortalUrl(task.ReceiverAccount, formUrl, "zh-cn"));
            //邮件的Comments信息
            //获取TempClosurePackage的对象
            var rebuildPackage = RebuildPackage.GetRebuildPackageInfo(rebuildInfoEntity.ProjectId);
            var projectComment = GetLatestOperator(RebuildPackage.TableName, rebuildPackage.Id);
            bodyValues.Add("OperatorID", task.ReceiverAccount);//--任务接收人
            bodyValues.Add("Operator", projectComment.UserNameENUS);//--提交人
            bodyValues.Add("EntityID", rebuildPackage.Id.ToString());

            //获取邮件正文的基本信息（Store Basic Info等）
            var storeBasicInfoDict = rebuildPackage.GetPrintTemplateFields();

            //流程日志
            emailMessage = MailHelper.BuildEmailMessage(RebuildPackage.TableName, rebuildPackage.Id, FlowCode.Rebuild);
            //Store Basic Info
            emailMessage.StoreBasicInfoDict = storeBasicInfoDict;
            emailMessage.EmailBodyValues = bodyValues;
            var receiverList = new List<SimpleEmployee>();
            if (action == ActionLogType.Approve)
            {
                //获取邮件接收人
                var receiver = Employee.GetSimpleEmployeeByCode(task.ReceiverAccount);
                if (receiver.Code != projectComment.UserAccount)
                {
                    emailMessage.To = receiver.Mail;
                    receiverList.Add(receiver);
                }
            }
            else
            {
                receiverList = GetReturnANDRejectNotifyUserEmails(task, rebuildInfoEntity.AssetActorAccount, projectComment.UserAccount);
                emailMessage.To = String.Join(";", receiverList.Select(i => i.Mail).ToArray());
            }
            //获取邮件附件
            if (action != ActionLogType.Recall)
            {
                var attList = RebuildPackage.GetPackageAgreementList(task.RefID, rebuildPackage.Id.ToString(), SiteFilePath.UploadFiles_URL);
                Dictionary<string, string> attachments = new Dictionary<string, string>();
                foreach (var att in attList)
                {
                    if (!string.IsNullOrEmpty(att.InternalName))
                    {
                        if (att.TypeCode == "Cover")
                            continue;
                        var fileExt = att.InternalName.Substring(att.InternalName.LastIndexOf("."));
                        var fileName = string.Empty;
                        if (!string.IsNullOrEmpty(att.RequireName))
                        {
                            fileName = att.RequireName + fileExt;
                        }
                        else
                        {
                            if (att.Name.IndexOf(".") < 0)
                            {
                                fileName = att.Name + fileExt;
                            }
                            else
                            {
                                fileName = att.Name;
                            }
                        }
                        string key = SiteFilePath.UploadFiles_DIRECTORY + "\\" + att.InternalName;
                        if (attachments.Keys.Contains(key)) continue;
                        attachments.Add(key, fileName);
                    }
                }
                emailMessage.AttachmentsDict = attachments;
            }
            //邮件模板Code,如果不指定，则会用默认的邮件模板发送邮件.
            emailMessage.TemplateCode = EmailTemplateCode.Rebuild_Package;

            #region Build NotifycationModule
            notification.EntityId = rebuildPackage.Id;
            notification.FlowCode = task.TypeCode;
            notification.Operation = actionDesc;
            notification.OperatorCode = projectComment.UserAccount;
            notification.OperatorNameENUS = projectComment.UserNameENUS;
            notification.OperatorNameZHCN = projectComment.UserNameZHCN;
            notification.OriginatorNameENUS = rebuildInfoEntity.AssetActorNameENUS;
            notification.OriginatorNameZHCN = rebuildInfoEntity.AssetActorNameZHCN;
            notification.ProjectId = task.RefID;
            notification.ReceiverList = receiverList;
            notification.StoreCode = rebuildInfoEntity.USCode;
            notification.StoreNameENUS = rebuildInfoEntity.StoreNameENUS;
            notification.StoreNameZHCN = rebuildInfoEntity.StoreNameZHCN;
            #endregion

            return emailMessage;
        }

        static EmailMessage BuildRenewalPackageEmailMessage(TaskWork task, NotificationModule notification, ref ActionLogType action)
        {
            EmailMessage emailMessage = null;
            Dictionary<string, string> bodyValues = new Dictionary<string, string>();
            var renewalInfoEntity = RenewalInfo.FirstOrDefault(i => i.ProjectId == task.RefID);
            if (action == ActionLogType.None)
            {
                if (task.ActivityName != "Start" && task.ActivityName != "Originator")
                    action = ActionLogType.Approve;
                else
                {
                    var projectInfo = ProjectInfo.Search(e => e.ProjectId == task.RefID && e.FlowCode == task.TypeCode).FirstOrDefault();
                    if (projectInfo != null && projectInfo.Status == ProjectStatus.Recalled)
                        action = ActionLogType.Recall;
                    else
                        action = ActionLogType.Return;
                }
            }
            //bodyValues.Add("ApplicantName", currentUser.NameENUS);//--提交人
            bodyValues.Add("StoreCode", renewalInfoEntity.USCode);
            bodyValues.Add("StoreName", renewalInfoEntity.StoreNameENUS);
            bodyValues.Add("Actor", renewalInfoEntity.AssetActorNameENUS);////--呈递人
            bodyValues.Add("ProjectName", FlowCode.Renewal);//项目名称
            bodyValues.Add("WorkflowName", FlowCode.Renewal_Package);////--流程名称
            var actionDesc = string.Empty;
            switch (action)
            {
                case ActionLogType.Approve:
                    actionDesc = "Approved";
                    break;
                case ActionLogType.Decline:
                    actionDesc = "Rejected";
                    break;
                case ActionLogType.Return:
                    actionDesc = "Returned";
                    break;
                case ActionLogType.Recall:
                    actionDesc = "Recalled";
                    break;
                case ActionLogType.Comments:
                    actionDesc = "Commented";
                    break;
            }
            if (!string.IsNullOrEmpty(actionDesc))
                bodyValues.Add("Operation", actionDesc);
            //邮件内容里面的审批信息
            //流程发起人
            bodyValues.Add("Originator", renewalInfoEntity.AssetActorNameENUS);
            bodyValues.Add("SN", task.K2SN);//SN
            bodyValues.Add("StepName", string.Empty);//
            var webRootUrl = ConfigurationManager.AppSettings["webHost"];
            var rootUrl = webRootUrl.EndsWith("/") ? webRootUrl.Substring(0, webRootUrl.LastIndexOf("/")) : webRootUrl;
            var formUrl = rootUrl + task.Url;
            if (task.ProcInstID.HasValue && !string.IsNullOrEmpty(task.RefID) && task.Url.ToLower().IndexOf(task.RefID.ToLower()) < 0)
            {
                formUrl = string.Format("{0}&projectId={1}", formUrl, task.RefID);
            }
            bodyValues.Add("FormUrl", HtmlConversionUtility.FormatPortalUrl(task.ReceiverAccount, formUrl, "zh-cn"));
            //邮件的Comments信息
            //获取TempClosurePackage的对象
            var renewalPackage = RenewalPackage.Get(renewalInfoEntity.ProjectId);
            var projectComment = GetLatestOperator(renewalPackage.TableName, renewalPackage.Id);
            bodyValues.Add("OperatorID", task.ReceiverAccount);//--任务接收人
            bodyValues.Add("Operator", projectComment.UserNameENUS);//--提交人
            bodyValues.Add("EntityID", renewalPackage.Id.ToString());

            //获取邮件正文的基本信息（Store Basic Info等）
            var storeBasicInfoDict = renewalPackage.GetPrintTemplateFields();

            //流程日志
            emailMessage = MailHelper.BuildEmailMessage(renewalPackage.TableName, renewalPackage.Id, FlowCode.Renewal);
            //Store Basic Info
            emailMessage.StoreBasicInfoDict = storeBasicInfoDict;
            emailMessage.EmailBodyValues = bodyValues;
            var receiverList = new List<SimpleEmployee>();
            if (action == ActionLogType.Approve)
            {
                //获取邮件接收人
                var receiver = Employee.GetSimpleEmployeeByCode(task.ReceiverAccount);
                if (receiver.Code != projectComment.UserAccount)
                {
                    emailMessage.To = receiver.Mail;
                    receiverList.Add(receiver);
                }
            }
            else
            {
                receiverList = GetReturnANDRejectNotifyUserEmails(task, renewalInfoEntity.AssetActorAccount, projectComment.UserAccount);
                emailMessage.To = String.Join(";", receiverList.Select(i => i.Mail).ToArray());
            }
            //获取邮件附件
            if (action != ActionLogType.Recall)
            {
                var attList = Attachment.GetAllAttachmentsIncludeRequire(renewalPackage.TableName, task.RefID, FlowCode.Renewal_Package);
                Dictionary<string, string> attachments = new Dictionary<string, string>();
                foreach (var att in attList)
                {
                    if (!string.IsNullOrEmpty(att.InternalName))
                    {
                        if (att.TypeCode == "Cover")
                            continue;
                        var index = att.InternalName.LastIndexOf(".");
                        var fileExt = att.InternalName.Substring(index < 0 ? 0 : index);
                        var fileName = string.Empty;
                        if (!string.IsNullOrEmpty(att.RequireName))
                        {
                            fileName = att.RequireName + fileExt;
                        }
                        else
                        {
                            if (att.Name.IndexOf(".") < 0)
                            {
                                fileName = att.Name + fileExt;
                            }
                            else
                            {
                                fileName = att.Name;
                            }
                        }
                        string key = SiteFilePath.UploadFiles_DIRECTORY + "\\" + att.InternalName;
                        if (attachments.Keys.Contains(key)) continue;
                        attachments.Add(key, fileName);
                    }
                }
                emailMessage.AttachmentsDict = attachments;
            }
            //邮件模板Code,如果不指定，则会用默认的邮件模板发送邮件.
            emailMessage.TemplateCode = EmailTemplateCode.Renewal_Package;

            #region Build NotifycationModule
            notification.EntityId = renewalPackage.Id;
            notification.FlowCode = task.TypeCode;
            notification.Operation = actionDesc;
            notification.OperatorCode = projectComment.UserAccount;
            notification.OperatorNameENUS = projectComment.UserNameENUS;
            notification.OperatorNameZHCN = projectComment.UserNameZHCN;
            notification.OriginatorNameENUS = renewalInfoEntity.AssetActorNameENUS;
            notification.OriginatorNameZHCN = renewalInfoEntity.AssetActorNameZHCN;
            notification.ProjectId = task.RefID;
            notification.ReceiverList = receiverList;
            notification.StoreCode = renewalInfoEntity.USCode;
            notification.StoreNameENUS = renewalInfoEntity.StoreNameENUS;
            notification.StoreNameZHCN = renewalInfoEntity.StoreNameZHCN;
            #endregion

            return emailMessage;
        }

        /// <summary>
        /// 获取已经处理过任务的员工的邮件地址列表
        /// </summary>
        /// <param name="projectId"></param>
        /// <param name="flowCode"></param>
        /// <param name="isAddComment"></param>
        /// <returns></returns>
        public static string GetWorkflowRelationUserEmails(string projectId, string flowCode, NotificationModule notificationModule, bool isAddComment = false)
        {
            #region 获取操作人
            var projectComment = new ProjectComment();
            switch (flowCode)
            {
                case FlowCode.Closure_ClosurePackage:
                    {
                        var closurePackage = ClosurePackage.Get(projectId);
                        projectComment = GetLatestOperator(ClosurePackage.TableName, closurePackage.Id);
                    }
                    break;
                case FlowCode.TempClosure_ClosurePackage:
                    {
                        var tempClosurePackage = TempClosurePackage.Get(projectId);
                        projectComment = GetLatestOperator(tempClosurePackage.TableName, tempClosurePackage.Id);
                    }
                    break;
                case FlowCode.MajorLease_Package:
                    {
                        var majorLeasePackage = MajorLeaseChangePackage.GetMajorPackageInfo(projectId);
                        projectComment = GetLatestOperator(majorLeasePackage.TableName, majorLeasePackage.Id);
                    }
                    break;
                case FlowCode.Rebuild_Package:
                    {
                        var rebuildPackage = RebuildPackage.GetRebuildPackageInfo(projectId);
                        projectComment = GetLatestOperator(RebuildPackage.TableName, rebuildPackage.Id);
                    }
                    break;
                case FlowCode.Renewal_Package:
                    {
                        var renewalPackage = RenewalPackage.Get(projectId);
                        projectComment = GetLatestOperator(renewalPackage.TableName, renewalPackage.Id);
                    }
                    break;
                case FlowCode.Reimage_Package:
                    {
                        var reimagePackage = ReimagePackage.GetReimagePackageInfo(projectId);
                        projectComment = GetLatestOperator(ReimagePackage.TableName, reimagePackage.Id);
                    }
                    break;
                default:
                    break;
            }
            #endregion

            var simpleReceiverList = new List<SimpleEmployee>();

            //流程相关处理人
            var userCodeList = TaskWork.GetPackageProcessedUsers(projectId, flowCode);
            foreach (string eid in userCodeList)
            {
                //排除操作人自己
                if (eid.Equals(projectComment.UserAccount))
                    continue;

                var employee = Employee.GetSimpleEmployeeByCode(eid);
                if (employee != null && !string.IsNullOrEmpty(employee.Mail) && !simpleReceiverList.Contains(employee))
                    simpleReceiverList.Add(employee);
            }

            //流程必要抄送人
            userCodeList = ApproveDialogUser.GetNotifyDialogUser(projectId, flowCode, true);
            foreach (string eid in userCodeList)
            {
                //排除操作人自己
                if (eid.Equals(projectComment.UserAccount))
                    continue;

                var employee = Employee.GetSimpleEmployeeByCode(eid);
                if (employee != null && !string.IsNullOrEmpty(employee.Mail) && !simpleReceiverList.Contains(employee))
                    simpleReceiverList.Add(employee);
            }

            //流程非必要抄送人
            userCodeList = ApproveDialogUser.GetNotifyDialogUser(projectId, flowCode, false);
            foreach (string eid in userCodeList)
            {
                //排除操作人自己
                if (eid.Equals(projectComment.UserAccount))
                    continue;

                var employee = Employee.GetSimpleEmployeeByCode(eid);
                if (employee != null && !string.IsNullOrEmpty(employee.Mail) && !simpleReceiverList.Contains(employee))
                    simpleReceiverList.Add(employee);
            }

            //Add Comment时，同环节多人审批，其他未审批人也要收到邮件
            if (isAddComment)
            {
                userCodeList = TaskWork.GetPackageProcessingUsers(projectId, flowCode);
                foreach (string eid in userCodeList)
                {
                    if (eid.Equals(projectComment.UserAccount))
                        continue;

                    var employee = Employee.GetSimpleEmployeeByCode(eid);
                    if (employee != null && !string.IsNullOrEmpty(employee.Mail) && !simpleReceiverList.Contains(employee))
                        simpleReceiverList.Add(employee);
                }
            }
            //发送站内Notice
            Remind.Send(notificationModule.RemindInfo, simpleReceiverList, notificationModule.ProjectId, notificationModule.FlowCode);

            return String.Join(";", simpleReceiverList.Select(i => i.Mail).ToArray());
        }

        //获取通知用户的邮件地址
        public static string GetNotifyUserEmails(string projectId, string flowCode, string commentsBy)
        {
            string notifyUserEmails = string.Empty;
            var userCodeList = new List<string>();
            //var specialNotifyUserEid = ApproveDialogUser.GetNotifyDialogUserByRole(projectId, flowCode, RoleCode.MCCL_Asset_Mgr);
            //if (!string.IsNullOrEmpty(specialNotifyUserEid)) userCodeList.Add(specialNotifyUserEid);
            userCodeList = ApproveDialogUser.GetNotifyDialogUser(projectId, flowCode, true);
            foreach (string eid in userCodeList)
            {
                if (!string.IsNullOrEmpty(commentsBy) && eid.Equals(commentsBy))
                    continue;//排除自己添加comments的时候，会发给自己邮件。为了测试方便，暂时不添加此限制。
                var employee = Employee.GetSimpleEmployeeByCode(eid);
                if (employee != null && !string.IsNullOrEmpty(employee.Mail))
                {
                    notifyUserEmails += employee.Mail + ";";
                }
            }
            userCodeList = ApproveDialogUser.GetNotifyDialogUser(projectId, flowCode);
            foreach (string eid in userCodeList)
            {
                //if (!string.IsNullOrEmpty(commentsBy) && eid.Equals(commentsBy)) continue;//排除自己添加comments的时候，会发给自己邮件。为了测试方便，暂时不添加此限制。
                var employee = Employee.GetSimpleEmployeeByCode(eid);
                if (employee != null && !string.IsNullOrEmpty(employee.Mail))
                {
                    notifyUserEmails += employee.Mail + ";";
                }
            }
            return notifyUserEmails;
        }

        //获取通知用户的邮件地址
        public static string GetNotifyUserEmails(string sn)
        {
            string notifyUserEmails = string.Empty;
            var task = TaskWork.GetTaskBySN(sn);
            if (task == null) return notifyUserEmails;
            switch (task.TypeCode)
            {
                case FlowCode.Closure_ClosurePackage:
                    {
                        var closureInfoEntity = ClosureInfo.GetByProjectId(task.RefID);
                        var closurePackage = ClosurePackage.Get(closureInfoEntity.ProjectId);
                        var projectComment = GetLatestOperator(ClosurePackage.TableName, closurePackage.Id);
                        notifyUserEmails = GetNotifyUserEmails(closureInfoEntity.ProjectId, FlowCode.Closure_ClosurePackage, projectComment.UserAccount);
                    }
                    break;
                case FlowCode.TempClosure_ClosurePackage:
                    {
                        var tempClosureInfoEntity = TempClosureInfo.FirstOrDefault(i => i.ProjectId == task.RefID);
                        var tempClosurePackage = TempClosurePackage.Get(tempClosureInfoEntity.ProjectId);
                        var projectComment = GetLatestOperator(tempClosurePackage.TableName, tempClosurePackage.Id);
                        notifyUserEmails = GetNotifyUserEmails(tempClosureInfoEntity.ProjectId, FlowCode.TempClosure_ClosurePackage, projectComment.UserAccount);
                    }
                    break;
                case FlowCode.MajorLease_Package:
                    {
                        var majorLeaseInfoEntity = MajorLeaseInfo.FirstOrDefault(i => i.ProjectId == task.RefID);
                        var majorLeasePackage = MajorLeaseChangePackage.GetMajorPackageInfo(task.RefID);
                        var projectComment = GetLatestOperator(majorLeasePackage.TableName, majorLeasePackage.Id);
                        notifyUserEmails = GetNotifyUserEmails(majorLeaseInfoEntity.ProjectId, FlowCode.MajorLease_Package, projectComment.UserAccount);
                    }
                    break;
                case FlowCode.Rebuild_Package:
                    {
                        var rebuildInfoEntity = RebuildInfo.FirstOrDefault(i => i.ProjectId == task.RefID);
                        var rebuildPackage = RebuildPackage.GetRebuildPackageInfo(rebuildInfoEntity.ProjectId);
                        var projectComment = GetLatestOperator(RebuildPackage.TableName, rebuildPackage.Id);
                        notifyUserEmails = GetNotifyUserEmails(rebuildInfoEntity.ProjectId, FlowCode.Rebuild_Package, projectComment.UserAccount);
                    }
                    break;
                case FlowCode.Renewal_Package:
                    {
                        var renewalInfoEntity = RenewalInfo.FirstOrDefault(i => i.ProjectId == task.RefID);
                        var renewalPackage = RenewalPackage.Get(renewalInfoEntity.ProjectId);
                        var projectComment = GetLatestOperator(renewalPackage.TableName, renewalPackage.Id);
                        notifyUserEmails = GetNotifyUserEmails(renewalInfoEntity.ProjectId, FlowCode.Renewal_Package, projectComment.UserAccount);
                    }
                    break;
                case FlowCode.Reimage_Package:
                    {
                        var reimageInfoEntity = ReimageInfo.FirstOrDefault(i => i.ProjectId == task.RefID);
                        var reimagePackage = ReimagePackage.GetReimagePackageInfo(reimageInfoEntity.ProjectId);
                        var projectComment = GetLatestOperator(ReimagePackage.TableName, reimagePackage.Id);
                        notifyUserEmails = GetNotifyUserEmails(reimageInfoEntity.ProjectId, FlowCode.Reimage_Package, projectComment.UserAccount);
                    }
                    break;
                default:
                    break;
            }
            return notifyUserEmails;
        }

        /// <summary>
        /// Return或Reject时，获取所有参与过流程审批人
        /// </summary>
        /// <param name="task"></param>
        /// <param name="actorAccount"></param>
        /// <param name="operatorAccount"></param>
        /// <returns></returns>
        public static List<SimpleEmployee> GetReturnANDRejectNotifyUserEmails(TaskWork task, string actorAccount, string operatorAccount)
        {
            List<SimpleEmployee> notifyUsers = new List<SimpleEmployee>();
            if (task == null) return notifyUsers;
            //获取Actor的发起任务
            var lastActorTask = TaskWork.Search(t => t.RefID == task.RefID && t.TypeCode == task.TypeCode && t.Status != TaskWorkStatus.UnFinish && t.ReceiverAccount == actorAccount).OrderByDescending(i => i.CreateTime).ToList();
            if (lastActorTask.Count > 0)
            {
                var restartTime = lastActorTask[0].CreateTime;
                var taskList = TaskWork.Search(t => t.RefID == task.RefID && t.TypeCode == task.TypeCode && t.CreateTime >= restartTime);
                List<string> receiverList = new List<string>();
                foreach (var taskItem in taskList)
                {
                    if (!receiverList.Contains(taskItem.ReceiverAccount) && taskItem.ReceiverAccount != operatorAccount)
                        receiverList.Add(taskItem.ReceiverAccount);
                }
                foreach (string reItem in receiverList)
                {
                    var employee = Employee.GetSimpleEmployeeByCode(reItem);
                    notifyUsers.Add(employee);
                }
            }
            return notifyUsers;
        }

        /// <summary>
        /// 获取最后那条审批意见
        /// </summary>
        /// <param name="RefTableName"></param>
        /// <param name="RefTableId"></param>
        /// <returns></returns>
        public static ProjectComment GetLatestOperator(string RefTableName, Guid RefTableId)
        {
            var projectComment = ProjectComment.Search(comment => comment.Status == ProjectCommentStatus.Submit && comment.RefTableName == RefTableName && comment.RefTableId == RefTableId).OrderByDescending(comment => comment.CreateTime).ToList();
            if (projectComment.Count > 0)
                return projectComment[0];
            else
                return null;
        }

        public class NotificationModule
        {
            public string FlowCode { get; set; }
            public string ProjectId { get; set; }
            public string OperatorCode { get; set; }
            public string OperatorNameENUS { get; set; }
            public string OperatorNameZHCN { get; set; }
            public string OriginatorNameENUS { get; set; }
            public string OriginatorNameZHCN { get; set; }
            public string StoreCode { get; set; }
            public string StoreNameENUS { get; set; }
            public string StoreNameZHCN { get; set; }
            public List<SimpleEmployee> ReceiverList { get; set; }
            public Guid EntityId { get; set; }
            public string Operation { get; set; }
            public Remind RemindInfo { get; set; }
        }
    }
}