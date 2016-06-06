!function () {
    $.support.cors = true;
    var reg = /user-id=(.+?)($|&)/ig;
    var matches = document.URL.match(reg);
    var userid = "";
    if (matches && matches.length > 0) {
        var end = matches[0].lastIndexOf("&");
        if (end <= 0) {
            end = matches[0].length;
        }
        var userid = decodeURI(matches[0].substring(matches[0].indexOf('=') + 1, end));
    }
    var lang = Utils.Common.getParameterByName('request-locale');
    if (lang) {
        switch (lang) {
            case "zh-cn":
                lang = "zh";
                break;
            case "en-us":
                lang = "en";
                break;
        }
        document.cookie = 'Mcd_AM.LangTag=' + lang;

        //setTimeout(function () {
        //    var url = window.location.href.replace('request-locale', 'locale');
        //    console.log(url);
        //    window.location.href = url;
        //}, 1000);
    }
    head = document.getElementsByTagName('head').item(0);
    script = document.createElement('script');
    script.src = Utils.ServiceURI.AppUri + 'CurrentUserDefine.ashx?userid=' + userid;
    script.type = 'text/javascript';
    script.defer = true;
    void (head.appendChild(script));
    var timeout = null;
    var timeoutMessage = null;
    var timeoutMemo = null;
    window.resetVertionPos = function (scrollTop) {
        var $btn = $(".btn-project-versions"), $obj = $(".project-versions");
        if (timeout != null) {
            clearTimeout(timeout);
        }
        timeout = setTimeout(function () {
            if ($btn.is(":animated")) {
                $btn.stop();
            };
            if ($obj.is(":animated")) {
                $obj.stop();
            };
            $btn.animate({ top: 185 + scrollTop }, 200);
            $obj.animate({ top: 185 + scrollTop }, 200);
        }, 100);
    }
    window.resetMessagePos = function (scrollTop) {
        var $btn = $(".btn-project-message"), $obj = $(".project-message");
        if (timeoutMessage != null) {
            clearTimeout(timeoutMessage);
        }
        timeoutMessage = setTimeout(function () {
            if ($btn.is(":animated")) {
                $btn.stop();
            };
            if ($obj.is(":animated")) {
                $obj.stop();
            };
            $btn.animate({ top: 80 + scrollTop }, 200);
            $obj.animate({ top: 80 + scrollTop }, 200);

        }, 100);
    }
    window.resetAttachmentsMemoPos = function (scrollTop) {
        var $btn = $(".btn-project-memos"), $obj = $(".project-memos");
        var $btnVersions = $(".btn-project-versions");
        var top = 310;
        if ($btnVersions != null && $btnVersions.length == 0) {
            top = 185;
        }
        if (timeoutMemo != null) {
            clearTimeout(timeoutMemo);
        }
        timeoutMemo = setTimeout(function () {
            if ($btn.is(":animated")) {
                $btn.stop();
            };
            if ($obj.is(":animated")) {
                $obj.stop();
            };
            $btn.animate({ top: top + scrollTop }, 200);
            $obj.animate({ top: top + scrollTop }, 200);

        }, 100);
    }
    $(function () {
        $(document).on("click", ".btn-project-versions", function () {
            var $obj = $(".project-versions"), width = $obj.width();
            if (!$obj.is(":animated")) {
                if ($obj.is(":visible")) {
                    $obj.animate({ right: -width }, 200, function () {
                        $obj.hide();
                    });
                } else {
                    checkOtherPop(".btn-project-message", ".btn-project-memos");
                    $obj.show().animate({ right: 18 }, 200);
                }
            }
        });
        $(document).on("click", ".btn-project-message", function () {
            var $obj = $(".project-message"), width = $obj.width();
            if (!$obj.is(":animated")) {
                if ($obj.is(":visible")) {
                    $obj.animate({ right: -width }, 200, function () {
                        $obj.hide();
                    });
                } else {
                    checkOtherPop(".btn-project-versions", ".btn-project-memos");
                    $obj.show().animate({ right: 18 }, 200);
                }
            }
        });
        $(document).on("click", ".btn-project-memos", function () {
            var $obj = $(".project-memos"), width = $obj.width();
            if (!$obj.is(":animated")) {
                if ($obj.is(":visible")) {
                    $obj.animate({ right: -width }, 200, function () {
                        $obj.hide();
                    });
                } else {
                    checkOtherPop(".btn-project-versions", ".btn-project-message");
                    $obj.show().animate({ right: 18 }, 200);
                }
            }
        });
        $(document).on("click", ".project-status-con>.arrow", function () {
            var $obj = $(".project-status-con>.project-status"), $fa = $(".fa", this);
            if (!$obj.is(":animated")) {
                $(".project-status-con>.project-status").toggle(200, "linear", function () {
                    $fa.toggleClass("fa-angle-double-right fa-angle-double-left");
                });
            }
        });
    });
    function checkOtherPop() {
        for (var i = 0; i < arguments.length; i++) {
            var $btn = $(arguments[i]), $content = $btn.next();
            if ($content.is(":visible")) {
                $btn.click();
            }
        }
    }
}();
