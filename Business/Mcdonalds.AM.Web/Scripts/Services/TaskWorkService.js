/// <reference path="../Utils/Utils.ts" />
/// <reference path="../Libs/Angular/angular.d.ts" />
angular.module("mcd.am.service.taskwork", []).factory("taskWorkService", [
    "$http",
    function ($http) {
        var service = {
            getTasks: function (userCode, status, pageIndex, pageSize, title, sourceCode, params) {
                return $http.get(Utils.ServiceURI.Address() + "api/taskwork/" + pageIndex + "/" + pageSize + "/" + userCode, {
                    cache: false,
                    params: {
                        Status: status,
                        Title: title,
                        SourceCode: sourceCode,
                        StoreCode: params ? params.StoreCode : null,
                        StoreName: params ? params.StoreName : null
                    }
                });
            },
            getOperators: function (typeCode, refId) {
                return $http.get(Utils.ServiceURI.Address() + "api/taskwork/operators", {
                    cache: false,
                    params: {
                        typeCode: typeCode,
                        refId: refId
                    }
                });
            },
            ifUndo: function (typeCode, refId) {
                return $http.get(Utils.ServiceURI.Address() + "api/taskwork/ifUndo", {
                    cache: false,
                    params: {
                        typeCode: typeCode,
                        refId: refId
                    }
                });
            },
            Complete: function (projectId, flowCode) {
                return $http.get(Utils.ServiceURI.Address() + "api/taskwork/Complete", {
                    cache: false,
                    params: {
                        projectId: projectId,
                        flowCode: flowCode
                    }
                });
            }
        };
        return service;
    }
]);
