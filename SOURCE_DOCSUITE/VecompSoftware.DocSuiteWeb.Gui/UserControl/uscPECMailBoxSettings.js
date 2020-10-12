var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "Tblt/TbltPECMailBoxBase"], function (require, exports, ServiceConfigurationHelper, TbltPECMailBoxBase) {
    var uscPECMailBoxSettings = /** @class */ (function (_super) {
        __extends(uscPECMailBoxSettings, _super);
        function uscPECMailBoxSettings(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, TbltPECMailBoxBase.PECMailBox_TYPE_NAME)) || this;
            _this.rwPECMailBoxSettings_Show = function (sender, args) {
                if (document.getElementById(_this.lblPECMailBoxIdId).innerText !== "") {
                    _this.loadPECMailBoxDetails(Number(document.getElementById(_this.lblPECMailBoxIdId).innerText));
                }
                else
                    _this.emptyUserInputs();
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        uscPECMailBoxSettings.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            _super.prototype.initializeServices.call(this, this._serviceConfigurations);
            this._txtMailBoxName = document.getElementById(this.txtMailBoxNameId);
            this._txtUsername = document.getElementById(this.txtUsernameId);
            this._txtPassword = document.getElementById(this.txtPasswordId);
            this._chkIsInterop = document.getElementById(this.chkIsInteropId);
            this._chkIsProtocol = document.getElementById(this.chkIsProtocolId);
            this._chkIsPublicProtocol = document.getElementById(this.chkIsPublicProtocolId);
            this._ddlLocation = document.getElementById(this.ddlLocationId);
            this._ddlINServerType = document.getElementById(this.ddlINServerTypeId);
            this._txtINServerName = document.getElementById(this.txtINServerNameId);
            this._txtINPort = document.getElementById(this.txtINPortId);
            this._chkINSsl = document.getElementById(this.chkINSslId);
            this._txtOUTServerName = document.getElementById(this.txtOUTServerNameId);
            this._txtOUTPort = document.getElementById(this.txtOUTPortId);
            this._chkOUTSsl = document.getElementById(this.chkOUTSslId);
            this._chkManaged = document.getElementById(this.chkManagedId);
            this._chkUnmanaged = document.getElementById(this.chkUnmanagedId);
            this._chkIsHandleEnabled = document.getElementById(this.chkIsHandleEnabledId);
            this._ddlProfileAdd = document.getElementById(this.ddlProfileAddId);
            this._ddlJeepServiceIn = document.getElementById(this.ddlJeepServiceInId);
            this._ddlJeepServiceOut = document.getElementById(this.ddlJeepServiceOutId);
            this._ddlInvoiceType = document.getElementById(this.ddlInvoiceTypeId);
            this._chkLoginError = document.getElementById(this.chkLoginErrorId);
            this._rwPECMailBoxSettings = $find(this.rwPECMailBoxSettingsId);
            this._rwPECMailBoxSettings.add_show(this.rwPECMailBoxSettings_Show);
            this.loadDropdowns();
        };
        uscPECMailBoxSettings.prototype.loadDropdowns = function () {
            var _this = this;
            var thisObj = this;
            this.locationService.getLocations(function (dataLocation) {
                if (!dataLocation)
                    return;
                _this.locations = dataLocation;
                $.each(_this.locations, function (i, value) {
                    thisObj._ddlLocation.options.add(new Option(value.Name, String(value.EntityShortId)));
                });
            }, function (exception) {
                thisObj.showNotificationException(thisObj.rwPECMailBoxSettingsId, exception);
            });
            this.pecMailBoxConfigurationService.getPECMailBoxConfigurations(function (dataPECMailBoxConfiguration) {
                if (!dataPECMailBoxConfiguration)
                    return;
                _this.pecMailBoxConfigurations = dataPECMailBoxConfiguration;
                $.each(_this.pecMailBoxConfigurations, function (i, value) {
                    thisObj._ddlProfileAdd.options.add(new Option(value.Name, String(value.EntityId)));
                });
            }, function (exception) {
                thisObj.showNotificationException(thisObj.rwPECMailBoxSettingsId, exception);
            });
            this.jeepServiceHostService.getJeepServiceHosts(function (dataJeepServiceHost) {
                if (!dataJeepServiceHost)
                    return;
                _this.jeepServiceHosts = dataJeepServiceHost;
                $.each(_this.jeepServiceHosts, function (i, value) {
                    thisObj._ddlJeepServiceIn.options.add(new Option(value.Hostname, value.UniqueId));
                    thisObj._ddlJeepServiceOut.options.add(new Option(value.Hostname, value.UniqueId));
                });
            }, function (exception) {
                thisObj.showNotificationException(thisObj.rwPECMailBoxSettingsId, exception);
            });
            $.each(this.invoiceTypes, function (i, value) {
                thisObj._ddlInvoiceType.options.add(new Option(value, String(i)));
            });
        };
        uscPECMailBoxSettings.prototype.loadPECMailBoxDetails = function (pecMailBoxId) {
            var _this = this;
            this.pecMailBoxService.getPECMailBoxById(pecMailBoxId, function (data) {
                if (!data)
                    return;
                var pecMailBox = data[0];
                _this._txtMailBoxName.value = pecMailBox.MailBoxRecipient;
                _this._txtUsername.value = pecMailBox.Username;
                _this._txtPassword.value = "";
                _this._chkIsInterop.checked = pecMailBox.IsForInterop;
                _this._chkIsProtocol.checked = pecMailBox.IsProtocolBox;
                _this._chkIsPublicProtocol.checked = pecMailBox.IsProtocolBoxExplicit;
                _this._txtINServerName.value = pecMailBox.IncomingServer;
                _this._txtINPort.value = String(pecMailBox.IncomingServerPort);
                _this._chkINSsl.checked = pecMailBox.IncomingServerUseSsl;
                _this._txtOUTServerName.value = pecMailBox.OutgoingServer;
                _this._txtOUTPort.value = String(pecMailBox.OutgoingServerPort);
                _this._chkOUTSsl.checked = pecMailBox.OutgoingServerUseSsl;
                _this._chkManaged.checked = pecMailBox.Managed;
                _this._chkUnmanaged.checked = pecMailBox.Unmanaged;
                _this._chkIsHandleEnabled.checked = pecMailBox.IsHandleEnabled;
                _this._chkLoginError.checked = pecMailBox.LoginError;
                _this._ddlLocation.options.selectedIndex =
                    _this.locations.findIndex(function (x) { return x.EntityShortId === pecMailBox.Location.EntityShortId; }) + 1;
                _this._ddlINServerType.options.selectedIndex = 1 + pecMailBox.IncomingServerProtocol;
                _this._ddlProfileAdd.options.selectedIndex =
                    _this.pecMailBoxConfigurations.findIndex(function (x) { return x.EntityId === pecMailBox.IdConfiguration; });
                _this._ddlJeepServiceIn.options.selectedIndex =
                    _this.jeepServiceHosts.findIndex(function (x) { return x.UniqueId === pecMailBox.IdJeepServiceIncomingHost; }) + 1;
                _this._ddlJeepServiceOut.options.selectedIndex =
                    _this.jeepServiceHosts.findIndex(function (x) { return x.UniqueId === pecMailBox.IdJeepServiceOutgoingHost; }) + 1;
                _this._ddlInvoiceType.value = pecMailBox.InvoiceType;
            }, function (exception) {
                _this.showNotificationException(_this.rwPECMailBoxSettingsId, exception);
            });
        };
        uscPECMailBoxSettings.prototype.emptyUserInputs = function () {
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
        };
        uscPECMailBoxSettings.prototype.savePECMailBox = function () {
            var _this = this;
            if (document.getElementById(this.lblPECMailBoxIdId).innerText !== "") {
                this.pecMailBoxService.getPECMailBoxById(Number(document.getElementById(this.lblPECMailBoxIdId).innerText), function (data) {
                    if (!data)
                        return;
                    var pecMailBoxModel = data[0];
                    pecMailBoxModel.MailBoxRecipient = _this._txtMailBoxName.value;
                    pecMailBoxModel.IncomingServer = _this._txtINServerName.value;
                    pecMailBoxModel.OutgoingServer = _this._txtOUTServerName.value;
                    pecMailBoxModel.Username = _this._txtUsername.value;
                    pecMailBoxModel.Password = _this._txtPassword.value !== "" ? _this._txtPassword.value : pecMailBoxModel.Password;
                    pecMailBoxModel.IsForInterop = _this._chkIsInterop.checked;
                    pecMailBoxModel.Managed = _this._chkManaged.checked;
                    pecMailBoxModel.Unmanaged = _this._chkUnmanaged.checked;
                    pecMailBoxModel.IdConfiguration =
                        _this.pecMailBoxConfigurations[_this._ddlProfileAdd.options.selectedIndex].EntityId;
                    pecMailBoxModel.IncomingServerProtocol = _this._ddlINServerType.options.selectedIndex - 1;
                    pecMailBoxModel.IncomingServerPort = Number(_this._txtINPort.value);
                    pecMailBoxModel.IncomingServerUseSsl = _this._chkINSsl.checked;
                    pecMailBoxModel.OutgoingServerPort = Number(_this._txtOUTPort.value);
                    pecMailBoxModel.OutgoingServerUseSsl = _this._chkOUTSsl.checked;
                    pecMailBoxModel.IsHandleEnabled = _this._chkIsHandleEnabled.checked;
                    pecMailBoxModel.IsProtocolBox = _this._chkIsProtocol.checked;
                    pecMailBoxModel.IsProtocolBoxExplicit = _this._chkIsPublicProtocol.checked;
                    pecMailBoxModel.IdJeepServiceIncomingHost = _this.jeepServiceHosts.length !== 0
                        ? _this.jeepServiceHosts[_this._ddlJeepServiceIn.options.selectedIndex].UniqueId
                        : "";
                    pecMailBoxModel.IdJeepServiceOutgoingHost = _this.jeepServiceHosts.length !== 0
                        ? _this.jeepServiceHosts[_this._ddlJeepServiceOut.options.selectedIndex].UniqueId
                        : "";
                    pecMailBoxModel.LoginError = _this._chkLoginError.checked;
                    pecMailBoxModel.Location = _this.locations[_this._ddlLocation.options.selectedIndex - 1];
                    pecMailBoxModel.InvoiceType = _this._ddlInvoiceType.options[_this._ddlInvoiceType.selectedIndex].value;
                    _this.pecMailBoxService.updatePECMailBox(pecMailBoxModel, function (data) {
                        if (!data)
                            return;
                        _this._rwPECMailBoxSettings.close();
                    }, function (exception) {
                        console.log(exception);
                        alert("Errore in fase di salvataggio casella PEC.");
                    });
                }, function (exception) {
                    _this.showNotificationException(_this.rwPECMailBoxSettingsId, exception);
                });
            }
            else {
                var pecMailBoxModel = {
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
                this.pecMailBoxService.insertPECMailBox(pecMailBoxModel, function (data) {
                    if (!data)
                        return;
                    _this._rwPECMailBoxSettings.close();
                }, function (exception) {
                    console.log(exception);
                    alert("Errore in fase di salvataggio casella PEC.");
                });
            }
        };
        return uscPECMailBoxSettings;
    }(TbltPECMailBoxBase));
    return uscPECMailBoxSettings;
});
//# sourceMappingURL=uscPECMailBoxSettings.js.map