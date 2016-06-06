marjorLeaseApp.controller("contractInfoController", [
    "$http",
    "$scope",
    "$window",
    '$location',
    "$routeParams",
    "contractService",
    'taskWorkService',
    'majorLeaseService',
    "messager",
    function ($http, $scope, $window, $location, $routeParams, contractService, taskWorkService, majorLeaseService, messager) {
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
        $scope.subFlowCode = "MajorLease_ContractInfo";
        $scope.checkPointRefresh = true;

        $scope.contract = {};

        majorLeaseService.getMajorInfo({ projectId: $scope.projectId }).$promise.then(function (data) {

            $scope.MajorLeaseInfo = data;

            if (data.IsContractInfoSaveable && $scope.pageType == 'View') {
                $scope.isPageEditable = $scope.isPageEditable || data.IsContractInfoSaveable;
            }

        }, function (error) {
            messager.showMessage(error.statusText, "fa-warning c_orange");
        });

        $scope.save = function () {
            $scope.saved = true;
            contractService.save($scope.contract, $scope.revisions).then(function (response) {
                //$scope.contract = response.data.Contract;
                //$scope.revisions = response.data.Revisions;
                messager.showMessage("[[[保存成功]]]", "fa-check c_orange").then(function () {
                    //   $location.path("Home/Personal");
                });
            }, function (error) {

            });
        };

        $scope.submit = function () {
            if ($.grep($scope.attlist || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "89186470-945b-417b-82a8-2dbc9b0df09b";
            }) == 0) {
                messager.showMessage("[[[请上传补充协议]]]", "fa-warning c_orange");
                return false;
            }
            $scope.submitted = true;
            contractService.save($scope.contract, $scope.revisions, $scope.subFlowCode).then(function (response) {
                //$scope.contract = response.data.Contract;
                //$scope.revisions = response.data.Revisions;
                messager.showMessage("[[[提交成功]]]", "fa-check c_orange").then(function () {
                    $location.path("Home/Personal");
                });
            });
        };
        $scope.confirm = function (frm) {
            if (frm.$valid) {
                if ($.grep($scope.attlist || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "89186470-945b-417b-82a8-2dbc9b0df09b";
                }) == 0) {
                    messager.showMessage("[[[请上传补充协议]]]", "fa-warning c_orange");
                    return false;
                }
                $scope.confirmed = true;
                contractService.submit($scope.contract, $scope.revisions, $scope.subFlowCode).then(function (result) {
                    $scope.contract = result.data.Contract;
                    $scope.revisions = result.data.Revisions;
                    taskWorkService.Complete($scope.projectId, $scope.subFlowCode).then(function (response) {
                        messager.showMessage("[[[确认成功]]]", "fa-check c_orange").then(function () {
                            $window.location = Utils.ServiceURI.WebAddress() + "/redirect";
                        });
                    });
                });
            }
        };
    }
]);