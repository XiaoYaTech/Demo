angular.module("mcd.am.modules")
    .directive("formSubmit", [function(){
        return {
            restrict: "A",
            link: function ($scope, element, attrs) {
                element.on("click", function () {
                    $scope.$submited = true;
                    $scope.$root.$submited = true;
                    if ($scope.$parent) {
                        $scope.$parent.$submited = true;
                    }
                    $scope.$broadcast("formSubmited");
                    $("input,textarea,select").addClass("ng-submit");
                    $scope.$apply();
                    var $firstError = $(".ng-error:visible").first();
                    if ($firstError.length > 0) {
                        var $pDoc = $(window.parent.document.documentElement);
                        if ($pDoc.is(":animated")) {
                            $pDoc.stop();
                        }
                        $pDoc.animate({ scrollTop: $firstError.offset().top }, 200);
                    }
                });
            }
        };
    }])
    .directive('valnumber', [function () {
        return {
            require: 'ngModel',
            link: function (scope, ele, attrs, c) {
                scope.$watch(attrs.ngModel, function (val) {
                    if (!!val) {
                        if (!isNaN(Number(val))) {
                            c.$setValidity('valnumber', true);
                        } else {
                            c.$setValidity('valnumber', false);
                        }
                    } else {
                        c.$setValidity('valnumber', true);
                    }
                });
            }
        }
    }])
    .directive('percent', [function () {
        return {
            require: 'ngModel',
            link: function (scope, ele, attrs, c) {
                scope.$watch(attrs.ngModel, function (val) {
                    if (!!val) {
                        var num = Number(val);
                        if (isNaN(num) || num > 1 || num < 0) {
                            c.$setValidity('percent', false);
                        } else {
                            c.$setValidity('percent', true);
                        }
                    } else {
                        c.$setValidity('percent', true);
                    }
                });
            }
        }
    }])
    .directive("errMsgs", [
        function () {
            return {
                restrict: "E",
                replace: true,
                templateUrl: Utils.ServiceURI.AppUri + "Module/ErrorMsg",
                scope: {
                    field: "=?",
                    submited: "=?"
                },
                link: function ($scope, ele, attrs) {
                }
            };
        }
    ]);