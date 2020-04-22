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
    var ProtocolLinkModelMapper = /** @class */ (function (_super) {
        __extends(ProtocolLinkModelMapper, _super);
        function ProtocolLinkModelMapper() {
            return _super.call(this) || this;
        }
        ProtocolLinkModelMapper.prototype.Map = function (source) {
            var toMap = {};
            if (!source) {
                return null;
            }
            toMap.Number = source.Number;
            toMap.Year = source.Year;
            toMap.UniqueId = source.UniqueId;
            toMap.RegistrationDate = source.RegistrationDate;
            toMap.RegistrationUser = source.RegistrationUser;
            toMap.ProtocolLinked = source.ProtocolLinked;
            return toMap;
        };
        return ProtocolLinkModelMapper;
    }(BaseMapper));
    return ProtocolLinkModelMapper;
});
//# sourceMappingURL=ProtocolLinkModelMapper.js.map