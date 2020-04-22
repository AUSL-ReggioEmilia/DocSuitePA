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
define(["require", "exports", "App/Services/BaseService", "App/Mappers/Monitors/MonitoringQualityGridViewModelMapper"], function (require, exports, BaseService, MonitoringQualityGridViewModelMapper) {
    var MonitoringQualityService = /** @class */ (function (_super) {
        __extends(MonitoringQualityService, _super);
        function MonitoringQualityService(configuration) {
            var _this = _super.call(this) || this;
            _this._configuration = configuration;
            return _this;
        }
        MonitoringQualityService.prototype.getMonitoringQualitySummary = function (searchFilter, callback, error) {
            var url = this._configuration.ODATAUrl.
                concat("/DocumentSeriesService.GetMonitoringQualitySummary(dateFrom='", searchFilter.dateFrom, "',dateTo='", searchFilter.dateTo, "')");
            this.getRequest(url, null, function (response) {
                if (callback && response) {
                    var viewModelMapper_1 = new MonitoringQualityGridViewModelMapper();
                    var monitoringSeriesSection_1 = [];
                    $.each(response.value, function (i, value) {
                        monitoringSeriesSection_1.push(viewModelMapper_1.Map(value));
                    });
                    callback(monitoringSeriesSection_1);
                }
                ;
            }, error);
        };
        return MonitoringQualityService;
    }(BaseService));
    return MonitoringQualityService;
});
//# sourceMappingURL=MonitoringQualityService.js.map