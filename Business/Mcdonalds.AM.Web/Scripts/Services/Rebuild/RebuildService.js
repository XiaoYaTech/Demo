angular.module("mcd.am.service.rebuild", ["ngResource"])
    .factory("rebuildService", [
        "$resource",
        function ($resource) {
            return $resource(Utils.ServiceURI.Address() + 'api/:contorller/:action', {}, {
                getProjectId: {
                    method: 'GET',
                    params: { contorller: "Rebuild", action: 'GetProjectId', procInstId: '@procInstId' },
                    isArray: false
                },
                queryDictionary: {
                    method: 'GET',
                    params: { contorller: "dictionary", action: 'QueryList' },
                    isArray: true
                },
                beginCreateRebuild: {
                    method: "POST",
                    params: { contorller: "Rebuild", action: "BeginCreate" },
                    isArray: false
                },
                getRebuildInfo: {
                    method: "GET",
                    params: { contorller: "Rebuild", action: "GetRebuildInfo" },
                    isArray: false
                },
                getRebuildTopNav: {
                    method: "GET",
                    params: { contorller: "Rebuild", action: "GetNavTop" },
                    isArray: false
                },
                //-----------------------LegalReview
                getLegalReviewInfo: {
                    method: "GET",
                    params: { contorller: "Rebuild/LegalReview", action: "GetLegalReview" },
                    isArray: false
                },
                getLegalContractList: {
                    method: "GET",
                    params: { contorller: "Rebuild/LegalReview", action: "GetLegalContractList" },
                    isArray: true
                },
                saveRebuildLegalReview: {
                    method: "POST",
                    params: { contorller: "Rebuild/LegalReview", action: "SaveLegalReview" },
                    isArray: false
                },
                submitRebuildLegalReview: {
                    method: "POST",
                    params: { contorller: "Rebuild/LegalReview", action: "SubmitLegalReview" },
                    isArray: false
                },
                recallRebuildLegalReview: {
                    method: "POST",
                    params: { contorller: "Rebuild/LegalReview", action: "RecallLegalReview" },
                    isArray: false
                },
                editRebuildLegalReview: {
                    method: "POST",
                    params: { contorller: "Rebuild/LegalReview", action: "EditLegalReview" },
                    isArray: false
                },
                resubmitRebuildLegalReview: {
                    method: "POST",
                    params: { contorller: "Rebuild/LegalReview", action: "ResubmitLegalReview" },
                    isArray: false
                },
                approveRebuildLegalReview: {
                    method: "POST",
                    params: { contorller: "Rebuild/LegalReview", action: "ApproveLegalReview" },
                    isArray: false
                },
                rejectRebuildLegalReview: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/LegalReview', action: 'RejectLegalReview' },
                    isArray: false
                },
                returnRebuildLegalReview: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/LegalReview', action: 'ReturnLegalReview' },
                    isArray: false
                },
                ///////////////////////---------FinanceAnalysis
                getFinanceInfo: {
                    method: "GET",
                    params: { contorller: "Rebuild/FinanceAnalysis", action: "GetFinanceAnalysis" },
                    isArray: false
                },
                getFinanceAgreementList: {
                    method: "GET",
                    params: { contorller: "Rebuild/FinanceAnalysis", action: "GetFinanceAgreementList" },
                    isArray: true
                },
                saveFinanceAnalysis: {
                    method: "POST",
                    params: { contorller: "Rebuild/FinanceAnalysis", action: "SaveFinanceAnalysis" },
                    isArray: false
                },
                submitFinanceAnalysis: {
                    method: "POST",
                    params: { contorller: "Rebuild/FinanceAnalysis", action: "SubmitFinanceAnalysis" },
                    isArray: false
                },
                recallFinanceAnalysis: {
                    method: "POST",
                    params: { contorller: "Rebuild/FinanceAnalysis", action: "RecallFinanceAnalysis" },
                    isArray: false
                },
                editFinanceAnalysis: {
                    method: "POST",
                    params: { contorller: "Rebuild/FinanceAnalysis", action: "EditFinanceAnalysis" },
                    isArray: false
                },
                resubmitFinanceAnalysis: {
                    method: "POST",
                    params: { contorller: "Rebuild/FinanceAnalysis", action: "ResubmitFinanceAnalysis" },
                    isArray: false
                },
                approveFinanceAnalysis: {
                    method: "POST",
                    params: { contorller: "Rebuild/FinanceAnalysis", action: "ApproveFinanceAnalysis" },
                    isArray: false
                },
                rejectFinanceAnalysis: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/FinanceAnalysis', action: 'RejectFinanceAnalysis' },
                    isArray: false
                },
                returnFinanceAnalysis: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/FinanceAnalysis', action: 'ReturnFinanceAnalysis' },
                    isArray: false
                },
                ///////////////-----------ConsInfo
                getConsInfo: {
                    method: "GET",
                    params: { contorller: "Rebuild/ConsInfo", action: "GetConsInfo" },
                    isArray: false
                },
                getReinvenstmentAmountType: {
                    method: "GET",
                    params: { contorller: "Rebuild/ConsInfo", action: "GetReinvenstmentAmountType" },
                    isArray: true
                },
                getConsInfoAgreementList: {
                    method: "GET",
                    params: { contorller: "Rebuild/ConsInfo", action: "GetConsInfoAgreementList" },
                    isArray: true
                },
                getInvestCost: {
                    method: "GET",
                    params: { contorller: "Rebuild/ConsInfo", action: "GetInvestCost" },
                    isArray: false
                },
                getWriteOff: {
                    method: "GET",
                    params: { contorller: "Rebuild/ConsInfo", action: "GetWriteOff" },
                    isArray: false
                },
                saveConsInfo: {
                    method: "POST",
                    params: { contorller: "Rebuild/ConsInfo", action: "SaveConsInfo" },
                    isArray: false
                },
                submitConsInfo: {
                    method: "POST",
                    params: { contorller: "Rebuild/ConsInfo", action: "SubmitConsInfo" },
                    isArray: false
                },
                recallConsInfo: {
                    method: "POST",
                    params: { contorller: "Rebuild/ConsInfo", action: "RecallConsInfo" },
                    isArray: false
                },
                editConsInfo: {
                    method: "POST",
                    params: { contorller: "Rebuild/ConsInfo", action: "EditConsInfo" },
                    isArray: false
                },
                resubmitConsInfo: {
                    method: "POST",
                    params: { contorller: "Rebuild/ConsInfo", action: "ResubmitConsInfo" },
                    isArray: false
                },
                approveConsInfo: {
                    method: "POST",
                    params: { contorller: "Rebuild/ConsInfo", action: "ApproveConsInfo" },
                    isArray: false
                },
                rejectConsInfo: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/ConsInfo', action: 'RejectConsInfo' },
                    isArray: false
                },
                returnConsInfo: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/ConsInfo', action: 'ReturnConsInfo' },
                    isArray: false
                },
                ///////----------LeaseChangePackage
                getPackageInfo: {
                    method: "GET",
                    params: { contorller: "Rebuild/RebuildPackage", action: "GetPackageInfo" },
                    isArray: false
                },
                savePackageInfo: {
                    method: "POST",
                    params: { contorller: "Rebuild/RebuildPackage", action: "SavePackageInfo" },
                    isArray: false
                },
                submitPackageInfo: {
                    method: "POST",
                    params: { contorller: "Rebuild/RebuildPackage", action: "SubmitPackageInfo" },
                    isArray: false
                },
                confirmPackageInfo: {
                    method: "POST",
                    params: { contorller: "Rebuild/RebuildPackage", action: "ConfirmPackageInfo" },
                    isArray: false
                },
                recallPackageInfo: {
                    method: "POST",
                    params: { contorller: "Rebuild/RebuildPackage", action: "RecallPackageInfo" },
                    isArray: false
                },
                editPackageInfo: {
                    method: "POST",
                    params: { contorller: "Rebuild/RebuildPackage", action: "EditPackageInfo" },
                    isArray: false
                },
                resubmitPackageInfo: {
                    method: "POST",
                    params: { contorller: "Rebuild/RebuildPackage", action: "ResubmitPackageInfo" },
                    isArray: false
                },
                approvePackageInfo: {
                    method: "POST",
                    params: { contorller: "Rebuild/RebuildPackage", action: "ApprovePackageInfo" },
                    isArray: false
                },
                rejectPackageInfo: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/RebuildPackage', action: 'RejectPackageInfo' },
                    isArray: false
                },
                returnPackageInfo: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/RebuildPackage', action: 'ReturnPackageInfo' },
                    isArray: false
                },
                getPackageAgreementList: {
                    method: "GET",
                    params: { contorller: "Rebuild/RebuildPackage", action: "GetPackageAgreementList" },
                    isArray: true
                },
                generateZipFile: {
                    method: "POST",
                    params: { contorller: "Rebuild/RebuildPackage", action: "GenerateZipFile" },
                    isArray: false
                },
                ////----------ConsInvtChecking
                getConsInvtChecking: {
                    method: "GET",
                    params: { contorller: "Rebuild/ConsInvtChecking", action: "GetConsInvtChecking" },
                    isArray: false
                },
                saveConsInvtChecking: {
                    method: "POST",
                    params: { contorller: "Rebuild/ConsInvtChecking", action: "SaveConsInvtChecking" },
                    isArray: false
                },
                submitConsInvtChecking: {
                    method: "POST",
                    params: { contorller: "Rebuild/ConsInvtChecking", action: "SubmitConsInvtChecking" },
                    isArray: false
                },
                recallConsInvtChecking: {
                    method: "POST",
                    params: { contorller: "Rebuild/ConsInvtChecking", action: "RecallConsInvtChecking" },
                    isArray: false
                },
                editConsInvtChecking: {
                    method: "POST",
                    params: { contorller: "Rebuild/ConsInvtChecking", action: "EditConsInvtChecking" },
                    isArray: false
                },
                resubmitConsInvtChecking: {
                    method: "POST",
                    params: { contorller: "Rebuild/ConsInvtChecking", action: "ResubmitConsInvtChecking" },
                    isArray: false
                },
                approveConsInvtChecking: {
                    method: "POST",
                    params: { contorller: "Rebuild/ConsInvtChecking", action: "ApproveConsInvtChecking" },
                    isArray: false
                },
                rejectConsInvtChecking: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/ConsInvtChecking', action: 'RejectConsInvtChecking' },
                    isArray: false
                },
                returnConsInvtChecking: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/ConsInvtChecking', action: 'ReturnConsInvtChecking' },
                    isArray: false
                },
                ////----------SiteInfo
                initSiteInfoPage: {
                    method: 'GET',
                    params: { contorller: 'Rebuild/SiteInfo', action: 'InitPage' },
                    isArray: false
                },
                submitSiteInfo: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/SiteInfo', action: 'SubmitSiteInfo' },
                    isArray: false
                },
                saveSiteInfo: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/SiteInfo', action: 'SaveSiteInfo' },
                    isArray: false
                },
                ////----------GBMemo
                getGBMemoInfo: {
                    method: "GET",
                    params: { contorller: "Rebuild/GBMemo", action: "GetGBMemoInfo" },
                    isArray: false
                },
                saveGBMemo: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/GBMemo', action: 'SaveGBMemo' },
                    isArray: false
                },
                submitGBMemo: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/GBMemo', action: 'SubmitGBMemo' },
                    isArray: false
                },
                resubmitGBMemo: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/GBMemo', action: 'ResubmitGBMemo' },
                    isArray: false
                },
                approveGBMemo: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/GBMemo', action: 'ApproveGBMemo' },
                    isArray: false
                },
                recallGBMemo: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/GBMemo', action: 'RecallGBMemo' },
                    isArray: false
                },
                editGBMemo: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/GBMemo', action: 'EditGBMemo' },
                    isArray: false
                },
                returnGBMemo: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/GBMemo', action: 'ReturnGBMemo' },
                    isArray: false
                },
                notifyGBMemo: {
                    method: 'POST',
                    params: { contorller: 'Rebuild/GBMemo', action: 'NotifyGBMemo' },
                    isArray: false
                },
            });
        }
    ]);