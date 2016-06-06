rebuildApp.controller("financeAnalysisController", [
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
    function ($http, $scope, $window, $modal, $location, $routeParams, rebuildService, approveDialogService,contractService,redirectService, messager) {
        $scope.pageType = $routeParams.PageType;
        $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);
        switch ($scope.pageType) {
            case 'Approval':
            case 'View':
                $scope.isPageEditable = false;
                break;
            default:
                $scope.isPageEditable = true;
                break;
        }

        $scope.userAccount = window.currentUser.Code;
        $scope.projectId = $routeParams.projectId;
        $scope.entityId = $routeParams.entityId;
        $scope.isHistory = $routeParams.isHistory;
        $scope.subFlowCode = "Rebuild_FinanceAnalysis";
        $scope.checkPointRefresh = true;
        $scope.isEditComments = true;
        $scope.IsShowComments = true;
        $scope.userNameENUS = window.currentUser.NameENUS;
        $scope.userNameZHCN = window.currentUser.NameZHCN;
        $scope.finance = {};

        rebuildService.getRebuildInfo({ projectId: $scope.projectId }).$promise.then(function (data) {
            if (data == null)
                return;
            $scope.RebuildInfo = data;
        }, function (error) {
            messager.showMessage(error.statusText, "fa-warning c_orange");
        });

        var loadFinanceInfo = function () {
            return rebuildService.getFinanceInfo({ projectId: $scope.projectId, entityId: $scope.entityId }).$promise.then(function (data) {
                if (data != null && data.Id != "00000000-0000-0000-0000-000000000000") {
                    $scope.finance = data;
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
            var arrAttachment = new Array();
            $scope.FinAgreement = {};
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
                if (o.TypeCode == "Attachment")
                    arrAttachment.push(o);
                else if (o.TypeCode == "FinAgreement")
                    $scope.FinAgreement = o;
            });
            if (atts.length == 0)
                $scope.FinAgreement.downloadLink = "";
            $scope.Attachment = arrAttachment;
        }
        var loadContract = function () {
            if ($scope.finance.Id == null) {
                loadFinanceInfo().then(function () {
                    if ($scope.finance.Id == null)
                    { return; }
                    rebuildService.getFinanceAgreementList({ refTableId: $scope.finance.Id, typeCode: "" }).$promise.then(function (atts) {
                        if (atts != null)
                            populateAttach(atts);
                    }, function (attError) {
                        messager.showMessage(attError.statusText, "fa-warning c_orange");
                    });
                });
            } else {
                rebuildService.getFinanceAgreementList({ refTableId: $scope.finance.Id, typeCode: "" }).$promise.then(function (atts) {
                    if (atts != null)
                        populateAttach(atts);
                }, function (attError) {
                    messager.showMessage(attError.statusText, "fa-warning c_orange");
                });
            }
        }
        loadContract();
        var uploadSuccess = function () {
            messager.showMessage("[[[上传成功]]]", "fa-warning c_orange");
        };
        $scope.uploadAttachFinished = function () {
            loadContract();
            uploadSuccess();
        };
        $scope.uploadFAFinished = function () {
            loadContract();
            uploadSuccess();
            $scope.checkPointRefresh = true;
        };
        $scope.deleteAttachment = function (attId, typecode) {
            messager.confirm("[[[确认要删除]]]？", "fa-warning c_orange").then(function (result) {
                if (result) {
                    if (typecode == "Attachment") {
                        $http({
                            method: "POST",
                            params: {
                                id: attId
                            },
                            url: Utils.ServiceURI.Address() + "api/attachment/delete/"

                        }).success(function (data) {
                            loadContract();
                        });

                    } else {
                        $http.get(Utils.ServiceURI.Address() + "api/Rebuild/DeleteAttachement", {
                            params: {
                                attId: attId,
                                projectId: $scope.projectId,
                                flowCode: "FinanceAnalysis"
                            }
                        }).success(function (data) {
                            loadContract();
                            $scope.checkPointRefresh = true;
                        });
                    }
                }
            });
        }

        var save = function (action) {
            if ($scope.finance.Id == null) {
                $scope.finance.Id = "00000000-0000-0000-0000-000000000000";
            }
            $scope.finance.projectId = $scope.projectId;
            $scope.finance.LastUpdateUserAccount = window.currentUser.Code;
            $scope.finance.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.finance.LastUpdateUserNameENUS = window.currentUser.NameENUS;

            if (action == "save") {
                $scope.IsClickSave = true;
                rebuildService.saveFinanceAnalysis($scope.finance).$promise.then(function (data) {
                    loadFinanceInfo();
                    messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                }, function (error) {
                    messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
                });
            }
            else if (action == "submit") {
                $scope.IsClickSubmit = true;
                rebuildService.submitFinanceAnalysis($scope.finance).$promise.then(function (data) {
                    messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                        redirectService.flowRedirect($scope.finance.WorkflowCode, $scope.projectId);
                    });
                }, function (error) {
                    messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
                });
            } else
                return;
        }
        var checkValidate = function () {
            if ($scope.FinAgreement == null || $scope.FinAgreement.downloadLink=="") {
                messager.showMessage("[[[请上传Finance Analysis附件]]]", "fa-warning c_orange");
                return false;
            }
            if ($scope.finance.FinanceComments == null) {
                messager.showMessage("[[[请填写Finance Comments]]]", "fa-warning c_orange");
                return false;
            }
            //if ($scope.finance.Comments == null) {
            //    messager.showMessage("[[[请填写Comments]]]", "fa-warning c_orange");
            //    return false;
            //}
            return true;
        }
        $scope.save = function () {
            save("save");
        }
        var resubmit = function () {
            $scope.IsClickSubmit = true;
            $scope.finance.SerialNumber = $routeParams.SN;
            $scope.finance.LastUpdateUserAccount = window.currentUser.Code;
            $scope.finance.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.finance.LastUpdateUserNameENUS = window.currentUser.NameENUS;
            rebuildService.resubmitFinanceAnalysis($scope.finance).$promise.then(function () {
                messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                    redirectService.flowRedirect($scope.finance.WorkflowCode, $scope.projectId);
                });
            }, function (error) {
                messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
            });
        };
        $scope.submit = function () {
            if (checkValidate()) {
                approveDialogService.open($scope.projectId, $scope.subFlowCode, "", "", $scope.finance).then(function (approverInfo) {
                    $scope.approveDialogSubmit(approverInfo);
                });
            }
        }
        $scope.approveDialogSubmit = function (notifyUsersInfo) {
            $scope.finance.AppUsers = notifyUsersInfo;
            if ($scope.pageType == "Resubmit") {
                resubmit();
            } else {
                save("submit");
            }
        };

        $scope.approve = function () {
            $scope.IsClickApprove = true;
            $scope.finance.SerialNumber = $routeParams.SN;
            $scope.finance.LastUpdateUserAccount = window.currentUser.Code;
            $scope.finance.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.finance.LastUpdateUserNameENUS = window.currentUser.NameENUS;
            rebuildService.approveFinanceAnalysis($scope.finance).$promise.then(function () {
                messager.showMessage("[[[审批成功]]]", "fa-check c_green").then(function () {
                    redirectService.flowRedirect($scope.finance.WorkflowCode, $scope.projectId);
                });
            }, function (error) {
                messager.showMessage("[[[审批成功]]]" + error.message, "fa-warning c_orange");
            });
        }

        $scope.recall = function () {
            $scope.finance.SerialNumber = $routeParams.SN;
            $scope.finance.LastUpdateUserAccount = window.currentUser.Code;
            $scope.finance.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.finance.LastUpdateUserNameENUS = window.currentUser.NameENUS;
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
                            $scope.finance.Comments = entity.Comment;
                            rebuildService.recallFinanceAnalysis($scope.finance).$promise.then(function () {
                                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                                    redirectService.flowRedirect($scope.finance.WorkflowCode, $scope.projectId);
                                });
                            }, function (error) {
                                messager.showMessage("[[[撤回失败]]]" + error.message, "fa-warning c_orange");
                            });
                        });
                    }
                });


        }

        $scope.edit = function () {
            messager.confirm("[[[Finance Analysis 审批流程已经完成，现在编辑会导致流程重新提交，是否确认]]]？", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        rebuildService.editFinanceAnalysis($scope.finance).$promise.then(function (response) {
                            $window.location.href = Utils.ServiceURI.WebAddress() + response.TaskUrl;
                        }, function () {
                            messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                        });
                    }
                });
        }

        $scope.resubmit = function () {
            $scope.IsClickSubmit = true;
            $scope.finance.SerialNumber = $routeParams.SN;
            rebuildService.resubmitFinanceAnalysis($scope.finance).$promise.then(function () {
                messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                    redirectService.flowRedirect($scope.finance.WorkflowCode, $scope.projectId);
                });
            }, function (error) {
                messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
            });
        }

        $scope.returnToOriginator = function () {
            if (!$scope.finance.Comments) {
                messager.showMessage("[[[请先填写意见]]]!", "fa-warning c_orange");
                return false;
            }
            messager.confirm("[[[确认需要Return吗？]]]", "fa-warning c_orange").then(function (result) {
                if (result) {
                    $scope.IsClickReturn = true;
                    $scope.finance.SerialNumber = $routeParams.SN;
                    $scope.finance.LastUpdateUserAccount = window.currentUser.Code;
                    $scope.finance.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
                    $scope.finance.LastUpdateUserNameENUS = window.currentUser.NameENUS;
                    rebuildService.returnFinanceAnalysis($scope.finance).$promise.then(function () {
                        messager.showMessage("[[[退回成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect($scope.finance.WorkflowCode, $scope.projectId);
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
        $scope.showContractAttachments = function () {
            $modal.open({
                size: "lg",
                backdrop: "static",
                templateUrl: Utils.ServiceURI.AppUri + "Module/ContractAttachments",
                controller: ["$scope", "$modalInstance", function ($modalScope, $modalInstance) {
                    contractService.queryAttachments($scope.projectId).then(function (response) {
                        angular.forEach(response.data, function (d, i) {
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
    }]);