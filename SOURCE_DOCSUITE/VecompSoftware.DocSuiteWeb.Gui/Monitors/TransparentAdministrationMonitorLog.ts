import TransparentAdministrationMonitorLogBase = require('Monitors/TransparentAdministrationMonitorLogBase');
import TransparentAdministrationMonitorLogSearchFilterDTO = require('App/DTOs/TransparentAdministrationMonitorLogSearchFilterDTO');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import UscAmmTraspMonitorLogGrid = require('UserControl/uscAmmTraspMonitorLogGrid');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import TransparentAdministrationMonitorLogGridViewModel = require('App/ViewModels/Monitors/TransparentAdministrationMonitorLogGridViewModel');
import ContainerService = require('App/Services/Commons/ContainerService');
import LocationTypeEnum = require('App/Models/Commons/LocationTypeEnum');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import RoleModel = require('App/Models/Commons/RoleModel');
import UscSettori = require('UserControl/uscSettori');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');

class TransparentAdministrationMonitorLog extends TransparentAdministrationMonitorLogBase {

    uscAmmTraspMonitorLogGridId: string;
    ajaxLoadingPanelId: string;
    uscNotificationId: string;
    dpStartDateFromId: string;
    dpEndDateFromId: string;
    btnSearchId: string;
    btnCleanId: string;
    gridResult: TransparentAdministrationMonitorLogGridViewModel[];
    uscAmmTraspMonitorLogId: string;
    transparentAdministrationMonitorLogItemsId: string;
    txtUsernameId: string;
    cmbDocumentTypeId: string;
    cmbContainerId: string;
    uscOwnerRoleId: string;
    maxNumberElements: string;

    private _dpStartDateFrom: Telerik.Web.UI.RadDatePicker;
    private _dpEndDateFrom: Telerik.Web.UI.RadDatePicker;
    private _btnSearch: Telerik.Web.UI.RadButton;
    private _btnClean: Telerik.Web.UI.RadButton;
    private _txtTransparentAdministrationMonitorLogItems: HTMLInputElement;
    private _txtUsername: Telerik.Web.UI.RadTextBox;
    private _cmbDocumentType: Telerik.Web.UI.RadComboBox;
    private _cmbContainer: Telerik.Web.UI.RadComboBox;
    private _containerService: ContainerService;

    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _serviceConfigurations: ServiceConfiguration[];

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, TransparentAdministrationMonitorLogBase.TransparentAdministrationMonitorLog_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        let containerConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Container");
        this._containerService = new ContainerService(containerConfiguration);
        $(document).ready(() => {

        });
    }

    initialize() {
        super.initialize();
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._loadingPanel.show(this.uscAmmTraspMonitorLogGridId);
        $("#".concat(this.uscAmmTraspMonitorLogGridId)).bind(UscAmmTraspMonitorLogGrid.LOADED_EVENT, () => {
            this.loadAmmTraspMonitorLogGrid();
        });
        this.loadAmmTraspMonitorLogGrid();

        $("#".concat(this.uscAmmTraspMonitorLogGridId)).bind(UscAmmTraspMonitorLogGrid.PAGE_CHANGED_EVENT, (args) => {
            let uscAmmTraspMonitorLogGrid: UscAmmTraspMonitorLogGrid = <UscAmmTraspMonitorLogGrid>$("#".concat(this.uscAmmTraspMonitorLogGridId)).data();
            if (!jQuery.isEmptyObject(uscAmmTraspMonitorLogGrid)) {
                this.pageChange(uscAmmTraspMonitorLogGrid);
            }
        });

        this._dpStartDateFrom = <Telerik.Web.UI.RadDatePicker>$find(this.dpStartDateFromId);
        this._dpEndDateFrom = <Telerik.Web.UI.RadDatePicker>$find(this.dpEndDateFromId);
        this._btnSearch = <Telerik.Web.UI.RadButton>$find(this.btnSearchId);
        this._btnSearch.add_clicking(this.btnSearch_onClick);
        this._btnClean = <Telerik.Web.UI.RadButton>$find(this.btnCleanId);
        this._btnClean.add_clicking(this.btnClean_onClick);
        this._txtTransparentAdministrationMonitorLogItems = <HTMLInputElement>document.getElementById(this.transparentAdministrationMonitorLogItemsId);
        this._txtUsername = <Telerik.Web.UI.RadTextBox>$find(this.txtUsernameId);
        this._cmbDocumentType = <Telerik.Web.UI.RadComboBox>$find(this.cmbDocumentTypeId);
        this._cmbContainer = <Telerik.Web.UI.RadComboBox>$find(this.cmbContainerId);
        this._cmbDocumentType.add_selectedIndexChanged(this._cmbDocumentType_OnSelectedIndexChanged);

        this.populateDocumentTypeComboBox();
        this._cmbContainer.add_selectedIndexChanged(this._cmbContainer_OnSelectedIndexChanged);
        this._cmbContainer.add_itemsRequested(this.cmbContainer_OnClientItemsRequested);
        let scrollContainer: JQuery = $(this._cmbContainer.get_dropDownElement()).find('div.rcbScroll');
        $(scrollContainer).scroll(this.cmdContainer_onScroll);

    }

    private loadAmmTraspMonitorLogGrid() {
        let uscAmmTraspMonitorLogGrid: UscAmmTraspMonitorLogGrid = <UscAmmTraspMonitorLogGrid>$("#".concat(this.uscAmmTraspMonitorLogGridId)).data();
        if (!jQuery.isEmptyObject(uscAmmTraspMonitorLogGrid)) {
            this.loadResults(uscAmmTraspMonitorLogGrid, 0);
        }
    }

    private pageChange(uscAmmTraspMonitorLogGrid: UscAmmTraspMonitorLogGrid) {
        this._loadingPanel.show(this.uscAmmTraspMonitorLogGridId);
        let skip = uscAmmTraspMonitorLogGrid.getGridCurrentPageIndex() * uscAmmTraspMonitorLogGrid.getGridPageSize();
        this.loadResults(uscAmmTraspMonitorLogGrid, skip);
    }

    private loadResults(uscAmmTraspMonitorLogGrid: UscAmmTraspMonitorLogGrid, skip: number) {
        let startDateFromFilter: string = "";
        if (this._dpStartDateFrom !== undefined && this._dpStartDateFrom.get_selectedDate()) {
            startDateFromFilter = this._dpStartDateFrom.get_selectedDate().format("yyyyMMddHHmmss").toString();
        }
        let endDateFromFilter: string = "";
        if (this._dpEndDateFrom !== undefined && this._dpEndDateFrom.get_selectedDate()) {
            endDateFromFilter = this._dpEndDateFrom.get_selectedDate().format("yyyyMMddHHmmss").toString();
        }
        let usernameFromFilter: string = "";
        if (this._txtUsername !== undefined && this._txtUsername.get_textBoxValue() !== "") {
            usernameFromFilter = this._txtUsername.get_textBoxValue();
        }
        let documentTypeFromFilter: string = "";
        if (this._cmbDocumentType != undefined && this._cmbDocumentType.get_selectedItem() !== null) {
            documentTypeFromFilter = this._cmbDocumentType.get_selectedItem().get_value();
        }
        let containerFromFilter: string = "";
        if (this._cmbContainer != undefined && this._cmbContainer.get_selectedItem() !== null) {
            containerFromFilter = this._cmbContainer.get_selectedItem().get_value();
        }

        let idRole: number;
        let role: RoleModel = this.getUscRole();
        if (role) {
            idRole = role.EntityShortId;
        }

        let searchDTO: TransparentAdministrationMonitorLogSearchFilterDTO = new TransparentAdministrationMonitorLogSearchFilterDTO();
        searchDTO.dateFrom = startDateFromFilter;
        searchDTO.dateTo = endDateFromFilter;
        searchDTO.username = usernameFromFilter;
        searchDTO.documentType = documentTypeFromFilter;
        searchDTO.container = containerFromFilter;
        searchDTO.idRole = idRole;
        let top: number = skip + uscAmmTraspMonitorLogGrid.getGridPageSize();
        if (searchDTO.dateFrom !== "" || searchDTO.dateTo !== "")
            this.service.getTransparentAdministrationMonitorLogs(searchDTO,
                (data) => {
                    if (!data) return;
                    this.gridResult = data;
                    uscAmmTraspMonitorLogGrid.setDataSource(this.gridResult);
                    for (let item of this.gridResult) {
                        item.DocumentUnitSummaryUrl = "";
                    }
                    this._txtTransparentAdministrationMonitorLogItems.value = JSON.stringify(data);
                    this._loadingPanel.hide(this.uscAmmTraspMonitorLogGridId);
                },
                (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.uscAmmTraspMonitorLogGridId);
                    $("#".concat(this.uscAmmTraspMonitorLogGridId)).hide();
                    this.showNotificationException(this.uscNotificationId, exception);
                });
        else
            this._loadingPanel.hide(this.uscAmmTraspMonitorLogGridId);
    }

    btnSearch_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        let uscAmmTraspMonitorLogGrid: UscAmmTraspMonitorLogGrid = <UscAmmTraspMonitorLogGrid>$("#".concat(this.uscAmmTraspMonitorLogGridId)).data();
        this.loadResults(uscAmmTraspMonitorLogGrid, 0);
    }

    btnClean_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.cleanSearchFilters();
    }

    cleanSearchFilters = () => {
        this._dpStartDateFrom.clear();
        this._dpEndDateFrom.clear();
        this._cmbDocumentType.clearSelection();
        this._cmbContainer.clearSelection();
        this._txtUsername.set_value("");

    }

    /**
    * Evento scatenato allo scrolling della RadComboBox di selezione fascicoli
    * @param args
    */
    cmdContainer_onScroll = (args: JQueryEventObject) => {
        var element = args.target;
        if ((element.scrollHeight - element.scrollTop === element.clientHeight) && element.clientHeight > 0) {
            let filter: string = this._cmbContainer.get_text();
            this.cmbContainer_OnClientItemsRequested(this._cmbContainer, new (<any>Telerik.Web.UI.RadComboBoxRequestEventArgs)(filter, args));
        }
    }
    /**
* Metodo che setta la label MoreResultBoxText della RadComboBox
* @param message
*/
    private setMoreResultBoxText(message: string) {
        this._cmbContainer.get_moreResultsBoxMessageElement().innerText = message;
    }

    private populateDocumentTypeComboBox() {
        let documentTypeItems: any = [
            {
                Name: "",
                Value: ""
            },
            {
                Name: "Protocollo",
                Value: "1"
            },
            {
                Name: "Atti",
                Value: "2"
            },
            {
                Name: "Serie Documentali",
                Value: "4"
            },
            {
                Name: "Archivio",
                Value: "100"
            }
        ];
        let item: Telerik.Web.UI.RadComboBoxItem;
        for (let documentTypeItem of documentTypeItems) {
            item = new Telerik.Web.UI.RadComboBoxItem();
            item.set_text(documentTypeItem.Name);
            item.set_value(documentTypeItem.Value);
            this._cmbDocumentType.get_items().add(item);
        }
    }



    _cmbContainer_OnSelectedIndexChanged = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        let selectedItem: Telerik.Web.UI.RadComboBoxItem = sender.get_selectedItem();
        let domEvent: Sys.UI.DomEvent = args.get_domEvent();
        if (domEvent.type == 'mousedown') {
            return;
        }

        if (domEvent.type == 'click' || (domEvent.type == 'keydown' && (domEvent.keyCode == 9 || domEvent.keyCode == 13))) {
            let emptyItem: Telerik.Web.UI.RadComboBoxItem = sender.findItemByText('');
            sender.clearItems();
            sender.get_items().add(emptyItem);
            sender.get_items().add(selectedItem);
            sender.get_attributes().setAttribute('currentFilter', selectedItem.get_text());
            sender.get_attributes().setAttribute('otherContainerCount', '1');
            this.setMoreResultBoxText("Visualizzati 1 di 1");
        }


    }

    cmbContainer_OnClientItemsRequested = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxRequestEventArgs) => {

        let numberOfItems: number = sender.get_items().get_count();
        if (numberOfItems > 0) {
            //Decremento di 1 perchè la combo visualizza anche un item vuoto
            numberOfItems -= 1;
        }

        let currentOtherContainerItems: number = numberOfItems;
        let currentComboFilter: string = sender.get_attributes().getAttribute('currentFilter');
        let otherContainerCount: number = Number(sender.get_attributes().getAttribute('otherContainerCount'));
        let updating: boolean = sender.get_attributes().getAttribute('updating') == 'true';
        if (isNaN(otherContainerCount) || currentComboFilter != args.get_text()) {
            //Se il valore del filtro è variato re-inizializzo la radcombobox per chiamare le WebAPI
            otherContainerCount = undefined;
        }
        sender.get_attributes().setAttribute('currentFilter', args.get_text());

        this.setMoreResultBoxText('Caricamento...');

        if ((otherContainerCount == undefined || currentOtherContainerItems < otherContainerCount) && !updating) {
            sender.get_attributes().setAttribute('updating', 'true');
            let location: LocationTypeEnum = null;
            if (this._cmbDocumentType.get_selectedItem() != undefined && this._cmbDocumentType.get_selectedItem().get_value() != "") {
                let env: string = this._cmbDocumentType.get_selectedItem().get_value();
                
                if (env == "1") {
                    location = LocationTypeEnum.ProtLocation;
                }
                if (env == "4") {
                    location = LocationTypeEnum.DocumentSeriesLocation;
                }
                //i valori della variabile env sono dati dal popolamento della treeview della tipologia di Documento
                if (env == "100") {
                    location = LocationTypeEnum.UDSLocation;
                }
                if (location == null) {
                    location = LocationTypeEnum.ReslLocation;
                }
            }
            this._containerService.getContainersByEnvironment(args.get_text(),this.maxNumberElements, currentOtherContainerItems, location,
                (data: ODATAResponseModel<ContainerModel>) => {
                    if (!data) return;
                    try {
                        this.addContainers(data.value);
                        let scrollToPosition: boolean = args.get_domEvent() == undefined;
                        if (scrollToPosition) {
                            if (sender.get_items().get_count() > 0) {
                                let scrollContainer: JQuery = $(sender.get_dropDownElement()).find('div.rcbScroll');
                                scrollContainer.scrollTop($(sender.get_items().getItem(currentOtherContainerItems + 1).get_element()).position().top);
                            }
                        }

                        sender.get_attributes().setAttribute('otherContainerCount', data.count.toString());
                        sender.get_attributes().setAttribute('updating', 'false');
                        if (sender.get_items().get_count() > 0) {
                            currentOtherContainerItems = sender.get_items().get_count() - 1;
                        }

                        this.setMoreResultBoxText('Visualizzati '.concat(currentOtherContainerItems.toString(), ' di ', data.count.toString()));
                    }
                    catch (error) {
                        this.showNotificationMessage(this.uscNotificationId, 'Errore in inizializzazione pagina: '.concat(error.message));
                        console.log(JSON.stringify(error));
                    }

                },
                (exception: ExceptionDTO) => {
                    this.showNotificationException(this.uscNotificationId, exception);
                }
            );
        }
    }


    protected addContainers(containers: ContainerModel[]) {
        if (containers.length > 0) {
            this._cmbContainer.beginUpdate();
            if (this._cmbContainer.get_items().get_count() == 0) {
                let item: Telerik.Web.UI.RadComboBoxItem;
                item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text("");
                item.set_value("");
                this._cmbContainer.get_items().add(item);
            }
            $.each(containers, (index, container) => {
                let item = new Telerik.Web.UI.RadComboBoxItem();
                item.set_text(container.Name);
                item.set_value(container.EntityShortId.toString());
                this._cmbContainer.get_items().add(item);
            });
            this._cmbContainer.showDropDown();
            this._cmbContainer.endUpdate();
        }
        else {
            if (this._cmbContainer.get_items().get_count() == 0) {
            }
        }

    }

    _cmbDocumentType_OnSelectedIndexChanged = (sender: Telerik.Web.UI.RadComboBox, args: Telerik.Web.UI.RadComboBoxItemEventArgs) => {
        let domEvent: Sys.UI.DomEvent = args.get_domEvent();
        if (domEvent.type == 'mousedown') {
            return;
        }

        if (domEvent.type == 'click' || (domEvent.type == 'keydown' && (domEvent.keyCode == 9 || domEvent.keyCode == 13))) {
            this._cmbContainer.clearItems();
        }
    }

    private getUscRole = () => {
        let roles: Array<RoleModel> = new Array<RoleModel>();
        let uscRoles: UscSettori = <UscSettori>$("#".concat(this.uscOwnerRoleId)).data();

        if (!jQuery.isEmptyObject(uscRoles)) {
            let source: any = JSON.parse(uscRoles.getRoles());
            if (source != null) {
                for (let s of source) {
                    roles.push(s);
                }
            }
        }

        if (roles.length > 0) {
            return roles[0];
        }

        return null;
    }

}

export = TransparentAdministrationMonitorLog;