angular.module('ngUploadify', [])
.directive("snailUploadify", function () {
    return {
        require: '?ngModel',
        restrict: 'A',
        link: function ($scope, element, attrs, ngModel) {
            var opts = angular.extend({}, $scope.$eval(attrs.nluploadify));
            element.uploadify({
                'fileObjName': opts.fileObjName || 'upfile',
                fileTypeExts: opts.fileTypeExts || "*.*",
                fileTypeDesc: opts.fileTypeDesc || "所有文件",
                'auto': opts.auto != undefined ? opts.auto : true,
                'cancelImage': '/Scripts/Libs/JQuery/uploadify/uploadify-cancel.png',
                'swf': '/Scripts/Libs/JQuery/uploadify/uploadify.swf',
                scriptAccess: 'always',
                //这里目前不能重写上传附件的地址，默认为Api服务器的地址
                'uploader': Utils.ServiceURI.Address() + opts.uploader,//图片上传方法
                'buttonText': opts.buttonText || '本地图片',
                'width': opts.width || 80,
                'height': opts.height || 25,
                'onUploadSuccess': function (file, d, response) {
                   
                    if (ngModel) {
                        //var result = eval("[" + d + "]")[0];
                        //if (result != null && result != "null") {
                        //    $scope.$apply(function () {
                        //        ngModel.$setViewValue(result + "$" + Math.random());
                        //    });
                        //}
               
                        ngModel.$setViewValue(Math.random());
                        $scope.$parent.$apply();
                    }

                }
            });
        }
    };
});