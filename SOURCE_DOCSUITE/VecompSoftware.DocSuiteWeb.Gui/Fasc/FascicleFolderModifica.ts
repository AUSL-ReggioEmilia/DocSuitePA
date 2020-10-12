/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import FascBase = require('Fasc/FascBase');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import FascicleFolderService = require('App/Services/Fascicles/FascicleFolderService');
import FascicleFolderModel = require('App/Models/Fascicles/FascicleFolderModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import DossierSummaryFolderViewModel = require('App/ViewModels/Dossiers/DossierSummaryFolderViewModel');
import DossierFolderSummaryModelMapper = require('App/Mappers/Dossiers/DossierFolderSummaryModelMapper');
import AjaxModel = require('App/Models/AjaxModel');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import FascicleModel = require("App/Models/Fascicles/FascicleModel");
import FascicleFolderStatus = require("App/Models/Fascicles/FascicleFolderStatus");
import FascicleFolderTypology = require("App/Models/Fascicles/FascicleFolderTypology");
import FascicleSummaryFolderViewModel = require('App/ViewModels/Fascicles/FascicleSummaryFolderViewModel');
import FascicleFolderSummaryModelMapper = require('App/Mappers/Fascicles/FascicleSummaryFolderViewModelMapper');
import UpdateActionType = require("App/Models/UpdateActionType");
import ValidationExceptionDTO = require('App/DTOs/ValidationExceptionDTO');
import ValidationMessageDTO = require('App/DTOs/ValidationMessageDTO');


class FascicleFolderModifica extends FascBase {

    currentPageId: string;
    btnConfermaId: string;
    txtNameId: string;
    loadingPanelId: string;
    uscNotificationId: string;
    ajaxManagerId: string;
    currentFascicleFolderId: string;
    nameRowId: string;
    sessionUniqueKey: string;
    
    private _serviceConfigurations: ServiceConfiguration[];
    private _btnConferma: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _txtName: Telerik.Web.UI.RadTextBox;
    private _fascicleFolderService: FascicleFolderService;
    private _currentFolder: FascicleSummaryFolderViewModel;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    
    /**
     * Costruttore
     * @param serviceConfigurations
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascBase.FASCICLEFOLDER_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {
        });
    }

    /**
    * ---------------------------- Events ---------------------------------
    */

    /**
    * Evento al click del bottone conferma
    * @param sender
    * @param eventArgs
    * @returns 
    */
    btnConferma_ButtonClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        this._loadingPanel.show(this.currentPageId);
        this._btnConferma.set_enabled(false);
        this.modifyFascicleFolder();

    }

    /**
     * Inizializzazione
     */
    initialize() {
        super.initialize();
        this._btnConferma = <Telerik.Web.UI.RadButton>$find(this.btnConfermaId);
        this._btnConferma.add_clicked(this.btnConferma_ButtonClicked);
        this._txtName = <Telerik.Web.UI.RadTextBox>$find(this.txtNameId);
        this._txtName.focus();
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.loadingPanelId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);

     
        let fascicleFolderConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascBase.FASCICLEFOLDER_TYPE_NAME);
        this._fascicleFolderService = new FascicleFolderService(fascicleFolderConfiguration);
        this._loadingPanel.show(this.currentPageId);
        this._currentFolder = this.getFolderFromSessionStorage(this.currentFascicleFolderId);
        this.setData(this._currentFolder);


        this.bindLoaded();
    }

    /*
    * ---------------------------- Methods ----------------------------
    */

    /**
    * Recupero i dati della cartella dalla SessionStorage
    * @param idFasccileFolder
    */
    getFolderFromSessionStorage = (idFascicleFolder: string) => {
        let fascicleFolder = <FascicleSummaryFolderViewModel>{};
        let result: any = sessionStorage[this.sessionUniqueKey];
        if (result == null) {
            return null;
        }
        let source = JSON.parse(result);
        if (source) {
            fascicleFolder.UniqueId = source.UniqueId;
            fascicleFolder.Name = source.Name;
            fascicleFolder.Status = source.Status;
            fascicleFolder.idCategory = source.idCategory;
            fascicleFolder.idFascicle = source.idFascicle;
            fascicleFolder.Typology = source.Typology;
        }

        return fascicleFolder;
    }


    modifyFascicleFolder() {
        let fascicleFolder: FascicleFolderModel = <FascicleFolderModel>{};
        fascicleFolder.Name = this._txtName.get_textBoxValue();
        fascicleFolder.UniqueId = this._currentFolder.UniqueId;
        fascicleFolder.Typology = <FascicleFolderTypology>FascicleFolderTypology[this._currentFolder.Typology.toString()];

        if (this._currentFolder.idFascicle) {
            let fasc: FascicleModel = new FascicleModel();
            fasc.UniqueId = this._currentFolder.idFascicle;
            fascicleFolder.Fascicle = fasc;
        }
       
        this._fascicleFolderService.updateFascicleFolder(fascicleFolder, null,
            (data: any) => {
                if (data == null) return;
                this.closeFascicleFolderModifica(data, fascicleFolder);               
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.currentPageId);
                this.showNotificationException(this.uscNotificationId, exception);
                this._btnConferma.set_enabled(true);
            }
        );

    }

    /**
     *  Imposto i dati della cartella nella pagina di modifica
     * @param dossierFolder
     */
    setData(fascicleFolder: FascicleSummaryFolderViewModel) {
        this._txtName.set_value(fascicleFolder.Name);
        this._loadingPanel.hide(this.currentPageId);
    }
    /**
    * salvo lo stato corrente della pagina
    */
    private bindLoaded(): void {
        $("#".concat(this.currentPageId)).data(this);

    }
   

    closeFascicleFolderModifica = (data: FascicleFolderModel, fascicleFolder: FascicleFolderModel) => {
        let model = <AjaxModel>{};
        model.ActionName = "ModifyFolder";
        model.Value = [];
        let mapper = new FascicleFolderSummaryModelMapper();
        let resultModel: FascicleSummaryFolderViewModel = mapper.Map(data);
        if (fascicleFolder.Fascicle) {
            resultModel.idFascicle = fascicleFolder.Fascicle.UniqueId;
        }
        model.Value.push(JSON.stringify(resultModel));
        this._loadingPanel.hide(this.currentPageId);
        this.closeWindow(model);
    }

    exceptionWindow = (exception: ExceptionDTO) => {
        let message: string = "";
        let ex: ExceptionDTO = exception;
        if (exception && exception instanceof ValidationExceptionDTO && exception.validationMessages.length > 0) {
            message = message.concat("Gli errori sono i seguenti: <br />");
            exception.validationMessages.forEach(function (item: ValidationMessageDTO) {
                message = message.concat(item.message, "<br />");
            })
        }
        this.showNotificationException(this.uscNotificationId, ex, message);
    }

    /**
* Recupera una RadWindow dalla pagina
*/
    protected getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }


    /**
    * Chiude la RadWindow
    */
    protected closeWindow(message?: AjaxModel): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(message);
    }
}

export = FascicleFolderModifica;