!function () {
    angular.module("mcd.am.services.financialPreAnalysis", ["ngResource"]).factory("financialPreAnalysisService", ["$resource", function ($resource) {
        return $resource(Utils.ServiceURI.Address() + "api/financialPreAnalysis/:action", {}, {
            //get: { Method: "GET", params: { action: "GetFinancialPreAnalysis" }, isArray: false },
            getTTMAndRoI: { Method: "GET", params: { action: "GetTTMAndRoI" }, isArray: false }
        });
    }]);
}();