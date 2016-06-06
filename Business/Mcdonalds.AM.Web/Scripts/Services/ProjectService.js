angular.module("mcd.am.services.project", ["ngResource"]).factory("projectService", ["$resource", function ($resource) {
    return $resource(Utils.ServiceURI.Address() + "api/project/:action", {}, {
        pending: { method: "POST", params: { action: "PendingProject" }, isArray: false },
        resume: { method: "POST", params: { action: "ResumeProject" }, isArray: false },
        recall: { method: "POST", params: { action: "RecallProject" }, isArray: false },
        edit: { method: "POST", params: { action: "EditProject" }, isArray: false },
        editMulty: { method: "POST", params: { action: "EditMultipleProjects" }, isArray: false },
        changeProjectTeam: { method: "POST", params: { action: "ChangeProjectTeamMembers" }, isArray: false },
        changeWorkflowApprovers: { method: "POST", params: { action: "ChangeWorkflowApprovers" }, isArray: false },
        changeProjectStatus: { method: 'POST', params: { action: 'ChangeProjectStatus', isArray: false } },
        isShowSave: { method: 'GET', params: { action: 'IsShowSave', isArray: false } },
        getProjectStatusChangeLog: { method: 'GET', params: { action: 'GetProjectStatusChangeLog' }, isArray: false },
        changePackageHoldingStatus: { method: 'POST', params: { action: 'ChangePackageHoldingStatus' }, isArray: false },
        outputExcel: { method: 'POST', params: { action: 'OutputExcel' }, isArray: false }
    });
}]);