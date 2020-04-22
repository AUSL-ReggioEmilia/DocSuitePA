define(["require", "exports"], function (require, exports) {
    var GenericHelper = /** @class */ (function () {
        function GenericHelper() {
        }
        GenericHelper.getUrlParams = function (URL, name) {
            if (!URL)
                URL = window.location.href;
            name = name.replace(/[\[\]]/g, '\\$&');
            var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)');
            var results = regex.exec(location.search);
            return results === null ? '' : decodeURIComponent(results[2].replace(/\+/g, ' '));
        };
        return GenericHelper;
    }());
    return GenericHelper;
});
//# sourceMappingURL=GenericHelper.js.map