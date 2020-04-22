
import CategoryModel = require('App/Models/Commons/CategoryModel');
import CategoryModelMapper = require('App/Mappers/Commons/CategoryModelMapper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UpdateActionType = require("App/Models/UpdateActionType");
import CategorySearchFilterDTO = require('App/DTOs/CategorySearchFilterDTO');
import CategoryTreeViewModel = require('App/ViewModels/Commons/CategoryTreeViewModel');
import CategoryTreeViewModelMapper = require('App/Mappers/Commons/CategoryTreeViewModelMapper');

class CategoryService extends BaseService {
    _configuration: ServiceConfiguration;

    /**
     * Costruttore
     */

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    getByIdMassimarioScarto(idMassimarioScarto: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "$filter=MassimarioScarto/UniqueId eq ".concat(idMassimarioScarto);
        this.getRequest(url, qs, (response: any) => {
            if (callback) callback(response.value);
        }, error);
    }

    getById(categoryId: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "$filter=EntityShortId eq ".concat(categoryId.toString(), "&$expand=Parent");
        this.getRequest(url, qs, (response: any) => {
            if (callback) {
                let instance: CategoryModel = new CategoryModel();
                let categoryMapper: CategoryModelMapper = new CategoryModelMapper();
                instance = categoryMapper.Map(response.value[0]);

                callback(instance);
            }
        }, error);
    }

    /**
     * Modifica un Classificatore esistente
     * @param model
     * @param callback
     * @param error
     */
    updateCategory(model: CategoryModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        url = url.concat("?actionType=", UpdateActionType.UpdateCategory.toString())
        this.putRequest(url, JSON.stringify(model), callback, error);
    }

    findTreeCategory(categoryId: number, fascicleType?: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let fascicleTypeParam: string = null;
        if (fascicleType) {
            fascicleTypeParam = `VecompSoftware.DocSuiteWeb.Entity.Fascicles.FascicleType'${fascicleType}'`;
        }
        url = url.concat(`/CategoryService.FindCategory(idCategory=${categoryId},fascicleType=${fascicleTypeParam})`);
        this.getRequest(url, null, (response: any) => {
            if (callback) {
                let instance: CategoryTreeViewModel;
                if (response && response.value) {
                    let categoryMapper: CategoryTreeViewModelMapper = new CategoryTreeViewModelMapper();
                    instance = categoryMapper.Map(response.value[0]);
                }                

                callback(instance);
            }
        }, error);
    }

    findTreeCategories(finder: CategorySearchFilterDTO, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat(`/CategoryService.FindCategories(finder=@p0)?@p0=${JSON.stringify(finder)}&$orderby=FullCode`);
        this.getRequest(url, undefined, (response: any) => {
            if (callback) {
                let instances: CategoryTreeViewModel[] = [];
                if (response && response.value) {
                    let categoryMapper: CategoryTreeViewModelMapper = new CategoryTreeViewModelMapper();
                    instances = categoryMapper.MapCollection(response.value);
                }
                callback(instances);
            }
        }, error);
    }
}

export = CategoryService;