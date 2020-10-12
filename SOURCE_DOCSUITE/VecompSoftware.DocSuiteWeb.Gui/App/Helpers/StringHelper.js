define(["require", "exports"], function (require, exports) {
    var StringHelper = /** @class */ (function () {
        function StringHelper() {
        }
        StringHelper.prototype.pad = function (number, length) {
            var str = '' + number;
            while (str.length < length) {
                str = '0' + str;
            }
            return str;
        };
        return StringHelper;
    }());
    return StringHelper;
});
//# sourceMappingURL=StringHelper.js.map