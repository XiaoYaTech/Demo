!function() {
    var RenewalApp = angular.module("amApp");
    RenewalApp.controller("closureMemoController", [
        '$scope',
        '$routeParams',
        "$location",
        "$selectUser",
        "$window",
        "taskWorkService",
        "tempClosureService",
        "redirectService",
        "messager",
        function($scope, $routeParams, $location, $selectUser, $window, taskWorkService, tempClosureService,redirectService, messager) {
            $scope.projectId = $routeParams.projectId;
            $scope.from = $routeParams.from;
            $scope.pageType = $routeParams.PageType;
            switch ($routeParams.PageType) {
            case 'Approval':
            case 'View':
                $scope.isPageEditable = false;
                break;
            default:
                $scope.isPageEditable = true;
                break;
            }

            $scope.openDate = function($event, tag) {
                $event.preventDefault();
                $event.stopPropagation();
                $scope[tag] = true;
            };

            $scope.checkPointRefresh = true;
            $scope.entity = {};
            $scope.acting = false;

            $scope.save = function() {
                $scope.acting = true;
                tempClosureService.saveClosureMemo($scope.entity).$promise.then(function(response) {
                    messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                    $scope.acting = false;
                }, function(response) {
                    messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
                    $scope.acting = false;
                });
            };
            $scope.notify = function (frm) {
                if ($scope.entity.ClosureDate != null && $scope.entity.ClosureDate != "") {
                    if (($scope.entity.BecauseOfReimaging == null || $scope.entity.BecauseOfReimaging == false)
                    && ($scope.entity.BecauseOfRemodel == null || $scope.entity.BecauseOfRemodel == false)
                    && ($scope.entity.BecauseOfDespute == null || $scope.entity.BecauseOfDespute == false)
                    && ($scope.entity.BecauseOfRedevelopment == null || $scope.entity.BecauseOfRedevelopment == false)
                    && ($scope.entity.BecauseOfPlanedClosure == null || $scope.entity.BecauseOfPlanedClosure == false)
                    && ($scope.entity.BecauseOfRebuild == null || $scope.entity.BecauseOfRebuild == false)
                    && ($scope.entity.BecauseOfOthers == null || $scope.entity.BecauseOfOthers == "")) {
                        messager.showMessage("[[[请填写Reason for Closure]]]", "fa-warning c_orange");
                        return;
                    }
                    var tmpDate = moment($scope.entity.ClosureDate);
                    if (tmpDate.isBefore(new Date().toDateString())) {
                        messager.confirm("[[[关店日期早于等于今天,Closure Memo将不能再修改了,您确定要修改吗？]]]", "fa-warning c_red").then(function (result) {
                            if (result) {
                                $scope.submit(frm);
                            }
                        });
                    } else {
                        $scope.submit(frm);
                    }
                }
            };
            $scope.submit = function (frm) {
                if (!frm.$valid) {
                    return;
                }
                $selectUser.open({
                    storeCode: $scope.entity.USCode,
                    positionCodes: ["suoya612036", "suoya303055", "suoya303054"],
                    checkUsers: function () { return true; },
                    OnUserSelected: function (users) {
                        $scope.acting = true;
                        messager.blockUI("[[[正在提交，请稍等...]]]");
                        tempClosureService.sendClosureMemo({
                            Entity: $scope.entity,
                            Receivers: users
                        }).$promise.then(function (response) {
                            messager.unBlockUI();
                            messager.showMessage("[[[发送成功]]]", "fa-check c_green").then(function () {
                                //$window.location = Utils.ServiceURI.WebAddress() + "/redirect";
                                redirectService.flowRedirect("Renewal_ClosureMemo", $scope.projectId);
                            });
                        }, function (response) {
                            messager.unBlockUI();
                            messager.showMessage("[[[发送失败]]]", "fa-warning c_orange");
                            $scope.acting = false;
                        });
                    }
                });
            };
            $scope.unNotify = true;
            //taskWorkService.ifUndo("Rebuild_TempClosureMemo", $scope.projectId).then(function (result) {
            //    $scope.unNotify = result;
            //});
        }
    ]);
}();