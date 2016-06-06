angular.module("mcd.am.service.system", ["ngResource"]).factory("systemService", ["$resource", function ($resource) {
    return $resource(Utils.ServiceURI.Address() + "api/system/:servicePath", {}, {
        getReminders: { Method: "GET", params: { servicePath: "reminders" }, isArray: false }
    });
}]);