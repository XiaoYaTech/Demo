$(function () {
    $("#startDate,#expireDate").datepicker();
});

function Generate() {
    if (document.getElementById("cardcount").value == "0") {
        alert("请选择生成卡片的张数。");
        return;
    }
    if (document.getElementById("preNum").value == "") {
        alert("请填写卡号前缀。");
        return;
    }
    if (document.getElementById("pwdtype").value == "") {
        alert("请选择密码类型。");
        return;
    }
    if (document.getElementById("cardAmount").value == "") {
        alert("请填写卡内金额。");
        return;
    }
    if (document.getElementById("startDate").value == "") {
        alert("请选择生效日期。");
        return;
    }
    if (document.getElementById("expireDate").value == "") {
        alert("请选择截止日期。");
        return;
    }
    var param = {};
    param.cardcount = document.getElementById("cardcount").value;
    param.preNum = document.getElementById("preNum").value;
    param.cardAmount = document.getElementById("cardAmount").value;
    param.startDate = document.getElementById("startDate").value;
    param.expireDate = document.getElementById("expireDate").value;

    $.ajax({
        url: "/Admin/GiftCard/GenerateProcess",
        data: param,
        type: "POST",
        datatype: "json",
        success: function (result) {
            if (result == "SUCCESS") {
                alert("保存成功.");
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