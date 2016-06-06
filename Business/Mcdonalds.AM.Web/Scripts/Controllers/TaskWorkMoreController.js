var taskWorkControllers = angular.module('taskWorkMoreControllers', [
    "mcd.am.service.taskwork"
]);

taskWorkControllers.controller('taskWorkMoreCtrl', [
    '$scope',
    '$http',
    '$routeParams',
    "taskWorkService",
    function ($scope, $http, $routeParams, taskWorkService) {
        $scope.status = $routeParams.status;
        $scope.pageIndex = 1;
        $scope.pageSize = 10;
        $scope.totalItems = 0;
        $scope.list = [];
        $scope.searchCondition = {};
        $scope.sourceCodes = ["Closure", "MajorLease", "Reimage", "Rebuild", "Renewal", "TempClosure"];
        if ($scope.status == 0) {
            $scope.name = "[[[待办列表]]]";
            $scope.isWaitTask = true;
        }
        else if ($scope.status == 2) {
            $scope.name = '[[[已办列表]]]';
            $scope.isDoneTask = true;
        }
        var pagging = function () {
            $scope.loadFinished = false;

            taskWorkService
                .getTasks(window.currentUser.Code, $scope.status, $scope.pageIndex, $scope.pageSize, $scope.title, $scope.sourceCode, $scope.searchCondition)
                .then(function (response) {
                    angular.forEach(response.data.List, function (task, i) {
                        switch (task.TaskType) {
                            case 2:
                                task.TagClass = "task-tag-orange";
                                task.Icon = "fa-external-link";
                                break;
                            case 3:
                            case 5:
                                task.TagClass = "task-tag-red";
                                task.Icon = "fa-warning";
                                break;
                            default:
                                task.TagClass = "task-tag-blue";
                                task.Icon = "fa-thumb-tack";
                                task.ActionName = "Task";
                                break;
                        }
                    });
                    $scope.list = response.data.List;
                    $scope.totalItems = response.data.TotalItems;
                    $scope.loadFinished = true;
                });

        }
        $scope.$watch("pageIndex", function (val) {
            if (!!val) {
                pagging();
            }
        });

        $scope.open = function ($event) {

            $event.preventDefault();
            $event.stopPropagation();

            $scope.opened = true;
        };


        $scope.search = function () {
            $scope.pageIndex = 1;
            pagging();
        };
    }
]);
