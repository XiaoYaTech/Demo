marjorLeaseApp.controller("majorLeaseCreateController", [
    "$scope",
    "$http",
    "$modal",
    "$window",
    "$location",
    "$selectUser",
    "majorLeaseService",
    'storeService',
    "messager",
    "$routeParams",
    function ($scope, $http, $modal, $window, $location, $selectUser, majorLeaseService, storeService, messager, $routeParams) {

        $scope.entity = {};
        $scope.selNoticeUsers = {};
        //$scope.entity.GBDate = new Date();
        //$scope.entity.ReopenDate = new Date();
        if ($routeParams.uscode) {
            $scope.storeCode = $routeParams.uscode;
        }

        $scope.$watch("storeCode", function (val) {
            if (!!val && val.length == 7 && !isNaN(val)) {
                $scope.entity.USCode = val;
                $scope.entity.projectID = "";
                //$scope.entity.rental = false;
                //$scope.entity.redline = false;
                //$scope.entity.leaseterm = false;
                $scope.entity.CreateDate = new Date();

                storeService.getStoreBasic({ usCode: val }).$promise.then(function (data) {
                    if (data != null) {
                        if (data.ProjectContractRevision) {
                            $scope.entity.ProjectContractRevision = data.ProjectContractRevision;
                            $scope.entity.OldChangeLeaseTermExpiraryDate = data.ProjectContractRevision.LeaseChangeExpiryOld;
                            $scope.entity.OldLandlord = data.ProjectContractRevision.LandlordOld;
                            $scope.entity.OldRentalStructure = data.ProjectContractRevision.RentStructureOld;
                            $scope.entity.OldChangeRedLineRedLineArea = data.ProjectContractRevision.RedlineAreaOld;
                        }
                    }
                });
            }
        });

        $scope.$watch('entity.GBDate', function (val) {
            if (val) {
                $scope.MinReopenDate = !!$scope.entity.GBDate ? moment($scope.entity.GBDate).toDate() : null;
            }
        });

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


        $scope.now = new Date();
        $scope.formats = ['dd-MMMM-yyyy', 'yyyy/MM/dd', 'dd.MM.yyyy', 'shortDate'];
        $scope.format = $scope.formats[1];

        $scope.ShowNotifyUserModalDialog = false;
        $scope.selectNoticeUsers = function (frm) {

            if ($scope.entity.GBDate
                    && $scope.entity.ReopenDate) {
                var gbDate = moment($scope.entity.GBDate);
                var reopDate = moment($scope.entity.ReopenDate);
                if (reopDate.isBefore(gbDate)) {
                    messager.showMessage("[[[Re-open Date 不能早于 GB Date]]]", "fa-warning c_orange");
                    return false;
                }
            }

            if (!$scope.entity.ChangeRentalType
                && !$scope.entity.ChangeRedLineType
                && !$scope.entity.ChangeLeaseTermType
                && !$scope.entity.ChangeLandlordType
                && !$scope.entity.ChangeOtherType) {
                messager.showMessage("[[[请选择 Major Lease Changes Type!]]]", "fa-warning c_orange");
                return false;
            }

            if (frm.$valid) {
                $scope.ShowNotifyUserModalDialog = true;
            }

            return true;
        };
        $scope.beginNoticeUsers = function (frm) {
            //if (!$scope.entity.rental
            //    && !$scope.entity.redline
            //    && !$scope.entity.leaseterm
            //    && !$scope.entity.landlord
            //    && !$scope.entity.others) {
            //    messager.showMessage("[[[请选择 Major Lease Changes Type!]]]", "fa-warning c_orange");
            //    return false;
            //}

            //if ($scope.entity.others
            //    && !$scope.entity.ChangeOthersDESC) {
            //    messager.showMessage("请填写 Others 的描述!", "fa-warning c_orange");
            //    return false;
            //}


            //if (!$scope.entity.GBDate) {
            //    messager.showMessage("请选择GB Date!", "fa-warning c_orange");
            //    return false;
            //}
            //if (!$scope.entity.ReopenDate) {
            //    messager.showMessage("请选择Re-open Date!", "fa-warning c_orange");
            //    return false;
            //}
            if (!$scope.team.AssetRep) {
                messager.showMessage("[[[请选择Asset Rep！]]]", "fa-warning c_orange");
                return false;
            }

            if (!$scope.team.AssetActor) {
                messager.showMessage("[[[请选择Asset Actor！]]]", "fa-warning c_orange");
                return false;
            }

            if (!$scope.team.Finance) {
                messager.showMessage("[[[请选择Finance！]]]", "fa-warning c_orange");
                return false;
            }
            if (!$scope.team.PM) {
                messager.showMessage("[[[请选择PM！]]]", "fa-warning c_orange");
                return false;
            }
            if (!$scope.team.Legal) {
                messager.showMessage("[[[请选择Legal！]]]", "fa-warning c_orange");
                return false;
            }

            if (frm.$valid) {
                $scope.ShowNotifyUserModalDialog = true;
            }

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

            var legalAccount = $scope.team.Legal;
            $scope.entity.LegalAccount = legalAccount.UserAccount;
            $scope.entity.LegalNameZHCN = legalAccount.UserNameZHCN;
            $scope.entity.LegalNameENUS = legalAccount.UserNameENUS;

            var assetMgrAccount = $scope.team.AssetMgr;
            $scope.entity.AssetManagerAccount = assetMgrAccount.UserAccount;
            $scope.entity.AssetManagerNameENUS = assetMgrAccount.UserNameENUS;
            $scope.entity.AssetManagerNameZHCN = assetMgrAccount.UserNameZHCN;

            var cMAccount = $scope.team.CM;
            $scope.entity.CMAccount = cMAccount.UserAccount;
            $scope.entity.CMNameENUS = cMAccount.UserNameENUS;
            $scope.entity.CMNameZHCN = cMAccount.UserNameZHCN;

            //$scope.entity.ChangeRentalType = $scope.entity.rental;
            //if (!$scope.entity.ChangeRentalType) {
            //    $scope.entity.ChangeRentalTypeDESC = "";
            //}
            //$scope.entity.ChangeRedLineType = $scope.entity.redline;
            //if (!$scope.entity.ChangeRedLineType) {
            //    $scope.entity.ChangeRedLineTypeDESC = "";
            //}
            //$scope.entity.ChangeLeaseTermType = $scope.entity.leaseterm;
            //if (!$scope.entity.ChangeLeaseTermType) {
            //    $scope.entity.ChangeLeaseTermDESC = "";
            //}

            //$scope.entity.ChangeLandLordType = $scope.entity.landlord;
            //if (!$scope.entity.ChangeLandLordType) {
            //    $scope.entity.ChangeLandLordDESC = "";
            //}
            //$scope.entity.ChangeOthersType = $scope.entity.others;
            //if (!$scope.entity.ChangeOthersType) {
            //    $scope.entity.ChangeOthersDESC = "";
            //}

            $scope.entity.CreateUserAccount = window.currentUser.Code;
            $scope.entity.CreateUserNameZHCN = window.currentUser.NameZHCN;
            $scope.entity.CreateUserNameENUS = window.currentUser.NameENUS;

            $scope.entity.StoreNameZHCN = $scope.storeBasicInfo.StoreBasicInfo.NameZHCN;
            $scope.entity.StoreNameENUS = $scope.storeBasicInfo.StoreBasicInfo.NameENUS;

            $scope.entity.NecessaryNoticeUserList = notifyUsersInfo.NecessaryNoticeUsers;
            $scope.entity.NoticeUserList = notifyUsersInfo.NoticeUsers;
            
            majorLeaseService.beginCreateMajorLease($scope.entity).$promise.then(function (data) {
                var obj = data;
                $window.location = Utils.ServiceURI.WebAddress() + "redirect";
                //$location.path(url);
            });

        };
    }]);