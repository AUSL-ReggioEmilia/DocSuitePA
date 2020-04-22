import IMapper = require('App/Mappers/IMapper');
import WorkflowRoleModel = require('App/Models/Workflows/WorkflowRoleModel');

class WorkflowRoleModelMapper implements IMapper<WorkflowRoleModel>{
    constructor() {
    }

    public Map(source: any): WorkflowRoleModel {
        let toMap = <WorkflowRoleModel>{};
        toMap.EmailAddress = source.EmailAddress;
        toMap.Name = source.Name;
        toMap.TenantId = source.TenantId;
        toMap.IdRole = source.EntityShortId ? source.EntityShortId : source.IdRole;
        toMap.UniqueId = source.UniqueId;
        return toMap;
    }
}

export = WorkflowRoleModelMapper