/// <reference path="../Libs/moment/moment.d.ts" />
/// <reference path="../Libs/Angular/Plugins/angular-ui-bootstrap.d.ts" />
/// <reference path="../Libs/Angular/Plugins/angular-cookies.d.ts" />
/// <reference path="../Utils/Utils.ts" />
/// <reference path="../Libs/Angular/angular.d.ts" />
angular.module("mcd.am.modules", [
    "ngCookies",
    "ngPlupload",
    'ui.bootstrap',
    'mcd.am.services',
    "mcd.am.services.store",
    "mcd.am.services.contract",
    "mcd.am.services.contractRevision",
    "mcd.am.services.projectUsers",
    "mcd.am.services.project",
    "mcd.am.service.taskwork",
    "mcd.am.service.attachment",
    "mcd.am.services.flow",
    "mcd.am.services.tempClosure",
    "mcd.am.services.reinvestmentInfo",
    "mcd.am.services.StoreProfitabilityAndLeaseInfo",
    "mcd.am.services.summaryReinvestmentCost",
    "mcd.am.services.financialPreAnalysis",
    "mcd.am.closure.services.closureTool",
    "mcd.am.services.renewal",
    "dictionaryServices"
]).directive("taskReminder", [
    "$http", function ($http) {
        return {
            restrict: "EA",
            replace: true,
            template: $("#tplTaskReminder").html(),
            link: function ($scope, element, attrs) {
                $http.get(Utils.ServiceURI.Address() + "api/system/remindercount", {
                    cache: false
                }).success(function (result) {
                    $scope.TaskCount = result.TaskCount;
                    $scope.ReminderCount = result.ReminderCount;
                    $scope.NoticeCount = result.NoticeCount;
                    $scope.PMT_ApproveCount = result.PMT_ApproveCount;
                    $scope.PMT_DealCount = result.PMT_DealCount;
                    $scope.PMT_NotifyCount = result.PMT_NotifyCount;
                    $scope.PMT_RemindCount = result.PMT_RemindCount;
                    $scope.PMT_Total = result.PMT_Total;
                    $scope.PMT_URL = result.PMT_URL;
                });
            }
        };
    }]).directive("chekingPoint", [
    "$http", function ($http) {
        return {
            restrict: "EA",
            scope: {
                projectId: "=",
                flowCode: "@",
                refresh: "="
            },
            replace: true,
            templateUrl: Utils.ServiceURI.AppUri + "Module/CheckingPoint",
            link: function ($scope, ele, attrs) {
                $scope.$watch("refresh", function (val) {
                    if (!!val && val === true) {
                        var url = Utils.ServiceURI.Address() + "api/NodeInfo/QueryCheckPoints/" + $scope.projectId + "/" + $scope.flowCode;
                        $http.get(url).success(function (data) {
                            angular.forEach(data.Nodes, function (checkPoint, i) {
                                if (checkPoint.Sequence <= checkPoint.CurrentNodeSequence) {
                                    checkPoint.isFinished = true;
                                } else if (checkPoint.Sequence == checkPoint.CurrentNodeSequence + 1) {
                                    checkPoint.isCurrent = true;
                                }
                            });
                            var OperatorHTML = "";
                            if (!!data.Operators && data.Operators.length > 0) {
                                OperatorHTML += "<ul class='node-operators'>";
                                angular.forEach(data.Operators, function (o, i) {
                                    OperatorHTML += "<li>【" + o.Code + "】" + o.OperateMsgDisp + "</li>";
                                });
                                OperatorHTML += "</ul>";
                            } else if (data.IsFinished) {
                                OperatorHTML = "<p class='node-operators-none'>[[[已完成]]]</p>";
                            } else {
                                OperatorHTML = "<p class='node-operators-none'>[[[暂无处理人]]]</p>";
                            }
                            $scope.progress = data.Progress;
                            $scope.project = data.Info;
                            $scope.checkPointList = data.Nodes;
                            $scope.operatorHTML = OperatorHTML;
                            $scope.refresh = false;
                        });
                    }
                });
            }
        };
    }]).directive("step1", [
    "$http",
    "$modal",
    "messager",
    "storeService",
    function ($http, $modal, messager, storeService) {
        return {
            restrict: "EA",
            replace: true,
            scope: {
                code: "=",
                storeValid: "=?",
                flowCode: "@"
            },
            templateUrl: Utils.ServiceURI.AppUri + "Module/Step1",
            link: function ($scope, element, attrs) {
                $scope.condition = {
                    name: "",
                    code: ""
                };
                $scope.isValidStore = function () {
                    return angular.isObject($scope.searchedStore) && !angular.isString($scope.searchedStore);
                };
                $scope.hoverStoreName = function (hover) {
                    if (!$scope.isValidStore()) {
                        $scope.searchedStore = null;
                    }
                    $scope.isHoverStoreName = hover;
                };
                $scope.hoverCode = function (hover) {
                    if (!$scope.isValidStore()) {
                        $scope.searchedStore = null;
                    }
                    $scope.isHoverUsCode = hover;
                };
                $scope.searchStore = function (inputCode) {
                    $scope.condition.name = "";
                    $scope.condition.code = inputCode;
                    return storeService.searchMyStore({
                        code: inputCode,
                        pageSize: 5,
                        userCode: window["currentUser"].Code,
                        flowCode: $scope.flowCode
                    }).$promise.then(function (stores) {
                        stores = stores || [];
                        stores.sort(function (s1, s2) {
                            return s1.StoreCode > s2.StoreCode ? 1 : -1;
                        });
                        return stores;
                    });
                };
                $scope.searchStoreByName = function (name) {
                    $scope.condition.name = name;
                    $scope.condition.code = "";
                    return storeService.searchMyStore({
                        name: name,
                        pageSize: 5,
                        userCode: window["currentUser"].Code,
                        flowCode: $scope.flowCode
                    }).$promise.then(function (stores) {
                        stores = stores || [];
                        stores.sort(function (s1, s2) {
                            return Number(s1.NameZHCN) > Number(s2.NameZHCN) ? 1 : -1;
                        });
                        return stores;
                    });
                };
                $scope.$watch("searchedStore", function (s) {
                    if (!!s) {
                        $scope.code = s.StoreCode;
                    }
                });
                $scope.$watch("code", function (val) {
                    if (!!val && val.length == 7 && !isNaN(val)) {
                        //storeService.checkStore({
                        //    code: val
                        //}).$promise.then((result) => {
                        //        $scope.storeValid = result.StoreValid;
                        //        if (!result.StoreValid) {
                        //            messager.showMessage("没有餐厅'" + val + "'的操作权限", "fa-warning c_red");
                        //        }
                        //});
                        storeService.checkStoreFlow({
                            code: val,
                            flowCode: $scope.flowCode
                        }).$promise.then(function (result) {
                            $scope.storeValid = result.StoreValid;
                            if (!result.StoreValid) {
                                messager.showMessage("'" + result.Store.NameZHCN + "'当前已有正在进行中的'" + $scope.flowCode + "'流程，请重新选择。", "fa-warning c_red");
                            }
                        });
                        if (!$scope.searchedStore) {
                            storeService.getStoreBasic({
                                usCode: val
                            }).$promise.then(function (result) {
                                $scope.searchedStore = result;
                            });
                        }
                    } else {
                        $scope.storeValid = false;
                    }
                });
                $scope.selectStore = function () {
                    $modal.open({
                        size: "lg",
                        templateUrl: Utils.ServiceURI.AppUri + "Template/SelectStore",
                        backdrop: 'static',
                        resolve: {
                            condition: function () {
                                return angular.copy($scope.condition);
                            }
                        },
                        controller: [
                            "$scope", "$modalInstance", "condition", function ($modalScope, $modalInstance, condition) {
                                $modalScope.condition = condition;
                                $modalScope.pagging = function () {
                                    $.ajax(Utils.ServiceURI.ApiDelegate, {
                                        cache: false,
                                        data: {
                                            url: Utils.ServiceURI.Address() + "api/store/" + $modalScope.pageIndex + "/" + $modalScope.pageSize + "/" + window["currentUser"].Code,
                                            code: $modalScope.condition.code,
                                            name: $modalScope.condition.name
                                        },
                                        success: function (data) {
                                            $modalScope.list = data.List;
                                            $modalScope.totalItems = data.TotalItems;
                                            $modalScope.$apply();
                                        }
                                    });
                                };
                                $modalScope.search = function () {
                                    $modalScope.pageIndex = 1;
                                    $modalScope.pagging();
                                };
                                $modalScope.pageIndex = 1;
                                $modalScope.pageSize = 10;
                                $modalScope.totalItems = 0;
                                $modalScope.list = [];
                                $modalScope.$watch("pageIndex", function (page) {
                                    $modalScope.pagging();
                                });
                                $modalScope.onPageChange = function (page) {
                                    $modalScope.pageIndex = page;
                                };

                                $modalScope.cancel = function () {
                                    $modalInstance.dismiss("cancel");
                                };

                                $modalScope.ok = function (store) {
                                    $modalInstance.close(store);
                                };

                                $modalScope.search();
                            }]
                    }).result.then(function (store) {
                        $scope.searchedStore = angular.copy(store);
                        $scope.code = angular.copy(store.Code);
                    });
                };
            }
        };
    }]).directive("storeBasicInfo", [
    "$http",
    "projectUsersService",
    "$routeParams",
    function ($http, projectUsersService, $routeParams) {
        return {
            restrict: "EA",
            scope: {
                code: "=",
                store: '=?',
                project: "=?",
                workflowType: '@'
            },
            replace: true,
            template: "<div ng-include='templatePath'></div>",
            //templateUrl: Utils.ServiceURI.AppUri + "Module/StoreBasicInfo",
            link: function ($scope, ele, attrs) {
                if (!$scope.workflowType) {
                    $scope.workflowType = "";
                }
                var workflowTypeParts = $scope.workflowType.split("_");
                var flowName = workflowTypeParts[0];
                var subFlowName = workflowTypeParts.length > 1 ? workflowTypeParts[1] : "";
                $scope.templatePath = Utils.ServiceURI.AppUri + flowName + "Module/" + subFlowName + "StoreBasicInfo";
                $scope.isShowCurrentLeaseEndYear = true;
                $scope.$watch("code", function (val) {
                    if (!!val && val.length == 7 && !isNaN(val)) {
                        $http.get(Utils.ServiceURI.Address() + "api/Store/Details/" + val).success(function (data) {
                            $scope.store = data;
                            var closeDate = moment($scope.store.StoreBasicInfo.CloseDate);
                            if (closeDate.year() == 1900) {
                                $scope.store.StoreBasicInfo.CloseDate = null;
                            }

                            var backUrl = bindUrl(document.URL, val);
                            $scope.store.linkUrl = "/StoreList/?uscode=" + val + "&backurl=" + escape(backUrl);
                        });
                    } else {
                        $scope.store = null;
                    }
                });

                //处理Url
                var bindUrl = function (url, uscode) {
                    if (url.indexOf('user-id') > 0) {
                        var userIdStr = url.substr(url.indexOf('user-id'));
                        if (userIdStr.indexOf('&') > 0) {
                            userIdStr = userIdStr.substring(0, userIdStr.indexOf('&'));
                            url = url.replace(userIdStr + '&', "");
                        } else
                            url = url.replace(userIdStr, "");
                        if (url.indexOf('?') == url.length - 1)
                            url = url.substr(0, url.indexOf('?'));
                    }
                    if (url.indexOf('&uscode=') < 0 && url.indexOf('?uscode=') < 0) {
                        if (url.indexOf('?') > 0)
                            return url + '&uscode=' + uscode;
                        else
                            return url + '?uscode=' + uscode;
                    } else {
                        return url.replace("uscode=" + $routeParams.uscode, "uscode=" + uscode);
                    }
                };
            }
        };
    }
]).directive("projectTeam", [
    "messager",
    "projectUsersService",
    "projectService",
    function (messager, projectUsersService, projectService) {
        return {
            restrict: "EA",
            scope: {
                projectId: "@",
                flowCode: "@",
                usCode: "=",
                views: "=",
                team: "=?",
                editable: "=",
                showInnerEdit: "=?"
            },
            templateUrl: Utils.ServiceURI.AppUri + "Module/ProjectTeam",
            replace: true,
            link: function ($scope, element, attrs) {
                $scope.team = {};
                $scope.$watch("usCode", function (val) {
                    if (!!val && val.length == 7 && !isNaN(val)) {
                        if ($scope.projectId) {
                            projectUsersService.canInnerEdit($scope.projectId).then(function (response) {
                                $scope.canInnerEdit = response.data;
                            });
                        }
                        projectUsersService.queryTeamUser($scope.projectId, $scope.usCode).then(function (response) {
                            $scope.assetReps = response.data.AssetReps;
                            $scope.assetActors = response.data.AssetActors;
                            $scope.finances = response.data.Finances;
                            $scope.pms = response.data.PMs;
                            $scope.legals = response.data.Legals;
                            $scope.assetMgrs = response.data.AssetMgrs;
                            $scope.cms = response.data.CMs;
                            if (response.data.AssetReps.length == 1) {
                                $scope.team.AssetRep = response.data.AssetReps[0];
                            } else {
                                angular.forEach(response.data.AssetReps, function (rep, i) {
                                    if (rep.IsSelected) {
                                        $scope.team.AssetRep = rep;
                                    }
                                    ;
                                });
                            }

                            if (response.data.AssetActors.length == 1) {
                                $scope.team.AssetActor = response.data.AssetActors[0];
                            } else {
                                angular.forEach(response.data.AssetActors, function (actor, i) {
                                    if (actor.IsSelected) {
                                        $scope.team.AssetActor = actor;
                                    }
                                    ;
                                });
                            }
                            if (response.data.Finances.length == 1) {
                                $scope.team.Finance = response.data.Finances[0];
                            } else {
                                angular.forEach(response.data.Finances, function (finance, i) {
                                    if (finance.IsSelected) {
                                        $scope.team.Finance = finance;
                                    }
                                    ;
                                });
                            }

                            if (response.data.PMs.length == 1) {
                                $scope.team.PM = response.data.PMs[0];
                            } else {
                                angular.forEach(response.data.PMs, function (pm, i) {
                                    if (pm.IsSelected) {
                                        $scope.team.PM = pm;
                                    }
                                    ;
                                });
                            }
                            if (response.data.CMs.length == 1) {
                                $scope.team.CM = response.data.CMs[0];
                            } else {
                                angular.forEach(response.data.CMs, function (cm, i) {
                                    if (cm.IsSelected) {
                                        $scope.team.CM = cm;
                                    }
                                    ;
                                });
                            }

                            if (response.data.Legals.length == 1) {
                                $scope.team.Legal = response.data.Legals[0];
                            } else {
                                angular.forEach(response.data.Legals, function (legal, i) {
                                    if (legal.IsSelected) {
                                        $scope.team.Legal = legal;
                                    }
                                    ;
                                });
                            }

                            if (response.data.AssetMgrs.length == 1) {
                                $scope.team.AssetMgr = response.data.AssetMgrs[0];
                            } else {
                                angular.forEach(response.data.AssetMgrs, function (assetMgrs, i) {
                                    if (assetMgrs.IsSelected) {
                                        $scope.team.AssetMgr = assetMgrs;
                                    }
                                    ;
                                });
                            }
                            $scope.dataLoaded = true;
                        });
                    }
                    ;
                });
                $scope.edit = function () {
                    $scope.innerEditing = true;
                };
                $scope.save = function () {
                    projectService.changeProjectTeam({
                        ProjectId: $scope.projectId,
                        FlowCode: $scope.flowCode,
                        ProjectTeamMembers: {
                            AssetRep: $scope.team.AssetRep,
                            AssetActor: $scope.team.AssetActor,
                            AssetMgr: $scope.team.AssetMgr,
                            Finance: $scope.team.Finance,
                            PM: $scope.team.PM,
                            CM: $scope.team.CM,
                            Legal: $scope.team.Legal
                        }
                    }).$promise.then(function () {
                        messager.showMessage("[[[保存成功]]]", "fa_check c_green");
                        $scope.innerEditing = false;
                    }, function () {
                        messager.showMessage("[[[保存失败]]]", "fa-warning c_red");
                    });
                };
            }
        };
    }]).directive('notifyUsersModal', [
    '$modal', 'projectUsersService', "messager",
    function ($modal, projectUsersService, messager) {
        return {
            restrict: "EA",
            scope: {
                editable: '=',
                usCode: "=",
                team: "=?",
                projectId: "@",
                notifyUsersInfo: '=?',
                visible: '=',
                onSubmit: '&',
                flowCode: "@"
            },
            replace: true,
            link: function ($scope, element, attrs) {
                $scope.notifyUsersInfo = $.extend($scope.notifyUsersInfo, {
                    AssetMgr: null,
                    CM: null,
                    NoticeUsers: []
                });
                $scope.$watch('visible', function (val) {
                    if (val) {
                        $scope.visible = false;
                        $modal.open({
                            templateUrl: "/Module/NoticeUsers",
                            backdrop: 'static',
                            size: 'lg',
                            resolve: {
                                team: function () {
                                    return angular.copy($scope.team);
                                },
                                notifyUsersInfo: function () {
                                    return angular.copy($scope.notifyUsersInfo);
                                }
                            },
                            controller: [
                                "$scope", "$modalInstance", "$selectUser", "team", "notifyUsersInfo", function (noticeScope, $modalInstance, $selectUser, team, notifyUsersInfo) {
                                    noticeScope.notifyUsersInfo = notifyUsersInfo;
                                    noticeScope.team = team;

                                    //noticeScope.dataLoaded = false;
                                    //projectUsersService.queryNoticeUser($scope.projectId, $scope.usCode).then((response) => {
                                    //    noticeScope.AssetMgrs = response.data.AssetMgrs;
                                    //    noticeScope.CMs = response.data.CMs;
                                    //    angular.forEach(response.data.AssetMgrs, (u, i) => {
                                    //        if (u.IsSelected) {
                                    //            noticeScope.notifyUsersInfo.AssetMgr = u;
                                    //        }
                                    //    });
                                    //    angular.forEach(response.data.CMs, (u, i) => {
                                    //        if (u.IsSelected) {
                                    //            noticeScope.notifyUsersInfo.CM = u;
                                    //        }
                                    //    });
                                    //    noticeScope.dataLoaded = true;
                                    //});
                                    //noticeScope.loading = true;
                                    //projectUsersService.getNecessaryNotifyUsers($scope.usCode, $scope.flowCode).then((response) => {
                                    //    if (response.data && response.data != null) {
                                    //        noticeScope.necessaryNoticeUserCode = response.data.UserCodes;
                                    //        noticeScope.necessaryNoticeRoleNames = response.data.RoleNames;
                                    //        noticeScope.necessaryNoticeRoleUser = response.data.RoleUser;
                                    //    }
                                    //    noticeScope.loading = false;
                                    //});
                                    noticeScope.selectEmployee = function () {
                                        var users = noticeScope.notifyUsersInfo.NoticeUsers;
                                        if (users) {
                                            users = $.map(noticeScope.notifyUsersInfo.NoticeUsers, function (u, i) {
                                                return {
                                                    Code: u.UserAccount,
                                                    NameZHCN: u.UserNameZHCN,
                                                    NameENUS: u.UsernameENUS
                                                };
                                            });
                                        }

                                        $selectUser.open({
                                            checkUsers: function (selectedUsers) {
                                                return true;
                                            },
                                            selectedUsers: angular.copy(users),
                                            OnUserSelected: function (selectedUsers) {
                                                noticeScope.noticeUsersName = $.map(selectedUsers, function (u, i) {
                                                    return u.NameENUS;
                                                }).join(",");
                                                noticeScope.notifyUsersInfo.NoticeUsers = $.map(selectedUsers, function (u, i) {
                                                    return {
                                                        UserAccount: u.Code,
                                                        UserNameZHCN: u.NameZHCN,
                                                        UsernameENUS: u.NameENUS
                                                    };
                                                });
                                            }
                                        });
                                    };

                                    //noticeScope.selectNecessary = () => {
                                    //    var users = noticeScope.notifyUsersInfo.NecessaryNoticeUsers;
                                    //    if (users) {
                                    //        users = $.map(noticeScope.notifyUsersInfo.NecessaryNoticeUsers, (u, i) => {
                                    //            return {
                                    //                Code: u.UserAccount,
                                    //                NameZHCN: u.UserNameZHCN,
                                    //                NameENUS: u.UsernameENUS
                                    //            };
                                    //        });
                                    //    }
                                    //    $selectUser.open({
                                    //        checkUsers: (selectedUsers) => {
                                    //            return true;
                                    //        },
                                    //        selectedUsers: angular.copy(users),
                                    //        scopeUserCodes: noticeScope.necessaryNoticeUserCode,
                                    //        OnUserSelected: function (selectedUsers) {
                                    //            noticeScope.necessaryNoticeUsersName = $.map(selectedUsers, (u, i) => {
                                    //                return u.NameENUS;
                                    //            }).join(",");
                                    //            noticeScope.notifyUsersInfo.NecessaryNoticeUsers = $.map(selectedUsers, (u, i) => {
                                    //                return {
                                    //                    UserAccount: u.Code,
                                    //                    UserNameZHCN: u.NameZHCN,
                                    //                    UsernameENUS: u.NameENUS
                                    //                };
                                    //            });
                                    //        }
                                    //    });
                                    //};
                                    noticeScope.ok = function () {
                                        var errors = [];

                                        //if (!noticeScope.notifyUsersInfo.AssetMgr) {
                                        //    errors.push("请选择Assert Mgr!");
                                        //} else if (!noticeScope.notifyUsersInfo.CM) {
                                        //    errors.push("请选择CM!");
                                        //}
                                        //if (noticeScope.notifyUsersInfo.NecessaryNoticeUsers == undefined || noticeScope.notifyUsersInfo.NecessaryNoticeUsers == null || noticeScope.notifyUsersInfo.NecessaryNoticeUsers.length == 0) {
                                        //    errors.push("请选择必要抄送人!");
                                        //}
                                        //else {
                                        //    var selectedRoleCode = [];
                                        //    angular.forEach(noticeScope.notifyUsersInfo.NecessaryNoticeUsers, (r, i) => {
                                        //        angular.forEach(noticeScope.necessaryNoticeRoleUser, (s, j) => {
                                        //            if (r.UserAccount == s.UserCode) {
                                        //                selectedRoleCode.push(s.RoleName);
                                        //            }
                                        //        });
                                        //    });
                                        //    angular.forEach(noticeScope.necessaryNoticeRoleNames.split(','), (r, i) => {
                                        //        var result = false;
                                        //        angular.forEach(selectedRoleCode, (s, j) => {
                                        //            if (s == r)
                                        //                result = true;
                                        //        });
                                        //        if (!result)
                                        //            errors.push("请选择必要抄送人" + r + "!");
                                        //    });
                                        //}
                                        if (errors.length > 0) {
                                            messager.showMessage(errors.join("<br />"), "fa-warning c_red");
                                        } else {
                                            $modalInstance.close(notifyUsersInfo);
                                            return true;
                                        }
                                    };
                                    noticeScope.cancel = function () {
                                        $modalInstance.dismiss("cancel");
                                    };
                                }
                            ]
                        }).result.then(function (notifyUsersInfo) {
                            $scope.notifyUsersInfo = angular.copy(notifyUsersInfo);
                            $scope.onSubmit({ notifyUsersInfo: notifyUsersInfo });
                        });
                    }
                });
            }
        };
    }]).directive("contractInfo", [
    "contractService",
    "contractRevisionService",
    "$http",
    function (contractService, contractRevisionService, $http) {
        return {
            restrict: "EA",
            scope: {
                editable: "=",
                projectId: "@",
                flowCode: "@",
                contract: "=?",
                revisions: "=?",
                transAttList: "=?"
            },
            replace: true,
            templateUrl: Utils.ServiceURI.AppUri + "Module/ContractInfo",
            link: function ($scope, element, attrs) {
                $scope.RentTypes = [
                    'Fixed Rent', 'Fixed Rent Plus Rent Percent', 'Rent Percent',
                    'Rent Percent with Sales Hurdles', 'The Higher of Base Rent or Rent Percent', 'Others'];
                var loadRevisions = function (contractId) {
                    contractRevisionService.query($scope.projectId, contractId).then(function (response) {
                        $scope.revisions = response.data || [];
                    });
                };
                $scope.$on("onAttachsLoaded", function (e, list) {
                    $scope.transAttList = list;
                });
                var parseYear = function (dateStr, prop) {
                    if (!!dateStr) {
                        var year = moment(dateStr).format("YYYY");
                        if (year !== "Invalid date") {
                            $scope.contract[prop] = year;
                        }
                    }
                };
                $scope.EditModes = [
                    {
                        Name: "编辑",
                        Value: "EDIT"
                    }, {
                        Name: "新增",
                        Value: "ADD"
                    }];
                $scope.enableSelectEditMode = true;
                $scope.enableEditRevision = false;
                $scope.enableEditSpecial = false;
                $scope.showRevisionInfo = false;
                $scope.RevisionInfo = {};
                $scope.showChangeDate = {};
                $scope.title = "";
                contractService.query($scope.projectId).then(function (response) {
                    $scope.contracts = response.data.Histories;
                    $scope.currentContract = response.data.Current;
                    $scope.contract = response.data.Current;
                    $scope.contract.contractEditable = true;
                    switch ($scope.flowCode) {
                        case "Closure_ContractInfo":
                            response.data.Current.EditMode = "EDIT";
                            $scope.enableSelectEditMode = false;
                            $scope.enableEditRevision = true;
                            $scope.enableEditSpecial = false;
                            $scope.showRevisionInfo = false;
                            loadRevisions($scope.contract.ContractInfoId);
                            break;
                        case "MajorLease_ContractInfo":
                            $scope.enableSelectEditMode = true;
                            $scope.enableEditRevision = false;
                            $scope.enableEditSpecial = false;
                            $scope.showRevisionInfo = true;
                            $http.get(Utils.ServiceURI.Address() + "api/MajorLease/GetMajorInfo?projectId=" + $scope.projectId, {
                                cache: false
                            }).success(function (response) {
                                $scope.RevisionInfo = response;
                                $scope.title = "Major Lease Change";
                                if ($scope.RevisionInfo.ChangeLandlordType) {
                                    $scope.contract.PartyAFullName = $scope.RevisionInfo.NewLandlord;
                                }
                                if ($scope.RevisionInfo.ChangeRedLineType) {
                                    $scope.contract.TotalLeasedArea = $scope.RevisionInfo.NewChangeRedLineRedLineArea;
                                }
                                if ($scope.RevisionInfo.ChangeLeaseTermType) {
                                    $scope.contract.EndDate = $scope.RevisionInfo.NewChangeLeaseTermExpiraryDate;
                                }
                                if ($scope.RevisionInfo.ChangeRentalType) {
                                    $scope.contract.RentStructure = $scope.RevisionInfo.NewRentalStructure;
                                }
                            });
                            loadRevisions($scope.contract.ContractInfoId);
                            break;
                        case "Rebuild_ContractInfo":
                            $scope.enableSelectEditMode = true;
                            $scope.enableEditRevision = false;
                            $scope.enableEditSpecial = false;
                            $scope.showRevisionInfo = true;
                            $http.get(Utils.ServiceURI.Address() + "api/Rebuild/RebuildPackage/GetPackageInfo?projectId=" + $scope.projectId, {
                                cache: false
                            }).success(function (response) {
                                $scope.RevisionInfo = response;
                                $scope.title = "Lease Change due to Rebuild";
                                if ($scope.RevisionInfo.ChangeLandlordType) {
                                    $scope.contract.PartyAFullName = $scope.RevisionInfo.NewLandlord;
                                }
                                if ($scope.RevisionInfo.ChangeRedLineType) {
                                    $scope.contract.TotalLeasedArea = $scope.RevisionInfo.NewChangeRedLineRedLineArea;
                                }
                                if ($scope.RevisionInfo.ChangeLeaseTermType) {
                                    $scope.contract.EndDate = $scope.RevisionInfo.NewChangeLeaseTermExpiraryDate;
                                }
                                if ($scope.RevisionInfo.ChangeRentalType) {
                                    $scope.contract.RentStructure = $scope.RevisionInfo.NewRentalStructure;
                                }
                            });
                            loadRevisions($scope.contract.ContractInfoId);
                            break;
                        case "Renewal_ContractInfo":
                            response.data.Current.EditMode = "ADD";
                            $scope.enableSelectEditMode = false;
                            $scope.enableEditRevision = false;
                            $scope.enableEditSpecial = true;
                            $scope.showRevisionInfo = false;
                            $http.get(Utils.ServiceURI.Address() + "api/renewalAnalysis/contractinfo?projectId=" + $scope.projectId, {
                                cache: false
                            }).success(function (response) {
                                $scope.contract.FreeRentalPeriod = response.FreeRentalPeriod;
                                $scope.contract.ExclusivityClause = response.ExclusivityClause;
                            });
                            break;
                        default:
                            loadRevisions($scope.contract.ContractInfoId);
                            break;
                    }
                });
                $scope.openDate = function ($event, dateTag) {
                    $event.preventDefault();
                    $event.stopPropagation();
                    $scope[dateTag] = true;
                };
                $scope.$watch("contract.StartDate", function (val) {
                    parseYear(val, "StartYear");
                });
                $scope.$watch("contract.EndDate", function (val) {
                    parseYear(val, "EndYear");
                });
                $scope.$watch("contract.HasBankGuarantee", function (val) {
                    if (val == "0") {
                        $scope.contract.BGNumber = null;
                        $scope.contract.BGCommencementDate = null;
                        $scope.contract.BGEndDate = null;
                        $scope.contract.BGAmount = null;
                    }
                });

                $scope.$watch("contract.HasDeposit", function (val) {
                    if (val == "0") {
                        $scope.contract.DepositAmount = null;
                        $scope.contract.Refundable = null;
                    }
                });

                $scope.$watch("contract.Refundable", function (val) {
                    if (!val || val == "0") {
                        $scope.contract.RefundableDate = null;
                    }
                });

                $scope.$watch("contract.WithEarlyTerminationClause", function (val) {
                    if (!val || val == "0") {
                        $scope.contract.EarlyTerminationClauseDetail = "";
                    }
                });

                $scope.editContract = function (contract) {
                    $scope.contract = contract;
                    if (contract === $scope.currentContract) {
                        $scope.contract.contractEditable = true;
                    } else {
                        $scope.contract.contractEditable = false;
                    }
                    loadRevisions($scope.contract.ContractInfoId);
                };
                $scope.addRevision = function () {
                    if (!$scope.revisions) {
                        $scope.revisions = [];
                    }
                    $scope.revisions.push({
                        ChangeDate: new Date(),
                        StoreCode: $scope.currentContract.StoreCode,
                        StoreContractInfoId: $scope.currentContract.ContractInfoId,
                        ProjectContractId: $scope.currentContract.Id,
                        ProjectId: $scope.currentContract.ProjectId,
                        StoreID: $scope.currentContract.StoreId,
                        LeaseRecapID: $scope.currentContract.LeaseRecapID,
                        RentStructureOld: $scope.currentContract.RentStructure,
                        RedlineAreaOld: $scope.currentContract.TotalLeasedArea,
                        LeaseChangeExpiryOld: $scope.currentContract.EndDate,
                        LandlordOld: $scope.currentContract.PartyAFullName
                    });
                };
                $scope.removeRevision = function (revision) {
                    $scope.revisions = $.grep($scope.revisions, function (re, i) {
                        return re !== revision;
                    });
                };
            }
        };
    }]).directive("comment", [function () {
        return {
            restrict: "EA",
            scope: {
                editable: "=",
                title: "@",
                comments: "=",
                panelStyle: "@"
            },
            replace: true,
            templateUrl: Utils.ServiceURI.AppUri + "Module/Comment",
            link: function ($scope, element, attrs) {
                if (!$scope.title) {
                    $scope.title = "Comments";
                }
                if (!$scope.panelStyle) {
                    $scope.panelStyle = "panel-orange mg_t_10";
                }
            }
        };
    }]).directive("recall", [
    "$modal",
    function ($modal) {
        return {
            restrict: "E",
            scope: {
                callBack: "&"
            },
            replace: true,
            templateUrl: Utils.ServiceURI.AppUri + "Module/Recall",
            link: function ($scope, element, attrs) {
                $scope.beginReCall = function () {
                    $modal.open({
                        size: "lg",
                        templateUrl: Utils.ServiceURI.WebAddress() + "Template/Recall",
                        backdrop: 'static',
                        resolve: {
                            condition: function () {
                                return angular.copy($scope.condition);
                            }
                        },
                        controller: [
                            "$scope", "$modalInstance", "condition", function ($scope, $modalInstance, condition) {
                                $scope.entity = {};

                                $scope.ok = function (e) {
                                    $modalInstance.close($scope.entity);
                                };
                                $scope.cancel = function () {
                                    $modalInstance.dismiss("cancel");
                                };
                            }
                        ]
                    }).result.then(function (entity) {
                        $scope.callBack(entity.Comment);
                    });
                };
            }
        };
    }
]).directive('attachments', [
    "$modal",
    "$document",
    "attachmentService",
    "contractService",
    "messager",
    function ($modal, $document, attachmentService, contractService, messager) {
        return {
            restrict: "E",
            scope: {
                refTableName: "@",
                projectId: "@",
                refTableId: "@",
                flowCode: "@",
                isHistory: "=?",
                uploadFinish: "&",
                deleteFinish: "&",
                beforePackDownload: "&",
                editable: "@",
                list: "=?",
                showPackDownload: "=?",
                showContract: "=?",
                uploadSet: "=?",
                requireSet: '=?',
                reload: '=?'
            },
            replace: true,
            templateUrl: Utils.ServiceURI.AppUri + "Module/Attachments?nc=" + Math.random(),
            link: function ($scope, element, attrs) {
                if ($scope.showContract) {
                    $scope.startIndex = 2;
                }
                if (!$scope.beforePackDownload) {
                    $scope.beforePackDownload = function () {
                        return true;
                    };
                }
                if ($scope.uploadSet == null) {
                    $scope.uploadSet = [];
                }
                $scope.uploadable = function (att) {
                    return $.inArray(att.RequirementId, $scope.uploadSet || []) >= 0;
                };
                $scope.requireable = function (att) {
                    if ($scope.requireSet && $scope.requireSet.length > 0) {
                        if ($.inArray(att.RequirementId, $scope.requireSet) >= 0) {
                            return false;
                        }
                    }
                    return true;
                };
                $scope.packDownloadLink = Utils.ServiceURI.Address() + "api/attachment/packDownload?projectId=" + $scope.projectId + "&refTableName=" + $scope.refTableName;
                var load = function () {
                    attachmentService.query({
                        refTableName: $scope.refTableName,
                        projectId: $scope.projectId,
                        flowCode: $scope.flowCode,
                        refTableId: $scope.refTableId,
                        includeCover: false
                    }).$promise.then(function (result) {
                        angular.forEach(result, function (r, i) {
                            if ($.inArray(r.RequirementId, $scope.uploadSet || []) >= 0) {
                                r.Uploadable = true;
                            }
                            if (r.ID != "00000000-0000-0000-0000-000000000000") {
                                r.downloadLink = Utils.ServiceURI.Address() + "api/attachment/download?id=" + r.ID;
                                switch (r.Extension.toLowerCase()) {
                                    case ".xlsx":
                                    case ".xls":
                                        r.Icon = "fa fa-file-excel-o c_green";
                                        break;
                                    case ".ppt":
                                        r.Icon = "fa fa-file-powerpoint-o c_red";
                                        break;
                                    case ".doc":
                                    case ".docx":
                                        r.Icon = "fa fa-file-word-o c_blue";
                                        break;
                                    default:
                                        r.Icon = "fa fa-file c_orange";
                                        break;
                                }
                            }
                        });
                        $scope.list = result;
                        $scope.$emit("onAttachsLoaded", result);
                    });
                    contractService.queryAttachmentCount($scope.projectId).then(function (response) {
                        $scope.contractAttachCount = response.data;
                    });
                };

                //if (!$scope.editable) {
                //    $scope.editable = true;
                //};
                /*事件订阅 Start*/
                $scope.$on("AttachmentUploadFinish", function () {
                    load();
                });

                /*事件订阅 End*/
                $scope.uploadAttachFinished = function (up, files) {
                    messager.showMessage("上传文件成功", "fa-check c_green").then(function () {
                        load();
                        $scope.uploadFinish && $scope.uploadFinish({
                            up: up,
                            files: files
                        });
                    });
                };
                $scope.deleteAttachment = function (id, requirementId) {
                    messager.confirm("[[[确认要删除？]]]", "fa-warning c_orange").then(function (result) {
                        if (result) {
                            attachmentService.deleteAttachment({
                                ProjectId: $scope.projectId,
                                Id: id,
                                RequirementId: requirementId
                            }).$promise.then(function () {
                                messager.showMessage("删除附件成功", "fa-check c_green");
                                load();
                                $scope.deleteFinish && $scope.deleteFinish({
                                    id: id,
                                    requirementId: requirementId
                                });
                            }, function () {
                                messager.showMessage("删除附件失败", "fa-warning c_red");
                            });
                        }
                    });
                };
                $scope.showContractAttachments = function () {
                    $modal.open({
                        size: "lg",
                        backdrop: "static",
                        templateUrl: Utils.ServiceURI.AppUri + "Module/ContractAttachments",
                        controller: [
                            "$scope", "$modalInstance",
                            function ($modalScope, $modalInstance) {
                                contractService.queryAttachments($scope.projectId).then(function (response) {
                                    angular.forEach(response.data, function (d, i) {
                                        d.downloadLink = Utils.ServiceURI.AttachmentAddress() + "api/contract/downloadAttachment?id=" + d.Id;
                                    });
                                    $modalScope.list = response.data;
                                    $modalScope.attachLoaded = true;
                                });
                                $modalScope.cancel = function () {
                                    $modalInstance.dismiss('');
                                };
                            }
                        ]
                    });
                };
                if ($scope.isHistory) {
                    $scope.$watch("refTableId", function (val) {
                        if (!!val) {
                            load();
                        }
                    });
                } else {
                    load();
                }

                $scope.$watch("reload", function (val) {
                    if (!!val) {
                        load();
                    }
                });
            }
        };
    }]).directive("materTracking", [
    "$http", "$timeout", function ($http, $timeout) {
        return {
            scope: {
                workflowId: "=",
                workflowType: "@",
                nodeId: "=",
                nodeType: "@"
            },
            restrict: "EA",
            templateUrl: Utils.ServiceURI.AppUri + "Module/MaterTracking",
            replace: true,
            link: function ($scope, element, attrs) {
                var onTimeout = function () {
                    $scope.now = new Date();
                    timer = $timeout(onTimeout, 1000);
                };
                var timer = $timeout(onTimeout, 1000);
                $scope.$on("$destroy", function () {
                    if (timer) {
                        $timeout.cancel(timer);
                    }
                });
                $scope.isShowSend = false;
                $scope.hasUnFinishFollow = false;
                var load = function () {
                    $http.get(Utils.ServiceURI.Address() + "api/matertrack", {
                        cache: false,
                        params: {
                            workflowId: $scope.workflowId,
                            nodeId: $scope.nodeId
                        }
                    }).success(function (data) {
                        $scope.reps = data.Reps;
                        $scope.repsLoaded = true;
                        $scope.hasUnFinishFollow = false;
                        $scope.isLegal = data.IsLegal;
                        $scope.isAssetActor = data.IsAssetActor;
                        angular.forEach(data.Reps, function (rep, i) {
                            if (!rep.IsFinish)
                                $scope.hasUnFinishFollow = true;
                        });
                        if (data.IsLegal || (data.Reps != null && data.Reps.length > 0)) {
                            $scope.IsShowMaterTracking = true;
                        } else {
                            $scope.IsShowMaterTracking = false;
                        }
                    });
                };
                $scope.$watch("workflowId+nodeId", function (val) {
                    if (!!$scope.workflowId && !!$scope.nodeId) {
                        load();
                    }
                });
                $scope.showSendRep = function () {
                    $scope.isShowSend = true;
                };
                $scope.cancleSendRep = function () {
                    $scope.isShowSend = false;
                };
                $scope.showReply = function (rep) {
                    rep.isShowSend = true;
                };
                $scope.cancleReply = function (rep) {
                    rep.isShowSend = false;
                };
                $scope.isHistory = false;
                if ($scope.$parent.isHistory) {
                    $scope.isHistory = true;
                }
                $scope.finish = function () {
                    $http.get(Utils.ServiceURI.Address() + "api/matertrack/finish", {
                        cache: false,
                        params: {
                            WorkFlowId: $scope.workflowId,
                            WorkFlowType: $scope.workflowType,
                            NodeType: $scope.nodeType
                        }
                    }).success(function () {
                        load();
                        $scope.isShowSend = false;
                        $scope.trackInput = "";
                    });
                };
                $scope.submitTrack = function (track) {
                    var trackType = 1;
                    var trackInput = $scope.trackInput;
                    var parentId = null;
                    if (!!track) {
                        trackType = $scope.isLegal ? 3 : 2;
                        parentId = track.Id;
                        trackInput = track.trackInput;
                    }
                    $http.post(Utils.ServiceURI.Address() + "api/matertrack/add", {
                        TrackType: trackType,
                        Content: trackInput,
                        ParentId: parentId,
                        NodeType: $scope.nodeType,
                        NodeId: $scope.nodeId,
                        WorkflowType: $scope.workflowType,
                        WorkflowId: $scope.workflowId,
                        Creator: window["currentUser"].Code,
                        CreatorZHCN: window["currentUser"].NameZHCN,
                        CreatorENUS: window["currentUser"].NameENUS
                    }).success(function () {
                        load();
                        $scope.isShowSend = false;
                        $scope.trackInput = "";
                        if (!!track) {
                            track.isShowSend = false;
                            track.trackInput = "";
                        }
                    });
                };
            }
        };
    }]).directive("approvalRecords", [
    "$http", function ($http) {
        return {
            restrict: "EA",
            scope: {
                workflowType: "@",
                refTableId: "=",
                refTableName: "@"
            },
            templateUrl: Utils.ServiceURI.AppUri + "Module/ApprovalRecords",
            replace: true,
            link: function ($scope, element, attrs) {
                $scope.$watch("refTableId", function (val) {
                    if (!!val) {
                        $http.get(Utils.ServiceURI.Address() + "api/ProjectComment/Search/" + $scope.workflowType + "/" + $scope.refTableName + "/" + val, {
                            cache: false,
                            params: {
                                workflowType: $scope.workflowType
                            }
                        }).success(function (records) {
                            $scope.records = records;
                        });
                    }
                    ;
                });
            }
        };
    }]).directive("actionLog", [
    "$http", function ($http) {
        return {
            restrict: "EA",
            replace: true,
            scope: {
                projectId: "="
            },
            templateUrl: Utils.ServiceURI.AppUri + "Module/ActionLog",
            link: function ($scope, element, attrs) {
                $scope.$watch("projectId", function (val) {
                    if (!!val) {
                        $http.get(Utils.ServiceURI.Address() + "api/actionlog", {
                            cache: false,
                            params: {
                                projectId: val
                            }
                        }).success(function (logs) {
                            $scope.ajaxFinished = true;
                            $scope.logs = logs;
                        });
                    }
                });
            }
        };
    }]).directive("contactInfo", [
    "$http", function ($http) {
        return {
            restrict: "EA",
            scope: {
                uscode: "=",
                projectId: '=?'
            },
            replace: true,
            templateUrl: Utils.ServiceURI.AppUri + "Module/ContactInfo",
            link: function ($scope, ele, attrs) {
                $scope.$watch("uscode", function (val) {
                    if (!!val) {
                        var url;
                        if ($scope.projectId) {
                            url = Utils.ServiceURI.Address() + "api/StoreUsers/GetStoreEmployeesContactInfo/" + $scope.projectId;
                        } else {
                            url = Utils.ServiceURI.Address() + "api/StoreUsers/GetStoreEmployeesByMultiRoles/" + val + "?roleCodes=Asset_Mgr,Asset_Rep,Asset_Actor";
                        }

                        $http.get(url).error(function (response) {
                        }).then(function (response) {
                            var repData = response.data;
                            $scope.contactList = [];
                            angular.forEach(repData, function (mgr, i) {
                                $scope.contactList.push(mgr);
                            });
                        });
                    }
                });
            }
        };
    }]).directive("topNav", [
    "flowService",
    "taskWorkService",
    function (flowService, taskWorkService) {
        return {
            restrict: "E",
            scope: {
                flowCode: "@",
                subCode: "@",
                projectId: "@",
                placeHolder: "@"
            },
            templateUrl: Utils.ServiceURI.AppUri + "Module/TopNav",
            replace: true,
            link: function ($scope, element, attrs) {
                $scope.navs = [];
                var $table = $("#navProjectTable");
                for (var i = 0; i < $scope.placeHolder || 0; i++) {
                    $scope.navs.push({});
                }
                var itemWidth = 160, pageSize;
                var caculateNavScroll = function () {
                    $scope.enableLeftScroll = false;
                    $scope.enableRightScroll = false;
                    var cWidth = $(".nav-project").width(), tWidth;
                    if (cWidth < 540) {
                        tWidth = 160;
                    } else if (cWidth < 700) {
                        tWidth = 480;
                    } else if (cWidth < 1020) {
                        tWidth = 640;
                    } else {
                        tWidth = 960;
                    }
                    $table.width(tWidth);
                    pageSize = Math.floor(tWidth / itemWidth);
                    $scope.pageCount = Math.ceil($scope.navCount / pageSize);
                    $scope.pageIndex = Math.ceil($scope.currentNavIndex / pageSize);

                    var itemStart = ($scope.pageIndex - 1) * pageSize;
                    if ($scope.pageIndex > 1) {
                        $scope.enableLeftScroll = true;
                    }
                    if ($scope.pageIndex * pageSize <= $scope.navCount) {
                        $scope.enableRightScroll = true;
                    }
                    if (!$table.is(":animated")) {
                        $table.animate({ scrollLeft: itemStart * itemWidth }, 10, "linear");
                    }
                };
                $(window).resize(caculateNavScroll);
                flowService.topNav({
                    projectId: $scope.projectId,
                    flowCode: $scope.flowCode,
                    subCode: $scope.subCode
                }).$promise.then(function (data) {
                    $scope.navs = data;
                    $scope.navs = data;
                    $scope.navCount = $scope.navs.length;
                    $scope.currentNavIndex = 0;
                    angular.forEach(data, function (n, i) {
                        if (n.Code == $scope.subCode) {
                            $scope.currentNavIndex = i + 1;
                        }
                    });
                    caculateNavScroll();
                    $scope.navLoaded = true;
                });
                $scope.scrollNav = function (direction) {
                    switch (direction) {
                        case "left":
                            if ($scope.pageIndex <= 1) {
                                return;
                            }
                            $scope.pageIndex -= 1;
                            break;
                        case "right":
                            if ($scope.pageIndex >= $scope.pageCount) {
                                return;
                            }
                            $scope.pageIndex += 1;
                            break;
                    }
                    ;
                    var itemStart = ($scope.pageIndex - 1) * pageSize;
                    if ($scope.pageIndex == 1) {
                        $scope.enableLeftScroll = false;
                    } else {
                        $scope.enableLeftScroll = true;
                    }
                    if ($scope.pageIndex * pageSize > $scope.navCount) {
                        $scope.enableRightScroll = false;
                    } else {
                        $scope.enableRightScroll = true;
                    }
                    if (!$table.is(":animated")) {
                        $table.animate({ scrollLeft: itemStart * itemWidth }, 500, "linear");
                    }
                };
                $scope.loadOperators = function (nav, projectId) {
                    nav.operatorHTML = "<img src='" + Utils.ServiceURI.AppUri + "Content/Images/loading.gif' />";
                    taskWorkService.getOperators(nav.Code, projectId).then(function (response) {
                        var operators = response.data;
                        var OperatorHTML = "";
                        if (!!operators && operators.length > 0) {
                            OperatorHTML += "<ul class='node-operators'>";
                            angular.forEach(operators, function (o, i) {
                                OperatorHTML += "<li>" + o.OperateMsgZHCN + "</li>";
                            });
                            OperatorHTML += "</ul>";
                        } else if (nav.IsFinished) {
                            OperatorHTML = "<p class='node-operators-none'>已完成</p>";
                        } else {
                            OperatorHTML = "<p class='node-operators-none'>暂无处理人</p>";
                        }
                        nav.operatorHTML = OperatorHTML;
                    });
                };
            }
        };
    }]).factory("messager", [
    "$sce",
    "$modal",
    function ($sce, $modal) {
        return {
            showMessage: function (content, iconClass, modalSize) {
                modalSize = modalSize || "sm";
                return $modal.open({
                    backdrop: 'static',
                    size: modalSize,
                    windowClass: "am-messager",
                    templateUrl: Utils.ServiceURI.AppUri + "Module/ShowMessage",
                    controller: [
                        "$scope", "$modalInstance", function ($scope, $modalInstance) {
                            $scope.content = $sce.trustAsHtml(content);
                            $scope.iconClass = iconClass;
                            $scope.ok = function () {
                                $modalInstance.close(true);
                            };
                        }]
                }).result;
            },
            confirm: function (content, iconClass) {
                return $modal.open({
                    backdrop: 'static',
                    size: "sm",
                    templateUrl: Utils.ServiceURI.AppUri + "Module/Confirm",
                    controller: [
                        "$scope", "$modalInstance", function ($scope, $modalInstance) {
                            $scope.content = $sce.trustAsHtml(content);
                            $scope.iconClass = iconClass;
                            $scope.ok = function () {
                                $modalInstance.close(true);
                            };
                            $scope.cancel = function () {
                                $modalInstance.close(false);
                            };
                        }]
                }).result;
            },
            blockUI: function (content) {
                return $modal.open({
                    backdrop: 'static',
                    size: "sm",
                    template: "<div class='modal-body'>\
                                        <table>\
                                            <tr valign='middle'>\
                                                <td class='pd_0_15'>\
                                                    <img src='/Content/Images/loading.gif' alt='' />\
                                                </td>\
                                                <td class='fs_14' ng-bind-html='content'></td>\
                                            </tr>\
                                        </table>\
                                    </div>",
                    controller: [
                        "$scope", "$modalInstance", function ($scope, $modalInstance) {
                            $scope.content = $sce.trustAsHtml(content);
                            $(document).data("blocker", $modalInstance);
                        }]
                }).result;
            },
            unBlockUI: function () {
                var mInstance = $(document).data("blocker");
                mInstance && mInstance.dismiss('');
                $(document).removeData("blocker");
            }
        };
    }
]).directive('reinvestmentBasicInfo', [
    "$http", "messager", "storeService", "dictionaryHandler", function ($http, messager, storeService, dictionaryHandler) {
        return {
            restrict: 'EA',
            replace: true,
            templateUrl: '/Module/ReinvestmentBasicInfo',
            scope: {
                source: '=?',
                code: "=",
                storeTypeName: "@",
                editable: '=',
                flowCode: '@',
                now: '=?'
            },
            link: function ($scope, element, attrs) {
                $scope.now = new Date();
                $scope.$watch("code", function (val) {
                    if (val) {
                        $http.get(Utils.ServiceURI.Address() + "api/StoreSTLocation/" + $scope.code).then(function (response) {
                            if (response.data == null) {
                                return;
                            }
                            $scope.storeLocation = response.data;
                            if (response.data.StoreTypeName) {
                                switch (response.data.StoreTypeName) {
                                    case 'IS':
                                        $scope.editDTSize = false;
                                        break;
                                }
                            }
                            $scope.originSTL = response.data;
                            $scope.frmReinBasic.GBDate.$setValidity("date", true);
                            $scope.frmReinBasic.ConsCompletionDate.$setValidity("date", true);
                            $scope.frmReinBasic.ReopenDate.$setValidity("date", true);
                        });
                        dictionaryHandler.queryByParent({ parentCode: 'DesignType' }).$promise.then(function (dicts) {
                            if (dicts != null) {
                                $scope.NewDesignTypes = dicts;
                            }
                        });
                    }
                });
                $scope.alternativeOptions = [
                    { text: 'Yes', value: true },
                    { text: 'No', value: false }
                ];

                $scope.open = function ($event, ele) {
                    $event.preventDefault();
                    $event.stopPropagation();

                    $scope[ele] = true;
                };

                $scope.dateOptions = {
                    formatYear: 'yy',
                    startingDay: 1
                };
                $scope.editField = function (field) {
                    $scope['edit' + field] = true;
                };
                $scope.saveField = function (field, frm) {
                    messager.confirm("数据将会直接更新到系统中该餐厅的基本信息!", "fa-warning c_orange").then(function (result) {
                        if (result) {
                            frm.$submited = true;
                            if (frm.$valid) {
                                $http.post(Utils.ServiceURI.Address() + "api/StoreSTLocation/update", $scope.storeLocation).then(storeService.updateStoreLocation($scope.storeLocation).$promise.then(function () {
                                    messager.showMessage("保存成功！", "fa-warning c_orange");
                                    $scope['edit' + field] = false;
                                }, function () {
                                    messager.showMessage("保存失败！", "fa-warning c_orange");
                                    $scope.storeLocation[field] = $scope.originSTL[field];
                                }));
                            }
                        } else {
                            $scope.storeLocation[field] = $scope.originSTL[field];

                            $scope['edit' + field] = false;
                        }
                    });
                };
                $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
                $scope.format = $scope.formats[1];
            }
        };
    }]).directive('reinvestmentInfo', [
    "$http", 'storeService', function ($http, storeService) {
        return {
            restrict: 'EA',
            replace: true,
            templateUrl: '/Module/ReinvestmentInfo',
            scope: {
                projectId: "=",
                code: "=",
                source: '=?',
                editable: '='
            },
            link: function ($scope, element, attrs) {
                $scope.$watch("code", function (val) {
                    if (val) {
                        storeService.getStoreDetailInfo({ usCode: $scope.code }).$promise.then(function (result) {
                            if (result) {
                                $scope.storeBasicInfo = result.storeInfo;
                            }
                        });
                    }
                });

                $scope.alternativeOptions = [
                    { text: 'Yes', value: true },
                    { text: 'No', value: false }
                ];

                $scope.open = function ($event, ele) {
                    $event.preventDefault();
                    $event.stopPropagation();

                    $scope[ele] = true;
                };

                $scope.dateOptions = {
                    formatYear: 'yy',
                    startingDay: 1
                };

                $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
                $scope.format = $scope.formats[1];
            }
        };
    }]).directive('summaryReinvestmentCost', [
    "$http", "summaryReinvestmentCostService", function ($http, summaryReinvestmentCostService) {
        return {
            restrict: 'EA',
            replace: true,
            templateUrl: '/Module/SummaryReinvestmentCost',
            scope: {
                projectId: "=",
                code: "=",
                source: '=?',
                editable: '='
            },
            link: function ($scope, element, attrs) {
                $scope.alternativeOptions = [
                    { text: 'Yes', value: true },
                    { text: 'No', value: false }
                ];

                $scope.open = function ($event, ele) {
                    $event.preventDefault();
                    $event.stopPropagation();

                    $scope[ele] = true;
                };

                $scope.dateOptions = {
                    formatYear: 'yy',
                    startingDay: 1
                };

                $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
                $scope.format = $scope.formats[1];
            }
        };
    }]).directive('storeProfitabilityAndLeaseInfo', [
    "$http", "StoreProfitabilityAndLeaseInfoService", function ($http, StoreProfitabilityAndLeaseInfoService) {
        return {
            restrict: 'EA',
            replace: true,
            templateUrl: '/Module/StoreProfitabilityAndLeaseInfo',
            scope: {
                projectId: "=",
                code: "=",
                source: '=?',
                editable: '=',
                gbdate: '@',
                leaseenddate: '@',
                ttmSales: '=?'
            },
            link: function ($scope, element, attrs) {
                if (!!$scope.editable) {
                    StoreProfitabilityAndLeaseInfoService.getSelectYearMonth({
                        projectId: $scope.projectId
                    }).$promise.then(function (response) {
                        $scope.yearMonthList = response.data;
                        angular.forEach(response.data, function (d, i) {
                            if (d.selected)
                                $scope.source.AsOf = d.value;
                        });
                    });
                }

                $scope.alternativeOptions = [
                    { text: 'Yes', value: true },
                    { text: 'No', value: false }
                ];

                $scope.open = function ($event, ele) {
                    $event.preventDefault();
                    $event.stopPropagation();

                    $scope[ele] = true;
                };

                $scope.dateOptions = {
                    formatYear: 'yy',
                    startingDay: 1
                };

                $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
                $scope.format = $scope.formats[1];
                $scope.$watch('leaseenddate+gbdate', function (val) {
                    if (val && $scope.source) {
                        if ($scope.leaseenddate && $scope.gbdate) {
                            $scope.source.RemainingLeaseYears = Math.abs(moment($scope.leaseenddate).year() - moment($scope.gbdate).year());
                        } else {
                            $scope.source.RemainingLeaseYears = 0;
                        }
                    }
                });
            }
        };
    }]).directive('financialPreAnalysis', [
    "$http", "financialPreAnalysisService", '$filter', 'messager',
    function ($http, financialPreAnalysisService, $filter, messager) {
        return {
            restrict: 'EA',
            replace: true,
            templateUrl: '/Module/FinancialPreanalysis',
            scope: {
                projectId: "=",
                source: '=?',
                editable: '=',
                code: "=",
                spali: '=?'
            },
            link: function ($scope, element, attrs) {
                $scope.filterDefaultDate = function (val) {
                    var result = null;
                    if (val) {
                        result = Utils.Common.filterDefaultDate(val);
                    }

                    return result;
                };

                $scope.getTTMAndRoI = function () {
                    financialPreAnalysisService.getTTMAndRoI({
                        projectId: $scope.projectId,
                        usCode: $scope.code,
                        dateTime: $scope.spali.AsOf
                    }).$promise.then(function (response) {
                        if (response) {
                            $scope.source.TTMSales = response.TTMSales;
                            $scope.source.ROI = response.ROI;
                            $scope.source.CurrentPriceTier = response.CurrentPriceTier;
                            $scope.source.PaybackYears = response.PaybackYears;

                            if ($scope.spali) {
                                $scope.spali.TTMSOIPercent = response.STFinanceData.SOI_TTM;
                            }
                        }
                    });
                };

                $scope.$on("LoadFinancialPreAnalysis", function () {
                    $scope.getTTMAndRoI();
                });

                $scope.$watch('spali.AsOf', function (val) {
                    if (val) {
                        $scope.getTTMAndRoI();
                    }
                });

                //financialPreAnalysisService.get({
                //    projectId: $scope.projectId,
                //    usCode: $scope.code,
                //    pageType: $scope.pageType
                //}).$promise.then(function (data) {
                //        if (!!data) {
                //            $scope.source = data;
                //        }
                //    });
                $scope.alternativeOptions = [
                    { text: 'Yes', value: true },
                    { text: 'No', value: false }
                ];

                $scope.open = function ($event, ele) {
                    $event.preventDefault();
                    $event.stopPropagation();

                    $scope[ele] = true;
                };

                $scope.dateOptions = {
                    formatYear: 'yy',
                    startingDay: 1
                };

                $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
                $scope.format = $scope.formats[1];
                $scope.$watch('source.ReimagePrice+source.TTMSales', function (newVal, oldVal) {
                    if ($scope.source && newVal != oldVal) {
                        $scope.source.ReimagePriceSales = $scope.source.TTMSales * $scope.source.ReimagePrice;
                    }
                });
                $scope.$watch('source.IsMcCafe', function (val) {
                    if ($scope.source) {
                        if (val == 0) {
                            $scope.source.McCafe = null;
                        }
                    }
                });
                $scope.$watch('source.McCafe+source.TTMSales', function (newVal, oldVal) {
                    if ($scope.source && newVal != oldVal) {
                        $scope.source.McCafeSales = $scope.source.TTMSales * $scope.source.McCafe;
                    }
                });
                $scope.$watch('source.IsKiosk', function (val) {
                    if ($scope.source) {
                        if (val == 0) {
                            $scope.source.Kiosk = null;
                        }
                    }
                });
                $scope.$watch('source.Kiosk+source.TTMSales', function (newVal, oldVal) {
                    if ($scope.source && newVal != oldVal) {
                        $scope.source.KioskSales = $scope.source.TTMSales * $scope.source.Kiosk;
                    }
                });
                $scope.$watch('source.IsMDS', function (val) {
                    if ($scope.source) {
                        if (val == 0) {
                            $scope.source.MDS = null;
                        }
                    }
                });
                $scope.$watch('source.MDS+source.TTMSales', function (newVal, oldVal) {
                    if ($scope.source && newVal != oldVal) {
                        $scope.source.MDSSales = $scope.source.TTMSales * $scope.source.MDS;
                    }
                });
                $scope.$watch('source.IsTwientyFourHour', function (val) {
                    if ($scope.source) {
                        if (val == 0) {
                            $scope.source.TwientyFourHour = null;
                        }
                    }
                });
                $scope.$watch('source.TwientyFourHour+source.TTMSales', function (newVal, oldVal) {
                    if ($scope.source && newVal != oldVal) {
                        $scope.source.TwientyFourHourSales = $scope.source.TTMSales * $scope.source.TwientyFourHour;
                    }
                });
                $scope.$watch('source.ReimagePrice+source.McCafe+source.Kiosk+source.MDS+source.TwientyFourHour', function (newVal, oldVal) {
                    if ($scope.source && newVal != oldVal) {
                        var reimagePrice = isNaN(parseFloat($scope.source.ReimagePrice)) ? 0 : parseFloat($scope.source.ReimagePrice);
                        var mcCafe = isNaN(parseFloat($scope.source.McCafe)) ? 0 : parseFloat($scope.source.McCafe);
                        var kiosk = isNaN(parseFloat($scope.source.Kiosk)) ? 0 : parseFloat($scope.source.Kiosk);
                        var mDS = isNaN(parseFloat($scope.source.MDS)) ? 0 : parseFloat($scope.source.MDS);
                        var twientyFourHour = isNaN(parseFloat($scope.source.TwientyFourHour)) ? 0 : parseFloat($scope.source.TwientyFourHour);
                        $scope.source.TotalSalesInc = reimagePrice + mcCafe + kiosk + mDS + twientyFourHour;
                    }
                });
                $scope.$watch('source.ReimagePriceSales+source.McCafeSales+source.KioskSales+source.MDSSales+source.TwientyFourHourSales', function (newVal, oldVal) {
                    if ($scope.source && newVal != oldVal) {
                        var reimagePriceSales = isNaN(parseFloat($scope.source.ReimagePriceSales)) ? 0 : parseFloat($scope.source.ReimagePriceSales);
                        var mcCafeSales = isNaN(parseFloat($scope.source.McCafeSales)) ? 0 : parseFloat($scope.source.McCafeSales);
                        var kioskSales = isNaN(parseFloat($scope.source.KioskSales)) ? 0 : parseFloat($scope.source.KioskSales);
                        var mDSSales = isNaN(parseFloat($scope.source.MDSSales)) ? 0 : parseFloat($scope.source.MDSSales);
                        var twientyFourHourSales = isNaN(parseFloat($scope.source.TwientyFourHourSales)) ? 0 : parseFloat($scope.source.TwientyFourHourSales);
                        $scope.source.IncSales = reimagePriceSales + mcCafeSales + kioskSales + mDSSales + twientyFourHourSales;
                    }
                });

                $scope.$watch('source.IncSales+source.StoreCM', function (newVal, oldVal) {
                    if ($scope.source && newVal != oldVal) {
                        var storeCM = isNaN(parseFloat($scope.source.StoreCM)) ? 0 : parseFloat($scope.source.StoreCM);
                        var incSales = isNaN(parseFloat($scope.source.IncSales)) ? 0 : parseFloat($scope.source.IncSales);
                        $scope.source.MarginInc = storeCM * incSales;
                    }
                });
                //$scope.$watch('source.SalesBuildingInvestment+source.IINB', function (newVal, oldVal) {
                //    if ($scope.source
                //        && newVal != oldVal) {
                //        var SalesBuildingInvestment = isNaN(parseFloat($scope.source.SalesBuildingInvestment)) ? 0 : parseFloat($scope.source.SalesBuildingInvestment);
                //        var IINB = isNaN(parseFloat($scope.source.IINB)) ? 0 : parseFloat($scope.source.IINB);
                //        $scope.source.NoneSalesBuildingInvst = SalesBuildingInvestment + IINB;
                //    }
                //});
            }
        };
    }]).directive('writeOffAmount', function () {
    return {
        restrict: 'EA',
        replace: true,
        templateUrl: '/Module/WriteOffAmount',
        scope: {
            source: '=?',
            editable: '=',
            uploadApi: '@',
            templateUrl: '@',
            isCheckPage: "=?",
            consInfoId: "=?",
            uploadFinish: '&'
        },
        link: function ($scope, $http, element, attrs) {
            $scope.downLoadTemplateUrl = Utils.ServiceURI.Address() + $scope.templateUrl;
            $scope.$watch("consInfoId", function (val) {
                if (val == null || val == "")
                    return;
                $scope.downLoadTemplateUrl = Utils.ServiceURI.Address() + $scope.templateUrl + "?consInfoID=" + val;
            });
            $scope.uploadWOFinish = function () {
                if ($scope.uploadFinish()) {
                    ($scope.uploadFinish())();
                }
            };
            $scope.ActualVSBudget = function (FinanceActual, Budget) {
                var returnVal = null;
                try  {
                    if (FinanceActual != null && Budget != null) {
                        returnVal = (parseFloat(FinanceActual) - parseFloat(Budget)).toString();
                    }
                } catch (e) {
                }
                return returnVal;
            };
            $scope.Variance = function (actualVSBudget, Budget) {
                var returnVal = null;
                try  {
                    if (actualVSBudget != null && Budget != null && Budget != 0) {
                        returnVal = ((parseFloat(actualVSBudget) / parseFloat(Budget)) * 100).toString();
                    }
                } catch (e) {
                }
                return returnVal;
            };
            $scope.Total = function (val1, val2) {
                var returnVal = null;
                try  {
                    if (val1 != null && val2 != null) {
                        returnVal = (parseFloat(val1) + parseFloat(val2)).toString();
                    } else if (val1 != null) {
                        returnVal = val1;
                    } else if (val2 != null) {
                        returnVal = val2;
                    }
                } catch (e) {
                }
                return returnVal;
            };
        }
    };
}).directive("dictSelector", [
    "dictionaryHandler", function (dictionaryHandler) {
        return {
            restrict: "E",
            replace: true,
            template: "\
                <form name='frmDict'>\
                <div>\
                    <div ng-if='loaded' class='am-dropdown'>\
                        <p class='input-group'>\
                            <input type='text' class='form-control' style='text-align:center;' name='selectedDictField' ng-model='selectedDict.NameENUS' ng-if='required' required ng-disabled='!editable'  />\
                            <input type='text' class='form-control' style='text-align:center;' ng-model='selectedDict.NameENUS' ng-if='!required' ng-disabled='!editable' />\
                            <span class='input-group-btn'>\
                                <button type='button' class='btn btn-default' ng-click='showOptions()'><i class='fa' ng-class='showItems?\"fa-angle-up\":\"fa-angle-down\"'></i></button>\
                            </span>\
                        </p>\
                        <err-msgs field='frmDict.selectedDictField' submited='$root.$submited' ng-if='required&&editable'></err-msgs>\
                        <div class='am-dropdown-list' ng-show='showItems' style='line-height: 20px;'>\
                            <a ng-click='chose(d)' ng-repeat='d in dicts' ng-class='{\"inactive\":d.Status === 0,\"selected\":d.Selected}'>{{d.NameENUS}}</a>\
                        </div>\
                    </div>\
                    <img src = '" + Utils.ServiceURI.AppUri + "Content/Images/loading.gif' ng-if ='!loaded' /></div></form>",
            scope: {
                parentCode: "@",
                required: "@",
                selectedCode: "=?",
                selectedDict: "=?",
                dictFilter: '=?',
                editable: "=?",
                defaultSelect: '=?'
            },
            link: function ($scope, element, attrs) {
                dictionaryHandler.queryByParent({ parentCode: $scope.parentCode }).$promise.then(function (dicts) {
                    if ($scope.dictFilter) {
                        var dictsLocal = [];
                        angular.forEach(dicts, function (d, i) {
                            if ($.inArray(d.NameENUS, $scope.dictFilter) >= 0) {
                                dictsLocal.push(d);
                            }
                        });
                        $scope.dicts = dictsLocal;
                    } else {
                        $scope.dicts = dicts;
                    }

                    if ($scope.defaultSelect && $scope.dicts && $scope.dicts.length == 1) {
                        $scope.selectedCode = $scope.dicts[0].Code;
                    }

                    $scope.loaded = true;
                });
                $scope.$watch("selectedCode+dicts", function (val) {
                    angular.forEach($scope.dicts, function (d, i) {
                        if (d.Code == $scope.selectedCode) {
                            $scope.selectedDict = d;
                            d.Selected = true;
                        } else {
                            d.Selected = false;
                        }
                    });
                });
                $scope.showOptions = function () {
                    if ($scope.editable) {
                        $scope.showItems = true;
                    }
                };
                $scope.chose = function (dict) {
                    angular.forEach($scope.dicts, function (d, i) {
                        d.Selected = d === dict;
                    });
                    $scope.selectedCode = dict.Code;
                    $scope.selectedDict = dict;
                    $scope.showItems = false;
                };
                element.on("click", ":text", function () {
                    $(this).blur();
                });
                $(document).click(function (e) {
                    var target = angular.element(e.srcElement || e.target).get(0);
                    if (element.has(target).length == 0) {
                        $scope.showItems = false;
                        $scope.$apply();
                    }
                });
            }
        };
    }]).directive("dictMutiSelector", [
    "dictionaryHandler", function (dictionaryHandler) {
        return {
            restrict: "E",
            replace: true,
            template: "\
                <form name='frmDict'>\
                <div>\
                    <div ng-if='loaded'>\
                        <div ng-if='required'>\
                            <select name='selectedCode' required chosen='{{chosenOptions}}' multiple='multiple' class='form-control' ng-model='$parent.$parent.selectedCodes' ng-options='d.Code as d.NameZHCN for d in dicts'>\
                                <option value = ''>请选择</option>\
                            </select>\
                            <err-msgs field='frmDict.selectedCode' submited='$root.$submited'></err-msgs>\
                        </div>\
                        <div ng-if='!required'>\
                            <select name='selectedCode' chosen='{{chosenOptions}}'  multiple='multiple' class='form-control' ng-model='$parent.$parent.selectedCodes' ng-options='d.Code as d.NameZHCN for d in dicts'>\
                                <option value = ''>请选择</option>\
                            </select>\
                        </div>\
                    </div>\
                    <img src = '" + Utils.ServiceURI.AppUri + "Content/Images/loading.gif' ng-if ='!loaded' /></div></form>",
            scope: {
                parentCode: "@",
                required: "@",
                selectedCodes: "=?",
                selectedDicts: "=?",
                dictFilter: '=?',
                editable: "=?",
                defaultSelect: '=?'
            },
            link: function ($scope, element, attrs) {
                $scope.$watch("editable", function (val) {
                    $scope.chosenOptions = val ? '{disable_search:true,width:"100%"}' : '{disable_search:true,width:"100%",disabled:true}';
                });
                dictionaryHandler.queryByParent({ parentCode: $scope.parentCode }).$promise.then(function (dicts) {
                    if ($scope.dictFilter) {
                        var dictsLocal = [];
                        angular.forEach(dicts, function (d, i) {
                            if ($.inArray(d.NameENUS, $scope.dictFilter) >= 0) {
                                dictsLocal.push(d);
                            }
                        });
                        $scope.dicts = dictsLocal;
                    } else {
                        $scope.dicts = dicts;
                    }

                    if ($scope.defaultSelect && $scope.dicts && $scope.dicts.length == 1) {
                        $scope.selectedCodes = [$scope.dicts[0].Code];
                    }

                    $scope.loaded = true;
                });
                $scope.$watch("selectedCodes", function (val) {
                    if (!!val) {
                        var result = [];
                        angular.forEach($scope.dicts, function (d, i) {
                            if ($.inArray(d.Code, val) >= 0) {
                                result.push(d);
                            }
                        });
                        $scope.selectedDicts = result;
                    }
                });
            }
        };
    }]).directive('reinvestmentCost', [
    "$window", 'messager', function ($window, messager) {
        return {
            restrict: "EA",
            replace: true,
            scope: {
                source: '=?',
                uploadApi: '@',
                templateUrl: '@',
                editable: '=',
                beforeDownload: '&',
                isCheckPage: "=?",
                consInfoId: "=?",
                uploadFinish: '&',
                hideNormType: '=?',
                normTypeFilter: '=?'
            },
            templateUrl: "/Module/ReinvestmentCost",
            link: function ($scope, ele, attrs) {
                $scope.$watch("consInfoId", function (val) {
                    if (val == null || val == "")
                        return;
                    $scope.downLoadTemplateUrl = Utils.ServiceURI.Address() + $scope.templateUrl + "?consInfoID=" + val;
                });

                $scope.beforeUploadFA = function (up, files) {
                    //$scope.saveBasicInfo();
                };

                $scope.saveBasicInfo = function () {
                    if ($scope.source && ($scope.source.NormType || $scope.hideNormType)) {
                        if ($scope.beforeDownload) {
                            $scope.beforeDownload({ redirectUrl: Utils.ServiceURI.Address() + $scope.templateUrl + "?NormType=" + $scope.source.NormType });
                        }
                    } else {
                        messager.showMessage('请选择 Norm Type!', "fa-warning c_orange");
                    }
                };
                $scope.uploadFAFinish = function () {
                    if ($scope.uploadFinish()) {
                        ($scope.uploadFinish())();
                    }
                };
                $scope.PMActVSBudget = function (PMActual, Budget) {
                    var returnVal = null;
                    try  {
                        if (PMActual != null && Budget != null) {
                            returnVal = (parseFloat(PMActual) - parseFloat(Budget)).toString();
                        }
                    } catch (e) {
                    }
                    return returnVal;
                };
                $scope.Variance = function (pMActVSBudget, Budget) {
                    var returnVal = null;
                    try  {
                        if (Budget != null && Budget != 0 && pMActVSBudget != null) {
                            returnVal = ((parseFloat(pMActVSBudget) / parseFloat(Budget)) * 100).toString();
                        }
                    } catch (e) {
                    }
                    return returnVal;
                };
            }
        };
    }]).directive("reinvstcostAndWriteoff", [
    '$filter', function ($filter) {
        return {
            restrict: "EA",
            replace: true,
            scope: {
                writeOffSource: '=?',
                inventCostSource: "=?",
                editable: '=',
                reinvenstmentType: '='
            },
            templateUrl: "/Module/ReinvstCostAndWriteOff",
            link: function ($scope, ele, attrs) {
                $scope.$watch('inventCostSource.RECostNorm+inventCostSource.LHINorm+inventCostSource.ESSDNorm', function (newVal, oldVal) {
                    if ($scope.reinvenstmentType == 2 && $scope.inventCostSource && newVal != oldVal) {
                        var RECostNorm = isNaN(parseFloat($scope.inventCostSource.RECostNorm)) ? 0 : parseFloat($scope.inventCostSource.RECostNorm);
                        var LHINorm = isNaN(parseFloat($scope.inventCostSource.LHINorm)) ? 0 : parseFloat($scope.inventCostSource.LHINorm);
                        var ESSDNorm = isNaN(parseFloat($scope.inventCostSource.ESSDNorm)) ? 0 : parseFloat($scope.inventCostSource.ESSDNorm);
                        $scope.inventCostSource.TotalReinvestmentNorm = Number(Utils.caculator.plus(Utils.caculator.plus(RECostNorm, LHINorm), ESSDNorm).toFixed(2));
                    }
                });

                $scope.$watch('writeOffSource.REWriteOff+writeOffSource.LHIWriteOff+writeOffSource.ESSDWriteOff', function (newVal, oldVal) {
                    if ($scope.reinvenstmentType == 2 && $scope.writeOffSource && newVal != oldVal) {
                        var REWriteOff = isNaN(parseFloat($scope.writeOffSource.REWriteOff)) ? 0 : parseFloat($scope.writeOffSource.REWriteOff);
                        var LHIWriteOff = isNaN(parseFloat($scope.writeOffSource.LHIWriteOff)) ? 0 : parseFloat($scope.writeOffSource.LHIWriteOff);
                        var ESSDWriteOff = isNaN(parseFloat($scope.writeOffSource.ESSDWriteOff)) ? 0 : parseFloat($scope.writeOffSource.ESSDWriteOff);
                        $scope.writeOffSource.TotalWriteOff = Number(Utils.caculator.plus(Utils.caculator.plus(REWriteOff, LHIWriteOff), ESSDWriteOff).toFixed(2));
                    }
                });
            }
        };
    }]).directive("aSimpleTour", [
    function () {
        return {
            restrict: "E",
            template: "",
            replace: true,
            scope: {
                options: "="
            },
            link: function ($scope, element, attrs) {
                $["aSimpleTour"]($scope.options);
            }
        };
    }
]).directive("estimatedVsActualConstruction", [
    function () {
        return {
            restrict: "EA",
            templateUrl: "/Module/EstimatedVsActualConstruction",
            replace: true,
            scope: {
                source: "=",
                editable: "=",
                flowCode: '@'
            },
            link: function ($scope, element, attrs) {
                $scope.open = function ($event, ele) {
                    $event.preventDefault();
                    $event.stopPropagation();

                    $scope[ele] = true;
                };

                $scope.dateOptions = {
                    formatYear: 'yy',
                    startingDay: 1
                };

                $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
                $scope.format = $scope.formats[1];
            }
        };
    }
]).directive('siteInfo', [
    "siteInfoService",
    "storeService",
    '$filter',
    function (siteInfoService, storeService, $filter) {
        return {
            restrict: "EA",
            templateUrl: "/Module/SiteInfo",
            replace: true,
            scope: {
                source: "=",
                editable: "=",
                usCode: "=",
                identifier: '=',
                store: '=?',
                flowCode: '@',
                projectId: '@',
                isInnerDesignDisable: '=?'
            },
            link: function ($scope, element, attrs) {
                $scope.dataLoaded = false;
                $scope.DirectionalEffects = [];
                $scope.DTTypeNames = [];
                $scope.Floors = [];
                $scope.KitchenFloors = [];
                $scope.FrontCounterFloors = [];
                $scope.ExteriorDesigns = [];
                $scope.flowCodeFilter = ['Reimage_SiteInfo', 'Rebuild_SiteInfo'];
                $scope.isInnerDesignDisable = false;

                if ($scope.flowCode && $.inArray($scope.flowCode, $scope.flowCodeFilter) >= 0) {
                    $scope.isInnerDesignDisable = true;
                }
                var loadDropdown = function () {
                    siteInfoService.getDropdownDatas().$promise.then(function (data) {
                        if (data != null) {
                            $scope.DirectionalEffects = data.DirectionalEffects;
                            $scope.DTTypeNames = data.DTTypeNames;
                            $scope.Floors = data.Floors;
                            $scope.FloorsCount = ['1', '2', '3', '4', '5'];
                            $scope.KitchenFloors = data.KitchenFloors;
                            $scope.FrontCounterFloors = data.FrontCounterFloors;
                            $scope.ExteriorDesigns = data.ExteriorDesigns;
                            $scope.InnerDesign = data.InnerDesign;
                            $scope.AppearDesign = data.AppearDesign;
                        }
                        $scope.dataLoaded = true;
                    });
                };
                $scope.$watch("usCode", function (val) {
                    if (val != null && val != "") {
                        $scope.dataLoaded = false;
                        siteInfoService.getSiteInfo({ usCode: val, identifier: $scope.identifier, projectId: $scope.projectId, flowCode: $scope.flowCode }).$promise.then(function (data) {
                            $scope.source = data;
                            $scope.source.PlayPlace = data.PlayPlace;
                            $scope.source.PartyRoom = data.PartyRoom;
                            loadDropdown();
                        });
                        storeService.getStoreBasic({ usCode: val }).$promise.then(function (data) {
                            if (data != null) {
                                $scope.store = data;
                            }
                        });
                    }
                });
                $scope.floorChange = function () {
                    if ($scope.source.Floor) {
                        var pendingClearFloorArray = [];
                        var floorIndex = $.inArray($scope.source.Floor, $scope.FloorsCount);
                        for (var i = floorIndex + 1; i < $scope.FloorsCount.length; i++) {
                            pendingClearFloorArray.push($scope.FloorsCount[i]);
                        }
                        $.each(pendingClearFloorArray, function (index, val) {
                            $scope.source['Floor' + val] = null;
                            $scope.source['Size' + val] = null;
                            $scope.source['Seats' + val] = null;
                        });
                    }
                };
                $scope.$watch('source.McdCarParkCount+source.PublicCarParkCount+source.RoadCarParkCount', function (newVal, oldVal) {
                    if ($scope.source && newVal != oldVal) {
                        var mcdCarParkCount = $.isNumeric($scope.source.McdCarParkCount) ? parseFloat($scope.source.McdCarParkCount) : 0;
                        var publicCarParkCount = $.isNumeric($scope.source.PublicCarParkCount) ? parseFloat($scope.source.PublicCarParkCount) : 0;
                        var roadCarParkCount = $.isNumeric($scope.source.RoadCarParkCount) ? parseFloat($scope.source.RoadCarParkCount) : 0;
                        $scope.source.CarParkTotal = mcdCarParkCount + publicCarParkCount + roadCarParkCount;
                    }
                });

                $scope.getSeatingRatio = function () {
                    if ($scope.source && $scope.source.TotalArea && $scope.source.TotalSeatsNo) {
                        $scope.source.SeatingRatio = $filter("numberCustom")($scope.source.TotalArea == 0 ? 0 : ($scope.source.TotalSeatsNo / $scope.source.TotalArea), 2);
                        ;
                    }
                };
                $scope.$watch('source.Size1+source.Size2+source.Size3+source.Size4+source.Size5', function (newVal, oldVal) {
                    if ($scope.source && newVal != oldVal) {
                        var Size1 = isNaN(parseFloat($scope.source.Size1)) ? 0 : parseFloat($scope.source.Size1);
                        var Size2 = isNaN(parseFloat($scope.source.Size2)) ? 0 : parseFloat($scope.source.Size2);
                        var Size3 = isNaN(parseFloat($scope.source.Size3)) ? 0 : parseFloat($scope.source.Size3);
                        var Size4 = isNaN(parseFloat($scope.source.Size4)) ? 0 : parseFloat($scope.source.Size4);
                        var Size5 = isNaN(parseFloat($scope.source.Size5)) ? 0 : parseFloat($scope.source.Size5);
                        $scope.source.TotalArea = Size1 + Size2 + Size3 + Size4 + Size5;
                        $scope.getSeatingRatio();
                    }
                });
                $scope.$watch('source.Seats1+source.Seats2+source.Seats3+source.Seats4+source.Seats5', function (newVal, oldVal) {
                    if ($scope.source && newVal != oldVal) {
                        var Seats1 = isNaN(parseFloat($scope.source.Seats1)) ? 0 : parseFloat($scope.source.Seats1);
                        var Seats2 = isNaN(parseFloat($scope.source.Seats2)) ? 0 : parseFloat($scope.source.Seats2);
                        var Seats3 = isNaN(parseFloat($scope.source.Seats3)) ? 0 : parseFloat($scope.source.Seats3);
                        var Seats4 = isNaN(parseFloat($scope.source.Seats4)) ? 0 : parseFloat($scope.source.Seats4);
                        var Seats5 = isNaN(parseFloat($scope.source.Seats5)) ? 0 : parseFloat($scope.source.Seats5);
                        $scope.source.TotalSeatsNo = Seats1 + Seats2 + Seats3 + Seats4 + Seats5;
                        $scope.getSeatingRatio();
                    }
                });
                $scope.$watch('source.WaitingArea+source.SeatArea+source.PlayPlaceArea+source.ToiletArea+source.StairArea+source.OtherArea', function (newVal, oldVal) {
                    if ($scope.source && newVal != oldVal) {
                        var WaitingArea = isNaN(parseFloat($scope.source.WaitingArea)) ? 0 : parseFloat($scope.source.WaitingArea);
                        var SeatArea = isNaN(parseFloat($scope.source.SeatArea)) ? 0 : parseFloat($scope.source.SeatArea);
                        var PlayPlaceArea = isNaN(parseFloat($scope.source.PlayPlaceArea)) ? 0 : parseFloat($scope.source.PlayPlaceArea);
                        var ToiletArea = isNaN(parseFloat($scope.source.ToiletArea)) ? 0 : parseFloat($scope.source.ToiletArea);
                        var StairArea = isNaN(parseFloat($scope.source.StairArea)) ? 0 : parseFloat($scope.source.StairArea);
                        var OtherArea = isNaN(parseFloat($scope.source.OtherArea)) ? 0 : parseFloat($scope.source.OtherArea);
                        $scope.source.DiningArea = WaitingArea + SeatArea + PlayPlaceArea + ToiletArea + StairArea + OtherArea;
                    }
                });
                $scope.$watch('source.ServiceArea+source.ProducingArea+source.BackArea+source.ColdStorageArea+source.DryArea+source.StaffroomArea', function (newVal, oldVal) {
                    if ($scope.source && newVal != oldVal) {
                        var ServiceArea = isNaN(parseFloat($scope.source.ServiceArea)) ? 0 : parseFloat($scope.source.ServiceArea);
                        var ProducingArea = isNaN(parseFloat($scope.source.ProducingArea)) ? 0 : parseFloat($scope.source.ProducingArea);
                        var BackArea = isNaN(parseFloat($scope.source.BackArea)) ? 0 : parseFloat($scope.source.BackArea);
                        var ColdStorageArea = isNaN(parseFloat($scope.source.ColdStorageArea)) ? 0 : parseFloat($scope.source.ColdStorageArea);
                        var DryArea = isNaN(parseFloat($scope.source.DryArea)) ? 0 : parseFloat($scope.source.DryArea);
                        var StaffroomArea = isNaN(parseFloat($scope.source.StaffroomArea)) ? 0 : parseFloat($scope.source.StaffroomArea);
                        $scope.source.KitchenArea = ServiceArea + ProducingArea + BackArea + ColdStorageArea + DryArea + StaffroomArea;
                    }
                });
                $scope.editablePoleSign = $scope.editable;
                $scope.$watch("source.PoleSign", function (val) {
                    if ($scope.source && val != null) {
                        if ($scope.editable) {
                            if (val == "1") {
                                $scope.editablePoleSign = true;
                            } else {
                                $scope.editablePoleSign = false;
                            }
                        }
                    }
                });
                $scope.editableSignage = false;
                $scope.$watch("source.Signage", function (val) {
                    if ($scope.source && val != null) {
                        if ($scope.editable) {
                            if (val == "1") {
                                $scope.editableSignage = true;
                            } else {
                                $scope.editableSignage = false;
                            }
                        }
                    }
                });
            }
        };
    }
]).directive("projectHistory", [
    "$http", function ($http) {
        return {
            restrict: "EA",
            scope: {
                pageUrl: "@",
                projectId: "=",
                tableName: "@",
                title: "@",
                hasTemplate: "=?",
                titleName: "@"
            },
            replace: true,
            templateUrl: Utils.ServiceURI.AppUri + "Module/ProjectHistory",
            link: function ($scope, ele, attrs, ngModelCtrls) {
                if (!$scope.hasTemplate) {
                    $scope.hasTemplate = false;
                }
                $scope.$watch("projectId", function (val) {
                    if (!!val) {
                        $http.get(Utils.ServiceURI.Address() + "api/project/GetHistory/" + val + "/" + $scope.tableName + "/" + $scope.hasTemplate).success(function (historyData) {
                            if (historyData != "") {
                                $scope.historyList = historyData;
                                var fistSplitChar;
                                if ($scope.pageUrl.indexOf('?') >= 0) {
                                    fistSplitChar = '&';
                                } else {
                                    fistSplitChar = '?';
                                }

                                angular.forEach($scope.historyList, function (history, i) {
                                    history.Url = $scope.pageUrl + fistSplitChar + "entityId=" + $scope.historyList[i].Id + "&isHistory=true";
                                });
                            }
                        });
                    }
                    ;
                });
            }
        };
    }
]).directive("attachmentsMemo", [
    "$http", function ($http) {
        return {
            restrict: "EA",
            scope: {
                projectId: "=",
                flowCode: "@"
            },
            replace: true,
            templateUrl: Utils.ServiceURI.AppUri + "Module/AttachmentsMemo",
            link: function ($scope, ele, attrs, ngModelCtrls) {
                $http.get(Utils.ServiceURI.Address() + "api/AttachmentsMemo/GetMemoList", {
                    cache: false,
                    params: {
                        flowCode: $scope.flowCode,
                        projectId: $scope.projectId
                    }
                }).success(function (data) {
                    if (data != null && data != "") {
                        $scope.MemoList = data;
                    }
                });
            }
        };
    }
]).directive("notificationMessage", [
    "$http",
    "$window",
    "$selectUser", "messager", function ($http, $window, $selectUser, messager) {
        return {
            restrict: "EA",
            scope: {
                projectId: "=",
                flowCode: "@",
                subFlowCode: "@",
                uscode: "=",
                nodeId: "=",
                isHistory: '=?'
            },
            replace: true,
            templateUrl: Utils.ServiceURI.AppUri + "Module/NotificationMessage",
            link: function ($scope, ele, attrs, ngModelCtrls) {
                $scope.notifyUsersInfo = {
                    NoticeUsers: []
                };

                $scope.IsSendMail = false;
                $scope.selectEmployee = function () {
                    var users = $.map($scope.notifyUsersInfo.NoticeUsers, function (u, i) {
                        return {
                            Code: u.UserAccount,
                            NameZHCN: u.UserNameZHCN,
                            NameENUS: u.UsernameENUS
                        };
                    });
                    $selectUser.open({
                        checkUsers: function (selectedUsers) {
                            return true;
                        },
                        selectedUsers: angular.copy(users || []),
                        OnUserSelected: function (selectedUsers) {
                            $scope.noticeUsersName = $.map(selectedUsers, function (u, i) {
                                return u.NameENUS;
                            }).join(",");
                            $scope.notifyUsersInfo.NoticeUsers = $.map(selectedUsers, function (u, i) {
                                return {
                                    UserAccount: u.Code,
                                    UserNameZHCN: u.NameZHCN,
                                    UsernameENUS: u.NameENUS
                                };
                            });
                        }
                    });
                };
                $scope.Notification = {};
                $scope.sendMsg = function () {
                    if ($scope.MessageContent == null || $scope.MessageContent == "") {
                        return;
                    }
                    if ($scope.nodeId == null || $scope.nodeId == "" || $scope.nodeId == "00000000-0000-0000-0000-000000000000") {
                        messager.showMessage("请先保存页面数据", "fa-warning c_orange");
                        return;
                    }

                    var receiverCodeList = [];
                    angular.forEach($scope.notifyUsersInfo.NoticeUsers, function (o, i) {
                        receiverCodeList.push(o.UserAccount);
                    });
                    if ($scope.SubFlow.Code == null || $scope.SubFlow.Code == "") {
                        return;
                    }
                    if (!$scope.IsSendMail) {
                        receiverCodeList = [];
                        receiverCodeList.push($scope.SubFlow.Code);
                    }
                    $scope.IsSend = true;
                    $http.post(Utils.ServiceURI.Address() + "api/NotificationMessage/SendMsg", {
                        Message: $scope.MessageContent,
                        FlowCode: $scope.SubFlow.Code,
                        ProjectId: $scope.projectId,
                        SenderCode: window["currentUser"].Code,
                        IsSendEmail: $scope.IsSendMail,
                        ReceiverCodeList: receiverCodeList,
                        UsCode: $scope.uscode,
                        RefId: $scope.nodeId,
                        Title: window["currentUser"].NameENUS + "在" + " " + $scope.uscode + " " + $scope.projectId + " " + $scope.SubFlow.NameENUS + "给您留言了"
                    }).success(function (data) {
                        messager.showMessage("[[[发送成功]]]", "fa-check c_green");
                        $scope.MessageContent = "";
                        $scope.notifyUsersInfo.NoticeUsers = [];
                        $scope.noticeUsersName = "";
                        loadMessageList();
                        $scope.IsSend = false;
                    }).error(function (error) {
                        messager.showMessage("发送失败，请重试", "fa-warning c_orange");
                        $scope.IsSend = false;
                    });
                };
                $scope.Others = true;
                $scope.MyComments = true;
                $scope.ApprovalRecords = true;
                $scope.MessageList = [];
                var MessageListFilter = function (MessageData) {
                    if (MessageData == null)
                        return;
                    $scope.MessageList = [];
                    if ($scope.MyComments) {
                        if (MessageData.NotificationList != null && MessageData.NotificationList.length > 0) {
                            angular.forEach(MessageData.NotificationList, function (item, i) {
                                if (item != null && (item.ReceiverENUS != null && item.ReceiverENUS != "")) {
                                    $scope.MessageList.push(item);
                                }
                            });
                        }
                    }
                    if ($scope.ApprovalRecords) {
                        if (MessageData.ApprovalRecords != null && MessageData.ApprovalRecords.length > 0) {
                            angular.forEach(MessageData.ApprovalRecords, function (item, i) {
                                $scope.MessageList.push(item);
                            });
                        }
                    }
                    if ($scope.Others) {
                        if (MessageData.NotificationList != null && MessageData.NotificationList.length > 0) {
                            angular.forEach(MessageData.NotificationList, function (item, i) {
                                if (item != null && (item.ReceiverENUS == null || item.ReceiverENUS == "")) {
                                    $scope.MessageList.push(item);
                                }
                            });
                        }
                    }
                };
                $scope.$watch("Others + MyComments + ApprovalRecords", function () {
                    if ($scope.MessageData != null) {
                        MessageListFilter($scope.MessageData);
                    }
                });
                $scope.gotoAllMsg = function () {
                    $window.location.href = Utils.ServiceURI.WebAddress() + "Home/Main#/CommentsList?projectId=" + $scope.projectId + "&flowCode=" + $scope.flowCode + "&subFlowCode=" + $scope.subFlowCode;
                };
                var loadFlowCodeList = function () {
                    $http.get(Utils.ServiceURI.Address() + "api/NotificationMessage/GetFlowCodeList?parentCode=" + $scope.flowCode).success(function (listFlowCodes) {
                        $scope.FlowCodes = listFlowCodes;
                        angular.forEach(listFlowCodes, function (o, i) {
                            if (o.Code == $scope.subFlowCode) {
                                $scope.SubFlow = o;
                                return true;
                            }
                        });
                    });
                };

                var loadMessageList = function () {
                    $http.get(Utils.ServiceURI.Address() + "api/NotificationMessage/GetMessageList/" + window["currentUser"].Code + "/" + $scope.projectId + "/" + $scope.subFlowCode).success(function (MessageList) {
                        $scope.MessageData = MessageList;
                        MessageListFilter($scope.MessageData);
                    });
                };
                var load = function () {
                    loadFlowCodeList();
                    loadMessageList();
                };
                load();
            }
        };
    }]).directive("fileDownloader", [
    "$timeout",
    "messager",
    function ($timeout, messager) {
        return {
            restrict: "A",
            link: function ($scope, element, attrs) {
                var timer;
                var beginDownload = function () {
                    var $iframe = $("#frmFileDownloader");
                    if ($iframe.length == 0) {
                        $iframe = $("<iframe id='frmFileDownloader' class='hide' />").appendTo(document.body);
                        $iframe.get(0).onreadystatechange = function () {
                            messager.unBlockUI();
                        };
                    }
                    if (!!document.onreadystatechange) {
                        messager.blockUI("正在处理，请稍等...");
                    }
                    timer = $timeout(function () {
                        $iframe.prop("src", attrs.fileLink);
                    }, 1000);
                };
                element.click(function (e) {
                    if (!!$scope[attrs.beforeDownload] && $scope[attrs.beforeDownload]()) {
                        beginDownload();
                    } else {
                        beginDownload();
                    }
                });
                $scope.$on("$destroy", function (event) {
                    $timeout.cancel(timer);
                });
            }
        };
    }]).directive("packDownloader", [
    "$http", "messager", function ($http, messager) {
        return {
            restrict: "A",
            scope: {
                refTableName: "@",
                projectId: "@",
                beforeDownload: "&"
            },
            replace: false,
            link: function ($scope, ele, attrs) {
                var $frm = $("#frmPackDownload");
                if ($frm.length == 0) {
                    $frm = $("<iframe id='frmPackDownload' src='about:blank' class='hide'></iframe>").appendTo(document.body);
                }
                ele.click(function () {
                    $scope.beforeDownload();
                    messager.blockUI("正在处理中，请稍后...");
                    $http.get(Utils.ServiceURI.Address() + "api/attachment/preparePackDownload", {
                        params: {
                            refTableName: $scope.refTableName,
                            projectId: $scope.projectId
                        }
                    }).then(function (response) {
                        $frm.prop("src", Utils.ServiceURI.Address() + "api/attachment/packDownload?packUrl=" + response.data.PackUrl + "&projectId=" + $scope.projectId);
                        messager.unBlockUI();
                    }, function () {
                        messager.unBlockUI();
                        messager.showMessage("打包下载失败，请稍后重试...", "fa-warning c_orange");
                    });
                });
            }
        };
    }]).directive("reopenMemo", [
    "$http", "messager", function ($http, messager) {
        return {
            restrict: "EA",
            scope: {
                projectId: "@",
                source: "=?",
                editable: "="
            },
            replace: true,
            templateUrl: Utils.ServiceURI.AppUri + "Module/ReopenMemo",
            link: function ($scope, element, attrs) {
                var loadReopenMemo = function (type) {
                    $http.get(Utils.ServiceURI.Address() + "api/ReopenMemo/GetReopenMemo/" + $scope.projectId).success(function (data) {
                        if (data != null && data != "") {
                            if (type == "UpImg" || type == "DelImg") {
                                $scope.memo = data;
                                $scope.ReopenMemo.Id = $scope.memo.Id;
                                $scope.ReopenMemo.ExteriorAfterImgURL1 = $scope.memo.ExteriorAfterImgURL1;
                                $scope.ReopenMemo.ExteriorAfterImg1 = $scope.memo.ExteriorAfterImg1;

                                $scope.ReopenMemo.ExteriorAfterImgURL2 = $scope.memo.ExteriorAfterImgURL2;
                                $scope.ReopenMemo.ExteriorAfterImg2 = $scope.memo.ExteriorAfterImg2;

                                $scope.ReopenMemo.InteriorAfterImgURL1 = $scope.memo.InteriorAfterImgURL1;
                                $scope.ReopenMemo.InteriorAfterImg1 = $scope.memo.InteriorAfterImg1;

                                $scope.ReopenMemo.InteriorAfterImgURL2 = $scope.memo.InteriorAfterImgURL2;
                                $scope.ReopenMemo.InteriorAfterImg2 = $scope.memo.InteriorAfterImg2;
                                $scope.source = $scope.ReopenMemo;
                            } else {
                                $scope.ReopenMemo = data;
                                $scope.source = $scope.ReopenMemo;
                            }
                        }
                    }).error(function (error) {
                        messager.showMessage(error);
                    });
                };
                $scope.uploadFinished = function () {
                    messager.showMessage("[[[上传成功]]]", "fa-warning c_orange");
                    loadReopenMemo("UpImg");
                };
                $scope.DeleteImg = function (typeCode) {
                    $scope.IsClickDelete = true;
                    $http.post(Utils.ServiceURI.Address() + "api/ReopenMemo/DeleteImg/" + typeCode + "/" + $scope.projectId, {}).success(function (data) {
                        loadReopenMemo("DelImg");
                        $scope.IsClickDelete = false;
                    });
                };
                loadReopenMemo("");
                $scope.open = function ($event, ele) {
                    $event.preventDefault();
                    $event.stopPropagation();

                    $scope[ele] = true;
                };

                $scope.dateOptions = {
                    formatYear: 'yy',
                    startingDay: 0
                };

                $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
                $scope.format = $scope.formats[1];
            }
        };
    }]).directive("gbMemo", [
    "$http", "messager", function ($http, messager) {
        return {
            restrict: "EA",
            scope: {
                projectId: "@",
                source: "=?",
                editable: "="
            },
            replace: true,
            templateUrl: Utils.ServiceURI.AppUri + "Module/GBMemo",
            link: function ($scope, element, attrs) {
                $scope.$watch("source", function (val) {
                    if (val != null) {
                        $scope.GBMemo = $scope.source;
                    }
                });
                $scope.StoreReimageStatus = function (action) {
                    if (action == "IsClosed") {
                        $scope.GBMemo.IsClosed = true;
                        $scope.GBMemo.IsInOperation = false;
                    }
                    if (action == "IsInOperation") {
                        $scope.GBMemo.IsClosed = false;
                        $scope.GBMemo.IsInOperation = true;
                    }
                };
                $scope.open = function ($event, ele) {
                    $event.preventDefault();
                    $event.stopPropagation();

                    $scope[ele] = true;
                };

                $scope.dateOptions = {
                    formatYear: 'yy',
                    startingDay: 0
                };

                $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
                $scope.format = $scope.formats[1];
            }
        };
    }]).directive("tempClosureMemo", [
    "$http", "messager", function ($http, messager) {
        return {
            restrict: "EA",
            scope: {
                projectId: "@",
                source: "=?",
                editable: "="
            },
            replace: true,
            templateUrl: Utils.ServiceURI.AppUri + "Module/TempClosureMemo",
            link: function ($scope, element, attrs) {
                $scope.now = new Date();
                $scope.$watch("projectId", function (val) {
                    if (val != null && val != "") {
                        if (val.toLocaleLowerCase().indexOf("reimage") != -1) {
                            $scope.flowCode = "Reimage";
                        } else if (val.toLocaleLowerCase().indexOf("rebuild") != -1) {
                            $scope.flowCode = "Rebuild";
                        } else if (val.toLocaleLowerCase().indexOf("majorlease") != -1) {
                            $scope.flowCode = "MajorLease";
                        } else if (val.toLocaleLowerCase().indexOf("renewal") != -1) {
                            $scope.flowCode = "Renewal";
                        }
                    }
                });
                var loadTempClosureMemo = function () {
                    $http.get(Utils.ServiceURI.Address() + "api/tempClosureMemo/get?projectId=" + $scope.projectId).success(function (data) {
                        if (data != null && data != "") {
                            $scope.entity = data;
                            $scope.entity = $scope.entity.Entity;
                            $scope.source = $scope.entity;
                            if ($scope.entity.ClosureDate != null && $scope.entity.ClosureDate != "") {
                                $scope.source.ClosureDate = moment($scope.entity.ClosureDate).toDate();
                            }
                        }
                    }).error(function (error) {
                        messager.showMessage(error);
                    });
                };
                loadTempClosureMemo();

                $scope.open = function ($event, ele) {
                    $event.preventDefault();
                    $event.stopPropagation();

                    $scope[ele] = true;
                };

                $scope.dateOptions = {
                    formatYear: 'yy',
                    startingDay: 0
                };

                $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
                $scope.format = $scope.formats[1];
            }
        };
    }]).directive("leaseChange", [
    "$http", "messager", function ($http, messager) {
        return {
            restrict: "EA",
            scope: {
                projectId: "@",
                source: "=?",
                title: "@",
                isEdit: "="
            },
            replace: true,
            templateUrl: Utils.ServiceURI.AppUri + "Module/LeaseChangeTemp",
            link: function ($scope, element, attrs) {
                $scope.open = function ($event, ele) {
                    $event.preventDefault();
                    $event.stopPropagation();

                    $scope[ele] = true;
                };

                $scope.dateOptions = {
                    formatYear: 'yy',
                    startingDay: 0
                };

                $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
                $scope.format = $scope.formats[1];
            }
        };
    }]).factory('projectStatusChangeService', [
    '$modal',
    function ($modal) {
        return {
            open: function (projectId, flowCode, status, usCode) {
                return $modal.open({
                    backdrop: 'static',
                    templateUrl: "/Module/ProjectStatusDialog",
                    size: 'lg',
                    resolve: {
                        model: function () {
                            return angular.copy({
                                SelectedProjectStatus: status,
                                Comment: null
                            });
                        }
                    },
                    controller: [
                        "$scope", "$modalInstance", "messager", 'projectService', 'storeService', 'model', function ($scope, $modalInstance, messager, projectService, storeService, model) {
                            $scope.model = model;
                            $scope.projectStatusOpts = [
                                { name: 'Active', value: 0 },
                                { name: 'Killed', value: 7 },
                                { name: 'Pending', value: 3 },
                                { name: 'Rejected', value: 4 },
                                { name: 'Completed', value: 5 },
                                { name: 'Recall', value: 6 }
                            ];

                            projectService.getProjectStatusChangeLog({ projectId: projectId }).$promise.then(function (result) {
                                $scope.changelogs = result.changeLog;
                            });

                            $scope.loadProjectStatusOpts = (function () {
                                switch (status) {
                                    case 0:
                                    case 7:
                                    case 3:
                                        $scope.projectStatusOpts = [
                                            { name: 'Active', value: 0 },
                                            { name: 'Pending', value: 3 },
                                            { name: 'Killed', value: 7 }
                                        ];
                                        break;
                                    default:
                                }
                            })();

                            $scope.changeStatus = function () {
                                if (!model.Comment) {
                                    messager.showMessage('请先填写意见!', "fa-warning c_orange");
                                    return false;
                                }

                                projectService.changeProjectStatus({
                                    ProjectId: projectId,
                                    FlowCode: flowCode,
                                    PrevStatus: status,
                                    CurrStatus: $scope.model.SelectedProjectStatus,
                                    Comment: $scope.model.Comment
                                }).$promise.then(function (response) {
                                    $modalInstance.close(model);
                                });

                                return true;
                            };
                            $scope.ok = function () {
                                if (status == 0 || status == 3) {
                                    $scope.changeStatus();
                                } else {
                                    storeService.checkStoreFlow({
                                        code: usCode,
                                        flowCode: flowCode
                                    }).$promise.then(function (result) {
                                        if (!result.StoreValid) {
                                            messager.showMessage("'" + result.Store.NameZHCN + "'当前已有正在进行中的'" + flowCode + "'流程，请重新选择。", "fa-warning c_red");
                                            return false;
                                        } else {
                                            return $scope.changeStatus();
                                        }
                                    });
                                }
                            };
                            $scope.cancel = function () {
                                $modalInstance.dismiss("cancel");
                            };
                        }
                    ]
                }).result;
            }
        };
    }]).factory("redirectService", [
    "$location",
    "$window",
    function ($location, $window) {
        return {
            flowRedirect: function (flowCode, projectId) {
                var route = "/" + flowCode.split("_")[1] + "/Process/View";
                if (flowCode.split("_")[0] == "Rebuild" && flowCode.split("_")[1] == "Package")
                    route = "/RebuildPackage/Process/View";
                $location.path(route).search({ projectId: projectId, from: "useraction" }).replace();
            }
        };
    }
]).directive('approvedPackage', [
    "$modal",
    "$http",
    "messager",
    function ($modal, $http, messager) {
        return {
            restrict: "E",
            scope: {
                entityId: "=?",
                flowCode: "@",
                usCode: "=?",
                showContract: "@"
            },
            replace: true,
            templateUrl: Utils.ServiceURI.AppUri + "Module/ApprovedPackage",
            link: function ($scope, element, attrs) {
                var load = function () {
                    var url = Utils.ServiceURI.Address() + "api/DL/Attachments/Get";
                    $http.get(url, {
                        cache: false,
                        params: {
                            Id: $scope.entityId,
                            flowCode: $scope.flowCode
                        }
                    }).success(function (data) {
                        $scope.approvedPackage = null;
                        $scope.contract = null;
                        angular.forEach(data, function (r, i) {
                            switch (r.Extension.toLowerCase()) {
                                case ".xlsx":
                                case ".xls":
                                    r.Icon = "fa fa-file-excel-o c_green";
                                    break;
                                case ".ppt":
                                    r.Icon = "fa fa-file-powerpoint-o c_red";
                                    break;
                                case ".doc":
                                case ".docx":
                                    r.Icon = "fa fa-file-word-o c_blue";
                                    break;
                                default:
                                    r.Icon = "fa fa-file c_orange";
                                    break;
                            }
                            if (r.TypeCode == "ApprovedPackage")
                                $scope.approvedPackage = r;
                            else if (r.TypeCode == "Contract")
                                $scope.contract = r;
                        });
                    });
                };
                $scope.uploadAttachFinished = function (up, files) {
                    messager.showMessage("上传文件成功", "fa-check c_green").then(function () {
                        load();
                    });
                };
                $scope.deleteAttachment = function (id) {
                    messager.confirm("[[[确认要删除？]]]", "fa-warning c_orange").then(function (result) {
                        if (result) {
                            $http.post(Utils.ServiceURI.Address() + "api/DL/Attachments/Delete/" + id).success(function (data) {
                                messager.showMessage("删除附件成功", "fa-check c_green");
                                load();
                            }).error(function (data) {
                                messager.showMessage("删除附件失败", "fa-warning c_red");
                            });
                        }
                    });
                };

                $scope.$watch("entityId", function (val) {
                    load();
                });
            }
        };
    }]);
