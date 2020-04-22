import ServiceConfiguration = require('App/Services/ServiceConfiguration');

class TenantModelconfiguration{
    serviceConfiguration: ServiceConfiguration[];

    constructor(jsonModel: string) {
        let reportBuilderModel: ServiceConfiguration[] = JSON.parse(jsonModel);
        this.serviceConfiguration = reportBuilderModel;
    }
}

export = TenantModelconfiguration;