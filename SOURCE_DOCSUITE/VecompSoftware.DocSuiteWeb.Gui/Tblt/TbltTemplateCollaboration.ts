/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import TemplateCollaborationService = require('App/Services/Templates/TemplateCollaborationService');
import TemplateCollaborationModel = require('App/Models/Templates/TemplateCollaborationModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import TemplateCollaborationRepresentationType = require('App/Models/Templates/TemplateCollaborationRepresentationType');
import UscTemplateCollaborationRest = require('UserControl/UscTemplateCollaborationRest');
import StringHelper = require('App/Helpers/StringHelper');
import CollaborationDocumentType = require('App/Models/Collaborations/CollaborationDocumentType');
import CrossWindowMessagingListener = require('App/Core/Messaging/CrossWindowMessagingListener');
import TemplatesConstants = require('App/Core/Templates/TemplatesConstants');
import EnumValue = require('App/Helpers/EnumValue');
import TemplateCollaborationStatus = require('App/Models/Templates/TemplateCollaborationStatus');
import BoolValue = require('App/Helpers/BoolValue');
import PageClassHelper = require('App/Helpers/PageClassHelper');

/*
 *  folder delete functionality is not CURRENTLY implemented in WebApi
 *   - the ui functionality is implemented to delete, and regenerate the tree. The delete folder button
 *   - is kept on TemplateUserCollCartellaGestione but not rendered until WebApi functionality is made
 */

class TbltTemplateCollaboration {

    /**
     * initialized from TbltTemplateCollaboration.aspx
     */
    public uscNotificationId: string;
    public folderToolbarId: string;
    public filterToolbarId: string;
    public uscTemplateCollaborationRestId: string;
    public templateCollaborationDetailsPaneId: string;
    public queryViewNotActive: string;

    /**
     * private fields
     */
    private _serviceTemplateCollaboration: TemplateCollaborationService;
    private _folderToolbar: Telerik.Web.UI.RadToolBar;
    private _filterToolbar: Telerik.Web.UI.RadToolBar;
    private _serviceConfigurations: ServiceConfiguration[];
    private _templateCollaborationDetailsPane: Telerik.Web.UI.RadPane;
    private _selectedNode: TemplateCollaborationModel;
    private _messageListener: CrossWindowMessagingListener;
    private _rcbStatusCombo: Telerik.Web.UI.RadComboBox;

    //TODO: searh to be implemented in lazy loaded tre
    //private _rtbSearchName: Telerik.Web.UI.RadComboBox;

    private static TOOLBAR_CREATE_FOLDER = "createFolder";
    private static TOOLBAR_CREATE_TEMPLATE = "createTemplate";

    private static TEMPLATE_UPDATE_TEMPLATE_URL = "../User/TemplateUserCollGestione.aspx?Action=Edit&Type={0}&TemplateId={1}"
    private static TEMPLATE_INSERT_TEMPLATE_URL = "../User/TemplateUserCollGestione.aspx?Action=Insert&Type={0}&ParentId={1}";

    private static TEMPLATE_UPDATE_FOLDER_URL = "../User/TemplateUserCollCartellaGestione.aspx?Action=Edit&TemplateId={0}";
    private static TEMPLATE_INSERT_FOLDER_URL = "../User/TemplateUserCollCartellaGestione.aspx?Action=Insert&ParentId={0}";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._messageListener = new CrossWindowMessagingListener(window);
    }

    // called from aspx page
    public initialize(): void {
        this.InitializeServices();
        this.InitializeControls();
        this.InitializeEvents();
        this.RefreshToolbarItemsLocking(null);

        let treeInitialized = false;
        if (this.queryViewNotActive !== null || this.queryViewNotActive !== undefined) {
            let boolValue = new BoolValue(this.queryViewNotActive, false);
            if (boolValue.IsValid && boolValue.ValueAsBoolean) {
                treeInitialized = true;
                PageClassHelper.callUserControlFunctionSafe<UscTemplateCollaborationRest>(this.uscTemplateCollaborationRestId)
                    .done((instance) => {
                        let node = this._rcbStatusCombo.findItemByValue(TemplateCollaborationStatus.NotActive.toString());
                        node.select();
                    });
            }
        }
        if (!treeInitialized) {
            PageClassHelper.callUserControlFunctionSafe<UscTemplateCollaborationRest>(this.uscTemplateCollaborationRestId)
                .done((instance) => {
                    instance.ReloadRoot()
                });
        }
    }

    /*
     * Initializers
     */
    private InitializeServices(): void {

        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "TemplateCollaboration");
        if (!serviceConfiguration) {
            this.ShowNotificationMessage("Errore in inizializzazione. Nessun servizio configurato per il Template di collaborazione");
            return;
        }
        this._serviceTemplateCollaboration = new TemplateCollaborationService(serviceConfiguration);
    }

    private InitializeControls(): void {
        this._folderToolbar = <Telerik.Web.UI.RadToolBar>$find(this.folderToolbarId);
        this._filterToolbar = <Telerik.Web.UI.RadToolBar>$find(this.filterToolbarId);
        this._templateCollaborationDetailsPane = <Telerik.Web.UI.RadPane>$find(this.templateCollaborationDetailsPaneId);
        this._rcbStatusCombo = <Telerik.Web.UI.RadComboBox>this._filterToolbar.findItemByValue("selectStatus").findControl("cmbTemplateStatus");

        //TODO: search to be implemented on lazy loaded tree
        //this._rtbSearchName = <Telerik.Web.UI.RadComboBox>this._filterToolbar.findItemByValue("searchInput").findControl("txtSearch");

        this.EnsureNotNullOrUndefined(this._folderToolbar, '_folderToolbar');
        this.EnsureNotNullOrUndefined(this._templateCollaborationDetailsPane, '_templateCollaborationDetailsPane');
    }

    private InitializeEvents(): void {
        this._folderToolbar.add_buttonClicked(this.FolderToolBarOnClick);

        PageClassHelper.callUserControlFunctionSafe<UscTemplateCollaborationRest>(this.uscTemplateCollaborationRestId)
            .done((instance) => {
                // listening to messages from the usc tree component (located in the left pane)
                UscTemplateCollaborationRest.OnTreeNodeClicked(this.uscTemplateCollaborationRestId, this.OnTreeNodeClicked);
                UscTemplateCollaborationRest.OnTreeRootNodeClicked(this.uscTemplateCollaborationRestId, this.OnTreeRootNodeClicked);
            });
      
        // listening to messages from the window loaded in the iframe (right radpane)
        this._messageListener.ListenToMessage<TemplateCollaborationModel>(TemplatesConstants.Events.EventFolderCreated, payload => {
            if (payload.ParentInsertId) { //insert folder
                this.GetTreeControl().ReloadContent(payload.ParentInsertId)
                    .then(() => {
                        // selecting the node will trigger and event which ends up back here
                        // in the handler [this.OnTreeNodeClicked]
                        this.GetTreeControl().SelectNode(payload.UniqueId);
                    });
            }
            else { //update folder
                this.GetTreeControl().ReloadContentForParentOf(payload.UniqueId)
                    .then(() => {
                        // selecting the node will trigger and event which ends up back here
                        // in the handler [this.OnTreeNodeClicked]
                        this.GetTreeControl().SelectNode(payload.UniqueId);
                    });
            }
        });

        this._messageListener.ListenToMessage(TemplatesConstants.Events.EventFolderDeleted, (uniqueId: string) => {
            this.GetTreeControl().ReloadContentForParentOf(uniqueId)
                .then(() => {
                    // selecting the node will trigger and event which ends up back here
                    // in the handler [this.OnTreeRootNodeClicked]
                    this.GetTreeControl().SelectRootNode();
                });
        });

        this._messageListener.ListenToMessage<TemplateCollaborationModel>(TemplatesConstants.Events.EventTemplateCreated, payload => {
            if (payload.ParentInsertId) { //insert template
                this.GetTreeControl().ReloadContent(payload.ParentInsertId)
                    .then(() => {
                        // selecting the node will trigger and event which ends up back here
                        // in the handler [this.OnTreeNodeClicked]

                        this.GetTreeControl().SelectNode(payload.UniqueId);
                    });
            }
            else { //update template
                this.GetTreeControl().ReloadContentForParentOf(payload.UniqueId)
                    .then(() => {
                        // selecting the node will trigger and event which ends up back here
                        // in the handler [this.OnTreeNodeClicked]

                        this.GetTreeControl().SelectNode(payload.UniqueId);
                    });
            }
        });

        this._messageListener.ListenToMessage(TemplatesConstants.Events.EventTemplateDeleted, (uniqueId: string) => {
            this.GetTreeControl().ReloadContentForParentOf(uniqueId)
                .then(() => {
                    // selecting the node will trigger and event which ends up back here
                    // in the handler [this.OnTreeRootNodeClicked]
                    this.GetTreeControl().SelectRootNode();
                });
        });

        this._rcbStatusCombo.add_selectedIndexChanged(this.StatusSelectedItemChanged);
    }

    /**
     * FOLDER TOOLBAR
     */

    private RefreshToolbarItemsLocking(selectedNode: TemplateCollaborationModel | null) {
        if (selectedNode === null || selectedNode === undefined) { // this is root
            this.GetFolderToolbarCreateFolderButton().set_enabled(false);
            this.GetFolderToolbarCreateTemplateButton().set_enabled(false);
            return;
        }
        if (selectedNode.RepresentationTypeValue.Equals(TemplateCollaborationRepresentationType.FixedTemplates)) {
            this.GetFolderToolbarCreateFolderButton().set_enabled(true);
            this.GetFolderToolbarCreateTemplateButton().set_enabled(true);
            return;
        }
        if (selectedNode.RepresentationTypeValue.Equals(TemplateCollaborationRepresentationType.Template)) {
            this.GetFolderToolbarCreateFolderButton().set_enabled(false);
            this.GetFolderToolbarCreateTemplateButton().set_enabled(false);
            return;
        }

        if (selectedNode.RepresentationTypeValue.Equals(TemplateCollaborationRepresentationType.Folder)) {
            this.GetFolderToolbarCreateFolderButton().set_enabled(true);
            this.GetFolderToolbarCreateTemplateButton().set_enabled(true);
            return;
        }

        throw new Error(`Representation type ${selectedNode.RepresentationTypeValue.ValueAsString} not supported.`);
    }

    /**
     * FOLDER TOOLBAR EVENTS
     **/

    private FolderToolBarOnClick = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        switch (args.get_item().get_value()) {
            case TbltTemplateCollaboration.TOOLBAR_CREATE_FOLDER: {
                this.CreateFolderButton_OnClick();
                break;
            }
            case TbltTemplateCollaboration.TOOLBAR_CREATE_TEMPLATE: {
                this.CreateTemplateButton_OnClick();
                break;
            }
        }
    }

    private CreateFolderButton_OnClick() {
        if (this.TemplateModelCanHostFolder(this._selectedNode)) {
            this.LoadRightPaneWithNewFolderPage(this._selectedNode);
        }
    }

    private CreateTemplateButton_OnClick() {
        if (this.TemplateModelCanHostTemplate(this._selectedNode)) {
            this.LoadRightPaneWithNewTemplatePage(this._selectedNode);
        }
    }

    /*
     * FILTER TOOLBAR
     */

    private StatusSelectedItemChanged = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        let currentValueStr = args.get_item().get_value();
        this.ChangeFilterInUscControl(currentValueStr);
        this.UnloadRightPaneContent();
        this.GetTreeControl().SelectRootNode();
    }


    private ChangeFilterInUscControl(status: string | TemplateCollaborationStatus) {
        let value = new EnumValue(status, TemplateCollaborationStatus, false);

        if (!value.IsValid) {
            // the value for the first dropdown is -1 and it's not part of the enum status
            // if this is selected, the enumvalue class will detect it and set the inner flag as not valid
            this.GetTreeControl().SetFilterStatus(null);
        } else {
            this.GetTreeControl().SetFilterStatus(value.ValueAsNumber);
        }
    }

    /*
     * USC TREE EVENTS
     */

    private OnTreeNodeClicked = (selectedNode: TemplateCollaborationModel) => {
        this.RefreshToolbarItemsLocking(selectedNode);
        this.StoreCurrentSelectedNode(selectedNode);

        if (this.TemplateModelIsTemplateLike(selectedNode)) {
            this.LoadRightPaneWithEditTemplatePage(selectedNode);
        } else {
            this.LoadRightPaneWithEditFolderPage(selectedNode);
        }
    }

    private OnTreeRootNodeClicked = () => {
        this.RefreshToolbarItemsLocking(null);
        this.StoreCurrentSelectedNode(null);
        this.UnloadRightPaneContent();
    }


    private StoreCurrentSelectedNode(selectedNode: TemplateCollaborationModel | null): void {
        this._selectedNode = selectedNode;
    }

    /**
     * RIGHT PANE 
     **/

    private UnloadRightPaneContent(): void {
        this._templateCollaborationDetailsPane.set_contentUrl('');
    }

    private LoadRightPaneWithNewTemplatePage(selectedNode: TemplateCollaborationModel): void {
        let parentId = selectedNode.UniqueId;
        let pageType = this.GetPageTypeFromDocumentType(selectedNode);

        let url = (new StringHelper()).format(
            TbltTemplateCollaboration.TEMPLATE_INSERT_TEMPLATE_URL,
            pageType,
            parentId);
        this._templateCollaborationDetailsPane.set_contentUrl(url);
    }

    private LoadRightPaneWithEditTemplatePage(selectedNode: TemplateCollaborationModel): void {
        let pageType = this.GetPageTypeFromDocumentType(selectedNode);
        let editingId = selectedNode.UniqueId;

        let url = (new StringHelper()).format(
            TbltTemplateCollaboration.TEMPLATE_UPDATE_TEMPLATE_URL,
            pageType,
            editingId);
        this._templateCollaborationDetailsPane.set_contentUrl(url);
    }

    private LoadRightPaneWithEditFolderPage(selectedNode: TemplateCollaborationModel): void {
        let editingId = selectedNode.UniqueId;

        let url = (new StringHelper()).format(
            TbltTemplateCollaboration.TEMPLATE_UPDATE_FOLDER_URL,
            editingId);
        this._templateCollaborationDetailsPane.set_contentUrl(url);

    }

    private LoadRightPaneWithNewFolderPage(selectedNode: TemplateCollaborationModel): void {
        // in this case selectedNode will be the parent of the new folder
        let parentId = selectedNode.UniqueId;

        let url = (new StringHelper()).format(
            TbltTemplateCollaboration.TEMPLATE_INSERT_FOLDER_URL,
            parentId);

        this._templateCollaborationDetailsPane.set_contentUrl(url);
    }

    /*
     * UTILS
     */

    private TemplateModelCanHostFolder(selectedNode: TemplateCollaborationModel): boolean {
        if (selectedNode === null || selectedNode === undefined) {
            return false;
        }
        return selectedNode.RepresentationTypeValue.Equals(TemplateCollaborationRepresentationType.Folder)
            || selectedNode.RepresentationTypeValue.Equals(TemplateCollaborationRepresentationType.FixedTemplates);
    }

    private TemplateModelCanHostTemplate(selectedNode: TemplateCollaborationModel): boolean {
        if (selectedNode === null || selectedNode === undefined) {
            return false;
        }
        return selectedNode.RepresentationTypeValue.Equals(TemplateCollaborationRepresentationType.Folder)
            || selectedNode.RepresentationTypeValue.Equals(TemplateCollaborationRepresentationType.FixedTemplates);
    }

    private TemplateModelIsTemplateLike(selectedNode: TemplateCollaborationModel): boolean {
        if (selectedNode === null || selectedNode === undefined) {
            return false;
        }
        return selectedNode.RepresentationTypeValue.Equals(TemplateCollaborationRepresentationType.FixedTemplates)
            || selectedNode.RepresentationTypeValue.Equals(TemplateCollaborationRepresentationType.Template);
    }

    private GetPageTypeFromDocumentType(model: TemplateCollaborationModel): string {
        if (model.DocumentType == CollaborationDocumentType[CollaborationDocumentType.A]
            || model.DocumentType == CollaborationDocumentType[CollaborationDocumentType.D]) {
            return 'resl';
        }

        if (model.DocumentType == CollaborationDocumentType[CollaborationDocumentType.S]) {
            return 'series';
        }

        if (model.DocumentType == CollaborationDocumentType[CollaborationDocumentType.UDS]
            || !isNaN(parseInt(model.DocumentType))) {
            return 'uds';
        }
        return 'prot';
    }

    private GetTreeControl(): UscTemplateCollaborationRest {
        return jQuery(`#${this.uscTemplateCollaborationRestId}`).data()
    }

    private GetFolderToolbarCreateFolderButton(): Telerik.Web.UI.RadToolBarItem {
        return this._folderToolbar.findItemByValue(TbltTemplateCollaboration.TOOLBAR_CREATE_FOLDER);
    }

    private GetFolderToolbarCreateTemplateButton(): Telerik.Web.UI.RadToolBarItem {
        return this._folderToolbar.findItemByValue(TbltTemplateCollaboration.TOOLBAR_CREATE_TEMPLATE);
    }

    private IsRootNode(node: Telerik.Web.UI.RadTreeNode) {
        return node.get_level() == 0;
    }

    /*
     * ERROR HANDLING
     */

    private EnsureNotNullOrUndefined(source: any, propertyName: string): void {
        if (source === null || source === undefined) {
            throw new Error(`Invalid property state. Property \'${propertyName}\' is null or undefined.`);
        }
    }

    private ShowNotificationMessage(customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${this.uscNotificationId}`).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage)
        }
    }
}


export = TbltTemplateCollaboration;