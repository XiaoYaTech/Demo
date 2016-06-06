!function () {
    angular.module("mcd.am.services.StoreProfitabilityAndLeaseInfo", ["ngResource"]).factory("StoreProfitabilityAndLeaseInfoService", ["$resource", function ($resource) {
        return $resource(Utils.ServiceURI.Address() + "api/StoreProfitabilityAndLeaseInfo/:action", {}, {
            get: { Method: "GET", params: { action: "GetStoreProfitabilityAndLeaseInfo" }, isArray: false },
            getSelectYearMonth: { Method: "GET", params: { action: "GetSelectYearMonth" }, isArray: false }             
        });
    }]);
}();