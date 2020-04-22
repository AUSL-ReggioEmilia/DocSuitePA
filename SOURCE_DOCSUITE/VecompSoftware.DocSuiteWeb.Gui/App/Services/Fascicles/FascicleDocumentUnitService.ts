
import FascicleDocumentUnitModel = require('App/Models/Fascicles/FascicleDocumentUnitModel');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import FascicolableBaseService = require('App/Services/Fascicles/FascicolableBaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import FascicleDocumentUnitModelMapper = require('App/Mappers/Fascicles/FascicleDocumentUnitModelMapper');

class FascicleDocumentUnitService extends FascicolableBaseService<FascicleDocumentUnitModel> {
    _configuration: ServiceConfiguration;

    /**
     * Costruttore
     */
    constructor(configuration: ServiceConfiguration) {
        super(configuration);
        this._configuration = configuration;
    }

    getByDocumentUnitAndFascicle(idDocumentUnit: string, idFascicle: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$filter=Fascicle/UniqueId eq ${idFascicle} and DocumentUnit/UniqueId eq ${idDocumentUnit}&$expand=DocumentUnit,Fascicle`;
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    let mapper = new FascicleDocumentUnitModelMapper();
                    callback(mapper.Map(response.value[0]));
                }
            }, error);
    }

    getFascicleListById(IdUDS: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "$expand=Fascicle&$filter=DocumentUnit/UniqueId eq ".concat(IdUDS);
        this.getRequest(url, data,
            (response: any) => {
                if (callback) {
                    callback(response.value);
                }
            }, error);
    }

    countFascicleById(IdUDS: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = "/$count?$filter=DocumentUnit/UniqueId eq ".concat(IdUDS);
        url = url.concat(data);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }
}

export = FascicleDocumentUnitService;