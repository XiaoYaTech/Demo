dictionaryApp.controller('contractInfoController', [
    '$scope',
    "$http",
    "$routeParams",
    "$location",
    "contractService",
    "messager",
    "redirectService",
    function ($scope, $http, $routeParams, $location, contractService, messager, redirectService) {
        $scope.projectId = $routeParams.projectId;
        if (!$scope.projectId) {
            $scope.projectId = $routeParams.param;
        }

        $scope.flowCode = "Closure_ContractInfo";
        $scope.checkPointRefresh = true;
        $scope.contract = {};
        $scope.save = function () {
            $scope.saved = true;
            contractService.save($scope.contract, $scope.revisions || [], $scope.flowCode).then(function (response) {
                //$scope.contract = response.data.Contract;
                //$scope.revisions = response.data.Revisions;
                messager.showMessage("[[[保存成功]]]", "fa-check c_green").then(function () {
                    $scope.saved = false;
                    //   $location.path("Home/Personal");
                });
            }, function () {
                messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                $scope.saved = false;
            });
        };

        $http.get(Utils.ServiceURI.Address() + "api/Closure/GetByProjectId/" + $scope.projectId).success(function (data) {
            if (data.ClosureTypeNameENUS == "Lease Expiry")
                $scope.checkAtt = false;
            else
                $scope.checkAtt = true;
        });

        $scope.submit = function (frm) {
            if (frm.$valid) {
                if ($scope.checkAtt && $.grep($scope.attlist || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "b728ce63-92d2-4773-8e05-d0def6bbe2f5";
                }) == 0) {
                    messager.showMessage("[[[请上传Signed Termination Agreement]]]", "fa-warning c_orange");
                    return false;
                }
                $scope.submitted = true;
                contractService.submit($scope.contract, $scope.revisions || [], $scope.flowCode).then(function (response) {
                    $scope.contract = response.data.Contract;
                    $scope.revisions = response.data.Revisions;
                    messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                        //window.location.href = Utils.ServiceURI.WebAddress() + "/redirect";
                        messager.unBlockUI();
                        redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                    });
                }, function () {
                    messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                    $scope.submitted = false;
                });
            }
        };
    }]);