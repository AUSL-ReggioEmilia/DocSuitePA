/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/DTOs/ExceptionDTO", "App/Helpers/FileHelper"], function (require, exports, ExceptionDTO, FileHelper) {
    var uscMiscellanea = /** @class */ (function () {
        /**
        * Costruttore
        * @param webApiConfiguration
        */
        function uscMiscellanea() {
            var _this = this;
            /**
            * Metono chiamato in chiusura della radwindow di inserimento
            * @param sender
            * @param args
            */
            this.closeUploadDocumentWindow = function (sender, args) {
                if (args.get_argument) {
                    var result = args.get_argument();
                    if (result) {
                        $("#".concat(_this.pageId)).triggerHandler(uscMiscellanea.UPDATE_DOCUMENTS_EVENT, result.Value[0].toString());
                    }
                }
            };
            /**
             * Metodo che nasconde il loading
             */
            this.hideLoadingPanel = function () {
                _this._loadingPanel.hide(_this.pageId);
            };
            this.initializeCallback = function () {
                _this.hideLoadingPanel();
            };
            $(document).ready(function () {
            });
        }
        /**
        *---------------------------- Events ---------------------------
        */
        /**
        *---------------------------- Methods ---------------------------
        */
        /**
       * Inizializzazione
       */
        uscMiscellanea.prototype.initialize = function () {
            this._ajaxManager = $find(this.ajaxManagerId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._miscellaneaToolBar = $find(this.miscellaneaToolBarId);
            this._managerUploadDocument = $find(this.managerUploadDocumentId);
            this._manager = $find(this.managerId);
            this._managerUploadDocument.add_close(this.closeUploadDocumentWindow);
            this._miscellaneaGrid = $find(this.miscellaneaGridId);
            this._masterTableView = this._miscellaneaGrid.get_masterTableView();
            this._currentDocumentToSignInformations = undefined;
            this.bindLoaded();
        };
        /**
        * Carico i documenti
        */
        uscMiscellanea.prototype.bindMiscellanea = function (documents) {
            this._masterTableView.set_dataSource(documents);
            this._masterTableView.dataBind();
            this.initializeCallback();
        };
        uscMiscellanea.prototype.loadMiscellanea = function (idArchiveChain, location) {
            this.loadDocuments(idArchiveChain, location);
        };
        uscMiscellanea.prototype.loadDocuments = function (idArchiveChain, location) {
            this._loadingPanel.show(this.pageId);
            var ajaxRequest = {};
            ajaxRequest.ActionName = uscMiscellanea.LOAD_DOCUMENTS;
            ajaxRequest.Value = new Array();
            ajaxRequest.Value.push(idArchiveChain);
            ajaxRequest.Value.push(location);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxRequest));
        };
        /**
        * Scateno l'evento di "Load Completed" del controllo
        */
        uscMiscellanea.prototype.bindLoaded = function () {
            $("#".concat(this.pageId)).data(this);
            $("#".concat(this.pageId)).triggerHandler(uscMiscellanea.LOADED_EVENT);
        };
        uscMiscellanea.prototype.onGridDataBound = function () {
            var row = this._masterTableView.get_dataItems();
            for (var i = 0; i < row.length; i++) {
                if (i % 2) {
                    row[i].addCssClass("Chiaro");
                }
                else {
                    row[i].addCssClass("Scuro");
                }
            }
        };
        uscMiscellanea.prototype.getDocumentExtension = function (documentName) {
            return FileHelper.getImageByFileName(documentName, true);
        };
        /**
        * Apre una nuova nuova RadWindow
        * @param url
        * @param name
        * @param width
        * @param height
        */
        uscMiscellanea.prototype.openWindow = function (url, name, width, height) {
            var manager = $find(this.managerWindowsId);
            var wnd = manager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.center();
            return false;
        };
        uscMiscellanea.prototype.openPreviewWindow = function (serializedDoc) {
            var url = '../Viewers/DocumentInfoViewer.aspx?'.concat(serializedDoc);
            this.openWindow(url, 'windowPreviewDocument', 750, 450);
        };
        uscMiscellanea.prototype.openEditWindow = function (idDocument, idArchiveChain, locationId) {
            var url = '../UserControl/CommonSelMiscellanea.aspx?Action=Edit&IdDocument='.concat(idDocument, "&Type=", this.type);
            url = url.concat('&IdArchiveChain=').concat(idArchiveChain);
            url = url.concat('&IdLocation=').concat(locationId);
            this.openWindow(url, 'managerUploadDocument', 770, 450);
        };
        uscMiscellanea.prototype.openDeleteWindow = function (idDocument, idArchiveChain) {
            var _this = this;
            this._manager.radconfirm("Sei sicuro di voler eliminare il documento?", function (arg) {
                if (arg) {
                    _this._loadingPanel.show(_this.pageId);
                    $("#".concat(_this.pageId)).triggerHandler(uscMiscellanea.DELETE_DOCUMENT_EVENT, idDocument, idArchiveChain);
                }
            }, 300, 160);
        };
        uscMiscellanea.prototype.openInsertWindow = function (url) {
            this.openWindow(url, "managerUploadDocument", 820, 530);
        };
        uscMiscellanea.prototype.openSignWindow = function (serializedDoc, locationId) {
            this._currentDocumentToSignInformations = [serializedDoc, locationId];
            var url = '../Comm/SingleSign.aspx?'.concat(serializedDoc);
            this.openWindow(url, 'signWindow', 750, 500);
        };
        uscMiscellanea.prototype.showNotification = function (uscNotificationId, error) {
            var uscNotification = $("#".concat(this.uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                if (error instanceof ExceptionDTO) {
                    uscNotification.showNotification(error);
                }
                else {
                    uscNotification.showNotificationMessage(error);
                }
            }
        };
        uscMiscellanea.prototype.closeSignWindow = function (sender, args) {
            if (args.get_argument() && this._currentDocumentToSignInformations) {
                var ajaxRequest = {};
                ajaxRequest.ActionName = uscMiscellanea.SIGN_DOCUMENT;
                ajaxRequest.Value = new Array();
                ajaxRequest.Value.push(args.get_argument());
                ajaxRequest.Value.push(this._currentDocumentToSignInformations[0]);
                ajaxRequest.Value.push(this._currentDocumentToSignInformations[1].toString());
                this._ajaxManager = $find(this.ajaxManagerId);
                this._ajaxManager.ajaxRequest(JSON.stringify(ajaxRequest));
            }
            this._currentDocumentToSignInformations = undefined;
        };
        uscMiscellanea.ON_END_LOAD_EVENT = "onEndLoad";
        uscMiscellanea.LOADED_EVENT = "onLoaded";
        uscMiscellanea.LOAD_DOCUMENTS = "LoadDocuments";
        uscMiscellanea.SIGN_DOCUMENT = "Sign";
        uscMiscellanea.UPDATE_DOCUMENTS_EVENT = "Update_Documents";
        uscMiscellanea.DELETE_DOCUMENT_EVENT = "Delete_Document";
        return uscMiscellanea;
    }());
    return uscMiscellanea;
});
//# sourceMappingURL=uscMiscellanea.js.map