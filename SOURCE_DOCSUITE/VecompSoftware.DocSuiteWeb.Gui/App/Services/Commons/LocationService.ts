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
        let data: string = `$orderby=Name`;
        this.getRequest(url, data, (response: any) => {
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

    getLocationDetailsByUniqueId(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$filter=UniqueId eq ${uniqueId.toString()}`;
        this.getRequest(url, data, (response: any) => {
            if (callback) callback(response.value[0]);
        }, error);
    }

    filterLocationByNameAndArchive(name: string, archive: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$orderby=Name&$filter=`;
        
        if (name != "" && archive != "") {
            data = `${data} contains(Name,'${name}') or contains(ProtocolArchive, '${archive}') or contains(DossierArchive, '${archive}') or contains(ResolutionArchive, '${archive}')`;
        } else if (name != "") {
            data = `${data} contains(Name,'${name}')`;
        } else if (archive != "") {
            data = `${data} contains(ProtocolArchive, '${archive}') or contains(DossierArchive, '${archive}') or contains(ResolutionArchive, '${archive}')`;
        } else {
            data = `${data} contains(Name,'') or contains(ProtocolArchive, '') or contains(DossierArchive, '') or contains(ResolutionArchive, '')`;
        }

        this.getRequest(url, data, (response: any) => {
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

    insert(location: LocationViewModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(location), callback, error);
    }

    update(location: LocationViewModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any) {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(location), callback, error);
    }
}

export = LocationService;