/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
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
import ContactModel = require('App/Models/Commons/ContactModel');
import UpdateActionType = require('App/Models/UpdateActionType');
import TenantAOOModel = require('App/Models/Tenants/TenantAOOModel');
import TenantAOOAttribute = require('App/Models/Tenants/TenantAOOAttributeEnum');
import TenantTypologyTypeEnum = require('App/Models/Tenants/TenantTypologyTypeEnum');
import uscContainerRest = require('UserControl/uscContainerRest');
import PageClassHelper = require('App/Helpers/PageClassHelper');


class TbltTenant extends TbltTenantBase {

    ajaxManagerId: string;
    uscNotificationId: string;
    ajaxLoadingPanelId: string;
    splitterMainId: string;
    toolBarSearchId: string;
    pnlDetailsId: string;
    managerId: string;
    uscContainerId: string;

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _manager: Telerik.Web.UI.RadWindowManager;

    //rad tree views
    rtvTenantsId: string;
    rtvPECMailBoxesId: string;
    rtvWorkflowRepositoriesId: string;
    rtvTenantConfigurationsId: string;

    // toolbars where the buttons are
    tbPECMailBoxesControlId: string;
    tbWorkflowRepositoryControlId: string;
    tbConfigurationControlId: string;
    rtbCompanyOptionsId: string;

    //details right zone
    lblCompanyNameId: string;
    lblTenantNameId: string;
    lblTenantNoteId: string;
    lblTenantDataDiAttivazioneId: string;
    lblTenantDataDiDisattivazioneId: string;

    //rad windows
    rwPECMailBoxId: string;
    rwRoleId: string;
    rwWorkflowRepositoryId: string;
    rwTenantConfigurationId: string;
    rwTenantSelectorId: string;

    //window combos
    cmbPECMailBoxId: string;
    cmbWorkflowRepositoryId: string;
    cmbRoleId: string;
    cmbConfigurationTypeId: string;
    cmbTenantWorkflowRepositoryTypeId: string;

    // Window buttons Confirm, Cancel
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
    btnTenantAOOSelectorOkId: string;
    btnTenantAOOSelectorCancelId: string;

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
    txtTenantAOONameId: string;
    txtTenantAOONoteId: string;
    txtCategorySuffixId: string;
    txtTenantAOONameInfoId: string;
    txtTenantAOONoteInfoId: string;
    txtTenantAOOSuffixInfoId: string;

    selectedTenantConfiguration: TenantConfigurationModel;

    //dropdown data models
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
    isTenantAOOUpdate: boolean;
    private _enumHelper: EnumHelper;
    maxNumberElements: string;
    contacts: ContactModel[];

    private _currentSelectedTenant: TenantViewModel;

    //rad tree views
    private _rtvTenants: Telerik.Web.UI.RadTreeView;
    private _rtvPECMailBoxes: Telerik.Web.UI.RadTreeView;
    private _rtvRoles: Telerik.Web.UI.RadTreeView;
    private _rtvTenantWorkflowRepositories: Telerik.Web.UI.RadTreeView;
    private _rtvTenantConfigurations: Telerik.Web.UI.RadTreeView;

    // toolbars where the buttons are
    private _toolbarSearch: Telerik.Web.UI.RadToolBar;
    private _toolbarPECMailBox: Telerik.Web.UI.RadToolBar;
    private _toolbarRole: Telerik.Web.UI.RadToolBar;
    private _toolbarWorkflowRepository: Telerik.Web.UI.RadToolBar;
    private _tbConfigurationControl: Telerik.Web.UI.RadToolBar;
    private _toolbarItemSearchTenantName: Telerik.Web.UI.RadToolBarItem;
    private _toolbarItemSearchCompanyName: Telerik.Web.UI.RadToolBarItem;
    private _txtSearchTenantName: Telerik.Web.UI.RadTextBox;
    private _txtSearchCompanyName: Telerik.Web.UI.RadTextBox;
    private _rtbCompanyOptions: Telerik.Web.UI.RadToolBar;

    //details right zone
    private _lblCompanyNameId: HTMLLabelElement;
    private _lblTenantNameId: HTMLLabelElement;
    private _lblTenantNoteId: HTMLLabelElement;
    private _lblTenantDataDiAttivazioneId: HTMLLabelElement;
    private _lblTenantDataDiDisattivazioneId: HTMLLabelElement;
    private _txtTenantAOONameInfo: HTMLLabelElement;
    private _txtTenantAOONoteInfo: HTMLLabelElement;
    private _txtTenantAOOSuffixInfo: HTMLLabelElement;

    // windows
    private _rwPECMailBox: Telerik.Web.UI.RadWindow;
    private _rwWorkflowRepository: Telerik.Web.UI.RadWindow;
    private _rwTenantConfiguration: Telerik.Web.UI.RadWindow;
    private _rwTenantSelector: Telerik.Web.UI.RadWindow;

    //window combos
    private _cmbPECMailBox: Telerik.Web.UI.RadComboBox;
    private _cmbRole: Telerik.Web.UI.RadComboBox;
    private _cmbWorkflowRepository: Telerik.Web.UI.RadComboBox;
    private _cmbConfigurationType: Telerik.Web.UI.RadComboBox;
    private _cmbTenantWorkflowRepositoryType: Telerik.Web.UI.RadComboBox;

    // Window buttons Confirm, Cancel
    private _btnPECMailBoxSelectorOk: Telerik.Web.UI.RadButton;
    private _btnPECMailBoxSelectorCancel: Telerik.Web.UI.RadButton;
    private _btnWorkflowRepositorySelectorOk: Telerik.Web.UI.RadButton;
    private _btnWorkflowRepositorySelectorCancel: Telerik.Web.UI.RadButton;
    private _btnTenantConfigurationSelectorOk: Telerik.Web.UI.RadButton;
    private _btnTenantConfigurationSelectorCancel: Telerik.Web.UI.RadButton;
    private _btnTenantSelectorOk: Telerik.Web.UI.RadButton;
    private _btnTenantSelectorCancel: Telerik.Web.UI.RadButton;
    private _btnTenantAOOSelectorOk: Telerik.Web.UI.RadButton;
    private _btnTenantAOOSelectorCancel: Telerik.Web.UI.RadButton;

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
    private _txtTenantAOOName: Telerik.Web.UI.RadTextBox;
    private _txtTenantAOONote: Telerik.Web.UI.RadTextBox;
    private _txtCategorySuffix: Telerik.Web.UI.RadTextBox;

    private _serviceConfiguration: ServiceConfiguration[];
    private _uscContainerRest: uscContainerRest;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(serviceConfigurations);
        this._serviceConfiguration = serviceConfigurations;

        this._currentSelectedTenant = new TenantViewModel();
        this.selectedTenantConfiguration = new TenantConfigurationModel();

        this._enumHelper = new EnumHelper();
    }

    initialize(): void {
        super.initialize();
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.managerId);

        this._toolbarSearch = <Telerik.Web.UI.RadToolBar>$find(this.toolBarSearchId);
        this._toolbarSearch.add_buttonClicking(this.toolbarSearch_onClick);

        //rad tree views
        this._rtvTenants = <Telerik.Web.UI.RadTreeView>$find(this.rtvTenantsId);
        this._rtvTenants.add_nodeExpanded(this.tenantAOO_onExpanded);
        this._rtvTenants.add_nodeClicked(this.rtvTenants_onClick);
        //this._rtvPECMailBoxes = <Telerik.Web.UI.RadTreeView>$find(this.rtvPECMailBoxesId);
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
        this._txtTenantAOONameInfo = <HTMLLabelElement>document.getElementById(this.txtTenantAOONameInfoId);
        this._txtTenantAOONoteInfo = <HTMLLabelElement>document.getElementById(this.txtTenantAOONoteInfoId);
        this._txtTenantAOOSuffixInfo = <HTMLLabelElement>document.getElementById(this.txtTenantAOOSuffixInfoId);

        //Containers, PECMailBoxes, Rules, WorkflowReposiory, TenantConfiguration
        //this._toolbarPECMailBox = <Telerik.Web.UI.RadToolBar>$find(this.tbPECMailBoxesControlId);
        //this._toolbarPECMailBox.add_buttonClicking(this.toolbarPECMailBox_onClick);
        this._toolbarWorkflowRepository = <Telerik.Web.UI.RadToolBar>$find(this.tbWorkflowRepositoryControlId);
        this._toolbarWorkflowRepository.add_buttonClicking(this.toolbarWorkflowRepository_onClick);
        this._tbConfigurationControl = <Telerik.Web.UI.RadToolBar>$find(this.tbConfigurationControlId);
        this._tbConfigurationControl.add_buttonClicking(this.toolbarConfiguration_onClick);
        this._rtbCompanyOptions = <Telerik.Web.UI.RadToolBar>$find(this.rtbCompanyOptionsId);
        this._rtbCompanyOptions.add_buttonClicked(this.rtbCompanyOptions_onClick);

        // windows
        //this._rwPECMailBox = <Telerik.Web.UI.RadWindow>$find(this.rwPECMailBoxId);
        //this._rwPECMailBox.add_show(this._rwPECMailBox_OnShow);
        this._rwTenantConfiguration = <Telerik.Web.UI.RadWindow>$find(this.rwTenantConfigurationId);
        this._rwWorkflowRepository = <Telerik.Web.UI.RadWindow>$find(this.rwWorkflowRepositoryId);
        this._rwWorkflowRepository.add_show(this._rwWorkflowRepository_OnShow);
        this._rwTenantSelector = <Telerik.Web.UI.RadWindow>$find(this.rwTenantSelectorId);

        //combos from windows
        //this._cmbPECMailBox = <Telerik.Web.UI.RadComboBox>$find(this.cmbPECMailBoxId);
        //this._cmbPECMailBox.add_selectedIndexChanged(this.cmbPECMailBoxes_onClick);
        //this._cmbPECMailBox.add_itemsRequested(this._cmbPECMailBox_OnClientItemsRequested);
        this._cmbRole = <Telerik.Web.UI.RadComboBox>$find(this.cmbRoleId);
        this._cmbWorkflowRepository = <Telerik.Web.UI.RadComboBox>$find(this.cmbWorkflowRepositoryId);
        this._cmbWorkflowRepository.add_selectedIndexChanged(this.cmbWorkflowRepositories_onClick);
        this._cmbWorkflowRepository.add_itemsRequested(this._cmbWorkflowRepository_OnClientItemsRequested);
        this._cmbConfigurationType = <Telerik.Web.UI.RadComboBox>$find(this.cmbConfigurationTypeId);
        this._cmbTenantWorkflowRepositoryType = <Telerik.Web.UI.RadComboBox>$find(this.cmbTenantWorkflowRepositoryTypeId);

        // Window buttons Confirm, Cancel
        //this._btnPECMailBoxSelectorOk = <Telerik.Web.UI.RadButton>$find(this.btnPECMailBoxSelectorOkId);
        //this._btnPECMailBoxSelectorOk.add_clicking(this.btnPECMailBoxOk_onClick);
        //this._btnPECMailBoxSelectorCancel = <Telerik.Web.UI.RadButton>$find(this.btnPECMailBoxSelectorCancelId);
        //this._btnPECMailBoxSelectorCancel.add_clicking(this.btnPECMailBoxCancel_onClick);
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
        this._btnTenantAOOSelectorOk = <Telerik.Web.UI.RadButton>$find(this.btnTenantAOOSelectorOkId);
        this._btnTenantAOOSelectorOk.add_clicked(this.btnTenantAOOSelectorOk_onClick);
        this._btnTenantAOOSelectorCancel = <Telerik.Web.UI.RadButton>$find(this.btnTenantAOOSelectorCancelId);
        this._btnTenantAOOSelectorCancel.add_clicked(this.btnTenantAOOSelectorCancel_onClick);

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
        this._txtTenantAOOName = <Telerik.Web.UI.RadTextBox>$find(this.txtTenantAOONameId);
        this._txtTenantAOONote = <Telerik.Web.UI.RadTextBox>$find(this.txtTenantAOONoteId);
        this._txtCategorySuffix = <Telerik.Web.UI.RadTextBox>$find(this.txtCategorySuffixId);

        this._uscContainerRest = <uscContainerRest>$(`#${this.uscContainerId}`).data();
        let searchDTO: TenantSearchFilterDTO = null;
        $(`#tenantLinkOptions`).hide();
        $(`#tenantAOOInfo`).hide();
        this.loadTenantsAOO(searchDTO);
    }

    //region [ Tenants ]
    rtvTenants_onClick = (sender: any, args: Telerik.Web.UI.RadTreeNodeCancelEventArgs) => {
        this._rtbCompanyOptions.findItemByValue("create").set_enabled(true);
        this._rtbCompanyOptions.findItemByValue("modify").set_enabled(true);

        if (args.get_node().get_level() === 0) {
            $(`#tenantLinkOptions`).hide();
            $(`#tenantAOOInfo`).hide();
            this._rtbCompanyOptions.findItemByValue("modify").set_enabled(false);
        }

        if (args.get_node().get_attributes().getAttribute("nodeType") === TenantAOOAttribute.Tenant) {
            $(`#tenantLinkOptions`).show();
            $(`#tenantAOOInfo`).hide();

            this.populateTenantConfigurations();

            this._rtbCompanyOptions.findItemByValue("create").set_enabled(false);
            this.loadTenantDetails(args.get_node());
            this._currentSelectedTenant.UniqueId = args.get_node().get_value();

        }
        if (args.get_node().get_attributes().getAttribute("nodeType") === TenantAOOAttribute.AOO) {
            $(`#tenantLinkOptions`).hide();
            $(`#tenantAOOInfo`).show();
            this.loadTenantAOODetails(args.get_node());
        }

    }

    private populateTenantConfigurations(): void {
        this._cmbConfigurationType.clearItems();
        this._cmbTenantWorkflowRepositoryType.clearItems();

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
    }

    tenantAOO_onExpanded = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        if (args.get_node().get_attributes().getAttribute("nodeType") === TenantAOOAttribute.AOO) {
            this._loadingPanel.show(this.splitterMainId);
            this.getTenantsForTenantAOO(args);
        }
    }

    private getTenantsForTenantAOO(args: Telerik.Web.UI.RadTreeNodeEventArgs) {
        this._tenantAOOService.getTenantsByTenantAOOId(args.get_node().get_value(), (data: TenantAOOModel[]) => {
            let node: Telerik.Web.UI.RadTreeNode;
            args.get_node().get_nodes().clear();
            for (let i = 0; i < data[0].Tenants.length; i++) {
                let tenant: TenantViewModel = data[0].Tenants[i];
                node = new Telerik.Web.UI.RadTreeNode();
                let rtvNodeText: string = `${tenant.CompanyName} (${tenant.TenantName})`;
                node.set_text(rtvNodeText);
                node.set_value(tenant.UniqueId);
                node.get_attributes().setAttribute("nodeType", TenantAOOAttribute.Tenant);
                node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/tenant.png");
                let nodeIndex: number = args.get_node().get_index();
                this._rtvTenants.get_nodes().getNode(0).get_nodes().getNode(nodeIndex).get_nodes().add(node);
            }
            this._loadingPanel.hide(this.splitterMainId);
        }, (exception: ExceptionDTO) => {
            this._loadingPanel.hide(this.splitterMainId);
            $("#".concat(this.rtvTenantsId)).hide();
            this.showNotificationException(this.uscNotificationId, exception);
        });
    }

    loadTenantAOODetails(node: Telerik.Web.UI.RadTreeNode) {
        if (node.get_attributes().getAttribute("nodeType") !== TenantAOOAttribute.AOO) {
            return;
        }
        this._tenantAOOService.getTenantAOOById(node.get_value(), (data: TenantAOOModel) => {
            this._txtTenantAOONameInfo.innerText = data.Name;
            this._txtTenantAOONoteInfo.innerText = data.Note;
            this._txtTenantAOOSuffixInfo.innerText = data.CategorySuffix;
        }, (exception: ExceptionDTO) => {
            this._loadingPanel.hide(this.splitterMainId);
            $("#".concat(this.rtvTenantsId)).hide();
            this.showNotificationException(this.uscNotificationId, exception);
        });
    }

    loadTenantDetails(node: Telerik.Web.UI.RadTreeNode) {

        if (node.get_attributes().getAttribute("nodeType") !== TenantAOOAttribute.Tenant) {
            return;
        }
        let tenantId = node.get_value();
        this._tenantService.getTenantById(tenantId, (data: TenantViewModel) => {
            this._currentSelectedTenant = $.extend({}, data);

            this._lblCompanyNameId.innerText = data !== undefined ? data.CompanyName : "";
            this._lblTenantNameId.innerText = data !== undefined ? data.TenantName : "";
            this._lblTenantNoteId.innerText = data.Note !== null ? data.Note : "";
            this._lblTenantDataDiAttivazioneId.innerText = data !== undefined && moment(data.StartDate).isValid() ? moment(data.StartDate).format("DD-MM-YYYY") : "";
            this._lblTenantDataDiDisattivazioneId.innerText = data !== undefined && moment(data.EndDate).isValid() ? moment(data.EndDate).format("DD-MM-YYYY") : "";

            this.populateContainerTreeView();
            //this.populatePECMailBoxesTreeView();
            this.populateTenantWorkflowRepositoriesTreeView();
            this.populateTenantConfigurationsTreeView();
            this.populateWorkflowRepositoryComboBox();
        },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.splitterMainId);
                $("#".concat(this.rtvTenantsId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    private populateContainerTreeView(): void {
        PageClassHelper.callUserControlFunctionSafe<uscContainerRest>(this.uscContainerId)
            .done((instance) => {
                instance.initializeContainersTreeView(this._currentSelectedTenant);
            });
    }

    rtbCompanyOptions_onClick = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        if (this._rtvTenants.get_selectedNode() === null) {
            return;
        }
        switch (args.get_item().get_value()) {
            case "create": {
                if (this._rtvTenants.get_selectedNode().get_attributes().getAttribute("nodeType") === TenantAOOAttribute.AOO) {
                    this.openInsertTenantWindow();
                } else {
                    this.openInsertTenantAOOWindow();
                }
                break;
            }
            case "modify": {
                if (this._rtvTenants.get_selectedNode().get_attributes().getAttribute("nodeType") === TenantAOOAttribute.AOO) {
                    this.openEditTenantAOOWindow();
                }
                if (this._rtvTenants.get_selectedNode().get_attributes().getAttribute("nodeType") === TenantAOOAttribute.Tenant) {
                    this.openEditTenantWindow();
                }
                break;
            }
        }
    }

    private openEditTenantWindow() {
        $("#TenantSelectorSelectorWindowTable").show();
        $("#TenantAOOSelectorWindowTable").hide();
        if (this._rtvTenants.get_selectedNode() !== null) {
            this._dpTenantDateFrom.set_selectedDate(moment(this._currentSelectedTenant.StartDate).isValid() ? new Date(this._currentSelectedTenant.StartDate) : null);
            this._dpTenantDateTo.set_selectedDate(moment(this._currentSelectedTenant.EndDate).isValid() ? new Date(this._currentSelectedTenant.EndDate) : null);
            this._txtTenantName.set_value(this._currentSelectedTenant.TenantName);
            this._txtTenantCompany.set_value(this._currentSelectedTenant.CompanyName);
            this._txtTenantNote.set_value(this._currentSelectedTenant.Note);
            this.isTenantUpdate = true;
            this._dpTenantDateTo.get_element().parentElement.style.visibility = "visible";
            this._rwTenantSelector.set_title("Modifica UO");
            this._rwTenantSelector.set_height(220);
            this._rwTenantSelector.show();
        }
        else {
            alert("Selezionare un UP");
        }
    }

    private openEditTenantAOOWindow() {
        $("#TenantSelectorSelectorWindowTable").hide();
        $("#TenantAOOSelectorWindowTable").show();
        this.isTenantAOOUpdate = true;
        let tenantAOOId = this._rtvTenants.get_selectedNode().get_value();
        this._tenantAOOService.getTenantAOOById(tenantAOOId, (data: TenantAOOModel) => {
            this._txtTenantAOOName.set_value(data.Name);
            this._txtTenantAOONote.set_value(data.Note);
            this._txtCategorySuffix.set_value(data.CategorySuffix);
            this._rwTenantSelector.set_title("Modifica AOO");
            this._rwTenantSelector.set_height(180);
            this._rwTenantSelector.show();
        });
    }

    private openInsertTenantAOOWindow() {
        $("#TenantSelectorSelectorWindowTable").hide();
        $("#TenantAOOSelectorWindowTable").show();
        this._txtTenantAOOName.clear();
        this._txtTenantAOONote.clear();
        this._txtCategorySuffix.clear();
        this.isTenantAOOUpdate = false;
        this._rwTenantSelector.set_title("Aggiungi AOO");
        this._rwTenantSelector.set_height(180);
        this._rwTenantSelector.show();
    }

    private openInsertTenantWindow() {
        $("#TenantSelectorSelectorWindowTable").show();
        $("#TenantAOOSelectorWindowTable").hide();
        this._dpTenantDateFrom.clear();
        this._txtTenantName.clear();
        this._txtTenantCompany.clear();
        this._txtTenantNote.clear();
        this.isTenantUpdate = false;
        this._dpTenantDateTo.get_element().parentElement.style.visibility = "hidden";
        this._dpTenantDateFrom.set_selectedDate(new Date());
        this._rwTenantSelector.set_title("Aggiungi UO");
        this._rwTenantSelector.set_height(220);
        this._rwTenantSelector.show();
    }
    // endregion

    toolbarSearch_onClick = (sender: any, args: Telerik.Web.UI.RadToolBarCancelEventArgs) => {
        switch (args.get_item().get_value()) {
            case "search": {
                let tenantName = this._txtSearchTenantName.get_textBoxValue();
                let companyName = this._txtSearchCompanyName.get_textBoxValue();
                let searchDTO: TenantSearchFilterDTO = new TenantSearchFilterDTO();
                if (tenantName) {
                    searchDTO.tenantName = tenantName;
                }
                if (companyName) {
                    searchDTO.companyName = companyName;
                }
                let activeItem: any = this._toolbarSearch.findItemByValue("active");
                let disabledItem: any = this._toolbarSearch.findItemByValue("disabled");
                searchDTO.isActive = activeItem.get_checked()
                    ? disabledItem.get_checked() ? null : true
                    : disabledItem.get_checked() ? false : null;
                if (tenantName === "" && companyName === "" && searchDTO.isActive === null) {
                    searchDTO = null;
                }

                $(`#tenantLinkOptions`).hide();
                $(`#tenantAOOInfo`).hide();
                this._rtbCompanyOptions.findItemByValue("modify").set_enabled(false);

                this.loadTenantsAOO(searchDTO);
                break;
            }
        }
    }

    private loadTenantsAOO(searchDTO: TenantSearchFilterDTO) {
        this._loadingPanel.show(this.splitterMainId);
        if (searchDTO) {
            this._tenantAOOService.getFilteredTenants(searchDTO, (data: TenantAOOModel[]) => {
                this.createTenantAOORootNode();
                let node: Telerik.Web.UI.RadTreeNode;
                for (let i = 0; i < data.length; i++) {
                    let tenantAOO: TenantAOOModel = data[i];
                    if (tenantAOO.Tenants.length == 0) {
                        continue;
                    }
                    node = new Telerik.Web.UI.RadTreeNode();
                    node.set_text(tenantAOO.Name);
                    node.set_value(tenantAOO.UniqueId);
                    node.get_attributes().setAttribute("nodeType", TenantAOOAttribute.AOO);
                    node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/tenantAOO.png");
                    node.expand();
                    this._rtvTenants.get_nodes().getNode(0).get_nodes().add(node);
                    let nodeIndex: number = node.get_index();

                    for (let j = 0; j < tenantAOO.Tenants.length; j++) {
                        let tenant: TenantViewModel = tenantAOO.Tenants[j];
                        node = new Telerik.Web.UI.RadTreeNode();
                        let rtvNodeText: string = `${tenant.CompanyName} (${tenant.TenantName})`;
                        node.set_text(rtvNodeText);
                        node.set_value(tenant.UniqueId);
                        node.get_attributes().setAttribute("nodeType", TenantAOOAttribute.Tenant);
                        node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/tenant.png");
                        this._rtvTenants.get_nodes().getNode(0).get_nodes().getNode(nodeIndex).get_nodes().add(node);
                        let rtMinusElement: any = node.get_element().parentElement.parentElement.getElementsByClassName("rtMinus").item(0);
                        rtMinusElement.style.display = "none";
                    }
                }
                this._loadingPanel.hide(this.splitterMainId);
            }, (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.splitterMainId);
                $("#".concat(this.rtvTenantsId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            });
        }
        else {
            this._tenantAOOService.getTenantsAOO((data: TenantAOOModel[]) => {
                this.createTenantAOORootNode();
                let node: Telerik.Web.UI.RadTreeNode;
                for (let i = 0; i < data.length; i++) {
                    let tenantAOO: TenantAOOModel = data[i];
                    node = new Telerik.Web.UI.RadTreeNode();
                    node.set_text(tenantAOO.Name);
                    node.set_value(tenantAOO.UniqueId);
                    node.get_attributes().setAttribute("nodeType", TenantAOOAttribute.AOO);
                    node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/tenantAOO.png");
                    this._rtvTenants.get_nodes().getNode(0).get_nodes().add(node);
                    node = new Telerik.Web.UI.RadTreeNode();
                    node.set_text("");
                    this._rtvTenants.get_nodes().getNode(0).get_nodes().getNode(i).get_nodes().add(node);
                }
                this._loadingPanel.hide(this.splitterMainId);
            }, (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.splitterMainId);
                $("#".concat(this.rtvTenantsId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            });
        }
    }

    private createTenantAOORootNode() {
        this._rtvTenants.get_nodes().clear();
        let rtvNode: Telerik.Web.UI.RadTreeNode;
        rtvNode = new Telerik.Web.UI.RadTreeNode();
        rtvNode.set_text("AOO");
        rtvNode.expand();
        this._rtvTenants.get_nodes().add(rtvNode);
        rtvNode.select();
        this._rtbCompanyOptions.findItemByValue("modify").set_enabled(false);
    }

    btnTenantSelectorOk_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        if (this._txtTenantName.get_value() == "") {
            alert("Il sigla UO e obbligatorio");
            return;
        }

        if (this._txtTenantCompany.get_value() == "") {
            alert("Il nome UO e obbligatorio");
            return;
        }

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
                RegistrationUser: null,
                TenantAOO: <TenantAOOModel>{ UniqueId: this._rtvTenants.get_selectedNode().get_value() },
                TenantTypology: TenantTypologyTypeEnum.InternalTenant
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
                        rtvNode.get_attributes().setAttribute("nodeType", TenantAOOAttribute.Tenant);
                        tenant.UniqueId = data.UniqueId;
                        let tenantAOONode: Telerik.Web.UI.RadTreeNode = this._rtvTenants.get_selectedNode();
                        if (data.TenantTypology == TenantTypologyTypeEnum.InternalTenant) {
                            this._rtvTenants.get_nodes().getNode(0).get_nodes().getNode(tenantAOONode.get_index()).get_nodes().add(rtvNode);
                        }
                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.splitterMainId);
                        $("#".concat(this.rtvTenantsId)).hide();
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
            } else {
                if (this._rtvTenants.get_selectedNode() !== null) {
                    tenant.UniqueId = this._rtvTenants.get_selectedNode().get_value();
                    tenant.Configurations = this._currentSelectedTenant.Configurations;
                    tenant.Containers = this._currentSelectedTenant.Containers;
                    tenant.PECMailBoxes = this._currentSelectedTenant.PECMailBoxes;
                    tenant.Roles = this._currentSelectedTenant.Roles;
                    tenant.TenantWorkflowRepositories = this._currentSelectedTenant.TenantWorkflowRepositories;
                    tenant.TenantAOO = <TenantAOOModel>{ UniqueId: this._rtvTenants.get_selectedNode().get_parent().get_value() };
                    nodeValue = tenant.UniqueId;
                }
                this._tenantService.updateTenant(tenant, null,
                    (data) => {
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

    btnTenantSelectorCancel_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        this._rwTenantSelector.close();
    }

    btnTenantAOOSelectorOk_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        if (this._txtTenantAOOName.get_value() == "") {
            alert("Il campo nome e obbligatorio");
            return;
        }

        let tenantAOOModel: TenantAOOModel = <TenantAOOModel>{
            Name: this._txtTenantAOOName.get_textBoxValue(),
            Note: this._txtTenantAOONote.get_textBoxValue(),
            CategorySuffix: this._txtCategorySuffix.get_textBoxValue(),
            Tenants: [],
            TenantTypology: TenantTypologyTypeEnum.InternalTenant
        }

        if (this.isTenantAOOUpdate) {
            this.updateTenantAOO(tenantAOOModel);
        } else {
            this.insertTenantAOO(tenantAOOModel);
        }
    }

    btnTenantAOOSelectorCancel_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        this._rwTenantSelector.close();
    }

    private insertTenantAOO(tenantAOOModel: TenantAOOModel) {
        this._tenantAOOService.insertTenantAOO(tenantAOOModel, (data) => {            
            let node = new Telerik.Web.UI.RadTreeNode();
            node.set_text(data.Name);
            node.set_value(data.UniqueId);
            node.get_attributes().setAttribute("nodeType", TenantAOOAttribute.AOO);
            if (tenantAOOModel.TenantTypology == TenantTypologyTypeEnum.InternalTenant) {
                this._rtvTenants.get_nodes().getNode(0).get_nodes().add(node);
            }
            this._rwTenantSelector.close();
        }, (exception: ExceptionDTO) => {
            this._loadingPanel.hide(this.splitterMainId);
            $("#".concat(this.rtvTenantsId)).hide();
            this.showNotificationException(this.uscNotificationId, exception);
        });
    }

    private updateTenantAOO(tenantAOOModel: TenantAOOModel) {
        tenantAOOModel.UniqueId = this._rtvTenants.get_selectedNode().get_value();
        this._tenantAOOService.updateTenantAOO(tenantAOOModel, (data: TenantAOOModel) => {
            this._rtvTenants.get_selectedNode().set_text(data.Name);
            this._rwTenantSelector.close();
            this._txtTenantAOONameInfo.innerHTML = tenantAOOModel.Name;
            this._txtTenantAOONoteInfo.innerHTML = tenantAOOModel.Note;
            this._txtTenantAOOSuffixInfo.innerHTML = tenantAOOModel.CategorySuffix;
        }, (exception: ExceptionDTO) => {
            this._loadingPanel.hide(this.splitterMainId);
            $("#".concat(this.rtvTenantsId)).hide();
            this.showNotificationException(this.uscNotificationId, exception);
        });
    }
    //endregion

    //region [ Add/Delete PECMailBoxes from RadTreeView ]
    //toolbarPECMailBox_onClick = (sender: any, args: Telerik.Web.UI.RadToolBarCancelEventArgs) => {
    //    var btn = args.get_item();
    //    switch (btn.get_index()) {
    //        case 0:
    //            this._rwPECMailBox.show();
    //            this._pecMailBoxService.getPECMailBoxes("",
    //                (data: any) => {
    //                    this.pecMailBoxes = <PECMailBoxModel[]>data;
    //                    this.addPECMailBoxes(this.pecMailBoxes, this._cmbPECMailBox);
    //                },
    //                (exception: ExceptionDTO) => {
    //                    this._loadingPanel.hide(this.splitterMainId);
    //                    this.showNotificationException(this.uscNotificationId, exception);
    //                });
    //            args.set_cancel(true);
    //            break;
    //        case 1:
    //            if (this._rtvPECMailBoxes.get_selectedNode() !== null) {
    //                this._manager.radconfirm("Sei sicuro di voler eliminare il casella PEC selezionato?", (arg) => {
    //                    if (arg) {
    //                        let tenantToUpdate: TenantViewModel = this.constructTenant();
    //                        tenantToUpdate.PECMailBoxes = this._currentSelectedTenant.PECMailBoxes.filter(x => x.EntityShortId === Number(this._rtvPECMailBoxes.get_selectedNode().get_value()));
    //                        let removeIndex = this._currentSelectedTenant.PECMailBoxes.map(item => item.EntityShortId).indexOf(Number(this._rtvPECMailBoxes.get_selectedNode().get_value()));
    //                        this._currentSelectedTenant.PECMailBoxes.splice(removeIndex, 1);
    //                        this._tenantService.updateTenant(tenantToUpdate, UpdateActionType.TenantPECMailBoxRemove,
    //                            (data) => {
    //                                this._rtvPECMailBoxes.get_nodes().getNode(0).get_nodes().removeAt(this._rtvPECMailBoxes.get_selectedNode().get_index());
    //                                if (this._rtvPECMailBoxes.get_nodes().getNode(0).get_nodes().getItem(0) === undefined)
    //                                    this._rtvPECMailBoxes.get_nodes().clear();
    //                            },
    //                            (exception: ExceptionDTO) => {
    //                                this._loadingPanel.hide(this.splitterMainId);
    //                                $("#".concat(this.rtvTenantsId)).hide();
    //                                this.showNotificationException(this.uscNotificationId, exception);
    //                            });
    //                    }
    //                }, 400, 300);
    //            } else {
    //                alert("Selezionare una caselle PEC");
    //            }
    //            args.set_cancel(true);
    //            break;
    //    }
    //}

    //btnPECMailBoxOk_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
    //    if (this._cmbPECMailBox && this.selectedPECMailBox) {
    //        this._rwPECMailBox.close();
    //        this._loadingPanel.show(this.tbPECMailBoxesControlId);
    //        let nodeImageUrl = "../App_Themes/DocSuite2008/imgset16/box_open.png";
    //        let nodeValue = this.selectedPECMailBox.EntityShortId.toString();
    //        let nodeText = this.selectedPECMailBox.MailBoxRecipient;
    //        let alreadySavedInTree: boolean = this.alreadySavedInTree(nodeValue, this._rtvPECMailBoxes);
    //        if (!alreadySavedInTree) {
    //            this._currentSelectedTenant.PECMailBoxes.push(this.selectedPECMailBox);
    //            let tenantToUpdate: TenantViewModel = this.constructTenant();
    //            tenantToUpdate.PECMailBoxes = [this.selectedPECMailBox];
    //            this._tenantService.updateTenant(tenantToUpdate, UpdateActionType.TenantPECMailBoxAdd,
    //                (data) => {
    //                    this.addNodesToRadTreeView(nodeValue, nodeText, "Caselle PEC", nodeImageUrl, this._rtvPECMailBoxes);
    //                },
    //                (exception: ExceptionDTO) => {
    //                    this._loadingPanel.hide(this.splitterMainId);
    //                    $("#".concat(this.rtvTenantsId)).hide();
    //                    this.showNotificationException(this.uscNotificationId, exception);
    //                });
    //        }
    //        this._loadingPanel.hide(this.tbPECMailBoxesControlId);
    //    }
    //}

    //btnPECMailBoxCancel_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
    //    this._rwPECMailBox.close();
    //}

    //protected addPECMailBoxes(pecMailBoxes: PECMailBoxModel[], cmbPECMailBox: Telerik.Web.UI.RadComboBox) {
    //    this.pecMailBoxes = pecMailBoxes;
    //    cmbPECMailBox.get_items().clear();
    //    let item: Telerik.Web.UI.RadComboBoxItem;
    //    item = new Telerik.Web.UI.RadComboBoxItem();
    //    item.set_text("");
    //    item.set_value("");
    //    cmbPECMailBox.get_items().add(item);
    //    for (let pecMailBox of pecMailBoxes) {
    //        item = new Telerik.Web.UI.RadComboBoxItem();
    //        item.set_text(pecMailBox.MailBoxRecipient);
    //        item.set_value(pecMailBox.EntityShortId.toString());
    //        cmbPECMailBox.get_items().add(item);
    //    }
    //}

    //cmbPECMailBoxes_onClick = (sender: any, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
    //    this.selectedPECMailBox = this.pecMailBoxes.filter(function (x) {
    //        return x.EntityShortId.toString() === args.get_item().get_value()
    //    })[0];
    //}

    //_cmbPECMailBox_OnClientItemsRequested = (sender: Telerik.Web.UI.RadComboBox,
    //    args: Telerik.Web.UI.RadComboBoxRequestEventArgs) => {
    //    let pecMailBoxNumberOfItems: number = sender.get_items().get_count();
    //    this._pecMailBoxService.getAllPECMailBoxes(args.get_text(), this.maxNumberElements, pecMailBoxNumberOfItems,
    //        (data: ODATAResponseModel<PECMailBoxModel>) => {
    //            try {
    //                this.refreshPECMailBoxes(data.value);
    //                let scrollToPosition: boolean = args.get_domEvent() == undefined;
    //                if (scrollToPosition) {
    //                    if (sender.get_items().get_count() > 0) {
    //                        let scrollContainer: JQuery = $(sender.get_dropDownElement()).find('div.rcbScroll');
    //                        scrollContainer.scrollTop($(sender.get_items().getItem(pecMailBoxNumberOfItems + 1).get_element()).position().top);
    //                    }
    //                }
    //                sender.get_attributes().setAttribute('otherContainerCount', data.count.toString());
    //                sender.get_attributes().setAttribute('updating', 'false');
    //                if (sender.get_items().get_count() > 0) {
    //                    pecMailBoxNumberOfItems = sender.get_items().get_count() - 1;
    //                }
    //                this._cmbPECMailBox.get_moreResultsBoxMessageElement().innerText = `Visualizzati ${pecMailBoxNumberOfItems.toString()} di ${data.count.toString()}`;
    //            }
    //            catch (error) {
    //            }
    //        },
    //        (exception: ExceptionDTO) => {
    //            this.showNotificationException(this.uscNotificationId, exception);
    //        });
    //}

    //refreshPECMailBoxes = (data: PECMailBoxModel[]) => {
    //    if (data.length > 0) {
    //        this._cmbPECMailBox.beginUpdate();
    //        if (this._cmbPECMailBox.get_items().get_count() === 0) {
    //            let emptyItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
    //            emptyItem.set_text("");
    //            emptyItem.set_value("");
    //            this._cmbPECMailBox.get_items().insert(0, emptyItem);
    //        }

    //        $.each(data, (index, pecMailBox) => {
    //            let item: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
    //            item.set_text(pecMailBox.MailBoxRecipient);
    //            item.set_value(pecMailBox.EntityShortId.toString());
    //            this._cmbPECMailBox.get_items().add(item);
    //            this.pecMailBoxes.push(pecMailBox);
    //        });
    //        this._cmbPECMailBox.showDropDown();
    //        this._cmbPECMailBox.endUpdate();
    //    }
    //    else {
    //        if (this._cmbPECMailBox.get_items().get_count() === 0) {
    //        }

    //    }
    //}

    //private populatePECMailBoxesTreeView() {
    //    this._tenantService.getTenantPECMailBoxes(this._currentSelectedTenant.UniqueId,
    //        (data: PECMailBoxModel[]) => {
    //            if (data === undefined) {
    //                return;
    //            } else {
    //                this._rtvPECMailBoxes.get_nodes().clear();
    //                let thisObj = this;
    //                this._currentSelectedTenant.PECMailBoxes = data;
    //                $.each(data, function (i, value) {
    //                    let nodeImageUrl = "../App_Themes/DocSuite2008/imgset16/box_open.png";
    //                    let nodeValue = value.EntityShortId.toString();
    //                    let nodeText = value.MailBoxRecipient;
    //                    let alreadySavedInTree: boolean = thisObj.alreadySavedInTree(nodeValue, thisObj._rtvPECMailBoxes);
    //                    if (!alreadySavedInTree) {
    //                        thisObj.addNodesToRadTreeView(nodeValue, nodeText, "Caselle PEC", nodeImageUrl, thisObj._rtvPECMailBoxes);
    //                    }
    //                });
    //            }
    //        },
    //        (exception: ExceptionDTO) => {
    //            this._loadingPanel.hide(this.splitterMainId);
    //            $("#".concat(this.rtvTenantsId)).hide();
    //            this.showNotificationException(this.uscNotificationId, exception);
    //        });
    //}

    //_rwPECMailBox_OnShow = (sender: Telerik.Web.UI.RadWindow, args: Sys.EventArgs) => {
    //    this._cmbPECMailBox.clearSelection();
    //    this.selectedPECMailBox = null;
    //}
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
                    let removeIndex = this._currentSelectedTenant.TenantWorkflowRepositories.map(item => item.UniqueId).indexOf(this._rtvTenantWorkflowRepositories.get_selectedNode().get_value());
                    this._tenantWorkflowRepositoryService.deleteTenantWorkflowRepository(this._currentSelectedTenant.TenantWorkflowRepositories[removeIndex],
                        (data) => {
                            this._currentSelectedTenant.TenantWorkflowRepositories.splice(removeIndex, 1);
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

    btnWorkflowRepositoryOk_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        if (this._cmbWorkflowRepository && this.selectedWorkflowRepository &&
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
            let tenantWorkflowRepository: TenantWorkflowRepositoryModel = {
                Tenant: viewModelMapper.Map(this._currentSelectedTenant),
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
                let asd = this._cmbWorkflowRepository.get_selectedItem().get_text();
                console.log(this._cmbWorkflowRepository.get_selectedItem().get_text());
                let thisObj = this;
                if (this._currentSelectedTenant.TenantWorkflowRepositories.length === 0 ||
                    this._currentSelectedTenant.TenantWorkflowRepositories.filter(function (x) {
                        return x.WorkflowRepository.Name !== thisObj._cmbWorkflowRepository.get_selectedItem().get_text()
                    })[0])
                    this._tenantWorkflowRepositoryService.insertTenantWorkflowRepository(tenantWorkflowRepository,
                        (data) => {
                            nodeValue = data.UniqueId;
                            this.addNodesToRadTreeView(nodeValue, nodeText, "Attività", nodeImageUrl, this._rtvTenantWorkflowRepositories);
                            this._currentSelectedTenant.TenantWorkflowRepositories.push(data);
                            this._currentSelectedTenant.TenantWorkflowRepositories[this._currentSelectedTenant.TenantWorkflowRepositories.length - 1].ConfigurationType =
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
                            let thisObj = this;
                            this._currentSelectedTenant.TenantWorkflowRepositories.filter(function (x) {
                                return x.UniqueId === thisObj._rtvTenantWorkflowRepositories.get_selectedNode().get_value()
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

    btnWorkflowRepositoryCancel_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
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

    private populateTenantWorkflowRepositoriesTreeView() {
        this._tenantService.getTenantWorkflowRepositories(this._currentSelectedTenant.UniqueId,
            (data: TenantWorkflowRepositoryModel[]) => {
                if (data === undefined) {
                    return;
                } else {
                    this._rtvTenantWorkflowRepositories.get_nodes().clear();
                    let thisObj = this;
                    this._currentSelectedTenant.TenantWorkflowRepositories = data;
                    this.tenantWorkflowRepositories = this._currentSelectedTenant.TenantWorkflowRepositories;
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
                    let editIndex = this._currentSelectedTenant.Configurations.map(item => item.UniqueId)
                        .indexOf(this._rtvTenantConfigurations.get_selectedNode().get_value());
                    this._dpStartDateFrom.set_selectedDate(new Date(this._currentSelectedTenant.Configurations[editIndex].StartDate));
                    if (this._currentSelectedTenant.Configurations[editIndex].EndDate && this._currentSelectedTenant.Configurations[editIndex].EndDate !== "")
                        this._dpEndDateFrom.set_selectedDate(new Date(this._currentSelectedTenant.Configurations[editIndex].EndDate));
                    else
                        this._dpEndDateFrom.clear();
                    this._txtTenantConfigurationNote.set_value(this._currentSelectedTenant.Configurations[editIndex].Note);
                    this._txtTenantConfigurationJsonValue.set_value(this._currentSelectedTenant.Configurations[editIndex].JsonValue);
                    var item = this._cmbConfigurationType.findItemByValue(this._currentSelectedTenant.Configurations[editIndex].ConfigurationType);
                    item.select();

                    this.currentTenantConfigurationUniqueId = this._currentSelectedTenant.Configurations[editIndex].UniqueId;
                    this._rwTenantConfiguration.show();
                } else {
                    alert("Selezionare un configurazione");
                }
                args.set_cancel(true);
                break;
            case 2:
                if (this._rtvTenantConfigurations.get_selectedNode() !== null) {
                    this._manager.radconfirm("Sei sicuro di voler eliminare il configurazione selezionato?", (arg) => {
                        if (arg) {
                            let tenantToUpdate: TenantViewModel = this.constructTenant();
                            tenantToUpdate.Configurations = this._currentSelectedTenant.Configurations.filter(x => x.UniqueId == this._rtvTenantConfigurations.get_selectedNode().get_value());
                            let removeIndex = this._currentSelectedTenant.Configurations.map(item => item.UniqueId).indexOf(this._rtvTenantConfigurations.get_selectedNode().get_value());
                            this._currentSelectedTenant.Configurations.splice(removeIndex, 1);
                            this._tenantService.updateTenant(tenantToUpdate, UpdateActionType.TenantConfigurationRemove,
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
                        }
                    }, 400, 300);
                } else {
                    alert("Selezionare un configurazione");
                }
                args.set_cancel(true);
                break;
        }
    }

    btnTenantConfigurationOk_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        if (this._dpStartDateFrom && this._dpStartDateFrom.get_selectedDate() &&
            this._txtTenantConfigurationJsonValue && this._txtTenantConfigurationJsonValue.get_textBoxValue() !== "" &&
            this._cmbConfigurationType && this._cmbConfigurationType.get_selectedItem().get_text() !== "") {
            let viewModelMapper = new TenantViewModelMapper();
            let tenantConfiguration: TenantConfigurationModel = {
                Tenant: viewModelMapper.Map(this._currentSelectedTenant),
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
                this._currentSelectedTenant.Configurations.push(tenantConfiguration);
                let tenantToUpdate: TenantViewModel = this.constructTenant();
                tenantToUpdate.Configurations = [tenantConfiguration];
                this._tenantService.updateTenant(tenantToUpdate, UpdateActionType.TenantConfigurationAdd,
                    (data) => {
                        let selectedIndex = this._currentSelectedTenant.Configurations.map(item => item.UniqueId)
                            .indexOf(tenantConfiguration.UniqueId);
                        this._currentSelectedTenant.Configurations[selectedIndex].UniqueId = data.Configurations.$values[selectedIndex].UniqueId;
                        nodeValue = this._currentSelectedTenant.Configurations[selectedIndex].UniqueId;
                        this.addNodesToRadTreeView(nodeValue, nodeText, "Configurazioni", nodeImageUrl, this._rtvTenantConfigurations);
                    },
                    (exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.splitterMainId);
                        $("#".concat(this.rtvTenantsId)).hide();
                        this.showNotificationException(this.uscNotificationId, exception);
                    });
            } else {
                let editIndex = this._currentSelectedTenant.Configurations.map(item => item.UniqueId)
                    .indexOf(this._rtvTenantConfigurations.get_selectedNode().get_value());
                this._currentSelectedTenant.Configurations[editIndex] = tenantConfiguration;
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

    btnTenantConfigurationCancel_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        this._rwTenantConfiguration.close();
    }

    private populateTenantConfigurationsTreeView() {
        this._tenantService.getTenantConfigurations(this._currentSelectedTenant.UniqueId,
            (data: TenantConfigurationModel[]) => {
                if (data === undefined) {
                    return;
                } else {
                    this._rtvTenantConfigurations.get_nodes().clear();
                    let thisObj = this;
                    this._currentSelectedTenant.Configurations = data;
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

    constructTenant(): TenantViewModel {
        let newTenant: TenantViewModel = new TenantViewModel();
        newTenant.UniqueId = this._currentSelectedTenant.UniqueId;
        newTenant.CompanyName = this._currentSelectedTenant.CompanyName;
        newTenant.TenantName = this._currentSelectedTenant.TenantName;
        newTenant.Note = this._currentSelectedTenant.Note;
        newTenant.StartDate = this._currentSelectedTenant.StartDate;
        newTenant.EndDate = this._currentSelectedTenant.EndDate;
        newTenant.TenantAOO = this._currentSelectedTenant.TenantAOO;
        return newTenant;
    }
}
export = TbltTenant;