/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />
var __spreadArrays = (this && this.__spreadArrays) || function () {
    for (var s = 0, i = 0, il = arguments.length; i < il; i++) s += arguments[i].length;
    for (var r = Array(s), k = 0, i = 0; i < il; i++)
        for (var a = arguments[i], j = 0, jl = a.length; j < jl; j++, k++)
            r[k] = a[j];
    return r;
};
define(["require", "exports", "App/Models/Workflows/WorkflowEvalutionPropertyHelper", "App/Services/Workflows/WorkflowRoleMappingService", "App/Services/Workflows/WorkflowRepositoryService", "App/Services/Securities/DomainUserService", "App/Helpers/ServiceConfigurationHelper", "App/Models/Workflows/WorkflowAuthorizationType", "App/DTOs/ExceptionDTO", "App/Services/Workflows/WorkflowEvaluationPropertyService", "App/Models/Workflows/ArgumentType", "App/Models/Workflows/WorkflowTreeNodeType", "App/Models/Workflows/OpenWindowOperationType", "App/Models/Workflows/WorkflowArgumentType", "App/Models/Workflows/WorkflowDSWEnvironmentType", "App/Mappers/Commons/RoleModelMapper", "App/Models/Workflows/WorkflowRepositoryStatus", "App/Helpers/EnumHelper", "App/Models/Workflows/ActivityType", "App/Models/Workflows/ActivityArea", "App/Models/Workflows/QueryStringModels/QueryParametersWorkflowEvaluationProperty", "App/Models/Workflows/WorkflowPropertyHelper", "App/Helpers/SessionStorageKeysHelper", "../app/core/extensions/string"], function (require, exports, WorkflowEvalutionPropertyHelper, WorkflowRoleMappingService, WorkflowRepositoryService, DomainUserService, ServiceConfigurationHelper, WorkflowAuthorizationType, ExceptionDTO, WorkflowEvaluationPropertyService, ArgumentType, WorkflowTreeNodeType, OpenWindowOperationType, WorkflowArgumentType, DSWEnvironmentType, RoleModelMapper, WorkflowRepositoryStatus, EnumHelper, ActivityType, ActivityArea, QueryParameters, WorkflowPropertyHelper, SessionStorageKeysHelper) {
    var TbltWorkflowRepository = /** @class */ (function () {
        /**
         * Costruttore
         * @param serviceConfigurations
         */
        function TbltWorkflowRepository(serviceConfigurations) {
            var _this_1 = this;
            /**
             *------------------------- Events -----------------------------
             */
            /**
             * Evento scatenato alla selezione di un workflow
             * @param sender
             * @param args
             */
            this.rtvWorkflowRepository_nodeClicked = function (sender, args) {
                $('#'.concat(_this_1.pnlDetailsId)).show();
                $('#'.concat(_this_1.btnModificaId)).show();
                $('#'.concat(_this_1.btnEliminaId)).show();
                var clickedNode = _this_1._rtvWorkflowRepository.get_selectedNode();
                if (!clickedNode) {
                    return;
                }
                var nodeType = clickedNode.get_attributes().getAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME);
                if (nodeType === WorkflowTreeNodeType.Workflow) {
                    _this_1.registerUscRoleRestEventHandlers();
                    _this_1._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_ADD).set_enabled(true);
                    _this_1._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_ADD).set_toolTip("Aggiungi workflow step");
                    _this_1._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_EDIT).set_enabled(true);
                    _this_1._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_EDIT).set_toolTip("Modifica workflow repository");
                    _this_1._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_REMOVE).set_enabled(false);
                    $('#'.concat(_this_1.pnlRepositoryInformationsId)).show();
                    $('#'.concat(_this_1.pnlStepInformationsId)).hide();
                    $('#'.concat(_this_1.pnlSelectRolesId)).show();
                    _this_1._pnlBarDetails.findItemByValue(TbltWorkflowRepository.TAG_PANEL).show();
                    _this_1._pnlBarDetails.findItemByValue(TbltWorkflowRepository.STARTUP_PANEL).show();
                    _this_1._pnlBarDetails.findItemByValue(TbltWorkflowRepository.INPUT_ARG_PANEL).hide();
                    _this_1._pnlBarDetails.findItemByValue(TbltWorkflowRepository.EVALUATION_ARG_PANEL).hide();
                    _this_1._pnlBarDetails.findItemByValue(TbltWorkflowRepository.OUTPUT_ARG_PANEL).hide();
                    _this_1._currentWorkflowRepositoryId = clickedNode.get_value();
                    _this_1._workflowRepositoryService.getWorkflowRepositoryRoles(_this_1._currentWorkflowRepositoryId, function (data) {
                        _this_1._uscRoleRest.renderRolesTree([]);
                        if (data.length > 0) {
                            var roleModelMapper = new RoleModelMapper();
                            var roles = roleModelMapper.MapCollection(data);
                            _this_1._uscRoleRest.renderRolesTree(roles);
                        }
                    });
                }
                else if (nodeType === WorkflowTreeNodeType.Step) {
                    _this_1._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_ADD).set_enabled(false);
                    _this_1._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_EDIT).set_enabled(true);
                    _this_1._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_REMOVE).set_enabled(true);
                    $('#'.concat(_this_1.pnlRepositoryInformationsId)).hide();
                    $('#'.concat(_this_1.pnlStepInformationsId)).show();
                    $('#'.concat(_this_1.pnlSelectRolesId)).hide();
                    _this_1._pnlBarDetails.findItemByValue(TbltWorkflowRepository.TAG_PANEL).hide();
                    _this_1._pnlBarDetails.findItemByValue(TbltWorkflowRepository.STARTUP_PANEL).hide();
                    _this_1._pnlBarDetails.findItemByValue(TbltWorkflowRepository.INPUT_ARG_PANEL).show();
                    _this_1._pnlBarDetails.findItemByValue(TbltWorkflowRepository.EVALUATION_ARG_PANEL).show();
                    _this_1._pnlBarDetails.findItemByValue(TbltWorkflowRepository.OUTPUT_ARG_PANEL).show();
                    _this_1._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_EDIT).set_toolTip("Modifica workflow step");
                    _this_1._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_REMOVE).set_toolTip("Elimina workflow step");
                }
                else {
                    _this_1._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_ADD).set_enabled(true);
                    _this_1._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_ADD).set_toolTip("Aggiungi workflow repository");
                    _this_1._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_EDIT).set_enabled(false);
                    _this_1._toolbarStep.findButtonByCommandName(TbltWorkflowRepository.COMMANDNAME_REMOVE).set_enabled(false);
                    $('#'.concat(_this_1.pnlRepositoryInformationsId)).hide();
                    $('#'.concat(_this_1.pnlStepInformationsId)).hide();
                    $('#'.concat(_this_1.pnlDetailsId)).hide();
                    $('#'.concat(_this_1.pnlSelectRolesId)).hide();
                }
                if (nodeType != WorkflowTreeNodeType.Root) {
                    _this_1.loadDetails();
                }
            };
            /**
             * Evento scatenato al click del pulsante aggiungi
             * @param sender
             * @param args
             */
            this.btnAggiungi_onClick = function (sender, args) {
                _this_1.openManagementWindow('Add');
                return false;
            };
            /**
             * Evento scatenato al click del pulsante modifica
             * @param sender
             * @param args
             */
            this.btnModifica_onClick = function (sender, args) {
                _this_1.openManagementWindow('Edit');
                return false;
            };
            /**
             * Evento scatenato al click del pulsante elimina
             * @param sender
             * @param args
             */
            this.btnDelete_onClick = function (sender, args) {
                var selectedNode = _this_1._rtvWorkflowRepository.get_selectedNode();
                var isXamlRepository = selectedNode.get_attributes().getAttribute('isXaml');
                var selectedMappingTags;
                if (isXamlRepository) {
                    selectedMappingTags = _this_1._rgvXamlWorkflowRoleMappings.get_selectedItems();
                }
                else {
                    selectedMappingTags = _this_1._rgvWorkflowRoleMappings.get_selectedItems();
                }
                if (selectedMappingTags == undefined || selectedMappingTags.length == 0) {
                    _this_1.showWarningMessage(_this_1.uscNotificationId, 'Nessun Tag selezionato');
                    return false;
                }
                var model;
                if (isXamlRepository) {
                    var workflowActivityModel = selectedMappingTags[0].get_dataItem();
                    if (workflowActivityModel.WorkflowRoleMapping.UniqueId == undefined) {
                        _this_1.showNotificationMessage(_this_1.uscNotificationId, "Nessuna definizione di autorizzazione associata all'activity selezionata.");
                        return;
                    }
                    model = { UniqueId: workflowActivityModel.WorkflowRoleMapping.UniqueId };
                }
                else {
                    model = { UniqueId: selectedMappingTags[0].get_dataItem().UniqueId };
                }
                _this_1._workflowRoleMappingService.deleteWorkflowRoleMapping(model, function (data) { return _this_1.loadDetails(); }, function (exception) {
                    _this_1.showNotificationException(_this_1.uscNotificationId, exception);
                    return;
                });
            };
            /**
             * Evento scatenato al click del pulsante di ricerca workflow
             * @param sender
             * @param args
             */
            this.toolbarSearch_onClick = function (sender, args) {
                _this_1.searchWorkflows();
                return false;
            };
            this.toolbarStep_onClick = function (sender, args) {
                var selectedNode = _this_1._rtvWorkflowRepository.get_selectedNode();
                var nodeType = selectedNode.get_attributes().getAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME);
                var btn = args.get_item();
                switch (btn.get_index()) {
                    case 0:
                        if (nodeType === WorkflowTreeNodeType.Root) {
                            _this_1.openWorkflowRepositoryGesWindow(OpenWindowOperationType.Add);
                        }
                        else {
                            _this_1.openWorkflowStepGesWindow(OpenWindowOperationType.Add);
                        }
                        break;
                    case 1:
                        if (nodeType === WorkflowTreeNodeType.Workflow) {
                            _this_1.openWorkflowRepositoryGesWindow(OpenWindowOperationType.Edit);
                        }
                        else {
                            _this_1.openWorkflowStepGesWindow(OpenWindowOperationType.Edit);
                        }
                        break;
                    case 2:
                        var selectedNodeParent_1 = selectedNode.get_parent();
                        _this_1._confirmWindowManager.set_cssClass("remove");
                        _this_1._confirmWindowManager.radconfirm("Sei sicuro di voler eliminare il workflow step dal repository selezionato?", function (arg) {
                            if (arg) {
                                var workflowSteps = _this_1.deleteWorkflowRepositoryStep(selectedNode.get_index(), selectedNodeParent_1.get_value());
                                _this_1.redrawNodeWithPositions(selectedNodeParent_1, selectedNode, workflowSteps);
                            }
                        });
                        $('#'.concat(_this_1.pnlDetailsId)).hide();
                        break;
                }
            };
            this._rwAddWorkflowStep_onClose = function (sender, args) {
                if (!args.get_argument()) {
                    return;
                }
                var workflowStep = args.get_argument()[0];
                var uniqueId = args.get_argument()[1];
                _this_1.updateWorkflowRepositoryStep(workflowStep, uniqueId);
            };
            this._rwWorkflowProperty_onClose = function (sender, args) {
                if (!args.get_argument()) {
                    return;
                }
                var argObj = args.get_argument();
                var workflowArgument = argObj.WorkflowArgument;
                var uniqueId = argObj.UniqueId;
                var stepPosition = argObj.StepPosition;
                var argumentType = argObj.ArgumentType;
                _this_1.updateWorkflowStepArgument(workflowArgument, uniqueId, stepPosition, WorkflowArgumentType[argumentType]);
            };
            this._rwWorkflowRepository_onClose = function (sender, args) {
                if (!args.get_argument()) {
                    return;
                }
                var argObj = args.get_argument();
                var workflowRepository = argObj.WorkflowRepository;
                var action = argObj.Action;
                _this_1.updateWorkflowRepository(workflowRepository, OpenWindowOperationType[action]);
            };
            this.btnAddInputArguments_onClick = function (sender, args) {
                _this_1.openWorkflowPropertyGesWindow(OpenWindowOperationType.Add, WorkflowArgumentType.Input);
            };
            this.btnEditInputArguments_onClick = function (sender, args) {
                _this_1.openWorkflowPropertyGesWindow(OpenWindowOperationType.Edit, WorkflowArgumentType.Input);
            };
            this.btnDeleteInputArguments_onClick = function (sender, args) {
                var selectedNode = _this_1._rtvWorkflowRepository.get_selectedNode();
                var selectedItem = _this_1._rgvStepInputProperties.get_selectedItems()[0];
                var argumentName = selectedItem.get_dataItem().Name;
                var stepPosition = selectedNode.get_value();
                var uniqueId = selectedNode.get_parent().get_value();
                var argumentType = WorkflowArgumentType.Input;
                _this_1.deleteWorkflowStepArgument(argumentName, uniqueId, stepPosition, argumentType);
            };
            this.btnAddEvaluationArguments_onClick = function (sender, args) {
                _this_1.openWorkflowPropertyGesWindow(OpenWindowOperationType.Add, WorkflowArgumentType.Evaluation);
            };
            this.btnEditEvaluationArguments_onClick = function (sender, args) {
                _this_1.openWorkflowPropertyGesWindow(OpenWindowOperationType.Edit, WorkflowArgumentType.Evaluation);
            };
            this.btnDeleteEvaluationArguments_onClick = function (sender, args) {
                var selectedNode = _this_1._rtvWorkflowRepository.get_selectedNode();
                var selectedItem = _this_1._rgvStepEvaluationProperties.get_selectedItems()[0];
                var argumentName = selectedItem.get_dataItem().Name;
                var stepPosition = selectedNode.get_value();
                var uniqueId = selectedNode.get_parent().get_value();
                var argumentType = WorkflowArgumentType.Evaluation;
                _this_1.deleteWorkflowStepArgument(argumentName, uniqueId, stepPosition, argumentType);
            };
            this.btnAddOutputArguments_onClick = function (sender, args) {
                _this_1.openWorkflowPropertyGesWindow(OpenWindowOperationType.Add, WorkflowArgumentType.Output);
            };
            this.btnEditOutputArguments_onClick = function (sender, args) {
                _this_1.openWorkflowPropertyGesWindow(OpenWindowOperationType.Edit, WorkflowArgumentType.Output);
            };
            this.btnDeleteOutputArguments_onClick = function (sender, args) {
                var selectedNode = _this_1._rtvWorkflowRepository.get_selectedNode();
                var selectedItem = _this_1._rgvStepOutputProperties.get_selectedItems()[0];
                var argumentName = selectedItem.get_dataItem().Name;
                var stepPosition = selectedNode.get_value();
                var uniqueId = selectedNode.get_parent().get_value();
                var argumentType = WorkflowArgumentType.Output;
                _this_1.deleteWorkflowStepArgument(argumentName, uniqueId, stepPosition, argumentType);
            };
            /**
             * Evento scatenato al click del pulsante di ricerca Tag per workflow di tipo XAML
             * @param sender
             * @param args
             */
            this.btnSelectMappingTag_onClick = function (sender, args) {
                var selectedMappingText = _this_1._rcbSelectMappingTag.get_text();
                if (String.isNullOrEmpty(selectedMappingText) || selectedMappingText == _this_1._rcbSelectMappingTag.get_emptyMessage())
                    return;
                var selectedNode = _this_1._rtvWorkflowRepository.get_selectedNode();
                _this_1._workflowRepositoryService.getById(selectedNode.get_value(), function (response) {
                    if (response == undefined)
                        return;
                    try {
                        var model = response;
                        var customActivities = JSON.parse(model.CustomActivities);
                        if (customActivities.length == 0) {
                            _this_1.showWarningMessage(_this_1.uscNotificationId, 'Nessuna activity configurata per il Workflow selezionato.');
                            return;
                        }
                        _this_1.fillXamlMappings(model, customActivities, selectedMappingText);
                    }
                    catch (error) {
                        _this_1.showNotificationMessage(_this_1.uscNotificationId, error.message);
                        console.log(JSON.stringify(error));
                    }
                }, function (exception) {
                    _this_1.showNotificationException(_this_1.uscNotificationId, exception);
                });
            };
            /**
             * Evento scatenato alla chiusura della finestra di gestione mapping
             * @param sender
             * @param args
             */
            this.windowAddWorkflowRoleMapping_onClose = function (sender, args) {
                if (args.get_argument() != undefined) {
                    _this_1.loadDetails();
                }
            };
            this.deleteWorkflowRolePromise = function (roleIdToDelete, instanceId) {
                var promise = $.Deferred();
                if (!roleIdToDelete) {
                    return promise.promise();
                }
                var selectedNode = _this_1._rtvWorkflowRepository.get_selectedNode();
                var currentWorkflowRepository = _this_1.repositories.filter(function (r) { return r.UniqueId == selectedNode.get_value(); })[0];
                currentWorkflowRepository.Roles = currentWorkflowRepository.Roles.filter(function (r) { return r.EntityShortId !== roleIdToDelete; });
                _this_1._workflowRepositoryService.updateWorkflowRepository(currentWorkflowRepository, function (data) {
                    promise.resolve(data);
                });
                return promise.promise();
            };
            this.addWorkflowRolesPromise = function (rolesToAdd, instanceId) {
                var promise = $.Deferred();
                if (!rolesToAdd.length) {
                    return promise.promise();
                }
                var selectedNode = _this_1._rtvWorkflowRepository.get_selectedNode();
                var currentWorkflowRepository = _this_1.repositories.filter(function (r) { return r.UniqueId == selectedNode.get_value(); })[0];
                currentWorkflowRepository.Roles = __spreadArrays(currentWorkflowRepository.Roles, rolesToAdd);
                _this_1._workflowRepositoryService.updateWorkflowRepository(currentWorkflowRepository, function (data) {
                    promise.resolve(data);
                }, function (exception) {
                    _this_1.showNotificationException(_this_1.uscNotificationId, exception);
                });
                return promise.promise();
            };
            this.btnAdd_onClick = function (sender, args) {
                _this_1.openWorkflowEvaluationPropertiesManagementWindow("Add");
            };
            this.btnEdit_onClick = function (sender, args) {
                _this_1.openWorkflowEvaluationPropertiesManagementWindow("Edit");
            };
            this.btnDeleteStartup_onClick = function (sender, args) {
                var selectedEvaluationPropertyId = _this_1._rgvWorkflowStartUp.get_selectedItems();
                if (selectedEvaluationPropertyId == undefined || selectedEvaluationPropertyId.length == 0) {
                    _this_1.showWarningMessage(_this_1.uscNotificationId, 'Nessun proprietà selezionata');
                    return;
                }
                if (selectedEvaluationPropertyId.length > 1) {
                    _this_1.showWarningMessage(_this_1.uscNotificationId, 'Seleziona una sola proprietà per la modifica');
                    return;
                }
                var workflowEvaluationModel;
                var workflowEvaluationModelUniqueId = selectedEvaluationPropertyId[0].get_dataItem().UniqueId;
                workflowEvaluationModel = { UniqueId: workflowEvaluationModelUniqueId };
                _this_1._workflowEvaluationPropertyService.deleteWorkflowEvaluationProperty(workflowEvaluationModel, function (data) {
                    _this_1.loadDetails();
                }, function (exception) {
                    console.log(exception);
                });
            };
            this._serviceConfigurations = serviceConfigurations;
            this._enumHelper = new EnumHelper();
        }
        /**
         * Inizializzazione classe
         */
        TbltWorkflowRepository.prototype.initialize = function () {
            $('#'.concat(this.pnlDetailsId)).hide();
            $('#'.concat(this.btnAggiungiId)).hide();
            $('#'.concat(this.btnModificaId)).hide();
            $('#'.concat(this.btnEliminaId)).hide();
            this._uscRoleRest = $("#" + this.uscRoleRestId).data();
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._confirmWindowManager = $find(this.radWindowManagerId);
            this._windowAddWorkflowRoleMapping = $find(this.windowAddWorkflowRoleMappingId);
            this._windowAddWorkflowRoleMapping.add_close(this.windowAddWorkflowRoleMapping_onClose);
            this._rtvWorkflowRepository = $find(this.rtvWorkflowRepositoryId);
            this._rtvWorkflowRepository.add_nodeClicked(this.rtvWorkflowRepository_nodeClicked);
            this._rgvWorkflowRoleMappings = $find(this.rgvWorkflowRoleMappingsId);
            this._rgvXamlWorkflowRoleMappings = $find(this.rgvXamlWorkflowRoleMappingsId);
            this._rgvStepInputProperties = $find(this.rgvStepInputPropertiesId);
            this._rgvStepEvaluationProperties = $find(this.rgvStepEvaluationPropertiesId);
            this._rgvStepOutputProperties = $find(this.rgvStepOutputPropertiesId);
            this._rcbSelectMappingTag = $find(this.rcbSelectMappingTagId);
            this._mappingDataSource = $find(this.mappingDataSourceId);
            this._btnAggiungi = $find(this.btnAggiungiId);
            this._btnAggiungi.add_clicked(this.btnAggiungi_onClick);
            this._btnModifica = $find(this.btnModificaId);
            this._btnModifica.add_clicked(this.btnModifica_onClick);
            this._btnElimina = $find(this.btnEliminaId);
            this._btnElimina.add_clicked(this.btnDelete_onClick);
            this._btnSelectMappingTag = $find(this.btnSelectMappingTagId);
            this._btnSelectMappingTag.add_clicked(this.btnSelectMappingTag_onClick);
            this._btnAdd = $find(this.btnAddId);
            this._btnAdd.add_clicked(this.btnAdd_onClick);
            this._btnEdit = $find(this.btnEditId);
            this._btnEdit.add_clicked(this.btnEdit_onClick);
            this._btnDelete = $find(this.btnDeleteId);
            this._btnDelete.add_clicked(this.btnDeleteStartup_onClick);
            this._rgvWorkflowStartUp = $find(this.rgvWorkflowStartUpId);
            this._btnAddInputArguments = $find(this.btnAddInputArgumentId);
            this._btnAddInputArguments.add_clicked(this.btnAddInputArguments_onClick);
            this._btnEditInputArguments = $find(this.btnEditInputArgumentId);
            this._btnEditInputArguments.add_clicked(this.btnEditInputArguments_onClick);
            this._btnDeleteInputArguments = $find(this.btnDeleteInputArgumentId);
            this._btnDeleteInputArguments.add_clicked(this.btnDeleteInputArguments_onClick);
            this._btnAddEvaluationArguments = $find(this.btnAddEvaluationArgumentId);
            this._btnAddEvaluationArguments.add_clicked(this.btnAddEvaluationArguments_onClick);
            this._btnEditEvaluationArguments = $find(this.btnEditEvaluationArgumentId);
            this._btnEditEvaluationArguments.add_clicked(this.btnEditEvaluationArguments_onClick);
            this._btnDeleteEvaluationArguments = $find(this.btnDeleteEvaluationArgumentId);
            this._btnDeleteEvaluationArguments.add_clicked(this.btnDeleteEvaluationArguments_onClick);
            this._btnAddOutputArguments = $find(this.btnAddOutputArgumentId);
            this._btnAddOutputArguments.add_clicked(this.btnAddOutputArguments_onClick);
            this._btnEditOutputArguments = $find(this.btnEditOutputArgumentId);
            this._btnEditOutputArguments.add_clicked(this.btnEditOutputArguments_onClick);
            this._btnDeleteOutputArguments = $find(this.btnDeleteOutputArgumentId);
            this._btnDeleteOutputArguments.add_clicked(this.btnDeleteOutputArguments_onClick);
            this._toolbarSearch = $find(this.ToolBarSearchId);
            this._toolbarSearch.add_buttonClicking(this.toolbarSearch_onClick);
            this._toolbarItemSearchName = this._toolbarSearch.findItemByValue("searchName");
            this._txtSearchName = this._toolbarItemSearchName.findControl("txtName");
            this._toolbarItemButtonSearch = this._toolbarSearch.findItemByValue("searchCommand");
            this._toolbarStep = $find(this.ToolBarStepId);
            this._toolbarStep.add_buttonClicking(this.toolbarStep_onClick);
            this._rwmWorkflowStep = $find(this.rwmWorkflowStepId);
            this._rwmWorkflowStep.add_close(this._rwAddWorkflowStep_onClose);
            this._rwWorkflowProperty = $find(this.rwWorkflowPropertyId);
            this._rwWorkflowProperty.add_close(this._rwWorkflowProperty_onClose);
            this._rwWorkflowRepository = $find(this.rwWorkflowRepositoryId);
            this._rwWorkflowRepository.add_close(this._rwWorkflowRepository_onClose);
            this._pnlBarDetails = $find(this.pnlBarDetailsId);
            var workflowRoleMappingConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowRoleMapping');
            this._workflowRoleMappingService = new WorkflowRoleMappingService(workflowRoleMappingConfiguration);
            var workflowRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowRepository');
            this._workflowRepositoryService = new WorkflowRepositoryService(workflowRepositoryConfiguration);
            var domainUserConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'DomainUserModel');
            this._domainUserService = new DomainUserService(domainUserConfiguration);
            var workflowEvaluationProperty = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowEvaluationProperty");
            this._workflowEvaluationPropertyService = new WorkflowEvaluationPropertyService(workflowEvaluationProperty);
            this.repositories = new Array();
            this.searchWorkflows();
        };
        TbltWorkflowRepository.prototype.searchWorkflows = function () {
            var _this_1 = this;
            this._loadingPanel.show(this.rtvWorkflowRepositoryId);
            this._workflowRepositoryService.getByName(this._txtSearchName.get_textBoxValue(), function (data) {
                _this_1.repositories = data;
                _this_1.loadWorkflowRepositories(data);
                _this_1._loadingPanel.hide(_this_1.rtvWorkflowRepositoryId);
            }, function (exception) {
                _this_1._loadingPanel.hide(_this_1.rtvWorkflowRepositoryId);
                _this_1.showNotificationException(_this_1.uscNotificationId, exception);
            });
        };
        TbltWorkflowRepository.prototype.redrawNodeWithPositions = function (selectedNodeParent, selectedNode, workflowSteps) {
            var nodeLength = selectedNodeParent.get_allNodes().length - 1;
            for (var i = nodeLength; i >= 0; i--) {
                selectedNodeParent.get_nodes().remove(selectedNodeParent.get_nodes().getNode(i));
            }
            for (var i = 0; i < workflowSteps.length; i++) {
                var stepNode = new Telerik.Web.UI.RadTreeNode();
                stepNode.set_text(workflowSteps[i].Name);
                stepNode.set_value(workflowSteps[i].Position);
                stepNode.set_imageUrl('../Comm/Images/DocSuite/Resolution16.gif');
                stepNode.get_attributes().setAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME, WorkflowTreeNodeType.Step);
                selectedNodeParent.get_nodes().add(stepNode);
            }
        };
        TbltWorkflowRepository.prototype.updateWorkflowRepositoryStep = function (workflowStep, uniqueId) {
            var workflowRepositoryToUpdate = this.repositories.filter(function (repository) { return repository.UniqueId == uniqueId; })[0];
            var Json = JSON.parse(workflowRepositoryToUpdate.Json);
            var JsonToUpdate = Object.keys(Json).map(function (i) { return Json[i]; });
            var workflowStepToUpdate = JsonToUpdate.filter(function (step) { return step.Position == workflowStep.Position; })[0];
            if (JsonToUpdate.indexOf(workflowStepToUpdate) >= 0) {
                JsonToUpdate[JsonToUpdate.indexOf(workflowStepToUpdate)] = workflowStep;
                var stepNode = this._rtvWorkflowRepository.get_selectedNode();
                stepNode.set_text(workflowStep.Name);
                this.fillWorkflowStepInformations(workflowStep);
            }
            else {
                JsonToUpdate.push(workflowStep);
                var stepNode = new Telerik.Web.UI.RadTreeNode();
                stepNode.set_text(workflowStep.Name);
                stepNode.set_value(workflowStep.Position);
                stepNode.set_imageUrl('../Comm/Images/DocSuite/Resolution16.gif');
                stepNode.get_attributes().setAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME, WorkflowTreeNodeType.Step);
                var parentNode = this._rtvWorkflowRepository.findNodeByValue(uniqueId);
                parentNode.get_nodes().add(stepNode);
            }
            var updatedJson = {};
            JsonToUpdate.forEach(function (step, idx) {
                updatedJson[idx] = step;
            });
            workflowRepositoryToUpdate.Json = JSON.stringify(updatedJson);
            this._workflowRepositoryService.updateWorkflowRepository(workflowRepositoryToUpdate);
        };
        TbltWorkflowRepository.prototype.deleteWorkflowRepositoryStep = function (workflowStepPosition, uniqueId) {
            var workflowRepository = this.repositories.filter(function (repository) { return repository.UniqueId == uniqueId; })[0];
            var json = JSON.parse(workflowRepository.Json);
            var workflowSteps = Object.keys(json).map(function (i) { return json[i]; });
            workflowSteps = workflowSteps.filter(function (step) { return step.Position != workflowStepPosition; });
            workflowSteps = this.updateWorkflowStepsPositions(workflowSteps, workflowStepPosition);
            var updatedJson = {};
            workflowSteps.forEach(function (step, idx) {
                updatedJson[idx] = step;
            });
            workflowRepository.Json = JSON.stringify(updatedJson);
            this._workflowRepositoryService.updateWorkflowRepository(workflowRepository);
            return workflowSteps;
        };
        TbltWorkflowRepository.prototype.updateWorkflowStepsPositions = function (workflowSteps, startPosition) {
            for (var i = startPosition; i < workflowSteps.length; i++) {
                workflowSteps[i].Position--;
            }
            return workflowSteps;
        };
        TbltWorkflowRepository.prototype.updateWorkflowStepArgument = function (argument, uniqueId, stepPosition, type) {
            var workflowRepositoryToUpdate = this.repositories.filter(function (repository) { return repository.UniqueId == uniqueId; })[0];
            var Json = JSON.parse(workflowRepositoryToUpdate.Json);
            var JsonToUpdate = Object.keys(Json).map(function (i) { return Json[i]; });
            var workflowStepToUpdate = JsonToUpdate.filter(function (step) { return step.Position == stepPosition; })[0];
            switch (type) {
                case WorkflowArgumentType.Input: {
                    var inputArguments = workflowStepToUpdate.InputArguments ? workflowStepToUpdate.InputArguments : new Array();
                    var argumentToUpdate = inputArguments.filter(function (arg) { return arg.Name == argument.Name; })[0];
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
                    var evaluationArguments = workflowStepToUpdate.EvaluationArguments ? workflowStepToUpdate.EvaluationArguments : new Array();
                    var argumentToUpdate = evaluationArguments.filter(function (arg) { return arg.Name == argument.Name; })[0];
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
                    var outputArguments = workflowStepToUpdate.OutputArguments ? workflowStepToUpdate.OutputArguments : new Array();
                    var argumentToUpdate = outputArguments.filter(function (arg) { return arg.Name == argument.Name; })[0];
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
            var updatedJson = {};
            JsonToUpdate.forEach(function (step, idx) {
                updatedJson[idx] = step;
            });
            workflowRepositoryToUpdate.Json = JSON.stringify(updatedJson);
            this._workflowRepositoryService.updateWorkflowRepository(workflowRepositoryToUpdate);
        };
        TbltWorkflowRepository.prototype.deleteWorkflowStepArgument = function (argumentName, uniqueId, stepPosition, argumentType) {
            var workflowRepository = this.repositories.filter(function (repository) { return repository.UniqueId == uniqueId; })[0];
            var json = JSON.parse(workflowRepository.Json);
            var workflowSteps = Object.keys(json).map(function (i) { return json[i]; });
            var workflowStepToUpdate = workflowSteps.filter(function (step) { return step.Position == stepPosition; })[0];
            switch (argumentType) {
                case WorkflowArgumentType.Input: {
                    var inputArguments = workflowStepToUpdate.InputArguments;
                    inputArguments = inputArguments.filter(function (argument) { return argument.Name != argumentName; });
                    this.updateWorkflowStepArgumentsTableView(inputArguments, uniqueId, WorkflowArgumentType.Input);
                    workflowStepToUpdate.InputArguments = inputArguments;
                    workflowSteps[workflowSteps.indexOf(workflowStepToUpdate)] = workflowStepToUpdate;
                    break;
                }
                case WorkflowArgumentType.Evaluation: {
                    var evaluationArguments = workflowStepToUpdate.EvaluationArguments;
                    evaluationArguments = evaluationArguments.filter(function (argument) { return argument.Name != argumentName; });
                    this.updateWorkflowStepArgumentsTableView(evaluationArguments, uniqueId, WorkflowArgumentType.Evaluation);
                    workflowStepToUpdate.EvaluationArguments = evaluationArguments;
                    workflowSteps[workflowSteps.indexOf(workflowStepToUpdate)] = workflowStepToUpdate;
                    break;
                }
                case WorkflowArgumentType.Output: {
                    var outputArguments = workflowStepToUpdate.OutputArguments;
                    outputArguments = outputArguments.filter(function (argument) { return argument.Name != argumentName; });
                    this.updateWorkflowStepArgumentsTableView(outputArguments, uniqueId, WorkflowArgumentType.Output);
                    workflowStepToUpdate.OutputArguments = outputArguments;
                    workflowSteps[workflowSteps.indexOf(workflowStepToUpdate)] = workflowStepToUpdate;
                    break;
                }
            }
            var updatedJson = {};
            workflowSteps.forEach(function (step, idx) {
                updatedJson[idx] = step;
            });
            workflowRepository.Json = JSON.stringify(updatedJson);
            this._workflowRepositoryService.updateWorkflowRepository(workflowRepository);
        };
        TbltWorkflowRepository.prototype.updateWorkflowStepArgumentsTableView = function (argumentsCollection, uniqueId, argumentType) {
            var agumentsModel = this.populateArgumentsMasterViewTable(argumentsCollection, uniqueId);
            var workflowStepArgumentsTableView;
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
        };
        TbltWorkflowRepository.prototype.updateWorkflowRepository = function (workflowRepository, action) {
            if (action == OpenWindowOperationType.Add) {
                //insert workflow
                var rootNode = this._rtvWorkflowRepository.get_selectedNode();
                var node = new Telerik.Web.UI.RadTreeNode();
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
                var repositoryNode = this._rtvWorkflowRepository.findNodeByValue(workflowRepository.UniqueId);
                repositoryNode.set_text(workflowRepository.Name);
                this.fillWorkflowRepositoryInformations(workflowRepository);
                this._workflowRepositoryService.updateWorkflowRepository(workflowRepository);
            }
        };
        TbltWorkflowRepository.prototype.openWorkflowStepGesWindow = function (operation) {
            var _this_1 = this;
            this._rwmWorkflowStep.setSize(750, 550);
            var url = "../Tblt/TbltWorkflowStepGes.aspx?Type=Comm&Action=" + OpenWindowOperationType[operation];
            var selectedNode = this._rtvWorkflowRepository.get_selectedNode();
            var parentNodeId = operation === OpenWindowOperationType.Add ? selectedNode.get_value() : selectedNode.get_parent().get_value();
            var workflowRepository;
            this._workflowRepositoryService.getById(parentNodeId, function (data) {
                workflowRepository = data;
                if (workflowRepository.Json === "{}") { /*If it's step 0*/
                    workflowRepository.Json = _this_1.initializeFirstStep();
                }
                sessionStorage.setItem(WorkflowTreeNodeType.Workflow.toString(), JSON.stringify(workflowRepository));
                if (operation === OpenWindowOperationType.Edit) {
                    var steps_1 = JSON.parse(workflowRepository.Json);
                    var workflowSteps = Object.keys(steps_1).map(function (i) { return steps_1[i]; });
                    var workflowStep = workflowSteps.filter(function (step) { return step.Position == selectedNode.get_value(); })[0];
                    sessionStorage.setItem(WorkflowTreeNodeType.Step.toString(), JSON.stringify(workflowStep));
                    _this_1._rwmWorkflowStep.set_modal(true);
                    _this_1._rwmWorkflowStep.open(url, "windowWorkflowStep", undefined);
                }
            });
            if (operation === OpenWindowOperationType.Add) {
                this._rwmWorkflowStep.set_modal(true);
                this._rwmWorkflowStep.open(url, "windowWorkflowStep", undefined);
            }
        };
        TbltWorkflowRepository.prototype.initializeFirstStep = function () {
            var wfStep = new Array();
            var firstStep = {
                Position: -1,
                Name: "",
                AuthorizationType: null,
                ActivityType: null,
                ActivityOperation: {
                    Action: null,
                    Area: null
                },
                EvaluationArguments: new Array(),
                InputArguments: new Array(),
                OutputArguments: new Array()
            };
            wfStep.push(firstStep);
            return JSON.stringify(wfStep);
        };
        TbltWorkflowRepository.prototype.openWorkflowPropertyGesWindow = function (operation, argumentType) {
            this._rwWorkflowProperty.setSize(650, 400);
            var validation = argumentType === WorkflowArgumentType.Evaluation;
            var url = "../Tblt/TbltWorkflowPropertyGes.aspx?Type=Comm&Action=" + OpenWindowOperationType[operation] + "&Argument=" + WorkflowArgumentType[argumentType] + "&Validation=" + validation;
            var selectedNode = this._rtvWorkflowRepository.get_selectedNode();
            var parentNodeId = selectedNode.get_parent().get_value();
            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_REPOSITORY, parentNodeId);
            sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_WORKFLOW_STEP, selectedNode.get_value());
            var selectedItem;
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
        };
        TbltWorkflowRepository.prototype.openWorkflowRepositoryGesWindow = function (operation) {
            var _this_1 = this;
            this._rwWorkflowRepository.setSize(750, 550);
            var url = "../Tblt/TbltRepositoryGes.aspx?Type=Comm&Action=" + OpenWindowOperationType[operation];
            if (operation === OpenWindowOperationType.Edit) {
                var selectedNode = this._rtvWorkflowRepository.get_selectedNode();
                var parentNodeId = selectedNode.get_value();
                var workflowRepository_1;
                this._workflowRepositoryService.getById(parentNodeId, function (data) {
                    workflowRepository_1 = data;
                    sessionStorage.setItem(WorkflowTreeNodeType.Workflow.toString(), JSON.stringify(workflowRepository_1));
                    _this_1._rwWorkflowRepository.set_modal(true);
                    _this_1._rwWorkflowRepository.open(url, "windowWorkflowRepository", undefined);
                });
            }
            if (operation === OpenWindowOperationType.Add) {
                this._rwWorkflowRepository.set_modal(true);
                this._rwWorkflowRepository.open(url, "windowWorkflowRepository", undefined);
            }
        };
        /**
         *------------------------- Methods -----------------------------
         */
        /**
         * Metodo per l'apertura delle finestre di gestione workflow
         * @param operation
         */
        TbltWorkflowRepository.prototype.openManagementWindow = function (operation) {
            var selectedNode = this._rtvWorkflowRepository.get_selectedNode();
            if (selectedNode == undefined) {
                this.showWarningMessage(this.uscNotificationId, 'Nessuna attività selezionato');
                return;
            }
            var isXamlRepository = selectedNode.get_attributes().getAttribute('isXaml');
            var selectedMappingTags;
            if (isXamlRepository) {
                selectedMappingTags = this._rgvXamlWorkflowRoleMappings.get_selectedItems();
            }
            else {
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
            var qs = 'Action='.concat(operation, '&IdWorkflowRepository=', selectedNode.get_value());
            if (operation == 'Edit') {
                if (isXamlRepository) {
                    var workflowActivityModel = selectedMappingTags[0].get_dataItem();
                    if (workflowActivityModel.WorkflowRoleMapping.UniqueId != undefined) {
                        qs = qs.concat('&IdWorkflowRoleMapping=', workflowActivityModel.WorkflowRoleMapping.UniqueId);
                    }
                    else {
                        qs = 'Action=Add&IdWorkflowRepository='.concat(selectedNode.get_value());
                    }
                    qs = qs.concat('&MappingTag=', workflowActivityModel.MappingTag, '&FromXamlActivity=true&XamlInternalActivity=', workflowActivityModel.Activity.Id);
                }
                else {
                    qs = qs.concat('&IdWorkflowRoleMapping=', selectedMappingTags[0].get_dataItem().UniqueId);
                }
            }
            var url = '../Tblt/TbltWorkflowRepositoryGes.aspx?Type=Comm&'.concat(qs);
            this._windowAddWorkflowRoleMapping.setSize(750, 550);
            this._windowAddWorkflowRoleMapping.setUrl(url);
            this._windowAddWorkflowRoleMapping.set_modal(true);
            this._windowAddWorkflowRoleMapping.show();
        };
        /**
         * Metodo che imposta a video i workflow cercati
         * @param repositories
         */
        TbltWorkflowRepository.prototype.loadWorkflowRepositories = function (repositories) {
            if (repositories == undefined || repositories.length == 0) {
                return;
            }
            try {
                this._rtvWorkflowRepository.beginUpdate();
                this._rtvWorkflowRepository.get_nodes().clear();
                var rootNode_1 = new Telerik.Web.UI.RadTreeNode();
                rootNode_1.set_text("Workflow Repositories");
                rootNode_1.set_imageUrl('../App_Themes/DocSuite2008/imgset16/process.png');
                rootNode_1.get_attributes().setAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME, WorkflowTreeNodeType.Root);
                rootNode_1.set_expanded(true);
                $.each(repositories, function (index, repository) {
                    var node = new Telerik.Web.UI.RadTreeNode();
                    node.get_attributes().setAttribute('isXaml', !String.isNullOrEmpty(repository.Xaml));
                    node.set_text(repository.Name);
                    node.set_value(repository.UniqueId);
                    node.set_imageUrl('../Comm/Images/DocSuite/Workflow16.png');
                    node.get_attributes().setAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME, WorkflowTreeNodeType.Workflow);
                    if (repository.Json) {
                        var steps = JSON.parse(repository.Json);
                        $.each(steps, function (index, step) {
                            var stepNode = new Telerik.Web.UI.RadTreeNode();
                            stepNode.set_text(step.Name);
                            stepNode.set_value(step.Position);
                            stepNode.set_imageUrl('../App_Themes/DocSuite2008/imgset16/link.png');
                            stepNode.get_attributes().setAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME, WorkflowTreeNodeType.Step);
                            node.get_nodes().add(stepNode);
                        });
                    }
                    rootNode_1.get_nodes().add(node);
                });
                this._rtvWorkflowRepository.get_nodes().add(rootNode_1);
                this._rtvWorkflowRepository.endUpdate();
            }
            catch (error) {
                this.showNotificationMessage(this.uscNotificationId, error.message);
                console.log(JSON.stringify(error));
            }
        };
        TbltWorkflowRepository.prototype.registerUscRoleRestEventHandlers = function () {
            var uscRoleRestEvents = this._uscRoleRest.uscRoleRestEvents;
            this._uscRoleRest.registerEventHandler(uscRoleRestEvents.NewRolesAdded, this.addWorkflowRolesPromise);
            this._uscRoleRest.registerEventHandler(uscRoleRestEvents.RoleDeleted, this.deleteWorkflowRolePromise);
        };
        /**
         * Pulisce i datasource delle tabelle di visualizzazione mapping
         */
        TbltWorkflowRepository.prototype.clearDataSources = function () {
            var workflowRoleMappingsMasterTableView = this._rgvWorkflowRoleMappings.get_masterTableView();
            var xamlWorkflowRoleMappingsMasterTableView = this._rgvXamlWorkflowRoleMappings.get_masterTableView();
            workflowRoleMappingsMasterTableView.set_dataSource([]);
            workflowRoleMappingsMasterTableView.dataBind();
            xamlWorkflowRoleMappingsMasterTableView.set_dataSource([]);
            xamlWorkflowRoleMappingsMasterTableView.dataBind();
        };
        /**
         * Aggiorna il datasource dei Tag disponibili per i workflow di tipo XAML
         */
        TbltWorkflowRepository.prototype.fillMappingSelection = function () {
            var _this_1 = this;
            var result = $.Deferred();
            var selectedNode = this._rtvWorkflowRepository.get_selectedNode();
            this._rcbSelectMappingTag.clearItems();
            var nodeType = selectedNode.get_attributes().getAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME);
            var repositoryId = nodeType === WorkflowTreeNodeType.Workflow ? selectedNode.get_value() : selectedNode.get_parent().get_value();
            this._workflowRoleMappingService.getByName('', repositoryId, function (response) {
                if (response == undefined)
                    return result.resolve();
                try {
                    var models = response;
                    if (models.length > 0) {
                        _this_1._mappingDataSource.set_data(models);
                    }
                    else {
                        _this_1._mappingDataSource.set_data('[{}]');
                    }
                    _this_1._mappingDataSource.fetch();
                    return result.resolve();
                }
                catch (error) {
                    console.log(JSON.stringify(error));
                    return result.reject(error);
                }
            }, function (exception) {
                return result.reject(exception);
            });
            return result.promise();
        };
        /**
         * Imposta la visibilità degli elementi del pannello dettagi in base
         * al tipo di workflow gestito
         * @param model
         */
        TbltWorkflowRepository.prototype.setDetailsVisibility = function (model) {
            if (String.isNullOrEmpty(model.Xaml)) {
                $('#workflowRoleMappings').show();
                $('#xamlWorkflowRoleMappings').hide();
                $('#'.concat(this.pnlSelectMappingTagId)).hide();
                $(this._btnAggiungi.get_element()).show();
            }
            else {
                $('#workflowRoleMappings').hide();
                $('#xamlWorkflowRoleMappings').show();
                $('#'.concat(this.pnlSelectMappingTagId)).show();
                $(this._btnAggiungi.get_element()).hide();
            }
        };
        /**
         * Imposta i dettagli del workflow selezionato
         */
        TbltWorkflowRepository.prototype.fillDetailsPanel = function () {
            var _this_1 = this;
            var result = $.Deferred();
            var selectedNode = this._rtvWorkflowRepository.get_selectedNode();
            var nodeType = selectedNode.get_attributes().getAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME);
            var repositoryId = nodeType === WorkflowTreeNodeType.Workflow ? selectedNode.get_value() :
                nodeType === WorkflowTreeNodeType.Step ? selectedNode.get_parent().get_value() : null;
            this._workflowRepositoryService.getById(repositoryId, function (response) {
                if (response == undefined)
                    return result.reject();
                try {
                    var model = response;
                    if (selectedNode.get_attributes().getAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME) === WorkflowTreeNodeType.Step) {
                        var jsonModel_1 = JSON.parse(model.Json);
                        var workflowSteps = Object.keys(jsonModel_1).map(function (i) { return jsonModel_1[i]; });
                        var workflowStepModel = workflowSteps.filter(function (step) { return step.Position == selectedNode.get_value(); })[0];
                        _this_1.fillWorkflowStepInformations(workflowStepModel);
                    }
                    _this_1.fillWorkflowRepositoryInformations(model);
                    _this_1.clearDataSources();
                    _this_1.setDetailsVisibility(model);
                    return result.resolve(model);
                }
                catch (error) {
                    console.log(JSON.stringify(error));
                    return result.reject(error);
                }
            }, function (exception) {
                return result.reject(exception);
            });
            return result.promise();
        };
        TbltWorkflowRepository.prototype.fillWorkflowRepositoryInformations = function (workflowRepositoryModel) {
            $('#'.concat(this.lblStatusId)).html(this.getStatusDescription(workflowRepositoryModel.Status.toString()));
            $('#'.concat(this.lblVersionId)).html(workflowRepositoryModel.Version.toString());
            if (workflowRepositoryModel.ActiveTo != undefined) {
                $('#'.concat(this.lblActiveToId)).html(moment(workflowRepositoryModel.ActiveTo).format("DD/MM/YYYY"));
            }
            $('#'.concat(this.lblActiveFromId)).html(moment(workflowRepositoryModel.ActiveFrom).format("DD/MM/YYYY"));
            if (parseInt(workflowRepositoryModel.DSWEnvironment.toString())) {
                $('#'.concat(this.lblTipoligiaId)).html(DSWEnvironmentType[workflowRepositoryModel.DSWEnvironment]);
            }
            else {
                $('#'.concat(this.lblTipoligiaId)).html(workflowRepositoryModel.DSWEnvironment.toString());
            }
            this.workflowEnvironment = workflowRepositoryModel.DSWEnvironment.toString();
        };
        TbltWorkflowRepository.prototype.fillWorkflowStepInformations = function (workflowStepModel) {
            $('#'.concat(this.lblPositionId)).html(workflowStepModel.Position.toString());
            $('#'.concat(this.lblStepNameId)).html(workflowStepModel.Name.toString());
            var authType = workflowStepModel.AuthorizationType != undefined ? workflowStepModel.AuthorizationType.toString() : "";
            $('#'.concat(this.lblAutorizationTypeId)).html(authType);
            var activityType = workflowStepModel.ActivityType != undefined ? workflowStepModel.ActivityType.toString() : "";
            if (Number(activityType)) {
                $('#'.concat(this.lblActivityTypeId)).html(this._enumHelper.getActivityTypeDescription(ActivityType[activityType]));
            }
            else {
                $('#'.concat(this.lblActivityTypeId)).html(this._enumHelper.getActivityTypeDescription(activityType));
            }
            var area = workflowStepModel.ActivityOperation.Area != undefined ? workflowStepModel.ActivityOperation.Area.toString() : "";
            if (Number(area)) {
                $('#'.concat(this.lblAreaId)).html(this._enumHelper.getActivityAreaDescription(ActivityArea[area]));
            }
            else {
                $('#'.concat(this.lblAreaId)).html(this._enumHelper.getActivityAreaDescription(area));
            }
            var action = workflowStepModel.ActivityOperation.Action != undefined ? workflowStepModel.ActivityOperation.Action.toString() : "";
            if (Number(action)) {
                $('#'.concat(this.lblActionId)).html(this._enumHelper.getWorkflowActivityActionDescription(Number(action)));
            }
            else {
                $('#'.concat(this.lblActionId)).html(action);
            }
        };
        /**
         * Imposta i mapping tag già gestiti per i workflow di tipo json
         * @param model
         */
        TbltWorkflowRepository.prototype.fillJsonMappings = function (model) {
            try {
                if (String.isNullOrEmpty(model.Xaml)) {
                    var workflowRoleMappingsMasterTableView = this._rgvWorkflowRoleMappings.get_masterTableView();
                    workflowRoleMappingsMasterTableView.set_dataSource(model.WorkflowRoleMappings);
                    workflowRoleMappingsMasterTableView.clearSelectedItems();
                    workflowRoleMappingsMasterTableView.dataBind();
                    var startupModel = this.populateStartupMasterViewTable(model.WorkflowEvaluationProperties);
                    var workflowStartupMasterTableView = this._rgvWorkflowStartUp.get_masterTableView();
                    workflowStartupMasterTableView.set_dataSource(startupModel);
                    workflowStartupMasterTableView.clearSelectedItems();
                    workflowStartupMasterTableView.dataBind();
                    var selectedNode = this._rtvWorkflowRepository.get_selectedNode();
                    var nodePosition_1 = selectedNode.get_value();
                    var nodeType = selectedNode.get_attributes().getAttribute(TbltWorkflowRepository.NODETYPE_ATTRNAME);
                    if (nodeType === WorkflowTreeNodeType.Step) {
                        var json_1 = JSON.parse(model.Json);
                        var workflowSteps = Object.keys(json_1).map(function (i) { return json_1[i]; });
                        var workflowStep = workflowSteps.filter(function (step) { return step.Position == nodePosition_1; })[0];
                        var inputArguments = workflowStep.InputArguments ? workflowStep.InputArguments : new Array();
                        ;
                        this.updateWorkflowStepArgumentsTableView(inputArguments, model.UniqueId, WorkflowArgumentType.Input);
                        var evaluationArguments = workflowStep.EvaluationArguments ? workflowStep.EvaluationArguments : new Array();
                        ;
                        this.updateWorkflowStepArgumentsTableView(evaluationArguments, model.UniqueId, WorkflowArgumentType.Evaluation);
                        var outputArguments = workflowStep.OutputArguments ? workflowStep.OutputArguments : new Array();
                        ;
                        this.updateWorkflowStepArgumentsTableView(outputArguments, model.UniqueId, WorkflowArgumentType.Output);
                    }
                }
            }
            catch (error) {
                this.showNotificationMessage(this.uscNotificationId, error.message);
                console.log(JSON.stringify(error));
            }
        };
        TbltWorkflowRepository.prototype.populateStartupMasterViewTable = function (model) {
            var customModel = new Array();
            for (var _i = 0, model_1 = model; _i < model_1.length; _i++) {
                var wep = model_1[_i];
                if (wep.ValueInt != null) {
                    customModel.push({ Name: wep.Name, Value: wep.ValueInt, UniqueId: wep.UniqueId });
                }
                else if (wep.ValueDate != null) {
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
        };
        TbltWorkflowRepository.prototype.populateArgumentsMasterViewTable = function (model, repositoryId) {
            var customModel = new Array();
            if (!model || model.length < 1 || Object.keys(model[0]).length < 1) {
                return customModel;
            }
            for (var _i = 0, model_2 = model; _i < model_2.length; _i++) {
                var argument = model_2[_i];
                var argumentPropertyType = this._enumHelper.fixEnumValue(argument.PropertyType, ArgumentType);
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
        };
        TbltWorkflowRepository.prototype.resolveDomainUsers = function (roleMappings) {
            var _this_1 = this;
            var promises = new Array();
            $.each(roleMappings, function (index, mapping) {
                var deferred = $.Deferred();
                if (mapping.AuthorizationType.toString() != WorkflowAuthorizationType[WorkflowAuthorizationType.UserName])
                    return;
                _this_1._domainUserService.getUser(mapping.AccountName, function (response) {
                    if (response == undefined)
                        return deferred.reject();
                    var domainUser = response;
                    mapping.AccountName = domainUser.DisplayName;
                    deferred.resolve();
                }, function (exception) {
                    deferred.reject(exception);
                });
                promises.push(deferred);
            });
            return $.when.apply(undefined, promises).promise(roleMappings);
        };
        /**
         * Imposta i mapping tag per i workflow di tipo xaml, gestendo le internal activity custom
         * @param model
         */
        TbltWorkflowRepository.prototype.fillXamlMappings = function (model, customActivities, selectedMappingText) {
            var _this_1 = this;
            var result = $.Deferred();
            if (selectedMappingText == '')
                return result.resolve(null);
            if (customActivities.length == 0)
                return result.resolve(null);
            $.when(this.resolveDomainUsers(model.WorkflowRoleMappings)).always(function () {
                var sourceModel = new Array();
                try {
                    $.each(customActivities, function (index, activity) {
                        var filteredMappings = $.grep(model.WorkflowRoleMappings, function (map, index) {
                            return map.IdInternalActivity.toLowerCase() == activity.Id.toLowerCase() && map.MappingTag.toLowerCase() == selectedMappingText.toLowerCase();
                        });
                        var mapping;
                        if (filteredMappings.length > 0) {
                            mapping = filteredMappings[0];
                        }
                        else {
                            mapping = {};
                        }
                        sourceModel.push({ Activity: activity, WorkflowRoleMapping: mapping, MappingTag: selectedMappingText });
                    });
                    var xamlWorkflowRoleMappingsMasterTableView = _this_1._rgvXamlWorkflowRoleMappings.get_masterTableView();
                    xamlWorkflowRoleMappingsMasterTableView.set_dataSource(sourceModel);
                    xamlWorkflowRoleMappingsMasterTableView.clearSelectedItems();
                    xamlWorkflowRoleMappingsMasterTableView.dataBind();
                    result.resolve();
                }
                catch (error) {
                    _this_1.showNotificationMessage(_this_1.uscNotificationId, error.message);
                    console.log(JSON.stringify(error));
                    result.reject(error);
                }
            }).fail(function (exception) { _this_1.showNotificationException(_this_1.uscNotificationId, exception); });
            return result.promise();
        };
        /**
         * Carica i dettagli del workflow selezionato
         */
        TbltWorkflowRepository.prototype.loadDetails = function () {
            var _this_1 = this;
            this._loadingPanel.show(this.pnlDetailsId);
            //Gestito JQueryPromise che simula la gestion async dei metodi (IE8 supportato)     
            $.when(this.fillDetailsPanel()).done(function (model) {
                var activities = !String.isNullOrEmpty(model.CustomActivities) ? JSON.parse(model.CustomActivities) : [];
                var selectedTag = (!String.isNullOrEmpty(_this_1._rcbSelectMappingTag.get_text()) && _this_1._rcbSelectMappingTag.get_text() != _this_1._rcbSelectMappingTag.get_emptyMessage()) ? _this_1._rcbSelectMappingTag.get_text() : '';
                _this_1._currentWorkflowRepositoryModel = model;
                //- changing ProponenteDiAvio/DestinatarioDiAvio will invalidate ProponenteDiDefault/DestinatarioDiDefault
                //- when changing ProponenteDiAvio/DestinatarioDiAvio, after closing the edit window, this method is called
                //- final values are updated after fillDetailsPanel
                if (_this_1.updateDefaultOnStartDependencyValue()) {
                    _this_1._loadingPanel.hide(_this_1.pnlDetailsId);
                    _this_1.loadDetails();
                }
                else {
                    $.when(_this_1.fillMappingSelection(), _this_1.fillJsonMappings(model), _this_1.fillXamlMappings(model, activities, selectedTag)).always(function () {
                        _this_1._loadingPanel.hide(_this_1.pnlDetailsId);
                    }).fail(function (exception) {
                        _this_1._currentWorkflowRepositoryModel = null;
                        _this_1.showNotificationException(_this_1.uscNotificationId, exception);
                    });
                }
            }).fail(function (exception) {
                _this_1._currentWorkflowRepositoryModel = null;
                _this_1._loadingPanel.hide(_this_1.pnlDetailsId);
                _this_1.showNotificationException(_this_1.uscNotificationId, exception, "Errore nel caricamento dell'attività.");
            });
        };
        /**
         * Esegue il mapping delle WorkflowAuthorizationType
         * @param authorizationType
         */
        TbltWorkflowRepository.prototype.getAuthorizationTypeDescription = function (authorizationType) {
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
        };
        TbltWorkflowRepository.prototype.getNameDescription = function (name) {
            return WorkflowEvalutionPropertyHelper[name] ? WorkflowEvalutionPropertyHelper[name].Name : name;
        };
        /**
         * Esegue il mapping delle WorkflowStatus
         * @param status
         */
        TbltWorkflowRepository.prototype.getStatusDescription = function (status) {
            switch (WorkflowRepositoryStatus[status]) {
                case WorkflowRepositoryStatus.Draft:
                    return 'Bozza';
                case WorkflowRepositoryStatus.Published:
                    return 'Pubblicato';
                default:
                    return '';
            }
        };
        TbltWorkflowRepository.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                var uscNotification = $("#".concat(uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            }
            else {
                this.showNotificationMessage(uscNotificationId, customMessage);
            }
        };
        TbltWorkflowRepository.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        TbltWorkflowRepository.prototype.showWarningMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showWarningMessage(customMessage);
            }
        };
        TbltWorkflowRepository.prototype.openWorkflowEvaluationPropertiesManagementWindow = function (operation) {
            this._windowAddWorkflowRoleMapping.setSize(600, 250);
            var selectedNode = this._rtvWorkflowRepository.get_selectedNode();
            var _this = this;
            if (operation === "Add") {
                var openUrl = "TbltWorkflowEvaluationPropertyGes.aspx?Action=" + operation + "&WorkflowRepositoryId=" + selectedNode.get_value() + "&WorkflowEnv=" + this.workflowEnvironment;
                openUrl = this.makeStartQueryParameters(openUrl);
                this._windowAddWorkflowRoleMapping.setUrl(openUrl);
            }
            else if (operation === "Edit") {
                var selectedEvaluationPropertyId = this._rgvWorkflowStartUp.get_selectedItems();
                if (selectedEvaluationPropertyId == undefined || selectedEvaluationPropertyId.length == 0) {
                    this.showWarningMessage(this.uscNotificationId, 'Nessun proprietà selezionata');
                    return;
                }
                if (selectedEvaluationPropertyId.length > 1) {
                    this.showWarningMessage(this.uscNotificationId, 'Seleziona una sola proprietà per la modifica');
                    return;
                }
                var workflowEvaluationModel = selectedEvaluationPropertyId[0].get_dataItem().UniqueId;
                var openUrl = "TbltWorkflowEvaluationPropertyGes.aspx";
                openUrl += "?" + QueryParameters.QUERY_PARAM_ACTION + "=" + operation;
                openUrl += "&" + QueryParameters.QUERY_PARAM_WORKFLOW_REPOSITORY_ID + "=" + selectedNode.get_value();
                openUrl += "&" + QueryParameters.QUERY_PARAM_WORKFLOW_EVALUATION_PROPERTY_ID + "=" + workflowEvaluationModel;
                openUrl += "&WorkflowEnv=" + this.workflowEnvironment;
                openUrl = this.makeStartQueryParameters(openUrl);
                this._windowAddWorkflowRoleMapping.setUrl(openUrl);
            }
            this._windowAddWorkflowRoleMapping.set_modal(true);
            this._windowAddWorkflowRoleMapping.show();
        };
        /**
         * If the WorflowRepositoryModel has the properties _dsw_p_WorkflowStartProposer (ProponenteDiAvio) and
         * _dsw_p_WorkflowStartRecipient (Destinatario Di Avio) it will create extra query parameters and add them to the query
         * @param url
         */
        TbltWorkflowRepository.prototype.makeStartQueryParameters = function (url) {
            //TODO: refactor below
            var startProposer = this._currentWorkflowRepositoryModel
                .WorkflowEvaluationProperties
                .filter(function (x) { return x.Name == TbltWorkflowRepository.PROPERTY_NAME_WORKFLOW_START_PROPOSER; })[0];
            var startReceiver = this._currentWorkflowRepositoryModel
                .WorkflowEvaluationProperties
                .filter(function (x) { return x.Name == TbltWorkflowRepository.PROPERTY_NAME_WORKFLOW_START_RECIPIENT; })[0];
            if (startProposer !== null && startProposer !== undefined) {
                url += "&" + QueryParameters.QUERY_PARAM_START_PROPOSER + "=" + startProposer.ValueInt;
            }
            if (startReceiver !== null && startReceiver !== undefined) {
                url += "&" + QueryParameters.QUERY_PARAM_START_RECEIVER + "=" + startReceiver.ValueInt;
            }
            return url;
        };
        /**
         * If the workflow repositort has the properties  _dsw_p_WorkflowStartProposer(ProponenteDiAvio) and _dsw_p_WorkflowDefaultProposer (ProponenteDiDefault)
         * then there is a strict dependency on their values.
         *   If ProponenteDiAvio=0 the ProponenteDiDefault = Settore
         *   If ProponenteDiAvio=1 the ProponenteDiDefault = Account/Utente
         * Same logic applies for _dsw_p_WorkflowStartRecipient (DestinatarioDiAvio) and _dsw_p_WorkflowDefaultRecipient (DestinatarioDiDefault)
         **/
        TbltWorkflowRepository.prototype.updateDefaultOnStartDependencyValue = function () {
            var mustReload = false;
            var defaultProposer = this._currentWorkflowRepositoryModel
                .WorkflowEvaluationProperties
                .filter(function (x) { return x.Name == TbltWorkflowRepository.PROPERTY_NAME_WORKFLOW_DEFAULT_PROPOSER; })[0];
            var startProposer = this._currentWorkflowRepositoryModel
                .WorkflowEvaluationProperties
                .filter(function (x) { return x.Name == TbltWorkflowRepository.PROPERTY_NAME_WORKFLOW_START_PROPOSER; })[0];
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
            var defaultReceiver = this._currentWorkflowRepositoryModel
                .WorkflowEvaluationProperties
                .filter(function (x) { return x.Name === TbltWorkflowRepository.PROPERTY_NAME_WORKFLOW_DEFAULT_RECIPIENT; })[0];
            var startReceiver = this._currentWorkflowRepositoryModel
                .WorkflowEvaluationProperties
                .filter(function (x) { return x.Name === TbltWorkflowRepository.PROPERTY_NAME_WORKFLOW_START_RECIPIENT; })[0];
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
        };
        TbltWorkflowRepository.NODETYPE_ATTRNAME = "NodeType";
        TbltWorkflowRepository.COMMANDNAME_ADD = "ADD";
        TbltWorkflowRepository.COMMANDNAME_EDIT = "EDIT";
        TbltWorkflowRepository.COMMANDNAME_REMOVE = "REMOVE";
        TbltWorkflowRepository.TAG_PANEL = "Tag";
        TbltWorkflowRepository.STARTUP_PANEL = "Startup";
        TbltWorkflowRepository.INPUT_ARG_PANEL = "Input";
        TbltWorkflowRepository.EVALUATION_ARG_PANEL = "Evaluation";
        TbltWorkflowRepository.OUTPUT_ARG_PANEL = "Output";
        //_dsw_p_WorkflowStartProposer - Proponente Di Avio (values 0<Settore> or 1<Utente>)
        TbltWorkflowRepository.PROPERTY_NAME_WORKFLOW_START_PROPOSER = WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_PROPOSER;
        //_dsw_p_WorkflowStartRecipient - Destinatario Di Avio (values 0<Settore> or 1<Utente>)
        TbltWorkflowRepository.PROPERTY_NAME_WORKFLOW_START_RECIPIENT = WorkflowPropertyHelper.DSW_PROPERTY_WORKFLOW_START_RECIPIENT;
        //_dsw_p_WorkflowDefaultProposer - Proponente Di Default
        TbltWorkflowRepository.PROPERTY_NAME_WORKFLOW_DEFAULT_PROPOSER = WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_DEFAULT;
        //_dsw_p_WorkflowDefaultRecipient - Destinatario Di Default
        TbltWorkflowRepository.PROPERTY_NAME_WORKFLOW_DEFAULT_RECIPIENT = WorkflowPropertyHelper.DSW_PROPERTY_RECIPIENT_DEFAULT;
        return TbltWorkflowRepository;
    }());
    return TbltWorkflowRepository;
});
//# sourceMappingURL=TbltWorkflowRepository.js.map