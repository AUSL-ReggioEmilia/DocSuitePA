/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import ContactModel = require('App/Models/Commons/ContactModel');
import FascicleType = require('App/Models/Fascicles/FascicleType');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import FascicleBase = require('Fasc/FascBase');
import UscFascicolo = require('UserControl/uscFascicolo');
import FascicleDocumentModel = require('App/Models/Fascicles/FascicleDocumentModel');
import ChainType = require('App/Models/DocumentUnits/ChainType');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import AjaxModel = require('App/Models/AjaxModel');
import ProcessService = require('App/Services/Processes/ProcessService');
import ProcessModel = require('App/Models/Processes/ProcessModel');
import DossierFolderModel = require('App/Models/Dossiers/DossierFolderModel');
import DossierFolderService = require('App/Services/Dossiers/DossierFolderService');
import DossierFolderModelMapper = require('App/Mappers/Dossiers/DossierFolderModelMapper');
import UpdateActionType = require('App/Models/UpdateActionType');
import uscSetiContactSel = require('UserControl/uscSetiContactSel');
import MetadataDesignerViewModel = require('App/ViewModels/Metadata/MetadataDesignerViewModel');
import SetiContactModel = require('App/Models/Commons/SetiContactModel');
import UscMetadataRepositorySel = require('UserControl/uscMetadataRepositorySel');
import Environment = require('App/Models/Environment');
import RoleService = require('App/Services/Commons/RoleService');
import PageClassHelper = require('App/Helpers/PageClassHelper');
import uscContattiSelRest = require('UserControl/uscContattiSelRest');
import UscDynamicMetadataRest = require('UserControl/uscDynamicMetadataRest');

declare var Page_IsValid: any;
class FascModifica extends FascicleBase {
    currentFascicleId: string;
    txtNameId: string;
    txtObjectId: string;
    txtRackId: string;
    rowNameId: string;
    rowRackId: string;
    rowLegacyManagerId: string;
    txtManagerId: string;
    txtNoteId: string;
    ajaxManagerId: string;
    ajaxLoadingPanelId: string;
    btnConfermaId: string;
    pageContentId: string;
    radWindowManagerId: string;
    uscNotificationId: string;
    uscFascicoloId: string;
    rowManagerId: string;
    isEditPage: boolean;
    fasciclesPanelVisibilities: { [key: string]: boolean };
    rowDynamicMetadataId: string;
    metadataRepositoryEnabled: boolean;
    processEnabled: boolean;
    processPanelId: string;
    ddlProcessId: string;
    rtvProcessFoldersId: string;
    uscSetiContactId: string;
    setiContactEnabledId: boolean;
    rowTransformIntoProcessFascicleId: string;
    chkTransformIntoProcessFascicleId: string;
    uscContactId: string;
    uscContactDivId: string;
    uscDynamicMetadataId: string;

    private _txtName: Telerik.Web.UI.RadTextBox;
    private _txtRack: Telerik.Web.UI.RadTextBox;
    private _txtNote: Telerik.Web.UI.RadTextBox;
    private _txtManager: Telerik.Web.UI.RadTextBox;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _txtObject: Telerik.Web.UI.RadTextBox;
    private _ddlProcess: Telerik.Web.UI.RadComboBox;
    private _rtvProcessFolders: Telerik.Web.UI.RadTreeView;
    private _rowName: JQuery;
    private _rowRacks: JQuery;
    private _rowDynamicMetadata: JQuery;
    private _processPanel: JQuery;
    private _fascicleModel: FascicleModel;
    private _uscSetiContact: uscSetiContactSel;

    private _dossierFolderService: DossierFolderService;
    private _processService: ProcessService;
    private _roleService: RoleService;

    private _serviceConfigurations: ServiceConfiguration[];
    private _selectedProcessDossierFolders: DossierFolderModel[];
    private _selectedDossierFolderId: string;
    private fascicleContacts: ContactModel[];

    private _rtvProcessFoldersRootNode(): Telerik.Web.UI.RadTreeNode {
        return this._rtvProcessFolders.get_nodes().getNode(0);
    }

    private static PROCESS_TYPE_NAME = "Process";
    private static DOSSIER_FOLDER_TYPE_NAME = "DossierFolder";
    private static ROLE_FOLDER_TYPE = "Role";
    private static DOSSIERFOLDER_IMGURL = "../App_Themes/DocSuite2008/imgset16/folder_closed.png";
    private static DOSSIERFOLDER_EXPANDED_IMGURL = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
    private static PROCESS_IMGURL = "../App_Themes/DocSuite2008/imgset16/process.png";

    /**
     * Costruttore
     * @param serviceConfigurations
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME));
        this._selectedProcessDossierFolders = [];
        this._serviceConfigurations = serviceConfigurations;
    }

    /**
     * Initialize
     */
    initialize(): void {
        super.initialize();

        let processConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascModifica.PROCESS_TYPE_NAME);
        this._processService = new ProcessService(processConfiguration);

        let dossierFolderConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascModifica.DOSSIER_FOLDER_TYPE_NAME);
        this._dossierFolderService = new DossierFolderService(dossierFolderConfiguration);

        let roleConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascModifica.ROLE_FOLDER_TYPE);
        this._roleService = new RoleService(roleConfiguration);

        this._txtName = <Telerik.Web.UI.RadTextBox>$find(this.txtNameId);
        this._txtRack = <Telerik.Web.UI.RadTextBox>$find(this.txtRackId);
        this._txtNote = <Telerik.Web.UI.RadTextBox>$find(this.txtNoteId);
        this._txtManager = <Telerik.Web.UI.RadTextBox>$find(this.txtManagerId);
        this._txtObject = <Telerik.Web.UI.RadTextBox>$find(this.txtObjectId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfermaId);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        this._ddlProcess = <Telerik.Web.UI.RadComboBox>$find(this.ddlProcessId);
        this._ddlProcess.add_selectedIndexChanged(this.ddlProcess_OnClientSelectedIndexChanged);
        this._rtvProcessFolders = <Telerik.Web.UI.RadTreeView>$find(this.rtvProcessFoldersId);
        this._rtvProcessFolders.add_nodeClicked(this.rtvProcessFolders_OnNodeClicked);
        this._rowName = $("#".concat(this.rowNameId));
        this._rowRacks = $("#".concat(this.rowRackId));
        this._rowDynamicMetadata = $("#".concat(this.rowDynamicMetadataId));
        this._btnConfirm.add_clicking(this.btnConferma_OnClick);
        this._btnConfirm.set_enabled(false);
        this._rowDynamicMetadata.hide();
        this._uscSetiContact = <uscSetiContactSel>$(`#${this.uscSetiContactId}`).data();
        $(`#${this.chkTransformIntoProcessFascicleId}`).on("click", () => {
            $(`#${this.uscContactDivId}`).toggle();
        });

        this._processPanel = $(`#${this.processPanelId}`);
        this._processPanel.hide();

        this.initializeFascicle();
        this.initializeDossierFoldersTree();

        $("#".concat(this.uscDynamicMetadataId)).on(UscMetadataRepositorySel.SELECTED_SETI_CONTACT_EVENT, (sender, args: SetiContactModel) => {
            let uscDynamicMetadataRest: UscDynamicMetadataRest = <UscDynamicMetadataRest>$("#".concat(this.uscDynamicMetadataId)).data();
            uscDynamicMetadataRest.populateMetadataRepository(args, this._fascicleModel.MetadataDesigner);
        });

        this.fascicleContacts = [];
        this.registerUscContactRestEventHandlers();
    }

    /**
     *------------------------- Events -----------------------------
     */

    /**
     * Evento scatenato al click del pulsante di inserimento
     * @param sender
     * @param args
     */
    btnConferma_OnClick = (sender: any, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        if (!Page_IsValid) {
            return;
        }
        this._loadingPanel.show(this.pageContentId);
        this._btnConfirm.set_enabled(false);

        if (this.isPageValid()) {
            let insertsArchiveChain: string = this.getInsertsArchiveChain();
            let ajaxModel: AjaxModel = <AjaxModel>{};
            ajaxModel.Value = new Array<string>();
            ajaxModel.ActionName = "Update";
            ajaxModel.Value = new Array<string>();
            ajaxModel.Value.push(insertsArchiveChain);
            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
            return;
        }

        this._loadingPanel.hide(this.pageContentId);
        this._btnConfirm.set_enabled(true);
    }

    rtvProcessFolders_OnNodeClicked = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs): void => {
        this._selectedDossierFolderId = args.get_node().get_value();
    }

    ddlProcess_OnClientSelectedIndexChanged = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        let selectedProcess: Telerik.Web.UI.RadComboBoxItem = args.get_item();

        if (!selectedProcess || !selectedProcess.get_value()) {
            this._rtvProcessFoldersRootNode().get_nodes().clear();
            return;
        }

        this.loadProcessDossierFolders(this._ddlProcess.get_value());
    }

    /**
     *------------------------- Methods -----------------------------
     */

    private selectDefaultDossierFolder(defaultNode: Telerik.Web.UI.RadTreeNode): void {
        let currentNodeParent: Telerik.Web.UI.RadTreeNode = defaultNode.get_parent();
        currentNodeParent.set_expanded(true);

        if (!!currentNodeParent.get_value()) {
            this.selectDefaultDossierFolder(currentNodeParent);
        }
    }

    private loadProcessDossierFolders(processId: string, defaultDossierFolderId?: string): void {
        if (this._rtvProcessFoldersRootNode().get_nodes().get_count() > 0) {
            this._rtvProcessFoldersRootNode().get_nodes().clear();
        }

        this._rtvProcessFoldersRootNode().showLoadingStatus('', Telerik.Web.UI.TreeViewLoadingStatusPosition.BeforeNodeText);
        this._dossierFolderService.getProcessFolders(null, processId, false, true,
            (dossierFolders: DossierFolderModel[]) => {
                let dossierFolderModelMapper: DossierFolderModelMapper = new DossierFolderModelMapper();
                this._selectedProcessDossierFolders = dossierFolderModelMapper.MapCollection(dossierFolders);
                this.populateDossierFolderTreeRecursive(this._selectedProcessDossierFolders, this._rtvProcessFoldersRootNode());

                if (defaultDossierFolderId) {
                    let defaultNode: Telerik.Web.UI.RadTreeNode = this._rtvProcessFoldersRootNode().get_allNodes().filter(node => node.get_value() === defaultDossierFolderId)[0];

                    if (defaultNode) {
                        defaultNode.set_selected(true);
                        this.selectDefaultDossierFolder(defaultNode);
                    }
                }
                this._rtvProcessFoldersRootNode().hideLoadingStatus();
            },
            (exception: ExceptionDTO) => {
                this._rtvProcessFoldersRootNode().hideLoadingStatus();
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    private loadProcesses(defaultProcessId?: string) {
        this._loadingPanel.show(this.ddlProcessId);
        let fascicleCategoryId: number = this._fascicleModel.Category.EntityShortId;
        this._processService.getAvailableProcesses(null, true, fascicleCategoryId, null,
            (processes: ProcessModel[]) => {
                let today = new Date();
                processes = processes.filter(x => new Date(x.StartDate) < today && (x.EndDate === null || new Date(x.EndDate) > today));

                let processesCbItems: Telerik.Web.UI.RadComboBoxItem[]
                    = processes.map((process: ProcessModel) => this._createComboboxItem(process.Name, process.UniqueId, FascModifica.PROCESS_IMGURL));
                let defaultSelectedItem: Telerik.Web.UI.RadComboBoxItem
                    = processes.length === 1 ? processesCbItems[0] : (!defaultProcessId ? undefined : processesCbItems.filter(item => item.get_value() === defaultProcessId)[0]);
                this.populateCombobox(processesCbItems, this._ddlProcess, defaultSelectedItem);

                this._loadingPanel.hide(this.ddlProcessId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.ddlProcessId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    private initializeDossierFoldersTree(): void {
        let rootNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
        rootNode.set_text("Tutti i volumi");
        rootNode.set_expanded(true);
        rootNode.set_checkable(false);
        rootNode.disable();
        this._rtvProcessFolders.get_nodes().add(rootNode);
    }

    private populateDossierFolderTreeRecursive(dossierFolders: DossierFolderModel[], parentNode: Telerik.Web.UI.RadTreeNode): void {
        dossierFolders.forEach((dossierFolder: DossierFolderModel) => {
            if (dossierFolder.Status.toString() === "Fascicle") {
                return;
            }

            let currentNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();

            currentNode.set_text(dossierFolder.Name);
            currentNode.set_value(dossierFolder.UniqueId);
            currentNode.set_imageUrl(FascModifica.DOSSIERFOLDER_IMGURL);
            currentNode.set_expandedImageUrl(FascModifica.DOSSIERFOLDER_EXPANDED_IMGURL);

            if (parentNode) {
                parentNode.get_nodes().add(currentNode);
            } else {
                this._rtvProcessFolders.get_nodes().add(currentNode);
            }

            if (dossierFolder.DossierFolders.length) {
                this.populateDossierFolderTreeRecursive(dossierFolder.DossierFolders, currentNode);
            }
        });
    }

    private initializeFascicle(): void {
        this._loadingPanel.show(this.pageContentId);
        this.service.getFascicle(this.currentFascicleId,
            (data: any) => {
                if (data == null) return;

                this._fascicleModel = data;

                this.checkFascicleRight(this.currentFascicleId)
                    .done((isEditable: boolean) => {
                        if (!isEditable) {
                            this._loadingPanel.hide(this.pageContentId);
                            this.showNotificationMessage(this.uscNotificationId, `Fascicolo n. ${this._fascicleModel.Title}. Mancano diritti di modifica.`);
                            $("#".concat(this.pageContentId)).hide();
                            return;
                        }
                        this.bindPageFromModel(this._fascicleModel);

                        if (this.metadataRepositoryEnabled) {
                            this.loadMetadata(this._fascicleModel.MetadataDesigner, this._fascicleModel.MetadataValues);
                        }

                        if (this._fascicleModel && this._fascicleModel.MetadataDesigner) {
                            let metadata: MetadataDesignerViewModel = JSON.parse(this._fascicleModel.MetadataDesigner);
                            if (metadata && metadata.SETIFieldEnabled) {
                                $("#".concat(this.uscSetiContactId)).triggerHandler(uscSetiContactSel.SHOW_SETI_CONTACT_BUTTON, metadata.SETIFieldEnabled && this.setiContactEnabledId);
                            }
                        }

                        let jsonFascicle: string = JSON.stringify(this._fascicleModel);
                        let ajaxModel: AjaxModel = <AjaxModel>{};
                        ajaxModel.Value = new Array<string>();
                        ajaxModel.ActionName = "Initialize";
                        ajaxModel.Value = new Array<string>();
                        ajaxModel.Value.push(jsonFascicle);

                        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
                    })
                    .fail((exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.pageContentId);
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    private initializeProcessPanel(): void {
        let fascicleDossierFolder: DossierFolderModel = this._fascicleModel.DossierFolders[0];

        if (!fascicleDossierFolder) {
            this.loadProcesses();
            return;
        }

        this._processService.getProcessByDossierFolderId(this._fascicleModel.DossierFolders[0].UniqueId, (process: ProcessModel) => {
            if (process) {
                this.loadProcesses(process.UniqueId);
                this.loadProcessDossierFolders(process.UniqueId, this._fascicleModel.DossierFolders[0].UniqueId);
            } else {
                this.loadProcesses();
            }
        }, (exception: ExceptionDTO) => {
            this.showNotificationException(this.uscNotificationId, exception);
        });
    }

    private populateCombobox(comboboxItems: Telerik.Web.UI.RadComboBoxItem[], combobox: Telerik.Web.UI.RadComboBox, defaultSelectedItem?: Telerik.Web.UI.RadComboBoxItem): void {
        comboboxItems.forEach((cbItem: Telerik.Web.UI.RadComboBoxItem) => {
            combobox.get_items().add(cbItem);
        });

        if (comboboxItems.length !== 1) {
            let emptyComboboxItem = new Telerik.Web.UI.RadComboBoxItem();
            emptyComboboxItem.set_text("");
            emptyComboboxItem.set_value("");
            combobox.get_items().insert(0, emptyComboboxItem);
        }

        if (defaultSelectedItem) {
            combobox.set_selectedItem(defaultSelectedItem);
            combobox.set_text(defaultSelectedItem.get_text());
            combobox.set_value(defaultSelectedItem.get_value());
        }
    }

    private checkFascicleRight(idFascicle: string): JQueryPromise<boolean> {
        let promise: JQueryDeferred<boolean> = $.Deferred<boolean>();
        this.service.hasManageableRight(idFascicle,
            (data: any) => promise.resolve(!!data),
            (exception: ExceptionDTO) => promise.reject(exception));
        return promise.promise();
    }

    private bindPageFromModel(fascicle: FascicleModel): void {
        this._txtObject.set_value(fascicle.FascicleObject);
        this._txtNote.set_value(fascicle.Note);
        this._txtManager.set_value(fascicle.Manager);
        if (!this.isEditPage && !(this.fasciclesPanelVisibilities["FascicleNameVisibility"])) {
            $(`#${this.rowNameId}`).hide();
        }

        if (this._txtName) {
            this._txtName.set_value(fascicle.Name);
        }

        if (!this.isEditPage && !(this.fasciclesPanelVisibilities["FascicleRacksVisibility"])) {
            $(`#${this.rowRackId}`).hide();
        }
        this._txtRack.set_value(fascicle.Rack);


        if (fascicle.FascicleType != (<any>FascicleType)[FascicleType.Legacy]) {
            $(`#${this.rowLegacyManagerId}`).remove();
        }

        if (fascicle.FascicleType == (<any>FascicleType)[FascicleType.Activity]) {
            $(`#${this.rowManagerId}`).hide();
        }

        if (this.processEnabled) {
            this._processPanel.show();
            this.initializeProcessPanel();
        }

        this.setTransformIntoProcessFascicleVisibility(fascicle);
    }

    /**
     * Inizializza lo user control del sommario di fascicolo
     */
    private loadFascicoloSummary(): void {
        PageClassHelper.callUserControlFunctionSafe<UscFascicolo>(this.uscFascicoloId)
            .done((instance) => {
                $("#".concat(this.uscFascicoloId)).bind(UscFascicolo.DATA_LOADED_EVENT, (args) => {
                    this._loadingPanel.hide(this.pageContentId);
                    this._btnConfirm.set_enabled(true);
                });
                instance.loadDataWithoutFolders(this._fascicleModel);
            });
    }

    private _createComboboxItem(itemText: string, itemValue: string, itemImgUrl?: string): Telerik.Web.UI.RadComboBoxItem {
        let cmbItem = new Telerik.Web.UI.RadComboBoxItem();
        cmbItem.set_text(itemText);
        cmbItem.set_value(itemValue);

        if (itemImgUrl) {
            cmbItem.set_imageUrl(itemImgUrl);
        }

        return cmbItem;
    }

    /**
     * Callback inizializzazione pagina
     */
    initializeCallback(): void {
        this.loadFascicoloSummary();
    }

    /**
     * Metodo per la verifica della validità della pagina
     */
    isPageValid(): boolean {
        let txtObject: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>$find(this.txtObjectId);
        if (txtObject.get_maxLength() != 0 && txtObject.get_textBoxValue().length > txtObject.get_maxLength()) {
            this.showNotificationMessage(this.uscNotificationId, "Impossibile salvare.\nIl campo Oggetto ha superato i caratteri disponibili.\n(Caratteri ".concat(txtObject.get_textBoxValue().length.toString(), " Disponibili ", txtObject.get_maxLength().toString()));
            return false;
        }

        return true;
    }

    /**
     * Callback di modifica fascicolo
     * @param contact
     */
    updateCallback(contact: number): void {
        if (this._fascicleModel == null) {
            this._loadingPanel.hide(this.pageContentId);
            this._btnConfirm.set_enabled(true);
            this.showWarningMessage(this.uscNotificationId, "Nessun fascicolo definito per la modifica");
            return;
        }

        let txtObject: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>$find(this.txtObjectId);
        if (this._txtName) {
            this._fascicleModel.Name = this._txtName.get_value();
        }
        this._fascicleModel.Rack = this._txtRack.get_value();
        this._fascicleModel.Note = this._txtNote.get_value();
        this._fascicleModel.FascicleObject = txtObject.get_value();

        if (this._fascicleModel.FascicleType == FascicleType.Legacy) {
            this._fascicleModel.Manager = this._txtManager.get_value();
        }

        if (this._selectedDossierFolderId) {
            let processDossierFolder: DossierFolderModel = {} as DossierFolderModel;
            processDossierFolder.UniqueId = this._selectedDossierFolderId;
            this._fascicleModel.DossierFolders = [processDossierFolder];
        }

        if (this._fascicleModel.FascicleType != FascicleType.Activity && contact != null && contact != 0) {
            let contactModel: ContactModel = <ContactModel>{};
            contactModel.EntityId = contact;
            this._fascicleModel.Contacts.splice(0, this._fascicleModel.Contacts.length);
            this._fascicleModel.Contacts.push(contactModel);
        }

        if (this.metadataRepositoryEnabled) {
            let uscDynamicMetadataRest: UscDynamicMetadataRest = <UscDynamicMetadataRest>$("#".concat(this.uscDynamicMetadataId)).data();
            if (!jQuery.isEmptyObject(uscDynamicMetadataRest)) {
                let metadata: MetadataDesignerViewModel = JSON.parse(this._fascicleModel.MetadataDesigner);
                if (metadata) {
                    let setiIntegrationField = metadata.SETIFieldEnabled;
                    let result = uscDynamicMetadataRest.bindModelFormPage(setiIntegrationField);
                    if (!result) {
                        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfermaId);
                        this._btnConfirm.set_enabled(true);
                        return;
                    }
                    this._fascicleModel.MetadataDesigner = result[0];
                    this._fascicleModel.MetadataValues = result[1];
                }
            }
        }

        let actionType: UpdateActionType = UpdateActionType.AssociatedProcessDossierFolderToFascicle;
        if ($(`#${this.chkTransformIntoProcessFascicleId}`).is(":checked")) {
            if (!this.fascicleContacts || this.fascicleContacts.length === 0) {
                this.showWarningMessage(this.uscNotificationId, "Responsabile di procedimento non può essere vuoto.");
                this._loadingPanel.hide(this.pageContentId);
                this._btnConfirm.set_enabled(true);
                return;
            }
            this._fascicleModel.Contacts = this.fascicleContacts;
            actionType = UpdateActionType.ChangeFascicleType;
        }

        this.service.updateFascicle(this._fascicleModel, actionType,
            (data: any) => {
                window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(this._fascicleModel.UniqueId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this._btnConfirm.set_enabled(true);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    /**
     * Recupera il record relativo agli Inserti in FascicleDocuments
     */
    getInsertsArchiveChain(): string {
        let insertsArchiveChain: string = ""
        let inserts: FascicleDocumentModel = $.grep(this._fascicleModel.FascicleDocuments, (x) => ChainType[x.ChainType.toString()] == ChainType.Miscellanea)[0];
        if (inserts != undefined) {
            insertsArchiveChain = inserts.IdArchiveChain;
        }
        return insertsArchiveChain;
    }

    private loadMetadata(metadatas: string, metadataValues: string) {
        if (metadatas) {
            this._rowDynamicMetadata.show();
            let uscDynamicMetadataRest: UscDynamicMetadataRest = <UscDynamicMetadataRest>$("#".concat(this.uscDynamicMetadataId)).data();
            if (!jQuery.isEmptyObject(uscDynamicMetadataRest)) {
                uscDynamicMetadataRest.loadPageItems(metadatas, metadataValues);
            }
        }
    }

    private setTransformIntoProcessFascicleVisibility(fascicle: FascicleModel): void {
        $(`#${this.rowTransformIntoProcessFascicleId}`).hide();
        $(`#${this.uscContactDivId}`).hide();
        this._roleService.hasCategoryFascicleRole(fascicle.Category.EntityShortId, (userBelongsToCategoryRole: boolean) => {

            let isQualifiedForTransforming: boolean = fascicle.Category.CategoryFascicles
                .some(x => x.Environment === Environment.Any);

            if (fascicle.FascicleType.toString() === FascicleType[FascicleType.Activity] && isQualifiedForTransforming && userBelongsToCategoryRole) {
                $(`#${this.rowTransformIntoProcessFascicleId}`).show();
            }

        }, (exception: ExceptionDTO) => {
            console.log(exception);
        });
    }

    private registerUscContactRestEventHandlers(): void {
        PageClassHelper.callUserControlFunctionSafe<uscContattiSelRest>(this.uscContactId)
            .done((instance) => {
                instance.registerEventHandler(instance.uscContattiSelRestEvents.ContactDeleted, (contactIdToDelete: number) => {
                    this.deleteContactFromModel(contactIdToDelete);
                    return $.Deferred<void>().resolve();
                });
                instance.registerEventHandler(instance.uscContattiSelRestEvents.NewContactsAdded, (newAddedContact: ContactModel) => {
                    this.addContactToModel(newAddedContact);
                    return $.Deferred<void>().resolve();
                });
            });
    }

    private deleteContactFromModel(contactIdToDelete: number): void {
        if (!contactIdToDelete)
            return;
        this.fascicleContacts = this.fascicleContacts.filter(x => x.EntityId !== contactIdToDelete);
    }

    private addContactToModel(newAddedContact: ContactModel): void {
        if (!newAddedContact)
            return;
        this.fascicleContacts.push(newAddedContact);
    }
}

export = FascModifica;