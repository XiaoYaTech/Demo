reimageApp.controller('selectPackageCtrl',
[
    "$scope",
    '$window',
    '$location',
    '$routeParams',
    "reimageService",
    '$modal',
     "messager",
    function ($scope, $window, $location, $routeParams, reimageService, $modal, messager) {
        $scope.pageIndex = 1;
        $scope.pageSize = 10;
        $scope.selectedItems = [];
        $scope.isLoading = false;

        $scope.searchCondition = {
            PageIndex: $scope.pageIndex,
            PageSize: $scope.pageSize
        };

        $scope.$watch("pageIndex", function (val) {
            if (!!val) {
                $scope.pagging();
            };
        });

        $scope.pagging = function () {
            
            reimageService.packageList($scope.searchCondition).$promise.then(function (response) {
                $scope.packageList = response.data;
                $scope.totalItems = response.totalSize == 0 ? 1 : response.totalSize;
                $scope.isLoading = true;
            });
        };


        $scope.search = function () {
            $scope.pageIndex = 1;
            $scope.pagging();
        }

        $scope.selectChange = function (pkg) {
            if (pkg.Checked) {
                $scope.selectedItems.push(pkg);
            } else {
                angular.forEach($scope.selectedItems, function (u, i) {
                    if (u.Id == pkg.Id) {
                        $scope.selectedItems.splice(i, 1);
                    }
                });
            }
        };
        $scope.removeItem = function (pkg) {
            angular.forEach($scope.selectedItems, function (u, i) {
                if (u.Id == pkg.Id) {
                    $scope.selectedItems.splice(i, 1);
                }
            });

            angular.forEach($scope.packageList, function (u, i) {
                if (u.Id == pkg.Id) {
                    u.Checked = false;
                }
            });
        };

        $scope.ok = function () {
            reimageService.releasePackages($scope.selectedItems).$promise.then(function (response) {
                $scope.selectedItems = [];
                $scope.search();

                messager.showMessage("呈递成功", "fa-check c_green");

            });
        };
    }
]);