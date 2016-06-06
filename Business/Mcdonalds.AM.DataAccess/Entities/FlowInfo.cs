/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   7/29/2014 10:54:21 AM
 * FileName     :   FlowInfo
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
    public partial class FlowInfo : BaseEntity<FlowInfo>
    {
        public static FlowInfo Get(string flowCode)
        {
            return FirstOrDefault(p => p.Code == flowCode);
        }
        public static IEnumerable<FlowInfo> GetFlowList(string parentCode)
        {
            return Search(e => e.ParentCode.Equals(parentCode));
        }
    }
}
