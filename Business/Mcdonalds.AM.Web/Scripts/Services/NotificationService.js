var notificationService = angular.module("mcd.am.service.notification", ["ngResource"]);

notificationService.factory("notificationService", ["$resource",

    function ($resource) {
        return $resource(Utils.ServiceURI.Address() + 'api/Notification/:action', {}, {

            query: { method: 'POST', params: { action: 'Query' }, isArray: false },
            save: { method: 'POST', params: { action: 'Save' }, isArray: false }
        });

    }]);