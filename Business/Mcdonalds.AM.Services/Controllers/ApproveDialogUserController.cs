using Mcdonalds.AM.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers
{
    public class ApproveDialogUserController : ApiController
    {
        [Route("api/ApproveDialogUser/GetApproveDialogUsers")]
        [HttpGet]
        public IHttpActionResult GetApproveDialogUsers(string ProjectId, string FlowCode)
        {
            var approvUser = ApproveDialogUser.GetApproveDialogUser(ProjectId, FlowCode);
            return Ok(approvUser);
        }

        [Route("api/ApproveDialogUser/SaveApproveDialogUsers")]
        [HttpPost]
        public IHttpActionResult SaveApproveDialogUsers(ApproveDialogUser users)
        {
            users.Save();
            return Ok();
        }
    }
}