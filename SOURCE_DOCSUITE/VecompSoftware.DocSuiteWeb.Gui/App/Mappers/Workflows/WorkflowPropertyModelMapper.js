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
    var WorkflowPropertyModelMapper = /** @class */ (function (_super) {
        __extends(WorkflowPropertyModelMapper, _super);
        function WorkflowPropertyModelMapper() {
            return _super.call(this) || this;
        }
        WorkflowPropertyModelMapper.prototype.Map = function (source) {
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
            toMap.WorkflowActivity = source.WorkflowActivity;
            return toMap;
        };
        return WorkflowPropertyModelMapper;
    }(BaseMapper));
    return WorkflowPropertyModelMapper;
});
//# sourceMappingURL=WorkflowPropertyModelMapper.js.map