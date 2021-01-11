/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />

import FascicleBase = require('Fasc/FascBase');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import FascicleDocumentModel = require('App/Models/Fascicles/FascicleDocumentModel');
import ChainType = require('App/Models/DocumentUnits/ChainType');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import FascicleModelMapper = require('App/Mappers/Fascicles/FascicleModelMapper');
import AjaxModel = require('App/Models/AjaxModel');
import FascicleDocumentService = require('App/Services/Fascicles/FascicleDocumentService');
import FascicleFolderModel = require('App/Models/Fascicles/FascicleFolderModel');

class FascDocumentsInsert extends FascicleBase {

    currentFascicleId: string;
    ajaxManagerId: string;
    currentPageId: string;
    ajaxLoadingPanelId: string;
    radNotificationInfoId: string;
    radWindowManagerId: string;
    uscNotificationId: string;
    locationId: string;
    archiveName: string;
    idFascicleFolder: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _notificationInfo: Telerik.Web.UI.RadNotification;    
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _fascicleModel: FascicleModel;
    private _fascicleDocumentService: FascicleDocumentService;

    /**
     * Costruttore
    */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }

     /**
     *------------------------- Events -----------------------------
     */

     /**
     *------------------------- Methods -----------------------------
     */
    initialize() {
        super.initialize();
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._notificationInfo = <Telerik.Web.UI.RadNotification>$find(this.radNotificationInfoId);        
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        
    }
}
export = FascDocumentsInsert;