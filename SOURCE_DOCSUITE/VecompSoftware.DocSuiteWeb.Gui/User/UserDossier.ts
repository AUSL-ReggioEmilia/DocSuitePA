import DossierType = require("App/Models/Dossiers/DossierType");
import EnumHelper = require("App/Helpers/EnumHelper");
import DossierSearchFilterDTO = require("App/DTOs/DossierSearchFilterDTO");
import DossierStatus = require("App/Models/Dossiers/DossierStatus");
import DossierService = require("App/Services/Dossiers/DossierService");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import UscDossierGrid = require('UserControl/uscDossierGrid');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import UscErrorNotification = require('UserControl/uscErrorNotification');

class UserDossier {

    // STATIC
    private static DOSSIER_SERVICE_NAME = "Dossier";
    private static ACTION_DOP = "DOP";

    //PAGE DATA
    public rdpDateFromId: string;
    public rdpDateToId: string;
    public btnSearchId: string;
    public ddlDossierTypeId: string;
    public desktopDayDiff: number;
    public uscDossierGridId: string;
    public uscNotificationId: string;
    public actionType: string;

    //PAGE CONTROLS
    private _rdpDateFrom: Telerik.Web.UI.RadDatePicker;
    private _rdpDateTo: Telerik.Web.UI.RadDatePicker;
    private _btnSearch: Telerik.Web.UI.RadButton;
    private _ddlDossierType: Telerik.Web.UI.RadDropDownList;

    //PRIVATE
    private _enumHelper: EnumHelper;
    private _serviceConfiguration: ServiceConfiguration;
    private service: DossierService;
    private _currentDossierType: DossierType = DossierType.Procedure;

    constructor(serviceConfiguration: ServiceConfiguration[]) {
        this._serviceConfiguration = ServiceConfigurationHelper.getService(serviceConfiguration, UserDossier.DOSSIER_SERVICE_NAME);
    }

    public initialize(): void {
        this._enumHelper = new EnumHelper();
        this.service = new DossierService(this._serviceConfiguration);

        this._rdpDateFrom = <Telerik.Web.UI.RadDatePicker>$find(this.rdpDateFromId);
        this._rdpDateTo = <Telerik.Web.UI.RadDatePicker>$find(this.rdpDateToId);
        this._btnSearch = <Telerik.Web.UI.RadButton>$find(this.btnSearchId);
        this._ddlDossierType = <Telerik.Web.UI.RadDropDownList>$find(this.ddlDossierTypeId);

        this.initializeDropDownListItems();
        this.initializeSearchDateRanges();
        this.initializeMouseEvents();
        this.initializePageEvents();
        this.initializeDossierTypeChangeEvent();
        this.initializeResultsByActionType(this.actionType);
    }

    /**
     * When the page loads, and if the query parameter Action=DOP we load results for Procedure and for diffDays
     * @param action
     */
    private initializeResultsByActionType(action: string): void {
        if (action === UserDossier.ACTION_DOP) {
            let uscDossierGrid: UscDossierGrid = <UscDossierGrid>$(`#${this.uscDossierGridId}`).data();
            this._currentDossierType = DossierType.Procedure;
            this.loadDossierResults(uscDossierGrid, 0);
        }
    }

    /**
     * When the dossier type changes we change the currentDossierType. On next search button click, the current dossier type is searched for 
     **/
    private initializeDossierTypeChangeEvent(): void {
        this._ddlDossierType.add_selectedIndexChanged((sender, args) => {
            let uscDossierGrid: UscDossierGrid = <UscDossierGrid>$(`#${this.uscDossierGridId}`).data();
            let selectedDropDown: Telerik.Web.UI.DropDownListItem = this._ddlDossierType.getItem(args.get_index());
            this._currentDossierType = parseInt(selectedDropDown.get_value(), 0);
        });
    }

    /**
     * The grid is paginated. When page changes we fetch next results 
     **/
    private initializePageEvents(): void {
        $(`#${this.uscDossierGridId}`).bind(UscDossierGrid.PAGE_CHANGED_EVENT, (args) => {
            let uscDossierGrid: UscDossierGrid = <UscDossierGrid>$(`#${this.uscDossierGridId}`).data();
            if (!jQuery.isEmptyObject(uscDossierGrid)) {
                this.pageChange(uscDossierGrid);
            }
        });
    }

    /**
     * The grid is paginated. When page changes we fetch next results 
     **/
    private pageChange(uscDossierGrid: UscDossierGrid) {
        //this._loadingPanel.show(this.uscDossierGridId);
        let skip = uscDossierGrid.getGridCurrentPageIndex() * uscDossierGrid.getGridPageSize();
        this.loadDossierResults(uscDossierGrid, skip);
    }

    /**
     * When search button is clicked we fetch results 
     **/
    private initializeMouseEvents(): void {
        this._btnSearch.add_clicked(this.btnSearch_onClick);
    }

    /**
     * Initialize the combo box with the types of dossiers
     **/
    private initializeDropDownListItems(): void {
        this.addDropDownListItem(DossierType.Person);
        this.addDropDownListItem(DossierType.PhysicalObject);
        this.addDropDownListItem(DossierType.Procedure);
        this.addDropDownListItem(DossierType.Process);
    }

    private addDropDownListItem(type: DossierType, selected: boolean = false): void {
        const listItem = new Telerik.Web.UI.DropDownListItem();
        listItem.set_text(this._enumHelper.getDossierTypeDescription(DossierType[type]));
        listItem.set_value(type.toString()); //number as string
        this._ddlDossierType.get_items().add(listItem);

        if (type === this._currentDossierType) {
            listItem.set_selected(true);
        }
    }

    private initializeSearchDateRanges(): void {
        var today = new Date();
        var beforeToday = new Date();
        beforeToday.setDate(beforeToday.getDate() - this.desktopDayDiff);

        this._rdpDateFrom.set_selectedDate(beforeToday);
        this._rdpDateTo.set_selectedDate(today);
    }

    /**
     * When search button is clicked we fetch results
     **/
    private btnSearch_onClick = (sender: any, args: Telerik.Web.UI.ButtonEventArgs) => {
        let uscDossierGrid: UscDossierGrid = <UscDossierGrid>$(`#${this.uscDossierGridId}`).data();
        this.loadDossierResults(uscDossierGrid, 0);
    }

    /**
     * Loading results for the current search settings. Search settings take data from the form and class properties
     * @param uscDossierGrid the grid for setting the results
     * @param skip skipping a number of results
     */
    private loadDossierResults(uscDossierGrid: UscDossierGrid, skip: number): void {
        let top: number = skip + uscDossierGrid.getGridPageSize();
        const searchDTO: DossierSearchFilterDTO = new DossierSearchFilterDTO();

        let fromDateFilter: string = null;
        if (this._rdpDateFrom.get_selectedDate()) {
            fromDateFilter = moment(this._rdpDateFrom.get_selectedDate()).format("YYYY-MM-DD");
        }

        let toDateFilter: string = null;
        if (this._rdpDateTo.get_selectedDate()) {
          
            toDateFilter = moment(this._rdpDateTo.get_selectedDate()).format("YYYY-MM-DD");
        }
        
        searchDTO.StartDateFrom = fromDateFilter;
        searchDTO.StartDateTo = toDateFilter;

        searchDTO.Status = DossierStatus[DossierStatus.Open]
        searchDTO.DossierType = DossierType[this._currentDossierType]

        searchDTO.Skip = skip;
        searchDTO.Top = top;

        this.service.getAuthorizedDossiers(searchDTO, (data) => {
            if (!data) return;
            uscDossierGrid.setDataSource(data);

            this.service.countAuthorizedDossiers(searchDTO,
                (data) => {
                    if (data == undefined) return;
                    uscDossierGrid.setItemCount(data);
                },
                (exception: ExceptionDTO) => {
                    $(`#${this.uscDossierGridId}`).hide();
                    console.log(exception)
                    this.showNotificationException(this.uscNotificationId, exception);
                }
            );
        },
            (exception: ExceptionDTO) => {
                $(`#${this.uscDossierGridId}`).hide();
                console.log(exception)
                this.showNotificationException(this.uscNotificationId, exception);
            });
    }

    private showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${uscNotificationId}`).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(uscNotificationId, customMessage)
        }
    }

    private showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${uscNotificationId}`).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage);
        }
    }
}

export = UserDossier;