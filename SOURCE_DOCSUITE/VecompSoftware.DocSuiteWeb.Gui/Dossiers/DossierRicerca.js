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
define(["require", "exports", "App/DTOs/DossierSearchFilterDTO", "App/Helpers/ServiceConfigurationHelper", "Dossiers/DossierBase", "App/Services/Commons/ContainerService"], function (require, exports, DossierSearchFilterDTO, ServiceConfigurationHelper, DossierBase, ContainerService) {
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
                var metadataValueFilter = _this._txtMetadataValue.get_value();
                var startDateFromFilter = "";
                if (_this._rdpStartDateFrom.get_selectedDate()) {
                    startDateFromFilter = _this._rdpStartDateFrom.get_selectedDate().format("yyyy-MM-dd").toString();
                }
                var startDateToFilter = "";
                if (_this._rdpStartDateTo.get_selectedDate()) {
                    startDateToFilter = _this._rdpStartDateTo.get_selectedDate().format("yyyy-MM-dd").toString();
                }
                var endDateFromFilter = "";
                if (_this._rdpEndDateFrom.get_selectedDate()) {
                    endDateFromFilter = _this._rdpEndDateFrom.get_selectedDate().format("yyyy-MM-dd").toString();
                }
                var endDateToFilter = "";
                if (_this._rdpEndDateTo.get_selectedDate()) {
                    endDateToFilter = _this._rdpEndDateTo.get_selectedDate().format("yyyy-MM-dd").toString();
                }
                var containerFilter = "";
                if (_this._rdlContainer.get_selectedItem()) {
                    containerFilter = _this._rdlContainer.get_selectedItem().get_value();
                }
                var metadataRepositoryId = "";
                var uscMetadataRepositorySel = $("#".concat(_this.uscMetadataRepositorySelId)).data();
                if (!jQuery.isEmptyObject(uscMetadataRepositorySel)) {
                    metadataRepositoryId = uscMetadataRepositorySel.getSelectedMetadataRepositoryId();
                }
                searchDTO.year = yearFilter ? +yearFilter : null;
                searchDTO.number = numberFilter ? +numberFilter : null;
                searchDTO.subject = subjectFilter;
                searchDTO.note = noteFilter;
                searchDTO.idContainer = containerFilter ? +containerFilter : null;
                searchDTO.endDateFrom = endDateFromFilter;
                searchDTO.endDateTo = endDateToFilter;
                searchDTO.startDateFrom = startDateFromFilter;
                searchDTO.startDateTo = startDateToFilter;
                searchDTO.idMetadataRepository = metadataRepositoryId ? metadataRepositoryId : null;
                searchDTO.metadataValue = metadataValueFilter;
                sessionStorage.setItem("DossierSearch", JSON.stringify(searchDTO));
                window.location.href = "../Dossiers/DossierRisultati.aspx?Type=Dossier";
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
                var dossierLastSearch = sessionStorage.getItem("DossierSearch");
                if (_this.hasTxtYearDefaultValue == true) {
                    _this._txtYear.set_value(new Date().getFullYear().toString());
                }
                if (dossierLastSearch) {
                    var lastsearchFilter = JSON.parse(dossierLastSearch);
                    _this._txtYear.set_value(lastsearchFilter.year ? lastsearchFilter.year.toString() : null);
                    _this._txtNumber.set_value(lastsearchFilter.number ? lastsearchFilter.number.toString() : null);
                    _this._txtSubject.set_value(lastsearchFilter.subject);
                    _this._txtMetadataValue.set_value(lastsearchFilter.metadataValue);
                    if (lastsearchFilter.idContainer) {
                        var selectedItem = _this._rdlContainer.findItemByValue(lastsearchFilter.idContainer.toString());
                        selectedItem.set_selected(true);
                        _this._rdlContainer.trackChanges();
                    }
                }
            };
            _this.loadContainers = function () {
                _this._containerService.getAnyDossierAuthorizedContainers(function (data) {
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
                sessionStorage.removeItem("DossierSearch");
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
            this._btnSearch = $find(this.btnSearchId);
            this._btnSearch.add_clicking(this.btnSearch_onClick);
            this._btnClean = $find(this.btnCleanId);
            this._btnClean.add_clicking(this.btnClean_onClick);
            this._txtYear = $find(this.txtYearId);
            this._txtNumber = $find(this.txtNumberId);
            this._txtSubject = $find(this.txtSubjectId);
            this._txtNote = $find(this.txtNoteId);
            this._txtMetadataValue = $find(this.txtMetadataValueId);
            this._rdlContainer = $find(this.rdlContainerId);
            this._rdpEndDateFrom = $find(this.rdpEndDateFromId);
            this._rdpEndDateTo = $find(this.rdpEndDateToId);
            this._rdpStartDateFrom = $find(this.rdpStartDateFromId);
            this._rdpStartDateTo = $find(this.rdpStartDateToId);
            this._loadingPanel = $find(this.ajaxLoadingPanelId);
            var containerConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Container");
            this._containerService = new ContainerService(containerConfiguration);
            this._loadingPanel.show(this.searchTableId);
            this._btnSearch.set_enabled(false);
            this._btnClean.set_enabled(false);
            this._dossierStatusRow = $("#".concat(this.dossierStatusRowId));
            this._dossierStatusRow.hide();
            this._rowMetadataRepository = $("#".concat(this.rowMetadataRepositoryId));
            this._rowMetadataRepository.hide();
            this._rowMetadataValue = $("#".concat(this.rowMetadataValueId));
            this._rowMetadataValue.hide();
            this.loadContainers();
            if (this.metadataRepositoryEnabled) {
                this._rowMetadataRepository.show();
                this._rowMetadataValue.show();
            }
        };
        return DossierRicerca;
    }(DossierBase));
    return DossierRicerca;
});
//# sourceMappingURL=DossierRicerca.js.map