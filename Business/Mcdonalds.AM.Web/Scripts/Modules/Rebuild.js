/// <reference path="../Libs/moment/moment.d.ts" />
/// <reference path="../Libs/Angular/Plugins/angular-ui-bootstrap.d.ts" />
/// <reference path="../Libs/Angular/Plugins/angular-cookies.d.ts" />
/// <reference path="../Utils/Utils.ts" />
/// <reference path="../Libs/Angular/angular.d.ts" />
/// <reference path="../Utils/CurrentUser.ts" />
angular.module("mcd.am.modules.rebuild", ["nttmnc.fx.modules", "mcd.am.service.taskwork"]).directive("rebuildLeaseChange", [
    "$http", "rebuildService", function ($http, rebuildService) {
        return {
            restrict: "EA",
            scope: {
                source: "=?",
                isEdit: "="
            },
            replace: true,
            templateUrl: Utils.ServiceURI.AppUri + "Rebuild/RebuildLeaseChangeTemp",
            link: function ($scope, ele, attr) {
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
]);
//.directive("rebuildNav", [
//    "rebuildService",
//    "messager",
//    "taskWorkService",
//    (rebuildService, messager, taskWorkService): ng.IDirective => {
//        return {
//            restrict: "EA",
//            scope: {
//                projectId: "=",
//                currSubFlowCode: "@"
//            },
//            replace: true,
//            templateUrl: Utils.ServiceURI.AppUri + "Rebuild/NavTop",
//            link: ($scope: any, ele: JQuery, attr: any) => {
//                if ($scope.projectId == null)
//                    return;
//                var Navs: any[] = [];
//                rebuildService.getRebuildTopNav({ projectId: $scope.projectId, userCode: window["currentUser"].Code }).$promise.then((data: any) => {
//                    if (data == null)
//                        return;
//                    var finishedTaskList: any[] = data.FinishedTaskList;
//                    var navInfos: any[] = data.NavInfos;
//                    for (var i = 0; i < navInfos.length; i++) {
//                        if (navInfos[i].SubFlowCode.toLowerCase() == $scope.currSubFlowCode.toLowerCase()) {
//                            navInfos[i].IsSelected = true;
//                        }
//                        for (var t = 0; t < finishedTaskList.length; t++) {
//                            if (finishedTaskList[t].FlowCode.toLowerCase() == navInfos[i].SubFlowCode.toLowerCase()) {
//                                navInfos[i].IsFinished = true;
//                            }
//                        }
//                        Navs.push(navInfos[i]);
//                        $scope.Navs = Navs;
//                    }
//                }, (error: any) => {
//                        messager.showMessage(error.message, "fa-warning c_orange");
//                    });
//                $scope.loadOperators = (nav: any, projectId: string) => {
//                    if (!nav.operatorLoaded) {
//                        nav.operatorHTML = "<img src='" + Utils.ServiceURI.AppUri + "Content/Images/loading.gif' />";
//                        taskWorkService.getOperators(nav.SubFlowCode, projectId).then((response: any) => {
//                            var operators = response.data;
//                            var OperatorHTML: string = "";
//                            if (!!operators && operators.length > 0) {
//                                OperatorHTML += "<ul class='node-operators'>";
//                                angular.forEach(operators, (o, i) => {
//                                    OperatorHTML += "<li><span class='fa fa-user'></span> " + o.Code + "  " + o.NameENUS + "</li>";
//                                });
//                                OperatorHTML += "</ul>";
//                            } else {
//                                OperatorHTML = "<p class='node-operators-none'>暂无处理人</p>";
//                            }
//                            nav.operatorHTML = OperatorHTML;
//                            nav.operatorLoaded = true;
//                        });
//                    }
//                }
//        }
//        };
//    }]);
