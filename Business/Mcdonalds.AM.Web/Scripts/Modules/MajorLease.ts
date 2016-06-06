/// <reference path="../Libs/moment/moment.d.ts" />
/// <reference path="../Libs/Angular/Plugins/angular-ui-bootstrap.d.ts" />
/// <reference path="../Libs/Angular/Plugins/angular-cookies.d.ts" />
/// <reference path="../Utils/Utils.ts" />
/// <reference path="../Libs/Angular/angular.d.ts" />
/// <reference path="../Utils/CurrentUser.ts" />
angular.module("mcd.am.modules.majorLease", ["nttmnc.fx.modules", "mcd.am.service.taskwork"])
    .directive("majorLeaseChange", ["$http", "majorLeaseService", ($http: ng.IHttpService, majorLeaseService) => {
        return {
            restrict: "EA",
            scope: {
                projectId: "@",
                rental: "=?",
                rentalDesc: "=?",
                redline: "=?",
                redlineDesc: "=?",
                leaseterm: "=?",
                leasetermDesc: "=?",
                landlord: '=?',
                landlordDesc: '=?',
                others: '=?',
                othersDesc: '=?',
                isEdit: "="
            },
            replace: true,
            templateUrl: Utils.ServiceURI.AppUri + "MajorLease/MajorLeaseChangeTemp",
            link: ($scope: any, ele: JQuery, attr: any) => {
                if ($scope.projectId == null)
                    return;
                majorLeaseService.getMajorInfo({
                    projectId: $scope.projectId
                }).$promise.then((response: any) => {
                        if (response == null)
                            return;
                        $scope.rental = response.ChangeRentalType;
                        $scope.redline = response.ChangeRedLineType;
                        $scope.leaseterm = response.ChangeLeaseTermType;
                        $scope.landlord = response.ChangeLandLordType;
                        $scope.others = response.ChangeOthersType;

                        if (response.ChangeRedLineTypeDESC) {
                            $scope.redlineDesc = response.ChangeRedLineTypeDESC;
                        }
                        if (response.ChangeLeaseTermDESC) {
                            $scope.leasetermDesc = response.ChangeLeaseTermDESC;
                        }
                        if (response.ChangeRentalTypeDESC) {
                            $scope.rentalDesc = response.ChangeRentalTypeDESC;
                        }
                        if (response.ChangeRedLineTypeDESC) {
                            $scope.redlineDesc = response.ChangeRedLineTypeDESC;
                        }
                        if (response.ChangeLeaseTermDESC) {
                            $scope.leasetermDesc = response.ChangeLeaseTermDESC;
                        }
                        if (response.ChangeLandLordDESC) {
                            $scope.landlordDesc = response.ChangeLandLordDESC;
                        }
                        if (response.ChangeOthersDESC) {
                            $scope.othersDesc = response.ChangeOthersDESC;
                        }

                    }, (error: any) => {
                        alert(error.status);
                    });
            }
        };
    }])
    .directive("majorLeaseNav", [
        "majorLeaseService",
        "messager",
        "taskWorkService",
        (majorLeaseService, messager, taskWorkService): ng.IDirective => {
            return {
                restrict: "EA",
                scope: {
                    projectId: "=",
                    currSubFlowCode: "@"
                },
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "MajorLease/NavTop",
                link: ($scope: any, ele: JQuery, attr: any) => {
                    if ($scope.projectId == null)
                        return;
                    var Navs: any[] = [];
                    majorLeaseService.getMajorTopNav({ projectId: $scope.projectId, userCode: window["currentUser"].Code }).$promise.then((data: any) => {
                        if (data == null)
                            return;
                        var projectId = $scope.projectId;
                        var userList: any[] = data.UserList;
                        var finishedTaskList: any[] = data.FinishedTaskList;
                        var navInfos: any[] = data.NavInfos;
                        for (var i = 0; i < navInfos.length; i++) {
                            if (navInfos[i].SubFlowCode.toLowerCase() == $scope.currSubFlowCode.toLowerCase()) {
                                navInfos[i].IsSelected = true;
                            }

                            var navHref = navInfos[i].Href + "/Process/View?projectId=" + projectId;

                            for (var t = 0; t < finishedTaskList.length; t++) {
                                if (finishedTaskList[t].FlowCode.toLowerCase() == navInfos[i].SubFlowCode.toLowerCase()) {

                                    navInfos[i].IsFinished = true;
                                }
                            }
                            //switch (navInfos[i].SubFlowCode) {
                            //    case "MajorLease_LeaseChangePackage":
                            //        for (var j = 0; j < userList.length; j++) {
                            //            if (userList[j].RoleCode == "AssetActor") {
                            //                navHref = navInfos[i].Href + "/" + projectId;
                            //            }
                            //        }
                            //        break;
                            //    case "MajorLease_FinanceAnalysis":
                            //        for (var j = 0; j < userList.length; j++) {
                            //            if (userList[j].RoleCode == "Finance") {
                            //                navHref = navInfos[i].Href + "/" + projectId;
                            //            }
                            //        }
                            //        break;
                            //    case "MajorLease_ConsInfo":
                            //        for (var j = 0; j < userList.length; j++) {
                            //            if (userList[j].RoleCode == "PM") {
                            //                navHref = navInfos[i].Href + "/" + projectId;

                            //            }
                            //        }
                            //        break;
                            //    case "MajorLease_LegalReview":
                            //        for (var j = 0; j < userList.length; j++) {
                            //            if (userList[j].RoleCode == "Legal" || userList[j].RoleCode == "AssetActor") {
                            //                navHref = navInfos[i].Href + "/" + projectId;
                            //            }
                            //        }
                            //        break;
                            //    case "MajorLease_ConsInvtChecking":
                            //        for (var j = 0; j < userList.length; j++) {
                            //            if (userList[j].RoleCode == "PM") {
                            //                navHref = navInfos[i].Href + "/" + projectId;
                            //            }
                            //        }
                            //        break;
                            //    case "MajorLease_ContractInfo":
                            //        for (var j = 0; j < userList.length; j++) {
                            //            if (userList[j].RoleCode == "AssetActor") {
                            //                navHref = navInfos[i].Href + "/" + projectId;
                            //            }
                            //        }
                            //        break;
                            //    case "MajorLease_SiteInfo":
                            //        for (var j = 0; j < userList.length; j++) {
                            //            if (userList[j].RoleCode == "PM") {
                            //                navHref = navInfos[i].Href + "/" + projectId;
                            //            }
                            //        }
                            //        break;
                            //}
                            //navInfos[i].Href = navHref;
                            Navs.push(navInfos[i]);
                            $scope.Navs = Navs;
                        }
                    }, (error: any) => {
                            messager.showMessage(error.message, "fa-warning c_orange");
                        });
                    $scope.loadOperators = (nav: any, projectId: string) => {
                        if (!nav.operatorLoaded) {
                            nav.operatorHTML = "<img src='" + Utils.ServiceURI.AppUri + "Content/Images/loading.gif' />";
                            taskWorkService.getOperators(nav.SubFlowCode, projectId).then((response: any) => {
                                var operators = response.data;
                                var OperatorHTML: string = "";
                                if (!!operators && operators.length > 0) {
                                    OperatorHTML += "<ul class='node-operators'>";
                                    angular.forEach(operators, (o, i) => {
                                        OperatorHTML += "<li><span class='fa fa-user'></span> " + o.Code + "  " + o.NameENUS + "</li>";
                                    });
                                    OperatorHTML += "</ul>";
                                } else {
                                    OperatorHTML = "<p class='node-operators-none'>暂无处理人</p>";
                                }
                                nav.operatorHTML = OperatorHTML;
                                nav.operatorLoaded = true;
                            });
                        }
                    }
            }
            };
        }])
    .directive("reinvenstmentAmountType", [
        "majorLeaseService",
        (majorLeaseService, messager): ng.IDirective => {
    return {
                restrict: "EA",
                scope: {
                    isEdit: "=",
                    amountType: "=?",
                    amount: "=?"
                },
                templateUrl: Utils.ServiceURI.AppUri + "MajorLease/ReinvenstmentAmountType",
                replace: true,
                link: ($scope: any, ele: JQuery, attr: any) => {
                    var GetAmount = (val: any) => {
                        angular.forEach($scope.arrAmountType, (v: any, k: any) => {
                            if (v.Id == val) {
                                $scope.amount = v.Amount;
                            }
                        });
                    };
                    $scope.$watch("amountType", (val: any) => {
                        if (!$scope.arrAmountType) {
                            return;
                        }
                        GetAmount(val);
                    });

                    majorLeaseService.getReinvenstmentAmountType().$promise.then((data: any) => {
                        $scope.arrAmountType = data;
                        if ($scope.amountType) {
                            GetAmount($scope.amountType);
                        }
                    });
                }
            }
}]);