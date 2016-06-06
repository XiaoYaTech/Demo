rebuildApp.controller("rebuildPackageController", [
    "$http",
    "$scope",
    "$window",
    "$modal",
    "$location",
    "$routeParams",
    "rebuildService",
    "approveDialogService",
    "redirectService",
    "messager",
    function ($http, $scope, $window, $modal, $location, $routeParams, rebuildService, approveDialogService, redirectService, messager) {
        $scope.pageType = $routeParams.PageType;
        $scope.action = $routeParams.action;
        $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);
        switch ($routeParams.PageType) {
            case 'Upload':
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
        $scope.subFlowCode = "Rebuild_Package";
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
                "packageInfo.CashFlowNVPAfterChange"; //+
            //"packageInfo.OtherCompensation";
        };
        var uploadSuccess = function () {
            messager.showMessage("[[[上传成功]]]", "fa-warning c_orange");
        };
        var loadAttachment = function () {
            var refTableId = $scope.packageInfo.Id;
            if (refTableId == null) {
                loadPackageInfo(false).then(function () {
                    if ($scope.packageInfo.Id == null || $scope.packageInfo.Id == "00000000-0000-0000-0000-000000000000") {
                        refTableId = null;
                    }
                });
            }
            rebuildService.getPackageAgreementList({ refTableId: refTableId, projectId: $scope.projectId }).$promise.then(function (atts) {
                if (atts != null) {
                    $scope.Attachment = atts;
                    angular.forEach(atts, function (r, i) {
                        if (r.ID != "00000000-0000-0000-0000-000000000000") {
                            r.downloadLink = Utils.ServiceURI.Address() + "api/attachment/download?id=" + r.ID;
                            switch (r.Extension.toLowerCase()) {
                                case ".xlsx":
                                case ".xls":
                                    r.Icon = "fa fa-file-excel-o c_green";
                                    break;
                                case ".ppt":
                                    r.Icon = "fa fa-file-powerpoint-o c_red";
                                    break;
                                case ".doc":
                                case ".docx":
                                    r.Icon = "fa fa-file-word-o c_blue";
                                    break;
                                default:
                                    r.Icon = "fa fa-file c_orange";
                                    break;
                            }
                        }
                        else if (r.TypeCode == "SignedApproval"
                            && r.ID == "00000000-0000-0000-0000-000000000000"
                            && r.Required) {
                            r.ID = null;
                        }
                    });
                }
            }, function (attError) {
                messager.showMessage(attError.statusText + "error in loadAttachment", "fa-warning c_orange");
            });
        };

        var loadPackageInfo = function (isNeedLoadAttach) {
            return rebuildService.getPackageInfo({ projectId: $scope.projectId, entityId: $scope.entityId }).$promise.then(function (data) {
                if (data != null && isNeedLoadAttach && data.Id != "00000000-0000-0000-0000-000000000000") {
                    $scope.packageInfo = data;
                }
                if (data != null) {
                    $scope.packageInfo.USCode = data.USCode;
                    $scope.packageInfo.RbdInfo = data.RbdInfo;
                    if (data.TempClosureDate != null && data.TempClosureDate != "") {
                        $scope.packageInfo.TempClosureDate = moment(data.TempClosureDate).toDate();
                    }
                    if (data.ReopenDate != null && data.ReopenDate != "") {
                        $scope.packageInfo.ReopenDate = moment(data.ReopenDate).toDate();
                    }

                    if (data.OldChangeLeaseTermExpiraryDate != null) {
                        $scope.packageInfo.OldChangeLeaseTermExpiraryDate = moment(data.OldChangeLeaseTermExpiraryDate).toDate();
                    }
                    if (data.OldChangeRedLineRedLineArea != null) {
                        $scope.packageInfo.OldChangeRedLineRedLineArea = data.OldChangeRedLineRedLineArea;
                    }
                    if (data.OldLandlord != null) {
                        $scope.packageInfo.OldLandlord = data.OldLandlord;
                    }
                    if (data.OldRentalStructure != null) {
                        $scope.packageInfo.OldRentalStructure = data.OldRentalStructure;
                    }
                    if (data.PCR != null) {
                        $scope.packageInfo.PCR = data.PCR;
                    }
                    $scope.packageInfo.IsAssetMgr = data.IsAssetMgr;
                    $scope.packageInfo.IsProjectFreezed = data.IsProjectFreezed;
                    $scope.packageInfo.AppUsers = data.AppUsers;
                    $scope.packageInfo.WriteOff = data.WriteOff;
                    $scope.packageInfo.NewInvestment = data.NewInvestment;
                    if (data.Id != "00000000-0000-0000-0000-000000000000") {
                        $scope.packageInfo.Id = data.Id;
                    }
                    if (data.IsShowSave && $scope.pageType == 'View') {
                        $scope.isPageEditable = $scope.isPageEditable || data.IsShowSave;
                        $scope.packageInfo.IsShowSave = data.IsShowSave;
                    }
                    $scope.packageInfo.WorkflowCode = data.WorkflowCode;
                }
                if (isNeedLoadAttach) {
                    loadAttachment();
                }
            },
                function (error) {
                    messager.showMessage(error.statusText + "error in loadPackageInfo", "fa-warning c_orange");
                });
        }
        loadPackageInfo(true);
        $scope.$watch(populateKeyMeasures(), function () {
            var a = $scope.packageInfo.WriteOff;
            var b = $scope.packageInfo.CashCompensation;
            var c = $scope.packageInfo.CashFlowNVPAfterChange;
            //var d = $scope.packageInfo.OtherCompensation;
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
            if (b != null && c != null && e != null && f != null) {
                try {
                    $scope.packageInfo.NetGain = parseFloat((parseFloat(b) + parseFloat(c) - parseFloat(e) - parseFloat(f)).toFixed(2));
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
            $scope.packageInfo.LastUpdateUserAccount = window.currentUser.Code;
            $scope.packageInfo.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.packageInfo.LastUpdateUserNameENUS = window.currentUser.NameENUS;

            rebuildService.generateZipFile($scope.packageInfo).$promise.then(function (data) {
                if (data != null && data.fileName != "") {
                    var redirectUrl = Utils.ServiceURI.Address() + "api/Rebuild/RebuildPackage/DownloadPackage?fileName=" + data.fileName;
                    $window.location.href = redirectUrl;
                    //$scope.packDownloadLink = redirectUrl;
                }
                loadPackageInfo(true);
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
            $scope.packageInfo.LastUpdateUserAccount = window.currentUser.Code;
            $scope.packageInfo.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.packageInfo.LastUpdateUserNameENUS = window.currentUser.NameENUS;

            if (!$scope.packageInfo.ChangeRentalType) {
                $scope.packageInfo.NewRentalStructure = "";
            }
            if (!$scope.packageInfo.ChangeRedLineType) {
                $scope.packageInfo.NewChangeRedLineRedLineArea = "";
            }
            if (!$scope.packageInfo.ChangeLeaseTermType) {
                $scope.packageInfo.NewChangeLeaseTermExpiraryDate = "";
            }
            if (!$scope.packageInfo.ChangeLandlordType) {
                $scope.packageInfo.NewLandlord = "";
            }
            if (!$scope.packageInfo.ChangeOtherType) {
                $scope.packageInfo.Others = "";
            }
            if ($scope.packageInfo.NewRentalStructure == ""
                && $scope.packageInfo.NewChangeRedLineRedLineArea == ""
                && $scope.packageInfo.NewChangeLeaseTermExpiraryDate == ""
                && $scope.packageInfo.NewLandlord == ""
                && $scope.packageInfo.Others == "") {
                $scope.packageInfo.LeaseChangeDescription = "";
            }

            if (action == "save") {
                $scope.IsClickSave = true;
                rebuildService.savePackageInfo($scope.packageInfo).$promise.then(function (data) {
                    loadPackageInfo(true);
                    messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                }, function (error) {
                    messager.showMessage("[[[保存失败]]]" + error.message, "fa-warning c_orange");
                });
            } else if (action == "submit") {
                $scope.IsClickSubmit = true;
                rebuildService.submitPackageInfo($scope.packageInfo).$promise.then(function (data) {
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
            if (($scope.packageInfo.ReasonDesc == null || $scope.packageInfo.ReasonDesc == "") && $scope.packageInfo.NetGain < 0) {
                messager.showMessage("[[[请填写Negative Net Cashflow NPV Reason Description]]]", "fa-warning c_orange");
                return false;
            }
            if ($scope.packageInfo.DecisionLogicRecomendation == null || $scope.packageInfo.DecisionLogicRecomendation == "") {
                messager.showMessage("[[[请填写Decision Logic And Recomendation]]]", "fa-warning c_orange");
                return false;
            }
            if ($scope.packageInfo.TempClosureDate != null && $scope.packageInfo.TempClosureDate != "") {
                var tmpDate = moment($scope.packageInfo.TempClosureDate);
                if (tmpDate.isBefore(new Date().toDateString())) {
                    messager.showMessage("[[[TempClosure Date 不能早于今天]]]", "fa-warning c_orange");
                    return false;
                }
            }
            if ($scope.packageInfo.TempClosureDate != null && $scope.packageInfo.TempClosureDate != ""
                && $scope.packageInfo.ReopenDate != null && $scope.packageInfo.ReopenDate != "") {
                var tmpDate = moment($scope.packageInfo.TempClosureDate);
                var reopDate = moment($scope.packageInfo.ReopenDate);
                if (tmpDate.isBefore(new Date().toDateString())) {
                    messager.showMessage("[[[TempClosure Date 不能早于今天]]]", "fa-warning c_orange");
                    return false;
                }
                if (reopDate.isBefore(tmpDate)) {
                    messager.showMessage("[[[Re-open Date 不能早于 TempClosure Date]]]", "fa-warning c_orange");
                    return false;
                }
            }
            if (($scope.packageInfo.ChangeRentalType == null || !$scope.packageInfo.ChangeRentalType)
                && ($scope.packageInfo.ChangeRedLineType == null || !$scope.packageInfo.ChangeRedLineType)
                && ($scope.packageInfo.ChangeLeaseTermType == null || !$scope.packageInfo.ChangeLeaseTermType)
                && ($scope.packageInfo.ChangeLandlordType == null || !$scope.packageInfo.ChangeLandlordType)
                && ($scope.packageInfo.ChangeOtherType == null || !$scope.packageInfo.ChangeOtherType)) {
                messager.showMessage("[[[请填写Lease Change due to Rebuild]]]", "fa-warning c_orange");
                return false;
            }
            return true;
        };
        $scope.save = function () {
            save("save");
        };
        var resubmit = function () {
            $scope.packageInfo.SerialNumber = $routeParams.SN;
            $scope.packageInfo.LastUpdateUserAccount = window.currentUser.Code;
            $scope.packageInfo.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.packageInfo.LastUpdateUserNameENUS = window.currentUser.NameENUS;
            $scope.IsClickSubmit = true;
            rebuildService.resubmitPackageInfo($scope.packageInfo).$promise.then(function () {
                messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                    redirectService.flowRedirect($scope.packageInfo.WorkflowCode, $scope.projectId);
                });
            }, function (error) {
                $scope.IsClickSubmit = false;
                messager.showMessage("[[[提交失败]]]" + error.message, "fa-warning c_orange");
            });
        };
        $scope.submit = function (frm) {
            //if ($scope.packageInfo.Comments == null) {
            //    messager.showMessage("[[[请填写Comments]]]", "fa-warning c_orange");
            //    return;
            //}
            if (!checkValidate()) {
                return;
            }
            if (!frm.$valid) {
                return;
            }
            approveDialogService.open($scope.projectId,
                $scope.subFlowCode, "",
                $scope.packageInfo.RbdInfo.USCode,
                $scope.packageInfo).then(function (approverInfo) {
                    $scope.approveDialogSubmit(approverInfo);
                });
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
            $scope.packageInfo.SerialNumber = $routeParams.SN;
            if (!hasUploadAttachment('SignedApproval')) {
                messager.showMessage("[[[请先上传 Signed Approval 附件]]]!", "fa-warning c_orange");
                return false;
            }
            //if (!hasUploadAttachment('SignedAgreement')) {
            //    messager.showMessage("请先上传 SignedAgreement 附件!", "fa-warning c_orange");
            //    return false;
            //}
            //$scope.approve('cfm');
            $scope.IsClickApprove = true;
            rebuildService.confirmPackageInfo($scope.packageInfo).$promise.then(function () {
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

        $scope.approve = function (action) {
            $scope.IsClickApprove = true;
            $scope.packageInfo.SerialNumber = $routeParams.SN;
            $scope.packageInfo.LastUpdateUserAccount = window.currentUser.Code;
            $scope.packageInfo.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.packageInfo.LastUpdateUserNameENUS = window.currentUser.NameENUS;
            rebuildService.approvePackageInfo($scope.packageInfo).$promise.then(function () {
                var msg = "[[[审批成功]]]";
                if (action == "cfm")
                { msg = "[[[提交成功]]]"; }
                messager.showMessage(msg, "fa-check c_green").then(function () {
                    redirectService.flowRedirect($scope.packageInfo.WorkflowCode, $scope.projectId);
                });
            }, function (error) {
                $scope.IsClickApprove = false;
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
            $scope.packageInfo.LastUpdateUserAccount = window.currentUser.Code;
            $scope.packageInfo.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.packageInfo.LastUpdateUserNameENUS = window.currentUser.NameENUS;
            messager.confirm("[[[Reject后该流程将直接终止，请确认需要执行该操作吗]]]？", "fa-warning c_orange")
                    .then(function (result) {
                        if (result) {
                            rebuildService.rejectPackageInfo($scope.packageInfo).$promise.then(function () {
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
                            $scope.packageInfo.Comments = entity.Comment;
                            rebuildService.recallPackageInfo($scope.packageInfo).$promise.then(function () {
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
            messager.confirm("[[[Rebuild Package 审批流程已经完成，现在编辑会导致流程重新提交，是否确认]]]？", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        rebuildService.editPackageInfo($scope.packageInfo).$promise.then(function (response) {
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
            messager.confirm("[[[确认需要Return吗？]]]", "fa-warning c_orange").then(function (result) {
                if (result) {
                    $scope.IsClickReturn = true;
                    $scope.packageInfo.SerialNumber = $routeParams.SN;
                    $scope.packageInfo.LastUpdateUserAccount = window.currentUser.Code;
                    $scope.packageInfo.LastUpdateUserNameZHCN = window.currentUser.NameZHCN;
                    $scope.packageInfo.LastUpdateUserNameENUS = window.currentUser.NameENUS;
                    rebuildService.returnPackageInfo($scope.packageInfo).$promise.then(function () {
                        messager.showMessage("[[[退回成功]]]", "fa-check c_green").then(function () {
                            redirectService.flowRedirect($scope.packageInfo.WorkflowCode, $scope.projectId);
                        });
                    }, function (error) {
                        messager.showMessage("[[[退回失败]]]" + error.message, "fa-warning c_orange");
                    });
                }
            });
            return true;
        }

        $scope.open = function ($event, ele) {
            $event.preventDefault();
            $event.stopPropagation();

            $scope[ele] = true;
        };

        $scope.dateOptions = {
            formatYear: 'yy',
            startingDay: 0
        };
        $scope.minDate = new Date();
        $scope.$watch("packageInfo.TempClosureDate", function (val) {
            if (val != null && val != "") {
                $scope.minReopenDate = $scope.packageInfo.TempClosureDate;
            } else {
                $scope.minReopenDate = $scope.minDate;
            }
        });

        $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
        $scope.format = $scope.formats[1];
    }
]);