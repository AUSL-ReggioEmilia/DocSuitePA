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
define(["require", "exports", "App/Mappers/Commons/ConservationModelMapper", "App/Services/BaseService"], function (require, exports, ConservationModelMapper, BaseService) {
    var ConservationService = /** @class */ (function (_super) {
        __extends(ConservationService, _super);
        /**
      * Costruttore
      */
        function ConservationService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        /**
          * Recupero tutti gli idConservation
          * @param callback
          * @param error
          */
        ConservationService.prototype.getById = function (uniqueId, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$filter=UniqueId eq ", uniqueId);
            this.getRequest(url, undefined, function (response) {
                if (callback) {
                    var mapper = new ConservationModelMapper();
                    if (response) {
                        callback(mapper.Map(response.value[0]));
                    }
                }
            }, error);
        };
        return ConservationService;
    }(BaseService));
    return ConservationService;
});
//# sourceMappingURL=ConservationService.js.map