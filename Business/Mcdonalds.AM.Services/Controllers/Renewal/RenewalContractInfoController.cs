using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers.Renewal
{
    public class RenewalContractInfoController : ApiController
    {
        [Route("api/renewalContractInfo/initPage")]
        [HttpGet]
        public IHttpActionResult InitPage(string projectId)
        {
            return Ok(new
            {
                Savable = ProjectInfo.IsFlowSavable(projectId, FlowCode.Renewal_ContractInfo)
            });
        }
    }
}
