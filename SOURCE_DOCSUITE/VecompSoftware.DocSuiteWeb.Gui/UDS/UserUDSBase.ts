import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import UDSRepositoryService = require('../App/Services/UDS/UDSRepositoryService');
import UDSLogService = require('../App/Services/UDS/UDSLogService');
import ServiceConfigurationHelper = require('../App/Helpers/ServiceConfigurationHelper');

class UserUDSBase {
    protected static UDS_REPOSITORY_NAME: string = "UDSRepository";
    protected static UDS_LOG_NAME: string = "UDSLog";

    protected _serviceConfigurations: ServiceConfiguration[];
    protected udsRepositoryService: UDSRepositoryService;
    protected udsLogService: UDSLogService;

    constructor(serviceConfigurations: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfigurations;
    }

    initialize() {
        let udsRepositoryConfiguration: ServiceConfiguration
            = ServiceConfigurationHelper.getService(this._serviceConfigurations, UserUDSBase.UDS_REPOSITORY_NAME);
        this.udsRepositoryService = new UDSRepositoryService(udsRepositoryConfiguration);
        let udsLogConfiguration: ServiceConfiguration
            = ServiceConfigurationHelper.getService(this._serviceConfigurations, UserUDSBase.UDS_LOG_NAME);
        this.udsLogService = new UDSLogService(udsLogConfiguration);
    }
}

export = UserUDSBase;