angular.module("mcd.am.modules")
    .factory("approveDialogService", [
        '$modal',
        'majorLeaseApprovalDialogService',
        "tempClosureApprovalDialogService",
        "closureApprovalDialogService",
        "rebuildApprovalDialogService",
        'reimageApprovalDialogService',
        'renewalApprovalDialogService',
        'notifyApprovalDialogService',
        function ($modal,
            majorLeaseApprovalDialogService,
            tempClosureApprovalDialogService,
            closureApprovalDialogService,
            rebuildApprovalDialogService,
            reimageApprovalDialogService,
            renewalApprovalDialogService,
            notifyApprovalDialogService) {
            return {
                open: function (projectId, flowCode, approvalType, usCode, entity, roleCode) {
                    var result = null;
                    switch (flowCode) {
                        case "MajorLease_ConsInfo":
                        case "MajorLease_Package":
                        case "MajorLease_FinanceAnalysis":
                        case "MajorLease_ConsInvtChecking":
                        case "MajorLease_LegalReview":
                        case "MajorLease_GBMemo":
                            result = majorLeaseApprovalDialogService.open(projectId, flowCode, approvalType, entity, usCode);
                            break;

                        case "TempClosure_ClosurePackage":
                            result = tempClosureApprovalDialogService.open(projectId, usCode);
                            break;

                        case "Closure_ClosurePackage":
                            result = closureApprovalDialogService.open(projectId, flowCode, approvalType, usCode);
                            break;

                        case "Rebuild_ConsInfo":
                        case "Rebuild_Package":
                        case "Rebuild_FinanceAnalysis":
                        case "Rebuild_ConsInvtChecking":
                        case "Rebuild_LegalReview":
                        case "Rebuild_GBMemo":
                            result = rebuildApprovalDialogService.open(projectId, flowCode, approvalType, entity);
                            break;
                        case "Reimage_ConsInfo":
                        case "Reimage_Package":
                        case "Reimage_ConsInvtChecking":
                        case "Reimage_GBMemo":
                            result = reimageApprovalDialogService.open(projectId, flowCode, approvalType, entity, usCode);
                            break;
                        case "Renewal_Letter":
                        case "Renewal_ConsInfo":
                        case "Renewal_Tool":
                        case "Renewal_LegalApproval":
                        case "Renewal_Package":
                        case "Renewal_GBMemo":
                            result = renewalApprovalDialogService.open(projectId, flowCode, approvalType, usCode);
                            break;

                        case "TempClosure_ClosureMemo":
                        case "TempClosure_ReopenMemo":
                            result = notifyApprovalDialogService.open(projectId, flowCode, usCode, roleCode);
                            break;
                    }
                    return result;
                }
            }
        }
    ]).factory("majorLeaseApprovalDialogService", [
        '$modal',
        'projectUsersService',
        'flowService',
        "messager",
        function ($modal, projectUsersService, flowService, messager) {
            return {
                open: function (projectId, flowCode, approvalType, entity, usCode) {
                    return $modal.open({
                        backdrop: 'static',
                        templateUrl: "/MajorLease/ApproveDialog",
                        size: 'lg',
                        resolve: {
                            notifyUsersInfo: function () {
                                return {
                                    MCCLAssetMgr: null,
                                    GM: null,
                                    NoticeUsers: []
                                }
                            }
                        },
                        controller: [
                            "$scope", "$modalInstance", "$selectUser", "notifyUsersInfo", function (noticeScope, $modalInstance, $selectUser, notifyUsersInfo) {
                                noticeScope.projectId = projectId;
                                noticeScope.subFlowName = '';
                                noticeScope.subFlowCode = flowCode;
                                noticeScope.ApprovalType = approvalType;
                                noticeScope.notifyUsersInfo = notifyUsersInfo;
                                noticeScope.isShowCC = false;
                                noticeScope.isShowCDOFOMD = false;
                                noticeScope.isNecessaryLoading = true;
                                var checkIfShowCDOFOMD = function (data) {
                                    if (data
                                        && data.NetWriteOff) {
                                        try {
                                            if (parseFloat(entity.NetWriteOff) >= (61.1 * 10000)) {
                                                noticeScope.isShowCDOFOMD = true;
                                            }
                                        } catch (e) {
                                        }
                                    }
                                }
                                if (approvalType == 'ProjectList') {
                                    //noticeScope.isShowCDOFOMD = true;
                                    flowService.getFlowInfo({ projectId: noticeScope.projectId, flowCode: noticeScope.subFlowCode }).$promise.then(function (response) {
                                        checkIfShowCDOFOMD(response);
                                    });
                                }

                                checkIfShowCDOFOMD(entity);

                                noticeScope.isLoading = true;
                                var populateData = function (response) {
                                    var dicUsers = response.data.dicUsers;
                                    var approvers = response.data.approvers;
                                    if (noticeScope.subFlowCode == "MajorLease_Package") {
                                        noticeScope.isHideMCCLAssetDirector = true;
                                        noticeScope.subFlowName = 'Package';
                                        noticeScope.isShowCC = true;
                                        noticeScope.MarketMgrs = dicUsers.MarketMgrs;
                                        if (noticeScope.MarketMgrs != null && noticeScope.MarketMgrs.length == 1) {
                                            noticeScope.notifyUsersInfo.MarketMgr = noticeScope.MarketMgrs[0];
                                        } else {
                                            angular.forEach(noticeScope.MarketMgrs, function (v, k) {
                                                if (approvers && v.Code == approvers.MarketMgrCode) {
                                                    noticeScope.notifyUsersInfo.MarketMgr = v;
                                                }
                                            });
                                        }
                                        noticeScope.RegionalMgrs = dicUsers.RegionalMgrs;
                                        if (noticeScope.RegionalMgrs != null && noticeScope.RegionalMgrs.length == 1) {
                                            noticeScope.notifyUsersInfo.RegionalMgr = noticeScope.RegionalMgrs[0];
                                        } else {
                                            angular.forEach(noticeScope.RegionalMgrs, function (v, k) {
                                                if (approvers && v.Code == approvers.RegionalMgrCode) {
                                                    noticeScope.notifyUsersInfo.RegionalMgr = v;
                                                }
                                            });
                                        }

                                        noticeScope.DDs = dicUsers.DDs;
                                        if (noticeScope.DDs != null && noticeScope.DDs.length == 1) {
                                            noticeScope.notifyUsersInfo.DD = noticeScope.DDs[0];
                                        } else {
                                            angular.forEach(noticeScope.DDs, function (v, k) {
                                                if (approvers && v.Code == approvers.DDCode) {
                                                    noticeScope.notifyUsersInfo.DD = v;
                                                }
                                            });
                                        }

                                        noticeScope.GMs = dicUsers.GMs;
                                        if (noticeScope.GMs != null && noticeScope.GMs.length == 1) {
                                            noticeScope.notifyUsersInfo.GM = noticeScope.GMs[0];
                                        } else {
                                            angular.forEach(noticeScope.GMs, function (v, k) {
                                                if (approvers && v.Code == approvers.GMCode) {
                                                    noticeScope.notifyUsersInfo.GM = v;
                                                }
                                            });
                                        }

                                        noticeScope.FCs = dicUsers.FCs;
                                        if (noticeScope.FCs != null && noticeScope.FCs.length == 1) {
                                            noticeScope.notifyUsersInfo.FC = noticeScope.FCs[0];
                                        } else {
                                            angular.forEach(noticeScope.FCs, function (v, k) {
                                                if (approvers && v.Code == approvers.FCCode) {
                                                    noticeScope.notifyUsersInfo.FC = v;
                                                }
                                            });
                                        }

                                        noticeScope.RDDs = dicUsers.RDDs;
                                        if (noticeScope.RDDs != null && noticeScope.RDDs.length == 1) {
                                            noticeScope.notifyUsersInfo.RDD = noticeScope.RDDs[0];
                                        } else {
                                            angular.forEach(noticeScope.RDDs, function (v, k) {
                                                if (noticeScope.notifyUsersInfo.RDD != null && v.Code == noticeScope.notifyUsersInfo.RDD.Code) {
                                                    noticeScope.notifyUsersInfo.RDD = v;
                                                }
                                                if (approvers && v.Code == approvers.RDDCode) {
                                                    noticeScope.notifyUsersInfo.RDD = v;
                                                }
                                            });
                                        }

                                        noticeScope.VPGMs = dicUsers.VPGMs;
                                        if (noticeScope.VPGMs != null && noticeScope.VPGMs.length == 1) {
                                            noticeScope.notifyUsersInfo.VPGM = noticeScope.VPGMs[0];
                                        } else {
                                            angular.forEach(noticeScope.VPGMs, function (v, k) {
                                                if (approvers && v.Code == approvers.VPGMCode) {
                                                    noticeScope.notifyUsersInfo.VPGM = v;
                                                }
                                            });
                                        }

                                        noticeScope.CDOs = dicUsers.CDOs;
                                        if (noticeScope.CDOs != null && noticeScope.CDOs.length == 1) {
                                            noticeScope.notifyUsersInfo.CDO = noticeScope.CDOs[0];
                                        } else {
                                            angular.forEach(noticeScope.CDOs, function (v, k) {
                                                if (approvers && v.Code == approvers.CDOCode) {
                                                    noticeScope.notifyUsersInfo.CDO = v;
                                                }
                                            });
                                        }

                                        noticeScope.CFOs = dicUsers.CFOs;
                                        if (noticeScope.CFOs != null && noticeScope.CFOs.length == 1) {
                                            noticeScope.notifyUsersInfo.CFO = noticeScope.CFOs[0];
                                        } else {
                                            angular.forEach(noticeScope.CFOs, function (v, k) {
                                                if (approvers && v.Code == approvers.CFOCode) {
                                                    noticeScope.notifyUsersInfo.CFO = v;
                                                }
                                            });
                                        }

                                        noticeScope.ManagingDirectors = dicUsers.ManagingDirectors;
                                        if (noticeScope.ManagingDirectors != null && noticeScope.ManagingDirectors.length == 1) {
                                            noticeScope.notifyUsersInfo.ManagingDirector = noticeScope.ManagingDirectors[0];
                                        } else {
                                            angular.forEach(noticeScope.ManagingDirectors, function (v, k) {
                                                if (approvers && v.Code == approvers.MngDirectorCode) {
                                                    noticeScope.notifyUsersInfo.ManagingDirector = v;
                                                }
                                            });
                                        }

                                        if (entity && entity.AppUsers.NoticeUsers != null && entity.AppUsers.NoticeUsers.length > 0) {
                                            noticeScope.notifyUsersInfo.NoticeUsers = entity.AppUsers.NoticeUsers;
                                            noticeScope.noticeUsersName = $.map(entity.AppUsers.NoticeUsers, function (u, i) {
                                                return u.NameENUS == "" ? u.NameZHCN : u.NameENUS;
                                            }).join(",");
                                        }

                                        if (entity && entity.AppUsers.NecessaryNoticeUsers != null && entity.AppUsers.NecessaryNoticeUsers.length > 0) {
                                            noticeScope.notifyUsersInfo.NecessaryNoticeUsers = entity.AppUsers.NecessaryNoticeUsers;
                                            noticeScope.necessaryNoticeUsersName = $.map(entity.AppUsers.NecessaryNoticeUsers, function (u, i) {
                                                return u.NameENUS == "" ? u.NameZHCN : u.NameENUS;
                                            }).join(",");
                                        }

                                    } else if (noticeScope.subFlowCode == "MajorLease_FinanceAnalysis") {
                                        noticeScope.subFlowName = 'FinanceAnalysis';
                                        noticeScope.FMs = dicUsers.FMs;
                                        if (noticeScope.FMs != null && noticeScope.FMs.length == 1) {
                                            noticeScope.notifyUsersInfo.FM = noticeScope.FMs[0];
                                        } else {
                                            angular.forEach(noticeScope.FMs, function (v, k) {
                                                if (approvers && v.Code == approvers.FMCode) {
                                                    noticeScope.notifyUsersInfo.FM = v;
                                                }
                                            });
                                        }
                                    } else if (noticeScope.subFlowCode == "MajorLease_ConsInfo") {
                                        noticeScope.subFlowName = 'ConsInfo';
                                        noticeScope.ConstructionManagers = dicUsers.ConstructionManagers;
                                        if (noticeScope.ConstructionManagers != null && noticeScope.ConstructionManagers.length == 1) {
                                            noticeScope.notifyUsersInfo.ConstructionManager = noticeScope.ConstructionManagers[0];
                                        } else {
                                            angular.forEach(noticeScope.ConstructionManagers, function (v, k) {
                                                if (approvers && v.Code == approvers.ConstructionManagerCode) {
                                                    noticeScope.notifyUsersInfo.ConstructionManager = v;
                                                }
                                            });
                                        }

                                        noticeScope.MCCLConsManagers = dicUsers.MCCLConsManagers;
                                        if (noticeScope.MCCLConsManagers != null && noticeScope.MCCLConsManagers.length == 1) {
                                            noticeScope.notifyUsersInfo.MCCLConsManager = noticeScope.MCCLConsManagers[0];
                                        } else {
                                            angular.forEach(noticeScope.MCCLConsManagers, function (v, k) {
                                                if (approvers && v.Code == approvers.MCCLConsManagerCode) {
                                                    noticeScope.notifyUsersInfo.MCCLConsManager = v;
                                                }
                                            });
                                        }
                                    } else if (noticeScope.subFlowCode == "MajorLease_ConsInvtChecking") {
                                        noticeScope.subFlowName = 'ConsInvtChecking';
                                        noticeScope.ConstructionManagers = dicUsers.ConstructionManagers;
                                        if (noticeScope.ConstructionManagers != null && noticeScope.ConstructionManagers.length == 1) {
                                            noticeScope.notifyUsersInfo.ConstructionManager = noticeScope.ConstructionManagers[0];
                                        } else {
                                            angular.forEach(noticeScope.ConstructionManagers, function (v, k) {
                                                if (approvers && v.Code == approvers.ConstructionManagerCode) {
                                                    noticeScope.notifyUsersInfo.ConstructionManager = v;
                                                }
                                            });
                                        }

                                        noticeScope.FMs = dicUsers.FMs;
                                        if (noticeScope.FMs != null && noticeScope.FMs.length == 1) {
                                            noticeScope.notifyUsersInfo.FM = noticeScope.FMs[0];
                                        } else {
                                            angular.forEach(noticeScope.FMs, function (v, k) {
                                                if (approvers && v.Code == approvers.FMCode) {
                                                    noticeScope.notifyUsersInfo.FM = v;
                                                }
                                            });
                                        }

                                        noticeScope.FinanceControllers = dicUsers.FinanceControllers;
                                        if (noticeScope.ConstructionManagers != null && noticeScope.FinanceControllers.length == 1) {
                                            noticeScope.notifyUsersInfo.FinanceController = noticeScope.FinanceControllers[0];
                                        } else {
                                            angular.forEach(noticeScope.FinanceControllers, function (v, k) {
                                                if (approvers && v.Code == approvers.FinanceControllerCode) {
                                                    noticeScope.notifyUsersInfo.FinanceController = v;
                                                }
                                            });
                                        }

                                        noticeScope.VPGMs = dicUsers.VPGMs;
                                        if (noticeScope.VPGMs != null && noticeScope.VPGMs.length == 1) {
                                            noticeScope.notifyUsersInfo.VPGM = noticeScope.VPGMs[0];
                                        } else {
                                            angular.forEach(noticeScope.VPGMs, function (v, k) {
                                                if (approvers && v.Code == approvers.VPGMCode) {
                                                    noticeScope.notifyUsersInfo.VPGM = v;
                                                }
                                            });
                                        }
                                    } else if (noticeScope.subFlowCode == "MajorLease_LegalReview") {
                                        noticeScope.subFlowName = 'LegalReview';
                                        noticeScope.Legals = dicUsers.Legals;
                                        if (noticeScope.Legals != null && noticeScope.Legals.length == 1) {
                                            noticeScope.notifyUsersInfo.Legal = noticeScope.Legals[0];
                                        } else {
                                            angular.forEach(noticeScope.Legals, function (v, k) {
                                                if (approvers && v.Code == approvers.LegalCode) {
                                                    noticeScope.notifyUsersInfo.Legal = v;
                                                }
                                            });
                                        }
                                    }
                                    else if (noticeScope.subFlowCode == "MajorLease_GBMemo") {
                                        noticeScope.subFlowName = 'GBMemo';
                                        noticeScope.ConstructionManagers = dicUsers.ConstructionManagers;
                                        if (noticeScope.ConstructionManagers != null && noticeScope.ConstructionManagers.length == 1) {
                                            noticeScope.notifyUsersInfo.ConstructionManager = noticeScope.ConstructionManagers[0];
                                        } else {
                                            angular.forEach(noticeScope.ConstructionManagers, function (v, k) {
                                                if (approvers && v.Code == approvers.ConstructionManager) {
                                                    noticeScope.notifyUsersInfo.ConstructionManager = v;
                                                }
                                            });
                                        }
                                    }
                                    noticeScope.MCCLAssetMgrs = dicUsers.MCCLAssetMgrs;
                                    if (noticeScope.MCCLAssetMgrs != null && noticeScope.MCCLAssetMgrs.length == 1) {
                                        noticeScope.notifyUsersInfo.MCCLAssetMgr = noticeScope.MCCLAssetMgrs[0];
                                    } else {
                                        angular.forEach(noticeScope.MCCLAssetMgrs, function (v, k) {
                                            if (approvers && v.Code == approvers.MCCLAssetMgrCode) {
                                                noticeScope.notifyUsersInfo.MCCLAssetMgr = v;
                                            }
                                        });
                                    }

                                    noticeScope.MCCLAssetDtrs = dicUsers.MCCLAssetDtrs;
                                    if (noticeScope.MCCLAssetDtrs != null && noticeScope.MCCLAssetDtrs.length == 1) {
                                        noticeScope.notifyUsersInfo.MCCLAssetDtr = noticeScope.MCCLAssetDtrs[0];
                                    } else {
                                        angular.forEach(noticeScope.MCCLAssetDtrs, function (v, k) {
                                            if (approvers && v.Code == approvers.MCCLAssetDtrCode) {
                                                noticeScope.notifyUsersInfo.MCCLAssetDtr = v;
                                            }
                                        });
                                    }
                                };
                                projectUsersService.getMajorLeaseApprovers(noticeScope.subFlowCode, noticeScope.projectId).then(function (response) {
                                    if (response && response.data) {
                                        populateData(response);

                                        if (noticeScope.isShowCC) {
                                            projectUsersService.getNecessaryNotifyUsers(usCode, noticeScope.subFlowCode).then(function (result) {
                                                if (result && result.data) {
                                                    noticeScope.necessaryNoticeUserCode = result.data.UserCodes;
                                                    noticeScope.necessaryNoticeRoleNames = result.data.RoleNames;
                                                    noticeScope.necessaryNoticeRoleUser = result.data.RoleUser;
                                                }
                                                noticeScope.isNecessaryLoading = false;
                                            });
                                        }
                                    }
                                    noticeScope.isLoading = false;
                                });

                                noticeScope.selectEmployee = function () {
                                    var users = noticeScope.notifyUsersInfo.NoticeUsers;

                                    $selectUser.open({
                                        checkUsers: function (selectedUsers) {
                                            return true;
                                        },
                                        selectedUsers: angular.copy(users),
                                        OnUserSelected: function (selectedUsers) {
                                            noticeScope.noticeUsersName = $.map(selectedUsers, function (u, i) {
                                                return u.NameENUS;
                                            }).join(",");
                                            noticeScope.notifyUsersInfo.NoticeUsers = $.map(selectedUsers, function (u, i) {
                                                return {
                                                    Code: u.Code,
                                                    NameZHCN: u.NameZHCN,
                                                    NameENUS: u.NameENUS
                                                };
                                            });
                                        }
                                    });
                                };
                                noticeScope.selectNecessary = function () {
                                    var users = noticeScope.notifyUsersInfo.NecessaryNoticeUsers;

                                    $selectUser.open({
                                        checkUsers: function (selectedUsers) {
                                            return true;
                                        },
                                        selectedUsers: angular.copy(users),
                                        scopeUserCodes: noticeScope.necessaryNoticeUserCode,
                                        OnUserSelected: function (selectedUsers) {
                                            noticeScope.necessaryNoticeUsersName = $.map(selectedUsers, function (u, i) {
                                                return u.NameENUS;
                                            }).join(",");
                                            noticeScope.notifyUsersInfo.NecessaryNoticeUsers = $.map(selectedUsers, function (u, i) {
                                                return {
                                                    Code: u.Code,
                                                    NameZHCN: u.NameZHCN,
                                                    NameENUS: u.NameENUS
                                                };
                                            });
                                        }
                                    });
                                };
                                var validate = function () {
                                    var errors = [];
                                    if (noticeScope.subFlowCode == "MajorLease_Package") {
                                        if (!noticeScope.notifyUsersInfo.MarketMgr) {
                                            errors.push("[[[请选择]]]Market Manager!");
                                        }

                                        //if (!noticeScope.notifyUsersInfo.RegionalMgr) {
                                        //    errors.push("[[[请选择]]]Regional Manager!");
                                        //}
                                        //if (!noticeScope.notifyUsersInfo.DD) {
                                        //    errors.push("[[[请选择]]]DD!");
                                        //}
                                        if (!noticeScope.notifyUsersInfo.GM) {
                                            errors.push("[[[请选择]]]GM!");
                                        }
                                        if (!noticeScope.notifyUsersInfo.FC) {
                                            errors.push("[[[请选择]]]FC!");
                                        }

                                        //if (!noticeScope.notifyUsersInfo.RDD) {
                                        //    errors.push("[[[请选择]]]RDD!");
                                        //}
                                        if (!noticeScope.notifyUsersInfo.VPGM) {
                                            errors.push("[[[请选择]]]VPGM!");
                                        }
                                        if (!noticeScope.notifyUsersInfo.CDO) {
                                            errors.push("[[[请选择]]]CDO!");
                                        }
                                        if (!noticeScope.notifyUsersInfo.CFO) {
                                            errors.push("[[[请选择]]]CFO!");
                                        }
                                        //if (!noticeScope.notifyUsersInfo.MCCLAssetMgr) {
                                        //    errors.push("[[[请选择]]]MCCL Asset Manager!");
                                        //}
                                        if (!noticeScope.notifyUsersInfo.MCCLAssetDtr) {
                                            errors.push("[[[请选择]]]MCCL Asset Director!");
                                        }

                                        if (noticeScope.notifyUsersInfo.NecessaryNoticeUsers == undefined
                                            || noticeScope.notifyUsersInfo.NecessaryNoticeUsers == null
                                            || noticeScope.notifyUsersInfo.NecessaryNoticeUsers.length == 0) {
                                            errors.push("[[[请选择必要抄送人]]]");
                                        }
                                        else {
                                            var selectedRoleCode = [];
                                            angular.forEach(noticeScope.notifyUsersInfo.NecessaryNoticeUsers, function (r, i) {
                                                angular.forEach(noticeScope.necessaryNoticeRoleUser, function (s, j) {
                                                    if (r.Code == s.UserCode) {
                                                        selectedRoleCode.push(s.RoleName);
                                                    }
                                                });
                                            });
                                            angular.forEach(noticeScope.necessaryNoticeRoleNames.split(','), function (r, i) {
                                                var result = false;
                                                angular.forEach(selectedRoleCode, function (s, j) {
                                                    if (s == r)
                                                        result = true;
                                                });
                                                if (!result)
                                                    errors.push("[[[请选择必要抄送人]]]" + r);
                                            });
                                        }
                                    } else if (noticeScope.subFlowCode == "MajorLease_FinanceAnalysis") {
                                        if (!noticeScope.notifyUsersInfo.FM) {
                                            errors.push("[[[请选择]]]FM!");
                                        }
                                    } else if (noticeScope.subFlowCode == "MajorLease_ConsInfo") {
                                        if (!noticeScope.notifyUsersInfo.ConstructionManager) {
                                            errors.push("[[[请选择]]]Construction Manager!");
                                        }
                                        if (!noticeScope.notifyUsersInfo.MCCLConsManager) {
                                            errors.push("[[[请选择]]]MCCL Cons Manager!");
                                        }
                                    } else if (noticeScope.subFlowCode == "MajorLease_ConsInvtChecking") {
                                        if (noticeScope.ApprovalType == "LeqFivePercent") {
                                            if (!noticeScope.notifyUsersInfo.ConstructionManager) {
                                                errors.push("[[[请选择]]]Construction Manager!");
                                            }
                                            if (!noticeScope.notifyUsersInfo.FM) {
                                                errors.push("[[[请选择]]]Finance Manager!");
                                            }
                                        } else if (noticeScope.ApprovalType == "BetweenFiveAndTenPercent") {
                                            if (!noticeScope.notifyUsersInfo.ConstructionManager) {
                                                errors.push("[[[请选择]]]Construction Manager!");
                                            }
                                            if (!noticeScope.notifyUsersInfo.FM) {
                                                errors.push("[[[请选择]]]Finance Manager!");
                                            }
                                            if (!noticeScope.notifyUsersInfo.FinanceController) {
                                                errors.push("[[[请选择]]]Finance Controller!");
                                            }
                                        } else if (noticeScope.ApprovalType == "MoreThanPercent") {
                                            if (!noticeScope.notifyUsersInfo.ConstructionManager) {
                                                errors.push("[[[请选择]]]Construction Manager!");
                                            }
                                            if (!noticeScope.notifyUsersInfo.FM) {
                                                errors.push("[[[请选择]]]Finance Manager!");
                                            }
                                            if (!noticeScope.notifyUsersInfo.FinanceController) {
                                                errors.push("[[[请选择]]]Finance Controller!");
                                            }
                                            if (!noticeScope.notifyUsersInfo.VPGM) {
                                                errors.push("[[[请选择]]]VPGM!");
                                            }
                                        }
                                    } else if (noticeScope.subFlowCode == "MajorLease_LegalReview") {
                                        if (!noticeScope.notifyUsersInfo.Legal) {
                                            errors.push("[[[请选择]]]Legal!");
                                        }
                                    }

                                    if (errors.length > 0) {
                                        messager.showMessage(errors.join("<br />"), "fa-warning c_red");
                                        return false;
                                    } else {
                                        return true;
                                    }
                                };
                                noticeScope.ok = function () {
                                    if (validate()) {
                                        $modalInstance.close(notifyUsersInfo);
                                        return true;
                                    } else {
                                        return false;
                                    }
                                };
                                noticeScope.cancel = function () {
                                    $modalInstance.dismiss("cancel");
                                };
                            }
                        ]
                    }).result;
                }
            }
        }
    ]).factory("tempClosureApprovalDialogService", [
        "$modal",
        "messager",
        "$selectUser",
        "tempClosureService",
        "projectUsersService",
        function ($modal, messager, $selectUser, tempClosureService, projectUsersService) {
            return {
                open: function (projectId, usCode) {
                    return $modal.open({
                        backdrop: "static",
                        size: "lg",
                        templateUrl: Utils.ServiceURI.AppUri + "TempClosureModule/SelectApprovers",
                        controller: [
                            "$scope",
                            "$modalInstance",
                            function ($modalScope, $modalInstance) {
                                $modalScope.isNecessaryLoading = true;
                                tempClosureService.getClosurePackageApprovers({
                                    projectId: projectId
                                }).$promise.then(function (data) {
                                    var checkApprover = function (_approvers, userRole) {
                                        if (_approvers.length == 1 && !data.ProjectDto.ApproveUsers[userRole]) {
                                            data.ProjectDto.ApproveUsers[userRole] = _approvers[0];
                                        } else if (!!data.ProjectDto.ApproveUsers[userRole]) {
                                            angular.forEach(_approvers, function (e, i) {
                                                if (e.Code == data.ProjectDto.ApproveUsers[userRole].Code) {
                                                    data.ProjectDto.ApproveUsers[userRole] = e;
                                                    return false;
                                                }
                                            });
                                        }
                                    };
                                    checkApprover(data.MarketMgrs, "MarketMgr");
                                    checkApprover(data.MDDs, "MDD");
                                    checkApprover(data.GMs, "GM");
                                    checkApprover(data.FCs, "FC");
                                    checkApprover(data.VPGMs, "VPGM");
                                    checkApprover(data.MCCLAssetMgrs, "MCCLAssetMgr");
                                    checkApprover(data.MCCLAssetDirs, "MCCLAssetDtr");
                                    checkApprover(data.RegionalMgrs, "RegionalMgr");
                                    $modalScope.data = data;
                                    $modalScope.approversLoaded = true;
                                });
                                projectUsersService.getNecessaryNotifyUsers(usCode, "TempClosure_ClosurePackage").then(function (response) {
                                    if (response.data && response.data != null) {
                                        $modalScope.necessaryNoticeUserCode = response.data.UserCodes;
                                        $modalScope.necessaryNoticeRoleNames = response.data.RoleNames;
                                        $modalScope.necessaryNoticeRoleUser = response.data.RoleUser;
                                    }
                                    $modalScope.isNecessaryLoading = false;
                                });
                                $modalScope.selectEmployee = function () {
                                    $selectUser.open({
                                        checkUsers: function (selectedUsers) {
                                            return true;
                                        },
                                        selectedUsers: angular.copy($modalScope.data.ProjectDto.ApproveUsers.NoticeUsers || []),
                                        OnUserSelected: function (selectedUsers) {
                                            $modalScope.data.ProjectDto.ApproveUsers.NoticeUsers = angular.copy(selectedUsers);
                                        }
                                    });
                                };
                                $modalScope.selectNecessary = function () {
                                    $selectUser.open({
                                        checkUsers: function (selectedUsers) {
                                            return true;
                                        },
                                        selectedUsers: angular.copy($modalScope.data.ProjectDto.ApproveUsers.NecessaryNoticeUsers || []),
                                        scopeUserCodes: $modalScope.necessaryNoticeUserCode,
                                        OnUserSelected: function (selectedUsers) {
                                            $modalScope.data.ProjectDto.ApproveUsers.NecessaryNoticeUsers = angular.copy(selectedUsers);
                                        }
                                    });
                                };
                                $modalScope.ok = function (frm) {
                                    if (!frm.$valid)
                                        return false;

                                    var errors = [];
                                    var approvers = $modalScope.data.ProjectDto.ApproveUsers;
                                    if (!approvers.MarketMgr) {
                                        errors.push("[[[请选择]]]Market Manager");
                                    };
                                    if (!approvers.MDD) {
                                        errors.push("[[[请选择]]]MDD");
                                    };
                                    if (!approvers.GM) {
                                        errors.push("[[[请选择]]]GM");
                                    };
                                    if (!approvers.FC) {
                                        errors.push("[[[请选择]]]FC");
                                    };
                                    if (!approvers.VPGM) {
                                        errors.push("[[[请选择]]]VPGM");
                                    };
                                    //if (!approvers.MCCLAssetMgr) {
                                    //    errors.push("[[[请选择]]]MCCL Asset Manager");
                                    //};
                                    //if (!approvers.MCCLAssetDtr) {
                                    //    errors.push("[[[请选择]]]MCCL Asset Director");
                                    //};
                                    if (approvers.NecessaryNoticeUsers == undefined || approvers.NecessaryNoticeUsers == null || approvers.NecessaryNoticeUsers.length == 0) {
                                        errors.push("[[[请选择必要抄送人]]]");
                                    }
                                    else {
                                        var selectedRoleCode = [];
                                        angular.forEach(approvers.NecessaryNoticeUsers, function (r, i) {
                                            angular.forEach($modalScope.necessaryNoticeRoleUser, function (s, j) {
                                                if (r.Code == s.UserCode) {
                                                    selectedRoleCode.push(s.RoleName);
                                                }
                                            });
                                        });
                                        angular.forEach($modalScope.necessaryNoticeRoleNames.split(','), function (r, i) {
                                            var result = false;
                                            angular.forEach(selectedRoleCode, function (s, j) {
                                                if (s == r)
                                                    result = true;
                                            });
                                            if (!result)
                                                errors.push("[[[请选择必要抄送人]]]" + r);
                                        });
                                    }
                                    if (errors.length > 0) {
                                        messager.showMessage(errors.join("<br />"), "fa-warning c_red");
                                        return;
                                    };
                                    $modalInstance.close($modalScope.data.ProjectDto);
                                };
                                $modalScope.cancel = function () {
                                    $modalInstance.dismiss('');
                                };
                            }
                        ]
                    }).result;
                }
            };
        }
    ]).factory("closureApprovalDialogService", [
        "$http",
        "$modal",
        "messager",
        "$selectUser",
        function ($http, $modal, messager, $selectUser) {
            return {
                open: function (projectId, flowCode, approvalType, usCode) {
                    return $modal.open({
                        templateUrl: "/Template/ClosurePackageSelApprover",
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            notifyUsersInfo: function () {
                                return {
                                    MCCLAssetMgr: null,
                                    GM: null,
                                    NoticeUsers: []
                                }
                            }
                        },
                        controller: [
                            "$scope", "$modalInstance", "notifyUsersInfo", function ($scope, $modalInstance, notifyUsersInfo) {
                                $scope.notifyUsersInfo = notifyUsersInfo;
                                $scope.projectId = projectId;
                                $scope.subFlowCode = flowCode;
                                $scope.ApprovalType = approvalType;

                                $scope.MarketMgrList = [];
                                $scope.RegionalMgrList = [];
                                $scope.MDDList = [];

                                $scope.VPGMList = [];
                                $scope.DEVVPList = [];
                                $scope.CFOList = [];

                                $scope.DDList = [];
                                $scope.FCList = [];
                                $scope.RDDList = [];
                                $scope.GMList = [];

                                $scope.CDOList = [];
                                $scope.DirectorList = [];
                                $scope.MngDirectorList = [];

                                //$scope.MCCLAssetMgrs = [];
                                $scope.MCCLAssetDtrs = [];
                                $scope.approversLoaded = false;
                                $scope.necessaryLoaded = false;

                                var url = Utils.ServiceURI.Address() + "api/StoreUsers/GetStoreEmployeesByMultiRoles/" +
                                    usCode + "?roleCodes=Market_Asset_Mgr,Regional_Asset_Mgr,Market_DD,GM,Finance_Controller,VPGM,CDO,CFO,MD,Regional_DD";
                                $http.get(url).then(function (response) {
                                    var userPositionData = response.data;
                                    var positionData;
                                    for (var i = 0; i < response.data.length; i++) {
                                        positionData = userPositionData[i];
                                        switch (positionData.PositionENUS) {
                                            case "Market Asset Mgr":
                                                $scope.MarketMgrList.push(positionData);
                                                break;
                                            case "Regional Asset Mgr":
                                                $scope.RegionalMgrList.push(positionData);
                                                break;
                                            case "Market DD":
                                                $scope.DDList.push(positionData);
                                                break;
                                            case "GM":
                                                $scope.GMList.push(positionData);
                                                break;
                                            case "Finance Controller":
                                                $scope.FCList.push(positionData);
                                                break;
                                            case "VPGM":
                                                $scope.VPGMList.push(positionData);
                                                break;
                                            case "CDO":
                                                $scope.CDOList.push(positionData);
                                                break;
                                            case "CFO":
                                                $scope.CFOList.push(positionData);
                                                break;
                                            case "MD":
                                                $scope.MngDirectorList.push(positionData);
                                                break;
                                            case "MD":
                                                $scope.MngDirectorList.push(positionData);
                                                break;
                                            case "Regional DD":
                                                $scope.RDDList.push(positionData);
                                                break;
                                                //case "MCCL_Asset Mgr":
                                                //    $scope.MCCLAssetMgrs.push(positionData);
                                                //    break;
                                        }
                                    }
                                    $scope.entity = {};
                                    $http.get(Utils.ServiceURI.Address() + "api/ClosurePackage/GetApproverUsers?projectId=" + projectId + "&flowCode=" + flowCode).then(function (response) {
                                        var approvers = response.data.ApproveUser;

                                        if ($scope.RegionalMgrList.length == 1) {
                                            $scope.entity.selRegionalMgr = $scope.RegionalMgrList[0];
                                        } else {
                                            angular.forEach($scope.RegionalMgrList, function (e, i) {
                                                if (approvers != null && e.Code == approvers.RegionalMgrCode) {
                                                    $scope.entity.selRegionalMgr = e;
                                                    return false;
                                                }
                                            });
                                        }

                                        if ($scope.FCList.length == 1) {
                                            $scope.entity.selFC = $scope.FCList[0];
                                        } else {
                                            angular.forEach($scope.FCList, function (e, i) {
                                                if (approvers != null && e.Code == approvers.FCCodeFCCode) {
                                                    $scope.entity.selFC = e;
                                                    return false;
                                                }
                                            });
                                        }
                                        if ($scope.DDList.length == 1) {
                                            $scope.entity.selDD = $scope.DDList[0];
                                        } else {
                                            angular.forEach($scope.DDList, function (e, i) {
                                                if (approvers != null && e.Code == approvers.MDDCode) {
                                                    $scope.entity.selDD = e;
                                                    return false;
                                                }
                                            });
                                        }
                                        if ($scope.MarketMgrList.length == 1) {
                                            $scope.entity.selMarketMgr = $scope.MarketMgrList[0];
                                        } else {
                                            angular.forEach($scope.MarketMgrList, function (e, i) {
                                                if (approvers != null && e.Code == approvers.MarketMgrCode) {
                                                    $scope.entity.selMarketMgr = e;
                                                    return false;
                                                }
                                            });
                                        }
                                        //if ($scope.MDDList.length == 1) {
                                        //    $scope.entity.selMDD = $scope.MDDList[0];
                                        //} else {
                                        //    angular.forEach($scope.MDDList, function (e, i) {
                                        //        if (approvers != null && e.Code == approvers.MDDCode) {
                                        //            $scope.entity.selMDD = e;
                                        //            return false;
                                        //        }
                                        //    });
                                        //}

                                        if ($scope.VPGMList.length == 1) {
                                            $scope.entity.selVPGM = $scope.VPGMList[0];
                                        } else {
                                            angular.forEach($scope.VPGMList, function (e, i) {
                                                if (approvers != null && e.Code == approvers.VPGMCode) {
                                                    $scope.entity.selVPGM = e;
                                                    return false;
                                                }
                                            });
                                        }

                                        if ($scope.CDOList.length == 1) {
                                            $scope.entity.selCDO = $scope.CDOList[0];
                                        } else {
                                            angular.forEach($scope.CDOList, function (e, i) {
                                                if (approvers != null && e.Code == approvers.CDOCode) {
                                                    $scope.entity.selCDO = e;
                                                    return false;
                                                }
                                            });
                                        }
                                        if ($scope.CFOList.length == 1) {
                                            $scope.entity.selCFO = $scope.CFOList[0];
                                        } else {
                                            angular.forEach($scope.CFOList, function (e, i) {
                                                if (approvers != null && e.Code == approvers.CFOCode) {
                                                    $scope.entity.selCFO = e;
                                                    return false;
                                                }
                                            });
                                        }

                                        if ($scope.DEVVPList.length == 1) {
                                            $scope.entity.selDEVVP = $scope.DEVVPList[0];
                                        }

                                        if ($scope.GMList.length == 1) {
                                            $scope.entity.selGM = $scope.GMList[0];
                                        } else {
                                            angular.forEach($scope.GMList, function (e, i) {
                                                if (approvers != null && e.Code == approvers.GMCode) {
                                                    $scope.entity.selGM = e;
                                                    return false;
                                                }
                                            });
                                        }

                                        if ($scope.MngDirectorList.length == 1) {
                                            $scope.entity.selMngDirector = $scope.MngDirectorList[0];
                                        } else {
                                            angular.forEach($scope.MngDirectorList, function (e, i) {
                                                if (approvers != null && e.Code == approvers.MngDirectorCode) {
                                                    $scope.entity.selMngDirector = e;
                                                    return false;
                                                }
                                            });
                                        }

                                        if ($scope.RDDList.length == 1) {
                                            $scope.entity.selRDD = $scope.RDDList[0];
                                        } else {
                                            angular.forEach($scope.RDDList, function (e, i) {
                                                if (approvers != null && e.Code == approvers.RDDCode) {
                                                    $scope.entity.selRDD = e;
                                                    return false;
                                                }
                                            });
                                        }

                                        //if ($scope.MCCLAssetMgrs.length == 1) {
                                        //    $scope.entity.MCCLAssetMgr = $scope.MCCLAssetMgrs[0];
                                        //} else {
                                        //    angular.forEach($scope.MCCLAssetMgrs, function (e, i) {
                                        //        if (approvers != null && e.Code == approvers.MCCLAssetMgrCode) {
                                        //            $scope.entity.MCCLAssetMgr = e;
                                        //            return false;
                                        //        }
                                        //    });
                                        //}
                                        if (response.data.NoticeUsers != null && response.data.NoticeUsers.length > 0) {
                                            $scope.entity.NoticeUsers = response.data.NoticeUsers;
                                            $scope.noticeUsersName = $.map(response.data.NoticeUsers, function (u, i) {
                                                return u.NameENUS;
                                            }).join(",");
                                        }

                                        if (response.data.NecessaryNoticeUsers != null && response.data.NecessaryNoticeUsers.length > 0) {
                                            $scope.entity.NecessaryNoticeUsers = response.data.NecessaryNoticeUsers;
                                            $scope.necessaryNoticeUsersName = $.map(response.data.NecessaryNoticeUsers, function (u, i) {
                                                return u.NameENUS;
                                            }).join(",");
                                        }
                                        $scope.approversLoaded = true;
                                    });

                                });

                                url = Utils.ServiceURI.Address() + "api/NecessaryNotice/GetAvailableUserCodes/" + usCode + "/" + flowCode;
                                $http.get(url).success(function (response) {
                                    if (response && response != null) {
                                        $scope.necessaryNoticeUserCode = response.UserCodes;
                                    }
                                    $scope.necessaryLoaded = true;
                                });

                                $scope.selectEmployee = function () {
                                    var users = $scope.notifyUsersInfo.NoticeUsers = $scope.entity.NoticeUsers;

                                    $selectUser.open({
                                        checkUsers: function (selectedUsers) {
                                            return true;
                                        },
                                        selectedUsers: angular.copy($scope.entity.NoticeUsers),
                                        OnUserSelected: function (selectedUsers) {
                                            $scope.noticeUsersName = $.map(selectedUsers, function (u, i) {
                                                return u.NameENUS;
                                            }).join(",");
                                            $scope.entity.NoticeUsers = $scope.notifyUsersInfo.NoticeUsers = $.map(selectedUsers, function (u, i) {
                                                return {
                                                    Code: u.Code,
                                                    NameZHCN: u.NameZHCN,
                                                    NameENUS: u.NameENUS
                                                };
                                            });
                                        }
                                    });
                                };

                                $scope.selectNecessaryEmployee = function () {
                                    var users = $scope.notifyUsersInfo.NecessaryNoticeUsers = $scope.entity.NecessaryNoticeUsers;

                                    $selectUser.open({
                                        checkUsers: function (selectedUsers) {
                                            return true;
                                        },
                                        selectedUsers: angular.copy($scope.entity.NecessaryNoticeUsers),
                                        scopeUserCodes: $scope.necessaryNoticeUserCode,
                                        OnUserSelected: function (selectedUsers) {
                                            $scope.necessaryNoticeUsersName = $.map(selectedUsers, function (u, i) {
                                                return u.NameENUS;
                                            }).join(",");
                                            $scope.entity.NecessaryNoticeUsers = $scope.notifyUsersInfo.NecessaryNoticeUsers = $.map(selectedUsers, function (u, i) {
                                                return {
                                                    Code: u.Code,
                                                    NameZHCN: u.NameZHCN,
                                                    NameENUS: u.NameENUS
                                                };
                                            });
                                        }
                                    });
                                };

                                $scope.ok = function () {
                                    if (!$scope.entity.selMarketMgr) {
                                        messager.showMessage('[[[请选择Market Manager!]]]', 'fa-warning c_red');
                                        return false;
                                    }

                                    if (!$scope.entity.selDD) {
                                        messager.showMessage('[[[请选择MDD!]]]', 'fa-warning c_red');
                                        return false;
                                    }

                                    if (!$scope.entity.selGM) {
                                        messager.showMessage('[[[请选择GM!]]]', 'fa-warning c_red');
                                        return false;
                                    }

                                    if (!$scope.entity.selFC) {
                                        messager.showMessage('[[[请选择FC!]]]', 'fa-warning c_red');
                                        return false;
                                    }

                                    if (!$scope.entity.selVPGM) {
                                        messager.showMessage('[[[请选择VPGM!]]]', 'fa-warning c_red');
                                        return false;
                                    }

                                    if (!$scope.entity.selCDO) {
                                        messager.showMessage('[[[请选择Head of Development]]]', 'fa-warning c_red');
                                        return false;
                                    }

                                    if (!$scope.entity.selCFO) {
                                        messager.showMessage('[[[请选择CFO]]]', 'fa-warning c_red');
                                        return false;
                                    }

                                    if (!$scope.entity.selMngDirector) {
                                        messager.showMessage('[[[请选择Managing Director]]]', 'fa-warning c_red');
                                        return false;
                                    }

                                    //if (!$scope.entity.selRDD) {
                                    //    messager.showMessage('[[[请选择RDD]]]', 'fa-warning c_red');
                                    //    return false;
                                    //}
                                    //if (!$scope.entity.MCCLAssetMgr) {
                                    //    messager.showMessage('请选择MCCL Asset Manager!', 'fa-warning c_red');
                                    //    return false;
                                    //}
                                    if (!$scope.entity.NecessaryNoticeUsers || $scope.entity.NecessaryNoticeUsers.length == 0) {
                                        messager.showMessage('[[[请选择必要抄送人]]]', 'fa-warning c_red');
                                        return false;
                                    }
                                    $modalInstance.close($scope.entity);
                                    return true;
                                };
                                $scope.cancel = function () {
                                    $modalInstance.dismiss("cancel");
                                };
                            }
                        ]
                    }).result;
                }
            }
        }
    ]).factory("rebuildApprovalDialogService", [
        '$modal',
        'projectUsersService',
        "messager",
        function ($modal, projectUsersService, messager) {
            return {
                open: function (projectId, flowCode, approvalType, entity) {

                    return $modal.open({
                        backdrop: 'static',
                        templateUrl: "/Rebuild/ApproveDialog",
                        size: 'lg',
                        resolve: {
                            notifyUsersInfo: function () {
                                return {
                                    MCCLAssetMgr: null,
                                    GM: null,
                                    NoticeUsers: []
                                }
                            }
                        },
                        controller: ["$scope", "$modalInstance", "$selectUser", "notifyUsersInfo", function (noticeScope, $modalInstance, $selectUser, notifyUsersInfo) {
                            noticeScope.projectId = projectId;
                            noticeScope.subFlowName = '';
                            noticeScope.subFlowCode = flowCode;
                            noticeScope.ApprovalType = approvalType;
                            noticeScope.notifyUsersInfo = notifyUsersInfo;
                            noticeScope.isShowCC = false;
                            noticeScope.isLoading = true;
                            noticeScope.isNecessaryLoading = true;
                            noticeScope.isShowCDOFOMD = false;

                            var populateData = function (response) {
                                if (noticeScope.subFlowCode == "Rebuild_Package") {
                                    noticeScope.subFlowName = 'Package';
                                    noticeScope.isShowCC = true;
                                    noticeScope.MarketMgrs = response.data.dicUsers.MarketMgrs;
                                    if (noticeScope.MarketMgrs != null && noticeScope.MarketMgrs.length == 1) {
                                        noticeScope.notifyUsersInfo.MarketMgr = noticeScope.MarketMgrs[0];
                                    } else {
                                        angular.forEach(noticeScope.MarketMgrs, function (v, k) {
                                            if (entity.AppUsers.MarketMgr != null && v.Code == entity.AppUsers.MarketMgr.Code) {
                                                noticeScope.notifyUsersInfo.MarketMgr = v;
                                            }
                                        });
                                    }
                                    noticeScope.RegionalMgrs = response.data.dicUsers.RegionalMgrs;
                                    if (noticeScope.RegionalMgrs != null && noticeScope.RegionalMgrs.length == 1) {
                                        noticeScope.notifyUsersInfo.RegionalMgr = noticeScope.RegionalMgrs[0];
                                    } else {
                                        angular.forEach(noticeScope.RegionalMgrs, function (v, k) {
                                            if (entity.AppUsers.RegionalMgr != null && v.Code == entity.AppUsers.RegionalMgr.Code) {
                                                noticeScope.notifyUsersInfo.RegionalMgr = v;
                                            }
                                        });
                                    }

                                    noticeScope.MDDs = response.data.dicUsers.MDDs;
                                    if (noticeScope.MDDs != null && noticeScope.MDDs.length == 1) {
                                        noticeScope.notifyUsersInfo.MDD = noticeScope.MDDs[0];
                                    } else {
                                        angular.forEach(noticeScope.MDDs, function (v, k) {
                                            if (entity.AppUsers.MDD != null && v.Code == entity.AppUsers.MDD.Code) {
                                                noticeScope.notifyUsersInfo.MDD = v;
                                            }
                                        });
                                    }

                                    noticeScope.GMs = response.data.dicUsers.GMs;
                                    if (noticeScope.GMs != null && noticeScope.GMs.length == 1) {
                                        noticeScope.notifyUsersInfo.GM = noticeScope.GMs[0];
                                    } else {
                                        angular.forEach(noticeScope.GMs, function (v, k) {
                                            if (entity.AppUsers.GM != null && v.Code == entity.AppUsers.GM.Code) {
                                                noticeScope.notifyUsersInfo.GM = v;
                                            }
                                        });
                                    }

                                    noticeScope.FCs = response.data.dicUsers.FCs;
                                    if (noticeScope.FCs != null && noticeScope.FCs.length == 1) {
                                        noticeScope.notifyUsersInfo.FC = noticeScope.FCs[0];
                                    } else {
                                        angular.forEach(noticeScope.FCs, function (v, k) {
                                            if (entity.AppUsers.FC != null && v.Code == entity.AppUsers.FC.Code) {
                                                noticeScope.notifyUsersInfo.FC = v;
                                            }
                                        });
                                    }

                                    noticeScope.RDDs = response.data.dicUsers.RDDs;
                                    if (noticeScope.RDDs != null && noticeScope.RDDs.length == 1) {
                                        noticeScope.notifyUsersInfo.RDD = noticeScope.RDDs[0];
                                    } else {
                                        angular.forEach(noticeScope.RDDs, function (v, k) {
                                            if (entity.AppUsers.RDD != null && v.Code == entity.AppUsers.RDD.Code) {
                                                noticeScope.notifyUsersInfo.RDD = v;
                                            }
                                        });
                                    }

                                    noticeScope.VPGMs = response.data.dicUsers.VPGMs;
                                    if (noticeScope.VPGMs != null && noticeScope.VPGMs.length == 1) {
                                        noticeScope.notifyUsersInfo.VPGM = noticeScope.VPGMs[0];
                                    } else {
                                        angular.forEach(noticeScope.VPGMs, function (v, k) {
                                            if (entity.AppUsers.VPGM != null && v.Code == entity.AppUsers.VPGM.Code) {
                                                noticeScope.notifyUsersInfo.VPGM = v;
                                            }
                                        });
                                    }

                                    noticeScope.CDOs = response.data.dicUsers.CDOs;
                                    if (noticeScope.CDOs != null && noticeScope.CDOs.length == 1) {
                                        noticeScope.notifyUsersInfo.CDO = noticeScope.CDOs[0];
                                    } else {
                                        angular.forEach(noticeScope.CDOs, function (v, k) {
                                            if (entity.AppUsers.CDO != null && v.Code == entity.AppUsers.CDO.Code) {
                                                noticeScope.notifyUsersInfo.CDO = v;
                                            }
                                        });
                                    }

                                    noticeScope.CFOs = response.data.dicUsers.CFOs;
                                    if (noticeScope.CFOs != null && noticeScope.CFOs.length == 1) {
                                        noticeScope.notifyUsersInfo.CFO = noticeScope.CFOs[0];
                                    } else {
                                        angular.forEach(noticeScope.CFOs, function (v, k) {
                                            if (entity.AppUsers.CFO != null && v.Code == entity.AppUsers.CFO.Code) {
                                                noticeScope.notifyUsersInfo.CFO = v;
                                            }
                                        });
                                    }

                                    noticeScope.ManagingDirectors = response.data.dicUsers.MngDirectors;
                                    if (noticeScope.ManagingDirectors != null && noticeScope.ManagingDirectors.length == 1) {
                                        noticeScope.notifyUsersInfo.ManagingDirector = noticeScope.ManagingDirectors[0];
                                    } else {
                                        angular.forEach(noticeScope.ManagingDirectors, function (v, k) {
                                            if (entity.AppUsers.ManagingDirector != null && v.Code == entity.AppUsers.ManagingDirector.Code) {
                                                noticeScope.notifyUsersInfo.ManagingDirector = v;
                                            }
                                        });
                                    }

                                    if (entity.AppUsers.NoticeUsers != null && entity.AppUsers.NoticeUsers.length > 0) {
                                        noticeScope.notifyUsersInfo.NoticeUsers = entity.AppUsers.NoticeUsers;
                                        noticeScope.noticeUsersName = $.map(entity.AppUsers.NoticeUsers, function (u, i) {
                                            return u.NameENUS == "" ? u.NameZHCN : u.NameENUS;
                                        }).join(",");
                                    }

                                    if (entity.AppUsers.NecessaryNoticeUsers != null && entity.AppUsers.NecessaryNoticeUsers.length > 0) {
                                        noticeScope.notifyUsersInfo.NecessaryNoticeUsers = entity.AppUsers.NecessaryNoticeUsers;
                                        noticeScope.necessaryNoticeUsersName = $.map(entity.AppUsers.NecessaryNoticeUsers, function (u, i) {
                                            return u.NameENUS == "" ? u.NameZHCN : u.NameENUS;
                                        }).join(",");
                                    }

                                } else if (noticeScope.subFlowCode == "Rebuild_FinanceAnalysis") {
                                    noticeScope.subFlowName = 'FinanceAnalysis';
                                    noticeScope.FMs = response.data.dicUsers.FMs;
                                    if (noticeScope.FMs != null && noticeScope.FMs.length == 1) {
                                        noticeScope.notifyUsersInfo.FM = noticeScope.FMs[0];
                                    } else {
                                        angular.forEach(noticeScope.FMs, function (v, k) {
                                            if (entity.AppUsers.FM != null && v.Code == entity.AppUsers.FM.Code) {
                                                noticeScope.notifyUsersInfo.FM = v;
                                            }
                                        });
                                    }
                                } else if (noticeScope.subFlowCode == "Rebuild_ConsInfo") {
                                    noticeScope.subFlowName = 'ConsInfo';
                                    noticeScope.ConstructionManagers = response.data.dicUsers.ConstructionManagers;
                                    if (noticeScope.ConstructionManagers != null && noticeScope.ConstructionManagers.length == 1) {
                                        noticeScope.notifyUsersInfo.ConstructionManager = noticeScope.ConstructionManagers[0];
                                    } else {
                                        angular.forEach(noticeScope.ConstructionManagers, function (v, k) {
                                            if (entity.AppUsers.ConstructionManager != null && v.Code == entity.AppUsers.ConstructionManager.Code) {
                                                noticeScope.notifyUsersInfo.ConstructionManager = v;
                                            }
                                        });
                                    }

                                    noticeScope.MCCLConsManagers = response.data.dicUsers.MCCLConsManagers;
                                    if (noticeScope.MCCLConsManagers != null && noticeScope.MCCLConsManagers.length == 1) {
                                        noticeScope.notifyUsersInfo.MCCLConsManager = noticeScope.MCCLConsManagers[0];
                                    } else {
                                        angular.forEach(noticeScope.MCCLConsManagers, function (v, k) {
                                            if (entity.AppUsers.MCCLConsManager != null && v.Code == entity.AppUsers.MCCLConsManager.Code) {
                                                noticeScope.notifyUsersInfo.MCCLConsManager = v;
                                            }
                                        });
                                    }
                                } else if (noticeScope.subFlowCode == "Rebuild_ConsInvtChecking") {
                                    noticeScope.subFlowName = 'ConsInvtChecking';
                                    noticeScope.ConstructionManagers = response.data.dicUsers.ConstructionManagers;
                                    if (noticeScope.ConstructionManagers != null && noticeScope.ConstructionManagers.length == 1) {
                                        noticeScope.notifyUsersInfo.ConstructionManager = noticeScope.ConstructionManagers[0];
                                    } else {
                                        angular.forEach(noticeScope.ConstructionManagers, function (v, k) {
                                            if (entity.AppUsers.ConstructionManager != null && v.Code == entity.AppUsers.ConstructionManager.Code) {
                                                noticeScope.notifyUsersInfo.ConstructionManager = v;
                                            }
                                        });
                                    }

                                    noticeScope.FMs = response.data.dicUsers.FMs;
                                    if (noticeScope.FMs != null && noticeScope.FMs.length == 1) {
                                        noticeScope.notifyUsersInfo.FM = noticeScope.FMs[0];
                                    } else {
                                        angular.forEach(noticeScope.FMs, function (v, k) {
                                            if (entity.AppUsers.FM != null && v.Code == entity.AppUsers.FM.Code) {
                                                noticeScope.notifyUsersInfo.FM = v;
                                            }
                                        });
                                    }

                                    noticeScope.FinanceControllers = response.data.dicUsers.FinanceControllers;
                                    if (noticeScope.ConstructionManagers != null && noticeScope.FinanceControllers.length == 1) {
                                        noticeScope.notifyUsersInfo.FinanceController = noticeScope.FinanceControllers[0];
                                    } else {
                                        angular.forEach(noticeScope.FinanceControllers, function (v, k) {
                                            if (entity.AppUsers.FinanceController != null && v.Code == entity.AppUsers.FinanceController.Code) {
                                                noticeScope.notifyUsersInfo.FinanceController = v;
                                            }
                                        });
                                    }

                                    noticeScope.VPGMs = response.data.dicUsers.VPGMs;
                                    if (noticeScope.VPGMs != null && noticeScope.VPGMs.length == 1) {
                                        noticeScope.notifyUsersInfo.VPGM = noticeScope.VPGMs[0];
                                    } else {
                                        angular.forEach(noticeScope.VPGMs, function (v, k) {
                                            if (entity.AppUsers.VPGM != null && v.Code == entity.AppUsers.VPGM.Code) {
                                                noticeScope.notifyUsersInfo.VPGM = v;
                                            }
                                        });
                                    }
                                } else if (noticeScope.subFlowCode == "Rebuild_LegalReview") {
                                    noticeScope.subFlowName = 'LegalReview';
                                    noticeScope.Legals = response.data.dicUsers.Legals;
                                    if (noticeScope.Legals != null && noticeScope.Legals.length == 1) {
                                        noticeScope.notifyUsersInfo.Legal = noticeScope.Legals[0];
                                    } else {
                                        angular.forEach(noticeScope.Legals, function (v, k) {
                                            if (entity.AppUsers.Legal != null && v.Code == entity.AppUsers.Legal.Code) {
                                                noticeScope.notifyUsersInfo.Legal = v;
                                            }
                                        });
                                    }
                                }
                                else if (noticeScope.subFlowCode == "Rebuild_GBMemo") {
                                    noticeScope.subFlowName = 'GBMemo';
                                    noticeScope.ConstructionManagers = response.data.dicUsers.ConstructionManagers;
                                    if (noticeScope.ConstructionManagers != null && noticeScope.ConstructionManagers.length == 1) {
                                        noticeScope.notifyUsersInfo.ConstructionManager = noticeScope.ConstructionManagers[0];
                                    } else {
                                        angular.forEach(noticeScope.ConstructionManagers, function (v, k) {
                                            if (entity.AppUsers.ConstructionManager != null && v.Code == entity.AppUsers.ConstructionManager.Code) {
                                                noticeScope.notifyUsersInfo.ConstructionManager = v;
                                            }
                                        });
                                    }
                                }
                                noticeScope.MCCLAssetMgrs = response.data.dicUsers.MCCLAssetMgrs;
                                if (noticeScope.MCCLAssetMgrs != null && noticeScope.MCCLAssetMgrs.length == 1) {
                                    noticeScope.notifyUsersInfo.MCCLAssetMgr = noticeScope.MCCLAssetMgrs[0];
                                } else {
                                    angular.forEach(noticeScope.MCCLAssetMgrs, function (v, k) {
                                        if (entity.AppUsers.MCCLAssetMgr != null && v.Code == entity.AppUsers.MCCLAssetMgr.Code) {
                                            noticeScope.notifyUsersInfo.MCCLAssetMgr = v;
                                        }
                                    });
                                }

                                noticeScope.MCCLAssetDtrs = response.data.dicUsers.MCCLAssetDtrs;
                                if (noticeScope.MCCLAssetDtrs != null && noticeScope.MCCLAssetDtrs.length == 1) {
                                    noticeScope.notifyUsersInfo.MCCLAssetDtr = noticeScope.MCCLAssetDtrs[0];
                                } else {
                                    angular.forEach(noticeScope.MCCLAssetDtrs, function (v, k) {
                                        if (entity.AppUsers.MCCLAssetDtr != null && v.Code == entity.AppUsers.MCCLAssetDtr.Code) {
                                            noticeScope.notifyUsersInfo.MCCLAssetDtr = v;
                                        }
                                    });
                                }
                            };
                            var isNeedEntity = false;
                            if (entity == null) {
                                isNeedEntity = true;
                            }
                            projectUsersService.getRebuildApprovers(noticeScope.subFlowCode, noticeScope.projectId, isNeedEntity).then(function (response) {
                                if (response != null && response.data != null) {
                                    if (isNeedEntity) {
                                        entity = response.data.returnEntity;
                                        entity.RbdInfo = response.data.rbdInfo;
                                    }
                                    if (entity != null && entity.NetWriteOff != null) {
                                        try {
                                            if (parseFloat(entity.NetWriteOff) >= (61.1 * 10000)) {
                                                noticeScope.isShowCDOFOMD = true;
                                            }
                                        } catch (e) {
                                        }
                                    }
                                    populateData(response);
                                    if (noticeScope.isShowCC) {
                                        projectUsersService.getNecessaryNotifyUsers(entity.RbdInfo.USCode, noticeScope.subFlowCode).then(function (response) {
                                            if (response.data && response.data != null) {
                                                noticeScope.necessaryNoticeUserCode = response.data.UserCodes;
                                                noticeScope.necessaryNoticeRoleNames = response.data.RoleNames;
                                                noticeScope.necessaryNoticeRoleUser = response.data.RoleUser;
                                            }
                                            noticeScope.isNecessaryLoading = false;
                                        });
                                    }
                                }
                                noticeScope.isLoading = false;
                            });

                            noticeScope.selectEmployee = function () {
                                var users = noticeScope.notifyUsersInfo.NoticeUsers = entity.AppUsers.NoticeUsers;

                                $selectUser.open({
                                    checkUsers: function (selectedUsers) {
                                        return true;
                                    },
                                    selectedUsers: angular.copy(users),
                                    OnUserSelected: function (selectedUsers) {
                                        noticeScope.noticeUsersName = $.map(selectedUsers, function (u, i) {
                                            return u.NameENUS;
                                        }).join(",");
                                        noticeScope.notifyUsersInfo.NoticeUsers = $.map(selectedUsers, function (u, i) {
                                            return {
                                                Code: u.Code,
                                                NameZHCN: u.NameZHCN,
                                                NameENUS: u.NameENUS
                                            };
                                        });
                                    }
                                });
                            };
                            noticeScope.selectNecessary = function () {
                                var users = noticeScope.notifyUsersInfo.NecessaryNoticeUsers = entity.AppUsers.NecessaryNoticeUsers;

                                $selectUser.open({
                                    checkUsers: function (selectedUsers) {
                                        return true;
                                    },
                                    selectedUsers: angular.copy(users),
                                    scopeUserCodes: noticeScope.necessaryNoticeUserCode,
                                    OnUserSelected: function (selectedUsers) {
                                        noticeScope.necessaryNoticeUsersName = $.map(selectedUsers, function (u, i) {
                                            return u.NameENUS;
                                        }).join(",");
                                        noticeScope.notifyUsersInfo.NecessaryNoticeUsers = $.map(selectedUsers, function (u, i) {
                                            return {
                                                Code: u.Code,
                                                NameZHCN: u.NameZHCN,
                                                NameENUS: u.NameENUS
                                            };
                                        });
                                    }
                                });
                            };
                            var validate = function () {
                                var errors = [];
                                if (noticeScope.subFlowCode == "Rebuild_Package") {
                                    if (!noticeScope.notifyUsersInfo.MarketMgr) {
                                        errors.push("[[[请选择]]]Market Manager!");
                                    }
                                    if (!noticeScope.notifyUsersInfo.GM) {
                                        errors.push("[[[请选择]]]GM!");
                                    }
                                    if (!noticeScope.notifyUsersInfo.FC) {
                                        errors.push("[[[请选择]]]FC!");
                                    }
                                    //if (!noticeScope.notifyUsersInfo.RDD) {
                                    //    errors.push("[[[请选择]]]RDD!");
                                    //}
                                    if (!noticeScope.notifyUsersInfo.MDD) {
                                        errors.push("[[[请选择]]]MDD!");
                                    }
                                    if (!noticeScope.notifyUsersInfo.VPGM) {
                                        errors.push("[[[请选择]]]VPGM!");
                                    }
                                    if (!noticeScope.notifyUsersInfo.CDO && noticeScope.isShowCDOFOMD) {
                                        errors.push("[[[请选择]]]CDO!");
                                    }
                                    if (!noticeScope.notifyUsersInfo.CFO && noticeScope.isShowCDOFOMD) {
                                        errors.push("[[[请选择]]]CFO!");
                                    }
                                    if (!noticeScope.notifyUsersInfo.ManagingDirector && noticeScope.isShowCDOFOMD) {
                                        errors.push("[[[请选择]]]Managing Director!");
                                    }
                                    //if (!noticeScope.notifyUsersInfo.MCCLAssetMgr) {
                                    //    errors.push("[[[请选择]]]MCCL Asset Manager!");
                                    //}
                                    if (!noticeScope.notifyUsersInfo.MCCLAssetDtr) {
                                        errors.push("[[[请选择]]]MCCL Asset Director!");
                                    }
                                    //if (noticeScope.notifyUsersInfo.NoticeUsers != null && noticeScope.notifyUsersInfo.NoticeUsers.length==0) {
                                    //    errors.push("[[[请选择]]]抄送人!");
                                    //}
                                    //if (entity.NetWriteOff != null) {
                                    //    try {
                                    //        if (parseFloat(entity.NetWriteOff) > (152.75 * 10000)) {
                                    //            errors.push("Net Write Off 不能大于152.75万!");
                                    //        }
                                    //    } catch (e) {
                                    //    }
                                    //}
                                    if (noticeScope.notifyUsersInfo.NecessaryNoticeUsers == undefined || noticeScope.notifyUsersInfo.NecessaryNoticeUsers == null || noticeScope.notifyUsersInfo.NecessaryNoticeUsers.length == 0) {
                                        errors.push("[[[请选择必要抄送人]]]");
                                    }
                                    else {
                                        var selectedRoleCode = [];
                                        angular.forEach(noticeScope.notifyUsersInfo.NecessaryNoticeUsers, function (r, i) {
                                            angular.forEach(noticeScope.necessaryNoticeRoleUser, function (s, j) {
                                                if (r.Code == s.UserCode) {
                                                    selectedRoleCode.push(s.RoleName);
                                                }
                                            });
                                        });
                                        angular.forEach(noticeScope.necessaryNoticeRoleNames.split(','), function (r, i) {
                                            var result = false;
                                            angular.forEach(selectedRoleCode, function (s, j) {
                                                if (s == r)
                                                    result = true;
                                            });
                                            if (!result)
                                                errors.push("[[[请选择必要抄送人]]]" + r);
                                        });
                                    }
                                } else if (noticeScope.subFlowCode == "Rebuild_FinanceAnalysis") {
                                    if (!noticeScope.notifyUsersInfo.FM) {
                                        errors.push("[[[请选择]]]FM!");
                                    }
                                } else if (noticeScope.subFlowCode == "Rebuild_ConsInfo") {
                                    if (!noticeScope.notifyUsersInfo.ConstructionManager) {
                                        errors.push("[[[请选择]]]Construction Manager!");
                                    }
                                    if (!noticeScope.notifyUsersInfo.MCCLConsManager) {
                                        errors.push("[[[请选择]]]MCCL Cons Manager!");
                                    }
                                } else if (noticeScope.subFlowCode == "Rebuild_ConsInvtChecking") {
                                    if (noticeScope.ApprovalType == "LeqFivePercent") {
                                        if (!noticeScope.notifyUsersInfo.ConstructionManager) {
                                            errors.push("[[[请选择]]]Construction Manager!");
                                        }
                                        if (!noticeScope.notifyUsersInfo.FM) {
                                            errors.push("[[[请选择]]]Finance Manager!");
                                        }
                                    } else if (noticeScope.ApprovalType == "BetweenFiveAndTenPercent") {
                                        if (!noticeScope.notifyUsersInfo.ConstructionManager) {
                                            errors.push("[[[请选择]]]Construction Manager!");
                                        }
                                        if (!noticeScope.notifyUsersInfo.FM) {
                                            errors.push("[[[请选择]]]Finance Manager!");
                                        }
                                        if (!noticeScope.notifyUsersInfo.FinanceController) {
                                            errors.push("[[[请选择]]]Finance Controller!");
                                        }
                                    } else if (noticeScope.ApprovalType == "MoreThanPercent") {
                                        if (!noticeScope.notifyUsersInfo.ConstructionManager) {
                                            errors.push("[[[请选择]]]Construction Manager!");
                                        }
                                        if (!noticeScope.notifyUsersInfo.FM) {
                                            errors.push("[[[请选择]]]Finance Manager!");
                                        }
                                        if (!noticeScope.notifyUsersInfo.FinanceController) {
                                            errors.push("[[[请选择]]]Finance Controller!");
                                        }
                                        if (!noticeScope.notifyUsersInfo.VPGM) {
                                            errors.push("[[[请选择]]]VPGM!");
                                        }
                                    }
                                } else if (noticeScope.subFlowCode == "Rebuild_LegalReview") {
                                    if (!noticeScope.notifyUsersInfo.Legal) {
                                        errors.push("[[[请选择]]]Legal!");
                                    }
                                }
                                else if (noticeScope.subFlowCode == "Rebuild_GBMemo") {
                                    if (!noticeScope.notifyUsersInfo.ConstructionManager) {
                                        errors.push("[[[请选择]]]Construction Manager!");
                                    }
                                }
                                if (errors.length > 0) {
                                    messager.showMessage(errors.join("<br />"), "fa-warning c_red");
                                    return false;
                                } else {
                                    return true;
                                }
                            };
                            noticeScope.ok = function () {
                                if (validate()) {
                                    if (isNeedEntity) {
                                        var ProjectDto = {
                                            ProjectId: noticeScope.projectId,
                                            FlowCode: noticeScope.subFlowCode,
                                            ApproveUsers: notifyUsersInfo
                                        };
                                        $modalInstance.close(ProjectDto);
                                    } else {
                                        $modalInstance.close(notifyUsersInfo);
                                    }
                                    return true;
                                } else {
                                    return false;
                                }
                            };
                            noticeScope.cancel = function () {
                                $modalInstance.dismiss("cancel");
                            };
                        }]
                    }).result;
                }
            }
        }
    ])
    .factory("reimageApprovalDialogService", [
        '$modal',
        'projectUsersService',
        "messager",
        function ($modal, projectUsersService, messager) {
            return {
                open: function (projectId, flowCode, approvalType, entity, usCode) {
                    var appUsers = {};
                    if (entity && entity.AppUsers) {
                        appUsers = entity.AppUsers;
                    }
                    return $modal.open({
                        backdrop: 'static',
                        templateUrl: "/Reimage/ApproveDialog",
                        size: 'lg',
                        resolve: {
                            notifyUsersInfo: function () {
                                return {
                                    MCCLAssetMgr: null,
                                    GM: null,
                                    NoticeUsers: []
                                }
                            }
                        },
                        controller: [
                            "$scope", "$modalInstance", "$selectUser", "notifyUsersInfo", function (noticeScope, $modalInstance, $selectUser, notifyUsersInfo) {
                                noticeScope.projectId = projectId;
                                noticeScope.subFlowName = '';
                                noticeScope.subFlowCode = flowCode;
                                noticeScope.ApprovalType = approvalType;
                                noticeScope.notifyUsersInfo = notifyUsersInfo;
                                noticeScope.isShowCC = false;
                                noticeScope.isLoading = true;
                                noticeScope.USCode = usCode;
                                noticeScope.isNecessaryLoading = true;
                                if (entity) {
                                    noticeScope.USCode = entity.USCode;
                                }
                                var populateData = function (response) {
                                    if (noticeScope.subFlowCode == "Reimage_Package") {
                                        noticeScope.subFlowName = 'Package';
                                        noticeScope.isShowCC = true;
                                        noticeScope.MarketMgrs = response.data.MarketMgrs;
                                        if (noticeScope.MarketMgrs != null && noticeScope.MarketMgrs.length == 1) {
                                            noticeScope.notifyUsersInfo.MarketMgr = noticeScope.MarketMgrs[0];
                                        } else {
                                            angular.forEach(noticeScope.MarketMgrs, function (v, k) {
                                                if (appUsers.MarketMgr != null && v.Code == appUsers.MarketMgr.Code) {
                                                    noticeScope.notifyUsersInfo.MarketMgr = v;
                                                }
                                            });
                                        }
                                        noticeScope.RegionalMgrs = response.data.RegionalMgrs;
                                        if (noticeScope.RegionalMgrs != null && noticeScope.RegionalMgrs.length == 1) {
                                            noticeScope.notifyUsersInfo.RegionalMgr = noticeScope.RegionalMgrs[0];
                                        } else {
                                            angular.forEach(noticeScope.RegionalMgrs, function (v, k) {
                                                if (appUsers.RegionalMgr != null && v.Code == appUsers.RegionalMgr.Code) {
                                                    noticeScope.notifyUsersInfo.RegionalMgr = v;
                                                }
                                            });
                                        }

                                        noticeScope.MDDs = response.data.MDDs;
                                        if (noticeScope.MDDs != null && noticeScope.MDDs.length == 1) {
                                            noticeScope.notifyUsersInfo.MDD = noticeScope.MDDs[0];
                                        } else {
                                            angular.forEach(noticeScope.MDDs, function (v, k) {
                                                if (appUsers.MDD != null && v.Code == appUsers.MDD.Code) {
                                                    noticeScope.notifyUsersInfo.MDD = v;
                                                }
                                            });
                                        }

                                        noticeScope.DOs = response.data.DOs;
                                        if (noticeScope.DOs != null && noticeScope.DOs.length == 1) {
                                            noticeScope.notifyUsersInfo.DO = noticeScope.DOs[0];
                                        } else {
                                            angular.forEach(noticeScope.DOs, function (v, k) {
                                                if (appUsers.DO != null && v.Code == appUsers.DO.Code) {
                                                    noticeScope.notifyUsersInfo.DO = v;
                                                }
                                            });
                                        }


                                        noticeScope.GMs = response.data.GMs;
                                        if (noticeScope.GMs != null && noticeScope.GMs.length == 1) {
                                            noticeScope.notifyUsersInfo.GM = noticeScope.GMs[0];
                                        } else {
                                            angular.forEach(noticeScope.GMs, function (v, k) {
                                                if (appUsers.GM != null && v.Code == appUsers.GM.Code) {
                                                    noticeScope.notifyUsersInfo.GM = v;
                                                }
                                            });
                                        }

                                        noticeScope.FCs = response.data.FCs;
                                        if (noticeScope.FCs != null && noticeScope.FCs.length == 1) {
                                            noticeScope.notifyUsersInfo.FC = noticeScope.FCs[0];
                                        } else {
                                            angular.forEach(noticeScope.FCs, function (v, k) {
                                                if (appUsers.FC != null && v.Code == appUsers.FC.Code) {
                                                    noticeScope.notifyUsersInfo.FC = v;
                                                }
                                            });
                                        }

                                        noticeScope.RDDs = response.data.RDDs;
                                        if (noticeScope.RDDs != null && noticeScope.RDDs.length == 1) {
                                            noticeScope.notifyUsersInfo.RDD = noticeScope.RDDs[0];
                                        } else {
                                            angular.forEach(noticeScope.RDDs, function (v, k) {
                                                if (appUsers.RDD != null && v.Code == appUsers.RDD.Code) {
                                                    noticeScope.notifyUsersInfo.RDD = v;
                                                }
                                            });
                                        }


                                        noticeScope.VPGMs = response.data.VPGMs;
                                        if (noticeScope.VPGMs != null && noticeScope.VPGMs.length == 1) {
                                            noticeScope.notifyUsersInfo.VPGM = noticeScope.VPGMs[0];
                                        } else {
                                            angular.forEach(noticeScope.VPGMs, function (v, k) {
                                                if (appUsers.VPGM != null && v.Code == appUsers.VPGM.Code) {
                                                    noticeScope.notifyUsersInfo.VPGM = v;
                                                }
                                            });
                                        }

                                        noticeScope.CDOs = response.data.CDOs;
                                        if (noticeScope.CDOs != null && noticeScope.CDOs.length == 1) {
                                            noticeScope.notifyUsersInfo.CDO = noticeScope.CDOs[0];
                                        } else {
                                            angular.forEach(noticeScope.CDOs, function (v, k) {
                                                if (appUsers.CDO != null && v.Code == appUsers.CDO.Code) {
                                                    noticeScope.notifyUsersInfo.CDO = v;
                                                }
                                            });
                                        }

                                        noticeScope.CFOs = response.data.CFOs;
                                        if (noticeScope.CFOs != null && noticeScope.CFOs.length == 1) {
                                            noticeScope.notifyUsersInfo.CFO = noticeScope.CFOs[0];
                                        } else {
                                            angular.forEach(noticeScope.CFOs, function (v, k) {
                                                if (appUsers.CFO != null && v.Code == appUsers.CFO.Code) {
                                                    noticeScope.notifyUsersInfo.CFO = v;
                                                }
                                            });
                                        }

                                        noticeScope.ManagingDirectors = response.data.MngDirectors;
                                        if (noticeScope.ManagingDirectors != null && noticeScope.ManagingDirectors.length == 1) {
                                            noticeScope.notifyUsersInfo.ManagingDirector = noticeScope.ManagingDirectors[0];
                                        } else {
                                            angular.forEach(noticeScope.ManagingDirectors, function (v, k) {
                                                if (appUsers.ManagingDirector != null && v.Code == appUsers.ManagingDirector.Code) {
                                                    noticeScope.notifyUsersInfo.ManagingDirector = v;
                                                }
                                            });
                                        }

                                        if (entity && entity.AppUsers.NoticeUsers != null && entity.AppUsers.NoticeUsers.length > 0) {
                                            noticeScope.notifyUsersInfo.NoticeUsers = entity.AppUsers.NoticeUsers;
                                            noticeScope.noticeUsersName = $.map(entity.AppUsers.NoticeUsers, function (u, i) {
                                                return u.NameENUS == "" ? u.NameZHCN : u.NameENUS;
                                            }).join(",");
                                        }

                                        if (entity && entity.AppUsers.NecessaryNoticeUsers != null && entity.AppUsers.NecessaryNoticeUsers.length > 0) {
                                            noticeScope.notifyUsersInfo.NecessaryNoticeUsers = entity.AppUsers.NecessaryNoticeUsers;
                                            noticeScope.necessaryNoticeUsersName = $.map(entity.AppUsers.NecessaryNoticeUsers, function (u, i) {
                                                return u.NameENUS == "" ? u.NameZHCN : u.NameENUS;
                                            }).join(",");
                                        }
                                    } else if (noticeScope.subFlowCode == "Reimage_FinanceAnalysis") {
                                        noticeScope.subFlowName = 'FinanceAnalysis';
                                        noticeScope.FMs = response.data.FMs;
                                        if (noticeScope.FMs != null && noticeScope.FMs.length == 1) {
                                            noticeScope.notifyUsersInfo.FM = noticeScope.FMs[0];
                                        } else {
                                            angular.forEach(noticeScope.FMs, function (v, k) {
                                                if (noticeScope.AppUsers.FM != null && v.Code == noticeScope.AppUsers.FM.Code) {
                                                    noticeScope.notifyUsersInfo.FM = v;
                                                }
                                            });
                                        }
                                    } else if (noticeScope.subFlowCode == "Reimage_ConsInfo") {
                                        noticeScope.subFlowName = 'ConsInfo';
                                        noticeScope.ConstructionManagers = response.data.ConstructionManagers;
                                        if (noticeScope.ConstructionManagers != null && noticeScope.ConstructionManagers.length == 1) {
                                            noticeScope.notifyUsersInfo.ConstructionManager = noticeScope.ConstructionManagers[0];
                                        } else {
                                            angular.forEach(noticeScope.ConstructionManagers, function (v, k) {
                                                if (appUsers.ConstructionManager != null && v.Code == appUsers.ConstructionManager.Code) {
                                                    noticeScope.notifyUsersInfo.ConstructionManager = v;
                                                }
                                            });
                                        }

                                        noticeScope.MCCLConsManagers = response.data.MCCLConsManagers;
                                        if (noticeScope.MCCLConsManagers != null && noticeScope.MCCLConsManagers.length == 1) {
                                            noticeScope.notifyUsersInfo.MCCLConsManager = noticeScope.MCCLConsManagers[0];
                                        } else {
                                            angular.forEach(noticeScope.MCCLConsManagers, function (v, k) {
                                                if (appUsers.MCCLConsManager != null && v.Code == appUsers.MCCLConsManager.Code) {
                                                    noticeScope.notifyUsersInfo.MCCLConsManager = v;
                                                }
                                            });
                                        }
                                    } else if (noticeScope.subFlowCode == "Reimage_ConsInvtChecking") {
                                        noticeScope.subFlowName = 'ConsInvtChecking';
                                        noticeScope.ConstructionManagers = response.data.ConstructionManagers;
                                        if (noticeScope.ConstructionManagers != null && noticeScope.ConstructionManagers.length == 1) {
                                            noticeScope.notifyUsersInfo.ConstructionManager = noticeScope.ConstructionManagers[0];
                                        } else {
                                            angular.forEach(noticeScope.ConstructionManagers, function (v, k) {
                                                if (appUsers.ConstructionManager != null && v.Code == appUsers.ConstructionManager.Code) {
                                                    noticeScope.notifyUsersInfo.ConstructionManager = v;
                                                }
                                            });
                                        }

                                        noticeScope.FMs = response.data.FMs;
                                        if (noticeScope.FMs != null && noticeScope.FMs.length == 1) {
                                            noticeScope.notifyUsersInfo.FM = noticeScope.FMs[0];
                                        } else {
                                            angular.forEach(noticeScope.FMs, function (v, k) {
                                                if (appUsers.FM != null && v.Code == appUsers.FM.Code) {
                                                    noticeScope.notifyUsersInfo.FM = v;
                                                }
                                            });
                                        }

                                        noticeScope.FinanceControllers = response.data.FinanceControllers;
                                        if (noticeScope.ConstructionManagers != null && noticeScope.FinanceControllers.length == 1) {
                                            noticeScope.notifyUsersInfo.FinanceController = noticeScope.FinanceControllers[0];
                                        } else {
                                            angular.forEach(noticeScope.FinanceControllers, function (v, k) {
                                                if (appUsers.FinanceController != null && v.Code == appUsers.FinanceController.Code) {
                                                    noticeScope.notifyUsersInfo.FinanceController = v;
                                                }
                                            });
                                        }

                                        noticeScope.VPGMs = response.data.VPGMs;
                                        if (noticeScope.VPGMs != null && noticeScope.VPGMs.length == 1) {
                                            noticeScope.notifyUsersInfo.VPGM = noticeScope.VPGMs[0];
                                        } else {
                                            angular.forEach(noticeScope.VPGMs, function (v, k) {
                                                if (appUsers.VPGM != null && v.Code == appUsers.VPGM.Code) {
                                                    noticeScope.notifyUsersInfo.VPGM = v;
                                                }
                                            });
                                        }
                                    } else if (noticeScope.subFlowCode == "Reimage_LegalReview") {
                                        noticeScope.subFlowName = 'ConsInvtChecking';
                                        noticeScope.Legals = response.data.Legals;
                                        if (noticeScope.Legals != null && noticeScope.Legals.length == 1) {
                                            noticeScope.notifyUsersInfo.Legal = noticeScope.Legals[0];
                                        } else {
                                            angular.forEach(noticeScope.Legals, function (v, k) {
                                                if (appUsers.Legal != null && v.Code == appUsers.Legal.Code) {
                                                    noticeScope.notifyUsersInfo.Legal = v;
                                                }
                                            });
                                        }
                                    }
                                    else if (noticeScope.subFlowCode == "Reimage_GBMemo") {
                                        noticeScope.subFlowName = 'ReimageGBMemo';
                                        noticeScope.ConstructionManagers = response.data.ConstructionManagers;
                                        if (noticeScope.ConstructionManagers != null && noticeScope.ConstructionManagers.length == 1) {
                                            noticeScope.notifyUsersInfo.ConstructionManager = noticeScope.ConstructionManagers[0];
                                        } else {
                                            angular.forEach(noticeScope.ConstructionManagers, function (v, k) {
                                                if (appUsers.ConstructionManager != null && v.Code == appUsers.ConstructionManager.Code) {
                                                    noticeScope.notifyUsersInfo.ConstructionManager = v;
                                                }
                                            });
                                        }
                                    }
                                    noticeScope.MCCLAssetMgrs = response.data.MCCLAssetMgrs;
                                    if (noticeScope.MCCLAssetMgrs != null && noticeScope.MCCLAssetMgrs.length == 1) {
                                        noticeScope.notifyUsersInfo.MCCLAssetMgr = noticeScope.MCCLAssetMgrs[0];
                                    } else {
                                        angular.forEach(noticeScope.MCCLAssetMgrs, function (v, k) {
                                            if (appUsers.MCCLAssetMgr != null && v.Code == appUsers.MCCLAssetMgr.Code) {
                                                noticeScope.notifyUsersInfo.MCCLAssetMgr = v;
                                            }
                                        });
                                    }

                                    noticeScope.MCCLAssetDtrs = response.data.MCCLAssetDtrs;
                                    if (noticeScope.MCCLAssetDtrs != null && noticeScope.MCCLAssetDtrs.length == 1) {
                                        noticeScope.notifyUsersInfo.MCCLAssetDtr = noticeScope.MCCLAssetDtrs[0];
                                    } else {
                                        angular.forEach(noticeScope.MCCLAssetDtrs, function (v, k) {
                                            if (appUsers.MCCLAssetDtr != null && v.Code == appUsers.MCCLAssetDtr.Code) {
                                                noticeScope.notifyUsersInfo.MCCLAssetDtr = v;
                                            }
                                        });
                                    }
                                };
                                projectUsersService.getReimageApprovers(noticeScope.subFlowCode, noticeScope.projectId).then(function (response) {
                                    if (response != null && response.data != null) {
                                        populateData(response);

                                        if (noticeScope.isShowCC) {
                                            projectUsersService.getNecessaryNotifyUsers(noticeScope.USCode, noticeScope.subFlowCode).then(function (response) {
                                                if (response.data && response.data != null) {
                                                    noticeScope.necessaryNoticeUserCode = response.data.UserCodes;
                                                    noticeScope.necessaryNoticeRoleNames = response.data.RoleNames;
                                                    noticeScope.necessaryNoticeRoleUser = response.data.RoleUser;
                                                }
                                                noticeScope.isNecessaryLoading = false;
                                            });
                                        }
                                    }
                                    noticeScope.isLoading = false;
                                });

                                noticeScope.selectEmployee = function () {
                                    var users = noticeScope.notifyUsersInfo.NoticeUsers;

                                    $selectUser.open({
                                        checkUsers: function (selectedUsers) {
                                            return true;
                                        },
                                        selectedUsers: angular.copy(users),
                                        OnUserSelected: function (selectedUsers) {
                                            noticeScope.noticeUsersName = $.map(selectedUsers, function (u, i) {
                                                return u.NameENUS;
                                            }).join(",");
                                            noticeScope.notifyUsersInfo.NoticeUsers = $.map(selectedUsers, function (u, i) {
                                                return {
                                                    Code: u.Code,
                                                    NameZHCN: u.NameZHCN,
                                                    NameENUS: u.NameENUS
                                                };
                                            });
                                        }
                                    });
                                };
                                noticeScope.selectNecessary = function () {
                                    var users = noticeScope.notifyUsersInfo.NecessaryNoticeUsers;

                                    $selectUser.open({
                                        checkUsers: function (selectedUsers) {
                                            return true;
                                        },
                                        selectedUsers: angular.copy(users),
                                        scopeUserCodes: noticeScope.necessaryNoticeUserCode,
                                        OnUserSelected: function (selectedUsers) {
                                            noticeScope.necessaryNoticeUsersName = $.map(selectedUsers, function (u, i) {
                                                return u.NameENUS;
                                            }).join(",");
                                            noticeScope.notifyUsersInfo.NecessaryNoticeUsers = $.map(selectedUsers, function (u, i) {
                                                return {
                                                    Code: u.Code,
                                                    NameZHCN: u.NameZHCN,
                                                    NameENUS: u.NameENUS
                                                };
                                            });
                                        }
                                    });
                                };
                                var validate = function () {
                                    var errors = [];
                                    if (noticeScope.subFlowCode == "Reimage_Package") {
                                        if (!noticeScope.notifyUsersInfo.MarketMgr) {
                                            errors.push("[[[请选择]]]Market Manager!");
                                        }

                                        //if (!noticeScope.notifyUsersInfo.RegionalMgr) {
                                        //    errors.push("[[[请选择]]]Regional Manager!");
                                        //}
                                        if (!noticeScope.notifyUsersInfo.DO) {
                                            errors.push("[[[请选择]]]DO!");
                                        }
                                        if (!noticeScope.notifyUsersInfo.GM) {
                                            errors.push("[[[请选择]]]GM!");
                                        }
                                        if (!noticeScope.notifyUsersInfo.FC) {
                                            errors.push("[[[请选择]]]FC!");
                                        }

                                        //if (!noticeScope.notifyUsersInfo.RDD) {
                                        //    errors.push("[[[请选择]]]RDD!");
                                        //}
                                        if (!noticeScope.notifyUsersInfo.VPGM) {
                                            errors.push("[[[请选择]]]VPGM!");
                                        }
                                        if (!noticeScope.notifyUsersInfo.CDO) {
                                            errors.push("[[[请选择]]]CDO!");
                                        }
                                        if (!noticeScope.notifyUsersInfo.CFO) {
                                            errors.push("[[[请选择]]]CFO!");
                                        }
                                        if (!noticeScope.notifyUsersInfo.ManagingDirector) {
                                            errors.push("[[[请选择]]]ManagingDirector!");
                                        }
                                        //if (!noticeScope.notifyUsersInfo.MCCLAssetMgr) {
                                        //    errors.push("[[[请选择]]]MCCL Asset Manager!");
                                        //}
                                        //if (!noticeScope.notifyUsersInfo.MCCLAssetDtr) {
                                        //    errors.push("[[[请选择]]]MCCL Asset Director!");
                                        //}
                                        if (noticeScope.notifyUsersInfo.NecessaryNoticeUsers == undefined || noticeScope.notifyUsersInfo.NecessaryNoticeUsers == null || noticeScope.notifyUsersInfo.NecessaryNoticeUsers.length == 0) {
                                            errors.push("[[[请选择必要抄送人]]]");
                                        }
                                        else {
                                            var selectedRoleCode = [];
                                            angular.forEach(noticeScope.notifyUsersInfo.NecessaryNoticeUsers, function (r, i) {
                                                angular.forEach(noticeScope.necessaryNoticeRoleUser, function (s, j) {
                                                    if (r.Code == s.UserCode) {
                                                        selectedRoleCode.push(s.RoleName);
                                                    }
                                                });
                                            });
                                            angular.forEach(noticeScope.necessaryNoticeRoleNames.split(','), function (r, i) {
                                                var result = false;
                                                angular.forEach(selectedRoleCode, function (s, j) {
                                                    if (s == r)
                                                        result = true;
                                                });
                                                if (!result)
                                                    errors.push("[[[请选择必要抄送人]]]" + r);
                                            });
                                        }
                                    } else if (noticeScope.subFlowCode == "Reimage_FinanceAnalysis") {
                                        if (!noticeScope.notifyUsersInfo.FM) {
                                            errors.push("[[[请选择]]]FM!");
                                        }
                                    } else if (noticeScope.subFlowCode == "Reimage_ConsInfo") {
                                        if (!noticeScope.notifyUsersInfo.ConstructionManager) {
                                            errors.push("[[[请选择]]]Construction Manager!");
                                        }
                                        if (!noticeScope.notifyUsersInfo.MCCLConsManager) {
                                            errors.push("[[[请选择]]]MCCL Cons Manager!");
                                        }
                                    } else if (noticeScope.subFlowCode == "Reimage_ConsInvtChecking") {
                                        if (noticeScope.ApprovalType == "LeqFivePercent") {
                                            if (!noticeScope.notifyUsersInfo.ConstructionManager) {
                                                errors.push("[[[请选择]]]Construction Manager!");
                                            }
                                            if (!noticeScope.notifyUsersInfo.FM) {
                                                errors.push("[[[请选择]]]Finance Manager!");
                                            }
                                        } else if (noticeScope.ApprovalType == "BetweenFiveAndTenPercent") {
                                            if (!noticeScope.notifyUsersInfo.ConstructionManager) {
                                                errors.push("[[[请选择]]]Construction Manager!");
                                            }
                                            if (!noticeScope.notifyUsersInfo.FM) {
                                                errors.push("[[[请选择]]]Finance Manager!");
                                            }
                                            if (!noticeScope.notifyUsersInfo.FinanceController) {
                                                errors.push("[[[请选择]]]Finance Controller!");
                                            }
                                        } else if (noticeScope.ApprovalType == "MoreThanPercent") {
                                            if (!noticeScope.notifyUsersInfo.ConstructionManager) {
                                                errors.push("[[[请选择]]]Construction Manager!");
                                            }
                                            if (!noticeScope.notifyUsersInfo.FM) {
                                                errors.push("[[[请选择]]]Finance Manager!");
                                            }
                                            if (!noticeScope.notifyUsersInfo.FinanceController) {
                                                errors.push("[[[请选择]]]Finance Controller!");
                                            }
                                            if (!noticeScope.notifyUsersInfo.VPGM) {
                                                errors.push("[[[请选择]]]VPGM!");
                                            }
                                        }
                                    } else if (noticeScope.subFlowCode == "Reimage_LegalReview") {
                                        if (!noticeScope.notifyUsersInfo.Legal) {
                                            errors.push("[[[请选择]]]Legal!");
                                        }
                                    }
                                    if (errors.length > 0) {
                                        messager.showMessage(errors.join("<br />"), "fa-warning c_red");
                                        return false;
                                    } else {
                                        return true;
                                    }
                                };
                                noticeScope.ok = function () {
                                    if (validate()) {
                                        $modalInstance.close(notifyUsersInfo);

                                    }
                                };
                                noticeScope.cancel = function () {
                                    $modalInstance.dismiss("cancel");
                                };
                            }
                        ]
                    }).result;
                }
            }
        }
    ]).factory("renewalApprovalDialogService", [
        '$modal',
        'projectUsersService',
        "messager",
        "renewalService",
        function ($modal, projectUsersService, messager, renewalService) {
            return {
                open: function (projectId, flowCode, approvalType, usCode) {
                    var modalResult;
                    var initApprovers = function ($modalScope, response, key) {
                        $.each(response.data[key + "s"] || [], function (i, e) {
                            if (e.Code == response.data[key + 'Code']) {
                                $modalScope.Approvers[key] = e;
                            }
                        });
                        if (response.data[key + "s"] instanceof Array && response.data[key + "s"].length == 1) {
                            $modalScope.Approvers[key] = response.data[key + "s"][0];
                        }
                        $modalScope[key + "s"] = response.data[key + "s"];
                    };
                    switch (flowCode) {
                        case "Renewal_Letter":
                            modalResult = $modal.open({
                                backdrop: "static",
                                size: "lg",
                                resolve: {
                                    Approvers: function () {
                                        return {
                                            AssetManager: null
                                        };
                                    }
                                },
                                templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/LetterApprovalDialog",
                                controller: [
                                    "$scope",
                                    "$modalInstance",
                                    "Approvers", function ($modalScope, $modalInstance, Approvers) {
                                        $modalScope.Approvers = Approvers;
                                        projectUsersService.getRenewalApprovers(flowCode, projectId).then(function (response) {
                                            initApprovers($modalScope, response, "AssetManager");
                                            $modalScope.dataLoaded = true;
                                        }, function (response) {
                                            $modalInstance.dismiss("cancel");
                                            messager.showMessage(response.data.Message, "fa-warning c_orange");
                                        });
                                        $modalScope.ok = function () {
                                            var errors = [];
                                            if (!$modalScope.Approvers.AssetManager) {
                                                errors.push("[[[请选择]]]Asset Manager");
                                            }
                                            if (errors.length > 0) {
                                                messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
                                                return;
                                            }
                                            $modalInstance.close($modalScope.Approvers);
                                        };
                                        $modalScope.cancel = function () {
                                            $modalInstance.dismiss("cancel");
                                        };
                                    }
                                ]
                            }).result;
                            break;
                        case "Renewal_ConsInfo":
                            modalResult = $modal.open({
                                backdrop: "static",
                                size: "lg",
                                resolve: {
                                    Approvers: function () {
                                        return {
                                            ConstructionManager: null,
                                            MCCLConsManager: null
                                        };
                                    }
                                },
                                templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/ConsInfoApprovalDialog",
                                controller: [
                                    "$scope",
                                    "$modalInstance",
                                    "Approvers", function ($modalScope, $modalInstance, Approvers) {
                                        $modalScope.Approvers = Approvers;
                                        projectUsersService.getRenewalApprovers(flowCode, projectId).then(function (response) {
                                            initApprovers($modalScope, response, "ConstructionManager");
                                            initApprovers($modalScope, response, "MCCLConsManager");
                                            $modalScope.dataLoaded = true;
                                        }, function (response) {
                                            $modalInstance.dismiss("cancel");
                                            messager.showMessage(response.data.Message, "fa-warning c_orange");
                                        });
                                        $modalScope.ok = function () {
                                            var errors = [];
                                            if (!$modalScope.Approvers.ConstructionManager) {
                                                errors.push("[[[请选择]]]Construction Manager");
                                            }
                                            if (!$modalScope.Approvers.MCCLConsManager) {
                                                errors.push("[[[请选择]]]MCCL Construction Manager");
                                            }
                                            if (errors.length > 0) {
                                                messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
                                                return;
                                            }
                                            $modalInstance.close($modalScope.Approvers);
                                        };
                                        $modalScope.cancel = function () {
                                            $modalInstance.dismiss("cancel");
                                        };
                                    }
                                ]
                            }).result;
                            break;
                        case "Renewal_Tool":
                            {
                                modalResult = $modal.open({
                                    backdrop: "static",
                                    size: "lg",
                                    resolve: {
                                        Approvers: function () {
                                            return {
                                                FM: null
                                            };
                                        }
                                    },
                                    templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/ToolApprovalDialog",
                                    controller: [
                                        "$scope",
                                        "$modalInstance",
                                        "Approvers", function ($modalScope, $modalInstance, Approvers) {
                                            $modalScope.Approvers = Approvers;
                                            projectUsersService.getRenewalApprovers(flowCode, projectId).then(function (response) {
                                                initApprovers($modalScope, response, "FM");
                                                $modalScope.dataLoaded = true;
                                            }, function (response) {
                                                $modalInstance.dismiss("cancel");
                                                messager.showMessage(response.data.Message, "fa-warning c_orange");
                                            });
                                            $modalScope.ok = function (frm) {
                                                if (frm.$valid) {
                                                    $modalInstance.close($modalScope.Approvers);
                                                }
                                            };
                                            $modalScope.cancel = function () {
                                                $modalInstance.dismiss("cancel");
                                            };
                                        }
                                    ]
                                }).result;
                            }
                            break;
                        case "Renewal_LegalApproval":
                            {
                                modalResult = $modal.open({
                                    backdrop: "static",
                                    size: "lg",
                                    resolve: {
                                        Approvers: function () {
                                            return {
                                                Legal: null,
                                                GeneralCounsel: null
                                            };
                                        }
                                    },
                                    templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/LegalApprovalDialog",
                                    controller: [
                                        "$scope",
                                        "$modalInstance",
                                        "Approvers", function ($modalScope, $modalInstance, Approvers) {
                                            $modalScope.Approvers = Approvers;
                                            $modalScope.ApprovalType = approvalType;
                                            projectUsersService.getRenewalApprovers(flowCode, projectId).then(function (response) {
                                                initApprovers($modalScope, response, "Legal");
                                                initApprovers($modalScope, response, "GeneralCounsel");
                                                $modalScope.dataLoaded = true;
                                            }, function (response) {
                                                $modalInstance.dismiss("cancel");
                                                messager.showMessage(response.data.Message, "fa-warning c_orange");
                                            });
                                            $modalScope.ok = function (frm) {
                                                if (frm.$valid) {
                                                    $modalInstance.close($modalScope.Approvers);
                                                }
                                            };
                                            $modalScope.cancel = function () {
                                                $modalInstance.dismiss("cancel");
                                            };
                                        }
                                    ]
                                }).result;
                            }
                            break;
                        case "Renewal_GBMemo":
                            {
                                modalResult = $modal.open({
                                    backdrop: "static",
                                    size: "lg",
                                    resolve: {
                                        Approvers: function () {
                                            return {
                                                ConstructionManager: null
                                            };
                                        }
                                    },
                                    templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/GBMemoApprovalDialog",
                                    controller: [
                                        "$scope",
                                        "$modalInstance",
                                        "Approvers", function ($modalScope, $modalInstance, Approvers) {
                                            $modalScope.Approvers = Approvers;
                                            projectUsersService.getRenewalApprovers(flowCode, projectId).then(function (response) {
                                                initApprovers($modalScope, response, "ConstructionManager");
                                                $modalScope.dataLoaded = true;
                                            }, function (response) {
                                                $modalInstance.dismiss("cancel");
                                                messager.showMessage(response.data.Message, "fa-warning c_orange");
                                            });
                                            $modalScope.ok = function (frm) {
                                                if (frm.$valid) {
                                                    $modalInstance.close($modalScope.Approvers);
                                                }
                                            };
                                            $modalScope.cancel = function () {
                                                $modalInstance.dismiss("cancel");
                                            };
                                        }
                                    ]
                                }).result;
                            }
                            break;
                        case "Renewal_Package":
                            {
                                modalResult = $modal.open({
                                    backdrop: "static",
                                    size: "lg",
                                    resolve: {
                                        Approvers: function () {
                                            return angular.copy({
                                                MarketMgr: null,
                                                RegionalMgr: null,
                                                MDD: null,
                                                GM: null,
                                                FC: null,
                                                RDD:null,
                                                VPGM: null,
                                                MCCLAssetDtr: null,
                                                CDO: null,
                                                ManagingDirector: null,
                                                MCCLAssetMgr: null,
                                                NoticeUsers: [],
                                                NecessaryNoticeUsers: []
                                            });
                                        }
                                    },
                                    templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/PackageApprovalDialog",
                                    controller: [
                                        "$scope",
                                        "$modalInstance",
                                        "$selectUser",
                                        "Approvers", function ($modalScope, $modalInstance, $selectUser, Approvers) {
                                            $modalScope.isNecessaryLoading = true;
                                            $modalScope.Approvers = Approvers;
                                            renewalService.needCDOApprovePackage({ projectId: projectId }).$promise.then(function (data) {
                                                $modalScope.needCDOApproval = data.NeedCDOApproval;
                                            });
                                            projectUsersService.getRenewalApprovers(flowCode, projectId).then(function (response) {
                                                initApprovers($modalScope, response, "MarketMgr");
                                                initApprovers($modalScope, response, "RegionalMgr");
                                                initApprovers($modalScope, response, "MDD");
                                                initApprovers($modalScope, response, "GM");
                                                initApprovers($modalScope, response, "FC");
                                                initApprovers($modalScope, response, "RDD");
                                                initApprovers($modalScope, response, "VPGM");
                                                initApprovers($modalScope, response, "MCCLAssetDtr");
                                                initApprovers($modalScope, response, "CDO");
                                                initApprovers($modalScope, response, "ManagingDirector");
                                                initApprovers($modalScope, response, "MCCLAssetMgr");
                                                $modalScope.Approvers.NoticeUsers = response.data.NoticeUsers || [];
                                                $modalScope.Approvers.NecessaryNoticeUsers = response.data.NecessaryNoticeUsers || [];
                                                $modalScope.dataLoaded = true;
                                            }, function (response) {
                                                $modalInstance.dismiss("cancel");
                                                messager.showMessage(response.data.Message, "fa-warning c_orange");
                                            });

                                            projectUsersService.getNecessaryNotifyUsers(usCode, flowCode).then(function (response) {
                                                if (response.data && response.data != null) {
                                                    $modalScope.necessaryNoticeUserCode = response.data.UserCodes;
                                                    $modalScope.necessaryNoticeRoleNames = response.data.RoleNames;
                                                    $modalScope.necessaryNoticeRoleUser = response.data.RoleUser;
                                                }
                                                $modalScope.isNecessaryLoading = false;
                                            });
                                            $modalScope.selectEmployee = function () {
                                                $selectUser.open({
                                                    checkUsers: function (selectedUsers) {
                                                        return true;
                                                    },
                                                    selectedUsers: angular.copy($modalScope.Approvers.NoticeUsers || []),
                                                    OnUserSelected: function (selectedUsers) {
                                                        $modalScope.Approvers.NoticeUsers = $.map(selectedUsers, function (u, i) {
                                                            return {
                                                                Code: u.Code,
                                                                NameZHCN: u.NameZHCN,
                                                                NameENUS: u.NameENUS
                                                            };
                                                        });
                                                    }
                                                });
                                            };
                                            $modalScope.selectNecessary = function () {
                                                $selectUser.open({
                                                    checkUsers: function (selectedUsers) {
                                                        return true;
                                                    },
                                                    selectedUsers: angular.copy($modalScope.Approvers.NecessaryNoticeUsers || []),
                                                    scopeUserCodes: $modalScope.necessaryNoticeUserCode,
                                                    OnUserSelected: function (selectedUsers) {
                                                        $modalScope.Approvers.NecessaryNoticeUsers = $.map(selectedUsers, function (u, i) {
                                                            return {
                                                                Code: u.Code,
                                                                NameZHCN: u.NameZHCN,
                                                                NameENUS: u.NameENUS
                                                            };
                                                        });
                                                    }
                                                });
                                            };
                                            $modalScope.ok = function (frm) {
                                                if (frm.$valid) {
                                                    var errors = [];
                                                    if ($modalScope.Approvers.NecessaryNoticeUsers == undefined || $modalScope.Approvers.NecessaryNoticeUsers == null || $modalScope.Approvers.NecessaryNoticeUsers.length == 0) {
                                                        errors.push("[[[请选择必要抄送人]]]");
                                                    }
                                                    else {
                                                        var selectedRoleCode = [];
                                                        angular.forEach($modalScope.Approvers.NecessaryNoticeUsers, function (r, i) {
                                                            angular.forEach($modalScope.necessaryNoticeRoleUser, function (s, j) {
                                                                if (r.Code == s.UserCode) {
                                                                    selectedRoleCode.push(s.RoleName);
                                                                }
                                                            });
                                                        });
                                                        angular.forEach($modalScope.necessaryNoticeRoleNames.split(','), function (r, i) {
                                                            var result = false;
                                                            angular.forEach(selectedRoleCode, function (s, j) {
                                                                if (s == r)
                                                                    result = true;
                                                            });
                                                            if (!result)
                                                                errors.push("[[[请选择必要抄送人]]]" + r);
                                                        });
                                                    }
                                                    if (errors.length > 0) {
                                                        messager.showMessage(errors.join("<br />"), "fa-warning c_red");
                                                        return;
                                                    };
                                                    $modalInstance.close($modalScope.Approvers);
                                                }
                                            };
                                            $modalScope.cancel = function () {
                                                $modalInstance.dismiss("cancel");
                                            };
                                        }
                                    ]
                                }).result;
                            }
                            break;
                    }
                    return modalResult;
                }
            };
        }
    ]).factory("notifyApprovalDialogService", [
        '$modal',
        'projectUsersService',
        function ($modal, projectUsersService) {
            return {
                open: function (projectId, flowCode, usCode, roleCode) {
                    var templateUrl;
                    switch (flowCode) {
                        case "TempClosure_ClosureMemo":
                        case "TempClosure_ReopenMemo":
                            templateUrl = "/TempClosureModule/ClosureMemoSelNotice";
                            break;
                        default:
                            templateUrl = "/TempClosureModule/ClosureMemoSelNotice";
                            break;
                    }
                    return $modal.open({
                        templateUrl: templateUrl,
                        backdrop: 'static',
                        size: 'md',
                        controller: [
                            "$scope",
                            "$modalInstance",
                            "$selectUser",
                            function (noticeScope, $modalInstance, $selectUser) {
                                projectUsersService.getNotifyUsers(usCode, projectId, roleCode).then(function (response) {
                                    noticeScope.entity = response.data;
                                });
                                noticeScope.ok = function () {
                                    $modalInstance.close(noticeScope.NoticeUsers);
                                };
                                noticeScope.selectEmployee = function () {
                                    $selectUser.open({
                                        storeCode: usCode,
                                        positionCodes: ["suoya612036", "suoya303055", "suoya303054"],
                                        checkUsers: function () { return true; },
                                        OnUserSelected: function (users) {
                                            noticeScope.NoticeUsers = users;
                                            noticeScope.NoticeUsersNameENUS = $.map(users || [], function (u) {
                                                return u.NameENUS;
                                            }).join(";");
                                        }
                                    });
                                };
                                noticeScope.cancel = function () {
                                    $modalInstance.dismiss("cancel");
                                };
                            }
                        ]
                    }).result;
                }
            };
        }
    ])
    .filter("NoticeUsersNameENUS", function () {
        return function (users) {
            return $.map(users || [], function (u) {
                return u.UserNameENUS == undefined ? u.NameENUS : u.UserNameENUS;
            }).join(";");
        };
    })
