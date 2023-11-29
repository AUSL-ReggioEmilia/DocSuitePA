import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import EnumHelper = require("App/Helpers/EnumHelper");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
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
import PECMailDirection = require("App/Models/PECMails/PECMailDirection");
import DocumentUnitReferenceLinkTypeEnum = require("App/Models/Commons/DocumentUnitReferenceLinkTypeEnum");
import WorkflowStatus = require("App/Models/Workflows/WorkflowStatus");
import FascicleLogService = require("App/Services/Fascicles/FascicleLogService");
import FascicleLogViewModel = require("App/ViewModels/Fascicles/FascicleLogViewModel");
import FascicleLogType = require("App/Models/Fascicles/FascicleLogType");
import PECMailActiveTypeEnum = require("App/Models/PECMails/PECMailActiveTypeEnum");
import FascicleDocumentUnitService = require("App/Services/Fascicles/FascicleDocumentUnitService");
import PaginationModel = require("App/Models/Commons/PaginationModel");
import PECMailDirectionProcessStatusEnum = require("App/Models/PECMails/PECMailDirectionProcessStatusEnum");

class uscDocumentUnitReferences {
    radTreeDocumentsId: string;
    referenceUniqueId: string;
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
    pecMails: PECMailViewModel[];
    documentSeriesItemList: DocumentSeriesItemsModel;
    documentSeriesItemLinksList: DocumentSeriesItemLinksModel;
    documentSeriesItemProtocol: DocumentSeriesItemsModel;
    resolutionMessage: ResolutionModel;
    resolutionDocumentSeriesItem: ResolutionDocumentSeriesItemModel[];
    fascicleLinkList: FascicleLinkModel[];
    fascicleList: FascicleModel[];
    documentUnitFascicleLinksList: FascicleModel[];
    dossierFolderList: DossierFolderModel[];
    workflowActivityList: WorkflowActivityModel[];
    tNoticeSummaryList: TNoticeStatusSummaryDTO[];
    showRemoveUDSLinksButton: string;
    deletedFascicleDocumentUnitList: FascicleLogViewModel[];
    deletedFascicleDocumentList: FascicleLogViewModel[];
    docSuiteNextBaseUrl: string;

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
    showPECUnifiedLinks: string;
    showResolutionlMessageLinks: string;
    showResolutionDocumentSeriesLinks: string;
    showFasciclesLinks: string;
    showDossierLinks: string;
    showDeletedFascicleDocumentUnits: string;
    showDeletedFascicleDocuments: string;
    showTNotice: string;
    showWorkflowActivities: string;
    showDocumentUnitFascicleLinks: string;

    btnExpandDocumentUnitReferenceId: string;
    documentUnitReferenceInfoId: string;

    private _DocumentRadTreeId: Telerik.Web.UI.RadTreeView;
    private _serviceConfigurations: ServiceConfiguration[];
    private _enumHelper: EnumHelper;
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
    private _fascicleLogService: FascicleLogService;
    private _dossierFolderService: DossierFolderService;
    private _fascicleDocumentUnitService: FascicleDocumentUnitService;
    private _tnoticeService: TNoticeService;
    private _workflowActivityService: WorkflowActivityService;
    private _btnExpandDocumentUnitReference: Telerik.Web.UI.RadButton;
    private _isDocumentUnitReferenceOpen: boolean;
    private _documentUnitReferenceContent: JQuery;
    private index: number = 0;
    private static Attribute_NodeType = "ParentNodeType";
    private static Attribute_LinkType = "LinkType";
    private static Attribute_Direction = "Direction";
    private static Attribute_OpenInDocSuiteNext = "OpenInDocSuiteNext";
    private static Attribute_UniqueId = "Attribute_UniqueId";
    private static LOAD_MORE_NODE_IMAGEURL: string = "../App_Themes/DocSuite2008/imgset16/add.png";
    private static LOAD_MORE_NODE_TYPE: string = "LoadMore";
    private static LOAD_MORE_NODE_LABEL: string = "Carica più elementi";
    private static TOTAL_CHILDREN_COUNT_ATTRNAME: string = "totalChildrenCount";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
        //$(document).ready(() => {
        //});
    }

    private get _defaultPaginationModel(): PaginationModel {
        return new PaginationModel(0, 100);
    }

    initialize(): void {
        this._btnExpandDocumentUnitReference = <Telerik.Web.UI.RadButton>$find(this.btnExpandDocumentUnitReferenceId);
        this._btnExpandDocumentUnitReference.addCssClass("dsw-arrow-down");
        this._btnExpandDocumentUnitReference.add_clicking(this.btnExpandDocumentUnitReference_OnClick);
        this._documentUnitReferenceContent = $("#".concat(this.documentUnitReferenceInfoId));
        this._documentUnitReferenceContent.show();


        let fascicleLogServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleLog");
        this._fascicleLogService = new FascicleLogService(fascicleLogServiceConfiguration);

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

        let fascicleDocumentUnitServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleDocumentUnit");
        this._fascicleDocumentUnitService = new FascicleDocumentUnitService(fascicleDocumentUnitServiceConfiguration);

        let workflowActivityServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowActivity");
        this._workflowActivityService = new WorkflowActivityService(workflowActivityServiceConfiguration);

        let polRequestConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "POLRequest");
        this._tnoticeService = new TNoticeService(polRequestConfiguration);

        this._DocumentRadTreeId = $find(this.radTreeDocumentsId) as Telerik.Web.UI.RadTreeView;

        if (this.administrationTrasparenteProtocol.toLocaleLowerCase() == "true" && this.showProtocolDocumentSeriesLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory(this.seriesTitle ? this.seriesTitle : "Serie documentali", DocumentUnitReferenceTypeEnum.SeriesProtocol, DocumentUnitReferenceLinkTypeEnum.SeriesItem, () => this.loadProtocolDocumentSeriesCount(this.referenceUniqueId, this.index));
        }

        if (this.showResolutionDocumentSeriesLinks.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory(this.seriesTitle ? this.seriesTitle : "Serie documentali", DocumentUnitReferenceTypeEnum.Series, DocumentUnitReferenceLinkTypeEnum.SeriesItem, () => this.loadDocumentSeriesItemCount(this.referenceUniqueId, this.index));
        }

        if (this.showArchiveRelationLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Archivi", DocumentUnitReferenceTypeEnum.ArchiveProtocol, DocumentUnitReferenceLinkTypeEnum.Archive, () => this.loadProtocolUDSCount(this.referenceUniqueId, this.index));
        }

        if (this.showArchiveLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Archivi", DocumentUnitReferenceTypeEnum.Archive, DocumentUnitReferenceLinkTypeEnum.Archive, () => this.loadUDSCount(this.referenceUniqueId, this.index));
        }

        if (this.showDocumentSeriesResolutionsLinks.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Atti", DocumentUnitReferenceTypeEnum.Atti, DocumentUnitReferenceLinkTypeEnum.Atti, () => this.loadDocumentSeriesItemLinksCount(this.referenceUniqueId, this.index));
        }

        if (this.showDeletedFascicleDocumentUnits.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Documenti eliminati", DocumentUnitReferenceTypeEnum.DeletedFascicleDocumentUnit, DocumentUnitReferenceLinkTypeEnum.None, () => this.loadDeletedFascicleDocumentUnitsCount(this.referenceUniqueId, this.index));
        }

        if (this.showDeletedFascicleDocuments.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Inserti eliminati", DocumentUnitReferenceTypeEnum.DeletedFascicleDocument, DocumentUnitReferenceLinkTypeEnum.None, () => this.loadDeletedFascicleDocumentsCount(this.referenceUniqueId, this.index));
        }

        if (this.showDossierLinks.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Dossiers", DocumentUnitReferenceTypeEnum.Dossier, DocumentUnitReferenceLinkTypeEnum.Dossier, () => this.loadDossierCount(this.referenceUniqueId, this.index));
        }

        if (this.showFascicleLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Fascicoli", DocumentUnitReferenceTypeEnum.Fascicle, DocumentUnitReferenceLinkTypeEnum.Fascicle, () => this.loadFascicleCount(this.referenceUniqueId, this.index));
        }

        if (this.showFasciclesLinks.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Fascicoli", DocumentUnitReferenceTypeEnum.FascicleProtocol, DocumentUnitReferenceLinkTypeEnum.Fascicle, () => this.loadFasciclesCount(this.referenceUniqueId, this.index));
        }

        if (this.showDocumentUnitFascicleLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Fascicoli", DocumentUnitReferenceTypeEnum.FascicleDocumentUnitLink, DocumentUnitReferenceLinkTypeEnum.None, () => this.loadFascicleDocumentUnitCount(this.referenceUniqueId, this.index));
        }

        if (this.showWorkflowActivities.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Registro attività", DocumentUnitReferenceTypeEnum.Workflows, DocumentUnitReferenceLinkTypeEnum.None, () => this.loadWorkflowActivitiesCount(this.referenceUniqueId, this.index));
        }
        if (this.showResolutionlMessageLinks.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Messaggi", DocumentUnitReferenceTypeEnum.MessageResolution, DocumentUnitReferenceLinkTypeEnum.Message, () => this.loadResolutionMessageCount(this.referenceUniqueId, this.index));
        }

        if (this.showProtocolMessageLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Messaggi", DocumentUnitReferenceTypeEnum.MessageProtocol, DocumentUnitReferenceLinkTypeEnum.Message, () => this.loadProtocolMessageCount(this.referenceUniqueId, this.index));
        }

        if (this.showDocumentSeriesMessageLinks.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Messaggi", DocumentUnitReferenceTypeEnum.MessageSeries, DocumentUnitReferenceLinkTypeEnum.Message, () => this.loadDocumentSeriesMessageCount(this.referenceUniqueId, this.index));
        }

        if (this.showIncomingPECMailLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory("PEC ingresso", DocumentUnitReferenceTypeEnum.PECIncoming, DocumentUnitReferenceLinkTypeEnum.PEC, () => this.loadIncomingPECMailCount(this.referenceUniqueId, `'Incoming'`, this.index));
        }

        if (this.showOutgoingPECMailLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory("PEC uscita", DocumentUnitReferenceTypeEnum.PECOutgoing, DocumentUnitReferenceLinkTypeEnum.PEC, () => this.loadOutgoingPECMailCount(this.referenceUniqueId, `'Outgoing'`, this.index))
        }

        if (this.showPECUnifiedLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory("PEC", DocumentUnitReferenceTypeEnum.PECUnified, DocumentUnitReferenceLinkTypeEnum.PEC, () => this.loadPECMailCount(this.referenceUniqueId, this.index))
        }

        if (this.showProtocolRelationLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Protocolli", DocumentUnitReferenceTypeEnum.ProtocolLinks, DocumentUnitReferenceLinkTypeEnum.Protocol, () => this.loadProtocolLinkCount(this.referenceUniqueId, this.index));
        }

        if (this.showProtocolLinks.toLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Protocolli", DocumentUnitReferenceTypeEnum.Protocol, DocumentUnitReferenceLinkTypeEnum.Protocol, () => this.loadProtocolCount(this.referenceUniqueId, this.index));
        }

        if (this.showDocumentSeriesProtocolsLinks.toLocaleLowerCase() == "true" && this.protocolDocumentSeriesButtonEnable.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory("Protocolli", DocumentUnitReferenceTypeEnum.ProtocolSeries, DocumentUnitReferenceLinkTypeEnum.SeriesItem, () => this.loadDocumentSeriesProtocolsLinksCount(this.referenceUniqueId, this.index));
        }

        if (this.showTNotice.toLocaleLowerCase() == "true") {
            this.evaluateNodePropertyFactory("TNotice", DocumentUnitReferenceTypeEnum.TNotice, DocumentUnitReferenceLinkTypeEnum.TNotice, () => this.loadTNoticeCount(this.referenceUniqueId, this.index));
        }

        this._DocumentRadTreeId.add_nodeClicked(this.generalOnNodeExpanding);
    }

    private evaluateNodePropertyFactory(nodeText: string, value: DocumentUnitReferenceTypeEnum, link: DocumentUnitReferenceLinkTypeEnum, callback: any) {
        const node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        node.set_text(nodeText);
        node.set_cssClass("font_node");
        node.get_attributes().setAttribute(uscDocumentUnitReferences.Attribute_NodeType, value);
        node.get_attributes().setAttribute(uscDocumentUnitReferences.Attribute_LinkType, link);
        this._DocumentRadTreeId.get_nodes().add(node);
        callback();
        this.index++;
    }

    generalOnNodeExpanding = (sender, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        let node: Telerik.Web.UI.RadTreeNode = args.get_node();
        node.toggle();
        node = args.get_node();
        const nodeTypeAttribute: string = node.get_attributes().getAttribute(uscDocumentUnitReferences.Attribute_NodeType);
        const loadIndex = node.get_index();
        const parentNode = node.get_parent();

        if (node.get_level() != 0) {
            const linkTypeAttribute: string = parentNode.get_attributes().getAttribute(uscDocumentUnitReferences.Attribute_LinkType);
            const directionAttribute: string = node.get_attributes().getAttribute(uscDocumentUnitReferences.Attribute_Direction);
            const openInDocSuiteNextAttribute: boolean = node.get_attributes().getAttribute(uscDocumentUnitReferences.Attribute_OpenInDocSuiteNext)
            const uniqueIdAttribute: string = node.get_attributes().getAttribute(uscDocumentUnitReferences.Attribute_UniqueId);

            if (linkTypeAttribute == DocumentUnitReferenceLinkTypeEnum.Message.toString()) {
                const nodeValue: string = node.get_value();
                const url = `../Prot/MessageDetails.aspx?MessageId=${nodeValue}&DocumentUnitId=${this.referenceUniqueId}`;
                this.openWindow(url, "searchMessages", 750, 300);
            }

            if (linkTypeAttribute == DocumentUnitReferenceLinkTypeEnum.PEC.toString()) {
                if (nodeTypeAttribute === uscDocumentUnitReferences.LOAD_MORE_NODE_TYPE) {
                    this.loadMorePECMailData(this.referenceUniqueId, parentNode.get_index());
                    return;
                }
                if (openInDocSuiteNextAttribute && openInDocSuiteNextAttribute === true) {
                    const url = `${this.docSuiteNextBaseUrl}${uniqueIdAttribute}`;
                    window.open(url, '_blank').focus();
                }
                else {
                    const nodeValue: string = node.get_value();
                    const url = `../Prot/PECDetails.aspx?PECEntityId=${nodeValue}&Direction='${directionAttribute}'`;
                    this.openWindow(url, "searchPECDetails", 750, 300);
                }
            }

            if (linkTypeAttribute == DocumentUnitReferenceLinkTypeEnum.TNotice.toString()) {
                const nodeValue: string = node.get_value();
                const url = `../Prot/TNoticeDetails.aspx?RequestId=${nodeValue}`;
                this.openWindow(url, "searchTNoticeDetails", 750, 300);
            }
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.FascicleProtocol.toString()) {
            this.loadFasciclesData(this.referenceUniqueId, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.FascicleDocumentUnitLink.toString()) {
            this.loadFasciclesFromDocumentUnitData(this.referenceUniqueId, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.Fascicle.toString()) {
            this.loadFascicleData(this.referenceUniqueId, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.DeletedFascicleDocumentUnit.toString()) {
            this.loadDeletedFascicleDocumentUnitData(this.referenceUniqueId, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.DeletedFascicleDocument.toString()) {
            this.loadDeletedFascicleDocumentData(this.referenceUniqueId, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.Dossier.toString()) {
            this.loadDossiersData(this.referenceUniqueId, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.Workflows.toString()) {
            this.loadWorkflowActivitiesData(this.referenceUniqueId, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.Atti.toString()) {
            this.loadDocumentSeriesItemLinkData(this.referenceUniqueId, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.PECIncoming.toString()) {
            this.loadIncomingPECMailData(this.referenceUniqueId, `'Incoming'`, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.PECOutgoing.toString()) {
            this.loadOutgoingPECMailData(this.referenceUniqueId, `'Outgoing'`, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.PECUnified.toString()) {
            this.loadPECMailData(this.referenceUniqueId, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.Archive.toString()) {
            this.loadUDSData(this.referenceUniqueId, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.ArchiveProtocol.toString()) {
            this.loadUDSProtocolData(this.referenceUniqueId, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.Protocol.toString()) {
            this.loadProtocolData(this.referenceUniqueId, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.ProtocolLinks.toString()) {
            this.loadProtocolLinkData(this.referenceUniqueId, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.ProtocolSeries.toString()) {
            this.loadDocumentSeriesProtocolsLinksData(this.referenceUniqueId, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.MessageResolution.toString()) {
            this.loadResolutionMessageData(this.referenceUniqueId, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.MessageProtocol.toString()) {
            this.loadMessageData(this.referenceUniqueId, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.MessageSeries.toString()) {
            this.loadDocumentSeriesMessageData(this.referenceUniqueId, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.Series.toString() && this.showResolutionDocumentSeriesLinks.toLowerCase() == "true") {
            this.loadResolutionDocumentSerieData(this.referenceUniqueId, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.SeriesProtocol.toString() && this.showResolutionDocumentSeriesLinks.toLowerCase() == "false") {
            this.loadDocumentSerieData(this.referenceUniqueId, loadIndex);
        }

        if (nodeTypeAttribute == DocumentUnitReferenceTypeEnum.TNotice.toString()) {
            this.loadTNoticeData(this.referenceUniqueId, loadIndex);
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


            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
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

        const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
        parentNode.get_nodes().add(node);

        this._DocumentRadTreeId.commitChanges();

    }

    private loadIncomingPECMailData(uniqueId: string, pecMailDirection: string, position: number): void {
        let node: Telerik.Web.UI.RadTreeNode = this._DocumentRadTreeId.get_nodes().getNode(position);
        let totalChildrenCount: number = node.get_attributes().getAttribute(uscDocumentUnitReferences.TOTAL_CHILDREN_COUNT_ATTRNAME);
        let skip: number = (!this.pecMailIncomings || this.pecMailIncomings.length == 0) ? 0 : this.pecMailIncomings.length;
        if (!this.pecMailIncomings || this.pecMailIncomings.length == 0 || this.pecMailIncomings.length != totalChildrenCount) {
            this._pecMailService.getIncomingPECMail(uniqueId, pecMailDirection, (data) => {
                if (!data) return;
                if (!this.pecMailIncomings || this.pecMailIncomings.length == 0) {
                    this.pecMailIncomings = data;
                }
                else {
                    this.pecMailIncomings.push(...data)
                }
                this.renderIncomingPECMailNodes(this.pecMailIncomings, position, true);
                this._appendLoadMoreNode(node);
            }, null, true, new PaginationModel(skip, this._defaultPaginationModel.Take));
        } else {
            this.renderIncomingPECMailNodes(this.pecMailIncomings, position, true);
            this._appendLoadMoreNode(node);
        }
    }

    private loadOutgoingPECMailData(uniqueId: string, direction: string, position: number): void {
        let node: Telerik.Web.UI.RadTreeNode = this._DocumentRadTreeId.get_nodes().getNode(position);
        let totalChildrenCount: number = node.get_attributes().getAttribute(uscDocumentUnitReferences.TOTAL_CHILDREN_COUNT_ATTRNAME);
        let skip: number = (!this.pecMailOutgoings || this.pecMailOutgoings.length == 0) ? 0 : this.pecMailOutgoings.length;
        if (!this.pecMailOutgoings || this.pecMailOutgoings.length == 0 || this.pecMailOutgoings.length != totalChildrenCount) {
            this._pecMailService.getOutgoingPECMail(uniqueId, direction, (data) => {
                if (!data) return;
                if (!this.pecMailOutgoings || this.pecMailOutgoings.length == 0) {
                    this.pecMailOutgoings = data;
                }
                else {
                    this.pecMailOutgoings.push(...data)
                }
                this.renderOutgoingPECMailNodes(this.pecMailOutgoings, position, true);
                this._appendLoadMoreNode(node);
            }, null, true, new PaginationModel(skip, this._defaultPaginationModel.Take));
        } else {
            this.renderOutgoingPECMailNodes(this.pecMailOutgoings, position, true);
            this._appendLoadMoreNode(node);
        }
    }

    private loadPECMailData(uniqueId: string, position: number): void {
        let node: Telerik.Web.UI.RadTreeNode = this._DocumentRadTreeId.get_nodes().getNode(position);
        let totalChildrenCount: number = node.get_attributes().getAttribute(uscDocumentUnitReferences.TOTAL_CHILDREN_COUNT_ATTRNAME);
        let skip: number = (!this.pecMails || this.pecMails.length == 0) ? 0 : this.pecMails.length;
        if (!this.pecMails || this.pecMails.length == 0 || this.pecMails.length != totalChildrenCount) {
            this._pecMailService.getPECMailsByDocumentUnit(uniqueId, (data) => {
                if (!data) return;
                if (!this.pecMails || this.pecMails.length == 0) {
                    this.pecMails = data;
                }
                else {
                    this.pecMails.push(...data)
                }
                this.pecMailOutgoings = this.pecMails.filter(function (item) {
                    return item.Direction == PECMailDirection.Outgoing;
                });
                this.pecMailIncomings = this.pecMails.filter(function (item) {
                    return item.Direction == PECMailDirection.Incoming;
                });
                this.renderIncomingPECMailNodes(this.pecMailIncomings, position, true);
                this.renderOutgoingPECMailNodes(this.pecMailOutgoings, position, false);
                this._appendLoadMoreNode(node);
            }, null, true, new PaginationModel(skip, this._defaultPaginationModel.Take));
        } else {
            this.renderIncomingPECMailNodes(this.pecMailIncomings, position, true);
            this.renderOutgoingPECMailNodes(this.pecMailOutgoings, position, false);
            this._appendLoadMoreNode(node);
        }
    }

    private loadMorePECMailData(uniqueId: string, position: number): void {
        let node: Telerik.Web.UI.RadTreeNode = this._DocumentRadTreeId.get_nodes().getNode(position);
        let nodeType: string = node.get_attributes().getAttribute(uscDocumentUnitReferences.Attribute_NodeType);
        if (nodeType == DocumentUnitReferenceTypeEnum.PECUnified.toString()) {
            this.loadPECMailData(uniqueId, position);
        }
        if (nodeType == DocumentUnitReferenceTypeEnum.PECIncoming.toString()) {
            this.loadIncomingPECMailData(this.referenceUniqueId, `'Incoming'`, position);
        }
        if (nodeType == DocumentUnitReferenceTypeEnum.PECOutgoing.toString()) {
            this.loadOutgoingPECMailData(this.referenceUniqueId, `'Outgoing'`, position);
        }
    }

    private loadFascicleData(uniqueId: string, position: number): void {
        if (!this.fascicleDocumentUnitList || this.fascicleDocumentUnitList.length == 0) {
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
        if (!this.fascicleLinkList || this.fascicleLinkList.length == 0) {
            this._fascicleLinksService.getLinkedFasciclesById(uniqueId, (data) => {
                if (!data) return;
                this.fascicleLinkList = data;
                this.renderFasciclesNodes(this.fascicleLinkList, position);
            });
        } else {
            this.renderFasciclesNodes(this.fascicleLinkList, position);
        }
    }

    private loadFasciclesFromDocumentUnitData(uniqueId: string, position: number): void {
        if (!this.documentUnitFascicleLinksList || this.documentUnitFascicleLinksList.length == 0) {
            this._fascicleService.getFasciclesFromDocumentUnit(this.referenceUniqueId, (data) => {
                if (!data) { return; }
                this.documentUnitFascicleLinksList = data;
                this.renderViewableFascicleNodes(this.documentUnitFascicleLinksList, position);
            }, (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
            });
        } else {
            this.renderViewableFascicleNodes(this.documentUnitFascicleLinksList, position);
        }
    }

    private loadDeletedFascicleDocumentUnitData(uniqueId: string, position: number): void {
        if (!this.deletedFascicleDocumentUnitList || this.deletedFascicleDocumentUnitList.length == 0) {
            this._fascicleLogService.getDeletedFasciclesByFascicleId(uniqueId, FascicleLogType.UDDelete, (data) => {
                if (!data) return;
                this.deletedFascicleDocumentUnitList = data;
                this.renderDeletedFascicleDocumentUnitNodes(this.deletedFascicleDocumentUnitList, position);
            });
        } else {
            this.renderDeletedFascicleDocumentUnitNodes(this.deletedFascicleDocumentUnitList, position);
        }
    }

    private loadDeletedFascicleDocumentData(uniqueId: string, position: number): void {
        if (!this.deletedFascicleDocumentList || this.deletedFascicleDocumentList.length == 0) {
            this._fascicleLogService.getDeletedFasciclesByFascicleId(uniqueId, FascicleLogType.DocumentDelete, (data) => {
                if (!data) return;
                this.deletedFascicleDocumentList = data;
                this.renderDeletedFascicleDocumentNodes(this.deletedFascicleDocumentList, position);
            });
        } else {
            this.renderDeletedFascicleDocumentNodes(this.deletedFascicleDocumentList, position);
        }
    }

    private loadDossiersData(uniqueId: string, position: number): void {
        if (!this.dossierFolderList || this.dossierFolderList.length == 0) {
            this._dossierFolderService.getByFascicleId(uniqueId, (data) => {
                if (!data) return;
                this.dossierFolderList = data;
                this.renderDossiersNodes(this.dossierFolderList, position);
            });
        } else {
            this.renderDossiersNodes(this.dossierFolderList, position);
        }
    }

    private loadWorkflowActivitiesData(uniqueId: string, position: number): void {
        if (!this.workflowActivityList || this.workflowActivityList.length == 0) {
            this._workflowActivityService.getByReferenceDocumentUnitId(uniqueId, (data: WorkflowActivityModel[]) => {
                if (!data) return;
                this.workflowActivityList = data;
                this.renderWorkflowActivityNodes(true, true, this.workflowActivityList, position);
            });
        } else {
            this.renderWorkflowActivityNodes(true, true, this.workflowActivityList, position);
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
        if (!this.udsIdList || this.udsIdList.length == 0) {
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
        if (!this.protocolLinks || this.protocolLinks.length == 0) {
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
        if (!this.protocolDocumentList || this.protocolDocumentList.length == 0) {
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
        if (!this.udsDocumentList || this.udsDocumentList.length == 0) {
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
        if (!this.tNoticeSummaryList || this.tNoticeSummaryList.length == 0) {
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
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Messaggi (${data})`);
        });
    }

    private loadDocumentSeriesItemLinksCount(uniqueId: string, position: number): void {
        this._documentSeriesItemLinkService.countDocumentSeriesItemLinksById(uniqueId, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Atti (${data})`);
        });
    }

    private loadDocumentSeriesProtocolsLinksCount(uniqueId: string, position: number): void {
        this._protocolService.countDocumentSeriesItemProtocolById(uniqueId, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Protocolli (${data})`);
        });
    }

    private loadFascicleCount(uniqueId: string, position: number): void {
        this._fascicleService.countAuthorizedFasciclesFromDocumentUnit(uniqueId, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Fascicoli (${data})`);
        });
    }

    private loadProtocolCount(uniqueId: string, position: number): void {
        this._udsService.countProtocolsById(uniqueId, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Protocolli (${data})`);
        });
    }

    private loadProtocolLinkCount(uniqueId: string, position: number): void {
        this._protocolLinkService.countProtocolsById(uniqueId, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Protocolli (${data})`);
        });
    }

    private loadUDSCount(uniqueId: string, position: number): void {
        this._udsService.countUDSById(uniqueId, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Archivi (${data})`);
        });
    }

    private loadProtocolUDSCount(uniqueId: string, position: number): void {
        this._udsService.countUDSByRelationId(uniqueId, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Archivi (${data})`);
        });
    }

    private loadProtocolMessageCount(uniqueId: string, position: number): void {
        this._messageService.countProtocolMessagesById(uniqueId, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Messaggi (${data})`);
        });
    }

    private loadProtocolDocumentSeriesCount(uniqueId: string, position: number): void {
        this._documentSeriesItemService.countDocumentSeriesItemById(uniqueId, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            let nodeText: string = this.seriesTitle ? this.seriesTitle : "Serie documentali";
            parentNode.set_text(`${nodeText} (${data})`);
        });
    }

    private loadIncomingPECMailCount(uniqueId: string, direction: string, position: number): void {
        this._pecMailService.countIncomingPECMail(uniqueId, direction, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`PEC ingresso (${data})`);
            this._appendChildrenCountAttribute(parentNode, data);
        });        
    }

    private loadOutgoingPECMailCount(uniqueId: string, direction: string, position: number): void {
        this._pecMailService.countOutgoingPECMail(uniqueId, direction, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`PEC uscita (${data})`);
            this._appendChildrenCountAttribute(parentNode, data);
        });
    }

    private loadPECMailCount(uniqueId: string, position: number): void {
        this._pecMailService.countPECMail(uniqueId, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`PEC (${data})`);
            this._appendChildrenCountAttribute(parentNode, data);
        });
    }

    private loadResolutionMessageCount(uniqueId: string, position: number): void {
        this._messageService.countResolutionMessagesById(uniqueId, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
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

    private loadFascicleDocumentUnitCount(uniqueId: string, position: number): void {
        this._fascicleDocumentUnitService.countFascicleById(uniqueId, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Fascicoli (${data})`);
        });
    }

    private loadDeletedFascicleDocumentUnitsCount(uniqueId: string, position: number): void {
        this._fascicleLogService.countByFascicleId(uniqueId, FascicleLogType.UDDelete, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Documenti eliminati (${data})`);
        });
    }

    private loadDeletedFascicleDocumentsCount(uniqueId: string, position: number): void {
        this._fascicleLogService.countByFascicleId(uniqueId, FascicleLogType.DocumentDelete, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Inserti eliminati (${data})`);
        });
    }

    private loadDossierCount(uniqueId: string, position: number): void {
        this._dossierFolderService.countByFascicleId(uniqueId, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Dossiers (${data})`);
        });
    }

    private loadWorkflowActivitiesCount(uniqueId: string, position: number): void {
        this._workflowActivityService.countByReferenceDocumentUnitId(uniqueId, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`Registro attività (${data})`);
        });
    }

    private loadTNoticeCount(idDocumentUnit: string, position: number): void {
        this._tnoticeService.countRequestsSummariesByDocumentId(idDocumentUnit, (data) => {
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.set_text(`TNotice (${data})`);
        });
    }

    private renderTNoticeSummary(currentItems: TNoticeStatusSummaryDTO[], position: number): void {
        const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
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

            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);

            this._DocumentRadTreeId.commitChanges();
        }

    }

    private renderIncomingPECMailNodes(currentItems: PECMailViewModel[], position: number, clearNodes: boolean): void {
        if (clearNodes === true) {
            this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
        }

        for (const pecMail of currentItems) {

            let recipients: string = pecMail.MailRecipients;
            let senderAndRecipient: string = `${pecMail.MailSenders} ${recipients}`;

            let result = senderAndRecipient;
            if (pecMail.MailDate) {
                result = `${senderAndRecipient} - ${moment(pecMail.MailDate).format("DD/MM/YYYY, HH:mm:ss")}`;
            }

            const node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_text(result);
            node.set_value(pecMail.EntityId);
            node.set_cssClass("text-label");
            node.get_attributes().setAttribute(uscDocumentUnitReferences.Attribute_Direction, "Incoming");
            if (PECMailDirectionProcessStatusEnum[pecMail.ProcessStatus] === PECMailDirectionProcessStatusEnum[PECMailDirectionProcessStatusEnum.ArchivedInDocSuiteNext]) {
                node.get_attributes().setAttribute(uscDocumentUnitReferences.Attribute_OpenInDocSuiteNext, true);
                node.get_attributes().setAttribute(uscDocumentUnitReferences.Attribute_UniqueId, pecMail.UniqueId);
            }

            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/receiveMail.png");
            node.set_toolTip("Posta ricevuta");
            parentNode.get_nodes().add(node);
            node.get_imageElement().outerHTML = `<img class="rtImg" alt = "" src = "../App_Themes/DocSuite2008/imgset16/receiveMail.png" title="Posta inviata">`
            this._DocumentRadTreeId.commitChanges();
        }
    }

    private renderOutgoingPECMailNodes(currentItems: PECMailViewModel[], position: number, clearNodes: boolean): void {
        if (clearNodes === true) {
            this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
        }

        for (const pecMail of currentItems) {

            const recipients: string = pecMail.MailRecipients;
            const senderAndRecipient = `${pecMail.MailSenders} ${recipients}`;
            let result = "";
            if (pecMail.MailDate) {
                result = `${senderAndRecipient} - ${moment(pecMail.MailDate).format("DD/MM/YYYY, HH:mm:ss")}`;
            } else {
                result = senderAndRecipient;
            }

            const node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_text(result);
            node.set_value(pecMail.EntityId);
            node.set_cssClass("text-label");
            node.get_attributes().setAttribute(uscDocumentUnitReferences.Attribute_Direction, "Outgoing");

            const numberOfItems: number = pecMail.PECMailReceipts.length;
            const pecMailsReceipts: PECMailReceiptsModel[] = pecMail.PECMailReceipts;

            let nodeImages: string = `<img class="rtImg" alt = "" src = "../App_Themes/DocSuite2008/imgset16/sendEmail.png" title="Posta inviata">`;
            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/sendEmail.png");
            node.set_toolTip("Posta inviata");

            for (let i = 0; i < numberOfItems; i++) {
                const status = pecMailsReceipts[i].ReceiptType;

                if (status == "accettazione") {
                    nodeImages = `${nodeImages}<img class="rtImg" alt = "" src = "../Comm/Images/pec-accettazione.gif" title="accettazione">`;
                }

                if (status == "avvenuta-consegna") {
                    nodeImages = `${nodeImages}<img class="rtImg" alt = "" src = "../Comm/Images/pec-avvenuta-consegna.gif" title="avvenuta-consegna">`;
                }

                if (status == "non-accettazione") {
                    nodeImages = `${nodeImages}<img class="rtImg" alt = "" src = "../Comm/Images/pec-non-accettazione.gif" title="non-accettazione">`;
                }

                if (status == "preavviso-errore-consegna") {
                    nodeImages = `${nodeImages}<img class="rtImg" alt = "" src = "../Comm/Images/preavviso-errore-consegna.gif" title="preavviso-errore-consegna">`;
                }

                if (status == "errore-consegna") {
                    nodeImages = `${nodeImages}<img class="rtImg" alt = "" src = "../Comm/Images/pec-errore-consegna.gif" title="errore-consegna">`;
                }
            }

            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);
            node.get_imageElement().outerHTML = nodeImages
            this._DocumentRadTreeId.commitChanges();
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

            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);

            this._DocumentRadTreeId.commitChanges();
        }
    }

    private renderResolutionMessageNodes(currentItems: ResolutionModel, position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
        let messages: MessageModel[] = currentItems.Messages;
        let numberOfItems: number = currentItems.Messages.length;
        this.renderMessageNodes(numberOfItems, messages, position)
    }

    private renderProtocolMessageNodes(currentItems: ProtocolModel[], position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();
        let numberOfItems: number = currentItems[0].Messages.length;
        let messages: MessageModel[] = currentItems[0].Messages;
        this.renderMessageNodes(numberOfItems, messages, position)
    }

    private renderFascicleNodes(currentItems: FascicleModel[], position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        for (let fascDoc of currentItems) {
            this.addFascicleTreNode(`${fascDoc.Title} - ${fascDoc.FascicleObject}`, fascDoc, position);
        }
    }

    private renderViewableFascicleNodes(currentItems: FascicleModel[], position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        for (let fascDoc of currentItems) {
            this.addFascicleTreNode(fascDoc.Title, fascDoc, position);
        }
    }

    private addFascicleTreNode(nodeText: string, fascDoc: FascicleModel, position: number) {
        let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        node.set_text(nodeText);
        node.set_value(fascDoc.UniqueId);
        node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/fascicle_open.png");
        node.set_navigateUrl(`../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=${fascDoc.UniqueId}`);

        const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
        parentNode.get_nodes().add(node);
        this._DocumentRadTreeId.commitChanges();

    }

    private renderFasciclesNodes(currentItems: FascicleLinkModel[], position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        for (let fascicles of currentItems) {
            let fascicleName: string = `${fascicles.FascicleLinked.Title} - ${fascicles.FascicleLinked.FascicleObject}`;

            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_text(fascicleName);
            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/fascicle_open.png");
            node.set_navigateUrl(`../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=${fascicles.FascicleLinked.UniqueId}`);

            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);

            this._DocumentRadTreeId.commitChanges();
        }
    }

    private renderDeletedFascicleDocumentUnitNodes(currentItems: FascicleLogViewModel[], position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        for (const log of currentItems) {
            const logDesc = `${log.LogDate} - ${log.LogUser} - ${log.Description}`;

            const node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_text(logDesc);

            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);

            this._DocumentRadTreeId.commitChanges();
        }
    }

    private renderDeletedFascicleDocumentNodes(currentItems: FascicleLogViewModel[], position: number): void {
        this._DocumentRadTreeId.get_nodes().getNode(position).get_nodes().clear();

        for (const log of currentItems) {
            const logDesc = `${log.LogDate} - ${log.LogUser} - ${log.Description}`;

            const node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.set_text(logDesc);

            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
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
            if (includeName && includeName == true) {
                name = workflowActivity.Name
            }
            if (workflowActivity.Subject && workflowActivity.Subject !== '') {
                name = `${name} - ${workflowActivity.Subject}`
            }
            name = `${name} del ${workflowActivity.RegistrationDateFormatted}`

            const node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            //base image
            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/workflow_16x.png");

            let imageContent: string;
            switch ((WorkflowStatus[workflowActivity.Status.toString()]) as WorkflowStatus) {
                case WorkflowStatus.Todo:
                    {
                        node.set_toolTip("Attività in attesa di essere avviata");
                        imageContent = `<img class="rtImg" alt="" src="../App_Themes/DocSuite2008/imgset16/workflow_delay_16x.png" title="Attività in attesa di essere avviata" />`;
                        name = `${name} richiesta da ${workflowActivity.RegistrationUser} in attesa`;
                        break;
                    }
                case WorkflowStatus.Progress:
                    {
                        node.set_toolTip("Attività avviata");
                        imageContent = `<img class="rtImg" alt="" src="../App_Themes/DocSuite2008/imgset16/workflow_activate_16x.png" title="Attività avviata" />`;
                        name = `${name} richiesta da ${workflowActivity.RegistrationUser} avviata`;
                        break;
                    }
                case WorkflowStatus.Done:
                    {
                        node.set_toolTip("Attività completata");
                        imageContent = `<img class="rtImg" alt="" src="../App_Themes/DocSuite2008/imgset16/workflow_16x.png" title="Attività completata" />`;
                        name = `${name} richiesta da ${workflowActivity.RegistrationUser} completata`;
                        break;
                    }
                case WorkflowStatus.Error:
                    {
                        node.set_toolTip("Attività in errore");
                        imageContent = `<img class="rtImg" alt="" src="../App_Themes/DocSuite2008/imgset16/workflow_delete_16x.png" title="Attività in errore" />`;
                        name = `${name} richiesta da ${workflowActivity.RegistrationUser} in errore`;
                        break;
                    }
            }
            node.set_text(name);
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
            parentNode.get_nodes().add(node);
            node.get_imageElement().outerHTML = imageContent;
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

            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
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
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
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
            node.set_imageUrl("../Comm/Images/DocSuite/Protocollo16.png");
            node.set_navigateUrl(`../Prot/ProtVisualizza.aspx?UniqueId=${protLink.ProtocolLinked.UniqueId}&Type=Prot`);

            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position)
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
            node.set_imageUrl("../Comm/Images/DocSuite/Protocollo16.png");
            node.set_navigateUrl(`../Prot/ProtVisualizza.aspx?UniqueId=${protDoc.Relation.UniqueId}&Type=Prot`);

            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
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
            const parentNode = this._DocumentRadTreeId.get_nodes().getNode(position);
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

    private _appendChildrenCountAttribute(node: Telerik.Web.UI.RadTreeNode, totalChildrenCount: number): void {
        let currentChildrenAttributeValue: number | undefined = node.get_attributes().getAttribute(uscDocumentUnitReferences.TOTAL_CHILDREN_COUNT_ATTRNAME);
        if (currentChildrenAttributeValue === undefined) {
            node.get_attributes().setAttribute(uscDocumentUnitReferences.TOTAL_CHILDREN_COUNT_ATTRNAME, totalChildrenCount);
        }
    }

    private _appendLoadMoreNode(expandedNode: Telerik.Web.UI.RadTreeNode) {
        let expandedNodeType: string = expandedNode.get_attributes().getAttribute(uscDocumentUnitReferences.Attribute_NodeType);
        let totalChildrenCount: number = expandedNode.get_attributes().getAttribute(uscDocumentUnitReferences.TOTAL_CHILDREN_COUNT_ATTRNAME);

        if (expandedNodeType === uscDocumentUnitReferences.LOAD_MORE_NODE_TYPE) {
            let expandedNodeParentChildrenCollection: Telerik.Web.UI.RadTreeNodeCollection = expandedNode.get_parent().get_nodes();
            let allChildrenLoaded: boolean = (expandedNodeParentChildrenCollection.get_count() - 1) === totalChildrenCount;

            expandedNodeParentChildrenCollection.remove(expandedNode);
            if (!allChildrenLoaded) {
                expandedNodeParentChildrenCollection.add(this._createLoadMoreNode());
            }
        }
        else {
            let expandedNodeChildrenCollection: Telerik.Web.UI.RadTreeNodeCollection = expandedNode.get_nodes();
            let loadedAllChildren: boolean = expandedNodeChildrenCollection.get_count() === totalChildrenCount; 
            if (loadedAllChildren) {
                return;
            }
            expandedNodeChildrenCollection.add(this._createLoadMoreNode());
        }
    }

    private _createLoadMoreNode(): Telerik.Web.UI.RadTreeNode {
        let loadMoreNode: Telerik.Web.UI.RadTreeNode = this._createTreeNode(uscDocumentUnitReferences.LOAD_MORE_NODE_TYPE, uscDocumentUnitReferences.LOAD_MORE_NODE_LABEL, null, uscDocumentUnitReferences.LOAD_MORE_NODE_IMAGEURL);
        return loadMoreNode;
    }

    /**
     * Creates a RadTreeNode object with the given details. 
     * If the parent node is passed, the new created node is added to the parent node collection.
     * @param nodeType
     * @param nodeDescription
     * @param nodeValue
     * @param imageUrl
     * @param parentNode
     * @param tooltipText
     * @param expandedImageUrl
     */
    private _createTreeNode(nodeType: string, nodeDescription: string, nodeValue: number | string, imageUrl: string, parentNode?: Telerik.Web.UI.RadTreeNode, tooltipText?: string, expandedImageUrl?: string): Telerik.Web.UI.RadTreeNode {
        let treeNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        treeNode.set_text(nodeDescription);
        treeNode.set_value(nodeValue);
        treeNode.get_attributes().setAttribute(uscDocumentUnitReferences.Attribute_NodeType, nodeType);

        if (imageUrl) {
            treeNode.set_imageUrl(imageUrl);
        }

        if (tooltipText) {
            treeNode.set_toolTip(tooltipText);
        }

        if (expandedImageUrl) {
            treeNode.set_expandedImageUrl(expandedImageUrl);
        }

        if (parentNode) {
            parentNode.get_nodes().add(treeNode);
        }

        return treeNode;
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

    updateDocumentDeleteNodeName(uniqueId: string) {
        let positionDocumentiEliminati: number = this._DocumentRadTreeId.get_allNodes().filter(x => x.get_text().indexOf(`Documenti eliminati`) !== -1)[0].get_index();
        this.loadDeletedFascicleDocumentUnitsCount(uniqueId, positionDocumentiEliminati);

        let positionInsertiEliminati: number = this._DocumentRadTreeId.get_allNodes().filter(x => x.get_text().indexOf(`Inserti eliminati`) !== -1)[0].get_index();
        this.loadDeletedFascicleDocumentsCount(uniqueId, positionInsertiEliminati);
    }

}
export = uscDocumentUnitReferences;