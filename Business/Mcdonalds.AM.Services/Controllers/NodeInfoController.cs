using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Mcdonalds.AM.DataAccess;

namespace Mcdonalds.AM.Services.Controllers
{
    public class NodeInfoController : ApiController
    {
        private NodeInfo nodeInfo = new NodeInfo();

        [Route("api/NodeInfo/QueryCheckPoints/{projectId}/{flowCode}")]
        [HttpGet]
        public IHttpActionResult QueryCheckPoints(string projectId, string flowCode)
        {
            var list = nodeInfo.QueryCheckPoints(projectId, flowCode);
            var project = ProjectInfo.Get(projectId, flowCode.Split('_')[0]);
            return Ok(new
            {
                Nodes = list,
                Info = project,
                Progress = ProjectProgress.GetProgress(projectId),
                Operators = TaskWork.GetOperators(flowCode, projectId),
                IsFinished = ProjectInfo.IsFlowFinished(projectId, flowCode)
            });
        }
    }
}
