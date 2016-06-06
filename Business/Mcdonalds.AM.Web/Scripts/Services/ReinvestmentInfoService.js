!function () {
    angular.module("mcd.am.services.reinvestmentInfo", ["ngResource"]).factory("reinvestmentInfoService", ["$resource", function ($resource) {
        return $resource(Utils.ServiceURI.Address() + "api/ReinvestmentInfo/:action", {}, {
            get: { Method: "GET", params: { action: "GetReinvestmentInfo" }, isArray: false }
        });
    }]);
}();