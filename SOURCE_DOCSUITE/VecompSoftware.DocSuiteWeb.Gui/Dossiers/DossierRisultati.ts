import DossierBase = require('Dossiers/DossierBase');
import DossierSearchFilterDTO = require("App/DTOs/DossierSearchFilterDTO");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import UscDossierGrid = require('UserControl/uscDossierGrid');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');

class DossierRisultati extends DossierBase {

    uscDossierGridId: string;
    ajaxLoadingPanelId: string;
    uscNotificationId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;

    /**
    * Costruttore
    */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }

    initialize() {
        super.initialize();
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._loadingPanel.show(this.uscDossierGridId);
        $("#".concat(this.uscDossierGridId)).bind(UscDossierGrid.LOADED_EVENT, () => {
            this.loadDossierGrid();
        });
        this.loadDossierGrid();

        $("#".concat(this.uscDossierGridId)).bind(UscDossierGrid.PAGE_CHANGED_EVENT, (args) => {
            let uscDossierGrid: UscDossierGrid = <UscDossierGrid>$("#".concat(this.uscDossierGridId)).data();
            if (!jQuery.isEmptyObject(uscDossierGrid)) {
                this.pageChange(uscDossierGrid);
            }
        });

    }


    private loadDossierGrid() {
        let uscDossierGrid: UscDossierGrid = <UscDossierGrid>$("#".concat(this.uscDossierGridId)).data();
        if (!jQuery.isEmptyObject(uscDossierGrid)) {
            this.loadResults(uscDossierGrid, 0);
        }
    }

    private pageChange(uscDossierGrid: UscDossierGrid) {
        this._loadingPanel.show(this.uscDossierGridId);
        let skip = uscDossierGrid.getGridCurrentPageIndex() * uscDossierGrid.getGridPageSize();
        this.loadResults(uscDossierGrid, skip);
    }

    private loadResults(uscDossierGrid: UscDossierGrid, skip: number) {
        let top: number = skip + uscDossierGrid.getGridPageSize();

        let filter: string = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_DOSSIER_SEARCH);
        let dossierSearchFilter: DossierSearchFilterDTO;
        if (filter) {
            dossierSearchFilter = <DossierSearchFilterDTO>JSON.parse(filter);
        }

        dossierSearchFilter.Skip = skip;
        dossierSearchFilter.Top = top;

        this.service.getAuthorizedDossiers(dossierSearchFilter, (data) => {
            if (!data) return;
            uscDossierGrid.setDataSource(data);

            this.service.countAuthorizedDossiers(dossierSearchFilter,
                (data) => {
                    if (data == undefined) return;
                    uscDossierGrid.setItemCount(data);
                    this._loadingPanel.hide(this.uscDossierGridId);
                },
                (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.uscDossierGridId);
                    $("#".concat(this.uscDossierGridId)).hide();
                    this.showNotificationException(this.uscNotificationId, exception);
                }
            );
        },
        (exception: ExceptionDTO) => {
            this._loadingPanel.hide(this.uscDossierGridId);
            $("#".concat(this.uscDossierGridId)).hide();
            this.showNotificationException(this.uscNotificationId, exception);
        });
    }
}
export = DossierRisultati;