!function () {
    var ctrlModule = angular.module("mcd.am.tempClosure.controllers.closureMemo", []);
    ctrlModule.controller("closureMemoController", [
        '$scope',
        '$routeParams',
        "$window",
        "$location",
        "$selectUser",
        "taskWorkService",
        "tempClosureService",
        "messager",
        "notifyApprovalDialogService",
        "redirectService",
        function ($scope, $routeParams, $window, $location, $selectUser, taskWorkService, tempClosureService, messager, notifyApprovalDialogService, redirectService) {
            $scope.projectId = $routeParams.projectId;
            $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);
            $scope.checkPointRefresh = true;
            $scope.subFlowCode = "TempClosure_ClosureMemo";
            tempClosureService.getClosureMemo({
                projectId: $scope.projectId
            }).$promise.then(function (data) {
                $scope.entity = data.Entity;
                $scope.isActor = data.IsActor;
            });

            tempClosureService.isTempClosed({
                projectId: $scope.projectId
            }).$promise.then(function (data) {
                $scope.editable = !data.result && data.isActor;
            });

            $scope.openDate = function ($event, tag) {
                $event.preventDefault();
                $event.stopPropagation();
                $scope[tag] = true;
            };
            $scope.save = function () {
                $scope.acting = true;
                var date = $scope.entity.ClosureDate;
                if (!date || date == "") {
                    messager.showMessage("请填写Actual Closure Date ", "fa-error c_orange");
                    return;
                }
                if (!$scope.entity.BecauseOfReimaging &&
                        !$scope.entity.BecauseOfRemodel &&
                        !$scope.entity.BecauseOfDespute &&
                        !$scope.entity.BecauseOfRedevelopment &&
                        !$scope.entity.BecauseOfPlanedClosure &&
                        !$scope.entity.BecauseOfRebuild &&
                        !$scope.entity.BecauseOfOthers) {
                    messager.showMessage("请至少选择一项Reason for Closure", "fa-error c_orange");
                    return;
                }
                var now = new Date();
                if (Number(moment(date).format("YYYYMMDD")) <= Number(moment(now).format("YYYYMMDD"))) {
                    messager.confirm("[[[关店日期早于等于今天,Closure Memo将不能再修改了,您确定要修改吗？]]]", "fa-warning c_red").then(function (result) {
                        if (result) {
                            tempClosureService.saveClosureMemo($scope.entity).$promise.then(function (response) {
                                messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                                $scope.acting = false;
                                $scope.editable = false;
                            }, function (response) {
                                messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
                                $scope.acting = false;
                            });
                        }
                    });
                }
                else {
                    tempClosureService.saveClosureMemo($scope.entity).$promise.then(function (response) {
                        messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                        $scope.acting = false;
                    }, function (response) {
                        messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
                        $scope.acting = false;
                    });
                }
            };
            $scope.notify = function (frm) {
                if (frm.$valid) {
                    $scope.acting = true;
                    var date = $scope.entity.ClosureDate;
                    if (!date || date == "") {
                        messager.showMessage("请填写Actual Closure Date ", "fa-error c_orange");
                        return;
                    }
                    if (!$scope.entity.BecauseOfReimaging &&
                        !$scope.entity.BecauseOfRemodel &&
                        !$scope.entity.BecauseOfDespute &&
                        !$scope.entity.BecauseOfRedevelopment &&
                        !$scope.entity.BecauseOfPlanedClosure &&
                        !$scope.entity.BecauseOfRebuild &&
                        !$scope.entity.BecauseOfOthers) {
                        messager.showMessage("请至少选择一项Reason for Closure", "fa-error c_orange");
                        return;
                    }
                    var now = new Date();
                    if (Number(moment(date).format("YYYYMMDD")) <= Number(moment(now).format("YYYYMMDD"))) {
                        messager.confirm("[[[关店日期早于等于今天,Closure Memo将不能再修改了,您确定要修改吗？]]]", "fa-warning c_red").then(function (result) {
                            if (result) {
                                noticeUsers();
                            }
                        });
                    }
                    else
                        noticeUsers();

                }
            };
            var noticeUsers = function () {
                notifyApprovalDialogService.open(
                        $scope.projectId,
                        $scope.subFlowCode,
                        $scope.entity.USCode,
                        "Coordinator,Actor,Market_Asset_Mgr,Regional_Asset_Mgr,MCCL_Asset_Mgr"
                        ).then(function (NoticeUsers) {
                            messager.blockUI("正在处理中，请稍等...");
                            tempClosureService.sendClosureMemo({
                                Entity: $scope.entity,
                                Receivers: NoticeUsers
                            }).$promise.then(function (response) {
                                messager.showMessage("[[[发送成功]]]", "fa-check c_green").then(function () {
                                    redirectService.flowRedirect($scope.subFlowCode, $scope.projectId);
                                    messager.unBlockUI();
                                });
                            }, function (response) {
                                messager.unBlockUI();
                                messager.showMessage("[[[发送失败]]]", "fa-warning c_orange");
                                $scope.acting = false;
                            });
                        });
            };
            taskWorkService.ifUndo("TempClosure_ClosureMemo", $scope.projectId).then(function (result) {
                $scope.unNotify = result;
            });
            function isLaterDay(input) {
                input = input.replace(/-/g, "/");
                var inputDate = new Date(input);
                var date = new Date();
                var today = date.getFullYear() + "-" + (date.getMonth() + 1) + "-" + date.getDate();
                today = today.replace(/-/g, "/");
                if (inputDate >= new Date(today)) {
                    return true;
                } else {
                    return false;
                }
            }
        }
    ]);
}();