/// <reference path="../../Libs/moment/moment.d.ts" />
/// <reference path="../../Libs/Angular/angular.d.ts" />
/// <reference path="../../Libs/Angular/Plugins/angular-animate.d.ts" />
/// <reference path="../../Libs/Angular/Plugins/angular-cookies.d.ts" />
/// <reference path="../../Libs/Angular/Plugins/angular-mocks.d.ts" />
/// <reference path="../../Libs/Angular/Plugins/angular-resource.d.ts" />
/// <reference path="../../Libs/Angular/Plugins/angular-route.d.ts" />
/// <reference path="../../Libs/Angular/Plugins/angular-sanitize.d.ts" />
/// <reference path="../../Libs/Angular/Plugins/angular-scenario.d.ts" />
/// <reference path="../../Libs/Angular/Plugins/angular-ui-bootstrap.d.ts" />
/// <reference path="../../Libs/JQuery/jquery.d.ts" />
/// <reference path="../../Libs/AjaxQueue.d.ts" />
/// <reference path="../../Utils/Utils.ts" />
/// <reference path="../../Utils/CurrentUser.ts" />
angular.module("mcd.am.notices.service", []).factory("amNoticesService", [
    "$http",
    function ($http) {
        var service = {
            getItemsList: function (pageSize, pageIndex, searchCondition) {
                var receiver = window["currentUser"].Code;

                return $http.post(Utils.ServiceURI.Address() + "api/notices/list/" + receiver + "/" + pageSize + "/" + pageIndex, {
                    cache: false,
                    params: {
                        //Status: status
                        condtion: searchCondition
                    }
                });
            },
            getItemDetail: function (refId) {
                var receiver = window["currentUser"].Code;
                return $http.get(Utils.ServiceURI.Address() + "api/notices/detail", {
                    cache: false,
                    params: {
                        noticeId: refId,
                        receiver: receiver
                    }
                });
            }
        };
        return service;
    }
]);
