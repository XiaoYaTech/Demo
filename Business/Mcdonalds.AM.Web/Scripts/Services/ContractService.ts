/// <reference path="../Utils/Utils.ts" />
/// <reference path="../Libs/Angular/angular.d.ts" />
angular.module("mcd.am.services.contract", [])
 .factory("contractService", [
     "$http",
     ($http:ng.IHttpService)=> {
        var service = {
            query:(projectId:string)=>{
                return $http.get(Utils.ServiceURI.Address()+"api/contract/get/project",{
                    cache:false,
                    params:{
                        projectId:projectId
                    } 
                });
            },
            queryReVertions:(contractId:string)=>{
                return $http.get(Utils.ServiceURI.Address()+"api/contract/revertions",{
                    cache:false,
                    params:{
                        contractId:contractId
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
            save:(contract:any,revisions:any[])=>{
                return $http.post(Utils.ServiceURI.Address()+"api/contract/save",{
                    Contract:contract,
                    Revisions:revisions
                });
            },
            submit: (contract: any, revisions: any[]) => {
                return $http.post(Utils.ServiceURI.Address() + "api/contract/submit", {
                    Contract: contract,
                    Revisions: revisions
                });
            },
            querySaveable: (projectId) => {
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
