using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mcdonalds.AM.Services.Workflows.MajorLeaseChange
{
    public class LegalReview
    {
        #region ---- [ Constant Strings ] ----

        public const string ProcessName = @"MCDAMK2Project.MajorLeaseChange\LegalReview";
        public const string ProcessCode = @"MCD_AM_MLC_LR";
        public const string TableName = "MajorLeaseChange_LR";

        public const string Act_Originator = "Originator"; // 退回至发起人节点名
        #endregion
    }
}