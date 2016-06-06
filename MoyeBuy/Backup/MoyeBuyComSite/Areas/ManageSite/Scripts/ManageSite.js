///<reference path="/Scripts/jquery-1.8.0.js" />
///<reference path="/Scripts/MoyeBuyCom.js" />
///<reference path="/Scripts/json2.js" />
function toggleMenu(obj) {
    if (obj != null) {
        var subMenu = $(obj).parent().next().children();
        if (subMenu.is(":hidden"))
            subMenu.show();
        else
            subMenu.hide();

    }
    else {

        if ($(".Menu b").css("background-image").indexOf("menu_minus.gif") != -1) {
            $(".AdminCategorys li ul").each(function () {
                $(this).hide();
            });
            $(".Menu b").css("background-image", "url('/Areas/ManageSite/Content/Imgs/menu_plus.gif')");

        }
        else {
            $(".AdminCategorys li ul").each(function () {
                $(this).show();
            });
            $(".Menu b").css("background-image", "url('/Areas/ManageSite/Content/Imgs/menu_minus.gif')");
        }

    }
}
function addNewMenuRow(menuType) {
    if (menuType == "adminMenu") {
        var rowNum = $("#MainMenuTable tr").length - 2;

        var Row = "<tr><td align=\"center\">" + (rowNum + 1) + "<input type=\"hidden\" name=\"MenuID" + rowNum + "\" id=\"MenuID" + rowNum + "\" value=\"\" /></td><td><input type=\"text\" name=\"NavItem" + rowNum + "\" id=\"NavItem" + rowNum + "\" value=\"\" maxlength=\"50\" /></td>";
        Row += "<td><input type=\"text\" name=\"MenuStyle" + rowNum + "\" id=\"MenuStyle" + rowNum + "\" value=\"\" maxlength=\"50\" /></td>";
        Row += "<td><input type=\"text\" name=\"MenuUrl" + rowNum + "\" id=\"MenuUrl" + rowNum + "\" value=\"\" readonly /></td>";
        Row += "<td><select name=\"MenuTarget" + rowNum + "\" id=\"MenuTarget" + rowNum + "\" disabled><option value=\"_blank\" >新窗口打开</option><option  value=\"_self\" >当前窗口打开</option></select></td>";
        Row += "<td><span ><a href=\"javascript:void(0);\" onclick=\"javascript:delRow(this);\">删除</a></span></td></tr>";
        $("#MainMenuTable").append(Row);
    }
    else if (menuType == "adminSub") {
        var mainMenuID = $("#MainMenuSel option:selected").val();
        var rowNum = $("#SubMenuTable tr:visible").length - 3;
        var Row = "<tr id=\""+mainMenuID+"\"><td align=\"center\">" + (rowNum + 1) + "<input type=\"hidden\" name=\"MenuID" + rowNum + "\" id=\"MenuID" + rowNum + "\" value=\"\" /></td><td><input type=\"text\" name=\"NavItem" + rowNum + "\" id=\"NavItem" + rowNum + "\" value=\"\" maxlength=\"50\" /></td>";
        Row += "<td><input type=\"text\" name=\"MenuStyle" + rowNum + "\" id=\"MenuStyle" + rowNum + "\" value=\"\" maxlength=\"50\" /></td>";
        Row += "<td><input type=\"text\" name=\"MenuUrl" + rowNum + "\" id=\"MenuUrl" + rowNum + "\" value=\"http://\" /></td>";
        Row += "<td><select name=\"MenuTarget" + rowNum + "\" id=\"MenuTarget" + rowNum + "\" ><option value=\"_blank\" >新窗口打开</option><option  value=\"_self\" >当前窗口打开</option></select></td>";
        Row += "<td><span ><a href=\"javascript:void(0);\" onclick=\"javascript:delRow(this);\">删除</a></span></td></tr>";
        $("#SubMenuTable").append(Row);
    }
    else {
        var rowNum = $("#MenuTable tr").length - 2;
        if (rowNum >= 10) {
            alert("最多只能10个菜单.");
            return;
        }
        var Row = "<tr><td align=\"center\">" + (rowNum + 1) + "<input type=\"hidden\" name=\"MenuID" + rowNum + "\" id=\"MenuID" + rowNum + "\" value=\"\" /></td><td><input type=\"text\" name=\"NavItem" + rowNum + "\" id=\"NavItem" + rowNum + "\" value=\"\" maxlength=\"50\" /></td>";
        Row += "<td><input type=\"text\" name=\"MenuStyle" + rowNum + "\" id=\"MenuStyle" + rowNum + "\" value=\"NavItem\" maxlength=\"50\" /></td>";
        Row += "<td><input type=\"text\" name=\"MenuUrl" + rowNum + "\" id=\"MenuUrl" + rowNum + "\" value=\"http://\" /></td>";
        Row += "<td><select name=\"MenuTarget" + rowNum + "\" id=\"MenuTarget" + rowNum + "\"><option value=\"_blank\" >新窗口打开</option><option  value=\"_self\" >当前窗口打开</option></select></td>";
        Row += "<td><span ><a href=\"javascript:void(0);\" onclick=\"javascript:delRow(this);\">删除</a></span></td></tr>";
        $("#MenuTable").append(Row);
    }
    
}
function changeTable(from) {
    if (from == 'sub') {
        $("#SubMenuTable").show();
        $("#MainMenuTable").hide();
    }
    else {
        $("#MainMenuTable").show();
        $("#SubMenuTable").hide();
    }
}
function delRow(obj) {
    $(obj).parent().parent().parent().remove();
}
function saveMenu(table) {
    var param = {};
    var ListMenu = new Array();
    var saveType = "SaveMainMenu";
    if (table == null) {
        table = "MainMenuTable";
        if ($("#MainMenuTable").is(":hidden")) {
            table = "SubMenuTable";
            saveType = "SaveSubMenu";
        }
    }
    if (table == "SubMenuTable") {
        if ($("#MainMenuSel option:selected").val() == "") {
            alert("请选择要保存的主菜单。");
            $("#MainMenuSel").focus();
            return;
        }
    }
    var selector = "#" + table + " tr:visible ";
    for (var i = 0; i < $(selector).length; i++) {
        var Menu = {};
        if ($(selector+"input[id='NavItem" + i+"']").val() == null) {
            break;
        }

        Menu.MenuDisq = i;

        if ($(selector + "input[id='MenuID" + i + "']").val() != "")
            Menu.MenuID = $(selector + "input[id='MenuID" + i + "']").val();
        else {
            Menu.MenuID = "";
        }

//        if ($(selector + "input[id='SubMenuMappingID" + i + "']").val() != "")
//            Menu.SubMenuMappingID = $(selector + "input[id='SubMenuMappingID" + i + "']").val();
//        else {
//            Menu.SubMenuMappingID = "";
//        }

        if ($(selector + "input[id='NavItem" + i + "']").val() != "")
            Menu.MenuName = $(selector + "input[id='NavItem" + i + "']").val();
        else {
            alert("菜单名称不能为空.");
            $(selector + "input[id='NavItem" + i + "']").focus();
            return;
        }

        if ($(selector + "input[id='MenuStyle" + i + "']").val() != "") {
            Menu.MenuClassName = $(selector + "input[id='MenuStyle" + i + "']").val();
        }
        else {
            alert("菜单默认样式为\"NavItem\"");
            $(selector + "input[id='MenuStyle" + i + "']").focus();
            return;
        }

        if (table != "MainMenuTable") {
            if ($(selector + "input[id='MenuUrl" + i + "']").val()) {
                Menu.MenuUrl = $(selector + "input[id='MenuUrl" + i + "']").val();
            }
            else {
                alert("请输入要导航的连接.");
                $(selector + "input[id='MenuUrl" + i + "']").focus();
                return;
            }
        }
        else {
            Menu.MenuUrl = "";
        }
        if (table == "MenuTable") {
            Menu.Target = $(selector + "select[id='MenuTarget" + i + "'] option:selected").val();
            Menu.IsAdminMenu = 0;
            Menu.MenuType = "MainMenu";
        }
        else {
            if (table == "MainMenuTable") {
                Menu.MenuType = "MainSubCategory";
                Menu.Target = "";
            }
            else {
                Menu.MenuType = "SubCategory";
                Menu.Target = $(selector + "select[id='MenuTarget" + i + "'] option:selected").val();
            }
            Menu.IsAdminMenu = 1;
        }
        
        ListMenu.push(Menu);
    }
    if (ListMenu.length == 0)
        return;
    var mainMenuID = "";
    if (table == "SubMenuTable") {
        mainMenuID = $("#MainMenuSel option:selected").val();
    }
    param["listMenu"] = ListMenu;
    param["mainMenuID"] = mainMenuID;
    param["saveType"] = saveType;
    $.ajax({
        url: "/Admin/Home/AddMenu",
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
function GetSubCategory(obj) {
    for (var i = 0; i < obj.options.length; i++) {
        if (obj.options[i].value != "" && obj.options[i].selected) {
            $("#SubMenuTable tr[id='" + obj.options[i].value + "']").each(function (i, opt) {
                $(opt).show();
            });
        } 
        else {
            $("#SubMenuTable tr[id=='" + obj.options[i].value + "']").each(function (i, opt) {
                $(opt).hide();
            });
        }
    }
}