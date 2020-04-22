define(["require", "exports"], function (require, exports) {
    var WorkflowRoleModelMapper = /** @class */ (function () {
        function WorkflowRoleModelMapper() {
        }
        WorkflowRoleModelMapper.prototype.Map = function (source) {
            var toMap = {};
            toMap.EmailAddress = source.EmailAddress;
            toMap.Name = source.Name;
            toMap.TenantId = source.TenantId;
            toMap.IdRole = source.EntityShortId ? source.EntityShortId : source.IdRole;
            toMap.UniqueId = source.UniqueId;
            return toMap;
        };
        return WorkflowRoleModelMapper;
    }());
    return WorkflowRoleModelMapper;
});
//# sourceMappingURL=WorkflowRoleModelMapper.js.map