reimageApp.controller('reimageSiteInfoCtrl',
[
    "$scope",
    '$window',
    '$location',
    '$routeParams',
    "reimageService",
    'siteInfoService',
    'taskWorkService',
    'approveDialogService',
    'redirectService',
    '$modal',
     "messager",
    function ($scope, $window, $location, $routeParams, reimageService, siteInfoService, taskWorkService, approveDialogService, redirectService, $modal, messager) {
        $scope.pageType = $routeParams.PageType;
        $scope.projectId = $routeParams.projectId;
        $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);
        switch ($routeParams.PageType) {
            //case 'Approval':
            case 'View':
                $scope.isPageEditable = false;
                break;
            default:
                $scope.isPageEditable = true;
                break;
        }
        $scope.checkPointRefresh = true;
        $scope.isActor = true;
        $scope.subFlowCode = "Reimage_SiteInfo";

        reimageService.getReimageInfo({ projectId: $scope.projectId }).$promise.then(function (response) {
            $scope.reimageInfo = response;

            if (response.IsSiteInfoSaveable && $scope.pageType == 'View') {
                $scope.isPageEditable = $scope.isPageEditable || response.IsSiteInfoSaveable;
            }

            $scope.estimatedVsActualConstruction = response.EstimatedVsActualConstruction;

            if ($scope.siteInfo) {
                $scope.estimatedVsActualConstruction.NewOperationSize = $scope.siteInfo.TotalArea;
                $scope.estimatedVsActualConstruction.ARSN = $scope.siteInfo.TotalSeatsNo;
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

        var save = function (action) {
            $scope.siteInfo.ProjectIdentifier = $scope.reimageInfo.SiteInfoId;
            $scope.siteInfo.EstimatedVsActualConstruction = $scope.estimatedVsActualConstruction;
            siteInfoService.saveSiteInfo($scope.siteInfo).$promise.then(function () {
                if (action == "save") {
                    messager.showMessage("[[[保存成功]]]", "fa-warning c_orange");
                    $scope.acting = false;
                }
                else if (action == "submit") {
                    messager.showMessage("[[[提交成功]]]", "fa-check c_orange").then(function () {
                        redirectService.flowRedirect($scope.subFlowCode, $scope.projectId);
                    });
                }
            }, function (error) {
                messager.showMessage(error.statusText + "in save site info", "fa-warning c_orange");
                $scope.acting = false;
            });
        };

        $scope.save = function () {
            $scope.acting = true;
            save('save');
        };
        $scope.submit = function () {
            $scope.acting = true;
            save("submit");
        };

        $scope.checkValidity = function () {
            if ($scope.siteInfo
                && $scope.siteInfo.FrontCounterSeats - $scope.siteInfo.TotalSeatsNo > 0) {
                messager.showMessage("[[[柜台楼层座位数不能大于总座位数！]]]", "fa-warning c_orange");
                return false;
            }
            return true;
        }

        $scope.confirm = function (frm) {
            if (frm.$valid && $scope.checkValidity()) {
                $scope.acting = true;
                $scope.siteInfo.ProjectIdentifier = $scope.reimageInfo.SiteInfoId;
                $scope.siteInfo.EstimatedVsActualConstruction = $scope.estimatedVsActualConstruction;

                messager.confirm("确定要提交吗？", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        siteInfoService.submitSiteInfo($scope.siteInfo).$promise.then(function (result) {
                            taskWorkService.Complete($scope.projectId, $scope.subFlowCode).then(function (response) {
                                messager.showMessage("[[[提交成功]]]", "fa-check c_orange").then(function () {
                                    redirectService.flowRedirect($scope.subFlowCode, $scope.projectId);
                                });
                            });
                        });

                    }
                    else
                        $scope.IsClickSubmit = false;
                });
            };
        }

    }
]);