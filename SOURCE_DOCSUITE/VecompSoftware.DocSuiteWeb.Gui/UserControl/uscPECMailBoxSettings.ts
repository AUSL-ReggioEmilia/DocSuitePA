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

class uscPECMailBoxSettings extends TbltPECMailBoxBase {
  private _serviceConfigurations: ServiceConfiguration[];
  protected pecMailBoxService: PECMailBoxService;
  protected pecMailBoxConfigurationService: PECMailBoxConfigurationService;
  protected locationService: LocationService;
  protected jeepServiceHostService:JeepServiceHostService;

  rwPECMailBoxSettingsId: string;
  lblPECMailBoxIdId: string;
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
  locations: LocationViewModel[];
  pecMailBoxConfigurations: PECMailBoxConfigurationViewModel[];
  jeepServiceHosts: JeepServiceHostViewModel[];
  invoiceTypes: string[];

  private _rwPECMailBoxSettings: Telerik.Web.UI.RadWindow;
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

  constructor(serviceConfigurations: ServiceConfiguration[]) {
    super(ServiceConfigurationHelper.getService(serviceConfigurations, TbltPECMailBoxBase.PECMailBox_TYPE_NAME));
    this._serviceConfigurations = serviceConfigurations;
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
    this._rwPECMailBoxSettings = <Telerik.Web.UI.RadWindow>$find(this.rwPECMailBoxSettingsId);
    this._rwPECMailBoxSettings.add_show(this.rwPECMailBoxSettings_Show);
    this.loadDropdowns();
  }

  rwPECMailBoxSettings_Show = (sender: any, args: any) => {
    if (document.getElementById(this.lblPECMailBoxIdId).innerText !== "") {
      this.loadPECMailBoxDetails(Number(document.getElementById(this.lblPECMailBoxIdId).innerText));
    }
    else
      this.emptyUserInputs();
  }

  loadDropdowns() {
    let thisObj = this;
    this.locationService.getLocations(
      (dataLocation: LocationViewModel[]) => {
        if (!dataLocation) return;
        this.locations = dataLocation;
        $.each(this.locations, function (i, value) {
          thisObj._ddlLocation.options.add(new Option(value.Name, String(value.EntityShortId)));
        });
      },
      (exception: ExceptionDTO) => {
        thisObj.showNotificationException(thisObj.rwPECMailBoxSettingsId, exception);
      });
    
    this.pecMailBoxConfigurationService.getPECMailBoxConfigurations(
      (dataPECMailBoxConfiguration: any) => {
        if (!dataPECMailBoxConfiguration) return;
        this.pecMailBoxConfigurations = dataPECMailBoxConfiguration;
        $.each(this.pecMailBoxConfigurations, function (i, value) {
          thisObj._ddlProfileAdd.options.add(new Option(value.Name, String(value.EntityId)));
        });
      },
      (exception: ExceptionDTO) => {
        thisObj.showNotificationException(thisObj.rwPECMailBoxSettingsId, exception);
      });

    this.jeepServiceHostService.getJeepServiceHosts(
      (dataJeepServiceHost: JeepServiceHostViewModel[]) => {
        if (!dataJeepServiceHost) return;
        this.jeepServiceHosts = dataJeepServiceHost;
        $.each(this.jeepServiceHosts, function (i, value) {
          thisObj._ddlJeepServiceIn.options.add(new Option(value.Hostname, value.UniqueId));
          thisObj._ddlJeepServiceOut.options.add(new Option(value.Hostname, value.UniqueId));
        });
      },
      (exception: ExceptionDTO) => {
        thisObj.showNotificationException(thisObj.rwPECMailBoxSettingsId, exception);
      });

    $.each(this.invoiceTypes, function (i, value) {
      thisObj._ddlInvoiceType.options.add(new Option(value, String(i)));
    });
  }

  loadPECMailBoxDetails(pecMailBoxId: number) {
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
        this.showNotificationException(this.rwPECMailBoxSettingsId, exception);
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
    this._chkLoginError.checked = true;
    
    this._ddlLocation.options.selectedIndex = 0;
    this._ddlINServerType.options.selectedIndex = 0;
    this._ddlProfileAdd.options.selectedIndex = 0;
    this._ddlJeepServiceIn.options.selectedIndex = 0;
    this._ddlJeepServiceOut.options.selectedIndex = 0;
    this._ddlInvoiceType.value = "";
  }

  savePECMailBox() {
    if (document.getElementById(this.lblPECMailBoxIdId).innerText !== "") {
      this.pecMailBoxService.getPECMailBoxById(Number(document.getElementById(this.lblPECMailBoxIdId).innerText),
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
          pecMailBoxModel.IdConfiguration =
            this.pecMailBoxConfigurations[this._ddlProfileAdd.options.selectedIndex].EntityId;
          pecMailBoxModel.IncomingServerProtocol = this._ddlINServerType.options.selectedIndex - 1;
          pecMailBoxModel.IncomingServerPort = Number(this._txtINPort.value);
          pecMailBoxModel.IncomingServerUseSsl = this._chkINSsl.checked;
          pecMailBoxModel.OutgoingServerPort = Number(this._txtOUTPort.value);
          pecMailBoxModel.OutgoingServerUseSsl = this._chkOUTSsl.checked;
          pecMailBoxModel.IsHandleEnabled = this._chkIsHandleEnabled.checked;
          pecMailBoxModel.IsProtocolBox = this._chkIsProtocol.checked;
          pecMailBoxModel.IsProtocolBoxExplicit = this._chkIsPublicProtocol.checked;
          pecMailBoxModel.IdJeepServiceIncomingHost = this.jeepServiceHosts.length !== 0
            ? this.jeepServiceHosts[this._ddlJeepServiceIn.options.selectedIndex].UniqueId
            : "";
          pecMailBoxModel.IdJeepServiceOutgoingHost = this.jeepServiceHosts.length !== 0
            ? this.jeepServiceHosts[this._ddlJeepServiceOut.options.selectedIndex].UniqueId
            : "";
          pecMailBoxModel.LoginError = this._chkLoginError.checked;
          pecMailBoxModel.Location = this.locations[this._ddlLocation.options.selectedIndex - 1];
          pecMailBoxModel.InvoiceType = this._ddlInvoiceType.options[this._ddlInvoiceType.selectedIndex].value;
          
          this.pecMailBoxService.updatePECMailBox(pecMailBoxModel,
            (data: any) => {
              if (!data) return;
              this._rwPECMailBoxSettings.close();
            },
            (exception: ExceptionDTO) => {
              console.log(exception);
              alert("Errore in fase di salvataggio casella PEC.");
            });
        },
        (exception: ExceptionDTO) => {
          this.showNotificationException(this.rwPECMailBoxSettingsId, exception);
        });
    } else {
      let pecMailBoxModel: PECMailBoxModel = {
        MailBoxRecipient: this._txtMailBoxName.value,
        IncomingServer: this._txtINServerName.value,
        OutgoingServer: this._txtOUTServerName.value,
        Username: this._txtUsername.value,
        Password: this._txtPassword.value,
        IsForInterop: this._chkIsInterop.checked,
        Managed: this._chkManaged.checked,
        Unmanaged: this._chkUnmanaged.checked,
        IdConfiguration: this.pecMailBoxConfigurations[this._ddlProfileAdd.options.selectedIndex].EntityId,
        IncomingServerProtocol: this._ddlINServerType.options.selectedIndex - 1,
        IncomingServerPort: Number(this._txtINPort.value),
        IncomingServerUseSsl: this._chkINSsl.checked,
        OutgoingServerPort: Number(this._txtOUTPort.value),
        OutgoingServerUseSsl: this._chkOUTSsl.checked,
        IsHandleEnabled: this._chkIsHandleEnabled.checked,
        IsProtocolBox: this._chkIsProtocol.checked,
        IsProtocolBoxExplicit: this._chkIsPublicProtocol.checked,
        IdJeepServiceIncomingHost: this.jeepServiceHosts.length !== 0
          ? this.jeepServiceHosts[this._ddlJeepServiceIn.options.selectedIndex].UniqueId
          : "",
        IdJeepServiceOutgoingHost: this.jeepServiceHosts.length !== 0
          ? this.jeepServiceHosts[this._ddlJeepServiceOut.options.selectedIndex].UniqueId
          : "",
        LoginError: this._chkLoginError.checked,
        Location: this.locations[this._ddlLocation.selectedIndex],
        InvoiceType: this._ddlInvoiceType.options[this._ddlInvoiceType.selectedIndex].value,
        RulesetDefinition: "",
        EntityShortId: 0,
        UniqueId: "",
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

      this.pecMailBoxService.insertPECMailBox(pecMailBoxModel,
        (data: any)=> {
          if (!data) return;
          this._rwPECMailBoxSettings.close();
        },
        (exception: ExceptionDTO) => {
          console.log(exception);
          alert("Errore in fase di salvataggio casella PEC.");
        });
    }
  }

}

export = uscPECMailBoxSettings;