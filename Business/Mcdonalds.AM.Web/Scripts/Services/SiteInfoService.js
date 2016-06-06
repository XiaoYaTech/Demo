angular.module("mcd.am.services.siteinfo", ["ngResource"])
.factory("siteInfoService", ["$resource", function ($resource) {
    return $resource(Utils.ServiceURI.Address() + 'api/:contorller/:action', {}, {
        getSiteInfo: {
            method: "GET",
            params: { contorller: "SiteInfo", action: 'GetSiteInfo' },
            isArray: false
        },
        getDropdownDatas: {
            method: "GET",
            params: { contorller: "SiteInfo", action: 'GetDropdownDatas' },
            isArray: false
        },
        saveSiteInfo: {
            method: "POST",
            params: { contorller: "SiteInfo", action: 'Save' },
            isArray: false
        },
        submitSiteInfo: {
            method: "POST",
            params: { contorller: "SiteInfo", action: 'Submit' },
            isArray: false
        }
    });
}]);