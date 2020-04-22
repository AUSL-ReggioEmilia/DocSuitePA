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
    var WorkflowEvaluationPropertyModelMapper = /** @class */ (function (_super) {
        __extends(WorkflowEvaluationPropertyModelMapper, _super);
        function WorkflowEvaluationPropertyModelMapper() {
            return _super.call(this) || this;
        }
        WorkflowEvaluationPropertyModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.Name = source.Name;
            toMap.PropertyType = source.PropertyType;
            toMap.ValueBoolean = source.ValueBoolean;
            toMap.ValueDate = source.ValueDate;
            toMap.ValueDouble = source.ValueDouble;
            toMap.ValueGuid = source.ValueGuid;
            toMap.ValueInt = source.ValueInt;
            toMap.ValueString = source.ValueString;
            toMap.WorkflowType = source.WorkflowType;
            return toMap;
        };
        return WorkflowEvaluationPropertyModelMapper;
    }(BaseMapper));
    return WorkflowEvaluationPropertyModelMapper;
});
//# sourceMappingURL=WorkflowEvaluationPropertyModelMapper.js.map