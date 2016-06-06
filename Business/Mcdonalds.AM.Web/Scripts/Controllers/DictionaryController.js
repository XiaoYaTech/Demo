/*数据字典控制器*/

//数据绑定
function dataBindList(scope, data) {
    scope.dictionaryList = data;
    if (data != null && data.length > 0) {
        scope.parentCode = scope.dictionaryList[0].ParentCode;
    }
}
//初始化查询条件
function initCondition(pageIndex, pageSize, parentCode) {
    var dictionary = new Object();
    dictionary.PageIndex = pageIndex;
    dictionary.PageSize = pageSize;
    dictionary.ParentCode = parentCode;
    return dictionary;
}
//初始化实体对象
function initDictionary() {
    var entity = new Object();
    entity.NameENUS = "";
    entity.NameZHCN = "";
    entity.Value = "";
    entity.Code = "";
    entity.Id = 0;
    entity.IsDirectory = "true";
    entity.Sequence = 0;
    return entity;
}

//当前父节点编号
var currentParentCode = "";
//控制器
var phonecatControllers = angular.module('dictionaryControllers', []);
//初始化控制器
phonecatControllers.controller('DictionaryCtrl', [
    '$scope',
    'dictionaryHandler',
    "messager",
    function ($scope, dictionaryHandler, messager) {

      //var dictionary = new Object();
      //dictionary.PageIndex = 1;
      //dictionary.PageSize = 10;
      //dictionary.ParentCode = "root";
      var dicCondition = initCondition(1, 10, "root");
      dictionaryHandler.query(dicCondition).$promise.then(function (data) {
          dataBindList($scope, data);
      });

      //初始化实体对象（实体对象用于保存数据）
      $scope.dic = initDictionary();


      //打开数据添加界面
      $scope.showDicInfo = function (e) {

          $("#tbDicInfo")[0].style.display = "";
          $scope.dic = initDictionary();

      }
      //取消事件
      $scope.cancel = function (e) {
          $("#tbDicInfo")[0].style.display = "none";
      }

      //查询事件
      $scope.search = function (e) {
          var condition = initCondition(1, 10, $scope.parentCode);
          condition.Code = $("#txtCode").val();
          condition.NameENUS = $("#txtNameENUS").val();
          condition.NameZHCN = $("#txtNameZHCN").val();

          var dicEntity = JSON.stringify(condition);

          dictionaryHandler.search(dicEntity).$promise.then(function (data) {
              dataBindList($scope, data);
              formInit();
          });
      }

      //层级钻取事件
      $scope.toList = function (e) {
          var condition = initCondition(1, 10, e.Code);

          if (e.IsDirectory) {
              dictionaryHandler.query(condition).$promise.then(function (data) {
                  $scope.dictionaryList = data;
                  $scope.parentCode = e.Code;
                  formInit();
              });
          }
      }

      //返回事件
      $scope.returnList = function (e) {

          if ($scope.dictionaryList.ParentCode != "root") {
              var condition = initCondition(1, 10, $scope.parentCode);
              dictionaryHandler.upperList(condition).$promise.then(function (data) {
                  {
                      dataBindList($scope, data);
                      formInit();
                  }
              });
          }
      }

      //更新排序号
      $scope.updateSeq = function (e) {
          //获取当前列表数据的排序号
          var txtseqList = $("input[type=text][name=txtSequence]", $("#tbDicList"));
          txtseqList.each(function (index, element) {
              $scope.dictionaryList[index].Sequence = txtseqList[index].value;
          });
          var dicEntity = JSON.stringify($scope.dictionaryList);
          //进行更新
          dictionaryHandler.updateList(dicEntity).$promise.then(function (data) {
              $scope.search($scope.dictionaryList);
          }, function (err) {
              messager.showMessage("错误", "fa-warning c_orange");
          });
      }

      //删除
      $scope.delDic = function (e) {
          //获取勾选的数据的Id集合
          var cbDelList = $("input[type=checkbox][name=cbSelID]:checked", $("#tbDicList"));

          if (cbDelList.length == 0) {
              messager.showMessage("当前没有可操作项", "fa-warning c_orange");
              return false;
          } else {
              messager.confirm("您确定要批量执行此操作吗？", "fa-warning c_orange").then(function (result) {
                  if (result) {
                      var ids = "";
                      var listLength = cbDelList.length;
                      cbDelList.each(function (index, element) {
                          ids += $(this).val();
                          if (listLength != index + 1) {
                              ids += ",";
                          }
                      });

                      //进行更新
                      var dicEntity = new Object();
                      dicEntity.Ids = ids;
                      dictionaryHandler.deleteByIds(dicEntity).$promise.then(function (data) {
                          $scope.search($scope.dictionaryList);
                      });
                  }
              });
          }
          return false;
      }

      //保存数据
      $scope.save = function (e) {
          //设置数据的父节点编号为当前目录的父节点编号
          $scope.dic.ParentCode = $scope.parentCode;
          var dicEntity = JSON.stringify($scope.dic);
          dictionaryHandler.saveDic(dicEntity).$promise.then(function (data) {
              $scope.search($scope.dictionaryList);
          });
      }
      //打开编辑页面
      $scope.edit = function (entity) {
          $scope.dic = entity;
          //这里需要将boolean类型的值转为字符串否则下拉框绑定会出现问题
          var isDirectoryStr = $scope.dic.IsDirectory == true ? "true" : "false";
          $scope.dic.IsDirectory = isDirectoryStr;
          $("#tbDicInfo")[0].style.display = "";
      }
  }]);


