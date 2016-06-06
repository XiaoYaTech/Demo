angular.module("mcd.am.services.contractRevision", []).factory("contractRevisionService", [
    "$http",
    function ($http) {
        var service = {
            query: function (projectId,contractId) {
                return $http.get(Utils.ServiceURI.Address() + "api/contract/revisions", {
                    cache: false,
                    params: {
                        projectId: projectId,
                        contractId: contractId
                    }
                });
            }
        };
        return service;
    }]);