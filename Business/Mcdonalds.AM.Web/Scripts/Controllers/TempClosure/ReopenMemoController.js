!function () {
    var ctrlModule = angular.module("mcd.am.tempClosure.controllers.reopenMemo", []);
    ctrlModule.controller("reopenMemoController", [
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
            $scope.subFlowCode = "TempClosure_ReopenMemo";
            tempClosureService.getReopenMemo({
                projectId: $scope.projectId
            }).$promise.then(function (data) {
                $scope.entity = data.Entity;
                $scope.tempClosureDate = data.TempClosureDate;
                $scope.isActor = data.IsActor;
            });
            $scope.openDate = function ($event, tag) {
                $event.preventDefault();
                $event.stopPropagation();
                $scope[tag] = true;
            };
            $scope.save = function () {
                $scope.acting = true;
                var now = new Date();
                if (Number(moment($scope.entity.ActualConsFinishDate).format("YYYYMMDD")) < Number(moment(now).format("YYYYMMDD"))) {
                    messager.showMessage("实际完工日期不能早于今天", "fa-warning c_orange");
                    $scope.acting = false;
                    return;
                }
                else if (Number(moment($scope.entity.ActualConsFinishDate).format("YYYYMMDD")) < Number(moment($scope.tempClosureDate).format("YYYYMMDD"))) {
                    messager.showMessage("实际完工日期不能早于Closure Date", "fa-warning c_orange");
                    $scope.acting = false;
                    return;
                }
                tempClosureService.saveReopenMemo($scope.entity).$promise.then(function (response) {
                    messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                    $scope.acting = false;
                }, function (response) {
                    messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
                    $scope.acting = false;
                });
            };
            $scope.notify = function (frm) {
                if (frm.$valid) {
                    $scope.acting = true;
                    var now = new Date();
                    if (Number(moment($scope.entity.ActualConsFinishDate).format("YYYYMMDD")) < Number(moment(now).format("YYYYMMDD"))) {
                        messager.showMessage("实际完工日期不能早于今天", "fa-warning c_orange");
                        $scope.acting = false;
                        return;
                    }
                    else if (Number(moment($scope.entity.ActualConsFinishDate).format("YYYYMMDD")) < Number(moment($scope.tempClosureDate).format("YYYYMMDD"))) {
                        messager.showMessage("实际完工日期不能早于Closure Date", "fa-warning c_orange");
                        $scope.acting = false;
                        return;
                    }
                    notifyApprovalDialogService.open(
                        $scope.projectId,
                        $scope.subFlowCode,
                        $scope.entity.USCode,
                        "Coordinator,Actor,Market_Asset_Mgr,Regional_Asset_Mgr,MCCL_Asset_Mgr"
                        ).then(function (NoticeUsers) {
                            messager.blockUI("正在处理中，请稍等...");
                            tempClosureService.sendReopenMemo({
                                Entity: $scope.entity,
                                Receivers: NoticeUsers
                            }).$promise.then(function (response) {
                                messager.showMessage("[[[发送成功]]]", "fa-check c_green").then(function () {
                                    //$window.location.href = Utils.ServiceURI.AppUri + "Home/Main#/taskwork";
                                    messager.unBlockUI();
                                    redirectService.flowRedirect($scope.subFlowCode, $scope.projectId);
                                });
                            }, function (response) {
                                messager.unBlockUI();
                                messager.showMessage("[[[发送失败]]]", "fa-warning c_orange");
                                $scope.acting = false;
                            });
                        }, function () {
                            $scope.acting = false;
                        });
                }
            };
            taskWorkService.ifUndo("TempClosure_ClosureMemo", $scope.projectId).then(function (result) {
                $scope.unNotify = result;
            });
        }
    ]);
}();