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
define(["require", "exports", "App/Mappers/BaseMapper"], function (require, exports, BaseMapper) {
    var CategoryTreeViewModelMapper = /** @class */ (function (_super) {
        __extends(CategoryTreeViewModelMapper, _super);
        function CategoryTreeViewModelMapper() {
            return _super.call(this) || this;
        }
        CategoryTreeViewModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.FullCode = source.FullCode;
            toMap.Code = source.Code;
            toMap.FullIncrementalPath = source.FullIncrementalPath;
            toMap.IdCategory = source.IdCategory;
            toMap.IdParent = source.IdParent;
            toMap.Name = source.Name;
            toMap.UniqueId = source.UniqueId;
            toMap.HasChildren = source.HasChildren;
            toMap.HasFascicleDefinition = source.HasFascicleDefinition;
            toMap.CategoryType = source.CategoryType;
            return toMap;
        };
        return CategoryTreeViewModelMapper;
    }(BaseMapper));
    return CategoryTreeViewModelMapper;
});
//# sourceMappingURL=CategoryTreeViewModelMapper.js.map