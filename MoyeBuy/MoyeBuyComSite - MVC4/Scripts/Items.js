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
$(function () {
    $("#BuyProdNum").val(1);
});
function Buy(prodid) {
    var num = $("#BuyProdNum").val();
    var price = $("#sumprice").val();
    if (num != "" && parseInt(num) > 0) {
        document.location = "/Cart/Index/" + prodid + "----" + num + "----" + price;
    }
    else {
        alert("商品数量不能小于0！")
        return;
    }
    
}
function addToCart(prodid) {
    var num = $("#BuyProdNum").val();
    var price = $("#sumprice").val();
    if (num != "" && parseInt(num) > 0) {
        document.location = "/Cart/AddToCart/" + prodid + "----" + num + "----" + price;
    }
    else {
        alert("商品数量不能小于0！")
        return;
    }
}
function countProd() {
    var myprice = $("#myprice").text();
    var num = $("#BuyProdNum").val();
    if(myprice!="" && num!="")
    {
        if(!isNaN(parseFloat(myprice)) && !isNaN(parseInt(num))) {
            var count = parseFloat(myprice) * parseInt(num);
            count = count.toFixed(2);
        }
    }
    $("#sumprice").val(count);
    $("#countRslt").html(count);
}