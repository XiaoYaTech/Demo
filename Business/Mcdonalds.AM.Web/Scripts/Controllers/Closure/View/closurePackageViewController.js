dictionaryApp.controller('closurePackageViewController', [
    '$scope',
    "$http",
    "$modal",
    "$window",
    '$routeParams',
    'closureCreateHandler',
    "messager",
    "redirectService",
    "$location",
    function ($scope, $http, $modal, $window, $routeParams, closureCreateHandler, messager, redirectService, $location) {

        var procInstID = $routeParams.ProcInstID;
        $scope.projectId = $routeParams.projectId;
        $scope.isHistory = $routeParams.isHistory;
        $scope.entityId = $routeParams.entityId;
        $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);
        $scope.pageUrl = window.location.href;
        $scope.entity = {};
        $scope.ClosureTool = {};
        $scope.flowCode = "Closure_ClosurePackage";
        loadData();
        $scope.isHistory = $routeParams.isHistory;
        $scope.uploadSet = [];

        $scope.$watch("entity.OtherCFNPV + entity.NewSiteNetCFNPV + ClosureTool.Compensation + entity.OriginalCFNPV", function () {
            try {
                if (!$scope.ClosureTool.Compensation) {
                    $scope.ClosureTool.Compensation = 0;
                }
                if (!$scope.entity.OtherCFNPV) {
                    $scope.entity.OtherCFNPV = 0;
                }
                if (!$scope.entity.NewSiteNetCFNPV) {
                    $scope.entity.NewSiteNetCFNPV = 0;
                }
                if (!$scope.entity.OriginalCFNPV) {
                    $scope.entity.OriginalCFNPV = 0;
                }
                $scope.entity.NetGain = (parseFloat($scope.entity.OtherCFNPV) + parseFloat($scope.entity.NewSiteNetCFNPV) + parseFloat($scope.ClosureTool.Compensation) - parseFloat($scope.entity.OriginalCFNPV));
            } catch (e) {
                $scope.entity.NetGain = null;
            }
            if (!$scope.entity.NetGain) {
                $scope.entity.NetGain = "0";
            }
        });

        function loadData() {
            $scope.isEditor = false;
            $scope.checkPointRefresh = true;

            $http.get(Utils.ServiceURI.Address() + "api/ClosurePackage/Get", {
                cache: false,
                params: {
                    projectId: $scope.projectId,
                    id: $scope.entityId
                }
            }).success(function (data) {
                $scope.entity = data.Package;
                if ($scope.entity) {
                    if ($scope.entity.PipelineName == null)
                        $scope.entity.PipelineName = "[[[无记录]]]";
                }
                $scope.ClosureInfo = data.ClosureInfo;
                $scope.ClosureTool = data.ClosureTool;
                $scope.ClosureWOCheckList = data.ClosureWoCheckList;
                $scope.enableReCall = data.Recallable;
                $scope.enableEdit = data.Editable;
                $scope.signedPackageSavable = data.SignedPackageSavable;
                if (data.SignedPackageSavable)
                    $scope.uploadSet.push("15570aac-42e5-4a75-880b-ec742fb4a92d");
                //获取Store信息
                $http.get(Utils.ServiceURI.Address() + "api/Store/Details/" + data.ClosureInfo.USCode).success(function (storeData) {
                    $scope.store = storeData;
                    //$scope.entity.RelocationPipelineID = storeData.StoreBasicInfo.PipelineID;
                    //$scope.entity.PipelineName = storeData.StoreBasicInfo.PipelineNameENUS == null ? "无记录" : storeData.StoreBasicInfo.PipelineNameENUS;
                });
                $http.get(Utils.ServiceURI.Address() + "api/project/isFlowSavable/" + $scope.projectId + "/" + $scope.flowCode).success(function (data) {
                    if (data) {
                        $scope.enableSave = data.Savable;
                        if (data.Savable) {
                            $scope.uploadSet.push("1d548fdd-15e6-42a2-afc7-c972e7f2e6d1");
                        }
                    }
                });
            });
        }

        $scope.editPackage = function () {
            messager.confirm("[[[Closure Package 审批流程已经完成，现在编辑会导致流程重新提交，是否确认？]]]", "fa-warning c_orange").then(function (result) {

                if (result) {
                    $scope.entity.UserAccount = window.currentUser.Code;
                    $scope.entity.ProcInstID = procInstID;
                    $scope.entity.ProjectId = $scope.projectId;
                    $http.post(Utils.ServiceURI.Address() + "api/ClosurePackage/Edit", $scope.entity).success(function (data) {
                        messager.showMessage("[[[操作成功]]]", "fa-check c_green").then(function () {
                            //var url = "/Closure/ClosurePackage/" + $scope.projectId;
                            //$location.path(url);
                            //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                            messager.unBlockUI();
                            $window.location.href = Utils.ServiceURI.WebAddress() + data.TaskUrl;
                        });
                    }).error(function (data) {
                        messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                    });
                }
            });
        };

        $scope.recallPackage = function () {
            $scope.entity.UserAccount = window.currentUser.Code;
            $http.post(Utils.ServiceURI.Address() + "api/ClosurePackage/Recall", $scope.entity).success(function (data) {
                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                    //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                    messager.unBlockUI();
                    redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                });
            }).error(function (data) {
                messager.showMessage("[[[撤回失败]]]", "fa-warning c_orange");
            });
        }

        $scope.beginReCall = function () {

            $modal.open({
                templateUrl: "/Template/Recall",
                backdrop: 'static',
                size: 'lg',
                controller: [
                    "$scope", "$modalInstance", function ($scope, $modalInstance) {

                        $scope.entity = {};

                        $scope.ok = function (e) {

                            $modalInstance.close($scope.entity);
                        };
                        $scope.cancel = function () {
                            $modalInstance.dismiss("cancel");
                        };
                    }
                ]

            }).result.then(function (entity) {

                $scope.entity.Comments = entity.Comment;

                $scope.recallPackage();


            });

        };

        $scope.savePackage = function () {
            $scope.entity.ProjectId = $scope.projectId;

            $scope.entity.UserAccount = window.currentUser.Code;
            $scope.entity.UserNameZHCN = window.currentUser.NameZHCN;
            $scope.entity.UserNameENUS = window.currentUser.NameENUS;
            messager.blockUI("[[[正在处理中，请稍等...]]]");

            var url = Utils.ServiceURI.Address() + "api/ClosurePackage/SaveClosurePackage";
            $http.post(url, $scope.entity).success(function (data) {
                messager.unBlockUI();
                messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                $scope.entity.Id = data.replace("\"", "").replace("\"", "");
            }).error(function (data) {
                messager.showMessage("[[[保存失败]]]", "fa-check c_orange");
            });
        };
    }]);

