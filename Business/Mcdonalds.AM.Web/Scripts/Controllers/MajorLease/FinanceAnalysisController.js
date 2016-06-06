marjorLeaseApp.controller("financeAnalysisController", [
    "$http",
    "$scope",
    "$window",
    "$modal",
    '$location',
    "$routeParams",
    "majorLeaseService",
    'approveDialogService',
    'redirectService',
    "messager",
    function ($http, $scope, $window, $modal,$location, $routeParams, majorLeaseService,approveDialogService,redirectService, messager) {
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
        $scope.subFlowCode = "MajorLease_FinanceAnalysis";
        $scope.checkPointRefresh = true;
        $scope.isEditComments = true;
        $scope.IsShowComments = true;
        $scope.userNameENUS = window.currentUser.NameENUS;
        $scope.userNameZHCN = window.currentUser.NameZHCN;
        $scope.finance = {};
        $scope.uploadSet = [];
        $scope.$watch("isPageEditable", function (val) {
            if (val) {
                $scope.uploadSet = ['c71f6f13-7c37-473e-af04-8655863935a3'];
            }
        });

        majorLeaseService.getMajorInfo({ projectId: $scope.projectId }).$promise.then(function (data) {
            if (data == null)
                return;
            $scope.MajorLeaseInfo = data;
        }, function (error) {
            messager.showMessage(error.statusText, "fa-warning c_orange");
        });

        var loadFinanceInfo = function () {
            return majorLeaseService.getFinanceInfo({ projectId: $scope.projectId ,entityId:$scope.entityId}).$promise.then(function (data) {
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
            $scope.FinAgreement = null;
            $.each(atts, function (i, o) {
                if (o.TypeCode == "Attachment")
                    arrAttachment.push(o);
                else if (o.TypeCode == "FinAgreement")
                    $scope.FinAgreement = o;
            });
            if ($scope.FinAgreement == null)
                $scope.FinAgreement = { "FileURL": window.location.href, "Icon": "", "CreatorNameENUS": "", "CreateTime": "" };
            $scope.Attachment = arrAttachment;
        }
        var loadContract = function () {
            if ($scope.finance.Id == null) {
                loadFinanceInfo().then(function () {
                    if ($scope.finance.Id == null)
                        {return;}
                    //majorLeaseService.getFinanceAgreementList({ refTableId: $scope.finance.Id, typeCode: "" }).$promise.then(function (atts) {
                    //    if (atts != null)
                    //        populateAttach(atts);
                    //}, function (attError) {
                    //    messager.showMessage(attError.statusText, "fa-warning c_orange");
                    //});
                });
            } else {
                //majorLeaseService.getFinanceAgreementList({ refTableId: $scope.finance.Id, typeCode: "" }).$promise.then(function (atts) {
                //    if (atts != null)
                //        populateAttach(atts);
                //}, function (attError) {
                //    messager.showMessage(attError.statusText, "fa-warning c_orange");
                //});
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
            messager.confirm("[[[确认要删除？]]]", "fa-warning c_orange").then(function (result) {
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
                        $http.get(Utils.ServiceURI.Address() + "api/MajorLease/DeleteAttachement", {
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
                majorLeaseService.saveFinanceAnalysis($scope.finance).$promise.then(function (data) {
                    loadFinanceInfo();
                    messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                }, function (error) {
                    messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
                });
            }
            else if (action == "submit") {
                $scope.IsClickSubmit = true;
                majorLeaseService.submitFinanceAnalysis($scope.finance).$promise.then(function (data) {
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
            var errors = [];
            if ($.grep($scope.attachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "c71f6f13-7c37-473e-af04-8655863935a3";
            }) == 0) {
                errors.push("[[[请上传Finance Analysis附件]]]");
            }
            
            //if ($.grep($scope.attachments || [], function (att, i) {
            //        return !!att.FileURL && att.RequirementId == "23a0aa44-7c4d-42d6-b64a-31c14afb0370";
            //}) == 0) {
            //    errors.push("请上传Others");
            //}

            if (errors.length > 0) {
                messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
            }
            
            //if ($scope.FinAgreement == null) {
            //    messager.showMessage("请上传Finance Analysis附件", "fa-warning c_orange");
            //    return false;
            //}
            if ($scope.finance.FinanceComments == null) {
                messager.showMessage("[[[请填写Finance Comments]]]", "fa-warning c_orange");
                return false;
            }
            //if ($scope.finance.Comments == null) {
            //    messager.showMessage("[[[请填写Comments]]]", "fa-warning c_orange");
            //    return false;
            //}
            return errors.length == 0;
        }
        $scope.save = function () {
                save("save");
        }
        var resubmit = function() {
            $scope.IsClickSubmit = true;
            $scope.finance.SerialNumber = $routeParams.SN;
            $scope.finance.LastUpdateUserAccount = window.currentUser.Code;
            $scope.finance.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.finance.LastUpdateUserNameENUS = window.currentUser.NameENUS;
            majorLeaseService.resubmitFinanceAnalysis($scope.finance).$promise.then(function () {
                messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                    redirectService.flowRedirect($scope.finance.WorkflowCode, $scope.projectId);
                });
            }, function (error) {
                messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
            });
        };
        $scope.submit = function (frm) {
            if (frm.$valid) {
                if (checkValidate()) {
                    approveDialogService.open($scope.projectId, $scope.subFlowCode).then(function(approverInfo) {
                        $scope.approveDialogSubmit(approverInfo);
                    });
                }
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
            majorLeaseService.approveFinanceAnalysis($scope.finance).$promise.then(function () {
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
                            majorLeaseService.recallFinanceAnalysis($scope.finance).$promise.then(function () {
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
            messager.confirm("[[[Finance Analysis 审批流程已经完成，现在编辑会导致流程重新提交，是否确认？]]]", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        majorLeaseService.editFinanceAnalysis($scope.finance).$promise.then(function (response) {
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
            majorLeaseService.resubmitFinanceAnalysis($scope.finance).$promise.then(function () {
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
            $scope.IsClickReturn = true;
            $scope.finance.SerialNumber = $routeParams.SN;
            $scope.finance.LastUpdateUserAccount = window.currentUser.Code;
            $scope.finance.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.finance.LastUpdateUserNameENUS = window.currentUser.NameENUS;
            messager.confirm("[[[确定要进行退回吗？]]]", "fa-warning c_orange").then(function (result) {
                if (result) {
                    majorLeaseService.returnFinanceAnalysis($scope.finance).$promise.then(function () {
                        messager.showMessage("[[[退回成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect($scope.finance.WorkflowCode, $scope.projectId);
                        });
                    }, function (error) {
                        messager.showMessage("[[[退回失败]]]" + error.message, "fa-warning c_orange");
                    });
                }
                else
                    $scope.IsClickReturn = false;
            });
            
            return true;
        }
    }]);