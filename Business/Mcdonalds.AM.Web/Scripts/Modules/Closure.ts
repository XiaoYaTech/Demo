/// <reference path="../libs/moment/moment.d.ts" />
/// <reference path="../Libs/Angular/angular.d.ts" />
/// <reference path="../Utils/Utils.ts" />
angular.module("mcd.am.closure.modules", [
    "mcd.am.closure.services.closureMemo",
    "mcd.am.closure.services.closureTool",
    "mcd.am.closure.filters"
]).directive("closureUploadWochecklist", [
        "$http", "$location", "messager", function ($http, $location, messager) {
            return {
                restrict: "EA",
                scope: {
                    editable: "=",
                    projectId: "=",
                    entityId: "=",
                    panelTitle: "@"
                },
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "ClosureModule/UploadWoChecklist",
                link: function ($scope, ele, attrs) {
                    if (!$scope.panelTitle) {
                        $scope.panelTitle = "Upload WO Checklist";
                    }
                    $scope.loadingTemplates = function () {
                        $scope.ajaxFinished = false;
                        var url;
                        if ($scope.entityId != undefined)
                            url = Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetHistoryTemplates/" + $scope.entityId;
                        else
                            url = Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetTemplates/" + $scope.projectId;
                        $http.get(url).success(function (atts) {
                            $scope.ajaxFinished = true;
                            if (atts != "null") {
                                if ($scope.entityId == undefined) {
                                    $http.get(Utils.ServiceURI.Address() + "api/ClosureWOCheckList/GetByProjectId/" + $scope.projectId, {
                                        cache: false,
                                        params: {
                                            userAccount: window["currentUser"].Code
                                        }
                                    }).success(function (entity) {
                                            $scope.$parent.entity = entity;
                                        });
                                }

                                $scope.$parent.dataRefresh = true;
                                $scope.$parent.isUploadTemplate = true;
                                $scope.templateList = atts;
                                for (var i = 0; i < atts.length; i++) {
                                    atts[i].IsAvailable = (i == atts.length - 1) ? "生效" : "失效";
                                }
                            }
                        });
                    };

                    $scope.loadingTemplates();

                    $scope.$watch("projectId", function (val) {
                        if (!!val) {
                            //$scope.templateUrl = Utils.ServiceURI.Address() + "api/ClosureWOCheckList/DownLoadTemplate/" + val;
                            $scope.templateUrl = Utils.ServiceURI.Address() + "api/ExcelTemplate/DownLoadTemplate/ClosureWOCheckList/" + val + "/Closure_WOCheckList";
                        } else {
                            $scope.templateUrl = "#";
                        }
                    });
                    $scope.uploadFinished = function (u, a) {
                        messager.showMessage("[[[上传成功]]]", "fa-check c_green").then(function () {
                            $scope.$parent.isUploadTemplate = true;
                            if ($scope.$parent.reloadAtt)
                                $scope.$parent.reloadAtt = $scope.$parent.reloadAtt + 1;
                            else
                                $scope.$parent.reloadAtt = 1;

                            $scope.loadingTemplates();
                        });
                    };
                    //$scope.$watch("entityId", (val) => {
                    //    if (!!val) {
                    //        $scope.loadingTemplates();
                    //    }
                    //});
                }
            };
        }
    ]).directive("closureUploadConsInvt", [
        "$http", "messager", function ($http, messager) {
            return {
                restrict: "EA",
                scope: {
                    editable: "=",
                    projectId: "=",
                    entityId: "=",
                    panelTitle: "@"
                },
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "ClosureModule/UploadConsInvtChecking",
                link: function ($scope, ele, attrs) {
                    if (!$scope.panelTitle) {
                        $scope.panelTitle = "Upload ConsInvtChecking";
                    }
                    $scope.loadingTemplates = function () {
                        $scope.ajaxFinished = false;
                        var url = Utils.ServiceURI.Address() + "api/ClosureConsInvtChecking/GetTemplates/" + $scope.entityId;
                        $http.get(url).success(function (atts) {
                            $scope.ajaxFinished = true;
                            $scope.$parent.dataRefresh = true;
                            $scope.$parent.isUploadTemplate = true;
                            $scope.templateList = atts;
                            for (var i = 0; i < atts.length; i++) {
                                atts[i].IsAvailable = (i == atts.length - 1) ? "生效" : "失效";
                            }
                        });
                    };

                    if (!$scope.entityId) {
                        $scope.ajaxFinished = true;
                    } else {
                        $scope.loadingTemplates();
                    }

                    $scope.$watch("projectId", function (val) {
                        if (!!val) {
                            $scope.templateUrl = Utils.ServiceURI.Address() + "api/ClosureConsInvtChecking/DownLoadTemplate/" + val;
                        } else {
                            $scope.templateUrl = "#";
                        }
                    });

                    $scope.uploadFinished = function (up, files) {
                        messager.showMessage("[[[上传成功]]]", "fa-check c_green").then(function () {
                            $scope.$parent.isUploadTemplate = true;
                            $scope.loadingTemplates();
                        });
                    };

                    $scope.$watch("entityId", function (val) {
                        if (!!val) {
                            $scope.loadingTemplates();
                        }
                    });
                }
            };
        }
    ]).directive("closureWriteOffData", [
        function () {
            return {
                restrict: "EA",
                scope: {
                    datas: "="
                },
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "ClosureModule/WriteOffData",
                link: function ($scope, ele, attrs) {
                }
            };
        }
    ]).directive("decisionLogic", [
        "closureToolService",
        function (closureToolService) {
            return {
                restrict: "EA",
                scope: {
                    entity: "=",
                    editable: "=",
                    isLoadFinished: "=",
                    isLoaded: "=",
                    usCode: "="
                },
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "ClosureModule/DecisionLogic",
                link: function ($scope, ele, attrs) {
                    //$scope.$watch("isLoadFinished", function (val) {
                    //    if (!!val && !!$scope.entity && val == true) {
                    //        if ($scope.entity.IsOptionOffered == undefined) {
                    //            $scope.entity.IsOptionOffered = false;
                    //        }

                    //        if (!$scope.entity.IsOptionOffered) {
                    //            //$($("[name='my-radion']").get(1))[0].checked = true;
                    //            $("[name='my-radion']")[1].checked = true;
                    //            disableInput(false);
                    //        } else {
                    //            // $($("[name='my-checkbox']").get(0)).attr("checked", "checked");
                    //            $("[name='my-radion']")[0].checked = true;
                    //            //$($("[name='my-radion']").get(0))[0].checked = true;
                    //        }
                    //        $("[name='my-radion']").on('change', function (e, data) {
                    //            var sel = this.value.toLowerCase() == "true";
                    //            disableInput(sel);

                    //            $scope.$apply();

                    //            $scope.entity.IsOptionOffered = sel;
                    //        });
                    //    }
                    //});

                    //var disableInput = function (sel) {
                    //    setCtrlStatus($scope.frmDecision.compAssumption, sel);
                    //    setCtrlStatus($scope.frmDecision.investment, sel);
                    //    setCtrlStatus($scope.frmDecision.npVRestaurantCashflows, sel);
                    //    setCtrlStatus($scope.frmDecision.yr1SOI, sel);
                    //    setCtrlStatus($scope.frmDecision.irr, sel);
                    //    setCtrlStatus($scope.frmDecision.compAssumption, sel);
                    //    setCtrlStatus($scope.frmDecision.leaseTerm, sel);
                    //    setCtrlStatus($scope.frmDecision.cashflowGrowth, sel);
                    //    setCtrlStatus($scope.frmDecision.relocationPipelineID, sel);
                    //    setCtrlStatus($scope.frmDecision.pipelineName, sel);
                    //    var nums = $("#divDecisionLogic :input[type!='radio']");
                    //    nums.prop("value", "");
                    //    nums.prop("disabled", !sel);
                    //};

                    //var setCtrlStatus = function (ctrl, sel) {
                    //    ctrl.$setViewValue("");
                    //    if (!sel) {
                    //        ctrl.$setValidity("number", true);
                    //        ctrl.$setValidity("required", true);
                    //    } else {
                    //        ctrl.$setValidity("required", false);
                    //    }
                    //};
                    //$scope.$watch("isLoaded", function (val) {
                    //    if ($("[name='my-radion']")[1].checked) {
                    //        disableInput(false);
                    //    }
                    //});
                    $scope.$watch("entity.IsOptionOffered", function (val) {
                        if (!val) {
                            $scope.entity.LeaseTerm = null;
                            $scope.entity.Investment = null;
                            $scope.entity.NPVRestaurantCashflows = null;
                            $scope.entity.Yr1SOI = null;
                            $scope.entity.IRR = null;
                            $scope.entity.RelocationPipelineID = null;
                            $scope.entity.PipelineName = null;
                            $scope.entity.NPVRestaurantCashflows = null;
                            $scope.entity.CompAssumption = null;
                            $scope.entity.CashflowGrowth = null;
                        }
                    });

                    $scope.searchPipelineId = function (inputCode) {
                        return closureToolService.searchPipelineId(inputCode, 5, $scope.usCode).then(function (pipelines) {
                            $scope.Pipelines = pipelines.data;
                            return pipelines.data;
                        });
                    };
                    $scope.$watch("entity.RelocationPipelineID", function (val) {
                        if (val && $scope.Pipelines) {
                            angular.forEach($scope.Pipelines, function (v, k) {
                                if (v.PipelineCode == $scope.entity.RelocationPipelineID) {
                                    $scope.entity.PipelineName = v.PipelineNameZHCN + "-" + v.PipelineNameENUS;
                                    return false;
                                }
                            });
                        }
                    });
                    $scope.searchPipelineName = function (inputCode) {
                        return closureToolService.searchPipelineName(inputCode, 5, $scope.usCode).then(function (pipelines) {
                            $scope.Pipelines = pipelines.data;
                            return pipelines.data;
                        });
                    };
                    $scope.$watch("entity.PipelineName", function (val) {
                        if (val && $scope.Pipelines) {
                            angular.forEach($scope.Pipelines, function (v, k) {
                                if (v.PipelineNameZHCN + "-" + v.PipelineNameENUS == $scope.entity.PipelineName) {
                                    $scope.entity.RelocationPipelineID = v.PipelineCode;
                                    return false;
                                }
                            });
                        }
                    });
                }
            };
        }
    ]).directive("decisionValid", [function () {
        return {
            restrict: 'A',
            require: "ngModel",
            link: function (scope, element, attr, ctrl) {
                if (ctrl) {
                    var customValidator = function (value) {
                        var validity = ctrl.$isEmpty(value);
                        ctrl.$setValidity("multipleEmail", validity);
                        return validity ? value : undefined;
                    };
                    ctrl.$formatters.push(customValidator);
                    ctrl.$parsers.push(customValidator);
                }
            }
        };
    }]).directive("assetsInputs", [
        function () {
            return {
                restrict: "EA",
                scope: {
                    inputs: "=",
                    editable: "="
                },
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "ClosureModule/AssetsInputs",
                link: function ($scope, ele, attrs, ngModelCtrls) {
                }
            };
        }
    ]).directive("impactOnOtherStores", [
        "$http", function ($http) {
            return {
                restrict: "EA",
                scope: {
                    editable: "=",
                    entity: "=",
                    impactStore1: "=?",
                    impactStore2: "=?"
                },
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "ClosureModule/ImpactOnOtherStores",
                link: function ($scope, ele, attrs) {
                    $scope.$watch("entity", function (val) {
                        if (!!val) {
                            $scope.entity = val;

                            var impactStores = $scope.entity.ImpactOtherStores;
                            if (!!impactStores) {
                                $scope.impactStore1 = impactStores.length > 0 ? impactStores[0] : null;
                                if (impactStores.length > 0)
                                    $scope.searchStore($scope.impactStore1.StoreCode, $scope.impactStore1.NameZHCN, 1);
                                $scope.impactStore2 = impactStores.length > 1 ? impactStores[1] : null;
                                if (impactStores.length > 1)
                                    $scope.searchStore($scope.impactStore2.StoreCode, $scope.impactStore2.NameZHCN, 2);
                            }
                        }
                        ;
                    });

                    $scope.$watch("impactStore1.StoreCode", function (val) {
                        if (val == "") {
                            $scope.impactStore1 = {};
                        }
                        if (!!$scope.temp1) {
                            angular.forEach($scope.temp1, function (v, k) {
                                if (v.StoreCode == $scope.impactStore1.StoreCode) {
                                    $scope.impactStore1.NameENUS = v.NameENUS;
                                    $scope.impactStore1.NameZHCN = v.NameZHCN;
                                    return false;
                                }
                            });
                        }
                    });

                    $scope.$watch("impactStore1.NameZHCN", function (val) {
                        if ($.trim(val) == "") {
                            $scope.impactStore1 = {};
                        }
                        if (!!$scope.temp1) {
                            angular.forEach($scope.temp1, function (v, k) {
                                if (v.NameZHCN == $scope.impactStore1.NameZHCN) {
                                    $scope.impactStore1.NameENUS = v.NameENUS;
                                    $scope.impactStore1.StoreCode = v.StoreCode;
                                    return false;
                                }
                            });
                        }
                    });

                    $scope.$watch("impactStore2.StoreCode", function (val) {
                        if (val == "") {
                            $scope.impactStore2 = {};
                        }
                        if (!!$scope.temp2) {
                            angular.forEach($scope.temp2, function (v, k) {
                                if (v.StoreCode == $scope.impactStore2.StoreCode) {
                                    $scope.impactStore2.NameENUS = v.NameENUS;
                                    $scope.impactStore2.NameZHCN = v.NameZHCN;
                                    return false;
                                }
                            });
                        }
                    });

                    $scope.$watch("impactStore2.NameZHCN", function (val) {
                        if (val == "") {
                            $scope.impactStore2 = {};
                        }
                        if (!!$scope.temp2) {
                            //console.log($scope.temp2.length);
                            angular.forEach($scope.temp2, function (v, k) {
                                if (v.NameZHCN == $scope.impactStore2.NameZHCN) {
                                    $scope.impactStore2.NameENUS = v.NameENUS;
                                    $scope.impactStore2.StoreCode = v.StoreCode;
                                    return false;
                                }
                            });
                        }
                    });

                    //$http.get(Utils.ServiceURI.Address() + "api/ClosureTool/GetImpactStores/" + $scope.entity.ProjectId + "/" + $scope.editable).success((stores: any[]) => {
                    //});
                    $scope.searchStore = function (code, name, type) {
                        return $http.get(Utils.ServiceURI.ApiDelegate, {
                            cache: false,
                            params: {
                                url: Utils.ServiceURI.Address() + "api/ClosureTool/GetImpactStores/5/" + $scope.entity.ProjectId,
                                code: code,
                                name: name
                            }
                        }).then(function (response) {
                                if (type == 1) {
                                    $scope.temp1 = response.data;
                                    if ($scope.temp1.length == 0)
                                        $scope.impactStore1 = {};
                                }
                                if (type == 2) {
                                    $scope.temp2 = response.data;
                                    if ($scope.temp2.length == 0)
                                        $scope.impactStore2 = {};
                                }
                                return response.data;
                            });
                    };
                }
            };
        }
    ]).directive("ttmHistoricalFinancialData", [
        "closureToolService",
        function (closureToolService) {
            return {
                restrict: "EA",
                scope: {
                    editable: "=",
                    projectId: "=",
                    entity: "=?",
                    yearmonth: "=?"
                },
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "ClosureModule/TTMHistoricalFinancialData",
                link: function ($scope, ele, attrs) {
                    $scope.plEntity = {};
                    $scope.copyEntity = {};
                    $scope.$watch("yearmonth", function (val) {
                        getFinanceData(val);
                    });
                    var getFinanceData = function (yearmonth) {
                        var year = "", month = "";
                        if (yearmonth && yearmonth.value.indexOf('-') > 0) {
                            year = yearmonth.value.split('-')[0];
                            month = yearmonth.value.split('-')[1];
                        }
                        closureToolService.getFinanceData($scope.entity.Id, $scope.projectId, year, month).then(function (response) {
                            var financedata = response.data;
                            $scope.plEntity.YearMonth = financedata.yearMonth;
                            $scope.plEntity.TotalSales = financedata.totalSales;
                            $scope.plEntity.CompSales = financedata.comp_sales_ttm;
                            $scope.plEntity.CompSalesMacket = financedata.comp_sales_market_ttm;
                            $scope.plEntity.CompCG = financedata.comp_gc_ttm;
                            $scope.plEntity.CompCGMacket = financedata.comp_gc_market_ttm;
                            $scope.plEntity.PAC_RMB = financedata.Pac_TTM;
                            $scope.plEntity.PAC = financedata.PACPct_TTM;
                            $scope.plEntity.PACMarket = financedata.PACPct_MARKET_TTM;
                            $scope.plEntity.Rent_RMB = response.data.rent;
                            $scope.plEntity.DepreciationLHI_RMB = financedata.Depreciation_LHI_TTM;
                            $scope.plEntity.InterestLHI_RMB = financedata.Interest_LHI_TTM;
                            $scope.plEntity.ServiceFee_RMB = financedata.Service_Fee_TTM;
                            $scope.plEntity.Accounting_RMB = financedata.Accounting_TTM;
                            $scope.plEntity.Insurance_RMB = financedata.Insurance_TTM;
                            $scope.plEntity.TaxesLicenses_RMB = financedata.Taxes_Licenses_TTM;
                            $scope.plEntity.Depreciation_ESSD_RMB = financedata.Depreciation_Essd_TTM;
                            $scope.plEntity.Interest_ESSD_RMB = financedata.Interest_Essd_TTM;
                            $scope.plEntity.OtherIncExp_RMB = financedata.Other_Exp_TTM;
                            $scope.plEntity.Product_Sales_RMB = financedata.Product_Sales_TTM;
                            $scope.plEntity.NonProduct_Sales_RMB = financedata.Non_Product_Sales_TTM;
                            $scope.plEntity.NonProduct_Costs_RMB = financedata.Non_Product_Costs_TTM;
                            $scope.plEntity.SOI = financedata.SOI;
                            $scope.plEntity.SOIMarket = financedata.SOIPct_MARKET_TTM;
                            $scope.plEntity.CashFlow_RMB = financedata.Cash_Flow_TTM;
                            if (!$scope.copyEntity.TotalSales_Adjustment_RMB && !financedata.reset) {
                                $scope.copyEntity.TotalSales_Adjustment_RMB = $scope.entity.TotalSales_Adjustment_RMB;
                                $scope.copyEntity.CompSales_Adjustment = $scope.entity.CompSales_Adjustment;
                                $scope.copyEntity.CompSalesMacket_Adjustment = $scope.entity.CompSalesMacket_Adjustment;
                                $scope.copyEntity.CompCG_Adjustment = $scope.entity.CompCG_Adjustment;
                                $scope.copyEntity.CompCGMacket_Adjustment = $scope.entity.CompCGMacket_Adjustment;
                                $scope.copyEntity.PAC_RMB_Adjustment = $scope.entity.PAC_RMB_Adjustment;
                                $scope.copyEntity.PAC_Adjustment = $scope.entity.PAC_Adjustment;
                                $scope.copyEntity.PACMarket_Adjustment = $scope.entity.PACMarket_Adjustment;
                                $scope.copyEntity.Rent_RMB_Adjustment = $scope.entity.Rent_RMB_Adjustment;
                                $scope.copyEntity.DepreciationLHI_RMB_Adjustment = $scope.entity.DepreciationLHI_RMB_Adjustment;
                                $scope.copyEntity.InterestLHI_RMB_Adjustment = $scope.entity.InterestLHI_RMB_Adjustment;
                                $scope.copyEntity.ServiceFee_RMB_Adjustment = $scope.entity.ServiceFee_RMB_Adjustment;
                                $scope.copyEntity.Accounting_RMB_Adjustment = $scope.entity.Accounting_RMB_Adjustment;
                                $scope.copyEntity.Insurance_RMB_Adjustment = $scope.entity.Insurance_RMB_Adjustment;
                                $scope.copyEntity.TaxesLicenses_RMB_Adjustment = $scope.entity.TaxesLicenses_RMB_Adjustment;
                                $scope.copyEntity.Depreciation_ESSD_RMB_Adjustment = $scope.entity.Depreciation_ESSD_RMB_Adjustment;
                                $scope.copyEntity.Interest_ESSD_RMB_Adjustment = $scope.entity.Interest_ESSD_RMB_Adjustment;
                                $scope.copyEntity.OtherIncExp_RMB_Adjustment = $scope.entity.OtherIncExp_RMB_Adjustment;
                                $scope.copyEntity.Product_Sales_RMB_Adjustment = $scope.entity.Product_Sales_RMB_Adjustment;
                                $scope.copyEntity.NonProduct_Sales_RMB_Adjustment = $scope.entity.NonProduct_Sales_RMB_Adjustment;
                                $scope.copyEntity.NonProduct_Costs_RMB_Adjustment = $scope.entity.NonProduct_Costs_RMB_Adjustment;
                                $scope.copyEntity.SOI_Adjustment = $scope.entity.SOI_Adjustment;
                                $scope.copyEntity.SOIMarket_Adjustment = $scope.entity.SOIMarket_Adjustment;
                                $scope.copyEntity.CashFlow_RMB_Adjustment = $scope.entity.CashFlow_RMB_Adjustment;
                                $scope.copyEntity.CompSales = $scope.entity.CompSales;
                                $scope.copyEntity.CompSalesMacket = $scope.entity.CompSalesMacket;
                                $scope.copyEntity.CompCG = $scope.entity.CompCG;
                                $scope.copyEntity.CompCGMacket = financedata.comp_gc_market_ttm;
                                $scope.copyEntity.TotalSales_TTMY1 = $scope.entity.TotalSales_TTMY1;
                                $scope.copyEntity.TotalSales_TTMY2 = $scope.entity.TotalSales_TTMY2;
                                $scope.copyEntity.CompSales_TTMY1 = $scope.entity.CompSales_TTMY1;
                                $scope.copyEntity.CompSales_TTMY2 = $scope.entity.CompSales_TTMY2;
                                $scope.copyEntity.CompSales_Market_TTMY1 = $scope.entity.CompSales_Market_TTMY1;
                                $scope.copyEntity.CompSales_Market_TTMY2 = $scope.entity.CompSales_Market_TTMY2;
                                $scope.copyEntity.CompGC_TTMY1 = $scope.entity.CompGC_TTMY1;
                                $scope.copyEntity.CompGC_TTMY2 = $scope.entity.CompGC_TTMY2;
                                $scope.copyEntity.CompGCMarket_TTMY1 = $scope.entity.CompGCMarket_TTMY1;
                                $scope.copyEntity.CompGCMarket_TTMY2 = $scope.entity.CompGCMarket_TTMY2;
                                $scope.copyEntity.PAC_TTMY1 = $scope.entity.PAC_TTMY1;
                                $scope.copyEntity.PAC_TTMY2 = $scope.entity.PAC_TTMY2;
                                $scope.copyEntity.PACMarket_TTMY1 = $scope.entity.PACMarket_TTMY1;
                                $scope.copyEntity.PACMarket_TTMY2 = $scope.entity.PACMarket_TTMY2;
                                $scope.copyEntity.SOI_TTMY1 = $scope.entity.SOI_TTMY1;
                                $scope.copyEntity.SOI_TTMY2 = $scope.entity.SOI_TTMY2;
                                $scope.copyEntity.SOIMarket_TTMY1 = $scope.entity.SOIMarket_TTMY1;
                                $scope.copyEntity.SOIMarket_TTMY2 = $scope.entity.SOIMarket_TTMY2;
                                $scope.copyEntity.CashFlow_TTMY1 = $scope.entity.CashFlow_TTMY1;
                                $scope.copyEntity.CashFlow_TTMY2 = $scope.entity.CashFlow_TTMY2;
                            }
                            if (!!$scope.editable && !!financedata.reset) {
                                $scope.entity.TotalSales_Adjustment_RMB = $scope.plEntity.TotalSales;
                                $scope.entity.CompSales_Adjustment = $scope.plEntity.CompSales;
                                $scope.entity.CompSalesMacket_Adjustment = $scope.plEntity.CompSalesMacket;
                                $scope.entity.CompCG_Adjustment = $scope.plEntity.CompCG;
                                $scope.entity.CompCGMacket_Adjustment = $scope.plEntity.CompCGMacket;
                                $scope.entity.PAC_RMB_Adjustment = $scope.plEntity.PAC_RMB;
                                $scope.entity.PAC_Adjustment = $scope.plEntity.PAC;
                                $scope.entity.PACMarket_Adjustment = $scope.plEntity.PACMarket;
                                $scope.entity.Rent_RMB_Adjustment = $scope.plEntity.Rent_RMB;
                                $scope.entity.DepreciationLHI_RMB_Adjustment = $scope.plEntity.DepreciationLHI_RMB;
                                $scope.entity.InterestLHI_RMB_Adjustment = $scope.plEntity.InterestLHI_RMB;
                                $scope.entity.ServiceFee_RMB_Adjustment = $scope.plEntity.ServiceFee_RMB;
                                $scope.entity.Accounting_RMB_Adjustment = $scope.plEntity.Accounting_RMB;
                                $scope.entity.Insurance_RMB_Adjustment = $scope.plEntity.Insurance_RMB;
                                $scope.entity.TaxesLicenses_RMB_Adjustment = $scope.plEntity.TaxesLicenses_RMB;
                                $scope.entity.Depreciation_ESSD_RMB_Adjustment = $scope.plEntity.Depreciation_ESSD_RMB;
                                $scope.entity.Interest_ESSD_RMB_Adjustment = $scope.plEntity.Interest_ESSD_RMB;
                                $scope.entity.OtherIncExp_RMB_Adjustment = $scope.plEntity.OtherIncExp_RMB;
                                $scope.entity.Product_Sales_RMB_Adjustment = $scope.plEntity.Product_Sales_RMB;
                                $scope.entity.NonProduct_Sales_RMB_Adjustment = $scope.plEntity.NonProduct_Sales_RMB;
                                $scope.entity.NonProduct_Costs_RMB_Adjustment = $scope.plEntity.NonProduct_Costs_RMB;
                                $scope.entity.SOI_Adjustment = $scope.plEntity.SOI;
                                $scope.entity.SOIMarket_Adjustment = $scope.plEntity.SOIMarket;
                                $scope.entity.CashFlow_RMB_Adjustment = $scope.plEntity.CashFlow_RMB;
                                $scope.entity.CompSales = financedata.comp_sales_ttm;
                                $scope.entity.CompSalesMacket = financedata.comp_sales_market_ttm;
                                $scope.entity.CompCG = financedata.comp_gc_ttm;
                                $scope.plEntity.CompCGMacket = financedata.comp_gc_market_ttm;
                                $scope.entity.TotalSales_TTMY1 = financedata.Total_Sales_TTMPY1;
                                $scope.entity.TotalSales_TTMY2 = financedata.Total_Sales_TTMPY2;
                                $scope.entity.CompSales_TTMY1 = financedata.comp_sales_ttm_py1;
                                $scope.entity.CompSales_TTMY2 = financedata.comp_sales_ttm_py2;
                                $scope.entity.CompSales_Market_TTMY1 = financedata.comp_sales_market_ttm_py1;
                                $scope.entity.CompSales_Market_TTMY2 = financedata.comp_sales_market_ttm_py2;
                                $scope.entity.CompGC_TTMY1 = financedata.comp_gc_ttm_py1;
                                $scope.entity.CompGC_TTMY2 = financedata.comp_gc_ttm_py2;
                                $scope.entity.CompGCMarket_TTMY1 = financedata.comp_gc_market_ttm_py1;
                                $scope.entity.CompGCMarket_TTMY2 = financedata.comp_gc_market_ttm_py2;
                                $scope.entity.PAC_TTMY1 = financedata.PAC_TTMPreviousY1;
                                $scope.entity.PAC_TTMY2 = financedata.PAC_TTMPreviousY2;
                                $scope.entity.PACMarket_TTMY1 = financedata.PACPct_MARKET_TTMPreviousY1;
                                $scope.entity.PACMarket_TTMY2 = financedata.PACPct_MARKET_TTMPreviousY2;
                                $scope.entity.SOI_TTMY1 = financedata.SOI_TTMPreviousY1;
                                $scope.entity.SOI_TTMY2 = financedata.SOI_TTMPreviousY2;
                                $scope.entity.SOIMarket_TTMY1 = financedata.SOIPct_MARKET_TTMPreviousY1;
                                $scope.entity.SOIMarket_TTMY2 = financedata.SOIPct_MARKET_TTMPreviousY2;
                                $scope.entity.CashFlow_TTMY1 = financedata.Cash_Flow_TTMPreviousY1;
                                $scope.entity.CashFlow_TTMY2 = financedata.Cash_Flow_TTMPreviousY2;
                            }
                            else {
                                $scope.entity.TotalSales_Adjustment_RMB = $scope.copyEntity.TotalSales_Adjustment_RMB;
                                $scope.entity.CompSales_Adjustment = $scope.copyEntity.CompSales_Adjustment;
                                $scope.entity.CompSalesMacket_Adjustment = $scope.copyEntity.CompSalesMacket_Adjustment;
                                $scope.entity.CompCG_Adjustment = $scope.copyEntity.CompCG_Adjustment;
                                $scope.entity.CompCGMacket_Adjustment = $scope.copyEntity.CompCGMacket_Adjustment;
                                $scope.entity.PAC_RMB_Adjustment = $scope.copyEntity.PAC_RMB_Adjustment;
                                $scope.entity.PAC_Adjustment = $scope.copyEntity.PAC_Adjustment;
                                $scope.entity.PACMarket_Adjustment = $scope.copyEntity.PACMarket_Adjustment;
                                $scope.entity.Rent_RMB_Adjustment = $scope.copyEntity.Rent_RMB_Adjustment;
                                $scope.entity.DepreciationLHI_RMB_Adjustment = $scope.copyEntity.DepreciationLHI_RMB_Adjustment;
                                $scope.entity.InterestLHI_RMB_Adjustment = $scope.copyEntity.InterestLHI_RMB_Adjustment;
                                $scope.entity.ServiceFee_RMB_Adjustment = $scope.copyEntity.ServiceFee_RMB_Adjustment;
                                $scope.entity.Accounting_RMB_Adjustment = $scope.copyEntity.Accounting_RMB_Adjustment;
                                $scope.entity.Insurance_RMB_Adjustment = $scope.copyEntity.Insurance_RMB_Adjustment;
                                $scope.entity.TaxesLicenses_RMB_Adjustment = $scope.copyEntity.TaxesLicenses_RMB_Adjustment;
                                $scope.entity.Depreciation_ESSD_RMB_Adjustment = $scope.copyEntity.Depreciation_ESSD_RMB_Adjustment;
                                $scope.entity.Interest_ESSD_RMB_Adjustment = $scope.copyEntity.Interest_ESSD_RMB_Adjustment;
                                $scope.entity.OtherIncExp_RMB_Adjustment = $scope.copyEntity.OtherIncExp_RMB_Adjustment;
                                $scope.entity.Product_Sales_RMB_Adjustment = $scope.copyEntity.Product_Sales_RMB_Adjustment;
                                $scope.entity.NonProduct_Sales_RMB_Adjustment = $scope.copyEntity.NonProduct_Sales_RMB_Adjustment;
                                $scope.entity.NonProduct_Costs_RMB_Adjustment = $scope.copyEntity.NonProduct_Costs_RMB_Adjustment;
                                $scope.entity.SOI_Adjustment = $scope.copyEntity.SOI_Adjustment;
                                $scope.entity.SOIMarket_Adjustment = $scope.copyEntity.SOIMarket_Adjustment;
                                $scope.entity.CashFlow_RMB_Adjustment = $scope.copyEntity.CashFlow_RMB_Adjustment;
                                $scope.entity.CompSales = $scope.copyEntity.CompSales;
                                $scope.entity.CompSalesMacket = $scope.copyEntity.CompSalesMacket;
                                $scope.entity.CompCG = $scope.copyEntity.CompCG;
                                $scope.plEntity.CompCGMacket = $scope.copyEntity.CompCGMacket;
                                $scope.entity.TotalSales_TTMY1 = $scope.copyEntity.TotalSales_TTMY1;
                                $scope.entity.TotalSales_TTMY2 = $scope.copyEntity.TotalSales_TTMY2;
                                $scope.entity.CompSales_TTMY1 = $scope.copyEntity.CompSales_TTMY1;
                                $scope.entity.CompSales_TTMY2 = $scope.copyEntity.CompSales_TTMY2;
                                $scope.entity.CompSales_Market_TTMY1 = $scope.copyEntity.CompSales_Market_TTMY1;
                                $scope.entity.CompSales_Market_TTMY2 = $scope.copyEntity.CompSales_Market_TTMY2;
                                $scope.entity.CompGC_TTMY1 = $scope.copyEntity.CompGC_TTMY1;
                                $scope.entity.CompGC_TTMY2 = $scope.copyEntity.CompGC_TTMY2;
                                $scope.entity.CompGCMarket_TTMY1 = $scope.copyEntity.CompGCMarket_TTMY1;
                                $scope.entity.CompGCMarket_TTMY2 = $scope.copyEntity.CompGCMarket_TTMY2;
                                $scope.entity.PAC_TTMY1 = $scope.copyEntity.PAC_TTMY1;
                                $scope.entity.PAC_TTMY2 = $scope.copyEntity.PAC_TTMY2;
                                $scope.entity.PACMarket_TTMY1 = $scope.copyEntity.PACMarket_TTMY1;
                                $scope.entity.PACMarket_TTMY2 = $scope.copyEntity.PACMarket_TTMY2;
                                $scope.entity.SOI_TTMY1 = $scope.copyEntity.SOI_TTMY1;
                                $scope.entity.SOI_TTMY2 = $scope.copyEntity.SOI_TTMY2;
                                $scope.entity.SOIMarket_TTMY1 = $scope.copyEntity.SOIMarket_TTMY1;
                                $scope.entity.SOIMarket_TTMY2 = $scope.copyEntity.SOIMarket_TTMY2;
                                $scope.entity.CashFlow_TTMY1 = $scope.copyEntity.CashFlow_TTMY1;
                                $scope.entity.CashFlow_TTMY2 = $scope.copyEntity.CashFlow_TTMY2;
                            }
                            $scope.plEntity.TotalSales_TTMY1 = financedata.Total_Sales_TTMPY1;
                            $scope.plEntity.TotalSales_TTMY2 = financedata.Total_Sales_TTMPY2;
                            $scope.plEntity.CompSales_TTMY1 = financedata.comp_sales_ttm_py1;
                            $scope.plEntity.CompSales_TTMY2 = financedata.comp_sales_ttm_py2;
                            $scope.plEntity.CompSales_Market_TTMY1 = financedata.comp_sales_market_ttm_py1;
                            $scope.plEntity.CompSales_Market_TTMY2 = financedata.comp_sales_market_ttm_py2;
                            $scope.plEntity.CompGC_TTMY1 = financedata.comp_gc_ttm_py1;
                            $scope.plEntity.CompGC_TTMY2 = financedata.comp_gc_ttm_py2;
                            $scope.plEntity.CompGCMarket_TTMY1 = financedata.comp_gc_market_ttm_py1;
                            $scope.plEntity.CompGCMarket_TTMY2 = financedata.comp_gc_market_ttm_py2;
                            $scope.plEntity.PAC_TTMY1 = financedata.PAC_TTMPreviousY1;
                            $scope.plEntity.PAC_TTMY2 = financedata.PAC_TTMPreviousY2;
                            $scope.plEntity.PACMarket_TTMY1 = financedata.PACPct_MARKET_TTMPreviousY1;
                            $scope.plEntity.PACMarket_TTMY2 = financedata.PACPct_MARKET_TTMPreviousY2;
                            $scope.plEntity.SOI_TTMY1 = financedata.SOI_TTMPreviousY1;
                            $scope.plEntity.SOI_TTMY2 = financedata.SOI_TTMPreviousY2;
                            $scope.plEntity.SOIMarket_TTMY1 = financedata.SOIPct_MARKET_TTMPreviousY1;
                            $scope.plEntity.SOIMarket_TTMY2 = financedata.SOIPct_MARKET_TTMPreviousY2;
                            $scope.plEntity.CashFlow_TTMY1 = financedata.Cash_Flow_TTMPreviousY1;
                            $scope.plEntity.CashFlow_TTMY2 = financedata.Cash_Flow_TTMPreviousY2;
                        });
                    };
                    if (!!$scope.editable) {
                        closureToolService.getSelectYearMonth($scope.projectId).then(function (response) {
                            $scope.yearMonthList = response.data;
                            for (var i = 0; i < response.data.length; i++) {
                                if (response.data[i].selected)
                                    $scope.yearmonth = response.data[i];
                            }
                            if ($scope.yearmonth == undefined) {
                                $scope.yearmonth = response.data[0];
                            }
                        });
                    }
                }
            };
        }
    ]).directive("closureMemo", [
        "$window",
        "messager",
        "closureMemoService",
        "closureToolService",
        function ($window, messager, closureMemoService, closureToolService) {
            return {
                restrict: "EA",
                replace: true,
                scope: {
                    editable: "@",
                    projectId: "@",
                    entity: "=?"
                },
                templateUrl: Utils.ServiceURI.AppUri + "ClosureModule/ClosureMemo?nc=" + Math.random(),
                link: function ($scope, element, attrs) {
                    closureMemoService.get($scope.projectId).then(function (response) {
                        $scope.entity = response.data.Entity;
                        $scope.ClosureDate = response.data.Entity.ClosureDate;
                        $scope.searchedPipeline = new function () {
                            this.PipelineID = !$scope.entity.PipelineId ? '' : $scope.entity.PipelineId;
                            this.PipelineNameZHCN = $scope.entity.PipelineName;
                            this.PipelineNameENUS = ''; //缺少字段
                            this.PipelineCode = '';
                        };
                    });
                    closureToolService.getCompensation($scope.projectId).then(function (response) {
                        $scope.compensation = Number(response.data);
                        $scope.compensationLoaded = true;
                    }, function (response) {
                            $scope.compensation = "暂无数据";
                            $scope.compensationLoaded = true;
                        });
                    $scope.searchPipeline = function (inputCode) {
                        return closureMemoService.searchPipeline(inputCode, 5, $scope.entity.USCode).then(function (pipelines) {
                            return pipelines.data;
                        });
                    };
                    $scope.$watch("searchedPipeline", function (s) {
                        if (!!s && !!s.PipelineNameZHCN) {
                            $scope.entity.PipelineName = s.PipelineNameZHCN;
                            $scope.entity.PipelineId = s.PipelineID;
                        }
                    });
                    //$scope.$watch("entity.ClosureDate", function (val) {
                    //    var now = new Date();
                    //    if (!!val && val != $scope.ClosureDate && Number(moment(val).format("YYYYMMDD")) < Number(moment(now).format("YYYYMMDD"))) {
                    //        messager.confirm("Closure Date早于今天,Closure Memo将不能再修改了,您确定要修改吗？", "fa-warning c_red").then(function (result) {
                    //            if (!result) {
                    //                $scope.entity.ClosureDate = $scope.ClosureDate;
                    //            }
                    //        });
                    //    }
                    //    ;
                    //});

                    $scope.openDate = function ($event, tag) {
                        $event.preventDefault();
                        $event.stopPropagation();
                        $scope[tag] = true;
                    };
                }
            };
        }]).directive("legalReviewStoreBasicInfo", [
        "$http", function ($http) {
            return {
                restrict: "EA",
                scope: {
                    code: "=",
                    project: "=?"
                },
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "ClosureModule/legalReviewStoreBasicInfo",
                link: function ($scope, ele, attrs) {
                    $scope.$watch("code", function (val) {
                        if (!!val) {
                            $http.get(Utils.Constants.ApiDelegate, {
                                params: {
                                    url: Utils.ServiceURI.Address() + "api/Store/Details/" + val
                                }
                            }).success(function (data) {
                                    $scope.store = data;
                                    $scope.store.linkUrl = "/StoreList/?uscode=" + val + "&backurl=" + escape(window.location.toString());
                                });

                            var url = Utils.ServiceURI.Address() + "api/StoreUsers/GetStoreEmployeesByMultiRoles/" + val + "?roleCodes=Market_DD,GM,VPGM";
                            $http.get(url).then(function (response) {
                                var userPositionData = response.data;

                                for (var i = 0; i < response.data.length; i++) {
                                    var positionData = userPositionData[i];
                                    switch (positionData.PositionENUS) {
                                        case "VPGM":
                                            $scope.VPGM = positionData;
                                            break;
                                        case "Market DD":
                                            $scope.MDD = positionData;
                                            break;
                                        case "GM":
                                            $scope.GM = positionData;
                                            break;
                                    }
                                }
                            });
                        }
                    });
                }
            };
        }]).directive("closureToolStoreBasicInfo", [
        "$http", function ($http) {
            return {
                restrict: "EA",
                scope: {
                    code: "=",
                    project: "=?"
                },
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "ClosureModule/closureToolStoreBasicInfo",
                link: function ($scope, ele, attrs) {
                    $scope.$watch("code", function (val) {
                        if (!!val) {
                            $http.get(Utils.Constants.ApiDelegate, {
                                params: {
                                    url: Utils.ServiceURI.Address() + "api/Store/Details/" + val
                                }
                            }).success(function (data) {
                                    $scope.store = data;
                                    $scope.store.linkUrl = "/StoreList/?uscode=" + val + "&backurl=" + escape(window.location.toString());
                                });
                        }
                    });
                }
            };
        }]).directive("executiveSummary", [
        "$http", function ($http) {
            return {
                restrict: "EA",
                scope: {
                    projectId: "=",
                    entity: "=",
                    editable: "="
                },
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "ClosureModule/ExecutiveSummary",
                link: function ($scope, ele, attrs) {
                    $scope.$watch("projectId", function (val) {
                        if (!!val) {
                            var url = Utils.ServiceURI.Address() + "api/ClosureExecutiveSummary/GetOnline/" + $scope.projectId;
                            $http.get(url).success(function (data) {
                                $scope.Store = data.Store;
                                $scope.ClosureTool = data.ClosureTool;
                                $scope.WOCheckList = data.WOCheckList;
                                $scope.RemoteKiosk1_Status = "No";
                                $scope.RemoteKiosk2_Status = "No";
                                $scope.RemoteKiosk3_Status = "No";
                                $scope.AttachedKiosk1_Status = "No";
                                $scope.AttachedKiosk2_Status = "No";
                                $scope.AttachedKiosk3_Status = "No";
                                $scope.MDS_Status = "No";
                                $scope.McCafe_Status = "No";
                                $scope.TwentyFourHour_Status = "No";

                                if (data.RemoteBeList.length > 0) {
                                    $scope.RemoteKiosk1_Status = "Yes";
                                    $scope.RemoteKiosk1_OpenDate = datetimeConvert(data.RemoteBeList[0].LaunchDate);
                                    if (data.RemoteBeList.length > 1) {
                                        $scope.RemoteKiosk2_Status = "Yes";
                                        $scope.RemoteKiosk2_OpenDate = datetimeConvert(data.RemoteBeList[1].LaunchDate);
                                        if (data.RemoteBeList.length > 2) {
                                            $scope.RemoteKiosk3_Status = "Yes";
                                            $scope.RemoteKiosk3_OpenDate = datetimeConvert(data.RemoteBeList[2].LaunchDate);
                                        }
                                    }
                                }
                                if (data.AttachedBeList.length > 0) {
                                    $scope.AttachedKiosk1_Status = "Yes";
                                    $scope.AttachedKiosk1_OpenDate = datetimeConvert(data.AttachedBeList[0].LaunchDate);
                                    if (data.AttachedBeList.length > 1) {
                                        $scope.AttachedKiosk2_Status = "Yes";
                                        $scope.AttachedKiosk2_OpenDate = datetimeConvert(data.AttachedBeList[1].LaunchDate);
                                        if (data.AttachedBeList.length > 2) {
                                            $scope.AttachedKiosk3_Status = "Yes";
                                            $scope.AttachedKiosk3_OpenDate = datetimeConvert(data.AttachedBeList[2].LaunchDate);
                                        }
                                    }
                                }
                                if (data.MDS != null) {
                                    $scope.MDS_Status = "Yes";
                                    $scope.MDS_OpenDate = datetimeConvert(data.MDS.LaunchDate);
                                }
                                if (data.McCafe != null) {
                                    $scope.McCafe_Status = "Yes";
                                    $scope.McCafe_OpenDate = datetimeConvert(data.McCafe.LaunchDate);
                                }
                                if (data.Hour24 != null) {
                                    $scope.TwentyFourHour_Status = "Yes";
                                    $scope.TwentyFourHour_OpenDate = datetimeConvert(data.Hour24.LaunchDate);
                                }
                            });
                        }
                    });
                    var datetimeConvert = (dtValue) => {
                        var result = '';
                        if (dtValue == null || dtValue.length == 0)
                            result = '';
                        else
                            result = moment(dtValue.toString().replace('T', ' '), "YYYYMMDD").format("YYYY-MM-DD");

                        result = result.replace('1900-01-01', '');
                        return result;
                    }
                }
            };
        }]);