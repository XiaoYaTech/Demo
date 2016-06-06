dictionaryApp.controller('closureLegalReviewViewController', [
    '$scope',
    "$http",
    "$modal",
    "$window",
    '$routeParams',
    'closureCreateHandler',
    "messager",
    '$location',
    "contractService",
    "redirectService",
    function ($scope, $http, $modal, $window, $routeParams, closureCreateHandler, messager, $location, contractService, redirectService) {

        var procInstID = $routeParams.ProcInstID;
        $scope.projectId = $routeParams.projectId;

        $scope.flowCode = "Closure_LegalReview";
        $scope.entity = {};
        $scope.isHistory = $routeParams.isHistory;
        $scope.entityId = $routeParams.entityId;
        $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);
        loadData();
        $scope.pageUrl = window.location.href;



        function loadData() {

            $scope.checkPointRefresh = true;

            //获取项目基本数据
            $http.get(Utils.ServiceURI.Address() + "api/Closure/GetByProjectId/" + $scope.projectId).success(function (data) {
                $scope.ClosureInfo = data;
            });

            var url;
            if ($scope.isHistory && $scope.entityId) {
                url = Utils.ServiceURI.Address() + "api/LegalReview/GetById/" + $scope.entityId;
            } else {
                url = Utils.ServiceURI.Address() + "api/LegalReview/GetByProjectId/" + $scope.projectId + "/" + window.currentUser.Code;
            }
            $http.get(url).success(function (fdata) {

                $scope.legalView = fdata;
            }).then(function (entity) {
                if ($scope.legalView != "null") {
                    $scope.LegalCommers = $scope.legalView.LegalCommers;

                    $scope.enableReCall = false;
                    $scope.enableEdit = false;

                    //判断流程是否进入K2
                    if ($scope.legalView.ProcInstID) {

                        var roleCode = "Asset Actor";

                        //判断当前用户是否是AssetActor
                        $http.get(Utils.ServiceURI.Address() + "api/ProjectUsers/IsExists/" + $scope.projectId + "/" + window.currentUser.Code + "/" + roleCode).success(function (isEditor) {

                            if (isEditor == "true") {


                                $http.get(Utils.ServiceURI.Address() + "api/project/isFinished/" + $scope.legalView.ProjectId + "/" + $scope.flowCode).success(function (isFinished) {
                                    if (isFinished == "true") {
                                        $scope.enableEdit = true;
                                    } else {
                                        $http.get(Utils.ServiceURI.Address() + "api/project/EnableReCall/ClosureLegalReview/" + $scope.legalView.Id + "/" + $scope.legalView.ProjectId).success(function (isStart) {
                                            if (isStart == "true") {
                                                $scope.enableReCall = true;
                                            }
                                        });
                                    }
                                });


                            }
                        });
                    }

                }
            });
        }

        $scope.editLegalReview = function () {
            messager.confirm("[[[Legal Review 审批流程已经完成，现在编辑会导致流程重新提交，是否确认？]]]", "fa-warning c_orange").then(function (result) {

                if (result) {
                    $scope.legalView.UserAccount = window.currentUser.Code;

                    $scope.legalView.ProjectId = $scope.projectId;
                    $http.post(Utils.ServiceURI.Address() + "api/LegalReview/Edit", $scope.legalView).success(function (data) {
                        messager.showMessage("[[[操作成功]]]", "fa-check c_green").then(function () {
                            //$window.location = Utils.ServiceURI.WebAddress() + "/redirect";
                            messager.unBlockUI();
                            $window.location.href = Utils.ServiceURI.WebAddress() + data.TaskUrl;
                        });
                    }).error(function (data) {
                        messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                    });
                }
            });
        };

        $scope.beginReCall = function (closureInfo) {

            $modal.open({
                templateUrl: "/Template/Recall",
                backdrop: 'static',
                size: 'lg',
                controller: [
                    "$scope", "$modalInstance", "storeEntity", function ($scope, $modalInstance, storeEntity) {



                        $scope.entity = {};

                        $scope.ok = function (e) {

                            $modalInstance.close($scope.entity);
                        };
                        $scope.cancel = function () {
                            $modalInstance.dismiss("cancel");
                        };
                    }
                ],
                resolve: {
                    storeEntity: function () {
                        return angular.copy(closureInfo);
                    }
                }

            }).result.then(function (storeEntity) {

                $scope.legalView.Comments = storeEntity.Comment;

                $scope.recallLegalReview();


            });

        };

        $scope.recallLegalReview = function () {

            $http.post(Utils.ServiceURI.Address() + "api/LegalReview/Recall", $scope.legalView).success(function (data) {
                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                    //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                    messager.unBlockUI();
                    redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                });
            }).error(function (data) {
                messager.showMessage("[[[撤回失败]]]", "fa-warning c_orange");
            });
        }
    }
]);