import WorkflowArgumentModel = require("../App/Models/Workflows/WorkflowArgumentModel");
import WorkflowPropertyType = require("../App/Models/Workflows/WorkflowPropertyType");
import WorkflowPropertyHelper = require("../App/Models/Workflows/WorkflowPropertyHelper");
import WorkflowEvaluationPropertyType = require("../App/Models/Workflows/WorkflowEvaluationPropertyType");
import WorkflowEvalutionPropertyHelper = require("../App/Models/Workflows/WorkflowEvalutionPropertyHelper");
import ArgumentType = require("../App/Models/Workflows/ArgumentType");
import WorkflowRepositoryModel = require("../App/Models/Workflows/WorkflowRepositoryModel");
import ValidationCode = require("../App/Models/Validations/ValidationCode");
import FascAddUDLink = require("../Fasc/FascAddUDLink");
declare var ValidatorEnable: any;

class TbltWorkflowPropertyGes {
    rcbNameId: string;
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

    private get isValidatorEnabled(): boolean {
        return JSON.parse(this.validation.toLowerCase());
    }

    private static WORKFLOW_REPOSITORY: string = "WorkflowRepository";
    private static WORKFLOW_STEP: string = "WorkflowStep";
    private static WORKFLOW_STEP_ARGUMENT: string = "WorkflowStepArgument";
    private static ACTION_PAGE_EDIT = "Edit";

    private _rcbName: Telerik.Web.UI.RadComboBox;
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
        let value = args.get_item().get_value();
        this.dynamicallyAdjustInputFields(value);
    }

    btnConfirm_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        let isValid: boolean;
        let repositoryId: string = sessionStorage[TbltWorkflowPropertyGes.WORKFLOW_REPOSITORY];
        let stepPosition: string = sessionStorage[TbltWorkflowPropertyGes.WORKFLOW_STEP];

        let argumentName: string = this._rcbName.get_text();
        let propertyType = WorkflowEvalutionPropertyHelper[argumentName] != undefined ? WorkflowEvalutionPropertyHelper[argumentName].Type : WorkflowEvaluationPropertyType.String;

        let workflowArgument: WorkflowArgumentModel = {
            Name: argumentName,
            PropertyType: propertyType,
            ValueInt: null,
            ValueDate: null,
            ValueDouble: null,
            ValueBoolean: null,
            ValueGuid: null,
            ValueString: null,
            ValueJson: null
        }

        switch (propertyType) {
            case WorkflowEvaluationPropertyType.String: {
                let valueString = this._rtbValueString.get_textBoxValue() ? this._rtbValueString.get_textBoxValue() : null;
                if (valueString == null) {
                    ValidatorEnable($get(this.rfvNewValueStringId), true);
                    isValid = false;
                }
                else {
                    isValid = true;
                    workflowArgument.ValueString = valueString;
                }
                break;
            }
            case WorkflowEvaluationPropertyType.Boolean: {
                let valueBool = this._rlbValueBool.get_selectedItem();
                if (valueBool == null) {
                    ValidatorEnable($get(this.rfvNewValueBoolId), true);
                    isValid = false;
                }
                else {
                    isValid = true;
                    workflowArgument.ValueBoolean = valueBool.get_value() === "1";
                }
                break;
            }
            case WorkflowEvaluationPropertyType.Integer: {
                let valueInt = this._rntbValueInt.get_value();
                if (valueInt == "") {
                    ValidatorEnable($get(this.rfvNewValueIntId), true);
                    isValid = false;
                }
                else {
                    isValid = true;
                    workflowArgument.ValueInt = Number(valueInt);
                }
                break;
            }
            case WorkflowEvaluationPropertyType.Date: {
                let valueDate = this._rdpValueDate.get_selectedDate();
                if (valueDate == null) {
                    ValidatorEnable($get(this.rfvNewValueDateId), true);
                    isValid = false;
                }
                else {
                    isValid = true;
                    workflowArgument.ValueDate = valueDate;
                }
                break;
            }
            case WorkflowEvaluationPropertyType.Double: {
                let valueDouble = this._rntbValueDouble.get_value();
                if (valueDouble == "") {
                    ValidatorEnable($get(this.rfvNewValueDoubleId), true);
                    isValid = false;
                }
                else {
                    isValid = true;
                    workflowArgument.ValueDouble = Number(valueDouble);
                }
                break;
            }
            case WorkflowEvaluationPropertyType.Guid: {
                let valueGuid = this._rdbGuid.get_textBoxValue();
                if (valueGuid == "") {
                    ValidatorEnable($get(this.rfvNewValueGuidId), true);
                    isValid = false;
                }
                else {
                    isValid = true;
                    workflowArgument.ValueGuid = valueGuid;
                }
                break;
            }
            case WorkflowEvaluationPropertyType.Json: {
                let valueJson = this._rtbValueJson.get_textBoxValue();
                if (valueJson == "") {
                    ValidatorEnable($get(this.rfvNewValueBoolId), true);
                    isValid = false;
                }
                else {
                    isValid = true;
                    workflowArgument.ValueJson = valueJson;
                }
                break;
            }
        }
        if (this.isValidatorEnabled && isValid) {
            this.closeWindow(workflowArgument, repositoryId, stepPosition);
        }
        else if (!this.isValidatorEnabled) {
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
    private dynamicallyAdjustInputFields(value: string): void {

        if (WorkflowEvalutionPropertyHelper[value] != undefined) {

            switch (WorkflowEvalutionPropertyHelper[value].Type) {
                case WorkflowEvaluationPropertyType.String: {
                    this._rtbValueString.set_visible(true);
                    this._rtbValueString.clear();
                    break;
                }
                case WorkflowEvaluationPropertyType.Boolean: {
                    $("#valueBool").show();
                    this._rlbValueBool.clearSelection();
                    break;
                }
                case WorkflowEvaluationPropertyType.Integer: {
                    this._rntbValueInt.set_visible(true);
                    this._rntbValueInt.clear();
                    break;
                }
                case WorkflowEvaluationPropertyType.Date: {
                    $("#valueDate").show();
                    this._rdpValueDate.clear();
                    break;
                }
                case WorkflowEvaluationPropertyType.Double: {
                    this._rntbValueDouble.set_visible(true);
                    this._rntbValueDouble.clear();
                    break;
                }
                case WorkflowEvaluationPropertyType.Guid: {
                    this._rdbGuid.set_visible(true);
                    this._rdbGuid.clear();
                    break;
                }
                case WorkflowEvaluationPropertyType.Json: {
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
            if (typeof item != "function") {
                obj._argumentsDataSource.add({ Name: item, Value: item });
            }
        })
    }

    private populateFieldsForEdit() {
        if (this.actionPage == TbltWorkflowPropertyGes.ACTION_PAGE_EDIT) {
            let argument: any = JSON.parse(sessionStorage[TbltWorkflowPropertyGes.WORKFLOW_STEP_ARGUMENT]);

            let name: string = argument.Name;
            let value: any = argument.Value;
            this._rcbName.set_text(name);
            this._rcbName.disable();

            if (WorkflowEvalutionPropertyHelper[name] != undefined) {

                switch (WorkflowEvalutionPropertyHelper[name].Type) {
                    case WorkflowEvaluationPropertyType.String: {
                        this._rtbValueString.set_visible(true);
                        this._rtbValueString.set_value(value);
                        break;
                    }
                    case WorkflowEvaluationPropertyType.Boolean: {
                        $("#valueBool").show();
                        let item = this._rlbValueBool.getItem(Number(value));
                        item.select();
                        break;
                    }
                    case WorkflowEvaluationPropertyType.Integer: {
                        this._rntbValueInt.set_visible(true);
                        this._rntbValueInt.set_value(value);
                        break;
                    }
                    case WorkflowEvaluationPropertyType.Date: {
                        $("#valueDate").show();
                        this._rdpValueDate.set_selectedDate(moment(value).isValid() ? new Date(value) : null);
                        break;
                    }
                    case WorkflowEvaluationPropertyType.Double: {
                        this._rntbValueDouble.set_visible(true);
                        this._rntbValueDouble.set_value(value);
                        break;
                    }
                    case WorkflowEvaluationPropertyType.Guid: {
                        this._rdbGuid.set_visible(true);
                        this._rdbGuid.set_textBoxValue(value);
                        break;
                    }
                    case WorkflowEvaluationPropertyType.Json: {
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