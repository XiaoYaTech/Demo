!function () {
    var ctrlModule = angular.module("mcd.am.tempClosure.controllers.closurePackage", [
        "mcd.am.services.tempClosure",
        "mcd.am.services.projectUsers"
    ]);
    ctrlModule.controller("closurePackageController", [
        "$scope",
        "$routeParams",
        "$window",
        "$modal",
        "$location",
        "messager",
        "projectUsersService",
        "tempClosureService",
        "tempClosureApprovalDialogService",
        "storeService",
        "redirectService",
        function (
                $scope,
                $routeParams,
                $window,
                $modal,
                $location,
                messager,
                projectUsersService,
                tempClosureService,
                tempClosureApprovalDialogService,
                storeService,
                redirectService) {
            $scope.projectId = $routeParams.projectId;
            $scope.isHistory = $routeParams.isHistory;
            $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);
            $scope.subFlowCode = "TempClosure_ClosurePackage";
            $scope.checkPointRefresh = true;
            tempClosureService.get({
                projectId: $scope.projectId
            }).$promise.then(function (data) {
                $scope.TPCLS = data;
            });
            tempClosureService.getClosurePackageInfo({
                projectId: $scope.projectId,
                Id: $routeParams.entityId
            }).$promise.then(function (data) {
                $scope.info = data.Info;
                $scope.closurePackage = data.ClosurePackage;
                $scope.approver = data.Approver || {};
                $scope.projectComment = data.ProjectComment;
                $scope.editable = data.Editable;
                $scope.recallable = data.Recallable;
                $scope.rejectable = data.Rejectable;
                $scope.savable = data.Savable;
                if (data.Savable) {
                    $scope.viewUploadSet = ['f1b36b8a-bfbb-4015-8c83-dda4f34956b2', 'dc1c194c-4a47-44a9-b465-21b89f629340'];
                }
            });
            $scope.openDate = function ($event, tag) {
                $event.preventDefault();
                $event.stopPropagation();
                $scope[tag] = true;
            };
            $scope.uploadAttachmentFinish = function (up, files) {
                $scope.checkPointRefresh = true;
            };
            $scope.deleteAttachmentFinish = function (id, requirementId) {
                $scope.checkPointRefresh = true;
            };
            $scope.saveClosurePackage = function () {
                $scope.acting = true;
                tempClosureService.saveClosurePackage({
                    Entity: $scope.closurePackage,
                    projectComment: $scope.projectComment
                }).$promise.then(function () {
                    messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                    $scope.acting = false;
                }, function () {
                    messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
                    $scope.acting = false;
                });
            };
            $scope.beforePackDownload = function () {
                tempClosureService.saveClosurePackage({
                    Entity: $scope.closurePackage,
                    projectComment: $scope.projectComment
                });
                return true;
            };
            $scope.submitClosurePackage = function (frm) {
                if (frm.$valid) {
                    $scope.acting = true;
                    var errors = [];
                    //if (!$scope.closurePackage.RentReliefStartDate) {
                    //    errors.push("请填写减免租起始日期");
                    //}
                    //if (!$scope.closurePackage.RentReliefEndDate) {
                    //    errors.push("请填写减免租结束日期");
                    //}
                    //if (!$scope.closurePackage.RentReliefClause) {
                    //    errors.push("请填写减免租条款");
                    //}
                    if ($.grep($scope.packageAttachments || [], function (att, i) {
                        return !!att.FileURL && att.RequirementId == "f1b36b8a-bfbb-4015-8c83-dda4f34956b2";
                    }) == 0) {
                        errors.push("请上传Excutive Summary");
                    }
                    if (errors.length > 0) {
                        messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
                        $scope.acting = false;
                        return;
                    }
                    tempClosureApprovalDialogService.open($scope.projectId, $scope.TPCLS.USCode).then(function (approver) {
                        messager.blockUI("正在处理中，请稍等...");
                        tempClosureService.submitClosurePackage({
                            Entity: $scope.closurePackage,
                            projectComment: $scope.projectComment,
                            ProjectDto: approver
                        }).$promise.then(function () {
                            messager.unBlockUI();
                            messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                                //$window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
                                redirectService.flowRedirect($scope.subFlowCode, $scope.projectId);
                                messager.unBlockUI();
                            });
                        }, function () {
                            messager.unBlockUI();
                            messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
                            $scope.acting = false;
                        });
                    }, function () {
                        $scope.acting = false;
                    });
                }
            };
            $scope.approveClosurePackage = function () {
                $scope.acting = true;
                tempClosureService.approveClosurePackage({
                    Entity: $scope.closurePackage,
                    sn: $routeParams.SN,
                    projectComment: $scope.projectComment
                }).$promise.then(function () {
                    messager.showMessage("[[[审批成功]]]", "fa-check c_green").then(function () {
                        //$window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
                        redirectService.flowRedirect($scope.subFlowCode, $scope.projectId);
                        messager.unBlockUI();
                    });
                }, function () {
                    messager.showMessage("[[[审批成功]]]", "fa-warning c_orange");
                    $scope.acting = false;
                });
            };
            $scope.returnClosurePackage = function () {
                $scope.acting = true;
                if (!$scope.projectComment) {
                    messager.showMessage("[[[请填写Comments]]]", "fa-warning c_orange").then(function () {
                        window.parent.scrollTo(0, $("#divComments textarea").offset().top);
                    });
                    $scope.acting = false;
                    return;
                };
                messager.confirm("[[[确定要进行退回吗？]]]", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        tempClosureService.returnClosurePackage({
                            Entity: $scope.closurePackage,
                            sn: $routeParams.SN,
                            projectComment: $scope.projectComment
                        }).$promise.then(function () {
                            messager.showMessage("[[[退回成功]]]", "fa-check c_green").then(function () {
                                //$window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
                                redirectService.flowRedirect($scope.subFlowCode, $scope.projectId);
                                messager.unBlockUI();
                            });
                        }, function () {
                            messager.showMessage("[[[退回失败]]]", "fa-warning c_orange");
                            $scope.acting = false;
                        });
                    }
                    else {
                        $scope.acting = false;
                    }
                });
            };
            $scope.rejectClosurePackage = function () {
                $scope.acting = true;
                if (!$scope.projectComment) {
                    messager.showMessage("[[[请填写Comments]]]", "fa-warning c_orange").then(function () {
                        window.parent.scrollTo(0, $("#divComments textarea").offset().top);
                    });
                    $scope.acting = false;
                    return;
                };
                messager.confirm("[[[Reject后该流程将直接终止，请确认需要执行该操作吗？]]]", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        tempClosureService.rejectClosurePackage({
                            Entity: $scope.closurePackage,
                            sn: $routeParams.SN,
                            projectComment: $scope.projectComment
                        }).$promise.then(function () {
                            messager.showMessage("[[[拒绝成功]]]", "fa-check c_green").then(function () {
                                //$window.location = Utils.ServiceURI.WebAddress() + "/redirect";
                                redirectService.flowRedirect($scope.subFlowCode, $scope.projectId);
                                messager.unBlockUI();
                            });
                        }, function (error) {
                            messager.showMessage("[[[拒绝失败]]]" + error.message, "fa-warning c_orange");
                            $scope.acting = false;
                        });
                    }
                    else {
                        $scope.acting = false;
                    }
                });
            };
            $scope.resubmitClosurePackage = function (frm) {
                if (!frm.$valid)
                    return false;
                $scope.acting = true;
                var errors = [];
                //if (!$scope.closurePackage.RentReliefStartDate) {
                //    errors.push("请填写减免租起始日期");
                //}
                //if (!$scope.closurePackage.RentReliefEndDate) {
                //    errors.push("请填写减免租结束日期");
                //}
                //if (!$scope.closurePackage.RentReliefClause) {
                //    errors.push("请填写减免租条款");
                //}
                if ($.grep($scope.packageAttachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "f1b36b8a-bfbb-4015-8c83-dda4f34956b2";
                }) == 0) {
                    errors.push("请上传Excutive Summary");
                }
                if (errors.length > 0) {
                    messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
                    $scope.acting = false;
                    return;
                }
                tempClosureApprovalDialogService.open($scope.projectId, $scope.TPCLS.USCode).then(function (projectDto) {
                    messager.blockUI("正在处理中，请稍等...");
                    tempClosureService.resubmitClosurePackage({
                        sn: $routeParams.SN,
                        Entity: $scope.closurePackage,
                        projectComment: $scope.projectComment,
                        ProjectDto: projectDto
                    }).$promise.then(function () {
                        messager.unBlockUI();
                        messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                            //$window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
                            redirectService.flowRedirect($scope.subFlowCode, $scope.projectId);
                            messager.unBlockUI();
                        });
                    }, function () {
                        messager.unBlockUI();
                        messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
                        $scope.acting = false;
                    });
                }, function () {
                    $scope.acting = false;
                });
            };
            $scope.editClosurePackage = function () {
                messager.confirm("Closure Package 审批流程已经完成，现在编辑会导致流程重新提交，是否确认？", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        tempClosureService.editClosurePackage($scope.closurePackage).$promise.then(function (editResult) {
                            messager.showMessage("[[[操作成功]]]", "fa-check c_green").then(function () {
                                //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                                messager.unBlockUI();
                                $window.location.href = Utils.ServiceURI.WebAddress() + editResult.TaskUrl;
                            });
                        }, function () {
                            messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                        });
                    }
                });
            };
            $scope.recallClosurePackage = function () {
                messager.confirm("[[[确认需要撤销吗？]]]", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        $modal.open({
                            templateUrl: Utils.ServiceURI.AppUri + "Template/Recall",
                            backdrop: 'static',
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
                            $scope.projectComment = entity.Comment;
                            tempClosureService.recallClosurePackage({
                                sn: $routeParams.SN,
                                Entity: $scope.closurePackage,
                                projectComment: $scope.projectComment
                            }).$promise.then(function () {
                                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                                    //$window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
                                    redirectService.flowRedirect($scope.subFlowCode, $scope.projectId);
                                    messager.unBlockUI();
                                });
                            }, function () {
                                messager.showMessage("[[[撤回失败]]]", "fa-warning c_orange");
                            });
                        });
                    }
                });
            };
            $scope.checkClosurePackageUpload = function () {
                if ($.grep($scope.packageAttachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "1e9b17af-357a-4dc9-8a60-17766663fb75";
                }) == 0) {
                    messager.showMessage("请上传Signed Agreement文件", "fa-warning c_red");
                } else {
                    tempClosureService.confirmClosurePackage($scope.closurePackage).$promise.then(function () {
                        messager.showMessage("[[[操作成功]]]", "fa-check c_green").then(function () {
                            //$window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
                            redirectService.flowRedirect($scope.subFlowCode, $scope.projectId);
                            messager.unBlockUI();
                        });
                    });
                }
            }
        }
    ]);
}();