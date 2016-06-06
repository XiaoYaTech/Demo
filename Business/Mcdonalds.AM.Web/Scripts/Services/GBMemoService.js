angular.module("mcd.am.service.GBMemo", ["ngResource"])
    .factory("GBMemoService", ["$resource",
        function ($resource) {
            return $resource(Utils.ServiceURI.Address() + 'api/:contorller/:action', {}, {
                saveGBMemo: {
                    method: 'POST',
                    params: { contorller: "GBMemo", action: 'SaveGBMemo' },
                    isArray: false
                },
                sendGBMemo: {
                    method: "POST",
                    params: { contorller: "GBMemo", action: "SendGBMemo" },
                    isArray: false
                }
            });
        }]);