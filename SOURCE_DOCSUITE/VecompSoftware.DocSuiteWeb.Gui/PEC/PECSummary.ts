import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import SessionStorageKeysHelper = require("App/Helpers/SessionStorageKeysHelper");
import PECMailService = require("App/Services/PECMails/PECMailService");
import PECMailViewModel = require("App/ViewModels/PECMails/PECMailViewModel");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import UscErrorNotification = require('UserControl/uscErrorNotification');

class PECSummary {
    uscNotificationId: string;
    btnWorkflowId: string;
    radWindowManagerPecId: string;
    pecMailId: number;

    private _pecMailService: PECMailService;
    private _serviceConfigurations: ServiceConfiguration[];

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    initialize() {
        let pecMailServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "PECMail");
        this._pecMailService = new PECMailService(pecMailServiceConfiguration);
    }

    btnWorkflow_onClick(): void {
        this.setWorkflowSessionStorage();
        let url: string = `../Workflows/StartWorkflow.aspx?ManagerID=${this.radWindowManagerPecId}&DSWEnvironment=PECMail&Callback=${window.location.href}`;
        this.openWindow(url, "windowStartWorkflow", 730, 550);
    }

    private setWorkflowSessionStorage(): void {
        this._pecMailService.getPECMailById(this.pecMailId,
            (data: PECMailViewModel) => {
                sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_MODEL, JSON.stringify(data));
                sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_ID, data.UniqueId);
                sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_REFERENCE_TITLE, data.MailSubject);
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    private openWindow(url, name, width, height): void {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerPecId);
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

export = PECSummary;