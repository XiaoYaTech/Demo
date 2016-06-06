!function () {
    var RenewalApp = angular.module("amApp");
    RenewalApp.controller("packageController", [
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
            $scope.viewUploadSet = [];
            messager.blockUI("[[[正在初始化页面，请稍等]]]...");
            var init = function () {
                renewalService.initPackagePage({
                    projectId: $scope.projectId,
                    id: $routeParams.entityId
                }).$promise.then(function (data) {
                    messager.unBlockUI();
                    $scope.package = data.Entity;
                    $scope.info = data.Info;
                    $scope.analysis = data.Analysis;
                    $scope.finMeasureOutput = data.FinMeasureOutput;
                    $scope.editable = data.Editable;
                    $scope.recallable = data.Recallable;
                    $scope.rejectable = data.Rejectable;
                    $scope.savable = data.Savable;
                    $scope.isLindaLu = data.IsLindaLu;
                    $scope.projectComment = data.ProjectComment;
                    if (data.Savable) {
                        $scope.viewUploadSet.push(['d7348acc-6e63-4194-b0a9-18eeff3d7bfc', '4d8002c0-d5f1-4bb5-b756-7fe9dc25a019', '95063830-6282-467c-a2fa-2cfea5e62c5b']);
                    }
                    if (data.IsLindaLu) {
                        $scope.viewUploadSet.push(['fdb19f00-496b-4e16-9a87-05cfd65ae7f3']);
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
                renewalService.savePackage({
                    Entity: $scope.package,
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
                $scope.acting = true;
                //if ($.grep($scope.attachments || [], function (att, i) {
                //    return !!att.FileURL && att.RequirementId == "4d8002c0-d5f1-4bb5-b756-7fe9dc25a019";
                //}) == 0) {
                //    messager.showMessage("请上传Signed Renewal Contract", "fa-warning c_red");
                //    $scope.acting = false;
                //    return;
                //}
                approveDialogService.open($scope.projectId, "Renewal_Package", null, $scope.info.USCode, $scope.analysis).then(function (approvers) {
                    $scope.package.AppUsers = approvers;
                    renewalService.submitPackage({
                        Entity: $scope.package,
                        ProjectComment: $scope.projectComment
                    }).$promise.then(function () {
                        messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect("Renewal_Package", $scope.projectId);
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
                $scope.acting = true;
                messager.confirm("[[[确认审批通过吗]]]？", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        renewalService.approvePackage({
                            Entity: $scope.package,
                            SN: $routeParams.SN,
                            ProjectComment: $scope.projectComment
                        }).$promise.then(function () {
                            messager.showMessage("[[[审批成功]]]", "fa-check c_green").then(function () {
                                redirectService.flowRedirect("Renewal_Package", $scope.projectId);
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
                    messager.showMessage("[[[请填写Comments]]]", "fa-warning c_orange").then(function () {
                        window.parent.scrollTo(0, $("#divComments textarea").offset().top);
                    });
                    
                    $scope.acting = false;
                    return;
                }
                messager.confirm("[[[确认退回吗]]]？", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        renewalService.returnPackage({
                            Entity: $scope.package,
                            SN: $routeParams.SN,
                            ProjectComment: $scope.projectComment
                        }).$promise.then(function () {
                            messager.showMessage("[[[退回成功]]]", "fa-check c_green").then(function () {
                                redirectService.flowRedirect("Renewal_Package", $scope.projectId);
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
                $scope.acting = true;
                approveDialogService.open($scope.projectId, "Renewal_Package", null, $scope.info.USCode, $scope.analysis).then(function (approvers) {
                    $scope.package.AppUsers = approvers;
                    renewalService.resubmitPackage({
                        Entity: $scope.package,
                        SN: $routeParams.SN,
                        ProjectComment: $scope.projectComment
                    }).$promise.then(function () {
                        messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect("Renewal_Package", $scope.projectId);
                        });
                    }, function () {
                        messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
                        $scope.acting = false;
                    });
                }, function () {
                    $scope.acting = false;
                });
            };
            $scope.reject = function () {
                $scope.acting = true;
                if (!$scope.projectComment) {
                    messager.showMessage("[[[请填写Comments]]]", "fa-warning c_orange").then(function () {
                        window.parent.scrollTo(0, $("#divComments textarea").offset().top);
                    });
                    $scope.acting = false;
                    return;
                }
                messager.confirm("[[[Reject后该流程将直接终止，请确认需要执行该操作吗]]]？", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        renewalService.rejectPackage({
                            Entity: $scope.package,
                            SN: $routeParams.SN,
                            ProjectComment: $scope.projectComment
                        }).$promise.then(function () {
                            messager.showMessage("[[[拒绝成功]]]", "fa-check c_green").then(function () {
                                redirectService.flowRedirect("Renewal_Package", $scope.projectId);
                            });
                        }, function () {
                            messager.showMessage("[[[拒绝失败]]]", "fa-warning c_orange");
                            $scope.acting = false;
                        });
                    } else {
                        $scope.acting = false;
                    }
                });
                
            }
            $scope.edit = function () {
                $scope.acting = true;
                messager.confirm("[[[Renewal Package 审批流程已经完成，现在编辑会导致流程重新提交，是否确认]]]？", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        renewalService.editPackage($scope.package).$promise.then(function (editResult) {
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
                            renewalService.recallPackage({
                                Entity: $scope.package,
                                SN: $routeParams.SN,
                                ProjectComment: entity.Comment
                            }).$promise.then(function () {
                                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                                    redirectService.flowRedirect("Renewal_Package", $scope.projectId);
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

            $scope.confirm = function () {
                $scope.acting = true;
                if ($.grep($scope.attachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "fdb19f00-496b-4e16-9a87-05cfd65ae7f3";
                }) == 0) {
                    messager.showMessage("[[[请上传Signed Package]]]", "fa-warning c_red");
                    $scope.acting = false;
                    return;
                }
                renewalService.confirmPackage({
                    Entity: $scope.package,
                    SN: $routeParams.SN,
                    ProjectComment: $scope.projectComment
                }).$promise.then(function (editResult) {
                    messager.showMessage("[[[操作成功]]]", "fa-check c_green").then(function () {
                        redirectService.flowRedirect("Renewal_Package", $scope.projectId);
                    });
                }, function () {
                    messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                    $scope.acting = false;
                });
            };
        }
    ]);
}();