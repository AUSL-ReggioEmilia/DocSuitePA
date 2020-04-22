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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/JeepServiceHosts/JeepServiceHostViewModelMapper"], function (require, exports, BaseService, JeepServiceHostViewModelMapper) {
    var JeepServiceHostService = /** @class */ (function (_super) {
        __extends(JeepServiceHostService, _super);
        function JeepServiceHostService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        JeepServiceHostService.prototype.getJeepServiceHosts = function (callback, error) {
            var url = this._configuration.ODATAUrl;
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var viewModelMapper_1 = new JeepServiceHostViewModelMapper();
                    var jeepServiceHosts_1 = [];
                    $.each(response.value, function (i, value) {
                        jeepServiceHosts_1.push(viewModelMapper_1.Map(value));
                    });
                    callback(jeepServiceHosts_1);
                }
                ;
            }, error);
        };
        return JeepServiceHostService;
    }(BaseService));
    return JeepServiceHostService;
});
//# sourceMappingURL=JeepServiceHostService.js.map