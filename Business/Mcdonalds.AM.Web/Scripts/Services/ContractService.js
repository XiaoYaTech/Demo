/// <reference path="../Utils/Utils.ts" />
/// <reference path="../Libs/Angular/angular.d.ts" />
angular.module("mcd.am.services.contract", []).factory("contractService", [
    "$http",
    function ($http) {
        var service = {
            query: function (projectId) {
                return $http.get(Utils.ServiceURI.Address() + "api/contract/get/project", {
                    cache: false,
                    params: {
                        projectId: projectId
                    }
                });
            },
            queryReVertions: function (contractId) {
                return $http.get(Utils.ServiceURI.Address() + "api/contract/revertions", {
                    cache: false,
                    params: {
                        contractId: contractId
                    }
                });
            },
            queryAttachments: function (projectId) {
                return $http.get(Utils.ServiceURI.Address() + "api/contract/attachments", {
                    cache: false,
                    params: {
                        projectId: projectId
                    }
                });
            },
            queryAttachmentCount: function (projectId) {
                return $http.get(Utils.ServiceURI.Address() + "api/contract/getAttachmentCount", {
                    cache: false,
                    params: {
                        projectId: projectId
                    }
                });
            },
            save: function (contract, revisions, flowCode) {
                return $http.post(Utils.ServiceURI.Address() + "api/contract/save", {
                    Contract: contract,
                    Revisions: revisions,
                    FlowCode: flowCode
                });
            },
            submit: function (contract, revisions, flowCode) {
                return $http.post(Utils.ServiceURI.Address() + "api/contract/submit", {
                    Contract: contract,
                    Revisions: revisions,
                    FlowCode: flowCode
                });
            },
            querySaveable: function (projectId) {
                return $http.get(Utils.ServiceURI.Address() + "api/contract/querySaveable", {
                    cache: false,
                    params: {
                        projectId: projectId
                    }
                });
            }
        };
        return service;
    }]);
