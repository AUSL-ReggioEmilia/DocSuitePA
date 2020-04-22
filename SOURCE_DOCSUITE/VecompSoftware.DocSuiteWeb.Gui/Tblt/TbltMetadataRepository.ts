import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import MetadataRepositoryBase = require('Tblt/MetadataRepositoryBase');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import UscMetadataRepositoryDesigner = require('UserControl/uscMetadataRepositoryDesigner');
import UscMetadataRepository = require('UserControl/uscMetadataRepository');
import UscMetadataRepositorySummary = require('UserControl/uscMetadataRepositorySummary');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import MetadataRepositoryService = require('App/Services/Commons/MetadataRepositoryService');
import UscErrorNotification = require('UserControl/uscErrorNotification');

class TbltMetadataRepository extends MetadataRepositoryBase {

    btnAggiungiId: string;
    btnModificaId: string;
    uscMetadataRepositorySummaryId: string;
    ajaxLoadingPanelId: string;
    pnlRtvMetadataId: string;
    uscNotificationId: string;
    uscMetadataRepositoryId: string;
    pageContentId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _serviceConfigurationHelper: ServiceConfigurationHelper;
    private _btnAggiungi: Telerik.Web.UI.RadButton;
    private _btnModifica: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;

    private _metadataRepositoryId: string;

    /**
     * Costruttore
     * @param serviceConfigurations
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, MetadataRepositoryBase.METADATA_REPOSITORY_NAME));
        this._serviceConfigurations = serviceConfigurations;
    }

    /*
     * ----------------------------- Events ---------------------------
     */

    /**
     *  Evento scatenato al click del bottone Aggiungi
     * @param sender
     * @param args
     */
    btnAggiungi_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonCancelEventArgs) => {

        window.location.href = "../Tblt/TbltMetadataRepositoryDesigner.aspx?Type=Comm";

    }

    /**
     * Evento scatenato al clik del bottone Modifica
     * @param sender
     * @param args
     */
    btnModifica_OnClick(sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonCancelEventArgs) {

        this._metadataRepositoryId = sessionStorage.getItem("UniqueIdMetadataRepository");
        if (!!this._metadataRepositoryId) {
            window.location.href = "../Tblt/TbltMetadataRepositoryDesigner.aspx?Type=Comm&IdMetadtaRepository=".concat(this._metadataRepositoryId, "&IsEditPage=True");
        }
    }

    /**
     * Inizializzazione
     */
    initialize() {
        super.initialize();
        this._btnAggiungi = <Telerik.Web.UI.RadButton>$find(this.btnAggiungiId);
        this._btnAggiungi.add_clicking(this.btnAggiungi_OnClick);
        this._btnModifica = <Telerik.Web.UI.RadButton>$find(this.btnModificaId);
        this._btnModifica.add_clicking(this.btnModifica_OnClick);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);

        $("#".concat(this.uscMetadataRepositoryId)).on(UscMetadataRepository.ON_ROOT_NODE_CLICKED, (args, data) => {

            let uscMetadaRepository: UscMetadataRepositorySummary = <UscMetadataRepositorySummary>$("#".concat(this.uscMetadataRepositorySummaryId)).data();
            if (!jQuery.isEmptyObject(uscMetadaRepository)) {
                uscMetadaRepository.clearPage();
            }
            this._btnAggiungi.set_enabled(true);
            this._btnModifica.set_enabled(false);
        });

        $("#".concat(this.uscMetadataRepositoryId)).on(UscMetadataRepository.ON_NODE_CLICKED, (args, data) => {

            let uscMetadaRepository: UscMetadataRepositorySummary = <UscMetadataRepositorySummary>$("#".concat(this.uscMetadataRepositorySummaryId)).data();
            if (!jQuery.isEmptyObject(uscMetadaRepository)) {
                sessionStorage.setItem("UniqueIdMetadataRepository", data);
                uscMetadaRepository.loadMetadataRepository(data);
            }
            this._btnAggiungi.set_enabled(false);
            this._btnModifica.set_enabled(true);
        });

    }

    /*
     * ---------------------------- Methods ---------------------------
     */

}
export = TbltMetadataRepository;