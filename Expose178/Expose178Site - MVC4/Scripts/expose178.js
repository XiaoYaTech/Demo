/// <reference path="jquery-1.8.0.js" />
/// <reference path="json2.js" />
/// <reference path="kindeditor.js" />

$(document).ready(function () {
    $(".Container").masonry({
        itemSelector: '.item',
        columnWidth: 240
    });
});