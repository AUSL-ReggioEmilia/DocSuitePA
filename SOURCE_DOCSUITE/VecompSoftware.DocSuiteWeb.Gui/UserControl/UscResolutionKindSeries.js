/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO", "App/Services/Resolutions/ResolutionKindDocumentSeriesService", "App/Services/DocumentArchives/DocumentSeriesService", "App/Services/DocumentArchives/DocumentSeriesConstraintService"], function (require, exports, ServiceConfigurationHelper, ExceptionDTO, ResolutionKindDocumentSeriesService, DocumentSeriesService, DocumentSeriesConstraintService) {
    var UscResolutionKindSeries = /** @class */ (function () {
        function UscResolutionKindSeries(serviceConfigurations) {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            this.btnAddSeries_Click = function (sender, args) {
                try {
                    sender.enableAfterSingleClick();
                    _this.openWindow(_this.wndResolutionKindDocumentSeriesId, 'Associa un nuovo archivio alla tipologia atto');
                    _this._loadingManager.show(_this.pnlWindowContentId);
                    _this._rcbArchives.enable();
                    _this.resetCombosSource();
                    _this.panelConstraintsSelectionControl.hide();
                    _this._btnConfirmSeries.set_commandArgument(UscResolutionKindSeries.ADD_SERIES_COMMAND);
                    $("#".concat(_this.chbDocumentRequiredId)).prop("checked", false);
                    _this.loadArchives()
                        .fail(function (exception) { return _this.showNotificationException(exception); })
                        .always(function () { return _this._loadingManager.hide(_this.pnlWindowContentId); });
                }
                catch (e) {
                    console.error(e);
                    _this.showNotificationException("Errore nella gestione della finestra di associazione archivio a tipologia atto");
                }
            };
            this.btnEditSeries_Click = function (sender, args) {
                sender.enableAfterSingleClick();
                if (!_this.currentResolutionKind) {
                    alert("Nessuna tipologia di atto trovata per la modifica");
                    return;
                }
                var selectedArchives = _this._grdDocumentSeries.get_selectedItems();
                if (!selectedArchives || selectedArchives.length == 0) {
                    alert("Selezionare un archivio per la modifica");
                    return;
                }
                try {
                    _this.openWindow(_this.wndResolutionKindDocumentSeriesId, 'Modifica archivio tipologia atto');
                    _this._loadingManager.show(_this.pnlWindowContentId);
                    _this.resetCombosSource();
                    _this._btnConfirmSeries.set_commandArgument(UscResolutionKindSeries.EDIT_SERIES_COMMAND);
                    var model_1 = selectedArchives[0].get_dataItem();
                    $("#".concat(_this.chbDocumentRequiredId)).prop("checked", model_1.DocumentRequired);
                    $("#".concat(_this.pnlPageContentId)).off(UscResolutionKindSeries.CONSTRAINT_LOADED_EVENT_NAME);
                    _this._documentSeriesService.getById(model_1.DocumentSeries.EntityId, function (data) {
                        $("#".concat(_this.pnlPageContentId)).on(UscResolutionKindSeries.CONSTRAINT_LOADED_EVENT_NAME, function () {
                            if (model_1.DocumentSeriesConstraint) {
                                _this._rcbConstraints.requestItems('', true);
                                var constraint = _this._rcbConstraints.findItemByValue(model_1.DocumentSeriesConstraint.UniqueId);
                                if (constraint) {
                                    constraint.select();
                                }
                            }
                        });
                        var item = new Telerik.Web.UI.RadComboBoxItem();
                        item.set_value(model_1.DocumentSeries.EntityId.toString());
                        item.set_text(model_1.DocumentSeries.Name);
                        _this._rcbArchives.get_items().add(item);
                        _this._rcbArchives.get_items().getItem(0).select();
                        _this._rcbArchives.disable();
                        _this._loadingManager.hide(_this.pnlWindowContentId);
                    }, function (exception) {
                        _this._loadingManager.hide(_this.pnlWindowContentId);
                        _this.showNotificationException(exception);
                    });
                }
                catch (e) {
                    console.error(e);
                    _this._loadingManager.hide(_this.pnlWindowContentId);
                    _this.showNotificationException("Errore nella gestione della finestra di associazione archivio a tipologia atto");
                }
            };
            this.btnCancelSeries_Click = function (sender, args) {
                var selectedArchives = _this._grdDocumentSeries.get_selectedItems();
                if (!selectedArchives || selectedArchives.length == 0) {
                    alert("Selezionare un archivio per la cancellazione");
                    sender.enableAfterSingleClick();
                    return;
                }
                var kindSeriesModel = selectedArchives[0].get_dataItem();
                _this._defaultManagerWindows.radconfirm("Sei sicuro di voler rimuovere l'archivio selezionato?", function (arg) {
                    if (arg) {
                        try {
                            var model = {};
                            model.UniqueId = kindSeriesModel.UniqueId;
                            _this._loadingManager.show(_this.grdDocumentSeriesId);
                            _this.saveResolutionKindDocumentSeries(model, UscResolutionKindSeries.DELETE_SERIES_COMMAND)
                                .done(function () { return _this.loadSeries(_this.currentResolutionKind); })
                                .fail(function (exception) { return _this.showNotificationException(exception); })
                                .always(function () { return _this._loadingManager.hide(_this.grdDocumentSeriesId); });
                        }
                        catch (e) {
                            console.error(e);
                            _this.showNotificationException("Errore nella fase di cancellazione archivio da tipologia atto");
                        }
                    }
                    sender.enableAfterSingleClick();
                }, 300, 160);
            };
            this.rcbArchive_SelectedIndexChanged = function (sender, args) {
                _this._rcbConstraints.clearSelection();
                _this._rcbConstraints.clearItems();
                if (!args.get_item()) {
                    _this.panelConstraintsSelectionControl.hide();
                    return;
                }
                var selectedArchive = args.get_item();
                _this.loadConstraints(Number(selectedArchive.get_value()))
                    .done(function (countConstraint) {
                    _this.panelConstraintsSelectionControl.hide();
                    if (countConstraint > 0) {
                        _this.panelConstraintsSelectionControl.show();
                    }
                })
                    .fail(function (exception) { return _this.showNotificationException(exception); });
            };
            this.btnConfirmSeries_Click = function (sender, args) {
                var selectedArchive = _this._rcbArchives.get_selectedItem();
                if (!selectedArchive || !selectedArchive.get_value()) {
                    alert("E' richiesta la selezione di un archivio per il salvataggio");
                    sender.enableAfterSingleClick();
                    return;
                }
                try {
                    var model = {};
                    model.DocumentRequired = $("#".concat(_this.chbDocumentRequiredId)).is(':checked');
                    model.DocumentSeries = {};
                    model.DocumentSeries.EntityId = Number(selectedArchive.get_value());
                    model.ResolutionKind = _this.currentResolutionKind;
                    if (_this._rcbConstraints.get_selectedItem()) {
                        model.DocumentSeriesConstraint = {};
                        model.DocumentSeriesConstraint.UniqueId = _this._rcbConstraints.get_selectedItem().get_value();
                    }
                    if (sender.get_commandArgument() == UscResolutionKindSeries.EDIT_SERIES_COMMAND) {
                        var selectedArchives = _this._grdDocumentSeries.get_selectedItems();
                        var kindSeriesModel = selectedArchives[0].get_dataItem();
                        model.UniqueId = kindSeriesModel.UniqueId;
                    }
                    _this._loadingManager.show(_this.pnlWindowContentId);
                    _this.saveResolutionKindDocumentSeries(model, sender.get_commandArgument())
                        .done(function () {
                        _this.closeWindow(_this.wndResolutionKindDocumentSeriesId);
                        _this._loadingManager.show(_this.grdDocumentSeriesId);
                        _this.loadSeries(_this.currentResolutionKind)
                            .always(function () { return _this._loadingManager.hide(_this.grdDocumentSeriesId); });
                    })
                        .fail(function (exception) { return _this.showNotificationException(exception); })
                        .always(function () {
                        sender.enableAfterSingleClick();
                        _this._loadingManager.hide(_this.pnlWindowContentId);
                    });
                }
                catch (e) {
                    console.error(e);
                    sender.enableAfterSingleClick();
                    _this.showNotificationException("Errore nella fase di salvataggio archivio per tipologia atto");
                }
            };
            this._serviceConfigurations = serviceConfigurations;
        }
        Object.defineProperty(UscResolutionKindSeries.prototype, "panelConstraintsSelectionControl", {
            get: function () {
                return $("#".concat(this.pnlConstraintsSelectionId));
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(UscResolutionKindSeries.prototype, "currentResolutionKind", {
            get: function () {
                var sessionValue = sessionStorage.getItem(UscResolutionKindSeries.SESSION_KIND_KEY);
                if (!sessionValue) {
                    return null;
                }
                return JSON.parse(sessionValue);
            },
            enumerable: true,
            configurable: true
        });
        /**
         *------------------------- Methods -----------------------------
         */
        UscResolutionKindSeries.prototype.initialize = function () {
            this._grdDocumentSeries = $find(this.grdDocumentSeriesId);
            this._btnAddSeries = $find(this.btnAddSeriesId);
            this._btnAddSeries.add_clicked(this.btnAddSeries_Click);
            this._btnEditSeries = $find(this.btnEditSeriesId);
            this._btnEditSeries.add_clicked(this.btnEditSeries_Click);
            this._btnCancelSeries = $find(this.btnCancelSeriesId);
            this._btnCancelSeries.add_clicked(this.btnCancelSeries_Click);
            this._archivesDataSource = $find(this.archivesDataSourceId);
            this._rcbArchives = $find(this.rcbArchivesId);
            this._rcbArchives.add_selectedIndexChanged(this.rcbArchive_SelectedIndexChanged);
            this._rcbConstraints = $find(this.rcbConstraintsId);
            this._constraintsDataSource = $find(this.constraintsDataSourceId);
            this._btnConfirmSeries = $find(this.btnConfirmSeriesId);
            this._btnConfirmSeries.add_clicked(this.btnConfirmSeries_Click);
            this._loadingManager = $find(this.ajaxLoadingPanelId);
            this._windowManager = $find(this.managerWindowsId);
            this._defaultManagerWindows = $find(this.defaultManagerWindowsId);
            var resolutionKindDocumentSeriesConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "ResolutionKindDocumentSeries");
            this._resolutionKindDocumentSeriesService = new ResolutionKindDocumentSeriesService(resolutionKindDocumentSeriesConfiguration);
            var documentSeriesConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentSeries");
            this._documentSeriesService = new DocumentSeriesService(documentSeriesConfiguration);
            var documentSeriesConstraintConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentSeriesConstraint");
            this._documentSeriesConstraintService = new DocumentSeriesConstraintService(documentSeriesConstraintConfiguration);
            sessionStorage.removeItem(UscResolutionKindSeries.SESSION_KIND_KEY);
            this.bindLoaded();
        };
        UscResolutionKindSeries.prototype.bindLoaded = function () {
            $("#".concat(this.pnlPageContentId)).data(this);
        };
        UscResolutionKindSeries.prototype.loadArchives = function () {
            var _this = this;
            var promise = $.Deferred();
            this._documentSeriesService.getAll(function (data) {
                _this._archivesDataSource.set_data(data);
                _this._archivesDataSource.fetch(undefined);
                promise.resolve();
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        UscResolutionKindSeries.prototype.loadConstraints = function (idDocumentSeries) {
            var _this = this;
            var promise = $.Deferred();
            this._documentSeriesConstraintService.getByIdSeries(idDocumentSeries, function (data) {
                if (!data)
                    return promise.resolve(0);
                _this._constraintsDataSource.set_data(data);
                _this._constraintsDataSource.fetch(undefined);
                $("#".concat(_this.pnlPageContentId)).trigger(UscResolutionKindSeries.CONSTRAINT_LOADED_EVENT_NAME);
                promise.resolve(data.length);
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        UscResolutionKindSeries.prototype.loadSeries = function (resolutionKind) {
            var _this = this;
            var promise = $.Deferred();
            if (!resolutionKind) {
                return promise.resolve();
            }
            sessionStorage.setItem(UscResolutionKindSeries.SESSION_KIND_KEY, JSON.stringify(resolutionKind));
            this._resolutionKindDocumentSeriesService.getByResolutionKind(resolutionKind.UniqueId, function (data) {
                if (!data)
                    return;
                var masterTable = _this._grdDocumentSeries.get_masterTableView();
                masterTable.set_dataSource(data);
                masterTable.dataBind();
                masterTable.clearSelectedItems();
                promise.resolve();
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        UscResolutionKindSeries.prototype.saveResolutionKindDocumentSeries = function (model, command) {
            var _this = this;
            var promise = $.Deferred();
            var action;
            switch (command) {
                case UscResolutionKindSeries.ADD_SERIES_COMMAND:
                    {
                        action = function (m, c, e) { return _this._resolutionKindDocumentSeriesService.insertResolutionKindDocumentSeriesModel(m, c, e); };
                    }
                    break;
                case UscResolutionKindSeries.EDIT_SERIES_COMMAND:
                    {
                        action = function (m, c, e) { return _this._resolutionKindDocumentSeriesService.updateResolutionKindDocumentSeriesModel(m, c, e); };
                    }
                    break;
                case UscResolutionKindSeries.DELETE_SERIES_COMMAND:
                    {
                        action = function (m, c, e) { return _this._resolutionKindDocumentSeriesService.deleteResolutionKindDocumentSeriesModel(m, c, e); };
                    }
                    break;
                default:
                    {
                        throw new Error("Command type ".concat(command, " not defined"));
                    }
            }
            action(model, function (data) {
                promise.resolve();
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        UscResolutionKindSeries.prototype.resetCombosSource = function () {
            this._rcbArchives.enable();
            this._rcbArchives.clearItems();
            this._rcbArchives.clearSelection();
            this._rcbConstraints.clearItems();
            this._rcbConstraints.clearSelection();
        };
        /**
        * Apre una nuova nuova RadWindow
        * @param id
        */
        UscResolutionKindSeries.prototype.openWindow = function (id, title) {
            var manager = $find(this.managerWindowsId);
            var wnd = manager.getWindowById(id);
            wnd.show();
            wnd.set_title(title);
            wnd.set_modal(true);
            wnd.center();
            return false;
        };
        /**
         * Chiude una RadWindow specifica
         * @param id
         */
        UscResolutionKindSeries.prototype.closeWindow = function (id) {
            var manager = $find(this.managerWindowsId);
            var wnd = manager.getWindowById(id);
            wnd.close();
        };
        UscResolutionKindSeries.prototype.showNotificationException = function (exception) {
            var uscNotification = $("#".concat(this.uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                if (exception instanceof ExceptionDTO) {
                    uscNotification.showNotification(exception);
                }
                else {
                    uscNotification.showNotificationMessage(exception);
                }
            }
        };
        UscResolutionKindSeries.ADD_SERIES_COMMAND = "addSeries";
        UscResolutionKindSeries.EDIT_SERIES_COMMAND = "editSeries";
        UscResolutionKindSeries.DELETE_SERIES_COMMAND = "deleteSeries";
        UscResolutionKindSeries.SESSION_KIND_KEY = "ResolutionKindKey";
        UscResolutionKindSeries.CONSTRAINT_LOADED_EVENT_NAME = "ConstraintsLoaded";
        return UscResolutionKindSeries;
    }());
    return UscResolutionKindSeries;
});
//# sourceMappingURL=UscResolutionKindSeries.js.map