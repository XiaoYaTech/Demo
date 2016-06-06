(() => {
    var renewalApp = angular.module("amApp");
    renewalApp.directive("analysisForm", [
        (): ng.IDirective=> {
            return {
                restrict: "E",
                replace: true,
                scope: {
                    editable: "=?",
                    entity: "=?",
                    storeInfo: "=?"
                },
                templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/AnalysisForm",
                link: ($scope: any, element, attrs) => {
                    $scope.boolVals = ["Y", "N"];
                    $scope.beTypes = ["Attach Kiosk", "Remote kiosk", "MDS", "McCafe", "24H"];
                    
                    $scope.$watch("entity.DR1stTYAmount+entity.FairMarketRentAmount", function () {
                        if (!!$scope.entity) {
                            if (!!$scope.entity.FairMarketRentAmount) {
                                $scope.entity.RentDeviation = Number(Utils.caculator.subtract(Utils.caculator.division($scope.entity.DR1stTYAmount, $scope.entity.FairMarketRentAmount), 1).toFixed(3));
                            } else {
                                $scope.entity.RentDeviation = null;
                            }
                        }
                    });
                }
            };
        }
    ]);
    renewalApp.directive("renewaltoolFinMeasureInput", [
        "renewalService",
        (renewalService): ng.IDirective=> {
            return {
                restrict: "E",
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/RenewalToolFinMeasureInput",
                scope: {
                    info: "=?",
                    editable: "=?",
                    salesEditable: "=?",
                    showSalesEditpart: "@",
                    projectId: "@",
                    finMeasureInput: "=?",
                    yearMonths: "=?"
                },
                link: ($scope: any, element, attrs) => {
                    var loadTTMFinanceData = function (yearMonthChanged) {
                        renewalService.getTTMFinanceData({
                            projectId: $scope.projectId,
                            yearMonth: $scope.finMeasureInput.FinanceDataYearMonth
                        }).$promise.then(function (data) {
                                $scope.finMeasureInput.AccountingOriginal = data.Accounting;
                                $scope.finMeasureInput.DepreciationEssdOriginal = data.DepreciationEssd;
                                $scope.finMeasureInput.DepreciationLhiOriginal = data.DepreciationLhi;
                                $scope.finMeasureInput.NonProductCostsOriginal = data.NonProductCosts;
                                $scope.finMeasureInput.NonProductSalesOriginal = data.NonProductSales;
                                $scope.finMeasureInput.InsuranceOriginal = data.Insurance;
                                $scope.finMeasureInput.InterestEssdOriginal = data.InterestEssd;
                                $scope.finMeasureInput.InterestLhiOriginal = data.InterestLhi;
                                $scope.finMeasureInput.OtherIncExpOriginal = data.OtherIncExp;
                                $scope.finMeasureInput.PacOriginal = data.Pac;
                                $scope.finMeasureInput.ProductSalesOriginal = data.ProductSales;
                                $scope.finMeasureInput.RentOriginal = data.Rent;
                                $scope.finMeasureInput.ServiceFeeOriginal = data.ServiceFee;
                                $scope.finMeasureInput.TaxesAndLicensesOriginal = data.TaxesAndLicenses;
                                $scope.finMeasureInput.CompSalesOriginal = data.CompSales;
                                if (yearMonthChanged) {
                                    $scope.finMeasureInput.AccountingAdjustment = data.Accounting;
                                    $scope.finMeasureInput.DepreciationEssdAdjustment = data.DepreciationEssd;
                                    $scope.finMeasureInput.DepreciationLhiAdjustment = data.DepreciationLhi;
                                    $scope.finMeasureInput.NonProductCostsAdjustment = data.NonProductCosts;
                                    $scope.finMeasureInput.NonProductSalesAdjustment = data.NonProductSales;
                                    $scope.finMeasureInput.InsuranceAdjustment = data.Insurance;
                                    $scope.finMeasureInput.InterestEssdAdjustment = data.InterestEssd;
                                    $scope.finMeasureInput.InterestLhiAdjustment = data.InterestLhi;
                                    $scope.finMeasureInput.OtherIncExpAdjustment = data.OtherIncExp;
                                    $scope.finMeasureInput.PacAdjustment = data.Pac;
                                    $scope.finMeasureInput.ProductSalesAdjustment = data.ProductSales;
                                    $scope.finMeasureInput.RentAdjustment = data.Rent;
                                    $scope.finMeasureInput.ServiceFeeAdjustment = data.ServiceFee;
                                    $scope.finMeasureInput.TaxesAndLicensesAdjustment = data.TaxesAndLicenses;
                                    $scope.finMeasureInput.CompSalesAdjustment = data.CompSales;
                                } else {
                                    $scope.finMeasureInput.AccountingAdjustment = $scope.finMeasureInput.AccountingAdjustment || data.Accounting;
                                    $scope.finMeasureInput.DepreciationEssdAdjustment = $scope.finMeasureInput.DepreciationEssdAdjustment || data.DepreciationEssd;
                                    $scope.finMeasureInput.DepreciationLhiAdjustment = $scope.finMeasureInput.DepreciationLhiAdjustment || data.DepreciationLhi;
                                    $scope.finMeasureInput.NonProductCostsAdjustment = $scope.finMeasureInput.NonProductCostsAdjustment || data.NonProductCosts;
                                    $scope.finMeasureInput.NonProductSalesAdjustment = $scope.finMeasureInput.NonProductSalesAdjustment || data.NonProductSales;
                                    $scope.finMeasureInput.InsuranceAdjustment = $scope.finMeasureInput.InsuranceAdjustment || data.Insurance;
                                    $scope.finMeasureInput.InterestEssdAdjustment = $scope.finMeasureInput.InterestEssdAdjustment || data.InterestEssd;
                                    $scope.finMeasureInput.InterestLhiAdjustment = $scope.finMeasureInput.InterestLhiAdjustment || data.InterestLhi;
                                    $scope.finMeasureInput.OtherIncExpAdjustment = $scope.finMeasureInput.OtherIncExpAdjustment || data.OtherIncExp;
                                    $scope.finMeasureInput.PacAdjustment = $scope.finMeasureInput.PacAdjustment || data.Pac;
                                    $scope.finMeasureInput.ProductSalesAdjustment = $scope.finMeasureInput.ProductSalesAdjustment || data.ProductSales;
                                    $scope.finMeasureInput.RentAdjustment = $scope.finMeasureInput.RentAdjustment || data.Rent;
                                    $scope.finMeasureInput.ServiceFeeAdjustment = $scope.finMeasureInput.ServiceFeeAdjustment || data.ServiceFee;
                                    $scope.finMeasureInput.TaxesAndLicensesAdjustment = $scope.finMeasureInput.TaxesAndLicensesAdjustment || data.TaxesAndLicenses;
                                    $scope.finMeasureInput.CompSalesAdjustment = $scope.finMeasureInput.CompSalesAdjustment || data.CompSales;
                                }
                            });
                    };
                    $scope.showSales = function (years) {
                        if ($scope.info) {
                            return $scope.info && Math.floor($scope.info.RenewalYears) >= years && $scope.showSalesEditpart;
                        } else
                            return false;
                    };
                    $scope.$on("loadTTMFinanceData", function (e, finMeasureInput) {
                        $scope.finMeasureInput = finMeasureInput;
                        loadTTMFinanceData(false);
                    });
                    $scope.onYearMonthChange = function () {
                        var ym = $scope.finMeasureInput.FinanceDataYearMonth.split('-');
                        $scope.finMeasureInput.FinanceYear = ym[0];
                        $scope.finMeasureInput.FinanceMonth = ym[1];
                        loadTTMFinanceData(true);
                    }
                    if ($scope.finMeasureInput)
                        loadTTMFinanceData(false);
                }
            };
        }
    ])
    .directive("renewaltoolWriteoffAndReincost", [
        "renewalService",
        (renewalService): ng.IDirective=> {
            return {
                restrict: "E",
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/RenewalToolWriteOffAndReinCost",
                scope: {
                    editable: "=?",
                    hasReinvestment:"=?",
                    writeOffAndReinCost:"="
                },
                link: ($scope: any, element, attrs) => {
                    $scope.sum = function () {
                        var result = 0;
                        for (var i = 0; i < arguments.length; i++) {
                            var num = Number(arguments[i]);
                            if (isNaN(num)) {
                                num = 0;
                            }
                            result = Utils.caculator.plus(result, num);
                        }
                        return result;
                    }
                }
            };
        }
    ])
    .directive("renewaltoolFinMeasureOutput", [
        "renewalService",
        (renewalService): ng.IDirective=> {
            return {
                restrict: "E",
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/RenewalToolFinMeasureOutput",
                scope: {
                    editable: "=?"
                },
                link: ($scope: any, element, attrs) => {
                    $scope.$on("loadFinMeasureOutput", function ($event,toolId) {
                        renewalService.getToolFinMeasureOutput({
                            toolId: toolId
                        }).$promise.then(function (data) {
                            $scope.finMeasureOutput = data;
                        });
                    });
                }
            };
        }
    ])
    .directive("specialApplication", [
        "renewalService",
        (renewalService): ng.IDirective=> {
            return {
                restrict: "E",
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/SpecialApplication",
                scope: {
                    entity:"=?",
                    editable: "=?"
                },
                link: ($scope: any, element, attrs) => {
                }
            };
        }
    ])
    .directive("transactionInvolve", [
        "renewalService",
        (renewalService): ng.IDirective=> {
            return {
                restrict: "E",
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/TransactionInvolve",
                scope: {
                    entity: "=?",
                    editable: "=?"
                },
                link: ($scope: any, element, attrs) => {
                    $scope.noBLClauseChanged = function () {
                        if (!$scope.entity.IsNoBLClause) {
                            $scope.entity.IsOFAC = false;
                            $scope.entity.IsAntiC = false;
                            $scope.entity.IsBenefitConflict = false;
                        }
                    }
                }
            };
        }
    ])
    .directive("anyLegalConcern", [
        "renewalService",
        (renewalService): ng.IDirective=> {
            return {
                restrict: "E",
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/AnyLegalConcern",
                scope: {
                    entity: "=?",
                    editable: "=?"
                },
                link: ($scope: any, element, attrs) => {
                    $scope.$watch("entity.AnyLegalConcern", function (val) {
                        if (val === false) {
                            $scope.entity.IllegalStructure = false;
                            $scope.entity.Occupying = false;
                            $scope.entity.NoAuthorityToRelease = false;
                            $scope.entity.EntrustLease = false;
                            $scope.entity.SubLease = false;
                            $scope.entity.BeingSealedUp = false;
                            $scope.entity.LicenseCantBeObtained = false;
                            $scope.entity.PendingOrDispute = false;
                            $scope.entity.OtherIssure = false;
                        }
                    });
                }
            };
        }
    ])
    .directive("legalDepartmentReview", [
        "renewalService",
        (renewalService): ng.IDirective=> {
            return {
                restrict: "E",
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/LegalDepartmentReview",
                scope: {
                    entity: "=?",
                    editable: "=?"
                },
                link: ($scope: any, element, attrs) => {
                    $scope.$watch("entity.ReviewStatus", function (val) {
                        if ($scope.entity && val !== "LegalComments") {
                            $scope.entity.SubmitBeforeSign = false;
                            $scope.entity.LandlordFormLeaseUserd = false;
                            $scope.entity.OwnerRefuseToHonorLease = false;
                            $scope.entity.MortgageeRefuseToGuarantee = false;
                            $scope.entity.OtherLegalComment = false;
                            if ($scope.entity.NotEndorsedIssureNo != null && $scope.entity.NotEndorsedIssureNo != "") {
                                $scope.entity.NotEndorsedIssureNo = $scope.entity.NotEndorsedIssureNo.toString();
                            }
                        }
                    });
                    $scope.$watch("entity.SubmitBeforeSign", function (val) {
                        if ($scope.entity && !val) {
                            $scope.entity.SubmitBeforeSignDesc = '';
                        }
                    });
                    $scope.$watch("entity.OtherLegalComment", function (val) {
                        if ($scope.entity && !val) {
                            $scope.entity.OtherLegalCommentDesc = '';
                        }
                    });
                }
            };
        }
    ])
    .directive("soxAudit", [
        "renewalService",
        (renewalService): ng.IDirective=> {
            return {
                restrict: "E",
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/SoxAudit",
                scope: {
                    entity: "=?",
                    editable: "=?"
                },
                link: ($scope: any, element, attrs) => {
                }
            };
        }
    ])
    .directive("endorsementByGeneralCounsel", [
        "renewalService",
        (renewalService): ng.IDirective=> {
            return {
                restrict: "E",
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/EndorsementByGeneralCounsel",
                scope: {
                    entity: "=?",
                    editable: "=?"
                },
                link: ($scope: any, element, attrs) => {
                }
            };
        }
    ])
    .directive("keyMeasures", [
        "renewalService",
        (renewalService): ng.IDirective=> {
            return {
                restrict: "E",
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "RenewalModule/KeyMeasures",
                scope: {
                    analysis: "=?",
                    fmoutput: "=?"
                },
                link: ($scope: any, element, attrs) => {
                }
            };
        }
    ])
})();