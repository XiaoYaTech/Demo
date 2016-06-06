using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.DataTransferObjects.Renewal;
using Mcdonalds.AM.Services.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers.Renewal
{
    public class RenewalPackageController : ApiController
    {
        [Route("api/renewalPackage/initPage")]
        [HttpGet]
        public IHttpActionResult InitPage(string projectId, string id = null)
        {
            return Ok(RenewalPackage.InitPage(projectId, id));
        }

        [Route("api/renewalPackage/save")]
        [HttpPost]
        public IHttpActionResult Save(RenewalPackageDTO postData)
        {
            postData.Entity.Save(postData.ProjectComment);
            return Ok();

        }
        [Route("api/renewalPackage/submit")]
        [HttpPost]
        public IHttpActionResult Submit(RenewalPackageDTO postData)
        {
            postData.Entity.Submit(postData.ProjectComment, () =>
            {
                RenewalEmail.SendPackageApprovalEmail(postData.Info, postData.Entity, postData.Entity.AppUsers);
            });
            return Ok();
        }

        [Route("api/renewalPackage/approve")]
        [HttpPost]
        public IHttpActionResult Approve(RenewalPackageDTO postData)
        {
            postData.Entity.Approve(postData.ProjectComment, postData.SN);
            return Ok();
        }

        [Route("api/renewalPackage/return")]
        [HttpPost]
        public IHttpActionResult Return(RenewalPackageDTO postData)
        {
            postData.Entity.Return(postData.ProjectComment, postData.SN);
            return Ok();
        }

        [Route("api/renewalPackage/reject")]
        [HttpPost]
        public IHttpActionResult Reject(RenewalPackageDTO postData)
        {
            postData.Entity.Reject(postData.ProjectComment, postData.SN);
            return Ok();
        }

        [Route("api/renewalPackage/resubmit")]
        [HttpPost]
        public IHttpActionResult Resubmit(RenewalPackageDTO postData)
        {
            postData.Entity.Resubmit(postData.ProjectComment, postData.SN);
            return Ok();
        }

        [Route("api/renewalPackage/confirm")]
        [HttpPost]
        public IHttpActionResult Confirm(RenewalPackageDTO postData)
        {
            postData.Entity.Confirm(postData.SN);
            return Ok();
        }

        [Route("api/renewalPackage/recall")]
        [HttpPost]
        public IHttpActionResult Recall(RenewalPackageDTO postData)
        {
            postData.Entity.Recall(postData.ProjectComment);
            return Ok();
        }

        [Route("api/renewalPackage/edit")]
        [HttpPost]
        public IHttpActionResult Edit(RenewalPackage entity)
        {
            return Ok(new ProjectEditResult
            {
                TaskUrl = entity.Edit()
            });
        }

        [Route("api/renewalPackage/needCDOApproval")]
        [HttpGet]
        public IHttpActionResult NeedCDOApproval(string projectId)
        {
            RenewalAnalysis analysis = RenewalAnalysis.Get(projectId);
            return Ok(new
            {
                NeedCDOApproval = Math.Abs(analysis.RentDeviation.HasValue ? analysis.RentDeviation.Value : 0M) >= 0.1M
            });
        }
    }
}
