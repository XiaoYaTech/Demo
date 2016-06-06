marjorLeaseApp.controller("consInvtCheckingController", ["$http",
    "$scope",
    "$window",
    "$modal",
    "$location",
    "$routeParams",
    "majorLeaseService",
    'approveDialogService',
    "redirectService",
    "messager",
    function ($http, $scope, $window, $modal, $location, $routeParams, majorLeaseService, approveDialogService,redirectService, messager) {
        $scope.pageType = $routeParams.PageType;
        $scope.isUserAction = Utils.Common.isUserAction($scope.from);
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
        $scope.subFlowCode = "MajorLease_ConsInvtChecking";
        $scope.checkPointRefresh = true;
        $scope.displayName = window.currentUser.NameENUS;
        $scope.uploadDate = new Date();
        $scope.ConsInvtChecking = {};
        $scope.ConsInfo = {};

        var loadConsInfo = function () {
            return majorLeaseService.getConsInfo({ projectId: $scope.projectId }).$promise.then(function (data) {
                if (data != null && data != "NULL") {
                    $scope.ConsInfo = data;

                    loadCheckingInfo();
                }
            },
            function (error) {
                messager.showMessage(error.statusText, "fa-warning c_orange");
            });
        }

        var loadCheckingInfo = function () {
            return majorLeaseService.getConsInvtChecking({ projectId: $scope.projectId, entityId: $scope.entityId }).$promise.then(function (data) {

                $scope.ConsInvtChecking = data;
                if (data.IsShowSave && $scope.pageType == 'View') {
                    $scope.isPageEditable = $scope.isPageEditable || data.IsShowSave;
                }

                loadInvestCost();
                loadWriteOff();
            },
            function (error) {
                messager.showMessage(error.statusText + "getConsInvtChecking", "fa-warning c_orange");
            });
        }

        var loadInvestCost = function () {
            if ($scope.ConsInvtChecking.Id == null)
            { return; }
            majorLeaseService.getInvestCost({ refTableId: $scope.ConsInvtChecking.Id }).$promise.then(function (cost) {
                $scope.ReinCost = cost;
            }, function (attError) {
                messager.showMessage(attError.statusText + "loadInvestCost", "fa-warning c_orange");
            });
        }
        var loadWriteOff = function () {
            if ($scope.ConsInvtChecking.Id == null)
            { return; }
            majorLeaseService.getWriteOff({ refTableId: $scope.ConsInvtChecking.Id }).$promise.then(function (writeoff) {
                $scope.WriteOff = writeoff;
            }, function (attError) {
                messager.showMessage(attError.statusText + "loadWriteOff", "fa-warning c_orange");
            });
        }
        var uploadSuccess = function () {
            messager.showMessage("[[[上传成功]]]", "fa-warning c_orange");
            if (!$scope.ConsInvtChecking.ApprovalType)
                loadConsInfo();
        };
        $scope.uploadWOFinish = function () {
            loadWriteOff();
            uploadSuccess();
        };
        $scope.uploadFAFinish = function () {
            loadInvestCost();
            uploadSuccess();
        };
        loadConsInfo();
        var save = function (action) {
            if ($scope.ConsInvtChecking.Id == null) {
                $scope.ConsInvtChecking.Id = "00000000-0000-0000-0000-000000000000";
            }
            $scope.ConsInvtChecking.ProjectId = $scope.projectId;
            $scope.ConsInvtChecking.LastUpdateUserAccount = window.currentUser.Code;
            $scope.ConsInvtChecking.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.ConsInvtChecking.LastUpdateUserNameENUS = window.currentUser.NameENUS;

            if (action == "save") {
                majorLeaseService.saveConsInvtChecking($scope.ConsInvtChecking).$promise.then(function (data) {
                    messager.showMessage("[[[保存成功]]]", "fa-check c_green").then(function () {
                        //$window.location = Utils.ServiceURI.WebAddress() + "/redirect";
                    });
                }, function (error) {
                    messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
                });
            }
            else if (action == "submit") {
                majorLeaseService.submitConsInvtChecking($scope.ConsInvtChecking).$promise.then(function (data) {
                    messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                        redirectService.flowRedirect($scope.ConsInvtChecking.WorkflowCode, $scope.projectId);
                    });
                }, function (error) {
                    messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
                });
            } else { return; }
        }
        var checkValidate = function () {
            if (!$scope.WriteOff.ConsInfoID) {
                messager.showMessage("[[[请上传Write Off Actual Checking]]]", "fa-warning c_orange");
                return false;
            }
            var errors = [];
            if ($scope.WriteOff.REActual && $scope.WriteOff.REWriteOff && !$scope.WriteOff.ExpFAActVsReCost) {
                var recostVar = ActualVSBudget($scope.WriteOff.REActual, $scope.WriteOff.REWriteOff);
                var v = Variance(recostVar, $scope.WriteOff.REWriteOff);
                if (Math.abs(v) > 5) {
                    //messager.showMessage("请填写Explanation if ABS(RE Cost Variance)", "fa-warning c_orange");
                    //messager.showMessage("缺少描述信息，请在模板中填写Description for RE Cost Variance>+-5%后再重新上传", "fa-warning c_orange");
                    errors.push("缺少描述信息，请在WO模板中填写Description for RE Cost Variance>+-5%后再重新上传");
                }
            }
            if ($scope.WriteOff.LHIActual && $scope.WriteOff.LHIWriteOff && !$scope.WriteOff.ExpFAActVsLHI) {
                var recostVar = ActualVSBudget($scope.WriteOff.LHIActual, $scope.WriteOff.LHIWriteOff);
                var v = Variance(recostVar, $scope.WriteOff.LHIWriteOff);
                if (Math.abs(v) > 5) {
                    //messager.showMessage("请填写Explanation if ABS(LHI Variance)", "fa-warning c_orange");
                    //messager.showMessage("缺少描述信息，请在模板中填写Description for LHI Variance>+-5%后再重新上传", "fa-warning c_orange");
                    errors.push("缺少描述信息，请在WO模板中填写Description for LHI Variance>+-5%后再重新上传");
                }
            }
            var tESSD = Total($scope.WriteOff.EquipmentWriteOff, Total($scope.WriteOff.SignageWriteOff, Total($scope.WriteOff.SeatingWriteOff, Total($scope.WriteOff.DecorationWriteOff, null))));
            var tESSDAc = Total($scope.WriteOff.EquipmentActual, Total($scope.WriteOff.SignageActual, Total($scope.WriteOff.SeatingActual, Total($scope.WriteOff.DecorationActual, null))));
            if (tESSD && tESSDAc && !$scope.WriteOff.ExpFAActVsESSD) {
                var recostVar = ActualVSBudget(tESSDAc, tESSD);
                var v = Variance(recostVar, tESSD);
                if (Math.abs(v) > 5) {
                    //messager.showMessage("缺少描述信息，请在模板中填写Description for ESSD Variance>+-5%后再重新上传", "fa-warning c_orange");
                    //return false;
                    errors.push("缺少描述信息，请在WO模板中填写Description for ESSD Variance>+-5%后再重新上传");
                }
            }


            if ($scope.WriteOff.TotalActual && $scope.WriteOff.TotalWriteOff && !$scope.WriteOff.ExpFAActVsTotal) {
                var recostVar = ActualVSBudget($scope.WriteOff.TotalActual, $scope.WriteOff.TotalWriteOff);
                var v = Variance(recostVar, $scope.WriteOff.TotalWriteOff);
                if (Math.abs(v) > 5) {
                    //messager.showMessage("请填写Explanation if ABS(Total Variance)", "fa-warning c_orange");
                    //messager.showMessage("缺少描述信息，请在模板中填写Description for Total Variance>+-5%后再重新上传", "fa-warning c_orange");
                    //return false;
                    errors.push("缺少描述信息，请在WO模板中填写Description for Total Variance>+-5%后再重新上传");
                }
            }
            if (!$scope.ReinCost.ConsInfoID) {
                messager.showMessage("[[[请上传Reinvestment Actual Checking]]]", "fa-warning c_orange");
                return false;
            }

            if ($scope.ReinCost.RECostFAAct && $scope.ReinCost.RECostBudget && !$scope.ReinCost.ExpFAActVsReCost) {
                var recostVarFA = PMActVSBudget($scope.ReinCost.RECostFAAct, $scope.ReinCost.RECostBudget);
                var v = Variance(recostVarFA, $scope.ReinCost.RECostBudget);
                if (Math.abs(v) > 5) {
                    //messager.showMessage("请填写Explanation_FA Actual vs. Budget (RE Cost)", "fa-warning c_orange");
                    //return false;
                    errors.push("缺少描述信息，请在FA模板中填写Explanation_FA Actual vs. Budget (RE Cost)>+-5%后再重新上传");
                }
            }
            if ($scope.ReinCost.LHIFAAct && $scope.ReinCost.LHIBudget && !$scope.ReinCost.ExpFAActVsLHI) {
                var recostVar2FA = PMActVSBudget($scope.ReinCost.LHIFAAct, $scope.ReinCost.LHIBudget);
                var v = Variance(recostVar2FA, $scope.ReinCost.LHIBudget);
                if (Math.abs(v) > 5) {
                    //messager.showMessage("请填写Explanation_FA Actual vs. Budget (LHI)", "fa-warning c_orange");
                    //return false;
                    errors.push("缺少描述信息，请在FA模板中填写Explanation_FA Actual vs. Budget (LHI)>+-5%后再重新上传");
                }
            }
            if ($scope.ReinCost.ESSDFAAct && $scope.ReinCost.ESSDBudget && !$scope.ReinCost.ExpFAActVsESSD) {
                var recostVar17FA = PMActVSBudget($scope.ReinCost.ESSDFAAct, $scope.ReinCost.ESSDBudget)
                var v = Variance(recostVar17FA, $scope.ReinCost.ESSDBudget);
                if (Math.abs(v) > 5) {
                    //messager.showMessage("请填写Explanation_FA Actual vs. Budget (ESSD)", "fa-warning c_orange");
                    //return false;
                    errors.push("缺少描述信息，请在FA模板中填写Explanation_FA Actual vs. Budget (ESSD)>+-5%后再重新上传");
                }
            }
            if ($scope.ReinCost.TotalReinvestmentFAAct && $scope.ReinCost.TotalReinvestmentBudget && !$scope.ReinCost.ExpFAActVsTotal) {
                var recostVar22FA = PMActVSBudget($scope.ReinCost.TotalReinvestmentFAAct, $scope.ReinCost.TotalReinvestmentBudget);
                var v = Variance(recostVar22FA, $scope.ReinCost.TotalReinvestmentBudget);
                if (Math.abs(v) > 5) {
                    //messager.showMessage("请填写Explanation_FA Actual vs Budget", "fa-warning c_orange");
                    //return false;
                    errors.push("缺少描述信息，请在FA模板中填写Explanation_FA Actual vs Budget (Total)>+-5%后再重新上传");
                }
            }
            if (errors.length > 0) {
                messager.showMessage(errors.join("<br />"), "fa-warning c_orange", "md");
                return false;
            }
            return true;
        };

        $scope.save = function () {
            if (checkValidate()) {
                save("save");
            }
        };
        var resubmit = function () {
            $scope.IsClickSubmit = true;
            $scope.ConsInvtChecking.SerialNumber = $routeParams.SN;
            majorLeaseService.resubmitConsInvtChecking($scope.ConsInvtChecking).$promise.then(function () {
                messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                    $window.location = Utils.ServiceURI.WebAddress() + "/redirect";
                });
            }, function (error) {
                messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
            });
        };

        $scope.submit = function (frm) {
            //if ($scope.ConsInvtChecking.Comments == null) {
            //    messager.showMessage("[[[请填写Comments]]]", "fa-warning c_orange");
            //    return;
            //}
            if (checkValidate()
                && frm.$valid) {
                approveDialogService.open($scope.projectId, $scope.subFlowCode, $scope.ConsInvtChecking.ApprovalType).then(function (approverInfo) {
                    $scope.approveDialogSubmit(approverInfo);
                });
            }
        };

        $scope.approveDialogSubmit = function (notifyUsersInfo) {
            $scope.ConsInvtChecking.AppUsers = notifyUsersInfo;
            $scope.IsClickSubmit = true;
            if ($scope.pageType == "Resubmit") {
                resubmit();
            } else {
                save("submit");
            }
        };

        $scope.approve = function () {
            $scope.IsClickApprove = true;
            $scope.ConsInvtChecking.SerialNumber = $routeParams.SN;
            majorLeaseService.approveConsInvtChecking($scope.ConsInvtChecking).$promise.then(function () {
                messager.showMessage("[[[审批成功]]]", "fa-check c_green").then(function () {
                    $window.location = Utils.ServiceURI.WebAddress() + "/redirect";
                });
            }, function (error) {
                messager.showMessage("[[[审批成功]]]" + error.message, "fa-warning c_orange");
            });
        };

        $scope.recall = function () {
            $scope.ConsInvtChecking.SerialNumber = $routeParams.SN;

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
                          $scope.ConsInvtChecking.Comments = entity.Comment;
                          majorLeaseService.recallConsInvtChecking($scope.ConsInvtChecking).$promise.then(function () {
                              messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                                  $window.location = Utils.ServiceURI.WebAddress() + "/redirect";
                              });
                          }, function (error) {
                              messager.showMessage("[[[撤回失败]]]" + error.message, "fa-warning c_orange");
                          });
                      });
                  }
              });


        };

        $scope.edit = function () {
            messager.confirm("[[[Construction Investment Checking 审批流程已经完成，现在编辑会导致流程重新提交，是否确认？]]]", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        majorLeaseService.editConsInvtChecking($scope.ConsInvtChecking).$promise.then(function (response) {
                            messager.showMessage("[[[操作成功]]]", "fa-warning c_orange");
                            $window.location.href = Utils.ServiceURI.WebAddress() + response.TaskUrl;
                        }, function () {
                            messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                        });
                    }
                });
        }
        $scope.returnToOriginator = function () {
            if (!$scope.ConsInvtChecking.Comments) {
                messager.showMessage("[[[请先填写意见]]]!", "fa-warning c_orange");
                return false;
            }
            $scope.IsClickReturn = true;
            $scope.ConsInvtChecking.SerialNumber = $routeParams.SN;
            messager.confirm("[[[确定要进行退回吗？]]]", "fa-warning c_orange").then(function (result) {
                if (result) {
                    majorLeaseService.returnConsInvtChecking($scope.ConsInvtChecking).$promise.then(function () {
                        messager.showMessage("[[[退回成功]]]", "fa-check c_green").then(function () {
                            $window.location = Utils.ServiceURI.WebAddress() + "/redirect";
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
        };
        $scope.IsShowSubmit = true;
        //majorLeaseService.getMajorInfo({ projectId: $scope.projectId }).$promise.then(function (data) {
        //    if (data != null && data.ReopenedDays>30) {
        //        $scope.IsShowSubmit = true;
        //    }
        //});

        var ActualVSBudget = function (FinanceActual, Budget) {
            var returnVal = null;
            try {
                if (FinanceActual != null && Budget != null) {
                    returnVal = (parseFloat(FinanceActual) - parseFloat(Budget));
                }
            } catch (e) {
            }
            return returnVal;
        };
        var Variance = function (actualVSBudget, Budget) {
            var returnVal = null;
            try {
                if (actualVSBudget != null && Budget != null && Budget != 0) {
                    returnVal = ((parseFloat(actualVSBudget) / parseFloat(Budget)) * 100).toFixed(1);
                }
            } catch (e) {
            }
            return returnVal;
        };
        var PMActVSBudget = function (PMActual, Budget) {
            var returnVal = null;
            try {
                if (PMActual != null && Budget != null) {
                    returnVal = (parseFloat(PMActual) - parseFloat(Budget));
                }
            } catch (e) {
            }
            return returnVal;
        };
        var Total = function (val1, val2) {
            var returnVal = null;
            try {
                if (val1 != null && val2 != null) {
                    returnVal = (parseFloat(val1) + parseFloat(val2)).toString();
                }
                else if (val1 != null) {
                    returnVal = val1;
                }
                else if (val2 != null) {
                    returnVal = val2;
                }
            } catch (e) {
            }
            return returnVal;
        };

    }
]);