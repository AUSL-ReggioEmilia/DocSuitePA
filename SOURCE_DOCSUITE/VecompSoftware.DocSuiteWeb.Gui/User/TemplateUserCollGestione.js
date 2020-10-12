/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Templates/TemplateCollaborationService", "App/Models/Templates/TemplateCollaborationStatus", "App/Models/Collaborations/CollaborationDocumentType", "App/Models/Commons/JsonParameter", "App/DTOs/ExceptionDTO"], function (require, exports, ServiceConfigurationHelper, TemplateCollaborationService, TemplateCollaborationStatus, CollaborationDocumentType, JsonParameter, ExceptionDTO) {
    var TemplateUserCollGestione = /** @class */ (function () {
        /**
         * Costruttore
         * @param serviceConfiguration
         */
        function TemplateUserCollGestione(serviceConfigurations) {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            /**
             * Evento scatenato al click del pulsante Salva Bozza
             * @param sender
             * @param args
             */
            this.btnConfirm_OnClicked = function (sender, args) {
                if (Page_IsValid) {
                    _this.showLoadingPanels();
                    $find(_this.ajaxManagerId).ajaxRequestWithTarget(_this.btnConfirmUniqueId, '');
                }
            };
            /**
             * Evento scatenato al click del pulsante Pubblica
             * @param sender
             * @param args
             */
            this.btnPublish_OnClicked = function (sender, args) {
                if (Page_IsValid) {
                    _this.showLoadingPanels();
                    $find(_this.ajaxManagerId).ajaxRequestWithTarget(_this.btnPublishUniqueId, '');
                }
            };
            /**
             * Evento scatenato al cambio di selezione della tipologia documento
             * @param sender
             * @param args
             */
            this.ddlDocumentType_selectedIndexChanged = function (sender, args) {
                var documentType = _this.getPageTypeFromDocumentType(sender.get_selectedItem().get_value());
                _this.changeBodyClass(documentType);
                $("#specificTypeRow").hide();
                if (sender.get_selectedItem().get_value() == CollaborationDocumentType[CollaborationDocumentType.UDS]) {
                    $("#specificTypeRow").show();
                }
                var ajaxmodel = { ActionName: 'DocumentTypeChanged', Value: [documentType] };
                $find(_this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxmodel));
            };
            /**
             * Evento scatenato all'uscita del focus di una RadTextbox per validare i caratteri inseriti
             * @param sender
             * @param args
             */
            this.changeStrWithValidCharacterHandler = function (sender, args) {
                window.ChangeStrWithValidCharacter(sender.get_element());
            };
            /**
             * Evento scatenato al click del pulsante Elimina
             * @param sender
             * @param args
             */
            this.btnDelete_OnClicked = function (sender, args) {
                _this._manager.radconfirm("Sei sicuro di voler eliminare il template corrente?", function (arg) {
                    if (arg) {
                        _this.showLoadingPanels();
                        try {
                            _this._service.getById(_this.templateId, function (data) {
                                _this._service.deleteTemplateCollaboration(data, function (data) {
                                    window.location.href = "../Tblt/TbltTemplateCollaborationManager.aspx?Type=Comm";
                                }, function (exception) {
                                    _this.hideLoadingPanels();
                                    _this.showNotificationException(_this.uscNotificationId, exception);
                                });
                            }, function (exception) {
                                _this.hideLoadingPanels();
                                _this.showNotificationException(_this.uscNotificationId, exception);
                            });
                        }
                        catch (error) {
                            _this.hideLoadingPanels();
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
        TemplateUserCollGestione.prototype._chkSecretaryViewRightEnabled = function () {
            return $("#" + this.chkSecretaryViewRightEnabledId);
        };
        TemplateUserCollGestione.prototype._chkPopupDocumentNotSignedAlertEnabled = function () {
            return $("#" + this.chkPopupDocumentNotSignedAlertEnabledId);
        };
        TemplateUserCollGestione.prototype._chkBtnCheckoutEnabled = function () {
            return $("#" + this.chkBtnCheckoutEnabledId);
        };
        /**
         *------------------------- Methods -----------------------------
         */
        /**
         * Metodo di inizializzazione della classe
         */
        TemplateUserCollGestione.prototype.initialize = function () {
            var _this = this;
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicked(this.btnConfirm_OnClicked);
            this._btnPublish = $find(this.btnPublishId);
            this._btnPublish.add_clicked(this.btnPublish_OnClicked);
            this._btnPublish.set_visible(this.action == TemplateUserCollGestione.EDIT_ACTION);
            this._btnDelete = $find(this.btnDeleteId);
            this._btnDelete.add_clicked(this.btnDelete_OnClicked);
            this._btnDelete.set_visible(this.action == TemplateUserCollGestione.EDIT_ACTION);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._flatLoadingPanel = $find(this.ajaxFlatLoadingPanelId);
            this._txtName = $find(this.txtNameId);
            this._txtObject = $find(this.txtObjectId);
            this._txtNote = $find(this.txtNoteId);
            this._ddlDocumentType = $find(this.ddlDocumentTypeId);
            this._ddlDocumentType.add_selectedIndexChanged(this.ddlDocumentType_selectedIndexChanged);
            this._ddlDocumentType.findItemByValue('P').select();
            this._ddlSpecificDocumentType = $find(this.ddlSpecificDocumentTypeId);
            this._manager = $find(this.radWindowManagerId);
            this._rblPriority = $("#".concat(this.rblPriorityId));
            this._currentTemplateIsLocked = false;
            this._chkDocumentUnitDraftEnabled = $("#".concat(this.chkDocumentUnitDraftEnabledId));
            $("#".concat(this.rowDocumentUnitDraftId)).hide();
            $("#specificTypeRow").hide();
            if (this.action == TemplateUserCollGestione.EDIT_ACTION) {
                this.showLoadingPanels();
                try {
                    this._service.getById(this.templateId, function (data) {
                        try {
                            if (data == undefined) {
                                _this._btnConfirm.set_enabled(false);
                                _this._btnPublish.set_enabled(false);
                                _this._btnDelete.set_enabled(false);
                                throw 'Nessun template trovato';
                            }
                            _this._currentTemplateIsLocked = data.IsLocked;
                            _this._btnPublish.set_enabled(TemplateCollaborationStatus[data.Status] != TemplateCollaborationStatus.Active);
                            _this._btnDelete.set_enabled(!data.IsLocked);
                            _this.fillPageFromEntity(data);
                            var ajaxmodel = { ActionName: 'LoadFromEntity', Value: [JSON.stringify(data)] };
                            $find(_this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxmodel));
                        }
                        catch (error) {
                            _this.hideLoadingPanels();
                            _this.showNotificationMessage(_this.uscNotificationId, 'Errore in caricamento dati del Template');
                            console.log(JSON.stringify(error));
                        }
                    }, function (exception) {
                        _this.hideLoadingPanels();
                        _this.showNotificationException(_this.uscNotificationId, exception);
                    });
                }
                catch (error) {
                    this.hideLoadingPanels();
                    this.showNotificationMessage(this.uscNotificationId, 'Errore in caricamento dati del Template');
                    console.log(JSON.stringify(error));
                }
            }
            else {
                this.ddlDocumentType_selectedIndexChanged(this._ddlDocumentType, undefined);
            }
        };
        /**
         * Metodo che esegue il fill dei dati dalla pagina nell'entità TemplateCollaborationModel
         * @param entity
         */
        TemplateUserCollGestione.prototype.fillEntity = function (entity) {
            entity.Name = this._txtName.get_value();
            entity.Object = this._txtObject.get_value();
            entity.Note = this._txtNote.get_value();
            if (this._ddlDocumentType.get_selectedItem().get_value() == CollaborationDocumentType[CollaborationDocumentType.UDS]) {
                if (this._ddlSpecificDocumentType.get_selectedItem().get_value() != '') {
                    entity.DocumentType = this._ddlSpecificDocumentType.get_selectedItem().get_value();
                }
                else {
                    entity.DocumentType = this._ddlDocumentType.get_selectedItem().get_value();
                }
            }
            else {
                entity.DocumentType = this._ddlDocumentType.get_selectedItem().get_value();
            }
            entity.IdPriority = this._rblPriority.find(":checked").val();
            if (this.action == TemplateUserCollGestione.EDIT_ACTION) {
                entity.IsLocked = this._currentTemplateIsLocked;
            }
            else {
                entity.IsLocked = false;
            }
            entity.Status = TemplateCollaborationStatus.Draft;
            var jpars = [];
            var documentUnitDraftParam = new JsonParameter();
            documentUnitDraftParam.Name = TemplateUserCollGestione.PRECOMPILE_PARAM;
            documentUnitDraftParam.PropertyType = 16;
            documentUnitDraftParam.ValueBoolean = $("#".concat(this.chkDocumentUnitDraftEnabledId)).is(":checked");
            var secretaryRightParam = new JsonParameter();
            secretaryRightParam.Name = TemplateUserCollGestione.SECRETARY_PARAM;
            secretaryRightParam.PropertyType = 16;
            secretaryRightParam.ValueBoolean = this._chkSecretaryViewRightEnabled().is(":checked");
            var popupDocumentNotSignedAlertParam = new JsonParameter();
            popupDocumentNotSignedAlertParam.Name = TemplateUserCollGestione.POPUP_PARAM;
            popupDocumentNotSignedAlertParam.PropertyType = 16;
            popupDocumentNotSignedAlertParam.ValueBoolean = this._chkPopupDocumentNotSignedAlertEnabled().is(":checked");
            var btncheckoutParam = new JsonParameter();
            btncheckoutParam.Name = TemplateUserCollGestione.BTNCHEKOUT_PARAM;
            btncheckoutParam.PropertyType = 16;
            btncheckoutParam.ValueBoolean = this._chkBtnCheckoutEnabled().is(":checked");
            jpars.push(documentUnitDraftParam);
            jpars.push(secretaryRightParam);
            jpars.push(popupDocumentNotSignedAlertParam);
            jpars.push(btncheckoutParam);
            entity.JsonParameters = JSON.stringify(jpars);
            return entity;
        };
        /**
         * Ripristina lo stato dei controlli della pagina
         */
        TemplateUserCollGestione.prototype.resetControlState = function () {
            this._txtName = $find(this.txtNameId);
            this._txtObject = $find(this.txtObjectId);
            this._txtNote = $find(this.txtNoteId);
        };
        /**
         * Callback per l'inserimento/aggiornamento di un TemplateCollaborationModel
         * @param entity
         */
        TemplateUserCollGestione.prototype.confirmCallback = function (entity, publishing) {
            var _this = this;
            try {
                entity = this.fillEntity(entity);
                var apiAction = this.action == TemplateUserCollGestione.INSERT_ACTION ? function (m, c, e) { return _this._service.insertTemplateCollaboration(m, c, e); } : function (m, c, e) { return _this._service.updateTemplateCollaboration(m, c, e); };
                apiAction(entity, function (data) {
                    if (publishing) {
                        _this._service.publishTemplate(data, function (data) {
                            _this.resetControlState();
                            _this._btnPublish.set_enabled(false);
                            _this.hideLoadingPanels();
                            alert("Template pubblicato correttamente");
                        }, function (exception) {
                            _this.resetControlState();
                            _this.hideLoadingPanels();
                            _this.showNotificationException(_this.uscNotificationId, exception);
                        });
                    }
                    else {
                        alert("Template salvato correttamente");
                        if (_this.action == 'Insert') {
                            window.location.href = "../User/TemplateUserCollGestione.aspx?Action=Edit&Type=".concat(_this.getPageTypeFromDocumentType(data.Environment), "&TemplateId=", data.UniqueId);
                        }
                        _this.resetControlState();
                        _this._btnPublish.set_enabled(true);
                        _this.hideLoadingPanels();
                    }
                }, function (exception) {
                    _this.hideLoadingPanels();
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }
            catch (error) {
                this.hideLoadingPanels();
                this.showNotificationMessage(this.uscNotificationId, "Errore in esecuzione dell'attività di salvataggio.");
                console.log(JSON.stringify(error));
            }
        };
        /**
         * Metodo che permette di modificare lo stile della pagina
         * @param classType
         */
        TemplateUserCollGestione.prototype.changeBodyClass = function (classType) {
            $("body").attr("class", classType.toLowerCase());
        };
        TemplateUserCollGestione.prototype.getCollaborationDocumentTypeFromEntity = function (entity) {
            if (!isNaN(parseInt(entity.DocumentType))) {
                return CollaborationDocumentType[CollaborationDocumentType.UDS];
            }
            return entity.DocumentType;
        };
        /**
         * Metodo che recupera il type corretto in base alla selezione della Tipologia di documento
         * @param documentType
         */
        TemplateUserCollGestione.prototype.getPageTypeFromDocumentType = function (documentType) {
            if (documentType == CollaborationDocumentType[CollaborationDocumentType.A] || documentType == CollaborationDocumentType[CollaborationDocumentType.D]) {
                return 'resl';
            }
            if (documentType == CollaborationDocumentType[CollaborationDocumentType.S]) {
                return 'series';
            }
            if (documentType == CollaborationDocumentType[CollaborationDocumentType.UDS] || !isNaN(parseInt(documentType))) {
                return 'uds';
            }
            return 'prot';
        };
        /**
         * Esegue il fill dei dati dalle WebAPI nella pagina
         * @param entity
         */
        TemplateUserCollGestione.prototype.fillPageFromEntity = function (entity) {
            this._txtName.set_value(entity.Name);
            this._txtNote.set_value(entity.Note);
            this._txtObject.set_value(entity.Object);
            if (entity.JsonParameters) {
                var jsonParms = JSON.parse(entity.JsonParameters);
                var draftParm = jsonParms.filter(function (f) { return f.Name == TemplateUserCollGestione.PRECOMPILE_PARAM; });
                if (draftParm && draftParm.length > 0) {
                    $("#".concat(this.chkDocumentUnitDraftEnabledId)).prop("checked", draftParm[0].ValueBoolean);
                }
                var secreataryParm = jsonParms.filter(function (f) { return f.Name == TemplateUserCollGestione.SECRETARY_PARAM; });
                if (secreataryParm && secreataryParm.length > 0) {
                    this._chkSecretaryViewRightEnabled().prop("checked", secreataryParm[0].ValueBoolean);
                }
                var popupDocumentNotSignedAlertParam = jsonParms.filter(function (f) { return f.Name == TemplateUserCollGestione.POPUP_PARAM; });
                if (popupDocumentNotSignedAlertParam && popupDocumentNotSignedAlertParam.length > 0) {
                    this._chkPopupDocumentNotSignedAlertEnabled().prop("checked", popupDocumentNotSignedAlertParam[0].ValueBoolean);
                }
                var btncheckoutParam = jsonParms.filter(function (f) { return f.Name == TemplateUserCollGestione.BTNCHEKOUT_PARAM; });
                if (btncheckoutParam && btncheckoutParam.length > 0) {
                    this._chkBtnCheckoutEnabled().prop("checked", btncheckoutParam[0].ValueBoolean);
                }
            }
            var collaborationDocumentTypeName = this.getCollaborationDocumentTypeFromEntity(entity);
            this._ddlDocumentType.findItemByValue(collaborationDocumentTypeName).select();
            if (collaborationDocumentTypeName == CollaborationDocumentType[CollaborationDocumentType.UDS] && !isNaN(parseInt(entity.DocumentType))) {
                this._ddlSpecificDocumentType.findItemByValue(entity.DocumentType).select();
            }
            this.showPanel(entity.DocumentType);
            $("#".concat(this.rblPriorityId, " :radio[value='", entity.IdPriority, "']")).prop("checked", true);
        };
        TemplateUserCollGestione.prototype.showPanel = function (documentType) {
            if (documentType == CollaborationDocumentType[CollaborationDocumentType.P] || !isNaN(parseInt(documentType))) {
                $("#" + this.rowDocumentUnitDraftId).show();
            }
        };
        /**
         * Callback scatenato al load dei dati dalle WebAPI lato code-behind
         */
        TemplateUserCollGestione.prototype.loadFromEntityCallback = function () {
            this.hideLoadingPanels();
        };
        /**
         * Visualizza i pannelli di loading nella pagina
         */
        TemplateUserCollGestione.prototype.showLoadingPanels = function () {
            this._loadingPanel.show(this.pnlMainPanelId);
            this._flatLoadingPanel.show(this.pnlHeaderId);
            this._flatLoadingPanel.show(this.pnlButtonsId);
        };
        /**
         * Nasconde i pannelli di loading della pagina
         */
        TemplateUserCollGestione.prototype.hideLoadingPanels = function () {
            this._loadingPanel.hide(this.pnlMainPanelId);
            this._flatLoadingPanel.hide(this.pnlHeaderId);
            this._flatLoadingPanel.hide(this.pnlButtonsId);
        };
        TemplateUserCollGestione.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
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
        TemplateUserCollGestione.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        TemplateUserCollGestione.INSERT_ACTION = "Insert";
        TemplateUserCollGestione.EDIT_ACTION = "Edit";
        TemplateUserCollGestione.PRECOMPILE_PARAM = "DocumentUnitDraftEditorEnabled";
        TemplateUserCollGestione.SECRETARY_PARAM = "SecretaryViewRightEnabled";
        TemplateUserCollGestione.POPUP_PARAM = "PopUpDocumentNotSignedAlertEnabled";
        TemplateUserCollGestione.BTNCHEKOUT_PARAM = "BtnCheckoutEnabled";
        return TemplateUserCollGestione;
    }());
    return TemplateUserCollGestione;
});
//# sourceMappingURL=TemplateUserCollGestione.js.map