
//  加载详情页-TA
function loadTA(USCode) {

    $.ajax({
        type: "GET",
        url: "/StoreList/StoreMMInfoQuery",
        cache: false,
        dataType: "json",
        data: {
            _USCode: USCode
        },
        success: function (data) {

            var result = eval(data);

            $("#divMMName")[0].innerText = stringNullConvert(result.MMName);
            $("#divPriority")[0].innerText = stringNullConvert(result.Priority);
            $("#divPopulation")[0].innerText = stringNullConvert(result.Population);


            $("#divTACode")[0].innerText = stringNullConvert(result.TACode);
            $("#divTAName")[0].innerText = stringNullConvert(result.TAName);
            $("#divTAType")[0].innerText = stringNullConvert(result.TAType);

            $("#divTAAPriority")[0].innerText = stringNullConvert(result.TAAPriority);
            $("#divDesirability")[0].innerText = stringNullConvert(result.Desirability);
            $("#divMaturity")[0].innerText = stringNullConvert(result.Maturity);

            $("#divTASize")[0].innerText = stringNullConvertNumber(result.TASize);
            $("#divIncomeLevel")[0].innerText = stringNullConvert(result.IncomeLevel);


            $("#divStoreTypeName")[0].innerText = stringNullConvert(result.StoreTypeName);
            $("#divPortfolioTypeName")[0].innerText = stringNullConvert(result.PortfolioTypeName);
            $("#divLocationRatingPP")[0].innerText = stringNullConvert(result.LocationRatingPP);

            $("#divBackDistance")[0].innerText = stringNullConvertYesNoENUS(result.BackDistance);
            $("#divOverallVisibility")[0].innerText = stringNullConvert(result.OverallVisibility);
            $("#divOverallAccessibility")[0].innerText = stringNullConvert(result.OverallAccessibility);

            $("#divKeyword1")[0].innerText = stringNullConvert(result.Keyword1);
            $("#divKeyword2")[0].innerText = stringNullConvert(result.Keyword2);
            $("#divKeyword3")[0].innerText = stringNullConvert(result.Keyword3);

            $("#divIsNearStreet")[0].innerText = stringNullConvertYesNoENUS(result.IsNearStreet);

            $("#divStrategicInvestmentType")[0].innerText = stringNullConvert(result.StrategicInvestmentType);
            $("#divNatZone")[0].innerText = stringNullConvert(result.NatZone);

            $("#divIsAlliance")[0].innerText = stringNullConvertYesNoENUS(result.IsAlliance);
            $("#divAllianceName")[0].innerText = stringNullConvert(result.AllianceName);

            $("#divIsBigLL")[0].innerText = stringNullConvertYesNoENUS(result.IsBigLL);
            $("#divBigLLName")[0].innerText = stringNullConvert(result.BigLLName);


            $("#divNearestStorePP")[0].innerText = stringNullConvert(result.NearestStorePP);
            $("#divNearestDistancePP")[0].innerText = stringNullConvert(result.NearestDistancePP);

            $("#divNearestMcDOpenDate")[0].innerText = stringNullConvert(result.NearestMcDOpenDate);
            $("#divNearestMcDAUV")[0].innerText = stringNullConvertCurrency(result.NearestMcDAUV);
            $("#divEstimatedImpactOnPp")[0].innerText = stringNullConvertCurrency(result.EstimatedImpactOnPp);

            $("#divSecondNearestStorePP")[0].innerText = stringNullConvert(result.SecondNearestStorePP);
            $("#divSecondNearestDistancePP")[0].innerText = stringNullConvertNumber(result.SecondNearestDistancePP);

            $("#divNearest2ndMcDOpenDate")[0].innerText = stringNullConvert(result.Nearest2ndMcDOpenDate);
            $("#divNearest2ndMcDAUV")[0].innerText = stringNullConvert(result.Nearest2ndMcDAUV);
            $("#divEstimatedImpactOnPp2")[0].innerText = stringNullConvertCurrency(result.EstimatedImpactOnPp2);


            $("#divNearestKFC")[0].innerText = stringNullConvert(result.NearestKFC);
            $("#divNearestKFCDist")[0].innerText = stringNullConvertNumber(result.NearestKFCDist);

            $("#divKFCOpenDate")[0].innerText = stringNullConvert(result.KFCOpenDate);
            $("#divNearestKFCY1Sales")[0].innerText = stringNullConvertCurrency(result.NearestKFCY1Sales);


            $("#divHomePop")[0].innerText = stringNullConvert(result.HomePop);
            $("#divHomeIncomeLevel")[0].innerText = stringNullConvert(result.HomeIncomeLevel);
            $("#divHomeSalesPct")[0].innerText = stringNullConvertPercentage(result.HomeSalesPct);
            $("#divWorkersNo")[0].innerText = stringNullConvert(result.WorkersNo);
            $("#divWorkersProfile")[0].innerText = stringNullConvert(result.WorkersProfile);
            $("#divWorkSalesPct")[0].innerText = stringNullConvertPercentage(result.WorkSalesPct);
            $("#divBuildingCommercialSize")[0].innerText = stringNullConvert(result.BuildingCommercialSize);
            $("#divSchoolStudentNo")[0].innerText = stringNullConvert(result.SchoolStudentNo);
            $("#divSchoolStudentProfile")[0].innerText = stringNullConvert(result.SchoolStudentProfile);
            $("#divSchoolSalesPct")[0].innerText = stringNullConvertPercentage(result.SchoolSalesPct);

            $("#divShoppingLevel")[0].innerText = stringNullConvert(result.ShoppingLevel);
            $("#divShopMallsNo")[0].innerText = stringNullConvert(result.ShopMallsNo);
            $("#divShopTotalCommercialSize")[0].innerText = stringNullConvert(result.ShopTotalCommercialSize);
            $("#divShopSteetLength")[0].innerText = stringNullConvert(result.ShopSteetLength);
            $("#divBuildingNature")[0].innerText = stringNullConvert(result.BuildingNature);
            $("#divBuildingOpenDate")[0].innerText = stringNullConvert(result.BuildingOpenDate);
            $("#divBuildingRetailActStrength")[0].innerText = stringNullConvert(result.BuildingRetailActStrength);
            $("#divBuildingAnchor1Name")[0].innerText = stringNullConvert(result.BuildingAnchor1Name);
            $("#divBuildingAnchor1Size")[0].innerText = stringNullConvert(result.BuildingAnchor1Size);
            $("#divBuildingAnchor2Name")[0].innerText = stringNullConvert(result.BuildingAnchor2Name);
            $("#divBuildingAnchor2Size")[0].innerText = stringNullConvert(result.BuildingAnchor2Size);
            $("#divShopSalesPct")[0].innerText = stringNullConvertPercentage(result.ShopSalesPct);

            $("#divTHType")[0].innerText = stringNullConvert(result.THType);
            $("#divTHStoreLocation")[0].innerText = stringNullConvert(result.THStoreLocation);
            $("#divStoreLocatedon")[0].innerText = stringNullConvert(result.StoreLocatedon);
            $("#divTHTurnoverCount")[0].innerText = stringNullConvert(result.THTurnoverCount);
            $("#divTHMainCmpLocatedon")[0].innerText = stringNullConvert(result.THMainCmpLocatedon);
            $("#divTHSalesPct")[0].innerText = stringNullConvertPercentage(result.THSalesPct);
            $("#divLeisureNature")[0].innerText = stringNullConvert(result.LeisureNature);
            $("#divLeisureSalesPct")[0].innerText = stringNullConvertPercentage(result.LeisureSalesPct);
            $("#divOtherFactorsName")[0].innerText = stringNullConvert(result.OtherFactorsName);
            $("#divOtherSalesPct")[0].innerText = stringNullConvertPercentage(result.OtherSalesPct);

            $("#divHomeRemark")[0].innerText = stringNullConvert(result.HomeRemark);
            $("#divWorkRemark")[0].innerText = stringNullConvert(result.WorkRemark);
            $("#divShopRemark")[0].innerText = stringNullConvert(result.ShopRemark);
            $("#divSchoolRemark")[0].innerText = stringNullConvert(result.SchoolRemark);
            $("#divTHRemark")[0].innerText = stringNullConvert(result.THRemark);
            $("#divLeisureRemark")[0].innerText = stringNullConvert(result.LeisureRemark);
            $("#divOthersRemark")[0].innerText = stringNullConvert(result.OthersRemark);


            $("#divSubwayLines")[0].innerText = stringNullConvert(result.SubwayLines);
            $("#divSubwayClosetExitDist")[0].innerText = stringNullConvert(result.SubwayClosetExitDist);
            $("#divSubwayVisibility")[0].innerText = stringNullConvert(result.SubwayVisibility);

            $("#divBusStopNo")[0].innerText = stringNullConvert(result.BusStopNo);
            $("#divBusLines")[0].innerText = stringNullConvert(result.BusLines);
            $("#divBusClosetStopDist")[0].innerText = stringNullConvert(result.BusClosetStopDist);

            $("#divCompetitorsNo")[0].innerText = stringNullConvert(result.CompetitorsNo);
            $("#divCompetitorsTop3Name")[0].innerText = stringNullConvert(result.CompetitorsTop3Name);
            $("#divKFCCount")[0].innerText = stringNullConvert(result.KFCCount);

            $("#divKFCY1Count")[0].innerText = stringNullConvert(result.KFCY1Count);
            $("#divKFCAvgSales")[0].innerText = stringNullConvertCurrency(result.KFCAvgSales);
            $("#divNearestKFCY1Sales2")[0].innerText = stringNullConvertCurrency(result.NearestKFCY1Sales2);
        }
    });
}
