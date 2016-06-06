/// <reference path="jquery-1.8.0.js" />
/// <reference path="json2.js" />
/// <reference path="kindeditor.js" />

var editor;
var KeditorOptions = {
    resizeType: 1,
    allowPreviewEmoticons: false,
    allowImageUpload: false,
    items: [
				'fontname', 'fontsize', '|', 'forecolor', 'hilitecolor', 'bold', 'italic', 'underline',
				'removeformat', '|', 'justifyleft', 'justifycenter', 'justifyright', 'insertorderedlist',
				'insertunorderedlist', '|', 'emoticons', 'image', 'link'
           ]
};
KindEditor.ready(function (K) {
    editor = K.create("#id_editor", KeditorOptions);
});

function SubmitArt() {
    var title = $("#title").val();
    var content = editor.html();
    if (title == "") {
        alert("请填写揭露文章的标题.");
        $("#title").focus();
        return;
    }
    if (content == "") {
        alert("请填写揭露的内容.");
        editor.focus();
        return;
    }
    var isDraft = $("#IsDraft option:selected").val();
    var strAritcleType = $("#AritcleType option:selected").val();
    if (strAritcleType == "") {
        alert("请选择类型.");
        $("#AritcleType").focus();
        return;
    }
    var strReadRole = $("#ReadRole option:selected").val();
    if (strReadRole == "") {
        alert("请选阅读权限.");
        $("#ReadRole").focus();
        return;
    }

    var param = {};
    param["ArticleTile"] = escape(title);
    param["ArticleBody"] = escape(content);
    param["AritcleTypeCode"] = strAritcleType;
    param["ReadRoleTypeCode"] = strReadRole;
    param["IsDraft"] = isDraft;
    $.ajax({
        url: "/Article/Add",
        data: param,
        type: "POST",
        datatype: "json",
        success: function (result) {
            if (result == "SUCC") {
                alert("添加新揭露成功！");
                document.location.href = "/TitleList.html";
            }
            else {
                alert("保存失败!");
            }
        },
        error: function (result) {

        },
        beforeSend: function () {

        }
    });
}