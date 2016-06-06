using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Mcdonalds.AM.Services.Controllers
{
    public class FlowInfoController : ApiController
    {
        [Route("api/flow/topnav")]
        [HttpGet]
        public IHttpActionResult TopNav(string projectId, string flowCode, string subCode)
        {
            var tasks = TaskWork.Search(t => t.ReceiverAccount == ClientCookie.UserCode && t.RefID == projectId && t.Status == 0).ToList();
            var projects = ProjectInfo.Search(p => p.ProjectId == projectId);
            var navs = FlowInfo.Search(f => f.ParentCode == flowCode).OrderBy(f => f.LayoutSequence).ToList().Select(f =>
            {
                var task = tasks.FirstOrDefault(t => t.TypeCode == f.Code);
                var project = projects.FirstOrDefault(p => p.FlowCode == f.Code) ?? new ProjectInfo();
                var nav = new TopNavigator();
                if (f.Code == subCode)
                {
                    nav.IsSelected = true;
                }
                else
                {
                    nav.IsSelected = false;
                }
                nav.Code = f.Code;
                nav.ParentCode = f.ParentCode;
                nav.NameENUS = f.NameENUS;
                nav.NameZHCN = f.NameZHCN;
                nav.ExecuteSequence = f.ExecuteSequence;
                nav.Percentage = f.Percentage;
                nav.IsFinished = project.Status == ProjectStatus.Finished;
                nav.Status = project.Status;
                nav.IsRejected = project.Status == ProjectStatus.Rejected;
                if (task != null)
                {
                    if (task.Url.IndexOf(task.RefID, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        if (task.Url.IndexOf("?") >= 0)
                        {
                            task.Url += "&";
                        }
                        else
                        {
                            task.Url += "?";
                        }
                        task.Url += "projectId=" + task.RefID;
                    }
                    nav.Url = task.Url;
                }
                else if (f.NoTaskEditable && project.CreateUserAccount == ClientCookie.UserCode)
                {
                    nav.Url = string.Format("/{0}/Main#/{1}?projectId={2}", flowCode, f.Code.Split('_')[1], projectId);
                }
                else
                {
                    var tmpCode = f.Code.Split('_')[1];
                    if (flowCode == "Rebuild" && tmpCode == "Package")
                    {
                        tmpCode = "RebuildPackage";
                    }
                    nav.Url = string.Format("/{0}/Main#/{1}/Process/View?projectId={2}", flowCode, tmpCode, projectId);
                }
                return nav;
            }).ToList();
            navs.Insert(0, new TopNavigator
            {
                NameENUS = "Project Detail",
                NameZHCN = "Project Detail",
                Code = "Detail",
                Url = "/Home/Main#/project/detail/" + projectId + "?flowCode=" + flowCode,
                IsSelected = subCode == "Detail"
            });
            return Ok(navs);
        }

        [Route("api/flow/nodes")]
        [HttpGet]
        public IHttpActionResult GetNodes(string projectId, string flowCode)
        {
            var tasks = TaskWork.Search(t => t.ReceiverAccount == ClientCookie.UserCode && t.RefID == projectId && t.Status == 0).ToList();
            var isActor = ProjectUsers.IsRole(projectId, ClientCookie.UserCode, ProjectUserRoleCode.AssetActor);
            var projects = ProjectInfo.Search(p => p.ProjectId == projectId);
            var flows = FlowInfo.Search(f => f.ParentCode == flowCode).OrderBy(f => f.LayoutSequence).ToList();

            var editApproverableCodes = new string[]{
                FlowCode.Closure_ClosurePackage,
                FlowCode.TempClosure_ClosurePackage,
                FlowCode.MajorLease_Package,
                FlowCode.Reimage_Package,
                FlowCode.Rebuild_Package,
                FlowCode.Renewal_Package
            };

            var navs = flows.Select(f =>
            {
                var task = tasks.FirstOrDefault(t => t.TypeCode == f.Code);
                var project = projects.FirstOrDefault(p => p.FlowCode == f.Code);
                var node = NodeInfo.FirstOrDefault(n => n.Code == project.NodeCode);
                var nextFlow = flows.FirstOrDefault(nf => nf.ExecuteSequence == f.ExecuteSequence + 1);
                var isOriginator = ProjectInfo.IsOriginator(projectId, f.Code, ClientCookie.UserCode);
                var canEdit = project.Status == ProjectStatus.Finished || project.Status == ProjectStatus.Rejected;
                var flowStarted = node.Code != NodeCode.Start;
                var lastStartTask = TaskWork.Search(t => t.RefID == projectId && t.TypeCode == f.Code && (t.ActivityName == "Start" || t.ActivityName == "Originator")).OrderByDescending(t => t.CreateTime).FirstOrDefault();
                var nextFlowStarted = nextFlow != null && projects.Any(p => p.FlowCode == nextFlow.Code && p.NodeCode != NodeCode.Start);
                var nav = new TopNavigator();
                nav.Code = f.Code;
                nav.ParentCode = f.ParentCode;
                nav.NameENUS = f.NameENUS;
                nav.NameZHCN = f.NameZHCN;
                nav.ExecuteSequence = f.ExecuteSequence;
                nav.Percentage = f.Percentage;
                nav.IsFinished = project.Status == ProjectStatus.Finished;
                nav.Status = project.Status;
                nav.IsRejected = project.Status == ProjectStatus.Rejected;
                nav.ProgressRate = node.ProgressRate;
                if (task != null)
                {
                    if (task.Url.IndexOf(task.RefID, StringComparison.OrdinalIgnoreCase) < 0)
                    {
                        if (task.Url.IndexOf("?") >= 0)
                        {
                            task.Url += "&";
                        }
                        else
                        {
                            task.Url += "?";
                        }
                        task.Url += "projectId=" + task.RefID;
                    }
                    nav.Url = task.Url;
                }
                else if (f.NoTaskEditable && isActor)
                {
                    nav.Url = string.Format("/{0}/Main#/{1}?projectId={2}", flowCode, f.Code.Split('_')[1], projectId);
                }
                else
                {
                    var tmpCode = f.Code.Split('_')[1];
                    if (flowCode == "Rebuild" && tmpCode == "Package")
                    {
                        tmpCode = "RebuildPackage";
                    }
                    nav.Url = string.Format("/{0}/Main#/{1}/Process/View?projectId={2}", flowCode, tmpCode, projectId);
                }
                if (projectId.ToLower().IndexOf("renewal") != -1)
                    nav.Editable = f.Editable && canEdit && !nextFlowStarted && ProjectInfo.IsFlowEditable(projectId, f.Code);
                else
                    nav.Editable = f.Editable && canEdit && ProjectInfo.IsFlowEditable(projectId, f.Code);
                nav.Recallable = f.Recallable && !canEdit && isOriginator && ((lastStartTask == null && TaskWork.Any(t => t.RefID == projectId && t.TypeCode == f.Code && t.ActivityName != "Start")) || (lastStartTask != null && TaskWork.Any(t => t.CreateTime >= lastStartTask.CreateTime && t.RefID == projectId && t.TypeCode == f.Code && t.ActivityName != "Start"))) && project.NodeCode != NodeCode.Start;
                nav.EditApproverable = !canEdit && isOriginator && editApproverableCodes.Contains(nav.Code) && ((lastStartTask == null && TaskWork.Any(t => t.RefID == projectId && t.TypeCode == f.Code && (t.ActivityName != "Start" && t.ActivityName != "Originator"))) || (lastStartTask != null && TaskWork.Any(t => t.CreateTime >= lastStartTask.CreateTime && t.RefID == projectId && t.TypeCode == f.Code && (t.ActivityName != "Start" && t.ActivityName != "Originator"))));
                return nav;
            }).ToList();
            return Ok(navs);
        }

        [Route("api/flow/GetFlowInfo")]
        [HttpGet]
        public IHttpActionResult GetFlowInfo(string projectId, string flowCode)
        {
            var wfEntity = BaseWFEntity.GetWorkflowEntity(projectId, flowCode);
            return Ok(wfEntity);
        }
    }
}
