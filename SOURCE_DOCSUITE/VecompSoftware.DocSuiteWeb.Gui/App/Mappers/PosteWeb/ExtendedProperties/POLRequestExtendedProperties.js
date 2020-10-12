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
define(["require", "exports", "App/Mappers/BaseMapper", "./PRETNoticePushResponseMapper", "./PRETNoticeGetStatusResponseMapper", "./ExceptionInfoMapper", "./POLRequestMetaDataMapper"], function (require, exports, BaseMapper, PRETNoticePushResponseMapper, PRETNoticeGetStatusResponseMapper, ExceptionInfoMapper, POLRequestMetaDataMapper) {
    var POLRequestExtendedPropertiesMapper = /** @class */ (function (_super) {
        __extends(POLRequestExtendedPropertiesMapper, _super);
        function POLRequestExtendedPropertiesMapper() {
            return _super.call(this) || this;
        }
        POLRequestExtendedPropertiesMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.PushResponse = (new PRETNoticePushResponseMapper()).Map(source.PushResponse);
            toMap.GetStatus = (new PRETNoticeGetStatusResponseMapper()).Map(source.GetStatus);
            toMap.IsFaulted = source.IsFaulted;
            toMap.ExceptionInfo = (new ExceptionInfoMapper()).Map(source.ExceptionInfo);
            toMap.MetaData = (new POLRequestMetaDataMapper()).Map(source.Metadata);
            return toMap;
        };
        return POLRequestExtendedPropertiesMapper;
    }(BaseMapper));
    return POLRequestExtendedPropertiesMapper;
});
//# sourceMappingURL=POLRequestExtendedProperties.js.map