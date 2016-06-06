reimageApp.controller('reimagePackageCtrl',
[
    "$scope",
    '$window',
     "$modal",
    '$location',
    '$routeParams',
    "reimageService",
    'approveDialogService',
    'redirectService',
     "messager",
    function ($scope, $window, $modal, $location, $routeParams, reimageService, approveDialogService, redirectService, messager) {
        $scope.pageType = $routeParams.PageType;
        $scope.projectId = $routeParams.projectId;
        $scope.entityId = $routeParams.entityId;
        $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);
        $scope.needCDOApproval = true;
        // debugger;
        switch ($routeParams.PageType) {
            case 'Approval':
            case 'View':
                $scope.isPageEditable = false;
                break;
            default:
                $scope.isPageEditable = true;
                $scope.uploadSet = ['1f7a5c25-223f-4ee3-9008-9f804f299c4c', 'e0c42475-254c-4839-8d48-8258b18dac23', 'f6f08648-d107-403f-a2ab-ea3a95ff0a7c'];
                break;
        }

        if ($scope.isPageEditable) {
            $scope.isEditAmountType = true;
            $scope.isEditComments = true;
            $scope.isEditReinvenstmentBasicInfo = true;
            $scope.isEditReinvstAndWriteOff = true;

        } else {
            $scope.isEditAmountType = false;
            $scope.isEditComments = false;
            $scope.isEditReinvenstmentBasicInfo = false;
            $scope.isEditReinvstAndWriteOff = false;
        }
        $scope.isShowReinvenstmentCost = true;
        $scope.isHistory = $routeParams.isHistory;
        $scope.subFlowCode = "Reimage_Package";
        $scope.packageInfo = {};
        $scope.packageInfo.ReinvenstmentType = null;
        $scope.packageInfo.ReinBasicInfo = {};
        $scope.packageInfo.WriteOff = {};
        $scope.packageInfo.ReinCost = {};

        $scope.entity = {
            ProjectId: $routeParams.projectId,
            CreateUserAccount: window.currentUser.Code
        };

        $scope.flowCode = "Reimage_Package";
        $scope.checkPointRefresh = true;

        $scope.submit = function () {
            if (checkValidate()) {

                approveDialogService.open($scope.projectId,
                   $scope.subFlowCode, "",
                   $scope.packageInfo.USCode,
                   $scope.packageInfo).then(function (approverInfo) {
                       $scope.approveDialogSubmit(approverInfo);
                   });
            }
        };

        var checkValidate = function () {
            var errors = [];
            //if ($.grep($scope.attachments || [], function (att, i) {
            //        return !!att.FileURL && att.RequirementId == "e0c42475-254c-4839-8d48-8258b18dac23";
            //}) == 0) {
            //    errors.push("请上传往来函件、业主证明附件！");
            //} 
            if ($.grep($scope.attachments || [], function (att, i) {
                   return !!att.FileURL && att.RequirementId == "f6f08648-d107-403f-a2ab-ea3a95ff0a7c";
            }) == 0) {
                //if ($scope.consInfo
                //    && $scope.consInfo.ReinvenstmentType != 1) {
                   errors.push("请上传New Store Layout！");
                //}
            }
            if (errors.length > 0) {
                messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
            }
            return errors.length == 0;
        }

        var resubmit = function () {
            $scope.packageInfo.SerialNumber = $routeParams.SN;
            $scope.packageInfo.LastUpdateUserAccount = window.currentUser.Code;
            $scope.IsClickSubmit = true;
            messager.confirm("确定要进行提交吗？", "fa-warning c_orange").then(function (result) {
                if (result) {
                    reimageService.resubmitPackage($scope.packageInfo).$promise.then(function () {
                        messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect($scope.packageInfo.WorkflowCode, $scope.projectId);
                        });
                    }, function (error) {
                        messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
                        $scope.IsClickSubmit = false;
                    });
                }
                else
                    $scope.IsClickSubmit = false;
            });
        };
        $scope.save = function () {
            save("save");
        }

        $scope.approveDialogSubmit = function (notifyUsersInfo) {
            $scope.packageInfo.AppUsers = notifyUsersInfo;
            if ($scope.pageType == "Resubmit") {
                resubmit();
            } else {
                save("submit");
            }
        };
        var loadPackageInfo = function () {
            reimageService.getReimageInfo({ projectId: $scope.projectId, entityId: $scope.entityId }).$promise.then(function (data) {

                if (data != null && data != "00000000-0000-0000-0000-000000000000") {
                    $scope.reimageInfo = data;
                }
            },
             function (error) {
                 messager.showMessage(error.statusText + "error in loadPackageInfo", "fa-warning c_orange");
             });
            reimageService.getPackageInfo({ projectId: $scope.projectId, entityId: $scope.entityId }).$promise.then(function (data) {

                if (data != null && data != "00000000-0000-0000-0000-000000000000") {
                    $scope.packageInfo = data;
                }
                if (data.IsShowSave && $scope.pageType == 'View') {
                    $scope.isPageEditable = $scope.isPageEditable || data.IsShowSave;
                }
            },
                function (error) {
                    messager.showMessage(error.statusText + "error in loadPackageInfo", "fa-warning c_orange");
                });
        }
        var save = function (action, redirectUrl) {

            $scope.packageInfo.projectId = $scope.projectId;

            if (action == "save") {
                $scope.IsClickSave = true;
                reimageService.savePackage($scope.packageInfo).$promise.then(function (data) {
                    loadPackageInfo();
                    if (redirectUrl) {
                        $window.location.href = redirectUrl;
                    } else {
                        messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                    }
                }, function (error) {
                    messager.showMessage("[[[保存失败]]]" + error.message, "fa-warning c_orange");
                });
            }
            else if (action == "submit") {

                $scope.IsClickSubmit = true;
                messager.confirm("确定要进行提交吗？", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        reimageService.submitPackage($scope.packageInfo).$promise.then(function (data) {
                            messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                                redirectService.flowRedirect($scope.packageInfo.WorkflowCode, $scope.projectId);
                            });
                        }, function (error) {
                            messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
                            $scope.IsClickSubmit = false;
                        });
                    }
                    else
                        $scope.IsClickSubmit = false;
                });
            } else { return; }
        }

        loadPackageInfo();
        $scope.approve = function () {
            $scope.IsClickApprove = true;
            $scope.packageInfo.SerialNumber = $routeParams.SN;
            $scope.packageInfo.LastUpdateUserAccount = window.currentUser.Code;
            messager.confirm("确定要进行审批吗？", "fa-warning c_orange").then(function (result) {
                if (result) {
                    reimageService.approvePackage($scope.packageInfo).$promise.then(function () {
                        messager.showMessage("[[[审批成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect($scope.packageInfo.WorkflowCode, $scope.projectId);
                        });
                    }, function (error) {
                        $scope.IsClickApprove = false;
                        messager.showMessage("[[[审批成功]]]" + error.message, "fa-warning c_orange");
                    });
                }
                else
                    $scope.IsClickApprove = false;
            });
        }
        $scope.returnToOriginator = function () {
            if (!$scope.packageInfo.Comments) {
                messager.showMessage("[[[请填写Comments]]]", "fa-warning c_orange").then(function () {
                    window.parent.scrollTo(0, $("#divComments textarea").offset().top);
                });
                return false;
            }
            $scope.IsClickReturn = true;
            $scope.packageInfo.SerialNumber = $routeParams.SN;
            $scope.packageInfo.LastUpdateUserAccount = window.currentUser.Code;
            messager.confirm("[[[确定要进行退回吗？]]]", "fa-warning c_orange").then(function (result) {
                if (result) {
                    reimageService.returnPackage($scope.packageInfo).$promise.then(function () {
                        messager.showMessage("[[[退回成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect($scope.packageInfo.WorkflowCode, $scope.projectId);
                        });
                    }, function (error) {
                        messager.showMessage("[[[退回失败]]]" + error.message, "fa-warning c_orange");
                        $scope.IsClickReturn = false;
                    });
                }
                else
                    $scope.IsClickReturn = false;
            });
            return true;
        }

        $scope.recall = function () {
            $scope.packageInfo.SerialNumber = $routeParams.SN;
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
                            $scope.packageInfo.Comments = entity.Comment;
                            reimageService.recallPackage($scope.packageInfo).$promise.then(function () {
                                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                                    redirectService.flowRedirect($scope.packageInfo.WorkflowCode, $scope.projectId);
                                });
                            }, function (error) {
                                messager.showMessage("[[[撤回失败]]]" + error.message, "fa-warning c_orange");
                            });
                        });
                    }
                });
        }
        $scope.reject = function () {
            if (!$scope.packageInfo.Comments) {
                messager.showMessage("[[[请填写Comments]]]", "fa-warning c_orange").then(function () {
                    window.parent.scrollTo(0, $("#divComments textarea").offset().top);
                });
                return false;
            }
            $scope.IsClickReject = true;
            $scope.packageInfo.SerialNumber = $routeParams.SN;
            $scope.packageInfo.LastUpdateUserAccount = window.currentUser.Code;
            messager.confirm("[[[Reject后该流程将直接终止，请确认需要执行该操作吗？]]]", "fa-warning c_orange")
                    .then(function (result) {
                        if (result) {
                            reimageService.rejectPackage($scope.packageInfo).$promise.then(function () {
                                messager.showMessage("[[[拒绝成功]]]", "fa-check c_green").then(function () {
                                    redirectService.flowRedirect($scope.packageInfo.WorkflowCode, $scope.projectId);
                                });
                            }, function (error) {
                                messager.showMessage("[[[拒绝失败]]]" + error.message, "fa-warning c_orange");
                            });
                        } else {
                            $scope.IsClickReject = false;
                        }
                    });
        }

        $scope.edit = function () {
            messager.confirm("Reimage Package 审批流程已经完成，现在编辑会导致流程重新提交，是否确认？", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        reimageService.editPackage($scope.packageInfo).$promise.then(function (response) {
                            $window.location.href = Utils.ServiceURI.WebAddress() + response.TaskUrl;
                        }, function () {
                            messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                        });
                    }
                });
        }

        $scope.beforePackDownload = function (callback) {
            callback && callback();
        }
    }
]);