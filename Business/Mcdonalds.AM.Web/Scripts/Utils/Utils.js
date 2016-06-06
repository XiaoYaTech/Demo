var Utils;
(function (Utils) {
    var Generator = (function () {
        function Generator() {
        }
        Generator.newGuid = function () {
            var guid = "";
            for (var i = 1; i <= 32; i++) {
                var n = Math.floor(Math.random() * 16.0).toString(16);
                guid += n;
                if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
                    guid += "-";
            }
            return guid;
        };
        return Generator;
    })();
    Utils.Generator = Generator;

    var Common = (function () {
        function Common() {
        }
        Common.isInt = function (val) {
            var regInt = /^-?[0-9]+(\.0*)?$/;
            return regInt.test(val);
        };

        Common.resolveAlertMsg = function (val, isError) {
            var result;
            var msgFormat;
            if (isError) {
                msgFormat = '{0}[[[失败]]]';
            } else {
                msgFormat = '{0}[[[成功]]]';
            }
            switch (val) {
                case 'Submit':
                case 'ReSubmit':
                    result = '[[[提交]]]';
                    break;
                case 'Save':
                    result = '[[[保存]]]';
                    break;
                case 'Return':
                    result = '[[[退回]]]';
                    break;
                case 'Approve':
                    result = '[[[审批]]]';
                    break;
                case 'Decline':
                    result = '[[[拒绝]]]';
                    break;
                case 'Recall':
                    result = '[[[撤回]]]';
                    break;
                default:
                    result = '[[[操作]]]';
            }
            return Common.format.call(this, msgFormat, result);
        };

        Common.isUserAction = function (from) {
            var isUserAction = false;
            if (from && from == 'useraction') {
                isUserAction = true;
            }

            return isUserAction;
        };

        Common.getParameterByName = function (name) {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"), results = regex.exec(location.hash ? location.hash : location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        };

        Common.filterDefaultDate = function (val) {
            var result = null;
            var valMoment = moment(val);
            if (valMoment.isValid() && valMoment.year() != 1900) {
                result = val;
            }

            return result;
        };

        Common.format = function () {
            if (arguments.length == 0)
                return null;

            var str = arguments[0];
            for (var i = 1; i < arguments.length; i++) {
                var re = new RegExp('\\{' + (i - 1) + '\\}', 'gm');
                str = str.replace(re, arguments[i]);
            }
            return str;
        };

        Common.GetQueryString = function (key) {
            var result = "";
            if (document.URL.indexOf("?") > 0) {
                var query = document.URL.substr(document.URL.indexOf("?") + 1).split("&");
                for (var i = 0, len = query.length; i < len; i++) {
                    var keyVal = query[i].split("=");
                    if (keyVal[0].toLowerCase() == key.toLowerCase()) {
                        result = keyVal[1];
                        break;
                    }
                }
            }
            return result;
        };
        return Common;
    })();
    Utils.Common = Common;

    ///Javascript中的浮点数精确计算
    ///Author: Stephen.Wang
    ///Date: 2014-07-09
    var caculator = (function () {
        function caculator() {
        }
        //加法
        caculator.plus = function (a, b) {
            if (!a) {
                a = 0;
            }
            ;
            if (!b) {
                b = 0;
            }
            var s1 = a.toString(), s2 = b.toString(), m1 = s1.indexOf(".") > 0 ? s1.length - s1.indexOf(".") - 1 : 0, m2 = s2.indexOf(".") > 0 ? s2.length - s2.indexOf(".") - 1 : 0, m = Math.pow(10, Math.max(m1, m2));
            return (caculator.multiply(a, m) + caculator.multiply(b, m)) / m;
        };

        //乘法
        caculator.multiply = function (a, b) {
            if (!a) {
                a = 0;
            }
            ;
            if (!b) {
                b = 0;
            }
            var s1 = a.toString(), s2 = b.toString(), m1 = s1.indexOf(".") > 0 ? s1.length - s1.indexOf(".") - 1 : 0, m2 = s2.indexOf(".") > 0 ? s2.length - s2.indexOf(".") - 1 : 0;
            return Number(s1.replace(".", "")) * Number(s2.replace(".", "")) / Math.pow(10, m1 + m2);
        };

        ///减法
        caculator.subtract = function (a, b) {
            return caculator.plus(a, -b);
        };

        ///除法
        caculator.division = function (a, b) {
            //return caculator.multiply(a, 1 / b);
            return a * 1 / b;
        };
        return caculator;
    })();
    Utils.caculator = caculator;

    var ServiceURI = (function () {
        function ServiceURI() {
        }
        //webApi地址
        ServiceURI.Address = function () {
            return "http://172.24.130.43:10083/";

            //本机
            return "http://localhost:10083/";
        };

        //web服务器地址
        ServiceURI.WebAddress = function () {
            return "http://172.24.130.43:10082/";

            //本机
            return "http://localhost:10082/";
        };

        //基础框架Api地址
        ServiceURI.FrameAddress = function () {
            return "http://172.24.130.43:10080/";
        };

        //基础框架Web地址
        ServiceURI.FrameWebAddress = function () {
            return "http://172.24.130.43:10081/";
        };

        //附件服务器地址
        ServiceURI.AttachmentAddress = function () {
            return "http://1.1.2.5:9000/PMT/upload?action=download&&fileName=";
        };

        ServiceURI.AppUri = window["AppUri"];

        ServiceURI.ApiDelegate = ServiceURI.AppUri + "ApiDelegate.ashx";
        return ServiceURI;
    })();
    Utils.ServiceURI = ServiceURI;

    var Constants = (function () {
        function Constants() {
        }
        Constants.ApiDelegate = Utils.ServiceURI.ApiDelegate;
        Constants.BaseUri = Utils.ServiceURI.FrameWebAddress();
        Constants.ServiceUri = Utils.ServiceURI.FrameAddress();
        return Constants;
    })();
    Utils.Constants = Constants;
})(Utils || (Utils = {}));
