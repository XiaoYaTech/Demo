/// <reference path="../Libs/moment/moment.d.ts" />
/// <reference path="../Libs/Angular/Plugins/angular-ui-bootstrap.d.ts" />
/// <reference path="../Libs/Angular/Plugins/angular-cookies.d.ts" />
/// <reference path="../Utils/Utils.ts" />
/// <reference path="../Libs/Angular/angular.d.ts" />
/// <reference path="../Utils/CurrentUser.ts" />
var noticesApp = angular.module('amApp', [
    'ngRoute',
    'ngChosen',
    'ui.bootstrap',
    'nttmnc.fx.modules',
    'mcd.am.modules',
    'mcd.am.filters',
    'mcd.am.notices.controller'
]);
noticesApp.run([
    "$window", "messager", function ($window, messager) {
        validateUser($window, messager);
    }
]);
noticesApp.config([
    '$routeProvider',
    function ($routeProvider) {
        $routeProvider.when('/list', {
            templateUrl: Utils.ServiceURI.AppUri + 'Notices/List',
            controller: 'amNoticesListCtrl'
        }).when('/detail/:noticeId', {
            templateUrl: Utils.ServiceURI.AppUri + 'Notices/Detail',
            controller: 'amNoticesDetailCtrl'
        }).otherwise({
            redirectTo: "/list"
        });
    }]);
