

var clousreApp = angular.module('remindApp', ['ngRoute'],
 'mcd.am.controller.remind', 'mcd.am.service.remind');

clousreApp.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider.
           when('/111', {
               templateUrl: '/dictionary/List',
               controller: 'remindCtrl'
           }).
          when('/222', {
              templateUrl: '/remind/List',
              controller: 'remindCtrl'
          }).otherwise({
              redirectTo: '/111'
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
