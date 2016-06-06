

var clousreApp = angular.module('remindRegisterApp', ['ngRoute',
  'remindRegisterControllers',
  'remindRegisterServices']);

clousreApp.config(['$routeProvider',
  function ($routeProvider) {
      $routeProvider.
          when('/remindRegister', {
              templateUrl: '/remindRegister/List',
              controller: 'remindRegisterCtrl'
          }).otherwise({
              redirectTo: '/remindRegister'
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
