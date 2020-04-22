import FascicleModel = require("App/Models/Fascicles/FascicleModel");
import FascicleService = require("App/Services/Fascicles/FascicleService");
import ServiceConfiguration = require("App/Services/ServiceConfiguration");
import ServiceConfigurationHelper = require("App/Helpers/ServiceConfigurationHelper");
import CategoryFascicleService = require("App/Services/Commons/CategoryFascicleService");
import ExceptionDTO = require("App/DTOs/ExceptionDTO");

class FascicleRule {
    private readonly _fascicle: FascicleModel;
    private readonly _fascicleService: FascicleService;
    private readonly _categoryFascicleService: CategoryFascicleService;

    constructor(fascicle: FascicleModel, serviceConfigurations: ServiceConfiguration[]) {
        let fascicleServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Fascicle");
        this._fascicleService = new FascicleService(fascicleServiceConfiguration);

        let categoryFascicleServiceConfiguration: ServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "CategoryFascicle");
        this._categoryFascicleService = new CategoryFascicleService(categoryFascicleServiceConfiguration);
        this._fascicle = fascicle;
    }

    hasViewableRight(): JQueryPromise<boolean> {
        let promise: JQueryDeferred<boolean> = $.Deferred<boolean>();
        this._fascicleService.hasViewableRight(this._fascicle.UniqueId,
            (data: any) => promise.resolve(data),
            (exception: ExceptionDTO) => promise.reject(exception)
        );
        return promise.promise();
    }

    hasManageableRight(): JQueryPromise<boolean> {
        let promise: JQueryDeferred<boolean> = $.Deferred<boolean>();
        this._fascicleService.hasManageableRight(this._fascicle.UniqueId,
            (data: any) => promise.resolve(data),
            (exception: ExceptionDTO) => promise.reject(exception)
        );
        return promise.promise();
    }

    isManager(): JQueryPromise<boolean> {
        let promise: JQueryDeferred<boolean> = $.Deferred<boolean>();
        this._fascicleService.isManager(this._fascicle.UniqueId,
            (data: any) => promise.resolve(data),
            (exception: ExceptionDTO) => promise.reject(exception)
        );
        return promise.promise();
    }

    isProcedureSecretary(): JQueryPromise<boolean> {
        let promise: JQueryDeferred<boolean> = $.Deferred<boolean>();
        this._categoryFascicleService.isProcedureSecretary(this._fascicle.Category.EntityShortId,
            (data: any) => promise.resolve(data),
            (exception: ExceptionDTO) => promise.reject(exception)
        );
        return promise.promise();
    }
}

export = FascicleRule;