rebuildApp.controller("createController", [
    "$scope",
    "$http",
    "$modal",
    "$window",
    "$location",
    "$selectUser",
    "rebuildService",
    "messager",
    "$routeParams",
    function ($scope, $http, $modal, $window, $location, $selectUser, rebuildService, messager, $routeParams) {

        $scope.entity = {};
        $scope.selNoticeUsers = {};
        $scope.entity.GBDate = new Date();
        $scope.entity.ReopenDate = null;
        $scope.entity.TempClosureDate = null;
        $scope.entity.ConstCompletionDate = new Date();
        if ($routeParams.uscode) {
            $scope.storeCode = $routeParams.uscode;
        }

        $scope.$watch("storeCode", function (val) {
            if (!!val && val.length == 7 && !isNaN(val)) {
                $scope.entity.USCode = val;
                $scope.entity.projectID = "";
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

                $scope.now = new Date();
                if ($scope.entity.TempClosureDate != null && $scope.entity.TempClosureDate != '') {
                    $scope.entity.GBDate = moment($scope.entity.TempClosureDate).toDate();
                }
                if ($scope.entity.GBDate != null && $scope.entity.GBDate != "") {
                    $scope.entity.ConstCompletionDate = moment($scope.entity.GBDate).toDate();
                }
                if ($scope.entity.ConstCompletionDate != null && $scope.entity.ConstCompletionDate != ''
                    && $scope.entity.TempClosureDate != null && $scope.entity.TempClosureDate != '') {
                    $scope.entity.ReopenDate = moment($scope.entity.ConstCompletionDate).toDate();
                }
            }
        });

        $scope.ShowNotifyUserModalDialog = false;
        $scope.beginNoticeUsers = function (frm) {

            //if (!$scope.entity.GBDate) {
            //    messager.showMessage("请选择GB Date!", "fa-warning c_orange");
            //    return false;
            //}
            //if (!$scope.entity.ReopenDate) {
            //    messager.showMessage("请选择Re-open Date!", "fa-warning c_orange");
            //    return false;
            //}
            //if (!$scope.entity.ConstCompletionDate) {
            //    messager.showMessage("请选择Construction Completion Date Date!", "fa-warning c_orange");
            //    return false;
            //}
            //if (!$scope.entity.TempClosureDate) {
            //    messager.showMessage("请选择Temp Closure Date Date!", "fa-warning c_orange");
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
            if ($scope.entity.TempClosureDate != null
                    && $scope.entity.GBDate != null
                    && $scope.entity.ConstCompletionDate != null
                    && $scope.entity.ReopenDate != null) {
                var tmpDate = moment($scope.entity.TempClosureDate);
                var gbDate = moment($scope.entity.GBDate.toDateString());
                var consDate = moment($scope.entity.ConstCompletionDate.toDateString());
                var reopDate = moment($scope.entity.ReopenDate);
                if (gbDate.isBefore(tmpDate)) {
                    messager.showMessage("[[[开工日期不能早于Temp Closure Date]]]", "fa-warning c_orange");
                    return false;
                }
                if (consDate.isBefore(gbDate)) {
                    messager.showMessage("[[[完工日期不能早于开工日期]]]", "fa-warning c_orange");
                    return false;
                }
                if (reopDate.isBefore(consDate)) {
                    messager.showMessage("[[[重开日期不能早于完工日期]]]", "fa-warning c_orange");
                    return false;
                }
            }
            else if ($scope.entity.GBDate != null
                && $scope.entity.ConstCompletionDate != null) {
                var gbDate = moment($scope.entity.GBDate);
                var consDate = moment($scope.entity.ConstCompletionDate);
                if (consDate.isBefore(gbDate)) {
                    messager.showMessage("[[[完工日期不能早于开工日期]]]", "fa-warning c_orange");
                    return false;
                }
            }
            
            if (!frm.$valid) {
                return false;
            }
            $scope.ShowNotifyUserModalDialog = true;
            return true;
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

            var assetMgrAccount = $scope.team.AssetMgr;
            $scope.entity.AssetManagerAccount = assetMgrAccount.UserAccount;
            $scope.entity.AssetManagerNameENUS = assetMgrAccount.UserNameENUS;
            $scope.entity.AssetManagerNameZHCN = assetMgrAccount.UserNameZHCN;

            var cMAccount = $scope.team.CM;
            $scope.entity.CMAccount = cMAccount.UserAccount;
            $scope.entity.CMNameENUS = cMAccount.UserNameENUS;
            $scope.entity.CMNameZHCN = cMAccount.UserNameZHCN;

            var legalAccount = $scope.team.Legal;
            $scope.entity.LegalAccount = legalAccount.UserAccount;
            $scope.entity.LegalNameZHCN = legalAccount.UserNameZHCN;
            $scope.entity.LegalNameENUS = legalAccount.UserNameENUS;

            $scope.entity.CreateUserAccount = window.currentUser.Code;
            $scope.entity.CreateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.entity.CreateUserNameENUS = window.currentUser.NameENUS;

            $scope.entity.StoreNameZHCN = $scope.storeBasicInfo.StoreBasicInfo.NameZHCN;
            $scope.entity.StoreNameENUS = $scope.storeBasicInfo.StoreBasicInfo.NameENUS;

            $scope.entity.NecessaryNoticeUserList = notifyUsersInfo.NecessaryNoticeUsers;
            $scope.entity.NoticeUserList = notifyUsersInfo.NoticeUsers;
            
            rebuildService.beginCreateRebuild($scope.entity).$promise.then(function (data) {
                var obj = data;
                $window.location = Utils.ServiceURI.WebAddress() + "redirect";
                //$location.path(url);
            });
        };
    }]);