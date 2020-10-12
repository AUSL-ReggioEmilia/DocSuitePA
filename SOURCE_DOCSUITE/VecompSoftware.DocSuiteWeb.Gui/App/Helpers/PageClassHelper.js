define(["require", "exports"], function (require, exports) {
    var PageClassHelper = /** @class */ (function () {
        function PageClassHelper() {
        }
        PageClassHelper.callUserControlFunctionSafe = function (controlId) {
            var promise = $.Deferred();
            var controlInstance = $("#" + controlId).data();
            if (jQuery.isEmptyObject(controlInstance)) {
                $("#" + controlId).on(this.LOADED_EVENT, function () {
                    return promise.resolve(controlInstance);
                });
                return promise.promise();
            }
            return promise.resolve(controlInstance);
        };
        PageClassHelper.LOADED_EVENT = "onLoaded";
        return PageClassHelper;
    }());
    return PageClassHelper;
});
//# sourceMappingURL=PageClassHelper.js.map