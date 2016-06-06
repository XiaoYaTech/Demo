dictionaryApp.controller('woCheckListProcessEditController', [
    '$scope',
    "$http",
    "$modal",
    "$window",
    '$routeParams',
    'closureCreateHandler',
    "messager",
    "redirectService",
    function ($scope, $http, $modal, $window, $routeParams, closureCreateHandler, messager, redirectService) {

        var procInstID = $routeParams.ProcInstID;
        $scope.projectId = $routeParams.projectId;
        var sn = $routeParams.SN;
        $scope.flowCode = "Closure_WOCheckList";
        $scope.checkPointRefresh = false;
        $scope.entity = {};
        $scope.userAccount = window.currentUser.Code;
        $scope.userNameZHCN = escape(window.currentUser.NameZHCN);
        $scope.userNameENUS = escape(window.currentUser.NameENUS);
        if (!$scope.projectId) {

            $http.get(Utils.ServiceURI.Address() + "api/project/GetProjectIDByProcInstID/" + procInstID).success(function (data) {
                if (data != "null") {
                    $scope.projectId = data.replace("\"", "").replace("\"", "");
                }
                loadData();
            });
        } else {
            loadData();
        }

        $scope.uploadFinFinish = function (up, files) {
            $scope.checkPointRefresh = true;
        }

        $scope.deleteAttachmentFinish = function (id, requirementId) {
            $scope.checkPointRefresh = true;
        }

        function loadData() {
            $scope.checkPointRefresh = true;

            $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetByProjectId/" + $scope.projectId).success(function (data) {
                $scope.entity = data;

            }).then(function (data) {
                if ($scope.entity != "null") {
                    //获取项目基本数据
                    $http.get(Utils.ServiceURI.Address() + "api/Closure/GetByProjectId/" + $scope.projectId).success(function (data) {
                        if (data != "null") {
                            $scope.ClosureInfo = data;

                        }
                    });

                    $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetTemplates/" + $scope.entity.Id).success(function (atts) {
                        $scope.templateList = atts;
                        for (var i = 0; i < atts.length; i++) {
                            atts[i].IsAvailable = (i == atts.length - 1) ? "[[[生效]]]" : "[[[失效]]]";
                        }

                        $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetHistoryList/" + $scope.projectId).success(function (historyData) {
                            $scope.historyList = historyData;
                        }).error(function (err) {
                            var s = err;
                        });

                        $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetClosureCommers/ClosureWOCheckList/" + $scope.entity.Id).success(function (closureCommers) {
                            $scope.closureCommers = closureCommers;

                        });
                    });
                };
            });
        }


        $scope.ApproverSubmit = function (action) {

            $scope.entity.UserAccount = window.currentUser.Code;
            $scope.entity.UserNameZHCN = window.currentUser.NameZHCN;
            $scope.entity.UserNameENUS = window.currentUser.NameENUS;


            $scope.entity.SN = sn;
            $scope.entity.Action = action;
            $http.post(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/ProcessClosureWOCheckList/", $scope.entity).success(function (data) {
                messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                    //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                    messager.unBlockUI();
                    redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                });
            }).error(function (data) {
                messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
            });

        };

        $scope.PMSave = function (action) {
            $scope.entity.Action = action;
            $http.post(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/SaveClosureWOCheckList", $scope.entity).success(function (data) {
                messager.showMessage("[[[保存成功]]]", "fa-check c_green").then(function () {
                    // $window.location = Utils.ServiceURI.WebAddress() + "redirect";
                });
            }).error(function (data) {
                messager.showMessage("[[[保存失败]]]", "fa-warning c_red");
            });
        }

        $scope.beginSelApprover = function (closureInfo, woCheckList) {
            if ($.grep($scope.WOAttachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "5ef6f0f9-0177-4f1e-bf84-f081462dc6d7";
            }) == 0) {
                messager.showMessage("[[[请先上传Closing Cost！]]]", "fa-warning c_red");
                return false;
            }
            $modal.open({
                templateUrl: "/Template/ClosureWOCheckListSelApprover",
                backdrop: 'static',
                size: 'lg',
                controller: [
                    "$scope", "$modalInstance", "storeEntity", function ($scope, $modalInstance, storeEntity) {

                        $scope.ConstructionMgrList = [];
                        $scope.MCCLConsManagerList = [];
                        $scope.MCCLITList = [];
                        $scope.MCCLEquipmentList = [];
                        $scope.entity = woCheckList;
                        var ready = 0;

                        $http.get(Utils.ServiceURI.ApiDelegate, {
                            cache: false,
                            params: {
                                url: Utils.ServiceURI.Address() + "api/StoreUsers/GetStoreEmployeesByMultiRoles/" + closureInfo.USCode,
                                roleCodes: "Cons_Mgr,MCCL_Cons_Manager,Dev_Equipment,IT_Equipment"
                            }
                        }).then(function (response) {
                            var userPositionData = response.data;
                            var positionData;
                            for (var i = 0; i < userPositionData.length; i++) {
                                positionData = userPositionData[i];
                                switch (positionData.PositionENUS) {
                                    case "IT Equipment":
                                        $scope.MCCLITList.push(positionData);
                                        break;
                                    case "MCCL Cons Manager":
                                        $scope.MCCLConsManagerList.push(positionData);
                                        break;
                                    case "Dev Equipment":
                                        $scope.MCCLEquipmentList.push(positionData);
                                        break;
                                }
                            }

                            //Construction Manager
                            if ($scope.ConstructionMgrList.length == 1) {
                                $scope.entity.selConstructionMgr = $scope.ConstructionMgrList[0];
                            }
                            else if ($scope.entity.PMSupervisorAccount) {
                                angular.forEach($scope.ConstructionMgrList, function (r, i) {
                                    if ($scope.entity.PMSupervisorAccount == r.Code) {
                                        $scope.entity.selConstructionMgr = r;
                                    }
                                });
                            }

                            //MCCL Cons Manager
                            if ($scope.MCCLConsManagerList.length == 1) {
                                $scope.entity.selMCCLConsManager = $scope.MCCLConsManagerList[0];
                            }
                            else if ($scope.entity.MCCLApproverAccount) {
                                angular.forEach($scope.MCCLConsManagerList, function (r, i) {
                                    if ($scope.entity.MCCLApproverAccount == r.Code) {
                                        $scope.entity.selMCCLConsManager = r;
                                    }
                                });
                            }

                            //IT Equipment
                            if ($scope.MCCLITList.length == 1) {
                                $scope.entity.selMCCLIT = $scope.MCCLITList[0];
                            }
                            else if ($scope.entity.MCCLITApproverAccount) {
                                angular.forEach($scope.MCCLITList, function (r, i) {
                                    if ($scope.entity.MCCLITApproverAccount == r.Code) {
                                        $scope.entity.selMCCLIT = r;
                                    }
                                });
                            }

                            //Construction Equipment
                            if ($scope.MCCLEquipmentList.length == 1) {
                                $scope.entity.selMCCLEquipment = $scope.MCCLEquipmentList[0];
                            }
                            else if ($scope.entity.MCCLMCCLEqApproverAccount) {
                                angular.forEach($scope.MCCLEquipmentList, function (r, i) {
                                    if ($scope.entity.MCCLMCCLEqApproverAccount == r.Code) {
                                        $scope.entity.selMCCLEquipment = r;
                                    }
                                });
                            }
                            ready++;
                            if (ready == 2)
                                $scope.dataLoaded = true;
                        });

                        $http.get(Utils.ServiceURI.ApiDelegate, {
                            cache: false,
                            params: {
                                url: Utils.ServiceURI.Address() + "api/StoreUsers/get/" + closureInfo.USCode + "/Cons_Mgr"

                            }
                        }).then(function (response) {
                            var userPositionData = response.data;
                            for (var i = 0; i < userPositionData.length; i++) {
                                $scope.ConstructionMgrList.push(userPositionData[i]);
                            }
                            if ($scope.ConstructionMgrList.length == 1) {
                                $scope.entity.selConstructionMgr = $scope.ConstructionMgrList[0];
                            }
                            ready++;
                            if (ready == 2)
                                $scope.dataLoaded = true;
                        });

                        $scope.ok = function (e) {

                            $modalInstance.close($scope.entity);
                        };
                        $scope.cancel = function () {
                            $scope.entity.selConstructionMgr = null;
                            $scope.entity.selMCCLConsManager = null;
                            $scope.entity.selMCCLIT = null;
                            $scope.entity.selMCCLEquipment = null;
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

                $scope.entity.PMSupervisorAccount = storeEntity.selConstructionMgr.Code;
                $scope.entity.MCCLApproverAccount = storeEntity.selMCCLConsManager.Code;
                $scope.entity.MCCLITApproverAccount = storeEntity.selMCCLIT.Code;
                $scope.entity.MCCLMCCLEqApproverAccount = storeEntity.selMCCLEquipment.Code;
                /*
                $http.post(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/PostClosureWOCheckList", $scope.entity).success(function (data) {
                    messager.showMessage("[[[操作成功]]]", "fa-check c_green").then(function () {
                        $window.location = Utils.ServiceURI.WebAddress() + "redirect";
                    });
                }).error(function (data) {
                    messager.showMessage("[[[提交失败]]]", "fa-warning c_red");
                });
                */
                $scope.ApproverSubmit("ReSubmit");
            });

        };
    }]);