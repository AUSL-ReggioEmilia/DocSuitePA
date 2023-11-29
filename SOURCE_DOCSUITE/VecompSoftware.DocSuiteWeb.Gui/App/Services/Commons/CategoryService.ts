
import CategoryModel = require('App/Models/Commons/CategoryModel');
import CategoryModelMapper = require('App/Mappers/Commons/CategoryModelMapper');
import ServiceConfiguration = require('App/Services/ServiceConfiguration');
import BaseService = require('App/Services/BaseService');
import ExceptionDTO = require('App/DTOs/ExceptionDTO');
import UpdateActionType = require("App/Models/UpdateActionType");
import CategorySearchFilterDTO = require('App/DTOs/CategorySearchFilterDTO');
import CategoryTreeViewModel = require('App/ViewModels/Commons/CategoryTreeViewModel');
import CategoryTreeViewModelMapper = require('App/Mappers/Commons/CategoryTreeViewModelMapper');
import PaginationModel = require('App/Models/Commons/PaginationModel');
import ODATAResponseModel = require('App/Models/ODATAResponseModel');

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

    getById(categoryId: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any, tenantAOOId?: string, includeProperties: string[] = []): void {
        let url: string = this._configuration.ODATAUrl;
        let filterQueries: string[] = [`EntityShortId eq ${categoryId}`];

        if (tenantAOOId) {
            filterQueries.push(`TenantAOO/UniqueId eq ${tenantAOOId}`);
        }

        let odataFilter: string = `$filter=${filterQueries.join(" and ")}`;

        if (includeProperties && includeProperties.length) {
            odataFilter = `${odataFilter}&$expand=${includeProperties.join(",")}`;
        }

        this.getRequest(url, odataFilter, (response: any) => {
            if (callback) {
                let instance: CategoryModel = new CategoryModel();
                let categoryMapper: CategoryModelMapper = new CategoryModelMapper();
                instance = categoryMapper.Map(response.value[0]);

                callback(instance);
            }
        }, error);
    }

    getRolesByCategoryId(categoryId: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        let qs: string = `$filter=EntityShortId eq ${categoryId}&$expand=CategoryFascicles($expand=CategoryFascicleRights($expand=Role($expand=Father))),MetadataRepository,MassimarioScarto`;
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

    findTreeCategories(finder: CategorySearchFilterDTO, callback?: (data: CategoryTreeViewModel[]) => any, error?: (exception: ExceptionDTO) => any): void {
        let odataUrl: string = this._configuration.ODATAUrl;
        let odataQuery = `${odataUrl}/CategoryService.FindCategories(finder=@p0)?@p0=${JSON.stringify(finder)}&$orderby=FullCode,Name`;

        this.getRequest(odataQuery, undefined, (response: any) => {

            if (!callback) {
                return;
            }

            let instances: CategoryTreeViewModel[] = [];
            if (response && response.value) {
                let categoryMapper: CategoryTreeViewModelMapper = new CategoryTreeViewModelMapper();
                instances = categoryMapper.MapCollection(response.value);
            }
            callback(instances);
        }, error);
    }

    findFascicolableCategory(idCategory: number, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any): void {
        let url: string = this._configuration.ODATAUrl;
        url = url.concat(`/CategoryService.FindFascicolableCategory(idCategory=${idCategory})`);
        this.getRequest(url, undefined, (response: any) => {
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

    getOnlyFascicolableCategories(tenantAOOId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any, paginationModel?: PaginationModel): void {
        let url: string = `${this._configuration.ODATAUrl}?$expand=CategoryFascicles,Parent&$filter=TenantAOO/UniqueId eq ${tenantAOOId} and(CategoryFascicles/any(cf:cf/FascicleType ne 'SubFascicle'))&$orderby=FullCode,Name`;

        if (paginationModel) {
            url = `${url}&$skip=${paginationModel.Skip}&$top=${paginationModel.Take}&$count=true`;
        }

        this.getRequest(url, null, (response: any) => {
            if (!callback) {
                return;
            }

            let categories: CategoryModel[] = [];

            if (response && response.value) {
                let categoryMaper: CategoryModelMapper = new CategoryModelMapper();
                categories = categoryMaper.MapCollection(response.value);
            }

            if (!paginationModel) {
                callback(categories);
                return;
            }

            const odataResult: ODATAResponseModel<CategoryModel> = new ODATAResponseModel<CategoryModel>(response);
            odataResult.value = categories;
            callback(odataResult);
        }, error);
    }

    getCategoriesByIds(categoryIds: number[], tenantAOOId: string, callback?: (data: any) => any, error?: (exception: ExceptionDTO) => any, paginationModel?: PaginationModel): void {
        let url: string = `${this._configuration.ODATAUrl}?$filter=TenantAOO/UniqueId eq ${tenantAOOId} and EntityShortId in [${categoryIds.join(",")}]&$expand=CategoryFascicles,Parent&$orderby=FullCode,Name`;

        if (paginationModel) {
            url = `${url}&$skip=${paginationModel.Skip}&$top=${paginationModel.Take}&$count=true`;
        }

        this.getRequest(url, null, (response: any) => {

            if (!callback) {
                return;
            }

            let categories: CategoryModel[] = [];

            if (response && response.value) {
                let categoryMaper: CategoryModelMapper = new CategoryModelMapper();
                categories = categoryMaper.MapCollection(response.value);
            }

            if (!paginationModel) {
                callback(categories);
                return;
            }

            const odataResult: ODATAResponseModel<CategoryModel> = new ODATAResponseModel<CategoryModel>(response);
            odataResult.value = categories;
            callback(odataResult);
        }, error);
    }

    getTenantAOORootCategory(tenantAOOId: string, callback?: (rootCategory: CategoryModel) => any, error?: (exception: ExceptionDTO) => any){
        const odataQuery: string = `${this._configuration.ODATAUrl}?$filter=TenantAOO/UniqueId eq ${tenantAOOId} and Code eq 0`;

        this.getRequest(odataQuery, null, (response: any) => {
            if (!callback) {
                return;
            }

            let tenantAOOCategory: CategoryModel = null;
            if (response && response.value) {
                let categoryMaper: CategoryModelMapper = new CategoryModelMapper();
                tenantAOOCategory = categoryMaper.Map(response.value[0]);
            }

            callback(tenantAOOCategory);
        }, error);
    }

    countSubCategories(parentCategoryId: number, callback?: (childrenCount: number) => any, error?: (exception: ExceptionDTO) => any)
    countSubCategories(parentCategoryId: number, callback?: (childrenCount: number) => any, error?: (exception: ExceptionDTO) => any, tenantAOOId?: string)
    countSubCategories(parentCategoryId: number, callback?: (childrenCount: number) => any, error?: (exception: ExceptionDTO) => any, tenantAOOId?: string): void {
        const baseOdataURL: string = `${this._configuration.ODATAUrl}/$count`;
        let filterQueries: string[] = [`Parent/EntityShortId eq ${parentCategoryId}`];

        if (tenantAOOId) {
            filterQueries.push(`TenantAOO/UniqueId eq ${tenantAOOId}`);
        }

        let odataFilterQuery: string = `$filter=${filterQueries.join(" and ")}`;
        this.getRequest(baseOdataURL, odataFilterQuery,
            (response: any) => {
                if (callback) {
                    callback(response);
                }
            }, error);
    }

    getSubCategories(parentCategoryId: number, callback?: (data: CategoryModel[] | ODATAResponseModel<CategoryModel>) => any, error?: (exception: ExceptionDTO) => any, paginationModel?: PaginationModel)
    getSubCategories(parentCategoryId: number, callback?: (data: CategoryModel[] | ODATAResponseModel<CategoryModel>) => any, error?: (exception: ExceptionDTO) => any, paginationModel?: PaginationModel, tenantAOOId?: string, includeProperties?: string[])
    getSubCategories(parentCategoryId: number, callback?: (data: CategoryModel[] | ODATAResponseModel<CategoryModel>) => any, error?: (exception: ExceptionDTO) => any, paginationModel?: PaginationModel, tenantAOOId?: string, includeProperties?: string[]) {
        let baseOdataURL: string = this._configuration.ODATAUrl;
        let filterQueries: string[] = [`Parent/EntityShortId eq ${parentCategoryId}`];

        if (tenantAOOId) {
            filterQueries.push(`TenantAOO/UniqueId eq ${tenantAOOId}`);
        }

        let odataFilterQuery: string = `$filter=${filterQueries.join(" and ")}`;
        let odataQuery: string = paginationModel
            ? `${baseOdataURL}?${odataFilterQuery}&$skip=${paginationModel.Skip}&$top=${paginationModel.Take}&$count=true&$orderby=Code asc`
            : `${baseOdataURL}?${odataFilterQuery}&$orderby=Code asc`;

        if (includeProperties && includeProperties.length) {
            odataQuery = `${odataQuery}&$expand=${includeProperties.join(",")}`;
        }

        this.getRequest(odataQuery, null, (response: any) => {
            if (!callback && !response) {
                return;
            }

            let subCategories: CategoryModel[] = [];
            if (response.value) {
                let categoryMaper: CategoryModelMapper = new CategoryModelMapper();
                subCategories = categoryMaper.MapCollection(response.value);
            }

            if (!paginationModel) {
                callback(subCategories);
                return;
            }

            const odataResult: ODATAResponseModel<CategoryModel> = new ODATAResponseModel<CategoryModel>(response);
            odataResult.value = subCategories;
            callback(odataResult);
        }, error);
    }
}

export = CategoryService;