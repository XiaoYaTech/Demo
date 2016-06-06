var remindServices = angular.module('mcd.am.service.remind', ['ngResource']);

remindServices.factory('remindService', ['$resource',
  function ($resource) {
      return $resource(Utils.ServiceURI.Address() + 'api/remind/:action', {}, {

          query: { method: 'POST', params: { action: 'QueryList' }, isArray: false },
          save: { method: 'POST', params: { action: 'Save' }, isArray: false },
      });
  }]);
