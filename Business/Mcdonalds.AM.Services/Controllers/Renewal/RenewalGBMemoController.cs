using System.Text;
using System.Transactions;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.DataTransferObjects.Renewal;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.Services.EmailServiceReference;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers.Renewal
{
    public class RenewalGBMemoController : ApiController
    {
        #region GBMemo
        [HttpGet]
        [Route("api/Renewal/GBMemo/GetGBMemoInfo")]
        public IHttpActionResult GetGBMemoInfo(string projectId, string entityId = "")
        {
            var memo = RenewalGBMemo.GetGBMemo(projectId, entityId);
            return Ok(memo);
        }
        [HttpPost]
        [Route("api/Renewal/GBMemo/SaveGBMemo")]
        public IHttpActionResult SaveGBMemo(RenewalGBMemo memo)
        {
            memo.Save();
            return Ok();
        }

        [HttpPost]
        [Route("api/Renewal/GBMemo/SubmitGBMemo")]
        public IHttpActionResult SubmitGBMemo(RenewalGBMemo memo)
        {
            memo.Submit();
            return Ok();
        }
        [HttpPost]
        [Route("api/Renewal/GBMemo/ResubmitGBMemo")]
        public IHttpActionResult ResubmitGBMemo(RenewalGBMemo entity)
        {
            entity.Resubmit(ClientCookie.UserCode);
            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Renewal/GBMemo/ApproveGBMemo")]
        public IHttpActionResult ApproveGBMemo(RenewalGBMemo entity)
        {
            entity.Approve(ClientCookie.UserCode);
            return Ok(entity);
        }

        [HttpPost]
        [Route("api/Renewal/GBMemo/RecallGBMemo")]
        public IHttpActionResult RecallGBMemo(RenewalGBMemo entity)
        {
            entity.Recall(entity.Comments);
            return Ok();
        }

        [HttpPost]
        [Route("api/Renewal/GBMemo/EditGBMemo")]
        public IHttpActionResult EditGBMemo(RenewalGBMemo entity)
        {
            var taskUrl = entity.Edit();
            return Ok(new ProjectEditResult
            {
                TaskUrl = taskUrl
            });
        }

        [HttpPost]
        [Route("api/Renewal/GBMemo/ReturnGBMemo")]
        public IHttpActionResult ReturnGBMemo(RenewalGBMemo entity)
        {
            entity.Return(ClientCookie.UserCode);
            return Ok(entity);
        }

        [Route("api/Renewal/GBMemo/NotifyGBMemo")]
        [HttpPost]
        public IHttpActionResult NotifyGBMemo(PostMemo<RenewalGBMemo> postData)
        {
            var actor = ProjectUsers.GetProjectUser(postData.Entity.ProjectId, ProjectUserRoleCode.AssetActor);
            using (TransactionScope tranScope = new TransactionScope())
            {
                Dictionary<string, string> pdfData = new Dictionary<string, string>();
                pdfData.Add("WorkflowName", Constants.Rebuild);
                pdfData.Add("ProjectID", postData.Entity.ProjectId);
                pdfData.Add("RegionNameENUS", postData.Entity.Store.StoreBasicInfo.RegionENUS);
                pdfData.Add("RegionNameZHCN", postData.Entity.Store.StoreBasicInfo.RegionZHCN);
                pdfData.Add("MarketNameENUS", postData.Entity.Store.StoreBasicInfo.MarketENUS);
                pdfData.Add("MarketNameZHCN", postData.Entity.Store.StoreBasicInfo.MarketZHCN);
                pdfData.Add("ProvinceNameENUS", postData.Entity.Store.StoreBasicInfo.ProvinceENUS);
                pdfData.Add("ProvinceNameZHCN", postData.Entity.Store.StoreBasicInfo.ProvinceZHCN);
                pdfData.Add("CityNameENUS", postData.Entity.Store.StoreBasicInfo.CityENUS);
                pdfData.Add("CityNameZHCN", postData.Entity.Store.StoreBasicInfo.CityZHCN);
                pdfData.Add("StoreNameENUS", postData.Entity.Store.StoreBasicInfo.NameENUS);
                pdfData.Add("StoreNameZHCN", postData.Entity.Store.StoreBasicInfo.NameZHCN);
                pdfData.Add("USCode", postData.Entity.Store.StoreBasicInfo.StoreCode);

                pdfData.Add("IsClosed", postData.Entity.IsClosed ? "Y" : "N");
                pdfData.Add("IsInOperation", postData.Entity.IsInOperation ? "Y" : "N");
                pdfData.Add("IsMcCafe", postData.Entity.IsMcCafe ? "Y" : "N");
                pdfData.Add("IsKiosk", postData.Entity.IsKiosk ? "Y" : "N");
                pdfData.Add("IsMDS", postData.Entity.IsMDS ? "Y" : "N");
                pdfData.Add("Is24Hour", postData.Entity.Is24Hour ? "Y" : "N");

                pdfData.Add("GBDate", postData.Entity.GBDate.HasValue ? postData.Entity.GBDate.Value.ToString("yyyy-MM-dd") : "");
                pdfData.Add("ConstCompletionDate", postData.Entity.ConstCompletionDate.HasValue ? postData.Entity.ConstCompletionDate.Value.ToString("yyyy-MM-dd") : "");
                pdfData.Add("ReopenDate", postData.Entity.ReopenDate.HasValue ? postData.Entity.ReopenDate.Value.ToString("yyyy-MM-dd") : "");

                string pdfPath = HtmlConversionUtility.HtmlConvertToPDF(HtmlTempalteType.GBMemo, pdfData, null);
                EmailSendingResultType result;
                //邮件模板中的数据
                Dictionary<string, string> bodyValues = new Dictionary<string, string>();
                //邮件内容中的键值对
                bodyValues.Add("ApplicantName", ClientCookie.UserNameENUS);//--提交人
                bodyValues.Add("StoreCode", postData.Entity.Store.StoreBasicInfo.StoreCode);
                bodyValues.Add("StoreName", postData.Entity.Store.StoreBasicInfo.NameENUS);
                bodyValues.Add("Actor", actor.RoleNameENUS);////--呈递人
                bodyValues.Add("WorkflowName", FlowCode.Renewal_GBMemo); ////--流程名称
                bodyValues.Add("ProjectName", "Renewal"); //项目名称

                string viewPage = string.Format("{0}/Renewal/Main#/GBMemo/Process/View?projectId={1}",
                        HttpContext.Current.Request.Url.Authority, postData.Entity.ProjectId);
                bodyValues.Add("FormUrl", viewPage);

                //调用邮件服务发送邮件
                using (EmailServiceClient client = new EmailServiceClient())
                {
                    EmailMessage message = new EmailMessage();
                    StringBuilder sbTo = new StringBuilder();
                    Dictionary<string, string> attachments = new Dictionary<string, string>();
                    foreach (Employee emp in postData.Receivers)
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
                    string strTitle = FlowCode.Renewal_GBMemo;
                    attachments.Add(pdfPath, strTitle + "_" + postData.Entity.ProjectId + ".pdf");
                    message.AttachmentsDict = attachments;
                    message.To = sbTo.ToString();
                    message.TemplateCode = EmailTemplateCode.GBMemoNotification;
                    result = client.SendGBMemoNotificationEmail(message);
                }

                if (!result.Successful)
                {
                    return BadRequest(result.ErrorMessage + " " + pdfPath);
                }
                postData.Entity.CompleteNotifyTask(postData.Entity.ProjectId);
                ProjectInfo.FinishNode(postData.Entity.ProjectId, FlowCode.Renewal_GBMemo, NodeCode.Finish);
                AttachmentsMemoProcessInfo.UpdateNotifyDate(postData.Entity.ProjectId, FlowCode.GBMemo);
                tranScope.Complete();
            }
            return Ok();
        }
        #endregion
    }
}
