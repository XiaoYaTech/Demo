angular.module("mcd.am.closure.services.closureMemo", []).factory("closureMemoService", [
    "$http",
    function ($http) {
        var service = {
            get: function (projectId) {
                return $http.get(Utils.ServiceURI.Address() + "api/closure/closurememo", {
                    cache: false,
                    params: {
                        projectId: projectId
                    }
                });
            },
            send: function (closureMemo, receivers) {
                return $http.post(Utils.ServiceURI.Address() + "api/closure/closurememo/send", {
                    Entity: closureMemo,
                    Receivers: receivers
                });
            },
            save: function (closureMemo) {
                return $http.post(Utils.ServiceURI.Address() + "api/closure/closurememo/save", closureMemo);
            },
            searchPipeline: function (inputcode, pageSize, storeCode) {
                return $http.get(Utils.ServiceURI.Address() + "api/closure/closurememo/searchpipelines", {
                    cache: false,
                    params: {
                        queryString: inputcode,
                        pageSize: pageSize,
                        storeCode: storeCode
                    }
                });
            }
        };
        return service;
    }
]);
