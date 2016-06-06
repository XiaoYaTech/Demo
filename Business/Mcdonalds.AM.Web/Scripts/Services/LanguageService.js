var languageService = angular.module('mcd.am.service.language', ['ngResource']);

languageService.factory('languageService', ['$resource',
  function ($resource) {
      return $resource(Utils.ServiceURI.WebAddress() + 'Home/:action', {}, {

          setLanguage: { method: 'POST', params: { action: 'SetLanguage' }, isArray: false }

      });
  }]);
