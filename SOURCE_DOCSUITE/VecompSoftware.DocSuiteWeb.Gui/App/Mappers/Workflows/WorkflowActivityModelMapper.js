define(["require", "exports"], function (require, exports) {
    var WorkflowActivityModelMapper = /** @class */ (function () {
        function WorkflowActivityModelMapper() {
        }
        WorkflowActivityModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.ActivityType = source.ActivityType;
            toMap.Name = source.Name;
            toMap.Status = source.Status;
            toMap.Subject = source.Subject;
            toMap.UniqueId = source.UniqueId;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.Priority = source.Priority;
            toMap.Documents = source.Documents;
            toMap.WorkflowAuthorizations = source.WorkflowAuthorizations;
            toMap.WorkflowProperties = source.WorkflowProperties;
            toMap.WorkflowInstance = source.WorkflowInstance;
            toMap.DueDate = source.DueDate;
            toMap.IdArchiveChain = source.IdArchiveChain;
            toMap.ActivityAction = source.ActivityAction;
            toMap.ActivityArea = source.ActivityArea;
            if (toMap.RegistrationDate) {
                toMap.RegistrationDateFormatted = moment(source.RegistrationDate).format("DD/MM/YYYY");
            }
            toMap.WorkflowActivityLogs = source.WorkflowActivityLogs;
            return toMap;
        };
        return WorkflowActivityModelMapper;
    }());
    return WorkflowActivityModelMapper;
});
//# sourceMappingURL=WorkflowActivityModelMapper.js.map