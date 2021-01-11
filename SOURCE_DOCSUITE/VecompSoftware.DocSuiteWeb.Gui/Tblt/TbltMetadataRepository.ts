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
import SessionStorageKeysHelper = require('App/Helpers/SessionStorageKeysHelper');

class TbltMetadataRepository extends MetadataRepositoryBase {

    uscMetadataRepositorySummaryId: string;
    ajaxLoadingPanelId: string;
    pnlRtvMetadataId: string;
    uscNotificationId: string;
    uscMetadataRepositoryId: string;
    pageContentId: string;
    folderToolbarId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _serviceConfigurationHelper: ServiceConfigurationHelper;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _folderToolbar: Telerik.Web.UI.RadToolBar;

    private _metadataRepositoryId: string;


    private static CREATE_OPTION = "create";
    private static MODIFY_OPTION = "modify";
    private static DELETE_OPTION = "delete";
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

    folderToolBar_onClick = (sender: Telerik.Web.UI.RadToolBar, args: Telerik.Web.UI.RadToolBarEventArgs) => {
        switch (args.get_item().get_value()) {
            case TbltMetadataRepository.CREATE_OPTION: {
                window.location.href = "../Tblt/TbltMetadataRepositoryDesigner.aspx?Type=Comm";
                break;
            }
            case TbltMetadataRepository.MODIFY_OPTION: {
                this._metadataRepositoryId = sessionStorage.getItem(SessionStorageKeysHelper.SESSION_KEY_UNIQUEID_METADATA_REPOSITORY);
                if (!!this._metadataRepositoryId) {
                    window.location.href = "../Tblt/TbltMetadataRepositoryDesigner.aspx?Type=Comm&IdMetadtaRepository=".concat(this._metadataRepositoryId, "&IsEditPage=True");
                }
                break;
            }
        }
    }

    /**
     * Inizializzazione
     */
    initialize() {
        super.initialize();
        this._folderToolbar = <Telerik.Web.UI.RadToolBar>$find(this.folderToolbarId);
        this._folderToolbar.add_buttonClicked(this.folderToolBar_onClick);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);

        $("#".concat(this.uscMetadataRepositoryId)).on(UscMetadataRepository.ON_ROOT_NODE_CLICKED, (args, data) => {

            let uscMetadaRepository: UscMetadataRepositorySummary = <UscMetadataRepositorySummary>$("#".concat(this.uscMetadataRepositorySummaryId)).data();
            if (!jQuery.isEmptyObject(uscMetadaRepository)) {
                uscMetadaRepository.clearPage();
            }
            this._folderToolbar.findItemByValue(TbltMetadataRepository.CREATE_OPTION).set_enabled(true);
            this._folderToolbar.findItemByValue(TbltMetadataRepository.MODIFY_OPTION).set_enabled(false);
        });

        $("#".concat(this.uscMetadataRepositoryId)).on(UscMetadataRepository.ON_NODE_CLICKED, (args, data) => {

            let uscMetadaRepository: UscMetadataRepositorySummary = <UscMetadataRepositorySummary>$("#".concat(this.uscMetadataRepositorySummaryId)).data();
            if (!jQuery.isEmptyObject(uscMetadaRepository)) {
                sessionStorage.setItem(SessionStorageKeysHelper.SESSION_KEY_UNIQUEID_METADATA_REPOSITORY, data);
                uscMetadaRepository.loadMetadataRepository(data);
            }
            this._folderToolbar.findItemByValue(TbltMetadataRepository.CREATE_OPTION).set_enabled(false);
            this._folderToolbar.findItemByValue(TbltMetadataRepository.MODIFY_OPTION).set_enabled(true);
        });

    }

    /*
     * ---------------------------- Methods ---------------------------
     */

}
export = TbltMetadataRepository;