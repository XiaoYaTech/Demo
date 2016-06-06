///<reference path="jquery-1.8.0.js" />
$(function () {
    $(".CategorysItems").hide();
    $(".CategorysTitle").hover(function () {
        $(".CategorysItems").show();
    }, function () {
        $(".CategorysItems").hide();
    });
    $(".CategorysItems").hover(function () {
        $(".CategorysItems").show();
    }, function () {
        $(".CategorysItems").hide();
    });
});