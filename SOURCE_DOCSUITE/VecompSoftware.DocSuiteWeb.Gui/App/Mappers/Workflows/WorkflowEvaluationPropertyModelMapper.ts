import BaseMapper = require('App/Mappers/BaseMapper');
import WorkflowEvaluationProperty = require('App/Models/Workflows/WorkflowEvaluationProperty');

class WorkflowEvaluationPropertyModelMapper extends BaseMapper<WorkflowEvaluationProperty>{
    constructor() {
        super();
    }

    public Map(source): WorkflowEvaluationProperty {
        let toMap: WorkflowEvaluationProperty = <WorkflowEvaluationProperty>{};

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
    }
}
export = WorkflowEvaluationPropertyModelMapper;