reimageApp.controller('reimageSummaryCtrl',
[
    "$scope",
    '$window',
    '$location',
    '$routeParams',
    "reimageService",
    'approveDialogService',
    'redirectService',
    '$modal',
     "messager",
     "excelService",
     "$http",
    function ($scope, $window, $location, $routeParams, reimageService, approveDialogService, redirectService, $modal, messager, excelService, $http) {
        $scope.pageType = $routeParams.PageType;
        $scope.projectId = $routeParams.projectId;
        $scope.entityId = $routeParams.entityId;
        $scope.isHistory = $routeParams.isHistory;
        $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);
        switch ($routeParams.PageType) {
            case 'Approval':
            case 'View':
                $scope.isPageEditable = false;
                break;
            default:
                $scope.isPageEditable = true;
                //$scope.uploadSet = ['5647bdbb-5b5f-47cd-9ad2-3e35a16dd303'];
                break;
        }
        $scope.checkPointRefresh = true;
        $scope.hasGenerateReimageSummary = false;

        reimageService.getReimageInfo({ projectId: $routeParams.projectId }).$promise.then(function (response) {
            $scope.reimageInfo = response;
            if (response.USCode) {
                $scope.reimageInfo.USCode = response.USCode;
            }
        });

        reimageService.initReimageSummary({
            projectId: $scope.projectId,
            entityId: $scope.entityId
        }).$promise.then(function (response) {
            $scope.entity = response.entity;
            $scope.isActor = response.isActor;
            //$scope.pageType = response.PageType;
        });

        $scope.uploadAttachmentFinish = function (up, files) {
            reimageService.getReimageSummary({ projectId: $routeParams.projectId }).$promise.then(function (response) {
                $scope.entity.Id = response.Id;
                $scope.entity.IsHistory = response.IsHistory;

            });
        };
        $scope.generateReimageSummary = function () {
            var url = Utils.ServiceURI.Address() + "api/project/isFlowFinished/" + $scope.projectId + "/Reimage_ConsInfo";
            $http.get(url).success(function (result) {
                if ($.trim(result) != "true") {
                    messager.showMessage("ConsInfo未完成", "fa-warning c_orange");
                }
                else {
                    if (!$scope.entity.StoreProfitabilityAndLeaseInfo.AsOf) {
                        messager.showMessage("请先选择 TTM Sales (RMB)", "fa-warning c_orange");
                        return false;
                    }

                    reimageService.saveReimageSummary($scope.entity).$promise.then(function () {
                        excelService.generateReimageSummary({ projectId: $scope.projectId }).$promise.then(function (att) {
                            $scope.$broadcast("AttachmentUploadFinish");
                            $scope.$broadcast('LoadFinancialPreAnalysis');
                            $window.location.href = Utils.ServiceURI.Address() + "api/attachment/download?id=" + att.ID;
                        });
                    }, function () {
                        messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
                    });
                }
            });
        }

        $scope.save = function () {
            $scope.acting = true;
            $scope.entity.projectId = $scope.projectId;

            reimageService.saveReimageSummary($scope.entity).$promise.then(function () {
                $scope.$broadcast("AttachmentUploadFinish");
                $scope.$broadcast('LoadFinancialPreAnalysis');
                messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                $scope.acting = false;
            }, function () {
                messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
                $scope.acting = false;
            });
        };
        $scope.submit = function (frm) {
            //var url = Utils.ServiceURI.Address() + "api/project/isFlowFinished/" + $scope.projectId + "/Reimage_ConsInfo";
            //$http.get(url).success(function (result) {
            //if ($.trim(result) != "true") {
            //    messager.showMessage("ConsInfo未完成", "fa-warning c_orange");
            //}
            //else {
            if (!$scope.entity.StoreProfitabilityAndLeaseInfo.AsOf) {
                messager.showMessage("请先选择 TTM Sales (RMB)", "fa-warning c_orange");
                return false;
            }

            if (frm.$valid) {
                $scope.acting = true;
                $scope.entity.projectId = $scope.projectId;
                if (checkValidate()) {
                    messager.confirm("确定要进行提交吗？", "fa-warning c_orange").then(function (result) {
                        if (result) {
                            reimageService.submitReimageSummary($scope.entity).$promise.then(function () {
                                messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                                    redirectService.flowRedirect($scope.entity.WorkflowCode, $scope.projectId);
                                });
                            }, function () {
                                messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
                                $scope.acting = false;
                            });
                        }
                        else
                            $scope.acting = false;
                    });
                }
            }
            //}
            //});
        }

        $scope.edit = function () {
            messager.confirm("Reimage Summary 审批流程已经完成，现在编辑会导致流程重新提交，是否确认？", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        reimageService.editReimageSummary($scope.entity).$promise.then(function (response) {
                            $window.location.href = Utils.ServiceURI.WebAddress() + response.TaskUrl;
                        }, function () {
                            messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                        });
                    }
                });
        }

        $scope.recall = function () {
            $scope.entity.SerialNumber = $routeParams.SN;

            messager.confirm("[[[确认需要撤销吗？]]]", "fa-warning c_orange")
               .then(function (result) {
                   if (result) {
                       $modal.open({
                           backdrop: 'static',
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
                           //$scope.entity.Comments = entity.Comment;
                           reimageService.recallReimageSummary($scope.entity).$promise.then(function () {
                               messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                                   redirectService.flowRedirect($scope.entity.WorkflowCode, $scope.projectId);
                               });
                           }, function (error) {
                               messager.showMessage("[[[撤回失败]]]" + error.message, "fa-warning c_orange");
                           });
                       });
                   }
               });


        }

        var checkValidate = function () {
            var errors = [];
            //if ($.grep($scope.packageAttachments || [], function (att, i) {
            //        return !!att.FileURL && att.RequirementId == "5647bdbb-5b5f-47cd-9ad2-3e35a16dd303";
            //}) == 0) {
            //    errors.push("请先上传Reimage Summary附件！");
            //}

            //if (errors.length > 0) {
            //    messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
            //}
            return errors.length == 0;
        }


    }
]);