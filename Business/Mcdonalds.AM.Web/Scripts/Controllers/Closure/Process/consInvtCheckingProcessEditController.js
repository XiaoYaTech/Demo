dictionaryApp.controller('consInvtCheckingProcessEditController', [
    '$scope',
    "$http",
    "$routeParams",
    "$modal",
    "$window",
    'closureCreateHandler',
    "messager",
    "redirectService",
    function ($scope, $http, $routeParams, $modal, $window, closureCreateHandler, messager, redirectService) {

        var procInstID = $routeParams.ProcInstID;
        var sn = $routeParams.SN;
        $scope.projectId = $routeParams.projectId;
        $scope.entity = {};
        $scope.entity.ProjectId = $scope.projectId;
        $scope.checkPointRefresh = true;
        $scope.flowCode = "Closure_ConsInvtChecking";

        var userAccount = window.currentUser.Code;

        $scope.ApproverSubmit = function (action) {

            $scope.entity.SN = sn;
            $scope.entity.Action = action;
            $scope.entity.ProcInstID = procInstID;

            $scope.entity.UserAccount = userAccount;
            $scope.entity.UserNameENUS = window.currentUser.NameENUS;
            $scope.entity.UserNameZHCN = window.currentUser.NameZHCN;

            $http.post(Utils.ServiceURI.Address() + "api/ClosureConsInvtChecking/ProcessClosureConsInvtChecking/", $scope.entity).success(function (successData) {
                messager.showMessage(Utils.Common.resolveAlertMsg(action), "fa-check c_green").then(function () {
                    //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                    messager.unBlockUI();
                    redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                });
            }).error(function (err) {
                messager.showMessage(Utils.Common.resolveAlertMsg(action, true), "fa-warning c_orange");
            });

        }




        function loadConsInvtData() {


            $http.get(Utils.ServiceURI.Address() + "api/ClosureConsInvtChecking/GetByProjectId/" + $scope.projectId).success(function (data) {

                $scope.entity = data;


            }).then(function (data) {
                if ($scope.entity != "null") {
                    $http.get(Utils.ServiceURI.Address() + "api/ClosureConsInvtChecking/GetTemplates/" + $scope.entity.Id.toString()).success(function (atts) {
                        $scope.templateList = atts;
                        for (var i = 0; i < atts.length; i++) {
                            atts[i].IsAvailable = (i == atts.length - 1) ? "[[[生效]]]" : "[[[失效]]]";
                        }

                    });
                };
            });
        }

        $scope.isUploadTemplate = null;

        $scope.$watch("isUploadTemplate", function (val) {
            if (!!val) {
                loadConsInvtData();
                $scope.isUploadTemplate = false;
            }

        });
        loadConsInvtData();

        $scope.save = function (action) {
            $scope.entity.Action = action;
            $http.post(Utils.ServiceURI.Address() + "api/ClosureConsInvtChecking/Save", $scope.entity).success(function (data) {
                messager.showMessage("[[[保存成功]]]", "fa-check c_green").then(function () {
                    // $window.location = Utils.ServiceURI.WebAddress() + "redirect";
                });
            }).error(function (data) {
                messager.showMessage("[[[保存失败]]]", "fa-warning c_red");
            });
        }

        $scope.beginSelApprover = function (conEntity) {

            var errors = [];
            if ($scope.recostVar < -0.05 || $scope.recostVar > 0.05 || $scope.entity.RECostBudget == 0) {
                if (!conEntity.REExplanation || $.trim(conEntity.REExplanation) == "") {
                    errors.push("[[[请填写RE Cost Variance原因]]]");

                }
            }

            if ($scope.recostVar2 < -0.05 || $scope.recostVar2 > 0.05 || $scope.entity.LHIBudget == 0) {
                if (!conEntity.LHIExplanation || $.trim(conEntity.LHIExplanation) == "") {
                    errors.push("[[[请填写LHI Variance原因]]]");
                }
            }

            if ($scope.recostVar3 < -0.05 || $scope.recostVar3 > 0.05 || $scope.entity.ESSDBudget == 0) {
                if (!conEntity.ESSDExplanation || $.trim(conEntity.ESSDExplanation) == "") {
                    errors.push("[[[请填写ESSD Variance原因]]]");
                }
            }

            if ($scope.recostVar4 < -0.05 || $scope.recostVar4 > 0.05 || $scope.entity.TotalWriteoffBudget == 0) {
                if (!conEntity.TotalExplanation || $.trim(conEntity.TotalExplanation) == "") {
                    errors.push("[[[请填写Total Variance原因]]]");
                }
            }

            /*
      Total Variance <= +-5%   :  1
         +-5%  < Total Variance <= +- 10%   :   2
         Total Variance > +- 10% :  3
      */
            if (errors.length > 0) {
                messager.showMessage(errors.join("<br />"), "fa-warning c_red");
                return false;
            }
            var diff = 1;
            if ($scope.recostVar4 < -0.1 || $scope.recostVar4 > 0.1)
                diff = 3;
            else if ($scope.recostVar4 < -0.05 || $scope.recostVar4 > 0.05)
                diff = 2;

            $modal.open({
                templateUrl: "/Template/ConsInvtCheckingSelApprover",
                backdrop: 'static',
                size: 'lg',
                scope: $scope,
                controller: [
                    "$scope", "$modalInstance", "storeEntity", function ($scope, $modalInstance, storeEntity) {

                        $scope.showPM = false;
                        $scope.showFinaceManger = false;
                        $scope.showFinController = false;
                        $scope.showVPGM = false;
                        $scope.entity = {};
                        $scope.frmInvalid = true;
                        $scope.diff = diff;

                        switch (diff) {
                            case 1:
                                $scope.showPM = true;
                                $scope.showFinaceManger = true;

                                break;
                            case 2:
                                $scope.showPM = true;
                                $scope.showFinaceManger = true;
                                $scope.showFinController = true;
                                break;
                            case 3:
                                $scope.showPM = true;
                                $scope.showFinaceManger = true;
                                $scope.showFinController = true;
                                $scope.showVPGM = true;
                                break;
                        }


                        $http.get(Utils.ServiceURI.Address() + "api/Closure/GetByProjectId/" + conEntity.ProjectId).success(function (data) {
                            if (data != "null") {
                                var closureInfo = data;
                                $http.get(Utils.ServiceURI.ApiDelegate, {
                                    cache: false,
                                    params: {
                                        url: Utils.ServiceURI.Address() + "api/StoreUsers/get/" + conEntity.USCode + "/Cons_Mgr"

                                    }
                                }).then(function (response) {

                                    var userPositionData = response.data;
                                    $scope.PMList = userPositionData;
                                    if ($scope.PMList.length == 1) {
                                        $scope.entity.selPM = $scope.PMList[0];
                                    }
                                    else if ($scope.entity.PMSupervisor) {
                                        angular.forEach($scope.PMList, function (r, i) {
                                            if ($scope.entity.PMSupervisor == r.Code) {
                                                $scope.entity.selPM = r;
                                            }
                                        });
                                    }

                                    //$scope.FinManagerList = [{ "Code": closureInfo.FinanceAccount, "NameENUS": closureInfo.FinanceNameENUS }];
                                    //if ($scope.FinManagerList.length == 1) {
                                    //    $scope.entity.selFinManager = $scope.FinManagerList[0];
                                    //}

                                    if ($scope.showVPGM || $scope.showFinController) {

                                        $scope.VPGMList = [];
                                        $scope.FinControllerList = [];
                                        $scope.FinManagerList = [];
                                        var url = Utils.ServiceURI.Address() + "api/StoreUsers/GetStoreEmployeesByMultiRoles/" + closureInfo.USCode + "?roleCodes=Finance_Controller,VPGM,Finance_Manager";
                                        $http.get(url, {
                                            cache: false
                                        }).then(function (result) {
                                            for (var i = 0; i < result.data.length; i++) {
                                                positionData = result.data[i];
                                                switch (positionData.PositionENUS) {
                                                    case "Finance Controller":
                                                        $scope.FinControllerList.push(positionData);
                                                        break;
                                                    case "VPGM":
                                                        $scope.VPGMList.push(positionData);
                                                        break;
                                                    case "Finance Manager":
                                                        $scope.FinManagerList.push(positionData);
                                                        break;
                                                }
                                            }
                                            if ($scope.showVPGM) {
                                                if ($scope.VPGMList.length == 1) {
                                                    $scope.entity.selVPGM = $scope.VPGMList[0];
                                                }
                                            }
                                            if ($scope.showFinController) {
                                                if ($scope.FinControllerList.length == 1) {
                                                    $scope.entity.selFinController = $scope.FinControllerList[0];
                                                }
                                            }
                                            if ($scope.showFinaceManger) {
                                                if ($scope.FinManagerList.length == 1) {
                                                    $scope.entity.selFinManager = $scope.FinManagerList[0];
                                                }
                                            }
                                            $scope.formValid();
                                        });
                                    }

                                    $scope.formValid();

                                });

                            }
                        });

                        //表单验证
                        $scope.formValid = function () {

                            if ($scope.showPM) {
                                if (!$scope.entity.selPM) {
                                    $scope.frmInvalid = true;
                                    return false;
                                }
                            }
                            if ($scope.showFinaceManger) {
                                if (!$scope.entity.selFinManager) {
                                    $scope.frmInvalid = true;
                                    return false;
                                }
                            }

                            if ($scope.showFinController) {
                                if (!$scope.entity.selFinController) {
                                    $scope.frmInvalid = true;
                                    return false;
                                }
                            }

                            if ($scope.showVPGM) {
                                if (!$scope.entity.selVPGM) {
                                    $scope.frmInvalid = true;
                                    return false;
                                }
                            }

                            $scope.frmInvalid = false;
                        }

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

                $scope.entity.UserAccount = window.currentUser.Code;
                $scope.entity.UserNameENUS = window.currentUser.NameENUS;
                $scope.entity.UserNameZHCN = window.currentUser.NameZHCN;

                $scope.entity.PMSupervisor = storeEntity.selPM.Code;
                $scope.entity.FinanceAccount = storeEntity.selFinManager.Code;
                if (storeEntity.selVPGM) {
                    $scope.entity.VPGMAccount = storeEntity.selVPGM.Code;
                }
                if (storeEntity.selFinController) {
                    $scope.entity.FinControllerAccount = storeEntity.selFinController.Code;
                }

                $scope.ApproverSubmit('ReSubmit');


            });


        }


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

