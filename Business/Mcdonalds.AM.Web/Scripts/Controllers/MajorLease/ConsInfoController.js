marjorLeaseApp.controller("consInfoController", [
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
        switch ($routeParams.PageType) {
            case 'Approval':
            case 'View':
                $scope.isPageEditable = false;
                break;
            default:
                $scope.isPageEditable = true;

                break;
        }
        $scope.isRedLine = false;
        $scope.IsShowMaterTracking = true;
        $scope.projectId = $routeParams.projectId;
        $scope.entityId = $routeParams.entityId;
        $scope.isHistory = $routeParams.isHistory;
        $scope.subFlowCode = "MajorLease_ConsInfo";
        $scope.checkPointRefresh = true;
        $scope.Amount = 0;

        $scope.uploadSet = [];
        $scope.$watch("isPageEditable", function (val) {
            if (val) {
                $scope.uploadSet = ['4e87c7ea-c4fd-4030-b73a-591d3459c3f6', '72481d16-8a06-45c1-adfb-cb44fbf6fe0f'];
            }
        });

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
        $scope.consInfo.ReinvenstmentType = null;
        $scope.consInfo.ReinBasicInfo = {};
        $scope.consInfo.WriteOff = {};
        $scope.consInfo.ReinCost = {};

        $scope.$watch("consInfo.ReinvenstmentType", function (val) {
            if (val == 1) {//No Reinvenstment 
                $scope.isShowReinvenstmentBasicInfo = false;
                $scope.isShowReinvenstmentCost = false;
                $scope.isShowWriteOffAmount = false;
                $scope.isShowReinvstAndWriteOff = false;
                $scope.requireSet = ['4e87c7ea-c4fd-4030-b73a-591d3459c3f6'];
            } else if (val == 2) {//Less than 50K
                $scope.isShowReinvenstmentBasicInfo = true;
                $scope.isShowReinvstAndWriteOff = true;
                $scope.isShowReinvenstmentCost = false;
                $scope.isShowWriteOffAmount = false;
                $scope.requireSet = [];
            }
            else if (val == 3) {// More than 50k
                $scope.isShowReinvenstmentBasicInfo = true;
                $scope.isShowReinvenstmentCost = true;
                $scope.isShowWriteOffAmount = true;
                $scope.isShowReinvstAndWriteOff = false;
                $scope.requireSet = [];
            }
        });
        var checkAmount = function (val) {
            try {
                var total = parseFloat(val);
                var am = parseFloat($scope.Amount);
                if (total > (am * 1000)) {
                    //messager.showMessage("Total Reinvestment must Less Than 50K USD(RMB 310K)", "fa-warning c_orange");
                    messager.showMessage("[[[Reinvestment超过310K，请选择‘More Than 50K USD（RMB310K）’进行填写]]]", "fa-warning c_orange");
                    return false;
                } else
                    return true;
            } catch (e) {
                messager.showMessage(e.message, "fa-warning c_orange");
                return false;
            }
        };
        //$scope.$watch("consInfo.ReinCost.TotalReinvestmentNorm", function(val) {
        //    if (!val) {
        //        return;
        //    }
        //    checkAmount(val);
        //});
        majorLeaseService.getMajorInfo({ projectId: $scope.projectId }).$promise.then(function (data) {
            if (data == null)
            { return; }
            $scope.MajorLeaseInfo = data;
            if ($scope.MajorLeaseInfo.ChangeRedLineType) {
                $scope.isRedLine = true;
            }
        }, function (error) {
            messager.showMessage(error.statusText, "fa-warning c_orange");
        });

        var loadConsInfo = function () {
            return majorLeaseService.getConsInfo({ projectId: $scope.projectId, entityId: $scope.entityId }).$promise.then(function (data) {
                if (data != null) {
                    var tmpReInvst = $scope.consInfo.ReinvenstmentType;
                    if (data.Id != "00000000-0000-0000-0000-000000000000") {
                        $scope.consInfo = data;
                    }

                    if (data.IsShowSave && $scope.pageType == 'View') {
                        $scope.isPageEditable = $scope.isPageEditable || data.IsShowSave;
                    }
                    if (data.ReinBasicInfo) {
                        $scope.consInfo.ReinBasicInfo = data.ReinBasicInfo;
                    }
                    if (data.ReinvenstmentType) {
                        $scope.consInfo.ReinvenstmentType = data.ReinvenstmentType;
                    }
                    if (tmpReInvst) {
                        $scope.consInfo.ReinvenstmentType = tmpReInvst;
                    }
                }
            },
            function (error) {
                messager.showMessage(error.statusText, "fa-warning c_orange");
            });
        }
        var loadContract = function () {
            if ($scope.consInfo.Id == null) {
                loadConsInfo().then(function () {
                    if ($scope.consInfo.Id == null || $scope.consInfo.Id == "00000000-0000-0000-0000-000000000000")
                    { return; }
                    //majorLeaseService.getConsInfoAgreementList({ refTableId: $scope.consInfo.Id, typeCode: "" }).$promise.then(function (atts) {
                    //    if (atts != null)
                    //    { populateAttach(atts); }
                    //}, function (attError) {
                    //    messager.showMessage(attError.statusText, "fa-warning c_orange");
                    //});
                    loadInvestCost();
                    loadWriteOff();
                });
            } else {
                //majorLeaseService.getConsInfoAgreementList({ refTableId: $scope.consInfo.Id, typeCode: "" }).$promise.then(function (atts) {
                //    if (atts != null)
                //    { populateAttach(atts); }
                //}, function (attError) {
                //    messager.showMessage(attError.statusText, "fa-warning c_orange");
                //});
                loadInvestCost();
                loadWriteOff();
            }
        }
        var loadInvestCost = function () {
            if ($scope.consInfo.ReinvenstmentType != 3)
            { return; }
            majorLeaseService.getInvestCost({ refTableId: $scope.consInfo.Id }).$promise.then(function (cost) {
                //cost.UploadDate = new Date();
                //cost.UploadedBy = window.currentUser.NameENUS;
                $scope.consInfo.ReinCost = cost;
            }, function (attError) {
                messager.showMessage(attError.statusText, "fa-warning c_orange");
            });
        }
        var loadWriteOff = function () {
            if ($scope.consInfo.ReinvenstmentType != 3)
            { return; }
            majorLeaseService.getWriteOff({ refTableId: $scope.consInfo.Id }).$promise.then(function (writeoff) {
                //writeoff.UploadDate = new Date();
                //writeoff.UploadedBy = window.currentUser.NameENUS;
                $scope.consInfo.WriteOff = writeoff;
            }, function (attError) {
                messager.showMessage(attError.statusText, "fa-warning c_orange");
            });
        }
        loadContract();
        var uploadSuccess = function () {
            $scope.$broadcast('AttachmentUploadFinish');
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
            uploadSuccess();
        };
        $scope.uploadWOFinish = function () {
            loadContract();
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
                        $http.get(Utils.ServiceURI.Address() + "api/MajorLease/DeleteAttachement", {
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
                majorLeaseService.saveConsInfo($scope.consInfo).$promise.then(function (data) {
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
                if ($scope.consInfo.ReinvenstmentType == 1) {
                    $scope.consInfo.ReinBasicInfo = {};
                    $scope.consInfo.WriteOff = {};
                    $scope.consInfo.ReinCost = {};
                }
                $scope.IsClickSubmit = true;
                majorLeaseService.submitConsInfo($scope.consInfo).$promise.then(function (data) {
                    messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                        redirectService.flowRedirect($scope.consInfo.WorkflowCode, $scope.projectId);
                    });
                }, function (error) {
                    messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
                });
            } else { return; }
        }
        var checkValidate = function () {
            var errors = [];

            var gbDate = $scope.consInfo.ReinBasicInfo.GBDate;
            var consDate = $scope.consInfo.ReinBasicInfo.ConsCompletionDate;
            var reopDate = $scope.consInfo.ReinBasicInfo.ReopenDate;

            gbDate = moment(gbDate);
            consDate = moment(consDate);
            reopDate = moment(reopDate);
            if (gbDate.isValid
                && consDate.isValid
                && consDate.isBefore(gbDate)) {
                errors.push("[[[Construction Completion Date 不能早于 GB Date]]]");
            }
            if (reopDate.isValid
                && consDate.isValid
                && reopDate.isBefore(consDate)) {
                errors.push("[[[Reopen Date 不能早于 Construction Completion Date]]]");
            }

            if (!$scope.consInfo.ReinvenstmentType) {
                messager.showMessage('[[[请选择Reinvestment Amount Type]]]', 'fa-warning c_orange');
                return false;
            }
            if ($scope.consInfo.ReinvenstmentType != 1) {
                if ($.grep($scope.attachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "4e87c7ea-c4fd-4030-b73a-591d3459c3f6";
                }) == 0) {
                    errors.push("[[[请上传Store Layout]]]");
                }
            }

            if ($scope.consInfo.ReinvenstmentType == 3) {
                if ($.grep($scope.attachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "c485cd90-10f0-4af6-b666-d9b06ef15bc0";
                }) == 0) {
                    errors.push("[[[请上传Write Off Agreement]]]");
                }

                if ($.grep($scope.attachments || [], function (att, i) {
            return !!att.FileURL && att.RequirementId == "87152dc6-c6c6-40a5-addf-f9c4a7be21ee";
                }) == 0) {
                    errors.push("[[[请上传Investment Cost]]]");
                }
            }


            if (errors.length > 0) {
                messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
                return false;
            }
                //if ($scope.ConsAgreement.CreatorNameENUS == "" && $scope.isRedLine) {
                //    messager.showMessage("[[[请上传Store Layout]]]", "fa-warning c_orange");
                //    return false;
                //} else if ($scope.WriteOffAgreement.CreatorNameENUS == "" && $scope.consInfo.ReinvenstmentType == 3 && $scope.isRedLine) {
                //    messager.showMessage("[[[请上传Write Off Agreement]]]", "fa-warning c_orange");
                //    return false;
                //} else if ($scope.InvestAgreement.CreatorNameENUS == "" && $scope.consInfo.ReinvenstmentType == 3 && $scope.isRedLine) {
                //    messager.showMessage("[[[请上传Investment Cost]]]", "fa-warning c_orange");
                //    return false;
                //}
            else if ($scope.consInfo.ReinvenstmentType == 2) {
                if ($scope.consInfo.ReinCost == null) {
                    messager.showMessage("[[[请填写Reinvestment Cost]]]", "fa-warning c_orange");
                    return false;
                }
                if ($scope.consInfo.WriteOff == null) {
                    messager.showMessage("[[[请填写Write Off Amount]]]", "fa-warning c_orange");
                    return false;
                }
                return checkAmount($scope.consInfo.ReinCost.TotalReinvestmentNorm);
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
            if ($scope.consInfo.ReinvenstmentType == 1) {
                $scope.consInfo.ReinBasicInfo = {};
                $scope.consInfo.WriteOff = {};
                $scope.consInfo.ReinCost = {};
            }
            majorLeaseService.resubmitConsInfo($scope.consInfo).$promise.then(function () {
                messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                    redirectService.flowRedirect($scope.consInfo.WorkflowCode, $scope.projectId);
                });
            }, function (error) {
                messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
            });
        };
        $scope.submit = function (frm) {
            //if ($scope.consInfo.Comments == null && $scope.isRedLine) {
            //    messager.showMessage("[[[请填写Comments]]]", "fa-warning c_orange");
            //    return;
            //}
            if (checkValidate()
                && frm.$valid) {
                if ($scope.consInfo.ReinvenstmentType == 1) {
                    //selected first radio button,we do not need approve users
                    save("submit");
                } else {
                    approveDialogService.open($scope.projectId, $scope.consInfo.WorkflowCode).then(function (approverInfo) {
                        $scope.approveDialogSubmit(approverInfo);
                    });
                }
            }

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
            majorLeaseService.approveConsInfo($scope.consInfo).$promise.then(function () {
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
                           $scope.consInfo.Comments = entity.Comment;
                           majorLeaseService.recallConsInfo($scope.consInfo).$promise.then(function () {
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
                        majorLeaseService.editConsInfo($scope.consInfo).$promise.then(function (response) {
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
            $scope.IsClickReturn = true;
            $scope.consInfo.SerialNumber = $routeParams.SN;
            $scope.consInfo.LastUpdateUserAccount = window.currentUser.Code;
            $scope.consInfo.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.consInfo.LastUpdateUserNameENUS = window.currentUser.NameENUS;
            messager.confirm("[[[确定要进行退回吗？]]]", "fa-warning c_orange").then(function (result) {
                if (result) {
                    majorLeaseService.returnConsInfo($scope.consInfo).$promise.then(function () {
                        messager.showMessage("[[[退回成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect($scope.consInfo.WorkflowCode, $scope.projectId);
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

    }
]);