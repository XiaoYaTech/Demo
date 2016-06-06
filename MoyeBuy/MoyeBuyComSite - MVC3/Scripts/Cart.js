///<reference path="jquery-1.8.0.js" />
///<reference path="jquery-ui-1.9.0.custom.js" />
///<reference path="MoyeBuyCom.js" />
///<reference path="json2.js" />

$(function () {
    $("#CheckoutResult").dialog({
        autoOpen: false,
        height: 350,
        width: 400,
        modal: true,
        title: "<span id='dialogtitle'></span>",
        buttons: {
            "继续": function () {

            },
            "取消": function () {
                $(this).dialog("close");
            }
        }
    });
});

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

function CheckGiftCard() {
    var getProdMothed = $("#GetProdMothed option:selected").val();
    if (getProdMothed == "") {
        alert("请选择配送方式");
        document.getElementById("GetProdMothed").focus();
        return;
    }

    var GiftCardNo = $("#GiftCardNo").val();
    if (GiftCardNo == "") {
        alert("请输入购物卡卡号");
        document.getElementById("GiftCardNo").select();
        return;
    }

    var GiftCardNo = $("#GiftCardPwd").val();
    if (GiftCardNo == "") {
        alert("请输入购物卡密码");
        document.getElementById("GiftCardPwd").select();
        return;
    }
}
function delProd(id) {
    var parama = {};
    parama.id = id;
    $.ajax({
        url: "/Cart/DelProd",
        data: parama,
        type: "POST",
        datatype: "json",
        success: function (result) {
            if (result != "FAIL") {
                document.location = "/Cart/Index";
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
function clear() {
    var parama = {};
    $.ajax({
        url: "/Cart/ClearCart",
        data: parama,
        type: "POST",
        datatype: "json",
        success: function (result) {
            if (result != "FAIL") {
                document.location="/Cart/Index";
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

function changeProvince(province) {
    var parama = {};
    parama.provinceId = province.value;
    $.ajax({
        url: "/Cart/GetCity",
        data: parama,
        type: "POST",
        datatype: "json",
        success: function (result) {
            if (result != "FAIL") {
                $("#divCity").html(result);
            }
            else
                alert("网络连接出错.");
        },
        error: function (result) {

        },
        beforeSend: function () {

        }
    });
}
function chagneCity(city) {
    var parama = {};
    parama.cityid = city.value;
    $.ajax({
        url: "/Cart/GetDistrict",
        data: parama,
        type: "POST",
        datatype: "json",
        success: function (result) {
            if (result != "FAIL") {
                $("#divDistrict").html(result);
            }
            else
                alert("网络连接出错.");
        },
        error: function (result) {

        },
        beforeSend: function () {

        }
    });
}
function subProd() {
    var prodnum = document.getElementById("ProdCount").value;
    if (prodnum <= 0)
        return;
    prodnum = parseInt(prodnum);
    if (isNaN(prodnum)) {
        alert("请填写数字")
        return;
    }
    prodnum = prodnum - 1;
    document.getElementById("ProdCount").value = prodnum;
}
function addProd() {
    var prodnum = document.getElementById("ProdCount").value;
    prodnum = parseInt(prodnum);
    if (isNaN(prodnum)) {
        alert("请填写数字")
        return;
    }
    prodnum = prodnum + 1;
    document.getElementById("ProdCount").value = prodnum;
}
function checknum(obj) {
    var val = obj.value;
    if (isInteger(val)) {
        return;
    }
    else {
        alert("请输入数字");
        obj.select();
        return;
    }
}