var RenewalApp = angular.module("amApp", [
    "ngRoute",
    'ngChosen',
    "nttmnc.fx.modules",
    "ui.bootstrap",
    "mcd.am.modules",
    "mcd.am.filters",
    "mcd.am.services.renewal",
    "mcd.am.services.siteinfo",
    "mcd.am.service.reopenMemo"
]);
RenewalApp.run([
    "$rootScope", "$window", "messager",
    function ($rootScope, $window, messager) {
        $rootScope.regs = window.ValidatePatterns;
        validateUser($window, messager);
    }
]);
RenewalApp.config([
    "$routeProvider",
    "$sceProvider",
    function ($routeProvider, $sceProvider) {
        $sceProvider.enabled(false);
        $routeProvider
        .when("/Create", {
            controller: "createController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/Create"
        })
        .when("/Letter", {
            controller: "letterController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/Letter"
        })
        .when("/Letter/Process/View", {
            controller: "letterController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/LetterView"
        })
        .when("/Letter/Process/Approval", {
            controller: "letterController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/LetterApproval"
        })
        .when("/Letter/Process/Resubmit", {
            controller: "letterController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/LetterResubmit"
        })
        .when("/LLNegotiation", {
            controller: "lLNegotiationController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/LLNegotiation"
        })
        .when("/LLNegotiation/Process/Resubmit", {
            controller: "lLNegotiationController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/LLNegotiationResubmit"
        })
        .when("/LLNegotiation/Process/View", {
            controller: "lLNegotiationController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/LLNegotiationView"
        })
        .when("/ConsInfo", {
            controller: "consInfoController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/ConsInfo"
        })
        .when("/ConsInfo/Process/Approval", {
            controller: "consInfoController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/ConsInfoApproval"
        })
        .when("/ConsInfo/Process/Resubmit", {
            controller: "consInfoController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/ConsInfoResubmit"
        })
        .when("/ConsInfo/Process/View", {
            controller: "consInfoController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/ConsInfoView"
        })
        .when("/Tool", {
            controller: "toolController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/Tool"
        })
        .when("/Tool/Process/Approval", {
            controller: "toolController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/ToolApproval"
        })
        .when("/Tool/Process/Resubmit", {
            controller: "toolController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/ToolResubmit"
        })
        .when("/Tool/Process/View", {
            controller: "toolController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/ToolView"
        })
        .when("/Analysis", {
            controller: "analysisController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/Analysis"
        })
        .when("/Analysis/Process/Resubmit", {
            controller: "analysisController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/AnalysisResubmit"
        })
        .when("/Analysis/Process/View", {
            controller: "analysisController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/AnalysisView"
        })
        .when("/ClearanceReport", {
            controller: "clearanceReportController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/ClearanceReport"
        })
        .when("/ClearanceReport/Process/Resubmit", {
            controller: "clearanceReportController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/ClearanceReportResubmit"
        })
        .when("/ClearanceReport/Process/View", {
            controller: "clearanceReportController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/ClearanceReportView"
        })
        .when("/ConfirmLetter", {
            controller: "confirmLetterController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/ConfirmLetter"
        })
        .when("/ConfirmLetter/Process/Resubmit", {
            controller: "confirmLetterController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/ConfirmLetterResubmit"
        })
        .when("/ConfirmLetter/Process/View", {
            controller: "confirmLetterController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/ConfirmLetterView"
        })
        .when("/LegalApproval", {
            controller: "legalApprovalController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/LegalApproval"
        })
        .when("/LegalApproval/Process/View", {
            controller: "legalApprovalController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/LegalApprovalView"
        })
        .when("/LegalApproval/Process/Approval", {
            controller: "legalApprovalController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/LegalApprovalApproval"
        })
        .when("/LegalApproval/Process/Resubmit", {
            controller: "legalApprovalController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/LegalApprovalResubmit"
        })
        .when("/Package", {
            controller: "packageController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/Package"
        })
        .when("/Package/Process/View", {
            controller: "packageController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/PackageView"
        })
        .when("/Package/Process/Approval", {
            controller: "packageController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/PackageApproval"
        })
        .when("/Package/Process/Upload", {
            controller: "packageController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/PackageUpload"
        })
        .when("/Package/Process/Resubmit", {
            controller: "packageController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/PackageResubmit"
        })
        .when("/ContractInfo", {
            controller: "contractInfoController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/ContractInfo"
        })
        .when("/ContractInfo/Process/View", {
            controller: "contractInfoController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/ContractInfoView"
        })
        .when("/SiteInfo", {
            controller: "siteInfoController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/SiteInfo"
        })
        .when("/SiteInfo/Process/View", {
            controller: "siteInfoController",
            templateUrl: Utils.ServiceURI.AppUri + "Renewal/SiteInfoView"
        })
            //////------ReopenMemo
            .when("/ReopenMemo", {
                templateUrl: "/Renewal/ReopenMemo",
                controller: 'reopenMemoController'
            })
            .when("/ReopenMemo/Process/:PageType", {
                templateUrl: "/Renewal/ReopenMemo",
                controller: 'reopenMemoController'
            })
            //////------GBMemo
            .when("/GBMemo", {
                templateUrl: "/Renewal/GBMemo",
                controller: 'GBMemoController'
            })
            .when("/GBMemo/Process/:PageType", {
                templateUrl: "/Renewal/GBMemo",
                controller: 'GBMemoController'
            })
            //////------ClosureMemo
            .when("/ClosureMemo", {
                templateUrl: "/Renewal/ClosureMemo",
                controller: 'closureMemoController'
            })
            .when("/TempClosureMemo/Process/:PageType", {
                templateUrl: "/Renewal/ClosureMemo",
                controller: 'closureMemoController'
            })
        .otherwise({
            redirectTo: "/Create"
        });
    }
]);