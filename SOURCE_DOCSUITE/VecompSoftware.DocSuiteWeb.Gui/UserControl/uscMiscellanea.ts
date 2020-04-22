/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/moment/moment.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import AjaxModel = require('App/Models/AjaxModel');
import FileHelper = require('App/Helpers/FileHelper');

class uscMiscellanea {

    pageId: string;
    ajaxManagerId: string;
    uscNotificationId: string;
    ajaxLoadingPanelId: string;
    miscellaneaToolBarId: string;  
    miscellaneaGridId: string;
    managerWindowsId: string;
    managerUploadDocumentId: string;    
    managerId: string;
    locationId: string;
    archiveChainId: string;
    type: string;

    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;    
    private _uscNotification: UscErrorNotification;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _miscellaneaToolBar: Telerik.Web.UI.RadToolBar;
    private _managerUploadDocument: Telerik.Web.UI.RadWindowManager;
    private _manager: Telerik.Web.UI.RadWindowManager;    
    private _miscellaneaGrid: Telerik.Web.UI.RadGrid;
    private _masterTableView: Telerik.Web.UI.GridTableView;  
    private _currentDocumentToSign: string;

    public static ON_END_LOAD_EVENT = "onEndLoad";
    public static LOADED_EVENT: string = "onLoaded";    
    public static LOAD_DOCUMENTS: string = "LoadDocuments";
    public static SIGNED_DOCUMENT: string = "Signed";
    public static UPDATE_DOCUMENTS_EVENT: string = "Update_Documents";    
    public static DELETE_DOCUMENT_EVENT: string = "Delete_Document";

    /**
    * Costruttore
    * @param webApiConfiguration
    */
    constructor() {
        $(document).ready(() => {
        });
    }



    /**
    *---------------------------- Events ---------------------------
    */


    /**
    *---------------------------- Methods ---------------------------
    */
    /**
   * Inizializzazione
   */
    initialize() {
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._miscellaneaToolBar = <Telerik.Web.UI.RadToolBar>$find(this.miscellaneaToolBarId);     
        this._managerUploadDocument = <Telerik.Web.UI.RadWindowManager>$find(this.managerUploadDocumentId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.managerId);        
        this._managerUploadDocument.add_close(this.closeUploadDocumentWindow);

        this._miscellaneaGrid = <Telerik.Web.UI.RadGrid>$find(this.miscellaneaGridId);
        this._masterTableView = this._miscellaneaGrid.get_masterTableView();

        this._currentDocumentToSign = undefined;

        this.bindLoaded();
    }

    /**
    * Carico i documenti 
    */

    bindMiscellanea(documents: string): void {        
        this._masterTableView.set_dataSource(documents);
        this._masterTableView.dataBind();
        this.initializeCallback();
    }    

    loadMiscellanea(idArchiveChain: string, location: string): void {
         this.loadDocuments(idArchiveChain, location);
    }

    loadDocuments(idArchiveChain: string, location: string): void {
        this._loadingPanel.show(this.pageId);
        let ajaxRequest: AjaxModel = <AjaxModel>{};
        ajaxRequest.ActionName = uscMiscellanea.LOAD_DOCUMENTS;
        ajaxRequest.Value = new Array<string>();
        ajaxRequest.Value.push(idArchiveChain);
        ajaxRequest.Value.push(location);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxRequest));
    }

    /**
    * Scateno l'evento di "Load Completed" del controllo
    */
    private bindLoaded(): void {
        $("#".concat(this.pageId)).data(this);
        $("#".concat(this.pageId)).triggerHandler(uscMiscellanea.LOADED_EVENT);
    }

    /**
    * Metono chiamato in chiusura della radwindow di inserimento
    * @param sender
    * @param args
    */
    closeUploadDocumentWindow = (sender: any, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument) {
            let result: AjaxModel = <AjaxModel>args.get_argument();
            if (result) {               
                $("#".concat(this.pageId)).triggerHandler(uscMiscellanea.UPDATE_DOCUMENTS_EVENT, result.Value[0].toString());
            }
        }
    }

    onGridDataBound() {
        let row = this._masterTableView.get_dataItems();
        for (let i = 0; i < row.length; i++) {
            if (i % 2) {
                row[i].addCssClass("Chiaro");
            }
            else {
                row[i].addCssClass("Scuro");
            }            
        }
    }


    getDocumentExtension(documentName: string) {
        return FileHelper.getImageByFileName(documentName, true);
    }

    /**
    * Apre una nuova nuova RadWindow
    * @param url
    * @param name
    * @param width
    * @param height
    */
    openWindow(url, name, width, height): boolean {
        let manager: Telerik.Web.UI.RadWindowManager = <Telerik.Web.UI.RadWindowManager>$find(this.managerWindowsId);
        let wnd: Telerik.Web.UI.RadWindow = manager.open(url, name, null);
        wnd.setSize(width, height);
        wnd.set_modal(true);
        wnd.center();
        return false;
    }

    openPreviewWindow(serializedDoc: string) {
        let url: string = '../Viewers/DocumentInfoViewer.aspx?'.concat(serializedDoc);
        this.openWindow(url, 'windowPreviewDocument', 750, 450);
    }

    openEditWindow(idDocument: string, idArchiveChain: string, locationId: string) {
        let url: string = '../UserControl/CommonSelMiscellanea.aspx?Action=Edit&IdDocument='.concat(idDocument, "&Type=", this.type);
        url = url.concat('&IdArchiveChain=').concat(idArchiveChain);
        url = url.concat('&IdLocation=').concat(locationId);
        
        this.openWindow(url, 'managerUploadDocument', 770, 450);
    }

    openDeleteWindow(idDocument: string, idArchiveChain: string) {
        this._manager.radconfirm("Sei sicuro di voler eliminare il documento?", (arg) => {
            if (arg) {
                this._loadingPanel.show(this.pageId);
                $("#".concat(this.pageId)).triggerHandler(uscMiscellanea.DELETE_DOCUMENT_EVENT, idDocument, idArchiveChain);             
            }
        }, 300, 160);
    }

    openInsertWindow(url:string) {
        this.openWindow(url, "managerUploadDocument", 820, 530);
    }

    initializeSign(idDocument: string) {
        this._loadingPanel.show(this.pageId);
        let ajaxRequest: AjaxModel = <AjaxModel>{};
        ajaxRequest.ActionName = "InitializeSignDocument";
        ajaxRequest.Value = new Array<string>();
        ajaxRequest.Value.push(idDocument);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxRequest));
    }

    openSignWindow(serializedDoc: string) {
        this._loadingPanel.hide(this.pageId);
        this._currentDocumentToSign = serializedDoc;
        let url: string = `../Comm/SingleSign.aspx?${serializedDoc}`;
        this.openWindow(url, 'signWindow', 750, 500);
    }
    /**
     * Metodo che nasconde il loading 
     */
    hideLoadingPanel = () => {
        this._loadingPanel.hide(this.pageId);
    }
 
    private showNotification(uscNotificationId: string, message: string): void
    private showNotification(uscNotificationId: string, error: ExceptionDTO): void
    private showNotification(uscNotificationId: string, error: any): void {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(this.uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            if (error instanceof ExceptionDTO) {
                uscNotification.showNotification(error);
            } else {
                uscNotification.showNotificationMessage(error);
            }
        }
    }

    initializeCallback = () => {
        this.hideLoadingPanel();
    }

    closeSignWindow(sender: any, args: any) {
        if (args.get_argument() && this._currentDocumentToSign) {
            this._loadingPanel.show(this.pageId);
            let ajaxRequest: AjaxModel = <AjaxModel>{};
            ajaxRequest.ActionName = uscMiscellanea.SIGNED_DOCUMENT;
            ajaxRequest.Value = new Array<string>();
            ajaxRequest.Value.push(args.get_argument());
            ajaxRequest.Value.push(this._currentDocumentToSign);
            this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxRequest));
        }
        this._currentDocumentToSign = undefined;
    }
}
export = uscMiscellanea;