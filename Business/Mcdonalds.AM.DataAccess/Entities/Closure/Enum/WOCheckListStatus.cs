using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.Entities.Closure.Enum
{
   public enum WOCheckListStatus
    {
       /// <summary>
       /// 创建
       /// </summary>
       Create = 0,
       /// <summary>
       /// 线下填写
       /// </summary>
       WriteOff = 1,
        /// <summary>
        /// 模板上传
        /// </summary>
       Uploaded = 2,
       /// <summary>
       /// ClosingCost
       /// </summary>
       ClosingCost =3,
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
