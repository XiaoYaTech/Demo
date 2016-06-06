angular.module("mcd.am.service.attachment", ["ngResource"]).factory("attachmentService", ["$resource", function ($resource) {
    return $resource(Utils.ServiceURI.Address() + "api/attachment/:servicePath", {}, {
       
        query: { method: "GET", params: { servicePath: "" }, isArray: true },
        upload: { method: "POST", params: { servicePath: "upload" }, isArray: false },
        deleteAttachment: { method: "POST", params: { servicePath: "delete", projectId: "@ProjectId", id: "@Id", requirementId: "@RequirementId" }, isArray: false }
    });
}]);