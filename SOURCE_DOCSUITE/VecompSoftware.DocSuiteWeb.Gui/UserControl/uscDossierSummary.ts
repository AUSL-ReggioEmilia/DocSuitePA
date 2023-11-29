import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import DossierService = require('App/Services/Dossiers/DossierService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import DossierSummaryViewModel = require('App/ViewModels/Dossiers/DossierSummaryViewModel');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import EnumHelper = require('App/Helpers/EnumHelper');

class uscDossierSummary {
    lblDossierSubjectId: string;
    lblStartDateId: string;
    lblDossierNoteId: string;
    lblNumberId: string;
    lblYearId: string;
    lblDossierTypeId: string;
    lblDossierStatusId: string;
    uscNotificationId: string;
    currentDossierId: string;
    pageId: string;
    lcDossierStatusKeyId: string;
    lcDossierStatusValueId: string;
    dossierStatusEnabled: boolean;

    private _lblDossierSubject: JQuery;
    private _lblStartDate: JQuery;
    private _lblDossierNote: JQuery;
    private _lblNumber: JQuery;
    private _lblYear: JQuery;
    private _lblDossierType: JQuery;
    private _lblDossierStatus: JQuery;
    private _serviceConfigurations: ServiceConfiguration[]
    private _DossierModel: DossierSummaryViewModel;
    private _dossierService: DossierService;
    private _enumHelper: EnumHelper;

    public static DOSSIER_TITLE: string;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
        this._enumHelper = new EnumHelper();
    }

    initialize(): void {
        this._lblDossierSubject = $("#".concat(this.lblDossierSubjectId));
        this._lblStartDate = $("#".concat(this.lblStartDateId));
        this._lblDossierNote = $("#".concat(this.lblDossierNoteId));
        this._lblYear = $("#".concat(this.lblYearId));
        this._lblNumber = $("#".concat(this.lblNumberId));
        this._lblDossierType = $(`#${this.lblDossierTypeId}`);
        this._lblDossierStatus = $(`#${this.lblDossierStatusId}`);

        $(`#${this.lcDossierStatusKeyId}`).hide();
        $(`#${this.lcDossierStatusValueId}`).hide();

        let serviceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, "Dossier");
        this._dossierService = new DossierService(serviceConfiguration);

        $(`#${this.pageId}`).data(this);
    }

    loadDossierSummary(dossierId: string): JQueryPromise<void> {
        let promise: JQueryDeferred<void> = $.Deferred<void>();
        this._dossierService.getDossier(dossierId,
            (data: any) => {
                this._DossierModel = data;

                uscDossierSummary.DOSSIER_TITLE = `${this._DossierModel.Year}/${this.pad(+this._DossierModel.Number, 7)}`;
                this._lblDossierSubject.html(this._DossierModel.Subject);
                this._lblDossierNote.html(this._DossierModel.Note);
                this._lblYear.html(this._DossierModel.Year.toString());
                this._lblNumber.html(this._DossierModel.Number);
                this._lblStartDate.html(this._DossierModel.FormattedStartDate);
                this._lblDossierType.html(this._enumHelper.getDossierTypeDescription(this._DossierModel.DossierType)); if (this.dossierStatusEnabled) {
                    $(`#${this.lcDossierStatusKeyId}`).show();
                    $(`#${this.lcDossierStatusValueId}`).show();
                    this._lblDossierStatus.html(this._enumHelper.getDossierStatusDescription(this._DossierModel.Status));
                }
                promise.resolve();
            },
            (exception: ExceptionDTO): void => {
                this.showNotificationException(this.uscNotificationId, exception);
                promise.reject(exception);
            });
        return promise.promise();
    }

    protected showNotificationException(uscNotificationId: string, exception: ExceptionDTO, customMessage?: string) {
        if (exception && exception instanceof ExceptionDTO) {
            let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${uscNotificationId}`).data();
            if (!jQuery.isEmptyObject(uscNotification)) {
                uscNotification.showNotification(exception);
            }
        }
        else {
            this.showNotificationMessage(uscNotificationId, customMessage)
        }
    }

    protected showNotificationMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$(`#${uscNotificationId}`).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showNotificationMessage(customMessage);
        }
    }

    private pad(currentNumber: number, paddingSize: number): string {
        let s = currentNumber + "";
        while (s.length < paddingSize) {
            s = `0${s}`
        }
        return s;
    }
}
export = uscDossierSummary;