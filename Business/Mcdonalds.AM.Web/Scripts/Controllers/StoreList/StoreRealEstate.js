
//  加载详情页-RealEstate
function loadRealEstate(USCode) {

    $.ajax({
        type: "GET",
        url: "/StoreList/StoreRealEstateQuery",
        cache: false,
        dataType: "json",
        data: {
            _USCode: USCode
        },
        success: function (data) {

            var result = eval(data);

            $("#divLL1Contact1")[0].innerText = stringNullConvert(result.LL1Contact1);
            $("#divLL1Title1")[0].innerText = stringNullConvert(result.LL1Title1);
            $("#divLL1Tel1")[0].innerText = stringNullConvert(result.LL1Tel1);
            $("#divLL1Mobile1")[0].innerText = stringNullConvert(result.LL1Mobile1);

            $("#divLL1Contact2")[0].innerText = stringNullConvert(result.LL1Contact2);
            $("#divLL1Title2")[0].innerText = stringNullConvert(result.LL1Title2);
            $("#divLL1Tel2")[0].innerText = stringNullConvert(result.LL1Tel2);
            $("#divLL1Mobile2")[0].innerText = stringNullConvert(result.LL1Mobile2);

            $("#divLL1ZipCode")[0].innerText = stringNullConvert(result.LL1ZipCode);
            $("#divLL1Address")[0].innerText = stringNullConvert(result.LL1Address);

            $("#divLL2Contact1")[0].innerText = stringNullConvert(result.LL2Contact1);
            $("#divLL2Title1")[0].innerText = stringNullConvert(result.LL2Title1);
            $("#divLL2Tel1")[0].innerText = stringNullConvert(result.LL2Tel1);
            $("#divLL2Mobile1")[0].innerText = stringNullConvert(result.LL2Mobile1);

            $("#divLL2Contact2")[0].innerText = stringNullConvert(result.LL2Contact2);
            $("#divLL2Title2")[0].innerText = stringNullConvert(result.LL2Title2);
            $("#divLL2Tel2")[0].innerText = stringNullConvert(result.LL2Tel2);
            $("#divLL2Mobile2")[0].innerText = stringNullConvert(result.LL2Mobile2);

            $("#divLL2ZipCode")[0].innerText = stringNullConvert(result.LL2ZipCode);
            $("#divLL2Address")[0].innerText = stringNullConvert(result.LL2Address);

            $("#divOwnerContact1")[0].innerText = stringNullConvert(result.OwnerContact1);
            $("#divOwnerTitle1")[0].innerText = stringNullConvert(result.OwnerTitle1);
            $("#divOwnerTel1")[0].innerText = stringNullConvert(result.OwnerTel1);
            $("#divOwnerMobile1")[0].innerText = stringNullConvert(result.OwnerMobile1);

            $("#divOwnerContact2")[0].innerText = stringNullConvert(result.OwnerContact2);
            $("#divOwnerTitle2")[0].innerText = stringNullConvert(result.OwnerTitle2);
            $("#divOwnerTel2")[0].innerText = stringNullConvert(result.OwnerTel2);
            $("#divOwnerMobile2")[0].innerText = stringNullConvert(result.OwnerMobile2);

            $("#divOwnerZipCode")[0].innerText = stringNullConvert(result.OwnerZipCode);
            $("#divOwnerAddress")[0].innerText = stringNullConvert(result.OwnerAddress);

            $("#divPropertyContact1")[0].innerText = stringNullConvert(result.PropertyContact1);
            $("#divPropertyTitle1")[0].innerText = stringNullConvert(result.PropertyTitle1);
            $("#divPropertyTel1")[0].innerText = stringNullConvert(result.PropertyTel1);
            $("#divPropertyMobile1")[0].innerText = stringNullConvert(result.PropertyMobile1);

            $("#divPropertyContact2")[0].innerText = stringNullConvert(result.PropertyContact2);
            $("#divPropertyTitle2")[0].innerText = stringNullConvert(result.PropertyTitle2);
            $("#divPropertyTel2")[0].innerText = stringNullConvert(result.PropertyTel2);
            $("#divPropertyMobile2")[0].innerText = stringNullConvert(result.PropertyMobile2);

            $("#divPropertyZipCode")[0].innerText = stringNullConvert(result.PropertyZipCode);
            $("#divPropertyAddress")[0].innerText = stringNullConvert(result.PropertyAddress);


            $("#divProperty1YesNo")[0].innerText = (stringNullConvert(result.Property1YesNo) == "1" ? "是" : "否");
            $("#divProperty1PlanTime")[0].innerText = datetimeConvert(result.Property1PlanTime);
            $("#divProperty1FinalTime")[0].innerText = datetimeConvert(result.Property1FinalTime);

            $("#divProperty2YesNo")[0].innerText = (stringNullConvert(result.Property2YesNo) == "1" ? "是" : "否");
            $("#divProperty2Remark")[0].innerText = stringNullConvert(result.Property2Remark);

            $("#divProperty3YesNo")[0].innerText = (stringNullConvert(result.Property3YesNo) == "1" ? "是" : "否");
            $("#divProperty3Remark")[0].innerText = stringNullConvert(result.Property3Remark);

            $("#divProperty4YesNo")[0].innerText = (stringNullConvert(result.Property4YesNo) == "1" ? "是" : "否");
            $("#divProperty4Remark")[0].innerText = stringNullConvert(result.Property4Remark);

            $("#divProperty5YesNo")[0].innerText = (stringNullConvert(result.Property5YesNo) == "1" ? "是" : "否");
            $("#divProperty5Remark")[0].innerText = stringNullConvert(result.Property5Remark);

            $("#divProperty6YesNo")[0].innerText = (stringNullConvert(result.Property6YesNo) == "1" ? "是" : "否");
            $("#divProperty6Remark")[0].innerText = stringNullConvert(result.Property6Remark);

            $("#divProperty7YesNo")[0].innerText = (stringNullConvert(result.Property7YesNo) == "1" ? "是" : "否");
            $("#divProperty7Remark")[0].innerText = stringNullConvert(result.Property7Remark);

            $("#divProperty8YesNo")[0].innerText = (stringNullConvert(result.Property8YesNo) == "1" ? "是" : "否");
            $("#divProperty8Remark")[0].innerText = stringNullConvert(result.Property8Remark);

            $("#divProperty9YesNo")[0].innerText = (stringNullConvert(result.Property9YesNo) == "1" ? "是" : "否");
            $("#divProperty9Remark")[0].innerText = stringNullConvert(result.Property9Remark);
        }
    });
}

//  创建HTML表格到RealEstateFileHandover
function loadRealEstateAttachmentQuery(USCode) {

    $.ajax({
        type: "GET",
        url: "/StoreList/StoreRealEstateAttachmentQuery",
        cache: false,
        dataType: "json",
        data: {
            _USCode: USCode
        },
        success: function (data) {

            var result = eval(data);

            var html = [];

            if (result.length > 0) {
                html.push("<tr>");
                html.push("<th style='width:60px'>序号</th>");
                html.push("<th style='width:200px'>附件名称</th>");
                html.push("<th style='width:80px'>当前状态</th>");
                html.push("<th style='width:200px'>附件</th>");
                html.push("<th style='width:100px'>创建时间</th>");
                html.push("<th style='width:100px'>修改时间</th>");
                html.push("<th style='width:100px'>创建人</th>");
                html.push("<th>操作</th>");
                html.push("</tr>");

                for (var i = 0; i < result.length; i++) {
                    var plan = result[i];
                    html.push("<tr>");
                    html.push("<td>&nbsp;" + stringNullConvert(i + 1) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.AttachmentName) + "</td>");
                    html.push("<td><input type='checkbox' disabled " + (stringNullConvert(plan.Stauts) == "1" ? "checked" : "") + "></td>");
                    html.push("<td style='white-space:nowrap;overflow:hidden;word-break:break-all;' title='" + stringNullConvert(plan.DOCName) + "'>" + stringNullConvert(plan.DOCName) + "</td>");
                    html.push("<td>" + datetimeConvert(plan.CreateTime) + "</td>");
                    html.push("<td>" + datetimeConvert(plan.ModifyTime) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.CreatePerson) + "</td>");
                    html.push("<td><a href='javascript:void(0)' onclick='alert(\"" + stringNullConvert(plan.FilePath) + "\");'>下载</a></td>");
                    html.push("</tr>");
                }
            }
            $("#tabRealEstateFileHandover").html("");
            $("#tabRealEstateFileHandover").append(html.join(""));
        }
    });

}
