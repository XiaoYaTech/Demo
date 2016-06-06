/// <reference path="../Libs/moment/moment.js" />
/// <reference path="../Utils/Utils.js" />
!function () {
    angular.module("project.commentslist", [
        "mcd.am.service.taskwork",
        "mcd.am.service.commentsList",
        "mcd.am.services.projectUsers",
        "mcd.am.services.flow",
        "mcd.am.services.project"
    ]).controller('commentsListCtrl', [
        '$scope',
        "$http",
        "$modal",
        "$window",
        "$selectStore",
        "$routeParams",
        "flowService",
        "commentsListService",
        "messager",
        function ($scope, $http, $modal, $window, $selectStore, $routeParams, flowService, commentsListService, messager) {
            $scope.projectId = $routeParams.projectId;
            $scope.flowCode = $routeParams.flowCode;
            $scope.subFlowCode = $routeParams.subFlowCode;
            
            $scope.pageIndex = 1;
            $scope.projectPageIndex = 1;
            $scope.pageSize = 10;
            $scope.totalItems = 0;
            $scope.projectTotalItems = 0;
            $scope.list = [];
            $scope.projectComentsList = [];
            $scope.Sender = {};
            $scope.Flow = {};
            $scope.searchCondition = {
                ProjectId: $scope.projectId,
                FlowCode: "",//this is Sub Flow Code
                SenderZHCN:"",
                SenderCode: "",
                Title: "",
                CreateDate: "",
                EndDate:"",
                PageIndex: $scope.pageIndex,
                PageSize: $scope.pageSize
            };
            $scope.pagging = function() {
                $scope.conditionTable = [];
                $scope.ajaxFinished = false;

                var subFlowCode = "";
                if ($scope.Flow == null)
                    subFlowCode = "";
                else if ($scope.Flow.Code == null)
                    subFlowCode = $routeParams.subFlowCode;
                else
                    subFlowCode = $scope.Flow.Code;

                var senderCode = ($scope.Sender == null || $scope.Sender.Code == null) ? "" : $scope.Sender.Code;

                $scope.searchCondition.FlowCode = subFlowCode;
                $scope.searchCondition.SenderCode = senderCode;
                $scope.searchCondition.PageIndex = $scope.pageIndex;

                commentsListService.queryCommentsList($scope.searchCondition).$promise.then(function (data) {
                    if (data != null) {
                        $scope.list = data.data;
                        $scope.totalItems = data.totalSize;
                    }
                    $scope.ajaxFinished = true;
                }, function(error) {
                    messager.showMessage(error.statusText, "fa-warning c_orange");
                });
            };
            $scope.projectPagging = function () {
                $scope.conditionTable = [];
                $scope.ajaxProjectFinished = false;

                var subFlowCode = "";
                if ($scope.Flow == null)
                    subFlowCode = "";
                else if ($scope.Flow.Code == null)
                    subFlowCode = $routeParams.subFlowCode;
                else
                    subFlowCode = $scope.Flow.Code;

                var senderCode = ($scope.Sender == null || $scope.Sender.Code == null) ? "" : $scope.Sender.Code;

                $scope.searchCondition.FlowCode = subFlowCode;
                $scope.searchCondition.SenderCode = senderCode;
                $scope.searchCondition.PageIndex = $scope.projectPageIndex;

                commentsListService.queryProjectCommentsList($scope.searchCondition).$promise.then(function (data) {
                    if (data != null) {
                        $scope.projectComentsList = data.data;
                        $scope.projectTotalItems = data.totalSize;
                    }
                    $scope.ajaxProjectFinished = true;
                }, function (error) {
                    messager.showMessage(error.statusText, "fa-warning c_orange");
                });
            };
            $scope.$watch("pageIndex", function(val) {
                if (!!val) {
                    $scope.pagging();
                };
            });
            $scope.$watch("projectPageIndex", function (val) {
                if (!!val) {
                    $scope.projectPagging();
                };
            });
            $scope.search = function () {
                if ($scope.searchCondition.Title!="" &&( $scope.Flow == null || $scope.Flow.Code == null)) {
                    messager.showMessage("请选择Form", "fa-warning c_orange");
                    return;
                }
                $scope.pageIndex = 1;
                $scope.pagging();
                //$scope.projectPageIndex = 1;
                //$scope.projectPagging();
            };
            $scope.download = function () {
                commentsListService.exportExcel($scope.searchCondition).$promise.then(function(data) {
                    if (data != null && data.fileName != "") {
                        var redirectUrl = Utils.ServiceURI.Address() + "api/attachment/Download?fileName=" + data.fileName;
                        $scope.packDownloadLink = redirectUrl;
                    }
                }, function (error) {
                    messager.showMessage(error.statusText, "fa-warning c_orange");
                });
            };
            $scope.removeSearchCondition = function(key) {
                $scope.searchCondition[key] = null;
                angular.forEach($scope.conditionTable, function(cdtItem, i) {
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
            var loadFlowInfo = function () {
                commentsListService.getCreateFlowInfo({ projectId: $scope.projectId, flowCode: $scope.flowCode }).$promise.then(function (data) {
                    if (data != null) {
                        $scope.entity = data;
                    }
                });
            };
            loadFlowInfo();

            $scope.IsFlowCodsComp = false;
            $scope.FlowCodes = [];
            var loadForms = function() {
                commentsListService.getFlowCodeList({ parentCode: $scope.flowCode }).$promise.then(function (data) {
                    if (data != null) {
                        $scope.FlowCodes = data;
                        if (!!$scope.subFlowCode) {
                            angular.forEach(data, function(o, i) {
                                if (o.Code == $scope.subFlowCode) {
                                    $scope.Flow = o;
                                    return true;
                                }
                            });
                        }
                    }
                    $scope.IsFlowCodsComp = true;
                });
            };
            loadForms();

            $scope.IsCreatorComp = false;
            $scope.Creators = [];
            var loadCreators = function() {
                commentsListService.getCreatorList({ projectId: $scope.projectId, flowCode: $scope.subFlowCode }).$promise.then(function (data) {
                    if (data != null) {
                        $scope.Creators = data;
                    }
                    $scope.IsCreatorComp = true;
                });
            };
            loadCreators();

            $scope.back = function() {
                history.back();
            };
            //初始化日期控件
            $scope.today = function () {
                $scope.dt = new Date();
            };
            $scope.today();

            $scope.clear = function () {
                $scope.dt = null;
            };

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
    ]);
}();