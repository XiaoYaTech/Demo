rebuildApp.controller("contractInfoController", [
    "$http",
    "$scope",
    "$window",
    '$location',
    "$routeParams",
    "contractService",
    'taskWorkService',
    "redirectService",
    "messager",
    function ($http, $scope, $window, $location, $routeParams, contractService, taskWorkService, redirectService, messager) {
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
        $scope.subFlowCode = "Rebuild_ContractInfo";
        $scope.checkPointRefresh = true;

        $scope.contract = {};

        messager.blockUI("[[[正在初始化页面，请稍等]]]...");
        contractService.querySaveable($scope.projectId).then(function (data) {
            messager.unBlockUI();
            $scope.IsShowSave = data.data.IsShowSave;
            $scope.isPageEditable = $scope.isPageEditable || data.data.IsShowSave;
        }, function () {
            messager.unBlockUI();
            messager.showMessage("[[[页面初始化出错]]]", "fa-warning c_orange").then(function () {
                //$window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
            });
        });

        $scope.save = function () {
            $scope.acting = true;
            contractService.save($scope.contract, $scope.revisions || [], $scope.subFlowCode).then(function (response) {
                //$scope.contract = response.data.Contract;
                //$scope.revisions = response.data.Revisions;
                messager.showMessage("[[[保存成功]]]", "fa-check c_green").then(function () {
                    $scope.acting = false;
                    //   $location.path("Home/Personal");
                });
            }, function () {
                messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                $scope.acting = false;
            });
        };

        $scope.submit = function (frm) {
            if (frm.$valid) {
                if ($.grep($scope.attlist || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "cb98ef0b-5793-4b4c-bd29-58272fffda22";
                }) == 0) {
                    messager.showMessage("[[[请上传补充协议]]]", "fa-warning c_orange");
                    return false;
                }
                $scope.acting = true;
                contractService.submit($scope.contract, $scope.revisions || [], $scope.subFlowCode).then(function (response) {
                    $scope.contract = response.data.Contract;
                    $scope.revisions = response.data.Revisions;
                    messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                        redirectService.flowRedirect("Rebuild_ContractInfo", $scope.projectId);
                    });
                }, function () {
                    messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                    $scope.acting = false;
                });
            }
        };
    }
]);