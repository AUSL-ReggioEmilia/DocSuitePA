/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
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
define(["require", "exports", "Dossiers/DossierBase", "App/Helpers/ServiceConfigurationHelper", "UserControl/uscMiscellanea", "App/ViewModels/BaseEntityViewModel", "App/Models/DocumentUnits/ChainType", "App/Services/Dossiers/DossierDocumentService", "../app/core/extensions/string"], function (require, exports, DossierBase, ServiceConfigurationHelper, UscMiscellanea, BaseEntityViewModel, ChainType, DossierDocumentService) {
    var DossierMiscellanea = /** @class */ (function (_super) {
        __extends(DossierMiscellanea, _super);
        /**
         * Costruttore
        */
        function DossierMiscellanea(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME)) || this;
            /**
            *------------------------- Events -----------------------------
            */
            _this.btnUploadDocument_OnClicked = function (sender, eventArgs) {
                var uscMiscellanea = $("#".concat(_this.uscMiscellaneaId)).data();
                if (!jQuery.isEmptyObject(uscMiscellanea)) {
                    var url = '../UserControl/CommonSelMiscellanea.aspx?Action=Add&Type=Dossier&IdLocation='.concat(_this.locationId, '&ArchiveName=', _this.archiveName);
                    var insertsArchiveChain = "";
                    if (_this._dossierModel.Documents != undefined && _this._dossierModel.Documents.length > 0) {
                        insertsArchiveChain = _this._dossierModel.Documents[0].IdArchiveChain.toString();
                        url = url.concat('&IdArchiveChain=').concat(insertsArchiveChain);
                    }
                    uscMiscellanea.openInsertWindow(url);
                    return false;
                }
            };
            _this.refreshDocuments = function () {
                _this.loadMiscellanea();
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        /**
        *------------------------- Methods -----------------------------
        */
        DossierMiscellanea.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._notificationInfo = $find(this.radNotificationInfoId);
            this._manager = $find(this.radWindowManagerId);
            this._btnUploadDocument = $find(this.btnUploadDocumentId);
            this._btnUploadDocument.add_clicked(this.btnUploadDocument_OnClicked);
            this._pnlButtons = $("#".concat(this.pnlButtonsId));
            this._pnlButtons.hide();
            this._dossierModel = {};
            this._DossierDocuments = new Array();
            var dossierDocumentConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, DossierBase.DOSSIERDOCUMENT_TYPE_NAME);
            this._dossierDocumentService = new DossierDocumentService(dossierDocumentConfiguration);
            this.service.getDossier(this.currentDossierId, function (data) {
                if (data == null)
                    return;
                _this._dossierModel = data;
                _this._dossierDocumentService.getDossierDocuments(_this.currentDossierId, function (data) {
                    try {
                        if (!data) {
                            return;
                        }
                        _this._DossierDocuments = data;
                        _this._dossierModel.Documents = _this._DossierDocuments;
                        $("#".concat(_this.uscMiscellaneaId)).bind(UscMiscellanea.LOADED_EVENT, function (args) {
                            _this.loadMiscellanea();
                        });
                        $("#".concat(_this.uscMiscellaneaId)).on(UscMiscellanea.DELETE_DOCUMENT_EVENT, function (args, idDocument, idArchiveChain) {
                            _this.deleteDocument(idDocument, idArchiveChain);
                        });
                        $("#".concat(_this.uscMiscellaneaId)).on(UscMiscellanea.UPDATE_DOCUMENTS_EVENT, function (args, idArchiveChain) {
                            _this.UpdateDocuments(idArchiveChain);
                        });
                        _this.loadMiscellanea();
                    }
                    catch (error) {
                        console.log(JSON.stringify(error));
                    }
                }, function (exception) {
                    _this.showNotificationException(_this.uscNotificationId, exception);
                    _this.showNotificationMessage(_this.uscNotificationId, "Errore durante il caricamento degli inserti del dossier.");
                });
            });
        };
        DossierMiscellanea.prototype.loadMiscellanea = function () {
            var uscMiscellanea = $("#".concat(this.uscMiscellaneaId)).data();
            if (!jQuery.isEmptyObject(uscMiscellanea)) {
                var insertsArchiveChain = "";
                if (this._dossierModel.Documents != undefined && this._dossierModel.Documents.length > 0) {
                    insertsArchiveChain = this._dossierModel.Documents[0].IdArchiveChain.toString();
                }
                uscMiscellanea.archiveChainId = insertsArchiveChain;
                uscMiscellanea.locationId = this.locationId;
                uscMiscellanea.loadMiscellanea(insertsArchiveChain, this.locationId);
            }
            this._pnlButtons.show();
        };
        DossierMiscellanea.prototype.UpdateDocuments = function (idArchiveChain) {
            var _this = this;
            var insertArchiveChain = "";
            if (this._dossierModel.Documents != undefined && this._dossierModel.Documents.length > 0) {
                insertArchiveChain = this._dossierModel.Documents[0].IdArchiveChain.toString();
            }
            if ((!insertArchiveChain || idArchiveChain != insertArchiveChain) && !!idArchiveChain) {
                //aggiorna dossier
                var dossier = {};
                dossier.UniqueId = this._dossierModel.UniqueId;
                var dossierDocumentModel_1 = {};
                dossierDocumentModel_1.ChainType = ChainType.Miscellanea;
                dossierDocumentModel_1.IdArchiveChain = idArchiveChain;
                dossierDocumentModel_1.Dossier = dossier;
                //secondo me devi mettere tutto il modello del dossier (anche contatti)
                this._dossierDocumentService.insertDossierDocument(dossierDocumentModel_1, function (data) {
                    var inserted = new BaseEntityViewModel();
                    inserted.IdArchiveChain = dossierDocumentModel_1.IdArchiveChain;
                    _this._dossierModel.Documents.push(inserted);
                    _this.loadMiscellanea();
                }, function (exception) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }
            else {
                this.loadMiscellanea();
            }
        };
        DossierMiscellanea.prototype.deleteDocument = function (idDocument, idArchiveChain) {
            var request = {};
            request.ActionName = DossierMiscellanea.DELETE_DOCUMENT;
            request.Value = [];
            request.Value.push(idDocument);
            request.Value.push(idArchiveChain);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._ajaxManager.ajaxRequest(JSON.stringify(request));
        };
        DossierMiscellanea.DELETE_DOCUMENT = "Delete_Document";
        return DossierMiscellanea;
    }(DossierBase));
    return DossierMiscellanea;
});
//# sourceMappingURL=DossierMiscellanea.js.map