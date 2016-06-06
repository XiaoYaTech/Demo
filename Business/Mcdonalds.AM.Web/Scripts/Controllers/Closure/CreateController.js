/// <reference path="contractInfoController.js" />
function loadData($scope, closureCreateHandler) {






}

function initCondition(pageIndex, pageSize, parentCode) {
    var dictionary = new Object();
    dictionary.PageIndex = pageIndex;
    dictionary.PageSize = pageSize;
    dictionary.ParentCode = parentCode;
    return dictionary;
}


function queryDictionaryByParentCode(handler, parentCode, scope) {
    //if (parentCode == "Save") {
    //    var entity = new Object();
    //    var postEntity = JSON.stringify(entity);
    //    handler.queryDictionary("Code1").$promise.then(function (data) {
    //        var s = data;
    //    });


    //    handler.saveClosureInfo(postEntity).$promise.then(function (data) {

    //    });

    //    handler.beginCreate(postEntity).$promise.then(function (data) {

    //    });
    //}else{

    var dicCondition = initCondition(1, 10, parentCode);

    handler.queryDictionary(dicCondition).$promise.then(function (data) {
        switch (parentCode) {
            case "ClosureType":
                scope.closureTypeDics = data;
                break;
            case "RiskStatus":
                scope.riskStatus = data;
                break;
            case "ClosureReasons":
                scope.closureReasons = data;
                break;
            case "Relocations":
                scope.relocations = data;
                break;
        }

    });
    //}
}


dictionaryApp.controller('closureCreateController', [
    '$scope',
    "$http",
    "$modal",
    'closureCreateHandler',
    '$selectUser',
    '$window',
    '$location',
    'messager',
    '$routeParams',
    function ($scope, $http, $modal, closureCreateHandler, $selectUser, $window, $location, messager, $routeParams) {
        //只有角色为Market Asset Mgr（RoleID=15）和Asset Rep(RoleID=40)才可以创建.
        //if (window.currentUserRoles.toString().indexOf('40') < 0 && window.currentUserRoles.toString().indexOf('15') < 0) {
        //    messager.showMessage("您没有权限访问该页面", "fa-warning c_orange").then(function () {
        //        $window.location.href = Utils.ServiceURI.WebAddress() + "redirect";
        //    });
        //    return false;
        //}
        var USCode = "";
        $scope.entity = {};
        $scope.selNoticeUsers = {};
        $scope.selNecessaryNoticeUsers = {};
        if ($routeParams.uscode) {
            $scope.storeCode = $routeParams.uscode;
        }
        $scope.$watch("storeCode", function (val) {
            if (!!val && val.length == 7 && !isNaN(val)) {
                closureCreateHandler.beginCreate().$promise.then(function (data) {
                    //获取服务器的时间
                    var currentDate = new Date();
                    $scope.entity.USCode = USCode = val;
                    //$scope.entity.ActualCloseDate = currentDate;

                    currentScope.CreateDate = currentDate;
                });
                $scope.now = new Date();

                //获取字典项
                queryDictionaryByParentCode(closureCreateHandler, "ClosureType", $scope);
                queryDictionaryByParentCode(closureCreateHandler, "RiskStatus", $scope);
                queryDictionaryByParentCode(closureCreateHandler, "ClosureReasons", $scope);
                queryDictionaryByParentCode(closureCreateHandler, "Relocations", $scope);

                var getAssetRepUrl = Utils.ServiceURI.Address() + "api/StoreUsers/GetStoreEmployeesByMultiRoles/" + val

                var url = Utils.ServiceURI.Address() + "api/StoreUsers/GetStoreEmployeesByMultiRoles/" + val + "?roleCodes=Asset Rep,Asset Mgr,Asset Actor,PM,Legal_Counsel,Cons_Mgr,Finance_Consultant&usercode=" + window.currentUser.Code;
                $http.get(url)
                    .success(function (response) {
                        $scope.PMList = [];
                        $scope.RepList = [];
                        $scope.ActorList = [];
                        $scope.FinanceList = [];
                        $scope.LegalList = [];
                        $scope.AssetMgrList = [];
                        $scope.CMList = [];
                        angular.forEach(response, function (positionData, i) {
                            switch (positionData.PositionENUS) {
                                case "PM":
                                    $scope.PMList.push(positionData);
                                    break;
                                case "Asset Rep":
                                    $scope.RepList.push(positionData);
                                    $scope.isRep = true;
                                    break;
                                case "Asset Actor":
                                    $scope.ActorList.push(positionData);
                                    $scope.isRep = true;
                                    break;
                                case "Finance Consultant":
                                    $scope.FinanceList.push(positionData);
                                    break;
                                case "Legal Counsel":
                                    $scope.LegalList.push(positionData);
                                    break;
                                case "Market Asset Mgr":
                                    $scope.AssetMgrList.push(positionData);
                                    break;
                                case "Cons Mgr":
                                    $scope.CMList.push(positionData);
                                    break;
                            }
                        });
                        if ($scope.RepList.length == 1)
                            $scope.selRep = $scope.RepList[0];
                        if ($scope.ActorList.length == 1)
                            $scope.selActor = $scope.ActorList[0];
                        if ($scope.FinanceList.length == 1)
                            $scope.selFinance = $scope.FinanceList[0];
                        if ($scope.PMList.length == 1)
                            $scope.selPM = $scope.PMList[0];
                        if ($scope.LegalList.length == 1)
                            $scope.selLegal = $scope.LegalList[0];
                        if ($scope.AssetMgrList.length == 1)
                            $scope.selAssetMgr = $scope.AssetMgrList[0];
                        if ($scope.CMList.length == 1)
                            $scope.selCM = $scope.CMList[0];
                        
                        $scope.dataLoaded = true;
                    });
            }
        });
        $http.get(Utils.ServiceURI.ApiDelegate, {

            params: {
                url: Utils.ServiceURI.FrameAddress() + "api/user/1/10",
                name: "Gloria Chiu"
            }
        }).success(function (storeData, status, headers, config) {

            $scope.userPosition = storeData[0];

        });


        $("#navClosureNavigation").hide();

        $("#closureReasonRemark").hide();
        $scope.Code = "1";

        //初始化日期控件



        $scope.open = function ($event) {

            $event.preventDefault();
            $event.stopPropagation();

            $scope.opened = true;
        };




        //var dicCondition = initCondition(1, 10, "ClosureType");
        //dictionaryHandler.query(dicCondition).$promise.then(function (data) {
        //    $scope.ClosureTypeDics = data;
        //});
        var that = this;
        $scope.vm = that;
        $("#divInfo").hide();
        // $("#divDate").hide();
        //var projectCode =  Utils.ProjectCodes.Closure();

        $scope.closureReasonsChange = function () {

            var result = false;
            if (!!$scope.selClosureReason) {
                var s = $scope.selClosureReason.Value;
                if (s == "Others") {
                    result = true;
                }
            }

            if (result) {
                $("#closureReasonRemark").show();
            } else {
                $("#closureReasonRemark").hide();
            }
        }


        //选择处理人
        $scope.beginNoticeUsers = function (frm, closureInfo) {
            if (!frm.$valid) {
                return;
            }
            var errors = [];
            if ($scope.selRep == undefined || $scope.selRep == null) {
                errors.push("[[[请选择资产代表！]]]");
            }

            if ($scope.selActor == undefined || $scope.selActor == null) {
                errors.push("[[[请选择资产发起人！]]]");
            }

            if ($scope.selFinance == undefined || $scope.selFinance == null) {
                errors.push("[[[请选择Finance！]]]");
            }
            if ($scope.selAssetMgr == undefined || $scope.selAssetMgr == null) {
                errors.push("[[[请选择Finance Manager！]]]");
            }
            if ($scope.selPM == undefined || $scope.selPM == null) {
                errors.push("[[[请选择PM！]]]");
            }
            if ($scope.selLegal == undefined || $scope.selLegal == null) {
                errors.push("[[[请选择Legal！]]]");
            } 
            if ($scope.selCM == undefined || $scope.selCM == null) {
                errors.push("[[[请选择Construction Manager！]]]");
            }

            if ($scope.selClosureType == undefined || $scope.selClosureType == null) {
                errors.push("[[[请选择关店类型!]]]");
            }
            else {
                if ($scope.selClosureType.Value == "Lease Expiry" && $scope.storeBasicInfo.StoreContractInfo.EndYear > new Date($scope.entity.ActualCloseDate).getFullYear())
                    errors.push("[[[未到Expire year，关店类型不能选择Lease Expiry!]]]");
            }
            if ($scope.selRiskStatus == undefined || $scope.selRiskStatus == null) {
                errors.push("[[[请选择风险等级!]]]");
            }

            if ($scope.selClosureReason == undefined || $scope.selClosureReason == null) {
                errors.push("[[[请选择关店原因!]]]");
            }
            else if ($scope.selClosureReason.NameZHCN == "Others" && ($scope.entity.ClosureReasonRemark == undefined || $scope.entity.ClosureReasonRemark == null)) {
                errors.push("[[[请填写关店原因!]]]");
            }

            if ($scope.selRelocation == undefined || $scope.selRelocation == null) {
                errors.push("[[[请选择迁址!]]]");
            }

            if (!$scope.entity.ActualCloseDate || $scope.entity.ActualCloseDate == "") {
                errors.push("[[[请选择关店日期!]]]");
            }
            else {
                var now = new Date();
                if (Number(moment($scope.entity.ActualCloseDate).format("YYYYMMDD")) < Number(moment(now).format("YYYYMMDD"))) {
                    errors.push("[[[关店日期不能早于今天!]]]");
                }
            }
            if (errors.length > 0) {
                messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
                return false;
            }


            $scope.entity.AssetRepAccount = $scope.selRep.Code;
            $scope.entity.AssetRepNameZHCN = $scope.selRep.NameZHCN;
            $scope.entity.AssetRepNameENUS = $scope.selRep.NameENUS;

            $scope.entity.AssetActorAccount = $scope.selActor.Code;
            $scope.entity.AssetActorNameZHCN = $scope.selActor.NameZHCN;
            $scope.entity.AssetActorNameENUS = $scope.selActor.NameENUS;

            $scope.entity.FinanceAccount = $scope.selFinance.Code;
            $scope.entity.FinanceNameZHCN = $scope.selFinance.NameZHCN;
            $scope.entity.FinanceNameENUS = $scope.selFinance.NameENUS;

            $scope.entity.AssetManagerAccount = $scope.selAssetMgr.Code;
            $scope.entity.AssetManagerNameZHCN = $scope.selAssetMgr.NameZHCN;
            $scope.entity.AssetManagerNameENUS = $scope.selAssetMgr.NameENUS;

            $scope.entity.PMAccount = $scope.selPM.Code;
            $scope.entity.PMNameZHCN = $scope.selPM.NameZHCN;
            $scope.entity.PMNameENUS = $scope.selPM.NameENUS;

            $scope.entity.CMAccount = $scope.selCM.Code;
            $scope.entity.CMNameZHCN = $scope.selCM.NameZHCN;
            $scope.entity.CMNameENUS = $scope.selCM.NameENUS;

            $scope.entity.LegalAccount = $scope.selLegal.Code;
            $scope.entity.LegalNameZHCN = $scope.selLegal.NameZHCN;
            $scope.entity.LegalNameENUS = $scope.selLegal.NameENUS;

            var noticeScope;

            //$scope.storeEntity = new Object();
            $modal.open({
                templateUrl: "/Template/NoticeUsers",
                backdrop: 'static',
                size: 'lg',
                controller: ["$scope", "$modalInstance", "storeEntity", function ($scope, $modalInstance, storeEntity) {
                    $scope.noticeUsersName = "";
                    $scope.necessaryNoticeUsersName = "";
                    $scope.entity = closureInfo;
                    noticeScope = $scope;
                    //$scope.isLoading = true;
                    $scope.CMList = currentScope.CMList;
                    $scope.AssetMgrList = currentScope.AssetMgrList;
                    //var url = Utils.ServiceURI.Address() + "api/NecessaryNotice/GetAvailableUserCodes/" + noticeScope.entity.USCode + "/Closure";
                    //$http.get(url).success(function (response) {
                    //    if (response && response != null) {
                    //        $scope.necessaryNoticeUserCode = response.UserCodes;
                    //        $scope.necessaryNoticeRoleNames = response.RoleNames;
                    //        $scope.necessaryNoticeRoleUser = response.RoleUser;
                    //    }
                    //    $scope.isLoading = false;
                    //});
                    $scope.selectEmployee = function (users, role) {

                        users = $scope.selNoticeUsers;
                        var _this = this;

                        $selectUser.open({
                            checkUsers: function (selectedUsers) {
                                return true;
                            },
                            selectedUsers: angular.copy(users),
                            OnUserSelected: function (selectedUsers) {
                                $scope.entity.NoticeUsers = "";
                                $scope.noticeUsersName = "";
                                $scope.selNoticeUsers = selectedUsers;
                                $scope.entity.NoticeUserList = selectedUsers;
                                for (var i = 0; i < selectedUsers.length; i++) {
                                    if (i != selectedUsers.length - 1) {
                                        $scope.entity.NoticeUsersaAccount = $scope.entity.NoticeUsers + selectedUsers[i].Code + ",";
                                        $scope.noticeUsersName = $scope.noticeUsersName + selectedUsers[i].NameENUS + ",";
                                    } else {
                                        $scope.entity.NoticeUsers = $scope.entity.NoticeUsers + selectedUsers[i].Code;
                                        $scope.noticeUsersName = $scope.noticeUsersName + selectedUsers[i].NameENUS;

                                    }
                                }

                            }
                        });
                    };
                    //$scope.selectNecessaryNoticeEmployee = function (users, role) {

                    //    users = $scope.selNecessaryNoticeUsers;
                    //    var _this = this;

                    //    $selectUser.open({
                    //        checkUsers: function (selectedUsers) {
                    //            return true;
                    //        },
                    //        scopeUserCodes: $scope.necessaryNoticeUserCode,
                    //        selectedUsers: angular.copy(users),
                    //        OnUserSelected: function (selectedUsers) {
                    //            $scope.entity.NecessaryNoticeUsers = "";
                    //            $scope.necessaryNoticeUsersName = "";
                    //            $scope.selNecessaryNoticeUsers = selectedUsers;
                    //            $scope.entity.NecessaryNoticeUserList = selectedUsers;
                    //            for (var i = 0; i < selectedUsers.length; i++) {
                    //                if (i != selectedUsers.length - 1) {
                    //                    $scope.entity.NoticeUsersaAccount = $scope.entity.NecessaryNoticeUsers + selectedUsers[i].Code + ",";
                    //                    $scope.necessaryNoticeUsersName = $scope.necessaryNoticeUsersName + selectedUsers[i].NameENUS + ",";
                    //                } else {
                    //                    $scope.entity.NecessaryNoticeUsers = $scope.entity.NecessaryNoticeUsers + selectedUsers[i].Code;
                    //                    $scope.necessaryNoticeUsersName = $scope.necessaryNoticeUsersName + selectedUsers[i].NameENUS;

                    //                }
                    //            }

                    //        }
                    //    });
                    //};


                    storeEntity.selAssetMgr = $scope.selAssetMgr;
                    $scope.storeEntity = storeEntity;
                    $scope.ok = function () {
                        var errors = [];
                        //if ($scope.entity.NecessaryNoticeUserList == undefined || $scope.entity.NecessaryNoticeUserList == null || $scope.entity.NecessaryNoticeUserList.length == 0) {
                        //    errors.push("请选择必要抄送人!");
                        //}
                        //else {
                        //    var selectedRoleCode = [];
                        //    angular.forEach($scope.entity.NecessaryNoticeUserList, function (r, i) {
                        //        angular.forEach($scope.necessaryNoticeRoleUser, function (s, j) {
                        //            if (r.Code == s.UserCode) {
                        //                selectedRoleCode.push(s.RoleName);
                        //            }
                        //        });
                        //    });
                        //    angular.forEach($scope.necessaryNoticeRoleNames.split(','), function (r, i) {
                        //        var result = false;
                        //        angular.forEach(selectedRoleCode, function (s, j) {
                        //            if (s == r)
                        //                result = true;
                        //        });
                        //        if (!result)
                        //            errors.push("请选择必要抄送人" + r + "!");
                        //    });
                        //}

                        if (errors.length > 0) {
                            messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
                        }
                        else {
                            $modalInstance.close($scope.storeEntity);
                        }

                    };
                    $scope.cancel = function () {
                        $modalInstance.dismiss("cancel");
                    };
                    $scope.isUnchanged = function (_storeEntity) {
                        return angular.equals(closureInfo, _storeEntity);
                    };
                }],
                resolve: {
                    storeEntity: function () {
                        return angular.copy(closureInfo);
                    }
                }
            }).result.then(function (storeEntity) {



                //获取选择的字典数据

                $scope.entity.CreateUserAccount = window.currentUser.Code;
                $scope.entity.CreateUserNameZHCN = window.currentUser.NameZHCN;
                $scope.entity.CreateUserNameENUS = window.currentUser.NameENUS;

                $scope.entity.RelocationNameENUS = $scope.selRelocation.NameENUS;
                $scope.entity.RelocationNameZHCN = $scope.selRelocation.NameZHCN;
                $scope.entity.RelocationCode = $scope.selRelocation.Code;

                $scope.entity.ClosureReasonNameENUS = $scope.selClosureReason.NameENUS;
                $scope.entity.ClosureReasonNameZHCN = $scope.selClosureReason.NameZHCN;
                $scope.entity.ClosureReasonCode = $scope.selClosureReason.Code;

                $scope.entity.RiskStatusNameZHCN = $scope.selRiskStatus.NameZHCN;
                $scope.entity.RiskStatusNameENUS = $scope.selRiskStatus.NameENUS;
                $scope.entity.RiskStatusCode = $scope.selRiskStatus.Code;

                $scope.entity.ClosureTypeNameENUS = $scope.selClosureType.NameENUS;
                $scope.entity.ClosureTypeNameZHCN = $scope.selClosureType.NameZHCN;
                $scope.entity.ClosureTypeCode = $scope.selClosureType.Code;

                //$scope.entity.NoticeUsers = storeEntity.NoticeUsers;

                $scope.entity.USCode = $scope.storeBasicInfo.StoreBasicInfo.StoreCode;
                $scope.entity.StoreNameZHCN = $scope.storeBasicInfo.StoreBasicInfo.NameZHCN;
                $scope.entity.StoreNameENUS = $scope.storeBasicInfo.StoreBasicInfo.NameENUS;

                //$scope.entity.AssetManagerAccount = $scope.entity.selAssetMgr.Code;
                //$scope.entity.AssetManagerNameENUS = $scope.entity.selAssetMgr.NameENUS;
                //$scope.entity.AssetManagerNameZHCN = $scope.entity.selAssetMgr.NameZHCN;

                //$scope.entity.CMAccount = $scope.entity.selCM.Code;
                //$scope.entity.CMNameENUS = $scope.entity.selCM.NameENUS;
                //$scope.entity.CMNameZHCN = $scope.entity.selCM.NameZHCN;

                $scope.entity.ActualCloseDate = $("#closeDate").val();

                closureCreateHandler.saveClosureInfo($scope.entity).$promise.then(function (data) {
                    var obj = data;
                    messager.showMessage("[[[提交成功]]]", "fa-check c_green").then(function () {
                        $window.location = Utils.ServiceURI.WebAddress() + "redirect";
                    });
                });

            }, function () {
            });


        }

        var currentScope = $scope;


        $scope.selectEmployee = function (users, role) {

            var _this = this;
            var roleStr = "";
            if (role == "Asset Actor") {
                roleStr = "Asset Rep";
            }
            $selectUser.open({

                storeCode: $scope.entity.USCode,
                positionCode: role,



                checkUsers: function (selectedUsers) {
                    var errors = [];
                    if (selectedUsers.length > 1) {
                        errors.push("[[[只能选择一个人]]]");
                        return false;
                    }
                    if ($.trim(selectedUsers[0].Code) == "") {
                        errors.push("[[[编号不能为空！]]]");
                    }

                    if (errors.length > 0) {
                        messager.showMessage(errors.join("<br />"), "fa-warning c_orange");
                        return false;
                    }
                    return true;
                },
                selectedUsers: angular.copy(users),
                OnUserSelected: function (selectedUsers) {
                    var selUser = selectedUsers[0];
                    switch (role) {
                        case "Asset Rep":
                            $scope.entity.AssetRepAccount = selUser.Code;
                            $scope.entity.AssetRepName = selUser.NameENUS;
                            break;
                        case "Asset Actor":
                            $scope.entity.AssetActorAccount = selUser.Code;
                            $scope.entity.AssetActorName = selUser.NameENUS;
                            break;

                        case "Finance Controller":
                            $scope.entity.FinanceAccount = selUser.Code;
                            $scope.entity.FinanceName = selUser.NameENUS;
                            break;
                        case "PM":
                            $scope.entity.PMAccount = selUser.Code;
                            $scope.entity.PMName = selUser.NameENUS;
                            break;
                        case "Legal Counsel":
                            $scope.entity.LegalAccount = selUser.Code;
                            $scope.entity.LegalName = selUser.NameENUS;

                            break;
                    }
                }
            });
        };
    }]);