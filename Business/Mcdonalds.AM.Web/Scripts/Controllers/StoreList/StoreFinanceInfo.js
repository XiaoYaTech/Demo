
//  加载详情页-FinanceInfo
function loadStoreFinanceInfo(USCode) {

    $.ajax({
        type: "GET",
        url: "/StoreList/StoreSTMonthlyFinaceInfoQuery",
        cache: false,
        dataType: "json",
        data: {
            _USCode: USCode
        },
        success: function (data) {

            var result = eval(data);
            $("#divNetProductSales")[0].innerText = stringNullConvertCurrency(result.NetProductSales);
            $("#divCashFlow")[0].innerText = stringNullConvertCurrency(result.CashFlow);
            $("#divSOI")[0].innerText = stringNullConvertCurrency(result.SOI);

            $("#divRent")[0].innerText = stringNullConvertCurrency(result.Rent);
            $("#divMcOpCoMargin")[0].innerText = stringNullConvertCurrency(result.McOpCoMargin);
            $("#divSOIPct")[0].innerText = stringNullConvert(result.SOIPct);

            $("#divRent_inclAdjustment")[0].innerText = stringNullConvertCurrency(result.Rent_inclAdjustment);
            $("#divRent_inclAdjustmentPct")[0].innerText = stringNullConvert(result.Rent_inclAdjustmentPct);
            $("#divCompsSales")[0].innerText = stringNullConvert(result.CompsSales);

            $("#divLHINBV")[0].innerText = stringNullConvertCurrency(result.LHINBV);
            $("#divESSDNBV")[0].innerText = stringNullConvertCurrency(result.ESSDNBV);
            $("#divTotalNBV")[0].innerText = stringNullConvertCurrency(result.TotalNBV);

            var html = [];
            html.push("<tr>");
            html.push("<th style='width:100px'>Year</th>");
            html.push("<th style='width:240px'>Comps Sales%</th>");
            html.push("<th style='width:240px'>SOI(RMB)</th>");
            html.push("<th style='width:240px'>Cash Flow(RMB)</th>");
            html.push("<th>Actual Rental to LL(RMB)</th>");
            html.push("</tr>");

            if (result.Year != null) {
                html.push("<tr>");
                html.push("<td>" + stringNullConvert(result.Year - 1) + "</td>");
                html.push("<td>" + stringNullConvert(result.CompSalePreYr1) + "</td>");
                html.push("<td>" + stringNullConvertCurrency(result.SOIPreYr1) + "</td>");
                html.push("<td>" + stringNullConvertCurrency(result.CashFlowPreYr1) + "</td>");
                html.push("<td>" + stringNullConvertCurrency(result.RentalPaidtoLLPreYr1) + "</td>");
                html.push("</tr>");

                html.push("<tr>");
                html.push("<td>" + stringNullConvert((result.Year - 2)) + "</td>");
                html.push("<td>" + stringNullConvert(result.CompSalePreYr2) + "</td>");
                html.push("<td>" + stringNullConvertCurrency(result.SOIPreYr2) + "</td>");
                html.push("<td>" + stringNullConvertCurrency(result.CashFlowPreYr2) + "</td>");
                html.push("<td>" + stringNullConvertCurrency(result.RentalPaidtoLLPreYr2) + "</td>");
                html.push("</tr>");

                html.push("<tr>");
                html.push("<td>" + stringNullConvert((result.Year - 3)) + "</td>");
                html.push("<td>" + stringNullConvert(result.CompSalePreYr3) + "</td>");
                html.push("<td>" + stringNullConvertCurrency(result.SOIPreYr3) + "</td>");
                html.push("<td>" + stringNullConvertCurrency(result.CashFlowPreYr3) + "</td>");
                html.push("<td>" + stringNullConvertCurrency(result.RentalPaidtoLLPreYr3) + "</td>");
                html.push("</tr>");
            }

            $("#tabFinanceInfo").html("");
            $("#tabFinanceInfo").append(html.join(""));

        }
    });

    $.ajax({
        type: "GET",
        url: "/StoreList/StoreSTMonthlyFinaceInfoTTMQuery",
        cache: false,
        dataType: "json",
        data: {
        },
        success: function (data) {

            var result = eval(data);

            $("#spanTTMValue")[0].innerText = stringNullConvert(result.TTMValue);

        }
    });
}

