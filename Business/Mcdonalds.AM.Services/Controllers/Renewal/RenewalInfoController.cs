using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers.Renewal
{
    public class RenewalInfoController : ApiController
    {
        [Route("api/renewal/create")]
        [HttpPost]
        public IHttpActionResult Create(PostCreateWorkflow<RenewalInfo> postCreateRenewal)
        {
            RenewalInfo.Create(postCreateRenewal);
            return Ok();
        }

        [Route("api/renewal/get")]
        [HttpGet]
        public IHttpActionResult Get(string projectId)
        {
            return Ok(RenewalInfo.Get(projectId));
        }

        [Route("api/renewal/update")]
        [HttpPost]
        public IHttpActionResult Update(RenewalInfo renewal)
        {
            renewal.UpdateFromProjectList();
            return Ok();
        }
    }
}
