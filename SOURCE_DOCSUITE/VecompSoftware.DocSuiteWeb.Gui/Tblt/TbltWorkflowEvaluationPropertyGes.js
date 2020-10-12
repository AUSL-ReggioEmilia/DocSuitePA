define(["require", "exports", "App/Models/Workflows/WorkflowType", "App/Models/Workflows/ArgumentType", "App/Models/Commons/UscRoleRestEventType", "App/Services/Workflows/WorkflowEvaluationPropertyService", "App/Services/Commons/RoleService", "App/Models/Workflows/WorkflowEvalutionPropertyHelper", "App/Helpers/ServiceConfigurationHelper", "App/Helpers/EnumHelper", "App/Models/Workflows/JsonModels/PropertyJsonValueSettori", "App/Models/Workflows/JsonModels/PropertyJsonValueContact", "App/Helpers/PageClassHelper", "App/Models/Workflows/QueryStringModels/QueryParametersWorkflowEvaluationProperty", "App/Models/Workflows/WorkflowPropertyHelper", "App/Services/Templates/TemplateCollaborationService", "App/Services/Handlers/UIHandlerEvalPropertyTemplateCollaboration", "App/Services/Handlers/UIHandlerEvalPropertyTemplateDeposito", "App/Models/Workflows/WorkflowAuthorizationType", "App/Models/Workflows/WorkflowDSWEnvironmentType"], function (require, exports, WorkflowType, ArgumentType, UscRoleRestEventType, WorkflowEvaluationPropertyService, RolesService, WorkflowEvalutionPropertyHelper, ServiceConfigurationHelper, EnumHelper, PropertyJsonValueSettori, PropertyJsonValueContact, PageClassHelper, QueryParameters, WorkflowPropertyHelper, TemplateCollaborationService, UIHandlerEvalPropertyTemplateCollaboration, UIHandlerEvalPropertyTemplateDeposito, WorkflowAuthorizationType, DSWEnvironmentType) {
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
                _this._templateCollabUIHandler.Commit();
                _this._collaborationSignSummaryUIHandler.Commit();
                _this._actionGenerateUIHandler.Commit();
                if (_this._rcbName.get_selectedItem().get_value() === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_DSW_WORKFLOW_STARTVALIDATIONS) {
                    PageClassHelper.callUserControlFunctionSafe(_this.uscWorkflowDesignerValidationsId).done(function (instance) {
                        _this._rtbValueJson.set_textBoxValue(instance.getWorkflowRulesModel(_this.workflowEnv));
                    });
                }
                if (_this.queryParameters.Action === "Add") {
                    _this.AddWorkflowEvaluationProperty(_this.queryParameters.WorkflowRepositoryId);
                }
                else if (_this.queryParameters.Action === "Edit") {
                    _this.EditWorkflowEvaluationProperty(_this.queryParameters.WorkflowRepositoryId, _this.queryParameters.WorkflowEvaluationPropertyId);
                }
                return false;
            };
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
            var templateServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "TemplateCollaboration");
            this._templateCollaborationService = new TemplateCollaborationService(templateServiceConfiguration);
        }
        ;
        TbltWorkflowEvaluationPropertyGes.prototype.initialize = function () {
            this.queryParameters = new QueryParameters();
            var serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltWorkflowEvaluationPropertyGes.CONFIGURATION_EVALUATION_PROPERTY);
            this._workflowEvaluationPropertyService = new WorkflowEvaluationPropertyService(serviceConfiguration);
            var rolesServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, TbltWorkflowEvaluationPropertyGes.CONFIGURATION_ROLE);
            this._rolesService = new RolesService(rolesServiceConfiguration);
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
            // combobox that shows a list of collaboration templates. Will render the value of the property  WorkflowPropertyHelper.DSW_PROPERTY_TEMPLATE_COLLABORATION_DEFAULT
            this._ddlTemplateCollaboration = $find(this.ddlTemplateCollaborationId);
            // handler for _ddlTemplateCollaboration , that will load templates and manages updateing the underlying rtbValue storage
            this._templateCollabUIHandler = new UIHandlerEvalPropertyTemplateCollaboration(this._rtbValueString, this._ddlTemplateCollaboration, this._templateCollaborationService);
            // combobox that shows a list of deposito documentale templates. 
            this._ddlTemplateCollaborationSignSummary = $find(this.ddlCollaborationSignSummaryId);
            // handler for _ddlTemplateCollaborationSignSummary 
            this._collaborationSignSummaryUIHandler = new UIHandlerEvalPropertyTemplateDeposito(this._rtbValueString, this._ddlTemplateCollaborationSignSummary, this._serviceConfigurations);
            // combobox that shows a list of deposito documentale templates. 
            this._ddlTemplateGenerate = $find(this.ddlActionGenerateId);
            // handler for _ddlTemplateGenerate 
            this._actionGenerateUIHandler = new UIHandlerEvalPropertyTemplateDeposito(this._rtbValueString, this._ddlTemplateGenerate, this._serviceConfigurations);
            this._uscRoleRestContainer = $("#" + this.uscRoleRestContainerId);
            this._uscContattiSelContainer = $("#" + this.uscDomainUserSelRestContainerId);
            this._uscTemplateCollaborationContainer = $("#" + this.uscTemplateCollaborationContainerId);
            this._uscCollaborationSignSummaryContainer = $("#" + this.uscCollaborationSignSummaryContainerId);
            this._uscActionGenerateContainer = $("#" + this.uscActionGenerateContainerId);
            this._uscWorkflowDesignerValidationsContainer = $("#" + this.uscWorkflowDesignerValidationsContainerId);
            this._uscWorkflowDesignerValidations = $("#" + this.uscWorkflowDesignerValidationsId).data();
            this.populateComboNames();
            this.resetValueVisibility();
            this.getQueryParameters(window.location.search);
            this.initializeEditOperation();
        };
        TbltWorkflowEvaluationPropertyGes.prototype.initializeEditOperation = function () {
            var _this = this;
            if (this.queryParameters.Action === "Edit") {
                this._workflowEvaluationPropertyService.getWorkflowEvaluationProperty(this.queryParameters.WorkflowEvaluationPropertyId, function (data) {
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
            var _this = this;
            /**
             * Specific Properties
             */
            //specific property: DEFAULT PROPOSER
            if (model.Name === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_WORKFLOW_DEFAULT_PROPOSER) {
                if (this.queryParameters.StartProposer === 0) {
                    this.WorkflowRestRoleRenderProperty(model);
                }
                else {
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
                }
                else {
                    //this.queryParameters.DestinatarioDiAvio === 1
                    this.WorkflowRestContactsRenderProperty(model);
                }
                //setting value in the json field to be used in model validation. Field is not visible
                this._rtbValueJson.set_value(model.ValueString);
                return;
            }
            if (model.Name === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_DSW_WORKFLOW_STARTVALIDATIONS) {
                //setting value in the json field to be used in model validation. Field is not visible
                PageClassHelper.callUserControlFunctionSafe(this.uscWorkflowDesignerValidationsId).done(function (instance) {
                    _this._rtbValueJson.set_textBoxValue(instance.getWorkflowRulesModel(_this.workflowEnv));
                    instance.createValidationTree((JSON.parse(model.ValueString))[_this.workflowEnv].Rules);
                });
                this._rtbValueJson.set_value(model.ValueString);
                return;
            }
            if (model.Name === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_TEMPLATE_COLLABORATION_DEFAULT) {
                this._templateCollabUIHandler.SetSelectedItem(model.ValueGuid);
                this._templateCollabUIHandler.UpdateSelection();
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
        TbltWorkflowEvaluationPropertyGes.prototype.ensureUscRoleRestEvents = function (instance) {
            var _this = this;
            instance.registerEventHandler(UscRoleRestEventType.RoleDeleted, function (roleId) {
                _this._rtbValueJson.set_value("");
                _this.WorkflowRestRoleRenderRoles([]);
                //solving manually
                return $.Deferred().reject();
            });
            instance.registerEventHandler(UscRoleRestEventType.NewRolesAdded, function (newAddedRoles) {
                var existedRole;
                var json = new PropertyJsonValueSettori();
                json.AuthorizationType = WorkflowAuthorizationType.AllRoleUser;
                json.Role = {
                    IdRole: newAddedRoles[newAddedRoles.length - 1].EntityShortId,
                    Name: newAddedRoles[newAddedRoles.length - 1].Name,
                    TenantId: newAddedRoles[newAddedRoles.length - 1].TenantId,
                    UniqueId: newAddedRoles[newAddedRoles.length - 1].UniqueId
                };
                _this._rtbValueJson.set_value(JSON.stringify(json));
                return $.Deferred().resolve(existedRole);
            });
        };
        /**
         * The method will always ensure that the events are registered.
         * The initial logic for using this method implied calling it from the constructor, but it does not always work
         * because sometimes code reaches using the instance faster then the rest component is loaded
         * @param usc
         */
        TbltWorkflowEvaluationPropertyGes.prototype.ensureUscContattiSelRestEvents = function (usc) {
            var _this = this;
            usc.registerEventHandlerContactsDeleted(function (data) {
                _this._rtbValueJson.set_value("");
                //manually setting the rendering for an empty array. Rejecting default action of usc rest component
                _this.WorkflowRestContactsRenderContacts([]);
                //we rendered manually. Prevent rest component to render:
                return $.Deferred().reject();
            });
            usc.registerEventHandlerContactsAdded(function (data) {
                var json = new PropertyJsonValueContact();
                if (data.length === 0) {
                    _this._rtbValueJson.set_value("");
                    _this.WorkflowRestContactsRenderContacts([]);
                }
                else {
                    //the rest control returns only one element(in an array...)
                    var newAddedContact = data[0];
                    json.AuthorizationType = WorkflowAuthorizationType.UserName;
                    //see uscStartWorkflow.ts/startWorkflow
                    json.Account = {
                        AccountName: newAddedContact.Code,
                        DisplayName: newAddedContact.Description,
                        Required: true,
                        EmailAddress: newAddedContact.EmailAddress
                    };
                    _this._rtbValueJson.set_value(JSON.stringify(json));
                    //manually rendering the saved json in the rest component. We will reject the default action
                    _this.WorkflowRestContactsRenderJsonModel(json);
                }
                //we rendered manually. Prevent rest component to render:
                return $.Deferred().reject();
            });
        };
        //#region WorkflowRestRole
        TbltWorkflowEvaluationPropertyGes.prototype.WorkflowRestRoleRenderRoles = function (model) {
            var _this = this;
            PageClassHelper.callUserControlFunctionSafe(this.uscRoleRestId)
                .done(function (instance) {
                _this.ensureUscRoleRestEvents(instance);
                instance.renderRolesTree(model);
            });
        };
        TbltWorkflowEvaluationPropertyGes.prototype.WorkflowRestRoleRenderProperty = function (model) {
            if (model.ValueString === "" || model.ValueString === undefined || model.ValueString === null) {
                this.WorkflowRestRoleRenderRoles([]);
            }
            else {
                var proposerModel = JSON.parse(model.ValueString);
                this.WorkflowRestRoleRenderJsonModel(proposerModel);
            }
        };
        TbltWorkflowEvaluationPropertyGes.prototype.WorkflowRestRoleRenderJsonModel = function (model) {
            var _this = this;
            this._rolesService.findRoles({
                LoadAlsoParent: true,
                UniqueId: model.Role.UniqueId
            }, function (data) {
                if (data === null || data === undefined) {
                    _this.WorkflowRestRoleRenderRoles([]);
                }
                else {
                    _this.WorkflowRestRoleRenderRoles(data);
                }
            }, function (exception) {
                console.log(exception);
            });
        };
        //#endregion
        //#region WorkflowRestContacts
        TbltWorkflowEvaluationPropertyGes.prototype.WorkflowRestContactsRenderContacts = function (model) {
            var _this = this;
            PageClassHelper.callUserControlFunctionSafe(this.uscDomainUserSelRestId)
                .done(function (instance) {
                _this.ensureUscContattiSelRestEvents(instance);
                instance.createDomainUsersContactsTree(model);
            });
        };
        TbltWorkflowEvaluationPropertyGes.prototype.WorkflowRestContactsRenderProperty = function (model) {
            if (model.ValueString === "" || model.ValueString === undefined || model.ValueString === null) {
                this.WorkflowRestContactsRenderContacts([]);
            }
            else {
                var jsonModel = JSON.parse(model.ValueString);
                this.WorkflowRestContactsRenderJsonModel(jsonModel);
            }
        };
        TbltWorkflowEvaluationPropertyGes.prototype.WorkflowRestContactsRenderJsonModel = function (model) {
            this.WorkflowRestContactsRenderContacts([
                {
                    EmailAddress: model.Account.EmailAddress,
                    Code: model.Account.AccountName,
                    Description: (model.Account.DisplayName !== null
                        && model.Account.DisplayName !== undefined
                        && model.Account.DisplayName !== "|")
                        ? model.Account.DisplayName
                        : model.Account.EmailAddress
                }
            ]);
        };
        //#endregion
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
            this._uscRoleRestContainer.hide();
            this._uscWorkflowDesignerValidationsContainer.hide();
            this._uscContattiSelContainer.hide();
            this._uscTemplateCollaborationContainer.hide();
            this._uscCollaborationSignSummaryContainer.hide();
            this._uscActionGenerateContainer.hide();
        };
        TbltWorkflowEvaluationPropertyGes.prototype.dynamicallyAdjustInputFields = function (propertyFieldName) {
            /*
             * specific properties
             */
            if (propertyFieldName === TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_WORKFLOW_DEFAULT_PROPOSER
                && this.queryParameters.StartProposer === 0) {
                // The main storage remains the rbtValueJson which stores the serialized data
                // The rest component will replace the default view by taking the data and rendering it
                this.WorkflowRestRoleRenderRoles([]);
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
                this.WorkflowRestRoleRenderRoles([]);
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
                this._templateCollabUIHandler.LoadTemplateCollaborations();
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
                    PageClassHelper.callUserControlFunctionSafe(this.uscWorkflowDesignerValidationsId).done(function (instance) {
                        instance.displayDisableEnvironmentMessage();
                    });
                }
                return;
            }
            /**
             * general properties
             **/
            var prop = WorkflowEvalutionPropertyHelper[propertyFieldName];
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
        };
        TbltWorkflowEvaluationPropertyGes.prototype.AddWorkflowEvaluationProperty = function (workflowRepositoryId) {
            var validModel = this.validateModel(workflowRepositoryId);
            this.insertWorkflowEvaluationProperty(validModel);
        };
        TbltWorkflowEvaluationPropertyGes.prototype.EditWorkflowEvaluationProperty = function (workflowRepositoryId, workflowEvaluationPropertyId) {
            var _this = this;
            var validModel = this.validateModel(workflowRepositoryId);
            validModel.UniqueId = workflowEvaluationPropertyId;
            this._workflowEvaluationPropertyService.updateWorkflowEvaluationProperty(validModel, function (data) {
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
            this._workflowEvaluationPropertyService.insertWorkflowEvaluationProperty(model, function (data) {
                var operator = _this.getRadWindow();
                _this.closeWindow(operator);
            }, function (exception) {
                console.log(exception);
            });
        };
        TbltWorkflowEvaluationPropertyGes.prototype.checkModelValidation = function (model) {
            /*
             *Specific linked containers
             */
            if (this._uscRoleRestContainer.is(":visible")) {
                var valueString = this._rtbValueJson.get_textBoxValue();
                if (valueString === "") {
                    alert("Inserisci un valore");
                    return;
                }
                model.ValueString = valueString;
                return model;
            }
            if (this._uscWorkflowDesignerValidationsContainer.is(":visible")) {
                var valueString = this._rtbValueJson.get_textBoxValue();
                if (valueString === "") {
                    alert("Inserisci un valore");
                    return;
                }
                model.ValueString = valueString;
                return model;
            }
            if (this._uscContattiSelContainer.is(":visible")) {
                var valueString = this._rtbValueJson.get_textBoxValue();
                if (valueString === "") {
                    alert("Inserisci un valore");
                    return;
                }
                model.ValueString = valueString;
                return model;
            }
            if (this._uscTemplateCollaborationContainer.is(":visible")) {
                var valueGuid = this._rtbValueString.get_textBoxValue();
                if (valueGuid === "") {
                    alert("Inserisci un valore");
                    return;
                }
                model.ValueGuid = valueGuid;
                return model;
            }
            if (this._uscCollaborationSignSummaryContainer.is(":visible")) {
                var valueString = this._rtbValueString.get_textBoxValue();
                if (valueString === "") {
                    alert("Inserisci un valore");
                    return;
                }
                model.ValueString = valueString;
                return model;
            }
            if (this._uscActionGenerateContainer.is(":visible")) {
                var valueGuid = this._rtbValueString.get_textBoxValue();
                if (valueGuid === "") {
                    alert("Inserisci un valore");
                    return;
                }
                model.ValueGuid = valueGuid;
                return model;
            }
            //general containers
            if (this._rtbValueString.get_visible()) {
                var valueString = this._rtbValueString.get_textBoxValue();
                if (valueString === "") {
                    alert("Inserisci un valore");
                    return;
                }
                model.ValueString = valueString;
            }
            else if ($("#valueBool").is(":visible")) {
                var valueBool = this._rlbValueBool.get_selectedItem();
                if (valueBool === undefined || valueBool == null) {
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
                if (valueJson === "") {
                    alert("Inserisci un valore");
                    return;
                }
                model.ValueString = valueJson;
            }
            else if ($("#valueDate").is(":visible")) {
                var valueDate = this._rdpValueDate.get_selectedDate();
                if (valueDate === undefined || valueDate === null) {
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
                if (valueGuid === "") {
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
        TbltWorkflowEvaluationPropertyGes.prototype.getQueryParameters = function (query) {
            //removing ? and getting string on the right side
            if (query.indexOf('?') > -1) {
                query = query.split('?')[1];
            }
            var queryPairCollection = query.split("&");
            var queryString = {};
            //decompose raw query string
            for (var i = 0; i < queryPairCollection.length; i++) {
                var queryPair = queryPairCollection[i].split("=");
                var key = decodeURIComponent(queryPair[0]);
                var value = decodeURIComponent(queryPair[1]);
                queryString[key] = decodeURIComponent(value);
            }
            //populate typed query parameters
            for (var _i = 0, _a = Object.keys(queryString); _i < _a.length; _i++) {
                var key = _a[_i];
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
        };
        TbltWorkflowEvaluationPropertyGes.CONFIGURATION_ROLE = "Role";
        TbltWorkflowEvaluationPropertyGes.CONFIGURATION_EVALUATION_PROPERTY = "WorkflowEvaluationProperty";
        // _dsw_p_WorkflowDefaultProposer
        TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_WORKFLOW_DEFAULT_PROPOSER = WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_DEFAULT;
        // _dsw_p_WorkflowDefaultRecipient
        TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_WORKFLOW_DEFAULT_RECIPIENT = WorkflowPropertyHelper.DSW_PROPERTY_RECIPIENT_DEFAULT;
        // _dsw_p_WorkflowDefaultTemplateCollaboration
        TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_TEMPLATE_COLLABORATION_DEFAULT = WorkflowPropertyHelper.DSW_PROPERTY_TEMPLATE_COLLABORATION_DEFAULT;
        // _dsw_p_CollaborationSignSummaryTemplateId
        TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_COLLABORATION_SIGN_SUMMARY = WorkflowPropertyHelper.DSW_PROPERTY_COLLABORATION_SIGN_SUMMARY_TEMPLATE_ID;
        // _dsw_a_Generate_TemplateId
        TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_ACTION_GENERATE = WorkflowPropertyHelper.DSW_ACTION_GENERATE_TEMPLATE_ID;
        TbltWorkflowEvaluationPropertyGes.PROPERTY_NAME_DSW_WORKFLOW_STARTVALIDATIONS = WorkflowPropertyHelper.DSW_VALIDATION_WORKFLOW_START;
        return TbltWorkflowEvaluationPropertyGes;
    }());
    return TbltWorkflowEvaluationPropertyGes;
});
//# sourceMappingURL=TbltWorkflowEvaluationPropertyGes.js.map