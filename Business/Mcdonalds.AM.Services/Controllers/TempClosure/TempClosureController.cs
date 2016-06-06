using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Common;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Entities.Condition;
using Mcdonalds.AM.DataAccess.Infrastructure;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers.TempClosure
{
    public class TempClosureController : ApiController
    {
        [Route("api/tempClosure/create")]
        [HttpPost]
        public IHttpActionResult Create(PostCreateWorkflow<TempClosureInfo> tempClosure)
        {
            TempClosureInfo.Create(tempClosure);
            return Ok();
        }

        [Route("api/tempClosure/get")]
        [HttpGet]
        public IHttpActionResult GetTempClosureInfo(string projectId)
        {
            var tempClosure = TempClosureInfo.Get(projectId);
            return Ok(tempClosure);
        }

        [Route("api/tempClosure/enableEditProject")]
        [HttpGet]
        public IHttpActionResult EnableEditProject(string projectId)
        {
            var isPackgeStarted = ProjectInfo.IsFlowStarted(projectId, FlowCode.TempClosure_ClosurePackage);
            return Ok(isPackgeStarted);
        }

        [Route("api/tempClosure/isTempClosed")]
        [HttpGet]
        public IHttpActionResult IsTempClosed(string projectId)
        {
            var result = false;
            var projectInfo = ProjectInfo.Get(projectId, FlowCode.TempClosure);
            if (projectInfo != null)
            {
                if (projectInfo.Status == ProjectStatus.Completed)
                    result = true;
                else if (projectInfo.Status == ProjectStatus.UnFinish)
                {
                    var closureMemo = TempClosureMemo.FirstOrDefault(i=>i.ProjectId==projectId);
                    if (closureMemo == null)
                        result = false;
                    else
                    {
                        var tempClosure = TempClosureInfo.Get(projectId);
                        if (tempClosure != null && tempClosure.ActualTempClosureDate.Date <= DateTime.Now.Date)
                            result = true;
                    }
                }
            }
            return Ok(new
            {
                result = result,
                isActor = ProjectUsers.IsRole(projectId, ClientCookie.UserCode, ProjectUserRoleCode.AssetActor)
            });
        }

        [Route("api/tempClosure/update")]
        [HttpPost]
        public IHttpActionResult UpdateTempClosureInfo(TempClosureInfo entity)
        {
            entity.Update();
            return Ok();
        }
    }
}
