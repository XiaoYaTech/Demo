dictionaryApp.controller('consInvtCheckingProcessController', [
    '$scope',
    "$http",
    "$routeParams",
    "$modal",
    "$window",
    'closureCreateHandler',
    "messager",
    '$location',
    "redirectService",
function ($scope, $http, $routeParams, $modal, $window, closureCreateHandler, messager, $location, redirectService) {

    var procInstID = $routeParams.ProcInstID;
    var sn = $routeParams.SN;
    $scope.projectId = $routeParams.projectId;
    $scope.entity = {};
    $scope.entity.ProjectId = $scope.projectId;
    $scope.checkPointRefresh = true;
    $scope.flowCode = "Closure_ConsInvtChecking";

    var userAccount = window.currentUser.Code;

    var k2ApiUrl = Utils.ServiceURI.Address() + "api/ClosureConsInvtChecking/GetK2Status/" + userAccount + "/" + sn + "/" + procInstID;


    $http.get(k2ApiUrl).success(function (data) {

        if (data.Status == "Process") {

            loadConsInvtData();
        } else if (data.Status == "Edit") {
            var url = "/Closure/ConsInvtChecking/Process/Edit/param";
            $location.path(url);
        }
    });

    loadConsInvtData();
    $scope.ApproverSubmit = function (action) {

        $scope.entity.SN = sn;
        $scope.entity.Action = action;
        $scope.entity.ProcInstID = procInstID;


        $scope.entity.UserAccount = userAccount;
        $scope.entity.UserNameENUS = window.currentUser.NameENUS;
        $scope.entity.UserNameZHCN = window.currentUser.NameZHCN;
        if (action == "Return" && !$scope.entity.Comments) {
            messager.showMessage("[[[请填写意见]]]", "fa-warning c_orange");
            return false;
        }

        messager.confirm(action == "Return" ? "[[[确定要进行退回吗？]]]" : "[[[确定要进行审批吗？]]]", "fa-warning c_orange").then(function (result) {
            if (result) {
                messager.blockUI("[[[正在处理中，请稍等...]]]");
                $http.post(Utils.ServiceURI.Address() + "api/ClosureConsInvtChecking/ProcessClosureConsInvtChecking/", $scope.entity).success(function (successData) {
                    messager.showMessage(Utils.Common.resolveAlertMsg(action), "fa-check c_green").then(function () {
                        //$window.location = Utils.ServiceURI.WebAddress() + "redirect";
                        messager.unBlockUI();
                        redirectService.flowRedirect($scope.flowCode, $scope.projectId);
                    });
                }).error(function (err) {
                    messager.showMessage(Utils.Common.resolveAlertMsg(action, true), "fa-warning c_orange");
                });
            }
        });
    }

    function loadConsInvtData() {

        $http.get(Utils.ServiceURI.Address() + "api/ClosureConsInvtChecking/GetByProjectId/" + $scope.projectId).success(function (data) {

            $scope.entity = data;


        }).then(function (data) {
            if ($scope.entity != "null") {
                $http.get(Utils.ServiceURI.Address() + "api/ClosureConsInvtChecking/GetTemplates/" + $scope.entity.Id.toString()).success(function (atts) {
                    $scope.templateList = atts;
                    for (var i = 0; i < atts.length; i++) {
                        atts[i].IsAvailable = (i == atts.length - 1) ? "[[[生效]]]" : "[[[失效]]]";
                    }

                });
            };
        });
    }

    $scope.$watch("(entity.RECostActual - entity.RECostBudget) / entity.RECostBudget", function (newval, oldval) {
        if (!isNaN(newval) && newval != oldval) {
            var temp = $scope.entity.RECostActual - $scope.entity.RECostBudget;
            if ($scope.entity.RECostBudget != 0)
                $scope.recostVar = temp / $scope.entity.RECostBudget;
        }
    });

    $scope.$watch("(entity.LHIActual - entity.LHIBudget)  / entity.LHIBudget", function (newval, oldval) {
        if (!isNaN(newval) && newval != oldval) {
            var temp = $scope.entity.LHIActual - $scope.entity.LHIBudget;
            if ($scope.entity.LHIBudget != 0)
                $scope.recostVar2 = temp / $scope.entity.LHIBudget;
        }
    });

    $scope.$watch("(entity.EquipmentActual + entity.SignageActual + entity.SeatingActual + entity.DecorationActual - entity.ESSDBudget) / entity.ESSDBudget", function (newval, oldval) {
        if (!isNaN(newval) && newval != oldval) {
            var temp = $scope.entity.EquipmentActual + $scope.entity.SignageActual + $scope.entity.SeatingActual + $scope.entity.DecorationActual - $scope.entity.ESSDBudget;
            if ($scope.entity.ESSDBudget != 0)
                $scope.recostVar3 = temp / $scope.entity.ESSDBudget;
        }
    });

    $scope.$watch("(entity.TotalActual -entity.TotalWriteoffBudget) / entity.TotalWriteoffBudget", function (newval, oldval) {
        if (!isNaN(newval) && newval != oldval) {
            var temp = $scope.entity.TotalActual - $scope.entity.TotalWriteoffBudget;
            if ($scope.entity.TotalWriteoffBudget != 0)
                $scope.recostVar4 = temp / $scope.entity.TotalWriteoffBudget;
        }
    });

}]);

