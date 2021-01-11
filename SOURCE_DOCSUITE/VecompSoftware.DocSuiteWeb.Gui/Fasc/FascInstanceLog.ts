import FascBase = require('Fasc/FascBase');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import WorkflowInstanceLogService = require('App/Services/Workflows/WorkflowInstancelogService')
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import WorkflowInstanceLogViewModel = require('App/ViewModels/Workflows/WorkflowInstanceLogViewModel');
import UscWorkflowInstanceLog = require('UserControl/uscWorkflowInstanceLog')

class FascInstanceLog extends FascBase {

    IdFascicle: string;
    uscWorkflowInstanceLogsId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _serviceConfigurationHelper: ServiceConfigurationHelper;
    private _workflowInstanceLogService: WorkflowInstanceLogService;

    /**
     * Costruttore
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascBase.FASCICLE_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {
        });
    }

    /**
     * inizializzazione
     */
    initialize() {
        super.initialize();
        let workflowInstanceLogServiceCongiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowInstanceLog");
        this._workflowInstanceLogService = new WorkflowInstanceLogService(workflowInstanceLogServiceCongiguration);

        this.loadData(0);

        $("#".concat(this.uscWorkflowInstanceLogsId)).on(UscWorkflowInstanceLog.ON_PAGE_CHANGE, (args, data) => {
            this.loadData(data);
        });
    }

    /**
     * carico i dati nello usercontrol
     * @param skip
     */
    loadData(skip: number) {
        this._workflowInstanceLogService.getFascicleInstanceLogs(this.IdFascicle, skip, 30,
            (data: ODATAResponseModel<WorkflowInstanceLogViewModel>) => {
                if (data) {
                    let uscWorkflowInstanceLog: UscWorkflowInstanceLog = <UscWorkflowInstanceLog>$("#".concat(this.uscWorkflowInstanceLogsId)).data();
                    if (!jQuery.isEmptyObject(uscWorkflowInstanceLog)) {
                        uscWorkflowInstanceLog.setGrid(data);
                    }
                }
            }
        )

    }
}

export = FascInstanceLog;