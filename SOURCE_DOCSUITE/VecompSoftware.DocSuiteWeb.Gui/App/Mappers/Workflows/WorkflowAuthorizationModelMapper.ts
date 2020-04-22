import WorkflowAuthorizationModel = require('App/Models/Workflows/WorkflowAuthorizationModel');
import BaseMapper = require('App/Mappers/BaseMapper');

class WorkflowAuthorizationModelMapper extends BaseMapper<WorkflowAuthorizationModel>{
    constructor() {
        super();
    }

    public Map(source: any) {
        let toMap: WorkflowAuthorizationModel = <WorkflowAuthorizationModel>{};
        if (!source) {
            return null;
        }
        toMap.Account = source.Account;
        toMap.UniqueId = source.UniqueId
        toMap.IsHandler = source.IsHandler

        return toMap;
    }
}
export = WorkflowAuthorizationModelMapper;