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
define(["require", "exports", "App/Mappers/BaseMapper", "App/Models/Reports/ReportBuilderPropertyModel"], function (require, exports, BaseMapper, ReportBuilderPropertyModel) {
    var ReportBuilderPropertyModelMapper = /** @class */ (function (_super) {
        __extends(ReportBuilderPropertyModelMapper, _super);
        function ReportBuilderPropertyModelMapper() {
            return _super.call(this) || this;
        }
        ReportBuilderPropertyModelMapper.prototype.Map = function (source) {
            var toMap = new ReportBuilderPropertyModel();
            toMap.DisplayName = source.DisplayName;
            toMap.EntityType = source.EntityType;
            toMap.Name = source.Name;
            toMap.PropertyType = source.PropertyType;
            toMap.Children = this.MapCollection(source.Children);
            return toMap;
        };
        return ReportBuilderPropertyModelMapper;
    }(BaseMapper));
    return ReportBuilderPropertyModelMapper;
});
//# sourceMappingURL=ReportBuilderPropertyModelMapper.js.map