import WorkflowInstanceLogViewModel = require('App/ViewModels/Workflows/WorkflowInstanceLogViewModel');
import BaseMapper = require('App/Mappers/BaseMapper');
import EnumHelper = require('App/Helpers/EnumHelper');

class WorkflowInstanceLogViewModelMapper extends BaseMapper<WorkflowInstanceLogViewModel>{

    private _enumHelper: EnumHelper;

    constructor() {
        super();
        this._enumHelper = new EnumHelper();
    }

    public Map(source): WorkflowInstanceLogViewModel {
        let toMap: WorkflowInstanceLogViewModel = <WorkflowInstanceLogViewModel>{};

        if (!source) {
            return null;
        }

        toMap.Computer = source.SystemComputer;
        toMap.Description = source.LogDescription;
        toMap.TypeDescription = this._enumHelper.getWorkflowInstanceLogDescription(source.LogType);
        toMap.LogDate = moment(source.RegistrationDate).format("L").concat(" ").concat(moment(source.RegistrationDate).format("LTS")); 
        toMap.LogUser = source.RegistrationUser;

        return toMap;        
    }
}
export = WorkflowInstanceLogViewModelMapper;