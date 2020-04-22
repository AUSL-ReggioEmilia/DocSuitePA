/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import UDSService = require('App/Services/UDS/UDSService');
import UDSRepositoryService = require("App/Services/UDS/UDSRepositoryService");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import UscErrorNotification = require('UserControl/uscErrorNotification');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
abstract class UDSInvoiceBase {
    protected static UDSRepositoryInvoice_TYPE_NAME = "UDSRepository";
    private _serviceConfigurations: ServiceConfiguration[];
    protected udsRepositoryService: UDSRepositoryService;
    constructor(serviceConfiguration: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfiguration;
    }

    initialize() {
        let UDSRepositoryConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, UDSInvoiceBase.UDSRepositoryInvoice_TYPE_NAME);
        this.udsRepositoryService = new UDSRepositoryService(UDSRepositoryConfiguration);
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
            uscNotification.showNotificationMessage(customMessage);
        }
    }

}

export = UDSInvoiceBase;