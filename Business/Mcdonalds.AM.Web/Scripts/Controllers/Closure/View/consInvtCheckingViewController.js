dictionaryApp.controller('consInvtCheckingViewController', [
    '$scope',
    "$http",
    "$routeParams",
    "$modal",
    "$window",
    'closureCreateHandler',
    "messager",
    "$location",
    "redirectService",
    function ($scope, $http, $routeParams, $modal, $window, closureCreateHandler, messager, $location, redirectService) {
        
        var procInstID = $routeParams.ProcInstID;
        var sn = $routeParams.SN;
        $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);
        $scope.projectId = $routeParams.projectId;
        $scope.entity = {};
        $scope.entity.ProjectId = $scope.projectId;
        $scope.checkPointRefresh = true;
        $scope.isHistory = $routeParams.isHistory;
        $scope.entityId = $routeParams.entityId;
        $scope.flowCode = "Closure_ConsInvtChecking";
        loadConsInvtData();


        $http.get(Utils.ServiceURI.Address() + "api/project/isFlowSavable/" + $scope.projectId + "/" + $scope.flowCode).success(function (data) {
            if (data) {
                $scope.savable = data.Savable;
            }
        });

        function loadConsInvtData() {
            var url = "";
            if ($scope.isHistory && $scope.entityId) {
                url = Utils.ServiceURI.Address() + "api/ClosureConsInvtChecking/GetById/" + $scope.entityId;
            } else {
                url = Utils.ServiceURI.Address() + "api/ClosureConsInvtChecking/GetByProjectId/" + $scope.projectId;
            }
            $http.get(url).success(function (data) {
                $scope.entity = data;
            }).then(function (data) {
                if ($scope.entity != "null") {
                    $http.get(Utils.ServiceURI.Address() + "api/ClosureConsInvtChecking/GetTemplates/" + $scope.entity.Id.toString()).success(function (atts) {
                        $scope.templateList = atts;
                        for (var i = 0; i < atts.length; i++) {
                            atts[i].IsAvailable = (i == atts.length - 1) ? "[[[生效]]]" : "[[[失效]]]";
                        }
                    });
                    $scope.enableReCall = false;
                    $scope.enableEdit = false;

                    //判断流程是否进入K2
                    if ($scope.entity.ProcInstID) {

                        var roleCode = "PM";

                        //判断当前用户是否是PM
                        $http.get(Utils.ServiceURI.Address() + "api/ProjectUsers/IsExists/" + $scope.projectId + "/" + window.currentUser.Code + "/" + roleCode).success(function (isEditor) {

                            if (isEditor == "true") {

                                $http.get(Utils.ServiceURI.Address() + "api/project/isFinished/" + $scope.projectId + "/Closure_ConsInvtChecking").success(function (isFinished) {
                                    if (isFinished == "true") {
                                        $scope.enableEdit = true;
                                    } else {
                                        $http.get(Utils.ServiceURI.Address() + "api/project/EnableReCall/ClosureConsInvtChecking/" + $scope.entity.Id + "/" + $scope.entity.ProjectId).success(function (isStart) {
                                            if (isStart == "true") {
                                                $scope.enableReCall = true;
                                            }
                                        });
                                    }
                                });



                            }


                        });
                    }
                };
            });





        }

        $scope.beginReCall = function (conEntity) {

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
                        return angular.copy(conEntity);
                    }
                }

            }).result.then(function (storeEntity) {

                $scope.entity.Comments = storeEntity.Comment;

                $scope.recall();


            });

        };

        $scope.edit = function () {
            messager.confirm("[[[Construction Investment Checking 审批流程已经完成，现在编辑会导致流程重新提交，是否确认？]]]", "fa-warning c_orange").then(function (result) {

                if (result) {
                    $scope.entity.UserAccount = window.currentUser.Code;
                    $http.post(Utils.ServiceURI.Address() + "api/ClosureConsInvtChecking/Edit", $scope.entity).success(function (data) {
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

        $scope.recall = function () {
            $scope.entity.UserAccount = window.currentUser.Code;
            $http.post(Utils.ServiceURI.Address() + "api/ClosureConsInvtChecking/Recall", $scope.entity).success(function (data) {
                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                    //$window.location = Utils.ServiceURI.WebAddress() + "/redirect";
                    messager.unBlockUI();
                    redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                });
            }).error(function (data) {
                messager.showMessage("[[[撤回失败]]]", "fa-warning c_orange");
            });
        }


        $scope.save = function () {

            $scope.entity.UserAccount = window.currentUser.Code;
            $http.post(Utils.ServiceURI.Address() + "api/ClosureConsInvtChecking/Save", $scope.entity).success(function (data) {
                messager.showMessage("[[[保存成功]]]", "fa-check c_green").then(function () {
                });
            }).error(function (data) {
                messager.showMessage("[[[保存失败]]]", "fa-warning c_red");
            });
        }

        $scope.isUploadTemplate = null;

        $scope.$watch("isUploadTemplate", function (val) {
            if (!!val) {
                loadConsInvtData();
                $scope.isUploadTemplate = false;
            }

        });

        $scope.$watch("(entity.RECostActual - entity.RECostBudget) / entity.RECostBudget", function (newval, oldval) {
            if (!isNaN(newval) && newval != oldval) {
                var temp = $scope.entity.RECostActual - $scope.entity.RECostBudget;
                if ($scope.entity.RECostBudget != 0)
                    $scope.recostVar = temp / $scope.entity.RECostBudget;
            }
        });

        $scope.$watch("(entity.LHIActual - entity.LHIBudget)  / entity.LHIBudget", function (newval, oldval) {
            if (!isNaN(newval) && newval != oldval) {
                var temp = $scope.entity.LHIActual - $scope.entity.LHIBudget;
                if ($scope.entity.LHIBudget != 0)
                    $scope.recostVar2 = temp / $scope.entity.LHIBudget;
            }
        });

        $scope.$watch("(entity.EquipmentActual + entity.SignageActual + entity.SeatingActual + entity.DecorationActual - entity.ESSDBudget) / entity.ESSDBudget", function (newval, oldval) {
            if (!isNaN(newval) && newval != oldval) {
                var temp = $scope.entity.EquipmentActual + $scope.entity.SignageActual + $scope.entity.SeatingActual + $scope.entity.DecorationActual - $scope.entity.ESSDBudget;
                if ($scope.entity.ESSDBudget != 0)
                    $scope.recostVar3 = temp / $scope.entity.ESSDBudget;
            }
        });

        $scope.$watch("(entity.TotalActual -entity.TotalWriteoffBudget) / entity.TotalWriteoffBudget", function (newval, oldval) {
            if (!isNaN(newval) && newval != oldval) {
                var temp = $scope.entity.TotalActual - $scope.entity.TotalWriteoffBudget;
                if ($scope.entity.TotalWriteoffBudget != 0)
                    $scope.recostVar4 = temp / $scope.entity.TotalWriteoffBudget;
            }
        });

    }]);

