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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/PECMails/PECMailBoxConfigurationViewModelMapper"], function (require, exports, BaseService, PECMailBoxConfigurationViewModelMapper) {
    var PECMailBoxConfigurationService = /** @class */ (function (_super) {
        __extends(PECMailBoxConfigurationService, _super);
        function PECMailBoxConfigurationService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        PECMailBoxConfigurationService.prototype.getPECMailBoxConfigurations = function (callback, error) {
            var url = this._configuration.ODATAUrl;
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var viewModelMapper_1 = new PECMailBoxConfigurationViewModelMapper();
                    var pecMailBoxConfigurations_1 = [];
                    $.each(response.value, function (i, value) {
                        pecMailBoxConfigurations_1.push(viewModelMapper_1.Map(value));
                    });
                    callback(pecMailBoxConfigurations_1);
                }
                ;
            }, error);
        };
        return PECMailBoxConfigurationService;
    }(BaseService));
    return PECMailBoxConfigurationService;
});
//# sourceMappingURL=PECMailBoxConfigurationService.js.map