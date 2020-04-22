import DossierModel = require('App/Models/Dossiers/DossierModel');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import ContactModel = require('App/Models/Commons/ContactModel');
import BaseEntityViewModel = require('App/ViewModels/BaseEntityViewModel');
import DossierSummaryViewModel = require('App/ViewModels/Dossiers/DossierSummaryViewModel');
import AjaxModel = require('App/Models/AjaxModel');
import DossierBase = require('Dossiers/DossierBase');
import WindowHelper = require('App/Helpers/WindowHelper');
import DossierService = require('App/Services/Dossiers/DossierService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import DossierDocumentService = require('App/Services/Dossiers/DossierDocumentService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscDynamicMetadataClient = require('UserControl/uscDynamicMetadataClient');

declare var Page_IsValid: any;

class DossierModifica extends DossierBase {

    currentDossierId: string
    dossierPageContentId: string;
    btnConfirmId: string;
    btnConfirmUniqueId: string;
    lblStartDateId: string;
    lblYearId: string;
    lblNumberId: string;
    lblContainerId: string;
    txtObjectId: string;
    txtNoteId: string;
    rdpStartDateId: string;
    rfvStartDateId: string;
    ajaxLoadingPanelId: string;
    ajaxManagerId: string;
    radWindowManagerId: string;
    uscNotificationId: string;
    uscDynamicMetadataId: string;
    metadataRepositoryEnabled: boolean;
    rowMetadataId: string;

    private _serviceConfigurations: ServiceConfiguration[];
    private _dossierService: DossierService;
    private _dossierDocumentService: DossierDocumentService;
    private _txtNote: Telerik.Web.UI.RadTextBox;
    private _rdpStartDate: Telerik.Web.UI.RadDatePicker;
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _DossierModel: DossierSummaryViewModel;
    private _DossierContacts: Array<BaseEntityViewModel>;
    private _lblStartDate: JQuery;
    private _lblNumber: JQuery;
    private _lblYear: JQuery;
    private _lblContainer: JQuery;
    private _rowMetadataRepository: JQuery;

    /**
* Costruttore
* @param serviceConfiguration
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

    /**
   *------------------------- Methods -----------------------------
   */

    /**
    * Metodo di inizializzazione
    */
    initialize() {
        super.initialize();
        this._ajaxManager = (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId))
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._txtNote = <Telerik.Web.UI.RadTextBox>$find(this.txtNoteId);
        this._rdpStartDate = <Telerik.Web.UI.RadDatePicker>$find(this.rdpStartDateId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.ajaxManagerId);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._DossierModel = <DossierSummaryViewModel>{};
        this._DossierContacts = new Array<BaseEntityViewModel>();
        this._lblStartDate = $("#".concat(this.lblStartDateId));
        this._lblYear = $("#".concat(this.lblYearId));
        this._lblNumber = $("#".concat(this.lblNumberId));
        this._lblContainer = $("#".concat(this.lblContainerId));
        this._btnConfirm.set_enabled(false);
        this._loadingPanel.show(this.dossierPageContentId);
        this._rowMetadataRepository = $("#".concat(this.rowMetadataId));
        this._rowMetadataRepository.hide();

        (<DossierService>this.service).hasModifyRight(this.currentDossierId,
            (data: any) => {
                if (data == null) return;
                if (data) {

                    $.when(this.loadDossier(), this.loadContacts()).done(() => {
                        this._DossierModel.Contacts = this._DossierContacts;
                        this.fillPageFromModel(this._DossierModel);

                        if (this.metadataRepositoryEnabled) {

                            this.loadMetadata(this._DossierModel.JsonMetadata);
                        }
                    }).fail((exception) => {
                        this._btnConfirm.set_enabled(false);
                        this._loadingPanel.hide(this.dossierPageContentId);
                        this.showNotificationException(this.uscNotificationId, exception, "Errore nel caricamento del Dossier.");
                    });
                }
                else {
                    this._btnConfirm.set_enabled(false);
                    this._loadingPanel.hide(this.dossierPageContentId);
                    $("#".concat(this.dossierPageContentId)).hide();
                    this.showNotificationMessage(this.uscNotificationId, "Errore in inizializzazione pagina.<br \> Utente non autorizzato alla modifica del Dossier.");
                }

            },
            (exception: ExceptionDTO) => {
                this._btnConfirm.set_enabled(false);
                this._loadingPanel.hide(this.dossierPageContentId);
                $("#".concat(this.dossierPageContentId)).hide();
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }


    /*
    * Carico il dossier corrente senza navigation properties
    */
    private loadDossier(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();

        try {
            (<DossierService>this.service).getDossier(this.currentDossierId,
                (data: any) => {
                    if (data == undefined) {
                        promise.resolve();
                        return;
                    }
                    this._DossierModel = data;
                    promise.resolve();
                },
                (exception: ExceptionDTO): void => {
                    promise.reject(exception);
                });
        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject(error);
        }
        return promise.promise();
    }

    /**
    * carico i contatti del Dossier
    */
    loadContacts(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            this.service.getDossierContacts(this.currentDossierId,
                (data: any) => {
                    try {
                        if (!data) {
                            promise.resolve();
                            return;
                        }
                        this._DossierContacts = data;
                        promise.resolve();
                    } catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject(error);
                    }
                },
                (exception: ExceptionDTO): void => {
                    promise.reject(exception);
                });
        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject(error);
        }
        return promise.promise();
    }

    /**
 * Esegue il fill dei controlli della pagina in  modello DossierModel in update
 */
    private fillPageFromModel(model: DossierSummaryViewModel) {

        this._lblYear.html(model.Year.toString());
        this._lblNumber.html(model.Number);
        this._lblContainer.html(model.ContainerName);
        this._lblStartDate.html(model.FormattedStartDate);
        this._rdpStartDate.set_selectedDate(new Date(model.StartDate.toString()));

        let txtObject: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>$find(this.txtObjectId);
        txtObject.set_value(model.Subject);
        this._txtNote.set_value(model.Note);

        let ajaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();
        ajaxModel.Value.push(JSON.stringify(model.Contacts));
        ajaxModel.ActionName = "LoadExternalData";

        (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxModel));
    }
    
    /**
    * Callback da code-behind per chiusura caricamento pagina
    * @param contact
    * @param category
    */
    endLoadingDataCallback(): void {
        this._btnConfirm.set_enabled(true);
        this._loadingPanel.hide(this.dossierPageContentId);
    }

    /**
    * Callback da code-behind per la modifica di un Dossier
    * @param contact
    * @param category
    */
    updateCallback(contact: string): void {
        let dossierModel: DossierModel = <DossierModel>{};

        //riferimento
        this.fillContacts(contact, dossierModel);
        this.fillModelFromPage(dossierModel);

        if (this.metadataRepositoryEnabled){
            let uscDynamicMetadataClient: UscDynamicMetadataClient = <UscDynamicMetadataClient>$("#".concat(this.uscDynamicMetadataId)).data();
            if (!jQuery.isEmptyObject(uscDynamicMetadataClient)) {
                dossierModel.JsonMetadata = uscDynamicMetadataClient.bindModelFormPage();
            }
        }

        (<DossierService>this.service).updateDossier(dossierModel,
            (data: any) => {
                if (data == null) return;
                this._loadingPanel.show(this.dossierPageContentId);
                window.location.href = "DossierVisualizza.aspx?Type=Dossier&IdDossier=".concat(this.currentDossierId, "&DossierTitle=", data.Year.toString(), "/", ("000000000" + data.Number.toString()).slice(-7));
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.dossierPageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    /**
    * Esegue il fill dei controlli della pagina in  modello DossierModel in update
    */
    private fillModelFromPage(model: DossierModel): DossierModel {
        let txtObject: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>$find(this.txtObjectId);
        model.UniqueId = this.currentDossierId;
        model.Subject = txtObject.get_value();
        model.Note = this._txtNote.get_value();
        model.Year = Number(this._lblYear.text());
        model.Number = this._lblNumber.text();

        let containerModel: ContainerModel = <ContainerModel>{};
        containerModel.EntityShortId = Number(this._DossierModel.ContainerId);
        model.Container = containerModel;

        let selectedDate = new Date(this._rdpStartDate.get_selectedDate().getTime() - this._rdpStartDate.get_selectedDate().getTimezoneOffset() * 60000);
        model.StartDate = selectedDate;

        return model;
    }


    private loadMetadata(metadatas: string) {
        if (metadatas) {
            this._rowMetadataRepository.show();
            let uscDynamicMetadataClient: UscDynamicMetadataClient = <UscDynamicMetadataClient>$("#".concat(this.uscDynamicMetadataId)).data();
            if (!jQuery.isEmptyObject(uscDynamicMetadataClient)) {
                uscDynamicMetadataClient.loadPageItems(metadatas);
            }
        }
    }

}

export = DossierModifica;