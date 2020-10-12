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
define(["require", "exports", "App/Models/Commons/CategoryModel", "App/Mappers/Commons/CategoryModelMapper", "App/Services/BaseService", "App/Models/UpdateActionType", "App/Mappers/Commons/CategoryTreeViewModelMapper"], function (require, exports, CategoryModel, CategoryModelMapper, BaseService, UpdateActionType, CategoryTreeViewModelMapper) {
    var CategoryService = /** @class */ (function (_super) {
        __extends(CategoryService, _super);
        /**
         * Costruttore
         */
        function CategoryService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        CategoryService.prototype.getByIdMassimarioScarto = function (idMassimarioScarto, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=MassimarioScarto/UniqueId eq ".concat(idMassimarioScarto);
            this.getRequest(url, qs, function (response) {
                if (callback)
                    callback(response.value);
            }, error);
        };
        CategoryService.prototype.getById = function (categoryId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=EntityShortId eq ".concat(categoryId.toString(), "&$expand=Parent");
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var instance = new CategoryModel();
                    var categoryMapper = new CategoryModelMapper();
                    instance = categoryMapper.Map(response.value[0]);
                    callback(instance);
                }
            }, error);
        };
        CategoryService.prototype.getRolesByCategoryId = function (categoryId, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=EntityShortId eq " + categoryId + "&$expand=CategoryFascicles($expand=CategoryFascicleRights($expand=Role($expand=Father))),MetadataRepository,MassimarioScarto";
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    var instance = new CategoryModel();
                    var categoryMapper = new CategoryModelMapper();
                    instance = categoryMapper.Map(response.value[0]);
                    callback(instance);
                }
            }, error);
        };
        /**
         * Modifica un Classificatore esistente
         * @param model
         * @param callback
         * @param error
         */
        CategoryService.prototype.updateCategory = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            url = url.concat("?actionType=", UpdateActionType.UpdateCategory.toString());
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        CategoryService.prototype.findTreeCategory = function (categoryId, fascicleType, callback, error) {
            var url = this._configuration.ODATAUrl;
            var fascicleTypeParam = null;
            if (fascicleType) {
                fascicleTypeParam = "VecompSoftware.DocSuiteWeb.Entity.Fascicles.FascicleType'" + fascicleType + "'";
            }
            url = url.concat("/CategoryService.FindCategory(idCategory=" + categoryId + ",fascicleType=" + fascicleTypeParam + ")");
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var instance = void 0;
                    if (response && response.value) {
                        var categoryMapper = new CategoryTreeViewModelMapper();
                        instance = categoryMapper.Map(response.value[0]);
                    }
                    callback(instance);
                }
            }, error);
        };
        CategoryService.prototype.findTreeCategories = function (finder, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("/CategoryService.FindCategories(finder=@p0)?@p0=" + JSON.stringify(finder) + "&$orderby=FullCode,Name");
            this.getRequest(url, undefined, function (response) {
                if (callback) {
                    var instances = [];
                    if (response && response.value) {
                        var categoryMapper = new CategoryTreeViewModelMapper();
                        instances = categoryMapper.MapCollection(response.value);
                    }
                    callback(instances);
                }
            }, error);
        };
        CategoryService.prototype.findFascicolableCategory = function (idCategory, callback, error) {
            var url = this._configuration.ODATAUrl;
            url = url.concat("/CategoryService.FindFascicolableCategory(idCategory=" + idCategory + ")");
            this.getRequest(url, undefined, function (response) {
                if (callback) {
                    var instance = void 0;
                    if (response && response.value) {
                        var categoryMapper = new CategoryTreeViewModelMapper();
                        instance = categoryMapper.Map(response.value[0]);
                    }
                    callback(instance);
                }
            }, error);
        };
        CategoryService.prototype.getOnlyFascicolableCategories = function (tenantAOOId, callback, error) {
            var url = this._configuration.ODATAUrl + "?$expand=CategoryFascicles,Parent&$filter=TenantAOO/UniqueId eq " + tenantAOOId + " and(CategoryFascicles/any(cf:cf/FascicleType ne 'SubFascicle'))&$orderby=FullCode,Name";
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var categories = [];
                    if (response && response.value) {
                        var categoryMaper = new CategoryModelMapper();
                        categories = categoryMaper.MapCollection(response.value);
                    }
                    callback(categories);
                }
            }, error);
        };
        CategoryService.prototype.getCategoriesByIds = function (categoryIds, tenantAOOId, callback, error) {
            var url = this._configuration.ODATAUrl + "?$filter=TenantAOO/UniqueId eq " + tenantAOOId + " and EntityShortId in [" + categoryIds.join(",") + "]&$expand=CategoryFascicles,Parent&$orderby=FullCode,Name";
            this.getRequest(url, null, function (response) {
                if (callback) {
                    var categories = [];
                    if (response && response.value) {
                        var categoryMaper = new CategoryModelMapper();
                        categories = categoryMaper.MapCollection(response.value);
                    }
                    callback(categories);
                }
            }, error);
        };
        return CategoryService;
    }(BaseService));
    return CategoryService;
});
//# sourceMappingURL=CategoryService.js.map