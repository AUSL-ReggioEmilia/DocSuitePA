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
define(["require", "exports", "App/Mappers/BaseMapper"], function (require, exports, BaseMapper) {
    var POLRequestMetaDataMapper = /** @class */ (function (_super) {
        __extends(POLRequestMetaDataMapper, _super);
        function POLRequestMetaDataMapper() {
            return _super.call(this) || this;
        }
        POLRequestMetaDataMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.PushCalledAt = source.PushCalledAt;
            toMap.LastGetStatusAt = source.LastGetStatusAt;
            toMap.DoneWithGetStatus = source.DoneWithGetStatus;
            toMap.DocumentFromUrlCpfSaved = source.DocumentFromUrlCpfSaved;
            toMap.DocumentFromUrlCpfXmlSaved = source.DocumentFromUrlCpfXmlSaved;
            toMap.PolRequestContactName = source.PolRequestContactName;
            toMap.PolAccountName = source.PolAccountName;
            return toMap;
        };
        return POLRequestMetaDataMapper;
    }(BaseMapper));
    return POLRequestMetaDataMapper;
});
//# sourceMappingURL=POLRequestMetaDataMapper.js.map