using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.Services.Common
{
    public enum HtmlTempalteType : int
    {
        /// <summary>
        /// 默认模板
        /// </summary>
        Default = 0,

        /// <summary>
        /// 默认模板-附加Comments
        /// </summary>
        DefaultAdding = 1,

        /// <summary>
        /// 默认模板-附加Comments
        /// </summary>
        DefaultWithComments = 2,

        /// <summary>
        /// ClosureMemo模板
        /// </summary>
        ClosureMemo = 100,

        /// <summary>
        /// GBMemo模板
        /// </summary>
        GBMemo = 101,

        /// <summary>
        /// ReopenMemo模板
        /// </summary>
        ReopenMemo = 102,

        /// <summary>
        /// Major Lease模板
        /// </summary>
        MajorLease = 200,

        /// <summary>
        /// Temp Closure模板
        /// </summary>
        TempClosure = 300,

        /// <summary>
        /// Temp Closure Reopen Memo模板
        /// </summary>
        TempClosureReopenMemo = 301,

        /// <summary>
        /// Rebuild模板
        /// </summary>
        Rebuild = 400,

        /// <summary>
        /// Rebuild模板
        /// </summary>
        RebuildPackage = 401,

        /// <summary>
        /// Reimage模板
        /// </summary>
        Reimage = 500,

        /// <summary>
        /// Renewal模板
        /// </summary>
        Renewal = 600,

        /// <summary>
        /// RenewalLegalApproval模板
        /// </summary>
        RenewalLegalApproval = 601,
    }

    /// <summary>
    /// 流程日志类型
    /// </summary>
    public enum ActionLogType
    {
        /// <summary>
        /// Reject
        /// </summary>
        Decline,

        /// <summary>
        /// Approve
        /// </summary>
        Approve,

        /// <summary>
        /// Return
        /// </summary>
        Return,

        /// <summary>
        /// Save
        /// </summary>
        Save,

        /// <summary>
        /// Record
        /// </summary>
        Recall,

        /// <summary>
        /// Submit
        /// </summary>
        Submit,

        /// <summary>
        /// ReSubmit
        /// </summary>
        ReSubmit,

        /// <summary>
        /// Comments
        /// </summary>
        Comments,

        /// <summary>
        /// None
        /// </summary>
        None
    }
}