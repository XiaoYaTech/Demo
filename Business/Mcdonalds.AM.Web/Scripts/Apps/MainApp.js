var amApp = angular.module('amApp', [
    'ngRoute',
    'ngChosen',
    'ui.bootstrap',
    'mcd.am.controller.remind',
    'mcd.am.service.remind',
    'dictionaryControllers',
    'dictionaryServices',
    'dictionaryFilters',
    'remindRegisterControllers',
    'remindRegisterServices',
    'taskWorkControllers',
    'taskWorkMoreControllers',
    'project.list',
    'project.commentslist',
    'nttmnc.fx.modules',
    'mcd.am.modules',
    'mcd.am.filters'
]);
amApp.run(["$window", "messager", function ($window, messager) {
    validateUser($window, messager);
}]);
amApp.config([
    '$routeProvider',
    function ($routeProvider) {
      $routeProvider.
          when('/remind', {
              templateUrl: '/remind/List',
              controller: 'remindCtrl'
          }).when('/dictionary', {
              templateUrl: '/dictionary/List',
              controller: 'DictionaryCtrl'
          }).when('/remindRegister', {
              templateUrl: '/remindRegister/List',
              controller: 'remindRegisterCtrl'
          }).when('/taskwork', {
              templateUrl: '/TaskWork/List',
              controller: 'taskWorkCtrl'
          }).when('/projectList', {
              templateUrl: '/Home/ProjectList',
              controller: 'projectListCtrl'
          }).when('/CommentsList', {
              templateUrl: '/Home/CommentsList',
              controller: 'commentsListCtrl'
          })
          .when('/project/detail/:projectId', {
              templateUrl: '/Home/ProjectDetail',
              controller: 'projectDetailCtrl'
          }).when('/taskworkMore/:status', {
              templateUrl: '/TaskWork/MoreList',
              controller: 'taskWorkMoreCtrl'
      });
  }]);