/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />
/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />

import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import ConservationService = require("App/Services/Commons/ConservationService");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");
import ConservationModel = require("App/Models/Commons/ConservationModel");
import ConservationStatusType = require("App/Models/Commons/ConservationStatusType");
import UscErrorNotification = require("UserControl/uscErrorNotification");
import DocumentUnitService = require("App/Services/DocumentUnits/DocumentUnitService");
import DocumentUnitModel = require("App/Models/DocumentUnits/DocumentUnitModel");

class uscDocumentUnitConservationRest {
    pnlMainContentId: string;
    idDocumentUnit: string;
    imgConservationId: string;
    lblConservationDescriptionId: string;
    lblArchivedDateId: string;
    lblParerUriId: string;
    lblHasErrorId: string;
    lblLastErrorId: string;
    windowConservationDetailsId: string;
    uscNotificationId: string;
    imgConservationInfoId: string;

    static LOADED_EVENT: string = "onLoaded";

    private readonly _service: ConservationService;
    private readonly _documentUnitService: DocumentUnitService;
    private _windowConservationDetails: Telerik.Web.UI.RadWindow;

    private _imgConservationStatus(): JQuery {
        return $(`#${this.imgConservationId}`);
    }

    private _lblConservationDescription(): JQuery {
        return $(`#${this.lblConservationDescriptionId}`);
    }

    private _lblArchivedDate(): JQuery {
        return $(`#${this.lblArchivedDateId}`);
    }

    private _lblParerUri(): JQuery {
        return $(`#${this.lblParerUriId}`);
    }

    private _lblHasError(): JQuery {
        return $(`#${this.lblHasErrorId}`);
    }

    private _lblLastError(): JQuery {
        return $(`#${this.lblLastErrorId}`);
    }

    private _imgConservationInfo(): JQuery {
        return $(`#${this.imgConservationInfoId}`);
    }

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        let serviceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Conservation");
        this._service = new ConservationService(serviceConfiguration);

        let documentUnitServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "DocumentUnit");
        this._documentUnitService = new DocumentUnitService(documentUnitServiceConfiguration);
    }

    /**
    *------------------------- Events -----------------------------
    */
    openConservationDetails = (sender: any): void => {
        this._windowConservationDetails.show();
        this._windowConservationDetails.center();
    }

    /**
    *------------------------- Methods -----------------------------
    */
    initialize(): void {
        this._windowConservationDetails = $find(this.windowConservationDetailsId) as Telerik.Web.UI.RadWindow;
        this.loadConservationInfos()
            .fail((exception: ExceptionDTO) => {
                this.showNotificationException(exception);
            })
            .always(() => this.bindLoaded());
    }

    bindLoaded(): void {
        $(`#${this.pnlMainContentId}`).data(this);
        $(`#${this.pnlMainContentId}`).triggerHandler(uscDocumentUnitConservationRest.LOADED_EVENT);
    }

    loadConservationInfos(): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        this._service.getById(this.idDocumentUnit,
            (data: any) => {
                let conservation: ConservationModel = data as ConservationModel;
                if (!conservation) {
                    this._imgConservationStatus().attr("src", "../Comm/images/parer/lightgray.png");
                    this._lblConservationDescription().text("Documento non soggetto a versamento.");
                    this._imgConservationInfo().hide();
                    return promise.resolve();
                }

                this._documentUnitService.getDocumentUnitById(this.idDocumentUnit,
                    (data: any) => {
                        try {
                            let documentUnit: DocumentUnitModel = data as DocumentUnitModel;
                            this.setConservationStatus(conservation);
                            this.setWindowDetails(conservation, documentUnit);
                            promise.resolve();
                        } catch (error) {
                            console.error(error);
                            let ex: ExceptionDTO = new ExceptionDTO();
                            ex.statusText = "E' avvenuto un errore durante il recupero delle informazioni di conservazione per l'unità documentaria corrente." 
                            promise.reject(ex);
                        }                        
                    },
                    (exception: ExceptionDTO) => promise.reject(exception)
                );                
            },
            (exception: ExceptionDTO) => {
                promise.reject(exception);
            }
        )
        return promise.promise();
    }

    private setConservationStatus(conservation: ConservationModel): void {
        if (conservation.Status == ConservationStatusType.Conservated && !conservation.Message) {
            this._imgConservationStatus().attr("src", "../Comm/images/parer/green.png");
            this._lblConservationDescription().html("Conservazione corretta.");
            return;
        }

        if (conservation.Status == ConservationStatusType.Conservated && conservation.Message) {
            this._imgConservationStatus().attr("src", "../Comm/images/parer/yellow.png");
            this._lblConservationDescription().html("Conservazione con avviso.");
            return;
        }

        this._imgConservationStatus().attr("src", "../Comm/images/parer/red.png");
        this._lblConservationDescription().html("Conservazione con errori.");
    }

    private setWindowDetails(conservation: ConservationModel, documentUnit: DocumentUnitModel): void {
        this._windowConservationDetails.set_title(`Dettaglio conservazione ${documentUnit.DocumentUnitName} ${documentUnit.Title}`);
        if (conservation.SendDate) {
            this._lblArchivedDate().html(`${moment(conservation.SendDate).format("L")} ${moment(conservation.SendDate).format("LTS")}`);
        }
        this._lblParerUri().html(conservation.Uri);
        this._lblHasError().html(conservation.Status == ConservationStatusType.Error ? "sì" : "no");
        this._lblLastError().html(conservation.Message);
    }

    protected showNotificationException(exception: ExceptionDTO, customMessage?: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${this.uscNotificationId}`).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            if (exception) {
                uscNotification.showNotification(exception);
                return;
            }
            uscNotification.showWarningMessage(customMessage);
        }
    }
}

export = uscDocumentUnitConservationRest;