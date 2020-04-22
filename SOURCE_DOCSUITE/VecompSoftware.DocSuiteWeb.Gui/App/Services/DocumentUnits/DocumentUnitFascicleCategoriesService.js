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
define(["require", "exports", "../BaseService", "../../Mappers/Fascicles/FascicleDocumentUnitCategoryModelMapper"], function (require, exports, BaseService, FascicleDocumentUnitCategoryModelMapper) {
    var DocumentUnitFascicleCategoriesService = /** @class */ (function (_super) {
        __extends(DocumentUnitFascicleCategoriesService, _super);
        function DocumentUnitFascicleCategoriesService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        DocumentUnitFascicleCategoriesService.prototype.getDocumentUnitFascicleCategory = function (idDocumentUnit, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$apply=filter(DocumentUnit/UniqueId eq " + idDocumentUnit + ")/groupby((Category/FullCode,Category/Name))&$orderby=Category/FullCode";
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var mapper = new FascicleDocumentUnitCategoryModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
            }, error);
        };
        return DocumentUnitFascicleCategoriesService;
    }(BaseService));
    return DocumentUnitFascicleCategoriesService;
});
//# sourceMappingURL=DocumentUnitFascicleCategoriesService.js.map