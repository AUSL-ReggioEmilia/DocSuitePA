/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />

import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import uscFascSummary = require("./uscFascSummary");
import FascicleService = require("App/Services/Fascicles/FascicleService");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import FascicleModel = require("App/Models/Fascicles/FascicleModel");
import uscErrorNotification = require("./uscErrorNotification");
import UscFascicleFolders = require('UserControl/uscFascicleFolders');
import PageClassHelper = require('App/Helpers/PageClassHelper');
import uscFascicolo = require("./uscFascicolo");
import FascicleSummaryFolderViewModel = require("App/ViewModels/Fascicles/FascicleSummaryFolderViewModel");
import uscFascicleFolders = require("UserControl/uscFascicleFolders");
import AjaxModel = require("App/Models/AjaxModel");
import SessionStorageKeysHelper = require("App/Helpers/SessionStorageKeysHelper");
import DSWEnvironmentType = require("App/Models/Workflows/WorkflowDSWEnvironmentType");

class uscFascicleSearch {
    public btnSearchId: string;
    public uscFascicleSummaryId: string;
    public managerWindowsId: string;
    public searchWindowId: string;
    public ajaxFlatLoadingPanelId: string;
    public finderContentId: string;
    public uscNotificationId: string;
    public summaryContentId: string;
    public fascFoldersContentId: string;
    public pageId: string;
    public defaultCategoryId: string;
    public fascicleObject: string;
    public categoryFullIncrementalPath: string;
    public uscFascFoldersId: string;
    public fascFoldersPaneId: string;
    public ajaxLoadingPanelId: string;
    public fascDetailsPaneId: string;
    public folderSelectionEnabled: boolean;
    public btnSearchByCategoryId: string;
    public btnSearchBySubjectId: string;
    public btnSearchByMetadataId: string;
    public rddlSelectMetadataId: string;
    public metadataContainerId: string;
    public ajaxManagerId: string;
    public dswEnvironment: number;

    private readonly _serviceConfigurations: ServiceConfiguration[];
    private _btnSearch: Telerik.Web.UI.RadButton;
    private _uscFascicleSummary: uscFascSummary;
    private _fascicleService: FascicleService;
    private _flatLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _sessionStorageFascicleKey: string;
    private _uscFascFolders: uscFascicleFolders;
    private _btnSearchByCategory: Telerik.Web.UI.RadButton;
    private _btnSearchBySubject: Telerik.Web.UI.RadButton;
    private _btnSearchByMetadata: Telerik.Web.UI.RadButton;
    private _rddlSelectMetadata: Telerik.Web.UI.RadDropDownList;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;

    private summaryContentPanel(): JQuery {
        return $(`#${this.summaryContentId}`);
    }

    private fascFoldersPanel(): JQuery {
        return $(`#${this.fascFoldersContentId}`);
    }

    private static LOADED_EVENT: string = "onLoaded";
    private static FASCICLE_SELECTED_EVENT: string = "onFascicleSelected";
    private static FOLDERTREE_ROOTNODE_TEXT = "Cartelle del fascicolo";
    private static SEARCH_BY_CATEGORY_ACTION: string = "searchByCategory";
    private static SEARCH_BY_SUBJECT_ACTION: string = "searchBySubject";
    private static SEARCH_BY_MEADATA_VALUE_ACTION: string = "searchByMetadataValue";

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    /**
     *------------------------- Events -----------------------------
     */

    btnSearch_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        let url: string = `../Fasc/FascRicerca.aspx?Type=Fasc&Action=SearchFascicles&DefaultCategoryId=${this.defaultCategoryId}&BackButtonEnabled=True`;
        this.openWindow(url, "searchFascicles", 900, 600, this.closeSearchFasciclesWindow);
    }

    closeSearchFasciclesWindow = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument()) {
            try {
                let fascicleId: string = args.get_argument();
                sessionStorage.removeItem(this._sessionStorageFascicleKey);
                this._showSearchButtonsLoader();
                this._loadingPanel.show(this.fascDetailsPaneId);
                $.when(this.loadFascicle(fascicleId, true), this._loadFascFoldersData(fascicleId))
                    .done(() => {
                        this.summaryContentPanel().show();
                        this.fascFoldersPanel().show();
                    })
                    .fail((exception: any) => {
                        this.showNotificationError(exception);
                    }).always(() => {
                        this._hideSearchButtonsLoader();
                        this._loadingPanel.hide(this.fascDetailsPaneId);
                    });
            }
            catch (error) {
                console.error(JSON.stringify(error));
                this.showNotificationError("Errore nella richiesta. Nessun fascicolo selezionato.")
            }
        }
    }

    /**
     *------------------------- Methods -----------------------------
     */

    initialize(): void {
        this._flatLoadingPanel = $find(this.ajaxFlatLoadingPanelId) as Telerik.Web.UI.RadAjaxLoadingPanel;
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._btnSearch = $find(this.btnSearchId) as Telerik.Web.UI.RadButton;
        this._btnSearch.add_clicked(this.btnSearch_OnClick);

        if (!this.categoryFullIncrementalPath) {
            $(`#${this.btnSearchByCategoryId}`).hide();
        }
        else {
            this._btnSearchByCategory = <Telerik.Web.UI.RadButton>$find(this.btnSearchByCategoryId);
            this._btnSearchByCategory.add_clicked(this.btnSearchByCategory_OnClick);
        }

        if (!this.fascicleObject) {
            $(`#${this.btnSearchBySubjectId}`).hide();
        }
        else {
            this._btnSearchBySubject = <Telerik.Web.UI.RadButton>$find(this.btnSearchBySubjectId);
            this._btnSearchBySubject.add_clicked(this.btnSearchBySubject_OnClick);
        }

        if (sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_CURRENT_UDS_METADATAS) && this.dswEnvironment == DSWEnvironmentType.UDS) {
            $(`#${this.btnSearchByMetadataId}`).show();
            this._btnSearchByMetadata = <Telerik.Web.UI.RadButton>$find(this.btnSearchByMetadataId);
            this._btnSearchByMetadata.add_clicked(this.btnSearchByMetadata_OnClick);
        }

        this._sessionStorageFascicleKey = `${this.pageId}_selectedFascicle`;
        sessionStorage.removeItem(this._sessionStorageFascicleKey);

        let fascicleServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Fascicle");
        this._fascicleService = new FascicleService(fascicleServiceConfiguration);

        this._uscFascFolders = $(`#${this.uscFascFoldersId}`).data() as uscFascicleFolders;

        this.bindLoaded();
    }

    public _loadFascFoldersData = (fascicleId: string): JQueryPromise<void> => {
        let promise: JQueryDeferred<void> = $.Deferred<void>();

        if (!this.folderSelectionEnabled) {
            return promise.resolve();
        }

        PageClassHelper.callUserControlFunctionSafe<UscFascicleFolders>(this.uscFascFoldersId)
            .done((instance) => {
                instance.loadFolders(fascicleId)
                    .done(() => {
                        let fascFoldersTreeRootNode: Telerik.Web.UI.RadTreeNode = instance.getFascicleFolderTree().get_nodes().getNode(0);
                        fascFoldersTreeRootNode.get_attributes().setAttribute("selectable", !this.folderSelectionEnabled);
                        instance.selectFascicleNode(false);
                        instance.setRootNode(fascicleId, uscFascicleSearch.FOLDERTREE_ROOTNODE_TEXT, !this.folderSelectionEnabled);

                        if (sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_FASCICE_ID) == fascicleId &&
                            sessionStorage.getItem(instance.SESSION_FascicleHierarchy) &&
                            sessionStorage.getItem(instance.SESSION_FascicleHierarchy) != "[]") {
                            instance.rebuildTreeFromSession(fascicleId)
                                .done(() => promise.resolve())
                                .fail((exception: ExceptionDTO) => promise.reject(exception));
                        } else {
                            sessionStorage.removeItem(instance.SESSION_FascicleHierarchy);
                            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_FASCICE_ID, fascicleId);
                            promise.resolve();
                        }
                    })
                    .fail((exception: ExceptionDTO) => promise.reject(exception));
            });
        return promise.promise();
    }

    public getSelectedFascicleFolder(): FascicleSummaryFolderViewModel {
        let selectedFascicle: FascicleModel = this.getSelectedFascicle();
        return this._uscFascFolders.isVisible && selectedFascicle ? this._uscFascFolders.getSelectedFascicleFolder(selectedFascicle.UniqueId) : undefined;
    }

    private bindLoaded(): void {
        $(`#${this.pageId}`).data(this);
        $(`#${this.pageId}`).triggerHandler(uscFascicleSearch.LOADED_EVENT);
    }
    
    clearSelections(): void {
        sessionStorage.removeItem(this._sessionStorageFascicleKey);
        this.summaryContentPanel().hide();
        this.fascFoldersPanel().hide();
    }

    showContentPanel(): void {
        this.summaryContentPanel().show();
        this.fascFoldersPanel().show();
    }

    loadFascicle(fascicleId: string, cacheSelectedFascicle: boolean = false): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        if (!fascicleId) {
            return promise.reject("Nessun id fascicolo definito per la ricerca");
        }

        this._fascicleService.getFascicle(fascicleId,
            (data: any) => {
                if (!data) {
                    return promise.reject(`Nessun fascicolo trovato con id ${fascicleId}`);
                }

                this._uscFascicleSummary = $(`#${this.uscFascicleSummaryId}`).data() as uscFascSummary;
                if (jQuery.isEmptyObject(this._uscFascicleSummary)) {
                    return promise.reject(`E' avvenuto un errore durante il carimento delle informazioni del fascicolo selezionato. Si prega di riprovare.`);
                }

                if (cacheSelectedFascicle) {
                    sessionStorage.setItem(this._sessionStorageFascicleKey, JSON.stringify(data));
                }

                $(`#${this.pageId}`).triggerHandler(uscFascicleSearch.FASCICLE_SELECTED_EVENT, data);
                this._uscFascicleSummary.loadData(data as FascicleModel)
                    .done(() => promise.resolve())
                    .fail((exception) => promise.reject(exception));
            },
            (exception: ExceptionDTO) => {
                promise.reject(exception);
            }
        )
        return promise.promise();
    }

    getSelectedFascicle(): FascicleModel {
        if (sessionStorage[this._sessionStorageFascicleKey]) {
            return JSON.parse(sessionStorage[this._sessionStorageFascicleKey]) as FascicleModel;
        }
        return undefined;
    }

    setButtonSearchEnabled(value: boolean): void {
        this._btnSearch.set_enabled(value);
    }

    btnSearchByCategory_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.sendAjaxRequest(uscFascicleSearch.SEARCH_BY_CATEGORY_ACTION, [this.categoryFullIncrementalPath]);
    }

    btnSearchBySubject_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.sendAjaxRequest(uscFascicleSearch.SEARCH_BY_SUBJECT_ACTION, [this.fascicleObject]);
    }

    btnSearchByMetadata_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        $(`#${this.metadataContainerId}`).show();
        this._rddlSelectMetadata = <Telerik.Web.UI.RadDropDownList>$find(this.rddlSelectMetadataId);
        let metadatas: any = JSON.parse(sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_CURRENT_UDS_METADATAS));

        if (this._rddlSelectMetadata) {
            this.populateMetadataDropdown(metadatas, this._rddlSelectMetadata);
            this._rddlSelectMetadata.add_selectedIndexChanged(this.rddlSelectMetadata_OnSelectedIndexChanged);
        }
    }

    rddlSelectMetadata_OnSelectedIndexChanged = (sender: Telerik.Web.UI.RadDropDownList, eventArgs: Telerik.Web.UI.DropDownListIndexChangedEventArgs) => {
        if (sender.get_selectedItem()) {
            this.sendAjaxRequest(uscFascicleSearch.SEARCH_BY_MEADATA_VALUE_ACTION, [sender.get_selectedItem().get_value()]);
        }
    }

    private populateMetadataDropdown(metadatas: any, rddlSelectMetadata: Telerik.Web.UI.RadDropDownList) {
        let item: Telerik.Web.UI.DropDownListItem;
        for (let metadata of metadatas) {
            item = new Telerik.Web.UI.DropDownListItem();
            item.set_text(metadata.KeyName);
            item.set_value(JSON.stringify(metadata));

            rddlSelectMetadata.get_items().add(item);
        }
        rddlSelectMetadata.trackChanges();
    }

    private sendAjaxRequest(actionName: string, ajaxValues: string[]): void {
        let ajaxModel: AjaxModel = <AjaxModel>{
            ActionName: actionName,
            Value: ajaxValues
        };
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
    }

    openWindowCallback(url: string, windowName: string) {
        this.openWindow(url, windowName, 900, 600, this.closeSearchFasciclesWindow);
    }

    private openWindow(url, name, width, height, closeHandler: (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => void): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = $find(this.managerWindowsId) as Telerik.Web.UI.RadWindowManager;
        let wnd: Telerik.Web.UI.RadWindow = manager.open(url, name, null);
        wnd.setSize(width, height);
        wnd.set_modal(true);
        wnd.remove_close(closeHandler);
        wnd.add_close(closeHandler);
        wnd.center();
        return false;
    }

    private showNotificationError(exception: string)
    private showNotificationError(exception: ExceptionDTO)
    private showNotificationError(exception: any) {
        let uscNotification: uscErrorNotification = $(`#${this.uscNotificationId}`).data() as uscErrorNotification;
        if (!jQuery.isEmptyObject(uscNotification)) {
            if (exception instanceof ExceptionDTO) {
                uscNotification.showNotification(exception);
            }
            else {
                uscNotification.showNotificationMessage(exception);
            }
        }
    }

    private _showSearchButtonsLoader(): void {
        if (this._btnSearch) {
            this._flatLoadingPanel.show(this.btnSearchId);
        }
        if (this._btnSearchByCategory) {
            this._flatLoadingPanel.show(this.btnSearchByCategoryId);
        }
        if (this._btnSearchBySubject) {
            this._flatLoadingPanel.show(this.btnSearchBySubjectId);
        }
    }

    private _hideSearchButtonsLoader(): void {
        if (this._btnSearch) {
            this._flatLoadingPanel.hide(this.btnSearchId);
        }
        if (this._btnSearchByCategory) {
            this._flatLoadingPanel.hide(this.btnSearchByCategoryId);
        }
        if (this._btnSearchBySubject) {
            this._flatLoadingPanel.hide(this.btnSearchBySubjectId);
        }
    }
}

export = uscFascicleSearch;