!function () {
    var ctrlModule = angular.module("mcd.am.tempClosure.controllers.create", [
        "mcd.am.services.tempClosure"
    ]);
    ctrlModule.controller("createController", [
        "$scope",
        "$window",
        "tempClosureService",
        "messager",
        "$routeParams",
        function ($scope, $window, tempClosureService, messager, $routeParams) {
            var now = new Date();
            $scope.now = now;
            $scope.entity = {
                EstimatedTempClosureDate: now,
                //ActualTempClosureDate: now,
                EstimatedReopenDate: now
                //ActualReopenDate: now
            };
            if ($routeParams.uscode) {
                $scope.entity.USCode = $routeParams.uscode;
            }
            $scope.openDate = function ($event, tag) {
                $event.preventDefault();
                $event.stopPropagation();
                $scope[tag] = true;
            };
            $scope.selectNoticeUsers = function (frm) {
                if (frm.$valid) {
                    var errors = [];
                    if (!$scope.team.AssetRep) {
                        errors.push("请选择Asset Rep！");
                    }

                    if (!$scope.team.AssetActor) {
                        errors.push("[[[请选择Asset Actor！]]]");
                    }

                    if (!$scope.team.Finance) {
                        errors.push("请选择Finance！");
                    }
                    if (!$scope.team.PM) {
                        errors.push("请选择PM！");
                    }
                    if (!$scope.team.AssetMgr) {
                        errors.push("请选择Finance Manager！");
                    }
                    if (!$scope.team.CM) {
                        errors.push("请选择CM！");
                    }
                    if (!$scope.team.Legal) {
                        errors.push("请选择Legal！");
                    }
                    if (!$scope.entity.EstimatedTempClosureDate) {
                        errors.push("请填写Estimated TempClosure Date");
                    }
                    if (!$scope.entity.ActualTempClosureDate) {
                        errors.push("请填写Actual TempClosure Date");
                    }
                    if (!$scope.entity.EstimatedReopenDate) {
                        errors.push("请填写Estimated Re-open Date");
                    }
                    if (!$scope.entity.ActualReopenDate) {
                        errors.push("请填写Actual Re-open Date");
                    }
                    if (!$scope.entity.ClosureReasonCode) {
                        errors.push("请选择ClosureReason");
                    }
                    else if ($scope.entity.ClosureReasonCode == "Others" && !$scope.entity.ClosureReasonRemark) {
                        errors.push("请填写ClosureReason");
                    }
                    if ($scope.entity.ActualTempClosureDate > $scope.entity.ActualReopenDate) {
                        errors.push("ReopenDate不能早于TempClosureDate");
                    }
                    if (errors.length > 0) {
                        messager.showMessage(errors.join("<br />"), "fa-warning c_red");
                        return;
                    }
                    $scope.ShowNotifyUserModalDialog = true;
                }
            };
            $scope.submit = function (notifyUsersInfo) {
                tempClosureService.create({
                    Entity: $scope.entity,
                    Team: $scope.team,
                    //AssetMgr: notifyUsersInfo.AssetMgr,
                    //CM: notifyUsersInfo.CM,
                    Viewers: notifyUsersInfo.NoticeUsers,
                    NecessaryViewers: notifyUsersInfo.NecessaryNoticeUsers
                }).$promise.then(function (data) {
                    messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                        $window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
                    });
                });
            };
        }
    ]);
}();