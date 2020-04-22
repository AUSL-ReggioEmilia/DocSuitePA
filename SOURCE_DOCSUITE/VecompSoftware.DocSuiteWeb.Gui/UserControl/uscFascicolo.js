/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
define(["require", "exports", "App/Models/Environment", "App/Services/Fascicles/FascicleService", "App/Models/Fascicles/FascicleType", "App/Services/Securities/DomainUserService", "App/Helpers/ServiceConfigurationHelper", "App/DTOs/ExceptionDTO", "App/Models/Workflows/WorkflowPropertyHelper", "App/Services/Workflows/WorkflowActivityService", "App/Mappers/Fascicles/FiltersGridUDFasciclesViewModelMapper", "UserControl/uscFascicleFolders"], function (require, exports, Environment, FascicleService, FascicleType, DomainUserService, ServiceConfigurationHelper, ExceptionDTO, WorkflowPropertyHelper, WorkflowActivityService, FiltersGridUDFasciclesViewModelMapper, UscFascicleFolders) {
    var uscFascicolo = /** @class */ (function () {
        /**
         * Costruttore
         * @param webApiConfiguration
         */
        function uscFascicolo(serviceConfigurations) {
            var _this = this;
            this._expandedFoldersPanel = true;
            this._notFireEvent = false;
            /**
             *------------------------- Events -----------------------------
             */
            /**
             * Evento al click del pulsante per la visualizzazione dell'UD
             * @param sender
             * @param args
             */
            this.btnUDLink_OnClick = function (sender, args) {
                args.set_cancel(true);
                _this._loadingPanel.show(_this.grdUDId);
                var uniqueId = $(sender.get_element()).attr("UniqueId");
                var incremental = Number($(sender.get_element()).attr("EntityId"));
                var environment = Number($(sender.get_element()).attr("Environment"));
                var year = Number($(sender.get_element()).attr("Year"));
                var number = Number($(sender.get_element()).attr("Number"));
                var url = "";
                switch (environment) {
                    case Environment.Protocol:
                        url = "../Prot/ProtVisualizza.aspx?FromFascicle=true&Year=".concat(year.toString(), "&Number=", number.toString(), "&IdFascicle=", _this.currentFascicleId, "&Type=Prot");
                        break;
                    case Environment.Resolution:
                        url = "../Resl/ReslVisualizza.aspx?FromFascicle=true&IdResolution=".concat(incremental.toString(), "&Type=Resl", "&IdFascicle=", _this.currentFascicleId);
                        break;
                    case Environment.DocumentSeries:
                        url = "../Series/Item.aspx?FromFascicle=true&Type=Series&IdDocumentSeriesItem=".concat(incremental.toString(), "&Action=View&Type=Series", "&IdFascicle=", _this.currentFascicleId);
                        break;
                    case Environment.UDS:
                        url = "../UDS/UDSView.aspx?Type=UDS&FromFascicle=true&IdUDS=".concat(uniqueId.toString(), "&IdUDSRepository=", $(sender.get_element()).attr("UDSRepositoryId"), "&IdFascicle=", _this.currentFascicleId);
                        break;
                    default:
                        var serializedDoc = $(sender.get_element()).attr("SerializedDoc");
                        _this.openPreviewWindow(serializedDoc);
                        _this._loadingPanel.hide(_this.grdUDId);
                        return;
                }
                window.location.href = url;
            };
            /**
             * Evento al click del pulsante per la visualizzazione del fascicolo dove l'UD è fascicolata
             */
            this.imgUDFascicle_OnClick = function (sender, args) {
                args.set_cancel(true);
                _this._loadingPanel.show(_this.grdUDId);
                var uniqueId = $(sender.get_element()).attr("IdFascicle");
                var url = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(uniqueId.toString());
                window.location.href = url;
            };
            /**
            * Evento al click del pulsante per la espandere o comprimere la gliglia dei fascicoli collegati
            */
            this.btnExpandLinkedFascicles_OnClick = function (sender, args) {
                args.set_cancel(true);
                if (_this._isLinkedFasciclesGridOpen) {
                    _this._isLinkedFasciclesGridOpen = false;
                }
                else {
                    _this._isLinkedFasciclesGridOpen = true;
                }
            };
            /**
            * Evento al click del pulsante per la espandere o comprimere la gliglia delle UD presenti nel fascicolo
            */
            this.btnExpandUDFascicle_OnClick = function (sender, args) {
                args.set_cancel(true);
                if (_this._isUDFascicleGridOpen) {
                    _this._UDFascicleGrid.hide();
                    _this._isUDFascicleGridOpen = false;
                    _this._btnExpandUDFascicle.removeCssClass("dsw-arrow-down");
                    _this._btnExpandUDFascicle.addCssClass("dsw-arrow-up");
                }
                else {
                    _this._UDFascicleGrid.show();
                    _this._isUDFascicleGridOpen = true;
                    _this._btnExpandUDFascicle.removeCssClass("dsw-arrow-up");
                    _this._btnExpandUDFascicle.addCssClass("dsw-arrow-down");
                }
            };
            /**
            * Evento al click del pulsante per espandere o collassare il pannello dei metadati dinamici
            */
            this.btnExpandDynamicMetadata_OnClick = function (sender, args) {
                args.set_cancel(true);
                if (_this._isDynamicMetadataContentOpen) {
                    _this._dynamicMetadataContent.hide();
                    _this._isDynamicMetadataContentOpen = false;
                    _this._btnExpandDynamicMetadata.removeCssClass("dsw-arrow-down");
                    _this._btnExpandDynamicMetadata.addCssClass("dsw-arrow-up");
                }
                else {
                    _this._dynamicMetadataContent.show();
                    _this._isDynamicMetadataContentOpen = true;
                    _this._btnExpandDynamicMetadata.removeCssClass("dsw-arrow-up");
                    _this._btnExpandDynamicMetadata.addCssClass("dsw-arrow-down");
                }
            };
            /**
             * Evento scatenato dai comandi della griglia
             * @param sender
             * @param args
             */
            this.gridOnCommand = function (sender, args) {
                args.set_cancel(true);
                if (args.get_commandName() == "Filter") {
                    _this._loadingPanel.show(_this.grdUDId);
                    $("#".concat(_this.pageId)).triggerHandler(uscFascicolo.REBIND_EVENT);
                }
                if (args.get_commandName() == "Sort") {
                    _this._loadingPanel.show(_this.grdUDId);
                    $("#".concat(_this.pageId)).triggerHandler(uscFascicolo.REBIND_EVENT);
                }
            };
            /**
             * Evento scatenato dal cambio di selezione del tipo di UD da visualizzare in griglia
             * @param sender
             * @param args
             */
            this.rcbUd_OnSelectedIndexChanged = function (sender, args) {
                if (!_this._notFireEvent) {
                    _this._grid = $find(_this.grdUDId);
                    if (!(_this.fasciclesPanelVisibilities["GridSearchPanelVisibility"])) {
                        if (sender.get_selectedItem().get_value() == null || sender.get_selectedItem().get_value() == "") {
                            _this.removeSpecificFilter("DocumentUnitName");
                        }
                        else {
                            _this._grid.get_masterTableView().filter("DocumentUnitName", sender.get_selectedItem().get_value(), "EqualTo", true);
                        }
                    }
                    else {
                        _this._grid.get_masterTableView().filter("DocumentUnitName", _this._rcbUd.get_selectedItem().get_value(), "EqualTo", true);
                    }
                }
            };
            /**
             * Evento scatenato dal cambio di selezione del tipo di fascicolazione delle UD
             * @param sender
             * @param args
             */
            this.rcbReferenceType_OnSelectedIndexChanged = function (sender, args) {
                _this._grid = $find(_this.grdUDId);
                if (!(_this.fasciclesPanelVisibilities["GridSearchPanelVisibility"])) {
                    if (sender.get_selectedItem().get_value() == null || sender.get_selectedItem().get_value() == "") {
                        _this.removeSpecificFilter("ReferenceType");
                    }
                    else {
                        _this._grid.get_masterTableView().filter("ReferenceType", sender.get_selectedItem().get_value(), "EqualTo", true);
                    }
                }
                else {
                    _this._grid.get_masterTableView().filter("ReferenceType", _this._rcbReferenceType.get_selectedItem().get_value(), "EqualTo", true);
                }
            };
            /**
             * Evento scatenato dall'input di valori o azioni nella TextBox del filtro
             * @param sender
             * @param args
             */
            this.txtTitle_OnKeyPressed = function (sender, args) {
                if (args.get_keyCode() == 13) {
                    args.set_cancel(true);
                    setTimeout(function () {
                        _this._grid = $find(_this.grdUDId);
                        _this._grid.get_masterTableView().filter("Title", _this._txtTitleUD.get_element().value, "Contains", true);
                    }, 0);
                }
            };
            /**
         * Evento scatenato dall'input di valori o azioni nella TextBox del filtro
         * @param sender
         * @param args
         */
            this.udSubject_OnKeyPressed = function (sender, args) {
                if (args.get_keyCode() == 13) {
                    args.set_cancel(true);
                    setTimeout(function () {
                        _this._grid = $find(_this.grdUDId);
                        if ((sender.get_element()).value == null || (sender.get_element()).value == "") {
                            _this.removeSpecificFilter("Title");
                        }
                        else {
                            _this._grid.get_masterTableView().filter("Title", (sender.get_element()).value, "Contains", true);
                        }
                    }, 0);
                }
            };
            /**
             * Metodo per caricare i fascicoli collegati in griglia
             * @param models
             */
            //TODO: la griglia dei fascicoli collegati dovrà essere uno usercontrol
            this.refreshLinkedFascicles = function (data) {
                var models = new Array();
                if (data == null)
                    return;
                if (data.FascicleLinks.length > 0) {
                    $.each(data.FascicleLinks, function (index, fascicleLink) {
                        var model;
                        var imageUrl = "";
                        var openCloseTooltip = "";
                        var fascicleTypeImageUrl = "";
                        var fascicleTypeTooltip = "";
                        if (fascicleLink.FascicleLinked.EndDate == null) {
                            imageUrl = "../App_Themes/DocSuite2008/imgset16/folder_open.png";
                            openCloseTooltip = "Aperto";
                        }
                        else {
                            imageUrl = "../App_Themes/DocSuite2008/imgset16/folder_closed.png";
                            openCloseTooltip = "Chiuso";
                        }
                        switch (FascicleType[fascicleLink.FascicleLinked.FascicleType.toString()]) {
                            case FascicleType.Period:
                                fascicleTypeImageUrl = "../App_Themes/DocSuite2008/imgset16/history.png";
                                fascicleTypeTooltip = "Periodico";
                                break;
                            case FascicleType.Legacy:
                                fascicleTypeImageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_legacy.png";
                                fascicleTypeTooltip = "Fascicolo non a norma";
                                break;
                            case FascicleType.Procedure:
                                fascicleTypeImageUrl = "../App_Themes/DocSuite2008/imgset16/fascicle_procedure.png";
                                fascicleTypeTooltip = "Per procedimento";
                                break;
                            case FascicleType.SubFascicle:
                                fascicleTypeImageUrl = "";
                                fascicleTypeTooltip = "Sotto fascicolo";
                                break;
                        }
                        var tileText = fascicleLink.FascicleLinked.Title.concat(" ", fascicleLink.FascicleLinked.FascicleObject);
                        model = {
                            Name: tileText, FascicleLinkUniqueId: fascicleLink.UniqueId, UniqueId: fascicleLink.FascicleLinked.UniqueId, Category: fascicleLink.FascicleLinked.Category.Name,
                            ImageUrl: imageUrl, OpenCloseTooltip: openCloseTooltip, FascicleTypeImageUrl: fascicleTypeImageUrl, FascicleTypeToolTip: fascicleTypeTooltip
                        };
                        models.push(model);
                    });
                }
            };
            this.removeSpecificFilter = function (name) {
                var filterComm = null;
                _this._grid.get_masterTableView().get_filterExpressions().forEach(function (item) {
                    if (item.ColumnUniqueName === name) {
                        filterComm = item;
                        return;
                    }
                });
                if (filterComm) {
                    _this._grid.get_masterTableView().get_filterExpressions().remove(filterComm);
                }
                _this._grid.get_masterTableView().fireCommand("Filter", "GridFilterCommandEventArgs");
            };
            this._service = new FascicleService(ServiceConfigurationHelper.getService(serviceConfigurations, "Fascicle"));
            this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
        }
        /**
         * Inizializzazione
         */
        uscFascicolo.prototype.initialize = function () {
            var _this = this;
            this._UDFascicleGrid = $("#".concat(this.UDFascicleGridId));
            this._dynamicMetadataContent = $("#".concat(this.dynamicMetadataContentId));
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._txtTitleUD = $find(this.txtTitleId);
            this._txtTitleUD.add_keyPress(this.txtTitle_OnKeyPressed);
            this._ajaxManager = $find(this.ajaxManagerId);
            this._rcbUd = $find(this.rcbUDId);
            this._rcbReferenceType = $find(this.rcbReferenceTypeId);
            this._rcbReferenceType.add_selectedIndexChanged(this.rcbReferenceType_OnSelectedIndexChanged);
            this._rcbUd.add_selectedIndexChanged(this.rcbUd_OnSelectedIndexChanged);
            this._grid = $find(this.grdUDId);
            this._grid.get_masterTableView().hideFilterItem();
            this._btnExpandUDFascicle = $find(this.btnExpandUDFascicleId);
            this._btnExpandDynamicMetadata = $find(this.btnExpandDynamicMetadataId);
            this._racUDDataSource = $find(this.racUDDataSourceId);
            this._btnExpandUDFascicle.addCssClass("dsw-arrow-down");
            this._UDFascicleGrid.show();
            this._isLinkedFasciclesGridOpen = false;
            this._isUDFascicleGridOpen = true;
            this._btnExpandDynamicMetadata.add_clicking(this.btnExpandDynamicMetadata_OnClick);
            $("#".concat(this.rowDynamicMetadataId)).hide();
            this._btnExpandDynamicMetadata.addCssClass("dsw-arrow-down");
            this._isDynamicMetadataContentOpen = true;
            this._dynamicMetadataContent.show();
            this._btnExpandUDFascicle.add_clicking(this.btnExpandUDFascicle_OnClick);
            this._lblWorkflowHandlerUser = $("#".concat(this.lblWorkflowHandlerUserId));
            this._pnlGridSearch = $("#".concat(this.pnlGrdSearchId));
            var domainUserConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "DomainUserModel");
            this._domainUserService = new DomainUserService(domainUserConfiguration);
            var workflowActivityConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, 'WorkflowActivity');
            this._workflowActivityService = new WorkflowActivityService(workflowActivityConfiguration);
            $("#".concat(this.rowAccountedRolesId)).hide();
            if (this.isEditPage) {
                $("#".concat(this.rowManagerId)).hide();
                $("#".concat(this.rowRolesId)).hide();
                $("#".concat(this.pnlUDId)).hide();
            }
            if (this.isAuthorizePage) {
                $("#".concat(this.rowManagerId)).hide();
                $("#".concat(this.rowRolesId)).hide();
                $("#".concat(this.rowAccountedRolesId)).show();
                $("#".concat(this.pnlUDId)).hide();
            }
            if (!this.isEditPage && !(this.fasciclesPanelVisibilities["GridSearchPanelVisibility"])) {
                this._pnlGridSearch.hide();
                this._grid.get_masterTableView().showFilterItem();
            }
            $("#".concat(this.uscFascFoldersId)).bind(UscFascicleFolders.RESIZE_EVENT, function (args) {
                _this.expandCollapseFolders();
                _this._expandedFoldersPanel = false;
            });
            $(".rspTabsContainer").bind("click", function () {
                if (_this._expandedFoldersPanel) {
                    _this.expandCollapseFolders();
                    _this._expandedFoldersPanel = false;
                    return;
                }
                _this._expandedFoldersPanel = true;
            });
            this.initializeUDFilterDataSource();
            this.bindLoaded();
        };
        /**
         *------------------------- Methods -----------------------------
         */
        /**
         * Ritorna un oggetto DocumentUnitFilterModel per i filtri da applicare alla griglia
         */
        //getFilterModel(): DocumentUnitFilterModel {
        //    let filterModel: DocumentUnitFilterModel = <DocumentUnitFilterModel>{};
        //    if (this._rcbUd.get_selectedItem() != null)
        //        filterModel.DocumentUnitName = this._rcbUd.get_selectedItem().get_value();
        //    if (this._rcbReferenceType.get_selectedItem() != null)
        //        filterModel.ReferenceType = FascicleReferenceType[this._rcbReferenceType.get_selectedItem().get_value()];
        //    filterModel.Title = (<any>this._txtTitleUD.get_element()).value;
        //    return filterModel;
        //}  
        uscFascicolo.prototype.initializeUDFilterDataSource = function () {
            var empty = { Name: '', Value: null };
            var protocol = { Name: 'Protocollo', Value: 'Protocollo' };
            var delibera = { Name: this.deliberaCaption, Value: this.deliberaCaption };
            var determina = { Name: this.determinaCaption, Value: this.determinaCaption };
            var archive = { Name: 'Archivio', Value: 'Archivio' };
            var miscellanea = { Name: 'Inserto', Value: 'Inserto' };
            var dataSource = [];
            dataSource.push(empty);
            dataSource.push(protocol);
            dataSource.push(delibera);
            dataSource.push(determina);
            dataSource.push(archive);
            dataSource.push(miscellanea);
            dataSource = dataSource.sort(function (first, second) {
                if (first.Name > second.Name) {
                    return 1;
                }
                if (first.Name < second.Name) {
                    return -1;
                }
                return 0;
            });
            var comboItem;
            for (var _i = 0, dataSource_1 = dataSource; _i < dataSource_1.length; _i++) {
                var dataItem = dataSource_1[_i];
                comboItem = new Telerik.Web.UI.RadComboBoxItem();
                comboItem.set_text(dataItem.Name);
                comboItem.set_value(dataItem.Value);
                this._rcbUd.get_items().add(comboItem);
            }
            this._racUDDataSource.set_data(dataSource);
            this._racUDDataSource.fetch(undefined);
        };
        /**
         * Ritorna la query string odata per i filtri
         */
        uscFascicolo.prototype.getFilterModel = function () {
            var filterQs = "";
            this._grid = $find(this.grdUDId);
            if (!(this.fasciclesPanelVisibilities["GridSearchPanelVisibility"])) {
                var filters = this._grid.get_masterTableView().get_filterExpressions();
                var filtersCount_1 = filters.get_count();
                if (filtersCount_1 > 0) {
                    filterQs = "$filter=";
                    var currentIndex_1 = 1;
                    filters.forEach(function (filter) {
                        var className = "";
                        if (filter.get_fieldValue() != "") {
                            if (filter.get_columnUniqueName() == "ReferenceType") {
                                className = "VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles.ReferenceType";
                            }
                            if (filter.get_columnUniqueName() == "Title") {
                                filterQs = filterQs.concat("contains(tolower(Subject), '", filter.get_fieldValue().toLowerCase(), "')");
                            }
                            else {
                                if (filter.get_columnUniqueName() == "DocumentUnitName" && filter.get_fieldValue() == "Archivio") {
                                    filterQs = filterQs.concat("Environment ge 100");
                                }
                                else {
                                    filterQs = filterQs.concat(filter.get_columnUniqueName(), " eq ", className, "'", filter.get_fieldValue(), "'");
                                }
                            }
                            if (currentIndex_1 < filtersCount_1) {
                                filterQs = filterQs.concat(" and ");
                            }
                        }
                        ++currentIndex_1;
                    });
                }
            }
            else {
                filterQs = "$filter=";
                if (this._rcbUd.get_selectedItem() != null && this._rcbUd.get_selectedItem().get_value() != "") {
                    var selectedValue = this._rcbUd.get_selectedItem().get_value();
                    if (selectedValue == "Archivio") {
                        filterQs = filterQs.concat("Environment ge 100 and ");
                    }
                    else {
                        filterQs = filterQs.concat("DocumentUnitName eq '", this._rcbUd.get_selectedItem().get_value(), "' and ");
                    }
                }
                if (this._rcbReferenceType.get_selectedItem() != null && this._rcbReferenceType.get_selectedItem().get_value() != "")
                    filterQs = filterQs.concat("ReferenceType eq VecompSoftware.DocSuiteWeb.Model.Entities.Fascicles.ReferenceType'", this._rcbReferenceType.get_selectedItem().get_value(), "' and ");
                filterQs = filterQs.concat("contains(Subject, '", this._txtTitleUD.get_element().value, "')");
            }
            var orders = this._grid.get_masterTableView().get_sortExpressions();
            var ordersCount = orders.get_count();
            if (ordersCount > 0) {
                filterQs = filterQs.concat("&$orderby=");
                var currentIndex_2 = 1;
                orders.forEach(function (order) {
                    filterQs = filterQs.concat(order.FieldName);
                    if (order.SortOrder === 2) {
                        filterQs = filterQs.concat(" desc");
                    }
                    if (currentIndex_2 < ordersCount) {
                        filterQs = filterQs.concat(", ");
                    }
                    ++currentIndex_2;
                });
            }
            return filterQs;
        };
        /**
         * Scatena l'evento di "load completed" del controllo
         */
        uscFascicolo.prototype.bindLoaded = function () {
            $("#".concat(this.pageId)).data(this);
            $("#".concat(this.pageId)).triggerHandler(uscFascicolo.LOADED_EVENT);
        };
        /**
         * Carica i dati dello user control
         */
        uscFascicolo.prototype.loadData = function (fascicle) {
            var _this = this;
            if (fascicle == null)
                return;
            this.loadFascFoldersData(fascicle)
                .done(function () {
                _this.setSummaryData(fascicle);
                _this.loadUscSummaryFascicle(fascicle);
            });
        };
        uscFascicolo.prototype.loadUscSummaryFascicle = function (fascicle) {
            var uscFascSummary = $("#".concat(this.uscFascSummaryId)).data();
            if (!jQuery.isEmptyObject(uscFascSummary)) {
                uscFascSummary.workflowActivityId = this.workflowActivityId;
                uscFascSummary.loadData(fascicle);
            }
        };
        uscFascicolo.prototype.loadFascFoldersData = function (fascicle) {
            var promise = $.Deferred();
            var uscFascFolders = $("#".concat(this.uscFascFoldersId)).data();
            if (!jQuery.isEmptyObject(uscFascFolders)) {
                uscFascFolders.setRootNode(fascicle.UniqueId);
                uscFascFolders.loadFolders(fascicle.UniqueId)
                    .done(function () { return promise.resolve(); })
                    .fail(function (exception) { return promise.reject(exception); });
            }
            else {
                return promise.resolve();
            }
            return promise.promise();
        };
        /**
         * Imposta i dati nel sommario
         * @param fascicle
         */
        uscFascicolo.prototype.setSummaryData = function (fascicle) {
            var _this = this;
            sessionStorage.removeItem("CurrentMetadataValues");
            var fascicleTypeName = "";
            switch (FascicleType[fascicle.FascicleType.toString()]) {
                case FascicleType.Procedure:
                    fascicleTypeName = "Fascicolo di procedimento";
                    break;
                case FascicleType.Period:
                    fascicleTypeName = "Fascicolo periodico";
                    $("#".concat(this.rowManagerId)).hide();
                    break;
                case FascicleType.Legacy:
                    fascicleTypeName = "Fascicolo non a norma";
                    break;
                case FascicleType.Activity:
                    fascicleTypeName = "Fascicolo di attività";
                    $("#".concat(this.rowManagerId)).hide();
                    break;
            }
            if ($.type(fascicle.FascicleType) === "string") {
                fascicle.FascicleType = FascicleType[fascicle.FascicleType.toString()];
            }
            $("#".concat(this.rowManagerId)).hide();
            if (fascicle.Contacts.length > 0 && !this.isEditPage) {
                $("#".concat(this.rowManagerId)).show();
            }
            var handler = "";
            var role;
            if (!String.isNullOrEmpty(this.workflowActivityId)) {
                this._workflowActivityService.getWorkflowActivity(this.workflowActivityId, function (data) {
                    if (data == null)
                        return;
                    _this._workflowActivity = data;
                    var subject;
                    // Per ora non si mostra il proponente del flusso di lavoro, questa informazione sarà visibile solo nella scrivania del flusso di lavoro
                    //if (this._workflowActivity.WorkflowProperties != null) {
                    //    let mapper: WorkflowRoleModelMapper = new WorkflowRoleModelMapper();
                    //    let propertyRole: WorkflowPropertyModel = this._workflowActivity.WorkflowProperties.filter(function (item) { if (item.Name == WorkflowPropertyHelper.DSW_PROPERTY_PROPOSER_ROLE) return item; })[0];
                    //    role = mapper.Map(JSON.parse(propertyRole.ValueString));                        
                    //}
                    subject = _this._workflowActivity.WorkflowProperties.filter(function (item) { if (item.Name == WorkflowPropertyHelper.DSW_PROPERTY_SUBJECT)
                        return item; })[0].ValueString;
                    fascicle.Note = subject;
                    _this.loadUscSummaryFascicle(fascicle);
                    if (_this._workflowActivity.WorkflowAuthorizations) {
                        var authorization = _this._workflowActivity.WorkflowAuthorizations.filter(function (item) { if (item.IsHandler == true)
                            return item; })[0];
                        if (authorization) {
                            handler = authorization.Account;
                            _this._domainUserService.getUser(handler, function (user) {
                                if (user) {
                                    _this.loadExternalDataAjaxRequest(fascicle, user.DisplayName);
                                }
                            }, function (exception) {
                                _this._uscNotification = $("#".concat(_this.uscNotificationId)).data();
                                if (!jQuery.isEmptyObject(_this._uscNotification)) {
                                    _this._uscNotification.showNotification(exception);
                                }
                            });
                        }
                    }
                }, function (exception) {
                    _this._uscNotification = $("#".concat(_this.uscNotificationId)).data();
                    if (!jQuery.isEmptyObject(_this._uscNotification)) {
                        _this._uscNotification.showNotification(exception);
                    }
                });
            }
            if (this.fasciclesPanelVisibilities["FasciclesLinkedPanelVisibility"] != undefined && this.fasciclesPanelVisibilities["FasciclesLinkedPanelVisibility"]) {
                $.when(this.refreshLinkedFasciclesRequest(fascicle)).always(function () {
                    setTimeout(function () {
                        _this.loadExternalDataAjaxRequest(fascicle, handler);
                    }, 1);
                });
            }
            else {
                this.loadExternalDataAjaxRequest(fascicle, handler);
            }
        };
        uscFascicolo.prototype.loadExternalDataAjaxRequest = function (fascicle, workflowHandler) {
            var jsonFascicle = JSON.stringify(fascicle);
            this._ajaxManager = $find(this.ajaxManagerId);
            var ajaxModel = {};
            ajaxModel.Value = new Array();
            ajaxModel.ActionName = "LoadExternalData";
            ajaxModel.Value = new Array();
            ajaxModel.Value.push(jsonFascicle);
            ajaxModel.Value.push(JSON.stringify(!String.isNullOrEmpty(this.workflowActivityId)));
            ajaxModel.Value.push(workflowHandler);
            if (!this.isEditPage && this.metadataRepositoryEnabled && fascicle.MetadataValues) {
                $("#".concat(this.rowDynamicMetadataId)).show();
                sessionStorage.setItem("CurrentMetadataValues", fascicle.MetadataValues);
            }
            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
        };
        /**
         * Callback del caricamento della lista dei manager
         */
        uscFascicolo.prototype.loadExternalDataCallback = function () {
            var uscFascicleFolder = $("#".concat(this.uscFascFoldersId)).data();
            if (!jQuery.isEmptyObject(uscFascicleFolder)) {
                uscFascicleFolder.selectFascicleNode();
            }
            $("#".concat(this.pageId)).triggerHandler(uscFascicolo.DATA_LOADED_EVENT);
        };
        /**
        * Metodo per il caricamento della griglia dei fascicoli collegati
        * @param sender
        * @param onDoneCallback
        */
        uscFascicolo.prototype.refreshLinkedFasciclesRequest = function (fascicle) {
            var _this = this;
            var result = $.Deferred();
            this._service.getLinkedFascicles(fascicle, null, function (data) {
                _this.refreshLinkedFascicles(data);
                return result.resolve();
            }, function (exception) {
                _this._uscNotification = $("#".concat(_this.uscNotificationId)).data();
                if (!jQuery.isEmptyObject(_this._uscNotification)) {
                    _this._uscNotification.showNotification(exception);
                }
                return result.reject();
            });
            return result.promise();
        };
        /**
         * Metodo per caricare le UD in griglia
         * @param models
         */
        uscFascicolo.prototype.refreshGridUD = function (models, insertsArchiveChains) {
            var filters = {};
            if (!(this.fasciclesPanelVisibilities["GridSearchPanelVisibility"])) {
                this._grid.get_masterTableView().get_filterExpressions().forEach(function (item, index) {
                    filters[item.get_columnUniqueName()] = item.get_fieldValue();
                });
            }
            var selectedFolder = this.getSelectedFascicleFolder();
            if (selectedFolder) {
                $("#".concat(this.lblUDGridTitleId)).html("Documenti nel fascicolo (".concat(selectedFolder.Name, ")"));
            }
            var orders = this._grid.get_masterTableView().get_sortExpressions();
            var sort = [];
            orders.forEach(function (order) {
                sort.push({
                    FieldName: order.FieldName,
                    SortOrder: order.SortOrder
                });
            });
            var ajaxModel = {};
            ajaxModel.Value = new Array();
            ajaxModel.ActionName = "ReloadGrid";
            ajaxModel.Value = new Array();
            ajaxModel.Value.push(JSON.stringify(models));
            ajaxModel.Value.push(JSON.stringify(filters));
            ajaxModel.Value.push(JSON.stringify(insertsArchiveChains));
            ajaxModel.Value.push(JSON.stringify(sort));
            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
        };
        /**
         * Callback di finalizzazione caricamento griglia
         */
        uscFascicolo.prototype.refreshGridUDCallback = function (filters, orders) {
            this._grid = $find(this.grdUDId);
            var filtersModel = new FiltersGridUDFasciclesViewModelMapper().Map(filters);
            for (var prop in filtersModel) {
                if (filtersModel[prop]) {
                    var filt = new Telerik.Web.UI.GridFilterExpression();
                    filt.set_columnUniqueName(prop);
                    filt.set_fieldValue(filtersModel[prop]);
                    this._grid.get_masterTableView().get_filterExpressions().add(filt);
                    if (prop == "DocumentUnitName" && !(this.fasciclesPanelVisibilities["GridSearchPanelVisibility"])) {
                        try {
                            var dropDownItem = $find($(".udComboFilter")[0].id);
                            var toSelectItem = dropDownItem.findItemByValue(filtersModel[prop]);
                            this._notFireEvent = true;
                            toSelectItem.select();
                        }
                        finally {
                            this._notFireEvent = false;
                        }
                    }
                }
            }
            ;
            $("#".concat(this.pageId)).triggerHandler(uscFascicolo.GRID_REFRESH_EVENT);
            for (var _i = 0, orders_1 = orders; _i < orders_1.length; _i++) {
                var order = orders_1[_i];
                var sortExpression = new Telerik.Web.UI.GridSortExpression();
                sortExpression.set_fieldName(order.FieldName);
                sortExpression.set_sortOrder(order.SortOrder);
                this._grid.get_masterTableView().get_sortExpressions().add(sortExpression);
                var gridMasterTableView = this._grid.get_masterTableView();
                gridMasterTableView._showSortIconForField(order.FieldName, order.SortOrder);
            }
            this._loadingPanel.hide(this.grdUDId);
        };
        uscFascicolo.prototype.openPreviewWindow = function (serializedDoc) {
            var url = '../Viewers/DocumentInfoViewer.aspx?'.concat(serializedDoc);
            this.openWindow(url, 'windowPreviewDocument', 750, 450);
        };
        /**
    * Apre una nuova nuova RadWindow
    * @param url
    * @param name
    * @param width
    * @param height
    */
        uscFascicolo.prototype.openWindow = function (url, name, width, height) {
            var manager = $find(this.rwmDocPreviewId);
            var wnd = manager.open(url, name, null);
            wnd.setSize(width, height);
            wnd.set_modal(true);
            wnd.center();
            return false;
        };
        uscFascicolo.prototype.showNotificationException = function (uscNotificationId, exception, customMessage) {
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
        uscFascicolo.prototype.showNotificationMessage = function (uscNotificationId, customMessage) {
            var uscNotification = $("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotificationMessage(customMessage);
            }
        };
        uscFascicolo.prototype.getSelectedFascicleFolder = function () {
            var uscFascFolders = $("#".concat(this.uscFascFoldersId)).data();
            if (!jQuery.isEmptyObject(uscFascFolders)) {
                return uscFascFolders.getSelectedFascicleFolder(this.currentFascicleId);
            }
            return undefined;
        };
        uscFascicolo.prototype.expandCollapseFolders = function () {
            var pnl = $find(this.rszFolderId);
            var pane = pnl.getPaneById(this.rsPnlFoldersId);
            if (pane.get_collapsed()) {
                pnl.expandPane(this.rsPnlFoldersId);
                pnl.dockPane(this.rsPnlFoldersId);
            }
            else {
                pnl.undockPane(this.rsPnlFoldersId);
                pnl.collapsePane(this.rsPnlFoldersId);
            }
        };
        uscFascicolo.prototype.getSelectedAccountedVisibilityType = function () {
            if (this.isAuthorizePage) {
                var uscAccountedRoles = $("#".concat(this.uscSettoriAccountedId)).data();
                if (!jQuery.isEmptyObject(uscAccountedRoles)) {
                    return uscAccountedRoles.getFascicleVisibilityType();
                }
            }
            return undefined;
        };
        uscFascicolo.LOADED_EVENT = "onLoaded";
        uscFascicolo.DATA_LOADED_EVENT = "onDataLoaded";
        uscFascicolo.GRID_REFRESH_EVENT = "onGridRefresh";
        uscFascicolo.REBIND_EVENT = "onRebind";
        return uscFascicolo;
    }());
    return uscFascicolo;
});
//# sourceMappingURL=uscFascicolo.js.map