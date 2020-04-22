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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Commons/LocationViewModelMapper"], function (require, exports, BaseService, LocationViewModelMapper) {
    var LocationService = /** @class */ (function (_super) {
        __extends(LocationService, _super);
        function LocationService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        LocationService.prototype.getLocations = function (callback, error) {
            var url = this._configuration.ODATAUrl;
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var viewModelMapper_1 = new LocationViewModelMapper();
                    var locations_1 = [];
                    $.each(response.value, function (i, value) {
                        locations_1.push(viewModelMapper_1.Map(value));
                    });
                    callback(locations_1);
                }
                ;
            }, error);
        };
        return LocationService;
    }(BaseService));
    return LocationService;
});
//# sourceMappingURL=LocationService.js.map