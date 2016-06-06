using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System;

namespace Mcdonalds.AM.DataAccess
{
    public partial class Employee : BaseEntity<Employee>
    {
       
        #region 根据岗位获取用户信息
        /// <summary>
        /// 根据Code获取员工对象
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static Employee GetEmployeeByCode(string code)
        {
            return FirstOrDefault(e => e.Status && e.Code.Equals(code));
        }

        /// <summary>
        /// 获取Finance Manager
        /// </summary>
        /// <returns></returns>
        public List<Employee> GetEmployeeByPosition(string positionCode)
        {
            List<Employee> fianceManagerList;
            var context = GetDb();
            var result = context.Employee.Where(e => e.Status && e.PositionENUS.Equals(positionCode) && e.Code != "");
            fianceManagerList = result.ToList();
            return fianceManagerList;
        }

        /// <summary>
        /// 根据岗位ID获取用户信息
        /// </summary>
        /// <param name="positionID"></param>
        /// <returns></returns>
        public static List<SimpleEmployee> GetSimpleEmployeeByPositionID(int positionID)
        {
            return Employee.Search(e => e.PositionID == positionID && e.Status).Select(e => new SimpleEmployee()
            {
                Code = e.Code,
                PositionCode = e.PositionCode,
                PositionENUS = e.PositionENUS,
                PositionZHCN = e.PositionZHCN,
                TitleCode = e.TitleCode,
                TitleENUS = e.TitleENUS,
                TitleZHCN = e.TitleZHCN,
                NameENUS = e.NameENUS,
                NameZHCN = e.NameZHCN,
                Mail = e.Mail
            }).AsNoTracking().ToList();
        }

        public static SimpleEmployee GetSimpleEmployeeByCode(string code)
        {
            return Employee.Search(e => e.Code == code && e.Status).Select(e => new SimpleEmployee()
            {
                Code = e.Code,
                PositionCode = e.PositionCode,
                PositionENUS = e.PositionENUS,
                PositionZHCN = e.PositionZHCN,
                TitleCode = e.TitleCode,
                TitleENUS = e.TitleENUS,
                TitleZHCN = e.TitleZHCN,
                NameENUS = e.NameENUS,
                NameZHCN = e.NameZHCN,

                Mail = e.Mail
            }).AsNoTracking().FirstOrDefault();
        }

        public static List<SimpleEmployee> GetSimpleEmployeeByCodes(string[] codes)
        {
            return Employee.Search(e => codes.Contains(e.Code) && e.Status).Select(e => new SimpleEmployee()
            {
                Code = e.Code,
                PositionCode = e.PositionCode,
                PositionENUS = e.PositionENUS,
                PositionZHCN = e.PositionZHCN,
                TitleCode = e.TitleCode,
                TitleENUS = e.TitleENUS,
                TitleZHCN = e.TitleZHCN,
                NameENUS = e.NameENUS,
                NameZHCN = e.NameZHCN,
                Mail = e.Mail
            }).AsNoTracking().ToList();
        }

        /// <summary>
        /// 根据岗位的名称获取用户-简单用户对象
        /// </summary>
        /// <param name="positionCode">Position Code是常量，直接在PositionCodeConstants中取值，如PositionCodeConstants.Asset_Mgr</param>
        /// <returns></returns>
        public static List<SimpleEmployee> GetSimpleEmployeesByPosition(string positionCode)
        {
            var db = PrepareDb();
            var items = db.Employee.Where(e => e.Status && e.PositionENUS.Equals(positionCode)).Select(e => new SimpleEmployee
            {
                Code = e.Code,
                NameENUS = e.NameENUS,
                NameZHCN = e.NameZHCN,
                TitleCode = e.TitleCode,
                TitleENUS = e.TitleENUS,
                TitleZHCN = e.TitleZHCN,
                Mail = e.Mail,
                Mobile = e.Mobile,
                Phone = e.Phone,
                PositionENUS = e.PositionENUS,
                PositionCode = e.PositionCode
            }).ToList();
            return items;
        }

        /// <summary>
        /// 根据多个岗位的名称获取用户
        /// </summary>
        /// <param name="positionCodes"></param>
        /// <returns></returns>
        public List<SimpleEmployee> GetSimpleEmployeesByPosition(params string[] positionCodes)
        {
            var employeeList = new List<SimpleEmployee>();
            foreach (var positionCode in positionCodes)
            {
                employeeList.AddRange(GetSimpleEmployeesByPosition(positionCode));
            }

            return employeeList;
        }

        /// <summary>
        /// 获取资产代表
        /// </summary>
        /// <param name="storeCode"></param>
        /// <returns></returns>
        public static List<SimpleEmployee> GetStoreAssetRepByStoreCode(string storeCode)
        {
            var db = PrepareDb();
            List<SimpleEmployee> result = new List<SimpleEmployee>();
            var storeDev = db.StoreDevelop.FirstOrDefault(s => s.StoreCode.Equals(storeCode));
            if (!string.IsNullOrEmpty(storeDev.AssetRepEid))
            {
                var assetRep = db.Employee.Where(e => e.Code.Equals(storeDev.AssetRepEid) && e.Status).Select(e => new SimpleEmployee
                {
                    Code = e.Code,
                    NameENUS = e.NameENUS,
                    NameZHCN = e.NameZHCN,
                    Mail = e.Mail,
                    Mobile = e.Mobile,
                    Phone = e.Phone,
                    PositionENUS = "Asset Rep",
                    TitleCode = e.TitleCode,
                    TitleENUS = e.TitleENUS,
                    TitleZHCN = e.TitleZHCN,
                    PositionCode = e.PositionCode
                }).SingleOrDefault();
                if (assetRep != null) result.Add(assetRep);
            }
            return result;
        }

        /// <summary>
        /// 获取操作人
        /// </summary>
        /// <param name="storeCode"></param>
        /// <returns></returns>
        public static List<SimpleEmployee> GetStoreAssetActorsByStoreCode(string storeCode, string userCode)
        {
            var db = PrepareDb();
            List<SimpleEmployee> result = new List<SimpleEmployee>();
            //var storeDev = db.StoreDevelop.FirstOrDefault(s => s.StoreCode.Equals(storeCode));
            //当前登陆用户
            var currentUser = db.Employee.Where(e => e.Code.Equals(userCode) && e.Status).Select(e => new SimpleEmployee
            {
                Code = e.Code,
                NameENUS = e.NameENUS,
                NameZHCN = e.NameZHCN,
                Mail = e.Mail,
                Mobile = e.Mobile,
                Phone = e.Phone,
                TitleCode = e.TitleCode,
                TitleENUS = e.TitleENUS,
                TitleZHCN = e.TitleZHCN,
                PositionENUS = e.PositionENUS,
                PositionCode = e.PositionCode
            }).FirstOrDefault();
            var defaultrep = GetStoreAssetRepByStoreCode(storeCode).FirstOrDefault();
            switch(currentUser.PositionCode)
            {
                case PositionCodeConstants.Asset_Rep:
                    currentUser.PositionENUS = PositionCodeConstants.Asset_Actor;
                    result.Add(currentUser);
                    break;
                case PositionCodeConstants.Asset_Mgr:
                case PositionCodeConstants.Regional_Asset_Mgr:
                    result = db.Employee.Where(e => e.ReportToEID.Equals(userCode) && e.Status).Select(e => new SimpleEmployee
                    {
                        Code = e.Code,
                        NameENUS = e.NameENUS,
                        NameZHCN = e.NameZHCN,
                        Mail = e.Mail,
                        Mobile = e.Mobile,
                        Phone = e.Phone,
                        TitleCode = e.TitleCode,
                        TitleENUS = e.TitleENUS,
                        TitleZHCN = e.TitleZHCN,
                        PositionCode = e.PositionCode,
                        PositionENUS = PositionCodeConstants.Asset_Actor
                    }).ToList();
                    currentUser.PositionENUS = PositionCodeConstants.Asset_Actor;
                    result.Add(currentUser);
                    if (defaultrep != null && !result.Contains(defaultrep) && !currentUser.Code.Equals(defaultrep.Code, StringComparison.CurrentCultureIgnoreCase))
                    {
                        defaultrep.PositionENUS = PositionCodeConstants.Asset_Actor;
                        result.Add(defaultrep);
                    }
                    break;
                default:
                    currentUser.PositionENUS = PositionCodeConstants.Asset_Actor;
                    result.Add(currentUser);
                    break;
            }
            return result;
        }

        /// <summary>
        /// 获取资产代表经理
        /// </summary>
        /// <param name="storeCode"></param>
        /// <returns></returns>
        public static List<SimpleEmployee> GetStoreAssetRepMgrByStoreCode(string storeCode)
        {
            return GetStoreEmployeesByRole(storeCode,RoleCode.Market_Asset_Mgr);

            //var db = PrepareDb();
            //List<SimpleEmployee> result = new List<SimpleEmployee>();
            //var storeDev = db.StoreDevelop.FirstOrDefault(s => s.StoreCode.Equals(storeCode));
            //if (storeDev != null)
            //{
            //    var assetRepMgr = db.Employee.Where(e => e.Code.Equals(storeDev.AssetMgrEid) && e.Status).Select(e => new SimpleEmployee
            //    {
            //        Code = e.Code,
            //        NameENUS = e.NameENUS,
            //        NameZHCN = e.NameZHCN,
            //        Mail = e.Mail,
            //        Mobile = e.Mobile,
            //        Phone = e.Phone,
            //        TitleCode = e.TitleCode,
            //        TitleENUS = e.TitleENUS,
            //        TitleZHCN = e.TitleZHCN,
            //        PositionCode = e.PositionCode,
            //        PositionENUS = "Market Asset Mgr"
            //    }).SingleOrDefault();
            //    if (assetRepMgr != null) result.Add(assetRepMgr);
            //}
            //return result;
        }

        /// <summary>
        /// 获取资产经理
        /// </summary>
        /// <param name="storeCode">Store Code</param>
        /// <returns></returns>
        public static List<ProjectTeamMember> GetAssetRepMgrByStoreCode(string storeCode)
        {
            var db = PrepareDb();
            List<ProjectTeamMember> result = new List<ProjectTeamMember>();
            var storeDev = db.StoreDevelop.FirstOrDefault(s => s.StoreCode.Equals(storeCode));
            if (storeDev != null)
            {
                string roleName = GetRoleName(RoleCode.Market_Asset_Mgr);
                var assetRepMgr = db.Employee.Where(e => e.Code.Equals(storeDev.AssetMgrEid) && e.Status).Select(e => new ProjectTeamMember
                {
                    IsSelected = true,
                    RoleCode = roleName,
                    RoleNameENUS = roleName,
                    RoleNameZHCN = roleName,
                    UserAccount = e.Code,
                    UserNameENUS = e.NameENUS,
                    UserNameZHCN = e.NameZHCN

                }).FirstOrDefault();
                if (assetRepMgr != null) result.Add(assetRepMgr);
            }
            //if (result.Count == 0) result.Add(GetDefaultUser(PositionCodeConstants.Asset_Mgr));
            return result;
        }

        /// <summary>
        /// 获取资产代表 
        /// </summary>
        /// <param name="storeCode">店Code</param>
        /// <returns></returns>
        public List<ProjectTeamMember> GetAssetRepsByStoreCode(string storeCode, string currentUserCode, string roleCode)
        {
            var db = GetDb();
            List<ProjectTeamMember> result = new List<ProjectTeamMember>();

            //获取Store的Region/Market/City的Code
            var store = StoreBasicInfo.GetStore(storeCode);
            var user = db.Employee.Where(e => e.Code.Equals(store.StoreDevelop.AssetRepEid) && e.Status).Select(p => new ProjectTeamMember
            {
                UserAccount = p.Code,
                UserNameENUS = p.NameENUS,
                UserNameZHCN = p.NameZHCN
            }).FirstOrDefault();
            if (user != null) result.Add(user);

            return result;
        }

        /// <summary>
        /// 获取资产代表 
        /// </summary>
        /// <param name="storeCode">店Code</param>
        /// <returns></returns>
        public List<ProjectTeamMember> GetAssetActorByStoreCode(string storeCode, string currentUserCode, string roleCode)
        {
            var db = GetDb();
            List<ProjectTeamMember> result = new List<ProjectTeamMember>();

            var users = GetStoreAssetActorsByStoreCode(storeCode, currentUserCode).Select(p => new ProjectTeamMember
            {
                UserAccount = p.Code,
                UserNameENUS = p.NameENUS,
                UserNameZHCN = p.NameZHCN,
            });
            result.AddRange(users);

            return result;
        }

        #endregion

        #region 根据角色获取用户信息
        /// <summary>
        /// 根据角色获取该角色下所有的用户信息
        /// </summary>
        /// <param name="roleCode"></param>
        /// <returns></returns>
        public static List<SimpleEmployee> GetEmployeesByRole(RoleCode roleCode)
        {
            var db = PrepareDb();
            string rolename = GetRoleName(roleCode);
            var items = db.Database.SqlQuery<SimpleEmployee>(string.Format(@"
select e.Code, e.NameZHCN,e.NameENUS,e.PositionCode,e.PositionZHCN,
'{1}' as PositionENUS,e.Mail,e.Phone,e.Mobile  from Employee e where e.Code <> '' AND e.Status = 1 AND C_OldId in(
select UserID from Position where PositionID in (
SELECT PositionID FROM SysRolePositionMap
where RoleID ={0}))", (int)roleCode, rolename)).ToList();
            return items;
        }

        public static string GetRoleName(RoleCode roleCode)
        {
            var db = PrepareDb();
            int roleid = (int)roleCode;
            SysRole role = db.SysRole.FirstOrDefault(r => r.RoleID == roleid);
            return role == null ? "" : role.Code;
        }

        /// <summary>
        /// 根据角色名称获取用户-简单用户对象
        /// </summary>
        /// <param name="storeCode"></param>
        /// <param name="roleCode">Role Code是常量，直接在RoleCode枚举类中取值，如RoleCode.Asset_Mgr</param>
        /// <returns></returns>
        public static List<SimpleEmployee> GetStoreEmployeesByRole(string storeCode, RoleCode roleCode)
        {
            //是否是Market Asset Mgr， 如果是，即从当前Store中制定的Asset Mgr中获取
            //if (roleCode == RoleCode.Market_Asset_Mgr)
            //    return GetStoreAssetRepMgrByStoreCode(storeCode);
            RoleCode[] specialCodes = new RoleCode[] { RoleCode.IT, RoleCode.CDO, RoleCode.CFO, RoleCode.MCCL_Asset_Director };
            if (specialCodes.Contains(roleCode))
                return GetEmployeesByRole(roleCode);
            var db = PrepareDb();
            string rolename = GetRoleName(roleCode);
            var items = db.Database.SqlQuery<SimpleEmployee>(string.Format(@"
select e.Code, e.NameZHCN,e.NameENUS,e.PositionCode,e.PositionZHCN,
'{2}' as PositionENUS,e.Mail,e.Phone,e.Mobile from StoreBasicInfo s
join SysUserMMMap m on s.MMCode=m.MMCode
join SysRolePositionMap rp on m.PositionID=rp.PositionID
join SysRole r on r.RoleID=rp.RoleID
join Position p on p.PositionID=rp.PositionID
join Employee e on p.UserID=e.C_OldId
where s.StoreCode='{0}' and r.RoleId={1} and e.Code <> '' AND e.Status = 1", storeCode, (int)roleCode, rolename)).ToList();
            return items;
        }


        /// <summary>
        /// 根据角色名称获取用户-简单用户对象
        /// </summary>
        /// <param name="storeCode">Store US Code</param>
        /// <param name="roleCodes">RoleCode组合</param>
        /// <param name="userCode"></param>
        /// <returns></returns>
        public static List<SimpleEmployee> GetStoreEmployeesByMultiRoles(string storeCode, string[] roleCodes, string userCode)
        {
            var db = PrepareDb();
            List<SimpleEmployee> result = new List<SimpleEmployee>();
            var storeEntry = new StoreBasicInfo();
            //获取Store的Region/Market/City的Code
            var store = StoreBasicInfo.GetStore(storeCode);
            foreach (var roleCode in roleCodes)
            {
                string rc = roleCode.Trim();
                switch (rc)
                {
                    case "Asset Rep":
                        var reps = GetStoreAssetRepByStoreCode(storeCode);
                        result.AddRange(reps);
                        break;
                    case "Asset Mgr":
                        var repMgrs = GetStoreAssetRepMgrByStoreCode(storeCode);
                        result.AddRange(repMgrs);
                        break;
                    case "Asset Actor":
                        var repActors = GetStoreAssetActorsByStoreCode(storeCode, userCode);
                        result.AddRange(repActors);
                        break;
                    default:
                        RoleCode roleEnum;
                        if (Enum.TryParse(rc, out roleEnum))
                        {
                            var users = GetStoreEmployeesByRole(storeCode, roleEnum);
                            result.AddRange(users);
                        }
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 根据角色名称获取用户-简单用户对象
        /// </summary>
        /// <param name="storeCode">Store US Code</param>
        /// <param name="roleCodes">RoleCode组合</param>
        /// <returns>UserCode,RoleCode</returns>
        public static Dictionary<string, string> GetStoreEmployeesRoleCodeByMultiRoles(string storeCode, string[] roleCodes)
        {
            var db = PrepareDb();
            Dictionary<string, string> result = new Dictionary<string, string>();
            var storeEntry = new StoreBasicInfo();
            //获取Store的Region/Market/City的Code
            var store = StoreBasicInfo.GetStore(storeCode);
            foreach (var roleCode in roleCodes)
            {
                string rc = roleCode.Trim();
                switch (rc)
                {
                    case "Asset_Rep":
                        var reps = GetStoreAssetRepByStoreCode(storeCode);
                        foreach (var rep in reps)
                        {
                            result.Add(rep.Code, rc);
                        }
                        break;
                    case "Asset_Mgr":
                        var repMgrs = GetStoreAssetRepMgrByStoreCode(storeCode);
                        foreach (var repMgr in repMgrs)
                        {
                            result.Add(repMgr.Code, rc);
                        }
                        break;
                    default:
                        RoleCode roleEnum;
                        if (Enum.TryParse(rc, out roleEnum))
                        {
                            var users = GetStoreEmployeesByRole(storeCode, roleEnum);
                            foreach (var user in users)
                            {
                                result.Add(user.Code, rc);
                            }
                        }
                        break;
                }
            }
            return result;
        }

        #endregion

        public static List<SimpleEmployee> GetEmployeeContact(string projectId)
        {
            var roleCodeFilter = new List<string>()
            {
                ProjectUserRoleCode.AssetActor,
                ProjectUserRoleCode.AssetManager,
                ProjectUserRoleCode.AssetRep
            };
            var projectUsers = ProjectUsers.Search(e => e.ProjectId == projectId
                                                           && roleCodeFilter.Contains(e.RoleCode))
                .Select(e => e.UserAccount)
                .ToList();

            var storeEmployees = Employee.Search(e => projectUsers.Contains(e.Code)).Select(e => new SimpleEmployee()
            {
                NameENUS = e.NameENUS,
                Mail = e.Mail,
                Mobile = e.Mobile,
                Phone = e.Phone,
                PositionENUS = e.PositionENUS
            }).ToList();
            return storeEmployees;
        }
    }
}
