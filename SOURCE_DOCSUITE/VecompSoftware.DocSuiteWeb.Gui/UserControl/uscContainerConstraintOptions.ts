/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import DocumentSeriesConstraintService = require('App/Services/DocumentArchives/DocumentSeriesConstraintService');
import DocumentSeriesConstraintModel = require('App/Models/DocumentArchives/DocumentSeriesConstraintModel');
import DocumentSeriesModel = require('App/Models/DocumentArchives/DocumentSeriesModel');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import GuidHelper = require('App/Helpers/GuidHelper');
import UscErrorNotification = require('UserControl/uscErrorNotification');

import ServiceConfiguration = require('App/Services/ServiceConfiguration');

class UscContainerConstraintOptions {
    rtbConstraintActionsId: string;
    rtvConstraintsId: string;
    managerWindowsId: string;
    windowManageConstraintId: string;
    txtConstraintNameId: string;
    btnConfirmId: string;
    splPageContentId: string;
    btnSaveId: string;
    seriesId: number;
    uscNotificationId: string;
    ajaxLoadingPanelId: string;
    windowManagerId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _rtbConstraintActions: Telerik.Web.UI.RadToolBar;
    private _rtvConstraints: Telerik.Web.UI.RadTreeView;
    private _txtConstraintName: Telerik.Web.UI.RadTextBox;
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _service: DocumentSeriesConstraintService;
    private _btnSave: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _windowManager: Telerik.Web.UI.RadWindowManager;

    private static CREATE_CONSTRAINT_ACTION = "createConstraint";
    private static EDIT_CONSTRAINT_ACTION = "editConstraint";
    private static REMOVE_CONSTRAINT_ACTION = "removeConstraint";
    private static NODE_COMMAND_ATTRIBUTE = "NodeCommandType";
    private static PERSISTED_ATTRIBUTE = "IsAlreadyPersisted";
    private static TO_DELETE_STORAGE_KEY = "ConstraintsToDelete";

    private get currentSelectedNode(): Telerik.Web.UI.RadTreeNode {
        return this._rtvConstraints.get_selectedNode();
    }

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    /**
     *------------------------- Events -----------------------------
     */

    /**
    * Evento scatenato al click della toolbar delle cartelle
    */
    rtbConstraintActions_ButtonClicked = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        try {            
            let item: Telerik.Web.UI.RadToolBarItem = args.get_item();
            if (item) {
                switch (item.get_value()) {
                    case UscContainerConstraintOptions.CREATE_CONSTRAINT_ACTION: {
                        {
                            this._txtConstraintName.set_value('');
                            this._btnConfirm.set_commandArgument(UscContainerConstraintOptions.CREATE_CONSTRAINT_ACTION);
                            this.openWindow(this.windowManageConstraintId, "Inserimento nuovo obbligo di trasparenza");
                        }
                        break;
                    }
                    case UscContainerConstraintOptions.EDIT_CONSTRAINT_ACTION: {
                        {
                            if (!this.currentSelectedNode.get_value()) {
                                return;
                            }
                            this._txtConstraintName.set_value(this.currentSelectedNode.get_text());
                            this._btnConfirm.set_commandArgument(UscContainerConstraintOptions.EDIT_CONSTRAINT_ACTION);
                            this.openWindow(this.windowManageConstraintId, "Modifica obbligo di trasparenza");
                        }
                        break;
                    }
                    case UscContainerConstraintOptions.REMOVE_CONSTRAINT_ACTION: {
                        {
                            if (!this.currentSelectedNode.get_value()) {
                                return;
                            }
                            this._windowManager.radconfirm("Sei sicuro di voler rimuovere l'obbligo selezionato?", (arg) => {
                                if (arg) {
                                    let constraintModel: DocumentSeriesConstraintModel = {} as DocumentSeriesConstraintModel;
                                    constraintModel.UniqueId = this.currentSelectedNode.get_value();
                                    this.saveConstraint(UscContainerConstraintOptions.REMOVE_CONSTRAINT_ACTION, constraintModel);
                                }
                            }, 300, 160);                            
                        }
                        break;
                    }
                }
            }
        } catch (e) {
            console.error(e);
            this.showNotificationException("Errore nell'esecuzione dell'attività selezionata");
        }        
    }

    rtvConstraints_NodeClicked = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        let currentNode: Telerik.Web.UI.RadTreeNode = args.get_node();
        this.setNodeActionBehaviours(currentNode);
    }

    btnConfirm_Click = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        try {
            let constraintName: string = this._txtConstraintName.get_value();
            if (!constraintName) {
                alert("E' obbligatorio inserire il nome dell'obbligo di trasparenza");
                return;
            }

            this.closeWindow(this.windowManageConstraintId);
            let constraintModel: DocumentSeriesConstraintModel;
            switch (sender.get_commandArgument()) {
                case UscContainerConstraintOptions.CREATE_CONSTRAINT_ACTION:
                    {
                        constraintModel = {} as DocumentSeriesConstraintModel;
                        constraintModel.Name = constraintName;
                        this.saveConstraint(UscContainerConstraintOptions.CREATE_CONSTRAINT_ACTION, constraintModel);
                    }
                    break;                
                case UscContainerConstraintOptions.EDIT_CONSTRAINT_ACTION:
                    {
                        constraintModel = {} as DocumentSeriesConstraintModel;
                        constraintModel.Name = constraintName;
                        constraintModel.UniqueId = this.currentSelectedNode.get_value();
                        this.saveConstraint(UscContainerConstraintOptions.EDIT_CONSTRAINT_ACTION, constraintModel);
                    }
                    break;                
            }            
        } catch (e) {
            console.error(e);
            this.showNotificationException("Errore in gestione dell'obbligo");
        }        
    }

    /**
     *------------------------- Methods -----------------------------
     */
    initialize(): void {
        this._rtbConstraintActions = $find(this.rtbConstraintActionsId) as Telerik.Web.UI.RadToolBar;
        this._rtbConstraintActions.add_buttonClicked(this.rtbConstraintActions_ButtonClicked);
        this._rtvConstraints = $find(this.rtvConstraintsId) as Telerik.Web.UI.RadTreeView;
        this._rtvConstraints.add_nodeClicked(this.rtvConstraints_NodeClicked);
        this._txtConstraintName = $find(this.txtConstraintNameId) as Telerik.Web.UI.RadTextBox;
        this._btnConfirm = $find(this.btnConfirmId) as Telerik.Web.UI.RadButton;
        this._btnConfirm.add_clicked(this.btnConfirm_Click);
        this._loadingPanel = $find(this.ajaxLoadingPanelId) as Telerik.Web.UI.RadAjaxLoadingPanel;
        this._windowManager = $find(this.windowManagerId) as Telerik.Web.UI.RadWindowManager;

        let documentSeriesConstraintConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentSeriesConstraint");
        this._service = new DocumentSeriesConstraintService(documentSeriesConstraintConfiguration);

        this.setNodeActionBehaviours(this._rtvConstraints.get_nodes().getNode(0));
        this.bindLoaded();
    }

    private bindLoaded(): void {
        $("#".concat(this.splPageContentId)).data(this);
    }

    loadConstraints(idSeries: number): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        sessionStorage.removeItem(UscContainerConstraintOptions.TO_DELETE_STORAGE_KEY);
        this._rtvConstraints.get_nodes().getNode(0).get_nodes().clear();
        this.seriesId = idSeries;
        this._loadingPanel.show(this.splPageContentId);
        this._service.getByIdSeries(idSeries,
            (data: any) => {
                if (!data) return;
                let constraints: DocumentSeriesConstraintModel[] = data as DocumentSeriesConstraintModel[];
                let constraintNode: Telerik.Web.UI.RadTreeNode;
                for (let constraint of constraints) {
                    constraintNode = new Telerik.Web.UI.RadTreeNode();
                    constraintNode.set_text(constraint.Name);
                    constraintNode.set_value(constraint.UniqueId);
                    constraintNode.set_imageUrl('../App_Themes/DocSuite2008/imgset16/information.png');
                    constraintNode.get_attributes().setAttribute(UscContainerConstraintOptions.PERSISTED_ATTRIBUTE, true);
                    this._rtvConstraints.get_nodes().getNode(0).get_nodes().add(constraintNode);
                }
                this._rtvConstraints.get_nodes().getNode(0).select();
                this.setNodeActionBehaviours(this._rtvConstraints.get_nodes().getNode(0));
                this._rtvConstraints.get_nodes().getNode(0).expand();
                this._loadingPanel.hide(this.splPageContentId);
                promise.resolve();
            },
            (exception: ExceptionDTO) => {                
                promise.reject(exception);
                this._loadingPanel.hide(this.splPageContentId);
                this.showNotificationException(exception);
            }
        );
        return promise.promise();
    }

    private setNodeActionBehaviours(nodeToCheck: Telerik.Web.UI.RadTreeNode): void {
        let createCommandButton: Telerik.Web.UI.RadToolBarItem = this._rtbConstraintActions.findItemByValue(UscContainerConstraintOptions.CREATE_CONSTRAINT_ACTION);
        (!nodeToCheck.get_value()) ? createCommandButton.enable() : createCommandButton.disable();
        let editCommandButton: Telerik.Web.UI.RadToolBarItem = this._rtbConstraintActions.findItemByValue(UscContainerConstraintOptions.EDIT_CONSTRAINT_ACTION);
        (nodeToCheck.get_value()) ? editCommandButton.enable() : editCommandButton.disable();
        let removeCommandButton: Telerik.Web.UI.RadToolBarItem = this._rtbConstraintActions.findItemByValue(UscContainerConstraintOptions.REMOVE_CONSTRAINT_ACTION);
        (nodeToCheck.get_value()) ? removeCommandButton.enable() : removeCommandButton.disable();
    }

    saveConstraint(command: string, model: DocumentSeriesConstraintModel): void {
        if (!model) {
            return;
        }
        let action: Function;
        switch (command) {
            case UscContainerConstraintOptions.CREATE_CONSTRAINT_ACTION:
                {
                    action = (m, c, e) => this._service.insertDocumentSeriesConstraint(m, c, e);
                }                
                break;

            case UscContainerConstraintOptions.EDIT_CONSTRAINT_ACTION:
                {
                    action = (m, c, e) => this._service.updateDocumentSeriesConstraint(m, c, e);
                }
                break;

            case UscContainerConstraintOptions.REMOVE_CONSTRAINT_ACTION:
                {
                    action = (m, c, e) => this._service.deleteDocumentSeriesConstraint(m, c, e);
                }
                break;
        }
        model.DocumentSeries = {} as DocumentSeriesModel;
        model.DocumentSeries.EntityId = this.seriesId;

        this._loadingPanel.show(this.splPageContentId);
        action(model,
            (data: any) => {
                this._loadingPanel.hide(this.splPageContentId);
                this.loadConstraints(this.seriesId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.splPageContentId);
                this.showNotificationException(exception);
            }
        );
    }    

    /**
    * Apre una nuova nuova RadWindow
    * @param id
    */
    openWindow(id, title): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.managerWindowsId);
        let wnd: Telerik.Web.UI.RadWindow = manager.getWindowById(id);
        wnd.show();
        wnd.set_title(title);
        wnd.set_modal(true);
        wnd.center();
        return false;
    }

    /**
     * Chiude una RadWindow specifica
     * @param id
     */
    closeWindow(id): void {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.managerWindowsId);
        let wnd: Telerik.Web.UI.RadWindow = manager.getWindowById(id);
        wnd.close();
    }

    private showNotificationException(exception: ExceptionDTO)
    private showNotificationException(message: string)
    private showNotificationException(exception: any) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            if (exception instanceof ExceptionDTO) {
                uscNotification.showNotification(exception);
            }
            else {
                uscNotification.showNotificationMessage(exception);
            }
        }
    }
}
export = UscContainerConstraintOptions;
