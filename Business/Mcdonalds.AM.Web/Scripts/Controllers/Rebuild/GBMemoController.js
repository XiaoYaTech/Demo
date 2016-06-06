rebuildApp.controller("GBMemoController", [
    '$scope',
    '$routeParams',
    "$modal",
    "$location",
    "$selectUser",
    "$window",
    "taskWorkService",
    "approveDialogService",
    "rebuildService",
    "redirectService",
    "messager",
    function ($scope, $routeParams,$modal, $location, $selectUser, $window, taskWorkService, approveDialogService, rebuildService,redirectService, messager) {
        $scope.projectId = $routeParams.projectId;
        $scope.pageType = $routeParams.PageType;
        
        $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);
        switch ($routeParams.PageType) {
        case 'Approval':
        case 'View':
        case 'Notify':
            $scope.isPageEditable = false;
            break;
        default:
            $scope.isPageEditable = true;
            break;
        }
        $scope.userAccount = window.currentUser.Code;
        $scope.entityId = $routeParams.entityId;
        $scope.isHistory = $routeParams.isHistory;
        $scope.subFlowCode = "Rebuild_GBMemo";
        $scope.checkPointRefresh = true;
        $scope.entity = {};

        var loadGBMemoInfo = function () {
            return rebuildService.getGBMemoInfo({ projectId: $scope.projectId, entityId: $scope.entityId }).$promise.then(function (data) {
                if (data != null) {
                    $scope.entity = data;
                    if (data.IsShowSave && $scope.pageType == 'View') {
                        $scope.isPageEditable = $scope.isPageEditable || data.IsShowSave;
                    }
                }
            },
            function (error) {
                messager.showMessage(error.statusText, "fa-warning c_orange");
            });
        }
        loadGBMemoInfo();
        var save = function (action) {
            if ($scope.entity.Id == null) {
                $scope.entity.Id = "00000000-0000-0000-0000-000000000000";
            }
            $scope.entity.projectId = $scope.projectId;
            $scope.entity.LastUpdateUserAccount = window.currentUser.Code;
            $scope.entity.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.entity.LastUpdateUserNameENUS = window.currentUser.NameENUS;

            if (action == "save") {
                $scope.IsClickSave = true;
                $scope.IsClickSubmit = true;
                rebuildService.saveGBMemo($scope.entity).$promise.then(function (data) {
                    loadGBMemoInfo();
                    messager.showMessage("[[[保存成功]]]", "fa-warning c_orange");
                }, function (error) {
                    messager.showMessage("[[[保存失败]]]" + error.message, "fa-warning c_orange");
                });
                $scope.IsClickSave = false;
                $scope.IsClickSubmit = false;
            }
            else if (action == "submit") {
                $scope.IsClickSubmit = true;
                $scope.IsClickSave = true;
                rebuildService.submitGBMemo($scope.entity).$promise.then(function (data) {
                    messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                        redirectService.flowRedirect($scope.entity.WorkflowCode, $scope.projectId);
                    });
                }, function (error) {
                    $scope.IsClickSave = false;
                    $scope.IsClickSubmit = false;
                    messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
                });
            } else
                return;
        }

        $scope.save = function () {
            save("save");
        }
        $scope.submit = function () {
            if (!($scope.entity.IsClosed) && !($scope.entity.IsInOperation)) {
                messager.showMessage("[[[请选择Store Re-image status(餐厅是否停业改造)]]]", "fa-warning c_orange");
                return false;
            }
            if ($scope.entity.GBDate != null
                    && $scope.entity.ConstCompletionDate != null) {
                var gbDate = moment(moment($scope.entity.GBDate).format("YYYY-MM-DD")); 
                var consDate = moment(moment($scope.entity.ConstCompletionDate).format("YYYY-MM-DD"));
                if (consDate.isBefore(gbDate)) {
                    messager.showMessage("[[[Construction Completion Date 不能早于 GB Date]]]", "fa-warning c_orange");
                    return false;
                }
            }
            if ($scope.entity.ReopenDate != null && $scope.entity.ConstCompletionDate != null) {
                var consDate = moment(moment($scope.entity.ConstCompletionDate).format("YYYY-MM-DD"));
                var reopDate = moment(moment($scope.entity.ReopenDate).format("YYYY-MM-DD"));
                if (reopDate.isBefore(consDate)) {
                    messager.showMessage("[[[Re-open Date 不能早于 Construction Completion Date]]]", "fa-warning c_orange");
                    return false;
                }
            }
            if ($scope.entity.IsClosed && ($scope.entity.ReopenDate == null || $scope.entity.ReopenDate == "")) {
                messager.showMessage("[[[请选择Re-open Date]]]", "fa-warning c_orange");
                return false;
            }
            approveDialogService.open($scope.projectId, $scope.subFlowCode, "", "", $scope.entity).then(function (approverInfo) {
                $scope.approveDialogSubmit(approverInfo);
            });
        }
        var resubmit = function () {
            $scope.entity.SerialNumber = $routeParams.SN;
            $scope.entity.LastUpdateUserAccount = window.currentUser.Code;
            $scope.entity.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.entity.LastUpdateUserNameENUS = window.currentUser.NameENUS;
            $scope.IsClickSubmit = true;
            $scope.IsClickSave = true;
            rebuildService.resubmitGBMemo($scope.entity).$promise.then(function () {
                messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                    redirectService.flowRedirect($scope.entity.WorkflowCode, $scope.projectId);
                });
            }, function (error) {
                $scope.IsClickSave = false;
                $scope.IsClickSubmit = false;
                messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
            });
        };
        $scope.approveDialogSubmit = function (notifyUsersInfo) {
            $scope.entity.AppUsers = notifyUsersInfo;
            if ($scope.pageType == "Resubmit") {
                resubmit();
            } else {
                save("submit");
            }
        };
        $scope.approve = function () {
            $scope.IsClickApprove = true;
            $scope.entity.SerialNumber = $routeParams.SN;
            $scope.entity.LastUpdateUserAccount = window.currentUser.Code;
            $scope.entity.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.entity.LastUpdateUserNameENUS = window.currentUser.NameENUS;
            rebuildService.approveGBMemo($scope.entity).$promise.then(function () {
                messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                    redirectService.flowRedirect($scope.entity.WorkflowCode, $scope.projectId);
                });
            }, function (error) {
                $scope.IsClickApprove = false;
                messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
            });
        };

        $scope.recall = function () {
            $scope.entity.SerialNumber = $routeParams.SN;
            $scope.entity.LastUpdateUserAccount = window.currentUser.Code;
            $scope.entity.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.entity.LastUpdateUserNameENUS = window.currentUser.NameENUS;

            messager.confirm("[[[确认需要撤销吗？]]]", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        $modal.open({
                            backdrop: 'static',
                            templateUrl: Utils.ServiceURI.AppUri + "Template/Recall",
                            size: "lg",
                            resolve: {
                                entity: function () {
                                    return {
                                        Comment: ""
                                    };
                                }
                            },
                            controller: ["$scope", "$modalInstance", "entity", function ($modalScope, $modalInstance, entity) {
                                $modalScope.entity = entity;
                                $modalScope.ok = function () {
                                    $modalInstance.close($modalScope.entity);
                                };

                                $modalScope.cancel = function () {
                                    $modalInstance.dismiss("");
                                };
                            }]
                        }).result.then(function (entity) {
                            $scope.entity.Comments = entity.Comment;
                            rebuildService.recallGBMemo($scope.entity).$promise.then(function () {
                                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                                    redirectService.flowRedirect($scope.entity.WorkflowCode, $scope.projectId);
                                });
                            }, function (error) {
                                messager.showMessage("[[[撤回失败]]]" + error.message, "fa-warning c_orange");
                            });
                        });
                    }
                });
        }
        $scope.edit = function () {
            messager.confirm("[[[GBMemo 审批流程已经完成，现在编辑会导致流程重新提交，是否确认]]]？", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        rebuildService.editGBMemo($scope.entity).$promise.then(function (response) {
                            $window.location.href = Utils.ServiceURI.WebAddress() + response.TaskUrl;
                        }, function () {
                            messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                        });
                    }
                });
        }
        $scope.resubmit = function () {
            resubmit();
        }
        $scope.returnToOriginator = function () {
            if (!$scope.entity.Comments) {
                messager.showMessage("[[[请先填写意见!]]]", "fa-warning c_orange");
                return false;
            }
            messager.confirm("[[[确认需要Return吗？]]]", "fa-warning c_orange").then(function (result) {
                if (result) {
                    $scope.IsClickReturn = true;
                    $scope.entity.LastUpdateUserAccount = window.currentUser.Code;
                    $scope.entity.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
                    $scope.entity.LastUpdateUserNameENUS = window.currentUser.NameENUS;
                    $scope.entity.SerialNumber = $routeParams.SN;
                    rebuildService.returnGBMemo($scope.entity).$promise.then(function () {
                        messager.showMessage("[[[退回成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect($scope.entity.WorkflowCode, $scope.projectId);
                        });
                    }, function (error) {
                        messager.showMessage("[[[退回失败]]]" + error.message, "fa-warning c_orange");
                    });
                }
            });
            return true;
        }
        $scope.notify = function () {
            $selectUser.open({
                storeCode: $scope.entity.Store.StoreBasicInfo.StoreCode,
                positionCodes: ["suoya612036", "suoya303055", "suoya303054"],
                checkUsers: function () { return true; },
                OnUserSelected: function (users) {
                    $scope.entity.SerialNumber = $routeParams.SN;
                    $scope.entity.LastUpdateUserAccount = window.currentUser.Code;
                    $scope.entity.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
                    $scope.entity.LastUpdateUserNameENUS = window.currentUser.NameENUS;
                    if (users== null || users.length == 0)
                    {
                        return;
                    }
                    $scope.acting = true;
                    rebuildService.notifyGBMemo({
                        Entity: $scope.entity,
                        Receivers: users
                    }).$promise.then(function (response) {
                        messager.showMessage("[[[发送成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect($scope.entity.WorkflowCode, $scope.projectId);
                        });
                    }, function (response) {
                        messager.showMessage("[[[发送失败]]]", "fa-warning c_orange");
                        $scope.acting = false;
                    });
                }
            });
        };
        taskWorkService.ifUndo("Rebuild_GBMemo", $scope.projectId).then(function (result) {
            $scope.unNotify = result;
        });
    }
]);