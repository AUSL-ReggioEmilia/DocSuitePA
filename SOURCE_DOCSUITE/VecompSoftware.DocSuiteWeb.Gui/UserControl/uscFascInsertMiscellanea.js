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
define(["require", "exports", "Fasc/FascBase", "App/Helpers/ServiceConfigurationHelper", "UserControl/uscMiscellanea", "App/Models/DocumentUnits/ChainType", "App/Services/Fascicles/FascicleDocumentService", "../app/core/extensions/string"], function (require, exports, FascicleBase, ServiceConfigurationHelper, UscMiscellanea, ChainType, FascicleDocumentService) {
    var uscFascInsertMiscellanea = /** @class */ (function (_super) {
        __extends(uscFascInsertMiscellanea, _super);
        /**
         * Costruttore
        */
        function uscFascInsertMiscellanea(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME)) || this;
            /**
            *------------------------- Events -----------------------------
            */
            _this.btnUploadDocument_OnClicked = function (sender, eventArgs) {
                _this.openUploadDocumentWindow("CommonSelMiscellanea", "True");
            };
            _this._btnUploadZipDocument_OnClicked = function (sender, eventArgs) {
                _this.openUploadDocumentWindow("CommonUploadZIPMiscellanea", "False");
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
        uscFascInsertMiscellanea.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._notificationInfo = $find(this.radNotificationInfoId);
            this._manager = $find(this.radWindowManagerId);
            this._btnUploadDocument = $find(this.btnUploadDocumentId);
            if (this._btnUploadDocument) {
                this._btnUploadDocument.add_clicked(this.btnUploadDocument_OnClicked);
            }
            this._btnUploadZipDocument = $find(this.btnUploadZipDocumentId);
            if (this._btnUploadZipDocument) {
                this._btnUploadZipDocument.add_clicked(this._btnUploadZipDocument_OnClicked);
            }
            this._pnlButtons = $("#".concat(this.pnlButtonsId));
            if (this._pnlButtons) {
                this._pnlButtons.hide();
            }
            var fascicleDocumentConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleDocument");
            this._fascicleDocumentService = new FascicleDocumentService(fascicleDocumentConfiguration);
            this.service.getFascicle(this.currentFascicleId, function (data) {
                if (data == null)
                    return;
                _this._fascicleModel = data;
                _this._fascicleDocumentService.getByFolder(_this._fascicleModel.UniqueId, _this.idFascicleFolder, function (data) {
                    _this._fascicleModel.FascicleDocuments = data;
                    $("#".concat(_this.uscMiscellaneaId)).bind(UscMiscellanea.LOADED_EVENT, function (args) {
                        _this._loadingPanel.show(_this.currentPageId);
                        _this.loadMiscellanea();
                    });
                    $("#".concat(_this.uscMiscellaneaId)).on(UscMiscellanea.DELETE_DOCUMENT_EVENT, function (args, idDocument, idArchiveChain) {
                        _this.deleteDocument(idDocument, idArchiveChain);
                    });
                    $("#".concat(_this.uscMiscellaneaId)).on(UscMiscellanea.UPDATE_DOCUMENTS_EVENT, function (args, idArchiveChain) {
                        _this.UpdateDocuments(idArchiveChain);
                    });
                    _this._loadingPanel.show(_this.currentPageId);
                    _this.loadMiscellanea();
                });
            });
        };
        uscFascInsertMiscellanea.prototype.loadMiscellanea = function () {
            var uscMiscellanea = $("#".concat(this.uscMiscellaneaId)).data();
            if (!jQuery.isEmptyObject(uscMiscellanea)) {
                var insertsArchiveChain = "";
                var inserts = $.grep(this._fascicleModel.FascicleDocuments, function (element, index) {
                    if (isNaN(element.ChainType)) {
                        element.ChainType = ChainType[element.ChainType.toString()];
                    }
                    return element.ChainType == ChainType.Miscellanea;
                })[0];
                if (inserts != undefined) {
                    insertsArchiveChain = inserts.IdArchiveChain;
                }
                uscMiscellanea.archiveChainId = insertsArchiveChain;
                uscMiscellanea.locationId = this.locationId;
                uscMiscellanea.loadMiscellanea(insertsArchiveChain, this.locationId);
            }
            this._loadingPanel.hide(this.currentPageId);
            this._pnlButtons.show();
        };
        uscFascInsertMiscellanea.prototype.UpdateDocuments = function (idArchiveChain) {
            var _this = this;
            var inserts = $.grep(this._fascicleModel.FascicleDocuments, function (x) { return x.ChainType == ChainType.Miscellanea; })[0];
            if ((!inserts || idArchiveChain != inserts.IdArchiveChain) && !!idArchiveChain) {
                //aggiorna fascicolo
                var fascicleDocumentModel_1 = {};
                fascicleDocumentModel_1.ChainType = ChainType.Miscellanea;
                fascicleDocumentModel_1.IdArchiveChain = idArchiveChain;
                fascicleDocumentModel_1.Fascicle = this._fascicleModel;
                if (this.idFascicleFolder) {
                    fascicleDocumentModel_1.FascicleFolder = {};
                    fascicleDocumentModel_1.FascicleFolder.UniqueId = this.idFascicleFolder;
                }
                this._fascicleDocumentService.insertFascicleDocument(fascicleDocumentModel_1, function (data) {
                    _this._fascicleModel.FascicleDocuments.push(fascicleDocumentModel_1);
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
        uscFascInsertMiscellanea.prototype.deleteDocument = function (idDocument, idArchiveChain) {
            var request = {};
            request.ActionName = uscFascInsertMiscellanea.DELETE_DOCUMENT;
            request.Value = [];
            request.Value.push(idDocument);
            request.Value.push(idArchiveChain);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._ajaxManager.ajaxRequest(JSON.stringify(request));
        };
        uscFascInsertMiscellanea.prototype.openUploadDocumentWindow = function (documentPageName, multiDoc) {
            var uscMiscellanea = $("#".concat(this.uscMiscellaneaId)).data();
            if (!jQuery.isEmptyObject(uscMiscellanea)) {
                var url = "../UserControl/" + documentPageName + ".aspx?Action=Add&Type=Fasc&IdLocation=" + this.locationId + "&ArchiveName=" + this.archiveName + "&MultiDoc=" + multiDoc;
                var inserts = $.grep(this._fascicleModel.FascicleDocuments, function (x) { return x.ChainType == ChainType.Miscellanea; })[0];
                if (inserts != undefined) {
                    url = url.concat('&IdArchiveChain=').concat(inserts.IdArchiveChain);
                }
                uscMiscellanea.openInsertWindow(url);
                return false;
            }
        };
        uscFascInsertMiscellanea.DELETE_DOCUMENT = "Delete_Document";
        return uscFascInsertMiscellanea;
    }(FascicleBase));
    return uscFascInsertMiscellanea;
});
//# sourceMappingURL=uscFascInsertMiscellanea.js.map