import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import DocumentUnitService = require("App/Services/DocumentUnits/DocumentUnitService");
import DocumentUnitModel = require("App/Models/DocumentUnits/DocumentUnitModel");
import UscStartWorkflow = require('UserControl/uscStartWorkflow');
import AjaxModel = require("App/Models/AjaxModel");
import SessionStorageKeysHelper = require("App/Helpers/SessionStorageKeysHelper");

class ProtVisualizza {
    btnWorkflowId: string;
    radWindowManagerId: string;
    currentDocumentUnitId: string;
    ajaxLoadingPanelId: string;
    radNotificationInfoId: string;
    pnlMainContentId: string;

    private _serviceConfigurations: ServiceConfiguration[];

    private _btnWorkflow: JQuery;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel
    private _documentUnitModel: DocumentUnitModel;
    private _documentUnitService: DocumentUnitService;
    private _notificationInfo: Telerik.Web.UI.RadNotification;
    private _radWindowManager: Telerik.Web.UI.RadWindowManager;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    initialize() {
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._notificationInfo = <Telerik.Web.UI.RadNotification>$find(this.radNotificationInfoId);
        this._radWindowManager = $find(this.radWindowManagerId) as Telerik.Web.UI.RadWindowManager;

        let documentUnitConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentUnit");
        this._documentUnitService = new DocumentUnitService(documentUnitConfiguration);

        this._documentUnitService.getDocumentUnitById(this.currentDocumentUnitId, (data: DocumentUnitModel) => {
            this._documentUnitModel = data;
        });

        if (this.btnWorkflowId && this.btnWorkflowId !== "") {
            this._btnWorkflow = <JQuery>$(`#${this.btnWorkflowId}`);
            this._btnWorkflow.click(this.btnWorkflow_onClick);
            this._radWindowManager.add_close((this.onWorkflowCloseWindow));

        }
    }

    btnWorkflow_onClick = (sender: any) => {
        this.setSessionVariables();

        var url = `../Workflows/StartWorkflow.aspx?Type=Prot&ManagerID=${this.radWindowManagerId}&DSWEnvironment=Protocol&Callback${window.location.href}`;
        return this.openWindow(url, "windowStartWorkflow", 730, 550);
    }

    setSessionVariables() {
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL, JSON.stringify(this._documentUnitModel));
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_ID, this._documentUnitModel.UniqueId);
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_TITLE, `${this._documentUnitModel.Title} - ${this._documentUnitModel.Subject}`);
    }

    onWorkflowCloseWindow = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument()) {
            let result: AjaxModel = <AjaxModel>{};
            result = <AjaxModel>args.get_argument();
            this._notificationInfo.show();
            this._notificationInfo.set_text(result.ActionName);
            this._loadingPanel.show(this.pnlMainContentId);
            location.reload();
        }
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