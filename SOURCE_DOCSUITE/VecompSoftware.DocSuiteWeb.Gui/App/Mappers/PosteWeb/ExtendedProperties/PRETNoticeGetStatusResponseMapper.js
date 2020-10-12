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
    var PRETNoticeGetStatusResponseMapper = /** @class */ (function (_super) {
        __extends(PRETNoticeGetStatusResponseMapper, _super);
        function PRETNoticeGetStatusResponseMapper() {
            return _super.call(this) || this;
        }
        PRETNoticeGetStatusResponseMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.ReturnCode = source.ReturnCode;
            toMap.Message = source.Message;
            toMap.TimeStamp = source.TimeStamp;
            toMap.FatalErrorType = source.FatalErrorType;
            toMap.UrltNotice = source.UrltNotice;
            toMap.UrltNotice_xml = source.UrltNotice_xml;
            toMap.UrlAccept = source.UrlAccept;
            toMap.UrlCPF = source.UrlCPF;
            toMap.UrlCPF_xml = source.UrlCPF_xml;
            toMap.StatusDescription = source.StatusDescription;
            toMap.StatusCodeType = source.StatusCodeType;
            return toMap;
        };
        return PRETNoticeGetStatusResponseMapper;
    }(BaseMapper));
    return PRETNoticeGetStatusResponseMapper;
});
//# sourceMappingURL=PRETNoticeGetStatusResponseMapper.js.map