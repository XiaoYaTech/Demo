﻿angular.module("mcd.am.services.tempClosure", ["ngResource"]).factory("tempClosureService", ["$resource", function ($resource) {
    return $resource(Utils.ServiceURI.Address() + "api/:module/:action", {}, {
        create: { method: "POST", params: { module: "tempClosure", action: "create" }, isArray: true },
        getLegalReviewInfo: { method: "GET", params: { module: "tempClosureLegalReview", action: "get" }, isArray: false },
        saveLegalReview: { method: "POST", params: { module: "tempClosureLegalReview", action: "save" }, isArray: false },
        submitLegalReview: { method: "POST", params: { module: "tempClosureLegalReview", action: "submit" }, isArray: false },
        approveLegalReview: { method: "POST", params: { module: "tempClosureLegalReview", action: "approve" }, isArray: false },
        returnLegalReview: { method: "POST", params: { module: "tempClosureLegalReview", action: "return" }, isArray: false },
        resubmitLegalReview: { method: "POST", params: { module: "tempClosureLegalReview", action: "resubmit" }, isArray: false },
        recallLegalReview: { method: "POST", params: { module: "tempClosureLegalReview", action: "recall" }, isArray: false },
        editLegalReview: { method: "POST", params: { module: "tempClosureLegalReview", action: "edit" }, isArray: false },
        getClosurePackageInfo: { method: "GET", params: { module: "tempClosurePackage", action: "get" }, isArray: false },
        getClosurePackageApprovers: { method: "GET", params: { module: "tempClosurePackage", action: "getApprovers" }, isArray: false },
        saveClosurePackage: { method: "POST", params: { module: "tempClosurePackage", action: "save" }, isArray: false },
        submitClosurePackage: { method: "POST", params: { module: "tempClosurePackage", action: "submit" }, isArray: false },
        approveClosurePackage: { method: "POST", params: { module: "tempClosurePackage", action: "approve" }, isArray: false },
        returnClosurePackage: { method: "POST", params: { module: "tempClosurePackage", action: "return" }, isArray: false },
        rejectClosurePackage: { method: "POST", params: { module: "tempClosurePackage", action: "reject" }, isArray: false },
        resubmitClosurePackage: { method: "POST", params: { module: "tempClosurePackage", action: "resubmit" }, isArray: false },
        recallClosurePackage: { method: "POST", params: { module: "tempClosurePackage", action: "recall" }, isArray: false },
        editClosurePackage: { method: "POST", params: { module: "tempClosurePackage", action: "edit" }, isArray: false },
        confirmClosurePackage: { method: "POST", params: { module: "tempClosurePackage", action: "confirm" }, isArray: false },
        getClosureMemo: { method: "GET", params: { module: "tempClosureMemo", action: "get" }, isArray: false },
        saveClosureMemo: { method: "POST", params: { module: "tempClosureMemo", action: "save" }, isArray: false },
        sendClosureMemo: { method: "POST", params: { module: "tempClosureMemo", action: "send" }, isArray: false },
        querySaveable: { method: "GET", params: { module: "tempClosureMemo", action: "querySaveable" }, isArray: false },
        getReopenMemo: { method: "GET", params: { module: "tempClosureReopenMemo", action: "get" }, isArray: false },
        saveReopenMemo: { method: "POST", params: { module: "tempClosureReopenMemo", action: "save" }, isArray: false },
        sendReopenMemo: { method: "POST", params: { module: "tempClosureReopenMemo", action: "send" }, isArray: false },
        getGBMemo: { method: "GET", params: { module: "tempClosureGBMemo", action: "get" }, isArray: false },
        saveGBMemo: { method: "POST", params: { module: "tempClosureGBMemo", action: "save" }, isArray: false },
        sendGBMemo: { method: "POST", params: { module: "tempClosureGBMemo", action: "send" }, isArray: false },
        get: { method: "GET", params: { module: "tempClosure", action: "get" }, isArray: false },
        isTempClosed: { method: "GET", params: { module: "tempClosure", action: "isTempClosed" }, isArray: false }
    });
}]);