/// <reference path="../Libs/Angular/angular.d.ts" />
var app = angular.module('myApp', ['ui.bootstrap', 'nttmnc.fx.modules', 'mcd.am.modules']);
app.config([
    "$sceProvider", function ($sceProvider) {
        $sceProvider.enabled(false);
    }]);
app.run(function () {
});
