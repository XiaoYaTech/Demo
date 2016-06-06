using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.Services.Entities.TempClosure;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers.TempClosure
{
    public class TempClosurePackageController : ApiController
    {
        [Route("api/tempClosurePackage/get")]
        [HttpGet]
        public IHttpActionResult Get(string projectId, Guid? Id = null)
        {
            var project = ProjectInfo.Get(projectId, FlowCode.TempClosure_ClosurePackage);
            var tempClosure = TempClosureInfo.FirstOrDefault(tc => tc.ProjectId == projectId);
            var isActor = ProjectUsers.IsRole(projectId, ClientCookie.UserCode, ProjectUserRoleCode.AssetActor);
            TempClosurePackage entity;
            if (Id != null)
            {
                entity = TempClosurePackage.Get(Id.Value);
            }
            else
            {
                entity = TempClosurePackage.Get(projectId);
            }
            if (entity != null)
            {
                entity.IsProjectFreezed = entity.CheckIfFreezeProject(projectId);
                var SavedComment = ProjectComment.GetSavedComment(entity.Id, "TempClosurePackage", ClientCookie.UserCode);
                var hasValidTask = TaskWork.Any(t => t.RefID == projectId && t.Status == TaskWorkStatus.UnFinish && t.ReceiverAccount != ClientCookie.UserCode);
                return Ok(new
                {
                    Info = tempClosure,
                    ClosurePackage = entity,
                    Approver = ApproveDialogUser.GetApproveDialogUser(projectId, FlowCode.TempClosure_ClosurePackage),
                    ProjectComment = SavedComment != null ? SavedComment.Content : "",
                    Editable = ProjectInfo.IsFlowEditable(projectId, FlowCode.TempClosure_ClosurePackage),
                    Recallable = ProjectInfo.IsFlowRecallable(projectId, FlowCode.TempClosure_ClosurePackage),
                    Rejectable = ApproveDialogUser.Any(u => u.ProjectId == projectId && u.FlowCode == FlowCode.TempClosure_ClosurePackage && u.VPGMCode == ClientCookie.UserCode),
                    Savable = ProjectInfo.IsFlowSavable(projectId, FlowCode.TempClosure_ClosurePackage)
                });
            }
            else
            {
                entity = TempClosurePackage.Create(projectId);
                return Ok(new
                {
                    Info = tempClosure,
                    ClosurePackage = entity,
                    Approver = ApproveDialogUser.GetApproveDialogUser(projectId, FlowCode.TempClosure_ClosurePackage),
                    ProjectComment = "",
                    Editable = (project.Status == ProjectStatus.Finished || project.Status == ProjectStatus.Rejected) && isActor && !entity.IsHistory,
                    Recallable = project.Status == ProjectStatus.UnFinish && isActor && project.NodeCode != NodeCode.Start,
                    Rejectable = ApproveDialogUser.Any(u => u.ProjectId == projectId && u.FlowCode == FlowCode.TempClosure_ClosurePackage && u.VPGMCode == ClientCookie.UserCode),
                    Savable = ProjectInfo.IsFlowSavable(projectId, FlowCode.TempClosure_ClosurePackage)
                });
            }
        }
        [Route("api/tempClosurePackage/getApprovers")]
        [HttpGet]
        public IHttpActionResult getApprovers(string projectId)
        {
            var project = ProjectInfo.Get(projectId, FlowCode.TempClosure_ClosurePackage);
            return Ok(new
            {
                MarketMgrs = Employee.GetStoreEmployeesByRole(project.USCode, RoleCode.Market_Asset_Mgr),
                RegionalMgrs = Employee.GetStoreEmployeesByRole(project.USCode, RoleCode.Regional_Asset_Mgr),
                MDDs = Employee.GetStoreEmployeesByRole(project.USCode, RoleCode.Market_DD),
                GMs = Employee.GetStoreEmployeesByRole(project.USCode, RoleCode.GM),
                FCs = Employee.GetStoreEmployeesByRole(project.USCode, RoleCode.Finance_Controller),
                VPGMs = Employee.GetStoreEmployeesByRole(project.USCode, RoleCode.VPGM),
                MCCLAssetMgrs = Employee.GetStoreEmployeesByRole(project.USCode, RoleCode.MCCL_Asset_Mgr),
                MCCLAssetDirs = Employee.GetStoreEmployeesByRole(project.USCode, RoleCode.MCCL_Asset_Director),
                ProjectDto = TempClosurePackage.GetApproverAndNoticeUsers(projectId)
            });
        }

        [Route("api/tempClosurePackage/save")]
        [HttpPost]
        public IHttpActionResult SaveClosurePackage(PostWorkflowData<TempClosurePackage> postData)
        {
            postData.Entity.Save(postData.ProjectComment);
            return Ok();
        }

        [Route("api/tempClosurePackage/submit")]
        [HttpPost]
        public IHttpActionResult SubmitClosurePackage(PostWorkflowData<TempClosurePackage> postData)
        {

            var tempClosureInfo = TempClosureInfo.FirstOrDefault(tc => tc.ProjectId == postData.Entity.ProjectId);
            postData.Entity.Submit(postData.ProjectComment, postData.ProjectDto.ApproveUsers, () =>
            {
                TempClosureEmail.SendPackageApprovalEmail(tempClosureInfo, postData.Entity, postData.ProjectDto.ApproveUsers);
            });
            return Ok();
        }

        [Route("api/tempClosurePackage/approve")]
        [HttpPost]
        public IHttpActionResult ApproveClosurePackage(PostWorkflowData<TempClosurePackage> postData)
        {
            postData.Entity.Approve(postData.ProjectComment, postData.SN);
            return Ok();
        }

        [Route("api/tempClosurePackage/return")]
        [HttpPost]
        public IHttpActionResult ReturnClosurePackage(PostWorkflowData<TempClosurePackage> postData)
        {
            postData.Entity.Return(postData.ProjectComment, postData.SN);
            return Ok();
        }

        [Route("api/tempClosurePackage/reject")]
        [HttpPost]
        public IHttpActionResult RejectClosurePackage(PostWorkflowData<TempClosurePackage> postData)
        {
            postData.Entity.Reject(postData.ProjectComment, postData.SN);
            return Ok();
        }

        [Route("api/tempClosurePackage/resubmit")]
        [HttpPost]
        public IHttpActionResult ResubmitClosurePackage(PostWorkflowData<TempClosurePackage> postData)
        {

            var tempClosureInfo = TempClosureInfo.FirstOrDefault(tc => tc.ProjectId == postData.Entity.ProjectId);
            postData.Entity.Resubmit(postData.ProjectComment, postData.SN, postData.ProjectDto.ApproveUsers, () =>
            {
                TempClosureEmail.SendPackageApprovalEmail(tempClosureInfo, postData.Entity, postData.ProjectDto.ApproveUsers);
            });
            return Ok();
        }

        [Route("api/tempClosurePackage/recall")]
        [HttpPost]
        public IHttpActionResult RecallClosurePackage(PostWorkflowData<TempClosurePackage> postData)
        {
            postData.Entity.Recall(postData.ProjectComment);
            return Ok();
        }

        [Route("api/tempClosurePackage/edit")]
        [HttpPost]
        public IHttpActionResult EditClosurePackage(TempClosurePackage entity)
        {
            return Ok(new ProjectEditResult
            {
                TaskUrl = entity.Edit()
            });
        }

        [Route("api/tempClosurePackage/confirm")]
        [HttpPost]
        public IHttpActionResult ConfirmClosurePackage(TempClosurePackage entity)
        {
            entity.Confirm();
            return Ok();
        }
    }
}
