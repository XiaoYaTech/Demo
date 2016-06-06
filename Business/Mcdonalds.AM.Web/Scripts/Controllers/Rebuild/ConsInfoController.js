rebuildApp.controller("consInfoController", [
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
        switch ($routeParams.PageType) {
            case 'Approval':
            case 'View':
                $scope.isPageEditable = false;
                break;
            default:
                $scope.isPageEditable = true;
                break;
        }
        $scope.IsShowMaterTracking = true;
        $scope.userAccount = window.currentUser.Code;
        $scope.projectId = $routeParams.projectId;
        $scope.entityId = $routeParams.entityId;
        $scope.isHistory = $routeParams.isHistory;
        $scope.subFlowCode = "Rebuild_ConsInfo";
        $scope.checkPointRefresh = true;
        $scope.Amount = 0;

        if ($scope.isPageEditable) {
            $scope.isEditAmountType = true;
            $scope.isEditComments = true;
            $scope.isEditReinvenstmentBasicInfo = true;
            $scope.isEditReinvstAndWriteOff = true;
        } else {
            $scope.isEditAmountType = false;
            $scope.isEditComments = false;
            $scope.isEditReinvenstmentBasicInfo = false;
            $scope.isEditReinvstAndWriteOff = false;
        }

        $scope.isShowReinvenstmentBasicInfo = true;
        $scope.isShowReinvenstmentCost = true;
        $scope.isShowWriteOffAmount = true;
        $scope.isShowReinvstAndWriteOff = true;

        $scope.userNameENUS = window.currentUser.NameENUS;
        $scope.userNameZHCN = window.currentUser.NameZHCN;
        $scope.displayName = window.currentUser.NameENUS;
        $scope.uploadDate = new Date();

        $scope.consInfo = {};
        $scope.consInfo.ReinBasicInfo = {};
        $scope.consInfo.WriteOff = {};
        $scope.consInfo.ReinCost = {};

        rebuildService.getRebuildInfo({ projectId: $scope.projectId }).$promise.then(function (data) {
            if (data == null)
            { return; }
            $scope.RebuildInfo = data;
        }, function (error) {
            messager.showMessage(error.statusText, "fa-warning c_orange");
        });

        var loadConsInfo = function () {
            return rebuildService.getConsInfo({ projectId: $scope.projectId, entityId: $scope.entityId }).$promise.then(function (data) {
                if (data != null) {
                    if (data.Id != "00000000-0000-0000-0000-000000000000") {
                        $scope.consInfo = data;
                        
                        if (data.IsShowSave && $scope.pageType == 'View') {
                            $scope.isPageEditable = $scope.isPageEditable || data.IsShowSave;
                        }
                    }
                }
            },
            function (error) {
                messager.showMessage(error.statusText, "fa-warning c_orange");
            });
        }
        var populateAttach = function (atts) {
            var arrContract = new Array();
            $scope.ConsAgreement = null;
            $scope.Agreement = null;
            $scope.WriteOffAgreement = null;
            $scope.InvestAgreement = null;
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
                { arrContract.push(o); }
                else if (o.TypeCode == "ReinCost")
                { $scope.InvestAgreement = o; }
                else if (o.TypeCode == "WriteOff")
                { $scope.WriteOffAgreement = o; }
                else if (o.TypeCode == "ConsInfoAgreement")
                { $scope.ConsAgreement = o; }
            });
            $scope.Attachment = arrContract;
        }
        var loadContract = function () {
            if ($scope.consInfo.Id == null) {
                loadConsInfo().then(function () {
                    if ($scope.consInfo.Id == null || $scope.consInfo.Id == "00000000-0000-0000-0000-000000000000")
                    { return; }
                    rebuildService.getConsInfoAgreementList({ refTableId: $scope.consInfo.Id, typeCode: "" }).$promise.then(function (atts) {
                        if (atts != null)
                        { populateAttach(atts); }
                    }, function (attError) {
                        messager.showMessage(attError.statusText, "fa-warning c_orange");
                    });
                    //loadInvestCost();
                    //loadWriteOff();
                });
            } else {
                rebuildService.getConsInfoAgreementList({ refTableId: $scope.consInfo.Id, typeCode: "" }).$promise.then(function (atts) {
                    if (atts != null)
                    { populateAttach(atts); }
                }, function (attError) {
                    messager.showMessage(attError.statusText, "fa-warning c_orange");
                });
                //loadInvestCost();
                //loadWriteOff();
            }
        }
        var loadInvestCost = function () {
            rebuildService.getInvestCost({ refTableId: $scope.consInfo.Id }).$promise.then(function (cost) {
                $scope.consInfo.ReinCost = cost;
            }, function (attError) {
                messager.showMessage(attError.statusText, "fa-warning c_orange");
            });
        }
        var loadWriteOff = function () {
            rebuildService.getWriteOff({ refTableId: $scope.consInfo.Id }).$promise.then(function (writeoff) {
                $scope.consInfo.WriteOff = writeoff;
            }, function (attError) {
                messager.showMessage(attError.statusText, "fa-warning c_orange");
            });
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
        $scope.uploadFAFinish = function () {
            loadContract();
            loadInvestCost();
            uploadSuccess();
        };
        $scope.uploadWOFinish = function () {
            loadContract();
            loadWriteOff();
            uploadSuccess();
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
                            url: Utils.ServiceURI.Address() + "api/attachment/delete"

                        }).success(function (data) {
                            loadContract();
                        }).error(function (e) {

                        });
                    } else {
                        $http.get(Utils.ServiceURI.Address() + "api/Rebuild/DeleteAttachement", {
                            params: {
                                attId: attId,
                                projectId: $scope.projectId,
                                flowCode: "Consinfo"
                            }
                        }).success(function (data) {
                            loadContract();
                            $scope.checkPointRefresh = true;
                        });
                    }
                }
            });
        }

        var save = function (action, redirectUrl) {
            if ($scope.consInfo.Id == null) {
                $scope.consInfo.Id = "00000000-0000-0000-0000-000000000000";
            }
            $scope.consInfo.projectId = $scope.projectId;
            $scope.consInfo.LastUpdateUserAccount = window.currentUser.Code;
            $scope.consInfo.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.consInfo.LastUpdateUserNameENUS = window.currentUser.NameENUS;

            if (action == "save") {
                $scope.IsClickSave = true;
                rebuildService.saveConsInfo($scope.consInfo).$promise.then(function (data) {
                    loadConsInfo();
                    if (redirectUrl) {
                        $window.location.href = redirectUrl;
                    } else {
                        messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                    }
                }, function (error) {
                    messager.showMessage("[[[保存失败]]]" + error.message, "fa-warning c_orange");
                });
            }
            else if (action == "submit") {
                $scope.IsClickSubmit = true;
                rebuildService.submitConsInfo($scope.consInfo).$promise.then(function (data) {
                    messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                        redirectService.flowRedirect($scope.consInfo.WorkflowCode, $scope.projectId);
                    });
                }, function (error) {
                    messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
                });
            } else { return; }
        }
        var checkValidate = function () {
            var gbDate = $scope.consInfo.ReinBasicInfo.GBDate;
            var consDate = $scope.consInfo.ReinBasicInfo.ConsCompletionDate;
            var reopDate = $scope.consInfo.ReinBasicInfo.ReopenDate;

            if (gbDate != null && consDate != null) {
                gbDate = moment(moment(gbDate).format("YYYY-MM-DD"));
                consDate = moment(moment(consDate).format("YYYY-MM-DD"));
                //if (gbDate.isBefore(new Date().toDateString())) {
                //    messager.showMessage("GB Date 不能早于今天", "fa-warning c_orange");
                //    return false;
                //}

                if (consDate.isBefore(gbDate)) {
                    messager.showMessage("[[[Construction Completion Date 不能早于 GB Date]]]", "fa-warning c_orange");
                    return false;
                }
            }
            if (reopDate != null && consDate != null) {
                reopDate = moment(moment(reopDate).format("YYYY-MM-DD"));
                if (reopDate.isBefore(consDate)) {
                    messager.showMessage("[[[Reopen Date 不能早于 Construction Completion Date]]]", "fa-warning c_orange");
                    return false;
                }
            }
            if ($scope.ConsAgreement == null) {
                messager.showMessage("[[[请上传Store Layout]]]", "fa-warning c_orange");
                return false;
            } else if ($scope.WriteOffAgreement == null) {
                messager.showMessage("[[[请上传Write Off Agreement]]]", "fa-warning c_orange");
                return false;
            } else if ($scope.InvestAgreement == null) {
                messager.showMessage("[[[请上传Investment Cost]]]", "fa-warning c_orange");
                return false;
            }
            return true;
        }
        $scope.save = function () {
            save("save");
        }
        var resubmit = function () {
            $scope.IsClickSubmit = true;
            $scope.consInfo.SerialNumber = $routeParams.SN;
            $scope.consInfo.LastUpdateUserAccount = window.currentUser.Code;
            $scope.consInfo.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.consInfo.LastUpdateUserNameENUS = window.currentUser.NameENUS;
            rebuildService.resubmitConsInfo($scope.consInfo).$promise.then(function () {
                messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                    redirectService.flowRedirect($scope.consInfo.WorkflowCode, $scope.projectId);
                });
            }, function (error) {
                messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
            });
        };
        $scope.submit = function (frm) {
            if (!checkValidate()) {
                return;
            }
            if (!frm.$valid) {
                return;
            }
            approveDialogService.open($scope.projectId, $scope.subFlowCode, "", "", $scope.consInfo).then(function (approverInfo) {
                $scope.approveDialogSubmit(approverInfo);
            });
        }

        $scope.approveDialogSubmit = function (notifyUsersInfo) {
            $scope.consInfo.AppUsers = notifyUsersInfo;
            if ($scope.pageType == "Resubmit") {
                resubmit();
            } else {
                save("submit");
            }
        };

        $scope.saveReinvestmentBasicInfo = function (redirectUrl) {
            save("save", redirectUrl);
        }

        $scope.approve = function () {
            $scope.IsClickApprove = true;
            $scope.consInfo.SerialNumber = $routeParams.SN;
            $scope.consInfo.LastUpdateUserAccount = window.currentUser.Code;
            $scope.consInfo.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.consInfo.LastUpdateUserNameENUS = window.currentUser.NameENUS;
            rebuildService.approveConsInfo($scope.consInfo).$promise.then(function () {
                messager.showMessage("[[[审批成功]]]", "fa-check c_green").then(function () {
                    redirectService.flowRedirect($scope.consInfo.WorkflowCode, $scope.projectId);
                });
            }, function (error) {
                messager.showMessage("[[[审批成功]]]" + error.message, "fa-warning c_orange");
            });
        }

        $scope.recall = function () {
            $scope.consInfo.SerialNumber = $routeParams.SN;
            $scope.consInfo.LastUpdateUserAccount = window.currentUser.Code;
            $scope.consInfo.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.consInfo.LastUpdateUserNameENUS = window.currentUser.NameENUS;

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
                           $scope.consInfo.Comments = entity.Comment;
                           rebuildService.recallConsInfo($scope.consInfo).$promise.then(function () {
                               messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                                   redirectService.flowRedirect($scope.consInfo.WorkflowCode, $scope.projectId);
                               });
                           }, function (error) {
                               messager.showMessage("[[[撤回失败]]]" + error.message, "fa-warning c_orange");
                           });
                       });
                   }
               });
        }

        $scope.edit = function () {
            messager.confirm("[[[Cons Info 审批流程已经完成，现在编辑会导致流程重新提交，是否确认？]]]", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        rebuildService.editConsInfo($scope.consInfo).$promise.then(function (response) {
                            $window.location.href = Utils.ServiceURI.WebAddress() + response.TaskUrl;
                        }, function () {
                            messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                        });
                    }
                });
        }

        $scope.returnToOriginator = function () {
            if (!$scope.consInfo.Comments) {
                messager.showMessage("[[[请先填写意见]]]!", "fa-warning c_orange");
                return false;
            }
            messager.confirm("[[[确认需要Return吗？]]]", "fa-warning c_orange").then(function (result) {
                if (result) {
                    $scope.IsClickReturn = true;
                    $scope.consInfo.SerialNumber = $routeParams.SN;
                    $scope.consInfo.LastUpdateUserAccount = window.currentUser.Code;
                    $scope.consInfo.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
                    $scope.consInfo.LastUpdateUserNameENUS = window.currentUser.NameENUS;
                    rebuildService.returnConsInfo($scope.consInfo).$promise.then(function () {
                        messager.showMessage("[[[退回成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect($scope.consInfo.WorkflowCode, $scope.projectId);
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
    }
]);