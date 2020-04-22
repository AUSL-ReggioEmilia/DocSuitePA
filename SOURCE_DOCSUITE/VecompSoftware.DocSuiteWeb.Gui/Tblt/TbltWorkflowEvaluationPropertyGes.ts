import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import WorkflowEvalutionPropertyHelper = require('App/Models/Workflows/WorkflowEvalutionPropertyHelper');
import WorkflowEvaluationPropertyService = require('App/Services/Workflows/WorkflowEvaluationPropertyService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import WorkflowEvaluationProperty = require('App/Models/Workflows/WorkflowEvaluationProperty');
import WorkflowType = require('App/Models/Workflows/WorkflowType');
import WorkflowPropertyType = require('App/Models/Workflows/WorkflowPropertyType');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import WorkflowRepositoryModel = require('App/Models/Workflows/WorkflowRepositoryModel');
import EnumHelper = require("App/Helpers/EnumHelper");
import WorkflowEvaluationPropertyType = require('App/Models/Workflows/WorkflowEvaluationPropertyType');

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

    private _rcbName: Telerik.Web.UI.RadComboBox;
    private _rntbValueInt: Telerik.Web.UI.RadNumericTextBox;
    private _rtbValueString: Telerik.Web.UI.RadTextBox;
    private _rtbValueJson: Telerik.Web.UI.RadTextBox;
    private _rlbValueBool: Telerik.Web.UI.RadListBox;
    private _rdpValueDate: Telerik.Web.UI.RadDatePicker;
    private _rntbValueDouble: Telerik.Web.UI.RadNumericTextBox;
    private _rdbGuid: Telerik.Web.UI.RadTextBox;

    private btnConfirm: Telerik.Web.UI.RadButton;

    private _serviceConfigurations: ServiceConfiguration[];
    private _service: WorkflowEvaluationPropertyService;
    private _enumHelper: EnumHelper;


    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
    }

    initialize(): void {
        let serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowEvaluationProperty");
        this._service = new WorkflowEvaluationPropertyService(serviceConfiguration);

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

        this.populateComboNames();
        this.resetValueVisibility();

        this.initializeEditOperation();
    }

    rcbName_OnClientItemsRequested = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxRequestEventArgs) => {
        try {
            let filteringList = [];
            $.each(WorkflowEvalutionPropertyHelper, (key, value) => {
                if (value.Name != undefined || value.Type != undefined) {

                    filteringList.push({ Key: key, Name: value.Name, Type: value.Type });
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
        let qs = this.parse_query_string(window.location.search);
        let param = qs["?Action"];
        if (param === "Edit") {
            let propId = qs["WorkflowEvaluationPropertyId"];
            this._service.getWorkflowEvaluationProperty(propId, (data: WorkflowEvaluationProperty) => {
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

    private populateComboNames() {
        let cmbItem: Telerik.Web.UI.RadComboBoxItem = null;
        let obj = this;
        let workflowEvaluationPropertyHelper = [];
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
                cmbItem.set_text(item.Value.Name);
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
    }

    private rcbName_onSelectedIndexChanged = (sender: any, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        this.resetValueVisibility();
        let vals = args.get_item().get_value();
        this.dynamicallyAdjustInputFields(vals);
    }

    private dynamicallyAdjustInputFields(vals: string): void {
        if (WorkflowEvalutionPropertyHelper[vals].Type === WorkflowEvaluationPropertyType.String) {
            this._rtbValueString.set_visible(true);
            this._rtbValueString.clear();
        }
        else if (WorkflowEvalutionPropertyHelper[vals].Type === WorkflowEvaluationPropertyType.Boolean) {
            $("#valueBool").show();
            this._rlbValueBool.clearSelection();
        }
        else if (WorkflowEvalutionPropertyHelper[vals].Type === WorkflowEvaluationPropertyType.Json) {
            this._rtbValueJson.set_visible(true);
            this._rtbValueJson.clear();
        }
        else if (WorkflowEvalutionPropertyHelper[vals].Type === WorkflowEvaluationPropertyType.Integer) {
            this._rntbValueInt.set_visible(true);
            this._rntbValueInt.clear();
        }
        else if (WorkflowEvalutionPropertyHelper[vals].Type === WorkflowEvaluationPropertyType.Date) {
            $("#valueDate").show();
            this._rdpValueDate.clear();
        }
        else if (WorkflowEvalutionPropertyHelper[vals].Type === WorkflowEvaluationPropertyType.Double) {
            this._rntbValueDouble.set_visible(true);
            this._rntbValueDouble.clear();
        }
        else if (WorkflowEvalutionPropertyHelper[vals].Type === WorkflowEvaluationPropertyType.Guid) {
            this._rdbGuid.set_visible(true);
            this._rdbGuid.clear();
        }
        this._rcbName.hideDropDown();
    }

    private btnConfirm_onClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonEventArgs) => {
        let qs = this.parse_query_string(window.location.search);

        let param = qs["?Action"];
        if (param === "Add") {
            this.AddWorkflowEvaluationProperty(qs["WorkflowRepositoryId"]);
        } else if (param === "Edit") {
            this.EditWorkflowEvaluationProperty(qs["WorkflowRepositoryId"], qs["WorkflowEvaluationPropertyId"]);
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
        this._service.updateWorkflowEvaluationProperty(validModel, (data: any) => {
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
        this._service.insertWorkflowEvaluationProperty(model,
            (data: any) => {
                let operator = this.getRadWindow();
                this.closeWindow(operator);
            },
            (exception: ExceptionDTO) => {
                console.log(exception);
            });
    }

    private checkModelValidation(model: WorkflowEvaluationProperty): WorkflowEvaluationProperty {
        if (this._rtbValueString.get_visible()) {
            let valueString = this._rtbValueString.get_textBoxValue();
            if (valueString == "") {
                alert("Inserisci un valore");
                return;
            }
            model.ValueString = valueString;
        }
        else if ($("#valueBool").is(":visible")) {
            let valueBool = this._rlbValueBool.get_selectedItem();
            if (valueBool == undefined || valueBool == null) {
                alert("Inserisci un valore");
                return;
            }
            model.ValueBoolean = valueBool.get_value() === "1";
        }
        else if (this._rntbValueInt.get_visible()) {
            let valueInt = this._rntbValueInt.get_value();
            if (valueInt === "") {
                alert("Inserisci un valore");
                return;
            }

            model.ValueInt = Number(valueInt);
        }
        else if (this._rtbValueJson.get_visible()) {
            let valueJson = this._rtbValueJson.get_textBoxValue();
            if (valueJson == "") {
                alert("Inserisci un valore");
                return;
            }
            model.ValueString = valueJson;
        }
        else if ($("#valueDate").is(":visible")) {
            let valueDate = this._rdpValueDate.get_selectedDate();
            if (valueDate == undefined || valueDate == null) {
                alert("Inserisci un valore");
                return;
            }
            model.ValueDate = valueDate;
        }
        else if (this._rntbValueDouble.get_visible()) {
            let valueDouble = this._rntbValueDouble.get_value();
            if (valueDouble === "") {
                alert("Inserisci un valore");
                return;
            }

            model.ValueDouble = Number(valueDouble);
        }
        else if (this._rdbGuid.get_visible()) {
            let valueGuid = this._rdbGuid.get_textBoxValue();
            if (valueGuid == "") {
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


    private parse_query_string(query): any {
        var vars = query.split("&");
        var query_string = {};
        for (var i = 0; i < vars.length; i++) {
            var pair = vars[i].split("=");
            var key = decodeURIComponent(pair[0]);
            var value = decodeURIComponent(pair[1]);
            if (typeof query_string[key] === "undefined") {
                query_string[key] = decodeURIComponent(value);
            } else if (typeof query_string[key] === "string") {
                var arr = [query_string[key], decodeURIComponent(value)];
                query_string[key] = arr;
            } else {
                query_string[key].push(decodeURIComponent(value));
            }
        }
        return query_string;
    }

}

export = TbltWorkflowEvaluationPropertyGes;