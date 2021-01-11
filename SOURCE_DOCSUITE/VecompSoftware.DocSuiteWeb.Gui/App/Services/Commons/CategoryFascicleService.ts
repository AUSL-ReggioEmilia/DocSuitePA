import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import CategoryFascicleViewModelMapper = require('App/Mappers/Commons/CategoryFascicleViewModelMapper');
import CategoryFascicleViewModel = require('App/ViewModels/Commons/CategoryFascicleViewModel');
import CategoryFascicleModel = require('App/Models/Commons/CategoryFascicleModel');
import FascicleType = require('App/Models/Fascicles/FascicleType');
class CategoryFascicleService extends BaseService {
    _configuration: ServiceConfiguration;

    constructor(configuration: ServiceConfiguration) {
        super();
        this._configuration = configuration;
    }

    /**
     * Recupero i CategoryFascicle sul classificatore selezionato
     * @param idCategory
     * @param callback
     * @param error
     */
    getByIdCategory(idCategory: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "$filter=Category/EntityShortId eq ".concat(idCategory, ' &$expand=Category,FasciclePeriod,Manager');
        this.getRequest(url, qs,
            (response: any) => {
                if (callback && response) {
                        callback(response.value);
                }
            }, error);
    }

    getAvailableProcedureCategoryFascicleByCategory(idCategory: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = `$filter=Category/EntityShortId eq ${idCategory} and FascicleType eq 'Procedure' and DSWEnvironment eq 0&$expand=Category,FasciclePeriod,Manager`;
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    if (!response) {
                        callback(null);
                    }    
                    callback(response.value);
                }
            }, error);
    }

    /**
     * recupero il category fascicle per UniqueId
     * @param uniqueId
     * @param callback
     * @param error
     */
    getById(uniqueId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = "$filter=UniqueId eq ".concat(uniqueId, ' &$expand=Category,FasciclePeriod,Manager');
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    let mapper = new CategoryFascicleViewModelMapper();
                    if (response) {
                        callback(mapper.Map(response.value[0]));
                    }
                }
            }, error);
    }

    /**
     * recupero i category fascicle periodici per i quali non è ancora attivo un fascicolo periodico
     * @param idCategory
     * @param callback
     * @param error
     */
    geAvailablePeriodicCategoryFascicles(idCategory: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat("/CategoryFascicleService.GeAvailablePeriodicCategoryFascicles(idCategory=", idCategory, ")");
        let qs: string = "";
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    let mapper = new CategoryFascicleViewModelMapper();
                    let categoryFascicles: CategoryFascicleViewModel[] = [];
                    if (response) {
                        categoryFascicles = mapper.MapCollection(response.value);

                        callback(categoryFascicles);
                    }
                }
            }, error);
    }

    /**
     * metodo per l'inserimento di un nuovo categoryFascicle
     * @param categoryFascicle
     * @param callback
     * @param error
     */
    insertCategoryFascicle(categoryFascicle: CategoryFascicleModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.postRequest(url, JSON.stringify(categoryFascicle), callback, error)
    }

    /**
     * metodo per l'aggiornamento
     * @param categoryFascicle
     * @param callback
     * @param error
     */
    updateCategoryFascicle(categoryFascicle: CategoryFascicleModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.WebAPIUrl;
        this.putRequest(url, JSON.stringify(categoryFascicle), callback, error);
    }

    /**
   * Cancellazione di un CategoryFascicle
   * @param model
   * @param callback
   * @param error
   */
    deleteCategoryFascicle(model: CategoryFascicleModel, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = `${this._configuration.WebAPIUrl}?actionType=DeleteCategoryFascicle`;
        this.deleteRequest(url, JSON.stringify(model), callback, error);
    }

    isProcedureSecretary(idCategory: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl.concat(`/CategoryFascicleService.IsProcedureSecretary(idCategory=${idCategory})`);
        this.getRequest(url, null,
            (response: any) => {
                if (callback) {
                    if (!response) {
                        callback(undefined);
                        return;
                    }

                    callback(response.value);
                }
            }, error);
    }

    getPeriodicCategoryFascicleByCategory(idCategory: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = `$filter=Category/EntityShortId eq ${idCategory} and FascicleType eq 'Period'&$expand=Category,FasciclePeriod,Manager`;
        this.getRequest(url, qs,
            (response: any) => {
                if (callback) {
                    if (!response) {
                        callback(null);
                    }
                    callback(response.value);
                }
            }, error);
    }
}
export = CategoryFascicleService