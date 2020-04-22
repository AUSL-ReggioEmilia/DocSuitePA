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
define(["require", "exports", "App/Mappers/BaseMapper", "App/Models/Commons/CategoryModel", "App/Mappers/MassimariScarto/MassimarioScartoModelMapper"], function (require, exports, BaseMapper, CategoryModel, MassimarioScartoModelMapper) {
    var CategoryModelMapper = /** @class */ (function (_super) {
        __extends(CategoryModelMapper, _super);
        function CategoryModelMapper() {
            return _super.call(this) || this;
        }
        CategoryModelMapper.prototype.Map = function (source) {
            var toMap = new CategoryModel();
            if (!source) {
                return null;
            }
            toMap.Code = source.Code;
            toMap.EntityShortId = source.EntityShortId;
            toMap.FullCode = source.FullCode;
            toMap.FullIncrementalPath = source.FullIncrementalPath;
            toMap.FullSearchComputed = source.FullSearchComputed;
            toMap.IdCategory = source.IdCategory;
            toMap.IsActive = source.IsActive;
            toMap.MassimarioScarto = source.MassimarioScarto ? new MassimarioScartoModelMapper().Map(source.MassimarioScarto) : null;
            toMap.Parent = source.Parent ? new CategoryModelMapper().Map(source.Parent) : null;
            toMap.Name = source.Name;
            toMap.UniqueId = source.UniqueId;
            toMap.StartDate = source.StartDate;
            toMap.EndDate = source.EndDate;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.LastChangedDate = source.LastChangedDate;
            toMap.LastChangedUser = source.LastChangedUser;
            return toMap;
        };
        return CategoryModelMapper;
    }(BaseMapper));
    return CategoryModelMapper;
});
//# sourceMappingURL=CategoryModelMapper.js.map