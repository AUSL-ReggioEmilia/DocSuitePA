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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Fascicles/FascicleDocumentModelMapper"], function (require, exports, BaseService, FascicleDocumentModelMapper) {
    var FascicleDocumentService = /** @class */ (function (_super) {
        __extends(FascicleDocumentService, _super);
        /**
         * Costruttore
         */
        function FascicleDocumentService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        FascicleDocumentService.prototype.insertFascicleDocument = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.postRequest(url, JSON.stringify(model), callback, error);
        };
        FascicleDocumentService.prototype.updateFascicleDocument = function (model, callback, error) {
            var url = this._configuration.WebAPIUrl;
            this.putRequest(url, JSON.stringify(model), callback, error);
        };
        /**
         * Recupera un Fascicolo per ID
         * @param id
         * @param callback
         * @param error
         */
        FascicleDocumentService.prototype.getByFolder = function (idFascicle, idFascicleFolder, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=Fascicle/UniqueId eq ".concat(idFascicle);
            if (idFascicleFolder) {
                data = data.concat(" and FascicleFolder/UniqueId eq ", idFascicleFolder);
            }
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var mapper = new FascicleDocumentModelMapper();
                    callback(mapper.MapCollection(response.value));
                }
            }, error);
        };
        return FascicleDocumentService;
    }(BaseService));
    return FascicleDocumentService;
});
//# sourceMappingURL=FascicleDocumentService.js.map