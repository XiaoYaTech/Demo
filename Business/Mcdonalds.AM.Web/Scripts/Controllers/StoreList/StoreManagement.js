
//  加载详情页-StoreManagement
function loadStoreManagement(USCode) {
    loadStoreManagement2(USCode);
    return;

    $("#imgStoreListDetial")[0].style.display = '';
    
    $.ajax({
        type: "GET",
        url: "/StoreList/StoreQuery",
        cache: false,
        dataType: "json",
        data: {
            _USCode: USCode
        },
        success: function (data) {

            var result = eval(data);

            $("#biUSCode")[0].innerText = stringNullConvert(result.Code);
            $("#biStoreNameEN")[0].innerText = stringNullConvert(result.NameENUS);
            $("#biStoreNameCN")[0].innerText = stringNullConvert(result.NameZHCN);

            $("#biRegion")[0].innerText = stringNullConvert(result.RegionName);
            $("#biMarket")[0].innerText = stringNullConvert(result.MarketName);
            $("#biCityName")[0].innerText = stringNullConvert(result.CityName);

            $("#biDistrict")[0].innerText = stringNullConvert(result.DistrictName);
            $("#biMMName")[0].innerText = stringNullConvert(result.MMName);
            $("#biTA")[0].innerText = stringNullConvert(result.TATypeName);

            $("#biStoreType")[0].innerText = stringNullConvert(result.StoreTypeName);
            $("#biPortfolioType")[0].innerText = stringNullConvert(result.PortfolioTypeName);
            $("#biDTType")[0].innerText = stringNullConvert(result.DTTypeName);

            $("#biIsAlliance")[0].innerText = (stringNullConvert(result.IsAlliance) == "1" ? "Yes" : "No");
            $("#biAllianceName")[0].innerText = stringNullConvert(result.AllianceName);
            $("#biTVMarket")[0].innerText = (stringNullConvert(result.TVMarket) == "1" ? "Yes" : "No");

            $("#biIsBigLL")[0].innerText = (stringNullConvert(result.IsBigLL) == "1" ? "Yes" : "No");
            $("#biBigLLName")[0].innerText = stringNullConvert(result.BigLLName);
            $("#biStatus")[0].innerText = stringNullConvert(result.StoreStatus);

            $("#biOpenDate")[0].innerText = datetimeConvert(result.OpenDate);
            $("#biReimageDate")[0].innerText = datetimeConvert(result.ReImagingDate);
            $("#biCloseDate")[0].innerText = datetimeConvert(result.CloseDate);

            $("#biAddressZHCN")[0].innerText = stringNullConvert(result.AddressZHCN);
            $("#biAddressENUS")[0].innerText = stringNullConvert(result.AddressENUS);

            $("#biAssetRep")[0].innerText = stringNullConvert(result.AssetRepName);         //AssetRepAD
            $("#biAssetMgr")[0].innerText = stringNullConvert(result.AMName);               //AMAD
            $("#biPlanner")[0].innerText = stringNullConvert(result.PlannerName);           //PlannerAD

            $("#biProjectmanager")[0].innerText = stringNullConvert(result.PMName);         //PMAD
            $("#biPlannerMgr")[0].innerText = stringNullConvert(result.PlanningMgrName);    //PlanningMgrAD
            $("#biConsMgr")[0].innerText = stringNullConvert(result.CMName);                //CMAD

            $("#biREPep")[0].innerText = stringNullConvert(result.RepName);
            $("#biREMgr")[0].innerText = stringNullConvert(result.RepMgrName);

            $("#biVPGM")[0].innerText = stringNullConvert(result.VPGMName);                 //VPGMAD
            $("#biGM")[0].innerText = stringNullConvert(result.GM);
            $("#biDO")[0].innerText = stringNullConvert(result.DO);

            $("#biOM")[0].innerText = stringNullConvert(result.OM);
            $("#biOC")[0].innerText = stringNullConvert(result.OCName);
            $("#biStoreManager")[0].innerText = stringNullConvert(result.StoreMgrName);     //StoreMgrAD

            $("#biStorePhone")[0].innerText = stringNullConvert(result.Tel);
            $("#biStoreEmail")[0].innerText = stringNullConvert(result.Email);

            $("#imgStoreListDetial")[0].style.display = 'none';
        }
    });
}


function loadStoreManagement2(USCode) {

    $("#imgStoreListDetial")[0].style.display = '';

    $.ajax({
        type: "GET",
        url: "/StoreList/StoreBasicInfoQuery",
        cache: false,
        dataType: "json",
        data: {
            _USCode: USCode
        },
        success: function (data) {

            var result = eval(data);

            $("#biUSCode")[0].innerText = stringNullConvert(result.StoreCode);
            $("#biStoreNameEN")[0].innerText = stringNullConvert(result.NameENUS);
            $("#biStoreNameCN")[0].innerText = stringNullConvert(result.NameZHCN);

            $("#biRegion")[0].innerText = stringNullConvert(result.RegionZHCN);
            $("#biMarket")[0].innerText = stringNullConvert(result.MarketZHCN);
            $("#biCityName")[0].innerText = stringNullConvert(result.CityZHCN);

            $("#biDistrict")[0].innerText = stringNullConvert(result.DistrictZHCN);
            $("#biMMName")[0].innerText = stringNullConvert(result.MMZHCN);
            $("#biTA")[0].innerText = stringNullConvert(result.TAName);

            $("#biStoreType")[0].innerText = stringNullConvert(result.StoreTypeName);
            $("#biPortfolioType")[0].innerText = stringNullConvert(result.PortfolioTypeName);
            $("#biDTType")[0].innerText = stringNullConvert(result.DTModel);

            $("#biIsAlliance")[0].innerText = (stringNullConvert(result.IsAlliance) == "1" ? "Yes" : "No");
            $("#biAllianceName")[0].innerText = stringNullConvert(result.AllianceCodeName);
            $("#biTVMarket")[0].innerText = (stringNullConvert(result.TVMarket) == "1" ? "Yes" : "No");

            $("#biIsBigLL")[0].innerText = (stringNullConvert(result.IsBigLL) == "1" ? "Yes" : "No");
            $("#biBigLLName")[0].innerText = stringNullConvert(result.BigLLName);
            $("#biStatus")[0].innerText = stringNullConvert(result.statusName);

            $("#biOpenDate")[0].innerText = datetimeConvert(result.OpenDate);
            $("#biReimageDate")[0].innerText = datetimeConvert(result.ReImageDate);
            $("#biCloseDate")[0].innerText = datetimeConvert(result.CloseDate);

            $("#biAddressZHCN")[0].innerText = stringNullConvert(result.AddressZHCN);
            $("#biAddressENUS")[0].innerText = stringNullConvert(result.AddressENUS);

            $("#imgStoreListDetial")[0].style.display = 'none';
        }
    });

    $.ajax({
        type: "GET",
        url: "/StoreList/StoreDevelopQuery",
        cache: false,
        dataType: "json",
        data: {
            _USCode: USCode
        },
        success: function (data) {

            var result = eval(data);

            $("#biAssetRep")[0].innerText = stringNullConvert(result.AssetRepName);
            $("#biAssetMgr")[0].innerText = stringNullConvert(result.AssetMgrName);
            $("#biPlanner")[0].innerText = stringNullConvert(result.PlannerName);

            $("#biProjectmanager")[0].innerText = stringNullConvert(result.ProjectManagerName);
            $("#biPlannerMgr")[0].innerText = stringNullConvert(result.PlannerMgrName);
            $("#biConsMgr")[0].innerText = stringNullConvert(result.ConsMgrName);

            $("#biREPep")[0].innerText = stringNullConvert(result.RERepName);
            $("#biREMgr")[0].innerText = stringNullConvert(result.REMgrName);

            $("#imgStoreListDetial")[0].style.display = 'none';
        }
    });

    $.ajax({
        type: "GET",
        url: "/StoreList/StoreOpQuery",
        cache: false,
        dataType: "json",
        data: {
            _USCode: USCode
        },
        success: function (data) {

            var result = eval(data);

            $("#biVPGM")[0].innerText = stringNullConvert(result.VPGMName);
            $("#biGM")[0].innerText = stringNullConvert(result.GMName);
            $("#biDO")[0].innerText = stringNullConvert(result.DOName);

            $("#biOM")[0].innerText = stringNullConvert(result.OMName);
            $("#biOC")[0].innerText = stringNullConvert(result.OCName);
            $("#biStoreManager")[0].innerText = stringNullConvert(result.StoreManagerName);

            $("#biStorePhone")[0].innerText = stringNullConvert(result.Tel);
            $("#biStoreEmail")[0].innerText = stringNullConvert(result.Email);

            $("#imgStoreListDetial")[0].style.display = 'none';
        }
    });
}

