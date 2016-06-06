dictionaryApp.controller('closureToolController', [
    '$scope',
    "$http",
    "$parse",
    "$routeParams",
    "$window",
    'closureCreateHandler',
    "messager",
    "$modal",
    "redirectService",
    function ($scope, $http, $parse, $routeParams, $window, closureCreateHandler, messager, $modal, redirectService) {

        $scope.projectId = $routeParams.projectId;
        $scope.entity = {};
        $scope.entity.ProjectId = $scope.projectId;
        //$scope.entity.IsOptionOffered = true;

        $scope.flowCode = "Closure_ClosureTool";
        $scope.checkPointRefresh = true;
        $scope.impactStoreEditable = false;
        $scope.decisionEditable = false;

        $scope.userAccount = window.currentUser.Code;
        $scope.userNameZHCN = escape(window.currentUser.NameZHCN);
        $scope.userNameENUS = escape(window.currentUser.NameENUS);


        $scope.isActor = false;

        $scope.entity.isChecked = false;
        $scope.impactStore1 = {};
        $scope.impactStore2 = {};
        $scope.hasClosureTool = false;

        $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetByProjectId/" + $scope.projectId).success(function (data) {
            $scope.woCheckList = data;

        });

        $scope.genClosureTool = function () {
            var u = Utils.ServiceURI.Address() + "api/ClosureTool/EnableGenClosureTool/" + $scope.entity.Id;
            $http.get(u).success(function (result) {

                if ($.trim(result) != "true") {
                    messager.showMessage(result, "fa-warning c_orange");
                } else {
                    $scope.commit('Save');
                    var url = Utils.ServiceURI.Address() + "api/ClosureTool/GenClosureTool/" + $scope.entity.Id;
                    $http.get(url).success(function (atts) {
                        messager.showMessage("[[[生成成功！]]]", "fa-warning c_orange");
                        switch (atts.Extension.toLowerCase()) {
                            case ".xlsx":
                            case ".xls":
                                atts.Icon = "fa fa-file-excel-o c_green";
                                break;
                            case ".ppt":
                                atts.Icon = "fa fa-file-powerpoint-o c_red";
                                break;
                            case ".doc":
                            case ".docx":
                                atts.Icon = "fa fa-file-word-o c_blue";
                                break;
                            default:
                                atts.Icon = "fa fa-file c_orange";
                                break;
                        }
                        $scope.ClosureTool = atts;
                        $scope.closureToolUrl = Utils.ServiceURI.Address() + "api/ClosureTool/DownLoadClosureTool/" + atts.ID;
                        if ($scope.ClosureTool != undefined)
                            $scope.hasClosureTool = true;
                        $scope.checkPointRefresh = true;
                    });
                }

            });

        }

        //获取项目基本数据
        $http.get(Utils.ServiceURI.Address() + "api/Closure/GetByProjectId/" + $scope.projectId).success(function (data) {

            if (data != "null") {

                $scope.ClosureInfo = data;


                $http.get(Utils.ServiceURI.ApiDelegate, {
                    cache: false,
                    params: {

                        url: Utils.ServiceURI.Address() + "api/StoreUsers/get/" + $scope.ClosureInfo.USCode + "/Finance_Manager"

                    }
                }).then(function (response) {

                    var userPositionData = response.data;

                    var financeSupervisorAccount = "";

                    for (var i = 0; i < userPositionData.length; i++) {
                        financeSupervisorAccount += userPositionData[i].Code;
                        if (i != userPositionData.length - 1) {
                            financeSupervisorAccount += ";";
                        }
                    }
                    $scope.entity.FinReportToAccount = financeSupervisorAccount;
                });
            }
        });




        $http.get(Utils.ServiceURI.Address() + "api/ClosureTool/GetClosureToolByProjectId/" + $scope.projectId).success(function (data) {
            if (data != "null") {
                if (!!data.CreateUserAccount) {
                    $scope.entity = data;
                } else {
                    $scope.entity.Id = data.Id;
                }
                $scope.entity.IsOptionOffered = data.IsOptionOffered;
                var url = Utils.ServiceURI.Address() + "api/projectusers/IsExists/" + $scope.projectId + "/" + window.currentUser.Code + "/Asset Actor";
                $http.get(url).success(function (d) {
                    $scope.isActor = d;
                });

            } else {
                $scope.entity.IsOptionOffered = false;
            }

            $scope.frmMain.frmFin.$removeControl($scope.frmMain.frmFin.receipt);
            // $scope.frmMain.frmFin.receipt.$setValidity("required", true);

            $scope.isLoadFinished = true;

        });

        $scope.ActorSubmit = function () {

            prePostData();
            $http.post(Utils.ServiceURI.Address() + "api/ClosureTool/PostActorClosureTool", $scope.entity).success(function (result) {

                messager.showMessage('[[[提交成功]]]', "fa-check c_orange").then(function () {
                    messager.unBlockUI();
                    //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                    redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                });

            }).error(function (data) {
                messager.showMessage("[[[提交失败！]]]", "fa-warning c_orange");
            });
        }

        function prePostData() {



            //checkDecisionLogic();

            $scope.entity.ProjectId = $scope.projectId;



        }


        $scope.ClosureToolSubmit = function () {
            commit("Submit");
        };

        function commit(type, callback) {

            prePostData();

            var impactStores = [];
            if (!!$scope.impactStore1 && !!$scope.impactStore1.StoreCode) {
                impactStores.push($scope.impactStore1);
            }
            if (!!$scope.impactStore2 && !!$scope.impactStore2.StoreCode) {
                impactStores.push($scope.impactStore2);
            }

            var url = Utils.ServiceURI.Address();

            if (type == "Save") {
                url = url + "api/ClosureTool/SaveClosureTool";
            }
            else if (type == "Submit") {
                url = url + "api/ClosureTool/PostClosureTool";
            }

            $http.post(url, {
                Entity: $scope.entity,
                ImpactStores: impactStores,
                yearMonth: $scope.yearmonth.value
            }).success(function (data) {
                messager.showMessage(Utils.Common.resolveAlertMsg(type), "fa-check c_green").then(function () {
                    if (type == "Submit") {
                        messager.unBlockUI();
                        //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                        redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                    } else {

                        $scope.entity.Id = data.replace("\"", "").replace("\"", "");
                        $scope.isLoaded = !$scope.isLoaded;
                        callback && callback();
                    }
                });
            }).error(function (data) {
                messager.showMessage(Utils.Common.resolveAlertMsg(type, true), "fa-check c_orange");
            });


        }

        $scope.SaveClosureTool = function () {

            commit("Save");
            //});
        };

        function checkDecisionLogic() {
            if (!$scope.entity.IsOptionOffered) {
                $scope.entity.LeaseTerm = null;
                $scope.entity.Investment = null;
                $scope.entity.NPVRestaurantCashflows = null;
                $scope.entity.Yr1SOI = null;
                $scope.entity.IRR = null;
                $scope.entity.CompAssumption = null;
                $scope.entity.CashflowGrowth = null;
            }
        }


        $scope.beginSelApprover = function (frm, closureInfo) {
            if (!frm.$valid)
                return;

            //if (($scope.entity.McppcoMargin && $scope.entity.McppcoMargin != 0) || ($scope.entity.MccpcoCashFlow && $scope.entity.MccpcoCashFlow != 0)) {
            //    var flag = false;
            //    if (!$scope.impactStore1 && !$scope.impactStore2) {
            //        flag = true;
            //    }
            //    else if ($scope.impactStore1 && (!$scope.impactStore1.StoreCode || $scope.impactStore1.StoreCode == "") && $scope.impactStore2 && (!$scope.impactStore2.StoreCode || $scope.impactStore2.StoreCode == "")) {
            //        flag = true;
            //    }
            //}
            //if (flag) {
            //    messager.showMessage("Impact On Other Stores 至少填写一家餐厅", "fa-warning c_orange");
            //    return;
            //}

            $modal.open({
                templateUrl: "/Template/ClosureToolSelApprover",
                backdrop: 'static',
                size: 'lg',
                controller: [
                    "$scope", "$modalInstance", "storeEntity", function ($scope, $modalInstance, storeEntity) {


                        $scope.FinSupervisorList = [];

                        $scope.entity = {};

                        $http.get(Utils.ServiceURI.ApiDelegate, {
                            cache: false,
                            params: {
                                url: Utils.ServiceURI.Address() + "api/StoreUsers/get/" + closureInfo.USCode + "/Finance_Manager"


                            }
                        }).then(function (response) {

                            var userPositionData = response.data;
                            for (var i = 0; i < userPositionData.length; i++) {
                                $scope.FinSupervisorList.push(userPositionData[i]);
                            }

                            if ($scope.FinSupervisorList.length == 1) {
                                $scope.entity.selFinSupervisor = $scope.FinSupervisorList[0];

                            }
                            $scope.dataLoaded = true;
                        });





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

                $scope.entity.FinReportToAccount = storeEntity.selFinSupervisor.Code;

                $scope.ClosureToolSubmit();


            });

        };
    }]);