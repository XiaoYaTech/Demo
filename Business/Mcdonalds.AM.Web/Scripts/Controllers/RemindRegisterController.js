

/* Controllers */
var currentParentCode = "";
var remindRegisterControllers = angular.module('remindRegisterControllers', []);

remindRegisterControllers.controller('remindRegisterCtrl', [
    '$scope',
    'remindRegisterhandler',
    "messager",
    function ($scope, handler) {

      var remindRegister = new Object();
      remindRegister.PageIndex = 1;
      remindRegister.PageSize = 10;
      handler.query(remindRegister).$promise.then(function (data) {
              formInit();
              $scope.remindRegisterList = data;
      });

      var entity = new Object();
      entity.Code = "";
      entity.Name = "";
      entity.Id = 0;
      entity.Sequence = 0;
      $scope.entity = entity;

      $scope.showDicInfo = function (e) {

          var entity = new Object();
          entity.Code = "";
          entity.Name = "";
      
          entity.Sequence = 0;
          $scope.entity = entity;

          $("#tbInfo")[0].style.display = "";

      }
      $scope.cancel = function (e) {
          formInit();
      }

      $scope.search = function (e) {
          var condition = new Object();
          condition.Code = $("#txtCode").val();
          condition.Name = $("#txtName").val();
          condition.ModuleNameZHCN = $("#txtModuleName").val();
          condition.ModuleCode = $("#txtModuleCode").val();
          condition.PageIndex = 1;
          condition.PageSize = 10;
          var dicEntity = JSON.stringify(condition);
  
          handler.query(dicEntity).$promise.then(function (data) {
             
                  $scope.remindRegisterList = data;
                  formInit();
          });
      }

     


      $scope.updateSeq = function (e) {


          var txtseqList = $("input[type=text][name=txtSequence]", $("#tbDicList"));
          txtseqList.each(function (index, element) {
              $scope.remindRegisterList[index].Sequence = txtseqList[index].value;
          });
          var dicEntity = JSON.stringify($scope.remindRegisterList);

          handler.updateList(dicEntity).$promise.then(function (data) {
              $scope.search($scope.remindRegisterList);
          }, function (err) {
              messager.showMessage("错误", "fa-warning c_orange");
          });
      }

      $scope.deleteEntity = function (e) {

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
                          ids += "'" + $(this).val() + "'";
                          if (listLength != index + 1) {
                              ids += ",";
                          }
                      });

                      var dicEntity = new Object();
                      dicEntity.Ids = ids;
                      handler.deleteByIds(dicEntity).$promise.then(function (data) {

                          $scope.search($scope.remindRegisterList);

                      });
                  };
              });
          };
          return false;
      }

      $scope.save = function (e) {
     

          var dicEntity = JSON.stringify($scope.entity);
       
          handler.save(dicEntity).$promise.then(function (data) {
              $scope.search($scope.remindRegisterList);
          });
      }
      $scope.edit = function (e) {
          $scope.entity = e;
          $("#tbInfo")[0].style.display = "";
        
      }


  }]);


