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
define(["require", "exports", "App/Services/DocumentUnits/DocumentUnitService", "App/Services/Fascicles/FascicleDocumentUnitService", "App/Models/Environment", "Fasc/FascBase", "App/Helpers/ServiceConfigurationHelper", "App/Services/Commons/ContainerService", "App/Models/FascicolableActionType", "App/Models/Commons/LocationTypeEnum", "App/DTOs/ValidationExceptionDTO", "App/DTOs/ValidationMessageDTO", "App/Models/Fascicles/FascicleDocumentUnitModel"], function (require, exports, DocumentUnitService, FascicleDocumentUnitService, Environment, FascicleBase, ServiceConfigurationHelper, ContainerService, FascicolableActionType, LocationTypeEnum, ValidationExceptionDTO, ValidationMessageDTO, FascicleDocumentUnitModel) {
    var uscFascInsertUD = /** @class */ (function (_super) {
        __extends(uscFascInsertUD, _super);
        /**
         * Costruttore
         * @param serviceConfiguration
         */
        function uscFascInsertUD(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME)) || this;
            /**
             *------------------------- Events -----------------------------
             */
            /**
             * Evento scatenato alla selezione/deselezione di un record della griglia
             * @param sender
             * @param args
             */
            _this.grdUdDocSelected_OnRowSelectChanged = function (sender, args) {
                _this.enableButtons(_this._grdUdDocSelected.get_selectedItems().length > 0);
            };
            /**
             * Evento scatenato al click del pulsante Inserisci
             * @param sender
             * @param args
             */
            _this.btnReference_OnClick = function (sender, args) {
                args.set_cancel(true);
                if (!Page_IsValid) {
                    return;
                }
                _this.enableButtons(false);
                _this.addFascicleUD();
            };
            /**
             * Evento scatenato al click del pulsante di cerca UD
             * @param sender
             * @param args
             */
            _this.btnSearch_OnClick = function (sender, args) {
                args.set_cancel(true);
                if (!Page_IsValid) {
                    return;
                }
                if (_this._grdUdDocSelected.get_selectedItems().length > 0) {
                    _this._grdUdDocSelected.clearSelectedItems();
                }
                _this.enableButtons(false);
                $("#panelContent").hide();
                _this.findDocumentUnits(0);
            };
            /**
            * Evento al cambio di classificatore
            */
            _this.onCategoryChanged = function (idCategory) {
                if (idCategory != 0) {
                    $("#".concat(_this.chbCategoryChildId)).show();
                    $("label[for=".concat(_this.chbCategoryChildId, "]")).show();
                }
                if (idCategory == 0) {
                    $("#".concat(_this.chbCategoryChildId)).hide();
                    $("label[for=".concat(_this.chbCategoryChildId, "]")).hide();
                }
            };
            /**
           * Evento scatenato dalla RadComboBox per inizializzare i dati da visualizzare
           * @param sender
           * @param args
           */
            _this.rcbContainers_OnClientItemsRequested = function (sender, args) {
                var numberOfItems = sender.get_items().get_count();
                if (numberOfItems > 0) {
                    //Decremento di 1 perchè la combo visualizza anche un item vuoto
                    numberOfItems -= 1;
                }
                var currentOtherContainerItems = numberOfItems;
                var currentComboFilter = sender.get_attributes().getAttribute('currentFilter');
                var otherContainerCount = Number(sender.get_attributes().getAttribute('otherContainerCount'));
                var updating = sender.get_attributes().getAttribute('updating') == 'true';
                if (isNaN(otherContainerCount) || currentComboFilter != args.get_text()) {
                    //Se il valore del filtro è variato re-inizializzo la radcombobox per chiamare le WebAPI
                    otherContainerCount = undefined;
                }
                sender.get_attributes().setAttribute('currentFilter', args.get_text());
                _this.setMoreResultBoxText('Caricamento...');
                if ((otherContainerCount == undefined || currentOtherContainerItems < otherContainerCount) && !updating) {
                    sender.get_attributes().setAttribute('updating', 'true');
                    var location_1 = null;
                    if (_this._rcbUDDoc.get_selectedItem() != undefined && _this._rcbUDDoc.get_selectedItem().get_value() != "") {
                        var env = _this._rcbUDDoc.get_selectedItem().get_value();
                        if (env == "Protocollo") {
                            location_1 = LocationTypeEnum.ProtLocation;
                        }
                        if (env == "Serie documentale") {
                            location_1 = LocationTypeEnum.DocumentSeriesLocation;
                        }
                        if (env == "Archivio") {
                            location_1 = LocationTypeEnum.UDSLocation;
                        }
                        if (location_1 == null) {
                            location_1 = LocationTypeEnum.ReslLocation;
                        }
                    }
                    _this._containerService.getContainersByEnvironment(args.get_text(), _this.maxNumberElements, currentOtherContainerItems, location_1, function (data) {
                        try {
                            _this.refreshContainers(data.value);
                            var scrollToPosition = args.get_domEvent() == undefined;
                            if (scrollToPosition) {
                                if (sender.get_items().get_count() > 0) {
                                    var scrollContainer = $(sender.get_dropDownElement()).find('div.rcbScroll');
                                    scrollContainer.scrollTop($(sender.get_items().getItem(currentOtherContainerItems + 1).get_element()).position().top);
                                }
                            }
                            sender.get_attributes().setAttribute('otherContainerCount', data.count.toString());
                            sender.get_attributes().setAttribute('updating', 'false');
                            if (sender.get_items().get_count() > 0) {
                                currentOtherContainerItems = sender.get_items().get_count() - 1;
                            }
                            _this.setMoreResultBoxText('Visualizzati '.concat(currentOtherContainerItems.toString(), ' di ', data.count.toString()));
                        }
                        catch (error) {
                            _this.showNotificationMessage(_this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
                            console.log(JSON.stringify(error));
                        }
                    }, function (exception) {
                        _this.showNotificationException(_this.uscNotificationId, exception);
                    });
                }
            };
            /**
         * Metodo per popolare la RadComboBox di selezione fascicoli
         * @param data
         */
            _this.refreshContainers = function (data) {
                if (data.length > 0) {
                    _this._rcbContainers.beginUpdate();
                    if (_this._rcbContainers.get_items().get_count() == 0) {
                        var emptyItem = new Telerik.Web.UI.RadComboBoxItem();
                        emptyItem.set_text("");
                        emptyItem.set_value("");
                        _this._rcbContainers.get_items().insert(0, emptyItem);
                    }
                    $.each(data, function (index, container) {
                        var item = new Telerik.Web.UI.RadComboBoxItem();
                        item.set_text(container.Name);
                        item.set_value(container.EntityShortId.toString());
                        _this._rcbContainers.get_items().add(item);
                    });
                    _this._rcbContainers.showDropDown();
                    _this._rcbContainers.endUpdate();
                }
                else {
                    if (_this._rcbContainers.get_items().get_count() == 0) {
                    }
                }
            };
            /**
         * Evento scatenato allo scrolling della RadComboBox di selezione fascicoli
         * @param args
         */
            _this.rcbOtherFascicles_onScroll = function (args) {
                var element = args.target;
                if ((element.scrollHeight - element.scrollTop === element.clientHeight) && element.clientHeight > 0) {
                    var filter = _this._rcbContainers.get_text();
                    _this.rcbContainers_OnClientItemsRequested(_this._rcbContainers, new Telerik.Web.UI.RadComboBoxRequestEventArgs(filter, args));
                }
            };
            _this.rcbContainers_OnSelectedIndexChanged = function (sender, args) {
                var selectedItem = sender.get_selectedItem();
                var domEvent = args.get_domEvent();
                if (domEvent.type == 'mousedown') {
                    return;
                }
                if (domEvent.type == 'click' || (domEvent.type == 'keydown' && (domEvent.keyCode == 9 || domEvent.keyCode == 13))) {
                    var emptyItem = sender.findItemByText('');
                    sender.clearItems();
                    sender.get_items().add(emptyItem);
                    sender.get_items().add(selectedItem);
                    sender.get_attributes().setAttribute('currentFilter', selectedItem.get_text());
                    sender.get_attributes().setAttribute('otherContainerCount', '1');
                    _this.setMoreResultBoxText("Visualizzati 1 di 1");
                }
            };
            _this.rcbUDDoc_OnSelectedIndexChanged = function (sender, args) {
                var domEvent = args.get_domEvent();
                if (domEvent.type == 'mousedown') {
                    return;
                }
                if (domEvent.type == 'click' || (domEvent.type == 'keydown' && (domEvent.keyCode == 9 || domEvent.keyCode == 13))) {
                    _this._rcbContainers.clearItems();
                }
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        uscFascInsertUD.prototype.DocumentUnitRedirectUrls = function () {
            var items = [
                [Environment.Protocol, function (d) { return "../Prot/ProtVisualizza.aspx?UniqueId=" + d.UniqueId + "&Type=Prot"; }],
                [Environment.Resolution, function (d) { return "../Resl/ReslVisualizza.aspx?IdResolution=" + d.EntityId.toString() + "&Type=Resl"; }],
                [Environment.DocumentSeries, function (d) { return "../Series/Item.aspx?IdDocumentSeriesItem=" + d.EntityId.toString() + "&Action=View&Type=Series"; }],
                [Environment.UDS, function (d) { return "../UDS/UDSView.aspx?Type=UDS&IdUDS=" + d.UniqueId.toString() + "&IdUDSRepository=" + d.UDSRepository.UniqueId.toString(); }]
            ];
            return items;
        };
        uscFascInsertUD.prototype.DocumentUnitInsertActions = function () {
            var _this = this;
            var items = [
                [Environment.Protocol, function (d) { return _this.insertFascicleDocumentUnit(d); }],
                [Environment.Resolution, function (d) { return _this.insertFascicleDocumentUnit(d); }],
                [Environment.UDS, function (d) { return _this.insertFascicleDocumentUnit(d); }]
            ];
            return items;
        };
        /**
         * Inizializzazione
         */
        uscFascInsertUD.prototype.initialize = function () {
            var _this = this;
            _super.prototype.initialize.call(this);
            this._panelFilter = $("#".concat(this.panelFilterId));
            this._lblUDObject = $("#".concat(this.lblUDObjectId));
            this._lblCategory = $("#".concat(this.lblCategoryId));
            this._lblUDType = $("#".concat(this.lblUDTypeId));
            this._chbCategoryChild = ($("#".concat(this.chbCategoryChildId))[0]);
            this._btnUDLink = $find(this.btnUDLinkId);
            ;
            this._btnReference = $find(this.btnReferenceId);
            this._btnReference.add_clicking(this.btnReference_OnClick);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._btnReference.set_enabled(false);
            this._btnSearch = $find(this.btnSearchId);
            this._btnSearch.add_clicking(this.btnSearch_OnClick);
            this._txtNumber = $find(this.txtNumberId);
            this._txtYear = $find(this.txtYearId);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._radWindowManager = $find(this.radWindowManagerId);
            this._grdUdDocSelected = $find(this.grdUdDocSelectedId);
            this._txtSubject = $find(this.txtSubjectId);
            this._treeCategory = $find(this.treeCategoryId);
            this._grdUdDocSelected.add_rowSelected(this.grdUdDocSelected_OnRowSelectChanged);
            this._grdUdDocSelected.add_rowDeselected(this.grdUdDocSelected_OnRowSelectChanged);
            this._rcbUDDoc = $find(this.rcbUDDocId);
            this._rcbUDDoc.add_selectedIndexChanged(this.rcbUDDoc_OnSelectedIndexChanged);
            this._rcbContainers = $find(this.rcbContainersId);
            this._rcbContainers.add_selectedIndexChanged(this.rcbContainers_OnSelectedIndexChanged);
            this._rcbContainers.add_itemsRequested(this.rcbContainers_OnClientItemsRequested);
            var containerConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Container");
            this._containerService = new ContainerService(containerConfiguration);
            var scrollContainer = $(this._rcbContainers.get_dropDownElement()).find('div.rcbScroll');
            $(scrollContainer).scroll(this.rcbOtherFascicles_onScroll);
            $("#".concat(this.rowResolutionTypeId)).hide();
            var documentUnitConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.DOCUMENT_UNIT_TYPE_NAME);
            this._documentUnitService = new DocumentUnitService(documentUnitConfiguration);
            var fascicleDocumentUnitService = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLE_DOCUMENTUNIT_TYPE_NAME);
            this._fascicleDocumentUnitService = new FascicleDocumentUnitService(fascicleDocumentUnitService);
            if (this.currentFascicleId == "") {
                this._panelFilter.hide();
            }
            this._loadingPanel.show(this.pageContentId);
            $("#panelContent").hide();
            this.service.getFascicle(this.currentFascicleId, function (data) {
                if (data == null)
                    return;
                _this._fascicleModel = data;
                $("#".concat(_this.labelTitoloId)).text("Fascicolo: ".concat(" (", _this._fascicleModel.Title, ")"));
                _this.enableButtons(false);
                _this._panelFilter.show();
                _this._txtYear.set_value(_this._fascicleModel.Year.toString());
                _this._txtNumber.focus();
                _this._loadingPanel.hide(_this.pageContentId);
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        uscFascInsertUD.prototype.onPageChanged = function () {
            var masterTable = this._grdUdDocSelected.get_masterTableView();
            var skip = masterTable.get_currentPageIndex() * masterTable.get_pageSize();
            this.findDocumentUnits(skip);
        };
        /**
         *------------------------- Methods -----------------------------
         */
        /**
         * Metodo che esegue l'inserimento della UD selezionata
         * @param model
         */
        uscFascInsertUD.prototype.addFascicleUD = function () {
            var _this = this;
            var selectedDocumentUnits = this.getSelectedDocumentUnits();
            if (selectedDocumentUnits.length == 0) {
                this.showWarningMessage(this.uscNotificationId, "Selezionare un documento per l'inserimento.");
                return;
            }
            var deferredActions = [];
            var errorMessages = [];
            var _loop_1 = function (documentUnit) {
                var deferredInsertAction = function () {
                    var promise = $.Deferred();
                    _this.insertFascicleDocumentUnit(documentUnit)
                        .fail(function (exception) {
                        if (exception) {
                            if (exception instanceof ValidationExceptionDTO) {
                                for (var _i = 0, _a = exception.validationMessages; _i < _a.length; _i++) {
                                    var validationMessage = _a[_i];
                                    errorMessages.push("Il documento " + documentUnit.Title + " ha restituito il seguente errore: " + validationMessage.message);
                                }
                            }
                            else {
                                errorMessages.push("Il documento " + documentUnit.Title + " ha restituito il seguente errore: " + exception.statusText);
                            }
                        }
                    })
                        .always(function () { return promise.resolve(); });
                    return promise.promise();
                };
                deferredActions.push(deferredInsertAction());
            };
            for (var _i = 0, selectedDocumentUnits_1 = selectedDocumentUnits; _i < selectedDocumentUnits_1.length; _i++) {
                var documentUnit = selectedDocumentUnits_1[_i];
                _loop_1(documentUnit);
            }
            this._loadingPanel.show(this.pageContentId);
            $.when.apply(null, deferredActions)
                .then(function () {
                if (errorMessages && errorMessages.length > 0) {
                    _this.enableButtons(true);
                    var validationException = new ValidationExceptionDTO();
                    validationException.statusText = "Non tutti i documenti selezionati sono stati inseriti correttamente. Per maggiori informazioni verificare i messaggi di errore ottenuti ed eventualmente conttattare l'assistenza.";
                    validationException.validationMessages = [];
                    var validationMessage = void 0;
                    for (var _i = 0, errorMessages_1 = errorMessages; _i < errorMessages_1.length; _i++) {
                        var errorMessage = errorMessages_1[_i];
                        validationMessage = new ValidationMessageDTO();
                        validationMessage.message = errorMessage;
                        validationException.validationMessages.push(validationMessage);
                    }
                    _this.showNotificationException(_this.uscNotificationId, validationException);
                }
                else {
                    _this.closeWindow();
                }
            })
                .always(function () { return _this._loadingPanel.hide(_this.pageContentId); });
        };
        uscFascInsertUD.prototype.getSelectedDocumentUnits = function () {
            var masterTable = this._grdUdDocSelected.get_masterTableView();
            var selectedItems = masterTable.get_selectedItems();
            var selectedIds = selectedItems.map(function (item) { return item.get_dataItem().UniqueId; });
            return this._documentUnitsFound.filter(function (item) { return selectedIds.some(function (e) { return e == item.UniqueId; }); });
        };
        /**
         * Recupera una RadWindow dalla pagina
         */
        uscFascInsertUD.prototype.getRadWindow = function () {
            var wnd = null;
            if (window.radWindow)
                wnd = window.radWindow;
            else if (window.frameElement.radWindow)
                wnd = window.frameElement.radWindow;
            return wnd;
        };
        /**
         * Chiude la RadWindow
         */
        uscFascInsertUD.prototype.closeWindow = function () {
            var wnd = this.getRadWindow();
            wnd.close(true);
        };
        /**
         * Apre una nuova nuova RadWindow
         * @param url
         * @param name
         * @param width
         * @param height
         */
        uscFascInsertUD.prototype.openWindow = function (url, width, height) {
            var wnd = this._radWindowManager.open(url, null, null);
            wnd.set_modal(true);
            wnd.setSize(width, height);
            wnd.center();
            return false;
        };
        /**
         * Recupera l'url per la visualizzazione della UD
         * @param documentUnit
         */
        uscFascInsertUD.prototype.getDocumentUnitUrl = function (documentUnit) {
            var env = this._documentUnitService.getDocumentUnitEnvironment(documentUnit);
            var foundItems = this.DocumentUnitRedirectUrls().filter(function (item) { return item[0] == env; })
                .map(function (item) { return item[1](documentUnit); });
            if (!foundItems || foundItems.length == 0) {
                return "#";
            }
            return foundItems[0];
        };
        uscFascInsertUD.prototype.getFinderModel = function () {
            var finderModel = {};
            finderModel.IdTenantAOO = this.currentTenantAOOId;
            if (this._txtYear.get_value() != "")
                finderModel.Year = Number(this._txtYear.get_value());
            if (this._txtNumber.get_value() != "")
                finderModel.Number = this._txtNumber.get_value().replace("/", "|");
            if (this._rcbUDDoc.get_selectedItem() != undefined && this._rcbUDDoc.get_selectedItem().get_value() != "") {
                finderModel.DocumentUnitName = this._rcbUDDoc.get_selectedItem().get_value();
            }
            if (this._txtSubject.get_value() != "") {
                finderModel.Subject = this._txtSubject.get_value();
            }
            if (this._rcbContainers.get_selectedItem() && this._rcbContainers.get_selectedItem().get_value()) {
                finderModel.IdContainer = Number(this._rcbContainers.get_selectedItem().get_value());
            }
            this._treeCategory = $find(this.treeCategoryId);
            if (this._treeCategory.get_allNodes().length > 0) {
                var lastNodeIndex = this._treeCategory.get_allNodes().length - 1;
                finderModel.IdCategory = this._treeCategory.get_allNodes()[lastNodeIndex].get_value();
            }
            finderModel.IncludeChildClassification = this._chbCategoryChild.checked;
            return finderModel;
        };
        /**
         * Esegue una ricerca delle documentunits corrispondenti all'anno/numero impostati
         * senza SecurityGroup attiva
         * @param protocolYear
         * @param protocolNumber
         */
        uscFascInsertUD.prototype.findDocumentUnits = function (skip) {
            var _this = this;
            this._loadingPanel.show(this.pageContentId);
            var finderModel = this.getFinderModel();
            var masterTableView = this._grdUdDocSelected.get_masterTableView();
            var top = skip + masterTableView.get_pageSize();
            finderModel.Skip = skip;
            finderModel.Top = top;
            finderModel.IdFascicle = this._fascicleModel.UniqueId;
            this._documentUnitService.findDocumentUnits(finderModel, function (data) {
                if (data == null) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this.showWarningMessage(_this.uscNotificationId, "Nessun elemento trovato con i parametri passati");
                    return;
                }
                _this.bindDocumentUnitsGrid(data);
                _this._documentUnitService.countDocumentUnits(finderModel, function (data) {
                    if (data == null) {
                        _this._loadingPanel.hide(_this.pageContentId);
                        _this.showWarningMessage(_this.uscNotificationId, "Nessun elemento trovato con i parametri passati");
                        return;
                    }
                    masterTableView.set_virtualItemCount(data);
                    masterTableView.dataBind();
                    _this._loadingPanel.hide(_this.pageContentId);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.pageContentId);
                    _this.enableButtons(false);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            }, function (exception) {
                _this._loadingPanel.hide(_this.pageContentId);
                _this.enableButtons(false);
                _this.showNotificationException(_this.uscNotificationId, exception);
            });
        };
        /**
         * Esegue il binding della griglia dei risultati di ricerca UD
         * @param documentUnits
         */
        uscFascInsertUD.prototype.bindDocumentUnitsGrid = function (documentUnits) {
            var _this = this;
            var models = new Array();
            $.each(documentUnits, function (index, documentUnit) {
                var model = {};
                if (documentUnit.Category) {
                    model.Category = documentUnit.Category.Name;
                }
                if (documentUnit.Container) {
                    model.Container = documentUnit.Container.Name;
                }
                model.UDLink = _this.getDocumentUnitUrl(documentUnit);
                model.Object = documentUnit.Subject;
                model.UDType = documentUnit.DocumentUnitName;
                model.UDTitle = documentUnit.Title;
                model.UniqueId = documentUnit.UniqueId;
                model.IdFascicle = documentUnit.IdFascicle;
                model.RegistrationDate = moment(documentUnit.RegistrationDate).format("DD/MM/YYYY");
                model.IsFascicolable = documentUnit.IsFascicolable;
                models.push(model);
            });
            this._documentUnitsFound = documentUnits;
            $("#panelContent").show();
            var masterTableView = this._grdUdDocSelected.get_masterTableView();
            masterTableView.set_dataSource(models);
            masterTableView.dataBind();
        };
        uscFascInsertUD.prototype.insertFascicleDocumentUnit = function (documentUnit) {
            var fascicleDocumentUnitModel = new FascicleDocumentUnitModel(this._fascicleModel.UniqueId);
            fascicleDocumentUnitModel.DocumentUnit = documentUnit;
            return this.insertFascicleUD(fascicleDocumentUnitModel, this._fascicleDocumentUnitService);
        };
        /**
         * Metodo generico che esegue la chiamata ajax per l'inserimento di una nuova UD nel fascicolo
         * @param model
         * @param service
         */
        uscFascInsertUD.prototype.insertFascicleUD = function (model, service) {
            var promise = $.Deferred();
            if (this.idFascicleFolder) {
                model.FascicleFolder = {};
                model.FascicleFolder.UniqueId = this.idFascicleFolder;
            }
            service.insertFascicleUD(model, FascicolableActionType.AutomaticDetection, function (data) {
                promise.resolve();
            }, function (exception) {
                promise.reject(exception);
            });
            return promise.promise();
        };
        /**
        * Metodo che imposta l'abilitazione dei pulsanti Fascicola e Inserisci
        * @param value
        */
        uscFascInsertUD.prototype.enableButtons = function (value) {
            this._btnReference.set_enabled(value);
        };
        /**
     * Metodo che setta la label MoreResultBoxText della RadComboBox
     * @param message
     */
        uscFascInsertUD.prototype.setMoreResultBoxText = function (message) {
            this._rcbContainers.get_moreResultsBoxMessageElement().innerText = message;
        };
        return uscFascInsertUD;
    }(FascicleBase));
    return uscFascInsertUD;
});
//# sourceMappingURL=uscFascInsertUD.js.map