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
define(["require", "exports", "App/Services/Protocols/ProtocolService", "App/Services/Resolutions/ResolutionService", "App/Services/UDS/UDSRepositoryService", "App/Services/UDS/UDSService", "App/Services/Fascicles/FascicleFolderService", "App/Services/Fascicles/FascicleDocumentUnitService", "App/Services/Securities/DomainUserService", "App/Models/Fascicles/FascicleDocumentUnitModel", "App/Models/Fascicles/ReferenceModel", "App/Models/Fascicles/FascicleReferenceType", "Fasc/FascBase", "App/Models/Environment", "App/Helpers/ServiceConfigurationHelper", "App/Models/FascicolableActionType", "../app/core/extensions/string"], function (require, exports, ProtocolService, ResolutionService, UDSRepositoryService, UDSService, FascicleFolderService, FascicleDocumentUnitService, DomainUserService, FascicleDocumentUnitModel, ReferenceModel, FascicleReferenceType, FascicleBase, Environment, ServiceConfigurationHelper, FascicolableActionType) {
    var FascUDManager = /** @class */ (function (_super) {
        __extends(FascUDManager, _super);
        /**
         * Costruttore
         * @param serviceConfiguration
         */
        function FascUDManager(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME)) || this;
            /**
             *------------------------- Events -----------------------------
             */
            /**
             * Evento scatenato al click del pulsante "Nuovo fascicolo"
             * @param sender
             * @param args
             */
            _this.btnNewFascicle_OnClick = function (sender, args) {
                args.set_cancel(true);
                _this._loadingPanel.show(_this.pageContentId);
                _this._btnInsert.set_enabled(false);
                _this._btnRemove.set_enabled(false);
                if (_this._btnNewFascicle.get_visible()) {
                    _this._btnNewFascicle.set_enabled(false);
                }
                if (_this._btnCategoryChange.get_visible()) {
                    _this._btnCategoryChange.set_enabled(false);
                }
                var params = "Titolo=Nuovo Fascicolo&Type=Fasc";
                switch (_this.documentUnitType) {
                    case Environment.Protocol:
                        var protocol = _this._currentUD;
                        params = params.concat("&IdCategory=", protocol.Category.EntityShortId.toString(), "&IdDocumentUnit=", protocol.UniqueId, "&Environment=", Environment.Protocol.toString());
                        break;
                    case Environment.Resolution:
                        var resolution = _this._currentUD;
                        params = params.concat("&IdCategory=", resolution.Category.EntityShortId.toString(), "&IdDocumentUnit=", resolution.UniqueId, "&Environment=", Environment.Resolution.toString());
                        break;
                    case Environment.UDS:
                        var UDS = _this._currentUD;
                        params = params.concat("&IdCategory=", UDS.Category.EntityShortId.toString(), "&IdDocumentUnit=", UDS.UniqueId, "&Environment=", Environment.UDS.toString(), "&IdUDSRepository=", _this.currentIdUDSRepository);
                        break;
                }
                var locationPath = "../Fasc/FascInserimento.aspx";
                if (_this.processEnabled) {
                    locationPath = "../Fasc/FascProcessInserimento.aspx";
                }
                window.location.href = locationPath + "?" + params;
            };
            /**
             * Evento scatenato al click del pulsante "Rimuovi"
             * @param sender
             * @param args
             */
            _this.btnRemoveFascicle_OnClick = function (sender, args) {
                args.set_cancel(true);
                var dataItems = _this._rgvAssociatedFascicles.get_selectedItems();
                if (dataItems.length == 0) {
                    _this.showNotificationMessage(_this.uscNotificationId, "Nessun Fascicolo selezionato");
                    return;
                }
                _this._selectedAssociatedFascicle = dataItems[0].get_dataItem();
                if (_this._selectedAssociatedFascicle.FascicleType == FascicleReferenceType.Fascicle && !_this.validationModel.CanManageFascicle) {
                    _this.showNotificationMessage(_this.uscNotificationId, "Non si hanno i diritti di rimozione del Fascicolo selezionato");
                    return;
                }
                _this._manager.radconfirm("Sei sicuro di voler eliminare il documento dal fascicolo selezionato?", function (arg) {
                    if (arg) {
                        _this.removeFascicleDocumentUnit(_this._selectedAssociatedFascicle);
                    }
                }, 300, 160);
            };
            /**
             * Evento scatenato al click del pulsante "Fascicola"
             * @param sender
             * @param args
             */
            _this.btnInsert_OnClick = function (sender, args) {
                args.set_cancel(true);
                var selectedFascicle;
                var uscFascicleSearch = $("#" + _this.uscFascicleSearchId).data();
                if (!jQuery.isEmptyObject(uscFascicleSearch)) {
                    selectedFascicle = uscFascicleSearch.getSelectedFascicle();
                }
                if (!selectedFascicle) {
                    _this.showNotificationMessage(_this.uscNotificationId, "Nessun Fascicolo selezionato");
                    return;
                }
                var selectedFascicleFolder = _this._uscFascicleSearch.getSelectedFascicleFolder();
                if (_this.folderSelectionEnabled && !selectedFascicleFolder) {
                    _this.showNotificationMessage(_this.uscNotificationId, "Nessuna cartella fascicolo selezionata");
                    return;
                }
                _this.insertFascicleDocumentUnit(selectedFascicle, selectedFascicleFolder);
            };
            /**
             * Evento scatenato al click del pulsante "Cambio classificazione"
             * @param sender
             * @param args
             */
            _this.btnCategoryChange_OnClick = function (sender, args) {
                args.set_cancel(true);
                _this._manager.add_close(_this.closeChangeCategoryWindow);
                var protocol = _this._currentUD;
                var url = "../UserControl/CommonSelCategoryRest.aspx?Type=Fasc&FascicleBehavioursEnabled=true&FascicleType=1&IncludeParentDescendants=True&ParentId=" + protocol.Category.EntityShortId;
                _this._manager.open(url, "windowInsertProtocollo", null);
                _this._manager.center();
            };
            /**
            * Evento di chiusura finestra di Cambio Classificazione
            * @param sender
             * @param args
            */
            _this.closeChangeCategoryWindow = function (sender, args) {
                if (args.get_argument()) {
                    _this._loadingPanel.show(_this.pageContentId);
                    _this._btnInsert.set_enabled(false);
                    _this._btnRemove.set_enabled(false);
                    if (_this._btnNewFascicle.get_visible()) {
                        _this._btnNewFascicle.set_enabled(false);
                    }
                    if (_this._btnCategoryChange.get_visible()) {
                        _this._btnCategoryChange.set_enabled(false);
                    }
                    var category = args.get_argument();
                    var ajaxModel = {};
                    ajaxModel.ActionName = "ChangeCategory";
                    ajaxModel.Value = new Array();
                    ajaxModel.Value.push(category.IdCategory.toString());
                    _this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
                }
            };
            /**
            *------------------------- Methods -----------------------------
            */
            /**
             * Metodo che aggiorna la tabella dei fascicoli associati
             * @param data
             */
            _this.refreshAssociatedFascicles = function (data) {
                var models = new Array();
                var cmbOpenCloseItems;
                var isAlreadyFascicolate = false;
                if (data.length > 0) {
                    try {
                        $.each(data, function (index, fascicle) {
                            var model;
                            var imageUrl = "";
                            var referenceImageUrl = "";
                            var refTooltip = "";
                            var openCloseTooltip = "";
                            if (fascicle.EndDate == null) {
                                imageUrl = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
                                openCloseTooltip = "Fascicolo aperto";
                            }
                            else {
                                imageUrl = "../App_Themes/DocSuite2008/imgset16/folder_closed.png";
                                openCloseTooltip = "Fascicolo chiuso";
                            }
                            var referenceType = FascicleReferenceType.Fascicle;
                            var fascicleUDId;
                            var refModel;
                            refModel = _this.getDocumentUnitUDReferenceModel(fascicle, _this.documentUnitType);
                            if (refModel != null) {
                                referenceType = refModel.ReferenceType;
                                fascicleUDId = refModel.UniqueId;
                            }
                            referenceImageUrl = (referenceType == FascicleReferenceType.Fascicle) ? "../App_Themes/DocSuite2008/imgset16/folder_document.png" : "../App_Themes/DocSuite2008/imgset16/link.png";
                            refTooltip = (referenceType == FascicleReferenceType.Fascicle) ? "Fascicolato" : "Per riferimento";
                            var tileText = fascicle.Title.concat(" ", fascicle.FascicleObject);
                            model = {
                                Name: tileText,
                                UniqueId: fascicle.UniqueId,
                                ImageUrl: imageUrl,
                                OpenCloseTooltip: openCloseTooltip,
                                ReferenceImageUrl: referenceImageUrl,
                                ReferenceTooltip: refTooltip,
                                FascicleType: referenceType,
                                UDUniqueId: fascicleUDId
                            };
                            models.push(model);
                            if (!isAlreadyFascicolate) {
                                isAlreadyFascicolate = (referenceType == FascicleReferenceType.Fascicle);
                            }
                        });
                        _this._btnInsert.set_enabled(_this.validationModel.CanManageFascicle);
                        _this._btnNewFascicle.set_visible(!isAlreadyFascicolate);
                        if (_this._btnNewFascicle.get_visible()) {
                            _this._btnNewFascicle.set_enabled(_this.validationModel.CanInsertFascicle);
                        }
                        _this._btnRemove.set_enabled(_this.validationModel.CanManageFascicle);
                        _this._btnCategoryChange.set_visible(_this.validationModel.CanChangeCategory);
                        if (_this._btnCategoryChange.get_visible()) {
                            _this._btnCategoryChange.set_enabled(!isAlreadyFascicolate);
                        }
                    }
                    catch (error) {
                        _this.showNotificationMessage(_this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
                        console.log(JSON.stringify(error));
                        return;
                    }
                }
                else {
                    _this.setButtonEnable();
                }
                $("#".concat(_this.availableFascicleRowId)).show();
                if (!isAlreadyFascicolate && !_this.validationModel.CanManageFascicle) {
                    $("#".concat(_this.availableFascicleRowId)).hide();
                }
                var tableView = _this._rgvAssociatedFascicles.get_masterTableView();
                tableView.clearSelectedItems();
                tableView.set_dataSource(models);
                tableView.dataBind();
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        /**
         * Inizializzazione
         */
        FascUDManager.prototype.initialize = function () {
            _super.prototype.initialize.call(this);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._rgvAssociatedFascicles = $find(this.rgvAssociatedFasciclesId);
            this._btnInsert = $find(this.btnInsertId);
            this._btnInsert.add_clicking(this.btnInsert_OnClick);
            this._btnNewFascicle = $find(this.btnNewFascicleId);
            this._btnNewFascicle.add_clicking(this.btnNewFascicle_OnClick);
            this._btnRemove = $find(this.btnRemoveId);
            this._btnRemove.add_clicking(this.btnRemoveFascicle_OnClick);
            this._btnCategoryChange = $find(this.btnCategoryChangeId);
            this._btnCategoryChange.add_clicking(this.btnCategoryChange_OnClick);
            this._manager = $find(this.radWindowManagerId);
            this._lblUDSelected = $("#".concat(this.lblUDSelectedId));
            this._lblUDTitle = $("#".concat(this.lblUDTitleId));
            this._lblDocumentUnitType = $("#".concat(this.lblDocumentUnitTypeId));
            this._lblContainer = $("#".concat(this.lblContainerId));
            this._lblObject = $("#".concat(this.lblObjectId));
            this._lblCategory = $("#".concat(this.lblCategoryId));
            this._ajaxManager = $find(this.ajaxManagerId);
            this._uscFascicleSearch = $("#" + this.uscFascicleSearchId).data();
            try {
                var fascicleDocumentUnitConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLE_DOCUMENTUNIT_TYPE_NAME);
                this._fascicleDocumentUnitService = new FascicleDocumentUnitService(fascicleDocumentUnitConfiguration);
                var domainUserConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.DOMAIN_TYPE_NAME);
                this._domainUserService = new DomainUserService(domainUserConfiguration);
                var fascicleFolderServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLEFOLDER_TYPE_NAME);
                this._fascicleFolderService = new FascicleFolderService(fascicleFolderServiceConfiguration);
                this._btnCategoryChange.set_visible(false);
                this._btnInsert.set_enabled(false);
                this._btnNewFascicle.set_enabled(false);
                this._btnRemove.set_enabled(false);
                switch (this.documentUnitType) {
                    case Environment.Protocol:
                        var protocolConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.PROTOCOL_TYPE_NAME);
                        this._udservice = new ProtocolService(protocolConfiguration);
                        this.loadProtocol();
                        break;
                    case Environment.Resolution:
                        var resolutionConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.RESOLUTION_TYPE_NAME);
                        this._udservice = new ResolutionService(resolutionConfiguration);
                        this.loadResolution();
                        break;
                    case Environment.DocumentSeries:
                        break;
                    case Environment.UDS:
                        var udsConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, this.documentUnitRepositoryName);
                        var udsRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.UDSREPOSITORY_TYPE_NAME);
                        this._udsRepositoryService = new UDSRepositoryService(udsRepositoryConfiguration);
                        this._udservice = new UDSService(udsConfiguration);
                        this.loadUDSRepository();
                        break;
                    default:
                        break;
                }
            }
            catch (error) {
                this.showNotificationMessage(this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
                console.log(JSON.stringify(error));
            }
        };
        FascUDManager.prototype.getDocumentUnitUDReferenceModel = function (fascicle, dswEnvironment) {
            var _this = this;
            var protocol = this._currentUD;
            var model = new ReferenceModel();
            $.each(fascicle.FascicleDocumentUnits, function (index, fascicleDocumentUnit) {
                if (((dswEnvironment == Environment.UDS && fascicleDocumentUnit.DocumentUnit.Environment >= 100) || (dswEnvironment != Environment.UDS && fascicleDocumentUnit.DocumentUnit.Environment == dswEnvironment))
                    && fascicleDocumentUnit.DocumentUnit.UniqueId == protocol.UniqueId) {
                    if ($.type(fascicleDocumentUnit.ReferenceType) === "string") {
                        fascicleDocumentUnit.ReferenceType = FascicleReferenceType[fascicleDocumentUnit.ReferenceType.toString()];
                    }
                    if (fascicleDocumentUnit.ReferenceType == FascicleReferenceType.Fascicle) {
                        _this._btnInsert.set_enabled(false);
                        _this._btnNewFascicle.set_visible(false);
                        _this._btnRemove.set_enabled(_this.validationModel.CanManageFascicle);
                        if (_this._btnCategoryChange.get_visible()) {
                            _this._btnCategoryChange.set_enabled(false);
                        }
                        var uscFascicleSearch_1 = $("#" + _this.uscFascicleSearchId).data();
                        if (!jQuery.isEmptyObject(uscFascicleSearch_1)) {
                            uscFascicleSearch_1.loadFascicle(fascicle.UniqueId);
                        }
                    }
                    model.ReferenceType = fascicleDocumentUnit.ReferenceType;
                    model.UniqueId = fascicleDocumentUnit.UniqueId;
                }
            });
            return model;
        };
        /**
         * Carica da UD di tipo protocollo
         */
        FascUDManager.prototype.loadProtocol = function () {
            var _this = this;
            this._loadingPanel.show(this.pageContentId);
            this._udservice.getProtocolByUniqueId(this.documentUnitUniqueId, function (data) {
                if (data == null) {
                    $("#".concat(_this.pageContentId)).hide();
                    _this._btnRemove.set_enabled(false);
                    _this._btnNewFascicle.set_enabled(false);
                    _this._loadingPanel.hide(_this.pageContentId);
                    return;
                }
                if (!_this.validationModel.CanManageFascicle && !_this.validationModel.CanChangeCategory) {
                    _this.showNotificationMessage(_this.uscNotificationId, "Mancano i diritti di gestione dei fascicoli relativi al documento");
                    $("#".concat(_this.pageContentId)).hide();
                    _this._btnRemove.set_enabled(false);
                    _this._btnNewFascicle.set_enabled(false);
                    _this._loadingPanel.hide(_this.pageContentId);
                    return;
                }
                var model = data;
                _this._lblUDSelected.html("Protocollo selezionato");
                _this._lblDocumentUnitType.html("Protocollo:");
                _this._lblUDTitle.html(model.Year.toString().concat("/", String("0000000" + model.Number.toString()).slice(-7), " del ", moment(model.RegistrationDate).format("DD/MM/YYYY")));
                _this._lblObject.html(model.Object);
                _this._lblCategory.html(model.Category.Name);
                _this._lblContainer.html(model.Container.Name);
                _this._currentUD = model;
                _this.loadUDdata(model, function () {
                    _this._loadingPanel.hide(_this.pageContentId);
                });
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        /**
         * Carica da UD di tipo Resolution
         */
        FascUDManager.prototype.loadResolution = function () {
            var _this = this;
            this._loadingPanel.show(this.pageContentId);
            this._udservice.getResolutionByUniqueId(this.documentUnitUniqueId, function (data) {
                if (data == null) {
                    $("#".concat(_this.pageContentId)).hide();
                    _this._btnRemove.set_enabled(false);
                    _this._btnNewFascicle.set_enabled(false);
                    _this._loadingPanel.hide(_this.pageContentId);
                    return;
                }
                if (!_this.validationModel.CanManageFascicle) {
                    _this.showNotificationMessage(_this.uscNotificationId, "Mancano i diritti di gestione dei fascicoli relativi al documento");
                    $("#".concat(_this.pageContentId)).hide();
                    _this._btnRemove.set_enabled(false);
                    _this._btnNewFascicle.set_enabled(false);
                    _this._loadingPanel.hide(_this.pageContentId);
                    return;
                }
                var model = data;
                _this._lblUDSelected.html("Atto selezionato");
                _this._lblDocumentUnitType.html("Atto:");
                _this._lblUDTitle.html(model.InclusiveNumber);
                _this._lblObject.html(model.Object);
                _this._lblCategory.html(model.Category.Name);
                $("#".concat(_this.rowContainerId)).hide();
                _this._currentUD = model;
                _this.loadUDdata(model, function () {
                    _this._loadingPanel.hide(_this.pageContentId);
                });
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        /**
     * Carica da UD di tipo UDS
     */
        FascUDManager.prototype.loadUDSRepository = function () {
            var _this = this;
            this._loadingPanel.show(this.pageContentId);
            this._udservice.getUDSByUniqueId(this.documentUnitUniqueId, function (data) {
                if (data == null) {
                    $("#".concat(_this.pageContentId)).hide();
                    _this._btnRemove.set_enabled(false);
                    _this._btnNewFascicle.set_enabled(false);
                    _this._loadingPanel.hide(_this.pageContentId);
                    return;
                }
                if (!_this.validationModel.CanManageFascicle) {
                    _this.showNotificationMessage(_this.uscNotificationId, "Mancano i diritti di gestione dei fascicoli relativi al documento");
                    $("#".concat(_this.pageContentId)).hide();
                    _this._btnRemove.set_enabled(false);
                    _this._btnNewFascicle.set_enabled(false);
                    _this._loadingPanel.hide(_this.pageContentId);
                    return;
                }
                var model = data;
                _this._lblUDSelected.html("Archivio selezionato");
                _this._lblDocumentUnitType.html(_this.documentUnitRepositoryName.concat(":"));
                _this._lblUDTitle.html(model.Year.toString().concat("/", String("0000000" + model.Number.toString()).slice(-7), " del ", moment(model.RegistrationDate).format("DD/MM/YYYY")));
                _this._lblObject.html(model.Subject);
                _this._lblCategory.html(model.Category.Name);
                $("#".concat(_this.rowContainerId)).hide();
                _this._currentUD = model;
                _this.loadUDdata(model, function () {
                    _this._loadingPanel.hide(_this.pageContentId);
                });
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        /**
     * Rimuove un riferimento per document unit
     * @param model
     */
        FascUDManager.prototype.removeFascicleDocumentUnit = function (model) {
            var fascicleDocumentUnit = new FascicleDocumentUnitModel(model.UniqueId);
            fascicleDocumentUnit.ReferenceType = model.FascicleType;
            fascicleDocumentUnit.UniqueId = model.UDUniqueId;
            var documentUnit = this._currentUD;
            fascicleDocumentUnit.DocumentUnit = documentUnit;
            this.removeFascicleUD(fascicleDocumentUnit, documentUnit, this._fascicleDocumentUnitService);
        };
        /**
         * Rimuove un riferimento per l'UD  corrente
         * @param model
         * @param reference
         * @param service
         */
        FascUDManager.prototype.removeFascicleUD = function (model, reference, service) {
            var _this = this;
            this._loadingPanel.show(this.pageContentId);
            this._btnInsert.set_enabled(false);
            this._btnRemove.set_enabled(false);
            if (this._btnNewFascicle.get_visible()) {
                this._btnNewFascicle.set_enabled(false);
            }
            if (this._btnCategoryChange.get_visible()) {
                this._btnCategoryChange.set_enabled(false);
            }
            service.deleteFascicleUD(model, function (data) {
                _this.loadUDdata(reference, function () {
                    if (_this._btnNewFascicle.get_visible()) {
                        _this._btnNewFascicle.set_enabled(_this.validationModel.CanInsertFascicle);
                    }
                    _this._loadingPanel.hide(_this.pageContentId);
                });
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.setButtonEnable();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        /**
         * Inserisce un riferimento per la documentunit
         * @param selectedFascicle
         **/
        FascUDManager.prototype.insertFascicleDocumentUnit = function (selectedFascicle, selectedFascicleFolder) {
            var model = new FascicleDocumentUnitModel(selectedFascicle.UniqueId);
            model.ReferenceType = FascicleReferenceType.Fascicle;
            var documentUnit = this._currentUD;
            model.DocumentUnit = documentUnit;
            model.FascicleFolder = {};
            model.FascicleFolder.UniqueId = selectedFascicleFolder.UniqueId;
            this.insertFascicleUD(model, documentUnit, this._fascicleDocumentUnitService);
        };
        /**
         * Inserisce un riferimento per la UD selezionata
         * @param model
         * @param reference
         * @param service
         */
        FascUDManager.prototype.insertFascicleUD = function (model, reference, service) {
            var _this = this;
            this._loadingPanel.show(this.pageContentId);
            this._btnInsert.set_enabled(false);
            this._btnRemove.set_enabled(false);
            this._btnInsert.removeCssClass("rbHovered");
            if (this._btnNewFascicle.get_visible()) {
                this._btnNewFascicle.set_enabled(false);
            }
            if (this._btnCategoryChange.get_visible()) {
                this._btnCategoryChange.set_enabled(false);
            }
            service.insertFascicleUD(model, FascicolableActionType.AutomaticDetection, function (data) {
                _this.loadUDdata(reference, function () {
                    if (_this._btnCategoryChange.get_visible()) {
                        _this._btnCategoryChange.set_enabled(false);
                    }
                    _this._loadingPanel.hide(_this.pageContentId);
                });
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.setButtonEnable();
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        /**
         * Inizializza i dati della pagina
         * TODO: da togliere a favore di Signalr
         */
        FascUDManager.prototype.loadUDdata = function (model, done) {
            var _this = this;
            this.service.getAssociatedFascicles(model.UniqueId, this.documentUnitType, null, function (data) {
                var uscFascicleSearch = $("#" + _this.uscFascicleSearchId).data();
                if (!jQuery.isEmptyObject(uscFascicleSearch)) {
                    uscFascicleSearch.clearSelections();
                }
                _this.refreshAssociatedFascicles(data);
                if (done) {
                    done();
                }
            }, function (exception) {
                _this.showNotificationMessage(_this.uscNotificationId, 'Errore nella richiesta. Non è stato possibile recuperare i Fascicoli associati.');
                _this.setButtonEnable();
                if (done) {
                    done();
                }
            });
        };
        /**
        * Metodo di callback di cambio classficazione
        * @param categoryName
        * @param validationModel
        */
        FascUDManager.prototype.changeCategoryCallback = function (categoryName, validationModel) {
            this.validationModel = JSON.parse(validationModel);
            this.loadProtocol();
            this._loadingPanel.hide(this.pageContentId);
        };
        /**
        * Metodo di callback di errore
        * @param message
        */
        FascUDManager.prototype.errorCallback = function (message) {
            if (message) {
                this._loadingPanel.hide(this.pageContentId);
                this.showNotificationMessage(this.uscNotificationId, message);
            }
        };
        /**
        * Metodo abilitare/disabilitare pulsanti in base ai diritti
        */
        FascUDManager.prototype.setButtonEnable = function () {
            this._btnInsert.set_enabled(this.validationModel.CanManageFascicle);
            this._btnRemove.set_enabled(this.validationModel.CanManageFascicle);
            if (this._btnNewFascicle.get_visible()) {
                this._btnNewFascicle.set_enabled(this.validationModel.CanInsertFascicle);
            }
            else {
                this._btnNewFascicle.set_visible(this.validationModel.CanInsertFascicle);
            }
            this._btnCategoryChange.set_visible(this.validationModel.CanChangeCategory);
            if (this._btnCategoryChange.get_visible()) {
                this._btnCategoryChange.set_enabled(true);
            }
        };
        return FascUDManager;
    }(FascicleBase));
    return FascUDManager;
});
//# sourceMappingURL=FascUDManager.js.map