!function () {
    var RenewalApp = angular.module("amApp");
    RenewalApp.controller("confirmLetterController", [
        "$scope",
        "$routeParams",
        "$window",
        "$modal",
        "renewalService",
        "messager",
        "redirectService",
        function ($scope, $routeParams, $window, $modal, renewalService, messager, redirectService) {
            $scope.projectId = $routeParams.projectId;
            $scope.from = $routeParams.from;
            $scope.checkPointRefresh = true;
            messager.blockUI("[[[正在初始化页面，请稍等]]]...");
            renewalService.initConfirmLetterPage({
                projectId: $scope.projectId,
                id: $routeParams.entityId
            }).$promise.then(function (data) {
                messager.unBlockUI();
                $scope.confirmLetter = data.Entity;
                $scope.info = data.Info;
                $scope.editable = data.Editable;
                if (!!$routeParams.entityId)
                    $scope.editable = false;
                $scope.recallable = data.Recallable;
                $scope.savable = data.Savable;
                if (data.Savable) {
                    $scope.viewUploadSet = ['4c3cdbbb-45ac-4f28-937f-2dd6b3aeefcd'];
                }
            }, function () {
                messager.unBlockUI();
                messager.showMessage("[[[页面初始化出错]]]", "fa-warning c_orange").then(function () {
                    $window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
                });
            });
            $scope.save = function () {
                $scope.acting = true;
                renewalService.saveConfirmLetter($scope.confirmLetter).$promise.then(function (data) {
                    messager.showMessage("[[[保存成功]]]", "fa-check c_green").then(function () {
                        $scope.acting = false;
                    });
                }, function () {
                    messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
                    $scope.acting = false;
                });
            };
            $scope.submit = function () {
                $scope.acting = true;
                if ($.grep($scope.attachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "4c3cdbbb-45ac-4f28-937f-2dd6b3aeefcd";
                }) == 0) {
                    messager.showMessage("[[[请上传Confirm Letter]]]", "fa-warning c_orange");
                    $scope.acting = false;
                    return;
                }
                renewalService.submitConfirmLetter($scope.confirmLetter).$promise.then(function (data) {
                    messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                        redirectService.flowRedirect("Renewal_ConfirmLetter", $scope.projectId);
                    });
                }, function () {
                    messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
                    $scope.acting = false;
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
                    renewalService.resubmitConfirmLetter({
                        Entity: $scope.confirmLetter,
                        Info: $scope.info,
                        SN: $routeParams.SN,
                        ProjectComment: ""
                    }).$promise.then(function () {
                        messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect("Renewal_ConfirmLetter", $scope.projectId);
                        });
                    }, function () {
                        messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
                        $scope.acting = false;
                    });
                };
            };
            $scope.edit = function () {
                $scope.acting = true;
                messager.confirm("[[[Renewal Confirm Letter 审批流程已经完成，现在编辑会导致流程重新提交，是否确认]]]？", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        renewalService.editConfirmLetter($scope.confirmLetter).$promise.then(function (editResult) {
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
                            renewalService.recallConfirmLetter({
                                Entity: $scope.confirmLetter,
                                Info: $scope.info,
                                SN: $routeParams.SN,
                                ProjectComment: entity.Comment
                            }).$promise.then(function () {
                                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                                    redirectService.flowRedirect("Renewal_ConfirmLetter", $scope.projectId);
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