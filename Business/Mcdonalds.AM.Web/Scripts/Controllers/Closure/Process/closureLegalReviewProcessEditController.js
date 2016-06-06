dictionaryApp.controller('closureLegalReviewProcessEditController', [
    '$scope',
    "$http",
    "$modal",
    "$window",
    '$routeParams',
    'closureCreateHandler',
    "messager",
    "contractService",
    "redirectService",
    function ($scope, $http, $modal, $window, $routeParams, closureCreateHandler, messager, contractService, redirectService) {
        
        var procInstID = $routeParams.ProcInstID;
        $scope.procInstID = procInstID;
        $scope.pageUrl = window.location.href;
        var sn = $routeParams.SN;
        $scope.entity = {};
        $scope.projectId = $routeParams.projectId;
        $scope.checkPointRefresh = true;
        $scope.flowCode = "Closure_LegalReview";

        $scope.userAccount = window.currentUser.Code;
        $scope.userNameENUS = window.currentUser.NameENUS;
        $scope.userNameZHCN = window.currentUser.NameZHCN;

        if (!$scope.projectId) {

            $http.get(Utils.ServiceURI.Address() + "api/LegalReview/GetByProcInstID/" + procInstID).success(function (data) {
                $scope.entity = data;


                $scope.projectId = $scope.entity.ProjectId;

                loadData();
            });
        } else {
            loadData();
        }

        function loadData() {

            //获取项目基本数据
            $http.get(Utils.ServiceURI.Address() + "api/Closure/GetByProjectId/" + $scope.projectId).success(function (data) {
                $scope.ClosureInfo = data;
            });

            $http.get(Utils.ServiceURI.Address() + "api/LegalReview/GetByProjectId/" + $scope.projectId + "/" + window.currentUser.Code).success(function (fdata) {
                $scope.legalView = fdata;
            });
        }
        
        $scope.ApproverSubmit = function (action) {


            $scope.legalView.Action = action;
            $scope.legalView.SN = sn;


            $scope.legalView.UserAccount = window.currentUser.Code;
            $scope.legalView.UserNameZHCN = window.currentUser.NameZHCN;
            $scope.legalView.UserNameENUS = window.currentUser.NameENUS;
            $scope.legalView.ProcInstID = procInstID;


            $http.post(Utils.ServiceURI.Address() + "api/LegalReview/ProcessLegalReview/", $scope.legalView).success(function (data) {
                messager.showMessage(Utils.Common.resolveAlertMsg(action), "fa-check c_green").then(function () {
                    //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                    messager.unBlockUI();
                    redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                });
            }).error(function (data) {
                messager.showMessage(Utils.Common.resolveAlertMsg(action, true), "fa-warning c_orange");
            });


        };
        $scope.beginSelApprover = function (closureInfo) {

            $modal.open({
                templateUrl: "/Template/LegalReviewSelApprover",
                backdrop: 'static',
                size: 'lg',
                controller: [
                    "$scope", "$modalInstance", "storeEntity", function ($scope, $modalInstance, storeEntity) {


                        $scope.legalList = [{ "Code": closureInfo.LegalAccount, "NameENUS": closureInfo.LegalNameENUS }];

                        $scope.entity = {};

                        $scope.entity.selLegal = $scope.legalList[0];





                        $scope.ok = function (e) {

                            $modalInstance.close($scope.entity);
                        };
                        $scope.cancel = function () {
                            $modalInstance.dismiss("cancel");
                        };
                    }
                ],
                resolve: {
                    storeEntity: function () {
                        return angular.copy(closureInfo);
                    }
                }

            }).result.then(function (storeEntity) {

                $scope.legalView.LegalAccount = storeEntity.selLegal.Code;

                $scope.ApproverSubmit("ReSubmit");


            });

        };

        $scope.save = function (action) {


            $scope.legalView.Action = action;

            $scope.legalView.UserAccount = window.currentUser.Code;
            $scope.legalView.UserNameZHCN = window.currentUser.NameZHCN;
            $scope.legalView.UserNameENUS = window.currentUser.NameENUS;
            $http.post(Utils.ServiceURI.Address() + "api/LegalReview/SaveClosureLegalReview", $scope.legalView).success(function (data) {
                messager.showMessage("[[[保存成功]]]", "fa-check c_green");

            }).error(function (data) {
                messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
            });

        }


    }
]);