var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "../BaseMapper", "../../Helpers/RequireJSHelper", "../Commons/CategoryModelMapper"], function (require, exports, BaseMapper, RequireJSHelper, CategoryModelMapper) {
    var FascicleDocumentUnitCategoryModelMapper = /** @class */ (function (_super) {
        __extends(FascicleDocumentUnitCategoryModelMapper, _super);
        function FascicleDocumentUnitCategoryModelMapper() {
            return _super.call(this) || this;
        }
        FascicleDocumentUnitCategoryModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            var _categoryModelMapper;
            _categoryModelMapper = RequireJSHelper.getModule(CategoryModelMapper, 'App/Mappers/Commons/CategoryModelMapper');
            toMap.Category = source.Category ? _categoryModelMapper.Map(source.Category) : null;
            toMap.CategoryTitle = this.formatCategoryTitle(source.Category.FullCode) + " " + source.Category.Name;
            toMap.UniqueId = source.UniqueId;
            return toMap;
        };
        FascicleDocumentUnitCategoryModelMapper.prototype.formatCategoryTitle = function (title) {
            var formattedTitle = "";
            var titlesArray = [];
            var numbers = [];
            numbers = title.split("|");
            for (var _i = 0, numbers_1 = numbers; _i < numbers_1.length; _i++) {
                var char = numbers_1[_i];
                var number = +char; //parsing the string gets rid of the first 0 characters
                titlesArray.push(number.toString());
            }
            formattedTitle = titlesArray.join(".");
            return formattedTitle;
        };
        return FascicleDocumentUnitCategoryModelMapper;
    }(BaseMapper));
    return FascicleDocumentUnitCategoryModelMapper;
});
//# sourceMappingURL=FascicleDocumentUnitCategoryModelMapper.js.map