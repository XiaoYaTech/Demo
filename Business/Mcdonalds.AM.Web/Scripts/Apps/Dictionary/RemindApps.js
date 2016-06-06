var remindApp = angular.module("amApp", ['ngRoute',
                                        'mcd.am.service.remind',
                                        'mcd.am.controller.remind',
                                        'mcd.am.filters',
                                        'mcd.am.modules'
]);

remindApp.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider.
          when('/List', {
              templateUrl: '/Remind/List',
              controller: 'remindCtrl'
          }).otherwise({
              redirectTo: '/List'
          });
  }]);


/* App Module */

//var clousreApp = angular.module('clousreApp', [
//  'ngRoute',


//  'dictionaryControllers',
//  'dictionaryServices'
//]);

//clousreApp.config(['$routeProvider',
//  function ($routeProvider) {
//      $routeProvider.
//        when('/phones', {
//            templateUrl: 'phone-list.html',
//            controller: 'PhoneListCtrl'
//        }).
//        when('/phones/:phoneId', {
//            templateUrl: 'phone-detail.html',
//            controller: 'PhoneDetailCtrl'
//        }).
//        otherwise({
//            redirectTo: '/phones'
//        });
//  }]);
