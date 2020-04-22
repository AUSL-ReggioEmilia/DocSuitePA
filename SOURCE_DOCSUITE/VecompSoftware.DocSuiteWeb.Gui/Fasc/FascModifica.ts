/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import ContactModel = require('App/Models/Commons/ContactModel');
import FascicleType = require('App/Models/Fascicles/FascicleType');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import FascicleBase = require('Fasc/FascBase');
import UscFascicolo = require('UserControl/uscFascicolo');
import FascicleDocumentModel = require('App/Models/Fascicles/FascicleDocumentModel');
import ChainType = require('App/Models/DocumentUnits/ChainType');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import AjaxModel = require('App/Models/AjaxModel');

declare var Page_IsValid: any;
class FascModifica extends FascicleBase {
    currentFascicleId: string;
    txtNameId: string;
    txtObjectId: string;
    txtRackId: string;
    rowNameId: string;
    rowRackId: string;
    rowLegacyManagerId: string;
    txtManagerId: string;
    txtNoteId: string;
    ajaxManagerId: string;
    ajaxLoadingPanelId: string;
    btnConfermaId: string;
    pageContentId: string;
    radWindowManagerId: string;
    uscNotificationId: string;
    uscFascicoloId: string;
    rowManagerId: string;
    isEditPage: boolean;
    fasciclesPanelVisibilities: { [key: string]: boolean };
    uscDynamicMetadataId: string;
    rowDynamicMetadataId: string;
    metadataRepositoryEnabled: boolean;

    private _txtName: Telerik.Web.UI.RadTextBox;
    private _txtRack: Telerik.Web.UI.RadTextBox;
    private _txtNote: Telerik.Web.UI.RadTextBox;
    private _txtManager: Telerik.Web.UI.RadTextBox;
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _txtObject: Telerik.Web.UI.RadTextBox;
    private _rowName: JQuery;
    private _rowRacks: JQuery;
    private _rowDynamicMetadata: JQuery;
    private _fascicleModel: FascicleModel;

    /**
     * Costruttore
     * @param serviceConfigurations
     */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, FascicleBase.FASCICLE_TYPE_NAME));
    }

    /**
     * Initialize
     */
    initialize(): void {
        super.initialize();
        this._txtName = <Telerik.Web.UI.RadTextBox>$find(this.txtNameId);
        this._txtRack = <Telerik.Web.UI.RadTextBox>$find(this.txtRackId);
        this._txtNote = <Telerik.Web.UI.RadTextBox>$find(this.txtNoteId);
        this._txtManager = <Telerik.Web.UI.RadTextBox>$find(this.txtManagerId);
        this._txtObject = <Telerik.Web.UI.RadTextBox>$find(this.txtObjectId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfermaId);
        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        this._rowName = $("#".concat(this.rowNameId));
        this._rowRacks = $("#".concat(this.rowRackId));
        this._rowDynamicMetadata = $("#".concat(this.rowDynamicMetadataId));        
        this._btnConfirm.add_clicking(this.btnConferma_OnClick);
        this._btnConfirm.set_enabled(false);
        this._rowDynamicMetadata.hide();

        this.initializeFascicle();
    }

    /**
     *------------------------- Events -----------------------------
     */

    /**
     * Evento scatenato al click del pulsante di inserimento
     * @param sender
     * @param args
     */
    btnConferma_OnClick = (sender: any, args: Telerik.Web.UI.RadButtonCancelEventArgs) => {
        args.set_cancel(true);
        if (!Page_IsValid) {
            return;
        }
        this._loadingPanel.show(this.pageContentId);
        this._btnConfirm.set_enabled(false);

        if (this.isPageValid()) {
            let insertsArchiveChain: string = this.getInsertsArchiveChain();
            let ajaxModel: AjaxModel = <AjaxModel>{};
            ajaxModel.Value = new Array<string>();
            ajaxModel.ActionName = "Update";
            ajaxModel.Value = new Array<string>();
            ajaxModel.Value.push(insertsArchiveChain);
            this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
            return;
        }

        this._loadingPanel.hide(this.pageContentId);
        this._btnConfirm.set_enabled(true);
    }

    /**
     *------------------------- Methods -----------------------------
     */

    private initializeFascicle(): void {   
        this._loadingPanel.show(this.pageContentId);
        this.service.getFascicle(this.currentFascicleId,
            (data: any) => {
                if (data == null) return;

                this._fascicleModel = data;
                this.checkFascicleRight(this.currentFascicleId)
                    .done((isEditable: boolean) => {
                        if (!isEditable) {
                            this._loadingPanel.hide(this.pageContentId);
                            this.showNotificationMessage(this.uscNotificationId, `Fascicolo n. ${this._fascicleModel.Title}. Mancano diritti di modifica.`);
                            $("#".concat(this.pageContentId)).hide();
                            return;
                        }  

                        this.bindPageFromModel(this._fascicleModel);

                        let jsonFascicle: string = JSON.stringify(this._fascicleModel);
                        let ajaxModel: AjaxModel = <AjaxModel>{};
                        ajaxModel.Value = new Array<string>();
                        ajaxModel.ActionName = "Initialize";
                        ajaxModel.Value = new Array<string>();
                        ajaxModel.Value.push(jsonFascicle);

                        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
                    })
                    .fail((exception: ExceptionDTO) => {
                        this._loadingPanel.hide(this.pageContentId);
                        this.showNotificationException(this.uscNotificationId, exception);
                    });                                
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    private checkFascicleRight(idFascicle: string): JQueryPromise<boolean> {
        let promise: JQueryDeferred<boolean> = $.Deferred<boolean>();
        this.service.hasManageableRight(idFascicle,
            (data: any) => promise.resolve(!!data),
            (exception: ExceptionDTO) => promise.reject(exception));
        return promise.promise();
    }

    private bindPageFromModel(fascicle: FascicleModel): void {
        this._txtObject.set_value(fascicle.FascicleObject);
        this._txtNote.set_value(fascicle.Note);
        this._txtManager.set_value(fascicle.Manager);
        if (!this.isEditPage && !(this.fasciclesPanelVisibilities["FascicleNameVisibility"])) {
            $(`#${this.rowNameId}`).hide();
        }

        if (this._txtName) {
            this._txtName.set_value(fascicle.Name);
        }

        if (!this.isEditPage && !(this.fasciclesPanelVisibilities["FascicleRacksVisibility"])) {
            $(`#${this.rowRackId}`).hide();
        }
        this._txtRack.set_value(fascicle.Rack);


        if (fascicle.FascicleType != (<any>FascicleType)[FascicleType.Legacy]) {
            $(`#${this.rowLegacyManagerId}`).remove();
        }

        if (fascicle.FascicleType == (<any>FascicleType)[FascicleType.Activity]) {
            $(`#${this.rowManagerId}`).hide();
        }

        if (this.metadataRepositoryEnabled && fascicle.MetadataValues) {
            this._rowDynamicMetadata.show();
        }
    }

    /**
     * Inizializza lo user control del sommario di fascicolo
     */
    private loadFascicoloSummary(): void {
        let uscFascicolo: UscFascicolo = <UscFascicolo>$("#".concat(this.uscFascicoloId)).data();
        if (!jQuery.isEmptyObject(uscFascicolo)) {
            $("#".concat(this.uscFascicoloId)).bind(UscFascicolo.DATA_LOADED_EVENT, (args) => {
                this._loadingPanel.hide(this.pageContentId);
                this._btnConfirm.set_enabled(true);
            });
            uscFascicolo.loadData(this._fascicleModel);
        }
    }

    /**
     * Callback inizializzazione pagina
     */
    initializeCallback(): void {
        $("#".concat(this.uscFascicoloId)).bind(UscFascicolo.LOADED_EVENT, (args) => {
            this.loadFascicoloSummary();
        });
        this.loadFascicoloSummary();
    }

    /**
     * Metodo per la verifica della validità della pagina
     */
    isPageValid(): boolean {
        let txtObject: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>$find(this.txtObjectId);
        if (txtObject.get_maxLength() != 0 && txtObject.get_textBoxValue().length > txtObject.get_maxLength()) {
            this.showNotificationMessage(this.uscNotificationId, "Impossibile salvare.\nIl campo Oggetto ha superato i caratteri disponibili.\n(Caratteri ".concat(txtObject.get_textBoxValue().length.toString(), " Disponibili ", txtObject.get_maxLength().toString()));
            return false;
        }

        return true;
    }

    /**
     * Callback di modifica fascicolo
     * @param contact
     */
    updateCallback(contact: number, metadataModel: string): void {
        if (this._fascicleModel == null) {
            this._loadingPanel.hide(this.pageContentId);
            this._btnConfirm.set_enabled(true);
            this.showWarningMessage(this.uscNotificationId, "Nessun fascicolo definito per la modifica");
            return;
        }

        let txtObject: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>$find(this.txtObjectId);
        if (this._txtName) {
            this._fascicleModel.Name = this._txtName.get_value();
        }        
        this._fascicleModel.Rack = this._txtRack.get_value();
        this._fascicleModel.Note = this._txtNote.get_value();
        this._fascicleModel.FascicleObject = txtObject.get_value();

        if (this._fascicleModel.FascicleType == FascicleType.Legacy) {
            this._fascicleModel.Manager = this._txtManager.get_value();
        }

        if (this._fascicleModel.FascicleType != FascicleType.Activity && contact != null && contact != 0) {
            let contactModel: ContactModel = <ContactModel>{};
            contactModel.EntityId = contact;
            this._fascicleModel.Contacts.splice(0, this._fascicleModel.Contacts.length);
            this._fascicleModel.Contacts.push(contactModel);
        }

        if (!!metadataModel) {
            this._fascicleModel.MetadataValues = metadataModel;
        }

        this.service.updateFascicle(this._fascicleModel,null,
            (data: any) => {
                window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(this._fascicleModel.UniqueId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.pageContentId);
                this._btnConfirm.set_enabled(true);
                this.showNotificationException(this.uscNotificationId, exception);
            }
        );
    }

    /**
     * Recupera il record relativo agli Inserti in FascicleDocuments
     */
    getInsertsArchiveChain(): string {
        let insertsArchiveChain: string = ""
        let inserts: FascicleDocumentModel = $.grep(this._fascicleModel.FascicleDocuments, (x) => ChainType[x.ChainType.toString()] == ChainType.Miscellanea)[0];
        if (inserts != undefined) {
            insertsArchiveChain = inserts.IdArchiveChain;
        }
        return insertsArchiveChain;
    }
}

export = FascModifica;