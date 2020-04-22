/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
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
define(["require", "exports", "App/Models/Fascicles/FascicleType", "App/Helpers/ServiceConfigurationHelper", "Fasc/FascBase", "UserControl/uscFascicolo", "App/Models/DocumentUnits/ChainType"], function (require, exports, FascicleType, ServiceConfigurationHelper, FascicleBase, UscFascicolo, ChainType) {
    var FascModifica = /** @class */ (function (_super) {
        __extends(FascModifica, _super);
        /**
         * Costruttore
         * @param serviceConfigurations
         */
        function FascModifica(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME)) || this;
            /**
             *------------------------- Events -----------------------------
             */
            /**
             * Evento scatenato al click del pulsante di inserimento
             * @param sender
             * @param args
             */
            _this.btnConferma_OnClick = function (sender, args) {
                args.set_cancel(true);
                if (!Page_IsValid) {
                    return;
                }
                _this._loadingPanel.show(_this.pageContentId);
                _this._btnConfirm.set_enabled(false);
                if (_this.isPageValid()) {
                    var insertsArchiveChain = _this.getInsertsArchiveChain();
                    var ajaxModel = {};
                    ajaxModel.Value = new Array();
                    ajaxModel.ActionName = "Update";
                    ajaxModel.Value = new Array();
                    ajaxModel.Value.push(insertsArchiveChain);
                    _this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
                    return;
                }
                _this._loadingPanel.hide(_this.pageContentId);
                _this._btnConfirm.set_enabled(true);
            };
            return _this;
        }
        /**
         * Initialize
         */
        FascModifica.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._txtName = $find(this.txtNameId);
            this._txtRack = $find(this.txtRackId);
            this._txtNote = $find(this.txtNoteId);
            this._txtManager = $find(this.txtManagerId);
            this._txtObject = $find(this.txtObjectId);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._btnConfirm = $find(this.btnConfermaId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._manager = $find(this.radWindowManagerId);
            this._rowName = $("#".concat(this.rowNameId));
            this._rowRacks = $("#".concat(this.rowRackId));
            this._rowDynamicMetadata = $("#".concat(this.rowDynamicMetadataId));
            this._btnConfirm.add_clicking(this.btnConferma_OnClick);
            this._btnConfirm.set_enabled(false);
            this._rowDynamicMetadata.hide();
            this.initializeFascicle();
        };
        /**
         *------------------------- Methods -----------------------------
         */
        FascModifica.prototype.initializeFascicle = function () {
            var _this = this;
            this._loadingPanel.show(this.pageContentId);
            this.service.getFascicle(this.currentFascicleId, function (data) {
                if (data == null)
                    return;
                _this._fascicleModel = data;
                _this.checkFascicleRight(_this.currentFascicleId)
                    .done(function (isEditable) {
                    if (!isEditable) {
                        _this._loadingPanel.hide(_this.pageContentId);
                        _this.showNotificationMessage(_this.uscNotificationId, "Fascicolo n. " + _this._fascicleModel.Title + ". Mancano diritti di modifica.");
                        $("#".concat(_this.pageContentId)).hide();
                        return;
                    }
                    _this.bindPageFromModel(_this._fascicleModel);
                    var jsonFascicle = JSON.stringify(_this._fascicleModel);
                    var ajaxModel = {};
                    ajaxModel.Value = new Array();
                    ajaxModel.ActionName = "Initialize";
                    ajaxModel.Value = new Array();
                    ajaxModel.Value.push(jsonFascicle);
                    _this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
                })
                    .fail(function (exception) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        FascModifica.prototype.checkFascicleRight = function (idFascicle) {
            var promise = $.Deferred();
            this.service.hasManageableRight(idFascicle, function (data) { return promise.resolve(!!data); }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        FascModifica.prototype.bindPageFromModel = function (fascicle) {
            this._txtObject.set_value(fascicle.FascicleObject);
            this._txtNote.set_value(fascicle.Note);
            this._txtManager.set_value(fascicle.Manager);
            if (!this.isEditPage && !(this.fasciclesPanelVisibilities["FascicleNameVisibility"])) {
                $("#" + this.rowNameId).hide();
            }
            if (this._txtName) {
                this._txtName.set_value(fascicle.Name);
            }
            if (!this.isEditPage && !(this.fasciclesPanelVisibilities["FascicleRacksVisibility"])) {
                $("#" + this.rowRackId).hide();
            }
            this._txtRack.set_value(fascicle.Rack);
            if (fascicle.FascicleType != FascicleType[FascicleType.Legacy]) {
                $("#" + this.rowLegacyManagerId).remove();
            }
            if (fascicle.FascicleType == FascicleType[FascicleType.Activity]) {
                $("#" + this.rowManagerId).hide();
            }
            if (this.metadataRepositoryEnabled && fascicle.MetadataValues) {
                this._rowDynamicMetadata.show();
            }
        };
        /**
         * Inizializza lo user control del sommario di fascicolo
         */
        FascModifica.prototype.loadFascicoloSummary = function () {
            var _this = this;
            var uscFascicolo = $("#".concat(this.uscFascicoloId)).data();
            if (!jQuery.isEmptyObject(uscFascicolo)) {
                $("#".concat(this.uscFascicoloId)).bind(UscFascicolo.DATA_LOADED_EVENT, function (args) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this._btnConfirm.set_enabled(true);
                });
                uscFascicolo.loadData(this._fascicleModel);
            }
        };
        /**
         * Callback inizializzazione pagina
         */
        FascModifica.prototype.initializeCallback = function () {
            var _this = this;
            $("#".concat(this.uscFascicoloId)).bind(UscFascicolo.LOADED_EVENT, function (args) {
                _this.loadFascicoloSummary();
            });
            this.loadFascicoloSummary();
        };
        /**
         * Metodo per la verifica della validità della pagina
         */
        FascModifica.prototype.isPageValid = function () {
            var txtObject = $find(this.txtObjectId);
            if (txtObject.get_maxLength() != 0 && txtObject.get_textBoxValue().length > txtObject.get_maxLength()) {
                this.showNotificationMessage(this.uscNotificationId, "Impossibile salvare.\nIl campo Oggetto ha superato i caratteri disponibili.\n(Caratteri ".concat(txtObject.get_textBoxValue().length.toString(), " Disponibili ", txtObject.get_maxLength().toString()));
                return false;
            }
            return true;
        };
        /**
         * Callback di modifica fascicolo
         * @param contact
         */
        FascModifica.prototype.updateCallback = function (contact, metadataModel) {
            var _this = this;
            if (this._fascicleModel == null) {
                this._loadingPanel.hide(this.pageContentId);
                this._btnConfirm.set_enabled(true);
                this.showWarningMessage(this.uscNotificationId, "Nessun fascicolo definito per la modifica");
                return;
            }
            var txtObject = $find(this.txtObjectId);
            if (this._txtName) {
                this._fascicleModel.Name = this._txtName.get_value();
            }
            this._fascicleModel.Rack = this._txtRack.get_value();
            this._fascicleModel.Note = this._txtNote.get_value();
            this._fascicleModel.FascicleObject = txtObject.get_value();
            if (this._fascicleModel.FascicleType == FascicleType.Legacy) {
                this._fascicleModel.Manager = this._txtManager.get_value();
            }
            if (this._fascicleModel.FascicleType != FascicleType.Activity && contact != null && contact != 0) {
                var contactModel = {};
                contactModel.EntityId = contact;
                this._fascicleModel.Contacts.splice(0, this._fascicleModel.Contacts.length);
                this._fascicleModel.Contacts.push(contactModel);
            }
            if (!!metadataModel) {
                this._fascicleModel.MetadataValues = metadataModel;
            }
            this.service.updateFascicle(this._fascicleModel, null, function (data) {
                window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(_this._fascicleModel.UniqueId);
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this._btnConfirm.set_enabled(true);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        /**
         * Recupera il record relativo agli Inserti in FascicleDocuments
         */
        FascModifica.prototype.getInsertsArchiveChain = function () {
            var insertsArchiveChain = "";
            var inserts = $.grep(this._fascicleModel.FascicleDocuments, function (x) { return ChainType[x.ChainType.toString()] == ChainType.Miscellanea; })[0];
            if (inserts != undefined) {
                insertsArchiveChain = inserts.IdArchiveChain;
            }
            return insertsArchiveChain;
        };
        return FascModifica;
    }(FascicleBase));
    return FascModifica;
});
//# sourceMappingURL=FascModifica.js.map