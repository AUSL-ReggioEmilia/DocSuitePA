/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />

import UserLogsService = require("App/Services/UserLogs/UserLogsService");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import EnumHelper = require("App/Helpers/EnumHelper");
import UserLogsModel = require("App/Models/UserLogs/UserLogsModel");
import ProviderSignType = require("App/Models/SignDocuments/ProviderSignType");
import GuidHelper = require("App/Helpers/GuidHelper");
import ArubaSignModel = require("App/Models/SignDocuments/ArubaSignModel");
import SignType = require("App/Models/SignDocuments/SignType");
import SignRequestType = require("App/Models/SignDocuments/SignRequestType");
import ProxySignModel = require("App/Models/SignDocuments/ProxySignModel");
import SignModel = require("App/Models/SignDocuments/SignModel");
import GenericHelper = require("App/Helpers/GenericHelper");
import WorkflowSignModel = require("App/Models/Workflows/WorkflowSignModel");
import BiblosDocumentsService = require("App/Services/Biblos/BiblosDocumentsService");
import DocumentModel = require("App/Models/Workflows/DocumentModel");
import WorkflowStartModel = require("App/Models/Workflows/WorkflowStartModel");
import WorkflowPropertyHelper = require("App/Models/Workflows/WorkflowPropertyHelper");
import ArgumentType = require("App/Models/Workflows/ArgumentType");

class SingleSignRest {
    typeOfSign: string;
    ToolBarId: string;
    storageType: string;
    dswSignalR: string;
    singleSignInfo: UserLogsModel;
    currentUserDomain: string;
    currentSignProperties: SignModel;
    correlationId: string;
    chainId: string;
    currentUserTenantName: string;
    currentUserTenantId: string;
    currentUserTenantAOOId: string;

    documentFormatType: string;
    documentsToSign: DocumentModel[];

    private _ToolBar: Telerik.Web.UI.RadToolBar;
    private _CAdES: Telerik.Web.UI.RadToolBarButton;
    private _PAdES: Telerik.Web.UI.RadToolBarButton;
    private _signalR: DSWSignalR;
    private _requestOTP: Telerik.Web.UI.RadToolBarButton;

    private _service: UserLogsService;
    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;
    private _biblosDocuments: BiblosDocumentsService;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
    }

    initialize(): void {
        this._ToolBar = <Telerik.Web.UI.RadToolBar>$find(this.ToolBarId);
        this._CAdES = this._ToolBar.findItemByValue("CAdES") as Telerik.Web.UI.RadToolBarButton;
        this._PAdES = this._ToolBar.findItemByValue("PAdES") as Telerik.Web.UI.RadToolBarButton;

        let serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "UserLog");
        this._service = new UserLogsService(serviceConfiguration);

        let biblosDocumentsServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "BiblosDocument");
        this._biblosDocuments = new BiblosDocumentsService(biblosDocumentsServiceConfiguration);

        let smartcardDescription: string = this._enumHelper.getRemoteSignDescription(ProviderSignType.Smartcard);
        let arubaAutomaticDescription: string = this._enumHelper.getRemoteSignDescription(ProviderSignType.ArubaAutomatic);
        let arubaRemoteDescription: string = this._enumHelper.getRemoteSignDescription(ProviderSignType.ArubaRemote);
        let infocertAutomaticDescription: string = this._enumHelper.getRemoteSignDescription(ProviderSignType.InfocertAutomatic);
        let infocertRemoteDescription: string = this._enumHelper.getRemoteSignDescription(ProviderSignType.InfocertRemote);

        this._requestOTP = this._ToolBar.findItemByValue("requestOtp") as Telerik.Web.UI.RadToolBarButton;

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
    }

    ToolBar_ButtonClick = (event: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        switch (args.get_item().get_value()) {
            case "requestOtp": {
                this._signalR.startConnection(() => this.onDoneSignalRConnectionCallback(), this.onErrorSignalRCallback);
                break;
            }
            case "PAdES":
            case "CAdES": {
                this.documentFormatType = args.get_item().get_value();
                break;
            }

            case "sign": {
                this._signalR.startConnection(() => this.signDocument(this.chainId), this.onErrorSignalRCallback);
            }
        }
    };

    private changeCaseFirstLetter(params) {
        if (typeof params === 'string') {
            return params.charAt(0).toUpperCase() + params.slice(1);
        }
        return null;
    }

    signDocument(chainID) {
        if (this.typeOfSign == "InfocertRemote") {
            alert("La funzionalità è disabilitata");
        }

        if (this.typeOfSign == "ArubaRemote") {
            let arubaModel: ArubaSignModel = new ArubaSignModel();

            let otpInput: Telerik.Web.UI.RadTextBox = this._ToolBar.findItemByValue("otpContainer2").findControl("proxyOtp") as Telerik.Web.UI.RadTextBox;
            arubaModel.OTPPassword = otpInput.get_value();

            arubaModel.OTPAuthType = this.singleSignInfo.UserProfile.ArubaRemote.CustomProperties.OTPAuthType;
            arubaModel.User = this.singleSignInfo.UserProfile.ArubaRemote.Alias;

            let pinInput: Telerik.Web.UI.RadTextBox = this._ToolBar.findItemByValue("pinContainer").findControl("pin") as Telerik.Web.UI.RadTextBox;
            arubaModel.UserPassword = pinInput.get_value();

            arubaModel.CertificateId = this.singleSignInfo.UserProfile.ArubaRemote.CustomProperties.CertificateId;
            arubaModel.SignType = SignType.Remote;
            arubaModel.RequestType = SignRequestType[this.changeCaseFirstLetter(this.documentFormatType.toLowerCase())];

            this.currentSignProperties.OTP = otpInput.get_value();
        }

        if (this.typeOfSign == "ArubaAutomatic") {
            let arubaModel: ArubaSignModel = new ArubaSignModel();

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
            let infocertModel: ProxySignModel = new ProxySignModel();

            infocertModel.Alias = this.singleSignInfo.UserProfile.InfocertRemote.Alias;

            let otpInput: Telerik.Web.UI.RadTextBox = this._ToolBar.findItemByValue("otpContainer2").findControl("proxyOtp") as Telerik.Web.UI.RadTextBox;
            infocertModel.OTPPassword = otpInput.get_value();

            let pinInput: Telerik.Web.UI.RadTextBox = this._ToolBar.findItemByValue("pinContainer").findControl("pin") as Telerik.Web.UI.RadTextBox;
            infocertModel.PINPassword = pinInput.get_value();

            infocertModel.SignType = SignType.Remote;
            infocertModel.RequestType = SignRequestType[this.changeCaseFirstLetter(this.documentFormatType.toLowerCase())];

            this.currentSignProperties.OTP = otpInput.get_value();
        }

        if (this.typeOfSign == "InfocertAutomatic") {
            let infocertModel: ProxySignModel = new ProxySignModel();

            infocertModel.Alias = this.singleSignInfo.UserProfile.InfocertAutomatic.Alias;

            let otpInput: Telerik.Web.UI.RadTextBox = this._ToolBar.findItemByValue("otpContainer2").findControl("proxyOtp") as Telerik.Web.UI.RadTextBox;
            infocertModel.OTPPassword = otpInput.get_value();

            let pinInput: Telerik.Web.UI.RadTextBox = this._ToolBar.findItemByValue("pinContainer").findControl("pin") as Telerik.Web.UI.RadTextBox;
            infocertModel.PINPassword = pinInput.get_value();

            infocertModel.SignType = SignType.Automatic;
            infocertModel.RequestType = SignRequestType[this.changeCaseFirstLetter(this.documentFormatType.toLowerCase())];
        }

        this._biblosDocuments.getDocumentsByChainId(chainID, (data) => {
            this.documentsToSign = data;
            let serverFunction = "SubscribeStartWorkflow";
            let workflowReferenceBiblosModel = [];

            for (let i = 0; i < this.documentsToSign.length; i++) {
                workflowReferenceBiblosModel.push({
                    DocumentName: this.documentsToSign[i].FileName,
                    ArchiveName: this.documentsToSign[i].ArchiveSection,
                    ArchiveChainId: this.documentsToSign[i].ChainId,
                    ArchiveDocumentId: this.documentsToSign[i].DocumentId,
                    ReferenceModel: JSON.stringify(this.currentSignProperties)
                });
            }

            let startImportModel: any = { "Documents": workflowReferenceBiblosModel };
            const serializedModel = JSON.stringify(startImportModel);

            var workflowName = "Avvia firma remota";
            var evt = {
                "WorkflowName": workflowName, "WorkflowAutoComplete": true, "EventModel": { "CustomProperties": {} }
            };
            evt.EventModel.CustomProperties["DocumentManagementRequestModel"] = serializedModel;

            var referenceModel = { "ReferenceId": this.correlationId, "ReferenceModel": JSON.stringify(evt) };

            let workflowStartModel: WorkflowStartModel = <WorkflowStartModel>{};
            workflowStartModel.WorkflowName = workflowName;
            workflowStartModel.Arguments = {};
            workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME] = { "PropertyType": ArgumentType.PropertyString, "Name": WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME, "ValueString": "Firma remota" };
            workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID] = { "PropertyType": ArgumentType.PropertyGuid, "Name": WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, "ValueGuid": this.currentUserTenantId };
            workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID] = { "PropertyType": ArgumentType.PropertyGuid, "Name": WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, "ValueGuid": this.currentUserTenantAOOId };
            workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME] = { "PropertyType": ArgumentType.PropertyString, "Name": WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, "ValueString": this.currentUserTenantName };
            workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL] = { "PropertyType": ArgumentType.Json, "Name": WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "ValueString": JSON.stringify(referenceModel) };

            this._signalR.sendServerMessages(serverFunction, this.correlationId, JSON.stringify(workflowStartModel), this.onDoneSignalRSubscriptionCallback, this.onErrorSignalRCallback);
        });
    }



    onDoneSignalRConnectionCallback() {
        let serverFunction = "SubscribeStartWorkflow";
        var requestOTPData: any = [];
        requestOTPData.push({ "ArchiveName": "", "ArchiveChainId": this.chainId, "DocumentName": "", ReferenceModel: JSON.stringify(this.currentSignProperties) });

        let startImportModel: any = { "Documents": requestOTPData };
        const serializedModel = JSON.stringify(startImportModel);

        var workflowName = "Avvio richiesta OTP";
        var evt = {
            "WorkflowName": workflowName, "WorkflowAutoComplete": true, "EventModel": { "CustomProperties": {} }
        };
        evt.EventModel.CustomProperties["DocumentManagementRequestModel"] = serializedModel;

        var referenceModel = { "ReferenceId": this.correlationId, "ReferenceModel": JSON.stringify(evt) };

        let workflowStartModel: WorkflowStartModel = <WorkflowStartModel>{};
        workflowStartModel.WorkflowName = workflowName;
        workflowStartModel.Arguments = {};
        workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME] = { "PropertyType": ArgumentType.PropertyString, "Name": WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME, "ValueString": "Richiesta OTP" };
        workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID] = { "PropertyType": ArgumentType.PropertyGuid, "Name": WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, "ValueGuid": this.currentUserTenantId };
        workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID] = { "PropertyType": ArgumentType.PropertyGuid, "Name": WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, "ValueGuid": this.currentUserTenantAOOId };
        workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME] = { "PropertyType": ArgumentType.PropertyString, "Name": WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, "ValueString": this.currentUserTenantName };
        workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL] = { "PropertyType": ArgumentType.Json, "Name": WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, "ValueString": JSON.stringify(referenceModel) };

        this._signalR.sendServerMessages(serverFunction, this.correlationId, JSON.stringify(workflowStartModel), this.onDoneSignalRSubscriptionCallback, this.onErrorSignalRCallback);
    }

    onDoneSignalRSubscriptionCallback() {
        console.log("Importazione avviata, a breve verranno visualizzate le attività realtive allo stato di importazione fatture.");
    }

    onErrorSignalRCallback(error) {
        console.log(error);
    }

    private loadData(name: string) {
        this._service.getUserDetailBySystemUser(name, (data) => {
            if (!data) return;

            this.singleSignInfo = data[0];

            let defaultprovider: string = this._enumHelper.getRemoteSignDescription(this.singleSignInfo.UserProfile.DefaultProvider);
            let pin: Telerik.Web.UI.RadTextBox = this._ToolBar.findItemByValue("pinContainer").findControl("pin") as Telerik.Web.UI.RadTextBox;

            this.currentSignProperties = this.singleSignInfo.UserProfile[ProviderSignType[this.singleSignInfo.UserProfile.DefaultProvider]];
            this.currentSignProperties.CustomProperties.ProviderType = ProviderSignType[defaultprovider];
            this.currentSignProperties.CustomProperties.RequestType = SignRequestType.Cades;

            let arubaAutomaticDescription: string = this._enumHelper.getRemoteSignDescription(ProviderSignType.ArubaAutomatic);
            let arubaRemoteDescription: string = this._enumHelper.getRemoteSignDescription(ProviderSignType.ArubaRemote);
            let infocertAutomaticDescription: string = this._enumHelper.getRemoteSignDescription(ProviderSignType.InfocertAutomatic);
            let infocertRemoteDescription: string = this._enumHelper.getRemoteSignDescription(ProviderSignType.InfocertRemote);

            if (this.storageType == "Forever") {
                if (defaultprovider == arubaRemoteDescription) {
                    pin.set_value(this.singleSignInfo.UserProfile.ArubaRemote.PIN);
                }

                if (defaultprovider == infocertRemoteDescription) {
                    pin.set_value(this.singleSignInfo.UserProfile.InfocertRemote.PIN);
                }

                if (defaultprovider == arubaAutomaticDescription) {
                    pin.set_value(this.singleSignInfo.UserProfile.ArubaAutomatic.CustomProperties.DelegatedPassword);
                }

                if (defaultprovider == infocertAutomaticDescription) {
                    pin.set_value(this.singleSignInfo.UserProfile.InfocertAutomatic.Password);
                }
            } else if (this.storageType == "Session")
                pin.set_value(localStorage.getItem("DocumentSignPassword"));
        })
    }
}

export = SingleSignRest;