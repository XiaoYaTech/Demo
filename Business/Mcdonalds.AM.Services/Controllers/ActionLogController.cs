using Mcdonalds.AM.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers
{
    public class ActionLogController : ApiController
    {
        [Route("api/actionlog")]
        public IHttpActionResult GetActionLogs(string projectId)
        {
            var list = ActionLog.Search(log => log.ProjectId == projectId).OrderByDescending(log => log.CreateTime).ToList();
            return Ok(list);
        }
    }
}
