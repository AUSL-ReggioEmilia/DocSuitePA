/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceBusTopicService = require('App/Services/ServiceBus/ServiceBusTopicService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import AjaxModel = require('App/Models/AjaxModel');

abstract class EventPECSummaryErrorBase {
    protected static SERVICEBUSTOPIC_TYPE_NAME = "ServiceBusTopicMessage";

    private _serviceConfiguration: ServiceConfiguration;
    protected service: ServiceBusTopicService;

    constructor(serviceConfiguration: ServiceConfiguration) {
        this._serviceConfiguration = serviceConfiguration;
    }

    initialize() {
        this.service = new ServiceBusTopicService(this._serviceConfiguration);
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

export = EventPECSummaryErrorBase;