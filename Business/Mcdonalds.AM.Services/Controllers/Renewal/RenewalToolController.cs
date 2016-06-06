using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.DataTransferObjects.Renewal;
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
    public class RenewalToolController : ApiController
    {
        [Route("api/renewalTool/initPage")]
        [HttpGet]
        public IHttpActionResult InitPage(string projectId, string id = null)
        {
            return Ok(RenewalTool.InitPage(projectId, id));
        }

        [Route("api/renewalTool/getTTMFinanceData")]
        [HttpGet]
        public IHttpActionResult GetTTMFinanceData(string projectId, string yearMonth)
        {
            var keyValue = yearMonth.Split('-');
            return Ok(RenewalToolFinMeasureInput.GetFinanceData(projectId, keyValue[0], keyValue[1]));
        }

        [Route("api/renewalTool/save")]
        [HttpPost]
        public IHttpActionResult Save(RenewalToolDTO postData)
        {
            postData.Entity.Save(postData.ProjectComment, () =>
            {
                postData.FinMeasureInput.Save();
                postData.WriteOffAndReinCost.Save();
            });
            return Ok();

        }
        [Route("api/renewalTool/submit")]
        [HttpPost]
        public IHttpActionResult Submit(RenewalToolDTO postData)
        {
            postData.Entity.Submit(postData.ProjectComment, () =>
            {
                postData.FinMeasureInput.Save();
                postData.WriteOffAndReinCost.Save();
            });
            return Ok();
        }

        [Route("api/renewalTool/approve")]
        [HttpPost]
        public IHttpActionResult Approve(PostWorkflowData<RenewalTool> postData)
        {
            postData.Entity.Approve(postData.ProjectComment, postData.SN);
            return Ok();
        }

        [Route("api/renewalTool/return")]
        [HttpPost]
        public IHttpActionResult Return(PostWorkflowData<RenewalTool> postData)
        {
            postData.Entity.Return(postData.ProjectComment, postData.SN);
            return Ok();
        }

        [Route("api/renewalTool/resubmit")]
        [HttpPost]
        public IHttpActionResult Resubmit(RenewalToolDTO postData)
        {
            postData.Entity.Resubmit(postData.ProjectComment, postData.SN, () =>
            {
                postData.FinMeasureInput.Save();
                postData.WriteOffAndReinCost.Save();
            });
            return Ok();
        }

        [Route("api/renewalTool/recall")]
        [HttpPost]
        public IHttpActionResult Recall(PostWorkflowData<RenewalTool> postData)
        {
            postData.Entity.Recall(postData.ProjectComment);
            return Ok();
        }

        [Route("api/renewalTool/edit")]
        [HttpPost]
        public IHttpActionResult Edit(RenewalTool entity)
        {
            return Ok(new ProjectEditResult
            {
                TaskUrl = entity.Edit()
            });
        }

        [Route("api/renewalTool/downloadToolTemplate")]
        [HttpGet]
        public void DownloadToolTemplate(string projectId)
        {
            var renewalTool = RenewalTool.Get(projectId);
            var renewalInfo = RenewalInfo.Get(projectId);
            var fileName = renewalTool.DownloadToolTemplate();
            var current = HttpContext.Current;
            current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + SiteFilePath.GetTemplateFileName(renewalInfo.USCode, FlowCode.Renewal_Tool, "RenewalToolTemplate.xlsx"));
            current.Response.ContentType = "application/octet-stream";
            current.Response.WriteFile(fileName);
            current.Response.End();
        }

        [Route("api/renewalTool/uploadTool/{projectId}")]
        [HttpPost]
        public IHttpActionResult UploadTool(string projectId)
        {
            var renewalTool = RenewalTool.Get(projectId);
            renewalTool.UploadTool();
            return Ok();
        }

        [Route("api/renewalTool/confirmUploadTool")]
        [HttpPost]
        public IHttpActionResult ConfirmUploadTool(PostWorkflowData<RenewalTool> postData)
        {
            
            postData.Entity.ConfirmUploadTool(postData.SN);
            return Ok();
        }

        [Route("api/renewalTool/getFinMeasureOutput")]
        [HttpGet]
        public IHttpActionResult GetFinMeasureOutput(Guid toolId)
        {
            var finOutput = RenewalToolFinMeasureOutput.GetByToolId(toolId);
            return Ok(finOutput);
        }

    }
}
