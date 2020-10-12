/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import TemplateDocumentRepositoryModel = require('App/Models/Templates/TemplateDocumentRepositoryModel');
import TemplateDocumentRepositoryStatus = require('App/Models/Templates/TemplateDocumentRepositoryStatus');
import LocationModel = require('App/Models/Commons/LocationModel');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import TemplateDocumentRepositoryService = require('App/Services/Templates/TemplateDocumentRepositoryService');
import uscTemplateDocumentRepository = require('UserControl/uscTemplateDocumentRepository');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');


declare var Page_IsValid: any;
declare var ValidatorEnable: any;

class TbltTemplateDocumentRepositoryGes {
    action: string;
    currentTemplateId: string;    
    pnlMetadataId: string;
    txtNameId: string;
    txtObjectId: string;
    acbQualityTagId: string;
    racTagsDataSourceId: string;
    rfvNameId: string;
    pnlButtonsId: string;
    btnConfirmId: string;
    btnConfirmUniqueId: string;
    btnPublishId: string;
    btnPublishUniqueId: string;
    ajaxLoadingPanelId: string;
    ajaxFlatLoadingPanelId: string;
    ajaxManagerId: string;
    managerId: string;
    uscNotificationId: string;

    private _service: TemplateDocumentRepositoryService;
    private _templateDocumentRepositoryService: TemplateDocumentRepositoryService;
    private _txtName: Telerik.Web.UI.RadTextBox;
    private _txtObject: Telerik.Web.UI.RadTextBox;
    private _acbQualityTag: Telerik.Web.UI.RadAutoCompleteBox;
    private _racTagsDataSource: Telerik.Web.UI.RadClientDataSource;
    private _btnConfirm: Telerik.Web.UI.RadButton;
    private _btnPublish: Telerik.Web.UI.RadButton;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _currentTemplate: TemplateDocumentRepositoryModel;

    private static EDIT_ACTION: string = 'Edit';
    private static INSERT_ACTION: string = 'Insert';

    /**
  * Costruttore
  * @param serviceConfiguration
  */
    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "TemplateDocumentRepository");
        if (!serviceConfiguration) {
            this.showNotificationMessage(this.uscNotificationId, "Errore in inizializzazione. Nessun servizio configurato per il Deposito Documentale");
            return;
        }
        this._service = new TemplateDocumentRepositoryService(serviceConfiguration);
    }

    /**
    *------------------------- Events -----------------------------
    */

    /**
   * Evento scatenato al click del pulsante Conferma/SalvaBozza
   * @method
   * @param sender
   * @param eventArgs
   * @returns
   */
    btnConfirm_Clicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        if (Page_IsValid) {
            this.showLoadingPanel(this.pnlMetadataId);
            this.showFlatLoadingPanel();
            (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequestWithTarget(this.btnConfirmUniqueId, '');
        }
    }


    /**
     * Evento scatenato al click del pulsante Pubblica
     * @method
     * @param sender
     * @param eventArgs
     * @returns
     */
    btnPublish_Clicked = (sender: Telerik.Web.UI.RadButton, eventArgs: Telerik.Web.UI.ButtonEventArgs) => {
        if (Page_IsValid) {
            this.showLoadingPanel(this.pnlMetadataId);
            this.showFlatLoadingPanel();
            (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequestWithTarget(this.btnPublishUniqueId, '');
        }
    }

    /**
    *------------------------- Methods -----------------------------
    */

    /**
    * Metodo di inizializzazione
    */
    initialize() {
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._btnConfirm.add_clicked(this.btnConfirm_Clicked);
        this._btnPublish = <Telerik.Web.UI.RadButton>$find(this.btnPublishId);
        this._btnPublish.add_clicked(this.btnPublish_Clicked);
        this._txtName = <Telerik.Web.UI.RadTextBox>$find(this.txtNameId);
        this._txtObject = <Telerik.Web.UI.RadTextBox>$find(this.txtObjectId);
        this._acbQualityTag = <Telerik.Web.UI.RadAutoCompleteBox>$find(this.acbQualityTagId);
        this._racTagsDataSource = <Telerik.Web.UI.RadClientDataSource>$find(this.racTagsDataSourceId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.managerId);

        this.showLoadingPanel(this.pnlMetadataId);
        this.showFlatLoadingPanel();
        $.when(this.loadTags(), ((this.action == TbltTemplateDocumentRepositoryGes.EDIT_ACTION) ? this.loadTemplateData() : undefined)).fail((exception) => {
            this.showNotificationException(this.uscNotificationId, exception, "Errore nel caricamento del Template.");
        }).always(() => {
            this.closeLoadingPanel(this.pnlMetadataId);
            this.closeFlatLoadingPanel();
        });
    }


    /**
     * Esegue il caricamento dei Quality Tag per la funzionalità di inserimento tag
     */
    loadTags(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            this._service.getTags(
                (data: any) => {
                    try {
                        if (data == undefined) {
                            console.log("No quality tags found.")
                            promise.resolve();
                        }

                        let sourceModel: any[] = this.prepareTagsSourceModel(data.value);
                        (<any>this._racTagsDataSource).set_data(sourceModel);
                        this._racTagsDataSource.fetch(undefined);
                        promise.resolve();
                    } catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject("Errore durante il caricamento dei Tag");
                    }
                }, (exception: ExceptionDTO): void => {
                    promise.reject(exception);
                });
        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject("Errore durante il caricamento dei Tag");
        }
        return promise.promise();
    }

    /**
     * Carica i dati del template corrente e li visualizza nella pagina
     */
    loadTemplateData(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        try {
            this._service.getTemplateById(this.currentTemplateId,
                (data: any) => {
                    try {
                        if (data == undefined) {
                            console.log("No template found.")
                            promise.reject("Nessun template trovato con ID ".concat(this.currentTemplateId));
                        }

                        this.fillPageFromModel(data);
                        this._currentTemplate = data;
                        promise.resolve();
                    } catch (error) {
                        console.log(JSON.stringify(error));
                        promise.reject((<Error>error).message);
                    }
                }, (exception: ExceptionDTO): void => {
                    promise.reject(exception);
                });
        } catch (error) {
            console.log((<Error>error).stack);
            promise.reject("Errore in caricamento del template");
        }
        return promise.promise();
    }

    /**
    * Metodo che prepara il modello da passare al datasource della RadAutoCompleteBox
    * @param tags
    */
    prepareTagsSourceModel(tags: string[]): any {
        let models: any[] = new Array();
        $.each(tags, (index: number, tag: string) => {
            models.push({ id: tag, value: tag });
        });
        return models;
    }

    /**
    * Callback per l'inserimento/aggiornamento di un TemplateDocumentRepositoryModel
    * @param entity
    */
    confirmCallback(persistedChainId: string, toPublish?: boolean) {
        try {
            let entity: TemplateDocumentRepositoryModel = this.action == TbltTemplateDocumentRepositoryGes.EDIT_ACTION ? this._currentTemplate : <TemplateDocumentRepositoryModel>{};
            entity = this.fillModelFromPage(entity);
            entity.Version = this.action == TbltTemplateDocumentRepositoryGes.EDIT_ACTION ? entity.Version + 1 : 1;
            entity.IdArchiveChain = persistedChainId;
            if (toPublish != undefined) {
                entity.Status = (toPublish) ? TemplateDocumentRepositoryStatus.Available : TemplateDocumentRepositoryStatus.Draft;
            }

            let apiAction: any = this.action == 'Insert' ? (m, c, e) => this._service.insertTemplateDocument(m, c, e) : (m, c, e) => this._service.updateTemplateDocument(m, c, e);
            apiAction(entity,
                this.closeCallback,
                (exception: ExceptionDTO) => {
                    this.showNotificationException(this.uscNotificationId, exception);
                    this.closeLoadingPanel(this.pnlMetadataId);
                    this.closeFlatLoadingPanel();
                });
        }
        catch (error) {
            this.closeLoadingPanel(this.pnlMetadataId);
            this.closeFlatLoadingPanel();
            this.showNotificationMessage(this.uscNotificationId, "Errore in esecuzione dell'attività di salvataggio.");
            console.log(JSON.stringify(error));
        }
    }

    /**
     * Callback per chiusura inserimento
     * @param entity
     */
    closeCallback = (entity: TemplateDocumentRepositoryModel) => {
        this.closeLoadingPanel(this.pnlMetadataId);
        this.closeFlatLoadingPanel();
        this.closeWindow(entity);
    }

    /**
     * Callback in caso di errore
     * @param entity
     */
    errorCallback = () => {
        this.closeLoadingPanel(this.pnlMetadataId);
        this.closeFlatLoadingPanel();
    }

    /**
     * Esegue il fill dei controlli della pagina in  modello TemplateDocumentRepositoryModel in inserimento
     */
    private fillModelFromPage(model: TemplateDocumentRepositoryModel): TemplateDocumentRepositoryModel {
        model.Name = this._txtName.get_value();
        model.Object = this._txtObject.get_value();
        model.QualityTag = this.getTagEntries();
        return model;
    }

    /**
     * Esegue il fill di un template nella pagina
     * @param model
     */
    private fillPageFromModel(model: TemplateDocumentRepositoryModel): TemplateDocumentRepositoryModel {
        this._txtName.set_value(model.Name);
        this._txtObject.set_value(model.Object);
        let tags: string[] = model.QualityTag.split(";");
        $.each(tags, (index: number, tag: string) => {
            if (!tag) {
                return;
            }

            let entry: Telerik.Web.UI.AutoCompleteBoxEntry = new Telerik.Web.UI.AutoCompleteBoxEntry();
            (<any>entry).set_text(tag);
            this._acbQualityTag.get_entries().add(entry);
        });

        return model;
    }

    /**
     * Recupera gli elementi inseriti nel controllo relativo ai tag
     */
    getTagEntries(): string {
        let qualityTags: string = '';
        let entries: Telerik.Web.UI.AutoCompleteBoxEntryCollection = this._acbQualityTag.get_entries();
        for (let i: number = 0; i < entries.get_count(); i++) {
            qualityTags = qualityTags.concat(entries.getEntry(i).get_text(), ';');
        }

        return qualityTags;
    }

    /**
     * Recupera una RadWindow dalla pagina
     */
    getRadWindow(): Telerik.Web.UI.RadWindow {
        let wnd: Telerik.Web.UI.RadWindow = null;
        if ((<any>window).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) wnd = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return wnd;
    }

    /**
     * Chiude la RadWindow
     */
    closeWindow(entity: TemplateDocumentRepositoryModel): void {
        let wnd: Telerik.Web.UI.RadWindow = this.getRadWindow();
        wnd.close(JSON.stringify(entity));
    }

    /**
     * Visualizza nuovi loading panel nella pagina
     */
    private showLoadingPanel(updatedElementId: string): void {
        let ajaxDefaultLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        ajaxDefaultLoadingPanel.show(updatedElementId);
    }

    /**
     * Nasconde i loading panel nella pagina 
     */
    private closeLoadingPanel(updatedElementId: string): void {
        let ajaxDefaultLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        ajaxDefaultLoadingPanel.hide(updatedElementId);
    }

    /**
    * Visualizza flat loading panel sul pannello bottoni
    */
    private showFlatLoadingPanel(): void {
        let ajaxDefaultFlatLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxFlatLoadingPanelId);
        ajaxDefaultFlatLoadingPanel.show(this.pnlButtonsId);
    }

    /**
    * Nasconde flat loading panel sul pannello bottoni
    */
    private closeFlatLoadingPanel(): void {
        let ajaxDefaultFlatLoadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxFlatLoadingPanelId);
        ajaxDefaultFlatLoadingPanel.hide(this.pnlButtonsId);
    }

    /**
     * Resetta lo stato dei controlli
     */
    resetControls(): any {
        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
        this._btnConfirm.add_clicked(this.btnConfirm_Clicked);
        this._btnPublish = <Telerik.Web.UI.RadButton>$find(this.btnPublishId);
        this._btnPublish.add_clicked(this.btnPublish_Clicked);
    }

    protected showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(uscNotificationId, customMessage)
        }

    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage)
        }
    }

}

export = TbltTemplateDocumentRepositoryGes;