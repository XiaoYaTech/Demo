using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.Services.EmailServiceReference;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers.TempClosure
{
    public class TempClosureMemoController : ApiController
    {
        [Route("api/tempClosureMemo/get")]
        [HttpGet]
        public IHttpActionResult Get(string projectId)
        {
            TempClosureMemo entity = null;
            if (projectId.ToLower().IndexOf("rebuild") != -1
                || projectId.ToLower().IndexOf("reimage") != -1)
                entity = TempClosureMemo.GetTempClosureMemo(projectId);
            else if (projectId.ToLower().IndexOf("majorlease") != -1)
            {
                entity = TempClosureMemo.GetAttachClosureMemo(projectId, FlowCode.MajorLease);
            }
            else if (projectId.ToLower().IndexOf("renewal") != -1)
            {
                entity = TempClosureMemo.GetAttachClosureMemo(projectId, FlowCode.Renewal);
            }
            //else if (projectId.ToLower().IndexOf("reimage") != -1)
            //{
            //    entity = TempClosureMemo.GetAttachClosureMemo(projectId, FlowCode.Reimage);
            //}
            else
                entity = TempClosureMemo.Get(projectId);
            var isActor = ProjectUsers.IsRole(projectId, ClientCookie.UserCode, ProjectUserRoleCode.AssetActor);
            return Ok(new
            {
                Entity = entity,
                IsActor = isActor
            });
        }

        [Route("api/tempClosureMemo/save")]
        [HttpPost]
        public IHttpActionResult Save(TempClosureMemo closureMemo)
        {
            //var project = ProjectInfo.Get(closureMemo.ProjectId, FlowCode.TempClosure_ClosureMemo);
            //project.CreateUserAccount = ClientCookie.UserCode;
            //project.Update();
            closureMemo.Save();
            var tempClosure = TempClosureInfo.Get(closureMemo.ProjectId);
            if (tempClosure != null && closureMemo.ClosureDate != null)
            {
                tempClosure.ActualTempClosureDate = closureMemo.ClosureDate.Value;
                tempClosure.Update();
            }
            if (closureMemo.ProjectId.ToLower().Contains("tpcls"))
            {
                ProjectInfo.FinishNode(closureMemo.ProjectId, FlowCode.TempClosure_ClosureMemo, NodeCode.TempClosure_ClosureMemo_Input);
            }
            else if (closureMemo.ProjectId.ToLower().Contains("reimage"))
            {
                ProjectInfo.FinishNode(closureMemo.ProjectId, FlowCode.Reimage_TempClosureMemo, NodeCode.Reimage_TempClosureMemo_Input);
            }
            else if (closureMemo.ProjectId.ToLower().Contains("rebuild"))
            {
                ProjectInfo.FinishNode(closureMemo.ProjectId, FlowCode.Rebuild_TempClosureMemo, NodeCode.Rebuild_TempClosureMemo_Input);
            }

            return Ok();
        }

        [Route("api/tempClosureMemo/send")]
        [HttpPost]
        public IHttpActionResult Send(PostMemo<TempClosureMemo> postData)
        {
            var actor = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == postData.Entity.ProjectId && pu.RoleCode == ProjectUserRoleCode.AssetActor);
            Dictionary<string, string> pdfData = new Dictionary<string, string>();
            if (postData.Entity.ProjectId.ToLower().IndexOf("rebuild") != -1)
            {
                pdfData.Add("WorkflowName", "Rebuild");
                pdfData.Add("ClosureNature", "Temporary");
            }
            else if (postData.Entity.ProjectId.ToLower().IndexOf("majorlease") != -1)
            {
                pdfData.Add("WorkflowName", "MajorLease");
                pdfData.Add("ClosureNature", "Temporary");
            }
            else if (postData.Entity.ProjectId.ToLower().IndexOf("reimage") != -1)
            {
                pdfData.Add("WorkflowName", "Reimage");
                pdfData.Add("ClosureNature", "Temporary");
            }
            else if (postData.Entity.ProjectId.ToLower().IndexOf("renewal") != -1)
            {
                pdfData.Add("WorkflowName", "Renewal");
                pdfData.Add("ClosureNature", "Temporary");
            }
            else
            {
                pdfData.Add("WorkflowName", "TempClosure");
                pdfData.Add("ClosureNature", postData.Entity.ClosureNature.ToString());
            }
            pdfData.Add("ProjectID", postData.Entity.ProjectId);
            pdfData.Add("RegionNameENUS", postData.Entity.RegionNameENUS);
            pdfData.Add("RegionNameZHCN", postData.Entity.RegionNameZHCN);
            pdfData.Add("MarketNameENUS", postData.Entity.MarketNameENUS);
            pdfData.Add("MarketNameZHCN", postData.Entity.MarketNameZHCN);
            pdfData.Add("ProvinceNameENUS", postData.Entity.ProvinceNameENUS);
            pdfData.Add("ProvinceNameZHCN", postData.Entity.ProvinceNameZHCN);
            pdfData.Add("CityNameENUS", postData.Entity.CityNameENUS);
            pdfData.Add("CityNameZHCN", postData.Entity.CityNameZHCN);
            pdfData.Add("StoreNameENUS", postData.Entity.StoreNameENUS);
            pdfData.Add("StoreNameZHCN", postData.Entity.StoreNameZHCN);
            pdfData.Add("StoreAddressENUS", postData.Entity.StoreAddressENUS);
            pdfData.Add("StoreAddressZHCN", postData.Entity.StoreAddressZHCN);
            pdfData.Add("USCode", postData.Entity.USCode);
            pdfData.Add("OpenDate", postData.Entity.OpenDate.HasValue ? postData.Entity.OpenDate.Value.ToString("yyyy-MM-dd") : "");
            pdfData.Add("ClosureDate", postData.Entity.ClosureDate.HasValue ? postData.Entity.ClosureDate.Value.ToString("yyyy-MM-dd") : "");

            pdfData.Add("BecauseOfReimaging", postData.Entity.BecauseOfReimaging ? "Yes" : "No");
            pdfData.Add("BecauseOfRemodel", postData.Entity.BecauseOfRemodel ? "Yes" : "No");
            pdfData.Add("BecauseOfDespute", postData.Entity.BecauseOfDespute ? "Yes" : "No");
            pdfData.Add("BecauseOfRedevelopment", postData.Entity.BecauseOfRedevelopment ? "Yes" : "No");
            pdfData.Add("BecauseOfPlanedClosure", postData.Entity.BecauseOfPlanedClosure ? "Yes" : "No");
            pdfData.Add("BecauseOfRebuild", postData.Entity.BecauseOfRebuild ? "Yes" : "No");
            pdfData.Add("BecauseOfOthers", postData.Entity.BecauseOfOthers);
            pdfData.Add("PermanentCloseOpportunity", postData.Entity.PermanentCloseOpportunity ? "Yes" : "No");
            pdfData.Add("HasRelocationPlan", postData.Entity.HasRelocationPlan ? "Yes" : "No");
            pdfData.Add("PipelineName", postData.Entity.PipelineName);
            pdfData.Add("CompensationAwarded", postData.Entity.CompensationAwarded ? "Yes" : "No");
            pdfData.Add("Compensation", postData.Entity.Compensation.HasValue ? postData.Entity.Compensation.Value.ToString("N") : "");
            string pdfPath = HtmlConversionUtility.HtmlConvertToPDF(HtmlTempalteType.ClosureMemo, pdfData, null);
            EmailSendingResultType result;
            //邮件模板中的数据
            Dictionary<string, string> bodyValues = new Dictionary<string, string>();
            //邮件内容中的键值对
            bodyValues.Add("ApplicantName", ClientCookie.UserNameENUS);//--提交人
            bodyValues.Add("StoreCode", postData.Entity.USCode);
            bodyValues.Add("StoreName", postData.Entity.StoreNameENUS);
            bodyValues.Add("Actor", actor.UserNameENUS);////--呈递人


            string viewPage = string.Format("{0}/TempClosure/Main#/ClosureMemo/Process/View?projectId={1}",
                ConfigurationManager.AppSettings["webHost"], postData.Entity.ProjectId);
            if (postData.Entity.ProjectId.ToLower().IndexOf("rebuild") != -1)
            {
                bodyValues.Add("WorkflowName", Constants.TempClosure_Memo);////--流程名称
                bodyValues.Add("ProjectName", Constants.Rebuild);//项目名称
                viewPage = string.Format("{0}/Rebuild/Main#/TempClosureMemo/View?projectId={1}",
                ConfigurationManager.AppSettings["webHost"], postData.Entity.ProjectId);

            }
            else if (postData.Entity.ProjectId.ToLower().IndexOf("majorlease") != -1)
            {
                bodyValues.Add("WorkflowName", Constants.Closure_Memo);////--流程名称
                bodyValues.Add("ProjectName", Constants.MajorLease);//项目名称
                viewPage = string.Format("{0}/MajorLease/Main#/ClosureMemo/View?projectId={1}",
                ConfigurationManager.AppSettings["webHost"], postData.Entity.ProjectId);
            }
            else if (postData.Entity.ProjectId.ToLower().IndexOf("reimage") != -1)
            {
                bodyValues.Add("WorkflowName", Constants.TempClosure_Memo);////--流程名称
                bodyValues.Add("ProjectName", Constants.Reimage);//项目名称
                viewPage = string.Format("{0}/Reimage/Main#/TempClosureMemo/View?projectId={1}",
                ConfigurationManager.AppSettings["webHost"], postData.Entity.ProjectId);
            }
            else if (postData.Entity.ProjectId.ToLower().IndexOf("renewal") != -1)
            {
                bodyValues.Add("WorkflowName", Constants.Closure_Memo);////--流程名称
                bodyValues.Add("ProjectName", Constants.Renewal); //项目名称
                viewPage = string.Format("{0}/Renewal/Main#/ClosureMemo/View?projectId={1}",
                    ConfigurationManager.AppSettings["webHost"], postData.Entity.ProjectId);
            }
            else if (postData.Entity.ProjectId.ToLower().IndexOf("closure") != -1)
            {
                bodyValues.Add("WorkflowName", Constants.Closure_Memo);////--流程名称
                bodyValues.Add("ProjectName", Constants.Closure); //项目名称
            }
            else
            {
                bodyValues.Add("WorkflowName", Constants.Closure_Memo);////--流程名称
                bodyValues.Add("ProjectName", Constants.TempClosure);//项目名称
            }
            bodyValues.Add("FormUrl", viewPage);

            if (postData.Entity.ProjectId.ToLower().Contains("tpcls"))
            {
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
                    attachments.Add(pdfPath, postData.Entity.USCode + " " + Constants.TempClosure_Memo + ".pdf");
                    message.AttachmentsDict = attachments;
                    message.To = sbTo.ToString();
                    message.CC = sbCC.ToString();
                    message.TemplateCode = EmailTemplateCode.GBMemoNotification;
                    result = client.SendNotificationEmail(message);
                }
            }
            else
            {
                using (EmailServiceClient client = new EmailServiceClient())
                {
                    EmailMessage message = new EmailMessage();
                    StringBuilder sbTo = new StringBuilder();
                    StringBuilder sbCC = new StringBuilder();
                    Dictionary<string, string> attachments = new Dictionary<string, string>();

                    if (postData.Receivers != null)
                    {
                        foreach (Employee emp in postData.Receivers)
                        {
                            if (sbCC.Length > 0)
                            {
                                sbTo.Append(";");
                            }
                            if (!string.IsNullOrEmpty(emp.Mail))
                            {
                                sbTo.Append(emp.Mail);
                            }
                        }
                    }
                    if (sbCC.Length > 0)
                    {
                        sbCC.Append(";");
                    }
                    message.EmailBodyValues = bodyValues;
                    string strTitle = FlowCode.TempClosure_ClosureMemo;
                    if (postData.Entity.ProjectId.ToLower().IndexOf("rebuild") >= 0)
                    {
                        strTitle = FlowCode.Rebuild_TempClosureMemo;
                    }
                    else if (postData.Entity.ProjectId.ToLower().IndexOf("reimage") >= 0)
                    {
                        strTitle = FlowCode.Reimage_TempClosureMemo;
                    }
                    else if (postData.Entity.ProjectId.ToLower().IndexOf("majorlease") >= 0)
                    {
                        strTitle = "MajorLease_ClosureMemo";
                    }

                    attachments.Add(pdfPath, postData.Entity.USCode + " " + strTitle + ".pdf");
                    message.AttachmentsDict = attachments;
                    message.To = sbTo.ToString();
                    message.TemplateCode = EmailTemplateCode.GBMemoNotification;
                    result = client.SendNotificationEmail(message);
                }
            }

            if (!result.Successful)
            {
                return BadRequest(result.ErrorMessage + " " + pdfPath);
            }
            using (TransactionScope tranScope = new TransactionScope())
            {
                postData.Entity.Save();
                ProjectInfo projectInfo = null;
                if (postData.Entity.ProjectId.ToLower().IndexOf("rebuild") >= 0)
                {
                    projectInfo = ProjectInfo.Get(postData.Entity.ProjectId, FlowCode.Rebuild_TempClosureMemo);
                }
                else if (postData.Entity.ProjectId.ToLower().IndexOf("reimage") >= 0)
                {
                    projectInfo = ProjectInfo.Get(postData.Entity.ProjectId, FlowCode.Reimage_TempClosureMemo);
                }
                else if (postData.Entity.ProjectId.ToLower().IndexOf("majorlease") >= 0
                         || postData.Entity.ProjectId.ToLower().IndexOf("renewal") >= 0)
                {
                    postData.Entity.Submit();
                }
                else
                {
                    projectInfo = ProjectInfo.Get(postData.Entity.ProjectId, FlowCode.TempClosure_ClosureMemo);
                    var tempClosure = TempClosureInfo.Get(postData.Entity.ProjectId);
                    if (postData.Entity.ClosureDate != null)
                        tempClosure.ActualTempClosureDate = postData.Entity.ClosureDate.Value;
                    tempClosure.Update();
                }
                if (projectInfo != null && projectInfo.Status == ProjectStatus.UnFinish)
                {
                    postData.Entity.Submit();

                    if (postData.Entity.ProjectId.ToLower().Contains("tpcls"))
                    {
                        if (postData.Entity.ClosureDate.HasValue)
                        {
                            ScheduleLog.UpdateStoreStatusSchedule(postData.Entity.USCode, postData.Entity.ProjectId, postData.Entity.ClosureDate.Value, ClientCookie.UserCode);
                        }
                    }
                }
                tranScope.Complete();
            }
            return Ok();
        }

        [Route("api/tempClosureMemo/querySaveable")]
        [HttpGet]
        public IHttpActionResult QuerySaveable(string projectId)
        {
            string flowCode = "";
            if (projectId.ToLower().IndexOf("rebuild") != -1)
                flowCode = FlowCode.Rebuild_TempClosureMemo;
            return Ok(new
            {
                IsShowSave = ProjectInfo.IsFlowSavable(projectId, flowCode)
            });
        }
    }
}
