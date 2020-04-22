/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import FascBase = require("Fasc/FascBase");
import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import uscFascicleFolders = require("UserControl/uscFascicleFolders");
import FascicleMoveItemViewModel = require("App/ViewModels/Fascicles/FascicleMoveItemViewModel");
import Environment = require("App/Models/Environment");
import FascicleSummaryFolderViewModel = require("App/ViewModels/Fascicles/FascicleSummaryFolderViewModel");
import FascicolableBaseModel = require("App/Models/Fascicles/FascicolableBaseModel");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import FascicleDocumentUnitService = require('App/Services/Fascicles/FascicleDocumentUnitService');
import FascicleDocumentService = require("App/Services/Fascicles/FascicleDocumentService");
import UpdateActionType = require("App/Models/UpdateActionType");
import IFascicolableBaseModel = require("App/Models/Fascicles/IFascicolableBaseModel");
import FascicleFolderModel = require("App/Models/Fascicles/FascicleFolderModel");
import AjaxModel = require("App/Models/AjaxModel");
import FascicleDocumentModel = require("App/Models/Fascicles/FascicleDocumentModel");
import ChainType = require("App/Models/DocumentUnits/ChainType");
import FascicleModel = require("App/Models/Fascicles/FascicleModel");
import FascicleFolderService = require("App/Services/Fascicles/FascicleFolderService");
import Guid = require("App/Helpers/GuidHelper");

class FascMoveItems extends FascBase {
    idFascicle: string;
    idFascicleFolder: string;
    uscFascicleFoldersId: string;
    uscNotificationId: string;
    btnConfirmId: string;
    rtvItemsToMoveId: string;
    itemsType: string;
    ajaxLoadingPanelId: string;
    pnlPageId: string;
    radWindowManagerId: string;
    ajaxManagerId: string;
    lblItemSelectedDescriptionId: string;

    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _rtvItemsToMove: Telerik.Web.UI.RadTreeView;
    private _toMoveItems: FascicleMoveItemViewModel[];
    private _serviceConfigurations: ServiceConfiguration[];
    private _fascicleDocumentUnitService: FascicleDocumentUnitService;
    private _fascicleDocumentService: FascicleDocumentService;
    private _fascicleFolderService: FascicleFolderService;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _miscellaneaDeferreds: JQueryDeferred<void>[] = [];

    static FASC_MOVE_ITEMS_Session_key = "FascMoveItemsSessionKey";
    static MOVE_MISCELLANEA_DOCUMENT_AJAX_ACTION_NAME = "MoveMiscellaneaDocument"
    private static DOCUMENT_ITEMS_TYPE = "DocumentType";
    private static FOLDER_ITEMS_TYPE = "FolderType";

    private get ItemTypeMoveActions(): Array<[string, (folderId: string) => JQueryPromise<void>]> {
        let items: Array<[string, (folderId: string) => JQueryPromise<void>]> = [
            [FascMoveItems.DOCUMENT_ITEMS_TYPE, (folderId) => this.moveDocuments(folderId)],
            [FascMoveItems.FOLDER_ITEMS_TYPE, (folderId) => this.moveFolders(folderId)]
        ];
        return items;
    }

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascBase.FASCICLE_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
    }

    /**
     *------------------------- Events -----------------------------
     */

    /**
     * Evento scatenato al click del pulsante conferma
     */
    btnConfirm_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        let uscFascicleFolder: uscFascicleFolders = <uscFascicleFolders>$(`#${this.uscFascicleFoldersId}`).data();
        let selectedFolder: FascicleSummaryFolderViewModel = uscFascicleFolder.getSelectedFascicleFolder(this.idFascicle);
        if (!selectedFolder || !selectedFolder.Typology) {
            this.showWarningMessage(this.uscNotificationId, "Selezionare una cartella del Fascicolo");
            sender.enableAfterSingleClick();
            return;
        }

        if (selectedFolder.UniqueId == this.idFascicleFolder) {
            this.showWarningMessage(this.uscNotificationId, "Selezionare una cartella di destinazione differente dalla cartella originale");
            sender.enableAfterSingleClick();
            return;
        }

        let moveActions: ((folderId: string) => JQueryPromise<void>)[] = this.ItemTypeMoveActions.filter((item: [string, (folderId: string) => JQueryPromise<void>]) => item[0] == this.itemsType)
            .map((item: [string, (folderId: string) => JQueryPromise<void>]) => item[1]);

        if (!moveActions || moveActions.length == 0) {
            this.showNotificationMessage(this.uscNotificationId, "E' avvenuto un errore durante la procedura di sposta");
            return;
        }

        this._manager.radconfirm(`Sei sicuro di voler eseguire lo spostamento degli elementi selezionati nella cartella ${selectedFolder.Name}?`, (arg) => {
            if (!arg) {
                sender.enableAfterSingleClick();
                return;
            }

            this.showLoading();
            moveActions[0](selectedFolder.UniqueId)
                .done(() => this.closeWindow())
                .fail((exception: ExceptionDTO) => {
                    console.error(exception);
                    this.showNotificationException(this.uscNotificationId, exception);
                })
                .always(() => {
                    sender.enableAfterSingleClick();
                    this.hideLoading();
                });
        });
    }

    /**
     *------------------------- Methods -----------------------------
     */

    initialize() {
        this._btnConfirm = $find(this.btnConfirmId) as Telerik.Web.UI.RadButton;
        this._btnConfirm.add_clicked(this.btnConfirm_OnClick);
        this._rtvItemsToMove = $find(this.rtvItemsToMoveId) as Telerik.Web.UI.RadTreeView;
        this._loadingPanel = $find(this.ajaxLoadingPanelId) as Telerik.Web.UI.RadAjaxLoadingPanel;
        this._manager = $find(this.radWindowManagerId) as Telerik.Web.UI.RadWindowManager;
        this._ajaxManager = $find(this.ajaxManagerId) as Telerik.Web.UI.RadAjaxManager;

        try {
            let fascicleDocumentUnitConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascBase.FASCICLE_DOCUMENTUNIT_TYPE_NAME);
            this._fascicleDocumentUnitService = new FascicleDocumentUnitService(fascicleDocumentUnitConfiguration);

            let fascicleDocumentConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascBase.FASCICLE_DOCUMENT_TYPE_NAME);
            this._fascicleDocumentService = new FascicleDocumentService(fascicleDocumentConfiguration);

            let fascicleFolderConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascBase.FASCICLEFOLDER_TYPE_NAME);
            this._fascicleFolderService = new FascicleFolderService(fascicleFolderConfiguration);

            if (!this.isSessionStorageReaded()) {
                this.showWarningMessage(this.uscNotificationId, "Nessun elemento presente per l'attività corrente")
                return;
            }

            this.initializeTitleDescription();
            this.initializeItems();
            this.initializeExternalHandlers();
        } catch (error) {
            console.error(error);
            this.showNotificationMessage(this.uscNotificationId, "Errore in inizializzazione pagina");
        }
    }

    private initializeTitleDescription(): void {
        let description: string = "Documenti selezionati";
        if (this.itemsType == FascMoveItems.FOLDER_ITEMS_TYPE) {
            description = "Cartelle selezionate";
        }
        $(`#${this.lblItemSelectedDescriptionId}`).text(description);
    }

    private isSessionStorageReaded(): boolean {
        let selectedItems: string = sessionStorage.getItem(FascMoveItems.FASC_MOVE_ITEMS_Session_key);
        if (!selectedItems) {
            return false;
        }

        this._toMoveItems = JSON.parse(selectedItems);
        if (!this._toMoveItems || this._toMoveItems.length == 0) {
            return false;
        }

        return true;
    }

    private initializeItems(): void {
        let node: Telerik.Web.UI.RadTreeNode;
        for (let toMoveItem of this._toMoveItems) {
            node = new Telerik.Web.UI.RadTreeNode();
            node.set_text(toMoveItem.name);
            node.set_value(toMoveItem.uniqueId);

            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/folder_closed.png");
            if (this.itemsType == FascMoveItems.DOCUMENT_ITEMS_TYPE) {
                node.set_imageUrl(this.getIconByEnvironment(toMoveItem.environment));
            }
            this._rtvItemsToMove.get_nodes().add(node);
        }
    }

    private initializeExternalHandlers(): void {
        $(`#${this.uscFascicleFoldersId}`).bind(uscFascicleFolders.LOADED_EVENT, () => {
            this.loadFascicleFolders();
        });
        this.loadFascicleFolders();
    }

    loadFascicleFolders(): void {
        let uscFascicleFolder: uscFascicleFolders = <uscFascicleFolders>$(`#${this.uscFascicleFoldersId}`).data();
        if (!jQuery.isEmptyObject(uscFascicleFolder)) {
            uscFascicleFolder.setManageFascicleFolderVisibility(true);
            uscFascicleFolder.setRootNode(this.idFascicle, "Cartelle del fascicolo");
            uscFascicleFolder.loadFolders(this.idFascicle);
        }
    }

    private getIconByEnvironment(env: Environment): string {
        switch (env) {
            case Environment.Protocol:
                return "../Comm/Images/DocSuite/Protocollo16.gif";
            case Environment.Resolution:
                return "../Comm/Images/DocSuite/Atti16.gif";
            case Environment.DocumentSeries:
                return "../App_Themes/DocSuite2008/imgset16/document_copies.png";
            case Environment.UDS:
                return "../App_Themes/DocSuite2008/imgset16/document_copies.png"
            default:
                return "../App_Themes/DocSuite2008/imgset16/document.png";
        }
    }

    private moveFolders(folderId: string): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let deferredActions: JQueryPromise<void>[] = [];
        for (let toMoveItem of this._toMoveItems) {
            const deferredMoveAction = () => {
                let promise = $.Deferred<void>();
                this._fascicleFolderService.getById(toMoveItem.uniqueId,
                    (data: any) => {
                        if (!data) {
                            promise.reject("La cartella specificata non esiste. Riprovare.");
                            return;
                        }

                        let model: FascicleFolderModel = data as FascicleFolderModel;
                        model.ParentInsertId = folderId;
                        this._fascicleFolderService.updateFascicleFolder(model, UpdateActionType.FascicleMoveToFolder,
                            (data: any) => promise.resolve(),
                            (exception: ExceptionDTO) => promise.reject(exception)
                        )
                    },
                    (exception: ExceptionDTO) => promise.reject(exception)
                )
                return promise.promise();
            }
            deferredActions.push(deferredMoveAction());
        }

        $.when.apply(null, deferredActions)
            .done(() => promise.resolve())
            .fail((exception: ExceptionDTO) => promise.reject(exception));
        return promise.promise();
    }

    private moveDocuments(folderId: string): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let deferredActions: JQueryPromise<void>[] = [];
        try {
            for (let toMoveItem of this._toMoveItems.filter((item: FascicleMoveItemViewModel) => item.environment != Environment.Document)) {
                const deferredMoveAction = () => {
                    let promise = $.Deferred<void>();
                    this._fascicleDocumentUnitService.getByDocumentUnitAndFascicle(toMoveItem.uniqueId, this.idFascicle,
                        (data: any) => {
                            data.FascicleFolder = {} as FascicleFolderModel;
                            data.FascicleFolder.UniqueId = folderId;
                            this._fascicleDocumentUnitService.updateFascicleUD(data, UpdateActionType.FascicleMoveToFolder, (data: any) => promise.resolve(),
                                (exception: ExceptionDTO) => promise.reject(exception));
                        },
                        (exception: ExceptionDTO) => promise.reject(exception));
                    return promise.promise();
                };
                deferredActions.push(deferredMoveAction());
            }

            $.when.apply(null, deferredActions)
                .done(() => {
                    this._fascicleDocumentService.getByFolder(this.idFascicle, folderId,
                        (data: any) => {
                            let idArchiveChain: string;
                            if (data && data.length > 0) {
                                idArchiveChain = data[0].IdArchiveChain;
                            }
                            this._miscellaneaDeferreds.push(promise);
                            this.moveMiscellaneaDocuments(idArchiveChain);
                        },
                        (exception: ExceptionDTO) => {
                            promise.reject(exception);
                        });
                })
                .fail((exception: ExceptionDTO) => promise.reject(exception));
        } catch (error) {
            console.error(error);
            this.showNotificationMessage(this.uscNotificationId, "E' avvenuto un errore durante il processo di sposta documenti");
        }
        return promise.promise();
    }

    private moveMiscellaneaDocuments(folderChainId: string): void {
        let request: AjaxModel = <AjaxModel>{};
        request.ActionName = FascMoveItems.MOVE_MISCELLANEA_DOCUMENT_AJAX_ACTION_NAME;
        request.Value = [];
        let idDocuments: string[] = this._toMoveItems.filter((item: FascicleMoveItemViewModel) => item.environment == Environment.Document)
            .map((item: FascicleMoveItemViewModel) => item.uniqueId);
        request.Value.push(JSON.stringify(idDocuments));
        request.Value.push(folderChainId);

        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._ajaxManager.ajaxRequest(JSON.stringify(request));
    }

    moveMiscellaneaDocumentsCallback = (errorMessage: string, idArchiveChain: string, toCreateChain: boolean): void => {
        if (errorMessage) {
            this._miscellaneaDeferreds.forEach((promise: JQueryDeferred<void>) => {
                let errorDto: ExceptionDTO = new ExceptionDTO();
                errorDto.statusText = errorMessage;
                promise.reject(errorDto);
            });
            return;
        }

        let deferredAction = () => $.Deferred<void>().resolve().promise();
        if (toCreateChain && (idArchiveChain && idArchiveChain != Guid.empty)) {
            deferredAction = () => this.addMiscellaneaChain(idArchiveChain);
        }

        deferredAction()
            .done(() => {
                this.updateCurrentMiscellaneaChain()
                    .done(() => this._miscellaneaDeferreds.forEach((promise: JQueryDeferred<void>) => promise.resolve()))
                    .fail((exception: ExceptionDTO) => this._miscellaneaDeferreds.forEach((promise: JQueryDeferred<void>) => promise.reject(exception)))
            })
            .fail((exception: ExceptionDTO) => this._miscellaneaDeferreds.forEach((promise: JQueryDeferred<void>) => promise.reject(exception)));
    }

    private updateCurrentMiscellaneaChain(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        this._fascicleDocumentService.getByFolder(this.idFascicle, this.idFascicleFolder,
            (data: any) => {
                if (!data || data.length == 0) {
                    promise.resolve();
                    return;
                }

                this._fascicleDocumentService.updateFascicleDocument(data[0],
                    (data: any) => promise.resolve(),
                    (exception: ExceptionDTO) => promise.reject(exception)
                );
            },
            (exception: ExceptionDTO) => {
                promise.reject(exception);
            }
        )
        return promise.promise();
    }

    private addMiscellaneaChain(idArchiveChain: string): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        let fascicleDocumentModel: FascicleDocumentModel = {} as FascicleDocumentModel;
        fascicleDocumentModel.ChainType = ChainType.Miscellanea;
        fascicleDocumentModel.IdArchiveChain = idArchiveChain;
        fascicleDocumentModel.Fascicle = new FascicleModel();
        fascicleDocumentModel.Fascicle.UniqueId = this.idFascicle;

        let uscFascicleFolder: uscFascicleFolders = <uscFascicleFolders>$(`#${this.uscFascicleFoldersId}`).data();
        let selectedFolder: FascicleSummaryFolderViewModel = uscFascicleFolder.getSelectedFascicleFolder(this.idFascicle);
        fascicleDocumentModel.FascicleFolder = <FascicleFolderModel>{};
        fascicleDocumentModel.FascicleFolder.UniqueId = selectedFolder.UniqueId;

        this._fascicleDocumentService.insertFascicleDocument(fascicleDocumentModel,
            (data: any) => promise.resolve(),
            (exception: ExceptionDTO) => promise.reject(exception));
        return promise.promise();
    }

    protected closeWindow(): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(true);
    }

    /**
    * Recupera una RadWindow dalla pagina
    */
    protected getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }

    showLoading(): void {
        this._loadingPanel.show(this.pnlPageId);
    }

    hideLoading(): void {
        this._loadingPanel.hide(this.pnlPageId);
    }
}

export = FascMoveItems;