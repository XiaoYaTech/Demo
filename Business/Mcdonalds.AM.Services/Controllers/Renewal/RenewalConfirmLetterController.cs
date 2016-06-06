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
    public class RenewalConfirmLetterController : ApiController
    {

        [HttpGet]
        [Route("api/renewalConfirmLetter/initPage")]
        public IHttpActionResult InitPage(string projectId, string id=null)
        {
            var entity = RenewalConfirmLetter.Get(projectId, id);
            return Ok(new
            {
                Entity = entity,
                Info = RenewalInfo.Get(projectId),
                Editable = ProjectInfo.IsFlowEditable(projectId, FlowCode.Renewal_ConfirmLetter),
                Recallable = ProjectInfo.IsFlowRecallable(projectId, FlowCode.Renewal_ConfirmLetter),
                Savable = ProjectInfo.IsFlowSavable(projectId, FlowCode.Renewal_ConfirmLetter)
            });
        }

        [Route("api/renewalConfirmLetter/save")]
        [HttpPost]
        public IHttpActionResult Save(RenewalConfirmLetter entity)
        {
            entity.Save();
            return Ok();
        }
        [Route("api/renewalConfirmLetter/submit")]
        [HttpPost]
        public IHttpActionResult Submit(RenewalConfirmLetter entity)
        {
            entity.Submit();
            return Ok();
        }


        [Route("api/renewalConfirmLetter/resubmit")]
        [HttpPost]
        public IHttpActionResult Resubmit(PostWorkflowData<RenewalConfirmLetter> postData)
        {
            postData.Entity.Resubmit(postData.SN);
            return Ok();
        }

        [Route("api/renewalConfirmLetter/recall")]
        [HttpPost]
        public IHttpActionResult Recall(PostWorkflowData<RenewalConfirmLetter> postData)
        {
            postData.Entity.Recall(postData.ProjectComment);
            return Ok();
        }

        [Route("api/renewalConfirmLetter/edit")]
        [HttpPost]
        public IHttpActionResult Edit(RenewalConfirmLetter entity)
        {
            return Ok(new ProjectEditResult
            {
                TaskUrl = entity.Edit()
            });
        }
    }
}
