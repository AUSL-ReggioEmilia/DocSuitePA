define(["require", "exports", "App/Helpers/EnumHelper", "App/Models/Commons/WorkflowValidationRulesType", "App/Helpers/SessionStorageKeysHelper"], function (require, exports, EnumHelper, WorkflowValidationRulesType, SessionStorageKeysHelper) {
    var CommonWorkflowDesignerValidations = /** @class */ (function () {
        function CommonWorkflowDesignerValidations() {
            var _this = this;
            this.btnConfirm_OnClick = function (sender, args) {
                args.set_cancel(true);
                if ($("#" + _this.lblNameId).val() == "") {
                    alert("Campo nome obbligatorio");
                    return;
                }
                if ($("#" + _this.lblMessageErrorId).val() == "") {
                    alert("Campo messaggio obbligatorio");
                    return;
                }
                var workflowRuleModel = {
                    Name: $("#" + _this.lblNameId).val(),
                    ValidationMessage: $("#" + _this.lblMessageErrorId).val()
                };
                var listOfElements = $("#" + _this.dvCheckBoxListControlId + " input");
                for (var i = 0; i <= listOfElements.length - 1; i++) {
                    var propertyName = _this._enumHelper.getValidationRuleType(listOfElements[i].closest("td").textContent);
                    workflowRuleModel[propertyName] = listOfElements[i].checked;
                }
                var wnd = _this.getRadWindow();
                wnd.close(workflowRuleModel);
            };
        }
        CommonWorkflowDesignerValidations.prototype.initialize = function () {
            this._enumHelper = new EnumHelper();
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicking(this.btnConfirm_OnClick);
            $("#" + this.lblNameId).val("");
            $("#" + this.lblMessageErrorId).val("");
            this.populateCheckBoxList();
            var wfRuleFromSession = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_RULE_MODEL);
            if (wfRuleFromSession != null) {
                this.populateFields(wfRuleFromSession);
            }
        };
        CommonWorkflowDesignerValidations.prototype.populateCheckBoxList = function () {
            var table = $('<table id="tbodyid"></table>');
            var counter = 0;
            for (var item in WorkflowValidationRulesType) {
                if (isNaN(Number(item))) {
                    table.append($('<tr></tr>').append($('<td></td>').append($('<input>').attr({
                        type: 'checkbox', name: 'chklistitem', value: item, id: 'chklistitem' + counter, checked: item == "IsExist", disabled: item == "IsExist"
                    })).append($('<label>').attr({
                        for: 'chklistitem' + counter++
                    }).text(WorkflowValidationRulesType[item]))));
                }
            }
            $("#" + this.dvCheckBoxListControlId).append(table);
        };
        CommonWorkflowDesignerValidations.prototype.populateFields = function (wfRuleFromSession) {
            var wfRuleModel = JSON.parse(wfRuleFromSession);
            $("#" + this.lblNameId).val(wfRuleModel.Name);
            $("#" + this.lblMessageErrorId).val(wfRuleModel.ValidationMessage);
            var listOfElements = $("#" + this.dvCheckBoxListControlId + " input");
            for (var i = 0; i <= listOfElements.length - 1; i++) {
                var propertyName = this._enumHelper.getValidationRuleType(listOfElements[i].closest("td").textContent);
                listOfElements[i].checked = wfRuleModel[propertyName];
            }
        };
        CommonWorkflowDesignerValidations.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        return CommonWorkflowDesignerValidations;
    }());
    return CommonWorkflowDesignerValidations;
});
//# sourceMappingURL=CommonWorkflowDesignerValidations.js.map