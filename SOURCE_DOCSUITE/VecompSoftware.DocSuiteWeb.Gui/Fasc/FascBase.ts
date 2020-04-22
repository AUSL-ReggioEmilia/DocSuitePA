/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import FascicleService = require('App/Services/Fascicles/FascicleService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');

abstract class FascBase {
    protected static FASCICLE_TYPE_NAME = "Fascicle";
    protected static FASCICLEROLE_TYPE_NAME = 'FascicleRole';
    protected static DOCUMENT_UNIT_TYPE_NAME = "DocumentUnit";
    protected static PROTOCOL_TYPE_NAME = "Protocol";
    protected static RESOLUTION_TYPE_NAME = "Resolution";
    protected static UDSREPOSITORY_TYPE_NAME = "UDSRepository";
    protected static FASCICLE_DOCUMENTUNIT_TYPE_NAME = "FascicleDocumentUnit";
    protected static FASCICLE_LINK_TYPE_NAME = "FascicleLink";
    protected static FASCICLE_LOG_TYPE_NAME = "FascicleLog";
    protected static FASCICLE_DOCUMENT_TYPE_NAME = "FascicleDocument";
    protected static DOMAIN_TYPE_NAME = "DomainUserModel";
    protected static FASCICLE_CATEGORY_FASCICLE = "CategoryFascicle";
    protected static FASCICLEFOLDER_TYPE_NAME= "FascicleFolder";

    private _serviceConfiguration: ServiceConfiguration;
    protected service: FascicleService;

    constructor(serviceConfiguration: ServiceConfiguration) {
        this._serviceConfiguration = serviceConfiguration
    }

    initialize() {
        this.service = new FascicleService(this._serviceConfiguration);
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

    protected showWarningMessage(uscNotificationId: string, customMessage: string) {
        let uscNotification: UscErrorNotification = <UscErrorNotification>$("#".concat(uscNotificationId)).data();
        if (!jQuery.isEmptyObject(uscNotification)) {
            uscNotification.showWarningMessage(customMessage)
        }
    }

    
}

export = FascBase;