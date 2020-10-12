import DossierModel = require('App/Models/Dossiers/DossierModel');
import ContainerModel = require('App/Models/Commons/ContainerModel');
import ContactModel = require('App/Models/Commons/ContactModel');
import BaseEntityViewModel = require('App/ViewModels/BaseEntityViewModel');
import DossierSummaryViewModel = require('App/ViewModels/Dossiers/DossierSummaryViewModel');
import AjaxModel = require('App/Models/AjaxModel');
import DossierBase = require('Dossiers/DossierBase');
import DossierService = require('App/Services/Dossiers/DossierService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import DossierDocumentService = require('App/Services/Dossiers/DossierDocumentService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscDynamicMetadataRest = require('UserControl/uscDynamicMetadataRest');
import uscSetiContactSel = require('UserControl/uscSetiContactSel');
import UscMetadataRepositorySel = require('UserControl/uscMetadataRepositorySel');
import SetiContactModel = require('App/Models/Commons/SetiContactModel');
import MetadataDesignerViewModel = require('App/ViewModels/Metadata/MetadataDesignerViewModel');
import uscContattiSelRest = require('UserControl/uscContattiSelRest');
import PageClassHelper = require('App/Helpers/PageClassHelper');
import DossierStatus = require('App/Models/Dossiers/DossierStatus');
import EnumHelper = require('App/Helpers/EnumHelper');
import DossierType = require('App/Models/Dossiers/DossierType');

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
    setiContactEnabledId: boolean;
    uscSetiContactSelId: string;
    uscContattiSelRestId: string;
    contacts: ContactModel[] = [];
    contactInsertId: number[] = [];
    rcbDossierStatusId: string;

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
    private _uscContattiSelRest: uscContattiSelRest;
    private _rcbDossierStatus: Telerik.Web.UI.RadComboBox;
    private _enumHelper: EnumHelper;
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
        this._enumHelper = new EnumHelper();
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
        this._rcbDossierStatus = <Telerik.Web.UI.RadComboBox>$find(this.rcbDossierStatusId);
        this._btnConfirm.set_enabled(false);
        this._loadingPanel.show(this.dossierPageContentId);
        this._rowMetadataRepository = $("#".concat(this.rowMetadataId));
        this._rowMetadataRepository.hide();

        this._uscContattiSelRest = <uscContattiSelRest>$(`#${this.uscContattiSelRestId}`).data();

        (<DossierService>this.service).hasModifyRight(this.currentDossierId,
            (data: any) => {
                if (data == null) return;
                if (data) {
                    $.when(this.loadDossier(), this.loadContacts()).done(() => {
                        this._DossierModel.Contacts = this._DossierContacts;
                        this.fillPageFromModel(this._DossierModel);
                        if (this.metadataRepositoryEnabled) {
                            this.loadMetadata(this._DossierModel.MetadataDesigner, this._DossierModel.MetadataValues);
                        }
                        if (this._DossierModel && this._DossierModel.MetadataDesigner) {
                            let metadata: MetadataDesignerViewModel = JSON.parse(this._DossierModel.MetadataDesigner);
                            if (metadata && metadata.SETIFieldEnabled) {
                                $("#".concat(this.uscSetiContactSelId)).triggerHandler(uscSetiContactSel.SHOW_SETI_CONTACT_BUTTON, metadata.SETIFieldEnabled && this.setiContactEnabledId);
                            }
                        }

                        this.registerUscContactRestEventHandlers();
                        this._btnConfirm.set_enabled(true);
                        this._btnConfirm.add_clicked(this.btnConfirm_clicked);
                        this._loadingPanel.hide(this.dossierPageContentId);
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

        /*event for filing out the fields with the chosen Seti contact*/
        $("#".concat(this.uscDynamicMetadataId)).on(UscMetadataRepositorySel.SELECTED_SETI_CONTACT_EVENT, (sender, args: SetiContactModel) => {
            let uscDynamicMetadataRest: UscDynamicMetadataRest = <UscDynamicMetadataRest>$("#".concat(this.uscDynamicMetadataId)).data();
            uscDynamicMetadataRest.populateMetadataRepository(args, this._DossierModel.MetadataDesigner);
        });

    }

    btnConfirm_clicked = (sender: Telerik.Web.UI.RadButton, args: Telerik.Web.UI.ButtonEventArgs) => {
        this.updateCallback();
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
                        for (let contact of this._DossierContacts) {
                            let newContact: ContactModel = <ContactModel>{
                                UniqueId: contact.UniqueId,
                                EntityId: contact.EntityShortId,
                                Description: contact.Name,
                                IdContactType: contact.Type,
                                IncrementalFather: contact.IncrementalFather
                            };
                            this.contactInsertId.push(newContact.EntityId);
                            this.contacts.push(newContact);
                        }
                        PageClassHelper.callUserControlFunctionSafe<uscContattiSelRest>(this.uscContattiSelRestId)
                            .done((instance) => instance.renderContactsTree(this.contacts));

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

        (<DossierService>this.service).allFasciclesAreClosed(model.UniqueId, (data: boolean) => {
            this.populateDossierStatusComboBox(data, model.Status);
        }, (exception: ExceptionDTO) => {
            console.error(exception);
        });

        let txtObject: Telerik.Web.UI.RadTextBox = <Telerik.Web.UI.RadTextBox>$find(this.txtObjectId);
        txtObject.set_value(model.Subject);
        this._txtNote.set_value(model.Note);

        let ajaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();
        ajaxModel.Value.push(JSON.stringify(model.Contacts));
        ajaxModel.ActionName = "LoadExternalData";

        (<Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId)).ajaxRequest(JSON.stringify(ajaxModel));
    }

    private registerUscContactRestEventHandlers(): void {
        PageClassHelper.callUserControlFunctionSafe<uscContattiSelRest>(this.uscContattiSelRestId)
            .done((instance) => {
                instance.registerEventHandler(instance.uscContattiSelRestEvents.ContactDeleted, (contactIdToDelete: number) => {
                    this.contactInsertId = this.contactInsertId.filter(x => x != contactIdToDelete);
                    return $.Deferred<void>().resolve();
                });
                instance.registerEventHandler(instance.uscContattiSelRestEvents.NewContactsAdded, (newAddedContact: ContactModel) => {
                    this.contactInsertId.push(newAddedContact.EntityId);
                    return $.Deferred<void>().resolve();
                });
            });
    }

    /**
    * Callback da code-behind per la modifica di un Dossier
    * @param contact
    * @param category
    */
    updateCallback(): void {
        let dossierModel: DossierModel = <DossierModel>{};

        //riferimento
        this.fillContacts(JSON.stringify(this.contactInsertId), dossierModel);
        this.fillModelFromPage(dossierModel);

        if (this.metadataRepositoryEnabled) {
            let uscDynamicMetadataRest: UscDynamicMetadataRest = <UscDynamicMetadataRest>$("#".concat(this.uscDynamicMetadataId)).data();
            if (!jQuery.isEmptyObject(uscDynamicMetadataRest)) {
                let metadata: MetadataDesignerViewModel = JSON.parse(this._DossierModel.MetadataDesigner);
                if (metadata) {
                    let setiIntegrationField = metadata.SETIFieldEnabled;
                    let result = uscDynamicMetadataRest.bindModelFormPage(setiIntegrationField);
                    if (!result) {
                        this._btnConfirm = <Telerik.Web.UI.RadButton>$find(this.btnConfirmId);
                        this._btnConfirm.set_enabled(true);
                        return;
                    }
                    dossierModel.MetadataDesigner = result[0];
                    dossierModel.MetadataValues = result[1];
                }
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
        model.DossierType = DossierType[this._DossierModel.DossierType];

        let containerModel: ContainerModel = <ContainerModel>{};
        containerModel.EntityShortId = Number(this._DossierModel.ContainerId);
        model.Container = containerModel;

        let selectedDate = new Date(this._rdpStartDate.get_selectedDate().getTime() - this._rdpStartDate.get_selectedDate().getTimezoneOffset() * 60000);
        model.StartDate = selectedDate;

        model.Status = DossierStatus[this._DossierModel.Status];
        let selectedDossierStatus: string = this._rcbDossierStatus.get_selectedItem().get_value();
        if (selectedDossierStatus) {
            model.Status = DossierStatus[selectedDossierStatus];
        }

        return model;
    }


    private loadMetadata(metadatas: string, metadataValues: string) {
        if (metadatas) {
            this._rowMetadataRepository.show();
            let uscDynamicMetadataRest: UscDynamicMetadataRest = <UscDynamicMetadataRest>$("#".concat(this.uscDynamicMetadataId)).data();
            if (!jQuery.isEmptyObject(uscDynamicMetadataRest)) {
                uscDynamicMetadataRest.loadPageItems(metadatas, metadataValues);
            }
        }
    }

    private populateDossierStatusComboBox(allFasciclesAreClosed: boolean, currentDossierStatus: string): void {
        let rcbItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
        rcbItem.set_text("");
        this._rcbDossierStatus.get_items().add(rcbItem);
        for (let dossierStatus in DossierStatus) {
            //if I have at least one fascicle opened I don't add Closed option in combobox
            if (dossierStatus === DossierStatus.Closed.toString() && !allFasciclesAreClosed) {
                continue;
            }
            if (typeof DossierStatus[dossierStatus] === 'string' && dossierStatus !== DossierStatus[currentDossierStatus].toString()) {
                let rcbItem: Telerik.Web.UI.RadComboBoxItem = new Telerik.Web.UI.RadComboBoxItem();
                rcbItem.set_text(this._enumHelper.getDossierStatusDescription(DossierStatus[dossierStatus]));
                rcbItem.set_value(DossierStatus[dossierStatus]);
                this._rcbDossierStatus.get_items().add(rcbItem);
            }
        }
        this._rcbDossierStatus.get_items().getItem(0).select();
    }

}

export = DossierModifica;