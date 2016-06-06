/********************************************************/
/* Author:  Stephen Wang                                */
/* Date:    2013-12-02                                  */
/********************************************************/
/// <reference path="JQuery/jquery-1.10.2.js" />
!function (win, $) {
    var hookMethod = function (obj, method, hookLogic, beforeMethod) {
        var _method = obj[method];
        if (!!_method) {
            obj[method] = function () {
                if (beforeMethod) {
                    hookLogic.apply(this, arguments);
                    _method.apply(this, arguments);
                } else {
                    _method.apply(this, arguments);
                    hookLogic.apply(this, arguments);
                }
            }
        }
    };
    win.AjaxQueue = function (name) {
        this._name = name;
        this._requests = [{}];
        $(document).queue(this._name, []);
    }
    win.AjaxQueue.prototype = {
        Request: function (key, xhrOption) {
            /// <summary>将Ajax请求放入队列</summary>
            /// <param name="key" type="String">Ajax请求标示，用于管理Ajax状态</param>
            /// <param name="xhrOption" type="Object Literal">JQuery Ajax对象参数选项</param>
            var self = this;
            if (!!xhrOption.complete) {
                hookMethod(xhrOption, "complete", ajaxHook, false);
            } else {
                hookMethod(xhrOption, "success", ajaxHook, false);
                hookMethod(xhrOption, "error", ajaxHook, false);
            };

            function ajaxHook() {
                $(document).dequeue(self._name);

            }

            $(document).queue(self._name, function () {
                self.Abort(key);//取消未完成的相同请求
                xhr = $.ajax(xhrOption);
                self._requests.push({
                    key: key,
                    xhr: xhr
                });
            });

            return self;
        },
        Abort: function (key) {
            var self = this;
            $.each(self._requests || [], function (i, req) {
                if (req.key === key) {
                    try {
                        req.xhr.abort();
                    }
                    catch (err) {

                    }
                }
            });
        },
        Run: function () {
            $(document).dequeue(this._name);
            return this;
        }
    };
}(window,jQuery);