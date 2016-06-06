using System.IO;
using System.Linq.Expressions;
using System.Text;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Common.Excel;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web;
using System.Web.Http;
using System.Data.Entity;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.Services.EmailServiceReference;
using Mcdonalds.AM.Services.Infrastructure;
using Mcdonalds.AM.DataAccess.Common;

namespace Mcdonalds.AM.Services.Controllers
{
    public class NotificationMessageController : ApiController
    {
        [HttpGet]
        [Route("api/NotificationMessage/GetMessageList/{userCode}/{projectId}/{flowCode}")]
        public IHttpActionResult GetMessageList(string userCode,string projectId,string flowCode)
        {
            var notifi = new Notification();
            var searchCondition = new NotificationSearchCondition();
            searchCondition.ProjectId = projectId;
            searchCondition.FlowCode = flowCode;
            searchCondition.PageSize = 1000;
            int totalSize;
            var result = new
            {
                //ReceiveMessageList = notifi.GetNotificationByReceiverCode(userCode, projectId),
                //SendMessageList = notifi.GetNotificationBySenderCode(userCode, projectId),
                NotificationList = notifi.GetNotificationByProjectId(projectId, flowCode),
                ApprovalRecords = QueryProjectComments(searchCondition, out totalSize)
            };
            return Ok(result);
        }

        [HttpGet]
        [Route("api/NotificationMessage/GetFlowCodeList")]
        public IHttpActionResult GetFlowCodeList(string parentCode)
        {
            var notifi = new Notification();
            var list = notifi.GetFlowList(parentCode);
            return Ok(list);
        }

        [HttpGet]
        [Route("api/NotificationMessage/GetCreatorList")]
        public IHttpActionResult GetCreatorList(string projectId,string flowCode)
        {
            var notifi = new Notification();
            notifi.ProjectId = projectId;
            notifi.FlowCode = flowCode;
            var list = notifi.GetCreatorList();
            return Ok(list);
        }

        [HttpGet]
        [Route("api/NotificationMessage/GetCreateFlowInfo")]
        public IHttpActionResult GetCreateFlowInfo(string projectId,string flowCode)
        {
            Object entity = null;
            if (flowCode.ToLower() == "rebuild")
            {
                entity = new RebuildInfo().GetRebuildInfo(projectId);
            }
            else if (flowCode.ToLower() == "reimage")
            {
                entity = ReimageInfo.GetReimageInfo(projectId);
            }
            else if (flowCode.ToLower() == "renewal")
            {
                entity = RenewalInfo.Get(projectId);
            }
            else if (flowCode.ToLower() == "tempclosure")
            {
                entity = TempClosureInfo.Get(projectId);
            }
            else if (flowCode.ToLower() == "closure")
            {
                entity = ClosureInfo.GetByProjectId(projectId);
            }
            else if (flowCode.ToLower() == "majorlease")
            {
                entity = new MajorLeaseInfo().GetMajorLeaseInfo(projectId);
            }
            return Ok(entity);
        }

        [HttpPost]
        [Route("api/NotificationMessage/SendMsg")]
        public IHttpActionResult SendMsg(NotificationMsg msg)
        {
            using (var scope = new TransactionScope())
            {
                Notification.Send(msg);
                if (msg.IsSendEmail)
                {
                    //邮件模板中的数据
                    Dictionary<string, string> bodyValues = new Dictionary<string, string>();
                    if (string.IsNullOrEmpty(msg.UsCode))
                    {
                        var projectInfo = ProjectInfo.FirstOrDefault(e => e.ProjectId == msg.ProjectId);
                        if (projectInfo != null)
                        {
                            msg.UsCode = projectInfo.USCode;
                        }
                    }
                    var stor = StoreBasicInfo.GetStorInfo(msg.UsCode);
                    bodyValues.Add("ApplicantName", ClientCookie.UserNameENUS);//--提交人
                    bodyValues.Add("StoreCode", stor.StoreCode);
                    bodyValues.Add("StoreName", stor.NameENUS);
                    bodyValues.Add("Comments", msg.Message);
                    var actor = ProjectUsers.GetProjectUser(msg.ProjectId, ProjectUserRoleCode.AssetActor);
                    bodyValues.Add("Actor", actor.RoleNameENUS);////--呈递人
                    var flow = FlowInfo.Get(msg.FlowCode);
                    if (flow != null)
                    {
                        bodyValues.Add("WorkflowName", flow.NameENUS); ////--流程名称
                    }
                    if (msg.ProjectId.ToLower().IndexOf("rebuild") >= 0)
                    {
                        bodyValues.Add("ProjectName", Constants.Rebuild); //项目名称
                    }
                    else if (msg.ProjectId.ToLower().IndexOf("reimage") >= 0)
                    {
                        bodyValues.Add("ProjectName", Constants.Reimage); //项目名称
                    }
                    else if (msg.ProjectId.ToLower().IndexOf("closure") >= 0)
                    {
                        bodyValues.Add("ProjectName", Constants.Closure); //项目名称
                    }
                    else if (msg.ProjectId.ToLower().IndexOf("renewal") >= 0)
                    {
                        bodyValues.Add("ProjectName", "Renewal"); //项目名称
                    }
                    else if (msg.ProjectId.ToLower().IndexOf("majorlease") >= 0)
                    {
                        bodyValues.Add("ProjectName", "MajorLease"); //项目名称
                    }
                    else
                    {
                        bodyValues.Add("ProjectName", Constants.TempClosure);//项目名称
                    }
                    string viewPage = "";
                    if (!string.IsNullOrEmpty(msg.FlowCode))
                    {
                        var tmp = msg.FlowCode.Split('_');
                        if (tmp[0] == "Rebuild" && tmp[1] == "Package")
                            tmp[1] = "RebuildPackage";
                        viewPage = string.Format("{0}/{1}/Main#/{2}/Process/View?projectId={3}",
                           ConfigurationManager.AppSettings["webHost"], tmp[0], tmp[1], msg.ProjectId);
                    }

                    bodyValues.Add("FormUrl", viewPage);

                    var Receivers = Employee.GetSimpleEmployeeByCodes(msg.ReceiverCodeList.ToArray());
                    //调用邮件服务发送邮件
                    EmailSendingResultType result;
                    using (EmailServiceClient client = new EmailServiceClient())
                    {
                        EmailMessage message = new EmailMessage();
                        StringBuilder sbTo = new StringBuilder();
                        Dictionary<string, string> attachments = new Dictionary<string, string>();
                        foreach (var emp in Receivers)
                        {
                            if (sbTo.Length > 0)
                            {
                                sbTo.Append(";");
                            }
                            if (!string.IsNullOrEmpty(emp.Mail))
                            {
                                sbTo.Append(emp.Mail);
                            }
                        }
                        if (sbTo.Length > 0)
                        {
                            sbTo.Append(";");
                        }
                        message.EmailBodyValues = bodyValues;
                        message.To = sbTo.ToString();
                        message.TemplateCode = EmailTemplateCode.Notification;
                        result = client.SendNotificationEmail(message);
                    }

                    if (!result.Successful)
                    {
                        return BadRequest(result.ErrorMessage);
                    }
                }
                scope.Complete();
            }
            return Ok();
        }

        [HttpPost]
        [Route("api/NotificationMessage/Query")]
        public IHttpActionResult Query(NotificationSearchCondition searchCondition)
        {
            int totalSize;
            var data = Notification.GetNotificationList(searchCondition, out totalSize);
            if (data != null && data.Count < 10)
            {
                if (!string.IsNullOrEmpty(searchCondition.FlowCode))
                {
                    var tmpCount = 0;
                    var count = searchCondition.PageSize - data.Count;
                    searchCondition.PageIndex = 0;
                    searchCondition.PageSize = count;
                }
            }
            
            return Ok(new {data, totalSize});
        }

        [HttpPost]
        [Route("api/NotificationMessage/QueryProjectComments")]
        public IHttpActionResult QueryProjectComments(NotificationSearchCondition searchCondition)
        {
            int totalSize;
            var data = QueryProjectComments(searchCondition, out totalSize);
            return Ok(new { data, totalSize });
        }

        private List<Notification> QueryProjectComments(NotificationSearchCondition searchCondition, out int totalSize)
        {
            var tmpList = GetProjectComments(searchCondition, out totalSize);
            var data = new List<Notification>();
            if (tmpList != null && tmpList.Count > 0)
            {
                for (var i = 0; i < tmpList.Count; i++)
                {
                    var l = tmpList[i];
                    data.Add(new Notification()
                    {
                        FlowCode = l.SourceCode,
                        Message = l.Content,
                        SenderZHCN = string.IsNullOrEmpty(l.UserNameZHCN)?l.CreateUserNameZHCN:l.UserNameZHCN,
                        SenderENUS = string.IsNullOrEmpty(l.UserNameENUS)?l.CreateUserNameENUS:l.UserNameENUS,
                        CreateTime = l.CreateTime.HasValue ? l.CreateTime.Value : DateTime.Now,
                        PositionENUS = l.TitleNameENUS,
                        HasRead = true
                    });
                }
            }
            return data;
        }

        private List<ProjectComment> GetProjectComments(NotificationSearchCondition searchCondition,out int totalSize)
        {
            var entity = BaseWFEntity.GetWorkflowEntity(searchCondition.ProjectId, searchCondition.FlowCode);
            var list = entity == null ? null : entity.GetEntityProjectComment();
            if (list != null && list.Count > 0)
            {
                var nextDay = searchCondition.CreateDate;
                if (searchCondition.CreateDate.HasValue)
                    nextDay = searchCondition.CreateDate.Value.AddDays(1);

                var tmpList =
                    list.Where(
                        e =>
                            (string.IsNullOrEmpty(searchCondition.Title) ||
                             e.Content.Contains(searchCondition.Title)) &&
                            (!searchCondition.CreateDate.HasValue ||
                             (e.CreateTime > searchCondition.CreateDate.Value && e.CreateTime < nextDay))
                            &&
                            (string.IsNullOrEmpty(searchCondition.SenderCode) ||
                             e.CreateUserAccount == searchCondition.SenderCode)).ToList();
                totalSize = tmpList.Count;
                tmpList =
                    tmpList.Skip(searchCondition.PageSize*(searchCondition.PageIndex - 1))
                        .Take(searchCondition.PageSize)
                        .ToList();
                return tmpList;
            }
            else
            {
                totalSize = 0;
                return null;
            }
            
        }

        [HttpPost]
        [Route("api/NotificationMessage/ExportExcel")]
        public IHttpActionResult ExportExcel(NotificationSearchCondition searchCondition)
        {
            var current = HttpContext.Current;
            int totalSize;
            searchCondition.PageSize = 1000000;
            var data = Notification.GetNotificationList(searchCondition, out totalSize);
            //if (!string.IsNullOrEmpty(searchCondition.FlowCode))
            //{
            //    var tmpList = GetProjectComments(searchCondition, out totalSize);
            //    if (tmpList != null && tmpList.Count > 0)
            //    {
            //        foreach (var l in tmpList)
            //        {
            //            data.Add(new NotificationDTO()
            //            {
            //                FlowCode = l.SourceCode,
            //                Message = l.Content,
            //                SenderZHCN = l.CreateUserNameZHCN,
            //                SenderENUS = l.CreateUserNameENUS,
            //                CreateTime = l.CreateTime.HasValue ? l.CreateTime.Value : DateTime.Now,
            //                PositionENUS = l.TitleNameENUS,
            //                HasRead = true
            //            });
            //        }
            //    }
            //}
            var siteFilePath = SiteFilePath.CommentsList_Template;
            var path = string.Format(@"{0}\{1}", SiteFilePath.Template_DIRECTORY, siteFilePath);
            var fileName = Guid.NewGuid() + ".xlsx";
            var tempFilePath = current.Server.MapPath("~/") + "Temp\\" + fileName ;

            File.Copy(path, tempFilePath);
            var excelOutputDirector = new ExcelDataInputDirector(new FileInfo(tempFilePath),
                ExcelDataInputType.CommentsList);

            var dataList = data.Select(e => new ExcelInputDTO()
            {
                Form = e.FlowCode,
                Comments = e.Message,
                CreateBy = e.SenderZHCN + "(" + e.SenderENUS + ")",
                CreateDate = e.CreateTime==null?"":e.CreateTime.Value.ToShortDateString(),
                SendTo = e.ReceiverZHCN + "(" + e.ReceiverENUS + ")",
                SenderPosition = e.PositionENUS,
                IsRead = e.HasRead ? "Yes" : "No"
            }).ToList();
            excelOutputDirector.ListInput(dataList);
            return Ok(new
            {
                fileName = fileName
            }
                );
        }
    }
}
