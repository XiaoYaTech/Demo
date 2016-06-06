/*===========================================================
 * Author       :   Stephen.Wang
 * CreateTime   :   7/28/2014 11:14:35 AM
 * FileName     :   angular-plupload.js
 * Version      :   Ver 1.0.00
 * CopyRights © NTTDATA China Stephen.Wang
 *=========================================================*/
!function () {
    angular.module("ngPlupload", ["mcd.am.modules"])
        .directive("pluploadSimple", ["messager", function (messager) {
            return {
                restrict: "A",
                link: function ($scope, element, attrs) {
                    var id = element.prop("id");
                    if (!id) {
                        id = Utils.Generator.newGuid();
                        element.prop("id", id);
                    }
                    var uploadUrl = Utils.ServiceURI.Address() + attrs.uploadUrl;
                    var mmTypes = $scope.$eval(attrs.mmTypes || "[{'title':'All Files','extensions':'*'}]");
                    var multiSelection = $scope.$eval(attrs.multiSelection || "true");
                    var maxFileSize = attrs.maxFileSize || "20mb";
                    var errorMsg = attrs.errorMsg;
                    var isError = false;
                    var uploader = new plupload.Uploader({
                        runtimes: 'html5,flash,silverlight,html4',
                        browse_button: id,
                        container: element.parent().get(0),
                        multi_selection: multiSelection,
                        url: uploadUrl,
                        flash_swf_url: Utils.ServiceURI.AppUri + 'Content/plupload/Moxie.swf',
                        silverlight_xap_url: Utils.ServiceURI.AppUri + 'Content/plupload/Moxie.xap',
                        filters: {
                            max_file_size: maxFileSize,
                            mime_types: mmTypes
                        },
                        init: {
                            PostInit: function () {
                            },
                            FilesAdded: function (up, files) {
                                if ($scope[attrs.beforeUpload]) {
                                    $scope[attrs.beforeUpload](up, files);
                                }
                                uploader.start();
                                messager.blockUI("正在上传中，请稍等...");
                            },
                            UploadComplete: function (up, files) {
                                if (!isError) {
                                    messager.unBlockUI();
                                    $scope[attrs.uploadFinished](up, files);
                                }
                                isError = false;
                            },
                            Error: function (up, err) {
                                isError = true;
                                messager.unBlockUI();
                                var response = err.response ? $.parseJSON(err.response) : null;
                                if (response && response.ExceptionMessage) {
                                    messager.showMessage(response.ExceptionMessage || errorMsg, "fa-warning c_red");
                                }
                                else if (err.message) {
                                    messager.showMessage(err.message+'请刷新浏览器后重试。', "fa-warning c_red");
                                } else {
                                    messager.showMessage(errorMsg, "fa-warning c_red");
                                }
                                $scope[attrs.uploadFailed] && $scope[attrs.uploadFailed](up, err);
                            }
                        }
                    });

                    uploader.init();
                }
            };
        }]);
}();
