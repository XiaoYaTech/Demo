!function () {
    var RenewalApp = angular.module("amApp");
    RenewalApp.controller("consInfoController", [
        "$scope",
        "$routeParams",
        "$window",
        "$modal",
        "renewalService",
        "approveDialogService",
        "messager",
        "redirectService",
        function ($scope, $routeParams, $window, $modal, renewalService, approveDialogService, messager, redirectService) {
            $scope.projectId = $routeParams.projectId;
            $scope.isHistory = $routeParams.isHistory;
            $scope.from = $routeParams.from;
            $scope.checkPointRefresh = true;
            $scope.hasReinvenstment = false;
            var init = function () {
                messager.blockUI("[[[正在初始化页面，请稍等]]]...");
                renewalService.initConsInfoPage({
                    projectId: $scope.projectId,
                    id: $routeParams.entityId
                }).$promise.then(function (data) {
                    messager.unBlockUI();
                    $scope.consInfo = data.Entity;
                    $scope.info = data.Info;
                    $scope.reinBasicInfo = data.ReinBasicInfo || {};
                    $scope.reinCost = data.ReinCost || {};
                    $scope.writeOffAmount = data.WriteOff || {};
                    $scope.projectComment = data.ProjectComment;
                    $scope.editable = data.Editable;
                    $scope.recallable = data.Recallable;
                    $scope.savable = data.Savable;
                }, function () {
                    messager.unBlockUI();
                    messager.confirm("[[[页面初始化出错,点击确定重新加载]]]？", "fa-warning c_orange").then(function (result) {
                        if (result) {
                            init();
                        } else {
                            $window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
                        }
                    });
                });
            }
            init();
            $scope.$watch("consInfo.HasReinvenstment", function (val) {
                if (val === true) {
                    $scope.uploadSet = ['3578ca53-ead1-480a-b995-e1408fc605ae', '7ed67241-4266-4b31-a6bb-35a67429881e'];
                    if ($scope.savable) {
                        $scope.viewUploadSet = ['3578ca53-ead1-480a-b995-e1408fc605ae', '7ed67241-4266-4b31-a6bb-35a67429881e'];
                    }
                }
                else {
                    $scope.uploadSet = [];
                    $scope.viewUploadSet = [];
                }
            });
            $scope.save = function () {
                $scope.acting = true;
                renewalService.saveConsInfo({
                    Entity: $scope.consInfo,
                    Info: $scope.info,
                    ReinBasicInfo: $scope.reinBasicInfo,
                    ReinCost: $scope.reinCost,
                    WriteOff: $scope.writeOffAmount,
                    ProjectComment: $scope.projectComment
                }).$promise.then(function () {
                    messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                    $scope.acting = false;
                }, function () {
                    messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
                    $scope.acting = false;
                });
            };
            $scope.submit = function (frm) {
                if (frm.$valid) {
                    $scope.acting = true;
                    var errors = [];
                    var gbDate = $scope.reinBasicInfo.GBDate;
                    var consDate = $scope.reinBasicInfo.ConsCompletionDate;
                    var reopDate = $scope.reinBasicInfo.ReopenDate;
                    if (gbDate != null && consDate != null) {
                        if (consDate < gbDate) {
                            errors.push("[[[Construction Completion Date 不能早于 GB Date]]]");
                        }
                    }
                    if (reopDate != null && consDate != null) {
                        if (reopDate < consDate) {
                            errors.push("[[[Reopen Date 不能早于 Construction Completion Date]]]");
                        }
                    }
                    if ($scope.consInfo.HasReinvenstment === true) {
                        if ($.grep($scope.attachments || [], function (att, i) {
                        return !!att.FileURL && att.RequirementId == "3578ca53-ead1-480a-b995-e1408fc605ae";
                        }) == 0) {
                            errors.push("[[[请上传Current Store layout]]]");
                        }
                        if ($.grep($scope.attachments || [], function (att, i) {
                        return !!att.FileURL && att.RequirementId == "7ed67241-4266-4b31-a6bb-35a67429881e";
                        }) == 0) {
                            errors.push("[[[请上传After Renewal Store layout]]]");
                        }
                    }
                    if (errors.length > 0) {
                        messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
                        $scope.acting = false;
                        return;
                    }
                    var submit = function () {
                        renewalService.submitConsInfo({
                            Entity: $scope.consInfo,
                            Info: $scope.info,
                            ReinBasicInfo: $scope.reinBasicInfo,
                            ReinCost: $scope.reinCost,
                            WriteOff: $scope.writeOffAmount,
                            ProjectComment: $scope.projectComment
                        }).$promise.then(function () {
                            messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                                redirectService.flowRedirect("Renewal_ConsInfo", $scope.projectId);
                            });
                        }, function () {
                            messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
                            $scope.acting = false;
                        });
                    }
                    if ($scope.consInfo.HasReinvenstment) {
                        approveDialogService.open($scope.consInfo.ProjectId, "Renewal_ConsInfo").then(function (approvers) {
                            $scope.consInfo.AppUsers = approvers;
                            submit();
                        }, function () {
                            $scope.acting = false;
                        });
                    }
                    else {
                        submit();
                    }
                }
            };
            $scope.approve = function () {
                $scope.acting = true;
                messager.confirm("[[[确认审批通过吗]]]？", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        renewalService.approveConsInfo({
                            Entity: $scope.consInfo,
                            SN: $routeParams.SN,
                            ProjectComment: $scope.projectComment
                        }).$promise.then(function () {
                            messager.showMessage("[[[审批成功]]]", "fa-check c_green").then(function () {
                                redirectService.flowRedirect("Renewal_ConsInfo", $scope.projectId);
                            });
                        }, function () {
                            messager.showMessage("[[[审批成功]]]", "fa-warning c_orange");
                            $scope.acting = false;
                        });
                    } else {
                        $scope.acting = false;
                    }
                });
            };
            $scope.returnProcess = function () {
                $scope.acting = true;
                if (!$scope.projectComment) {
                    messager.showMessage("[[[请填写Comments]]]", "fa-warning c_orange");
                    $scope.acting = false;
                    return;
                };
                messager.confirm("[[[确认退回吗]]]？", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        renewalService.returnConsInfo({
                            Entity: $scope.consInfo,
                            SN: $routeParams.SN,
                            ProjectComment: $scope.projectComment
                        }).$promise.then(function () {
                            messager.showMessage("[[[退回成功]]]", "fa-check c_green").then(function () {
                                redirectService.flowRedirect("Renewal_ConsInfo", $scope.projectId);
                            });
                        }, function () {
                            messager.showMessage("[[[退回失败]]]", "fa-warning c_orange");
                            $scope.acting = false;
                        });
                    } else {
                        $scope.acting = false;
                    }
                });
            };
            $scope.resubmit = function (frm) {
                if (frm.$valid) {
                    var errors = [];
                    if ($.grep($scope.attachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "3578ca53-ead1-480a-b995-e1408fc605ae";
                    }) == 0) {
                        errors.push("[[[请上传Current Store layout]]]");
                    };
                    if ($.grep($scope.attachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "7ed67241-4266-4b31-a6bb-35a67429881e";
                    }) == 0) {
                        errors.push("[[[请上传After Renewal Store layout]]]");
                    };
                    if (errors.length > 0) {
                        messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
                        $scope.acting = false;
                        return;
                    };
                    var resubmit = function () {
                        renewalService.resubmitConsInfo({
                            Entity: $scope.consInfo,
                            Info: $scope.info,
                            ReinBasicInfo: $scope.reinBasicInfo,
                            ReinCost: $scope.reinCost,
                            WriteOff: $scope.writeOffAmount,
                            SN: $routeParams.SN,
                            ProjectComment: $scope.projectComment
                        }).$promise.then(function () {
                            messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                                redirectService.flowRedirect("Renewal_ConsInfo", $scope.projectId);
                            });
                        }, function () {
                            messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
                        });
                    };
                    if ($scope.consInfo.HasReinvenstment) {
                        approveDialogService.open($scope.consInfo.ProjectId, "Renewal_ConsInfo").then(function (approvers) {
                            $scope.consInfo.AppUsers = approvers;
                            resubmit();
                        });
                    } else {
                        resubmit();
                    };
                };
            };
            $scope.edit = function () {
                $scope.acting = true;
                messager.confirm("[[[Renewal Cons Info 审批流程已经完成，现在编辑会导致流程重新提交，是否确认]]]？", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        renewalService.editConsInfo($scope.consInfo).$promise.then(function (editResult) {
                            messager.showMessage("[[[操作成功]]]", "fa-check c_green").then(function () {
                                $window.location.href = Utils.ServiceURI.WebAddress() + editResult.TaskUrl;
                            });
                        }, function () {
                            messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                            $scope.acting = false;
                        });
                    } else {
                        $scope.acting = false;
                    }
                });
            };
            $scope.recall = function () {
                $scope.acting = true;
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
                            renewalService.recallConsInfo({
                                Entity: $scope.consInfo,
                                SN: $routeParams.SN,
                                ProjectComment: entity.Comment
                            }).$promise.then(function () {
                                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                                    redirectService.flowRedirect("Renewal_ConsInfo", $scope.projectId);
                                });
                            }, function () {
                                messager.showMessage("[[[撤回失败]]]", "fa-warning c_orange");
                                $scope.acting = false;
                            });
                        }, function () {
                            $scope.acting = false;
                        });
                    } else {
                        $scope.acting = false;
                    }
                });
            };
        }
    ]);
}();