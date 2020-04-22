define(["require", "exports"], function (require, exports) {
    var Guid = /** @class */ (function () {
        function Guid() {
        }
        Guid.newGuid = function () {
            return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
        };
        Object.defineProperty(Guid, "empty", {
            get: function () {
                return "00000000-0000-0000-0000-000000000000";
            },
            enumerable: true,
            configurable: true
        });
        return Guid;
    }());
    return Guid;
});
//# sourceMappingURL=GuidHelper.js.map