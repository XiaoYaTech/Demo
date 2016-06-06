dictionaryApp.controller('closureToolProcessController', [
    '$scope',
    "$http",
    "$routeParams",
    "$window",
    '$location',
    'closureCreateHandler',
    "messager",
    "redirectService",
    function ($scope, $http, $routeParams, $window, $location, closureCreateHandler, messager, redirectService) {

        var procInstID = $routeParams.ProcInstID;
        var sn = $routeParams.SN;
        $scope.projectId = $routeParams.projectId;
        $scope.entity = {};
        $scope.entity.ProjectId = $scope.projectId;

        $scope.flowCode = "Closure_ClosureTool";

        $scope.isActor = false;
        $scope.pageUrl = window.location.href;

        var userAccount = window.currentUser.Code;
        $scope.userAccount = userAccount;
        $scope.userNameZHCN = escape(window.currentUser.NameZHCN);
        $scope.userNameENUS = escape(window.currentUser.NameENUS);

        var k2ApiUrl = Utils.ServiceURI.Address() + "api/ClosureConsInvtChecking/GetK2Status/" + userAccount + "/" + sn + "/" + procInstID;


        $http.get(k2ApiUrl).success(function (data) {

            if (data.Status == "Process") {
                loadData();
            } else if (data.Status == "Edit") {
                var url = "/ClosureTool/Process/Resubmit";//?SN=" + $routeParams.SN + "&ProcInstID=" + $routeParams.ProcInstID + "&projectId=" + $routeParams.projectId;

                $location.path(url);
            }
        });
        
        $scope.genClosureTool = function () {
            var u = Utils.ServiceURI.Address() + "api/ClosureTool/EnableGenClosureTool/" + $scope.entity.Id.toString();
            $http.get(u).success(function (result) {

                if ($.trim(result) != "true") {
                    messager.showMessage(result, "fa-warning c_orange");
                } else {
                    $scope.Save(function () {
                        var url = Utils.ServiceURI.Address() + "api/ClosureTool/GenClosureTool/" + $scope.entity.Id;
                        $http.get(url).success(function (atts) {
                            messager.showMessage("[[[生成成功！]]]", "fa-check c_green");
                            switch (atts.Extension.toLowerCase()) {
                                case ".xlsx":
                                case ".xls":
                                    atts.Icon = "fa fa-file-excel-o c_green";
                                    break;
                                case ".ppt":
                                    atts.Icon = "fa fa-file-powerpoint-o c_red";
                                    break;
                                case ".doc":
                                case ".docx":
                                    atts.Icon = "fa fa-file-word-o c_blue";
                                    break;
                                default:
                                    atts.Icon = "fa fa-file c_orange";
                                    break;
                            }
                            $scope.ClosureTool = atts;
                            $scope.checkPointRefresh = true;
                            if ($scope.reloadAtt)
                                $scope.reloadAtt = $scope.reloadAtt + 1;
                            else
                                $scope.reloadAtt = 1;

                            $http.get(Utils.ServiceURI.Address() + "api/ClosureTool/CallClosureTool/" + $scope.entity.Id).success(function (result) {
                                $scope.entity.TotalOneOffCosts = result.TotalOneOffCosts;
                                $scope.entity.OperatingIncome = result.OperatingIncome;
                                $scope.entity.CompensationReceipt = result.CompensationReceipt;
                                $scope.entity.ClosingCosts = result.ClosingCosts;
                                $scope.entity.NPVSC = result.NPVSC;
                            });
                        });
                    });
                }
            });
        }

        function loadData() {
            loadDataByProjectId();
        }

        function loadDataByProjectId() {
            $scope.checkPointRefresh = true;
            var url = Utils.ServiceURI.Address() + "api/projectusers/IsExists/" + $scope.projectId + "/" + window.currentUser.Code + "/Asset Actor";
            $http.get(url).success(function (data) {
                if (data.toLowerCase() == "true")
                    $scope.isActor = true;
                else
                    $scope.isActor = false;
                $scope.decisionEditable = !$scope.isActor;
                if (!!$scope.entity.Id) {
                    $scope.isLoadFinished = true;
                }
            });

            $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetByProjectId/" + $scope.projectId).success(function (data) {
                $scope.woCheckList = data;
            });

            //获取项目基本数据
            $http.get(Utils.ServiceURI.Address() + "api/Closure/GetByProjectId/" + $scope.projectId).success(function (data) {
                if (data != "null") {
                    $scope.ClosureInfo = data;
                }
            });

            $http.get(Utils.ServiceURI.Address() + "api/ClosureTool/GetClosureToolByProjectId/" + $scope.projectId).success(function (data) {

                if (data != "null") {
                    $scope.entity = data;
                }
                if ($scope.decisionEditable != undefined) {
                    $scope.isLoadFinished = true;
                }
            });
        }

        $scope.DoSubmit = function (frm) {
            if (!frm.$valid)
                return;
            if (($scope.entity.McppcoMargin && $scope.entity.McppcoMargin != 0) || ($scope.entity.MccpcoCashFlow && $scope.entity.MccpcoCashFlow != 0)) {
                var flag = false;
                if (!$scope.selStore1 && !$scope.selStore2) {
                    flag = true;
                }
                else if ($scope.selStore1 && (!$scope.selStore1.StoreCode || $scope.selStore1.StoreCode == "") && $scope.selStore2 && (!$scope.selStore2.StoreCode || $scope.selStore2.StoreCode == "")) {
                    flag = true;
                }
            }
            if (flag) {
                messager.showMessage("[[[Impact On Other Stores 至少填写一家餐厅]]]", "fa-warning c_orange");
                return;
            }
            $scope.ActorSubmit('Submit');
        };

        $scope.ApproverSubmit = function (action) {
            $scope.submiting = true;
            if (action == "Return" && !$scope.entity.Comments) {
                $scope.submiting = false;
                messager.showMessage("[[[请填写意见]]]", "fa-warning c_orange");
                return false;
            };

            $scope.entity.SN = sn;
            $scope.entity.Action = action;
            $scope.entity.ProcInstID = procInstID;

            var userAccount = window.currentUser.Code;
            $scope.entity.UserAccount = userAccount;
            $scope.entity.Username = window.currentUser.NameZHCN + "(" + window.currentUser.NameENUS + ")";
            messager.confirm(action == "Return" ? "[[[确定要进行退回吗？]]]" : "[[[确定要进行审批吗？]]]", "fa-warning c_orange").then(function (result) {
                if (result) {
                    messager.blockUI("[[[正在处理中，请稍等...]]]");
                    $http.post(Utils.ServiceURI.Address() + "api/ClosureTool/ProcessClosureTool/", {
                        Entity: $scope.entity
                    }).success(function (successData) {

                        messager.showMessage(Utils.Common.resolveAlertMsg(action), "fa-check c_green").then(function () {
                            //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                            messager.unBlockUI();
                            redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                        });
                    }).error(function (err) {
                        $scope.submiting = false;
                        messager.showMessage(Utils.Common.resolveAlertMsg(action, true), "fa-warning c_orange");
                    });
                }
                else
                    $scope.submiting = false;
            });
        };

        $scope.Save = function (callback) {
            prePostData();
            
            var impactStores = [];
            if (!!$scope.selStore1) {
                impactStores.push($scope.selStore1);
            }
            if (!!$scope.selStore2) {
                impactStores.push($scope.selStore2);
            }

            var url = Utils.ServiceURI.Address();
            url = url + "api/ClosureTool/UpdateClosureTool";

            $http.post(url, {
                Entity: $scope.entity,
                ImpactStores: impactStores
            }).success(function (data) {
                callback && callback();
            }).error(function (data) {
                messager.showMessage("[[[生成失败]]]", "fa-check c_orange");
            });
        }
        function prePostData() {
            //checkDecisionLogic();
            $scope.entity.ProjectId = $scope.projectId;
        }
        function checkDecisionLogic() {
            if (!$scope.entity.IsOptionOffered) {
                $scope.entity.LeaseTerm = null;
                $scope.entity.Investment = null;
                $scope.entity.NPVRestaurantCashflows = null;
                $scope.entity.Yr1SOI = null;
                $scope.entity.IRR = null;
                $scope.entity.CompAssumption = null;
                $scope.entity.CashflowGrowth = null;
            }
        }
        $scope.ActorSubmit = function (action) {
            $scope.submiting = true;
            if ($scope.isActor == true && action == "Submit") {
                //if (!$scope.ClosureTool || $scope.ClosureTool == "null" || $scope.ClosureTool.length == 0) {
                //    messager.showMessage("请先生成ClosureTool", "fa-warning c_orange");
                //    $scope.submiting = false;
                //    return false;
                //}
                if ($.grep($scope.ClosureToolAttachment || [], function (att, i) {
                    return !!att.FileURL && att.RequirementId == "64a039c8-04a6-44f3-9310-f041d233a997";
                }) == 0) {
                    messager.showMessage("[[[请先生成ClosureTool]]]", "fa-warning c_orange");
                    $scope.submiting = false;
                    return false;
                }
                if (!$scope.entity.ConclusionComment) {
                    messager.showMessage("[[[请填写Comments and Conclusion]]]", "fa-warning c_orange");
                    $scope.submiting = false;
                    return false;
                }
                if (!$scope.entity.Compensation) {
                    messager.showMessage("[[[请填写Compensation]]]", "fa-warning c_orange");
                    $scope.submiting = false;
                    return false;
                }
            }
            var impactStores = [];
            if (!!$scope.selStore1) {
                impactStores.push($scope.selStore1);
            }
            if (!!$scope.selStore2) {
                impactStores.push($scope.selStore2);
            }

            $scope.entity.SN = sn;
            $scope.entity.Action = action;
            $scope.entity.ProcInstID = procInstID;

            var userAccount = window.currentUser.Code;
            $scope.entity.UserAccount = userAccount;
            $scope.entity.Username = window.currentUser.NameZHCN + "(" + window.currentUser.NameENUS + ")";

            if (action == "Submit") {
                $http.post(Utils.ServiceURI.Address() + "api/ClosureTool/PostActorClosureTool/", {
                    Entity: $scope.entity,
                    ImpactStores: impactStores
                }).success(function (successData) {

                    //提交时自动重新生成ClosureTool
                    var url = Utils.ServiceURI.Address() + "api/ClosureTool/GenClosureTool/" + $scope.entity.Id;
                    $http.get(url).success(function (atts) {
                        $http.get(Utils.ServiceURI.Address() + "api/ClosureTool/CallClosureTool/" + $scope.entity.Id);
                    });

                    messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                        //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                        messager.unBlockUI();
                        redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                    });
                }).error(function (err) {
                    $scope.submiting = false;
                    messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
                });
            }
            else {
                $http.post(Utils.ServiceURI.Address() + "api/ClosureTool/UpdateClosureTool/", {
                    Entity: $scope.entity,
                    ImpactStores: impactStores
                }).success(function (data) {
                    $scope.submiting = false;
                    messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                    $scope.checkPointRefresh = true;
                }).error(function (data) {
                    $scope.submiting = false;
                    messager.showMessage("[[[保存失败]]]", "fa-warning c_orange");
                });
            }
        };
    }]);