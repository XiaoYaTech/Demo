var coreService = angular.module("mcd.am.services", []);

coreService.factory("Team", [
    '$http', $http => {
        return {
            query: (storeCodes, positionCodes) => {
                return $http.get(Utils.ServiceURI.FrameAddress() + 'api/user/team/', {
                    cache: false,
                    params: {
                        storeCodes: storeCodes,
                        positionCodes: positionCodes
                    }
                });
            }
        };
    }
]);