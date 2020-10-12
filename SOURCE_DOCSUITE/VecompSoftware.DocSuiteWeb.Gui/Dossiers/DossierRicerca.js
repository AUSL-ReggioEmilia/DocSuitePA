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
define(["require", "exports", "App/DTOs/DossierSearchFilterDTO", "App/Helpers/ServiceConfigurationHelper", "Dossiers/DossierBase", "App/Services/Commons/ContainerService", "App/Models/Dossiers/DossierType", "App/Helpers/EnumHelper", "App/Models/Dossiers/DossierStatus", "App/Helpers/SessionStorageKeysHelper"], function (require, exports, DossierSearchFilterDTO, ServiceConfigurationHelper, DossierBase, ContainerService, DossierType, EnumHelper, DossierStatus, SessionStorageKeysHelper) {
    var DossierRicerca = /** @class */ (function (_super) {
        __extends(DossierRicerca, _super);
        /**
     * Costruttore
     * @param webApiConfiguration
     */
        function DossierRicerca(serviceConfigurations) {
            var _this = _super.call(this, ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME)) || this;
            /**
            *------------------------- Events -----------------------------
            */
            /**
             * Evento scatenato al click del pulsante di ricerca
             * @param sender
             * @param args
             */
            _this.btnSearch_onClick = function (sender, args) {
                var searchDTO = new DossierSearchFilterDTO();
                var yearFilter = _this._txtYear.get_value();
                var numberFilter = _this._txtNumber.get_value();
                var subjectFilter = _this._txtSubject.get_value();
                var noteFilter = _this._txtNote.get_value();
                var startDateFromFilter = null;
                if (_this._rdpStartDateFrom.get_selectedDate()) {
                    startDateFromFilter = _this._rdpStartDateFrom.get_selectedDate().format("yyyy-MM-dd").toString();
                }
                var startDateToFilter = null;
                if (_this._rdpStartDateTo.get_selectedDate()) {
                    startDateToFilter = _this._rdpStartDateTo.get_selectedDate().format("yyyy-MM-dd").toString();
                }
                var endDateFromFilter = null;
                if (_this._rdpEndDateFrom.get_selectedDate()) {
                    endDateFromFilter = _this._rdpEndDateFrom.get_selectedDate().format("yyyy-MM-dd").toString();
                }
                var endDateToFilter = null;
                if (_this._rdpEndDateTo.get_selectedDate()) {
                    endDateToFilter = _this._rdpEndDateTo.get_selectedDate().format("yyyy-MM-dd").toString();
                }
                var containerFilter = "";
                if (_this._rdlContainer.get_selectedItem()) {
                    containerFilter = _this._rdlContainer.get_selectedItem().get_value();
                }
                var metadataRepositoryId = "";
                var uscMetadataRepositorySel = $("#".concat(_this.uscMetadataRepositorySelId)).data();
                if (!jQuery.isEmptyObject(uscMetadataRepositorySel) && _this.metadataRepositoryEnabled) {
                    metadataRepositoryId = uscMetadataRepositorySel.getSelectedMetadataRepositoryId();
                    var _a = uscMetadataRepositorySel.getMetadataFilterValues(), metadataValue = _a[0], metadataFinderModels = _a[1], metadataValuesAreValid = _a[2];
                    searchDTO.MetadataValue = metadataValue;
                    searchDTO.MetadataValues = metadataFinderModels;
                    if (!metadataValuesAreValid) {
                        alert("Alcuni valori di metadati non sono validi");
                        return;
                    }
                }
                searchDTO.Year = yearFilter ? +yearFilter : null;
                searchDTO.Number = numberFilter ? +numberFilter : null;
                searchDTO.Subject = subjectFilter;
                searchDTO.Note = noteFilter;
                searchDTO.IdContainer = containerFilter ? +containerFilter : null;
                searchDTO.EndDateFrom = endDateFromFilter;
                searchDTO.EndDateTo = endDateToFilter;
                searchDTO.StartDateFrom = startDateFromFilter;
                searchDTO.StartDateTo = startDateToFilter;
                searchDTO.IdMetadataRepository = metadataRepositoryId ? metadataRepositoryId : null;
                var uscCategoryRest = $("#" + _this.uscCategoryRestId).data();
                var category = uscCategoryRest.getSelectedCategory();
                searchDTO.IdCategory = category ? category.EntityShortId : null;
                var dossierType = _this._rcbDossierType.get_selectedItem().get_value();
                searchDTO.DossierType = dossierType ? dossierType : null;
                if (!_this.dossierStatusEnabled) {
                    searchDTO.Status = DossierStatus.Open.toString();
                }
                else {
                    var checkedStatus = $("#" + _this.rblDossierStatusId + " input:checked").val();
                    searchDTO.Status = checkedStatus !== "All" ? checkedStatus : null;
                }
                sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_DOSSIER_SEARCH, JSON.stringify(searchDTO));
                var url = "../Dossiers/DossierRisultati.aspx?Type=Dossier";
                if (_this.isWindowPopupEnable) {
                    url = url + "&IsWindowPopupEnable=True";
                }
                if (_this.dossierStatusEnabled) {
                    url = url + "&DossierStatusEnabled=True";
                }
                window.location.href = url;
            };
            /**
           * Evento scatenato al click del pulsante di svuota ricerca
           * @param sender
           * @param args
           */
            _this.btnClean_onClick = function (sender, args) {
                _this.cleanSearchFilters();
            };
            /**
            *------------------------- Methods -----------------------------
            */
            _this.setLastSearchFilter = function () {
                var dossierLastSearch = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOSSIER_SEARCH);
                if (_this.hasTxtYearDefaultValue == true) {
                    _this._txtYear.set_value(new Date().getFullYear().toString());
                }
                if (dossierLastSearch) {
                    var lastsearchFilter = JSON.parse(dossierLastSearch);
                    _this._txtYear.set_value(lastsearchFilter.Year ? lastsearchFilter.Year.toString() : null);
                    _this._txtNumber.set_value(lastsearchFilter.Number ? lastsearchFilter.Number.toString() : null);
                    _this._txtSubject.set_value(lastsearchFilter.Subject);
                    if (lastsearchFilter.IdContainer) {
                        var selectedItem = _this._rdlContainer.findItemByValue(lastsearchFilter.IdContainer.toString());
                        selectedItem.set_selected(true);
                        _this._rdlContainer.trackChanges();
                    }
                }
            };
            _this.loadContainers = function () {
                _this._containerService.getAnyDossierAuthorizedContainers(_this.currentTenantId, function (data) {
                    if (!data)
                        return;
                    var containers = data;
                    _this.addContainers(containers, _this._rdlContainer);
                    _this.setLastSearchFilter();
                    _this._loadingPanel.hide(_this.searchTableId);
                    _this._btnSearch.set_enabled(true);
                    _this._btnClean.set_enabled(true);
                }, function (exception) {
                    _this._loadingPanel.hide(_this.searchTableId);
                    _this._btnSearch.set_enabled(false);
                    _this.showNotificationException(_this.uscNotificationId, exception);
                });
            };
            _this.cleanSearchFilters = function () {
                _this._txtYear.set_value('');
                _this._txtNumber.set_value('');
                _this._txtSubject.set_value('');
                var selectedContainer = _this._rdlContainer.get_selectedItem();
                if (selectedContainer) {
                    selectedContainer.set_selected(false);
                }
                var uscCategoryRest = $("#" + _this.uscCategoryRestId).data();
                uscCategoryRest.clearTree();
                _this._rcbDossierType.get_items().getItem(0).select();
                var openedCheckbox = $("#" + _this.rblDossierStatusId + " input:radio")[1];
                openedCheckbox.checked = "checked";
                sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_DOSSIER_SEARCH);
            };
            _this._serviceConfigurations = serviceConfigurations;
            $(document).ready(function () {
            });
            return _this;
        }
        /**
        * Initialize
        */
        DossierRicerca.prototype.initialize = function () {
            this._enumHelper = new EnumHelper();
            this._btnSearch = $find(this.btnSearchId);
            this._btnSearch.add_clicking(this.btnSearch_onClick);
            this._btnClean = $find(this.btnCleanId);
            this._btnClean.add_clicking(this.btnClean_onClick);
            this._txtYear = $find(this.txtYearId);
            this._txtNumber = $find(this.txtNumberId);
            this._txtSubject = $find(this.txtSubjectId);
            this._txtNote = $find(this.txtNoteId);
            this._rdlContainer = $find(this.rdlContainerId);
            this._rdpEndDateFrom = $find(this.rdpEndDateFromId);
            this._rdpEndDateTo = $find(this.rdpEndDateToId);
            this._rdpStartDateFrom = $find(this.rdpStartDateFromId);
            this._rdpStartDateTo = $find(this.rdpStartDateToId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            this._rcbDossierType = $find(this.rcbDossierTypeId);
            var containerConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Container");
            this._containerService = new ContainerService(containerConfiguration);
            this._loadingPanel.show(this.searchTableId);
            this._btnSearch.set_enabled(false);
            this._btnClean.set_enabled(false);
            this._rowMetadataRepository = $("#".concat(this.rowMetadataRepositoryId));
            $("#" + this.metadataTableId).hide();
            this.loadContainers();
            this.populateDossierTypeComboBox();
            if (this.metadataRepositoryEnabled) {
                $("#" + this.metadataTableId).show();
            }
        };
        DossierRicerca.prototype.populateDossierTypeComboBox = function () {
            var rcbItem = new Telerik.Web.UI.RadComboBoxItem();
            rcbItem.set_text("");
            this._rcbDossierType.get_items().add(rcbItem);
            for (var dossierType in DossierType) {
                if (typeof DossierType[dossierType] === 'string') {
                    var rcbItem_1 = new Telerik.Web.UI.RadComboBoxItem();
                    rcbItem_1.set_text(this._enumHelper.getDossierTypeDescription(DossierType[dossierType]));
                    rcbItem_1.set_value(DossierType[dossierType]);
                    this._rcbDossierType.get_items().add(rcbItem_1);
                }
            }
            this._rcbDossierType.get_items().getItem(0).select();
        };
        return DossierRicerca;
    }(DossierBase));
    return DossierRicerca;
});
//# sourceMappingURL=DossierRicerca.js.map