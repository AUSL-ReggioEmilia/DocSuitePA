/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />

import WindowHelper = require('App/Helpers/WindowHelper');
import CategoryViewModel = require('App/ViewModels/Tblt/CategoryViewModel');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import TbltTenantBase = require("./TbltTenantBase");
import TenantViewModel = require("App/ViewModels/Tenants/TenantViewModel");
import TenantSearchFilterDTO = require('App/DTOs/TenantSearchFilterDTO');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import PECMailBoxModel = require('App/Models/PECMails/PECMailBoxModel');
import TenantConfigurationModel = require('App/Models/Tenants/TenantConfigurationModel');
import RoleModel = require('App/Models/Commons/RoleModel');
import WorkflowRepositoryModel = require('App/Models/Workflows/WorkflowRepositoryModel');
import TenantWorkflowRepositoryModel = require('App/Models/Tenants/TenantWorkflowRepositoryModel');
import TenantViewModelMapper = require('App/Mappers/Tenants/TenantViewModelMapper');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');
import TenantConfigurationTypeEnum = require('App/Models/Tenants/TenantConfigurationTypeEnum');
import TenantWorkflowRepositoryTypeEnum = require('App/Models/Tenants/TenantWorkflowRepositoryTypeEnum');
import EnumHelper = require("App/Helpers/EnumHelper");
import ContactModel = require('../App/Models/Commons/ContactModel');
import uscContattiSelRest = require('../UserControl/uscContattiSelRest');
import uscRoleRest = require('../UserControl/uscRoleRest');


class TbltTenant extends TbltTenantBase {

    ajaxManagerId: string;
    uscNotificationId: string;
    ajaxLoadingPanelId: string;
    splitterMainId: string;
    toolBarSearchId: string;
    pnlDetailsId: string;
    uscRoleRestId: string;
    uscContattiSelRestId: string;

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;

    //rad tree views
    rtvTenantsId: string;
    rtvContainersId: string;
    rtvPECMailBoxesId: string;
    rtvWorkflowRepositoriesId: string;
    rtvTenantConfigurationsId: string;

    // toolbars where the buttons are
    tbContainersControlId: string;
    tbPECMailBoxesControlId: string;
    tbWorkflowRepositoryControlId: string;
    tbConfigurationControlId: string;

    //details right zone
    lblCompanyNameId: string;
    lblTenantNameId: string;
    lblTenantNoteId: string;
    lblTenantDataDiAttivazioneId: string;
    lblTenantDataDiDisattivazioneId: string;

    //rad windows
    rwContainerId: string;
    rwPECMailBoxId: string;
    rwRoleId: string;
    rwWorkflowRepositoryId: string;
    rwTenantConfigurationId: string;
    rwTenantSelectorId: string;
 
    //window combos
    cmbContainerId: string;
    cmbPECMailBoxId: string;
    cmbWorkflowRepositoryId: string;
    cmbRoleId: string;
    cmbConfigurationTypeId: string;
    cmbTenantWorkflowRepositoryTypeId: string;

    // Window buttons Confirm, Cancel
    btnContainerSelectorOkId: string;
    btnContainerSelectorCancelId: string;
    btnPECMailBoxSelectorOkId: string;
    btnPECMailBoxSelectorCancelId: string;
    btnRoleSelectorOkId: string;
    btnRoleSelectorCancelId: string;
    btnWorkflowRepositorySelectorOkId: string;
    btnWorkflowRepositorySelectorCancelId: string;
    btnTenantConfigurationSelectorOkId: string;
    btnTenantConfigurationSelectorCancelId: string;
    btnTenantSelectorOkId: string;
    btnTenantSelectorCancelId: string;
    btnTenantInsertId: string;
    btnTenantUpdateId: string;

    // window configuration fields
    dpStartDateFromId: string;
    dpEndDateFromId: string;
    tenantConfigurationNoteId: string;
    dpTenantDateFromId: string;
    dpTenantDateToId: string;
    txtTenantNameId: string;
    txtTenantCompanyId: string;
    txtTenantNoteId: string;
    dpTenantWorkflowRepositoryDateFromId: string;
    dpTenantWorkflowRepositoryDateToId: string;
    txtTenantConfigurationJsonValueId: string;
    txtTenantWorkflowRepositoryJsonValueId: string;
    txtTenantWorkflowRepositoryIntegrationModuleNameId: string;
    txtTenantWorkflowRepositoryConditionsId: string;

    rtvResult: TenantViewModel[];
    selectedTenant: TenantViewModel;
    selectedTenantConfiguration: TenantConfigurationModel;

    //dropdown data models
    containers: ContainerModel[];
    selectedContainer: ContainerModel;
    pecMailBoxes: PECMailBoxModel[];
    selectedPECMailBox: PECMailBoxModel;
    roles: RoleModel[];
    selectedRole: RoleModel;
    tenantWorkflowRepositories: TenantWorkflowRepositoryModel[];
    selectedTenantWorkflowRepository: TenantWorkflowRepositoryModel;
    workflowRepositories: WorkflowRepositoryModel[];
    selectedWorkflowRepository: WorkflowRepositoryModel;
    currentTenantConfigurationUniqueId: string;
    currentTenantWorkflowRepositoryUniqueId: string;
    isTenantUpdate: boolean;
    private _enumHelper: EnumHelper;
    maxNumberElements: string;
    contacts: ContactModel[];

    private _currentSelectedTenant: TenantViewModel;
   
    //rad tree views
    private _rtvTenants: Telerik.Web.UI.RadTreeView;
    private _rtvContainers: Telerik.Web.UI.RadTreeView;
    private _rtvPECMailBoxes: Telerik.Web.UI.RadTreeView;
    private _rtvRoles: Telerik.Web.UI.RadTreeView;
    private _rtvTenantWorkflowRepositories: Telerik.Web.UI.RadTreeView;
    private _rtvTenantConfigurations: Telerik.Web.UI.RadTreeView;

    // toolbars where the buttons are
    private _toolbarSearch: Telerik.Web.UI.RadToolBar;
    private _toolbarContainer: Telerik.Web.UI.RadToolBar;
    private _toolbarPECMailBox: Telerik.Web.UI.RadToolBar;
    private _toolbarRole: Telerik.Web.UI.RadToolBar;
    private _toolbarWorkflowRepository: Telerik.Web.UI.RadToolBar;
    private _tbConfigurationControl: Telerik.Web.UI.RadToolBar;
    private _toolbarItemSearchTenantName: Telerik.Web.UI.RadToolBarItem;
    private _toolbarItemSearchCompanyName: Telerik.Web.UI.RadToolBarItem;
    private _txtSearchTenantName: Telerik.Web.UI.RadTextBox;
    private _txtSearchCompanyName: Telerik.Web.UI.RadTextBox;

    //details right zone
    private _lblCompanyNameId: HTMLLabelElement;
    private _lblTenantNameId: HTMLLabelElement;
    private _lblTenantNoteId: HTMLLabelElement;
    private _lblTenantDataDiAttivazioneId: HTMLLabelElement;
    private _lblTenantDataDiDisattivazioneId: HTMLLabelElement;

    // windows
    private _rwContainer: Telerik.Web.UI.RadWindow;
    private _rwPECMailBox: Telerik.Web.UI.RadWindow;
    private _rwWorkflowRepository: Telerik.Web.UI.RadWindow;
    private _rwTenantConfiguration: Telerik.Web.UI.RadWindow;
    private _rwTenantSelector: Telerik.Web.UI.RadWindow;

    //window combos
    private _cmbContainer: Telerik.Web.UI.RadComboBox;
    private _cmbPECMailBox: Telerik.Web.UI.RadComboBox;
    private _cmbRole: Telerik.Web.UI.RadComboBox;
    private _cmbWorkflowRepository: Telerik.Web.UI.RadComboBox;
    private _cmbConfigurationType: Telerik.Web.UI.RadComboBox;
    private _cmbTenantWorkflowRepositoryType: Telerik.Web.UI.RadComboBox;

    // Window buttons Confirm, Cancel
    private _btnContainerOk: Telerik.Web.UI.RadButton;
    private _btnContainerCancel: Telerik.Web.UI.RadButton;
    private _btnPECMailBoxSelectorOk: Telerik.Web.UI.RadButton;
    private _btnPECMailBoxSelectorCancel: Telerik.Web.UI.RadButton;
    private _btnWorkflowRepositorySelectorOk: Telerik.Web.UI.RadButton;
    private _btnWorkflowRepositorySelectorCancel: Telerik.Web.UI.RadButton;
    private _btnTenantConfigurationSelectorOk: Telerik.Web.UI.RadButton;
    private _btnTenantConfigurationSelectorCancel: Telerik.Web.UI.RadButton;
    private _btnTenantSelectorOk: Telerik.Web.UI.RadButton;
    private _btnTenantSelectorCancel: Telerik.Web.UI.RadButton;
    private _btnTenantInsert: Telerik.Web.UI.RadButton;
    private _btnTenantUpdate: Telerik.Web.UI.RadButton;

    // window configuration fields
    private _dpStartDateFrom: Telerik.Web.UI.RadDatePicker;
    private _dpEndDateFrom: Telerik.Web.UI.RadDatePicker;
    private _txtTenantConfigurationNote: Telerik.Web.UI.RadTextBox;
    private _dpTenantDateFrom: Telerik.Web.UI.RadDatePicker;
    private _dpTenantDateTo: Telerik.Web.UI.RadDatePicker;
    private _txtTenantName: Telerik.Web.UI.RadTextBox;
    private _txtTenantCompany: Telerik.Web.UI.RadTextBox;
    private _txtTenantNote: Telerik.Web.UI.RadTextBox;
    private _dpTenantWorkflowRepositoryDateFrom: Telerik.Web.UI.RadDatePicker;
    private _dpTenantWorkflowRepositoryDateTo: Telerik.Web.UI.RadDatePicker;
    private _txtTenantConfigurationJsonValue: Telerik.Web.UI.RadTextBox;
    private _txtTenantWorkflowRepositoryJsonValue: Telerik.Web.UI.RadTextBox;
    private _txtTenantWorkflowRepositoryIntegrationModuleName: Telerik.Web.UI.RadTextBox;
    private _txtTenantWorkflowRepositoryConditions: Telerik.Web.UI.RadTextBox;

    private _serviceConfiguration: ServiceConfiguration[];
    private _uscRoleRest: uscRoleRest;
    private _uscContattiSelRest: uscContattiSelRest;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(serviceConfigurations);
        this._serviceConfiguration = serviceConfigurations;

        this.selectedTenant = new TenantViewModel();
        this._currentSelectedTenant = new TenantViewModel();
        this.selectedTenantConfiguration = new TenantConfigurationModel();

        this._enumHelper = new EnumHelper();
    }

    initialize(): void {
        super.initialize();
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);

        this._toolbarSearch = <Telerik.Web.UI.RadToolBar>$find(this.toolBarSearchId);
        this._toolbarSearch.add_buttonClicking(this.toolbarSearch_onClick);

        //rad tree views
        this._rtvTenants = <Telerik.Web.UI.RadTreeView>$find(this.rtvTenantsId);
        this._rtvTenants.add_nodeClicking(this.rtvTenants_onClick);
        this._rtvContainers = <Telerik.Web.UI.RadTreeView>$find(this.rtvContainersId);
        this._rtvPECMailBoxes = <Telerik.Web.UI.RadTreeView>$find(this.rtvPECMailBoxesId);
        this._rtvTenantWorkflowRepositories = <Telerik.Web.UI.RadTreeView>$find(this.rtvWorkflowRepositoriesId);
        this._rtvTenantWorkflowRepositories.add_nodeClicked(this.rtvTenantWorkflowrepositories_onNodeClick);
        this._rtvTenantConfigurations = <Telerik.Web.UI.RadTreeView>$find(this.rtvTenantConfigurationsId);

        // details right zone
        this._toolbarItemSearchTenantName = this._toolbarSearch.findItemByValue("searchTenantName");
        this._toolbarItemSearchCompanyName = this._toolbarSearch.findItemByValue("searchCompanyName");
        this._txtSearchTenantName = <Telerik.Web.UI.RadTextBox>this._toolbarItemSearchTenantName.findControl("txtSearchTenantName");
        this._txtSearchCompanyName = <Telerik.Web.UI.RadTextBox>this._toolbarItemSearchCompanyName.findControl("txtSearchCompanyName");

        this._lblCompanyNameId = <HTMLLabelElement>document.getElementById(this.lblCompanyNameId);
        this._lblTenantNameId = <HTMLLabelElement>document.getElementById(this.lblTenantNameId);
        this._lblTenantNoteId = <HTMLLabelElement>document.getElementById(this.lblTenantNoteId);
        this._lblTenantDataDiAttivazioneId = <HTMLLabelElement>document.getElementById(this.lblTenantDataDiAttivazioneId);
        this._lblTenantDataDiDisattivazioneId = <HTMLLabelElement>document.getElementById(this.lblTenantDataDiDisattivazioneId);

        //Containers, PECMailBoxes, Rules, WorkflowReposiory, TenantConfiguration
        this._toolbarContainer = <Telerik.Web.UI.RadToolBar>$find(this.tbContainersControlId);
        this._toolbarContainer.add_buttonClicking(this.toolbarContainer_onClick);
        this._toolbarPECMailBox = <Telerik.Web.UI.RadToolBar>$find(this.tbPECMailBoxesControlId);
        this._toolbarPECMailBox.add_buttonClicking(this.toolbarPECMailBox_onClick);
        this._toolbarWorkflowRepository = <Telerik.Web.UI.RadToolBar>$find(this.tbWorkflowRepositoryControlId);
        this._toolbarWorkflowRepository.add_buttonClicking(this.toolbarWorkflowRepository_onClick);
        this._tbConfigurationControl = <Telerik.Web.UI.RadToolBar>$find(this.tbConfigurationControlId);
        this._tbConfigurationControl.add_buttonClicking(this.toolbarConfiguration_onClick);

        // windows
        this._rwContainer = <Telerik.Web.UI.RadWindow>$find(this.rwContainerId);
        this._rwContainer.add_show(this._rwContainer_OnShow);
        this._rwPECMailBox = <Telerik.Web.UI.RadWindow>$find(this.rwPECMailBoxId);
        this._rwPECMailBox.add_show(this._rwPECMailBox_OnShow);
        this._rwTenantConfiguration = <Telerik.Web.UI.RadWindow>$find(this.rwTenantConfigurationId);
        this._rwWorkflowRepository = <Telerik.Web.UI.RadWindow>$find(this.rwWorkflowRepositoryId);
        this._rwWorkflowRepository.add_show(this._rwWorkflowRepository_OnShow);
        this._rwTenantSelector = <Telerik.Web.UI.RadWindow>$find(this.rwTenantSelectorId);

        //combos from windows
        this._cmbContainer = <Telerik.Web.UI.RadComboBox>$find(this.cmbContainerId);
        this._cmbContainer.add_selectedIndexChanged(this.cmbContainers_onClick);
        this._cmbContainer.add_itemsRequested(this._cmbContainer_OnClientItemsRequested);
        this._cmbPECMailBox = <Telerik.Web.UI.RadComboBox>$find(this.cmbPECMailBoxId);
        this._cmbPECMailBox.add_selectedIndexChanged(this.cmbPECMailBoxes_onClick);
        this._cmbPECMailBox.add_itemsRequested(this._cmbPECMailBox_OnClientItemsRequested);
        this._cmbRole = <Telerik.Web.UI.RadComboBox>$find(this.cmbRoleId);
        this._cmbWorkflowRepository = <Telerik.Web.UI.RadComboBox>$find(this.cmbWorkflowRepositoryId);
        this._cmbWorkflowRepository.add_selectedIndexChanged(this.cmbWorkflowRepositories_onClick);
        this._cmbWorkflowRepository.add_itemsRequested(this._cmbWorkflowRepository_OnClientItemsRequested);
        this._cmbConfigurationType = <Telerik.Web.UI.RadComboBox>$find(this.cmbConfigurationTypeId);
        this._cmbTenantWorkflowRepositoryType = <Telerik.Web.UI.RadComboBox>$find(this.cmbTenantWorkflowRepositoryTypeId);

        // Window buttons Confirm, Cancel
        this._btnContainerOk = <Telerik.Web.UI.RadButton>$find(this.btnContainerSelectorOkId);
        this._btnContainerOk.add_clicking(this.btnContainerOk_onClick);
        this._btnContainerCancel = <Telerik.Web.UI.RadButton>$find(this.btnContainerSelectorCancelId);
        this._btnContainerCancel.add_clicking(this.btnContainerCancel_onClick);
        this._btnPECMailBoxSelectorOk = <Telerik.Web.UI.RadButton>$find(this.btnPECMailBoxSelectorOkId);
        this._btnPECMailBoxSelectorOk.add_clicking(this.btnPECMailBoxOk_onClick);
        this._btnPECMailBoxSelectorCancel = <Telerik.Web.UI.RadButton>$find(this.btnPECMailBoxSelectorCancelId);
        this._btnPECMailBoxSelectorCancel.add_clicking(this.btnPECMailBoxCancel_onClick);
        this._btnWorkflowRepositorySelectorOk = <Telerik.Web.UI.RadButton>$find(this.btnWorkflowRepositorySelectorOkId);
        this._btnWorkflowRepositorySelectorOk.add_clicking(this.btnWorkflowRepositoryOk_onClick);
        this._btnWorkflowRepositorySelectorCancel = <Telerik.Web.UI.RadButton>$find(this.btnWorkflowRepositorySelectorCancelId);
        this._btnWorkflowRepositorySelectorCancel.add_clicking(this.btnWorkflowRepositoryCancel_onClick);
        this._btnTenantConfigurationSelectorOk = <Telerik.Web.UI.RadButton>$find(this.btnTenantConfigurationSelectorOkId);
        this._btnTenantConfigurationSelectorOk.add_clicking(this.btnTenantConfigurationOk_onClick);
        this._btnTenantConfigurationSelectorCancel = <Telerik.Web.UI.RadButton>$find(this.btnTenantConfigurationSelectorCancelId);
        this._btnTenantConfigurationSelectorCancel.add_clicking(this.btnTenantConfigurationCancel_onClick);
        this._btnTenantSelectorOk = <Telerik.Web.UI.RadButton>$find(this.btnTenantSelectorOkId);
        this._btnTenantSelectorOk.add_clicking(this.btnTenantSelectorOk_onClick);
        this._btnTenantSelectorCancel = <Telerik.Web.UI.RadButton>$find(this.btnTenantSelectorCancelId);
        this._btnTenantSelectorCancel.add_clicking(this.btnTenantSelectorCancel_onClick);
        this._btnTenantInsert = <Telerik.Web.UI.RadButton>$find(this.btnTenantInsertId);
        this._btnTenantInsert.add_clicking(this.btnTenantInsert_onClick);
        this._btnTenantUpdate = <Telerik.Web.UI.RadButton>$find(this.btnTenantUpdateId);
        this._btnTenantUpdate.add_clicking(this.btnTenantUpdate_onClick);

        // window configuration fields
        this._dpStartDateFrom = <Telerik.Web.UI.RadDatePicker>$find(this.dpStartDateFromId);
        this._dpEndDateFrom = <Telerik.Web.UI.RadDatePicker>$find(this.dpEndDateFromId);
        this._txtTenantConfigurationNote = <Telerik.Web.UI.RadTextBox>$find(this.tenantConfigurationNoteId);
        this._dpTenantDateFrom = <Telerik.Web.UI.RadDatePicker>$find(this.dpTenantDateFromId);
        this._dpTenantDateTo = <Telerik.Web.UI.RadDatePicker>$find(this.dpTenantDateToId);
        this._txtTenantName = <Telerik.Web.UI.RadTextBox>$find(this.txtTenantNameId);
        this._txtTenantCompany = <Telerik.Web.UI.RadTextBox>$find(this.txtTenantCompanyId);
        this._txtTenantNote = <Telerik.Web.UI.RadTextBox>$find(this.txtTenantNoteId);
        this._txtTenantConfigurationJsonValue = <Telerik.Web.UI.RadTextBox>$find(this.txtTenantConfigurationJsonValueId);
        this._dpTenantWorkflowRepositoryDateFrom = <Telerik.Web.UI.RadDatePicker>$find(this.dpTenantWorkflowRepositoryDateFromId);
        this._dpTenantWorkflowRepositoryDateTo = <Telerik.Web.UI.RadDatePicker>$find(this.dpTenantWorkflowRepositoryDateToId);
        this._txtTenantWorkflowRepositoryJsonValue = <Telerik.Web.UI.RadTextBox>$find(this.txtTenantWorkflowRepositoryJsonValueId);
        this._txtTenantWorkflowRepositoryIntegrationModuleName = <Telerik.Web.UI.RadTextBox>$find(this.txtTenantWorkflowRepositoryIntegrationModuleNameId);
        this._txtTenantWorkflowRepositoryConditions = <Telerik.Web.UI.RadTextBox>$find(this.txtTenantWorkflowRepositoryConditionsId);

        this._uscRoleRest = <uscRoleRest>$(`#${this.uscRoleRestId}`).data();

        this._uscContattiSelRest = <uscContattiSelRest>$(`#${this.uscContattiSelRestId}`).data();
        let searchDTO: TenantSearchFilterDTO = new TenantSearchFilterDTO();
        this.loadResults(searchDTO);
    }

    //region [ Tenants ]
    rtvTenants_onClick = (sender: any, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        this.loadTenantDetails(args.get_node().get_value());
    }

    loadTenantDetails(tenantId: string) {
        let tenant: TenantViewModel = this.rtvResult.filter(function (x) {
            return x.UniqueId === tenantId
        })[0];
        this._currentSelectedTenant = $.extend({}, tenant);

        this._lblCompanyNameId.innerText = tenant !== undefined ? tenant.CompanyName : "";
        this._lblTenantNameId.innerText = tenant !== undefined ? tenant.TenantName : "";
        this._lblTenantNoteId.innerText = tenant.Note !== null ? tenant.Note : "";
        this._lblTenantDataDiAttivazioneId.innerText = tenant !== undefined && moment(tenant.StartDate).isValid() ? moment(tenant.StartDate).format("DD-MM-YYYY") : "";
        this._lblTenantDataDiDisattivazioneId.innerText = tenant !== undefined && moment(tenant.EndDate).isValid() ? moment(tenant.EndDate).format("DD-MM-YYYY") : "";

        this.populateContainersTreeView(tenant);
        this.populatePECMailBoxesTreeView(tenant);
        this.populateTenantWorkflowRepositoriesTreeView(tenant);
        this.populateTenantConfigurationsTreeView(tenant);
        this.populateWorkflowRepositoryComboBox();
        this.populateContactTreeView(tenant);
        this.registerUscContattiRestEventHandlers();

        this.populateRolesTree(tenant);
        this.registerUscRoleRestEventHandlers();
    }

    //region [ Roles tree view ]
    private populateRolesTree(tenant: TenantViewModel): void {
        this._loadingPanel.show(this.splitterMainId);
        this._roleService.getTenantRoles(tenant.UniqueId,
            (tenantRoles: RoleModel[]) => {
                this._uscRoleRest.renderRolesTree(tenantRoles);
                this._currentSelectedTenant.Roles = tenantRoles;
                this._loadingPanel.hide(this.splitterMainId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.splitterMainId);
                $("#".concat(this.rtvTenantsId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    private registerUscRoleRestEventHandlers(): void {
        let uscRoleRestEvents = this._uscRoleRest.uscRoleRestEvents;

        this._uscRoleRest.registerEventHandler(uscRoleRestEvents.RoleDeleted, this.deleteTenantRolePromise);
        this._uscRoleRest.registerEventHandler(uscRoleRestEvents.NewRolesAdded, this.updateTenantRolesPromise);
    }

    private deleteTenantRolePromise = (roleIdToDelete: number, instanceId?: string): JQueryPromise<any> => {
        let promise: JQueryDeferred<any> = $.Deferred<any>();

        if (!roleIdToDelete)
            return promise.promise();

        this._currentSelectedTenant.Roles = this._currentSelectedTenant.Roles
            .filter(role => role.IdRole !== roleIdToDelete && role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1);

        this._loadingPanel.show(this.splitterMainId);
        this._tenantService.updateTenant(this._currentSelectedTenant,
            (data: any) => {
                promise.resolve(data);
                this._loadingPanel.hide(this.splitterMainId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.splitterMainId);
                $("#".concat(this.rtvTenantsId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            });
        return promise.promise();
    }

    private updateTenantRolesPromise = (newAddedRoles: RoleModel[], instanceId?: string): JQueryPromise<any> => {
        let promise: JQueryDeferred<any> = $.Deferred<any>();

        if (!newAddedRoles.length)
            return promise.promise();

        this._currentSelectedTenant.Roles = [...this._currentSelectedTenant.Roles, ...newAddedRoles];
        this._loadingPanel.show(this.splitterMainId);
        this._tenantService.updateTenant(this._currentSelectedTenant,
            (data: any) => {
                promise.resolve(data);
                this._loadingPanel.hide(this.splitterMainId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.splitterMainId);
                $("#".concat(this.rtvTenantsId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            });
        return promise.promise();
    }

    // endregion

    toolbarSearch_onClick = (sender: any, args: Telerik.Web.UI.RadToolBarCancelEventArgs) => {
        let tenantName = this._txtSearchTenantName.get_textBoxValue();
        let companyName = this._txtSearchCompanyName.get_textBoxValue();
        let searchDTO: TenantSearchFilterDTO = new TenantSearchFilterDTO();
        if (tenantName) {
            searchDTO.tenantName = tenantName;
        }
        if (companyName) {
            searchDTO.companyName = companyName;
        }
        this.loadResults(searchDTO);
    }

    private loadResults(searchDTO: TenantSearchFilterDTO) {
        this._loadingPanel.show(this.splitterMainId);
        let cmbItem: Telerik.Web.UI.RadComboBoxItem = null;
        for (let n in TenantConfigurationTypeEnum) {
            if (typeof TenantConfigurationTypeEnum[n] === 'string') {
                cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                cmbItem.set_text(this._enumHelper.getTenantConfigurationTypeDescription(TenantConfigurationTypeEnum[n]));
                cmbItem.set_value(<any>TenantConfigurationTypeEnum[n]);
                this._cmbConfigurationType.get_items().add(cmbItem);
            }
        }
        for (let n in TenantWorkflowRepositoryTypeEnum) {
            if (typeof TenantWorkflowRepositoryTypeEnum[n] === 'string') {
                cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                cmbItem.set_text(this._enumHelper.getTenantWorkflowRepositoryTypeDescription(TenantWorkflowRepositoryTypeEnum[n]));
                cmbItem.set_value(<any>TenantWorkflowRepositoryTypeEnum[n]);
                this._cmbTenantWorkflowRepositoryType.get_items().add(cmbItem);
            }
        }
        this._tenantService.getTenants(searchDTO,
            (data) => {
                if (!data) return;
                this.rtvResult = data;
                this._rtvTenants.get_nodes().clear();
                let rtvNode: Telerik.Web.UI.RadTreeNode;
                rtvNode = new Telerik.Web.UI.RadTreeNode();
                rtvNode.set_text("AOO");
                this._rtvTenants.get_nodes().add(rtvNode);
                if (this.rtvResult.length === 0) {
                    $(`#${this.pnlDetailsId}`).hide();
                }
                else {
                    $(`#${this.pnlDetailsId}`).show();
                    var thisObj = this;
                    $.each(this.rtvResult, function (i, value: TenantViewModel) {
                        rtvNode = new Telerik.Web.UI.RadTreeNode();
                        let rtvNodeText: string = `${value.CompanyName} (${value.TenantName})`;
                        rtvNode.set_text(rtvNodeText);
                        rtvNode.set_value(value.UniqueId);
                        thisObj._rtvTenants.get_nodes().getNode(0).get_nodes().add(rtvNode);
                    });
                    if (this._rtvTenants.get_nodes().getNode(0).get_nodes().get_count() > 0) {
                        let node = this._rtvTenants.get_nodes().getNode(0).get_nodes().getNode(0);
                        node.select();
                        this.loadTenantDetails(node.get_value());
                        this.selectedTenant.UniqueId = node.get_value();
                    }
                    this._rtvTenants.get_nodes().getNode(0).expand();
                }
                this._loadingPanel.hide(this.splitterMainId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.splitterMainId);
                $("#".concat(this.rtvTenantsId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    btnTenantSelectorOk_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        if (this._dpTenantDateFrom && this._dpTenantDateFrom.get_selectedDate() &&
            this._txtTenantName && this._txtTenantName.get_textBoxValue() !== "" &&
            this._txtTenantCompany && this._txtTenantCompany.get_textBoxValue() !== "") {
            let tenant: TenantViewModel = {
                Configurations: [],
                Containers: [],
                PECMailBoxes: [],
                Roles: [],
                Contacts: [],
                TenantWorkflowRepositories: [],
                UniqueId: "",
                StartDate: this._dpTenantDateFrom.get_selectedDate(),
                EndDate: (this._dpTenantDateTo && this._dpTenantDateTo.get_selectedDate())
                    ? this._dpTenantDateTo.get_selectedDate()
                    : null,
                TenantName: this._txtTenantName.get_textBoxValue(),
                Note: this._txtTenantNote ? this._txtTenantNote.get_textBoxValue() : "",
                CompanyName: this._txtTenantCompany.get_textBoxValue(),
                Location: null,
                LastChangedDate: null,
                LastChangedUser: null,
                RegistrationDate: null,
                RegistrationUser: null
            };
            this._rwTenantSelector.close();
            this._loadingPanel.show(this.splitterMainId);
            let nodeValue = tenant.UniqueId;
            let nodeText = `${tenant.CompanyName} (${tenant.TenantName})`;
            let alreadySavedInTree: boolean = this.isTenantUpdate;
            if (!alreadySavedInTree) {
                tenant.UniqueId = "";
                this._tenantService.insertTenant(tenant,
                    (data) => {
                        let rtvNode: Telerik.Web.UI.RadTreeNode;
                        rtvNode = new Telerik.Web.UI.RadTreeNode();
                        rtvNode.set_text(nodeText);
                        rtvNode.set_value(data.UniqueId);
                        tenant.UniqueId = data.UniqueId;
                        this.rtvResult.push(tenant);
                        this._rtvTenants.get_nodes().getNode(0).get_nodes().add(rtvNode);
                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.splitterMainId);
                        $("#".concat(this.rtvTenantsId)).hide();
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
            } else {
                if (this._rtvTenants.get_selectedNode() !== null) {
                    tenant.UniqueId = this._rtvTenants.get_selectedNode().get_value();
                    let selectedTenant: TenantViewModel = this.rtvResult.filter(function (x) {
                        return x.UniqueId === tenant.UniqueId
                    })[0];
                    tenant.Configurations = selectedTenant.Configurations;
                    tenant.Containers = selectedTenant.Containers;
                    tenant.PECMailBoxes = selectedTenant.PECMailBoxes;
                    tenant.Roles = selectedTenant.Roles;
                    tenant.TenantWorkflowRepositories = selectedTenant.TenantWorkflowRepositories;
                    nodeValue = tenant.UniqueId;
                }
                this._tenantService.updateTenant(tenant,
                    (data) => {
                        let index = this.rtvResult.findIndex(x => x.UniqueId === tenant.UniqueId);
                        this.rtvResult[index] = tenant;
                        this._rtvTenants.get_selectedNode().set_text(nodeText);
                        this._rtvTenants.get_selectedNode().set_value(nodeValue);
                        this._lblCompanyNameId.innerText = tenant.CompanyName;
                        this._lblTenantNameId.innerText = tenant.TenantName;
                        this._lblTenantNoteId.innerText = tenant.Note;
                        this._lblTenantDataDiAttivazioneId.innerText = moment(tenant.StartDate).isValid() ? moment(tenant.StartDate).format("DD-MM-YYYY") : "";
                        this._lblTenantDataDiDisattivazioneId.innerText = moment(tenant.EndDate).isValid() ? moment(tenant.EndDate).format("DD-MM-YYYY") : "";
                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.splitterMainId);
                        $("#".concat(this.rtvTenantsId)).hide();
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
            }
            this._loadingPanel.hide(this.splitterMainId);
        }
    }

    btnTenantSelectorCancel_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this._rwTenantSelector.close();
    }

    btnTenantInsert_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this._dpTenantDateFrom.clear();
        this._txtTenantName.clear();
        this._txtTenantCompany.clear();
        this._txtTenantNote.clear();
        this.isTenantUpdate = false;
        this._dpTenantDateTo.get_element().parentElement.style.visibility = "hidden";
        this._rwTenantSelector.set_title("Aggiungi AOO");
        this._rwTenantSelector.show();
    }

    btnTenantUpdate_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        if (this._rtvTenants.get_selectedNode() !== null) {
            let thisObj = this;
            let tenant: TenantViewModel =
                this.rtvResult.filter(function (x) {
                    return x.UniqueId === thisObj._rtvTenants.get_selectedNode().get_value()
                })[0];
            this._dpTenantDateFrom.set_selectedDate(moment(tenant.StartDate).isValid() ? new Date(tenant.StartDate) : null);
            this._dpTenantDateTo.set_selectedDate(moment(tenant.EndDate).isValid() ? new Date(tenant.EndDate) : null);
            this._txtTenantName.set_value(tenant.TenantName);
            this._txtTenantCompany.set_value(tenant.CompanyName);
            this._txtTenantNote.set_value(tenant.Note);
            this.isTenantUpdate = true;
            this._dpTenantDateTo.get_element().parentElement.style.visibility = "visible";
            this._rwTenantSelector.set_title("Modifica AOO");
            this._rwTenantSelector.show();
        } else {
            alert("Selezionare un aziende");
        }
    }
    //endregion

    //region [ Add/Delete Containers from RadTreeView ]
    toolbarContainer_onClick = (sender: any, args: Telerik.Web.UI.RadToolBarCancelEventArgs) => {
        var btn = args.get_item();
        switch (btn.get_index()) {
            case 0:
                this._rwContainer.show();
                this._containerService.getContainers(null,
                    (data: any) => {
                        this.containers = <ContainerModel[]>data;
                        this.addContainers(this.containers, this._cmbContainer);
                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.splitterMainId);
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
                args.set_cancel(true);
                break;
            case 1:
                if (this._rtvContainers.get_selectedNode() !== null) {
                    let thisObj = this;
                    let tenant: TenantViewModel = this.rtvResult.filter(function (x) {
                        return x.UniqueId === thisObj._rtvTenants.get_selectedNode().get_value()
                    })[0];
                    let removeIndex = tenant.Containers.map(item => item.EntityShortId).indexOf(Number(this._rtvContainers.get_selectedNode().get_value()));
                    tenant.Containers.splice(removeIndex, 1);
                    this._tenantService.updateTenant(tenant,
                        (data) => {
                            this._rtvContainers.get_nodes().getNode(0).get_nodes().removeAt(this._rtvContainers.get_selectedNode().get_index());
                            if (this._rtvContainers.get_nodes().getNode(0).get_nodes().getItem(0) === undefined)
                                this._rtvContainers.get_nodes().clear();
                        },
                        (exception: ExceptionDTO) => {
                            this._loadingPanel.hide(this.splitterMainId);
                            $("#".concat(this.rtvTenantsId)).hide();
                            this.showNotificationException(this.uscNotificationId, exception);
                        });
                } else {
                    alert("Selezionare un Contentitore");
                }
                args.set_cancel(true);
                break;
        }
    }

    btnContainerOk_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        if (this._cmbContainer && this.selectedContainer && this.selectedTenant.UniqueId !== undefined) {
            this._rwContainer.close();
            this._loadingPanel.show(this.tbContainersControlId);
            let nodeImageUrl = "../App_Themes/DocSuite2008/imgset16/box_open.png";
            let nodeValue = this.selectedContainer.EntityShortId.toString();
            let nodeText = this.selectedContainer.Name;
            let alreadySavedInTree: boolean = this.alreadySavedInTree(nodeValue, this._rtvContainers);
            if (!alreadySavedInTree) {
                let thisObj = this;
                let tenant: TenantViewModel = this.rtvResult.filter(function (x) {
                    return x.UniqueId === thisObj._rtvTenants.get_selectedNode().get_value()
                })[0];
                tenant.Containers.push(this.selectedContainer);
                this._tenantService.updateTenant(tenant,
                    (data) => {
                        this.addNodesToRadTreeView(nodeValue, nodeText, "Contenitori", nodeImageUrl, this._rtvContainers);
                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.splitterMainId);
                        $("#".concat(this.rtvTenantsId)).hide();
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
            }
            this._loadingPanel.hide(this.tbContainersControlId);
        }
    }

    btnContainerCancel_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this._rwContainer.close();
    }

    protected addContainers(containers: ContainerModel[], cmbContainer: Telerik.Web.UI.RadComboBox) {
        this.containers = containers;
        cmbContainer.get_items().clear();
        let item: Telerik.Web.UI.RadComboBoxItem;
        item = new Telerik.Web.UI.RadComboBoxItem();
        item.set_text("");
        item.set_value("");
        cmbContainer.get_items().add(item);
        for (let container of containers) {
            item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text(container.Name);
            item.set_value(container.EntityShortId.toString());
            cmbContainer.get_items().add(item);
        }
    }

    cmbContainers_onClick = (sender: any, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        this.selectedContainer = this.containers.filter(function (x) {
            return x.EntityShortId.toString() === args.get_item().get_value()
        })[0];
    }

    _cmbContainer_OnClientItemsRequested = (sender: Telerik.Web.UI.RadComboBox,
        args: Telerik.Web.UI.RadComboBoxRequestEventArgs) => {
        let containerNumberOfItems: number = sender.get_items().get_count();
        this._containerService.getAllContainers(args.get_text(), this.maxNumberElements, containerNumberOfItems,
            (data: ODATAResponseModel<ContainerModel>) => {
                try {
                    this.refreshContainers(data.value);
                    let scrollToPosition: boolean = args.get_domEvent() == undefined;
                    if (scrollToPosition) {
                        if (sender.get_items().get_count() > 0) {
                            let scrollContainer: JQuery = $(sender.get_dropDownElement()).find('div.rcbScroll');
                            scrollContainer.scrollTop($(sender.get_items().getItem(containerNumberOfItems + 1).get_element()).position().top);
                        }
                    }
                    sender.get_attributes().setAttribute('otherContainerCount', data.count.toString());
                    sender.get_attributes().setAttribute('updating', 'false');
                    if (sender.get_items().get_count() > 0) {
                        containerNumberOfItems = sender.get_items().get_count() - 1;
                    }
                    this._cmbContainer.get_moreResultsBoxMessageElement().innerText = `Visualizzati ${containerNumberOfItems.toString()} di ${data.count.toString()}`;
                }
                catch (error) {
                }
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    refreshContainers = (data: ContainerModel[]) => {
        if (data.length > 0) {
            this._cmbContainer.beginUpdate();
            if (this._cmbContainer.get_items().get_count() === 0) {
                let emptyItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                emptyItem.set_text("");
                emptyItem.set_value("");
                this._cmbContainer.get_items().insert(0, emptyItem);
            }

            $.each(data, (index, container) => {
                let item: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(container.Name);
                item.set_value(container.EntityShortId.toString());
                this._cmbContainer.get_items().add(item);
                this.containers.push(container);
            });
            this._cmbContainer.showDropDown();
            this._cmbContainer.endUpdate();
        }
        else {
            if (this._cmbContainer.get_items().get_count() === 0) {
            }

        }
    }

    private populateContainersTreeView(tenant: TenantViewModel) {
        this._tenantService.getTenantContainers(tenant.UniqueId,
            (data: ContainerModel[]) => {
                if (data === undefined) {
                    return;
                } else {
                    this._rtvContainers.get_nodes().clear();
                    let thisObj = this;
                    tenant.Containers = data;
                    $.each(data, function (i, value) {
                        let nodeImageUrl = "../App_Themes/DocSuite2008/imgset16/box_open.png";
                        let nodeValue = value.EntityShortId.toString();
                        let nodeText = value.Name;
                        let alreadySavedInTree: boolean = thisObj.alreadySavedInTree(nodeValue, thisObj._rtvContainers);
                        if (!alreadySavedInTree) {
                            thisObj.addNodesToRadTreeView(nodeValue, nodeText, "Contenitori", nodeImageUrl, thisObj._rtvContainers);
                        }
                    });
                }
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.splitterMainId);
                $("#".concat(this.rtvTenantsId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    _rwContainer_OnShow = (sender: Telerik.Web.UI.RadWindow, args: Sys.EventArgs) => {
        this._cmbContainer.clearSelection();
        this.selectedContainer = null;
    }
    //endregion

    //region [ Add/Delete PECMailBoxes from RadTreeView ]
    toolbarPECMailBox_onClick = (sender: any, args: Telerik.Web.UI.RadToolBarCancelEventArgs) => {
        var btn = args.get_item();
        switch (btn.get_index()) {
            case 0:
                this._rwPECMailBox.show();
                this._pecMailBoxService.getPECMailBoxes("",
                    (data: any) => {
                        this.pecMailBoxes = <PECMailBoxModel[]>data;
                        this.addPECMailBoxes(this.pecMailBoxes, this._cmbPECMailBox);
                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.splitterMainId);
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
                args.set_cancel(true);
                break;
            case 1:
                if (this._rtvPECMailBoxes.get_selectedNode() !== null) {
                    let thisObj = this;
                    let tenant: TenantViewModel = this.rtvResult.filter(function (x) {
                        return x.UniqueId === thisObj._rtvTenants.get_selectedNode().get_value()
                    })[0];
                    let removeIndex = tenant.PECMailBoxes.map(item => item.EntityShortId).indexOf(Number(this._rtvPECMailBoxes.get_selectedNode().get_value()));
                    tenant.PECMailBoxes.splice(removeIndex, 1);
                    this._tenantService.updateTenant(tenant,
                        (data) => {
                            this._rtvPECMailBoxes.get_nodes().getNode(0).get_nodes().removeAt(this._rtvPECMailBoxes.get_selectedNode().get_index());
                            if (this._rtvPECMailBoxes.get_nodes().getNode(0).get_nodes().getItem(0) === undefined)
                                this._rtvPECMailBoxes.get_nodes().clear();
                        },
                        (exception: ExceptionDTO) => {
                            this._loadingPanel.hide(this.splitterMainId);
                            $("#".concat(this.rtvTenantsId)).hide();
                            this.showNotificationException(this.uscNotificationId, exception);
                        });
                } else {
                    alert("Selezionare una caselle PEC");
                }
                args.set_cancel(true);
                break;
        }
    }

    btnPECMailBoxOk_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        if (this._cmbPECMailBox && this.selectedPECMailBox && this.selectedTenant.UniqueId !== undefined) {
            this._rwPECMailBox.close();
            this._loadingPanel.show(this.tbPECMailBoxesControlId);
            let nodeImageUrl = "../App_Themes/DocSuite2008/imgset16/box_open.png";
            let nodeValue = this.selectedPECMailBox.EntityShortId.toString();
            let nodeText = this.selectedPECMailBox.MailBoxRecipient;
            let alreadySavedInTree: boolean = this.alreadySavedInTree(nodeValue, this._rtvPECMailBoxes);
            if (!alreadySavedInTree) {
                let thisObj = this;
                let tenant: TenantViewModel = this.rtvResult.filter(function (x) {
                    return x.UniqueId === thisObj._rtvTenants.get_selectedNode().get_value()
                })[0];
                tenant.PECMailBoxes.push(this.selectedPECMailBox);
                this._tenantService.updateTenant(tenant,
                    (data) => {
                        this.addNodesToRadTreeView(nodeValue, nodeText, "Caselle PEC", nodeImageUrl, this._rtvPECMailBoxes);
                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.splitterMainId);
                        $("#".concat(this.rtvTenantsId)).hide();
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
            }
            this._loadingPanel.hide(this.tbPECMailBoxesControlId);
        }
    }

    btnPECMailBoxCancel_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this._rwPECMailBox.close();
    }

    protected addPECMailBoxes(pecMailBoxes: PECMailBoxModel[], cmbPECMailBox: Telerik.Web.UI.RadComboBox) {
        this.pecMailBoxes = pecMailBoxes;
        cmbPECMailBox.get_items().clear();
        let item: Telerik.Web.UI.RadComboBoxItem;
        item = new Telerik.Web.UI.RadComboBoxItem();
        item.set_text("");
        item.set_value("");
        cmbPECMailBox.get_items().add(item);
        for (let pecMailBox of pecMailBoxes) {
            item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text(pecMailBox.MailBoxRecipient);
            item.set_value(pecMailBox.EntityShortId.toString());
            cmbPECMailBox.get_items().add(item);
        }
    }

    cmbPECMailBoxes_onClick = (sender: any, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        this.selectedPECMailBox = this.pecMailBoxes.filter(function (x) {
            return x.EntityShortId.toString() === args.get_item().get_value()
        })[0];
    }

    _cmbPECMailBox_OnClientItemsRequested = (sender: Telerik.Web.UI.RadComboBox,
        args: Telerik.Web.UI.RadComboBoxRequestEventArgs) => {
        let pecMailBoxNumberOfItems: number = sender.get_items().get_count();
        this._pecMailBoxService.getAllPECMailBoxes(args.get_text(), this.maxNumberElements, pecMailBoxNumberOfItems,
            (data: ODATAResponseModel<PECMailBoxModel>) => {
                try {
                    this.refreshPECMailBoxes(data.value);
                    let scrollToPosition: boolean = args.get_domEvent() == undefined;
                    if (scrollToPosition) {
                        if (sender.get_items().get_count() > 0) {
                            let scrollContainer: JQuery = $(sender.get_dropDownElement()).find('div.rcbScroll');
                            scrollContainer.scrollTop($(sender.get_items().getItem(pecMailBoxNumberOfItems + 1).get_element()).position().top);
                        }
                    }
                    sender.get_attributes().setAttribute('otherContainerCount', data.count.toString());
                    sender.get_attributes().setAttribute('updating', 'false');
                    if (sender.get_items().get_count() > 0) {
                        pecMailBoxNumberOfItems = sender.get_items().get_count() - 1;
                    }
                    this._cmbPECMailBox.get_moreResultsBoxMessageElement().innerText = `Visualizzati ${pecMailBoxNumberOfItems.toString()} di ${data.count.toString()}`;
                }
                catch (error) {
                }
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    refreshPECMailBoxes = (data: PECMailBoxModel[]) => {
        if (data.length > 0) {
            this._cmbPECMailBox.beginUpdate();
            if (this._cmbPECMailBox.get_items().get_count() === 0) {
                let emptyItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                emptyItem.set_text("");
                emptyItem.set_value("");
                this._cmbPECMailBox.get_items().insert(0, emptyItem);
            }

            $.each(data, (index, pecMailBox) => {
                let item: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(pecMailBox.MailBoxRecipient);
                item.set_value(pecMailBox.EntityShortId.toString());
                this._cmbPECMailBox.get_items().add(item);
                this.pecMailBoxes.push(pecMailBox);
            });
            this._cmbPECMailBox.showDropDown();
            this._cmbPECMailBox.endUpdate();
        }
        else {
            if (this._cmbPECMailBox.get_items().get_count() === 0) {
            }

        }
    }

    private populatePECMailBoxesTreeView(tenant: TenantViewModel) {
        this._tenantService.getTenantPECMailBoxes(tenant.UniqueId,
            (data: PECMailBoxModel[]) => {
                if (data === undefined) {
                    return;
                } else {
                    this._rtvPECMailBoxes.get_nodes().clear();
                    let thisObj = this;
                    tenant.PECMailBoxes = data;
                    $.each(data, function (i, value) {
                        let nodeImageUrl = "../App_Themes/DocSuite2008/imgset16/box_open.png";
                        let nodeValue = value.EntityShortId.toString();
                        let nodeText = value.MailBoxRecipient;
                        let alreadySavedInTree: boolean = thisObj.alreadySavedInTree(nodeValue, thisObj._rtvPECMailBoxes);
                        if (!alreadySavedInTree) {
                            thisObj.addNodesToRadTreeView(nodeValue, nodeText, "Caselle PEC", nodeImageUrl, thisObj._rtvPECMailBoxes);
                        }
                    });
                }
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.splitterMainId);
                $("#".concat(this.rtvTenantsId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    _rwPECMailBox_OnShow = (sender: Telerik.Web.UI.RadWindow, args: Sys.EventArgs) => {
        this._cmbPECMailBox.clearSelection();
        this.selectedPECMailBox = null;
    }
    //endregion

    //region [ Add/Delete WorkflowRepositories from RadTreeView ]
    toolbarWorkflowRepository_onClick = (sender: any, args: Telerik.Web.UI.RadToolBarCancelEventArgs) => {
        var btn = args.get_item();
        switch (btn.get_index()) {
            case 0:
                this._dpTenantWorkflowRepositoryDateFrom.clear();
                this._dpTenantWorkflowRepositoryDateTo.clear();
                this.currentTenantWorkflowRepositoryUniqueId = "";
                this._txtTenantWorkflowRepositoryJsonValue.clear();
                this._txtTenantWorkflowRepositoryIntegrationModuleName.clear();
                this._txtTenantWorkflowRepositoryConditions.clear();
                this._cmbTenantWorkflowRepositoryType.set_selectedIndex(0);
                if (this._rtvTenantWorkflowRepositories.get_selectedNode()) {
                    this._rtvTenantWorkflowRepositories.get_selectedNode().set_selected(false);
                    this.selectedTenantWorkflowRepository = undefined;
                }
                this._rwWorkflowRepository.show();
                args.set_cancel(true);
                break;
            case 1:
                if (this._rtvTenantWorkflowRepositories.get_selectedNode() &&
                    this._rtvTenantWorkflowRepositories.get_selectedNode() !== this._rtvTenantWorkflowRepositories.get_nodes().getNode(0)) {
                    let uniqueId = this._rtvTenantWorkflowRepositories.get_selectedNode().get_value();
                    this._tenantWorkflowRepositoryService.getTenantWorkflowRepositoryById(uniqueId,
                        (data) => {
                            let editIndex = data.map(item => item.UniqueId)
                                .indexOf(this._rtvTenantWorkflowRepositories.get_selectedNode().get_value());
                            this._rwWorkflowRepository.show();
                            var cmbWorkflowRepositoryItem =
                                this._cmbWorkflowRepository.findItemByValue(
                                    data[editIndex].WorkflowRepository.UniqueId);
                            cmbWorkflowRepositoryItem.select();
                            this._dpTenantWorkflowRepositoryDateFrom.set_selectedDate(new Date(data[editIndex].StartDate));
                            if (data[editIndex].EndDate && data[editIndex].EndDate !== "")
                                this._dpTenantWorkflowRepositoryDateTo.set_selectedDate(new Date(data[editIndex].EndDate));
                            else
                                this._dpTenantWorkflowRepositoryDateTo.clear();
                            this._txtTenantWorkflowRepositoryJsonValue.set_value(data[editIndex].JsonValue);
                            this._txtTenantWorkflowRepositoryIntegrationModuleName.set_value(data[editIndex].IntegrationModuleName);
                            this._txtTenantWorkflowRepositoryConditions.set_value(data[editIndex].Conditions);
                            var cmbTenantWorkflowRepositoryTypeItem =
                                this._cmbTenantWorkflowRepositoryType.findItemByValue(
                                    data[editIndex].ConfigurationType);
                            cmbTenantWorkflowRepositoryTypeItem.select();
                            this.currentTenantWorkflowRepositoryUniqueId = data[editIndex].UniqueId;
                        },
                        (exception: ExceptionDTO) => {
                            this._loadingPanel.hide(this.splitterMainId);
                            $("#".concat(this.rtvTenantsId)).hide();
                            this.showNotificationException(this.uscNotificationId, exception);
                        });
                } else {
                    alert("Selezionare una attività ");
                }
                args.set_cancel(true);
                break;
            case 2:
                if (this._rtvTenantWorkflowRepositories.get_selectedNode() !== null) {
                    let thisObj = this;
                    let tenant: TenantViewModel = this.rtvResult.filter(function (x) {
                        return x.UniqueId === thisObj._rtvTenants.get_selectedNode().get_value()
                    })[0];
                    let removeIndex = tenant.TenantWorkflowRepositories.map(item => item.UniqueId).indexOf(this._rtvTenantWorkflowRepositories.get_selectedNode().get_value());
                    this._tenantWorkflowRepositoryService.deleteTenantWorkflowRepository(tenant.TenantWorkflowRepositories[removeIndex],
                        (data) => {
                            tenant.TenantWorkflowRepositories.splice(removeIndex, 1);
                            this._rtvTenantWorkflowRepositories.get_nodes().getNode(0).get_nodes().removeAt(this._rtvTenantWorkflowRepositories.get_selectedNode().get_index());
                            if (this._rtvTenantWorkflowRepositories.get_nodes().getNode(0).get_nodes().getItem(0) === undefined)
                                this._rtvTenantWorkflowRepositories.get_nodes().clear();
                        },
                        (exception: ExceptionDTO) => {
                            this._loadingPanel.hide(this.splitterMainId);
                            $("#".concat(this.rtvTenantsId)).hide();
                            this.showNotificationException(this.uscNotificationId, exception);
                        });
                } else {
                    alert("Selezionare una attività");
                }
                args.set_cancel(true);
                break;
        }
    }

    btnWorkflowRepositoryOk_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        if (this._cmbWorkflowRepository && this.selectedWorkflowRepository && this.selectedTenant.UniqueId !== undefined &&
            this._txtTenantWorkflowRepositoryJsonValue && this._txtTenantWorkflowRepositoryJsonValue.get_textBoxValue() !== "" &&
            this._cmbTenantWorkflowRepositoryType && this._cmbTenantWorkflowRepositoryType.get_selectedItem().get_text() !== "" &&
            this._dpTenantWorkflowRepositoryDateFrom && this._dpTenantWorkflowRepositoryDateFrom.get_selectedDate()) {

            this._loadingPanel.show(this.tbWorkflowRepositoryControlId);
            let nodeImageUrl = "../App_Themes/DocSuite2008/imgset16/box_open.png";
            let nodeValue = this.selectedTenantWorkflowRepository !== undefined
                ? this.selectedTenantWorkflowRepository.UniqueId
                : "";
            let nodeText = this._cmbWorkflowRepository.get_selectedItem().get_text();
            let viewModelMapper = new TenantViewModelMapper();
            let thisObj = this;
            let tenant: TenantViewModel = this.rtvResult.filter(function (x) {
                return x.UniqueId === thisObj._rtvTenants.get_selectedNode().get_value()
            })[0];
            let tenantWorkflowRepository: TenantWorkflowRepositoryModel = {
                Tenant: viewModelMapper.Map(tenant),
                WorkflowRepository: this.selectedWorkflowRepository,
                JsonValue: this._txtTenantWorkflowRepositoryJsonValue ? this._txtTenantWorkflowRepositoryJsonValue.get_textBoxValue() : "",
                IntegrationModuleName: this._txtTenantWorkflowRepositoryIntegrationModuleName ? this._txtTenantWorkflowRepositoryIntegrationModuleName.get_textBoxValue() : "",
                Conditions: this._txtTenantWorkflowRepositoryConditions ? this._txtTenantWorkflowRepositoryConditions.get_textBoxValue() : "",
                ConfigurationType: this._cmbTenantWorkflowRepositoryType.get_selectedItem().get_value(),
                EndDate: (this._dpTenantWorkflowRepositoryDateTo && this._dpTenantWorkflowRepositoryDateTo.get_selectedDate())
                    ? moment(this._dpTenantWorkflowRepositoryDateTo.get_selectedDate()).format("MM-DD-YYYY")
                    : "",
                StartDate: (this._dpTenantWorkflowRepositoryDateFrom && this._dpTenantWorkflowRepositoryDateFrom.get_selectedDate())
                    ? moment(this._dpTenantWorkflowRepositoryDateFrom.get_selectedDate()).format("MM-DD-YYYY")
                    : "",
                LastChangedDate: null,
                LastChangedUser: null,
                RegistrationDate: null,
                RegistrationUser: null,
                UniqueId: this.currentTenantWorkflowRepositoryUniqueId
            };
            let alreadySavedInTree: boolean = this.alreadySavedInTree(nodeValue, this._rtvTenantWorkflowRepositories);
            if (!alreadySavedInTree) {
                if (tenant.TenantWorkflowRepositories.length === 0 ||
                    tenant.TenantWorkflowRepositories.filter(function (x) {
                        return x.WorkflowRepository.Name !== this._cmbWorkflowRepository.get_selectedItem().get_text()
                    })[0])
                    this._tenantWorkflowRepositoryService.insertTenantWorkflowRepository(tenantWorkflowRepository,
                        (data) => {
                            nodeValue = data.UniqueId;
                            this.addNodesToRadTreeView(nodeValue, nodeText, "Attività", nodeImageUrl, this._rtvTenantWorkflowRepositories);
                            tenant.TenantWorkflowRepositories.push(data);
                            tenant.TenantWorkflowRepositories[tenant.TenantWorkflowRepositories.length - 1].ConfigurationType =
                                <any>TenantWorkflowRepositoryTypeEnum[data.ConfigurationType];
                            this._rwWorkflowRepository.close();
                        },
                        (exception: ExceptionDTO) => {
                            this._loadingPanel.hide(this.splitterMainId);
                            $("#".concat(this.rtvTenantsId)).hide();
                            this.showNotificationException(this.uscNotificationId, exception);
                        });
            }
            else {
                let existsInTree_cmbSelectedExcluded: boolean = false;
                for (let i = 0; i < this._rtvTenantWorkflowRepositories.get_nodes().getNode(0).get_nodes().get_count(); i++) {
                    if (this._cmbWorkflowRepository.get_selectedItem().get_text() === this._rtvTenantWorkflowRepositories.get_nodes().getNode(0).get_nodes().getItem(i).get_text() &&
                        this._cmbWorkflowRepository.get_selectedItem().get_text() !== this._rtvTenantWorkflowRepositories.get_selectedNode().get_text())
                        existsInTree_cmbSelectedExcluded = true;
                }
                if (!existsInTree_cmbSelectedExcluded)
                    this._tenantWorkflowRepositoryService.updateTenantWorkflowRepository(tenantWorkflowRepository,
                        (data) => {
                            tenant.TenantWorkflowRepositories.filter(function (x) {
                                return x.UniqueId === this._rtvTenantWorkflowRepositories.get_selectedNode().get_value()
                            })[0].WorkflowRepository = data.WorkflowRepository;
                            this._rtvTenantWorkflowRepositories.get_selectedNode().set_text(nodeText);
                            this._rtvTenantWorkflowRepositories.get_selectedNode().set_value(nodeValue);
                            this._rwWorkflowRepository.close();
                        },
                        (exception: ExceptionDTO) => {
                            this._loadingPanel.hide(this.splitterMainId);
                            $("#".concat(this.rtvTenantsId)).hide();
                            this.showNotificationException(this.uscNotificationId, exception);
                        });
            }
            this._loadingPanel.hide(this.tbWorkflowRepositoryControlId);
        }
    }

    rtvTenantWorkflowrepositories_onNodeClick = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        this.selectedTenantWorkflowRepository = this.tenantWorkflowRepositories.filter(function (x) {
            return x.UniqueId === args.get_node().get_value()
        })[0];
    }

    btnWorkflowRepositoryCancel_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this._rwWorkflowRepository.close();
    }

    protected addWorkflowRepositories(workflowRepositories: WorkflowRepositoryModel[], cmbWorkflowRepository: Telerik.Web.UI.RadComboBox) {
        this.workflowRepositories = workflowRepositories;
        cmbWorkflowRepository.get_items().clear();
        let item: Telerik.Web.UI.RadComboBoxItem;
        item = new Telerik.Web.UI.RadComboBoxItem();
        item.set_text("");
        item.set_value("");
        cmbWorkflowRepository.get_items().add(item);
        for (let workflowRepository of workflowRepositories) {
            item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text(workflowRepository.Name);
            item.set_value(workflowRepository.UniqueId);
            cmbWorkflowRepository.get_items().add(item);
        }
    }

    cmbWorkflowRepositories_onClick = (sender: any, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        this.selectedWorkflowRepository = this.workflowRepositories.filter(function (x) {
            return x.UniqueId === args.get_item().get_value()
        })[0];
    }

    _cmbWorkflowRepository_OnClientItemsRequested = (sender: Telerik.Web.UI.RadComboBox,
        args: Telerik.Web.UI.RadComboBoxRequestEventArgs) => {
        let workflowRepositoryNumberOfItems: number = sender.get_items().get_count();
        this._workflowRepositoryService.getAllWorkflowRepositories(args.get_text(), this.maxNumberElements, workflowRepositoryNumberOfItems,
            (data: ODATAResponseModel<WorkflowRepositoryModel>) => {
                try {
                    this.refreshWorkflowRepositories(data.value);
                    let scrollToPosition: boolean = args.get_domEvent() == undefined;
                    if (scrollToPosition) {
                        if (sender.get_items().get_count() > 0) {
                            let scrollContainer: JQuery = $(sender.get_dropDownElement()).find('div.rcbScroll');
                            scrollContainer.scrollTop($(sender.get_items().getItem(workflowRepositoryNumberOfItems + 1).get_element()).position().top);
                        }
                    }
                    sender.get_attributes().setAttribute('otherContainerCount', data.count.toString());
                    sender.get_attributes().setAttribute('updating', 'false');
                    if (sender.get_items().get_count() > 0) {
                        workflowRepositoryNumberOfItems = sender.get_items().get_count() - 1;
                    }
                    this._cmbWorkflowRepository.get_moreResultsBoxMessageElement().innerText = `Visualizzati ${workflowRepositoryNumberOfItems.toString()} di ${data.count.toString()}`;
                }
                catch (error) {
                }
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    refreshWorkflowRepositories = (data: WorkflowRepositoryModel[]) => {
        if (data.length > 0) {
            this._cmbWorkflowRepository.beginUpdate();
            if (this._cmbWorkflowRepository.get_items().get_count() === 0) {
                let emptyItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                emptyItem.set_text("");
                emptyItem.set_value("");
                this._cmbWorkflowRepository.get_items().insert(0, emptyItem);
            }

            $.each(data, (index, workflowRepository) => {
                let item: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(workflowRepository.Name);
                item.set_value(workflowRepository.UniqueId);
                this._cmbWorkflowRepository.get_items().add(item);
                this.workflowRepositories.push(workflowRepository);
            });
            this._cmbWorkflowRepository.showDropDown();
            this._cmbWorkflowRepository.endUpdate();
        }
        else {
            if (this._cmbWorkflowRepository.get_items().get_count() === 0) {
            }

        }
    }

    private populateTenantWorkflowRepositoriesTreeView(tenant: TenantViewModel) {
        this._tenantService.getTenantWorkflowRepositories(tenant.UniqueId,
            (data: TenantWorkflowRepositoryModel[]) => {
                if (data === undefined) {
                    return;
                } else {
                    this._rtvTenantWorkflowRepositories.get_nodes().clear();
                    let thisObj = this;
                    tenant.TenantWorkflowRepositories = data;
                    this.tenantWorkflowRepositories = tenant.TenantWorkflowRepositories;
                    $.each(data, function (i, value) {
                        let nodeImageUrl = "../App_Themes/DocSuite2008/imgset16/box_open.png";
                        let nodeValue = value.UniqueId;
                        let nodeText = value.WorkflowRepository.Name;
                        let alreadySavedInTree: boolean = thisObj.alreadySavedInTree(nodeValue, thisObj._rtvTenantWorkflowRepositories);
                        if (!alreadySavedInTree) {
                            thisObj.addNodesToRadTreeView(nodeValue, nodeText, "Attività", nodeImageUrl, thisObj._rtvTenantWorkflowRepositories);
                        }
                    });
                }
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.splitterMainId);
                $("#".concat(this.rtvTenantsId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    private populateWorkflowRepositoryComboBox() {
        this._workflowRepositoryService.getWorkflowRepositories(
            (data: any) => {
                this.addWorkflowRepositories(data, this._cmbWorkflowRepository);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.splitterMainId);
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    _rwWorkflowRepository_OnShow = (sender: Telerik.Web.UI.RadWindow, args: Sys.EventArgs) => {
        this._cmbWorkflowRepository.clearSelection();
        this.selectedWorkflowRepository = null;
    }
    //endregion

    //region [ Add/Update/Delete TenantConfigurations from RadTreeView ]
    toolbarConfiguration_onClick = (sender: any, args: Telerik.Web.UI.RadToolBarCancelEventArgs) => {
        var btn = args.get_item();
        switch (btn.get_index()) {
            case 0:
                this._dpStartDateFrom.clear();
                this._dpEndDateFrom.clear();
                this._txtTenantConfigurationNote.clear();
                this._txtTenantConfigurationJsonValue.clear();
                this._cmbConfigurationType.set_selectedIndex(0);
                this.currentTenantConfigurationUniqueId = "";
                this._rwTenantConfiguration.show();
                args.set_cancel(true);
                break;
            case 1:
                if (this._rtvTenantConfigurations.get_selectedNode() !== null) {
                    let thisObj = this;
                    let tenant: TenantViewModel =
                        this.rtvResult.filter(function (x) {
                            return x.UniqueId === thisObj._rtvTenants.get_selectedNode().get_value()
                        })[0];
                    let editIndex = tenant.Configurations.map(item => item.UniqueId)
                        .indexOf(this._rtvTenantConfigurations.get_selectedNode().get_value());
                    this._dpStartDateFrom.set_selectedDate(new Date(tenant.Configurations[editIndex].StartDate));
                    if (tenant.Configurations[editIndex].EndDate && tenant.Configurations[editIndex].EndDate !== "")
                        this._dpEndDateFrom.set_selectedDate(new Date(tenant.Configurations[editIndex].EndDate));
                    else
                        this._dpEndDateFrom.clear();
                    this._txtTenantConfigurationNote.set_value(tenant.Configurations[editIndex].Note);
                    this._txtTenantConfigurationJsonValue.set_value(tenant.Configurations[editIndex].JsonValue);
                    var item = this._cmbConfigurationType.findItemByValue(tenant.Configurations[editIndex].ConfigurationType);
                    item.select();

                    this.currentTenantConfigurationUniqueId = tenant.Configurations[editIndex].UniqueId;
                    this._rwTenantConfiguration.show();
                } else {
                    alert("Selezionare un configurazione");
                }
                args.set_cancel(true);
                break;
            case 2:
                if (this._rtvTenantConfigurations.get_selectedNode() !== null) {
                    let thisObj = this;
                    let tenant: TenantViewModel = this.rtvResult.filter(function (x) {
                        return x.UniqueId === thisObj._rtvTenants.get_selectedNode().get_value()
                    })[0];
                    let removeIndex = tenant.Configurations.map(item => item.UniqueId).indexOf(this._rtvTenantConfigurations.get_selectedNode().get_value());
                    tenant.Configurations.splice(removeIndex, 1);
                    this._tenantService.updateTenant(tenant,
                        (data) => {
                            this._rtvTenantConfigurations.get_nodes().getNode(0).get_nodes().removeAt(this._rtvTenantConfigurations.get_selectedNode().get_index());
                            if (this._rtvTenantConfigurations.get_nodes().getNode(0).get_nodes().getItem(0) === undefined)
                                this._rtvTenantConfigurations.get_nodes().clear();
                        },
                        (exception: ExceptionDTO) => {
                            this._loadingPanel.hide(this.splitterMainId);
                            $("#".concat(this.rtvTenantsId)).hide();
                            this.showNotificationException(this.uscNotificationId, exception);
                        });
                } else {
                    alert("Selezionare un configurazione");
                }
                args.set_cancel(true);
                break;
        }
    }

    btnTenantConfigurationOk_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        if (this._dpStartDateFrom && this._dpStartDateFrom.get_selectedDate() &&
            this._txtTenantConfigurationJsonValue && this._txtTenantConfigurationJsonValue.get_textBoxValue() !== "" &&
            this._cmbConfigurationType && this._cmbConfigurationType.get_selectedItem().get_text() !== "") {
            let viewModelMapper = new TenantViewModelMapper();
            let thisObj = this;
            let tenant: TenantViewModel = this.rtvResult.filter(function (x) {
                return x.UniqueId === thisObj._rtvTenants.get_selectedNode().get_value()
            })[0];
            let tenantConfiguration: TenantConfigurationModel = {
                Tenant: viewModelMapper.Map(tenant),
                ConfigurationType: this._cmbConfigurationType.get_selectedItem().get_value(),
                EndDate: (this._dpEndDateFrom && this._dpEndDateFrom.get_selectedDate())
                    ? moment(this._dpEndDateFrom.get_selectedDate()).format("MM-DD-YYYY")
                    : "",
                StartDate: (this._dpStartDateFrom && this._dpStartDateFrom.get_selectedDate())
                    ? moment(this._dpStartDateFrom.get_selectedDate()).format("MM-DD-YYYY")
                    : "",
                JsonValue: this._txtTenantConfigurationJsonValue ? this._txtTenantConfigurationJsonValue.get_textBoxValue() : "",
                Note: this._txtTenantConfigurationNote ? this._txtTenantConfigurationNote.get_textBoxValue() : "",
                UniqueId: this.currentTenantConfigurationUniqueId
            };
            this._rwTenantConfiguration.close();
            this._loadingPanel.show(this.tbConfigurationControlId);
            let nodeImageUrl = "../App_Themes/DocSuite2008/imgset16/box_open.png";
            let nodeValue = tenantConfiguration.UniqueId;
            let nodeText = this._cmbConfigurationType.get_selectedItem().get_text();
            let alreadySavedInTree: boolean = this.alreadySavedInTree(nodeValue, this._rtvTenantConfigurations);
            if (!alreadySavedInTree) {
                tenant.Configurations.push(tenantConfiguration);
                this._tenantService.updateTenant(tenant,
                    (data) => {
                        let selectedIndex = tenant.Configurations.map(item => item.UniqueId)
                            .indexOf(tenantConfiguration.UniqueId);
                        tenant.Configurations[selectedIndex].UniqueId = data.Configurations.$values[selectedIndex].UniqueId;
                        nodeValue = tenant.Configurations[selectedIndex].UniqueId;
                        this.addNodesToRadTreeView(nodeValue, nodeText, "Configurazioni", nodeImageUrl, this._rtvTenantConfigurations);
                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.splitterMainId);
                        $("#".concat(this.rtvTenantsId)).hide();
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
            } else {
                let editIndex = tenant.Configurations.map(item => item.UniqueId)
                    .indexOf(this._rtvTenantConfigurations.get_selectedNode().get_value());
                tenant.Configurations[editIndex] = tenantConfiguration;
                this._tenantConfigurationService.updateTenantConfiguration(tenantConfiguration,
                    (data) => {
                        this._rtvTenantConfigurations.get_selectedNode().set_text(nodeText);
                        this._rtvTenantConfigurations.get_selectedNode().set_value(nodeValue);
                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.splitterMainId);
                        $("#".concat(this.rtvTenantsId)).hide();
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
            }
            this._loadingPanel.hide(this.tbConfigurationControlId);
        }
    }

    btnTenantConfigurationCancel_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this._rwTenantConfiguration.close();
    }

    private populateTenantConfigurationsTreeView(tenant: TenantViewModel) {
        this._tenantService.getTenantConfigurations(tenant.UniqueId,
            (data: TenantConfigurationModel[]) => {
                if (data === undefined) {
                    return;
                } else {
                    this._rtvTenantConfigurations.get_nodes().clear();
                    let thisObj = this;
                    tenant.Configurations = data;
                    $.each(data, function (i, value) {
                        let nodeImageUrl = "../App_Themes/DocSuite2008/imgset16/box_open.png";
                        let nodeValue = value.UniqueId;
                        let nodeText = thisObj._enumHelper.getTenantConfigurationTypeDescription(value.ConfigurationType);
                        let alreadySavedInTree: boolean = thisObj.alreadySavedInTree(nodeValue, thisObj._rtvTenantConfigurations);
                        if (!alreadySavedInTree) {
                            thisObj.addNodesToRadTreeView(nodeValue, nodeText, "Configurazioni", nodeImageUrl, thisObj._rtvTenantConfigurations);
                        }
                    });
                }
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.splitterMainId);
                $("#".concat(this.rtvTenantsId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }
    //endregion


    //region [Add/Delete TenantContact]

    private registerUscContattiRestEventHandlers(): void {
        let uscContattiSelRestEvents = this._uscContattiSelRest.uscContattiSelRestEvents;

        this._uscContattiSelRest.registerEventHandler(uscContattiSelRestEvents.ContactDeleted, this.deleteTenantContactPromise);
        this._uscContattiSelRest.registerEventHandler(uscContattiSelRestEvents.NewContactsAdded, this.updateTenantContactPromise);
    }

    private deleteTenantContactPromise = (contactId: number): JQueryPromise<any> => {
        let promise: JQueryDeferred<any> = $.Deferred<any>();

        if (contactId) {
            let tenant: TenantViewModel = this.rtvResult.filter((x) => {
                return x.UniqueId === this._rtvTenants.get_selectedNode().get_value()
            })[0];
            let contactParent = tenant.Contacts.filter(contact => contact.EntityId === contactId)[0];
            let contactParentId = null;
            if (contactParent) {
                contactParentId = contactParent.IncrementalFather;
            }
            tenant.Contacts = tenant.Contacts.filter(contact => contact.EntityId !== contactId && contact.IncrementalFather !== contactId);
            this._tenantService.updateTenant(tenant,
                (data: any) => {
                    promise.resolve(contactParentId);
                    this._loadingPanel.hide(this.splitterMainId);
                },
                (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.splitterMainId);
                    $("#".concat(this.rtvTenantsId)).hide();
                    this.showNotificationException(this.uscNotificationId, exception);
                });
        } 
        return promise.promise();
    }

    private updateTenantContactPromise = (newContactAdded: ContactModel): JQueryPromise<any> => {
        let promise: JQueryDeferred<any> = $.Deferred<any>();
        if (newContactAdded) {
            let tenant: TenantViewModel = this.rtvResult.filter((x) => {
                return x.UniqueId === this._rtvTenants.get_selectedNode().get_value()
            })[0];
            tenant.Contacts.push(newContactAdded);

            this._loadingPanel.show(this.splitterMainId);
            this._tenantService.updateTenant(tenant,
                (data: any) => {
                    promise.resolve(data);
                    this._loadingPanel.hide(this.splitterMainId);
                },
                (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.splitterMainId);
                    $("#".concat(this.rtvTenantsId)).hide();
                    this.showNotificationException(this.uscNotificationId, exception);
                });
        }
        return promise.promise();
    }


    private populateContactTreeView(tenant: TenantViewModel) {
        this._loadingPanel.show(this.splitterMainId);
        this._tenantService.getTenantContacts(tenant.UniqueId,
            (data: ContactModel[]) => {
                if (data === undefined) {
                    return;
                } else {
                    tenant.Contacts = data;
                    this._uscContattiSelRest.renderContactsTree(data);
                    this._loadingPanel.hide(this.splitterMainId);
                }
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.splitterMainId);
                $("#".concat(this.rtvTenantsId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    //endregion
    protected addNodesToRadTreeView(nodeValue: string, nodeText: string, text: string, nodeImageUrl: string, radTreeView: Telerik.Web.UI.RadTreeView) {
        let rtvNode: Telerik.Web.UI.RadTreeNode;

        if (radTreeView.get_nodes().get_count() === 0) {
            rtvNode = new Telerik.Web.UI.RadTreeNode();
            rtvNode.set_text(text);
            radTreeView.get_nodes().add(rtvNode);
        }
        rtvNode = new Telerik.Web.UI.RadTreeNode();
        rtvNode.set_text(nodeText);
        rtvNode.set_value(nodeValue);
        rtvNode.set_imageUrl(nodeImageUrl);
        radTreeView.get_nodes().getNode(0).get_nodes().add(rtvNode);
        radTreeView.get_nodes().getNode(0).expand();
    }

    private alreadySavedInTree(nodeValue: string, radTreeView: Telerik.Web.UI.RadTreeView): boolean {
        let alreadySavedInTree: boolean = false;
        if (radTreeView.get_nodes().get_count() !== 0) {
            var allNodes = radTreeView.get_nodes().getNode(0).get_allNodes();
            for (var i = 0; i < allNodes.length; i++) {
                var node = allNodes[i];
                if (node.get_value() === nodeValue) {
                    alreadySavedInTree = true;
                    break;
                }
            }
        }
        return alreadySavedInTree;
    }
}
export = TbltTenant;