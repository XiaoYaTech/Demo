var app = angular.module("amApp", [
    'ngRoute',
    'ngChosen',
    'ui.bootstrap',
    'dictionaryFilters',
    'mcd.am.modules',
    'mcd.am.filters',
    'closureCreateServices'
]);
app.controller("storeContractController", [
    "$scope",
    "$parse",
    "$http",
    "$window",
    "messager",
    '$filter',
    function ($scope, $parse, $http, $window, messager, $filter) {
        $scope.usCode = getUrlPar("uscode");
        $scope.RentTypes = ['Fixed Rent', 'Fixed Rent Plus Rent Percent', 'Rent Percent',
            'Rent Percent with Sales Hurdles', 'The Higher of Base Rent or Rent Percent', 'Others'];
        var loadData = function () {
            $http.get(Utils.ServiceURI.Address() + "api/contract/get/store?usCode=" + $scope.usCode).success(function (response) {
                $scope.contracts = response.Histories;
                $scope.currentContract = response.Current;
                $scope.contract = response.Current;
                $scope.contract.contractEditable = true;
                angular.forEach(response.Attachments.contract, function (att, i) {
                    //att.downloadLink = Utils.ServiceURI.Address() + "api/contract/downloadAttachment?id=" + att.Id;
                    att.downloadLink = Utils.ServiceURI.AttachmentAddress() + att.DocName;
                });
                angular.forEach(response.Attachments.project, function (att, i) {
                    //att.downloadLink = Utils.ServiceURI.Address() + "api/attachment/download?id=" + att.ID;
                    att.downloadLink = Utils.ServiceURI.AttachmentAddress() + att.DocName;
                });
                loadRevisions($scope.contract.Id);
                $scope.attachments = response.Attachments;
            });
        }
        loadData();
        if ($.inArray("am_sm_asset_sb_edit", window.currentUser.RightCodes || []) >= 0) {
            $scope.editable = true;
        } else {
            $scope.editable = false;
        }
        var loadRevisions = function (contractId) {
            $http.get(Utils.ServiceURI.Address() + "api/contract/storerevisions?contractId=" + contractId).then(function (response) {
                $scope.revisions = response.data || [];
            });
        };
        var parseYear = function (dateStr, prop) {
            if (!!dateStr) {
                var year = moment(dateStr).format("YYYY");
                if (year !== "Invalid date") {
                    $scope.contract[prop] = year;
                }
            }
        };
        $scope.showChangeDate = {};
        $scope.openDate = function ($event, dateTag) {
            $event.preventDefault();
            $event.stopPropagation();
            $scope[dateTag] = true;
        };
        $scope.$watch("contract.StartDate", function (val) {
            parseYear(val, "StartYear");
        });
        $scope.$watch("contract.EndDate", function (val) {
            parseYear(val, "EndYear");
        });
        $scope.$watch("contract.HasBankGuarantee", function (val) {
            if (val == "0") {
                $scope.contract.BGNumber = null;
                $scope.contract.BGCommencementDate = null;
                $scope.contract.BGEndDate = null;
                $scope.contract.BGAmount = null;
            }
        });

        $scope.$watch("contract.HasDeposit", function (val) {
            if (val == "0") {
                $scope.contract.DepositAmount = null;
                $scope.contract.Refundable = null;
            }
        });

        $scope.$watch("contract.Refundable", function (val) {
            if (!val || val == "0") {
                if (!!$scope.contract)
                    $scope.contract.RefundableDate = null;
            }
        });

        $scope.$watch("contract.WithEarlyTerminationClause", function (val) {
            if (!val || val == "0") {
                $scope.contract.EarlyTerminationClauseDetail = "";
            }
        });

        $scope.editContract = function (contract) {
            $scope.contract = contract;
            if (contract.Id === $scope.currentContract.Id) {
                $scope.contract.contractEditable = true;
                $scope.contract = $scope.currentContract;
            } else {
                $scope.contract.contractEditable = false;
            }
            loadRevisions($scope.contract.Id);
        };
        $scope.addRevision = function () {
            if (!$scope.revisions) {
                $scope.revisions = [];
            }
            $scope.revisions.push({
                ChangeDate: new Date(),
                StoreCode: $scope.currentContract.StoreCode,
                StoreContractInfoId: $scope.currentContract.Id,
                StoreID: $scope.currentContract.StoreID,
                LeaseRecapID: $scope.currentContract.LeaseRecapID,
                RentStructureOld: $scope.currentContract.RentStructure,
                RedlineAreaOld: $scope.currentContract.TotalLeasedArea,
                LeaseChangeExpiryOld: $scope.currentContract.EndDate,
                LandlordOld: $scope.currentContract.PartyAFullName
            });
        };
        $scope.removeRevision = function (revision) {
            $scope.revisions = $.grep($scope.revisions, function (re, i) {
                return re !== revision;
            });
        };

        $scope.save = function (frm) {
            if (frm.$valid) {
                $http.post(Utils.ServiceURI.Address() + "api/contract/storesave",
                   {
                       Contract: $scope.contract,
                       Revisions: $scope.revisions
                   }).success(function (data) {
                       messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                       loadData();
                   }).error(function (data) {
                       messager.showMessage("[[[保存失败]]]", "fa-warning c_red");
                   });
            }
        };
    }
]);

app.controller("storeSiteInfoController", [
    "$scope",
    "$parse",
    "$http",
    "$window",
    "messager",
    '$filter',
    function ($scope, $parse, $http, $window, messager, $filter) {
        if ($.inArray("am_sm_asset_sb_edit", window.currentUser.RightCodes || []) >= 0) {
            $scope.editable = true;
        } else {
            $scope.editable = false;
        }
        $scope.usCode = getUrlPar("uscode");
        $scope.dataLoaded = false;
        $scope.DirectionalEffects = [];
        $scope.DTTypeNames = [];
        $scope.Floors = [];
        $scope.KitchenFloors = [];
        $scope.FrontCounterFloors = [];
        $scope.ExteriorDesigns = [];
        $scope.AppearDesign = [];
        var loadDropdown = function () {
            $http.get(Utils.ServiceURI.Address() + "api/SiteInfo/GetDropdownDatas").success(function (data) {
                if (data != null) {
                    $scope.DirectionalEffects = data.DirectionalEffects;
                    $scope.DTTypeNames = data.DTTypeNames;
                    $scope.Floors = data.Floors;
                    $scope.FloorsCount = ['1', '2', '3', '4', '5'];
                    $scope.KitchenFloors = data.KitchenFloors;
                    $scope.FrontCounterFloors = data.FrontCounterFloors;
                    $scope.ExteriorDesigns = data.ExteriorDesigns;
                    $scope.InnerDesign = data.InnerDesign;
                    $scope.AppearDesign = data.AppearDesign;
                }
                $scope.dataLoaded = true;
            });
        };
        $scope.$watch("usCode", function (val) {
            if (val != null && val != "") {
                $scope.dataLoaded = false;
                $http.get(Utils.ServiceURI.Address() + "api/SiteInfo/GetStoreSiteInfo?usCode=" + val).success(function (data) {
                    loadDropdown();
                    $scope.source = data;
                    $scope.source.PlayPlace = data.PlayPlace;
                    $scope.source.PartyRoom = data.PartyRoom;
                });
                $http.get(Utils.ServiceURI.Address() + "api/store/basic?usCode=" + val).success(function (data) {
                    if (data != null) {
                        $scope.store = data;
                    }
                });
            }
        });
        $scope.$watch('source.McdCarParkCount+source.PublicCarParkCount+source.RoadCarParkCount', function (newVal, oldVal) {
            if ($scope.source && newVal != oldVal) {
                var mcdCarParkCount = $.isNumeric($scope.source.McdCarParkCount) ? parseFloat($scope.source.McdCarParkCount) : 0;
                var publicCarParkCount = $.isNumeric($scope.source.PublicCarParkCount) ? parseFloat($scope.source.PublicCarParkCount) : 0;
                var roadCarParkCount = $.isNumeric($scope.source.RoadCarParkCount) ? parseFloat($scope.source.RoadCarParkCount) : 0;
                $scope.source.CarParkTotal = mcdCarParkCount + publicCarParkCount + roadCarParkCount;
            }
        });

        $scope.getSeatingRatio = function () {
            if ($scope.source && $scope.source.TotalArea && $scope.source.TotalSeatsNo) {
                $scope.source.SeatingRatio = $filter("numberCustom")($scope.source.TotalArea == 0 ? 0 : ($scope.source.TotalSeatsNo / $scope.source.TotalArea), 2);
            }
        };
        $scope.$watch('source.Size1+source.Size2+source.Size3+source.Size4+source.Size5', function (newVal, oldVal) {
            if ($scope.source && newVal != oldVal) {
                var Size1 = isNaN(parseFloat($scope.source.Size1)) ? 0 : parseFloat($scope.source.Size1);
                var Size2 = isNaN(parseFloat($scope.source.Size2)) ? 0 : parseFloat($scope.source.Size2);
                var Size3 = isNaN(parseFloat($scope.source.Size3)) ? 0 : parseFloat($scope.source.Size3);
                var Size4 = isNaN(parseFloat($scope.source.Size4)) ? 0 : parseFloat($scope.source.Size4);
                var Size5 = isNaN(parseFloat($scope.source.Size5)) ? 0 : parseFloat($scope.source.Size5);
                $scope.source.TotalArea = Size1 + Size2 + Size3 + Size4 + Size5;
                $scope.getSeatingRatio();
            }
        });
        $scope.$watch('source.Seats1+source.Seats2+source.Seats3+source.Seats4+source.Seats5', function (newVal, oldVal) {
            if ($scope.source && newVal != oldVal) {
                var Seats1 = isNaN(parseFloat($scope.source.Seats1)) ? 0 : parseFloat($scope.source.Seats1);
                var Seats2 = isNaN(parseFloat($scope.source.Seats2)) ? 0 : parseFloat($scope.source.Seats2);
                var Seats3 = isNaN(parseFloat($scope.source.Seats3)) ? 0 : parseFloat($scope.source.Seats3);
                var Seats4 = isNaN(parseFloat($scope.source.Seats4)) ? 0 : parseFloat($scope.source.Seats4);
                var Seats5 = isNaN(parseFloat($scope.source.Seats5)) ? 0 : parseFloat($scope.source.Seats5);
                $scope.source.TotalSeatsNo = Seats1 + Seats2 + Seats3 + Seats4 + Seats5;
                $scope.getSeatingRatio();
            }
        });
        $scope.$watch('source.WaitingArea+source.SeatArea+source.PlayPlaceArea+source.ToiletArea+source.StairArea+source.OtherArea', function (newVal, oldVal) {
            if ($scope.source && newVal != oldVal) {
                var WaitingArea = isNaN(parseFloat($scope.source.WaitingArea)) ? 0 : parseFloat($scope.source.WaitingArea);
                var SeatArea = isNaN(parseFloat($scope.source.SeatArea)) ? 0 : parseFloat($scope.source.SeatArea);
                var PlayPlaceArea = isNaN(parseFloat($scope.source.PlayPlaceArea)) ? 0 : parseFloat($scope.source.PlayPlaceArea);
                var ToiletArea = isNaN(parseFloat($scope.source.ToiletArea)) ? 0 : parseFloat($scope.source.ToiletArea);
                var StairArea = isNaN(parseFloat($scope.source.StairArea)) ? 0 : parseFloat($scope.source.StairArea);
                var OtherArea = isNaN(parseFloat($scope.source.OtherArea)) ? 0 : parseFloat($scope.source.OtherArea);
                $scope.source.DiningArea = WaitingArea + SeatArea + PlayPlaceArea + ToiletArea + StairArea + OtherArea;
            }
        });
        $scope.$watch('source.ServiceArea+source.ProducingArea+source.BackArea+source.ColdStorageArea+source.DryArea+source.StaffroomArea', function (newVal, oldVal) {
            if ($scope.source && newVal != oldVal) {
                var ServiceArea = isNaN(parseFloat($scope.source.ServiceArea)) ? 0 : parseFloat($scope.source.ServiceArea);
                var ProducingArea = isNaN(parseFloat($scope.source.ProducingArea)) ? 0 : parseFloat($scope.source.ProducingArea);
                var BackArea = isNaN(parseFloat($scope.source.BackArea)) ? 0 : parseFloat($scope.source.BackArea);
                var ColdStorageArea = isNaN(parseFloat($scope.source.ColdStorageArea)) ? 0 : parseFloat($scope.source.ColdStorageArea);
                var DryArea = isNaN(parseFloat($scope.source.DryArea)) ? 0 : parseFloat($scope.source.DryArea);
                var StaffroomArea = isNaN(parseFloat($scope.source.StaffroomArea)) ? 0 : parseFloat($scope.source.StaffroomArea);
                $scope.source.KitchenArea = ServiceArea + ProducingArea + BackArea + ColdStorageArea + DryArea + StaffroomArea;
            }
        });
        $scope.editablePoleSign = $scope.editable;
        $scope.$watch("source.PoleSign", function (val) {
            if ($scope.source && val != null) {
                if ($scope.editable) {
                    if (val == "1") {
                        $scope.editablePoleSign = true;
                    } else {
                        $scope.editablePoleSign = false;
                    }
                }
            }
        });
        $scope.editableSignage = $scope.editable;
        $scope.$watch("source.Signage", function (val) {
            if ($scope.source && val != null) {
                if ($scope.editable) {
                    if (val == "1") {
                        $scope.editableSignage = true;
                    } else {
                        $scope.editableSignage = false;
                    }
                }
            }
        });

        $scope.save = function (frm) {
            if (frm.$valid) {
                $http.post(Utils.ServiceURI.Address() + "api/SiteInfo/StoreSave", $scope.source).success(function (data) {
                    messager.showMessage("[[[保存成功]]]", "fa-check c_green");
                }).error(function (data) {
                    messager.showMessage("[[[保存失败]]]", "fa-warning c_red");
                });
            }
        };
    }
]);
app.controller("summaryController", ["$scope", "$window", "$http", "messager", "$modal", function ($scope, $window, $http, messager, $modal) {

    $scope.entity = {};
    $scope.usCode = getUrlPar("uscode");
    $scope.pageIndex = 1;
    $scope.pageSize = 5;
    $scope.currentFlow = "";
    $scope.loadingList = false;
    $scope.enableEdit = false;

    $http.get(Utils.ServiceURI.Address() + "api/DL/Authority", {
        cache: false,
        params: {
            usCode: $scope.usCode
        }
    }).success(function (data) {
        $scope.enableEdit = data.EnableEdit;
    });

    $scope.openDate = function ($event, tag) {
        $event.preventDefault();
        $event.stopPropagation();
        $scope[tag] = true;
    };

    var LoadSummaryList = function () {
        $scope.loadingList = true;
        $http.get(Utils.ServiceURI.Address() + "api/DL/List", {
            cache: false,
            params: {
                usCode: $scope.usCode,
                index: $scope.pageIndex,
                size: $scope.pageSize
            }
        }).success(function (data) {
            $scope.summaryList = data.List;
            $scope.totalItems = data.TotalItems;
            $scope.loadingList = false;
        });
    };

    var showList = function (show) {
        if (show) {
            $("#divSummaryDetails").hide();
            $("#divSummaryList").show();
        }
        else {
            $("#divSummaryDetails").show();
            $("#divSummaryList").hide();
        }
    }

    LoadSummaryList();

    $scope.LoadDetailData = function (flowType, Id) {
        showList(false);
        $scope.currentFlow = flowType;
        if (Id) {
            var api = Utils.ServiceURI.Address() + "api/DL/Detail";
            $http.get(api, {
                cache: false,
                params: {
                    flowCode: flowType,
                    Id: Id
                }
            }).success(function (data) {
                $scope.entity = data;
                $scope.editable = data.Editable && $scope.enableEdit;
            });
        }
        else {
            $scope.entity = {};
            $scope.entity.Id = Utils.Generator.newGuid();
            $scope.editable = true;
        }

        var url = Utils.ServiceURI.WebAddress() + "StoreList/SummaryBy" + flowType;
        $scope.flowUrl = url;
    };

    $scope.$watch("pageIndex", function (val) {
        if (!!val) {
            LoadSummaryList();
        }
    });

    $scope.add = function () {
        $modal.open({
            templateUrl: "/StoreList/AddSummary",
            backdrop: 'static',
            size: 'sm',
            controller: ["$scope", "$modalInstance", function (winScope, $modalInstance) {
                winScope.ok = function (flowType) {
                    $modalInstance.close(flowType);
                };
                winScope.cancel = function () {
                    $modalInstance.dismiss("cancel");
                };
            }]
        }).result.then(function (flowType) {
            $scope.LoadDetailData(flowType);
        }, function () {
        });
    }

    $scope.save = function (frm,push) {
        if (frm.$invalid)
            return;
        if ($scope.entity.IsPushed == true)
            push = true;
        var api = Utils.ServiceURI.Address() + "api/DL/Submit/" + $scope.currentFlow;
        $scope.entity.USCode = $scope.usCode;
        messager.blockUI("[[[正在处理中，请稍等...]]]");
        $http.post(api, {
            Entity: $scope.entity,
            PushOrNot: push
        }).success(function (data) {
            messager.unBlockUI();
            messager.showMessage("[[[操作成功]]]", "fa-check c_green");
            $scope.pageIndex = 1;
            LoadSummaryList();
            showList(true);
        }).error(function (data) {
            messager.unBlockUI();
            messager.showMessage("[[[操作失败！]]]", "fa-warning c_orange");
        });
    };
}]);
function getUrlPar(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.href.substr(window.location.href.indexOf('?', 0)).substr(1).match(reg);
    if (r != null) return unescape(r[2]); return '';
}