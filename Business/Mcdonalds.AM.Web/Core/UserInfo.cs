﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.Web.Core
{
    [Serializable]
    public class UserInfo
    {
        public System.Guid Id { get; set; }
        public string Code { get; set; }
        public int C_OldId { get; set; }
        public string Account { get; set; }
        public string NameZHCN { get; set; }
        public string NameENUS { get; set; }
        public bool Status { get; set; }
        public string Mail { get; set; }
        public string Phone { get; set; }
        public string Mobile { get; set; }
        public int Sex { get; set; }
        public string Birthday { get; set; }
        public string Address { get; set; }
        public string JoinDate { get; set; }
        public string Fax { get; set; }
        public string IDNumber { get; set; }
        public string Education { get; set; }
        public string University { get; set; }
        public string Major { get; set; }
        public string Ext { get; set; }
        public string HCRegion { get; set; }
        public string HCMarket { get; set; }
        public string HCCity { get; set; }
        public string HCFunction { get; set; }
        public string CompanyEntity { get; set; }
        public Nullable<bool> IsDevDept { get; set; }
        public string Function { get; set; }
        public string Department { get; set; }
        public Nullable<int> Leader { get; set; }
        public string Office { get; set; }
        public string WorkStatus { get; set; }
        public Nullable<int> IsAgent { get; set; }
        public Nullable<int> Agent { get; set; }
        public string Begindate { get; set; }
        public string EndDate { get; set; }
        public string StationOffice { get; set; }
        public string OfficeNo { get; set; }
        public Nullable<int> iPadStatus { get; set; }
        public string JoinFrom { get; set; }
        public string FromDepartment { get; set; }
        public string FirstName_EN { get; set; }
        public string SecondName_EN { get; set; }
        public string FirstName_CN { get; set; }
        public string SecondName_CN { get; set; }
        public string DirectLine { get; set; }
        public string CurPositionDate { get; set; }
        public string Development { get; set; }
        public string DTPipelineOnly { get; set; }
        public string PhotoLink { get; set; }
        public string WorkingExpLink { get; set; }
        public string IsPMT { get; set; }
        public string IsPMO { get; set; }
        public string Remark { get; set; }
        public string LeaderAccount { get; set; }
        public string LeaderEID { get; set; }
        public Nullable<System.Guid> LeaderID { get; set; }
        public string TitleCode { get; set; }
        public string TitleZHCN { get; set; }
        public string TitleENUS { get; set; }
        public string PositionCode { get; set; }
        public string PositionZHCN { get; set; }
        public string PositionENUS { get; set; }
        public string HCRegionZHCN { get; set; }
        public string HCRegionENUS { get; set; }
        public string HCMarketZHCN { get; set; }
        public string HCMarketENUS { get; set; }
        public string HCCityZHCN { get; set; }
        public string HCCityENUS { get; set; }
        public string CompanyEntityZHCN { get; set; }
        public string CompanyEntityENUS { get; set; }
        public string Scope { get; set; }
        public string ScopeZHCN { get; set; }
        public string ScopeENUS { get; set; }
        public string ScopeLevel { get; set; }
        public string ScopeLevelZHCN { get; set; }
        public string ScopeLevelENUS { get; set; }
        public string FromDepartmentZHCN { get; set; }
        public string FromDepartmentENUS { get; set; }
        public string HCFunctionZHCN { get; set; }
        public string HCFunctionENUS { get; set; }
        public string FunctionZHCN { get; set; }
        public string FunctionENUS { get; set; }
        public string JoinFromZHCN { get; set; }
        public string JoinFromENUS { get; set; }
        public string DepartmentZHCN { get; set; }
        public string DepartmentENUS { get; set; }
        public string ReportToAccount { get; set; }
        public string ReportToEID { get; set; }
        public string ReportToID { get; set; }
        public string RoleCode { get; set; }
        public List<string> RightCodes { get; set; }
    }
}