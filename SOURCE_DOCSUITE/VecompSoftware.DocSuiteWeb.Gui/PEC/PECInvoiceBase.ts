/// <reference path="../scripts/typings/telerik/telerik.web.ui.d.ts" />
/// <reference path="../scripts/typings/telerik/microsoft.ajax.d.ts" />

import PECMailService = require('App/Services/PECMails/PECMailService');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import TenantService = require("App/Services/Tenants/TenantService");
import TenantWorkflowRepositoryService = require("App/Services/Tenants/TenantWorkflowRepositoryService");
import PECMailBoxService = require('App/Services/PECMails/PECMailBoxService');

abstract class PECInvoiceBase {
    protected static PECMailInvoice_TYPE_NAME = "PECMail";
    protected static PECMailBoxInvoice_TYPE_NAME = "PECMailBox";
    protected static Tenant_TYPE_NAME = "Tenant";
    protected static TenantWorkflowRepository_TYPE_NAME = "TenantWorkflowRepository";

    protected _serviceConfigurations: ServiceConfiguration[];
    protected _pecMailService: PECMailService;
    protected _pecMailBoxService: PECMailBoxService;
    protected _tenantService: TenantService;
    protected _tenantWorkflowRepositoryService: TenantWorkflowRepositoryService;

    constructor(serviceConfiguration: ServiceConfiguration[]) {
        this._serviceConfigurations = serviceConfiguration;
    }

    initialize() {

        let pecMailConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, PECInvoiceBase.PECMailInvoice_TYPE_NAME);
        let pecMailBoxConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, PECInvoiceBase.PECMailBoxInvoice_TYPE_NAME);
        let tenantConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, PECInvoiceBase.Tenant_TYPE_NAME);
        let tenantWorkflowRepositoryConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(this._serviceConfigurations, PECInvoiceBase.TenantWorkflowRepository_TYPE_NAME);


        this._pecMailService = new PECMailService(pecMailConfiguration);
        this._pecMailBoxService = new PECMailBoxService(pecMailBoxConfiguration);
        this._tenantService = new TenantService(tenantConfiguration);
        this._tenantWorkflowRepositoryService = new TenantWorkflowRepositoryService(tenantWorkflowRepositoryConfiguration);

    }
}

export = PECInvoiceBase;