define(["require", "exports", "App/Models/Workflows/WorkflowPropertyHelper", "App/Models/Workflows/WorkflowEvalutionPropertyHelper", "../App/Models/Workflows/ArgumentType"], function (require, exports, WorkflowPropertyHelper, WorkflowEvalutionPropertyHelper, ArgumentType) {
    var TbltWorkflowPropertyGes = /** @class */ (function () {
        function TbltWorkflowPropertyGes() {
            var _this = this;
            this.rcbName_onSelectedIndexChanged = function (sender, args) {
                _this.resetValueVisibility();
                var value = args.get_item().get_value();
                _this.dynamicallyAdjustInputFields(value);
                _this._rtbPropertyName.set_value(value);
            };
            this.btnConfirm_onClick = function (sender, args) {
                var isValid;
                var repositoryId = sessionStorage[TbltWorkflowPropertyGes.WORKFLOW_REPOSITORY];
                var stepPosition = sessionStorage[TbltWorkflowPropertyGes.WORKFLOW_STEP];
                var wfProp = _this._rcbName.get_value();
                if (wfProp === "") {
                    wfProp = _this._rcbName.get_text();
                }
                var propertyType = WorkflowEvalutionPropertyHelper[wfProp] !== undefined ? WorkflowEvalutionPropertyHelper[wfProp].Type : ArgumentType.PropertyString;
                var workflowArgument = {
                    Name: wfProp,
                    PropertyType: propertyType,
                    ValueInt: null,
                    ValueDate: null,
                    ValueDouble: null,
                    ValueBoolean: null,
                    ValueGuid: null,
                    ValueString: null,
                    ValueJson: null
                };
                isValid = true;
                if (_this.isValidatorEnabled()) {
                    switch (propertyType) {
                        case ArgumentType.PropertyString: {
                            var valueString = _this._rtbValueString.get_textBoxValue() ? _this._rtbValueString.get_textBoxValue() : null;
                            if (!valueString) {
                                ValidatorEnable($get(_this.rfvNewValueStringId), true);
                                isValid = false;
                            }
                            else {
                                workflowArgument.ValueString = valueString;
                            }
                            break;
                        }
                        case ArgumentType.PropertyBoolean: {
                            var valueBool = _this._rlbValueBool.get_selectedItem();
                            if (!valueBool) {
                                ValidatorEnable($get(_this.rfvNewValueBoolId), true);
                                isValid = false;
                            }
                            else {
                                workflowArgument.ValueBoolean = valueBool.get_value() === "1";
                            }
                            break;
                        }
                        case ArgumentType.PropertyInt: {
                            var valueInt = _this._rntbValueInt.get_value();
                            if (!valueInt) {
                                ValidatorEnable($get(_this.rfvNewValueIntId), true);
                                isValid = false;
                            }
                            else {
                                workflowArgument.ValueInt = Number(valueInt);
                            }
                            break;
                        }
                        case ArgumentType.PropertyDate: {
                            var valueDate = _this._rdpValueDate.get_selectedDate();
                            if (!valueDate) {
                                ValidatorEnable($get(_this.rfvNewValueDateId), true);
                                isValid = false;
                            }
                            else {
                                workflowArgument.ValueDate = valueDate;
                            }
                            break;
                        }
                        case ArgumentType.PropertyDouble: {
                            var valueDouble = _this._rntbValueDouble.get_value();
                            if (!valueDouble) {
                                ValidatorEnable($get(_this.rfvNewValueDoubleId), true);
                                isValid = false;
                            }
                            else {
                                workflowArgument.ValueDouble = Number(valueDouble);
                            }
                            break;
                        }
                        case ArgumentType.PropertyGuid: {
                            var valueGuid = _this._rdbGuid.get_textBoxValue();
                            if (!valueGuid) {
                                ValidatorEnable($get(_this.rfvNewValueGuidId), true);
                                isValid = false;
                            }
                            else {
                                workflowArgument.ValueGuid = valueGuid;
                            }
                            break;
                        }
                        case ArgumentType.Json: {
                            var valueJson = _this._rtbValueJson.get_textBoxValue();
                            if (!valueJson) {
                                ValidatorEnable($get(_this.rfvNewValueBoolId), true);
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
                    _this.closeWindow(workflowArgument, repositoryId, stepPosition);
                }
            };
        }
        TbltWorkflowPropertyGes.prototype.isValidatorEnabled = function () {
            return JSON.parse(this.validation.toLowerCase());
        };
        TbltWorkflowPropertyGes.prototype.initialize = function () {
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicked(this.btnConfirm_onClick);
            this._rtbPropertyName = $find(this.rtbPropertyNameId);
            this._rcbName = $find(this.rcbNameId);
            this._rcbName.add_selectedIndexChanged(this.rcbName_onSelectedIndexChanged);
            this._argumentsDataSource = $find(this.argumentsDataSourceId);
            this._rntbValueInt = $find(this.rntbValueIntId);
            this._rtbValueString = $find(this.rtbValueStringId);
            this._rlbValueBool = $find(this.rlbValueBoolId);
            this._rdpValueDate = $find(this.rdpValueDateId);
            this._rntbValueDouble = $find(this.rntbValueDoubleId);
            this._rdbGuid = $find(this.rdbGuidId);
            this._rtbValueJson = $find(this.rtbValueJsonId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._loadingPanel.show(this.pageContentId);
            this.resetValueVisibility();
            this.populateComboNames();
            this.populateFieldsForEdit();
            this._loadingPanel.hide(this.pageContentId);
        };
        TbltWorkflowPropertyGes.prototype.closeWindow = function (workflowArgument, uniqueId, stepPosition) {
            var wnd = this.getRadWindow();
            var obj = {
                WorkflowArgument: workflowArgument,
                UniqueId: uniqueId,
                StepPosition: Number(stepPosition),
                ArgumentType: this.argumentType
            };
            wnd.close(obj);
        };
        TbltWorkflowPropertyGes.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        TbltWorkflowPropertyGes.prototype.resetValueVisibility = function () {
            $("#idValue").css("display", "block");
            this._rntbValueInt.set_visible(false);
            this._rtbValueString.set_visible(false);
            $("#valueBool").hide();
            $("#valueDate").hide();
            this._rntbValueDouble.set_visible(false);
            this._rdbGuid.set_visible(false);
            this._rtbValueJson.set_visible(false);
        };
        TbltWorkflowPropertyGes.prototype.populateComboNames = function () {
            var obj = this;
            obj._rcbName.get_items().clear();
            $.each(WorkflowPropertyHelper, function (index, item) {
                if (typeof item !== "function") {
                    var workflowPropertyItem = WorkflowEvalutionPropertyHelper[item];
                    if (workflowPropertyItem) {
                        obj._argumentsDataSource.add({ Name: workflowPropertyItem.Name + " (" + item + ")", Value: item });
                    }
                    else {
                        obj._argumentsDataSource.add({ Name: item, Value: item });
                    }
                }
            });
        };
        TbltWorkflowPropertyGes.prototype.dynamicallyAdjustInputFields = function (value) {
            var prop = WorkflowEvalutionPropertyHelper[value];
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
        };
        TbltWorkflowPropertyGes.prototype.populateFieldsForEdit = function () {
            if (this.actionPage === TbltWorkflowPropertyGes.ACTION_PAGE_EDIT) {
                var argument = JSON.parse(sessionStorage[TbltWorkflowPropertyGes.WORKFLOW_STEP_ARGUMENT]);
                var name_1 = argument.Name;
                var value = argument.Value;
                this._rtbPropertyName.set_value(name_1);
                this._rcbName.set_text(name_1);
                this._rcbName.disable();
                var prop = WorkflowEvalutionPropertyHelper[name_1];
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
                            var item = this._rlbValueBool.getItem(Number(value));
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
        };
        TbltWorkflowPropertyGes.WORKFLOW_REPOSITORY = "WorkflowRepository";
        TbltWorkflowPropertyGes.WORKFLOW_STEP = "WorkflowStep";
        TbltWorkflowPropertyGes.WORKFLOW_STEP_ARGUMENT = "WorkflowStepArgument";
        TbltWorkflowPropertyGes.ACTION_PAGE_EDIT = "Edit";
        return TbltWorkflowPropertyGes;
    }());
    return TbltWorkflowPropertyGes;
});
//# sourceMappingURL=TbltWorkflowPropertyGes.js.map