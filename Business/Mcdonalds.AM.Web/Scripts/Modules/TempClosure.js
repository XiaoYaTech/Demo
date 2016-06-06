/// <reference path="../Libs/moment/moment.d.ts" />
/// <reference path="../Libs/Angular/Plugins/angular-ui-bootstrap.d.ts" />
/// <reference path="../Libs/Angular/Plugins/angular-cookies.d.ts" />
/// <reference path="../Utils/Utils.ts" />
/// <reference path="../Libs/Angular/angular.d.ts" />
(function () {
    var myModule = angular.module("mcd.am.modules.tempClosure", []);
    myModule.factory("tempClosurePackageApproverService", [
        "$modal",
        "messager",
        "$selectUser",
        "tempClosureService",
        function ($modal, messager, $selectUser, tempClosureService) {
            return {
                ShowSelector: function (projectId) {
                    return $modal.open({
                        backdrop: "static",
                        size: "lg",
                        templateUrl: Utils.ServiceURI.AppUri + "TempClosureModule/SelectApprovers",
                        controller: [
                            "$scope",
                            "$modalInstance",
                            function ($modalScope, $modalInstance) {
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
                                    $modalScope.data = data;
                                    $modalScope.approversLoaded = true;
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
                                $modalScope.ok = function () {
                                    var errors = [];
                                    var approvers = $modalScope.data.ProjectDto.ApproveUsers;
                                    if (!approvers.MarketMgr) {
                                        errors.push("请选择Market Manager");
                                    }
                                    ;
                                    if (!approvers.MDD) {
                                        errors.push("请选择MDD");
                                    }
                                    ;
                                    if (!approvers.GM) {
                                        errors.push("请选择GM");
                                    }
                                    ;
                                    if (!approvers.FC) {
                                        errors.push("请选择FC");
                                    }
                                    ;
                                    if (!approvers.VPGM) {
                                        errors.push("请选择VPGM");
                                    }
                                    ;
                                    if (!approvers.MCCLAssetMgr) {
                                        errors.push("请选择MCCL Asset Manager");
                                    }
                                    ;
                                    if (!approvers.MCCLAssetDtr) {
                                        errors.push("请选择MCCL Asset Director");
                                    }
                                    ;
                                    if (errors.length > 0) {
                                        messager.showMessage(errors.join("<br />"), "fa-warning c_red");
                                        return;
                                    }
                                    ;
                                    $modalInstance.close($modalScope.data);
                                };
                                $modalScope.cancel = function () {
                                    $modalInstance.dismiss('');
                                };
                            }
                        ]
                    }).result;
                }
            };
        }]).filter("NoticeUsersNameENUS", function () {
            return function (users) {
                return $.map(users || [], function (u) {
                    return u.NameENUS;
                }).join(";");
            };
        }).directive("reliefRent", [function () {
            return {
                restrict: "EA",
                scope: {
                    editable: "=",
                    entity: "="
                },
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "TempClosureModule/ReliefRent",
                link: function ($scope, ele, attrs) {
                    $scope.$watch("entity", function (val) {
                        if (!!val && !!$scope.entity) {
                            if ($scope.entity.RentRelief != undefined) {
                                $scope.RentRelief = $scope.entity.RentRelief.toString().toLowerCase();
                                resetInputs($scope.entity.RentRelief);
                            }
                            else {
                                $scope.RentRelief = "true";
                                resetInputs(true);
                            }
                        }
                    });
                    $scope.$watch("RentRelief", function (val) {
                        if (!!val && !!$scope.entity) {
                            if ($scope.RentRelief == "true") {
                                $scope.entity.RentRelief = true;
                            }
                            else {
                                $scope.entity.RentRelief = false;
                            }
                            resetInputs($scope.entity.RentRelief);
                        }
                    });
                    $scope.openDate = function ($event, tag) {
                        $event.preventDefault();
                        $event.stopPropagation();
                        $scope[tag] = true;
                    };
                    $scope.changeRentRelief = function () {
                        emptyValue();
                    };
                    var emptyValue = function () {
                        $scope.entity.RentReliefStartDate = null;
                        $scope.entity.RentReliefEndDate = null;
                        $scope.entity.RentReliefClause = null;
                    };
                    var resetInputs = function (val) {
                        setCtrlStatus($scope.frmRR.rentReliefStartDate, val);
                        setCtrlStatus($scope.frmRR.rentReliefEndDate, val);
                        setCtrlStatus($scope.frmRR.rentReliefClause, val);
                    };
                    var setCtrlStatus = function (ctrl, sel) {
                        if (!sel) {
                            ctrl.$setValidity("required", true);
                        } else {
                            if (!ctrl.$modelValue)
                                ctrl.$setValidity("required", false);
                        }
                    };
                }
            };
        }
        ]);
})();
