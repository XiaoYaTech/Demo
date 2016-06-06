dictionaryApp.controller('closureMemoController', [
    '$scope',
    "$http",
    '$routeParams',
    "$location",
    "$selectUser",
    "taskWorkService",
    "closureMemoService",
    "messager",
    "redirectService",
    function ($scope, $http, $routeParams, $location, $selectUser, taskWorkService, closureMemoService, messager, redirectService) {
        $scope.entity = { Creator: window["currentUser"].Code };
        $scope.projectId = $routeParams.projectId;
        $scope.checkPointRefresh = true;
        $scope.editable = false;
        $scope.flowCode = "Closure_ClosureMemo";
        
        closureMemoService.get($scope.projectId).then(function (response) {
            $scope.editable = response.data.Editable;
        });
        $scope.save = function () {
            var date = $scope.entity.ClosureDate;
            if (!date || date == "") {
                messager.showMessage("[[[请填写Actual Closure Date]]] ", "fa-error c_orange");
                return;
            }
            var now = new Date();
            if ($scope.entity.ClosureNature == 1 && Number(moment(date).format("YYYYMMDD")) < Number(moment(now).format("YYYYMMDD"))) {
                messager.confirm("[[[关店日期早于等于今天,Closure Memo将不能再修改了,您确定要修改吗？]]]", "fa-warning c_red").then(function (result) {
                    if (result) {
                        closureMemoService.save($scope.entity).then(function (response) {
                            messager.showMessage("[[[保存成功]]]", "fa-check c_green").then(function () {
                                // $location.path("/Home/Personal");
                            });
                        }, function (response) {
                            messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
                        });
                    }
                });
            }
            else {
                closureMemoService.save($scope.entity).then(function (response) {
                    messager.showMessage("[[[保存成功]]]", "fa-check c_green").then(function () {
                        // $location.path("/Home/Personal");
                    });
                }, function (response) {
                    messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
                });
            }
        };
        $scope.notify = function () {
            var date = $scope.entity.ClosureDate;
            if (!date || date == "") {
                messager.showMessage("[[[请填写Actual Closure Date]]] ", "fa-error c_orange");
                return;
            }
            if (!$scope.entity.BecauseOfReimaging&&
                !$scope.entity.BecauseOfRemodel&&
                !$scope.entity.BecauseOfDespute&&
                !$scope.entity.BecauseOfRedevelopment&&
                !$scope.entity.BecauseOfPlanedClosure&&
                !$scope.entity.BecauseOfRebuild&&
                !$scope.entity.BecauseOfOthers)
            {
                messager.showMessage("[[[请至少选择一项Reason for Closure]]]", "fa-error c_orange");
                return;
            }
            var now = new Date();
            if ($scope.entity.ClosureNature == 1 && Number(moment(date).format("YYYYMMDD")) < Number(moment(now).format("YYYYMMDD"))) {
                messager.confirm("[[[关店日期早于今天,确定要继续发送通知吗？]]]", "fa-warning c_red").then(function (result) {
                    if (result) {
                        $selectUser.open({
                            storeCode: $scope.entity.USCode,
                            positionCodes: ["suoya612036", "suoya303055", "suoya303054"],
                            checkUsers: function () { return true; },
                            OnUserSelected: function (users) {
                                closureMemoService.send($scope.entity, users).then(function (response) {
                                    messager.showMessage("[[[发送成功]]]", "fa-check c_green").then(function () {
                                        //window.location.href = Utils.ServiceURI.WebAddress() + "/redirect";
                                        messager.unBlockUI();
                                        redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                                    });
                                }, function (response) {
                                    messager.showMessage("[[[发送失败]]]", "fa-warning c_orange")
                                });
                            }
                        });
                    }
                });
            }
            else {
                $selectUser.open({
                    storeCode: $scope.entity.USCode,
                    positionCodes: ["suoya612036", "suoya303055", "suoya303054"],
                    checkUsers: function () { return true; },
                    OnUserSelected: function (users) {
                        closureMemoService.send($scope.entity, users).then(function (response) {
                            messager.showMessage("[[[发送成功]]]", "fa-check c_green").then(function () {
                                //window.location.href = Utils.ServiceURI.WebAddress() + "/redirect";
                                messager.unBlockUI();
                                redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                            });
                        }, function (response) {
                            messager.showMessage("[[[发送失败]]]", "fa-warning c_orange")
                        });
                    }
                });
            }
        };
        taskWorkService.ifUndo("Closure_ClosureMemo", $scope.projectId).then(function (result) {
            $scope.unNotify = result;
        });

    }]);