dictionaryApp.controller('contractInfoViewController', [
    '$scope',
    "$http",
    "$routeParams",
    "contractService",
    "messager",
    function ($scope, $http, $routeParams, contractService, messager) {
        $scope.projectId = $routeParams.projectId;
        $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);
        $scope.checkPointRefresh = true;
        $scope.contract = {};
        $scope.flowCode = "Closure_ContractInfo";

        $http.get(Utils.ServiceURI.Address() + "api/project/isFlowSavable/" + $scope.projectId + "/" + $scope.flowCode).success(function (data) {
            if (data) {
                $scope.savable = data.Savable;
            }
        });

        $scope.save = function () {
            $scope.saved = true;
            contractService.save($scope.contract, $scope.revisions || []).then(function (response) {
                messager.showMessage("[[[保存成功]]]", "fa-check c_green").then(function () {
                    $scope.saved = false;
                });
            }, function () {
                messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                $scope.saved = false;
            });
        };
    }]);