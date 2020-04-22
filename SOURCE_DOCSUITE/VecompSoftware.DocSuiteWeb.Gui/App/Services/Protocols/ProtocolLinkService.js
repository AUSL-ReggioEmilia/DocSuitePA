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
define(["require", "exports", "../BaseService", "App/Mappers/Protocols/ProtocolLinkModelMapper"], function (require, exports, BaseService, ProtocolLinkModelMapper) {
    var ProtocolLinkService = /** @class */ (function (_super) {
        __extends(ProtocolLinkService, _super);
        /**
         * Costruttore
         */
        function ProtocolLinkService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            _this._mapper = new ProtocolLinkModelMapper();
            return _this;
        }
        ProtocolLinkService.prototype.getProtocolById = function (Id, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "$expand=ProtocolLinked&$filter=Protocol/UniqueId eq ".concat(Id);
            this.getRequest(url, data, function (response) {
                if (callback) {
                    callback(response.value);
                }
            }, error);
        };
        ProtocolLinkService.prototype.countProtocolsById = function (Id, callback, error) {
            var url = this._configuration.ODATAUrl;
            var data = "/$count?$expand=ProtocolLinked&$filter=Protocol/UniqueId eq ".concat(Id);
            url = url.concat(data);
            this.getRequest(url, null, function (response) {
                if (callback) {
                    callback(response);
                }
            }, error);
        };
        return ProtocolLinkService;
    }(BaseService));
    return ProtocolLinkService;
});
//# sourceMappingURL=ProtocolLinkService.js.map