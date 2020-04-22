/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />

import FascicleBase = require('Fasc/FascBase');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import UscMiscellanea = require('UserControl/uscMiscellanea');
import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import FascicleDocumentModel = require('App/Models/Fascicles/FascicleDocumentModel');
import ChainType = require('App/Models/DocumentUnits/ChainType');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import FascicleModelMapper = require('App/Mappers/Fascicles/FascicleModelMapper');
import AjaxModel = require('App/Models/AjaxModel');
import FascicleDocumentService = require('App/Services/Fascicles/FascicleDocumentService');
import FascicleFolderModel = require('App/Models/Fascicles/FascicleFolderModel');

class uscFascInsertMiscellanea extends FascicleBase {

    currentFascicleId: string;
    ajaxManagerId: string;
    currentPageId: string;
    ajaxLoadingPanelId: string;
    pageContentId: string;
    btnUploadDocumentId: string;
    radNotificationInfoId: string;
    pnlButtonsId: string;
    radWindowManagerId: string;
    uscNotificationId: string;
    uscMiscellaneaId: string;
    locationId: string;
    archiveName: string;
    idFascicleFolder: string;
    btnUploadZipDocumentId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _notificationInfo: Telerik.Web.UI.RadNotification;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _pnlButtons: JQuery;
    private _fascicleModel: FascicleModel;
    private _btnUploadDocument: Telerik.Web.UI.RadButton;
    private _fascicleDocumentService: FascicleDocumentService;
    private _btnUploadZipDocument: Telerik.Web.UI.RadButton;

    public static DELETE_DOCUMENT: string = "Delete_Document";
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

    btnUploadDocument_OnClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonEventArgs) => {
        this.openUploadDocumentWindow("CommonSelMiscellanea", "True");
    }

    _btnUploadZipDocument_OnClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonEventArgs) => {
        this.openUploadDocumentWindow("CommonUploadZIPMiscellanea", "False");
    }
    /**
    *------------------------- Methods -----------------------------
    */
    initialize() {
        super.initialize();
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._notificationInfo = <Telerik.Web.UI.RadNotification>$find(this.radNotificationInfoId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        this._btnUploadDocument = <Telerik.Web.UI.RadButton>$find(this.btnUploadDocumentId);
        if (this._btnUploadDocument) {
            this._btnUploadDocument.add_clicked(this.btnUploadDocument_OnClicked);
        }
        this._btnUploadZipDocument = <Telerik.Web.UI.RadButton>$find(this.btnUploadZipDocumentId);
        if (this._btnUploadZipDocument) {
            this._btnUploadZipDocument.add_clicked(this._btnUploadZipDocument_OnClicked);
        }

        this._pnlButtons = $("#".concat(this.pnlButtonsId));
        if (this._pnlButtons) {
            this._pnlButtons.hide();
        }

        let fascicleDocumentConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "FascicleDocument");
        this._fascicleDocumentService = new FascicleDocumentService(fascicleDocumentConfiguration);

        this.service.getFascicle(this.currentFascicleId,
            (data: any) => {
                if (data == null) return;
                this._fascicleModel = data;
                this._fascicleDocumentService.getByFolder(this._fascicleModel.UniqueId, this.idFascicleFolder,
                    (data: any) => {
                        this._fascicleModel.FascicleDocuments = data;
                        $("#".concat(this.uscMiscellaneaId)).bind(UscMiscellanea.LOADED_EVENT, (args) => {
                            this._loadingPanel.show(this.currentPageId);
                            this.loadMiscellanea();
                        });

                        $("#".concat(this.uscMiscellaneaId)).on(UscMiscellanea.DELETE_DOCUMENT_EVENT, (args, idDocument, idArchiveChain) => {
                            this.deleteDocument(idDocument, idArchiveChain);
                        });


                        $("#".concat(this.uscMiscellaneaId)).on(UscMiscellanea.UPDATE_DOCUMENTS_EVENT, (args, idArchiveChain) => {
                            this.UpdateDocuments(idArchiveChain);
                        });
                        this._loadingPanel.show(this.currentPageId);
                        this.loadMiscellanea();
                    });
            }
        );
    }

    private loadMiscellanea() {
        let uscMiscellanea: UscMiscellanea = <UscMiscellanea>$("#".concat(this.uscMiscellaneaId)).data();
        if (!jQuery.isEmptyObject(uscMiscellanea)) {
            let insertsArchiveChain: string = ""

            let inserts: FascicleDocumentModel = $.grep(this._fascicleModel.FascicleDocuments, (element, index) => {
                if (isNaN(element.ChainType)) {
                    element.ChainType = ChainType[element.ChainType.toString()];
                }

                return element.ChainType == ChainType.Miscellanea;
            })[0];
            if (inserts != undefined) {
                insertsArchiveChain = inserts.IdArchiveChain;
            }

            uscMiscellanea.archiveChainId = insertsArchiveChain;
            uscMiscellanea.locationId = this.locationId;
            uscMiscellanea.loadMiscellanea(insertsArchiveChain, this.locationId);
        }
        this._loadingPanel.hide(this.currentPageId);
        this._pnlButtons.show();
    }

    private UpdateDocuments(idArchiveChain) {
        let inserts: FascicleDocumentModel = $.grep(this._fascicleModel.FascicleDocuments, (x) => x.ChainType == ChainType.Miscellanea)[0];
        if ((!inserts || idArchiveChain != inserts.IdArchiveChain) && !!idArchiveChain) {
            //aggiorna fascicolo
            let fascicleDocumentModel: FascicleDocumentModel = <FascicleDocumentModel>{};
            fascicleDocumentModel.ChainType = ChainType.Miscellanea;
            fascicleDocumentModel.IdArchiveChain = idArchiveChain;
            fascicleDocumentModel.Fascicle = this._fascicleModel;
            if (this.idFascicleFolder) {
                fascicleDocumentModel.FascicleFolder = <FascicleFolderModel>{};
                fascicleDocumentModel.FascicleFolder.UniqueId = this.idFascicleFolder;
            }

            this._fascicleDocumentService.insertFascicleDocument(fascicleDocumentModel,
                (data: any) => {
                    this._fascicleModel.FascicleDocuments.push(fascicleDocumentModel);
                    this.loadMiscellanea();
                },
                (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.pageContentId);
                    this.showNotificationException(this.uscNotificationId, exception);
                }
            );
        }
        else {
            this.loadMiscellanea();
        }
    }

    private deleteDocument(idDocument: string, idArchiveChain: string) {
        let request: AjaxModel = <AjaxModel>{};
        request.ActionName = uscFascInsertMiscellanea.DELETE_DOCUMENT;
        request.Value = [];
        request.Value.push(idDocument);
        request.Value.push(idArchiveChain);

        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._ajaxManager.ajaxRequest(JSON.stringify(request));
    }

    refreshDocuments = () => {
        this.loadMiscellanea();
    }

    openUploadDocumentWindow(documentPageName: string, multiDoc: string) {
        let uscMiscellanea: UscMiscellanea = <UscMiscellanea>$("#".concat(this.uscMiscellaneaId)).data();
        if (!jQuery.isEmptyObject(uscMiscellanea)) {
            let url: string = `../UserControl/${documentPageName}.aspx?Action=Add&Type=Fasc&IdLocation=${this.locationId}&ArchiveName=${this.archiveName}&MultiDoc=${multiDoc}`;

            let inserts: FascicleDocumentModel = $.grep(this._fascicleModel.FascicleDocuments, (x) => x.ChainType == ChainType.Miscellanea)[0];
            if (inserts != undefined) {
                url = url.concat('&IdArchiveChain=').concat(inserts.IdArchiveChain);
            }

            uscMiscellanea.openInsertWindow(url);
            return false;
        }
    }
}
export = uscFascInsertMiscellanea;