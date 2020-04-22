import BaseMapper = require('App/Mappers/BaseMapper');
import WorkflowProperty = require('App/Models/Workflows/WorkflowProperty');

class WorkflowPropertyModelMapper extends BaseMapper<WorkflowProperty>{
    constructor() {
        super();
    }

    public Map(source): WorkflowProperty {
        let toMap: WorkflowProperty = <WorkflowProperty>{};

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
    }
}
export = WorkflowPropertyModelMapper;