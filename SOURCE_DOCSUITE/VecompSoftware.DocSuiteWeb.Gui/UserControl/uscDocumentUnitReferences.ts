import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import EnumHelper = require("App/Helpers/EnumHelper");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import FascicleDocumentUnitService = require("App/Services/Fascicles/FascicleDocumentUnitService");
import DossierModel = require("App/Models/Dossiers/DossierModel");
import UDSDocumentUnitService = require("App/Services/UDS/UDSDocumentUnitService");
import UDSDocumentUnitModel = require("App/Models/UDS/UDSDocumentUnitModel");
import UscErrorNotification = require('UserControl/uscErrorNotification');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ProtocolLinkService = require("App/Services/Protocols/ProtocolLinkService");
import ProtocolLinkModel = require("App/Models/Protocols/ProtocolLinkModel");
import ProtocolService = require("App/Services/Protocols/ProtocolService");
import ProtocolModel = require("App/Models/Protocols/ProtocolModel");
import MessageService = require("App/Services/Messages/MessageService");
import DocumentSeriesItemService = require("App/Services/DocumentArchives/DocumentSeriesItemService");
import MessageModel = require("App/Models/Messages/MessageModel");
import DocumentSeriesItemModel = require("App/Models/DocumentArchives/DocumentSeriesItemModel");
import MessageContactPosition = require("App/Models/Messages/MessageContactPosition");
import PECMailService = require("App/Services/PECMails/PECMailService");
import PECMailViewModel = require("App/ViewModels/PECMails/PECMailViewModel");
import PECMailReceiptsModel = require("App/ViewModels/PECMails/PECMailReceiptsModel");
import DocumentSeriesItemsModel = require("App/Models/Series/DocumentSeriesItemModel");
import DocumentSeriesItemLinksModel = require("App/Models/Series/DocumentSeriesItemLinksModel");
import DocumentSeriesItemLinksService = require("App/Services/DocumentArchives/DocumentSeriesItemLinksService");
import ResolutionModel = require("App/Models/Resolutions/ResolutionModel");
import ResolutionService = require("App/Services/Resolutions/ResolutionService");
import ResolutionDocumentSeriesItemService = require("App/Services/Resolutions/ResolutionDocumentSeriesItemService");
import ResolutionDocumentSeriesItemModel = require("App/Models/Resolutions/ResolutionDocumentSeriesItemModel");
import FascicleLinkService = require("App/Services/Fascicles/FascicleLinkService");
import FascicleLinkModel = require("App/Models/Fascicles/FascicleLinkModel");
import FascicleService = require("App/Services/Fascicles/FascicleService");
import DossierFolderService = require("App/Services/Dossiers/DossierFolderService");
import WorkflowActivityService = require("App/Services/Workflows/WorkflowActivityService");
import FascicleModel = require("App/Models/Fascicles/FascicleModel");
import DossierFolderModel = require("App/Models/Dossiers/DossierFolderModel");
import DocumentUnitReferenceTypeEnum = require("App/Models/Commons/DocumentUnitReferenceTypeEnum");
import WorkflowActivityModel = require('App/Models/Workflows/WorkflowActivityModel');
import TNoticeService = require("App/Services/PosteWeb/TNoticeService");
import TNoticeStatusSummaryDTO = require("App/DTOs/TNoticeStatusSummaryDTO");
import StatusColor = require("App/Models/PosteWeb/StatusColor");
import DossierFolderStatus = require("App/Models/Dossiers/DossierFolderStatus");

class uscDocumentUnitReferences {
    radTreeDocumentsId: string;
    documentUnitId: string;
    documentUnitYear: string;
    documentUnitNumber: string;
    udsDocumentList: UDSDocumentUnitModel[];
    protocolUdsDocumentList: UDSDocumentUnitModel[];
    udsIdList: UDSDocumentUnitModel[];
    protocolDocumentList: UDSDocumentUnitModel[];
    fascicleDocumentUnitList: FascicleModel[];
    protocolLinks: ProtocolLinkModel[];
    protocolMessages: ProtocolModel[];
    protocolDocumentSeries: ProtocolModel;
    pecMailIncomings: PECMailViewModel[];
    pecMailOutgoings: PECMailViewModel[];
    pecMailIncomingAndOutgoing: PECMailViewModel[];
    documentSeriesItemList: DocumentSeriesItemsModel;
    documentSeriesItemLinksList: DocumentSeriesItemLinksModel;
    documentSeriesItemProtocol: DocumentSeriesItemsModel;
    resolutionMessage: ResolutionModel;
    resolutionDocumentSeriesItem: ResolutionDocumentSeriesItemModel[];
    fascicleLinkList: FascicleLinkModel[];
    fascicleList: FascicleModel[];
    dossierFolderList: DossierFolderModel[];
    activeWorkflowActivityList: WorkflowActivityModel[];
    doneWorkflowActivityList: WorkflowActivityModel[];
    tNoticeSummaryList: TNoticeStatusSummaryDTO[];
    showRemoveUDSLinksButton: string;

    rpbDocumentsId: string;
    uscNotificationId: string;
    managerWindowsId: string;
    administrationTrasparenteProtocol: string;
    seriesTitle: string;
    protocolDocumentSeriesButtonEnable: string;

    showFascicleLinks: string;
    showProtocolRelationLinks: string;
    showArchiveRelationLinks: string;
    showProtocolMessageLinks: string;
    showProtocolDocumentSeriesLinks: string;
    showDocumentSeriesMessageLinks: string;
    showDocumentSeriesResolutionsLinks: string;
    showArchiveLinks: string;
    showProtocolLinks: string;
    showDocumentSeriesProtocolsLinks: string;
    showIncomingPECMailLinks: string;
    showOutgoingPECMailLinks: string;
    showResolutionlMessageLinks: string;
    showResolutionDocumentSeriesLinks: string;
    showFasciclesLinks: string;
    showDossierLinks: string;
    showTNotice: string;
    showActiveWorkflowActivities: string;
    showDoneWorkflowActivities: string;

    btnExpandDocumentUnitReferenceId: string;
    documentUnitReferenceInfoId: string;

    private _DocumentRadTreeId: Telerik.Web.UI.RadTreeView;
    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;
    private _fascService: FascicleDocumentUnitService;
    private _udsService: UDSDocumentUnitService;
    private _protocolLinkService: ProtocolLinkService;
    private _protocolService: ProtocolService;
    private _messageService: MessageService;
    private _documentSeriesItemService: DocumentSeriesItemService;
    private _pecMailService: PECMailService;
    private _documentSeriesItemLinkService: DocumentSeriesItemLinksService;
    private _resolutionService: ResolutionService;
    private _resolutionDocumentSeriesItemService: ResolutionDocumentSeriesItemService;
    private _fascicleLinksService: FascicleLinkService;
    private _fascicleService: FascicleService;
    private _dossierFolderService: DossierFolderService;
    private _tnoticeService: TNoticeService;
    private _workflowActivityService: WorkflowActivityService;
    private _btnExpandDocumentUnitReference: Telerik.Web.UI.RadButton;
    private _isDocumentUnitReferenceOpen: boolean;
    private _documentUnitReferenceContent: JQuery;
    private index: number = 0;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
        //$(document).ready(() => {
        //});
    }

    initialize(): void {
        this._btnExpandDocumentUnitReference = <Telerik.Web.UI.RadButton>$find(this.btnExpandDocumentUnitReferenceId);
        this._btnExpandDocumentUnitReference.addCssClass("dsw-arrow-down");
        this._btnExpandDocumentUnitReference.add_clicking(this.btnExpandDocumentUnitReference_OnClick);
        this._documentUnitReferenceContent = $("#".concat(this.documentUnitReferenceInfoId));
        this._documentUnitReferenceContent.show();



        let serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleDocumentUnit");
        this._fascService = new FascicleDocumentUnitService(serviceConfiguration);

        let udsServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "UDSDocumentUnit");
        this._udsService = new UDSDocumentUnitService(udsServiceConfiguration);

        let protocolLinkServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "ProtocolLink");
        this._protocolLinkService = new ProtocolLinkService(protocolLinkServiceConfiguration);

        let protocolServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Protocol");
        this._protocolService = new ProtocolService(protocolServiceConfiguration);

        let messageServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Message");
        this._messageService = new MessageService(messageServiceConfiguration);

        let documentSeriesItemServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentSeriesItem");
        this._documentSeriesItemService = new DocumentSeriesItemService(documentSeriesItemServiceConfiguration);

        let documentSeriesItemLinksServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentSeriesItemLink");
        this._documentSeriesItemLinkService = new DocumentSeriesItemLinksService(documentSeriesItemLinksServiceConfiguration);

        let pecMailServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "PECMail");
        this._pecMailService = new PECMailService(pecMailServiceConfiguration);

        let resolutionServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Resolution");
        this._resolutionService = new ResolutionService(resolutionServiceConfiguration);

        let resolutionDocumentSeriesItemServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "ResolutionDocumentSeriesItem");
        this._resolutionDocumentSeriesItemService = new ResolutionDocumentSeriesItemService(resolutionDocumentSeriesItemServiceConfiguration);

        let fascicleLinksServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleLink");
        this._fascicleLinksService = new FascicleLinkService(fascicleLinksServiceConfiguration);

        let fascicleServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Fascicle");
        this._fascicleService = new FascicleService(fascicleServiceConfiguration);

        let dossierFolderServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DossierFolder");
        this._dossierFolderService = new DossierFolderService(dossierFolderServiceConfiguration);

        let workflowActivityServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowActivity");
        this._workflowActivityService = new WorkflowActivityService(workflowActivityServiceConfiguration);

        let polRequestConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "POLRequest");
        this._tnoticeService = new TNoticeService(polRequestConfiguration);

        this._DocumentRadTreeId = <Telerik.Web.UI.RadTreeView>$find(this.radTreeDocumentsId);

        if (this.showIncomingPECMailLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory("PEC ingresso", DocumentUnitReferenceTypeEnum.PECIngresso, () => this.loadIncomingPECMailCount(this.documentUnitId, `'Incoming'`, this.index));
        }

        if (this.showOutgoingPECMailLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory("PEC uscita", DocumentUnitReferenceTypeEnum.PECUscita, () => this.loadOutgoingPECMailCount(this.documentUnitId, `'Outgoing'`, this.index))
        }

        if (this.showTNotice.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory("TNotice", DocumentUnitReferenceTypeEnum.TNotice, () => this.loadTNoticeCount(this.documentUnitId, this.index));
        }

        if (this.showFascicleLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Fascicoli", DocumentUnitReferenceTypeEnum.Fascicle, () => this.loadFascicleCount(this.documentUnitId, this.index));
        }

        if (this.showFasciclesLinks.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Fascicoli", DocumentUnitReferenceTypeEnum.FascicleProtocol, () => this.loadFasciclesCount(this.documentUnitId, this.index));
        }

        if (this.showArchiveRelationLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Archivi", DocumentUnitReferenceTypeEnum.ArchiveProtocol, () => this.loadProtocolUDSCount(this.documentUnitId, this.index));
        }

        if (this.showArchiveLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Archivi", DocumentUnitReferenceTypeEnum.Archive, () => this.loadUDSCount(this.documentUnitId, this.index));
        }

        if (this.administrationTrasparenteProtocol.toLocaleLowerCase() == "true" && this.showProtocolDocumentSeriesLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory(this.seriesTitle ? this.seriesTitle : "Serie documentali", DocumentUnitReferenceTypeEnum.SeriesProtocol, () => this.loadProtocolDocumentSeriesCount(this.documentUnitId, this.index));
        }

        if (this.showResolutionDocumentSeriesLinks.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory(this.seriesTitle ? this.seriesTitle : "Serie documentali", DocumentUnitReferenceTypeEnum.Series, () => this.loadDocumentSeriesItemCount(this.documentUnitId, this.index));
        }

        if (this.showProtocolRelationLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Protocolli", DocumentUnitReferenceTypeEnum.ProtocolLinks, () => this.loadProtocolLinkCount(this.documentUnitId, this.index));
        }

        if (this.showProtocolLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Protocolli", DocumentUnitReferenceTypeEnum.Protocol, () => this.loadProtocolCount(this.documentUnitId, this.index));
        }

        if (this.showDocumentSeriesProtocolsLinks.toLocaleLowerCase() == "true" && this.protocolDocumentSeriesButtonEnable.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Protocolli", DocumentUnitReferenceTypeEnum.ProtocolSeries, () => this.loadDocumentSeriesProtocolsLinksCount(this.documentUnitId, this.index));
        }

        if (this.showResolutionlMessageLinks.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Messaggi", DocumentUnitReferenceTypeEnum.MessageResolution, () => this.loadResolutionMessageCount(this.documentUnitId, this.index));
        }

        if (this.showProtocolMessageLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Messaggi", DocumentUnitReferenceTypeEnum.MessageProtocol, () => this.loadProtocolMessageCount(this.documentUnitId, this.index));
        }

        if (this.showDocumentSeriesMessageLinks.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Messaggi", DocumentUnitReferenceTypeEnum.MessageSeries, () => this.loadDocumentSeriesMessageCount(this.documentUnitId, this.index));
        }

        if (this.showDocumentSeriesResolutionsLinks.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Atti", DocumentUnitReferenceTypeEnum.Atti, () => this.loadDocumentSeriesItemLinksCount(this.documentUnitId, this.index));
        }

        if (this.showDossierLinks.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Dossiers", DocumentUnitReferenceTypeEnum.Dossier, () => this.loadDossierCount(this.documentUnitId, this.index));
        }

        if (this.showActiveWorkflowActivities.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Flussi di lavoro attivi", DocumentUnitReferenceTypeEnum.ActiveWorkflows, () => this.loadActiveWorkflowActivitiesCount(this.documentUnitId, this.index));
        }
        if (this.showDoneWorkflowActivities.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Flussi di lavoro completati", DocumentUnitReferenceTypeEnum.DoneWorkflows, () => this.loadDoneWorkflowActivitiesCount(this.documentUnitId, this.index));
        }

        this._DocumentRadTreeId.add_nodeClicked(this.generalOnNodeExpanding);
    }

    private evaluateNodePropertyFactory(nodeText: string, value: DocumentUnitReferenceTypeEnum, callback: any) {
        let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        node.set_text(nodeText);
        node.set_cssClass("font_node");
        node.get_attributes().setAttribute("ParentNodeType", value);
        this._DocumentRadTreeId.get_nodes().add(node);
        callback();
        this.index++;
    }

    generalOnNodeExpanding = (sender, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        let node: Telerik.Web.UI.RadTreeNode = args.get_node();
        node.toggle();

        let parentNodeAttribute: string = args.get_node().get_attributes().getAttribute("ParentNodeType");
        let loadIndex = args.get_node().get_index();

        node = args.get_node();
        if (node.get_level() != 0) {
            var parentNode = node.get_parent();

            if (parentNode.get_text().startsWith("Messaggi")) {
                let nodeValue: string = args.get_node().get_value();
                let url: string = `../Prot/MessageDetails.aspx?MessageId=${nodeValue}&DocumentUnitId=${this.documentUnitId}`;
                this.openWindow(url, "searchMessages", 750, 300);
            }

            if (parentNode.get_text().startsWith("PEC ingresso")) {
                let nodeValue: string = args.get_node().get_value();
                let url: string = `../Prot/PECDetails.aspx?PECEntityId=${nodeValue}`;
                this.openWindow(url, "searchPECDetails", 750, 300);
            }

            if (parentNode.get_text().startsWith("PEC uscita")) {
                let nodeValue: string = args.get_node().get_value();
                let url: string = `../Prot/PECDetails.aspx?PECEntityId=${nodeValue}&Direction='Outgoing'`;
                this.openWindow(url, "searchPECDetails", 750, 300);
            }

            if (parentNode.get_text().startsWith("TNotice")) {
                var nodeValue: string = args.get_node().get_value();
                let url: string = `../Prot/TNoticeDetails.aspx?RequestId=${nodeValue}`;
                this.openWindow(url, "searchTNoticeDetails", 750, 300);
            }
        }

        if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.FascicleProtocol.toString()) {
            this.loadFasciclesData(this.documentUnitId, loadIndex);
        }

        if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.Fascicle.toString()) {
            this.loadFascicleData(this.documentUnitId, loadIndex);
        }

        if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.Dossier.toString()) {
            this.loadDossiersData(this.documentUnitId, loadIndex);
        }

        if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.ActiveWorkflows.toString()) {
            this.loadActiveWorkflowActivitiesData(this.documentUnitId, loadIndex);
        }

        if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.DoneWorkflows.toString()) {
            this.loadDoneWorkflowActivitiesData(this.documentUnitId, loadIndex);
        }

        if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.Atti.toString()) {
            this.loadDocumentSeriesItemLinkData(this.documentUnitId, loadIndex);
        }

        if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.PECIngresso.toString()) {
            this.loadIncomingPECMailData(this.documentUnitId, `'Incoming'`, loadIndex);
        }

        if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.PECUscita.toString()) {
            this.loadOutgoingPECMailData(this.documentUnitId, `'Outgoing'`, loadIndex);
        }

        if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.Archive.toString()) {
            this.loadUDSData(this.documentUnitId, loadIndex);
        }

        if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.ArchiveProtocol.toString()) {
            this.loadUDSProtocolData(this.documentUnitId, loadIndex);
        }

        if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.Protocol.toString()) {
            this.loadProtocolData(this.documentUnitId, loadIndex);
        }

        if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.ProtocolLinks.toString()) {
            this.loadProtocolLinkData(this.documentUnitId, loadIndex);
        }

        if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.ProtocolSeries.toString()) {
            this.loadDocumentSeriesProtocolsLinksData(this.documentUnitId, loadIndex);
        }

        if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.MessageResolution.toString()) {
            this.loadResolutionMessageData(this.documentUnitId, loadIndex);
        }

        if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.MessageProtocol.toString()) {
            this.loadMessageData(this.documentUnitId, loadIndex);
        }

        if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.MessageSeries.toString()) {
            this.loadDocumentSeriesMessageData(this.documentUnitId, loadIndex);
        }

        if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.Series.toString() && this.showResolutionDocumentSeriesLinks.toLowerCase() == "true") {
            this.loadResolutionDocumentSerieData(this.documentUnitId, loadIndex);
        }

        if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.SeriesProtocol.toString() && this.showResolutionDocumentSeriesLinks.toLowerCase() == "false") {
            this.loadDocumentSerieData(this.documentUnitId, loadIndex);
        }

        if (parentNodeAttribute == DocumentUnitReferenceTypeEnum.TNotice.toString()) {
            this.loadTNoticeData(this.documentUnitId, loadIndex);
        }
    }

    private loadDocumentSeriesProtocolsLinksData(uniqueId: string, position: number): void {
        if (!this.documentSeriesItemProtocol) {
            this._documentSeriesItemService.getDocumentSeriesItemProtocolById(uniqueId, (data) => {
                if (!data) return;
                this.documentSeriesItemProtocol = data;
                this.renderDocumentSerieItemProtocolNodes(this.documentSeriesItemProtocol, position);
            });
        } else {
            this.renderDocumentSerieItemProtocolNodes(this.documentSeriesItemProtocol, position);
        }
    }

    private renderDocumentSerieItemProtocolNodes(currentItems: DocumentSeriesItemsModel, position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        let protocolDocumentSeriesItems: ProtocolModel[] = currentItems.Protocols;
        let numberOfItems: number = currentItems.Protocols.length;

        for (let i = 0; i < numberOfItems; i++) {
            let documentSeriesItemProtocolName: string = `${protocolDocumentSeriesItems[i].Year}-${protocolDocumentSeriesItems[i].Number} - ${protocolDocumentSeriesItems[i].Object}`;
            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_text(documentSeriesItemProtocolName);
            node.set_value(protocolDocumentSeriesItems[i].UniqueId);

            node.set_navigateUrl(`../Prot/ProtVisualizza.aspx?UniqueId=${protocolDocumentSeriesItems[i].UniqueId}&Type=Prot`);


            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);

            this._DocumentRadTreeId.commitChanges();
        }
    }

    private loadDocumentSeriesMessageData(uniqueId: string, position: number): void {
        if (!this.documentSeriesItemList) {
            this._documentSeriesItemService.getDocumentSeriesItemById(uniqueId, (data) => {
                if (!data) return;
                this.documentSeriesItemList = data;
                this.renderDocumentSerieMessageNodes(this.documentSeriesItemList, position);
            });
        } else {
            this.renderDocumentSerieMessageNodes(this.documentSeriesItemList, position);
        }
    }

    private renderDocumentSerieMessageNodes(currentItems: DocumentSeriesItemsModel, position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        let numberOfItems: number = currentItems.Messages.length;
        let messages: MessageModel[] = currentItems.Messages;

        this.renderMessageNodes(numberOfItems, messages, position);
    }

    private loadDocumentSeriesItemLinkData(uniqueId: string, position: number): void {
        if (!this.documentSeriesItemLinksList) {
            this._documentSeriesItemLinkService.getDocumentSeriesItemLinksById(uniqueId, (data) => {
                if (!data) return;
                this.documentSeriesItemLinksList = data;
                this.renderDocumentSeriesItemLinkNodes(this.documentSeriesItemLinksList, position);
            });
        } else {
            this.renderDocumentSeriesItemLinkNodes(this.documentSeriesItemLinksList, position);
        }
    }

    private renderDocumentSeriesItemLinkNodes(currentItems: DocumentSeriesItemLinksModel, position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        let documentSeriesItemName: string = `${currentItems.Resolution.InclusiveNumber} - ${currentItems.Resolution.Object}`;
        let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        node.set_text(documentSeriesItemName);
        node.set_value(currentItems.Resolution.EntityId);

        node.set_navigateUrl(`../Resl/ReslVisualizza.aspx?Type=Resl&IdResolution=${currentItems.Resolution.EntityId}`);

        let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
        parentNode.get_nodes().add(node);

        this._DocumentRadTreeId.commitChanges();

    }

    private loadIncomingPECMailData(uniqueId: string, pecMailDirection: string, position: number): void {
        if (!this.pecMailIncomingAndOutgoing || this.pecMailIncomingAndOutgoing.length === 0) {
            this._pecMailService.getIncomingPECMail(uniqueId, pecMailDirection, (data) => {
                if (!data) return;
                this.pecMailIncomingAndOutgoing = data;
                this.renderIncomingPECMailNodes(this.pecMailIncomingAndOutgoing, position);
            });
        } else {
            this.renderIncomingPECMailNodes(this.pecMailIncomingAndOutgoing, position);
        }
    }

    private loadOutgoingPECMailData(uniqueId: string, direction: string, position: number): void {
        if (!this.pecMailOutgoings) {
            this._pecMailService.getOutgoingPECMail(uniqueId, direction, (data) => {
                if (!data) return;
                this.pecMailOutgoings = data;
                this.renderOutgoingPECMailNodes(this.pecMailOutgoings, position);
            });
        } else {
            this.renderOutgoingPECMailNodes(this.pecMailOutgoings, position);
        }
    }

    private loadFascicleData(uniqueId: string, position: number): void {
        if (!this.fascicleDocumentUnitList || this.fascicleDocumentUnitList.length === 0) {
            this._fascicleService.getAuthorizedFasciclesFromDocumentUnit(uniqueId, (data) => {
                if (!data) return;
                this.fascicleDocumentUnitList = data;
                this.renderFascicleNodes(this.fascicleDocumentUnitList, position);
            });
        } else {
            this.renderFascicleNodes(this.fascicleDocumentUnitList, position);
        }
    }

    private loadFasciclesData(uniqueId: string, position: number): void {
        if (!this.fascicleLinkList || this.fascicleLinkList.length === 0) {
            this._fascicleLinksService.getLinkedFasciclesById(uniqueId, (data) => {
                if (!data) return;
                this.fascicleLinkList = data;
                this.renderFasciclesNodes(this.fascicleLinkList, position);
            });
        } else {
            this.renderFasciclesNodes(this.fascicleLinkList, position);
        }
    }

    private loadDossiersData(uniqueId: string, position: number): void {
        if (!this.dossierFolderList || this.dossierFolderList.length === 0) {
            this._dossierFolderService.getByFascicleId(uniqueId, (data) => {
                if (!data) return;
                this.dossierFolderList = data;
                this.renderDossiersNodes(this.dossierFolderList, position);
            });
        } else {
            this.renderDossiersNodes(this.dossierFolderList, position);
        }
    }

    private loadActiveWorkflowActivitiesData(uniqueId: string, position: number): void {
        if (!this.activeWorkflowActivityList || this.activeWorkflowActivityList.length === 0) {
            this._workflowActivityService.getActiveByReferenceDocumentUnitId(uniqueId, (data: WorkflowActivityModel[]) => {
                if (!data) return;
                this.activeWorkflowActivityList = data;
                this.renderWorkflowActivityNodes(true, true, this.activeWorkflowActivityList, position);
            });
        } else {
            this.renderWorkflowActivityNodes(true, true, this.activeWorkflowActivityList, position);
        }
    }

    private loadDoneWorkflowActivitiesData(uniqueId: string, position: number): void {
        if (!this.doneWorkflowActivityList || this.doneWorkflowActivityList.length === 0) {
            this._workflowActivityService.getByStatusReferenceDocumentUnitId('Done', uniqueId, (data: WorkflowActivityModel[]) => {
                if (!data) return;
                this.doneWorkflowActivityList = data;
                this.renderWorkflowActivityNodes(true, false, this.doneWorkflowActivityList, position);
            });
        } else {
            this.renderWorkflowActivityNodes(true, false, this.doneWorkflowActivityList, position);
        }
    }

    private loadDocumentSerieData(uniqueId: string, position: number): void {
        if (!this.protocolDocumentSeries) {
            this._protocolService.getDocumentSerieslByUniqueId(uniqueId, (data) => {
                if (!data) return;
                this.protocolDocumentSeries = data;
                this.renderDocumentSerieNodes(this.protocolDocumentSeries, position);
            });
        } else {
            this.renderDocumentSerieNodes(this.protocolDocumentSeries, position);
        }
    }

    private loadResolutionDocumentSerieData(uniqueId: string, position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        if (!this.resolutionDocumentSeriesItem) {
            this._resolutionDocumentSeriesItemService.getResolutionDocumentSeriesItemLinks(uniqueId, (data) => {
                if (!data) return;
                this.resolutionDocumentSeriesItem = data;
                this.renderResolutionDocumentSeriesItemNode(this.resolutionDocumentSeriesItem, position);

            });
        } else {
            this.renderResolutionDocumentSeriesItemNode(this.resolutionDocumentSeriesItem, position);
        }
    }

    private loadResolutionMessageData(uniqueId: string, position: number): void {
        if (!this.resolutionMessage) {
            this._resolutionService.getResolutionMessageByUniqueId(uniqueId, (data) => {
                if (!data) return;
                this.resolutionMessage = data;
                this.renderResolutionMessageNodes(this.resolutionMessage, position);
            });
        } else {
            this.renderResolutionMessageNodes(this.resolutionMessage, position);
        }
    }

    private loadMessageData(uniqueId: string, position: number): void {
        if (!this.protocolMessages) {
            this._protocolService.getProtocolMessageById(uniqueId, (data) => {
                if (!data) return;
                this.protocolMessages = data;
                this.renderProtocolMessageNodes(this.protocolMessages, position);
            });
        } else {
            this.renderProtocolMessageNodes(this.protocolMessages, position);
        }
    }

    private loadUDSProtocolData(uniqueId: string, position: number): void {
        if (!this.udsIdList || this.udsIdList.length === 0) {
            this._udsService.getUDSByProtocolId(uniqueId, (data) => {
                if (!data) return;
                this.udsIdList = data;
                this.renderUDSByProtocolNodes(this.udsIdList, position);
            });
        } else {
            this.renderUDSByProtocolNodes(this.udsIdList, position);
        }
    }

    private loadProtocolLinkData(uniqueId: string, position: number): void {
        if (!this.protocolLinks || this.protocolLinks.length === 0) {
            this._protocolLinkService.getProtocolById(uniqueId, (data) => {
                if (!data) return;
                this.protocolLinks = data;
                this.renderProtocolLinkNodes(this.protocolLinks, position);
            });
        } else {
            this.renderProtocolLinkNodes(this.protocolLinks, position);
        }
    }

    private loadProtocolData(uniqueId: string, position: number): void {
        if (!this.protocolDocumentList || this.protocolDocumentList.length === 0) {
            this._udsService.getProtocolListById(uniqueId, (data) => {
                if (!data) return;
                this.protocolDocumentList = data;
                this.renderProtocolNodes(this.protocolDocumentList, position);
            });
        } else {
            this.renderProtocolNodes(this.protocolDocumentList, position);
        }
    }

    private loadUDSData(uniqueId: string, position: number): void {
        if (!this.udsDocumentList || this.udsDocumentList.length === 0) {
            this._udsService.getUDSListById(uniqueId, (data) => {
                if (!data) return;
                this.udsDocumentList = data;
                this.renderUDSNodes(this.udsDocumentList, position);
            });
        } else {
            this.renderUDSNodes(this.udsDocumentList, position);
        }
    }

    private loadTNoticeData(idDocumentUnit: string, position: number): void {
        if (!this.tNoticeSummaryList || this.tNoticeSummaryList.length === 0) {
            this._tnoticeService.getRequestsSummariesByDocumentId(idDocumentUnit, (data) => {
                if (!data) { return; }
                this.tNoticeSummaryList = data;
                this.renderTNoticeSummary(data, position);
            });
        } else {
            this.renderTNoticeSummary(this.tNoticeSummaryList, position);
        }
    }

    private loadDocumentSeriesMessageCount(uniqueId: string, position: number): void {
        this._messageService.countDocumentSeriesItemById(uniqueId, (data) => {
            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Messaggi (${data})`);
        });
    }

    private loadDocumentSeriesItemLinksCount(uniqueId: string, position: number): void {
        this._documentSeriesItemLinkService.countDocumentSeriesItemLinksById(uniqueId, (data) => {
            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Atti (${data})`);
        });
    }

    private loadDocumentSeriesProtocolsLinksCount(uniqueId: string, position: number): void {
        this._protocolService.countDocumentSeriesItemProtocolById(uniqueId, (data) => {
            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Protocolli (${data})`);
        });
    }

    private loadFascicleCount(uniqueId: string, position: number): void {
        this._fascicleService.countAuthorizedFasciclesFromDocumentUnit(uniqueId, (data) => {
            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Fascicoli (${data})`);
        });
    }

    private loadProtocolCount(uniqueId: string, position: number): void {
        this._udsService.countProtocolsById(uniqueId, (data) => {
            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Protocolli (${data})`);
        });
    }

    private loadProtocolLinkCount(uniqueId: string, position: number): void {
        this._protocolLinkService.countProtocolsById(uniqueId, (data) => {
            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Protocolli (${data})`);
        });
    }

    private loadUDSCount(uniqueId: string, position: number): void {
        this._udsService.countUDSById(uniqueId, (data) => {
            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Archivi (${data})`);
        });
    }

    private loadProtocolUDSCount(uniqueId: string, position: number): void {
        this._udsService.countUDSByRelationId(uniqueId, (data) => {
            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Archivi (${data})`);
        });
    }

    private loadProtocolMessageCount(uniqueId: string, position: number): void {
        this._messageService.countProtocolMessagesById(uniqueId, (data) => {
            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Messaggi (${data})`);
        });
    }

    private loadProtocolDocumentSeriesCount(uniqueId: string, position: number): void {
        this._documentSeriesItemService.countDocumentSeriesItemById(uniqueId, (data) => {
            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            let nodeText: string = this.seriesTitle ? this.seriesTitle : "Serie documentali";
            parentNode.set_text(`${nodeText} (${data})`);
        });
    }

    private loadIncomingPECMailCount(uniqueId: string, direction: string, position: number): void {
        this._pecMailService.countIncomingPECMail(uniqueId, direction, (data) => {
            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`PEC ingresso (${data})`);
        });
    }

    private loadOutgoingPECMailCount(uniqueId: string, direction: string, position: number): void {
        this._pecMailService.countOutgoingPECMail(uniqueId, direction, (data) => {
            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`PEC uscita (${data})`);
        });
    }

    private loadResolutionMessageCount(uniqueId: string, position: number): void {
        this._messageService.countResolutionMessagesById(uniqueId, (data) => {
            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Messaggi (${data})`);
        });
    }

    private loadDocumentSeriesItemCount(uniqueId: string, position: number): void {
        this._resolutionDocumentSeriesItemService.getResolutionDocumentSeriesItemLinksCount(uniqueId, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            const nodeText: string = this.seriesTitle ? this.seriesTitle : "Serie documentali";
            parentNode.set_text(`${nodeText} (${data})`);
        });

    }

    private loadFasciclesCount(uniqueId: string, position: number): void {
        this._fascicleLinksService.countLinkedFascicleById(uniqueId, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Fascicoli (${data})`);
        });
    }

    private loadDossierCount(uniqueId: string, position: number): void {
        this._dossierFolderService.countByFascicleId(uniqueId, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Dossiers (${data})`);
        });
    }

    private loadActiveWorkflowActivitiesCount(uniqueId: string, position: number): void {
        this._workflowActivityService.countActiveByReferenceDocumentUnitId(uniqueId, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Flussi di lavoro attivi (${data})`);
        });
    }

    private loadDoneWorkflowActivitiesCount(uniqueId: string, position: number): void {
        this._workflowActivityService.countByStatusReferenceDocumentUnitId('Done', uniqueId, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Flussi di lavoro conclusi (${data})`);
        });
    }

    private loadTNoticeCount(idDocumentUnit: string, position: number): void {
        this._tnoticeService.countRequestsSummariesByDocumentId(idDocumentUnit, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`TNotice (${data})`);
        });
    }

    private renderTNoticeSummary(currentItems: TNoticeStatusSummaryDTO[], position: number): void {
        let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
        parentNode.get_nodes().clear();

        for (let summary of currentItems) {
            var node = new Telerik.Web.UI.RadTreeNode();

            let displayText = summary.Status;

            if (summary.RegistrationDate) {
                displayText = `${displayText} - ${summary.RegistrationDate}`;
            }

            node.set_text(displayText);
            node.set_value(summary.RequestUniqueId);

            if (summary.DisplayColor == StatusColor.Blue) {
                node.set_imageUrl("../Comm/Images/pec-accettazione.gif");
            }

            if (summary.DisplayColor == StatusColor.Yellow) {
                node.set_imageUrl("../Comm/Images/pec-preavviso-errore-consegna.gif");
            }

            if (summary.DisplayColor == StatusColor.Green) {
                node.set_imageUrl("../Comm/Images/pec-avvenuta-consegna.gif");
            }

            if (summary.DisplayColor == StatusColor.Red) {
                node.set_imageUrl("../Comm/Images/pec-errore-consegna.gif");
            }

            parentNode.get_nodes().add(node);

        }

        this._DocumentRadTreeId.commitChanges();
    }

    private renderResolutionDocumentSeriesItemNode(currentItems: ResolutionDocumentSeriesItemModel[], position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        for (let currentItem of currentItems) {
            let documentSeriesItemName: string = `${currentItem.DocumentSeriesItem.DocumentSeries.Name} ${currentItem.DocumentSeriesItem.Year} ` +
                `${this.pad(currentItem.DocumentSeriesItem.Number, 7)} del ${moment(currentItem.DocumentSeriesItem.RegistrationDate).format("DD/MM/YYYY")} ` +
                `${currentItem.DocumentSeriesItem.PublishingDate !== null ? `Pubblicata il: ${moment(currentItem.DocumentSeriesItem.PublishingDate).format("DD/MM/YYYY")}` : ""} ` +
                `${currentItem.DocumentSeriesItem.RetireDate !== null ? `Ritirato il: ${moment(currentItem.DocumentSeriesItem.RetireDate).format("DD/MM/YYYY")}` : ""}`;
            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();

            node.set_text(documentSeriesItemName);
            node.set_value(currentItems[0].EntityId);

            node.set_navigateUrl(`../Series/Item.aspx?Type=Series&Action=2&IdDocumentSeriesItem=${currentItem.DocumentSeriesItem.EntityId}`);

            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);

            this._DocumentRadTreeId.commitChanges();
        }

    }

    private renderIncomingPECMailNodes(currentItems: PECMailViewModel[], position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        for (let pecMails of currentItems) {

            let recipients: string = pecMails.MailRecipients;
            let senderAndRecipient: string = `${pecMails.MailSenders} ${recipients}`;

            let result: string = "";
            if (pecMails.MailDate) {
                result = `${senderAndRecipient} - ${moment(pecMails.MailDate).format("DD/MM/YYYY, HH:mm:ss")}`;
            } else {
                result = senderAndRecipient;
            }

            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_text(result);
            node.set_value(pecMails.EntityId);

            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);

        }

    }

    private renderDocumentSerieNodes(currentItems: ProtocolModel, position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        let protocolDocumentSeriesItems: DocumentSeriesItemModel[] = currentItems.DocumentSeriesItems;
        let numberOfItems: number = currentItems.DocumentSeriesItems.length;

        for (let i = 0; i < numberOfItems; i++) {
            let regDate: string = moment(protocolDocumentSeriesItems[i].RegistrationDate).format("DD/MM/YYYY");
            let publishDate: string = moment(protocolDocumentSeriesItems[i].PublishingDate).format("DD/MM/YYYY");
            let retDate: string = moment(protocolDocumentSeriesItems[i].RetireDate).format("DD/MM/YYYY");

            let documentSeriesItemTitle: string = `${protocolDocumentSeriesItems[i].Subject} n. ${protocolDocumentSeriesItems[i].Year}/${protocolDocumentSeriesItems[i].Number} - Reg: ${regDate} - Pubb: ${publishDate} - Rit: ${retDate}`;

            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_text(documentSeriesItemTitle);
            node.set_value(protocolDocumentSeriesItems[i].UniqueId);
            node.set_navigateUrl(`../Series/Item.aspx?Type=Series&Action=2&IdDocumentSeriesItem=${protocolDocumentSeriesItems[i].EntityId}`);

            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);

            this._DocumentRadTreeId.commitChanges();
        }
    }

    private renderOutgoingPECMailNodes(currentItems: PECMailViewModel[], position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        for (let outGoingPECMails of currentItems) {

            let recipients: string = outGoingPECMails.MailRecipients;
            let senderAndRecipient: string = `${outGoingPECMails.MailSenders} ${recipients}`;
            let result: string = "";
            if (outGoingPECMails.MailDate) {
                result = `${senderAndRecipient} - ${moment(outGoingPECMails.MailDate).format("DD/MM/YYYY, HH:mm:ss")}`;
            } else {
                result = senderAndRecipient;
            }

            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_text(result);
            node.set_value(outGoingPECMails.EntityId);
            node.set_cssClass("text-label");

            let numberOfItems: number = outGoingPECMails.PECMailReceipts.length;
            let pecMailsReceipts: PECMailReceiptsModel[] = outGoingPECMails.PECMailReceipts;

            let nodeImages: string = "";

            for (let i = 0; i < numberOfItems; i++) {
                let status: string = pecMailsReceipts[i].ReceiptType;

                if (status == "accettazione") {
                    node.set_imageUrl("../Comm/Images/pec-accettazione.gif");
                    node.set_toolTip("accettazione");
                    nodeImages = `${nodeImages}<img class="rtImg" alt = "" src = "../Comm/Images/pec-accettazione.gif" title="accettazione">`;
                }

                if (status == "avvenuta-consegna") {
                    node.set_imageUrl("../Comm/Images/pec-avvenuta-consegna.gif");
                    node.set_toolTip("avvenuta-consegna");
                    nodeImages = `${nodeImages}<img class="rtImg" alt = "" src = "../Comm/Images/pec-avvenuta-consegna.gif" title="avvenuta-consegna">`;
                }

                if (status == "non-accettazione") {
                    node.set_imageUrl("../Comm/Images/pec-non-accettazione.gif");
                    node.set_toolTip("non-accettazione");
                    nodeImages = `${nodeImages}<img class="rtImg" alt = "" src = "../Comm/Images/pec-non-accettazione.gif" title="non-accettazione">`;
                }

                if (status == "preavviso-errore-consegna") {
                    node.set_imageUrl("../Comm/Images/pec-preavviso-errore-consegna.gif");
                    node.set_toolTip("preavviso-errore-consegna");
                    nodeImages = `${nodeImages}<img class="rtImg" alt = "" src = "../Comm/Images/preavviso-errore-consegna.gif" title="preavviso-errore-consegna">`;
                }

                if (status == "errore-consegna") {
                    node.set_imageUrl("../Comm/Images/pec-errore-consegna.gif");
                    node.set_toolTip("errore-consegna");
                    nodeImages = `${nodeImages}<img class="rtImg" alt = "" src = "../Comm/Images/pec-errore-consegna.gif" title="errore-consegna">`;
                }
            }

            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);
            if (nodeImages) {
                node.get_imageElement().outerHTML = nodeImages
            }
            this._DocumentRadTreeId.commitChanges();

        }
    }

    private renderResolutionMessageNodes(currentItems: ResolutionModel, position: number): void {
        let messages: MessageModel[] = currentItems.Messages;
        let numberOfItems: number = currentItems.Messages.length;
        this.renderMessageNodes(numberOfItems, messages, position)
    }

    private renderProtocolMessageNodes(currentItems: ProtocolModel[], position: number): void {
        let numberOfItems: number = currentItems[0].Messages.length;
        let messages: MessageModel[] = currentItems[0].Messages;
        this.renderMessageNodes(numberOfItems, messages, position)
    }

    private renderFascicleNodes(currentItems: FascicleModel[], position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        for (let fascDoc of currentItems) {
            let fascicleDocumentsName: string = `${fascDoc.Title} - ${fascDoc.FascicleObject}`;

            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_text(fascicleDocumentsName);
            node.set_value(fascDoc.UniqueId);
            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/fascicle_open.png");
            node.set_navigateUrl(`../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=${fascDoc.UniqueId}`);

            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);
            this._DocumentRadTreeId.commitChanges();
        }
    }

    private renderFasciclesNodes(currentItems: FascicleLinkModel[], position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        for (let fascicles of currentItems) {
            let fascicleName: string = `${fascicles.FascicleLinked.Title} - ${fascicles.FascicleLinked.FascicleObject}`;

            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_text(fascicleName);
            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/fascicle_open.png");
            node.set_navigateUrl(`../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=${fascicles.FascicleLinked.UniqueId}`);

            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);

            this._DocumentRadTreeId.commitChanges();
        }
    }

    private renderDossiersNodes(currentItems: DossierFolderModel[], position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        for (const dossierFolder of currentItems) {
            const dossierName = `${dossierFolder.Dossier.Year}/${this.pad(+dossierFolder.Dossier.Number, 7)} - ${dossierFolder.Dossier.Subject}`;

            const node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_text(dossierName);
            node.set_value(dossierFolder.Dossier.UniqueId);
            node.set_imageUrl("../Comm/Images/DocSuite/Dossier_16.png");
            node.set_navigateUrl(`../Dossiers/DossierVisualizza.aspx?Type=Dossier&IdDossier=${dossierFolder.Dossier.UniqueId}`);

            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);

            this._DocumentRadTreeId.commitChanges();
        }
    }

    private renderWorkflowActivityNodes(includeName: boolean, includeRegistrationUser: boolean, currentItems: WorkflowActivityModel[], position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        for (const workflowActivity of currentItems) {
            let name = '';
            if (includeName && includeName === true) {
                name = workflowActivity.Name
            }
            if (workflowActivity.Subject && workflowActivity.Subject !== '') {
                name = `${name} - ${workflowActivity.Subject}`
            }
            name = `${name} del ${workflowActivity.RegistrationDateFormatted}`
            if (includeRegistrationUser && includeRegistrationUser === true) {
                name = `${name} richiesta da ${workflowActivity.RegistrationUser}`
            }

            const node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();

            node.set_text(name);
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);
            this._DocumentRadTreeId.commitChanges();
        }
    }

    private renderMessageNodes(numberOfItems: number, messages: any[], position: number) {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        for (let i = 0; i < numberOfItems; i++) {
            const status: string = messages[i].Status;
            //Inviato in data <data> a : elenco destinatari
            const protocolMessageName: string = `${messages[i].MessageContacts.filter(f => f.ContactPosition == MessageContactPosition.Sender).map(f => f.Description).join("; ")} ${messages[i].MessageEmails[0].SentDate ? 'ha inviato in data ' + moment(messages[i].MessageEmails[0].SentDate).format("DD/MM/YYYY") : 'invio in corso'} a ${messages[i].MessageContacts.filter(f => f.ContactPosition == MessageContactPosition.Recipient || f.ContactPosition == MessageContactPosition.RecipientBcc).map(f => f.Description).join("; ")}`;;
            const idMessage: number = messages[i].EntityId;
            const node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_text(protocolMessageName);
            node.set_value(idMessage);

            if (status == "Active") {
                node.set_imageUrl("../Comm/Images/pec-accettazione.gif");
                node.set_toolTip("Invio in corso");
            }

            if (status == "Sent") {
                node.set_imageUrl("../Comm/Images/pec-avvenuta-consegna.gif");
                node.set_toolTip("Inviato");
            }

            if (status == "Error") {
                node.set_imageUrl("../Comm/Images/pec-errore-consegna.gif");
                node.set_toolTip("Errore");
            }

            if (status == "Draft") {
                node.set_imageUrl("../Comm/Images/pec-non-accettazione.gif");
                node.set_toolTip("Bozza");
            }

            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);

            this._DocumentRadTreeId.commitChanges();

        }
    }

    private renderUDSByProtocolNodes(currentItems: UDSDocumentUnitModel[], position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        for (let udsDoc of currentItems) {
            let udsDocumentsName: string = `${udsDoc.SourceUDS.Title} - ${udsDoc.SourceUDS.Subject}`;

            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_text(udsDocumentsName);
            node.set_value(udsDoc.SourceUDS.UniqueId);
            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/document_copies.png");
            if (udsDoc.SourceUDS.UDSRepository) {
                node.set_navigateUrl(`../UDS/UDSView.aspx?Type=UDS&IdUDS=${udsDoc.IdUDS}&IdUDSRepository=${udsDoc.SourceUDS.UDSRepository.UniqueId.toString()}`);
            }
            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);
            this._DocumentRadTreeId.commitChanges();
        }
    }

    private renderProtocolLinkNodes(currentItems: ProtocolLinkModel[], position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        for (let protLink of currentItems) {
            let protocolLinkName: string = `${protLink.ProtocolLinked.Year}-${protLink.ProtocolLinked.Number} - ${protLink.ProtocolLinked.Object}`;

            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_text(protocolLinkName);
            node.set_value(protLink.ProtocolLinked.UniqueId);
            node.set_imageUrl("../Comm/Images/DocSuite/Protocollo16.gif");
            node.set_navigateUrl(`../Prot/ProtVisualizza.aspx?UniqueId=${protLink.ProtocolLinked.UniqueId}&Type=Prot`);

            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position)
            parentNode.get_nodes().add(node);
            this._DocumentRadTreeId.commitChanges();
        }
    }

    private renderProtocolNodes(currentItems: UDSDocumentUnitModel[], position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        for (let protDoc of currentItems) {
            let protocolDocumentsName: string = `${protDoc.Relation.Title} - ${protDoc.Relation.Subject}`;

            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_text(protocolDocumentsName);
            node.set_value(protDoc.Relation.UniqueId);
            node.set_imageUrl("../Comm/Images/DocSuite/Protocollo16.gif");
            node.set_navigateUrl(`../Prot/ProtVisualizza.aspx?UniqueId=${protDoc.Relation.UniqueId}&Type=Prot`);

            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);
            this._DocumentRadTreeId.commitChanges();
        }
    }

    private renderUDSNodes(currentItems: UDSDocumentUnitModel[], position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
        for (let udsDoc of currentItems) {
            let udsDocumentsName: string = `${udsDoc.Relation.Title} - ${udsDoc.Relation.Subject}`;

            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_text(udsDocumentsName);
            node.set_value(udsDoc.Relation.UniqueId);
            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/document_copies.png");
            if (udsDoc.Relation.UDSRepository) {
                node.set_navigateUrl(`../UDS/UDSView.aspx?Type=UDS&IdUDS=${udsDoc.Relation.UniqueId.toString()}&IdUDSRepository=${udsDoc.Relation.UDSRepository.UniqueId.toString()}`);
            }
            let parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);

            var contentElement: any = node.get_contentElement();

            if (this.showRemoveUDSLinksButton.toLowerCase() == "true") {
                contentElement.innerHTML = `<img src=\"${node.get_imageUrl()}\"/><a href=\"${node.get_navigateUrl()}\">${node.get_text()}</a>
                <img src = \"../App_Themes/DocSuite2008/imgset16/remove.png\" style=\"margin-left: 5px;\" onclick='removeLink(\"${udsDoc.UniqueId}\", \"${udsDoc.IdUDS}\", \"${udsDoc.Relation.UniqueId}\");'>`;
            }

            this._DocumentRadTreeId.commitChanges();
        }
    }

    removeLink(uniqueId: any, udsId: any, relationId: any) {
        if (window.confirm("Vuoi eliminare l'archivio selezionato?")) {
            let from: UDSDocumentUnitModel = <UDSDocumentUnitModel>{};
            from.UniqueId = uniqueId;

            this._udsService.getUDSById(relationId, udsId, (data) => {
                this._udsService.deleteUDSByid(from, () => {
                    this._udsService.deleteUDSByid(data[0], () => {
                        window.location.reload();
                        alert("Collegamento eliminato correttamente");
                    }, (exception: ExceptionDTO) => {
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
                }, (exception: ExceptionDTO) => {
                    this.showNotificationException(this.uscNotificationId, exception);
                });
            }, (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
            });
        }
    }

    protected showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${uscNotificationId}`).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(uscNotificationId, customMessage)
        }
    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${uscNotificationId}`).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage);
        }
    }

    private openWindow(url: string, name: string, width: number, height: number): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.managerWindowsId);
        let wnd: Telerik.Web.UI.RadWindow = manager.open(url, name, null);
        wnd.setSize(width, height);
        wnd.set_modal(true);
        wnd.center();
        return false;
    }

    private pad(currentNumber: number, paddingSize: number): string {
        let s = currentNumber + "";
        while (s.length < paddingSize) {
            s = `0${s}`
        }
        return s;
    }
    /**
* Evento al click del pulsante per la espandere o comprimere il sommario
*/
    btnExpandDocumentUnitReference_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        if (this._isDocumentUnitReferenceOpen) {
            this._isDocumentUnitReferenceOpen = false;
            this._documentUnitReferenceContent.hide();
            this._btnExpandDocumentUnitReference.removeCssClass("dsw-arrow-down");
            this._btnExpandDocumentUnitReference.addCssClass("dsw-arrow-up");
        }
        else {
            this._isDocumentUnitReferenceOpen = true;
            this._documentUnitReferenceContent.show();
            this._btnExpandDocumentUnitReference.removeCssClass("dsw-arrow-up");
            this._btnExpandDocumentUnitReference.addCssClass("dsw-arrow-down");
        }
    }

}
export = uscDocumentUnitReferences;