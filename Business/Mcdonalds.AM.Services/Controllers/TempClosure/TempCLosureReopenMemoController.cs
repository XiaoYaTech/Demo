using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.Services.EmailServiceReference;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers.TempClosure
{
    public class TempClosureReopenMemoController : ApiController
    {
        [Route("api/tempClosureReopenMemo/get")]
        [HttpGet]
        public IHttpActionResult Get(string projectId)
        {
            var tempClosureInfo = TempClosureInfo.Get(projectId);
            return Ok(new
            {
                Entity = TempClosureReopenMemo.Get(projectId),
                TempClosureDate = tempClosureInfo.ActualTempClosureDate,
                IsActor = ProjectUsers.IsRole(projectId, ClientCookie.UserCode, ProjectUserRoleCode.AssetActor)
            });
        }

        [Route("api/tempClosureReopenMemo/save")]
        [HttpPost]
        public IHttpActionResult Save(TempClosureReopenMemo tempReopenMemo)
        {
            var project = ProjectInfo.Get(tempReopenMemo.ProjectId, FlowCode.TempClosure_ReopenMemo);
            project.CreateUserAccount = ClientCookie.UserCode;
            project.Update();
            tempReopenMemo.Save();
            ProjectInfo.FinishNode(tempReopenMemo.ProjectId, FlowCode.TempClosure_ReopenMemo, NodeCode.TempClosure_ReopenMemo_Input);
            return Ok();
        }

        [Route("api/tempClosureReopenMemo/send")]
        [HttpPost]
        public IHttpActionResult Send(PostMemo<TempClosureReopenMemo> postData)
        {
            var actor = ProjectUsers.GetProjectUser(postData.Entity.ProjectId, ProjectUserRoleCode.AssetActor);
            using (TransactionScope tranScope = new TransactionScope())
            {
                Save(postData.Entity);
                Dictionary<string, string> pdfData = new Dictionary<string, string>();
                pdfData.Add("WorkflowName", "TempClosure");
                pdfData.Add("ProjectID", postData.Entity.ProjectId);
                pdfData.Add("RegionENUS", postData.Entity.RegionENUS);
                pdfData.Add("RegionZHCN", postData.Entity.RegionZHCN);
                pdfData.Add("MarketENUS", postData.Entity.MarketENUS);
                pdfData.Add("MarketZHCN", postData.Entity.MarketZHCN);
                pdfData.Add("ProvinceENUS", postData.Entity.ProvinceENUS);
                pdfData.Add("ProvinceZHCN", postData.Entity.ProvinceZHCN);
                pdfData.Add("CityENUS", postData.Entity.CityENUS);
                pdfData.Add("CityZHCN", postData.Entity.CityZHCN);
                pdfData.Add("PipelineId", postData.Entity.PipelineId);
                pdfData.Add("USCode", postData.Entity.USCode);
                pdfData.Add("StoreENUS", postData.Entity.StoreENUS);
                pdfData.Add("StoreZHCN", postData.Entity.StoreZHCN);
                pdfData.Add("ActualConsFinishDate", postData.Entity.ActualConsFinishDate.HasValue ? postData.Entity.ActualConsFinishDate.Value.ToString("yyyy-MM-dd") : "");
                pdfData.Add("OpeningDate", postData.Entity.OpeningDate.HasValue ? postData.Entity.OpeningDate.Value.ToString("yyyy-MM-dd") : "");
                pdfData.Add("ProtfolioTypeName", postData.Entity.ProtfolioTypeName);
                pdfData.Add("TAClassificationName", postData.Entity.TAClassificationName);
                pdfData.Add("MarketDesirability", postData.Entity.MarketDesirability);
                pdfData.Add("RERating", postData.Entity.RERating);
                pdfData.Add("BusinessArea", postData.Entity.BusinessArea);
                pdfData.Add("KitchenFloor", postData.Entity.KitchenFloor);
                pdfData.Add("SeatingFloor", postData.Entity.SeatingFloor);
                pdfData.Add("SeatingNum", postData.Entity.SeatingNum);
                pdfData.Add("ParkingNum", postData.Entity.ParkingNum);
                pdfData.Add("ContractType", postData.Entity.ContractType);
                pdfData.Add("Kiosk", postData.Entity.Kiosk);
                pdfData.Add("RERepName", postData.Entity.RERepName);
                pdfData.Add("PlannerName", postData.Entity.PlannerName);
                string pdfPath = HtmlConversionUtility.HtmlConvertToPDF(HtmlTempalteType.TempClosureReopenMemo, pdfData, null);
                EmailSendingResultType result;
                //邮件模板中的数据
                Dictionary<string, string> bodyValues = new Dictionary<string, string>();
                //邮件内容中的键值对
                bodyValues.Add("ApplicantName", ClientCookie.UserNameENUS);//--提交人
                bodyValues.Add("StoreCode", postData.Entity.USCode);
                bodyValues.Add("StoreName", postData.Entity.StoreENUS);
                bodyValues.Add("Actor", actor.RoleNameENUS);////--呈递人
                bodyValues.Add("WorkflowName", Constants.TempClosure_ReopenMemo);////--流程名称
                bodyValues.Add("ProjectName", Constants.TempClosure);//项目名称
                var viewPage = string.Format("{0}/TempClosure/Main#/ReopenMemo/Process/View?projectId={1}",
                    HttpContext.Current.Request.Url.Authority, postData.Entity.ProjectId);
                bodyValues.Add("FormUrl", viewPage);

                //调用邮件服务发送邮件
                using (EmailServiceClient client = new EmailServiceClient())
                {
                    EmailMessage message = new EmailMessage();
                    StringBuilder sbTo = new StringBuilder();
                    StringBuilder sbCC = new StringBuilder();
                    Dictionary<string, string> attachments = new Dictionary<string, string>();
                    //主送人
                    var actorEmployee = Employee.GetEmployeeByCode(actor.UserAccount);
                    if (actorEmployee != null)
                        sbTo.Append(actorEmployee.Mail + ";");
                    var cooList = Employee.GetStoreEmployeesByRole(postData.Entity.USCode, RoleCode.Coordinator);
                    foreach (var coo in cooList)
                    {
                        sbTo.Append(coo.Mail + ";");
                    }
                    var mamList = Employee.GetStoreEmployeesByRole(postData.Entity.USCode, RoleCode.Market_Asset_Mgr);
                    foreach (var mam in mamList)
                    {
                        sbTo.Append(mam.Mail + ";");
                    }
                    var ramList = Employee.GetStoreEmployeesByRole(postData.Entity.USCode, RoleCode.Regional_Asset_Mgr);
                    foreach (var ram in ramList)
                    {
                        sbTo.Append(ram.Mail + ";");
                    }
                    var mcamList = Employee.GetStoreEmployeesByRole(postData.Entity.USCode, RoleCode.MCCL_Asset_Mgr);
                    foreach (var mcam in mcamList)
                    {
                        sbTo.Append(mcam.Mail + ";");
                    }

                    //抄送人
                    if (postData.Receivers != null)
                    {
                        foreach (Employee emp in postData.Receivers)
                        {
                            if (sbCC.Length > 0)
                            {
                                sbCC.Append(";");
                            }
                            if (!string.IsNullOrEmpty(emp.Mail))
                            {
                                sbCC.Append(emp.Mail);
                            }
                        }
                    }
                    if (sbCC.Length > 0)
                    {
                        sbCC.Append(";");
                    }

                    message.EmailBodyValues = bodyValues;
                    string strTitle = FlowCode.TempClosure_ReopenMemo;
                    attachments.Add(pdfPath, strTitle + "_" + postData.Entity.ProjectId + ".pdf");
                    message.AttachmentsDict = attachments;
                    message.To = sbTo.ToString();
                    message.CC = sbCC.ToString();
                    message.TemplateCode = EmailTemplateCode.GBMemoNotification;
                    result = client.SendNotificationEmail(message);
                }

                if (!result.Successful)
                {
                    return BadRequest(result.ErrorMessage + " " + pdfPath);
                }
                var projectInfo = ProjectInfo.Get(postData.Entity.ProjectId, FlowCode.TempClosure_ReopenMemo);
                if (projectInfo.Status == ProjectStatus.UnFinish)
                    postData.Entity.Submit();
                tranScope.Complete();
            }
            return Ok();
        }
    }
}
