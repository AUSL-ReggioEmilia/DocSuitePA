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
define(["require", "exports", "App/Mappers/BaseMapper", "./RoleModelMapper"], function (require, exports, BaseMapper, RoleModelMapper) {
    var CategoryFascicleRightViewModelMapper = /** @class */ (function (_super) {
        __extends(CategoryFascicleRightViewModelMapper, _super);
        function CategoryFascicleRightViewModelMapper() {
            return _super.call(this) || this;
        }
        CategoryFascicleRightViewModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.Role = source.Role ? new RoleModelMapper().Map(source.Role) : null;
            return toMap;
        };
        return CategoryFascicleRightViewModelMapper;
    }(BaseMapper));
    return CategoryFascicleRightViewModelMapper;
});
//# sourceMappingURL=CategoryFascicleRightViewModelMapper.js.map