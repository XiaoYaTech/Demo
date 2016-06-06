using Mcdonalds.AM.DataAccess.DataTransferObjects;
using Mcdonalds.AM.DataAccess.Infrastructure;
using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using System.Linq.Expressions;

namespace Mcdonalds.AM.DataAccess
{
    public partial class StoreBasicInfo : BaseEntity<StoreBasicInfo>
    {

        public ProjectContractRevision ProjectContractRevision { get; set; }
        public string Region
        {
            get
            {
                return I18N.GetValue(this, "Region");
            }
        }

        public string Market
        {
            get
            {
                return I18N.GetValue(this, "Market");
            }
        }

        /// <summary>
        /// 根据不同的区域类型，查找所属区域的店
        /// </summary>
        /// <param name="scopeType"></param>
        /// <param name="regionCode"></param>
        /// <returns></returns>
        public static List<StoreBasicInfo> GetStoresByRegionCode(RegionScopeType scopeType, string regionCode)
        {
            var store = new StoreBasicInfo();
            List<StoreBasicInfo> items = new List<StoreBasicInfo>();
            switch (scopeType)
            {
                case RegionScopeType.MCCL:
                    items = StoreBasicInfo.Search(s => true).OrderBy(s => s.NameENUS).AsNoTracking().ToList();
                    break;
                case RegionScopeType.Region:
                    items = StoreBasicInfo.Search(s => s.RegionCode.Equals(regionCode)).OrderBy(s => s.NameENUS).AsNoTracking().ToList();
                    break;
                case RegionScopeType.Market:
                    items = StoreBasicInfo.Search(s => s.MarketCode.Equals(regionCode)).OrderBy(s => s.NameENUS).AsNoTracking().ToList();
                    break;
                case RegionScopeType.City:
                    items = StoreBasicInfo.Search(s => s.CityCode.Equals(regionCode)).OrderBy(s => s.NameENUS).AsNoTracking().ToList();
                    break;
                default:
                    break;
            }
            return items;
        }

        /// <summary>
        /// 获取用户所辖店的信息
        /// </summary>
        /// <param name="employeeCode">员工EID</param>
        /// <param name="pageIndex">页面索引</param>
        /// <param name="pageSize">页面大小</param>
        /// <param name="total">总数</param>
        /// <returns></returns>
        public List<StoreBasicInfo> GetStoresByEmployeeCode(int pageIndex, int pageSize, string employeeCode, string storeName, string storeCode, out int total)
        {
            List<StoreBasicInfo> stores = null;
            //var storeEntity = new StoreBasicInfo();
            total = 0;
            //获取用户所属的区域范围信息
            //var employee = Employee.GetEmployeeByCode(employeeCode);
            //if (employee != null)
            //{
            //    var regionScope = employee.ScopeENUS;
            //    switch (regionScope)
            //    {
            //        case RegionScope.MCCL:
            //            stores = storeEntity.Search(s => (string.IsNullOrEmpty(storeCode) || s.StoreCode.StartsWith(storeCode) && ((string.IsNullOrEmpty(storeName) || s.NameENUS.Contains(storeName) || s.NameZHCN.Contains(storeName)))), p => p.NameENUS, 1, pageSize, out total);
            //            break;
            //        case RegionScope.Region:
            //            stores = storeEntity.Search(s => s.RegionCode.Equals(employee.HCRegion) && (string.IsNullOrEmpty(storeCode) || s.StoreCode.StartsWith(storeCode)) && ((string.IsNullOrEmpty(storeName) || s.NameENUS.Contains(storeName) || s.NameZHCN.Contains(storeName))), p => p.NameENUS, 1, pageSize, out total);
            //            break;
            //        case RegionScope.Market:
            //            stores = storeEntity.Search(s => s.MarketCode.Equals(employee.HCMarket) && (string.IsNullOrEmpty(storeCode) || s.StoreCode.StartsWith(storeCode)) && ((string.IsNullOrEmpty(storeName) || s.NameENUS.Contains(storeName) || s.NameZHCN.Contains(storeName))), p => p.NameENUS, 1, pageSize, out total);
            //            break;
            //        case RegionScope.City:
            //            stores = storeEntity.Search(s => s.CityCode.Equals(employee.HCCity) && (string.IsNullOrEmpty(storeCode) || s.StoreCode.StartsWith(storeCode)) && ((string.IsNullOrEmpty(storeName) || s.NameENUS.Contains(storeName) || s.NameZHCN.Contains(storeName))), p => p.NameENUS, 1, pageSize, out total);
            //            break;
            //        default:
            //            break;
            //    }
            //}
            //return stores;

            var currentUser = Employee.GetSimpleEmployeeByCode(employeeCode);
            if (currentUser.PositionCode.Equals(PositionCodeConstants.Asset_Rep) || currentUser.PositionCode.Equals(PositionCodeConstants.Asset_Mgr))
            {
                var db = GetDb();
                string where = string.Format(@" StoreCode in (select StoreCode from StoreDevelop 
where AssetRepEid='{0}' or AssetMgrEid='{0}')", employeeCode);
                if (!string.IsNullOrEmpty(storeCode))
                    where += " and StoreCode like '%" + storeCode + "%'";
                if (!string.IsNullOrEmpty(storeName))
                    where += " and (NameZHCN like '%" + storeName + "%' OR NameENUS like '%" + storeName + "%')";
                total = db.Database.SqlQuery<int>(string.Format(@"select count(id) FROM StoreBasicInfo where {0}", where)).ToArray()[0];
                stores = db.Database.SqlQuery<StoreBasicInfo>(string.Format(@"
select top {1} * from StoreBasicInfo where {0}
and StoreCode not in (
select top {2} StoreCode from StoreBasicInfo
where {0} order by StoreCode)
order by StoreCode", where, pageSize, pageSize * (pageIndex - 1))).ToList();
            }
            return stores;
        }

        public static IEnumerable<StoreBasicInfo> GetStoreList(Expression<Func<StoreBasicInfo, bool>> predicate)
        {
            var db = PrepareDb();
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
            IEnumerable<StoreBasicInfo> stores;
            var emp = Employee.GetEmployeeByCode(ClientCookie.UserCode);
            var storesByRole = from s in db.StoreBasicInfo
                               from sd in db.StoreDevelop
                               where sd.StoreCode == s.StoreCode
                                    && sd.AssetRepEid == ClientCookie.UserCode
                               select s;
            if (haveScope)
            {
                var storesByScope = from s in db.StoreBasicInfo
                                    from um in db.SysUserMMMap
                                    from e in db.Employee
                                    where e.Code == ClientCookie.UserCode && s.MMCode == um.MMCode && um.UserID == e.C_OldId
                                    select s;

                stores = storesByScope.Union(storesByRole);
            }
            else if (emp.PositionCode == "suoya303055")//点对点 AssetRep
            {
                stores = storesByRole;
            }
            else
            {
                stores = db.StoreBasicInfo.Where(e => true);
            }
            return stores.AsQueryable().Where(predicate);
        }

        public List<StoreBasicInfo> GetStoresByAssetRepOrMgrEID(int pageSize, string employeeCode, string storeCode, string storeName,string flowCode = null)
        {
            var stores = new List<StoreBasicInfo>();
            var currentUser = Employee.GetSimpleEmployeeByCode(employeeCode);
            var db = GetDb();
            if (db.SysRoleUserMap.Count(r => r.EID == currentUser.Code && (
                r.RoleID == (int)RoleCode.Market_Asset_Mgr || r.RoleID == (int)RoleCode.Regional_Asset_Mgr)) > 0)//Asset Mgr
            {
                string where = string.Format(" r.RoleID in ({1},{2}) AND e.Code='{0}' {3}"
                    , currentUser.Code
                    , (int)RoleCode.Market_Asset_Mgr
                    , (int)RoleCode.Regional_Asset_Mgr
                    , string.Compare(flowCode, "Renewal", true) == 0 ? "" : " and s.statusName = 'Opened'");
                List<string> storeCodes = db.Database.SqlQuery<string>(@"
select distinct s.StoreCode from StoreBasicInfo s
join SysUserMMMap m on s.MMCode=m.MMCode
join SysRolePositionMap rp on m.PositionID=rp.PositionID
join SysRole r on r.RoleID=rp.RoleID
join Position p on p.PositionID=rp.PositionID
join Employee e on p.UserID=e.C_OldId 
where " + where).ToList();
                var results = from s in db.StoreBasicInfo
                              where storeCodes.Contains(s.StoreCode)
                              && (string.IsNullOrEmpty(storeCode) || s.StoreCode.StartsWith(storeCode))
                              && (string.IsNullOrEmpty(storeName) || s.NameENUS.Contains(storeName) || s.NameZHCN.Contains(storeName))
                              select s;
                stores = results.Take(pageSize).OrderBy(s => s.StoreCode).ToList();
            }
            else if (db.SysRoleUserMap.Count(r => r.EID == currentUser.Code && r.RoleID == (int)RoleCode.Asset_Rep) > 0)//Asset Rep
            {
                var results = from s in db.StoreBasicInfo
                              from sd in db.StoreDevelop
                              where sd.StoreCode == s.StoreCode
                              && (string.Compare(flowCode, "Renewal", true) == 0 || s.statusName == "Opened")
                              && sd.AssetRepEid.Equals(employeeCode)
                              && (string.IsNullOrEmpty(storeCode) || s.StoreCode.StartsWith(storeCode))
                              && (string.IsNullOrEmpty(storeName) || s.NameENUS.Contains(storeName) || s.NameZHCN.Contains(storeName))
                              select s;
                stores = results.Take(pageSize).OrderBy(s => s.StoreCode).ToList();
            }
            return stores;
        }

        public bool CheckStore(string storeCode, out string storeName)
        {
            var result = false;
            var currentUser = Employee.GetSimpleEmployeeByCode(ClientCookie.UserCode);
            var db = GetDb();
            if (db.SysRoleUserMap.Count(r => r.EID == currentUser.Code && (
                r.RoleID == (int)RoleCode.Market_Asset_Mgr || r.RoleID == (int)RoleCode.Regional_Asset_Mgr)) > 0)//Asset Mgr
            {
                var results = db.Database.SqlQuery<StoreBasicInfo>(string.Format(@"
select * from StoreBasicInfo s
join SysUserMMMap m on s.MMCode=m.MMCode
join SysRolePositionMap rp on m.PositionID=rp.PositionID
join SysRole r on r.RoleID=rp.RoleID
join Position p on p.PositionID=rp.PositionID
join Employee e on p.UserID=e.C_OldId and s.statusName = 'Opened'
where r.RoleID in ({1},{2}) AND e.Code='{0}' and s.Code='{2}'", currentUser.Code, (int)RoleCode.Market_Asset_Mgr, storeCode, (int)RoleCode.Regional_Asset_Mgr)).ToList();
                result = results.Any();
                storeName = results.Select(s => s.NameZHCN + "(" + s.NameENUS + ")").FirstOrDefault();
            }
            else if (db.SysRoleUserMap.Count(r => r.EID == currentUser.Code && r.RoleID == (int)RoleCode.Asset_Rep) > 0)//Asset Rep
            {
                var results = from s in db.StoreBasicInfo
                              from sd in db.StoreDevelop
                              where sd.StoreCode == s.StoreCode
                              && sd.AssetRepEid.Equals(currentUser.Code)
                              && s.StoreCode == storeCode
                              && s.statusName == "Opened"
                              select s;
                result = results.Any();
                storeName = results.Select(s => s.NameZHCN + "(" + s.NameENUS + ")").FirstOrDefault();
            }
            else
            {
                storeName = "";
            }
            return result;
        }

        /// <summary>
        /// 根据用户的Code获取store的详细信息
        /// </summary>
        /// <param name="employeeCode"></param>
        /// <param name="storeCode"></param>
        /// <returns></returns>
        public StoreInfo GetStoreDetailsByEID(string employeeCode, string storeCode)
        {
            var store = GetStore(storeCode);
            bool valid = false;
            var currentUser = Employee.GetEmployeeByCode(employeeCode);
            if (store != null && currentUser != null)
            {
                var regionScope = currentUser.ScopeENUS;
                switch (regionScope)
                {
                    case RegionScope.MCCL:
                        if (currentUser.PositionCode.Equals(PositionCodeConstants.Asset_Mgr) || store.StoreDevelop.AssetRepEid.Equals(employeeCode)) valid = true;
                        break;
                    case RegionScope.Region:
                        if ((currentUser.PositionCode.Equals(PositionCodeConstants.Asset_Mgr) && store.StoreBasicInfo.RegionCode.Equals(currentUser.HCRegion)) || store.StoreDevelop.AssetRepEid.Equals(employeeCode)) valid = true;
                        break;
                    case RegionScope.Market:
                        if ((currentUser.PositionCode.Equals(PositionCodeConstants.Asset_Mgr) && store.StoreBasicInfo.MarketCode.Equals(currentUser.HCMarket)) || store.StoreDevelop.AssetRepEid.Equals(employeeCode)) valid = true;
                        break;
                    case RegionScope.City:
                        if ((currentUser.PositionCode.Equals(PositionCodeConstants.Asset_Mgr) && store.StoreBasicInfo.CityCode.Equals(currentUser.HCCity)) || store.StoreDevelop.AssetRepEid.Equals(employeeCode)) valid = true;
                        break;
                    default:
                        valid = false;
                        break;
                }
            }

            return valid ? store : null;
        }
    }
}
