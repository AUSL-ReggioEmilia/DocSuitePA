/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ResolutionKindModel = require('App/Models/Resolutions/ResolutionKindModel');
import ResolutionKindDocumentSeriesModel = require('App/Models/Resolutions/ResolutionKindDocumentSeriesModel');
import DocumentSeriesModel = require('App/Models/DocumentArchives/DocumentSeriesModel');
import DocumentSeriesConstraintModel = require('App/Models/DocumentArchives/DocumentSeriesConstraintModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ResolutionKindDocumentSeriesService = require('App/Services/Resolutions/ResolutionKindDocumentSeriesService');
import DocumentSeriesService = require('App/Services/DocumentArchives/DocumentSeriesService');
import DocumentSeriesConstraintService = require('App/Services/DocumentArchives/DocumentSeriesConstraintService');
import UscErrorNotification = require('UserControl/UscErrorNotification');
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');

class UscResolutionKindSeries {
    pnlPageContentId: string;
    grdDocumentSeriesId: string;
    btnAddSeriesId: string;
    btnEditSeriesId: string;
    btnCancelSeriesId: string;
    archivesDataSourceId: string;
    rcbArchivesId: string;
    rcbConstraintsId: string;
    constraintsDataSourceId: string;
    btnConfirmSeriesId: string;
    pnlConstraintsSelectionId: string;
    chbDocumentRequiredId: string;
    wndResolutionKindDocumentSeriesId: string;
    managerWindowsId: string;
    uscNotificationId: string;
    ajaxLoadingPanelId: string;
    pnlWindowContentId: string;
    defaultManagerWindowsId: string;

    private static ADD_SERIES_COMMAND: string = "addSeries";
    private static EDIT_SERIES_COMMAND: string = "editSeries";
    private static DELETE_SERIES_COMMAND: string = "deleteSeries";
    private static CONSTRAINT_LOADED_EVENT_NAME: string = "ConstraintsLoaded";

    get panelConstraintsSelectionControl(): JQuery {
        return $("#".concat(this.pnlConstraintsSelectionId));
    }

    get currentResolutionKind(): ResolutionKindModel {
        let sessionValue: string = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KIND_KEY);
        if (!sessionValue) {
            return null;
        }
        return JSON.parse(sessionValue) as ResolutionKindModel;
    }

    private _serviceConfigurations: ServiceConfiguration[];
    private _rcbArchives: Telerik.Web.UI.RadComboBox;
    private _rcbConstraints: Telerik.Web.UI.RadComboBox;
    private _grdDocumentSeries: Telerik.Web.UI.RadGrid;
    private _btnAddSeries: Telerik.Web.UI.RadButton;
    private _btnEditSeries: Telerik.Web.UI.RadButton;
    private _btnCancelSeries: Telerik.Web.UI.RadButton;
    private _btnConfirmSeries: Telerik.Web.UI.RadButton;
    private _archivesDataSource: Telerik.Web.UI.RadClientDataSource;
    private _constraintsDataSource: Telerik.Web.UI.RadClientDataSource;
    private _documentSeriesService: DocumentSeriesService;
    private _documentSeriesConstraintService: DocumentSeriesConstraintService;
    private _resolutionKindDocumentSeriesService: ResolutionKindDocumentSeriesService;
    private _loadingManager: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _windowManager: Telerik.Web.UI.RadWindowManager;
    private _defaultManagerWindows: Telerik.Web.UI.RadWindowManager;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    /**
     *------------------------- Events -----------------------------
     */

    private btnAddSeries_Click = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        try {
            sender.enableAfterSingleClick();
            this.openWindow(this.wndResolutionKindDocumentSeriesId, 'Associa un nuovo archivio alla tipologia atto');
            this._loadingManager.show(this.pnlWindowContentId);
            this._rcbArchives.enable();
            this.resetCombosSource();
            this.panelConstraintsSelectionControl.hide();
            this._btnConfirmSeries.set_commandArgument(UscResolutionKindSeries.ADD_SERIES_COMMAND);
            $("#".concat(this.chbDocumentRequiredId)).prop("checked", false);
            this.loadArchives()
                .fail((exception) => this.showNotificationException(exception))
                .always(() => this._loadingManager.hide(this.pnlWindowContentId));
        } catch (e) {
            console.error(e);
            this.showNotificationException("Errore nella gestione della finestra di associazione archivio a tipologia atto");
        }        
    }

    private btnEditSeries_Click = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        sender.enableAfterSingleClick();
        if (!this.currentResolutionKind) {
            alert("Nessuna tipologia di atto trovata per la modifica");
            return;
        }

        let selectedArchives: Telerik.Web.UI.GridDataItem[] = this._grdDocumentSeries.get_selectedItems();
        if (!selectedArchives || selectedArchives.length == 0) {
            alert("Selezionare un archivio per la modifica");
            return;
        }

        try {            
            this.openWindow(this.wndResolutionKindDocumentSeriesId, 'Modifica archivio tipologia atto');
            this._loadingManager.show(this.pnlWindowContentId);
            this.resetCombosSource();            
            this._btnConfirmSeries.set_commandArgument(UscResolutionKindSeries.EDIT_SERIES_COMMAND);
            let model: ResolutionKindDocumentSeriesModel = selectedArchives[0].get_dataItem();
            $("#".concat(this.chbDocumentRequiredId)).prop("checked", model.DocumentRequired);
            $("#".concat(this.pnlPageContentId)).off(UscResolutionKindSeries.CONSTRAINT_LOADED_EVENT_NAME);
            this._documentSeriesService.getById(model.DocumentSeries.EntityId,
                (data: any) => {
                    $("#".concat(this.pnlPageContentId)).on(UscResolutionKindSeries.CONSTRAINT_LOADED_EVENT_NAME, () => {
                        if (model.DocumentSeriesConstraint) {
                            this._rcbConstraints.requestItems('', true);
                            let constraint: Telerik.Web.UI.RadComboBoxItem = this._rcbConstraints.findItemByValue(model.DocumentSeriesConstraint.UniqueId);                            
                            if (constraint) {
                                constraint.select();
                            }                            
                        }
                    });
                    let item: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                    item.set_value(model.DocumentSeries.EntityId.toString());
                    item.set_text(model.DocumentSeries.Name);
                    this._rcbArchives.get_items().add(item);
                    this._rcbArchives.get_items().getItem(0).select();
                    this._rcbArchives.disable();
                    this._loadingManager.hide(this.pnlWindowContentId);
                },
                (exception: ExceptionDTO) => {
                    this._loadingManager.hide(this.pnlWindowContentId);
                    this.showNotificationException(exception);                    
                }
            );
        } catch (e) {
            console.error(e);
            this._loadingManager.hide(this.pnlWindowContentId);
            this.showNotificationException("Errore nella gestione della finestra di associazione archivio a tipologia atto");            
        }        
    }

    private btnCancelSeries_Click = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {        
        let selectedArchives: Telerik.Web.UI.GridDataItem[] = this._grdDocumentSeries.get_selectedItems();
        if (!selectedArchives || selectedArchives.length == 0) {
            alert("Selezionare un archivio per la cancellazione");
            sender.enableAfterSingleClick();
            return;
        }

        let kindSeriesModel: ResolutionKindDocumentSeriesModel = selectedArchives[0].get_dataItem();
        this._defaultManagerWindows.radconfirm("Sei sicuro di voler rimuovere l'archivio selezionato?", (arg) => {
            if (arg) {
                try {
                    let model: ResolutionKindDocumentSeriesModel = {} as ResolutionKindDocumentSeriesModel;
                    model.UniqueId = kindSeriesModel.UniqueId;

                    this._loadingManager.show(this.grdDocumentSeriesId);
                    this.saveResolutionKindDocumentSeries(model, UscResolutionKindSeries.DELETE_SERIES_COMMAND)
                        .done(() => this.loadSeries(this.currentResolutionKind))
                        .fail((exception) => this.showNotificationException(exception))
                        .always(() => this._loadingManager.hide(this.grdDocumentSeriesId));
                } catch (e) {
                    console.error(e);
                    this.showNotificationException("Errore nella fase di cancellazione archivio da tipologia atto");
                }                
            }            
            sender.enableAfterSingleClick();
        }, 300, 160);
    }

    private rcbArchive_SelectedIndexChanged = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        this._rcbConstraints.clearSelection();
        this._rcbConstraints.clearItems();

        if (!args.get_item()) {
            this.panelConstraintsSelectionControl.hide();
            return;
        }

        let selectedArchive: Telerik.Web.UI.RadComboBoxItem = args.get_item();
        this.loadConstraints(Number(selectedArchive.get_value()))
            .done((countConstraint) => {
                this.panelConstraintsSelectionControl.hide();
                if (countConstraint > 0) {
                    this.panelConstraintsSelectionControl.show();
                }
            })
            .fail((exception) => this.showNotificationException(exception));
    }    

    private btnConfirmSeries_Click = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        let selectedArchive: Telerik.Web.UI.RadComboBoxItem = this._rcbArchives.get_selectedItem();
        if (!selectedArchive || !selectedArchive.get_value()) {
            alert("E' richiesta la selezione di un archivio per il salvataggio");
            sender.enableAfterSingleClick();
            return;
        }

        try {            
            let model: ResolutionKindDocumentSeriesModel = {} as ResolutionKindDocumentSeriesModel;
            model.DocumentRequired = $("#".concat(this.chbDocumentRequiredId)).is(':checked');
            model.DocumentSeries = {} as DocumentSeriesModel;
            model.DocumentSeries.EntityId = Number(selectedArchive.get_value());
            model.ResolutionKind = this.currentResolutionKind;
            if (this._rcbConstraints.get_selectedItem()) {
                model.DocumentSeriesConstraint = {} as DocumentSeriesConstraintModel;
                model.DocumentSeriesConstraint.UniqueId = this._rcbConstraints.get_selectedItem().get_value();
            }

            if (sender.get_commandArgument() == UscResolutionKindSeries.EDIT_SERIES_COMMAND) {
                let selectedArchives: Telerik.Web.UI.GridDataItem[] = this._grdDocumentSeries.get_selectedItems();
                let kindSeriesModel: ResolutionKindDocumentSeriesModel = selectedArchives[0].get_dataItem();
                model.UniqueId = kindSeriesModel.UniqueId;
            }

            this._loadingManager.show(this.pnlWindowContentId);
            this.saveResolutionKindDocumentSeries(model, sender.get_commandArgument())
                .done(() => {
                    this.closeWindow(this.wndResolutionKindDocumentSeriesId);
                    this._loadingManager.show(this.grdDocumentSeriesId);
                    this.loadSeries(this.currentResolutionKind)
                        .always(() => this._loadingManager.hide(this.grdDocumentSeriesId));
                })
                .fail((exception) => this.showNotificationException(exception))
                .always(() => {
                    sender.enableAfterSingleClick();
                    this._loadingManager.hide(this.pnlWindowContentId);
                });
        } catch (e) {
            console.error(e);
            sender.enableAfterSingleClick();
            this.showNotificationException("Errore nella fase di salvataggio archivio per tipologia atto");
        }        
    }

    /**
     *------------------------- Methods -----------------------------
     */

    initialize() {
        this._grdDocumentSeries = $find(this.grdDocumentSeriesId) as Telerik.Web.UI.RadGrid;
        this._btnAddSeries = $find(this.btnAddSeriesId) as Telerik.Web.UI.RadButton;
        this._btnAddSeries.add_clicked(this.btnAddSeries_Click);
        this._btnEditSeries = $find(this.btnEditSeriesId) as Telerik.Web.UI.RadButton;
        this._btnEditSeries.add_clicked(this.btnEditSeries_Click);
        this._btnCancelSeries = $find(this.btnCancelSeriesId) as Telerik.Web.UI.RadButton;
        this._btnCancelSeries.add_clicked(this.btnCancelSeries_Click);
        this._archivesDataSource = $find(this.archivesDataSourceId) as Telerik.Web.UI.RadClientDataSource;
        this._rcbArchives = $find(this.rcbArchivesId) as Telerik.Web.UI.RadComboBox;
        this._rcbArchives.add_selectedIndexChanged(this.rcbArchive_SelectedIndexChanged);
        this._rcbConstraints = $find(this.rcbConstraintsId) as Telerik.Web.UI.RadComboBox;
        this._constraintsDataSource = $find(this.constraintsDataSourceId) as Telerik.Web.UI.RadClientDataSource;
        this._btnConfirmSeries = $find(this.btnConfirmSeriesId) as Telerik.Web.UI.RadButton;
        this._btnConfirmSeries.add_clicked(this.btnConfirmSeries_Click);
        this._loadingManager = $find(this.ajaxLoadingPanelId) as Telerik.Web.UI.RadAjaxLoadingPanel;
        this._windowManager = $find(this.managerWindowsId) as Telerik.Web.UI.RadWindowManager;
        this._defaultManagerWindows = $find(this.defaultManagerWindowsId) as Telerik.Web.UI.RadWindowManager;

        let resolutionKindDocumentSeriesConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "ResolutionKindDocumentSeries");
        this._resolutionKindDocumentSeriesService = new ResolutionKindDocumentSeriesService(resolutionKindDocumentSeriesConfiguration);
        let documentSeriesConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentSeries");
        this._documentSeriesService = new DocumentSeriesService(documentSeriesConfiguration);
        let documentSeriesConstraintConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentSeriesConstraint");
        this._documentSeriesConstraintService = new DocumentSeriesConstraintService(documentSeriesConstraintConfiguration);

        sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KIND_KEY);

        this.bindLoaded();
    }

    private bindLoaded() {
        $("#".concat(this.pnlPageContentId)).data(this);
    }

    loadArchives(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        this._documentSeriesService.getAll(
            (data: any) => {
                (<any>this._archivesDataSource).set_data(data);
                this._archivesDataSource.fetch(undefined);
                promise.resolve();
            },
            (exception: ExceptionDTO) => {
                promise.reject(exception);
            }
        );
        return promise.promise();
    }

    loadConstraints(idDocumentSeries: number): JQueryPromise<number> {
        let promise: JQueryDeferred<number> = $.Deferred<number>();
        this._documentSeriesConstraintService.getByIdSeries(idDocumentSeries,
            (data: any) => {
                if (!data) return promise.resolve(0);
                (<any>this._constraintsDataSource).set_data(data);
                this._constraintsDataSource.fetch(undefined);
                $("#".concat(this.pnlPageContentId)).trigger(UscResolutionKindSeries.CONSTRAINT_LOADED_EVENT_NAME);
                promise.resolve(data.length);
            },
            (exception: ExceptionDTO) => {
                promise.reject(exception);
            }
        )
        return promise.promise();
    }

    loadSeries(resolutionKind: ResolutionKindModel): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        if (!resolutionKind) {
            return promise.resolve();
        }

        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KIND_KEY, JSON.stringify(resolutionKind));
        this._resolutionKindDocumentSeriesService.getByResolutionKind(resolutionKind.UniqueId,
            (data: any) => {
                if (!data) return;
                let masterTable: Telerik.Web.UI.GridTableView = this._grdDocumentSeries.get_masterTableView();
                masterTable.set_dataSource(data);
                masterTable.dataBind();
                masterTable.clearSelectedItems();
                promise.resolve();
            },
            (exception: ExceptionDTO) => {
                promise.reject(exception);
            }
        );
        return promise.promise();
    }

    private saveResolutionKindDocumentSeries(model: ResolutionKindDocumentSeriesModel, command: string): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let action: Function;
        switch (command) {
            case UscResolutionKindSeries.ADD_SERIES_COMMAND:
                {
                    action = (m, c, e) => this._resolutionKindDocumentSeriesService.insertResolutionKindDocumentSeriesModel(m, c, e);
                }
                break;
            case UscResolutionKindSeries.EDIT_SERIES_COMMAND:
                {
                    action = (m, c, e) => this._resolutionKindDocumentSeriesService.updateResolutionKindDocumentSeriesModel(m, c, e);
                }
                break;
            case UscResolutionKindSeries.DELETE_SERIES_COMMAND:
                {
                    action = (m, c, e) => this._resolutionKindDocumentSeriesService.deleteResolutionKindDocumentSeriesModel(m, c, e);
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

    private resetCombosSource(): void {
        this._rcbArchives.enable();
        this._rcbArchives.clearItems();
        this._rcbArchives.clearSelection();
        this._rcbConstraints.clearItems();
        this._rcbConstraints.clearSelection();
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

export = UscResolutionKindSeries;