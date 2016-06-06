using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.Services.Entities
{
    [Serializable]
    public class WorkflowEmailComment
    {
        /// <summary>
        /// K2 流程的SN
        /// </summary>
        public string K2SN { get; set; }

        /// <summary>
        /// 当前操作人EID
        /// </summary>
        public string OperatorID { get; set; }

        /// <summary>
        /// 当前操作人EID
        /// </summary>
        public string Action { get; set; }

        /// <summary>
        /// 邮件中的审批意见
        /// </summary>
        public string EmailComments {get; set; }
    }
}