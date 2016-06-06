
dictionaryApp.controller('woCheckListController', [
    '$scope',
    "$http",
    "$modal",
    "$window",
    '$routeParams',
    'closureCreateHandler',
    '$selectUser',
    "messager",
    "redirectService",
    function ($scope, $http, $modal, $window, $routeParams, closureCreateHandler, $selectUser, messager, redirectService) {



        //检查点用到的三个参数
        $scope.projectId = $routeParams.projectId;

        $scope.flowCode = "Closure_WOCheckList";

        $scope.checkPointRefresh = true;

        $scope.userAccount = window.currentUser.Code;
        $scope.userNameZHCN = escape(window.currentUser.NameZHCN);
        $scope.userNameENUS = escape(window.currentUser.NameENUS);

        $scope.isUploadClosingCost = false;

        $scope.dataRefresh = false;

        $scope.entity = {};
        $scope.entity.Status = 0;

        $scope.isUploadTemplate = false;
        loadData();

        $scope.$watch("dataRefresh", function (val) {

            if (!!val && val == true) {

                $scope.checkPointRefresh = true;
                $scope.dataRefresh = false;
            }
        });

        $scope.uploadFinFinish = function (up, files) {
            $scope.checkPointRefresh = true;
        }

        $scope.deleteAttachmentFinish = function (id, requirementId) {
            $scope.checkPointRefresh = true;
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

                        $http.get(Utils.ServiceURI.ApiDelegate, {
                            cache: false,
                            params: {
                                url: Utils.ServiceURI.Address() + "api/StoreUsers/GetStoreEmployeesByMultiRoles/" + closureInfo.USCode,
                                roleCodes: "Cons_Mgr,MCCL_Cons_Manager,IT_Equipment,Dev_Equipment"
                            }
                        }).then(function (response) {
                            var userPositionData = response.data;
                            var positionData;
                            for (var i = 0; i < userPositionData.length; i++) {
                                positionData = userPositionData[i];
                                switch (positionData.PositionENUS) {
                                    case "Cons Mgr":
                                        $scope.ConstructionMgrList.push(positionData);
                                        break;
                                    case "MCCL Cons Manager":
                                        $scope.MCCLConsManagerList.push(positionData);
                                        break;
                                    case "IT Equipment":
                                        $scope.MCCLITList.push(positionData);
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

                            $scope.dataLoaded = true;
                        });

                        $scope.ok = function (frm, e) {
                            if (!frm.$valid)
                                return;
                            $scope.submiting = true;
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

                $http.post(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/PostClosureWOCheckList", $scope.entity).success(function (data) {
                    messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                        //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                        messager.unBlockUI();
                        redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                    });
                }).error(function (data) {
                    storeEntity.submiting = false;
                    messager.showMessage("[[[提交失败]]]", "fa-warning c_red");
                });
            });
        };

        $scope.uploadCCFinished = function (u, a) {
            $scope.entity.Status = true;
            $scope.checkPointRefresh = true;
        };

        $scope.PMSave = function () {

            $scope.entity.UserAccount = window.currentUser.Code;
            $scope.entity.UserNameZHCN = window.currentUser.NameZHCN;
            $scope.entity.UserNameENUS = window.currentUser.NameENUS;
            $http.post(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/SaveClosureWOCheckList", $scope.entity).success(function (data) {
                messager.showMessage("[[[保存成功]]]", "fa-check c_green").then(function () {
                    // $window.location = Utils.ServiceURI.WebAddress() + "redirect";
                });
            }).error(function (data) {
                messager.showMessage("[[[保存失败]]]", "fa-warning c_red");
            });
        }

        //$scope.PMSubmit = function () {
        //    if ($scope.ClosingCosts == undefined || $scope.ClosingCosts == "null") {
        //        messager.showMessage("请先上传Closing Cost！", "fa-warning c_red");
        //        return false;
        //    }

        //    $scope.entity.UserAccount = window.currentUser.Code;
        //    $scope.entity.UserNameZHCN = window.currentUser.NameZHCN;
        //    $scope.entity.UserNameENUS = window.currentUser.NameENUS;
        //    $scope.entity.PMSupervisorAccount = $scope.PMSupervisorAccount;
        //    $scope.entity.MCCLApproverAccount = $scope.MCCLApproverAccount;
        //    $http.post(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/PostClosureWOCheckList", $scope.entity).success(function (data) {
        //        messager.showMessage("[[[操作成功]]]", "fa-check c_green").then(function () {
        //            $window.location = Utils.ServiceURI.WebAddress() + "redirect";
        //        });
        //    }).error(function (data) {
        //        messager.showMessage("[[[提交失败]]]", "fa-warning c_red");
        //    });

        //};


        //$scope.selectEmployee = function (users, role) {

        //    var _this = this;
        //    var roleStr = "";
        //    if (role == "Asset Actor") {
        //        roleStr = "Asset Rep";
        //    }
        //    $selectUser.open({
        //        //选用户设置的参数，目前无效，别删！
        //        //storeCode: $scope.entity.USCode,
        //        //positionCode: roleStr,



        //        checkUsers: function (selectedUsers) {

        //            if ($.trim(selectedUsers[0].Code) == "") {
        //                messager.showMessage("编号不能为空！", "fa-warning c_red");
        //            }
        //            return true;
        //        },
        //        selectedUsers: angular.copy(users),
        //        OnUserSelected: function (selectedUsers) {
        //            var userAccounts = "";
        //            var userNames = "";
        //            for (var i = 0; i < selectedUsers.length; i++) {
        //                if (i != selectedUsers.length - 1) {
        //                    userAccounts = userAccounts + selectedUsers[i].Code + ";";
        //                    userNames = userNames + selectedUsers[i].NameENUS + ";";

        //                } else {
        //                    userAccounts = userAccounts + selectedUsers[i].Code + ";";
        //                    userNames = userNames + selectedUsers[i].NameENUS + ";";

        //                }
        //            }
        //            switch (role) {
        //                case "PMSupervisor":
        //                    $scope.entity.PMSupervisorAccount = userAccounts;
        //                    $scope.entity.PMSupervisorName = userNames;
        //                    break;
        //                case "MCCLApprover":
        //                    $scope.entity.MCCLApproverAccount = userAccounts;
        //                    $scope.entity.MCCLApproverName = userNames;
        //                    break;
        //            }
        //        }
        //    });
        //};

        $scope.download = function () {
            $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/DownLoadTemplate");
        };

        function loadData() {
            //获取项目基本数据
            $http.get(Utils.ServiceURI.Address() + "api/Closure/GetByProjectId/" + $scope.projectId).success(function (data) {
                if (data != "null") {
                    $scope.ClosureInfo = data;

                    $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetByProjectId/" + $scope.projectId, {
                        cache: false,
                        params: {
                            userAccount: window.currentUser.Code
                        }
                    }
                   ).success(function (entity) {

                       if (entity != "null") {
                           $scope.entity = entity;
                           $http.get(Utils.ServiceURI.Address() + "api/project/isFlowNodeFinished/" + $scope.projectId + "/Closure_WOCheckList/Closure_WOCheckList_ResultUpload").success(function (data) {
                               if (data == "true")
                                   $scope.isUploadTemplate = true;
                               else
                                   $scope.isUploadTemplate = false;
                           });
                       }

                   }).then(function (entity) {
                       if ($scope.entity != "null") {

                           $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetHistoryList/" + $scope.projectId).success(function (historyData) {
                               $scope.historyList = historyData;
                           }).error(function (err) {
                               var s = err;
                           });
                       };
                   });
                }
            });

            //$scope.$watch("entity.Comments", function (val) {
            //    console.log(val);
            //});

        }

        $scope.loadWOCheckList = function () {
            loadData();
        };

    }]);