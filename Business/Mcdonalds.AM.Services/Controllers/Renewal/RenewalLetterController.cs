using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers.Renewal
{
    public class RenewalLetterController : ApiController
    {
        [Route("api/renewalLetter/initPage")]
        [HttpGet]
        public IHttpActionResult InitPage(string projectId, string id = null)
        {
            var letter = RenewalLetter.Get(projectId, id);
            var project = ProjectInfo.Get(projectId, FlowCode.Renewal_Letter);
            var isActor = ProjectUsers.IsRole(projectId, ClientCookie.UserCode, ProjectUserRoleCode.AssetActor);
            letter.IsProjectFreezed = letter.CheckIfFreezeProject(projectId);
            var nextRefTableId = new Guid(FlowInfo.GetRefTableId("RenewalConsInfo", projectId));
            var nextFlowStarted = ProjectInfo.IsFlowStarted(projectId, FlowCode.Renewal_ConsInfo);
            var haveTask = TaskWork.Any(t => t.RefID == projectId && t.TypeCode == FlowCode.Renewal_Letter && t.Status == TaskWorkStatus.UnFinish && t.ReceiverAccount == ClientCookie.UserCode);
            var projectComment = ProjectComment.GetSavedComment(letter.Id, "RenewalLetter", ClientCookie.UserCode);
            var isStarted = ProjectInfo.IsFlowStarted(projectId, FlowCode.Renewal_Letter);
            var isFinished = ProjectInfo.IsFlowFinished(projectId, FlowCode.Renewal_Letter);
            return Ok(new
            {
                Info = RenewalInfo.FirstOrDefault(info => info.ProjectId == letter.ProjectId),
                Letter = letter,
                ProjectComment = projectComment != null ? projectComment.Content : "",
                Editable = ProjectInfo.IsFlowEditable(projectId, FlowCode.Renewal_Letter),
                Recallable = ProjectInfo.IsFlowRecallable(projectId, FlowCode.Renewal_Letter),
                Savable = ProjectInfo.IsFlowSavable(projectId, FlowCode.Renewal_Letter) && string.IsNullOrEmpty(id)
            });
        }

        [Route("api/renewalLetter/save")]
        [HttpPost]
        public IHttpActionResult Save(PostWorkflowData<RenewalLetter> postData)
        {
            postData.Entity.Save(postData.ProjectComment);
            return Ok();

        }
        [Route("api/renewalLetter/submit")]
        [HttpPost]
        public IHttpActionResult Submit(PostWorkflowData<RenewalLetter> postData)
        {
            postData.Entity.Submit(postData.ProjectComment);
            return Ok();
        }

        [Route("api/renewalLetter/approve")]
        [HttpPost]
        public IHttpActionResult Approve(PostWorkflowData<RenewalLetter> postData)
        {
            postData.Entity.Approve(postData.ProjectComment, postData.SN);
            return Ok();
        }

        [Route("api/renewalLetter/return")]
        [HttpPost]
        public IHttpActionResult Return(PostWorkflowData<RenewalLetter> postData)
        {
            postData.Entity.Return(postData.ProjectComment, postData.SN);
            return Ok();
        }

        [Route("api/renewalLetter/resubmit")]
        [HttpPost]
        public IHttpActionResult Resubmit(PostWorkflowData<RenewalLetter> postData)
        {
            postData.Entity.Resubmit(postData.ProjectComment, postData.SN);
            return Ok();
        }

        [Route("api/renewalLetter/recall")]
        [HttpPost]
        public IHttpActionResult Recall(PostWorkflowData<RenewalLetter> postData)
        {
            postData.Entity.Recall(postData.ProjectComment);
            return Ok();
        }

        [Route("api/renewalLetter/edit")]
        [HttpPost]
        public IHttpActionResult Edit(RenewalLetter entity)
        {
            return Ok(new ProjectEditResult
            {
                TaskUrl = entity.Edit()
            });
        }
    }
}
