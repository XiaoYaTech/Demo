

var __uscode = "";
var __backurl = "";

(function ($) {
    $.getUrlParam = function (name) {
        var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
        //var r = window.location.search.substr(1).match(reg);
        //var s = window.location.href.substr(window.location.href.indexOf('?', 0));
        var r = window.location.href.substr(window.location.href.indexOf('?', 0)).substr(1).match(reg);
        if (r != null) return unescape(r[2]); return '';
    }
})(jQuery);

//
window.onload = function () {

    if (window.currentUser != null) {

        //var _url = Utils.ServiceURI.Address() + 'api/taskwork/reminders/' + window.currentUser.Code;

        $.support.cors = true;
        //$.ajax({
        //    url: _url,
        //    cache: false,
        //    dataType: "json",
        //    success: function (data) {
        //        var objdata = eval(data);

        //        $("#taskBarTasks")[0].innerHTML = objdata[0];
        //        $("#taskBarReminding")[0].innerHTML = objdata[1];
        //        $("#taskBarNotices")[0].innerHTML = objdata[2];
        //    }
        //});

        if ($.inArray("am_sm_asset_sb_view", window.currentUser.RightCodes || []) >= 0) {
            $("#divStoreManagement").show();
        } else {
            $("#divStoreManagement").hide();
        }


        if ($.inArray("am_sm_view", window.currentUser.RightCodes || []) >= 0) {
            $("#liStoreManagement").show();
        } else {
            $("#liStoreManagement").hide();
        }

        if ($.inArray("am_sm_re_view", window.currentUser.RightCodes || []) >= 0) {
            $("#liRealEstate").show();
        } else {
            $("#liRealEstate").hide();
        }

        if ($.inArray("am_sm_ta_view", window.currentUser.RightCodes || []) >= 0) {
            $("#liTA").show();
        } else {
            $("#liTA").hide();
        }

        if ($.inArray("am_sm_asset_cons_view", window.currentUser.RightCodes || []) >= 0) {
            $("#liConstruction").show();
        } else {
            $("#liConstruction").hide();
        }

        if ($.inArray("am_sm_asset_view", window.currentUser.RightCodes || []) >= 0) {
            $("#liCOBA").show();
        } else {
            $("#liCOBA").hide();
        }

        if ($.inArray("am_sm_asset_fi_view", window.currentUser.RightCodes || []) >= 0) {
            $("#liFinanceInfo").show();
        } else {
            $("#liFinanceInfo").hide();
        }

        if ($.inArray("am_sm_asset_ct_view", window.currentUser.RightCodes || []) >= 0) {
            $("#liContractInfo").show();
        } else {
            $("#liContractInfo").hide();
        }

        if ($.inArray("am_sm_asset_ll_view", window.currentUser.RightCodes || []) >= 0) {
            $("#liOwnerContactInfo").show();
        } else {
            $("#liOwnerContactInfo").hide();
        }

        if ($.inArray("am_sm_asset_be_view", window.currentUser.RightCodes || []) >= 0) {
            $("#liBE").show();
        } else {
            $("#liBE").hide();
        }

        if ($.inArray("am_sm_asset_lic_view", window.currentUser.RightCodes || []) >= 0) {
            $("#liAssetManagement").show();
        } else {
            $("#liAssetManagement").hide();
        }

        $("#liSummary").hide();
        $.ajax({
            type: "GET",
            url: Utils.ServiceURI.Address() + "api/DL/Authority",
            cache: false,
            dataType: "json",
            data: {
                usCode: $.getUrlParam('uscode')
            },
            success: function (data) {
                if (data.EnableView) {
                    $("#liSummary").show();
                }
                else {
                    $("#liSummary").hide();
                }
            }
        });
    }

    __uscode = unescape($.getUrlParam('uscode'));
    __backurl = unescape($.getUrlParam('backurl'));

    if (__uscode == null) __uscode = "";
    if (__backurl == null) __backurl = "";

    if (__uscode.length != 0) {
        viewDetial(__uscode);
    }
    else {
        selectPage(1);
    }
}

//  
function selectPage(pageIndex) {

    var pageSize = 9;

    $("#imgStoreList")[0].style.display = '';
    $("#tabStoreList").hide();
    $.ajax({
        type: "GET",
        url: "/StoreList/StoreListQuery",
        cache: false,
        dataType: "json",
        data: {
            _CityName: $("#txtCityName")[0].value,
            _NameZHCN: $("#txtNameZHCN")[0].value,
            _Province: $("#txtProvince")[0].value,
            _USCode: $("#txtUSCode")[0].value,
            _PageIndex: pageIndex,
            _PageSize: pageSize
        },
        success: function (data) {

            var objdata = eval(data);
            $("#paginationDesc")[0].innerHTML = objdata.paginationDesc;
            $("#paginationInnerHtml")[0].innerHTML = objdata.paginationInnerHtml;
            createTabStoreList(objdata);
            $("#imgStoreList")[0].style.display = 'none';
            $("#tabStoreList").show();
        }
    });
}

//  
function viewDetial(USCode) {

    $('#divStore')[0].style.display = 'none';
    $('#divStoreDetail')[0].style.display = '';

    $("#spStoreListUSCode")[0].innerText = USCode;

    loadDataDetialHead(USCode);

    //////////////////////////////////////////////////
    loadStoreManagement(USCode);

    loadRealEstate(USCode);
    loadRealEstateAttachmentQuery(USCode);

    loadTA(USCode);

    //loadConstruction(USCode);

    //loadStoreContractInfo(USCode);

    loadStoreOwnerContactInfo(USCode);

    loadStoreBEInfo(USCode);
    loadStoreBEContractInfo(USCode);

    loadStoreLicenses(USCode);

    loadStoreFinanceInfo(USCode);

    //LoadStoreSummaryList(USCode);
}

//  
function switchMain() {
    if (__backurl.length != 0) {
        //history.go(-1);
        location.href = __backurl;
    }
    else {
        $('#divStoreDetail')[0].style.display = 'none';
        $('#divStore')[0].style.display = '';
        selectPage(1);
    }
}

//  
$(document).ready(function ($) {

    // Workaround for bug in mouse item selection
    $.fn.typeahead.Constructor.prototype.blur = function () {
        var that = this;
        setTimeout(function () { that.hide() }, 250);
    };

    $('#txtUSCode').typeahead({
        source: function (query, process) {
            $.ajax({
                type: "GET",
                url: "/StoreList/StoreSearchListQuery",
                cache: false,
                dataType: "json",
                data: {
                    _SearchType: 'USCode',
                    _SearchValue: query
                },
                success: function (data) {
                    process(data);
                }
            });
        }
    });

    $('#txtNameZHCN').typeahead({
        source: function (query, process) {
            $.ajax({
                type: "GET",
                url: "/StoreList/StoreSearchListQuery",
                cache: false,
                dataType: "json",
                data: {
                    _SearchType: 'NameZHCN',
                    _SearchValue: query
                },
                success: function (data) {
                    process(data);
                }
            });
        }
    });

    $('#txtProvince').typeahead({
        source: function (query, process) {
            $.ajax({
                type: "GET",
                url: "/StoreList/StoreSearchListQuery",
                cache: false,
                dataType: "json",
                data: {
                    _SearchType: 'Province',
                    _SearchValue: query
                },
                success: function (data) {
                    process(data);
                }
            });
        }
    });

    $('#txtCityName').typeahead({
        source: function (query, process) {
            $.ajax({
                type: "GET",
                url: "/StoreList/StoreSearchListQuery",
                cache: false,
                dataType: "json",
                data: {
                    _SearchType: 'CityName',
                    _SearchValue: query
                },
                success: function (data) {
                    process(data);
                }
            });
        }
    });

});

//  
function switchStoreCategory(categoryName) {
    //  head1
    $("#aStoreManagement").removeClass("active");
    $("#aRealEstate").removeClass("active");
    $("#aTA").removeClass("active");
    $("#aConstruction").removeClass("active");
    $("#aAssetManagement").removeClass("active");
    $("#aContractInfo").removeClass("active");
    $("#aOwnerContactInfo").removeClass("active");
    $("#aBE").removeClass("active");
    $("#aFinanceInfo").removeClass("active");

    $("#a" + categoryName).addClass("active");

    //  head2
    $("#liStoreManagement").removeClass("active");
    $("#liRealEstate").removeClass("active");
    $("#liTA").removeClass("active");
    $("#liConstruction").removeClass("active");
    $("#liAssetManagement").removeClass("active");
    $("#liContractInfo").removeClass("active");
    $("#liOwnerContactInfo").removeClass("active");
    $("#liBE").removeClass("active");
    $("#liFinanceInfo").removeClass("active");
    $("#liSummaryList").removeClass("active");

    $("#li" + categoryName).addClass("active");
    if (categoryName == 'ContractInfo' ||
        categoryName == 'OwnerContactInfo' ||
        categoryName == 'BE' ||
        categoryName == 'AssetManagement') {
        $("#liCOBA").addClass("active");
    }
    else {
        $("#liCOBA").removeClass("active");
    }

    //  content
    $('#divStoreManagement')[0].style.display = 'none';
    $('#divRealEstate')[0].style.display = 'none';
    $('#divTA')[0].style.display = 'none';
    $('#divConstruction')[0].style.display = 'none';
    $('#divAssetManagement')[0].style.display = 'none';
    $('#divContractInfo')[0].style.display = 'none';
    $('#divOwnerContactInfo')[0].style.display = 'none';
    $('#divBE')[0].style.display = 'none';
    $('#divFinanceInfo')[0].style.display = 'none';
    $('#divSummaryList')[0].style.display = 'none';
    $('#divSummaryDetails')[0].style.display = 'none';
    $('#div' + categoryName)[0].style.display = '';
}

//  
function switchStoreFlowType(flowTypeName) {
    var vUSCode = $("#spStoreListUSCode")[0].innerText;
    var vTopRowCount = -1;
    var closeDiv = $("#a" + flowTypeName).hasClass("active");
    var selectDiv = !$("#a" + flowTypeName).hasClass("active");

    //  head1
    $("#aClosure").removeClass("active");
    $("#aRenewal").removeClass("active");
    $("#aRebuild").removeClass("active");
    $("#aMajorLease").removeClass("active");
    $("#aReimage").removeClass("active");
    $("#aTempClosure").removeClass("active");
    if (selectDiv) $("#a" + flowTypeName).addClass("active");

    //  head2
    $("#liClosure").removeClass("active");
    $("#liRenewal").removeClass("active");
    $("#liRebuild").removeClass("active");
    $("#liMajorLease").removeClass("active");
    $("#liReimage").removeClass("active");
    $("#liTempClosure").removeClass("active");
    if (selectDiv) $("#li" + flowTypeName).addClass("active");

    //
    if (closeDiv) {
        $('#divStoreFlowType')[0].style.display = 'none';
        $('#divStoreFlowTypeMarginBottom')[0].style.display = 'none';
        $('#divStoreFlowTypeMore')[0].style.display = 'none';
        return;
    }

    $("#tabStoreListProjectInfo").html("");
    $('#divStoreFlowType')[0].style.display = '';
    $('#divStoreFlowTypeMarginBottom')[0].style.display = '';
    $('#divStoreFlowTypeMore')[0].style.display = '';

    $("#inputFlowTypeName")[0].value = flowTypeName;

    $.ajax({
        type: "GET",
        url: "/StoreList/FlowInfoQuery",
        cache: false,
        dataType: "json",
        data: {
            _USCode: vUSCode,
            _FlowCode: flowTypeName,
            _TopRowCount: vTopRowCount
        },
        success: function (data) {

            var objdata = eval(data);
            createTabStoreListProjectInfo(objdata, flowTypeName);
        }
    });
}
function divStoreFlowTypeMore_onclick() {

    var vUSCode = $("#spStoreListUSCode")[0].innerText;
    var vTopRowCount = -1;
    var flowTypeName = $("#inputFlowTypeName")[0].value;

    if (flowTypeName == 'Closure') {

        $.ajax({
            type: "GET",
            url: "/StoreList/ClosureInfoQuery",
            cache: false,
            dataType: "json",
            data: {
                _USCode: vUSCode,
                _TopRowCount: vTopRowCount
            },
            success: function (data) {

                var objdata = eval(data);
                //$("#spanClosureRowsCount")[0].innerText = objdata.length;
                //$("#lispanClosureRowsCount")[0].innerText = objdata.length;
                createTabStoreListProjectInfo(objdata);
            }
        });
    }





}

function datetimeConvert(dtValue) {

    var result = '';

    if (dtValue == null || dtValue.length == 0)
        result = '';
    else
        result = moment(dtValue).format("YYYY-MM-DD");
        //result = moment(dtValue.toString().replace('T', ' '), "YYYYMMDD").format("YYYY-MM-DD");
    

    result = result.replace('1900-01-01', '');

    return result;
}

function datetimeConvertForOld(dtValue) {

    var result = '';

    if (dtValue == null || dtValue.length == 0)
        result = '';
    else
      result = moment(dtValue.toString().replace('T', ' '), "YYYYMMDD").format("YYYY-MM-DD");


    result = result.replace('1900-01-01', '');

    return result;
}

function stringNullConvert(dtValue) {

    var result = '';

    if (dtValue == null)
        result = '';
    else
        result = dtValue;
    //result = window.encodeURIComponent(result);
    return result;
}

function stringNullConvertWithoutEncode(dtValue) {

    var result = '';

    if (dtValue == null)
        result = '';
    else
        result = dtValue;
    
    return result;
}

function stringNullConvertYesNoZHCN(dtValue) {

    var result = '';

    if (dtValue == null || dtValue == "")
        return '';
    else
        result = dtValue;

    return (result == "0" ? "否" : "是");
}

function stringNullConvertYesNoENUS(dtValue) {

    var result = '';

    if (dtValue == null || dtValue == "")
        return '';
    else
        result = dtValue;

    return (result == "0" ? "No" : "Yes");
}

//保留2位小数
function stringNullConvertNumber(dtValue) {
    var result = '';
    if (dtValue == null || dtValue == "")
        return '';
    else {
        var num = new Number(dtValue.replace(/,/g, ""));
        if (num.toString().indexOf('.') < 0)
            return num;
        else {
            var cent = Math.round(((num) - Math.floor(num)) * 100);
            if (cent == 0)
                cent = "";
            else if (cent.toString().indexOf('0') == 1)
                cent = "." + cent.toString()[0];
            else
                cent = (cent < 10 ? '.0' + cent : '.' + cent);
            return parseInt(num).toString() + cent;
        }
    }
}

function stringNullConvertPercentage(dtValue) {
    var result = '';
    if (dtValue == null || dtValue == "")
        return '';
    else {
        var num = new Number(dtValue.replace(/,/g, ""));
        num = num * 100;
        if (num.toString().indexOf('.') < 0)
            return num + "%";
        else {
            var cent = Math.round(((num) - Math.floor(num)) * 10);
            if (cent == 0)
                cent = "";
            else
                cent = '.' + cent;
            return parseInt(num).toString() + cent + "%";
        }
    }
}

function stringNullConvertCurrency(dtValue) {
    var result = '';
    if (dtValue == null || dtValue == "")
        return '';
    else {
        var num = new Number(dtValue.replace(/,/g, ""));
        //if (num < 0) {
        //    result = '-' + outputDollars(Math.floor(Math.abs(num) - 0) + '') + outputCents(Math.abs(num) - 0);
        //}
        //else {
        //    result = outputDollars(Math.floor(num - 0) + '') + outputCents(num - 0);
        //}
        if (num < 0) {
            result = '-' + outputDollars(Math.floor(Math.abs(num) - 0) + '');
        }
        else {
            result = outputDollars(Math.floor(num - 0) + '');
        }
    }
    return result;
}

function outputDollars(number) {
    if (number.length <= 3)
        return (number == '' ? '0' : number);
    else {
        var mod = number.length % 3;
        var output = (mod == 0 ? '' : (number.substring(0, mod)));
        for (i = 0 ; i < Math.floor(number.length / 3) ; i++) {
            if ((mod == 0) && (i == 0))
                output += number.substring(mod + 3 * i, mod + 3 * i + 3);
            else
                output += ',' + number.substring(mod + 3 * i, mod + 3 * i + 3);
        }
        return (output);
    }
}

function outputCents(amount) {
    amount = Math.round(((amount) - Math.floor(amount)) * 100);
    if (amount == 0)
        return "";
    //if (amount.toString().indexOf('0') == 1)
    //    return "." + amount.toString()[0];
    return (amount < 10 ? '.0' + amount : '.' + amount);
}

//  创建HTML表格到StoreList
function createTabStoreList(objdata) {

    var html = [];

    html.push("<tr>");
    html.push("<th style='width:80px'>[[[餐厅编号]]]</th>");
    html.push("<th style='width:135px'>[[[中文名称]]]</th>");
    html.push("<th style='width:145px'>[[[英文名称]]]</th>");
    html.push("<th style='width:60px'>[[[省份]]]</th>");
    html.push("<th style='width:45px'>[[[城市]]]</th>");
    html.push("<th style='width:90px'>[[[餐厅类别]]]</th>");
    html.push("<th style='width:125px'>[[[投资组合]]]</th>");
    html.push("<th style='width:60px'>[[[Status]]]</th>");
    html.push("<th style='width:90px'>[[[开业日期]]]</th>");
    html.push("<th style='width:90px'>[[[Close Date]]]</th>");
    html.push("<th style='width:110px'>[[[Reimage Date]]]</th>");
    html.push("<th style='width:90px'>[[[内部设计]]]</th>");
    html.push("<th style='width:90px'>[[[资产专员]]]</th>");
    html.push("<th style='width:90px'>[[[资产经理]]]</th>");
    html.push("</tr>");

    for (var i = 0; i < objdata.dataPage.length || i < 9; i++) {
        if (i < objdata.dataPage.length) {
            var plan = objdata.dataPage[i];
            html.push("<tr>");
            html.push("<td style='white-space:nowrap;overflow:hidden;word-break:break-all;'><a href='javascript:RedirectDetail(" + plan.StoreCode + ")'>" + plan.StoreCode + "</a></td>");
            html.push("<td style='white-space:nowrap;overflow:hidden;word-break:break-all;'>" + plan.NameENUS + "</td>");
            html.push("<td style='white-space:nowrap;overflow:hidden;word-break:break-all;'>" + plan.NameZHCN + "</td>");
            html.push("<td style='white-space:nowrap;overflow:hidden;word-break:break-all;'>" + stringNullConvertWithoutEncode(plan.ProvinceZHCN) + "</td>");
            html.push("<td style='white-space:nowrap;overflow:hidden;word-break:break-all;'>" + stringNullConvertWithoutEncode(plan.CityZHCN) + "</td>");
            html.push("<td style='white-space:nowrap;overflow:hidden;word-break:break-all;'>" + stringNullConvertWithoutEncode(plan.StoreTypeName) + "</td>");
            html.push("<td style='white-space:nowrap;overflow:hidden;word-break:break-all;'>" + stringNullConvertWithoutEncode(plan.PortfolioTypeName) + "</td>");
            html.push("<td style='white-space:nowrap;overflow:hidden;word-break:break-all;'>" + stringNullConvertWithoutEncode(plan.statusName) + "</td>");
            html.push("<td style='white-space:nowrap;overflow:hidden;word-break:break-all;'>" + datetimeConvert(plan.OpenDate) + "</td>");
            html.push("<td style='white-space:nowrap;overflow:hidden;word-break:break-all;'>" + datetimeConvert(plan.CloseDate) + "</td>");
            html.push("<td style='white-space:nowrap;overflow:hidden;word-break:break-all;'>" + datetimeConvert(plan.ReImageDate) + "</td>");

            html.push("<td style='white-space:nowrap;overflow:hidden;word-break:break-all;'>" + stringNullConvertWithoutEncode(plan.DesignType) + "</td>");
            html.push("<td style='white-space:nowrap;overflow:hidden;word-break:break-all;'>" + stringNullConvertWithoutEncode(plan.AssetRepName) + "</td>");
            html.push("<td style='white-space:nowrap;overflow:hidden;word-break:break-all;'>" + stringNullConvertWithoutEncode(plan.AssetMgrName) + "</td>");
            html.push("</tr>");
        }
        else {
            html.push("<tr>");
            html.push("<td>&nbsp;</td>");
            html.push("<td></td>");
            html.push("<td></td>");
            html.push("<td></td>");
            html.push("<td></td>");
            html.push("<td></td>");
            html.push("<td></td>");
            html.push("<td></td>");
            html.push("<td></td>");
            html.push("<td></td>");
            html.push("<td></td>");
            html.push("<td></td>");
            html.push("<td></td>");
            html.push("<td></td>");
            html.push("</tr>");
        }
    }
    $("#tabStoreList").html("");
    $("#tabStoreList").append(html.join(""));

}

function RedirectDetail(uscode) {
    var url = window.location.href.substr(0, window.location.href.indexOf('?', 0));
    window.location.href = url + "?uscode=" + uscode;
}

//  创建HTML表格到StoreListProjectInfo
function createTabStoreListProjectInfo(objdata, flowTypeName) {

    var html = [];

    if (objdata.length > 0) {
        html.push("<tr>");
        html.push("<th>[[[编号]]]</th>");
        html.push("<th>[[[项目类型]]]</th>");
        html.push("<th>[[[餐厅编号]]]</th>");
        html.push("<th>[[[英文名称]]]</th>");
        html.push("<th>[[[中文名称]]]</th>");
        html.push("<th>[[[资产发起人]]]</th>");
        html.push("<th>[[[Status]]]</th>");
        html.push("</tr>");
        for (var i = 0; i < objdata.length; i++) {
            var plan = objdata[i];
            html.push("<tr>");
            if (!!flowTypeName) {
                var url = "/Home/Main#/project/detail/" + plan.ProjectId + "?flowCode=" + flowTypeName;
                html.push("<td><a href='" + url + "' target='_self'>" + plan.ProjectId + "</a></td>");
            }
            else
                html.push("<td>" + plan.ProjectId + "</td>");
            html.push("<td>" + stringNullConvert(plan.FlowCode) + "</td>"); //  ClosureTypeCode
            html.push("<td>" + plan.USCode + "</td>");
            html.push("<td>" + stringNullConvert(plan.StoreNameENUS) + "</td>");
            html.push("<td>" + stringNullConvert(plan.StoreNameZHCN) + "</td>");
            html.push("<td>" + stringNullConvert(plan.AssetActorNameENUS) + "</td>");  //  AssetActorAccount
            switch (plan.Status) {
                case 3:
                    plan.Status = "Pending";
                    break;
                case 4:
                    plan.Status = "Rejected";
                    break;
                case 5:
                    plan.Status = "Completed";
                    break;
                case 7:
                    plan.Status = "Killed";
                    break;
                default:
                    plan.Status = "Active";
                    break;
            }
            html.push("<td>" + stringNullConvert(plan.Status) + "</td>");  //  RiskStatusCode
            html.push("</tr>");
        }
    }
    $("#tabStoreListProjectInfo").html("");
    $("#tabStoreListProjectInfo").append(html.join(""));

}

//  加载详情页-关联项目总数
function loadDataDetialHead(USCode) {

    $.ajax({
        type: "GET",
        url: "/StoreList/StoreProjectRowsCount",
        cache: false,
        dataType: "json",
        data: {
            _USCode: USCode
        },
        success: function (data) {
            var objdata = eval(data);
            $("#spanClosureRowsCount")[0].innerText = objdata.ClosureRowsCount;
            $("#spanRenewalRowsCount")[0].innerText = objdata.RenewalRowsCount;
            $("#spanRebuildRowsCount")[0].innerText = objdata.RebuildRowsCount;
            $("#spanMajorLeaseRowsCount")[0].innerText = objdata.MajorLeaseRowsCount;
            $("#spanReimageRowsCount")[0].innerText = objdata.ReimageRowsCount;
            $("#spanTempClosureRowsCount")[0].innerText = objdata.TempClosureRowsCount;

            $("#lispanClosureRowsCount")[0].innerText = objdata.ClosureRowsCount;
            $("#lispanRenewalRowsCount")[0].innerText = objdata.RenewalRowsCount;
            $("#lispanRebuildRowsCount")[0].innerText = objdata.RebuildRowsCount;
            $("#lispanMajorLeaseRowsCount")[0].innerText = objdata.MajorLeaseRowsCount;
            $("#lispanReimageRowsCount")[0].innerText = objdata.ReimageRowsCount;
            $("#lispanTempClosureRowsCount")[0].innerText = objdata.TempClosureRowsCount;
        }
    });

}


