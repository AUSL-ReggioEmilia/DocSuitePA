define(["require", "exports"], function (require, exports) {
    var MonitoringQualityGridViewModelMapper = /** @class */ (function () {
        function MonitoringQualityGridViewModelMapper() {
        }
        MonitoringQualityGridViewModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.DocumentSeries = source.DocumentSeries;
            toMap.Role = source.Role;
            toMap.IdDocumentSeries = source.IdDocumentSeries;
            toMap.IdRole = source.IdRole;
            toMap.Published = source.Published;
            toMap.FromResolution = source.FromResolution;
            toMap.FromProtocol = source.FromProtocol;
            toMap.WithoutLink = source.WithoutLink;
            toMap.WithoutDocument = source.WithoutDocument;
            return toMap;
        };
        return MonitoringQualityGridViewModelMapper;
    }());
    return MonitoringQualityGridViewModelMapper;
});
//# sourceMappingURL=MonitoringQualityGridViewModelMapper.js.map