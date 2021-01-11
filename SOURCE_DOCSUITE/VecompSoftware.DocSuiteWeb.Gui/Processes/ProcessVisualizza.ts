import uscProcessDetails = require("UserControl/uscProcessDetails");
import ProcessNodeType = require("App/Models/Processes/ProcessNodeType");
import ServiceConfiguration = require("App/Services/ServiceConfiguration");

class ProcessVisualizza {
    public uscProcessDetailsId: string;
    public processId: string;

    private _uscProcessDetails: uscProcessDetails;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
    }

    public initialize(): void {
        this._initializeControls();
        this._showProcessDetails();
    }

    private _initializeControls(): void {
        this._uscProcessDetails = <uscProcessDetails>$(`#${this.uscProcessDetailsId}`).data();
    }

    private _showProcessDetails(): void {
        this._uscProcessDetails.clearProcessDetails();
        $(`#${this._uscProcessDetails.pnlDetailsId}`).show();
        this._uscProcessDetails.setPanelVisibility(uscProcessDetails.InformationDetails_PanelName, true);
        this._uscProcessDetails.setPanelVisibility(uscProcessDetails.CategoryInformationDetails_PanelName, false);
        this._uscProcessDetails.setPanelVisibility(uscProcessDetails.WorkflowDetails_PanelName, false);
        this._uscProcessDetails.setPanelVisibility(uscProcessDetails.FascicleDetails_PanelName, false);
        this._uscProcessDetails.setPanelVisibility(uscProcessDetails.RoleDetails_PanelName, true);

        uscProcessDetails.selectedProcessId = this.processId;
        uscProcessDetails.selectedEntityType = ProcessNodeType.Process;
        this._uscProcessDetails.setProcessDetails('', true);
    }
}

export = ProcessVisualizza;