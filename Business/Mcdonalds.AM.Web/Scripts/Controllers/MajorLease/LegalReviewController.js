marjorLeaseApp.controller("legalReviewController", [
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
    function ($http, $scope, $window, $modal, $location, $routeParams, majorLeaseService, approveDialogService, redirectService, messager) {
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
        $scope.subFlowCode = "MajorLease_LegalReview";
        $scope.checkPointRefresh = true;
        $scope.isEditComments = true;
        $scope.userNameENUS = window.currentUser.NameENUS;
        $scope.userNameZHCN = window.currentUser.NameZHCN;
        $scope.legalView = {};
        $scope.uploadSet = [];
        $scope.$watch("isPageEditable", function (val) {
            if (val) {
                if ($scope.pageType == 'Approval') {
                    $scope.uploadSet = ['8daaa73f-5e2f-4f09-9f56-e9941604d9e6'];
                } else {
                    $scope.uploadSet = ['3c296056-1d64-440f-9426-2ed1624ed4fa', '23a0aa44-7c4d-42d6-b64a-31c14afb0370'];
                }
            }
        });
        majorLeaseService.getMajorInfo({ projectId: $scope.projectId }).$promise.then(function (data) {
            if (data == null)
            { return; }
            $scope.MajorLeaseInfo = data;
            if ($scope.MajorLeaseInfo.AssetActorAccount == window.currentUser.Code) {
                $scope.isEditLegalReviewComments = false;
                $scope.AssetActor = true;
            }
            else if ($scope.MajorLeaseInfo.LegalAccount == window.currentUser.Code) {
                if ($scope.pageType == "Approval") {
                    $scope.isEditLegalReviewComments = true;
                    $scope.isPageEditable = true;
                }
                $scope.AssetActor = false;
            } else {
                $scope.isEditLegalReviewComments = false;
            }
        }, function (error) {
            messager.showMessage(error.statusText, "fa-warning c_orange");
        });

        var loadLegalReviewInfo = function () {
            return majorLeaseService.getLegalReviewInfo({ projectId: $scope.projectId, entityId: $scope.entityId }).$promise.then(function (data) {
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
            if ($scope.LegalClearanceReport == null)
                $scope.LegalClearanceReport = { "FileURL": window.location.href, "Icon": "", "CreatorNameENUS": "", "CreateTime": "" };
            if ($scope.Agreement == null)
                $scope.Agreement = { "FileURL": window.location.href, "Icon": "", "CreatorNameENUS": "", "CreateTime": "" };
            if ($scope.Others == null)
                $scope.Others = { "FileURL": window.location.href, "Icon": "", "CreatorNameENUS": "", "CreateTime": "" };
            $scope.Contract = arrContract;
        }
        var loadContract = function () {
            if ($scope.legalView.Id == null) {
                loadLegalReviewInfo().then(function () {
                    if ($scope.legalView.Id == null) {
                        return;
                    }
                    //majorLeaseService.getLegalContractList({ refTableId: $scope.legalView.Id, typeCode: "" }).$promise.then(function (atts) {
                    //    if (atts != null)
                    //    { populateAttach(atts); }
                    //}, function (attError) {
                    //    messager.showMessage(attError.statusText, "fa-warning c_orange");
                    //});
                });
            } else {
                //majorLeaseService.getLegalContractList({ refTableId: $scope.legalView.Id, typeCode: "" }).$promise.then(function (atts) {
                //    if (atts != null) {
                //        populateAttach(atts);
                //    }
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
        $scope.uploadFinished = function () {
            loadContract();
            uploadSuccess();
            $scope.checkPointRefresh = true;
        };
        $scope.deleteAttachment = function (attId, typecode) {
            messager.confirm("[[[确认要删除？]]]", "fa-warning c_orange").then(function (result) {
                if (result) {
                    if (typecode == "contract") {
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
                                flowCode: "LegalReview"
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
            if ($scope.legalView.Id == null) {
                $scope.legalView.Id = "00000000-0000-0000-0000-000000000000";
            }
            $scope.legalView.projectId = $scope.projectId;

            if (action == "save") {
                $scope.IsClickSave = true;
                majorLeaseService.saveMajorLegalReview($scope.legalView).$promise.then(function (data) {
                    loadLegalReviewInfo();
                    messager.showMessage("[[[保存成功]]]", "fa-warning c_orange");
                }, function (error) {
                    messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
                });
            }
            else if (action == "submit") {
                $scope.IsClickSubmit = true;
                majorLeaseService.submitMajorLegalReview($scope.legalView).$promise.then(function (data) {
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
                    return !!att.FileURL && att.RequirementId == "3c296056-1d64-440f-9426-2ed1624ed4fa";
            }) == 0) {
                errors.push("[[[请上传Legal Clearance Report]]]");
            }
            //if ($.grep($scope.attachments || [], function (att, i) {
            //        return !!att.FileURL && att.RequirementId == "8daaa73f-5e2f-4f09-9f56-e9941604d9e6";
            //}) == 0) {
            //    errors.push("请上传Agreement");
            //}
            //if ($.grep($scope.attachments || [], function (att, i) {
            //        return !!att.FileURL && att.RequirementId == "23a0aa44-7c4d-42d6-b64a-31c14afb0370";
            //}) == 0) {
            //    errors.push("请上传Others");
            //}

            if (errors.length > 0) {
                messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
            }
            return errors.length == 0;
            //if ($scope.LegalClearanceReport.CreatorNameENUS == "" && !$scope.AssetActor) {
            //    messager.showMessage("请上传Legal Clearance Report", "fa-warning c_orange");
            //    return false;
            //} else if ($scope.Agreement.CreatorNameENUS == "" && !$scope.AssetActor) {
            //    messager.showMessage("请上传Agreement", "fa-warning c_orange");
            //    return false;
            //} else if ($scope.Others.CreatorNameENUS == "" && !$scope.AssetActor) {
            //    messager.showMessage("请上传Others", "fa-warning c_orange");
            //    return false;
            //}
            ////else if ($scope.legalView.Comments == null) {
            ////    messager.showMessage("[[[请先填写意见]]]!", "fa-warning c_orange");
            ////    return false;
            ////}
            //return true;
        }
        $scope.save = function () {

            save("save");

        }
        $scope.submit = function (frm) {
            if (frm.$valid) {
                if (checkValidate()) {
                    approveDialogService.open($scope.projectId, $scope.subFlowCode).then(function (approverInfo) {
                        $scope.approveDialogSubmit(approverInfo);
                    });
                }
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
            majorLeaseService.resubmitMajorLegalReview($scope.legalView).$promise.then(function () {
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
            if ($.grep($scope.attachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "8daaa73f-5e2f-4f09-9f56-e9941604d9e6";
            }) == 0) {
                messager.showMessage("[[[请上传Agreement]]]", "fa-warning c_orange");
                return false;
            }

            if (!$scope.legalView.LegalComments) {
                messager.showMessage("[[[请填写LegalComments]]]", "fa-warning c_orange");
                return false;
            }
            //if (!$scope.legalView.Comments) {
            //    messager.showMessage("[[[请先填写意见]]]!", "fa-warning c_orange");
            //    return false;
            //}
            $scope.legalView.SerialNumber = $routeParams.SN;

            if (!checkValidate()) {
                return;
            }
            $scope.IsClickApprove = true;
            majorLeaseService.approveMajorLegalReview($scope.legalView).$promise.then(function () {
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
                            majorLeaseService.recallMajorLegalReview($scope.legalView).$promise.then(function () {
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
                        majorLeaseService.editMajorLegalReview($scope.legalView).$promise.then(function (response) {
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
            majorLeaseService.resubmitMajorLegalReview($scope.legalView).$promise.then(function () {
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
            $scope.IsClickReturn = true;
            $scope.legalView.LastUpdateUserAccount = window.currentUser.Code;
            $scope.legalView.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.legalView.LastUpdateUserNameENUS = window.currentUser.NameENUS;
            $scope.legalView.SerialNumber = $routeParams.SN;
            messager.confirm("[[[确定要进行退回吗？]]]", "fa-warning c_orange").then(function (result) {
                if (result) {
                    majorLeaseService.returnMajorLegalReview($scope.legalView).$promise.then(function () {
                        messager.showMessage("[[[退回成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect($scope.legalView.WorkflowCode, $scope.projectId);
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
    }]).controller('legalReviewProcessCtrl', ['$routeParams', '$location', 'majorLeaseService', function ($routeParams, $location, majorLeaseService) {
        if ($routeParams.ProcInstID
       && $routeParams.SN) {

            majorLeaseService.getProjectId({ procInstId: $routeParams.ProcInstID }).$promise.then(function (response) {
                $location.url($location.url().replace($location.path(), '/MajorLease/LegalReview/View/' + response.ProjectId));
            });

        }
    }
    ]);
