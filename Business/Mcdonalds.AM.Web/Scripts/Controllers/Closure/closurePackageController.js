dictionaryApp.controller('closurePackageController', [
    '$scope',
    "$http",
    "$modal",
    "$window",
    '$routeParams',
    'closureCreateHandler',
    '$selectUser',
    "messager",
    "approveDialogService",
    "redirectService",
    "$location",
    function ($scope, $http, $modal, $window, $routeParams, closureCreateHandler, $selectUser, messager, approveDialogService, redirectService, $location) {

        $scope.projectId = $routeParams.projectId;
        $scope.isHistory = $routeParams.isHistory;
        $scope.pageUrl = window.location.href;
        $scope.action = $routeParams.action;
        $scope.OriginalCFNPV = 0;
        $scope.OtherCFNPV = 0;

        $scope.entity = {};
        $scope.entity.ProjectId = $scope.projectId;
        $scope.entity.OriginalCFNPV = 0;
        $scope.entity.OtherCFNPV = 0;
        $scope.entity.NewSiteNetCFNPV = 0;
        $scope.ClosureTool = {};
        $scope.ClosureTool.Compensation = 0;

        $scope.UserAccount = window.currentUser.Code;
        $scope.UserNameZHCN = window.currentUser.NameZHCN;
        $scope.UserNameENUS = window.currentUser.NameENUS;

        $scope.checkPointRefresh = true;
        $scope.flowCode = "Closure_ClosurePackage";

        $scope.$watch("entity.OtherCFNPV + entity.NewSiteNetCFNPV + ClosureTool.Compensation + entity.OriginalCFNPV", function () {
            try {
                if (!$scope.ClosureTool.Compensation) {
                    $scope.ClosureTool.Compensation = 0;
                }
                //if (!$scope.entity.OtherCFNPV) {
                //    $scope.entity.OtherCFNPV = 0;
                //}
                if (!$scope.entity.NewSiteNetCFNPV) {
                    $scope.entity.NewSiteNetCFNPV = 0;
                }
                if (!$scope.entity.OriginalCFNPV) {
                    $scope.entity.OriginalCFNPV = 0;
                }
                $scope.entity.NetGain = (parseFloat($scope.entity.OtherCFNPV, 10) + parseFloat($scope.entity.NewSiteNetCFNPV, 10) + parseFloat($scope.ClosureTool.Compensation, 10) - parseFloat($scope.entity.OriginalCFNPV, 10)).toFixed(2);
            } catch (e) {
                $scope.entity.NetGain = null;
            }
            if (!$scope.entity.NetGain) {
                $scope.entity.NetGain = 0;
            }
        });

        //获取项目基本数据
        $http.get(Utils.ServiceURI.Address() + "api/Closure/GetByProjectId/" + $scope.projectId).success(function (data) {
            $scope.ClosureInfo = data;
            //获取Store信息
            $http.get(Utils.ServiceURI.Address() + "api/Store/Details/" + data.USCode).success(function (storeData) {
                $scope.store = storeData;
                //$scope.entity.RelocationPipelineID = storeData.StoreBasicInfo.PipelineID;
                //$scope.entity.PipelineName = storeData.StoreBasicInfo.PipelineNameENUS == null ? "无记录" : storeData.StoreBasicInfo.PipelineNameENUS;
            });
        });
        $http.get(Utils.ServiceURI.Address() + "api/ClosureTool/GetClosureToolByProjectId/" + $scope.projectId).success(function (data) {
            $scope.ClosureTool = data;
        });

        $http.get(Utils.ServiceURI.Address() + "api/project/isFlowFinished/" + $scope.projectId + "/" + $scope.flowCode).success(function (data) {
            if (data == "true")
                $scope.isFlowFinished = true;
            else
                $scope.isFlowFinished = false;
        });

        $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetByProjectId/" + $scope.projectId, {
            cache: false,
            params: {
                userAccount: window.currentUser.Code
            }
        }).success(function (data) {
            $scope.ClosureWOCheckList = data;
        });

        $http.get(Utils.ServiceURI.Address() + "api/ClosurePackage/GetByProjectId/" + $scope.projectId).success(function (data) {



        }).then(function (data) {
            //var url = Utils.ServiceURI.Address() + "api/ClosurePackage/PackageAttachment/" + $scope.projectId + "/" + window.currentUser.Code + "/" + $scope.entity.Id;
            if (data.data != 'null') {
                $scope.entity = data.data;
                if (!$scope.entity.OtherCFNPV)
                    $scope.entity.OtherCFNPV = 0;
                //$http.get(Utils.ServiceURI.Address() + "api/ClosurePackage/GetClosureCommers/ClosurePackage/" + $scope.entity.Id).success(function (closureCommers) {
                //    $scope.closureCommers = closureCommers;
                //});

            }
        });

        $http.get(Utils.ServiceURI.Address() + "api/ClosurePackage/EnableSubmit/" + $scope.projectId).success(function (data) {
            if (data == "true")
                $scope.enableSubmit = true;
            else
                $scope.enableSubmit = false;
        });

        $http.get(Utils.ServiceURI.Address() + "api/ClosurePackage/EnableVirtualSubmit/" + $scope.projectId).success(function (data) {
            if (data == "true")
                $scope.enableVirtualSubmit = true;
            else
                $scope.enableVirtualSubmit = false;
        });

        $scope.packageAttachment = function () {

            if ($scope.entity.Id == undefined) {
                messager.showMessage("[[[请先保存]]]", "fa-warning c_orange");
                return;
            }

            $scope.entity.UserAccount = window.currentUser.Code;
            $scope.entity.UserNameZHCN = window.currentUser.NameZHCN;
            $scope.entity.UserNameENUS = window.currentUser.NameENUS;

            var entityJson = JSON.stringify($scope.entity);
            var url = Utils.ServiceURI.Address() + "api/ClosurePackage/SaveClosurePackage";
            $http.post(url, $scope.entity).success(function (data) {
                url = Utils.ServiceURI.Address() + "api/ClosurePackage?projectId=" + $scope.projectId;
                $scope.packDownloadLink = url;
            })


        }

        $scope.$watch("entity.NetGain", function (val) {

            if (!!val) {
                if (val < 0) {

                    $("#spNetGain").addClass("c_red");
                } else {
                    $("#spNetGain").removeClass("c_red");
                }
            }
        });

        $scope.postClosurePackage = function () {

            if ($scope.entity.NetGain < 0 && ($scope.entity.ReasonDescriptionForNegativeNetGain == null || $.trim($scope.entity.ReasonDescriptionForNegativeNetGain) == "")) {
                messager.showMessage("[[[请填写Reason Description for Negative Net CF NPV!]]]", "fa-check c_orange");
            }
            else {
                commit("Submit");
            }
        };
        var validate = function () {
            //if (!$scope.entity.OriginalCFNPV) {
            //    messager.showMessage("请填写Original CF NPV!", "fa-check c_orange");
            //    return false;
            //}
            //if (!$scope.entity.NetOperatingIncome) {
            //    messager.showMessage("请填写Net Operating Income!", "fa-check c_orange");
            //    return false;
            //}
            ////if (!$scope.entity.RelocationPipelineID) {
            ////    messager.showMessage("请填写Relocation Pipeline ID !", "fa-check c_orange");
            ////    return false;
            ////}
            ////if (!$scope.entity.PipelineName) {
            ////    messager.showMessage("请填写Relocation Pipeline Name !", "fa-check c_orange");
            ////    return false;
            ////}
            ////if (!$scope.entity.NewSiteNetCFNPV) {
            ////    messager.showMessage("请填写New Site Net CF NPV!", "fa-check c_orange");
            ////    return false;
            ////}
            //if (!$scope.entity.OtherCFNPV) {
            //    messager.showMessage("请填写Other CF NPV !", "fa-check c_orange");
            //    return false;
            //}
            //if (!$scope.entity.ReasonDescriptionForNegativeNetGain) {
            //    messager.showMessage("请填写Reason Description For Negative Net Gain!", "fa-check c_orange");
            //    return false;
            //}
            return true;
        };
        $scope.savePackage = function (closureInfo) {
            //var exp = /^([1-9][\d]{0,7}|0)(\.[\d]{1,2})?$/;
            //if (!exp.test($scope.entity.OtherCFNPV)) {
            //    messager.showMessage("Other CF NPV 输入不正确", "fa-check c_orange");
            //    return;
            //}
            commit("Save");
        }

        //$scope.FinishPackage = function (closureInfo) {
        //    //var exp = /^([1-9][\d]{0,7}|0)(\.[\d]{1,2})?$/;
        //    //if (!exp.test($scope.entity.OtherCFNPV)) {
        //    //    messager.showMessage("Other CF NPV 输入不正确", "fa-check c_orange");
        //    //    return;
        //    //}
        //    if (!$scope.signedPackageUrl) {
        //        messager.showMessage("请上传Signed Package", "fa-check c_orange");
        //        return;
        //    }
        //    commit("Finish");
        //}

        $scope.FinishPackage = function (action) {
            //if (!$scope.signedPackageUrl) {
            //    messager.showMessage("请上传Signed Package", "fa-check c_orange");
            //    return;
            //}
            messager.blockUI("[[[正在处理中，请稍等...]]]");
            $scope.entity.SN = $routeParams.SN;
            $scope.entity.Action = "Confirm";
            $scope.entity.UserAccount = window.currentUser.Code;
            $scope.entity.UserNameZHCN = window.currentUser.NameZHCN;
            $scope.entity.UserNameENUS = window.currentUser.NameENUS;
            $scope.entity.ProcInstID = $routeParams.ProcInstID;

            $http.post(Utils.ServiceURI.Address() + "api/ClosurePackage/ProcessClosurePackage", $scope.entity).success(function (data) {
                messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                    messager.unBlockUI();
                    redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                });
            }).error(function (data) {
                messager.unBlockUI();
                messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
            });
        };

        //选择处理人
        $scope.beginSelApprover = function (frm, closureInfo) {
            if (!frm.$valid) {
                return;
            }
            $scope.entity.NetGain = (parseFloat($scope.entity.OtherCFNPV, 10) + parseFloat($scope.entity.NewSiteNetCFNPV, 10) + parseFloat($scope.ClosureTool.Compensation, 10) - parseFloat($scope.entity.OriginalCFNPV, 10)).toFixed(2);

            if ($scope.entity.NetGain < 0 && ($scope.entity.ReasonDescriptionForNegativeNetGain == null || $.trim($scope.entity.ReasonDescriptionForNegativeNetGain) == "")) {
                messager.showMessage("[[[请填写Reason Description for Negative Net CF NPV!]]]", "fa-check c_orange");
                return false;
            }

            approveDialogService.open($scope.projectId, $scope.flowCode, "Submit", closureInfo.USCode).then(function (storeEntity) {
                var rddCode = !!storeEntity.selDD ? storeEntity.selDD.Code + ";" : "";
                $scope.entity.DD_GM_FC_RDD = rddCode + storeEntity.selGM.Code + ";" + storeEntity.selFC.Code;
                if (storeEntity.selRDD != null && storeEntity.selRDD.Code != null)
                    $scope.entity.DD_GM_FC_RDD = $scope.entity.DD_GM_FC_RDD + ";" + storeEntity.selRDD.Code;

                $scope.entity.DD = rddCode;
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
                if (storeEntity.selMngDirector)
                { $scope.entity.MngDirector = storeEntity.selMngDirector.Code; }
                //if (storeEntity.MCCLAssetMgr)
                //{ $scope.entity.MCCLAssetMgr = storeEntity.MCCLAssetMgr.Code; }
                if (storeEntity.MCCLAssetDtr)
                { $scope.entity.MCCLAssetDtr = storeEntity.MCCLAssetDtr.Code; }
                if (storeEntity.NoticeUsers && storeEntity.NoticeUsers.length > 0)
                { $scope.entity.NoticeUsers = storeEntity.NoticeUsers; }
                if (storeEntity.NecessaryNoticeUsers && storeEntity.NecessaryNoticeUsers.length > 0)
                { $scope.entity.NecessaryNoticeUsers = storeEntity.NecessaryNoticeUsers; }

                $scope.postClosurePackage();
            }, function () {
            });
        }


        //function loadPackageAttchment() {
        //    $http.get(Utils.ServiceURI.Address() + "api/ClosurePackage/LoadAttachment/" + $scope.projectId).success(function (data) {
        //    });
        //}

        function commit(type) {
            $scope.entity.ProjectId = $scope.projectId;

            //var sel = $("#dicIsDirectory").val();
            //if (sel == "true") {
            //    $scope.entity.IsRelocation = true;
            //} else {
            //    $scope.entity.IsRelocation = false;
            //}

            $scope.entity.UserAccount = window.currentUser.Code;
            $scope.entity.UserNameZHCN = window.currentUser.NameZHCN;
            $scope.entity.UserNameENUS = window.currentUser.NameENUS;
            messager.blockUI("[[[正在处理中，请稍等...]]]");

            var url = Utils.ServiceURI.Address();
            if (type == "Save") {
                url = url + "api/ClosurePackage/SaveClosurePackage";
            }
            else if (type == "Submit") {
                url = url + "api/ClosurePackage/PostClosurePackage";
            }
            else if (type == "Finish") {
                url = url + "api/ClosurePackage/ProcessClosurePackage";
            }

            $http.post(url, $scope.entity).success(function (data) {
                messager.unBlockUI();
                if (type == "Submit" || type == "Finish") {
                    messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                        //$window.location = Utils.ServiceURI.WebAddress() + "/redirect";
                        messager.unBlockUI();
                        redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                    });
                }
                else {
                    messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                    $scope.entity.Id = data.replace("\"", "").replace("\"", "");
                }
            }).error(function (data) {
                messager.unBlockUI();
                if (type == "Submit" || type == "Finish") {
                    messager.unBlockUI();
                    messager.showMessage("[[[提交失败]]]", "fa-warning c_red");
                }
                else {
                    messager.showMessage("[[[保存失败]]]", "fa-check c_orange");
                }
            });
        }
    }]);

