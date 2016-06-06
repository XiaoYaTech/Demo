dictionaryApp.controller('executiveSummaryController', [
    '$scope',
    "$http",
    "$modal",
    "$window",
    '$routeParams',
    'closureCreateHandler',
    "messager",
    "redirectService",
    function ($scope, $http, $modal, $window, $routeParams, closureCreateHandler, messager, redirectService) {


        $scope.projectId = $routeParams.projectId;
        $scope.isHistory = $routeParams.isHistory;
        $scope.isUserAction = Utils.Common.isUserAction($routeParams.from);

        $scope.userAccount = window.currentUser.Code;
        // $scope.userName = window.currentUser.NameZHCN + "(" + window.currentUser.NameENUS + ")";
        $scope.userNameZHCN = window.currentUser.NameZHCN;
        $scope.userNameENUS = window.currentUser.NameENUS;
        $scope.entityId = $routeParams.entityId;


        $scope.flowCode = "Closure_ExecutiveSummary";
        $scope.checkPointRefresh = true;
        $scope.editable = true;

        $("#downLoadTemplate").attr("href", Utils.ServiceURI.Address() + "api/ClosureExecutiveSummary/DownLoadTemplate/" + $routeParams.projectId);

        $scope.uploadTplFinish = function () {
            $scope.checkPointRefresh = true;
            $window.location = Utils.ServiceURI.WebAddress() + "Closure/Main#/ExecutiveSummary?projectId=" + $scope.projectId + "&nocache=" + Math.random();
        };

        $http.get(Utils.ServiceURI.Address() + "api/ClosureExecutiveSummary/GetByProjectId/" + $scope.projectId).success(function (data) {
            if (data != "null") {
                $scope.entity = data.entity;
                $scope.enableReCall = data.enableReCall;
                $scope.enableEdit = data.enableEdit;
            }
        });

        $http.get(Utils.ServiceURI.Address() + "api/project/isFlowSavable/" + $scope.projectId + "/" + $scope.flowCode).success(function (data) {
            if (data) {
                $scope.enableSave = data.Savable;
            }
        });

        //获取项目基本数据
        $http.get(Utils.ServiceURI.Address() + "api/Closure/GetByProjectId/" + $scope.projectId).success(function (data) {

            if (data != "null") {

                $scope.ClosureInfo = data;
            }
        });


        $scope.ActorSubmit = function () {

            //if (!$scope.hasES) {
            //    messager.showMessage("请先生成ExecutiveSummary", "fa-warning c_red");
            //    return false;
            //}
            if ($.grep($scope.ExecutiveSummaryAttachment || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "79258ffb-c2ef-4eff-897d-ba8376c90071";
            }) == 0) {
                messager.showMessage("[[[请先生成ExecutiveSummary]]]", "fa-warning c_orange");
                return false;
            }
            if (!$scope.entity || $scope.entity == "null") {
                $scope.entity = {};
            }
            $scope.entity.UserAccount = window.currentUser.Code;
            $scope.entity.UserNameZHCN = window.currentUser.NameZHCN;
            $scope.entity.UserNameENUS = window.currentUser.NameENUS;
            $scope.entity.ProjectId = $scope.projectId;

            //提交前重新生成一次
            $http.post(Utils.ServiceURI.Address() + "api/ClosureExecutiveSummary/GenExecutiveSummaty", $scope.entity).success(function (result) {
                $http.post(Utils.ServiceURI.Address() + "api/LegalReview/PostExcutiveSummary", $scope.entity).success(function (postData) {
                    messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                        //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                        messager.unBlockUI();
                        redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                    });
                }).error(function (err) {
                    messager.showMessage("[[[提交失败]]]", "fa-warning c_red");
                });
            });
        };

        $scope.saveExecutiveSummary = function () {
            if (!$scope.entity || $scope.entity == "null") {
                $scope.entity = {};
            }
            $scope.entity.UserAccount = window.currentUser.Code;
            $scope.entity.UserNameZHCN = window.currentUser.NameZHCN;
            $scope.entity.UserNameENUS = window.currentUser.NameENUS;
            $scope.entity.ProjectId = $scope.projectId;

            if ($.grep($scope.ExecutiveSummaryAttachment || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "79258ffb-c2ef-4eff-897d-ba8376c90071";
            }) != 0) {
                $http.post(Utils.ServiceURI.Address() + "api/ClosureExecutiveSummary/GenExecutiveSummaty", $scope.entity);
            }

            $http.post(Utils.ServiceURI.Address() + "api/ClosureExecutiveSummary/SaveExcutiveSummary", $scope.entity).success(function (postData) {
                messager.showMessage("[[[保存成功]]]", "fa-check c_green").then(function () {
                });
            }).error(function (err) {
                messager.showMessage("[[[保存失败]]]", "fa-warning c_red");
            });
        };

        $scope.genExecutiveSummary = function () {
            $http.post(Utils.ServiceURI.Address() + "api/ClosureExecutiveSummary/GenExecutiveSummaty", $scope.entity).success(function (postData) {
                messager.showMessage("[[[生成成功！]]]", "fa-check c_green");
                $scope.checkPointRefresh = true;
                if ($scope.reloadAtt)
                    $scope.reloadAtt = $scope.reloadAtt + 1;
                else
                    $scope.reloadAtt = 1;
            });
        };

        $scope.editExecuiveSummary = function () {
            messager.confirm("[[[Closure Executive Summary 流程已经完成，现在编辑会导致流程重新提交，是否确认？]]]", "fa-warning c_orange")
                .then(function (result) {
                    if (result) {
                        $http.post(Utils.ServiceURI.Address() + "api/ClosureExecutiveSummary/Edit", $scope.entity).success(function (response) {
                            $window.location.href = Utils.ServiceURI.WebAddress() + response.TaskUrl;
                        }, function () {
                            messager.showMessage("[[[操作失败]]]", "fa-warning c_orange");
                        });
                    }
                });
        };

        $scope.recallExecuiveSummary = function () {

        };
    }]);