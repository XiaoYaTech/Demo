module Utils {
    export class Generator {
        public static newGuid() {
            var guid = "";
            for (var i = 1; i <= 32; i++) {
                var n = Math.floor(Math.random() * 16.0).toString(16);
                guid += n;
                if ((i == 8) || (i == 12) || (i == 16) || (i == 20))
                    guid += "-";
            }
            return guid;
        }
    }

    export class Common {
        public static isInt(val: string): boolean {
            var regInt = /^-?[0-9]+(\.0*)?$/;
            return regInt.test(val);
        }

        public static resolveAlertMsg(val: string, isError: boolean): string {
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
        }

        public static isUserAction(from: string): boolean {
            var isUserAction = false;
            if (from
                && from == 'useraction') {
                isUserAction = true;
            }

            return isUserAction;
        }

        public static getParameterByName(name: string): string {
            name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
            var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
                results = regex.exec(location.hash ? location.hash : location.search);
            return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
        }

        public static filterDefaultDate(val: Date): Date {
            var result = null;
            var valMoment = moment(val);
            if (valMoment.isValid()
                && valMoment.year() != 1900) {
                result = val;
            }

            return result;
        }

        public static format(): string {
            if (arguments.length == 0)
                return null;

            var str = arguments[0];
            for (var i = 1; i < arguments.length; i++) {
                var re = new RegExp('\\{' + (i - 1) + '\\}', 'gm');
                str = str.replace(re, arguments[i]);
            }
            return str;
        }

        public static GetQueryString(key: string): string {
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
        }

    }

    ///Javascript中的浮点数精确计算
    ///Author: Stephen.Wang
    ///Date: 2014-07-09
    export class caculator {

        //加法
        public static plus(a: number, b: number): number {
            if (!a) {
                a = 0;
            };
            if (!b) {
                b = 0;
            }
            var s1 = a.toString(), s2 = b.toString(),
                m1 = s1.indexOf(".") > 0 ? s1.length - s1.indexOf(".") - 1 : 0,
                m2 = s2.indexOf(".") > 0 ? s2.length - s2.indexOf(".") - 1 : 0,
                m = Math.pow(10, Math.max(m1, m2));
            return (caculator.multiply(a, m) + caculator.multiply(b, m)) / m;
        }

        //乘法
        public static multiply(a, b): number {
            if (!a) {
                a = 0;
            };
            if (!b) {
                b = 0;
            }
            var s1 = a.toString(), s2 = b.toString(),
                m1 = s1.indexOf(".") > 0 ? s1.length - s1.indexOf(".") - 1 : 0,
                m2 = s2.indexOf(".") > 0 ? s2.length - s2.indexOf(".") - 1 : 0;
            return Number(s1.replace(".", "")) * Number(s2.replace(".", "")) / Math.pow(10, m1 + m2);
        }

        ///减法
        public static subtract(a, b): number {
            return caculator.plus(a, -b);
        }

        ///除法
        public static division(a, b): number {
            //return caculator.multiply(a, 1 / b);
            return a * 1 / b;
        }
    }

    export class ServiceURI {
        //webApi地址
        public static Address() {
            return "http://172.24.130.43:10083/";
            //本机
            return "http://localhost:10083/";
        }
        //web服务器地址
        public static WebAddress() {
            return "http://172.24.130.43:10082/";
            //本机
            return "http://localhost:10082/";
        }

        //基础框架Api地址
        public static FrameAddress() {
            return "http://172.24.130.43:10080/";
        }

        //基础框架Web地址
        public static FrameWebAddress() {
            return "http://172.24.130.43:10081/";
        }
        //附件服务器地址 
        public static AttachmentAddress() {
            return "http://1.1.2.5:9000/PMT/upload?action=download&&fileName=";
        }

        public static AppUri = window["AppUri"];

        public static ApiDelegate = ServiceURI.AppUri + "ApiDelegate.ashx";

    }


    export class Constants {
        public static ApiDelegate = Utils.ServiceURI.ApiDelegate;
        public static BaseUri = Utils.ServiceURI.FrameWebAddress();
        public static ServiceUri = Utils.ServiceURI.FrameAddress();
    }
}

declare function escape(str: any);