dictionaryApp.controller('closureLegalReviewProcessController', [
    '$scope',
    "$http",
    "$modal",
    "$window",
    '$routeParams',
    '$location',
    'closureCreateHandler',
    "messager",
    "contractService",
    "redirectService",
    function ($scope, $http, $modal, $window, $routeParams, $location, closureCreateHandler, messager, contractService, redirectService) {


        var procInstID = $routeParams.ProcInstID;
        $scope.procInstID = procInstID;
        $scope.projectId = $routeParams.projectId;
        $scope.pageUrl = window.location.href;
        $scope.flowCode = "Closure_LegalReview";
        var sn = $routeParams.SN;


        $scope.legalView = {};


        loadData();


        $scope.uploadFinFinish = function (up, files) {
            $scope.checkPointRefresh = true;
        }

        $scope.deleteAttachmentFinish = function (id, requirementId) {
            $scope.checkPointRefresh = true;
        }

        function loadData() {

            //获取项目基本数据
            $http.get(Utils.ServiceURI.Address() + "api/Closure/GetByProjectId/" + $scope.projectId).success(function (data) {
                $scope.ClosureInfo = data;
            });

            $http.get(Utils.ServiceURI.Address() + "api/LegalReview/GetByProjectId/" + $scope.projectId + "/" + window.currentUser.Code).success(function (fdata) {
                $scope.legalView = fdata;
                $scope.checkPointRefresh = true;
            });
        }

        $scope.submitLegalReview = function () {

            $http.post(Utils.ServiceURI.Address() + "api/LegalReview/PostClosureLegalReview", $scope.legalView).success(function (data) {

                //$window.location = Utils.ServiceURI.WebAddress() + "/redirect";
                messager.unBlockUI();
                redirectService.flowRedirect($scope.flowCode, $scope.projectId);
            }).error(function (data) {

            });
        }

        //根据K2的状态跳转页面



        var k2ApiUrl = Utils.ServiceURI.Address() + "api/LegalReview/GetK2Status/" + window.currentUser.Code + "/" + sn + "/" + procInstID;
        $http.get(k2ApiUrl).success(function (successData) {

            if (successData.Status == "Process") {
                //loadData();
            } else if (successData.Status == "Edit") {
                var url = "/Closure/LegalReview/Process/Edit/param";

                $location.path(url);
            }
        });


        $scope.ApproverSubmit = function (action) {

            if (action == "Return" && !$scope.legalView.Comments) {
                $scope.submiting = false;
                messager.showMessage("[[[请填写意见]]]", "fa-warning c_orange");
                return false;
            }

            if (action == "Submit") {

                if (!$scope.legalView.LegalCommers) {
                    $scope.submiting = false;
                    messager.showMessage("[[[请填写法律意见]]]", "fa-warning c_orange");
                    return false;
                }

                if ($.grep($scope.legalReviewAttachments || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "8b086d16-b65b-412f-9e81-013566f732ff";
                }) == 0) {
                    messager.showMessage("[[[请上传Draft Termination Agreement]]]", "fa-warning c_orange");
                    return false;
                }
                //if (!$scope.legalView.Comments) {
                //    $scope.submiting = false;
                //    messager.showMessage("[[[请填写Comments]]]", "fa-warning c_orange");
                //    return false;
                //}
            }


            $scope.legalView.Action = action;
            $scope.legalView.SN = sn;

            $scope.legalView.ProcInstID = procInstID;
            $scope.legalView.UserAccount = window.currentUser.Code;
            $scope.legalView.UserNameZHCN = window.currentUser.NameZHCN;
            $scope.legalView.UserNameENUS = window.currentUser.NameENUS;
            messager.confirm(action == "Return" ? "[[[确定要进行退回吗？]]]" : "[[[确定要进行审批吗？]]]", "fa-warning c_orange").then(function (result) {
                if (result) {
                    messager.blockUI("[[[正在处理中，请稍等...]]]");
                    $http.post(Utils.ServiceURI.Address() + "api/LegalReview/ProcessLegalReview/", $scope.legalView).success(function (processData) {
                        messager.showMessage(Utils.Common.resolveAlertMsg(action), "fa-check c_green").then(function () {
                            //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                            messager.unBlockUI();
                            redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                        });
                    }).error(function (err) {
                        messager.showMessage(Utils.Common.resolveAlertMsg(action, true), "fa-warning c_orange");
                    });
                }
            });

        };
    }
]);