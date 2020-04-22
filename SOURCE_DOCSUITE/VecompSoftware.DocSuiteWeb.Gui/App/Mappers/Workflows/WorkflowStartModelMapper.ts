import WorkflowStartModel = require('App/Models/Workflows/WorkflowStartModel');
import WorkflowArgumentModel = require('App/Models/Workflows/WorkflowArgumentModel');
import IMapper = require('App/Mappers/IMapper');

class WorkflowStartMapper implements IMapper<WorkflowStartModel>{
     constructor() {       
    }

    public Map(source: any): WorkflowStartModel {
        let toMap: WorkflowStartModel = <WorkflowStartModel>{};

        if (!source) {
            return null;
        }

        toMap.WorkflowName = source.WorkflowName;
        toMap.Arguments = {};
        for (let argument in source.Arguments) {
            toMap.Arguments[argument] = source.Arguments[argument];
        }

        toMap.StartParameters = {};
        for (let argument in source.StartParameters) {
            toMap.StartParameters[argument] = source.StartParameters[argument];
        }

        return toMap;
    }
}

export = WorkflowStartMapper;