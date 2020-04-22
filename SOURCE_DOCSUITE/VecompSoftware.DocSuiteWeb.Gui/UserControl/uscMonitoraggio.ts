import TransparentAdministrationMonitorLogBase = require('Monitors/TransparentAdministrationMonitorLogBase');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');

class uscMonitoraggio extends TransparentAdministrationMonitorLogBase {

    lblArchiveId: string;
    lblCreatedById: string;
    lblMonitoringId: string;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        super(ServiceConfigurationHelper.getService(serviceConfigurations, TransparentAdministrationMonitorLogBase.TransparentAdministrationMonitorLog_TYPE_NAME));
        $(document).ready(() => {
        });
    }

    initialize() {
        super.initialize();
    }

}

export = uscMonitoraggio;