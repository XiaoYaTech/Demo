using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers.Renewal
{
    public class RenewalClearanceReportController : ApiController
    {

        [HttpGet]
        [Route("api/renewalClearanceReport/initPage")]
        public IHttpActionResult InitPage(string projectId,string id = null)
        {
            var entity = RenewalClearanceReport.Get(projectId, id);
            return Ok(new
            {
                Entity = entity,
                Info = RenewalInfo.Get(projectId),
                Editable = ProjectInfo.IsFlowEditable(projectId, FlowCode.Renewal_ClearanceReport),
                Recallable = ProjectInfo.IsFlowRecallable(projectId, FlowCode.Renewal_ClearanceReport),
                Savable = ProjectInfo.IsFlowSavable(projectId, FlowCode.Renewal_ClearanceReport)
            });
        }

        [Route("api/renewalClearanceReport/save")]
        [HttpPost]
        public IHttpActionResult Save(RenewalClearanceReport entity)
        {
            entity.Save();
            return Ok();
        }

        [Route("api/renewalClearanceReport/submit")]
        [HttpPost]
        public IHttpActionResult Submit(RenewalClearanceReport entity)
        {
            entity.Submit();
            return Ok();
        }

        [Route("api/renewalClearanceReport/resubmit")]
        [HttpPost]
        public IHttpActionResult Resubmit(PostWorkflowData<RenewalClearanceReport> postData)
        {
            postData.Entity.Resubmit(postData.SN);
            return Ok();
        }

        [Route("api/renewalClearanceReport/recall")]
        [HttpPost]
        public IHttpActionResult Recall(PostWorkflowData<RenewalClearanceReport> postData)
        {
            postData.Entity.Recall(postData.ProjectComment);
            return Ok();
        }

        [Route("api/renewalClearanceReport/edit")]
        [HttpPost]
        public IHttpActionResult Edit(RenewalClearanceReport entity)
        {
            return Ok(new ProjectEditResult
            {
                TaskUrl = entity.Edit()
            });
        }
    }
}
