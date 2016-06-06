dictionaryApp.controller('closurePackageProcessController', [
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

        $scope.entity = {};
        $scope.ClosureTool = {};
        $scope.isHistory = $routeParams.isHistory;
        //$scope.pageUrl = window.location.href;
        var procInstID = $routeParams.ProcInstID;
        $scope.projectId = $routeParams.projectId;
        $scope.enableReject = false;
        $scope.flowCode = "Closure_ClosurePackage";
        var sn = $routeParams.SN;

        $scope.$watch("entity.OtherCFNPV + entity.NewSiteNetCFNPV + ClosureTool.Compensation + entity.OriginalCFNPV", function () {
            try {
                if (!$scope.ClosureTool.Compensation) {
                    $scope.ClosureTool.Compensation = 0;
                }
                if (!$scope.entity.OtherCFNPV) {
                    $scope.entity.OtherCFNPV = 0;
                }
                if (!$scope.entity.NewSiteNetCFNPV) {
                    $scope.entity.NewSiteNetCFNPV = 0;
                }
                if (!$scope.entity.OriginalCFNPV) {
                    $scope.entity.OriginalCFNPV = 0;
                }
                $scope.entity.NetGain = (parseFloat($scope.entity.OtherCFNPV) + parseFloat($scope.entity.NewSiteNetCFNPV) + parseFloat($scope.ClosureTool.Compensation) - parseFloat($scope.entity.OriginalCFNPV));
            } catch (e) {
                $scope.entity.NetGain = null;
            }
            if (!$scope.entity.NetGain) {
                $scope.entity.NetGain = "0";
            }
        });


        function loadData() {

            $scope.isEditor = false;
            $scope.checkPointRefresh = true;
            //获取项目基本数据
            $http.get(Utils.ServiceURI.Address() + "api/Closure/GetByProjectId/" + $scope.projectId).success(function (data) {
                if (data != "null") {

                    $scope.ClosureInfo = data;
                    //获取Store信息
                    $http.get(Utils.ServiceURI.Address() + "api/Store/Details/" + data.USCode).success(function (storeData) {
                        $scope.store = storeData;
                        //$scope.entity.RelocationPipelineID = storeData.StoreBasicInfo.PipelineID;
                        //$scope.entity.PipelineName = storeData.StoreBasicInfo.PipelineNameENUS == null ? "无记录" : storeData.StoreBasicInfo.PipelineNameENUS;
                    });
                }
            });
            $http.get(Utils.ServiceURI.Address() + "api/ClosureTool/GetClosureToolByProjectId/" + $scope.projectId).success(function (data) {
                if (data != "null") {
                    $scope.ClosureTool = data;
                }
            });

            loadAttachment();

            $http.get(Utils.ServiceURI.Address() + "api/ClosurePackage/GetByProjectId/" + $scope.projectId).success(function (data) {
                $scope.entity = data;

            }).then(function () {
                if ($scope.entity != "null") {
                    $http.get(Utils.ServiceURI.Address() + "api/ClosurePackage/GetClosureCommers/ClosurePackage/" + $scope.entity.Id.toString()).success(function (closureCommers) {
                        $scope.closureCommers = closureCommers;
                    });
                }
            });

            $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetByProjectId/" + $scope.projectId, {
                cache: false,
                params: {
                    userAccount: window.currentUser.Code
                }
            }).success(function (data) {
                if (data != "null") {
                    $scope.ClosureWOCheckList = data;
                }
            });

            enableReject();

        }



        $scope.packageAttachment = function () {


            $scope.entity.UserAccount = window.currentUser.Code;
            $scope.entity.UserNameZHCN = window.currentUser.NameZHCN;
            $scope.entity.UserNameENUS = window.currentUser.NameENUS;

            var entityJson = JSON.stringify($scope.entity);

            var url = Utils.ServiceURI.Address() + "api/ClosurePackage?projectId=" + $scope.projectId;
            $scope.packDownloadLink = url;
            //$("#form1")[0].action = url;
            //$("#form1").submit();
            //    $.ajax({
            //        type: "Post",
            //        contentType: "application/json",
            //        url: url,

            //        data: entityJson,

            //        dataType: "json",
            //        async: false

            //    }
            //);


        }


        function loadAttachment() {

            $http.get(Utils.ServiceURI.Address() + "api/ClosurePackage/LoadAttachment/" + $scope.projectId).success(function (data) {
                var downLoadAttUrl = Utils.ServiceURI.Address() + "api/ClosurePackage/DownLoadAttachment/";
                for (var i = 0; i < data.length; i++) {
                    if (data[i].RefTableName == "ClosureExecutiveSummary") {

                        $scope.exAttUrl = downLoadAttUrl + data[i].ID;
                    }

                    else if (data[i].RefTableName == "ClosureWOCheckList") {

                        $scope.woAttUrl = downLoadAttUrl + data[i].ID;
                    }

                    else if (data[i].RefTableName == "ClosureTool") {
                        $scope.closureToolAttUrl = downLoadAttUrl + data[i].ID;
                    }

                    else if (data[i].TypeCode == "SignedPackage") {
                        $scope.signedPackageUrl = downLoadAttUrl + data[i].ID;
                    }
                    else if (data[i].TypeCode == "FinAgreement") {
                        $scope.finAgreementUrl = downLoadAttUrl + data[i].ID;
                    }
                    else if (data[i].TypeCode == "Cover") {
                        $scope.coverUrl = downLoadAttUrl + data[i].ID;
                    }
                    else if (data[i].TypeCode == "Others") {
                        $scope.othersUrl = downLoadAttUrl + data[i].ID;
                    }
                }
            }).error(function (data) {

            });
        }

        //根据K2的状态跳转页面


        //var k2ApiUrl = Utils.ServiceURI.Address() + "api/ClosurePackage/GetK2Status/" + window.currentUser.Code + "/" + sn + "/" + procInstID;
        //$http.get(k2ApiUrl).success(function (data) {

        //    if (data.Status == "Edit") {
        //        var url = "/ClosurePackage/Process/Approval";
        //        $location.path(url);
        //    } else {
        //        loadData();
        //    }
        //});

        function enableReject() {
            var url = Utils.ServiceURI.Address() + "api/ClosurePackage/EnableReject/" + sn;
            $http.get(url).success(function (result) {
                if (result == "true") {
                    $scope.enableReject = true;
                } else {
                    $scope.enableReject = false;
                }
            });
        }

        $scope.ApproverSubmit = function (action) {

            if (action == "Return" && !$scope.entity.Comments) {
                messager.showMessage("[[[请填写意见]]]", "fa-warning c_orange").then(function () {
                    window.parent.scrollTo(0, $("#divComments textarea").offset().top);
                });
                return false;
            }
            if (action == "Decline" && !$scope.entity.Comments) {
                messager.showMessage("[[[请填写意见]]]", "fa-warning c_orange").then(function () {
                    window.parent.scrollTo(0, $("#divComments textarea").offset().top);
                });
                return false;
            }
            if (action == "Decline") {
                messager.confirm("[[[Reject后该流程将直接终止，请确认需要执行该操作吗？]]]", "fa-warning c_orange")
                    .then(function (result) {
                        if (result) {
                            $scope.postAction(action);
                        }
                    });
            } else if (action == "Return") {
                messager.confirm("[[[确定要进行退回吗？]]]", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        $scope.postAction(action);
                    }
                });
            } else {
                messager.confirm("[[[确定要进行审批吗？]]]", "fa-warning c_orange").then(function (result) {
                    if (result) {
                        $scope.postAction(action);
                    }
                });
            }
        };

        $scope.postAction = function (action) {
            messager.blockUI("[[[正在处理中，请稍等...]]]");
            $scope.entity.SN = sn;
            $scope.entity.Action = action;
            $scope.entity.UserAccount = window.currentUser.Code;
            $scope.entity.UserNameZHCN = window.currentUser.NameZHCN;
            $scope.entity.UserNameENUS = window.currentUser.NameENUS;
            $scope.entity.ProcInstID = procInstID;

            $http.post(Utils.ServiceURI.Address() + "api/ClosurePackage/ProcessClosurePackage", $scope.entity).success(function (data) {
                messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                    //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                    messager.unBlockUI();
                    redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                });
            }).error(function (data) {
                messager.unBlockUI();
                messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
            });
        };
        loadData();
    }]);

