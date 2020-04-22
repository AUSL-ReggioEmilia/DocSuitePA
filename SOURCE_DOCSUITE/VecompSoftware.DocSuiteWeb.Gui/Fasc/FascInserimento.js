/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />
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
define(["require", "exports", "App/Models/Fascicles/FascicleModel", "App/Models/Fascicles/FascicleType", "Fasc/FascBase", "App/Helpers/ServiceConfigurationHelper", "App/Models/Fascicles/FascicleReferenceType", "App/Models/Fascicles/FascicleDocumentUnitModel", "App/Services/Fascicles/FascicleDocumentUnitService", "App/DTOs/ValidationExceptionDTO", "UserControl/uscFascicleInsert", "App/Models/Commons/MetadataRepositoryModel", "App/Models/FascicolableActionType", "../app/core/extensions/string"], function (require, exports, FascicleModel, FascicleType, FascicleBase, ServiceConfigurationHelper, FascicleReferenceType, FascicleDocumentUnitModel, FascicleDocumentUnitService, ValidationExceptionDTO, UscFascicleInsert, MetadataRepositoryModel, FascicolableActionType) {
    var FascInserimento = /** @class */ (function (_super) {
        __extends(FascInserimento, _super);
        /**
         * Costruttore
         */
        function FascInserimento(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME)) || this;
            /**
             *------------------------- Events -----------------------------
             */
            /**
             * Evento alla partenza di una request ajax
             * @param sender
             * @param args
             */
            _this.onRequestStart = function (sender, args) {
                _this._btnInserimento.set_enabled(false);
            };
            /**
             * Evento al termine di una request ajax
             * @param sender
             * @param args
             */
            _this.onResponseEnd = function (sender, args) {
                _this._btnInserimento.set_enabled(true);
            };
            /**
             * Evento scatenato al click del pulsante di inserimento
             * @param sender
             * @param args
             */
            _this.btnInserimento_OnClick = function (sender, args) {
                if (!Page_IsValid) {
                    args.set_cancel(true);
                    return;
                }
                _this._btnInserimento.set_enabled(false);
                _this._loadingPanel.show(_this.fasciclePageContentId);
                var isFascValid = false;
                var uscFascInsert = $("#".concat(_this.uscFascInsertId)).data();
                if (!jQuery.isEmptyObject(uscFascInsert)) {
                    isFascValid = uscFascInsert.isPageValid();
                    var selectedFascicleType = uscFascInsert.getSelectedFascicleType();
                    if (String.isNullOrEmpty(selectedFascicleType)) {
                        _this.showNotificationMessage(_this.uscNotificationId, 'Selezionare una tipologia di fascicolo');
                    }
                }
                var ajaxModel = {};
                ajaxModel.Value = new Array();
                ajaxModel.ActionName = "Insert";
                _this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
                args.set_cancel(true);
            };
            /**
             *------------------------- Methods -----------------------------
             */
            _this.onCreateNewFascicle = function () {
                var uscFascInsert = $("#".concat(_this.uscFascInsertId)).data();
                if (!jQuery.isEmptyObject(uscFascInsert)) {
                    uscFascInsert.enableValidators(true);
                    if ((_this.environment && _this.currentUDId) || !_this.activityFascicleEnabled) {
                        uscFascInsert.setProcedureTypeSelected();
                    }
                }
                $("#".concat(_this.fascicleTypeRowId)).show();
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        /**
         * Initialize
         */
        FascInserimento.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._btnInserimento = $find(this.btnInserimentoId);
            this._btnInserimento.add_clicking(this.btnInserimento_OnClick);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._manager = $find(this.radWindowManagerId);
            $("#".concat(this.uscFascInsertId)).bind(UscFascicleInsert.LOADED_EVENT, function (args) {
                _this.onCreateNewFascicle();
            });
            $("#".concat(this.uscFascInsertId)).bind(UscFascicleInsert.FASCICLE_TYPE_CHANGED_EVENT, function (args) {
                _this._btnInserimento.set_enabled(true);
            });
            this.onCreateNewFascicle();
        };
        /**
         * Metodo per il recupero di una specifica radwindow
         */
        FascInserimento.prototype.getRadWindow = function () {
            var radWindow;
            if (window.radWindow)
                radWindow = window.radWindow;
            else if (window.frameElement.radWindow)
                radWindow = window.frameElement.radWindow;
            return radWindow;
        };
        /**
         * Metodo di chiusura di una radwindow
         * @param callback
         */
        FascInserimento.prototype.closeWindow = function (callback) {
            var radWindow = this.getRadWindow();
            if (radWindow != null)
                radWindow.close(callback);
        };
        /**
         * Callback da code-behind per l'inserimento di un fascicolo
         * @param contact
         * @param category
         */
        FascInserimento.prototype.insertCallback = function (responsibleContact, metadataModel) {
            var _this = this;
            var uscFascInsert = $("#".concat(this.uscFascInsertId)).data();
            if (!jQuery.isEmptyObject(uscFascInsert)) {
                var fascicleModel_1 = new FascicleModel();
                fascicleModel_1 = uscFascInsert.getFascicle();
                if (fascicleModel_1.FascicleType != FascicleType.Activity) {
                    var contactModel = {};
                    contactModel.EntityId = responsibleContact;
                    fascicleModel_1.Contacts.push(contactModel);
                }
                if (!!metadataModel) {
                    fascicleModel_1.MetadataValues = metadataModel;
                    if (sessionStorage.getItem("MetadataRepository")) {
                        var metadataRepository = new MetadataRepositoryModel();
                        metadataRepository.UniqueId = sessionStorage.getItem("MetadataRepository");
                        fascicleModel_1.MetadataRepository = metadataRepository;
                    }
                }
                this.service.insertFascicle(fascicleModel_1, function (data) {
                    if (_this.currentUDId && _this.environment) {
                        fascicleModel_1.UniqueId = data.UniqueId;
                        _this.insertFascicolableUD(fascicleModel_1);
                    }
                    else {
                        window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(data.UniqueId);
                    }
                }, function (exception) {
                    _this._loadingPanel.hide(_this.fasciclePageContentId);
                    _this._btnInserimento.set_enabled(true);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }
        };
        FascInserimento.prototype.insertFascicolableUD = function (fascicleModel) {
            var fascicolableService;
            var fascicleDocumentUnitModel = new FascicleDocumentUnitModel();
            fascicleDocumentUnitModel.ReferenceType = FascicleReferenceType.Fascicle;
            fascicleDocumentUnitModel.DocumentUnit = { UniqueId: this.currentUDId };
            fascicleDocumentUnitModel.Fascicle = fascicleModel;
            var fascicleDocumentUnitConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLE_DOCUMENTUNIT_TYPE_NAME);
            fascicolableService = new FascicleDocumentUnitService(fascicleDocumentUnitConfiguration);
            this.insertUD(fascicleDocumentUnitModel, fascicolableService, fascicleModel.UniqueId);
        };
        FascInserimento.prototype.insertUD = function (model, service, fascicleId) {
            var _this = this;
            service.insertFascicleUD(model, FascicolableActionType.AutomaticDetection, function (data) {
                window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(fascicleId);
            }, function (exception) {
                _this._loadingPanel.hide(_this.fasciclePageContentId);
                _this.excpetionWindow(fascicleId, exception);
            });
        };
        FascInserimento.prototype.excpetionWindow = function (uniqueId, exception) {
            var _this = this;
            var message = "Attenzione: il fascicolo è stato creato correttamente ma sono occorsi degli errori in fase di fascicolazione del documento.<br /> <br />";
            if (exception && exception instanceof ValidationExceptionDTO && exception.validationMessages.length > 0) {
                message = message.concat("Gli errori sono i seguenti: <br />");
                exception.validationMessages.forEach(function (item) {
                    message = message.concat(item.message, "<br />");
                });
            }
            message = message.concat("Proseguire con la visualizzazione del sommario del fascicolo?");
            this._manager.radconfirm(message, function (arg) {
                if (arg) {
                    _this._loadingPanel.show(_this.fasciclePageContentId);
                    window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(uniqueId);
                }
            }, 300, 160);
        };
        return FascInserimento;
    }(FascicleBase));
    return FascInserimento;
});
//# sourceMappingURL=FascInserimento.js.map