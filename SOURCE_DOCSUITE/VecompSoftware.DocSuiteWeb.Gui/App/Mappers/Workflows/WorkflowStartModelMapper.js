define(["require", "exports"], function (require, exports) {
    var WorkflowStartMapper = /** @class */ (function () {
        function WorkflowStartMapper() {
        }
        WorkflowStartMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.WorkflowName = source.WorkflowName;
            toMap.Arguments = {};
            for (var argument in source.Arguments) {
                toMap.Arguments[argument] = source.Arguments[argument];
            }
            toMap.StartParameters = {};
            for (var argument in source.StartParameters) {
                toMap.StartParameters[argument] = source.StartParameters[argument];
            }
            return toMap;
        };
        return WorkflowStartMapper;
    }());
    return WorkflowStartMapper;
});
//# sourceMappingURL=WorkflowStartModelMapper.js.map