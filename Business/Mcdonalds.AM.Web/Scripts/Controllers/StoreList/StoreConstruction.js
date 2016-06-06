
//  加载详情页-Construction
function loadConstruction(USCode) {

    $.ajax({
        type: "GET",
        url: "/StoreList/StoreSTLocationQuery",
        cache: false,
        dataType: "json",
        data: {
            _USCode: USCode
        },
        success: function (data) {

            var result = eval(data);

            $("#divPoleSign")[0].innerText = stringNullConvertYesNoENUS(result.PoleSign);
            $("#divPoleSignageCount")[0].innerText = stringNullConvert(result.PoleSignageCount);
            $("#divDirectionalEffect")[0].innerText = stringNullConvert(result.DirectionalEffect);

            $("#divSignage")[0].innerText = stringNullConvertYesNoENUS(result.Signage);
            $("#divSignageCount")[0].innerText = stringNullConvert(result.SignageCount);
            $("#divFacadeLenth")[0].innerText = stringNullConvert(result.FacadeLenth);

            $("#divEgressCount")[0].innerText = stringNullConvert(result.EgressCount);
            $("#divIngressCount")[0].innerText = stringNullConvert(result.IngressCount);
            $("#divOpenCount")[0].innerText = stringNullConvert(result.OpenCount);

            $("#divMcdCarParkCount")[0].innerText = stringNullConvert(result.McdCarParkCount);
            $("#divPublicCarParkCount")[0].innerText = stringNullConvert(result.PublicCarParkCount);
            $("#divRoadCarParkCount")[0].innerText = stringNullConvert(result.RoadCarParkCount);

            $("#divCarParkTotal")[0].innerText = stringNullConvert(result.CarParkTotal);
            $("#divConstructionPortfolioTypeName")[0].innerText = stringNullConvert(result.PortfolioTypeName);
            $("#divDTTypeName")[0].innerText = stringNullConvert(result.DTTypeName);

            $("#divFloor")[0].innerText = stringNullConvert(result.Floor);

            $("#divFloor1")[0].innerText = stringNullConvert(result.Floor1);
            $("#divSize1")[0].innerText = stringNullConvert(result.Size1);
            $("#divSeats1")[0].innerText = stringNullConvert(result.Seats1);

            $("#divFloor2")[0].innerText = stringNullConvert(result.Floor2);
            $("#divSize2")[0].innerText = stringNullConvert(result.Size2);
            $("#divSeats2")[0].innerText = stringNullConvert(result.Seats2);

            $("#divFloor3")[0].innerText = stringNullConvert(result.Floor3);
            $("#divSize3")[0].innerText = stringNullConvert(result.Size3);
            $("#divSeats3")[0].innerText = stringNullConvert(result.Seats3);

            $("#divFloor4")[0].innerText = stringNullConvert(result.Floor4);
            $("#divSize4")[0].innerText = stringNullConvert(result.Size4);
            $("#divSeats4")[0].innerText = stringNullConvert(result.Seats4);

            $("#divFloor5")[0].innerText = stringNullConvert(result.Floor5);
            $("#divSize5")[0].innerText = stringNullConvert(result.Size5);
            $("#divSeats5")[0].innerText = stringNullConvert(result.Seats5);

            $("#divSeatingRatio")[0].innerText = stringNullConvert(result.SeatingRatio);
            $("#divTotalArea")[0].innerText = stringNullConvert(result.TotalArea);
            $("#divTotalSeatsNo")[0].innerText = stringNullConvert(result.TotalSeatsNo);

            $("#divSplitKitchen")[0].innerText = stringNullConvertYesNoENUS(result.SplitKitchen);
            $("#divKitchenFloor")[0].innerText = stringNullConvert(result.KitchenFloor);
            $("#divFrontCounterFloor1")[0].innerText = stringNullConvert(result.FrontCounterFloor1);

            $("#divFrontCounterFloor2")[0].innerText = stringNullConvert(result.FrontCounterFloor2);
            $("#divFrontCounterSeats")[0].innerText = stringNullConvert(result.FrontCounterSeats);
            $("#divOutsideSeats")[0].innerText = stringNullConvert(result.OutsideSeats);


            $("#divDTSize")[0].innerText = stringNullConvert(result.DTSize);
            $("#divDiningArea")[0].innerText = stringNullConvert(result.DiningArea);
            $("#divKitchenArea")[0].innerText = stringNullConvert(result.KitchenArea);

            $("#divRemoteKioskArea")[0].innerText = stringNullConvert(result.RemoteKioskArea);
            $("#divWaitingArea")[0].innerText = stringNullConvert(result.WaitingArea);
            $("#divServiceArea")[0].innerText = stringNullConvert(result.ServiceArea);

            $("#divPlayPlace")[0].innerText = stringNullConvert(result.PlayPlace);
            $("#divSeatArea")[0].innerText = stringNullConvert(result.SeatArea);
            $("#divProducingArea")[0].innerText = stringNullConvert(result.ProducingArea);

            $("#divPartyRoom")[0].innerText = stringNullConvert(result.PartyRoom);
            $("#divPlayPlaceArea")[0].innerText = stringNullConvert(result.PlayPlaceArea);
            $("#divBackArea")[0].innerText = stringNullConvert(result.BackArea);

            $("#divToiletArea")[0].innerText = stringNullConvert(result.ToiletArea);
            $("#divColdStorageArea")[0].innerText = stringNullConvert(result.ColdStorageArea);
            $("#divStairArea")[0].innerText = stringNullConvert(result.StairArea);
            $("#divDryArea")[0].innerText = stringNullConvert(result.DryArea);
            $("#divOtherArea")[0].innerText = stringNullConvert(result.OtherArea);
            $("#divStaffroomArea")[0].innerText = stringNullConvert(result.StaffroomArea);


            $("#divDesignStyle")[0].innerText = stringNullConvert(result.DesignStyle);
            $("#divExteriorDesign")[0].innerText = stringNullConvert(result.ExteriorDesign);
        }
    });
}

