/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Helpers/ServiceConfigurationHelper", "App/Services/Resolutions/ResolutionKindService", "App/DTOs/ExceptionDTO"], function (require, exports, ServiceConfigurationHelper, ResolutionKindService, ExceptionDTO) {
    var ReslTipologia = /** @class */ (function () {
        function ReslTipologia(serviceConfigurations) {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            this.btnAdd_Click = function (sender, args) {
                _this._txtKindName.set_value('');
                $("#".concat(_this.rcbKindActiveId)).prop("checked", true);
                _this._btnConfirm.set_commandArgument(ReslTipologia.ADD_KIND_COMMAND);
                _this.openWindow(_this.wndResolutionKindId, "Inserimento nuova tipologia di atto");
            };
            this.btnEdit_Click = function (sender, args) {
                if (!_this.currentSelectedNode || !_this.currentSelectedNode.get_value()) {
                    alert("Selezionare una tipologia di atto per la modifica");
                    return;
                }
                _this._txtKindName.set_value(_this.currentSelectedNode.get_text());
                var isActive = _this.currentSelectedNode.get_attributes().getAttribute(ReslTipologia.ISACTIVE_ATTRIBUTE_NODE);
                $("#".concat(_this.rcbKindActiveId)).prop("checked", isActive);
                _this._btnConfirm.set_commandArgument(ReslTipologia.EDIT_KIND_COMMAND);
                _this.openWindow(_this.wndResolutionKindId, "Modifica tipologia di atto");
            };
            this.btnCancel_Click = function (sender, args) {
                if (!_this.currentSelectedNode || !_this.currentSelectedNode.get_value()) {
                    alert("Selezionare una tipologia di atto per la cancellazione");
                    sender.enableAfterSingleClick();
                    return;
                }
                _this._defaultManagerWindows.radconfirm("Sei sicuro di voler rimuovere la tipologia selezionata?", function (arg) {
                    if (arg) {
                        try {
                            var model = {};
                            model.UniqueId = _this.currentSelectedNode.get_value();
                            _this._loadingManager.show(_this.rtvResolutionKindsId);
                            _this.saveResolutionKind(model, ReslTipologia.DELETE_KIND_COMMAND)
                                .done(function () {
                                _this.loadTipologies()
                                    .fail(function (exception) { return _this.showNotificationException(exception); })
                                    .always(function () { return _this._loadingManager.hide(_this.rtvResolutionKindsId); });
                            })
                                .fail(function (exception) {
                                _this._loadingManager.hide(_this.rtvResolutionKindsId);
                                _this.showNotificationException(exception);
                            });
                        }
                        catch (e) {
                            console.error(e);
                            _this.showNotificationException("Errore nella fase di cancellazione tipologia atto");
                        }
                    }
                    sender.enableAfterSingleClick();
                }, 300, 160);
            };
            this.btnRestore_Click = function (sender, args) {
                if (!_this.currentSelectedNode || !_this.currentSelectedNode.get_value()) {
                    alert("Selezionare una tipologia di atto per il recupero");
                    sender.enableAfterSingleClick();
                    return;
                }
                _this._defaultManagerWindows.radconfirm("Sei sicuro di voler recuperare la tipologia selezionata?", function (arg) {
                    if (arg) {
                        try {
                            var model = {};
                            model.Name = _this.currentSelectedNode.get_text();
                            model.UniqueId = _this.currentSelectedNode.get_value();
                            model.IsActive = true;
                            _this._loadingManager.show(_this.rtvResolutionKindsId);
                            _this.saveResolutionKind(model, ReslTipologia.EDIT_KIND_COMMAND)
                                .done(function () {
                                _this.loadTipologies()
                                    .fail(function (exception) { return _this.showNotificationException(exception); })
                                    .always(function () { return _this._loadingManager.hide(_this.rtvResolutionKindsId); });
                            })
                                .fail(function (exception) {
                                _this._loadingManager.hide(_this.rtvResolutionKindsId);
                                _this.showNotificationException(exception);
                            });
                        }
                        catch (e) {
                            console.error(e);
                            _this.showNotificationException("Errore nella fase di recupero tipologia atto");
                        }
                    }
                    sender.enableAfterSingleClick();
                }, 300, 160);
            };
            this.rtvResolutionKinds_NodeClicked = function (sender, args) {
                _this.setButtonsBehaviors(args.get_node());
                if (!args.get_node().get_value()) {
                    $("#".concat(_this.pnlDetailsId)).hide();
                    return;
                }
                $("#".concat(_this.pnlDetailsId)).show();
                _this._loadingManager.show(_this.pnlDetailsId);
                _this.loadDetails(args.get_node())
                    .fail(function (exception) { return _this.showNotificationException(exception); })
                    .always(function () { return _this._loadingManager.hide(_this.pnlDetailsId); });
            };
            this.btnConfirm_Click = function (sender, args) {
                if (!_this._txtKindName.get_value()) {
                    alert("Nessun nome definito per la tipologia di atto");
                    sender.enableAfterSingleClick();
                    return;
                }
                try {
                    var model = {};
                    model.Name = _this._txtKindName.get_value();
                    model.IsActive = $("#".concat(_this.rcbKindActiveId)).is(':checked');
                    switch (sender.get_commandArgument()) {
                        case ReslTipologia.EDIT_KIND_COMMAND:
                            {
                                model.UniqueId = _this.currentSelectedNode.get_value();
                            }
                            break;
                    }
                    _this._loadingManager.show(_this.pnlWindowContentId);
                    _this.saveResolutionKind(model, sender.get_commandArgument())
                        .done(function () {
                        _this.closeWindow(_this.wndResolutionKindId);
                        _this._loadingManager.show(_this.rtvResolutionKindsId);
                        _this.loadTipologies()
                            .fail(function (exception) { return _this.showNotificationException(exception); })
                            .always(function () { return _this._loadingManager.hide(_this.rtvResolutionKindsId); });
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
                    _this.showNotificationException("Errore nella fase di salvataggio della tipologia atto");
                }
            };
            this.tlbStatusSearch_ButtonClicked = function (sender, args) {
                _this._loadingManager.show(_this.rtvResolutionKindsId);
                _this.loadTipologies()
                    .fail(function (exception) { return _this.showNotificationException(exception); })
                    .always(function () { return _this._loadingManager.hide(_this.rtvResolutionKindsId); });
            };
            this._serviceConfigurations = serviceConfigurations;
        }
        Object.defineProperty(ReslTipologia.prototype, "searchDisabled", {
            get: function () {
                var toolBarButton = this._tlbStatusSearch.findItemByValue("searchDisabled");
                if (toolBarButton) {
                    return toolBarButton.get_checked();
                }
                return false;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(ReslTipologia.prototype, "searchActive", {
            get: function () {
                var toolBarButton = this._tlbStatusSearch.findItemByValue("searchActive");
                if (toolBarButton) {
                    return toolBarButton.get_checked();
                }
                return false;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(ReslTipologia.prototype, "rootTreeNode", {
            get: function () {
                return this._rtvResolutionKinds.get_nodes().getNode(0);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(ReslTipologia.prototype, "currentSelectedNode", {
            get: function () {
                return this._rtvResolutionKinds.get_selectedNode();
            },
            enumerable: true,
            configurable: true
        });
        /**
         *------------------------- Methods -----------------------------
         */
        ReslTipologia.prototype.initialize = function () {
            var _this = this;
            this._tlbStatusSearch = $find(this.tlbStatusSearchId);
            this._tlbStatusSearch.add_buttonClicked(this.tlbStatusSearch_ButtonClicked);
            this._rtvResolutionKinds = $find(this.rtvResolutionKindsId);
            this._rtvResolutionKinds.add_nodeClicked(this.rtvResolutionKinds_NodeClicked);
            this._btnAdd = $find(this.btnAddId);
            this._btnAdd.add_clicked(this.btnAdd_Click);
            this._btnEdit = $find(this.btnEditId);
            this._btnEdit.add_clicked(this.btnEdit_Click);
            this._btnCancel = $find(this.btnCancelId);
            this._btnCancel.add_clicked(this.btnCancel_Click);
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicked(this.btnConfirm_Click);
            this._txtKindName = $find(this.txtKindNameId);
            this._loadingManager = $find(this.ajaxLoadingPanelId);
            this._defaultManagerWindows = $find(this.defaultManagerWindowsId);
            this._btnRestore = $find(this.btnRestoreId);
            this._btnRestore.add_clicked(this.btnRestore_Click);
            var resolutionKindConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "ResolutionKind");
            this._resolutionKindService = new ResolutionKindService(resolutionKindConfiguration);
            $("#".concat(this.pnlDetailsId)).hide();
            this.setButtonsBehaviors(this.rootTreeNode);
            this._loadingManager.show(this.rtvResolutionKindsId);
            this.loadTipologies()
                .fail(function (exception) { return _this.showNotificationException(exception); })
                .always(function () { return _this._loadingManager.hide(_this.rtvResolutionKindsId); });
        };
        ReslTipologia.prototype.loadTipologies = function () {
            var _this = this;
            var promise = $.Deferred();
            this.rootTreeNode.get_nodes().clear();
            var action;
            switch (true) {
                case this.searchActive && !this.searchDisabled:
                    action = function (c, e) { return _this._resolutionKindService.findActiveTypologies(c, e); };
                    break;
                case this.searchActive && this.searchDisabled:
                case !this.searchActive && !this.searchDisabled:
                    action = function (c, e) { return _this._resolutionKindService.findAllTypologies(c, e); };
                    break;
                case this.searchDisabled && !this.searchActive:
                    action = function (c, e) { return _this._resolutionKindService.findDisabledTypologies(c, e); };
                    break;
                default:
            }
            action(function (data) {
                if (!data)
                    return;
                try {
                    var resolutionKinds = data;
                    var node = void 0;
                    for (var _i = 0, resolutionKinds_1 = resolutionKinds; _i < resolutionKinds_1.length; _i++) {
                        var resolutionKind = resolutionKinds_1[_i];
                        node = new Telerik.Web.UI.RadTreeNode();
                        node.set_text(resolutionKind.Name);
                        node.set_value(resolutionKind.UniqueId);
                        node.get_attributes().setAttribute(ReslTipologia.ISACTIVE_ATTRIBUTE_NODE, resolutionKind.IsActive);
                        if (resolutionKind.IsActive) {
                            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/type_definition.png");
                        }
                        else {
                            node.set_imageUrl("../App_Themes/DocSuite2008/imgset16/type_definition_private.png");
                            node.set_cssClass("node-disabled");
                        }
                        _this.rootTreeNode.get_nodes().add(node);
                    }
                    _this.rootTreeNode.expand();
                    _this.rootTreeNode.select();
                    _this.setButtonsBehaviors(_this.rootTreeNode);
                    $("#".concat(_this.pnlDetailsId)).hide();
                    promise.resolve();
                }
                catch (e) {
                    console.error(e);
                    promise.reject("Errore nel caricamento delle tipologie di atto");
                }
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        ReslTipologia.prototype.setButtonsBehaviors = function (nodeSelected) {
            var rootNodeSelected = !!!nodeSelected.get_value();
            this._btnAdd.set_enabled(rootNodeSelected);
            this._btnEdit.set_enabled(!rootNodeSelected);
            var isActive = Boolean(nodeSelected.get_attributes().getAttribute(ReslTipologia.ISACTIVE_ATTRIBUTE_NODE));
            this._btnCancel.set_enabled(!rootNodeSelected && isActive);
            this._btnRestore.set_enabled(!rootNodeSelected && !isActive);
        };
        ReslTipologia.prototype.saveResolutionKind = function (model, command) {
            var _this = this;
            var promise = $.Deferred();
            var action;
            switch (command) {
                case ReslTipologia.ADD_KIND_COMMAND:
                    {
                        action = function (m, c, e) { return _this._resolutionKindService.insertResolutionKindModel(m, c, e); };
                    }
                    break;
                case ReslTipologia.EDIT_KIND_COMMAND:
                    {
                        action = function (m, c, e) { return _this._resolutionKindService.updateResolutionKindModel(m, c, e); };
                    }
                    break;
                case ReslTipologia.DELETE_KIND_COMMAND:
                    {
                        action = function (m, c, e) { return _this._resolutionKindService.deleteResolutionKindModel(m, c, e); };
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
        ReslTipologia.prototype.loadDetails = function (selectedNode) {
            var _this = this;
            var promise = $.Deferred();
            var idResolutionKind = selectedNode.get_value();
            this._resolutionKindService.getById(idResolutionKind, function (data) {
                var resolutionKind = data;
                var uscResolutionKindDetails = $("#".concat(_this.uscResolutionKindDetailsId)).data();
                if (!jQuery.isEmptyObject(uscResolutionKindDetails)) {
                    uscResolutionKindDetails.loadDetails(resolutionKind);
                }
                var uscResolutionKindSeries = $("#".concat(_this.uscResolutionKindSeriesId)).data();
                if (!jQuery.isEmptyObject(uscResolutionKindSeries)) {
                    uscResolutionKindSeries.loadSeries(resolutionKind);
                }
                promise.resolve();
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        /**
        * Apre una nuova nuova RadWindow
        * @param id
        */
        ReslTipologia.prototype.openWindow = function (id, title) {
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
        ReslTipologia.prototype.closeWindow = function (id) {
            var manager = $find(this.managerWindowsId);
            var wnd = manager.getWindowById(id);
            wnd.close();
        };
        ReslTipologia.prototype.showNotificationException = function (exception) {
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
        ReslTipologia.ADD_KIND_COMMAND = "createKind";
        ReslTipologia.EDIT_KIND_COMMAND = "editKind";
        ReslTipologia.DELETE_KIND_COMMAND = "deleteKind";
        ReslTipologia.ISACTIVE_ATTRIBUTE_NODE = "isActive";
        return ReslTipologia;
    }());
    return ReslTipologia;
});
//# sourceMappingURL=ReslTipologia.js.map