import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ConservationModel = require('App/Models/Commons/ConservationModel');
import ConservationModelMapper = require('App/Mappers/Commons/ConservationModelMapper');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');


class ConservationService extends BaseService {
    private _configuration: ServiceConfiguration;
    /**
  * Costruttore 
  */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    /**
      * Recupero tutti gli idConservation
      * @param callback
      * @param error
      */

    getById(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("?$filter=UniqueId eq ", uniqueId);
        this.getRequest(url,undefined,
            (response: any) => {
                if (callback) {
                    let mapper = new ConservationModelMapper();
                    if (response) {
                        callback(mapper.Map(response.value[0]));
                    }
                }
            }, error);
    }




} export = ConservationService;