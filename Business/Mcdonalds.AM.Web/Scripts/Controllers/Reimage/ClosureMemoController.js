reimageApp.controller("closureMemoController", [
    '$scope',
    '$routeParams',
    "$location",
    "$selectUser",
    "$window",
    "taskWorkService",
    "tempClosureService",
    "redirectService",
    "messager",
    function ($scope, $routeParams, $location, $selectUser, $window, taskWorkService, tempClosureService, redirectService,messager) {
        $scope.projectId = $routeParams.projectId;
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
        
        $scope.openDate = function ($event, tag) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope[tag] = true;
        };

        $scope.checkPointRefresh = true;
        $scope.entity = {};
        $scope.acting = false;

        $scope.save = function () {
            $scope.acting = true;
            tempClosureService.saveClosureMemo($scope.entity).$promise.then(function (response) {
                messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                $scope.acting = false;
            }, function (response) {
                messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
                $scope.acting = false;
            });
        };
        $scope.notify = function (frm) {
            if (!frm.$valid) {
                return;
            }
            $selectUser.open({
                storeCode: $scope.entity.USCode,
                positionCodes: ["suoya612036", "suoya303055", "suoya303054"],
                checkUsers: function () { return true; },
                OnUserSelected: function (users) {
                    $scope.acting = true;
                    tempClosureService.sendClosureMemo({
                        Entity: $scope.entity,
                        Receivers: users
                    }).$promise.then(function (response) {
                        messager.showMessage("[[[发送成功]]]", "fa-check c_green").then(function () {
                            //$window.location = Utils.ServiceURI.WebAddress() + "/redirect";
                            redirectService.flowRedirect("Reimage_TempClosureMemo", $scope.projectId);
                        });
                    }, function (response) {
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