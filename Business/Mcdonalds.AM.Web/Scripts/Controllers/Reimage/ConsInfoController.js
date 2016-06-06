reimageApp.controller('ConsInfoCtrl',
[
    "$scope",
    '$window',
     "$modal",
    '$location',
    '$routeParams',
    "reimageService",
    'approveDialogService',
    'redirectService',
     "messager",
    function ($scope, $window, $modal, $location, $routeParams, reimageService, approveDialogService, redirectService, messager) {
        $scope.pageType = $routeParams.PageType;
        $scope.projectId = $routeParams.projectId;
        $scope.entityId = $routeParams.entityId;
        $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);
        switch ($routeParams.PageType) {
            case 'Approval':
            case 'View':
                $scope.isPageEditable = false;
                break;
            default:
                $scope.isPageEditable = true;
                $scope.uploadSet = ['8154b2d4-a58c-47e0-8cb0-6c2b6d067f82', '1fffae8b-aa9d-4c6e-a141-8d84f09f89b8'];
                break;
        }

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
        $scope.isShowReinvenstmentCost = true;
        $scope.isHistory = $routeParams.isHistory;
        $scope.subFlowCode = "Reimage_ConsInfo";
        $scope.consInfo = {};
        $scope.consInfo.ReinvenstmentType = null;
        $scope.consInfo.ReinBasicInfo = {};
        $scope.consInfo.WriteOff = {};
        $scope.consInfo.ReinCost = {};
        $scope.consInfo.ProjectId = $scope.projectId;
        $scope.entity = {
            ProjectId: $routeParams.projectId,
            CreateUserAccount: window.currentUser.Code
        };

        $scope.flowCode = "Reimage_ConsInfo";
        $scope.checkPointRefresh = true;

        reimageService.getReimageInfo({ projectId: $routeParams.projectId }).$promise.then(function (response) {
            //debugger;
            $scope.ReimageInfo = response;

            if (response.USCode) {
                $scope.ReimageInfo.USCode = response.USCode;
            }
        });

        $scope.deleteAttachmentFinish = function (id, requirementId) {
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
                            url: Utils.ServiceURI.Address() + "api/attachment/delete"

                        }).success(function (data) {
                            loadContract();
                        }).error(function (e) {

                        });
                    } else {
                        $http.get(Utils.ServiceURI.Address() + "api/Reimage/DeleteAttachement", {
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

        $scope.submit = function (frm) {
            if (checkValidate() && frm.$valid) {
                approveDialogService.open($scope.projectId,
                      $scope.subFlowCode, "",
                      $scope.consInfo.USCode,
                      $scope.consInfo).then(function (approverInfo) {
                          $scope.approveDialogSubmit(approverInfo);
                      });

            }
        };

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

            if ($.grep($scope.attachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "d2de2578-aca7-4b37-84f0-327446f82abf";
            }) == 0) {
                errors.push("请上传FA Write - off Tool附件！");
            }
            if ($.grep($scope.attachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "abe155bb-7e7f-466c-8db0-94be0f9f6f53";
            }) == 0) {
                errors.push("请上传Reinvestment Cost！");
            }
            if ($.grep($scope.attachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "1fffae8b-aa9d-4c6e-a141-8d84f09f89b8";
            }) == 0) {
                errors.push("[[[请上传Store Layout]]]！");
            }
            //if ($.grep($scope.attachments || [], function (att, i) {
            //        return !!att.FileURL && att.RequirementId == "f6f08648-d107-403f-a2ab-ea3a95ff0a7c";
            //}) == 0) {
                //if ($scope.consInfo
                //    && $scope.consInfo.ReinvenstmentType != 1) {
                //    errors.push("请上传New Store Layout！");
                //}
            //}
            //if ($.grep($scope.attachments || [], function (att, i) {
            //        return !!att.FileURL && att.RequirementId == "8154b2d4-a58c-47e0-8cb0-6c2b6d067f82";
            //}) == 0) {
            //    errors.push("请上传Facade,sinage rendering！");
            //}
            if (errors.length > 0) {
                messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
            }
            return errors.length == 0;
            //if ($scope.WriteOffAgreement == null) {
            //    messager.showMessage("[[[请上传Write Off Agreement]]]", "fa-warning c_orange");
            //    return false;
            //} else if ($scope.InvestAgreement == null) {
            //    messager.showMessage("请上传Reinvestment Cost", "fa-warning c_orange");
            //    return false;
            //}           
            //return true;
        }

        $scope.approveDialogSubmit = function (notifyUsersInfo) {
            $scope.consInfo.AppUsers = notifyUsersInfo;
            if ($scope.pageType == "Resubmit") {
                resubmit();
            } else {
                save("submit");
            }
        };

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
                           reimageService.recallConsInfo($scope.consInfo).$promise.then(function () {
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
                        reimageService.editConsInfo($scope.consInfo).$promise.then(function (response) {
                            $window.location.href = Utils.ServiceURI.WebAddress() + response.TaskUrl;
                        }, function () {
                            messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                        });
                    }
                });
        }

        var resubmit = function () {
            $scope.IsClickSubmit = true;
            $scope.consInfo.SerialNumber = $routeParams.SN;
            $scope.consInfo.LastUpdateUserAccount = window.currentUser.Code;
            $scope.consInfo.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.consInfo.LastUpdateUserNameENUS = window.currentUser.NameENUS;

            messager.confirm("[[[确定要提交吗？]]]", "fa-warning c_orange").then(function (result) {
                if (result) {
                    reimageService.resubmitConsInfo($scope.consInfo).$promise.then(function () {
                        messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect($scope.consInfo.WorkflowCode, $scope.projectId);
                        });
                    }, function (error) {
                        messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
                        $scope.IsClickSubmit = false;
                    });
                }
                else
                    $scope.IsClickSubmit = false;
            });
        };
        $scope.save = function () {
            save("save");
        }
        $scope.saveReinvestmentBasicInfo = function (redirectUrl) {
            save("save", redirectUrl);

        }
        var loadContract = function () {
            if ($scope.consInfo.Id == null) {
                loadConsInfo().then(function () {
                    if ($scope.consInfo.Id == null || $scope.consInfo.Id == "00000000-0000-0000-0000-000000000000")
                    { return; }

                    loadInvestCost();
                    loadWriteOff();
                });
            } else {

                loadInvestCost();
                loadWriteOff();
            }
        }
        var loadInvestCost = function () {

            reimageService.getInvestCost({ refTableId: $scope.consInfo.Id }).$promise.then(function (cost) {

                $scope.consInfo.ReinCost = cost;



            }, function (attError) {
                messager.showMessage(attError.statusText, "fa-warning c_orange");
            });
        }

        var loadWriteOff = function () {
            reimageService.getWriteOff({ refTableId: $scope.consInfo.Id }).$promise.then(function (writeoff) {

                $scope.consInfo.WriteOff = writeoff;
            }, function (attError) {
                messager.showMessage(attError.statusText, "fa-warning c_orange");
            });
        }
        $scope.uploadFAFinish = function () {
            //debugger;
            //$scope.consInfo.ProjectId = $scope.projectId;
            //reimageService.saveConsInfo($scope.consInfo).$promise.then(function (data) {                    
            //loadConsInfo();
            loadContract();
            uploadSuccess();
            //}, function (error) {
            //    messager.showMessage("[[[保存失败]]]" + error.message, "fa-warning c_orange");
            //});        

        };

        $scope.uploadWOFinish = function () {
            // $scope.consInfo.ProjectId = $scope.projectId;
            //reimageService.saveConsInfo($scope.consInfo).$promise.then(function (data) {
            //    loadConsInfo();
            loadContract();
            uploadSuccess();
            //}, function (error) {
            //     messager.showMessage("[[[保存失败]]]" + error.message, "fa-warning c_orange");

            //});

        };

        var uploadSuccess = function () {
            messager.showMessage("[[[上传成功]]]", "fa-warning c_orange");
            $scope.$broadcast('AttachmentUploadFinish');
        };




        var loadConsInfo = function () {
            reimageService.getReimageInfo({ projectId: $scope.projectId, entityId: $scope.entityId }).$promise.then(function (data) {

                if (data != null && data != "00000000-0000-0000-0000-000000000000") {
                    $scope.reimageInfo = data;
                }
            },
            function (error) {
                messager.showMessage(error.statusText + "error in loadConsInfo", "fa-warning c_orange");
            });
            return reimageService.getConsInfo({ projectId: $scope.projectId, entityId: $scope.entityId }).$promise.then(function (data) {
                if (data != null) {

                    $scope.consInfo = data;
                    if (data.IsShowSave && $scope.pageType == 'View') {
                        $scope.isPageEditable = $scope.isPageEditable || data.IsShowSave;
                    }

                }
            },
            function (error) {
                messager.showMessage(error.statusText, "fa-warning c_orange");
            });
        }

        loadContract();

        var save = function (action, redirectUrl) {

            $scope.consInfo.projectId = $scope.projectId;

            if (action == "save") {
                $scope.IsClickSave = true;
                reimageService.saveConsInfo($scope.consInfo).$promise.then(function (data) {
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
                messager.confirm("[[[确定要提交吗？]]]", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        reimageService.submitConsInfo($scope.consInfo).$promise.then(function (data) {
                            messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                                redirectService.flowRedirect($scope.consInfo.WorkflowCode, $scope.projectId);
                            });
                        }, function (error) {
                            messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
                            $scope.IsClickSubmit = false;
                        });
                    }
                    else
                        $scope.IsClickSubmit = false;
                });
            } else { return; }
        }

        $scope.edit = function () {
            messager.confirm("[[[Cons Info 审批流程已经完成，现在编辑会导致流程重新提交，是否确认？]]]", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        reimageService.editConsInfo($scope.consInfo).$promise.then(function (response) {
                            $window.location.href = Utils.ServiceURI.WebAddress() + response.TaskUrl;
                        }, function () {
                            messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                        });
                    }
                });
        }

        $scope.approve = function () {
            $scope.IsClickApprove = true;
            $scope.consInfo.SerialNumber = $routeParams.SN;
            $scope.consInfo.LastUpdateUserAccount = window.currentUser.Code;
            $scope.consInfo.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.consInfo.LastUpdateUserNameENUS = window.currentUser.NameENUS;
            messager.confirm("[[[确定要进行审批吗？]]]", "fa-warning c_orange").then(function (result) {
                if (result) {
                    reimageService.approveConsInfo($scope.consInfo).$promise.then(function () {
                        messager.showMessage("[[[审批成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect($scope.consInfo.WorkflowCode, $scope.projectId);
                        });
                    }, function (error) {
                        messager.showMessage("[[[审批成功]]]" + error.message, "fa-warning c_orange");
                        $scope.IsClickApprove = false;
                    });
                }
                else
                    $scope.IsClickApprove = false;
            });
        }


        $scope.returnToOriginator = function () {
            if (!$scope.consInfo.Comments) {
                messager.showMessage("[[[请先填写意见!]]]", "fa-warning c_orange");
                return false;
            }
            $scope.IsClickReturn = true;
            $scope.consInfo.SerialNumber = $routeParams.SN;
            $scope.consInfo.LastUpdateUserAccount = window.currentUser.Code;
            $scope.consInfo.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.consInfo.LastUpdateUserNameENUS = window.currentUser.NameENUS;
            messager.confirm("[[[确定要进行退回吗？]]]", "fa-warning c_orange").then(function (result) {
                if (result) {
                    reimageService.returnConsInfo($scope.consInfo).$promise.then(function () {
                        messager.showMessage("[[[退回成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect($scope.consInfo.WorkflowCode, $scope.projectId);
                        });
                    }, function (error) {
                        messager.showMessage("[[[退回失败]]]" + error.message, "fa-warning c_orange");
                        $scope.IsClickReturn = false;
                    });
                }
                else
                    $scope.IsClickReturn = false;
            });
            return true;
        }
    }
]);