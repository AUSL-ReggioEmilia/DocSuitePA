/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
define(["require", "exports", "App/Services/UserLogs/UserLogsService", "App/Helpers/ServiceConfigurationHelper", "App/Helpers/EnumHelper", "App/Models/SignDocuments/ProviderSignType", "App/Helpers/GuidHelper", "../App/Models/SignDocuments/ArubaSignModel", "../App/Models/SignDocuments/SignType", "../App/Models/SignDocuments/SignRequestType", "../App/Models/SignDocuments/ProxySignModel", "../App/Models/SignDocuments/SignModel", "../App/Helpers/GenericHelper", "../App/Services/Biblos/BiblosDocumentsService"], function (require, exports, UserLogsService, ServiceConfigurationHelper, EnumHelper, ProviderSignType, GuidHelper, ArubaSignModel, SignType, SignRequestType, ProxySignModel, SignModel, GenericHelper, BiblosDocumentsService) {
    var SingleSignRest = /** @class */ (function () {
        function SingleSignRest(serviceConfigurations) {
            var _this = this;
            this.ToolBar_ButtonClick = function (event, args) {
                switch (args.get_item().get_value()) {
                    case "requestOtp": {
                        _this._signalR.startConnection(function () { return _this.onDoneSignalRConnectionCallback(); }, _this.onErrorSignalRCallback);
                        break;
                    }
                    case "PAdES":
                    case "CAdES": {
                        _this.documentFormatType = args.get_item().get_value();
                        break;
                    }
                    case "sign": {
                        _this._signalR.startConnection(function () { return _this.signDocument(_this.chainId); }, _this.onErrorSignalRCallback);
                    }
                }
            };
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
        }
        SingleSignRest.prototype.initialize = function () {
            this._ToolBar = $find(this.ToolBarId);
            this._CAdES = this._ToolBar.findItemByValue("CAdES");
            this._PAdES = this._ToolBar.findItemByValue("PAdES");
            var serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "UserLog");
            this._service = new UserLogsService(serviceConfiguration);
            var biblosDocumentsServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "BiblosDocument");
            this._biblosDocuments = new BiblosDocumentsService(biblosDocumentsServiceConfiguration);
            var smartcardDescription = this._enumHelper.getRemoteSignDescription(ProviderSignType.Smartcard);
            var arubaAutomaticDescription = this._enumHelper.getRemoteSignDescription(ProviderSignType.ArubaAutomatic);
            var arubaRemoteDescription = this._enumHelper.getRemoteSignDescription(ProviderSignType.ArubaRemote);
            var infocertAutomaticDescription = this._enumHelper.getRemoteSignDescription(ProviderSignType.InfocertAutomatic);
            var infocertRemoteDescription = this._enumHelper.getRemoteSignDescription(ProviderSignType.InfocertRemote);
            this._requestOTP = this._ToolBar.findItemByValue("requestOtp");
            this._signalR = new DSWSignalR(this.dswSignalR);
            this.correlationId = GuidHelper.newGuid();
            this._signalR.setup("WorkflowHub", {
                'correlationId': this.correlationId
            });
            this._ToolBar.add_buttonClicked(this.ToolBar_ButtonClick);
            if (this.typeOfSign == smartcardDescription) {
                this._ToolBar.findItemByValue("pinText").set_visible(true);
                this._ToolBar.findItemByValue("pinContainer").set_visible(true);
                this._ToolBar.findItemByValue("pinContainer2").set_visible(false);
                this._ToolBar.findItemByValue("requestOtp").set_visible(false);
                this._ToolBar.findItemByValue("otpContainer").set_visible(false);
                this._ToolBar.findItemByValue("otpContainer2").set_visible(false);
                this._CAdES.check();
                this._PAdES.unCheck();
                this.documentFormatType = "CAdES";
            }
            if (this.typeOfSign == arubaRemoteDescription) {
                this._ToolBar.findItemByValue("pinText").set_visible(true);
                this._ToolBar.findItemByValue("pinContainer2").set_visible(false);
                this._ToolBar.findItemByValue("requestOtp").set_visible(true);
                this._ToolBar.findItemByValue("otpContainer").set_visible(true);
                this._ToolBar.findItemByValue("otpContainer2").set_visible(true);
                this._PAdES.hide();
                this._CAdES.check();
                this.documentFormatType = "CAdES";
            }
            if (this.typeOfSign == infocertRemoteDescription) {
                this._ToolBar.findItemByValue("pinText").set_visible(true);
                this._ToolBar.findItemByValue("pinContainer2").set_visible(false);
                this._ToolBar.findItemByValue("requestOtp").set_visible(true);
                this._ToolBar.findItemByValue("otpContainer").set_visible(true);
                this._ToolBar.findItemByValue("otpContainer2").set_visible(true);
                this._ToolBar.findItemByValue("PAdES").set_visible(true);
                this._PAdES.unCheck();
                this._CAdES.check();
                this.documentFormatType = "CAdES";
            }
            if (this.typeOfSign == arubaAutomaticDescription) {
                this._ToolBar.findItemByValue("pinText").set_visible(false);
                this._ToolBar.findItemByValue("pinContainer").set_visible(true);
                this._ToolBar.findItemByValue("pinContainer2").set_visible(true);
                this._ToolBar.findItemByValue("requestOtp").set_visible(false);
                this._ToolBar.findItemByValue("otpContainer").set_visible(false);
                this._ToolBar.findItemByValue("otpContainer2").set_visible(false);
                this._CAdES.check();
                this._PAdES.hide();
                this.documentFormatType = "CAdES";
            }
            if (this.typeOfSign == infocertAutomaticDescription) {
                this._ToolBar.findItemByValue("pinText").set_visible(false);
                this._ToolBar.findItemByValue("pinContainer").set_visible(true);
                this._ToolBar.findItemByValue("pinContainer2").set_visible(true);
                this._ToolBar.findItemByValue("requestOtp").set_visible(false);
                this._ToolBar.findItemByValue("otpContainer").set_visible(false);
                this._ToolBar.findItemByValue("otpContainer2").set_visible(false);
                this._CAdES.check();
                this._PAdES.unCheck();
                this.documentFormatType = "CAdES";
            }
            this.loadData(this.currentUserDomain);
            this.chainId = GenericHelper.getUrlParams(window.location.href, "IdChain");
            this.currentSignProperties = new SignModel();
        };
        SingleSignRest.prototype.changeCaseFirstLetter = function (params) {
            if (typeof params === 'string') {
                return params.charAt(0).toUpperCase() + params.slice(1);
            }
            return null;
        };
        SingleSignRest.prototype.signDocument = function (chainID) {
            var _this = this;
            if (this.typeOfSign == "InfocertRemote") {
                alert("La funzionalità è disabilitata");
            }
            if (this.typeOfSign == "ArubaRemote") {
                var arubaModel = new ArubaSignModel();
                var otpInput = this._ToolBar.findItemByValue("otpContainer2").findControl("proxyOtp");
                arubaModel.OTPPassword = otpInput.get_value();
                arubaModel.OTPAuthType = this.singleSignInfo.UserProfile.ArubaRemote.CustomProperties.OTPAuthType;
                arubaModel.User = this.singleSignInfo.UserProfile.ArubaRemote.Alias;
                var pinInput = this._ToolBar.findItemByValue("pinContainer").findControl("pin");
                arubaModel.UserPassword = pinInput.get_value();
                arubaModel.CertificateId = this.singleSignInfo.UserProfile.ArubaRemote.CustomProperties.CertificateId;
                arubaModel.SignType = SignType.Remote;
                arubaModel.RequestType = SignRequestType[this.changeCaseFirstLetter(this.documentFormatType.toLowerCase())];
                this.currentSignProperties.OTP = otpInput.get_value();
            }
            if (this.typeOfSign == "ArubaAutomatic") {
                var arubaModel = new ArubaSignModel();
                arubaModel.DelegatedDomain = this.singleSignInfo.UserProfile.ArubaAutomatic.CustomProperties.DelegatedDomain;
                arubaModel.DelegatedPassword = this.singleSignInfo.UserProfile.ArubaAutomatic.CustomProperties.DelegatedPassword;
                arubaModel.DelegatedUser = this.singleSignInfo.UserProfile.ArubaAutomatic.CustomProperties.DelegatedUser;
                arubaModel.OTPPassword = this.singleSignInfo.UserProfile.ArubaAutomatic.OTP;
                arubaModel.OTPAuthType = this.singleSignInfo.UserProfile.ArubaAutomatic.CustomProperties.OTPAuthType;
                arubaModel.User = this.singleSignInfo.UserProfile.ArubaAutomatic.CustomProperties.User;
                arubaModel.CertificateId = this.singleSignInfo.UserProfile.ArubaAutomatic.CustomProperties.CertificateId;
                arubaModel.RequestType = SignRequestType[this.changeCaseFirstLetter(this.documentFormatType.toLowerCase())];
                arubaModel.SignType = SignType.Automatic;
            }
            if (this.typeOfSign == "InfocertRemote") {
                var infocertModel = new ProxySignModel();
                infocertModel.Alias = this.singleSignInfo.UserProfile.InfocertRemote.Alias;
                var otpInput = this._ToolBar.findItemByValue("otpContainer2").findControl("proxyOtp");
                infocertModel.OTPPassword = otpInput.get_value();
                var pinInput = this._ToolBar.findItemByValue("pinContainer").findControl("pin");
                infocertModel.PINPassword = pinInput.get_value();
                infocertModel.SignType = SignType.Remote;
                infocertModel.RequestType = SignRequestType[this.changeCaseFirstLetter(this.documentFormatType.toLowerCase())];
                this.currentSignProperties.OTP = otpInput.get_value();
            }
            if (this.typeOfSign == "InfocertAutomatic") {
                var infocertModel = new ProxySignModel();
                infocertModel.Alias = this.singleSignInfo.UserProfile.InfocertAutomatic.Alias;
                var otpInput = this._ToolBar.findItemByValue("otpContainer2").findControl("proxyOtp");
                infocertModel.OTPPassword = otpInput.get_value();
                var pinInput = this._ToolBar.findItemByValue("pinContainer").findControl("pin");
                infocertModel.PINPassword = pinInput.get_value();
                infocertModel.SignType = SignType.Automatic;
                infocertModel.RequestType = SignRequestType[this.changeCaseFirstLetter(this.documentFormatType.toLowerCase())];
            }
            this._biblosDocuments.getDocumentsByChainId(chainID, function (data) {
                _this.documentsToSign = data;
                var serverFunction = "SubscribeStartWorkflow";
                var workflowReferenceBiblosModel = [];
                for (var i = 0; i < _this.documentsToSign.length; i++) {
                    workflowReferenceBiblosModel.push({
                        DocumentName: _this.documentsToSign[i].FileName,
                        ArchiveName: _this.documentsToSign[i].ArchiveSection,
                        ArchiveChainId: _this.documentsToSign[i].ChainId,
                        ArchiveDocumentId: _this.documentsToSign[i].DocumentId,
                        ReferenceModel: JSON.stringify(_this.currentSignProperties)
                    });
                }
                _this._signalR.sendServerMessages(serverFunction, _this.correlationId, JSON.stringify(workflowReferenceBiblosModel), 'workflow_integration', 'WorkflowStartRemoteSign', _this.onDoneSignalRSubscriptionCallback, _this.onErrorSignalRCallback);
            });
        };
        SingleSignRest.prototype.onDoneSignalRConnectionCallback = function () {
            var serverFunction = "SubscribeStartWorkflow";
            var requestOTPData = [];
            requestOTPData.push({ "ArchiveName": "", "ArchiveChainId": this.chainId, "DocumentName": "", ReferenceModel: JSON.stringify(this.currentSignProperties) });
            this._signalR.sendServerMessages(serverFunction, this.correlationId, JSON.stringify(requestOTPData), 'workflow_integration', 'WorkflowStartOTPRequest', this.onDoneSignalRSubscriptionCallback, this.onErrorSignalRCallback);
        };
        SingleSignRest.prototype.onDoneSignalRSubscriptionCallback = function () {
            console.log("Importazione avviata, a breve verranno visualizzate le attività realtive allo stato di importazione fatture.");
        };
        SingleSignRest.prototype.onErrorSignalRCallback = function (error) {
            console.log(error);
        };
        SingleSignRest.prototype.loadData = function (name) {
            var _this = this;
            this._service.getUserDetailBySystemUser(name, function (data) {
                if (!data)
                    return;
                _this.singleSignInfo = data[0];
                var defaultprovider = _this._enumHelper.getRemoteSignDescription(_this.singleSignInfo.UserProfile.DefaultProvider);
                var pin = _this._ToolBar.findItemByValue("pinContainer").findControl("pin");
                _this.currentSignProperties = _this.singleSignInfo.UserProfile[ProviderSignType[_this.singleSignInfo.UserProfile.DefaultProvider]];
                _this.currentSignProperties.CustomProperties.ProviderType = ProviderSignType[defaultprovider];
                _this.currentSignProperties.CustomProperties.RequestType = SignRequestType.Cades;
                var arubaAutomaticDescription = _this._enumHelper.getRemoteSignDescription(ProviderSignType.ArubaAutomatic);
                var arubaRemoteDescription = _this._enumHelper.getRemoteSignDescription(ProviderSignType.ArubaRemote);
                var infocertAutomaticDescription = _this._enumHelper.getRemoteSignDescription(ProviderSignType.InfocertAutomatic);
                var infocertRemoteDescription = _this._enumHelper.getRemoteSignDescription(ProviderSignType.InfocertRemote);
                if (_this.storageType == "Forever") {
                    if (defaultprovider == arubaRemoteDescription) {
                        pin.set_value(_this.singleSignInfo.UserProfile.ArubaRemote.PIN);
                    }
                    if (defaultprovider == infocertRemoteDescription) {
                        pin.set_value(_this.singleSignInfo.UserProfile.InfocertRemote.PIN);
                    }
                    if (defaultprovider == arubaAutomaticDescription) {
                        pin.set_value(_this.singleSignInfo.UserProfile.ArubaAutomatic.CustomProperties.DelegatedPassword);
                    }
                    if (defaultprovider == infocertAutomaticDescription) {
                        pin.set_value(_this.singleSignInfo.UserProfile.InfocertAutomatic.Password);
                    }
                }
                else if (_this.storageType == "Session")
                    pin.set_value(localStorage.getItem("DocumentSignPassword"));
            });
        };
        return SingleSignRest;
    }());
    return SingleSignRest;
});
//# sourceMappingURL=SingleSignRest.js.map