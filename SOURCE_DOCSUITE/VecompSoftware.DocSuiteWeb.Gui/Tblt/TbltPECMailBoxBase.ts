/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import PECMailBoxService = require('App/Services/PECMails/PECMailBoxService');
import PECMailBoxConfigurationService = require('App/Services/PECMails/PECMailBoxConfigurationService');
import LocationService = require('App/Services/Commons/LocationService');
import JeepServiceHostService = require('App/Services/JeepServiceHosts/JeepServiceHostService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import PECMailBoxModel = require('App/Models/PECMails/PECMailBoxModel');
import ChainType = require('App/Models/DocumentUnits/ChainType');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UscErrorNotification = require('UserControl/uscErrorNotification');
import AjaxModel = require('App/Models/AjaxModel');

abstract class TbltPECMailBoxBase {
  protected static PECMailBox_TYPE_NAME = "PECMailBox";
  protected static PECMailBoxConfiguraton_TYPE_NAME = "PECMailBoxConfiguration";
  protected static Location_TYPE_NAME = "Location";
  protected static JeepServiceHost_TYPE_NAME = "JeepServiceHost";

  private _serviceConfiguration: ServiceConfiguration;
  protected pecMailBoxService: PECMailBoxService;
  protected pecMailBoxConfigurationService: PECMailBoxConfigurationService;
  protected locationService: LocationService;
  protected jeepServiceHostService: JeepServiceHostService;

  constructor(serviceConfiguration: ServiceConfiguration) {
    this._serviceConfiguration = serviceConfiguration;
  }
  
  initialize() {
    this.pecMailBoxService = new PECMailBoxService(this._serviceConfiguration);
  }

  initializeServices(serviceConfigurations: ServiceConfiguration[]) {
    this.pecMailBoxConfigurationService =
      new PECMailBoxConfigurationService(ServiceConfigurationHelper.getService(serviceConfigurations, TbltPECMailBoxBase.PECMailBoxConfiguraton_TYPE_NAME));
    this.locationService =
      new LocationService(ServiceConfigurationHelper.getService(serviceConfigurations, TbltPECMailBoxBase.Location_TYPE_NAME));
    this.jeepServiceHostService =
      new JeepServiceHostService(ServiceConfigurationHelper.getService(serviceConfigurations, TbltPECMailBoxBase.JeepServiceHost_TYPE_NAME));
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

export = TbltPECMailBoxBase;