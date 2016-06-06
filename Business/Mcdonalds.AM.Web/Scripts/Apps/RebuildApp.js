var rebuildApp = angular.module("amApp", [
    'ngRoute',
    'ngChosen',
    "ngUploadify",
    'ui.bootstrap',
    'mcd.am.controller.remind',
    'mcd.am.service.remind',
    'dictionaryControllers',
    'dictionaryServices',
    'dictionaryFilters',
    'mcd.am.service.rebuild',
    "mcd.am.modules.rebuild",
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
rebuildApp.run([
    "$window", "messager", function ($window, messager) {
        validateUser($window, messager);
    }
]);
rebuildApp.config(['$routeProvider', "$sceProvider",
    function ($routeProvider, $sceProvider) {
        $sceProvider.enabled(false);
        $routeProvider.when('/Create',
            {
                templateUrl: '/Rebuild/Create',
                controller: 'createController'
            })
            //////------------LegalReview
            .when('/LegalReview',
            {
                templateUrl: '/Rebuild/LegalReview',
                controller: 'legalReviewController'
            })
            .when('/LegalReview/Process/:PageType', {
                templateUrl: '/Rebuild/LegalReview',
                controller: 'legalReviewController'
            })
            ///////------------FinanceAnalysis
            .when("/FinanceAnalysis",
            {
                templateUrl: '/Rebuild/FinanceAnalysis',
                controller: 'financeAnalysisController'
            })
            .when('/FinanceAnalysis/Process/:PageType', {
                templateUrl: '/Rebuild/FinanceAnalysis',
                controller: 'financeAnalysisController'
            })
            /////--------------ConsInfo
            .when("/ConsInfo",
            {
                templateUrl: '/Rebuild/ConsInfo',
                controller: 'consInfoController'
            })
            .when('/ConsInfo/Process/:PageType', {
                templateUrl: '/Rebuild/ConsInfo',
                controller: 'consInfoController'
            })
            /////-----------RebuildChangePackage
            .when("/RebuildPackage",
            {
                templateUrl: '/Rebuild/RebuildPackage',
                controller: 'rebuildPackageController'
            })
            .when('/RebuildPackage/Process/:PageType', {
                templateUrl: '/Rebuild/RebuildPackage',
                controller: 'rebuildPackageController'
            })
            ////-----------ContractInfo
            .when("/ContractInfo", {
                templateUrl: '/Rebuild/ContractInfo',
                controller: 'contractInfoController'
            })
            .when('/ContractInfo/Process/:PageType', {
                templateUrl: '/Rebuild/ContractInfo',
                controller: 'contractInfoController'
            })
            /////---------SiteInfo
            .when("/SiteInfo", {
                templateUrl: '/Rebuild/SiteInfo',
                controller: 'siteInfoController'
            })
            .when("/SiteInfo/Process/:PageType", {
                templateUrl: '/Rebuild/SiteInfo',
                controller: 'siteInfoController'
            })
            /////----------ConsInvtChecking
            .when("/ConsInvtChecking", {
                templateUrl: '/Rebuild/ConsInvtChecking',
                controller: 'consInvtCheckingController'
            })
            .when("/ConsInvtChecking/Process/:PageType", {
                templateUrl: '/Rebuild/ConsInvtChecking',
                controller: 'consInvtCheckingController'
            })
            //////------ReopenMemo
            .when("/ReopenMemo", {
                templateUrl: "/Rebuild/ReopenMemo",
                controller: 'reopenMemoController'
            })
            .when("/ReopenMemo/Process/:PageType", {
                templateUrl: "/Rebuild/ReopenMemo",
                controller: 'reopenMemoController'
            })
            //////------GBMemo
            .when("/GBMemo", {
                templateUrl: "/Rebuild/GBMemo",
                controller: 'GBMemoController'
            })
            .when("/GBMemo/Process/:PageType", {
                templateUrl: "/Rebuild/GBMemo",
                controller: 'GBMemoController'
            })
            //////------TempClosureMemo
            .when("/TempClosureMemo", {
                templateUrl: "/Rebuild/TempClosureMemo",
                controller: 'tempClosureMemoController'
            })
            .when("/TempClosureMemo/Process/:PageType", {
                templateUrl: "/Rebuild/TempClosureMemo",
                controller: 'tempClosureMemoController'
            })
            .otherwise({
                redirectTo: '/Index',
            });
    }]);