///<reference path="/Scripts/jquery-1.8.0.js" />
///<reference path="/Scripts/jquery-ui-1.9.0.custom.js" />
///<reference path="/Scripts/MoyeBuyCom.js" />
///<reference path="/Scripts/json2.js" />

$(function () {
    $("#addUserDiv").dialog({
        autoOpen: false,
        height: 350,
        width: 300,
        modal: true,
        title:"<span id='dialogtitle'></span>",
        buttons: {
            "保存": function () {
                addUpdUser();
            },
            "取消": function () {
                cancel();
                $(this).dialog("close");
            }
        }
    });
});

function adduser() {
    $("#addUserDiv").dialog("open");
    $("#dialogtitle").html("添加");
}
function cancel() {
    $("#addUserDiv").hide();
}
function showAddWindow(UID) {
    if (UID != null) {
        $("#UID").val(UID);
    }
    $("#addUserDiv").dialog("open");
    $("#dialogtitle").html("更新");
}
function delUser(UID) {
    if (UID == null || UID == "") {
        return;
    }
    if (!confirm("确定删除账号？")) {
        return;
    }
    var parama = {};
    parama.UID = UID;
    $.ajax({
        url: "/admin/Account/DelUser",
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
function addUpdUser() {
    var strUID = $("#UID").val();
    var strUserName = $("#UserName").val();
    var strpwd = $("#pwd").val();
    var strpwdcomfirm = $("#pwdcomfirm").val();
    var strUserName = $("#UserName").val();
    var strUserEmail = $("#UserEmail").val();
    var strUserPhoneNo = $("#UserPhoneNo").val();
    var strRole = $("#role").val();
    var strIsEffective = $("#IsEffective option:selected").val();

    if (strpwd == "" || typeof (strpwd) == "undefined") {
        alert("密码不能为空");
        $("#pwd").focus();
        return;
    }
    if (strpwdcomfirm == "" || strpwd != strpwdcomfirm) {
        alert("两次输入密码不同");
        $("#pwd").focus();
        return;
    }
    if (strUserName == "" || typeof (strUserName) == "undefined") {
        alert("账户名不能为空");
        $("#UserName").focus();
        return;
    }
    if (strUserEmail == "" || typeof (strUserEmail) == "undefined") {
        alert("登陆账号不能为空");
        $("#UserEmail").focus();
        return;
    }
    if (strUserPhoneNo == "" || typeof (strUserPhoneNo) == "undefined") {
        alert("电话不能为空");
        $("#UserPhoneNo").focus();
        return;
    }
    if (strRole == "" || typeof (strRole) == "undefined") {
        alert("权限不能为空");
        $("#role").focus();
        return;
    }
    
    var parama = {};
    parama.MoyeBuyComUserID = strUID;
    parama.MoyeBuyComUserName = strUserName;
    parama.UserPhoneNo = strUserPhoneNo;
    parama.MoyeBuyComEmail = strUserEmail;
    parama.RoleID = strRole;
    parama.IsEffective = strIsEffective=="1"?"true":"false";
    parama.pwd=strpwd;

    $.ajax({
        url: "/admin/account/Register",
        data: parama,
        type: "POST",
        datatype: "json",
        success: function (result) {
            if (result != "FAIL") {
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
