/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/dsw/dsw.signalr.d.ts" />
/// <reference path="../Scripts/typings/moment/moment.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />

import DossierBase = require('Dossiers/DossierBase');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import UscMiscellanea = require('UserControl/uscMiscellanea');
import DossierSummaryViewModel = require('App/ViewModels/Dossiers/DossierSummaryViewModel');
import DossierModel = require('App/Models/Dossiers/DossierModel');
import DossierService = require('App/Services/Dossiers/DossierService');
import BaseEntityViewModel = require('App/ViewModels/BaseEntityViewModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ChainType = require('App/Models/DocumentUnits/ChainType');
import AjaxModel = require('App/Models/AjaxModel');
import DossierDocumentModel = require('App/Models/Dossiers/DossierDocumentModel');
import DossierDocumentService = require('App/Services/Dossiers/DossierDocumentService');

class DossierMiscellanea extends DossierBase {

    currentDossierId: string;
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

    private _serviceConfigurations: ServiceConfiguration[];
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _notificationInfo: Telerik.Web.UI.RadNotification;    
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _pnlButtons: JQuery;
    private _dossierModel: DossierSummaryViewModel;
    private _dossierService: DossierService;
    private _btnUploadDocument: Telerik.Web.UI.RadButton;
    private _DossierDocuments: Array<BaseEntityViewModel>;
    private _dossierDocumentService: DossierDocumentService;

    public static DELETE_DOCUMENT: string = "Delete_Document";
    /**
     * Costruttore
    */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, DossierBase.DOSSIER_TYPE_NAME));
        this._serviceConfigurations = serviceConfigurations;
        $(document).ready(() => {

        });
    }

    /**
    *------------------------- Events -----------------------------
    */
    btnUploadDocument_OnClicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.RadButtonEventArgs) => {
        let uscMiscellanea: UscMiscellanea = <UscMiscellanea>$("#".concat(this.uscMiscellaneaId)).data();
        if (!jQuery.isEmptyObject(uscMiscellanea)) {
            let url: string = '../UserControl/CommonSelMiscellanea.aspx?Action=Add&Type=Dossier&IdLocation='.concat(this.locationId,'&ArchiveName=', this.archiveName);

            let insertsArchiveChain: string = "";            
            if (this._dossierModel.Documents != undefined && this._dossierModel.Documents.length > 0) {
                insertsArchiveChain = this._dossierModel.Documents[0].IdArchiveChain.toString();
                url = url.concat('&IdArchiveChain=').concat(insertsArchiveChain);
            }
            
            uscMiscellanea.openInsertWindow(url);
            return false;
        }
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
        this._btnUploadDocument.add_clicked(this.btnUploadDocument_OnClicked);

        this._pnlButtons = $("#".concat(this.pnlButtonsId));
        this._pnlButtons.hide();

        this._dossierModel = <DossierSummaryViewModel>{};
        this._DossierDocuments = new Array<BaseEntityViewModel>();

        let dossierDocumentConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, DossierBase.DOSSIERDOCUMENT_TYPE_NAME);
        this._dossierDocumentService = new DossierDocumentService(dossierDocumentConfiguration);

        (<DossierService>this.service).getDossier(this.currentDossierId,
            (data: any) => {
                if (data == null) return;
                this._dossierModel = data;

                (<DossierDocumentService>this._dossierDocumentService).getDossierDocuments(this.currentDossierId,
                    (data: any) => {
                        try {
                            if (!data) {
                                return;
                            }
                            this._DossierDocuments = data;
                            this._dossierModel.Documents = this._DossierDocuments;

                            $("#".concat(this.uscMiscellaneaId)).bind(UscMiscellanea.LOADED_EVENT, (args) => {
                                this.loadMiscellanea();
                            });


                            $("#".concat(this.uscMiscellaneaId)).on(UscMiscellanea.DELETE_DOCUMENT_EVENT, (args, idDocument, idArchiveChain) => {
                                this.deleteDocument(idDocument, idArchiveChain);
                            });

                            $("#".concat(this.uscMiscellaneaId)).on(UscMiscellanea.UPDATE_DOCUMENTS_EVENT, (args, idArchiveChain) => {
                                this.UpdateDocuments(idArchiveChain);
                            });

                            this.loadMiscellanea();

                        } catch (error) {
                            console.log(JSON.stringify(error));
                        }
                    },
                    (exception: ExceptionDTO): void => {
                        this.showNotificationException(this.uscNotificationId, exception);
                        this.showNotificationMessage(this.uscNotificationId, "Errore durante il caricamento degli inserti del dossier.");
                    });               
            }

        );
    }

    private loadMiscellanea() {        
        let uscMiscellanea: UscMiscellanea = <UscMiscellanea>$("#".concat(this.uscMiscellaneaId)).data();
        if (!jQuery.isEmptyObject(uscMiscellanea)) {
            let insertsArchiveChain: string = "";

            if (this._dossierModel.Documents != undefined && this._dossierModel.Documents.length > 0) {
                insertsArchiveChain = this._dossierModel.Documents[0].IdArchiveChain.toString();
            }                

            uscMiscellanea.archiveChainId = insertsArchiveChain;
            uscMiscellanea.locationId = this.locationId;
            uscMiscellanea.loadMiscellanea(insertsArchiveChain, this.locationId);
        }
        this._pnlButtons.show();
    }

    private UpdateDocuments(idArchiveChain) {
        let insertArchiveChain: string = "";
        if (this._dossierModel.Documents != undefined && this._dossierModel.Documents.length > 0) {
            insertArchiveChain = this._dossierModel.Documents[0].IdArchiveChain.toString();
        }

        if ((!insertArchiveChain || idArchiveChain != insertArchiveChain) && !!idArchiveChain) {
            //aggiorna dossier

            let dossier: DossierModel = <DossierModel>{};
            dossier.UniqueId = this._dossierModel.UniqueId;            

            let dossierDocumentModel: DossierDocumentModel = <DossierDocumentModel>{};            
            dossierDocumentModel.ChainType = ChainType.Miscellanea;
            dossierDocumentModel.IdArchiveChain = idArchiveChain;
            dossierDocumentModel.Dossier = dossier;

            //secondo me devi mettere tutto il modello del dossier (anche contatti)
            (<DossierDocumentService>this._dossierDocumentService).insertDossierDocument(dossierDocumentModel,
                (data: any) => {
                    let inserted: BaseEntityViewModel = new BaseEntityViewModel();
                    inserted.IdArchiveChain = dossierDocumentModel.IdArchiveChain;
                    this._dossierModel.Documents.push(inserted);
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
        request.ActionName = DossierMiscellanea.DELETE_DOCUMENT;
        request.Value = [];
        request.Value.push(idDocument);
        request.Value.push(idArchiveChain);

        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._ajaxManager.ajaxRequest(JSON.stringify(request));
    }

    refreshDocuments = () => {
        this.loadMiscellanea();
    }
}
export = DossierMiscellanea;