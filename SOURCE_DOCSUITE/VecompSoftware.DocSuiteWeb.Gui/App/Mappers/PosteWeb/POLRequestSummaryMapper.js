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
define(["require", "exports", "App/Mappers/BaseMapper", "./ExtendedProperties/POLRequestExtendedProperties", "App/Helpers/NullSafe", "App/Models/PosteWeb/ExtendedProperties/POLRequestHelper", "App/Models/PosteWeb/StatusColor"], function (require, exports, BaseMapper, POLRequestExtendedPropertiesMapper, NullSafe, POLRequestHelper, StatusColor) {
    var POLRequestSummaryMapper = /** @class */ (function (_super) {
        __extends(POLRequestSummaryMapper, _super);
        function POLRequestSummaryMapper() {
            return _super.call(this) || this;
        }
        POLRequestSummaryMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            if (source.ExtendedProperties) {
                //the source is a POLRequest object
                var extendedPropsJson = JSON.parse(source.ExtendedProperties);
                var extendedPropertiesDeserialized_1 = (new POLRequestExtendedPropertiesMapper()).Map(extendedPropsJson);
                if (extendedPropertiesDeserialized_1) {
                    toMap.Status = NullSafe.Do(function () { return extendedPropertiesDeserialized_1.GetStatus.StatusDescription; }, "");
                    toMap.DisplayColor = POLRequestHelper.DetermineStatusColor(extendedPropertiesDeserialized_1);
                }
            }
            if (!toMap.Status || toMap.Status === "") {
                toMap.Status = source.StatusDescription;
                toMap.DisplayColor = StatusColor.Blue;
            }
            toMap.RegistrationDate = moment(source.RegistrationDate).format("DD/MM/YYYY");
            toMap.RequestUniqueId = source.UniqueId;
            return toMap;
        };
        return POLRequestSummaryMapper;
    }(BaseMapper));
    return POLRequestSummaryMapper;
});
//# sourceMappingURL=POLRequestSummaryMapper.js.map