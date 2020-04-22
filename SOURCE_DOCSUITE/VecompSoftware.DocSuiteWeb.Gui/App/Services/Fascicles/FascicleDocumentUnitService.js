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
define(["require", "exports", "App/Services/Fascicles/FascicolableBaseService", "App/Mappers/Fascicles/FascicleDocumentUnitModelMapper"], function (require, exports, FascicolableBaseService, FascicleDocumentUnitModelMapper) {
    var FascicleDocumentUnitService = /** @class */ (function (_super) {
        __extends(FascicleDocumentUnitService, _super);
        /**
         * Costruttore
         */
        function FascicleDocumentUnitService(configuration) {
            var _this = _super.call(this, configuration) || this;
            _this._configuration = configuration;
            return _this;
        }
        FascicleDocumentUnitService.prototype.getByDocumentUnitAndFascicle = function (idDocumentUnit, idFascicle, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$filter=Fascicle/UniqueId eq " + idFascicle + " and DocumentUnit/UniqueId eq " + idDocumentUnit + "&$expand=DocumentUnit,Fascicle";
            this.getRequest(url, data, function (response) {
                if (callback) {
                    var mapper = new FascicleDocumentUnitModelMapper();
                    callback(mapper.Map(response.value[0]));
                }
            }, error);
        };
        FascicleDocumentUnitService.prototype.getFascicleListById = function (IdUDS, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$expand=Fascicle&$filter=DocumentUnit/UniqueId eq ".concat(IdUDS);
            this.getRequest(url, data, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        FascicleDocumentUnitService.prototype.countFascicleById = function (IdUDS, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$filter=DocumentUnit/UniqueId eq ".concat(IdUDS);
            url = url.concat(data);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        return FascicleDocumentUnitService;
    }(FascicolableBaseService));
    return FascicleDocumentUnitService;
});
//# sourceMappingURL=FascicleDocumentUnitService.js.map