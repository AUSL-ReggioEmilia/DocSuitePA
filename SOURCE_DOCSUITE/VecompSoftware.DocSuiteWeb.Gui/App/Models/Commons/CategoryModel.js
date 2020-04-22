define(["require", "exports"], function (require, exports) {
    var CategoryModel = /** @class */ (function () {
        function CategoryModel() {
        }
        /**
         * Formatta il FullCode del classificatore
         */
        CategoryModel.prototype.getFullCodeDotted = function () {
            var codes = new Array();
            var splittedCodes = this.FullCode.split("|");
            $.each(splittedCodes, function (index, code) {
                codes.push(Number(code).toString());
            });
            var fullCodeDotted = codes.join(".");
            return fullCodeDotted;
        };
        return CategoryModel;
    }());
    return CategoryModel;
});
//# sourceMappingURL=CategoryModel.js.map