/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Models/Templates/TemplateDocumentRepositoryStatus", "App/Helpers/ServiceConfigurationHelper", "App/Services/Templates/TemplateDocumentRepositoryService", "UserControl/uscTemplateDocumentRepository", "App/ViewModels/Telerik/RadTreeNodeViewModel", "App/DTOs/ExceptionDTO"], function (require, exports, TemplateDocumentRepositoryStatus, ServiceConfigurationHelper, TemplateDocumentRepositoryService, uscTemplateDocumentRepository, RadTreeNodeViewModel, ExceptionDTO) {
    var TbltTemplateDocumentRepository = /** @class */ (function () {
        /**
         * Costruttore
         * @param serviceConfiguration
         */
        function TbltTemplateDocumentRepository(serviceConfigurations) {
            var _this = this;
            /**
            *------------------------- Events -----------------------------
            */
            /**
             * Evento scatenato al click del pulsante Aggiungi
             * @param sender
             * @param eventArgs
             */
            this.btnAggiungi_Clicked = function (sender, eventArgs) {
                _this._manager.add_close(_this.closeInsertWindow);
                _this._manager.open('../Tblt/TbltTemplateDocumentRepositoryGes.aspx?Action=Insert', 'windowManageTemplate', null);
                _this._manager.center();
            };
            /**
             * Evento scatenato al click del pulsante Modifica
             * @param sender
             * @param eventArgs
             */
            this.btnModifica_Clicked = function (sender, eventArgs) {
                var userControl = $('#'.concat(_this.uscTemplateDocumentRepositoryId)).data();
                var selectedTemplate = userControl.getSelectedTemplateDocument();
                if (selectedTemplate == undefined) {
                    _this.showWarningMessage(_this.uscNotificationId, 'Selezionare almeno un template');
                    return;
                }
                _this._manager.add_close(_this.closeEditWindow);
                _this._manager.open('../Tblt/TbltTemplateDocumentRepositoryGes.aspx?Action=Edit&TemplateId='.concat(selectedTemplate.UniqueId, "&TemplateIdArchiveChain=", selectedTemplate.IdArchiveChain), 'windowManageTemplate', null);
                _this._manager.center();
            };
            /**
             * Evento scatenato al click del pulsante Visualizza
             * @param sender
             * @param eventArgs
             */
            this.btnVisualizza_Clicked = function (sender, eventArgs) {
                var userControl = $('#'.concat(_this.uscTemplateDocumentRepositoryId)).data();
                var selectedTemplate = userControl.getSelectedTemplateDocument();
                if (selectedTemplate == undefined) {
                    _this.showNotificationMessage(_this.uscNotificationId, 'Selezionare almeno un template');
                    return;
                }
                var url = "../Viewers/TemplateDocumentViewer.aspx?IsPreview=false&IdChain=".concat(selectedTemplate.IdArchiveChain, "&Label=", selectedTemplate.Name);
                window.location.href = url;
            };
            /**
             * Evento scatenato al click del pulsante Elimina
             * @param sender
             * @param eventArgs
             */
            this.btnElimina_Clicked = function (sender, eventArgs) {
                var userControl = $('#'.concat(_this.uscTemplateDocumentRepositoryId)).data();
                var templateToDelete = userControl.getSelectedTemplateDocument();
                if (templateToDelete == undefined) {
                    _this.showNotificationMessage(_this.uscNotificationId, 'Selezionare almeno un elemento');
                    return;
                }
                _this._manager.remove_close(_this.closeEditWindow);
                _this._manager.remove_close(_this.closeInsertWindow);
                _this._manager.radconfirm("Sei sicuro di voler eliminare l'elemento selezionato?", function (arg) {
                    if (arg) {
                        _this.showLoadingPanel(_this.splitterPageId);
                        $find(_this.ajaxManagerId).ajaxRequestWithTarget(_this.btnEliminaUniqueId, templateToDelete.IdArchiveChain);
                    }
                }, 300, 160);
            };
            /**
             * Evento scatenato al click del pulsante Log
             * @param sender
             * @param eventArgs
             */
            this.btnLog_Clicked = function (sender, eventArgs) {
                var url = "../Tblt/TbltLog.aspx?Type=Comm&TableName=TemplateDocumentRepository";
                var userControl = $('#'.concat(_this.uscTemplateDocumentRepositoryId)).data();
                var selectedTemplate = userControl.getSelectedTemplateDocument();
                if (selectedTemplate != null) {
                    url += "&entityUniqueId=".concat(selectedTemplate.UniqueId);
                }
                _this._manager.open(url, 'windowLogTemplate', null);
                _this._manager.center();
            };
            this.closeInsertWindow = function (sender, args) {
                if (args.get_argument() != null) {
                    var userControl = $('#'.concat(_this.uscTemplateDocumentRepositoryId)).data();
                    userControl.refreshTemplates(true);
                }
            };
            this.closeEditWindow = function (sender, args) {
                if (args.get_argument() != undefined) {
                    var userControl = $('#'.concat(_this.uscTemplateDocumentRepositoryId)).data();
                    var entity = {};
                    entity = JSON.parse(args.get_argument());
                    userControl.refreshTemplates(true);
                    _this.setDetailPanelControls(entity);
                }
            };
            var serviceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "TemplateDocumentRepository");
            if (!serviceConfiguration) {
                this.showNotificationMessage(this.uscNotificationId, "Errore in inizializzazione. Nessun servizio configurato per il Deposito Documentale");
                return;
            }
            this._templateDocumentRepositoryService = new TemplateDocumentRepositoryService(serviceConfiguration);
        }
        /**
        *------------------------- Methods -----------------------------
        */
        /**
         * Metodo di inizializzazione
         */
        TbltTemplateDocumentRepository.prototype.initialize = function () {
            var _this = this;
            this.hideDetailsPanel();
            this._btnAggiungi = $find(this.btnAggiungiId);
            this._btnAggiungi.add_clicked(this.btnAggiungi_Clicked);
            this._btnModifica = $find(this.btnModificaId);
            this._btnModifica.add_clicked(this.btnModifica_Clicked);
            this._btnVisualizza = $find(this.btnVisualizzaId);
            this._btnVisualizza.add_clicked(this.btnVisualizza_Clicked);
            this._btnElimina = $find(this.btnEliminaId);
            this._btnElimina.add_clicked(this.btnElimina_Clicked);
            this._btnLog = $find(this.btnLogId);
            this._btnLog.add_clicked(this.btnLog_Clicked);
            this._previewSplitter = $find(this.previewSplitterId);
            this._previewPane = $find(this.previewPaneId);
            this._manager = $find(this.managerId);
            this.showLoadingPanel(this.splitterPageId);
            //parte caricamento treeview e dati gia presenti in tabella
            $("#".concat(this.uscTemplateDocumentRepositoryId)).on(uscTemplateDocumentRepository.ON_SELECTED_NODE_EVENT, function (args, data) {
                if (data != undefined) {
                    try {
                        var node = new RadTreeNodeViewModel();
                        node.fromJson(data);
                        if (!!node.value) {
                            _this.loadTemplateDocumentRepositoryDetails(node.attributes.UniqueId);
                            _this.setButtonVisibility(true);
                            _this.setPreviewSize();
                        }
                        else {
                            _this.hideDetailsPanel();
                            _this.setButtonVisibility(false);
                        }
                    }
                    catch (error) {
                        _this.showNotificationMessage(_this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
                        console.log(error.message);
                    }
                }
            });
            $("#".concat(this.uscTemplateDocumentRepositoryId)).on(uscTemplateDocumentRepository.ON_START_LOAD_EVENT, function (args) {
                _this.hideDetailsPanel();
                _this.showLoadingPanel(_this.splitterPageId);
            });
            $("#".concat(this.uscTemplateDocumentRepositoryId)).on(uscTemplateDocumentRepository.ON_END_LOAD_EVENT, function (args) {
                _this.hideLoadingPanel(_this.splitterPageId);
            });
            $("#".concat(this.uscTemplateDocumentRepositoryId)).on(uscTemplateDocumentRepository.ON_ERROR_EVENT, function (args, error) {
                _this.showNotificationMessage(_this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
                console.log(error.message);
                _this.hideLoadingPanel(_this.splitterPageId);
            });
            this.setPreviewSize();
            $(window).resize(function (eventObject) {
                if (_this._resizeTO)
                    clearTimeout(_this._resizeTO);
                _this._resizeTO = setTimeout(function () {
                    _this.setPreviewSize();
                }, 500);
            });
        };
        TbltTemplateDocumentRepository.prototype.setPreviewSize = function () {
            var panelInformationHeight = $('#'.concat(this.pnlInformationsId)).height();
            var contentHeight = $('#divContent').height();
            var previewHeightComputed = contentHeight - (panelInformationHeight + 85);
            this._previewSplitter.set_height(previewHeightComputed.toString().concat('px'));
            this._previewSplitter.set_width($('.rpTemplate').width().toString().concat('px'));
        };
        /**
         * Visualizza un nuovo loading panel nella pagina
         */
        TbltTemplateDocumentRepository.prototype.showLoadingPanel = function (updatedElementId) {
            var ajaxDefaultLoadingPanel = $find(this.ajaxLoadingPanelId);
            ajaxDefaultLoadingPanel.show(updatedElementId);
        };
        /**
         * Nasconde il loading panel nella pagina
         */
        TbltTemplateDocumentRepository.prototype.hideLoadingPanel = function (updatedElementId) {
            var ajaxDefaultLoadingPanel = $find(this.ajaxLoadingPanelId);
            ajaxDefaultLoadingPanel.hide(updatedElementId);
        };
        /**
         * Nasconde il pannello dei dettagli
         */
        TbltTemplateDocumentRepository.prototype.hideDetailsPanel = function () {
            $('#'.concat(this.pnlDetailsId)).hide();
        };
        /**
         * Visualizza il pannello dei dettagli
         */
        TbltTemplateDocumentRepository.prototype.showDetailsPanel = function () {
            $('#'.concat(this.pnlDetailsId)).show();
        };
        /**
         * Metodo che recupera i metadati di un template e li imposta nella pagina.
         * Gestisce anche le logiche di visualizzazione dei pulsanti e pannelli nella pagina.
         * @param templateDocumentId
         */
        TbltTemplateDocumentRepository.prototype.loadTemplateDocumentRepositoryDetails = function (templateDocumentId) {
            var _this = this;
            this.showLoadingPanel(this.pnlDetailsId);
            this._templateDocumentRepositoryService.getTemplateById(templateDocumentId, function (data) {
                if (data == null)
                    return;
                var templateDocument = data;
                _this.setDetailPanelControls(templateDocument);
                var url = "../Viewers/TemplateDocumentViewer.aspx?IsPreview=true&IdChain=".concat(templateDocument.IdArchiveChain, "&Label=", templateDocument.Name);
                _this._previewPane.set_contentUrl(url);
                _this.hideLoadingPanel(_this.pnlDetailsId);
            }, function (exception) {
                _this.hideLoadingPanel(_this.pnlDetailsId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
            this.showDetailsPanel();
        };
        /**
         * Imposta i valori del Template selezionato nel pannelo dei dettagli
         * @param templateDocument
         */
        TbltTemplateDocumentRepository.prototype.setDetailPanelControls = function (templateDocument) {
            $("#".concat(this.lblVersionId)).html(templateDocument.Version.toString());
            $("#".concat(this.lblStatusId)).html(this.mappingStatusLabel(templateDocument));
            $("#".concat(this.lblObjectId)).html(templateDocument.Object);
            $("#".concat(this.lblTagsId)).html(templateDocument.QualityTag.split(';').join(', '));
            $("#".concat(this.lblIdentifierId)).html(templateDocument.UniqueId);
        };
        /**
         * Metodo che prepara il modello da passare al datasource del RadTagCloud
         * @param tags
         */
        TbltTemplateDocumentRepository.prototype.mappingStatusLabel = function (templateDocument) {
            switch (templateDocument.Status) {
                case TemplateDocumentRepositoryStatus.Available:
                    return 'Attivo';
                case TemplateDocumentRepositoryStatus.Draft:
                    return 'Bozza';
                case TemplateDocumentRepositoryStatus.NotAvailable:
                    return 'Non attivo';
            }
        };
        /**
         * Metodo che setta la visibilità dei pulsanti
         * @param templateDocumentNodeSelected
         */
        TbltTemplateDocumentRepository.prototype.setButtonVisibility = function (templateDocumentNodeSelected) {
            this._btnAggiungi.set_enabled(!templateDocumentNodeSelected);
            this._btnModifica.set_enabled(templateDocumentNodeSelected);
            this._btnElimina.set_enabled(templateDocumentNodeSelected);
            this._btnVisualizza.set_enabled(templateDocumentNodeSelected);
            this._btnLog.set_enabled(templateDocumentNodeSelected);
        };
        /**
         * Metodo che resetta lo stato dei pulsanti dato dall'ajaxificazione degli stessi
         */
        TbltTemplateDocumentRepository.prototype.resetButtonsState = function () {
            this._btnAggiungi = $find(this.btnAggiungiId);
            this._btnModifica = $find(this.btnModificaId);
            this._btnVisualizza = $find(this.btnVisualizzaId);
            this._btnElimina = $find(this.btnEliminaId);
            //Reset in quanto il pulsante viene ricreato
            this._btnElimina.add_clicked(this.btnElimina_Clicked);
            this._btnLog = $find(this.btnLogId);
        };
        /**
         * Callback per la cancellazione di un template
         */
        TbltTemplateDocumentRepository.prototype.deleteCallback = function () {
            var _this = this;
            try {
                var userControl_1 = $('#'.concat(this.uscTemplateDocumentRepositoryId)).data();
                var templateToDelete_1 = userControl_1.getSelectedTemplateDocument();
                this._templateDocumentRepositoryService.deleteTemplateDocument(templateToDelete_1, function (data) {
                    try {
                        userControl_1.removeTemplate(templateToDelete_1);
                        _this.hideDetailsPanel();
                        _this.setButtonVisibility(false);
                        _this.hideLoadingPanel(_this.splitterPageId);
                    }
                    catch (error) {
                        _this.showNotificationMessage(_this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
                        console.log(error.message);
                    }
                }, function (exception) {
                    _this.hideLoadingPanel(_this.splitterPageId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }
            catch (error) {
                this.hideLoadingPanel(this.splitterPageId);
                this.showNotificationMessage(this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
                console.log(error.message);
            }
        };
        TbltTemplateDocumentRepository.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
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
        TbltTemplateDocumentRepository.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        TbltTemplateDocumentRepository.prototype.showWarningMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showWarningMessage(customMessage);
            }
        };
        return TbltTemplateDocumentRepository;
    }());
    return TbltTemplateDocumentRepository;
});
//# sourceMappingURL=TbltTemplateDocumentRepository.js.map