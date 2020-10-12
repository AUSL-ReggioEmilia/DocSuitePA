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
define(["require", "exports", "App/Models/Fascicles/FascicleModel", "App/Models/Fascicles/FascicleRoleModel", "App/Models/Fascicles/FascicleType", "App/Models/Fascicles/VisibilityType", "Fasc/FascBase", "App/Helpers/ServiceConfigurationHelper", "UserControl/uscMetadataRepositorySel", "App/Helpers/EnumHelper", "App/Services/Commons/ContainerService", "App/Helpers/PageClassHelper", "App/Models/Commons/UscRoleRestEventType", "App/Models/Commons/AuthorizationRoleType"], function (require, exports, FascicleModel, FascicleRoleModel, FascicleType, VisibilityType, FascicleBase, ServiceConfigurationHelper, UscMetadataRepositorySel, EnumHelper, ContainerService, PageClassHelper, UscRoleRestEventType, AuthorizationRoleType) {
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
            _this.onCategoryChanged = function (conservationYear, idMetadataRepository, customActionsJson) {
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
                if (customActionsJson !== "") {
                    PageClassHelper.callUserControlFunctionSafe(_this.uscCustomActionsRestId)
                        .done(function (instance) {
                        instance.loadItems(JSON.parse(customActionsJson));
                    });
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
                PageClassHelper.callUserControlFunctionSafe(_this.uscRoleMasterId)
                    .done(function (instance) {
                    instance.forceBehaviourValidationState(_this.selectedFascicleType === FascicleType.Procedure);
                    instance.enableValidators(_this.selectedFascicleType === FascicleType.Procedure);
                });
                PageClassHelper.callUserControlFunctionSafe(_this.uscRoleId)
                    .done(function (instance) {
                    instance.forceBehaviourValidationState(_this.selectedFascicleType !== FascicleType.Procedure);
                    instance.enableValidators(_this.selectedFascicleType !== FascicleType.Procedure);
                    instance.disableRaciRoleButton();
                });
                PageClassHelper.callUserControlFunctionSafe(_this.uscCustomActionsRestId)
                    .done(function (instance) {
                    instance.loadItems({
                        AutoClose: false,
                        AutoCloseAndClone: false
                    });
                });
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
                fascicleModel.VisibilityType = _this._fascicleVisibilityType;
                fascicleModel.FascicleRoles = _this.populateFascicleRoles();
                return fascicleModel;
            };
            _this.getSelectedFascicleType = function () {
                return _this._rdlFascicleType.get_selectedItem().get_value();
            };
            _this.enableValidators = function (enabled) {
                ValidatorEnable($get(_this.rfvConservationId), enabled);
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
        uscFascicleInsert.prototype.fascicleDataRow = function () {
            return $("#" + this.fascicleDataRowId);
        };
        uscFascicleInsert.prototype.contattiRespRow = function () {
            return $("#" + this.contattiRespRowId);
        };
        uscFascicleInsert.prototype.isMasterRow = function () {
            return $("#" + this.isMasterRowId);
        };
        uscFascicleInsert.prototype.startDateRow = function () {
            return $("#" + this.rowStartDateId);
        };
        uscFascicleInsert.prototype.metadataRepositoryRow = function () {
            return $("#" + this.metadataRepositoryRowId);
        };
        uscFascicleInsert.prototype.fascicleTypologyRow = function () {
            return $("#" + this.fascicleTypologyRowId);
        };
        uscFascicleInsert.prototype.containerRow = function () {
            return $("#" + this.containerRowId);
        };
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
            this.metadataRepositoryRow().hide();
            if (this.selectedFascicleType && this.selectedFascicleType == FascicleType.Period) {
                this.initializeFasciclePeriodic();
            }
            if (this.activityFascicleEnabled && !this.selectedFascicleType) {
                this.initializeEmptyFascicleTypeSelected();
            }
            this._fascicleVisibilityType = VisibilityType.Confidential;
            this.registerUscRoleRestEventHandlers();
            PageClassHelper.callUserControlFunctionSafe(this.uscRoleId).done(function (instance) {
                instance.setToolbarVisibility(true);
                instance.renderRolesTree([]);
            });
            PageClassHelper.callUserControlFunctionSafe(this.uscRoleMasterId).done(function (instance) {
                instance.setToolbarVisibility(true);
                instance.renderRolesTree([]);
            });
            sessionStorage.removeItem(this.clientId + "_FascicleRolesToAdd");
            if (this.metadataRepositoryEnabled) {
                this.metadataRepositoryRow().show();
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
                        var ajaxModel = {
                            ActionName: "Initialize",
                            Value: []
                        };
                        $find(_this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
                    })
                        .fail(function (exception) { return promise.reject(exception); });
                    return promise.promise();
                };
                deferredInitializeAction()
                    .always(function () {
                    if (!_this.activityFascicleEnabled && (!_this.selectedFascicleType || _this.selectedFascicleType != FascicleType.Period)) {
                        _this.setProcedureTypeSelected();
                    }
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
                            var ajaxModel = {
                                ActionName: "FascicleTypeSelected",
                                Value: []
                            };
                            if (_this._selectedResponsibleRole) {
                                ajaxModel.Value.push(_this._selectedResponsibleRole.EntityShortId.toString());
                            }
                            $find(_this.ajaxManagerId).ajaxRequest(JSON.stringify(ajaxModel));
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
            var _this = this;
            switch (+this._rdlFascicleType.get_selectedItem().get_value()) {
                case FascicleType.Activity: {
                    PageClassHelper.callUserControlFunctionSafe(this.uscRoleId)
                        .done(function (instance) {
                        instance.requiredValidationEnabled = "true";
                        instance.multipleRoles = "false";
                        instance.onlyMyRoles = "true";
                        instance.setConfiguration({
                            isReadOnlyMode: false
                        });
                    });
                    break;
                }
                case FascicleType.Procedure: {
                    PageClassHelper.callUserControlFunctionSafe(this.uscRoleId)
                        .done(function (instance) {
                        instance.multipleRoles = "true";
                        instance.onlyMyRoles = "false";
                        instance.setConfiguration({
                            isReadOnlyMode: true
                        });
                    });
                    PageClassHelper.callUserControlFunctionSafe(this.uscRoleMasterId)
                        .done(function (instance) {
                        instance.multipleRoles = "false";
                        instance.onlyMyRoles = "true";
                    });
                    break;
                }
                case FascicleType.Period: {
                    PageClassHelper.callUserControlFunctionSafe(this.uscRoleId)
                        .done(function (instance) {
                        instance.multipleRoles = "true";
                        instance.onlyMyRoles = "false";
                    });
                    PageClassHelper.callUserControlFunctionSafe(this.uscRoleMasterId)
                        .done(function (instance) {
                        instance.requiredValidationEnabled = "" + !_this.fascicleContainerEnabled;
                        instance.multipleRoles = "false";
                        instance.onlyMyRoles = "true";
                    });
                    break;
                }
            }
            this._deferredFascicleSelectedTypeActions.forEach(function (item) { return item.resolve(); });
        };
        uscFascicleInsert.prototype.printCategoryNotFascicolable = function () {
            this.showWarningMessage(this.uscNotificationId, "Attenzione! Il classificatore selezionato non ha nessuna tipologia di fascicolazione associata");
        };
        uscFascicleInsert.prototype.initializeFascicleProcedure = function () {
            if (this.fascicleContainerEnabled) {
                this.containerRow().show();
                ValidatorEnable($get(this.rfvContainerId), true);
                $("#" + this.rfvContainerId).show();
            }
            else {
                this.containerRow().hide();
                ValidatorEnable($get(this.rfvContainerId), false);
                $("#" + this.rfvContainerId).hide();
            }
            this.fascicleDataRow().show();
            this.contattiRespRow().show();
            this.isMasterRow().show();
            this.startDateRow().hide();
            if (!String.isNullOrEmpty(this.pnlConservationId)) {
                $("#".concat(this.pnlConservationId)).show();
            }
        };
        uscFascicleInsert.prototype.initializeFascicleActivity = function () {
            ValidatorEnable($get(this.rfvContainerId), false);
            $("#" + this.rfvContainerId).hide();
            this.containerRow().hide();
            this.fascicleDataRow().show();
            this.contattiRespRow().hide();
            this.isMasterRow().hide();
            this.startDateRow().hide();
            if (!String.isNullOrEmpty(this.pnlConservationId)) {
                $("#".concat(this.pnlConservationId)).hide();
            }
        };
        uscFascicleInsert.prototype.initializeFasciclePeriodic = function () {
            if (this.fascicleContainerEnabled) {
                this.containerRow().show();
                ValidatorEnable($get(this.rfvContainerId), true);
                $("#" + this.rfvContainerId).hide();
            }
            else {
                this.containerRow().hide();
                ValidatorEnable($get(this.rfvContainerId), false);
                $("#" + this.rfvContainerId).hide();
                this.isMasterRow().show();
            }
            this.fascicleDataRow().show();
            this.contattiRespRow().hide();
            this.fascicleTypologyRow().hide();
        };
        uscFascicleInsert.prototype.initializeEmptyFascicleTypeSelected = function () {
            ValidatorEnable($get(this.rfvContainerId), false);
            $("#" + this.rfvContainerId).hide();
            this.fascicleDataRow().hide();
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
            $("#".concat(this.uscMetadataRepositorySelId)).off(UscMetadataRepositorySel.SELECTED_REPOSITORY_EVENT);
            $("#".concat(this.uscMetadataRepositorySelId)).on(UscMetadataRepositorySel.SELECTED_REPOSITORY_EVENT, function (args, data) {
                if (data) {
                    var uscDynamicMetadata_1 = $("#".concat(_this.uscDynamicMetadataId)).data();
                    if (!jQuery.isEmptyObject(uscDynamicMetadata_1)) {
                        setTimeout(function () {
                            uscDynamicMetadata_1.loadDynamicMetadata(data);
                        }, 500);
                    }
                }
            });
        };
        uscFascicleInsert.prototype.setCategoryRole = function (idRole) {
            var uscClassificatore = $("#".concat(this.uscClassificatoreId)).data();
            if (!jQuery.isEmptyObject(uscClassificatore)) {
                uscClassificatore.setShowRoleParam(idRole);
            }
        };
        uscFascicleInsert.prototype.getCustomActions = function () {
            var promise = $.Deferred();
            PageClassHelper.callUserControlFunctionSafe(this.uscCustomActionsRestId)
                .done(function (instance) {
                promise.resolve(instance.getCustomActions());
            });
            return promise.promise();
        };
        uscFascicleInsert.prototype.registerUscRoleRestEventHandlers = function () {
            var _this = this;
            PageClassHelper.callUserControlFunctionSafe(this.uscRoleId)
                .done(function (instance) {
                instance.registerEventHandler(UscRoleRestEventType.RoleDeleted, function (roleId) {
                    _this.deleteRoleFromModel(roleId);
                    return $.Deferred().resolve();
                });
                instance.registerEventHandler(UscRoleRestEventType.NewRolesAdded, function (newAddedRoles) {
                    var existedRole;
                    _this.addRoleToModel(_this.uscRoleMasterId, newAddedRoles, function (role) {
                        existedRole = role;
                    });
                    return $.Deferred().resolve(existedRole);
                });
            });
            PageClassHelper.callUserControlFunctionSafe(this.uscRoleMasterId)
                .done(function (instance) {
                instance.registerEventHandler(UscRoleRestEventType.RoleDeleted, function (roleId) {
                    _this.deleteRoleFromModel(roleId);
                    return $.Deferred().resolve();
                });
                instance.registerEventHandler(UscRoleRestEventType.NewRolesAdded, function (newAddedRoles) {
                    var existedRole;
                    _this.addRoleToModel(_this.uscRoleId, newAddedRoles, function (role) {
                        existedRole = role;
                    });
                    if (!existedRole) {
                        _this._selectedResponsibleRole = newAddedRoles[0];
                    }
                    return $.Deferred().resolve(existedRole, true);
                });
                instance.registerEventHandler(UscRoleRestEventType.SetFascicleVisibilityType, function (visibilityType) {
                    _this._fascicleVisibilityType = visibilityType;
                    return $.Deferred().resolve();
                });
            });
        };
        uscFascicleInsert.prototype.addRoleToModel = function (toCheckControlId, newAddedRoles, existedRoleCallback) {
            var _this = this;
            PageClassHelper.callUserControlFunctionSafe(toCheckControlId)
                .done(function (instance) {
                var existedRole = instance.existsRole(newAddedRoles);
                if (existedRole) {
                    alert("Non \u00E8 possibile selezionare il settore " + existedRole.Name + " in quanto gi\u00E0 presente come settore " + (toCheckControlId == _this.uscRoleMasterId ? "responsabile" : "autorizzato") + " del fascicolo");
                    existedRoleCallback(existedRole);
                    newAddedRoles = newAddedRoles.filter(function (x) { return x.IdRole !== existedRole.IdRole; });
                }
                if (toCheckControlId === _this.uscRoleMasterId) {
                    _this.setCategoryRole(newAddedRoles[0].EntityShortId);
                    return _this.addRole(newAddedRoles, false);
                }
            });
        };
        uscFascicleInsert.prototype.addRole = function (newAddedRoles, isMaster) {
            if (!newAddedRoles.length)
                return;
            var fascicleRoles = [];
            if (this.getFascicleRolesToAdd()) {
                fascicleRoles = this.getFascicleRolesToAdd();
            }
            for (var _i = 0, newAddedRoles_1 = newAddedRoles; _i < newAddedRoles_1.length; _i++) {
                var newAddedRole = newAddedRoles_1[_i];
                var fascicleRole = new FascicleRoleModel();
                fascicleRole.IsMaster = isMaster;
                fascicleRole.Role = newAddedRole;
                fascicleRoles.push(fascicleRole);
            }
            this.setFascicleRolesToSession(fascicleRoles);
        };
        uscFascicleInsert.prototype.getFascicleRolesToAdd = function () {
            var itemsFromSession = sessionStorage.getItem(this.clientId + "_FascicleRolesToAdd");
            if (itemsFromSession) {
                return JSON.parse(itemsFromSession);
            }
            return null;
        };
        uscFascicleInsert.prototype.setFascicleRolesToSession = function (fascicleRoles) {
            if (!fascicleRoles) {
                sessionStorage.removeItem(this.clientId + "_FascicleRolesToAdd");
            }
            sessionStorage[this.clientId + "_FascicleRolesToAdd"] = JSON.stringify(fascicleRoles);
        };
        uscFascicleInsert.prototype.deleteRoleFromModel = function (roleIdToDelete) {
            if (!roleIdToDelete)
                return;
            var fascicleRoles = [];
            if (this.getFascicleRolesToAdd()) {
                fascicleRoles = this.getFascicleRolesToAdd();
            }
            fascicleRoles = fascicleRoles.filter(function (x) { return x.Role.IdRole !== roleIdToDelete && x.Role.FullIncrementalPath.indexOf(roleIdToDelete.toString()) === -1; });
            this.setFascicleRolesToSession(fascicleRoles);
        };
        uscFascicleInsert.prototype.populateFascicleRoles = function () {
            var fascicleRoles = this.getFascicleRolesToAdd();
            if (this._selectedResponsibleRole) {
                if (!fascicleRoles) {
                    fascicleRoles = [];
                }
                fascicleRoles.push({
                    Role: this._selectedResponsibleRole,
                    IsMaster: true
                });
            }
            var uscRole = $("#" + this.uscRoleId).data();
            var raciRoles = uscRole.getRaciRoles();
            var _loop_1 = function (fascicleRole) {
                fascicleRole.AuthorizationRoleType = (raciRoles && raciRoles.some(function (x) { return x.EntityShortId === fascicleRole.Role.EntityShortId; })) || fascicleRole.IsMaster
                    ? AuthorizationRoleType.Responsible
                    : AuthorizationRoleType.Accounted;
            };
            for (var _i = 0, fascicleRoles_1 = fascicleRoles; _i < fascicleRoles_1.length; _i++) {
                var fascicleRole = fascicleRoles_1[_i];
                _loop_1(fascicleRole);
            }
            return fascicleRoles;
        };
        uscFascicleInsert.LOADED_EVENT = "onLoaded";
        uscFascicleInsert.FASCICLE_TYPE_CHANGED_EVENT = "onFascicleTypeChanged";
        return uscFascicleInsert;
    }(FascicleBase));
    return uscFascicleInsert;
});
//# sourceMappingURL=uscFascicleInsert.js.map