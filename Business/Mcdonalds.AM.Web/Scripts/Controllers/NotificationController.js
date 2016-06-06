var notificationControllers = angular.module("mcd.am.controller.notification", []);

notificationControllers.controller('notificationCtrl', [
    '$scope',
    'notificationService',
    '$location',
    '$anchorScroll',
    "messager",
    function ($scope, notificationService, $location, $anchorScroll, messager) {

        $scope.pageIndex = 1;
        $scope.pageSize = 10;
        $scope.totalItems = 0;

        $scope.searchCondition = {
            PageIndex: $scope.pageIndex,
            PageSize: $scope.pageSize
        };

        $scope.$watch("pageIndex", function (oldVal, newVal) {
            if (oldVal != newVal) {
                $scope.searchCondition.PageIndex = newVal;
                $scope.pagging();
            };
        });

        $scope.pagging = function () {

            notificationService.query($scope.searchCondition).$promise.then(function (response) {
                $scope.notificationList = response.data;
                $scope.totalItems = response.totalSize;
                $scope.showDetail = false;
            });

        };


        $scope.edit = function (notification) {
            if (!notification.HasRead) {
                notification.HasRead = true;

                notificationService.save(notification).$promise.then(function (response) {
                    $scope.entity = notification;
                    $scope.showDetail = true;

                });
            } else {
                $scope.entity = notification;
                $scope.showDetail = true;
            }
            $scope.goto('divDetail');
        }

        $scope.search = function () {
            $scope.pageIndex = 1;
            $scope.pagging();
        }

        $scope.goto = function (id) {
            window.parent.scrollTo(0, $('#' + id).offset().top);
            //$location.hash(id);
            //$anchorScroll();
        }

        $scope.search();
    }]);


