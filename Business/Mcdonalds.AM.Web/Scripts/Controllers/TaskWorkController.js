var taskWorkControllers = angular.module('taskWorkControllers', [
    "mcd.am.service.taskwork"
]);

taskWorkControllers.controller('taskWorkCtrl', [
    '$scope',
    '$http',
    "taskWorkService",
    function ($scope, $http, taskWorkService) {
        if (window.currentUser) {
            taskWorkService
                .getTasks(window.currentUser.Code, 0, 1, 10)
                .then(function (response) {
                    angular.forEach(response.data.List, function (task, i) {
                        switch (task.TaskType) {
                            case 2:
                                task.TagClass = "task-tag-orange";
                                task.Icon = "fa-external-link";
                                break;
                            case 3:
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
                    $scope.todoList = response.data.List;
                    $scope.todoCount = response.data.TotalItems;
                    $scope.loadTodoFinished = true;
                });
            taskWorkService
                .getTasks(window.currentUser.Code, 2, 1, 10)
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
                    $scope.doneList = response.data.List;
                    $scope.doneCount = response.data.TotalItems;
                    $scope.loadDoneFinished = true;
                });
        }
    }
]);
