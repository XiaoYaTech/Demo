!function () {
    var ctrlModule = angular.module("mcd.am.tempClosure.controllers.legalReview", [
        "mcd.am.services.tempClosure",
        "mcd.am.services.projectUsers"
    ]);
    ctrlModule.controller("legalReviewController", [
        "$scope",
        "$routeParams",
        "$window",
        "$modal",
        "$location",
        "messager",
        "projectUsersService",
        "tempClosureService",
        "storeService",
        "redirectService",
        function ($scope, $routeParams, $window, $modal, $location, messager, projectUsersService, tempClosureService, storeService, redirectService) {
            $scope.projectId = $routeParams.projectId;
            $scope.isHistory = $routeParams.isHistory;
            $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);
            $scope.subFlowCode = "TempClosure_LegalReview";
            $scope.checkPointRefresh = true;
            tempClosureService.get({
                projectId: $scope.projectId
            }).$promise.then(function (data) {
                $scope.TPCLS = data;
            });
            tempClosureService.getLegalReviewInfo({
                projectId: $scope.projectId,
                Id: $routeParams.entityId
            }).$promise.then(function (data) {
                $scope.info = data.Info;
                $scope.legalReview = data.LegalReview;
                $scope.projectComment = data.ProjectComment;
                $scope.editable = data.Editable;
                $scope.recallable = data.Recallable;
                $scope.savable = data.Savable;
                if (data.Savable) {
                    $scope.viewUploadSet = ['ea0ed677-8cc1-4628-9a76-0462d4409cbe'];
                }
            });
            $scope.uploadAttachmentFinish = function (up, files) {
                $scope.checkPointRefresh = true;
            };
            $scope.deleteAttachmentFinish = function (id, requirementId) {
                $scope.checkPointRefresh = true;
            };
            $scope.saveLegalReview = function () {
                $scope.acting = true;
                tempClosureService.saveLegalReview({
                    Entity: $scope.legalReview,
                    ProjectComment: $scope.projectComment
                }).$promise.then(function () {
                    messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                    $scope.acting = false;
                }, function () {
                    messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
                    $scope.acting = false;
                });
            };
            $scope.submitLegalReview = function () {
                $scope.acting = true;
                //var errors = [];
                //if ($.grep($scope.legalReviewAttachments || [], function (att, i) {
                //    return !!att.FileURL && att.RequirementId == "ea0ed677-8cc1-4628-9a76-0462d4409cbe";
                //}) == 0) {
                //    errors.push("请上传Others");
                //}
                //if (errors.length > 0) {
                //    messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
                //    $scope.acting = false;
                //    return;
                //}
                $modal.open({
                    size: "md",
                    backdrop: "static",
                    templateUrl: Utils.ServiceURI.AppUri + "TempClosureModule/LegalSubmitPopup",
                    controller: ["$scope", "$modalInstance", function ($modelScope, $modalInstance) {
                        $modelScope.LegalNameENUS = $scope.info.LegalNameENUS;
                        $modelScope.legalList = [{ "NameENUS": $scope.info.LegalNameENUS }];
                        $modelScope.selLegal = $modelScope.legalList[0];
                        $modelScope.ok = function () {
                            $modalInstance.close();
                        };
                        $modelScope.cancel = function () {
                            $modalInstance.dismiss('');
                            $scope.acting = false;
                        };
                    }]
                }).result.then(function () {
                    tempClosureService.submitLegalReview({
                        Entity: $scope.legalReview,
                        ProjectComment: $scope.projectComment
                    }).$promise.then(function () {
                        messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                            //$window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
                            redirectService.flowRedirect($scope.subFlowCode, $scope.projectId);
                            messager.unBlockUI();
                        });
                    }, function () {
                        messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
                        $scope.acting = false;
                    });
                });
            };
            $scope.approveLegalReview = function () {
                $scope.acting = true;
                var errors = [];
                if (!$scope.legalReview.ReviewComment) {
                    errors.push("请填写Legal Review Comments");
                }
                if (errors.length > 0) {
                    messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
                    $scope.acting = false;
                    return;
                }
                messager.confirm("确定要进行审批吗？", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        messager.blockUI("正在处理中，请稍等...");
                        tempClosureService.approveLegalReview({
                            Entity: $scope.legalReview,
                            SN: $routeParams.SN,
                            ProjectComment: $scope.projectComment
                        }).$promise.then(function () {
                            messager.showMessage("[[[审批成功]]]", "fa-check c_green").then(function () {
                                //$window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
                                redirectService.flowRedirect($scope.subFlowCode, $scope.projectId);
                                messager.unBlockUI();
                            });
                        }, function () {
                            messager.showMessage("[[[审批成功]]]", "fa-warning c_orange");
                            messager.unBlockUI();
                            $scope.acting = false;
                        });
                    }
                    else {
                        $scope.acting = false;
                    }
                });
            };
            $scope.returnLegalReview = function () {
                $scope.acting = true;
                if (!$scope.projectComment) {
                    messager.showMessage("[[[请填写Comments]]]", "fa-warning c_orange");
                    $scope.acting = false;
                    return;
                }
                messager.confirm("[[[确定要进行退回吗？]]]", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        messager.blockUI("正在处理中，请稍等...");
                        tempClosureService.returnLegalReview({
                            Entity: $scope.legalReview,
                            SN: $routeParams.SN,
                            ProjectComment: $scope.projectComment
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
            $scope.resubmitLegalReview = function () {
                $scope.acting = true;
                $modal.open({
                    size: "md",
                    backdrop: "static",
                    templateUrl: Utils.ServiceURI.AppUri + "TempClosureModule/LegalSubmitPopup",
                    controller: ["$scope", "$modalInstance", function ($modelScope, $modalInstance) {
                        $modelScope.LegalNameENUS = $scope.info.LegalNameENUS;
                        $modelScope.legalList = [{ "NameENUS": $scope.info.LegalNameENUS }];
                        $modelScope.selLegal = $modelScope.legalList[0];
                        $modelScope.ok = function () {
                            $modalInstance.close();
                        };
                        $modelScope.cancel = function () {
                            $modalInstance.dismiss('');
                            $scope.acting = false;
                        };
                    }]
                }).result.then(function () {
                    tempClosureService.resubmitLegalReview({
                        Entity: $scope.legalReview,
                        SN: $routeParams.SN,
                        ProjectComment: $scope.projectComment
                    }).$promise.then(function () {
                        messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                            //$window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
                            redirectService.flowRedirect($scope.subFlowCode, $scope.projectId);
                            messager.unBlockUI();
                        });
                    }, function () {
                        messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
                        $scope.acting = false;
                    });
                });
            };
            $scope.editLegalReview = function () {
                $scope.acting = true;
                messager.confirm("[[[Legal Review 审批流程已经完成，现在编辑会导致流程重新提交，是否确认？]]]", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        tempClosureService.editLegalReview($scope.legalReview).$promise.then(function (editResult) {
                            messager.showMessage("[[[操作成功]]]", "fa-check c_green").then(function () {
                                //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                                messager.unBlockUI();
                                $window.location.href = Utils.ServiceURI.WebAddress() + editResult.TaskUrl;
                            });
                        }, function () {
                            messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                            $scope.acting = false;
                        });
                    }
                    else
                        $scope.acting = false;
                });
            };
            $scope.recallLegalReview = function () {
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
                                    $scope.acting = false;
                                };
                            }]
                        }).result.then(function (entity) {
                            tempClosureService.recallLegalReview({
                                Entity: $scope.legalReview,
                                SN: $routeParams.SN,
                                ProjectComment: entity.Comment
                            }).$promise.then(function () {
                                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                                    //$window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
                                    redirectService.flowRedirect($scope.subFlowCode, $scope.projectId);
                                    messager.unBlockUI();
                                });
                            }, function () {
                                messager.showMessage("[[[撤回失败]]]", "fa-warning c_orange");
                                $scope.acting = false;
                            });
                        });
                    }
                    else
                        $scope.acting = false;
                });
            };
        }
    ]);
}();