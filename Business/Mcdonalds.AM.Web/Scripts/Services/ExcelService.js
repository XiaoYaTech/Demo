angular.module("mcd.am.services.excel", ["ngResource"]).factory("excelService", ["$resource", function ($resource) {
    return $resource(Utils.ServiceURI.Address() + "api/ExcelTemplate/:action", {}, {
        generateExcel: { Method: "GET", params: { action: "GenerateExcel" }, isArray: false },
        generateReimageSummary: { Method: "GET", params: { action: "GenerateReimageSummary" }, isArray: false }
    });
}]);