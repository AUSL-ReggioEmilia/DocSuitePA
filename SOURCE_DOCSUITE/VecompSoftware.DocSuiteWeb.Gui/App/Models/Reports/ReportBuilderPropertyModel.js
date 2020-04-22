define(["require", "exports"], function (require, exports) {
    var ReportBuilderPropertyModel = /** @class */ (function () {
        function ReportBuilderPropertyModel() {
            this.Children = [];
        }
        Object.defineProperty(ReportBuilderPropertyModel.prototype, "HasChildren", {
            get: function () { return this.Children.length > 0; },
            enumerable: true,
            configurable: true
        });
        return ReportBuilderPropertyModel;
    }());
    return ReportBuilderPropertyModel;
});
//# sourceMappingURL=ReportBuilderPropertyModel.js.map