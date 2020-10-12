import BaseService = require("../BaseService");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UDSDocumentUnitModelMapper = require("App/Mappers/UDS/UDSDocumentUnitModelMapper");
import UDSDocumentUnitModel = require("App/Models/UDS/UDSDocumentUnitModel");

class UDSDocumentUnitService extends BaseService {
    _configuration: ServiceConfiguration;
    _mapper: UDSDocumentUnitModelMapper;

    /**
     * Costruttore
     */
    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
        this._mapper = new UDSDocumentUnitModelMapper();
    }

    getUDSByProtocolId(Id: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$expand=Relation($expand=UDSRepository),SourceUDS($expand=UDSRepository)&$filter=Relation/Uniqueid eq ${Id} and Relation/Environment eq 1`;
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    callback(response.value);
                }
            }, error);
    }

    getProtocolListById(IdUDS: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$expand=Relation&$filter=Relation/Environment eq 1 and IdUDS eq ".concat(IdUDS);
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    callback(response.value);
                }
            }, error);
    }

    getUDSListById(IdUDS: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$expand=Relation($expand=UDSRepository($select=UniqueId))&$filter=Relation/Environment ge 100 and IdUDS eq ".concat(IdUDS);
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    callback(response.value);
                }
            }, error);
    }
    getUDSById(IdUDS: string, relationId:string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$expand=Relation($expand=UDSRepository($select=UniqueId))&$filter=Relation/Environment ge 100 and IdUDS eq ${IdUDS} and Relation/UniqueId eq ${relationId} `;
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    callback(response.value);
                }
            }, error);
    }

    deleteUDSByid(model: UDSDocumentUnitModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.deleteRequest(url, JSON.stringify(model), callback, error);
    }

    countProtocolsById(IdUDS: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "/$count?$filter=Relation/Environment eq 1 and IdUDS eq ".concat(IdUDS);
        url = url.concat(data);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }

    countUDSByRelationId(documentUnitId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "/$count?$filter=Relation/Environment eq 1 and Relation/UniqueId eq ".concat(documentUnitId);
        url = url.concat(data);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }

    countUDSById(IdUDS: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "/$count?$filter=Relation/Environment ge 100 and IdUDS eq ".concat(IdUDS);
        url = url.concat(data);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }

    countUDSId(IdUDS: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "/$count?$filter=IdUDS eq ".concat(IdUDS);
        url = url.concat(data);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }
}

export = UDSDocumentUnitService;