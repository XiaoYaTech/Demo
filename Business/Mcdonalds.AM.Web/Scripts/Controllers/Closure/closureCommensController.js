dictionaryApp.controller('closureCommensController', [
    '$scope',
    "$http",
    "$modal",
    "$window",
    '$routeParams',
    'closureCreateHandler',
    "messager",
    function ($scope, $http, $modal, $window, $routeParams, closureCreateHandler, messager) {

        $scope.projectId =  $routeParams.projectId;

        var templateUpload_dataurl = $("#testUpload").attr("data-url");
        $("#testUpload").attr("data-url", templateUpload_dataurl + $scope.projectId);

       

  

 
        $scope.ActorSubmit = function() {
            $http.post(Utils.ServiceURI.Address() + "api/ClosureExecutiveSummary/PostClosureExecutiveSummary", $scope.entity).success(function (data) {

                $window.location = Utils.ServiceURI.SiteRootURL() + "redirect";
            }).error(function (data) {
                messager.showMessage("[[[提交失败]]]", "fa-warning c_orange");
            });
        };
        $scope.loadExecutiveSummary = function (projectId) {
            $http.get(Utils.ServiceURI.Address() + "api/ClosureExecutiveSummary/GetByProjectId/" + projectId).success(function (data) {
                $scope.entity = data;
              

            }).then(function(data) {
                $http.get(Utils.ServiceURI.Address() + "api/ClosureExecutiveSummary/GetTemplates/" + $scope.entity.Id.toString()).success(function (atts) {
                    $scope.templateList = atts;
                    for (var i = 0; i < atts.length; i++) {
                        atts[i].IsAvailable = (i == atts.length - 1) ? "[[[生效]]]" : "[[[失效]]]";
                    }


                    showWoInfo();
                });
            });
        };
        
    }]);