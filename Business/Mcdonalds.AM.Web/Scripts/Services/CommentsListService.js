angular.module("mcd.am.service.commentsList", ["ngResource"])
    .factory("commentsListService", [
        "$resource",
        function ($resource) {
            return $resource(Utils.ServiceURI.Address() + 'api/:contorller/:action', {}, {
                queryCommentsList: {
                    method: "POST",
                    params: { contorller: "NotificationMessage", action: 'Query' },
                    isArray: false
                },
                queryProjectCommentsList: {
                    method: "POST",
                    params: { contorller: "NotificationMessage", action: 'QueryProjectComments' },
                    isArray: false
                },
                getFlowCodeList: {
                    method: 'GET',
                    params: { contorller: "NotificationMessage", action: 'GetFlowCodeList' },
                    isArray: true
                },
                getCreatorList: {
                    method: "GET",
                    params: { contorller: "NotificationMessage", action: "GetCreatorList" },
                    isArray: true
                },
                getCreateFlowInfo: {
                    method: "GET",
                    params: { contorller: "NotificationMessage", action: "GetCreateFlowInfo" },
                    isArray: false
                },
                exportExcel: {
                    method: "POST",
                    params: { contorller: "NotificationMessage", action: "ExportExcel" },
                    isArray: false
                }
            });
        }
    ]);