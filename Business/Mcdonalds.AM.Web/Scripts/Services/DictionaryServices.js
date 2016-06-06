/*数据字典服务*/
var phonecatServices = angular.module('dictionaryServices', ['ngResource']);

phonecatServices.factory('dictionaryHandler', ['$resource',
  function ($resource) {
      //数据字典webapi的访问地址，:extentionURL表示需要追加的URL参数
      return $resource(Utils.ServiceURI.Address() + 'api/dictionary/:extentionURL', {}, {

          queryByParent: { method: 'GET', params: { extentionURL: 'query' }, isArray: true },
          //查询
          query: { method: 'POST', params: { extentionURL: 'QueryList' }, isArray: true },
          //查询上一级
          upperList: { method: 'POST', params: { extentionURL: 'QueryUpperList' }, isArray: true },
          //复杂查询
          search: { method: 'POST', params: { extentionURL: 'SearchList/1/10' }, isArray: true },
          //更新
          updateList: { method: 'POST', params: { extentionURL: 'UpdateList' }, isArray: false },
          //删除
          deleteByIds: { method: 'POST', params: { extentionURL: 'Delete' }, isArray: false },
          //保存
          saveDic: { method: 'POST', params: { extentionURL: 'Save' }, isArray: false },
          queryNormType: { method: 'GET', params: { extentionURL: 'NormType' }, isArray: false },
      });
  }]);
