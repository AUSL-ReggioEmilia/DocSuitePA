define(["require", "exports", "App/Models/Workflows/WorkflowEvalutionPropertyHelper", "App/Services/Workflows/WorkflowEvaluationPropertyService", "App/Helpers/ServiceConfigurationHelper", "App/Models/Workflows/WorkflowType", "App/Helpers/EnumHelper", "App/Models/Workflows/WorkflowEvaluationPropertyType"], function (require, exports, WorkflowEvalutionPropertyHelper, WorkflowEvaluationPropertyService, ServiceConfigurationHelper, WorkflowType, EnumHelper, WorkflowEvaluationPropertyType) {
    var TbltWorkflowEvaluationPropertyGes = /** @class */ (function () {
        function TbltWorkflowEvaluationPropertyGes(serviceConfigurations) {
            var _this = this;
            this.rcbName_OnClientItemsRequested = function (sender, args) {
                try {
                    var filteringList_1 = [];
                    $.each(WorkflowEvalutionPropertyHelper, function (key, value) {
                        if (value.Name != undefined || value.Type != undefined) {
                            filteringList_1.push({ Key: key, Name: value.Name, Type: value.Type });
                        }
                    });
                    var filteredList = filteringList_1.filter(function (x) { return x.Name.toLowerCase().indexOf(sender.get_text().toLowerCase()) !== -1; });
                    _this.refreshNames(filteredList);
                }
                catch (error) {
                }
            };
            this.refreshNames = function (data) {
                if (data.length > 0) {
                    _this._rcbName.beginUpdate();
                    $.each(data, function (key, value) {
                        var item = new Telerik.Web.UI.RadComboBoxItem();
                        item.set_text(value.Name);
                        item.set_value(value.Key);
                        _this._rcbName.get_items().add(item);
                    });
                    _this._rcbName.showDropDown();
                    _this._rcbName.endUpdate();
                }
            };
            this.rcbName_onSelectedIndexChanged = function (sender, args) {
                _this.resetValueVisibility();
                var vals = args.get_item().get_value();
                _this.dynamicallyAdjustInputFields(vals);
            };
            this.btnConfirm_onClick = function (sender, args) {
                var qs = _this.parse_query_string(window.location.search);
                var param = qs["?Action"];
                if (param === "Add") {
                    _this.AddWorkflowEvaluationProperty(qs["WorkflowRepositoryId"]);
                }
                else if (param === "Edit") {
                    _this.EditWorkflowEvaluationProperty(qs["WorkflowRepositoryId"], qs["WorkflowEvaluationPropertyId"]);
                }
                return false;
            };
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
        }
        TbltWorkflowEvaluationPropertyGes.prototype.initialize = function () {
            var serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowEvaluationProperty");
            this._service = new WorkflowEvaluationPropertyService(serviceConfiguration);
            this.btnConfirm = $find(this.btnConfirmId);
            this.btnConfirm.add_clicked(this.btnConfirm_onClick);
            this._rcbName = $find(this.rcbNameId);
            this._rcbName.add_selectedIndexChanged(this.rcbName_onSelectedIndexChanged);
            this._rcbName.add_itemsRequested(this.rcbName_OnClientItemsRequested);
            this._rntbValueInt = $find(this.rntbValueIntId);
            this._rtbValueString = $find(this.rtbValueStringId);
            this._rtbValueJson = $find(this.rtbValueJsonId);
            this._rlbValueBool = $find(this.rlbValueBoolId);
            this._rdpValueDate = $find(this.rdpValueDateId);
            this._rntbValueDouble = $find(this.rntbValueDoubleId);
            this._rdbGuid = $find(this.rdbGuidId);
            this.populateComboNames();
            this.resetValueVisibility();
            this.initializeEditOperation();
        };
        TbltWorkflowEvaluationPropertyGes.prototype.initializeEditOperation = function () {
            var _this = this;
            var qs = this.parse_query_string(window.location.search);
            var param = qs["?Action"];
            if (param === "Edit") {
                var propId = qs["WorkflowEvaluationPropertyId"];
                this._service.getWorkflowEvaluationProperty(propId, function (data) {
                    var valueName = _this._rcbName.findItemByValue(data.Name);
                    valueName.select();
                    _this._rcbName.disable();
                    _this.getFirstNonNullValue(data);
                }, function (exception) {
                    console.log(exception);
                });
            }
        };
        TbltWorkflowEvaluationPropertyGes.prototype.getFirstNonNullValue = function (model) {
            if (model.ValueBoolean != undefined || model.ValueBoolean != null) {
                var item = this._rlbValueBool.getItem(Number(model.ValueBoolean));
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
        };
        TbltWorkflowEvaluationPropertyGes.prototype.populateComboNames = function () {
            var cmbItem = null;
            var obj = this;
            var workflowEvaluationPropertyHelper = [];
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
        };
        TbltWorkflowEvaluationPropertyGes.prototype.resetValueVisibility = function () {
            this._rntbValueInt.set_visible(false);
            this._rtbValueString.set_visible(false);
            this._rtbValueJson.set_visible(false);
            $("#valueBool").hide();
            $("#valueDate").hide();
            this._rntbValueDouble.set_visible(false);
            this._rdbGuid.set_visible(false);
        };
        TbltWorkflowEvaluationPropertyGes.prototype.dynamicallyAdjustInputFields = function (vals) {
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
        };
        TbltWorkflowEvaluationPropertyGes.prototype.AddWorkflowEvaluationProperty = function (workflowRepositoryId) {
            var validModel = this.validateModel(workflowRepositoryId);
            this.insertWorkflowEvaluationProperty(validModel);
        };
        TbltWorkflowEvaluationPropertyGes.prototype.EditWorkflowEvaluationProperty = function (workflowRepositoryId, workflowEvaluationPropertyId) {
            var _this = this;
            var validModel = this.validateModel(workflowRepositoryId);
            validModel.UniqueId = workflowEvaluationPropertyId;
            this._service.updateWorkflowEvaluationProperty(validModel, function (data) {
                var operator = _this.getRadWindow();
                _this.closeWindow(operator);
            }, function (exception) {
                console.log(exception);
            });
        };
        TbltWorkflowEvaluationPropertyGes.prototype.validateModel = function (workflowRepositoryId) {
            var name = this._rcbName.get_selectedItem().get_value();
            var model = {
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
                WorkflowRepository: {}
            };
            var validModel = this.checkModelValidation(model);
            validModel.WorkflowRepository.UniqueId = workflowRepositoryId;
            return model;
        };
        TbltWorkflowEvaluationPropertyGes.prototype.insertWorkflowEvaluationProperty = function (model) {
            var _this = this;
            this._service.insertWorkflowEvaluationProperty(model, function (data) {
                var operator = _this.getRadWindow();
                _this.closeWindow(operator);
            }, function (exception) {
                console.log(exception);
            });
        };
        TbltWorkflowEvaluationPropertyGes.prototype.checkModelValidation = function (model) {
            if (this._rtbValueString.get_visible()) {
                var valueString = this._rtbValueString.get_textBoxValue();
                if (valueString == "") {
                    alert("Inserisci un valore");
                    return;
                }
                model.ValueString = valueString;
            }
            else if ($("#valueBool").is(":visible")) {
                var valueBool = this._rlbValueBool.get_selectedItem();
                if (valueBool == undefined || valueBool == null) {
                    alert("Inserisci un valore");
                    return;
                }
                model.ValueBoolean = valueBool.get_value() === "1";
            }
            else if (this._rntbValueInt.get_visible()) {
                var valueInt = this._rntbValueInt.get_value();
                if (valueInt === "") {
                    alert("Inserisci un valore");
                    return;
                }
                model.ValueInt = Number(valueInt);
            }
            else if (this._rtbValueJson.get_visible()) {
                var valueJson = this._rtbValueJson.get_textBoxValue();
                if (valueJson == "") {
                    alert("Inserisci un valore");
                    return;
                }
                model.ValueString = valueJson;
            }
            else if ($("#valueDate").is(":visible")) {
                var valueDate = this._rdpValueDate.get_selectedDate();
                if (valueDate == undefined || valueDate == null) {
                    alert("Inserisci un valore");
                    return;
                }
                model.ValueDate = valueDate;
            }
            else if (this._rntbValueDouble.get_visible()) {
                var valueDouble = this._rntbValueDouble.get_value();
                if (valueDouble === "") {
                    alert("Inserisci un valore");
                    return;
                }
                model.ValueDouble = Number(valueDouble);
            }
            else if (this._rdbGuid.get_visible()) {
                var valueGuid = this._rdbGuid.get_textBoxValue();
                if (valueGuid == "") {
                    alert("Inserisci un valore");
                    return;
                }
                model.ValueGuid = valueGuid;
            }
            return model;
        };
        TbltWorkflowEvaluationPropertyGes.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        TbltWorkflowEvaluationPropertyGes.prototype.closeWindow = function (operator) {
            var wnd = this.getRadWindow();
            wnd.close(operator);
        };
        TbltWorkflowEvaluationPropertyGes.prototype.parse_query_string = function (query) {
            var vars = query.split("&");
            var query_string = {};
            for (var i = 0; i < vars.length; i++) {
                var pair = vars[i].split("=");
                var key = decodeURIComponent(pair[0]);
                var value = decodeURIComponent(pair[1]);
                if (typeof query_string[key] === "undefined") {
                    query_string[key] = decodeURIComponent(value);
                }
                else if (typeof query_string[key] === "string") {
                    var arr = [query_string[key], decodeURIComponent(value)];
                    query_string[key] = arr;
                }
                else {
                    query_string[key].push(decodeURIComponent(value));
                }
            }
            return query_string;
        };
        return TbltWorkflowEvaluationPropertyGes;
    }());
    return TbltWorkflowEvaluationPropertyGes;
});
//# sourceMappingURL=TbltWorkflowEvaluationPropertyGes.js.map