var app = angular.module("amApp", [
     "mcd.am.modules"
]);
app.controller("loginController", [
    "$scope",
    "$parse",
    "$http",
    "$window",
    "messager",
    function ($scope, $parse, $http,$window, messager) {
        $scope.login = function () {
            var isValid = $parse("frmLogin.$valid")($scope);
            if (isValid) {
                $http.post(Utils.ServiceURI.AppUri+"Home/Login", {
                    userCode: $scope.UserCode
                }).success(function () {
                    $window.location.href = Utils.ServiceURI.AppUri + "Home/Index";
                }).error(function () {
                    messager.showMessage("[[[登录失败，请联系管理员]]]", "fa-warning c_red");
                });
            }
        };
    }
]);