///<reference path="/Scripts/jquery-1.8.0.js" />
///<reference path="/Scripts/jquery-ui-1.9.0.custom.js" />
///<reference path="/Scripts/MoyeBuyCom.js" />
///<reference path="/Scripts/json2.js" />
$(function () {
    $("#addCategoryDiv").dialog({
        autoOpen: false,
        height: 350,
        width: 400,
        modal: true,
        title: "<span id='dialogtitle'></span>",
        buttons: {
            "保存": function () {
                addUpdCategory();
            },
            "取消": function () {
                cancel();
                $(this).dialog("close");
            }
        }
    });
});
function delCategory(categoryid) {
    if (categoryid == null || categoryid == "") {
        return;
    }
    if (!confirm("确认要删除类别？操作不可逆.")) {
        return;
    }
    var parama = {};
    parama.strCategoryID = categoryid;
    $.ajax({
        url: "/admin/category/Del",
        data: parama,
        type: "POST",
        datatype: "json",
        success: function (result) { 
            if (result == "SUCCESS") {
                //alert("删除成功.");
                window.location.reload();
            }
            else
                alert("删除出错.");
        },
        error: function (result) {

        },
        beforeSend: function () {

        }
    });
}

function showAddWindow(categoryid) {
    if (categoryid != null) {
        $("#CategoryID").val(categoryid);
        $("#CategoryName").val()
        $("#CategoryDesc").val()
        $("#dialogtitle").html("更新");
    }
    else
        $("#dialogtitle").html("添加");

    $("#addCategoryDiv").dialog("open");
    
}
function cancel() {
    $("#CategoryID").val("");
    $("#addCategoryDiv").hide();
}

function addUpdCategory() {
    var strCategoryID = $("#CategoryID").val();
    var strCategoryName = $("#CategoryName").val();
    var strCategoryDesc = $("#CategoryDesc").val();

    if (strCategoryName == "" || typeof(strCategoryName) == "undefined") {
        alert("类别名称不能为空");
        $("#CategoryName").focus();
        return;
    }
    if (strCategoryDesc == "" || typeof (strCategoryDesc) == "undefined") {
        alert("类别描述不能为空");
        $("#CategoryDesc").focus();
        return;
    }
    var parama = {};
    parama.CategoryId = strCategoryID;
    parama.CategoryName = strCategoryName;
    parama.CategoryDesc = strCategoryDesc;

    $.ajax({
        url: "/admin/category/addupd",
        data: parama,
        type: "POST",
        datatype: "json",
        success: function (result) {
            if (result == "SUCCESS") {
                //alert("保存成功.");
                window.location.reload();
            }
            else
                alert("保存出错.");
        },
        error: function (result) {

        },
        beforeSend: function () {

        }
    });
}