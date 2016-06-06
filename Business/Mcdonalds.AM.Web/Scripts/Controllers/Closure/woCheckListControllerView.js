dictionaryApp.controller('woCheckListViewController', [
    '$scope',
    "$http",
    "$modal",
    "$window",
    '$routeParams',
    'closureCreateHandler',
    "messager",
    '$location',
    "redirectService",
    function ($scope, $http, $modal, $window, $routeParams, closureCreateHandler, messager, $location, redirectService) {
        
        var procInstID = $routeParams.ProcInstID;

        $scope.pageUrl = window.location.href;
        //检查点用到的三个参数
        $scope.projectId = $routeParams.projectId;
        $scope.flowCode = "Closure_WOCheckList";
        $scope.checkPointRefresh = false;
    
        $scope.isHistory = $routeParams.isHistory;
        $scope.entityId = $routeParams.entityId;
        $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);

        loadData();


        function loadData() {

            $scope.store = {};
            $scope.isEditor = false;
            $scope.checkPointRefresh = true;
            //获取项目基本数据
            $http.get(Utils.ServiceURI.Address() + "api/Closure/GetByProjectId/" + $scope.projectId).success(function (data) {
                if (data != "null") {
                    $scope.ClosureInfo = data;

                }
            });

            var url;
            if ($scope.isHistory && $scope.entityId) {
                url = Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetById/" + $scope.entityId;
            } else {
                url = Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetByProjectId/" + $scope.projectId;
            }

            $http.get(url).success(function (data) {
                $scope.entity = data;


            }).then(function () {
                loadDateEx();
            });
        }

        function loadDateEx() {
            if ($scope.entity != "null") {
                $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetTemplates/" + $scope.entity.Id.toString()).success(function (atts) {

                    $scope.templateList = atts;

                    for (var i = 0; i < atts.length; i++) {
                        atts[i].IsAvailable = (i == atts.length - 1) ? "[[[生效]]]" : "[[[失效]]]";
                    }

                    $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetHistoryList/" + $scope.projectId).success(function (historyData) {
                        $scope.historyList = historyData;
                    }).error(function (err) {
                        var s = err;
                    });

                    $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetClosureCommers/ClosureWOCheckList/" + $scope.entity.Id.toString()).success(function (closureCommers) {
                        $scope.closureCommers = closureCommers;

                    });

                    $scope.enableReCall = false;
                    $scope.enableEdit = false;

                    //判断流程是否进入K2
                    if ($scope.entity.ProcInstID) {

                        var roleCode = "PM";

                        //判断当前用户是否是PM
                        $http.get(Utils.ServiceURI.Address() + "api/ProjectUsers/IsExists/" + $scope.projectId + "/" + window.currentUser.Code + "/" + roleCode).success(function (isEditor) {

                            if (isEditor == "true") {

                                $http.get(Utils.ServiceURI.Address() + "api/project/isFinished/" + $scope.entity.ProjectId + "/" + $scope.flowCode).success(function (isFinished) {
                                    if (isFinished == "true") {
                                        $scope.enableEdit = true;
                                    } else {
                                        $http.get(Utils.ServiceURI.Address() + "api/project/EnableReCall/ClosureWOCheckList/" + $scope.entity.Id + "/" + $scope.entity.ProjectId).success(function (isStart) {
                                            if (isStart == "true") {
                                                $scope.enableReCall = true;
                                            }
                                        });
                                    }
                                });
                               
                            }


                        });
                    }


                });
            };
        }

        $scope.editWOCheckList = function () {
            messager.confirm("[[[WO Check List 审批流程已经完成，现在编辑会导致流程重新提交，是否确认？]]]", "fa-warning c_orange").then(function (result) {

                if (result) {
                    $scope.entity.UserAccount = window.currentUser.Code;
                    $http.post(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/Edit", $scope.entity).success(function (data) {
                        messager.showMessage("[[[操作成功]]]", "fa-check c_green").then(function () {
                            //var url = "/Closure/WOCheckList/" + $scope.projectId;
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

        $scope.recallWOCheckList = function () {
            $scope.entity.UserAccount = window.currentUser.Code;
            $http.post(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/Recall", $scope.entity).success(function (data) {
                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                    //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                    messager.unBlockUI();
                    redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                });
            }).error(function (data) {
                messager.showMessage("[[[撤回失败]]]", "fa-warning c_orange");
            });
        }

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

                $scope.entity.Comments = storeEntity.Comment;

                $scope.recallWOCheckList();


            });

        };


    }]);