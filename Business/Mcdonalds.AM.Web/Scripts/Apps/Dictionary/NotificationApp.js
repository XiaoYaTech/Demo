var notificationApp = angular.module("amApp", ['ngRoute',
                                               'mcd.am.service.notification',
                                               'mcd.am.controller.notification',
                                               'mcd.am.filters',
                                               'mcd.am.modules']);

notificationApp.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider.
          when('/List', {
              templateUrl: '/Notification/List',
              controller: 'notificationCtrl'
          }).otherwise({
              redirectTo: '/List'
          });
  }]);



