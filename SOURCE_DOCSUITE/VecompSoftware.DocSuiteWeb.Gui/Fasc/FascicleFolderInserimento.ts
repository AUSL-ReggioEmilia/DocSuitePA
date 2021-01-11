/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import FascBase = require('Fasc/FascBase');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import DomainUserService = require('App/Services/Securities/DomainUserService');
import ErrorHelper = require('App/Helpers/ErrorHelper');
import FascicleFolderService = require('App/Services/Fascicles/FascicleFolderService');
import FascicleFolderModel = require('App/Models/Fascicles/FascicleFolderModel');
import AjaxModel = require('App/Models/AjaxModel');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import DomainUserModel = require('App/Models/Securities/DomainUserModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import FascicleFolderStatus = require("App/Models/Fascicles/FascicleFolderStatus");
import FascicleFolderTypology = require("App/Models/Fascicles/FascicleFolderTypology");
import FascicleSummaryFolderViewModel = require('App/ViewModels/Fascicles/FascicleSummaryFolderViewModel');
import FascicelFolderSummaryModelMapper = require('App/Mappers/Fascicles/FascicleSummaryFolderViewModelMapper');
import ValidationExceptionDTO = require('App/DTOs/ValidationExceptionDTO');
import ValidationMessageDTO = require('App/DTOs/ValidationMessageDTO');
import Guid = require('App/Helpers/GuidHelper');

declare var Page_IsValid: any;
class FascicleFolderInserimento extends FascBase {

    pageId: string;
    txtNameId: string;
    btnConfermaId: string;
    btnConfermaUniqueId: string;
    uniqueId: string;
    currentFascicleFolderId: string;
    ajaxManagerId: string;
    ajaxLoadingPanelId: string;
    currentPageId: string;
    managerId: string;
    uscNotificationId: string;    
    fascicleNameRowId: string; 
    sessionUniqueKey: string;
    doNotUpdateDatabase: string;

    private _manager: Telerik.Web.UI.RadWindowManager;
    private _serviceConfigurations: ServiceConfiguration[];    
    private _txtName: Telerik.Web.UI.RadTextBox;
    private _btnConferma: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _domainUserService: DomainUserService;
    private _fascicleFolderService: FascicleFolderService;

    /**
* Costruttore
* @param serviceConfiguration
*/
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascBase.FASCICLEFOLDER_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }

    /**
*------------------------- Events -----------------------------
*/

    /**
   * Evento scatenato al click del pulsante ConfermaInserimento
   * @method
   * @param sender
   * @param eventArgs
   * @returns
   */
    btmConferma_ButtonClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        if (!Page_IsValid) {
            return;
        }

        this._loadingPanel.show(this.currentPageId);
        this._btnConferma.set_enabled(false);

        this.insertFascicleFolder();
    }


    /**
    * Inizializzazione
    */

    initialize() {
        super.initialize();
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._btnConferma = <Telerik.Web.UI.RadButton>$find(this.btnConfermaId);
        this._btnConferma.add_clicked(this.btmConferma_ButtonClicked);
        this._txtName = <Telerik.Web.UI.RadTextBox>$find(this.txtNameId);
      
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.managerId);
       
        let fascicleFolderConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascBase.FASCICLEFOLDER_TYPE_NAME);
        this._fascicleFolderService = new FascicleFolderService(fascicleFolderConfiguration);
              
        $("#".concat(this.fascicleNameRowId)).show();
        this._loadingPanel.hide(this.currentPageId);
        (<Telerik.Web.UI.RadTextBox>this._txtName).focus();
    }


    /**
    *---------------------------- Methods ---------------------------
    */

    insertFascicleFolder() {

        let fascicleFolder = <FascicleFolderModel>{};
        let fascicle = <FascicleModel>{};
        fascicleFolder.Status = FascicleFolderStatus.Active;
        
        fascicleFolder.Name = this._txtName.get_textBoxValue();
        fascicleFolder.Fascicle = fascicle;

        let fascicleFolderToUpdate: FascicleFolderModel = this.getFolderParent(this.currentFascicleFolderId);
        if (fascicleFolderToUpdate) {
            fascicleFolder.ParentInsertId = fascicleFolderToUpdate.UniqueId;
            fascicleFolder.Fascicle.UniqueId = fascicleFolderToUpdate.Fascicle.UniqueId;
            fascicleFolder.Typology = FascicleFolderTypology.SubFascicle;
        }
        if (this.doNotUpdateDatabase === "False") {
            this.callInsertFascicleFolderService(fascicleFolder);
        }
        else {
            this._loadingPanel.hide(this.currentPageId);
            let model = <AjaxModel>{};
            model.ActionName = "ManageParent";
            model.Value = [];
            fascicleFolder.UniqueId = Guid.newGuid();
            model.Value.push(JSON.stringify(fascicleFolder));
            this.closeWindow(model);
        }
    }

    callInsertFascicleFolderService = (fascicleFolder: FascicleFolderModel) => {
        this._fascicleFolderService.insertFascicleFolder(fascicleFolder, null,
            (data: any) => {
                if (data == null) return;
                let model = <AjaxModel>{};
                model.ActionName = "ManageParent";
                model.Value = [];
                let mapper = new FascicelFolderSummaryModelMapper();
                let modelFascicleFolderSummary: FascicleSummaryFolderViewModel = mapper.Map(data);
                if (fascicleFolder.Fascicle != null && fascicleFolder.Fascicle.UniqueId != null) {
                    modelFascicleFolderSummary.idFascicle = fascicleFolder.Fascicle.UniqueId;
                }
                model.Value.push(JSON.stringify(modelFascicleFolderSummary));
                this._loadingPanel.hide(this.currentPageId);
                this.closeWindow(model);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.currentPageId);
                this.exceptionWindow(exception);
                this._btnConferma.set_enabled(true);
            }
        );
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
    * Recupero la cartella salvata nella session storage
    * @param idFascicleFolder
    */
    getFolderParent = (idFascicleFolder: string) => {
        let fascicleFolder = <FascicleFolderModel>{};        
        let result: any = sessionStorage[this.sessionUniqueKey];
        if (result == null) {
            return null;
        }
        let source = JSON.parse(result);
        if (source) {
            fascicleFolder.UniqueId = source.UniqueId;
            fascicleFolder.Fascicle = <FascicleModel>{};
            fascicleFolder.Fascicle.UniqueId = source.idFascicle;
            fascicleFolder.Typology = source.Typology;
        }
        return fascicleFolder;
    }  

    /**
* Chiude la RadWindow
*/
    protected closeWindow(message?: AjaxModel): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(message);
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
}

export = FascicleFolderInserimento;