using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   7/16/2014 2:31:46 PM
 * FileName     :   ProjectView
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using Mcdonalds.AM.DataAccess.Constants;
using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Entities;
using Mcdonalds.AM.Services.Infrastructure;

namespace Mcdonalds.AM.DataAccess
{
    public partial class VProject : BaseView<VProject>
    {
        public bool HasPendingRight { get; set; }

        public PackageHoldingDto PackageHoldingDto
        {
            get
            {
                var packageHoldingDto = new PackageHoldingDto();
                packageHoldingDto.ProjectId = ProjectId;
                packageHoldingDto.FlowCode = FlowCode;
                packageHoldingDto.HasRight = HasHoldingProjectRight.HasValue ? HasHoldingProjectRight.Value : false;
                packageHoldingDto.Status = HoldingProjectStatus.HasValue ? HoldingProjectStatus.Value : HoldingStatus.Unknown;
                return packageHoldingDto;
            }
        }

        public static List<VProject> Search(int pageIndex, int pageSize, ProjectSearchCondition condition, ref int totalItems)
        {
            var db = PrepareDb();
            IQueryable<VProject> projects;
            var haveScope = (
                                from um in db.SysUserMMMap
                                from e in db.Employee
                                from rp in db.SysRolePositionMap
                                from r in db.SysRole
                                where e.Code == ClientCookie.UserCode
                                    && um.UserID == e.C_OldId
                                    && rp.PositionID == e.PositionID
                                    && r.RoleID == rp.RoleID
                                    && r.UseRange
                                select 1
                            ).Any();
            var emp = Employee.GetEmployeeByCode(ClientCookie.UserCode);
            var projectsByTeam = (from p in db.VProject
                                  from team in db.ProjectUsers
                                  where (p.ProjectId == team.ProjectId && team.UserAccount == ClientCookie.UserCode)
                                  select p).Distinct();
            var projectsByWorkflowMember = (from p in db.VProject
                                            from ap in db.ApproveDialogUser
                                            where (p.ProjectId == ap.ProjectId && (ap.NoticeUsers.Contains(ClientCookie.UserCode) || ap.NecessaryNoticeUsers.Contains(ClientCookie.UserCode)))
                                            select p).Distinct();
            if (haveScope)
            {
                var projectsByScope = (from p in db.VProject
                                       from s in db.StoreBasicInfo
                                       from um in db.SysUserMMMap
                                       from e in db.Employee
                                       where e.Code == ClientCookie.UserCode
                                         && um.UserID == e.C_OldId
                                         && s.MMCode == um.MMCode
                                         && p.USCode == s.StoreCode
                                       select p).Distinct();
                projects = projectsByTeam.Union(projectsByWorkflowMember).Union(projectsByScope);
            }
            else if (emp.PositionCode == "suoya303055")
            {
                var projectsByRole = (from p in db.VProject
                                      from sd in db.StoreDevelop
                                      where (sd.AssetMgrEid == ClientCookie.UserCode || sd.AssetRepEid == ClientCookie.UserCode)
                                        && p.USCode == sd.StoreCode
                                      select p).Distinct();
                projects = projectsByTeam.Union(projectsByWorkflowMember).Union(projectsByRole);
            }
            else
            {
                projects = db.VProject.Where(e => true);
            }

            if (condition == null) condition = new ProjectSearchCondition();
            if (condition.CreateDateFrom.HasValue)
            {
                if (condition.CreateDateTo.HasValue)
                {
                    var enddate = Convert.ToDateTime(condition.CreateDateTo.Value.ToString("yyyy-MM-dd") + " 23:59:59");
                    projects = projects.Where(e => e.CreateTime >= condition.CreateDateFrom.Value
                                           && e.CreateTime <= enddate);
                }
                else
                {
                    projects = projects.Where(e => e.CreateTime >= condition.CreateDateFrom.Value);
                }

            }

            if (condition.ReImagingDateFrom.HasValue)
            {
                if (condition.ReImagingDateTo.HasValue)
                {
                    var enddate = Convert.ToDateTime(condition.ReImagingDateTo.Value.ToString("yyyy-MM-dd") + " 23:59:59");
                    projects = projects.Where(e => e.ReImageDate >= condition.ReImagingDateFrom.Value
                                           && e.ReImageDate <= enddate);
                }
                else
                {
                    projects = projects.Where(e => e.ReImageDate >= condition.ReImagingDateFrom.Value);
                }

            }

            if (condition.ReOpenDateFrom.HasValue)
            {
                if (condition.ReOpenDateTo.HasValue)
                {
                    var enddate = Convert.ToDateTime(condition.ReOpenDateTo.Value.ToString("yyyy-MM-dd") + " 23:59:59");
                    projects = projects.Where(e => e.ReopenDate >= condition.ReOpenDateFrom.Value
                                           && e.ReopenDate <= enddate);
                }
                else
                {
                    projects = projects.Where(e => e.ReopenDate >= condition.ReOpenDateFrom.Value);
                }

            }

            if (condition.GBDateFrom.HasValue)
            {
                if (condition.GBDateTo.HasValue)
                {
                    var enddate = Convert.ToDateTime(condition.GBDateTo.Value.ToString("yyyy-MM-dd") + " 23:59:59");
                    projects = projects.Where(e => e.GBDate >= condition.GBDateFrom.Value
                                           && e.GBDate <= enddate);
                }
                else
                {
                    projects = projects.Where(e => e.GBDate >= condition.GBDateFrom.Value);
                }

            }
            if (condition.CFDateFrom.HasValue)
            {
                if (condition.CFDateTo.HasValue)
                {
                    var enddate = Convert.ToDateTime(condition.CFDateTo.Value.ToString("yyyy-MM-dd") + " 23:59:59");
                    projects = projects.Where(e => e.ConstructionFinishDate >= condition.CFDateFrom.Value
                                           && e.ConstructionFinishDate <= enddate);
                }
                else
                {
                    projects = projects.Where(e => e.ConstructionFinishDate >= condition.CFDateFrom.Value);
                }

            }

            var statuses = new List<ProjectStatus>();
            if (condition.Status != null)
            {
                statuses = condition.Status.Select(s =>
                {
                    var status = ProjectStatus.UnKnown;
                    if (!string.IsNullOrEmpty(s))
                    {
                        Enum.TryParse(s, out status);
                    }
                    return status;
                }).Distinct().ToList();
            }




            //string[] usCodes = string.IsNullOrEmpty(condition.USCode) ? new string[0] : condition.USCode.Split(',');
            Expression<Func<VProject, bool>> predicate = pv =>
                    (string.IsNullOrEmpty(condition.ProjectId) || pv.ProjectId == condition.ProjectId)
                && (!condition.CreateDate.HasValue || pv.CreateTime == condition.CreateDate.Value)
                && (string.IsNullOrEmpty(condition.StoreType) || true)
                && (string.IsNullOrEmpty(condition.ProjectType) || pv.FlowCode == condition.ProjectType)
                && (string.IsNullOrEmpty(condition.PortfolioType) || true)
                && (statuses.Count == 0 || statuses.Any(s => s == ProjectStatus.UnKnown || s == pv.Status))
                && (string.IsNullOrEmpty(condition.StoreStatus) || true)
                && (!condition.OpenDate.HasValue || pv.OpenDate == condition.OpenDate.Value)
                && (string.IsNullOrEmpty(condition.AssetActor) || pv.AssetActorNameENUS.Contains(condition.AssetActor))
                && (string.IsNullOrEmpty(condition.AssetRep) || pv.AssetRepAccount == condition.AssetRep)
                && (string.IsNullOrEmpty(condition.AssetManager) || pv.AssetManagerAccount == condition.AssetManager)
                && (string.IsNullOrEmpty(condition.Finance) || pv.FinanceAccount == condition.Finance)
                && (string.IsNullOrEmpty(condition.PM) || pv.PMAccount == condition.PM)
                && (!condition.CloseDate.HasValue || pv.CloseDate == condition.CloseDate)
                && (string.IsNullOrEmpty(condition.Legal) || pv.LegalAccount == condition.Legal)
                && (string.IsNullOrEmpty(condition.USCode) || pv.USCode.Contains(condition.USCode))
                && (!condition.ReImagingDate.HasValue || pv.ReImageDate == condition.ReImagingDate.Value)
                && (string.IsNullOrEmpty(condition.StoreName) || (!string.IsNullOrEmpty(pv.StoreNameZHCN) && pv.StoreNameZHCN.Contains(condition.StoreName)) || (!string.IsNullOrEmpty(pv.StoreNameENUS) && pv.StoreNameENUS.Contains(condition.StoreName)))
                && (string.IsNullOrEmpty(condition.DesignType) || true)
                && (string.IsNullOrEmpty(condition.Province) || pv.ProvinceZHCN == condition.Province || pv.ProvinceENUS == condition.Province)
                && (string.IsNullOrEmpty(condition.City) || pv.CityZHCN == condition.City || pv.CityENUS == condition.City)
                && (!pv.IsPushed.HasValue)
                && (condition.HoldingStatus == HoldingStatus.Unknown || (condition.HoldingStatus != HoldingStatus.Unknown && pv.HoldingProjectStatus == condition.HoldingStatus));

            projects = projects.Where(predicate);
            totalItems = projects.Count();
            //var sql = projects.Where(predicate).OrderByDescending(e => e.CreateTime).ToString();

            #region
            var list = projects.Where(predicate).ToList();
            if (condition.CreateDateFrom.HasValue)
            {
                if (condition.CreateDateTo.HasValue)
                {
                    var enddate = Convert.ToDateTime(condition.CreateDateTo.Value.ToString("yyyy-MM-dd") + " 23:59:59");
                    list = list.Where(e => e.CreateTime >= condition.CreateDateFrom.Value
                                           && e.CreateTime <= enddate).ToList();
                }
                else
                {
                    list = list.Where(e => e.CreateTime >= condition.CreateDateFrom.Value).ToList();
                }

            }

            if (condition.ReImagingDateFrom.HasValue)
            {
                if (condition.ReImagingDateTo.HasValue)
                {
                    var enddate = Convert.ToDateTime(condition.ReImagingDateTo.Value.ToString("yyyy-MM-dd") + " 23:59:59");
                    list = list.Where(e => e.ReImageDate >= condition.ReImagingDateFrom.Value
                                           && e.ReImageDate <= enddate).ToList();
                }
                else
                {
                    list = list.Where(e => e.ReImageDate >= condition.ReImagingDateFrom.Value).ToList();
                }

            }

            if (condition.ReOpenDateFrom.HasValue)
            {
                if (condition.ReOpenDateTo.HasValue)
                {
                    var enddate = Convert.ToDateTime(condition.ReOpenDateTo.Value.ToString("yyyy-MM-dd") + " 23:59:59");
                    list = list.Where(e => e.ReopenDate >= condition.ReOpenDateFrom.Value
                                           && e.ReopenDate <= enddate).ToList();
                }
                else
                {
                    list = list.Where(e => e.ReopenDate >= condition.ReOpenDateFrom.Value).ToList();
                }

            }

            if (condition.GBDateFrom.HasValue)
            {
                if (condition.GBDateTo.HasValue)
                {
                    var enddate = Convert.ToDateTime(condition.GBDateTo.Value.ToString("yyyy-MM-dd") + " 23:59:59");
                    list = list.Where(e => e.GBDate >= condition.GBDateFrom.Value
                                           && e.GBDate <= enddate).ToList();
                }
                else
                {
                    list = list.Where(e => e.GBDate >= condition.GBDateFrom.Value).ToList();
                }

            }
            if (condition.CFDateFrom.HasValue)
            {
                if (condition.CFDateTo.HasValue)
                {
                    var enddate = Convert.ToDateTime(condition.CFDateTo.Value.ToString("yyyy-MM-dd") + " 23:59:59");
                    list = list.Where(e => e.ConstructionFinishDate >= condition.CFDateFrom.Value
                                           && e.ConstructionFinishDate <= enddate).ToList();
                }
                else
                {
                    list = list.Where(e => e.ConstructionFinishDate >= condition.CFDateFrom.Value).ToList();
                }

            }
            #endregion

            if (pageIndex > 0 && pageSize > 0)
                return list.OrderByDescending(e => e.CreateTime).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            else
                return list.OrderByDescending(e => e.CreateTime).ToList();
        }


        //public void SetPackageHoldingSource()
        //{
        //    var packageHoldingDto = new PackageHoldingDto();
        //    packageHoldingDto.ProjectId = ProjectId;
        //    packageHoldingDto.FlowCode = FlowCode;

        //    if (FlowCode == Constants.FlowCode.Reimage)
        //    {
        //        var wfEntity = BaseWFEntity.GetWorkflowEntity(ProjectId, Constants.FlowCode.Reimage_Package);

        //        if (wfEntity != null)
        //        {
        //            var packageRoleUsers = BaseWFEntity.GetPackageHoldingRoleUsers();
        //            if (packageRoleUsers.Select(e => e.Code).Contains(ClientCookie.UserCode)
        //                && ProjectInfo.Any(e => e.ProjectId == ProjectId
        //                    && e.FlowCode == Constants.FlowCode.Reimage_ConsInfo
        //                    && e.Status == ProjectStatus.Finished)
        //                && ProjectInfo.Any(e => e.ProjectId == ProjectId
        //                    && e.FlowCode == Constants.FlowCode.Reimage_Summary
        //                    && e.Status == ProjectStatus.Finished)
        //                && ProjectInfo.Any(e => e.ProjectId == ProjectId
        //                    && e.FlowCode == Constants.FlowCode.Reimage_Package
        //                    && e.Status == ProjectStatus.UnFinish))
        //            {
        //                packageHoldingDto.HasRight = true;
        //                //packageHoldingDto.HasRight = false;

        //                packageHoldingDto.Status = wfEntity.GetPackageHoldingStatus();

        //            }
        //        }

        //    }

        //    PackageHoldingDto = packageHoldingDto;
        //}

        public void SetPendingRight()
        {
            var marketAssetMgr = Employee.GetEmployeesByRole(RoleCode.Market_Asset_Mgr);
            var regionalAssetMgr = Employee.GetEmployeesByRole(RoleCode.Regional_Asset_Mgr);

            var packageRoleUsers = BaseWFEntity.GetPackageHoldingRoleUsers();
            if (ProjectUsers.Any(pu => pu.ProjectId == ProjectId
                                       && pu.UserAccount == ClientCookie.UserCode
                                       && pu.RoleCode == ProjectUserRoleCode.AssetActor)
                || packageRoleUsers.Select(e => e.Code).Contains(ClientCookie.UserCode)
                || marketAssetMgr.Select(e => e.Code).Contains(ClientCookie.UserCode)
                || regionalAssetMgr.Select(e => e.Code).Contains(ClientCookie.UserCode))
            {
                HasPendingRight = true;
            }
        }
    }

    public sealed class ProjectSearchCondition
    {
        public string ProjectId { get; set; }
        public DateTime? CreateDate { get; set; }

        public DateTime? CreateDateFrom { get; set; }
        public DateTime? CreateDateTo { get; set; }
        public string StoreType { get; set; }
        public string ProjectType { get; set; }
        public string PortfolioType { get; set; }
        public List<string> Status { get; set; }
        public string StoreStatus { get; set; }
        public DateTime? OpenDate { get; set; }
        public DateTime? ReOpenDateFrom { get; set; }
        public DateTime? ReOpenDateTo { get; set; }
        public DateTime? GBDateFrom { get; set; }
        public DateTime? GBDateTo { get; set; }
        public DateTime? CFDateFrom { get; set; }
        public DateTime? CFDateTo { get; set; }
        public string AssetActor { get; set; }
        public string AssetRep { get; set; }
        public string AssetManager { get; set; }
        public string Finance { get; set; }
        public string PM { get; set; }
        public DateTime? CloseDate { get; set; }
        public string Legal { get; set; }
        public string USCode { get; set; }
        public DateTime? ReImagingDate { get; set; }
        public DateTime? ReImagingDateFrom { get; set; }
        public DateTime? ReImagingDateTo { get; set; }
        public string StoreName { get; set; }
        public string DesignType { get; set; }
        public string Province { get; set; }
        public string City { get; set; }
        public HoldingStatus HoldingStatus { get; set; }
    }
}
