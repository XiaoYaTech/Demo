using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers.Renewal
{
    public class RenewalAnalysisController : ApiController
    {

        [HttpGet]
        [Route("api/renewalAnalysis/initPage")]
        public IHttpActionResult InitPage(string projectId)
        {
            return Ok(RenewalAnalysis.InitPage(projectId));
        }

        [HttpGet]
        [Route("api/renewalAnalysis/contractinfo")]
        public IHttpActionResult ContractInfo(string projectId)
        {
            var info = RenewalAnalysis.Get(projectId);
            string FreeRentalPeriod = info.FreeRentalPeriod;
            string ExclusivityClause = string.IsNullOrEmpty(info.ExclusivityClauseNew) ? null : (info.ExclusivityClauseNew == "Y" ? "1" : "0");
            ProjectContractInfo contract = ProjectContractInfo.FirstOrDefault(c => c.ProjectId == projectId);
            if (contract != null)
            {
                FreeRentalPeriod = contract.FreeRentalPeriod;
                ExclusivityClause = contract.ExclusivityClause;
            }
            return Ok(new
            {
                FreeRentalPeriod = FreeRentalPeriod,
                ExclusivityClause = ExclusivityClause
            });
        }

        [Route("api/renewalAnalysis/save")]
        [HttpPost]
        public IHttpActionResult Save(RenewalAnalysis entity)
        {
            entity.Save();
            return Ok();
        }
        [Route("api/renewalAnalysis/submit")]
        [HttpPost]
        public IHttpActionResult Submit(RenewalAnalysis entity)
        {
            entity.Submit();
            return Ok();
        }

        [Route("api/renewalAnalysis/resubmit")]
        [HttpPost]
        public IHttpActionResult Resubmit(PostWorkflowData<RenewalAnalysis> postData)
        {
            postData.Entity.Resubmit(postData.SN);
            return Ok();
        }

        [Route("api/renewalAnalysis/recall")]
        [HttpPost]
        public IHttpActionResult Recall(PostWorkflowData<RenewalAnalysis> postData)
        {
            postData.Entity.Recall(postData.ProjectComment);
            return Ok();
        }

        [Route("api/renewalAnalysis/edit")]
        [HttpPost]
        public IHttpActionResult Edit(RenewalAnalysis entity)
        {
            return Ok(new ProjectEditResult
            {
                TaskUrl = entity.Edit()
            });
        }
    }
}
