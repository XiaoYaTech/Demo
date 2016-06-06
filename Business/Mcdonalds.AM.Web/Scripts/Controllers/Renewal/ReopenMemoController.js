!function() {
    var RenewalApp = angular.module("amApp");
    RenewalApp.controller("reopenMemoController", [
        '$scope',
        '$routeParams',
        "$location",
        "$selectUser",
        "$window",
        "taskWorkService",
        "reopenMemoService",
        "redirectService",
        "messager",
        function ($scope, $routeParams, $location, $selectUser, $window, taskWorkService, reopenMemoService, redirectService,messager) {
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
            var checkValidate = function () {
                var gbDate = $scope.entity.GBDate;
                var consDate = $scope.entity.CompletionDate;
                var reopDate = $scope.entity.ReopenDate;
                if (gbDate != null && consDate != null && reopDate != null) {
                    gbDate = moment(moment(gbDate).format("YYYY-MM-DD"));
                    consDate = moment(moment(consDate).format("YYYY-MM-DD"));
                    reopDate = moment(moment(reopDate).format("YYYY-MM-DD"));
                    if (consDate.isBefore(gbDate)) {
                        messager.showMessage("[[[Completion Date 不能早于 GB Date]]]", "fa-warning c_orange");
                        return false;
                    }
                    if (reopDate.isBefore(consDate)) {
                        messager.showMessage("Reopen Date 不能早于 Completion Date", "fa-warning c_orange");
                        return false;
                    }
                }
                if ($scope.entity.DesignConcept == null || $scope.entity.DesignConcept == "") {
                    messager.showMessage("请选择 Design Concept!", "fa-warning c_orange");
                    //$scope.entity.isNotSelectDesignConcept = true;
                    return false;
                }
                if ($scope.entity.AftARPT == null || $scope.entity.AftARPT == "") {
                    messager.showMessage("请选择 After Price Tier!", "fa-warning c_orange");
                    //$scope.entity.isNotSelectPriceTiter = true;
                    return false;
                }
                if ($scope.entity.ExteriorAfterImg1 == null
                || $scope.entity.ExteriorAfterImg1 == ""
                || $scope.entity.ExteriorAfterImg1.toLowerCase().indexOf("mcd_logo") != -1) {
                    messager.showMessage("请上传 Exterior After !", "fa-warning c_orange");
                    return false;
                }
                if ($scope.entity.ExteriorAfterImg2 == null
                    || $scope.entity.ExteriorAfterImg2 == ""
                    || $scope.entity.ExteriorAfterImg2.toLowerCase().indexOf("mcd_logo") != -1) {
                    messager.showMessage("请上传 Interior After !", "fa-warning c_orange");
                    return false;
                }
                if ($scope.entity.InteriorAfterImg1 == null
                    || $scope.entity.InteriorAfterImg1 == ""
                    || $scope.entity.InteriorAfterImg1.toLowerCase().indexOf("mcd_logo") != -1) {
                    messager.showMessage("请上传 Exterior Before !", "fa-warning c_orange");
                    return false;
                }
                if ($scope.entity.InteriorAfterImg2 == null
                    || $scope.entity.InteriorAfterImg2 == ""
                    || $scope.entity.InteriorAfterImg2.toLowerCase().indexOf("mcd_logo") != -1) {
                    messager.showMessage("请上传 Interior Before !", "fa-warning c_orange");
                    return false;
                }
                return true;
            };
            $scope.checkPointRefresh = true;
            $scope.entity = {};
            $scope.acting = false;
            $scope.save = function() {
                $scope.acting = true;
                reopenMemoService.saveReopenMemo($scope.entity).$promise.then(function(response) {
                    messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                    $scope.acting = false;
                }, function(response) {
                    messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
                    $scope.acting = false;
                });
            };
            $scope.notify = function (frm) {
                if (!checkValidate()) {
                    return;
                }
                if (!frm.$valid) {
                    return;
                }
                $selectUser.open({
                    storeCode: $scope.entity.Store.StoreBasicInfo.StoreCode,
                    positionCodes: ["suoya612036", "suoya303055", "suoya303054"],
                    checkUsers: function() { return true; },
                    OnUserSelected: function (users) {
                        $scope.acting = true;
                        reopenMemoService.sendReopenMemo({
                            Entity: $scope.entity,
                            Receivers: users
                        }).$promise.then(function(response) {
                            messager.showMessage("[[[发送成功]]]", "fa-check c_green").then(function() {
                                //$window.location = Utils.ServiceURI.WebAddress() + "/redirect";
                                redirectService.flowRedirect("Renewal_ReopenMemo", $scope.projectId);
                            });
                        }, function(response) {
                            messager.showMessage("[[[发送失败]]]", "fa-warning c_orange");
                            $scope.acting = false;
                        });
                    }
                });
            };
            $scope.unNotify = true;
            //taskWorkService.ifUndo("Rebuild_ReopenMemo", $scope.projectId).then(function (result) {
            //    $scope.unNotify = result;
            //});
        }
    ]);
}();