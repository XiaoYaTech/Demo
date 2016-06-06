!function () {
    angular.module("mcd.am.services.summaryReinvestmentCost", ["ngResource"]).factory("summaryReinvestmentCostService", ["$resource", function ($resource) {
        return $resource(Utils.ServiceURI.Address() + "api/SummaryReinvestmentCost/:action", {}, {
            get: { Method: "GET", params: { action: "GetSummaryReinvestmentCost" }, isArray: false }
        });
    }]);
}();