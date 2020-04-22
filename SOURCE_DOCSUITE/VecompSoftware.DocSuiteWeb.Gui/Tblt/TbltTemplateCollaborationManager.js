define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Templates/TemplateCollaborationService"], function (require, exports, ServiceConfigurationHelper, TemplateCollaborationService) {
    var TbltTemplateCollaborationManager = /** @class */ (function () {
        /**
         * Costruttore della classe
         * @param serviceConfiguration
         */
        function TbltTemplateCollaborationManager(serviceConfigurations) {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            /**
             * Evento scatenato al click del pulsante Nuovo
             * @param sender
             * @param args
             */
            this.btnNew_OnClicked = function (sender, args) {
                _this._loadingPanel.show(_this.grdTemplateCollaborationId);
                window.location.href = TbltTemplateCollaborationManager.TEMPLATE_GESTIONE_URL;
            };
            /**
             * Evento scatenato al click del pulsante Elimina
             * @param sender
             * @param args
             */
            this.btnDelete_OnClicked = function (sender, args) {
                var selectedTemplates = _this._grdTemplateCollaboration.get_selectedItems();
                if (selectedTemplates.length == 0) {
                    _this.showWarningMessage(_this.uscNotificationId, "Selezionare un template");
                    return;
                }
                _this._manager.radconfirm("Sei sicuro di voler eliminare il template selezionato?", function (arg) {
                    if (arg) {
                        _this._loadingPanel.show(_this.grdTemplateCollaborationId);
                        try {
                            var template = selectedTemplates[0];
                            _this._service.getById(template.getDataKeyValue("Entity.UniqueId"), function (data) {
                                _this._service.deleteTemplateCollaboration(data, function (data) {
                                    _this._grdTemplateCollaboration.get_masterTableView().deleteSelectedItems();
                                    _this.resetControlState();
                                    _this._loadingPanel.hide(_this.grdTemplateCollaborationId);
                                }, function (exception) {
                                    _this._loadingPanel.hide(_this.grdTemplateCollaborationId);
                                    _this._uscNotification = $("#".concat(_this.uscNotificationId)).data();
                                    if (!jQuery.isEmptyObject(_this._uscNotification)) {
                                        _this._uscNotification.showNotification(exception);
                                    }
                                });
                            }, function (exception) {
                                _this._loadingPanel.hide(_this.grdTemplateCollaborationId);
                                _this._uscNotification = $("#".concat(_this.uscNotificationId)).data();
                                if (!jQuery.isEmptyObject(_this._uscNotification)) {
                                    _this._uscNotification.showNotification(exception);
                                }
                            });
                        }
                        catch (error) {
                            _this._loadingPanel.hide(_this.grdTemplateCollaborationId);
                            _this.showNotificationMessage(_this.uscNotificationId, "Errore in eliminazione del template");
                            console.log(JSON.stringify(error));
                        }
                    }
                }, 300, 160);
            };
            var serviceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "TemplateCollaboration");
            if (!serviceConfiguration) {
                this.showNotificationMessage(this.uscNotificationId, "Errore in inizializzazione. Nessun servizio configurato per il Template di collaborazione");
                return;
            }
            this._service = new TemplateCollaborationService(serviceConfiguration);
        }
        /**
         *------------------------- Methods -----------------------------
         */
        /**
         * Metodo di inizializzazione della classe
         */
        TbltTemplateCollaborationManager.prototype.initialize = function () {
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._btnNew = $find(this.btnNewId);
            this._btnNew.add_clicked(this.btnNew_OnClicked);
            this._btnDelete = $find(this.btnDeleteId);
            this._btnDelete.add_clicked(this.btnDelete_OnClicked);
            this._grdTemplateCollaboration = $find(this.grdTemplateCollaborationId);
            this._manager = $find(this.radWindowManagerId);
        };
        TbltTemplateCollaborationManager.prototype.resetControlState = function () {
            this._btnNew = $find(this.btnNewId);
            this._btnDelete = $find(this.btnDeleteId);
        };
        TbltTemplateCollaborationManager.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        TbltTemplateCollaborationManager.prototype.showWarningMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showWarningMessage(customMessage);
            }
        };
        TbltTemplateCollaborationManager.TEMPLATE_GESTIONE_URL = "../User/TemplateUserCollGestione.aspx?Action=Insert&Type=Prot";
        return TbltTemplateCollaborationManager;
    }());
    return TbltTemplateCollaborationManager;
});
//# sourceMappingURL=TbltTemplateCollaborationManager.js.map