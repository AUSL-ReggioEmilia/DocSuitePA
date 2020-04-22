import DossierSearchFilterDTO = require("App/DTOs/DossierSearchFilterDTO");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import DossierBase = require('Dossiers/DossierBase');
import ContainerService = require('App/Services/Commons/ContainerService');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscMetadataRepositorySel = require('UserControl/uscMetadataRepositorySel');

class DossierRicerca extends DossierBase {

    btnSearchId: string;
    txtYearId: string;
    txtNumberId: string;
    txtSubjectId: string;
    txtNoteId: string;
    rdlContainerId: string;
    rdpStartDateFromId: string;
    rdpStartDateToId: string;
    rdpEndDateToId: string;
    rdpEndDateFromId: string;
    uscNotificationId: string;
    ajaxLoadingPanelId: string;
    searchTableId: string;
    btnCleanId: string;
    dossierStatusRowId: string;
    hasTxtYearDefaultValue: boolean;
    uscMetadataRepositorySelId: string;
    metadataRepositoryEnabled: boolean;
    rowMetadataValueId: string;
    rowMetadataRepositoryId: string;
    txtMetadataValueId: string;

    private _btnSearch: Telerik.Web.UI.RadButton;
    private _txtYear: Telerik.Web.UI.RadTextBox;
    private _txtNumber: Telerik.Web.UI.RadTextBox;
    private _txtSubject: Telerik.Web.UI.RadTextBox;
    private _txtNote: Telerik.Web.UI.RadTextBox;
    private _txtMetadataValue: Telerik.Web.UI.RadTextBox;
    private _rdlContainer: Telerik.Web.UI.RadDropDownList;
    private _rdpStartDateFrom: Telerik.Web.UI.RadDatePicker;
    private _rdpStartDateTo: Telerik.Web.UI.RadDatePicker;
    private _rdpEndDateFrom: Telerik.Web.UI.RadDatePicker;
    private _rdpEndDateTo: Telerik.Web.UI.RadDatePicker;
    private _serviceConfigurations: ServiceConfiguration[];
    private _containerService: ContainerService;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _btnClean: Telerik.Web.UI.RadButton;
    private _dossierStatusRow: JQuery;
    private _rowMetadataRepository: JQuery;
    private _rowMetadataValue: JQuery;

    /**
 * Costruttore
 * @param webApiConfiguration
 */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {
        });
    }


    /**
    * Initialize
    */
    initialize() {
        this._btnSearch = <Telerik.Web.UI.RadButton>$find(this.btnSearchId);
        this._btnSearch.add_clicking(this.btnSearch_onClick);
        this._btnClean = <Telerik.Web.UI.RadButton>$find(this.btnCleanId);
        this._btnClean.add_clicking(this.btnClean_onClick);
        this._txtYear = <Telerik.Web.UI.RadTextBox>$find(this.txtYearId);
        this._txtNumber = <Telerik.Web.UI.RadTextBox>$find(this.txtNumberId);
        this._txtSubject = <Telerik.Web.UI.RadTextBox>$find(this.txtSubjectId);
        this._txtNote = <Telerik.Web.UI.RadTextBox>$find(this.txtNoteId);
        this._txtMetadataValue = <Telerik.Web.UI.RadTextBox>$find(this.txtMetadataValueId);
        this._rdlContainer = <Telerik.Web.UI.RadDropDownList>$find(this.rdlContainerId);
        this._rdpEndDateFrom = <Telerik.Web.UI.RadDatePicker>$find(this.rdpEndDateFromId);
        this._rdpEndDateTo = <Telerik.Web.UI.RadDatePicker>$find(this.rdpEndDateToId);
        this._rdpStartDateFrom = <Telerik.Web.UI.RadDatePicker>$find(this.rdpStartDateFromId);
        this._rdpStartDateTo = <Telerik.Web.UI.RadDatePicker>$find(this.rdpStartDateToId);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        let containerConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Container");
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
    }


    /**
    *------------------------- Events -----------------------------
    */

    /**
     * Evento scatenato al click del pulsante di ricerca
     * @param sender
     * @param args
     */

    btnSearch_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {

        let searchDTO: DossierSearchFilterDTO = new DossierSearchFilterDTO();
        let yearFilter: string = this._txtYear.get_value();
        let numberFilter: string = this._txtNumber.get_value();
        let subjectFilter: string = this._txtSubject.get_value();
        let noteFilter: string = this._txtNote.get_value();
        let metadataValueFilter: string = this._txtMetadataValue.get_value();
        let startDateFromFilter: string = "";
        if (this._rdpStartDateFrom.get_selectedDate()) {
            startDateFromFilter = this._rdpStartDateFrom.get_selectedDate().format("yyyy-MM-dd").toString();
        }
        let startDateToFilter: string = "";
        if (this._rdpStartDateTo.get_selectedDate()) {
            startDateToFilter = this._rdpStartDateTo.get_selectedDate().format("yyyy-MM-dd").toString();
        }
        let endDateFromFilter: string = "";
        if (this._rdpEndDateFrom.get_selectedDate()) {
            endDateFromFilter = this._rdpEndDateFrom.get_selectedDate().format("yyyy-MM-dd").toString();
        }
        let endDateToFilter: string = ""
        if (this._rdpEndDateTo.get_selectedDate()) {
            endDateToFilter = this._rdpEndDateTo.get_selectedDate().format("yyyy-MM-dd").toString();
        }

        let containerFilter: string = "";
        if (this._rdlContainer.get_selectedItem()) {
            containerFilter = this._rdlContainer.get_selectedItem().get_value();
        }

        let metadataRepositoryId: string = "";
        let uscMetadataRepositorySel: UscMetadataRepositorySel = <UscMetadataRepositorySel>$("#".concat(this.uscMetadataRepositorySelId)).data();
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

    }

    /**
   * Evento scatenato al click del pulsante di svuota ricerca
   * @param sender
   * @param args
   */

    btnClean_onClick = (sender: any, args: Telerik.Web.UI.RadButtonEventArgs) => {
        this.cleanSearchFilters();
    }


    /**
    *------------------------- Methods -----------------------------
    */

    setLastSearchFilter = () => {
        let dossierLastSearch: string = sessionStorage.getItem("DossierSearch");
        if (this.hasTxtYearDefaultValue == true) {
            this._txtYear.set_value(new Date().getFullYear().toString());
        }
        if (dossierLastSearch) {
            let lastsearchFilter: DossierSearchFilterDTO = <DossierSearchFilterDTO>JSON.parse(dossierLastSearch);
            this._txtYear.set_value(lastsearchFilter.year ? lastsearchFilter.year.toString() : null);
            this._txtNumber.set_value(lastsearchFilter.number ? lastsearchFilter.number.toString() : null);
            this._txtSubject.set_value(lastsearchFilter.subject);
            this._txtMetadataValue.set_value(lastsearchFilter.metadataValue);
            if (lastsearchFilter.idContainer) {
                let selectedItem: Telerik.Web.UI.DropDownListItem = this._rdlContainer.findItemByValue(lastsearchFilter.idContainer.toString());
                selectedItem.set_selected(true);
                this._rdlContainer.trackChanges();
            }
        }
    }

    loadContainers = () => {
        this._containerService.getAnyDossierAuthorizedContainers((data: any) => {
            if (!data) return;
            let containers: ContainerModel[] = <ContainerModel[]>data;
            this.addContainers(containers, this._rdlContainer);
            this.setLastSearchFilter();
            this._loadingPanel.hide(this.searchTableId);
            this._btnSearch.set_enabled(true);
            this._btnClean.set_enabled(true);
        },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.searchTableId);
                this._btnSearch.set_enabled(false);
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    cleanSearchFilters = () => {
        this._txtYear.set_value('');
        this._txtNumber.set_value('');
        this._txtSubject.set_value('');
        let selectedContainer: Telerik.Web.UI.DropDownListItem = this._rdlContainer.get_selectedItem();
        if (selectedContainer) {
            selectedContainer.set_selected(false);
        }
        sessionStorage.removeItem("DossierSearch");
    }
}
export = DossierRicerca;