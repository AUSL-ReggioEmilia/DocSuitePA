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
    var CategoryFascicleViewModelMapper = /** @class */ (function (_super) {
        __extends(CategoryFascicleViewModelMapper, _super);
        function CategoryFascicleViewModelMapper() {
            return _super.call(this) || this;
        }
        CategoryFascicleViewModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.FascicleType = source.FascicleType;
            toMap.Environment = source.DSWEnvironment;
            toMap.PeriodDays = source.FasciclePeriod ? source.FasciclePeriod.PeriodDays : null;
            toMap.PeriodName = source.FasciclePeriod ? source.FasciclePeriod.PeriodName : null;
            toMap.PeriodUniqueId = source.FasciclePeriod ? source.FasciclePeriod.PeriodUniqueId : null;
            toMap.CategoryId = source.Category ? source.Category.EntityShortId : null;
            toMap.ManagerId = source.Manager ? source.Manager.UniqueId : null;
            toMap.ManagerName = source.Manager ? source.Manager.Description : null;
            return toMap;
        };
        return CategoryFascicleViewModelMapper;
    }(BaseMapper));
    return CategoryFascicleViewModelMapper;
});
//# sourceMappingURL=CategoryFascicleViewModelMapper.js.map