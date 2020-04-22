/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Models/Templates/TemplateDocumentRepositoryStatus", "App/Helpers/ServiceConfigurationHelper", "App/Services/Templates/TemplateDocumentRepositoryService", "App/DTOs/ExceptionDTO"], function (require, exports, TemplateDocumentRepositoryStatus, ServiceConfigurationHelper, TemplateDocumentRepositoryService, ExceptionDTO) {
    var TbltTemplateDocumentRepositoryGes = /** @class */ (function () {
        /**
      * Costruttore
      * @param serviceConfiguration
      */
        function TbltTemplateDocumentRepositoryGes(serviceConfigurations) {
            var _this = this;
            /**
            *------------------------- Events -----------------------------
            */
            /**
           * Evento scatenato al click del pulsante Conferma/SalvaBozza
           * @method
           * @param sender
           * @param eventArgs
           * @returns
           */
            this.btnConfirm_Clicked = function (sender, eventArgs) {
                if (Page_IsValid) {
                    _this.showLoadingPanel(_this.pnlMetadataId);
                    _this.showFlatLoadingPanel();
                    $find(_this.ajaxManagerId).ajaxRequestWithTarget(_this.btnConfirmUniqueId, '');
                }
            };
            /**
             * Evento scatenato al click del pulsante Pubblica
             * @method
             * @param sender
             * @param eventArgs
             * @returns
             */
            this.btnPublish_Clicked = function (sender, eventArgs) {
                if (Page_IsValid) {
                    _this.showLoadingPanel(_this.pnlMetadataId);
                    _this.showFlatLoadingPanel();
                    $find(_this.ajaxManagerId).ajaxRequestWithTarget(_this.btnPublishUniqueId, '');
                }
            };
            /**
             * Callback per chiusura inserimento
             * @param entity
             */
            this.closeCallback = function (entity) {
                _this.closeLoadingPanel(_this.pnlMetadataId);
                _this.closeFlatLoadingPanel();
                _this.closeWindow(entity);
            };
            /**
             * Callback in caso di errore
             * @param entity
             */
            this.errorCallback = function () {
                _this.closeLoadingPanel(_this.pnlMetadataId);
                _this.closeFlatLoadingPanel();
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
        TbltTemplateDocumentRepositoryGes.prototype.initialize = function () {
            var _this = this;
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicked(this.btnConfirm_Clicked);
            this._btnPublish = $find(this.btnPublishId);
            this._btnPublish.add_clicked(this.btnPublish_Clicked);
            this._txtName = $find(this.txtNameId);
            this._txtObject = $find(this.txtObjectId);
            this._acbQualityTag = $find(this.acbQualityTagId);
            this._racTagsDataSource = $find(this.racTagsDataSourceId);
            this._manager = $find(this.managerId);
            this.showLoadingPanel(this.pnlMetadataId);
            this.showFlatLoadingPanel();
            $.when(this.loadTags(), ((this.action == TbltTemplateDocumentRepositoryGes.EDIT_ACTION) ? this.loadTemplateData() : undefined)).fail(function (exception) {
                _this.showNotificationException(_this.uscNotificationId, exception, "Errore nel caricamento del Template.");
            }).always(function () {
                _this.closeLoadingPanel(_this.pnlMetadataId);
                _this.closeFlatLoadingPanel();
            });
        };
        /**
         * Esegue il caricamento dei Quality Tag per la funzionalità di inserimento tag
         */
        TbltTemplateDocumentRepositoryGes.prototype.loadTags = function () {
            var _this = this;
            var promise = $.Deferred();
            try {
                this._service.getTags(function (data) {
                    try {
                        if (data == undefined) {
                            console.log("No quality tags found.");
                            promise.resolve();
                        }
                        var sourceModel = _this.prepareTagsSourceModel(data.value);
                        _this._racTagsDataSource.set_data(sourceModel);
                        _this._racTagsDataSource.fetch(undefined);
                        promise.resolve();
                    }
                    catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject("Errore durante il caricamento dei Tag");
                    }
                }, function (exception) {
                    promise.reject(exception);
                });
            }
            catch (error) {
                console.log(error.stack);
                promise.reject("Errore durante il caricamento dei Tag");
            }
            return promise.promise();
        };
        /**
         * Carica i dati del template corrente e li visualizza nella pagina
         */
        TbltTemplateDocumentRepositoryGes.prototype.loadTemplateData = function () {
            var _this = this;
            var promise = $.Deferred();
            try {
                this._service.getTemplateById(this.currentTemplateId, function (data) {
                    try {
                        if (data == undefined) {
                            console.log("No template found.");
                            promise.reject("Nessun template trovato con ID ".concat(_this.currentTemplateId));
                        }
                        _this.fillPageFromModel(data);
                        _this._currentTemplate = data;
                        promise.resolve();
                    }
                    catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error.message);
                    }
                }, function (exception) {
                    promise.reject(exception);
                });
            }
            catch (error) {
                console.log(error.stack);
                promise.reject("Errore in caricamento del template");
            }
            return promise.promise();
        };
        /**
        * Metodo che prepara il modello da passare al datasource della RadAutoCompleteBox
        * @param tags
        */
        TbltTemplateDocumentRepositoryGes.prototype.prepareTagsSourceModel = function (tags) {
            var models = new Array();
            $.each(tags, function (index, tag) {
                models.push({ id: tag, value: tag });
            });
            return models;
        };
        /**
        * Callback per l'inserimento/aggiornamento di un TemplateDocumentRepositoryModel
        * @param entity
        */
        TbltTemplateDocumentRepositoryGes.prototype.confirmCallback = function (persistedChainId, toPublish) {
            var _this = this;
            try {
                var entity = this.action == TbltTemplateDocumentRepositoryGes.EDIT_ACTION ? this._currentTemplate : {};
                entity = this.fillModelFromPage(entity);
                entity.Version = this.action == TbltTemplateDocumentRepositoryGes.EDIT_ACTION ? entity.Version + 1 : 1;
                entity.IdArchiveChain = persistedChainId;
                if (toPublish != undefined) {
                    entity.Status = (toPublish) ? TemplateDocumentRepositoryStatus.Available : TemplateDocumentRepositoryStatus.Draft;
                }
                var apiAction = this.action == 'Insert' ? function (m, c, e) { return _this._service.insertTemplateDocument(m, c, e); } : function (m, c, e) { return _this._service.updateTemplateDocument(m, c, e); };
                apiAction(entity, this.closeCallback, function (exception) {
                    _this.showNotificationException(_this.uscNotificationId, exception);
                    _this.closeLoadingPanel(_this.pnlMetadataId);
                    _this.closeFlatLoadingPanel();
                });
            }
            catch (error) {
                this.closeLoadingPanel(this.pnlMetadataId);
                this.closeFlatLoadingPanel();
                this.showNotificationMessage(this.uscNotificationId, "Errore in esecuzione dell'attività di salvataggio.");
                console.log(JSON.stringify(error));
            }
        };
        /**
         * Esegue il fill dei controlli della pagina in  modello TemplateDocumentRepositoryModel in inserimento
         */
        TbltTemplateDocumentRepositoryGes.prototype.fillModelFromPage = function (model) {
            model.Name = this._txtName.get_value();
            model.Object = this._txtObject.get_value();
            model.QualityTag = this.getTagEntries();
            return model;
        };
        /**
         * Esegue il fill di un template nella pagina
         * @param model
         */
        TbltTemplateDocumentRepositoryGes.prototype.fillPageFromModel = function (model) {
            var _this = this;
            this._txtName.set_value(model.Name);
            this._txtObject.set_value(model.Object);
            var tags = model.QualityTag.split(";");
            $.each(tags, function (index, tag) {
                if (!tag) {
                    return;
                }
                var entry = new Telerik.Web.UI.AutoCompleteBoxEntry();
                entry.set_text(tag);
                _this._acbQualityTag.get_entries().add(entry);
            });
            return model;
        };
        /**
         * Recupera gli elementi inseriti nel controllo relativo ai tag
         */
        TbltTemplateDocumentRepositoryGes.prototype.getTagEntries = function () {
            var qualityTags = '';
            var entries = this._acbQualityTag.get_entries();
            for (var i = 0; i < entries.get_count(); i++) {
                qualityTags = qualityTags.concat(entries.getEntry(i).get_text(), ';');
            }
            return qualityTags;
        };
        /**
         * Recupera una RadWindow dalla pagina
         */
        TbltTemplateDocumentRepositoryGes.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        /**
         * Chiude la RadWindow
         */
        TbltTemplateDocumentRepositoryGes.prototype.closeWindow = function (entity) {
            var wnd = this.getRadWindow();
            wnd.close(JSON.stringify(entity));
        };
        /**
         * Visualizza nuovi loading panel nella pagina
         */
        TbltTemplateDocumentRepositoryGes.prototype.showLoadingPanel = function (updatedElementId) {
            var ajaxDefaultLoadingPanel = $find(this.ajaxLoadingPanelId);
            ajaxDefaultLoadingPanel.show(updatedElementId);
        };
        /**
         * Nasconde i loading panel nella pagina
         */
        TbltTemplateDocumentRepositoryGes.prototype.closeLoadingPanel = function (updatedElementId) {
            var ajaxDefaultLoadingPanel = $find(this.ajaxLoadingPanelId);
            ajaxDefaultLoadingPanel.hide(updatedElementId);
        };
        /**
        * Visualizza flat loading panel sul pannello bottoni
        */
        TbltTemplateDocumentRepositoryGes.prototype.showFlatLoadingPanel = function () {
            var ajaxDefaultFlatLoadingPanel = $find(this.ajaxFlatLoadingPanelId);
            ajaxDefaultFlatLoadingPanel.show(this.pnlButtonsId);
        };
        /**
        * Nasconde flat loading panel sul pannello bottoni
        */
        TbltTemplateDocumentRepositoryGes.prototype.closeFlatLoadingPanel = function () {
            var ajaxDefaultFlatLoadingPanel = $find(this.ajaxFlatLoadingPanelId);
            ajaxDefaultFlatLoadingPanel.hide(this.pnlButtonsId);
        };
        /**
         * Resetta lo stato dei controlli
         */
        TbltTemplateDocumentRepositoryGes.prototype.resetControls = function () {
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicked(this.btnConfirm_Clicked);
            this._btnPublish = $find(this.btnPublishId);
            this._btnPublish.add_clicked(this.btnPublish_Clicked);
        };
        TbltTemplateDocumentRepositoryGes.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
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
        TbltTemplateDocumentRepositoryGes.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        TbltTemplateDocumentRepositoryGes.EDIT_ACTION = 'Edit';
        TbltTemplateDocumentRepositoryGes.INSERT_ACTION = 'Insert';
        return TbltTemplateDocumentRepositoryGes;
    }());
    return TbltTemplateDocumentRepositoryGes;
});
//# sourceMappingURL=TbltTemplateDocumentRepositoryGes.js.map