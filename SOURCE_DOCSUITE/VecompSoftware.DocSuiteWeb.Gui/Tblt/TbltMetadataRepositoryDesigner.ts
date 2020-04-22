import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import MetadataRepositoryBase = require('Tblt/MetadataRepositoryBase');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import UscMetadataRepositoryDesigner = require('UserControl/uscMetadataRepositoryDesigner');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import MetadataRepositoryService = require('App/Services/Commons/MetadataRepositoryService');
import UscErrorNotification = require('UserControl/uscErrorNotification');

class TbltMetadataRepositoryDesigner extends MetadataRepositoryBase {

    btnPublishId: string;
    btnDraftId: string;
    uscMetadataRepositoryDesignerId: string;
    ajaxLoadingPanelId: string;
    pageContentId: string;
    uscNotificationId: string;
    metadataRepositoryId: string;
    isEditPage: boolean;

    private _serviceConfigurations: ServiceConfiguration[];
    private _serviceConfigurationHelper: ServiceConfigurationHelper;
    private _btnPublish: Telerik.Web.UI.RadButton;
    private _btnDraft: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;

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
     *  Evento scatenato al click del bottone Pubblica
     * @param sender
     * @param args
     */
    btnPublish_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonCancelEventArgs) => {
        this._loadingPanel.show(this.pageContentId);
        this.setControls(false);
        let item: MetadataRepositoryModel = this.getMetadataRepository();
        if (item == null) {
            this._loadingPanel.hide(this.pageContentId);
            this.setControls(true);
        }
        item.Status = 1;
        this.saveMetadataRepository(item);
    }

    /**
     * Evento scatenato al click del bottone Pubblica
     * @param sender
     * @param args
     */
    btnDraft_OnClick = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.RadButtonCancelEventArgs) => {
        this._loadingPanel.show(this.pageContentId);
        this.setControls(false);
        let item: MetadataRepositoryModel = this.getMetadataRepository();
        if (item == null) {
            this._loadingPanel.hide(this.pageContentId);
            this.setControls(true);
        }
        item.Status = 0;
        this.saveMetadataRepository(item);
    }


    /**
     * Inizializzazione
     */
    initialize() {
        super.initialize();
        this._btnPublish = <Telerik.Web.UI.RadButton>$find(this.btnPublishId);
        this._btnPublish.add_clicking(this.btnPublish_OnClick);
        this._btnDraft = <Telerik.Web.UI.RadButton>$find(this.btnDraftId);
        this._btnDraft.add_clicking(this.btnDraft_OnClick);

        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);

        if (this.metadataRepositoryId && this.isEditPage) {
            this.loadMetadataRepository(this.metadataRepositoryId);
        }

    }

    /*
     * ---------------------------- Methods ---------------------------
     */

    /**
     * Funzione che setta la visibilià dei bottoni
     * @param visibility
     */
    setControls(visibility: boolean) {
        this._btnPublish.set_enabled(visibility);
        this._btnDraft.set_enabled(visibility);
    }

    /**
     * Recupero il MetadataRepositoryModel dallo userControl
     */
    getMetadataRepository(): MetadataRepositoryModel {
        let item: MetadataRepositoryModel;
        let uscMetadataDesigner: UscMetadataRepositoryDesigner = <UscMetadataRepositoryDesigner>$("#".concat(this.uscMetadataRepositoryDesignerId)).data();
        if (!jQuery.isEmptyObject(uscMetadataDesigner)) {
            item = uscMetadataDesigner.prepareModel();
        }
        return item;
    }

    /**
     * Metodo di inserimento del MetadataRepository
     * @param metadataRepository
     */
    insertRepositoryModel(metadataRepository: MetadataRepositoryModel) {
        this._service.insertMetadataRepository(metadataRepository,
            (data: any) => {
                if (data) {
                    this._loadingPanel.hide(this.pageContentId);
                    window.location.href = "../Tblt/TbltMetadataRepository.aspx?Type=Comm";
                }
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this.setControls(true);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    /**
     * aggiorno una metadataRepository esistente
     * @param metadataRepository
     */
    updateRepositoryModel(metadataRepository: MetadataRepositoryModel) {
        this._service.updateMetadataRepository(metadataRepository,
            (data: any) => {
                if (data) {
                    this._loadingPanel.hide(this.pageContentId);
                    window.location.href = "../Tblt/TbltMetadataRepository.aspx?Type=Comm";
                }
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this.setControls(true);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }


    /**
     * carico una metadataRepository esistente
     * @param idMetadataRepository
     */
    loadMetadataRepository(idMetadataRepository: string) {
        this._service.getFullModelById(idMetadataRepository,
            (data: any) => {
                if (data) {
                    this.loadPageItems(data);
                }
            }
        )
    }

    /**
     * inserisco un metadataRepository esistente nel desinger
     * @param metadataRepositoryModel
     */
    loadPageItems(metadataRepositoryModel: MetadataRepositoryModel) {
        let uscMetadataDesigner: UscMetadataRepositoryDesigner = <UscMetadataRepositoryDesigner>$("#".concat(this.uscMetadataRepositoryDesignerId)).data();
        if (!jQuery.isEmptyObject(uscMetadataDesigner)) {
            uscMetadataDesigner.loadModel(metadataRepositoryModel);
        }
    }

    /**
     * inserisco o aggiorno la metadataRepository corrente
     * @param item
     */
    saveMetadataRepository(item: MetadataRepositoryModel) {
        if (this.isEditPage == true) {
            item.UniqueId = sessionStorage.getItem("UniqueIdMetadataRepository");
            this.updateRepositoryModel(item);
        } else {
            this.insertRepositoryModel(item);
        }
    }
}
export = TbltMetadataRepositoryDesigner