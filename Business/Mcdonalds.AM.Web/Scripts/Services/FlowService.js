angular.module("mcd.am.services.flow", ["ngResource"]).factory("flowService", ["$resource", function ($resource) {
    return $resource(Utils.ServiceURI.Address() + "api/flow/:action", {}, {
        topNav: { Method: "GET", params: { action: "topnav" }, isArray: true },
        getNodes: { Method: "GET", params: { action: "nodes" }, isArray: true },
        getFlowInfo: { Method: "GET", params: { action: "GetFlowInfo" }, isArray: false }
    });
}]);