import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import JeepServiceHostViewModel = require('App/ViewModels/JeepServiceHosts/JeepServiceHostViewModel');
import JeepServiceHostViewModelMapper = require('App/Mappers/JeepServiceHosts/JeepServiceHostViewModelMapper');

class JeepServiceHostService extends BaseService {
  _configuration: ServiceConfiguration;

  constructor(configuration: ServiceConfiguration) {
    super();
    this._configuration = configuration;
  }

  getJeepServiceHosts(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
    let url: string = this._configuration.ODATAUrl;
    this.getRequest(url, null, (response: any) => {
      if (callback && response) {
        let viewModelMapper = new JeepServiceHostViewModelMapper();
        let jeepServiceHosts: JeepServiceHostViewModel[] = [];
        $.each(response.value, function (i, value) {
          jeepServiceHosts.push(viewModelMapper.Map(value));
        });
        callback(jeepServiceHosts);
      };
    }, error);
  }

}

export = JeepServiceHostService;