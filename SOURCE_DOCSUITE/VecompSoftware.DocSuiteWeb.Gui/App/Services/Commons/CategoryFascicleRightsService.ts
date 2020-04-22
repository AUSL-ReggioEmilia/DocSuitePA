import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import CategoryFascicleRightModel = require('App/Models/Commons/CategoryFascicleRightModel');

class CategoryFascicleRightsService extends BaseService {
    _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    insertCategoryFascicleRight(categoryFascicleRight: CategoryFascicleRightModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(categoryFascicleRight), callback, error)
    }

    getProcedureFascicleRightsByCategory(idCategory: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = `$filter=CategoryFascicle/Category/EntityShortId eq ${idCategory} and CategoryFascicle/FascicleType eq 'Procedure'&$expand=Role`;
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    callback(response.value);
                }
            }, error);
    }
 
    GetCategoryFascicleRight(idCategory: string, idContainer: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "$filter=CategoryFascicle/Category/EntityShortId eq ".concat(idCategory, " and Container/EntityShortId eq ", idContainer, "");
        this.getRequest(url, qs,
            (response: any) => {
                if (response) {
                    callback(response.value);
                }
            }, error);
    }

    deleteCategoryFascicleRight(model: CategoryFascicleRightModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.deleteRequest(url, JSON.stringify(model), callback, error);
    }
}
export = CategoryFascicleRightsService