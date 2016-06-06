dictionaryApp.controller('closureLegalReviewController', [
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


        $scope.flowCode = "Closure_LegalReview";

        $scope.userAccount = window.currentUser.Code;
        $scope.userNameENUS = window.currentUser.NameENUS;
        $scope.userNameZHCN = window.currentUser.NameZHCN;

        $scope.checkPointRefresh = true;
        $scope.pageUrl = window.location.href;


        $scope.uploadAttachFinished = function (up, file) {
            messager.showMessage("[[[上传成功]]]", "fa-check c_green");
        };

        //获取项目基本数据
        $http.get(Utils.ServiceURI.Address() + "api/Closure/GetByProjectId/" + $scope.projectId).success(function (data) {
            $scope.ClosureInfo = data;
        });


        $http.get(Utils.ServiceURI.Address() + "api/LegalReview/GetByProjectId/" + $scope.projectId + "/" + window.currentUser.Code).success(function (fdata) {

            $scope.legalView = fdata;
            //entityGuid = 
        }).then(function (entity) {
            if ($scope.legalView != "null") {
                $scope.guid = $scope.legalView.Id;
                $http.get(Utils.ServiceURI.Address() + "api/LegalReview/GetClosureCommers/ClosureLegalReview/" + $scope.legalView.Id.toString()).success(function (closureCommers) {
                    $scope.closureCommers = closureCommers;

                });


            } else {
                $scope.legalView = {};
                $scope.guid = Utils.Generator.newGuid();
                $scope.legalView.Id = $scope.guid;
                $scope.legalView.projectId = $scope.projectId;
            }



        });

        $http.get(Utils.ServiceURI.Address() + "api/Closure/GetByProjectId/" + $scope.projectId).success(function (data) {
            $scope.entity = data;
        });

        $scope.saveLegalReview = function () {
            if (!$scope.legalView.Id) {

                $scope.legalView.Id = $scope.guid;
                $scope.legalView.ProjectId = $scope.projectId;
            }



            $scope.legalView.UserAccount = window.currentUser.Code;
            $scope.legalView.UserNameZHCN = window.currentUser.NameZHCN;
            $scope.legalView.UserNameENUS = window.currentUser.NameENUS;
            $http.post(Utils.ServiceURI.Address() + "api/LegalReview/SaveClosureLegalReview", $scope.legalView).success(function (data) {
                messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                // $window.location = Utils.ServiceURI.WebAddress() + "/redirect";
            }).error(function (data) {
                messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
            });

        }

        $scope.submitLegalReview = function () {

            if (!$scope.legalView.Id) {

                $scope.legalView.Id = $scope.guid;
                $scope.legalView.ProjectId = $scope.projectId;
            }


            $scope.legalView.UserAccount = window.currentUser.Code;
            $scope.legalView.UserNameZHCN = window.currentUser.NameZHCN;
            $scope.legalView.UserNameENUS = window.currentUser.NameENUS;
            $http.post(Utils.ServiceURI.Address() + "api/LegalReview/PostClosureLegalReview", $scope.legalView).success(function (data) {
                messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                    //$window.location = Utils.ServiceURI.WebAddress() + "/redirect";
                    messager.unBlockUI();
                    redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                });
            }).error(function (data) {
                messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
            });

        }


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

                $scope.submitLegalReview();


            });

        };



    }
]);