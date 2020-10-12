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
define(["require", "exports", "../BaseMapper"], function (require, exports, BaseMapper) {
    var TenantAOOModelMapper = /** @class */ (function (_super) {
        __extends(TenantAOOModelMapper, _super);
        function TenantAOOModelMapper() {
            return _super.call(this) || this;
        }
        TenantAOOModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.Name = source.Name;
            toMap.Note = source.Note;
            toMap.CategorySuffix = source.CategorySuffix;
            toMap.Tenants = source.Tenants;
            toMap.TenantTypology = source.TenantTypology;
            return toMap;
        };
        return TenantAOOModelMapper;
    }(BaseMapper));
    return TenantAOOModelMapper;
});
//# sourceMappingURL=TenantAOOModelMapper.js.map