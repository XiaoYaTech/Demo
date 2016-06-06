/// <reference path="../Libs/moment/moment.d.ts" />
/// <reference path="../Libs/Angular/Plugins/angular-ui-bootstrap.d.ts" />
/// <reference path="../Libs/Angular/Plugins/angular-cookies.d.ts" />
/// <reference path="../Utils/Utils.ts" />
/// <reference path="../Libs/Angular/angular.d.ts" />
/// <reference path="../Utils/CurrentUser.ts" />
angular.module("mcd.am.modules.reimage", ['mcd.am.service.taskwork']).directive('keyMeasuresInfo', [
    "$http",
    "reimageService",
    "reinvestmentInfoService",
    "StoreProfitabilityAndLeaseInfoService",
    "summaryReinvestmentCostService",
    'storeService',
    function ($http, reimageService, reinvestmentInfoService, StoreProfitabilityAndLeaseInfoService, summaryReinvestmentCostService, storeService) {
        return {
            restrict: "E",
            replace: true,
            scope: {
                source: '=?',
                code: "=",
                projectId: "=",
                editable: '='
            },
            templateUrl: Utils.ServiceURI.AppUri + "Module/KeyMeasuresInfo",
            link: function ($scope, element, attrs) {
                if ($scope.source) {
                    $scope.reinvestment = $scope.source.ReinvestmentBasicInfo;
                    $scope.financialPreanalysis = $scope.source.FinancialPreanalysis;
                }

                StoreProfitabilityAndLeaseInfoService.get({
                    projectId: $scope.projectId
                }).$promise.then(function (data) {
                    if (!!data) {
                        $scope.store = data;
                    }
                });
                $scope.$watch("code", function (val) {
                    if (val) {
                        storeService.getStoreDetailInfo({ usCode: $scope.code }).$promise.then(function (result) {
                            if (result) {
                                $scope.storeBasicInfo = result.storeInfo;
                            }
                        });
                    }
                });
                summaryReinvestmentCostService.get({
                    projectId: $scope.projectId
                }).$promise.then(function (data) {
                    if (!!data) {
                        $scope.summary = data;
                    }
                });
            }
        };
    }]);
