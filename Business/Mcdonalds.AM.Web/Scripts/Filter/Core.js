angular.module("mcd.am.filters", []).filter("boolFilter", function () {
    return function (input) {
        return input ? "Yes" : "No";
    };
}).filter("closureNatureFilter", function () {
    return function (input) {
        var dict = ["Permanent", "Temporary"];
        return dict[input - 1];
    };
}).filter("closureNatureZHCNFilter", function () {
    return function (input) {
        var dict = ["永久", "临时"];
        return dict[input - 1];
    };
}).filter("projectStatusFilter", function () {
    return function (input) {
        var dict = ["Active", "", "Finished", "Pending", "Rejected", "Completed", "Recall", 'Killed'];
        return dict[input];
    };
}).filter("holdStatusFilter", function () {
    return function (input) {
        var dict = ['UnKnown', 'Yes', 'No'];
        return dict[input];
    };
}).filter("taskTypeFilter", function () {
    return function (input) {
        var taskTypes = ['未知', '任务', '审批', '退回', '撤回', '拒绝'];
        return taskTypes[input];
    };
}).filter("fileIconFilter", function () {
    return function (ext) {
        switch (ext) {
            case ".xlsx":
            case ".xls":
                return "fa fa-file-excel-o c_green";
            case ".ppt":
                return "fa fa-file-powerpoint-o c_red";
            case ".doc":
            case ".docx":
                return "fa fa-file-word-o c_blue";
            default:
                return "fa fa-file c_orange";
        }
    }
}).filter("numberDisplay", ["$filter", function ($filter) {
    return function (val, showType) {
        var result = val;
        if (!isNaN(val)) {
            switch (showType) {
                case "money":
                    result = $filter("money")(val);
                    break;
                case "percent":
                    result = Utils.caculator.multiply(val, 100).toFixed(1);
                    break;
                default:
                    if (!isNaN(val)) {
                        var round = Number(!!showType ? showType : 2);
                        if (isNaN(round)) {
                            round = 2;
                        }
                        result = $filter("numberCustom")(val, round);
                    }
                    break;
            }
        }
        else
            result = null;
        return result;
    };

}]).filter('numberCustom', ['$filter', function ($filter) {
    return function (input, size) {
        var result = null;
        try {
            if (Utils.Common.isInt(input)) {
                result = input;
            } else {
                result = $filter('number')(input, size);
            }
        } catch (e) {
        }
        return result;
    };
}]).filter("shortContent", function () {
    return function (input, size) {
        return input.length > size ? (input.substr(0, size) + "...") : input;
    }
}).filter('money', ['$filter', function ($filter) {
    return function (input) {
        //if (input && input.toString().indexOf('.') > 0 && input.toString().split('.')[1].length > 2) {
        //    return $filter('number')(input, 2);
        //}
        //return $filter('number')(input);

        if (input && input.toString().indexOf('.') > 0) {
            var result = $filter('number')(input, 0);
            if (result.toString() == "-0")
                return 0;
            else
                return $filter('number')(input, 0);
        }
        return $filter('number')(input);
    };
}]);