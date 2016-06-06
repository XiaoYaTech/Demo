using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ProjectUsers : BaseEntity<ProjectUsers>
    {
        public static bool IsRole(string projectId, string userAccount, string roleCode)
        {
            return Any(pu => pu.ProjectId == projectId && pu.UserAccount == userAccount && pu.RoleCode == roleCode);
        }
        public List<ProjectTeamMember> GetProjectUsers(string projectId, string storeCode, string roleCode)
        {
            List<ProjectTeamMember> items = new List<ProjectTeamMember>();
            if (string.IsNullOrEmpty(projectId))
            {
                RoleCode rc;
                if (Enum.TryParse(roleCode, out rc))
                {
                    items = Employee.GetStoreEmployeesByRole(storeCode, rc).Select(e => new ProjectTeamMember
                    {
                        UserAccount = e.Code,
                        UserNameZHCN = e.NameZHCN,
                        UserNameENUS = e.NameENUS,
                        RoleCode = Employee.GetRoleName(rc),
                        RoleNameENUS = e.PositionENUS,
                        RoleNameZHCN = e.PositionZHCN
                    }).ToList();
                    if (items.Count == 1) items[0].IsSelected = true;
                }
            }
            else
            {
                var projectRoleCode = roleCode.Replace('_', ' ');
                var selectedItem = FirstOrDefault(p => p.ProjectId.Equals(projectId) && p.RoleCode.Equals(projectRoleCode));
                if (selectedItem == null)
                    selectedItem = FirstOrDefault(p => p.ProjectId.Equals(projectId) && p.RoleCode.Equals("Finance Team"));
                RoleCode rc;
                if (Enum.TryParse(roleCode, out rc))
                {
                    items = Employee.GetStoreEmployeesByRole(storeCode, rc).Select(e => new ProjectTeamMember
                    {
                        UserAccount = e.Code,
                        UserNameZHCN = e.NameZHCN,
                        UserNameENUS = e.NameENUS,
                        RoleCode = Employee.GetRoleName(rc),
                        RoleNameENUS = e.PositionENUS,
                        RoleNameZHCN = e.PositionZHCN,
                        IsSelected = selectedItem == null ? false : (e.Code == selectedItem.UserAccount)
                    }).ToList();
                }
            }
            return items.ToList();
        }

        public List<ProjectTeamMember> GetProjctActorsByProjectId(string projectId, string storeCode, string roleCode, string currentUserCode)
        {
            List<ProjectTeamMember> items = new List<ProjectTeamMember>();
            Employee bllEmployee = new Employee();
            var projectUser = FirstOrDefault(p => p.ProjectId.Equals(projectId) && p.RoleCode.Equals(roleCode));
            items = Employee.GetStoreAssetActorsByStoreCode(storeCode, projectUser.UserAccount).Select(e => new ProjectTeamMember
            {
                UserAccount = e.Code,
                UserNameZHCN = e.NameZHCN,
                UserNameENUS = e.NameENUS,
                RoleCode = roleCode,
                RoleNameENUS = e.PositionENUS,
                RoleNameZHCN = e.PositionZHCN,
                IsSelected = e.Code == projectUser.UserAccount
            }).ToList();
            return items;
        }

        public List<ProjectTeamMember> GetProjctRepsByProjectId(string projectId, string storeCode, string roleCode)
        {
            List<ProjectTeamMember> items = new List<ProjectTeamMember>();
            var projectUser = FirstOrDefault(p => p.ProjectId.Equals(projectId) && p.RoleCode.Equals(roleCode));
            items = Employee.GetStoreAssetRepByStoreCode(storeCode).Select(e => new ProjectTeamMember
            {
                UserAccount = e.Code,
                UserNameZHCN = e.NameZHCN,
                UserNameENUS = e.NameENUS,
                RoleCode = roleCode,
                RoleNameENUS = e.PositionENUS,
                RoleNameZHCN = e.PositionZHCN,
                IsSelected = e.Code == projectUser.UserAccount
            }).ToList();

            return items;
        }

        public List<ProjectTeamMember> GetProjctMgrByProjectId(string projectId, string storeCode, string roleCode)
        {
            List<ProjectTeamMember> items = new List<ProjectTeamMember>();
            var projectUser = FirstOrDefault(p => p.ProjectId.Equals(projectId) && p.RoleCode.Equals(roleCode));
            items = Employee.GetStoreAssetRepMgrByStoreCode(storeCode).Select(e => new ProjectTeamMember
            {
                UserAccount = e.Code,
                UserNameZHCN = e.NameZHCN,
                UserNameENUS = e.NameENUS,
                RoleCode = roleCode,
                RoleNameENUS = e.PositionENUS,
                RoleNameZHCN = e.PositionZHCN,
                IsSelected = projectUser == null ? false : (e.Code == projectUser.UserAccount)
            }).ToList();

            return items;
        }

        public static ProjectUsers GetProjectUser(string projectId, string roleCode)
        {
            return FirstOrDefault(pu => pu.ProjectId == projectId && pu.RoleCode == roleCode);
        }
        /// <summary>
        /// 获取审批用户的Title信息
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="projectId"></param>
        /// <returns></returns>
        public ProjectUsers GetEntity(string userAccount, string projectId)
        {
            var bllVStorePosit = new V_StorePostionRelation();
            var _projUsers = Get(userAccount, projectId);
            if (_projUsers != null && !string.IsNullOrEmpty(_projUsers.RoleCode))
            {
                _projUsers.RoleCode = _projUsers.RoleCode;
                _projUsers.RoleNameENUS = _projUsers.RoleNameENUS;
                _projUsers.RoleNameZHCN = _projUsers.RoleNameZHCN;
            }
            else
            {
                //_projUsers = new ProjectUsers();
                //var __userStorePos = bllVStorePosit.QueryStorePositionByProjectID(projectId, userAccount).FirstOrDefault();
                //if (__userStorePos != null && !string.IsNullOrEmpty(__userStorePos.PositionCode))
                //{
                //    _projUsers.RoleCode = __userStorePos.PositionCode;
                //    _projUsers.RoleNameENUS = __userStorePos.PositionNameENUS;
                //    _projUsers.RoleNameZHCN = __userStorePos.PositionNameZHCN;
                //}
            }
            return _projUsers;
        }

        public static ProjectUsers Get(string userAccount, string projectId)
        {
            return Search(e => e.UserAccount == userAccount && e.ProjectId == projectId).FirstOrDefault();
        }

        public static string GetUserRoleCode(string userAccount, string projectId)
        {
            var userRole = Get(userAccount, projectId);
            return userRole == null ? "" : userRole.RoleCode;
        }

        public IEnumerable<ProjectUsers> GetProjectUserList(string userAccount, string projectId)
        {
            return Search(e => e.UserAccount == userAccount && e.ProjectId == projectId);
        }

        public static ProjectUsers Get(string roleNameENUS, string roleNameZHCN, string roleCode, string userAccount, string userNameENUS, string userNAMEZHCN, string projectId)
        {
            var closureUsers = new ProjectUsers();
            closureUsers.Id = Guid.NewGuid();
            closureUsers.RoleNameENUS = roleNameENUS;
            closureUsers.RoleNameZHCN = roleNameZHCN;
            closureUsers.RoleCode = roleCode;
            closureUsers.UserAccount = userAccount;
            closureUsers.UserNameENUS = userNameENUS;
            closureUsers.UserNameZHCN = userNAMEZHCN;

            closureUsers.Sequence = 0;
            closureUsers.ProjectId = projectId;
            closureUsers.CreateDate = DateTime.Now;
            return closureUsers;
        }

        public string TaskStatus { get; set; }

    }
}
