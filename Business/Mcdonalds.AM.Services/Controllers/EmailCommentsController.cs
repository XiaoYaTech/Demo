using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.Services.Entities;
using NTTMNC.BPM.Fx.Core;
using NTTMNC.BPM.Fx.EmailServices;
using NTTMNC.BPM.Fx.K2.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers
{
    public class EmailCommentsController : ApiController
    {
        [Route("api/email/addcomments")]
        [HttpPost]
        public IHttpActionResult AddComments(WorkflowStepInfo comments)
        {
            var entityQuid = new Guid(comments.EntityID);
            switch (comments.WorkflowName)
            {
                case FlowCode.Closure_ClosurePackage:
                    var closurePackage = ClosurePackage.Get(entityQuid);
                    AddComments(comments, closurePackage.Id, ClosurePackage.TableName, closurePackage.ProcInstID, closurePackage.ProjectId, FlowCode.Closure, EmailTemplateCode.Closure_Package);
                    break;
                case FlowCode.TempClosure_ClosurePackage:
                    var tempClosurePackage = TempClosurePackage.Get(entityQuid);
                    AddComments(comments, tempClosurePackage.Id, tempClosurePackage.TableName, tempClosurePackage.ProcInstId, tempClosurePackage.ProjectId, FlowCode.TempClosure, EmailTemplateCode.TempClosure_Package);
                    break;
                case FlowCode.MajorLease_Package:
                    var majorLeasePackage = MajorLeaseChangePackage.Get(entityQuid);
                    AddComments(comments, majorLeasePackage.Id, majorLeasePackage.TableName, majorLeasePackage.ProcInstID, majorLeasePackage.ProjectId, FlowCode.MajorLease, EmailTemplateCode.MajorLease_Package);
                    break;
                case FlowCode.Rebuild_Package:
                    var rebuildPackage = RebuildPackage.Get(entityQuid);
                    AddComments(comments, rebuildPackage.Id, RebuildPackage.TableName, rebuildPackage.ProcInstID, rebuildPackage.ProjectId, FlowCode.Rebuild, EmailTemplateCode.Rebuild_Package);
                    break;
                case FlowCode.Renewal_Package:
                    var renewalPackage = RenewalPackage.Get(entityQuid);
                    AddComments(comments, renewalPackage.Id, renewalPackage.TableName, renewalPackage.ProcInstId, renewalPackage.ProjectId, FlowCode.Renewal, EmailTemplateCode.Renewal_Package);
                    break;
                case FlowCode.Reimage_Package:
                    var reimagePackage = ReimagePackage.Get(entityQuid);
                    AddComments(comments, reimagePackage.Id, ReimagePackage.TableName, reimagePackage.ProcInstID, reimagePackage.ProjectId, FlowCode.Reimage, EmailTemplateCode.Reimage_Package);
                    break;
                default:
                    break;
            }
            return Ok(true);
        }

        /// <summary>
        /// 邮件审批return时，task状态不会由K2设置，需处理一下
        /// </summary>
        /// <param name="comments"></param>
        /// <returns></returns>
        [Route("api/email/returnTask")]
        [HttpPost]
        public IHttpActionResult ReturnTask(WorkflowStepInfo info)
        {
            var task = TaskWork.GetTaskBySN(info.SN);
            if (task == null || task.Status != TaskWorkStatus.UnFinish)
                return Ok();

            var entityId = new Guid(info.EntityID);
            string projectId = string.Empty;
            switch (info.WorkflowName)
            {
                case FlowCode.Closure_ClosurePackage:
                    var closurePackage = ClosurePackage.Get(entityId);
                    projectId = closurePackage.ProjectId;
                    break;
                case FlowCode.TempClosure_ClosurePackage:
                    var tempClosurePackage = TempClosurePackage.Get(entityId);
                    projectId = tempClosurePackage.ProjectId;
                    break;
                case FlowCode.MajorLease_Package:
                    var majorLeasePackage = MajorLeaseChangePackage.Get(entityId);
                    projectId = majorLeasePackage.ProjectId;
                    break;
                case FlowCode.Rebuild_Package:
                    var rebuildPackage = RebuildPackage.Get(entityId);
                    projectId = rebuildPackage.ProjectId;
                    break;
                case FlowCode.Renewal_Package:
                    var renewalPackage = RenewalPackage.Get(entityId);
                    projectId = renewalPackage.ProjectId;
                    break;
                case FlowCode.Reimage_Package:
                    var reimagePackage = RenewalPackage.Get(entityId);
                    projectId = reimagePackage.ProjectId;
                    break;
                default:
                    break;
            }
            if (!string.IsNullOrEmpty(projectId))
                TaskWork.Finish(e => e.RefID == projectId && e.TypeCode == info.WorkflowName && e.Status == TaskWorkStatus.UnFinish && e.ActivityName != "Originator");
            return Ok();
        }

        /// <summary>
        /// 添加审批邮件的Comments
        /// </summary>
        /// <param name="comments"></param>
        /// <param name="refTableId"></param>
        /// <param name="procInstID"></param>
        /// <param name="projectId"></param>
        private void AddComments(WorkflowStepInfo comments, Guid refTableId, string refTableName, int? procInstID, string projectId, string flowCode, string templateCode)
        {
            //获取操作人信息
            var user = Employee.GetSimpleEmployeeByCode(comments.OperatorID);
            ProjectComment projectcomment = new ProjectComment();
            projectcomment.RefTableId = refTableId;
            projectcomment.RefTableName = refTableName;
            projectcomment.TitleNameENUS = user.PositionENUS;
            projectcomment.TitleNameZHCN = user.PositionZHCN;
            projectcomment.TitleCode = string.Empty;
            projectcomment.Status = ProjectCommentStatus.Submit;

            projectcomment.CreateTime = DateTime.Now;
            projectcomment.CreateUserAccount = user.Code;

            projectcomment.UserAccount = user.Code;
            projectcomment.UserNameENUS = user.NameENUS;
            projectcomment.UserNameZHCN = user.NameZHCN;
            projectcomment.CreateUserNameZHCN = user.NameZHCN;
            if (procInstID > 0)
            {
                projectcomment.ProcInstID = procInstID;
            }
            projectcomment.Id = Guid.NewGuid();
            if (!string.IsNullOrEmpty(comments.Comments) && !comments.Comments.Equals(Constants.EmptyComments))
            {
                projectcomment.Content = comments.Comments.Trim();
            }
            projectcomment.Action = "Comments";
            projectcomment.Status = ProjectCommentStatus.Submit;

            projectcomment.SourceCode = flowCode;
            projectcomment.SourceNameENUS = flowCode;
            projectcomment.SourceNameZHCN = flowCode;
            projectcomment.Add();

            //using (var db = new McdAMEntities())
            //{
            //    db.ProjectComment.Add(projectcomment);
            //    db.SaveChanges();
            //}
            //邮件通知邮件干系人
            string emails = string.Empty;
            EmailServiceReference.EmailMessage emailMessage = new EmailServiceReference.EmailMessage();
            var notificationModule = new Mcdonalds.AM.Services.Common.MailHelper.NotificationModule();
            if (comments != null && !string.IsNullOrEmpty(comments.SN) && !string.IsNullOrEmpty(comments.OperatorID))
            {
                emailMessage = MailHelper.BuildEmailMessage(comments.SN, ActionLogType.Comments, ref notificationModule);
                emails = MailHelper.GetWorkflowRelationUserEmails(projectId, comments.WorkflowName, notificationModule, true);

            }
            //获取邮件的提醒人
            //var relationEmails = MailHelper.GetNotifyUserEmails(projectId, comments.WorkflowName, comments.OperatorID);
            //if (!string.IsNullOrEmpty(relationEmails))
            //{
            //    emails += relationEmails;
            //}
            emailMessage.To = emails;
            emailMessage.TemplateCode = templateCode;
            MailHelper.SendCommentsEmail(emailMessage);
        }

        /// <summary>
        /// 添加邮件备注里面
        /// </summary>
        /// <returns></returns>
        [Route("api/email/AddEmailAuditComment")]
        [HttpPost]
        public IHttpActionResult AddEmailComment(WorkflowEmailComment emailComment)
        {
            try
            {
                if (emailComment == null)
                {
                    Log4netHelper.WriteErrorLog("Email comments 对象为空!");
                    return Ok(false);
                }
                Log4netHelper.WriteInfo(
                    string.Format("Email Comment InputVals:K2SN:{0};OperatorID:{1};Action:{2};EmailComments:{3}", emailComment.K2SN, emailComment.OperatorID, emailComment.Action, emailComment.EmailComments)
                );

                if (string.IsNullOrEmpty(emailComment.K2SN) || string.IsNullOrEmpty(emailComment.OperatorID))
                {
                    Log4netHelper.WriteErrorLog("K2 SN 或操作人不能为空!");
                    return Ok(false);
                }

                string activity = string.Empty;
                try
                {
                    activity = K2FxContext.Current.GetCurrentActivityName(emailComment.K2SN, emailComment.OperatorID);
                }
                catch { }

                var taskItem = TaskWork.Search(c => c.K2SN == emailComment.K2SN).FirstOrDefault();
                if (taskItem == null)
                {
                    Log4netHelper.WriteErrorLog("不能获取当前的任务对象!");
                    return Ok(false);
                }

                var empInfo = Employee.GetSimpleEmployeeByCode(emailComment.OperatorID);

                ProjectComment projectComment = new ProjectComment();
                projectComment.Id = Guid.NewGuid();
                projectComment.UserAccount = projectComment.CreateUserAccount = emailComment.OperatorID;

                if (empInfo != null)
                {
                    projectComment.UserNameENUS = empInfo.NameENUS;
                    projectComment.UserNameZHCN = empInfo.NameZHCN;
                    projectComment.CreateUserNameENUS = empInfo.NameENUS;
                    projectComment.CreateUserNameZHCN = empInfo.NameZHCN;
                    projectComment.TitleCode = empInfo.TitleCode;
                    projectComment.TitleNameENUS = empInfo.TitleENUS;
                    projectComment.TitleNameZHCN = empInfo.TitleZHCN;
                }
                var existProjectComment = ProjectComment.Search(
                                    o => o.ProcInstID == taskItem.ProcInstID).FirstOrDefault();
                if (existProjectComment != null)
                {
                    projectComment.RefTableId = existProjectComment.RefTableId;
                    projectComment.RefTableName = existProjectComment.RefTableName;
                }

                projectComment.SourceCode = taskItem.SourceCode;
                projectComment.SourceNameENUS = taskItem.SourceNameENUS;
                projectComment.SourceNameZHCN = taskItem.SourceNameZHCN;

                projectComment.Action = emailComment.Action;
                projectComment.CreateTime = DateTime.Now;
                projectComment.Content = emailComment.EmailComments;
                projectComment.Status = ProjectCommentStatus.Submit;

                projectComment.ProcInstID = taskItem.ProcInstID;
                projectComment.ActivityName = activity;
                if (!string.IsNullOrEmpty(emailComment.EmailComments) && !emailComment.EmailComments.Equals(Constants.EmptyComments))
                {
                    projectComment.Content = emailComment.EmailComments.Trim();
                }
                projectComment.Add();
                //using (var db = new McdAMEntities())
                //{
                //    db.ProjectComment.Add(projectComment);
                //    db.SaveChanges();
                //}
            }
            catch (Exception ex)
            {
                Log4netHelper.WriteErrorLog(string.Format("添加Email Comments错误：{0}; Stack trace:{1}", ex.Message, ex.StackTrace));
                return Ok(false);
            }

            return Ok(true);
        }
    }
}
