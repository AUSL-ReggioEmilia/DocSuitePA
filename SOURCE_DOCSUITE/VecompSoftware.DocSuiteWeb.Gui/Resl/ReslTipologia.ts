/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ResolutionKindService = require('App/Services/Resolutions/ResolutionKindService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ResolutionKindModel = require('App/Models/Resolutions/ResolutionKindModel');
import UscResolutionKindDetails = require('UserControl/UscResolutionKindDetails');
import UscResolutionKindSeries = require('UserControl/UscResolutionKindSeries');
import UscErrorNotification = require('UserControl/UscErrorNotification');

class ReslTipologia {
    tlbStatusSearchId: string;
    rtvResolutionKindsId: string;
    btnAddId: string;
    btnEditId: string;
    btnCancelId: string;
    managerWindowsId: string;
    wndResolutionKindId: string;
    txtKindNameId: string;
    rcbKindActiveId: string;
    btnConfirmId: string;   
    uscResolutionKindDetailsId: string;
    uscResolutionKindSeriesId: string;
    pnlDetailsId: string;
    uscNotificationId: string;
    ajaxLoadingPanelId: string;
    pnlWindowContentId: string;
    defaultManagerWindowsId: string;
    btnRestoreId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _resolutionKindService: ResolutionKindService;
    private _tlbStatusSearch: Telerik.Web.UI.RadToolBar;
    private _rtvResolutionKinds: Telerik.Web.UI.RadTreeView;
    private _btnAdd: Telerik.Web.UI.RadButton;
    private _btnEdit: Telerik.Web.UI.RadButton;
    private _btnCancel: Telerik.Web.UI.RadButton;
    private _txtKindName: Telerik.Web.UI.RadTextBox;
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _loadingManager: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _defaultManagerWindows: Telerik.Web.UI.RadWindowManager;
    private _btnRestore: Telerik.Web.UI.RadButton;

    private static ADD_KIND_COMMAND: string = "createKind";
    private static EDIT_KIND_COMMAND: string = "editKind";
    private static DELETE_KIND_COMMAND: string = "deleteKind";    
    private static ISACTIVE_ATTRIBUTE_NODE: string = "isActive";

    private get searchDisabled(): boolean {
        let toolBarButton: Telerik.Web.UI.RadToolBarButton = this._tlbStatusSearch.findItemByValue("searchDisabled") as Telerik.Web.UI.RadToolBarButton;
        if (toolBarButton) {
            return toolBarButton.get_checked();
        }
        return false;
    }

    private get searchActive(): boolean {
        let toolBarButton: Telerik.Web.UI.RadToolBarButton = this._tlbStatusSearch.findItemByValue("searchActive") as Telerik.Web.UI.RadToolBarButton;
        if (toolBarButton) {
            return toolBarButton.get_checked();
        }
        return false;
    }

    private get rootTreeNode(): Telerik.Web.UI.RadTreeNode {
        return this._rtvResolutionKinds.get_nodes().getNode(0);
    }

    private get currentSelectedNode(): Telerik.Web.UI.RadTreeNode {
        return this._rtvResolutionKinds.get_selectedNode();
    }    

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    /**
     *------------------------- Events -----------------------------
     */
    private btnAdd_Click = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this._txtKindName.set_value('');
        $("#".concat(this.rcbKindActiveId)).prop("checked", true);
        this._btnConfirm.set_commandArgument(ReslTipologia.ADD_KIND_COMMAND);
        this.openWindow(this.wndResolutionKindId, "Inserimento nuova tipologia di atto");
    }

    private btnEdit_Click = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        if (!this.currentSelectedNode || !this.currentSelectedNode.get_value()) {
            alert("Selezionare una tipologia di atto per la modifica");
            return;
        }

        this._txtKindName.set_value(this.currentSelectedNode.get_text());
        let isActive: boolean = this.currentSelectedNode.get_attributes().getAttribute(ReslTipologia.ISACTIVE_ATTRIBUTE_NODE);
        $("#".concat(this.rcbKindActiveId)).prop("checked", isActive);
        this._btnConfirm.set_commandArgument(ReslTipologia.EDIT_KIND_COMMAND);
        this.openWindow(this.wndResolutionKindId, "Modifica tipologia di atto");
    }

    private btnCancel_Click = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {        
        if (!this.currentSelectedNode || !this.currentSelectedNode.get_value()) {
            alert("Selezionare una tipologia di atto per la cancellazione");
            sender.enableAfterSingleClick();
            return;
        }

        this._defaultManagerWindows.radconfirm("Sei sicuro di voler rimuovere la tipologia selezionata?", (arg) => {
            if (arg) {
                try {
                    let model: ResolutionKindModel = {} as ResolutionKindModel;
                    model.UniqueId = this.currentSelectedNode.get_value();
                    this._loadingManager.show(this.rtvResolutionKindsId);
                    this.saveResolutionKind(model, ReslTipologia.DELETE_KIND_COMMAND)
                        .done(() => {
                            this.loadTipologies()
                                .fail((exception) => this.showNotificationException(exception))
                                .always(() => this._loadingManager.hide(this.rtvResolutionKindsId));
                        })
                        .fail((exception) => {
                            this._loadingManager.hide(this.rtvResolutionKindsId);
                            this.showNotificationException(exception);
                        });
                } catch (e) {
                    console.error(e);
                    this.showNotificationException("Errore nella fase di cancellazione tipologia atto");
                }
            }
            sender.enableAfterSingleClick();
        }, 300, 160);
    }

    private btnRestore_Click = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        if (!this.currentSelectedNode || !this.currentSelectedNode.get_value()) {
            alert("Selezionare una tipologia di atto per il recupero");
            sender.enableAfterSingleClick();
            return;
        }

        this._defaultManagerWindows.radconfirm("Sei sicuro di voler recuperare la tipologia selezionata?", (arg) => {
            if (arg) {
                try {
                    let model: ResolutionKindModel = {} as ResolutionKindModel;
                    model.Name = this.currentSelectedNode.get_text();
                    model.UniqueId = this.currentSelectedNode.get_value();
                    model.IsActive = true;
                    this._loadingManager.show(this.rtvResolutionKindsId);
                    this.saveResolutionKind(model, ReslTipologia.EDIT_KIND_COMMAND)
                        .done(() => {
                            this.loadTipologies()
                                .fail((exception) => this.showNotificationException(exception))
                                .always(() => this._loadingManager.hide(this.rtvResolutionKindsId));
                        })
                        .fail((exception) => {
                            this._loadingManager.hide(this.rtvResolutionKindsId);
                            this.showNotificationException(exception);
                        });
                } catch (e) {
                    console.error(e);
                    this.showNotificationException("Errore nella fase di recupero tipologia atto");
                }
            }
            sender.enableAfterSingleClick();
        }, 300, 160);
    }

    private rtvResolutionKinds_NodeClicked = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        this.setButtonsBehaviors(args.get_node());

        if (!args.get_node().get_value()) {
            $("#".concat(this.pnlDetailsId)).hide();
            return;
        }

        $("#".concat(this.pnlDetailsId)).show();
        this._loadingManager.show(this.pnlDetailsId);
        this.loadDetails(args.get_node())
            .fail((exception) => this.showNotificationException(exception))
            .always(() => this._loadingManager.hide(this.pnlDetailsId));
    }

    private btnConfirm_Click = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {        
        if (!this._txtKindName.get_value()) {
            alert("Nessun nome definito per la tipologia di atto");
            sender.enableAfterSingleClick();
            return;
        }

        try {
            let model: ResolutionKindModel = {} as ResolutionKindModel;
            model.Name = this._txtKindName.get_value();
            model.IsActive = $("#".concat(this.rcbKindActiveId)).is(':checked');
            switch (sender.get_commandArgument()) {
                case ReslTipologia.EDIT_KIND_COMMAND:
                    {
                        model.UniqueId = this.currentSelectedNode.get_value();
                    }
                    break;
            }
            this._loadingManager.show(this.pnlWindowContentId);
            this.saveResolutionKind(model, sender.get_commandArgument())
                .done(() => {
                    this.closeWindow(this.wndResolutionKindId);
                    this._loadingManager.show(this.rtvResolutionKindsId);
                    this.loadTipologies()
                        .fail((exception) => this.showNotificationException(exception))
                        .always(() => this._loadingManager.hide(this.rtvResolutionKindsId));
                })
                .fail((exception) => this.showNotificationException(exception))
                .always(() => {                    
                    sender.enableAfterSingleClick();
                    this._loadingManager.hide(this.pnlWindowContentId);                    
                });
        } catch (e) {
            console.error(e);
            sender.enableAfterSingleClick();
            this.showNotificationException("Errore nella fase di salvataggio della tipologia atto");
        }        
    }

    tlbStatusSearch_ButtonClicked = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        this._loadingManager.show(this.rtvResolutionKindsId);
        this.loadTipologies()
            .fail((exception) => this.showNotificationException(exception))
            .always(() => this._loadingManager.hide(this.rtvResolutionKindsId));
    }

    /**
     *------------------------- Methods -----------------------------
     */
    initialize(): void {
        this._tlbStatusSearch = $find(this.tlbStatusSearchId) as Telerik.Web.UI.RadToolBar;
        this._tlbStatusSearch.add_buttonClicked(this.tlbStatusSearch_ButtonClicked);
        this._rtvResolutionKinds = $find(this.rtvResolutionKindsId) as Telerik.Web.UI.RadTreeView;
        this._rtvResolutionKinds.add_nodeClicked(this.rtvResolutionKinds_NodeClicked);
        this._btnAdd = $find(this.btnAddId) as Telerik.Web.UI.RadButton;
        this._btnAdd.add_clicked(this.btnAdd_Click);
        this._btnEdit = $find(this.btnEditId) as Telerik.Web.UI.RadButton;
        this._btnEdit.add_clicked(this.btnEdit_Click);
        this._btnCancel = $find(this.btnCancelId) as Telerik.Web.UI.RadButton;
        this._btnCancel.add_clicked(this.btnCancel_Click);
        this._btnConfirm = $find(this.btnConfirmId) as Telerik.Web.UI.RadButton;
        this._btnConfirm.add_clicked(this.btnConfirm_Click);
        this._txtKindName = $find(this.txtKindNameId) as Telerik.Web.UI.RadTextBox;
        this._loadingManager = $find(this.ajaxLoadingPanelId) as Telerik.Web.UI.RadAjaxLoadingPanel;
        this._defaultManagerWindows = $find(this.defaultManagerWindowsId) as Telerik.Web.UI.RadWindowManager;
        this._btnRestore = $find(this.btnRestoreId) as Telerik.Web.UI.RadButton;
        this._btnRestore.add_clicked(this.btnRestore_Click);

        let resolutionKindConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "ResolutionKind");
        this._resolutionKindService = new ResolutionKindService(resolutionKindConfiguration);

        $("#".concat(this.pnlDetailsId)).hide();

        this.setButtonsBehaviors(this.rootTreeNode);

        this._loadingManager.show(this.rtvResolutionKindsId);
        this.loadTipologies()
            .fail((exception) => this.showNotificationException(exception))
            .always(() => this._loadingManager.hide(this.rtvResolutionKindsId));
    }

    private loadTipologies(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        this.rootTreeNode.get_nodes().clear();
        let action: Function;
        switch (true) {
            case this.searchActive && !this.searchDisabled:
                action = (c, e) => this._resolutionKindService.findActiveTypologies(c, e);
                break;
            case this.searchActive && this.searchDisabled:
            case !this.searchActive && !this.searchDisabled:
                action = (c, e) => this._resolutionKindService.findAllTypologies(c, e);
                break;
            case this.searchDisabled && !this.searchActive:
                action = (c, e) => this._resolutionKindService.findDisabledTypologies(c, e);
                break;
            default:
        }

        action((data: any) => {
                if (!data) return;
                try {
                    let resolutionKinds: ResolutionKindModel[] = data as ResolutionKindModel[];
                    let node: Telerik.Web.UI.RadTreeNode;
                    for (let resolutionKind of resolutionKinds) {
                        node = new Telerik.Web.UI.RadTreeNode();
                        node.set_text(resolutionKind.Name);
                        node.set_value(resolutionKind.UniqueId);
                        node.get_attributes().setAttribute(ReslTipologia.ISACTIVE_ATTRIBUTE_NODE, resolutionKind.IsActive);
                        if (resolutionKind.IsActive) {
                            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/type_definition.png");
                        } else {
                            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/type_definition_private.png");
                            node.set_cssClass("node-disabled");
                        }
                        this.rootTreeNode.get_nodes().add(node);
                    }
                    this.rootTreeNode.expand();
                    this.rootTreeNode.select();
                    this.setButtonsBehaviors(this.rootTreeNode);
                    $("#".concat(this.pnlDetailsId)).hide();
                    promise.resolve();
                } catch (e) {
                    console.error(e);
                    promise.reject("Errore nel caricamento delle tipologie di atto");
                }                
            },
            (exception: ExceptionDTO) => {                
                promise.reject(exception);
            }
        );
        return promise.promise();
    }    

    private setButtonsBehaviors(nodeSelected: Telerik.Web.UI.RadTreeNode): void {
        let rootNodeSelected: boolean = !!!nodeSelected.get_value();
        this._btnAdd.set_enabled(rootNodeSelected);
        this._btnEdit.set_enabled(!rootNodeSelected);
        let isActive: boolean = Boolean(nodeSelected.get_attributes().getAttribute(ReslTipologia.ISACTIVE_ATTRIBUTE_NODE));
        this._btnCancel.set_enabled(!rootNodeSelected && isActive);
        this._btnRestore.set_enabled(!rootNodeSelected && !isActive);
    }

    private saveResolutionKind(model: ResolutionKindModel, command: string): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let action: Function;
        switch (command) {
            case ReslTipologia.ADD_KIND_COMMAND:
                {
                    action = (m, c, e) => this._resolutionKindService.insertResolutionKindModel(m, c, e);
                }
                break;
            case ReslTipologia.EDIT_KIND_COMMAND:
                {
                    action = (m, c, e) => this._resolutionKindService.updateResolutionKindModel(m, c, e);
                }
                break;
            case ReslTipologia.DELETE_KIND_COMMAND:
                {
                    action = (m, c, e) => this._resolutionKindService.deleteResolutionKindModel(m, c, e);
                }
                break;
            default:
                {
                    throw new Error("Command type ".concat(command, " not defined"));
                }
        }

        action(model,
            (data: any) => {
                promise.resolve();
            },
            (exception: ExceptionDTO) => {
                promise.reject(exception);
            }
        );
        return promise.promise();
    }    

    loadDetails(selectedNode: Telerik.Web.UI.RadTreeNode): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let idResolutionKind: string = selectedNode.get_value();
        this._resolutionKindService.getById(idResolutionKind,
            (data: any) => {
                let resolutionKind: ResolutionKindModel = data as ResolutionKindModel;
                let uscResolutionKindDetails: UscResolutionKindDetails = <UscResolutionKindDetails>$("#".concat(this.uscResolutionKindDetailsId)).data();
                if (!jQuery.isEmptyObject(uscResolutionKindDetails)) {
                    uscResolutionKindDetails.loadDetails(resolutionKind)
                }

                let uscResolutionKindSeries: UscResolutionKindSeries = <UscResolutionKindSeries>$("#".concat(this.uscResolutionKindSeriesId)).data();
                if (!jQuery.isEmptyObject(uscResolutionKindSeries)) {
                    uscResolutionKindSeries.loadSeries(resolutionKind);
                }
                promise.resolve();
            },
            (exception: ExceptionDTO) => {
                promise.reject(exception);
            }
        );
        return promise.promise();
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
export = ReslTipologia;