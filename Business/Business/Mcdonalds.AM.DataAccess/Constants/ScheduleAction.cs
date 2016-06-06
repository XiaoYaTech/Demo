using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess
{
    public class ScheduleAction
    {
        /// <summary>
        /// 更新店面状态
        /// </summary>
        public const string Update = "Update";

        /// <summary>
        /// 生成定时任务
        /// </summary>
        public const string Generate = "Generate";
    }
}
