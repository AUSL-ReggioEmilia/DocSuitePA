/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import DocumentUnitModel = require('App/Models/DocumentUnits/DocumentUnitModel');
import Environment = require('App/Models/Environment');
import FascicleService = require('App/Services/Fascicles/FascicleService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import FascicleType = require('App/Models/Fascicles/FascicleType');
import DomainUserService = require('App/Services/Securities/DomainUserService');
import DomainUserModel = require('App/Models/Securities/DomainUserModel');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import LinkedFasciclesViewModel = require('App/ViewModels/Fascicles/LinkedFasciclesViewModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import WorkflowPropertyHelper = require('App/Models/Workflows/WorkflowPropertyHelper');
import WorkflowActivityService = require('App/Services/Workflows/WorkflowActivityService');
import WorkflowActivityModel = require('App/Models/Workflows/WorkflowActivityModel');
import RoleModel = require('App/Models/Commons/RoleModel');
import WorkflowRoleModel = require('App/Models/Workflows/WorkflowRoleModel');
import FascicleRoleModel = require('App/Models/Fascicles/FascicleRoleModel');
import WorkflowAuthorization = require('App/Models/Workflows/WorkflowAuthorizationModel');
import FiltersGridUDFasciclesViewModelMapper = require('App/Mappers/Fascicles/FiltersGridUDFasciclesViewModelMapper');
import FiltersGridUDFasciclesGridViewModel = require('App/ViewModels/Fascicles/FiltersGridUDFasciclesViewModel');
import AjaxModel = require('App/Models/AjaxModel');
import UscFascSummary = require('UserControl/uscFascSummary');
import UscFascicleFolders = require('UserControl/uscFascicleFolders');
import FascicleSummaryFolderViewModel = require('App/ViewModels/Fascicles/FascicleSummaryFolderViewModel');
import VisibilityType = require('App/Models/Fascicles/VisibilityType');
import PageClassHelper = require('App/Helpers/PageClassHelper');
import uscCustomActionsRest = require('UserControl/uscCustomActionsRest');
import FascicleCustomActionModel = require('App/Models/Commons/FascicleCustomActionModel');
import CustomActionsIconModel = require('App/Models/Commons/CustomActionsIconModel');
import uscRoleRest = require('./uscRoleRest');
import UscRoleRestEventType = require('App/Models/Commons/UscRoleRestEventType');
import AuthorizationRoleType = require('App/Models/Commons/AuthorizationRoleType');
import UscDynamicMetadataSummaryRest = require('UserControl/uscDynamicMetadataSummaryRest');
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');
import FascicleDocumentUnitModel = require('../App/Models/Fascicles/FascicleDocumentUnitModel');

class uscFascicolo {
    clientId: string;
    ajaxLoadingPanelId: string;
    rcbUDId: string;
    pnlUDId: string;
    rcbReferenceTypeId: string;
    rowManagerId: string;
    txtTitleId: string;
    grdUDId: string;
    isEditPage: boolean;
    isAuthorizePage: boolean;
    pageId: string;
    ajaxManagerId: string;
    btnExpandUDFascicleId: string;
    UDFascicleGridId: string;
    deliberaCaption: string;
    determinaCaption: string;
    rowRolesId: string;
    uscNotificationId: string;
    fasciclesPanelVisibilities: { [key: string]: boolean };
    workflowActivityId: string;
    lblWorkflowHandlerUserId: string;
    pnlGrdSearchId: string;
    metadataRepositoryEnabled: boolean;
    btnExpandDynamicMetadataId: string;
    dynamicMetadataContentId: string;
    rowDynamicMetadataId: string;
    currentFascicleId: string;
    uscFascSummaryId: string;
    rwmDocPreviewId: string;
    uscFascFoldersId: string;
    lblUDGridTitleId: string;
    rsPnlFoldersId: string;
    rszFolderId: string;
    uscRoleId: string;
    splitterId: string;
    racUDDataSourceId: string;
    rowCustomActionsId: string;
    uscCustomActionsRestId: string;
    fascicleAutoCloseThresholdDays: number;
    fascicleAutoCloseWarningThresholdDays: number;
    fascSummaryColumnId: string;
    customActionsColumnId: string;
    uscDynamicMetadataSummaryRestId: string;

    public static LOADED_EVENT: string = "onLoaded";
    public static DATA_LOADED_EVENT: string = "onDataLoaded";
    public static GRID_REFRESH_EVENT: string = "onGridRefresh";
    public static REBIND_EVENT: string = "onRebind";
    public static masterRole: RoleModel;

    private static ACTIONS_COL_SPAN: string = "t-col-4";
    private static SUMMARY_COL_SPAN: string = "t-col-8";
    private static FULLWIDTH_COL_SPAN: string = "t-col-12";

    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _notification: Telerik.Web.UI.RadNotification;
    private _grid: Telerik.Web.UI.RadGrid;
    private _rcbUd: Telerik.Web.UI.RadComboBox;
    private _rcbReferenceType: Telerik.Web.UI.RadComboBox;
    private _txtTitleUD: Telerik.Web.UI.RadTextBox;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _btnExpandDynamicMetadata: Telerik.Web.UI.RadButton;
    private _btnExpandUDFascicle: Telerik.Web.UI.RadButton;
    private _racUDDataSource: Telerik.Web.UI.RadClientDataSource;

    private _UDFascicleGrid: JQuery;
    private _service: FascicleService;
    private _isLinkedFasciclesGridOpen: boolean;
    private _isUDFascicleGridOpen: boolean;
    private _isDynamicMetadataContentOpen: boolean;
    private _isCustomActionsContentOpen: boolean;
    private _domainUserService: DomainUserService;
    private _serviceConfigurations: ServiceConfiguration[];
    private _uscNotification: UscErrorNotification
    private _lblWorkflowHandlerUser: JQuery;
    private _workflowActivityService: WorkflowActivityService;
    private _workflowActivity: WorkflowActivityModel;
    private _pnlGridSearch: JQuery;
    private _dynamicMetadataContent: JQuery;
    private _expandedFoldersPanel: boolean = true;
    private _notFireEvent: boolean = false;
    private _fascicleVisibilityType: VisibilityType;
    private _rolesAddedIds: number[];
    private _rolesRemovedIds: number[] = [];


    /**
     * Costruttore
     * @param webApiConfiguration
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._service = new FascicleService(ServiceConfigurationHelper.getService(serviceConfigurations, "Fascicle"));
        this._serviceConfigurations = serviceConfigurations;
    }

    /**
     * Inizializzazione
     */
    initialize() {
        this._UDFascicleGrid = $(`#${this.UDFascicleGridId}`);
        this._dynamicMetadataContent = $(`#${this.dynamicMetadataContentId}`);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._txtTitleUD = <Telerik.Web.UI.RadTextBox>$find(this.txtTitleId);
        this._txtTitleUD.add_keyPress(this.txtTitle_OnKeyPressed);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._rcbUd = <Telerik.Web.UI.RadComboBox>$find(this.rcbUDId);
        this._rcbReferenceType = <Telerik.Web.UI.RadComboBox>$find(this.rcbReferenceTypeId);
        this._rcbReferenceType.add_selectedIndexChanged(this.rcbReferenceType_OnSelectedIndexChanged);
        this._rcbUd.add_selectedIndexChanged(this.rcbUd_OnSelectedIndexChanged);
        this._grid = <Telerik.Web.UI.RadGrid>$find(this.grdUDId);
        this._grid.get_masterTableView().hideFilterItem();
        this._btnExpandUDFascicle = <Telerik.Web.UI.RadButton>$find(this.btnExpandUDFascicleId);
        this._btnExpandDynamicMetadata = <Telerik.Web.UI.RadButton>$find(this.btnExpandDynamicMetadataId);
        this._racUDDataSource = $find(this.racUDDataSourceId) as Telerik.Web.UI.RadClientDataSource;
        this._btnExpandUDFascicle.addCssClass("dsw-arrow-down");
        this._UDFascicleGrid.show();
        this._isLinkedFasciclesGridOpen = false;
        this._isUDFascicleGridOpen = true;
        this._btnExpandDynamicMetadata.add_clicking(this.btnExpandDynamicMetadata_OnClick);
        $(`#${this.rowDynamicMetadataId}`).hide();
        this._btnExpandDynamicMetadata.addCssClass("dsw-arrow-down");
        this._isDynamicMetadataContentOpen = true;
        this._dynamicMetadataContent.show();
        this._btnExpandUDFascicle.add_clicking(this.btnExpandUDFascicle_OnClick);
        this._lblWorkflowHandlerUser = $(`#${this.lblWorkflowHandlerUserId}`);
        this._pnlGridSearch = $(`#${this.pnlGrdSearchId}`);

        this._isCustomActionsContentOpen = true;

        let domainUserConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DomainUserModel");
        this._domainUserService = new DomainUserService(domainUserConfiguration);

        let workflowActivityConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowActivity');
        this._workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);

        $(`#${this.rowRolesId}`).hide();
        if (this.isEditPage) {
            $(`#${this.rowManagerId}`).hide();
            $(`#${this.rowRolesId}`).hide();
            $(`#${this.pnlUDId}`).hide();

        }
        if (this.isAuthorizePage) {
            $(`#${this.rowManagerId}`).hide();
            $(`#${this.rowRolesId}`).hide();
            $(`#${this.rowRolesId}`).show();
            $(`#${this.pnlUDId}`).hide();

            this.registerUscRoleRestEventHandlers();
            PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleId).done((instance) => {
                instance.setToolbarVisibility(true);
                instance.renderRolesTree([]);
            });
        }

        if (!this.isEditPage && !(this.fasciclesPanelVisibilities["GridSearchPanelVisibility"])) {
            this._pnlGridSearch.hide();
            this._grid.get_masterTableView().showFilterItem();
        }

        $(`#${this.uscFascFoldersId}`).bind(UscFascicleFolders.RESIZE_EVENT, (args) => {
            this.expandCollapseFolders();
            this._expandedFoldersPanel = false;
        });

        $(".rspTabsContainer").bind("click", () => {
            if (this._expandedFoldersPanel) {
                this.expandCollapseFolders();
                this._expandedFoldersPanel = false;
                return;
            }
            this._expandedFoldersPanel = true;
        });

        this.initializeUDFilterDataSource();
        this.bindLoaded();

        $(`#${this.uscFascFoldersId}`).on(UscFascicleFolders.REFRESH_GRID_EVENT, () => {
            this.refreshGridUD([], []);
        });

        $(`#${this.uscFascFoldersId}`).on(UscFascicleFolders.REFRESH_GRID_UPLOAD_DOCUMENTS, (evt) => {
            this._loadingPanel.show(this.grdUDId);
            $(`#${this.pageId}`).triggerHandler(uscFascicolo.REBIND_EVENT);
        });
    }

    /**
     *------------------------- Events -----------------------------
     */

    /**
     * Evento al click del pulsante per la visualizzazione dell'UD
     * @param sender
     * @param args
     */
    btnUDLink_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        this._loadingPanel.show(this.grdUDId);
        let uniqueId: string = $(sender.get_element()).attr("UniqueId");
        let incremental: number = Number($(sender.get_element()).attr("EntityId"));
        let environment: number = Number($(sender.get_element()).attr("Environment"));
        let year: number = Number($(sender.get_element()).attr("Year"));
        let number: number = Number($(sender.get_element()).attr("Number"));

        let url: string = "";
        switch (<Environment>environment) {
            case Environment.Protocol:
                url = `../Prot/ProtVisualizza.aspx?FromFascicle=true&UniqueId=${uniqueId}&IdFascicle=${this.currentFascicleId}&Type=Prot`;
                break;
            case Environment.Resolution:
                url = `../Resl/ReslVisualizza.aspx?FromFascicle=true&IdResolution=${incremental}&Type=Resl&IdFascicle=${this.currentFascicleId}`;
                break;
            case Environment.DocumentSeries:
                url = `../Series/Item.aspx?FromFascicle=true&Type=Series&IdDocumentSeriesItem=${incremental}&Action=View&Type=Series&IdFascicle=${this.currentFascicleId}`;
                break;
            case Environment.UDS:
                url = `../UDS/UDSView.aspx?Type=UDS&FromFascicle=true&IdUDS=${uniqueId}&IdUDSRepository=${$(sender.get_element()).attr("UDSRepositoryId")}&IdFascicle=${this.currentFascicleId}`;
                break;
            default:
                let serializedDoc: string = $(sender.get_element()).attr("SerializedDoc");
                this.openPreviewWindow(serializedDoc);
                this._loadingPanel.hide(this.grdUDId);
                return;
        }

        window.location.href = url;
    }

    /**
     * Evento al click del pulsante per la visualizzazione del fascicolo dove l'UD è fascicolata
     */
    imgUDFascicle_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        this._loadingPanel.show(this.grdUDId);
        let uniqueId: string = $(sender.get_element()).attr("IdFascicle");
        let url: string = `../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=${uniqueId.toString()}`;
        window.location.href = url;
    }

    /**
    * Evento al click del pulsante per la espandere o comprimere la gliglia dei fascicoli collegati
    */
    btnExpandLinkedFascicles_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        if (this._isLinkedFasciclesGridOpen) {
            this._isLinkedFasciclesGridOpen = false;
        }
        else {
            this._isLinkedFasciclesGridOpen = true;
        }

    }


    /**
    * Evento al click del pulsante per la espandere o comprimere la gliglia delle UD presenti nel fascicolo
    */
    btnExpandUDFascicle_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        if (this._isUDFascicleGridOpen) {
            this._UDFascicleGrid.hide();
            this._isUDFascicleGridOpen = false;
            this._btnExpandUDFascicle.removeCssClass("dsw-arrow-down");
            this._btnExpandUDFascicle.addCssClass("dsw-arrow-up");
        }
        else {
            this._UDFascicleGrid.show();
            this._isUDFascicleGridOpen = true;
            this._btnExpandUDFascicle.removeCssClass("dsw-arrow-up");
            this._btnExpandUDFascicle.addCssClass("dsw-arrow-down");
        }

    }

    /**
    * Evento al click del pulsante per espandere o collassare il pannello dei metadati dinamici
    */
    btnExpandDynamicMetadata_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);
        if (this._isDynamicMetadataContentOpen) {
            this._dynamicMetadataContent.hide();
            this._isDynamicMetadataContentOpen = false;
            this._btnExpandDynamicMetadata.removeCssClass("dsw-arrow-down");
            this._btnExpandDynamicMetadata.addCssClass("dsw-arrow-up");
        }
        else {
            this._dynamicMetadataContent.show();
            this._isDynamicMetadataContentOpen = true;
            this._btnExpandDynamicMetadata.removeCssClass("dsw-arrow-up");
            this._btnExpandDynamicMetadata.addCssClass("dsw-arrow-down");
        }

    }

    /**
     * Evento scatenato dai comandi della griglia
     * @param sender
     * @param args
     */
    gridOnCommand = (sender: Telerik.Web.UI.RadGrid, args: Telerik.Web.UI.GridCommandEventArgs) => {
        args.set_cancel(true);
        if (args.get_commandName() == "Filter") {
            this._loadingPanel.show(this.grdUDId);
            $(`#${this.pageId}`).triggerHandler(uscFascicolo.REBIND_EVENT);
        }
        if (args.get_commandName() == "Sort") {
            this._loadingPanel.show(this.grdUDId);
            $(`#${this.pageId}`).triggerHandler(uscFascicolo.REBIND_EVENT);
        }
    }

    /**
     * Evento scatenato dal cambio di selezione del tipo di UD da visualizzare in griglia
     * @param sender
     * @param args
     */
    private rcbUd_OnSelectedIndexChanged = (sender: any, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        if (!this._notFireEvent) {
            this._grid = <Telerik.Web.UI.RadGrid>$find(this.grdUDId);
            if (!(this.fasciclesPanelVisibilities["GridSearchPanelVisibility"])) {
                if (sender.get_selectedItem().get_value() == null || sender.get_selectedItem().get_value() == "") {
                    this.removeSpecificFilter("DocumentUnitName");
                }
                else {
                    this._grid.get_masterTableView().filter("DocumentUnitName", sender.get_selectedItem().get_value(), "EqualTo", true);
                }
            }
            else {
                this._grid.get_masterTableView().filter("DocumentUnitName", this._rcbUd.get_selectedItem().get_value(), "EqualTo", true);
            }
        }
    }

    /**
     * Evento scatenato dal cambio di selezione del tipo di fascicolazione delle UD
     * @param sender
     * @param args
     */
    private rcbReferenceType_OnSelectedIndexChanged = (sender: any, args: Telerik.Web.UI.RadComboBoxEventArgs) => {
        this._grid = <Telerik.Web.UI.RadGrid>$find(this.grdUDId);
        if (!(this.fasciclesPanelVisibilities["GridSearchPanelVisibility"])) {
            if (sender.get_selectedItem().get_value() == null || sender.get_selectedItem().get_value() == "") {
                this.removeSpecificFilter("ReferenceType");
            }
            else {
                this._grid.get_masterTableView().filter("ReferenceType", sender.get_selectedItem().get_value(), "EqualTo", true);
            }
        }
        else {
            this._grid.get_masterTableView().filter("ReferenceType", this._rcbReferenceType.get_selectedItem().get_value(), "EqualTo", true);
        }
    }

    /**
     * Evento scatenato dall'input di valori o azioni nella TextBox del filtro
     * @param sender
     * @param args
     */
    private txtTitle_OnKeyPressed = (sender: any, args: Telerik.Web.UI.InputKeyPressEventArgs) => {
        if (args.get_keyCode() == 13) {
            args.set_cancel(true);
            setTimeout(() => {
                this._grid = <Telerik.Web.UI.RadGrid>$find(this.grdUDId);
                this._grid.get_masterTableView().filter("Title", (<any>this._txtTitleUD.get_element()).value, "Contains", true);
            }, 0);
        }
    }

    /**
 * Evento scatenato dall'input di valori o azioni nella TextBox del filtro
 * @param sender
 * @param args
 */
    private udSubject_OnKeyPressed = (sender: any, args: Telerik.Web.UI.InputKeyPressEventArgs) => {
        if (args.get_keyCode() == 13) {
            args.set_cancel(true);
            setTimeout(() => {
                this._grid = <Telerik.Web.UI.RadGrid>$find(this.grdUDId);
                if ((sender.get_element()).value == null || (sender.get_element()).value == "") {
                    this.removeSpecificFilter("Title");
                }
                else {
                    this._grid.get_masterTableView().filter("Title", (sender.get_element()).value, "Contains", true);
                }
            }, 0);
        }
    }

    /**
     *------------------------- Methods -----------------------------
     */

    /**
     * Ritorna un oggetto DocumentUnitFilterModel per i filtri da applicare alla griglia
     */
    //getFilterModel(): DocumentUnitFilterModel {
    //    let filterModel: DocumentUnitFilterModel = <DocumentUnitFilterModel>{};
    //    if (this._rcbUd.get_selectedItem() != null)
    //        filterModel.DocumentUnitName = this._rcbUd.get_selectedItem().get_value();

    //    if (this._rcbReferenceType.get_selectedItem() != null)
    //        filterModel.ReferenceType = FascicleReferenceType[this._rcbReferenceType.get_selectedItem().get_value()];

    //    filterModel.Title = (<any>this._txtTitleUD.get_element()).value;
    //    return filterModel;
    //}  

    private getFascSummaryColumn(): JQuery {
        return $(`#${this.fascSummaryColumnId}`);
    }

    private getCustomActionsColumn(): JQuery {
        return $(`#${this.customActionsColumnId}`);
    }

    initializeUDFilterDataSource(): void {
        let empty: any = { Name: '', Value: null };
        let protocol: any = { Name: 'Protocollo', Value: 'Protocollo' };
        let delibera: any = { Name: this.deliberaCaption, Value: this.deliberaCaption };
        let determina: any = { Name: this.determinaCaption, Value: this.determinaCaption };
        let archive: any = { Name: 'Archivio', Value: 'Archivio' };
        let miscellanea: any = { Name: 'Inserto', Value: 'Inserto' };

        let dataSource: any[] = [];
        dataSource.push(empty);
        dataSource.push(protocol);
        if (!String.isNullOrEmpty(this.deliberaCaption)) {
            dataSource.push(delibera);
        }
        if (!String.isNullOrEmpty(this.determinaCaption)) {
            dataSource.push(determina);
        }
        dataSource.push(archive);
        dataSource.push(miscellanea);

        dataSource = dataSource.sort((first, second) => {
            if (first.Name > second.Name) {
                return 1;
            }

            if (first.Name < second.Name) {
                return -1;
            }

            return 0;
        });

        let comboItem: Telerik.Web.UI.RadComboBoxItem;
        for (let dataItem of dataSource) {
            comboItem = new Telerik.Web.UI.RadComboBoxItem();
            comboItem.set_text(dataItem.Name);
            comboItem.set_value(dataItem.Value);
            this._rcbUd.get_items().add(comboItem);
        }

        (<any>this._racUDDataSource).set_data(dataSource);
        this._racUDDataSource.fetch(undefined);
    }

    /**
     * Ritorna la query string odata per i filtri
     */
    getFilterModel(): string {
        let filterQs: string = '';
        this._grid = <Telerik.Web.UI.RadGrid>$find(this.grdUDId);

        if (!(this.fasciclesPanelVisibilities["GridSearchPanelVisibility"])) {
            let filters: Telerik.Web.UI.GridFilterExpressions = this._grid.get_masterTableView().get_filterExpressions();
            let filtersCount: number = filters.get_count();
            if (filtersCount > 0) {
                filterQs = "$filter=";
                let currentIndex: number = 1;
                filters.forEach((filter) => {
                    if (filter.get_fieldValue() != "") {
                        if (filter.get_columnUniqueName() == "ReferenceType") {
                            filterQs = `${filterQs}ReferenceType eq VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles.ReferenceType'${filter.get_fieldValue()}'`;
                        }

                        if (filter.get_columnUniqueName() == "DocumentUnitName") {
                            if (filter.get_fieldValue() == "Archivio") {
                                filterQs = `${filterQs}DocumentUnit/Environment ge 100`;
                            } else {
                                filterQs = `${filterQs}DocumentUnit/DocumentUnitName eq '${filter.get_fieldValue()}'`;
                            }
                        }

                        if (filter.get_columnUniqueName() == "Title") {
                            filterQs = `${filterQs}contains(DocumentUnit/Subject, '${filter.get_fieldValue()}')`;
                        }

                        if (currentIndex < filtersCount) {
                            filterQs = `${filterQs} and `;
                        }
                    }
                    ++currentIndex;
                });
            }
        }
        else {
            filterQs = "$filter=";
            if (this._rcbUd.get_selectedItem() != null && !String.isNullOrEmpty(this._rcbUd.get_selectedItem().get_value())) {
                let selectedValue: string = this._rcbUd.get_selectedItem().get_value();
                if (selectedValue == "Archivio") {
                    filterQs = `${filterQs}DocumentUnit/Environment ge 100 and `;
                } else {
                    filterQs = `${filterQs}DocumentUnit/DocumentUnitName eq '${this._rcbUd.get_selectedItem().get_value()}' and `;
                }
            }

            if (this._rcbReferenceType.get_selectedItem() != null && this._rcbReferenceType.get_selectedItem().get_value() != "")
                filterQs = `${filterQs}ReferenceType eq VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles.ReferenceType'${this._rcbReferenceType.get_selectedItem().get_value()}' and `;

            filterQs = `${filterQs}contains(DocumentUnit/Subject, '${(<any>this._txtTitleUD.get_element()).value}')`;
        }
        let orders: Telerik.Web.UI.GridSortExpressions = this._grid.get_masterTableView().get_sortExpressions();
        let ordersCount: number = orders.get_count();
        if (ordersCount > 0) {
            filterQs = `${filterQs}&$orderby=`;
            let currentIndex: number = 1;
            orders.forEach((order) => {
                filterQs = `${filterQs}DocumentUnit/${order.FieldName}`;
                if (order.SortOrder === 2) {
                    filterQs = `${filterQs} desc`;
                }
                if (currentIndex < ordersCount) {
                    filterQs = `${filterQs}, `;
                }
                ++currentIndex;
            });
        }
        return filterQs;
    }

    /**
     * Scatena l'evento di "load completed" del controllo
     */
    private bindLoaded(): void {
        $(`#${this.pageId}`).data(this);
        $(`#${this.pageId}`).triggerHandler(uscFascicolo.LOADED_EVENT);
    }

    /**
     * Carica i dati dello user control
     */
    loadData(fascicle: FascicleModel): void {
        if (fascicle == null) return;
        this.loadFascFoldersData(fascicle)
            .done(() => {
                this.setSummaryData(fascicle);
                this.loadUscSummaryFascicle(fascicle);
            })
            .fail((exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    loadDataWithoutFolders(fascicle: FascicleModel): void {
        if (fascicle == null) return;
        this.setSummaryData(fascicle);
        this.loadUscSummaryFascicle(fascicle);
    }

    loadUscSummaryFascicle(fascicle: FascicleModel): void {
        let uscFascSummary: UscFascSummary = <UscFascSummary>$(`#${this.uscFascSummaryId}`).data();
        if (!jQuery.isEmptyObject(uscFascSummary)) {
            uscFascSummary.workflowActivityId = this.workflowActivityId
            uscFascSummary.loadData(fascicle);
        }
    }

    loadFascFoldersData(fascicle: FascicleModel): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        PageClassHelper.callUserControlFunctionSafe<UscFascicleFolders>(this.uscFascFoldersId)
            .done((instance) => {
                instance.setRootNode(fascicle.UniqueId);
                instance.loadFolders(fascicle.UniqueId)
                    .done(() => {
                        if (sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_FASCICE_ID) == fascicle.UniqueId &&
                            sessionStorage.getItem(instance.SESSION_FascicleHierarchy) &&
                            sessionStorage.getItem(instance.SESSION_FascicleHierarchy) != "[]") {
                            instance.rebuildTreeFromSession(this.currentFascicleId)
                                .done(() => promise.resolve())
                                .fail((exception: ExceptionDTO) => promise.reject(exception));
                        } else {
                            sessionStorage.removeItem(instance.SESSION_FascicleHierarchy);
                            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_FASCICE_ID, this.currentFascicleId);
                            promise.resolve();
                        }
                    })
                    .fail((exception: ExceptionDTO) => promise.reject(exception));
            });
        return promise.promise();
    }

    /**
     * Imposta i dati nel sommario
     * @param fascicle
     */
    setSummaryData(fascicle: FascicleModel) {
        sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_CURRENT_METADATA_VALUES);
        let fascicleTypeName: string = "";
        switch (FascicleType[fascicle.FascicleType.toString()]) {
            case FascicleType.Procedure:
                fascicleTypeName = "Fascicolo di procedimento";
                break;
            case FascicleType.Period:
                fascicleTypeName = "Fascicolo periodico";
                $(`#${this.rowManagerId}`).hide();
                break;
            case FascicleType.Legacy:
                fascicleTypeName = "Fascicolo non a norma";
                break;
            case FascicleType.Activity:
                fascicleTypeName = "Fascicolo di attività";
                $(`#${this.rowManagerId}`).hide();
                break;
        }

        if ($.type(fascicle.FascicleType) === "string") {
            fascicle.FascicleType = FascicleType[fascicle.FascicleType.toString()];
        }

        $(`#${this.rowManagerId}`).hide();
        if (fascicle.Contacts.length > 0 && !this.isEditPage) {
            $(`#${this.rowManagerId}`).show();
        }

        this.getCustomActionsColumn().hide();
        this._loadCustomActions(fascicle);

        let handler: string = "";
        let role: WorkflowRoleModel;

        if (!String.isNullOrEmpty(this.workflowActivityId)) {
            this._workflowActivityService.getWorkflowActivity(this.workflowActivityId,
                (data: any) => {
                    if (data == null) return;
                    this._workflowActivity = data;
                    let subject: string;

                    // Per ora non si mostra il proponente del flusso di lavoro, questa informazione sarà visibile solo nella scrivania del flusso di lavoro
                    //if (this._workflowActivity.WorkflowProperties != null) {
                    //    let mapper: WorkflowRoleModelMapper = new WorkflowRoleModelMapper();
                    //    let propertyRole: WorkflowPropertyModel = this._workflowActivity.WorkflowProperties.filter(function (item) { if (item.Name == WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE) return item; })[0];
                    //    role = mapper.Map(JSON.parse(propertyRole.ValueString));                        
                    //}

                    subject = this._workflowActivity.WorkflowProperties.filter(function (item) { if (item.Name == WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT) return item; })[0].ValueString;
                    fascicle.Note = subject;
                    this.loadUscSummaryFascicle(fascicle);

                    if (this._workflowActivity.WorkflowAuthorizations) {

                        let authorization: WorkflowAuthorization = this._workflowActivity.WorkflowAuthorizations.filter(function (item) { if (item.IsHandler == true) return item; })[0];

                        if (authorization) {
                            handler = authorization.Account;
                            this._domainUserService.getUser(handler,
                                (user: DomainUserModel) => {
                                    if (user) {
                                        this.loadExternalDataAjaxRequest(fascicle, user.DisplayName);
                                    }
                                },
                                (exception: ExceptionDTO) => {
                                    this._uscNotification = <UscErrorNotification>$(`#${this.uscNotificationId}`).data();
                                    if (!jQuery.isEmptyObject(this._uscNotification)) {
                                        this._uscNotification.showNotification(exception);
                                    }
                                }
                            );
                        }
                    }
                },
                (exception: ExceptionDTO) => {
                    this._uscNotification = <UscErrorNotification>$(`#${this.uscNotificationId}`).data();
                    if (!jQuery.isEmptyObject(this._uscNotification)) {
                        this._uscNotification.showNotification(exception);
                    }
                }
            );
        }

        if (this.fasciclesPanelVisibilities["FasciclesLinkedPanelVisibility"] != undefined && this.fasciclesPanelVisibilities["FasciclesLinkedPanelVisibility"]) {
            $.when(this.refreshLinkedFasciclesRequest(fascicle)).always(() => {
                setTimeout(() => {
                    this.loadExternalDataAjaxRequest(fascicle, handler);
                }, 1);
            });
        }
        else {
            this.loadExternalDataAjaxRequest(fascicle, handler);
        }
    }

    private loadExternalDataAjaxRequest(fascicle: FascicleModel, workflowHandler: string) {
        if (this.isAuthorizePage) {
            this.renderRoles(fascicle.FascicleRoles);
        }

        let jsonFascicle: string = JSON.stringify(fascicle);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();
        ajaxModel.ActionName = "LoadExternalData";
        ajaxModel.Value = new Array<string>();
        ajaxModel.Value.push(jsonFascicle);
        ajaxModel.Value.push(JSON.stringify(!String.isNullOrEmpty(this.workflowActivityId)));
        ajaxModel.Value.push(workflowHandler);
        if (!this.isEditPage && this.metadataRepositoryEnabled && fascicle.MetadataDesigner && fascicle.MetadataValues) {
            $(`#${this.rowDynamicMetadataId}`).show();
            let uscDynamicMetadataSummaryRest: UscDynamicMetadataSummaryRest = <UscDynamicMetadataSummaryRest>$("#".concat(this.uscDynamicMetadataSummaryRestId)).data();
            if (!jQuery.isEmptyObject(uscDynamicMetadataSummaryRest)) {
                uscDynamicMetadataSummaryRest.loadMetadatas(fascicle.MetadataDesigner, fascicle.MetadataValues);
                sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_CURRENT_METADATA_VALUES, fascicle.MetadataDesigner);
            }
        }

        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
    }

    /**
     * Callback del caricamento della lista dei manager
     */
    loadExternalDataCallback(fascicleVisibilityTypeButtonVisibility: boolean): void {
        let uscFascicleFolder = <UscFascicleFolders>$(`#${this.uscFascFoldersId}`).data();
        if (!jQuery.isEmptyObject(uscFascicleFolder)) {
            if (!sessionStorage.getItem(uscFascicleFolder.SESSION_FascicleHierarchy) ||
                sessionStorage.getItem(uscFascicleFolder.SESSION_FascicleHierarchy) == "[]") {
                uscFascicleFolder.selectFascicleNode(false);
            }
        }
        $(`#${this.pageId}`).triggerHandler(uscFascicolo.DATA_LOADED_EVENT);
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleId).done((instance) => {
            instance.setVisibilityOnFascicleVisibilityTypeButton(fascicleVisibilityTypeButtonVisibility);
            instance.setFascicleVisibilityTypeButtonCheck(this._fascicleVisibilityType.toString());
        });
    }

    /**
    * Metodo per il caricamento della griglia dei fascicoli collegati
    * @param sender
    * @param onDoneCallback
    */
    refreshLinkedFasciclesRequest(fascicle: FascicleModel): JQueryPromise<void> {
        let result: JQueryDeferred<void> = $.Deferred<void>();
        this._service.getLinkedFascicles(fascicle, null,
            (data: FascicleModel) => {
                this.refreshLinkedFascicles(data);
                return result.resolve();
            },
            (exception: ExceptionDTO) => {
                this._uscNotification = <UscErrorNotification>$(`#${this.uscNotificationId}`).data();
                if (!jQuery.isEmptyObject(this._uscNotification)) {
                    this._uscNotification.showNotification(exception);
                }

                return result.reject();
            }
        );

        return result.promise();
    }

    /**
     * Metodo per caricare le UD in griglia
     * @param models
     */
    refreshGridUD(models: FascicleDocumentUnitModel[], insertsArchiveChains: string[]): void {
        let filters: { [id: string]: string; } = {};
        this._grid.get_masterTableView().get_filterExpressions().forEach(
            (item, index) => {
                filters[item.get_columnUniqueName()] = item.get_fieldValue();
            }
        );

        let selectedFolder: FascicleSummaryFolderViewModel = this.getSelectedFascicleFolder();
        if (selectedFolder) {
            $(`#${this.lblUDGridTitleId}`).html(`Documenti nel fascicolo (${selectedFolder.Name})`);
        }

        let orders = this._grid.get_masterTableView().get_sortExpressions();
        let sort = [];
        orders.forEach((order) => {
            sort.push({
                FieldName: order.FieldName,
                SortOrder: order.SortOrder
            });
        });

        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();
        ajaxModel.ActionName = "ReloadGrid";
        ajaxModel.Value = new Array<string>();
        ajaxModel.Value.push(JSON.stringify(models));
        ajaxModel.Value.push(JSON.stringify(filters));
        ajaxModel.Value.push(JSON.stringify(insertsArchiveChains));
        ajaxModel.Value.push(JSON.stringify(sort));
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
    }

    /**
     * Callback di finalizzazione caricamento griglia
     */
    refreshGridUDCallback(filters: string, orders: any): void {
        this._grid = <Telerik.Web.UI.RadGrid>$find(this.grdUDId);

        let filtersModel: FiltersGridUDFasciclesGridViewModel = new FiltersGridUDFasciclesViewModelMapper().Map(filters);
        for (let prop in filtersModel) {
            if (filtersModel[prop]) {
                let filt: Telerik.Web.UI.GridFilterExpression = new Telerik.Web.UI.GridFilterExpression();
                filt.set_columnUniqueName(prop);
                filt.set_fieldValue(filtersModel[prop]);
                this._grid.get_masterTableView().get_filterExpressions().add(filt);

                if (prop == "DocumentUnitName" && !(this.fasciclesPanelVisibilities["GridSearchPanelVisibility"])) {
                    try {
                        let dropDownItem: Telerik.Web.UI.RadDropDownList = $find($(".udComboFilter")[0].id) as Telerik.Web.UI.RadDropDownList;
                        let toSelectItem: Telerik.Web.UI.DropDownListItem = dropDownItem.findItemByValue(filtersModel[prop]);
                        this._notFireEvent = true;
                        toSelectItem.select();
                    } finally {
                        this._notFireEvent = false;
                    }
                }
            }
        };
        $(`#${this.pageId}`).triggerHandler(uscFascicolo.GRID_REFRESH_EVENT);
        for (let order of orders) {
            let sortExpression: any = new Telerik.Web.UI.GridSortExpression();
            sortExpression.set_fieldName(order.FieldName);
            sortExpression.set_sortOrder(order.SortOrder);
            this._grid.get_masterTableView().get_sortExpressions().add(sortExpression);
            let gridMasterTableView: any = this._grid.get_masterTableView();
            gridMasterTableView._showSortIconForField(order.FieldName, order.SortOrder);
        }
        this._loadingPanel.hide(this.grdUDId);
    }

    /**
     * Metodo per caricare i fascicoli collegati in griglia
     * @param models
     */
    //TODO: la griglia dei fascicoli collegati dovrà essere uno usercontrol
    refreshLinkedFascicles = (data: FascicleModel) => {
        let models: Array<LinkedFasciclesViewModel> = new Array<LinkedFasciclesViewModel>();
        if (data == null) return;
        if (data.FascicleLinks.length > 0) {
            $.each(data.FascicleLinks, (index, fascicleLink) => {
                let model: LinkedFasciclesViewModel;
                let imageUrl: string = "";
                let openCloseTooltip: string = "";
                let fascicleTypeImageUrl: string = "";
                let fascicleTypeTooltip: string = "";
                if (fascicleLink.FascicleLinked.EndDate == null) {
                    imageUrl = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
                    openCloseTooltip = "Aperto";
                } else {
                    imageUrl = "../App_Themes/DocSuite2008/imgset16/folder_closed.png";
                    openCloseTooltip = "Chiuso";
                }

                switch (FascicleType[fascicleLink.FascicleLinked.FascicleType.toString()]) {
                    case FascicleType.Period:
                        fascicleTypeImageUrl = "../App_Themes/DocSuite2008/imgset16/history.png";
                        fascicleTypeTooltip = "Periodico";
                        break;
                    case FascicleType.Legacy:
                        fascicleTypeImageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_legacy.png";
                        fascicleTypeTooltip = "Fascicolo non a norma";
                        break;
                    case FascicleType.Procedure:
                        fascicleTypeImageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_procedure.png";
                        fascicleTypeTooltip = "Per procedimento";
                        break;
                    case FascicleType.SubFascicle:
                        fascicleTypeImageUrl = "";
                        fascicleTypeTooltip = "Sotto fascicolo";
                        break;
                }

                let tileText: string = `${fascicleLink.FascicleLinked.Title} ${fascicleLink.FascicleLinked.FascicleObject}`;

                model = {
                    Name: tileText, FascicleLinkUniqueId: fascicleLink.UniqueId, UniqueId: fascicleLink.FascicleLinked.UniqueId, Category: fascicleLink.FascicleLinked.Category.Name,
                    ImageUrl: imageUrl, OpenCloseTooltip: openCloseTooltip, FascicleTypeImageUrl: fascicleTypeImageUrl, FascicleTypeToolTip: fascicleTypeTooltip
                };
                models.push(model);
            });
        }
    }

    private removeSpecificFilter = (name: string) => {
        let filterComm: Telerik.Web.UI.GridFilterExpression = null;
        this._grid.get_masterTableView().get_filterExpressions().forEach((item) => {
            if (item.ColumnUniqueName === name) {
                filterComm = item;
                return;
            }
        });
        if (filterComm) {
            this._grid.get_masterTableView().get_filterExpressions().remove(filterComm);
        }
        this._grid.get_masterTableView().fireCommand("Filter", "GridFilterCommandEventArgs");
    }

    openPreviewWindow(serializedDoc: string) {
        let url: string = `../Viewers/DocumentInfoViewer.aspx?${serializedDoc}`;
        this.openWindow(url, 'windowPreviewDocument', 750, 450);
    }

    /**
* Apre una nuova nuova RadWindow
* @param url
* @param name
* @param width
* @param height
*/
    openWindow(url, name, width, height): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.rwmDocPreviewId);
        let wnd: Telerik.Web.UI.RadWindow = manager.open(url, name, null);
        wnd.setSize(width, height);
        wnd.set_modal(true);
        wnd.center();
        return false;
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

    getSelectedFascicleFolder(): FascicleSummaryFolderViewModel {
        let uscFascFolders: UscFascicleFolders = <UscFascicleFolders>$(`#${this.uscFascFoldersId}`).data();
        if (!jQuery.isEmptyObject(uscFascFolders)) {
            return uscFascFolders.getSelectedFascicleFolder(this.currentFascicleId);
        }
        return undefined;
    }

    expandCollapseFolders(): void {
        let pnl: Telerik.Web.UI.RadSlidingZone = <Telerik.Web.UI.RadSlidingZone>$find(this.rszFolderId);
        let pane: Telerik.Web.UI.RadSlidingPane = pnl.getPaneById(this.rsPnlFoldersId);
        if (pane.get_collapsed()) {
            pnl.expandPane(this.rsPnlFoldersId);
            pnl.dockPane(this.rsPnlFoldersId);
        }
        else {
            pnl.undockPane(this.rsPnlFoldersId);
            pnl.collapsePane(this.rsPnlFoldersId);
        }
    }

    getSelectedVisibilityType = (): VisibilityType => {
        if (this.isAuthorizePage) {
            return this._fascicleVisibilityType;
        }
        return undefined;
    }

    private _loadCustomActions(fascicle: FascicleModel): void {
        let currentDate: Date = new Date();
        const autoCloseThresholdDate: Date = new Date(currentDate
            .setDate(currentDate.getDate() - this.fascicleAutoCloseThresholdDays - this.fascicleAutoCloseWarningThresholdDays));
        let isAutoClosable: boolean = fascicle.CustomActions && (<FascicleCustomActionModel>JSON.parse(fascicle.CustomActions)).AutoClose &&
            fascicle.LastChangedDate && fascicle.LastChangedDate.toString() <= autoCloseThresholdDate.toString();
        if (isAutoClosable && !this.isEditPage) {
            const customActions: FascicleCustomActionModel = fascicle.CustomActions
                ? <FascicleCustomActionModel>JSON.parse(fascicle.CustomActions)
                : <FascicleCustomActionModel>{
                    AutoClose: false,
                    AutoCloseAndClone: false
                };
            let customActionsIcons: CustomActionsIconModel[] = null;
            if (customActions.AutoClose) {
                customActionsIcons = [
                    {
                        UseIconFor: "AutoClose",
                        IconURL: "../App_Themes/DocSuite2008/imgset32/auto-lock.png",
                        Tooltip: "Chiusura amministrativa a 60 giorni"
                    }
                ];
            }
            else {
                customActionsIcons = [
                    {
                        UseIconFor: "AutoCloseAndClone",
                        IconURL: "../App_Themes/DocSuite2008/imgset32/auto-lock-copy.png",
                        Tooltip: "Chiusura amministrativa a fine anno e riapertura automatica"
                    }
                ];
            }
            PageClassHelper.callUserControlFunctionSafe<uscCustomActionsRest>(this.uscCustomActionsRestId).done((instance) => {
                instance.loadItems(customActions, customActionsIcons);
            });

            let fascSummaryColumn = this.getFascSummaryColumn();
            let customActionsColumn = this.getCustomActionsColumn();

            fascSummaryColumn.removeClass(uscFascicolo.FULLWIDTH_COL_SPAN);
            fascSummaryColumn.addClass(uscFascicolo.SUMMARY_COL_SPAN);
            customActionsColumn.addClass(uscFascicolo.ACTIONS_COL_SPAN);
            customActionsColumn.show();
        }
    }

    private registerUscRoleRestEventHandlers(): void {
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleId)
            .done((instance) => {
                instance.registerEventHandler(UscRoleRestEventType.RoleDeleted, (roleId: number) => {
                    this.deleteRoleFromModel(roleId);
                    return $.Deferred<void>().resolve();
                });
                instance.registerEventHandler(UscRoleRestEventType.NewRolesAdded, (newAddedRoles: RoleModel[]) => {
                    if (!uscFascicolo.masterRole) {
                        this.addRole(newAddedRoles, false);
                        return $.Deferred<void>().resolve();
                    }
                    let existedRole: RoleModel = newAddedRoles.filter(x => x.EntityShortId === uscFascicolo.masterRole.EntityShortId)[0];
                    if (existedRole) {
                        alert(`Non è possibile selezionare il settore ${existedRole.Name} in quanto già presente come settore autorizzato del fascicolo`);
                        newAddedRoles = newAddedRoles.filter(x => x.IdRole !== existedRole.IdRole);
                    }
                    this.addRole(newAddedRoles, false);
                    return $.Deferred<RoleModel>().resolve(existedRole);
                });
                instance.registerEventHandler(UscRoleRestEventType.SetFascicleVisibilityType, (visibilityType: VisibilityType) => {
                    this._fascicleVisibilityType = visibilityType;
                    return $.Deferred<void>().resolve();
                });
            });
    }

    private addRole(newAddedRoles: RoleModel[], isMaster: boolean): void {
        if (!newAddedRoles.length)
            return;

        this._rolesAddedIds = newAddedRoles.map(x => x.IdRole);
        let fascicleRoles: FascicleRoleModel[] = [];
        if (this.getFascicleRolesToAdd()) {
            fascicleRoles = this.getFascicleRolesToAdd();
        }

        for (let newAddedRole of newAddedRoles) {
            let fascicleRole = new FascicleRoleModel();
            fascicleRole.IsMaster = isMaster;
            fascicleRole.Role = newAddedRole;
            fascicleRoles.push(fascicleRole);
        }
        this.setFascicleRolesToSession(fascicleRoles);
    }

    private getFascicleRolesToAdd(): FascicleRoleModel[] {
        let itemsFromSession: string = sessionStorage.getItem(`${this.clientId}_FascicleRolesToAdd`);
        if (itemsFromSession) {
            return JSON.parse(itemsFromSession) as FascicleRoleModel[];
        }
        return null;
    }

    private setFascicleRolesToSession(fascicleRoles: FascicleRoleModel[]): void {
        if (!fascicleRoles) {
            sessionStorage.removeItem(`${this.clientId}_FascicleRolesToAdd`);
        }

        sessionStorage[`${this.clientId}_FascicleRolesToAdd`] = JSON.stringify(fascicleRoles);
    }

    private deleteRoleFromModel(roleIdToDelete: number): void {
        if (!roleIdToDelete)
            return;

        this._rolesRemovedIds.push(roleIdToDelete);
        let fascicleRoles: FascicleRoleModel[] = [];
        if (this.getFascicleRolesToAdd()) {
            fascicleRoles = this.getFascicleRolesToAdd();
        }

        fascicleRoles = fascicleRoles.filter(x => x.Role.IdRole !== roleIdToDelete && x.Role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1);
        this.setFascicleRolesToSession(fascicleRoles);
    }

    getRaciRoles = (): JQueryPromise<RoleModel[]> => {
        let promise: JQueryDeferred<RoleModel[]> = $.Deferred<RoleModel[]>();
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleId)
            .done((instance) => {
                return promise.resolve(instance.getRaciRoles());
            });
        return promise.promise();
    }

    getAddedRolesIds = (): number[] => {
        return this._rolesAddedIds;
    }

    getRemovedRolesIds = (): number[] => {
        return this._rolesRemovedIds;
    }

    setFascicleVisibilityTypeButtonCheck = (fascicleVisibilityType: VisibilityType) => {
        this._fascicleVisibilityType = fascicleVisibilityType;
    }

    private renderRoles(fascicleRoles: FascicleRoleModel[]): void {
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleId).done((instance) => {
            instance.setToolbarVisibility(true);
            let fascicleRaciRoles: FascicleRoleModel[] = fascicleRoles.filter(x => x.IsMaster === false &&
                x.AuthorizationRoleType.toString() === AuthorizationRoleType[AuthorizationRoleType.Responsible]);
            if (fascicleRaciRoles) {
                instance.setRaciRoles(fascicleRaciRoles.map(x => x.Role));
            }
            instance.renderRolesTree(fascicleRoles.filter(x => x.IsMaster === false).map(x => x.Role));
        });
    }

    getRemovedRaciRoles = (): JQueryPromise<RoleModel[]> => {
        let promise: JQueryDeferred<RoleModel[]> = $.Deferred<RoleModel[]>();
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleId)
            .done((instance) => {
                return promise.resolve(instance.getRemovedRaciRoles());
            });
        return promise.promise();
    }
}

export = uscFascicolo;
