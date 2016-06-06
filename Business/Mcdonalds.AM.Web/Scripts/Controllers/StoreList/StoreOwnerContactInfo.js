

var debugLoadStoreOwnerContactInfo = false;

var results = null;

//  加载详情页-OwnerContactInfo
function loadStoreOwnerContactInfo(USCode) {

    $.ajax({
        type: "GET",
        url: "/StoreList/StoreSTLLRecordQuery",
        cache: false,
        dataType: "json",
        data: {
            _USCode: USCode
        },
        success: function (data) {
            if (debugLoadStoreOwnerContactInfo) alert('load StoreSTLLRecordQuery');
            var result = eval(data);

            $("#divLL0Contact")[0].innerText = stringNullConvert(result.LL0Contact);
            $("#divLL0Title")[0].innerText = stringNullConvert(result.LL0Title);
            $("#divLL0Tel")[0].innerText = stringNullConvert(result.LL0Tel);
            $("#divLL0Address")[0].innerText = stringNullConvert(result.LL0Address);

            $("#divLL1Contact")[0].innerText = stringNullConvert(result.LL1Contact);
            $("#divLL1Title")[0].innerText = stringNullConvert(result.LL1Title);
            $("#divLL1Tel")[0].innerText = stringNullConvert(result.LL1Tel);
            $("#divLL1Mobile")[0].innerText = stringNullConvert(result.LL1Mobile);

            $("#divLL2Contact")[0].innerText = stringNullConvert(result.LL2Contact);
            $("#divLL2Title")[0].innerText = stringNullConvert(result.LL2Title);
            $("#divLL2Tel")[0].innerText = stringNullConvert(result.LL2Tel);
            $("#divLL2Mobile")[0].innerText = stringNullConvert(result.LL2Mobile);

            $("#divLL3Contact")[0].innerText = stringNullConvert(result.LL3Contact);
            $("#divLL3Title")[0].innerText = stringNullConvert(result.LL3Title);
            $("#divLL3Tel")[0].innerText = stringNullConvert(result.LL3Tel);
            $("#divLL3Mobile")[0].innerText = stringNullConvert(result.LL3Mobile);


            $("#divIsBroker")[0].innerText = (stringNullConvert(result.IsBroker) == "1" ? "是" : "否");
            $("#divBrokerName")[0].innerText = stringNullConvert(result.BrokerName);

            $("#divSTLLRecordQueryIsAlliance")[0].innerText = (stringNullConvert(result.IsAlliance) == "1" ? "是" : "否");
            $("#divSTLLRecordQueryAllianceName")[0].innerText = stringNullConvert(result.AllianceName);
            $("#divSTLLRecordQuerySignageCount")[0].innerText = stringNullConvert(result.SignageCount);

            $("#divSTLLRecordQueryIsBigLL")[0].innerText = (stringNullConvert(result.IsBigLL) == "1" ? "是" : "否");
            $("#divSTLLRecordQueryBigLLName")[0].innerText = stringNullConvert(result.BigLLName);
            $("#divSTLLRecordQueryOpenCount")[0].innerText = stringNullConvert(result.OpenCount);

            $("#divIsFromRE")[0].innerText = (stringNullConvert(result.IsFromRE) == "1" ? "是" : "否");
        }
    });

    $.ajax({
        type: "GET",
        url: "/StoreList/StoreSTNegotiationQuery",
        cache: false,
        dataType: "json",
        data: {
            _USCode: USCode
        },
        success: function (data) {
            if (debugLoadStoreOwnerContactInfo) alert('load StoreSTLLRecordListQuery');
            var result = eval(data);
            results = result;

            var html = [];
            html.push("<tr>");
            html.push("<th style='width:60px'>序号</th>");
            html.push("<th style='width:150px'>麦当劳方参与人</th>");
            html.push("<th style='width:180px'>谈判内容</th>");
            html.push("<th style='width:150px'>业主方参与人</th>");
            html.push("<th style='width:120px'>谈判主题</th>");
            html.push("<th>地点</th>");
            html.push("<th style='width:90px'>时间</th>");
            html.push("</tr>");

            for (var i = 0; i < result.length; i++) {
                if (i < result.length) {
                    var plan = result[i];
                    
                    html.push("<tr style='cursor:pointer;' onclick='loadStoreOwnerContactInfoTableRow(" + i + ")' onmouseover='this.style.background=\"#ffd014\";' onmouseout='this.style.background=\"white\";' >");
                    html.push("<td>" + (i + 1) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.McdParticipants) + "</td>");
                    html.push("<td style='white-space:nowrap;overflow:hidden;word-break:break-all;' title='" + stringNullConvert(plan.Content) + "'>" + stringNullConvert(plan.Content) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.LLparticipants) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.Topic) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.Location) + "</td>");
                    html.push("<td>" + datetimeConvert(plan.DateTime) + "</td>");
                    html.push("</tr>");
                }
            }
            $("#tabOwnerContactInfo").html("");
            $("#tabOwnerContactInfo").append(html.join(""));

            if (result.length > 0) {
                var plan = result[0];

                $("#divDateTime")[0].innerText = datetimeConvert(plan.DateTime);
                $("#divLocation")[0].innerText = stringNullConvert(plan.Location);

                $("#divTopic")[0].innerText = stringNullConvert(plan.Topic);
                $("#divMcdParticipants")[0].innerText = stringNullConvert(plan.McdParticipants);
                $("#divLLparticipants")[0].innerText = stringNullConvert(plan.LLparticipants);

                $("#divStoreOwnerContactInfoContent")[0].innerText = stringNullConvert(plan.Content);

            }
        }
    });
}
function loadStoreOwnerContactInfoTableRow(index) {

    $("#divDateTime")[0].innerText = datetimeConvert(results[index].DateTime);
    $("#divLocation")[0].innerText = stringNullConvert(results[index].Location);

    $("#divTopic")[0].innerText = stringNullConvert(results[index].Topic);
    $("#divMcdParticipants")[0].innerText = stringNullConvert(results[index].McdParticipants);
    $("#divLLparticipants")[0].innerText = stringNullConvert(results[index].LLparticipants);

    $("#divStoreOwnerContactInfoContent")[0].innerText = stringNullConvert(results[index].Content);
}


