using System;
using System.Collections.Generic;
using Mcdonalds.AM.DataAccess;
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using System.Linq;
using System.Web.Http;
using Mcdonalds.AM.DataAccess.Entities;

namespace Mcdonalds.AM.Services.Controllers
{
    public class UserController : ApiController
    {
        [Route("api/Users/{pageIndex}/{pageSize}")]
        public IHttpActionResult GetUsers(int pageIndex, int pageSize)
        {
            var users = Employee.Search(e => e.Status);
            var skipCount = pageSize * (pageIndex - 1);
            var pagedUsers = users.OrderBy(e => e.NameENUS).Skip(skipCount).Take(pageSize);
            return Ok(new PagedDataSource(users.Count(), pagedUsers.ToArray()));
        }

        //[Route("api/StoreUsers/get/{storeCode}/{positionCode}")]
        //[HttpGet]
        //public IHttpActionResult GetStoreUsersByPosition(string storeCode, string positionCode)
        //{
        //    var user = Employee.GetStoreEmployeesByPosition(storeCode, positionCode);
        //    return Ok(user);
        //}

        [Route("api/StoreUsers/get/{storeCode}/{roleCode}")]
        [HttpGet]
        public IHttpActionResult GetStoreUsersByRole(string storeCode, string roleCode)
        {
            RoleCode rc;
            if (Enum.TryParse(roleCode, out rc))
            {
                var user = Employee.GetStoreEmployeesByRole(storeCode, rc);
                return Ok(user);
            }
            else
            {
                var user = Employee.GetStoreEmployeesByRole(storeCode, rc);
                return Ok(user);
            }
        }

        [Route("api/Approvers/GetMajorLeaseApprovers")]
        [HttpGet]
        public IHttpActionResult GetMajorLeaseApprovers(string flowCode, string projectId = "")
        {
            if (string.IsNullOrEmpty(flowCode))
                throw new Exception("flowCode is NULL");

            MajorLeaseInfo majorLeaseInfo = null;
            if (!string.IsNullOrEmpty(projectId))
            {
                majorLeaseInfo = MajorLeaseInfo.Search(e => e.ProjectId.Equals(projectId)).FirstOrDefault();

            }
            if (majorLeaseInfo == null)
            {
                throw new Exception("Could not find the Major Lease Info, please check it!");
            }

            var wfEntity = BaseWFEntity.GetWorkflowEntity(projectId, flowCode);
            var approvers = wfEntity.GetApproveDialogUser();

            var dicUsers = new Dictionary<string, List<SimpleEmployee>>();
            switch (flowCode)
            {
                case FlowCode.MajorLease_LegalReview:

                    dicUsers.Add("Legals", new List<SimpleEmployee>()
                    {
                        new SimpleEmployee()
                        {
                            Code = majorLeaseInfo.LegalAccount,
                            NameENUS = majorLeaseInfo.LegalNameENUS,
                            NameZHCN = majorLeaseInfo.LegalNameZHCN
                        }
                    });
                    break;
                case FlowCode.MajorLease_FinanceAnalysis:
                    dicUsers.Add("FMs", Employee.GetStoreEmployeesByRole(majorLeaseInfo.USCode, RoleCode.Finance_Manager));
                    break;
                case FlowCode.MajorLease_ConsInfo:
                    dicUsers.Add("ConstructionManagers", Employee.GetStoreEmployeesByRole(majorLeaseInfo.USCode, RoleCode.Cons_Mgr));
                    dicUsers.Add("MCCLConsManagers", Employee.GetStoreEmployeesByRole(majorLeaseInfo.USCode, RoleCode.MCCL_Cons_Manager));
                    break;
                case FlowCode.MajorLease_Package:
                    dicUsers.Add("MarketMgrs", Employee.GetStoreEmployeesByRole(majorLeaseInfo.USCode, RoleCode.Market_Asset_Mgr));
                    dicUsers.Add("RegionalMgrs", Employee.GetStoreEmployeesByRole(majorLeaseInfo.USCode, RoleCode.Regional_Asset_Mgr));
                    dicUsers.Add("DDs", Employee.GetStoreEmployeesByRole(majorLeaseInfo.USCode, RoleCode.Market_DD));
                    dicUsers.Add("GMs", Employee.GetStoreEmployeesByRole(majorLeaseInfo.USCode, RoleCode.GM));
                    dicUsers.Add("FCs", Employee.GetStoreEmployeesByRole(majorLeaseInfo.USCode, RoleCode.Finance_Controller));
                    dicUsers.Add("RDDs", Employee.GetStoreEmployeesByRole(majorLeaseInfo.USCode, RoleCode.Regional_DD));
                    dicUsers.Add("VPGMs", Employee.GetStoreEmployeesByRole(majorLeaseInfo.USCode, RoleCode.VPGM));
                    dicUsers.Add("CDOs", Employee.GetEmployeesByRole(RoleCode.CDO));
                    dicUsers.Add("CFOs", Employee.GetEmployeesByRole(RoleCode.CFO));
                    dicUsers.Add("ManagingDirectors", Employee.GetEmployeesByRole(RoleCode.MD));
                    break;
                case FlowCode.MajorLease_ConsInvtChecking:
                    //dicUsers.Add("FMs", new List<SimpleEmployee>()
                    //{
                    //    new SimpleEmployee()
                    //    {
                    //        Code = majorLeaseInfo.FinanceAccount,
                    //        NameENUS = majorLeaseInfo.FinanceNameENUS,
                    //        NameZHCN = majorLeaseInfo.FinanceNameZHCN
                    //    }
                    //});
                    dicUsers.Add("FMs", Employee.GetStoreEmployeesByRole(majorLeaseInfo.USCode, RoleCode.Finance_Manager));
                    dicUsers.Add("ConstructionManagers", Employee.GetStoreEmployeesByRole(majorLeaseInfo.USCode, RoleCode.Cons_Mgr));
                    dicUsers.Add("FinanceControllers", Employee.GetStoreEmployeesByRole(majorLeaseInfo.USCode, RoleCode.Finance_Controller));
                    dicUsers.Add("VPGMs", Employee.GetStoreEmployeesByRole(majorLeaseInfo.USCode, RoleCode.VPGM));
                    break;
                case FlowCode.MajorLease_GBMemo:
                    dicUsers.Add("ConstructionManagers", Employee.GetStoreEmployeesByRole(majorLeaseInfo.USCode, RoleCode.Cons_Mgr));
                    break;
            }
            dicUsers.Add("MCCLAssetMgrs", Employee.GetStoreEmployeesByRole(majorLeaseInfo.USCode, RoleCode.MCCL_Asset_Mgr));
            dicUsers.Add("MCCLAssetDtrs", Employee.GetStoreEmployeesByRole(majorLeaseInfo.USCode, RoleCode.MCCL_Asset_Director));
            return Ok(new { dicUsers, approvers });
        }

        [Route("api/Approvers/GetRebuildApprovers")]
        [HttpGet]
        public IHttpActionResult GetRebuildApprovers(string flowCode, string projectId = "", bool isNeedEntity = false)
        {
            if (string.IsNullOrEmpty(flowCode))
                throw new Exception("flowCode is NULL");

            RebuildInfo rbdInfo = null;
            if (!string.IsNullOrEmpty(projectId))
            {
                rbdInfo = RebuildInfo.Search(e => e.ProjectId.Equals(projectId)).FirstOrDefault();

            }
            if (rbdInfo == null)
            {
                throw new Exception("Could not find the Rebuild Info, please check it!");
            }
            var dicUsers = new Dictionary<string, List<SimpleEmployee>>();
            BaseWFEntity returnEntity = null;
            switch (flowCode)
            {
                case FlowCode.Rebuild_LegalReview:
                    dicUsers.Add("Legals", new List<SimpleEmployee>()
                    {
                        new SimpleEmployee()
                        {
                            Code = rbdInfo.LegalAccount,
                            NameENUS = rbdInfo.LegalNameENUS,
                            NameZHCN = rbdInfo.LegalNameZHCN
                        }
                    });
                    if (isNeedEntity)
                    {
                        var entity = RebuildLegalReview.FirstOrDefault((e => e.ProjectId.Equals(projectId) && !e.IsHistory));
                        if (entity != null)
                        {
                            RebuildLegalReview.PopulateAppUsers(entity);
                            returnEntity = entity;
                        }
                    }
                    break;
                case FlowCode.Rebuild_FinanceAnalysis:
                    dicUsers.Add("FMs", Employee.GetStoreEmployeesByRole(rbdInfo.USCode, RoleCode.Finance_Manager));
                    if (isNeedEntity)
                    {
                        var entity = RebuildFinancAnalysis.FirstOrDefault((e => e.ProjectId.Equals(projectId) && !e.IsHistory));
                        if (entity != null)
                        {
                            RebuildFinancAnalysis.PopulateAppUsers(entity);
                            returnEntity = entity;
                        }
                    }
                    break;
                case FlowCode.Rebuild_ConsInfo:
                    dicUsers.Add("ConstructionManagers", Employee.GetStoreEmployeesByRole(rbdInfo.USCode, RoleCode.Cons_Mgr));
                    dicUsers.Add("MCCLConsManagers", Employee.GetStoreEmployeesByRole(rbdInfo.USCode, RoleCode.MCCL_Cons_Manager));
                    if (isNeedEntity)
                    {
                        var entity = RebuildConsInfo.FirstOrDefault((e => e.ProjectId.Equals(projectId) && !e.IsHistory));
                        if (entity != null)
                        {
                            RebuildConsInfo.PopulateAppUsers(entity);
                            returnEntity = entity;
                        }
                    }
                    break;
                case FlowCode.Rebuild_Package:
                    dicUsers.Add("MarketMgrs", Employee.GetStoreEmployeesByRole(rbdInfo.USCode, RoleCode.Market_Asset_Mgr));
                    dicUsers.Add("RegionalMgrs", Employee.GetStoreEmployeesByRole(rbdInfo.USCode, RoleCode.Regional_Asset_Mgr));
                    dicUsers.Add("MDDs", Employee.GetStoreEmployeesByRole(rbdInfo.USCode, RoleCode.Market_DD));
                    dicUsers.Add("GMs", Employee.GetStoreEmployeesByRole(rbdInfo.USCode, RoleCode.GM));
                    dicUsers.Add("FCs", Employee.GetStoreEmployeesByRole(rbdInfo.USCode, RoleCode.Finance_Controller));
                    dicUsers.Add("RDDs", Employee.GetStoreEmployeesByRole(rbdInfo.USCode, RoleCode.Regional_DD));
                    dicUsers.Add("VPGMs", Employee.GetStoreEmployeesByRole(rbdInfo.USCode, RoleCode.VPGM));
                    dicUsers.Add("CDOs", Employee.GetEmployeesByRole(RoleCode.CDO));
                    dicUsers.Add("CFOs", Employee.GetEmployeesByRole(RoleCode.CFO));
                    dicUsers.Add("MngDirectors", Employee.GetEmployeesByRole(RoleCode.MD));
                    if (isNeedEntity)
                    {
                        var entity = RebuildPackage.FirstOrDefault((e => e.ProjectId.Equals(projectId) && !e.IsHistory));
                        if (entity != null)
                        {
                            RebuildPackage.PopulateAppUsers(entity);
                            returnEntity = entity;
                        }
                    }
                    break;
                case FlowCode.Rebuild_ConsInvtChecking:
                    //dicUsers.Add("FMs", new List<SimpleEmployee>()
                    //{
                    //    new SimpleEmployee()
                    //    {
                    //        Code = rbdInfo.FinanceAccount,
                    //        NameENUS = rbdInfo.FinanceNameENUS,
                    //        NameZHCN = rbdInfo.FinanceNameZHCN
                    //    }
                    //});
                    dicUsers.Add("FMs", Employee.GetStoreEmployeesByRole(rbdInfo.USCode, RoleCode.Finance_Manager));
                    dicUsers.Add("ConstructionManagers", Employee.GetStoreEmployeesByRole(rbdInfo.USCode, RoleCode.Cons_Mgr));
                    dicUsers.Add("FinanceControllers", Employee.GetStoreEmployeesByRole(rbdInfo.USCode, RoleCode.Finance_Controller));
                    dicUsers.Add("VPGMs", Employee.GetStoreEmployeesByRole(rbdInfo.USCode, RoleCode.VPGM));
                    if (isNeedEntity)
                    {
                        var entity = RebuildConsInvtChecking.FirstOrDefault((e => e.ProjectId.Equals(projectId) && !e.IsHistory));
                        if (entity != null)
                        {
                            RebuildConsInvtChecking.PopulateAppUsers(entity);
                            returnEntity = entity;
                        }
                    }
                    break;
                case FlowCode.Rebuild_GBMemo:
                    dicUsers.Add("ConstructionManagers", Employee.GetStoreEmployeesByRole(rbdInfo.USCode, RoleCode.Cons_Mgr));
                    if (isNeedEntity)
                    {
                        var entity = GBMemo.FirstOrDefault((e => e.ProjectId.Equals(projectId) && !e.IsHistory));
                        if (entity != null)
                        {
                            GBMemo.PopulateAppUsers(entity);
                            returnEntity = entity;
                        }
                    }
                    break;
            }
            dicUsers.Add("MCCLAssetMgrs", Employee.GetStoreEmployeesByRole(rbdInfo.USCode, RoleCode.MCCL_Asset_Mgr));
            dicUsers.Add("MCCLAssetDtrs", Employee.GetStoreEmployeesByRole(rbdInfo.USCode, RoleCode.MCCL_Asset_Director));


            return Ok(new
            {
                dicUsers,
                returnEntity,
                rbdInfo
            });
        }

        [Route("api/Approvers/GetReimageApprovers")]
        [HttpGet]
        public IHttpActionResult GetReimageApprovers(string flowCode, string projectId = "")
        {
            if (string.IsNullOrEmpty(flowCode))
                throw new Exception("flowCode is NULL");

            ReimageInfo reimageInfo = null;
            if (!string.IsNullOrEmpty(projectId))
            {
                reimageInfo = ReimageInfo.Search(e => e.ProjectId.Equals(projectId)).FirstOrDefault();

            }
            if (reimageInfo == null)
            {
                throw new Exception("Could not find the Reimage Info, please check it!");
            }
            var dicUsers = new Dictionary<string, List<SimpleEmployee>>();
            switch (flowCode)
            {
                case FlowCode.Reimage_Summary:
                    dicUsers.Add("FMs", Employee.GetStoreEmployeesByRole(reimageInfo.USCode, RoleCode.Finance_Manager));
                    break;
                case FlowCode.Reimage_ConsInfo:
                    dicUsers.Add("ConstructionManagers", Employee.GetStoreEmployeesByRole(reimageInfo.USCode, RoleCode.Cons_Mgr));
                    dicUsers.Add("MCCLConsManagers", Employee.GetStoreEmployeesByRole(reimageInfo.USCode, RoleCode.MCCL_Cons_Manager));
                    break;
                case FlowCode.Reimage_Package:
                    dicUsers.Add("MarketMgrs", Employee.GetStoreEmployeesByRole(reimageInfo.USCode, RoleCode.Market_Asset_Mgr));
                    dicUsers.Add("RegionalMgrs", Employee.GetStoreEmployeesByRole(reimageInfo.USCode, RoleCode.Regional_Asset_Mgr));
                    dicUsers.Add("MDDs", Employee.GetStoreEmployeesByRole(reimageInfo.USCode, RoleCode.Market_DD));
                    dicUsers.Add("GMs", Employee.GetStoreEmployeesByRole(reimageInfo.USCode, RoleCode.GM));
                    dicUsers.Add("FCs", Employee.GetStoreEmployeesByRole(reimageInfo.USCode, RoleCode.Finance_Controller));
                    dicUsers.Add("DOs", Employee.GetStoreEmployeesByRole(reimageInfo.USCode, RoleCode.DO));
                    dicUsers.Add("RDDs", Employee.GetStoreEmployeesByRole(reimageInfo.USCode, RoleCode.Regional_DD));
                    dicUsers.Add("VPGMs", Employee.GetStoreEmployeesByRole(reimageInfo.USCode, RoleCode.VPGM));
                    dicUsers.Add("CDOs", Employee.GetEmployeesByRole(RoleCode.CDO));
                    dicUsers.Add("CFOs", Employee.GetEmployeesByRole(RoleCode.CFO));
                    dicUsers.Add("MngDirectors", Employee.GetEmployeesByRole(RoleCode.MD));
                    break;
                case FlowCode.Reimage_ConsInvtChecking:
                    //dicUsers.Add("FMs", new List<SimpleEmployee>()
                    //{
                    //    new SimpleEmployee()
                    //    {
                    //        Code = reimageInfo.FinanceAccount,
                    //        NameENUS = reimageInfo.FinanceNameENUS,
                    //        NameZHCN = reimageInfo.FinanceNameZHCN
                    //    }
                    //});
                    dicUsers.Add("FMs", Employee.GetStoreEmployeesByRole(reimageInfo.USCode, RoleCode.Finance_Manager));
                    dicUsers.Add("ConstructionManagers", Employee.GetStoreEmployeesByRole(reimageInfo.USCode, RoleCode.Cons_Mgr));
                    dicUsers.Add("FinanceControllers", Employee.GetStoreEmployeesByRole(reimageInfo.USCode, RoleCode.Finance_Controller));
                    dicUsers.Add("VPGMs", Employee.GetStoreEmployeesByRole(reimageInfo.USCode, RoleCode.VPGM));
                    break;
                case FlowCode.Reimage_GBMemo:
                    dicUsers.Add("ConstructionManagers", Employee.GetStoreEmployeesByRole(reimageInfo.USCode, RoleCode.Cons_Mgr));
                    break;
            }
            dicUsers.Add("MCCLAssetMgrs", Employee.GetStoreEmployeesByRole(reimageInfo.USCode, RoleCode.MCCL_Asset_Mgr));
            dicUsers.Add("MCCLAssetDtrs", Employee.GetStoreEmployeesByRole(reimageInfo.USCode, RoleCode.MCCL_Asset_Director));
            return Ok(dicUsers);
        }

        [Route("api/Approvers/GetRenewalApprovers")]
        [HttpGet]
        public IHttpActionResult GetRenewalApprovers(string flowCode, string projectId = "")
        {
            if (string.IsNullOrEmpty(flowCode))
            {
                return BadRequest("flowCode is NULL");
            }

            RenewalInfo renewalInfo = RenewalInfo.Get(projectId);
            if (renewalInfo == null)
            {
                return BadRequest("Could not find the Renewal Info, please check it!");
            }
            var dicUsers = new Dictionary<string, object>();
            var consMgr = ProjectUsers.GetProjectUser(projectId, ProjectUserRoleCode.CM);
            switch (flowCode)
            {
                case FlowCode.Renewal_Letter:
                    {
                        var info = RenewalInfo.Get(projectId);
                        dicUsers.Add("AssetManagerCode", info.AssetManagerAccount);
                        dicUsers.Add("AssetManagers", new[] { Employee.GetSimpleEmployeeByCode(info.AssetManagerAccount) });
                    }
                    break;
                case FlowCode.Renewal_GBMemo:
                    {
                        var refTableId = RenewalInfo.GetRefTableId("RenewalGBMemo", projectId);
                        var approvers = ApproveDialogUser.GetApproveDialogUser(refTableId) ?? new ApproveDialogUser();
                        dicUsers.Add("ConstructionManagerCode", approvers.ConstructionManagerCode);
                        dicUsers.Add("ConstructionManagers", Employee.GetStoreEmployeesByRole(renewalInfo.USCode, RoleCode.Cons_Mgr));
                    }
                    break;
                case FlowCode.Renewal_ConsInfo:
                    {
                        var refTableId = RenewalInfo.GetRefTableId("RenewalConsInfo", projectId);
                        var approvers = ApproveDialogUser.GetApproveDialogUser(refTableId) ?? new ApproveDialogUser();
                        dicUsers.Add("ConstructionManagerCode", approvers.ConstructionManagerCode);
                        dicUsers.Add("MCCLConsManagerCode", approvers.MCCLConsManagerCode);
                        dicUsers.Add("ConstructionManagers", new List<SimpleEmployee>{
                            new SimpleEmployee { Code = consMgr.UserAccount, NameENUS = consMgr.UserNameENUS, NameZHCN = consMgr.UserNameZHCN }
                        });
                        dicUsers.Add("MCCLConsManagers", Employee.GetStoreEmployeesByRole(renewalInfo.USCode, RoleCode.MCCL_Cons_Manager));
                    }
                    break;
                case FlowCode.Renewal_Tool:
                    {
                        var refTableId = RenewalInfo.GetRefTableId("RenewalTool", projectId);
                        var approvers = ApproveDialogUser.GetApproveDialogUser(refTableId) ?? new ApproveDialogUser();
                        dicUsers.Add("FMCode", approvers.FMCode);
                        dicUsers.Add("FMs", Employee.GetStoreEmployeesByRole(renewalInfo.USCode, RoleCode.Finance_Manager));
                    }
                    break;
                case FlowCode.Renewal_LegalApproval:
                    {
                        var legal = ProjectUsers.GetProjectUser(projectId, ProjectUserRoleCode.Legal);
                        var refTableId = RenewalInfo.GetRefTableId("RenewalLegalApproval", projectId);
                        var approvers = ApproveDialogUser.GetApproveDialogUser(refTableId) ?? new ApproveDialogUser();
                        dicUsers.Add("LegalCode", legal.UserAccount);
                        dicUsers.Add("Legals", new List<SimpleEmployee>{
                            new SimpleEmployee { Code = legal.UserAccount, NameENUS = legal.UserNameENUS, NameZHCN = legal.UserNameZHCN }
                        });
                        dicUsers.Add("GeneralCounselCode", approvers.GeneralCounselCode);
                        dicUsers.Add("GeneralCounsels", Employee.GetStoreEmployeesByRole(renewalInfo.USCode, RoleCode.Legal_GeneralCounsel));
                    }
                    break;
                case FlowCode.Renewal_Package:
                    {
                        var refTableId = RenewalInfo.GetRefTableId("RenewalPackage", projectId);
                        var approvers = ApproveDialogUser.GetApproveDialogUser(refTableId) ?? new ApproveDialogUser();
                        dicUsers.Add("MarketMgrCode", approvers.MarketMgrCode);
                        dicUsers.Add("MarketMgrs", Employee.GetStoreEmployeesByRole(renewalInfo.USCode, RoleCode.Market_Asset_Mgr));
                        dicUsers.Add("RegionalMgrCode", approvers.RegionalMgrCode);
                        dicUsers.Add("RegionalMgrs", Employee.GetStoreEmployeesByRole(renewalInfo.USCode, RoleCode.Regional_Asset_Mgr));
                        dicUsers.Add("MDDCode", approvers.MDDCode);
                        dicUsers.Add("MDDs", Employee.GetStoreEmployeesByRole(renewalInfo.USCode, RoleCode.Market_DD));
                        dicUsers.Add("GMCode", approvers.GMCode);
                        dicUsers.Add("GMs", Employee.GetStoreEmployeesByRole(renewalInfo.USCode, RoleCode.GM));
                        dicUsers.Add("FCCode", approvers.FCCode);
                        dicUsers.Add("FCs", Employee.GetStoreEmployeesByRole(renewalInfo.USCode, RoleCode.Finance_Controller));
                        dicUsers.Add("RDDCode", approvers.RDDCode);
                        dicUsers.Add("RDDs", Employee.GetStoreEmployeesByRole(renewalInfo.USCode, RoleCode.Regional_DD));
                        dicUsers.Add("VPGMCode", approvers.VPGMCode);
                        dicUsers.Add("VPGMs", Employee.GetStoreEmployeesByRole(renewalInfo.USCode, RoleCode.VPGM));
                        dicUsers.Add("MCCLAssetDtrCode", approvers.MCCLAssetDtrCode);
                        dicUsers.Add("MCCLAssetDtrs", Employee.GetStoreEmployeesByRole(renewalInfo.USCode, RoleCode.MCCL_Asset_Director));
                        dicUsers.Add("CDOCode", approvers.CDOCode);
                        dicUsers.Add("CDOs", Employee.GetStoreEmployeesByRole(renewalInfo.USCode, RoleCode.CDO));
                        dicUsers.Add("ManagingDirectorCode", approvers.MngDirectorCode);
                        dicUsers.Add("ManagingDirectors", Employee.GetStoreEmployeesByRole(renewalInfo.USCode, RoleCode.MD));//MCCL_Cons_Director
                        dicUsers.Add("MCCLAssetMgrCode", approvers.MCCLAssetMgrCode);
                        dicUsers.Add("MCCLAssetMgrs", Employee.GetStoreEmployeesByRole(renewalInfo.USCode, RoleCode.MCCL_Asset_Mgr));
                        dicUsers.Add("NoticeUsers", !string.IsNullOrEmpty(approvers.NoticeUsers) ? Employee.GetSimpleEmployeeByCodes(approvers.NoticeUsers.Split(';')) : null);
                        dicUsers.Add("NecessaryNoticeUsers", !string.IsNullOrEmpty(approvers.NecessaryNoticeUsers) ? Employee.GetSimpleEmployeeByCodes(approvers.NecessaryNoticeUsers.Split(';')) : Employee.GetSimpleEmployeeByCodes(NecessaryNoticeConfig.Search(i => i.FlowCode == FlowCode.Renewal_Package && !string.IsNullOrEmpty(i.DefaultUserCode)).Select(i => i.DefaultUserCode).ToArray()));
                    }
                    break;
            }
            return Ok(dicUsers);
        }


        //[Route("api/StoreUsers/GetStoreEmployeesByMultiPositions/{storeCode}")]
        //[HttpGet]
        //public IHttpActionResult GetStoreEmployeesByMultiPositions(string storeCode, string positionCodes = "", string userCode = "")
        //{
        //    string[] arry = positionCodes.Split(',');
        //    List<SimpleEmployee> list = Employee.GetStoreEmployeesByMultiPositions(storeCode, arry, userCode);

        //    return Ok(list);
        //}

        [Route("api/StoreUsers/GetStoreEmployeesByMultiRoles/{storeCode}")]
        [HttpGet]
        public IHttpActionResult GetStoreEmployeesByMultiRoles(string storeCode, string roleCodes = "", string userCode = "")
        {
            string[] arry = roleCodes.Split(',');
            List<SimpleEmployee> list = Employee.GetStoreEmployeesByMultiRoles(storeCode, arry, userCode);

            return Ok(list);
        }

        [Route("api/StoreUsers/GetStoreEmployeesContactInfo/{projectId}")]
        [HttpGet]
        public IHttpActionResult GetStoreEmployeesContactInfo(string projectId)
        {
            var storeEmployees = Employee.GetEmployeeContact(projectId);
            //foreach (var code in projectUsers)
            //{
            //    var listuser = Employee.Search(e => e.Code.Equals(code)).Select(e => new SimpleEmployee()
            //    {
            //        NameENUS = e.NameENUS,
            //        Mail = e.Mail,
            //        Mobile = e.Mobile,
            //        Phone = e.Phone,
            //        PositionENUS = e.PositionENUS
            //    }).ToList();
            //    if (listuser != null && listuser.Count > 0)
            //    {
            //        storeEmployees.Add(listuser[0]);
            //    }
            //}
            return Ok(storeEmployees);
        }

        /// <summary>
        /// 获取必要抄送人
        /// </summary>
        /// <param name="storeCode"></param>
        /// <param name="flowCode"></param>
        /// <param name="userCode"></param>
        /// <returns>返回用户Code</returns>
        [Route("api/NecessaryNotice/GetAvailableUserCodes/{storeCode}/{flowCode}")]
        [HttpGet]
        public IHttpActionResult GetAvailableUserCodes(string storeCode, string flowCode, string userCode = "")
        {
            var config = NecessaryNoticeConfig.Search(i => i.FlowCode == flowCode);
            if (config == null)
            {
                return Ok();
            }
            var resultCodes = string.Empty;
            var arry = config.Select(i => i.NecessaryNoticeRoles).ToArray();
            var dictionary = Employee.GetStoreEmployeesRoleCodeByMultiRoles(storeCode, arry);
            var roleUser = new List<object>();
            foreach (var dictItem in dictionary)
            {
                if (string.IsNullOrEmpty(resultCodes))
                    resultCodes += dictItem.Key;
                else
                    resultCodes += "," + dictItem.Key;
                roleUser.Add(new
                {
                    UserCode = dictItem.Key,
                    RoleName = dictItem.Value
                });
            }
            return Ok(new
            {
                UserCodes = resultCodes,
                RoleNames = String.Join(",", arry),
                RoleUser = roleUser
            });
        }
    }
}
