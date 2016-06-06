using Mcdonalds.AM.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers
{
    public class ContractRevisionController : ApiController
    {
        [Route("api/contract/revisions")]
        [HttpGet]
        public IHttpActionResult GetRevisions(string projectId,Guid contractId)
        {
            return Ok(ProjectContractRevision.Get(projectId,contractId).ToList());
        }

        [Route("api/contract/storerevisions")]
        [HttpGet]
        public IHttpActionResult GetRevisions(Guid contractId)
        {
            return Ok(StoreContractRevision.Get(contractId).ToList());
        }

        
    }
}
