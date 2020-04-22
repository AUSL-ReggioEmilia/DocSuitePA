define(["require", "exports", "App/Services/Fascicles/FascicleService", "App/Helpers/ServiceConfigurationHelper", "App/Services/Commons/CategoryFascicleService"], function (require, exports, FascicleService, ServiceConfigurationHelper, CategoryFascicleService) {
    var FascicleRule = /** @class */ (function () {
        function FascicleRule(fascicle, serviceConfigurations) {
            var fascicleServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "Fascicle");
            this._fascicleService = new FascicleService(fascicleServiceConfiguration);
            var categoryFascicleServiceConfiguration = ServiceConfigurationHelper.getService(serviceConfigurations, "CategoryFascicle");
            this._categoryFascicleService = new CategoryFascicleService(categoryFascicleServiceConfiguration);
            this._fascicle = fascicle;
        }
        FascicleRule.prototype.hasViewableRight = function () {
            var promise = $.Deferred();
            this._fascicleService.hasViewableRight(this._fascicle.UniqueId, function (data) { return promise.resolve(data); }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        FascicleRule.prototype.hasManageableRight = function () {
            var promise = $.Deferred();
            this._fascicleService.hasManageableRight(this._fascicle.UniqueId, function (data) { return promise.resolve(data); }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        FascicleRule.prototype.isManager = function () {
            var promise = $.Deferred();
            this._fascicleService.isManager(this._fascicle.UniqueId, function (data) { return promise.resolve(data); }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        FascicleRule.prototype.isProcedureSecretary = function () {
            var promise = $.Deferred();
            this._categoryFascicleService.isProcedureSecretary(this._fascicle.Category.EntityShortId, function (data) { return promise.resolve(data); }, function (exception) { return promise.reject(exception); });
            return promise.promise();
        };
        return FascicleRule;
    }());
    return FascicleRule;
});
//# sourceMappingURL=FascicleRights.js.map