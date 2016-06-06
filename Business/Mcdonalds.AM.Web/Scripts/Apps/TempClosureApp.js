var tempClosureApp = angular.module("amApp", [
    "ngRoute",
    'ngChosen',
    "nttmnc.fx.modules",
    "ui.bootstrap",
    "mcd.am.modules",
    "mcd.am.filters",
    "mcd.am.modules.tempClosure",
    "mcd.am.tempClosure.controllers.create",
    "mcd.am.tempClosure.controllers.legalReview",
    "mcd.am.tempClosure.controllers.closurePackage",
    "mcd.am.tempClosure.controllers.closureMemo",
    "mcd.am.tempClosure.controllers.reopenMemo"
]);
tempClosureApp.run([
    "$window", "messager", function ($window, messager) {
        validateUser($window, messager);
    }
]);
tempClosureApp.config([
    "$routeProvider",
    "$sceProvider",
    function ($routeProvider, $sceProvider) {
        $sceProvider.enabled(false);
        $routeProvider
        .when("/Create", {
            controller: "createController",
            templateUrl: Utils.ServiceURI.AppUri + "TempClosure/Create"
        })
        .when("/LegalReview", {
            controller: "legalReviewController",
            templateUrl:Utils.ServiceURI.AppUri +"TempClosure/LegalReview"
        })
        .when("/LegalReview/Process/View", {
            controller: "legalReviewController",
            templateUrl: Utils.ServiceURI.AppUri + "TempClosure/LegalReviewView"
        })
        .when("/LegalReview/Process/Approval", {
            controller: "legalReviewController",
            templateUrl: Utils.ServiceURI.AppUri + "TempClosure/LegalReviewApproval"
        })
        .when("/LegalReview/Process/Resubmit", {
            controller: "legalReviewController",
            templateUrl: Utils.ServiceURI.AppUri + "TempClosure/LegalReviewResubmit"
        })
        .when("/ClosurePackage", {
            controller: "closurePackageController",
            templateUrl: Utils.ServiceURI.AppUri + "TempClosure/ClosurePackage"
        })
        .when("/ClosurePackage/Process/View", {
            controller: "closurePackageController",
            templateUrl: Utils.ServiceURI.AppUri + "TempClosure/ClosurePackageView"
        })
        .when("/ClosurePackage/Process/Approval", {
            controller: "closurePackageController",
            templateUrl: Utils.ServiceURI.AppUri + "TempClosure/ClosurePackageApproval"
        })
        .when("/ClosurePackage/Process/Resubmit", {
            controller: "closurePackageController",
            templateUrl: Utils.ServiceURI.AppUri + "TempClosure/ClosurePackageResubmit"
        }).when('/ClosurePackage/Process/Upload', {
            templateUrl: Utils.ServiceURI.AppUri + "TempClosure/ClosurePackageUpload",
            controller: 'closurePackageController'
        }).when('/ClosureMemo',
        {
            templateUrl: Utils.ServiceURI.AppUri + "TempClosure/ClosureMemo",
            controller: 'closureMemoController'
        }).when('/ClosureMemo/Process/View',
        {
            templateUrl: Utils.ServiceURI.AppUri + "TempClosure/ClosureMemoView",
            controller: 'closureMemoController'
        }).when('/ReopenMemo',
        {
            templateUrl: Utils.ServiceURI.AppUri + "TempClosure/ReopenMemo",
            controller: 'reopenMemoController'
        }).when('/ReopenMemo/Process/View',
        {
            templateUrl: Utils.ServiceURI.AppUri + "TempClosure/ReopenMemoView",
            controller: 'reopenMemoController'
        })
        .otherwise({
            redirectTo: "/Create"
        });
    }
]);