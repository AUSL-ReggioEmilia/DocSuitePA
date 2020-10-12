/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />
define(["require", "exports", "App/Models/MassimariScarto/MassimarioScartoModel", "App/Models/MassimariScarto/MassimarioScartoStatusType", "App/Helpers/ServiceConfigurationHelper", "App/Services/MassimariScarto/MassimarioScartoService", "App/Services/Commons/CategoryService", "UserControl/uscMassimarioScarto", "App/ViewModels/Telerik/RadTreeNodeViewModel", "App/Helpers/WindowHelper", "App/DTOs/ExceptionDTO", "../app/core/extensions/string"], function (require, exports, MassimarioScartoModel, MassimarioScartoStatusType, ServiceConfigurationHelper, MassimarioScartoService, CategoryService, uscMassimarioScarto, RadTreeNodeViewModel, WindowHelper, ExceptionDTO) {
    var TbltMassimarioScartoGes = /** @class */ (function () {
        /**
         * Costruttore
         */
        function TbltMassimarioScartoGes(serviceConfigurations) {
            var _this = this;
            /**
             *------------------------- Events -----------------------------
             */
            this.folderToolBar_onClick = function (sender, args) {
                switch (args.get_item().get_value()) {
                    case TbltMassimarioScartoGes.CREATE_OPTION: {
                        _this.initializeInsertForm();
                        _this._btnSaveMassimario.set_commandArgument("Insert");
                        _this.openWindow();
                        break;
                    }
                    case TbltMassimarioScartoGes.MODIFY_OPTION: {
                        _this._uscMassimarioScarto = $("#".concat(_this.uscMassimarioScartoId)).data();
                        var selectedModel = _this._uscMassimarioScarto.getSelectedMassimario();
                        _this.initializeEditForm(selectedModel);
                        _this._btnSaveMassimario.set_commandArgument("Edit");
                        _this.openWindow();
                        break;
                    }
                    case TbltMassimarioScartoGes.DELETE_OPTION: {
                        _this.initializeCancelForm();
                        _this._btnSaveMassimario.set_commandArgument("Cancel");
                        _this.openWindow();
                        break;
                    }
                    case TbltMassimarioScartoGes.RECOVER_OPTION: {
                        _this._uscMassimarioScarto = $("#".concat(_this.uscMassimarioScartoId)).data();
                        var selectedModel = _this._uscMassimarioScarto.getSelectedMassimario();
                        _this.initializeRecoverForm(selectedModel);
                        _this._btnSaveMassimario.set_commandArgument("Recover");
                        _this.openWindow();
                        break;
                    }
                }
            };
            /**
             * Evento scatenato al click del pulsante Conferma
             * @method
             * @param sender
             * @param eventArgs
             * @returns
             */
            this.btnSaveMassimario_Clicking = function (sender, eventArgs) {
                eventArgs.set_cancel(true);
                if (!Page_IsValid) {
                    return false;
                }
                _this._btnSaveMassimario.set_enabled(false);
                _this._uscMassimarioScarto = $("#".concat(_this.uscMassimarioScartoId)).data();
                var selectedModel = _this._uscMassimarioScarto.getSelectedMassimario();
                var model = undefined;
                var argument = _this._btnSaveMassimario.get_commandArgument();
                if (argument != "Cancel") {
                    model = _this.fillModelFromPage();
                }
                _this.showLoadingPanel(_this.pnlMetadataId);
                //Salvo l'elemento tramite le web api        
                switch (argument) {
                    case "Insert":
                        model.FakeInsertId = selectedModel.UniqueId;
                        _this._massimarioScartoService.insertMassimario(model, _this.insertMassimarioCallback, function (exception) {
                            _this.showNotificationException(_this.uscNotificationId, exception);
                            _this._btnSaveMassimario.set_enabled(true);
                            _this.closeLoadingPanel(_this.pnlMetadataId);
                        });
                        break;
                    case "Edit":
                        model.FakeInsertId = selectedModel.FakeInsertId;
                        model.UniqueId = selectedModel.UniqueId;
                        _this._massimarioScartoService.updateMassimario(model, _this.updateMassimarioCallback, function (exception) {
                            _this.showNotificationException(_this.uscNotificationId, exception);
                            _this._btnSaveMassimario.set_enabled(true);
                            _this.closeLoadingPanel(_this.pnlMetadataId);
                        });
                        break;
                    case "Recover":
                        model.FakeInsertId = selectedModel.FakeInsertId;
                        model.UniqueId = selectedModel.UniqueId;
                        model.EndDate = null;
                        model.Status = MassimarioScartoStatusType.Active;
                        _this._massimarioScartoService.updateMassimario(model, _this.updateMassimarioCallback, function (exception) {
                            _this.showNotificationException(_this.uscNotificationId, exception);
                            _this._btnSaveMassimario.set_enabled(true);
                            _this.closeLoadingPanel(_this.pnlMetadataId);
                        });
                        break;
                    case "Cancel":
                        _this._uscMassimarioScarto = $("#".concat(_this.uscMassimarioScartoId)).data();
                        selectedModel.Status = MassimarioScartoStatusType.LogicalDelete;
                        var tmp = _this._rdpEndDate.get_selectedDate();
                        selectedModel.EndDate = moment.utc(_this._rdpEndDate.get_selectedDate()).hours(24).minutes(0).seconds(0).milliseconds(0).toDate();
                        _this._massimarioScartoService.updateMassimario(selectedModel, _this.updateMassimarioCallback, function (exception) {
                            _this.showNotificationException(_this.uscNotificationId, exception);
                            _this._btnSaveMassimario.set_enabled(true);
                            _this.closeLoadingPanel(_this.pnlMetadataId);
                        });
                        break;
                }
            };
            /**
             * Evento scatenato all'inserimento di un valore nella radnumerictextbox
             * @method
             * @param sender
             * @param eventArgs
             * @returns
             */
            this.txtPeriod_KeyPress = function (sender, eventArgs) {
                _this._btnInfinite.set_checked(String.isNullOrEmpty(sender.get_value()));
            };
            /**
             * Callback da inserimento nuovo massimario
             */
            this.insertMassimarioCallback = function (data) {
                _this.closeLoadingPanel(_this.pnlMetadataId);
                _this._uscMassimarioScarto.updateSelectedNodeChildren();
                var selectedModel = _this._uscMassimarioScarto.getSelectedMassimario();
                if (selectedModel.UniqueId) {
                    _this.setButtonVisibility(selectedModel);
                }
                else {
                    _this.setButtonVisibility(0, true);
                }
                _this._btnSaveMassimario.set_enabled(true);
                _this.closeWindow();
            };
            /**
             * Callback da modifica massimario
             */
            this.updateMassimarioCallback = function (data) {
                _this.closeLoadingPanel(_this.pnlMetadataId);
                _this._uscMassimarioScarto.updateParentNode(function () {
                    _this.hideDetailsPanel();
                    var selectedModel = _this._uscMassimarioScarto.getSelectedMassimario();
                    if (selectedModel.UniqueId) {
                        _this.setButtonVisibility(selectedModel);
                    }
                    else {
                        _this.setButtonVisibility(0, true);
                    }
                    _this._btnSaveMassimario.set_enabled(true);
                    _this.closeWindow();
                });
            };
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
        }
        /**
         *------------------------- Methods -----------------------------
         */
        /**
     * Metodo di Gestione dell'errore
     */
        TbltMassimarioScartoGes.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
            if (exception && exception instanceof ExceptionDTO) {
                var uscNotification = $("#".concat(uscNotificationId)).data();
                if (!jQuery.isEmptyObject(uscNotification)) {
                    uscNotification.showNotification(exception);
                }
            }
            else {
                this.showNotificationMessage(uscNotificationId, customMessage);
            }
        };
        /**
     * Metodi di Gestione dell'errore
     */
        TbltMassimarioScartoGes.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        /**
         * Metodo di inizializzazione pagina
         */
        TbltMassimarioScartoGes.prototype.initialize = function () {
            var _this = this;
            this._folderToolBar = $find(this.folderToolBarId);
            this._folderToolBar.add_buttonClicked(this.folderToolBar_onClick);
            this._txtName = $find(this.txtNameId);
            this._txtCode = $find(this.txtCodeId);
            this._txtNote = $find(this.txtNoteId);
            this._btnInfinite = $find(this.btnInfiniteId);
            this._txtPeriod = $find(this.txtPeriodId);
            this._txtPeriod.add_valueChanged(this.txtPeriod_KeyPress);
            this._btnSaveMassimario = $find(this.btnSaveMassimarioId);
            this._btnSaveMassimario.add_clicking(this.btnSaveMassimario_Clicking);
            this._rgvCategories = $find(this.rgvCategoriesId);
            this._rdpStartDate = $find(this.rdpStartDateId);
            this._rdpEndDate = $find(this.rdpEndDateId);
            this._manager = $find(this.managerId);
            var massimarioScartoConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "MassimarioScarto");
            this._massimarioScartoService = new MassimarioScartoService(massimarioScartoConfiguration);
            var categoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Category");
            this._categoryService = new CategoryService(categoryConfiguration);
            this.hideDetailsPanel();
            $("#".concat(this.uscMassimarioScartoId)).on(uscMassimarioScarto.ON_SELECTED_NODE_EVENT, function (args, data) {
                if (data != undefined) {
                    var node = new RadTreeNodeViewModel();
                    node.fromJson(data);
                    if (node.value != 0) {
                        _this.loadMassimarioDetails(node.attributes.UniqueId);
                        _this.setButtonVisibility(node.attributes.MassimarioScartoLevel, node.attributes.IsActive);
                    }
                    else {
                        _this.hideDetailsPanel();
                        _this.setButtonVisibility(0, true);
                    }
                }
            });
            $("#".concat(this.uscMassimarioScartoId)).on(uscMassimarioScarto.ON_START_LOAD_EVENT, function (args) {
                _this.showLoadingPanel(_this.splitterPageId);
            });
            this.showLoadingPanel(this.splitterPageId);
            $("#".concat(this.uscMassimarioScartoId)).on(uscMassimarioScarto.ON_END_LOAD_EVENT, function (args) {
                _this.closeLoadingPanel(_this.splitterPageId);
            });
            $("#".concat(this.uscMassimarioScartoId)).on(uscMassimarioScarto.ON_ERROR_EVENT, function (args, exception) {
                _this.showNotificationException(_this.uscNotificationId, exception);
                _this.closeLoadingPanel(_this.splitterPageId);
            });
        };
        /**
          * Visualizza un nuovo loading panel nella pagina
          */
        TbltMassimarioScartoGes.prototype.showLoadingPanel = function (updatedElementId) {
            var ajaxDefaultLoadingPanel = $find(this.ajaxLoadingPanelId);
            ajaxDefaultLoadingPanel.show(updatedElementId);
        };
        /**
         * Nasconde il loading panel nella pagina
         */
        TbltMassimarioScartoGes.prototype.closeLoadingPanel = function (updatedElementId) {
            var ajaxDefaultLoadingPanel = $find(this.ajaxLoadingPanelId);
            ajaxDefaultLoadingPanel.hide(updatedElementId);
        };
        /**
         * Metodo che recupera i metadati di un massimario e li imposta nella pagina.
         * Gestisce anche le logiche di visualizzazione dei pulsanti e pannelli nella pagina.
         * @param massimarioId
         */
        TbltMassimarioScartoGes.prototype.loadMassimarioDetails = function (massimarioId) {
            var _this = this;
            this.showLoadingPanel(this.pnlDetailsId);
            this._massimarioScartoService.getMassimarioById(massimarioId, function (data) {
                if (data == null)
                    return;
                var massimario = data;
                _this._categoryService.getByIdMassimarioScarto(massimarioId, function (data) {
                    var categories = data;
                    if (categories == undefined) {
                        categories = new Array();
                    }
                    _this.setDetailPanelControls(massimario, categories);
                    _this.showDetailsPanel();
                    _this.closeLoadingPanel(_this.pnlDetailsId);
                }, function (exception) {
                    _this.closeLoadingPanel(_this.pnlDetailsId);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }, function (exception) {
                _this.closeLoadingPanel(_this.pnlDetailsId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        /**
         * Imposta i valori per i controlli del pannello dei dettagli di un massimario
         * @param massimario
         * @param categories
         */
        TbltMassimarioScartoGes.prototype.setDetailPanelControls = function (massimario, categories) {
            $("#".concat(this.lblConservationPeriodId)).html(massimario.getPeriodLabel());
            massimario.MassimarioScartoLevel != 2 ? $('#detailPeriodSection').hide() : $('#detailPeriodSection').show();
            $("#".concat(this.lblNoteId)).html(massimario.Note);
            $("#".concat(this.lblStartDateId)).html(moment(massimario.StartDate).format("DD/MM/YYYY"));
            $("#".concat(this.lblEndDateId)).html('');
            if (massimario.EndDate != undefined) {
                $("#".concat(this.lblEndDateId)).html(moment(massimario.EndDate).format("DD/MM/YYYY"));
            }
            var masterTable = this._rgvCategories.get_masterTableView();
            masterTable.set_dataSource(categories);
            masterTable.dataBind();
        };
        TbltMassimarioScartoGes.prototype.setButtonVisibility = function (massimarioModelOrLevel, active) {
            var massimarioLevel = 0;
            if (massimarioModelOrLevel instanceof MassimarioScartoModel) {
                var model = massimarioModelOrLevel;
                if (model.MassimarioScartoLevel != undefined && model.Status != undefined) {
                    massimarioLevel = model.MassimarioScartoLevel;
                    active = model.isActive();
                }
            }
            else {
                massimarioLevel = massimarioModelOrLevel;
            }
            this._uscMassimarioScarto = $("#".concat(this.uscMassimarioScartoId)).data();
            this._folderToolBar.findItemByValue(TbltMassimarioScartoGes.CREATE_OPTION).set_enabled(massimarioLevel < 2 && active);
            this._folderToolBar.findItemByValue(TbltMassimarioScartoGes.MODIFY_OPTION).set_enabled(massimarioLevel > 0 && active);
            this._folderToolBar.findItemByValue(TbltMassimarioScartoGes.DELETE_OPTION).set_enabled(massimarioLevel > 0 && active && this._uscMassimarioScarto.allSelectedChildrenIsCancel());
            this._folderToolBar.findItemByValue(TbltMassimarioScartoGes.RECOVER_OPTION).set_enabled(!active);
        };
        /**
         * Apre una nuova radwindow con dati personalizzati
         * @method
         * @param url
         * @param name
         * @param width
         * @param height
         * @returns
         */
        TbltMassimarioScartoGes.prototype.openWindow = function () {
            var wnd = $find(this.rwEditMassimarioId);
            wnd.setSize(WindowHelper.WIDTH_EDIT_WINDOW, WindowHelper.HEIGHT_EDIT_WINDOW);
            wnd.show();
            return false;
        };
        /**
         * Inizializza la form per l'inserimento di un massimario
         */
        TbltMassimarioScartoGes.prototype.initializeInsertForm = function () {
            var wnd = $find(this.rwEditMassimarioId);
            wnd.set_title("Inserisci massimario di scarto");
            this._txtName.clear();
            this._txtCode.clear();
            this._txtCode.get_element().readOnly = false;
            this._txtNote.clear();
            this._txtPeriod.clear();
            this._btnInfinite.set_checked(true);
            this._rdpStartDate.set_selectedDate(new Date());
            this._rdpEndDate.set_enabled(false);
            this._rdpEndDate.clear();
            this._uscMassimarioScarto = $("#".concat(this.uscMassimarioScartoId)).data();
            var selectedModel = this._uscMassimarioScarto.getSelectedMassimario();
            this.displayRowsForm("Insert", selectedModel.MassimarioScartoLevel != 1);
            //Reset validator
            ValidatorEnable($get(this.rfvCodeId), true);
            ValidatorEnable($get(this.rfvNameId), true);
            $("#".concat(this.rfvNameId)).hide();
            $("#".concat(this.rfvCodeId)).hide();
            ValidatorEnable($get(this.rfvEndDateId), false);
        };
        /**
         * Inizializza la form per la modifica di un massimario
         * @param model
         */
        TbltMassimarioScartoGes.prototype.initializeEditForm = function (model) {
            var wnd = $find(this.rwEditMassimarioId);
            wnd.set_title("Modifica massimario di scarto");
            this._txtName.set_value(model.Name);
            this._txtCode.set_value(model.Code.toString());
            this._txtCode.get_element().readOnly = true;
            this._txtNote.set_value(model.Note);
            this._rdpStartDate.set_selectedDate(moment(model.StartDate).startOf('day').toDate());
            this.displayRowsForm("Edit", model.MassimarioScartoLevel != 2);
            var minDate = moment().startOf('day').toDate();
            this._rdpEndDate.set_enabled(true);
            if (model.EndDate != undefined)
                this._rdpEndDate.set_selectedDate(moment(model.EndDate).startOf('day').toDate());
            else
                this._rdpEndDate.set_selectedDate(undefined);
            ValidatorEnable($get(this.rfvCodeId), true);
            ValidatorEnable($get(this.rfvNameId), true);
            ValidatorEnable($get(this.rfvEndDateId), false);
            if (model.ConservationPeriod != undefined) {
                this._btnInfinite.set_checked(model.ConservationPeriod == -1);
                this._txtPeriod.set_value(model.ConservationPeriod != -1 ? model.getPeriodLabel() : undefined);
            }
            else {
                this._txtPeriod.clear();
                this._btnInfinite.set_checked(true);
            }
        };
        /**
         * Inizializza la form per la cancellazione di un Massimario
         */
        TbltMassimarioScartoGes.prototype.initializeCancelForm = function () {
            var wnd = $find(this.rwEditMassimarioId);
            wnd.set_title("Elimina massimario di scarto");
            var minDate = moment().startOf('day').toDate();
            this._rdpEndDate.set_enabled(true);
            this._rdpEndDate.set_selectedDate(minDate);
            this._rdpEndDate.set_minDate(minDate);
            this.displayRowsForm("Cancel");
            ValidatorEnable($get(this.rfvCodeId), false);
            ValidatorEnable($get(this.rfvNameId), false);
            ValidatorEnable($get(this.rfvEndDateId), true);
        };
        TbltMassimarioScartoGes.prototype.initializeRecoverForm = function (model) {
            var wnd = $find(this.rwEditMassimarioId);
            wnd.set_title("Recupera massimario di scarto");
            if (model) {
                this._txtName.set_value(model.Name);
                this._txtName.get_element().readOnly = true;
                this._txtCode.set_value(model.Code.toString());
                this._txtCode.get_element().readOnly = true;
                this._txtNote.set_value(model.Note);
                this._txtNote.get_element().readOnly = true;
                this._rdpStartDate.set_selectedDate(moment(model.StartDate).startOf('day').toDate());
                this._rdpStartDate.set_enabled(false);
                this._txtPeriod.get_element().readOnly = true;
                if (model.ConservationPeriod != undefined) {
                    this._btnInfinite.set_checked(model.ConservationPeriod == -1);
                    this._txtPeriod.set_value(model.ConservationPeriod != -1 ? model.getPeriodLabel() : undefined);
                }
                else {
                    this._txtPeriod.clear();
                    this._btnInfinite.set_checked(true);
                }
                this.displayRowsForm("Recover", model.MassimarioScartoLevel != 2);
                ValidatorEnable($get(this.rfvCodeId), false);
                ValidatorEnable($get(this.rfvNameId), false);
                ValidatorEnable($get(this.rfvEndDateId), false);
            }
        };
        TbltMassimarioScartoGes.prototype.displayRowsForm = function (commandName, hidePeriods) {
            switch (commandName) {
                case "Insert":
                case "Edit":
                    $('#rowEndDate').show();
                    $('#rowName').show();
                    $('#rowCode').show();
                    $('#rowNote').show();
                    hidePeriods ? $('#rowConservationPeriod').hide() : $('#rowConservationPeriod').show();
                    $('#rowStartDate').show();
                    break;
                case "Recover":
                    $('#rowEndDate').hide();
                    $('#rowName').show();
                    $('#rowCode').show();
                    $('#rowNote').show();
                    $('#rowConservationPeriod').show();
                    $('#rowStartDate').show();
                    break;
                case "Cancel":
                    $('#rowEndDate').show();
                    $('#rowName').hide();
                    $('#rowCode').hide();
                    $('#rowNote').hide();
                    $('#rowConservationPeriod').hide();
                    $('#rowStartDate').hide();
                    break;
            }
        };
        /**
         * Chiude la radwindow attualmente aperta
         */
        TbltMassimarioScartoGes.prototype.closeWindow = function () {
            var wnd = $find(this.rwEditMassimarioId);
            wnd.close();
        };
        /**
         * Nasconde il pannello dei dettagli
         */
        TbltMassimarioScartoGes.prototype.hideDetailsPanel = function () {
            $('#'.concat(this.pnlDetailsId)).hide();
        };
        /**
         * Visualizza il pannello dei dettagli
         */
        TbltMassimarioScartoGes.prototype.showDetailsPanel = function () {
            $('#'.concat(this.pnlDetailsId)).show();
        };
        /**
         * Esegue il fill dei controlli della pagina in un nuovo modello MassimarioScartoModel
         */
        TbltMassimarioScartoGes.prototype.fillModelFromPage = function () {
            var model = new MassimarioScartoModel();
            model.Code = Number(this._txtCode.get_value());
            model.Name = this._txtName.get_value();
            model.Note = this._txtNote.get_value();
            if (!String.isNullOrEmpty(this._txtPeriod.get_value())) {
                model.ConservationPeriod = Number(this._txtPeriod.get_value());
            }
            else {
                model.ConservationPeriod = -1;
            }
            model.StartDate = moment.utc(this._rdpStartDate.get_selectedDate()).hours(24).minutes(0).seconds(0).milliseconds(0).toDate();
            model.EndDate = moment.utc(this._rdpEndDate.get_selectedDate()).hours(24).minutes(0).seconds(0).milliseconds(0).toDate();
            model.Status = MassimarioScartoStatusType.Active;
            return model;
        };
        TbltMassimarioScartoGes.CREATE_OPTION = "create";
        TbltMassimarioScartoGes.MODIFY_OPTION = "modify";
        TbltMassimarioScartoGes.DELETE_OPTION = "delete";
        TbltMassimarioScartoGes.RECOVER_OPTION = "recover";
        return TbltMassimarioScartoGes;
    }());
    return TbltMassimarioScartoGes;
});
//# sourceMappingURL=TbltMassimarioScartoGes.js.map