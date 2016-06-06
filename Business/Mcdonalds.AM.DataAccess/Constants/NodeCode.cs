using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mcdonalds.AM.DataAccess.Constants
{
    public class NodeCode
    {
        public const string Start = "Start";
        public const string Finish = "Finish";
        #region Closure
        public const string Closure_WOCheckList_DownLoadTemplate = "Closure_WOCheckList_DownLoadTemplate";
        public const string Closure_WOCheckList_WriteOffData = "Closure_WOCheckList_WriteOffData";
        public const string Closure_WOCheckList_ResultUpload = "Closure_WOCheckList_ResultUpload";
        public const string Closure_WOCheckList_ClosingCost = "Closure_WOCheckList_ClosingCost";
        public const string Closure_WOCheckList_Approve = "Closure_WOCheckList_Approve";

        public const string Closure_ClosureTool_FinInput = "Closure_ClosureTool_FinInput";
        public const string Closure_ClosureTool_FinApprove = "Closure_ClosureTool_FinApprove";
        public const string Closure_ClosureTool_RepInput = "Closure_ClosureTool_RepInput";
        public const string Closure_ClosureTool_Generate = "Closure_ClosureTool_Generate";

        public const string Closure_LegalReview_Input = "Closure_LegalReview_Input";
        public const string Closure_LegalReview_UploadAgreement = "Closure_LegalReview_UploadAgreement";
        public const string Closure_LegalReview_LegalConfirm = "Closure_LegalReview_LegalConfirm";

        public const string Closure_ExecutiveSummary_Input = "Closure_ExecutiveSummary_Input";
        public const string Closure_ExecutiveSummary_Generate = "Closure_ExecutiveSummary_Generate";

        public const string Closure_ClosurePackage_Input = "Closure_ClosurePackage_Input";
        public const string Closure_ClosurePackage_OnlineApprove = "Closure_ClosurePackage_OnlineApprove";
        public const string Closure_ClosurePackage_ResourceUpload = "Closure_ClosurePackage_ResourceUpload";

        public const string Closure_ContractInfo_CheckInfo = "Closure_ContractInfo_CheckInfo";

        public const string Closure_ClosureMemo_Input = "Closure_ClosureMemo_Input";
        public const string Closure_ClosureMemo_SendMemo = "Closure_ClosureMemo_SendMemo";


        public const string Closure_ConsInvtChecking_DownLoadTemplate = "Closure_ConsInvtChecking_DownLoadTemplate";
        public const string Closure_ConsInvtChecking_WriteOffData = "Closure_ConsInvtChecking_WriteOffData";
        public const string Closure_ConsInvtChecking_ResultUpload = "Closure_ConsInvtChecking_ResultUpload";
        public const string Closure_ConsInvtChecking_Approve = "Closure_ConsInvtChecking_Approve";
        #endregion

        #region MajorLease
        public const string MajorLease_LegalReview_Input = "MajorLease_LegalReview_Input";
        public const string MajorLease_LegalReview_Upload = "MajorLease_LegalReview_Upload";
        public const string MajorLease_LegalReview_Confirm = "MajorLease_LegalReview_Confirm";

        public const string MajorLease_FinanceAnalysis_Input = "MajorLease_FinanceAnalysis_Input";
        public const string MajorLease_FinanceAnalysis_Upload = "MajorLease_FinanceAnalysis_Upload";
        public const string MajorLease_FinanceAnalysis_Confirm = "MajorLease_FinanceAnalysis_Confirm";

        public const string MajorLease_ConsInfo_Input = "MajorLease_ConsInfo_Input";
        public const string MajorLease_ConsInfo_Upload = "MajorLease_ConsInfo_Upload";
        public const string MajorLease_ConsInfo_Confirm = "MajorLease_ConsInfo_Confirm";

        public const string MajorLease_Package_Input = "MajorLease_Package_Input";
        public const string MajorLease_Package_Examine = "MajorLease_Package_Examine";
        public const string MajorLease_Package_Upload = "MajorLease_Package_Upload";

        public const string MajorLease_ContractInfo_CheckInfo = "MajorLease_ContractInfo_CheckInfo";
        public const string MajorLease_SiteInfo_CheckInfo = "MajorLease_SiteInfo_CheckInfo";

        public const string MajorLease_ConsInvtChecking_Downlod = "MajorLease_ConsInvtChecking_Downlod";
        public const string MajorLease_ConsInvtChecking_Input = "MajorLease_ConsInvtChecking_Input";
        public const string MajorLease_ConsInvtChecking_Upload = "MajorLease_ConsInvtChecking_Upload";
        public const string MajorLease_ConsInvtChecking_Confirm = "MajorLease_ConsInvtChecking_Confirm";

        public const string MajorLease_GBMemo_Input = "MajorLease_GBMemo_Input";
        public const string MajorLease_GBMemo_Send = "MajorLease_GBMemo_Send";
        #endregion

        #region Reimage
        public const string Reimage_ConsInfo_Input = "Reimage_ConsInfo_Input";
        public const string Reimage_ConsInfo_Upload = "Reimage_ConsInfo_Upload";
        public const string Reimage_ConsInfo_Confirm = "Reimage_ConsInfo_Confirm";

        public const string Reimage_Summary_Push = "Reimage_Summary_Push";
        public const string Reimage_Summary_Input = "Reimage_Summary_Input";
        public const string Reimage_Summary_Generate = "Reimage_Summary_Generate";

        public const string Reimage_Package_Input = "Reimage_Package_Input";
        public const string Reimage_Package_Examine = "Reimage_Package_Examine";
        public const string Reimage_Package_Download = "Reimage_Package_Download";
        public const string Reimage_Package_Upload = "Reimage_Package_Upload";

        public const string Reimage_SiteInfo_CheckInfo = "Reimage_SiteInfo_CheckInfo";

        public const string Reimage_ConsInvtChecking_Downlod = "Reimage_ConsInvtChecking_Downlod";
        public const string Reimage_ConsInvtChecking_Input = "Reimage_ConsInvtChecking_Input";
        public const string Reimage_ConsInvtChecking_Upload = "Reimage_ConsInvtChecking_Upload";
        public const string Reimage_ConsInvtChecking_Confirm = "Reimage_ConsInvtChecking_Confirm";

        public const string Reimage_GBMemo_Input = "Reimage_GBMemo_Input";
        public const string Reimage_GBMemo_SendMemo = "Reimage_GBMemo_SendMemo";

        public const string Reimage_ReopenMemo_Input = "Reimage_ReopenMemo_Input";
        public const string Reimage_ReopenMemo_SendMemo = "Reimage_ReopenMemo_SendMemo";

        public const string Reimage_TempClosureMemo_Input = "Reimage_TempClosureMemo_Input";
        public const string Reimage_TempClosureMemo_Send = "Reimage_TempClosureMemo_Send";

        #endregion

        #region Temp Closure
        public const string TempClosure_LegalReview_Input = "TempClosure_LegalReview_Input";
        //public const string TempClosure_LegalReview_Upload = "TempClosure_LegalReview_Upload";
        public const string TempClosure_LegalReview_Approve = "TempClosure_LegalReview_Approve";

        public const string TempClosure_ClosurePackage_Input = "TempClosure_ClosurePackage_Input";
        public const string TempClosure_ClosurePackage_Approve = "TempClosure_ClosurePackage_Approve";
        public const string TempClosure_ClosurePackage_Upload = "TempClosure_ClosurePackage_Upload";

        public const string TempClosure_ClosureMemo_Input = "TempClosure_ClosureMemo_Input";
        public const string TempClosure_ClosureMemo_Send = "TempClosure_ClosureMemo_Send";

        public const string TempClosure_ReopenMemo_Input = "TempClosure_ReopenMemo_Input";
        public const string TempClosure_ReopenMemo_Send = "TempClosure_ReopenMemo_Send";
        #endregion

        #region Rebuild
        public const string Rebuild_LegalReview_Input = "Rebuild_LegalReview_Input";
        public const string Rebuild_LegalReview_Upload = "Rebuild_LegalReview_Upload";
        public const string Rebuild_LegalReview_Confirm = "Rebuild_LegalReview_Confirm";

        public const string Rebuild_FinanceAnalysis_Input = "Rebuild_FinanceAnalysis_Input";
        public const string Rebuild_FinanceAnalysis_Upload = "Rebuild_FinanceAnalysis_Upload";
        public const string Rebuild_FinanceAnalysis_Confirm = "Rebuild_FinanceAnalysis_Confirm";

        public const string Rebuild_ConsInfo_Input = "Rebuild_ConsInfo_Input";
        public const string Rebuild_ConsInfo_Upload = "Rebuild_ConsInfo_Upload";
        public const string Rebuild_ConsInfo_Confirm = "Rebuild_ConsInfo_Confirm";

        public const string Rebuild_Package_Input = "Rebuild_Package_Input";
        public const string Rebuild_Package_Examine = "Rebuild_Package_Examine";
        public const string Rebuild_Package_Upload = "Rebuild_Package_Upload";

        public const string Rebuild_ContractInfo_CheckInfo = "Rebuild_ContractInfo_CheckInfo";
        public const string Rebuild_SiteInfo_CheckInfo = "Rebuild_SiteInfo_CheckInfo";

        public const string Rebuild_ConsInvtChecking_Downlod = "Rebuild_ConsInvtChecking_Downlod";
        public const string Rebuild_ConsInvtChecking_Input = "Rebuild_ConsInvtChecking_Input";
        public const string Rebuild_ConsInvtChecking_Upload = "Rebuild_ConsInvtChecking_Upload";
        public const string Rebuild_ConsInvtChecking_Confirm = "Rebuild_ConsInvtChecking_Confirm";

        public const string Rebuild_ReopenMemo_Input = "Rebuild_ReopenMemo_Input";
        public const string Rebuild_ReopenMemo_Send = "Rebuild_ReopenMemo_Send";

        public const string Rebuild_GBMemo_Input = "Rebuild_GBMemo_Input";
        public const string Rebuild_GBMemo_Send = "Rebuild_GBMemo_Send";

        public const string Rebuild_TempClosureMemo_Input = "Rebuild_TempClosureMemo_Input";
        public const string Rebuild_TempClosureMemo_Send = "Rebuild_TempClosureMemo_Send";
        #endregion

        #region Renewal

        public const string Renewal_Letter_Upload = "Renewal_Letter_Upload";
        public const string Renewal_Letter_Confirm = "Renewal_Letter_Confirm";

        public const string Renewal_LLNegotiation_ConfirmLetter = "Renewal_LLNegotiation_ConfirmLetter";
        public const string Renewal_LLNegotiation_Input = "Renewal_LLNegotiation_Input";

        public const string Renewal_ConsInfo_Input = "Renewal_ConsInfo_Input";
        public const string Renewal_ConsInfo_Approval = "Renewal_ConsInfo_Approval";

        public const string Renewal_Tool_Input = "Renewal_Tool_Input";
        public const string Renewal_Tool_Upload = "Renewal_Tool_Upload";
        public const string Renewal_Tool_Approval = "Renewal_Tool_Approval";
        public const string Renewal_Tool_UploadTool = "Renewal_Tool_UploadTool";

        public const string Renewal_ClearanceReport_Upload = "Renewal_ClearanceReport_Upload";

        public const string Renewal_ConfirmLetter_Upload = "Renewal_ConfirmLetter_Upload";

        public const string Renewal_Analysis_Input = "Renewal_Analysis_Input";

        public const string Renewal_LegalApproval_Input = "Renewal_LegalApproval_Input";
        public const string Renewal_LegalApproval_LegalReview = "Renewal_LegalApproval_LegalReview";
        public const string Renewal_LegalApproval_Approval = "Renewal_LegalApproval_Approval";

        public const string Renewal_Package_Input = "Renewal_Package_Input";
        public const string Renewal_Package_Approval = "Renewal_Package_Approval";

        public const string Renewal_GBMemo_Input = "Renewal_GBMemo_Input";
        public const string Renewal_GBMemo_Send = "Renewal_GBMemo_Send";

        public const string Renewal_ContractInfo_CheckInfo = "Renewal_ContractInfo_CheckInfo";
        public const string Renewal_SiteInfo_CheckInfo = "Renewal_SiteInfo_CheckInfo";
        #endregion
    }
}
