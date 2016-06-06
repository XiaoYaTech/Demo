!function () {
    function DropDownAction(params, componentInfo, viewModel) {
        var $trigger = $(".dropdown-box-trigger", componentInfo.element);
        var $box = $(".dropdown-box", componentInfo.element);
        var $boxDialog = $(".dropdown-box-dialog", componentInfo.element);
        var direction = params.Direction || 'left';
        if (!!params.Width) {
            $boxDialog.width(params.Width);
        }
        $boxDialog.click(function (e) {
            e.stopPropagation();
        });
        $boxDialog.on({
            "dropdown.box.show": function () {
                var alignPos = $box.offset();
                if ($boxDialog.is(":animated")) {
                    $boxDialog.stop();
                };
                if (!!params.Judgement) {
                    var $judgement = $box.parents(params.Judgement);
                    if (!!$judgement.offset())
                        direction = $judgement.offset().left + $judgement.outerWidth() - (alignPos.left + $box.outerWidth()) > $boxDialog.outerWidth() ? "left" : "right";
                }
                $(".dropdown-box-dialog").not($boxDialog).trigger("dropdown.box.hide");
                switch (direction) {
                    case "left":
                        $boxDialog.css({
                            left: -$boxDialog.outerWidth(),
                            opacity: 0
                        }).show().animate({
                            left: 0,
                            opacity: 1
                        }, function () {
                            viewModel.showBox();
                        });
                        break;
                    case "right":
                        $boxDialog.css({
                            left: $(document).outerWidth(),
                            opacity: 0
                        }).show().animate({
                            left: $box.outerWidth() - $boxDialog.outerWidth(),
                            opacity: 1
                        }, function () {
                            viewModel.showBox();
                        });
                        break;
                    default:
                        break;
                }
            },
            "dropdown.box.hide": function () {
                if ($boxDialog.is(":animated")) {
                    $boxDialog.stop();
                };
                $boxDialog.hide();
                viewModel.hideBox();
            }
        });
        $trigger.click(function (e) {
            if ($boxDialog.is(":visible")) {
                $boxDialog.trigger("dropdown.box.hide");
            } else {
                $boxDialog.trigger("dropdown.box.show");
            }
            e.preventDefault();
            e.stopPropagation();
        });
    };
    $(document).click(function (e) {
        $(".dropdown-box-dialog").trigger("dropdown.box.hide");
    });
    ko.components.register("dropdown-buttons", {
        viewModel: {
            createViewModel: function (params, componentInfo) {
                var viewModel = new (function () {
                    var self = this;
                    self.Label = params.Label;
                    self.Buttons = params.Buttons;
                    self.Width = params.Width;
                    self.showBox = function () {

                    };

                    self.hideBox = function () {

                    };
                })();
                DropDownAction.call(this, params, componentInfo, viewModel);
                return viewModel;
            }
        },
        template: { element: 'tpl-dropdown-buttons' }
    });
    ko.components.register("dropdown-list", {
        viewModel: {
            createViewModel: function (params, componentInfo) {
                var viewModel = new (function () {
                    var self = this;
                    self.Label = params.Label;
                    self.TextField = params.TextField;
                    self.Width = params.Width;
                    self.List = params.List;
                    self.OnItemSelected = function (item) {
                        $(".dropdown-box-dialog", componentInfo.element).trigger("dropdown.box.hide");
                        params.OnItemSelected && params.OnItemSelected(item);
                    }
                    self.showBox = function () {

                    };

                    self.hideBox = function () {

                    };
                })();
                DropDownAction.call(this, params, componentInfo, viewModel);
                return viewModel;
            }
        },
        template: { element: 'tpl-dropdown-list' }
    });
    ko.components.register("dropdown-comboxes", {
        viewModel: {
            createViewModel: function (params, componentInfo) {
                var viewModel = new (function () {
                    var self = this;
                    self.Label = params.Label;
                    self.Loading = ko.observable(true);
                    self.fiterText = ko.observable(null);
                    self.fiterText.subscribe(function (val) {
                        val = (val || "").toLowerCase();
                        ko.utils.arrayForEach(self.Fields(), function (f, i) {
                            if (f[self.TextField].toLowerCase().indexOf(val) >= 0) {
                                f.Filtered(true);
                            } else {
                                f.Filtered(false);
                            }
                        });
                    });
                    self.TextField = params.TextField;
                    self.Width = params.Width;
                    self.Api = params.Api;
                    self.Fields = params.Fields;
                    self.SelectAll = function () {
                        ko.utils.arrayForEach(self.Fields(), function (f, i) {
                            f.Checked(true);
                        });
                    };
                    self.ClearAll = function () {
                        ko.utils.arrayForEach(self.Fields(), function (f, i) {
                            f.Checked(false);
                        });
                    };
                    self.showBox = function () {
                    };

                    self.hideBox = function () {
                    };
                })();
                DropDownAction.call(this, params, componentInfo, viewModel);
                return viewModel;
            }
        },
        template: { element: 'tpl-dropdown-comboxes' }
    });
    ko.components.register("dropdown-string", {
        viewModel: {
            createViewModel: function (params, componentInfo) {
                var ajaxQueue = params.AjaxQueue || new AjaxQueue("am-dropdown-string");
                var viewModel = new (function () {
                    var self = this;
                    self.Field = params.Field;
                    self.Label = self.Field.FieldDispENUS;
                    self.fiterText = ko.observable(ko.toJS(self.Field.ConditionText));
                    self.Width = params.Width;
                    self.Update = params.Update;
                    self.UpdateSearch = function () {
                        self.Update(self.Field.ID, self.fiterText());
                        //self.Field.ConditionText(self.fiterText());
                    }
                    self.showBox = function () {

                    };

                    self.hideBox = function () {
                    };
                })();
                DropDownAction.call(this, params, componentInfo, viewModel);
                return viewModel;
            }
        },
        template: { element: "tpl-dropdown-string" }
    });
    ko.components.register("dropdown-bool", {
        viewModel: {
            createViewModel: function (params, componentInfo) {
                var ajaxQueue = params.AjaxQueue || new AjaxQueue("am-dropdown-bool");
                var viewModel = new (function () {
                    var self = this;
                    self.Field = params.Field;
                    self.Label = self.Field.FieldDispENUS;
                    self.IsCheck = ko.observable(ko.toJS(self.Field.ConditionText));
                    self.Width = params.Width;
                    self.Update = params.Update;
                    self.UpdateSearch = function () {
                        self.Update(self.Field.ID, self.IsCheck());
                        //self.Field.ConditionText(self.fiterText());
                    }
                    self.showBox = function () {

                    };

                    self.hideBox = function () {
                    };
                })();
                DropDownAction.call(this, params, componentInfo, viewModel);
                return viewModel;
            }
        },
        template: { element: "tpl-dropdown-bool" }
    });
    ko.components.register("dropdown-dictionary", {
        viewModel: {
            createViewModel: function (params, componentInfo) {
                var ajaxQueue = params.AjaxQueue || new AjaxQueue("am-dropdown-string");
                var viewModel = new (function () {
                    var self = this;
                    self.Field = params.Field;
                    self.Label = self.Field.FieldDispENUS;
                    self.Loading = ko.observable(false);
                    self.Update = params.Update;
                    self.fiterText = ko.observable();
                    self.fiterText.subscribe(function (val) {
                        val = (val || "").toLowerCase();
                        var vallist = val.split(',');
                        ko.utils.arrayForEach(self.Fields(), function (f, i) {
                            if (ko.utils.arrayFilter(vallist, function (v) {
                                return f.Text.toLowerCase().indexOf(v) >= 0;
                            }).length > 0) {
                                f.Filtered(true);
                                f.Checked(true);
                            } else {
                                f.Filtered(false);
                                f.Checked(false);
                            }
                        });
                    });
                    self.Width = params.Width;
                    self.Api = params.Api;
                    self.Fields = ko.observableArray([]);
                    self.SelectAll = function () {
                        ko.utils.arrayForEach(self.Fields(), function (f, i) {
                            f.Checked(true);
                        });
                    };
                    self.ClearAll = function () {
                        ko.utils.arrayForEach(self.Fields(), function (f, i) {
                            f.Checked(false);
                        });
                    };
                    self.InvertAll = function () {
                        ko.utils.arrayForEach(self.Fields(), function (f, i) {
                            f.Checked(!f.Checked());
                        });
                    };
                    self.UpdateSearch = function () {
                        var selects = ko.utils.arrayFilter(self.Fields(), function (f) { return f.Checked() });
                        var selectvalues = new Array();
                        if (selects.length != self.Fields().length) {
                            ko.utils.arrayForEach(selects, function (s) {
                                selectvalues.push(s.Text);
                            });
                        }
                        self.Update(self.Field.ID, selectvalues.join(","));
                        //self.Field.ConditionText(selectvalues.join(","));
                    }
                    self.showBox = function () {
                        if (!self.loaded) {
                            self.Loading(true);
                            ajaxQueue.Request("report", {
                                url: Utils.ServiceURI.Address() + self.Api,
                                contentType: "application/json",
                                success: function (data) {
                                    var results = ko.utils.arrayMap(data || [], function (d, i) {
                                        var r = {};
                                        r.Checked = ko.observable(true);
                                        r.Filtered = ko.observable(true);
                                        r.Text = d;
                                        return r;
                                    });
                                    self.Fields(results);
                                    self.fiterText(ko.toJS(self.Field.ConditionText));
                                    self.Loading(false);
                                    self.loaded = true;
                                }
                            }).Run();
                        }
                    };

                    self.hideBox = function () {
                    };
                })();
                DropDownAction.call(this, params, componentInfo, viewModel);
                return viewModel;
            }
        },
        template: { element: "tpl-dropdown-dictionary" }
    });
    ko.components.register("dropdown-date-range", {
        viewModel: {
            createViewModel: function (params, componentInfo) {
                var viewModel = new (function () {
                    var self = this;
                    self.Field = params.Field;
                    self.Label = self.Field.FieldDispENUS;
                    self.Update = params.Update;
                    self.StartDate = ko.observable("");
                    self.EndDate = ko.observable("");
                    var ct = ko.toJS(self.Field.ConditionText);
                    if (ct) {
                        self.StartDate(ct.split('|')[0].split(':')[1] || "");
                        self.EndDate(ct.split('|')[1].split(':')[1] || "");
                    }
                    self.clearStart = function () {
                        self.StartDate("");
                    }
                    self.clearEnd = function () {
                        self.EndDate("");
                    }
                    self.Text = ko.computed(function () {
                        var result = "";
                        if (self.StartDate() && !!moment(self.StartDate()).year()) {
                            result += "StartDate:" + moment(self.StartDate()).format('YYYY-MM-DD');
                        }
                        else {
                            result += "StartDate:";
                        }
                        if (self.EndDate() && !!moment(self.EndDate()).year()) {
                            result += "|" + "EndDate:" + moment(self.EndDate()).format('YYYY-MM-DD');
                        }
                        else {
                            result += "|" + "EndDate:";
                        }
                        return result;
                    });
                    self.UpdateSearch = function () {
                        self.Update(self.Field.ID, self.Text());
                        //self.Field.ConditionText(self.Text());
                    }
                    self.showBox = function () {

                    };

                    self.hideBox = function () {

                    };
                })();
                DropDownAction.call(this, params, componentInfo, viewModel);
                return viewModel;
            }
        },
        template: { element: 'tpl-dropdown-date-range' }
    });
    ko.components.register("dropdown-number", {
        viewModel: {
            createViewModel: function (params, componentInfo) {
                var viewModel = new (function () {
                    var self = this;
                    self.Field = params.Field;
                    self.Label = self.Field.FieldDispENUS;
                    self.Update = params.Update;
                    self.LessValue = ko.observable("");
                    self.GreatValue = ko.observable("");
                    var ct = ko.toJS(self.Field.ConditionText);
                    if (ct) {
                        self.LessValue(ct.split('|')[0].split(':')[1] || "");
                        self.GreatValue(ct.split('|')[1].split(':')[1] || "");
                    }
                    self.Text = ko.computed(function () {
                        var result = "";
                        if (self.LessValue() && !isNaN(self.LessValue())) {
                            result += "LessValue:" + self.LessValue();
                        }
                        else {
                            result += "LessValue:";
                        }
                        if (self.GreatValue() && !isNaN(self.GreatValue())) {
                            result += "|" + "GreatValue:" + self.GreatValue();
                        }
                        else {
                            result += "|" + "GreatValue:";
                        }
                        return result;
                    });
                    self.UpdateSearch = function () {
                        self.Update(self.Field.ID, self.Text());
                        //self.Field.ConditionText(self.Text());
                    }
                    self.showBox = function () {

                    };

                    self.hideBox = function () {

                    };
                })();
                DropDownAction.call(this, params, componentInfo, viewModel);
                return viewModel;
            }
        },
        template: { element: "tpl-dropdown-number" }
    });
    ko.components.register("am-datagrid", {
        viewModel: {
            createViewModel: function (params, componentInfo) {
                var ajaxQueue = params.AjaxQueue || new AjaxQueue("am-datagrid");
                var viewModel = new (function () {
                    var self = this;
                    self.Fields = params.Fields;
                    self.pageSize = params.PageSize;
                    self.pageIndex = params.PageIndex;
                    self.lockedTables = params.LockedTables;
                    self.unLockedTables = params.UnLockedTables;
                    self.lockedDatas = params.LockedDatas;
                    self.unlockedDatas = params.UnLockedDatas;
                    self.totalItems = params.TotalItems;
                    self.isLoading = params.IsLoading;
                    self.loadData = params.LoadData;
                })();
                return viewModel;
            }
        },
        template: { element: 'tpl-am-datagrid' }
    });
    ko.components.register("pager", {
        viewModel: {
            createViewModel: function (params, componentInfo) {
                var model = new (function () {
                    var self = this;
                    self.pageSize = params.PageSize;
                    self.pageIndex = params.PageIndex;
                    self.totalItems = params.TotalItems;
                    self.numPages = params.NumPages;
                    self.totalPages = ko.computed(function () {
                        return Math.ceil(self.totalItems() / self.pageSize());
                    });
                    self.pages = ko.computed(function () {
                        var result = [], pagerCount = self.numPages, start = 0, end = pagerCount;
                        if (self.totalPages() < pagerCount) {
                            for (var i = start; i <= self.totalPages() - 1; i++) {
                                result.push(i);
                            };
                            return result;
                        }
                        else {
                            if (self.pageIndex() <= Math.floor(pagerCount / 2)) {
                                end = pagerCount - 1;
                            }
                            else if (self.pageIndex() >= self.totalPages() - 1 - Math.floor(pagerCount / 2)) {
                                end = self.totalPages() - 1;
                                start = end - pagerCount + 1;
                            }
                            else {
                                start = self.pageIndex() - Math.floor(pagerCount / 2);
                                if (pagerCount % 2 == 0)
                                    end = self.pageIndex() + Math.floor(pagerCount / 2) - 1;
                                else
                                    end = self.pageIndex() + Math.floor(pagerCount / 2);
                            }
                            for (var i = start; i <= end; i++) {
                                result.push(i);
                            };
                            return result;
                        }
                    });
                    self.selectPage = function (page) {
                        self.pageIndex(page);
                    };
                    self.noPrevious = function () {
                        return self.pageIndex() <= 0;
                    }
                    self.noNext = function () {
                        return self.pageIndex() >= self.totalPages() - 1;
                    }
                    self.selectPage(0);
                })();
                return model;
            }
        },
        template: { element: "tpl-pager" }
    });
}();