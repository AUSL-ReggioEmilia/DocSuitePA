define(function () {
    String.isNullOrEmpty = function (value) {
        return value == null || value == '';
    };
    String.format = function (value) {
        var params = [];
        for (var _i = 1; _i < arguments.length; _i++) {
            params[_i - 1] = arguments[_i];
        }
        return value.replace(/{(\d+)}/g, function (match, number) {
            return typeof params[number] != 'undefined' ? params[number] : match;
        });
    };
});
//# sourceMappingURL=String.js.map