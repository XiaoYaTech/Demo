function LoadStoreSummaryList(USCode) {
    LoadSummaryPage(USCode, 1, 5, false);
}

function SelectSummaryPage(index) {
    var USCode = $("#spStoreListUSCode")[0].innerText;
    LoadSummaryPage(USCode, index, 5, true);
}

function LoadSummaryPage(USCode, pageIndex, pageSize, hasCover) {
    if (hasCover) {
        //$("#imgStoreListDetial")[0].style.display = 'block';
    }
    $.ajax({
        type: "GET",
        url: "/StoreList/StoreSummaryListQuery",
        cache: false,
        dataType: "json",
        data: {
            _USCode: USCode,
            index: pageIndex,
            size: pageSize
        },
        success: function (data) {
            var objdata = eval(data);
            CreateSummaryList(objdata.dataPage);
            $("#SummaryPagination")[0].innerHTML = objdata.paginationInnerHtml;
            //$("#imgStoreListDetial")[0].style.display = 'none';


        }
    });
}

function CreateSummaryList(result) {

    var html = [];
    html.push("<tr>");
    html.push("<th style='width:100px'>Push or Not</th>");
    html.push("<th>Title</th>");
    html.push("<th style='width:240px'>Submitter</th>");
    html.push("<th style='width:240px'>Submit Date</th>");
    html.push("</tr>");

    if (result && result.length > 0) {
        for (var i = 0; i < result.length; i++) {
            html.push("<tr>");
            if (result[i].LastUpdateTime) {
                html.push("<td><a class=\"btn-sm btn-default\" href=\"#\"><span class=\"fa fa-check-circle-o\"></span></a>");
            }
            else {
                html.push("<td><a class=\"btn-sm btn-default\" href=\"#\"><span class=\"fa fa-upload\"></span></a>");
            }
            html.push("<td>" + stringNullConvert(result[i].FlowCode + " " + result[i].CreateTime) + "</td>");
            html.push("<td>" + stringNullConvert(result[i].CreateUserAccount) + "</td>");
            html.push("<td>" + stringNullConvertCurrency(result[i].LastUpdateTime) + "</td>");
            html.push("</tr>");
        }



    }

    $("#tabSummaryList").html("");
    $("#tabSummaryList").append(html.join(""));
}

function CreateSumarryDetail() {
    var flow = document.getElementById("flowCode").value;
    $.ajax({
        type: "GET",
        url: "/StoreList/StoreSummaryDetail",
        cache: false,
        dataType: "text",
        data: {
            flowCode: flow
        },
        success: function (data) {
            $("#divSummaryDetails").html("");
            $("#divSummaryDetails").append(data);
            $('#divSummaryList')[0].style.display = 'none';
            $('#divSummaryDetails')[0].style.display = 'block';

        }
    });
}