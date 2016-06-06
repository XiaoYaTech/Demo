angular.module("mcd.am.services.store", ["ngResource"]).factory("storeService", ["$resource", function ($resource) {
    return $resource(Utils.ServiceURI.Address() + "api/store/:action", {}, {
        searchMyStore: { Method: "GET", params: { action: "searchMyStore" }, isArray: true },
        checkStore: { Method: "GET", params: { action: "checkStore" }, isArray: false },
        checkStoreFlow: { Method: "GET", params: { action: "checkStoreFlow" }, isArray: false },
        getStoreDetail: { Method: "GET", params: { action: "Details" }, isArray: false },
        getStoreDetailInfo: { Method: "GET", params: { action: "DetailInfo" }, isArray: false },
        getStoreBasic: { Method: "GET", params: { action: "basic" }, isArray: false },
        updateStoreLocation: { Method: "POST", params: { action: "updateStoreLocation" }, isArray: false }
    });
}]);