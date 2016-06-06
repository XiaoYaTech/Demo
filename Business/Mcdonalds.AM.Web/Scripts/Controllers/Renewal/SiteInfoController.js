!function () {
    var RenewalApp = angular.module("amApp");
    RenewalApp.controller('siteInfoController', [
        '$scope',
        "$http",
        "$routeParams",
        "$window",
        "$location",
        "siteInfoService",
        "messager",
        "renewalService",
        "redirectService",
        function ($scope, $http, $routeParams, $window, $location, siteInfoService, messager, renewalService, redirectService) {
            $scope.projectId = $routeParams.projectId;
            $scope.from = $routeParams.from;
            $scope.checkPointRefresh = true;
            messager.blockUI("[[[正在初始化页面，请稍等]]]...");
            $scope.open = function ($event, ele) {
                $event.preventDefault();
                $event.stopPropagation();

                $scope[ele] = true;
            };

            $scope.$watch('siteInfo.DesignStyle', function (val) {
                if (val && $scope.estimatedVsActualConstruction) {
                    $scope.estimatedVsActualConstruction.ARDC = val;
                }
            });

            $scope.$watch('estimatedVsActualConstruction.ARDC', function (val) {
                if (val && $scope.siteInfo) {
                    $scope.siteInfo.DesignStyle = val;
                }
            });

            $scope.$watch('siteInfo.TotalArea', function (val) {
                if (val && $scope.estimatedVsActualConstruction) {
                    $scope.estimatedVsActualConstruction.NewOperationSize = val;
                }
            });

            $scope.$watch('siteInfo.TotalSeatsNo', function (val) {
                if (val && $scope.estimatedVsActualConstruction) {
                    $scope.estimatedVsActualConstruction.ARSN = val;
                }
            });

            renewalService.initSiteInfoPage({ projectId: $scope.projectId }).$promise.then(function (data) {
                messager.unBlockUI();
                $scope.info = data.Info;
                $scope.savable = data.Savable;
                $scope.estimatedVsActualConstruction = data.EstimatedVsActualConstruction;
                $scope.estimatedVsActualConstruction.NewOperationSize = $scope.siteInfo.TotalArea;
                $scope.estimatedVsActualConstruction.ARSN = $scope.siteInfo.TotalSeatsNo;
            }, function () {
                messager.unBlockUI();
                messager.showMessage("[[[页面初始化出错]]]", "fa-warning c_orange").then(function () {
                    $window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
                });
            });
            var formatEstiDate = function (esti) {
                esti.GBDate = moment(esti.GBDate).format("YYYY-MM-DD");
                esti.CompletionDate = moment(esti.CompletionDate).format("YYYY-MM-DD");
                esti.ActualGBDate = moment(esti.ActualGBDate).format("YYYY-MM-DD");
                esti.ActualCompletionDate = moment(esti.ActualCompletionDate).format("YYYY-MM-DD");
            };
            $scope.save = function () {
                $scope.acting = true;
                $scope.siteInfo.Identifier = $scope.estimatedVsActualConstruction.RefId;
                $scope.siteInfo.EstimatedVsActualConstruction = $scope.estimatedVsActualConstruction;
                formatEstiDate($scope.siteInfo.EstimatedVsActualConstruction);
                renewalService.saveSiteInfo($scope.siteInfo).$promise.then(function () {
                    messager.showMessage("[[[保存成功]]]", "fa-warning c_green");
                    $scope.acting = false;
                }, function (error) {
                    messager.showMessage(error.statusText + " in save site info", "fa-warning c_orange");
                    $scope.acting = false;
                });
            };

            $scope.checkValidity = function () {
                if ($scope.siteInfo
                    && $scope.siteInfo.FrontCounterSeats - $scope.siteInfo.TotalSeatsNo > 0) {
                    messager.showMessage("[[[柜台楼层座位数不能大于总座位数]]]！", "fa-warning c_orange");
                    return false;
                }
                return true;
            }

            $scope.submit = function (frm) {
                if ($scope.checkValidity()
                    && frm.$valid) {
                    $scope.acting = true;
                    $scope.siteInfo.Identifier = $scope.estimatedVsActualConstruction.RefId;
                    $scope.siteInfo.EstimatedVsActualConstruction = $scope.estimatedVsActualConstruction;
                    formatEstiDate($scope.siteInfo.EstimatedVsActualConstruction);
                    renewalService.submitSiteInfo($scope.siteInfo).$promise.then(function () {
                        messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect("Renewal_SiteInfo", $scope.projectId);
                        });
                    }, function (error) {
                        messager.showMessage(error.statusText + " in save site info", "fa-warning c_orange");
                        $scope.acting = false;
                    });
                }
            };
        }]);
}();