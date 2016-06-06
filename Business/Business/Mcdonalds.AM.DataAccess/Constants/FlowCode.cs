namespace Mcdonalds.AM.DataAccess.Constants
{
    public class FlowCode
    {



        #region Closure
        public const string Closure = "Closure";
        public const string Closure_WOCheckList = "Closure_WOCheckList";
        public const string Closure_ClosureTool = "Closure_ClosureTool";
        public const string Closure_LegalReview = "Closure_LegalReview";
        public const string Closure_ExecutiveSummary = "Closure_ExecutiveSummary";
        public const string Closure_ClosurePackage = "Closure_ClosurePackage";
        public const string Closure_ContractInfo = "Closure_ContractInfo";
        public const string Closure_Memo = "Closure_ClosureMemo";
        public const string Closure_ConsInvtChecking = "Closure_ConsInvtChecking";
        #endregion

        #region Temp Closure
        public const string TempClosure = "TempClosure";
        public const string TempClosure_LegalReview = "TempClosure_LegalReview";
        public const string TempClosure_ClosurePackage = "TempClosure_ClosurePackage";
        public const string TempClosure_ClosureMemo = "TempClosure_ClosureMemo";
        public const string TempClosure_ReopenMemo = "TempClosure_ReopenMemo";
        #endregion

        #region MajorLease

        public const string MajorLease = "MajorLease";
        public const string MajorLease_LegalReview = "MajorLease_LegalReview";
        public const string MajorLease_FinanceAnalysis = "MajorLease_FinanceAnalysis";
        public const string MajorLease_ConsInfo = "MajorLease_ConsInfo";
        public const string MajorLease_Package = "MajorLease_Package";
        public const string MajorLease_ContractInfo = "MajorLease_ContractInfo";
        public const string MajorLease_SiteInfo = "MajorLease_SiteInfo";
        public const string MajorLease_ConsInvtChecking = "MajorLease_ConsInvtChecking";
        public const string MajorLease_GBMemo = "MajorLease_GBMemo";
        #endregion

        #region Reimage
        public const string Reimage = "Reimage";
        public const string Reimage_ConsInfo = "Reimage_ConsInfo";
        public const string Reimage_Summary = "Reimage_Summary";
        public const string Reimage_Package = "Reimage_Package";
        public const string Reimage_SiteInfo = "Reimage_SiteInfo";
        public const string Reimage_ConsInvtChecking = "Reimage_ConsInvtChecking";
        public const string Reimage_GBMemo = "Reimage_GBMemo";
        public const string Reimage_ReopenMemo = "Reimage_ReopenMemo";
        public const string Reimage_TempClosureMemo = "Reimage_TempClosureMemo";
        #endregion

        #region Rebuild
        public const string Rebuild = "Rebuild";
        public const string Rebuild_LegalReview = "Rebuild_LegalReview";
        public const string Rebuild_FinanceAnalysis = "Rebuild_FinanceAnalysis";
        public const string Rebuild_ConsInfo = "Rebuild_ConsInfo";
        public const string Rebuild_Package = "Rebuild_Package";
        public const string Rebuild_ContractInfo = "Rebuild_ContractInfo";
        public const string Rebuild_SiteInfo = "Rebuild_SiteInfo";
        public const string Rebuild_ConsInvtChecking = "Rebuild_ConsInvtChecking";
        public const string Rebuild_TempClosureMemo = "Rebuild_TempClosureMemo";
        public const string Rebuild_GBMemo = "Rebuild_GBMemo";
        public const string Rebuild_ReopenMemo = "Rebuild_ReopenMemo";
        #endregion

        #region Renewal
        public const string Renewal = "Renewal";
        public const string Renewal_Letter = "Renewal_Letter";
        public const string Renewal_LLNegotiation = "Renewal_LLNegotiation";
        public const string Renewal_ConsInfo = "Renewal_ConsInfo";
        public const string Renewal_Tool = "Renewal_Tool";
        public const string Renewal_ClearanceReport = "Renewal_ClearanceReport";
        public const string Renewal_ConfirmLetter = "Renewal_ConfirmLetter";
        public const string Renewal_Analysis = "Renewal_Analysis";
        public const string Renewal_LegalApproval = "Renewal_LegalApproval";
        public const string Renewal_Package = "Renewal_Package";
        public const string Renewal_ContractInfo = "Renewal_ContractInfo";
        public const string Renewal_SiteInfo = "Renewal_SiteInfo";
        public const string Renewal_GBMemo = "Renewal_GBMemo";

        #endregion

        #region Attachments Memo
        public const string ClosureMemo = "ClosureMemo";
        public const string GBMemo = "GBMemo";
        public const string ReopenMemo = "ReopenMemo";
        #endregion
        public static string MappingTableName(string flowCode)
        {
            switch (flowCode)
            {
                case FlowCode.TempClosure_LegalReview:
                    return "TempClosureLegalReview";
                case FlowCode.TempClosure_ClosurePackage:
                    return "TempClosurePackage";
                default:
                    return "";
            }
        }

    }
}
