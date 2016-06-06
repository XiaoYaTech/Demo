reimageApp.controller("reimageCreateController", [
    "$scope",
    '$window',
    '$location',
    "reimageService",
    'messager',
    '$routeParams',
    function ($scope, $window, $location, reimageService, messager, $routeParams) {
        $scope.entity = {};
        $scope.ShowNotifyUserModalDialog = false;
        $scope.now = new Date();
        if ($routeParams.uscode) {
            $scope.storeCode = $routeParams.uscode;
        }
        //$scope.getMinReopenDate = function() {
        //    return moment($scope.entity.GBDate).add('days', 1).toDate();
        //}
        $scope.beginNoticeUsers = function (frm) {
            if ($scope.entity.GBDate
                    && $scope.entity.ReopenDate) {
                var gbDate = moment($scope.entity.GBDate);
                var reopDate = moment($scope.entity.ReopenDate);
                if (reopDate.isBefore(gbDate)) {
                    messager.showMessage("[[[Re-open Date 不能早于 GB Date]]]", "fa-warning c_orange");
                    return false;
                }
            }
            if (frm.$valid) {

                $scope.ShowNotifyUserModalDialog = true;
            }
            //if (!$scope.entity.GBDate) {
            //    messager.showMessage("请填写GBDate！", "fa-warning c_orange");
            //    return false;
            //}

            //if (!$scope.entity.ReopenDate) {
            //    messager.showMessage("请填写Re-open Date！", "fa-warning c_orange");
            //    return false;
            //}

            //if (!$scope.team.AssetRep) {
            //    messager.showMessage("[[[请选择Asset Rep！]]]", "fa-warning c_orange");
            //    return false;
            //}

            //if (!$scope.team.AssetActor) {
            //    messager.showMessage("[[[请选择Asset Actor！]]]", "fa-warning c_orange");
            //    return false;
            //}

            //if (!$scope.team.Finance) {
            //    messager.showMessage("[[[请选择Finance！]]]", "fa-warning c_orange");
            //    return false;
            //}
            //if (!$scope.team.PM) {
            //    messager.showMessage("[[[请选择PM！]]]", "fa-warning c_orange");
            //    return false;
            //}
            //if (!$scope.team.Legal) {
            //    messager.showMessage("[[[请选择Legal！]]]", "fa-warning c_orange");
            //    return false;
            //}



            //$scope.ShowNotifyUserModalDialog = true;
            //return true;
        };

        $scope.submit = function (notifyUsersInfo) {
            var assetRepAccount = $scope.team.AssetRep;
            $scope.entity.AssetRepAccount = assetRepAccount.UserAccount;
            $scope.entity.AssetRepNameZHCN = assetRepAccount.UserNameZHCN;
            $scope.entity.AssetRepNameENUS = assetRepAccount.UserNameENUS;

            var assetActorAccount = $scope.team.AssetActor;
            $scope.entity.AssetActorAccount = assetActorAccount.UserAccount;
            $scope.entity.AssetActorNameZHCN = assetActorAccount.UserNameZHCN;
            $scope.entity.AssetActorNameENUS = assetActorAccount.UserNameENUS;

            var financeAccount = $scope.team.Finance;
            $scope.entity.FinanceAccount = financeAccount.UserAccount;
            $scope.entity.FinanceNameZHCN = financeAccount.UserNameZHCN;
            $scope.entity.FinanceNameENUS = financeAccount.UserNameENUS;

            var pMAccount = $scope.team.PM;
            $scope.entity.PMAccount = pMAccount.UserAccount;
            $scope.entity.PMNameZHCN = pMAccount.UserNameZHCN;
            $scope.entity.PMNameENUS = pMAccount.UserNameENUS;

            //var legalAccount = $scope.team.Legal;
            $scope.entity.LegalAccount = "";
            $scope.entity.LegalNameZHCN = "";
            $scope.entity.LegalNameENUS = "";

            var assetMgrAccount = $scope.team.AssetMgr;
            $scope.entity.AssetManagerAccount = assetMgrAccount.UserAccount;
            $scope.entity.AssetManagerNameENUS = assetMgrAccount.UserNameENUS;
            $scope.entity.AssetManagerNameZHCN = assetMgrAccount.UserNameZHCN;

            var cMAccount = $scope.team.CM;
            $scope.entity.CMAccount = cMAccount.UserAccount;
            $scope.entity.CMNameENUS = cMAccount.UserNameENUS;
            $scope.entity.CMNameZHCN = cMAccount.UserNameZHCN;

            $scope.entity.NecessaryNoticeUserList = notifyUsersInfo.NecessaryNoticeUsers;
            $scope.entity.NoticeUserList = notifyUsersInfo.NoticeUsers;

            $scope.entity.CreateUserAccount = window.currentUser.Code;
            $scope.entity.CreateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.entity.CreateUserNameENUS = window.currentUser.NameENUS;

            $scope.entity.StoreNameZHCN = $scope.storeBasicInfo.StoreBasicInfo.NameZHCN;
            $scope.entity.StoreNameENUS = $scope.storeBasicInfo.StoreBasicInfo.NameENUS;
            
            reimageService.createProject($scope.entity).$promise.then(function (data) {
                $window.location = Utils.ServiceURI.WebAddress() + "redirect";
            });

        };

        $scope.$watch("storeCode", function (val) {
            if (!!val && val.length == 7 && !isNaN(val)) {
                $scope.step1Finished = true;
                $scope.entity.USCode = val;
                $scope.entity.ProjectId = "";
                $scope.entity.CreateDate = new Date();
                //初始化日期控件
                $scope.today = function () {
                    $scope.dt = new Date();
                };
                $scope.today();

                $scope.clear = function () {
                    $scope.dt = null;
                };

                $scope.open = function ($event, ele) {
                    $event.preventDefault();
                    $event.stopPropagation();

                    $scope[ele] = true;
                };

                $scope.dateOptions = {
                    formatYear: 'yy',
                    startingDay: 1
                };

                $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
                $scope.format = $scope.formats[1];

            } else {
                $scope.step1Finished = false;
            }
        });
    }
]);