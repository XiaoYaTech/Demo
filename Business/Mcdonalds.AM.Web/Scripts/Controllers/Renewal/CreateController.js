!function () {
    var RenewalApp = angular.module("amApp");
    RenewalApp.controller("createController", [
        "$scope",
        "$window",
        "renewalService",
        "messager",
        "$routeParams",
        function ($scope, $window, renewalService, messager, $routeParams) {
            var now = new Date();
            $scope.entity = {
                CreateTime: now,
                CreateUserAccount: window.currentUser.Code
            };
            if ($routeParams.uscode) {
                $scope.entity.USCode = $routeParams.uscode;
            }
            $scope.openDate = function ($event, tag) {
                $event.preventDefault();
                $event.stopPropagation();
                $scope[tag] = true;
            };
            $scope.$watch("entity.NewLeaseStartDate+entity.NewLeaseEndDate", function () {
                if (!!$scope.entity.NewLeaseStartDate && !!$scope.entity.NewLeaseEndDate) {
                    $scope.entity.RenewalYears = Math.round(Utils.caculator.multiply(($scope.entity.NewLeaseEndDate - $scope.entity.NewLeaseStartDate) / 31536000000, 10)) / 10;
                } else {
                    $scope.entity.RenewalYears = null;
                }
            });
            $scope.selectNoticeUsers = function (frm) {
                var errors = [];
                if ($scope.entity.NewLeaseStartDate > $scope.entity.NewLeaseEndDate) {
                    errors.push("[[[新租约到期日期必须在新租约起始日期之后]]]！");
                }
                if (errors.length > 0) {
                    messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
                }
                if (frm.$valid) {
                    $scope.ShowNotifyUserModalDialog = true;
                }
            };
            $scope.submit = function (notifyUsersInfo) {
                var assetMgr = {
                    UserAccount: $scope.store.StoreDevelop.AssetMgrEid,
                    UserNameENUS: $scope.store.StoreDevelop.AssetMgrName,
                    UserNameZHCN: $scope.store.StoreDevelop.AssetMgrName
                };
                renewalService.create({
                    Entity: $scope.entity,
                    Team: $scope.team,
                    //AssetMgr: assetMgr,
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