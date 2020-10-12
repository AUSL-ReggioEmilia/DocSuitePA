define(["require", "exports"], function (require, exports) {
    var WorkflowInstanceModelMapper = /** @class */ (function () {
        function WorkflowInstanceModelMapper() {
        }
        WorkflowInstanceModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.UniqueId = source.UniqueId;
            toMap.RegistrationDate = moment(source.RegistrationDate).format("DD/MM/YYYY");
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.Name = source.WorkflowRepository.Name;
            toMap.Status = source.Status;
            toMap.Subject = source.Subject;
            toMap.WorkflowActivitiesDoneCount = source.WorkflowActivities.filter(function (x) { return x.Status === "Done"; }).length;
            toMap.HasActivitiesInError = source.WorkflowActivities.filter(function (x) { return x.Status === "Error"; }).length > 0;
            toMap.HasActivitiesInErrorLabel = "";
            if (toMap.HasActivitiesInError && toMap.HasActivitiesInError === true) {
                toMap.HasActivitiesInErrorLabel = "Errori presenti";
            }
            toMap.WorkflowActivitiesCount = source.WorkflowActivities.length;
            toMap.WorkflowRepository = source.WorkflowRepository;
            toMap.WorkflowActivities = source.WorkflowActivities;
            return toMap;
        };
        return WorkflowInstanceModelMapper;
    }());
    return WorkflowInstanceModelMapper;
});
//# sourceMappingURL=WorkflowInstanceModelMapper.js.map