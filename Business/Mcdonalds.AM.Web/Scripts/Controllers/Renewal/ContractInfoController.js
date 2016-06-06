!function () {
    var RenewalApp = angular.module("amApp");
    RenewalApp.controller('contractInfoController', [
        '$scope',
        "$http",
        "$routeParams",
        "$location",
        "contractService",
        "messager",
        "renewalService",
        "redirectService",
    function ($scope, $http, $routeParams, $location, contractService, messager, renewalService, redirectService) {
        $scope.projectId = $routeParams.projectId;
        $scope.from = $routeParams.from;
        $scope.checkPointRefresh = true;
        $scope.contract = {};
        var init = function () {
            messager.blockUI("[[[正在初始化页面，请稍等]]]...");
            renewalService.initContractInfoPage({
                projectId: $scope.projectId,
                id: $routeParams.entityId
            }).$promise.then(function (data) {
                messager.unBlockUI();
                $scope.savable = data.Savable;
            }, function () {
                messager.unBlockUI();
                messager.confirm("[[[页面初始化出错,点击确定重新加载]]]？", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        init();
                    } else {
                        $window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
                    }
                });
            });
        }
        init();
        $scope.save = function () {
            $scope.acting = true;
            contractService.save($scope.contract, $scope.revisions || [], "Renewal_ContractInfo").then(function (response) {
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
                    return !!att.FileURL && att.RequirementId == "4d8002c0-d5f1-4bb5-b756-7fe9dc25a019";
                }) == 0) {
                    messager.showMessage("[[[请上传Signed Renewal Contract]]]", "fa-warning c_orange");
                    return false;
                }
                $scope.acting = true;
                contractService.submit($scope.contract, $scope.revisions || [], "Renewal_ContractInfo").then(function (response) {
                    $scope.contract = response.data.Contract;
                    $scope.revisions = response.data.Revisions;
                    messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                        redirectService.flowRedirect("Renewal_ContractInfo", $scope.projectId);
                    });
                }, function () {
                    messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                    $scope.acting = false;
                });
            }
        };
    }]);
}();