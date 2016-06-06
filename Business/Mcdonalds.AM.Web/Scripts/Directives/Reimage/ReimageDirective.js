var reimageDirectives = angular.module("amApp.reimage.directives", ['mcd.am.service.taskwork']);

reimageDirectives.directive('reimageNav', [
    "taskWorkService",
    function (taskWorkService) {
        return {
            restrict: 'E',
            replace: 'true',
            templateUrl: '/Template/ClosureNavs',
            link: function ($scope, $http, element, attrs) {
                var projectId = element.projectid;
                var code = element.code;

                $scope.navs = [];
                //初始化导航数据
                var navs = [
                    {
                        href: "#/Reimage/ConsInfo",
                        nameZHCN: "ConsInfo",
                        code: "Reimage_ConsInfo",
                        isSelected: false,
                        isFinished: false
                    },

                    {
                        href: "#/Reimage/ReimageSummary",
                        nameZHCN: "Reimage Summary",
                        code: "Reimage_Summary",
                        isSelected: false,
                        isFinished: false
                    }
                ];

                var closureUsersUrl = Utils.ServiceURI.Address() + 'api/ProjectUsers/GetCurrentByProjectId/' + projectId;
                $.ajax({

                    type: "GET",
                    url: closureUsersUrl,
                    dataType: "json",

                    success: function (data) {
                        var a = data;
                        if (data != "null") {
                            var closureUserList = data.closureUserList;
                            var finishedTaskList = data.finishedTaskList;

                            //判断流程状态

                            for (var i = 0; i < navs.length; i++) {
                                if (navs[i].code.toLowerCase() == code.toLowerCase()) {
                                    navs[i].isSelected = true;
                                }

                                var navHref = navs[i].href + "/View/param?projectId=" + projectId;


                                //判断流程状态
                                for (var t = 0; t < finishedTaskList.length; t++) {
                                    if (finishedTaskList[t].TypeCode.toLowerCase() == navs[i].nameZHCN.toLowerCase()) {

                                        navs[i].isFinished = true;
                                    }
                                }

                                switch (navs[i].nameZHCN) {
                                    case "ConsInfo":
                                        for (var j = 0; j < closureUserList.length; j++) {
                                            if (closureUserList[j].RoleCode == "PM" && closureUserList[j].TaskStatus == 0) {
                                                navHref = navs[i].href + "/" + projectId;
                                            }
                                        }
                                        break;

                                    case "Reimage Summary":
                                        for (var j = 0; j < closureUserList.length; j++) {
                                            if (closureUserList[j].RoleCode == "AssetActor" && closureUserList[j].TaskStatus == 0) {
                                                navHref = navs[i].href + "/" + projectId;
                                            }
                                        }
                                        break;
                                 
                                }
                                navs[i].href = navHref;

                                $scope.closureNavs.push(navs[i]);
                            }
                        }
                    },
                    error: function (err) {
                        var a = err;
                    }

                });
                $scope.loadOperators = function (nav, projectId) {
                    if (!nav.operatorLoaded) {
                        nav.operatorHTML = "<img src='" + Utils.ServiceURI.AppUri + "Content/Images/loading.gif' />";
                        taskWorkService.getOperators(nav.code, projectId).then(function (response) {
                            var operators = response.data;
                            var OperatorHTML = "";
                            if (!!operators && operators.length > 0) {
                                OperatorHTML += "<ul class='node-operators'>";
                                angular.forEach(operators, function (o, i) {
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
    }]);