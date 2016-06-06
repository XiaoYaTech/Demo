/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   8/1/2014 2:07:14 AM
 * FileName     :   ClosureToolImpactOtherStore
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public partial class ClosureToolImpactOtherStore : BaseEntity<ClosureToolImpactOtherStore>
    {
        public static List<ClosureToolImpactOtherStore> SearList(Guid closureId)
        {
            var list = Search(e => e.ClosureId == closureId).ToList();
            return list;
        }
    }
}
