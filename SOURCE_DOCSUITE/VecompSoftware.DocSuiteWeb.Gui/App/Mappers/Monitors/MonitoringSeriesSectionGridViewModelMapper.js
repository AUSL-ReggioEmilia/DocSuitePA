define(["require", "exports"], function (require, exports) {
    var MonitoringSeriesSectionGridViewModelMapper = /** @class */ (function () {
        function MonitoringSeriesSectionGridViewModelMapper() {
        }
        MonitoringSeriesSectionGridViewModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.Family = source.Family;
            toMap.Series = source.Series;
            toMap.SubSection = source.SubSection;
            toMap.ActivePublished = source.ActivePublished;
            toMap.Inserted = source.Inserted;
            toMap.Published = source.Published;
            toMap.Updated = source.Updated;
            toMap.Canceled = source.Canceled;
            toMap.Retired = source.Retired;
            toMap.LastUpdated = moment(source.LastUpdated).format("DD/MM/YYYY");
            return toMap;
        };
        return MonitoringSeriesSectionGridViewModelMapper;
    }());
    return MonitoringSeriesSectionGridViewModelMapper;
});
//# sourceMappingURL=MonitoringSeriesSectionGridViewModelMapper.js.map