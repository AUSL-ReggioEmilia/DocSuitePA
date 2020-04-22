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
define(["require", "exports", "App/Mappers/BaseMapper", "App/Models/Reports/ReportBuilderConditionModel"], function (require, exports, BaseMapper, ReportBuilderConditionModel) {
    var ReportBuilderConditionModelMapper = /** @class */ (function (_super) {
        __extends(ReportBuilderConditionModelMapper, _super);
        function ReportBuilderConditionModelMapper() {
            return _super.call(this) || this;
        }
        ReportBuilderConditionModelMapper.prototype.Map = function (source) {
            var toMap = new ReportBuilderConditionModel();
            toMap.ConditionName = source.ConditionName;
            return toMap;
        };
        return ReportBuilderConditionModelMapper;
    }(BaseMapper));
    return ReportBuilderConditionModelMapper;
});
//# sourceMappingURL=ReportBuilderConditionModelMapper.js.map