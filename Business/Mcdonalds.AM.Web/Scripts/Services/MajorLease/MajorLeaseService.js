angular.module("mcd.am.service.majorLease", ["ngResource"])
    .factory("majorLeaseService", [
        "$resource",
        function ($resource) {
            return $resource(Utils.ServiceURI.Address() + 'api/:contorller/:action', {}, {
                getProjectId: {
                    method: 'GET',
                    params: { contorller: "MajorLease", action: 'GetProjectId', procInstId: '@procInstId' },
                    isArray: false
                },
                queryDictionary: {
                    method: 'GET',
                    params: { contorller: "dictionary", action: 'QueryList' },
                    isArray: true
                },
                beginCreateMajorLease: {
                    method: "POST",
                    params: { contorller: "MajorLease", action: "BeginCreate" },
                    isArray: false
                },
                getMajorInfo: {
                    method: "GET",
                    params: { contorller: "MajorLease", action: "GetMajorInfo" },
                    isArray: false
                },
                getMajorTopNav: {
                    method: "GET",
                    params: { contorller: "MajorLease", action: "GetNavTop" },
                    isArray: false
                },
                //-----------------------LegalReview
                getLegalReviewInfo: {
                    method: "GET",
                    params: { contorller: "MajorLease/LegalReview", action: "GetLegalReview" },
                    isArray: false
                },
                getLegalContractList: {
                    method: "GET",
                    params: { contorller: "MajorLease/LegalReview", action: "GetLegalContractList" },
                    isArray: true
                },
                saveMajorLegalReview: {
                    method: "POST",
                    params: { contorller: "MajorLease/LegalReview", action: "SaveLegalReview" },
                    isArray: false
                },
                submitMajorLegalReview: {
                    method: "POST",
                    params: { contorller: "MajorLease/LegalReview", action: "SubmitLegalReview" },
                    isArray: false
                },
                recallMajorLegalReview: {
                    method: "POST",
                    params: { contorller: "MajorLease/LegalReview", action: "RecallLegalReview" },
                    isArray: false
                },
                editMajorLegalReview: {
                    method: "POST",
                    params: { contorller: "MajorLease/LegalReview", action: "EditLegalReview" },
                    isArray: false
                },
                resubmitMajorLegalReview: {
                    method: "POST",
                    params: { contorller: "MajorLease/LegalReview", action: "ResubmitLegalReview" },
                    isArray: false
                },
                approveMajorLegalReview: {
                    method: "POST",
                    params: { contorller: "MajorLease/LegalReview", action: "ApproveLegalReview" },
                    isArray: false
                },
                rejectMajorLegalReview: {
                    method: 'POST',
                    params: { contorller: 'MajorLease/LegalReview', action: 'RejectLegalReview' },
                    isArray: false
                },
                returnMajorLegalReview: {
                    method: 'POST',
                    params: { contorller: 'MajorLease/LegalReview', action: 'ReturnLegalReview' },
                    isArray: false
                },
                ///////////////////////---------FinanceAnalysis
                getFinanceInfo: {
                    method: "GET",
                    params: { contorller: "MajorLease/FinanceAnalysis", action: "GetFinanceAnalysis" },
                    isArray: false
                },
                getFinanceAgreementList: {
                    method: "GET",
                    params: { contorller: "MajorLease/FinanceAnalysis", action: "GetFinanceAgreementList" },
                    isArray: true
                },
                saveFinanceAnalysis: {
                    method: "POST",
                    params: { contorller: "MajorLease/FinanceAnalysis", action: "SaveFinanceAnalysis" },
                    isArray: false
                },
                submitFinanceAnalysis: {
                    method: "POST",
                    params: { contorller: "MajorLease/FinanceAnalysis", action: "SubmitFinanceAnalysis" },
                    isArray: false
                },
                recallFinanceAnalysis: {
                    method: "POST",
                    params: { contorller: "MajorLease/FinanceAnalysis", action: "RecallFinanceAnalysis" },
                    isArray: false
                },
                editFinanceAnalysis: {
                    method: "POST",
                    params: { contorller: "MajorLease/FinanceAnalysis", action: "EditFinanceAnalysis" },
                    isArray: false
                },
                resubmitFinanceAnalysis: {
                    method: "POST",
                    params: { contorller: "MajorLease/FinanceAnalysis", action: "ResubmitFinanceAnalysis" },
                    isArray: false
                },
                approveFinanceAnalysis: {
                    method: "POST",
                    params: { contorller: "MajorLease/FinanceAnalysis", action: "ApproveFinanceAnalysis" },
                    isArray: false
                },
                rejectFinanceAnalysis: {
                    method: 'POST',
                    params: { contorller: 'MajorLease/FinanceAnalysis', action: 'RejectFinanceAnalysis' },
                    isArray: false
                },
                returnFinanceAnalysis: {
                    method: 'POST',
                    params: { contorller: 'MajorLease/FinanceAnalysis', action: 'ReturnFinanceAnalysis' },
                    isArray: false
                },
                ///////////////-----------ConsInfo
                getConsInfo: {
                    method: "GET",
                    params: { contorller: "MajorLease/ConsInfo", action: "GetConsInfo" },
                    isArray: false
                },
                getReinvenstmentAmountType: {
                    method: "GET",
                    params: { contorller: "MajorLease/ConsInfo", action: "GetReinvenstmentAmountType" },
                    isArray: true
                },
                getConsInfoAgreementList: {
                    method: "GET",
                    params: { contorller: "MajorLease/ConsInfo", action: "GetConsInfoAgreementList" },
                    isArray: true
                },
                getInvestCost: {
                    method: "GET",
                    params: { contorller: "MajorLease/ConsInfo", action: "GetInvestCost" },
                    isArray: false
                },
                getWriteOff: {
                    method: "GET",
                    params: { contorller: "MajorLease/ConsInfo", action: "GetWriteOff" },
                    isArray: false
                },
                saveConsInfo: {
                    method: "POST",
                    params: { contorller: "MajorLease/ConsInfo", action: "SaveConsInfo" },
                    isArray: false
                },
                submitConsInfo: {
                    method: "POST",
                    params: { contorller: "MajorLease/ConsInfo", action: "SubmitConsInfo" },
                    isArray: false
                },
                recallConsInfo: {
                    method: "POST",
                    params: { contorller: "MajorLease/ConsInfo", action: "RecallConsInfo" },
                    isArray: false
                },
                editConsInfo: {
                    method: "POST",
                    params: { contorller: "MajorLease/ConsInfo", action: "EditConsInfo" },
                    isArray: false
                },
                resubmitConsInfo: {
                    method: "POST",
                    params: { contorller: "MajorLease/ConsInfo", action: "ResubmitConsInfo" },
                    isArray: false
                },
                approveConsInfo: {
                    method: "POST",
                    params: { contorller: "MajorLease/ConsInfo", action: "ApproveConsInfo" },
                    isArray: false
                },
                rejectConsInfo: {
                    method: 'POST',
                    params: { contorller: 'MajorLease/ConsInfo', action: 'RejectConsInfo' },
                    isArray: false
                },
                returnConsInfo: {
                    method: 'POST',
                    params: { contorller: 'MajorLease/ConsInfo', action: 'ReturnConsInfo' },
                    isArray: false
                },
                ///////----------LeaseChangePackage
                getPackageInfo: {
                    method: "GET",
                    params: { contorller: "MajorLease/LeaseChangePackage", action: "GetPackageInfo" },
                    isArray: false
                },
                savePackageInfo: {
                    method: "POST",
                    params: { contorller: "MajorLease/LeaseChangePackage", action: "SavePackageInfo" },
                    isArray: false
                },
                submitPackageInfo: {
                    method: "POST",
                    params: { contorller: "MajorLease/LeaseChangePackage", action: "SubmitPackageInfo" },
                    isArray: false
                },
                confirmPackageInfo: {
                    method: "POST",
                    params: { contorller: "MajorLease/LeaseChangePackage", action: "ConfirmPackageInfo" },
                    isArray: false
                },
                recallPackageInfo: {
                    method: "POST",
                    params: { contorller: "MajorLease/LeaseChangePackage", action: "RecallPackageInfo" },
                    isArray: false
                },
                editPackageInfo: {
                    method: "POST",
                    params: { contorller: "MajorLease/LeaseChangePackage", action: "EditPackageInfo" },
                    isArray: false
                },
                resubmitPackageInfo: {
                    method: "POST",
                    params: { contorller: "MajorLease/LeaseChangePackage", action: "ResubmitPackageInfo" },
                    isArray: false
                },
                approvePackageInfo: {
                    method: "POST",
                    params: { contorller: "MajorLease/LeaseChangePackage", action: "ApprovePackageInfo" },
                    isArray: false
                },
                rejectPackageInfo: {
                    method: 'POST',
                    params: { contorller: 'MajorLease/LeaseChangePackage', action: 'RejectPackageInfo' },
                    isArray: false
                },
                returnPackageInfo: {
                    method: 'POST',
                    params: { contorller: 'MajorLease/LeaseChangePackage', action: 'ReturnPackageInfo' },
                    isArray: false
                },
                getPackageAgreementList: {
                    method: "GET",
                    params: { contorller: "MajorLease/LeaseChangePackage", action: "GetPackageAgreementList" },
                    isArray: true
                },
                generateZipFile: {
                    method: "POST",
                    params: { contorller: "MajorLease/LeaseChangePackage", action: "GenerateZipFile" },
                    isArray: false
                },
                ////----------ConsInvtChecking
                getConsInvtChecking: {
                    method: "GET",
                    params: { contorller: "MajorLease/ConsInvtChecking", action: "GetConsInvtChecking" },
                    isArray: false
                },
                saveConsInvtChecking: {
                    method: "POST",
                    params: { contorller: "MajorLease/ConsInvtChecking", action: "SaveConsInvtChecking" },
                    isArray: false
                },
                submitConsInvtChecking: {
                    method: "POST",
                    params: { contorller: "MajorLease/ConsInvtChecking", action: "SubmitConsInvtChecking" },
                    isArray: false
                },
                recallConsInvtChecking: {
                    method: "POST",
                    params: { contorller: "MajorLease/ConsInvtChecking", action: "RecallConsInvtChecking" },
                    isArray: false
                },
                editConsInvtChecking: {
                    method: "POST",
                    params: { contorller: "MajorLease/ConsInvtChecking", action: "EditConsInvtChecking" },
                    isArray: false
                },
                resubmitConsInvtChecking: {
                    method: "POST",
                    params: { contorller: "MajorLease/ConsInvtChecking", action: "ResubmitConsInvtChecking" },
                    isArray: false
                },
                approveConsInvtChecking: {
                    method: "POST",
                    params: { contorller: "MajorLease/ConsInvtChecking", action: "ApproveConsInvtChecking" },
                    isArray: false
                },
                rejectConsInvtChecking: {
                    method: 'POST',
                    params: { contorller: 'MajorLease/ConsInvtChecking', action: 'RejectConsInvtChecking' },
                    isArray: false
                },
                returnConsInvtChecking: {
                    method: 'POST',
                    params: { contorller: 'MajorLease/ConsInvtChecking', action: 'ReturnConsInvtChecking' },
                    isArray: false
                },
                ////----------GBMemo
                getGBMemoInfo: {
                    method: "GET",
                    params: { contorller: "MajorLease/GBMemo", action: "GetGBMemoInfo" },
                    isArray: false
                },
                saveGBMemo: {
                    method: 'POST',
                    params: { contorller: 'MajorLease/GBMemo', action: 'SaveGBMemo' },
                    isArray: false
                },
                submitGBMemo: {
                    method: 'POST',
                    params: { contorller: 'MajorLease/GBMemo', action: 'SubmitGBMemo' },
                    isArray: false
                },
                resubmitGBMemo: {
                    method: 'POST',
                    params: { contorller: 'MajorLease/GBMemo', action: 'ResubmitGBMemo' },
                    isArray: false
                },
                approveGBMemo: {
                    method: 'POST',
                    params: { contorller: 'MajorLease/GBMemo', action: 'ApproveGBMemo' },
                    isArray: false
                },
                recallGBMemo: {
                    method: 'POST',
                    params: { contorller: 'MajorLease/GBMemo', action: 'RecallGBMemo' },
                    isArray: false
                },
                editGBMemo: {
                    method: 'POST',
                    params: { contorller: 'MajorLease/GBMemo', action: 'EditGBMemo' },
                    isArray: false
                },
                returnGBMemo: {
                    method: 'POST',
                    params: { contorller: 'MajorLease/GBMemo', action: 'ReturnGBMemo' },
                    isArray: false
                },
                notifyGBMemo: {
                    method: 'POST',
                    params: { contorller: 'MajorLease/GBMemo', action: 'NotifyGBMemo' },
                    isArray: false
                },
            });
        }
    ]);