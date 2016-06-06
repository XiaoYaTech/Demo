marjorLeaseApp.controller("siteInfoController", [
    "$http",
    "$scope",
    "$window",
    '$location',
    "$routeParams",
    "majorLeaseService",
    "siteInfoService",
    'taskWorkService',
    'redirectService',
    "messager",
    function ($http, $scope, $window, $location, $routeParams, majorLeaseService, siteInfoService, taskWorkService, redirectService, messager) {
        $scope.pageType = $routeParams.PageType;
        switch ($routeParams.PageType) {
            //case 'Approval':  
            case 'View':
                $scope.isPageEditable = false;
                break;
            default:
                $scope.isPageEditable = true;
                break;
        }
        $scope.userAccount = window.currentUser.Code;
        $scope.projectId = $routeParams.projectId;
        $scope.subFlowCode = "MajorLease_SiteInfo";
        $scope.checkPointRefresh = true;
        $scope.majorLeaseInfo = {};
        majorLeaseService.getMajorInfo({ projectId: $routeParams.projectId }).$promise.then(function (data) {
            if (data != null) {
                $scope.majorLeaseInfo = data;

                if (data.IsSiteInfoSaveable && $scope.pageType == 'View') {
                    $scope.isPageEditable = $scope.isPageEditable || data.IsSiteInfoSaveable;
                }
            }
        });

        var save = function (action) {
            siteInfoService.saveSiteInfo($scope.siteInfo).$promise.then(function () {
                if (action == "save") {
                    messager.showMessage("[[[保存成功]]]", "fa-warning c_orange");
                }
                else if (action == "submit") {
                    messager.showMessage("[[[提交成功]]]", "fa-check c_orange").then(function () {
                        $location.path("Home/Personal");
                    });
                }
            }, function (error) {
                messager.showMessage(error.statusText + "in save site info", "fa-warning c_orange");
            });
        };
        $scope.save = function () {
            $scope.IsClickSave = true;
            save('save');
        };
        $scope.submit = function () {
            $scope.IsClickSubmit = true;
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
            if ($scope.checkValidity()
                && frm.$valid) {
                $scope.confirmed = true;
                siteInfoService.submitSiteInfo($scope.siteInfo).$promise.then(function (result) {
                    taskWorkService.Complete($scope.projectId, $scope.subFlowCode).then(function (response) {
                        messager.showMessage("[[[确认成功]]]", "fa-check c_orange").then(function () {
                            redirectService.flowRedirect($scope.subFlowCode, $scope.projectId);
                        });
                    });
                });
            }
        };
    }
]);