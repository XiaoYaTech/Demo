var phonecatServices = angular.module('remindRegisterServices', ['ngResource']);

phonecatServices.factory('remindRegisterhandler', ['$resource',
  function ($resource) {
      return $resource(Utils.ServiceURI.Address() + 'api/remindRegister/:extentionURL', {}, {
          
          query: { method: 'POST', params: { extentionURL: 'QueryList' }, isArray: true },
          updateList: { method: 'POST', params: { extentionURL: 'UpdateList' }, isArray: false },
          save: { method: 'POST', params: { extentionURL: '' }, isArray: false },
          deleteByIds: { method: 'POST', params: { extentionURL: 'delete' }, isArray: false }
         
      });
  }]);
