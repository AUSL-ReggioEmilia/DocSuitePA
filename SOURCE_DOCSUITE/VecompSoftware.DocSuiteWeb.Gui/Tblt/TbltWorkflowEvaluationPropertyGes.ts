import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import WorkflowEvaluationProperty = require('App/Models/Workflows/WorkflowEvaluationProperty');
import WorkflowType = require('App/Models/Workflows/WorkflowType');
import WorkflowRepositoryModel = require('App/Models/Workflows/WorkflowRepositoryModel');
import ArgumentType = require('App/Models/Workflows/ArgumentType');
import RoleModel = require('App/Models/Commons/RoleModel');
import UscRoleRestEventType = require('App/Models/Commons/UscRoleRestEventType');
import RoleSearchFilterDTO = require('App/DTOs/RoleSearchFilterDTO');
import ContactModel = require('App/Models/Commons/ContactModel');

import WorkflowEvaluationPropertyService = require('App/Services/Workflows/WorkflowEvaluationPropertyService');
import RolesService = require('App/Services/Commons/RoleService');

import WorkflowEvalutionPropertyHelper = require('App/Models/Workflows/WorkflowEvalutionPropertyHelper');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import EnumHelper = require("App/Helpers/EnumHelper");
import PropertyJsonValueSettori = require('App/Models/Workflows/JsonModels/PropertyJsonValueSettori');
import PropertyJsonValueContact = require('App/Models/Workflows/JsonModels/PropertyJsonValueContact');
import PageClassHelper = require('App/Helpers/PageClassHelper');
import QueryParameters = require('App/Models/Workflows/QueryStringModels/QueryParametersWorkflowEvaluationProperty')

import uscRoleRest = require('UserControl/uscRoleRest');
import uscDomainUserSelRest = require('UserControl/uscDomainUserSelRest');
import WorkflowPropertyHelper = require('App/Models/Workflows/WorkflowPropertyHelper');
import WorkflowRoleModel = require('App/Models/Workflows/WorkflowRoleModel');
import WorkflowAccountModel = require('App/Models/Workflows/WorkflowAccountModel');
import TemplateCollaborationService = require('App/Services/Templates/TemplateCollaborationService');
import UIHandlerEvalPropertyTemplateCollaboration = require('App/Services/Handlers/UIHandlerEvalPropertyTemplateCollaboration');
import UIHandlerEvalPropertyTemplateDeposito = require('App/Services/Handlers/UIHandlerEvalPropertyTemplateDeposito');
import WorkflowAuthorizationType = require('App/Models/Workflows/WorkflowAuthorizationType');
import uscWorkflowDesignerValidations = require('UserControl/uscWorkflowDesignerValidations');
import DSWEnvironmentType = require('App/Models/Workflows/WorkflowDSWEnvironmentType');
import WorkflowRuleDefinitionModel = require('App/Models/Workflows/WorkflowRuleDefinitionModel');
import UscTemplateCollaborationSelRest = require('UserControl/uscTemplateCollaborationSelRest');

class TbltWorkflowEvaluationPropertyGes {
    rcbNameId: string;
    rntbValueIntId: string;
    rtbValueStringId: string;
    rtbValueJsonId: string;
    rlbValueBoolId: string;
    valueBoolId: string;
    btnConfirmId: string;
    rdpValueDateId: string;
    rntbValueDoubleId: string;
    rdbGuidId: string;
    workflowEnv: string;

    //roles - View Rendering
    uscRoleRestId: string;
    uscRoleRestContainerId: string;

    //domain users - View Rendering
    uscDomainUserSelRestId: string;
    uscDomainUserSelRestContainerId: string;

    //validationRules - View Rendering
    uscWorkflowDesignerValidationsId: string;
    uscWorkflowDesignerValidationsContainerId: string;

    //template di collaborazione - View Rendering
    uscTemplateCollaborationSelRestId: string;
    uscTemplateCollaborationContainerId: string;

    //template collaboration sign summary
    ddlCollaborationSignSummaryId: string;
    uscCollaborationSignSummaryContainerId: string;

    //template generate id
    ddlActionGenerateId: string;
    uscActionGenerateContainerId: string;

    private _rcbName: Telerik.Web.UI.RadComboBox;
    private _rntbValueInt: Telerik.Web.UI.RadNumericTextBox;
    private _rtbValueString: Telerik.Web.UI.RadTextBox;
    private _rtbValueJson: Telerik.Web.UI.RadTextBox;
    private _rlbValueBool: Telerik.Web.UI.RadListBox;
    private _rdpValueDate: Telerik.Web.UI.RadDatePicker;
    private _rntbValueDouble: Telerik.Web.UI.RadNumericTextBox;
    private _rdbGuid: Telerik.Web.UI.RadTextBox;
    private _ddlTemplateCollaborationSignSummary: Telerik.Web.UI.RadComboBox;
    private _ddlTemplateGenerate: Telerik.Web.UI.RadComboBox;;

    private btnConfirm: Telerik.Web.UI.RadButton;

    private _serviceConfigurations: ServiceConfiguration[];

    private _workflowEvaluationPropertyService: WorkflowEvaluationPropertyService;
    private _rolesService: RolesService;
    private _templateCollaborationService: TemplateCollaborationService;
    private _templateCollabUIHandler: UIHandlerEvalPropertyTemplateCollaboration;
    private _collaborationSignSummaryUIHandler: UIHandlerEvalPropertyTemplateDeposito;
    private _actionGenerateUIHandler: UIHandlerEvalPropertyTemplateDeposito;

    private _enumHelper: EnumHelper;

    private _uscRoleRestContainer: JQuery;
    private _uscContattiSelContainer: JQuery;
    private _uscWorkflowDesignerValidationsContainer: JQuery;
    private _uscTemplateCollaborationContainer: JQuery;
    private _uscCollaborationSignSummaryContainer: JQuery;
    private _uscActionGenerateContainer: JQuery;

    private _uscWorkflowDesignerValidations: uscWorkflowDesignerValidations;

    private queryParameters: QueryParameters;
    private static CONFIGURATION_ROLE = "Role";
    private static CONFIGURATION_EVALUATION_PROPERTY = "WorkflowEvaluationProperty";

    // _dsw_p_WorkflowDefaultProposer
    private static PROPERTY_NAME_WORKFLOW_DEFAULT_PROPOSER = WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_DEFAULT;
    // _dsw_p_WorkflowDefaultRecipient
    private static PROPERTY_NAME_WORKFLOW_DEFAULT_RECIPIENT = WorkflowPropertyHelper.DSW_PROPERTY_RECIPIENT_DEFAULT;
    // _dsw_p_WorkflowDefaultTemplateCollaboration
    private static PROPERTY_NAME_TEMPLATE_COLLABORATION_DEFAULT = WorkflowPropertyHelper.DSW_PROPERTY_TEMPLATE_COLLABORATION_DEFAULT;
    // _dsw_p_CollaborationSignSummaryTemplateId
    private static PROPERTY_NAME_COLLABORATION_SIGN_SUMMARY = WorkflowPropertyHelper.DSW_PROPERTY_COLLABORATION_SIGN_SUMMARY_TEMPLATE_ID;
    // _dsw_a_Generate_TemplateId
    private static PROPERTY_NAME_ACTION_GENERATE = WorkflowPropertyHelper.DSW_ACTION_GENERATE_TEMPLATE_ID;

    private static PROPERTY_NAME_DSW_WORKFLOW_STARTVALIDATIONS: string = WorkflowPropertyHelper.DSW_VALIDATION_WORKFLOW_START;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();

        let templateServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "TemplateCollaboration");
        this._templateCollaborationService = new TemplateCollaborationService(templateServiceConfiguration);
    }

    initialize(): void {
        this.queryParameters = new QueryParameters();

        const serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltWorkflowEvaluationPropertyGes.CONFIGURATION_EVALUATION_PROPERTY);
        this._workflowEvaluationPropertyService = new WorkflowEvaluationPropertyService(serviceConfiguration);

        const rolesServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltWorkflowEvaluationPropertyGes.CONFIGURATION_ROLE);
        this._rolesService = new RolesService(rolesServiceConfiguration);

        this.btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this.btnConfirm.add_clicked(this.btnConfirm_onClick);

        this._rcbName = <Telerik.Web.UI.RadComboBox>$find(this.rcbNameId);
        this._rcbName.add_selectedIndexChanged(this.rcbName_onSelectedIndexChanged);
        this._rcbName.add_itemsRequested(this.rcbName_OnClientItemsRequested);

        this._rntbValueInt = <Telerik.Web.UI.RadNumericTextBox>$find(this.rntbValueIntId);
        this._rtbValueString = <Telerik.Web.UI.RadTextBox>$find(this.rtbValueStringId);
        this._rtbValueJson = <Telerik.Web.UI.RadTextBox>$find(this.rtbValueJsonId);
        this._rlbValueBool = <Telerik.Web.UI.RadListBox>$find(this.rlbValueBoolId);
        this._rdpValueDate = <Telerik.Web.UI.RadDatePicker>$find(this.rdpValueDateId);
        this._rntbValueDouble = <Telerik.Web.UI.RadNumericTextBox>$find(this.rntbValueDoubleId);
        this._rdbGuid = <Telerik.Web.UI.RadTextBox>$find(this.rdbGuidId);

        // handler for uscTemplatecollaborationSelRest , that will load templates and manages updateing the underlying rtbValue storage
        this._templateCollabUIHandler = new UIHandlerEvalPropertyTemplateCollaboration(this._rtbValueString, this.uscTemplateCollaborationSelRestId);

        // combobox that shows a list of deposito documentale templates. 
        this._ddlTemplateCollaborationSignSummary = <Telerik.Web.UI.RadComboBox>$find(this.ddlCollaborationSignSummaryId);
        // handler for _ddlTemplateCollaborationSignSummary 
        this._collaborationSignSummaryUIHandler = new UIHandlerEvalPropertyTemplateDeposito(this._rtbValueString, this._ddlTemplateCollaborationSignSummary, this._serviceConfigurations);

        // combobox that shows a list of deposito documentale templates. 
        this._ddlTemplateGenerate = <Telerik.Web.UI.RadComboBox>$find(this.ddlActionGenerateId);
        // handler for _ddlTemplateGenerate 
        this._actionGenerateUIHandler = new UIHandlerEvalPropertyTemplateDeposito(this._rtbValueString, this._ddlTemplateGenerate, this._serviceConfigurations);

        this._uscRoleRestContainer = $(`#${this.uscRoleRestContainerId}`);
        this._uscContattiSelContainer = $(`#${this.uscDomainUserSelRestContainerId}`);
        this._uscTemplateCollaborationContainer = $(`#${this.uscTemplateCollaborationContainerId}`);
        this._uscCollaborationSignSummaryContainer= $(`#${this.uscCollaborationSignSummaryContainerId}`);
        this._uscActionGenerateContainer = $(`#${this.uscActionGenerateContainerId}`);
        this._uscWorkflowDesignerValidationsContainer = $(`#${this.uscWorkflowDesignerValidationsContainerId}`);
        this._uscWorkflowDesignerValidations = <uscWorkflowDesignerValidations>$(`#${this.uscWorkflowDesignerValidationsId}`).data();

        this.populateComboNames();
        this.resetValueVisibility();

        this.getQueryParameters(window.location.search);
        this.initializeEditOperation();

    }

    rcbName_OnClientItemsRequested = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxRequestEventArgs) => {
        try {
            let filteringList = [];
            $.each(WorkflowEvalutionPropertyHelper, (key, value) => {
                if (value.Name != undefined || value.Type != undefined) {

                    filteringList.push({ Key: key, Name: `${value.Name} (${key})`, Type: value.Type });
                }
            });
            let filteredList = filteringList.filter(x => x.Name.toLowerCase().indexOf(sender.get_text().toLowerCase()) !== -1);
            this.refreshNames(filteredList);
        }
        catch (error) {
        }
    }

    refreshNames = (data: any) => {
        if (data.length > 0) {
            this._rcbName.beginUpdate();
            $.each(data, (key, value) => {
                let item: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(value.Name);
                item.set_value(value.Key);
                this._rcbName.get_items().add(item);
            });
            this._rcbName.showDropDown();
            this._rcbName.endUpdate();
        }
    }

    private initializeEditOperation() {

        if (this.queryParameters.Action === "Edit") {
            this._workflowEvaluationPropertyService.getWorkflowEvaluationProperty(this.queryParameters.WorkflowEvaluationPropertyId, (data: WorkflowEvaluationProperty) => {
                let valueName = this._rcbName.findItemByValue(data.Name);
                valueName.select();
                this._rcbName.disable();
                this.getFirstNonNullValue(data);
            }, (exception: ExceptionDTO) => {
                console.log(exception);
            });
        }
    }

    private getFirstNonNullValue(model: WorkflowEvaluationProperty) {

        /**
         * Specific Properties
         */
        //specific property: DEFAULT PROPOSER
        if (model.Name === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_WORKFLOW_DEFAULT_PROPOSER) {

            if (this.queryParameters.StartProposer === 0) {
                this.WorkflowRestRoleRenderProperty(model);
            } else {
                //this.queryParameters.ProponenteDiAvio === 1
                this.WorkflowRestContactsRenderProperty(model);
            }

            //setting value in the json field to be used in model validation. Field is not visible
            this._rtbValueJson.set_value(model.ValueString);
            return;
        }

        //specific property: DEFAULT RECIPIENT
        if (model.Name === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_WORKFLOW_DEFAULT_RECIPIENT) {

            if (this.queryParameters.StartReceiver === 0) {
                this.WorkflowRestRoleRenderProperty(model);
            } else {
                //this.queryParameters.DestinatarioDiAvio === 1
                this.WorkflowRestContactsRenderProperty(model);
            }

            //setting value in the json field to be used in model validation. Field is not visible
            this._rtbValueJson.set_value(model.ValueString);
            return;
        }

        if (model.Name === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_DSW_WORKFLOW_STARTVALIDATIONS) {
            //setting value in the json field to be used in model validation. Field is not visible
            PageClassHelper.callUserControlFunctionSafe<uscWorkflowDesignerValidations>(this.uscWorkflowDesignerValidationsId).done((instance) => {
                this._rtbValueJson.set_textBoxValue(instance.getWorkflowRulesModel(this.workflowEnv));
                instance.createValidationTree((<WorkflowRuleDefinitionModel>(JSON.parse(model.ValueString))[this.workflowEnv]).Rules);
            });
            this._rtbValueJson.set_value(model.ValueString);
            return;
        }

        if (model.Name === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_TEMPLATE_COLLABORATION_DEFAULT) {
            this._templateCollabUIHandler.InitializeTemplateTreeviewControl(false);
            this._templateCollabUIHandler.SetSelectedItem(model.ValueGuid);
            this._templateCollabUIHandler.UpdateSelection();
            this._templateCollabUIHandler.InitializeDisableButtonEvent(this.btnConfirm);
            this._rtbValueString.set_value(model.ValueGuid);
            return;
        }

        if (model.Name === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_COLLABORATION_SIGN_SUMMARY) {
            this._collaborationSignSummaryUIHandler.SetSelectedItem(model.ValueString);
            this._collaborationSignSummaryUIHandler.UpdateSelection();
            this._rtbValueString.set_value(model.ValueString);
            return;
        }

        if (model.Name === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_ACTION_GENERATE) {
            this._actionGenerateUIHandler.SetSelectedItem(model.ValueGuid);
            this._actionGenerateUIHandler.UpdateSelection();
            this._rtbValueString.set_value(model.ValueGuid);
            return;
        }

        /**
         * general properties
         */
        if (model.ValueBoolean != undefined || model.ValueBoolean != null) {
            let item = this._rlbValueBool.getItem(Number(model.ValueBoolean));
            item.select();
        }
        else if (model.ValueDate != undefined || model.ValueDate != null) {
            this._rdpValueDate.set_selectedDate(moment(model.ValueDate).isValid() ? new Date(model.ValueDate) : null);
        }
        else if (model.ValueDouble != null) {
            this._rntbValueDouble.set_textBoxValue(model.ValueDouble.toString());
        }
        else if (model.ValueGuid != null) {
            this._rdbGuid.set_value(model.ValueGuid);
        }
        else if (model.ValueInt != null) {
            this._rntbValueInt.set_value(model.ValueInt.toString());
        }
        else if (model.ValueString != null) {
            this._rtbValueString.set_value(model.ValueString);
            this._rtbValueJson.set_value(model.ValueString);
        }
    }

    private ensureUscRoleRestEvents(instance: uscRoleRest): void {

        instance.registerEventHandler(UscRoleRestEventType.RoleDeleted, (roleId: number) => {

            this._rtbValueJson.set_value("");
            this.WorkflowRestRoleRenderRoles([]);

            //solving manually
            return $.Deferred<void>().reject();
        });

        instance.registerEventHandler(UscRoleRestEventType.NewRolesAdded, (newAddedRoles: RoleModel[]) => {
            let existedRole: RoleModel;

            const json = new PropertyJsonValueSettori();

            json.AuthorizationType = WorkflowAuthorizationType.AllRoleUser;

            json.Role = <WorkflowRoleModel>{
                IdRole: newAddedRoles[newAddedRoles.length - 1].EntityShortId,
                Name: newAddedRoles[newAddedRoles.length - 1].Name,
                IdTenantAOO: newAddedRoles[newAddedRoles.length - 1].IdTenantAOO,
                UniqueId: newAddedRoles[newAddedRoles.length - 1].UniqueId
            }

            this._rtbValueJson.set_value(JSON.stringify(json));

            return $.Deferred<RoleModel>().resolve(existedRole);
        });
    }

    /**
     * The method will always ensure that the events are registered. 
     * The initial logic for using this method implied calling it from the constructor, but it does not always work
     * because sometimes code reaches using the instance faster then the rest component is loaded
     * @param usc
     */
    private ensureUscContattiSelRestEvents(usc: uscDomainUserSelRest): void {

        usc.registerEventHandlerContactsDeleted((data: ContactModel[]) => {

            this._rtbValueJson.set_value("");

            //manually setting the rendering for an empty array. Rejecting default action of usc rest component
            this.WorkflowRestContactsRenderContacts([]);

            //we rendered manually. Prevent rest component to render:
            return $.Deferred<void>().reject();
        });

        usc.registerEventHandlerContactsAdded((data: ContactModel[]) => {

            const json = new PropertyJsonValueContact();

            if (data.length === 0) {

                this._rtbValueJson.set_value("");
                this.WorkflowRestContactsRenderContacts([]);

            } else {

                //the rest control returns only one element(in an array...)
                let newAddedContact = data[0];

                json.AuthorizationType = WorkflowAuthorizationType.UserName;

                //see uscStartWorkflow.ts/startWorkflow
                json.Account = <WorkflowAccountModel>{
                    AccountName: newAddedContact.Code,
                    DisplayName: newAddedContact.Description,
                    Required: true,
                    EmailAddress: newAddedContact.EmailAddress
                }

                this._rtbValueJson.set_value(JSON.stringify(json));

                //manually rendering the saved json in the rest component. We will reject the default action
                this.WorkflowRestContactsRenderJsonModel(json);
            }
            //we rendered manually. Prevent rest component to render:
            return $.Deferred<void>().reject();
        });
    }

    //#region WorkflowRestRole

    private WorkflowRestRoleRenderRoles(model: RoleModel[]): void {
        PageClassHelper.callUserControlFunctionSafe<uscRoleRest>(this.uscRoleRestId)
            .done((instance) => {
                this.ensureUscRoleRestEvents(instance);
                instance.renderRolesTree(model);
            });
    }

    private WorkflowRestRoleRenderProperty(model: WorkflowEvaluationProperty): void {
        if (model.ValueString === "" || model.ValueString === undefined || model.ValueString === null) {
            this.WorkflowRestRoleRenderRoles([]);

        } else {

            let proposerModel: PropertyJsonValueSettori = <PropertyJsonValueSettori>JSON.parse(model.ValueString)
            this.WorkflowRestRoleRenderJsonModel(proposerModel);
        }
    }

    private WorkflowRestRoleRenderJsonModel(model: PropertyJsonValueSettori): void {
        this._rolesService.findRoles(
            <RoleSearchFilterDTO>{
                LoadAlsoParent: true,
                UniqueId: model.Role.UniqueId
            },
            (data: RoleModel[]) => {
                if (data === null || data === undefined) {
                    this.WorkflowRestRoleRenderRoles([]);
                } else {
                    this.WorkflowRestRoleRenderRoles(data);
                }
            },
            (exception: ExceptionDTO) => {
                console.log(exception);
            });
    }

    //#endregion

    //#region WorkflowRestContacts

    private WorkflowRestContactsRenderContacts(model: ContactModel[]): void {
        PageClassHelper.callUserControlFunctionSafe<uscDomainUserSelRest>(this.uscDomainUserSelRestId)
            .done((instance) => {
                this.ensureUscContattiSelRestEvents(instance);
                instance.createDomainUsersContactsTree(model);
            });
    }

    private WorkflowRestContactsRenderProperty(model: WorkflowEvaluationProperty): void {
        if (model.ValueString === "" || model.ValueString === undefined || model.ValueString === null) {

            this.WorkflowRestContactsRenderContacts([]);

        } else {

            const jsonModel: PropertyJsonValueContact = <PropertyJsonValueContact>JSON.parse(model.ValueString);
            this.WorkflowRestContactsRenderJsonModel(jsonModel);
        }
    }

    private WorkflowRestContactsRenderJsonModel(model: PropertyJsonValueContact): void {
        this.WorkflowRestContactsRenderContacts([
            <ContactModel>{
                EmailAddress: model.Account.EmailAddress,
                Code: model.Account.AccountName,
                Description: (model.Account.DisplayName !== null
                    && model.Account.DisplayName !== undefined
                    && model.Account.DisplayName !== "|")
                    ? model.Account.DisplayName
                    : model.Account.EmailAddress
            }
        ]);
    }

    //#endregion

    private populateComboNames() {
        let cmbItem: Telerik.Web.UI.RadComboBoxItem = null;
        const obj = this;
        const workflowEvaluationPropertyHelper = [];
        $.each(WorkflowEvalutionPropertyHelper, function (index, item) {
            if (item.Name != undefined || item.Type != undefined) {
                workflowEvaluationPropertyHelper.push({ Value: item, Key: index });
            }
        });
        workflowEvaluationPropertyHelper.sort(function (a, b) {
            return (a.Value.Name > b.Value.Name) ? 1 : (a.Value.Name < b.Value.Name) ? -1 : 0;
        });
        $.each(workflowEvaluationPropertyHelper, function (index, item) {
            if (item.Value.Name != undefined || item.Value.Type != undefined) {
                cmbItem = new Telerik.Web.UI.RadComboBoxItem();
                cmbItem.set_text(`${item.Value.Name} (${item.Key})`);
                cmbItem.set_value(item.Key);
                obj._rcbName.get_items().add(cmbItem);
            }
        });
    }

    private resetValueVisibility() {
        this._rntbValueInt.set_visible(false);
        this._rtbValueString.set_visible(false);
        this._rtbValueJson.set_visible(false);
        $("#valueBool").hide();
        $("#valueDate").hide();
        this._rntbValueDouble.set_visible(false);
        this._rdbGuid.set_visible(false);
        this._uscRoleRestContainer.hide();
        this._uscWorkflowDesignerValidationsContainer.hide();
        this._uscContattiSelContainer.hide();
        this._uscTemplateCollaborationContainer.hide();
        this._uscCollaborationSignSummaryContainer.hide();
        this._uscActionGenerateContainer.hide();
    }

    private rcbName_onSelectedIndexChanged = (sender: any, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        this.resetValueVisibility();
        let vals = args.get_item().get_value();
        this.dynamicallyAdjustInputFields(vals);

        this.btnConfirm.set_enabled(true);
    }

    private dynamicallyAdjustInputFields(propertyFieldName: string): void {

        /*
         * specific properties
         */

        if (propertyFieldName === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_WORKFLOW_DEFAULT_PROPOSER
            && this.queryParameters.StartProposer === 0) {
            // The main storage remains the rbtValueJson which stores the serialized data
            // The rest component will replace the default view by taking the data and rendering it
            this.WorkflowRestRoleRenderRoles([])
            this._rtbValueJson.clear();
            this._uscRoleRestContainer.show();
            return;
        }

        // specific properties
        if (propertyFieldName === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_WORKFLOW_DEFAULT_PROPOSER
            && this.queryParameters.StartProposer === 1) {
            // The main storage remains the rbtValueJson which stores the serialized data
            // The rest component will replace the default view by taking the data and rendering it
            this.WorkflowRestContactsRenderContacts([]);
            this._rtbValueJson.clear();
            this._uscContattiSelContainer.show();
            return;
        }

        if (propertyFieldName === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_WORKFLOW_DEFAULT_RECIPIENT
            && this.queryParameters.StartReceiver === 0) {
            // The main storage remains the rbtValueJson which stores the serialized data
            // The rest component will replace the default view by taking the data and rendering it
            this.WorkflowRestRoleRenderRoles([])
            this._rtbValueJson.clear();
            this._uscRoleRestContainer.show();
            return;
        }

        if (propertyFieldName === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_WORKFLOW_DEFAULT_RECIPIENT
            && this.queryParameters.StartReceiver === 1) {
            // The main storage remains the rbtValueJson which stores the serialized data
            // The rest component will replace the default view by taking the data and rendering it
            this.WorkflowRestContactsRenderContacts([]);
            this._rtbValueJson.clear();
            this._uscContattiSelContainer.show();
            return;
        }

        if (propertyFieldName === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_TEMPLATE_COLLABORATION_DEFAULT) {
            // The main storage remains the rtbValueString which stores the serialized data
            // The rest component will replace the default view by taking the data and rendering it
            this._templateCollabUIHandler.SetSelectedItem(null);
            this._rtbValueString.clear();
            this._uscTemplateCollaborationContainer.show();

            if (this.queryParameters.Action === "Add") {
                this._templateCollabUIHandler.InitializeTemplateTreeviewControl();
                this._templateCollabUIHandler.InitializeDisableButtonEvent(this.btnConfirm);
            }
            return;
        }

        if (propertyFieldName === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_COLLABORATION_SIGN_SUMMARY) {
            // The main storage remains the rtbValueString which stores the serialized data
            // The rest component will replace the default view by taking the data and rendering it
            this._collaborationSignSummaryUIHandler.SetSelectedItem(null);
            this._rtbValueString.clear();
            this._uscCollaborationSignSummaryContainer.show();
            this._collaborationSignSummaryUIHandler.LoadTemplateDocumenti();
            return;
        }

        if (propertyFieldName === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_ACTION_GENERATE) {
            // The main storage remains the rtbValueString which stores the serialized data
            // The rest component will replace the default view by taking the data and rendering it
            this._actionGenerateUIHandler.SetSelectedItem(null);
            this._rtbValueString.clear();
            this._uscActionGenerateContainer.show();
            this._actionGenerateUIHandler.LoadTemplateDocumenti();
            return;
        }

        if (propertyFieldName === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_DSW_WORKFLOW_STARTVALIDATIONS) {
            this._rtbValueJson.clear();
            this._uscWorkflowDesignerValidationsContainer.show();

            if (this.workflowEnv != DSWEnvironmentType[DSWEnvironmentType.Fascicle]) {
                PageClassHelper.callUserControlFunctionSafe<uscWorkflowDesignerValidations>(this.uscWorkflowDesignerValidationsId).done((instance) => {
                    instance.displayDisableEnvironmentMessage();
                });
            }

            return;

        }

        /**
         * general properties
         **/
        const prop = WorkflowEvalutionPropertyHelper[propertyFieldName];
        if (prop) {
            switch (prop.Type) {
                case ArgumentType.PropertyString: {
                    this._rtbValueString.set_visible(true);
                    this._rtbValueString.clear();
                    break;
                }
                case ArgumentType.PropertyBoolean: {
                    $("#valueBool").show();
                    this._rlbValueBool.clearSelection();
                    break;
                }
                case ArgumentType.PropertyInt: {
                    this._rntbValueInt.set_visible(true);
                    this._rntbValueInt.clear();
                    break;
                }
                case ArgumentType.PropertyDate: {
                    $("#valueDate").show();
                    this._rdpValueDate.clear();
                    break;
                }
                case ArgumentType.PropertyDouble: {
                    this._rntbValueDouble.set_visible(true);
                    this._rntbValueDouble.clear();
                    break;
                }
                case ArgumentType.PropertyGuid: {
                    this._rdbGuid.set_visible(true);
                    this._rdbGuid.clear();
                    break;
                }
                case ArgumentType.Json: {
                    this._rtbValueJson.set_visible(true);
                    this._rtbValueJson.clear();
                    break;
                }
            }
        }

        this._rcbName.hideDropDown();
    }

    private btnConfirm_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this._templateCollabUIHandler.Commit();
        this._collaborationSignSummaryUIHandler.Commit();
        this._actionGenerateUIHandler.Commit();

        if (this._rcbName.get_selectedItem().get_value() === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_DSW_WORKFLOW_STARTVALIDATIONS) {
            PageClassHelper.callUserControlFunctionSafe<uscWorkflowDesignerValidations>(this.uscWorkflowDesignerValidationsId).done((instance) => {
                this._rtbValueJson.set_textBoxValue(instance.getWorkflowRulesModel(this.workflowEnv));
            });
        }

        if (this.queryParameters.Action === "Add") {
            this.AddWorkflowEvaluationProperty(this.queryParameters.WorkflowRepositoryId);
        } else if (this.queryParameters.Action === "Edit") {
            this.EditWorkflowEvaluationProperty(this.queryParameters.WorkflowRepositoryId, this.queryParameters.WorkflowEvaluationPropertyId);
        }

        return false;
    }

    private AddWorkflowEvaluationProperty(workflowRepositoryId: string): void {

        let validModel = this.validateModel(workflowRepositoryId);
        this.insertWorkflowEvaluationProperty(validModel);
    }

    private EditWorkflowEvaluationProperty(workflowRepositoryId: string, workflowEvaluationPropertyId: string): void {
        let validModel: WorkflowEvaluationProperty = this.validateModel(workflowRepositoryId);
        validModel.UniqueId = workflowEvaluationPropertyId;
        this._workflowEvaluationPropertyService.updateWorkflowEvaluationProperty(validModel, (data: any) => {
            let operator = this.getRadWindow();
            this.closeWindow(operator);
        }, (exception: ExceptionDTO) => {
            console.log(exception);
        });
    }

    private validateModel(workflowRepositoryId: string): WorkflowEvaluationProperty {
        let name = this._rcbName.get_selectedItem().get_value();
        let model: WorkflowEvaluationProperty = {
            UniqueId: "",
            Name: name,
            PropertyType: this._enumHelper.getWorkflowStartupDescription(WorkflowEvalutionPropertyHelper[name].Type),
            WorkflowType: WorkflowType.Workflow,
            ValueInt: null,
            ValueString: "",
            ValueBoolean: null,
            ValueDate: null,
            ValueDouble: null,
            ValueGuid: null,
            WorkflowRepository: {} as WorkflowRepositoryModel
        };

        let validModel = this.checkModelValidation(model);
        validModel.WorkflowRepository.UniqueId = workflowRepositoryId;
        return model;
    }

    private insertWorkflowEvaluationProperty(model: WorkflowEvaluationProperty): void {
        this._workflowEvaluationPropertyService.insertWorkflowEvaluationProperty(model,
            (data: any) => {
                let operator = this.getRadWindow();
                this.closeWindow(operator);
            },
            (exception: ExceptionDTO) => {
                console.log(exception);
            });
    }

    private checkModelValidation(model: WorkflowEvaluationProperty): WorkflowEvaluationProperty {

        /*
         *Specific linked containers
         */
        if (this._uscRoleRestContainer.is(":visible")) {
            const valueString = this._rtbValueJson.get_textBoxValue();
            if (valueString === "") {
                alert("Inserisci un valore");
                return;
            }
            model.ValueString = valueString;
            return model;
        }

        if (this._uscWorkflowDesignerValidationsContainer.is(":visible")) {
            const valueString = this._rtbValueJson.get_textBoxValue();
            if (valueString === "") {
                alert("Inserisci un valore");
                return;
            }
            model.ValueString = valueString;
            return model;
        }

        if (this._uscContattiSelContainer.is(":visible")) {
            const valueString = this._rtbValueJson.get_textBoxValue();
            if (valueString === "") {
                alert("Inserisci un valore");
                return;
            }
            model.ValueString = valueString;
            return model;
        }

        if (this._uscTemplateCollaborationContainer.is(":visible")) {
            const valueGuid = this._rtbValueString.get_textBoxValue();

            if (valueGuid === "") {
                alert("Inserisci un valore");
                return;
            }
            model.ValueGuid = valueGuid;
            return model;
        }

        if (this._uscCollaborationSignSummaryContainer.is(":visible")) {
            const valueString = this._rtbValueString.get_textBoxValue();

            if (valueString === "") {
                alert("Inserisci un valore");
                return;
            }
            model.ValueString = valueString;
            return model;
        }

        if (this._uscActionGenerateContainer.is(":visible")) {

            const valueGuid = this._rtbValueString.get_textBoxValue();

            if (valueGuid === "") {
                alert("Inserisci un valore");
                return;
            }
            model.ValueGuid = valueGuid;
            return model;
        }

        //general containers
        if (this._rtbValueString.get_visible()) {
            const valueString = this._rtbValueString.get_textBoxValue();
            if (valueString === "") {
                alert("Inserisci un valore");
                return;
            }
            model.ValueString = valueString;
        }
        else if ($("#valueBool").is(":visible")) {
            const valueBool = this._rlbValueBool.get_selectedItem();
            if (valueBool === undefined || valueBool == null) {
                alert("Inserisci un valore");
                return;
            }
            model.ValueBoolean = valueBool.get_value() === "1";
        }
        else if (this._rntbValueInt.get_visible()) {
            const valueInt = this._rntbValueInt.get_value();
            if (valueInt === "") {
                alert("Inserisci un valore");
                return;
            }

            model.ValueInt = Number(valueInt);
        }
        else if (this._rtbValueJson.get_visible()) {
            const valueJson = this._rtbValueJson.get_textBoxValue();
            if (valueJson === "") {
                alert("Inserisci un valore");
                return;
            }
            model.ValueString = valueJson;
        }
        else if ($("#valueDate").is(":visible")) {
            const valueDate = this._rdpValueDate.get_selectedDate();
            if (valueDate === undefined || valueDate === null) {
                alert("Inserisci un valore");
                return;
            }
            model.ValueDate = valueDate;
        }
        else if (this._rntbValueDouble.get_visible()) {
            const valueDouble = this._rntbValueDouble.get_value();
            if (valueDouble === "") {
                alert("Inserisci un valore");
                return;
            }

            model.ValueDouble = Number(valueDouble);
        }
        else if (this._rdbGuid.get_visible()) {
            const valueGuid = this._rdbGuid.get_textBoxValue();
            if (valueGuid === "") {
                alert("Inserisci un valore");
                return;
            }

            model.ValueGuid = valueGuid;
        }
        return model;
    }

    private getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd = null;
        if ((<any>window).radWindow) wnd = (<any>window).radWindow;
        else if ((<any>window).frameElement.radWindow) wnd = (<any>window).frameElement.radWindow;
        return wnd;
    }

    private closeWindow(operator): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(operator);
    }

    private getQueryParameters(query: string): void {
        //removing ? and getting string on the right side
        if (query.indexOf('?') > -1) {
            query = query.split('?')[1];
        }

        const queryPairCollection: string[] = query.split("&");
        const queryString: { [id: string]: string } = {};

        //decompose raw query string
        for (let i = 0; i < queryPairCollection.length; i++) {
            const queryPair: string[] = queryPairCollection[i].split("=");

            const key: string = decodeURIComponent(queryPair[0]);
            const value: string = decodeURIComponent(queryPair[1]);

            queryString[key] = decodeURIComponent(value);
        }

        //populate typed query parameters
        for (const key of Object.keys(queryString)) {
            if (key === QueryParameters.QUERY_PARAM_ACTION) {
                this.queryParameters.Action = queryString[QueryParameters.QUERY_PARAM_ACTION];
                continue;
            }
            if (key === QueryParameters.QUERY_PARAM_WORKFLOW_REPOSITORY_ID) {
                this.queryParameters.WorkflowRepositoryId = queryString[QueryParameters.QUERY_PARAM_WORKFLOW_REPOSITORY_ID];
                continue;
            }
            if (key === QueryParameters.QUERY_PARAM_WORKFLOW_EVALUATION_PROPERTY_ID) {
                this.queryParameters.WorkflowEvaluationPropertyId = queryString[QueryParameters.QUERY_PARAM_WORKFLOW_EVALUATION_PROPERTY_ID];
                continue;
            }
            if (key === QueryParameters.QUERY_PARAM_START_PROPOSER) {
                this.queryParameters.StartProposer = parseInt(queryString[QueryParameters.QUERY_PARAM_START_PROPOSER]);
                continue;
            }
            if (key === QueryParameters.QUERY_PARAM_START_RECEIVER) {
                this.queryParameters.StartReceiver = parseInt(queryString[QueryParameters.QUERY_PARAM_START_RECEIVER]);
                continue;
            }
        }
    }
}

export = TbltWorkflowEvaluationPropertyGes;