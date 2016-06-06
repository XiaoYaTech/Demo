angular.module("mcd.am.service.reopenMemo", ["ngResource"])
    .factory("reopenMemoService", ["$resource",
        function ($resource) {
            return $resource(Utils.ServiceURI.Address() + 'api/:contorller/:action', {}, {
                saveReopenMemo: {
                    method: 'POST',
                    params: { contorller: "ReopenMemo", action: 'SaveReopenMemo' },
                    isArray: false
                },
                sendReopenMemo: {
                    method: "POST",
                    params: { contorller: "ReopenMemo", action: "SendReopenMemo" },
                    isArray: false
                },
                querySaveable: {
                    method: "GET",
                    params: { contorller: "ReopenMemo", action: "QuerySaveable" },
                    isArray: false
                },
                getSelectYearMonth: {
                    method: "GET",
                    params: { contorller: "ReopenMemo", action: "GetSelectYearMonth" },
                    isArray: true
                }
            });
        }]);