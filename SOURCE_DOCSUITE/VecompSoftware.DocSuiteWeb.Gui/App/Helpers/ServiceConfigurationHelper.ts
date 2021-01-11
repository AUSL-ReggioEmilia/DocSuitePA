import ServiceConfiguration = require('App/Services/ServiceConfiguration');

class ServiceConfigurationHelper {
    constructor() { }

    static getService(serviceConfigurations: ServiceConfiguration[], name: string): ServiceConfiguration {
        return $.grep(serviceConfigurations, (x) => x.Name == name)[0];
    }
}

export = ServiceConfigurationHelper;