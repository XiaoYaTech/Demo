angular.module("mcd.am.modules")
    .directive("numberShow", [
        "$filter",
        function ($filter) {
            return {
                restrict: "A",
                require: "ngModel",
                replace: false,
                link: function ($scope, ele, attrs, ctrls) {
                    var $span = $("<input class='form-control' />").insertAfter(ele);
                    var showType = attrs.numberShow;
                    if (ele.is(":disabled")) {
                        $span.attr("disabled", "disabled");
                    }
                    var display = function (val) {
                        $span.attr("class", ele.attr("class"));
                        $span.val($filter("numberDisplay")(val, showType));
                    };
                    if (!!attrs.ngDisabled) {
                        $scope.$watch(attrs.ngDisabled, function (val) {
                            if (val) {
                                $span.attr("disabled", "disabled");
                            } else {
                                $span.removeAttr("disabled");
                            }
                        });
                    }
                    $scope.$watch(attrs.ngModel, display);
                    $span.on("focus", function () {
                        if (!ele.is(":disabled") && !ele.attr("readonly")) {
                            ele.show();
                            ele.focus();
                            $span.hide();
                        }
                    });
                    ele.on("blur", function () {
                        $span.show();
                        ele.hide();
                    });
                    $span.show();
                    ele.hide();
                }
            };
        }
    ])
    .directive('inputPercent', [function () {
        return {
            restrict: "A",
            require: "ngModel",
            replace: false,
            link: function ($scope, ele, attrs, ctrls) {
                var $input = $("<input />").insertAfter(ele);
                ele.hide();

                var mappingClasses = function (isInput) {
                    $input.get(0).className = ele.get(0).className;
                    if (ele.is(":disabled")) {
                        $input.attr("disabled", "disabled");
                    } else {
                        $input.removeAttr("disabled");
                    }
                }

                attrs.$observe('disabled', function () {
                    return mappingClasses();
                });

                var isKeyUp = false;
                if (!!attrs.ngDisabled) {
                    $scope.$watch(attrs.ngDisabled, function (val) {
                        if (val) {
                            $input.attr("disabled", "disabled");
                        } else {
                            $input.removeAttr("disabled");
                        }
                    });
                }
                $input.keyup(function () {
                    var val = $input.val();
                    if (val == null || val == "" || val == undefined) {
                        ctrls.$setViewValue(null);
                    } else {
                        var number = Number(val);
                        if (isNaN(number)) {
                            ctrls.$setViewValue(val);
                        } else {
                            ctrls.$setViewValue(Number(Utils.caculator.division(number, 100).toFixed(3)));
                        }
                    }
                    isKeyUp = true;
                    $scope.$apply();
                });
                
                $scope.$watch(attrs.ngModel, function (val) {
                    mappingClasses();
                    if (!isKeyUp && val !== null && val !== undefined && val !== "") {
                        $input.val(Number(Utils.caculator.multiply(val, 100).toFixed(1)));
                    }
                    isKeyUp = false;
                });
            }
        };
    }])
    .directive("inputMoney", [function () {
        return {
            restrict: "A",
            require: "ngModel",
            replace: false,
            link: function ($scope, ele, attrs, ctrls) {
                var $input = $("<input />").insertAfter(ele);
                ele.hide();

                var mappingClasses = function (isInput) {
                    $input.get(0).className = ele.get(0).className;
                    if (ele.is(":disabled")) {
                        $input.attr("disabled", "disabled");
                    } else {
                        $input.removeAttr("disabled");
                    }
                }
                if (!!attrs.ngDisabled) {
                    $scope.$watch(attrs.ngDisabled, function (val) {
                        if (val) {
                            $input.attr("disabled", "disabled");
                        } else {
                            $input.removeAttr("disabled");
                        }
                    });
                }
                $scope.$watch(attrs.ngModel, function (val) {
                    mappingClasses();
                    if (val !== null && val !== undefined && val !== "") {
                        $input.val(Number(val).toFixed(0));
                    }
                });
            }
        };
    }])
    .directive("amMutiSelect", ["$parse", function ($parse) {
        return {
            restrict: "E",
            require: "ngModel",
            replace: true,
            scope: {
                options: "=",
                valueField: "@",
                displayField: "@",
                model: "=ngModel",
                required: "@",
                editable: "@",
            },
            templateUrl: Utils.ServiceURI.AppUri + "Module/AMMutiSelect",
            link: function ($scope, ele, attrs, ctrls) {
                $scope.selectedText = function () {
                    return $.map($.grep($scope.options || [], function (op, i) {
                        return op.Selected;
                    }) || [], function (op, i) {
                        return $scope.optionText(op);
                    }).join(",");
                };
                var init = function () {
                    if (!!$scope.model && !!$scope.options) {
                        var selects = $scope.model ? $scope.model.split(",") : [];
                        var text = "";
                        if (selects.length > 0) {
                            angular.forEach($scope.options, function (op, i) {
                                if ($.inArray($scope.optionValue(op), selects) >= 0) {
                                    op.Selected = true;
                                    if (text != "") {
                                        text += ",";
                                    }
                                    text += $scope.optionText(op);
                                } else {
                                    op.Selected = false;
                                }
                            });
                        }
                        $scope.text = text;
                    }
                };
                var $input = $(".form-control", ele);
                var $con = $(".am-multi-select-con", ele);
                $input.click(function (e) {
                    e.preventDefault();
                    if ($scope.editable) {
                        if ($con.is(":animated")) {
                            $con.stop();
                        }
                        $con.slideDown();
                    }
                });
                $(document).click(function (e) {
                    var $tar = $(e.srcElement || e.target);
                    if ($tar.get(0) != $con.get(0)
                        && $tar.get(0) != $input.get(0)
                        && $tar.parents(".am-multi-select-con").get(0) != $con.get(0)
                        && $con.is(":visible")) {
                        if ($con.is(":animated")) {
                            $con.stop();
                        }
                        $con.slideUp();
                    }
                });
                $scope.$watch("options", init);
                $scope.$watch("model", init);
                $scope.selectChange = function () {
                    var selectedOps = $.grep($scope.options || [], function (op, i) {
                        return op.Selected;
                    }) || [];
                    $scope.text = $.map(selectedOps, function (op, i) {
                        return $scope.optionText(op);
                    }).join(",");
                    $scope.model = $.map(selectedOps, function (op, i) {
                        return $scope.optionValue(op);
                    }).join(",");
                    return true;
                }
                $scope.optionValue = function (op) {
                    return $scope.valueField ? op[$scope.valueField] : op;
                }
                $scope.optionText = function (op) {
                    return $scope.displayField ? op[$scope.displayField] : op;
                }
            }
        };
    }]);