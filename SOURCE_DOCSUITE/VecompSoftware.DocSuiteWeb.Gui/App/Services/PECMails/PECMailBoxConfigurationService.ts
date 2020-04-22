import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import PECMailBoxConfigurationViewModel = require('App/ViewModels/PECMails/PECMailBoxConfigurationViewModel');
import PECMailBoxConfigurationViewModelMapper = require('App/Mappers/PECMails/PECMailBoxConfigurationViewModelMapper');

class PECMailBoxConfigurationService extends BaseService {
  _configuration: ServiceConfiguration;

  constructor(configuration: ServiceConfiguration) {
    super();
    this._configuration = configuration;
  }

  getPECMailBoxConfigurations(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
    let url: string = this._configuration.ODATAUrl;
    this.getRequest(url, null, (response: any) => {
      if (callback && response) {
        let viewModelMapper = new PECMailBoxConfigurationViewModelMapper();
        let pecMailBoxConfigurations: PECMailBoxConfigurationViewModel[] = [];
        $.each(response.value, function (i, value) {
          pecMailBoxConfigurations.push(viewModelMapper.Map(value));
        });
        callback(pecMailBoxConfigurations);
      };
    }, error);
  }

}

export = PECMailBoxConfigurationService;