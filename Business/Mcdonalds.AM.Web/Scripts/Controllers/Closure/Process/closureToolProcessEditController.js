dictionaryApp.controller('closureToolProcessEditController', [
    '$scope',
    "$http",
    "$routeParams",
    "$window",
    'closureCreateHandler',
    "messager",
    "$modal",
    "redirectService",
    function ($scope, $http, $routeParams, $window, closureCreateHandler, messager, $modal, redirectService) {

        var procInstID = $routeParams.ProcInstID;
        var sn = $routeParams.SN;
        $scope.projectId = $routeParams.projectId;
        $scope.pageUrl = window.location.href;
        $scope.entity = {};
        $scope.flowCode = "Closure_ClosureTool";
        //是否是处理人
        $scope.entity.isEditor = false;
        $scope.impactStoreEditable = false;
        $scope.decisionEditable = false;
        $scope.entity.ProjectId = $scope.projectId;
        $scope.checkPointRefresh = true;

        loadData();

        function loadData() {

            $http.get(Utils.ServiceURI.Address() + "api/ClosureTool/GetClosureToolByProjectId/" + $scope.projectId).success(function (data) {

                $scope.entity = data;

                $scope.isLoadFinished = true;
            });

            $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetByProjectId/" + $scope.projectId).success(function (data) {
                $scope.woCheckList = data;

            });

            //获取项目基本数据
            $http.get(Utils.ServiceURI.Address() + "api/Closure/GetByProjectId/" + $scope.projectId).success(function (data) {
                if (data != "null") {
                    $scope.ClosureInfo = data;
                }
            });

        }

        $scope.ApproverSubmit = function (action) {

            $scope.submiting = true;

            $scope.entity.SN = sn;
            $scope.entity.ProcInstID = procInstID;
            var userAccount = window.currentUser.Code;
            $scope.entity.UserAccount = userAccount;
            $scope.entity.Username = window.currentUser.NameZHCN + "(" + window.currentUser.NameENUS + ")";

            var impactStores = [];
            if (!!$scope.selStore1) {
                impactStores.push($scope.selStore1);
            }
            if (!!$scope.selStore2) {
                impactStores.push($scope.selStore2);
            }

            //checkDecisionLogic();


            var url = Utils.ServiceURI.Address();

            if (action == "Save") {
                url = url + "api/ClosureTool/SaveClosureTool";
                $scope.entity.Action = "ReSubmit";
            }
            else {
                url = url + "api/ClosureTool/ProcessClosureTool";
                $scope.entity.Action = "ReSubmit";
            }

            $http.post(url, {
                Entity: $scope.entity,
                ImpactStores: impactStores,
                yearMonth: $scope.yearmonth.value
            }).success(function (data) {
                messager.showMessage("[[[操作成功]]]", "fa-check c_green").then(function () {
                    $scope.submiting = false;
                    if (action != "Save") {
                        //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                        messager.unBlockUI();
                        redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                    }
                });
            }).error(function (data) {
                $scope.submiting = false;
                messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
            });
        };
        $scope.Save = function (callback) {
            $scope.entity.SN = sn;
            $scope.entity.ProcInstID = procInstID;
            var userAccount = window.currentUser.Code;
            $scope.entity.UserAccount = userAccount;
            $scope.entity.Username = window.currentUser.NameZHCN + "(" + window.currentUser.NameENUS + ")";

            var impactStores = [];
            if (!!$scope.selStore1) {
                impactStores.push($scope.selStore1);
            }
            if (!!$scope.selStore2) {
                impactStores.push($scope.selStore2);
            }

            //checkDecisionLogic();


            var url = Utils.ServiceURI.Address();
            url = url + "api/ClosureTool/SaveClosureTool";
            $scope.entity.Action = "Resubmit";

            $http.post(url, {
                Entity: $scope.entity,
                ImpactStores: impactStores
            }).success(function (data) {
                callback && callback();
            }).error(function (data) {
                messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
            });
        }

        $scope.beginSelApprover = function (frm, closureInfo) {
            if (!frm.$valid)
                return;

            //if (($scope.entity.McppcoMargin && $scope.entity.McppcoMargin != 0) || ($scope.entity.MccpcoCashFlow && $scope.entity.MccpcoCashFlow != 0)) {
            //    var flag = false;
            //    if (!$scope.selStore1 && !$scope.selStore2) {
            //        flag = true;
            //    }
            //    else if ($scope.selStore1 && (!$scope.selStore1.StoreCode || $scope.selStore1.StoreCode == "") && $scope.selStore2 && (!$scope.selStore2.StoreCode || $scope.selStore2.StoreCode == "")) {
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


                        $http.get(Utils.ServiceURI.ApiDelegate, {
                            cache: false,
                            params: {
                                url: Utils.ServiceURI.Address() + "api/StoreUsers/get/" + closureInfo.USCode + "/Finance_Manager"

                            }
                        }).then(function (response) {
                            $scope.entity = {};

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
                            $scope.submiting = true;
                            $modalInstance.close($scope.entity);
                        };
                        $scope.cancel = function () {
                            $scope.submiting = false;
                            $modalInstance.dismiss("cancel");
                        };
                        $scope.submiting = false;
                    }
                ],
                resolve: {
                    storeEntity: function () {
                        return angular.copy(closureInfo);
                    }
                }

            }).result.then(function (storeEntity) {

                $scope.entity.FinReportToAccount = storeEntity.selFinSupervisor.Code;

                $scope.ApproverSubmit('ReSubmit');

            });

        };
        $scope.save = function () {

            $scope.ApproverSubmit("Save");
        }



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

    }]);