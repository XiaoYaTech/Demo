ko.bindingHandlers.foreachAsync = {
    init: function () {

    },
    update: function () {

    }
}

ko.bindingHandlers.datePicker = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        $(element).attr("readonly", "readonly").datepicker({
            format: 'yyyy-mm-dd'
        }).on("changeDate", function (val) {
            var value = valueAccessor();
            value(moment(val.date).format('YYYY-MM-DD'));
        });
    },
    update: function (element, valueAccessor) {
        var value = valueAccessor();
        var valueUnwraped = ko.unwrap(value);
        $(element).val(valueUnwraped);
    }
};


ko.bindingHandlers.DateText = {
    init: function (element, valueAccessor) {
        var value = valueAccessor();
        if (value != "" && !!moment(value).year())
            value = moment(value).format('YYYY-MM-DD');
        $(element).html(value);
    }
};

ko.bindingHandlers.NumberText = {
    init: function (element, valueAccessor) {
        var value = valueAccessor();
        if ((!!value || value == 0) && !isNaN(value)) {
            if (value > 0) {
                var amount_str = value.toString();
                var re = /\d{1,3}(?=(\d{3})+$)/g;
                value = amount_str.replace(/^(\d+)((\.\d+)?)$/, function (s, s1, s2) {
                    if (s2.length == 0) {
                        //s2 = ".00";
                    }
                    else if (s2.length > 3) {
                        s2 = s2.substr(0, 3);
                    }
                    return s1.replace(re, "$&,") + s2;
                });
            }
            else if (value < 0) {
                var amount_str = Math.abs(value).toString();
                var re = /\d{1,3}(?=(\d{3})+$)/g;
                value = amount_str.replace(/^(\d+)((\.\d+)?)$/, function (s, s1, s2) {
                    if (s2.length == 0) {
                        //s2 = ".00";
                    }
                    else if (s2.length > 3) {
                        s2 = s2.substr(0, 3);
                    }
                    return "-" + s1.replace(re, "$&,") + s2;
                });
            }
            else if (value == 0) {
                return "0";
            }
        } else {
            value = "";
        }
        $(element).html(value);
    }
};