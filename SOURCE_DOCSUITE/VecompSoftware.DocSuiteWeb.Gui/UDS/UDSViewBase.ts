import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import UDSConnectionService = require('App/Services/UDS/UDSConnectionService');
import UDSRepositoryService = require('App/Services/UDS/UDSRepositoryService');
import ServiceConfigurationHelper = require('App/Helpers/ServiceConfigurationHelper');

class UDSViewBase {
    protected static UDS_DOCUMENT_UNIT_NAME: string = "UDSDocumentUnit";
    protected static UDS_REPOSITORY_NAME: string = "UDSRepository";

    protected _serviceConfigurations: ServiceConfiguration[];
    protected udsConnectionService: UDSConnectionService;
    protected udsRepositoryService: UDSRepositoryService;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    initialize() {
        let udsConnectionConfiguration: ServiceConfiguration
            = ServiceConfigurationHelper.getService(this._serviceConfigurations, UDSViewBase.UDS_DOCUMENT_UNIT_NAME);
        let udsRepositoryConfiguration: ServiceConfiguration
            = ServiceConfigurationHelper.getService(this._serviceConfigurations, UDSViewBase.UDS_REPOSITORY_NAME);

        this.udsConnectionService = new UDSConnectionService(udsConnectionConfiguration);
        this.udsRepositoryService = new UDSRepositoryService(udsRepositoryConfiguration);
    }
}

export = UDSViewBase;