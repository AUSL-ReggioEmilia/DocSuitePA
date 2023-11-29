import BaseService = require("App/Services/BaseService");
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UDSFieldListModelMapper = require("App/Mappers/UDS/UDSFieldListModelMapper");

class UDSFieldListService extends BaseService {
    private _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getUDSFieldListRoot(idUDSRepository: string, fieldName: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let data: string = `$filter=UDSFieldListLevel eq 3 and FieldName eq '${fieldName}' and Repository/UniqueId eq ${idUDSRepository}&$expand=Repository&$orderby=Name`;
        this.getRequest(url, data, (response: any) => {
            if (callback) {
                callback(new UDSFieldListModelMapper().MapCollection(response.value));
            }
        }, error);
    }

    getChildrenByParent(parentId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = `${this._configuration.ODATAUrl}/UDSFieldListService.GetChildrenByParent(parentId=${parentId})`;
        this.getRequest(url, null, (response: any) => {
            if (callback) {
                callback(new UDSFieldListModelMapper().MapCollection(response.value));
            }
        }, error);
    }

    getAllParents(childId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = `${this._configuration.ODATAUrl}/UDSFieldListService.GetAllParents(childId=${childId})`;
        this.getRequest(url, null, (response: any) => {
            if (callback) {
                callback(new UDSFieldListModelMapper().MapCollection(response.value));
            }
        }, error);
    }
}

export = UDSFieldListService;