/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <amd-dependency path="../app/core/extensions/string" />

import FascicleModel = require('App/Models/Fascicles/FascicleModel');
import FascicleRoleModel = require('App/Models/Fascicles/FascicleRoleModel');
import RoleModel = require('App/Models/Commons/RoleModel');
import CategoryModel = require('App/Models/Commons/CategoryModel');
import ContactModel = require('App/Models/Commons/ContactModel');
import FascicleType = require('App/Models/Fascicles/FascicleType');
import VisibilityType = require('App/Models/Fascicles/VisibilityType')
import AuthorizationRoleType = require('App/Models/Commons/AuthorizationRoleType');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import FascicleBase = require('Fasc/FascBase');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ChainType = require('App/Models/DocumentUnits/ChainType');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import DocumentUnitModel = require('App/Models/DocumentUnits/DocumentUnitModel');
import ProtocolModel = require('App/Models/Protocols/ProtocolModel');
import FascicleReferenceType = require('App/Models/Fascicles/FascicleReferenceType');
import FascicleDocumentUnitModel = require('App/Models/Fascicles/FascicleDocumentUnitModel');
import ResolutionModel = require('App/Models/Resolutions/ResolutionModel');
import Environment = require('App/Models/Environment');
import IFascicolableBaseModel = require('App/Models/Fascicles/IFascicolableBaseModel');
import IFascicolableBaseService = require('App/Services/Fascicles/IFascicolableBaseService');
import FascicleDocumentUnitService = require('App/Services/Fascicles/FascicleDocumentUnitService');

import UDSRepositoryModel = require('App/Models/UDS/UDSRepositoryModel');
import ValidationExceptionDTO = require('App/DTOs/ValidationExceptionDTO');
import ValidationMessageDTO = require('App/DTOs/ValidationMessageDTO');
import UscFascicleInsert = require('UserControl/uscFascicleInsert');
import AjaxModel = require('App/Models/AjaxModel');
import MetadataRepositoryModel = require('App/Models/Commons/MetadataRepositoryModel');
import FascicolableActionType = require('App/Models/FascicolableActionType');

declare var Page_IsValid: any;
class FascInserimento extends FascicleBase {
    btnInserimentoId: string;
    ajaxLoadingPanelId: string;
    ajaxManagerId: string;
    radWindowManagerId: string;
    uscNotificationId: string;
    fasciclePageContentId: string;
    webAPIUrl: string;
    currentUDId: string;
    environment: string;
    currentIdUDSRepository: string;
    activityFascicleEnabled: boolean;
    fasciclesPanelVisibilities: { [key: string]: boolean };
    uscFascInsertId: string;
    fascicleTypeRowId: string;

    private _btnInserimento: Telerik.Web.UI.RadButton;
    private _loadingPanel: Telerik.Web.UI.RadAjaxLoadingPanel;
    private _manager: Telerik.Web.UI.RadWindowManager;
    private _serviceConfigurations: ServiceConfiguration[];
    private _ajaxManager: Telerik.Web.UI.RadAjaxManager;


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
     * Initialize
     */
    initialize(): void {
        super.initialize();
        this._btnInserimento = <Telerik.Web.UI.RadButton>$find(this.btnInserimentoId);
        this._btnInserimento.add_clicking(this.btnInserimento_OnClick);

        this._loadingPanel = <Telerik.Web.UI.RadAjaxLoadingPanel>$find(this.ajaxLoadingPanelId);
        this._ajaxManager = <Telerik.Web.UI.RadAjaxManager>$find(this.ajaxManagerId);
        this._manager = <Telerik.Web.UI.RadWindowManager>$find(this.radWindowManagerId);
        $("#".concat(this.uscFascInsertId)).bind(UscFascicleInsert.LOADED_EVENT, (args) => {
            this.onCreateNewFascicle();
        });
        $("#".concat(this.uscFascInsertId)).bind(UscFascicleInsert.FASCICLE_TYPE_CHANGED_EVENT, (args) => {
            this._btnInserimento.set_enabled(true);
        });
        this.onCreateNewFascicle();
    }

    /**
     *------------------------- Events -----------------------------
     */

    /**
     * Evento alla partenza di una request ajax
     * @param sender
     * @param args
     */
    onRequestStart = (sender: any, args: Sys.EventArgs) => {
        this._btnInserimento.set_enabled(false);
    }

    /**
     * Evento al termine di una request ajax
     * @param sender
     * @param args
     */
    onResponseEnd = (sender: any, args: Sys.EventArgs) => {
        this._btnInserimento.set_enabled(true);
    }

    /**
     * Evento scatenato al click del pulsante di inserimento
     * @param sender
     * @param args
     */
    btnInserimento_OnClick = (sender: any, args: Telerik.Web.UI.RadButtonCancelEventArgs) => {
        if (!Page_IsValid) {
            args.set_cancel(true);
            return;
        }
        this._btnInserimento.set_enabled(false);
        this._loadingPanel.show(this.fasciclePageContentId);
        let isFascValid: boolean = false;
        let uscFascInsert: UscFascicleInsert = <UscFascicleInsert>$("#".concat(this.uscFascInsertId)).data();
        if (!jQuery.isEmptyObject(uscFascInsert)) {
            isFascValid = uscFascInsert.isPageValid();
            let selectedFascicleType = uscFascInsert.getSelectedFascicleType();
            if (String.isNullOrEmpty(selectedFascicleType)) {
                this.showNotificationMessage(this.uscNotificationId, 'Selezionare una tipologia di fascicolo');
            }
        }

        let ajaxModel: AjaxModel = <AjaxModel>{};
        ajaxModel.Value = new Array<string>();
        ajaxModel.ActionName = "Insert";
        this._ajaxManager.ajaxRequest(JSON.stringify(ajaxModel));
        args.set_cancel(true);
    }

    /**
     *------------------------- Methods -----------------------------
     */

    onCreateNewFascicle = () => {

        let uscFascInsert: UscFascicleInsert = <UscFascicleInsert>$("#".concat(this.uscFascInsertId)).data();
        if (!jQuery.isEmptyObject(uscFascInsert)) {
            uscFascInsert.enableValidators(true);
            if ((this.environment && this.currentUDId) || !this.activityFascicleEnabled) {
                uscFascInsert.setProcedureTypeSelected();
            }
        }
        $("#".concat(this.fascicleTypeRowId)).show();
    }

    /**
     * Metodo per il recupero di una specifica radwindow
     */
    getRadWindow(): Telerik.Web.UI.RadWindow {
        let radWindow: Telerik.Web.UI.RadWindow;
        if ((<any>window).radWindow) radWindow = <Telerik.Web.UI.RadWindow>(<any>window).radWindow;
        else if ((<any>window.frameElement).radWindow) radWindow = <Telerik.Web.UI.RadWindow>(<any>window.frameElement).radWindow;
        return radWindow;
    }

    /**
     * Metodo di chiusura di una radwindow
     * @param callback
     */
    closeWindow(callback: any): void {
        let radWindow: Telerik.Web.UI.RadWindow = this.getRadWindow();
        if (radWindow != null) radWindow.close(callback);
    }

    /**
     * Callback da code-behind per l'inserimento di un fascicolo
     * @param contact
     * @param category
     */
    insertCallback(responsibleContact: number, metadataModel: string): void {
        let uscFascInsert: UscFascicleInsert = <UscFascicleInsert>$("#".concat(this.uscFascInsertId)).data();
        if (!jQuery.isEmptyObject(uscFascInsert)) {
            let fascicleModel: FascicleModel = new FascicleModel();
            fascicleModel = uscFascInsert.getFascicle();

            if (fascicleModel.FascicleType != FascicleType.Activity) {
                let contactModel: ContactModel = <ContactModel>{};
                contactModel.EntityId = responsibleContact;
                fascicleModel.Contacts.push(contactModel);
            }

            if (!!metadataModel) {
                fascicleModel.MetadataValues = metadataModel;
                if (sessionStorage.getItem("MetadataRepository")) {
                    let metadataRepository: MetadataRepositoryModel = new MetadataRepositoryModel();
                    metadataRepository.UniqueId = sessionStorage.getItem("MetadataRepository");
                    fascicleModel.MetadataRepository = metadataRepository;
                }
            }

            this.service.insertFascicle(fascicleModel,
                (data: FascicleModel) => {

                    if (this.currentUDId && this.environment) {
                        fascicleModel.UniqueId = data.UniqueId;
                        this.insertFascicolableUD(fascicleModel);
                    }
                    else {
                        window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(data.UniqueId);
                    }
                },
                (exception: ExceptionDTO) => {
                    this._loadingPanel.hide(this.fasciclePageContentId);
                    this._btnInserimento.set_enabled(true);
                    this.showNotificationException(this.uscNotificationId, exception);
                }
            );
        }
    }

    insertFascicolableUD(fascicleModel: FascicleModel) {
        let fascicolableService: IFascicolableBaseService<IFascicolableBaseModel>;

        let fascicleDocumentUnitModel = new FascicleDocumentUnitModel();
        fascicleDocumentUnitModel.ReferenceType = FascicleReferenceType.Fascicle;
        fascicleDocumentUnitModel.DocumentUnit = <DocumentUnitModel>{ UniqueId: this.currentUDId };
        fascicleDocumentUnitModel.Fascicle = fascicleModel;
        let fascicleDocumentUnitConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, FascicleBase.FASCICLE_DOCUMENTUNIT_TYPE_NAME);
        fascicolableService = new FascicleDocumentUnitService(fascicleDocumentUnitConfiguration); 
        this.insertUD(fascicleDocumentUnitModel, fascicolableService, fascicleModel.UniqueId);
    }

    insertUD(model: IFascicolableBaseModel, service: IFascicolableBaseService<IFascicolableBaseModel>, fascicleId: string) {
        service.insertFascicleUD(model, FascicolableActionType.AutomaticDetection,
            (data: IFascicolableBaseModel) => {
                window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(fascicleId);
            },
            (exception: ExceptionDTO) => {
                this._loadingPanel.hide(this.fasciclePageContentId);
                this.excpetionWindow(fascicleId, exception);
            }
        );
    }


    excpetionWindow(uniqueId: string, exception: ExceptionDTO) {
        let message: string = "Attenzione: il fascicolo è stato creato correttamente ma sono occorsi degli errori in fase di fascicolazione del documento.<br /> <br />";
        if (exception && exception instanceof ValidationExceptionDTO && exception.validationMessages.length > 0) {
            message = message.concat("Gli errori sono i seguenti: <br />");
            exception.validationMessages.forEach(function (item: ValidationMessageDTO) {
                message = message.concat(item.message, "<br />");
            })
        }

        message = message.concat("Proseguire con la visualizzazione del sommario del fascicolo?");

        this._manager.radconfirm(message,
            (arg) => {
                if (arg) {
                    this._loadingPanel.show(this.fasciclePageContentId);
                    window.location.href = "../Fasc/FascVisualizza.aspx?Type=Fasc&IdFascicle=".concat(uniqueId);
                }
            }, 300, 160);
    }

}

export = FascInserimento;