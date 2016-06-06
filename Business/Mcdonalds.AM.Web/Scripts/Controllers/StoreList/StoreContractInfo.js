

var debugLoadStoreContractInfo = false;
var loadContractInfoItem1 = false;
var loadContractInfoItem2 = false;
var loadContractInfoItem3 = false;

//  加载详情-StoreContractInfo
function loadStoreContractInfo(USCode) {

    $.ajax({
        type: "GET",
        url: "/StoreList/StoreContractInfoListQuery",
        cache: false,
        dataType: "json",
        data: {
            _USCode: USCode
        },
        success: function (data) {
            if (debugLoadStoreContractInfo) alert('load StoreContractInfoListQuery!');
            var result = eval(data);

            var html = [];
            html.push("<tr>");
            html.push("<th style='width:120px'>合同编号</th>");
            html.push("<th style='width:80px'>合同类型</th>");
            html.push("<th style='width:80px'>租赁面积（sqm）</th>");
            html.push("<th style='width:120px'>租赁/土地购置期限（Years）</th>");
            html.push("<th style='width:80px'>预估合同起始日</th>");
            html.push("<th style='width:80px'>预估合同到期日</th>");
            html.push("<th style='width:90px'>合同起始年</th>");
            html.push("<th style='width:90px'>合同到期年</th>");
            html.push("<th style='width:80px'>Rent Type</th>");
            html.push("<th style='width:100px'>续租通知最迟提交日</th>");
            html.push("<th style='width:100px'>租金起付日</th>");
            //html.push("<th>附件</th>");
            html.push("</tr>");

            for (var i = 0; i < result.length; i++) {
                if (i < result.length) {
                    var plan = result[i];

                    html.push("<tr style='cursor:pointer;' onclick='loadStoreContractInfoById(\"" + plan.Id + "\"," + plan.LeaseRecapID + ");' onmouseover='this.style.background=\"#ffd014\";' onmouseout='this.style.background=\"white\";' >");
                    html.push("<td>" + stringNullConvert(plan.StoreCode + "-" + plan.StartYear) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.LeasePurchase) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.TotalLeasedArea) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.LeasePurchaseTerm) + "</td>");
                    html.push("<td>" + datetimeConvert(plan.StartDate) + "</td>");
                    html.push("<td>" + datetimeConvert(plan.EndDate) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.StartYear) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.EndYear) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.RentType) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.DeadlineToNotice) + "</td>");
                    html.push("<td>" + datetimeConvert(plan.RentCommencementDate) + "</td>");
                    //html.push("<td><a href='javascript:void(0);'>附件(0)</a></td>");
                    html.push("</tr>");
                }
            }
            $("#tabContractInfo2").html("");
            $("#tabContractInfo2").append(html.join(""));

            if (result.length > 0) {
                //  加载最新的合同详情及修订记录
                loadStoreContractInfoById(result[0].Id, result[0].LeaseRecapID);
            }
        }
    });
}


//  加载指定ID的合同详情及修订记录
function loadStoreContractInfoById(Id, LeaseRecapID) {

    $("#imgStoreListDetialContractInfo")[0].style.display = '';
    loadContractInfoItem1 = false;
    loadContractInfoItem2 = false;
    loadContractInfoItem3 = false;

    //  合同详情
    $.ajax({
        type: "GET",
        url: "/StoreList/StoreContractInfoByIDQuery",
        cache: false,
        dataType: "json",
        data: {
            _ID: Id
        },
        success: function (data) {
            if (debugLoadStoreContractInfo) alert('load StoreContractInfoByIDQuery!');
            var result = eval(data);

            $("#divPartyAFullName")[0].innerText = stringNullConvert(result.PartyAFullName);
            $("#divMcDLegalEntity")[0].innerText = stringNullConvert(result.McDLegalEntity);

            $("#divMcDOwnership")[0].innerText = stringNullConvert(result.McDOwnership);
            $("#divContactPerson")[0].innerText = stringNullConvert(result.ContactPerson);
            $("#divContactMode")[0].innerText = stringNullConvert(result.ContactMode);

            $("#divRentType")[0].innerText = stringNullConvert(result.RentType);
            $("#divTotalLeasedArea")[0].innerText = stringNullConvert(result.TotalLeasedArea);

            $("#divLeasePurchase")[0].innerText = stringNullConvert(result.LeasePurchase);
            $("#divStartDate")[0].innerText = datetimeConvert(result.StartDate);
            //$("#divStoreContractInfoOverallAccessibility")[0].innerText = stringNullConvert(result.OverallAccessibility);

            //$("#divRentSupervalueDay")[0].innerText = datetimeConvert(result.RentSupervalueDay);
            $("#divEndDate")[0].innerText = datetimeConvert(result.EndDate);
            //$("#divStoreContractInfoKeyword3")[0].innerText = stringNullConvert(result.Keyword3);

            $("#divStartYear")[0].innerText = stringNullConvert(result.StartYear);
            $("#divRentCommencementDate")[0].innerText = datetimeConvert(result.RentCommencementDate);
            $("#divEndYear")[0].innerText = stringNullConvert(result.EndYear);

            $("#divDeadlineToNotice")[0].innerText = stringNullConvert(result.DeadlineToNotice);
            $("#divChangedafter2010")[0].innerText = result.Changedafter2010 == 0 ? "否" : "是";

            $("#divRentStructure")[0].innerText = stringNullConvert(result.RentStructure);

            $("#divWithEarlyTerminationClause")[0].innerText = (stringNullConvert(result.WithEarlyTerminationClause) == "1" ? "Yes" : "No");

            $("#divEarlyTerminationClauseDetail")[0].innerText = stringNullConvert(result.EarlyTerminationClauseDetail);


            $("#divRentalPaidto")[0].innerText = stringNullConvert(result.RentalPaidto);
            $("#divRentPaymentArrangement")[0].innerText = stringNullConvert(result.RentPaymentArrangement);
            $("#divHasDeposit")[0].innerText = stringNullConvertYesNoZHCN(result.HasDeposit);

            $("#divDepositAmount")[0].innerText = stringNullConvert(result.DepositAmount);
            $("#divRefundable")[0].innerText = stringNullConvertYesNoZHCN(result.Refundable);
            $("#divRefundableDate")[0].innerText = datetimeConvert(result.RefundableDate);

            $("#divWithPenaltyClause")[0].innerText = stringNullConvertYesNoZHCN(result.WithPenaltyClause);
            $("#divHasBankGuarantee")[0].innerText = stringNullConvertYesNoZHCN(result.HasBankGuarantee);
            $("#divBGNumber")[0].innerText = stringNullConvert(result.BGNumber);

            $("#divBGAmount")[0].innerText = stringNullConvert(result.BGAmount);
            $("#divBGCommencementDate")[0].innerText = datetimeConvert(result.BGCommencementDate);
            $("#divBGEndDate")[0].innerText = datetimeConvert(result.BGEndDate);

            $("#divRemarks")[0].innerText = stringNullConvert(result.Remarks);

            $.ajax({
                type: "GET",
                url: "/StoreList/StoreMMInfoQuery",
                cache: false,
                dataType: "json",
                data: {
                    _USCode: result.StoreCode
                },
                success: function (data) {
                    var result = eval(data);
                    $("#divLeasePurchaseTerm")[0].innerText = stringNullConvert(result.LocationRatingPP);
                    loadContractInfoItem1 = true;
                    if (loadContractInfoItem1 && loadContractInfoItem2 && loadContractInfoItem3) $("#imgStoreListDetialContractInfo")[0].style.display = 'none';
                }
            });
        }
    });

    //  修订记录
    $.ajax({
        type: "GET",
        url: "/StoreList/StoreContractRevisionQuery",
        cache: false,
        dataType: "json",
        data: {
            _LeaseRecapID: LeaseRecapID
        },
        success: function (data) {
            if (debugLoadStoreContractInfo) alert('load StoreContractRevisionQuery!');
            var result = eval(data);

            var html = [];
            html.push("<tr>");
            html.push("<th style='width:50px'>+</th>");
            html.push("<th style='width:120px'>修订日期</th>");
            html.push("<th>Changes Type</th>");
            html.push("</tr>");
            for (var i = 0; i < result.length   ; i++) {
                if (i < result.length) {
                    var plan = result[i];

                    html.push("<tr>");
                    html.push("<td rowspan='2'>-</td>");
                    html.push("<td rowspan='2'>" + datetimeConvert(plan.ChangeDate) + "</td>");
                    html.push("<td><table width='100%' class='table'><tr valign='top'>\
                               <td width='200'><input name='checkbox' type='checkbox' disabled " + (stringNullConvert(plan.Rent) == "Y" ? "checked='checked'" : "") + "   /> The change of the rental</td>\
                               <td style='width:200px'><div>Old RentStructure</div><textarea class='form-control' rows='3' disabled='disabled'>" + stringNullConvert(plan.RentStructureOld) + "</textarea></td>\
                               <td style='width:200px'><div>New RentStructure</div><textarea class='form-control' rows='3' disabled='disabled'>" + stringNullConvert(plan.RentStructureNew) + "</textarea></td>\
                               </tr>");

                    html.push("<tr valign='top'><td><input name='checkbox' type='checkbox' disabled " + (stringNullConvert(plan.Size) == "Y" ? "checked='checked'" : "") + "   /> The change of  redline</td>\
                               <td>Old Redline Area : " + stringNullConvert(plan.RedlineAreaOld) + "</td>\
                               <td>New Redline Area : " + stringNullConvert(plan.RedlineAreaNew) + "</td>\
                               </tr>");

                    html.push("<tr valign='top'><td><input name='checkbox' type='checkbox' disabled " + (stringNullConvert(plan.LeaseTerm) == "Y" ? "checked='checked'" : "") + " /> The change of lease term</td>\
                               <td>Old LeaseChangeExpiry : " + datetimeConvert(plan.LeaseChangeExpiryOld) + "</td>\
                               <td>New LeaseChangeExpiry : " + datetimeConvert(plan.LeaseChangeExpiryNew) + "</td>\
                               </tr>");

                    html.push("<tr valign='top'><td><input name='checkbox' type='checkbox' disabled " + (stringNullConvert(plan.Entity) == "Y" ? "checked='checked'" : "") + " /> The change of landlord</td>\
                               <td>Old Landlord : " + stringNullConvert(plan.LandlordOld) + "</td>\
                               <td>New Landlord : " + stringNullConvert(plan.LandlordOld) + "</td>\
                               </tr>");
                    html.push("<tr valign='top'><td><input name='checkbox' type='checkbox' disabled " + (stringNullConvert(plan.Others) == "Y" ? "checked='checked'" : "") + "   /> Others</td>\
                               <td colspan='2'>" + stringNullConvert(plan.OthersDescription) + "</td>\
                               </tr></table></td>");
                    html.push("</tr>");

                    html.push("<tr>");
                    html.push("<td><div>Description</div><pre class='mg_t_10'>" + stringNullConvert(plan.Description) + "</pre></td>");
                    html.push("</tr>");
                }
            }
            $("#tabContractInfo1").html("");
            $("#tabContractInfo1").append(html.join(""));

            loadContractInfoItem2 = true;
            if (loadContractInfoItem1 && loadContractInfoItem2 && loadContractInfoItem3) $("#imgStoreListDetialContractInfo")[0].style.display = 'none';
        }
    });

    //  附件记录
    $.ajax({
        type: "GET",
        url: "/StoreList/StoreContractInfoAttachedQuery",
        cache: false,
        dataType: "json",
        data: {
            _LeaseRecapID: LeaseRecapID
        },
        success: function (data) {
            if (debugLoadStoreContractInfo) alert('load StoreContractInfoAttachedQuery!');
            var result = eval(data);

            var html = [];
            html.push("<tr>");
            html.push("<th style='width:50px'>序号</th>");
            html.push("<th style='width:240px'>附件名称</th>");
            html.push("<th style='width:80px'>当前状态</th>");
            html.push("<th style='width:240px'>附件</th>");
            html.push("<th style='width:100px'>创建时间</th>");
            html.push("<th style='width:100px'>修改时间</th>");
            html.push("<th style='width:100px'>备注</th>");
            html.push("<th style='width:80px'>创建人</th>");
            html.push("<th>操作</th>");
            html.push("</tr>");
            for (var i = 0; i < result.length   ; i++) {
                if (i < result.length) {
                    var plan = result[i];

                    var htmlStatus = "<input name='checkbox' type='checkbox' disabled " + (stringNullConvert(plan.Status) == "1" ? "checked='checked'" : "") + " />";

                    html.push("<tr>");
                    html.push("<td>" + (i + 1) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.Title) + "</td>");
                    html.push("<td>" + htmlStatus + "</td>");
                    html.push("<td>" + stringNullConvert(plan.DocName) + "</td>");
                    html.push("<td>" + datetimeConvert(plan.CreateDate) + "</td>");
                    html.push("<td>" + datetimeConvert(plan.ModifyDate) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.Comments) + "</td>");
                    html.push("<td>" + stringNullConvert(plan.Owner) + "</td>");
                    html.push("<td><a href='" + Utils.ServiceURI.AttachmentAddress() + stringNullConvert(plan.FilePath) + "' target=\"_Blank\">下载</a></td>");
                    html.push("</tr>");
                }
            }
            $("#tabContractInfo3").html("");
            $("#tabContractInfo3").append(html.join(""));

            loadContractInfoItem3 = true;
            if (loadContractInfoItem1 && loadContractInfoItem2 && loadContractInfoItem3) $("#imgStoreListDetialContractInfo")[0].style.display = 'none';
        }
    });
}

