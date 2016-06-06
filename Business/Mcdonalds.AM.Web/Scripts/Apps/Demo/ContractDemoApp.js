var app = angular.module('amApp', ['ui.bootstrap', 'nttmnc.fx.modules', 'mcd.am.modules']);
app.config([
"$sceProvider", function ($sceProvider) {
    $sceProvider.enabled(false);
}]);
app.run(["$rootScope",function($rootScope){
    $rootScope.contract = {};
}]);
