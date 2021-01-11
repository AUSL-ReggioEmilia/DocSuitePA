import DossierSearchFilterDTO = require("App/DTOs/DossierSearchFilterDTO");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import DossierBase = require('Dossiers/DossierBase');
import ContainerService = require('App/Services/Commons/ContainerService');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscMetadataRepositorySel = require('UserControl/uscMetadataRepositorySel');
import MetadataFinderViewModel = require("App/ViewModels/Metadata/MetadataFinderViewModel");
import DossierType = require("App/Models/Dossiers/DossierType");
import EnumHelper = require('App/Helpers/EnumHelper');
import uscCategoryRest = require('UserControl/uscCategoryRest');
import CategoryModel = require("App/Models/Commons/CategoryModel");
import DossierStatus = require("App/Models/Dossiers/DossierStatus");
import SessionStorageKeysHelper = require("App/Helpers/SessionStorageKeysHelper");

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
    hasTxtYearDefaultValue: boolean;
    uscMetadataRepositorySelId: string;
    metadataRepositoryEnabled: boolean;
    rowMetadataRepositoryId: string;
    currentTenantId: string;
    metadataTableId: string;
    isWindowPopupEnable: boolean;
    uscCategoryRestId: string;
    rcbDossierTypeId: string;
    rblDossierStatusId: string;
    dossierStatusEnabled: boolean;
    dossierStatusRowId: string;

    private _btnSearch: Telerik.Web.UI.RadButton;
    private _txtYear: Telerik.Web.UI.RadTextBox;
    private _txtNumber: Telerik.Web.UI.RadTextBox;
    private _txtSubject: Telerik.Web.UI.RadTextBox;
    private _txtNote: Telerik.Web.UI.RadTextBox;
    private _rdlContainer: Telerik.Web.UI.RadDropDownList;
    private _rdpStartDateFrom: Telerik.Web.UI.RadDatePicker;
    private _rdpStartDateTo: Telerik.Web.UI.RadDatePicker;
    private _rdpEndDateFrom: Telerik.Web.UI.RadDatePicker;
    private _rdpEndDateTo: Telerik.Web.UI.RadDatePicker;
    private _serviceConfigurations: ServiceConfiguration[];
    private _containerService: ContainerService;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _btnClean: Telerik.Web.UI.RadButton;
    private _rowMetadataRepository: JQuery;
    private _rcbDossierType: Telerik.Web.UI.RadComboBox;
    private _enumHelper: EnumHelper;

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
        this._enumHelper = new EnumHelper();
        this._btnSearch = <Telerik.Web.UI.RadButton>$find(this.btnSearchId);
        this._btnSearch.add_clicking(this.btnSearch_onClick);
        this._btnClean = <Telerik.Web.UI.RadButton>$find(this.btnCleanId);
        this._btnClean.add_clicking(this.btnClean_onClick);
        this._txtYear = <Telerik.Web.UI.RadTextBox>$find(this.txtYearId);
        this._txtNumber = <Telerik.Web.UI.RadTextBox>$find(this.txtNumberId);
        this._txtSubject = <Telerik.Web.UI.RadTextBox>$find(this.txtSubjectId);
        this._txtNote = <Telerik.Web.UI.RadTextBox>$find(this.txtNoteId);
        this._rdlContainer = <Telerik.Web.UI.RadDropDownList>$find(this.rdlContainerId);
        this._rdpEndDateFrom = <Telerik.Web.UI.RadDatePicker>$find(this.rdpEndDateFromId);
        this._rdpEndDateTo = <Telerik.Web.UI.RadDatePicker>$find(this.rdpEndDateToId);
        this._rdpStartDateFrom = <Telerik.Web.UI.RadDatePicker>$find(this.rdpStartDateFromId);
        this._rdpStartDateTo = <Telerik.Web.UI.RadDatePicker>$find(this.rdpStartDateToId);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._rcbDossierType = <Telerik.Web.UI.RadComboBox>$find(this.rcbDossierTypeId);

        let containerConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Container");
        this._containerService = new ContainerService(containerConfiguration);
        this._loadingPanel.show(this.searchTableId);
        this._btnSearch.set_enabled(false);
        this._btnClean.set_enabled(false);
        this._rowMetadataRepository = $("#".concat(this.rowMetadataRepositoryId));
        $(`#${this.metadataTableId}`).hide();
        this.loadContainers();
        this.populateDossierTypeComboBox();

        if (this.metadataRepositoryEnabled) {
            $(`#${this.metadataTableId}`).show();
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

    btnSearch_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {

        let searchDTO: DossierSearchFilterDTO = new DossierSearchFilterDTO();
        let yearFilter: string = this._txtYear.get_value();
        let numberFilter: string = this._txtNumber.get_value();
        let subjectFilter: string = this._txtSubject.get_value();
        let noteFilter: string = this._txtNote.get_value();
        let startDateFromFilter: string = null;
        if (this._rdpStartDateFrom.get_selectedDate()) {
            startDateFromFilter = this._rdpStartDateFrom.get_selectedDate().format("yyyy-MM-dd").toString();
        }
        let startDateToFilter: string = null;
        if (this._rdpStartDateTo.get_selectedDate()) {
            startDateToFilter = this._rdpStartDateTo.get_selectedDate().format("yyyy-MM-dd").toString();
        }
        let endDateFromFilter: string = null;
        if (this._rdpEndDateFrom.get_selectedDate()) {
            endDateFromFilter = this._rdpEndDateFrom.get_selectedDate().format("yyyy-MM-dd").toString();
        }
        let endDateToFilter: string = null;
        if (this._rdpEndDateTo.get_selectedDate()) {
            endDateToFilter = this._rdpEndDateTo.get_selectedDate().format("yyyy-MM-dd").toString();
        }

        let containerFilter: string = "";
        if (this._rdlContainer.get_selectedItem()) {
            containerFilter = this._rdlContainer.get_selectedItem().get_value();
        }

        let metadataRepositoryId: string = "";
        let uscMetadataRepositorySel: UscMetadataRepositorySel = <UscMetadataRepositorySel>$("#".concat(this.uscMetadataRepositorySelId)).data();
        if (!jQuery.isEmptyObject(uscMetadataRepositorySel) && this.metadataRepositoryEnabled) {
            metadataRepositoryId = uscMetadataRepositorySel.getSelectedMetadataRepositoryId();
            let [metadataValue, metadataFinderModels, metadataValuesAreValid]: [string, MetadataFinderViewModel[], boolean] = uscMetadataRepositorySel.getMetadataFilterValues();
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

        let uscCategoryRest: uscCategoryRest = <uscCategoryRest>$(`#${this.uscCategoryRestId}`).data();
        let category: CategoryModel = uscCategoryRest.getSelectedCategory();
        searchDTO.IdCategory = category ? category.EntityShortId : null;

        let dossierType: string = this._rcbDossierType.get_selectedItem().get_value();
        searchDTO.DossierType = dossierType ? dossierType : null;

        if (!this.dossierStatusEnabled) {
            searchDTO.Status = DossierStatus.Open.toString();
        }
        else {
            let checkedStatus: string = $(`#${this.rblDossierStatusId} input:checked`).val();
            searchDTO.Status = checkedStatus !== "All" ? checkedStatus : null;
        }

        sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_DOSSIER_SEARCH, JSON.stringify(searchDTO));

        let url: string = "../Dossiers/DossierRisultati.aspx?Type=Dossier";
        if (this.isWindowPopupEnable) {
            url = `${url}&IsWindowPopupEnable=True`;
        }
        if (this.dossierStatusEnabled) {
            url = `${url}&DossierStatusEnabled=True`;
        }
        window.location.href = url;

    }

    /**
   * Evento scatenato al click del pulsante di svuota ricerca
   * @param sender
   * @param args
   */

    btnClean_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.cleanSearchFilters();
    }


    /**
    *------------------------- Methods -----------------------------
    */

    setLastSearchFilter = () => {
        let dossierLastSearch: string = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOSSIER_SEARCH);
        if (this.hasTxtYearDefaultValue == true) {
            this._txtYear.set_value(new Date().getFullYear().toString());
        }
        if (dossierLastSearch) {
            let lastsearchFilter: DossierSearchFilterDTO = <DossierSearchFilterDTO>JSON.parse(dossierLastSearch);
            this._txtYear.set_value(lastsearchFilter.Year ? lastsearchFilter.Year.toString() : null);
            this._txtNumber.set_value(lastsearchFilter.Number ? lastsearchFilter.Number.toString() : null);
            this._txtSubject.set_value(lastsearchFilter.Subject);
            if (lastsearchFilter.IdContainer) {
                let selectedItem: Telerik.Web.UI.DropDownListItem = this._rdlContainer.findItemByValue(lastsearchFilter.IdContainer.toString());
                selectedItem.set_selected(true);
                this._rdlContainer.trackChanges();
            }
        }
    }

    loadContainers = () => {
        this._containerService.getAnyDossierAuthorizedContainers(this.currentTenantId,
            (data: any) => {
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

        let uscCategoryRest: uscCategoryRest = <uscCategoryRest>$(`#${this.uscCategoryRestId}`).data();
        uscCategoryRest.clearTree();

        this._rcbDossierType.get_items().getItem(0).select();

        let openedCheckbox: any = $(`#${this.rblDossierStatusId} input:radio`)[1];
        openedCheckbox.checked = "checked";

        sessionStorage.removeItem(SessionStorageKeysHelper.SESSION_KEY_DOSSIER_SEARCH);
    }

    private populateDossierTypeComboBox(): void {
        let rcbItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
        rcbItem.set_text("");
        this._rcbDossierType.get_items().add(rcbItem);
        for (let dossierType in DossierType) {
            if (typeof DossierType[dossierType] === 'string') {
                let rcbItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                rcbItem.set_text(this._enumHelper.getDossierTypeDescription(DossierType[dossierType]));
                rcbItem.set_value(DossierType[dossierType]);
                this._rcbDossierType.get_items().add(rcbItem);
            }
        }
        this._rcbDossierType.get_items().getItem(0).select();
    }
}
export = DossierRicerca;