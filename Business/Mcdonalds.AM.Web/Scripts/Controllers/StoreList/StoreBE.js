
//  加载详情页-StoreBEInfo
function loadStoreBEInfo(USCode) {

    $.ajax({
        type: "GET",
        url: "/StoreList/StoreBEInfoQuery",
        cache: false,
        dataType: "json",
        data: {
            _USCode: USCode
        },
        success: function (data) {

            var result = eval(data);

            var html = [];
            html.push("<tr>");
            html.push("<th style='width:100px'>BE Code</th>");
            html.push("<th style='width:100px'>BE类型</th>");
            html.push("<th style='width:80px'>开业日期</th>");
            html.push("<th style='width:80px'>关店日期</th>");
            html.push("<th style='width:120px'>平均月销售额（RMB）</th>");
            html.push("<th style='width:100px'>平均月GC（RMB）</th>");
            html.push("<th style='width:100px'>是否独立合同</th>");
            html.push("</tr>");

            for (var i = 0; i < result.length   ; i++) {
                if (i < result.length) {
                    var plan = result[i];

                    html.push("<tr>");
                    html.push("<td>" + stringNullConvert(plan.BECode) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.BETypeName) + "</td>");
                    html.push("<td>" + datetimeConvertForOld(plan.LaunchDate) + "</td>");
                    html.push("<td>" + datetimeConvertForOld(plan.CloseDate) + "</td>");
                    html.push("<td>" + stringNullConvertCurrency(plan.MonthlyNetTotalSales) + "</td>");
                    html.push("<td>" + stringNullConvertCurrency(plan.MonthlyTotalGC) + "</td>");
                    html.push("<td>" + stringNullConvertYesNoENUS(plan.IsSingleContract)+ "</td>");
                    html.push("</tr>");
                }
            }
            $("#tabBEInfo").html("");
            $("#tabBEInfo").append(html.join(""));
        }
    });

}

//  加载详情页-StoreBEContractInfo
function loadStoreBEContractInfo(USCode) {

    $.ajax({
        type: "GET",
        url: "/StoreList/StoreBEContractInfoQuery",
        cache: false,
        dataType: "json",
        data: {
            _USCode: USCode
        },
        success: function (data) {

            var result = eval(data);

            var html = [];
            html.push("<tr>");
            html.push("<th style='width:80px'>BE名称</th>");
            html.push("<th style='width:80px'>业主名称</th>");
            html.push("<th style='width:80px'>Rental</th>");
            html.push("<th>Md Legal Entity</th>");
            html.push("<th style='width:120px'>Contract person</th>");
            html.push("<th style='width:80px'>Size</th>");
            html.push("<th style='width:100px'>Lease Term</th>");
            html.push("<th style='width:120px'>Lease Start Date</th>");
            html.push("<th style='width:120px'>Lease End Date</th>");
            html.push("<th style='width:100px'>Contact Mode</th>");
            html.push("</tr>");

            for (var i = 0; i < result.length   ; i++) {
                if (i < result.length) {
                    var plan = result[i];

                    html.push("<tr>");
                    html.push("<td>" + stringNullConvert(plan.BeName) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.LandlordName) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.Rental) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.McDLegalEntity) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.ContactPerson) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.Size) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.LeaseTerm) + "</td>");
                    html.push("<td>" + datetimeConvertForOld(plan.LeaseStartDate) + "</td>");
                    html.push("<td>" + datetimeConvertForOld(plan.LeaseEndDate) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.ContactMode) + "</td>");
                    html.push("</tr>");
                }
            }
            $("#tabBEContractInfo").html("");
            $("#tabBEContractInfo").append(html.join(""));
        }
    });

}

