/// <reference path="../../Libs/moment/moment.d.ts" />
/// <reference path="../../Libs/Angular/angular.d.ts" />
/// <reference path="../../Libs/Angular/Plugins/angular-animate.d.ts" />
/// <reference path="../../Libs/Angular/Plugins/angular-cookies.d.ts" />
/// <reference path="../../Libs/Angular/Plugins/angular-mocks.d.ts" />
/// <reference path="../../Libs/Angular/Plugins/angular-resource.d.ts" />
/// <reference path="../../Libs/Angular/Plugins/angular-route.d.ts" />
/// <reference path="../../Libs/Angular/Plugins/angular-sanitize.d.ts" />
/// <reference path="../../Libs/Angular/Plugins/angular-scenario.d.ts" />
/// <reference path="../../Libs/Angular/Plugins/angular-ui-bootstrap.d.ts" />
/// <reference path="../../Libs/JQuery/jquery.d.ts" />
/// <reference path="../../Libs/AjaxQueue.d.ts" />
/// <reference path="../../Utils/Utils.ts" />
/// <reference path="../../Utils/CurrentUser.ts" />
/// <reference path="../../Services/Notices/mcd.am.notices.service.ts" />
!function () {
    var ctrlModule = angular.module("mcd.am.notices.controller", [
        "mcd.am.notices.service"
    ]);

    ctrlModule.controller('amNoticesListCtrl', [
        '$scope',
        "$http",
        "$modal",
        "$window",
        "$selectStore",
        "amNoticesService",
        function ($scope, $http, $modal, $window, $selectStore, amNoticesService) {
            $scope.pageIndex = 1;
            $scope.pageSize = 10;
            $scope.totalItems = 0;
            $scope.list = [];
            $scope.searchCondition = new noticeSearchCondition();
            $scope.conditionTable = [];
            $scope.isShowSearchValues = false;
            $scope.isLoading = true;
            $scope.pagging = function () {
                $scope.conditionTable = [];
                $scope.ajaxFinished = false;
                for (var prop in $scope.searchCondition) {
                    var value = $scope.searchCondition[prop];
                    if (prop.indexOf("_") != 0 && !!value) {
                        if (typeof value == "object" && value instanceof Date) {
                            value = moment(value).format("YYYY-MM-DD");
                        }
                        if (typeof value == "object" && value instanceof Array) {
                            value = value.join(" , ");
                        }
                        $scope.conditionTable.push({
                            key: prop,
                            value: value
                        });
                    }
                }
                if ($scope.conditionTable.length > 0) {
                    $scope.isShowSearchValues = true;
                } else {
                    $scope.isShowSearchValues = false;
                }

                amNoticesService.getItemsList($scope.pageSize, $scope.pageIndex, $scope.searchCondition).then(function (response) {
                    $scope.noticeList = response.data.List;
                    $scope.totalItems = response.data.TotalItems;
                    $scope.loadNoticeFinished = true;
                    $scope.$apply();
                });
                /*
                var url = Utils.ServiceURI.Address() + "api/project/" + $scope.pageIndex + "/" + $scope.pageSize;
                $.post(url, $scope.searchCondition).success(function (data) {
                $scope.list = data.List;
                $scope.totalItems = data.TotalItems;
                $scope.ajaxFinished = true;
                $scope.$apply();
                });
                */
            };

            $scope.$watch("pageIndex", function (val) {
                if (!!val) {
                    $scope.pagging();
                }
                ;
            });
            $scope.search = function () {
                $scope.pageIndex = 1;
                $scope.pagging();
            };
            $scope.removeSearchCondition = function (key) {
                $scope.searchCondition[key] = null;
                angular.forEach($scope.conditionTable, function (cdtItem, i) {
                    if (cdtItem.key == key) {
                        $scope.conditionTable.splice(i, 1);
                    }
                });
                if ($scope.conditionTable.length > 0) {
                    $scope.isShowSearchValues = true;
                } else {
                    $scope.isShowSearchValues = false;
                }
            };

            $scope.loadOperators = function (node, dataItem) {
                if (!node.operatorLoaded) {
                    /*
                    amNoticesService.getOperators(node.FlowCode, dataItem.ItemID).then(function (response) {
                    var operators = response.data;
                    var OperatorHTML = "";
                    if (!!operators && operators.length > 0) {
                    OperatorHTML += "<ul class='node-operators'>";
                    angular.forEach(operators, function (o, i) {
                    OperatorHTML += "<li>" + o.NameZHCN + "</li>";
                    });
                    OperatorHTML += "</ul>";
                    } else {
                    OperatorHTML = "暂无处理人";
                    }
                    node.operatorHTML = OperatorHTML;
                    node.operatorLoaded = true;
                    });*/
                }
            };

            $scope.searchStore = function (inputCode) {
                return $http.get(Utils.ServiceURI.ApiDelegate, {
                    cache: false,
                    params: {
                        url: Utils.ServiceURI.FrameAddress() + "api/store/user/5/" + window["currentUser"].Code,
                        code: inputCode
                    }
                }).then(function (response) {
                    return response.data;
                });
            };

            //初始化日期控件
            $scope.today = function () {
                $scope.dt = new Date();
            };
            $scope.today();

            $scope.open = function ($event, dateTag) {
                $event.preventDefault();
                $event.stopPropagation();

                $scope[dateTag] = true;
                //$scope.opened = true;
            };

            $scope.dateOptions = {
                formatYear: 'yy',
                startingDay: 1
            };

            $scope.format = 'yyyy/MM/dd';

            // end date
            $scope.showAdvanceSearch = function () {
                $modal.open({
                    size: "lg",
                    backdrop: 'static',
                    resolve: {
                        condition: function () {
                            return angular.copy($scope.searchCondition);
                        }
                    },
                    templateUrl: Utils.ServiceURI.WebAddress() + "Template/ProjectAdvanceSearch",
                    controller: function ($scope, $modalInstance, condition) {
                        $scope.condition = condition;

                        $scope.seachOrg = function (level, name) {
                            return $http.get(Utils.ServiceURI.ApiDelegate, {
                                cache: false,
                                params: {
                                    url: Utils.ServiceURI.FrameAddress() + "api/org/5",
                                    type: level,
                                    name: name,
                                    status: 1
                                }
                            }).then(function (response) {
                                return $.map(response.data, function (d, i) {
                                    return d.NameZHCN;
                                });
                            });
                        };

                        $scope.searchEmployee = function (name) {
                            return $http.get(Utils.ServiceURI.ApiDelegate, {
                                cache: false,
                                params: {
                                    url: Utils.ServiceURI.FrameAddress() + "api/user/5",
                                    name: name
                                }
                            }).then(function (response) {
                                return response.data;
                            });
                        };

                        $scope.selectStore = function () {
                            $selectStore.open({
                                selectedStores: !!$scope.condition.USCode ? $.map($scope.condition.USCode.split(","), function (c, i) {
                                    return {
                                        Code: c
                                    };
                                }) : [],
                                userCode: window["currentUser"] ? window["currentUser"].Code : null,
                                OnStoreSelected: function (stores) {
                                    $scope.condition.USCode = $.map(stores, function (s, i) {
                                        return s.Code;
                                    }).join(",");
                                }
                            });
                        };

                        $scope.searchStore = function (name) {
                            return $http.get(Utils.ServiceURI.ApiDelegate, {
                                cache: false,
                                params: {
                                    url: Utils.ServiceURI.FrameAddress() + "api/store/fuzzy/5",
                                    name: name
                                }
                            }).then(function (response) {
                                return response.data;
                            });
                        };

                        $scope.openDatepicker = function ($event, dateOpenTag) {
                            $event.preventDefault();
                            $event.stopPropagation();
                            $scope.condition[dateOpenTag] = true;
                        };

                        $scope.cancel = function () {
                            $modalInstance.dismiss("cancel");
                        };

                        $scope.ok = function () {
                            $modalInstance.close($scope.condition);
                        };
                    }
                }).result.then(function (condition) {
                    $scope.searchCondition = angular.copy(condition);
                    $scope.search();
                });
            };
        }]).controller("amNoticesDetailCtrl", [
        "$scope",
        "$http",
        "$routeParams",
        "amNoticesService",
        function ($scope, $http, $routeParams, amNoticesService) {
            $scope.noticeId = $routeParams.noticeId;

            amNoticesService.getItemDetail($scope.noticeId).then(function (response) {
                $scope.detail = response.data;
                $scope.$apply();
            });
        }
    ]);

    function noticeSearchCondition() {
        this.ProcessId = "";
        this.Title = "";
        this.SenderName = "";

        this.Receiver = "";
        this.StoreCode = "";

        this.DateFrom = null;
        this._IsOpenDateFrom = false;
        this.DateTo = null;
        this._IsOpenDateTo = false;
    }
}();
