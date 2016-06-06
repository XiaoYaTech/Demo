using Mcdonalds.AM.Services.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class VStoreList : BaseEntity<VStoreList>
    {
        public static IEnumerable<VStoreList> GetStoreList(Expression<Func<VStoreList, bool>> predicate)
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
            IEnumerable<VStoreList> stores;
            var emp = Employee.GetEmployeeByCode(ClientCookie.UserCode);
            var storesByRole = from s in db.VStoreList
                               from sd in db.StoreDevelop
                               where sd.StoreCode == s.StoreCode
                                    && sd.AssetRepEid == ClientCookie.UserCode
                               select s;
            if (haveScope)
            {
                var storesByScope = from s in db.VStoreList
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
                stores = db.VStoreList.Where(e => true);
            }
            return stores.AsQueryable().Where(predicate);
        }
    }
}
