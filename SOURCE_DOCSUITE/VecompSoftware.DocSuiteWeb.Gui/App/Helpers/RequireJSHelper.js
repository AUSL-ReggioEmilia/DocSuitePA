define(["require", "exports"], function (require, exports) {
    var RequireJSHelper = /** @class */ (function () {
        function RequireJSHelper() {
        }
        RequireJSHelper.getModule = function (testType, className) {
            var moduleJS;
            if (requirejs.defined(className)) {
                moduleJS = new requirejs.s.contexts._.defined[className]();
            }
            else {
                moduleJS = new testType();
            }
            return moduleJS;
        };
        return RequireJSHelper;
    }());
    return RequireJSHelper;
});
//# sourceMappingURL=RequireJSHelper.js.map