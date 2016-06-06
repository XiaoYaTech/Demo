!function () {
    var RenewalApp = angular.module("amApp");
    RenewalApp.controller("lLNegotiationController", [
        "$scope",
        "$routeParams",
        "$window",
        "$modal",
        "renewalService",
        "attachmentService",
        "messager",
        "redirectService",
        function ($scope, $routeParams, $window, $modal, renewalService, attachmentService, messager, redirectService) {
            $scope.projectId = $routeParams.projectId;
            $scope.from = $routeParams.from;
            $scope.exportRecordsLink = Utils.ServiceURI.Address() + "api/renewalLLNegotiation/exportRecords?projectId=" + $scope.projectId;
            $scope.checkPointRefresh = true;
            $scope.pageSize = 10;
            messager.blockUI("[[[正在初始化页面，请稍等]]]...");
            renewalService.initLLNegotiationPage({
                projectId: $scope.projectId,
                id: $routeParams.entityId
            }).$promise.then(function (data) {
                messager.unBlockUI();
                $scope.lLNegotiation = data.LLNegotiation;
                $scope.editable = data.Editable;
                $scope.recallable = data.Recallable;
                $scope.savable = data.Savable;
                $scope.USCode = data.USCode;
                $scope.pageIndex = 1;
            }, function () {
                messager.unBlockUI();
                messager.showMessage("[[[页面初始化出错]]]", "fa-warning c_orange").then(function () {
                    $window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
                });
            });
            $scope.loadRecords = function (callback) {
                renewalService.getLLNegoRecords({
                    negotiationId: $scope.lLNegotiation.Id,
                    pageIndex: $scope.pageIndex,
                    pageSize: $scope.pageSize
                }).$promise.then(function (data) {
                    $scope.records = data.List;
                    $scope.totalItems = data.TotalItems;
                    callback && callback();
                });
            }
            $scope.$watch("pageIndex", function (val) {
                if (!!val) {
                    $scope.loadRecords();
                }
            })
            $scope.createLLNegoRecord = function () {
                var nRecord = {
                    Id: Utils.Generator.newGuid(),
                    RenewalLLNegotiationId: $scope.lLNegotiation.Id,
                    Date: new Date()
                };
                $modal.open({
                    backdrop: "static",
                    size: "lg",
                    templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/NegotiationRecord",
                    resolve: {
                        model: function () {
                            return angular.copy({
                                record: nRecord,
                                showDate: false
                            });
                        }
                    },
                    controller: [
                        "$scope",
                        "$parse",
                        "$modalInstance",
                        "model",
                        function ($modalScope,$parse, $modalInstance, model) {
                            $modalScope.model = model;
                            $modalScope.openDate = function ($event, tag) {
                                $event.preventDefault();
                                $event.stopPropagation();
                                $modalScope.model[tag] = true;
                            };
                            $modalScope.deleteAttachment = function (id) {
                                messager.confirm("[[[确认要删除？]]]", "fa-warning c_orange").then(function (result) {
                                    if (result) {
                                        attachmentService.deleteAttachment({
                                            ProjectId: $scope.projectId,
                                            Id: id
                                        }).$promise.then(function () {
                                            messager.showMessage("[[[删除附件成功]]]", "fa-check c_green");
                                            $modalScope.loadAttachments();
                                        }, function () {
                                            messager.showMessage("[[[删除附件失败]]]", "fa-warning c_red");
                                        });
                                    }
                                });
                            };

                            $modalScope.loadAttachments = function () {
                                renewalService.getLLNegoRecAttachments({
                                    recordId: $modalScope.model.record.Id
                                }).$promise.then(function (data) {
                                    $.each(data, function (i, att) {
                                        att.downloadLink = Utils.ServiceURI.Address() + "api/attachment/download?id=" + att.ID;
                                    });
                                    $modalScope.attachments = data;
                                });
                            };
                            $modalScope.save = function (frm) {
                                if (frm.$valid) {
                                    $modalInstance.close($modalScope.model.record);
                                }
                            };
                            $modalScope.cancel = function () {
                                $modalInstance.dismiss('');
                            };
                            $modalScope.loadAttachments();
                        }
                    ]
                }).result.then(function (record) {
                    renewalService.saveLLNegoRecord(record).$promise.then(function () {
                        messager.showMessage("[[[保存成功]]]", "fa-check c_green").then(function () {
                            $scope.loadRecords();
                            $scope.checkPointRefresh = true;
                        });
                    }, function () {
                        messager.showMessage("[[[创建失败]]]", "fa-warning c_orange");
                    });
                });
            };
            $scope.deleteLLNegoRecord = function (record) {
                messager.confirm("[[[确认删除这条记录]]]？", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        renewalService.deleteLLNegoRecord(record).$promise.then(function () {
                            messager.showMessage("[[[删除Record成功]]]", "fa-check c_green").then(function () {
                                $scope.loadRecords();
                            });
                        }, function () {
                            messager.showMessage("[[[删除Record失败]]]", "fa-warning c_orange");
                        });
                    }
                });
                
            };
            $scope.editLLNegoRecord = function (record) {
                $modal.open({
                    backdrop: "static",
                    size: "lg",
                    templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/NegotiationRecord",
                    resolve: {
                        model: function () {
                            return {
                                record: angular.copy(record)
                            };
                        }
                    },
                    controller: [
                        "$scope",
                        "$parse",
                        "$modalInstance",
                        "model",
                        function ($modalScope, $parse, $modalInstance, model) {
                            $modalScope.model = model;
                            $modalScope.openDate = function ($event, tag) {
                                $event.preventDefault();
                                $event.stopPropagation();
                                $modalScope.model[tag] = true;
                            };
                            $modalScope.deleteAttachment = function (id) {
                                messager.confirm("[[[确认要删除？]]]", "fa-warning c_orange").then(function (result) {
                                    if (result) {
                                        attachmentService.deleteAttachment({
                                            ProjectId: $scope.projectId,
                                            Id: id
                                        }).$promise.then(function () {
                                            messager.showMessage("[[[删除附件成功]]]", "fa-check c_green");
                                            $modalScope.loadAttachments();
                                        }, function () {
                                            messager.showMessage("[[[删除附件失败]]]", "fa-warning c_red");
                                        });
                                    }
                                });
                            };

                            $modalScope.loadAttachments = function () {
                                renewalService.getLLNegoRecAttachments({
                                    recordId: $modalScope.model.record.Id
                                }).$promise.then(function (data) {
                                    $.each(data, function (i, att) {
                                        att.downloadLink = Utils.ServiceURI.Address() + "api/attachment/download?id=" + att.ID;
                                    });
                                    $modalScope.attachments = data;
                                });
                            };
                            $modalScope.save = function (frm) {
                                if (frm.$valid) {
                                    $modalInstance.close($modalScope.model.record);
                                }
                            };
                            $modalScope.cancel = function () {
                                $modalInstance.dismiss('');
                            };
                            $modalScope.loadAttachments();
                        }
                    ]
                }).result.then(function (record) {
                    renewalService.saveLLNegoRecord(record).$promise.then(function () {
                        messager.showMessage("[[[保存成功]]]", "fa-check c_green").then(function () {
                            $scope.loadRecords();
                        });

                    }, function () {
                        messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
                    });
                });
            };
            $scope.showRecord = function (record,hideDetail) {
                $modal.open({
                    backdrop: "static",
                    size: "lg",
                    templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/NegotiationRecord",
                    resolve: {
                        model: function () {
                            return {
                                record: angular.copy(record)
                            };
                        }
                    },
                    controller: [
                        "$scope",
                        "$parse",
                        "$modalInstance",
                        "model",
                        function ($modalScope, $parse, $modalInstance, model) {
                            $modalScope.model = model;
                            $modalScope.hideDetail = hideDetail;
                            $modalScope.viewmode = true;
                            $modalScope.loadAttachments = function () {
                                renewalService.getLLNegoRecAttachments({
                                    recordId: $modalScope.model.record.Id
                                }).$promise.then(function (data) {
                                    $.each(data, function (i, att) {
                                        att.downloadLink = Utils.ServiceURI.Address() + "api/attachment/download?id=" + att.ID;
                                    });
                                    $modalScope.attachments = data;
                                });
                            }
                            $modalScope.cancel = function () {
                                $modalInstance.dismiss('');
                            };
                            $modalScope.loadAttachments();
                        }
                    ]
                });
            };
            $scope.saveLLNegotiation = function () {
                $scope.acting = true;
                renewalService.saveLLNegotiation($scope.lLNegotiation).$promise.then(function (data) {
                    messager.showMessage("[[[保存成功]]]", "fa-check c_green").then(function () {
                        $scope.acting = false;
                    });
                });
            };
            $scope.submitLLNegotiation = function () {
                $scope.acting = true;
                renewalService.submitLLNegotiation($scope.lLNegotiation).$promise.then(function (data) {
                });
            };
            $scope.resubmit = function (frm) {
                if (frm.$valid) {
                    $scope.acting = true;
                    if ($.grep($scope.attachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "4c3cdbbb-45ac-4f28-937f-2dd6b3aeefcd";
                    }) == 0) {
                        messager.showMessage("[[[请上传Confirm Letter]]]", "fa-warning c_orange");
                        $scope.acting = false;
                        return;
                    }
                    renewalService.resubmitLLNegotiation({
                        Entity: $scope.lLNegotiation,
                        Info: $scope.info,
                        SN: $routeParams.SN,
                        ProjectComment: ""
                    }).$promise.then(function () {
                        messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect("Renewal_LLNegotiation", $scope.projectId);
                        });
                    }, function () {
                        messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
                        $scope.acting = false;
                    });
                };
            };
            $scope.edit = function () {
                $scope.acting = true;
                messager.confirm("[[[Renewal LLNegotiation 审批流程已经完成，现在编辑会导致流程重新提交，是否确认]]]？", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        renewalService.editLLNegotiation($scope.lLNegotiation).$promise.then(function (editResult) {
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
                            renewalService.recallLLNegotiation({
                                Entity: $scope.lLNegotiation,
                                Info: $scope.info,
                                SN: $routeParams.SN,
                                ProjectComment: entity.Comment
                            }).$promise.then(function () {
                                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                                    redirectService.flowRedirect("Renewal_LLNegotiation", $scope.projectId);
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