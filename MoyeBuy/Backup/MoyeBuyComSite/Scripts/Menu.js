$(function () {
    $(".CategorysItems li").bind("mouseover", function () {
        $(this).addClass("CategorysItemsliHover").removeClass("CategorysItemsli");
        var subCatDivID = $(this).attr("subCatDivID");
        $("#CategorysItemsDiv" + subCatDivID).addClass("CategorysItemsDiv");
        var disq = $(this).attr("disq");
        if (disq != "2")
            $("#" + subCatDivID).css("margin-top", (-36*(parseInt(disq)-1))+"px");
        else
            $("#" + subCatDivID).css("margin-top", "-36px");
        $("#" + subCatDivID).show();
    });
    $(".CategorysItems li").bind("mouseout", function () {
        $(this).addClass("CategorysItemsli").removeClass("CategorysItemsliHover");
        var subCatDivID = $(this).attr("subCatDivID");
        $("#CategorysItemsDiv" + subCatDivID).removeClass("CategorysItemsDiv");
        $("#" + subCatDivID).hide();
    });
});

function CloseMenu(obj) {
    $(obj).parent().hide();
}