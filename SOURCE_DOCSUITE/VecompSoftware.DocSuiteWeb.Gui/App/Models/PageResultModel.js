define(["require", "exports"], function (require, exports) {
    var PageResultModel = /** @class */ (function () {
        function PageResultModel(model) {
            this.items = model.Items['$values'];
            this.count = model.Count;
        }
        return PageResultModel;
    }());
    return PageResultModel;
});
//# sourceMappingURL=PageResultModel.js.map