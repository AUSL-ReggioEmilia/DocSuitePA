import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');
import GuidHelper = require("App/Helpers/GuidHelper");
import FileHelper = require('App/Helpers/FileHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import WorkflowPropertyHelper = require('App/Models/Workflows/WorkflowPropertyHelper');
import BiblosDocumentsService = require("App/Services/Biblos/BiblosDocumentsService");
import SignalRService = require("App/Services/SignalR/SignalRService");
import BuilderService = require("App/Services/Builders/BuilderService");
import UserLogsService = require("App/Services/UserLogs/UserLogsService");
import DocumentModel = require("App/Models/DgrooveSigns/DocumentModel");
import SignOptionsModel = require("App/Models/DgrooveSigns/SignOptionsModel");
import UserProfileModel = require("App/Models/UserLogs/UserProfileModel");
import UserLogsModel = require("App/Models/UserLogs/UserLogsModel");
import WorkflowStorage = require('App/Core/WorkflowStorage/WorkflowStorage');
import SignRequestType = require("App/Models/SignDocuments/SignRequestType");
import UDSConstants = require('App/Core/UDS/UDSConstants');
import BuildActionModel = require('App/Models/Commons/BuildActionModel');
import BuildActionType = require('App/Models/Commons/BuildActionType');
import BuildArchiveDocumentModel = require('App/Models/Commons/BuildArchiveDocumentModel');
import ReferenceBuildModelType = require('App/Models/Commons/ReferenceBuildModelType');
import SynchronizeCollaborationModel = require('App/Models/Commons/SynchronizeCollaborationModel');
import SignType = require("App/Models/DgrooveSigns/SignType");
import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import DocumentRootFolderViewModel = require("App/ViewModels/SignDocuments/DocumentRootFolderViewModel");
import DocumentFolderViewModel = require("App/ViewModels/SignDocuments/DocumentFolderViewModel");
import DocumentViewModel = require("App/ViewModels/SignDocuments/DocumentViewModel");
import EvaluateCollaborationModel = require('App/Models/Commons/EvaluateCollaborationModel');
import EvaluateCollaborationDocumentModel = require('App/Models/Commons/EvaluateCollaborationDocumentModel');
import DocumentSignBehaviour = require('App/ViewModels/SignDocuments/DocumentSignBehaviour');
import EvaluateFascicleModel = require('App/Models/Commons/EvaluateFascicleModel');
import WorkflowStartModel = require('App/Models/Workflows/WorkflowStartModel');
import WorkflowReferenceModel = require('App/Models/Workflows/WorkflowReferenceModel');
import WorkflowReferenceBiblosModel = require('App/Models/Workflows/WorkflowReferenceBiblosModel');
import DocSuiteEvent = require('App/Models/Workflows/DocSuiteEvent');
import DSWEnvironment = require('App/Models/Environment');
import ArgumentType = require("App/Models/Workflows/ArgumentType");
import WorkflowProperty = require('App/Models/Workflows/WorkflowProperty');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import DocSuiteModel = require('App/Models/Workflows/DocSuiteModel');
import ChainType = require('App/Models/DocumentUnits/ChainType');
import WorkflowActivityService = require('App/Services/Workflows/WorkflowActivityService');
import WorkflowActivityModel = require('App/Models/Workflows/WorkflowActivityModel');
import WorkflowPropertyService = require('App/Services/Workflows/WorkflowPropertyService');
import WorkflowPropertyModel = require('App/Models/Workflows/WorkflowProperty');
import WorkflowDocumentModel = require('App/Models/Workflows/WorkflowDocumentModel');
import ProviderSignType = require('App/Models/SignDocuments/ProviderSignType');

class DgrooveSigns {
    currentUserDomain: string;
    currentUserTenantName: string;
    currentUserTenantId: string;
    currentUserTenantAOOId: string;
    signalrDgrooveSignerUrl: string;
    signalrWebApiUrl: string;
    dswBaseUrl: string;
    hasAruba: string;
    hasInfocert: string;
    workflowArchiveName: string;
    collaborationArchiveName: string;
    signToolBarId: string;
    documentsTreeId: string;
    documentViewerId: string;
    ajaxLoadingPanelId: string;
    pnlMainContentId: string;
    uscNotificationId: string;
    docTreePaneId: string;
    radWindowManagerSigner: string;
    radListMessagesId: string;
    radNotificationInfoId: string;
    notificationSignToolBarId: string;
    defaultSignType: string;

    private _signBtn: Telerik.Web.UI.RadToolBarButton;
    private _otpBtn: Telerik.Web.UI.RadToolBarButton;
    private _signToolBar: Telerik.Web.UI.RadToolBar;
    private _providerSignTypeDdl: Telerik.Web.UI.RadDropDownList;
    private _optInputText: Telerik.Web.UI.RadTextBox;
    private _radListMessages: Telerik.Web.UI.RadListBox;
    private _documentsTreeView: Telerik.Web.UI.RadTreeView;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _notificationInfo: Telerik.Web.UI.RadNotification;
    private _notificationSignToolBar: Telerik.Web.UI.RadToolBar;
    private _notificationOptInputText: Telerik.Web.UI.RadTextBox;
    private _notificationSignBtn: Telerik.Web.UI.RadToolBarButton;
    private _notificationOtpBtn: Telerik.Web.UI.RadToolBarButton;

    private _documentViewer: any;
    private _transactionId: string;
    private _correlationIdSignalrWebApi: string;
    private _hubConnectionDgrooveSigner: boolean;
    private _redirectUrl: string;
    private _documents: DocumentRootFolderViewModel[];
    private _documentsSignOptions: { [key: string]: SignOptionsModel };
    private _uploadedDocuments: DocumentViewModel[];
    private _collaborationToSynchronize: SynchronizeCollaborationModel;
    private _signType: SignType;
    private _webApiError: boolean;
    private _signalRError: boolean;
    private _userProfileModel: UserProfileModel;
    private _workflowStartModel: WorkflowStartModel;
    private _documentsToSign: DocumentViewModel[];
    private _moreToSign: boolean;
    private _indexFirstDocumentToSign: number;

    private _ajaxPromises: JQueryPromise<void>[];

    private _serviceConfigurations: ServiceConfiguration[];
    private _biblosDocumentsService: BiblosDocumentsService;
    private _builderService: BuilderService;
    private _signalRService: SignalRService;
    private _signalRService_WebApi: DSWSignalR;
    private _logsService: UserLogsService;
    private _workflowActivityService: WorkflowActivityService;
    private _workflowPropertyService: WorkflowPropertyService;

    private wstorage: WorkflowStorage;
    private wStorageEnabled: boolean = false;

    private static IMG_NODE_FOLDER = "../App_Themes/DocSuite2008/imgset16/folder.png";
    private static IMG_SIGNALR_ERROR = "../App_Themes/DocSuite2008/imgset16/error16.png";
    private static IMG_SIGNALR_CONNECTING = "../App_Themes/DocSuite2008/imgset16/error.png";
    private static IMG_SIGNALR_CONNECTED = "../App_Themes/DocSuite2008/imgset16/text_signature.png";

    //Toolbar buttons
    private static SIGNATURE_CADES = "cades";
    private static SIGNATURE_PADES = "pades";
    private static SELECT_ALL = "selectAll";
    private static SELECT_INVERT = "selectInvert";
    private static SIGN = "sign";
    private static NEXT = "next";
    private static COMMENT = "comment";
    private static PROVIDER_SIGN_TYPE = "providerSignType";
    private static OTP = "otp";
    private static OTPINPUT = "otpInput";
    private static NOTIFICATION_OTPINPUT = "notificationOtpInput";
    private static SMARTCARD = "0";

    //Attributes
    private static ATTRIBUTE_OTP = "otp";
    private static ATTRIBUTE_LINK = "link";
    private static ATTRIBUTE_NODEDATA = "nodeData";
    private static ATTRIBUTE_DOCUMENT = "isDocument";
    private static ATTRIBUTE_MAX_DOC = "maxDocuments";
    private static MAX_DOC_ARUBA_REMOTE = 15;

    //SignalR messages (smartcard)
    private static HUB_MSG_CONNECTED = "connected";
    private static HUB_MSG_LOGGING = "logging";
    private static HUB_MSG_SIGNED = "signedDocument";
    private static HUB_MSG_CLOSE = "closedTransaction";

    //SignalR (smartcard) error codes:
    private static SIGNAL_GENERIC_ERROR = 1000;
    private static SIGNAL_SIGN_ERROR = 1001;
    private static SIGNAL_DOCUMENTNOTVALID_ERROR = 1002;

    //SignalR WebAPI
    private static HUB_NAME = "WorkflowHub";

    private _documentsTreeNodes(): Telerik.Web.UI.RadTreeNode[] {
        return this._documentsTreeView.get_allNodes();
    }

    private _documentsNodes(checked: boolean = null, mandatorySelectable: boolean = null): Telerik.Web.UI.RadTreeNode[] {
        let nodes: Telerik.Web.UI.RadTreeNode[] = [];

        this._documentsTreeNodes().forEach((node: Telerik.Web.UI.RadTreeNode) => {
            let isDocument: boolean = node.get_attributes().getAttribute(DgrooveSigns.ATTRIBUTE_DOCUMENT);
            let nodeData: DocumentViewModel = node.get_attributes().getAttribute(DgrooveSigns.ATTRIBUTE_NODEDATA);

            if (isDocument === false || (checked != null && node.get_checked() != checked) || (mandatorySelectable != null && nodeData.Mandatory === true && nodeData.MandatorySelectable != mandatorySelectable)) {
                return;
            }

            nodes.push(node);
        });

        return nodes;
    }

    private _getNextDocumentsToSign(): DocumentViewModel[] {
        let maxDocuments: number = this._providerSignTypeDdl.get_selectedItem().get_attributes().getAttribute(DgrooveSigns.ATTRIBUTE_MAX_DOC);
        let documentToSign: DocumentViewModel[] = [];

        this._documentsNodes(true).forEach((node: Telerik.Web.UI.RadTreeNode) => {
            let nodeData: DocumentViewModel = node.get_attributes().getAttribute(DgrooveSigns.ATTRIBUTE_NODEDATA);
            documentToSign.push(nodeData);
        });

        if (!maxDocuments) {
            this._moreToSign = false;
            return documentToSign;
        }

        this._moreToSign = this._indexFirstDocumentToSign + maxDocuments < documentToSign.length;

        return documentToSign.slice(this._indexFirstDocumentToSign, this._indexFirstDocumentToSign + maxDocuments);
    }

    private _selectedProviderSignTypeOtpAttribute(): boolean {
        return this._providerSignTypeDdl.get_selectedItem().get_attributes().getAttribute(DgrooveSigns.ATTRIBUTE_OTP);
    }

    private _cachedDocuments(): DocumentRootFolderViewModel[] {
        let documents: DocumentRootFolderViewModel[] = null;
        if (sessionStorage[SessionStorageKeysHelper.SESSION_KEY_DOCS_TO_SIGN]) {
            documents = JSON.parse(sessionStorage[SessionStorageKeysHelper.SESSION_KEY_DOCS_TO_SIGN]);
        }
        return documents;
    }

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    signToolBar_ButtonClicked = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        let item: Telerik.Web.UI.RadToolBarItem = args.get_item();
        if (item) {
            switch (item.get_value()) {
                case DgrooveSigns.SIGNATURE_CADES: {
                    this._signType = SignType.CAdES;
                    break;
                }
                case DgrooveSigns.SIGNATURE_PADES: {
                    this._signType = SignType.PAdES;
                    this._documentsTreeNodes().forEach((node: Telerik.Web.UI.RadTreeNode) => {
                        let level = node.get_level();
                        if (level === 0) {
                            let nodeData: DocumentRootFolderViewModel = node.get_attributes().getAttribute("nodeData");
                            if (!nodeData.MainDocumentIsPadesCompliant && nodeData.SignBehaviour === DocumentSignBehaviour.Collaboration) {
                                node.set_checked(false);
                            }
                        }
                        if (level === 2) {
                            let nodeData: DocumentViewModel = node.get_attributes().getAttribute("nodeData");
                            if (!nodeData.PadesCompliant) {
                                node.set_checked(false);
                            }
                        }
                    });
                    break;
                }
                case DgrooveSigns.SELECT_ALL: {
                    let nodesChecked: boolean = false;

                    this._documentsNodes(null, true).forEach((node: Telerik.Web.UI.RadTreeNode) => {
                        if (node.get_checked()) {
                            this._documentsTreeNodes().forEach((node: Telerik.Web.UI.RadTreeNode) => node.set_checked(false));
                            nodesChecked = true;
                            return;
                        }
                    });

                    if (nodesChecked === false) {
                        this._documentsTreeNodes().forEach((node: Telerik.Web.UI.RadTreeNode) => node.set_checked(true));
                    }

                    this._setMandatoryDocuments();
                    break;
                }
                case DgrooveSigns.SELECT_INVERT: {
                    this._documentsNodes().forEach((node: Telerik.Web.UI.RadTreeNode) => {
                        if (node.get_checked()) {
                            node.set_checked(false);
                        }
                        else {
                            node.set_checked(true);
                        }

                        this._setMandatoryDocuments();
                    });
                    break;
                }
                case DgrooveSigns.OTP: {
                    this._executeRequestOTP();
                    break;
                }
                case DgrooveSigns.SIGN: {
                    if (this._signType == null) {
                        this.showWarningMessage("E' necessario selezionare il tipo di firma.");
                        return;
                    }
                    if (this._documentsNodes(true).length === 0) {
                        this.showWarningMessage("E' necessario selezionare almeno un documento.");
                        return;
                    }

                    let otp: string = this._optInputText.get_value();
                    if (this._selectedProviderSignTypeOtpAttribute() && !otp) {
                        this.showWarningMessage("E' necessario inserire l'OTP.");
                        return;
                    }

                    if (this._smartCardSelected()) {
                        this._executeSignSmartCard();
                    } else {
                        this._radListMessages.get_items().clear();
                        this._executeSignRemote(otp);
                    }

                    break;
                }
            }
        }
    }

    notificationSignToolBar_ButtonClicked = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        let item: Telerik.Web.UI.RadToolBarItem = args.get_item();
        if (item) {
            switch (item.get_value()) {
                case DgrooveSigns.OTP: {
                    this._executeRequestOTP();
                    break;
                }
                case DgrooveSigns.SIGN: {
                    if (this._signType == null) {
                        this.showWarningMessage("E' necessario selezionare il tipo di firma.");
                        return;
                    }
                    if (this._documentsNodes(true).length === 0) {
                        this.showWarningMessage("E' necessario selezionare almeno un documento.");
                        return;
                    }

                    let otp: string = this._notificationOptInputText.get_value();
                    if (this._selectedProviderSignTypeOtpAttribute() && !otp) {
                        this.showWarningMessage("E' necessario inserire l'OTP.");
                        return;
                    }

                    this._executeSignRemote(otp);

                    break;
                }
            }
        }
    }

    treeView_ClientNodeClicking = (sender: Telerik.Web.UI.RadTreeView, eventArgs: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        let nodeClicked: Telerik.Web.UI.RadTreeNode = eventArgs.get_node()

        if (nodeClicked === undefined) {
            return;
        }

        if (nodeClicked.get_attributes().getAttribute(DgrooveSigns.ATTRIBUTE_DOCUMENT) === true) {
            if (nodeClicked.get_checked() === false) {
                nodeClicked.set_checked(true);
            }

            let documentLink = nodeClicked.get_attributes().getAttribute(DgrooveSigns.ATTRIBUTE_LINK);
            this._documentViewer.fromFile(documentLink);
        }
    }

    treeView_ClientNodeChecked = (sender: Telerik.Web.UI.RadTreeView, eventArgs: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        this._documentsNodes(true, null).forEach((node: Telerik.Web.UI.RadTreeNode) => {
            let nodeData: DocumentViewModel = node.get_attributes().getAttribute("nodeData");
            if (!nodeData.PadesCompliant && this._signType === SignType.PAdES) {
                let btn = this._signToolBar.findItemByValue(DgrooveSigns.SIGNATURE_CADES) as Telerik.Web.UI.RadToolBarButton;
                btn.set_checked(true);
                this._signType = SignType.CAdES;
            }
        });
        this._setMandatoryDocuments();
    }

    signType_SelectedIndexChanged = (sender: Telerik.Web.UI.RadDropDownList, eventArgs: Telerik.Web.UI.DropDownListIndexChangedEventArgs) => {
        this._btnSignEnable();

        if (this._smartCardSelected()) {
            this._initializeSignalR_DgrooveSigner();
        }

        if (sender.get_selectedItem().get_attributes().getAttribute(DgrooveSigns.ATTRIBUTE_OTP)) {
            this._otpBtn.set_visible(true);
            this._optInputText.set_value("");
            this._optInputText.set_visible(true);
        }
        else {
            this._otpBtn.set_visible(false);
            this._optInputText.set_value("");
            this._optInputText.set_visible(false);
        }
    }

    initialize(): void {
        this._initializeServices();
        this._initializeComponents();
        this._initializeSignTypeDropDown();
        this._initializeTreeView();
        this._initializeSignPageDefault();
    }

    private _initializeComponents(): void {
        this._signToolBar = <Telerik.Web.UI.RadToolBar>$find(this.signToolBarId);
        this._signToolBar.add_buttonClicked(this.signToolBar_ButtonClicked);
        this._signBtn = this._signToolBar.findItemByValue(DgrooveSigns.SIGN) as Telerik.Web.UI.RadToolBarButton;
        this._otpBtn = this._signToolBar.findItemByValue(DgrooveSigns.OTP) as Telerik.Web.UI.RadToolBarButton;
        this._optInputText = this._signToolBar.findItemByValue(DgrooveSigns.OTPINPUT).findControl(DgrooveSigns.OTPINPUT) as Telerik.Web.UI.RadTextBox;
        this._documentsTreeView = <Telerik.Web.UI.RadTreeView>$find(this.documentsTreeId);
        this._documentViewer = <any>$find(this.documentViewerId);
        this._documentsTreeView.add_nodeClicked(this.treeView_ClientNodeClicking);
        this._documentsTreeView.add_nodeChecked(this.treeView_ClientNodeChecked);
        this._loadingPanel = $find(this.ajaxLoadingPanelId) as Telerik.Web.UI.RadAjaxLoadingPanel;
        this._radListMessages = <Telerik.Web.UI.RadListBox>$find(this.radListMessagesId);
        this._notificationInfo = <Telerik.Web.UI.RadNotification>$find(this.radNotificationInfoId);
        this._notificationSignToolBar = <Telerik.Web.UI.RadToolBar>$find(this.notificationSignToolBarId);
        this._notificationSignToolBar.add_buttonClicked(this.notificationSignToolBar_ButtonClicked);
        this._notificationOptInputText = this._notificationSignToolBar.findItemByValue(DgrooveSigns.NOTIFICATION_OTPINPUT).findControl(DgrooveSigns.NOTIFICATION_OTPINPUT) as Telerik.Web.UI.RadTextBox;
        this._notificationSignBtn = this._notificationSignToolBar.findItemByValue(DgrooveSigns.SIGN) as Telerik.Web.UI.RadToolBarButton;
        this._notificationOtpBtn = this._notificationSignToolBar.findItemByValue(DgrooveSigns.OTP) as Telerik.Web.UI.RadToolBarButton;
    }

    private _initializeSignTypeDropDown(): void {
        this._providerSignTypeDdl = this._signToolBar.findItemByValue(DgrooveSigns.PROVIDER_SIGN_TYPE).findControl(DgrooveSigns.PROVIDER_SIGN_TYPE) as Telerik.Web.UI.RadDropDownList;
        this._providerSignTypeDdl.add_selectedIndexChanged(this.signType_SelectedIndexChanged);
        this._providerSignTypeDdl.getItem(0).select();

        if (this.hasAruba === "False" && this.hasInfocert === "False")
            return;

        this._logsService.getCurrentUserDetailSecure((data: UserLogsModel) => {
            if (!data)
                return;

            this._userProfileModel = data.UserProfile;

            if (this.hasAruba === "True") {
                let arubaRemote = new Telerik.Web.UI.DropDownListItem();
                arubaRemote.set_text("Remota Aruba");
                arubaRemote.set_value("1");
                arubaRemote.get_attributes().setAttribute(DgrooveSigns.ATTRIBUTE_OTP, true);
                arubaRemote.get_attributes().setAttribute(DgrooveSigns.ATTRIBUTE_MAX_DOC, DgrooveSigns.MAX_DOC_ARUBA_REMOTE);
                arubaRemote.set_enabled(this._userProfileModel != null && this._userProfileModel.ArubaRemote != null);

                let arubaAutomatic = new Telerik.Web.UI.DropDownListItem();
                arubaAutomatic.set_text("Automatica Aruba");
                arubaAutomatic.set_value("3");
                arubaAutomatic.set_enabled(this._userProfileModel != null && this._userProfileModel.ArubaAutomatic != null);                

                this._providerSignTypeDdl.get_items().add(arubaRemote);
                this._providerSignTypeDdl.get_items().add(arubaAutomatic);
            }

            if (this.hasInfocert === "True") {
                let inforcertRemote = new Telerik.Web.UI.DropDownListItem();
                inforcertRemote.set_text("Remota Infocert");
                inforcertRemote.set_value("2");
                inforcertRemote.get_attributes().setAttribute(DgrooveSigns.ATTRIBUTE_OTP, true);
                inforcertRemote.set_enabled(this._userProfileModel != null && this._userProfileModel.InfocertRemote != null);                

                let infocertAutomatic = new Telerik.Web.UI.DropDownListItem();
                infocertAutomatic.set_text("Automatica Infocert");
                infocertAutomatic.set_value("4");
                infocertAutomatic.set_enabled(this._userProfileModel != null && this._userProfileModel.InfocertAutomatic != null);                

                //TODO: in attesa di verifiche con infocert
                //this._providerSignTypeDdl.get_items().add(inforcertRemote);
                //this._providerSignTypeDdl.get_items().add(infocertAutomatic);
            }

            let defaultProvider: ProviderSignType = this.getDefaultSignTypeProvider(data.UserProfile);
            let signTypeItem: Telerik.Web.UI.DropDownListItem = this._providerSignTypeDdl.findItemByValue(defaultProvider.toString());
            if (signTypeItem) {
                signTypeItem.select();
            }
        })
    }

    private _initializeTreeView(): void {
        this._documents = this._cachedDocuments();

        this._documents.forEach((documentRootFolder: DocumentRootFolderViewModel) => {
            let docRootNode: Telerik.Web.UI.RadTreeNode = this._createTreeNode(documentRootFolder.Name, DgrooveSigns.IMG_NODE_FOLDER, false, documentRootFolder);

            documentRootFolder.DocumentFolders.forEach((documentFolder: DocumentFolderViewModel) => {
                let folderNode: Telerik.Web.UI.RadTreeNode = this._createTreeNode(documentFolder.Name, DgrooveSigns.IMG_NODE_FOLDER, false, documentFolder);

                documentFolder.Documents.forEach((document: DocumentViewModel) => {
                    let img: string = FileHelper.getImageByFileName(document.Name, true)
                    let documentNode: Telerik.Web.UI.RadTreeNode = this._createTreeNode(document.Name, img, true, document);
                    documentNode.get_attributes().setAttribute(DgrooveSigns.ATTRIBUTE_LINK, this._createDocumentLink(document));

                    folderNode.get_nodes().add(documentNode);
                });

                docRootNode.get_nodes().add(folderNode);
            });

            this._documentsTreeView.get_nodes().add(docRootNode);
        });

        this._documentsNodes().forEach((node: Telerik.Web.UI.RadTreeNode) => {
            let nodeData: DocumentViewModel = node.get_attributes().getAttribute(DgrooveSigns.ATTRIBUTE_NODEDATA);

            if (nodeData.Mandatory === true) {
                node.set_checked(true);
            }
        });
    }

    private _initializeSignalR_DgrooveSigner(): void {
        if (this._hubConnectionDgrooveSigner) {
            return;
        }

        this._btnSignWaitingConnection();

        this._webApiError = false;
        this._hubConnectionDgrooveSigner = false;

        this._transactionId = GuidHelper.newGuid();
        this._signalRService = new SignalRService(this.signalrDgrooveSignerUrl, `transactionId=${this._transactionId}`);

        this._signalRService.registerOnClose(() => {
            this._btnSignDisabled();
        });

        this._signalRService.startConnection(
            () => {
                this._hubConnectionDgrooveSigner = true;
                this._signalRService.registerClientMessage(DgrooveSigns.HUB_MSG_CONNECTED, this.onSignalR_Connected);
                this._signalRService.registerClientMessage(DgrooveSigns.HUB_MSG_LOGGING, this.onSignalR_Logging);
                this._signalRService.registerClientMessage(DgrooveSigns.HUB_MSG_SIGNED, this.onSignalR_SignedDocument);
                this._signalRService.registerClientMessage(DgrooveSigns.HUB_MSG_CLOSE, this.onSignalR_ClosedTransaction);

                if (this._smartCardSelected() === false) {
                    return;
                }

                this._btnSignEnable();
            },
            (exception) => {
                console.log(exception);
                this._hubConnectionDgrooveSigner = false;

                if (this._smartCardSelected() === false) {
                    return;
                }

                this._btnSignDisabled();
            });
    }

    private _initializeWorkflowStorage(): void {
        try {
            this.wstorage = new WorkflowStorage();
            if (this.wstorage.IsValid) {
                this.wStorageEnabled = true;
            }
        }
        catch (err) {
            this.wStorageEnabled = false;
            window.alert("Questa funzionalità non è supportata con l'attuale browser. E' necessario utilizzare un browser moderno come IE10+, Edge o Chrome");
        }
    }

    private _onWebApiError(exception: ExceptionDTO): void {
        this._loadingPanel.hide(this.pnlMainContentId);
        if (this._signalRService) {
            this._signalRService.stopConnection();
            this._transactionId = null;
        }

        this._signBtn.set_imageUrl(DgrooveSigns.IMG_SIGNALR_ERROR);
        this._signBtn.set_toolTip("Errore durante il processo di firma.")
        this._signBtn.set_enabled(false);
        this._webApiError = true;
        this.showNotificationException(exception);
    }

    private _initializeServices(): void {
        let biblosDocumentsServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "BiblosDocument");
        this._biblosDocumentsService = new BiblosDocumentsService(biblosDocumentsServiceConfiguration);

        let builderServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "BuildActionModel");
        this._builderService = new BuilderService(builderServiceConfiguration);

        let logsServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "UserLog");
        this._logsService = new UserLogsService(logsServiceConfiguration);

        let workflowActivityConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowActivity");
        this._workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);

        let workflowPropertyConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowProperty");
        this._workflowPropertyService = new WorkflowPropertyService(workflowPropertyConfiguration);
    }

    private _initializeSignPageDefault(): void {
        //Documenti nel session storage
        sessionStorage[SessionStorageKeysHelper.SESSION_KEY_SIGNED_DOC] = "";
        sessionStorage[SessionStorageKeysHelper.SESSION_KEY_CORRELATION_ID] = "";

        //Modalità di firma: defualt Smartcard e otp non visibile
        this._otpBtn.set_visible(false);
        this._optInputText.set_visible(false);
        this._optInputText.set_value("");        

        //Tipo di firma: default Cades
        let btnCadES = this._signToolBar.findItemByValue(DgrooveSigns.SIGNATURE_CADES) as Telerik.Web.UI.RadToolBarButton;
        let btnPadES = this._signToolBar.findItemByValue(DgrooveSigns.SIGNATURE_PADES) as Telerik.Web.UI.RadToolBarButton;
        btnCadES.set_checked(true);
        this._signType = SignType.CAdES;

        if (this._documents[0].DocumentFolders[0].Documents[0].PadesCompliant
            && this.defaultSignType != null
            && this.defaultSignType.toLowerCase() == SignType[SignType.PAdES].toLowerCase()) {

            btnCadES.set_checked(false);
            btnPadES.set_checked(true);
            this._signType = SignType.PAdES;
        }
        this._initializeSignalR_DgrooveSigner();

        //Errori chiamate webapi
        this._webApiError = false;

        //Default numero massimo documenti da firmare
        this._notificationOptInputText.set_visible(false);
        this._notificationOtpBtn.set_visible(false);
        this._notificationSignBtn.set_visible(false);
        this._indexFirstDocumentToSign = 0;
        this._moreToSign = false;

        //Enviroment di provenienza
        switch (this._documents[0].SignBehaviour) {
            case DocumentSignBehaviour.Collaboration: {
                //Redirect URL
                this._redirectUrl = `../User/UserCollRisultati.aspx?Title=Da visionare / firmare&Action=CF`;

                //Visibilità pulsanti toolbar
                (this._signToolBar.findItemByValue(DgrooveSigns.NEXT) as Telerik.Web.UI.RadToolBarButton).set_visible(true);
                (this._signToolBar.findItemByValue(DgrooveSigns.COMMENT) as Telerik.Web.UI.RadToolBarButton).set_visible(this._documents[0].CommentVisibile);
                (this._signToolBar.findItemByValue(DgrooveSigns.COMMENT) as Telerik.Web.UI.RadToolBarButton).set_checked(this._documents[0].CommentChecked);

                break;
            }
            case DocumentSignBehaviour.Fascicle: {
                //Redirect URL
                this._redirectUrl = `../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=${this._documents[0].UniqueId}`;

                //Visibilità pulsanti toolbar
                (this._signToolBar.findItemByValue(DgrooveSigns.NEXT) as Telerik.Web.UI.RadToolBarButton).set_visible(false);
                (this._signToolBar.findItemByValue(DgrooveSigns.COMMENT) as Telerik.Web.UI.RadToolBarButton).set_visible(false);

                break;
            }

            case DocumentSignBehaviour.CollaborationDocumentUpload: {
                //Redirect URL
                this._redirectUrl = this._documents[0].PageUrl;

                //Visibilità pulsanti toolbar
                (this._signToolBar.findItemByValue(DgrooveSigns.NEXT) as Telerik.Web.UI.RadToolBarButton).set_visible(false);
                (this._signToolBar.findItemByValue(DgrooveSigns.COMMENT) as Telerik.Web.UI.RadToolBarButton).set_visible(this._documents[0].CommentVisibile);
                (this._signToolBar.findItemByValue(DgrooveSigns.COMMENT) as Telerik.Web.UI.RadToolBarButton).set_checked(this._documents[0].CommentChecked);
                (this._signToolBar.findItemByValue(DgrooveSigns.SIGNATURE_PADES) as Telerik.Web.UI.RadToolBarButton).set_visible(this._documents[0].DocumentFolders[0].Documents[0].PadesCompliant);

                break;
            }

            case DocumentSignBehaviour.DocumentUploadTempFolder: {
                //Redirect URL
                this._redirectUrl = null;

                //Visibilità pulsanti toolbar
                (this._signToolBar.findItemByValue(DgrooveSigns.NEXT) as Telerik.Web.UI.RadToolBarButton).set_visible(false);
                (this._signToolBar.findItemByValue(DgrooveSigns.SELECT_ALL) as Telerik.Web.UI.RadToolBarButton).set_visible(false);
                (this._signToolBar.findItemByValue(DgrooveSigns.SELECT_INVERT) as Telerik.Web.UI.RadToolBarButton).set_visible(false);
                (this._signToolBar.findItemByValue(DgrooveSigns.COMMENT) as Telerik.Web.UI.RadToolBarButton).set_visible(false);
                (this._signToolBar.findItemByValue(DgrooveSigns.SIGNATURE_PADES) as Telerik.Web.UI.RadToolBarButton).set_visible(this._documents[0].DocumentFolders[0].Documents[0].PadesCompliant);

                //Carico il documento
                this._documentViewer.fromFile(this._createDocumentLink(this._documents[0].DocumentFolders[0].Documents[0]));
                (<Telerik.Web.UI.RadPane>$find(this.docTreePaneId)).collapse(Telerik.Web.UI.SplitterDirection.Backward);
                this._documentsNodes()[0].set_checked(true);

                break;
            }
        }
    }

    private _nextEnabled(): boolean {
        let nextEnabled: boolean = (<Telerik.Web.UI.RadButton>this._signToolBar.findItemByValue(DgrooveSigns.NEXT).findControl('btnToggleNext')).get_checked();
        return nextEnabled;
    }

    private _commentEnabled(): boolean {
        let commentEnabled: boolean = (<Telerik.Web.UI.RadButton>this._signToolBar.findItemByValue(DgrooveSigns.COMMENT).findControl('btnAddComment')).get_checked();
        return commentEnabled;
    }

    private _smartCardSelected(): boolean {
        return this._providerSignTypeDdl.get_selectedItem().get_value() === DgrooveSigns.SMARTCARD
    }

    private _btnSignEnable() {
        this._signBtn.set_imageUrl(DgrooveSigns.IMG_SIGNALR_CONNECTED);
        this._signBtn.set_toolTip("Firma documenti.")
        this._signBtn.set_enabled(!this._webApiError);
    }

    private _btnSignDisabled() {
        this._signBtn.set_imageUrl(DgrooveSigns.IMG_SIGNALR_ERROR);
        this._signBtn.set_toolTip("Firma non configurata correttamente. Premere nuovamente per ritentare.")
        this._signBtn.set_enabled(!this._webApiError);
    }

    private _btnSignWaitingConnection() {
        this._signBtn.set_imageUrl(DgrooveSigns.IMG_SIGNALR_CONNECTING);
        this._signBtn.set_toolTip("In Connessione.")
        this._signBtn.set_enabled(false);
    }

    private _createTreeNode(nodeText: string, imageUrl: string, isDocument: boolean, nodeData: any): Telerik.Web.UI.RadTreeNode {
        let treeNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        treeNode.set_text(nodeText);
        treeNode.set_imageUrl(imageUrl);
        treeNode.set_expanded(true);
        treeNode.get_attributes().setAttribute(DgrooveSigns.ATTRIBUTE_DOCUMENT, isDocument);
        treeNode.get_attributes().setAttribute(DgrooveSigns.ATTRIBUTE_NODEDATA, nodeData);
        return treeNode;
    }

    private _createDocumentLink(document: DocumentViewModel): string {
        if (this._documents[0].SignBehaviour === DocumentSignBehaviour.DocumentUploadTempFolder) {
            let baseUrl = `${this.dswBaseUrl}/Viewers/Handlers/DocumentInfoHandler.ashx/`;
            return `${baseUrl}${document.Name}?Type=VecompSoftware.Services.Biblos.Models.TempFileDocumentInfo&IsSigned=False&FullName=${document.FullName}&Name=${document.Name}&FileName=${document.FileName}`;
        }
        else {
            let baseUrl = `${this.dswBaseUrl}/Viewers/Handlers/BiblosDocumentHandler.ashx/`;
            return `${baseUrl}${document.Name}?Type=VecompSoftware.Services.Biblos.Models.BiblosDocumentInfo&IsSigned=False&Name=${document.Name}&Guid=${document.DocumentId}&ChainId=${document.ChainId}`;
        }
    }

    private _setMandatoryDocuments(): void {
        this._documentsNodes().forEach((node: Telerik.Web.UI.RadTreeNode) => {
            let nodeData: DocumentViewModel = node.get_attributes().getAttribute(DgrooveSigns.ATTRIBUTE_NODEDATA);

            if (nodeData.Mandatory === true && nodeData.MandatorySelectable === false && node.get_checked() === false) {
                node.set_checked(true);
            }
        });
    }

    private _executeSignSmartCard(): void {
        if (this._hubConnectionDgrooveSigner === false) {
            this._initializeSignalR_DgrooveSigner();
            return;
        }

        this._loadingPanel.show(this.pnlMainContentId);

        this._signBtn.set_enabled(false);
        this._signalRError = false;

        this._documentsSignOptions = {};
        this._ajaxPromises = [];
        this._uploadedDocuments = [];
        this._collaborationToSynchronize = { IdCollaborations: [] } as SynchronizeCollaborationModel;

        $.when(this._uploadDocuments())
            .done(() => {
                this._signalRService.sendServerMessage("Sign", this._transactionId, this._documentsSignOptions);
            })
            .fail((exception: ExceptionDTO) => {
                this._onWebApiError(exception);
            });
    }

    private _executeSignRemote(otp: string): void {
        this.openNotificationWindow();
        this._signBtn.set_enabled(false);
        this._providerSignTypeDdl.set_enabled(false);
        this._notificationOptInputText.set_visible(false);
        this._notificationOtpBtn.set_visible(false);
        this._notificationSignBtn.set_visible(false);
        this._loadingPanel.show(this.pnlMainContentId);

        let providerSignType: string = this._providerSignTypeDdl.get_selectedItem().get_value();

        this._documentsToSign = this._getNextDocumentsToSign();

        this.addNotification(`Sto firmando ${this._documentsToSign.length} documenti...`);

        this._correlationIdSignalrWebApi = GuidHelper.newGuid();

        if (!this._collaborationToSynchronize) {
            this._collaborationToSynchronize = { IdCollaborations: [] } as SynchronizeCollaborationModel;
        }

        let workflowName: string = "Avvio firma remota";

        let workflowReferenceBiblosModel: WorkflowReferenceBiblosModel[] = [];
        this._documentsToSign.forEach((node: DocumentViewModel) => {
            workflowReferenceBiblosModel.push({
                DocumentName: node.Name,
                ArchiveChainId: node.ChainId,
                ArchiveDocumentId: node.DocumentId
            } as WorkflowReferenceBiblosModel);
        });

        let startRemoteSignModel: any = { "Documents": workflowReferenceBiblosModel, "UserProfileRemoteSignProperty": otp ? { "OTP": otp } : "" };

        let dswModel: DocSuiteModel = {} as DocSuiteModel;
        dswModel.CustomProperties = {};
        dswModel.CustomProperties["DocumentManagementRequestModel"] = JSON.stringify(startRemoteSignModel);

        let dswEvent: DocSuiteEvent = {} as DocSuiteEvent;
        dswEvent.WorkflowName = workflowName;
        dswEvent.WorkflowAutoComplete = true;
        dswEvent.EventModel = dswModel

        let workflowReferenceModel: WorkflowReferenceModel = {} as WorkflowReferenceModel;
        workflowReferenceModel.Documents = [];

        workflowReferenceModel.ReferenceId = this._correlationIdSignalrWebApi;
        workflowReferenceModel.ReferenceType = DSWEnvironment.Any;
        workflowReferenceModel.ReferenceModel = JSON.stringify(dswEvent);
        workflowReferenceModel.Title = workflowName;
        workflowReferenceModel.Documents = workflowReferenceBiblosModel;

        this._workflowStartModel = <WorkflowStartModel>{};
        this._workflowStartModel.WorkflowName = workflowName;
        this._workflowStartModel.Arguments = {};

        let activityNameProperty = { PropertyType: ArgumentType.PropertyString, Name: WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME, ValueString: workflowName } as WorkflowProperty;
        let tenantIdProperty = { PropertyType: ArgumentType.PropertyGuid, Name: WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, ValueGuid: this.currentUserTenantId } as WorkflowProperty;
        let tenantAOOIdProperty = { PropertyType: ArgumentType.PropertyGuid, Name: WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, ValueGuid: this.currentUserTenantAOOId } as WorkflowProperty;
        let tenantNameProperty = { PropertyType: ArgumentType.PropertyString, Name: WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, ValueString: this.currentUserTenantName } as WorkflowProperty;
        let providerSignTypeProperty = { PropertyType: ArgumentType.PropertyInt, Name: WorkflowPropertyHelper.DSW_PROPERTY_PROVIDER_SIGN_TYPE, ValueInt: Number(providerSignType) } as WorkflowProperty;
        let signRequestTypeProperty = { PropertyType: ArgumentType.PropertyInt, Name: WorkflowPropertyHelper.DSW_PROPERTY_SIGN_REQUEST_TYPE, ValueInt: this._signType == SignType.CAdES ? SignRequestType.Cades : SignRequestType.Pades } as WorkflowProperty;
        let loadCredentialProperty = { PropertyType: ArgumentType.PropertyBoolean, Name: WorkflowPropertyHelper.DSW_PROPERTY_REMOTE_SIGN_LOAD_CREDENTIALS, ValueBoolean: true } as WorkflowProperty;
        let referenceModelProperty = { PropertyType: ArgumentType.Json, Name: WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, ValueString: JSON.stringify(workflowReferenceModel) } as WorkflowProperty;

        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME] = activityNameProperty;
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID] = tenantIdProperty;
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID] = tenantAOOIdProperty;
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME] = tenantNameProperty;
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_PROVIDER_SIGN_TYPE] = providerSignTypeProperty;
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_SIGN_REQUEST_TYPE] = signRequestTypeProperty;
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_REMOTE_SIGN_LOAD_CREDENTIALS] = loadCredentialProperty;
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL] = referenceModelProperty;

        if (this._documents[0].SignBehaviour == DocumentSignBehaviour.DocumentUploadTempFolder) {
            let workflowActivityStartReferenceModel: WorkflowReferenceModel = {} as WorkflowReferenceModel;
            workflowActivityStartReferenceModel.ReferenceType = DSWEnvironment.Any;
            workflowActivityStartReferenceModel.Documents = [];

            let workflowDocuments: any[] = [];
            this._documentsToSign.forEach((node: DocumentViewModel) => {
                workflowDocuments.push({
                    Key: ChainType.Miscellanea,
                    Value: {
                        ChainType: ChainType.Miscellanea,
                        FileName: node.Name,
                        ContentStream: node.Content
                    }
                });
            });

            let workflowDocumentModel: WorkflowDocumentModel = {
                Documents: workflowDocuments
            };

            workflowActivityStartReferenceModel.ReferenceModel = JSON.stringify(workflowDocumentModel);

            let archiveDocumentsProperty = { PropertyType: ArgumentType.Json, Name: WorkflowPropertyHelper.DSW_ACTION_ARCHIVE_DOCUMENTS, ValueString: JSON.stringify(workflowActivityStartReferenceModel) } as WorkflowProperty;
            this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_ACTION_ARCHIVE_DOCUMENTS] = archiveDocumentsProperty;

            sessionStorage[SessionStorageKeysHelper.SESSION_KEY_CORRELATION_ID] = this._correlationIdSignalrWebApi;
        }

        this._initializeWorkflowStorage();
        this._signalRService_WebApi = new DSWSignalR(this.signalrWebApiUrl);
        this._signalRService_WebApi.setup(DgrooveSigns.HUB_NAME, { 'correlationId': this._correlationIdSignalrWebApi });
        this._signalRService_WebApi.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowStatusDone, this.onSignalRWebApi_actionWorkflowRemoteSignStatusDone);
        this._signalRService_WebApi.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowStatusError, this.onSignalRWebApi_actionWorkflowStatusError);
        this._signalRService_WebApi.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowNotificationInfo, this.onSignalRWebApi_actionHubWorkflowNotificationInfo);
        this._signalRService_WebApi.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowNotificationWarning, this.onSignalRWebApi_actionHubWorkflowNotificationWarning);
        this._signalRService_WebApi.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowNotificationError, this.onSignalRWebApi_actionHubWorkflowNotificationError);

        this._signalRService_WebApi.startConnection(this.onDoneSignalRConnection, this.onErrorSignalRCallback);
    }

    private _executeRequestOTP(): void {
        this._loadingPanel.show(this.pnlMainContentId);

        this._correlationIdSignalrWebApi = GuidHelper.newGuid();

        let workflowName: string = "Avvio richiesta OTP";
        let providerSignType: string = this._providerSignTypeDdl.get_selectedItem().get_value()

        let workflowReferenceBiblosModel: WorkflowReferenceBiblosModel[] = [];
        let startRemoteSignModel: any = { "Documents": workflowReferenceBiblosModel };

        let dswModel: DocSuiteModel = {} as DocSuiteModel;
        dswModel.CustomProperties = {};
        dswModel.CustomProperties["DocumentManagementRequestModel"] = JSON.stringify(startRemoteSignModel);

        let dswEvent: DocSuiteEvent = {} as DocSuiteEvent;
        dswEvent.WorkflowName = workflowName;
        dswEvent.WorkflowAutoComplete = true;
        dswEvent.EventModel = dswModel

        let workflowReferenceModel: WorkflowReferenceModel = {} as WorkflowReferenceModel;
        workflowReferenceModel.Documents = [];

        workflowReferenceModel.ReferenceId = this._correlationIdSignalrWebApi;
        workflowReferenceModel.ReferenceType = DSWEnvironment.Any;
        workflowReferenceModel.ReferenceModel = JSON.stringify(dswEvent)
        workflowReferenceModel.Title = workflowName;

        this._workflowStartModel = <WorkflowStartModel>{};
        this._workflowStartModel.WorkflowName = workflowName;
        this._workflowStartModel.Arguments = {};

        let activityNameProperty = { PropertyType: ArgumentType.PropertyString, Name: WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME, ValueString: workflowName } as WorkflowProperty;
        let tenantIdProperty = { PropertyType: ArgumentType.PropertyGuid, Name: WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID, ValueGuid: this.currentUserTenantId } as WorkflowProperty;
        let tenantAOOIdProperty = { PropertyType: ArgumentType.PropertyGuid, Name: WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID, ValueGuid: this.currentUserTenantAOOId } as WorkflowProperty;
        let tenantNameProperty = { PropertyType: ArgumentType.PropertyString, Name: WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME, ValueString: this.currentUserTenantName } as WorkflowProperty;
        let providerSignTypeProperty = { PropertyType: ArgumentType.PropertyInt, Name: WorkflowPropertyHelper.DSW_PROPERTY_PROVIDER_SIGN_TYPE, ValueInt: Number(providerSignType) } as WorkflowProperty;
        let loadCredentialProperty = { PropertyType: ArgumentType.PropertyBoolean, Name: WorkflowPropertyHelper.DSW_PROPERTY_REMOTE_SIGN_LOAD_CREDENTIALS, ValueBoolean: true } as WorkflowProperty;
        let referenceModelProperty = { PropertyType: ArgumentType.Json, Name: WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL, ValueString: JSON.stringify(workflowReferenceModel) } as WorkflowProperty;

        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_ACTIVITY_NAME] = activityNameProperty;
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_ID] = tenantIdProperty;
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_AOO_ID] = tenantAOOIdProperty;
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_TENANT_NAME] = tenantNameProperty;
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_PROVIDER_SIGN_TYPE] = providerSignTypeProperty;
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_REMOTE_SIGN_LOAD_CREDENTIALS] = loadCredentialProperty;
        this._workflowStartModel.Arguments[WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL] = referenceModelProperty;

        this._initializeWorkflowStorage();
        this._signalRService_WebApi = new DSWSignalR(this.signalrWebApiUrl);
        this._signalRService_WebApi.setup(DgrooveSigns.HUB_NAME, { 'correlationId': this._correlationIdSignalrWebApi });
        this._signalRService_WebApi.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowStatusDone, this.onSignalRWebApi_actionWorkflowOTPStatusDone);
        this._signalRService_WebApi.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowStatusError, this.onSignalRWebApi_actionWorkflowStatusError);
        this._signalRService_WebApi.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowNotificationInfo, this.onSignalRWebApi_actionHubWorkflowNotificationInfo);
        this._signalRService_WebApi.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowNotificationWarning, this.onSignalRWebApi_actionHubWorkflowNotificationWarning);
        this._signalRService_WebApi.registerClientMessage(UDSConstants.HubMessageEvents.WorkflowNotificationError, this.onSignalRWebApi_actionHubWorkflowNotificationError);

        this._signalRService_WebApi.startConnection(this.onDoneSignalRConnection, this.onErrorSignalRCallback);
    }

    private _uploadDocuments(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred();

        try {
            let signalRPromise: JQueryPromise<void>[] = [];

            this._documentsNodes(true).forEach((node: Telerik.Web.UI.RadTreeNode) => {
                let nodeData: DocumentViewModel = node.get_attributes().getAttribute(DgrooveSigns.ATTRIBUTE_NODEDATA);
                signalRPromise.push(this._uploadDocument(nodeData))
            });

            $.when.apply(null, signalRPromise)
                .done(() => promise.resolve())
                .fail((exception) => {
                    this._loadingPanel.hide(this.pnlMainContentId);
                    promise.reject(exception);
                });

        } catch (error) {
            return promise.reject(error);
        }

        return promise.promise();
    }

    private _uploadDocument(document: DocumentViewModel): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred();
        try {
            let comment = this._commentEnabled() ? this._documents[0].Comment : "";
            if (this._documents[0].SignBehaviour === DocumentSignBehaviour.DocumentUploadTempFolder) {
                let documentModel: DocumentModel = {
                    TransactionId: this._transactionId,
                    DocumentId: document.DocumentId,
                    Content: document.Content,
                    Filename: document.Name
                };

                this._documentsSignOptions[document.DocumentId] = { SignTime: moment().format(), IsSignatureVisible: false, SignType: this._signType, SignReason: comment };

                this._signalRService.sendServerMessage("UploadDocument", documentModel, null,
                    () => {
                        this._uploadedDocuments.push(document);
                        promise.resolve();
                    },
                    (exception: ExceptionDTO) => {
                        promise.reject(exception);
                    }
                );
            } else {
                this._biblosDocumentsService.getBiblosDocumentContent(document.DocumentId,
                    (data) => {
                        let documentModel: DocumentModel = {
                            TransactionId: this._transactionId,
                            DocumentId: document.DocumentId,
                            Content: data,
                            Filename: document.Name
                        };

                        this._documentsSignOptions[document.DocumentId] = { SignTime: moment().format(), IsSignatureVisible: false, SignType: this._signType, SignReason: comment };

                        this._signalRService.sendServerMessage("UploadDocument", documentModel, null,
                            () => {
                                this._uploadedDocuments.push(document);
                                promise.resolve();
                            },
                            (exception: ExceptionDTO) => {
                                promise.reject(exception);
                            }
                        );
                    },
                    (exception: ExceptionDTO) => {
                        promise.reject(exception);
                    }
                );
            }
        } catch (error) {
            return promise.reject(error);
        }
        return promise.promise();
    }

    private _evaluateCollaboration(model: any): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let buildModel: BuildActionModel = {} as BuildActionModel;
        buildModel.BuildType = BuildActionType.Evaluate;
        buildModel.ReferenceType = ReferenceBuildModelType.Collaboration;

        let documentInfo = this._uploadedDocuments.find(i => i.DocumentId === model.documentId);

        let documentModel = {
            Name: model.filename,
            Archive: this.collaborationArchiveName,
            ContentStream: model.content
        } as BuildArchiveDocumentModel;

        let evaluateDococumenModel = {
            IdCollaboration: documentInfo.CollaborationId,
            IdCollaborationVersioning: documentInfo.CollaborationVersioningId,
            ArchiveDocument: documentModel
        } as EvaluateCollaborationDocumentModel

        let collaborationModel = {
            DocumentModels: [evaluateDococumenModel],
            FromWorkflow: false
        } as EvaluateCollaborationModel

        buildModel.Model = JSON.stringify(collaborationModel);

        this._builderService.sendBuild(buildModel,
            (data: any) => {
                this._collaborationToSynchronize.IdCollaborations.push(documentInfo.CollaborationId);
                promise.resolve();
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pnlMainContentId);
                promise.reject(exception);
            });
        return promise.promise();
    }

    private _evaluateFascicle(model: any): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let buildModel: BuildActionModel = {} as BuildActionModel;
        buildModel.BuildType = BuildActionType.Evaluate;
        buildModel.ReferenceType = ReferenceBuildModelType.Fascicle;

        let documentInfo = this._uploadedDocuments.find(i => i.DocumentId === model.documentId);

        let documentModel = {
            Name: model.filename,
            IdDocument: documentInfo.DocumentId,
            Archive: documentInfo.ChainId,
            ContentStream: model.content
        } as BuildArchiveDocumentModel;

        let fascicleModel = {
            IdFascicle: documentInfo.FascicleId,
            ArchiveDocument: documentModel
        } as EvaluateFascicleModel

        buildModel.Model = JSON.stringify(fascicleModel);

        this._builderService.sendBuild(buildModel,
            (data: any) => {
                promise.resolve();
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pnlMainContentId);
                promise.reject(exception);
            });
        return promise.promise();
    }

    private _singleDocumentSigned(model: any): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();

        let signedDoc = this._uploadedDocuments.find(i => i.DocumentId === model.documentId);
        signedDoc.Content = model.content;
        signedDoc.Name = model.filename

        sessionStorage[SessionStorageKeysHelper.SESSION_KEY_SIGNED_DOC] = JSON.stringify(signedDoc);

        promise.resolve();

        return promise.promise();
    }

    private _synchronizeCollaborations(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();

        let buildModel: BuildActionModel = {} as BuildActionModel;
        buildModel.BuildType = BuildActionType.Synchronize;
        buildModel.ReferenceType = ReferenceBuildModelType.Collaboration;

        buildModel.Model = JSON.stringify(this._collaborationToSynchronize);

        this._builderService.sendBuild(buildModel,
            () => {
                promise.resolve();
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pnlMainContentId);
                promise.reject(exception);
            }
        );

        return promise.promise();
    }

    private _evaluateCollaborationWorkflow(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        if (this._documents[0].SignBehaviour === DocumentSignBehaviour.DocumentUploadTempFolder) {
            promise.resolve();
            return;
        }
        let buildModel: BuildActionModel = {} as BuildActionModel;
        buildModel.BuildType = BuildActionType.Evaluate;
        buildModel.ReferenceType = ReferenceBuildModelType.Collaboration;

        let collaborationModel: EvaluateCollaborationModel = { DocumentModels: [], FromWorkflow: true } as EvaluateCollaborationModel;
        let collaborationIdsToSynchronize: string[] = [];

        this._documentsToSign.forEach((document: DocumentViewModel) => {
            let documentModel = {
                Name: document.Name,
                Archive: this.collaborationArchiveName,
                IdDocument: document.DocumentId,
                IdChain: document.ChainId
            } as BuildArchiveDocumentModel;

            let evaluateDococumenModel = {
                IdCollaboration: document.CollaborationId,
                IdCollaborationVersioning: document.CollaborationVersioningId,
                ArchiveDocument: documentModel
            } as EvaluateCollaborationDocumentModel

            collaborationModel.DocumentModels.push(evaluateDococumenModel);
            collaborationIdsToSynchronize.push(document.CollaborationId);
        });

        buildModel.Model = JSON.stringify(collaborationModel);

        this._builderService.sendBuild(buildModel,
            (data: any) => {
                this._collaborationToSynchronize.IdCollaborations.push(...collaborationIdsToSynchronize);
                promise.resolve();
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pnlMainContentId);
                promise.reject(exception);
            });
        return promise.promise();
    }

    private _redirect(): void {
        sessionStorage[SessionStorageKeysHelper.SESSION_KEY_DOCS_TO_SIGN] = "";

        if (this._redirectUrl) {
            window.location.href = this._redirectUrl;
        }
    }

    public onSignalR_Logging = (message, notificationType) => {
        console.log(`[Logging] Message=${message}; NotificationType=${notificationType}`);
        if (notificationType === DgrooveSigns.SIGNAL_DOCUMENTNOTVALID_ERROR || notificationType === DgrooveSigns.SIGNAL_GENERIC_ERROR || notificationType === DgrooveSigns.SIGNAL_SIGN_ERROR) {
            this.showNotificationMessage(message);
            this._loadingPanel.hide(this.pnlMainContentId);
            this._signalRError = true;
        }
    }

    public onSignalR_Connected = () => {
        console.log(`[Connected]`);
    }

    public onSignalR_SignedDocument = (document: any) => {
        switch (this._documents[0].SignBehaviour) {
            case DocumentSignBehaviour.CollaborationDocumentUpload:
            case DocumentSignBehaviour.Collaboration: {
                this._ajaxPromises.push(this._evaluateCollaboration(document));
                break;
            }
            case DocumentSignBehaviour.Fascicle: {
                this._ajaxPromises.push(this._evaluateFascicle(document));
                break;
            }
            case DocumentSignBehaviour.DocumentUploadTempFolder: {
                this._ajaxPromises.push(this._singleDocumentSigned(document));
                break;
            }
        }
    }

    public onSignalR_ClosedTransaction = (transactionId: string) => {
        $.when.apply(null, this._ajaxPromises)
            .done(() => {
                if (this._signalRError) {
                    return;
                }
                switch (this._documents[0].SignBehaviour) {
                    case DocumentSignBehaviour.Collaboration: {
                        if (this._nextEnabled()) {
                            this._synchronizeCollaborations()
                                .done(() => {
                                    this._redirect();
                                })
                                .fail((exception: ExceptionDTO) => {
                                    this._onWebApiError(exception);
                                });
                        }
                        else {
                            this._redirect();
                        }
                        break;
                    }
                    case DocumentSignBehaviour.CollaborationDocumentUpload:
                    case DocumentSignBehaviour.Fascicle: {
                        this._redirect();
                        break;
                    }
                    case DocumentSignBehaviour.DocumentUploadTempFolder: {
                        this._loadingPanel.hide(this.pnlMainContentId);
                        window.close();
                        break;
                    }
                }
            })
            .fail((exception: ExceptionDTO) => {
                this._onWebApiError(exception);
            });
    }

    public onSignalRWebApi_actionWorkflowRemoteSignStatusDone = (model) => {
        if (this.wStorageEnabled) {
            this.wstorage.Unset();
        }

        this.addNotificationDone("Documenti firmati con successo.");
        this._signalRService_WebApi.stopClient();

        this._indexFirstDocumentToSign = this._indexFirstDocumentToSign + this._documentsToSign.length;

        $.when(this._evaluateCollaborationWorkflow())
            .done(() => {
                this.addNotification("Collaborazione aggiornata.");
                if (this._moreToSign) {
                    if (this._selectedProviderSignTypeOtpAttribute()) {
                        this.addNotification("E' necessario inserire un nuovo OTP per continuare.");
                        this._notificationOptInputText.set_visible(true);
                        this._notificationOtpBtn.set_visible(true);
                        this._notificationOptInputText.clear();
                    }
                    this._notificationSignBtn.set_visible(true);

                    this._loadingPanel.hide(this.pnlMainContentId);
                    return;
                }

                switch (this._documents[0].SignBehaviour) {
                    case DocumentSignBehaviour.Collaboration: {
                        if (this._nextEnabled()) {
                            this.addNotification("Avanzamento delle collaborazioni in corso...");
                            this._synchronizeCollaborations()
                                .done(() => {
                                    this.addNotification("Collaborazioni correttamente avanzate.");
                                    setTimeout(() => this._redirect(), 3000);
                                })
                                .fail((exception: ExceptionDTO) => {
                                    this.addNotificationError("Errore durante l'avanzamento delle collaborazioni.");
                                });
                        }
                        else {
                            setTimeout(() => this._redirect(), 3000);
                        }
                        break;
                    }
                    case DocumentSignBehaviour.CollaborationDocumentUpload:
                    case DocumentSignBehaviour.Fascicle: {
                        this._redirect();
                        break;
                    }
                    case DocumentSignBehaviour.DocumentUploadTempFolder: {
                        this.addNotification("Recupero documento firmato in corso...");
                        this.syncronyzeSignedTempDocument()
                            .done(() => window.close())
                            .fail((errorMessage: string) => this.addNotificationError(errorMessage));                        
                        break;
                    }
                }
                this._loadingPanel.hide(this.pnlMainContentId);
            })
            .fail((exception: ExceptionDTO) => {
                this.addNotificationError("Errore durante l'aggiornamento delle collaborazioni.");
            });
    }

    public onSignalRWebApi_actionWorkflowOTPStatusDone = (model) => {
        if (this.wStorageEnabled) {
            this.wstorage.Unset();
        }

        if (this.isNotificationWindowOpen()) {
            this.addNotificationDone("Richiesta inviata con successo.")
        } else {
            this._notificationInfo.show();
            this._notificationInfo.set_title("OTP");
            this._notificationInfo.set_text("Richiesta inviata con successo.");
        }

        this._signalRService_WebApi.stopClient();
        this._loadingPanel.hide(this.pnlMainContentId);
    }

    public onSignalRWebApi_actionWorkflowStatusError = (model) => {
        if (this.wStorageEnabled) {
            this.wstorage.Unset();
        }

        if (this.isNotificationWindowOpen()) {
            this.addNotificationError(model);
        } else {
            this.showWarningMessage(model);
        }

        this._signBtn.set_enabled(true);
        this._optInputText.set_value("");
        this._providerSignTypeDdl.set_enabled(true);

        this._loadingPanel.hide(this.pnlMainContentId);
        this._signalRService_WebApi.stopClient();
    }

    public onSignalRWebApi_actionHubWorkflowNotificationWarning = (model) => {
        console.log(`[SignalR-WebAPI-Warning] Message=${model}`);
    }

    public onSignalRWebApi_actionHubWorkflowNotificationInfo = (model) => {
        console.log(`[SignalR-WebAPI-Info] Message=${model}`);
    }

    public onSignalRWebApi_actionHubWorkflowNotificationError = (model) => {
        console.log(`[SignalR-WebAPI-Error] Message=${model}`);
    }

    public onDoneSignalRConnection = () => {
        let serverFunction: string = UDSConstants.HubMethods.SubscribeStartWorkflow;
        this._signalRService_WebApi.sendServerMessages(serverFunction, this._correlationIdSignalrWebApi, JSON.stringify(this._workflowStartModel), this.onDoneSignalRSubscriptionCallback, this.onErrorSignalRCallback);
    }

    public onErrorSignalRCallback = (error) => {
        this._loadingPanel.hide(this.pnlMainContentId);
        this.addNotificationError(`Anomalia nel avvio dell'attività: ${error}`);
        this._signalRService_WebApi.stopClient();
        this._signBtn.set_enabled(true);
        this._providerSignTypeDdl.set_enabled(true);
    }

    public onDoneSignalRSubscriptionCallback = (error) => {
    }

    private showNotificationException(exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(customMessage)
        }
    }

    private showWarningMessage(customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showWarningMessage(customMessage)
        }
    }

    private showNotificationMessage(customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage)
        }
    }

    private openNotificationWindow(): void {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerSigner);
        let wnd: Telerik.Web.UI.RadWindow = manager.open(null, "windowNotification", null);
        wnd.set_behaviors(Telerik.Web.UI.WindowBehaviors.Close)
        wnd.setSize(600, 400);
        wnd.set_modal(true);
        wnd.center();
    }

    private closeNotificationWindow(): void {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerSigner);
        let wnd: Telerik.Web.UI.RadWindow = manager.getWindowByName("windowNotification");
        wnd.close();
    }

    private isNotificationWindowOpen(): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerSigner);

        return manager.getActiveWindow() !== null;
    }

    private addNotification = (text: string) => {
        this.addItem(text, "../App_Themes/DocSuite2008/imgset16/information.png");
    }

    private addNotificationError = (text: string) => {
        this.addItem(text, "../App_Themes/DocSuite2008/imgset16/StatusSecurityCritical_16x.png");
    }

    private addNotificationDone = (text: string) => {
        this.addItem(text, "../App_Themes/DocSuite2008/imgset16/star.png");
    }

    private addItem = (text: string, imageUrl: string) => {
        var item = new Telerik.Web.UI.RadListBoxItem();
        item.set_text(text);
        item.set_imageUrl(imageUrl)
        this._radListMessages.get_items().add(item);
        this._radListMessages.commitChanges();
    }

    private syncronyzeSignedTempDocument(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();

        this._workflowActivityService.getWorkflowActivitiesByWorkflowInstanceId(sessionStorage[SessionStorageKeysHelper.SESSION_KEY_CORRELATION_ID],
            (data: WorkflowActivityModel[]) => {
                if (data == null || data.length == 0) {
                    promise.reject("Errore durante il recupero delle informazioni del documento firmato. Nessun documento firmato trovato");
                    return;
                }

                this._workflowPropertyService.getPropertiesFromActivity(data[0].UniqueId,
                    (data: WorkflowPropertyModel[]) => {
                        try {
                            let referenceModelProperty: WorkflowPropertyModel = data.find(x => x.Name == WorkflowPropertyHelper.DSW_PROPERTY_REFERENCE_MODEL);
                            let referenceModel: WorkflowReferenceModel = JSON.parse(referenceModelProperty.ValueString) as WorkflowReferenceModel;
                            let referenceDocumentModel: WorkflowReferenceBiblosModel = (<any>referenceModel.Documents).$values[0];
                            this._biblosDocumentsService.getBiblosDocumentContent(referenceDocumentModel.ArchiveDocumentId,
                                (data: any) => {
                                    let nodeData: DocumentViewModel = this._documentsNodes(true)[0].get_attributes().getAttribute(DgrooveSigns.ATTRIBUTE_NODEDATA);
                                    nodeData.Content = data;
                                    nodeData.Name = this._signType == SignType.CAdES ? `${referenceDocumentModel.DocumentName}.p7m` : referenceDocumentModel.DocumentName;
                                    sessionStorage[SessionStorageKeysHelper.SESSION_KEY_SIGNED_DOC] = JSON.stringify(nodeData);
                                    promise.resolve();
                                },
                                (exception: ExceptionDTO) => {
                                    console.error(JSON.stringify(exception));
                                    promise.reject("Errore durante il recupero del documento firmato.");
                                }
                            );
                        } catch (exception) {
                            console.error(exception);
                            promise.reject("Errore durante il recupero delle informazioni del documento firmato.");
                        }                        
                    },
                    (exception: ExceptionDTO) => {
                        console.error(JSON.stringify(exception));
                        promise.reject("Errore durante il recupero delle informazioni del documento firmato.");
                    }
                );
            },
            (exception: ExceptionDTO) => {
                console.error(JSON.stringify(exception));
                promise.reject("Errore durante il recupero delle informazioni del documento firmato.");
            }
        )

        return promise.promise();
    }

    private getDefaultSignTypeProvider(userProfileModel: UserProfileModel): ProviderSignType {
        if (userProfileModel) {
            if (userProfileModel.ArubaAutomatic?.IsDefault) {
                return ProviderSignType.ArubaAutomatic;
            }
            if (userProfileModel.ArubaRemote?.IsDefault) {
                return ProviderSignType.ArubaRemote;
            }
            if (userProfileModel.InfocertAutomatic?.IsDefault) {
                return ProviderSignType.InfocertAutomatic;
            }
            if (userProfileModel.InfocertRemote?.IsDefault) {
                return ProviderSignType.InfocertRemote;
            }
            return userProfileModel.DefaultProvider as ProviderSignType;
        }
        return ProviderSignType.Smartcard;
    }
}

export = DgrooveSigns;