dictionaryApp.controller('closurePackageProcessEditController', [
    '$scope',
    "$http",
    "$modal",
    "$window",
    '$routeParams',
    'closureCreateHandler',
    "messager",
    "approveDialogService",
    "redirectService",
    function ($scope, $http, $modal, $window, $routeParams, closureCreateHandler, messager, approveDialogService, redirectService) {

        $scope.isHistory = $routeParams.isHistory;
        $scope.pageUrl = window.location.href;
        $scope.projectId = $routeParams.projectId;
        $scope.OriginalCFNPV = 0;

        $scope.OtherCFNPV = 0;

        $scope.entity = {};
        $scope.entity.ProjectId = $scope.projectId;
        $scope.entity.OriginalCFNPV = 0;
        $scope.entity.OtherCFNPV = 0;
        $scope.entity.NewSiteNetCFNPV = 0;
        $scope.ClosureTool = {};
        $scope.ClosureTool.Compensation = 0;
        var procInstID = $routeParams.ProcInstID;
        $scope.projectId = $routeParams.projectId;
        var sn = $routeParams.SN;

        $scope.flowCode = "Closure_ClosurePackage";

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
                    //获取Store信息
                    $http.get(Utils.ServiceURI.Address() + "api/Store/Details/" + data.USCode).success(function (storeData) {
                        if (storeData != "null") {
                            $scope.store = storeData;
                            //$scope.entity.RelocationPipelineID = storeData.StoreBasicInfo.PipelineID;
                            //$scope.entity.PipelineName = storeData.StoreBasicInfo.PipelineNameENUS == null ? "无记录" : storeData.StoreBasicInfo.PipelineNameENUS;
                        }
                    });
                }
            });
            $http.get(Utils.ServiceURI.Address() + "api/ClosureTool/GetClosureToolByProjectId/" + $scope.projectId).success(function (data) {
                if (data != "null") {
                    $scope.ClosureTool = data;
                }
            });


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

        }

        //选择处理人
        $scope.beginSelApprover = function (frm, closureInfo) {
            if (!frm.$valid)
                return;

            $scope.entity.NetGain = $scope.entity.OtherCFNPV + $scope.entity.NewSiteNetCFNPV + $scope.ClosureTool.Compensation - $scope.entity.OriginalCFNPV;

            if ($scope.entity.NetGain < 0 && ($scope.entity.ReasonDescriptionForNegativeNetGain == null || $.trim($scope.entity.ReasonDescriptionForNegativeNetGain) == "")) {
                messager.showMessage("[[[请填写Reason Description for Negative Net CF NPV!]]]", "fa-check c_orange");
                return false;
            }
            approveDialogService.open($scope.projectId, $scope.flowCode, "ReSubmit", closureInfo.USCode).then(function (storeEntity) {

                var rddCode = !!storeEntity.selDD ? storeEntity.selDD.Code + ";" : "";
                $scope.entity.DD_GM_FC_RDD = rddCode + storeEntity.selGM.Code + ";" + storeEntity.selFC.Code;
                if (storeEntity.selRDD != null && storeEntity.selRDD.Code!=null)
                    $scope.entity.DD_GM_FC_RDD = $scope.entity.DD_GM_FC_RDD + ";" + storeEntity.selRDD.Code;


                $scope.entity.DD = storeEntity.selDD;
                $scope.entity.MDD = storeEntity.selDD.Code;
                $scope.entity.GM = storeEntity.selGM.Code;
                $scope.entity.FC = storeEntity.selFC.Code;

                $scope.entity.VPGM = storeEntity.selVPGM.Code;
                $scope.entity.CDO = storeEntity.selCDO.Code;
                $scope.entity.CFO = storeEntity.selCFO.Code;
                if (storeEntity.selRDD != null && storeEntity.selRDD.Code != null)
                    $scope.entity.RDD = storeEntity.selRDD.Code;

                if (!!storeEntity.selMarketMgr && !!storeEntity.selRegionalMgr) {
                    $scope.entity.MarketMgr = storeEntity.selMarketMgr.Code + ";" + storeEntity.selRegionalMgr.Code;
                    $scope.entity.MarketMgrCode = storeEntity.selMarketMgr.Code;
                    $scope.entity.RegionalMgrCode = storeEntity.selRegionalMgr.Code;
                }
                else if (!!storeEntity.selMarketMgr) {
                    $scope.entity.MarketMgr = storeEntity.selMarketMgr.Code;
                    $scope.entity.MarketMgrCode = storeEntity.selMarketMgr.Code;
                }
                else if (!!storeEntity.selRegionalMgr) {
                    $scope.entity.MarketMgr = storeEntity.selRegionalMgr.Code;
                    $scope.entity.RegionalMgrCode = storeEntity.selRegionalMgr.Code;
                }

                if (!!storeEntity.selMngDirector) {
                    $scope.entity.MngDirector = storeEntity.selMngDirector.Code;
                }
                //if (storeEntity.MCCLAssetMgr)
                //{ $scope.entity.MCCLAssetMgr = storeEntity.MCCLAssetMgr.Code; }
                if (storeEntity.MCCLAssetDtr)
                { $scope.entity.MCCLAssetDtr = storeEntity.MCCLAssetDtr.Code; }
                if (storeEntity.NoticeUsers && storeEntity.NoticeUsers.length > 0)
                { $scope.entity.NoticeUsers = storeEntity.NoticeUsers; }
                if (storeEntity.NecessaryNoticeUsers && storeEntity.NecessaryNoticeUsers.length > 0)
                { $scope.entity.NecessaryNoticeUsers = storeEntity.NecessaryNoticeUsers; }

                $scope.ApproverSubmit("ReSubmit");

                //$window.location = Utils.ServiceURI.WebAddress()+ "closure/typeahead#/closure/ProjectList";
            }, function () { });
        }

        $scope.ApproverSubmit = function (action) {



            $scope.entity.SN = sn;
            $scope.entity.Action = action;

            $scope.entity.ProcInstID = procInstID;
            $scope.entity.UserAccount = window.currentUser.Code;
            $scope.entity.UserNameZHCN = window.currentUser.NameZHCN;
            $scope.entity.UserNameENUS = window.currentUser.NameENUS;


            $http.post(Utils.ServiceURI.Address() + "api/ClosurePackage/ProcessClosurePackage", $scope.entity).success(function (data) {
                messager.showMessage(Utils.Common.resolveAlertMsg(action), "fa-check c_green").then(function () {
                    //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                    messager.unBlockUI();
                    redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                });
            }).error(function (data) {
                messager.showMessage(Utils.Common.resolveAlertMsg(action, true), "fa-warning c_orange");
            });

        };

        $scope.save = function (action) {
            $scope.entity.Action = action;
            $http.post(Utils.ServiceURI.Address() + "api/ClosurePackage/SaveClosurePackage", $scope.entity).success(function (data) {
                messager.showMessage("[[[保存成功]]]", "fa-check c_green").then(function () {
                    // $window.location = Utils.ServiceURI.WebAddress() + "redirect";
                });
            }).error(function (data) {
                messager.showMessage("[[[保存失败]]]", "fa-warning c_red");
            });
        }
    }]);

