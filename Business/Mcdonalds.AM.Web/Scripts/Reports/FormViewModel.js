(function () {
    var ajaxQueue = new AjaxQueue("reportAjax");
    var viewModel = function () {
        var self = this;
        var timeout = null;//异步加载数据延迟对象
        self.showTooltip = ko.observable(true);
        self.toolTip = ko.computed(function () {
            var now = new Date();
            var datestring = "";
            if (now.getHours() >= 5)
                datestring = moment(now).format("YYYY-MM-DD");
            else
                datestring = moment(now.setDate(now.getDate() - 1)).format("YYYY-MM-DD");
            return "¤报表数据截止到" + datestring + " 05:00:00"; 
        });
        self.closeTooltip = function () {
            self.showTooltip(false);
        };
        self.Templates = ko.observableArray([]);
        self.Tables = ko.observableArray([]);
        self.TemplateName = ko.observable("");
        self.message = ko.observable("");
        self.messageType = ko.observable(1);
        self.selectflow = ko.observable("");
        self.selectflow.subscribe(function (val) {
            var st = ko.utils.arrayFilter(self.Tables(), function (r) { return r.TableType == 2 });
            $.each(st, function (i, t) {
                if (t.ID == val)
                    t.Checked(true);
                else
                    t.Checked(false);
            });
        });
        self.loadTemplate = function () {
            ajaxQueue.Request("report", {
                url: Utils.ServiceURI.Address() + "api/report/initPage",
                success: function (data) {
                    ko.utils.arrayForEach(data.Templates, function (l, i) {
                        if (l.IsCommon) {
                            l.TName = '*' + l.TName;
                        }
                    });
                    self.Templates(data.Templates);
                    self.UnBlockUI();
                }
            }).Run();
        }
        self.Save = function () {
            self.BlockUI();
            var selecttables = ko.toJS(ko.utils.arrayFilter(self.Tables(), function (t) { return t.Checked() }));
            ajaxQueue.Request("report", {
                url: Utils.ServiceURI.Address() + "api/report/SaveTemplate",
                type: "POST",
                data: {
                    TemplateId: self.template().ID,
                    TemplateName: "",
                    Tables: selecttables
                },
                success: function (result) {
                    if (result.status) {
                        self.OpenMessage("[[[保存成功]]]", 1);
                        self.loadTemplate();
                        self.UnBlockUI();
                    }
                    else {
                        self.UnBlockUI();
                        self.OpenMessage("操作失败：" + result.message, 2);
                    }
                }
            }).Run();
        };
        self.SaveAs = function () {
            self.TemplateName("");
            $("#modalTemplate").modal({
                backdrop: 'static'
            });
        };
        self.SaveTemplate = function () {
            if (self.TemplateName() == "") {
                self.OpenMessage("请填写模板名称", 2);
            }
            else {
                self.BlockUI();
                var selecttables = ko.toJS(ko.utils.arrayFilter(self.Tables(), function (t) { return t.Checked() }));
                ajaxQueue.Request("report", {
                    url: Utils.ServiceURI.Address() + "api/report/SaveTemplate",
                    type: "POST",
                    data: {
                        TemplateId: 0,
                        TemplateName: self.TemplateName(),
                        Tables: selecttables
                    },
                    success: function (result) {
                        if (result.status) {
                            self.OpenMessage("[[[保存成功]]]", 1);
                            self.loadTemplate();
                            self.UnBlockUI();
                        }
                        else {
                            self.OpenMessage("操作失败：" + result.message, 2);
                        }
                    }
                }).Run();
            }
        }

        self.Delete = function () {
            self.BlockUI();
            ajaxQueue.Request("report", {
                url: Utils.ServiceURI.Address() + "api/report/RemoveTemplate/" + self.template().ID,
                type: "POST",
                success: function (result) {
                    if (result.status) {
                        self.OpenMessage("[[[操作成功]]]", 1);
                        self.loadTemplate();
                        self.template(null);
                        document.frames["frmData"].location.reload();
                        self.UnBlockUI();
                    }
                    else {
                        self.OpenMessage("操作失败：" + result.message, 2);
                    }
                }
            }).Run();
        };

        self.Search = function () {
            if (!!self.template()) {
                self.BlockUI();
                var selecttables = ko.toJS(ko.utils.arrayFilter(self.Tables(), function (t) { return t.Checked() || t.DispENUS == "Basic Info" }));
                ajaxQueue.Request("report", {
                    url: Utils.ServiceURI.Address() + "api/report/UpdateSearch",
                    type: "POST",
                    data: {
                        TemplateId: self.template().ID,
                        TemplateName: "",
                        Tables: selecttables
                    },
                    success: function (result) {
                        $("#btnLoad", document.frames['frmData'].document).click();
                    }
                }).Run();
            }
            else {
                self.OpenMessage("请先选择模板", 2);
            }
        }

        self.Export = function () {
            if (self.Tables().length == 0) {
                self.OpenMessage("请先查询数据后在进行导出", 2);
            }
            else {
                $form = $("#formExport");
                $form.attr("action", Utils.ServiceURI.Address() + "api/report/ExportData");
                $form.submit();
            }
        }

        self.template = ko.observable();
        self.SelectTemplate = function (template) {
            self.template(template);
            self.BlockUI();
            ajaxQueue.Request("report", {
                url: Utils.ServiceURI.Address() + "api/report/getTemplateDetails",
                data: {
                    templateID: template.ID
                },
                success: function (data) {
                    ko.utils.arrayForEach(data || [], function (t, i) {
                        ko.utils.arrayForEach(t.Fields || [], function (f, j) {
                            f.Checked = ko.observable(f.Checked);
                            f.Filtered = ko.observable(true);
                        });
                        t.Fields = ko.observableArray(t.Fields);
                        t.Checked = ko.observable(t.Checked);
                        if (t.TableType === 2 && t.Checked()) {
                            self.selectflow(t.ID);
                            //t.Checked.subscribe(function (val) {
                            //    if (val === true) {
                            //        ko.utils.arrayForEach(self.Tables() || [], function (tt, j) {
                            //            if (tt.TableType === 2 && tt !== t) {
                            //                tt.Checked(false);
                            //            }
                            //        });
                            //    } else {
                            //        if (ko.utils.arrayFilter(self.Tables() || [], function (tt) { return tt.TableType === 2 && tt.Checked() }).length == 0) {
                            //            self.OpenMessage("必须要选择一种流程类型", 2);
                            //            t.Checked(true);
                            //        }
                            //    }
                            //});
                        }
                        t.Checked.subscribe(function (val) {
                            if (val == true) {
                                ko.utils.arrayForEach(t.Fields(), function (tf, j) {
                                    tf.Checked(true);
                                });
                            }
                            else {
                                ko.utils.arrayForEach(t.Fields(), function (tf, j) {
                                    tf.Checked(false);
                                });
                            }
                        });
                    });
                    self.Tables(data);
                    self.UnBlockUI();
                }
            }).Run();
        }

        self.CloseTemplate = function () {
            $("#modalTemplate").modal('hide');
        }
        self.OpenMessage = function (message, messageType) {
            self.message(message);
            self.messageType(messageType);
            $("#modalMessage").modal({
                backdrop: 'static'
            });
        }
        self.CloseMessage = function () {
            $("#modalTemplate").modal('hide');
            $("#modalMessage").modal('hide');
        }
        self.BlockUI = function () {
            $("#modalBlock").modal({
                backdrop: 'static'
            });
        }
        self.UnBlockUI = function () {
            $("#modalBlock").modal('hide');
        }

        self.BlockUI();
        self.loadTemplate();
    }
    var vm = new viewModel();
    ko.applyBindings(vm, document.body);

})();