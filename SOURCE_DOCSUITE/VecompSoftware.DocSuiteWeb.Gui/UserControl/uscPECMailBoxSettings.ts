import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import TbltPECMailBoxBase = require('Tblt/TbltPECMailBoxBase');
import PECMailBoxModel = require("App/Models/PECMails/PECMailBoxModel");
import PECMailBoxService = require('App/Services/PECMails/PECMailBoxService');
import PECMailBoxConfigurationService = require('App/Services/PECMails/PECMailBoxConfigurationService');
import LocationService = require('App/Services/Commons/LocationService');
import JeepServiceHostService = require('App/Services/JeepServiceHosts/JeepServiceHostService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import AjaxModel = require('App/Models/AjaxModel');
import LocationViewModel = require('App/ViewModels/Commons/LocationViewModel');
import PECMailBoxConfigurationViewModel = require('App/ViewModels/PECMails/PECMailBoxConfigurationViewModel');
import JeepServiceHostViewModel = require('App/ViewModels/JeepServiceHosts/JeepServiceHostViewModel');
import Guid = require('App/Helpers/GuidHelper');

declare var Page_ClientValidate: any;
declare var ValidatorEnable: any;
declare var GetRadWindowManager: any;
class uscPECMailBoxSettings extends TbltPECMailBoxBase {
    private _serviceConfigurations: ServiceConfiguration[];
    protected pecMailBoxService: PECMailBoxService;
    protected pecMailBoxConfigurationService: PECMailBoxConfigurationService;
    protected locationService: LocationService;
    protected jeepServiceHostService: JeepServiceHostService;

    txtMailBoxNameId: string;
    txtUsernameId: string;
    txtPasswordId: string;
    txtPasswordRequireValidatorId: string;
    chkIsInteropId: string;
    chkIsProtocolId: string;
    chkIsPublicProtocolId: string;
    ddlLocationId: string;
    ddlINServerTypeId: string;
    txtINServerNameId: string;
    txtINPortId: string;
    chkINSslId: string;
    txtOUTServerNameId: string;
    txtOUTPortId: string;
    chkOUTSslId: string;
    chkManagedId: string;
    chkUnmanagedId: string;
    chkIsHandleEnabledId: string;
    ddlProfileAddId: string;
    ddlJeepServiceInId: string;
    ddlJeepServiceOutId: string;
    ddlInvoiceTypeId: string;
    chkLoginErrorId: string;
    locations: LocationViewModel[] = [];
    pecMailBoxConfigurations: PECMailBoxConfigurationViewModel[] = [];
    jeepServiceHosts: JeepServiceHostViewModel[] = [];
    invoiceTypes: string[];
    isValidEncryptionKey: boolean;
    ajaxManagerId: string;
    btnSaveId: string;
    pnlDetailsId: string;
    rpbDetailsId: string;
    ajaxLoadingPanelId: string;
    isInsertAction: boolean;
    validationGroupName: string;
    clientId: string;

    private static LOADED_EVENT: string = "onLoaded";
    private _ajaxLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _pnlInformations: Telerik.Web.UI.RadPanelItem;
    private _rpbDetails: Telerik.Web.UI.RadPanelBar;
    private _txtMailBoxName: HTMLInputElement;
    private _txtUsername: HTMLInputElement;
    private _txtPassword: HTMLInputElement;
    private _chkIsInterop: HTMLInputElement;
    private _chkIsProtocol: HTMLInputElement;
    private _chkIsPublicProtocol: HTMLInputElement;
    private _ddlLocation: HTMLSelectElement;
    private _ddlINServerType: HTMLSelectElement;
    private _txtINServerName: HTMLInputElement;
    private _txtINPort: HTMLInputElement;
    private _chkINSsl: HTMLInputElement;
    private _txtOUTServerName: HTMLInputElement;
    private _txtOUTPort: HTMLInputElement;
    private _chkOUTSsl: HTMLInputElement;
    private _chkManaged: HTMLInputElement;
    private _chkUnmanaged: HTMLInputElement;
    private _chkIsHandleEnabled: HTMLInputElement;
    private _ddlProfileAdd: HTMLSelectElement;
    private _ddlJeepServiceIn: HTMLSelectElement;
    private _ddlJeepServiceOut: HTMLSelectElement;
    private _ddlInvoiceType: HTMLSelectElement;
    private _chkLoginError: HTMLInputElement;
    private _btnSave: Telerik.Web.UI.RadButton;

    public static selectedPecId: number;

    private static ENCRYPT_PASSWORD: string = "EncryptPassword";

    constructor(serviceConfigurations: ServiceConfiguration[], clientId: string) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, TbltPECMailBoxBase.PECMailBox_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        this.clientId = clientId;
        $(document).ready(() => {
        });
    }

    public initialize() {
        super.initialize();
        super.initializeServices(this._serviceConfigurations);
        this._txtMailBoxName = <HTMLInputElement>document.getElementById(this.txtMailBoxNameId);
        this._txtUsername = <HTMLInputElement>document.getElementById(this.txtUsernameId);
        this._txtPassword = <HTMLInputElement>document.getElementById(this.txtPasswordId);
        this._chkIsInterop = <HTMLInputElement>document.getElementById(this.chkIsInteropId);
        this._chkIsProtocol = <HTMLInputElement>document.getElementById(this.chkIsProtocolId);
        this._chkIsPublicProtocol = <HTMLInputElement>document.getElementById(this.chkIsPublicProtocolId);
        this._ddlLocation = <HTMLSelectElement>document.getElementById(this.ddlLocationId);
        this._ddlINServerType = <HTMLSelectElement>document.getElementById(this.ddlINServerTypeId);
        this._txtINServerName = <HTMLInputElement>document.getElementById(this.txtINServerNameId);
        this._txtINPort = <HTMLInputElement>document.getElementById(this.txtINPortId);
        this._chkINSsl = <HTMLInputElement>document.getElementById(this.chkINSslId);
        this._txtOUTServerName = <HTMLInputElement>document.getElementById(this.txtOUTServerNameId);
        this._txtOUTPort = <HTMLInputElement>document.getElementById(this.txtOUTPortId);
        this._chkOUTSsl = <HTMLInputElement>document.getElementById(this.chkOUTSslId);
        this._chkManaged = <HTMLInputElement>document.getElementById(this.chkManagedId);
        this._chkUnmanaged = <HTMLInputElement>document.getElementById(this.chkUnmanagedId);
        this._chkIsHandleEnabled = <HTMLInputElement>document.getElementById(this.chkIsHandleEnabledId);
        this._ddlProfileAdd = <HTMLSelectElement>document.getElementById(this.ddlProfileAddId);
        this._ddlJeepServiceIn = <HTMLSelectElement>document.getElementById(this.ddlJeepServiceInId);
        this._ddlJeepServiceOut = <HTMLSelectElement>document.getElementById(this.ddlJeepServiceOutId);
        this._ddlInvoiceType = <HTMLSelectElement>document.getElementById(this.ddlInvoiceTypeId);
        this._chkLoginError = <HTMLInputElement>document.getElementById(this.chkLoginErrorId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._ajaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._rpbDetails = <Telerik.Web.UI.RadPanelBar>$find(this.rpbDetailsId);
        this._pnlInformations = this._rpbDetails.findItemByValue("pnlInformations");
        this._btnSave = <Telerik.Web.UI.RadButton>$find(this.btnSaveId);
        this._btnSave.add_clicking(this.btnSave_onClick);
        this.loadDropdowns();
        $(`#${this.pnlDetailsId}`).hide();
        this.bindLoaded();
        $(`#${this.pnlDetailsId}`).triggerHandler(uscPECMailBoxSettings.LOADED_EVENT);
    }

    private bindLoaded(): void {
        $(`#${this.pnlDetailsId}`).data(this);
    }

    private loadLocationService(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();

        this.locationService.getLocations(
            (dataLocation: LocationViewModel[]) => {
                if (!dataLocation) return;
                this.locations.push(...dataLocation);
                $.each(this.locations, (i, value) => {
                    this._ddlLocation.options.add(new Option(value.Name, String(value.EntityShortId)));
                });
                promise.resolve();
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(this._pnlInformations.get_element().id, exception);
                promise.reject();
            });
        return promise.promise();
    }

    private loadPecMailBoxConfigurationService(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();

        this.pecMailBoxConfigurationService.getPECMailBoxConfigurations(
            (dataPECMailBoxConfiguration: any) => {
                if (!dataPECMailBoxConfiguration) return;
                this.pecMailBoxConfigurations.push(...dataPECMailBoxConfiguration);
                $.each(this.pecMailBoxConfigurations, (i, value) => {
                    this._ddlProfileAdd.options.add(new Option(value.Name, String(value.EntityId)));
                });
                promise.resolve();
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(this._pnlInformations.get_element().id, exception);
                promise.reject();
            });
        return promise.promise();
    }

    private loadJeepServiceHosts(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();

        this.jeepServiceHostService.getJeepServiceHosts(
            (dataJeepServiceHost: JeepServiceHostViewModel[]) => {
                if (!dataJeepServiceHost) return;
                this.jeepServiceHosts.push(...dataJeepServiceHost);
                $.each(this.jeepServiceHosts, (i, value) => {
                    this._ddlJeepServiceIn.options.add(new Option(value.Hostname, value.UniqueId));
                    this._ddlJeepServiceOut.options.add(new Option(value.Hostname, value.UniqueId));
                });
                promise.resolve();
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(this._pnlInformations.get_element().id, exception);
                promise.reject();
            });
        return promise.promise();
    }

    private loadInvoiceTypes(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        $.each(this.invoiceTypes, (i, value) => {
            this._ddlInvoiceType.options.add(new Option(value, String(i)));
        });
        promise.resolve();
        return promise.promise();
    }

    loadDropdowns(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();

        $.when([this.loadLocationService(), this.loadPecMailBoxConfigurationService(), this.loadJeepServiceHosts(), this.loadInvoiceTypes()])
            .done(() => {
                promise.resolve();
            }).fail((err) => promise.reject(err));

        return promise.promise();
    }

    loadPECMailBoxDetails = (pecMailBoxId: number) => {
        this.pecMailBoxService.getPECMailBoxById(pecMailBoxId,
            (data: PECMailBoxModel[]) => {
                if (!data) return;
                let pecMailBox: PECMailBoxModel = data[0];
                this._txtMailBoxName.value = pecMailBox.MailBoxRecipient;
                this._txtUsername.value = pecMailBox.Username;
                this._txtPassword.value = "";
                this._chkIsInterop.checked = pecMailBox.IsForInterop;
                this._chkIsProtocol.checked = pecMailBox.IsProtocolBox;
                this._chkIsPublicProtocol.checked = pecMailBox.IsProtocolBoxExplicit;
                this._txtINServerName.value = pecMailBox.IncomingServer;
                this._txtINPort.value = String(pecMailBox.IncomingServerPort);
                this._chkINSsl.checked = pecMailBox.IncomingServerUseSsl;
                this._txtOUTServerName.value = pecMailBox.OutgoingServer;
                this._txtOUTPort.value = String(pecMailBox.OutgoingServerPort);
                this._chkOUTSsl.checked = pecMailBox.OutgoingServerUseSsl;
                this._chkManaged.checked = pecMailBox.Managed;
                this._chkUnmanaged.checked = pecMailBox.Unmanaged;
                this._chkIsHandleEnabled.checked = pecMailBox.IsHandleEnabled;
                this._chkLoginError.checked = pecMailBox.LoginError;

                this._ddlLocation.options.selectedIndex =
                    this.locations.findIndex(x => x.EntityShortId === pecMailBox.Location.EntityShortId) + 1;
                this._ddlINServerType.options.selectedIndex = 1 + pecMailBox.IncomingServerProtocol;
                this._ddlProfileAdd.options.selectedIndex =
                    this.pecMailBoxConfigurations.findIndex(x => x.EntityId === pecMailBox.IdConfiguration);
                this._ddlJeepServiceIn.options.selectedIndex =
                    this.jeepServiceHosts.findIndex(x => x.UniqueId === pecMailBox.IdJeepServiceIncomingHost) + 1;
                this._ddlJeepServiceOut.options.selectedIndex =
                    this.jeepServiceHosts.findIndex(x => x.UniqueId === pecMailBox.IdJeepServiceOutgoingHost) + 1;
                this._ddlInvoiceType.value = pecMailBox.InvoiceType;
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(this._pnlInformations.get_element().id, exception);
            });
    }

    emptyUserInputs() {
        this._txtMailBoxName.value = "";
        this._txtUsername.value = "";
        this._txtPassword.value = "";
        this._chkIsInterop.checked = false;
        this._chkIsProtocol.checked = false;
        this._chkIsPublicProtocol.checked = false;
        this._txtINServerName.value = "";
        this._txtINPort.value = "";
        this._chkINSsl.checked = false;
        this._txtOUTServerName.value = "";
        this._txtOUTPort.value = "";
        this._chkOUTSsl.checked = false;
        this._chkManaged.checked = false;
        this._chkUnmanaged.checked = false;
        this._chkIsHandleEnabled.checked = false;
        this._chkLoginError.checked = false;

        this._ddlLocation.options.selectedIndex = 0;
        this._ddlINServerType.options.selectedIndex = 0;
        this._ddlProfileAdd.options.selectedIndex = 0;
        this._ddlJeepServiceIn.options.selectedIndex = 0;
        this._ddlJeepServiceOut.options.selectedIndex = 0;
        this._ddlInvoiceType.value = "";
    }

    setPanelText(text: string) {
        this._rpbDetails.findItemByValue("pnlInformations").set_text(text);
    }

    btnSave_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        this._ajaxLoadingPanel.show(this.pnlDetailsId);

        if (this.isValidEncryptionKey) {
            this._btnSave.set_enabled(false);
            this._ajaxLoadingPanel.hide(this.pnlDetailsId);
            alert("La password non è impostata. Contattare assistenza per inserisci una password.");
            return;
        }

        //if (uscPECMailBoxSettings.selectedPecId) {
        //    ValidatorEnable(document.getElementById('<%=txtPasswordRequireValidator.ClientID%>'), false);
        //}

        let isValid = Page_ClientValidate(this.validationGroupName);
        if (!isValid) {
            this._ajaxLoadingPanel.hide(this.pnlDetailsId);
            args.set_cancel(true);
            return;
        }

        const selectedLocation: LocationViewModel = this.locations.find(p => p.EntityShortId === +this._ddlLocation.value);
        const selectedJeepServiceIncomingHost: JeepServiceHostViewModel = this.jeepServiceHosts?.find(p => p.UniqueId === this._ddlJeepServiceIn.value);
        const selectedJeepServiceOutgoingHost: JeepServiceHostViewModel = this.jeepServiceHosts?.find(p => p.UniqueId === this._ddlJeepServiceOut.value);
        const selectedPecMailBoxConfig: PECMailBoxConfigurationViewModel = this.pecMailBoxConfigurations?.find(p => p.EntityId === +this._ddlProfileAdd.value);
        const selectedInvoiceType: string = this._ddlInvoiceType.value;

        if (!this.isInsertAction) {
            this.pecMailBoxService.getPECMailBoxById(uscPECMailBoxSettings.selectedPecId,
                (data: PECMailBoxModel[]) => {
                    if (!data) return;
                    let pecMailBoxModel: PECMailBoxModel = data[0];
                    pecMailBoxModel.MailBoxRecipient = this._txtMailBoxName.value;
                    pecMailBoxModel.IncomingServer = this._txtINServerName.value;
                    pecMailBoxModel.OutgoingServer = this._txtOUTServerName.value;
                    pecMailBoxModel.Username = this._txtUsername.value;
                    pecMailBoxModel.Password = this._txtPassword.value !== "" ? this._txtPassword.value : pecMailBoxModel.Password;
                    pecMailBoxModel.IsForInterop = this._chkIsInterop.checked;
                    pecMailBoxModel.Managed = this._chkManaged.checked;
                    pecMailBoxModel.Unmanaged = this._chkUnmanaged.checked;
                    pecMailBoxModel.IdConfiguration = selectedPecMailBoxConfig.EntityId;
                    pecMailBoxModel.IncomingServerProtocol = this._ddlINServerType.options.selectedIndex - 1;
                    pecMailBoxModel.IncomingServerPort = Number(this._txtINPort.value);
                    pecMailBoxModel.IncomingServerUseSsl = this._chkINSsl.checked;
                    pecMailBoxModel.OutgoingServerPort = Number(this._txtOUTPort.value);
                    pecMailBoxModel.OutgoingServerUseSsl = this._chkOUTSsl.checked;
                    pecMailBoxModel.IsHandleEnabled = this._chkIsHandleEnabled.checked;
                    pecMailBoxModel.IsProtocolBox = this._chkIsProtocol.checked;
                    pecMailBoxModel.IsProtocolBoxExplicit = this._chkIsPublicProtocol.checked;
                    pecMailBoxModel.IdJeepServiceIncomingHost = selectedJeepServiceIncomingHost?.UniqueId || "";
                    pecMailBoxModel.IdJeepServiceOutgoingHost = selectedJeepServiceOutgoingHost?.UniqueId || "";
                    pecMailBoxModel.LoginError = this._chkLoginError.checked;
                    pecMailBoxModel.Location = selectedLocation;
                    pecMailBoxModel.InvoiceType = selectedInvoiceType;

                    let ajaxModel: AjaxModel = <AjaxModel>{};
                    ajaxModel.Value = new Array<string>();
                    ajaxModel.ActionName = uscPECMailBoxSettings.ENCRYPT_PASSWORD;
                    ajaxModel.Value.push("Update");
                    ajaxModel.Value.push(JSON.stringify(pecMailBoxModel));
                    ajaxModel.Value.push(this.clientId);
                    this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
                },
                (exception: ExceptionDTO) => {
                    this.showNotificationException(this._pnlInformations.get_element().id, exception);
                });
        }
        else {
            let pecMailBoxModel: PECMailBoxModel = {
                MailBoxRecipient: this._txtMailBoxName.value,
                IncomingServer: this._txtINServerName.value,
                OutgoingServer: this._txtOUTServerName.value,
                Username: this._txtUsername.value,
                Password: this._txtPassword.value,
                IsForInterop: this._chkIsInterop.checked,
                Managed: this._chkManaged.checked,
                Unmanaged: this._chkUnmanaged.checked,
                IdConfiguration: selectedPecMailBoxConfig.EntityId,
                IncomingServerProtocol: this._ddlINServerType.options.selectedIndex - 1,
                IncomingServerPort: Number(this._txtINPort.value),
                IncomingServerUseSsl: this._chkINSsl.checked,
                OutgoingServerPort: Number(this._txtOUTPort.value),
                OutgoingServerUseSsl: this._chkOUTSsl.checked,
                IsHandleEnabled: this._chkIsHandleEnabled.checked,
                IsProtocolBox: this._chkIsProtocol.checked,
                IsProtocolBoxExplicit: this._chkIsPublicProtocol.checked,
                IdJeepServiceIncomingHost: selectedJeepServiceIncomingHost?.UniqueId || "",
                IdJeepServiceOutgoingHost: selectedJeepServiceOutgoingHost?.UniqueId || "",
                LoginError: this._chkLoginError.checked,
                Location: selectedLocation,
                InvoiceType: selectedInvoiceType,
                RulesetDefinition: null,
                EntityShortId: 0,
                UniqueId: Guid.newGuid(),
                ReceiveDaysCap: 0,
                DeleteMailFromServer: false,
                RegistrationUser: "",
                RegistrationDate: new Date(),
                EntityId: 0,
                IsDestinationEnabled: false,
                LastChangedDate: null,
                LastChangedUser: "",
                RedirectAnomaliesPassword: "",
                RedirectAnomaliesRecipient: "",
                RedirectAnomaliesSMTP: "",
                RedirectAnomaliesUsername: "",
                RedirectStoragePassword: "",
                RedirectStorageRecipient: "",
                RedirectStorageSMTP: "",
                RedirectStorageUsername: ""
            };

            let ajaxModel: AjaxModel = <AjaxModel>{};
            ajaxModel.Value = new Array<string>();
            ajaxModel.ActionName = uscPECMailBoxSettings.ENCRYPT_PASSWORD;
            ajaxModel.Value.push("Insert");
            ajaxModel.Value.push(JSON.stringify(pecMailBoxModel));
            ajaxModel.Value.push(this.clientId);
            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
        }
    }

    pecMAilWithEncryptedPassword(pecMailBoxModelJSON: string, isInsertAction: boolean) {
        let pecMailBoxModel: PECMailBoxModel = <PECMailBoxModel>JSON.parse(pecMailBoxModelJSON);
        if (!isInsertAction) {
            this.pecMailBoxService.updatePECMailBox(pecMailBoxModel,
                (data: any) => {
                    if (!data) return;
                    this._ajaxLoadingPanel.hide(this.pnlDetailsId);
                },
                (exception: ExceptionDTO) => {
                    console.log(exception);
                    this._ajaxLoadingPanel.hide(this.pnlDetailsId);
                    alert("Errore in fase di salvataggio casella PEC.");
                });
        }
        else {
            pecMailBoxModel.UniqueId = null;
            this.pecMailBoxService.insertPECMailBox(pecMailBoxModel,
                (data: any) => {
                    if (!data) return;
                    this._ajaxLoadingPanel.hide(this.pnlDetailsId);
                    let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
                    if (wnd) {
                        wnd.close();
                    }
                },
                (exception: ExceptionDTO) => {
                    console.log(exception);
                    this._ajaxLoadingPanel.hide(this.pnlDetailsId);
                    alert("Errore in fase di salvataggio casella PEC.");
                });
        }

    }

    private getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window.parent).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.parent).radWindow;
        else if ((<any>window.parent.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.parent.frameElement).radWindow;
        else wnd = GetRadWindowManager().getActiveWindow();
        return wnd;
    }
}

export = uscPECMailBoxSettings;