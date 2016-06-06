var reimageServices = angular.module("mcd.am.service.reimage", ["ngResource"]);

reimageServices.factory("reimageService", ["$resource",
     function ($resource) {
         return $resource(Utils.ServiceURI.Address() + 'api/Reimage/:action', {}, {

             createProject: { method: "POST", params: { action: "CreateProject" }, isArray: false },

             getReimageInfo: { method: 'GET', params: { action: 'GetReimageInfo', projectId: '@projectId' }, isArray: false },
             getEstimatedVsActualConstruction: {
                 method: 'GET',
                 params: { action: 'GetEstimatedVsActualConstruction' },
                 isArray: false
             },
             getReimageSummary: { method: 'GET', params: { action: 'GetReimageSummary', projectId: '@projectId' }, isArray: false },
             submitConsInfo: { method: 'POST', params: { action: 'SubmitConsInfo' }, isArray: false },

             saveConsInfo: {
                 method: "POST",
                 params: { action: "SaveConsInfo" },
                 isArray: false
             },
             recallConsInfo: {
                 method: "POST",
                 params: { contorller: "ConsInfo", action: "RecallConsInfo" },
                 isArray: false
             },
             editConsInfo: {
                 method: "POST",
                 params: { contorller: "ConsInfo", action: "EditConsInfo" },
                 isArray: false
             },
             getKeyMeasuresInfo: {
                 method: "GET",
                 params: { action: "GetKeyMeasuresInfo" },
                 isArray: false
             },
             getConsInfo: {
                 method: "GET",
                 params: { action: "GetConsInfo" },
                 isArray: false
             },
             getConsInfoAgreementList: {
                 method: "GET",
                 params: { action: "GetConsInfoAgreementList" },
                 isArray: true
             },
             getPackageAgreementList: {
                 method: "GET",
                 params: { action: "GetPackageAgreementList" },
                 isArray: true
             },
             getInvestCost: {
                 method: "GET",
                 params: { action: "GetInvestCost" },
                 isArray: false
             },

             getPackageInfo: {
                 method: "GET",
                 params: { action: "GetPackageInfo" },
                 isArray: false
             },
             getConsInvtChecking: {
                 method: "GET",
                 params: { action: "GetConsInvtChecking" },
                 isArray: false
             },

             getWriteOff: {
                 method: "GET",
                 params: { action: "GetWriteOff" },
                 isArray: false
             },

             returnConsInfo: {
                 method: 'POST',
                 params: { action: 'ReturnConsInfo' },
                 isArray: false
             },
             returnPackage: {
                 method: 'POST',
                 params: { action: 'ReturnPackage' },
                 isArray: false
             },
             getPackageAgreementList: {
                 method: 'GET',
                 params: { action: 'GetPackageAgreementList' },
                 isArray: true
             },


             resubmitConsInfo: {
                 method: "POST",
                 params: { contorller: "ConsInfo", action: "ResubmitConsInfo" },
                 isArray: false
             },

             approveConsInfo: { method: 'POST', params: { action: 'ApproveConsInfo' }, isArray: false },
             approvePackage: { method: 'POST', params: { action: 'ApprovePackage' }, isArray: false },
             approveConsInvtChecking: { method: 'POST', params: { action: 'ApproveConsInvtChecking' }, isArray: false },
             rejectConsInfo: { method: 'POST', params: { action: 'RejectConsInfo' }, isArray: false },
             rejectPackage: { method: 'POST', params: { action: 'RejectPackage' }, isArray: false },
             recallPackage: { method: 'POST', params: { action: 'RecallPackage' }, isArray: false },
             recallConsInvtChecking: { method: 'POST', params: { action: 'RecallConsInvtChecking' }, isArray: false },
             returnConsInvtChecking: { method: 'POST', params: { action: 'ReturnConsInvtChecking' }, isArray: false },
             editPackage: { method: 'POST', params: { action: 'EditPackage' }, isArray: false },
             editConsInvtChecking: { method: 'POST', params: { action: 'EditConsInvtChecking' }, isArray: false },
             initReimageSummary: {
                 method: "GET",
                 params: { action: "InitReimageSummary" },
                 isArray: false
             },
             saveReimageSummary: {
                 method: "POST",
                 params: { action: "SaveReimageSummary" },
                 isArray: false
             },
             recallReimageSummary: {
                 method: "POST",
                 params: { contorller: "Reimage", action: "RecallReimageSummary" },
                 isArray: false
             },
             editReimageSummary: {
                 method: "POST",
                 params: { contorller: "Reimage", action: "EditReimageSummary" },
                 isArray: false
             },
             savePackage: {
                 method: "POST",
                 params: { action: "SavePackage" },
                 isArray: false
             },
             saveConsInvtChecking: {
                 method: "POST",
                 params: { action: "SaveConsInvtChecking" },
                 isArray: false
             },
             submitConsInvtChecking: {
                 method: "POST",
                 params: { action: "SubmitConsInvtChecking" },
                 isArray: false
             },
             resubmitConsInvtChecking: {
                 method: "POST",
                 params: { action: "ResubmitConsInvtChecking" },
                 isArray: false
             },
             resubmitPackage: {
                 method: "POST",
                 params: { action: "ResubmitPackage" },
                 isArray: false
             },
             submitPackage: {
                 method: "POST",
                 params: { action: "SubmitPackage" },
                 isArray: false
             },
             submitReimageSummary: {
                 method: "POST",
                 params: { action: "SubmitReimageSummary" },
                 isArray: false
             },
             isExistReimageSummaryAttachment: {
                 method: 'POST',
                 params: { action: "IsExistReimageSummaryAttachment" },
                 isArray: false
             },
             ////----------GBMemo
             getGBMemoInfo: {
                 method: "GET",
                 params: { action: "GetGBMemoInfo" },
                 isArray: false
             },
             saveGBMemo: {
                 method: 'POST',
                 params: { action: 'SaveGBMemo' },
                 isArray: false
             },
             submitGBMemo: {
                 method: 'POST',
                 params: { action: 'SubmitGBMemo' },
                 isArray: false
             },
             resubmitGBMemo: {
                 method: 'POST',
                 params: { action: 'ResubmitGBMemo' },
                 isArray: false
             },
             approveGBMemo: {
                 method: 'POST',
                 params: { action: 'ApproveGBMemo' },
                 isArray: false
             },
             recallGBMemo: {
                 method: 'POST',
                 params: { action: 'RecallGBMemo' },
                 isArray: false
             },
             editGBMemo: {
                 method: 'POST',
                 params: { action: 'EditGBMemo' },
                 isArray: false
             },
             returnGBMemo: {
                 method: 'POST',
                 params: { action: 'ReturnGBMemo' },
                 isArray: false
             },
             notifyGBMemo: {
                 method: 'POST',
                 params: { action: 'NotifyGBMemo' },
                 isArray: false
             },
             packageList: {
                 method: 'POST',
                 params: { action: 'PackageList' },
                 isArray: false
             },
             releasePackages: {
                 method: 'POST',
                 params: { action: 'ReleasePackages' },
                 isArray: true
             }

         });
     }]);