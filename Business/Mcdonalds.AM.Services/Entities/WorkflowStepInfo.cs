using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.Services
{
    [Serializable]
    public class WorkflowStepInfo
    {
        /// <summary>
        /// 操作人的ID，即写入Comments的人的EID
        /// </summary>
        public string OperatorID { set; get; }

        /// <summary>
        /// K2流程序列号
        /// </summary>
        public string SN { get; set; }

        /// <summary>
        /// 业务数据实体表中的主键ID，是QUID.
        /// </summary>
        public string EntityID { set; get; }

        /// <summary>
        /// 工作流名称
        /// </summary>
        public string WorkflowName { set; get; }

        /// <summary>
        /// 操作名称
        /// </summary>
        public string ActionName { set; get; }

        /// <summary>
        /// 批注意见
        /// </summary>
        public string Comments { set; get; }
    }
}