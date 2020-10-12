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
define(["require", "exports", "Fasc/FascBase", "App/Helpers/ServiceConfigurationHelper", "UserControl/uscFascicleFolders", "App/Models/Environment", "App/DTOs/ExceptionDTO", "App/Services/Fascicles/FascicleDocumentUnitService", "App/Services/Fascicles/FascicleDocumentService", "App/Models/UpdateActionType", "App/Models/DocumentUnits/ChainType", "App/Models/Fascicles/FascicleModel", "App/Services/Fascicles/FascicleFolderService", "App/Helpers/GuidHelper", "App/Models/FascicolableActionType", "App/Helpers/SessionStorageKeysHelper"], function (require, exports, FascBase, ServiceConfigurationHelper, uscFascicleFolders, Environment, ExceptionDTO, FascicleDocumentUnitService, FascicleDocumentService, UpdateActionType, ChainType, FascicleModel, FascicleFolderService, Guid, FascicolableActionType, SessionStorageKeysHelper) {
    var FascMoveItems = /** @class */ (function (_super) {
        __extends(FascMoveItems, _super);
        function FascMoveItems(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascBase.FASCICLE_TYPE_NAME)) || this;
            _this.destinationFascicleId = "";
            _this.moveToFascicle = false;
            _this._miscellaneaDeferreds = [];
            /**
             *------------------------- Events -----------------------------
             */
            /**
             * Evento scatenato al click del pulsante conferma
             */
            _this.btnConfirm_OnClick = function (sender, args) {
                var uscFascicleFolder = $("#" + _this.uscFascicleFoldersId).data();
                var selectedFolder = uscFascicleFolder.getSelectedFascicleFolder(_this.moveToFascicle ? _this.destinationFascicleId : _this.idFascicle);
                if (!selectedFolder || !selectedFolder.Typology) {
                    _this.showWarningMessage(_this.uscNotificationId, "Selezionare una cartella del Fascicolo");
                    sender.enableAfterSingleClick();
                    return;
                }
                if (selectedFolder.UniqueId == _this.idFascicleFolder) {
                    _this.showWarningMessage(_this.uscNotificationId, "Selezionare una cartella di destinazione differente dalla cartella originale");
                    sender.enableAfterSingleClick();
                    return;
                }
                var moveActions = _this.ItemTypeMoveActions().filter(function (item) { return item[0] == _this.itemsType; })
                    .map(function (item) { return item[1]; });
                if (!moveActions || moveActions.length == 0) {
                    _this.showNotificationMessage(_this.uscNotificationId, "E' avvenuto un errore durante la procedura di sposta");
                    return;
                }
                _this._manager.radconfirm("Sei sicuro di voler eseguire lo spostamento degli elementi selezionati nella cartella " + selectedFolder.Name + "?", function (arg) {
                    if (!arg) {
                        sender.enableAfterSingleClick();
                        return;
                    }
                    _this.showLoading();
                    moveActions[0](selectedFolder.UniqueId)
                        .done(function () { return _this.closeWindow(); })
                        .fail(function (exception) {
                        console.error(exception);
                        _this.showNotificationException(_this.uscNotificationId, exception);
                    })
                        .always(function () {
                        sender.enableAfterSingleClick();
                        _this.hideLoading();
                    });
                });
            };
            _this.moveMiscellaneaDocumentsCallback = function (errorMessage, idArchiveChain, toCreateChain) {
                if (errorMessage) {
                    _this._miscellaneaDeferreds.forEach(function (promise) {
                        var errorDto = new ExceptionDTO();
                        errorDto.statusText = errorMessage;
                        promise.reject(errorDto);
                    });
                    return;
                }
                var deferredAction = function () { return $.Deferred().resolve().promise(); };
                if (toCreateChain && (idArchiveChain && idArchiveChain != Guid.empty)) {
                    deferredAction = function () { return _this.addMiscellaneaChain(idArchiveChain); };
                }
                deferredAction()
                    .done(function () {
                    _this.updateCurrentMiscellaneaChain()
                        .done(function () { return _this._miscellaneaDeferreds.forEach(function (promise) { return promise.resolve(); }); })
                        .fail(function (exception) { return _this._miscellaneaDeferreds.forEach(function (promise) { return promise.reject(exception); }); });
                })
                    .fail(function (exception) { return _this._miscellaneaDeferreds.forEach(function (promise) { return promise.reject(exception); }); });
            };
            _this._serviceConfigurations = serviceConfigurations;
            return _this;
        }
        FascMoveItems.prototype.ItemTypeMoveActions = function () {
            var _this = this;
            var items = [
                [FascMoveItems.DOCUMENT_ITEMS_TYPE, function (folderId) { return _this.moveDocuments(folderId); }],
                [FascMoveItems.FOLDER_ITEMS_TYPE, function (folderId) { return _this.moveFolders(folderId); }]
            ];
            return items;
        };
        /**
         *------------------------- Methods -----------------------------
         */
        FascMoveItems.prototype.initialize = function () {
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicked(this.btnConfirm_OnClick);
            this._rtvItemsToMove = $find(this.rtvItemsToMoveId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._manager = $find(this.radWindowManagerId);
            this._ajaxManager = $find(this.ajaxManagerId);
            try {
                var fascicleDocumentUnitConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascBase.FASCICLE_DOCUMENTUNIT_TYPE_NAME);
                this._fascicleDocumentUnitService = new FascicleDocumentUnitService(fascicleDocumentUnitConfiguration);
                var fascicleDocumentConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascBase.FASCICLE_DOCUMENT_TYPE_NAME);
                this._fascicleDocumentService = new FascicleDocumentService(fascicleDocumentConfiguration);
                var fascicleFolderConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascBase.FASCICLEFOLDER_TYPE_NAME);
                this._fascicleFolderService = new FascicleFolderService(fascicleFolderConfiguration);
                if (!this.isSessionStorageReaded()) {
                    this.showWarningMessage(this.uscNotificationId, "Nessun elemento presente per l'attività corrente");
                    return;
                }
                this.initializeTitleDescription();
                this.initializeItems();
                this.initializeExternalHandlers();
            }
            catch (error) {
                console.error(error);
                this.showNotificationMessage(this.uscNotificationId, "Errore in inizializzazione pagina");
            }
        };
        FascMoveItems.prototype.initializeTitleDescription = function () {
            var description = "Documenti selezionati";
            if (this.itemsType == FascMoveItems.FOLDER_ITEMS_TYPE) {
                description = "Cartelle selezionate";
            }
            $("#" + this.lblItemSelectedDescriptionId).text(description);
        };
        FascMoveItems.prototype.isSessionStorageReaded = function () {
            var selectedItems = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_FASC_MOVE_ITEMS);
            if (!selectedItems) {
                return false;
            }
            this._toMoveItems = JSON.parse(selectedItems);
            if (!this._toMoveItems || this._toMoveItems.length == 0) {
                return false;
            }
            return true;
        };
        FascMoveItems.prototype.initializeItems = function () {
            var node;
            for (var _i = 0, _a = this._toMoveItems; _i < _a.length; _i++) {
                var toMoveItem = _a[_i];
                node = new Telerik.Web.UI.RadTreeNode();
                node.set_text(toMoveItem.name);
                node.set_value(toMoveItem.uniqueId);
                node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/folder_closed.png");
                if (this.itemsType == FascMoveItems.DOCUMENT_ITEMS_TYPE) {
                    node.set_imageUrl(this.getIconByEnvironment(toMoveItem.environment));
                }
                this._rtvItemsToMove.get_nodes().add(node);
            }
        };
        FascMoveItems.prototype.initializeExternalHandlers = function () {
            var _this = this;
            $("#" + this.uscFascicleFoldersId).bind(uscFascicleFolders.LOADED_EVENT, function () {
                _this.loadFascicleFolders();
            });
            this.loadFascicleFolders();
        };
        FascMoveItems.prototype.loadFascicleFolders = function () {
            var uscFascicleFolder = $("#" + this.uscFascicleFoldersId).data();
            if (!jQuery.isEmptyObject(uscFascicleFolder)) {
                uscFascicleFolder.setManageFascicleFolderVisibility(true);
                uscFascicleFolder.setRootNode(this.moveToFascicle ? this.destinationFascicleId : this.idFascicle, "Cartelle del fascicolo");
                uscFascicleFolder.loadFolders(this.moveToFascicle ? this.destinationFascicleId : this.idFascicle);
            }
        };
        FascMoveItems.prototype.getIconByEnvironment = function (env) {
            switch (env) {
                case Environment.Protocol:
                    return "../Comm/Images/DocSuite/Protocollo16.gif";
                case Environment.Resolution:
                    return "../Comm/Images/DocSuite/Atti16.gif";
                case Environment.DocumentSeries:
                    return "../App_Themes/DocSuite2008/imgset16/document_copies.png";
                case Environment.UDS:
                    return "../App_Themes/DocSuite2008/imgset16/document_copies.png";
                default:
                    return "../App_Themes/DocSuite2008/imgset16/document.png";
            }
        };
        FascMoveItems.prototype.moveFolders = function (folderId) {
            var _this = this;
            var promise = $.Deferred();
            var deferredActions = [];
            var _loop_1 = function (toMoveItem) {
                var deferredMoveAction = function () {
                    var promise = $.Deferred();
                    _this._fascicleFolderService.getById(toMoveItem.uniqueId, function (data) {
                        if (!data) {
                            promise.reject("La cartella specificata non esiste. Riprovare.");
                            return;
                        }
                        var model = data;
                        model.ParentInsertId = folderId;
                        _this._fascicleFolderService.updateFascicleFolder(model, UpdateActionType.FascicleMoveToFolder, function (data) { return promise.resolve(); }, function (exception) { return promise.reject(exception); });
                    }, function (exception) { return promise.reject(exception); });
                    return promise.promise();
                };
                deferredActions.push(deferredMoveAction());
            };
            for (var _i = 0, _a = this._toMoveItems; _i < _a.length; _i++) {
                var toMoveItem = _a[_i];
                _loop_1(toMoveItem);
            }
            $.when.apply(null, deferredActions)
                .done(function () { return promise.resolve(); })
                .fail(function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        FascMoveItems.prototype.moveDocuments = function (folderId) {
            var _this = this;
            var promise = $.Deferred();
            var deferredActions = [];
            try {
                var _loop_2 = function (toMoveItem) {
                    var deferredMoveAction = function () {
                        var promise = $.Deferred();
                        _this._fascicleDocumentUnitService.getByDocumentUnitAndFascicle(toMoveItem.uniqueId, _this.idFascicle, function (data) {
                            data.FascicleFolder = {};
                            data.FascicleFolder.UniqueId = folderId;
                            if (_this.moveToFascicle) { // Copia in
                                data.UniqueId = "";
                                data.Fascicle.UniqueId = _this.destinationFascicleId;
                                _this._fascicleDocumentUnitService.insertFascicleUD(data, FascicolableActionType.AutomaticDetection, function (data) { return promise.resolve(); }, function (exception) { return promise.reject(exception); });
                            }
                            else {
                                _this._fascicleDocumentUnitService.updateFascicleUD(data, UpdateActionType.FascicleMoveToFolder, function (data) { return promise.resolve(); }, function (exception) { return promise.reject(exception); });
                            }
                        }, function (exception) { return promise.reject(exception); });
                        return promise.promise();
                    };
                    deferredActions.push(deferredMoveAction());
                };
                for (var _i = 0, _a = this._toMoveItems.filter(function (item) { return item.environment != Environment.Document; }); _i < _a.length; _i++) {
                    var toMoveItem = _a[_i];
                    _loop_2(toMoveItem);
                }
                $.when.apply(null, deferredActions)
                    .done(function () {
                    _this._fascicleDocumentService.getByFolder(_this.moveToFascicle ? _this.destinationFascicleId : _this.idFascicle, folderId, function (data) {
                        var idArchiveChain;
                        if (data && data.length > 0) {
                            idArchiveChain = data[0].IdArchiveChain;
                        }
                        _this._miscellaneaDeferreds.push(promise);
                        _this.moveMiscellaneaDocuments(idArchiveChain);
                    }, function (exception) {
                        promise.reject(exception);
                    });
                })
                    .fail(function (exception) { return promise.reject(exception); });
            }
            catch (error) {
                console.error(error);
                this.showNotificationMessage(this.uscNotificationId, "E' avvenuto un errore durante il processo di sposta documenti");
            }
            return promise.promise();
        };
        FascMoveItems.prototype.moveMiscellaneaDocuments = function (folderChainId) {
            var request = {};
            request.ActionName = FascMoveItems.MOVE_MISCELLANEA_DOCUMENT_AJAX_ACTION_NAME;
            request.Value = [];
            var idDocuments = this._toMoveItems.filter(function (item) { return item.environment == Environment.Document; })
                .map(function (item) { return item.uniqueId; });
            request.Value.push(JSON.stringify(idDocuments));
            request.Value.push(folderChainId);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._ajaxManager.ajaxRequest(JSON.stringify(request));
        };
        FascMoveItems.prototype.updateCurrentMiscellaneaChain = function () {
            var _this = this;
            var promise = $.Deferred();
            this._fascicleDocumentService.getByFolder(this.idFascicle, this.idFascicleFolder, function (data) {
                if (!data || data.length == 0) {
                    promise.resolve();
                    return;
                }
                _this._fascicleDocumentService.updateFascicleDocument(data[0], function (data) { return promise.resolve(); }, function (exception) { return promise.reject(exception); });
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        FascMoveItems.prototype.addMiscellaneaChain = function (idArchiveChain) {
            var promise = $.Deferred();
            var fascicleDocumentModel = {};
            fascicleDocumentModel.ChainType = ChainType.Miscellanea;
            fascicleDocumentModel.IdArchiveChain = idArchiveChain;
            fascicleDocumentModel.Fascicle = new FascicleModel();
            fascicleDocumentModel.Fascicle.UniqueId = this.idFascicle;
            var uscFascicleFolder = $("#" + this.uscFascicleFoldersId).data();
            var selectedFolder = uscFascicleFolder.getSelectedFascicleFolder(this.idFascicle);
            fascicleDocumentModel.FascicleFolder = {};
            fascicleDocumentModel.FascicleFolder.UniqueId = selectedFolder.UniqueId;
            this._fascicleDocumentService.insertFascicleDocument(fascicleDocumentModel, function (data) { return promise.resolve(); }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        FascMoveItems.prototype.closeWindow = function () {
            var wnd = this.getRadWindow();
            wnd.close(true);
        };
        /**
        * Recupera una RadWindow dalla pagina
        */
        FascMoveItems.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        FascMoveItems.prototype.showLoading = function () {
            this._loadingPanel.show(this.pnlPageId);
        };
        FascMoveItems.prototype.hideLoading = function () {
            this._loadingPanel.hide(this.pnlPageId);
        };
        FascMoveItems.MOVE_MISCELLANEA_DOCUMENT_AJAX_ACTION_NAME = "MoveMiscellaneaDocument";
        FascMoveItems.DOCUMENT_ITEMS_TYPE = "DocumentType";
        FascMoveItems.FOLDER_ITEMS_TYPE = "FolderType";
        return FascMoveItems;
    }(FascBase));
    return FascMoveItems;
});
//# sourceMappingURL=FascMoveItems.js.map