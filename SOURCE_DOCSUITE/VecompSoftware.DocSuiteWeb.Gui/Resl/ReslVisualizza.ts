import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import SessionStorageKeysHelper = require("App/Helpers/SessionStorageKeysHelper");
import ResolutionModel = require("App/Models/Resolutions/ResolutionModel");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import UscErrorNotification = require('UserControl/uscErrorNotification');
import DocumentUnitService = require("App/Services/DocumentUnits/DocumentUnitService");
import DocumentUnitModel = require("App/Models/DocumentUnits/DocumentUnitModel");

class ReslVisualizza {

    protected static DOCUMENTUNIT_TYPE_NAME = "DocumentUnit";

    uscNotificationId: string;
    btnWorkflowId: string;
    radWindowManagerReslId: string;
    resolutionUniqueId: string;

    private _serviceConfigurations: ServiceConfiguration[];

    private _documentUnitService: DocumentUnitService;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;

        $(document).ready(() => {

        });
    }

    initialize() {
        let resolutionConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, ReslVisualizza.DOCUMENTUNIT_TYPE_NAME);
        this._documentUnitService = new DocumentUnitService(resolutionConfiguration);
    }

    btnWorkflow_OnClick(): void {
        this.setWorkflowSessionStorage();

        let url: string = `../Workflows/StartWorkflow.aspx?Type=Resl&ManagerID=${this.radWindowManagerReslId}&DSWEnvironment=Resolution&Callback=${window.location.href}`;

        this.openWindow(url, "windowStartWorkflow", 730, 550);
    }

    private setWorkflowSessionStorage(): void {
        this._documentUnitService.getDocumentUnitById(this.resolutionUniqueId,
            (data: DocumentUnitModel) => {
                sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL, JSON.stringify(data));
                sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_ID, data.UniqueId);
                sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_TITLE, `${data.Title} - ${data.Subject}`);
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    private openWindow(url, name, width, height): void {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerReslId);
        let wnd: Telerik.Web.UI.RadWindow = manager.open(url, name, null);
        wnd.setSize(width, height);
        wnd.set_modal(true);
        wnd.center();
    }

    protected showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(uscNotificationId, customMessage)
        }

    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage)
        }
    }
}

export = ReslVisualizza;