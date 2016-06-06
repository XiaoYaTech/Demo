using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.Entities.Closure.Enum
{
    public enum ClosureToolStatus
    {
       /// <summary>
       /// 创建
       /// </summary>
       Create = 0,
     
        /// <summary>
        /// 财务审批
        /// </summary>
       FinApprove = 1,
       /// <summary>
       /// 生成Tool
       /// </summary>
       GenerateTool = 3,
       /// <summary>
       /// 审批
       /// </summary>
       Approve = 4,
       /// <summary>
       /// 完成
       /// </summary>
       Finish = 200
    }
}
