angular.module("mcd.am.service.approveDialog", ["ngResource"])
    .factory("ApproveDialogUserService", [
        "$resource", function($resource) {
            return $resource(Utils.ServiceURI.Address() + 'api/:contorller/:action', {}, {
                getProjectId: {
                    method: 'GET',
                    params: { contorller: "ApproveDialogUser", action: 'GetApproveDialogUsers'},
                    isArray: false
                },
                beginCreateMajorLease: {
                    method: "POST",
                    params: { contorller: "ApproveDialogUser", action: "SaveApproveDialogUsers" },
                    isArray: false
                }
            });
        }
    ]);