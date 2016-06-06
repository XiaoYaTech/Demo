///<reference path="/Scripts/jquery-1.8.0.js" />
///<reference path="/Scripts/jquery-ui-1.9.0.custom.js" />
///<reference path="/Scripts/MoyeBuyCom.js" />
///<reference path="/Scripts/json2.js" />

$(function () {
    $("#addSupplierDiv").dialog({
        autoOpen: false,
        height: 450,
        width: 400,
        modal: true,
        title: "<span id='dialogtitle'></span>",
        buttons: {
            "保存": function () {
                addUpdSupplier();
            },
            "取消": function () {
                cancel();
                $(this).dialog("close");
            }
        }
    });
});

function delSupplier(SupplierID) {
    if (SupplierID == null || SupplierID == "") {
        return;
    }
    if (!confirm("确认要删除供应商？操作不可逆.")) {
        return;
    }
    var parama = {};
    parama.SupplierID = SupplierID;
    $.ajax({
        url: "/admin/Supplier/Del",
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

function showAddWindow(SupplierID) {
    if (SupplierID != null) {
        $("#SupplierID").val(SupplierID);
        $("#dialogtitle").html("更新");
    }
    else
        $("#dialogtitle").html("添加");
    $("#addSupplierDiv").dialog("open");
}
function cancel() {
    $("#SupplierID").val("");
    $("#addSupplierDiv").hide();
}

function addUpdSupplier() {
    var strSupplierID = $("#SupplierID").val();
    var strSupplierName = $("#SupplierName").val();
    var strSupplierPersonName = $("#SupplierPersonName").val();
    var strSupplierPhoneNo = $("#SupplierPhoneNo").val();
    var strSupplierFax = $("#SupplierFax").val();
    var strSupplierAddress = $("#SupplierAddress").val();

    if (strSupplierName == "" || typeof (strSupplierName) == "undefined") {
        alert("名称不能为空");
        $("#SupplierName").focus();
        return;
    }
    if (strSupplierPersonName == "" || typeof (strSupplierPersonName) == "undefined") {
        alert("联系人不能为空");
        $("#SupplierPersonName").focus();
        return;
    }
    if (strSupplierPhoneNo == "" || typeof (strSupplierPhoneNo) == "undefined") {
        alert("电话不能为空");
        $("#SupplierPhoneNo").focus();
        return;
    }

    var parama = {};
    parama.SupplierID = strSupplierID;
    parama.SupplierName = strSupplierName;
    parama.SupplierPersonName = strSupplierPersonName;
    parama.SupplierFax = strSupplierFax;
    parama.SupplierPhoneNo = strSupplierPhoneNo;
    parama.SupplierAddress = strSupplierAddress;

    $.ajax({
        url: "/admin/supplier/addupd",
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