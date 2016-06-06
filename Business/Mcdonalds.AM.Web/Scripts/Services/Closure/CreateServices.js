/*数据字典服务*/
var phonecatServices = angular.module('closureCreateServices', ['ngResource']);

phonecatServices.factory('closureCreateHandler', ['$resource',
  function ($resource) {
      //数据字典webapi的访问地址，:extentionURL表示需要追加的URL参数
      return $resource(Utils.ServiceURI.Address() + 'api/:extentionURL', {}, {
          //return $resource(':extentionURL', {}, {

          //查询
          queryDictionary: { method: 'Post', params: { extentionURL: 'dictionary/QueryList' }, isArray: true },

          beginCreate: { method: "POST", params: { extentionURL: "closure/BeginCreate/Closure/" }, isArray: false },

          saveClosureInfo: { method: "POST", params: { extentionURL: "closure" }, isArray: false },

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

          //保存DL数据
          saveSummaryClosure: { method: "Summary", params: { extentionURL: "closure" }, isArray: false }
      });
  }]);

phonecatServices.factory('summaryClosureHandler', ['$resource',
  function ($resource) {
      //数据字典webapi的访问地址，:extentionURL表示需要追加的URL参数
      return $resource(Utils.ServiceURI.Address() + 'api/:control/:action', {}, {
          //保存DL数据
          saveSummaryClosure: { method: "POST", params: { control: "closure", action: "SummaryClosure" }, isArray: false }
      })
  }])
