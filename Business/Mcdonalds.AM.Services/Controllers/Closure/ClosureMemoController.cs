using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.Services.EmailServiceReference;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Transactions;
using System.Web;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers.Closure
{
    public class ClosureMemoController : ApiController
    {
        [Route("api/closure/closurememo")]
        [HttpGet]
        public IHttpActionResult Get(string projectId)
        {
            var entity = ClosureMemo.Get(projectId);
            bool editable = false;
            var storeStatus = StoreBasicInfo.Search(s => s.StoreCode == entity.USCode).Select(s => s.statusName).FirstOrDefault();
            if (storeStatus == "Closed")
                editable = false;
            else
            {
                if (TaskWork.Count(e => e.ReceiverAccount == ClientCookie.UserCode && e.SourceCode == FlowCode.Closure
                 && e.TypeCode == FlowCode.Closure_Memo && e.RefID == projectId) > 0)
                {
                    if (entity.Id == Guid.Empty)
                        editable = true;
                    else
                    {
                        if (entity.ClosureDate.Value < DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd") + " 00:00") && entity.ClosureNature == ClosureNatureType.Permanent)
                            editable = false;
                        else
                            editable = true;
                    }
                }
                else
                {
                    editable = true;
                }
            }
            var isActor = ProjectUsers.IsRole(projectId, ClientCookie.UserCode, ProjectUserRoleCode.AssetActor);
            return Ok(new
            {
                Entity = entity,
                Editable = editable,
                IsActor = isActor
            });
        }

        [Route("api/closure/closurememo/searchpipelines")]
        [HttpGet]
        public List<Pipeline> SearchPipelines(string queryString, int pageSize, string storeCode)
        {
            List<Pipeline> list = new List<Pipeline>();
            McdAMEntities db = new McdAMEntities();
            list = db.Database.SqlQuery<Pipeline>(string.Format(@"
select PipelineID,PipelineCode,PipelineNameZHCN,PipelineNameENUS
from StoreBasicInfo where MarketCode in 
(select MarketCode from StoreBasicInfo where StoreCode='{1}')
and (PipelineCode like '%{0}%' or PipelineNameZHCN like '%{0}%' or PipelineNameENUS like '%{0}%')
", queryString, storeCode)).Take(5).ToList();
            return list;
        }

        [Route("api/closure/closureTool/searchpipelineId")]
        [HttpGet]
        public List<Pipeline> SearchPipelineId(string queryString, int pageSize, string storeCode)
        {
            List<Pipeline> list = new List<Pipeline>();
            McdAMEntities db = new McdAMEntities();
            list = db.Database.SqlQuery<Pipeline>(string.Format(@"
select PipelineID,PipelineCode,PipelineNameZHCN,PipelineNameENUS
from StoreBasicInfo where MarketCode in 
(select MarketCode from StoreBasicInfo where StoreCode='{1}')
and PipelineCode like '%{0}%'
", queryString, storeCode)).Take(5).ToList();
            return list;
        }

        [Route("api/closure/closureTool/searchpipelineName")]
        [HttpGet]
        public List<Pipeline> SearchPipelineName(string queryString, int pageSize, string storeCode)
        {
            List<Pipeline> list = new List<Pipeline>();
            McdAMEntities db = new McdAMEntities();
            list = db.Database.SqlQuery<Pipeline>(string.Format(@"
select PipelineID,PipelineCode,PipelineNameZHCN,PipelineNameENUS
from StoreBasicInfo where MarketCode in 
(select MarketCode from StoreBasicInfo where StoreCode='{1}')
and (PipelineNameZHCN like '%{0}%' or PipelineNameENUS like '%{0}%')
", queryString, storeCode)).Take(5).ToList();
            return list;
        }

        public class Pipeline
        {
            public int PipelineID { get; set; }
            public string PipelineCode { get; set; }
            public string PipelineNameZHCN { get; set; }
            public string PipelineNameENUS { get; set; }
        }

        [Route("api/closure/closurememo/save")]
        [HttpPost]
        public IHttpActionResult Save(ClosureMemo entity)
        {
            using (TransactionScope tranScope = new TransactionScope())
            {
                if (entity.Id != Guid.Empty)
                {
                    ClosureMemo.Update(entity);
                }
                else
                {
                    entity.Id = Guid.NewGuid();
                    entity.CreateTime = DateTime.Now;
                    entity.Creator = ClientCookie.UserCode;
                    ClosureMemo.Add(entity);
                }
                ClosureInfo closure = ClosureInfo.GetByProjectId(entity.ProjectId);
                closure.ActualCloseDate = entity.ClosureDate;
                closure.Update();
                ProjectInfo.FinishNode(entity.ProjectId, FlowCode.Closure_Memo, NodeCode.Closure_ClosureMemo_Input);
                tranScope.Complete();
            }
            return Ok();
        }

        [Route("api/closure/closurememo/send")]
        [HttpPost]
        public IHttpActionResult Send(PostClosureMemoModel model)
        {
            var actor = ProjectUsers.GetProjectUser(model.Entity.ProjectId, ProjectUserRoleCode.AssetActor);
            using (TransactionScope tranScope = new TransactionScope())
            {
                Save(model.Entity);
                ClosureTool closureTool = ClosureTool.FirstOrDefault(ct => ct.ProjectId == model.Entity.ProjectId);
                string compensationAwards = "";
                string compensation = "暂无数据";
                if (closureTool != null && closureTool.Compensation.HasValue)
                {
                    compensationAwards = closureTool.Compensation.Value > 0 ? "Yes" : "No";
                    compensation = closureTool.Compensation.Value.ToString("N");
                }
                Dictionary<string, string> pdfData = new Dictionary<string, string>();
                pdfData.Add("WorkflowName", "Closure");
                pdfData.Add("ProjectID", model.Entity.ProjectId);
                pdfData.Add("RegionNameENUS", model.Entity.RegionNameENUS);
                pdfData.Add("RegionNameZHCN", model.Entity.RegionNameZHCN);
                pdfData.Add("MarketNameENUS", model.Entity.MarketNameENUS);
                pdfData.Add("MarketNameZHCN", model.Entity.MarketNameZHCN);
                pdfData.Add("ProvinceNameENUS", model.Entity.ProvinceNameENUS);
                pdfData.Add("ProvinceNameZHCN", model.Entity.ProvinceNameZHCN);
                pdfData.Add("CityNameENUS", model.Entity.CityNameENUS);
                pdfData.Add("CityNameZHCN", model.Entity.CityNameZHCN);
                pdfData.Add("StoreNameENUS", model.Entity.StoreNameENUS);
                pdfData.Add("StoreNameZHCN", model.Entity.StoreNameZHCN);
                pdfData.Add("StoreAddressENUS", model.Entity.StoreAddressENUS);
                pdfData.Add("StoreAddressZHCN", model.Entity.StoreAddressZHCN);
                pdfData.Add("USCode", model.Entity.USCode);
                pdfData.Add("OpenDate", model.Entity.OpenDate.Value.ToString("yyyy-MM-dd"));
                pdfData.Add("ClosureDate", model.Entity.ClosureDate.Value.ToString("yyyy-MM-dd"));
                pdfData.Add("ClosureNature", model.Entity.ClosureNature.ToString());
                if (model.Entity.BecauseOfReimaging.HasValue)
                    pdfData.Add("BecauseOfReimaging", model.Entity.BecauseOfReimaging.Value ? "Yes" : "No");
                else
                    pdfData.Add("BecauseOfReimaging", "");

                if (model.Entity.BecauseOfRemodel.HasValue)
                    pdfData.Add("BecauseOfRemodel", model.Entity.BecauseOfRemodel.Value ? "Yes" : "No");
                else
                    pdfData.Add("BecauseOfRemodel", "");

                if (model.Entity.BecauseOfDespute.HasValue)
                    pdfData.Add("BecauseOfDespute", model.Entity.BecauseOfDespute.Value ? "Yes" : "No");
                else
                    pdfData.Add("BecauseOfDespute", "");

                if (model.Entity.BecauseOfRedevelopment.HasValue)
                    pdfData.Add("BecauseOfRedevelopment", model.Entity.BecauseOfRedevelopment.Value ? "Yes" : "No");
                else
                    pdfData.Add("BecauseOfRedevelopment", "");

                if (model.Entity.BecauseOfPlanedClosure.HasValue)
                    pdfData.Add("BecauseOfPlanedClosure", model.Entity.BecauseOfPlanedClosure.Value ? "Yes" : "No");
                else
                    pdfData.Add("BecauseOfPlanedClosure", "");

                if (model.Entity.BecauseOfRebuild.HasValue)
                    pdfData.Add("BecauseOfRebuild", model.Entity.BecauseOfRebuild.Value ? "Yes" : "No");
                else
                    pdfData.Add("BecauseOfRebuild", "");

                pdfData.Add("BecauseOfOthers", model.Entity.BecauseOfOthers);

                if (model.Entity.PermanentCloseOpportunity.HasValue)
                    pdfData.Add("PermanentCloseOpportunity", model.Entity.PermanentCloseOpportunity.Value ? "Yes" : "No");
                else
                    pdfData.Add("PermanentCloseOpportunity", "");

                if (model.Entity.HasRelocationPlan.HasValue)
                    pdfData.Add("HasRelocationPlan", model.Entity.HasRelocationPlan.Value ? "Yes" : "No");
                else
                    pdfData.Add("HasRelocationPlan", "");

                pdfData.Add("PipelineName", model.Entity.PipelineName);
                pdfData.Add("CompensationAwarded", compensationAwards);
                pdfData.Add("Compensation", compensation);
                string pdfPath = HtmlConversionUtility.HtmlConvertToPDF(HtmlTempalteType.ClosureMemo, pdfData, null);
                EmailSendingResultType result;
                //邮件模板中的数据
                Dictionary<string, string> bodyValues = new Dictionary<string, string>();
                //邮件内容中的键值对
                //bodyValues.Add("ApplicantName", ClientCookie.UserNameENUS);//--提交人
                bodyValues.Add("StoreCode", model.Entity.USCode);
                bodyValues.Add("StoreName", model.Entity.StoreNameENUS);
                bodyValues.Add("Actor", actor.RoleNameENUS);////--呈递人
                bodyValues.Add("WorkflowName", Constants.Closure_Memo);////--流程名称
                bodyValues.Add("ProjectName", Constants.Closure);//项目名称
                var webRootUrl = ConfigurationManager.AppSettings["webHost"];
                var viewPage = string.Format("{0}Closure/Main#/ClosureMemo/ClosureMemoView?projectId={1}",
                    webRootUrl, model.Entity.ProjectId);
                bodyValues.Add("FormUrl", viewPage);

                //调用邮件服务发送邮件
                using (EmailServiceClient client = new EmailServiceClient())
                {
                    EmailMessage message = new EmailMessage();
                    StringBuilder sbTo = new StringBuilder();
                    Dictionary<string, string> attachments = new Dictionary<string, string>();
                    foreach (Employee emp in model.Receivers)
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
                    attachments.Add(pdfPath, model.Entity.USCode + " " + FlowCode.Closure_Memo + ".pdf");
                    message.AttachmentsDict = attachments;
                    message.To = sbTo.ToString();
                    message.TemplateCode = EmailTemplateCode.GBMemoNotification;
                    result = client.SendNotificationEmail(message);
                }

                if (!result.Successful)
                {
                    return BadRequest(result.ErrorMessage + " " + pdfPath);
                }

                //store关闭不在这里设置，需要判断project状态和closuredata
                //var store = StoreBasicInfo.GetStorInfo(model.Entity.USCode);
                //store.StoreStatus = "suoya301003";
                //store.statusName = "Closed";
                //store.Update();


                if (model.Entity.ClosureNature == ClosureNatureType.Permanent)
                {
                    //选项为永久关闭并且发送成功后关闭任务
                    McdAMEntities _db = new McdAMEntities();
                    var task = _db.TaskWork.FirstOrDefault(e => e.ReceiverAccount == ClientCookie.UserCode && e.Status == 0 && e.SourceCode == FlowCode.Closure && e.TypeCode == FlowCode.Closure_Memo && e.RefID == model.Entity.ProjectId);
                    if (task != null)
                    {
                        task.Status = TaskWorkStatus.K2ProcessApproved;
                        task.FinishTime = DateTime.Now;
                        task.Url = SiteInfo.GetProjectViewPageUrl(FlowCode.Closure_Memo, task.RefID);

                        //var enableExecutiveSummary = handler.EnableExecutiveSummary(entity.ProjectId.Value);

                        _db.TaskWork.Attach(task);
                        _db.Entry(task).State = EntityState.Modified;
                        _db.SaveChanges();

                        ProjectInfo.FinishNode(model.Entity.ProjectId, FlowCode.Closure_Memo, NodeCode.Closure_ClosureMemo_Input);
                        ProjectInfo.FinishNode(model.Entity.ProjectId, FlowCode.Closure_Memo, NodeCode.Closure_ClosureMemo_SendMemo, ProjectStatus.Finished);

                        #region Memo完成后，设定计划任务
                        var closureConsInvtChecking = new ClosureConsInvtChecking();
                        closureConsInvtChecking.GenerateConsInvtCheckingTask(model.Entity.ProjectId);

                        if (model.Entity.ClosureDate.HasValue)
                            ScheduleLog.UpdateStoreStatusSchedule(model.Entity.USCode, model.Entity.ProjectId, model.Entity.ClosureDate.Value, ClientCookie.UserCode);
                        #endregion
                    }
                    ProjectInfo.CompleteMainIfEnable(model.Entity.ProjectId);
                }
                tranScope.Complete();
                return Ok();
            }
        }
    }
}
