rebuildApp.controller("legalReviewController", [
    "$http",
    "$scope",
    "$window",
    "$modal",
    '$location',
    "$routeParams",
    "rebuildService",
    'approveDialogService',
    "contractService",
    "redirectService",
    "messager",
    function ($http, $scope, $window, $modal, $location, $routeParams, rebuildService, approveDialogService, contractService, redirectService,messager) {
        $scope.pageType = $routeParams.PageType;
        $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);
        switch ($scope.pageType) {
            case 'Approval':
            case 'View':
                $scope.isPageEditable = false;
                break;
            default:
                $scope.isPageEditable = true;
                $scope.IsShowMaterTracking = false;
                break;
        }
        $scope.userAccount = window.currentUser.Code;
        $scope.projectId = $routeParams.projectId;
        $scope.entityId = $routeParams.entityId;
        $scope.isHistory = $routeParams.isHistory;
        $scope.subFlowCode = "Rebuild_LegalReview";
        $scope.checkPointRefresh = true;
        $scope.isEditComments = true;
        $scope.userNameENUS = window.currentUser.NameENUS;
        $scope.userNameZHCN = window.currentUser.NameZHCN;
        $scope.upLoadSet = ['5e00b134-b817-48ef-8f36-b542bc5687ca', '44b5ca21-816e-4fc6-8100-35a85120a614', '8f5439de-4c2e-47d2-a262-c40b5fbd2e1b'];

        $scope.legalView = {};

        //if ($scope.isHistory != null && $scope.isHistory)
        //    $scope.legalView.Id = $scope.entityId;
        rebuildService.getRebuildInfo({ projectId: $scope.projectId }).$promise.then(function (data) {
            if (data == null)
            { return; }
            $scope.RebuildInfo = data;
            if ($scope.RebuildInfo.AssetActorAccount == window.currentUser.Code) {
                $scope.isEditLegalReviewComments = false;
                $scope.AssetActor = true;
                $scope.upLoadSet = ['5e00b134-b817-48ef-8f36-b542bc5687ca', '8f5439de-4c2e-47d2-a262-c40b5fbd2e1b'];
            }
            else if ($scope.RebuildInfo.LegalAccount == window.currentUser.Code) {
                if ($scope.pageType == "Approval") {
                    $scope.isEditLegalReviewComments = true;
                    $scope.upLoadSet = ['44b5ca21-816e-4fc6-8100-35a85120a614'];
                }
                $scope.AssetActor = false;
                //$scope.isPageEditable = true;
            } else {
                $scope.isEditLegalReviewComments = false;
                $scope.upLoadSet = ['5e00b134-b817-48ef-8f36-b542bc5687ca', '8f5439de-4c2e-47d2-a262-c40b5fbd2e1b'];
            }
            if (!$scope.isPageEditable && $scope.pageType == "View") {
                $scope.upLoadSet = [];
            }
        }, function (error) {
            messager.showMessage(error.statusText, "fa-warning c_orange");
        });

        var loadLegalReviewInfo = function () {
            return rebuildService.getLegalReviewInfo({ projectId: $scope.projectId, entityId: $scope.entityId }).$promise.then(function (data) {
                if (data != null && data.Id != "00000000-0000-0000-0000-000000000000") {
                    $scope.legalView = data;
                    if (data.IsShowSave && $scope.pageType == 'View') {
                        $scope.isPageEditable = $scope.isPageEditable || data.IsShowSave;
                    }
                }
            },
            function (error) {
                messager.showMessage(error.statusText, "fa-warning c_orange");
            });
        }
        var populateAttach = function (atts) {
            var arrContract = new Array();
            $scope.LegalClearanceReport = null;
            $scope.Agreement = null;
            $scope.Others = null;
            $.each(atts, function (i, o) {
                if (o.ID != "00000000-0000-0000-0000-000000000000") {
                    o.downloadLink = Utils.ServiceURI.Address() + "api/attachment/download?id=" + o.ID;
                    switch (o.Extension.toLowerCase()) {
                    case ".xlsx":
                    case ".xls":
                        o.Icon = "fa fa-file-excel-o c_green";
                        break;
                    case ".ppt":
                        o.Icon = "fa fa-file-powerpoint-o c_red";
                        break;
                    case ".doc":
                    case ".docx":
                        o.Icon = "fa fa-file-word-o c_blue";
                        break;
                    default:
                        o.Icon = "fa fa-file c_orange";
                        break;
                    }
                }
                if (o.TypeCode == "Contract") {
                    arrContract.push(o);
                }
                else if (o.TypeCode == "LegalClearanceReport")
                { $scope.LegalClearanceReport = o; }
                else if (o.TypeCode == "Agreement")
                { $scope.Agreement = o; }
                else if (o.TypeCode == "Others")
                { $scope.Others = o; }
            });
            $scope.Contract = arrContract;
        }
        var loadContract = function () {
            if ($scope.legalView.Id == null) {
                loadLegalReviewInfo().then(function () {
                    if ($scope.legalView.Id == null || $scope.legalView.Id == "00000000-0000-0000-0000-000000000000") {
                        return;
                    }
                            //rebuildService.getLegalContractList({ refTableId: $scope.legalView.Id, typeCode: "" }).$promise.then(function (atts) {
                            //    if (atts != null)
                            //    { populateAttach(atts); }
                            //}, function (attError) {
                            //    messager.showMessage(attError.statusText, "fa-warning c_orange");
                            //});
                        });
            }
            //else {
                        //rebuildService.getLegalContractList({ refTableId: $scope.legalView.Id, typeCode: "" }).$promise.then(function (atts) {
                        //    if (atts != null) {
                        //        populateAttach(atts);
                        //    }
                        //}, function (attError) {
                        //    messager.showMessage(attError.statusText, "fa-warning c_orange");
                        //});
                //}
               
        }
        loadContract();
        var uploadSuccess = function () {
            messager.showMessage("[[[上传成功]]]", "fa-warning c_orange");
        };
        ////$scope.uploadAttachFinished = function () {
        ////    loadContract();
        ////    uploadSuccess();
        ////};
        ////$scope.uploadFinished = function () {
        ////    loadContract();
        ////    uploadSuccess();
        ////    $scope.checkPointRefresh = true;
        ////};
        //$scope.deleteAttachment = function (attId, typecode) {
        //    messager.confirm("[[[确认要删除？]]]", "fa-warning c_orange").then(function (result) {
        //        if (result) {
        //            if (typecode == "contract") {
        //                $http({
        //                    method: "POST",
        //                    params: {
        //                        id: attId
        //                    },
        //                    url: Utils.ServiceURI.Address() + "api/attachment/delete/"

        //                }).success(function (data) {
        //                    loadContract();
        //                });
        //            } else {
        //                $http.get(Utils.ServiceURI.Address() + "api/Rebuild/DeleteAttachement", {
        //                    params: {
        //                        attId: attId,
        //                        projectId: $scope.projectId,
        //                        flowCode: "LegalReview"
        //                    }
        //                }).success(function (data) {
        //                    loadContract();
        //                    $scope.checkPointRefresh = true;
        //                });
        //            }
        //        }
        //    });
        //}

        var save = function (action) {
            if ($scope.legalView.Id == null) {
                $scope.legalView.Id = "00000000-0000-0000-0000-000000000000";
            }
            $scope.legalView.projectId = $scope.projectId;
            $scope.legalView.LastUpdateUserAccount = window.currentUser.Code;
            $scope.legalView.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.legalView.LastUpdateUserNameENUS = window.currentUser.NameENUS;

            if (action == "save") {
                $scope.IsClickSave = true;
                rebuildService.saveRebuildLegalReview($scope.legalView).$promise.then(function (data) {
                    loadLegalReviewInfo();
                    messager.showMessage("[[[保存成功]]]", "fa-warning c_orange");
                }, function (error) {
                    messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
                });
            }
            else if (action == "submit") {
                $scope.IsClickSubmit = true;
                rebuildService.submitRebuildLegalReview($scope.legalView).$promise.then(function (data) {
                    messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                        redirectService.flowRedirect($scope.legalView.WorkflowCode, $scope.projectId);
                    });
                }, function (error) {
                    messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
                });
            } else
                return;
        }

        var checkValidate = function () {
            var errors = [];
            if ($.grep($scope.attachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "5e00b134-b817-48ef-8f36-b542bc5687ca";
            }) == 0) {
                errors.push("请上传Legal Clearance Report!");
            }
            if ($.grep($scope.attachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "44b5ca21-816e-4fc6-8100-35a85120a614";
            }) == 0) {
                if (!$scope.AssetActor) {
                    errors.push("请上传Agreement！");
                }
            }
            //if ($.grep($scope.attachments || [], function (att, i) {
            //        return !!att.FileURL && att.RequirementId == "8f5439de-4c2e-47d2-a262-c40b5fbd2e1b";
            //}) == 0) {
            //    errors.push("请上传Others！");
            //}
             if ($scope.legalView.LegalComments == null && !$scope.AssetActor) {
                 errors.push("请先填写Legal Review Comments！");
            }
            if (errors.length > 0) {
                messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
            }
            return errors.length == 0;
            ////if ($scope.LegalClearanceReport == null && !$scope.AssetActor) {
            ////    messager.showMessage("请上传Legal Clearance Report", "fa-warning c_orange");
            ////    return false;
            ////} else if ($scope.Agreement == null && !$scope.AssetActor) {
            ////    messager.showMessage("请上传Agreement", "fa-warning c_orange");
            ////    return false;
            ////} else if ($scope.Others == null && !$scope.AssetActor) {
            ////    messager.showMessage("请上传Others", "fa-warning c_orange");
            ////    return false;
            ////}
            ////else if ($scope.legalView.LegalComments == null && !$scope.AssetActor) {
            ////    messager.showMessage("请先填写Legal Review Comments!", "fa-warning c_orange");
            ////    return false;
            ////}
            ////return true;
        }
        $scope.save = function () {
            save("save");
        }
        $scope.submit = function () {
            if (checkValidate()) {
                approveDialogService.open($scope.projectId, $scope.subFlowCode, "", "", $scope.legalView).then(function (approverInfo) {
                    $scope.approveDialogSubmit(approverInfo);
                });
            }
        }
        var resubmit = function () {
            $scope.IsClickSubmit = true;
            $scope.legalView.SerialNumber = $routeParams.SN;
            $scope.legalView.LastUpdateUserAccount = window.currentUser.Code;
            $scope.legalView.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.legalView.LastUpdateUserNameENUS = window.currentUser.NameENUS;
            if (!checkValidate()) {
                return;
            }
            rebuildService.resubmitRebuildLegalReview($scope.legalView).$promise.then(function () {
                messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                    redirectService.flowRedirect($scope.legalView.WorkflowCode, $scope.projectId);
                });
            }, function (error) {
                messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
            });
        };
        $scope.approveDialogSubmit = function (notifyUsersInfo) {
            $scope.legalView.AppUsers = notifyUsersInfo;
            if ($scope.pageType == "Resubmit") {
                resubmit();
            } else {
                save("submit");
            }
        };

        $scope.approve = function () {
            $scope.legalView.SerialNumber = $routeParams.SN;
            $scope.legalView.LastUpdateUserAccount = window.currentUser.Code;
            $scope.legalView.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.legalView.LastUpdateUserNameENUS = window.currentUser.NameENUS;
            if (!checkValidate()) {
                return;
            }
            $scope.IsClickApprove = true;
            rebuildService.approveRebuildLegalReview($scope.legalView).$promise.then(function () {
                messager.showMessage("[[[审批成功]]]", "fa-check c_green").then(function () {
                    redirectService.flowRedirect($scope.legalView.WorkflowCode, $scope.projectId);
                });
            }, function (error) {
                messager.showMessage("[[[审批成功]]]" + error.message, "fa-warning c_orange");
            });
        }

        $scope.recall = function () {
            $scope.legalView.SerialNumber = $routeParams.SN;
            $scope.legalView.LastUpdateUserAccount = window.currentUser.Code;
            $scope.legalView.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.legalView.LastUpdateUserNameENUS = window.currentUser.NameENUS;

            messager.confirm("[[[确认需要撤销吗？]]]", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        $modal.open({
                            backdrop: 'static',
                            templateUrl: Utils.ServiceURI.AppUri + "Template/Recall",
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
                            $scope.legalView.Comments = entity.Comment;
                            rebuildService.recallRebuildLegalReview($scope.legalView).$promise.then(function () {
                                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                                    redirectService.flowRedirect($scope.legalView.WorkflowCode, $scope.projectId);
                                });
                            }, function (error) {
                                messager.showMessage("[[[撤回失败]]]" + error.message, "fa-warning c_orange");
                            });
                        });
                    }
                });
        }

        $scope.edit = function () {
            messager.confirm("[[[Legal Review 审批流程已经完成，现在编辑会导致流程重新提交，是否确认？]]]", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        rebuildService.editRebuildLegalReview($scope.legalView).$promise.then(function (response) {
                            $window.location.href = Utils.ServiceURI.WebAddress() + response.TaskUrl;
                        }, function () {
                            messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                        });
                    }
                });
        }

        $scope.resubmit = function () {
            $scope.IsClickSubmit = true;
            $scope.legalView.SerialNumber = $routeParams.SN;
            rebuildService.resubmitRebuildLegalReview($scope.legalView).$promise.then(function () {
                messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                    redirectService.flowRedirect($scope.legalView.WorkflowCode, $scope.projectId);
                });
            }, function (error) {
                messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
            });
        }

        $scope.returnToOriginator = function () {
            if (!$scope.legalView.Comments) {
                messager.showMessage("[[[请先填写意见]]]!", "fa-warning c_orange");
                return false;
            }
            messager.confirm("[[[确认需要Return吗？]]]", "fa-warning c_orange").then(function (result) {
                if (result) {
                    $scope.IsClickReturn = true;
                    $scope.legalView.LastUpdateUserAccount = window.currentUser.Code;
                    $scope.legalView.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
                    $scope.legalView.LastUpdateUserNameENUS = window.currentUser.NameENUS;
                    $scope.legalView.SerialNumber = $routeParams.SN;
                    rebuildService.returnRebuildLegalReview($scope.legalView).$promise.then(function () {
                        messager.showMessage("[[[退回成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect($scope.legalView.WorkflowCode, $scope.projectId);
                        });
                    }, function (error) {
                        messager.showMessage("[[[退回失败]]]" + error.message, "fa-warning c_orange");
                    });
                }
            });
            return true;
        }
        contractService.queryAttachmentCount($scope.projectId).then(function (response) {
            $scope.contractAttachCount = response.data;
        });
        $scope.showContractAttachments= function() {
            $modal.open({
                size: "lg",
                backdrop: "static",
                templateUrl: Utils.ServiceURI.AppUri + "Module/ContractAttachments",
                controller: ["$scope", "$modalInstance", function($modalScope, $modalInstance) {
                    contractService.queryAttachments($scope.projectId).then(function (response) {
                        angular.forEach(response.data, function (d, i){
                            d.downloadLink = Utils.ServiceURI.AttachmentAddress() + "api/contract/downloadAttachment?id=" + d.Id;
                    });
                    $modalScope.list = response.data;
                    $modalScope.attachLoaded = true;
                    });
                    $modalScope.cancel = function () {
                        $modalInstance.dismiss('');
                };
                }]
            });
        }
    }]).controller('legalReviewProcessCtrl', ['$routeParams', '$location', 'rebuildService', function ($routeParams, $location, rebuildService) {
        if ($routeParams.ProcInstID
       && $routeParams.SN) {
            rebuildService.getProjectId({ procInstId: $routeParams.ProcInstID }).$promise.then(function (response) {
                $location.url($location.url().replace($location.path(), '/Rebuild/LegalReview/View/' + response.ProjectId));
            });
        }
    }
    ]);
