var __extends = (this && this.__extends) || (function () {
    var extendStatics = function (d, b) {
        extendStatics = Object.setPrototypeOf ||
            ({ __proto__: [] } instanceof Array && function (d, b) { d.__proto__ = b; }) ||
            function (d, b) { for (var p in b) if (b.hasOwnProperty(p)) d[p] = b[p]; };
        return extendStatics(d, b);
    };
    return function (d, b) {
        extendStatics(d, b);
        function __() { this.constructor = d; }
        d.prototype = b === null ? Object.create(b) : (__.prototype = b.prototype, new __());
    };
})();
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Commons/CategoryFascicleViewModelMapper"], function (require, exports, BaseService, CategoryFascicleViewModelMapper) {
    var CategoryFascicleService = /** @class */ (function (_super) {
        __extends(CategoryFascicleService, _super);
        function CategoryFascicleService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        /**
         * Recupero i CategoryFascicle sul classificatore selezionato
         * @param idCategory
         * @param callback
         * @param error
         */
        CategoryFascicleService.prototype.getByIdCategory = function (idCategory, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=Category/EntityShortId eq ".concat(idCategory, ' &$expand=Category,FasciclePeriod,Manager');
            this.getRequest(url, qs, function (response) {
                if (callback && response) {
                    callback(response.value);
                }
            }, error);
        };
        CategoryFascicleService.prototype.getAvailableProcedureCategoryFascicleByCategory = function (idCategory, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=Category/EntityShortId eq " + idCategory + " and FascicleType eq 'Procedure' and DSWEnvironment eq 0&$expand=Category,FasciclePeriod,Manager";
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    if (!response) {
                        callback(null);
                    }
                    callback(response.value);
                }
            }, error);
        };
        /**
         * recupero il category fascicle per UniqueId
         * @param uniqueId
         * @param callback
         * @param error
         */
        CategoryFascicleService.prototype.getById = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=UniqueId eq ".concat(uniqueId, ' &$expand=Category,FasciclePeriod,Manager');
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var mapper = new CategoryFascicleViewModelMapper();
                    if (response) {
                        callback(mapper.Map(response.value[0]));
                    }
                }
            }, error);
        };
        /**
         * recupero i category fascicle periodici per i quali non è ancora attivo un fascicolo periodico
         * @param idCategory
         * @param callback
         * @param error
         */
        CategoryFascicleService.prototype.geAvailablePeriodicCategoryFascicles = function (idCategory, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/CategoryFascicleService.GeAvailablePeriodicCategoryFascicles(idCategory=", idCategory, ")");
            var qs = "";
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var mapper = new CategoryFascicleViewModelMapper();
                    var categoryFascicles = [];
                    if (response) {
                        categoryFascicles = mapper.MapCollection(response.value);
                        callback(categoryFascicles);
                    }
                }
            }, error);
        };
        /**
         * metodo per l'inserimento di un nuovo categoryFascicle
         * @param categoryFascicle
         * @param callback
         * @param error
         */
        CategoryFascicleService.prototype.insertCategoryFascicle = function (categoryFascicle, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(categoryFascicle), callback, error);
        };
        /**
         * metodo per l'aggiornamento
         * @param categoryFascicle
         * @param callback
         * @param error
         */
        CategoryFascicleService.prototype.updateCategoryFascicle = function (categoryFascicle, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(categoryFascicle), callback, error);
        };
        /**
       * Cancellazione di un CategoryFascicle
       * @param model
       * @param callback
       * @param error
       */
        CategoryFascicleService.prototype.deleteCategoryFascicle = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl + "?actionType=DeleteCategoryFascicle";
            this.deleteRequest(url, JSON.stringify(model), callback, error);
        };
        CategoryFascicleService.prototype.isProcedureSecretary = function (idCategory, callback, error) {
            var url = this._configuration.ODATAUrl.concat("/CategoryFascicleService.IsProcedureSecretary(idCategory=" + idCategory + ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    if (!response) {
                        callback(undefined);
                        return;
                    }
                    callback(response.value);
                }
            }, error);
        };
        CategoryFascicleService.prototype.getPeriodicCategoryFascicleByCategory = function (idCategory, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=Category/EntityShortId eq " + idCategory + " and FascicleType eq 'Period'&$expand=Category,FasciclePeriod,Manager";
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    if (!response) {
                        callback(null);
                    }
                    callback(response.value);
                }
            }, error);
        };
        return CategoryFascicleService;
    }(BaseService));
    return CategoryFascicleService;
});
//# sourceMappingURL=CategoryFascicleService.js.map