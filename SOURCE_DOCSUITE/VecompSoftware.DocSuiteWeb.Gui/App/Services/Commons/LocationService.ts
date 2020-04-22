import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import LocationViewModel = require('App/ViewModels/Commons/LocationViewModel');
import LocationViewModelMapper = require('App/Mappers/Commons/LocationViewModelMapper');

class LocationService extends BaseService {
  _configuration: ServiceConfiguration;

  constructor(configuration: ServiceConfiguration) {
    super();
    this._configuration = configuration;
  }

  getLocations(callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
    let url: string = this._configuration.ODATAUrl;
    this.getRequest(url, null, (response: any) => {
      if (callback && response) {
        let viewModelMapper = new LocationViewModelMapper();
        let locations: LocationViewModel[] = [];
        $.each(response.value, function (i, value) {
          locations.push(viewModelMapper.Map(value));
        });
        callback(locations);
      };
    }, error);
  }

}

export = LocationService;