//不允许上传下载的文档编号
var disableUploadDocType = [660, 664, 668];

//  加载详情页-Licenses
function loadStoreLicenses(USCode) {

    $.ajax({
        type: "GET",
        url: "/StoreList/StoreSTLicenseQuery",
        cache: false,
        dataType: "json",
        data: {
            _USCode: USCode
        },
        success: function (data) {

            var result = eval(data);

            var html = [];
            html.push("<tr>");
            html.push("<th style='width:50px'>序号</th>");
            html.push("<th style='width:220px'>附件名称</th>");
            html.push("<th style='width:80px'>当前状态</th>");
            html.push("<th style='width:200px'>附件</th>");
            html.push("<th style='width:100px'>创建时间</th>");
            html.push("<th style='width:100px'>修改时间</th>");
            html.push("<th style='width:100px'>End Date</th>");
            html.push("<th style='width:100px'>创建人</th>");
            html.push("<th>操作</th>");
            html.push("</tr>");

            for (var i = 0; i < result.length; i++) {
                var plan = result[i];
                html.push("<tr>");
                html.push("<td>" + (i + 1) + "</td>");
                html.push("<td>" + stringNullConvert(plan.Title) + "</td>");
                html.push("<td><input type='checkbox' disabled " + (stringNullConvert(plan.Status) == "1" ? "checked" : "") + "></td>");
                html.push("<td>" + stringNullConvert(plan.DocName) + "</td>");
                html.push("<td>" + datetimeConvert(plan.CreateDate) + "</td>");
                html.push("<td>" + datetimeConvert(plan.ModifyDate) + "</td>");
                html.push("<td>" + datetimeConvert(plan.EndDate) + "</td>");
                html.push("<td>" + stringNullConvert(plan.Owner) + "</td>");

                if ($.inArray(plan.DocType, disableUploadDocType) >= 0)
                    html.push("<td></td>");
                else {
                    if (!canEdit()) {
                        html.push("<td>");
                        if (!stringNullConvert(plan.FilePath).length == 0)
                            html.push("<a href='" + Utils.ServiceURI.AttachmentAddress() + stringNullConvert(plan.FilePath) + "' target=\"_Blank\">[下载]</a>");
                        html.push("</td>");
                    }
                    else {
                        html.push("<td>");
                        html.push("<a plupload-simple id='upload" + i + "' upload-url='api/StoreSTLicense/upload/" + plan.StoreCode + "/" + plan.Id + "' upload-finished='uploadFinished' error-msg='上传失败'>[上传]</a>");
                        if (!stringNullConvert(plan.FilePath).length == 0) {
                            html.push("<a href='" + Utils.ServiceURI.AttachmentAddress() + stringNullConvert(plan.FilePath) + "' target=\"_Blank\">[下载]</a>");
                            html.push("<a onclick='deleteStoreLicenseItem(\"" + plan.Id + "\")'>[删除]</a>");
                        }
                        html.push("</td>");
                    }
                }

                html.push("</tr>");
            }
            $("#tabAssetManagement").html("");
            $("#tabAssetManagement").append(html.join(""));
            initPlupload(USCode);
        }
    });
}

function canEdit () {
    if ($.inArray("am_sm_asset_lic_edit", window.currentUser.RightCodes || []) >= 0)
        return true;
    return false;
}

function deleteStoreLicenseItem(id) {
    $.ajax({
        type: "POST",
        url: Utils.ServiceURI.Address() + "/api/StoreSTLicense/delete/" + id,
        cache: false,
        success: function (data) {
            if (data)
                loadStoreLicenses(data);
        }
    });
}

function uploadFinished(USCode) {
    loadStoreLicenses(USCode);
}

function initPlupload(USCode) {
    $("a[plupload-simple]").each(function () {
        var id = this.id;
        if (!id) {
            id = Utils.Generator.newGuid();
            $(this).attr("id", id);
        }
        var uploadUrl = Utils.ServiceURI.Address() + $(this).attr("upload-url");
        var uploadFinish = $(this).attr("upload-finished");
        var isError = false;
        var uploader = new plupload.Uploader({
            runtimes: 'html5,flash,silverlight,html4',
            browse_button: id,
            //container: element.parent().get(0),
            multi_selection: false,
            url: uploadUrl,
            flash_swf_url: Utils.ServiceURI.AppUri + 'Content/plupload/Moxie.swf',
            silverlight_xap_url: Utils.ServiceURI.AppUri + 'Content/plupload/Moxie.xap',
            filters: {
                max_file_size: "1024mb",
                mime_types: [{ 'title': 'All Files', 'extensions': '*' }]
            },
            init: {
                PostInit: function () {
                },
                FilesAdded: function (up, files) {
                    uploader.start();
                },
                UploadComplete: function (up, files) {
                    if (!isError) {
                        if (uploadFinish)
                            eval(uploadFinish + "('" + USCode + "')");
                    }
                    isError = false;
                },
                Error: function (up, err) {
                    isError = true;
                }
            }
        });

        uploader.init();
    });
}
