define(["require", "exports"], function (require, exports) {
    var NullSafe = /** @class */ (function () {
        function NullSafe() {
        }
        NullSafe.Do = function (func, defaultValue) {
            try {
                var result = func();
                if (result == undefined || result == null) {
                    return defaultValue;
                }
                return result;
            }
            catch (e) {
                return defaultValue;
            }
        };
        return NullSafe;
    }());
    return NullSafe;
});
//# sourceMappingURL=NullSafe.js.map