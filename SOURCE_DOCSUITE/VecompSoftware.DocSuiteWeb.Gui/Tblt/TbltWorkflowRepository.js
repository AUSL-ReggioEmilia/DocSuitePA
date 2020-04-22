/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />
define(["require", "exports", "App/Models/Workflows/WorkflowEvalutionPropertyHelper", "App/Services/Workflows/WorkflowRoleMappingService", "App/Services/Workflows/WorkflowRepositoryService", "App/Services/Securities/DomainUserService", "App/Helpers/ServiceConfigurationHelper", "App/Models/Workflows/WorkflowAuthorizationType", "App/Models/Workflows/WorkflowStatus", "App/DTOs/ExceptionDTO", "App/Services/Workflows/WorkflowEvaluationPropertyService", "../app/core/extensions/string"], function (require, exports, WorkflowEvalutionPropertyHelper, WorkflowRoleMappingService, WorkflowRepositoryService, DomainUserService, ServiceConfigurationHelper, WorkflowAuthorizationType, WorkflowStatus, ExceptionDTO, WorkflowEvaluationPropertyService) {
    var TbltWorkflowRepository = /** @class */ (function () {
        /**
         * Costruttore
         * @param serviceConfigurations
         */
        function TbltWorkflowRepository(serviceConfigurations) {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            /**
             * Evento scatenato alla selezione di un workflow
             * @param sender
             * @param args
             */
            this.rtvWorkflowRepository_nodeClicked = function (sender, args) {
                $('#'.concat(_this.pnlDetailsId)).show();
                $('#'.concat(_this.btnModificaId)).show();
                $('#'.concat(_this.btnEliminaId)).show();
                _this.loadDetails();
            };
            /**
             * Evento scatenato al click del pulsante aggiungi
             * @param sender
             * @param args
             */
            this.btnAggiungi_onClick = function (sender, args) {
                _this.openManagementWindow('Add');
                return false;
            };
            /**
             * Evento scatenato al click del pulsante modifica
             * @param sender
             * @param args
             */
            this.btnModifica_onClick = function (sender, args) {
                _this.openManagementWindow('Edit');
                return false;
            };
            /**
             * Evento scatenato al click del pulsante elimina
             * @param sender
             * @param args
             */
            this.btnDelete_onClick = function (sender, args) {
                var selectedNode = _this._rtvWorkflowRepository.get_selectedNode();
                var isXamlRepository = selectedNode.get_attributes().getAttribute('isXaml');
                var selectedMappingTags;
                if (isXamlRepository) {
                    selectedMappingTags = _this._rgvXamlWorkflowRoleMappings.get_selectedItems();
                }
                else {
                    selectedMappingTags = _this._rgvWorkflowRoleMappings.get_selectedItems();
                }
                if (selectedMappingTags == undefined || selectedMappingTags.length == 0) {
                    _this.showWarningMessage(_this.uscNotificationId, 'Nessun Tag selezionato');
                    return false;
                }
                var model;
                if (isXamlRepository) {
                    var workflowActivityModel = selectedMappingTags[0].get_dataItem();
                    if (workflowActivityModel.WorkflowRoleMapping.UniqueId == undefined) {
                        _this.showNotificationMessage(_this.uscNotificationId, "Nessuna definizione di autorizzazione associata all'activity selezionata.");
                        return;
                    }
                    model = { UniqueId: workflowActivityModel.WorkflowRoleMapping.UniqueId };
                }
                else {
                    model = { UniqueId: selectedMappingTags[0].get_dataItem().UniqueId };
                }
                _this._workflowRoleMappingService.deleteWorkflowRoleMapping(model, function (data) { return _this.loadDetails(); }, function (exception) {
                    _this.showNotificationException(_this.uscNotificationId, exception);
                    return;
                });
            };
            /**
             * Evento scatenato al click del pulsante di ricerca workflow
             * @param sender
             * @param args
             */
            this.toolbarSearch_onClick = function (sender, args) {
                _this._loadingPanel.show(_this.rtvWorkflowRepositoryId);
                _this._workflowRepositoryService.getByName(_this._txtSearchName.get_textBoxValue(), function (data) {
                    _this.loadWorkflowRepositories(data);
                    _this._loadingPanel.hide(_this.rtvWorkflowRepositoryId);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.rtvWorkflowRepositoryId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
                return false;
            };
            /**
             * Evento scatenato al click del pulsante di ricerca Tag per workflow di tipo XAML
             * @param sender
             * @param args
             */
            this.btnSelectMappingTag_onClick = function (sender, args) {
                var selectedMappingText = _this._rcbSelectMappingTag.get_text();
                if (String.isNullOrEmpty(selectedMappingText) || selectedMappingText == _this._rcbSelectMappingTag.get_emptyMessage())
                    return;
                var selectedNode = _this._rtvWorkflowRepository.get_selectedNode();
                _this._workflowRepositoryService.getById(selectedNode.get_value(), function (response) {
                    if (response == undefined)
                        return;
                    try {
                        var model = response;
                        var customActivities = JSON.parse(model.CustomActivities);
                        if (customActivities.length == 0) {
                            _this.showWarningMessage(_this.uscNotificationId, 'Nessuna activity configurata per il Workflow selezionato.');
                            return;
                        }
                        _this.fillXamlMappings(model, customActivities, selectedMappingText);
                    }
                    catch (error) {
                        _this.showNotificationMessage(_this.uscNotificationId, error.message);
                        console.log(JSON.stringify(error));
                    }
                }, function (exception) {
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            /**
             * Evento scatenato alla chiusura della finestra di gestione mapping
             * @param sender
             * @param args
             */
            this.windowAddWorkflowRoleMapping_onClose = function (sender, args) {
                if (args.get_argument() != undefined) {
                    _this.loadDetails();
                }
            };
            this.btnAdd_onClick = function (sender, args) {
                _this.openWorkflowEvaluationPropertiesManagementWindow("Add");
            };
            this.btnEdit_onClick = function (sender, args) {
                _this.openWorkflowEvaluationPropertiesManagementWindow("Edit");
            };
            this.btnDeleteStartup_onClick = function (sender, args) {
                var selectedEvaluationPropertyId = _this._rgvWorkflowStartUp.get_selectedItems();
                if (selectedEvaluationPropertyId == undefined || selectedEvaluationPropertyId.length == 0) {
                    _this.showWarningMessage(_this.uscNotificationId, 'Nessun proprietà selezionata');
                    return;
                }
                if (selectedEvaluationPropertyId.length > 1) {
                    _this.showWarningMessage(_this.uscNotificationId, 'Seleziona una sola proprietà per la modifica');
                    return;
                }
                var workflowEvaluationModel;
                var workflowEvaluationModelUniqueId = selectedEvaluationPropertyId[0].get_dataItem().UniqueId;
                workflowEvaluationModel = { UniqueId: workflowEvaluationModelUniqueId };
                _this._workflowEvaluationPropertyService.deleteWorkflowEvaluationProperty(workflowEvaluationModel, function (data) {
                    _this.loadDetails();
                }, function (exception) {
                    console.log(exception);
                });
            };
            this._serviceConfigurations = serviceConfigurations;
        }
        /**
         * Inizializzazione classe
         */
        TbltWorkflowRepository.prototype.initialize = function () {
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._windowAddWorkflowRoleMapping = $find(this.windowAddWorkflowRoleMappingId);
            this._windowAddWorkflowRoleMapping.add_close(this.windowAddWorkflowRoleMapping_onClose);
            this._rtvWorkflowRepository = $find(this.rtvWorkflowRepositoryId);
            this._rtvWorkflowRepository.add_nodeClicked(this.rtvWorkflowRepository_nodeClicked);
            this._rgvWorkflowRoleMappings = $find(this.rgvWorkflowRoleMappingsId);
            this._rgvXamlWorkflowRoleMappings = $find(this.rgvXamlWorkflowRoleMappingsId);
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
            this._toolbarSearch = $find(this.ToolBarSearchId);
            this._toolbarSearch.add_buttonClicking(this.toolbarSearch_onClick);
            this._toolbarItemSearchName = this._toolbarSearch.findItemByValue("searchName");
            this._txtSearchName = this._toolbarItemSearchName.findControl("txtName");
            this._toolbarItemButtonSearch = this._toolbarSearch.findItemByValue("searchCommand");
            var workflowRoleMappingConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowRoleMapping');
            this._workflowRoleMappingService = new WorkflowRoleMappingService(workflowRoleMappingConfiguration);
            var workflowRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowRepository');
            this._workflowRepositoryService = new WorkflowRepositoryService(workflowRepositoryConfiguration);
            var domainUserConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'DomainUserModel');
            this._domainUserService = new DomainUserService(domainUserConfiguration);
            var workflowEvaluationProperty = ServiceConfigurationHelper.getService(this._serviceConfigurations, "WorkflowEvaluationProperty");
            this._workflowEvaluationPropertyService = new WorkflowEvaluationPropertyService(workflowEvaluationProperty);
            $('#'.concat(this.pnlDetailsId)).hide();
            $('#'.concat(this.btnAggiungiId)).hide();
            $('#'.concat(this.btnModificaId)).hide();
            $('#'.concat(this.btnEliminaId)).hide();
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
            var _this = this;
            if (repositories == undefined || repositories.length == 0) {
                return;
            }
            try {
                this._rtvWorkflowRepository.beginUpdate();
                this._rtvWorkflowRepository.get_nodes().clear();
                $.each(repositories, function (index, repository) {
                    var node = new Telerik.Web.UI.RadTreeNode();
                    node.get_attributes().setAttribute('isXaml', !String.isNullOrEmpty(repository.Xaml));
                    node.set_text(repository.Name);
                    node.set_value(repository.UniqueId);
                    node.set_imageUrl('../Comm/Images/DocSuite/Workflow16.png');
                    _this._rtvWorkflowRepository.get_nodes().add(node);
                });
                this._rtvWorkflowRepository.endUpdate();
            }
            catch (error) {
                this.showNotificationMessage(this.uscNotificationId, error.message);
                console.log(JSON.stringify(error));
            }
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
            var _this = this;
            var result = $.Deferred();
            var selectedNode = this._rtvWorkflowRepository.get_selectedNode();
            this._rcbSelectMappingTag.clearItems();
            this._workflowRoleMappingService.getByName('', selectedNode.get_value(), function (response) {
                if (response == undefined)
                    return result.resolve();
                try {
                    var models = response;
                    if (models.length > 0) {
                        _this._mappingDataSource.set_data(models);
                    }
                    else {
                        _this._mappingDataSource.set_data('[{}]');
                    }
                    _this._mappingDataSource.fetch();
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
            var _this = this;
            var result = $.Deferred();
            var selectedNode = this._rtvWorkflowRepository.get_selectedNode();
            this._workflowRepositoryService.getById(selectedNode.get_value(), function (response) {
                if (response == undefined)
                    return result.reject();
                try {
                    var model = response;
                    $('#'.concat(_this.lblStatusId)).html(_this.getStatusDescription(model.Status.toString()));
                    $('#'.concat(_this.lblVersionId)).html(model.Version.toString());
                    if (model.ActiveTo != undefined) {
                        $('#'.concat(_this.lblActiveToId)).html(moment(model.ActiveTo).format("DD/MM/YYYY"));
                    }
                    $('#'.concat(_this.lblActiveFromId)).html(moment(model.ActiveFrom).format("DD/MM/YYYY"));
                    _this.clearDataSources();
                    _this.setDetailsVisibility(model);
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
        TbltWorkflowRepository.prototype.resolveDomainUsers = function (roleMappings) {
            var _this = this;
            var promises = new Array();
            $.each(roleMappings, function (index, mapping) {
                var deferred = $.Deferred();
                if (mapping.AuthorizationType.toString() != WorkflowAuthorizationType[WorkflowAuthorizationType.UserName])
                    return;
                _this._domainUserService.getUser(mapping.AccountName, function (response) {
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
            var _this = this;
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
                    var xamlWorkflowRoleMappingsMasterTableView = _this._rgvXamlWorkflowRoleMappings.get_masterTableView();
                    xamlWorkflowRoleMappingsMasterTableView.set_dataSource(sourceModel);
                    xamlWorkflowRoleMappingsMasterTableView.clearSelectedItems();
                    xamlWorkflowRoleMappingsMasterTableView.dataBind();
                    result.resolve();
                }
                catch (error) {
                    _this.showNotificationMessage(_this.uscNotificationId, error.message);
                    console.log(JSON.stringify(error));
                    result.reject(error);
                }
            }).fail(function (exception) { _this.showNotificationException(_this.uscNotificationId, exception); });
            return result.promise();
        };
        /**
         * Carica i dettagli del workflow selezionato
         */
        TbltWorkflowRepository.prototype.loadDetails = function () {
            var _this = this;
            this._loadingPanel.show(this.pnlDetailsId);
            //Gestito JQueryPromise che simula la gestion async dei metodi (IE8 supportato)     
            $.when(this.fillDetailsPanel()).done(function (model) {
                var activities = !String.isNullOrEmpty(model.CustomActivities) ? JSON.parse(model.CustomActivities) : [];
                var selectedTag = (!String.isNullOrEmpty(_this._rcbSelectMappingTag.get_text()) && _this._rcbSelectMappingTag.get_text() != _this._rcbSelectMappingTag.get_emptyMessage()) ? _this._rcbSelectMappingTag.get_text() : '';
                $.when(_this.fillMappingSelection(), _this.fillJsonMappings(model), _this.fillXamlMappings(model, activities, selectedTag)).always(function () {
                    _this._loadingPanel.hide(_this.pnlDetailsId);
                }).fail(function (exception) { _this.showNotificationException(_this.uscNotificationId, exception); });
            }).fail(function (exception) {
                _this._loadingPanel.hide(_this.pnlDetailsId);
                _this.showNotificationException(_this.uscNotificationId, exception, "Errore nel caricamento dell'attività.");
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
            return WorkflowEvalutionPropertyHelper[name].Name;
        };
        /**
         * Esegue il mapping delle WorkflowStatus
         * @param status
         */
        TbltWorkflowRepository.prototype.getStatusDescription = function (status) {
            switch (WorkflowStatus[status]) {
                case WorkflowStatus.Done:
                    return 'Eseguita';
                case WorkflowStatus.Error:
                    return 'In errore';
                case WorkflowStatus.Progress:
                    return 'In esecuzione';
                case WorkflowStatus.Suspended:
                    return 'Sospesa';
                case WorkflowStatus.Todo:
                    return 'Attiva';
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
            if (operation === "Add") {
                this._windowAddWorkflowRoleMapping.setUrl("TbltWorkflowEvaluationPropertyGes.aspx?Action=" + operation + "&WorkflowRepositoryId=" + selectedNode.get_value());
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
                this._windowAddWorkflowRoleMapping.setUrl("TbltWorkflowEvaluationPropertyGes.aspx?Action=" + operation + "&WorkflowRepositoryId=" + selectedNode.get_value() + "&WorkflowEvaluationPropertyId=" + workflowEvaluationModel);
            }
            this._windowAddWorkflowRoleMapping.set_modal(true);
            this._windowAddWorkflowRoleMapping.show();
        };
        return TbltWorkflowRepository;
    }());
    return TbltWorkflowRepository;
});
//# sourceMappingURL=TbltWorkflowRepository.js.map