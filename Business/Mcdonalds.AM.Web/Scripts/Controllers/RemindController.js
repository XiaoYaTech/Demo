var remindControllers = angular.module('mcd.am.controller.remind', []);

remindControllers.controller('remindCtrl', [
    '$scope',
    'remindService',
     '$location',
    '$anchorScroll',
    "messager",
    function ($scope, remindService, $location, $anchorScroll, messager) {

        $scope.pageIndex = 1;
        $scope.pageSize = 10;

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
            remindService.query($scope.searchCondition).$promise.then(function (response) {
                $scope.remindList = response.data;
                $scope.totalItems = response.totalSize;
                $scope.showDetail = false;
            });
        };

        $scope.edit = function (remind) {
            if (!remind.IsReaded) {
                remind.IsReaded = true;

                remindService.save(remind).$promise.then(function (response) {
                    $scope.entity = remind;
                    $scope.showDetail = true;
                });
            } else {
                $scope.entity = remind;
                $scope.showDetail = true;
            }

            $scope.goto('divDetail');
        }

        $scope.search = function () {
            $scope.pageIndex = 1;
            $scope.pagging();
        }

        $scope.goto = function (id) {
            window.parent.scrollTo(0,$('#' + id).offset().top);
            //$location.hash(id);
            //$anchorScroll();
        }

        $scope.search();
    }]);


