!function () {
    var RenewalApp = angular.module("amApp");
    RenewalApp.controller("legalApprovalController", [
        "$scope",
        "$routeParams",
        "$window",
        "$modal",
        "renewalService",
        "messager",
        "approveDialogService",
        "redirectService",
        function ($scope, $routeParams, $window, $modal, renewalService, messager, approveDialogService, redirectService) {
            $scope.projectId = $routeParams.projectId;
            $scope.from = $routeParams.from;
            $scope.isHistory = $routeParams.isHistory;
            $scope.checkPointRefresh = true;
            messager.blockUI("[[[正在初始化页面，请稍等]]]...");
            var init = function () {
                renewalService.initLegalApprovalPage({
                    projectId: $scope.projectId,
                    id: $routeParams.entityId
                }).$promise.then(function (data) {
                    messager.unBlockUI();
                    $scope.legalApproval = data.Entity;
                    $scope.legalApproval.ReviewStatus = $scope.legalApproval.ReviewStatus || "Endorsed";
                    $scope.info = data.Info;
                    $scope.isGeneralCounsel = data.IsGeneralCounsel;
                    $scope.projectComment = data.ProjectComment;
                    $scope.editable = data.Editable;
                    $scope.recallable = data.Recallable;
                    $scope.savable = data.Savable;
                    $scope.$watch("legalApproval.IsUrgency", function (val) {
                        if (val === false) {
                            $scope.legalApproval.UrgencyReason = "";
                        }
                    });
                    $scope.$watch("legalApproval.OtherIssure", function (val) {
                        if (val === false) {
                            $scope.legalApproval.OtherIssureDesc = "";
                        }
                    });
                    $scope.$watch("legalApproval.IsUrgency", function (val) {
                        if (val === false) {
                            $scope.legalApproval.UrgencyReason = "";
                        }
                    });
                    $scope.$watch("legalApproval.NoneOfAbove", function (val) {
                        if (val === true) {
                            $scope.legalApproval.IsRecentTransfer = false;
                            $scope.legalApproval.IsIntermediaries = false;
                            $scope.legalApproval.IsRelatedParties = false;
                            $scope.legalApproval.IsBroker = false;
                            $scope.legalApproval.IsPTTP = false;
                            $scope.legalApproval.IsASIIWGO = false;
                            $scope.legalApproval.IsNoBLClause = false;
                            $scope.legalApproval.IsOFAC = false;
                            $scope.legalApproval.IsAntiC = false;
                            $scope.legalApproval.IsBenefitConflict = false;
                        }
                    });
                    $scope.$watch("legalApproval.IsRecentTransfer+legalApproval.IsIntermediaries+legalApproval.IsRelatedParties+legalApproval.IsBroker+legalApproval.IsPTTP+legalApproval.IsASIIWGO+legalApproval.IsNoBLClause+legalApproval.IsOFAC+legalApproval.IsAntiC+legalApproval.IsBenefitConflict", function () {
                        if ($scope.legalApproval.IsRecentTransfer
                            || $scope.legalApproval.IsIntermediaries
                            || $scope.legalApproval.IsRelatedParties
                            || $scope.legalApproval.IsBroker
                            || $scope.legalApproval.IsPTTP
                            || $scope.legalApproval.IsASIIWGO
                            || $scope.legalApproval.IsNoBLClause
                            || $scope.legalApproval.IsOFAC
                            || $scope.legalApproval.IsAntiC
                            || $scope.legalApproval.IsBenefitConflict) {
                            $scope.legalApproval.NoneOfAbove = false;
                        }  
                    });
                    if (data.Savable) {
                        $scope.viewUploadSet = ['e26669ad-a029-4eb5-9fdc-97e1add0f10d'];
                    }
                    if (!$scope.isGeneralCounsel) {
                        $scope.approvalUploadSet = ['69e577c6-6dae-4d5c-ab62-b8c220cd04a8'];
                    }
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
            };
            init();
            $scope.save = function () {
                $scope.acting = true;
                renewalService.saveLegalApproval({
                    Entity: $scope.legalApproval,
                    ProjectComment: $scope.projectComment
                }).$promise.then(function () {
                    messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                    $scope.acting = false;
                }, function () {
                    messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
                    $scope.acting = false;
                });
            };
            $scope.submit = function () {
                $scope.acting = true; ""
                var errors = [];
                if ($.grep($scope.attachments || [], function (att, i) {
                return !!att.FileURL && att.RequirementId == "e26669ad-a029-4eb5-9fdc-97e1add0f10d";
                }) == 0) {
                    errors.push("[[[请上传Renewal Contract Draft]]]");
                }
                if (errors.length > 0) {
                    messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
                    $scope.acting = false;
                    return;
                }
                approveDialogService.open($scope.info.ProjectId, "Renewal_LegalApproval","Asset_Actor").then(function (approvers) {
                    $scope.legalApproval.AppUsers = approvers;
                    renewalService.submitLegalApproval({
                        Entity: $scope.legalApproval,
                        Info: $scope.info,
                        ProjectComment: $scope.projectComment
                    }).$promise.then(function () {
                        messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect("Renewal_LegalApproval", $scope.projectId);
                        });
                    }, function () {
                        messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
                        $scope.acting = false;
                    });
                }, function () {
                    $scope.acting = false;
                });
            };
            $scope.approve = function () {
                if ($scope.frmMain.$valid) {
                    $scope.acting = true;
                    if (!$scope.isGeneralCounsel) {
                        var errors = [];
                        if ($.grep($scope.attachments || [], function (att, i) {
                        return !!att.FileURL && att.RequirementId == "69e577c6-6dae-4d5c-ab62-b8c220cd04a8";
                        }) == 0) {
                            errors.push("[[[请上传Confirmed Renewal Contract]]]");
                        }
                        if (errors.length > 0) {
                            messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
                            $scope.acting = false;
                            return;
                        }

                        approveDialogService.open($scope.info.ProjectId, "Renewal_LegalApproval", "Legal").then(function (approvers) {
                            $scope.legalApproval.AppUsers = approvers;
                            messager.confirm("[[[确认审批通过吗]]]？", "fa-warning c_orange").then(function (result) {
                                if (result) {
                                    renewalService.approveLegalApproval({
                                        Entity: $scope.legalApproval,
                                        SN: $routeParams.SN,
                                        IsGeneralCounsel: $scope.isGeneralCounsel,
                                        ProjectComment: $scope.projectComment
                                    }).$promise.then(function () {
                                        messager.showMessage("[[[审批成功]]]", "fa-check c_green").then(function () {
                                            redirectService.flowRedirect("Renewal_LegalApproval", $scope.projectId);
                                        });
                                    }, function () {
                                        messager.showMessage("[[[审批成功]]]", "fa-warning c_orange");
                                        $scope.acting = false;
                                    });
                                } else {
                                    $scope.acting = false;
                                }
                            });
                        }, function () {
                            $scope.acting = false;
                        });
                    } else {
                        messager.confirm("[[[确认审批通过吗]]]？", "fa-warning c_orange").then(function (result) {
                            if (result) {
                                renewalService.approveLegalApproval({
                                    Entity: $scope.legalApproval,
                                    SN: $routeParams.SN,
                                    IsGeneralCounsel: $scope.isGeneralCounsel,
                                    ProjectComment: $scope.projectComment
                                }).$promise.then(function () {
                                    messager.showMessage("[[[审批成功]]]", "fa-check c_green").then(function () {
                                        redirectService.flowRedirect("Renewal_LegalApproval", $scope.projectId);
                                    });
                                }, function () {
                                    messager.showMessage("[[[审批成功]]]", "fa-warning c_orange");
                                    $scope.acting = false;
                                });
                            } else {
                                $scope.acting = false;
                            }
                        });
                    }                
                };
            };
            $scope.returnProcess = function () {
                $scope.acting = true;
                if (!$scope.projectComment) {
                    messager.showMessage("[[[请填写Comments]]]", "fa-warning c_orange");
                    $scope.acting = false;
                    return;
                }
                messager.confirm("[[[确认退回吗]]]？", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        renewalService.returnLegalApproval({
                            Entity: $scope.legalApproval,
                            SN: $routeParams.SN,
                            ProjectComment: $scope.projectComment
                        }).$promise.then(function () {
                            messager.showMessage("[[[退回成功]]]", "fa-check c_green").then(function () {
                                redirectService.flowRedirect("Renewal_LegalApproval", $scope.projectId);
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
            $scope.resubmit = function () {
                var errors = [];
                if ($.grep($scope.attachments || [], function (att, i) {
                return !!att.FileURL && att.RequirementId == "e26669ad-a029-4eb5-9fdc-97e1add0f10d";
                }) == 0) {
                    errors.push("[[[请上传Renewal Contract Draft]]]");
                }
                if (errors.length > 0) {
                    messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
                    $scope.acting = false;
                    return;
                }
                approveDialogService.open($scope.info.ProjectId, "Renewal_LegalApproval", "Asset_Actor").then(function (approvers) {
                    $scope.legalApproval.AppUsers = approvers;
                    renewalService.resubmitLegalApproval({
                        Entity: $scope.legalApproval,
                        Info: $scope.info,
                        SN: $routeParams.SN,
                        ProjectComment: $scope.projectComment
                    }).$promise.then(function () {
                        messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect("Renewal_LegalApproval", $scope.projectId);
                        });
                    }, function () {
                        messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
                        $scope.acting = false;
                    });
                }, function () {
                    $scope.acting = false;
                });
            };
            $scope.edit = function () {
                $scope.acting = true;
                messager.confirm("[[[Renewal Legal Approval 审批流程已经完成，现在编辑会导致流程重新提交，是否确认]]]？", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        renewalService.editLegalApproval($scope.legalApproval).$promise.then(function (editResult) {
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
                }, function () {
                    $scope.acting = false;
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
                            renewalService.recallLegalApproval({
                                Entity: $scope.legalApproval,
                                SN: $routeParams.SN,
                                ProjectComment: entity.Comment
                            }).$promise.then(function () {
                                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                                    redirectService.flowRedirect("Renewal_LegalApproval", $scope.projectId);
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