/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import AjaxModel = require("App/Models/AjaxModel");
import UDSService = require("App/Services/UDS/UDSService");
import UDSRepositoryService = require("App/Services/UDS/UDSRepositoryService");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import UscErrorNotification = require('UserControl/uscErrorNotification');
import UDSModel = require("App/Models/UDS/UDSModel");
import UDSRepositoryModel = require("App/Models/UDS/UDSRepositoryModel");
import StringHelper = require("App/Helpers/StringHelper");

class AttachDocuments {

    protected static UDSREPOSITORY_TYPE_NAME = "UDSRepository";

    ajaxManagerId: string;
    ajaxLoadingPanelId: string;
    uscNotificationId: string;
    pageContentId: string;
    btnSearchId: string;
    tblPreviewId: string;
    lblRecordDetailsId: string;
    lblDocumentUDSId: string;
    lblObjectId: string;
    lblCategoryDescriptionId: string;
    btnAddId: string;

    private _serviceConfigurations: ServiceConfiguration[];

    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _ajaxLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _btnSearch: HTMLButtonElement;
    private _tblPreview: HTMLTableElement;
    private _lblRecordDetails: HTMLLabelElement;
    private _lblDocumentUDS: HTMLLabelElement;
    private _lblObject: HTMLLabelElement;
    private _lblCategoryDescription: HTMLLabelElement;
    private _btnAdd: HTMLButtonElement;

    private _udsService: UDSService;
    private _udsRepositoryService: UDSRepositoryService;
    private _idUDS: string;
    private _idUDSRepository: string;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    initialize() {
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._ajaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._btnSearch = <HTMLButtonElement>document.getElementById(this.btnSearchId);
        this._tblPreview = <HTMLTableElement>document.getElementById(this.tblPreviewId);
        this._lblRecordDetails = <HTMLLabelElement>document.getElementById(this.lblRecordDetailsId);
        this._lblDocumentUDS = <HTMLLabelElement>document.getElementById(this.lblDocumentUDSId);
        this._lblObject = <HTMLLabelElement>document.getElementById(this.lblObjectId);
        this._lblCategoryDescription = <HTMLLabelElement>document.getElementById(this.lblCategoryDescriptionId);
        this._btnAdd = <HTMLButtonElement>document.getElementById(this.btnAddId);

        this._btnSearch.onclick = (event: MouseEvent) => {
            this._openSearchWindow();
            return false;
        }
        this._btnAdd.onclick = (event: MouseEvent) => {
            this._addDocuments();
            return false;
        }

        let udsRepositoryConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, AttachDocuments.UDSREPOSITORY_TYPE_NAME);
        this._udsRepositoryService = new UDSRepositoryService(udsRepositoryConfiguration);
    }

    private _openSearchWindow() {
        let winManager: Telerik.Web.UI.RadWindowManager = this._getRadWindow().get_windowManager();
        let window: Telerik.Web.UI.RadWindow = winManager.open("../UDS/UDSLink.aspx?Type=UDS&Action=CopyDocuments", null, null);
        window.set_width(this._getRadWindow().get_width());
        window.set_height(this._getRadWindow().get_height());
        window.add_close(this.onSearchClientClose);
        window.center();
    }

    onSearchClientClose = (sender: Telerik.Web.UI.RadWindow, args: Telerik.Web.UI.WindowCloseEventArgs) => {
        if (args.get_argument() === null) {
            return;
        }

        this._ajaxLoadingPanel.show(this.pageContentId);
        this._idUDS = args.get_argument().split('|')[0];
        this._idUDSRepository = args.get_argument().split('|')[1];

        this._udsRepositoryService.getUDSRepositoryByID(this._idUDSRepository,
            (data: UDSRepositoryModel[]) => {
                let udsRepository: UDSRepositoryModel = data[0];
                let udsConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, udsRepository.Name);
                this._udsService = new UDSService(udsConfiguration);

                this._udsService.getUDSByUniqueId(this._idUDS,
                    (data: any) => {
                        let udsModel: UDSModel = <UDSModel>data;
                        this._populateUDSDetails(udsModel, udsRepository.Name);
                        this._populateDocumentsGrid();
                    },
                    (exception: ExceptionDTO) => {
                        this._ajaxLoadingPanel.hide(this.pageContentId);
                        this._showNotificationException(this.uscNotificationId, exception);
                    });
            },
            (exception: ExceptionDTO) => {
                this._ajaxLoadingPanel.hide(this.pageContentId);
                this._showNotificationException(this.uscNotificationId, exception);
            });

        sender.remove_close(this.onSearchClientClose);
    }

    private _getRadWindow() {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }

    closeWindow(argument) {
        let oWindow: Telerik.Web.UI.RadWindow = this._getRadWindow();
        oWindow.close(argument);
    }

    private _populateUDSDetails(udsModel: UDSModel, udsRepositoryName: string) {
        let stringHelper: StringHelper = new StringHelper();
        this._lblRecordDetails.innerText = `${udsModel.Year}/${stringHelper.pad(udsModel.Number, 6)} del ${new Date(udsModel.RegistrationDate).format("dd/MM/yyyy")}`;
        this._lblDocumentUDS.innerText = udsRepositoryName;
        this._lblObject.innerText = udsModel.Subject;
        this._lblCategoryDescription.innerText = `${udsModel.Category.Code}.${udsModel.Category.Name}`;
        this._tblPreview.style.removeProperty("display");
    }

    private _populateDocumentsGrid() {
        let ajaxModel: AjaxModel = <AjaxModel>{
            ActionName: "SetGridDocuments",
            Value: [this._idUDSRepository, this._idUDS]
        };
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
        this._ajaxLoadingPanel.hide(this.pageContentId);
    }

    gridDocumentsCallback() {
        this._ajaxLoadingPanel.hide(this.pageContentId);
        this._btnAdd.style.removeProperty("display");
    }

    private _addDocuments() {
        let ajaxModel: AjaxModel = <AjaxModel>{
            ActionName: "AddDocuments",
            Value: [this._idUDSRepository, this._idUDS]
        };
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
    }

    private _showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this._showNotificationMessage(uscNotificationId, customMessage)
        }
    }

    private _showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage)
        }
    }
}

export = AttachDocuments;