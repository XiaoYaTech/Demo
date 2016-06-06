(function () {
    var ajaxQueue = new AjaxQueue("reportAjax");
    var viewModel = function () {
        var self = this;
        self.isLoading = ko.observable(false);
        self.LockedTables = ko.observableArray([]);
        self.UnLockedTables = ko.observableArray([]);
        self.LockedDatas = ko.observableArray([]);
        self.UnLockedDatas = ko.observableArray([]);
        self.loadData = function () {
            self.BlockUI();
            self.isLoading(true);
            //self.FillDataFields();          
            ajaxQueue.Request("report", {
                url: Utils.ServiceURI.Address() + "api/report/getData",
                type: "POST",
                data: {
                    PageIndex: self.pageIndex(),
                    PageSize: self.pageSize()
                },
                success: function (result) {
                    self.LockedTables(result.LockedTables);
                    self.UnLockedTables(result.UnLockedTables);
                    self.LockedDatas(result.LockedDatas);
                    self.UnLockedDatas(result.UnLockedDatas);
                    self.totalItems(result.TotalItems);
                    self.isLoading(false);
                    self.UnBlockUI();
                }
            }).Run();
        };
        self.load = function () {
            self.loadData();
        }
        self.dataSource = ko.observableArray([]);
        self.totalItems = ko.observable(0);
        self.pageSize = ko.observable(10);
        self.pageIndex = ko.observable(0);
        self.pageIndex.subscribe(function (val) {
            if (!!val || val == 0) {
                if (!self.isLoading())
                    self.loadData();
            }
        });
        self.message = ko.observable();
        self.OpenMessage = function (message) {
            self.message(message);
            $("#modalMessage").modal({
                backdrop: 'static'
            });
        }
        self.CloseMessage = function () {
            $("#modalMessage").modal('hide');
        }

        self.UpdateCondition = function (fieldId, ConditionText) {
            if (ConditionText.length > 2000) {
                self.OpenMessage("所选条件太多，请重新选择!");
            }
            self.BlockUI();
            self.isLoading(true);
            self.pageIndex(0);
            ajaxQueue.Request("report", {
                url: Utils.ServiceURI.Address() + "api/report/UpdateCondition",
                data: {
                    FieldId: fieldId,
                    ConditionText: ConditionText,
                    PageSize: self.pageSize()
                },
                cache: false,
                success: function (result) {
                    self.LockedTables(result.LockedTables);
                    self.UnLockedTables(result.UnLockedTables);
                    self.LockedDatas(result.LockedDatas);
                    self.UnLockedDatas(result.UnLockedDatas);
                    self.totalItems(result.TotalItems);
                    self.isLoading(false);
                    self.UnBlockUI();
                }
            }).Run();
        }
        self.BlockUI = function () {
            $("#btnLoading", parent.document).click();
        }
        self.UnBlockUI = function () {
            $("#btnUnLoading", parent.document).click();
        }
    };
    var vm = new viewModel();
    ko.applyBindings(vm, document.body);
})();