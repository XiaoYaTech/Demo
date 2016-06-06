///<reference path="jquery-1.8.0.js" />
function RefreshValidateCode()
{
    $("#ImgValidateCode").attr("src","../ValidateCode/code.jpg?time="+(new Date()).getTime());
}
var t = n = 0, count;
$(document).ready(function ()
{
    count = $("#banner_list a").length;
    $("#banner_list a:not(:first-child)").hide();
    $("#banner_info").html($("#banner_list a:first-child").find("img").attr('alt'));
    $("#banner_info").click(function () { window.open($("#banner_list a:first-child").attr('href'), "_blank") });
    $("#banner li").click(function ()
    {
        var i = $(this).text() - 1; //获取Li元素内的值，即1，2，3，4
        n = i;
        if (i >= count) return;
        $("#banner_info").html($("#banner_list a").eq(i).find("img").attr('alt'));
        $("#banner_info").unbind().click(function () { window.open($("#banner_list a").eq(i).attr('href'), "_blank") })
        $("#banner_list a").filter(":visible").fadeOut(500).parent().children().eq(i).fadeIn(1000);
        document.getElementById("banner").style.background = "";
        $(this).toggleClass("on");
        $(this).siblings().removeAttr("class");
    });
    t = setInterval("showAuto()", 4000);
    $("#banner").hover(function () { clearInterval(t) }, function () { t = setInterval("showAuto()", 4000); });

    scroll(".mallMiddleBox", ".mallLeftImg", ".mallRightImg", ".mallUl", 3, false);
});
function showAuto()
{
    n = n >= (count - 1) ? 0 : ++n;
    $("#banner li").eq(n).trigger('click');
}

function scroll(box, left, right, img, speed, or)
{
    var box = $(box);
    var left = $(left);
    var right = $(right);
    var img = $(img).find('ul');
    var w = img.find('li').outerWidth(true);
    var s = speed;
    left.click(function ()
    {
        img.animate({ 'margin-left': -w }, function ()
        {
            img.find('li').eq(0).appendTo(img);
            img.css({ 'margin-left': 0 });
        });
    });
    right.click(function ()
    {
        img.find('li:last').prependTo(img);
        img.css({ 'margin-left': -w });
        img.animate({ 'margin-left': 0 });
    });
    if (or == true)
    {
        ad = setInterval(function () { right.click(); }, s * 1000);
        box.hover(function () { clearInterval(ad); }, function () { ad = setInterval(function () { right.click(); }, s * 1000); });
    }
}
function gologin() {
    document.location = "/account/logon";
}
function goregist() {
    document.location = "/account/register";
}
function logout() {
    document.location = "/account/LogOut";
}
function GoToCart() {
    document.location = "/Cart/Index";
}
function GoToMyAccount() {
    document.location = "/Account/Index";
}
function IsNumeric(strInput) {
    var i;
    var bResult;
    var intTemp;
    var intCountDot = 0;
    var intCountMinusSign = 0;
    var intCountPlusSign = 0

    strInput = trim(strInput);
    if (!strInput)
        return false;
    if (strInput.length > 0) {

        for (var i = 0; i < strInput.length; i++) {
            intTemp = strInput.charCodeAt(i);
            //ASCII 48-57:0-9, 46:., 43:+, 45:-
            if (!((intTemp >= 48 && intTemp <= 57) || (intTemp == 46) || (intTemp == 45) || (intTemp == 43))) {
                return false;
            }
            else {
                if (intTemp == 46) {
                    if (intCountDot == 0)
                        intCountDot = 1;
                    else
                        return false;
                }
                if (intTemp == 45) {
                    if (i > 0)
                        return false;
                    else {
                        if (intCountMinusSign == 0)
                            intCountMinusSign = 1;
                        else
                            return false;
                    }
                }

                if (intTemp == 43) {
                    if (i > 0)
                        return false;
                    else {
                        if (intCountPlusSign == 0)
                            intCountPlusSign = 1;
                        else
                            return false;
                    }
                }
            }
        }
    }
    else {
        return false;
    }
    return true;
}
function IsDecimal(strInput) {
    var bolResult = true;
    var i;
    var nokta = -1;
    var ind;
    if (strInput.indexOf(".") > 0) {
        nokta = strInput.indexOf(".");
    }
    for (i = 0; i < strInput.length; i++) {

        ind = strInput.substring(i, i + 1);
        if (ind < "0" || ind > "9") {
            if (i == nokta) {
                continue;
            }
            else {
                bolResult = false;
                break;
            }
        }

    }

    return bolResult;
}
function isInteger(s) {
    var i;
    for (var i = 0; i < s.length; i++) {
        // Check that current character is number.
        var c = s.charAt(i);
        if (((c < "0") || (c > "9"))) return false;
    }
    // All characters are numbers.
    return true;
}