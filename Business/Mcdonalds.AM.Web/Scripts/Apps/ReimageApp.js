var reimageApp = angular.module("amApp", [
    'ngRoute',
    'ngChosen',
    'ui.bootstrap',
    'mcd.am.controller.remind',
    'mcd.am.service.remind',
    'dictionaryControllers',
    'dictionaryServices',
    'dictionaryFilters',
    'mcd.am.filters',
    'project.list',
    'nttmnc.fx.modules',
    'mcd.am.modules',
    'mcd.am.service.reimage',
    'mcd.am.services.excel',
    'mcd.am.modules.reimage',
    'mcd.am.services.siteinfo',
    "mcd.am.service.reopenMemo",
    "mcd.am.service.GBMemo"
]);
reimageApp.run([
    "$window", "messager", function ($window, messager) {
        validateUser($window, messager);
    }
]);
reimageApp.config(['$routeProvider', "$sceProvider",
    function ($routeProvider, $sceProvider) {
        $sceProvider.enabled(false);
        $routeProvider.when('/Create', {
            templateUrl: '/Reimage/Create',
            controller: 'reimageCreateController'
        }).when('/ConsInfo', {
            templateUrl: '/Reimage/ConsInfo',
            controller: 'ConsInfoCtrl'
        }).when('/ConsInfo/Process/:PageType', {
            templateUrl: '/Reimage/ConsInfo',
            controller: 'ConsInfoCtrl'
        })
         /////-----------Reimage Summary
            .when("/Summary",
            {
                templateUrl: '/Reimage/ReimageSummary',
                controller: 'reimageSummaryCtrl'
            })
            .when('/Summary/Process/:PageType', {
                templateUrl: '/Reimage/ReimageSummary',
                controller: 'reimageSummaryCtrl'
            })
            .when("/Package",
            {
                templateUrl: '/Reimage/ReimagePackage',
                controller: 'reimagePackageCtrl'
            })
            .when('/Package/Process/:PageType', {
                templateUrl: '/Reimage/ReimagePackage',
                controller: 'reimagePackageCtrl'
            })
              .when("/ConsInvtChecking", {
                  templateUrl: '/Reimage/ConsInvtChecking',
                  controller: 'consInvtCheckingController'
              })
            .when("/ConsInvtChecking/Process/:PageType", {
                templateUrl: '/Reimage/ConsInvtChecking',
                controller: 'consInvtCheckingController'
            })
             //////------ReopenMemo
            .when("/ReopenMemo", {
                templateUrl: "/Reimage/ReopenMemo",
                controller: 'reopenMemoController'
            })
            .when("/ReopenMemo/Process/:PageType", {
                templateUrl: "/Reimage/ReopenMemo",
                controller: 'reopenMemoController'
            })
              //////------GBMemo
            .when("/GBMemo", {
                templateUrl: "/Reimage/GBMemo",
                controller: 'GBMemoController'
            })
            .when("/GBMemo/Process/:PageType", {
                templateUrl: "/Reimage/GBMemo",
                controller: 'GBMemoController'
            })
            //////------TempClosureMemo
            .when("/TempClosureMemo", {
                templateUrl: "/Reimage/TempClosureMemo",
                controller: 'tempClosureMemoController'
            })
            .when("/TempClosureMemo/Process/:PageType", {
                templateUrl: "/Reimage/TempClosureMemo",
                controller: 'tempClosureMemoController'
            })
        /////---------SiteInfo
            .when("/SiteInfo", {
                templateUrl: '/Reimage/SiteInfo',
                controller: 'reimageSiteInfoCtrl'
            })
            .when("/SiteInfo/Process/:PageType", {
                templateUrl: '/Reimage/SiteInfo',
                controller: 'reimageSiteInfoCtrl'
            })
        ////---------Select Package
        .when("/PackageList", {
            templateUrl: '/Reimage/SelectPackage',
            controller: 'selectPackageCtrl'
        });
    }]);