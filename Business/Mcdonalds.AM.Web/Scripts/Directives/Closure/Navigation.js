angular.module('myApp.directives', ["mcd.am.service.taskwork"]).
directive('closureNav', [
    "$http",
    "taskWorkService",
    function ($http,taskWorkService) {
        return {
            restrict: 'E',
            replace: true,
            scope: {
                code: "@",
                projectId:"="
            },
            templateUrl: '/Template/ClosureNavs',
            link: function ($scope, element, attrs) {
             
                $scope.closureNavs = [{}, {}, {}, {}, {}, {}, {}, {}];

                $scope.$watch("projectId", function (val) {
                    if (!!val) {
                     
                        var closureUsersUrl = Utils.ServiceURI.Address() + 'api/ProjectUsers/GetCurrentByProjectId/' + val+"/Closure"+"/"+$scope.code+"/"+window.currentUser.Code;
                        $http.get(closureUsersUrl).success(function (data) {
                           
                            if (data != "null") {
                              
                                $scope.closureNavs = data;
                            }
                        });
                    }
                });
                
                $scope.loadOperators = function (nav, projectId) {
               
                    if (!nav.operatorLoaded) {
                        nav.operatorHTML = "<img src='" + Utils.ServiceURI.AppUri + "Content/Images/loading.gif' />";
                        taskWorkService.getOperators(nav.Code, projectId).then(function (response) {
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
