import ContainerPropertyModel = require('App/Models/Commons/ContainerPropertyModel');
import ContainerModelMapper = require('App/Mappers/Commons/ContainerModelMapper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');

class ContainerPropertyService extends BaseService {
    _configuration: ServiceConfiguration;

    /**
     * Costruttore
     */

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }


    getByContainer(idContainer: number, name?: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string ="$filter=Container/EntityShortId eq ".concat(idContainer.toString());
        if (name) {
            data = data.concat(" and Name eq '", name, "'");
        }
        this.getRequest(url, data, (response: any) => {
            if (callback) callback(response.value);
        }, error);
    }

    /**
    * Inserisco una nuova ContainerProperty
    */
    insertContainerProperty(containerProperty: ContainerPropertyModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(containerProperty), callback, error);
    }

    /**
    * Aggiorno una ContainerProperty
    */
    updateContainerProperty(containerProperty: ContainerPropertyModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(containerProperty), callback, error);
    }
}

export = ContainerPropertyService;