//WorkflowRepositoryModelMapper
import BaseMapper = require('App/Mappers/BaseMapper');
import WorkflowRepositoryModel = require('App/Models/Workflows/WorkflowRepositoryModel')

class WorkflowRepositoryModelMapper extends BaseMapper<WorkflowRepositoryModel>{
  constructor() {
    super();
  }

  public Map(source: any): WorkflowRepositoryModel {
    let toMap = <WorkflowRepositoryModel>{};

    if (!source) {
      return null;
    }

    toMap.UniqueId = source.UniqueId;
    toMap.Name = source.Name;
    toMap.Version = source.Version;
    toMap.ActiveFrom = source.Activefrom;
    toMap.ActiveTo = source.ActiveTo;
    toMap.Xaml = source.Xaml;
    toMap.Json = source.Json;
    toMap.CustomActivities = source.CustomActivities;
    toMap.Status = source.Status;
    toMap.DSWEnvironment = source.DSWEnvironment;
    toMap.WorkflowRoleMappings = source.WorkflowRoleMappings;
    toMap.Roles = source.Roles;

    return toMap;
  }
}

export = WorkflowRepositoryModelMapper