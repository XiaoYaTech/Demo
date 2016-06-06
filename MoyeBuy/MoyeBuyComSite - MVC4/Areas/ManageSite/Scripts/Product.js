var editor;
var KeditorOptions = {
    resizeType: 1,
    allowPreviewEmoticons: true,
    allowImageUpload: true,
    allowFileManager: false,
    formatUploadUrl:false,
    uploadJson: "/Admin/Product/UploadFile",
    items: [
				'fontname', 'fontsize', '|', 'forecolor', 'hilitecolor', 'bold', 'italic', 'underline',
				'removeformat', '|', 'justifyleft', 'justifycenter', 'justifyright', 'insertorderedlist',
				'insertunorderedlist', '|', 'emoticons', 'image', 'link'
           ]
};
KindEditor.ready(function (K) {
    editor = K.create("#productDesc", KeditorOptions);
    K('#productImg').click(function () {
        editor.loadPlugin('image', function () {
            editor.plugin.imageDialog({
                showRemote: false,
                imageUrl: K('#productImgUrl').val(),
                clickFn: function (url, title, width, height, border, align) {
                    K('#productImgUrl').val(url);
                    editor.hideDialog();
                }
            });
        });
    });
});


function AddProd(strProdctID) {
    var strURL = "/admin/product/AddProd";
    if (strProdctID != null && strProdctID != "") {
        strURL = "/admin/product/UpdtProd";
    }

    var strProductName = $("#productName").val();
    var strProductDesc = editor.html();
    var strProductSpec = $("#productSpec").val();
    var strProductImgs = $("#productImgUrl").val();
    var strMoyeBuyPrice = $("#productPrice").val();
    var strMarketPrice = $("#productMarketPrice").val();
    var strIsSellHot = $("#isSellHot option:selected").val()=="1"?"true":"false";
    var strIsOnSell = $("#isOnSell option:selected").val()=="1"?"true":"false";
    var strCategoryId = $("#productCategory option:selected").val();
    var strSupplierID = $("#productSupplier option:selected").val();
    var strProductCount = $("#productCount").val();


    if (strProductName == "" || typeof (strProductName) == "undefined") {
        alert("名称不能为空");
        $("#productName").focus();
        return;
    }

    if (strProductSpec == "" || typeof (strProductSpec) == "undefined") {
        alert("规格不能为空");
        $("#productSpec").focus();
        return;
    }

    if (strMoyeBuyPrice == "" || typeof (strMoyeBuyPrice) == "undefined") {
        alert("价格不能为空");
        $("#productPrice").focus();
        return;
    }

    if (strIsOnSell == "" || typeof (strIsOnSell) == "undefined") {
        alert("请选择是否立即上架");
        $("#isOnSell").focus();
        return;
    }

    if (strCategoryId == "" || typeof (strCategoryId) == "undefined") {
        alert("请选择商品类别");
        $("#productCategory").focus();
        return;
    }

    if (strCategoryId == "" || typeof (strCategoryId) == "undefined") {
        alert("请选择供应商");
        $("#productSupplier").focus();
        return;
    }

    if (strProductCount == "" || typeof (strProductCount) == "undefined") {
        alert("请填写库存");
        $("#productCount").focus();
        return;
    }

    var parama = {};
    parama.ProductId = strProdctID;
    parama.ProductName = strProductName;
    parama.ProductDesc = strProductDesc;
    parama.ProductSpec = strProductSpec;
    parama.ProductImgs = strProductImgs;
    parama.MoyeBuyPrice = strMoyeBuyPrice;
    parama.MarketPrice = strMarketPrice;
    parama.IsSellHot = strIsSellHot;
    parama.IsOnSell = strIsOnSell;
    parama.CategoryId = strCategoryId;
    parama.SupplierID = strSupplierID;
    parama.ProductCount = strProductCount;
    

    $.ajax({
        url: strURL,
        data: parama,
        type: "POST",
        datatype: "json",
        success: function (result) {
            if (result == "SUCCESS") {
                //alert("保存成功.");
                window.location="/admin/product";
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

function ajustPrice() {

}
function delProd(strProdctID) {
    if (!confirm("确认要删除商品？操作不可逆.")) {
        return;
    }
    var parama = {};
    if (strProdctID == null)
        return;
    parama.prodID = strProdctID;
    $.ajax({
        url: "/admin/product/Del",
        data: parama,
        type: "POST",
        datatype: "json",
        success: function (result) { 
            if (result == "SUCCESS") {
                alert("删除成功.");
                window.location="/admin/product";
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