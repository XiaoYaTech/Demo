using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Web.Http;
using PagedDataSource = Mcdonalds.AM.DataAccess.DataTransferObjects.PagedDataSource;
using ApiProxy = Mcdonalds.AM.ApiCaller.ApiProxy;
using System.Web;
using System.Text;
using Newtonsoft.Json;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Entities;
using System.Linq;
using System.Transactions;
using System.Collections.Generic;
using Mcdonalds.AM.Services.Common;
using System.IO;
using Mcdonalds.AM.DataAccess.Common.Excel;

namespace Mcdonalds.AM.Services.Controllers
{
    public class ProjectController : ApiController
    {
        [Route("api/project/{pageIndex}/{pageSize}")]
        [HttpPost]
        public IHttpActionResult Search(int pageIndex, int pageSize, ProjectSearchCondition condition)
        {
            int totalItems = 0;
            var list = VProject.Search(pageIndex, pageSize, condition, ref totalItems);

            foreach (var projectItem in list)
            {
                var item = projectItem;

                item.SetPendingRight();
                //item.SetPackageHoldingSource();
            }

            //if (condition.HoldingStatus != HoldingStatus.Unknown)
            //{
            //    list =
            //        list.Where(
            //            e => e.PackageHoldingDto != null && e.PackageHoldingDto.Status == condition.HoldingStatus).ToList();
            //}
            return Ok(new PagedDataSource(totalItems, list.ToArray()));
        }

        [Route("api/project/get/uscode")]
        public IHttpActionResult GetUSCode(string projectId)
        {
            return Ok(ProjectInfo.FirstOrDefault(p => p.ProjectId == projectId).USCode);
        }

        [Route("api/project/GetProjectIDByProcInstID/{procInstID}")]
        public IHttpActionResult GetProjectIDByProcInstID(int procInstID)
        {
            var projectId = ProjectInfo.GetProjectIDByProcInstId(procInstID);
            return Ok(projectId);
        }

        [HttpPost]
        [Route("api/project/PendingProject")]
        public IHttpActionResult PendingProject()
        {
            string projectId = Request.Content.ReadAsStringAsync().Result;
            new ProjectInfo().PendingProject(projectId);
            return Ok();
        }

        [HttpPost]
        [Route("api/project/ResumeProject")]
        public IHttpActionResult ResumeProject()
        {
            string projectId = Request.Content.ReadAsStringAsync().Result;
            new ProjectInfo().ResumeProject(projectId);
            return Ok();
        }

        [HttpPost]
        [Route("api/project/RecallProject")]
        public IHttpActionResult RecallProject(PostSimpleWorkflowData postData)
        {
            ApiProxy.SetCookies(Request.RequestUri.Host, HttpContext.Current.Request.Cookies);
            var rootPath = string.Concat(HttpContext.Current.Request.Url.Scheme, @"://", HttpContext.Current.Request.Url.Authority, HttpContext.Current.Request.ApplicationPath);
            switch (postData.FlowCode)
            {
                #region Closure
                case FlowCode.Closure_WOCheckList:
                    {
                        var entity = ClosureWOCheckList.Get(postData.ProjectId);
                        entity.Comments = postData.ProjectComment;
                        var postDataBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entity));
                        ApiProxy.Call(rootPath + "api/ClosureWOCheckList/Recall", "POST", null, postDataBytes);
                    }
                    break;
                case FlowCode.Closure_LegalReview:
                    {
                        var entity = ClosureLegalReview.Get(postData.ProjectId);
                        entity.Comments = postData.ProjectComment;
                        var postDataBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entity));
                        ApiProxy.Call(rootPath + "api/LegalReview/Recall", "POST", null, postDataBytes);
                    }
                    break;
                case FlowCode.Closure_ClosurePackage:
                    {
                        var entity = ClosurePackage.Get(postData.ProjectId);
                        entity.Comments = postData.ProjectComment;
                        var postDataBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entity));
                        ApiProxy.Call(rootPath + "api/ClosurePackage/Recall", "POST", null, postDataBytes);
                    }
                    break;
                case FlowCode.Closure_ConsInvtChecking:
                    {
                        var entity = ClosureConsInvtChecking.Get(postData.ProjectId);
                        entity.Comments = postData.ProjectComment;
                        var postDataBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entity));
                        ApiProxy.Call(rootPath + "api/ClosureConsInvtChecking/Recall", "POST", null, postDataBytes);
                    }
                    break;
                case FlowCode.Closure_ClosureTool:
                    {
                        var entity = ClosureTool.Get(postData.ProjectId);
                        entity.Comments = postData.ProjectComment;
                        var postDataBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(entity));
                        ApiProxy.Call(rootPath + "api/closureTool/Recall", "POST", null, postDataBytes);
                    }
                    break;
                #endregion Closure
                default:
                    var wfEntity = BaseWFEntity.GetWorkflowEntity(postData.ProjectId, postData.FlowCode);
                    wfEntity.Recall(postData.ProjectComment);
                    break;
            }
            return Ok();
        }

        [HttpPost]
        [Route("api/project/EditProject")]
        public IHttpActionResult EditProject(PostSimpleWorkflowData postData)
        {
            var wfEntity = BaseWFEntity.GetWorkflowEntity(postData.ProjectId, postData.FlowCode);
            var result = wfEntity.Edit();
            return Ok(result);
        }

        [HttpPost]
        [Route("api/project/EditMultipleProjects")]
        public IHttpActionResult EditMultipleProjects(PostEditProjects postEditProjects)
        {
            if (postEditProjects.EditProjects.Count > 0)
            {
                using (TransactionScope tranScope = new TransactionScope())
                {
                    var projectId = postEditProjects.ProjectId;
                    var parentCode = postEditProjects.EditProjects[0].ParentCode;
                    var projects = ProjectInfo.Search(pi => pi.ProjectId == projectId).ToList();
                    var usCode = projects[0].USCode;
                    var store = StoreBasicInfo.GetStorInfo(usCode);
                    var minExecuteSequence = postEditProjects.EditProjects.Min(p => p.ExecuteSequence);
                    var maxExecuteSequance = postEditProjects.EditProjects.Max(p => p.ExecuteSequence);
                    foreach (TopNavigator nav in postEditProjects.EditProjects)
                    {
                        var workflow = BaseWFEntity.GetWorkflowEntity(postEditProjects.ProjectId, nav.Code);
                        workflow.Edit();
                    }
                    string[] cancelWorkflowCodes = FlowInfo.Search(f => f.ParentCode == parentCode && f.ExecuteSequence <= maxExecuteSequance + 1).Select(f => f.Code).ToArray();
                    TaskWork.Cancel(t => cancelWorkflowCodes.Contains(t.TypeCode) && t.RefID == postEditProjects.ProjectId && t.Status != TaskWorkStatus.Finished);
                    postEditProjects.EditProjects.Where(n => n.ExecuteSequence == minExecuteSequence).ToList().ForEach(n =>
                    {
                        var project = projects.FirstOrDefault(pi => pi.FlowCode == n.Code);
                        var originator = Employee.GetSimpleEmployeeByCode(project.CreateUserAccount);
                        var source = FlowInfo.Get(n.ParentCode);
                        var taskType = FlowInfo.Get(n.Code);
                        TaskWork task = new TaskWork();
                        task.Id = Guid.NewGuid();
                        task.SourceCode = source.Code;
                        task.SourceNameENUS = source.NameENUS;
                        task.SourceNameZHCN = source.NameZHCN;
                        task.Status = TaskWorkStatus.UnFinish;
                        task.StatusNameZHCN = "任务";
                        task.StatusNameENUS = "任务";
                        task.Title = TaskWork.BuildTitle(postEditProjects.ProjectId, store.NameZHCN, store.NameENUS);
                        task.RefID = postEditProjects.ProjectId;
                        task.StoreCode = usCode;
                        task.ReceiverAccount = originator.Code;
                        task.ReceiverNameENUS = originator.NameENUS;
                        task.ReceiverNameZHCN = originator.NameZHCN;
                        task.TypeCode = taskType.Code;
                        task.TypeNameENUS = taskType.NameENUS;
                        task.TypeNameZHCN = taskType.NameZHCN;
                        task.Url = TaskWork.BuildUrl(n.Code, projectId, "");
                        task.CreateTime = DateTime.Now;
                        task.CreateUserAccount = ClientCookie.UserCode;
                        task.Sequence = 0;
                        task.ActivityName = "Start";
                        task.Add();

                    });
                    tranScope.Complete();
                }
                return Ok();
            }
            else
            {
                return BadRequest("you must select projects to be edited！");
            }
        }

        [HttpPost]
        [Route("api/project/ChangeProjectTeamMembers")]
        public IHttpActionResult ChangeProjectTeamMembers(ProjectDto project)
        {
            var wfEntity = BaseWFEntity.GetWorkflowEntity(project.ProjectId, project.FlowCode);
            wfEntity.ChangeProjectTeamMembers(project.ProjectId, project.ProjectTeamMembers);

            return Ok();
        }

        [HttpPost]
        [Route("api/project/ChangeWorkflowApprovers")]
        public IHttpActionResult ChangeWorkflowApprovers(ProjectDto project)
        {
            try
            {
                var wfEntity = BaseWFEntity.GetWorkflowEntity(project.ProjectId, project.FlowCode);
                var newTaskList = wfEntity.ChangeWorkflowApprovers(project.ProjectId, project.ApproveUsers);
                if (newTaskList.Count > 0)
                    SendEmail(newTaskList);
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return Ok();
        }


        private void SendEmail(List<TaskWork> taskList)
        {
            foreach (TaskWork task in taskList)
            {
                if (task == null)
                    return;

                var action = Mcdonalds.AM.Services.Common.ActionLogType.None;
                var notificationModule = new Mcdonalds.AM.Services.Common.MailHelper.NotificationModule();
                var emailMessage = MailHelper.BuildEmailMessage(task, ref action, ref notificationModule);
                if (emailMessage == null)
                    return;
                EmailServiceReference.EmailSendingResultType result = null;
                if (action == Mcdonalds.AM.Services.Common.ActionLogType.Approve)
                    result = MailHelper.SendApprovalEmail(emailMessage);
                else
                    result = MailHelper.SendCommentsEmail(emailMessage);
            }
        }

        [HttpPost]
        [Route("api/project/ChangeProjectStatus")]
        public IHttpActionResult ChangeProjectStatus(ProjectDto project)
        {
            //var wfEntity = BaseWFEntity.GetWorkflowEntity(project.ProjectId, project.FlowCode);
            //wfEntity.ChangeWorkflowApprovers(project.ProjectId, project.ApproveUsers);
            var projectInfo = new ProjectInfo();
            projectInfo.ChangeProjectStatus(project);
            return Ok();
        }

        [HttpGet]
        [Route("api/project/GetProjectStatusChangeLog")]
        public IHttpActionResult GetProjectStatusChangeLog(string projectId)
        {
            return Ok(new
            {
                changeLog = ProjectStatusChangeLog.Search(i => i.ProjectId == projectId).ToList()
            });
        }

        //public override void Finish(TaskWorkStatus status, TaskWork task)
        //{
        //    using (var scope = new TransactionScope())
        //    {
        //        switch (status)
        //        {
        //            case TaskWorkStatus.K2ProcessDeclined:
        //                ProjectInfo.UpdateProjectStatus(ProjectId, FlowCode.Reimage, ProjectStatus.Rejected);
        //                ProjectInfo.UpdateProjectStatus(ProjectId, FlowCode.Reimage_GBMemo, ProjectStatus.Rejected);
        //                break;

        //        }

        //        scope.Complete();
        //    }

        //}

        [Route("api/project/isK2Finished/{procInstId}/{typeCode}")]
        [HttpGet]
        public IHttpActionResult IsFinished(int procInstId, string typeCode)
        {
            var userCode = ClientCookie.UserCode;
            var result = TaskWork.IsK2Finished(procInstId, typeCode, userCode);
            return Ok(result);
        }

        [Route("api/project/isFlowFinished/{projectId}/{flowCode}")]
        [HttpGet]
        public IHttpActionResult IsFlowFinished(string projectId, string flowCode)
        {
            var projectInfo = ProjectInfo.Get(projectId, flowCode);
            if (projectInfo != null && projectInfo.NodeCode == NodeCode.Finish)
                return Ok(true);
            else
                return Ok(false);
        }

        [Route("api/project/isFlowNodeFinished/{projectId}/{flowCode}/{nodeCode}")]
        [HttpGet]
        public IHttpActionResult IsFlowNodeFinished(string projectId, string flowCode, string nodeCode)
        {
            var currentNode = NodeInfo.GetCurrentNode(projectId, flowCode);
            var newNode = NodeInfo.GetNodeInfo(nodeCode);
            if (newNode.Sequence <= currentNode.Sequence)
                return Ok(true);
            else
                return Ok(false);
        }

        [Route("api/project/isFlowSavable/{projectId}/{flowCode}")]
        [HttpGet]
        public IHttpActionResult IsFlowSavable(string projectId, string flowCode)
        {
            return Ok(new
            {
                Savable = ProjectInfo.IsFlowSavable(projectId, flowCode)
            });
        }

        [Route("api/project/IsShowSave")]
        [HttpGet]
        public IHttpActionResult IsShowSave(string projectId, string flowCode)
        {
            return Ok(new
            {
                issShowSave = ProjectInfo.IsFlowSavable(projectId, flowCode)
            });
        }

        [Route("api/project/isFinished/{projectId}/{flowCode}")]
        [HttpGet]
        public IHttpActionResult IsFinished(string projectId, string flowCode)
        {
            IHttpActionResult result;
            var currentNode = NodeInfo.GetCurrentNode(projectId, flowCode);
            if (currentNode != null)
            {
                bool isFinish;
                if (flowCode.Contains("ConsInvtChecking"))//对账流程不需要判断package流程是否已启动
                {
                    isFinish = currentNode.Code == NodeCode.Finish;
                }
                else
                {
                    var isPackgeStarted = ProjectInfo.IsFlowStarted(projectId, FlowCode.Closure_ClosurePackage);
                    isFinish = (currentNode.Code == NodeCode.Finish) && !isPackgeStarted;
                }
                result = Ok(isFinish);
            }
            else
            {
                result = NotFound();
            }
            return result;
        }

        [Route("api/project/EnableRecall/{refTableName}/{refId}/{projectId}")]
        [HttpGet]
        public IHttpActionResult EnableRecall(string refTableName, Guid refId, string projectId)
        {
            bool isFinish = ProjectInfo.EnableRecall(refTableName, refId);
            if (!refTableName.Contains("ConsInvtChecking"))//对账流程不需要判断package流程是否已启动
            {
                var isPackgeStarted = ProjectInfo.IsFlowStarted(projectId, FlowCode.Closure_ClosurePackage);
                isFinish = ProjectInfo.EnableRecall(refTableName, refId) && !isPackgeStarted;
            }
            return Ok(isFinish);
        }

        [Route("api/project/GetHistory/{projectId}/{tableName}/{hasTemplate}")]
        public IHttpActionResult GetHistory(string projectId, string tableName, bool hasTemplate)
        {
            var result = ProcedureManager.Proc_GetProjectHistory(projectId, tableName, hasTemplate);
            return Ok(result);
        }


        #region reimage summary package holding feature

        [Route("api/project/ChangePackageHoldingStatus")]
        public IHttpActionResult ChangePackageHoldingStatus(PackageHoldingDto packageHoldingDto)
        {
            try
            {
                var wfEntity = BaseWFEntity.GetWorkflowEntity(packageHoldingDto.ProjectId, packageHoldingDto.HoldingPackageCode);
                wfEntity.ChangePackageHoldingStatus(packageHoldingDto.Status);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(packageHoldingDto);
        }
        #endregion

        [Route("api/project/OutputExcel")]
        [HttpPost]
        public IHttpActionResult OutputExcel(ProjectSearchCondition condition)
        {
            int totalItems = 0;
            var list = VProject.Search(-1, -1, condition, ref totalItems);

            var current = System.Web.HttpContext.Current;
            var fileName = "AM_Project_List_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx";
            string tempFilePath = current.Server.MapPath("~/") + "Temp\\" + fileName;
            ExcelDataInputDirector.SaveToExcel<VProject>(list, tempFilePath);

            //current.Response.AppendHeader("Content-Disposition", "attachment;filename=" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xlsx");
            //current.Response.ContentType = "application/octet-stream";
            //current.Response.WriteFile("" + tempFilePath + "");
            //current.Response.End();
            return Ok(new
            {
                fileName = fileName
            });
        }
    }
}
