angular.module("mcd.am.closure.services.closureTool", []).factory("closureToolService", [
    "$http",
    function ($http) {
        var service = {
            getCompensation: function (projectId) {
                return $http.get(Utils.ServiceURI.Address() + "api/closure/closureTool/compensation", {
                    cache: false,
                    params: {
                        projectId: projectId
                    }
                });
            },
            getFinanceData: function (entityId, projectId, year, month) {
                if (!entityId)
                    entityId = "00000000-0000-0000-0000-000000000000";
                if (year && month)
                    return $http.get(Utils.ServiceURI.Address() + "api/closure/closureTool/financedata", {
                        cache: false,
                        params: {
                            refTableId: entityId,
                            projectId: projectId,
                            financeYear: year,
                            financeMonth: month
                        }
                    });
                else
                    return $http.get(Utils.ServiceURI.Address() + "api/closure/closureTool/financedata", {
                        cache: false,
                        params: {
                            refTableId: entityId,
                            projectId: projectId
                        }
                    });
            },
            getSelectYearMonth: function (projectId) {
                return $http.get(Utils.ServiceURI.Address() + "api/closure/closureTool/selectyearmonth", {
                    cache: false,
                    params: {
                        projectId: projectId
                    }
                });
            },
            searchPipelineId: function (inputcode, pageSize, storeCode) {
                return $http.get(Utils.ServiceURI.Address() + "api/closure/closureTool/searchPipelineId", {
                    cache: false,
                    params: {
                        queryString: inputcode,
                        pageSize: pageSize,
                        storeCode: storeCode
                    }
                });
            },
            searchPipelineName: function (inputcode, pageSize, storeCode) {
                return $http.get(Utils.ServiceURI.Address() + "api/closure/closureTool/searchPipelineName", {
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
