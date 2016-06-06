angular.module("mcd.am.services.projectUsers", []).factory("projectUsersService", [
    "$http",
    function ($http) {
        return {
            inRole: function (projectId, userCode, roleCode) {
                return $http.get(Utils.ServiceURI.Address() + "api/projectusers/role/exist", {
                    cache: false,
                    params: {
                        projectId: projectId,
                        userCode: userCode,
                        roleCode: roleCode
                    }
                })
            },
            getRoles: function (projectId, userCode) {
                return $http.get(Utils.ServiceURI.Address() + "api/projectusers/roles/get", {
                    cache: false,
                    params: {
                        projectId: projectId,
                        userCode: userCode
                    }
                });
            },
            canInnerEdit: function (projectId) {
                return $http.get(Utils.ServiceURI.Address() + "api/projectusers/canInnerEdit", {
                    cache: false,
                    params: {
                        projectId: projectId
                    }
                });
            },
            queryTeamUser: function (projectId, storeCode) {
                return $http.get(Utils.ServiceURI.Address() + "api/projectusers/team", {
                    cache: false,
                    params: {
                        projectId: projectId,
                        storeCode: storeCode
                    }
                });
            },
            queryNoticeUser: function (projectId, storeCode) {
                return $http.get(Utils.ServiceURI.Address() + "api/projectusers/notice", {
                    cache: false,
                    params: {
                        projectId: projectId,
                        storeCode: storeCode
                    }
                });
            },
            queryViewers: function (projectId) {
                return $http.get(Utils.ServiceURI.Address() + "api/projectusers/viewers", {
                    cache: false,
                    params: {
                        projectId: projectId
                    }
                });
            },
            getMajorLeaseApprovers: function (flowCode, projectId) {
                return $http.get(Utils.ServiceURI.Address() + "api/Approvers/GetMajorLeaseApprovers", {
                    cache: false,
                    params: {
                        flowCode: flowCode,
                        projectId: projectId
                    }
                });
            },
            getRebuildApprovers: function (flowCode, projectId, isNeedEntity) {
                return $http.get(Utils.ServiceURI.Address() + "api/Approvers/GetRebuildApprovers", {
                    cache: false,
                    params: {
                        flowCode: flowCode,
                        projectId: projectId,
                        isNeedEntity:isNeedEntity
                    }
                });
            },
            getReimageApprovers: function (flowCode, projectId) {
                return $http.get(Utils.ServiceURI.Address() + "api/Approvers/GetReimageApprovers", {
                    cache: false,
                    params: {
                        flowCode: flowCode,
                        projectId: projectId
                    }
                });
            },
            getRenewalApprovers: function (flowCode, projectId) {
                return $http.get(Utils.ServiceURI.Address() + "api/Approvers/GetRenewalApprovers", {
                    cache: false,
                    params: {
                        flowCode: flowCode,
                        projectId: projectId
                    }
                });
            },
            getNotifyUsers: function (usCode, projectId, roleCodes) {
                return $http.get(Utils.ServiceURI.Address() + "api/projectusers/getNotifyUser", {
                    cache: false,
                    params: {
                        usCode: usCode,
                        projectId: projectId,
                        roleCodes: roleCodes
                    }
                });
            },
            getNecessaryNotifyUsers: function (usCode, flowCode) {
                return $http.get(Utils.ServiceURI.Address() + "api/NecessaryNotice/GetAvailableUserCodes/" + usCode + "/" + flowCode, {
                    cache: false
                });
            }
        };
    }
]);