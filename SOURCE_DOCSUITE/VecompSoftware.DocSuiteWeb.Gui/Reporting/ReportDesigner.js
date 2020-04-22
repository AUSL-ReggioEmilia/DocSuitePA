/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "UserControl/uscReportDesignerInformation", "UserControl/uscReportDesigner", "UserControl/uscReportDesignerToolbox", "App/Models/Environment", "App/ViewModels/Reports/ReportInformationViewModel", "App/Models/Reports/ReportBuilderModel", "App/Models/Reports/ReportBuilderProjectionModel", "App/Models/Reports/ReportBuilderConditionModel", "App/Models/Reports/ReportBuilderPropertyModel", "App/ViewModels/Reports/ReportToolboxItemType", "App/Mappers/Reports/ReportBuilderPropertyModelMapper", "App/Mappers/Reports/ReportBuilderConditionModelMapper", "App/Services/Templates/TemplateReportService", "App/Models/Templates/TemplateReportModel", "App/Models/Templates/TemplateReportStatus", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO", "App/Models/Commons/MetadataRepositoryModel", "App/Mappers/Reports/ReportInformationViewModelMapper"], function (require, exports, UscReportDesignerInformation, UscReportDesigner, UscReportDesignerToolbox, Environment, ReportInformationViewModel, ReportBuilderModel, ReportBuilderProjectionModel, ReportBuilderConditionModel, ReportBuilderPropertyModel, ReportToolboxItemType, ReportBuilderPropertyModelMapper, ReportBuilderConditionModelMapper, TemplateReportService, TemplateReportModel, TemplateReportStatus, ServiceConfigurationHelper, ExceptionDTO, MetadataRepositoryModel, ReportInformationViewModelMapper) {
    var ReportDesigner = /** @class */ (function () {
        function ReportDesigner(serviceConfigurations) {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            this.BtnDraft_OnClick = function (sender, args) {
                if (Page_ClientValidate("ReportData")) {
                    _this.showLoading();
                    $.when(_this.saveTemplate(true))
                        .done(function () {
                    })
                        .fail(function (ex) {
                        _this.showNotification(ex);
                    })
                        .always(function () {
                        _this.hideLoading();
                    });
                }
            };
            this.BtnSave_OnClick = function (sender, args) {
                if (Page_ClientValidate("ReportData")) {
                    _this.showLoading();
                    $.when(_this.saveTemplate(false))
                        .done(function () {
                    })
                        .fail(function (ex) {
                        _this.showNotification(ex);
                    })
                        .always(function () {
                        _this.hideLoading();
                    });
                }
            };
            var service = ServiceConfigurationHelper.getService(serviceConfigurations, ReportDesigner.TEMPLATE_REPORT_NAME);
            if (!service) {
                this.showNotification("Nessun servizio configurato per la gestione dei report.");
            }
            this._service = new TemplateReportService(service);
            this._propertyMapper = new ReportBuilderPropertyModelMapper();
            this._conditionMapper = new ReportBuilderConditionModelMapper();
            this._informationViewModelMapper = new ReportInformationViewModelMapper();
        }
        Object.defineProperty(ReportDesigner.prototype, "action", {
            get: function () {
                return (!this.reportUniqueId) ? ReportDesigner.INSERT_ACTION : ReportDesigner.EDIT_ACTION;
            },
            enumerable: true,
            configurable: true
        });
        /**
         *------------------------- Methods -----------------------------
         */
        ReportDesigner.prototype.initialize = function () {
            this._btnDraft = $find(this.btnDraftId);
            this._btnDraft.add_clicked(this.BtnDraft_OnClick);
            this._btnSave = $find(this.btnSaveId);
            this._btnSave.add_clicked(this.BtnSave_OnClick);
            this._ajaxLoadingPanel = $find(this.ajaxLoadingPanelId);
            this.initializeControls();
            this.checkCompletedLoad();
        };
        ReportDesigner.prototype.checkCompletedLoad = function () {
            var _this = this;
            this.showLoading();
            $.when(this.checkLoadDesigner(), this.checkLoadInformations(), this.checkLoadToolbox())
                .done(function () {
                if (!_this.reportUniqueId) {
                    _this.hideLoading();
                    return;
                }
                $.when(_this.loadCurrentReportTemplate(_this.reportUniqueId))
                    .always(function () {
                    _this.hideLoading();
                });
            })
                .fail(function () {
                _this.hideLoading();
            });
        };
        ReportDesigner.prototype.initializeControls = function () {
            this.initializeDesignerControl();
            this.initializeInformationControl();
            this.initializeToolboxControl();
        };
        ReportDesigner.prototype.initializeDesignerControl = function () {
            var _this = this;
            $("#".concat(this.uscReportDesignerId)).bind(UscReportDesigner.ON_CHANGED_VIEW, function (args) {
                _this.loadToolbox();
            });
        };
        ReportDesigner.prototype.initializeInformationControl = function () {
            var _this = this;
            $("#".concat(this.uscReportInformationId)).bind(UscReportDesignerInformation.ON_EXECUTE_LOAD_EVENT, function (args) {
                //TODO: Gestire caricamento Tag e informazioni
                _this.loadDesigner();
                _this.loadToolbox();
            });
        };
        ReportDesigner.prototype.initializeToolboxControl = function () {
            var _this = this;
            $("#".concat(this.uscReportToolboxId)).on(UscReportDesignerToolbox.ON_NODE_DROPPING, function (evt, sender, args, item) {
                var uscReportDesigner = $("#".concat(_this.uscReportDesignerId)).data();
                uscReportDesigner.Designer_DroppingItems(sender, args, item);
            });
        };
        ReportDesigner.prototype.initializeInformations = function (model) {
            var uscReportInformations = $("#".concat(this.uscReportInformationId)).data();
            var environments = [Environment.Fascicle];
            var documentUnits = [Environment.Protocol];
            if (!model) {
                model = new ReportInformationViewModel();
            }
            model.Environments = environments;
            model.DocumentUnits = documentUnits;
            uscReportInformations.loadInformations(model);
        };
        ReportDesigner.prototype.checkLoadInformations = function () {
            var _this = this;
            var promise = $.Deferred();
            var uscReportInformations = $("#".concat(this.uscReportInformationId)).data();
            if (!jQuery.isEmptyObject(uscReportInformations)) {
                this.initializeInformations();
                promise.resolve();
            }
            $("#".concat(this.uscReportInformationId)).bind(UscReportDesignerInformation.ON_END_LOAD_EVENT, function (args) {
                _this.initializeInformations();
                promise.resolve();
            });
            return promise.promise();
        };
        ReportDesigner.prototype.checkLoadToolbox = function () {
            var promise = $.Deferred();
            var uscReportToolbox = $("#".concat(this.uscReportToolboxId)).data();
            if (!jQuery.isEmptyObject(uscReportToolbox)) {
                promise.resolve();
            }
            $("#".concat(this.uscReportToolboxId)).bind(UscReportDesignerToolbox.ON_END_LOAD_EVENT, function (args) {
                promise.resolve();
            });
            return promise.promise();
        };
        ReportDesigner.prototype.checkLoadDesigner = function () {
            var promise = $.Deferred();
            var uscReportDesigner = $("#".concat(this.uscReportDesignerId)).data();
            if (!jQuery.isEmptyObject(uscReportDesigner)) {
                promise.resolve();
            }
            $("#".concat(this.uscReportDesignerId)).bind(UscReportDesigner.ON_END_LOAD_EVENT, function (args) {
                promise.resolve();
            });
            return promise.promise();
        };
        ReportDesigner.prototype.loadCurrentReportTemplate = function (uniqueId) {
            var _this = this;
            var promise = $.Deferred();
            this._service.getById(uniqueId, function (data) {
                try {
                    var templateReport = data;
                    var reportInformationModel = _this._informationViewModelMapper.Map(templateReport);
                    _this.initializeInformations(reportInformationModel);
                    _this.loadToolbox();
                    var reportBuilderModel = JSON.parse(templateReport.ReportBuilderJsonModel);
                    _this.loadDesigner(reportBuilderModel);
                    promise.resolve();
                }
                catch (e) {
                    console.error(e);
                    promise.reject();
                }
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        ReportDesigner.prototype.loadDesigner = function (model) {
            var uscReportDesigner = $("#".concat(this.uscReportDesignerId)).data();
            if (!model) {
                model = new ReportBuilderModel();
                var condition = new ReportBuilderConditionModel();
                condition.ConditionName = "HasPecMails";
                var condition2 = new ReportBuilderConditionModel();
                condition2.ConditionName = "HasPecMails2";
                //model.Conditions = [condition, condition2];
                var projection = new ReportBuilderProjectionModel();
                var projection1 = new ReportBuilderProjectionModel();
                var property1 = new ReportBuilderPropertyModel();
                property1.Name = "Prop1";
                property1.DisplayName = "Proprietà";
                var property2 = new ReportBuilderPropertyModel();
                property2.Name = "Prop2";
                property2.DisplayName = "Proprietà 2";
                var property3 = new ReportBuilderPropertyModel();
                property3.Name = "Prop3";
                property3.DisplayName = "Proprietà 3";
                //projection.ReportProperties = [property1, property2];
                projection.Alias = "Verifica";
                projection.TagName = "Test";
                //projection1.ReportProperties = [property3];
                projection1.TagName = "Test1";
                model.Projections = [projection, projection1];
            }
            uscReportDesigner.loadDesignerModel(model);
        };
        ReportDesigner.prototype.loadToolbox = function () {
            var _this = this;
            var uscReportDesigner = $("#".concat(this.uscReportDesignerId)).data();
            var uscReportToolbox = $("#".concat(this.uscReportToolboxId)).data();
            var uscReportInformations = $("#".concat(this.uscReportInformationId)).data();
            $.when(uscReportInformations.getInformations())
                .done(function (informationModel) {
                try {
                    if (!informationModel || !informationModel.SelectedEnvironment) {
                        return;
                    }
                    var selectedView = uscReportDesigner.currentActiveView();
                    var toolboxItemType_1 = (selectedView == UscReportDesigner.PROJECTIONS_VIEW) ? ReportToolboxItemType.Projection : ReportToolboxItemType.Condition;
                    var toolboxItemViewModels = JSON.parse(JSON.stringify(_this.toolboxItems));
                    toolboxItemViewModels = toolboxItemViewModels.filter(function (m) { return m.Environment == informationModel.SelectedEnvironment && m.ItemType == toolboxItemType_1; });
                    if (informationModel.SelectedEnvironment == Environment.Fascicle && selectedView == UscReportDesigner.PROJECTIONS_VIEW) {
                        for (var _i = 0, toolboxItemViewModels_1 = toolboxItemViewModels; _i < toolboxItemViewModels_1.length; _i++) {
                            var toolboxItemViewModel = toolboxItemViewModels_1[_i];
                            toolboxItemViewModel.ReportItems = toolboxItemViewModel.ReportItems.concat(informationModel.MetadataProperties);
                        }
                    }
                    if (informationModel.SelectedDocumentUnit) {
                        var documentUnitToolboxItems = _this.toolboxItems.filter(function (m) { return m.Environment == informationModel.SelectedDocumentUnit && m.ItemType == toolboxItemType_1; });
                        for (var _a = 0, documentUnitToolboxItems_1 = documentUnitToolboxItems; _a < documentUnitToolboxItems_1.length; _a++) {
                            var documentUnitToolboxItem = documentUnitToolboxItems_1[_a];
                            toolboxItemViewModels.push(documentUnitToolboxItem);
                        }
                    }
                    for (var _b = 0, toolboxItemViewModels_2 = toolboxItemViewModels; _b < toolboxItemViewModels_2.length; _b++) {
                        var toolboxItem = toolboxItemViewModels_2[_b];
                        if (toolboxItemType_1 == ReportToolboxItemType.Projection) {
                            toolboxItem.ReportItems = _this._propertyMapper.MapCollection(toolboxItem.ReportItems);
                        }
                        else if (toolboxItemType_1 == ReportToolboxItemType.Condition) {
                            toolboxItem.ReportItems = _this._conditionMapper.MapCollection(toolboxItem.ReportItems);
                        }
                    }
                    uscReportToolbox.loadToolbox(toolboxItemViewModels);
                }
                catch (e) {
                    console.error(e);
                    _this.showNotification("E' avvenuto un errore durante la fase di caricamento elementi nella toolbox.");
                }
            })
                .fail(function () {
                _this.showNotification("E' avvenuto un errore nel recupero delle informazioni del report corrente.");
            });
        };
        ReportDesigner.prototype.showNotification = function (error) {
            var uscNotification = $("#".concat(this.uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                if (error instanceof ExceptionDTO) {
                    uscNotification.showNotification(error);
                }
                else {
                    uscNotification.showNotificationMessage(error);
                }
            }
        };
        ReportDesigner.prototype.saveTemplate = function (draft) {
            var _this = this;
            var promise = $.Deferred();
            var saveAction = (!this.reportUniqueId) ? function (m, c, e) { return _this._service.insertTemplateReport(m, c, e); } : function (m, c, e) { return _this._service.updateTemplateReport(m, c, e); };
            var uscReportDesigner = $("#".concat(this.uscReportDesignerId)).data();
            var uscReportInformations = $("#".concat(this.uscReportInformationId)).data();
            $.when(uscReportInformations.getInformations())
                .done(function (informationModel) {
                if (!informationModel.Name) {
                    var ex = new ExceptionDTO();
                    ex.statusText = "Specificare il nome del report.";
                    promise.reject(ex);
                    return;
                }
                if (!informationModel.SelectedEnvironment) {
                    var ex = new ExceptionDTO();
                    ex.statusText = "Specificare almeno una tipologia.";
                    promise.reject(ex);
                    return;
                }
                var builderModel = uscReportDesigner.getDesignerModel();
                builderModel.Entity = informationModel.SelectedEnvironment;
                builderModel.UDType = informationModel.SelectedDocumentUnit;
                var metadataRepository = new MetadataRepositoryModel();
                metadataRepository.UniqueId = informationModel.SelectedMetadata;
                builderModel.MetadataRepository = metadataRepository;
                var toSave = new TemplateReportModel();
                if (_this.action == ReportDesigner.EDIT_ACTION) {
                    toSave.UniqueId = _this.reportUniqueId;
                }
                toSave.Environment = informationModel.SelectedEnvironment;
                toSave.Name = informationModel.Name;
                toSave.Status = (draft) ? TemplateReportStatus.Draft : TemplateReportStatus.Active;
                toSave.ReportBuilderJsonModel = JSON.stringify(builderModel);
                saveAction(toSave, function (data) {
                    _this.reportUniqueId = data.UniqueId;
                    _this.loadCurrentReportTemplate(data.UniqueId);
                    promise.resolve();
                }, function (exception) {
                    promise.reject(exception);
                });
            })
                .fail(function () {
                var ex = new ExceptionDTO();
                ex.statusText = "Errore nel recupero delle informazioni del report corrente.";
                promise.reject(ex);
            });
            return promise.promise();
        };
        ReportDesigner.prototype.showLoading = function () {
            this._ajaxLoadingPanel.show(this.splPageId);
        };
        ReportDesigner.prototype.hideLoading = function () {
            this._ajaxLoadingPanel.hide(this.splPageId);
        };
        ReportDesigner.TEMPLATE_REPORT_NAME = "TemplateReport";
        ReportDesigner.INSERT_ACTION = "Insert";
        ReportDesigner.EDIT_ACTION = "Edit";
        return ReportDesigner;
    }());
    return ReportDesigner;
});
//# sourceMappingURL=ReportDesigner.js.map