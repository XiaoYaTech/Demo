rebuildApp.controller("siteInfoController", [
    "$http",
    "$scope",
    "$window",
    '$location',
    "$routeParams",
    "rebuildService",
    "siteInfoService",
    'taskWorkService',
    "redirectService",
    "messager",
    function ($http, $scope, $window, $location, $routeParams, rebuildService, siteInfoService, taskWorkService, redirectService, messager) {
        $scope.pageType = $routeParams.PageType;
        $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);
        switch ($routeParams.PageType) {
            case 'Approval':
            case 'View':
                $scope.isPageEditable = false;
                break;
            default:
                $scope.isPageEditable = true;
                break;
        }
        $scope.userAccount = window.currentUser.Code;
        $scope.projectId = $routeParams.projectId;
        $scope.subFlowCode = "Rebuild_SiteInfo";
        $scope.checkPointRefresh = true;
        $scope.rebuildInfo = {};
        rebuildService.initSiteInfoPage({ projectId: $scope.projectId }).$promise.then(function (data) {
            if (data != null) {
                $scope.rebuildInfo = data.Info;
                $scope.isOriginator = data.IsOriginator;
                $scope.estimatedVsActualConstruction = data.EstimatedVsActualConstruction;
                $scope.estimatedVsActualConstruction.NewOperationSize = $scope.siteInfo.TotalArea;
                $scope.estimatedVsActualConstruction.ARSN = $scope.siteInfo.TotalSeatsNo;
                $scope.isPageEditable = $scope.isPageEditable || data.IsShowSave;
                $scope.IsShowSave = data.IsShowSave;
            }
        });

        //$scope.$watch('siteInfo.DesignStyle', function (val) {
        //    if (val && $scope.estimatedVsActualConstruction) {
        //        $scope.estimatedVsActualConstruction.ARDC = val;
        //    }
        //});

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

        $scope.save = function () {
            if (!$scope.checkValidity()) {
                return;
            }
            $scope.acting = true;
            $scope.siteInfo.EstimatedVsActualConstruction = $scope.estimatedVsActualConstruction;
            rebuildService.saveSiteInfo($scope.siteInfo).$promise.then(function () {
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

            var gbDate = $scope.estimatedVsActualConstruction.GBDate;
            var consDate = $scope.estimatedVsActualConstruction.CompletionDate;
            if (gbDate != null && consDate != null) {
                gbDate = moment(new Date(gbDate).toDateString());
                consDate = moment(new Date(consDate).toDateString());
                if (gbDate.isBefore(new Date().toDateString())) {
                    messager.showMessage("[[[开工日期不能早于今天]]]", "fa-warning c_orange");
                    return false;
                }

                if (consDate.isBefore(gbDate)) {
                    messager.showMessage("[[[完工日期不能早于开工日期]]]", "fa-warning c_orange");
                    return false;
                }
            }
            return true;
        }
        $scope.confirm = function (frm) {
            if (frm.$valid && $scope.checkValidity()) {
                $scope.confirmed = true;
                $scope.siteInfo.EstimatedVsActualConstruction = $scope.estimatedVsActualConstruction;
                rebuildService.saveSiteInfo($scope.siteInfo).$promise.then(function (result) {
                    taskWorkService.Complete($scope.projectId, $scope.subFlowCode).then(function (response) {
                        messager.showMessage("[[[提交成功]]]", "fa-check c_orange").then(function () {
                            redirectService.flowRedirect("Rebuild_SiteInfo", $scope.projectId);
                        });
                    });
                });
            }
        };
        $scope.submit = function () {
            $scope.acting = true;
            $scope.siteInfo.EstimatedVsActualConstruction = $scope.estimatedVsActualConstruction;
            rebuildService.submitSiteInfo($scope.siteInfo).$promise.then(function () {
                messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                    redirectService.flowRedirect("Rebuild_SiteInfo", $scope.projectId);
                });
            }, function (error) {
                messager.showMessage(error.statusText + " in save site info", "fa-warning c_orange");
                $scope.acting = false;
            });
        };
    }
]);