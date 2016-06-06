using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.Entities.Closure.Enum;
using Mcdonalds.AM.Services.Common;
using Mcdonalds.AM.Services.Infrastructure;
using MyExcel.NPOI;
using MyExcel.Services;
using NPOI.HSSF.UserModel;
using NPOI.SS.Formula.Functions;
using NPOI.SS.UserModel;
using System.Data.Entity;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Infrastructure;
using Mcdonalds.AM.DataAccess.Entities;

namespace Mcdonalds.AM.Services.Controllers
{
    public class ProjectUsersClosureController : ApiController
    {
        public class ClosureNodeInfo
        {
            public static string WOCheckListCode = "WOCheckList";
            public static string ClosureToolCode = "ClosureTool";
            public static string LegalReviewCode = "LegalReview";
        }

        public class ClosureRoleInfo
        {
            public static string PM = "PM";
            public static string AssetActor = "AssetActor";
        }

        /// <summary>
        /// 是否可以编辑
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>

        [Route("api/ProjectUsers/IsEditer/{projectId}/{nodeCode}")]
        [HttpGet]
        public IHttpActionResult IsEditer(string projectId, string nodeCode)
        {
            bool result = false;
            string nodeCodeStr = nodeCode.Trim().ToLower();
            //获取用户的关店流程权限
            var userRoles = ProjectUsers.Search(e => e.ProjectId == projectId &&
                                                        e.UserAccount == ClientCookie.UserCode).ToList();
            if (userRoles.Count > 0)
            {
                if (nodeCodeStr == ClosureNodeInfo.WOCheckListCode.ToLower())
                {
                    result = CheckEditer(userRoles, ClosureRoleInfo.PM);
                }
                else if (nodeCodeStr == ClosureNodeInfo.LegalReviewCode.ToLower())
                {
                    result = CheckEditer(userRoles, ClosureRoleInfo.AssetActor);
                }
            }

            return Ok(result);
        }



        [Route("api/projectusers/role")]
        [HttpGet]
        public string GetUserRoleInProject(string userCode, string projectId)
        {
            return ProjectUsers.FirstOrDefault(pu => pu.UserAccount == userCode && pu.ProjectId == projectId).RoleCode;
        }

        [Route("api/projectusers/canInnerEdit")]
        [HttpGet]
        public IHttpActionResult CanInnerEdit(string projectId)
        {
            var canInnerEditRoleCodeList = new List<string>
            {
                ProjectUserRoleCode.AssetActor,
                ProjectUserRoleCode.AssetRep,
                ProjectUserRoleCode.AssetManager
            };
            var result = ProjectUsers.Any(pu => pu.ProjectId == projectId
                && pu.UserAccount == ClientCookie.UserCode
                && canInnerEditRoleCodeList.Contains(pu.RoleCode));
            if (result)
            {
                var project = VProject.Search(i => i.ProjectId == projectId).FirstOrDefault();
                if (project != null)
                {
                    switch (project.FlowCode)
                    {
                        case FlowCode.Closure:
                            result = !ProjectInfo.IsFlowStarted(projectId, FlowCode.Closure_ClosurePackage);
                            break;
                        case FlowCode.Renewal:
                            result = !ProjectInfo.IsFlowStarted(projectId, FlowCode.Renewal_Package);
                            break;
                    }
                }
            }
            return Ok(result);
        }

        [Route("api/projectusers/team")]
        [HttpGet]
        public IHttpActionResult GetTeamUsers(string storeCode, string projectId = "")
        {
            var bll = new ProjectUsers();
            var bllEmployee = new Employee();
            List<ProjectTeamMember> assetReps = new List<ProjectTeamMember>();
            List<ProjectTeamMember> assetActors = new List<ProjectTeamMember>();
            List<ProjectTeamMember> assetMgrs = new List<ProjectTeamMember>();
            var currentUser = Employee.GetSimpleEmployeeByCode(ClientCookie.UserCode);
            if (string.IsNullOrEmpty(projectId))
            {
                assetReps = bllEmployee.GetAssetRepsByStoreCode(storeCode, currentUser.Code, ProjectUserRoleCode.AssetRep);
                assetActors = bllEmployee.GetAssetActorByStoreCode(storeCode, currentUser.Code, ProjectUserRoleCode.AssetActor);
                assetMgrs = Employee.GetAssetRepMgrByStoreCode(storeCode);
            }
            else
            {
                assetReps = bll.GetProjctRepsByProjectId(projectId, storeCode, ProjectUserRoleCode.AssetRep);
                assetActors = bll.GetProjctActorsByProjectId(projectId, storeCode, ProjectUserRoleCode.AssetActor, currentUser.Code);
                assetMgrs = bll.GetProjctMgrByProjectId(projectId, storeCode, ProjectUserRoleCode.AssetManager);
            }
            return Ok(new
            {
                AssetReps = assetReps,
                AssetActors = assetActors,
                PMs = bll.GetProjectUsers(projectId, storeCode, RoleCode.PM.ToString()),
                Finances = bll.GetProjectUsers(projectId, storeCode, RoleCode.Finance_Consultant.ToString()),
                Legals = bll.GetProjectUsers(projectId, storeCode, RoleCode.Legal_Counsel.ToString()),
                AssetMgrs = assetMgrs,
                CMs = bll.GetProjectUsers(projectId, storeCode, RoleCode.Cons_Mgr.ToString())
            });
        }

        [Route("api/projectusers/save")]
        [HttpGet]
        public IHttpActionResult SaveTeamUsers(PostProjectTeam team)
        {
            ProjectUsers.Update(new ProjectUsers[] { 
                team.AssetActor,
                team.AssetRep,
                team.Finance,
                team.Legal,
                team.PM
            });
            return Ok();
        }

        [Route("api/projectusers/notice")]
        [HttpGet]
        public IHttpActionResult GetNoticeUsers(string storeCode, string projectId = "")
        {
            var bll = new ProjectUsers();
            return Ok(new
            {

                AssetMgrs = Employee.GetAssetRepMgrByStoreCode(storeCode),//bll.GetProjectUsers(projectId, storeCode, RoleCode.Market_Asset_Mgr.ToString()),
                CMs = bll.GetProjectUsers(projectId, storeCode, RoleCode.Cons_Mgr.ToString())
            });
        }

        [Route("api/projectusers/viewers")]
        public IHttpActionResult GetViewers(string projectId)
        {
            var results = ProjectUsers.Search(pu => pu.ProjectId == projectId && pu.RoleCode == ProjectUserRoleCode.View).ToList();
            return Ok(results);
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="projectId">项目Id</param>
        /// <param name="userAccount">用户账号</param>
        /// <param name="roleCode">角色编号</param>
        /// <returns></returns>
        [Route("api/projectusers/IsExists/{projectId}/{userAccount}/{roleCode}")]
        [HttpGet]
        public IHttpActionResult IsExists(string projectId, string userAccount, string roleCode)
        {
            var list = ProjectUsers.Search(e => e.ProjectId == projectId && e.UserAccount == userAccount && e.RoleCode == roleCode).ToList();
            var result = list.Count > 0;
            return Ok(result);
        }

        private bool CheckEditer(List<ProjectUsers> list, string roleCode)
        {
            var result = list.FirstOrDefault(e => e.RoleCode == roleCode);
            return result != null;
        }

        [Route("api/ProjectUsers/GetCurrentByProjectId/{projectId}/{flowCode}/{subFlowCode}/{userAccount}")]
        [HttpGet]
        public List<NavigateInfo> GetCurrentByProjectId(string projectId, string flowCode, string subFlowCode, string userAccount)
        {
            var flowList = FlowInfo.Search(e => e.ParentCode == FlowCode.Closure).OrderBy(e => e.LayoutSequence);

            //获取当前用户的项目任务
            var sql = string.Format(@"SELECT tb_users.*,tb_work.Status AS TaskStatus,tb_work.TypeCode,tb_work.Url,
            tb_work.ProcInstID,tb_users.ProjectId
            FROM dbo.ProjectUsers tb_users
            RIGHT JOIN dbo.TaskWork tb_work
            ON tb_users.UserAccount = tb_work.ReceiverAccount
            AND tb_work.RefID = tb_users.ProjectId
            WHERE tb_work.RefID = '{0}' AND tb_work.ReceiverAccount ='{1}'
            AND ISNULL(RoleCode,'')!='View' AND tb_work.Status = 0  ", projectId, userAccount);

            var navigateList = new List<NavigateInfo>();

            var result = ProjectUsers.SqlQuery<ProjectUsersEntity>(sql, null);
            var list = result.ToList();





            //获取已完成的项目列表
            var finishedProjectList = ProjectInfo.Search(e => e.ProjectId == projectId && e.Status == ProjectStatus.Finished).ToList();

            NavigateInfo navigateItem = null;
            var viewPageStuff = "/View/param?projectId=" + projectId;
            var editPageStuff = "/" + projectId;
            //是否是编辑页

            foreach (var flowItem in flowList)
            {
                var isEditPage = false;
                navigateItem = new NavigateInfo();
                navigateItem.NameZHCN = flowItem.NameZHCN;
                navigateItem.Linked = true;
                navigateItem.Code = flowItem.Code;
                navigateItem.Href = string.Empty;

                //设置当前的子流程的为选中状态
                if (flowItem.Code == subFlowCode)
                {
                    navigateItem.IsSelected = true;
                }

                //设置流程的完成状态
                foreach (var project in finishedProjectList)
                {
                    if (flowItem.Code == project.FlowCode)
                    {
                        navigateItem.IsFinished = true;
                    }
                }


                //默认的链接 主流程Code+/+子流程Code
                string defaultUrl = "#/" + flowItem.FlowCodePrefix + "/" + flowItem.Code.Split('_')[1];

                //判断当前用户是否有待办任务
                foreach (var item in list)
                {
                    if (item.TypeCode == flowItem.Code)
                    {

                        if (item.TaskStatus == TaskWorkStatus.UnFinish)
                        {

                            //如果有待办任务导航tab的链接为待办任务的链接
                            navigateItem.Href = item.Url;
                            if (item.ProcInstID.HasValue && !string.IsNullOrEmpty(item.ProjectId))
                            {
                                navigateItem.Href += "&projectId=" + item.ProjectId;
                            }

                        }
                        //如果任务已结束设置为只读页面的链接
                        else if (item.TaskStatus == TaskWorkStatus.Finished ||
                                 item.TaskStatus == TaskWorkStatus.K2ProcessApproved || navigateItem.IsFinished)
                        {
                            navigateItem.Href = defaultUrl + viewPageStuff;
                        }


                    }
                }


                //获取当前用户的项目组头衔

                var projectUser = ProjectUsers.Get(ClientCookie.UserCode, projectId);

                if (string.IsNullOrEmpty(navigateItem.Href))
                {
                    if (projectUser != null)
                    {


                        if (!string.IsNullOrEmpty(flowItem.RoleCode) &&
                   flowItem.RoleCode.Split('_').Contains(projectUser.RoleCode) && !navigateItem.IsFinished)
                        {
                            isEditPage = true;
                        }

                        if (isEditPage)
                        {
                            navigateItem.Href = defaultUrl + editPageStuff;
                        }
                        else
                        {
                            navigateItem.Href = defaultUrl + viewPageStuff;
                        }
                    }
                    else
                    {
                        navigateItem.Href = defaultUrl + viewPageStuff;
                    }

                }


                navigateList.Add(navigateItem);
            }


            return navigateList;
        }

        [Route("api/projectusers/role/exist")]
        [HttpGet]
        public bool InRole(string projectId, string userCode, string roleCode)
        {
            var result = ProjectUsers.Any(pu => pu.ProjectId == projectId && pu.UserAccount == userCode && pu.RoleCode == roleCode);
            return result;
        }


        [Route("api/projectusers/roles/get")]
        [HttpGet]
        public IHttpActionResult GetRoles(string projectId, string userCode)
        {
            var projectUsers = ProjectUsers.Search(pu => pu.ProjectId == projectId && pu.UserAccount == userCode).Select(r => r.RoleCode).ToList();
            return Ok(projectUsers);
        }

        [Route("api/projectusers/getNotifyUser")]
        [HttpGet]
        public IHttpActionResult GetNotifyUser(string usCode, string projectId, string roleCodes)
        {
            Dictionary<string, string> dictNotify = new Dictionary<string, string>();

            foreach (string roleCode in roleCodes.Split(','))
            {
                if (!string.IsNullOrEmpty(roleCode))
                {
                    if (roleCode == "Actor")
                    {
                        var actor = ProjectUsers.FirstOrDefault(pu => pu.ProjectId == projectId && pu.RoleCode == ProjectUserRoleCode.AssetActor);
                        dictNotify.Add(roleCode, actor.UserNameENUS);
                    }
                    else
                    {
                        RoleCode enumRole;
                        if (Enum.TryParse(roleCode, out enumRole))
                        {
                            var employeeList = Employee.GetStoreEmployeesByRole(usCode, enumRole);
                            dictNotify.Add(roleCode, BuildNoticeUserNameStr(employeeList));
                        }
                    }
                }
            }
            return Ok(dictNotify);
        }


        [Route("api/projectusers/GetPackageHoldingRoleUsers")]
        [HttpGet]
        public IHttpActionResult GetPackageHoldingRoleUsers()
        {
            return Ok(BaseWFEntity.GetPackageHoldingRoleUsers());
        }


        /// <summary>
        /// 获取Name的字符串
        /// </summary>
        /// <param name="simpleEmployeeList"></param>
        /// <returns></returns>
        private string BuildNoticeUserNameStr(List<SimpleEmployee> simpleEmployeeList)
        {
            string userNames = string.Empty;
            foreach (var simpleEmployee in simpleEmployeeList)
            {
                userNames += simpleEmployee.NameENUS + "; ";
            }
            return string.IsNullOrEmpty(userNames) ? "" : userNames.TrimEnd(' ').TrimEnd(';');
        }

        public class NavigateInfo
        {
            //public List<ProjectUsersEntity> closureUserList;
            //public List<TaskWork> finishedTaskList;
            public string Href { get; set; }
            public string NameZHCN { get; set; }
            public string Code { get; set; }
            public bool IsSelected { get; set; }
            public bool IsFinished { get; set; }
            public bool Linked { get; set; }
        }

        private class ProjectUsersEntity
        {
            //public System.Guid Id { get; set; }
            //public string UserAccount { get; set; }
            //public string UserName { get; set; }
            public string RoleCode { get; set; }
            //public string RoleName { get; set; }
            //public DateTime? CreateDate { get; set; }
            //public string CreateUserAccount { get; set; }
            //public string CreateUserName { get; set; }
            //public int? Sequence { get; set; }
            //public string ProjectId { get; set; }
            public string Url { get; set; }
            public TaskWorkStatus TaskStatus { get; set; }
            public string TypeCode { get; set; }
            public int? ProcInstID { get; set; }
            public string ProjectId { get; set; }
        }

    }
}