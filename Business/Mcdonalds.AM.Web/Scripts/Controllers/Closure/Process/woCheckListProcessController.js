dictionaryApp.controller('woCheckListProcessController', [
    '$scope',
    "$http",
    "$modal",
    "$window",
    '$routeParams',
    '$location',
    'closureCreateHandler',
    "messager",
    "redirectService",
    function ($scope, $http, $modal, $window, $routeParams, $location, closureCreateHandler, messager, redirectService) {

        var procInstID = $routeParams.ProcInstID;
        var sn = $routeParams.SN;
        $scope.flowCode = "Closure_WOCheckList";
        $scope.checkPointRefresh = false;
        $scope.entity = {};
        $scope.projectId = $routeParams.projectId;
        if (!$scope.projectId) {

            $http.get(Utils.ServiceURI.Address() + "api/project/GetProjectIDByProcInstID/" + procInstID).success(function (data) {
                if (data != "null") {
                    $scope.projectId = data.replace("\"", "").replace("\"", "");
                }
                loadData();
            });
        } else {
            loadData();
        }


        function loadData() {

            $scope.isEditor = false;
            $scope.checkPointRefresh = true;
            //获取项目基本数据
            $http.get(Utils.ServiceURI.Address() + "api/Closure/GetByProjectId/" + $scope.projectId).success(function (data) {
                if (data != "null") {
                    $scope.ClosureInfo = data;

                }
            });

            $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetByProjectId/" + $scope.projectId, {
                cache: false,
                params: {
                    userAccount: window.currentUser.Code
                }
            }
            ).success(function (data) {
                if (data != "null") {
                    $scope.entity = data;
                    $scope.isUploadTemplate = true;
                }

            }).then(function (data) {
                if ($scope.entity != "null") {


                    $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetTemplates/" + $scope.entity.Id.toString()).success(function (atts) {
                        $scope.templateList = atts;
                        for (var i = 0; i < atts.length; i++) {
                            atts[i].IsAvailable = (i == atts.length - 1) ? "[[[生效]]]" : "[[[失效]]]";
                        }

                        $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetHistoryList/" + $scope.projectId).success(function (historyData) {
                            $scope.historyList = historyData;
                        }).error(function (err) {
                            var s = err;
                        });

                        //$http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetClosureCommers/ClosureWOCheckList/" + $scope.entity.Id.toString()).success(function (closureCommers) {
                        //    $scope.closureCommers = closureCommers;

                        //});

                    });

                }
            });



        }

        $scope.ApproverSubmit = function (action) {
            $scope.submiting = true;
            $scope.entity.Action = action;
            $scope.entity.SN = sn;

            if (action == "Return" && !$scope.entity.Comments) {
                $scope.submiting = false;
                messager.showMessage("[[[请填写意见]]]", "fa-warning c_orange");
                return false;
            };

            $scope.entity.ProcInstID = procInstID;
            $scope.entity.UserAccount = window.currentUser.Code;
            $scope.entity.UserNameZHCN = window.currentUser.NameZHCN;
            $scope.entity.UserNameENUS = window.currentUser.NameENUS;
            messager.confirm(action == "Return" ? "[[[确定要进行退回吗？]]]" : "[[[确定要进行审批吗？]]]", "fa-warning c_orange").then(function (result) {
                if (result) {
                    messager.blockUI("[[[正在处理中，请稍等...]]]");
                    $http.post(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/ProcessClosureWOCheckList/", $scope.entity).success(function (successData) {
                        messager.showMessage(Utils.Common.resolveAlertMsg(action), "fa-check c_green").then(function () {
                            //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                            messager.unBlockUI();
                            redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                        });
                    }).error(function (err) {
                        $scope.submiting = false;
                        messager.unBlockUI();
                        messager.showMessage(Utils.Common.resolveAlertMsg(action, true), "fa-warning c_orange");
                    });
                }
                else {
                    $scope.submiting = false;
                    messager.unBlockUI();
                }
            });
        };


        //根据K2的状态跳转页面

        $scope.entity = {};
        $scope.entity.UserAccount = window.currentUser.Code;
        $scope.entity.Username = window.currentUser.NameZHCN + "(" + window.currentUser.NameENUS + ")";

        var k2ApiUrl = Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetK2Status/" + window.currentUser.Code + "/" + sn + "/" + procInstID;
        $http.get(k2ApiUrl).success(function (statusData) {

            if (statusData.Status == "Process") {
                loadData();
            } else if (statusData.Status == "Edit") {
                var url = "/WOCheckList/Process/Resubmit";
                $location.path(url);
            }
        });

    }]);