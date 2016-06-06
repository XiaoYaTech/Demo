var marjorLeaseApp = angular.module("amApp", [
    'ngRoute',
    'ngChosen',
    "ngUploadify",
    'ui.bootstrap',
    'mcd.am.controller.remind',
    'mcd.am.service.remind',
    'dictionaryControllers',
    'dictionaryServices',
    'dictionaryFilters',
    'mcd.am.service.majorLease',
    "mcd.am.modules.majorLease",
    'mcd.am.service.taskwork',
    'mcd.am.filters',
    'project.list',
    'nttmnc.fx.modules',
    'mcd.am.modules',
    "mcd.am.services.contract",
    "mcd.am.services.siteinfo",
    "mcd.am.services.tempClosure",
    "mcd.am.service.reopenMemo",
    "mcd.am.service.GBMemo"]);
marjorLeaseApp.run(["$window", "messager", function ($window, messager) {
    validateUser($window, messager);
}]);
marjorLeaseApp.config(['$routeProvider', "$sceProvider",
    function ($routeProvider, $sceProvider) {
        $sceProvider.enabled(false);
        $routeProvider.when('/Create',
            {
                templateUrl: '/MajorLease/Create',
                controller: 'majorLeaseCreateController'
            })
            //////------------LegalReview
            .when('/LegalReview',
            {
                templateUrl: '/MajorLease/LegalReview',
                controller: 'legalReviewController'
            })
            .when('/LegalReview/Process/:PageType', {
                templateUrl: '/MajorLease/LegalReview',
                controller: 'legalReviewController'
            })
            ///////------------FinanceAnalysis
            .when("/FinanceAnalysis",
            {
                templateUrl: '/MajorLease/FinanceAnalysis',
                controller: 'financeAnalysisController'
            })
            .when('/FinanceAnalysis/Process/:PageType', {
                templateUrl: '/MajorLease/FinanceAnalysis',
                controller: 'financeAnalysisController'
            })
            /////--------------ConsInfo
            .when("/ConsInfo",
            {
                templateUrl: '/MajorLease/ConsInfo',
                controller: 'consInfoController'
            })
            .when('/ConsInfo/Process/:PageType', {
                templateUrl: '/MajorLease/ConsInfo',
                controller: 'consInfoController'
            })
            /////-----------LeaseChangePackage
            .when("/Package",
            {
                templateUrl: '/MajorLease/LeaseChangePackage',
                controller: 'leaseChangePackageController'
            })
            .when('/Package/Process/:PageType', {
                templateUrl: '/MajorLease/LeaseChangePackage',
                controller: 'leaseChangePackageController'
            })
            ////-----------ContractInfo
            .when("/ContractInfo", {
                templateUrl: '/MajorLease/ContractInfo',
                controller: 'contractInfoController'
            })
            .when('/ContractInfo/Process/:PageType', {
                templateUrl: '/MajorLease/ContractInfo',
                controller: 'contractInfoController'
            })
            /////---------SiteInfo
            .when("/SiteInfo", {
                templateUrl: '/MajorLease/SiteInfo',
                controller: 'siteInfoController'
            })
            .when("/SiteInfo/Process/:PageType", {
                templateUrl: '/MajorLease/SiteInfo',
                controller: 'siteInfoController'
            })
            /////----------ConsInvtChecking
            .when("/ConsInvtChecking", {
                templateUrl: '/MajorLease/ConsInvtChecking',
                controller: 'consInvtCheckingController'
            })
            .when("/ConsInvtChecking/Process/:PageType", {
                templateUrl: '/MajorLease/ConsInvtChecking',
                controller: 'consInvtCheckingController'
            })
            //////------ReopenMemo
            .when("/ReopenMemo", {
                templateUrl: "/MajorLease/ReopenMemo",
                controller: 'reopenMemoController'
            })
            .when("/ReopenMemo/Process/:PageType", {
                templateUrl: "/MajorLease/ReopenMemo",
                controller: 'reopenMemoController'
            })
            //////------GBMemo
            .when("/GBMemo", {
                templateUrl: "/MajorLease/GBMemo",
                controller: 'GBMemoController'
            })
            .when("/GBMemo/Process/:PageType", {
                templateUrl: "/MajorLease/GBMemo",
                controller: 'GBMemoController'
            })
            //////------TempClosureMemo
            .when("/ClosureMemo", {
                templateUrl: "/MajorLease/ClosureMemo",
                controller: 'closureMemoController'
            })
            .when("/ClosureMemo/Process/:PageType", {
                templateUrl: "/MajorLease/ClosureMemo",
                controller: 'closureMemoController'
            })
            .otherwise({
                redirectTo: '/Index',
            });
    }]);