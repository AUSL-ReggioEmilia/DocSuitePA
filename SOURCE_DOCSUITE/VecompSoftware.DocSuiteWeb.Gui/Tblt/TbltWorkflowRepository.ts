/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import WorkflowRoleMappingModel = require('App/Models/Workflows/WorkflowRoleMappingModel');
import WorkflowRepositoryModel = require('App/Models/Workflows/WorkflowRepositoryModel');
import WorkflowEvaluationProperty = require('App/Models/Workflows/WorkflowEvaluationProperty');
import WorkflowEvalutionPropertyHelper = require('App/Models/Workflows/WorkflowEvalutionPropertyHelper');
import WorkflowRoleMappingService = require('App/Services/Workflows/WorkflowRoleMappingService');
import WorkflowRepositoryService = require('App/Services/Workflows/WorkflowRepositoryService');
import DomainUserService = require('App/Services/Securities/DomainUserService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import WorkflowAuthorizationType = require('App/Models/Workflows/WorkflowAuthorizationType');
import ActivityModel = require('App/Models/Workflows/ActivityModel');
import DomainUserModel = require('App/Models/Securities/DomainUserModel');
import WorkflowActivityViewModel = require('App/ViewModels/Workflows/WorkflowActivityViewModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import WorkflowEvaluationPropertyService = require('App/Services/Workflows/WorkflowEvaluationPropertyService');
import WorkflowStep = require('App/Models/Workflows/WorkflowStep');
import WorkflowArgumentModel = require('App/Models/Workflows/WorkflowArgumentModel');
import ArgumentType = require('App/Models/Workflows/ArgumentType');
import WorkflowTreeNodeType = require('App/Models/Workflows/WorkflowTreeNodeType');
import OpenWindowOperationType = require('App/Models/Workflows/OpenWindowOperationType');
import WorkflowArgumentType = require('App/Models/Workflows/WorkflowArgumentType');
import DSWEnvironmentType = require('App/Models/Workflows/WorkflowDSWEnvironmentType');
import uscRoleRest = require('UserControl/uscRoleRest');
import RoleModel = require('App/Models/Commons/RoleModel');
import RoleModelMapper = require('App/Mappers/Commons/RoleModelMapper');
import WorkflowRepositoryStatus = require('App/Models/Workflows/WorkflowRepositoryStatus');
import EnumHelper = require('App/Helpers/EnumHelper');
import ActivityType = require('App/Models/Workflows/ActivityType');
import ActivityArea = require('App/Models/Workflows/ActivityArea');
import QueryParameters = require('App/Models/Workflows/QueryStringModels/QueryParametersWorkflowEvaluationProperty')
import WorkflowPropertyHelper = require('App/Models/Workflows/WorkflowPropertyHelper');
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');

class TbltWorkflowRepository {
    ajaxManagerId: string;
    rtvWorkflowRepositoryId: string;
    ajaxLoadingPanelId: string;
    rcbSelectMappingTagId: string;
    mappingDataSourceId: string;
    uscNotificationId: string;
    uscRoleRestId: string;
    workflowEnvironment: string;

    //radGrids
    rgvStepInputPropertiesId: string;
    rgvStepEvaluationPropertiesId: string;
    rgvStepOutputPropertiesId: string;
    rgvXamlWorkflowRoleMappingsId: string;
    rgvWorkflowStartUpId: string;
    rgvWorkflowRoleMappingsId: string;

    //panels
    pnlSelectMappingTagId: string;
    pnlDetailsId: string;
    pnlStepInformationsId: string;
    pnlRepositoryInformationsId: string;
    pnlBarDetailsId: string;
    pnlSelectRolesId: string;

    //labels
    lblStatusId: string;
    lblVersionId: string;
    lblActiveFromId: string;
    lblActiveToId: string;
    lblTipoligiaId: string;
    lblPositionId: string;
    lblStepNameId: string;
    lblAutorizationTypeId: string;
    lblActivityTypeId: string;
    lblAreaId: string;
    lblActionId: string;


    //buttons
    btnAddId: string;
    btnEditId: string;
    btnDeleteId: string;
    btnSelectMappingTagId: string;
    btnAggiungiId: string;
    btnModificaId: string;
    btnEliminaId: string;
    btnAddInputArgumentId: string;
    btnEditInputArgumentId: string;
    btnDeleteInputArgumentId: string;
    btnAddEvaluationArgumentId: string;
    btnEditEvaluationArgumentId: string;
    btnDeleteEvaluationArgumentId: string;
    btnAddOutputArgumentId: string;
    btnEditOutputArgumentId: string;
    btnDeleteOutputArgumentId: string;

    //toolbars
    ToolBarSearchId: string;
    ToolBarStepId: string;

    //windows
    windowAddWorkflowRoleMappingId: string;
    rwmWorkflowStepId: string;
    rwWorkflowPropertyId: string;
    rwWorkflowRepositoryId: string;
    radWindowManagerId: string;

    repositories: WorkflowRepositoryModel[];

    private static NODETYPE_ATTRNAME: string = "NodeType";
    private static COMMANDNAME_ADD: string = "ADD";
    private static COMMANDNAME_EDIT: string = "EDIT";
    private static COMMANDNAME_REMOVE: string = "REMOVE";
    private static TAG_PANEL: string = "Tag";
    private static STARTUP_PANEL: string = "Startup";
    private static INPUT_ARG_PANEL: string = "Input";
    private static EVALUATION_ARG_PANEL: string = "Evaluation";
    private static OUTPUT_ARG_PANEL: string = "Output";

    //_dsw_p_WorkflowStartProposer - Proponente Di Avio (values 0<Settore> or 1<Utente>)
    private static PROPERTY_NAME_WORKFLOW_START_PROPOSER = WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_PROPOSER;
    //_dsw_p_WorkflowStartRecipient - Destinatario Di Avio (values 0<Settore> or 1<Utente>)
    private static PROPERTY_NAME_WORKFLOW_START_RECIPIENT = WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_RECIPIENT;
    //_dsw_p_WorkflowDefaultProposer - Proponente Di Default
    private static PROPERTY_NAME_WORKFLOW_DEFAULT_PROPOSER = WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_DEFAULT;
    //_dsw_p_WorkflowDefaultRecipient - Destinatario Di Default
    private static PROPERTY_NAME_WORKFLOW_DEFAULT_RECIPIENT = WorkflowPropertyHelper.DSW_PROPERTY_RECIPIENT_DEFAULT;

    private _enumHelper: EnumHelper;
    private _windowAddWorkflowRoleMapping: Telerik.Web.UI.RadWindow;
    private _rtvWorkflowRepository: Telerik.Web.UI.RadTreeView;
    private _rgvWorkflowRoleMappings: Telerik.Web.UI.RadGrid;
    private _rgvXamlWorkflowRoleMappings: Telerik.Web.UI.RadGrid;
    private _rgvStepInputProperties: Telerik.Web.UI.RadGrid;
    private _rgvStepEvaluationProperties: Telerik.Web.UI.RadGrid;
    private _rgvStepOutputProperties: Telerik.Web.UI.RadGrid;
    private _btnAggiungi: Telerik.Web.UI.RadButton;
    private _btnModifica: Telerik.Web.UI.RadButton;
    private _btnElimina: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _rcbSelectMappingTag: Telerik.Web.UI.RadComboBox;
    private _btnSelectMappingTag: Telerik.Web.UI.RadButton;
    private _mappingDataSource: Telerik.Web.UI.RadClientDataSource;
    private _workflowRoleMappingService: WorkflowRoleMappingService;
    private _workflowRepositoryService: WorkflowRepositoryService;
    private _domainUserService: DomainUserService;
    private _serviceConfigurations: ServiceConfiguration[];

    private _btnAdd: Telerik.Web.UI.RadButton;
    private _btnEdit: Telerik.Web.UI.RadButton;
    private _btnDelete: Telerik.Web.UI.RadButton;
    private _btnAddInputArguments: Telerik.Web.UI.RadButton;
    private _btnEditInputArguments: Telerik.Web.UI.RadButton;
    private _btnDeleteInputArguments: Telerik.Web.UI.RadButton;
    private _btnAddEvaluationArguments: Telerik.Web.UI.RadButton;
    private _btnEditEvaluationArguments: Telerik.Web.UI.RadButton;
    private _btnDeleteEvaluationArguments: Telerik.Web.UI.RadButton;
    private _btnAddOutputArguments: Telerik.Web.UI.RadButton;
    private _btnEditOutputArguments: Telerik.Web.UI.RadButton;
    private _btnDeleteOutputArguments: Telerik.Web.UI.RadButton;

    private _rgvWorkflowStartUp: Telerik.Web.UI.RadGrid;
    private _workflowEvaluationPropertyService: WorkflowEvaluationPropertyService;

    private _toolbarSearch: Telerik.Web.UI.RadToolBar;
    private _toolbarItemSearchName: Telerik.Web.UI.RadToolBarItem;
    private _txtSearchName: Telerik.Web.UI.RadTextBox;
    private _toolbarItemButtonSearch: Telerik.Web.UI.RadToolBarItem;

    private _toolbarStep: Telerik.Web.UI.RadToolBar;
    private _rwmWorkflowStep: Telerik.Web.UI.RadWindowManager;
    private _rwWorkflowProperty: Telerik.Web.UI.RadWindowManager;
    private _rwWorkflowRepository: Telerik.Web.UI.RadWindowManager;
    private _confirmWindowManager: Telerik.Web.UI.RadWindowManager;
    private _pnlBarDetails: Telerik.Web.UI.RadPanelBar;

    private _uscRoleRest: uscRoleRest;
    private _currentWorkflowRepositoryId: string;

    private _currentWorkflowRepositoryModel: WorkflowRepositoryModel;

    /**
     * Costruttore
     * @param serviceConfigurations
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
    }

    /**
     * Inizializzazione classe
     */
    initialize(): void {
        $('#'.concat(this.pnlDetailsId)).hide();
        $('#'.concat(this.btnAggiungiId)).hide();
        $('#'.concat(this.btnModificaId)).hide();
        $('#'.concat(this.btnEliminaId)).hide();
        this._uscRoleRest = <uscRoleRest>$(`#${this.uscRoleRestId}`).data();

        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._confirmWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);

        this._windowAddWorkflowRoleMapping = <Telerik.Web.UI.RadWindow>$find(this.windowAddWorkflowRoleMappingId);
        this._windowAddWorkflowRoleMapping.add_close(this.windowAddWorkflowRoleMapping_onClose);
        this._rtvWorkflowRepository = <Telerik.Web.UI.RadTreeView>$find(this.rtvWorkflowRepositoryId);
        this._rtvWorkflowRepository.add_nodeClicked(this.rtvWorkflowRepository_nodeClicked);
        this._rgvWorkflowRoleMappings = <Telerik.Web.UI.RadGrid>$find(this.rgvWorkflowRoleMappingsId);
        this._rgvXamlWorkflowRoleMappings = <Telerik.Web.UI.RadGrid>$find(this.rgvXamlWorkflowRoleMappingsId);
        this._rgvStepInputProperties = <Telerik.Web.UI.RadGrid>$find(this.rgvStepInputPropertiesId);
        this._rgvStepEvaluationProperties = <Telerik.Web.UI.RadGrid>$find(this.rgvStepEvaluationPropertiesId);
        this._rgvStepOutputProperties = <Telerik.Web.UI.RadGrid>$find(this.rgvStepOutputPropertiesId);

        this._rcbSelectMappingTag = <Telerik.Web.UI.RadComboBox>$find(this.rcbSelectMappingTagId);
        this._mappingDataSource = <Telerik.Web.UI.RadClientDataSource>$find(this.mappingDataSourceId);

        this._btnAggiungi = <Telerik.Web.UI.RadButton>$find(this.btnAggiungiId);
        this._btnAggiungi.add_clicked(this.btnAggiungi_onClick);
        this._btnModifica = <Telerik.Web.UI.RadButton>$find(this.btnModificaId);
        this._btnModifica.add_clicked(this.btnModifica_onClick);
        this._btnElimina = <Telerik.Web.UI.RadButton>$find(this.btnEliminaId);
        this._btnElimina.add_clicked(this.btnDelete_onClick);
        this._btnSelectMappingTag = <Telerik.Web.UI.RadButton>$find(this.btnSelectMappingTagId);
        this._btnSelectMappingTag.add_clicked(this.btnSelectMappingTag_onClick);

        this._btnAdd = <Telerik.Web.UI.RadButton>$find(this.btnAddId);
        this._btnAdd.add_clicked(this.btnAdd_onClick);
        this._btnEdit = <Telerik.Web.UI.RadButton>$find(this.btnEditId);
        this._btnEdit.add_clicked(this.btnEdit_onClick);
        this._btnDelete = <Telerik.Web.UI.RadButton>$find(this.btnDeleteId);
        this._btnDelete.add_clicked(this.btnDeleteStartup_onClick);
        this._rgvWorkflowStartUp = <Telerik.Web.UI.RadGrid>$find(this.rgvWorkflowStartUpId);

        this._btnAddInputArguments = <Telerik.Web.UI.RadButton>$find(this.btnAddInputArgumentId);
        this._btnAddInputArguments.add_clicked(this.btnAddInputArguments_onClick);
        this._btnEditInputArguments = <Telerik.Web.UI.RadButton>$find(this.btnEditInputArgumentId);
        this._btnEditInputArguments.add_clicked(this.btnEditInputArguments_onClick);
        this._btnDeleteInputArguments = <Telerik.Web.UI.RadButton>$find(this.btnDeleteInputArgumentId);
        this._btnDeleteInputArguments.add_clicked(this.btnDeleteInputArguments_onClick);

        this._btnAddEvaluationArguments = <Telerik.Web.UI.RadButton>$find(this.btnAddEvaluationArgumentId);
        this._btnAddEvaluationArguments.add_clicked(this.btnAddEvaluationArguments_onClick);
        this._btnEditEvaluationArguments = <Telerik.Web.UI.RadButton>$find(this.btnEditEvaluationArgumentId);
        this._btnEditEvaluationArguments.add_clicked(this.btnEditEvaluationArguments_onClick);
        this._btnDeleteEvaluationArguments = <Telerik.Web.UI.RadButton>$find(this.btnDeleteEvaluationArgumentId);
        this._btnDeleteEvaluationArguments.add_clicked(this.btnDeleteEvaluationArguments_onClick);

        this._btnAddOutputArguments = <Telerik.Web.UI.RadButton>$find(this.btnAddOutputArgumentId);
        this._btnAddOutputArguments.add_clicked(this.btnAddOutputArguments_onClick);
        this._btnEditOutputArguments = <Telerik.Web.UI.RadButton>$find(this.btnEditOutputArgumentId);
        this._btnEditOutputArguments.add_clicked(this.btnEditOutputArguments_onClick);
        this._btnDeleteOutputArguments = <Telerik.Web.UI.RadButton>$find(this.btnDeleteOutputArgumentId);
        this._btnDeleteOutputArguments.add_clicked(this.btnDeleteOutputArguments_onClick);

        this._toolbarSearch = <Telerik.Web.UI.RadToolBar>$find(this.ToolBarSearchId);
        this._toolbarSearch.add_buttonClicking(this.toolbarSearch_onClick);
        this._toolbarItemSearchName = this._toolbarSearch.findItemByValue("searchName");
        this._txtSearchName = <Telerik.Web.UI.RadTextBox>this._toolbarItemSearchName.findControl("txtName");
        this._toolbarItemButtonSearch = <Telerik.Web.UI.RadToolBarItem>this._toolbarSearch.findItemByValue("searchCommand");

        this._toolbarStep = <Telerik.Web.UI.RadToolBar>$find(this.ToolBarStepId);
        this._toolbarStep.add_buttonClicking(this.toolbarStep_onClick);

        this._rwmWorkflowStep = <Telerik.Web.UI.RadWindowManager>$find(this.rwmWorkflowStepId);
        this._rwmWorkflowStep.add_close(this._rwAddWorkflowStep_onClose);

        this._rwWorkflowProperty = <Telerik.Web.UI.RadWindowManager>$find(this.rwWorkflowPropertyId);
        this._rwWorkflowProperty.add_close(this._rwWorkflowProperty_onClose);

        this._rwWorkflowRepository = <Telerik.Web.UI.RadWindowManager>$find(this.rwWorkflowRepositoryId);
        this._rwWorkflowRepository.add_close(this._rwWorkflowRepository_onClose);

        this._pnlBarDetails = <Telerik.Web.UI.RadPanelBar>$find(this.pnlBarDetailsId);

        let workflowRoleMappingConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowRoleMapping');
        this._workflowRoleMappingService = new WorkflowRoleMappingService(workflowRoleMappingConfiguration);

        let workflowRepositoryConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowRepository');
        this._workflowRepositoryService = new WorkflowRepositoryService(workflowRepositoryConfiguration);

        let domainUserConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'DomainUserModel');
        this._domainUserService = new DomainUserService(domainUserConfiguration);

        let workflowEvaluationProperty = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowEvaluationProperty");
        this._workflowEvaluationPropertyService = new WorkflowEvaluationPropertyService(workflowEvaluationProperty);

        this.repositories = new Array<WorkflowRepositoryModel>();
        this.searchWorkflows();
    }

    /**
     *------------------------- Events -----------------------------
     */

    /**
     * Evento scatenato alla selezione di un workflow
     * @param sender
     * @param args
     */
    private rtvWorkflowRepository_nodeClicked = (sender: Telerik.Web.UI.RadTreeView, args: Telerik.Web.UI.RadTreeNodeEventArgs) => {
        $('#'.concat(this.pnlDetailsId)).show();
        $('#'.concat(this.btnModificaId)).show();
        $('#'.concat(this.btnEliminaId)).show();

        let clickedNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.get_selectedNode();

        if (!clickedNode) {
            return;
        }
        let nodeType: WorkflowTreeNodeType = clickedNode.get_attributes().getAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME);
        if (nodeType === WorkflowTreeNodeType.Workflow) {
            this.registerUscRoleRestEventHandlers();
            this._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_ADD).set_enabled(true);
            this._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_ADD).set_toolTip("Aggiungi workflow step");
            this._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_EDIT).set_enabled(true);
            this._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_EDIT).set_toolTip("Modifica workflow repository");
            this._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_REMOVE).set_enabled(false);
            $('#'.concat(this.pnlRepositoryInformationsId)).show();
            $('#'.concat(this.pnlStepInformationsId)).hide();
            $('#'.concat(this.pnlSelectRolesId)).show();
            this._pnlBarDetails.findItemByValue(TbltWorkflowRepository.TAG_PANEL).show();
            this._pnlBarDetails.findItemByValue(TbltWorkflowRepository.STARTUP_PANEL).show();
            this._pnlBarDetails.findItemByValue(TbltWorkflowRepository.INPUT_ARG_PANEL).hide();
            this._pnlBarDetails.findItemByValue(TbltWorkflowRepository.EVALUATION_ARG_PANEL).hide();
            this._pnlBarDetails.findItemByValue(TbltWorkflowRepository.OUTPUT_ARG_PANEL).hide();
            this._currentWorkflowRepositoryId = clickedNode.get_value();
            this._workflowRepositoryService.getWorkflowRepositoryRoles(this._currentWorkflowRepositoryId, (data: RoleModel[]) => {
                this._uscRoleRest.renderRolesTree([]);
                if (data.length > 0) {
                    let roleModelMapper: RoleModelMapper = new RoleModelMapper();
                    let roles: RoleModel[] = roleModelMapper.MapCollection(data);
                    this._uscRoleRest.renderRolesTree(roles);
                }

            });
        }
        else if (nodeType === WorkflowTreeNodeType.Step) {
            this._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_ADD).set_enabled(false);
            this._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_EDIT).set_enabled(true);
            this._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_REMOVE).set_enabled(true);
            $('#'.concat(this.pnlRepositoryInformationsId)).hide();
            $('#'.concat(this.pnlStepInformationsId)).show();
            $('#'.concat(this.pnlSelectRolesId)).hide();
            this._pnlBarDetails.findItemByValue(TbltWorkflowRepository.TAG_PANEL).hide();
            this._pnlBarDetails.findItemByValue(TbltWorkflowRepository.STARTUP_PANEL).hide();
            this._pnlBarDetails.findItemByValue(TbltWorkflowRepository.INPUT_ARG_PANEL).show();
            this._pnlBarDetails.findItemByValue(TbltWorkflowRepository.EVALUATION_ARG_PANEL).show();
            this._pnlBarDetails.findItemByValue(TbltWorkflowRepository.OUTPUT_ARG_PANEL).show();
            this._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_EDIT).set_toolTip("Modifica workflow step");
            this._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_REMOVE).set_toolTip("Elimina workflow step");
        }
        else {
            this._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_ADD).set_enabled(true);
            this._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_ADD).set_toolTip("Aggiungi workflow repository");
            this._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_EDIT).set_enabled(false);
            this._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_REMOVE).set_enabled(false);
            $('#'.concat(this.pnlRepositoryInformationsId)).hide();
            $('#'.concat(this.pnlStepInformationsId)).hide();
            $('#'.concat(this.pnlDetailsId)).hide();
            $('#'.concat(this.pnlSelectRolesId)).hide();
        }
        if (nodeType != WorkflowTreeNodeType.Root) {
            this.loadDetails();
        }
    }

    /**
     * Evento scatenato al click del pulsante aggiungi
     * @param sender
     * @param args
     */
    private btnAggiungi_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.openManagementWindow('Add');
        return false;
    }

    /**
     * Evento scatenato al click del pulsante modifica
     * @param sender
     * @param args
     */
    private btnModifica_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.openManagementWindow('Edit');
        return false;
    }

    /**
     * Evento scatenato al click del pulsante elimina
     * @param sender
     * @param args
     */
    private btnDelete_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.get_selectedNode();
        let isXamlRepository: boolean = selectedNode.get_attributes().getAttribute('isXaml');
        let selectedMappingTags: Telerik.Web.UI.GridDataItem[];
        if (isXamlRepository) {
            selectedMappingTags = this._rgvXamlWorkflowRoleMappings.get_selectedItems();
        } else {
            selectedMappingTags = this._rgvWorkflowRoleMappings.get_selectedItems();
        }
        if (selectedMappingTags == undefined || selectedMappingTags.length == 0) {
            this.showWarningMessage(this.uscNotificationId, 'Nessun Tag selezionato');
            return false;
        }

        let model: WorkflowRoleMappingModel
        if (isXamlRepository) {
            let workflowActivityModel: WorkflowActivityViewModel = (<WorkflowActivityViewModel>selectedMappingTags[0].get_dataItem());
            if (workflowActivityModel.WorkflowRoleMapping.UniqueId == undefined) {
                this.showNotificationMessage(this.uscNotificationId, "Nessuna definizione di autorizzazione associata all'activity selezionata.");
                return;
            }
            model = <WorkflowRoleMappingModel>{ UniqueId: workflowActivityModel.WorkflowRoleMapping.UniqueId };
        } else {
            model = <WorkflowRoleMappingModel>{ UniqueId: (<WorkflowRoleMappingModel>selectedMappingTags[0].get_dataItem()).UniqueId };
        }

        this._workflowRoleMappingService.deleteWorkflowRoleMapping(model, (data) => this.loadDetails(),
            (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
                return;
            }
        );
    }

    /**
     * Evento scatenato al click del pulsante di ricerca workflow
     * @param sender
     * @param args
     */
    toolbarSearch_onClick = (sender: any, args: Telerik.Web.UI.RadToolBarCancelEventArgs) => {
        this.searchWorkflows();
        return false;
    }

    private searchWorkflows(): void {
        this._loadingPanel.show(this.rtvWorkflowRepositoryId);
        this._workflowRepositoryService.getByName(this._txtSearchName.get_textBoxValue(),
            (data) => {
                this.repositories = data;
                this.loadWorkflowRepositories(data);
                this._loadingPanel.hide(this.rtvWorkflowRepositoryId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.rtvWorkflowRepositoryId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    toolbarStep_onClick = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.get_selectedNode();
        let nodeType: WorkflowTreeNodeType = selectedNode.get_attributes().getAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME);
        let btn = args.get_item();
        switch (btn.get_index()) {
            case 0:
                if (nodeType === WorkflowTreeNodeType.Root) {
                    this.openWorkflowRepositoryGesWindow(OpenWindowOperationType.Add);
                }
                else {
                    this.openWorkflowStepGesWindow(OpenWindowOperationType.Add);
                }
                break;
            case 1:
                if (nodeType === WorkflowTreeNodeType.Workflow) {
                    this.openWorkflowRepositoryGesWindow(OpenWindowOperationType.Edit);
                }
                else {
                    this.openWorkflowStepGesWindow(OpenWindowOperationType.Edit);
                }
                break;
            case 2:
                let selectedNodeParent: Telerik.Web.UI.RadTreeNode = selectedNode.get_parent();
                this._confirmWindowManager.set_cssClass("remove");
                this._confirmWindowManager.radconfirm("Sei sicuro di voler eliminare il workflow step dal repository selezionato?", (arg) => {
                    if (arg) {
                        let workflowSteps: WorkflowStep[] = this.deleteWorkflowRepositoryStep(selectedNode.get_index(), selectedNodeParent.get_value());
                        this.redrawNodeWithPositions(selectedNodeParent, selectedNode, workflowSteps);
                    }
                });
                $('#'.concat(this.pnlDetailsId)).hide();
                break;
        }

    }

    private redrawNodeWithPositions(selectedNodeParent: Telerik.Web.UI.RadTreeNode, selectedNode: Telerik.Web.UI.RadTreeNode, workflowSteps: WorkflowStep[]) {
        let nodeLength = selectedNodeParent.get_allNodes().length - 1;

        for (let i = nodeLength; i >= 0; i--) {
            selectedNodeParent.get_nodes().remove(selectedNodeParent.get_nodes().getNode(i));
        }

        for (let i = 0; i < workflowSteps.length; i++) {
            let stepNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            stepNode.set_text(workflowSteps[i].Name);
            stepNode.set_value(workflowSteps[i].Position);
            stepNode.set_imageUrl('../Comm/Images/DocSuite/Resolution16.gif');
            stepNode.get_attributes().setAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME, WorkflowTreeNodeType.Step);
            selectedNodeParent.get_nodes().add(stepNode);
        }

    }

    private updateWorkflowRepositoryStep(workflowStep: WorkflowStep, uniqueId: string): void {
        let workflowRepositoryToUpdate: WorkflowRepositoryModel = this.repositories.filter(repository => repository.UniqueId == uniqueId)[0];
        let Json: WorkflowStep[] = JSON.parse(workflowRepositoryToUpdate.Json);
        let JsonToUpdate: WorkflowStep[] = Object.keys(Json).map(function (i) { return Json[i]; });
        let workflowStepToUpdate: WorkflowStep = JsonToUpdate.filter(step => step.Position == workflowStep.Position)[0];

        if (JsonToUpdate.indexOf(workflowStepToUpdate) >= 0) {
            JsonToUpdate[JsonToUpdate.indexOf(workflowStepToUpdate)] = workflowStep;

            let stepNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.get_selectedNode();
            stepNode.set_text(workflowStep.Name);
            this.fillWorkflowStepInformations(workflowStep);
        }
        else {
            JsonToUpdate.push(workflowStep);
            let stepNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            stepNode.set_text(workflowStep.Name);
            stepNode.set_value(workflowStep.Position);
            stepNode.set_imageUrl('../Comm/Images/DocSuite/Resolution16.gif');
            stepNode.get_attributes().setAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME, WorkflowTreeNodeType.Step);

            let parentNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.findNodeByValue(uniqueId);
            parentNode.get_nodes().add(stepNode);
        }

        let updatedJson = {};

        JsonToUpdate.forEach((step: WorkflowStep, idx: number) => {
            updatedJson[idx] = step;
        });

        workflowRepositoryToUpdate.Json = JSON.stringify(updatedJson);

        this._workflowRepositoryService.updateWorkflowRepository(workflowRepositoryToUpdate);
    }

    private deleteWorkflowRepositoryStep(workflowStepPosition: number, uniqueId: string): WorkflowStep[] {
        let workflowRepository: WorkflowRepositoryModel = this.repositories.filter(repository => repository.UniqueId == uniqueId)[0];
        let json: WorkflowStep[] = JSON.parse(workflowRepository.Json);
        let workflowSteps: WorkflowStep[] = Object.keys(json).map(function (i) { return json[i]; });
        workflowSteps = workflowSteps.filter(step => step.Position != workflowStepPosition);

        workflowSteps = this.updateWorkflowStepsPositions(workflowSteps, workflowStepPosition);

        let updatedJson = {};
        workflowSteps.forEach((step: WorkflowStep, idx: number) => {
            updatedJson[idx] = step;
        });

        workflowRepository.Json = JSON.stringify(updatedJson);

        this._workflowRepositoryService.updateWorkflowRepository(workflowRepository);
        return workflowSteps;
    }

    private updateWorkflowStepsPositions(workflowSteps: WorkflowStep[], startPosition: number): WorkflowStep[] {
        for (let i = startPosition; i < workflowSteps.length; i++) {
            workflowSteps[i].Position--;
        }
        return workflowSteps;
    }

    private updateWorkflowStepArgument(argument: WorkflowArgumentModel, uniqueId: string, stepPosition: number, type: number): void {
        let workflowRepositoryToUpdate: WorkflowRepositoryModel = this.repositories.filter(repository => repository.UniqueId == uniqueId)[0];
        let Json: WorkflowStep[] = JSON.parse(workflowRepositoryToUpdate.Json);
        let JsonToUpdate: WorkflowStep[] = Object.keys(Json).map(function (i) { return Json[i]; });
        let workflowStepToUpdate: WorkflowStep = JsonToUpdate.filter(step => step.Position == stepPosition)[0];
        switch (type) {
            case WorkflowArgumentType.Input: {
                let inputArguments: WorkflowArgumentModel[] = workflowStepToUpdate.InputArguments ? workflowStepToUpdate.InputArguments : new Array<WorkflowArgumentModel>();
                let argumentToUpdate: WorkflowArgumentModel = inputArguments.filter(arg => arg.Name == argument.Name)[0];
                if (inputArguments.indexOf(argumentToUpdate) >= 0) {
                    inputArguments[inputArguments.indexOf(argumentToUpdate)] = argument;
                }
                else {
                    inputArguments.push(argument);
                }
                this.updateWorkflowStepArgumentsTableView(inputArguments, uniqueId, WorkflowArgumentType.Input);
                workflowStepToUpdate.InputArguments = inputArguments;
                JsonToUpdate[JsonToUpdate.indexOf(workflowStepToUpdate)] = workflowStepToUpdate;
                break;
            }
            case WorkflowArgumentType.Evaluation: {
                let evaluationArguments: WorkflowArgumentModel[] = workflowStepToUpdate.EvaluationArguments ? workflowStepToUpdate.EvaluationArguments : new Array<WorkflowArgumentModel>();
                let argumentToUpdate: WorkflowArgumentModel = evaluationArguments.filter(arg => arg.Name == argument.Name)[0];
                if (evaluationArguments.indexOf(argumentToUpdate) >= 0) {
                    evaluationArguments[evaluationArguments.indexOf(argumentToUpdate)] = argument;
                }
                else {
                    evaluationArguments.push(argument);
                }
                this.updateWorkflowStepArgumentsTableView(evaluationArguments, uniqueId, WorkflowArgumentType.Evaluation);
                workflowStepToUpdate.EvaluationArguments = evaluationArguments;
                JsonToUpdate[JsonToUpdate.indexOf(workflowStepToUpdate)] = workflowStepToUpdate;
                break;
            }
            case WorkflowArgumentType.Output: {
                let outputArguments: WorkflowArgumentModel[] = workflowStepToUpdate.OutputArguments ? workflowStepToUpdate.OutputArguments : new Array<WorkflowArgumentModel>();
                let argumentToUpdate: WorkflowArgumentModel = outputArguments.filter(arg => arg.Name == argument.Name)[0];
                if (outputArguments.indexOf(argumentToUpdate) >= 0) {
                    outputArguments[outputArguments.indexOf(argumentToUpdate)] = argument;
                }
                else {
                    outputArguments.push(argument);
                }
                this.updateWorkflowStepArgumentsTableView(outputArguments, uniqueId, WorkflowArgumentType.Output);
                workflowStepToUpdate.OutputArguments = outputArguments;
                JsonToUpdate[JsonToUpdate.indexOf(workflowStepToUpdate)] = workflowStepToUpdate;
                break;
            }
        }

        let updatedJson = {};

        JsonToUpdate.forEach((step: WorkflowStep, idx: number) => {
            updatedJson[idx] = step;
        });

        workflowRepositoryToUpdate.Json = JSON.stringify(updatedJson);

        this._workflowRepositoryService.updateWorkflowRepository(workflowRepositoryToUpdate);
    }

    private deleteWorkflowStepArgument(argumentName: string, uniqueId: string, stepPosition: number, argumentType: number): void {
        let workflowRepository: WorkflowRepositoryModel = this.repositories.filter(repository => repository.UniqueId == uniqueId)[0];
        let json: WorkflowStep[] = JSON.parse(workflowRepository.Json);
        let workflowSteps: WorkflowStep[] = Object.keys(json).map(function (i) { return json[i]; });
        let workflowStepToUpdate: WorkflowStep = workflowSteps.filter(step => step.Position == stepPosition)[0];
        switch (argumentType) {
            case WorkflowArgumentType.Input: {
                let inputArguments: WorkflowArgumentModel[] = workflowStepToUpdate.InputArguments;
                inputArguments = inputArguments.filter(argument => argument.Name != argumentName);

                this.updateWorkflowStepArgumentsTableView(inputArguments, uniqueId, WorkflowArgumentType.Input);

                workflowStepToUpdate.InputArguments = inputArguments;
                workflowSteps[workflowSteps.indexOf(workflowStepToUpdate)] = workflowStepToUpdate;
                break;
            }
            case WorkflowArgumentType.Evaluation: {
                let evaluationArguments: WorkflowArgumentModel[] = workflowStepToUpdate.EvaluationArguments;
                evaluationArguments = evaluationArguments.filter(argument => argument.Name != argumentName);

                this.updateWorkflowStepArgumentsTableView(evaluationArguments, uniqueId, WorkflowArgumentType.Evaluation);

                workflowStepToUpdate.EvaluationArguments = evaluationArguments;
                workflowSteps[workflowSteps.indexOf(workflowStepToUpdate)] = workflowStepToUpdate;
                break;
            }
            case WorkflowArgumentType.Output: {
                let outputArguments: WorkflowArgumentModel[] = workflowStepToUpdate.OutputArguments;
                outputArguments = outputArguments.filter(argument => argument.Name != argumentName);

                this.updateWorkflowStepArgumentsTableView(outputArguments, uniqueId, WorkflowArgumentType.Output);

                workflowStepToUpdate.OutputArguments = outputArguments;
                workflowSteps[workflowSteps.indexOf(workflowStepToUpdate)] = workflowStepToUpdate;
                break;
            }
        }

        let updatedJson = {};

        workflowSteps.forEach((step: WorkflowStep, idx: number) => {
            updatedJson[idx] = step;
        });

        workflowRepository.Json = JSON.stringify(updatedJson);

        this._workflowRepositoryService.updateWorkflowRepository(workflowRepository);
    }

    private updateWorkflowStepArgumentsTableView(argumentsCollection: WorkflowArgumentModel[], uniqueId: string, argumentType: number) {
        let agumentsModel = this.populateArgumentsMasterViewTable(argumentsCollection, uniqueId);
        let workflowStepArgumentsTableView: Telerik.Web.UI.GridTableView;
        switch (argumentType) {
            case WorkflowArgumentType.Input:
                workflowStepArgumentsTableView = this._rgvStepInputProperties.get_masterTableView();
                break;
            case WorkflowArgumentType.Evaluation:
                workflowStepArgumentsTableView = this._rgvStepEvaluationProperties.get_masterTableView();
                break;
            case WorkflowArgumentType.Output:
                workflowStepArgumentsTableView = this._rgvStepOutputProperties.get_masterTableView();
                break;
        }
        workflowStepArgumentsTableView.set_dataSource(agumentsModel);
        workflowStepArgumentsTableView.clearSelectedItems();
        workflowStepArgumentsTableView.dataBind();
    }

    _rwAddWorkflowStep_onClose = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (!args.get_argument()) {
            return;
        }
        let workflowStep: WorkflowStep = args.get_argument()[0];
        let uniqueId: string = args.get_argument()[1];
        this.updateWorkflowRepositoryStep(workflowStep, uniqueId);
    }

    _rwWorkflowProperty_onClose = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (!args.get_argument()) {
            return;
        }
        let argObj = args.get_argument();
        let workflowArgument: WorkflowArgumentModel = argObj.WorkflowArgument;
        let uniqueId: string = argObj.UniqueId;
        let stepPosition: number = argObj.StepPosition;
        let argumentType: string = argObj.ArgumentType;

        this.updateWorkflowStepArgument(workflowArgument, uniqueId, stepPosition, WorkflowArgumentType[argumentType]);
    }

    _rwWorkflowRepository_onClose = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (!args.get_argument()) {
            return;
        }
        let argObj = args.get_argument();

        let workflowRepository: WorkflowRepositoryModel = argObj.WorkflowRepository;
        let action: string = argObj.Action;

        this.updateWorkflowRepository(workflowRepository, OpenWindowOperationType[action]);
    }

    private updateWorkflowRepository(workflowRepository: WorkflowRepositoryModel, action: OpenWindowOperationType) {
        if (action == OpenWindowOperationType.Add) {
            //insert workflow
            let rootNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.get_selectedNode();
            let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            node.get_attributes().setAttribute('isXaml', !String.isNullOrEmpty(workflowRepository.Xaml));
            node.set_text(workflowRepository.Name);
            node.set_value(workflowRepository.UniqueId);
            node.set_imageUrl('../Comm/Images/DocSuite/Workflow16.png');
            node.get_attributes().setAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME, WorkflowTreeNodeType.Workflow);
            rootNode.get_nodes().add(node);
            this.fillWorkflowRepositoryInformations(workflowRepository);

            this._workflowRepositoryService.insertWorkflowRepository(workflowRepository);

            this.repositories.push(workflowRepository);
        }
        else {
            //edit workflow
            let repositoryNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.findNodeByValue(workflowRepository.UniqueId);
            repositoryNode.set_text(workflowRepository.Name);
            this.fillWorkflowRepositoryInformations(workflowRepository);

            this._workflowRepositoryService.updateWorkflowRepository(workflowRepository);
        }
    }

    btnAddInputArguments_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.openWorkflowPropertyGesWindow(OpenWindowOperationType.Add, WorkflowArgumentType.Input);
    }

    btnEditInputArguments_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.openWorkflowPropertyGesWindow(OpenWindowOperationType.Edit, WorkflowArgumentType.Input);
    }

    btnDeleteInputArguments_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.get_selectedNode();
        let selectedItem: Telerik.Web.UI.GridDataItem = this._rgvStepInputProperties.get_selectedItems()[0];
        let argumentName: string = selectedItem.get_dataItem().Name;
        let stepPosition: number = selectedNode.get_value();
        let uniqueId: string = selectedNode.get_parent().get_value();
        let argumentType: number = WorkflowArgumentType.Input;
        this.deleteWorkflowStepArgument(argumentName, uniqueId, stepPosition, argumentType);
    }

    btnAddEvaluationArguments_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.openWorkflowPropertyGesWindow(OpenWindowOperationType.Add, WorkflowArgumentType.Evaluation);
    }

    btnEditEvaluationArguments_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.openWorkflowPropertyGesWindow(OpenWindowOperationType.Edit, WorkflowArgumentType.Evaluation);
    }

    btnDeleteEvaluationArguments_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.get_selectedNode();
        let selectedItem: Telerik.Web.UI.GridDataItem = this._rgvStepEvaluationProperties.get_selectedItems()[0];
        let argumentName: string = selectedItem.get_dataItem().Name;
        let stepPosition: number = selectedNode.get_value();
        let uniqueId: string = selectedNode.get_parent().get_value();
        let argumentType: number = WorkflowArgumentType.Evaluation;
        this.deleteWorkflowStepArgument(argumentName, uniqueId, stepPosition, argumentType);
    }

    btnAddOutputArguments_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.openWorkflowPropertyGesWindow(OpenWindowOperationType.Add, WorkflowArgumentType.Output);
    }

    btnEditOutputArguments_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.openWorkflowPropertyGesWindow(OpenWindowOperationType.Edit, WorkflowArgumentType.Output);
    }

    btnDeleteOutputArguments_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.get_selectedNode();
        let selectedItem: Telerik.Web.UI.GridDataItem = this._rgvStepOutputProperties.get_selectedItems()[0];
        let argumentName: string = selectedItem.get_dataItem().Name;
        let stepPosition: number = selectedNode.get_value();
        let uniqueId: string = selectedNode.get_parent().get_value();
        let argumentType: number = WorkflowArgumentType.Output;
        this.deleteWorkflowStepArgument(argumentName, uniqueId, stepPosition, argumentType);
    }


    openWorkflowStepGesWindow(operation: number) {
        this._rwmWorkflowStep.setSize(750, 550);
        let url: string = `../Tblt/TbltWorkflowStepGes.aspx?Type=Comm&Action=${OpenWindowOperationType[operation]}`;

        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.get_selectedNode();
        let parentNodeId: string = operation === OpenWindowOperationType.Add ? selectedNode.get_value() : selectedNode.get_parent().get_value();
        let workflowRepository: WorkflowRepositoryModel;
        this._workflowRepositoryService.getById(parentNodeId, (data: WorkflowRepositoryModel) => {
            workflowRepository = data;
            if (workflowRepository.Json === "{}") { /*If it's step 0*/
                workflowRepository.Json = this.initializeFirstStep();
            }
            sessionStorage.setItem(WorkflowTreeNodeType.Workflow.toString(), JSON.stringify(workflowRepository));

            if (operation === OpenWindowOperationType.Edit) {
                let steps = JSON.parse(workflowRepository.Json);
                let workflowSteps: WorkflowStep[] = Object.keys(steps).map(function (i) { return steps[i]; });
                let workflowStep: WorkflowStep = workflowSteps.filter(step => step.Position == selectedNode.get_value())[0];
                sessionStorage.setItem(WorkflowTreeNodeType.Step.toString(), JSON.stringify(workflowStep));
                this._rwmWorkflowStep.set_modal(true);
                this._rwmWorkflowStep.open(url, "windowWorkflowStep", undefined);
            }
        });
        if (operation === OpenWindowOperationType.Add) {
            this._rwmWorkflowStep.set_modal(true);
            this._rwmWorkflowStep.open(url, "windowWorkflowStep", undefined);
        }

    }

    private initializeFirstStep(): string {
        let wfStep = new Array<WorkflowStep>();
        let firstStep: WorkflowStep = {
            Position: -1, //it will be incresed to 0 when user presses confirm button.
            Name: "",
            AuthorizationType: null,
            ActivityType: null,
            ActivityOperation: {
                Action: null,
                Area: null
            },
            EvaluationArguments: new Array<WorkflowArgumentModel>(),
            InputArguments: new Array<WorkflowArgumentModel>(),
            OutputArguments: new Array<WorkflowArgumentModel>()
        };
        wfStep.push(firstStep);
        return JSON.stringify(wfStep);
    }

    openWorkflowPropertyGesWindow(operation: number, argumentType: number) {
        this._rwWorkflowProperty.setSize(650, 400);
        const validation: boolean = argumentType === WorkflowArgumentType.Evaluation;
        const url = `../Tblt/TbltWorkflowPropertyGes.aspx?Type=Comm&Action=${OpenWindowOperationType[operation]}&Argument=${WorkflowArgumentType[argumentType]}&Validation=${validation}`;

        const selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.get_selectedNode();
        const parentNodeId: string = selectedNode.get_parent().get_value();

        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_REPOSITORY, parentNodeId);
        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_STEP, selectedNode.get_value());

        let selectedItem;

        if (operation === OpenWindowOperationType.Edit) {
            switch (argumentType) {
                case WorkflowArgumentType.Input:
                    selectedItem = this._rgvStepInputProperties.get_selectedItems()[0].get_dataItem();
                    break;
                case WorkflowArgumentType.Evaluation:
                    selectedItem = this._rgvStepEvaluationProperties.get_selectedItems()[0].get_dataItem();
                    break;
                case WorkflowArgumentType.Output:
                    selectedItem = this._rgvStepOutputProperties.get_selectedItems()[0].get_dataItem();
                    break;
            }

            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_STEP_ARGUMENT, JSON.stringify({ Name: selectedItem.Name, Value: selectedItem.Value, UniqueId: selectedItem.UniqueId }));
        }

        this._rwWorkflowProperty.set_modal(true);
        this._rwWorkflowProperty.open(url, "windowWorkflowProperty", undefined);
    }

    openWorkflowRepositoryGesWindow(operation: number) {
        this._rwWorkflowRepository.setSize(750, 550);
        let url: string = `../Tblt/TbltRepositoryGes.aspx?Type=Comm&Action=${OpenWindowOperationType[operation]}`;

        if (operation === OpenWindowOperationType.Edit) {
            let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.get_selectedNode();
            let parentNodeId: string = selectedNode.get_value();
            let workflowRepository: WorkflowRepositoryModel;
            this._workflowRepositoryService.getById(parentNodeId, (data: WorkflowRepositoryModel) => {
                workflowRepository = data;
                sessionStorage.setItem(WorkflowTreeNodeType.Workflow.toString(), JSON.stringify(workflowRepository));
                this._rwWorkflowRepository.set_modal(true);
                this._rwWorkflowRepository.open(url, "windowWorkflowRepository", undefined);
            });
        }
        if (operation === OpenWindowOperationType.Add) {
            this._rwWorkflowRepository.set_modal(true);
            this._rwWorkflowRepository.open(url, "windowWorkflowRepository", undefined);
        }
    }
    /**
     * Evento scatenato al click del pulsante di ricerca Tag per workflow di tipo XAML
     * @param sender
     * @param args
     */
    private btnSelectMappingTag_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        let selectedMappingText: string = this._rcbSelectMappingTag.get_text();
        if (String.isNullOrEmpty(selectedMappingText) || selectedMappingText == this._rcbSelectMappingTag.get_emptyMessage()) return;
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.get_selectedNode();
        this._workflowRepositoryService.getById(selectedNode.get_value(),
            (response: any) => {
                if (response == undefined) return;
                try {
                    let model: WorkflowRepositoryModel = <WorkflowRepositoryModel>response;
                    let customActivities: ActivityModel[] = JSON.parse(model.CustomActivities);
                    if (customActivities.length == 0) {
                        this.showWarningMessage(this.uscNotificationId, 'Nessuna activity configurata per il Workflow selezionato.');
                        return;
                    }
                    this.fillXamlMappings(model, customActivities, selectedMappingText);
                }
                catch (error) {
                    this.showNotificationMessage(this.uscNotificationId, error.message);
                    console.log(JSON.stringify(error));
                }
            },
            (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    /**
     * Evento scatenato alla chiusura della finestra di gestione mapping
     * @param sender
     * @param args
     */
    windowAddWorkflowRoleMapping_onClose = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument() != undefined) {
            this.loadDetails();
        }
    }

    /**
     *------------------------- Methods -----------------------------
     */

    /**
     * Metodo per l'apertura delle finestre di gestione workflow
     * @param operation
     */
    openManagementWindow(operation: string): void {
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.get_selectedNode();
        if (selectedNode == undefined) {
            this.showWarningMessage(this.uscNotificationId, 'Nessuna attività selezionato');
            return;
        }

        let isXamlRepository: boolean = selectedNode.get_attributes().getAttribute('isXaml');
        let selectedMappingTags: Telerik.Web.UI.GridDataItem[];
        if (isXamlRepository) {
            selectedMappingTags = this._rgvXamlWorkflowRoleMappings.get_selectedItems();
        } else {
            selectedMappingTags = this._rgvWorkflowRoleMappings.get_selectedItems();
        }
        if (operation == 'Edit' && (selectedMappingTags == undefined || selectedMappingTags.length == 0)) {
            this.showWarningMessage(this.uscNotificationId, 'Nessun Tag selezionato');
            return;
        }

        if (operation == 'Edit' && selectedMappingTags.length > 1) {
            this.showWarningMessage(this.uscNotificationId, 'Selezionare un solo Tag per la modifica');
            return;
        }

        let qs: string = 'Action='.concat(operation, '&IdWorkflowRepository=', selectedNode.get_value());
        if (operation == 'Edit') {
            if (isXamlRepository) {
                let workflowActivityModel: WorkflowActivityViewModel = (<WorkflowActivityViewModel>selectedMappingTags[0].get_dataItem());
                if (workflowActivityModel.WorkflowRoleMapping.UniqueId != undefined) {
                    qs = qs.concat('&IdWorkflowRoleMapping=', workflowActivityModel.WorkflowRoleMapping.UniqueId);
                } else {
                    qs = 'Action=Add&IdWorkflowRepository='.concat(selectedNode.get_value());
                }
                qs = qs.concat('&MappingTag=', workflowActivityModel.MappingTag, '&FromXamlActivity=true&XamlInternalActivity=', workflowActivityModel.Activity.Id);
            } else {
                qs = qs.concat('&IdWorkflowRoleMapping=', (<WorkflowRoleMappingModel>selectedMappingTags[0].get_dataItem()).UniqueId);
            }
        }
        let url: string = '../Tblt/TbltWorkflowRepositoryGes.aspx?Type=Comm&'.concat(qs);

        this._windowAddWorkflowRoleMapping.setSize(750, 550);
        this._windowAddWorkflowRoleMapping.setUrl(url);
        this._windowAddWorkflowRoleMapping.set_modal(true);
        this._windowAddWorkflowRoleMapping.show();
    }

    /**
     * Metodo che imposta a video i workflow cercati
     * @param repositories
     */
    loadWorkflowRepositories(repositories: WorkflowRepositoryModel[]) {
        if (repositories == undefined || repositories.length == 0) {
            return;
        }

        try {
            this._rtvWorkflowRepository.beginUpdate();
            this._rtvWorkflowRepository.get_nodes().clear();

            let rootNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
            rootNode.set_text("Workflow Repositories");
            rootNode.set_imageUrl('../App_Themes/DocSuite2008/imgset16/process.png');
            rootNode.get_attributes().setAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME, WorkflowTreeNodeType.Root);
            rootNode.set_expanded(true);

            $.each(repositories, (index: number, repository: WorkflowRepositoryModel) => {
                let node: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
                node.get_attributes().setAttribute('isXaml', !String.isNullOrEmpty(repository.Xaml));
                node.set_text(repository.Name);
                node.set_value(repository.UniqueId);
                node.set_imageUrl('../Comm/Images/DocSuite/Workflow16.png');
                node.get_attributes().setAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME, WorkflowTreeNodeType.Workflow);

                if (repository.Json) {
                    let steps = JSON.parse(repository.Json);

                    $.each(steps, (index: number, step: WorkflowStep) => {
                        let stepNode: Telerik.Web.UI.RadTreeNode = new Telerik.Web.UI.RadTreeNode();
                        stepNode.set_text(step.Name);
                        stepNode.set_value(step.Position);
                        stepNode.set_imageUrl('../App_Themes/DocSuite2008/imgset16/link.png');
                        stepNode.get_attributes().setAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME, WorkflowTreeNodeType.Step);
                        node.get_nodes().add(stepNode);
                    });
                }
                rootNode.get_nodes().add(node);
            });
            this._rtvWorkflowRepository.get_nodes().add(rootNode);
            this._rtvWorkflowRepository.endUpdate();
        }
        catch (error) {
            this.showNotificationMessage(this.uscNotificationId, error.message);
            console.log(JSON.stringify(error));
        }
    }

    private registerUscRoleRestEventHandlers(): void {
        let uscRoleRestEvents = this._uscRoleRest.uscRoleRestEvents;

        this._uscRoleRest.registerEventHandler(uscRoleRestEvents.NewRolesAdded, this.addWorkflowRolesPromise);
        this._uscRoleRest.registerEventHandler(uscRoleRestEvents.RoleDeleted, this.deleteWorkflowRolePromise);
    }

    private deleteWorkflowRolePromise = (roleIdToDelete: number, instanceId?: string): JQueryPromise<any> => {
        let promise: JQueryDeferred<any> = $.Deferred<any>();
        if (!roleIdToDelete) {
            return promise.promise();
        }

        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.get_selectedNode();
        let currentWorkflowRepository: WorkflowRepositoryModel = this.repositories.filter(r => r.UniqueId == selectedNode.get_value())[0];
        currentWorkflowRepository.Roles = currentWorkflowRepository.Roles.filter(r => r.EntityShortId !== roleIdToDelete);
        this._workflowRepositoryService.updateWorkflowRepository(currentWorkflowRepository, (data) => {
            promise.resolve(data);
        })

        return promise.promise();
    }

    private addWorkflowRolesPromise = (rolesToAdd: RoleModel[], instanceId?: string): JQueryPromise<any> => {
        let promise: JQueryDeferred<any> = $.Deferred<any>();
        if (!rolesToAdd.length) {
            return promise.promise();
        }
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.get_selectedNode();
        let currentWorkflowRepository: WorkflowRepositoryModel = this.repositories.filter(r => r.UniqueId == selectedNode.get_value())[0];
        currentWorkflowRepository.Roles = [...currentWorkflowRepository.Roles, ...rolesToAdd];

        this._workflowRepositoryService.updateWorkflowRepository(currentWorkflowRepository, (data) => {
            promise.resolve(data);
        },
            (exception: ExceptionDTO) => {
                this.showNotificationException(this.uscNotificationId, exception);
            });
        return promise.promise();
    }
    /**
     * Pulisce i datasource delle tabelle di visualizzazione mapping
     */
    private clearDataSources(): void {
        let workflowRoleMappingsMasterTableView: Telerik.Web.UI.GridTableView = this._rgvWorkflowRoleMappings.get_masterTableView();
        let xamlWorkflowRoleMappingsMasterTableView: Telerik.Web.UI.GridTableView = this._rgvXamlWorkflowRoleMappings.get_masterTableView();

        workflowRoleMappingsMasterTableView.set_dataSource([]);
        workflowRoleMappingsMasterTableView.dataBind();

        xamlWorkflowRoleMappingsMasterTableView.set_dataSource([]);
        xamlWorkflowRoleMappingsMasterTableView.dataBind();

    }

    /**
     * Aggiorna il datasource dei Tag disponibili per i workflow di tipo XAML
     */
    private fillMappingSelection(): JQueryPromise<void> {
        let result: JQueryDeferred<void> = $.Deferred<void>();
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.get_selectedNode();
        this._rcbSelectMappingTag.clearItems();
        let nodeType: WorkflowTreeNodeType = selectedNode.get_attributes().getAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME);
        let repositoryId: string = nodeType === WorkflowTreeNodeType.Workflow ? selectedNode.get_value() : selectedNode.get_parent().get_value();

        this._workflowRoleMappingService.getByName('', repositoryId,
            (response: any) => {
                if (response == undefined) return result.resolve();
                try {
                    let models: WorkflowRoleMappingModel[] = <WorkflowRoleMappingModel[]>response;
                    if (models.length > 0) {
                        (<any>this._mappingDataSource).set_data(models);
                    } else {
                        (<any>this._mappingDataSource).set_data('[{}]');
                    }
                    (<any>this._mappingDataSource).fetch();
                    return result.resolve();
                }
                catch (error) {
                    console.log(JSON.stringify(error));
                    return result.reject(error);
                }
            },
            (exception: ExceptionDTO) => {
                return result.reject(exception);
            }
        );

        return result.promise();
    }

    /**
     * Imposta la visibilità degli elementi del pannello dettagi in base
     * al tipo di workflow gestito
     * @param model
     */
    private setDetailsVisibility(model: WorkflowRepositoryModel): void {
        if (String.isNullOrEmpty(model.Xaml)) {
            $('#workflowRoleMappings').show();
            $('#xamlWorkflowRoleMappings').hide();
            $('#'.concat(this.pnlSelectMappingTagId)).hide();
            $(this._btnAggiungi.get_element()).show();
        } else {
            $('#workflowRoleMappings').hide();
            $('#xamlWorkflowRoleMappings').show();
            $('#'.concat(this.pnlSelectMappingTagId)).show();
            $(this._btnAggiungi.get_element()).hide();
        }
    }

    /**
     * Imposta i dettagli del workflow selezionato
     */
    private fillDetailsPanel(): JQueryPromise<WorkflowRepositoryModel> {
        let result: JQueryDeferred<WorkflowRepositoryModel> = $.Deferred<WorkflowRepositoryModel>();
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.get_selectedNode();
        let nodeType: WorkflowTreeNodeType = selectedNode.get_attributes().getAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME);
        let repositoryId: string = nodeType === WorkflowTreeNodeType.Workflow ? selectedNode.get_value() :
            nodeType === WorkflowTreeNodeType.Step ? selectedNode.get_parent().get_value() : null;
        this._workflowRepositoryService.getById(repositoryId,
            (response: any) => {
                if (response == undefined) return result.reject();
                try {
                    let model: WorkflowRepositoryModel = <WorkflowRepositoryModel>response;
                    if (selectedNode.get_attributes().getAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME) === WorkflowTreeNodeType.Step) {
                        let jsonModel: WorkflowStep[] = JSON.parse(model.Json);
                        let workflowSteps: WorkflowStep[] = Object.keys(jsonModel).map(function (i) { return jsonModel[i]; });
                        let workflowStepModel: WorkflowStep = workflowSteps.filter(step => step.Position == selectedNode.get_value())[0];
                        this.fillWorkflowStepInformations(workflowStepModel);
                    }
                    this.fillWorkflowRepositoryInformations(model);
                    this.clearDataSources();
                    this.setDetailsVisibility(model);
                    return result.resolve(model);
                }
                catch (error) {
                    console.log(JSON.stringify(error));
                    return result.reject(error);
                }
            },
            (exception: ExceptionDTO) => {
                return result.reject(exception);
            }
        );
        return result.promise();
    }

    private fillWorkflowRepositoryInformations(workflowRepositoryModel: WorkflowRepositoryModel) {
        $('#'.concat(this.lblStatusId)).html(this.getStatusDescription(workflowRepositoryModel.Status.toString()));
        $('#'.concat(this.lblVersionId)).html(workflowRepositoryModel.Version.toString());
        if (workflowRepositoryModel.ActiveTo != undefined) {
            $('#'.concat(this.lblActiveToId)).html(moment(workflowRepositoryModel.ActiveTo).format("DD/MM/YYYY"));
        }
        $('#'.concat(this.lblActiveFromId)).html(moment(workflowRepositoryModel.ActiveFrom).format("DD/MM/YYYY"));
        if (parseInt(workflowRepositoryModel.DSWEnvironment.toString())) {
            $('#'.concat(this.lblTipoligiaId)).html(DSWEnvironmentType[workflowRepositoryModel.DSWEnvironment]);
        } else {
            $('#'.concat(this.lblTipoligiaId)).html(workflowRepositoryModel.DSWEnvironment.toString());
        }

        this.workflowEnvironment = workflowRepositoryModel.DSWEnvironment.toString();
    }

    private fillWorkflowStepInformations(workflowStepModel: WorkflowStep): void {
        $('#'.concat(this.lblPositionId)).html(workflowStepModel.Position.toString());
        $('#'.concat(this.lblStepNameId)).html(workflowStepModel.Name.toString());
        let authType: string = workflowStepModel.AuthorizationType != undefined ? workflowStepModel.AuthorizationType.toString() : "";
        $('#'.concat(this.lblAutorizationTypeId)).html(authType);
        let activityType: string = workflowStepModel.ActivityType != undefined ? workflowStepModel.ActivityType.toString() : "";
        if (Number(activityType)) {
            $('#'.concat(this.lblActivityTypeId)).html(this._enumHelper.getActivityTypeDescription(ActivityType[activityType]));
        }
        else {
            $('#'.concat(this.lblActivityTypeId)).html(this._enumHelper.getActivityTypeDescription(activityType));
        }

        let area: string = workflowStepModel.ActivityOperation.Area != undefined ? workflowStepModel.ActivityOperation.Area.toString() : "";
        if (Number(area)) {
            $('#'.concat(this.lblAreaId)).html(this._enumHelper.getActivityAreaDescription(ActivityArea[area]));
        }
        else {
            $('#'.concat(this.lblAreaId)).html(this._enumHelper.getActivityAreaDescription(area));
        }

        let action: string = workflowStepModel.ActivityOperation.Action != undefined ? workflowStepModel.ActivityOperation.Action.toString() : "";
        if (Number(action)) {
            $('#'.concat(this.lblActionId)).html(this._enumHelper.getWorkflowActivityActionDescription(Number(action)));
        }
        else {
            $('#'.concat(this.lblActionId)).html(action);
        }
    }

    /**
     * Imposta i mapping tag già gestiti per i workflow di tipo json
     * @param model
     */

    private fillJsonMappings(model: WorkflowRepositoryModel): void {
        try {
            if (String.isNullOrEmpty(model.Xaml)) {
                let workflowRoleMappingsMasterTableView: Telerik.Web.UI.GridTableView = this._rgvWorkflowRoleMappings.get_masterTableView();
                workflowRoleMappingsMasterTableView.set_dataSource(model.WorkflowRoleMappings);
                workflowRoleMappingsMasterTableView.clearSelectedItems();
                workflowRoleMappingsMasterTableView.dataBind();

                let startupModel = this.populateStartupMasterViewTable(model.WorkflowEvaluationProperties);
                let workflowStartupMasterTableView: Telerik.Web.UI.GridTableView = this._rgvWorkflowStartUp.get_masterTableView();
                workflowStartupMasterTableView.set_dataSource(startupModel);
                workflowStartupMasterTableView.clearSelectedItems();
                workflowStartupMasterTableView.dataBind();

                let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.get_selectedNode();
                let nodePosition: number = selectedNode.get_value();
                let nodeType: WorkflowTreeNodeType = selectedNode.get_attributes().getAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME);

                if (nodeType === WorkflowTreeNodeType.Step) {
                    let json: WorkflowStep[] = JSON.parse(model.Json);
                    let workflowSteps: WorkflowStep[] = Object.keys(json).map(function (i) { return json[i]; });
                    let workflowStep: WorkflowStep = workflowSteps.filter(step => step.Position == nodePosition)[0];

                    let inputArguments: WorkflowArgumentModel[] = workflowStep.InputArguments ? workflowStep.InputArguments : new Array<WorkflowArgumentModel>();;

                    this.updateWorkflowStepArgumentsTableView(inputArguments, model.UniqueId, WorkflowArgumentType.Input);

                    let evaluationArguments: WorkflowArgumentModel[] = workflowStep.EvaluationArguments ? workflowStep.EvaluationArguments : new Array<WorkflowArgumentModel>();;
                    this.updateWorkflowStepArgumentsTableView(evaluationArguments, model.UniqueId, WorkflowArgumentType.Evaluation);

                    let outputArguments: WorkflowArgumentModel[] = workflowStep.OutputArguments ? workflowStep.OutputArguments : new Array<WorkflowArgumentModel>();;
                    this.updateWorkflowStepArgumentsTableView(outputArguments, model.UniqueId, WorkflowArgumentType.Output);
                }

            }
        }
        catch (error) {
            this.showNotificationMessage(this.uscNotificationId, error.message);
            console.log(JSON.stringify(error));
        }
    }

    private populateStartupMasterViewTable(model: WorkflowEvaluationProperty[]) {
        let customModel: any[] = new Array();
        for (let wep of model) {
            if (wep.ValueInt != null) {
                customModel.push({ Name: wep.Name, Value: wep.ValueInt, UniqueId: wep.UniqueId });
            } else if (wep.ValueDate != null) {
                customModel.push({ Name: wep.Name, Value: wep.ValueDate, UniqueId: wep.UniqueId });
            }
            else if (wep.ValueDouble != null) {
                customModel.push({ Name: wep.Name, Value: wep.ValueDouble, UniqueId: wep.UniqueId });
            }
            else if (wep.ValueBoolean != null) {
                customModel.push({ Name: wep.Name, Value: wep.ValueBoolean, UniqueId: wep.UniqueId });
            }
            else if (wep.ValueGuid != null) {
                customModel.push({ Name: wep.Name, Value: wep.ValueGuid, UniqueId: wep.UniqueId });
            }
            else if (wep.ValueString != null) {
                customModel.push({ Name: wep.Name, Value: wep.ValueString, UniqueId: wep.UniqueId });
            }
        }
        return customModel;
    }

    private populateArgumentsMasterViewTable(model: WorkflowArgumentModel[], repositoryId: string) {
        let customModel: any[] = new Array();
        if (!model || model.length < 1 || Object.keys(model[0]).length < 1) {
            return customModel;
        }
        for (const argument of model) {
            let argumentPropertyType: ArgumentType = this._enumHelper.fixEnumValue(argument.PropertyType, ArgumentType);
            
            switch (argumentPropertyType) {
                case ArgumentType.PropertyInt:
                    customModel.push({ Name: argument.Name, Value: argument.ValueInt, UniqueId: repositoryId });
                    break;
                case ArgumentType.PropertyDate:
                    customModel.push({ Name: argument.Name, Value: argument.ValueDate, UniqueId: repositoryId });
                    break;
                case ArgumentType.PropertyDouble:
                    customModel.push({ Name: argument.Name, Value: argument.ValueDouble, UniqueId: repositoryId });
                    break;
                case ArgumentType.PropertyBoolean:
                    customModel.push({ Name: argument.Name, Value: argument.ValueBoolean, UniqueId: repositoryId });
                    break;
                case ArgumentType.PropertyGuid:
                    customModel.push({ Name: argument.Name, Value: argument.ValueGuid, UniqueId: repositoryId });
                    break;
                case ArgumentType.PropertyString:
                    customModel.push({ Name: argument.Name, Value: argument.ValueString, UniqueId: repositoryId });
                    break;
                case ArgumentType.Json:
                    customModel.push({ Name: argument.Name, Value: argument.ValueJson, UniqueId: repositoryId });
            }
        }
        return customModel;
    }

    private resolveDomainUsers(roleMappings: WorkflowRoleMappingModel[]): JQueryPromise<WorkflowRoleMappingModel[]> {
        let promises: JQueryDeferred<void>[] = new Array<JQueryDeferred<void>>();
        $.each(roleMappings, (index: number, mapping: WorkflowRoleMappingModel) => {
            let deferred: JQueryDeferred<void> = $.Deferred<void>();
            if (mapping.AuthorizationType.toString() != WorkflowAuthorizationType[WorkflowAuthorizationType.UserName]) return;
            this._domainUserService.getUser(mapping.AccountName,
                (response: any) => {
                    if (response == undefined) return deferred.reject();
                    let domainUser: DomainUserModel = <DomainUserModel>response;
                    mapping.AccountName = domainUser.DisplayName;
                    deferred.resolve();
                },
                (exception: ExceptionDTO) => {
                    deferred.reject(exception);
                });
            promises.push(deferred);
        });
        return $.when.apply(undefined, promises).promise(roleMappings);
    }

    /**
     * Imposta i mapping tag per i workflow di tipo xaml, gestendo le internal activity custom
     * @param model
     */
    private fillXamlMappings(model: WorkflowRepositoryModel, customActivities: ActivityModel[], selectedMappingText: string): JQueryPromise<void> {
        let result: JQueryDeferred<void> = $.Deferred<void>();
        if (selectedMappingText == '') return result.resolve(null);
        if (customActivities.length == 0) return result.resolve(null);

        $.when(this.resolveDomainUsers(model.WorkflowRoleMappings)).always(() => {
            let sourceModel: WorkflowActivityViewModel[] = new Array<WorkflowActivityViewModel>();
            try {
                $.each(customActivities, (index: number, activity: ActivityModel) => {
                    let filteredMappings: WorkflowRoleMappingModel[] = $.grep(model.WorkflowRoleMappings, (map: WorkflowRoleMappingModel, index: number) => {
                        return map.IdInternalActivity.toLowerCase() == activity.Id.toLowerCase() && map.MappingTag.toLowerCase() == selectedMappingText.toLowerCase()
                    });
                    let mapping: WorkflowRoleMappingModel;
                    if (filteredMappings.length > 0) {
                        mapping = filteredMappings[0];
                    } else {
                        mapping = <WorkflowRoleMappingModel>{};
                    }
                    sourceModel.push(<WorkflowActivityViewModel>{ Activity: activity, WorkflowRoleMapping: mapping, MappingTag: selectedMappingText });
                });
                let xamlWorkflowRoleMappingsMasterTableView: Telerik.Web.UI.GridTableView = this._rgvXamlWorkflowRoleMappings.get_masterTableView();
                xamlWorkflowRoleMappingsMasterTableView.set_dataSource(sourceModel);
                xamlWorkflowRoleMappingsMasterTableView.clearSelectedItems();
                xamlWorkflowRoleMappingsMasterTableView.dataBind();
                result.resolve();
            }
            catch (error) {
                this.showNotificationMessage(this.uscNotificationId, error.message);
                console.log(JSON.stringify(error));
                result.reject(error);
            }
        }).fail((exception: ExceptionDTO) => { this.showNotificationException(this.uscNotificationId, exception); });
        return result.promise();
    }

    /**
     * Carica i dettagli del workflow selezionato
     */
    loadDetails() {
        this._loadingPanel.show(this.pnlDetailsId);

        //Gestito JQueryPromise che simula la gestion async dei metodi (IE8 supportato)     
        $.when(this.fillDetailsPanel()).done((model) => {
            let activities: ActivityModel[] = !String.isNullOrEmpty(model.CustomActivities) ? JSON.parse(model.CustomActivities) : [];
            let selectedTag: string = (!String.isNullOrEmpty(this._rcbSelectMappingTag.get_text()) && this._rcbSelectMappingTag.get_text() != this._rcbSelectMappingTag.get_emptyMessage()) ? this._rcbSelectMappingTag.get_text() : '';
            this._currentWorkflowRepositoryModel = model;

            //- changing ProponenteDiAvio/DestinatarioDiAvio will invalidate ProponenteDiDefault/DestinatarioDiDefault
            //- when changing ProponenteDiAvio/DestinatarioDiAvio, after closing the edit window, this method is called
            //- final values are updated after fillDetailsPanel
            if (this.updateDefaultOnStartDependencyValue()) {
                this._loadingPanel.hide(this.pnlDetailsId);
                this.loadDetails();
            } else {
                $.when(this.fillMappingSelection(), this.fillJsonMappings(model), this.fillXamlMappings(model, activities, selectedTag)).always(() => {
                    this._loadingPanel.hide(this.pnlDetailsId);
                }).fail((exception: ExceptionDTO) => {
                    this._currentWorkflowRepositoryModel = null;
                    this.showNotificationException(this.uscNotificationId, exception);
                });
            }
        }).fail((exception) => {
            this._currentWorkflowRepositoryModel = null;
            this._loadingPanel.hide(this.pnlDetailsId);
            this.showNotificationException(this.uscNotificationId, exception, "Errore nel caricamento dell'attività.");
        });
    }

    /**
     * Esegue il mapping delle WorkflowAuthorizationType
     * @param authorizationType
     */
    getAuthorizationTypeDescription(authorizationType: string): string {
        switch (WorkflowAuthorizationType[authorizationType]) {
            case WorkflowAuthorizationType.ADGroup:
                return 'Gruppo AD';
            case WorkflowAuthorizationType.AllManager:
                return 'Tutti i responsabili';
            case WorkflowAuthorizationType.AllOChartHierarchyManager:
                return 'Tutti i responsabili di gerarchia in organigramma';
            case WorkflowAuthorizationType.AllOChartManager:
                return 'Tutti i responsabili configurati in organigramma';
            case WorkflowAuthorizationType.AllOChartRoleUser:
                return 'Tutti gli utenti configurati in organigramma';
            case WorkflowAuthorizationType.AllRoleUser:
                return 'Tutti gli utente di settore';
            case WorkflowAuthorizationType.AllSecretary:
                return 'Tutte le segreterie';
            case WorkflowAuthorizationType.AllSigner:
                return 'Tutti i firmatari';
            case WorkflowAuthorizationType.MappingTags:
                return 'Tags';
            case WorkflowAuthorizationType.UserName:
                return 'Nome utente';
            default:
                return '';
        }
    }

    getNameDescription(name: string): string {
        return WorkflowEvalutionPropertyHelper[name] ? WorkflowEvalutionPropertyHelper[name].Name : name;
    }

    /**
     * Esegue il mapping delle WorkflowStatus
     * @param status
     */
    getStatusDescription(status: string): string {
        switch (WorkflowRepositoryStatus[status]) {
            case WorkflowRepositoryStatus.Draft:
                return 'Bozza';
            case WorkflowRepositoryStatus.Published:
                return 'Pubblicato';
            default:
                return '';
        }
    }

    protected showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(uscNotificationId, customMessage)
        }

    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage)
        }
    }

    protected showWarningMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showWarningMessage(customMessage)
        }
    }

    private btnAdd_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.openWorkflowEvaluationPropertiesManagementWindow("Add");
    }

    private btnEdit_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.openWorkflowEvaluationPropertiesManagementWindow("Edit");
    }

    private btnDeleteStartup_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        let selectedEvaluationPropertyId = this._rgvWorkflowStartUp.get_selectedItems();
        if (selectedEvaluationPropertyId == undefined || selectedEvaluationPropertyId.length == 0) {
            this.showWarningMessage(this.uscNotificationId, 'Nessun proprietà selezionata');
            return;
        }

        if (selectedEvaluationPropertyId.length > 1) {
            this.showWarningMessage(this.uscNotificationId, 'Seleziona una sola proprietà per la modifica');
            return;
        }

        let workflowEvaluationModel: WorkflowEvaluationProperty;
        let workflowEvaluationModelUniqueId: string = (<WorkflowEvaluationProperty>selectedEvaluationPropertyId[0].get_dataItem()).UniqueId;

        workflowEvaluationModel = <WorkflowEvaluationProperty>{ UniqueId: workflowEvaluationModelUniqueId };

        this._workflowEvaluationPropertyService.deleteWorkflowEvaluationProperty(workflowEvaluationModel, (data) => {
            this.loadDetails();
        }, (exception: ExceptionDTO) => {
            console.log(exception);
        });
    }

    openWorkflowEvaluationPropertiesManagementWindow(operation: string) {
        this._windowAddWorkflowRoleMapping.setSize(600, 250);
        let selectedNode: Telerik.Web.UI.RadTreeNode = this._rtvWorkflowRepository.get_selectedNode();
        const _this = this;

        if (operation === "Add") {

            let openUrl = `TbltWorkflowEvaluationPropertyGes.aspx?Action=${operation}&WorkflowRepositoryId=${selectedNode.get_value()}&WorkflowEnv=${this.workflowEnvironment}`;
            openUrl = this.makeStartQueryParameters(openUrl);

            this._windowAddWorkflowRoleMapping.setUrl(openUrl);
        }
        else if (operation === "Edit") {

            let selectedEvaluationPropertyId = this._rgvWorkflowStartUp.get_selectedItems();

            if (selectedEvaluationPropertyId == undefined || selectedEvaluationPropertyId.length == 0) {
                this.showWarningMessage(this.uscNotificationId, 'Nessun proprietà selezionata');
                return;
            }

            if (selectedEvaluationPropertyId.length > 1) {
                this.showWarningMessage(this.uscNotificationId, 'Seleziona una sola proprietà per la modifica');
                return;
            }

            let workflowEvaluationModel = (<WorkflowEvaluationProperty>selectedEvaluationPropertyId[0].get_dataItem()).UniqueId;
            let openUrl = `TbltWorkflowEvaluationPropertyGes.aspx`;
            openUrl += `?${QueryParameters.QUERY_PARAM_ACTION}=${operation}`;
            openUrl += `&${QueryParameters.QUERY_PARAM_WORKFLOW_REPOSITORY_ID}=${selectedNode.get_value()}`;
            openUrl += `&${QueryParameters.QUERY_PARAM_WORKFLOW_EVALUATION_PROPERTY_ID}=${workflowEvaluationModel}`;
            openUrl += `&WorkflowEnv=${this.workflowEnvironment}`;

            openUrl = this.makeStartQueryParameters(openUrl);

            this._windowAddWorkflowRoleMapping.setUrl(openUrl);
        }

        this._windowAddWorkflowRoleMapping.set_modal(true);
        this._windowAddWorkflowRoleMapping.show();
    }

    /**
     * If the WorflowRepositoryModel has the properties _dsw_p_WorkflowStartProposer (ProponenteDiAvio) and 
     * _dsw_p_WorkflowStartRecipient (Destinatario Di Avio) it will create extra query parameters and add them to the query
     * @param url
     */
    private makeStartQueryParameters(url: string): string {
        //TODO: refactor below
        let startProposer: WorkflowEvaluationProperty = this._currentWorkflowRepositoryModel
            .WorkflowEvaluationProperties
            .filter(x => x.Name == TbltWorkflowRepository.PROPERTY_NAME_WORKFLOW_START_PROPOSER)[0];

        let startReceiver: WorkflowEvaluationProperty = this._currentWorkflowRepositoryModel
            .WorkflowEvaluationProperties
            .filter(x => x.Name == TbltWorkflowRepository.PROPERTY_NAME_WORKFLOW_START_RECIPIENT)[0];

        if (startProposer !== null && startProposer !== undefined) {
            url += `&${QueryParameters.QUERY_PARAM_START_PROPOSER}=${startProposer.ValueInt}`;
        }

        if (startReceiver !== null && startReceiver !== undefined) {
            url += `&${QueryParameters.QUERY_PARAM_START_RECEIVER}=${startReceiver.ValueInt}`;
        }

        return url;
    }


    /**
     * If the workflow repositort has the properties  _dsw_p_WorkflowStartProposer(ProponenteDiAvio) and _dsw_p_WorkflowDefaultProposer (ProponenteDiDefault)
     * then there is a strict dependency on their values. 
     *   If ProponenteDiAvio=0 the ProponenteDiDefault = Settore
     *   If ProponenteDiAvio=1 the ProponenteDiDefault = Account/Utente
     * Same logic applies for _dsw_p_WorkflowStartRecipient (DestinatarioDiAvio) and _dsw_p_WorkflowDefaultRecipient (DestinatarioDiDefault)
     **/
    private updateDefaultOnStartDependencyValue(): boolean {
        let mustReload: boolean = false;

        let defaultProposer: WorkflowEvaluationProperty = this._currentWorkflowRepositoryModel
            .WorkflowEvaluationProperties
            .filter(x => x.Name == TbltWorkflowRepository.PROPERTY_NAME_WORKFLOW_DEFAULT_PROPOSER)[0];

        let startProposer: WorkflowEvaluationProperty = this._currentWorkflowRepositoryModel
            .WorkflowEvaluationProperties
            .filter(x => x.Name == TbltWorkflowRepository.PROPERTY_NAME_WORKFLOW_START_PROPOSER)[0];

        if (startProposer !== null && startProposer !== undefined
            && defaultProposer !== null && defaultProposer !== undefined) {

            //if avio demands settore but contains Account. 
            //Note: do not check proponenteDiDefault.ValueString.indexOf("Role") < 0 because if fails if string is empty
            if (startProposer.ValueInt === 0 && defaultProposer.ValueString.indexOf("Account") > -1) {
                defaultProposer.ValueString = "";
                this._workflowEvaluationPropertyService.updateWorkflowEvaluationProperty(defaultProposer);
                mustReload = true;
                //this.loadDetails();
            }

            //if avio demands utente but contains Role. 
            //Note: do not check proponenteDiDefault.ValueString.indexOf("Account") < 0 because if fails if string is empty
            if (startProposer.ValueInt === 1 && defaultProposer.ValueString.indexOf("Role") > -1) {
                defaultProposer.ValueString = "";
                this._workflowEvaluationPropertyService.updateWorkflowEvaluationProperty(defaultProposer);
                mustReload = true;
                //this.loadDetails();
            }
        }

        let defaultReceiver: WorkflowEvaluationProperty = this._currentWorkflowRepositoryModel
            .WorkflowEvaluationProperties
            .filter(x => x.Name === TbltWorkflowRepository.PROPERTY_NAME_WORKFLOW_DEFAULT_RECIPIENT)[0];

        let startReceiver: WorkflowEvaluationProperty = this._currentWorkflowRepositoryModel
            .WorkflowEvaluationProperties
            .filter(x => x.Name === TbltWorkflowRepository.PROPERTY_NAME_WORKFLOW_START_RECIPIENT)[0];

        if (startReceiver !== null && startReceiver !== undefined
            && defaultReceiver !== null && defaultReceiver !== undefined) {

            //if avio demands settore but contains Account. 
            //Note: do not check proponenteDiDefault.ValueString.indexOf("Role") < 0 because if fails if string is empty
            if (startReceiver.ValueInt === 0 && defaultReceiver.ValueString.indexOf("Account") > -1) {
                defaultReceiver.ValueString = "";
                this._workflowEvaluationPropertyService.updateWorkflowEvaluationProperty(defaultReceiver);
                mustReload = true;
                //this.loadDetails();
            }

            //if avio demands utente but contains Role. 
            //Note: do not check proponenteDiDefault.ValueString.indexOf("Account") < 0 because if fails if string is empty
            if (startReceiver.ValueInt === 1 && defaultReceiver.ValueString.indexOf("Role") > -1) {
                defaultReceiver.ValueString = "";
                this._workflowEvaluationPropertyService.updateWorkflowEvaluationProperty(defaultReceiver);
                mustReload = true;
                this.loadDetails();
            }
        }

        return mustReload;
    }
}

export = TbltWorkflowRepository;

