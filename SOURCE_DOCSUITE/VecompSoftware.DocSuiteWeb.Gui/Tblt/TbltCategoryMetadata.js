/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "UserControl/uscMetadataRepository", "App/Models/Commons/MetadataRepositoryModel", "App/Services/Commons/CategoryService", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO"], function (require, exports, UscMetadataRepository, MetadataRepositoryModel, CategoryService, ServiceConfigurationHelper, ExceptionDTO) {
    var TbltCategoryMetadata = /** @class */ (function () {
        function TbltCategoryMetadata(serviceConfigurations) {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            /**
             * Evento scatenato al click del pulsante di conferma
             */
            this.btnSubmit_OnClicking = function (sender, eventArgs) {
                eventArgs.set_cancel(true);
                _this._loadingPanel.show(_this.uscMetadataRepositoryId);
                var uscMetadaRepository = $("#".concat(_this.uscMetadataRepositoryId)).data();
                if (!jQuery.isEmptyObject(uscMetadaRepository)) {
                    if (!uscMetadaRepository.getSelectedNode()) {
                        alert("Attenzione: selezionare un elemento.");
                        return;
                    }
                    var node = uscMetadaRepository.getSelectedNode();
                    var idMetadataRepository_1 = node.get_value();
                    if (idMetadataRepository_1) {
                        _this._categoryService.getById(+_this.categoryId, function (data) {
                            _this._category = data;
                            var metadata = new MetadataRepositoryModel();
                            metadata.UniqueId = idMetadataRepository_1;
                            _this._category.MetadataRepository = metadata;
                            _this._categoryService.updateCategory(_this._category, function (data) {
                                _this._loadingPanel.hide(_this.uscMetadataRepositoryId);
                                _this.closeWindow();
                            }, function (exception) {
                                _this._loadingPanel.hide(_this.uscMetadataRepositoryId);
                                _this.showNotificationException(exception);
                            });
                        }, function (exception) {
                            _this._loadingPanel.hide(_this.uscMetadataRepositoryId);
                            _this.showNotificationException(exception);
                        });
                    }
                }
            };
            /**
             * Evento scatenato al click del pulsante di conferma
             */
            this.btnRemove_OnClicking = function (sender, eventArgs) {
                eventArgs.set_cancel(true);
                if (!_this.metadataRepositoryId) {
                    _this.showNotificationException(null, "Attenzione: il classificatore non ha metadati associati.");
                    return;
                }
                _this._loadingPanel.show(_this.uscMetadataRepositoryId);
                _this._categoryService.getById(+_this.categoryId, function (data) {
                    _this._category = data;
                    _this._category.MetadataRepository = null;
                    _this._categoryService.updateCategory(_this._category, function (data) {
                        _this._loadingPanel.hide(_this.uscMetadataRepositoryId);
                        _this._btnRemove.set_enabled(false);
                        _this.closeWindow();
                    }, function (exception) {
                        _this._loadingPanel.hide(_this.uscMetadataRepositoryId);
                        _this.showNotificationException(exception);
                    });
                }, function (exception) {
                    _this._loadingPanel.hide(_this.uscMetadataRepositoryId);
                    _this.showNotificationException(exception);
                });
            };
            this.showNotificationException = function (exception, customMessage) {
                var uscNotification = $("#".concat(_this.uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    if (exception && exception instanceof ExceptionDTO) {
                        uscNotification.showNotification(exception);
                    }
                    else {
                        uscNotification.showNotificationMessage(customMessage);
                    }
                }
            };
            this._serviceConfigurations = serviceConfigurations;
            this._categoryService = new CategoryService(ServiceConfigurationHelper.getService(serviceConfigurations, "Category"));
        }
        /**
         *------------------------- Methods -----------------------------
         */
        /**
         * Inizializzazione
         */
        TbltCategoryMetadata.prototype.initialize = function () {
            var _this = this;
            this._btnSubmit = $find(this.btnSubmitId);
            this._btnSubmit.set_enabled(false);
            this._btnSubmit.add_clicking(this.btnSubmit_OnClicking);
            this._btnRemove = $find(this.btnRemoveId);
            this._btnRemove.set_enabled(false);
            this._btnRemove.add_clicking(this.btnRemove_OnClicking);
            if (this.metadataRepositoryId) {
                this._btnRemove.set_enabled(true);
            }
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            var uscMetadaRepository = $("#".concat(this.uscMetadataRepositoryId)).data();
            if (!jQuery.isEmptyObject(uscMetadaRepository)) {
                $("#".concat(this.uscMetadataRepositoryId)).on(UscMetadataRepository.ON_TREEVIEW_LOADED, function (args, data) {
                    if (_this.metadataRepositoryId) {
                        var associatedNode = uscMetadaRepository.findNodeByValue(_this.metadataRepositoryId);
                        associatedNode.get_textElement().style.fontWeight = 'bold';
                    }
                });
                $("#".concat(this.uscMetadataRepositoryId)).on(UscMetadataRepository.ON_NODE_CLICKED, function (args, data) {
                    _this._btnSubmit.set_enabled(true);
                });
                $("#".concat(this.uscMetadataRepositoryId)).on(UscMetadataRepository.ON_ROOT_NODE_CLICKED, function (args) {
                    _this._btnSubmit.set_enabled(false);
                });
            }
        };
        /**
         * Chiude la window corrente
         */
        TbltCategoryMetadata.prototype.closeWindow = function () {
            var wnd = this.getRadWindow();
            wnd.close(true);
        };
        /**
      * Recupera una RadWindow dalla pagina
      */
        TbltCategoryMetadata.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow) {
                wnd = window.radWindow;
            }
            else if (window.frameElement.radWindow) {
                wnd = window.frameElement.radWindow;
            }
            return wnd;
        };
        return TbltCategoryMetadata;
    }());
    return TbltCategoryMetadata;
});
//# sourceMappingURL=TbltCategoryMetadata.js.map