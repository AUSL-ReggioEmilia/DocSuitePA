import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import TenantConfigurationModel = require('App/Models/Tenants/TenantConfigurationModel');
import TenantConfigurationModelMapper = require('App/Mappers/Tenants/TenantConfigurationModelMapper');

class TenantConfigurationService extends BaseService {
  _configuration: ServiceConfiguration;

  constructor(configuration: ServiceConfiguration) {
    super();
    this._configuration = configuration;
  }

  getTenantConfigurations(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
    let url: string = this._configuration.ODATAUrl;
    this.getRequest(url, null, (response: any) => {
      if (callback && response) {
        let modelMapper = new TenantConfigurationModelMapper();
        let tenantConfigurations: TenantConfigurationModel[] = [];
        $.each(response.value, function (i, value) {
          tenantConfigurations.push(modelMapper.Map(value));
        });
        callback(tenantConfigurations);
      };
    }, error);
  }

  updateTenantConfiguration(model: TenantConfigurationModel,
    callback?: (data: any) => any,
    error?: (exception: ExceptionDTO) => any): void {
    let url: string = this._configuration.WebAPIUrl;
    this.putRequest(url, JSON.stringify(model), callback, error);
  }
}

export = TenantConfigurationService;