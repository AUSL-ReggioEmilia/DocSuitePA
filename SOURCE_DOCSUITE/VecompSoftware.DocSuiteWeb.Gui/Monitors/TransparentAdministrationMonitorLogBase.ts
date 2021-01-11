/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import TransparentAdministrationMonitorLogService = require('App/Services/Monitors/TransparentAdministrationMonitorLogService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import TransparentAdministrationMonitorLogModel = require('App/Models/Monitors/TransparentAdministrationMonitorLogModel');
import ChainType = require('App/Models/DocumentUnits/ChainType');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import AjaxModel = require('App/Models/AjaxModel');

abstract class TransparentAdministrationMonitorLogBase {
    protected static TransparentAdministrationMonitorLog_TYPE_NAME = "TransparentAdministrationMonitorLog";

    private _serviceConfiguration: ServiceConfiguration;
    protected service: TransparentAdministrationMonitorLogService;

    constructor(serviceConfiguration: ServiceConfiguration) {
        this._serviceConfiguration = serviceConfiguration;
    }

    initialize() {
        this.service = new TransparentAdministrationMonitorLogService(this._serviceConfiguration);
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

export = TransparentAdministrationMonitorLogBase;