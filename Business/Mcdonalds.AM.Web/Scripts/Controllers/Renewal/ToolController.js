!function () {
    var RenewalApp = angular.module("amApp");
    RenewalApp.controller("toolController", [
        "$scope",
        "$routeParams",
        "$window",
        "$location",
        "$modal",
        "renewalService",
        "messager",
        "approveDialogService",
        "redirectService",
        function ($scope, $routeParams, $window, $location, $modal, renewalService, messager, approveDialogService, redirectService) {
            $scope.projectId = $routeParams.projectId;
            $scope.isHistory = $routeParams.isHistory;
            $scope.from = $routeParams.from;
            $scope.checkPointRefresh = true;
            $scope.templateUrl = Utils.ServiceURI.Address() + "api/renewalTool/downloadToolTemplate?projectId=" + $scope.projectId;
            var init = function () {
                messager.blockUI("[[[正在初始化页面，请稍等]]]...");
                renewalService.initToolPage({
                    projectId: $scope.projectId,
                    id: $routeParams.entityId
                }).$promise.then(function (data) {
                    messager.unBlockUI();
                    $scope.tool = data.Entity;
                    $scope.info = data.Info;
                    $scope.projectComment = data.ProjectComment;
                    $scope.finMeasureInput = data.FinMeasureInput;
                    var writeOffAndReinCost = data.WriteOffAndReinCost;
                    if (!writeOffAndReinCost.REWriteOff)
                        writeOffAndReinCost.REWriteOff = 0;
                    if (!writeOffAndReinCost.LHIWriteOff)
                        writeOffAndReinCost.LHIWriteOff = 0;
                    if (!writeOffAndReinCost.ESSDWriteOff)
                        writeOffAndReinCost.ESSDWriteOff = 0;
                    if (!writeOffAndReinCost.RECost)
                        writeOffAndReinCost.RECost = 0;
                    if (!writeOffAndReinCost.LHICost)
                        writeOffAndReinCost.LHICost = 0;
                    if (!writeOffAndReinCost.ESSDCost)
                        writeOffAndReinCost.ESSDCost = 0;
                    $scope.writeOffAndReinCost = writeOffAndReinCost;
                    $scope.yearMonths = data.TTMDataYearMonths;
                    $scope.yearMonth = data.SelectedYearMonth;
                    $scope.editable = data.Editable;
                    $scope.recallable = data.Recallable;
                    $scope.uploadable = data.Uploadable;
                    $scope.savable = data.Savable;
                    $scope.isFinished = data.IsFinished;
                    $scope.renewalYears = Math.ceil($scope.info.RenewalYears);
                    if (data.Savable) {
                        $scope.viewUploadSet = ['016ea906-733b-4803-b30a-d2c3f6c252af'];
                    }
                    $scope.dataLoaded = true;
                    $scope.$broadcast("loadTTMFinanceData", data.FinMeasureInput);
                    $scope.$broadcast("loadFinMeasureOutput", data.Entity.Id);
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
            $scope.downloadTpl = function (frm) {
                if (frm.$valid) {
                    return renewalService.saveTool({
                        Entity: $scope.tool,
                        Info: $scope.info,
                        FinMeasureInput: $scope.finMeasureInput,
                        WriteOffAndReinCost: $scope.writeOffAndReinCost,
                        ProjectComment: $scope.projectComment
                    }).$promise.then(function () {
                        $("#frmDownload").attr("src", "about:tabs").delay(100).attr("src", $scope.templateUrl);
                    }, function () {
                        messager.showMessage("[[[下载模板失败，请稍后重试]]]", "fa-warning c_orange");
                    });

                }
                return frm.$valid;
            };
            $scope.save = function () {
                $scope.acting = true;
                renewalService.saveTool({
                    Entity: $scope.tool,
                    Info: $scope.info,
                    FinMeasureInput: $scope.finMeasureInput,
                    WriteOffAndReinCost: $scope.writeOffAndReinCost,
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
                    if ($.grep($scope.attachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "016ea906-733b-4803-b30a-d2c3f6c252af";
                    }) == 0) {
                        errors.push("[[[请上传Main Financial Index]]]");
                    }
                    if (errors.length > 0) {
                        messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
                        $scope.acting = false;
                        return;
                    }
                    approveDialogService.open($scope.info.ProjectId, "Renewal_Tool").then(function (approvers) {
                        $scope.tool.AppUsers = approvers;
                        renewalService.submitTool({
                            Entity: $scope.tool,
                            Info: $scope.info,
                            FinMeasureInput: $scope.finMeasureInput,
                            WriteOffAndReinCost: $scope.writeOffAndReinCost,
                            ProjectComment: $scope.projectComment
                        }).$promise.then(function () {
                            messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                                redirectService.flowRedirect("Renewal_Tool", $scope.projectId);
                            });
                        }, function () {
                            messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
                            $scope.acting = false;
                        });
                    }, function () {
                        $scope.acting = false;
                    });
                }
            };
            $scope.approve = function (frm) {
                if (frm.$valid) {
                    $scope.acting = true;
                    messager.confirm("[[[确认审批通过吗]]]？", "fa-warning c_orange").then(function (result) {
                        if (result) {
                            renewalService.approveTool({
                                Entity: $scope.tool,
                                SN: $routeParams.SN,
                                ProjectComment: $scope.projectComment
                            }).$promise.then(function () {
                                messager.showMessage("[[[审批成功]]]", "fa-check c_green").then(function () {
                                    redirectService.flowRedirect("Renewal_Tool", $scope.projectId);
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
            $scope.returnProcess = function () {
                $scope.acting = true;
                if (!$scope.projectComment) {
                    messager.showMessage("[[[请填写Comments]]]", "fa-warning c_orange");
                    $scope.acting = false;
                    return;
                }
                messager.confirm("[[[确认退回吗]]]？", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        renewalService.returnTool({
                            Entity: $scope.tool,
                            SN: $routeParams.SN,
                            ProjectComment: $scope.projectComment
                        }).$promise.then(function () {
                            messager.showMessage("[[[退回成功]]]", "fa-check c_green").then(function () {
                                redirectService.flowRedirect("Renewal_Tool", $scope.projectId);
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
                    $scope.acting = true;
                    var errors = [];
                    if ($.grep($scope.attachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "016ea906-733b-4803-b30a-d2c3f6c252af";
                    }) == 0) {
                        errors.push("[[[请上传Main Financial Index]]]");
                    }
                    if (errors.length > 0) {
                        messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
                        $scope.acting = false;
                        return;
                    }
                    approveDialogService.open($scope.info.ProjectId, "Renewal_Tool").then(function (approvers) {
                        $scope.tool.AppUsers = approvers;
                        renewalService.resubmitTool({
                            Entity: $scope.tool,
                            Info: $scope.info,
                            FinMeasureInput: $scope.finMeasureInput,
                            WriteOffAndReinCost: $scope.writeOffAndReinCost,
                            SN: $routeParams.SN,
                            ProjectComment: $scope.projectComment
                        }).$promise.then(function () {
                            messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                                redirectService.flowRedirect("Renewal_Tool", $scope.projectId);
                            });
                        }, function () {
                            messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
                            $scope.acting = false;
                        });
                    }, function () {
                        $scope.acting = false;
                    });
                }
            };
            $scope.edit = function () {
                $scope.acting = true;
                messager.confirm("[[[Renewal Tool 审批流程已经完成，现在编辑会导致流程重新提交，是否确认]]]？", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        renewalService.editTool($scope.tool).$promise.then(function (editResult) {
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
                            renewalService.recallTool({
                                Entity: $scope.tool,
                                SN: $routeParams.SN,
                                ProjectComment: entity.Comment
                            }).$promise.then(function () {
                                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                                    redirectService.flowRedirect("Renewal_Tool", $scope.projectId);
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
            $scope.uploadToolFinish = function () {
                messager.showMessage("[[[上传成功]]]", "fa-check c_green").then(function () {
                    $scope.$broadcast("AttachmentUploadFinish");
                    $scope.$broadcast("loadFinMeasureOutput", $scope.tool.Id);
                    //renewalService.confirmUploadTool({
                    //    Entity: $scope.tool,
                    //    SN: $routeParams.SN,
                    //    ProjectComment: $scope.projectComment
                    //}).$promise.then(function () {
                    //    redirectService.flowRedirect("Renewal_Tool", $scope.projectId);
                    //}, function () {
                    //});
                });
            }

            $scope.confirm = function () {
                if ($scope.tool
                    && $scope.tool.ComSalesDesc) {
                    renewalService.confirmUploadTool({
                        Entity: $scope.tool,
                        SN: $routeParams.SN,
                        ProjectComment: $scope.projectComment
                    }).$promise.then(function () {
                        redirectService.flowRedirect("Renewal_Tool", $scope.projectId);
                    }, function () {
                    });
                } else {
                    messager.showMessage("[[[请先填写Comments & Conclusion！]]]", "fa-warning c_orange");
                }

            }
        }
    ]);
}();