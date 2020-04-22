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
    var WorkflowRepositoryModelMapper = /** @class */ (function (_super) {
        __extends(WorkflowRepositoryModelMapper, _super);
        function WorkflowRepositoryModelMapper() {
            return _super.call(this) || this;
        }
        WorkflowRepositoryModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.Name = source.Name;
            toMap.Version = source.Version;
            toMap.ActiveFrom = source.Activefrom;
            toMap.ActiveTo = source.ActiveTo;
            toMap.Xaml = source.Xaml;
            toMap.Json = source.Json;
            toMap.CustomActivities = source.CustomActivities;
            toMap.Status = source.Status;
            toMap.DSWEnvironment = source.DSWEnvironment;
            toMap.WorkflowRoleMappings = source.WorkflowRoleMappings;
            return toMap;
        };
        return WorkflowRepositoryModelMapper;
    }(BaseMapper));
    return WorkflowRepositoryModelMapper;
});
//# sourceMappingURL=WorkflowRepositoryModelMapper.js.map