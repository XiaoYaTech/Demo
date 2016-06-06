marjorLeaseApp.controller("leaseChangePackageController", [
    "$http",
    "$scope",
    "$window",
    "$modal",
    "$location",
    "$routeParams",
    "majorLeaseService",
    "approveDialogService",
    'redirectService',
    "messager",
    function ($http, $scope, $window, $modal, $location, $routeParams, majorLeaseService, approveDialogService, redirectService, messager) {
        $scope.pageType = $routeParams.PageType;
        $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);
        $scope.action = $routeParams.action;
        $scope.uploadSet = [];
        switch ($routeParams.PageType) {
            case 'Upload':
                $scope.isPageEditable = false;
                $scope.uploadSet = ['3a21b40e-25c4-49b2-8971-47fb4062793e'];
                break;
            case 'Approval':
            case 'View':
                $scope.isPageEditable = false;
                break;
            default:
                $scope.isPageEditable = true;
                break;
        }




        $scope.$watch("isPageEditable", function (val) {
            if (val) {
                $scope.uploadSet = ['f16c18ee-cf4d-4e08-815e-0c142adebe03', 'daa2badf-b2ee-4a83-a57e-913634390d14'];
            }
        });
        $scope.IsShowMaterTracking = true;
        $scope.userAccount = window.currentUser.Code;
        $scope.projectId = $routeParams.projectId;
        $scope.entityId = $routeParams.entityId;
        $scope.isHistory = $routeParams.isHistory;
        $scope.subFlowCode = "MajorLease_Package";
        $scope.checkPointRefresh = true;
        $scope.packageInfo = {};
        $scope.Attachment = {};
        $scope.KeyMeasures = [];
        $scope.WarningWOClass = "";
        $scope.WarningNGClass = "";
        $scope.IsKeyMeasureValid = false;
        var populateKeyMeasures = function () {
            return "packageInfo.WriteOff + " +
                "packageInfo.CashCompensation +" +
                "packageInfo.NewInvestment + " +
                "packageInfo.CashFlowNVPCurrent +" +
                "packageInfo.CashFlowNVPAfterChange+" +
                "packageInfo.OtherCompensation";
        };
        var uploadSuccess = function () {
            messager.showMessage("[[[上传成功]]]", "fa-warning c_orange");
        };
        var loadAttachment = function () {
            if ($scope.packageInfo.Id == null)
            { return; }
            majorLeaseService.getPackageAgreementList({ refTableId: $scope.packageInfo.Id, projectId: $scope.projectId }).$promise.then(function (atts) {
                if (atts != null) {
                    for (var i = 0; i < atts.length; i++) {
                        var att = atts[i];
                        if (!att.IsExist) {
                            att.FileURL = window.location.href;
                        }
                    }
                    $scope.Attachment = atts;
                }
            }, function (attError) {
                messager.showMessage(attError.statusText + "error in loadAttachment", "fa-warning c_orange");
            });
        };

        var loadPackageInfo = function () {
            majorLeaseService.getPackageInfo({ projectId: $scope.projectId, entityId: $scope.entityId }).$promise.then(function (data) {
                if (data != null && data != "00000000-0000-0000-0000-000000000000") {
                    $scope.packageInfo = data;

                    if (data.IsShowSave && $scope.pageType == 'View') {
                        $scope.isPageEditable = $scope.isPageEditable || data.IsShowSave;
                    }
                    if (data.EnableAssetMgrUpload) {
                        $scope.uploadSet.push('3a21b40e-25c4-49b2-8971-47fb4062793e');
                    }
                    loadAttachment();
                }
            },
                function (error) {
                    messager.showMessage(error.statusText + "error in loadPackageInfo", "fa-warning c_orange");
                });
        }
        loadPackageInfo();
        $scope.$watch(populateKeyMeasures(), function () {
            var a = $scope.packageInfo.WriteOff;
            var b = $scope.packageInfo.CashCompensation;
            var c = $scope.packageInfo.CashFlowNVPAfterChange;
            //var d = $scope.packageInfo.OtherCompensation;
            var d = 0;
            var e = $scope.packageInfo.NewInvestment;
            var f = $scope.packageInfo.CashFlowNVPCurrent;

            if (a != null && b != null) {
                try {
                    $scope.packageInfo.NetWriteOff = parseFloat((parseFloat(a) - parseFloat(b)).toFixed(2));
                    if ($scope.packageInfo.NetWriteOff < 0) {
                        //$scope.WarningWOClass = "warning";
                        $scope.WarningWOClass = "";
                    } else {
                        $scope.WarningWOClass = "";
                    }
                    $scope.IsKeyMeasureValid = true;
                } catch (e) {
                    messager.showMessage("[[[请输入数字]]]", "fa-warning c_orange");
                    $scope.IsKeyMeasureValid = false;
                }
            } else {
                $scope.IsKeyMeasureValid = false;
            }
            if (b != null && c != null && d != null && e != null && f != null) {
                try {
                    $scope.packageInfo.NetGain = parseFloat((parseFloat(b) + parseFloat(c) + parseFloat(d) - parseFloat(e) - parseFloat(f)).toFixed(2));
                    if ($scope.packageInfo.NetGain < 0) {
                        $scope.WarningNGClass = "warning";
                    } else {
                        $scope.WarningNGClass = "";
                    }
                    $scope.IsKeyMeasureValid = true;
                } catch (e) {
                    messager.showMessage("[[[请输入数字]]]", "fa-warning c_orange");
                    $scope.IsKeyMeasureValid = false;
                }
            } else {
                $scope.IsKeyMeasureValid = false;
            }
        }, true);
        $scope.packageAttachment = function () {
            if ($scope.packageInfo.Id == null) {
                $scope.packageInfo.Id = "00000000-0000-0000-0000-000000000000";
            }
            $scope.packageInfo.projectId = $scope.projectId;

            majorLeaseService.generateZipFile($scope.packageInfo).$promise.then(function (data) {
                if (data != null && data.fileName != "") {
                    var redirectUrl = Utils.ServiceURI.Address() + "api/MajorLease/LeaseChangePackage/DownloadPackage?fileName=" + data.fileName;
                    $window.location.href = redirectUrl;
                }
                loadPackageInfo();
            }, function (error) {
                messager.showMessage(error.message + "in packageAttachment", "fa-warning c_orange");
            });
        };
        $scope.uploadFinished = function () {
            loadAttachment();
            uploadSuccess();
        };
        var save = function (action, redirectUrl) {
            if ($scope.packageInfo.Id == null) {
                $scope.packageInfo.Id = "00000000-0000-0000-0000-000000000000";
            }
            $scope.packageInfo.projectId = $scope.projectId;

            if (!$scope.packageInfo.ChangeRentalType) {
                $scope.packageInfo.ChangeRentalTypeDESC = "";
            }
            if (!$scope.packageInfo.ChangeRedLineType) {
                $scope.packageInfo.ChangeRedLineTypeDESC = "";
            }
            if (!$scope.packageInfo.ChangeLeaseTermType) {
                $scope.packageInfo.ChangeLeaseTermDESC = "";
            }

            if (action == "save") {
                $scope.IsClickSave = true;
                majorLeaseService.savePackageInfo($scope.packageInfo).$promise.then(function (data) {
                    loadPackageInfo();
                    messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                }, function (error) {
                    messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
                });
            } else if (action == "submit") {
                $scope.IsClickSubmit = true;
                majorLeaseService.submitPackageInfo($scope.packageInfo).$promise.then(function (data) {
                    messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                        redirectService.flowRedirect($scope.packageInfo.WorkflowCode, $scope.projectId);
                    });
                }, function (error) {
                    messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
                });
            } else {
                return;
            }
        };
        var checkValidate = function () {
            if (!$scope.IsKeyMeasureValid) {
                messager.showMessage("[[[请填写正确的Key Measures 信息]]]", "fa-warning c_orange");
                return false;
            }
            if (!$scope.packageInfo.ReasonDesc && $scope.packageInfo.NetGain < 0) {
                messager.showMessage("[[[请填写Negative Net Cashflow NPV Reason Description]]]", "fa-warning c_orange");
                return false;
            }
            //else if ($scope.packageInfo.OtherCompenDesc == null) {
            //    messager.showMessage("请填写Other Compensation Description", "fa-warning c_orange");
            //    return false;
            //}
            if (!$scope.packageInfo.DecisionLogicRecomendation) {
                messager.showMessage("[[[请填写Decision Logic And Recomendation]]]", "fa-warning c_orange");
                return false;
            }
            return true;
        };
        $scope.save = function () {

            save("save");

        };
        var resubmit = function () {
            $scope.packageInfo.SerialNumber = $routeParams.SN;

            majorLeaseService.resubmitPackageInfo($scope.packageInfo).$promise.then(function () {
                messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                    redirectService.flowRedirect($scope.packageInfo.WorkflowCode, $scope.projectId);
                });
            }, function (error) {
                messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
            });
        };
        $scope.submit = function (frm) {
            if (frm.$valid) {
                //if ($scope.packageInfo.Comments == null) {
                //    messager.showMessage("[[[请填写Comments]]]", "fa-warning c_orange");
                //    return;
                //}
                if (checkValidate()) {
                    approveDialogService.open($scope.projectId, $scope.subFlowCode, null, $scope.packageInfo.MajorInfo.USCode, $scope.packageInfo).then(function (approverInfo) {
                        $scope.approveDialogSubmit(approverInfo);
                    });
                }
            }
        };
        var hasUploadAttachment = function (typeCode) {
            var hasUpload = false;
            if ($scope.Attachment) {

                for (var i = 0; i < $scope.Attachment.length; i++) {
                    var item = $scope.Attachment[i];
                    if (item.TypeCode &&
                        item.TypeCode == typeCode
                        && item.IsExist) {
                        hasUpload = true;
                        break;
                    }
                }
            }

            return hasUpload;
        }

        $scope.confirm = function () {
            $scope.IsClickConfrim = true;
            $scope.packageInfo.SerialNumber = $routeParams.SN;
            if ($.grep($scope.attachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "3a21b40e-25c4-49b2-8971-47fb4062793e";
            }) == 0) {
                messager.showMessage("[[[请先上传 Signed Approval 附件!]]]", "fa-warning c_orange");
                return false;
            }
            //if (!hasUploadAttachment('SignedAgreement')) {
            //    messager.showMessage("请先上传 SignedAgreement 附件!", "fa-warning c_orange");
            //    return false;
            //}
            majorLeaseService.confirmPackageInfo($scope.packageInfo).$promise.then(function () {
                messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                    redirectService.flowRedirect($scope.packageInfo.WorkflowCode, $scope.projectId);
                });
            }, function (error) {
                messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
            });
            return true;
        }
        $scope.approveDialogSubmit = function (notifyUsersInfo) {
            $scope.packageInfo.AppUsers = notifyUsersInfo;
            if ($scope.pageType == "Resubmit") {
                resubmit();
            } else {
                save("submit");
            }
        };

        $scope.approve = function () {
            $scope.IsClickApprove = true;
            $scope.packageInfo.SerialNumber = $routeParams.SN;

            majorLeaseService.approvePackageInfo($scope.packageInfo).$promise.then(function () {
                messager.showMessage("[[[审批成功]]]", "fa-check c_green").then(function () {
                    redirectService.flowRedirect($scope.packageInfo.WorkflowCode, $scope.projectId);
                });
            }, function (error) {
                messager.showMessage("[[[审批成功]]]" + error.message, "fa-warning c_orange");
            });
        }

        $scope.reject = function () {
            if (!$scope.packageInfo.Comments) {
                messager.showMessage("[[[请填写Comments]]]", "fa-warning c_orange").then(function () {
                    window.parent.scrollTo(0, $("#divComments textarea").offset().top);
                });
                return false;
            }
            $scope.IsClickReject = true;
            $scope.packageInfo.SerialNumber = $routeParams.SN;

            messager.confirm("[[[Reject后该流程将直接终止，请确认需要执行该操作吗？]]]", "fa-warning c_orange")
                    .then(function (result) {
                        if (result) {
                            majorLeaseService.rejectPackageInfo($scope.packageInfo).$promise.then(function () {
                                messager.showMessage("[[[拒绝成功]]]", "fa-check c_green").then(function () {
                                    redirectService.flowRedirect($scope.packageInfo.WorkflowCode, $scope.projectId);
                                });
                            }, function (error) {
                                messager.showMessage("[[[拒绝失败]]]" + error.message, "fa-warning c_orange");
                            });
                        } else {
                            $scope.IsClickReject = false;
                        }
                    });
        }

        $scope.recall = function () {
            $scope.packageInfo.SerialNumber = $routeParams.SN;
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
                            //$scope.packageInfo.ProcInstID = $routeParams.ProcInstID;
                            $scope.packageInfo.Comments = entity.Comment;
                            majorLeaseService.recallPackageInfo($scope.packageInfo).$promise.then(function () {
                                messager.showMessage("[[[撤回成功]]]", "fa-check c_green").then(function () {
                                    redirectService.flowRedirect($scope.packageInfo.WorkflowCode, $scope.projectId);
                                });
                            }, function (error) {
                                messager.showMessage("[[[撤回失败]]]" + error.message, "fa-warning c_orange");
                            });
                        });
                    }
                });

        }

        $scope.edit = function () {
            messager.confirm("[[[Lease Change Package 审批流程已经完成，现在编辑会导致流程重新提交，是否确认？]]]", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        majorLeaseService.editPackageInfo($scope.packageInfo).$promise.then(function (response) {
                            $window.location.href = Utils.ServiceURI.WebAddress() + response.TaskUrl;
                        }, function () {
                            messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                        });
                    }
                });
        }
        $scope.returnToOriginator = function () {
            if (!$scope.packageInfo.Comments) {
                messager.showMessage("[[[请填写Comments]]]", "fa-warning c_orange").then(function () {
                    window.parent.scrollTo(0, $("#divComments textarea").offset().top);
                });
                return false;
            }
            $scope.IsClickReturn = true;
            $scope.packageInfo.SerialNumber = $routeParams.SN;
            $scope.packageInfo.LastUpdateUserAccount = window.currentUser.Code;
            $scope.packageInfo.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.packageInfo.LastUpdateUserNameENUS = window.currentUser.NameENUS;
            messager.confirm("[[[确定要进行退回吗？]]]", "fa-warning c_orange").then(function (result) {
                if (result) {
                    majorLeaseService.returnPackageInfo($scope.packageInfo).$promise.then(function () {
                        messager.showMessage("[[[退回成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect($scope.packageInfo.WorkflowCode, $scope.projectId);
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