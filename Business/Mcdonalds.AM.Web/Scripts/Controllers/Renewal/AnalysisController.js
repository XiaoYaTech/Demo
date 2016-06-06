!function () {
    var RenewalApp = angular.module("amApp");
    RenewalApp.controller("analysisController", [
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
            $scope.templateUrl = Utils.ServiceURI.Address() + "api/renewalAnalysis/downloadTemplate/" + $scope.projectId;
            messager.blockUI("[[[正在初始化页面，请稍等]]]...");
            renewalService.initAnalysisPage({
                projectId: $scope.projectId,
                id: $routeParams.entityId
            }).$promise.then(function (data) {
                messager.unBlockUI();
                $scope.analysis = data.Entity;
                $scope.analysis.BETypes = !!data.Entity.BEType ? data.Entity.BEType.split(",") : [];
                $scope.analysis.MajorGeneratorArray = !!data.Entity.MajorGenerators ? data.Entity.MajorGenerators.split(",") : [];
                $scope.analysis.MajorGeneratorArrayNew = !!data.Entity.MajorGeneratorsNew ? data.Entity.MajorGeneratorsNew.split(",") : [];
                $scope.info = data.Info;
                $scope.storeInfo = data.StoreInfo;
                $scope.hasReinvenstment = data.HasReinvenstment;
                $scope.editable = data.Editable;
                $scope.recallable = data.Recallable;
                $scope.savable = data.Savable; 
            }, function () {
                messager.unBlockUI();
                messager.confirm("[[[页面初始化出错,点击确定重新加载]]]？", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        $location.url($location.url(), true);
                    } else {
                        $window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
                    }
                });
            });
            $scope.save = function () {
                $scope.acting = true;
                $scope.analysis.BEType = $scope.analysis.BETypes.join(",");
                $scope.analysis.MajorGenerators = $scope.analysis.MajorGeneratorArray.join(",");
                $scope.analysis.MajorGeneratorsNew = $scope.analysis.MajorGeneratorArrayNew.join(",");
                $scope.analysis.AnnualSOILastTY = $scope.storeInfo.AnnualSOILastTY;
                $scope.analysis.CashROILastTY = $scope.storeInfo.CashROILastTY;
                renewalService.saveAnalysis($scope.analysis).$promise.then(function (data) {
                    messager.showMessage("[[[保存成功]]]", "fa-check c_green").then(function () {
                        $scope.acting = false;
                    });
                }, function () {
                    messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
                    $scope.acting = false;
                });
            };
            $scope.submit = function (frm) {
                if (frm.$valid) {
                    $scope.acting = true;
                    $scope.analysis.BEType = $scope.analysis.BETypes.join(",");
                    $scope.analysis.MajorGenerators = $scope.analysis.MajorGeneratorArray.join(",");
                    $scope.analysis.MajorGeneratorsNew = $scope.analysis.MajorGeneratorArrayNew.join(",");
                    renewalService.submitAnalysis($scope.analysis).$promise.then(function (data) {
                        messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect("Renewal_Analysis", $scope.projectId);
                        });
                    }, function () {
                        messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
                        $scope.acting = false;
                    });
                }
            };
            $scope.resubmit = function (frm) {
                if (frm.$valid) {
                    $scope.acting = true;
                    $scope.analysis.BEType = $scope.analysis.BETypes.join(",");
                    $scope.analysis.MajorGenerators = $scope.analysis.MajorGeneratorArray.join(",");
                    $scope.analysis.MajorGeneratorsNew = $scope.analysis.MajorGeneratorArrayNew.join(",");
                    renewalService.resubmitAnalysis({
                        Entity: $scope.analysis,
                        Info: $scope.info,
                        SN: $routeParams.SN,
                        ProjectComment: ""
                    }).$promise.then(function () {
                        messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect("Renewal_Analysis", $scope.projectId);
                        });
                    }, function () {
                        messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
                        $scope.acting = false;
                    });
                };
            };
            $scope.edit = function () {
                $scope.acting = true;
                messager.confirm("[[[Renewal Analysis 审批流程已经完成，现在编辑会导致流程重新提交，是否确认]]]？", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        renewalService.editAnalysis($scope.analysis).$promise.then(function (editResult) {
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
                            renewalService.recallAnalysis({
                                Entity: $scope.analysis,
                                Info: $scope.info,
                                SN: $routeParams.SN,
                                ProjectComment: entity.Comment
                            }).$promise.then(function () {
                                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                                    redirectService.flowRedirect("Renewal_Analysis", $scope.projectId);
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