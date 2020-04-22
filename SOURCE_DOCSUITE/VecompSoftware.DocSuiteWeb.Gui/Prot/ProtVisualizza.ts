import ServiceConfigurationHelper = require("../App/Helpers/ServiceConfigurationHelper");
import ServiceConfiguration = require("../App/Services/ServiceConfiguration");
import DocumentUnitService = require("../App/Services/DocumentUnits/DocumentUnitService");
import DocumentUnitModel = require("../App/Models/DocumentUnits/DocumentUnitModel");

class ProtVisualizza {
    btnWorkflowId: string;
    radWindowManagerId: string;
    currentId: string;

    private _btnWorkflow: JQuery;

    private _serviceConfigurations: ServiceConfiguration[];
    private _documentUnitModel: DocumentUnitModel;
    private _documentUnitService: DocumentUnitService;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    initialize() {
        this._btnWorkflow = <JQuery>$(`#${this.btnWorkflowId}`);
        this._btnWorkflow.click(this.btnWorkflow_OnClick);

        let documentUnitConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentUnit");
        this._documentUnitService = new DocumentUnitService(documentUnitConfiguration);

        this._documentUnitService.getDocumentUnitById(this.currentId, (data: DocumentUnitModel) => {
            this._documentUnitModel = data;
        });
    }

    btnWorkflow_OnClick = (sender: any) => {
        this.setSessionVariables();

        var url = `../Workflows/StartWorkflow.aspx?Type=Prot&ManagerID=${this.radWindowManagerId}&DSWEnvironment=Protocol&Callback${window.location.href}`;
        return this.openWindow(url, "windowStartWorkflow", 730, 550);
    }

    setSessionVariables() {
        sessionStorage.setItem("ReferenceModel", JSON.stringify(this._documentUnitModel));
        sessionStorage.setItem("ReferenceId", this._documentUnitModel.UniqueId);
        sessionStorage.setItem("ReferenceTitle", `${this._documentUnitModel.Title}- ${this._documentUnitModel.Subject}`);
    }

    openWindow(url, name, width, height): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        let wnd: Telerik.Web.UI.RadWindow = manager.open(url, name, null);
        wnd.setSize(width, height);
        wnd.set_modal(true);
        wnd.center();
        return false;
    }

}

export = ProtVisualizza;