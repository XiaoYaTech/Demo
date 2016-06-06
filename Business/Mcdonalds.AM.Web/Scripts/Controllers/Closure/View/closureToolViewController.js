dictionaryApp.controller('closureToolViewController', [
    '$scope',
    "$http",
    "$routeParams",
    "$window",
    'closureCreateHandler',
    "messager",
    "$modal",
    "$location",
    "redirectService",
    function ($scope, $http, $routeParams, $window, closureCreateHandler, messager, $modal, $location, redirectService) {

        var procInstID = $routeParams.ProcInstID;
        $scope.projectId = $routeParams.projectId;
        $scope.pageUrl = window.location.href;
        $scope.isHistory = $routeParams.isHistory;
        $scope.entityId = $routeParams.entityId;
        $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);
        $scope.isActor = false;
        $scope.isLoadFinished = true;
        $scope.impactStoreEditable = false;
        $scope.decisionEditable = false;
        $scope.hasClosureTool = false;
        $scope.flowCode = "Closure_ClosureTool";
        //if (!$scope.projectId) {

        //    $http.get(Utils.ServiceURI.Address() + "api/ClosureTool/GetByProcInstID/" + procInstID).success(function (data) {
        //        $scope.entity = data;
        //        $scope.projectId = $scope.entity.ProjectId;
        //        loadData();

        //    });
        //} else {
        loadData();
        //}


        $scope.entity = {};
        $scope.plEntity = {};
        $scope.plEntity.TotalSales = 100001.12;
        $scope.entity.TotalSales_Adjustment = $scope.plEntity.TotalSales;

        $scope.plEntity.CompSales = 201;
        $scope.entity.CompSales_Adjustment = $scope.plEntity.CompSales;

        $scope.plEntity.CompSalesMacket = 200;
        $scope.entity.CompSalesMacket_Adjustment = $scope.plEntity.CompSalesMacket;

        $scope.plEntity.CompCG = 200;
        $scope.entity.CompCG_Adjustment = $scope.plEntity.CompCG;

        $scope.plEntity.CompCGMacket = 200;
        $scope.entity.CompCGMacket_Adjustment = $scope.plEntity.CompCGMacket;

        $scope.plEntity.PAC_RMB = 200;
        $scope.entity.PAC_RMB_Adjustment = $scope.plEntity.PAC_RMB;

        $scope.plEntity.PAC = 200;
        $scope.entity.PAC_Adjustment = $scope.plEntity.PAC;

        $scope.plEntity.PACMarket = 200;
        $scope.entity.PACMarket_Adjustment = $scope.plEntity.PACMarket;

        $scope.plEntity.Rent_RMB = 200;
        $scope.entity.Rent_RMB_Adjustment = $scope.plEntity.Rent_RMB;

        $scope.plEntity.DepreciationLHI_RMB = 200;
        $scope.entity.DepreciationLHI_RMB_Adjustment = $scope.plEntity.DepreciationLHI_RMB;

        $scope.plEntity.ServiceFee_RMB = 200;
        $scope.entity.ServiceFee_RMB_Adjustment = $scope.plEntity.ServiceFee_RMB;

        $scope.plEntity.Accounting_RMB = 200;
        $scope.entity.Accounting_RMB_Adjustment = $scope.plEntity.Accounting_RMB;

        $scope.plEntity.Insurance_RMB = 200;
        $scope.entity.Insurance_RMB_Adjustment = $scope.plEntity.Insurance_RMB;

        $scope.plEntity.TaxesLicenses_RMB = 200;
        $scope.entity.TaxesLicenses_RMB_Adjustment = $scope.plEntity.TaxesLicenses_RMB;

        $scope.plEntity.Depreciation_ESSD_RMB = 200;
        $scope.entity.Depreciation_ESSD_RMB_Adjustment = $scope.plEntity.Depreciation_ESSD_RMB;

        $scope.plEntity.Interest_ESSD_RMB = 200;
        $scope.entity.Interest_ESSD_RMB_Adjustment = $scope.plEntity.Interest_ESSD_RMB;

        $scope.plEntity.OtherIncExp_RMB = 200;
        $scope.entity.OtherIncExp_RMB_Adjustment = $scope.plEntity.OtherIncExp_RMB;

        $scope.plEntity.NonProduct_Sales_RMB = 200;
        $scope.entity.NonProduct_Sales_RMB_Adjustment = $scope.plEntity.NonProduct_Sales_RMB;

        $scope.plEntity.NonProduct_Costs_RMB = 200;
        $scope.entity.NonProduct_Costs_RMB_Adjustment = $scope.plEntity.NonProduct_Costs_RMB;

        $scope.plEntity.SOI = 200;
        $scope.entity.SOI_Adjustment = $scope.plEntity.SOI;

        $scope.plEntity.SOIMarket = 200;
        $scope.entity.SOIMarket_Adjustment = $scope.plEntity.SOIMarket;

        $scope.plEntity.CashFlow_RMB = 200;
        $scope.entity.CashFlow_RMB_Adjustment = $scope.plEntity.CashFlow_RMB;
        $scope.entity.ProjectId = $scope.projectId;










        function loadData() {
            $scope.checkPointRefresh = true;
            $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetByProjectId/" + $scope.projectId).success(function (data) {
                $scope.woCheckList = data;

            });

            var url = Utils.ServiceURI.Address() + "api/projectusers/IsExists/" + $scope.projectId + "/" + window.currentUser.Code + "/Asset Actor";
            $http.get(url).success(function (data) {
                $scope.isActor = data;
            });

            //获取项目基本数据
            $http.get(Utils.ServiceURI.Address() + "api/Closure/GetByProjectId/" + $scope.projectId).success(function (data) {

                if (data != "null") {

                    $scope.ClosureInfo = data;
                }
            });
            var url;
            if ($scope.isHistory && $scope.entityId) {
                url = Utils.ServiceURI.Address() + "api/ClosureTool/GetById/" + $scope.entityId;
            } else {
                url = Utils.ServiceURI.Address() + "api/ClosureTool/GetClosureToolByProjectId/" + $scope.projectId;
            }

            $http.get(url).success(function (data) {
                if (data != "null") {
                    $scope.entity = data;
                    $scope.yearmonth = "";

                    $scope.enableReCall = false;
                    $scope.enableEdit = false;

                    //判断流程是否进入K2
                    if (!!$scope.entity.ProcInstID && $scope.entity.ProcInstID > 0) {

                        //var roleCode = "Finance Team";
                        var roleCode = "Finance Consultant";

                        //判断当前用户是否是Finance
                        $http.get(Utils.ServiceURI.Address() + "api/ProjectUsers/IsExists/" + $scope.projectId + "/" + window.currentUser.Code + "/" + roleCode).success(function (isEditor) {

                            if (isEditor == "true") {
                                var flowCode = "Closure_ClosureTool";

                                $http.get(Utils.ServiceURI.Address() + "api/project/isFinished/" + $scope.entity.ProjectId + "/" + flowCode).success(function (isFinished) {

                                    if (isFinished == "true") {
                                        $scope.enableEdit = true;
                                    } else {

                                        $http.get(Utils.ServiceURI.Address() + "api/project/EnableReCall/ClosureTool/" + $scope.entity.Id + "/" + $scope.entity.ProjectId).success(function (isStart) {
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
        $scope.beforeDownloadClosureTool = function () {
            if (!!$scope.closureToolUrl) {
                var u = Utils.ServiceURI.Address() + "api/ClosureTool/EnableGenClosureTool/" + $scope.entity.Id;
                $http.get(u).success(function (result) {
                    if ($.trim(result) != "true") {
                        messager.showMessage(result, "fa-warning c_orange");
                    } else {
                        var url = Utils.ServiceURI.Address() + "api/ClosureTool/GenClosureTool/" + $scope.entity.Id;
                        $http.get(url).success(function (atts) {
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
                            $scope.checkPointRefresh = true;
                            if ($scope.ClosureTool != undefined)
                                $scope.hasClosureTool = true;
                        });
                    }

                });
                return true;
            } else {
                $scope.ClosureTool.FileURL = "";
                return false;
            }
        }
        $scope.genClosureTool = function () {


            var u = Utils.ServiceURI.Address() + "api/ClosureTool/EnableGenClosureTool/" + $scope.entity.Id.toString();
            $http.get(u).success(function (result) {

                if ($.trim(result) != "true") {
                    messager.showMessage(result, "fa-warning c_orange");
                } else {

                    var url = Utils.ServiceURI.Address() + "api/ClosureTool/GenClosureTool/" + $scope.entity.Id;
                    $http.get(url).success(function (atts) {
                        messager.showMessage("[[[生成成功！]]]", "fa-check c_green");
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
                        $scope.checkPointRefresh = true;
                        if ($scope.ClosureTool != undefined)
                            $scope.hasClosureTool = true;
                        $http.get(Utils.ServiceURI.Address() + "api/ClosureTool/CallClosureTool/" + $scope.entity.Id).success(function (result) {
                            $scope.entity.TotalOneOffCosts = result.TotalOneOffCosts;
                            $scope.entity.OperatingIncome = result.OperatingIncome;
                            $scope.entity.CompensationReceipt = result.CompensationReceipt;
                            $scope.entity.ClosingCosts = result.ClosingCosts;
                            $scope.entity.NPVSC = result.NPVSC;
                        });
                    });
                }

            });

        }

        $scope.editClosureTool = function () {
            messager.confirm("[[[Closure Tool 审批流程已经完成，现在编辑会导致流程重新提交，是否确认？]]]", "fa-warning c_orange").then(function (result) {

                if (result) {
                    $scope.entity.UserAccount = window.currentUser.Code;
                    $http.post(Utils.ServiceURI.Address() + "api/closureTool/Edit", $scope.entity).success(function (data) {
                        messager.showMessage("[[[操作成功]]]", "fa-check c_green").then(function () {
                            //var url = "/Closure/ClosureTool/" + $scope.projectId;
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

        $scope.recallClosureTool = function (comment) {
            //messager.confirm("您确定执行ReCall操作吗？", "fa-warning c_orange").then(function (result) {
            //    if (result) {

            $scope.entity.UserAccount = window.currentUser.Code;
            $http.post(Utils.ServiceURI.Address() + "api/closureTool/Recall", $scope.entity).success(function (data) {
                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                    //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                    messager.unBlockUI();
                    redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                });
            }).error(function (data) {
                messager.showMessage("[[[撤回失败]]]", "fa-warning c_orange");
            });
        }
        //});
        //}

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

                $scope.recallClosureTool(storeEntity.Comment);


            });

        };
    }]);