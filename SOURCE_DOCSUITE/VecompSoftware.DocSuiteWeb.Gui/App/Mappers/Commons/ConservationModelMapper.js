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
define(["require", "exports", "App/Mappers/BaseMapper", "App/Models/Commons/ConservationStatusType", "App/Models/Commons/ConservationType"], function (require, exports, BaseMapper, ConservationStatusType, ConservationType) {
    var ConservationModelMapper = /** @class */ (function (_super) {
        __extends(ConservationModelMapper, _super);
        function ConservationModelMapper() {
            return _super.call(this) || this;
        }
        ConservationModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.EntityType = source.EntityType;
            toMap.Message = source.Message;
            toMap.SendDate = source.SendDate;
            toMap.Status = ConservationStatusType[source.Status];
            toMap.Type = ConservationType[source.Type];
            toMap.UniqueId = source.UniqueId;
            toMap.Uri = source.Uri;
            return toMap;
        };
        return ConservationModelMapper;
    }(BaseMapper));
    return ConservationModelMapper;
});
//# sourceMappingURL=ConservationModelMapper.js.map