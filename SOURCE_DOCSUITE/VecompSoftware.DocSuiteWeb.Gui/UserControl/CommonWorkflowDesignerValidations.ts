import WorkflowRule = require("App/Models/Workflows/WorkflowRule");
import EnumHelper = require("App/Helpers/EnumHelper");
import GenericHelper = require("App/Helpers/GenericHelper");
import WorkflowValidationRulesType = require("App/Models/Commons/WorkflowValidationRulesType");
import SessionStorageKeysHelper = require("App/Helpers/SessionStorageKeysHelper");

class CommonWorkflowDesignerValidations {
    btnConfirmId: string;
    pageContentId: string;
    lblNameId: string;
    lblMessageErrorId: string;
    dvCheckBoxListControlId: string;

    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _enumHelper: EnumHelper;

    initialize(): void {
        this._enumHelper = new EnumHelper();
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._btnConfirm.add_clicking(this.btnConfirm_OnClick);

        $(`#${this.lblNameId}`).val("");
        $(`#${this.lblMessageErrorId}`).val("");

        this.populateCheckBoxList();
        let wfRuleFromSession = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_RULE_MODEL);
        if (wfRuleFromSession != null) {
            this.populateFields(wfRuleFromSession);
        }
    }

    private populateCheckBoxList(): void {
        let table = $('<table id="tbodyid"></table>');
        let counter = 0;
        for (let item in WorkflowValidationRulesType) {
            if (isNaN(Number(item))) {
                table.append($('<tr></tr>').append($('<td></td>').append($('<input>').attr({
                    type: 'checkbox', name: 'chklistitem', value: item, id: 'chklistitem' + counter, checked: item == "IsExist", disabled: item == "IsExist"
                })).append(
                    $('<label>').attr({
                        for: 'chklistitem' + counter++
                    }).text(WorkflowValidationRulesType[item]))));
            }
        }
        $(`#${this.dvCheckBoxListControlId}`).append(table);
    }

    private populateFields(wfRuleFromSession): void {
        let wfRuleModel: WorkflowRule = JSON.parse(wfRuleFromSession);
        $(`#${this.lblNameId}`).val(wfRuleModel.Name);
        $(`#${this.lblMessageErrorId}`).val(wfRuleModel.ValidationMessage);

        let listOfElements: any = $(`#${this.dvCheckBoxListControlId} input`);

        for (let i = 0; i <= listOfElements.length - 1; i++) {
            let propertyName: string = this._enumHelper.getValidationRuleType(listOfElements[i].closest("td").textContent);
            listOfElements[i].checked = wfRuleModel[propertyName];
        }
    }

    btnConfirm_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonCancelEventArgs) => {
        args.set_cancel(true);

        if ($(`#${this.lblNameId}`).val() == "") {
            alert("Campo nome obbligatorio");
            return;
        }

        if ($(`#${this.lblMessageErrorId}`).val() == "") {
            alert("Campo messaggio obbligatorio");
            return;
        }

        let workflowRuleModel: WorkflowRule = <WorkflowRule>{
            Name: $(`#${this.lblNameId}`).val(),
            ValidationMessage: $(`#${this.lblMessageErrorId}`).val()
        };

        let listOfElements: any = $(`#${this.dvCheckBoxListControlId} input`);

        for (let i = 0; i <= listOfElements.length - 1; i++) {
            let propertyName: string = this._enumHelper.getValidationRuleType(listOfElements[i].closest("td").textContent);
            workflowRuleModel[propertyName] = listOfElements[i].checked;
        }

        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(workflowRuleModel);
    }

    protected getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }
}

export =CommonWorkflowDesignerValidations;