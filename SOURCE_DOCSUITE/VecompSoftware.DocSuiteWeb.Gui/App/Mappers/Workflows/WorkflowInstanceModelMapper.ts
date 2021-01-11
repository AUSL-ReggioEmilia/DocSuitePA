import WorkflowInstanceModel = require("App/Models/Workflows/WorkflowInstanceModel");
import IMapper = require("App/Mappers/IMapper");

class WorkflowInstanceModelMapper implements IMapper<WorkflowInstanceModel>{
    constructor() {
    }
    public Map(source: any): WorkflowInstanceModel {
        let toMap: WorkflowInstanceModel = <WorkflowInstanceModel>{};

        if (!source) {
            return null;
        }

        toMap.UniqueId = source.UniqueId;
        toMap.RegistrationDate = moment(source.RegistrationDate).format("DD/MM/YYYY");
        toMap.RegistrationUser = source.RegistrationUser;
        toMap.Name = source.WorkflowRepository.Name;
        toMap.Status = source.Status;
        toMap.Subject = source.Subject;
        toMap.WorkflowActivitiesDoneCount = source.WorkflowActivities.filter(x => x.Status === "Done").length;
        toMap.HasActivitiesInError = source.WorkflowActivities.filter(x => x.Status === "Error").length > 0;
        toMap.HasActivitiesInErrorLabel = "";
        if (toMap.HasActivitiesInError && toMap.HasActivitiesInError === true) {
            toMap.HasActivitiesInErrorLabel = "Errori presenti";
        }
        toMap.WorkflowActivitiesCount = source.WorkflowActivities.length;
        toMap.WorkflowRepository = source.WorkflowRepository;
        toMap.WorkflowActivities = source.WorkflowActivities;

        return toMap;
    }
}

export = WorkflowInstanceModelMapper;