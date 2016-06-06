using System.Text;
using System.Transactions;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.DataTransferObjects.Renewal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers.Renewal
{
    public class RenewalLegalApprovalController : ApiController
    {
        [Route("api/renewalLegalApproval/initPage")]
        [HttpGet]
        public IHttpActionResult InitPage(string projectId, string id = null)
        {
            var dto = RenewalLegalApproval.InitPage(projectId, id);
            return Ok(dto);
        }

        [Route("api/renewalLegalApproval/save")]
        [HttpPost]
        public IHttpActionResult Save(RenewalLegalApprovalDTO postData)
        {
            postData.Entity.Save(postData.ProjectComment);
            return Ok();

        }
        [Route("api/renewalLegalApproval/submit")]
        [HttpPost]
        public IHttpActionResult Submit(RenewalLegalApprovalDTO postData)
        {
            postData.Entity.Submit(postData.ProjectComment);
            return Ok();
        }

        [Route("api/renewalLegalApproval/approve")]
        [HttpPost]
        public IHttpActionResult Approve(RenewalLegalApprovalDTO postData)
        {
            postData.Entity.Approve(postData.ProjectComment, postData.SN, postData.IsGeneralCounsel);
            if (postData.IsGeneralCounsel)
                new AttachmentController().PreparePackDownload(postData.Entity.TableName, postData.Entity.ProjectId);
            return Ok();
        }

        [Route("api/renewalLegalApproval/return")]
        [HttpPost]
        public IHttpActionResult Return(RenewalLegalApprovalDTO postData)
        {
            postData.Entity.Return(postData.ProjectComment, postData.SN);
            return Ok();
        }

        [Route("api/renewalLegalApproval/resubmit")]
        [HttpPost]
        public IHttpActionResult Resubmit(RenewalLegalApprovalDTO postData)
        {
            postData.Entity.Resubmit(postData.ProjectComment, postData.SN);
            return Ok();
        }

        [Route("api/renewalLegalApproval/recall")]
        [HttpPost]
        public IHttpActionResult Recall(RenewalLegalApprovalDTO postData)
        {
            postData.Entity.Recall(postData.ProjectComment);
            return Ok();
        }

        [Route("api/renewalLegalApproval/edit")]
        [HttpPost]
        public IHttpActionResult Edit(RenewalLegalApproval entity)
        {
            return Ok(new ProjectEditResult
            {
                TaskUrl = entity.Edit()
            });
        }
    }
}
