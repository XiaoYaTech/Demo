using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.EmailServiceReference;
using Mcdonalds.AM.Services.Entities.TempClosure;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Transactions;
using System.Web;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers.TempClosure
{
    public class TempClosureLegalReviewController : ApiController
    {
        [Route("api/tempClosureLegalReview/get")]
        [HttpGet]
        public IHttpActionResult GetLegalReview(string projectId, Guid? Id = null)
        {
            var tempClosure = TempClosureInfo.FirstOrDefault(tc => tc.ProjectId == projectId);
            var project = ProjectInfo.Get(projectId, FlowCode.TempClosure_LegalReview);
            var isActor = ProjectUsers.IsRole(projectId, ClientCookie.UserCode, ProjectUserRoleCode.AssetActor);
            TempClosureLegalReview entity;
            if (Id != null)
            {
                entity = TempClosureLegalReview.Get(Id.Value);
            }
            else
            {
                entity = TempClosureLegalReview.Get(projectId);
            }
            entity.IsProjectFreezed = entity.CheckIfFreezeProject(projectId);
            var nextRefTableId = new Guid(FlowInfo.GetRefTableId("TempClosurePackage", projectId));
            var nextFlowStarted = ProjectInfo.Any(p => p.FlowCode == FlowCode.TempClosure_ClosurePackage && p.ProjectId == projectId && p.NodeCode != NodeCode.Start);
            //var haveTask = TaskWork.Any(t => t.RefID == projectId && t.TypeCode == FlowCode.TempClosure_LegalReview && t.Status == TaskWorkStatus.UnFinish && t.ReceiverAccount == ClientCookie.UserCode);
            var projectComment = ProjectComment.GetSavedComment(entity.Id, "TempClosureLegalReview", ClientCookie.UserCode);
            var isLegalStarted = ProjectInfo.IsFlowStarted(projectId, FlowCode.TempClosure_LegalReview);
            return Ok(new
            {
                Info = tempClosure,
                LegalReview = entity,
                ProjectComment = projectComment != null ? projectComment.Content : "",
                Editable = project.Status == ProjectStatus.Finished && isActor && !nextFlowStarted && !entity.IsHistory,
                Recallable = project.Status != ProjectStatus.Finished && isActor && !entity.IsHistory && isLegalStarted,
                Savable = ProjectInfo.IsFlowSavable(projectId,FlowCode.TempClosure_LegalReview)
            });

        }

        [Route("api/tempClosureLegalReview/save")]
        [HttpPost]
        public IHttpActionResult SaveLegalReview(PostWorkflowData<TempClosureLegalReview> postData)
        {
            postData.Entity.Save(postData.ProjectComment);
            return Ok();

        }

        [Route("api/tempClosureLegalReview/submit")]
        [HttpPost]
        public IHttpActionResult SubmitLegalReview(PostWorkflowData<TempClosureLegalReview> postData)
        {
            postData.Entity.Submit(postData.ProjectComment);
            return Ok();
        }

        [Route("api/tempClosureLegalReview/approve")]
        [HttpPost]
        public IHttpActionResult ApproveLegalReview(PostWorkflowData<TempClosureLegalReview> postData)
        {
            postData.Entity.Approve(postData.ProjectComment, postData.SN);
            return Ok();
        }

        [Route("api/tempClosureLegalReview/return")]
        [HttpPost]
        public IHttpActionResult ReturnLegalReview(PostWorkflowData<TempClosureLegalReview> postData)
        {
            postData.Entity.Return(postData.ProjectComment, postData.SN);
            return Ok();
        }

        [Route("api/tempClosureLegalReview/resubmit")]
        [HttpPost]
        public IHttpActionResult ResubmitLegalReview(PostWorkflowData<TempClosureLegalReview> postData)
        {
            postData.Entity.Resubmit(postData.ProjectComment, postData.SN);
            return Ok();
        }

        [Route("api/tempClosureLegalReview/recall")]
        [HttpPost]
        public IHttpActionResult RecallLegalReview(PostWorkflowData<TempClosureLegalReview> postData)
        {
            postData.Entity.Recall(postData.ProjectComment);
            return Ok();
        }

        [Route("api/tempClosureLegalReview/edit")]
        [HttpPost]
        public IHttpActionResult EditLegalReview(TempClosureLegalReview entity)
        {
            return Ok(new ProjectEditResult
            {
                TaskUrl = entity.Edit()
            });
        }
    }
}
