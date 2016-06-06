angular.module("mcd.am.services.renewal", ["ngResource"]).factory("renewalService", [
    "$resource", function($resource) {
        return $resource(Utils.ServiceURI.Address() + "api/:module/:action", {}, {
            create: { method: "POST", params: { module: "renewal", action: "create" }, isArray: true },
            initLetterPage: { method: "GET", params: { module: "renewalLetter", action: "initPage" }, isArray: false },
            saveLetter: { method: "POST", params: { module: "renewalLetter", action: "save" }, isArray: false },
            submitLetter: { method: "POST", params: { module: "renewalLetter", action: "submit" }, isArray: false },
            approveLetter: { method: "POST", params: { module: "renewalLetter", action: "approve" }, isArray: false },
            returnLetter: { method: "POST", params: { module: "renewalLetter", action: "return" }, isArray: false },
            resubmitLetter: { method: "POST", params: { module: "renewalLetter", action: "resubmit" }, isArray: false },
            recallLetter: { method: "POST", params: { module: "renewalLetter", action: "recall" }, isArray: false },
            editLetter: { method: "POST", params: { module: "renewalLetter", action: "edit" }, isArray: false },
            initLLNegotiationPage: { method: "GET", params: { module: "renewalLLNegotiation", action: "initPage" }, isArray: false },
            saveLLNegoRecord: { method: "POST", params: { module: "renewalLLNegotiation", action: "saveRecord" }, isArray: false },
            getLLNegoRecords: { method: "GET", params: { module: "renewalLLNegotiation", action: "getRecords" }, isArray: false },
            deleteLLNegoRecord: { method: "POST", params: { module: "renewalLLNegotiation", action: "deleteRecord" }, isArray: false },
            getLLNegoRecAttachments: { method: "GET", params: { module: "renewalLLNegotiation", action: "getRecordAttachments" }, isArray: true },
            saveLLNegotiation: { method: "POST", params: { module: "renewalLLNegotiation", action: "save" }, isArray: false },
            submitLLNegotiation: { method: "POST", params: { module: "renewalLLNegotiation", action: "submit" }, isArray: false },
            resubmitLLNegotiation: { method: "POST", params: { module: "renewalLLNegotiation", action: "resubmit" }, isArray: false },
            recallLLNegotiation: { method: "POST", params: { module: "renewalLLNegotiation", action: "recall" }, isArray: false },
            editLLNegotiation: { method: "POST", params: { module: "renewalLLNegotiation", action: "edit" }, isArray: false },
            initConsInfoPage: { method: "GET", params: { module: "renewalConsInfo", action: "initPage" }, isArray: false },
            saveConsInfo: { method: "POST", params: { module: "renewalConsInfo", action: "save" }, isArray: false },
            submitConsInfo: { method: "POST", params: { module: "renewalConsInfo", action: "submit" }, isArray: false },
            approveConsInfo: { method: "POST", params: { module: "renewalConsInfo", action: "approve" }, isArray: false },
            returnConsInfo: { method: "POST", params: { module: "renewalConsInfo", action: "return" }, isArray: false },
            resubmitConsInfo: { method: "POST", params: { module: "renewalConsInfo", action: "resubmit" }, isArray: false },
            recallConsInfo: { method: "POST", params: { module: "renewalConsInfo", action: "recall" }, isArray: false },
            editConsInfo: { method: "POST", params: { module: "renewalConsInfo", action: "edit" }, isArray: false },
            initToolPage: { method: "GET", params: { module: "renewalTool", action: "initPage" }, isArray: false },
            getTTMFinanceData: { method: "GET", params: { module: "renewalTool", action: "getTTMFinanceData" }, isArray: false },
            getToolFinMeasureOutput: { method: "GET", params: { module: "renewalTool", action: "getFinMeasureOutput" }, isArray: false },
            saveTool: { method: "POST", params: { module: "renewalTool", action: "save" }, isArray: false },
            submitTool: { method: "POST", params: { module: "renewalTool", action: "submit" }, isArray: false },
            approveTool: { method: "POST", params: { module: "renewalTool", action: "approve" }, isArray: false },
            confirmUploadTool: { method: "POST", params: { module: "renewalTool", action: "confirmUploadTool" }, isArray: false },
            returnTool: { method: "POST", params: { module: "renewalTool", action: "return" }, isArray: false },
            resubmitTool: { method: "POST", params: { module: "renewalTool", action: "resubmit" }, isArray: false },
            recallTool: { method: "POST", params: { module: "renewalTool", action: "recall" }, isArray: false },
            editTool: { method: "POST", params: { module: "renewalTool", action: "edit" }, isArray: false },
            initAnalysisPage: { method: "GET", params: { module: "renewalAnalysis", action: "initPage" }, isArray: false },
            saveAnalysis: { method: "POST", params: { module: "renewalAnalysis", action: "save" }, isArray: false },
            submitAnalysis: { method: "POST", params: { module: "renewalAnalysis", action: "submit" }, isArray: false },
            resubmitAnalysis: { method: "POST", params: { module: "renewalAnalysis", action: "resubmit" }, isArray: false },
            recallAnalysis: { method: "POST", params: { module: "renewalAnalysis", action: "recall" }, isArray: false },
            editAnalysis: { method: "POST", params: { module: "renewalAnalysis", action: "edit" }, isArray: false },
            initClearanceReportPage: { method: "GET", params: { module: "renewalClearanceReport", action: "initPage" }, isArray: false },
            saveClearanceReport: { method: "POST", params: { module: "renewalClearanceReport", action: "save" }, isArray: false },
            submitClearanceReport: { method: "POST", params: { module: "renewalClearanceReport", action: "submit" }, isArray: false },
            resubmitClearanceReport: { method: "POST", params: { module: "renewalClearanceReport", action: "resubmit" }, isArray: false },
            recallClearanceReport: { method: "POST", params: { module: "renewalClearanceReport", action: "recall" }, isArray: false },
            editClearanceReport: { method: "POST", params: { module: "renewalClearanceReport", action: "edit" }, isArray: false },
            initConfirmLetterPage: { method: "GET", params: { module: "renewalConfirmLetter", action: "initPage" }, isArray: false },
            saveConfirmLetter: { method: "POST", params: { module: "renewalConfirmLetter", action: "save" }, isArray: false },
            submitConfirmLetter: { method: "POST", params: { module: "renewalConfirmLetter", action: "submit" }, isArray: false },
            resubmitConfirmLetter: { method: "POST", params: { module: "renewalConfirmLetter", action: "resubmit" }, isArray: false },
            recallConfirmLetter: { method: "POST", params: { module: "renewalConfirmLetter", action: "recall" }, isArray: false },
            editConfirmLetter: { method: "POST", params: { module: "renewalConfirmLetter", action: "edit" }, isArray: false },
            initLegalApprovalPage: { method: "GET", params: { module: "renewalLegalApproval", action: "initPage" }, isArray: false },
            saveLegalApproval: { method: "POST", params: { module: "renewalLegalApproval", action: "save" }, isArray: false },
            submitLegalApproval: { method: "POST", params: { module: "renewalLegalApproval", action: "submit" }, isArray: false },
            approveLegalApproval: { method: "POST", params: { module: "renewalLegalApproval", action: "approve" }, isArray: false },
            returnLegalApproval: { method: "POST", params: { module: "renewalLegalApproval", action: "return" }, isArray: false },
            resubmitLegalApproval: { method: "POST", params: { module: "renewalLegalApproval", action: "resubmit" }, isArray: false },
            recallLegalApproval: { method: "POST", params: { module: "renewalLegalApproval", action: "recall" }, isArray: false },
            editLegalApproval: { method: "POST", params: { module: "renewalLegalApproval", action: "edit" }, isArray: false },
            initPackagePage: { method: "GET", params: { module: "renewalPackage", action: "initPage" }, isArray: false },
            savePackage: { method: "POST", params: { module: "renewalPackage", action: "save" }, isArray: false },
            submitPackage: { method: "POST", params: { module: "renewalPackage", action: "submit" }, isArray: false },
            approvePackage: { method: "POST", params: { module: "renewalPackage", action: "approve" }, isArray: false },
            returnPackage: { method: "POST", params: { module: "renewalPackage", action: "return" }, isArray: false },
            rejectPackage: { method: "POST", params: { module: "renewalPackage", action: "reject" }, isArray: false },
            resubmitPackage: { method: "POST", params: { module: "renewalPackage", action: "resubmit" }, isArray: false },
            confirmPackage: { method: "POST", params: { module: "renewalPackage", action: "confirm" }, isArray: false },
            recallPackage: { method: "POST", params: { module: "renewalPackage", action: "recall" }, isArray: false },
            editPackage: { method: "POST", params: { module: "renewalPackage", action: "edit" }, isArray: false },
            needCDOApprovePackage: { method: "GET", params: { module: "renewalPackage", action: "needCDOApproval" }, isArray: false },
            initContractInfoPage: { method: "GET", params: { module: "renewalContractInfo", action: "initPage" }, isArray: false },
            initSiteInfoPage: { method: "GET", params: { module: "renewalSiteInfo", action: "initPage" }, isArray: false },
            saveSiteInfo: { method: "POST", params: { module: "renewalSiteInfo", action: "save" }, isArray: false },
            submitSiteInfo: { method: "POST", params: { module: "renewalSiteInfo", action: "submit" }, isArray: false },

            ////----------GBMemo
            getGBMemoInfo: {
                method: "GET",
                params: { module: "Renewal/GBMemo", action: "GetGBMemoInfo" },
                isArray: false
            },
            saveGBMemo: {
                method: 'POST',
                params: { module: 'Renewal/GBMemo', action: 'SaveGBMemo' },
                isArray: false
            },
            submitGBMemo: {
                method: 'POST',
                params: { module: 'Renewal/GBMemo', action: 'SubmitGBMemo' },
                isArray: false
            },
            resubmitGBMemo: {
                method: 'POST',
                params: { module: 'Renewal/GBMemo', action: 'ResubmitGBMemo' },
                isArray: false
            },
            approveGBMemo: {
                method: 'POST',
                params: { module: 'Renewal/GBMemo', action: 'ApproveGBMemo' },
                isArray: false
            },
            recallGBMemo: {
                method: 'POST',
                params: { module: 'Renewal/GBMemo', action: 'RecallGBMemo' },
                isArray: false
            },
            editGBMemo: {
                method: 'POST',
                params: { module: 'Renewal/GBMemo', action: 'EditGBMemo' },
                isArray: false
            },
            returnGBMemo: {
                method: 'POST',
                params: { module: 'Renewal/GBMemo', action: 'ReturnGBMemo' },
                isArray: false
            },
            notifyGBMemo: {
                method: 'POST',
                params: { module: 'Renewal/GBMemo', action: 'NotifyGBMemo' },
                isArray: false
            },
        });
    }
]);