
//nttmnc.fx.modules 是基础框架的组件
var dictionaryApp = angular.module('amApp', [
    "ngRoute",
    'ngChosen',
    "ngCookies",
    "ngUploadify",
    "ui.bootstrap",
    "nttmnc.fx.modules",
    "mcd.am.modules",
    "closureCreateServices",
    "mcd.am.closure.modules",
    "mcd.am.filters",
    "mcd.am.service.taskwork",
    "mcd.am.closure.services.closureMemo"
]);
dictionaryApp.run(["$window", "messager", function ($window, messager) {
    validateUser($window, messager);
}]);
dictionaryApp.config(['$routeProvider', "$sceProvider",

  function ($routeProvider, $sceProvider) {
      $sceProvider.enabled(false);
      $routeProvider

        .when('/Create', {
            templateUrl: '/Closure/Create',
            controller: 'closureCreateController'
        }).when('/WOCheckList', {//----------------------------------------------------WOCheckList
            templateUrl: '/Closure/WOCheckList',
            controller: 'woCheckListController'
        }).when('/WOCheckList/Process/Approval', {
            templateUrl: '/Closure/WOCheckListProcess',
            controller: 'woCheckListProcessController'
        }).when('/WOCheckList/Process/View', {
            templateUrl: '/Closure/WOCheckListView',
            controller: 'woCheckListViewController'
        }).when('/WOCheckList/Process/Resubmit', {
            templateUrl: '/Closure/WOCheckListProcessEdit',
            controller: 'woCheckListProcessEditController'
        }).when('/ClosureTool', {//----------------------------------------------------ClosureTool
            templateUrl: '/Closure/ClosureTool',
            controller: 'closureToolController'
        }).when('/ClosureTool/Process/View', {
            templateUrl: '/Closure/ClosureToolView',
            controller: 'closureToolViewController'
        }).when('/ClosureTool/Process/Approval', {
            templateUrl: '/Closure/ClosureToolProcess',
            controller: 'closureToolProcessController'
        }).when('/ClosureTool/Process/Resubmit', {
            templateUrl: '/Closure/ClosureToolProcessEdit',
            controller: 'closureToolProcessEditController'
        }).when('/LegalReview', {//----------------------------------------------------LegalReview
            templateUrl: '/Closure/LegalReview',
            controller: 'closureLegalReviewController'
        }).when('/LegalReview/Process/View', {
            templateUrl: '/Closure/LegalReviewView',
            controller: 'closureLegalReviewViewController'
        }).when('/LegalReview/Process/Approval', {
            templateUrl: '/Closure/LegalReviewProcess',
            controller: 'closureLegalReviewProcessController'
        }).when('/LegalReview/Process/Resubmit', {
            templateUrl: '/Closure/LegalReviewProcessEdit',
            controller: 'closureLegalReviewProcessEditController'
        }).when('/ExecutiveSummary', {//----------------------------------------------------ExecutiveSummary
            templateUrl: '/Closure/ExecutiveSummary',
            controller: 'executiveSummaryController'
        }).when('/ExecutiveSummary/Process/View', {
            templateUrl: '/Closure/ExecutiveSummaryView',
            controller: 'executiveSummaryController'
        }).when('/ClosurePackage', {//----------------------------------------------------ClosurePackage
            templateUrl: '/Closure/ClosurePackage',
            controller: 'closurePackageController'
        }).when('/ClosurePackage/Process/View', {
            templateUrl: '/Closure/ClosurePackageView',
            controller: 'closurePackageViewController'
        }).when('/ClosurePackage/Process/Approval', {
            templateUrl: '/Closure/ClosurePackageProcess',
            controller: 'closurePackageProcessController'
        }).when('/ClosurePackage/Process/Resubmit', {
            templateUrl: '/Closure/ClosurePackageProcessEdit',
            controller: 'closurePackageProcessEditController'
        }).when('/ClosurePackage/Process/Upload', {
            templateUrl: '/Closure/ClosurePackageProcessUpload',
            controller: 'closurePackageController'
        }).when('/ContractInfo', {//----------------------------------------------------ContractInfo
            templateUrl: '/Closure/ContractInfo',
            controller: 'contractInfoController'
        }).when('/ContractInfo/Process/View', {
            templateUrl: '/Closure/ContractInfoView',
            controller: 'contractInfoViewController'
        }).when('/ClosureMemo', {//----------------------------------------------------ClosureMemo
            templateUrl: '/Closure/ClosureMemo',
            controller: 'closureMemoController'
        }).when('/ClosureMemo/Process/View', {
            templateUrl: '/Closure/ClosureMemoView',
            controller: 'closureMemoViewController'
        }).when('/ConsInvtChecking', {//----------------------------------------------------ConsInvtChecking
            templateUrl: '/Closure/ConsInvtChecking',
            controller: 'consInvtCheckingController'
        }).when('/ConsInvtChecking/Process/View', {
            templateUrl: '/Closure/ConsInvtCheckingView',
            controller: 'consInvtCheckingViewController'
        }).when('/ConsInvtChecking/Process/Approval', {
            templateUrl: '/Closure/ConsInvtCheckingProcess',
            controller: 'consInvtCheckingProcessController'
        }).when('/ConsInvtChecking/Process/Resubmit', {
            templateUrl: '/Closure/ConsInvtCheckingProcessEdit',
            controller: 'consInvtCheckingProcessEditController'
        }).otherwise({
            redirectTo: '/Closure/Index',
        });
  }]);