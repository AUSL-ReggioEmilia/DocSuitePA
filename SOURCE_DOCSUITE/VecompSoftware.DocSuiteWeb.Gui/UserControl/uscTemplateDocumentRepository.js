/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../app/helpers/stringhelper.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Models/Templates/TemplateDocumentRepositoryStatus", "App/ViewModels/Templates/TemplateDocumentFinderViewModel", "App/Services/Templates/TemplateDocumentRepositoryService", "App/Helpers/ServiceConfigurationHelper"], function (require, exports, TemplateDocumentRepositoryStatus, TemplateDocumentFinderViewModel, TemplateDocumentRepositoryService, ServiceConfigurationHelper) {
    var uscTemplateDocumentRepository = /** @class */ (function () {
        /**
         * Costruttore
         * @param serviceConfiguration
         */
        function uscTemplateDocumentRepository(serviceConfigurations) {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            /**
             * Evento scatenato al click di un nodo
             * @param sender
             * @param eventArgs
             */
            this.treeView_ClientNodeClicked = function (sender, eventArgs) {
                var node = eventArgs.get_node();
                $("#".concat(_this.treeTemplateDocumentId)).triggerHandler(uscTemplateDocumentRepository.ON_SELECTED_NODE_EVENT, node.toJsonString());
            };
            /**
             * Evento scatenato al click di un RadButton nella toolbar di ricerca
             * @param sender
             * @param eventArgs
             */
            this.toolBar_ButtonClicked = function (sender, eventArgs) {
                $("#".concat(_this.treeTemplateDocumentId)).triggerHandler(uscTemplateDocumentRepository.ON_START_LOAD_EVENT);
                $.when(_this.loadNodes()).done(function () {
                    $("#".concat(_this.treeTemplateDocumentId)).triggerHandler(uscTemplateDocumentRepository.ON_END_LOAD_EVENT);
                }).fail(function (exception) {
                    $("#".concat(_this.treeTemplateDocumentId)).triggerHandler(uscTemplateDocumentRepository.ON_ERROR_EVENT, exception);
                });
            };
            var serviceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "TemplateDocumentRepository");
            if (!serviceConfiguration) {
                this.showNotificationMessage(this.uscNotificationId, "Errore in inizializzazione. Nessun servizio configurato per il Deposito Documentale");
                return;
            }
            this._service = new TemplateDocumentRepositoryService(serviceConfiguration);
        }
        /**
         *------------------------- Methods -----------------------------
         */
        /**
         * Metodo di inizializzazione
         */
        uscTemplateDocumentRepository.prototype.initialize = function () {
            var _this = this;
            this._treeTemplateDocument = $find(this.treeTemplateDocumentId);
            this._toolBarSearch = $find(this.toolBarSearchId);
            this._toolBarSearch.add_buttonClicked(this.toolBar_ButtonClicked);
            this._toolBarTags = $find(this.toolBarTagsId);
            this._racTagsDataSource = $find(this.racTagsDataSourceId);
            $("#".concat(this.treeTemplateDocumentId)).data(this);
            $("#".concat(this.treeTemplateDocumentId)).triggerHandler(uscTemplateDocumentRepository.ON_START_LOAD_EVENT);
            $.when(this.loadNodes(), this.loadTags()).done(function () {
                $("#".concat(_this.treeTemplateDocumentId)).triggerHandler(uscTemplateDocumentRepository.ON_END_LOAD_EVENT);
            }).fail(function (exception) {
                $("#".concat(_this.treeTemplateDocumentId)).triggerHandler(uscTemplateDocumentRepository.ON_ERROR_EVENT, exception);
            });
        };
        /**
         * Imposta gli attributi di un nodo
         * @param node
         * @param templateDocument
         */
        uscTemplateDocumentRepository.prototype.setNodeAttribute = function (node, templateDocument) {
            node.get_attributes().setAttribute("UniqueId", templateDocument.UniqueId);
            node.get_attributes().setAttribute("Status", templateDocument.Status);
            node.get_attributes().setAttribute("Name", templateDocument.Name);
            node.get_attributes().setAttribute("QualityTag", templateDocument.QualityTag);
            node.get_attributes().setAttribute("Version", templateDocument.Version);
            node.get_attributes().setAttribute("Object", templateDocument.Object);
            node.get_attributes().setAttribute("IdArchiveChain", templateDocument.IdArchiveChain);
            return node;
        };
        /**
         * Esegue la ricerca dei Template esistenti in base ai filtri impostati
         */
        uscTemplateDocumentRepository.prototype.loadNodes = function () {
            var _this = this;
            var promise = $.Deferred();
            try {
                var finder = this.getFinder();
                this._service.findTemplateDocument(finder, function (data) {
                    try {
                        if (data == undefined) {
                            promise.resolve();
                            return;
                        }
                        _this.setNodes(data, false);
                        promise.resolve();
                    }
                    catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                }, function (exception) {
                    promise.reject(exception);
                });
            }
            catch (error) {
                console.log(error.stack);
                promise.reject(error);
            }
            return promise.promise();
        };
        /**
         * Esegue il caricamento dei Quality Tag per la funzionalità di ricerca
         */
        uscTemplateDocumentRepository.prototype.loadTags = function () {
            var _this = this;
            var promise = $.Deferred();
            try {
                this._service.getTags(function (data) {
                    try {
                        if (data == undefined) {
                            promise.resolve();
                            return;
                        }
                        var sourceModel = _this.prepareTagsSourceModel(data.value);
                        _this._racTagsDataSource.set_data(sourceModel);
                        _this._racTagsDataSource.fetch(undefined);
                        promise.resolve();
                    }
                    catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                }, function (exception) {
                    promise.reject(exception);
                });
            }
            catch (error) {
                console.log(error.stack);
                promise.reject(error);
            }
            return promise.promise();
        };
        /**
         * Metodo che prepara il modello da passare al datasource della RadAutoCompleteBox
         * @param tags
         */
        uscTemplateDocumentRepository.prototype.prepareTagsSourceModel = function (tags) {
            var models = new Array();
            $.each(tags, function (index, tag) {
                models.push({ id: tag, value: tag });
            });
            return models;
        };
        /**
         * Crea e imposta i nodi nella RadTreeView di visualizzazione
         * @param templates
         */
        uscTemplateDocumentRepository.prototype.setNodes = function (templates, append) {
            var _this = this;
            var rootNode = this._treeTemplateDocument.get_nodes().getNode(0);
            if (append == false) {
                rootNode.get_nodes().clear();
            }
            $.each(templates, function (index, template) {
                //Verifico se il nodo già esiste nella treeview
                if (_this._treeTemplateDocument.findNodeByValue(template.UniqueId) != undefined) {
                    return;
                }
                var newNode = new Telerik.Web.UI.RadTreeNode();
                newNode.set_text(template.Name);
                newNode.set_value(template.UniqueId);
                switch (+template.Status) {
                    case TemplateDocumentRepositoryStatus.Available:
                        newNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/template_active.png");
                        break;
                    case TemplateDocumentRepositoryStatus.Draft:
                        newNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/template_draft.png");
                        break;
                    case TemplateDocumentRepositoryStatus.NotAvailable:
                        newNode.set_imageUrl("../App_Themes/DocSuite2008/imgset16/template_not_active.png");
                        newNode.set_cssClass('node-disabled');
                        break;
                }
                _this.setNodeAttribute(newNode, template);
                rootNode.get_nodes().add(newNode);
            });
            rootNode.set_expanded(true);
            this._treeTemplateDocument.commitChanges();
        };
        /**
         * Prepara il finder per la ricerca
         */
        uscTemplateDocumentRepository.prototype.getFinder = function () {
            var txtSearchDescription = this._toolBarSearch.findItemByValue('searchDescription').findControl('txtTemplateName');
            var racTags = this._toolBarTags.findItemByValue('searchTags').findControl('racTags');
            var finder = new TemplateDocumentFinderViewModel();
            finder.Name = txtSearchDescription.get_value();
            if (this.onlyPublishedTemplate) {
                finder.Status.push(TemplateDocumentRepositoryStatus.Available);
            }
            else {
                finder.Status.push(TemplateDocumentRepositoryStatus.Available);
                finder.Status.push(TemplateDocumentRepositoryStatus.Draft);
            }
            var entries = racTags.get_entries();
            if (entries.get_count() > 0) {
                for (var i = 0; i < entries.get_count(); i++) {
                    finder.Tags.push(entries.getEntry(i).get_value());
                }
            }
            return finder;
        };
        /**
         * Recupera il modello dal nodo selezionato nella treeview
         */
        uscTemplateDocumentRepository.prototype.getSelectedTemplateDocument = function () {
            var selectedNode = this._treeTemplateDocument.get_selectedNode();
            if (selectedNode == undefined || !selectedNode.get_value()) {
                return undefined;
            }
            var model = {};
            model.UniqueId = selectedNode.get_attributes().getAttribute("UniqueId");
            if (selectedNode.get_attributes().getAttribute("Status") != undefined) {
                model.Status = TemplateDocumentRepositoryStatus[selectedNode.get_attributes().getAttribute("Status")];
            }
            model.Name = selectedNode.get_attributes().getAttribute("Name");
            model.QualityTag = selectedNode.get_attributes().getAttribute("QualityTag");
            model.Version = selectedNode.get_attributes().getAttribute("Version");
            model.Object = selectedNode.get_attributes().getAttribute("Object");
            model.IdArchiveChain = selectedNode.get_attributes().getAttribute("IdArchiveChain");
            return model;
        };
        /**
         * Rimuove uno specifico nodo dalla treeview
         * @param template
         */
        uscTemplateDocumentRepository.prototype.removeTemplate = function (template) {
            var node = this._treeTemplateDocument.findNodeByValue(template.UniqueId);
            if (node == undefined) {
                console.warn("Nessun nodo trovato con ID '".concat(template.UniqueId, "'"));
                return;
            }
            node.get_parent().get_nodes().remove(node);
        };
        /**
         * Aggiorna uno specifico nodo della treeview
         * @param template
         */
        uscTemplateDocumentRepository.prototype.refreshTemplates = function (selectCurrentNode) {
            var _this = this;
            var selectedTemplate = this.getSelectedTemplateDocument();
            $.when(this.loadNodes()).done(function () {
                if (selectCurrentNode) {
                    var node = _this._treeTemplateDocument.findNodeByValue(selectedTemplate.UniqueId);
                    if (node == undefined) {
                        console.warn("Nessun nodo trovato con ID '".concat(selectedTemplate.UniqueId, "'"));
                        return;
                    }
                    node.select();
                }
            }).fail(function (exception) {
                $("#".concat(_this.treeTemplateDocumentId)).triggerHandler(uscTemplateDocumentRepository.ON_ERROR_EVENT, exception);
            });
        };
        uscTemplateDocumentRepository.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        uscTemplateDocumentRepository.ON_SELECTED_NODE_EVENT = "onSelectedNode";
        uscTemplateDocumentRepository.ON_START_LOAD_EVENT = "onStartLoad";
        uscTemplateDocumentRepository.ON_END_LOAD_EVENT = "onEndLoad";
        uscTemplateDocumentRepository.ON_ERROR_EVENT = "onErrorEvent";
        return uscTemplateDocumentRepository;
    }());
    return uscTemplateDocumentRepository;
});
//# sourceMappingURL=uscTemplateDocumentRepository.js.map