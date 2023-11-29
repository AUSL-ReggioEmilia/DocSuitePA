import WorkflowArgumentModel = require("App/Models/Workflows/WorkflowArgumentModel");
import WorkflowPropertyHelper = require("App/Models/Workflows/WorkflowPropertyHelper");
import WorkflowEvalutionPropertyHelper = require("App/Models/Workflows/WorkflowEvalutionPropertyHelper");
import ArgumentType = require("../App/Models/Workflows/ArgumentType");

declare var ValidatorEnable: any;

class TbltWorkflowPropertyGes {
    rcbNameId: string;
    rtbPropertyNameId: string;
    rntbValueIntId: string;
    rtbValueStringId: string;
    rlbValueBoolId: string;
    rdpValueDateId: string;
    rntbValueDoubleId: string;
    rdbGuidId: string;
    rtbValueJsonId: string;
    btnConfirmId: string;
    actionPage: string;
    argumentType: string;
    ajaxLoadingPanelId: string;
    pageContentId: string;
    argumentsDataSourceId: string;
    rfvNewValueStringId: string;
    rfvNewValueBoolId: string;
    rfvNewValueDateId: string;
    rfvNewValueGuidId: string;
    rfvNewValueJsonId: string;
    rfvNewValueDoubleId: string;
    rfvNewValueIntId: string;
    validation: string;

    private isValidatorEnabled(): boolean {
        return JSON.parse(this.validation.toLowerCase());
    }

    private static WORKFLOW_REPOSITORY: string = "WorkflowRepository";
    private static WORKFLOW_STEP: string = "WorkflowStep";
    private static WORKFLOW_STEP_ARGUMENT: string = "WorkflowStepArgument";
    private static ACTION_PAGE_EDIT = "Edit";

    private _rcbName: Telerik.Web.UI.RadComboBox;
    private _rtbPropertyName: Telerik.Web.UI.RadTextBox;
    private _rntbValueInt: Telerik.Web.UI.RadNumericTextBox;
    private _rtbValueString: Telerik.Web.UI.RadTextBox;
    private _rlbValueBool: Telerik.Web.UI.RadListBox;
    private _rdpValueDate: Telerik.Web.UI.RadDatePicker;
    private _rntbValueDouble: Telerik.Web.UI.RadNumericTextBox;
    private _rdbGuid: Telerik.Web.UI.RadTextBox;
    private _rtbValueJson: Telerik.Web.UI.RadTextBox;
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _argumentsDataSource: Telerik.Web.UI.RadClientDataSource;

    constructor() {

    }

    initialize(): void {
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._btnConfirm.add_clicked(this.btnConfirm_onClick);

        this._rtbPropertyName = <Telerik.Web.UI.RadTextBox>$find(this.rtbPropertyNameId);
        this._rcbName = <Telerik.Web.UI.RadComboBox>$find(this.rcbNameId);
        this._rcbName.add_selectedIndexChanged(this.rcbName_onSelectedIndexChanged);
        this._argumentsDataSource = <Telerik.Web.UI.RadClientDataSource>$find(this.argumentsDataSourceId);

        this._rntbValueInt = <Telerik.Web.UI.RadNumericTextBox>$find(this.rntbValueIntId);
        this._rtbValueString = <Telerik.Web.UI.RadTextBox>$find(this.rtbValueStringId);
        this._rlbValueBool = <Telerik.Web.UI.RadListBox>$find(this.rlbValueBoolId);
        this._rdpValueDate = <Telerik.Web.UI.RadDatePicker>$find(this.rdpValueDateId);
        this._rntbValueDouble = <Telerik.Web.UI.RadNumericTextBox>$find(this.rntbValueDoubleId);
        this._rdbGuid = <Telerik.Web.UI.RadTextBox>$find(this.rdbGuidId);
        this._rtbValueJson = <Telerik.Web.UI.RadTextBox>$find(this.rtbValueJsonId);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);

        this._loadingPanel.show(this.pageContentId);
        this.resetValueVisibility();

        this.populateComboNames();

        this.populateFieldsForEdit();
        this._loadingPanel.hide(this.pageContentId);
    }

    rcbName_onSelectedIndexChanged = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        this.resetValueVisibility();
        const value: string = args.get_item().get_value();
        this.dynamicallyAdjustInputFields(value);
        this._rtbPropertyName.set_value(value);
    }

    btnConfirm_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        let isValid: boolean;
        const repositoryId: string = sessionStorage[TbltWorkflowPropertyGes.WORKFLOW_REPOSITORY];
        const stepPosition: string = sessionStorage[TbltWorkflowPropertyGes.WORKFLOW_STEP];

        let wfProp = this._rtbPropertyName.get_value();

        const propertyType: ArgumentType = WorkflowEvalutionPropertyHelper[wfProp] !== undefined ? WorkflowEvalutionPropertyHelper[wfProp].Type : ArgumentType.PropertyString;

        const workflowArgument: WorkflowArgumentModel = {
            Name: wfProp,
            PropertyType: propertyType,
            ValueInt: null,
            ValueDate: null,
            ValueDouble: null,
            ValueBoolean: null,
            ValueGuid: null,
            ValueString: null,
            ValueJson: null
        }
        isValid = true;
        if (this.isValidatorEnabled()) {
            switch (propertyType) {
                case ArgumentType.PropertyString: {
                    const valueString = this._rtbValueString.get_textBoxValue() ? this._rtbValueString.get_textBoxValue() : null;
                    if (!valueString) {
                        ValidatorEnable($get(this.rfvNewValueStringId), true);
                        isValid = false;
                    }
                    else {
                        workflowArgument.ValueString = valueString;
                    }
                    break;
                }
                case ArgumentType.PropertyBoolean: {
                    const valueBool = this._rlbValueBool.get_selectedItem();
                    if (!valueBool) {
                        ValidatorEnable($get(this.rfvNewValueBoolId), true);
                        isValid = false;
                    }
                    else {
                        workflowArgument.ValueBoolean = valueBool.get_value() === "1";
                    }
                    break;
                }
                case ArgumentType.PropertyInt: {
                    const valueInt = this._rntbValueInt.get_value();
                        workflowArgument.ValueInt = Number(valueInt);
                    break;
                }
                case ArgumentType.PropertyDate: {
                    const valueDate = this._rdpValueDate.get_selectedDate();
                    if (!valueDate) {
                        ValidatorEnable($get(this.rfvNewValueDateId), true);
                        isValid = false;
                    }
                    else {
                        workflowArgument.ValueDate = valueDate;
                    }
                    break;
                }
                case ArgumentType.PropertyDouble: {
                    const valueDouble = this._rntbValueDouble.get_value();
                    if (!valueDouble) {
                        ValidatorEnable($get(this.rfvNewValueDoubleId), true);
                        isValid = false;
                    }
                    else {
                        workflowArgument.ValueDouble = Number(valueDouble);
                    }
                    break;
                }
                case ArgumentType.PropertyGuid: {
                    const valueGuid = this._rdbGuid.get_textBoxValue();
                    if (!valueGuid) {
                        ValidatorEnable($get(this.rfvNewValueGuidId), true);
                        isValid = false;
                    }
                    else {
                        workflowArgument.ValueGuid = valueGuid;
                    }
                    break;
                }
                case ArgumentType.Json: {
                    const valueJson = this._rtbValueJson.get_textBoxValue();
                    if (!valueJson) {
                        ValidatorEnable($get(this.rfvNewValueBoolId), true);
                        isValid = false;
                    }
                    else {
                        workflowArgument.ValueJson = valueJson;
                    }
                    break;
                }
            }
        }
       
        if (isValid) {
            this.closeWindow(workflowArgument, repositoryId, stepPosition);
        }
    }


    private closeWindow(workflowArgument: WorkflowArgumentModel, uniqueId: string, stepPosition: string): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        let obj = {
            WorkflowArgument: workflowArgument,
            UniqueId: uniqueId,
            StepPosition: Number(stepPosition),
            ArgumentType: this.argumentType
        }
        wnd.close(obj);
    }

    private getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }

    
    private resetValueVisibility() {
        $("#idValue").css("display", "block");
        this._rntbValueInt.set_visible(false);
        this._rtbValueString.set_visible(false);
        $("#valueBool").hide();
        $("#valueDate").hide();
        this._rntbValueDouble.set_visible(false);
        this._rdbGuid.set_visible(false);
        this._rtbValueJson.set_visible(false);
    }

    private populateComboNames() {
        let obj = this;
        obj._rcbName.get_items().clear();
        $.each(WorkflowPropertyHelper, function (index: string, item: string) {
            if (typeof item !== "function") {
                const workflowPropertyItem = WorkflowEvalutionPropertyHelper[item];
                if (workflowPropertyItem) {
                    obj._argumentsDataSource.add({ Name: `${workflowPropertyItem.Name} (${item})`, Value: item });
                } else {
                    obj._argumentsDataSource.add({ Name: item, Value: item });
                }
            }
        })
    }

    private dynamicallyAdjustInputFields(value: string): void {
        const prop = WorkflowEvalutionPropertyHelper[value];
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
                default: {
                    this._rtbValueString.set_visible(true);
                    this._rtbValueString.clear();
                    break;
                }
            }
        }
        else {
            this._rtbValueString.set_visible(true);
            this._rtbValueString.clear();
        }

        this._rcbName.hideDropDown();
    }

    private populateFieldsForEdit() {
        if (this.actionPage === TbltWorkflowPropertyGes.ACTION_PAGE_EDIT) {
            const argument = JSON.parse(sessionStorage[TbltWorkflowPropertyGes.WORKFLOW_STEP_ARGUMENT]);

            const name: string = argument.Name;
            const value = argument.Value;

            this._rtbPropertyName.set_value(name);
            this._rcbName.set_text(name);
            this._rcbName.disable();
            const prop = WorkflowEvalutionPropertyHelper[name];
            if (prop) {
                this._rcbName.set_text(prop.Name);
                switch (prop.Type) {
                    case ArgumentType.PropertyString: {
                        this._rtbValueString.set_visible(true);
                        this._rtbValueString.set_value(value);
                        break;
                    }
                    case ArgumentType.PropertyBoolean: {
                        $("#valueBool").show();
                        let item = this._rlbValueBool.getItem(Number(value));
                        item.select();
                        break;
                    }
                    case ArgumentType.PropertyInt: {
                        this._rntbValueInt.set_visible(true);
                        this._rntbValueInt.set_value(value);
                        break;
                    }
                    case ArgumentType.PropertyDate: {
                        $("#valueDate").show();
                        this._rdpValueDate.set_selectedDate(moment(value).isValid() ? new Date(value) : null);
                        break;
                    }
                    case ArgumentType.PropertyDouble: {
                        this._rntbValueDouble.set_visible(true);
                        this._rntbValueDouble.set_value(value);
                        break;
                    }
                    case ArgumentType.PropertyGuid: {
                        this._rdbGuid.set_visible(true);
                        this._rdbGuid.set_textBoxValue(value);
                        break;
                    }
                    case ArgumentType.Json: {
                        this._rtbValueJson.set_visible(true);
                        this._rtbValueJson.set_value(value);
                        break;
                    }
                    default: {
                        this._rtbValueString.set_visible(true);
                        this._rtbValueString.set_value(value);
                        break;
                    }
                }
            }
            else {
                this._rtbValueString.set_visible(true);
                this._rtbValueString.set_value(value);
            }
        }
    }

}
export = TbltWorkflowPropertyGes;