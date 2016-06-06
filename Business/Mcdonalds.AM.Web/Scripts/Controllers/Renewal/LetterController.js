!function () {
    var RenewalApp = angular.module("amApp");
    RenewalApp.controller("letterController", [
        "$scope",
        "$routeParams",
        "$window",
        "$location",
        "$modal",
        "renewalService",
        "approveDialogService",
        "messager",
        "redirectService",
        function ($scope, $routeParams, $window, $location, $modal, renewalService, approveDialogService, messager, redirectService) {
            $scope.projectId = $routeParams.projectId;
            $scope.from = $routeParams.from;
            $scope.isHistory = $routeParams.isHistory;
            $scope.checkPointRefresh = true;
            messager.blockUI("[[[正在初始化页面，请稍等]]]...");
            renewalService.initLetterPage({
                projectId: $scope.projectId,
                id: $routeParams.entityId
            }).$promise.then(function (data) {
                messager.unBlockUI();
                $scope.letter = data.Letter;
                $scope.info = data.Info;
                $scope.projectComment = data.ProjectComment;
                $scope.editable = data.Editable;
                $scope.recallable = data.Recallable;
                $scope.savable = data.Savable;
                if (data.Savable) {
                    $scope.viewUploadSet = ['90f74245-171e-4df2-b3a7-e18272caa0ba'];
                }
            }, function () {
                messager.unBlockUI();
                messager.showMessage("[[[页面初始化出错]]]", "fa-warning c_orange").then(function () {
                    $window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
                });
            });
            $scope.save = function () {
                $scope.acting = true;
                renewalService.saveLetter({
                    Entity: $scope.letter,
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
                if ($.grep($scope.attachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "90f74245-171e-4df2-b3a7-e18272caa0ba";
                }) == 0) {
                    messager.showMessage("[[[请上传Rnewal Letter]]]", "fa-warning c_orange");
                    $scope.acting = false;
                    return;
                }
                approveDialogService.open($scope.letter.ProjectId, "Renewal_Letter").then(function (approvers) {
                    $scope.letter.AppUsers = approvers;
                    renewalService.submitLetter({
                        Entity: $scope.letter,
                        ProjectComment: $scope.projectComment
                    }).$promise.then(function () {
                        messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect("Renewal_Letter", $scope.projectId);
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
                        renewalService.approveLetter({
                            Entity: $scope.letter,
                            SN: $routeParams.SN,
                            ProjectComment: $scope.projectComment
                        }).$promise.then(function () {
                            messager.showMessage("[[[审批成功]]]", "fa-check c_green").then(function () {
                                redirectService.flowRedirect("Renewal_Letter", $scope.projectId);
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
                }
                messager.confirm("[[[确认退回吗]]]？", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        renewalService.returnLetter({
                            Entity: $scope.letter,
                            SN: $routeParams.SN,
                            ProjectComment: $scope.projectComment
                        }).$promise.then(function () {
                            messager.showMessage("[[[退回成功]]]", "fa-check c_green").then(function () {
                                redirectService.flowRedirect("Renewal_Letter", $scope.projectId);
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
                if ($.grep($scope.attachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "90f74245-171e-4df2-b3a7-e18272caa0ba";
                }) == 0) {
                    messager.showMessage("[[[请上传Rnewal Letter]]]", "fa-warning c_orange");
                    $scope.acting = false;
                    return;
                }
                approveDialogService.open($scope.letter.ProjectId, "Renewal_Letter").then(function (approvers) {
                    $scope.letter.AppUsers = approvers;
                    renewalService.resubmitLetter({
                        Entity: $scope.letter,
                        SN: $routeParams.SN,
                        ProjectComment: $scope.projectComment
                    }).$promise.then(function () {
                        messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect("Renewal_Letter", $scope.projectId);
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
                messager.confirm("[[[Renewal Letter 审批流程已经完成，现在编辑会导致流程重新提交，是否确认]]]？", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        renewalService.editLetter($scope.letter).$promise.then(function (editResult) {
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
                            renewalService.recallLetter({
                                Entity: $scope.letter,
                                SN: $routeParams.SN,
                                ProjectComment: entity.Comment
                            }).$promise.then(function () {
                                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                                    redirectService.flowRedirect("Renewal_Letter", $scope.projectId);
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