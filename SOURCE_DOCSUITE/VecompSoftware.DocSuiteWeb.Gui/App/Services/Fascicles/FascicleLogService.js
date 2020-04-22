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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Fascicles/FascicleLogViewModelMapper", "App/Models/ODATAResponseModel"], function (require, exports, BaseService, FascicleLogViewModelMapper, ODATAResponseModel) {
    var FascicleLogService = /** @class */ (function (_super) {
        __extends(FascicleLogService, _super);
        /**
        * Costruttore
        */
        function FascicleLogService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        FascicleLogService.prototype.getFascicleLogs = function (idFascicle, skip, top, callback, error) {
            var url = this._configuration.ODATAUrl.concat("?$filter=Entity/UniqueId eq ", idFascicle, "&$orderby=RegistrationDate&$skip=", skip.toString(), "&$top=", top.toString(), "&$count=true");
            this.getRequest(url, undefined, function (response) {
                if (callback) {
                    var mapper_1 = new FascicleLogViewModelMapper();
                    var fascicleLogs_1 = [];
                    var responseModel = new ODATAResponseModel(response);
                    if (response) {
                        $.each(response.value, function (i, value) {
                            fascicleLogs_1.push(mapper_1.Map(value));
                        });
                        responseModel.value = fascicleLogs_1;
                        callback(responseModel);
                    }
                }
            }, error);
        };
        return FascicleLogService;
    }(BaseService));
    return FascicleLogService;
});
//# sourceMappingURL=FascicleLogService.js.map