angular.module('dictionaryFilters', []).filter('isDirectoryFilter', function () {
    return function (input) {
        return input ? '目录' : '字典项';
    };
});