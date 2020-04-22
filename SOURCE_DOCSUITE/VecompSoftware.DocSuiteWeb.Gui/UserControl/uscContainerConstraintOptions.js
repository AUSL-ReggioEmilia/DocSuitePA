/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Services/DocumentArchives/DocumentSeriesConstraintService", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO"], function (require, exports, DocumentSeriesConstraintService, ServiceConfigurationHelper, ExceptionDTO) {
    var UscContainerConstraintOptions = /** @class */ (function () {
        function UscContainerConstraintOptions(serviceConfigurations) {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            /**
            * Evento scatenato al click della toolbar delle cartelle
            */
            this.rtbConstraintActions_ButtonClicked = function (sender, args) {
                try {
                    var item = args.get_item();
                    if (item) {
                        switch (item.get_value()) {
                            case UscContainerConstraintOptions.CREATE_CONSTRAINT_ACTION: {
                                {
                                    _this._txtConstraintName.set_value('');
                                    _this._btnConfirm.set_commandArgument(UscContainerConstraintOptions.CREATE_CONSTRAINT_ACTION);
                                    _this.openWindow(_this.windowManageConstraintId, "Inserimento nuovo obbligo di trasparenza");
                                }
                                break;
                            }
                            case UscContainerConstraintOptions.EDIT_CONSTRAINT_ACTION: {
                                {
                                    if (!_this.currentSelectedNode.get_value()) {
                                        return;
                                    }
                                    _this._txtConstraintName.set_value(_this.currentSelectedNode.get_text());
                                    _this._btnConfirm.set_commandArgument(UscContainerConstraintOptions.EDIT_CONSTRAINT_ACTION);
                                    _this.openWindow(_this.windowManageConstraintId, "Modifica obbligo di trasparenza");
                                }
                                break;
                            }
                            case UscContainerConstraintOptions.REMOVE_CONSTRAINT_ACTION: {
                                {
                                    if (!_this.currentSelectedNode.get_value()) {
                                        return;
                                    }
                                    _this._windowManager.radconfirm("Sei sicuro di voler rimuovere l'obbligo selezionato?", function (arg) {
                                        if (arg) {
                                            var constraintModel = {};
                                            constraintModel.UniqueId = _this.currentSelectedNode.get_value();
                                            _this.saveConstraint(UscContainerConstraintOptions.REMOVE_CONSTRAINT_ACTION, constraintModel);
                                        }
                                    }, 300, 160);
                                }
                                break;
                            }
                        }
                    }
                }
                catch (e) {
                    console.error(e);
                    _this.showNotificationException("Errore nell'esecuzione dell'attività selezionata");
                }
            };
            this.rtvConstraints_NodeClicked = function (sender, args) {
                var currentNode = args.get_node();
                _this.setNodeActionBehaviours(currentNode);
            };
            this.btnConfirm_Click = function (sender, args) {
                try {
                    var constraintName = _this._txtConstraintName.get_value();
                    if (!constraintName) {
                        alert("E' obbligatorio inserire il nome dell'obbligo di trasparenza");
                        return;
                    }
                    _this.closeWindow(_this.windowManageConstraintId);
                    var constraintModel = void 0;
                    switch (sender.get_commandArgument()) {
                        case UscContainerConstraintOptions.CREATE_CONSTRAINT_ACTION:
                            {
                                constraintModel = {};
                                constraintModel.Name = constraintName;
                                _this.saveConstraint(UscContainerConstraintOptions.CREATE_CONSTRAINT_ACTION, constraintModel);
                            }
                            break;
                        case UscContainerConstraintOptions.EDIT_CONSTRAINT_ACTION:
                            {
                                constraintModel = {};
                                constraintModel.Name = constraintName;
                                constraintModel.UniqueId = _this.currentSelectedNode.get_value();
                                _this.saveConstraint(UscContainerConstraintOptions.EDIT_CONSTRAINT_ACTION, constraintModel);
                            }
                            break;
                    }
                }
                catch (e) {
                    console.error(e);
                    _this.showNotificationException("Errore in gestione dell'obbligo");
                }
            };
            this._serviceConfigurations = serviceConfigurations;
        }
        Object.defineProperty(UscContainerConstraintOptions.prototype, "currentSelectedNode", {
            get: function () {
                return this._rtvConstraints.get_selectedNode();
            },
            enumerable: true,
            configurable: true
        });
        /**
         *------------------------- Methods -----------------------------
         */
        UscContainerConstraintOptions.prototype.initialize = function () {
            this._rtbConstraintActions = $find(this.rtbConstraintActionsId);
            this._rtbConstraintActions.add_buttonClicked(this.rtbConstraintActions_ButtonClicked);
            this._rtvConstraints = $find(this.rtvConstraintsId);
            this._rtvConstraints.add_nodeClicked(this.rtvConstraints_NodeClicked);
            this._txtConstraintName = $find(this.txtConstraintNameId);
            this._btnConfirm = $find(this.btnConfirmId);
            this._btnConfirm.add_clicked(this.btnConfirm_Click);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._windowManager = $find(this.windowManagerId);
            var documentSeriesConstraintConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DocumentSeriesConstraint");
            this._service = new DocumentSeriesConstraintService(documentSeriesConstraintConfiguration);
            this.setNodeActionBehaviours(this._rtvConstraints.get_nodes().getNode(0));
            this.bindLoaded();
        };
        UscContainerConstraintOptions.prototype.bindLoaded = function () {
            $("#".concat(this.splPageContentId)).data(this);
        };
        UscContainerConstraintOptions.prototype.loadConstraints = function (idSeries) {
            var _this = this;
            var promise = $.Deferred();
            sessionStorage.removeItem(UscContainerConstraintOptions.TO_DELETE_STORAGE_KEY);
            this._rtvConstraints.get_nodes().getNode(0).get_nodes().clear();
            this.seriesId = idSeries;
            this._loadingPanel.show(this.splPageContentId);
            this._service.getByIdSeries(idSeries, function (data) {
                if (!data)
                    return;
                var constraints = data;
                var constraintNode;
                for (var _i = 0, constraints_1 = constraints; _i < constraints_1.length; _i++) {
                    var constraint = constraints_1[_i];
                    constraintNode = new Telerik.Web.UI.RadTreeNode();
                    constraintNode.set_text(constraint.Name);
                    constraintNode.set_value(constraint.UniqueId);
                    constraintNode.set_imageUrl('../App_Themes/DocSuite2008/imgset16/information.png');
                    constraintNode.get_attributes().setAttribute(UscContainerConstraintOptions.PERSISTED_ATTRIBUTE, true);
                    _this._rtvConstraints.get_nodes().getNode(0).get_nodes().add(constraintNode);
                }
                _this._rtvConstraints.get_nodes().getNode(0).select();
                _this.setNodeActionBehaviours(_this._rtvConstraints.get_nodes().getNode(0));
                _this._rtvConstraints.get_nodes().getNode(0).expand();
                _this._loadingPanel.hide(_this.splPageContentId);
                promise.resolve();
            }, function (exception) {
                promise.reject(exception);
                _this._loadingPanel.hide(_this.splPageContentId);
                _this.showNotificationException(exception);
            });
            return promise.promise();
        };
        UscContainerConstraintOptions.prototype.setNodeActionBehaviours = function (nodeToCheck) {
            var createCommandButton = this._rtbConstraintActions.findItemByValue(UscContainerConstraintOptions.CREATE_CONSTRAINT_ACTION);
            (!nodeToCheck.get_value()) ? createCommandButton.enable() : createCommandButton.disable();
            var editCommandButton = this._rtbConstraintActions.findItemByValue(UscContainerConstraintOptions.EDIT_CONSTRAINT_ACTION);
            (nodeToCheck.get_value()) ? editCommandButton.enable() : editCommandButton.disable();
            var removeCommandButton = this._rtbConstraintActions.findItemByValue(UscContainerConstraintOptions.REMOVE_CONSTRAINT_ACTION);
            (nodeToCheck.get_value()) ? removeCommandButton.enable() : removeCommandButton.disable();
        };
        UscContainerConstraintOptions.prototype.saveConstraint = function (command, model) {
            var _this = this;
            if (!model) {
                return;
            }
            var action;
            switch (command) {
                case UscContainerConstraintOptions.CREATE_CONSTRAINT_ACTION:
                    {
                        action = function (m, c, e) { return _this._service.insertDocumentSeriesConstraint(m, c, e); };
                    }
                    break;
                case UscContainerConstraintOptions.EDIT_CONSTRAINT_ACTION:
                    {
                        action = function (m, c, e) { return _this._service.updateDocumentSeriesConstraint(m, c, e); };
                    }
                    break;
                case UscContainerConstraintOptions.REMOVE_CONSTRAINT_ACTION:
                    {
                        action = function (m, c, e) { return _this._service.deleteDocumentSeriesConstraint(m, c, e); };
                    }
                    break;
            }
            model.DocumentSeries = {};
            model.DocumentSeries.EntityId = this.seriesId;
            this._loadingPanel.show(this.splPageContentId);
            action(model, function (data) {
                _this._loadingPanel.hide(_this.splPageContentId);
                _this.loadConstraints(_this.seriesId);
            }, function (exception) {
                _this._loadingPanel.hide(_this.splPageContentId);
                _this.showNotificationException(exception);
            });
        };
        /**
        * Apre una nuova nuova RadWindow
        * @param id
        */
        UscContainerConstraintOptions.prototype.openWindow = function (id, title) {
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
        UscContainerConstraintOptions.prototype.closeWindow = function (id) {
            var manager = $find(this.managerWindowsId);
            var wnd = manager.getWindowById(id);
            wnd.close();
        };
        UscContainerConstraintOptions.prototype.showNotificationException = function (exception) {
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
        UscContainerConstraintOptions.CREATE_CONSTRAINT_ACTION = "createConstraint";
        UscContainerConstraintOptions.EDIT_CONSTRAINT_ACTION = "editConstraint";
        UscContainerConstraintOptions.REMOVE_CONSTRAINT_ACTION = "removeConstraint";
        UscContainerConstraintOptions.NODE_COMMAND_ATTRIBUTE = "NodeCommandType";
        UscContainerConstraintOptions.PERSISTED_ATTRIBUTE = "IsAlreadyPersisted";
        UscContainerConstraintOptions.TO_DELETE_STORAGE_KEY = "ConstraintsToDelete";
        return UscContainerConstraintOptions;
    }());
    return UscContainerConstraintOptions;
});
//# sourceMappingURL=uscContainerConstraintOptions.js.map