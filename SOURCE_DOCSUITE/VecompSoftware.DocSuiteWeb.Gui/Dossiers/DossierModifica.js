var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "Dossiers/DossierBase", "App/Helpers/ServiceConfigurationHelper", "UserControl/uscSetiContactSel", "UserControl/uscMetadataRepositorySel", "App/Helpers/PageClassHelper", "App/Models/Dossiers/DossierStatus", "App/Helpers/EnumHelper", "App/Models/Dossiers/DossierType"], function (require, exports, DossierBase, ServiceConfigurationHelper, uscSetiContactSel, UscMetadataRepositorySel, PageClassHelper, DossierStatus, EnumHelper, DossierType) {
    var DossierModifica = /** @class */ (function (_super) {
        __extends(DossierModifica, _super);
        /**
    * Costruttore
    * @param serviceConfiguration
    */
        function DossierModifica(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME)) || this;
            _this.contacts = [];
            _this.contactInsertId = [];
            _this.btnConfirm_clicked = function (sender, args) {
                _this.updateCallback();
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        /**
    *------------------------- Events -----------------------------
    */
        /**
       *------------------------- Methods -----------------------------
       */
        /**
        * Metodo di inizializzazione
        */
        DossierModifica.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._enumHelper = new EnumHelper();
            this._ajaxManager = $find(this.ajaxManagerId);
            this._btnConfirm = $find(this.btnConfirmId);
            this._txtNote = $find(this.txtNoteId);
            this._rdpStartDate = $find(this.rdpStartDateId);
            this._manager = $find(this.ajaxManagerId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._DossierModel = {};
            this._DossierContacts = new Array();
            this._lblStartDate = $("#".concat(this.lblStartDateId));
            this._lblYear = $("#".concat(this.lblYearId));
            this._lblNumber = $("#".concat(this.lblNumberId));
            this._lblContainer = $("#".concat(this.lblContainerId));
            this._rcbDossierStatus = $find(this.rcbDossierStatusId);
            this._btnConfirm.set_enabled(false);
            this._loadingPanel.show(this.dossierPageContentId);
            this._rowMetadataRepository = $("#".concat(this.rowMetadataId));
            this._rowMetadataRepository.hide();
            this._uscContattiSelRest = $("#" + this.uscContattiSelRestId).data();
            this.service.hasModifyRight(this.currentDossierId, function (data) {
                if (data == null)
                    return;
                if (data) {
                    $.when(_this.loadDossier(), _this.loadContacts()).done(function () {
                        _this._DossierModel.Contacts = _this._DossierContacts;
                        _this.fillPageFromModel(_this._DossierModel);
                        if (_this.metadataRepositoryEnabled) {
                            _this.loadMetadata(_this._DossierModel.MetadataDesigner, _this._DossierModel.MetadataValues);
                        }
                        if (_this._DossierModel && _this._DossierModel.MetadataDesigner) {
                            var metadata = JSON.parse(_this._DossierModel.MetadataDesigner);
                            if (metadata && metadata.SETIFieldEnabled) {
                                $("#".concat(_this.uscSetiContactSelId)).triggerHandler(uscSetiContactSel.SHOW_SETI_CONTACT_BUTTON, metadata.SETIFieldEnabled && _this.setiContactEnabledId);
                            }
                        }
                        _this.registerUscContactRestEventHandlers();
                        _this._btnConfirm.set_enabled(true);
                        _this._btnConfirm.add_clicked(_this.btnConfirm_clicked);
                        _this._loadingPanel.hide(_this.dossierPageContentId);
                    }).fail(function (exception) {
                        _this._btnConfirm.set_enabled(false);
                        _this._loadingPanel.hide(_this.dossierPageContentId);
                        _this.showNotificationException(_this.uscNotificationId, exception, "Errore nel caricamento del Dossier.");
                    });
                }
                else {
                    _this._btnConfirm.set_enabled(false);
                    _this._loadingPanel.hide(_this.dossierPageContentId);
                    $("#".concat(_this.dossierPageContentId)).hide();
                    _this.showNotificationMessage(_this.uscNotificationId, "Errore in inizializzazione pagina.<br \> Utente non autorizzato alla modifica del Dossier.");
                }
            }, function (exception) {
                _this._btnConfirm.set_enabled(false);
                _this._loadingPanel.hide(_this.dossierPageContentId);
                $("#".concat(_this.dossierPageContentId)).hide();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
            /*event for filing out the fields with the chosen Seti contact*/
            $("#".concat(this.uscDynamicMetadataId)).on(UscMetadataRepositorySel.SELECTED_SETI_CONTACT_EVENT, function (sender, args) {
                var uscDynamicMetadataRest = $("#".concat(_this.uscDynamicMetadataId)).data();
                uscDynamicMetadataRest.populateMetadataRepository(args, _this._DossierModel.MetadataDesigner);
            });
        };
        /*
        * Carico il dossier corrente senza navigation properties
        */
        DossierModifica.prototype.loadDossier = function () {
            var _this = this;
            var promise = $.Deferred();
            try {
                this.service.getDossier(this.currentDossierId, function (data) {
                    if (data == undefined) {
                        promise.resolve();
                        return;
                    }
                    _this._DossierModel = data;
                    promise.resolve();
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
        * carico i contatti del Dossier
        */
        DossierModifica.prototype.loadContacts = function () {
            var _this = this;
            var promise = $.Deferred();
            try {
                this.service.getDossierContacts(this.currentDossierId, function (data) {
                    try {
                        if (!data) {
                            promise.resolve();
                            return;
                        }
                        _this._DossierContacts = data;
                        for (var _i = 0, _a = _this._DossierContacts; _i < _a.length; _i++) {
                            var contact = _a[_i];
                            var newContact = {
                                UniqueId: contact.UniqueId,
                                EntityId: contact.EntityShortId,
                                Description: contact.Name,
                                IdContactType: contact.Type,
                                IncrementalFather: contact.IncrementalFather
                            };
                            _this.contactInsertId.push(newContact.EntityId);
                            _this.contacts.push(newContact);
                        }
                        PageClassHelper.callUserControlFunctionSafe(_this.uscContattiSelRestId)
                            .done(function (instance) { return instance.renderContactsTree(_this.contacts); });
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
     * Esegue il fill dei controlli della pagina in  modello DossierModel in update
     */
        DossierModifica.prototype.fillPageFromModel = function (model) {
            var _this = this;
            this._lblYear.html(model.Year.toString());
            this._lblNumber.html(model.Number);
            this._lblContainer.html(model.ContainerName);
            this._lblStartDate.html(model.FormattedStartDate);
            this._rdpStartDate.set_selectedDate(new Date(model.StartDate.toString()));
            this.service.allFasciclesAreClosed(model.UniqueId, function (data) {
                _this.populateDossierStatusComboBox(data, model.Status);
            }, function (exception) {
                console.error(exception);
            });
            var txtObject = $find(this.txtObjectId);
            txtObject.set_value(model.Subject);
            this._txtNote.set_value(model.Note);
            var ajaxModel = {};
            ajaxModel.Value = new Array();
            ajaxModel.Value.push(JSON.stringify(model.Contacts));
            ajaxModel.ActionName = "LoadExternalData";
            $find(this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
        };
        DossierModifica.prototype.registerUscContactRestEventHandlers = function () {
            var _this = this;
            PageClassHelper.callUserControlFunctionSafe(this.uscContattiSelRestId)
                .done(function (instance) {
                instance.registerEventHandler(instance.uscContattiSelRestEvents.ContactDeleted, function (contactIdToDelete) {
                    _this.contactInsertId = _this.contactInsertId.filter(function (x) { return x != contactIdToDelete; });
                    return $.Deferred().resolve();
                });
                instance.registerEventHandler(instance.uscContattiSelRestEvents.NewContactsAdded, function (newAddedContact) {
                    _this.contactInsertId.push(newAddedContact.EntityId);
                    return $.Deferred().resolve();
                });
            });
        };
        /**
        * Callback da code-behind per la modifica di un Dossier
        * @param contact
        * @param category
        */
        DossierModifica.prototype.updateCallback = function () {
            var _this = this;
            var dossierModel = {};
            //riferimento
            this.fillContacts(JSON.stringify(this.contactInsertId), dossierModel);
            this.fillModelFromPage(dossierModel);
            if (this.metadataRepositoryEnabled) {
                var uscDynamicMetadataRest = $("#".concat(this.uscDynamicMetadataId)).data();
                if (!jQuery.isEmptyObject(uscDynamicMetadataRest)) {
                    var metadata = JSON.parse(this._DossierModel.MetadataDesigner);
                    if (metadata) {
                        var setiIntegrationField = metadata.SETIFieldEnabled;
                        var result = uscDynamicMetadataRest.bindModelFormPage(setiIntegrationField);
                        if (!result) {
                            this._btnConfirm = $find(this.btnConfirmId);
                            this._btnConfirm.set_enabled(true);
                            return;
                        }
                        dossierModel.MetadataDesigner = result[0];
                        dossierModel.MetadataValues = result[1];
                    }
                }
            }
            this.service.updateDossier(dossierModel, function (data) {
                if (data == null)
                    return;
                _this._loadingPanel.show(_this.dossierPageContentId);
                window.location.href = "DossierVisualizza.aspx?Type=Dossier&IdDossier=".concat(_this.currentDossierId, "&DossierTitle=", data.Year.toString(), "/", ("000000000" + data.Number.toString()).slice(-7));
            }, function (exception) {
                _this._loadingPanel.hide(_this.dossierPageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        /**
        * Esegue il fill dei controlli della pagina in  modello DossierModel in update
        */
        DossierModifica.prototype.fillModelFromPage = function (model) {
            var txtObject = $find(this.txtObjectId);
            model.UniqueId = this.currentDossierId;
            model.Subject = txtObject.get_value();
            model.Note = this._txtNote.get_value();
            model.Year = Number(this._lblYear.text());
            model.Number = this._lblNumber.text();
            model.DossierType = DossierType[this._DossierModel.DossierType];
            var containerModel = {};
            containerModel.EntityShortId = Number(this._DossierModel.ContainerId);
            model.Container = containerModel;
            var selectedDate = new Date(this._rdpStartDate.get_selectedDate().getTime() - this._rdpStartDate.get_selectedDate().getTimezoneOffset() * 60000);
            model.StartDate = selectedDate;
            model.Status = DossierStatus[this._DossierModel.Status];
            var selectedDossierStatus = this._rcbDossierStatus.get_selectedItem().get_value();
            if (selectedDossierStatus) {
                model.Status = DossierStatus[selectedDossierStatus];
            }
            return model;
        };
        DossierModifica.prototype.loadMetadata = function (metadatas, metadataValues) {
            if (metadatas) {
                this._rowMetadataRepository.show();
                var uscDynamicMetadataRest = $("#".concat(this.uscDynamicMetadataId)).data();
                if (!jQuery.isEmptyObject(uscDynamicMetadataRest)) {
                    uscDynamicMetadataRest.loadPageItems(metadatas, metadataValues);
                }
            }
        };
        DossierModifica.prototype.populateDossierStatusComboBox = function (allFasciclesAreClosed, currentDossierStatus) {
            var rcbItem = new Telerik.Web.UI.RadComboBoxItem();
            rcbItem.set_text("");
            this._rcbDossierStatus.get_items().add(rcbItem);
            for (var dossierStatus in DossierStatus) {
                //if I have at least one fascicle opened I don't add Closed option in combobox
                if (dossierStatus === DossierStatus.Closed.toString() && !allFasciclesAreClosed) {
                    continue;
                }
                if (typeof DossierStatus[dossierStatus] === 'string' && dossierStatus !== DossierStatus[currentDossierStatus].toString()) {
                    var rcbItem_1 = new Telerik.Web.UI.RadComboBoxItem();
                    rcbItem_1.set_text(this._enumHelper.getDossierStatusDescription(DossierStatus[dossierStatus]));
                    rcbItem_1.set_value(DossierStatus[dossierStatus]);
                    this._rcbDossierStatus.get_items().add(rcbItem_1);
                }
            }
            this._rcbDossierStatus.get_items().getItem(0).select();
        };
        return DossierModifica;
    }(DossierBase));
    return DossierModifica;
});
//# sourceMappingURL=DossierModifica.js.map