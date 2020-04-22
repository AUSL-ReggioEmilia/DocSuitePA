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
define(["require", "exports", "App/Services/BaseService"], function (require, exports, BaseService) {
    var CategoryFascicleRightsService = /** @class */ (function (_super) {
        __extends(CategoryFascicleRightsService, _super);
        function CategoryFascicleRightsService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        CategoryFascicleRightsService.prototype.insertCategoryFascicleRight = function (categoryFascicleRight, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(categoryFascicleRight), callback, error);
        };
        CategoryFascicleRightsService.prototype.getProcedureFascicleRightsByCategory = function (idCategory, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=CategoryFascicle/Category/EntityShortId eq " + idCategory + " and CategoryFascicle/FascicleType eq 'Procedure'&$expand=Role";
            this.getRequest(url, qs, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        CategoryFascicleRightsService.prototype.GetCategoryFascicleRight = function (idCategory, idContainer, callback, error) {
            var url = this._configuration.ODATAUrl;
            var qs = "$filter=CategoryFascicle/Category/EntityShortId eq ".concat(idCategory, " and Container/EntityShortId eq ", idContainer, "");
            this.getRequest(url, qs, function (response) {
                if (response) {
                    callback(response.value);
                }
            }, error);
        };
        CategoryFascicleRightsService.prototype.deleteCategoryFascicleRight = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.deleteRequest(url, JSON.stringify(model), callback, error);
        };
        return CategoryFascicleRightsService;
    }(BaseService));
    return CategoryFascicleRightsService;
});
//# sourceMappingURL=CategoryFascicleRightsService.js.map