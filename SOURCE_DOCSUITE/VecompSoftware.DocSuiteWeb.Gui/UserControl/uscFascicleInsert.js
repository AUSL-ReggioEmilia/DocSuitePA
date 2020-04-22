/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
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
define(["require", "exports", "App/Models/Fascicles/FascicleModel", "App/Models/Fascicles/FascicleRoleModel", "App/Models/Fascicles/FascicleType", "App/Models/Fascicles/VisibilityType", "Fasc/FascBase", "App/Helpers/ServiceConfigurationHelper", "UserControl/uscMetadataRepositorySel", "App/Helpers/EnumHelper", "App/Services/Commons/ContainerService"], function (require, exports, FascicleModel, FascicleRoleModel, FascicleType, VisibilityType, FascicleBase, ServiceConfigurationHelper, UscMetadataRepositorySel, EnumHelper, ContainerService) {
    var uscFascicleInsert = /** @class */ (function (_super) {
        __extends(uscFascicleInsert, _super);
        /**
         * Costruttore
         */
        function uscFascicleInsert(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME)) || this;
            _this._deferredInitializeActions = [];
            _this._deferredFascicleSelectedTypeActions = [];
            /**
             *------------------------- Events -----------------------------
             */
            /**
             * Evento scatenato al cambio di selezione del classificatore
             * @param conservationYear
             */
            _this.onCategoryChanged = function (conservationYear, idMetadataRepository) {
                if (_this.selectedFascicleType == FascicleType.Procedure) {
                    var txtConservation = $find(_this.txtConservationId);
                    if (conservationYear && Number(conservationYear) != -1)
                        txtConservation.set_value(conservationYear);
                    else
                        txtConservation.set_value("0");
                }
                if (_this.fascicleContainerEnabled && _this.selectedFascicleType == FascicleType.Procedure) {
                    _this.initializeContainers()
                        .fail(function (exception) { return _this.showNotificationException(_this.uscNotificationId, exception); });
                }
                if (_this.metadataRepositoryEnabled) {
                    var uscMetadataRepositorySel = $("#".concat(_this.uscMetadataRepositorySelId)).data();
                    if (!jQuery.isEmptyObject(uscMetadataRepositorySel)) {
                        _this.setMetadataRepositorySelectedIndexEvent();
                        uscMetadataRepositorySel.clearComboboxText();
                        if (!!idMetadataRepository) {
                            uscMetadataRepositorySel.setComboboxText(idMetadataRepository);
                            var uscDynamicMetadata = $("#".concat(_this.uscDynamicMetadataId)).data();
                            if (!jQuery.isEmptyObject(uscDynamicMetadata)) {
                                uscDynamicMetadata.loadDynamicMetadata(idMetadataRepository);
                            }
                        }
                    }
                }
            };
            /**
             * Evento scatenato al cambio di selezione di una tipologia di Fascicolo
             * @param sender
             * @param args
             */
            _this.rdlFascicleType_onSelectedIndexChanged = function (sender, args) {
                _this.enableValidatorsByFasciceType();
                var uscClassificatore = $("#".concat(_this.uscClassificatoreId)).data();
                if (!jQuery.isEmptyObject(uscClassificatore)) {
                    uscClassificatore.setFascicleTypeParam(_this.selectedFascicleType);
                    uscClassificatore.setShowAuthorizedParam(_this.selectedFascicleType != FascicleType.Activity);
                }
                if (_this.activityFascicleEnabled && !_this.selectedFascicleType) {
                    _this.initializeEmptyFascicleTypeSelected();
                    return;
                }
                _this.setPageBehaviourAfterFascicleTypeChanged();
            };
            _this.ddlContainer_onSelectedIndexChanged = function (sender, args) {
                var uscClassificatore = $("#".concat(_this.uscClassificatoreId)).data();
                if (!jQuery.isEmptyObject(uscClassificatore)) {
                    uscClassificatore.setShowContainerParam(_this.selectedContainer);
                }
            };
            //contact: number
            _this.getFascicle = function () {
                var fascicleModel = new FascicleModel();
                fascicleModel.Conservation = null;
                if (_this.selectedFascicleType == FascicleType.Procedure) {
                    var txtConservation = $find(_this.txtConservationId);
                    fascicleModel.Conservation = Number(txtConservation.get_value());
                }
                else if (_this.selectedFascicleType == FascicleType.Activity) {
                    fascicleModel.VisibilityType = VisibilityType.Confidential;
                }
                fascicleModel.FascicleType = _this.selectedFascicleType;
                fascicleModel.StartDate = _this._radStartDate.get_selectedDate();
                fascicleModel.Note = _this._txtNote.get_value();
                if (_this.fascicleContainerEnabled && (_this.selectedFascicleType == FascicleType.Period
                    || _this.selectedFascicleType == FascicleType.Procedure)) {
                    fascicleModel.Container = {};
                    fascicleModel.Container.EntityShortId = _this.selectedContainer;
                }
                var uscOggetto = $("#".concat(_this.uscOggettoId)).data();
                if (!jQuery.isEmptyObject(uscOggetto)) {
                    fascicleModel.FascicleObject = uscOggetto.getText();
                }
                var uscClassificatore = $("#".concat(_this.uscClassificatoreId)).data();
                if (!jQuery.isEmptyObject(uscClassificatore)) {
                    var selectedCategory = uscClassificatore.getSelectedCategory();
                    if (selectedCategory) {
                        fascicleModel.Category = selectedCategory;
                    }
                }
                //TO DO: quando lo user control dei contatti sarà client side si potrà chiamare come i settori ed il classificatore
                //if (fascicleModel.FascicleType != FascicleType.Activity) {
                //    let contactModel: ContactModel = <ContactModel>{};
                //    contactModel.EntityId = contact;
                //    fascicleModel.Contacts.push(contactModel);
                //}
                var uscRoles = $("#".concat(_this.uscSettoriId)).data();
                if (!jQuery.isEmptyObject(uscRoles)) {
                    if (_this.selectedFascicleType == FascicleType.Procedure) {
                        fascicleModel.VisibilityType = uscRoles.getFascicleVisibilityType();
                    }
                    var source = JSON.parse(uscRoles.getRoles());
                    if (source != null) {
                        var role = void 0;
                        var fascicleRole = void 0;
                        for (var _i = 0, source_1 = source; _i < source_1.length; _i++) {
                            var s = source_1[_i];
                            role = {};
                            fascicleRole = new FascicleRoleModel();
                            role.IdRole = s.EntityShortId;
                            role.EntityShortId = s.EntityShortId;
                            role.TenantId = s.TenantId;
                            fascicleRole.Role = role;
                            fascicleModel.FascicleRoles.push(fascicleRole);
                        }
                    }
                }
                if (fascicleModel.FascicleType != FascicleType.Activity) {
                    var uscMasterRoles = $("#".concat(_this.uscMasterRolesId)).data();
                    if (!jQuery.isEmptyObject(uscMasterRoles)) {
                        var source = JSON.parse(uscMasterRoles.getRoles());
                        if (source != null) {
                            var role = void 0;
                            var fascicleRole = void 0;
                            for (var _a = 0, source_2 = source; _a < source_2.length; _a++) {
                                var s = source_2[_a];
                                role = {};
                                fascicleRole = new FascicleRoleModel();
                                role.IdRole = s.EntityShortId;
                                role.EntityShortId = s.EntityShortId;
                                role.TenantId = s.TenantId;
                                fascicleRole.Role = role;
                                fascicleRole.IsMaster = true;
                                fascicleModel.FascicleRoles.push(fascicleRole);
                            }
                        }
                    }
                }
                return fascicleModel;
            };
            _this.getSelectedFascicleType = function () {
                return _this._rdlFascicleType.get_selectedItem().get_value();
            };
            _this.enableValidators = function (enabled) {
                ValidatorEnable($get(_this.rfvConservationId), enabled);
                var uscMasterRoles = $("#".concat(_this.uscMasterRolesId)).data();
                if (!jQuery.isEmptyObject(uscMasterRoles)) {
                    uscMasterRoles.enableValidators(enabled);
                }
                var uscRoles = $("#".concat(_this.uscSettoriId)).data();
                if (!jQuery.isEmptyObject(uscRoles)) {
                    uscRoles.enableValidators(enabled);
                }
                var uscOggetto = $("#".concat(_this.uscOggettoId)).data();
                if (!jQuery.isEmptyObject(uscOggetto)) {
                    uscOggetto.enableVaidators(enabled);
                }
                var uscContattiResp = $("#".concat(_this.uscContattiRespId)).data();
                if (!jQuery.isEmptyObject(uscContattiResp)) {
                    uscContattiResp.enableValidators(enabled);
                }
            };
            _this.enableValidatorsByFasciceType = function () {
                var selectedType = _this._rdlFascicleType.get_selectedItem().get_value();
                if (String.isNullOrEmpty(selectedType)) {
                    _this.enableValidators(true);
                    return;
                }
                var uscOggetto = $("#".concat(_this.uscOggettoId)).data();
                if (!jQuery.isEmptyObject(uscOggetto)) {
                    uscOggetto.enableVaidators(true);
                }
                var isProcedure = false;
                if (selectedType == FascicleType.Procedure.toString()) {
                    isProcedure = true;
                }
                ValidatorEnable($get(_this.rfvConservationId), isProcedure);
                var uscMasterRoles = $("#".concat(_this.uscMasterRolesId)).data();
                if (!jQuery.isEmptyObject(uscMasterRoles)) {
                    uscMasterRoles.enableValidators(isProcedure);
                }
                var uscRoles = $("#".concat(_this.uscSettoriId)).data();
                if (!jQuery.isEmptyObject(uscRoles)) {
                    uscRoles.enableValidators(!isProcedure);
                }
                var uscContattiResp = $("#".concat(_this.uscContattiRespId)).data();
                if (!jQuery.isEmptyObject(uscContattiResp)) {
                    uscContattiResp.enableValidators(isProcedure);
                }
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        Object.defineProperty(uscFascicleInsert.prototype, "selectedFascicleType", {
            get: function () {
                if (this._rdlFascicleType.get_selectedItem().get_value()) {
                    return Number(this._rdlFascicleType.get_selectedItem().get_value());
                }
                return null;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(uscFascicleInsert.prototype, "selectedContainer", {
            get: function () {
                if (this._ddlContainer.get_selectedItem() && this._ddlContainer.get_selectedItem().get_value()) {
                    return Number(this._ddlContainer.get_selectedItem().get_value());
                }
                return null;
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(uscFascicleInsert.prototype, "fascicleDataRow", {
            get: function () {
                return $("#" + this.fascicleDataRowId);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(uscFascicleInsert.prototype, "contattiRespRow", {
            get: function () {
                return $("#" + this.contattiRespRowId);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(uscFascicleInsert.prototype, "isMasterRow", {
            get: function () {
                return $("#" + this.isMasterRowId);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(uscFascicleInsert.prototype, "startDateRow", {
            get: function () {
                return $("#" + this.rowStartDateId);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(uscFascicleInsert.prototype, "metadataRepositoryRow", {
            get: function () {
                return $("#" + this.metadataRepositoryRowId);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(uscFascicleInsert.prototype, "fascicleTypologyRow", {
            get: function () {
                return $("#" + this.fascicleTypologyRowId);
            },
            enumerable: true,
            configurable: true
        });
        Object.defineProperty(uscFascicleInsert.prototype, "containerRow", {
            get: function () {
                return $("#" + this.containerRowId);
            },
            enumerable: true,
            configurable: true
        });
        /**
         * Initialize
         */
        uscFascicleInsert.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._rdlFascicleType = $find(this.rdlFascicleTypeId);
            this._rdlFascicleType.add_selectedIndexChanged(this.rdlFascicleType_onSelectedIndexChanged);
            this._txtNote = $find(this.txtNoteId);
            this._radStartDate = $find(this.radStartDateId);
            this._radStartDate.set_selectedDate(moment().toDate());
            this._enumHelper = new EnumHelper();
            this._ddlContainer = $find(this.ddlContainerId);
            this._ddlContainer.add_selectedIndexChanged(this.ddlContainer_onSelectedIndexChanged);
            var containerServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Container");
            this._containerService = new ContainerService(containerServiceConfiguration);
            if (this.pnlConservationId) {
                var txtConservation = $find(this.txtConservationId);
                txtConservation.set_value("0");
            }
            this.metadataRepositoryRow.hide();
            if (this.selectedFascicleType && this.selectedFascicleType == FascicleType.Period) {
                this.initializeFasciclePeriodic();
            }
            else if (!this.activityFascicleEnabled) {
                this.setProcedureTypeSelected();
            }
            if (this.activityFascicleEnabled && !this.selectedFascicleType) {
                this.initializeEmptyFascicleTypeSelected();
            }
            if (this.metadataRepositoryEnabled) {
                this.metadataRepositoryRow.show();
                this.setMetadataRepositorySelectedIndexEvent();
            }
            this._loadingPanel.show(this.fasciclePageContentId);
            this.checkAutorizations()
                .done(function (result) {
                if (!result) {
                    _this._loadingPanel.hide(_this.fasciclePageContentId);
                    _this.showNotificationMessage(_this.uscNotificationId, "Errore in inizializzazione pagina.<br \> Utente non autorizzato all'inserimento di un nuovo fascicolo");
                    return;
                }
                var deferredInitializeAction = function () {
                    var promise = $.Deferred();
                    _this._deferredInitializeActions.push(promise);
                    var initializeContainersAction = function () { return $.Deferred().resolve().promise(); };
                    if (_this.fascicleContainerEnabled && (_this.selectedFascicleType == FascicleType.Period
                        || _this.selectedFascicleType == FascicleType.Procedure)) {
                        initializeContainersAction = function () { return _this.initializeContainers(); };
                    }
                    initializeContainersAction()
                        .done(function () {
                        $find(_this.ajaxManagerId).ajaxRequest("Initialize");
                    })
                        .fail(function (exception) { return promise.reject(exception); });
                    return promise.promise();
                };
                deferredInitializeAction()
                    .always(function () {
                    _this.bindLoaded();
                    _this._loadingPanel.hide(_this.fasciclePageContentId);
                });
            })
                .fail(function (exception) {
                _this._loadingPanel.hide(_this.fasciclePageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        /**
         *------------------------- Methods -----------------------------
         */
        uscFascicleInsert.prototype.checkAutorizations = function () {
            var promise = $.Deferred();
            if (!this.selectedFascicleType) {
                return promise.resolve(true);
            }
            this.service.hasInsertRight(this.selectedFascicleType, function (data) { return promise.resolve(data); }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        uscFascicleInsert.prototype.initializeContainers = function () {
            var _this = this;
            var promise = $.Deferred();
            var selectedIdCategory = null;
            var uscClassificatore = $("#".concat(this.uscClassificatoreId)).data();
            if (!jQuery.isEmptyObject(uscClassificatore)) {
                var selectedCategory = uscClassificatore.getSelectedCategory();
                if (selectedCategory) {
                    selectedIdCategory = selectedCategory.EntityShortId;
                }
            }
            this._containerService.getFascicleInsertAuthorizedContainers(selectedIdCategory, this.selectedFascicleType, function (data) {
                var containers = data;
                var lastSelectedContainer = _this.selectedContainer;
                if (_this._ddlContainer.get_selectedItem()) {
                    _this._ddlContainer.get_selectedItem().set_selected(false);
                }
                _this._ddlContainer.get_items().clear();
                if (containers.length != 1) {
                    var emptyItem = new Telerik.Web.UI.DropDownListItem();
                    _this._ddlContainer.get_items().add(emptyItem);
                }
                var containerItem;
                for (var _i = 0, containers_1 = containers; _i < containers_1.length; _i++) {
                    var container = containers_1[_i];
                    containerItem = new Telerik.Web.UI.DropDownListItem();
                    containerItem.set_text(container.Name);
                    containerItem.set_value(container.EntityShortId.toString());
                    _this._ddlContainer.get_items().add(containerItem);
                    if (containers.length == 1) {
                        containerItem.set_selected(true);
                    }
                    ;
                }
                if (lastSelectedContainer) {
                    var itemToSelect = _this._ddlContainer.findItemByValue(lastSelectedContainer.toString());
                    if (itemToSelect) {
                        itemToSelect.set_selected(true);
                    }
                }
                promise.resolve();
            }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        /**
         * Callback da code-behind per l'inizializzazione
         * @param isInitialized
         */
        uscFascicleInsert.prototype.initializeCallback = function () {
            if (this.selectedFascicleType) {
                var uscClassificatore = $("#".concat(this.uscClassificatoreId)).data();
                if (!jQuery.isEmptyObject(uscClassificatore)) {
                    uscClassificatore.setFascicleTypeParam(this.selectedFascicleType);
                }
            }
            this._deferredInitializeActions.forEach(function (item) { return item.resolve(); });
        };
        /**
     * Metodo di validazione della pagina
     */
        uscFascicleInsert.prototype.isPageValid = function () {
            var uscOggetto = $("#".concat(this.uscOggettoId)).data();
            if (!jQuery.isEmptyObject(uscOggetto)) {
                var txtOggetto = uscOggetto.getText();
                if (!uscOggetto.isValid()) {
                    this.showNotificationMessage(this.uscNotificationId, "Impossibile salvare.\nIl campo Oggetto ha superato i caratteri disponibili.<br />(Caratteri ".concat(txtOggetto.length.toString(), " Disponibili ", uscOggetto.getMaxLength().toString(), ")"));
                    return false;
                }
            }
            var fascicleTypeSelected = Number(this._rdlFascicleType.get_selectedItem().get_value());
            if (fascicleTypeSelected == FascicleType.Procedure) {
                var txtConservation = $find(this.txtConservationId);
                if (txtConservation.get_value() == null || isNaN(Number(txtConservation.get_value()))) {
                    this.showNotificationMessage(this.uscNotificationId, "Impossibile salvare. Il campo Conservazione Anni è mancante o non valido.");
                    return false;
                }
            }
            return Page_IsValid;
        };
        uscFascicleInsert.prototype.setPageBehaviourAfterFascicleTypeChanged = function () {
            var _this = this;
            this._loadingPanel.show(this.fasciclePageContentId);
            this.checkAutorizations()
                .done(function (result) {
                if (!result) {
                    var fascicleTypeDescription = _this._enumHelper.getFascicleTypeDescription(_this.selectedFascicleType);
                    _this.initializeEmptyFascicleTypeSelected();
                    _this.showNotificationMessage(_this.uscNotificationId, "Errore in inizializzazione pagina.<br > Utente non autorizzato all'inserimento di un nuovo fascicolo di tipo " + fascicleTypeDescription);
                    _this._loadingPanel.hide(_this.fasciclePageContentId);
                    return;
                }
                _this.initializePageByFascicleType(_this.selectedFascicleType);
                var deferredAction = function () {
                    var promise = $.Deferred();
                    _this._deferredFascicleSelectedTypeActions.push(promise);
                    var initializeContainersAction = function () { return $.Deferred().resolve().promise(); };
                    if (_this.fascicleContainerEnabled && (_this.selectedFascicleType == FascicleType.Period
                        || _this.selectedFascicleType == FascicleType.Procedure)) {
                        initializeContainersAction = function () { return _this.initializeContainers(); };
                    }
                    initializeContainersAction()
                        .done(function () {
                        setTimeout(function () {
                            $find(_this.ajaxManagerId).ajaxRequest("FascicleTypeSelected");
                        }, 500);
                    })
                        .fail(function (exception) { return promise.reject(exception); });
                    _this._loadingPanel.hide(_this.fasciclePageContentId);
                    return promise.promise();
                };
                deferredAction()
                    .always(function () {
                    $("#".concat(_this.fasciclePageContentId)).triggerHandler(uscFascicleInsert.FASCICLE_TYPE_CHANGED_EVENT);
                    _this._loadingPanel.hide(_this.fasciclePageContentId);
                });
            })
                .fail(function (exception) {
                _this._loadingPanel.hide(_this.fasciclePageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        uscFascicleInsert.prototype.initializePageByFascicleType = function (fascicleType) {
            switch (fascicleType) {
                case FascicleType.Procedure:
                    {
                        this.initializeFascicleProcedure();
                    }
                    break;
                case FascicleType.Activity:
                    {
                        this.initializeFascicleActivity();
                    }
                    break;
                case FascicleType.Period:
                    {
                        this.initializeFasciclePeriodic();
                    }
                    break;
            }
        };
        uscFascicleInsert.prototype.fascicleTypeSelectedCallback = function () {
            this._deferredFascicleSelectedTypeActions.forEach(function (item) { return item.resolve(); });
        };
        uscFascicleInsert.prototype.printCategoryNotFascicolable = function () {
            this.showWarningMessage(this.uscNotificationId, "Attenzione! Il classificatore selezionato non ha nessuna tipologia di fascicolazione associata");
        };
        uscFascicleInsert.prototype.initializeFascicleProcedure = function () {
            if (this.fascicleContainerEnabled) {
                this.containerRow.show();
                ValidatorEnable($get(this.rfvContainerId), true);
                $("#" + this.rfvContainerId).show();
            }
            else {
                this.containerRow.hide();
                ValidatorEnable($get(this.rfvContainerId), false);
                $("#" + this.rfvContainerId).hide();
            }
            this.fascicleDataRow.show();
            this.contattiRespRow.show();
            this.isMasterRow.show();
            this.startDateRow.hide();
            if (!String.isNullOrEmpty(this.pnlConservationId)) {
                $("#".concat(this.pnlConservationId)).show();
            }
        };
        uscFascicleInsert.prototype.initializeFascicleActivity = function () {
            ValidatorEnable($get(this.rfvContainerId), false);
            $("#" + this.rfvContainerId).hide();
            this.containerRow.hide();
            this.fascicleDataRow.show();
            this.contattiRespRow.hide();
            this.isMasterRow.hide();
            this.startDateRow.hide();
            if (!String.isNullOrEmpty(this.pnlConservationId)) {
                $("#".concat(this.pnlConservationId)).hide();
            }
        };
        uscFascicleInsert.prototype.initializeFasciclePeriodic = function () {
            if (this.fascicleContainerEnabled) {
                this.containerRow.show();
                ValidatorEnable($get(this.rfvContainerId), true);
                $("#" + this.rfvContainerId).hide();
            }
            else {
                this.containerRow.hide();
                ValidatorEnable($get(this.rfvContainerId), false);
                $("#" + this.rfvContainerId).hide();
                this.isMasterRow.show();
            }
            this.fascicleDataRow.show();
            this.contattiRespRow.hide();
            this.fascicleTypologyRow.hide();
        };
        uscFascicleInsert.prototype.initializeEmptyFascicleTypeSelected = function () {
            ValidatorEnable($get(this.rfvContainerId), false);
            $("#" + this.rfvContainerId).hide();
            this.fascicleDataRow.hide();
        };
        /**
    * Scateno l'evento di "Load Completed" del controllo
    */
        uscFascicleInsert.prototype.bindLoaded = function () {
            $("#".concat(this.fasciclePageContentId)).data(this);
            $("#".concat(this.fasciclePageContentId)).triggerHandler(uscFascicleInsert.LOADED_EVENT);
        };
        uscFascicleInsert.prototype.setProcedureTypeSelected = function () {
            var selectedItem = this._rdlFascicleType.findItemByValue(FascicleType.Procedure.toString());
            selectedItem.set_selected(true);
            this._rdlFascicleType.set_enabled(false);
            $("#".concat(this.fascicleDataRowId)).show();
        };
        uscFascicleInsert.prototype.setMetadataRepositorySelectedIndexEvent = function () {
            var _this = this;
            $("#".concat(this.uscMetadataRepositorySelId)).off(UscMetadataRepositorySel.SELECTED_INDEX_EVENT);
            $("#".concat(this.uscMetadataRepositorySelId)).on(UscMetadataRepositorySel.SELECTED_INDEX_EVENT, function (args, data) {
                var uscDynamicMetadata = $("#".concat(_this.uscDynamicMetadataId)).data();
                if (!jQuery.isEmptyObject(uscDynamicMetadata)) {
                    setTimeout(function () {
                        uscDynamicMetadata.loadDynamicMetadata(data);
                    }, 500);
                }
            });
        };
        uscFascicleInsert.prototype.setCategoryRole = function (idRole) {
            var uscClassificatore = $("#".concat(this.uscClassificatoreId)).data();
            if (!jQuery.isEmptyObject(uscClassificatore)) {
                uscClassificatore.setShowRoleParam(idRole);
            }
        };
        uscFascicleInsert.LOADED_EVENT = "onLoaded";
        uscFascicleInsert.FASCICLE_TYPE_CHANGED_EVENT = "onFascicleTypeChanged";
        return uscFascicleInsert;
    }(FascicleBase));
    return uscFascicleInsert;
});
//# sourceMappingURL=uscFascicleInsert.js.map